
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
using CodeImp.DoomBuilder.Windows;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Actions;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Editing;

#endregion

namespace CodeImp.DoomBuilder.VisualModes
{
	public enum PickingMode
	{
		Default,
		SidedefSlopeHandles,
		VertexSlopeHandles
	}

	/// <summary>
	/// Provides specialized functionality for a visual (3D) Doom Builder editing mode.
	/// </summary>
	public abstract class VisualMode : EditMode
	{
		#region ================== Constants

		private const double MOVE_SPEED_MULTIPLIER = 0.001;
		protected const float PICK_RANGE = 0.98f;
		private const float MOVE_CAMERA_DISTANCE = 64.0f;
		
		#endregion

		#region ================== Variables
		
		// Graphics
		protected IRenderer3D renderer;
		
		// Options
		private bool processgeometry;
		private bool processthings;
		
		// Input
		private bool keyforward;
		private bool keybackward;
		private bool keyleft;
		private bool keyright;
		private bool keyup;
		private bool keydown;
		private bool orbit;

		//mxd
		private List<VisualThing> selectedVisualThings;
		private List<VisualSector> selectedVisualSectors;
		protected Dictionary<Vertex, VisualVertexPair> vertices;
		private static Vector2D initialcameraposition;
		//used in "Play From Here" Action
		private Thing playerStart;
		private Vector3D playerStartPosition;
		private double playerStartAngle;

		private Vector3D orbitPoint;
		private double orbitXY;
		private double orbitZ;
		private double orbitRadius;
		private bool orbitPointPicked;

		// For picking
		protected PickingMode pickingmode;

		// Map
		protected VisualBlockMap blockmap;
		protected Dictionary<Thing, VisualThing> allthings;
		protected Dictionary<Sector, VisualSector> allsectors;
		protected Dictionary<Sector, List<VisualSlope>> allslopehandles;
		protected Dictionary<Sector, List<VisualSlope>> sidedefslopehandles;
		protected Dictionary<Sector, List<VisualSlope>> vertexslopehandles;
		protected List<VisualBlockEntry> visibleblocks;
		protected List<VisualThing> visiblethings;
		protected List<VisualSector> visiblesectors;
		protected List<VisualGeometry> visiblegeometry;
		protected HashSet<VisualSlope> usedslopehandles;

		#endregion

		#region ================== Properties

		public bool ProcessGeometry { get { return processgeometry; } set { processgeometry = value; } }
		public bool ProcessThings { get { return processthings; } set { processthings = value; } }
		public VisualBlockMap BlockMap { get { return blockmap; } }
		public Dictionary<Vertex, VisualVertexPair> VisualVertices { get { return vertices; } } //mxd
		public Dictionary<Sector, List<VisualSlope>> AllSlopeHandles { get { return allslopehandles; } }
		public Dictionary<Sector, List<VisualSlope>> SidedefSlopeHandles { get { return sidedefslopehandles; } }
		public Dictionary<Sector, List<VisualSlope>> VertexSlopeHandles { get { return vertexslopehandles; } }
		public HashSet<VisualSlope> UsedSlopeHandles { get { return usedslopehandles; } }

		// Rendering
		public IRenderer3D Renderer { get { return renderer; } }
		
		#endregion

		#region ================== Constructor / Disposer

		/// <summary>
		/// Provides specialized functionality for a visual (3D) Doom Builder editing mode.
		/// </summary>
		protected VisualMode()
		{
			// Initialize
			this.renderer = General.Map.Renderer3D;
			this.blockmap = new VisualBlockMap();
			this.allsectors = new Dictionary<Sector, VisualSector>(General.Map.Map.Sectors.Count);
			this.allthings = new Dictionary<Thing, VisualThing>(General.Map.Map.Things.Count);
			this.allslopehandles = new Dictionary<Sector, List<VisualSlope>>(General.Map.Map.Sectors.Count);
			this.sidedefslopehandles = new Dictionary<Sector, List<VisualSlope>>(General.Map.Map.Sectors.Count);
			this.vertexslopehandles = new Dictionary<Sector, List<VisualSlope>>(General.Map.Map.Sectors.Count);
			this.visibleblocks = new List<VisualBlockEntry>();
			this.visiblesectors = new List<VisualSector>(50);
			this.visiblegeometry = new List<VisualGeometry>(200);
			this.visiblethings = new List<VisualThing>(100);
			this.usedslopehandles = new HashSet<VisualSlope>();
			this.processgeometry = true;
			this.processthings = true;
			this.vertices = new Dictionary<Vertex, VisualVertexPair>(); //mxd
			this.pickingmode = PickingMode.Default;

			//mxd. Synch camera position to cursor position or center of the screen in 2d-mode
			if(General.Settings.GZSynchCameras && General.Editing.Mode is ClassicMode) 
			{
				ClassicMode oldmode = (ClassicMode)General.Editing.Mode;

				if(oldmode.IsMouseInside)
					initialcameraposition = new Vector2D(oldmode.MouseMapPos.x, oldmode.MouseMapPos.y);
				else
					initialcameraposition = new Vector2D(General.Map.CRenderer2D.Viewport.Left + General.Map.CRenderer2D.Viewport.Width / 2.0f, General.Map.CRenderer2D.Viewport.Top + General.Map.CRenderer2D.Viewport.Height / 2.0f);
			}
		}
		
		// Disposer
		public override void Dispose()
		{
			// Not already disposed?
			if(!isdisposed)
			{
				// Clean up
				foreach(KeyValuePair<Sector, VisualSector> s in allsectors) s.Value.Dispose();
				blockmap.Dispose();
				visiblesectors = null;
				visiblegeometry = null;
				visibleblocks = null;
				visiblethings = null;
				allsectors = null;
				allthings = null;
				blockmap = null;

				//mxd
				selectedVisualSectors = null;
				selectedVisualThings = null;
				
				// Done
				base.Dispose();
			}
		}

