using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.GZBuilder.Data;
using CodeImp.DoomBuilder.Rendering;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Text;

namespace CodeImp.DoomBuilder.GZBuilder.Models
{
    internal class KVXModelLoader : ModelLoader
    {
        public static void Load(ModelData mde, Stream stream)
        {
            PixelColor[] palette = new PixelColor[256];
            List<WorldVertex> verts = new List<WorldVertex>();
            List<int> indices = new List<int>();
            Dictionary<long, int> verthashes = new Dictionary<long, int>();
            int xsize, ysize, zsize;
            int facescount = 0;
            Vector3D pivot;

            using (BinaryReader reader = new BinaryReader(stream, Encoding.ASCII))
            {
                reader.ReadInt32(); //numbytes, we don't use that
                xsize = reader.ReadInt32();
                ysize = reader.ReadInt32();
                zsize = reader.ReadInt32();

                pivot = new Vector3D();
                pivot.x = reader.ReadInt32() / 256f;
                pivot.y = reader.ReadInt32() / 256f;
                pivot.z = reader.ReadInt32() / 256f;

                //read offsets
                int[] xoffset = new int[xsize + 1]; //why is it xsize + 1, not xsize?..
                short[,] xyoffset = new short[xsize, ysize + 1]; //why is it ysize + 1, not ysize?..

                for (int i = 0; i < xoffset.Length; i++)
                {
                    xoffset[i] = reader.ReadInt32();
                }

                for (int x = 0; x < xsize; x++)
                {
                    for (int y = 0; y < ysize + 1; y++)
                    {
                        xyoffset[x, y] = reader.ReadInt16();
                    }
                }

                //read slabs
                List<int> offsets = new List<int>(xsize * ysize);
                for (int x = 0; x < xsize; x++)
                {
                    for (int y = 0; y < ysize; y++)
                    {
                        offsets.Add(xoffset[x] + xyoffset[x, y] + 28); //for some reason offsets are counted from start of xoffset[]...
                    }
                }

                int counter = 0;
                int slabsEnd = (int)(reader.BaseStream.Length - 768);

                //read palette
                if (!mde.OverridePalette)
                {
                    reader.BaseStream.Position = slabsEnd;
                    for (int i = 0; i < 256; i++)
                    {
                        byte r = (byte)(reader.ReadByte() * 4);
                        byte g = (byte)(reader.ReadByte() * 4);
                        byte b = (byte)(reader.ReadByte() * 4);
                        palette[i] = new PixelColor(255, r, g, b);
                    }
                }
                else
                {
                    for (int i = 0; i < 256; i++)
                    {
                        palette[i] = General.Map.Data.Palette[i];
                    }
                }

                for (int x = 0; x < xsize; x++)
                {
                    for (int y = 0; y < ysize; y++)
                    {
                        reader.BaseStream.Position = offsets[counter];
                        int next = (counter < offsets.Count - 1 ? offsets[counter + 1] : slabsEnd);

                        //read slab
                        while (reader.BaseStream.Position < next)
                        {
                            int ztop = reader.ReadByte();
                            int zleng = reader.ReadByte();
                            if (ztop + zleng > zsize) break;
                            int flags = reader.ReadByte();

                            if (zleng > 0)
                            {
                                List<int> colorIndices = new List<int>(zleng);
                                for (int i = 0; i < zleng; i++)
                                {
                                    colorIndices.Add(reader.ReadByte());
                                }

                                if ((flags & 16) != 0)
                                {
                                    AddFace(verts, indices, verthashes, new Vector3D(x, y, ztop), new Vector3D(x + 1, y, ztop), new Vector3D(x, y + 1, ztop), new Vector3D(x + 1, y + 1, ztop), pivot, colorIndices[0]);
                                    facescount += 2;
                                }

                                int z = ztop;
                                int cstart = 0;
                                while (z < ztop + zleng)
                                {
                                    int c = 0;
                                    while (z + c < ztop + zleng && colorIndices[cstart + c] == colorIndices[cstart]) c++;

                                    if ((flags & 1) != 0)
                                    {
                                        AddFace(verts, indices, verthashes, new Vector3D(x, y, z), new Vector3D(x, y + 1, z), new Vector3D(x, y, z + c), new Vector3D(x, y + 1, z + c), pivot, colorIndices[cstart]);
                                        facescount += 2;
                                    }
                                    if ((flags & 2) != 0)
                                    {
                                        AddFace(verts, indices, verthashes, new Vector3D(x + 1, y + 1, z), new Vector3D(x + 1, y, z), new Vector3D(x + 1, y + 1, z + c), new Vector3D(x + 1, y, z + c), pivot, colorIndices[cstart]);
                                        facescount += 2;
                                    }
                                    if ((flags & 4) != 0)
                                    {
                                        AddFace(verts, indices, verthashes, new Vector3D(x + 1, y, z), new Vector3D(x, y, z), new Vector3D(x + 1, y, z + c), new Vector3D(x, y, z + c), pivot, colorIndices[cstart]);
                                        facescount += 2;
                                    }
                                    if ((flags & 8) != 0)
                                    {
                                        AddFace(verts, indices, verthashes, new Vector3D(x, y + 1, z), new Vector3D(x + 1, y + 1, z), new Vector3D(x, y + 1, z + c), new Vector3D(x + 1, y + 1, z + c), pivot, colorIndices[cstart]);
                                        facescount += 2;
                                    }

                                    if (c == 0) c++;
                                    z += c;
                                    cstart += c;
                                }

                                if ((flags & 32) != 0)
                                {
                                    z = ztop + zleng - 1;
                                    AddFace(verts, indices, verthashes, new Vector3D(x + 1, y, z + 1), new Vector3D(x, y, z + 1), new Vector3D(x + 1, y + 1, z + 1), new Vector3D(x, y + 1, z + 1), pivot, colorIndices[zleng - 1]);
                                    facescount += 2;
                                }
                            }
                        }

                        counter++;
                    }
                }
            }

            // get model extents
            int minX = (int)((xsize / 2f - pivot.x) * mde.Scale.X);
            int maxX = (int)((xsize / 2f + pivot.x) * mde.Scale.X);
            int minY = (int)((ysize / 2f - pivot.y) * mde.Scale.Y);
            int maxY = (int)((ysize / 2f + pivot.y) * mde.Scale.Y);

            // Calculate model radius
            mde.Model.Radius = Math.Max(Math.Max(Math.Abs(minY), Math.Abs(maxY)), Math.Max(Math.Abs(minX), Math.Abs(maxX)));

            // Create texture new Texture(bmp.Width)
            using (Bitmap bmp = CreateVoxelTexture(palette))
            {
                mde.Model.Textures.Add(new Texture(General.Map.Graphics, bmp));
            }

            // Create mesh
            Mesh mesh = new Mesh(General.Map.Graphics, verts.ToArray(), indices.ToArray());

            // Add mesh
            mde.Model.Meshes.Add(mesh);
        }

