
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
using System.Collections;
using CodeImp.DoomBuilder.Types;
using CodeImp.DoomBuilder.Config;
using System.Linq;

#endregion

namespace CodeImp.DoomBuilder.IO
{
	internal class UniversalMapSetIO : MapSetIO
	{
		#region ================== Constructor / Disposer

		// Constructor
		public UniversalMapSetIO(WAD wad, MapManager manager) : base(wad, manager)
		{
			if((manager != null) && (manager.Config != null))
			{
				// Build the dictionary of UDMF fields that are managed by the UI and should not be shown in the custom UDMF field dialog
				foreach ((MapElementType type, List<UniversalFieldInfo> data) in new[] {
					(MapElementType.LINEDEF, General.Map.Config.LinedefFields),
					(MapElementType.SECTOR, General.Map.Config.SectorFields),
					(MapElementType.SIDEDEF, General.Map.Config.SidedefFields),
					(MapElementType.THING, General.Map.Config.ThingFields),
					(MapElementType.VERTEX, General.Map.Config.VertexFields)
				})
				{
					uifields[type] = new Dictionary<string, UniversalType>(StringComparer.Ordinal);

					foreach (UniversalFieldInfo ufi in data.Where(o => o.Managed))
						uifields[type].Add(ufi.Name, (UniversalType)ufi.Type);
				}
			}
		}

		#endregion

		#region ================== Properties

		public override int MaxSidedefs { get { return int.MaxValue; } }
		public override int MaxVertices { get { return int.MaxValue; } }
		public override int MaxLinedefs { get { return int.MaxValue; } }
		public override int MaxSectors { get { return int.MaxValue; } }
		public override int MaxThings { get { return int.MaxValue; } }
		public override int MinTextureOffset { get { return int.MinValue; } }
		public override int MaxTextureOffset { get { return int.MaxValue; } }
		public override int VertexDecimals { get { return 3; } }
		public override string DecimalsFormat { get { return "0.000"; } }
		public override bool HasLinedefTag { get { return true; } }
		public override bool HasThingTag { get { return true; } }
		public override bool HasThingAction { get { return true; } }
		public override bool HasCustomFields { get { return true; } }
		public override bool HasThingHeight { get { return true; } }
		public override bool HasActionArgs { get { return true; } }
		public override bool HasMixedActivations { get { return true; } }
		public override bool HasPresetActivations { get { return false; } }
		public override bool HasBuiltInActivations { get { return false; } }
		public override bool HasNumericLinedefFlags { get { return false; } }
		public override bool HasNumericThingFlags { get { return false; } }
		public override bool HasNumericLinedefActivations { get { return false; } }
		public override int MaxTag { get { return int.MaxValue; } }
		public override int MinTag { get { return int.MinValue; } }
		public override int MaxAction { get { return int.MaxValue; } }
		public override int MinAction { get { return int.MinValue; } }
		public override int MaxArgument { get { return int.MaxValue; } }
		public override int MinArgument { get { return int.MinValue; } }
		public override int MaxEffect { get { return int.MaxValue; } }
		public override int MinEffect { get { return int.MinValue; } }
		public override int MaxBrightness { get { return int.MaxValue; } }
		public override int MinBrightness { get { return int.MinValue; } }
		public override int MaxThingType { get { return short.MaxValue; } } //mxd. Editor numbers must be in [1 .. 32767] range
		public override int MinThingType { get { return 1; } } //mxd
		public override float MaxCoordinate { get { return short.MaxValue; } } //mxd. UDMF maps are still bounded to -32768 .. 32767 range
		public override float MinCoordinate { get { return short.MinValue; } } //mxd
		public override int MaxThingAngle { get { return int.MaxValue; } }
		public override int MinThingAngle { get { return int.MinValue; } }
		public override Dictionary<MapElementType, Dictionary<string, UniversalType>> UIFields { get { return uifields; } } //mxd
		
		#endregion

		#region ================== Reading
		
		// This reads a map from the file and returns a MapSet
		public override MapSet Read(MapSet map, string mapname)
		{
			UniversalStreamReader udmfreader = new UniversalStreamReader(uifields); //mxd
			
			// Find the index where first map lump begins
			int firstindex = wad.FindLumpIndex(mapname) + 1;
			
			// Get the TEXTMAP lump from wad file
			Lump lump = wad.FindLump("TEXTMAP", firstindex);
			if(lump == null) throw new Exception("Could not find required lump TEXTMAP!");

			// Read the UDMF data
			lump.Stream.Seek(0, SeekOrigin.Begin);
			udmfreader.SetKnownCustomTypes = false;
			udmfreader.Read(map, lump.Stream);
			
			// Return result
			return map;
		}
		
		#endregion

		#region ================== Writing

		// This writes a MapSet to the file
		public override void Write(MapSet map, string mapname, int position)
		{
			UniversalStreamWriter udmfwriter = new UniversalStreamWriter();
			
			// Write map to memory stream
			MemoryStream memstream = new MemoryStream();
			memstream.Seek(0, SeekOrigin.Begin);
			udmfwriter.RememberCustomTypes = true;
			udmfwriter.Write(map, memstream, manager.Config.EngineName);

			// Find insert position and remove old lump
			int insertpos = MapManager.RemoveSpecificLump(wad, "TEXTMAP", position, MapManager.TEMP_MAP_HEADER, manager.Config.MapLumps);
			if(insertpos == -1) insertpos = position + 1;
			if(insertpos > wad.Lumps.Count) insertpos = wad.Lumps.Count;

			// Create the lump from memory
			Lump lump = wad.Insert("TEXTMAP", insertpos, (int)memstream.Length);
			lump.Stream.Seek(0, SeekOrigin.Begin);
			memstream.WriteTo(lump.Stream);

			// Done
			memstream.Dispose();
		}
		
		#endregion
	}
}