		#endregion

		#region ================== Start / Stop

		// Mode is engaged
		public override void OnEngage()
		{
			base.OnEngage();

			// Update projection (mxd)
			General.Map.CRenderer3D.CreateProjection();
			
			// Update the used textures
			General.Map.Data.UpdateUsedTextures();
			
			// Fill the blockmap
			FillBlockMap();

			//mxd. Synch camera position to cursor position or center of the screen in 2d-mode
			if(General.Settings.GZSynchCameras) 
			{
				// Keep previous camera position if Control is held and camera was previously moved in Visual mode
				if(!General.Interface.CtrlState || General.Map.VisualCamera.Position.GetLengthSq() == 0)
				{
					//If initial position is inside or nearby a sector - adjust camera.z accordingly
					double posz = General.Map.VisualCamera.Position.z;
					Sector nearestsector = General.Map.Map.GetSectorByCoordinates(initialcameraposition, blockmap);

					if(nearestsector == null)
					{
						Linedef nearestline = MapSet.NearestLinedef(General.Map.Map.Linedefs, initialcameraposition);
						if(nearestline != null)
						{
							double side = nearestline.SideOfLine(initialcameraposition);
							Sidedef nearestside = (side < 0.0f ? nearestline.Front : nearestline.Back) ?? (side < 0.0f ? nearestline.Back : nearestline.Front);
							if(nearestside != null) nearestsector = nearestside.Sector;
						}
					}

					if(nearestsector != null)
					{
						int sectorheight = nearestsector.CeilHeight - nearestsector.FloorHeight;
						if(sectorheight < 41)
							posz = nearestsector.FloorHeight + Math.Max(16, sectorheight / 2);
						else if(General.Map.VisualCamera.Position.z < nearestsector.FloorHeight + 41)
							posz = nearestsector.FloorHeight + 41; // same as in doom
						else if(General.Map.VisualCamera.Position.z > nearestsector.CeilHeight)
							posz = nearestsector.CeilHeight - 4;
					}

					General.Map.VisualCamera.Position = new Vector3D(initialcameraposition.x, initialcameraposition.y, posz);
				}
			} 
			else 
			{
				General.Map.VisualCamera.PositionAtThing();
			}
			
			// Start special input mode
			General.Interface.EnableProcessing();
			General.Interface.StartExclusiveMouseInput();
		}

		// Mode is disengaged
		public override void OnDisengage()
		{
			base.OnDisengage();
			
			// Dispose
			foreach(KeyValuePair<Sector, VisualSector> vs in allsectors)
				if(vs.Value != null) vs.Value.Dispose();

			// Dispose
			foreach(KeyValuePair<Thing, VisualThing> vt in allthings)
				if(vt.Value != null) vt.Value.Dispose();

			// Apply camera position to thing
			General.Map.VisualCamera.ApplyToThing();
			
			// Do not leave the sector on the camera
			General.Map.VisualCamera.Sector = null;

			//mxd
			selectedVisualSectors = null;
			selectedVisualThings = null;

			//mxd. Extra floors may've been edited
			General.Map.Renderer2D.UpdateExtraFloorFlag();
			
			// Stop special input mode
			General.Interface.DisableProcessing();
			General.Interface.StopExclusiveMouseInput();
		}

		#endregion

		#region ================== Events

		public override bool OnUndoBegin()
		{
			renderer.SetCrosshairBusy(true);
			General.Interface.RedrawDisplay();
			return base.OnUndoBegin();
		}

		public override void OnUndoEnd()
		{
			base.OnUndoEnd();
			ResourcesReloadedPartial();
			renderer.SetCrosshairBusy(false);
		}

		public override bool OnRedoBegin()
		{
			renderer.SetCrosshairBusy(true);
			General.Interface.RedrawDisplay();
			return base.OnRedoBegin();
		}

		public override void OnRedoEnd()
		{
			base.OnRedoEnd();
			ResourcesReloadedPartial();
			renderer.SetCrosshairBusy(false);
		}

		public override bool OnScriptRunBegin()
		{
			renderer.SetCrosshairBusy(true);
			return base.OnScriptRunBegin();
		}

		public override void OnScriptRunEnd()
		{
			base.OnScriptRunEnd();
			ResourcesReloadedPartial();
			renderer.SetCrosshairBusy(false);
		}

		public override void OnReloadResources()
		{
			base.OnReloadResources();
			ResourcesReloaded();
		}

		//mxd
		public override bool OnMapTestBegin(bool testFromCurrentPosition) 
		{
			if(testFromCurrentPosition) 
			{
				//find Single Player Start. Should have Type 1 in all games
				Thing start = null;
				foreach(Thing t in General.Map.Map.Things) 
				{
					if(t.Type == 1) 
					{
						//store thing and position
						start = t;
						break;
					}
				}

				if(start == null) 
				{
					General.MainWindow.DisplayStatus(StatusType.Warning, "Can't test from current position: no Player 1 start found!");
					return false;
				}

				//now check if camera is located inside a sector
				Vector3D camPos = General.Map.VisualCamera.Position;
				Sector s = General.Map.Map.GetSectorByCoordinates(new Vector2D(camPos.x, camPos.y), blockmap);

				if(s == null) 
				{
					General.MainWindow.DisplayStatus(StatusType.Warning, "Can't test from current position: cursor is not inside sector!");
					return false;
				}

				//41 = player's height in Doom. Is that so in all other games as well?
				if(s.CeilHeight - s.FloorHeight < 41) 
				{
					General.MainWindow.DisplayStatus(StatusType.Warning, "Can't test from current position: sector is too low!");
					return false;
				}

				//check camera Z
				double pz = camPos.z - s.FloorHeight;
				int ceilRel = s.CeilHeight - s.FloorHeight - 41; //relative ceiling height
				if(pz > ceilRel) pz = ceilRel; //above ceiling?
				else if(pz < 0) pz = 0; //below floor?

				//store initial position
				playerStart = start;
				playerStartPosition = start.Position;
				playerStartAngle = start.Angle;

				//everything should be valid, let's move player start here
				start.Move(new Vector3D(camPos.x, camPos.y, pz));
				start.Rotate(General.Map.VisualCamera.AngleXY - Angle2D.PI);// (float)Math.PI);
			}
			return true;
		}

