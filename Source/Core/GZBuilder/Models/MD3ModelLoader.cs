using CodeImp.DoomBuilder.GZBuilder.Data;
using CodeImp.DoomBuilder.Rendering;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CodeImp.DoomBuilder.GZBuilder.Models
{
    internal class MD3ModelLoader : ModelLoader
    {
        internal static ModelLoadResult Load(ref BoundingBoxSizes bbs, Dictionary<int, string> skins, Stream s, int frame)
        {
            long start = s.Position;
            ModelLoadResult result = new ModelLoadResult();

            using (var br = new BinaryReader(s, Encoding.ASCII))
            {
                string magic = ReadString(br, 4);
                if (magic != "IDP3")
                {
                    result.Errors = "unknown header: expected \"IDP3\", but got \"" + magic + "\"";
                    return result;
                }

                int modelVersion = br.ReadInt32();
                if (modelVersion != 15) //MD3 version. Must be equal to 15
                {
                    result.Errors = "expected MD3 version 15, but got " + modelVersion;
                    return result;
                }

                s.Position += 76;
                int numSurfaces = br.ReadInt32();
                s.Position += 12;
                int ofsSurfaces = br.ReadInt32();

                s.Position = ofsSurfaces + start;

                List<int> polyIndecesList = new List<int>();
                List<WorldVertex> vertList = new List<WorldVertex>();

                Dictionary<string, List<List<int>>> polyIndecesListsPerTexture = new Dictionary<string, List<List<int>>>(StringComparer.Ordinal);
                Dictionary<string, List<WorldVertex>> vertListsPerTexture = new Dictionary<string, List<WorldVertex>>(StringComparer.Ordinal);
                Dictionary<string, List<int>> vertexOffsets = new Dictionary<string, List<int>>(StringComparer.Ordinal);
                bool useskins = false;

                for (int c = 0; c < numSurfaces; c++)
                {
                    string skin = "";
                    string error = ReadSurface(ref bbs, ref skin, br, polyIndecesList, vertList, frame);

                    if (!string.IsNullOrEmpty(error))
                    {
                        result.Errors = error;
                        return result;
                    }

                    // Pick a skin to use
                    if (skins == null)
                    {
                        // skins is null when Skin MODELDEF property is set
                        skin = string.Empty;
                    }
                    else if (skins.ContainsKey(c))
                    {
                        // Overrtide surface skin with SurfaceSkin MODELDEF property
                        skin = skins[c];
                    }

                    if (!string.IsNullOrEmpty(skin))
                    {
                        useskins = true;

                        if (polyIndecesListsPerTexture.ContainsKey(skin))
                        {
                            polyIndecesListsPerTexture[skin].Add(polyIndecesList);
                            vertListsPerTexture[skin].AddRange(vertList.ToArray());
                            vertexOffsets[skin].Add(vertList.Count);
                        }
                        else
                        {
                            polyIndecesListsPerTexture.Add(skin, new List<List<int>> { polyIndecesList });
                            vertListsPerTexture.Add(skin, vertList);
                            vertexOffsets.Add(skin, new List<int> { vertList.Count });
                        }

                        //reset lists
                        polyIndecesList = new List<int>();
                        vertList = new List<WorldVertex>();
                    }
                }

                if (!useskins)
                {
                    //create mesh
                    CreateMesh(ref result, vertList, polyIndecesList);
                    result.Skins.Add("");
                }
                else
                {
                    //create a mesh for each surface texture
                    foreach (KeyValuePair<string, List<List<int>>> group in polyIndecesListsPerTexture)
                    {
                        polyIndecesList = new List<int>();
                        int offset = 0;

                        //collect indices, fix vertex offsets
                        for (int i = 0; i < group.Value.Count; i++)
                        {
                            if (i > 0)
                            {
                                //TODO: Damn I need to rewrite all of this stuff from scratch...
                                offset += vertexOffsets[group.Key][i - 1];
                                for (int c = 0; c < group.Value[i].Count; c++)
                                    group.Value[i][c] += offset;
                            }
                            polyIndecesList.AddRange(group.Value[i].ToArray());
                        }

                        CreateMesh(ref result, vertListsPerTexture[group.Key], polyIndecesList);
                        result.Skins.Add(group.Key.ToLowerInvariant());
                    }
                }
            }

            return result;
        }

        private static string ReadSurface(ref BoundingBoxSizes bbs, ref string skin, BinaryReader br, List<int> polyIndecesList, List<WorldVertex> vertList, int frame)
        {
            int vertexOffset = vertList.Count;
            long start = br.BaseStream.Position;

            string magic = ReadString(br, 4);
            if (magic != "IDP3") return "error while reading surface. Unknown header: expected \"IDP3\", but got \"" + magic + "\"";

            string name = ReadString(br, 64);
            int flags = br.ReadInt32();
            int numFrames = br.ReadInt32(); //Number of animation frames. This should match NUM_FRAMES in the MD3 header.
            int numShaders = br.ReadInt32(); //Number of Shader objects defined in this Surface, with a limit of MD3_MAX_SHADERS. Current value of MD3_MAX_SHADERS is 256.
            int numVerts = br.ReadInt32(); //Number of Vertex objects defined in this Surface, up to MD3_MAX_VERTS. Current value of MD3_MAX_VERTS is 4096.
            int numTriangles = br.ReadInt32(); //Number of Triangle objects defined in this Surface, maximum of MD3_MAX_TRIANGLES. Current value of MD3_MAX_TRIANGLES is 8192.
            int ofsTriangles = br.ReadInt32(); //Relative offset from SURFACE_START where the list of Triangle objects starts.
            int ofsShaders = br.ReadInt32();
            int ofsST = br.ReadInt32(); //Relative offset from SURFACE_START where the list of ST objects (s-t texture coordinates) starts.
            int ofsNormal = br.ReadInt32(); //Relative offset from SURFACE_START where the list of Vertex objects (X-Y-Z-N vertices) starts.
            int ofsEnd = br.ReadInt32(); //Relative offset from SURFACE_START to where the Surface object ends.

            // Sanity check
            if (frame < 0 || frame >= numFrames)
            {
                return "frame " + frame + " is outside of model's frame range [0.." + (numFrames - 1) + "]";
            }

            // Polygons
            if (start + ofsTriangles != br.BaseStream.Position)
                br.BaseStream.Position = start + ofsTriangles;

            for (int i = 0; i < numTriangles * 3; i++)
                polyIndecesList.Add(vertexOffset + br.ReadInt32());

            // Shaders
            if (start + ofsShaders != br.BaseStream.Position)
                br.BaseStream.Position = start + ofsShaders;

            skin = ReadString(br, 64); //we are interested only in the first one

            // Vertices
            if (start + ofsST != br.BaseStream.Position)
                br.BaseStream.Position = start + ofsST;

            for (int i = 0; i < numVerts; i++)
            {
                WorldVertex v = new WorldVertex();
                v.c = -1; //white
                v.u = br.ReadSingle();
                v.v = br.ReadSingle();

                vertList.Add(v);
            }

            // Positions and normals
            long vertoffset = start + ofsNormal + numVerts * 8 * frame; // The length of Vertex struct is 8 bytes
            if (br.BaseStream.Position != vertoffset) br.BaseStream.Position = vertoffset;

            for (int i = vertexOffset; i < vertexOffset + numVerts; i++)
            {
                WorldVertex v = vertList[i];

                //read vertex
                v.y = -(float)br.ReadInt16() / 64;
                v.x = (float)br.ReadInt16() / 64;
                v.z = (float)br.ReadInt16() / 64;

                //bounding box
                BoundingBoxTools.UpdateBoundingBoxSizes(ref bbs, v);

                var lat = br.ReadByte() * (2 * Math.PI) / 255.0;
                var lng = br.ReadByte() * (2 * Math.PI) / 255.0;

                v.nx = (float)(Math.Sin(lng) * Math.Sin(lat));
                v.ny = -(float)(Math.Cos(lng) * Math.Sin(lat));
                v.nz = (float)(Math.Cos(lat));

                vertList[i] = v;
            }

            if (start + ofsEnd != br.BaseStream.Position)
                br.BaseStream.Position = start + ofsEnd;
            return "";
        }
    }
}
