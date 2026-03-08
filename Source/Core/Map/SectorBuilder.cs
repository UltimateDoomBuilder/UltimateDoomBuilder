#region ================== Namespaces

using System.Collections.Generic;
using CodeImp.DoomBuilder.Geometry;

#endregion

namespace CodeImp.DoomBuilder.Map
{
	//mxd. Shameless Slade 3 SectorBuilder::SectorBuilder ripoff...
	//TODO: There are lots of overlaps with already existing code.
	//TODO: Replace with existing implementations if results are the same & existing code performs faster
	internal sealed class SectorBuilder
	{
		#region ================== Variables

		private List<LinedefSide> sector_edges;
		private HashSet<Vertex> vertex_valid;

		// Current outline
		private List<LinedefSide> o_edges;

		#endregion

		#region ================== Properties

		public List<LinedefSide> SectorEdges { get { return sector_edges; } }

		#endregion

		#region ================== Constructor

		public SectorBuilder()
		{
			sector_edges = new List<LinedefSide>();
			vertex_valid = new HashSet<Vertex>();
			o_edges = new List<LinedefSide>();
		}

		#endregion

		#region ================== Methods

		///<summary>Traces all edges to build a closed sector starting from [line]</summary>
		internal bool TraceSector(Linedef line, bool front)
		{
            // [ZZ] There indeed already was an implementation of this.
            //      To do: make sure this works correctly. I don't understand what the old SectorBuilder was doing exactly, but from the looks of it FindPotentialSectorAt should be ok here.
            sector_edges.Clear();
            List<LinedefSide> edges = Tools.FindPotentialSectorAt(line, front);
            if (edges == null)
                return false;
            sector_edges.AddRange(edges);
            return sector_edges.Count > 0;
        }

		/// <summary>Checks if the traced sector is valid (ie. all edges are currently referencing the same (existing) sector)</summary>
		public bool IsValidSector()
		{
			if(sector_edges.Count == 0) return false;

			// Get first edge's sector
			Sector sector = (sector_edges[0].Front ?
				(sector_edges[0].Line.Front != null ? sector_edges[0].Line.Front.Sector : null) :
				(sector_edges[0].Line.Back != null ? sector_edges[0].Line.Back.Sector : null));

			// Sector is invalid if any edge has no current sector
			if(sector == null) return false; 

			// Go through subsequent edges
			for(int a = 1; a < sector_edges.Count; a++)
			{
				// Get edge sector
				Sector ssector = (sector_edges[a].Front ? 
					(sector_edges[a].Line.Front != null ? sector_edges[a].Line.Front.Sector : null) :
					(sector_edges[a].Line.Back != null ? sector_edges[a].Line.Back.Sector : null));

				// Check if different
				if(sector != ssector) return false;
			}

			// Return true if the entire sector was traced
			return (sector.Sidedefs.Count == sector_edges.Count);
		}

		/// <summary>Finds any existing sector that is already part of the traced new sector</summary>
		internal Sector FindExistingSector(HashSet<Sidedef> sides_ignore)
		{
			// Go through new sector edges
			Sector sector = null;
			Sector sector_priority = null;
			foreach(LinedefSide edge in sector_edges)
			{
				// Check if the edge's corresponding MapSide has a front sector
				if(edge.Front && edge.Line.Front != null && edge.Line.Front.Sector != null)
				{
					if(sides_ignore.Contains(edge.Line.Front))
						sector = edge.Line.Front.Sector;
					else
						sector_priority = edge.Line.Front.Sector;
				}

				// Check if the edge's corresponding MapSide has a back sector
				if(!edge.Front && edge.Line.Back != null && edge.Line.Back.Sector != null)
				{
					if(sides_ignore.Contains(edge.Line.Back))
						sector = edge.Line.Back.Sector;
					else
						sector_priority = edge.Line.Back.Sector;
				}
			}

			return (sector_priority ?? sector);
		}

		/// <summary>Sets all traced edges to [sector], or creates a new sector using properties
		/// from [sector_copy] if none given</summary>
		internal void CreateSector(Sector sector, Sector sector_copy)
		{
			// Create the sector if needed
			if(sector == null)
			{
				sector = General.Map.Map.CreateSector();
				if(sector == null) return;
				sector.Marked = true; //mxd

				// Find potential sector to copy if none specified
				if(sector_copy == null) sector_copy = FindCopySector();
				if(sector_copy != null) sector_copy.CopyPropertiesTo(sector);
			}

			//DebugConsole.WriteLine(" ");
			//DebugConsole.WriteLine("Creating sector " + sector.Index + " from " + sector_edges.Count + " lines");
			//DebugConsole.WriteLine("*************************************************************");
			//DebugConsole.WriteLine(" ");

			// Set sides to new sector
			foreach(LinedefSide edge in sector_edges)
			{
				Sidedef target = (edge.Front ? edge.Line.Front : edge.Line.Back);
				if(target != null)
				{
					if(target.Sector != sector)
					{
						bool targetwas2s = (target.Other != null);
						target.SetSector(sector); //mxd. Reattach side

						//mxd. Mark for texture adjustments if sidedness was changed.
						//mxd. Also keep existing mark if the side was already marked.
						target.Marked |= ((targetwas2s && target.Other == null) || (!targetwas2s && target.Other != null));
					}
				}
				else
				{
					target = General.Map.Map.CreateSidedef(edge.Line, edge.Front, sector); //mxd. Create new side
					target.Marked = true; //mxd. Mark it for texture adjustments
					if(target.Other != null)
					{
						//mxd. Better than nothing
						target.Other.CopyPropertiesTo(target);

						//mxd. Other was singlesided. We'll need to adjust it's textures as well
						target.Other.Marked = true; 
					}
				}
			}
		}

		/// <summary>Finds an appropriate existing sector to copy properties from, for the new sector being built</summary>
		private Sector FindCopySector()
		{
			// Go through new sector edges
			Sector sector_copy = null;
			foreach(LinedefSide edge in sector_edges)
			{
				// Check if the edge's corresponding MapSide has a front sector
				if(edge.Line.Front != null && edge.Line.Front.Sector != null)
				{
					// Set sector to copy
					sector_copy = edge.Line.Front.Sector;

					// If the edge is a front edge, use this sector and ignore all else
					if(edge.Front) break;
				}

				// Check if the edge's corresponding MapSide has a back sector
				if(edge.Line.Back != null && edge.Line.Back.Sector != null)
				{
					// Set sector to copy
					sector_copy = edge.Line.Back.Sector;

					// If the edge is a back edge, use this sector and ignore all else
					if(!edge.Front) break;
				}
			}

			return sector_copy;
		}

		#endregion
	}
}