		//mxd
		public override void OnMapTestEnd(bool testFromCurrentPosition) 
		{
			if(testFromCurrentPosition) 
			{
				//restore position
				playerStart.Move(playerStartPosition);
				playerStart.Rotate(playerStartAngle);
				playerStart = null;
			}
		}
		
		#endregion
		
		#region ================== Input

		// Mouse input
		public override void OnMouseInput(Vector2D delta)
		{
			base.OnMouseInput(delta);

			if (orbit)
			{
				Vector3D start = General.Map.VisualCamera.Position;
				
				if (!orbitPointPicked)
				{
					Vector3D hitPosition = GetHitPosition();
					orbitPoint = hitPosition;
					
					if (!hitPosition.IsFinite())
					{
						return;
					}
					
					orbitPointPicked = true;

					Vector3D pickDelta = start - orbitPoint;
					orbitRadius = pickDelta.GetLength();
					orbitXY = pickDelta.GetNormal().GetAngleXY();
					orbitZ = -pickDelta.GetNormal().GetAngleZ();
				}
				
				// Change camera angles with the mouse changes
				orbitXY -= delta.x * VisualCamera.ANGLE_FROM_MOUSE;
				if(General.Settings.InvertYAxis)
					orbitZ -= delta.y * VisualCamera.ANGLE_FROM_MOUSE;
				else
					orbitZ += delta.y * VisualCamera.ANGLE_FROM_MOUSE;
				
				orbitXY = Angle2D.Normalized(orbitXY);
				orbitZ = Angle2D.Normalized(orbitZ);

				if (orbitZ < VisualCamera.MAX_ANGLEZ_LOW) orbitZ = VisualCamera.MAX_ANGLEZ_LOW;
				if (orbitZ > VisualCamera.MAX_ANGLEZ_HIGH) orbitZ = VisualCamera.MAX_ANGLEZ_HIGH;
				
				Vector3D orbitDelta = Vector3D.FromAngleXYZ(orbitXY, orbitZ);

				Vector3D newPosition = orbitPoint - orbitDelta * orbitRadius;
				Vector3D positionUpdate = newPosition - General.Map.VisualCamera.Position;

				General.Map.VisualCamera.Position += positionUpdate * General.Map.VisualCamera.MoveMultiplier;
				start = General.Map.VisualCamera.Position;
				
				// Recalculate angles to ensure we're always looking at the orbit point
				Vector3D updatedDelta = start - orbitPoint;
				orbitXY = updatedDelta.GetNormal().GetAngleXY();
				orbitZ = -updatedDelta.GetNormal().GetAngleZ();
				
				General.Map.VisualCamera.AngleZ = orbitZ;
				General.Map.VisualCamera.AngleXY = orbitXY;

				General.Map.VisualCamera.ProcessMouseInput(new Vector2D());
			}
			else
			{
				General.Map.VisualCamera.ProcessMouseInput(delta);	
			}
		}

		[BeginAction("moveforward", BaseAction = true)]
		public virtual void BeginMoveForward()
		{
			keyforward = true;
		}

		[EndAction("moveforward", BaseAction = true)]
		public virtual void EndMoveForward()
		{
			keyforward = false;
		}

		[BeginAction("movebackward", BaseAction = true)]
		public virtual void BeginMoveBackward()
		{
			keybackward = true;
		}

		[EndAction("movebackward", BaseAction = true)]
		public virtual void EndMoveBackward()
		{
			keybackward = false;
		}

		[BeginAction("moveleft", BaseAction = true)]
		public virtual void BeginMoveLeft()
		{
			keyleft = true;
		}

		[EndAction("moveleft", BaseAction = true)]
		public virtual void EndMoveLeft()
		{
			keyleft = false;
		}

		[BeginAction("moveright", BaseAction = true)]
		public virtual void BeginMoveRight()
		{
			keyright = true;
		}

		[EndAction("moveright", BaseAction = true)]
		public virtual void EndMoveRight()
		{
			keyright = false;
		}
		
		[BeginAction("orbit", BaseAction = true)]
		public virtual void BeginOrbit()
		{
			orbit = true;
		}

		[EndAction("orbit", BaseAction = true)]
		public virtual void EndOrbit()
		{
			orbit = false;
			orbitPointPicked = false;
		}

		[BeginAction("moveup", BaseAction = true)]
		public virtual void BeginMoveUp()
		{
			keyup = true;
		}

		[EndAction("moveup", BaseAction = true)]
		public virtual void EndMoveUp()
		{
			keyup = false;
		}

		[BeginAction("movedown", BaseAction = true)]
		public virtual void BeginMoveDown()
		{
			keydown = true;
		}

		[EndAction("movedown", BaseAction = true)]
		public virtual void EndMoveDown()
		{
			keydown = false;
		}
		
