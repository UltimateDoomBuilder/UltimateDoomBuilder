
#region ================== Copyright (c) 2007 Pascal vd Heiden

/*
 * Copyright (c) 2007 Pascal vd Heiden, www.codeimp.com
 * This program is released under GNU General Public License
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 */

#endregion

#region ================== Namespaces

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Types;

#endregion

namespace CodeImp.DoomBuilder.IO
{
	internal class UniversalStreamReader
	{
		#region ================== Constants

		// Name of the UDMF configuration file
		private const string UDMF_CONFIG_NAME = "UDMF.cfg";

		#endregion

		#region ================== Variables

		private readonly Configuration config;
		private bool setknowncustomtypes;
		private bool strictchecking = true;
		private readonly Dictionary<MapElementType, Dictionary<string, UniversalType>> uifields; //mxd
		
		#endregion

		#region ================== Properties

		public bool SetKnownCustomTypes { get { return setknowncustomtypes; } set { setknowncustomtypes = value; } }
		public bool StrictChecking { get { return strictchecking; } set { strictchecking = value; } }
		
		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public UniversalStreamReader(Dictionary<MapElementType, Dictionary<string, UniversalType>> uifields)
		{
			this.uifields = uifields;
			
			// Make configuration
			config = new Configuration();
			
			// Find a resource named UDMF.cfg
			string[] resnames = General.ThisAssembly.GetManifestResourceNames();
			foreach(string rn in resnames)
			{
				// Found it?
				if(rn.EndsWith(UDMF_CONFIG_NAME, StringComparison.OrdinalIgnoreCase))
				{
					// Get a stream from the resource
					Stream udmfcfg = General.ThisAssembly.GetManifestResourceStream(rn);
					StreamReader udmfcfgreader = new StreamReader(udmfcfg, Encoding.ASCII);

					// Load configuration from stream
					config.InputConfiguration(udmfcfgreader.ReadToEnd());

					// Now we add the linedef flags, activations and thing flags
					// to this list, so that these don't show up in the custom
					// fields list either. We use true as dummy value (it has no meaning)

					// Add linedef flags
					foreach(KeyValuePair<string, string> flag in General.Map.Config.LinedefFlags)
						config.WriteSetting("managedfields.linedef." + flag.Key, true);

					// Add linedef activations
					foreach(LinedefActivateInfo activate in General.Map.Config.LinedefActivates)
						config.WriteSetting("managedfields.linedef." + activate.Key, true);
					
					// Add linedef flag translations
					foreach(FlagTranslation f in General.Map.Config.LinedefFlagsTranslation)
					{
						foreach(string fn in f.Fields)
							config.WriteSetting("managedfields.linedef." + fn, true);
					}

					//mxd. Add sidedef flags
					foreach(KeyValuePair<string, string> flag in General.Map.Config.SidedefFlags)
						config.WriteSetting("managedfields.sidedef." + flag.Key, true);

					//mxd. Add sector flags
					foreach(KeyValuePair<string, string> flag in General.Map.Config.SectorFlags)
						config.WriteSetting("managedfields.sector." + flag.Key, true);
					foreach(KeyValuePair<string, string> flag in General.Map.Config.CeilingPortalFlags)
						config.WriteSetting("managedfields.sector." + flag.Key, true);
					foreach(KeyValuePair<string, string> flag in General.Map.Config.FloorPortalFlags)
						config.WriteSetting("managedfields.sector." + flag.Key, true);

					// Add thing flags
					foreach(KeyValuePair<string, string> flag in General.Map.Config.ThingFlags)
						config.WriteSetting("managedfields.thing." + flag.Key, true);

					// Add thing flag translations
					foreach(FlagTranslation f in General.Map.Config.ThingFlagsTranslation)
					{
						foreach(string fn in f.Fields)
							config.WriteSetting("managedfields.thing." + fn, true);
					}

					// Done
					udmfcfgreader.Dispose();
					break;
				}
			}
		}

		#endregion

		#region ================== Reading

