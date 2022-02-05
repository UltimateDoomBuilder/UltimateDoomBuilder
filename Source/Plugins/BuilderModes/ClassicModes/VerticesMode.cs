
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
using System.Linq;
using System.Windows.Forms;
using CodeImp.DoomBuilder.BuilderModes.Interface;
using CodeImp.DoomBuilder.Windows;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Editing;
using CodeImp.DoomBuilder.Actions;
using System.Drawing;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	[EditMode(DisplayName = "Vertices Mode",
			  SwitchAction = "verticesmode",	// Action name used to switch to this mode
			  ButtonImage = "VerticesMode.png",	// Image resource name for the button
			  ButtonOrder = int.MinValue,	// Position of the button (lower is more to the left)
			  ButtonGroup = "000_editing",
			  UseByDefault = true,
			  SafeStartMode = true)]

	public class VerticesMode : BaseClassicMode
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		// Highlighted item
		private Vertex highlighted;
		private Vector2D insertpreview = new Vector2D(float.NaN, float.NaN); //mxd

		// Interface
		new private bool editpressed;
		private bool selectionfromhighlight; //mxd

		// The blockmap makes is used to make finding lines faster
		BlockMap<BlockEntry> blockmap;

		#endregion

		#region ================== Properties

		public override object HighlightedObject { get { return highlighted; } }

		public override bool AlwaysShowVertices { get { return true;  } }

		#endregion

		#region ================== Constructor / Disposer

		#endregion

		#region ================== Methods

		/// <summary>
		/// Create a blockmap containing linedefs. This is used to speed up determining the closest line
		/// to the mouse cursor
		/// </summary>
		private void CreateBlockmap()
		{
			RectangleF area = MapSet.CreateArea(General.Map.Map.Vertices);
			blockmap = new BlockMap<BlockEntry>(area);
			blockmap.AddLinedefsSet(General.Map.Map.Linedefs);
		}

		public override void OnHelp()
		{
			General.ShowHelp("e_vertices.html");
		}

		// Cancel mode
		public override void OnCancel()
		{
			base.OnCancel();
			
			// Return to this mode
			General.Editing.ChangeMode(new VerticesMode());
		}

		// Mode engages
		public override void OnEngage()
		{
			base.OnEngage();
			renderer.SetPresentation(Presentation.Standard);

			// Add toolbar buttons
			if(General.Map.UDMF) 
			{
				General.Interface.BeginToolbarUpdate(); //mxd
				General.Interface.AddButton(BuilderPlug.Me.MenusForm.CopyProperties);
				General.Interface.AddButton(BuilderPlug.Me.MenusForm.PasteProperties);
				General.Interface.AddButton(BuilderPlug.Me.MenusForm.PastePropertiesOptions); //mxd
				General.Interface.AddButton(BuilderPlug.Me.MenusForm.TextureOffsetLock, ToolbarSection.Geometry); //mxd
				General.Interface.AddButton(BuilderPlug.Me.MenusForm.TextureOffset3DFloorLock, ToolbarSection.Geometry);
				General.Interface.EndToolbarUpdate(); //mxd
			}

			// Create the blockmap
			CreateBlockmap();

			// Convert geometry selection to vertices only
			General.Map.Map.ConvertSelection(SelectionType.Vertices);
			UpdateSelectionInfo(); //mxd
		}

		// Mode disengages
		public override void OnDisengage()
		{
			base.OnDisengage();

			// Remove toolbar buttons
			General.Interface.BeginToolbarUpdate();
			General.Interface.RemoveButton(BuilderPlug.Me.MenusForm.CopyProperties);
			General.Interface.RemoveButton(BuilderPlug.Me.MenusForm.PasteProperties);
			General.Interface.RemoveButton(BuilderPlug.Me.MenusForm.PastePropertiesOptions); //mxd
			General.Interface.RemoveButton(BuilderPlug.Me.MenusForm.TextureOffsetLock); //mxd
			General.Interface.RemoveButton(BuilderPlug.Me.MenusForm.TextureOffset3DFloorLock);
			General.Interface.EndToolbarUpdate();

			// Going to EditSelectionMode?
			EditSelectionMode mode = General.Editing.NewMode as EditSelectionMode;
			if(mode != null)
			{
				// Not pasting anything?
				if(!mode.Pasting)
				{
					// No selection made? But we have a highlight!
					if((General.Map.Map.GetSelectedVertices(true).Count == 0) && (highlighted != null))
					{
						// Make the highlight the selection
						highlighted.Selected = true;
					}
				}
			}

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
				if((highlighted != null) && !highlighted.IsDisposed)
					renderer.PlotVertex(highlighted, ColorCollection.HIGHLIGHT);
				renderer.Finish();
			}

			// Render things
			if(renderer.StartThings(true))
			{
				renderer.RenderThingSet(General.Map.ThingsFilter.HiddenThings, General.Settings.HiddenThingsAlpha);
				renderer.RenderThingSet(General.Map.ThingsFilter.VisibleThings, General.Settings.ActiveThingsAlpha);
				renderer.Finish();
			}

			// Render selection
			if(selecting && renderer.StartOverlay(true)) 
			{
				RenderMultiSelection();
				renderer.Finish();
			}

			renderer.Present();
		}
		
		// This highlights a new item
		private void Highlight(Vertex v)
		{
			// Update display
			if(renderer.StartPlotter(false))
			{
				// Undraw previous highlight
				if(highlighted != null && !highlighted.IsDisposed)
					renderer.PlotVertex(highlighted, renderer.DetermineVertexColor(highlighted));

				// Set new highlight
				highlighted = v;

				// Render highlighted item
				if(highlighted != null && !highlighted.IsDisposed)
					renderer.PlotVertex(highlighted, ColorCollection.HIGHLIGHT);
				
				// Done
				renderer.Finish();
				renderer.Present();
			}

			// Show highlight info
			if((highlighted != null) && !highlighted.IsDisposed)
				General.Interface.ShowVertexInfo(highlighted);
			else
				General.Interface.HideInfo();
		}
		
		// Selection
		protected override void OnSelectBegin()
		{
			// Item highlighted?
			if((highlighted != null) && !highlighted.IsDisposed)
			{
				// Redraw highlight to show selection
				if(renderer.StartPlotter(false))
				{
					renderer.PlotVertex(highlighted, renderer.DetermineVertexColor(highlighted));
					renderer.Finish();
					renderer.Present();
				}
			}

			base.OnSelectBegin();
		}
		
		// End selection
		protected override void OnSelectEnd()
		{
			// Not stopping from multiselection?
			if(!selecting)
			{
				// Item highlighted?
				if((highlighted != null) && !highlighted.IsDisposed)
				{
					//mxd. Flip selection
					highlighted.Selected = !highlighted.Selected;
					
					// Render highlighted item
					if(renderer.StartPlotter(false))
					{
						renderer.PlotVertex(highlighted, ColorCollection.HIGHLIGHT);
						renderer.Finish();
						renderer.Present();
					}
				} 
				else if(BuilderPlug.Me.AutoClearSelection && General.Map.Map.SelectedVerticessCount > 0) 
				{
					//mxd
					General.Map.Map.ClearSelectedVertices();
					General.Interface.RedrawDisplay();
				}

				//mxd
				UpdateSelectionInfo();
			}

			base.OnSelectEnd();
		}
		
		// Start editing
		protected override void OnEditBegin()
		{
			bool snaptogrid = General.Interface.ShiftState ^ General.Interface.SnapToGrid;
			bool snaptonearest = General.Interface.CtrlState ^ General.Interface.AutoMerge;
			
			// Vertex highlighted?
			if((highlighted != null) && !highlighted.IsDisposed)
			{
				// Edit pressed in this mode
				editpressed = true;

				// Highlighted item not selected?
				if(!highlighted.Selected && (BuilderPlug.Me.AutoClearSelection || (General.Map.Map.SelectedVerticessCount == 0)))
				{
					// Make this the only selection
					selectionfromhighlight = true; //mxd
					General.Map.Map.ClearSelectedVertices();
					highlighted.Selected = true;
					UpdateSelectionInfo(); //mxd
					General.Interface.RedrawDisplay();
				}

				// Update display
				if(renderer.StartPlotter(false))
				{
					// Redraw highlight to show selection
					renderer.PlotVertex(highlighted, renderer.DetermineVertexColor(highlighted));
					renderer.Finish();
					renderer.Present();
				}
			}
			else if(!selecting) //mxd. We don't want to do this stuff while multiselecting
			{
				// Find the nearest linedef within highlight range
				Linedef l = MapSet.NearestLinedefRange(blockmap, mousemappos, BuilderPlug.Me.SplitLinedefsRange / renderer.Scale);
				if(l != null)
				{
					// Create undo
					General.Map.UndoRedo.CreateUndo("Split linedef");

					Vector2D insertpos;

					// Snip to grid also?
					if(snaptogrid)
					{
						// Find all points where the grid intersects the line
						List<Vector2D> points = l.GetGridIntersections(General.Map.Grid.GridRotate, General.Map.Grid.GridOriginX, General.Map.Grid.GridOriginY);
						insertpos = mousemappos;
						double distance = double.MaxValue;
						foreach(Vector2D p in points)
						{
							double pdist = Vector2D.DistanceSq(p, mousemappos);
							if(pdist < distance)
							{
								insertpos = p;
								distance = pdist;
							}
						}
					}
					else
					{
						// Just use the nearest point on line
						insertpos = l.NearestOnLine(mousemappos);
					}

					// Make the vertex
					Vertex v = General.Map.Map.CreateVertex(insertpos);
					if(v == null)
					{
						General.Map.UndoRedo.WithdrawUndo();
						return;
					}
					
					// Snap to map format accuracy
					v.SnapToAccuracy();

					// Split the line with this vertex
					Linedef sld = l.Split(v);
					if(sld == null)
					{
						General.Map.UndoRedo.WithdrawUndo();
						return;
					}
					//BuilderPlug.Me.AdjustSplitCoordinates(l, sld);

					// Create the blockmap
					CreateBlockmap();

					// Update
					General.Map.Map.Update();

					// Highlight it
					Highlight(v);

					// Redraw display
					General.Interface.RedrawDisplay();
				}
				else if(BuilderPlug.Me.AutoDrawOnEdit)
				{
					// Start drawing mode
					DrawGeometryMode drawmode = new DrawGeometryMode();
					DrawnVertex v = DrawGeometryMode.GetCurrentPosition(mousemappos, snaptonearest, snaptogrid, false, false, renderer, new List<DrawnVertex>());

					if(drawmode.DrawPointAt(v))
						General.Editing.ChangeMode(drawmode);
					else
						General.Interface.DisplayStatus(StatusType.Warning, "Failed to draw point: outside of map boundaries.");
				}
			}
			
			base.OnEditBegin();
		}
		
		// Done editing
		protected override void OnEditEnd()
		{
			// Edit pressed in this mode?
			if(editpressed)
			{
				// Anything selected?
				ICollection<Vertex> selected = General.Map.Map.GetSelectedVertices(true);
				if(selected.Count > 0)
				{
					if(General.Interface.IsActiveWindow)
					{
						//mxd. Show realtime vertex edit dialog
						General.Interface.OnEditFormValuesChanged += vertexEditForm_OnValuesChanged;
						DialogResult result = General.Interface.ShowEditVertices(selected);
						General.Interface.OnEditFormValuesChanged -= vertexEditForm_OnValuesChanged;

						// When a single vertex was selected, deselect it now
						if(selected.Count == 1 && selectionfromhighlight) 
						{
							General.Map.Map.ClearSelectedVertices();
						} 
						else if(result == DialogResult.Cancel) //mxd. Restore selection...
						{ 
							foreach(Vertex v in selected) v.Selected = true;
						}

						// Update entire display
						UpdateSelectionInfo(); //mxd
						General.Interface.RedrawDisplay();
					}
				}
			}

			editpressed = false;
			selectionfromhighlight = false; //mxd
			base.OnEditEnd();
		}

		//mxd
		private void vertexEditForm_OnValuesChanged(object sender, EventArgs e) 
		{
			// Update entire display
			General.Map.Map.Update();
			General.Interface.RedrawDisplay();
		}
		
		// Mouse moves
		public override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			if(panning) return; //mxd. Skip all this jazz while panning

			//mxd
			if(selectpressed && !editpressed && !selecting) 
			{
				// Check if moved enough pixels for multiselect
				Vector2D delta = mousedownpos - mousepos;
				if((Math.Abs(delta.x) > BuilderPlug.Me.MouseSelectionThreshold) ||
				   (Math.Abs(delta.y) > BuilderPlug.Me.MouseSelectionThreshold)) 
				{
					// Start multiselecting
					StartMultiSelection();
				}
			}
			else if(paintselectpressed && !editpressed && !selecting)  //mxd. Drag-select
			{
				// Find the nearest thing within highlight range
				Vertex v = General.Map.Map.NearestVertexSquareRange(mousemappos, BuilderPlug.Me.HighlightRange / renderer.Scale);

				if(v != null) 
				{
					if(v != highlighted) 
					{
						//toggle selected state
						if(General.Interface.ShiftState ^ BuilderPlug.Me.AdditivePaintSelect)
							v.Selected = true;
						else if(General.Interface.CtrlState)
							v.Selected = false;
						else
							v.Selected = !v.Selected;
						highlighted = v;

						UpdateSelectionInfo(); //mxd

						// Update entire display
						General.Interface.RedrawDisplay();
					}
				} 
				else if(highlighted != null) 
				{
					highlighted = null;
					Highlight(null);

					// Update entire display
					General.Interface.RedrawDisplay();
				}
			}
			else if(e.Button == MouseButtons.None) // Not holding any buttons?
			{
				//mxd. Render insert vertex preview
				Linedef l = MapSet.NearestLinedefRange(blockmap, mousemappos, BuilderPlug.Me.SplitLinedefsRange / renderer.Scale);

				if(l != null) 
				{
					// Snip to grid?
					if(General.Interface.ShiftState ^ General.Interface.SnapToGrid) 
					{
						// Find all points where the grid intersects the line
						List<Vector2D> points = l.GetGridIntersections(General.Map.Grid.GridRotate, General.Map.Grid.GridOriginX, General.Map.Grid.GridOriginY);

						if(points.Count == 0) 
						{
							insertpreview = l.NearestOnLine(mousemappos);
						} 
						else 
						{
							insertpreview = mousemappos;
							double distance = double.MaxValue;
							foreach(Vector2D p in points) 
							{
								double pdist = Vector2D.DistanceSq(p, mousemappos);
								if(pdist < distance) 
								{
									insertpreview = p;
									distance = pdist;
								}
							}
						}
					} 
					else 
					{
						// Just use the nearest point on line
						insertpreview = l.NearestOnLine(mousemappos);
					}

					//render preview
					if(renderer.StartOverlay(true)) 
					{
						double dist = Math.Min(Vector2D.Distance(mousemappos, insertpreview), BuilderPlug.Me.SplitLinedefsRange);
						byte alpha = (byte)(255 - (dist / BuilderPlug.Me.SplitLinedefsRange) * 128);
						float vsize = (renderer.VertexSize + 1.0f) / renderer.Scale;
						renderer.RenderRectangleFilled(new RectangleF((float)(insertpreview.x - vsize), (float)(insertpreview.y - vsize), vsize * 2.0f, vsize * 2.0f), General.Colors.InfoLine.WithAlpha(alpha), true);
						renderer.Finish();
						renderer.Present();
					}
				} 
				else if(insertpreview.IsFinite()) 
				{
					insertpreview.x = float.NaN;

					//undraw preveiw
					if(renderer.StartOverlay(true)) 
					{
						renderer.Finish();
						renderer.Present();
					}
				}
				
				// Find the nearest vertex within highlight range
				Vertex v = General.Map.Map.NearestVertexSquareRange(mousemappos, BuilderPlug.Me.HighlightRange / renderer.Scale);

				// Highlight if not the same
				if(v != highlighted) Highlight(v);
			} 
		}

		// Mouse leaves
		public override void OnMouseLeave(EventArgs e)
		{
			base.OnMouseLeave(e);
			
			// Highlight nothing
			Highlight(null);
		}

		public override void OnUndoEnd()
		{
			base.OnUndoEnd();

			// Recreate the blockmap
			CreateBlockmap();

			// Select changed map elements
			if (BuilderPlug.Me.SelectChangedafterUndoRedo)
			{
				General.Map.Map.SelectMarkedGeometry(true, true);
				General.Map.Map.ConvertSelection(SelectionType.Vertices);
			}
		}

		public override void OnRedoEnd()
		{
			base.OnRedoEnd();

			// Recreate the blockmap
			CreateBlockmap();

			// Select changed map elements
			if (BuilderPlug.Me.SelectChangedafterUndoRedo)
			{
				General.Map.Map.SelectMarkedGeometry(true, true);
				General.Map.Map.ConvertSelection(SelectionType.Vertices);
			}
		}

		public override void OnScriptRunEnd()
		{
			base.OnScriptRunEnd();

			CreateBlockmap();

			General.Interface.RedrawDisplay();
		}

		//mxd
		protected override void BeginViewPan() 
		{
			if(insertpreview.IsFinite()) 
			{
				insertpreview.x = float.NaN;

				//undraw preveiw
				if(renderer.StartOverlay(true)) 
				{
					renderer.Finish();
					renderer.Present();
				}
			}

			base.BeginViewPan();
		}

		//mxd
		protected override void OnPaintSelectBegin() 
		{
			highlighted = null;
			base.OnPaintSelectBegin();
		}

		// Mouse wants to drag
		protected override void OnDragStart(MouseEventArgs e)
		{
			base.OnDragStart(e);

			// Edit button used?
			if(General.Actions.CheckActionActive(null, "classicedit"))
			{
				// Anything highlighted?
				if((highlighted != null) && !highlighted.IsDisposed)
				{
					// Highlighted item not selected?
					if(!highlighted.Selected)
					{
						// Select only this vertex for dragging
						General.Map.Map.ClearSelectedVertices();
						highlighted.Selected = true;
					}

					// Start dragging the selection
					if(!BuilderPlug.Me.DontMoveGeometryOutsideMapBoundary || CanDrag()) //mxd
						General.Editing.ChangeMode(new DragVerticesMode(mousedownmappos));
				}
			}
		}

		//mxd. Check if any selected vertex is outside of map boundary
		private static bool CanDrag()
		{
			ICollection<Vertex> selectedverts = General.Map.Map.GetSelectedVertices(true);
			int unaffectedCount = 0;

			foreach(Vertex v in selectedverts) 
			{
				// Make sure the vertex is inside the map boundary
				if(v.Position.x < General.Map.Config.LeftBoundary || v.Position.x > General.Map.Config.RightBoundary
					|| v.Position.y > General.Map.Config.TopBoundary || v.Position.y < General.Map.Config.BottomBoundary) 
				{

					v.Selected = false;
					unaffectedCount++;
				}
			}

			if(unaffectedCount == selectedverts.Count) 
			{
				General.Interface.DisplayStatus(StatusType.Warning, "Unable to drag selection: " + (selectedverts.Count == 1 ? "selected vertex is" : "all of selected vertices are") + " outside of map boundary!");
				General.Interface.RedrawDisplay();
				return false;
			}
			
			if(unaffectedCount > 0)
				General.Interface.DisplayStatus(StatusType.Warning, unaffectedCount + " of selected vertices " + (unaffectedCount == 1 ? "is" : "are") + " outside of map boundary!");

			return true;
		}

		// This is called wheh selection ends
		protected override void OnEndMultiSelection()
		{
			bool selectionvolume = ((Math.Abs(selectionrect.Width) > 0.1f) && (Math.Abs(selectionrect.Height) > 0.1f));

			if(selectionvolume)
			{
				//mxd
				switch(marqueSelectionMode) 
				{
					case MarqueSelectionMode.SELECT:
						foreach(Vertex v in General.Map.Map.Vertices)
							v.Selected = selectionrect.Contains((float)v.Position.x, (float)v.Position.y);
						break;

					case MarqueSelectionMode.ADD:
						foreach(Vertex v in General.Map.Map.Vertices)
							v.Selected |= selectionrect.Contains((float)v.Position.x, (float)v.Position.y);
						break;

					case MarqueSelectionMode.SUBTRACT:
						foreach(Vertex v in General.Map.Map.Vertices)
							if(selectionrect.Contains((float)v.Position.x, (float)v.Position.y)) v.Selected = false;
						break;

					default: //should be Intersect
						foreach(Vertex v in General.Map.Map.Vertices)
							if(!selectionrect.Contains((float)v.Position.x, (float)v.Position.y)) v.Selected = false;
						break;
				}

				//mxd
				UpdateSelectionInfo();
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

			// Render selection
			if(renderer.StartOverlay(true))
			{
				RenderMultiSelection();
				renderer.Finish();
				renderer.Present();
			}
		}
		
		// When copying
		public override bool OnCopyBegin()
		{
			// No selection made? But we have a highlight!
			if((General.Map.Map.GetSelectedVertices(true).Count == 0) && (highlighted != null))
			{
				// Make the highlight the selection
				highlighted.Selected = true;

				//mxd. Actually, we want it marked, not selected
				bool result = base.OnCopyBegin(); 
				highlighted.Selected = false;
				return result;
			}
			
			return base.OnCopyBegin();
		}

		/// <summary>
		/// If map elements have changed the blockmap needs to be recreated.
		/// </summary>
		public override void OnMapElementsChanged()
		{
			base.OnMapElementsChanged();

			CreateBlockmap();
		}

		#endregion

		#region ================== Actions

		// This copies the properties
		[BeginAction("classiccopyproperties")]
		public void CopyProperties()
		{
			// Determine source vertices
			ICollection<Vertex> sel = null;
			if(General.Map.Map.SelectedVerticessCount > 0) sel = General.Map.Map.GetSelectedVertices(true);
			else if(highlighted != null) sel = new List<Vertex> { highlighted };

			if(sel != null)
			{
				// Copy properties from first source vertex
				BuilderPlug.Me.CopiedVertexProps = new VertexProperties(General.GetByIndex(sel, 0));
				General.Interface.DisplayStatus(StatusType.Action, "Copied vertex properties.");
			}
			else
			{
				//mxd
				General.Interface.DisplayStatus(StatusType.Warning, "This action requires highlight or selection!");
			}
		}

		// This pastes the properties
		[BeginAction("classicpasteproperties")]
		public void PasteProperties()
		{
			if(BuilderPlug.Me.CopiedVertexProps != null)
			{
				// Determine target vertices
				ICollection<Vertex> sel = null;
				if(General.Map.Map.SelectedVerticessCount > 0) sel = General.Map.Map.GetSelectedVertices(true);
				else if(highlighted != null) sel = new List<Vertex> { highlighted };

				if(sel != null)
				{
					// Apply properties to selection
					string rest = (sel.Count == 1 ? "a single vertex" : sel.Count + " vertices"); //mxd
					General.Map.UndoRedo.CreateUndo("Paste properties to " + rest);
					BuilderPlug.Me.CopiedVertexProps.Apply(sel, false);
					General.Interface.DisplayStatus(StatusType.Action, "Pasted properties to " + rest + ".");

					// Update and redraw
					General.Map.IsChanged = true;
					General.Interface.RefreshInfo();
					General.Interface.RedrawDisplay();
				}
				else
				{
					//mxd
					General.Interface.DisplayStatus(StatusType.Warning, "This action requires highlight or selection!");
				}
			}
			else
			{
				//mxd
				General.Interface.DisplayStatus(StatusType.Warning, "Copy vertex properties first!");
			}
		}

		//mxd. This pastes the properties with options
		[BeginAction("classicpastepropertieswithoptions")]
		public void PastePropertiesWithOptions()
		{
			if(BuilderPlug.Me.CopiedVertexProps != null)
			{
				// Determine target vertices
				ICollection<Vertex> sel = null;
				if(General.Map.Map.SelectedVerticessCount > 0) sel = General.Map.Map.GetSelectedVertices(true);
				else if(highlighted != null) sel = new List<Vertex> { highlighted };

				if(sel != null)
				{
					PastePropertiesOptionsForm form = new PastePropertiesOptionsForm();
					if(form.Setup(MapElementType.VERTEX) && form.ShowDialog(General.Interface) == DialogResult.OK)
					{
						// Apply properties to selection
						string rest = (sel.Count == 1 ? "a single vertex" : sel.Count + " vertices");
						General.Map.UndoRedo.CreateUndo("Paste properties with options to " + rest);
						BuilderPlug.Me.CopiedVertexProps.Apply(sel, true);
						General.Interface.DisplayStatus(StatusType.Action, "Pasted properties with options to " + rest + ".");

						// Update and redraw
						General.Map.IsChanged = true;
						General.Interface.RefreshInfo();
						General.Interface.RedrawDisplay();
					}
				}
				else
				{
					General.Interface.DisplayStatus(StatusType.Warning, "This action requires highlight or selection!");
				}
			}
			else
			{
				General.Interface.DisplayStatus(StatusType.Warning, "Copy vertex properties first!");
			}
		}

		// This clears the selection
		[BeginAction("clearselection", BaseAction = true)]
		public void ClearSelection()
		{
			// Clear selection
			General.Map.Map.ClearAllSelected();

			//mxd. Clear selection info
			General.Interface.DisplayStatus(StatusType.Selection, string.Empty);

			// Redraw
			General.Interface.RedrawDisplay();
		}
		
		// This creates a new vertex at the mouse position
		[BeginAction("insertitem", BaseAction = true)]
		private void InsertVertex()
		{
			bool snaptogrid = General.Interface.ShiftState ^ General.Interface.SnapToGrid;
			bool snaptonearest = General.Interface.CtrlState ^ General.Interface.AutoMerge;

			// Mouse in window?
			if(General.Interface.MouseInDisplay)
			{
				Vector2D insertpos;

				// Create undo
				General.Map.UndoRedo.CreateUndo("Insert vertex");

				// Snap to geometry?
				Linedef l = MapSet.NearestLinedefRange(blockmap, mousemappos, BuilderPlug.Me.SplitLinedefsRange / renderer.Scale);
				if(snaptonearest && (l != null))
				{
					// Snip to grid also?
					if(snaptogrid)
					{
						// Find all points where the grid intersects the line
						List<Vector2D> points = l.GetGridIntersections(General.Map.Grid.GridRotate, General.Map.Grid.GridOriginX, General.Map.Grid.GridOriginY);
						if (points.Count == 0) 
						{
							//mxd. Just use the nearest point on line
							insertpos = l.NearestOnLine(mousemappos);
						} 
						else 
						{
							insertpos = mousemappos;
							double distance = double.MaxValue;
							foreach(Vector2D p in points) 
							{
								double pdist = Vector2D.DistanceSq(p, mousemappos);
								if(pdist < distance) 
								{
									insertpos = p;
									distance = pdist;
								}
							}
						}
					}
					else
					{
						// Just use the nearest point on line
						insertpos = l.NearestOnLine(mousemappos);
					}
				}
				// Snap to grid?
				else if(snaptogrid)
				{
					// Snap to grid
					insertpos = General.Map.Grid.SnappedToGrid(mousemappos);
				}
				else
				{
					// Just insert here, don't snap to anything
					insertpos = mousemappos;
				}

				// Make the vertex
				Vertex v = General.Map.Map.CreateVertex(insertpos);
				if(v == null)
				{
					General.Map.UndoRedo.WithdrawUndo();
					return;
				}

				// Snap to map format accuracy
				v.SnapToAccuracy();

				// Split the line with this vertex
				if(snaptonearest)
				{
					//mxd. Check if snapped vertex is still on top of a linedef
					l = MapSet.NearestLinedefRange(blockmap, v.Position, BuilderPlug.Me.SplitLinedefsRange / renderer.Scale);
					
					if(l != null) 
					{
						//mxd
						if(v.Position == l.Start.Position || v.Position == l.End.Position) 
						{
							General.Interface.DisplayStatus(StatusType.Info, "There's already a vertex here.");
							General.Map.UndoRedo.WithdrawUndo();
							return;
						}
						
						General.Interface.DisplayStatus(StatusType.Action, "Split a linedef.");
						Linedef sld = l.Split(v);
						if(sld == null) 
						{
							General.Map.UndoRedo.WithdrawUndo(); 
							return;
						}
						//BuilderPlug.Me.AdjustSplitCoordinates(l, sld);
					}
				}
				else
				{
					General.Interface.DisplayStatus(StatusType.Action, "Inserted a vertex.");
				}

				// Create the blockmap
				CreateBlockmap();

				// Update
				General.Map.Map.Update();

				// Redraw screen
				General.Interface.RedrawDisplay();
			}
		}

		[BeginAction("deleteitem", BaseAction = true)]
		public void DeleteItem() 
		{
			// Make list of selected vertices
			ICollection<Vertex> selected = General.Map.Map.GetSelectedVertices(true);
			if((selected.Count == 0) && (highlighted != null) && !highlighted.IsDisposed) selected.Add(highlighted);
			if(selected.Count == 0) return;

			// Make undo
			if(selected.Count > 1) 
			{
				General.Map.UndoRedo.CreateUndo("Delete " + selected.Count + " vertices");
				General.Interface.DisplayStatus(StatusType.Action, "Deleted " + selected.Count + " vertices.");
			} 
			else 
			{
				General.Map.UndoRedo.CreateUndo("Delete vertex");
				General.Interface.DisplayStatus(StatusType.Action, "Deleted a vertex.");
			}

			// Go for all vertices that need to be removed
			foreach(Vertex v in selected) 
			{
				// Not already removed automatically?
				if(!v.IsDisposed) 
				{
					// If the vertex only has 2 linedefs attached, then merge the linedefs
					if(v.Linedefs.Count == 2) 
					{
						Linedef ld1 = General.GetByIndex(v.Linedefs, 0);
						Linedef ld2 = General.GetByIndex(v.Linedefs, 1);
						Vertex v2 = (ld2.Start == v) ? ld2.End : ld2.Start;
						if(ld1.Start == v) ld1.SetStartVertex(v2); else ld1.SetEndVertex(v2);
						ld2.Dispose();
					}

					// Trash vertex
					v.Dispose();
				}
			}

			// Update cache values
			General.Map.IsChanged = true;
			General.Map.Map.Update();

			// Create the blockmap
			CreateBlockmap();

			// Invoke a new mousemove so that the highlighted item updates
			MouseEventArgs e = new MouseEventArgs(MouseButtons.None, 0, (int)mousepos.x, (int)mousepos.y, 0);
			OnMouseMove(e);

			// Redraw screen
			UpdateSelectionInfo(); //mxd
			General.Map.Renderer2D.UpdateExtraFloorFlag(); //mxd
			General.Interface.RedrawDisplay();
		}

		[BeginAction("dissolveitem", BaseAction = true)] //mxd
		public void DissolveItem() 
		{
			// Make list of selected vertices
			ICollection<Vertex> selected = General.Map.Map.GetSelectedVertices(true);
			if(selected.Count == 0) 
			{
				if(highlighted == null || highlighted.IsDisposed) return;
				selected.Add(highlighted);
			}

			// Make undo
			if(selected.Count > 1) 
			{
				General.Map.UndoRedo.CreateUndo("Dissolve " + selected.Count + " vertices");
				General.Interface.DisplayStatus(StatusType.Action, "Dissolved " + selected.Count + " vertices.");
			} 
			else 
			{
				General.Map.UndoRedo.CreateUndo("Dissolve vertex");
				General.Interface.DisplayStatus(StatusType.Action, "Dissolved a vertex.");
			}

			//collect linedefs count per vertex
			Dictionary<Vertex, int> linesPerVertex = new Dictionary<Vertex, int>();
			foreach(Vertex v in selected) 
			{
				linesPerVertex.Add(v, v.Linedefs.Count);
			}

			// Go for all vertices that need to be removed
			foreach(Vertex v in selected) 
			{
				// Not already removed automatically?
				if(!v.IsDisposed) 
				{
					// If the vertex only had 2 linedefs attached, then merge the linedefs
					if(linesPerVertex[v] == 2) 
					{
						Linedef ld1 = General.GetByIndex(v.Linedefs, 0);
						Linedef ld2 = General.GetByIndex(v.Linedefs, 1);
						Vertex v1 = (ld1.Start == v) ? ld1.End : ld1.Start;
						Vertex v2 = (ld2.Start == v) ? ld2.End : ld2.Start;

						//don't merge if it will collapse 3-sided sector
						bool dontMerge = false;
						foreach(Linedef l in v1.Linedefs) 
						{
							if(l == ld2) continue;
							if(l.Start == v2 || l.End == v2) 
							{
								TryJoinSectors(l);
								dontMerge = true;
								break;
							}
						}

						if(!dontMerge) MergeLines(selected, ld1, ld2, v);
					}

					// Trash vertex
					v.Dispose();
				}
			}

			// Update cache values
			General.Map.Map.Update();
			General.Map.IsChanged = true;

			// Create the blockmap
			CreateBlockmap();

			// Invoke a new mousemove so that the highlighted item updates
			MouseEventArgs e = new MouseEventArgs(MouseButtons.None, 0, (int)mousepos.x, (int)mousepos.y, 0);
			OnMouseMove(e);

			// Redraw screen
			UpdateSelectionInfo(); //mxd
			General.Map.Renderer2D.UpdateExtraFloorFlag(); //mxd
			General.Interface.RedrawDisplay();
		}

		[BeginAction("placethings")] //mxd
		public void PlaceThings() 
		{
			// Make list of selected vertices
			ICollection<Vertex> selected = General.Map.Map.GetSelectedVertices(true);
			if(selected.Count == 0) 
			{
				if(highlighted != null && !highlighted.IsDisposed)
				{
					selected.Add(highlighted);
				} 
				else 
				{
					General.Interface.DisplayStatus(StatusType.Warning, "This action requires selection of some description!");
					return;
				}
			}

			List<Vector2D> positions = new List<Vector2D>(selected.Count);
			foreach(Vertex v in selected)
				if(!positions.Contains(v.Position)) positions.Add(v.Position);
			PlaceThingsAtPositions(positions);
		}

		//mxd
		[BeginAction("selectsimilar")]
		public void SelectSimilar() 
		{
			ICollection<Vertex> selection = General.Map.Map.GetSelectedVertices(true);

			if(selection.Count == 0) 
			{
				General.Interface.DisplayStatus(StatusType.Warning, "This action requires a selection!");
				return;
			}

			var form = new SelectSimilarElementOptionsPanel();
			if(form.Setup(this)) form.ShowDialog(General.Interface);
		}

		[BeginAction("smartgridtransform", BaseAction = true)]
		protected void SmartGridTransform()
		{
			if (General.Map.Map.SelectedVerticessCount > 1)
			{
				General.Interface.DisplayStatus(StatusType.Warning, "Either nothing or exactly one vertex must be selected");
				General.Interface.MessageBeep(MessageBeepType.Warning);
				return;
			}

			Vertex vertex = null;

			if (General.Map.Map.SelectedVerticessCount == 1)
				vertex = General.Map.Map.GetSelectedVertices(true).First();
			else if (highlighted != null)
				vertex = highlighted;

			if (vertex != null)
			{
				General.Map.Grid.SetGridOrigin(vertex.Position.x, vertex.Position.y);
				General.Map.GridVisibilityChanged();
				General.Interface.RedrawDisplay();
			}
			else
			{
				General.Map.Grid.SetGridRotation(0.0);
				General.Map.Grid.SetGridOrigin(0, 0);
				General.Map.GridVisibilityChanged();
				General.Interface.RedrawDisplay();
			}
		}

		#endregion

		#region ================== Action assist (mxd)

			//mxd
		private static void MergeLines(ICollection<Vertex> selected, Linedef ld1, Linedef ld2, Vertex v) 
		{
			Vertex v1 = (ld1.Start == v) ? ld1.End : ld1.Start;
			Vertex v2 = (ld2.Start == v) ? ld2.End : ld2.Start;

			if(ld1.Start == v) ld1.SetStartVertex(v2);
			else ld1.SetEndVertex(v2);
			ld2.Dispose();
			bool redraw = true;

			if(!v2.IsDisposed && selected.Contains(v2) && v2.Linedefs.Count == 2) 
			{
				Linedef[] lines = new Linedef[2];
				v2.Linedefs.CopyTo(lines, 0);
				Linedef other = lines[0] == ld2 ? lines[1] :lines[0];

				MergeLines(selected, ld1, other, v2);
				v2.Dispose();
				redraw = false;
			}

			if(!v1.IsDisposed && selected.Contains(v1) && v1.Linedefs.Count == 2) 
			{
				Linedef[] lines = new Linedef[2];
				v1.Linedefs.CopyTo(lines, 0);
				Linedef other = lines[0] == ld1 ? lines[1] : lines[0];

				MergeLines(selected, other, ld1, v1);
				v1.Dispose();
				redraw = false;
			}

			if(redraw && ld1.Start != null && ld1.End != null) 
			{
				Vector2D start = ld1.Start.Position;
				Vector2D end = ld1.End.Position;
				ld1.Dispose();
				DrawLine(start, end);
			} 
			else 
			{
				ld1.Dispose();
			}
		}

		//mxd
		private static void DrawLine(Vector2D start, Vector2D end) 
		{
			DrawnVertex dv1 = new DrawnVertex();
			DrawnVertex dv2 = new DrawnVertex();
			dv1.stitchline = true;
			dv2.stitchline = true;
			dv1.stitch = true;
			dv2.stitch = true;
			dv1.pos = start;
			dv2.pos = end;
			Tools.DrawLines(new List<DrawnVertex> { dv1, dv2 }, false, false);

			// Update cache values
			General.Map.Map.Update();
			General.Map.IsChanged = true;
		}

		//mxd. If there are different sectors on both sides of given linedef, join them
		private static void TryJoinSectors(Linedef ld) 
		{
			if(ld.IsDisposed) return;

			if(ld.Front != null && ld.Front.Sector != null && ld.Back != null 
				&& ld.Back.Sector != null && ld.Front.Sector.Index != ld.Back.Sector.Index) 
			{
				if(ld.Front.Sector.BBox.Width * ld.Front.Sector.BBox.Height > ld.Back.Sector.BBox.Width * ld.Back.Sector.BBox.Height)
					ld.Back.Sector.Join(ld.Front.Sector);
				else
					ld.Front.Sector.Join(ld.Back.Sector);
			}
		}
		
		#endregion
	}
}
