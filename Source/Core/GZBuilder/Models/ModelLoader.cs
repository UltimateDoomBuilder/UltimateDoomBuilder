using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.GZBuilder.Data;
using CodeImp.DoomBuilder.Rendering;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace CodeImp.DoomBuilder.GZBuilder.Models
{
    internal class ModelLoader
    {
        #region ================== Load

        public static void Load(ModelData mde, List<DataReader> containers)
        {
            if (mde.IsVoxel) LoadKVX(mde, containers);
            else LoadModel(mde, containers);
        }

        private static void LoadKVX(ModelData mde, List<DataReader> containers)
        {
            mde.Model = new GZModel();
            string unused = string.Empty;
            foreach (string name in mde.ModelNames)
            {
                //find the model
                foreach (DataReader dr in containers)
                {
                    Stream ms = dr.GetVoxelData(name, ref unused);
                    if (ms == null) continue;

                    //load kvx
                    KVXModelLoader.Load(mde, ms);

                    //done
                    ms.Close();
                    break;
                }
            }

            if (mde.Model.Meshes == null || mde.Model.Meshes.Count == 0)
            {
                mde.Model = null;
			}
			else
			{
				//clear unneeded data
				mde.SkinNames = null;
				mde.ModelNames = null;
			}
        }

        private static void LoadModel(ModelData mde, List<DataReader> containers)
        {
            mde.Model = new GZModel();
            BoundingBoxSizes bbs = new BoundingBoxSizes();
            ModelLoadResult result = new ModelLoadResult();

            //load models and textures
            for (int i = 0; i < mde.ModelNames.Count; i++)
            {
                // Use model skins?
                // INFO: Skin MODELDEF property overrides both embedded surface names and ones set using SurfaceSkin MODELDEF property
                Dictionary<int, string> skins = null;
                if (string.IsNullOrEmpty(mde.SkinNames[i]))
                {
                    skins = (mde.SurfaceSkinNames[i].Count > 0 ? mde.SurfaceSkinNames[i] : new Dictionary<int, string>());
                }

                // Load mesh
                MemoryStream ms = LoadFile(containers, mde.ModelNames[i], true);
                if (ms == null)
                {
                    General.ErrorLogger.Add(ErrorType.Error, "Error while loading \"" + mde.ModelNames[i] + "\": unable to find file.");
                    continue;
                }

                string ext = Path.GetExtension(mde.ModelNames[i]);
                switch (ext)
                {
                    case ".md3":
                        if (!string.IsNullOrEmpty(mde.FrameNames[i]))
                        {
                            General.ErrorLogger.Add(ErrorType.Error, "Error while loading \"" + mde.ModelNames[i] + "\": frame names are not supported for MD3 models!");
                            continue;
                        }
                        result = MD3ModelLoader.Load(ref bbs, skins, ms, mde.FrameIndices[i]);
                        break;
                    case ".md2":
                        result = MD2ModelLoader.Load(ref bbs, ms, mde.FrameIndices[i], mde.FrameNames[i]);
                        break;
                    case ".3d":
                        result = UnrealModelLoader.Load(ref bbs, skins, ms, mde.FrameIndices[i], mde.ModelNames[i], containers);
                        break;
                    case ".obj":
                        // OBJ doesn't support frames, so print out an error
                        if (mde.FrameIndices[i] > 0)
                        {
                            General.ErrorLogger.Add(ErrorType.Error, "Trying to load frame " + mde.FrameIndices[i] + " of model \"" + mde.ModelNames[i] + "\", but OBJ doesn't support frames!");
                            continue;
                        }
                        result = OBJModelLoader.Load(ref bbs, skins, ms, mde.ModelNames[i]);
                        break;
                    case ".iqm":
                        if (!string.IsNullOrEmpty(mde.FrameNames[i]))
                        {
                            General.ErrorLogger.Add(ErrorType.Error, "Error while loading \"" + mde.ModelNames[i] + "\": frame names are not supported for IQM models!");
                            continue;
                        }
                        result = IQMModelLoader.Load(ref bbs, skins, ms, mde.FrameIndices[i]);
                        break;
                    default:
                        result.Errors = "model format is not supported";
                        break;
                }

                ms.Close();
                if (result == null)
                    continue;

                //got errors?
                if (!String.IsNullOrEmpty(result.Errors))
                {
                    General.ErrorLogger.Add(ErrorType.Error, "Error while loading \"" + mde.ModelNames[i] + "\": " + result.Errors);
                }
                else
                {
                    //add loaded data to ModeldefEntry
                    mde.Model.Meshes.AddRange(result.Meshes);

                    //load texture
                    List<string> errors = new List<string>();

                    // Texture not defined in MODELDEF?
                    if (skins != null)
                    {
                        //try to use model's own skins
                        for (int m = 0; m < result.Meshes.Count; m++)
                        {
                            // biwa. Makes sure to add a dummy texture if the MODELDEF skin definition is erroneous
                            if (m >= result.Skins.Count)
                            {
                                errors.Add("no skin defined for mesh " + m + ".");
                                mde.Model.Textures.Add(General.Map.Data.UnknownTexture3D.Texture);
                                continue;
                            }

                            if (string.IsNullOrEmpty(result.Skins[m]))
                            {
                                mde.Model.Textures.Add(General.Map.Data.UnknownTexture3D.Texture);
                                errors.Add("texture not found in MODELDEF or model skin.");
                                continue;
                            }

                            string path = result.Skins[m].Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);

                            if (!String.IsNullOrEmpty(mde.Path))
                                path = Path.Combine(mde.Path, path);

                            Texture t = GetTexture(containers, path);

                            if (t != null)
                            {
                                mde.Model.Textures.Add(t);
                                continue;
                            }

                            // That didn't work, let's try to load the texture without the additional path
                            path = result.Skins[m].Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
                            t = GetTexture(containers, path);

                            if (t == null)
                            {
                                mde.Model.Textures.Add(General.Map.Data.UnknownTexture3D.Texture);
                                errors.Add("unable to load skin \"" + path + "\"");
                                continue;
                            }

                            mde.Model.Textures.Add(t);
                        }
                    }
                    //Try to use texture loaded from MODELDEFS
                    else
                    {
                        Texture t = GetTexture(containers, mde.SkinNames[i]);

                        if (t == null)
                        {
                            mde.Model.Textures.Add(General.Map.Data.UnknownTexture3D.Texture);
                            errors.Add("unable to load skin \"" + mde.SkinNames[i] + "\"");
                        }
                        else
                        {
                            mde.Model.Textures.Add(t);
                        }
                    }

                    //report errors
                    if (errors.Count > 0)
                    {
                        foreach (string e in errors)
                            General.ErrorLogger.Add(ErrorType.Error, "Error while loading \"" + mde.ModelNames[i] + "\": " + e);
                    }
                }
            }

            //clear unneeded data
            mde.SkinNames = null;
            mde.ModelNames = null;

            if (mde.Model.Meshes == null || mde.Model.Meshes.Count == 0)
            {
                mde.Model = null;
                return;
            }

            //scale bbs
            bbs.MaxX = (int)(bbs.MaxX * mde.Scale.X);
            bbs.MinX = (int)(bbs.MinX * mde.Scale.X);
            bbs.MaxY = (int)(bbs.MaxY * mde.Scale.Y);
            bbs.MinY = (int)(bbs.MinY * mde.Scale.Y);
            bbs.MaxZ = (int)(bbs.MaxZ * mde.Scale.Z);
            bbs.MinZ = (int)(bbs.MinZ * mde.Scale.Z);

            //calculate model radius
            mde.Model.Radius = Math.Max(Math.Max(Math.Abs(bbs.MinY), Math.Abs(bbs.MaxY)), Math.Max(Math.Abs(bbs.MinX), Math.Abs(bbs.MaxX)));
            mde.Model.BBox = bbs;
        }

        private static Texture GetTexture(List<DataReader> containers, string texturename)
        {
            Texture t = null;
            string[] extensions = new string[ModelData.SUPPORTED_TEXTURE_EXTENSIONS.Length + 1];

            Array.Copy(ModelData.SUPPORTED_TEXTURE_EXTENSIONS, 0, extensions, 1, ModelData.SUPPORTED_TEXTURE_EXTENSIONS.Length);
            extensions[0] = "";

            // Try to load the texture as defined by its path. GZDoom doesn't care about extensions
            if (t == null)
            {
                foreach (string extension in extensions)
                {
                    string name = Path.ChangeExtension(texturename, null) + extension;
                    name = name.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);

                    t = LoadTexture(containers, name);

                    if (t != null)
                        break;
                }
            }

            // Try to use an already defined texture. Again, just try out all extensions
            foreach (string extension in extensions)
            {
                string name = Path.ChangeExtension(texturename, null) + extension;

                if (General.Map.Data.GetTextureExists(name))
                {
                    ImageData image = General.Map.Data.GetTextureImage(name);
                    image.LoadImageNow(false);

                    t = image.Texture;

                    break;
                }
            }

            // GZDoom can also ignore the path completely (because why not), so let's see if there's a texture with
            // just the skin name
            if (t == null)
            {
                string name = Path.ChangeExtension(Path.GetFileName(texturename), null);

                if (General.Map.Data.GetTextureExists(name))
                {
                    ImageData image = General.Map.Data.GetTextureImage(name);
                    image.LoadImageNow(false);

                    t = image.Texture;
                }
            }

            // Or maybe it's a sprite
            if (t == null)
            {
                string name = Path.ChangeExtension(texturename, null);

                if (General.Map.Data.GetSpriteExists(name))
                {
                    ImageData image = General.Map.Data.GetSpriteImage(name);
                    image.LoadImageNow(false);

                    t = image.Texture;
                }
            }

            return t;
        }

        #endregion

        #region ================== Utility

        protected static MemoryStream LoadFile(List<DataReader> containers, string path, bool isModel)
        {
            foreach (DataReader dr in containers)
            {
                if (isModel && dr is WADReader) continue;  //models cannot be stored in WADs

                //load file
                if (dr.FileExists(path)) return dr.LoadFile(path);
            }
            return null;
        }

        protected static Texture LoadTexture(List<DataReader> containers, string path)
        {
            if (string.IsNullOrEmpty(path)) return null;

            MemoryStream ms = LoadFile(containers, path, true);
            if (ms == null) return null;

            Texture texture = null;

            //create texture
            Bitmap bitmap = ImageDataFormat.TryLoadImage(ms);
            if (bitmap != null)
            {
                texture = new Texture(General.Map.Graphics, bitmap);
            }

            return texture;
        }

        protected static void CreateMesh(ref ModelLoadResult result, List<WorldVertex> verts, List<int> indices)
        {
            //create mesh
            Mesh mesh = new Mesh(General.Map.Graphics, verts.ToArray(), indices.ToArray());

            //store in result
            result.Meshes.Add(mesh);
        }

        protected static string ReadString(BinaryReader br, int len)
        {
            string result = string.Empty;
            int i;

            for (i = 0; i < len; ++i)
            {
                var c = br.ReadChar();
                if (c == '\0')
                {
                    ++i;
                    break;
                }
                result += c;
            }

            for (; i < len; ++i) br.ReadChar();
            return result;
        }

        #endregion
    }
}