		[BeginAction("movecameratocursor", BaseAction = true)]
		protected virtual void MoveCameraToCursor() 
		{
			if (orbit)
			{
				orbitRadius = MOVE_CAMERA_DISTANCE;
			}
			else
			{
				Vector3D hitPosition = GetHitPosition();
				if (!hitPosition.IsFinite())
				{
					return;
				}

				Vector3D start = General.Map.VisualCamera.Position;
				Vector3D delta = start - hitPosition;
				General.Map.VisualCamera.Position = hitPosition + delta.GetFixedLength(MOVE_CAMERA_DISTANCE);
			}
		}


		//mxd
		[BeginAction("movethingleft", BaseAction = true)]
		protected void MoveSelectedThingsLeft() 
		{
			MoveSelectedThings(new Vector2D(0f, -General.Map.Grid.GridSizeF), false);
		}
		//mxd
		[BeginAction("movethingright", BaseAction = true)]
		protected void MoveSelectedThingsRight() 
		{
			MoveSelectedThings(new Vector2D(0f, General.Map.Grid.GridSizeF), false);
		}
		//mxd
		[BeginAction("movethingfwd", BaseAction = true)]
		protected void MoveSelectedThingsForward() 
		{
			MoveSelectedThings(new Vector2D(-General.Map.Grid.GridSizeF, 0f), false);
		}
		//mxd
		[BeginAction("movethingback", BaseAction = true)]
		protected void MoveSelectedThingsBackward() 
		{
			MoveSelectedThings(new Vector2D(General.Map.Grid.GridSizeF, 0f), false);
		}

		//mxd
		[BeginAction("placethingatcursor", BaseAction = true)]
		protected void PlaceThingAtCursor() 
		{
			Vector2D hitpos = GetHitPosition();
			if(!hitpos.IsFinite()) 
			{
				General.Interface.DisplayStatus(StatusType.Warning, "Cannot place Thing here");
				return;
			}

			MoveSelectedThings(new Vector2D(Math.Round(hitpos.x), Math.Round(hitpos.y)), true);
		}

		//mxd. 
		public Vector3D GetHitPosition() 
		{
			Vector3D start = General.Map.VisualCamera.Position;
			Vector3D delta = General.Map.VisualCamera.Target - General.Map.VisualCamera.Position;
			delta = delta.GetFixedLength(General.Settings.ViewDistance * 0.98f);
			VisualPickResult target = PickObject(start, start + delta);

			if(target.picked == null) return new Vector3D(double.NaN, double.NaN, double.NaN);

			// Now find where exactly did we hit
			VisualGeometry vg = target.picked as VisualGeometry;
			if(vg != null) return GetIntersection(start, start + delta, vg.BoundingBox[0], new Vector3D(vg.Vertices[0].nx, vg.Vertices[0].ny, vg.Vertices[0].nz));


			VisualThing vt = target.picked as VisualThing;
			if (vt != null)
			{
				Vector3D normal = start - vt.CenterV3D;
				normal.z = 0;
				normal = normal.GetNormal();
				return GetIntersection(start, start + delta, vt.CenterV3D, normal);
			}

			return new Vector3D(double.NaN, double.NaN, double.NaN);
		}

		//mxd. This checks intersection between line and plane 
		private static Vector3D GetIntersection(Vector3D start, Vector3D end, Vector3D planeCenter, Vector3D planeNormal) 
		{
			Vector3D delta = new Vector3D(planeCenter.x - start.x, planeCenter.y - start.y, planeCenter.z - start.z);
			return start + Vector3D.DotProduct(planeNormal, delta) / Vector3D.DotProduct(planeNormal, end - start) * (end - start);
		}

		//mxd. Should move selected things in specified direction
		protected virtual void MoveSelectedThings(Vector2D direction, bool absolutePosition) { }

        #endregion

        #region ================== Visibility Culling

        int lastProcessed = 0;

		// This preforms visibility culling
		protected void DoCulling()
		{
            lastProcessed++;
			List<Linedef> visiblelines = new List<Linedef>();
			Vector2D campos2d = General.Map.VisualCamera.Position;
			
			// Make collections
			visiblesectors = new List<VisualSector>(visiblesectors.Count * 2);
			visiblegeometry = new List<VisualGeometry>(visiblegeometry.Count * 2);
			visiblethings = new List<VisualThing>(visiblethings.Count * 2);

			// Get the blocks within view range
			visibleblocks = blockmap.GetFrustumRange(renderer.Frustum2D);
			
			// Fill collections with geometry and things
			foreach(VisualBlockEntry block in visibleblocks)
			{
				if(processgeometry)
				{
					// Lines
					foreach(Linedef ld in block.Lines)
					{
						// Line not already processed?
						if (ld.LastProcessed != lastProcessed)
						{
                            // Add line if not added yet
                            ld.LastProcessed = lastProcessed;
                            visiblelines.Add(ld);

							// Which side of the line is the camera on?
							if(ld.SideOfLine(campos2d) < 0)
							{
								// Do front of line
								if(ld.Front != null) ProcessSidedefCulling(ld.Front);
							}
							else
							{
								// Do back of line
								if(ld.Back != null) ProcessSidedefCulling(ld.Back);
							}
						}
					}
				}

				if(processthings)
				{
					// Things
					foreach(Thing t in block.Things)
					{
                        if (t.LastProcessed == lastProcessed) continue;

                        t.LastProcessed = lastProcessed;

						// Not filtered out?
						if(!General.Map.ThingsFilter.IsThingVisible(t)) continue;

						VisualThing vt;
						if(allthings.ContainsKey(t))
						{
							vt = allthings[t];
						}
						else
						{
							// Create new visual thing
							vt = CreateVisualThing(t);
							allthings[t] = vt;
						}

						if(vt != null)
						{
							visiblethings.Add(vt);
						}
					}
				}
			}

			if(processgeometry)
			{
                // Find camera sector
                Sector camsector = blockmap.GetSectorAt(campos2d);
                if (camsector != null)
                {
                    General.Map.VisualCamera.Sector = camsector;
                }
                else
                {
                    // To do: fix this code. It is retarded. Walking over all visible lines is extremely expensive.

                    Linedef nld = MapSet.NearestLinedef(visiblelines, campos2d);
                    if (nld != null)
                    {
                        General.Map.VisualCamera.Sector = GetCameraSectorFromLinedef(nld);
                    }
                    else
                    {
                        // Exceptional case: no lines found in any nearby blocks!
                        // This could happen in the middle of an extremely large sector and in this case
                        // the above code will not have found any sectors/sidedefs for rendering.
                        // Here we handle this special case with brute-force. Let's find the sector
                        // the camera is in by searching the entire map and render that sector only.
                        nld = General.Map.Map.NearestLinedef(campos2d);
                        if (nld != null)
                        {
                            General.Map.VisualCamera.Sector = GetCameraSectorFromLinedef(nld);
                            if (General.Map.VisualCamera.Sector != null)
                            {
                                foreach (Sidedef sd in General.Map.VisualCamera.Sector.Sidedefs)
                                {
									double side = sd.Line.SideOfLine(campos2d);
                                    if (((side < 0) && sd.IsFront) ||
                                       ((side > 0) && !sd.IsFront))
                                        ProcessSidedefCulling(sd);
                                }
                            }
                            else
                            {
                                // Too far away from the map to see anything
                                General.Map.VisualCamera.Sector = null;
                            }
                        }
                        else
                        {
                            // Map is empty
                            General.Map.VisualCamera.Sector = null;
                        }
                    }
                }
			}
		}

