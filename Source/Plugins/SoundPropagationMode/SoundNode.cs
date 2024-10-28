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

using System.Collections.Generic;
using System.Drawing;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Rendering;

namespace CodeImp.DoomBuilder.SoundPropagationMode
{
	internal class SoundNode
	{
		public Vector2D Position { get; set; }
		public List<SoundNode> Neighbors { get; set; }
		public SoundNode From { get; set; }
		public double G { get; set; }
		public double H { get; }
		public double F { get; set; } // It's G + H, but computing it on the fly is too expensive
		public bool IsBlocking { get; }
		public bool IsSkip { get; set; }

		public SoundNode(Vector2D position)
		{
			Position = position;
			G = double.MaxValue;
			H = double.MaxValue;
			IsBlocking = false;
			IsSkip = false;
			Neighbors = new List<SoundNode>();
		}

		public SoundNode(Vector2D position, SoundNode destination): this(position)
		{
			H = Vector2D.Distance(Position, destination.Position);
		}

		public SoundNode(Linedef linedef, SoundNode destination) : this(linedef.Line.GetCoordinatesAt(0.5), destination)
		{
			IsBlocking = linedef.IsFlagSet(SoundPropagationMode.BlockSoundFlag);
		}

		/// <summary>
		/// Recomputes the values for the sound node's neighbors
		/// </summary>
		/// <param name="openset">The open set, the add the neighbor to if necessary</param>
		/// <param name="start">The start sound node</param>
		public void ProcessNeighbors(List<SoundNode> openset, SoundNode start)
		{
			bool blockinginpath = HasBlockingInPath(start);

			foreach (SoundNode neighbor in Neighbors)
			{
				// Skip neighbors that are blocking if there's already a blocking sound node in the path
				// Also skip neighbors that are set to be skipped
				if ((neighbor.IsBlocking && blockinginpath) || neighbor.IsSkip)
					continue;

				double newg = G + Vector2D.Distance(Position, neighbor.Position);

				// Compute new values if the path is better
				if (newg < neighbor.G)
				{
					neighbor.From = this;
					neighbor.G = newg;
					neighbor.F = neighbor.G + neighbor.H;

					if (!openset.Contains(neighbor))
						openset.Add(neighbor);
				}
			}
		}

		/// <summary>
		/// Checks if the path from this sound node to the start sound node has a blocking sound node
		/// </summary>
		/// <param name="start">The start sound node</param>
		/// <returns>true if there is a blocking sound node in the path, false if there isn't</returns>
		private bool HasBlockingInPath(SoundNode start)
		{
			SoundNode current = this;
			while(current != start)
			{
				if (current.IsBlocking)
					return true;
				current = current.From;
			}

			return false;
		}

		/// <summary>
		/// Resets the sound node's G and F values, and the sound node that leads here
		/// </summary>
		public void Reset()
		{
			From = null;
			G = double.MaxValue;
			F = double.MaxValue;
		}

		/// <summary>
		/// Renders the path from this node to the beginning. Traces the path from this sound node back to the start sound node
		/// </summary>
		/// <param name="renderer">The Renderer2D to render with</param>
		internal void RenderPath(IRenderer2D renderer)
		{
			SoundNode current = this;

			// If the current node is null we have reached the beginning
			while(current != null)
			{
				// Do not render the start and end sound nodes
				if (current != this && current.From != null)
				{
					RectangleF rectangle = new RectangleF((float)(current.Position.x - 4 / renderer.Scale), (float)(current.Position.y - 4 / renderer.Scale), 8 / renderer.Scale, 8 / renderer.Scale);
					renderer.RenderRectangleFilled(rectangle, PixelColor.FromColor(Color.Red), true);
				}

				if(current.From != null)
					renderer.RenderLine(current.Position, current.From.Position, 1.0f, PixelColor.FromColor(Color.Red), true);

				// One step back
				current = current.From;
			}
		}
	}
}