        // Shameless GZDoom rip-off
        private static void AddFace(List<WorldVertex> verts, List<int> indices, Dictionary<long, int> hashes, Vector3D v1, Vector3D v2, Vector3D v3, Vector3D v4, Vector3D pivot, int colorIndex)
        {
            float pu0 = (colorIndex % 16) / 16f;
            float pu1 = pu0 + 0.001f;
            float pv0 = (colorIndex / 16) / 16f;
            float pv1 = pv0 + 0.001f;

            WorldVertex wv1 = new WorldVertex
            {
                x = (float)(v1.x - pivot.x),
                y = (float)(-v1.y + pivot.y),
                z = (float)(-v1.z + pivot.z),
                c = -1,
                u = pu0,
                v = pv0
            };
            int i1 = AddVertex(wv1, verts, indices, hashes);

            WorldVertex wv2 = new WorldVertex
            {
                x = (float)(v2.x - pivot.x),
                y = (float)(-v2.y + pivot.y),
                z = (float)(-v2.z + pivot.z),
                c = -1,
                u = pu1,
                v = pv1
            };
            AddVertex(wv2, verts, indices, hashes);

            WorldVertex wv4 = new WorldVertex
            {
                x = (float)(v4.x - pivot.x),
                y = (float)(-v4.y + pivot.y),
                z = (float)(-v4.z + pivot.z),
                c = -1,
                u = pu0,
                v = pv0
            };
            int i4 = AddVertex(wv4, verts, indices, hashes);

            WorldVertex wv3 = new WorldVertex
            {
                x = (float)(v3.x - pivot.x),
                y = (float)(-v3.y + pivot.y),
                z = (float)(-v3.z + pivot.z),
                c = -1,
                u = pu1,
                v = pv1
            };
            AddVertex(wv3, verts, indices, hashes);

            indices.Add(i1);
            indices.Add(i4);
        }

        // Returns index of added vert
        private static int AddVertex(WorldVertex v, List<WorldVertex> verts, List<int> indices, Dictionary<long, int> hashes)
        {
            long hash;
            unchecked // Overflow is fine, just wrap
            {
                hash = 2166136261;
                hash = (hash * 16777619) ^ v.x.GetHashCode();
                hash = (hash * 16777619) ^ v.y.GetHashCode();
                hash = (hash * 16777619) ^ v.z.GetHashCode();
                hash = (hash * 16777619) ^ v.u.GetHashCode();
                hash = (hash * 16777619) ^ v.v.GetHashCode();
            }

            if (hashes.ContainsKey(hash))
            {
                indices.Add(hashes[hash]);
                return hashes[hash];
            }
            else
            {
                verts.Add(v);
                hashes.Add(hash, verts.Count - 1);
                indices.Add(verts.Count - 1);
                return verts.Count - 1;
            }
        }

        private unsafe static Bitmap CreateVoxelTexture(PixelColor[] palette)
        {
            Bitmap bmp = new Bitmap(16, 16);
            BitmapData bmpdata = bmp.LockBits(new Rectangle(0, 0, 16, 16), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            if (bmpdata != null)
            {
                PixelColor* pixels = (PixelColor*)(bmpdata.Scan0.ToPointer());
                const int numpixels = 256;
                int i = 255;

                for (PixelColor* cp = pixels + numpixels - 1; cp >= pixels; cp--, i--)
                {
                    cp->r = palette[i].r;
                    cp->g = palette[i].g;
                    cp->b = palette[i].b;
                    cp->a = palette[i].a;
                }
                bmp.UnlockBits(bmpdata);
            }

            //scale bitmap, so colors stay (almost) the same when bilinear filtering is enabled
            Bitmap scaled = new Bitmap(64, 64);
            using (Graphics gs = Graphics.FromImage(scaled))
            {
                gs.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                gs.DrawImage(bmp, new Rectangle(0, 0, 64, 64), new Rectangle(0, 0, 16, 16), GraphicsUnit.Pixel);
            }
            bmp.Dispose();

            return scaled;
        }
    }
}
