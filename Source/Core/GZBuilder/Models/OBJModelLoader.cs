using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.GZBuilder.Data;
using CodeImp.DoomBuilder.Rendering;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace CodeImp.DoomBuilder.GZBuilder.Models
{
    internal class OBJModelLoader : ModelLoader
    {
        public static ModelLoadResult Load(ref BoundingBoxSizes bbs, Dictionary<int, string> skins, Stream s, string name)
        {
            ModelLoadResult result = new ModelLoadResult();

            using (var reader = new StreamReader(s, Encoding.ASCII))
            {
                string line;
                int linenum = 1;
                string message;
                int surfaceskinid = 0;
                List<Vector3D> vertices = new List<Vector3D>();
                List<int> faces = new List<int>();
                List<Vector3D> normals = new List<Vector3D>();
                List<Vector2D> texcoords = new List<Vector2D>();
                List<WorldVertex> worldvertices = new List<WorldVertex>();
                List<int> polyindiceslist = new List<int>();

                while ((line = reader.ReadLine()) != null)
                {
                    string[] fields = line.Trim().Split(new[] { ' ', '\t' }, 2, StringSplitOptions.RemoveEmptyEntries);

                    // Empty line
                    if (fields.Length == 0)
                    {
                        linenum++;
                        continue;
                    }

                    // Comment
                    if (fields[0].Trim() == "#")
                    {
                        linenum++;
                        continue;
                    }

                    string keyword = fields[0].Trim();
                    string payload = null;

                    if (fields.Length == 2)
                        payload = fields[1].Trim();

                    switch (keyword)
                    {
                        case "v":
                            Vector3D v = new Vector3D(0, 0, 0);

                            if (OBJParseVertex(payload, ref v, out message))
                                vertices.Add(v);
                            else
                            {
                                result.Errors = String.Format("Error in line {0}: {1}", linenum, message);
                                return result;
                            }

                            break;
                        case "vt":
                            Vector2D t = new Vector2D(0, 0);

                            if (OBJParseTextureCoords(payload, ref t, out message))
                                texcoords.Add(t);
                            else
                            {
                                result.Errors = String.Format("Error in line {0}: {1}", linenum, message);
                                return result;
                            }

                            break;
                        case "vn":
                            Vector3D n = new Vector3D(0, 0, 0);

                            if (OBJParseNormal(payload, ref n, out message))
                                normals.Add(n);
                            else
                            {
                                result.Errors = String.Format("Error in line {0}: {1}", linenum, message);
                                return result;
                            }

                            break;
                        case "f":
                            List<int> fv = new List<int>();
                            List<int> vt = new List<int>();
                            List<int> vn = new List<int>();

                            if (OBJParseFace(payload, ref fv, ref vt, ref vn, out message))
                            {
                                // Sanity check for vertices
                                for (int i = 0; i < fv.Count; i++)
                                    if (fv[i] != -1 && fv[i] >= vertices.Count)
                                    {
                                        result.Errors = String.Format("Error in line {0}: vertex {1} does not exist", linenum, fv[i] + 1);
                                        return result;
                                    }

                                // Sanity check for texture coordinates
                                for (int i = 0; i < vt.Count; i++)
                                    if (vt[i] != -1 && vt[i] >= texcoords.Count)
                                    {
                                        result.Errors = String.Format("Error in line {0}: texture coordinate {1} does not exist", linenum, vt[i] + 1);
                                        return result;
                                    }

                                // Sanity check for normals
                                for (int i = 0; i < vn.Count; i++)
                                    if (vn[i] != -1 && vn[i] >= normals.Count)
                                    {
                                        result.Errors = String.Format("Error in line {0}: vertex {1} does not exist", linenum, vn[i] + 1);
                                        return result;
                                    }

                                int[] seq;

                                // If the face is a quad split it into two triangles
                                if (fv.Count == 3)
                                    seq = new int[] { 0, 1, 2 };
                                else
                                    seq = new int[] { 0, 1, 2, 0, 2, 3 };

                                for (int i = 0; i < seq.Length; i++)
                                {
                                    WorldVertex wc = new WorldVertex(vertices[fv[seq[i]]]);

                                    if (vt[seq[i]] != -1)
                                    {
                                        wc.u = (float)texcoords[vt[seq[i]]].x;
                                        wc.v = (float)texcoords[vt[seq[i]]].y;
                                    }

                                    if (vn[seq[i]] != -1)
                                    {
                                        wc.nx = (float)normals[vn[seq[i]]].x;
                                        wc.ny = (float)normals[vn[seq[i]]].y;
                                        wc.nz = (float)normals[vn[seq[i]]].z;
                                    }

                                    BoundingBoxTools.UpdateBoundingBoxSizes(ref bbs, wc);

                                    worldvertices.Add(wc);
                                    polyindiceslist.Add(polyindiceslist.Count);
                                }
                            }
                            else
                            {
                                result.Errors = String.Format("Error in line {0}: {1}", linenum, message);
                                return result;
                            }

                            break;
                        case "usemtl":
                            // If there's a new texture defined create a mesh from the current faces and
                            // start a gather new faces for the next mesh
                            if (worldvertices.Count > 0)
                            {
                                CreateMesh(ref result, worldvertices, polyindiceslist);
                                worldvertices.Clear();
                                polyindiceslist.Clear();
                            }

                            // Add texture name. It might be in quotes, so remove them.
                            // See https://github.com/jewalky/UltimateDoomBuilder/issues/758
                            if (fields.Length >= 2)
                                result.Skins.Add(fields[1].Replace("\"", ""));

                            surfaceskinid++;
                            break;
                        case "": // Empty line
                        case "#": // Line is a comment
                        case "s": // Smooth
                        case "g": // Group
                        case "o": // Object
                        default:
                            break;
                    }

                    linenum++;
                }

                CreateMesh(ref result, worldvertices, polyindiceslist);

                // Overwrite internal textures with SurfaceSkin definitions if necessary
                if (skins != null)
                {
                    foreach (KeyValuePair<int, string> group in skins)
                    {
                        // Add dummy skins if necessary
                        while (result.Skins.Count <= group.Key)
                            result.Skins.Add(String.Empty);

                        result.Skins[group.Key] = group.Value;
                    }
                }
            }

            return result;
        }

        private static bool OBJParseVertex(string payload, ref Vector3D v, out string message)
        {
            if (String.IsNullOrEmpty(payload))
            {
                message = "no arguments given";
                return false;
            }

            string[] fields = payload.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

            if (fields.Length < 3)
            {
                message = "too few arguments";
                return false;
            }

            try
            {
                v.x = float.Parse(fields[0], CultureInfo.InvariantCulture);
                v.z = float.Parse(fields[1], CultureInfo.InvariantCulture);
                v.y = -float.Parse(fields[2], CultureInfo.InvariantCulture);


                // Prepare to fix rotation angle
                double angleOfsetCos = Math.Cos(-Angle2D.PIHALF);
                double angleOfsetSin = Math.Sin(-Angle2D.PIHALF);

                // Fix rotation angle
                double rx = angleOfsetCos * v.x - angleOfsetSin * v.y;
                double ry = angleOfsetSin * v.x + angleOfsetCos * v.y;
                v.x = rx;
                v.y = ry;
            }
            catch (FormatException)
            {
                message = "field is not a float";
                return false;
            }

            message = "";
            return true;
        }

        private static bool OBJParseTextureCoords(string payload, ref Vector2D t, out string message)
        {
            if (String.IsNullOrEmpty(payload))
            {
                message = "no arguments given";
                return false;
            }

            string[] fields = payload.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

            if (fields.Length < 2)
            {
                message = "too few arguments";
                return false;
            }

            try
            {
                t.x = float.Parse(fields[0], CultureInfo.InvariantCulture);

                if (fields.Length >= 2)
                    t.y = 1.0f - float.Parse(fields[1], CultureInfo.InvariantCulture);
                else
                    t.y = 1.0f;
            }
            catch (FormatException)
            {
                message = "field is not a float";
                return false;
            }

            message = "";
            return true;
        }

        private static bool OBJParseNormal(string payload, ref Vector3D normal, out string message)
        {
            if (String.IsNullOrEmpty(payload))
            {
                message = "no arguments given";
                return false;
            }

            string[] fields = payload.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

            if (fields.Length < 3)
            {
                message = "too few arguments";
                return false;
            }

            try
            {
                normal.x = float.Parse(fields[0], CultureInfo.InvariantCulture);
                normal.y = float.Parse(fields[1], CultureInfo.InvariantCulture);
                normal.z = float.Parse(fields[2], CultureInfo.InvariantCulture);
            }
            catch (FormatException)
            {
                message = "field is not a float";
                return false;
            }

            message = "";
            return true;
        }

        private static bool OBJParseFace(string payload, ref List<int> face, ref List<int> texcoords, ref List<int> normals, out string message)
        {
            if (String.IsNullOrEmpty(payload))
            {
                message = "no arguments given";
                return false;
            }

            string[] fields = payload.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

            if (fields.Length < 3)
            {
                message = "too few arguments";
                return false;
            }

            if (fields.Length > 4)
            {
                message = "faces with more than 4 sides are not supported";
                return false;
            }

            try
            {
                for (int i = 0; i < fields.Length; i++)
                {
                    string[] vertexdata = fields[i].Split('/');

                    face.Add(int.Parse(vertexdata[0], CultureInfo.InvariantCulture) - 1);

                    if (vertexdata.Length > 1 && vertexdata[1] != "")
                        texcoords.Add(int.Parse(vertexdata[1], CultureInfo.InvariantCulture) - 1);
                    else
                        texcoords.Add(-1);

                    if (vertexdata.Length > 2 && vertexdata[2] != "")
                        normals.Add(int.Parse(vertexdata[2], CultureInfo.InvariantCulture) - 1);
                    else
                        normals.Add(-1);
                }
            }
            catch (FormatException)
            {
                message = "field is not an integer";
                return false;
            }

            message = "";
            return true;
        }
    }
}