		// This finds and adds visible sectors
		private void ProcessSidedefCulling(Sidedef sd)
		{
            // Do nothing if we already added it.
			if (sd.LastProcessed == lastProcessed)
                return;

            sd.LastProcessed = lastProcessed;

            VisualSector vs;
			
			// Find the visualsector and make it if needed
			if(allsectors.ContainsKey(sd.Sector))
			{
				// Take existing visualsector
				vs = allsectors[sd.Sector];
			}
			else
			{
				// Make new visualsector
				vs = CreateVisualSector(sd.Sector);
				//if(vs != null) allsectors.Add(sd.Sector, vs); //mxd
			}
			
			if(vs != null)
			{
                if (sd.Sector.LastProcessed != lastProcessed)
                {
                    sd.Sector.LastProcessed = lastProcessed;
                    visiblesectors.Add(vs);
                    visiblegeometry.AddRange(vs.FixedGeometry);
                }
				
				// Add sidedef geometry
				visiblegeometry.AddRange(vs.GetSidedefGeometry(sd));
			}
		}

		// This returns the camera sector from linedef
		private static Sector GetCameraSectorFromLinedef(Linedef ld)
		{
			if(ld.SideOfLine(General.Map.VisualCamera.Position) < 0)
			{
				return (ld.Front != null ? ld.Front.Sector : null);
			}

			return (ld.Back != null ? ld.Back.Sector : null);
		}
		
		#endregion

		#region ================== Object Picking

