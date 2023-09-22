
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
using System.Globalization;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.VisualModes;
using CodeImp.DoomBuilder.Data;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	internal class BaseVisualThing : VisualThing, IVisualEventReceiver
	{
		#region ================== Constants
		
		#endregion
		
		#region ================== Variables

		private readonly BaseVisualMode mode;
		
		private bool isloaded;
		private bool nointeraction; //mxd
		private ImageData[] sprites;
		private double cageradius2;
		private Vector2D pos2d;
		private Vector3D boxp1;
		private Vector3D boxp2;
		private static List<BaseVisualThing> updateList; //mxd
		
		// Undo/redo
		private int undoticket;

		// If this is set to true, the thing will be rebuilt after the action is performed.
		private bool changed;

		#endregion
		
		#region ================== Properties

		public bool Changed { get { return changed; } set { changed |= value; } }

		#endregion
		
		#region ================== Constructor / Setup
		
		// Constructor
		public BaseVisualThing(BaseVisualMode mode, Thing t) : base(t)
		{
			this.mode = mode;

			// Find thing information
			info = General.Map.Data.GetThingInfo(Thing.Type);

			//mxd. When true, the thing can be moved below floor/above ceiling
			nointeraction = (info.Actor != null && info.Actor.GetFlagValue("nointeraction", false));

			//mxd. Find sprite textures
			sprites = new ImageData[info.SpriteFrame.Length];
			for(int i = 0; i < info.SpriteFrame.Length; i++)
			{
				sprites[i] = General.Map.Data.GetSpriteImage(info.SpriteFrame[i].Sprite);
			}

			//mxd
			if(mode.UseSelectionFromClassicMode && t.Selected)
			{
				this.selected = true;
				mode.AddSelectedObject(this);
			}

			// We have no destructor
			GC.SuppressFinalize(this);
		}
		
		// This builds the thing geometry. Returns false when nothing was created.
		public bool Setup()
		{
			// Find the sector in which the thing resides
			Thing.DetermineSector(mode.BlockMap);
			
			//mxd. If the thing is inside a sector, apply DECORATE/UDMF alpha/renderstyle overrides
			byte alpha = 255;
			if(Thing.Sector != null)
			{
				string renderstyle = info.RenderStyle.ToLowerInvariant();
				alpha = info.AlphaByte;
				
				if(General.Map.UDMF)
				{
					if(Thing.IsFlagSet("translucent"))
					{
						renderstyle = "translucent";
						alpha = 64;
					}
					else if(Thing.IsFlagSet("invisible"))
					{
						renderstyle = "none";
						alpha = 0;
					}
					else if(Thing.Fields.ContainsKey("renderstyle"))
					{
						renderstyle = Thing.Fields.GetValue("renderstyle", renderstyle).ToLowerInvariant();
					}

					if((renderstyle == "add" || renderstyle == "translucent" || renderstyle == "subtract" || renderstyle == "translucentstencil") 
						&& Thing.Fields.ContainsKey("alpha"))
					{
						alpha = (byte)(General.Clamp(Thing.Fields.GetValue("alpha", info.Alpha), 0.0, 1.0) * 255.0);
					}
					else if(renderstyle == "soultrans")
					{
						// Lost Soul trasparency is controlled by a CVAR (see https://zdoom.org/wiki/CVARs:Display#transsouls), let's use the default 0.75 here
						alpha = 192;
					}
					else if(renderstyle == "shadow")
					{
						alpha = 76; // about 0.3
						stencilColor = PixelColor.FromInt(PixelColor.INT_BLACK);
					}

					if (renderstyle.EndsWith("stencil"))
                    {
                        stencilColor = PixelColor.FromInt(UniFields.GetInteger(Thing.Fields, "fillcolor", 0));
                        stencilColor.a = 255; // 0xFF alpha means nothing was read. 0x00 alpha means there was a valid fillcolor.
                    }
					else if(renderstyle != "shadow")
						stencilColor.a = 0;
                }
				else if(General.Map.HEXEN)
				{
					if(Thing.IsFlagSet("2048"))
					{
						renderstyle = "translucent";
						alpha = 64;
					}
					else if(Thing.IsFlagSet("4096"))
					{
						renderstyle = "none";
						alpha = 0;
					}
				}

				// Set appropriate RenderPass
				switch(renderstyle)
				{
					case "translucent":
					case "subtract":
					case "soultrans":
					case "translucentstencil":
					case "shadow":
						RenderPass = RenderPass.Alpha;
						break;

					case "add":
                    case "addstencil":
						RenderPass = RenderPass.Additive;
						break;

					case "none":
						RenderPass = RenderPass.Mask;
						alpha = 0;
						break;

					// Many render styles are not supported yet...
					default:
						RenderPass = RenderPass.Mask;
						alpha = 255;
						break;
				}
			}

			int sectorcolor = new PixelColor(alpha, 255, 255, 255).ToInt();
			fogfactor = 0f; //mxd
			
			//mxd. Check thing size 
			float thingradius = Thing.Size; // Thing.Size has ThingRadius arg override applied
			thingheight = Thing.Height; // Thing.Height has ThingHeight arg override applied

			if(thingradius < 0.1f || thingheight < 0.1f) 
			{
				thingradius = FIXED_RADIUS;
				thingheight = FIXED_RADIUS;
				sizeless = true;
			} 
			else 
			{
				sizeless = false;
			}

			if(Thing.Sector != null)
			{
				SectorData sd = mode.GetSectorData(Thing.Sector);
				Plane floor = sd.Floor.plane; //mxd

				if(!info.Bright)
				{
					Vector3D thingpos = new Vector3D(Thing.Position.x, Thing.Position.y, Thing.Position.z + sd.Floor.plane.GetZ(Thing.Position));

					// If is thing's height is flush to a 3D floor top it's not rendered at the brightness of the 3D floor, so take the level above that.
					// It's actually a bit more intricate, since GZDoom can render multiple vertical brightness levels for each thing, which UDB can't,
					// so this is more of a workaround than a real solution
					// See https://github.com/jewalky/UltimateDoomBuilder/issues/940
					//SectorLevel level = sd.GetLevelAboveOrAt(thingpos);
					SectorLevel level = sd.GetLevelAbove(thingpos);

					//mxd. Let's use point on floor plane instead of Thing.Sector.FloorHeight;
					if (nointeraction && level == null && sd.LightLevels.Count > 0) level = sd.LightLevels[sd.LightLevels.Count - 1];

					//mxd. Use the light level of the highest surface when a thing is above highest sector level.
					if(level != null)
					{
						// TECH: In GZDoom, ceiling glow doesn't affect thing brightness 
						// Use sector brightness for color shading
						int brightness = level.brightnessbelow;

						//mxd. Apply lightfloor value
						// According to Graf, this is incorrect behaviour...
						// TECH: In (G)ZDoom, this is ignored when ceiling texture is sky or a thing is below a 3D floor
						// It's probably more involved than this, but for now let's do it only when there are no 3d floors in Thing.Sector...
						/*if(General.Map.UDMF && sd.LightLevels.Count == 2 && !Thing.Sector.HasSkyCeiling)
						{
							if(sd.Sector.Fields.GetValue("lightfloorabsolute", false))
								brightness = UniFields.GetInteger(sd.Sector.Fields, "lightfloor");
							else
								brightness += UniFields.GetInteger(sd.Sector.Fields, "lightfloor");
						}*/

						// Level is glowing
						if(level.affectedbyglow && level.type == SectorLevelType.Floor)
						{
							// Extrafloor glow doesn't affect thing brightness
							if(level.sector == Thing.Sector)
							{
								double planez = level.plane.GetZ(thingpos);

								// Get glow brightness
								int glowbrightness = sd.FloorGlow.Brightness / 2;
								SectorLevel nexthigher = sd.GetLevelAbove(new Vector3D(thingpos, planez));

								// Interpolate thing brightness between glow and regular ones
								if(nexthigher != null)
								{
									double higherz = nexthigher.plane.GetZ(thingpos);
									double delta = General.Clamp(1.0f - (thingpos.z - planez) / (higherz - planez), 0f, 1f);
									brightness = (int)((glowbrightness + level.sector.Brightness / 2) * delta + nexthigher.sector.Brightness * (1.0f - delta));
								}
							}
						}
						// Level below this one is glowing. Only possible for floor glow(?)
						else if(level.type == SectorLevelType.Glow)
						{
							// Interpolate thing brightness between glow and regular ones
							if(sd.Floor != null && sd.FloorGlow != null)
							{
								// Get glow brightness
								double glowz = level.plane.GetZ(thingpos);
								double floorz = floor.GetZ(thingpos);
								double delta = General.Clamp((thingpos.z - floorz) / (glowz - floorz), 0f, 1f);

								brightness = (int)((sd.FloorGlow.Brightness / 2 + sd.Floor.sector.Brightness / 2) * (1.0f - delta) + sd.Floor.sector.Brightness * delta);
							}
						}

						PixelColor areabrightness = PixelColor.FromInt(mode.CalculateBrightness(brightness));
						PixelColor areacolor = PixelColor.Modulate(level.colorbelow, areabrightness);

                        // [ZZ] if sector is using Doom64 lighting, apply thing color here.
                        sectorcolor = PixelColor.Modulate(sd.ColorSprites, areacolor).WithAlpha(alpha).ToInt();

						//mxd. Calculate fogfactor
						fogfactor = VisualGeometry.CalculateFogFactor(level.sector, brightness);
					}
				}
				//TECH: even Bright Thing frames are affected by custom fade...
				else
				{
					Vector3D thingpos = new Vector3D(Thing.Position.x, Thing.Position.y, Thing.Position.z + sd.Floor.plane.GetZ(Thing.Position));
					SectorLevel level = sd.GetLevelAboveOrAt(thingpos);

					if(level != null && level.sector.FogMode > SectorFogMode.CLASSIC)
					{
						//mxd. Calculate fogfactor
						fogfactor = VisualGeometry.CalculateFogFactor(level.sector, level.brightnessbelow);
					}
				}
            }

            //mxd. Create verts for all sprite angles
            WorldVertex[][] allverts = new WorldVertex[info.SpriteFrame.Length][];
			Vector2D[] alloffsets = new Vector2D[info.SpriteFrame.Length];
			base.textures = new ImageData[info.SpriteFrame.Length];
			isloaded = true;

			for(int i = 0; i < sprites.Length; i++)
			{
				Vector2D offsets = new Vector2D();

				// Check if the texture is loaded
				ImageData sprite = sprites[i];
                if (!sprite.IsImageLoaded && !sprite.LoadFailed)
				    sprite.LoadImageNow();
				if(sprite.IsImageLoaded)
				{
					base.textures[i] = sprite;

					// Determine sprite size and offset
					float radius = sprite.ScaledWidth * 0.5f;
					float height = sprite.ScaledHeight;
					offsets.x = radius - (sprite.OffsetX == int.MinValue ? 0 : sprite.OffsetX) * sprite.Scale.x;
					offsets.y = (sprite.OffsetY == int.MinValue ? 0 : sprite.OffsetY) * sprite.Scale.y - height;

					// Scale by thing type/actor scale
					// We do this after the offset x/y determination above, because that is entirely in sprite pixels space
					radius *= info.SpriteScale.Width;
					height *= info.SpriteScale.Height;
					offsets.x *= info.SpriteScale.Width;
					offsets.y *= info.SpriteScale.Height;

					// Make vertices
					WorldVertex[] verts = new WorldVertex[6];

					//mxd. Sprite mirroring
					float ul = (info.SpriteFrame[i].Mirror ? 1f : 0f);
					float ur = (info.SpriteFrame[i].Mirror ? 0f : 1f);

					if(sizeless) //mxd
					{ 
						float hh = height / 2;
						verts[0] = new WorldVertex((float)(-radius + offsets.x), 0.0f, (float)(offsets.y - hh), sectorcolor, ul, 1.0f);
						verts[1] = new WorldVertex((float)(-radius + offsets.x), 0.0f, (float)(hh + offsets.y), sectorcolor, ul, 0.0f);
						verts[2] = new WorldVertex((float)(+radius + offsets.x), 0.0f, (float)(hh + offsets.y), sectorcolor, ur, 0.0f);
						verts[3] = verts[0];
						verts[4] = verts[2];
						verts[5] = new WorldVertex((float)(+radius + offsets.x), 0.0f, (float)(offsets.y - hh), sectorcolor, ur, 1.0f);
					} 
					else 
					{
						verts[0] = new WorldVertex((float)(-radius + offsets.x), 0.0f, (float)offsets.y, sectorcolor, ul, 1.0f);
						verts[1] = new WorldVertex((float)(-radius + offsets.x), 0.0f, (float)(height + offsets.y), sectorcolor, ul, 0.0f);
						verts[2] = new WorldVertex((float)(+radius + offsets.x), 0.0f, (float)(height + offsets.y), sectorcolor, ur, 0.0f);
						verts[3] = verts[0];
						verts[4] = verts[2];
						verts[5] = new WorldVertex((float)(+radius + offsets.x), 0.0f, (float)offsets.y, sectorcolor, ur, 1.0f);
					}
					allverts[i] = verts;
				}
				else
				{
					isloaded = false;
					base.textures[i] = sprite;

					// Determine sprite size
					float radius = Math.Min(thingradius, thingheight / 2f);
					float height = Math.Min(thingradius * 2f, thingheight);

					//mxd. Determine sprite offsets
					offsets.x = radius;
					offsets.y = height / 2;

					// Make vertices
					WorldVertex[] verts = new WorldVertex[6];
					verts[0] = new WorldVertex(-radius, 0.0f, 0.0f, sectorcolor, 0.0f, 1.0f);
					verts[1] = new WorldVertex(-radius, 0.0f, height, sectorcolor, 0.0f, 0.0f);
					verts[2] = new WorldVertex(+radius, 0.0f, height, sectorcolor, 1.0f, 0.0f);
					verts[3] = verts[0];
					verts[4] = verts[2];
					verts[5] = new WorldVertex(+radius, 0.0f, 0.0f, sectorcolor, 1.0f, 1.0f);
					allverts[i] = verts;
				}

				//mxd. Store offsets
				alloffsets[i] = offsets;
			}

			//mxd
			SetVertices(allverts, alloffsets/*, floor, ceiling*/);
			
			// Determine position
			Vector3D pos = Thing.Position;
			if(Thing.Type == 9501)
			{
				if(Thing.Sector != null) //mxd
				{ 
					// This is a special thing that needs special positioning
					SectorData sd = mode.GetSectorData(Thing.Sector);
					pos.z = sd.Ceiling.sector.CeilHeight + Thing.Position.z;
				}
			}
			else if(Thing.Type == 9500)
			{
				if(Thing.Sector != null) //mxd
				{ 
					// This is a special thing that needs special positioning
					SectorData sd = mode.GetSectorData(Thing.Sector);
					pos.z = sd.Floor.sector.FloorHeight + Thing.Position.z;
				}
			}
			else if(info.AbsoluteZ)
			{
				// Absolute Z position
				pos.z = Thing.Position.z;
			}
			else if(info.Hangs)
			{
				// Hang from ceiling
				if(Thing.Sector != null)
				{
					SectorData sd = mode.GetSectorData(Thing.Sector);
					double maxz = sd.Ceiling.plane.GetZ(Thing.Position) - info.Height;
					pos.z = maxz;

					if(Thing.Position.z > 0 || nointeraction) pos.z -= Thing.Position.z;

					// Check if below floor
					if(!nointeraction)
					{
						double minz = sd.Floor.plane.GetZ(Thing.Position);
						if(pos.z < minz) pos.z = Math.Min(minz, maxz);
					}
				}
			}
			else
			{
				// Stand on floor
				if(Thing.Sector != null)
				{
					SectorData sd = mode.GetSectorData(Thing.Sector);
					double minz = sd.Floor.plane.GetZ(Thing.Position);
					pos.z = minz;

					if(Thing.Position.z > 0 || nointeraction) pos.z += Thing.Position.z;

					// Check if above ceiling
					if(!nointeraction)
					{
						double maxz = sd.Ceiling.plane.GetZ(Thing.Position) - info.Height;
						if(pos.z > maxz) pos.z = Math.Max(minz, maxz);
					}
				}
			}
			
			// Apply settings
			SetPosition(pos);
			SetCageColor(Thing.Color);

			// Keep info for object picking
			cageradius2 = thingradius * Angle2D.SQRT2;
			cageradius2 = cageradius2 * cageradius2;
			pos2d = pos;

			if(sizeless) //mxd
			{ 
				boxp1 = new Vector3D(pos.x - thingradius, pos.y - thingradius, pos.z - thingradius/2);
				boxp2 = new Vector3D(pos.x + thingradius, pos.y + thingradius, pos.z + thingradius/2);
			} 
			else 
			{
				boxp1 = new Vector3D(pos.x - thingradius, pos.y - thingradius, pos.z);
				boxp2 = new Vector3D(pos.x + thingradius, pos.y + thingradius, pos.z + thingheight);
			}
			
			// Done
			changed = false;
			return true;
		}
		
		// Disposing
		public override void Dispose()
		{
			if(!IsDisposed)
			{
				sprites = null;

				base.Dispose();
			}
		}
		
		#endregion
		
		#region ================== Methods
		
		// This forces to rebuild the whole thing
		public void Rebuild()
		{
			// Find thing information
			info = General.Map.Data.GetThingInfo(Thing.Type);

			//mxd. When true, the thing can be moved below floor/above ceiling
			nointeraction = (info.Actor != null && info.Actor.GetFlagValue("nointeraction", false));

			//mxd. Find sprite textures
			sprites = new ImageData[info.SpriteFrame.Length];
			for(int i = 0; i < info.SpriteFrame.Length; i++)
			{
				sprites[i] = General.Map.Data.GetSpriteImage(info.SpriteFrame[i].Sprite);
			}
			
			// Setup visual thing
			Setup();
		}
		
		// This updates the thing when needed
		public override void Update()
		{
			if(!isloaded)
			{
				//mxd. Rebuild sprite geometry when all sprites are loaded
				isloaded = true;
				foreach(ImageData sprite in sprites)
				{
					if(!sprite.IsImageLoaded)
					{
						isloaded = false;
						break;
					}
				}

				if(isloaded) Setup();
			}
			
			// Let the base update
			base.Update();
		}

		// This performs a fast test in object picking
		public override bool PickFastReject(Vector3D from, Vector3D to, Vector3D dir)
		{
			//mxd. Don't highlight when thing sprite is not rendered and thing cages are disabled 
			if(!General.Map.Renderer3D.DrawThingCages && info.DistanceCheckSq < int.MaxValue
				&& (Thing.Position - General.Map.VisualCamera.Position).GetLengthSq() > info.DistanceCheckSq)
				return false;

			double distance2 = Line2D.GetDistanceToLineSq(from, to, pos2d, false);
			return (distance2 <= cageradius2);
		}

		// This performs an accurate test for object picking
		public override bool PickAccurate(Vector3D from, Vector3D to, Vector3D dir, ref double u_ray)
		{
			Vector3D delta = to - from;
			double tfar = double.MaxValue;
			double tnear = double.MinValue;
			
			// Ray-Box intersection code
			// See http://www.masm32.com/board/index.php?topic=9941.0
			
			// Check X slab
			if(delta.x == 0.0f)
			{
				if(from.x > boxp2.x || from.x < boxp1.x)
				{
					// Ray is parallel to the planes & outside slab
					return false;
				}
			}
			else
			{
				double tmp = 1.0f / delta.x;
				double t1 = (boxp1.x - from.x) * tmp;
				double t2 = (boxp2.x - from.x) * tmp;
				if(t1 > t2) General.Swap(ref t1, ref t2);
				if(t1 > tnear) tnear = t1;
				if(t2 < tfar) tfar = t2;
				if(tnear > tfar || tfar < 0.0f)
				{
					// Ray missed box or box is behind ray
					return false;
				}
			}
			
			// Check Y slab
			if(delta.y == 0.0f)
			{
				if(from.y > boxp2.y || from.y < boxp1.y)
				{
					// Ray is parallel to the planes & outside slab
					return false;
				}
			}
			else
			{
				double tmp = 1.0f / delta.y;
				double t1 = (boxp1.y - from.y) * tmp;
				double t2 = (boxp2.y - from.y) * tmp;
				if(t1 > t2) General.Swap(ref t1, ref t2);
				if(t1 > tnear) tnear = t1;
				if(t2 < tfar) tfar = t2;
				if(tnear > tfar || tfar < 0.0f)
				{
					// Ray missed box or box is behind ray
					return false;
				}
			}
			
			// Check Z slab
			if(delta.z == 0.0f)
			{
				if(from.z > boxp2.z || from.z < boxp1.z)
				{
					// Ray is parallel to the planes & outside slab
					return false;
				}
			}
			else
			{
				double tmp = 1.0f / delta.z;
				double t1 = (boxp1.z - from.z) * tmp;
				double t2 = (boxp2.z - from.z) * tmp;
				if(t1 > t2) General.Swap(ref t1, ref t2);
				if(t1 > tnear) tnear = t1;
				if(t2 < tfar) tfar = t2;
				if(tnear > tfar || tfar < 0.0f)
				{
					// Ray missed box or box is behind ray
					return false;
				}
			}
			
			// Set interpolation point
			u_ray = (tnear > 0.0f) ? tnear : tfar;
			return true;
		}

		//mxd
		public bool IsSelected() 
		{
			return selected;
		}
		
		#endregion

		#region ================== Events

		// Unused
		public void OnSelectBegin() { }
		public void OnEditBegin() { }
		public void OnChangeTargetBrightness(bool up) { }
		public void OnChangeTextureOffset(int horizontal, int vertical, bool doSurfaceAngleCorrection) { }
		public void OnSelectTexture() { }
		public void OnCopyTexture() { }
		public void OnPasteTexture() { }
		public void OnCopyTextureOffsets() { }
		public void OnPasteTextureOffsets() { }
		public void OnTextureAlign(bool alignx, bool aligny) { }
		public void OnToggleUpperUnpegged() { }
		public void OnToggleLowerUnpegged() { }
		public void OnProcess(long deltatime) { }
		public void OnTextureFloodfill() { }
		public void OnInsert() { }
		public void OnTextureFit(FitTextureOptions options) { } //mxd
		public void ApplyTexture(string texture) { }
		public void ApplyUpperUnpegged(bool set) { }
		public void ApplyLowerUnpegged(bool set) { }
		public void SelectNeighbours(bool select, bool withSameTexture, bool withSameHeight) { } //mxd
		public virtual void OnPaintSelectEnd() { } // biwa

		// Return texture name
		public string GetTextureName() { return ""; }

		// Select or deselect
		public void OnSelectEnd()
		{
			if(this.selected)
			{
				this.selected = false;
				mode.RemoveSelectedObject(this);
			}
			else
			{
				this.selected = true;
				mode.AddSelectedObject(this);
			}
		}

		//mxd. Delete thing
		public void OnDelete() 
		{
			mode.CreateUndo("Delete thing");
			mode.SetActionResult("Deleted a thing.");

			this.Thing.Fields.BeforeFieldsChange();
			this.Thing.Dispose();
			this.Dispose();

			General.Map.IsChanged = true;
			General.Map.ThingsFilter.Update();
		}
		
		// Copy properties
		public void OnCopyProperties()
		{
			BuilderPlug.Me.CopiedThingProps = new ThingProperties(Thing);
			mode.SetActionResult("Copied thing properties.");
		}
		
		// Paste properties
		public void OnPasteProperties(bool usecopysettings)
		{
			if(BuilderPlug.Me.CopiedThingProps != null)
			{
				mode.CreateUndo("Paste thing properties");
				mode.SetActionResult("Pasted thing properties.");
				BuilderPlug.Me.CopiedThingProps.Apply(new List<Thing> { Thing }, usecopysettings); //mxd. Added "usecopysettings"
				Thing.UpdateConfiguration();
				this.Rebuild();
				mode.ShowTargetInfo();
			}
		}
		
		// Edit button released
		public void OnEditEnd()
		{
			if(General.Interface.IsActiveWindow)
			{
				
				List<Thing> things = mode.GetSelectedThings();
				//mxd
				updateList = new List<BaseVisualThing>();
				foreach(Thing t in things)
				{
					VisualThing vt = mode.GetVisualThing(t);
					if(vt != null) updateList.Add((BaseVisualThing)vt);
				}

				General.Interface.OnEditFormValuesChanged += Interface_OnEditFormValuesChanged;
				mode.StartRealtimeInterfaceUpdate(SelectionType.Things);
				General.Interface.ShowEditThings(things);
				mode.StopRealtimeInterfaceUpdate(SelectionType.Things);
				General.Interface.OnEditFormValuesChanged -= Interface_OnEditFormValuesChanged;

				updateList.Clear();
				updateList = null;
			}
		}

		//mxd
		private void Interface_OnEditFormValuesChanged(object sender, EventArgs e) 
		{
			foreach(BaseVisualThing vt in updateList) vt.Changed = true;
		}

		//mxd
		public void OnResetTextureOffset() 
		{
			mode.CreateUndo("Reset thing scale");
			mode.SetActionResult("Thing scale reset.");

			Thing.SetScale(1.0f, 1.0f);

			// Update what must be updated
			this.Changed = true;
		}

		//mxd
		public void OnResetLocalTextureOffset() 
		{
			mode.CreateUndo("Reset thing scale, pitch and roll");
			mode.SetActionResult("Thing scale, pitch and roll reset.");

			Thing.SetScale(1.0f, 1.0f);
			Thing.SetPitch(0);
			Thing.SetRoll(0);

			// Update what must be updated
			this.Changed = true;
		}
		
		// Raise/lower thing
		public void OnChangeTargetHeight(int amount)
		{
			if(General.Map.FormatInterface.HasThingHeight)
			{
				if((General.Map.UndoRedo.NextUndo == null) || (General.Map.UndoRedo.NextUndo.TicketID != undoticket))
					undoticket = mode.CreateUndo("Change thing height");

				Thing.Move(Thing.Position + new Vector3D(0.0f, 0.0f, (info.Hangs ? -amount : amount)));

				mode.SetActionResult("Changed thing height to " + Thing.Position.z + ".");
				
				// Update what must be updated
				ThingData td = mode.GetThingData(this.Thing);
				foreach(KeyValuePair<Sector, bool> s in td.UpdateAlso)
				{
					if(mode.VisualSectorExists(s.Key))
					{
						BaseVisualSector vs = (BaseVisualSector)mode.GetVisualSector(s.Key);
						vs.UpdateSectorGeometry(s.Value);
					}
				}
				
				this.Changed = true;
			}
		}

		//mxd
		public void OnChangeScale(int incrementX, int incrementY)
		{
			if(!General.Map.UDMF || sprites == null || !sprites[0].IsImageLoaded) return;
			
			if((General.Map.UndoRedo.NextUndo == null) || (General.Map.UndoRedo.NextUndo.TicketID != undoticket))
				undoticket = mode.CreateUndo("Change thing scale");

			double scaleX = Thing.ScaleX;
			double scaleY = Thing.ScaleY;
			ImageData sprite = sprites[0];

			if(incrementX != 0) 
			{
				double pix = (int)Math.Round(sprite.Width * scaleX) + incrementX;
				double newscaleX = Math.Round(pix / sprite.Width, 3);
				scaleX = (newscaleX == 0 ? scaleX * -1 : newscaleX);
			}

			if(incrementY != 0) 
			{
				double pix = (int)Math.Round(sprite.Height * scaleY) + incrementY;
				double newscaleY = Math.Round(pix / sprite.Height, 3);
				scaleY = (newscaleY == 0 ? scaleY * -1 : newscaleY);
			}

			Thing.SetScale(scaleX, scaleY);
			mode.SetActionResult("Changed thing scale to " + scaleX.ToString("F03", CultureInfo.InvariantCulture) + ", " + scaleY.ToString("F03", CultureInfo.InvariantCulture) + " (" + (int)Math.Round(sprite.Width * scaleX) + " x " + (int)Math.Round(sprite.Height * scaleY) + ").");

			// Update what must be updated
			this.Changed = true;
		}

		//mxd
		public void OnMove(Vector3D newposition) 
		{
			if((General.Map.UndoRedo.NextUndo == null) || (General.Map.UndoRedo.NextUndo.TicketID != undoticket))
				undoticket = mode.CreateUndo("Move thing");
			Thing.Move(newposition);
			mode.SetActionResult("Changed thing position to " + Thing.Position + ".");

			// Update what must be updated
			ThingData td = mode.GetThingData(this.Thing);
			foreach(KeyValuePair<Sector, bool> s in td.UpdateAlso) 
			{
				if(mode.VisualSectorExists(s.Key)) 
				{
					BaseVisualSector vs = (BaseVisualSector)mode.GetVisualSector(s.Key);
					vs.UpdateSectorGeometry(s.Value);
				}
			}

			this.Changed = true;
		}

		// biwa. Moving the mouse
		public virtual void OnMouseMove(MouseEventArgs e)
		{
			// biwa. Paint selection going on?
			if (mode.PaintSelectPressed)
			{
				// toggle selected state
				if (mode.PaintSelectType == this.GetType() && mode.Highlighted != this)
				{
					if (General.Interface.ShiftState ^ BuilderPlug.Me.AdditivePaintSelect)
					{
						if (!selected)
						{
							selected = true;
							mode.AddSelectedObject(this);
						}
					}
					else if (General.Interface.CtrlState)
					{
						if (selected)
						{
							selected = false;
							mode.RemoveSelectedObject(this);
						}
					}
					else
					{
						if (selected)
							mode.RemoveSelectedObject(this);
						else
							mode.AddSelectedObject(this);

						selected = !selected;
					}
				}
			}
		}

		// biwa
		public virtual void OnPaintSelectBegin()
		{
			mode.PaintSelectType = this.GetType();

			// toggle selected state
			if (General.Interface.ShiftState ^ BuilderPlug.Me.AdditivePaintSelect)
			{
				if (!selected)
				{
					selected = true;
					mode.AddSelectedObject(this);
				}
			}
			else if (General.Interface.CtrlState)
			{
				if (selected)
				{
					selected = false;
					mode.RemoveSelectedObject(this);
				}
			}
			else
			{
				if (selected)
					mode.RemoveSelectedObject(this);
				else
					mode.AddSelectedObject(this);

				selected = !selected;
			}
		}

		//mxd
		public void SetAngle(int newangle)
		{
			if((General.Map.UndoRedo.NextUndo == null) || (General.Map.UndoRedo.NextUndo.TicketID != undoticket))
				undoticket = mode.CreateUndo("Change thing angle");
			Thing.Rotate(newangle);
			mode.SetActionResult("Changed thing angle to " + Thing.AngleDoom + ".");
			this.Changed = true;
		}

		//mxd
		public void SetPitch(int newpitch)
		{
			if(!General.Map.UDMF) return;
			if((General.Map.UndoRedo.NextUndo == null) || (General.Map.UndoRedo.NextUndo.TicketID != undoticket))
				undoticket = mode.CreateUndo("Change thing pitch");
			Thing.SetPitch(newpitch);
			mode.SetActionResult("Changed thing pitch to " + Thing.Pitch + ".");
			this.Changed = true;
		}

		//mxd
		public void SetRoll(int newroll)
		{
			if(!General.Map.UDMF) return;
			if((General.Map.UndoRedo.NextUndo == null) || (General.Map.UndoRedo.NextUndo.TicketID != undoticket))
				undoticket = mode.CreateUndo("Change thing roll");
			Thing.SetRoll(newroll);
			mode.SetActionResult("Changed thing roll to " + Thing.Roll + ".");
			this.Changed = true;
		}
		
		#endregion
	}
}
