
#region ================== Copyright (c) 2007 Pascal vd Heiden

/*
 * Copyright (c) 2007 Pascal vd Heiden, www.codeimp.com
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

#region ================== Namespaces

using System;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using CodeImp.DoomBuilder.Windows;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Editing;
using CodeImp.DoomBuilder.Actions;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.BuilderModes;
// using CodeImp.DoomBuilder.GZBuilder.Geometry;

#endregion

namespace CodeImp.DoomBuilder.ThreeDFloorMode
{
    public class SlopeObject
    {
        private ThreeDFloor threedfloor;
        private Vector2D position;
		private int v;

        public ThreeDFloor ThreeDFloor { get { return threedfloor; } set { threedfloor = value; } }
        public Vector2D Position { get { return position; } set { position = value; } }
		public int V { get { return v; } set { v = value; } }
    }

    [EditMode(DisplayName = "Slope Mode",
              SwitchAction = "threedslopemode",		// Action name used to switch to this mode
              ButtonImage = "SlopeModeIcon.png",	// Image resource name for the button
              ButtonOrder = int.MinValue + 501,	// Position of the button (lower is more to the left)
              ButtonGroup = "000_editing",
			  SupportedMapFormats = new[] { "UniversalMapSetIO" },
			  RequiredMapFeatures = new[] { "PlaneEquationSupport" },
			  UseByDefault = true,
              IsDeprecated = true,
              DeprecationMessage = "Please use the visual sloping functionality instead.")]

    public class SlopeMode : ClassicMode
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		// Highlighted item
        private SlopeVertex highlightedslope;
		private Sector highlightedsector;
		private Association[] association = new Association[Thing.NUM_ARGS];
		private List<SlopeVertexGroup> copyslopevertexgroups;

        private List<ThreeDFloor> threedfloors;
		bool dragging = false;

		private List<TextLabel> labels;
		private FlatVertex[] overlaygeometry;
		private FlatVertex[] overlaytaggedgeometry;
		private FlatVertex[] selectedsectorgeometry;

		private Vector2D dragstartmappos;
		private List<Vector2D> oldpositions;

		private bool contextmenuclosing = false;
		
		#endregion

		#region ================== Properties

		public Sector HighlightedSector { get { return highlightedsector; } }
		public bool ContextMenuClosing { get { return contextmenuclosing; } set { contextmenuclosing = value; } }

		#endregion

		#region ================== Constructor / Disposer

		#endregion

		#region ================== Methods

		public override void OnHelp()
		{
			General.ShowHelp("gzdb/features/classic_modes/mode_slopes.html");
		}

		// Cancel mode
		public override void OnCancel()
		{
			base.OnCancel();

			// Return to previous stable mode
			General.Editing.ChangeMode(General.Editing.PreviousStableMode.Name);
		}

		// Mode engages
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

            renderer.SetPresentation(Presentation.Things);

			General.Interface.AddButton(BuilderPlug.Me.MenusForm.UpdateSlopes);

            // Convert geometry selection to sectors
            General.Map.Map.ConvertSelection(SelectionType.Sectors);

            // Get all 3D floors in the map
            threedfloors = BuilderPlug.GetThreeDFloors(General.Map.Map.Sectors.ToList());

			SetupLabels();

			foreach (SlopeVertexGroup svg in BuilderPlug.Me.SlopeVertexGroups)
			{
				svg.FindSectors();
			}

			// Update overlay surfaces, so that selected sectors are drawn correctly
			updateOverlaySurfaces();
		}

		// Mode disengages
		public override void OnDisengage()
		{
			base.OnDisengage();

			General.Interface.RemoveButton(BuilderPlug.Me.MenusForm.UpdateSlopes);
			
			// Hide highlight info
			General.Interface.HideInfo();
		}

		// This redraws the display
		public override void OnRedrawDisplay()
		{
			renderer.RedrawSurface();

			// Render lines and vertices
			if(renderer.StartPlotter(true))
			{
				renderer.PlotLinedefSet(General.Map.Map.Linedefs);
				renderer.PlotVerticesSet(General.Map.Map.Vertices);

				foreach (Sector s in General.Map.Map.GetSelectedSectors(true).ToList())
					renderer.PlotSector(s, General.Colors.Selection);

				if ((highlightedsector != null) && !highlightedsector.IsDisposed)
					renderer.PlotSector(highlightedsector, General.Colors.Highlight);

				renderer.Finish();
			}

			// Render things
			if(renderer.StartThings(true))
			{
				renderer.RenderThingSet(General.Map.ThingsFilter.HiddenThings, Presentation.THINGS_HIDDEN_ALPHA);
				renderer.RenderThingSet(General.Map.ThingsFilter.VisibleThings, 1.0f);

				renderer.Finish();
			}

            UpdateOverlay();

			renderer.Present();
		}

		private void SetupLabels()
		{
			Dictionary<Sector, SectorLabelInfo> sectorlabels = new Dictionary<Sector, SectorLabelInfo>();
			PixelColor white = new PixelColor(255, 255, 255, 255);

			if (labels != null)
			{
				// Dispose old labels
				foreach (TextLabel l in labels)
					l.Dispose();

				labels.Clear();
			}
			else
			{
				labels = new List<TextLabel>();
			}

			// Go through all sectors that belong to a SVG and set which SVG their floor and
			// ceiling belongs to
			foreach (SlopeVertexGroup svg in BuilderPlug.Me.SlopeVertexGroups)
			{
				// Process all directly affected sectors
				foreach (Sector s in svg.Sectors)
				{
					if(!sectorlabels.ContainsKey(s))
						sectorlabels.Add(s, new SectorLabelInfo());

					if ((svg.SectorPlanes[s] & PlaneType.Floor) == PlaneType.Floor)
						sectorlabels[s].AddSlopeVertexGroup(PlaneType.Floor, svg);

					if ((svg.SectorPlanes[s] & PlaneType.Ceiling) == PlaneType.Ceiling)
						sectorlabels[s].AddSlopeVertexGroup(PlaneType.Ceiling, svg);
				}

				// Process all tagged sectors
				foreach(Sector s in svg.TaggedSectors)
				{
					if (!sectorlabels.ContainsKey(s))
						sectorlabels.Add(s, new SectorLabelInfo());

					// Bottom and Top are just virtual, the control sector has Floor and Ceiling
					if ((svg.SectorPlanes[s] & PlaneType.Floor) == PlaneType.Floor)
						sectorlabels[s].AddSlopeVertexGroup(PlaneType.Bottom, svg);

					if ((svg.SectorPlanes[s] & PlaneType.Ceiling) == PlaneType.Ceiling)
						sectorlabels[s].AddSlopeVertexGroup(PlaneType.Top, svg);
				}
			}

			// Create the labels for each sector and add them to the label list
			if (BuilderPlug.Me.SectorLabelDisplayOption != LabelDisplayOption.Never || General.Interface.AltState == true)
			{
				foreach (Sector s in sectorlabels.Keys)
				{
					bool showlabel = true;

					if (BuilderPlug.Me.SectorLabelDisplayOption == LabelDisplayOption.WhenHighlighted && General.Interface.AltState == false)
					{
						showlabel = false;
						foreach (SlopeVertexGroup svg in BuilderPlug.Me.SlopeVertexGroups)
							if ((svg.Sectors.Contains(s) || svg.TaggedSectors.Contains(s)) && svg.Vertices.Contains(highlightedslope))
								showlabel = true;
					}

					if(showlabel)
						labels.AddRange(sectorlabels[s].CreateLabels(s, highlightedslope, renderer.Scale));
				}
			}

			// Z position labels for slope vertices
			if (BuilderPlug.Me.SlopeVertexLabelDisplayOption != LabelDisplayOption.Never || General.Interface.AltState == true)
			{
				foreach (SlopeVertexGroup svg in BuilderPlug.Me.SlopeVertexGroups)
				{
					for (int i = 0; i < svg.Vertices.Count; i++)
					{
						if (BuilderPlug.Me.SlopeVertexLabelDisplayOption == LabelDisplayOption.Always || General.Interface.AltState == true || svg.Vertices.Contains(highlightedslope))
						{
							SlopeVertex sv = svg.Vertices[i];
							float scale = 1 / renderer.Scale;
							double x = sv.Pos.x;
							double y = sv.Pos.y - 14 * scale;
							string value = String.Format("Z: {0}", sv.Z);
							bool showlabel = true;

							// Rearrange labels if they'd be (exactly) on each other
							// TODO: do something like that also for overlapping labels
							foreach (TextLabel l in labels)
							{
								if (l.Location.x == x && l.Location.y == y) {
									// Reduce visual clutter by de-duping stacked labels, when "show all labels" is enabled
									if (l.Text == value) {
										showlabel = false; //dedupe

										// If any of the shared label lines are highlighted/selected, then override the color
										if (svg.Vertices.Contains(highlightedslope))
											l.Color = General.Colors.Highlight.WithAlpha(255);
										else if (sv.Selected)
											l.Color = General.Colors.Selection.WithAlpha(255);
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

								if (svg.Vertices.Contains(highlightedslope))
									label.Color = General.Colors.Highlight.WithAlpha(255);
								else if (sv.Selected)
									label.Color = General.Colors.Selection.WithAlpha(255);
								else
									label.Color = white;

								labels.Add(label);
							}
						}
					}
				}
			}
		}

		// This updates the overlay
        private void UpdateOverlay()
        {
            float size = 9 / renderer.Scale;

			SetupLabels();

            if (renderer.StartOverlay(true))
            {
				if(overlaygeometry != null)
					renderer.RenderHighlight(overlaygeometry, General.Colors.ModelWireframe.WithAlpha(64).ToInt());

				if (overlaytaggedgeometry != null)
					renderer.RenderHighlight(overlaytaggedgeometry, General.Colors.Vertices.WithAlpha(64).ToInt());

				if (selectedsectorgeometry != null)
					renderer.RenderHighlight(selectedsectorgeometry, General.Colors.Selection.WithAlpha(64).ToInt());

				if (BuilderPlug.Me.UseHighlight && highlightedsector != null)
				{
					renderer.RenderHighlight(highlightedsector.FlatVertices, General.Colors.Highlight.WithAlpha(64).ToInt());
				}

				List<SlopeVertex> vertices = new List<SlopeVertex>();
				List<Line2D> highlightlines = new List<Line2D>();

				// TMP
				foreach(Line3D l in BuilderPlug.Me.drawlines)
				{
					renderer.RenderLine(
						new Vector2D(l.Start.x, l.Start.z),
						new Vector2D(l.End.x, l.End.z),
						1, new PixelColor(255, 255, 255, 255), true
						);
				}

				foreach(Vector3D v in BuilderPlug.Me.drawpoints)
				{
					renderer.RenderLine(
						new Vector2D(v.x, v.z+2),
						new Vector2D(v.x, v.z-2),
						1, new PixelColor(255, 255, 0, 0), true);
				}

				// Store all slope vertices and draw the lines between them. If the lines connect highlighted slope vertices
				// draw them later, so they are on top
				foreach (SlopeVertexGroup svg in BuilderPlug.Me.SlopeVertexGroups)
				{
					bool highlighted = svg.Vertices.Where(o => o == highlightedslope).Count() > 0;

					for (int i = 0; i < svg.Vertices.Count; i++)
					{
						vertices.Add(svg.Vertices[i]);

						if (i < svg.Vertices.Count - 1)
						{
							if (highlighted)
								highlightlines.Add(new Line2D(svg.Vertices[0].Pos, svg.Vertices[i + 1].Pos));
							else
								renderer.RenderLine(svg.Vertices[0].Pos, svg.Vertices[i + 1].Pos, 1, new PixelColor(255, 255, 255, 255), true);
						}
					}
				}

				// Draw highlighted lines
				foreach (Line2D line in highlightlines)
					renderer.RenderLine(line.v1, line.v2, 1, General.Colors.Highlight, true);

				// Sort the slope vertex list and draw them. The sorting ensures that selected vertices are always drawn on top
				foreach(SlopeVertex sv in vertices.OrderBy(o=>o.Selected))
				{
					PixelColor c = General.Colors.Indication;
					Vector3D v = sv.Pos;

					if (sv.Selected)
						c = General.Colors.Selection;

					renderer.RenderRectangleFilled(new RectangleF((float)(v.x - size / 2), (float)(v.y - size / 2), size, size), General.Colors.Background, true);
					renderer.RenderRectangle(new RectangleF((float)(v.x - size / 2), (float)(v.y - size / 2), size, size), 2, c, true);
				}

				// Draw highlighted slope vertex
				if (highlightedslope != null)
				{
					renderer.RenderRectangleFilled(new RectangleF((float)(highlightedslope.Pos.x - size / 2), (float)(highlightedslope.Pos.y - size / 2), size, size), General.Colors.Background, true);
					renderer.RenderRectangle(new RectangleF((float)(highlightedslope.Pos.x - size / 2), (float)(highlightedslope.Pos.y - size / 2), size, size), 2, General.Colors.Highlight, true);
				}

				foreach (TextLabel l in labels)
					renderer.RenderText(l);

				if (selecting)
					RenderMultiSelection();

                renderer.Finish();
            }           
        }

		private void updateOverlaySurfaces()
		{
			string[] fieldnames = new string[] { "user_floorplane_id", "user_ceilingplane_id" };
			ICollection<Sector> orderedselection = General.Map.Map.GetSelectedSectors(true);
			List<FlatVertex> vertslist = new List<FlatVertex>();
			List<Sector> highlightedsectors = new List<Sector>();
			List<Sector> highlightedtaggedsectors = new List<Sector>();

			// Highlighted slope
			if (highlightedslope != null)
			{
				SlopeVertexGroup svg = BuilderPlug.Me.GetSlopeVertexGroup(highlightedslope);

				// All sectors the slope applies to
				foreach (Sector s in svg.Sectors)
				{
					if (s != null && !s.IsDisposed)
					{
						vertslist.AddRange(s.FlatVertices);
						highlightedsectors.Add(s);
					}
				}

				overlaygeometry = vertslist.ToArray();

				// All sectors that are tagged because of 3D floors
				vertslist = new List<FlatVertex>();

				foreach (Sector s in svg.TaggedSectors)
				{
					if (s != null && !s.IsDisposed)
					{
						vertslist.AddRange(s.FlatVertices);
						highlightedtaggedsectors.Add(s);
					}
				}

				overlaytaggedgeometry = vertslist.ToArray();
			}
			else
			{
				overlaygeometry = new FlatVertex[0];
				overlaytaggedgeometry = new FlatVertex[0];
			}

			// Selected sectors
			vertslist = new List<FlatVertex>();

			foreach (Sector s in orderedselection)
				if(!highlightedsectors.Contains(s))
					vertslist.AddRange(s.FlatVertices);

			selectedsectorgeometry = vertslist.ToArray();
		}

		// This highlights a new item
		protected void HighlightSector(Sector s)
		{
			// Update display

			highlightedsector = s;
			/*
			if (renderer.StartPlotter(false))
			{
				// Undraw previous highlight
				if ((highlightedsector != null) && !highlightedsector.IsDisposed)
					renderer.PlotSector(highlightedsector);

				// Set new highlight
				highlightedsector = s;

				// Render highlighted item
				if ((highlightedsector != null) && !highlightedsector.IsDisposed)
					renderer.PlotSector(highlightedsector, General.Colors.Highlight);

				// Done
				renderer.Finish();
			}

			UpdateOverlay();
			renderer.Present();
			*/
			General.Interface.RedrawDisplay();

			// Show highlight info
			if ((highlightedsector != null) && !highlightedsector.IsDisposed)
				General.Interface.ShowSectorInfo(highlightedsector);
			else
				General.Interface.HideInfo();
		}

		// This selectes or deselects a sector
		protected void SelectSector(Sector s, bool selectstate)
		{
			bool selectionchanged = false;

			if (!s.IsDisposed)
			{
				// Select the sector?
				if (selectstate && !s.Selected)
				{
					s.Selected = true;
					selectionchanged = true;
				}
				// Deselect the sector?
				else if (!selectstate && s.Selected)
				{
					s.Selected = false;
					selectionchanged = true;
				}

				// Selection changed?
				if (selectionchanged)
				{
					// Make update lines selection
					foreach (Sidedef sd in s.Sidedefs)
					{
						bool front, back;
						if (sd.Line.Front != null) front = sd.Line.Front.Sector.Selected; else front = false;
						if (sd.Line.Back != null) back = sd.Line.Back.Sector.Selected; else back = false;
						sd.Line.Selected = front | back;
					}

					//mxd. Also (de)select things?
					if (General.Interface.AltState)
					{
						foreach (Thing t in General.Map.ThingsFilter.VisibleThings)
						{
							t.DetermineSector();
							if (t.Sector != s) continue;
							t.Selected = s.Selected;
						}
					}
				}
			}
		}

		public void ResetHighlightedSector()
		{
			HighlightSector(null);
		}
		
		// Selection
		protected override void OnSelectBegin()
		{
			// Item highlighted?
			if(highlightedslope != null)
			{
				// Flip selection
				highlightedslope.Selected = !highlightedslope.Selected;

				updateOverlaySurfaces();
				UpdateOverlay();
			}

			base.OnSelectBegin();
		}

		// End selection
		protected override void OnSelectEnd()
		{
			// Not ending from a multi-selection?
			if(!selecting)
			{
				// Item highlighted?
				if (highlightedslope != null)
				{
					updateOverlaySurfaces();
					UpdateOverlay();
				}

				if (highlightedsector != null)
				{
					if (!contextmenuclosing)
					{
						SelectSector(highlightedsector, !highlightedsector.Selected);

						updateOverlaySurfaces();
						General.Interface.RedrawDisplay();
					}
				}

				contextmenuclosing = false;
			}

			base.OnSelectEnd();
		}

		// Done editing
		protected override void OnEditEnd()
		{
			base.OnEditEnd();

			if (dragging) return;

			if (highlightedslope != null)
			{
				SlopeVertex sv = highlightedslope;

				List<SlopeVertex> vertices = GetSelectedSlopeVertices();

				if (!vertices.Contains(highlightedslope))
					vertices.Add(highlightedslope);

				SlopeVertexEditForm svef = new SlopeVertexEditForm();
				svef.Setup(vertices);

				DialogResult result = svef.ShowDialog((Form)General.Interface);

				if (result == DialogResult.OK)
				{
					General.Map.IsChanged = true;

					BuilderPlug.Me.UpdateSlopes();
				}

				highlightedslope = null;
			}
			else if(highlightedsector != null)
			{
				if (General.Map.Map.SelectedSectorsCount == 0)
				{
					BuilderPlug.Me.MenusForm.AddSectorsContextMenu.Tag = new List<Sector>() { highlightedsector };
				}
				else
				{
					BuilderPlug.Me.MenusForm.AddSectorsContextMenu.Tag = General.Map.Map.GetSelectedSectors(true).ToList();
				}

				BuilderPlug.Me.MenusForm.AddSectorsContextMenu.Show(Cursor.Position);
			}

			updateOverlaySurfaces();
			UpdateOverlay();

			General.Interface.RedrawDisplay();
		}

		//Build a list of the closest svs, that share the same distance away from the mouse cursor
		private List<SlopeVertex> GetVertexStack()
		{
			List<SlopeVertex> stack = new List<SlopeVertex>();
			double d, last = double.MaxValue;

			foreach(SlopeVertexGroup svg in BuilderPlug.Me.SlopeVertexGroups) {
				foreach(SlopeVertex sv in svg.Vertices)
				{
					d = Vector2D.Distance(sv.Pos, mousemappos);
					if (d <= BuilderModes.BuilderPlug.Me.HighlightRange / renderer.Scale) {
						if (d > last)
							continue; //discard
						else if (d < last)
							stack.Clear();

						stack.Add(sv);
						last = d;
					}
				}
			}

			return stack;
		}

		// Mouse moves
		public override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);

			if (selectpressed && !editpressed && !selecting)
			{
				// Check if moved enough pixels for multiselect
				Vector2D delta = mousedownpos - mousepos;
				if ((Math.Abs(delta.x) > 2) || (Math.Abs(delta.y) > 2))
				{
					// Start multiselecting
					StartMultiSelection();
				}
			}
			else if(e.Button == MouseButtons.None)
			{
				SlopeVertex oldhighlight = highlightedslope;
				Sector oldhighlightedsector = highlightedsector;

				//select the closest handle within grabbing distance
				List<SlopeVertex> stack = GetVertexStack();
				if (stack.Count > 0) {
					SlopeVertex sv = stack[0];
					if (sv != highlightedslope)
					{
						//if the "closest" handle is the same distance away as the already highlighted handle, then do nothing
						if (highlightedslope == null || (Vector2D.Distance(sv.Pos, mousemappos) != Vector2D.Distance(highlightedslope.Pos, mousemappos))) {
							highlightedslope = sv;
						}
					}
				} else {
					//nothing within distance, so reset the highlight
					highlightedslope = null;
				}

				// If no slope vertex is highlighted, check if a sector should be
				if (highlightedslope == null)
				{
					// Find the nearest linedef within highlight range
					Linedef l = General.Map.Map.NearestLinedef(mousemappos);
					if (l != null)
					{
						// Check on which side of the linedef the mouse is
						double side = l.SideOfLine(mousemappos);
						if (side > 0)
						{
							// Is there a sidedef here?
							if (l.Back != null)
							{
								// Highlight if not the same
								if (l.Back.Sector != highlightedsector) HighlightSector(l.Back.Sector);
							}
							else
							{
								// Highlight nothing
								if (highlightedsector != null) HighlightSector(null);
							}
						}
						else
						{
							// Is there a sidedef here?
							if (l.Front != null)
							{
								// Highlight if not the same
								if (l.Front.Sector != highlightedsector) HighlightSector(l.Front.Sector);
							}
							else
							{
								// Highlight nothing
								if (highlightedsector != null) HighlightSector(null);
							}
						}
					}
				}
				else
				{
					HighlightSector(null);
				}

				if (highlightedslope != oldhighlight)
				{
					updateOverlaySurfaces();
					UpdateOverlay();
					General.Interface.RedrawDisplay();
				}
			}
			else if (dragging && highlightedslope != null)
			{
				Vector2D newpos = SnapToNearest(mousemappos);
				Vector2D offset = highlightedslope.Pos - newpos;

				foreach (SlopeVertex sl in GetSelectedSlopeVertices())
					sl.Pos -= offset;
				
				highlightedslope.Pos = newpos;

				General.Map.IsChanged = true;

				updateOverlaySurfaces();
				UpdateOverlay();
				General.Interface.RedrawDisplay();
			}
			else if (selecting)
			{
				UpdateOverlay();
				General.Interface.RedrawDisplay();
			}
		}

		// Mouse leaves
		public override void OnMouseLeave(EventArgs e)
		{
			base.OnMouseLeave(e);

			// Highlight nothing
			highlightedslope = null;
		}

		public override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);

			if (e.Alt)
			{
				General.Interface.RedrawDisplay();
			}
		}

		public override void OnKeyUp(KeyEventArgs e)
		{
			base.OnKeyUp(e);

			if (!e.Alt)
			{
				General.Interface.RedrawDisplay();
			}
		}

		// Mouse wants to drag
		protected override void OnDragStart(MouseEventArgs e)
		{
			base.OnDragStart(e);

			if (e.Button == MouseButtons.Right)
			{
				dragging = true;
				dragstartmappos = mousemappos;

				oldpositions = new List<Vector2D>();

				foreach(SlopeVertex sl in GetSelectedSlopeVertices())
					if(sl.Selected)
						oldpositions.Add(sl.Pos);


				if(highlightedslope != null)
					oldpositions.Add(highlightedslope.Pos);
			}
		}
		//retrieves the current mouse position on the grid, snapped as necessary
		private Vector2D SnapToNearest(Vector2D vm)
		{
			double vrange = 20f / renderer.Scale;
			bool snaptogrid = General.Interface.ShiftState ^ General.Interface.SnapToGrid; //allow temporary disable of snap by holding shift

			if (General.Interface.AutoMerge) //only snap to geometry if the option is enabled
			{
				// Try the nearest slope vertex
				SlopeVertex nh = NearestSlopeVertexSquareRange(vm, vrange);
				if (nh != null)
					return nh.Pos;

				// Try the nearest map vertex
				Vertex nv = General.Map.Map.NearestVertexSquareRange(vm, vrange);
				if (nv != null)
					return nv.Position;

				// Try the nearest linedef
				Linedef nl = General.Map.Map.NearestLinedefRange(vm, vrange);
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
							Vector2D delta = vm - v;
							if (delta.GetLengthSq() < found_distance)
							{
								found_distance = delta.GetLengthSq();
								found_coord = v;
								found = true;
							}
						}

						if (found)
							return found_coord;
					}
					else
					{
						return nl.NearestOnLine(vm);
					}
				}
			}

			//Just get the current mouse location instead
			if (snaptogrid)
				return General.Map.Grid.SnappedToGrid(vm);
			return vm;
		}

		/// <summary>This finds the thing closest to the specified position.</summary>
		public SlopeVertex NearestSlopeVertexSquareRange(Vector2D pos, double maxrange)
		{
			List<SlopeVertex> verts = GetUnSelectedSlopeVertices();
			if (highlightedslope != null)
				verts.Remove(highlightedslope);

			return NearestSlopeVertexSquareRange(verts, pos, maxrange);
		}

		/// <summary>This finds the slope vertex closest to the specified position.</summary>
		public SlopeVertex NearestSlopeVertexSquareRange(ICollection<SlopeVertex> selection, Vector2D pos, double maxrange)
		{
			RectangleF range = RectangleF.FromLTRB((float)(pos.x - maxrange), (float)(pos.y - maxrange), (float)(pos.x + maxrange), (float)(pos.y + maxrange));
			SlopeVertex closest = null;
			double distance = double.MaxValue;

			// Go for all vertices in selection
			foreach (SlopeVertex v in selection)
			{
				double px = v.Pos.x;
				double py = v.Pos.y;

				//mxd. Within range?
				if ((v.Pos.x < range.Left) || (v.Pos.x > range.Right)
					|| (v.Pos.y < range.Top) || (v.Pos.y > range.Bottom))
					continue;

				// Close than previous find?
				double d = Math.Abs(px - pos.x) + Math.Abs(py - pos.y);
				if (d < distance)
				{
					// This one is closer
					closest = v;
					distance = d;
				}
			}

			// Return result
			return closest;
		}

		// Mouse wants to drag
		protected override void OnDragStop(MouseEventArgs e)
		{
			base.OnDragStop(e);

			General.Map.UndoRedo.CreateUndo("Drag slope vertex");

			BuilderPlug.Me.StoreSlopeVertexGroupsInSector();
			General.Map.Map.Update();

			BuilderPlug.Me.UpdateSlopes();

			dragging = false;
		}


		// This is called wheh selection ends
		protected override void OnEndMultiSelection()
		{
			bool selectionvolume = ((Math.Abs(base.selectionrect.Width) > 0.1f) && (Math.Abs(base.selectionrect.Height) > 0.1f));

			if(BuilderPlug.Me.AutoClearSelection && !selectionvolume)
				General.Map.Map.ClearSelectedThings();

			if(selectionvolume)
			{
				if(General.Interface.ShiftState ^ BuilderPlug.Me.AdditiveSelect)
				{
					// Go for all slope vertices
					foreach (SlopeVertex sl in GetAllSlopeVertices())
					{
						sl.Selected |= ((sl.Pos.x >= selectionrect.Left) &&
										(sl.Pos.y >= selectionrect.Top) &&
										(sl.Pos.x <= selectionrect.Right) &&
										(sl.Pos.y <= selectionrect.Bottom));
					}
				}
				else
				{
					// Go for all slope vertices
					foreach (SlopeVertex sl in GetAllSlopeVertices())
					{
						sl.Selected |= ((sl.Pos.x >= selectionrect.Left) &&
										(sl.Pos.y >= selectionrect.Top) &&
										(sl.Pos.x <= selectionrect.Right) &&
										(sl.Pos.y <= selectionrect.Bottom));
					}
				}
			}
			
			base.OnEndMultiSelection();

			// Clear overlay
			if(renderer.StartOverlay(true)) renderer.Finish();

			// Redraw
			General.Interface.RedrawDisplay();
		}

		// This is called when the selection is updated
		protected override void OnUpdateMultiSelection()
		{
			base.OnUpdateMultiSelection();

			UpdateOverlay();
		}

		public override bool OnCopyBegin()
		{
			copyslopevertexgroups = new List<SlopeVertexGroup>();

			foreach (SlopeVertexGroup svg in BuilderPlug.Me.SlopeVertexGroups)
			{
				bool copy = false;

				// Check if the current SVG has to be copied
				foreach (SlopeVertex sv in svg.Vertices)
				{
					if (sv.Selected)
					{
						copy = true;
						break;
					}
				}

				if (copy)
				{
					List<SlopeVertex> newsv = new List<SlopeVertex>();

					foreach (SlopeVertex sv in svg.Vertices)
						newsv.Add(new SlopeVertex(sv.Pos, sv.Z));

					// Use -1 for id, since a real id will be assigned when pasting
					copyslopevertexgroups.Add(new SlopeVertexGroup(-1, newsv));
				}
			}

			return true;
		}

		public override bool OnPasteBegin(PasteOptions options)
		{
			if (copyslopevertexgroups == null || copyslopevertexgroups.Count == 0)
				return false;

			// Unselect all slope vertices, so the pasted vertices can be selected
			foreach (SlopeVertexGroup svg in BuilderPlug.Me.SlopeVertexGroups)
				svg.SelectVertices(false);

			double l = copyslopevertexgroups[0].Vertices[0].Pos.x;
			double r = copyslopevertexgroups[0].Vertices[0].Pos.x;
			double t = copyslopevertexgroups[0].Vertices[0].Pos.y;
			double b = copyslopevertexgroups[0].Vertices[0].Pos.y;

			// Find the outer dimensions of all SVGs to paste
			foreach (SlopeVertexGroup svg in copyslopevertexgroups)
			{
				foreach (SlopeVertex sv in svg.Vertices)
				{
					if (sv.Pos.x < l) l = sv.Pos.x;
					if (sv.Pos.x > r) r = sv.Pos.x;
					if (sv.Pos.y > t) t = sv.Pos.y;
					if (sv.Pos.y < b) b = sv.Pos.y;
				}
			}

			Vector2D center = new Vector2D(l + ((r - l) / 2), b + ((t - b) / 2));
			Vector2D diff = center - General.Map.Grid.SnappedToGrid(mousemappos);

			foreach (SlopeVertexGroup svg in copyslopevertexgroups)
			{
				int id;
				List<SlopeVertex> newsv = new List<SlopeVertex>();

				foreach (SlopeVertex sv in svg.Vertices)
				{
					newsv.Add(new SlopeVertex(new Vector2D(sv.Pos.x - diff.x, sv.Pos.y - diff.y), sv.Z));
				}

				SlopeVertexGroup newsvg = BuilderPlug.Me.AddSlopeVertexGroup(newsv, out id);
				newsvg.SelectVertices(true);
			}

			// Redraw the display, so that pasted SVGs are shown immediately
			General.Interface.RedrawDisplay();

			// Don't go into the standard process for pasting, so tell the core that
			// pasting should not proceed
			return false;
		}

		public List<SlopeVertex> GetSelectedSlopeVertices()
		{
			List<SlopeVertex> selected = new List<SlopeVertex>();

			foreach (SlopeVertexGroup svg in BuilderPlug.Me.SlopeVertexGroups)
			{
				foreach (SlopeVertex sv in svg.Vertices)
				{
					if (sv.Selected)
						selected.Add(sv);
				}
			}

			return selected;
		}

		public List<SlopeVertex> GetUnSelectedSlopeVertices()
		{
			List<SlopeVertex> notselected = new List<SlopeVertex>();

			foreach (SlopeVertexGroup svg in BuilderPlug.Me.SlopeVertexGroups)
			{
				foreach (SlopeVertex sv in svg.Vertices)
				{
					if (!sv.Selected)
						notselected.Add(sv);
				}
			}

			return notselected;
		}

		public List<SlopeVertex> GetAllSlopeVertices()
		{
			List<SlopeVertex> selected = new List<SlopeVertex>();

			foreach (SlopeVertexGroup svg in BuilderPlug.Me.SlopeVertexGroups)
			{
				foreach (SlopeVertex sv in svg.Vertices)
				{
					selected.Add(sv);
				}
			}

			return selected;
		}

		public List<SlopeVertexGroup> GetSelectedSlopeVertexGroups()
		{
			List<SlopeVertexGroup> svgs = new List<SlopeVertexGroup>();

			foreach (SlopeVertex sv in GetSelectedSlopeVertices())
			{
				SlopeVertexGroup svg = BuilderPlug.Me.GetSlopeVertexGroup(sv);

				if (!svgs.Contains(svg))
					svgs.Add(svg);
			}

			return svgs;
		}

		#endregion

		#region ================== Actions

		[BeginAction("drawfloorslope")]
		public void DrawFloorSlope()
		{
			BuilderPlug.Me.MenusForm.CeilingSlope.Checked = false;
			BuilderPlug.Me.MenusForm.FloorSlope.Checked = true;
			BuilderPlug.Me.MenusForm.FloorAndCeilingSlope.Checked = false;

			General.Interface.DisplayStatus(StatusType.Info, "Applying drawn slope to floor");
		}

		[BeginAction("drawceilingslope")]
		public void DrawCeilingSlope()
		{
			BuilderPlug.Me.MenusForm.CeilingSlope.Checked = true;
			BuilderPlug.Me.MenusForm.FloorSlope.Checked = false;
			BuilderPlug.Me.MenusForm.FloorAndCeilingSlope.Checked = false;

			General.Interface.DisplayStatus(StatusType.Info, "Applying drawn slope to ceiling");
		}

		[BeginAction("drawfloorandceilingslope")]
		public void DrawFloorAndCeilingSlope()
		{
			BuilderPlug.Me.MenusForm.CeilingSlope.Checked = false;
			BuilderPlug.Me.MenusForm.FloorSlope.Checked = false;
			BuilderPlug.Me.MenusForm.FloorAndCeilingSlope.Checked = true;

			General.Interface.DisplayStatus(StatusType.Info, "Applying drawn slope to floor and ceiling");
		}

		[BeginAction("threedflipslope")]
		public void FlipSlope()
		{
			if (highlightedslope == null)
				return;

			MessageBox.Show("Flipping temporarily removed");

			/*
			if (highlightedslope.IsOrigin)
			{
				origin = highlightedslope.ThreeDFloor.Slope.Origin + highlightedslope.ThreeDFloor.Slope.Direction;
				direction = highlightedslope.ThreeDFloor.Slope.Direction * (-1);
			}
			else 
			{
				origin = highlightedslope.ThreeDFloor.Slope.Origin + highlightedslope.ThreeDFloor.Slope.Direction;
				direction = highlightedslope.ThreeDFloor.Slope.Direction * (-1);
			}

			highlightedslope.ThreeDFloor.Slope.Origin = origin;
			highlightedslope.ThreeDFloor.Slope.Direction = direction;

			highlightedslope.ThreeDFloor.Rebuild = true;

			BuilderPlug.ProcessThreeDFloors(new List<ThreeDFloor> { highlightedslope.ThreeDFloor }, highlightedslope.ThreeDFloor.TaggedSectors);

			UpdateSlopeObjects();

			// Redraw
			General.Interface.RedrawDisplay();
			*/
		}

		// This clears the selection
		[BeginAction("clearselection", BaseAction = true)]
		public void ClearSelection()
		{
			int numselected = 0;
			// Clear selection
			foreach (SlopeVertexGroup svg in BuilderPlug.Me.SlopeVertexGroups)
			{
				foreach (SlopeVertex sv in svg.Vertices)
				{
					if (sv.Selected)
					{
						sv.Selected = false;
						numselected++;
					}
					
				}
			}

			// Clear selected sectors when no SVGs are selected
			if (numselected == 0)
				General.Map.Map.ClearAllSelected();
			
			// Redraw
			updateOverlaySurfaces();
			UpdateOverlay();
			General.Interface.RedrawDisplay();
		}
		

		[BeginAction("deleteitem", BaseAction = true)]
		public void DeleteItem()
		{
			// Make list of selected things
			List<SlopeVertex> selected = new List<SlopeVertex>(GetSelectedSlopeVertices());

			if(highlightedslope != null)
			{
				selected.Add(highlightedslope);
			}
			
			// Anything to do?
			if(selected.Count > 0)
			{
				List<SlopeVertexGroup> groups = new List<SlopeVertexGroup>();

				General.Map.UndoRedo.CreateUndo("Delete slope");

				foreach (SlopeVertex sv in selected)
				{
					SlopeVertexGroup svg = BuilderPlug.Me.GetSlopeVertexGroup(sv);

					if (!groups.Contains(svg))
						groups.Add(svg);
				}

				foreach (SlopeVertexGroup svg in groups)
				{
                    svg.RemovePlanes();
					svg.RemoveUndoRedoUDMFFields(BuilderPlug.Me.SlopeDataSector);

					BuilderPlug.Me.SlopeVertexGroups.Remove(svg);
				}				

				General.Map.IsChanged = true;

				// Invoke a new mousemove so that the highlighted item updates
				MouseEventArgs e = new MouseEventArgs(MouseButtons.None, 0, (int)mousepos.x, (int)mousepos.y, 0);
				OnMouseMove(e);

				// Redraw screen
				General.Interface.RedrawDisplay();
			}
		}


		[BeginAction("cyclehighlighted3dfloorup")]
		public void CycleHighlighted3DFloorUp()
		{
			if (highlightedslope == null)
				return;
			List<SlopeVertex> stack = GetVertexStack();
			if (stack.Count == 0)
				return;

			int idx = stack.IndexOf(highlightedslope) + 1;
			if (idx >= stack.Count)
				idx = 0;
			highlightedslope = stack[idx];

			updateOverlaySurfaces();
			UpdateOverlay();
			General.Interface.RedrawDisplay();
		}

		[BeginAction("cyclehighlighted3dfloordown")]
		public void CycleHighlighted3DFloorDown()
		{
			if (highlightedslope == null)
				return;
			List<SlopeVertex> stack = GetVertexStack();
			if (stack.Count == 0)
				return;

			int idx = stack.IndexOf(highlightedslope) - 1;
			if (idx < 0)
				idx = stack.Count - 1;
			highlightedslope = stack[idx];

			updateOverlaySurfaces();
			UpdateOverlay();
			General.Interface.RedrawDisplay();
		}
		
		#endregion
	}
}