		// This picks an object from the scene
		public VisualPickResult PickObject(Vector3D from, Vector3D to)
		{
			VisualPickResult result = new VisualPickResult();
			Line2D ray2d = new Line2D(from, to);
			Vector3D delta = to - from;
			
			// Setup no result
			result.picked = null;
			result.hitpos = new Vector3D();
			result.u_ray = 1.0f;
			
			// Find all blocks we are intersecting
			List<VisualBlockEntry> blocks = blockmap.GetLineBlocks(from, to);
			
			// Make collections
			Dictionary<Linedef, Linedef> lines = new Dictionary<Linedef, Linedef>(blocks.Count * 10);
			Dictionary<Sector, VisualSector> sectors = new Dictionary<Sector, VisualSector>(blocks.Count * 10);
			List<IVisualPickable> pickables = new List<IVisualPickable>(blocks.Count * 10);
			
			// Add geometry from the camera sector
			if((General.Map.VisualCamera.Sector != null) && allsectors.ContainsKey(General.Map.VisualCamera.Sector))
			{
				VisualSector vs = allsectors[General.Map.VisualCamera.Sector];
				sectors.Add(General.Map.VisualCamera.Sector, vs);
				foreach(VisualGeometry g in vs.FixedGeometry) pickables.Add(g);

				// Add slope handles
				if (General.Map.UDMF)
				{
					if (pickingmode == PickingMode.SidedefSlopeHandles && sidedefslopehandles.ContainsKey(General.Map.VisualCamera.Sector))
						pickables.AddRange(sidedefslopehandles[General.Map.VisualCamera.Sector]);
					else if (pickingmode == PickingMode.VertexSlopeHandles && vertexslopehandles.ContainsKey(General.Map.VisualCamera.Sector))
						pickables.AddRange(vertexslopehandles[General.Map.VisualCamera.Sector]);
				}
			}
			
			// Go for all lines to see which ones we intersect
			// We will collect geometry from the sectors and sidedefs
			foreach(VisualBlockEntry b in blocks)
			{
				foreach(Linedef ld in b.Lines)
				{
					// Make sure we don't test a line twice
					if(!lines.ContainsKey(ld))
					{
						lines.Add(ld, ld);

						// Intersecting?
						double u;
						if(ld.Line.GetIntersection(ray2d, out u))
						{
							// Check on which side we are
							double side = ld.SideOfLine(ray2d.v1);
							
							// Calculate intersection point
							Vector3D intersect = from + delta * u;
							
							// We must add the sectors of both sides of the line
							// If we wouldn't, then aiming at a sector that is just within range
							// could result in an incorrect hit (because the far line of the
							// sector may not be included in this loop)
							if(ld.Front != null)
							{
								// Find the visualsector
								if(allsectors.ContainsKey(ld.Front.Sector))
								{
									VisualSector vs = allsectors[ld.Front.Sector];
									
									// Add sector if not already added
									if(!sectors.ContainsKey(ld.Front.Sector))
									{
										sectors.Add(ld.Front.Sector, vs);
										foreach (VisualGeometry g in vs.FixedGeometry)
										{
											// Must have content
											if (g.Triangles > 0)
												pickables.Add(g);
										}

										// Add slope handles
										if (General.Map.UDMF)
										{
											if (pickingmode == PickingMode.SidedefSlopeHandles && sidedefslopehandles.ContainsKey(ld.Front.Sector))
												pickables.AddRange(sidedefslopehandles[ld.Front.Sector]);
											else if (pickingmode == PickingMode.VertexSlopeHandles && vertexslopehandles.ContainsKey(ld.Front.Sector))
												pickables.AddRange(vertexslopehandles[ld.Front.Sector]);
										}
									}
									
									// Add sidedef if on the front side
									if(side < 0.0f)
									{
										List<VisualGeometry> sidedefgeo = vs.GetSidedefGeometry(ld.Front);
										foreach(VisualGeometry g in sidedefgeo)
										{
											// Must have content
											if(g.Triangles > 0)
											{
												g.SetPickResults(intersect, u);
												pickables.Add(g);
											}
										}
									}
								}
							}
							
							// Add back side also
							if(ld.Back != null)
							{
								// Find the visualsector
								if(allsectors.ContainsKey(ld.Back.Sector))
								{
									VisualSector vs = allsectors[ld.Back.Sector];

									// Add sector if not already added
									if(!sectors.ContainsKey(ld.Back.Sector))
									{
										sectors.Add(ld.Back.Sector, vs);
										foreach (VisualGeometry g in vs.FixedGeometry)
										{
											// Must have content
											if (g.Triangles > 0)
												pickables.Add(g);
										}

										// Add slope handles
										if (General.Map.UDMF)
										{
											if (pickingmode == PickingMode.SidedefSlopeHandles && sidedefslopehandles.ContainsKey(ld.Back.Sector))
												pickables.AddRange(sidedefslopehandles[ld.Back.Sector]);
											else if (pickingmode == PickingMode.VertexSlopeHandles && vertexslopehandles.ContainsKey(ld.Back.Sector))
												pickables.AddRange(vertexslopehandles[ld.Back.Sector]);
										}
									}

									// Add sidedef if on the front side
									if(side > 0.0f)
									{
										List<VisualGeometry> sidedefgeo = vs.GetSidedefGeometry(ld.Back);
										foreach(VisualGeometry g in sidedefgeo)
										{
											// Must have content
											if(g.Triangles > 0)
											{
												g.SetPickResults(intersect, u);
												pickables.Add(g);
											}
										}
									}
								}
							}
						}
					}
				}
			}
			
			// Add all the visible things
			foreach(VisualThing vt in visiblethings) pickables.Add(vt);

			//mxd. And all visual vertices
			if (General.Map.UDMF && General.Settings.GZShowVisualVertices)
			{
				foreach (KeyValuePair<Vertex, VisualVertexPair> pair in vertices)
					pickables.AddRange(pair.Value.Vertices);
			}
			
			// Now we have a list of potential geometry that lies along the trace line.
			// We still don't know what geometry actually hits, but we ruled out that which doesn't get even close.
			// This is still too much for accurate intersection testing, so we do a fast reject pass first.
			Vector3D direction = to - from;
			direction = direction.GetNormal();
			List<IVisualPickable> potentialpicks = new List<IVisualPickable>(pickables.Count);
			foreach(IVisualPickable p in pickables)
			{
				if(p.PickFastReject(from, to, direction)) potentialpicks.Add(p);
			}
			
			// Now we do an accurate intersection test for all resulting geometry
			// We keep only the closest hit!
			foreach(IVisualPickable p in potentialpicks)
			{
				double u = result.u_ray;
				if(p.PickAccurate(from, to, direction, ref u))
				{
					// Closer than previous find?
					if((u > 0.0f) && (u < result.u_ray))
					{
						result.u_ray = u;
						result.picked = p;
					}
				}
			}
			
			// Setup final result
			result.hitpos = from + to * result.u_ray;

			// If picking mode is for slope handles only return slope handles. We have to do it this
			// way because otherwise it's possible to pick slope handles through other geometry
			if (pickingmode != PickingMode.Default && !(result.picked is VisualSlope))
				result.picked = null;

			// Done
			return result;
		}
		
		#endregion

		#region ================== Processing
		
		/// <summary>
		/// This disposes all resources. Needed geometry will be rebuild automatically.
		/// </summary>
		protected virtual void ResourcesReloaded()
		{
			// Dispose
			foreach(KeyValuePair<Sector, VisualSector> vs in allsectors)
				if(vs.Value != null) vs.Value.Dispose();
				
			foreach(KeyValuePair<Thing, VisualThing> vt in allthings)
				if(vt.Value != null) vt.Value.Dispose();
				
			// Clear collections
			allsectors.Clear();
			allthings.Clear();
			visiblesectors.Clear();
			visibleblocks.Clear();
			visiblegeometry.Clear();
			visiblethings.Clear();
			vertices.Clear(); //mxd
			
			// Make new blockmap
			FillBlockMap();

			// Visibility culling (this re-creates the needed resources)
			DoCulling();
		}