		// This reads from a stream
		public void Read(MapSet map, Stream stream)
		{
			StreamReader reader = new StreamReader(stream, Encoding.ASCII);
			UniversalParser textmap = new UniversalParser();
			textmap.StrictChecking = strictchecking;
			
			// Read UDMF from stream
			List<string> data = new List<string>(1000);
			while(!reader.EndOfStream) data.Add(reader.ReadLine());

			// Parse it
			textmap.InputConfiguration(data.ToArray());

			// Check for errors
			if(textmap.ErrorResult != 0)
			{
				//mxd. Throw parse error
				throw new Exception("Error on line " + textmap.ErrorLine + " while parsing UDMF map data:\n" + textmap.ErrorDescription);
			}

			if(textmap.HasWarnings)
			{
				foreach (string warning in textmap.Warnings)
					General.ErrorLogger.Add(ErrorType.Warning, warning);
			}

			// Read the map
			Dictionary<int, Vertex> vertexlink = ReadVertices(map, textmap);
			Dictionary<int, Sector> sectorlink = ReadSectors(map, textmap);
			ReadLinedefs(map, textmap, vertexlink, sectorlink);
			ReadThings(map, textmap);

			// Read all the stuff we don't know, but have to preserve
			map.UnknownUDMFData = GetUnknownCollections(textmap.Root, new List<string>() { "namespace", "vertex", "sector", "linedef", "sidedef", "thing" });
		}

		// This reads the things
		private void ReadThings(MapSet map, UniversalParser textmap)
		{
			// Get list of entries
			List<UniversalCollection> collections = GetNamedCollections(textmap.Root, "thing");

			// Go for all collections
			map.SetCapacity(0, 0, 0, 0, map.Things.Count + collections.Count);
			for(int i = 0; i < collections.Count; i++)
			{
				// Read fields
				UniversalCollection c = collections[i];
				int[] args = new int[Linedef.NUM_ARGS];
				string where = "thing " + i;
				double x = GetCollectionEntry(c, "x", true, 0.0, where);
				double y = GetCollectionEntry(c, "y", true, 0.0, where);
				double height = GetCollectionEntry(c, "height", false, 0.0, where);
				int tag = GetCollectionEntry(c, "id", false, 0, where);
				int angledeg = GetCollectionEntry(c, "angle", false, 0, where);
				int pitch = GetCollectionEntry(c, "pitch", false, 0, where); //mxd
				int roll = GetCollectionEntry(c, "roll", false, 0, where); //mxd
				double scaleX = GetCollectionEntry(c, "scalex", false, 1.0, where); //mxd
				double scaleY = GetCollectionEntry(c, "scaley", false, 1.0, where); //mxd
				double scale = GetCollectionEntry(c, "scale", false, 0.0, where); //mxd
				int type = GetCollectionEntry(c, "type", true, 0, where);
				int special = GetCollectionEntry(c, "special", false, 0, where);
				args[0] = GetCollectionEntry(c, "arg0", false, 0, where);
				args[1] = GetCollectionEntry(c, "arg1", false, 0, where);
				args[2] = GetCollectionEntry(c, "arg2", false, 0, where);
				args[3] = GetCollectionEntry(c, "arg3", false, 0, where);
				args[4] = GetCollectionEntry(c, "arg4", false, 0, where);

				if(scale != 0) //mxd
				{
					scaleX = scale;
					scaleY = scale;
				}

				// Flags
				Dictionary<string, bool> stringflags = new Dictionary<string, bool>(StringComparer.Ordinal);
				foreach(KeyValuePair<string, string> flag in General.Map.Config.ThingFlags)
					stringflags[flag.Key] = GetCollectionEntry(c, flag.Key, false, false, where);
				foreach(FlagTranslation ft in General.Map.Config.ThingFlagsTranslation)
				{
					foreach(string field in ft.Fields)
						stringflags[field] = GetCollectionEntry(c, field, false, false, where);
				}

				// Create new item
				Thing t = map.CreateThing();
				if(t != null)
				{
					t.Update(type, x, y, height, angledeg, pitch, roll, scaleX, scaleY, stringflags, 0, tag, special, args);

					// Custom fields
					ReadCustomFields(c, t, "thing");
				}
			}
		}

