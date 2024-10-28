#region ================== Namespaces

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Text;
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.VisualModes;
using CodeImp.DoomBuilder.Windows;
using CodeImp.DoomBuilder.BuilderModes.Interface;
using System.Windows.Forms;
using System.Linq;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes.IO
{
	#region ================== Structs

	internal struct WavefrontExportSettings
	{
		public string Obj;
		public readonly string ObjName;
		public readonly string ObjPath;
		public readonly float Scale;
		public readonly bool ExportForGZDoom;
		public readonly bool ExportTextures;
		public bool Valid;
		public string[] Textures;
		public string[] Flats;
		public string ActorName;
		public string BasePath;
		public string ActorPath;
		public string ModelPath;
		public List<string> SkipTextures;
		public bool IgnoreControlSectors;
		public bool NormalizeLowestVertex;
		public bool CenterModel;
		public bool ZScript;
		public bool GenerateCode;
		public bool GenerateModeldef;

		// Actor properties and flags
		public int Radius;
		public int Height;
		public string Sprite;
		public bool NoGravity;
		public bool SpawnOnCeiling;
		public bool Solid;

		public WavefrontExportSettings(WavefrontSettingsForm form)
		{
			ObjName = Path.GetFileNameWithoutExtension(form.FilePath);
			ObjPath = Path.GetDirectoryName(form.FilePath);
			Scale = form.ObjScale;
			ExportForGZDoom = form.UseGZDoomScale;
			ExportTextures = form.ExportTextures;

			ActorName = form.ActorName.Trim();
			BasePath = form.BasePath;
			ActorPath = form.ActorPath;
			ModelPath = form.ModelPath;

			// Only skip textures if we're exporting for GZDoom
			if (ExportForGZDoom)
				SkipTextures = form.SkipTextures;
			else
				SkipTextures = new List<string>();


			IgnoreControlSectors = form.IgnoreControlSectors;
			NormalizeLowestVertex = form.NormalizeLowestVertex;
			CenterModel = form.CenterModel;
			ZScript = form.ZScript;
			GenerateCode = form.GenerateCode;
			GenerateModeldef = form.GenerateModeldef;

			NoGravity = form.NoGravity;
			SpawnOnCeiling = form.SpawnOnCeiling;
			Solid = form.Solid;
			Sprite = form.Sprite;

			Radius = 20;
			Height = 16;

			Valid = false;
			Obj = string.Empty;
			Textures = null;
			Flats = null;

			if (ExportForGZDoom)
				SkipTextures.Add("-");
		}
	}

	#endregion

	internal class WavefrontExporter
	{

		#region ================== Variables and structs

		private const string DEFAULT = "Default";

		private struct VertexIndices
		{
			public int PositionIndex;
			public int UVIndex;
			public int NormalIndex;
		}

		#endregion

		#region ================== Export

		public void Export(ICollection<Sector> sectors, WavefrontExportSettings settings) 
		{
			CreateObjFromSelection(sectors, ref settings);

			if(!settings.Valid) 
			{
				General.Interface.DisplayStatus(StatusType.Warning, "OBJ creation failed. Check 'Errors and Warnings' window for details.");
				return;
			}

			// Export Textures, but only of it's not exporting for GZDoom
			if(settings.ExportTextures && !settings.ExportForGZDoom) 
			{
				//save all used textures
				if(settings.Textures != null) 
				{
					foreach(string s in settings.Textures) 
					{
						if(s == DEFAULT) continue;
						if(General.Map.Data.GetTextureExists(s)) 
						{
							ImageData id = General.Map.Data.GetTextureImage(s);
							if(id.Width == 0 || id.Height == 0) 
							{
								General.ErrorLogger.Add(ErrorType.Warning, "OBJ Exporter: texture \"" + s + "\" has invalid size (" + id.Width + "x" + id.Height + ")!");
								continue;
							}

							//Bitmap bmp = id.ExportBitmap();
							// The image might have a color correction applied, but we need it without. So we use LocalGetBitmap, because it reloads the image,
							// but doesn't applie the color correction if we set UseColorCorrection to false first
							Bitmap bmp = new Bitmap(id.LocalGetBitmap(false));
							lock (bmp)
                            {
								string filepath = Path.Combine(settings.ObjPath, Path.GetDirectoryName(s), Path.GetFileNameWithoutExtension(s) + ".png");

								// Make sure the directory is there
								Directory.CreateDirectory(Path.GetDirectoryName(filepath));

								bmp.Save(filepath, ImageFormat.Png);
                            }
						} 
						else 
						{
							General.ErrorLogger.Add(ErrorType.Warning, "OBJ Exporter: texture \"" + s + "\" does not exist!");
						}
					}
				}

				if(settings.Flats != null) 
				{
					foreach(string s in settings.Flats) 
					{
						if(s == DEFAULT) continue;
						if(General.Map.Data.GetFlatExists(s)) 
						{
							ImageData id = General.Map.Data.GetFlatImage(s);
							if(id.Width == 0 || id.Height == 0) 
							{
								General.ErrorLogger.Add(ErrorType.Warning, "OBJ Exporter: flat \"" + s + "\" has invalid size (" + id.Width + "x" + id.Height + ")!");
								continue;
							}

							//Bitmap bmp = id.ExportBitmap();
							// The image might have a color correction applied, but we need it without. So we use LocalGetBitmap, because it reloads the image,
							// but doesn't applie the color correction if we set UseColorCorrection to false first
							Bitmap bmp = new Bitmap(id.LocalGetBitmap(false));


							// Handle duplicate names
							string flatname = s;
							if(settings.Textures != null && Array.IndexOf(settings.Textures, s) != -1)
								flatname += "_FLAT";

                            lock (bmp)
                            {
                                bmp.Save(Path.Combine(settings.ObjPath, Path.GetFileNameWithoutExtension(flatname) + ".PNG"), ImageFormat.Png);
                            }

						} 
						else 
						{
							General.ErrorLogger.Add(ErrorType.Warning, "OBJ Exporter: flat \"" + s + "\" does not exist!");
						}
					}
				}
			}

			//write obj
			string savePath;

			if (settings.ExportForGZDoom)
				savePath = Path.Combine(settings.ModelPath, settings.ActorName + ".obj");
			else
				savePath = Path.Combine(settings.ObjPath, settings.ObjName + ".obj");

			// Make sure the directory is there
			Directory.CreateDirectory(Path.GetDirectoryName(savePath));

			using (StreamWriter sw = new StreamWriter(savePath, false))
				sw.Write(settings.Obj);

			//create mtl
			StringBuilder mtl = new StringBuilder();
			mtl.Append("# MTL for " + General.Map.FileTitle + ", map " + General.Map.Options.LevelName + Environment.NewLine);
			mtl.Append("# Created by Ultimate Doom Builder " + Application.ProductVersion + Environment.NewLine + Environment.NewLine);

			if(settings.Textures != null) 
			{
				foreach(string s in settings.Textures) 
				{
					if(s == DEFAULT) continue;

					string filepath = Path.Combine(settings.ObjPath, Path.GetDirectoryName(s), Path.GetFileNameWithoutExtension(s) + ".png");

					mtl.Append("newmtl " + s + Environment.NewLine);
					mtl.Append("Kd 1.0 1.0 1.0" + Environment.NewLine);
					if(settings.ExportTextures) mtl.Append("map_Kd " + filepath + Environment.NewLine);
					mtl.Append(Environment.NewLine);
				}
			}

			if(settings.Flats != null) 
			{
				foreach(string s in settings.Flats) 
				{
					if(s == DEFAULT) continue;
					mtl.Append("newmtl " + s + Environment.NewLine);
					mtl.Append("Kd 1.0 1.0 1.0" + Environment.NewLine);
					if(settings.ExportTextures) 
					{
						// Handle duplicate names
						string flatsuffix = string.Empty;
						if(settings.Textures != null && Array.IndexOf(settings.Textures, s) != -1)
							flatsuffix = "_FLAT";

						string filepath = Path.Combine(settings.ObjPath, Path.GetDirectoryName(s), Path.GetFileNameWithoutExtension(s) + flatsuffix + ".png");

						mtl.Append("map_Kd " + Path.Combine(settings.ObjPath, filepath) + Environment.NewLine);
					}
					mtl.Append(Environment.NewLine);
				}
			}

			if (!settings.ExportForGZDoom)
			{
				// Make sure the directory is there
				Directory.CreateDirectory(Path.GetDirectoryName(savePath));

				string mtlPath = Path.Combine(Path.GetDirectoryName(savePath), Path.GetFileNameWithoutExtension(savePath) + ".mtl");

				// Write mtl (only if not exporting for GZDoom, since it will be ignored anyway
				using (StreamWriter sw = new StreamWriter(mtlPath, false))
					sw.Write(mtl.ToString());
			}
			else
			{
				// Create ZScript or DECORATE
				if (settings.GenerateCode)
				{
					Stream stream;
					string path = Path.Combine(settings.ActorPath, settings.ActorName);

					if (settings.ZScript)
					{
						stream = BuilderPlug.Me.GetResourceStream("ObjExportZScriptTemplate.txt");
						path += ".zs";
					}
					else
					{
						stream = BuilderPlug.Me.GetResourceStream("ObjExportDecorateTemplate.txt");
						path += ".txt";
					}

					using (StreamReader reader = new StreamReader(stream, Encoding.ASCII))
					{
						string template = reader.ReadToEnd();

						template = template.Replace("{ActorName}", settings.ActorName);
						template = template.Replace("{Sprite}", settings.Sprite);
						template = template.Replace("{FlagNoGravity}", settings.NoGravity ? "+NOGRAVITY" : "");
						template = template.Replace("{FlagSpawnOnCeiling}", settings.SpawnOnCeiling ? "+SPAWNCEILING" : "");
						template = template.Replace("{FlagSolid}", settings.Solid ? "+SOLID" : "");
						template = template.Replace("{FlagInvulnerable}", settings.Solid ? "+INVULNERABLE" : "");
						template = template.Replace("{FlagNoDamage}", settings.Solid ? "+NODAMAGE" : "");
						template = template.Replace("{FlagShootable}", settings.Solid ? "+SHOOTABLE" : "");
						template = template.Replace("{FlagNotAutoAimed}", settings.Solid ? "+NOTAUTOAIMED" : "");
						template = template.Replace("{FlagNeverTarget}", settings.Solid ? "+NEVERTARGET" : "");
						template = template.Replace("{FlagDontThrust}", settings.Solid ? "+DONTTHRUST" : "");
						template = template.Replace("{PropRadius}", settings.Radius.ToString());
						template = template.Replace("{PropHeight}", settings.Height.ToString());

						// Make sure the directory is there
						Directory.CreateDirectory(Path.GetDirectoryName(path));

						using (StreamWriter sw = new StreamWriter(path, false))
							sw.Write(template);
					}
				}

				// Create MODELDEF
				if (settings.GenerateModeldef)
				{
					Stream stream = BuilderPlug.Me.GetResourceStream("ObjExportModeldefTemplate.txt");

					using (StreamReader reader = new StreamReader(stream, Encoding.ASCII))
					{
						string path = Path.Combine(settings.BasePath, "modeldef." + settings.ActorName + ".txt");
						string template = reader.ReadToEnd();

						// The path to the model is relative to the base path, so generate the base path
						string basepath = settings.BasePath.Trim();
						string modelpath = settings.ModelPath.Trim();

						// Make sue there's a directory separator at the end of the paths, otherwise it'll not work correctly
						if (!basepath.EndsWith(Path.DirectorySeparatorChar.ToString()))
							basepath += Path.DirectorySeparatorChar;

						if (!modelpath.EndsWith(Path.DirectorySeparatorChar.ToString()))
							modelpath += Path.DirectorySeparatorChar;

						Uri baseUri = new Uri(basepath);
						Uri modelUri = new Uri(modelpath);

						Uri relativeUri = baseUri.MakeRelativeUri(modelUri);
						string relativepath = Uri.UnescapeDataString(relativeUri.OriginalString);

						template = template.Replace("{ActorName}", settings.ActorName);
						template = template.Replace("{ModelPath}", relativepath);
						template = template.Replace("{Sprite}", settings.Sprite);

						// Make sure the directory is there
						Directory.CreateDirectory(Path.GetDirectoryName(path));

						using (StreamWriter sw = new StreamWriter(path, false))
							sw.Write(template);
					}
				}
			}

			//done
			General.Interface.DisplayStatus(StatusType.Info, "Geometry exported to \"" + savePath);
		}

		#endregion

		#region ================== Utility

		private static void CreateObjFromSelection(ICollection<Sector> sectors, ref WavefrontExportSettings data) 
		{
			BaseVisualMode mode = new BaseVisualMode();
			bool renderingEffectsDisabled = false;

			if(!General.Settings.EnhancedRenderingEffects) 
			{
				renderingEffectsDisabled = true;
				mode.ToggleEnhancedRendering();
			}

			mode.RebuildElementData();

			List<BaseVisualSector> visualSectors = new List<BaseVisualSector>();

			//create visual geometry
			foreach(Sector s in sectors) 
			{
				bool addvs = true;

				// Check if the sector has, or shares a line with a 3D floor control sector, and ignore it if necessary
				if (data.ExportForGZDoom && data.IgnoreControlSectors)
				{
					foreach (Sidedef sd in s.Sidedefs)
					{
						if (sd.Line.Action == 160)
						{
							addvs = false;
							break;
						}
					}
				}

				if (addvs)
				{
					BaseVisualSector bvs = mode.CreateBaseVisualSector(s);

					if (bvs != null)
					{
						// When the visual sector is created yet unseen wall textures are not fully loaded in time, which can result in the
						// texture coordinates (UV) ending up as NaN. This can happen if the export is started before the textures are loaded
						// (for example by opening the texture browser or going into visual mode. So make sure the textures are loaded immediately.
						// After that the visual sector has to be rebuilt. This only seems to affect wall textures, not floors/ceilings
						// See https://github.com/UltimateDoomBuilder/UltimateDoomBuilder/issues/1015
						foreach (VisualSidedefParts vsp in bvs.Sides.Values)
						{
							if (vsp.upper?.Texture != null && vsp.upper.Texture.ImageState != ImageLoadState.Ready)
								vsp.upper.Texture.LocalGetBitmap(true);

							if (vsp.middlesingle?.Texture != null && vsp.middlesingle.Texture.ImageState != ImageLoadState.Ready)
								vsp.middlesingle.Texture.LocalGetBitmap(true);

							if (vsp.middledouble?.Texture != null && vsp.middledouble.Texture.ImageState != ImageLoadState.Ready)
								vsp.middledouble.Texture.LocalGetBitmap(true);

							if (vsp.lower?.Texture != null && vsp.lower.Texture.ImageState != ImageLoadState.Ready)
								vsp.lower.Texture.LocalGetBitmap(true);

							if (vsp.middle3d != null)
							{
								foreach (VisualMiddle3D vm in vsp.middle3d.Where(o => o.Texture != null && o.Texture.ImageState != ImageLoadState.Ready))
								{
									vm.Texture.LocalGetBitmap(true);
								}
							}

							if (vsp.middleback != null)
							{
								foreach (VisualMiddleBack vm in vsp.middleback.Where(o => o.Texture != null && o.Texture.ImageState != ImageLoadState.Ready))
								{
									vm.Texture.LocalGetBitmap(true);
								}
							}
						}

						// Rebuild the visual sector so that the texture coordinates are guaranteed to be correct
						bvs.Rebuild();

						visualSectors.Add(bvs);
					}
				}
			}

			if (visualSectors.Count == 0) 
			{
				General.ErrorLogger.Add(ErrorType.Error, "OBJ Exporter: no visual sectors to export!");
				return;
			}

			//sort geometry
			List<Dictionary<string, List<WorldVertex[]>>> geometryByTexture = SortGeometry(visualSectors, data.SkipTextures, !data.ExportForGZDoom);

			//restore vm settings
			if(renderingEffectsDisabled) mode.ToggleEnhancedRendering();
			mode.Dispose();

			//create obj
			StringBuilder obj = CreateObjGeometry(geometryByTexture, ref data);

			if(obj.Length == 0) 
			{
				General.ErrorLogger.Add(ErrorType.Error, "OBJ Exporter: failed to create geometry!");
				return;
			}

			//add header
			obj.Insert(0, "o " + General.Map.Options.LevelName + Environment.NewLine); //name
			obj.Insert(0, "# Created by Ultimate Doom Builder " + Application.ProductVersion + Environment.NewLine + Environment.NewLine);
			obj.Insert(0, "# " + General.Map.FileTitle + ", map " + General.Map.Options.LevelName + Environment.NewLine);
			data.Obj = obj.ToString();

			string[] textures = new string[geometryByTexture[0].Keys.Count];
			geometryByTexture[0].Keys.CopyTo(textures, 0);
			Array.Sort(textures);
			data.Textures = textures;

			string[] flats = new string[geometryByTexture[1].Keys.Count];
			geometryByTexture[1].Keys.CopyTo(flats, 0);
			Array.Sort(flats);
			data.Flats = flats;

			data.Valid = true;
		}

		private static List<Dictionary<string, List<WorldVertex[]>>> SortGeometry(List<BaseVisualSector> visualSectors, List<string>skipTextures, bool defaultOnMissingTextures) 
		{
			var texturegeo = new Dictionary<string, List<WorldVertex[]>>(StringComparer.Ordinal);
			var flatgeo = new Dictionary<string, List<WorldVertex[]>>(StringComparer.Ordinal);

			if (defaultOnMissingTextures)
			{
				texturegeo.Add(DEFAULT, new List<WorldVertex[]>());
				flatgeo.Add(DEFAULT, new List<WorldVertex[]>());
			}

			foreach (BaseVisualSector vs in visualSectors) 
			{
				//floor
				string texture;
				if(vs.Floor != null) 
				{
					texture = vs.Sector.FloorTexture;
					if (!skipTextures.Contains(texture))
					{
						CheckTextureName(ref flatgeo, ref texture);
						flatgeo[texture].AddRange(OptimizeGeometry(vs.Floor.Vertices, vs.Floor.GeometryType, vs.Floor.Sector.Sector.Labels.Count > 1));
					}
				}

				//ceiling
				if(vs.Ceiling != null) 
				{
					texture = vs.Sector.CeilTexture;
					if (!skipTextures.Contains(texture))
					{
						CheckTextureName(ref flatgeo, ref texture);
						flatgeo[texture].AddRange(OptimizeGeometry(vs.Ceiling.Vertices, vs.Ceiling.GeometryType, vs.Ceiling.Sector.Sector.Labels.Count > 1));
					}
				}

				//walls
				if(vs.Sides != null) 
				{
					foreach(VisualSidedefParts part in vs.Sides.Values) 
					{
						//upper
						if(part.upper != null && part.upper.Vertices != null) 
						{
							texture = part.upper.Sidedef.HighTexture;
							if (!skipTextures.Contains(texture))
							{
								CheckTextureName(ref texturegeo, ref texture);
								texturegeo[texture].AddRange(OptimizeGeometry(part.upper.Vertices, part.upper.GeometryType));
							}
						}

						//middle single
						if(part.middlesingle != null && part.middlesingle.Vertices != null) 
						{
							texture = part.middlesingle.Sidedef.MiddleTexture;
							if (!skipTextures.Contains(texture))
							{
								CheckTextureName(ref texturegeo, ref texture);
								texturegeo[texture].AddRange(OptimizeGeometry(part.middlesingle.Vertices, part.middlesingle.GeometryType));
							}
						}

						//middle double
						if(part.middledouble != null && part.middledouble.Vertices != null) 
						{
							texture = part.middledouble.Sidedef.MiddleTexture;
							if (!skipTextures.Contains(texture))
							{
								CheckTextureName(ref texturegeo, ref texture);
								texturegeo[texture].AddRange(OptimizeGeometry(part.middledouble.Vertices, part.middledouble.GeometryType));
							}
						}

						//middle3d
						if(part.middle3d != null && part.middle3d.Count > 0) 
						{
							foreach(VisualMiddle3D m3d in part.middle3d) 
							{
								if(m3d.Vertices == null) continue;
								texture = m3d.GetTextureName();
								if (!skipTextures.Contains(texture))
								{
									CheckTextureName(ref texturegeo, ref texture);
									// 3D floor sides are cut so that there are only triangles for the visible parts. Some of those might be
									// triangles and not rectangles, so we can't optimize them into rectangles
									texturegeo[texture].AddRange(OptimizeGeometry(m3d.Vertices, m3d.GeometryType, true));
								}
							}
						}

						//backsides(?)

						//lower
						if(part.lower != null && part.lower.Vertices != null) 
						{
							texture = part.lower.Sidedef.LowTexture;
							if (!skipTextures.Contains(texture))
							{
								CheckTextureName(ref texturegeo, ref texture);
								texturegeo[texture].AddRange(OptimizeGeometry(part.lower.Vertices, part.lower.GeometryType));
							}
						}
					}
				}

				//3d ceilings
				foreach(VisualCeiling vc in vs.ExtraCeilings) 
				{
					texture = vc.GetControlSector().CeilTexture;
					if (!skipTextures.Contains(texture))
					{
						CheckTextureName(ref flatgeo, ref texture);
						flatgeo[texture].AddRange(OptimizeGeometry(vc.Vertices, (vc.ExtraFloor.VavoomType ? vc.GeometryType : VisualGeometryType.FLOOR)));
					}
				}

				//3d floors
				foreach(VisualFloor vf in vs.ExtraFloors) 
				{
					texture = vf.GetControlSector().FloorTexture;
					if (!skipTextures.Contains(texture))
					{
						CheckTextureName(ref flatgeo, ref texture);
						flatgeo[texture].AddRange(OptimizeGeometry(vf.Vertices, (vf.ExtraFloor.VavoomType ? vf.GeometryType : VisualGeometryType.CEILING)));
					}
				}

				//backsides(?)
			}

			return new List<Dictionary<string, List<WorldVertex[]>>> { texturegeo, flatgeo };
		}

		private static void CheckTextureName(ref Dictionary<string, List<WorldVertex[]>> geo, ref string texture) 
		{
			if(!string.IsNullOrEmpty(texture) && texture != "-") 
			{
				if(!geo.ContainsKey(texture)) geo.Add(texture, new List<WorldVertex[]>());
			} 
			else 
			{
				texture = DEFAULT;
			}
		}

		#endregion

		#region ================== Surface optimization

		private static List<WorldVertex[]> OptimizeGeometry(WorldVertex[] verts, VisualGeometryType geotype)
		{
			return OptimizeGeometry(verts, geotype, false);
		}

		private static List<WorldVertex[]> OptimizeGeometry(WorldVertex[] verts, VisualGeometryType geotype, bool skiprectoptimization) 
		{
			List<WorldVertex[]> groups = new List<WorldVertex[]>();

			// Only do optimizations for walls right now. Doing them blindly for floors/ceilings will cause problems with concave sectors with 4 corners
			if(!skiprectoptimization && verts.Length == 6 && (geotype != VisualGeometryType.CEILING && geotype != VisualGeometryType.FLOOR)) //rectangular surface
			{
				verts = new[] { verts[5], verts[2], verts[1], verts[0] };
				groups.Add(verts);
			} 
			else 
			{
				for(int i = 0; i < verts.Length; i += 3) 
				{
					groups.Add(new[] { verts[i + 2], verts[i + 1], verts[i] });
				}
			}

			return groups;
		}

		#endregion

		#region ================== OBJ Creation

		private static StringBuilder CreateObjGeometry(List<Dictionary<string, List<WorldVertex[]>>> geometryByTexture, ref WavefrontExportSettings data) 
		{
			StringBuilder obj = new StringBuilder();
			Vector2D offset;
			const string vertexFormatter = "{0} {2} {1}\n";

			Dictionary<Vector3D, int> uniqueVerts = new Dictionary<Vector3D, int>();
			Dictionary<Vector3D, int> uniqueNormals = new Dictionary<Vector3D, int>();
			Dictionary<PointF, int> uniqueUVs = new Dictionary<PointF, int>();

			var vertexDataByTexture = new Dictionary<string, Dictionary<WorldVertex, VertexIndices>>(StringComparer.Ordinal);
			int pc = 0;
			int nc = 0;
			int uvc = 0;

			Vector3D tl = new Vector3D(double.MaxValue, double.MinValue, double.MinValue);
			Vector3D br = new Vector3D(double.MinValue, double.MaxValue, double.MaxValue);

			//optimize geometry
			foreach (Dictionary<string, List<WorldVertex[]>> dictionary in geometryByTexture) 
			{
				foreach(KeyValuePair<string, List<WorldVertex[]>> group in dictionary) 
				{
					Dictionary<WorldVertex, VertexIndices> vertsData = new Dictionary<WorldVertex, VertexIndices>();
					foreach(WorldVertex[] verts in group.Value) 
					{
						//vertex normals. biwa not sure why I need to invert the normal's y component, but it seems to be necessary
						Vector3D n = new Vector3D(verts[0].nx, verts[0].ny, verts[0].nz).GetNormal();
						n.y *= -1;

						int ni;
						if(uniqueNormals.ContainsKey(n)) 
						{
							ni = uniqueNormals[n];
						} 
						else 
						{
							uniqueNormals.Add(n, ++nc);
							ni = nc;
						}

						foreach(WorldVertex v in verts) 
						{
							if(vertsData.ContainsKey(v)) continue;
							VertexIndices indices = new VertexIndices();
							indices.NormalIndex = ni;

							//vertex coords
							Vector3D vc = new Vector3D(v.x, v.y, v.z);
							if(uniqueVerts.ContainsKey(vc)) 
							{
								indices.PositionIndex = uniqueVerts[vc];
							} 
							else 
							{
								uniqueVerts.Add(vc, ++pc);
								indices.PositionIndex = pc;
							}

							//uv
							PointF uv = new PointF(v.u, v.v);
							if(uniqueUVs.ContainsKey(uv)) 
							{
								indices.UVIndex = uniqueUVs[uv];
							} 
							else 
							{
								uniqueUVs.Add(uv, ++uvc);
								indices.UVIndex = uvc;
							}

							vertsData.Add(v, indices);
						}
					}

					if(vertsData.Count > 0) 
					{
						if(vertexDataByTexture.ContainsKey(group.Key)) 
						{
							foreach(KeyValuePair<WorldVertex, VertexIndices> g in vertsData) 
								vertexDataByTexture[group.Key].Add(g.Key, g.Value);
						} 
						else 
						{
							vertexDataByTexture.Add(group.Key, vertsData);
						}
					}
				}
			}

			// Get the dimensions of the model
			foreach(Dictionary<WorldVertex, VertexIndices> vdata in vertexDataByTexture.Values)
			{
				foreach(WorldVertex wv in vdata.Keys)
				{
					if (wv.x < tl.x) tl.x = wv.x;
					if (wv.x > br.x) br.x = wv.x;
					if (wv.y > tl.y) tl.y = wv.y;
					if (wv.y < br.y) br.y = wv.y;
					if (wv.z > tl.z) tl.z = wv.z;
					if (wv.z < br.z) br.z = wv.z;
				}
			}

			data.Radius = br.x - tl.x > tl.y - br.y ? (int)(tl.y - br.y) / 2 : (int)(br.x - tl.x) / 2;
			data.Height = (int)(tl.z - br.z);

			if (data.CenterModel)
				offset = new Vector2D(tl.x + (br.x - tl.x) / 2.0, tl.y + (br.y - tl.y) / 2.0);
			else
				offset = new Vector2D(0.0, 0.0);

			//write geometry
			//write vertices
			if (data.ExportForGZDoom) 
			{


				foreach (KeyValuePair<Vector3D, int> group in uniqueVerts)
				{
					double z = (group.Key.z - (data.NormalizeLowestVertex ? br.z : 0)) * data.Scale * 1.2f;

					obj.Append(string.Format(CultureInfo.InvariantCulture, "v " + vertexFormatter, (group.Key.x - offset.x) * data.Scale, -(group.Key.y - offset.y) * data.Scale, z));
				}
			} 
			else 
			{
				// biwa. Not sure why the x-axis is flipped here, since it will result in wrong normals when using the model directly in GZDoom. For this reason
				// I disabled the flipping above
				foreach (KeyValuePair<Vector3D, int> group in uniqueVerts)
				{
					double z = (group.Key.z - (data.NormalizeLowestVertex ? br.z : 0)) * data.Scale;

					obj.Append(string.Format(CultureInfo.InvariantCulture, "v " + vertexFormatter, -(group.Key.x - offset.x) * data.Scale, (group.Key.y - offset.y) * data.Scale, z));
				}
			}

			//write normals
			foreach(KeyValuePair<Vector3D, int> group in uniqueNormals)
				obj.Append(string.Format(CultureInfo.InvariantCulture, "vn " + vertexFormatter, group.Key.x, group.Key.y, group.Key.z));

			//write UV coords
			foreach(KeyValuePair<PointF, int> group in uniqueUVs)
				obj.Append(string.Format(CultureInfo.InvariantCulture, "vt {0} {1}\n", group.Key.X, -group.Key.Y));

			// GZDoom ignores the material lib, so don't add it if the model is for GZDoom
			if (!data.ExportForGZDoom)
			{
				obj.Append("mtllib ").Append(data.ObjName + ".mtl").Append("\n");
			}

			//write materials and surface indices
			foreach(Dictionary<string, List<WorldVertex[]>> dictionary in geometryByTexture) 
			{
				foreach(KeyValuePair<string, List<WorldVertex[]>> group in dictionary) 
				{
					//material
					obj.Append("usemtl ").Append(group.Key).Append("\n");

					foreach(WorldVertex[] verts in group.Value) 
					{
						//surface indices
						obj.Append("f");
						foreach(WorldVertex v in verts) 
						{
							VertexIndices vi = vertexDataByTexture[group.Key][v];
							obj.Append(" " + vi.PositionIndex + "/" + vi.UVIndex + "/" + vi.NormalIndex);
						}
						obj.Append("\n");
					}
				}
			}

			return obj;
		}

		#endregion
	}
}
