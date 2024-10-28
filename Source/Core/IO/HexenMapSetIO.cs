
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
using System.IO;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Types;

#endregion

namespace CodeImp.DoomBuilder.IO
{
	internal class HexenMapSetIO : MapSetIO
	{
		#region ================== Constants

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public HexenMapSetIO(WAD wad, MapManager manager) : base(wad, manager) { }

		#endregion

		#region ================== Properties

		public override int MaxSidedefs { get { return ushort.MaxValue; } }
		public override int MaxVertices { get { return ushort.MaxValue; } }
		public override int MaxLinedefs { get { return ushort.MaxValue; } }
		public override int MaxSectors { get { return ushort.MaxValue; } }
		public override int MaxThings { get { return int.MaxValue; } }
		public override int MinTextureOffset { get { return short.MinValue; } }
		public override int MaxTextureOffset { get { return short.MaxValue; } }
		public override int VertexDecimals { get { return 0; } }
		public override string DecimalsFormat { get { return "0"; } }
		public override bool HasLinedefTag { get { return false; } }
		public override bool HasThingTag { get { return true; } }
		public override bool HasThingAction { get { return true; } }
		public override bool HasCustomFields { get { return false; } }
		public override bool HasThingHeight { get { return true; } }
		public override bool HasActionArgs { get { return true; } }
		public override bool HasMixedActivations { get { return false; } }
		public override bool HasPresetActivations { get { return true; } }
		public override bool HasBuiltInActivations { get { return false; } }
		public override bool HasNumericLinedefFlags { get { return true; } }
		public override bool HasNumericThingFlags { get { return true; } }
		public override bool HasNumericLinedefActivations { get { return true; } }
		public override int MaxTag { get { return ushort.MaxValue; } }
		public override int MinTag { get { return ushort.MinValue; } }
		public override int MaxAction { get { return byte.MaxValue; } }
		public override int MinAction { get { return byte.MinValue; } }
		public override int MaxArgument { get { return byte.MaxValue; } }
		public override int MinArgument { get { return byte.MinValue; } }
		public override int MaxEffect { get { return ushort.MaxValue; } }
		public override int MinEffect { get { return ushort.MinValue; } }
		public override int MaxBrightness { get { return short.MaxValue; } }
		public override int MinBrightness { get { return short.MinValue; } }
		public override int MaxThingType { get { return short.MaxValue; } } //mxd. Editor numbers must be in [1 .. 32767] range
		public override int MinThingType { get { return 1; } } //mxd
		public override float MaxCoordinate { get { return short.MaxValue; } }
		public override float MinCoordinate { get { return short.MinValue; } }
		public override int MaxThingAngle { get { return short.MaxValue; } }
		public override int MinThingAngle { get { return short.MinValue; } }
		public override Dictionary<MapElementType, Dictionary<string, UniversalType>> UIFields { get { return uifields; } } //mxd
		
		#endregion

		#region ================== Reading

		// This reads a map from the file and returns a MapSet
		public override MapSet Read(MapSet map, string mapname)
		{
			// Find the index where first map lump begins
			int firstindex = wad.FindLumpIndex(mapname) + 1;

			// Read vertices
			Dictionary<int, Vertex> vertexlink = ReadVertices(map, firstindex);

			// Read sectors
			Dictionary<int, Sector> sectorlink = ReadSectors(map, firstindex);

			// Read linedefs and sidedefs
			ReadLinedefs(map, firstindex, vertexlink, sectorlink);

			// Read things
			ReadThings(map, firstindex);
			
			// Remove unused vertices
			map.RemoveUnusedVertices();
			
			// Return result;
			return map;
		}