		// This reads the linedefs and sidedefs
		private void ReadLinedefs(MapSet map, UniversalParser textmap,
			Dictionary<int, Vertex> vertexlink, Dictionary<int, Sector> sectorlink)
		{
			// Get list of entries
			List<UniversalCollection> linescolls = GetNamedCollections(textmap.Root, "linedef");
			List<UniversalCollection> sidescolls = GetNamedCollections(textmap.Root, "sidedef");

			// Go for all lines
			map.SetCapacity(0, map.Linedefs.Count + linescolls.Count, map.Sidedefs.Count + sidescolls.Count, 0, 0);
			char[] splitter = { ' ' }; //mxd
			for(int i = 0; i < linescolls.Count; i++)
			{
				// Read fields
				UniversalCollection lc = linescolls[i];
				int[] args = new int[Linedef.NUM_ARGS];
				string where = "linedef " + i;
				int v1 = GetCollectionEntry(lc, "v1", true, 0, where);
				int v2 = GetCollectionEntry(lc, "v2", true, 0, where);

				if(!vertexlink.ContainsKey(v1) || !vertexlink.ContainsKey(v2))
				{ //mxd
					General.ErrorLogger.Add(ErrorType.Warning, "Linedef " + i + " references one or more invalid vertices. Linedef has been removed.");
					continue;
				}

				int tag = GetCollectionEntry(lc, "id", false, 0, where);
				int special = GetCollectionEntry(lc, "special", false, 0, where);
				args[0] = GetCollectionEntry(lc, "arg0", false, 0, where);
				args[1] = GetCollectionEntry(lc, "arg1", false, 0, where);
				args[2] = GetCollectionEntry(lc, "arg2", false, 0, where);
				args[3] = GetCollectionEntry(lc, "arg3", false, 0, where);
				args[4] = GetCollectionEntry(lc, "arg4", false, 0, where);
				int s1 = GetCollectionEntry(lc, "sidefront", false, -1, where);
				int s2 = GetCollectionEntry(lc, "sideback", false, -1, where);

				//mxd. MoreIDs
				List<int> tags = new List<int> { tag };
				string moreids = GetCollectionEntry(lc, "moreids", false, string.Empty, where);
				if(!string.IsNullOrEmpty(moreids))
				{
					string[] moreidscol = moreids.Split(splitter, StringSplitOptions.RemoveEmptyEntries);
					foreach(string sid in moreidscol)
					{
						int id;
						if(int.TryParse(sid.Trim(), out id) && id != 0 && !tags.Contains(id))
						{
							tags.Add(id);
						}
					}
				}
				if(tag == 0 && tags.Count > 1) tags.RemoveAt(0);

				// Flags
				Dictionary<string, bool> stringflags = new Dictionary<string, bool>(StringComparer.Ordinal);
				foreach(KeyValuePair<string, string> flag in General.Map.Config.LinedefFlags)
					stringflags[flag.Key] = GetCollectionEntry(lc, flag.Key, false, false, where);
				foreach(FlagTranslation ft in General.Map.Config.LinedefFlagsTranslation)
				{
					foreach(string field in ft.Fields)
						stringflags[field] = GetCollectionEntry(lc, field, false, false, where);
				}
				
				// Activations
				foreach(LinedefActivateInfo activate in General.Map.Config.LinedefActivates)
					stringflags[activate.Key] = GetCollectionEntry(lc, activate.Key, false, false, where);

				// Check if not zero-length
				if(Vector2D.ManhattanDistance(vertexlink[v1].Position, vertexlink[v2].Position) > 0.0001f) 
				{
					// Create new linedef
					Linedef l = map.CreateLinedef(vertexlink[v1], vertexlink[v2]);
					if(l != null)
					{
						l.Update(stringflags, 0, 0,tags, special, args);
						l.UpdateCache();

						// Custom fields
						ReadCustomFields(lc, l, "linedef");

						// Read sidedefs and connect them to the line
						if(s1 > -1)
						{
							if(s1 < sidescolls.Count) 
								ReadSidedef(map, sidescolls[s1], l, true, sectorlink, s1);
							else
								General.ErrorLogger.Add(ErrorType.Warning, "Linedef " + i + " references invalid front sidedef " + s1 + ". Sidedef has been removed.");
						}

						if(s2 > -1)
						{
							if(s2 < sidescolls.Count) 
								ReadSidedef(map, sidescolls[s2], l, false, sectorlink, s2);
							else
								General.ErrorLogger.Add(ErrorType.Warning, "Linedef " + i + " references invalid back sidedef " + s1 + ". Sidedef has been removed.");
						}
					}
				} 
				else 
				{
					General.ErrorLogger.Add(ErrorType.Warning, "Linedef " + i + " is zero-length. Linedef has been removed.");
				}
			}
		}

