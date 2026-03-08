
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
using System.Windows.Forms;
using System.Linq;
using CodeImp.DoomBuilder.Windows;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Types;
using CodeImp.DoomBuilder.Geometry;
using System.Drawing;
using CodeImp.DoomBuilder.Editing;
using CodeImp.DoomBuilder.Actions;

#endregion

namespace CodeImp.DoomBuilder.ThreeDFloorMode
{
	enum SlopeDrawingMode { Floor, Ceiling, FloorAndCeiling };

	[EditMode(DisplayName = "Draw Slopes Mode",
			  SwitchAction = "drawslopesmode",
			  ButtonImage = "DrawSlopeModeIcon.png",	// Image resource name for the button
			  ButtonOrder = int.MinValue + 501,	// Position of the button (lower is more to the left)
			  ButtonGroup = "000_editing",
			  AllowCopyPaste = false,
			  SupportedMapFormats = new[] { "UniversalMapSetIO" },
			  RequiredMapFeatures = new[] { "PlaneEquationSupport" },
			  Volatile = true,
			  UseByDefault = true,
			  Optional = false,
			  IsDeprecated = true,
			  DeprecationMessage = "Please use the visual sloping functionality instead.")]
	public class DrawSlopesMode : ClassicMode
	{
		#region ================== Constants

		private const float LINE_THICKNESS = 0.8f;

		#endregion

		#region ================== Variables

		// Drawing points
		private List<DrawnVertex> points;

		// Keep track of view changes
		private float lastoffsetx;
		private float lastoffsety;
		private float lastscale;

		// Options
		private bool snaptogrid;		// SHIFT to toggle
		private bool snaptonearest;		// CTRL to enable

		private FlatVertex[] overlayGeometry;
		private List<TextLabel> labels;

		private static SlopeDrawingMode slopedrawingmode = SlopeDrawingMode.Floor;

		#endregion

		#region ================== Properties

