#region ================== Copyright (c) 2014 Boris Iwanski

/*
 * Copyright (c) 2014 Boris Iwanski
 * This program is released under GNU General Public License
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 */

#endregion

using System;
using System.Windows.Forms;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Diagnostics;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Editing;
using CodeImp.DoomBuilder.Map;

namespace CodeImp.DoomBuilder.ThreeDFloorMode
{
	[Serializable]
	public class NoSpaceInCSAException : Exception
	{
		public NoSpaceInCSAException()
		{ }

		public NoSpaceInCSAException(string message) : base(message)
		{ }

		public NoSpaceInCSAException(string message, Exception innerException) : base(message, innerException)
		{ }
	}

	public class ControlSectorArea
	{

		#region ================== Enums

		public enum Highlight
		{
			None,
			OuterLeft,
			OuterRight,
			OuterTop,
			OuterBottom,
			OuterTopLeft,
			OuterTopRight,
			OuterBottomLeft,
			OuterBottomRight,
			Body
		};

		#endregion

		#region ================== Variables

		private RectangleF outerborder;
		private PixelColor bordercolor = new PixelColor(255, 0, 192, 0);
		private PixelColor fillcolor = new PixelColor(128, 0, 128, 0);
		private PixelColor borderhighlightcolor = new PixelColor(255, 0, 192, 0);
		private PixelColor fillhighlightcolor = new PixelColor(128, 0, 192, 0);
		private Dictionary<Highlight, Line2D> lines;
		private Dictionary<Highlight, Vector2D> points;
		private float gridsize;
		private float gridsizeinv;
		private float sectorsize;

		private double outerleft;
		private double outerright;
		private double outertop;
		private double outerbottom;

		private bool usecustomtagrange;
		private int firsttag;
		private int lasttag;

		#endregion

		#region ================== Properties

		public double GridSize { get { return gridsize; } }
		public double SectorSize { get { return sectorsize; } }

		public RectangleF OuterBorder { get { return outerborder; } }

		public double OuterLeft
		{
			get { return outerleft; }
			set { outerleft = value; UpdateLinesAndPoints();	}
		}

		public double OuterRight
		{
			get { return outerright; }
			set { outerright = value; UpdateLinesAndPoints(); }
		}

		public double OuterTop
		{
			get { return outertop; }
			set { outertop = value; UpdateLinesAndPoints(); }
		}

		public double OuterBottom
		{
			get { return outerbottom; }
			set { outerbottom = value; UpdateLinesAndPoints(); }
		}

		public bool UseCustomTagRnage { get { return usecustomtagrange; } set { usecustomtagrange = value; } }
		public int FirstTag { get { return firsttag; } set { firsttag = value; } }
		public int LastTag { get { return lasttag; } set { lasttag = value; } }

		#endregion

		#region ================== Constructor / Disposer

		public ControlSectorArea(float outerleft, float outerright, float outertop, float outerbottom, float gridsize, float sectorsize)
		{
			this.outerleft = outerleft;
			this.outerright = outerright;
			this.outertop = outertop;
			this.outerbottom = outerbottom;

			lines = new Dictionary<Highlight, Line2D>();
			points = new Dictionary<Highlight, Vector2D>();

			this.gridsize = gridsize;
			gridsizeinv = 1.0f / gridsize;

			this.sectorsize = sectorsize;

			UpdateLinesAndPoints();
		}

		#endregion

		#region ================== Methods

		public void UpdateLinesAndPoints()
		{
			lines[Highlight.OuterLeft] = new Line2D(outerleft, outertop, outerleft, outerbottom);
			lines[Highlight.OuterRight] = new Line2D(outerright, outertop, outerright, outerbottom);
			lines[Highlight.OuterTop] = new Line2D(outerleft, outertop, outerright, outertop);
			lines[Highlight.OuterBottom] = new Line2D(outerleft, outerbottom, outerright, outerbottom);

			points[Highlight.OuterTopLeft] = new Vector2D(outerleft, outertop);
			points[Highlight.OuterTopRight] = new Vector2D(outerright, outertop);
			points[Highlight.OuterBottomLeft] = new Vector2D(outerleft, outerbottom);
			points[Highlight.OuterBottomRight] = new Vector2D(outerright, outerbottom);

			outerborder = new RectangleF((float)outerleft, (float)outertop, (float)(outerright - outerleft), (float)(outerbottom - outertop));
		}