		// This reads a single sidedef and connects it to the given linedef
		private void ReadSidedef(MapSet map, UniversalCollection sc, Linedef ld,
								 bool front, Dictionary<int, Sector> sectorlink, int index)
		{
			// Read fields
			string where = "linedef " + ld.Index + (front ? " front sidedef " : " back sidedef ") + index;
			int offsetx = GetCollectionEntry(sc, "offsetx", false, 0, where);
			int offsety = GetCollectionEntry(sc, "offsety", false, 0, where);
			string thigh = GetCollectionEntry(sc, "texturetop", false, "-", where);
			string tlow = GetCollectionEntry(sc, "texturebottom", false, "-", where);
			string tmid = GetCollectionEntry(sc, "texturemiddle", false, "-", where);
			int sector = GetCollectionEntry(sc, "sector", true, 0, where);

			//mxd. Flags
			Dictionary<string, bool> stringflags = new Dictionary<string, bool>(StringComparer.Ordinal);
			foreach(KeyValuePair<string, string> flag in General.Map.Config.SidedefFlags)
				stringflags[flag.Key] = GetCollectionEntry(sc, flag.Key, false, false, where);

			// Create sidedef
			if(sectorlink.ContainsKey(sector))
			{
				Sidedef s = map.CreateSidedef(ld, front, sectorlink[sector]);
				if(s != null)
				{
					s.Update(offsetx, offsety, thigh, tmid, tlow, stringflags);

					// Custom fields
					ReadCustomFields(sc, s, "sidedef");
				}
			}
			else
			{
				General.ErrorLogger.Add(ErrorType.Warning, "Sidedef references invalid sector " + sector + ". Sidedef has been removed.");
			}
		}

		// This reads the sectors
		private Dictionary<int, Sector> ReadSectors(MapSet map, UniversalParser textmap)
		{
			// Get list of entries
			List<UniversalCollection> collections = GetNamedCollections(textmap.Root, "sector");

			// Create lookup table
			Dictionary<int, Sector> link = new Dictionary<int, Sector>(collections.Count);

			// Go for all collections
			map.SetCapacity(0, 0, 0, map.Sectors.Count + collections.Count, 0);
			char[] splitter = new[] { ' ' }; //mxd
			for(int i = 0; i < collections.Count; i++)
			{
				// Read fields
				UniversalCollection c = collections[i];
				string where = "sector " + i;
				int hfloor = GetCollectionEntry(c, "heightfloor", false, 0, where);
				int hceil = GetCollectionEntry(c, "heightceiling", false, 0, where);
				string tfloor = GetCollectionEntry(c, "texturefloor", true, "-", where);
				string tceil = GetCollectionEntry(c, "textureceiling", true, "-", where);
				int bright = GetCollectionEntry(c, "lightlevel", false, 160, where);
				int special = GetCollectionEntry(c, "special", false, 0, where);
				int tag = GetCollectionEntry(c, "id", false, 0, where);

				//mxd. MoreIDs
				List<int> tags = new List<int> { tag };
				string moreids = GetCollectionEntry(c, "moreids", false, string.Empty, where);
				if(!string.IsNullOrEmpty(moreids)) 
				{
					string[] moreidscol = moreids.Split(splitter, StringSplitOptions.RemoveEmptyEntries);
					foreach(string sid in moreidscol)
					{
						int id;
						if(int.TryParse(sid.Trim(), out id) && id != 0 && !tags.Contains(id)) 
						{
							tags.Add(id);
						}
					}
				}
				if(tag == 0 && tags.Count > 1) tags.RemoveAt(0);

				//mxd. Read slopes
				double fslopex = GetCollectionEntry(c, "floorplane_a", false, 0.0, where);
				double fslopey = GetCollectionEntry(c, "floorplane_b", false, 0.0, where);
				double fslopez = GetCollectionEntry(c, "floorplane_c", false, 0.0, where);
				double foffset = GetCollectionEntry(c, "floorplane_d", false, double.NaN, where);

				double cslopex = GetCollectionEntry(c, "ceilingplane_a", false, 0.0, where);
				double cslopey = GetCollectionEntry(c, "ceilingplane_b", false, 0.0, where);
				double cslopez = GetCollectionEntry(c, "ceilingplane_c", false, 0.0, where);
				double coffset = GetCollectionEntry(c, "ceilingplane_d", false, double.NaN, where);

				//mxd. Read flags
				Dictionary<string, bool> stringflags = new Dictionary<string, bool>(StringComparer.Ordinal);
				foreach(KeyValuePair<string, string> flag in General.Map.Config.SectorFlags)
					stringflags[flag.Key] = GetCollectionEntry(c, flag.Key, false, false, where);
				foreach(KeyValuePair<string, string> flag in General.Map.Config.CeilingPortalFlags)
					stringflags[flag.Key] = GetCollectionEntry(c, flag.Key, false, false, where);
				foreach(KeyValuePair<string, string> flag in General.Map.Config.FloorPortalFlags)
					stringflags[flag.Key] = GetCollectionEntry(c, flag.Key, false, false, where);

				// Create new item
				Sector s = map.CreateSector();
				if(s != null)
				{
					s.Update(hfloor, hceil, tfloor, tceil, special, stringflags, tags, bright, foffset, new Vector3D(fslopex, fslopey, fslopez).GetNormal(), coffset, new Vector3D(cslopex, cslopey, cslopez).GetNormal());

					// Custom fields
					ReadCustomFields(c, s, "sector");

					// Add it to the lookup table
					link.Add(i, s);
				}
			}

			// Return lookup table
			return link;
		}