		/// <summary>
		/// This disposes orphaned resources and resources on changed geometry.
		/// This usually happens when geometry is changed by undo, redo, cut or paste actions
		/// and uses the marks to check what needs to be reloaded.
		/// </summary>
		protected virtual void ResourcesReloadedPartial()
		{
			Dictionary<Sector, VisualSector> newsectors = new Dictionary<Sector,VisualSector>(allsectors.Count);
			
			// Neighbour sectors must be updated as well
			foreach(Sector s in General.Map.Map.Sectors)
			{
				if(s.Marked)
				{
					foreach(Sidedef sd in s.Sidedefs)
						if(sd.Other != null) sd.Other.Marked = true;
				}
			}
			
			// Go for all sidedefs to mark sectors that need updating
			foreach(Sidedef sd in General.Map.Map.Sidedefs)
				if(sd.Marked) sd.Sector.Marked = true;
			
			// Go for all vertices to mark linedefs that need updating
			foreach(Vertex v in General.Map.Map.Vertices)
			{
				if(v.Marked)
				{
					foreach(Linedef ld in v.Linedefs)
						ld.Marked = true;
				}
			}
			
			// Go for all linedefs to mark sectors that need updating
			foreach(Linedef ld in General.Map.Map.Linedefs)
			{
				if(ld.Marked)
				{
					if(ld.Front != null) ld.Front.Sector.Marked = true;
					if(ld.Back != null) ld.Back.Sector.Marked = true;
				}
			}
			
			// Dispose if source was disposed or marked
			foreach(KeyValuePair<Sector, VisualSector> vs in allsectors)
			{
				if(vs.Value != null)
				{
					if(vs.Key.IsDisposed || vs.Key.Marked)
						vs.Value.Dispose();
					else
						newsectors.Add(vs.Key, vs.Value);
				}
			}
			
			// Things depend on the sector they are in and because we can't
			// easily determine which ones changed, we dispose all things
			foreach(KeyValuePair<Thing, VisualThing> vt in allthings)
				if(vt.Value != null) vt.Value.Dispose();
			
			// Apply new lists
			allsectors = newsectors;
			allthings = new Dictionary<Thing, VisualThing>(allthings.Count);
			
			// Clear visibility collections
			visiblesectors.Clear();
			visibleblocks.Clear();
			visiblegeometry.Clear();
			visiblethings.Clear();
			
			// Make new blockmap
			FillBlockMap();
			
			// Visibility culling (this re-creates the needed resources)
			DoCulling();
		}
		
		/// <summary>
		/// Implement this to create an instance of your VisualSector implementation.
		/// </summary>
		protected abstract VisualSector CreateVisualSector(Sector s);

		/// <summary>
		/// Implement this to create an instance of your VisualThing implementation.
		/// </summary>
		protected abstract VisualThing CreateVisualThing(Thing t);
		
		/// <summary>
		/// This returns the VisualSector for the given Sector.
		/// </summary>
		public VisualSector GetVisualSector(Sector s) 
		{
			if(!allsectors.ContainsKey(s)) return CreateVisualSector(s); //mxd
			return allsectors[s]; 
		}
		
		/// <summary>
		/// This returns the VisualThing for the given Thing.
		/// </summary>
		public VisualThing GetVisualThing(Thing t) { return allthings[t]; }

		//mxd
		public List<VisualThing> GetSelectedVisualThings(bool refreshSelection) 
		{
			if(refreshSelection || selectedVisualThings == null) 
			{
				selectedVisualThings = new List<VisualThing>();
				foreach(KeyValuePair<Thing, VisualThing> group in allthings) 
				{
					if(group.Value != null && group.Value.Selected)
						selectedVisualThings.Add(group.Value);
				}

				//if nothing is selected - try to get thing from hilighted object
				if(selectedVisualThings.Count == 0) 
				{
					Vector3D start = General.Map.VisualCamera.Position;
					Vector3D delta = General.Map.VisualCamera.Target - General.Map.VisualCamera.Position;
					delta = delta.GetFixedLength(General.Settings.ViewDistance * 0.98f);
					VisualPickResult target = PickObject(start, start + delta);

					//not appropriate way to do this, but...
					if(target.picked is VisualThing)
						selectedVisualThings.Add((VisualThing)target.picked);
				}
			}

			return selectedVisualThings;
		}

		/// <summary>
		/// mxd. This returns list of selected sectors based on surfaces selected in visual mode
		/// </summary>
		public List<VisualSector> GetSelectedVisualSectors(bool refreshSelection) 
		{
			if(refreshSelection || selectedVisualSectors == null) 
			{
				selectedVisualSectors = new List<VisualSector>();
				foreach(KeyValuePair<Sector, VisualSector> group in allsectors) 
				{
					foreach(VisualGeometry vg in group.Value.AllGeometry) 
					{
						if(vg.Selected) 
						{
							selectedVisualSectors.Add(group.Value);
							break;
						}
					}
				}

				//if nothing is selected - try to get sector from hilighted object
				if(selectedVisualSectors.Count == 0) 
				{
					VisualGeometry vg = GetHilightedSurface();
					if(vg != null) selectedVisualSectors.Add(vg.Sector);
				}
			}
			return selectedVisualSectors;
		}

		/// <summary>
		/// mxd. This returns list of surfaces selected in visual mode
		/// </summary>
		public List<VisualGeometry> GetSelectedSurfaces() 
		{
			List<VisualGeometry> selectedSurfaces = new List<VisualGeometry>();
			foreach(KeyValuePair<Sector, VisualSector> group in allsectors) 
			{
				foreach(VisualGeometry vg in group.Value.AllGeometry) 
				{
					if(vg.Selected) selectedSurfaces.Add(vg);
				}
			}

			//if nothing is selected - try to get hilighted surface
			if(selectedSurfaces.Count == 0) 
			{
				VisualGeometry vg = GetHilightedSurface();
				if(vg != null) selectedSurfaces.Add(vg);
			}
			return selectedSurfaces;
		}