		// This reads the THINGS from WAD file
		private void ReadThings(MapSet map, int firstindex)
		{
			int[] args = new int[Thing.NUM_ARGS];

			// Get the lump from wad file
			Lump lump = wad.FindLump("THINGS", firstindex);
			if(lump == null) throw new Exception("Could not find required lump THINGS!");
			
			// Prepare to read the items
			MemoryStream mem = new MemoryStream(lump.Stream.ReadAllBytes());
			int num = (int)lump.Stream.Length / 20;
			BinaryReader reader = new BinaryReader(mem);

			// Sanity check against the defined maximum
			if (num > General.Map.FormatInterface.MaxThings)
			{
				General.ErrorLogger.Add(ErrorType.Warning, "There are " + num + " thing entries in the THINGS lump, exceeding the limit of " + General.Map.FormatInterface.MaxThings + " entries. Things after this limit will be ignored");
				num = General.Map.FormatInterface.MaxThings;
			}

			// Read items from the lump
			map.SetCapacity(0, 0, 0, 0, map.Things.Count + num);
			for(int i = 0; i < num; i++)
			{
				// Read properties from stream
				int tag = reader.ReadUInt16();
				int x = reader.ReadInt16();
				int y = reader.ReadInt16();
				int z = reader.ReadInt16();
				int angle = reader.ReadInt16();
				int type = reader.ReadUInt16();
				ushort flags = reader.ReadUInt16();
				int action = reader.ReadByte();
				args[0] = reader.ReadByte();
				args[1] = reader.ReadByte();
				args[2] = reader.ReadByte();
				args[3] = reader.ReadByte();
				args[4] = reader.ReadByte();

				// Make string flags
				Dictionary<string, bool> stringflags = new Dictionary<string, bool>(StringComparer.Ordinal);
				foreach(KeyValuePair<string, string> f in manager.Config.ThingFlags)
				{
					if (int.TryParse(f.Key, out int fnum)) stringflags[f.Key] = ((flags & fnum) == fnum);
				}
				
				// Create new item
				Thing t = map.CreateThing();
				t.Update(type, x, y, z, angle, 0, 0, 1.0f, 1.0f, stringflags, flags, tag, action, args);
			}

			// Done
			mem.Dispose();
		}

		// This reads the VERTICES from WAD file
		// Returns a lookup table with indices
		private Dictionary<int, Vertex> ReadVertices(MapSet map, int firstindex)
		{
			// Get the lump from wad file
			Lump lump = wad.FindLump("VERTEXES", firstindex);
			if(lump == null) throw new Exception("Could not find required lump VERTEXES!");

			// Prepare to read the items
			MemoryStream mem = new MemoryStream(lump.Stream.ReadAllBytes());
			int num = (int)lump.Stream.Length / 4;
			BinaryReader reader = new BinaryReader(mem);

			// There are some maps that have more than 65536 vertices, for example MAP32 of https://www.doomworld.com/forum/topic/106186-frog_and_toadwad-12-maps/
			// Vertices after this limit should not be read, as they can't be referenced by linedefs anyway, and cause massive slowdowns while loading (likely due to
			// CreateVertex not creating vertices past the limit and trying to play a warning sound)
			// Also see https://github.com/jewalky/UltimateDoomBuilder/issues/552
			if (num > General.Map.FormatInterface.MaxVertices)
			{
				General.ErrorLogger.Add(ErrorType.Warning, "There are " + num + " vertex entries in the VERTEXES lump, exceeding the limit of " + General.Map.FormatInterface.MaxVertices + " entries. Vertices after this limit will be ignored");
				num = General.Map.FormatInterface.MaxVertices;
			}

			// Create lookup table
			Dictionary<int, Vertex> link = new Dictionary<int, Vertex>(num);

			// Read items from the lump
			map.SetCapacity(map.Vertices.Count + num, 0, 0, 0, 0);
			for(int i = 0; i < num; i++)
			{
				// Read properties from stream
				int x = reader.ReadInt16();
				int y = reader.ReadInt16();

				// Create new item
				Vertex v = map.CreateVertex(new Vector2D(x, y));
				
				// Add it to the lookup table
				link.Add(i, v);
			}

			// Done
			mem.Dispose();

			// Return lookup table
			return link;
		}