		// This reads the vertices
		private Dictionary<int, Vertex> ReadVertices(MapSet map, UniversalParser textmap)
		{
			// Get list of entries
			List<UniversalCollection> collections = GetNamedCollections(textmap.Root, "vertex");

			// Create lookup table
			Dictionary<int, Vertex> link = new Dictionary<int, Vertex>(collections.Count);

			// Go for all collections
			map.SetCapacity(map.Vertices.Count + collections.Count, 0, 0, 0, 0);
			for(int i = 0; i < collections.Count; i++)
			{
				// Read fields
				UniversalCollection c = collections[i];
				string where = "vertex " + i;
				double x = GetCollectionEntry(c, "x", true, 0.0, where);
				double y = GetCollectionEntry(c, "y", true, 0.0, where);

                // [ZZ] Correct location if it's NaN. Note that there cannot be any meaningful value here, so I just reset it to 0,0 to avoid triggering the NaN exception
                //      TODO: remove once the cause of NaN is reported
                if (double.IsNaN(x) || double.IsNaN(y))
                {
                    x = y = 0.0;
                    General.ErrorLogger.Add(ErrorType.Warning, string.Format("Vertex {0} has NaN coordinates, resetting to 0,0", i));
                }

				// Create new item
				Vertex v = map.CreateVertex(new Vector2D(x, y));
				if(v != null)
				{
					//mxd. zoffsets
					v.ZCeiling = GetCollectionEntry(c, "zceiling", false, double.NaN, where); //mxd
					v.ZFloor = GetCollectionEntry(c, "zfloor", false, double.NaN, where); //mxd
					
					// Custom fields
					ReadCustomFields(c, v, "vertex");

					// Add it to the lookup table
					link.Add(i, v);
				}
			}

			// Return lookup table
			return link;
		}