		public void Draw(IRenderer2D renderer, Highlight highlight)
		{
			PixelColor fcolor = highlight == Highlight.Body ? fillhighlightcolor : fillcolor;

			renderer.RenderRectangleFilled(
				new RectangleF((float)outerleft, (float)outertop, (float)(outerright - outerleft), (float)(outerbottom - outertop)),
				fcolor,
				true
			);

			// Draw the borders
			renderer.RenderRectangle(outerborder, 1.0f, bordercolor, true);

			// Highlight a border if necessary
			if (highlight >= Highlight.OuterLeft && highlight <= Highlight.OuterBottom)
				renderer.RenderLine(lines[highlight].v1, lines[highlight].v2, 1.0f, borderhighlightcolor, true);
			else
			{
				// Highlight the corners
				switch (highlight)
				{
					// Outer corners
					case Highlight.OuterTopLeft:
						renderer.RenderLine(lines[Highlight.OuterTop].v1, lines[Highlight.OuterTop].v2, 1.0f, borderhighlightcolor, true);
						renderer.RenderLine(lines[Highlight.OuterLeft].v1, lines[Highlight.OuterLeft].v2, 1.0f, borderhighlightcolor, true);
						break;
					case Highlight.OuterTopRight:
						renderer.RenderLine(lines[Highlight.OuterTop].v1, lines[Highlight.OuterTop].v2, 1.0f, borderhighlightcolor, true);
						renderer.RenderLine(lines[Highlight.OuterRight].v1, lines[Highlight.OuterRight].v2, 1.0f, borderhighlightcolor, true);
						break;
					case Highlight.OuterBottomLeft:
						renderer.RenderLine(lines[Highlight.OuterBottom].v1, lines[Highlight.OuterBottom].v2, 1.0f, borderhighlightcolor, true);
						renderer.RenderLine(lines[Highlight.OuterLeft].v1, lines[Highlight.OuterLeft].v2, 1.0f, borderhighlightcolor, true);
						break;
					case Highlight.OuterBottomRight:
						renderer.RenderLine(lines[Highlight.OuterBottom].v1, lines[Highlight.OuterBottom].v2, 1.0f, borderhighlightcolor, true);
						renderer.RenderLine(lines[Highlight.OuterRight].v1, lines[Highlight.OuterRight].v2, 1.0f, borderhighlightcolor, true);
						break;
				}
			}
		}

		public Highlight CheckHighlight(Vector2D pos, double scale)
		{
			double distance = double.MaxValue;
			double d;
			Highlight highlight = Highlight.None;

			// Find a line to highlight
			foreach (Highlight h in (Highlight[])Enum.GetValues(typeof(Highlight)))
			{
				if (h >= Highlight.OuterLeft && h <= Highlight.OuterBottom)
				{
					d = Line2D.GetDistanceToLine(lines[h].v1, lines[h].v2, pos, true);

					if (d <= BuilderModes.BuilderPlug.Me.HighlightRange / scale && d < distance)
					{
						distance = d;
						highlight = h;
					}
				}
			}

			distance = double.MaxValue;

			// Find a corner to highlight
			foreach (Highlight h in (Highlight[])Enum.GetValues(typeof(Highlight)))
			{
				if (h >= Highlight.OuterTopLeft && h <= Highlight.OuterBottomRight)
				{
					d = Vector2D.Distance(pos, points[h]);

					if (d <= BuilderModes.BuilderPlug.Me.HighlightRange / scale && d < distance)
					{
						distance = d;
						highlight = h;
					}
				}
			}

			if (highlight != Highlight.None)
				return highlight;

			if (OuterLeft < pos.x && OuterRight > pos.x && OuterTop > pos.y && OuterBottom < pos.y)
				return Highlight.Body;

			return Highlight.None;
		}