		// This reads the SECTORS from WAD file
		// Returns a lookup table with indices
		private Dictionary<int, Sector> ReadSectors(MapSet map, int firstindex)
		{
			// Get the lump from wad file
			Lump lump = wad.FindLump("SECTORS", firstindex);
			if(lump == null) throw new Exception("Could not find required lump SECTORS!");

			// Prepare to read the items
			MemoryStream mem = new MemoryStream(lump.Stream.ReadAllBytes());
			int num = (int)lump.Stream.Length / 26;
			BinaryReader reader = new BinaryReader(mem);

			// Sanity check against the defined maximum
			if (num > General.Map.FormatInterface.MaxSectors)
			{
				General.ErrorLogger.Add(ErrorType.Warning, "There are " + num + " sector entries in the SECTORS lump, exceeding the limit of " + General.Map.FormatInterface.MaxSectors + " entries. Sectors after this limit will be ignored");
				num = General.Map.FormatInterface.MaxSectors;
			}

			// Create lookup table
			Dictionary<int, Sector> link = new Dictionary<int, Sector>(num);

			// Read items from the lump
			map.SetCapacity(0, 0, 0, map.Sectors.Count + num, 0);
			for(int i = 0; i < num; i++)
			{
				// Read properties from stream
				int hfloor = reader.ReadInt16();
				int hceil = reader.ReadInt16();
				string tfloor = Lump.MakeNormalName(reader.ReadBytes(8), WAD.ENCODING);
				string tceil = Lump.MakeNormalName(reader.ReadBytes(8), WAD.ENCODING);
				int bright = reader.ReadInt16();
				int special = reader.ReadUInt16();
				int tag = reader.ReadUInt16();
				
				// Create new item
				Sector s = map.CreateSector();
				s.Update(hfloor, hceil, tfloor, tceil, special, tag, bright);

				// Add it to the lookup table
				link.Add(i, s);
			}

			// Done
			mem.Dispose();

			// Return lookup table
			return link;
		}
		
