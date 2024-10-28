
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
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.Types;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	public abstract class DragGeometryMode : BaseClassicMode
	{
		#region ================== Constants

		#endregion

		#region ================== Variables
		
		// Mouse position on map where dragging started
		private Vector2D dragstartmappos;

		//mxd. Offset from nearest grid intersection to dragstartmappos
		private Vector2D dragstartoffset;

		// Item used as reference for snapping to the grid
		protected Vertex dragitem;
		private Vector2D dragitemposition;

		// List of old vertex positions
		private List<Vector2D> oldpositions;
		private List<Vector2D> oldthingpositions; //mxd

		// List of selected items
		protected ICollection<Vertex> selectedverts;
		protected ICollection<Thing> selectedthings; //mxd

		// List of non-selected items
		protected ICollection<Vertex> unselectedverts;
		protected ICollection<Thing> unselectedthings; //mxd

		// List of things, which should be moved
		protected ICollection<Thing> thingstodrag; //mxd

		//mxd. List of sectors
		private List<Sector> selectedsectors;

		// List of unstable lines
		protected ICollection<Linedef> unstablelines;
		
		// List of unselected lines
		protected ICollection<Linedef> snaptolines;
		
		// Text labels for all unstable lines
		protected LineLengthLabel[] labels;

		//mxd. Undo description
		protected string undodescription = "Drag geometry";
		
		// Keep track of view changes
		private float lastoffsetx;
		private float lastoffsety;
		private float lastscale;
		
		// Options
		private bool snaptogrid;		// SHIFT to toggle
		private bool snaptonearest;		// CTRL to enable
		private bool snaptogridincrement; //mxd. ALT to toggle 
		private bool snaptocardinaldirection; //mxd. ALT-SHIFT to enable

		#endregion

		#region ================== Properties

		// Just keep the base mode button checked
		public override string EditModeButtonName { get { return General.Editing.PreviousStableMode.Name; } }
		
		#endregion

		#region ================== Constructor / Disposer

		// Disposer
		public override void Dispose()
		{
			// Not already disposed?
			if(!isdisposed)
			{
				// Clean up
				if(labels != null)
					foreach(LineLengthLabel l in labels) l.Dispose();
				
				// Done
				base.Dispose();
			}
		}

		#endregion

		#region ================== Methods

		// Constructor to start dragging immediately
		protected void StartDrag(Vector2D dragstartmappos)
		{
			// Initialize
			this.dragstartmappos = dragstartmappos;
			
			Cursor.Current = Cursors.AppStarting;
			
			// We don't want to record this for undoing while we move the geometry around.
			// This will be set back to normal when we're done.
			General.Map.UndoRedo.IgnorePropChanges = true;

			// Make list of selected vertices and things
			selectedverts = General.Map.Map.GetMarkedVertices(true);
			selectedthings = General.Map.Map.GetSelectedThings(true); //mxd

			// Only set the things to drag if they weren't set by the inherited modes
			if(thingstodrag == null)
				thingstodrag = (BuilderPlug.Me.SyncronizeThingEdit ? selectedthings : new List<Thing>()); //mxd

			// Make list of non-selected vertices and things
			// Non-selected vertices will be used for snapping to nearest items
			unselectedverts = General.Map.Map.GetMarkedVertices(false);
			unselectedthings = new List<Thing>(); //mxd
			foreach(Thing t in General.Map.ThingsFilter.VisibleThings) if(!t.Selected) unselectedthings.Add(t);

			// Get the nearest vertex for snapping
			dragitem = MapSet.NearestVertex(selectedverts, dragstartmappos);

			//mxd. Get drag offset
			dragstartoffset = General.Map.Grid.SnappedToGrid(dragitem.Position) - dragitem.Position;
			
			// Lines to snap to
			snaptolines = General.Map.Map.LinedefsFromMarkedVertices(true, false, false);
			
			// Make old positions list
			// We will use this as reference to move the vertices, or to move them back on cancel
			oldpositions = new List<Vector2D>(selectedverts.Count);
			foreach(Vertex v in selectedverts) oldpositions.Add(v.Position);

			//mxd
			oldthingpositions = new List<Vector2D>(thingstodrag.Count);
			foreach(Thing t in thingstodrag) oldthingpositions.Add(t.Position);

			// Also keep old position of the dragged item
			dragitemposition = dragitem.Position;

			// Keep view information
			lastoffsetx = renderer.OffsetX;
			lastoffsety = renderer.OffsetY;
			lastscale = renderer.Scale;

			// Make list of unstable lines only
			// These will have their length displayed during the drag
			unstablelines = MapSet.UnstableLinedefsFromVertices(selectedverts);

			//mxd. Collect selected sectors
			if(General.Map.UDMF) 
			{
				ICollection<Linedef> selectedLines = General.Map.Map.LinedefsFromMarkedVertices(false, true, false);
				List<Sector> affectedSectors = new List<Sector>();
				foreach(Linedef l in selectedLines) 
				{
					if(l.Front != null && l.Front.Sector != null && !affectedSectors.Contains(l.Front.Sector))
						affectedSectors.Add(l.Front.Sector);
					if(l.Back != null && l.Back.Sector != null && !affectedSectors.Contains(l.Back.Sector))
						affectedSectors.Add(l.Back.Sector);
				}

				selectedsectors = new List<Sector>();
				foreach(Sector s in affectedSectors) 
				{
					bool selected = true;
					foreach(Sidedef side in s.Sidedefs) 
					{
						if(!selectedLines.Contains(side.Line)) 
						{
							selected = false;
							break;
						}
					}

					if(selected) selectedsectors.Add(s);
				}
			}

			// Make text labels
			labels = new LineLengthLabel[unstablelines.Count];
			int index = 0;
			foreach(Linedef l in unstablelines)
				labels[index++] = new LineLengthLabel(l.Start.Position, l.End.Position);
			
			Cursor.Current = Cursors.Default;
		}

		// This moves the selected geometry relatively
		// Returns true when geometry has actually moved
		private bool MoveGeometryRelative(Vector2D offset, bool snapgrid, bool snapgridincrement, bool snapnearest, bool snapcardinal)
		{
			//mxd. If snap to cardinal directions is enabled, modify the offset
			if(snapcardinal)
			{
				double angle = Angle2D.DegToRad((General.ClampAngle((int)Angle2D.RadToDeg(offset.GetAngle()) + 44)) / 90 * 90);
				offset = new Vector2D(0, -offset.GetLength()).GetRotated(angle);
				snapgridincrement = true; // We don't want to move the geometry away from the cardinal directions
			}
			
			Vector2D oldpos = dragitem.Position;
			Vector2D anchorpos = dragitemposition + offset;
			Vector2D tl, br;

			// don't move if the offset contains invalid data
			if(!offset.IsFinite()) return false;

			// Find the outmost vertices
			tl = br = oldpositions[0];
			for(int i = 0; i < oldpositions.Count; i++)
			{
				if(oldpositions[i].x < tl.x) tl.x = (int)oldpositions[i].x;
				if(oldpositions[i].x > br.x) br.x = (int)oldpositions[i].x;
				if(oldpositions[i].y > tl.y) tl.y = (int)oldpositions[i].y;
				if(oldpositions[i].y < br.y) br.y = (int)oldpositions[i].y;
			}
			
			// Snap to nearest?
			if(snapnearest)
			{
				// Find nearest unselected vertex within range
				Vertex nv = MapSet.NearestVertexSquareRange(unselectedverts, anchorpos, BuilderPlug.Me.StitchRange / renderer.Scale);
				if(nv != null)
				{
					// Move the dragged item
					dragitem.Move(nv.Position);

					// Adjust the offset
					offset = nv.Position - dragitemposition;

					// Do not snap to grid!
					snapgrid = false;
					snaptogridincrement = false; //mxd
				}
				else
				{
					// Find the nearest unselected line within range
					Linedef nl = MapSet.NearestLinedefRange(snaptolines, anchorpos, BuilderPlug.Me.StitchRange / renderer.Scale);
					if(nl != null)
					{
						// Snap to grid?
						if(snapgrid || snapgridincrement)
						{
							// Get grid intersection coordinates
							List<Vector2D> coords = nl.GetGridIntersections(snapgridincrement ? dragstartoffset : new Vector2D(),
								General.Map.Grid.GridRotate, General.Map.Grid.GridOriginX, General.Map.Grid.GridOriginY);

							// mxd. Do the rest only if we actually have some coordinates
							if(coords.Count > 0) 
							{
								// Find nearest grid intersection
								double found_distance = double.MaxValue;
								Vector2D found_coord = new Vector2D();

								foreach(Vector2D v in coords) 
								{
									Vector2D delta = anchorpos - v;
									if(delta.GetLengthSq() < found_distance) 
									{
										found_distance = delta.GetLengthSq();
										found_coord = v;
									}
								}

								// Move the dragged item
								dragitem.Move(found_coord);

								// Align to line here
								offset = found_coord - dragitemposition;

								// Do not snap to grid anymore
								snapgrid = false;
								snapgridincrement = false; //mxd
							}
						}
						else
						{
							// Move the dragged item
							dragitem.Move(nl.NearestOnLine(anchorpos));

							// Align to line here
							offset = nl.NearestOnLine(anchorpos) - dragitemposition;
						}
					}
				}
			}

			// Snap to grid or grid increment?
			if(snapgrid || snapgridincrement)
			{
				// Move the dragged item
				dragitem.Move(anchorpos);

				// Snap item to grid increment
				if(snapgridincrement) //mxd
				{
					dragitem.Move(General.Map.Grid.SnappedToGrid(dragitem.Position) - dragstartoffset);
				}
				else // Or to the grid itself
				{
					dragitem.SnapToGrid();
				}

				// Adjust the offset
				offset += dragitem.Position - anchorpos;
			}

			// Make sure the offset is inside the map boundaries
			if(offset.x + tl.x < General.Map.Config.LeftBoundary) offset.x = General.Map.Config.LeftBoundary - tl.x;
			if(offset.x + br.x > General.Map.Config.RightBoundary) offset.x = General.Map.Config.RightBoundary - br.x;
			if(offset.y + tl.y > General.Map.Config.TopBoundary) offset.y = General.Map.Config.TopBoundary - tl.y;
			if(offset.y + br.y < General.Map.Config.BottomBoundary) offset.y = General.Map.Config.BottomBoundary - br.y;

			// Drag item moved?
			if((!snapgrid && !snapgridincrement) || (dragitem.Position != oldpos))
			{
				int i = 0;

				// Move selected geometry
				foreach(Vertex v in selectedverts)
				{
					// Move vertex from old position relative to the mouse position change since drag start
					v.Move(oldpositions[i++] + offset);
				}

				//mxd. Move selected things
				i = 0;
				foreach(Thing t in thingstodrag) 
				{
					t.Move(oldthingpositions[i++] + offset);
				}

				// Update labels
				int index = 0;
				foreach(Linedef l in unstablelines)
					labels[index++].Move(l.Start.Position, l.End.Position);

				// Moved
				return true;
			}

			// No changes
			return false;
		}

		// Cancelled
		public override void OnCancel()
		{
			// Move geometry back to original position
			MoveGeometryRelative(new Vector2D(0f, 0f), false, false, false, false);
			
			// Resume normal undo/redo recording
			General.Map.UndoRedo.IgnorePropChanges = false;

			// If only a single vertex was selected, deselect it now
			if(selectedverts.Count == 1) General.Map.Map.ClearSelectedVertices();
			
			// Update cached values
			General.Map.Map.Update();
			
			// Cancel base class
			base.OnCancel();
			
			// Return to vertices mode
			General.Editing.ChangeMode(General.Editing.PreviousStableMode.Name);
		}

		// Mode engages
		public override void OnEngage()
		{
			base.OnEngage();
			EnableAutoPanning();
			renderer.SetPresentation(Presentation.Standard);
		}
		
		// Disenagaging
		public override void OnDisengage()
		{
			base.OnDisengage();
			DisableAutoPanning();
			
			// When not cancelled
			if(!cancelled)
			{
				Cursor.Current = Cursors.AppStarting;
				
				// Move geometry back to original position
				MoveGeometryRelative(new Vector2D(0f, 0f), false, false, false, false);

				// Resume normal undo/redo recording
				General.Map.UndoRedo.IgnorePropChanges = false;

				// Make undo for the dragging
				General.Map.UndoRedo.CreateUndo(undodescription);

				// Move selected geometry to final position
				MoveGeometryRelative(mousemappos - dragstartmappos, snaptogrid, snaptogridincrement, snaptonearest, snaptocardinaldirection);

				// Stitch geometry
				if(snaptonearest) General.Map.Map.StitchGeometry(General.Settings.MergeGeometryMode);
				
				// Snap to map format accuracy
				General.Map.Map.SnapAllToAccuracy();

				//mxd. Update floor/ceiling texture offsets and slopes?
				if (General.Map.UDMF)
				{
					Vector2D offset = dragitem.Position - dragitemposition;

					// Sectors may've been created/removed when applying dragging...
					HashSet<Sector> draggedsectors = new HashSet<Sector>(General.Map.Map.GetMarkedSectors(true));
					foreach (Sector ss in selectedsectors) if (!ss.IsDisposed) draggedsectors.Add(ss);

					// Sectors that have to be updated. They contain the dragged sectors, but also 3D floor control sectors
					HashSet<Sector> updatesectors = new HashSet<Sector>(draggedsectors);
					// Keep track of control sectors in general, and control sectors relevant to other dragged sectors
					HashSet<Sector> controlsectors = new HashSet<Sector>();
					HashSet<Sector> addcontrolsectors = new HashSet<Sector>();

					// Check if the dragged sectors are referenced as 3D floors, and add the control sectors to the list of
					// sectors that need updating
					foreach (Linedef ld in General.Map.Map.Linedefs)
					{
						if (ld.Action != 160) // Action 160 defines a 3D floor
							continue;

						if (ld.Args[0] == 0) // First argument of the action is the sector tag. 0 is not a valid value
							continue;

						if (ld.Front == null) // Must have a front side
							continue;

						controlsectors.Add(ld.Front.Sector);

						foreach (Sector s in draggedsectors)
						{
							if (!s.Tags.Contains(ld.Args[0]))
								continue;

							updatesectors.Add(ld.Front.Sector);
							addcontrolsectors.Add(ld.Front.Sector);
						}
					}

					// Update floor/ceiling texture offsets?
					foreach (Sector s in updatesectors)
					{
						bool updateoffsets = (draggedsectors.Contains(s) && BuilderPlug.Me.LockSectorTextureOffsetsWhileDragging) || (!draggedsectors.Contains(s) && BuilderPlug.Me.Lock3DFloorSectorTextureOffsetsWhileDragging);
						bool updateslopes = !((controlsectors.Contains(s) ^ addcontrolsectors.Contains(s)) && draggedsectors.Contains(s));

						// Update texture offsets
						if (updateoffsets)
						{
							s.Fields.BeforeFieldsChange();

							// Update ceiling offset
							if (s.LongCeilTexture != MapSet.EmptyLongName)
							{
								ImageData texture = General.Map.Data.GetFlatImage(s.CeilTexture);

								if (texture != null)
								{
									double scalex = s.Fields.GetValue("xscaleceiling", 1.0);
									double scaley = s.Fields.GetValue("yscaleceiling", 1.0);

									if (scalex != 0 && scaley != 0)
									{
										Vector2D ceiloffset = new Vector2D(-offset.x, offset.y).GetRotated(-Angle2D.DegToRad((int)s.Fields.GetValue("rotationceiling", 0.0)));
										ceiloffset.x += s.Fields.GetValue("xpanningceiling", 0.0);
										ceiloffset.y += s.Fields.GetValue("ypanningceiling", 0.0);

										int texturewidth = (int)Math.Round(texture.Width / scalex);
										int textureheight = (int)Math.Round(texture.Height / scaley);

										if (!s.Fields.ContainsKey("xpanningceiling")) s.Fields.Add("xpanningceiling", new UniValue(UniversalType.Float, ceiloffset.x % texturewidth));
										else s.Fields["xpanningceiling"].Value = ceiloffset.x % texturewidth;

										if (!s.Fields.ContainsKey("ypanningceiling")) s.Fields.Add("ypanningceiling", new UniValue(UniversalType.Float, ceiloffset.y % textureheight));
										else s.Fields["ypanningceiling"].Value = ceiloffset.y % textureheight;
									}
								}
							}

							// Update floor offset
							if (s.LongFloorTexture != MapSet.EmptyLongName)
							{
								ImageData texture = General.Map.Data.GetFlatImage(s.FloorTexture);
								if (texture != null)
								{
									double scalex = s.Fields.GetValue("xscalefloor", 1.0);
									double scaley = s.Fields.GetValue("yscalefloor", 1.0);

									if (scalex != 0 && scaley != 0)
									{
										Vector2D flooroffset = new Vector2D(-offset.x, offset.y).GetRotated(-Angle2D.DegToRad((int)s.Fields.GetValue("rotationfloor", 0.0)));
										flooroffset.x += s.Fields.GetValue("xpanningfloor", 0.0);
										flooroffset.y += s.Fields.GetValue("ypanningfloor", 0.0);

										int texturewidth = (int)Math.Round(texture.Width / scalex);
										int textureheight = (int)Math.Round(texture.Height / scaley);

										if (!s.Fields.ContainsKey("xpanningfloor")) s.Fields.Add("xpanningfloor", new UniValue(UniversalType.Float, flooroffset.x % texturewidth));
										else s.Fields["xpanningfloor"].Value = flooroffset.x % texturewidth;

										if (!s.Fields.ContainsKey("ypanningfloor")) s.Fields.Add("ypanningfloor", new UniValue(UniversalType.Float, flooroffset.y % textureheight));
										else s.Fields["ypanningfloor"].Value = flooroffset.y % textureheight;
									}
								}
							}
						}

						if (updateslopes)
						{
							// Update floor slope?
							if (s.FloorSlope.GetLengthSq() > 0 && !double.IsNaN(s.FloorSlopeOffset / s.FloorSlope.z))
							{
								Plane floor = new Plane(s.FloorSlope, s.FloorSlopeOffset);
								Vector2D center = new Vector2D(s.BBox.X + s.BBox.Width / 2, s.BBox.Y + s.BBox.Height / 2);
								s.FloorSlopeOffset = -Vector3D.DotProduct(s.FloorSlope, new Vector3D(center + offset, floor.GetZ(center)));
							}

							// Update ceiling slope?
							if (s.CeilSlope.GetLengthSq() > 0 && !double.IsNaN(s.CeilSlopeOffset / s.CeilSlope.z))
							{
								Plane ceiling = new Plane(s.CeilSlope, s.CeilSlopeOffset);
								Vector2D center = new Vector2D(s.BBox.X + s.BBox.Width / 2, s.BBox.Y + s.BBox.Height / 2);
								s.CeilSlopeOffset = -Vector3D.DotProduct(s.CeilSlope, new Vector3D(center + offset, ceiling.GetZ(center)));
							}
						}
					}
				}

				// Update cached values
				General.Map.Map.Update();

				//mxd. Let the plugins know
				General.Editing.AcceptMode();

				// Done
				Cursor.Current = Cursors.Default;
				General.Map.IsChanged = true;
			}
		}

		// This checks if the view offset/zoom changed and updates the check
		protected bool CheckViewChanged()
		{
			// View changed?
			bool viewchanged = (renderer.OffsetX != lastoffsetx || renderer.OffsetY != lastoffsety || renderer.Scale != lastscale);

			// Keep view information
			lastoffsetx = renderer.OffsetX;
			lastoffsety = renderer.OffsetY;
			lastscale = renderer.Scale;

			// Return result
			return viewchanged;
		}

		// This updates the dragging
		private void Update()
		{
			snaptocardinaldirection = (General.Interface.ShiftState && General.Interface.AltState); //mxd
			snaptogrid = (snaptocardinaldirection || General.Interface.ShiftState ^ General.Interface.SnapToGrid);
			snaptonearest = General.Interface.CtrlState ^ General.Interface.AutoMerge;
			snaptogridincrement = (!snaptocardinaldirection && General.Interface.AltState); //mxd
			
			// Move selected geometry
			if(MoveGeometryRelative(mousemappos - dragstartmappos, snaptogrid, snaptogridincrement, snaptonearest, snaptocardinaldirection))
			{
				// Update cached values
				General.Map.Map.Update(true, false);

				// Redraw
				UpdateRedraw();
				renderer.Present();
			}
		}

		// This redraws only the required things
		protected virtual void UpdateRedraw() { }

		// When edit button is released
		protected override void OnEditEnd()
		{
			// Just return to base mode, Disengage will be called automatically.
			General.Editing.ChangeMode(General.Editing.PreviousStableMode.Name);

			base.OnEditEnd();
		}
		
		// Mouse moving
		public override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);

			//mxd. Skip most of update jazz while panning
			if(panning)
			{
				// Update labels
				int index = 0;
				foreach(Linedef l in unstablelines)
					labels[index++].Move(l.Start.Position, l.End.Position);
			}
			else
			{
				Update();
			}
		}
		// When a key is released
		public override void OnKeyUp(KeyEventArgs e)
		{
			base.OnKeyUp(e);
			if((snaptogrid != (General.Interface.ShiftState ^ General.Interface.SnapToGrid)) ||
			   (snaptonearest != (General.Interface.CtrlState ^ General.Interface.AutoMerge)) ||
			   (snaptogridincrement != General.Interface.AltState) ||
			   (snaptocardinaldirection != (General.Interface.AltState && General.Interface.ShiftState))) Update();
		}

		// When a key is pressed
		public override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);
			if((snaptogrid != (General.Interface.ShiftState ^ General.Interface.SnapToGrid)) ||
			   (snaptonearest != (General.Interface.CtrlState ^ General.Interface.AutoMerge)) ||
			   (snaptogridincrement != General.Interface.AltState) ||
			   (snaptocardinaldirection != (General.Interface.AltState && General.Interface.ShiftState))) Update();
		}
		
		#endregion
	}
}