		public void SnapToGrid(Highlight highlight, Vector2D pos, Vector2D lastpos)
		{
			Vector2D newpos = GridSetup.SnappedToGrid(pos, gridsize, gridsizeinv);

			switch (highlight)
			{
				case Highlight.Body:
					Vector2D diff = GridSetup.SnappedToGrid(pos, gridsize, gridsizeinv) - GridSetup.SnappedToGrid(lastpos, gridsize, gridsizeinv);
					outerleft += diff.x;
					outerright += diff.x;
					outertop += diff.y;
					outerbottom += diff.y;
					break;

				// Outer border
				case Highlight.OuterLeft:
					if (newpos.x < outerright) outerleft = newpos.x;
					break;
				case Highlight.OuterRight:
					if(newpos.x > outerleft) outerright = newpos.x;
					break;
				case Highlight.OuterTop:
					if (newpos.y > outerbottom) outertop = newpos.y;
					break;
				case Highlight.OuterBottom:
					if (newpos.y < outertop) outerbottom = newpos.y;
					break;

				// Outer corners
				case Highlight.OuterTopLeft:
					if (newpos.x < outerright) outerleft = newpos.x;
					if (newpos.y > outerbottom) outertop = newpos.y;
					break;
				case Highlight.OuterTopRight:
					if (newpos.x > outerleft) outerright = newpos.x;
					if (newpos.y > outerbottom) outertop = newpos.y;
					break;
				case Highlight.OuterBottomLeft:
					if (newpos.x < outerright) outerleft = newpos.x;
					if (newpos.y < outertop) outerbottom = newpos.y;
					break;
				case Highlight.OuterBottomRight:
					if (newpos.x > outerleft) outerright = newpos.x;
					if (newpos.y < outertop) outerbottom = newpos.y;
					break;
			}

			UpdateLinesAndPoints();
		}

		public List<Vector2D> GetRelocatePositions(int numsectors)
		{
			List<Vector2D> positions = new List<Vector2D>();
			BlockMap<BlockEntry> blockmap = CreateBlockmap(true);
			int margin = (int)((gridsize - sectorsize) / 2);

			for (int x = (int)outerleft; x < (int)outerright; x += (int)gridsize)
			{
				for (int y = (int)outertop; y > (int)outerbottom; y -= (int)gridsize)
				{
					List<BlockEntry> blocks = blockmap.GetLineBlocks(
						new Vector2D(x + 1, y - 1),
						new Vector2D(x + gridsize - 1, y - gridsize + 1)
					);

					// The way our blockmap is built and queried we will always get exactly one block
					// Try the next position of the current one is occupied by another sector
					if (blocks[0].Sectors.Any(s => SectorInNewControlSectorSpace(x, y, s)))
						continue;

					positions.Add(new Vector2D(x + margin, y - margin));
					numsectors--;

					if (numsectors == 0)
						return positions;
				}
			}

			throw new NoSpaceInCSAException("Not enough space for control sector relocation");
		}

		public List<DrawnVertex> GetNewControlSectorVertices()
		{
			return GetNewControlSectorVertices(1);
		}

		public List<DrawnVertex> GetNewControlSectorVertices(int numsectors)
		{
			List<DrawnVertex> dv = new List<DrawnVertex>();
			BlockMap<BlockEntry> blockmap = CreateBlockmap();

			int margin = (int)((gridsize - sectorsize) / 2);

			// find position for new control sector
			for (int x = (int)outerleft; x < (int)outerright; x += (int)gridsize)
			{
				for (int y = (int)outertop; y > (int)outerbottom; y -= (int)gridsize)
				{
					List<BlockEntry> blocks = blockmap.GetLineBlocks(
						new Vector2D(x + 1, y - 1),
						new Vector2D(x + gridsize - 1, y - gridsize + 1)
					);

					// The way our blockmap is built and queried we will always get exactly one block
					// Try the next position of the current one is occupied by another sector
					if (blocks[0].Sectors.Any(s => SectorInNewControlSectorSpace(x, y, s)))
						continue;

					Point p = new Point(x + margin, y - margin);

					dv.Add(SectorVertex(p.X, p.Y));
					dv.Add(SectorVertex(p.X + BuilderPlug.Me.ControlSectorArea.SectorSize, p.Y));
					dv.Add(SectorVertex(p.X + BuilderPlug.Me.ControlSectorArea.SectorSize, p.Y - BuilderPlug.Me.ControlSectorArea.SectorSize));
					dv.Add(SectorVertex(p.X, p.Y - BuilderPlug.Me.ControlSectorArea.SectorSize));
					dv.Add(SectorVertex(p.X, p.Y));

					numsectors--;

					if (numsectors == 0)
						return dv;
				}
			}

			throw new NoSpaceInCSAException("No space left for control sectors");
		}