		// This reads custom fields from a collection and adds them to a map element
		private void ReadCustomFields(UniversalCollection collection, MapElement element, string elementname)
		{
			element.Fields.BeforeFieldsChange();
			
			// Go for all the elements in the collection
			foreach(UniversalEntry e in collection)
			{
				// mxd. Check if uifield
				if(uifields.ContainsKey(element.ElementType) && uifields[element.ElementType].ContainsKey(e.Key)) 
				{
					int type = (int)uifields[element.ElementType][e.Key];

					//mxd. Check type
					object value = e.Value;

					// Let's be kind and cast any int to a float if needed
					if(type == (int)UniversalType.Float && e.Value is int) 
					{
						value = (double)(int)e.Value;
					} 
					else if(!e.IsValidType(e.Value.GetType())) 
					{
						General.ErrorLogger.Add(ErrorType.Warning, element + ": the value of entry \"" + e.Key + "\" is of incompatible type (expected " + e.GetType().Name + ", but got " + e.Value.GetType().Name + "). If you save the map, this value will be ignored.");
						continue;
					}

					// Make custom field
					element.Fields[e.Key] = new UniValue(type, value);

				} // Check if not a managed field
				else if(!config.SettingExists("managedfields." + elementname + "." + e.Key)) 
				{
					int type = (int)UniversalType.Integer;

					//mxd. Try to find the type from configuration
					if(setknowncustomtypes) 
					{
						type = General.Map.Options.GetUniversalFieldType(elementname, e.Key, -1);

						if(type != -1) 
						{
							object value = e.Value;

							// Let's be kind and cast any int to a float if needed
							if(type == (int)UniversalType.Float && e.Value is int) 
							{
								value = (float)(int)e.Value;
							} 
							else if(!e.IsValidType(e.Value.GetType())) 
							{
								General.ErrorLogger.Add(ErrorType.Warning, element + ": the value of entry \"" + e.Key + "\" is of incompatible type (expected " + e.GetType().Name + ", but got " + e.Value.GetType().Name + "). If you save the map, this value will be ignored.");
								continue;
							}

							// Make custom field
							element.Fields[e.Key] = new UniValue(type, value);
							continue;
						}
					}

					// Determine default type
					if(e.Value is int) type = (int)UniversalType.Integer;
					else if(e.Value is double) type = (int)UniversalType.Float;
					else if(e.Value is bool) type = (int)UniversalType.Boolean;
					else if(e.Value is string) type = (int)UniversalType.String;

					// Make custom field
					element.Fields[e.Key] = new UniValue(type, e.Value);
				}
			}
		}

		// This validates and returns an entry
		private static T GetCollectionEntry<T>(UniversalCollection c, string entryname, bool required, T defaultvalue, string where)
		{
			T result = default(T);
			bool found = false;

			// Find the entry
			foreach(UniversalEntry e in c)
			{
				// Check if matches
				if(e.Key == entryname)
				{
					// Let's be kind and cast any int to a float if needed
					if((typeof(T) == typeof(double)) && (e.Value is int))
					{
						// Make it a float
						object fvalue = (double)(int)e.Value;
						result = (T)fvalue;
					}
					else
					{
						// Verify type
						e.ValidateType(typeof(T));

						// Found it!
						result = (T)e.Value;
					}

					// Done
					found = true;
				}
			}

			// Not found?
			if(!found)
			{
				// Report error when entry is required!
				if(required)
					General.ErrorLogger.Add(ErrorType.Error, "Error while reading UDMF map data: Missing required field \"" + entryname + "\" at " + where + ".");

				// Make default entry
				result = defaultvalue;
			}

			// Return result
			return result;
		}

		// This makes a list of all collections with the given name
		private static List<UniversalCollection> GetNamedCollections(UniversalCollection collection, string entryname)
		{
			List<UniversalCollection> list = new List<UniversalCollection>();

			// Make list
			foreach(UniversalEntry e in collection) 
			{
				UniversalCollection uc = e.Value as UniversalCollection;
				if(uc != null && e.Key == entryname) list.Add(uc);
			}

			return list;
		}

		/// <summary>
		/// Get unknown fields and blocks from an universal collection.
		/// </summary>
		/// <param name="collection">Universal collection to get the fields and blocks from</param>
		/// <param name="knowncollections">List of fields and block that are known and can be ignored</param>
		/// <returns></returns>
		private static List<UniversalEntry> GetUnknownCollections(UniversalCollection collection, List<string> knowncollections)
		{
			List<UniversalEntry> list = new List<UniversalEntry>();

			// Make list
			foreach (UniversalEntry e in collection)
			{
				if (!knowncollections.Contains(e.Key)) list.Add(e);
			}

			return list;
		}

		#endregion
	}
}