		// This reads the LINEDEFS and SIDEDEFS from WAD file
		private void ReadLinedefs(MapSet map, int firstindex,
			Dictionary<int, Vertex> vertexlink, Dictionary<int, Sector> sectorlink)
		{
			int[] args = new int[Linedef.NUM_ARGS];

			// Get the linedefs lump from wad file
			Lump linedefslump = wad.FindLump("LINEDEFS", firstindex);
			if(linedefslump == null) throw new Exception("Could not find required lump LINEDEFS!");

			// Get the sidedefs lump from wad file
			Lump sidedefslump = wad.FindLump("SIDEDEFS", firstindex);
			if(sidedefslump == null) throw new Exception("Could not find required lump SIDEDEFS!");

			// Prepare to read the items
			MemoryStream linedefsmem = new MemoryStream(linedefslump.Stream.ReadAllBytes());
			MemoryStream sidedefsmem = new MemoryStream(sidedefslump.Stream.ReadAllBytes());
			int num = (int)linedefslump.Stream.Length / 16;
			int numsides = (int)sidedefslump.Stream.Length / 30;
			BinaryReader readline = new BinaryReader(linedefsmem);
			BinaryReader readside = new BinaryReader(sidedefsmem);

			// Sanity check against the defined maximum
			if (num > General.Map.FormatInterface.MaxLinedefs)
			{
				General.ErrorLogger.Add(ErrorType.Warning, "There are " + num + " linedef entries in the LINEDEFS lump, exceeding the limit of " + General.Map.FormatInterface.MaxLinedefs + " entries. Linedefs after this limit will be ignored");
				num = General.Map.FormatInterface.MaxLinedefs;
			}

			if (numsides > General.Map.FormatInterface.MaxSidedefs)
			{
				General.ErrorLogger.Add(ErrorType.Warning, "There are " + numsides + " sidedef entries in the SIDEDEFS lump, exceeding the limit of " + General.Map.FormatInterface.MaxSidedefs + " entries. Sidedefs after this limit will be ignored");
				numsides = General.Map.FormatInterface.MaxSidedefs;
			}

			// Read items from the lump
			map.SetCapacity(0, map.Linedefs.Count + num, map.Sidedefs.Count + numsides, 0, 0);
			for(int i = 0; i < num; i++)
			{
				// Read properties from stream
				int v1 = readline.ReadUInt16();
				int v2 = readline.ReadUInt16();
				ushort flags = readline.ReadUInt16();
				int action = readline.ReadByte();
				args[0] = readline.ReadByte();
				args[1] = readline.ReadByte();
				args[2] = readline.ReadByte();
				args[3] = readline.ReadByte();
				args[4] = readline.ReadByte();
				int s1 = readline.ReadUInt16();
				int s2 = readline.ReadUInt16();
				
				// Make string flags
				Dictionary<string, bool> stringflags = new Dictionary<string, bool>(StringComparer.Ordinal);
				foreach(string f in manager.Config.SortedLinedefFlags)
				{
					int fnum;
					if(int.TryParse(f, out fnum)) stringflags[f] = ((flags & fnum) == fnum);
				}
				
				// Create new linedef
				if(vertexlink.ContainsKey(v1) && vertexlink.ContainsKey(v2))
				{
					// Check if not zero-length
					if(Vector2D.ManhattanDistance(vertexlink[v1].Position, vertexlink[v2].Position) > 0.0001f)
					{
						Linedef l = map.CreateLinedef(vertexlink[v1], vertexlink[v2]);
						l.Update(stringflags, flags, (flags & manager.Config.LinedefActivationsFilter), new List<int> { 0 }, action, args);
						l.UpdateCache();

						Sidedef s;
						string thigh, tmid, tlow;
						int offsetx, offsety, sc;

						// Line has a front side?
						if(s1 != ushort.MaxValue)
						{
							// Read front sidedef
							sidedefsmem.Seek(s1 * 30, SeekOrigin.Begin);
							if((s1 * 30L) <= (sidedefsmem.Length - 30L))
							{
								offsetx = readside.ReadInt16();
								offsety = readside.ReadInt16();
								thigh = Lump.MakeNormalName(readside.ReadBytes(8), WAD.ENCODING);
								tlow = Lump.MakeNormalName(readside.ReadBytes(8), WAD.ENCODING);
								tmid = Lump.MakeNormalName(readside.ReadBytes(8), WAD.ENCODING);
								sc = readside.ReadUInt16();

								// Create front sidedef
								if(sectorlink.ContainsKey(sc))
								{
									s = map.CreateSidedef(l, true, sectorlink[sc]);
									s.Update(offsetx, offsety, thigh, tmid, tlow);
								}
								else
								{
									General.ErrorLogger.Add(ErrorType.Warning, "Sidedef " + s1 + " references invalid sector " + sc + ". Sidedef has been removed.");
								}
							}
							else
							{
								General.ErrorLogger.Add(ErrorType.Warning, "Linedef " + i + " references invalid sidedef " + s1 + ". Sidedef has been removed.");
							}
						}

						// Line has a back side?
						if(s2 != ushort.MaxValue)
						{
							// Read back sidedef
							sidedefsmem.Seek(s2 * 30, SeekOrigin.Begin);
							if((s2 * 30L) <= (sidedefsmem.Length - 30L))
							{
								offsetx = readside.ReadInt16();
								offsety = readside.ReadInt16();
								thigh = Lump.MakeNormalName(readside.ReadBytes(8), WAD.ENCODING);
								tlow = Lump.MakeNormalName(readside.ReadBytes(8), WAD.ENCODING);
								tmid = Lump.MakeNormalName(readside.ReadBytes(8), WAD.ENCODING);
								sc = readside.ReadUInt16();

								// Create back sidedef
								if(sectorlink.ContainsKey(sc))
								{
									s = map.CreateSidedef(l, false, sectorlink[sc]);
									s.Update(offsetx, offsety, thigh, tmid, tlow);
								}
								else
								{
									General.ErrorLogger.Add(ErrorType.Warning, "Sidedef " + s2 + " references invalid sector " + sc + ". Sidedef has been removed.");
								}
							}
							else
							{
								General.ErrorLogger.Add(ErrorType.Warning, "Linedef " + i + " references invalid sidedef " + s2 + ". Sidedef has been removed.");
							}
						}
					}
					else
					{
						General.ErrorLogger.Add(ErrorType.Warning, "Linedef " + i + " is zero-length. Linedef has been removed.");
					}
				}
				else
				{
					General.ErrorLogger.Add(ErrorType.Warning, "Linedef " + i + " references one or more invalid vertices. Linedef has been removed.");
				}
			}

			// Done
			linedefsmem.Dispose();
			sidedefsmem.Dispose();
		}
		
		#endregion

		#region ================== Writing