		/// <summary>
		/// Checks if the given sector and position for the new control sector intersect in any way.
		/// </summary>
		/// <param name="x">Leftmost X position of the new control sector space</param>
		/// <param name="y">Topmost Y position of the new control sector space</param>
		/// <param name="sector">The sector to check against</param>
		/// <returns>true if there's an intersection, false if there isn't</returns>
		private bool SectorInNewControlSectorSpace(int x, int y, Sector sector)
		{
			int margin = (int)((gridsize - sectorsize) / 2);
			HashSet<Vertex> sectorvertices = new HashSet<Vertex>();

			// Any of the sector's sidedef's linedef's vertices inside the new control sector's space?
			foreach (Sidedef sd in sector.Sidedefs)
			{
				sectorvertices.Add(sd.Line.Start);
				sectorvertices.Add(sd.Line.End);
			}

			foreach (Vertex v in sectorvertices)
				if (v.Position.x >= x + margin && v.Position.x <= x + margin + sectorsize && v.Position.y <= y - margin && v.Position.y >= y - margin - sectorsize)
					return true;

			// Any of the new vertex positions in the sector?
			Vector2D[] points = new Vector2D[]
			{
				new Vector2D(x + margin, y - margin),
				new Vector2D(x + margin + sectorsize, y - margin -sectorsize),
		 		new Vector2D(x + margin + sectorsize, y - margin - sectorsize),
				new Vector2D(x + margin, y -margin)
			};

			foreach (Vector2D v in points)
				if (sector.Intersect(v))
					return true;

			// Any of the new lines and the sector's lines overlapping?
			Line2D[] lines = new Line2D[]
			{
				new Line2D(points[0], points[1]),
				new Line2D(points[1], points[2]),
				new Line2D(points[2], points[3]),
				new Line2D(points[3], points[0])
			};

			foreach (Sidedef sd in sector.Sidedefs)
				foreach (Line2D line in lines)
					if (Line2D.GetIntersection(sd.Line.Line, line))
						return true;

			return false;
		}

		public bool Inside(float x, float y)
		{
			return Inside(new Vector2D(x, y));
		}

		public bool Inside(Vector2D pos)
		{
			if (pos.x > outerleft && pos.x < outerright && pos.y < outertop && pos.y > outerbottom)
				return true;

			return false;
		}

		public bool OutsideOuterBounds(float x, float y)
		{
			return OutsideOuterBounds(new Vector2D(x, y));
		}

		public bool OutsideOuterBounds(Vector2D pos)
		{
			if(pos.x < outerleft || pos.x > outerright || pos.y > outertop || pos.y < outerbottom)
				return true;

			return false;
		}

		// Aligns the area to the grid, expanding the area if necessary
		private RectangleF AlignAreaToGrid(RectangleF area)
		{
			List<float> f = new List<float>
			{
				area.Left,
				area.Top,
				area.Right,
				area.Bottom
			};

			for (int i = 0; i < f.Count; i++)
			{
				if (f[i] < 0)
					f[i] = (float)Math.Floor(f[i] / gridsize) * gridsize;
				else
					f[i] = (float)Math.Ceiling(f[i] / gridsize) * gridsize;
			}


			float l = f[0];
			float t = f[1];
			float r = f[2];
			float b = f[3];

			return new RectangleF(l, t, r - l, b - t);
		}

		private BlockMap<BlockEntry> CreateBlockmap()
		{
			return CreateBlockmap(false);
		}

		private BlockMap<BlockEntry> CreateBlockmap(bool ignorecontrolsectors)
		{
			// Make blockmap
			RectangleF area = MapSet.CreateArea(General.Map.Map.Vertices);
			area = MapSet.IncreaseArea(area, new Vector2D(outerleft, outertop));
			area = MapSet.IncreaseArea(area, new Vector2D(outerright, outerbottom));
			area = AlignAreaToGrid(area);

			BlockMap<BlockEntry> blockmap = new BlockMap<BlockEntry>(area, (int)gridsize);

			if (ignorecontrolsectors)
			{
				foreach (Sector s in General.Map.Map.Sectors)
				{
					// Managed control sectors have the custom UDMF field "user_managed_3d_floor" set to true
					// So if the field is NOT set, add the sector to the blockmap
					bool managed = s.Fields.GetValue("user_managed_3d_floor", false);

					if (managed == false)
						blockmap.AddSector(s);
					else // When a tag was manually removed a control sector still might have the user_managed_3d_floor field, but not be
					{    // recognized as a 3D floor control sector. In that case also add the sector to the blockmap
						bool orphaned = true;

						foreach(ThreeDFloor tdf in ((ThreeDFloorHelperMode)General.Editing.Mode).ThreeDFloors)
						{
							if(tdf.Sector == s)
							{
								orphaned = false;
								break;
							}
						}

						if (orphaned)
							blockmap.AddSector(s);
					}
				}
			}
			else
			{
				blockmap.AddSectorsSet(General.Map.Map.Sectors);
			}

			return blockmap;
		}

