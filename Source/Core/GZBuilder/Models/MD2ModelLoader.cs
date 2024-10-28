using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.GZBuilder.Data;
using CodeImp.DoomBuilder.Rendering;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CodeImp.DoomBuilder.GZBuilder.Models
{
    internal class MD2ModelLoader : ModelLoader
    {
        public static ModelLoadResult Load(ref BoundingBoxSizes bbs, Stream s, int frame, string framename)
        {
            long start = s.Position;
            ModelLoadResult result = new ModelLoadResult();

            using (var br = new BinaryReader(s, Encoding.ASCII))
            {
                string magic = ReadString(br, 4);
                if (magic != "IDP2") //magic number: "IDP2"
                {
                    result.Errors = "unknown header: expected \"IDP2\", but got \"" + magic + "\"";
                    return result;
                }

                int modelVersion = br.ReadInt32();
                if (modelVersion != 8) //MD2 version. Must be equal to 8
                {
                    result.Errors = "expected MD3 version 15, but got " + modelVersion;
                    return result;
                }

                int texWidth = br.ReadInt32();
                int texHeight = br.ReadInt32();
                int framesize = br.ReadInt32(); // Size of one frame in bytes
                s.Position += 4; //Number of textures
                int num_verts = br.ReadInt32(); //Number of vertices
                int num_uv = br.ReadInt32(); //The number of UV coordinates in the model
                int num_tris = br.ReadInt32(); //Number of triangles
                s.Position += 4; //Number of OpenGL commands
                int num_frames = br.ReadInt32(); //Total number of frames

                // Sanity checks
                if (frame < 0 || frame >= num_frames)
                {
                    result.Errors = "frame " + frame + " is outside of model's frame range [0.." + (num_frames - 1) + "]";
                    return result;
                }

                s.Position += 4; //Offset to skin names (each skin name is an unsigned char[64] and are null terminated)
                int ofs_uv = br.ReadInt32();//Offset to s-t texture coordinates
                int ofs_tris = br.ReadInt32(); //Offset to triangles
                int ofs_animFrame = br.ReadInt32(); //An offset to the first animation frame

                List<int> polyIndecesList = new List<int>();
                List<int> uvIndecesList = new List<int>();
                List<Vector2f> uvCoordsList = new List<Vector2f>();
                List<WorldVertex> vertList = new List<WorldVertex>();

                // Polygons
                s.Position = ofs_tris + start;

                for (int i = 0; i < num_tris; i++)
                {
                    polyIndecesList.Add(br.ReadUInt16());
                    polyIndecesList.Add(br.ReadUInt16());
                    polyIndecesList.Add(br.ReadUInt16());

                    uvIndecesList.Add(br.ReadUInt16());
                    uvIndecesList.Add(br.ReadUInt16());
                    uvIndecesList.Add(br.ReadUInt16());
                }

                // UV coords
                s.Position = ofs_uv + start;

                for (int i = 0; i < num_uv; i++)
                    uvCoordsList.Add(new Vector2f((float)br.ReadInt16() / texWidth, (float)br.ReadInt16() / texHeight));

                // Frames
                // Find correct frame
                if (!string.IsNullOrEmpty(framename))
                {
                    // Skip frames untill frame name matches
                    bool framefound = false;
                    for (int i = 0; i < num_frames; i++)
                    {
                        s.Position = ofs_animFrame + start + i * framesize;
                        s.Position += 24; // Skip scale and translate
                        string curframename = ReadString(br, 16).ToLowerInvariant();

                        if (curframename == framename)
                        {
                            // Step back so scale and translate can be read
                            s.Position -= 40;
                            framefound = true;
                            break;
                        }
                    }

                    // No dice? Bail out!
                    if (!framefound)
                    {
                        result.Errors = "unable to find frame \"" + framename + "\"!";
                        return result;
                    }
                }
                else
                {
                    // If we have frame number, we can go directly to target frame
                    s.Position = ofs_animFrame + start + frame * framesize;
                }

                Vector3f scale = new Vector3f(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                Vector3f translate = new Vector3f(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());

                s.Position += 16; // Skip frame name

                // Prepare to fix rotation angle
                float angleOfsetCos = (float)Math.Cos(-Angle2D.PIHALF);
                float angleOfsetSin = (float)Math.Sin(-Angle2D.PIHALF);

                //verts
                for (int i = 0; i < num_verts; i++)
                {
                    WorldVertex v = new WorldVertex();

                    v.x = (br.ReadByte() * scale.X + translate.X);
                    v.y = (br.ReadByte() * scale.Y + translate.Y);
                    v.z = (br.ReadByte() * scale.Z + translate.Z);

                    // Fix rotation angle
                    float rx = angleOfsetCos * v.x - angleOfsetSin * v.y;
                    float ry = angleOfsetSin * v.x + angleOfsetCos * v.y;
                    v.y = ry;
                    v.x = rx;

                    vertList.Add(v);
                    s.Position += 1; //vertex normal
                }

                for (int i = 0; i < polyIndecesList.Count; i++)
                {
                    WorldVertex v = vertList[polyIndecesList[i]];

                    //bounding box
                    BoundingBoxTools.UpdateBoundingBoxSizes(ref bbs, new WorldVertex(v.y, v.x, v.z));

                    //uv
                    float tu = uvCoordsList[uvIndecesList[i]].X;
                    float tv = uvCoordsList[uvIndecesList[i]].Y;

                    //uv-coordinates already set?
                    if (v.c == -1 && (v.u != tu || v.v != tv))
                    {
                        //add a new vertex
                        vertList.Add(new WorldVertex(v.x, v.y, v.z, -1, tu, tv));
                        polyIndecesList[i] = vertList.Count - 1;
                    }
                    else
                    {
                        v.u = tu;
                        v.v = tv;
                        v.c = -1; //set color to white

                        //return to proper place
                        vertList[polyIndecesList[i]] = v;
                    }
                }

                //mesh
                Mesh mesh = new Mesh(General.Map.Graphics, vertList.ToArray(), polyIndecesList.ToArray());

                //store in result
                result.Meshes.Add(mesh);
                result.Skins.Add(""); //no skin support for MD2
            }

            return result;
        }
    }
}