		// This writes a MapSet to the file
		public override void Write(MapSet map, string mapname, int position)
		{
			Dictionary<Vertex, int> vertexids = new Dictionary<Vertex,int>();
			Dictionary<Sidedef, int> sidedefids = new Dictionary<Sidedef,int>();
			Dictionary<Sector, int> sectorids = new Dictionary<Sector,int>();
			
			// First index everything
			foreach(Vertex v in map.Vertices) vertexids.Add(v, vertexids.Count);
			foreach(Sidedef sd in map.Sidedefs) sidedefids.Add(sd, sidedefids.Count);
			foreach(Sector s in map.Sectors) sectorids.Add(s, sectorids.Count);
			
			// Write lumps to wad (note the backwards order because they
			// are all inserted at position+1 when not found)
			WriteSectors(map, position, manager.Config.MapLumps);
			WriteVertices(map, position, manager.Config.MapLumps);
			WriteSidedefs(map, position, manager.Config.MapLumps, sectorids);
			WriteLinedefs(map, position, manager.Config.MapLumps, sidedefids, vertexids);
			WriteThings(map, position, manager.Config.MapLumps);
		}

		// This writes the THINGS to WAD file
		private void WriteThings(MapSet map, int position, Dictionary<string, MapLumpInfo> maplumps)
		{
			// Create memory to write to
			MemoryStream mem = new MemoryStream();
			BinaryWriter writer = new BinaryWriter(mem, WAD.ENCODING);
			
			// Go for all things
			foreach(Thing t in map.Things)
			{
				t.UpdateRawFlagsFromFlags();

				// Write properties to stream
				writer.Write((UInt16)t.Tag);
				writer.Write((Int16)t.Position.x);
				writer.Write((Int16)t.Position.y);
				writer.Write((Int16)t.Position.z);
				writer.Write((Int16)t.AngleDoom);
				writer.Write((UInt16)t.Type);
				writer.Write(t.RawFlags);
				writer.Write((Byte)t.Action);
				writer.Write((Byte)t.Args[0]);
				writer.Write((Byte)t.Args[1]);
				writer.Write((Byte)t.Args[2]);
				writer.Write((Byte)t.Args[3]);
				writer.Write((Byte)t.Args[4]);
			}
			
			// Find insert position and remove old lump
			int insertpos = MapManager.RemoveSpecificLump(wad, "THINGS", position, MapManager.TEMP_MAP_HEADER, maplumps);
			if(insertpos == -1) insertpos = position + 1;
			if(insertpos > wad.Lumps.Count) insertpos = wad.Lumps.Count;
			
			// Create the lump from memory
			Lump lump = wad.Insert("THINGS", insertpos, (int)mem.Length);
			lump.Stream.Seek(0, SeekOrigin.Begin);
			mem.WriteTo(lump.Stream);
		}

		// This writes the VERTEXES to WAD file
		private void WriteVertices(MapSet map, int position, Dictionary<string, MapLumpInfo> maplumps)
		{
			// Create memory to write to
			MemoryStream mem = new MemoryStream();
			BinaryWriter writer = new BinaryWriter(mem, WAD.ENCODING);

			// Go for all vertices
			foreach(Vertex v in map.Vertices)
			{
				// Write properties to stream
				writer.Write((Int16)(int)Math.Round(v.Position.x));
				writer.Write((Int16)(int)Math.Round(v.Position.y));
			}

			// Find insert position and remove old lump
			int insertpos = MapManager.RemoveSpecificLump(wad, "VERTEXES", position, MapManager.TEMP_MAP_HEADER, maplumps);
			if(insertpos == -1) insertpos = position + 1;
			if(insertpos > wad.Lumps.Count) insertpos = wad.Lumps.Count;

			// Create the lump from memory
			Lump lump = wad.Insert("VERTEXES", insertpos, (int)mem.Length);
			lump.Stream.Seek(0, SeekOrigin.Begin);
			mem.WriteTo(lump.Stream);
		}

