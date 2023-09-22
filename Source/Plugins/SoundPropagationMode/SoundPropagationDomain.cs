#region ================== Namespaces

using System.Collections.Generic;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Rendering;

#endregion

namespace CodeImp.DoomBuilder.SoundPropagationMode
{
	public class SoundPropagationDomain
	{
		#region ================== Variables

		private HashSet<Sector> sectors;
		private HashSet<Sector> adjacentsectors;
		private FlatVertex[] level1geometry;
		private FlatVertex[] level2geometry;

		#endregion

		#region ================== Properties

		public HashSet<Sector> Sectors { get { return sectors; } }
		public HashSet<Sector> AdjacentSectors { get { return adjacentsectors; } }
		public FlatVertex[] Level1Geometry { get { return level1geometry; } }
		public FlatVertex[] Level2Geometry { get { return level2geometry; } }
		public int Color { get; set; } //mxd

		#endregion

		#region ================== Constructor

		public SoundPropagationDomain(Sector sector)
		{
			sectors = new HashSet<Sector>();
			adjacentsectors = new HashSet<Sector>();

			CreateSoundPropagationDomain(sector);
		}

		#endregion

		#region ================== Methods

		private void CreateSoundPropagationDomain(Sector sourcesector)
		{
			List<Sector> sectorstocheck = new List<Sector> { sourcesector };
			HashSet<Linedef> blockinglines = new HashSet<Linedef>(); //mxd

			while(sectorstocheck.Count > 0)
			{
				// Make sure to first check all sectors that are not behind a sound blocking line
				Sector sector = sectorstocheck[0];

				foreach(Sidedef sd in sector.Sidedefs)
				{
					bool blocksound = sd.Line.IsFlagSet(SoundPropagationMode.BlockSoundFlag);
					if(blocksound && sd.Other != null) blockinglines.Add(sd.Line);

					// If the line is one sided, the sound can travel nowhere, so try the next one
					if(sd.Other == null || blocksound) continue;
	
					// Get the sector on the other side of the line we're checking right now
					Sector oppositesector = sd.Other.Sector;

					bool blockheight = IsSoundBlockedByHeight(sd.Line);

					// Try next line if sound will not pass through the current one. The last check makes
					// sure that the next line is tried if the current line is blocking sound, and the current
					// sector is already behind a sound blocking line
					if(oppositesector == null || blockheight) continue;

					// If the opposite sector was not regarded at all yet...
					if(!sectors.Contains(oppositesector) && !sectorstocheck.Contains(oppositesector))
					{
						sectorstocheck.Add(oppositesector);
					}
				}

				sectorstocheck.Remove(sector);
				sectors.Add(sector);
			}

			foreach(Linedef ld in blockinglines)
			{
				// Lines that don't have a back side, or where the sound is blocked due to
				// the sector heights on each side can be skipped
				if(IsSoundBlockedByHeight(ld)) continue;
				if(!sectors.Contains(ld.Front.Sector)) adjacentsectors.Add(ld.Front.Sector);
				if(!sectors.Contains(ld.Back.Sector)) adjacentsectors.Add(ld.Back.Sector);
			}

			List<FlatVertex> vertices = new List<FlatVertex>();

			foreach(Sector s in sectors)
			{
				vertices.AddRange(s.FlatVertices);
			}

			level1geometry = vertices.ToArray();
			level2geometry = vertices.ToArray();

			for(int i = 0; i < level1geometry.Length; i++)
			{
				level1geometry[i].c = BuilderPlug.Me.Level1Color.WithAlpha(128).ToInt();
				level2geometry[i].c = BuilderPlug.Me.Level2Color.WithAlpha(128).ToInt();
			}
		}

		public static bool IsSoundBlockedByHeight(Linedef ld)
		{
			if(ld.Back == null || ld.Front == null) return false;

			Sector s1 = ld.Front.Sector;
			Sector s2 = ld.Back.Sector;

			// Check if the sound will be blocked because of sector floor and ceiling heights
			// (like closed doors, raised lifts etc.)
			return (s1.CeilHeight <= s2.FloorHeight || s1.FloorHeight >= s2.CeilHeight ||
			        s2.CeilHeight <= s2.FloorHeight || s1.CeilHeight <= s1.FloorHeight);
		}

		#endregion
	}
}