		// Just keep the base mode button checked
		public override string EditModeButtonName { get { return General.Editing.PreviousStableMode.Name; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public DrawSlopesMode()
		{
			// Initialize
			points = new List<DrawnVertex>();

			// No selection in this mode
			//General.Map.Map.ClearAllSelected();
			//General.Map.Map.ClearAllMarks(false);

			// We have no destructor
			GC.SuppressFinalize(this);
		}

		// Disposer
		public override void Dispose()
		{
			// Not already disposed?
			if (!isdisposed)
			{
				// Dispose old labels
				if (labels != null)
					foreach (TextLabel l in labels)
						l.Dispose();

				// Done
				base.Dispose();
			}
		}

		#endregion

		#region ================== Methods

		// This checks if the view offset/zoom changed and updates the check
		protected bool CheckViewChanged()
		{
			bool viewchanged = false;

			// View changed?
			if (renderer.OffsetX != lastoffsetx) viewchanged = true;
			if (renderer.OffsetY != lastoffsety) viewchanged = true;
			if (renderer.Scale != lastscale) viewchanged = true;

			// Keep view information
			lastoffsetx = renderer.OffsetX;
			lastoffsety = renderer.OffsetY;
			lastscale = renderer.Scale;

			// Return result
			return viewchanged;
		}

		// This sets up new labels
		/*
		private void SetupLabels()
		{
			if (labels != null)
			{
				// Dispose old labels
				foreach (KeyValuePair<Sector, TextLabel[]> lbl in labels)
					foreach (TextLabel l in lbl.Value) l.Dispose();
			}

			// Make text labels for sectors
			labels = new Dictionary<Sector, TextLabel[]>(General.Map.Map.Sectors.Count);
			foreach (Sector s in General.Map.Map.Sectors)
			{
				// Setup labels
				TextLabel[] labelarray = new TextLabel[s.Labels.Count];
				for (int i = 0; i < s.Labels.Count; i++)
				{
					Vector2D v = s.Labels[i].position;
					labelarray[i] = new TextLabel(20);
					labelarray[i].TransformCoords = true;
					labelarray[i].Rectangle = new RectangleF(v.x, v.y, 0.0f, 0.0f);
					labelarray[i].AlignX = TextAlignmentX.Center;
					labelarray[i].AlignY = TextAlignmentY.Middle;
					labelarray[i].Scale = 14f;
					labelarray[i].Color = General.Colors.Highlight.WithAlpha(255);
					labelarray[i].Backcolor = General.Colors.Background.WithAlpha(255);
				}
				labels.Add(s, labelarray);
			}
		}
		*/

		private void SetupLabels()
		{
			labels = new List<TextLabel>();
			PixelColor white = new PixelColor(255, 255, 255, 255);

			foreach (SlopeVertexGroup svg in BuilderPlug.Me.SlopeVertexGroups)
			{
				for (int i = 0; i < svg.Vertices.Count; i++)
				{
					if (BuilderPlug.Me.SlopeVertexLabelDisplayOption == LabelDisplayOption.Always || General.Interface.AltState == true)
					{
						SlopeVertex sv = svg.Vertices[i];
						double scale = 1 / renderer.Scale;
						double x = sv.Pos.x;
						double y = sv.Pos.y - 14 * scale;
						string value = String.Format("Z: {0}", sv.Z);
						bool showlabel = true;

						// Rearrange labels if they'd be (exactly) on each other
						foreach (TextLabel l in labels)
						{
							if (l.Location.x == x && l.Location.y == y) {
								// Reduce visual clutter by de-duping stacked labels, when "show all labels" is enabled
								if (l.Text == value) {
									showlabel = false; //dedupe
								} else {
									// Adjust the label position down one line
									y -= l.TextSize.Height * scale;
								}
							}
						}

						// Only proceed if the label was not deduped
						if (showlabel)
						{
							TextLabel label = new TextLabel();
							label.TransformCoords = true;
							label.Location = new Vector2D(x, y);
							label.AlignX = TextAlignmentX.Center;
							label.AlignY = TextAlignmentY.Middle;
							label.BackColor = General.Colors.Background.WithAlpha(128);
							label.Text = value;
							label.Color = white;

							labels.Add(label);
						}
					}
				}
			}
		}

		// This updates the dragging
		private void Update()
		{
			PixelColor stitchcolor = General.Colors.Highlight;
			PixelColor losecolor = General.Colors.Selection;
			PixelColor color;

			snaptogrid = General.Interface.ShiftState ^ General.Interface.SnapToGrid;
			snaptonearest = General.Interface.CtrlState ^ General.Interface.AutoMerge;

			DrawnVertex lastp = new DrawnVertex();
			DrawnVertex curp = GetCurrentPosition();
			float vsize = ((float)renderer.VertexSize + 1.0f) / renderer.Scale;
			float vsizeborder = ((float)renderer.VertexSize + 3.0f) / renderer.Scale;

			SetupLabels();

			// Render drawing lines
			if (renderer.StartOverlay(true))
			{
				float size = 9 / renderer.Scale;

				if (BuilderPlug.Me.UseHighlight)
				{
					renderer.RenderHighlight(overlayGeometry, General.Colors.Selection.WithAlpha(64).ToInt());
				}

				List<SlopeVertex> vertices = new List<SlopeVertex>();

				// Store all slope vertices and draw the lines between them
				foreach (SlopeVertexGroup svg in BuilderPlug.Me.SlopeVertexGroups)
				{
					for (int i = 0; i < svg.Vertices.Count; i++)
					{
						vertices.Add(svg.Vertices[i]);

						if (i < svg.Vertices.Count - 1)
							renderer.RenderLine(svg.Vertices[0].Pos, svg.Vertices[i + 1].Pos, 1, new PixelColor(255, 255, 255, 255), true);
					}
				}

				// Sort the slope vertex list and draw them. The sorting ensures that selected vertices are always drawn on top
				foreach (SlopeVertex sv in vertices.OrderBy(o => o.Selected))
				{
					PixelColor c = General.Colors.Indication;
					Vector3D v = sv.Pos;

					renderer.RenderRectangleFilled(new RectangleF((float)(v.x - size / 2), (float)(v.y - size / 2), size, size), General.Colors.Background, true);
					renderer.RenderRectangle(new RectangleF((float)(v.x - size / 2), (float)(v.y - size / 2), size, size), 2, c, true);
				}


				// Go for all points to draw lines
				if (points.Count > 0)
				{
					// Render lines
					lastp = points[0];
					for (int i = 1; i < points.Count; i++)
					{
						// Determine line color
						if (lastp.stitchline && points[i].stitchline) color = stitchcolor;
						else color = losecolor;

						// Render line
						renderer.RenderLine(points[0].pos, points[i].pos, LINE_THICKNESS, color, true);
						lastp = points[i];
					}

					// Determine line color
					if (lastp.stitchline && snaptonearest) color = stitchcolor;
					else color = losecolor;

					// Render line to cursor
					renderer.RenderLine(points[0].pos, curp.pos, LINE_THICKNESS, color, true);

					// Render vertices
					for (int i = 0; i < points.Count; i++)
					{
						// Determine vertex color
						if (points[i].stitch) color = stitchcolor;
						else color = losecolor;

						// Render vertex
						//renderer.RenderRectangleFilled(new RectangleF(points[i].pos.x - vsize, points[i].pos.y - vsize, vsize * 2.0f, vsize * 2.0f), color, true);
						renderer.RenderRectangleFilled(new RectangleF((float)(points[i].pos.x - size / 2), (float)(points[i].pos.y - size / 2), size, size), General.Colors.Background, true);
						renderer.RenderRectangle(new RectangleF((float)(points[i].pos.x - size / 2), (float)(points[i].pos.y - size / 2), size, size), 2, General.Colors.Indication, true);
					}
				}

				foreach (TextLabel l in labels)
					renderer.RenderText(l);

				// Determine point color
				if (snaptonearest) color = stitchcolor;
				else color = losecolor;

				// Render vertex at cursor
				//renderer.RenderRectangleFilled(new RectangleF(curp.pos.x - vsize, curp.pos.y - vsize, vsize * 2.0f, vsize * 2.0f), color, true);

				renderer.RenderRectangleFilled(new RectangleF((float)(curp.pos.x - size / 2), (float)(curp.pos.y - size / 2), size, size), General.Colors.Background, true);
				renderer.RenderRectangle(new RectangleF((float)(curp.pos.x - size / 2), (float)(curp.pos.y - size / 2), size, size), 2, General.Colors.Indication, true);

				// Done
				renderer.Finish();
			}

			// Done
			renderer.Present();
		}

		private void updateOverlaySurfaces()
		{
			ICollection<Sector> orderedselection = General.Map.Map.GetSelectedSectors(true);
			List<FlatVertex> vertsList = new List<FlatVertex>();

			// Go for all selected sectors
			foreach (Sector s in orderedselection) vertsList.AddRange(s.FlatVertices);
			overlayGeometry = vertsList.ToArray();
		}

		// This returns the aligned and snapped draw position
		public static DrawnVertex GetCurrentPosition(Vector2D mousemappos, bool snaptonearest, bool snaptogrid, IRenderer2D renderer, List<DrawnVertex> points)
		{
			DrawnVertex p = new DrawnVertex();
			Vector2D vm = mousemappos;
			float vrange = BuilderPlug.Me.StitchRange / renderer.Scale;

			// Snap to nearest?
			if (snaptonearest)
			{
				// Go for all drawn points
				foreach (DrawnVertex v in points)
				{
					if (Vector2D.DistanceSq(mousemappos, v.pos) < (vrange * vrange))
					{
						p.pos = v.pos;
						p.stitch = true;
						p.stitchline = true;
						return p;
					}
				}

				// Try the nearest vertex
				Vertex nv = General.Map.Map.NearestVertexSquareRange(mousemappos, vrange);
				if (nv != null)
				{
					p.pos = nv.Position;
					p.stitch = true;
					p.stitchline = true;
					return p;
				}

				// Try the nearest linedef
				Linedef nl = General.Map.Map.NearestLinedefRange(mousemappos, BuilderPlug.Me.StitchRange / renderer.Scale);
				if (nl != null)
				{
					// Snap to grid?
					if (snaptogrid)
					{
						// Get grid intersection coordinates
						List<Vector2D> coords = nl.GetGridIntersections();

						// Find nearest grid intersection
						bool found = false;
						double found_distance = float.MaxValue;
						Vector2D found_coord = new Vector2D();
						foreach (Vector2D v in coords)
						{
							Vector2D delta = mousemappos - v;
							if (delta.GetLengthSq() < found_distance)
							{
								found_distance = delta.GetLengthSq();
								found_coord = v;
								found = true;
							}
						}

						if (found)
						{
							// Align to the closest grid intersection
							p.pos = found_coord;
							p.stitch = true;
							p.stitchline = true;
							return p;
						}
					}
					else
					{
						// Aligned to line
						p.pos = nl.NearestOnLine(mousemappos);
						p.stitch = true;
						p.stitchline = true;
						return p;
					}
				}
			}
			else
			{
				// Always snap to the first drawn vertex so that the user can finish a complete sector without stitching
				if (points.Count > 0)
				{
					if (Vector2D.DistanceSq(mousemappos, points[0].pos) < (vrange * vrange))
					{
						p.pos = points[0].pos;
						p.stitch = true;
						p.stitchline = false;
						return p;
					}
				}
			}

			// if the mouse cursor is outside the map bondaries check if the line between the last set point and the
			// mouse cursor intersect any of the boundary lines. If it does, set the position to this intersection
			if (points.Count > 0 &&
				(mousemappos.x < General.Map.Config.LeftBoundary || mousemappos.x > General.Map.Config.RightBoundary ||
				mousemappos.y > General.Map.Config.TopBoundary || mousemappos.y < General.Map.Config.BottomBoundary))
			{
				Line2D dline = new Line2D(mousemappos, points[points.Count - 1].pos);
				bool foundintersection = false;
				double u = 0.0f;
				List<Line2D> blines = new List<Line2D>();

				// lines for left, top, right and bottom bondaries
				blines.Add(new Line2D(General.Map.Config.LeftBoundary, General.Map.Config.BottomBoundary, General.Map.Config.LeftBoundary, General.Map.Config.TopBoundary));
				blines.Add(new Line2D(General.Map.Config.LeftBoundary, General.Map.Config.TopBoundary, General.Map.Config.RightBoundary, General.Map.Config.TopBoundary));
				blines.Add(new Line2D(General.Map.Config.RightBoundary, General.Map.Config.TopBoundary, General.Map.Config.RightBoundary, General.Map.Config.BottomBoundary));
				blines.Add(new Line2D(General.Map.Config.RightBoundary, General.Map.Config.BottomBoundary, General.Map.Config.LeftBoundary, General.Map.Config.BottomBoundary));

				// check for intersections with boundaries
				for (int i = 0; i < blines.Count; i++)
				{
					if (!foundintersection)
					{
						// only check for intersection if the last set point is not on the
						// line we are checking against
						if (blines[i].GetSideOfLine(points[points.Count - 1].pos) != 0.0f)
						{
							foundintersection = blines[i].GetIntersection(dline, out u);
						}
					}
				}

				// if there was no intersection set the position to the last set point
				if (!foundintersection)
					vm = points[points.Count - 1].pos;
				else
					vm = dline.GetCoordinatesAt(u);

			}


			// Snap to grid?
			if (snaptogrid)
			{
				// Aligned to grid
				p.pos = General.Map.Grid.SnappedToGrid(vm);

				// special handling 
				if (p.pos.x > General.Map.Config.RightBoundary) p.pos.x = General.Map.Config.RightBoundary;
				if (p.pos.y < General.Map.Config.BottomBoundary) p.pos.y = General.Map.Config.BottomBoundary;
				p.stitch = snaptonearest;
				p.stitchline = snaptonearest;
				return p;
			}
			else
			{
				// Normal position
				p.pos = vm;
				p.stitch = snaptonearest;
				p.stitchline = snaptonearest;
				return p;
			}
		}

		// This gets the aligned and snapped draw position
		private DrawnVertex GetCurrentPosition()
		{
			return GetCurrentPosition(mousemappos, snaptonearest, snaptogrid, renderer, points);
		}

		// This draws a point at a specific location
		public bool DrawPointAt(DrawnVertex p)
		{
			return DrawPointAt(p.pos, p.stitch, p.stitchline);
		}

		// This draws a point at a specific location
		public bool DrawPointAt(Vector2D pos, bool stitch, bool stitchline)
		{
			if (pos.x < General.Map.Config.LeftBoundary || pos.x > General.Map.Config.RightBoundary ||
				pos.y > General.Map.Config.TopBoundary || pos.y < General.Map.Config.BottomBoundary)
				return false;

			DrawnVertex newpoint = new DrawnVertex();
			newpoint.pos = pos;
			newpoint.stitch = stitch;
			newpoint.stitchline = stitchline;
			points.Add(newpoint);
			updateOverlaySurfaces();
			Update();

			if (points.Count == 3)
				FinishDraw();

			return true;
		}

		private bool IsControlSector(Sector s)
		{
			//Determine whether or not the sector is actually a control sector for a 3D floor
			foreach (Sidedef sd in s.Sidedefs)
			{
				if (sd.Line.Action == 160)
					return true;
			}
			return false;
		}

		#endregion

		#region ================== Events

		public override void OnHelp()
		{
			General.ShowHelp("gzdb/features/classic_modes/mode_slopes.html");
		}

		// Engaging
		public override void OnEngage()
		{
			base.OnEngage();

			if (BuilderPlug.Me.SlopeDataSector == null || BuilderPlug.Me.SlopeDataSector.IsDisposed)
			{
				General.Map.UndoRedo.CreateUndo("Set up slope data sector");

				SlopeDataSectorDialog sdsd = new SlopeDataSectorDialog();
				DialogResult dr = sdsd.ShowDialog();

				if (dr == DialogResult.Cancel)
				{
					General.Map.UndoRedo.WithdrawUndo();
					General.Editing.CancelMode();
					return;
				}

				if (dr == DialogResult.OK)
				{
					BuilderPlug.Me.SlopeDataSector = General.Map.Map.GetMarkedSectors(true)[0];
					BuilderPlug.Me.StoreSlopeVertexGroupsInSector();
				}
			}
			else
			{
				BuilderPlug.Me.LoadSlopeVertexGroupsFromSector();
			}

			EnableAutoPanning();
			renderer.SetPresentation(Presentation.Standard);

			// Convert geometry selection to sectors only
			General.Map.Map.ConvertSelection(SelectionType.Sectors);

			General.Interface.AddButton(BuilderPlug.Me.MenusForm.FloorSlope);
			General.Interface.AddButton(BuilderPlug.Me.MenusForm.CeilingSlope);
			General.Interface.AddButton(BuilderPlug.Me.MenusForm.FloorAndCeilingSlope);

			BuilderPlug.Me.MenusForm.FloorSlope.Checked = false;
			BuilderPlug.Me.MenusForm.CeilingSlope.Checked = false;
			BuilderPlug.Me.MenusForm.FloorAndCeilingSlope.Checked = false;

			if (slopedrawingmode == SlopeDrawingMode.Floor)
				BuilderPlug.Me.MenusForm.FloorSlope.Checked = true;
			else if (slopedrawingmode == SlopeDrawingMode.Ceiling)
				BuilderPlug.Me.MenusForm.CeilingSlope.Checked = true;
			else
				BuilderPlug.Me.MenusForm.FloorAndCeilingSlope.Checked = true;

			// Make text labels for sectors
			SetupLabels();
			updateOverlaySurfaces();
			Update();

			// Set cursor
			General.Interface.SetCursor(Cursors.Cross);
		}

		// Disengaging
		public override void OnDisengage()
		{
			base.OnDisengage();

			General.Interface.RemoveButton(BuilderPlug.Me.MenusForm.FloorSlope);
			General.Interface.RemoveButton(BuilderPlug.Me.MenusForm.CeilingSlope);
			General.Interface.RemoveButton(BuilderPlug.Me.MenusForm.FloorAndCeilingSlope);

			DisableAutoPanning();
		}

		// Cancelled
		public override void OnCancel()
		{
			// Cancel base class
			base.OnCancel();

			// Return to previous stable mode
			General.Editing.ChangeMode(General.Editing.PreviousStableMode.Name);
		}

		// Accepted
		public override void OnAccept()
		{
			Cursor.Current = Cursors.AppStarting;

			General.Settings.FindDefaultDrawSettings();

			// When points have been drawn
			if (points.Count > 1)
			{
				// Make undo for the draw
				General.Map.UndoRedo.CreateUndo("Draw slope");

				List<SlopeVertex> sv_floor = new List<SlopeVertex>();
				List<SlopeVertex> sv_ceiling = new List<SlopeVertex>();

				// Fills the slope vertex list for both floor and ceiling slopes. Alos tried
				// to determine the default z position of the vertex
				List<Sector> selected = (List<Sector>)General.Map.Map.GetSelectedSectors(true);
				if (selected.Count == 1 && IsControlSector(selected[0]))
				{
					//If a 3D floor control sector is selected, then just use the height of it directly
					float zf = selected[0].FloorHeight;
					float zc = selected[0].CeilHeight;
					for (int i = 0; i < points.Count; i++)
					{
						sv_floor.Add(new SlopeVertex(points[i].pos, zf));
						sv_ceiling.Add(new SlopeVertex(points[i].pos, zc));
					}
				} else {
					//For normal sectors, grab the height of the sector each control handle lies within
					for (int i = 0; i < points.Count; i++)
					{
						float zf = 0;
						float zc = 0;
						Sector s = General.Map.Map.GetSectorByCoordinates(points[i].pos);

						if (s != null)
						{
							foreach (Sidedef sd in s.Sidedefs)
							{
								if (sd.Line.Line.GetSideOfLine(points[i].pos) == 0)
								{
									if (sd.Line.Back != null && !selected.Contains(sd.Line.Back.Sector))
									{
										zf = sd.Line.Back.Sector.FloorHeight;
										zc = sd.Line.Back.Sector.CeilHeight;
									}
									else
									{
										zf = sd.Line.Front.Sector.FloorHeight;
										zc = sd.Line.Front.Sector.CeilHeight;
									}
								}
							}
						}

						sv_floor.Add(new SlopeVertex(points[i].pos, zf));
						sv_ceiling.Add(new SlopeVertex(points[i].pos, zc));
					}
				}

				// Create the floor slope vertex group and add it to all selected sectors
				if (slopedrawingmode == SlopeDrawingMode.Floor || slopedrawingmode == SlopeDrawingMode.FloorAndCeiling)
				{
					int id = -1;
					SlopeVertexGroup svg = BuilderPlug.Me.AddSlopeVertexGroup(sv_floor, out id);

					svg.Sectors.Clear();

					foreach (Sector s in selected)
					{
						// Make sure the field work with undo/redo
						s.Fields.BeforeFieldsChange();

						if (s.Fields.ContainsKey("user_floorplane_id"))
							s.Fields.Remove("user_floorplane_id");

						s.Fields.Add("user_floorplane_id", new UniValue(UniversalType.Integer, id));

						// svg.Sectors.Add(s);

						svg.AddSector(s, PlaneType.Floor);
					}
				}

				// Create the ceiling slope vertex group and add it to all selected sectors
				if (slopedrawingmode == SlopeDrawingMode.Ceiling || slopedrawingmode == SlopeDrawingMode.FloorAndCeiling)
				{
					int id = -1;
					SlopeVertexGroup svg = BuilderPlug.Me.AddSlopeVertexGroup(sv_ceiling, out id);

					svg.Sectors.Clear();

					foreach (Sector s in selected)
					{
						// Make sure the field work with undo/redo
						s.Fields.BeforeFieldsChange();

						if (s.Fields.ContainsKey("user_ceilingplane_id"))
							s.Fields.Remove("user_ceilingplane_id");

						s.Fields.Add("user_ceilingplane_id", new UniValue(UniversalType.Integer, id));

						// svg.Sectors.Add(s);
						svg.AddSector(s, PlaneType.Ceiling);
					}
				}

				BuilderPlug.Me.StoreSlopeVertexGroupsInSector();

				// BuilderPlug.Me.UpdateSlopes();

				// Clear selection
				General.Map.Map.ClearAllSelected();

				// Update cached values
				General.Map.Map.Update();

				// Map is changed
				General.Map.IsChanged = true;
			}

			// Done
			Cursor.Current = Cursors.Default;

			// Return to original mode
			General.Editing.ChangeMode(General.Editing.PreviousStableMode.Name);
		}

		// This redraws the display
		public override void OnRedrawDisplay()
		{
			renderer.RedrawSurface();

			// Render lines
			if (renderer.StartPlotter(true))
			{
				renderer.PlotLinedefSet(General.Map.Map.Linedefs);
				renderer.PlotVerticesSet(General.Map.Map.Vertices);
				renderer.Finish();
			}

			// Render things
			if (renderer.StartThings(true))
			{
				renderer.RenderThingSet(General.Map.Map.Things, 1.0f);
				renderer.Finish();
			}

			// Normal update
			updateOverlaySurfaces();
			Update();
		}

		// Mouse moving
		public override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			updateOverlaySurfaces();
			Update();
		}

		// When a key is released
		public override void OnKeyUp(KeyEventArgs e)
		{
			base.OnKeyUp(e);
			if ((snaptogrid != (General.Interface.ShiftState ^ General.Interface.SnapToGrid)) ||
			   (snaptonearest != (General.Interface.CtrlState ^ General.Interface.AutoMerge))) Update();
		}

		// When a key is pressed
		public override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);
			if ((snaptogrid != (General.Interface.ShiftState ^ General.Interface.SnapToGrid)) ||
			   (snaptonearest != (General.Interface.CtrlState ^ General.Interface.AutoMerge))) Update();
		}