		// This writes the LINEDEFS to WAD file
		private void WriteLinedefs(MapSet map, int position, Dictionary<string, MapLumpInfo> maplumps, IDictionary<Sidedef, int> sidedefids, IDictionary<Vertex, int> vertexids)
		{
			// Create memory to write to
			MemoryStream mem = new MemoryStream();
			BinaryWriter writer = new BinaryWriter(mem, WAD.ENCODING);

			// Go for all lines
			foreach(Linedef l in map.Linedefs)
			{
				l.UpdateRawFlagsFromFlags();
				ushort flags = l.RawFlags;

				// Add activates to flags
				flags |= (ushort)(l.Activate & manager.Config.LinedefActivationsFilter);
				
				// Write properties to stream
				writer.Write((UInt16)vertexids[l.Start]);
				writer.Write((UInt16)vertexids[l.End]);
				writer.Write((UInt16)flags);
				writer.Write((Byte)l.Action);
				writer.Write((Byte)l.Args[0]);
				writer.Write((Byte)l.Args[1]);
				writer.Write((Byte)l.Args[2]);
				writer.Write((Byte)l.Args[3]);
				writer.Write((Byte)l.Args[4]);

				// Front sidedef
				ushort sid = (l.Front == null ? ushort.MaxValue : (UInt16)sidedefids[l.Front]);
				writer.Write(sid);

				// Back sidedef
				sid = (l.Back == null ? ushort.MaxValue : (UInt16)sidedefids[l.Back]);
				writer.Write(sid);
			}

			// Find insert position and remove old lump
			int insertpos = MapManager.RemoveSpecificLump(wad, "LINEDEFS", position, MapManager.TEMP_MAP_HEADER, maplumps);
			if(insertpos == -1) insertpos = position + 1;
			if(insertpos > wad.Lumps.Count) insertpos = wad.Lumps.Count;

			// Create the lump from memory
			Lump lump = wad.Insert("LINEDEFS", insertpos, (int)mem.Length);
			lump.Stream.Seek(0, SeekOrigin.Begin);
			mem.WriteTo(lump.Stream);
		}

		// This writes the SIDEDEFS to WAD file
		private void WriteSidedefs(MapSet map, int position, Dictionary<string, MapLumpInfo> maplumps, IDictionary<Sector, int> sectorids)
		{
			// Create memory to write to
			MemoryStream mem = new MemoryStream();
			BinaryWriter writer = new BinaryWriter(mem, WAD.ENCODING);

			// Go for all sidedefs
			foreach(Sidedef sd in map.Sidedefs)
			{
				// Write properties to stream
				writer.Write((Int16)sd.OffsetX);
				writer.Write((Int16)sd.OffsetY);
				writer.Write(Lump.MakeFixedName(sd.HighTexture, WAD.ENCODING));
				writer.Write(Lump.MakeFixedName(sd.LowTexture, WAD.ENCODING));
				writer.Write(Lump.MakeFixedName(sd.MiddleTexture, WAD.ENCODING));
				writer.Write((UInt16)sectorids[sd.Sector]);
			}

			// Find insert position and remove old lump
			int insertpos = MapManager.RemoveSpecificLump(wad, "SIDEDEFS", position, MapManager.TEMP_MAP_HEADER, maplumps);
			if(insertpos == -1) insertpos = position + 1;
			if(insertpos > wad.Lumps.Count) insertpos = wad.Lumps.Count;

			// Create the lump from memory
			Lump lump = wad.Insert("SIDEDEFS", insertpos, (int)mem.Length);
			lump.Stream.Seek(0, SeekOrigin.Begin);
			mem.WriteTo(lump.Stream);
		}

		// This writes the SECTORS to WAD file
		private void WriteSectors(MapSet map, int position, Dictionary<string, MapLumpInfo> maplumps)
		{
			// Create memory to write to
			MemoryStream mem = new MemoryStream();
			BinaryWriter writer = new BinaryWriter(mem, WAD.ENCODING);

			// Go for all sectors
			foreach(Sector s in map.Sectors)
			{
				// Write properties to stream
				writer.Write((Int16)s.FloorHeight);
				writer.Write((Int16)s.CeilHeight);
				writer.Write(Lump.MakeFixedName(s.FloorTexture, WAD.ENCODING));
				writer.Write(Lump.MakeFixedName(s.CeilTexture, WAD.ENCODING));
				writer.Write((Int16)s.Brightness);
				writer.Write((UInt16)s.Effect);
				writer.Write((UInt16)s.Tag);
			}

			// Find insert position and remove old lump
			int insertpos = MapManager.RemoveSpecificLump(wad, "SECTORS", position, MapManager.TEMP_MAP_HEADER, maplumps);
			if(insertpos == -1) insertpos = position + 1;
			if(insertpos > wad.Lumps.Count) insertpos = wad.Lumps.Count;

			// Create the lump from memory
			Lump lump = wad.Insert("SECTORS", insertpos, (int)mem.Length);
			lump.Stream.Seek(0, SeekOrigin.Begin);
			mem.WriteTo(lump.Stream);
		}
		
		#endregion
	}
}
