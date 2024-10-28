
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
using System.Drawing;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Actions;
using CodeImp.DoomBuilder.Editing;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Windows;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	[EditMode(DisplayName = "Draw Lines Mode",
			  SwitchAction = "drawlinesmode",
			  ButtonImage = "DrawGeometryMode.png", //mxd	
			  ButtonOrder = int.MinValue + 1, //mxd
			  ButtonGroup = "000_drawing", //mxd
			  AllowCopyPaste = false,
			  Volatile = true,
			  UseByDefault = true,
			  Optional = false)]

	public class DrawGeometryMode : BaseClassicMode
	{
		#region ================== Constants

		protected const float LINE_THICKNESS = 0.8f;

		#endregion

		#region ================== Variables

		// Drawing points
		protected List<DrawnVertex> points;
		protected List<LineLengthLabel> labels;
		private LineLengthLabel[] guidelabels; //mxd

		// Options
		protected bool snaptogrid;		// SHIFT to toggle
		protected bool snaptonearest;	// CTRL to enable
		protected bool snaptocardinaldirection; //mxd. ALT-SHIFT to enable
		protected bool usefourcardinaldirections;
		protected bool continuousdrawing; //mxd. Restart after finishing drawing?
		protected bool autoclosedrawing;  //mxd. Finish drawing when new points and existing geometry form a closed shape
		protected bool drawingautoclosed; //mxd
		protected bool showguidelines; //mxd
		protected bool placethingsatvertices;

		//mxd. Map area bounds
		private Line2D top, bottom, left, right;

		//mxd. Labels display style
		protected bool labelshowangle = true;
		protected bool labeluseoffset = true;

		//mxd. Interface
		private DrawLineOptionsPanel panel;
		
		#endregion

		#region ================== Properties

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public DrawGeometryMode()
		{
			// Initialize
			points = new List<DrawnVertex>();
			labels = new List<LineLengthLabel>();
			
			// No selection in this mode
			General.Map.Map.ClearAllSelected();
			General.Map.Map.ClearAllMarks(false);

			//mxd
			SetupInterface();
			
			// We have no destructor
			GC.SuppressFinalize(this);
		}

		// Disposer
		public override void Dispose()
		{
			// Not already disposed?
			if(!isdisposed)
			{
				// Clean up
				if(labels != null) foreach(LineLengthLabel l in labels) l.Dispose();
				if(guidelabels != null) foreach(LineLengthLabel l in guidelabels) l.Dispose();
				
				// Done
				base.Dispose();
			}
		}

		#endregion

		#region ================== Methods

		// This checks if the view offset/zoom changed and updates the check (never used. mxd)
		/*protected bool CheckViewChanged()
		{
			// View changed?
			bool viewchanged = (renderer.OffsetX != lastoffsetx || renderer.OffsetY != lastoffsety || renderer.Scale != lastscale);

			// Keep view information
			lastoffsetx = renderer.OffsetX;
			lastoffsety = renderer.OffsetY;
			lastscale = renderer.Scale;

			// Return result
			return viewchanged;
		}*/
		
		// This updates the dragging
		protected virtual void Update()
		{
			PixelColor stitchcolor = General.Colors.Highlight;
			PixelColor losecolor = General.Colors.Selection;

			snaptocardinaldirection = General.Interface.ShiftState && General.Interface.AltState; //mxd
			snaptogrid = (snaptocardinaldirection || General.Interface.ShiftState ^ General.Interface.SnapToGrid);
			snaptonearest = General.Interface.CtrlState ^ General.Interface.AutoMerge;

			DrawnVertex curp = GetCurrentPosition();
			float vsize = (renderer.VertexSize + 1.0f) / renderer.Scale;

			// Update label positions (mxd)
			if(labels.Count > 0)
			{
				// Update labels for already drawn lines
				for(int i = 0; i < labels.Count - 1; i++)
				{
					labels[i].ShowAngle = showguidelines;
					labels[i].Move(points[i].pos, points[i + 1].pos);
				}

				// Update label for active line
				labels[labels.Count - 1].ShowAngle = showguidelines;
				labels[labels.Count - 1].Move(points[points.Count - 1].pos, curp.pos);
			}

			// Render drawing lines
			if(renderer.StartOverlay(true))
			{
				// Go for all points to draw lines
				PixelColor color;
				if(points.Count > 0)
				{
					//mxd
					bool renderguidelabels = false;
					if(showguidelines)
					{
						Vector2D prevp = points[points.Count - 1].pos;
						renderguidelabels = (curp.pos.x != prevp.x && curp.pos.y != prevp.y);
						RenderGuidelines(prevp, curp.pos, General.Colors.Guideline.WithAlpha(80), -General.Map.Grid.GridRotate);
					}
					
					// Render lines
					DrawnVertex lastp = points[0];
					for(int i = 1; i < points.Count; i++)
					{
						// Determine line color
						if(lastp.stitchline && points[i].stitchline) color = stitchcolor;
						else color = losecolor;

						// Render line
						renderer.RenderLine(lastp.pos, points[i].pos, LINE_THICKNESS, color, true);
						RenderLinedefDirectionIndicator(lastp.pos, points[i].pos, color); //mxd
						lastp = points[i];
					}

					// Determine line color
					color = (lastp.stitchline && snaptonearest ? stitchcolor : losecolor);

					// Render line to cursor
					renderer.RenderLine(lastp.pos, curp.pos, LINE_THICKNESS, color, true);
					RenderLinedefDirectionIndicator(lastp.pos, curp.pos, color); //mxd

					// Render vertices
					for(int i = 0; i < points.Count; i++)
					{
						// Determine vertex color
						color = points[i].stitch ? stitchcolor : losecolor;

						// Render vertex
						renderer.RenderRectangleFilled(new RectangleF((float)(points[i].pos.x - vsize), (float)(points[i].pos.y - vsize), vsize * 2.0f, vsize * 2.0f), color, true);
					}

					//mxd. Render guide labels?
					if(renderguidelabels) renderer.RenderText(guidelabels);

					// Render labels
					renderer.RenderText(labels.ToArray());
				}

				// Determine point color
				color = snaptonearest ? stitchcolor : losecolor;

				// Render vertex at cursor
				renderer.RenderRectangleFilled(new RectangleF((float)(curp.pos.x - vsize), (float)(curp.pos.y - vsize), vsize * 2.0f, vsize * 2.0f), color, true);

				// Done
				renderer.Finish();
			}

			// Done
			renderer.Present();
		}

		protected void RenderGuidelines(Vector2D start, Vector2D end, PixelColor c)
		{
			RenderGuidelines(start, end, c, 0.0);
		}

		//mxd
		protected void RenderGuidelines(Vector2D start, Vector2D end, PixelColor c, double angle)
		{
			start = start.GetRotated(angle);
			end = end.GetRotated(angle);

			if(end.x != start.x && end.y != start.y)
			{
				Vector2D tr = new Vector2D(Math.Max(end.x, start.x), Math.Max(end.y, start.y));
				Vector2D bl = new Vector2D(Math.Min(end.x, start.x), Math.Min(end.y, start.y));

				// Create guidelines
				Line3D[] lines = new Line3D[5];
				lines[0] = new Line3D(new Vector2D(tr.x, General.Map.Config.TopBoundary).GetRotated(-angle), new Vector2D(tr.x, General.Map.Config.BottomBoundary).GetRotated(-angle), c, false);
				lines[1] = new Line3D(new Vector2D(bl.x, General.Map.Config.TopBoundary).GetRotated(-angle), new Vector2D(bl.x, General.Map.Config.BottomBoundary).GetRotated(-angle), c, false);
				lines[2] = new Line3D(new Vector2D(General.Map.Config.LeftBoundary, tr.y).GetRotated(-angle), new Vector2D(General.Map.Config.RightBoundary, tr.y).GetRotated(-angle), c, false);
				lines[3] = new Line3D(new Vector2D(General.Map.Config.LeftBoundary, bl.y).GetRotated(-angle), new Vector2D(General.Map.Config.RightBoundary, bl.y).GetRotated(-angle), c, false);

				// Create current line extent. Make sure v1 is to the left of v2
				Line2D current = (end.x < start.x ? new Line2D(end, start) : new Line2D(start, end));

				Vector2D extentstart, extentend;
				if(current.v1.y < current.v2.y) // Start is lower
				{
					// Start point can hit left or bottom boundaries
					extentstart = Line2D.GetIntersectionPoint(left, current, false);
					if(extentstart.y < General.Map.Config.BottomBoundary)
						extentstart = Line2D.GetIntersectionPoint(bottom, current, false);

					// End point can hit right or top boundaries
					extentend = Line2D.GetIntersectionPoint(right, current, false);
					if(extentend.y > General.Map.Config.TopBoundary)
						extentend = Line2D.GetIntersectionPoint(top, current, false);
				}
				else // Start is higher
				{
					// Start point can hit left or top boundaries
					extentstart = Line2D.GetIntersectionPoint(left, current, false);
					if(extentstart.y > General.Map.Config.TopBoundary)
						extentstart = Line2D.GetIntersectionPoint(top, current, false);

					// End point can hit right or bottom boundaries
					extentend = Line2D.GetIntersectionPoint(right, current, false);
					if(extentend.y < General.Map.Config.BottomBoundary)
						extentend = Line2D.GetIntersectionPoint(bottom, current, false);
				}

				lines[4] = new Line3D(extentstart.GetRotated(-angle), extentend.GetRotated(-angle), c, false);

				// Render them
				renderer.RenderArrows(lines);

				// Update horiz/vert length labels
				if(guidelabels != null)
				{
					guidelabels[0].Move(tr.GetRotated(-angle), new Vector2D(tr.x, bl.y).GetRotated(-angle));
					guidelabels[1].Move(new Vector2D(bl.x, tr.y).GetRotated(-angle), tr.GetRotated(-angle));
					guidelabels[2].Move(new Vector2D(tr.x, bl.y).GetRotated(-angle), bl.GetRotated(-angle));
					guidelabels[3].Move(bl.GetRotated(-angle), new Vector2D(bl.x, tr.y).GetRotated(-angle));
				}
			}
			// Render horizontal line + 2 vertical guidelines
			else if(end.x != start.x)
			{
				Line3D l = new Line3D(new Vector2D(General.Map.Config.LeftBoundary, end.y).GetRotated(-angle), new Vector2D(General.Map.Config.RightBoundary, end.y).GetRotated(-angle), c, false);
				Line3D gs = new Line3D(new Vector2D(start.x, General.Map.Config.TopBoundary).GetRotated(-angle), new Vector2D(start.x, General.Map.Config.BottomBoundary).GetRotated(-angle), c, false);
				Line3D ge = new Line3D(new Vector2D(end.x, General.Map.Config.TopBoundary).GetRotated(-angle), new Vector2D(end.x, General.Map.Config.BottomBoundary).GetRotated(-angle), c, false);
				renderer.RenderArrows(new List<Line3D> { l, gs, ge });
			}
			// Render vertical line + 2 horizontal guidelines
			else if(end.y != start.y)
			{
				Line3D l = new Line3D(new Vector2D(end.x, General.Map.Config.TopBoundary).GetRotated(-angle), new Vector2D(end.x, General.Map.Config.BottomBoundary).GetRotated(-angle), c, false);
				Line3D gs = new Line3D(new Vector2D(General.Map.Config.LeftBoundary, start.y).GetRotated(-angle), new Vector2D(General.Map.Config.RightBoundary, start.y).GetRotated(-angle), c, false);
				Line3D ge = new Line3D(new Vector2D(General.Map.Config.LeftBoundary, end.y).GetRotated(-angle), new Vector2D(General.Map.Config.RightBoundary, end.y).GetRotated(-angle), c, false);
				renderer.RenderArrows(new List<Line3D> {l, gs, ge});
			}
			// Start and end match. Render a cross
			else
			{
				Line3D gs = new Line3D(new Vector2D(General.Map.Config.LeftBoundary, start.y).GetRotated(-angle), new Vector2D(General.Map.Config.RightBoundary, start.y).GetRotated(-angle), c, false);
				Line3D ge = new Line3D(new Vector2D(start.x, General.Map.Config.TopBoundary).GetRotated(-angle), new Vector2D(start.x, General.Map.Config.BottomBoundary).GetRotated(-angle), c, false);
				renderer.RenderArrows(new List<Line3D> { gs, ge });
			}
		}

		//mxd
		private void RenderLinedefDirectionIndicator(Vector2D start, Vector2D end, PixelColor color) 
		{
			Vector2D delta = end - start;
			Vector2D middlePoint = new Vector2D(start.x + delta.x / 2, start.y + delta.y / 2);
			Vector2D scaledPerpendicular = delta.GetPerpendicular().GetNormal().GetScaled(18f / renderer.Scale);
			renderer.RenderLine(middlePoint, new Vector2D(middlePoint.x - scaledPerpendicular.x, middlePoint.y - scaledPerpendicular.y), LINE_THICKNESS, color, true);
		}

		// This returns the aligned and snapped draw position
		public static DrawnVertex GetCurrentPosition(Vector2D mousemappos, bool snaptonearest, bool snaptogrid, bool snaptocardinal, bool usefourcardinaldirections, IRenderer2D renderer, List<DrawnVertex> points)
		{
			return GetCurrentPosition(mousemappos, snaptonearest, snaptogrid, snaptocardinal, usefourcardinaldirections, renderer, points, null);
		}

		// This returns the aligned and snapped draw position
		public static DrawnVertex GetCurrentPosition(Vector2D mousemappos, bool snaptonearest, bool snaptogrid, bool snaptocardinal, bool usefourcardinaldirections, IRenderer2D renderer, List<DrawnVertex> points, BlockMap<BlockEntry> blockmap)
		{
			DrawnVertex p = new DrawnVertex();
			p.stitch = true; //mxd. Setting these to false seems to be a good way to create invalid geometry...
			p.stitchline = true; //mxd
			snaptocardinal = (snaptocardinal && points.Count > 0); //mxd. Don't snap to cardinal when there are no points

			//mxd. If snap to cardinal directions is enabled and we have points, modify mouse position
			Vector2D vm, gridoffset;
			if(snaptocardinal)
			{
				Vector2D offset = mousemappos - points[points.Count - 1].pos;

				double angle;
				if(usefourcardinaldirections)
					angle = Angle2D.DegToRad((General.ClampAngle((int)Angle2D.RadToDeg(offset.GetAngle()))) / 90 * 90 + 45);
				else
					angle = Angle2D.DegToRad((General.ClampAngle((int)Angle2D.RadToDeg(offset.GetAngle()) + 22)) / 45 * 45);

				offset = new Vector2D(0, -offset.GetLength()).GetRotated(angle);
				vm = points[points.Count - 1].pos + offset;

				//mxd. We need to be snapped relative to initial position
				Vector2D prev = points[points.Count - 1].pos;
				gridoffset = prev - General.Map.Grid.SnappedToGrid(prev);
			}
			else
			{
				vm = mousemappos;
				gridoffset = new Vector2D();
			}
			
			float vrange = BuilderPlug.Me.StitchRange / renderer.Scale;

			// Snap to nearest?
			if(snaptonearest)
			{
				// Go for all drawn points
				foreach(DrawnVertex v in points)
				{
					if(Vector2D.DistanceSq(vm, v.pos) < (vrange * vrange))
					{
						p.pos = v.pos;
						return p;
					}
				}

				List<Vertex> vertices = new List<Vertex>();
				Vertex nv = null;

				// If we got a blockmap get the veritces that are in range only
				if (blockmap != null)
				{
					HashSet<BlockEntry> blocks = blockmap.GetSquareRange(vm.x - vrange, vm.y - vrange, vrange * 2, vrange * 2);
					foreach (BlockEntry be in blocks)
						vertices.AddRange(be.Vertices);

					nv = MapSet.NearestVertexSquareRange(vertices, vm, vrange);
				}
				else
					nv = General.Map.Map.NearestVertexSquareRange(vm, vrange);

				// Try the nearest vertex
				if(nv != null)
				{
					//mxd. Line angle must stay the same
					if(snaptocardinal)
					{
						Line2D ourline = new Line2D(points[points.Count - 1].pos, vm);
						if(Math.Round(ourline.GetSideOfLine(nv.Position), 1) == 0)
						{
							p.pos = nv.Position;
							return p;
						}
					}
					else
					{
						p.pos = nv.Position;
						return p;
					}
				}

				// Try the nearest linedef. mxd. We'll need much bigger stitch distance when snapping to cardinal directions
				Linedef nl = blockmap != null ? MapSet.NearestLinedefRange(blockmap, vm, BuilderPlug.Me.StitchRange / renderer.Scale) : General.Map.Map.NearestLinedefRange(vm, BuilderPlug.Me.StitchRange / renderer.Scale);
				if(nl != null)
				{
					//mxd. Line angle must stay the same
					if(snaptocardinal)
					{
						Line2D ourline = new Line2D(points[points.Count - 1].pos, vm);
						Line2D nearestline = new Line2D(nl.Start.Position, nl.End.Position);
						Vector2D intersection = Line2D.GetIntersectionPoint(nearestline, ourline, false);
						if(!double.IsNaN(intersection.x))
						{
							// Intersection is on nearestline?
							double u = Line2D.GetNearestOnLine(nearestline.v1, nearestline.v2, intersection);

							if(u < 0f || u > 1f) { }
							else
							{
								p.pos = new Vector2D(Math.Round(intersection.x, General.Map.FormatInterface.VertexDecimals),
													 Math.Round(intersection.y, General.Map.FormatInterface.VertexDecimals));
								return p;
							}
						}
					}
					// Snap to grid?
					else if(snaptogrid)
					{
						// Get grid intersection coordinates
						List<Vector2D> coords = nl.GetGridIntersections(General.Map.Grid.GridRotate,
							General.Map.Grid.GridOriginX, General.Map.Grid.GridOriginY);

						// Find nearest grid intersection
						bool found = false;
						double found_distance = double.MaxValue;
						Vector2D found_coord = new Vector2D();
						foreach(Vector2D v in coords)
						{
							Vector2D delta = vm - v;
							if(delta.GetLengthSq() < found_distance)
							{
								found_distance = delta.GetLengthSq();
								found_coord = v;
								found = true;
							}
						}

						if(found)
						{
							// Align to the closest grid intersection
							p.pos = found_coord;
							return p;
						}
					}
					else
					{
						// Aligned to line
						p.pos = nl.NearestOnLine(vm);
						return p;
					}
				}
			}
			else
			{
				// Always snap to the first drawn vertex so that the user can finish a complete sector without stitching
				if(points.Count > 0)
				{
					if(Vector2D.DistanceSq(vm, points[0].pos) < (vrange * vrange))
					{
						p.pos = points[0].pos;
						return p;
					}
				}
			}

			// if the mouse cursor is outside the map bondaries check if the line between the last set point and the
			// mouse cursor intersect any of the boundary lines. If it does, set the position to this intersection
			if(points.Count > 0 &&
				(mousemappos.x < General.Map.Config.LeftBoundary || mousemappos.x > General.Map.Config.RightBoundary ||
				mousemappos.y > General.Map.Config.TopBoundary || mousemappos.y < General.Map.Config.BottomBoundary))
			{
				Line2D dline = new Line2D(mousemappos, points[points.Count - 1].pos);
				bool foundintersection = false;
				double u = 0.0;
				List<Line2D> blines = new List<Line2D>();

				// lines for left, top, right and bottom boundaries
				blines.Add(new Line2D(General.Map.Config.LeftBoundary, General.Map.Config.BottomBoundary, General.Map.Config.LeftBoundary, General.Map.Config.TopBoundary));
				blines.Add(new Line2D(General.Map.Config.LeftBoundary, General.Map.Config.TopBoundary, General.Map.Config.RightBoundary, General.Map.Config.TopBoundary));
				blines.Add(new Line2D(General.Map.Config.RightBoundary, General.Map.Config.TopBoundary, General.Map.Config.RightBoundary, General.Map.Config.BottomBoundary));
				blines.Add(new Line2D(General.Map.Config.RightBoundary, General.Map.Config.BottomBoundary, General.Map.Config.LeftBoundary, General.Map.Config.BottomBoundary));

				// check for intersections with boundaries
				for(int i = 0; i < blines.Count; i++)
				{
					if(!foundintersection)
					{
						// only check for intersection if the last set point is not on the
						// line we are checking against
						if(blines[i].GetSideOfLine(points[points.Count - 1].pos) != 0.0)
						{
							foundintersection = blines[i].GetIntersection(dline, out u);
						}
					}
				}

				// if there was no intersection set the position to the last set point
				if(!foundintersection)
					vm = points[points.Count - 1].pos;
				else
					vm = dline.GetCoordinatesAt(u);
			}

			// Snap to grid?
			if(snaptogrid)
			{
				// Aligned to grid
				p.pos = General.Map.Grid.SnappedToGrid(vm - gridoffset) + gridoffset;

				// special handling 
				if(p.pos.x > General.Map.Config.RightBoundary) p.pos.x = General.Map.Config.RightBoundary;
				if(p.pos.y < General.Map.Config.BottomBoundary) p.pos.y = General.Map.Config.BottomBoundary;

				return p;
			}
			else
			{
				// Normal position
				p.pos.x = Math.Round(vm.x); //mxd
				p.pos.y = Math.Round(vm.y); //mxd

				return p;
			}
		}
		
		// This gets the aligned and snapped draw position
		protected DrawnVertex GetCurrentPosition()
		{
			return GetCurrentPosition(mousemappos, snaptonearest, snaptogrid, snaptocardinaldirection, usefourcardinaldirections, renderer, points);
		}
		
		// This draws a point at a specific location
		public bool DrawPointAt(DrawnVertex p)
		{
			return DrawPointAt(p.pos, p.stitch, p.stitchline);
		}
		
		// This draws a point at a specific location
		public virtual bool DrawPointAt(Vector2D pos, bool stitch, bool stitchline)
		{
			if(pos.x < General.Map.Config.LeftBoundary || pos.x > General.Map.Config.RightBoundary ||
				pos.y > General.Map.Config.TopBoundary || pos.y < General.Map.Config.BottomBoundary)
				return false;

			//mxd. Avoid zero-length lines...
			if(points.Count > 0)
			{
				Vector2D delta = points[points.Count - 1].pos - pos;
				if((Math.Abs(delta.x) <= 0.001f) && (Math.Abs(delta.y) <= 0.001f))
					return true;
			}

			DrawnVertex newpoint = new DrawnVertex();
			newpoint.pos = pos;
			newpoint.stitch = stitch;
			newpoint.stitchline = stitchline;
			points.Add(newpoint);
			labels.Add(new LineLengthLabel(labelshowangle, labeluseoffset));
			Update();

			if(points.Count > 1)
			{
				// Check if point stitches with the first
				if(points[points.Count - 1].stitch)
				{
					Vector2D p1 = points[0].pos;
					Vector2D p2 = points[points.Count - 1].pos;
					Vector2D delta = p1 - p2;
					if((Math.Abs(delta.x) <= 0.001f) && (Math.Abs(delta.y) <= 0.001f))
					{
						//mxd. Seems... logical?
						if(points.Count == 2)
						{
							OnCancel();
							return true;
						}

						// Finish drawing
						FinishDraw();
						return true;
					}
				}
				
				//mxd. Points and existing geometry form a closed shape?
				if(autoclosedrawing)
				{
					// Determive center point
					double minx = float.MaxValue;
					double maxx = float.MinValue;
					double miny = float.MaxValue;
					double maxy = float.MinValue;

					foreach(DrawnVertex v in points)
					{
						if(v.pos.x < minx) minx = v.pos.x;
						if(v.pos.x > maxx) maxx = v.pos.x;
						if(v.pos.y < miny) miny = v.pos.y;
						if(v.pos.y > maxy) maxy = v.pos.y;
					}

					Vector2D shapecenter = new Vector2D(minx + (maxx - minx) / 2, miny + (maxy - miny) / 2);
					
					// Determine center point between start and end points
					minx = Math.Min(points[0].pos.x, points[points.Count - 1].pos.x);
					maxx = Math.Max(points[0].pos.x, points[points.Count - 1].pos.x);
					miny = Math.Min(points[0].pos.y, points[points.Count - 1].pos.y);
					maxy = Math.Max(points[0].pos.y, points[points.Count - 1].pos.y);

					Vector2D startendcenter = new Vector2D(minx + (maxx - minx) / 2, miny + (maxy - miny) / 2);

					// Offset the center perpendicular to the start -> end line direction...
					if(shapecenter == startendcenter)
					{
						shapecenter -= new Line2D(points[0].pos, points[points.Count - 1].pos).GetPerpendicular().GetNormal();
					}

					// Do the check
					if(CanFinishDrawing(points[0].pos, points[points.Count - 1].pos, shapecenter))
					{
						drawingautoclosed = true;
						FinishDraw();
					}
				}
			}

			return true;
		}

		//mxd
		private static bool CanFinishDrawing(Vector2D start, Vector2D end, Vector2D center)
		{
			Linedef startline = FindPotentialLine(start, center);
			if(startline == null) return false;

			Linedef endline = FindPotentialLine(end, center);
			if(endline == null) return false;

			// Can finish drawing if a path between startline and endline exists
			return Tools.FindClosestPath(startline, startline.SideOfLine(center) < 0.0f, endline, endline.SideOfLine(center) < 0.0f, true) != null;
		}

		//mxd
		private static Linedef FindPotentialLine(Vector2D target, Vector2D center)
		{
			// Target position on top of existing vertex?
			Vertex v = General.Map.Map.NearestVertex(target);
			if(v == null) return null;

			Linedef result = null;
			if(v.Position == target)
			{
				double mindistance = double.MaxValue;
				foreach(Linedef l in v.Linedefs)
				{
					if(result == null)
					{
						result = l;
						mindistance = Vector2D.DistanceSq(l.GetCenterPoint(), center);
					}
					else
					{
						double curdistance = Vector2D.DistanceSq(l.GetCenterPoint(), center);
						if(curdistance < mindistance)
						{
							mindistance = curdistance;
							result = l;
						}
					}
				}
			}
			else
			{
				// Result position will split a line?
				result = General.Map.Map.NearestLinedef(target);
				if(result.SideOfLine(target) != 0) return null;
			}

			return result;
		}
		
		#endregion

		#region ================== mxd. Settings panel

		protected virtual void SetupInterface()
		{
			//Add options docker
			panel = new DrawLineOptionsPanel();
			panel.OnContinuousDrawingChanged += OnContinuousDrawingChanged;
			panel.OnAutoCloseDrawingChanged += OnAutoCloseDrawingChanged;
			panel.OnShowGuidelinesChanged += OnShowGuidelinesChanged;

			// Needs to be set after adding the events...
			panel.ContinuousDrawing = General.Settings.ReadPluginSetting("drawlinesmode.continuousdrawing", false);
			panel.AutoCloseDrawing = General.Settings.ReadPluginSetting("drawlinesmode.autoclosedrawing", false);
			panel.ShowGuidelines = General.Settings.ReadPluginSetting("drawlinesmode.showguidelines", false);

			// Create guide labels
			guidelabels = new LineLengthLabel[4];
			for(int i = 0; i < guidelabels.Length; i++)
			{
				guidelabels[i] = new LineLengthLabel { ShowAngle = false, Color = General.Colors.InfoLine };
			}

			// Create map boudary lines
			Vector2D btl = new Vector2D(General.Map.Config.LeftBoundary, General.Map.Config.TopBoundary);
			Vector2D btr = new Vector2D(General.Map.Config.RightBoundary, General.Map.Config.TopBoundary);
			Vector2D bbl = new Vector2D(General.Map.Config.LeftBoundary, General.Map.Config.BottomBoundary);
			Vector2D bbr = new Vector2D(General.Map.Config.RightBoundary, General.Map.Config.BottomBoundary);
			top = new Line2D(btl, btr);
			right = new Line2D(btr, bbr);
			bottom = new Line2D(bbl, bbr);
			left = new Line2D(btl, bbl); 
		}

		protected virtual void AddInterface()
		{
			panel.Register();
		}

		protected virtual void RemoveInterface()
		{
			General.Settings.WritePluginSetting("drawlinesmode.continuousdrawing", panel.ContinuousDrawing);
			General.Settings.WritePluginSetting("drawlinesmode.autoclosedrawing", panel.AutoCloseDrawing);
			General.Settings.WritePluginSetting("drawlinesmode.showguidelines", panel.ShowGuidelines);
			panel.Unregister();
		}

		#endregion

		#region ================== Events

		public override void OnHelp()
		{
			General.ShowHelp("e_drawgeometry.html");
		}

		// Engaging
		public override void OnEngage()
		{
			base.OnEngage();
			EnableAutoPanning();
			AddInterface(); //mxd
			renderer.SetPresentation(Presentation.Standard);
			
			// Set cursor
			General.Interface.SetCursor(Cursors.Cross);
		}

		// Disengaging
		public override void OnDisengage()
		{
			RemoveInterface(); //mxd
			base.OnDisengage();
			DisableAutoPanning();
		}
		
		// Cancelled
		public override void OnCancel()
		{
			//mxd. Cannot leave this way when continuous drawing is enabled
			if(continuousdrawing)
			{
				drawingautoclosed = false;
				return;
			}
			
			// Cancel base class
			base.OnCancel();
			
			// Return to original mode
			General.Editing.ChangeMode(General.Editing.PreviousStableMode.Name);
		}

		// Accepted
		public override void OnAccept()
		{
			Cursor.Current = Cursors.AppStarting;
			General.Settings.FindDefaultDrawSettings();

			// When points have been drawn
			if(points.Count > 0)
			{
				// Make undo for the draw
				General.Map.UndoRedo.CreateUndo("Line draw");
				
				// Make an analysis and show info
				string[] adjectives = new[]
				{ "beautiful", "lovely", "romantic", "stylish", "cheerful", "comical",
				  "awesome", "accurate", "adorable", "adventurous", "attractive", "cute",
				  "elegant", "glamorous", "gorgeous", "handsome", "magnificent", "unusual",
				  "outstanding", "mysterious", "amusing", "charming", "fantastic", "jolly" };
				string word = adjectives[points.Count % adjectives.Length];
				word = (points.Count > adjectives.Length) ? "very " + word : word;
				string a = ((word[0] == 'a') || (word[0] == 'e') || (word[0] == 'o') || (word[0] == 'u')) ? "an " : "a ";
				General.Interface.DisplayStatus(StatusType.Action, "Created " + a + word + " drawing.");
				
				// Make the drawing
				if(Tools.DrawLines(points, true, BuilderPlug.Me.AutoAlignTextureOffsetsOnCreate)) //mxd
				{
					// Snap to map format accuracy
					General.Map.Map.SnapAllToAccuracy();

					// Clear selection
					General.Map.Map.ClearAllSelected();

					// Update cached values
					General.Map.Map.Update();

					//mxd. Outer sectors may require some splittin...
					if(General.Settings.SplitJoinedSectors) Tools.SplitOuterSectors(General.Map.Map.GetMarkedLinedefs(true));

					// Edit new sectors?
					List<Sector> newsectors = General.Map.Map.GetMarkedSectors(true);
					if(BuilderPlug.Me.EditNewSector && (newsectors.Count > 0))
						General.Interface.ShowEditSectors(newsectors);

					// Update the used textures
					General.Map.Data.UpdateUsedTextures();

					//mxd
					General.Map.Renderer2D.UpdateExtraFloorFlag();

					// Map is changed
					General.Map.IsChanged = true;
				}
				else
				{
					// Drawing failed
					General.Map.UndoRedo.WithdrawUndo();
				}
			}

			// Done
			Cursor.Current = Cursors.Default;

			if(continuousdrawing)
			{
				//mxd. Reset settings
				points.Clear();
				labels.Clear();
				drawingautoclosed = false;

				//mxd. Redraw display
				General.Interface.RedrawDisplay();
			}
			else
			{
				// Return to original mode
				General.Editing.ChangeMode(General.Editing.PreviousStableMode.Name);
			}
		}

		// This redraws the display
		public override void OnRedrawDisplay()
		{
			renderer.RedrawSurface();

			// Render lines
			if(renderer.StartPlotter(true))
			{
				renderer.PlotLinedefSet(General.Map.Map.Linedefs);
				renderer.PlotVerticesSet(General.Map.Map.Vertices);
				renderer.Finish();
			}

			// Render things
			if(renderer.StartThings(true))
			{
				renderer.RenderThingSet(General.Map.Map.Things, General.Settings.ActiveThingsAlpha);
				renderer.Finish();
			}

			// Normal update
			Update();
		}
		
		// Mouse moving
		public override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			if(panning) return; //mxd. Skip all this jazz while panning
			Update();
		}

		// When a key is released
		public override void OnKeyUp(KeyEventArgs e)
		{
			base.OnKeyUp(e);
			if((snaptogrid != (snaptocardinaldirection || (General.Interface.ShiftState ^ General.Interface.SnapToGrid))) ||
			   (snaptonearest != (General.Interface.CtrlState ^ General.Interface.AutoMerge)) ||
			   (snaptocardinaldirection != (General.Interface.AltState && General.Interface.ShiftState))) Update();
		}

		// When a key is pressed
		public override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);
			if ((snaptogrid != (snaptocardinaldirection || (General.Interface.ShiftState ^ General.Interface.SnapToGrid))) ||
			   (snaptonearest != (General.Interface.CtrlState ^ General.Interface.AutoMerge)) ||
			   (snaptocardinaldirection != (General.Interface.AltState && General.Interface.ShiftState))) Update();
		}

		//mxd
		protected void OnContinuousDrawingChanged(object value, EventArgs e)
		{
			continuousdrawing = (bool)value;
		}

		//mxd
		protected void OnAutoCloseDrawingChanged(object value, EventArgs e)
		{
			autoclosedrawing = (bool)value;
		}

		//mxd
		protected void OnShowGuidelinesChanged(object value, EventArgs e)
		{
			showguidelines = (bool)value;
			General.Interface.RedrawDisplay();
		}
	
		protected void OnPlaceThingsAtVerticesChanged(object value, EventArgs e)
		{
			placethingsatvertices = (bool)value;
			General.Interface.RedrawDisplay();
		}

		#endregion

		#region ================== Actions

		// Drawing a point
		[BeginAction("drawpoint")]
		public void DrawPoint()
		{
			// Mouse inside window?
			if(General.Interface.MouseInDisplay)
			{
				DrawnVertex newpoint = GetCurrentPosition();
				if(!DrawPointAt(newpoint)) General.Interface.DisplayStatus(StatusType.Warning, "Failed to draw point: outside of map boundaries.");
			}
		}
		
		// Remove last point
		[BeginAction("removepoint")]
		public virtual void RemovePoint() { RemovePointAt(points.Count - 1); }

		//mxd. Remove first point 
		[BeginAction("removefirstpoint")]
		public virtual void RemoveFirstPoint() { RemovePointAt(0); }

		//mxd
		private void RemovePointAt(int index)
		{
			if(points.Count > 0 && points.Count > index) points.RemoveAt(index);
			if(labels.Count > 0 && labels.Count > index)
			{
				labels[index].Dispose();
				labels.RemoveAt(index);
			}
			
			Update();
		}
		
		// Finish drawing
		[BeginAction("finishdraw")]
		public void FinishDraw()
		{
			// Accept the changes
			General.Editing.AcceptMode();
		}
		
		#endregion
	}
}