		#endregion

		#region ================== Actions

		[BeginAction("drawfloorslope")]
		public void DrawFloorSlope()
		{
			slopedrawingmode = SlopeDrawingMode.Floor;

			BuilderPlug.Me.MenusForm.CeilingSlope.Checked = false;
			BuilderPlug.Me.MenusForm.FloorSlope.Checked = true;
			BuilderPlug.Me.MenusForm.FloorAndCeilingSlope.Checked = false;

			General.Interface.DisplayStatus(StatusType.Info, "Applying drawn slope to floor");
		}

		[BeginAction("drawceilingslope")]
		public void DrawCeilingSlope()
		{
			slopedrawingmode = SlopeDrawingMode.Ceiling;

			BuilderPlug.Me.MenusForm.CeilingSlope.Checked = true;
			BuilderPlug.Me.MenusForm.FloorSlope.Checked = false;
			BuilderPlug.Me.MenusForm.FloorAndCeilingSlope.Checked = false;

			General.Interface.DisplayStatus(StatusType.Info, "Applying drawn slope to ceiling");
		}

		[BeginAction("drawfloorandceilingslope")]
		public void DrawFloorAndCeilingSlope()
		{
			slopedrawingmode = SlopeDrawingMode.FloorAndCeiling;

			BuilderPlug.Me.MenusForm.CeilingSlope.Checked = false;
			BuilderPlug.Me.MenusForm.FloorSlope.Checked = false;
			BuilderPlug.Me.MenusForm.FloorAndCeilingSlope.Checked = true;

			General.Interface.DisplayStatus(StatusType.Info, "Applying drawn slope to floor and ceiling");
		}

		// Drawing a point
		[BeginAction("drawslopepoint")]
		public void DrawPoint()
		{
			// Mouse inside window?
			if (General.Interface.MouseInDisplay)
			{
				DrawnVertex newpoint = GetCurrentPosition();

				if (!DrawPointAt(newpoint)) General.Interface.DisplayStatus(StatusType.Warning, "Failed to draw point: outside of map boundaries.");
			}
		}

		// Remove a point
		//[BeginAction("removepoint")]
		public void RemovePoint()
		{
			if (points.Count > 0) points.RemoveAt(points.Count - 1);

			updateOverlaySurfaces();
			Update();
		}

		// Finish drawing
		[BeginAction("finishslopedraw")]
		public void FinishDraw()
		{
			// Accept the changes
			General.Editing.AcceptMode();
		}

		#endregion
	}
}
