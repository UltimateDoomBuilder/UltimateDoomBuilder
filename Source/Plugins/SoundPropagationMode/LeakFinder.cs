#region ================== Copyright (c) 2023 Boris Iwanski

/*
 * This program is free software: you can redistribute it and/or modify
 *
 * it under the terms of the GNU General Public License as published by
 * 
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 *    but WITHOUT ANY WARRANTY; without even the implied warranty of
 * 
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
 * 
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.If not, see<http://www.gnu.org/licenses/>.
 */

#endregion

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Map;

namespace CodeImp.DoomBuilder.SoundPropagationMode
{
	internal class LeakFinder
	{
		public SoundNode Start { get; }
		public SoundNode End { get; }
		public List<SoundNode> Nodes { get; }
		public HashSet<Sector> Sectors { get; }
		public bool Finished { get; internal set; }

		private ConcurrentDictionary<Linedef, SoundNode> linedefs2nodes;
		private int numblockingnodes;

		public LeakFinder(Sector source, Vector2D sourceposition, Sector destination, Vector2D destinationposition, HashSet<Sector> sectors)
		{
			if (!sectors.Contains(source) || !sectors.Contains(destination))
				throw new ArgumentException("Sound propagation domain does not contain both the start and end sectors");

			End = new SoundNode(destinationposition);
			Start = new SoundNode(sourceposition, End) { G = 0 };
			Sectors = sectors;

			Finished = false;

			Nodes = new List<SoundNode>() { Start, End };

			linedefs2nodes = new ConcurrentDictionary<Linedef, SoundNode>();

			GenerateNodes(sectors);

			PopulateStartEndNeighbors(source, Start);
			PopulateStartEndNeighbors(destination, End);
		}

		/// <summary>
		/// Checks if the linedef is valid for passing sound.
		/// </summary>
		/// <param name="linedef">The linedef to check</param>
		/// <returns>true if sound can travel through the linedef, false if not</returns>
		private bool CheckLinedefValidity(Linedef linedef)
		{
			if (linedef.Back == null)
				return false;

			if (linedef.Front.Sector == linedef.Back.Sector)
				return false;

			if (SoundPropagationDomain.IsSoundBlockedByHeight(linedef))
				return false;

			return Sectors.Contains(linedef.Front.Sector) && Sectors.Contains(linedef.Back.Sector);
		}

		/// <summary>
		/// Generates all nodes for the A* search algorithm.
		/// </summary>
		/// <param name="sectors">sectors to generate the nodes from</param>
		private void GenerateNodes(HashSet<Sector> sectors)
		{
			// Create sound nodes for all valid linedefs in all given sectors
			foreach(Sector s in sectors)
			{
				IEnumerable<Sidedef> sidedefs = s.Sidedefs.Where(sd => CheckLinedefValidity(sd.Line));

				foreach(Sidedef sd in sidedefs)
				{
					if(!linedefs2nodes.ContainsKey(sd.Line))
					{
						linedefs2nodes[sd.Line] = new SoundNode(sd.Line, End);
						Nodes.Add(linedefs2nodes[sd.Line]);
					}
				}
			}

			// We need the number of blocking nodes for safety checking
			numblockingnodes = linedefs2nodes.Values.Count(n => n.IsBlocking);

			// Set the neighbors for each node. The amount of interconnections can be very high in complex maps
			// (for example there are nearly 3.9 million in Sunder map 20), so do it in parallel for speed
			Parallel.ForEach(linedefs2nodes.Keys, ld =>
			{
				foreach (Sidedef sd in ld.Front.Sector.Sidedefs)
				{
					if (sd.Line != ld && CheckLinedefValidity(sd.Line))
						linedefs2nodes[ld].Neighbors.Add(linedefs2nodes[sd.Line]);
				}

				foreach (Sidedef sd in ld.Back.Sector.Sidedefs)
				{
					if (sd.Line != ld && CheckLinedefValidity(sd.Line))
						linedefs2nodes[ld].Neighbors.Add(linedefs2nodes[sd.Line]);
				}
			});

#if DEBUG
			int bla = linedefs2nodes.Values.Sum(n => n.Neighbors.Count);
			Console.WriteLine($"There are {linedefs2nodes.Keys.Count} nodes with {bla} interconnections.");
#endif
		}

		/// <summary>
		/// Populates a sound node's neightbors to the linedefs of a sector. This is required for the start and end sound nodes.
		/// </summary>
		/// <param name="sector">The sector which linedef's sound nodes are used</param>
		/// <param name="node">The sound node to add the neighbors to</param>
		private void PopulateStartEndNeighbors(Sector sector, SoundNode node)
		{
			foreach(Sidedef sd in sector.Sidedefs)
			{
				if(CheckLinedefValidity(sd.Line) && linedefs2nodes.ContainsKey(sd.Line))
				{
					node.Neighbors.Add(linedefs2nodes[sd.Line]);
					linedefs2nodes[sd.Line].Neighbors.Add(node);
				}
			}
		}

		/// <summary>
		/// Finds a sound leak between the start and end sound nodes.
		/// </summary>
		/// <returns>true if a leak was found, false if no leak was found</returns>
		public bool FindLeak()
		{
			Finished = false;

			// Basic A* search. The twist is that sound blocking lines: we can only pass through one of them,
			// and A* doesn't backtrack, so it can fail to find a path even if there is a possible one. If that
			// happens we set the sound blocking node we traveled through to be ignored, and start again. We repeat
			// that until a path was found, or all blocking nodes are set to be ignored (which shouldn't happen)
			while (true)
			{
				List<SoundNode> openset = new List<SoundNode>() { Start };

				while (openset.Count > 0)
				{
					// Find the node with the lowest F score. Doing it that way seems to be fastest
					SoundNode current = openset[0];
					for (int i = 1; i < openset.Count; i++)
					{
						if (openset[i].F < current.F)
							current = openset[i];
					}

					// We're done if the node with the lowest F score is the end node
					if (current == End)
					{
						Finished = true;
						return true;
					}

					// Remove the current node from the open set
					openset.Remove(current);

					// Compute new values for the current node's neighbors
					current.ProcessNeighbors(openset, Start);
				}

				// If we got here we didn't find a path. So we have to start over

				int currentnumblockingnodes = 0;

				// Reset all nodes
				foreach(SoundNode sn in Nodes)
				{
					// Set the sound nodes that block sound and were visited (the G value was set to something) to be skipped.
					if(sn.IsBlocking && sn.G != double.MaxValue)
					{
						sn.IsSkip = true;
						currentnumblockingnodes++;
					}

					// We need to reset the sound node's G and F values
					sn.Reset();
				}

				// All blocking sound nodes are being skipped, so no path is possible
				if (currentnumblockingnodes == numblockingnodes)
				{
					Finished = true;

					return false;
				}

				// Don't forget the reset the start node to its special values
				Start.G = 0.0;
				Start.F = Start.H;
			}
		}
	}
}