		public void Edit()
		{
			ControlSectorAreaConfig csacfg = new ControlSectorAreaConfig(this);

			csacfg.ShowDialog((Form)General.Interface);
		}

		// When OK is pressed on the preferences dialog
		// Prevent inlining, otherwise there are unexpected interactions with Assembly.GetCallingAssembly
		// See https://docs.microsoft.com/en-us/dotnet/api/system.reflection.assembly.getcallingassembly?view=netframework-4.6.1#remarks
		[MethodImplAttribute(MethodImplOptions.NoInlining)]
		public void SaveConfig()
		{
			ListDictionary config = new ListDictionary();

			config.Add("usecustomtagrange", usecustomtagrange);

			if (usecustomtagrange)
			{
				config.Add("firsttag", firsttag);
				config.Add("lasttag", lasttag);
			}

			config.Add("outerleft", outerleft);
			config.Add("outerright", outerright);
			config.Add("outertop", outertop);
			config.Add("outerbottom", outerbottom);

			General.Map.Options.WritePluginSetting("controlsectorarea", config);
		}

		// When OK is pressed on the preferences dialog
		// Prevent inlining, otherwise there are unexpected interactions with Assembly.GetCallingAssembly
		// See https://docs.microsoft.com/en-us/dotnet/api/system.reflection.assembly.getcallingassembly?view=netframework-4.6.1#remarks
		[MethodImplAttribute(MethodImplOptions.NoInlining)]
		public void LoadConfig()
		{
			ListDictionary config = (ListDictionary)General.Map.Options.ReadPluginSetting("controlsectorarea", new ListDictionary());

			usecustomtagrange = General.Map.Options.ReadPluginSetting("controlsectorarea.usecustomtagrange", false);
			firsttag = General.Map.Options.ReadPluginSetting("controlsectorarea.firsttag", 0);
			lasttag = General.Map.Options.ReadPluginSetting("controlsectorarea.lasttag", 0);

			outerleft = General.Map.Options.ReadPluginSetting("controlsectorarea.outerleft", outerleft);
			outerright = General.Map.Options.ReadPluginSetting("controlsectorarea.outerright", outerright);
			outertop = General.Map.Options.ReadPluginSetting("controlsectorarea.outertop", outertop);
			outerbottom = General.Map.Options.ReadPluginSetting("controlsectorarea.outerbottom", outerbottom);

			UpdateLinesAndPoints();
		}

		public int GetNewSectorTag(List<int> tagblacklist)
		{
			List<int> usedtags = new List<int>();

			if (usecustomtagrange)
			{
				for (int i = firsttag; i <= lasttag; i++)
				{
					if (!tagblacklist.Contains(i) && BuilderPlug.GetSectorsByTag(i).Count == 0)
						return i;
				}

				throw new Exception("No free tags in the custom range between " + firsttag.ToString() + " and " + lasttag.ToString() + ".");
			}

			return General.Map.Map.GetNewTag(tagblacklist);
		}

		public int GetNewLineID()
		{
			return General.Map.Map.GetNewTag();
		}

		// Turns a position into a DrawnVertex and returns it
		private DrawnVertex SectorVertex(double x, double y)
		{
			DrawnVertex v = new DrawnVertex();

			v.stitch = true;
			v.stitchline = true;
			v.pos = new Vector2D(Math.Round(x, General.Map.FormatInterface.VertexDecimals), Math.Round(y, General.Map.FormatInterface.VertexDecimals));

			return v;
		}

		private DrawnVertex SectorVertex(Vector2D v)
		{
			return SectorVertex(v.x, v.y);
		}

		static int GCD(int[] numbers)
		{
			return numbers.Aggregate(GCD);
		}

		static int GCD(int a, int b)
		{
			return b == 0 ? a : GCD(b, a % b);
		}

		#endregion
	}
}