		//mxd
		private VisualGeometry GetHilightedSurface() 
		{
			Vector3D start = General.Map.VisualCamera.Position;
			Vector3D delta = General.Map.VisualCamera.Target - General.Map.VisualCamera.Position;
			delta = delta.GetFixedLength(General.Settings.ViewDistance * 0.98f);
			VisualPickResult target = PickObject(start, start + delta);

			if(target.picked is VisualGeometry) 
			{
				VisualGeometry vg = (VisualGeometry)target.picked;
				if(vg.Sector != null) return vg;
			}
			return null;
		}

		/// <summary>
		/// Returns True when a VisualSector has been created for the specified Sector.
		/// </summary>
		public bool VisualSectorExists(Sector s) { return allsectors.ContainsKey(s) && (allsectors[s] != null); }

		/// <summary>
		/// Returns True when a VisualThing has been created for the specified Thing.
		/// </summary>
		public bool VisualThingExists(Thing t) { return allthings.ContainsKey(t) && (allthings[t] != null); }

		/// <summary>
		/// This is called when the blockmap needs to be refilled, because it was invalidated.
		/// This usually happens when geometry is changed by undo, redo, cut or paste actions.
		/// Lines and Things are added to the block map by the base implementation.
		/// </summary>
		protected virtual void FillBlockMap()
		{
			blockmap.Clear();//mxd
			blockmap.AddLinedefsSet(General.Map.Map.Linedefs);
			blockmap.AddThingsSet(General.Map.Map.Things);
			blockmap.AddSectorsSet(General.Map.Map.Sectors);
		}
		
		/// <summary>
		/// While this mode is active, this is called continuously to process whatever needs processing.
		/// </summary>
		public override void OnProcess(long deltatime)
		{
			base.OnProcess(deltatime);
			
			// Camera vectors
			Vector3D camvec = Vector3D.FromAngleXYZ(General.Map.VisualCamera.AngleXY, General.Map.VisualCamera.AngleZ);
			Vector3D camvecstrafe = Vector3D.FromAngleXY(General.Map.VisualCamera.AngleXY + Angle2D.PIHALF);
			Vector3D cammovemul = General.Map.VisualCamera.MoveMultiplier;
			Vector3D camdeltapos = new Vector3D();
			Vector3D upvec = new Vector3D(0.0, 0.0, 1.0);

			double multiplier;
			if(General.Interface.ShiftState) multiplier = MOVE_SPEED_MULTIPLIER * 2.0f; else multiplier = MOVE_SPEED_MULTIPLIER;
			
			if (orbit)
			{
				if (keyforward) orbitRadius -= General.Settings.MoveSpeed * multiplier * deltatime;
				if (keybackward) orbitRadius += General.Settings.MoveSpeed * multiplier * deltatime;

				if (orbitRadius < 1) orbitRadius = 1f;

				General.Map.VisualCamera.ProcessMovement(new Vector3D());
			}
			else
			{
				// Move the camera
				
				if(keyforward) camdeltapos += camvec * cammovemul * General.Settings.MoveSpeed * multiplier * deltatime;
				if(keybackward) camdeltapos -= camvec * cammovemul * General.Settings.MoveSpeed * multiplier * deltatime;
				if(keyleft) camdeltapos -= camvecstrafe * cammovemul * General.Settings.MoveSpeed * multiplier * deltatime;
				if(keyright) camdeltapos += camvecstrafe * cammovemul * General.Settings.MoveSpeed * multiplier * deltatime;
				if(keyup) camdeltapos += upvec * cammovemul * General.Settings.MoveSpeed * multiplier * deltatime;
				if(keydown) camdeltapos += -upvec * cammovemul * General.Settings.MoveSpeed * multiplier * deltatime;
				
				// Move the camera
				General.Map.VisualCamera.ProcessMovement(camdeltapos);
			}

			// Apply new camera matrices
			renderer.PositionAndLookAt(General.Map.VisualCamera.Position, General.Map.VisualCamera.Target);
			
			// Visibility culling
			DoCulling();
			
			// Update labels in main window
			General.MainWindow.UpdateCoordinates(General.Map.VisualCamera.Position);
			
			// Now redraw
			General.Interface.RedrawDisplay();
		}
		
		#endregion

		#region ================== Actions

		//mxd
		[BeginAction("centeroncoordinates", BaseAction = true)]
		protected virtual void CenterOnCoordinates() 
		{
			// Show form...
			CenterOnCoordinatesForm form = new CenterOnCoordinatesForm();
			if(form.ShowDialog() == DialogResult.OK) CenterOnCoordinates(form.Coordinates);
		}

		//mxd
		public void CenterOnCoordinates(Vector2D coords)
		{
			Sector s = General.Map.Map.GetSectorByCoordinates(coords, blockmap);

			if(s == null)
				General.Map.VisualCamera.Position = coords;
			else
				General.Map.VisualCamera.Position = new Vector3D(coords.x, coords.y, s.FloorHeight + 54);

			General.Map.VisualCamera.Sector = s;
		}

		//mxd
		[BeginAction("togglehighlight", BaseAction = true)]
		public void ToggleHighlight()
		{
			General.Settings.UseHighlight = !General.Settings.UseHighlight;
			General.Interface.DisplayStatus(StatusType.Action, "Highlight is now " + (General.Settings.UseHighlight ? "ON" : "OFF") + ".");
		}

		#endregion
	}
}
