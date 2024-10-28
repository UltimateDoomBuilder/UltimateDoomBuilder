using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.GZBuilder.Data;
using CodeImp.DoomBuilder.Rendering;
using System.Collections.Generic;
using System.IO;

namespace CodeImp.DoomBuilder.GZBuilder.Models
{
    internal class UnrealModelLoader : ModelLoader
    {
        internal static ModelLoadResult Load(ref BoundingBoxSizes bbs, Dictionary<int, string> skins, Stream s, int frame, string filename, List<DataReader> containers)
        {
            Stream stream_d;
            Stream stream_a;

            if (filename.IndexOf("_d.3d") == filename.Length - 5)
            {
                string filename_a = filename.Replace("_d.3d", "_a.3d");
                stream_d = s;
                stream_a = LoadFile(containers, filename_a, true);
                if (stream_a == null)
                {
                    General.ErrorLogger.Add(ErrorType.Error, "Error while loading \"" + filename + "\": unable to find corresponding \"_a.3d\" file.");
                    return null;
                }
            }
            else
            {
                string filename_d = filename.Replace("_a.3d", "_d.3d");
                stream_a = s;
                stream_d = LoadFile(containers, filename_d, true);
                if (stream_d == null)
                {
                    General.ErrorLogger.Add(ErrorType.Error, "Error while loading \"" + filename + "\": unable to find corresponding \"_d.3d\" file.");
                    return null;
                }
            }

            ModelLoadResult result = new ModelLoadResult();
            BinaryReader br_d = new BinaryReader(stream_d);
            BinaryReader br_a = new BinaryReader(stream_a);

            // read d3d header
            uint d3d_numpolys = br_d.ReadUInt16();
            uint d3d_numverts = br_d.ReadUInt16();
            stream_d.Position += 44; // bogusrot, bogusframe, bogusnorm[3], fixscale, unused[3], padding[12]

            long start_d = stream_d.Position;

            // read a3d header
            uint a3d_numframes = br_a.ReadUInt16();
            uint a3d_framesize = br_a.ReadUInt16();

            long start_a = stream_a.Position;

            // Sanity check
            if (frame < 0 || frame >= a3d_numframes)
            {
                result.Errors = "frame " + frame + " is outside of model's frame range [0.." + (a3d_numframes - 1) + "]";
                return result;
            }

            // check for deus ex format
            bool isdeusex = false;
            if ((a3d_framesize / d3d_numverts) == 8) isdeusex = true;

            // read vertices
            WorldVertex[] vertices = new WorldVertex[d3d_numverts];
            for (uint i = 0; i < d3d_numverts; i++)
            {
                WorldVertex Vert = new WorldVertex();
                if (isdeusex)
                {
                    stream_a.Position = start_a + (i + frame * d3d_numverts) * 8;
                    int vx = br_a.ReadInt16();
                    int vy = br_a.ReadInt16();
                    int vz = br_a.ReadInt16();
                    Vert.y = -vx;
                    Vert.z = vz;
                    Vert.x = -vy;
                }
                else
                {
                    stream_a.Position = start_a + (i + frame * d3d_numverts) * 4;
                    int v_uint = br_a.ReadInt32();
                    Vert.y = -UnpackUVertex(v_uint, 0);
                    Vert.z = UnpackUVertex(v_uint, 2);
                    Vert.x = -UnpackUVertex(v_uint, 1);
                }
                vertices[i] = Vert;
            }

            // read polygons
            //int minverthack = 0;
            //int minvert = 2147483647;
            UE1Poly[] polys = new UE1Poly[d3d_numpolys];
            int[] polyindexlist = new int[d3d_numpolys * 3];
            for (uint i = 0; i < d3d_numpolys; i++)
            {
                //
                stream_d.Position = start_d + 16 * i;
                polys[i].V = new int[3];
                polys[i].S = new float[3];
                polys[i].T = new float[3];
                bool brokenpoly = false;
                for (int j = 0; j < 3; j++)
                {
                    polyindexlist[i * 3 + j] = polys[i].V[j] = br_d.ReadInt16();
                    if (polys[i].V[j] >= vertices.Length || polys[i].V[j] < 0)
                        brokenpoly = true;
                }

                // Resolves polygons that reference out-of-bounds vertices by simply making them null size.
                // This is easier than changing array to dynamically sized list.
                if (brokenpoly)
                {
                    polys[i].V[0] = 0;
                    polys[i].V[1] = 0;
                    polys[i].V[2] = 0;
                }

                polys[i].Type = br_d.ReadByte();
                stream_d.Position += 1; // color
                for (int j = 0; j < 3; j++)
                {
                    byte u = br_d.ReadByte();
                    byte v = br_d.ReadByte();
                    polys[i].S[j] = u / 255f;
                    polys[i].T[j] = v / 255f;
                }
                polys[i].TexNum = br_d.ReadByte();
            }

            // calculate poly normals
            for (uint i = 0; i < d3d_numpolys; i++)
            {
                Vector3D[] dir = new Vector3D[2];
                Vector3D norm;
                dir[0].x = vertices[polys[i].V[1]].x - vertices[polys[i].V[0]].x;
                dir[0].y = vertices[polys[i].V[1]].y - vertices[polys[i].V[0]].y;
                dir[0].z = vertices[polys[i].V[1]].z - vertices[polys[i].V[0]].z;
                dir[1].x = vertices[polys[i].V[2]].x - vertices[polys[i].V[0]].x;
                dir[1].y = vertices[polys[i].V[2]].y - vertices[polys[i].V[0]].y;
                dir[1].z = vertices[polys[i].V[2]].z - vertices[polys[i].V[0]].z;
                norm.x = dir[0].y * dir[1].z - dir[0].z * dir[1].y;
                norm.y = dir[0].z * dir[1].x - dir[0].x * dir[1].z;
                norm.z = dir[0].x * dir[1].y - dir[0].y * dir[1].x;
                polys[i].Normal = norm.GetNormal();
            }

            // calculate vertex normals
            for (uint i = 0; i < d3d_numverts; i++)
            {
                Vector3D nsum = new Vector3D(0, 0, 0);
                int total = 0;
                for (uint j = 0; j < d3d_numpolys; j++)
                {
                    if ((polys[j].V[0] != i) && (polys[j].V[1] != i) && (polys[j].V[2] != i)) continue;
                    nsum.x += polys[j].Normal.x;
                    nsum.y += polys[j].Normal.y;
                    nsum.z += polys[j].Normal.z;
                    total++;
                }
                vertices[i].nx = (float)-nsum.x / total;
                vertices[i].ny = (float)-nsum.y / total;
                vertices[i].nz = (float)-nsum.z / total;
            }

            List<int> exGroups = new List<int>();
            Dictionary<int, int> textureGroupRemap = new Dictionary<int, int>();
            for (int i = 0; i < polys.Length; i++)
            {
                if (exGroups.Contains(polys[i].TexNum))
                    continue;
                if (exGroups.Count == 0 ||
                    polys[i].TexNum <= exGroups[0])
                    exGroups.Insert(0, polys[i].TexNum);
                else if (exGroups.Count == 0 ||
                         polys[i].TexNum >= exGroups[exGroups.Count - 1])
                    exGroups.Add(polys[i].TexNum);
            }

            for (int i = 0; i < exGroups.Count; i++)
                textureGroupRemap[exGroups[i]] = i;

            if (skins == null)
            {
                List<WorldVertex> out_verts = new List<WorldVertex>();
                List<int> out_polys = new List<int>();

                for (int i = 0; i < polys.Length; i++)
                {
                    if ((polys[i].Type & 0x08) != 0)
                        continue;
                    for (int j = 0; j < 3; j++)
                    {
                        WorldVertex vx = vertices[polys[i].V[j]];
                        vx.u = polys[i].S[j];
                        vx.v = polys[i].T[j];
                        if ((polys[i].Type & 0x20) != 0)
                        {
                            vx.nx = (float)polys[i].Normal.x;
                            vx.ny = (float)polys[i].Normal.y;
                            vx.nz = (float)polys[i].Normal.z;
                        }
                        out_polys.Add(out_verts.Count);
                        out_verts.Add(vx);
                    }
                }

                CreateMesh(ref result, out_verts, out_polys);
                result.Skins.Add("");
            }
            else
            {
                for (int k = 0; k < exGroups.Count; k++)
                {
                    List<WorldVertex> out_verts = new List<WorldVertex>();
                    List<int> out_polys = new List<int>();

                    for (int i = 0; i < polys.Length; i++)
                    {
                        if ((polys[i].Type & 0x08) != 0)
                            continue;

                        if (textureGroupRemap[polys[i].TexNum] != k)
                            continue;

                        for (int j = 0; j < 3; j++)
                        {
                            WorldVertex vx = vertices[polys[i].V[j]];
                            vx.u = polys[i].S[j];
                            vx.v = polys[i].T[j];
                            if ((polys[i].Type & 0x20) != 0)
                            {
                                vx.nx = (float)polys[i].Normal.x;
                                vx.ny = (float)polys[i].Normal.y;
                                vx.nz = (float)polys[i].Normal.z;
                            }
                            out_polys.Add(out_verts.Count);
                            out_verts.Add(vx);
                        }
                    }

                    CreateMesh(ref result, out_verts, out_polys);
                    result.Skins.Add(skins.ContainsKey(k) ? skins[k].ToLowerInvariant() : string.Empty);
                }
            }

            return result;
        }

        // there is probably better way to emulate 16-bit cast, but this was easiest for me at 3am
        private static int PadInt16(int n)
        {
            if (n > 32767)
                return -(65536 - n);
            return n;
        }

        private static float UnpackUVertex(int n, int c)
        {
            switch (c)
            {
                case 0:
                    return PadInt16((n & 0x7ff) << 5) / 128f;
                case 1:
                    return PadInt16((((int)n >> 11) & 0x7ff) << 5) / 128f;
                case 2:
                    return PadInt16((((int)n >> 22) & 0x3ff) << 6) / 128f;
                default:
                    return 0f;
            }
        }

        private struct UE1Poly
        {
            public int[] V;
            public float[] S;
            public float[] T;
            public int TexNum, Type;
            public Vector3D Normal;
        }
    }
}
