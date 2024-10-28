	
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
using System.Collections.ObjectModel;
using System.Drawing;
using System.Globalization;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Types;
using CodeImp.DoomBuilder.VisualModes;
using CodeImp.DoomBuilder.Data;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	internal sealed class VisualFloor : BaseVisualGeometrySector
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		private bool innerside; //mxd

		#endregion

		#region ================== Properties

		#endregion

		#region ================== Constructor / Setup

		// Constructor
		public VisualFloor(BaseVisualMode mode, VisualSector vs) : base(mode, vs)
		{
			//mxd
			geometrytype = VisualGeometryType.FLOOR;
			partname = "floor";
			performautoselection = mode.UseSelectionFromClassicMode && vs != null && vs.Sector.Selected && (General.Map.ViewMode == ViewMode.FloorTextures || General.Map.ViewMode == ViewMode.Normal);
			
			// We have no destructor
			GC.SuppressFinalize(this);
		}

		// This builds the geometry. Returns false when no geometry created.
		public override bool Setup(SectorLevel level, Effect3DFloor extrafloor) 
		{
			return Setup(level, extrafloor, innerside);
		}

		//mxd
		public bool Setup(SectorLevel level, Effect3DFloor extrafloor, bool innerside)
		{
			Sector s = level.sector;
			Vector2D texscale;
			this.innerside = innerside;
			
			base.Setup(level, extrafloor);

			// Fetch ZDoom fields
			double rotate = Angle2D.DegToRad(s.Fields.GetValue("rotationfloor", 0.0));
			Vector2D offset = new Vector2D(s.Fields.GetValue("xpanningfloor", 0.0),
										   s.Fields.GetValue("ypanningfloor", 0.0));
			Vector2D scale = new Vector2D(s.Fields.GetValue("xscalefloor", 1.0),
										  s.Fields.GetValue("yscalefloor", 1.0));
			
			//Load floor texture
			if(s.LongFloorTexture != MapSet.EmptyLongName)
			{
				base.Texture = General.Map.Data.GetFlatImage(s.LongFloorTexture);
				if(base.Texture == null || base.Texture is UnknownImage)
				{
					base.Texture = General.Map.Data.UnknownTexture3D;
					setuponloadedtexture = s.LongFloorTexture;
				}
				else if(!base.Texture.IsImageLoaded)
				{
					setuponloadedtexture = s.LongFloorTexture;
				}
			}
			else
			{
				// Use missing texture
				base.Texture = General.Map.Data.MissingTexture3D;
				setuponloadedtexture = 0;
			}

			// Determine texture scale
			if(base.Texture.IsImageLoaded)
				texscale = new Vector2D(1.0f / base.Texture.ScaledWidth, 1.0f / base.Texture.ScaledHeight);
			else
				texscale = new Vector2D(1.0f / 64.0f, 1.0f / 64.0f);

            // Determine brightness
            byte alpha = (byte)General.Clamp(level.alpha, 0, 255);
            int color = PixelColor.FromInt(level.color).WithAlpha(alpha).ToInt();

            //mxd. Top extrafloor level should calculate fogdensity
            //from the brightness of the level above it
            SectorData sd = mode.GetSectorData(this.Sector.Sector);
            int targetbrightness;
			if(extrafloor != null && extrafloor.VavoomType && !level.disablelighting)
			{
				targetbrightness = 0;
				for(int i = 0; i < sd.LightLevels.Count - 1; i++)
				{
					if(sd.LightLevels[i] == level)
					{
						targetbrightness = sd.LightLevels[i + 1].brightnessbelow;
						break;
					}
				}
			}
			else
			{
				targetbrightness = level.brightnessbelow;
			}

            // [ZZ] Apply Doom 64 lighting here (for extrafloor)
            if (extrafloor != null) color = PixelColor.Modulate(PixelColor.FromInt(color), extrafloor.ColorFloor).WithAlpha(alpha).ToInt();

            //mxd. Determine fog density
            fogfactor = CalculateFogFactor(targetbrightness);

			// Make vertices
			ReadOnlyCollection<Vector2D> triverts = Sector.Sector.Triangles.Vertices;
			WorldVertex[] verts = new WorldVertex[triverts.Count];
			for(int i = 0; i < triverts.Count; i++)
			{
				// Color shading
				verts[i].c = color; //mxd
				
				// Vertex coordinates
				verts[i].x = (float)triverts[i].x;
				verts[i].y = (float)triverts[i].y;
				verts[i].z = (float)level.plane.GetZ(triverts[i]);

				// Texture coordinates
				Vector2D pos = triverts[i];
				pos = pos.GetRotated(rotate);
				pos.y = -pos.y;
				pos = (pos + offset) * scale * texscale;
				verts[i].u = (float)pos.x;
				verts[i].v = (float)pos.y;
			}

			// The sector triangulation created clockwise triangles that
			// are right up for the floor. For the ceiling we must flip
			// the triangles upside down.
			if((extrafloor != null) && !extrafloor.VavoomType && !innerside)
				SwapTriangleVertices(verts);
			
			// Determine render pass
			if(extrafloor != null)
			{
				if (extrafloor.Sloped3dFloor) //mxd
					this.RenderPass = RenderPass.Mask;
				else if (extrafloor.RenderAdditive) //mxd
					this.RenderPass = RenderPass.Additive;
				else if ((level.alpha < 255) || Texture.IsTranslucent)
					this.RenderPass = RenderPass.Alpha;
				else
					this.RenderPass = RenderPass.Mask;
			}
			else
			{
				this.RenderPass = RenderPass.Solid;
			}

			//mxd. Update sky render flag
			renderassky = level.sector.HasSkyFloor;
			
			// Apply vertices
			base.SetVertices(verts);
			return (verts.Length > 0);
		}

		#endregion
		
		#region ================== Methods

		//mxd
		public override void OnChangeScale(int incrementX, int incrementY)
		{
			// Only do this when not done yet in this call
			// Because we may be able to select the same 3D floor multiple times through multiple sectors
			SectorData sd = mode.GetSectorData(level.sector);
			if(!sd.FloorChanged)
			{
				sd.FloorChanged = true;
				base.OnChangeScale(incrementX, incrementY);
			}
		}

		//mxd
		public override void OnChangeTextureRotation(double angle)
		{
			// Only do this when not done yet in this call
			// Because we may be able to select the same 3D floor multiple times through multiple sectors
			SectorData sd = mode.GetSectorData(level.sector);
			if(!sd.FloorChanged)
			{
				sd.FloorChanged = true;
				base.OnChangeTextureRotation(angle);
			}
		}

		// Return texture coordinates
		protected override Point GetTextureOffset()
		{
			return new Point { X = (int)Sector.Sector.Fields.GetValue("xpanningfloor", 0.0), 
							   Y = (int)Sector.Sector.Fields.GetValue("ypanningfloor", 0.0) };
		}

		//mxd
		public override bool OnChangeTextureOffset(int horizontal, int vertical, bool doSurfaceAngleCorrection)
		{
			bool appliedoffset = false;

			// Only do this when not done yet in this call
			// Because we may be able to select the same 3D floor multiple times through multiple sectors
			SectorData sd = mode.GetSectorData(level.sector);
			if(!sd.FloorChanged)
			{
				sd.FloorChanged = appliedoffset = base.OnChangeTextureOffset(horizontal, vertical, doSurfaceAngleCorrection);
			}

			return appliedoffset;
		}

		// Move texture coordinates
		protected override void MoveTextureOffset(int offsetx, int offsety)
		{
			//mxd
			Sector s = GetControlSector();
			s.Fields.BeforeFieldsChange();
			double nx = (s.Fields.GetValue("xpanningfloor", 0.0) + offsetx) % (Texture.ScaledWidth / s.Fields.GetValue("xscalefloor", 1.0));
			double ny = (s.Fields.GetValue("ypanningfloor", 0.0) + offsety) % (Texture.ScaledHeight / s.Fields.GetValue("yscalefloor", 1.0));
			s.Fields["xpanningfloor"] = new UniValue(UniversalType.Float, nx);
			s.Fields["ypanningfloor"] = new UniValue(UniversalType.Float, ny);
			s.UpdateNeeded = true;

			mode.SetActionResult("Changed floor texture offsets to " + nx + ", " + ny + ".");
		}

		//mxd. Texture scale change
		protected override void ChangeTextureScale(int incrementX, int incrementY) 
		{
			if(Texture == null || !Texture.IsImageLoaded) return;
			Sector s = GetControlSector();
			double scaleX = s.Fields.GetValue("xscalefloor", 1.0);
			double scaleY = s.Fields.GetValue("yscalefloor", 1.0);

			s.Fields.BeforeFieldsChange();

			if(incrementX != 0) 
			{
				double pix = (int)Math.Round(Texture.Width * scaleX) - incrementX;
				double newscaleX = Math.Round(pix / Texture.Width, 3);
				scaleX = (newscaleX == 0 ? scaleX * -1 : newscaleX);
				UniFields.SetFloat(s.Fields, "xscalefloor", scaleX, 1.0);
			}

			if(incrementY != 0)
			{
				double pix = (int)Math.Round(Texture.Height * scaleY) - incrementY;
				double newscaleY = Math.Round(pix / Texture.Height, 3);
				scaleY = (newscaleY == 0 ? scaleY * -1 : newscaleY);
				UniFields.SetFloat(s.Fields, "yscalefloor", scaleY, 1.0);
			}

			mode.SetActionResult("Floor scale changed to " + scaleX.ToString("F03", CultureInfo.InvariantCulture) + ", " + scaleY.ToString("F03", CultureInfo.InvariantCulture) + " (" + (int)Math.Round(Texture.Width / scaleX) + " x " + (int)Math.Round(Texture.Height / scaleY) + ").");
		}

		//mxd
		public override void OnResetTextureOffset() 
		{
			ClearFields(new[] { "xpanningfloor", "ypanningfloor" }, "Reset texture offsets", "Texture offsets reset.");
		}

		//mxd
		public override void OnResetLocalTextureOffset() 
		{
			ClearFields(new[] { "xpanningfloor", "ypanningfloor", "xscalefloor", "yscalefloor", "rotationfloor", "lightfloor", "lightfloorabsolute" },
				"Reset texture offsets, scale, rotation and brightness", "Texture offsets, scale, rotation and brightness reset.");
		}
		
		// Paste texture
		public override void OnPasteTexture()
		{
			if(BuilderPlug.Me.CopiedFlat != null)
			{
				mode.CreateUndo("Paste floor \"" + BuilderPlug.Me.CopiedFlat + "\"");
				mode.SetActionResult("Pasted flat \"" + BuilderPlug.Me.CopiedFlat + "\" on floor.");
				
				SetTexture(BuilderPlug.Me.CopiedFlat);

				// Update. We need to create a visual sector if it doesn't exist yet. This can happen when pasting
				// to a 3D floor and its control sector wasn't in view before
				BaseVisualSector vs;

				if (mode.VisualSectorExists(level.sector))
				{
					vs = (BaseVisualSector)mode.GetVisualSector(level.sector);
				}
				else
				{
					vs = mode.CreateBaseVisualSector(level.sector);
				}

				if (vs != null)
					vs.UpdateSectorGeometry(false);
			}
		}

		// Call to change the height
		public override void OnChangeTargetHeight(int amount)
		{
			// Only do this when not done yet in this call
			// Because we may be able to select the same 3D floor multiple times through multiple sectors
			SectorData sd = mode.GetSectorData(level.sector);
			if(!sd.FloorChanged)
			{
				sd.FloorChanged = true;
				base.OnChangeTargetHeight(amount);
			}
		}

		// This changes the height
		protected override void ChangeHeight(int amount)
		{
			if (level.sector.FloorSlope.GetLengthSq() > 0)
			{
				mode.CreateUndo("Change floor slope height", UndoGroup.FloorHeightChange, level.sector.FixedIndex);

				level.sector.FloorSlopeOffset -= level.sector.FloorSlope.z * amount;

				mode.SetActionResult("Changed floor slope height by " + amount + ".");
			}
			else
			{
				mode.CreateUndo("Change floor height", UndoGroup.FloorHeightChange, level.sector.FixedIndex);
				level.sector.FloorHeight += amount;

				if (General.Map.UDMF)
				{
					//mxd. Modify vertex offsets?
					if (level.sector.Sidedefs.Count == 3)
					{
						ChangeVertexHeight(amount);
					}
				}

				mode.SetActionResult("Changed floor height to " + level.sector.FloorHeight + ".");
			}
		}

		//mxd
		private void ChangeVertexHeight(int amount) 
		{
			HashSet<Vertex> verts = new HashSet<Vertex>();

			// Do this only if all 3 verts have offsets
			foreach(Sidedef side in level.sector.Sidedefs) 
			{
				if(double.IsNaN(side.Line.Start.ZFloor) || double.IsNaN(side.Line.End.ZFloor)) return;
				verts.Add(side.Line.Start);
				verts.Add(side.Line.End);
			}

			foreach(Vertex v in verts) 
				mode.GetVisualVertex(v, true).OnChangeTargetHeight(amount);
		}

		//mxd. Sector brightness change
		public override void OnChangeTargetBrightness(bool up) 
		{
			if(level != null) 
			{
				// This floor is part of 3D-floor
				if(level.sector != Sector.Sector)
				{
					BaseVisualSector vs = (BaseVisualSector)mode.GetVisualSector(level.sector);
					vs.Floor.OnChangeTargetBrightness(up);
					vs.UpdateSectorGeometry(true);
				}
				// This is actual floor of a sector with extrafloors
				else if(Sector.ExtraFloors.Count > 0 && !Sector.ExtraFloors[0].ExtraFloor.Floor.restrictlighting && !Sector.ExtraFloors[0].ExtraFloor.Floor.disablelighting)
				{
					Sector.ExtraFloors[0].OnChangeTargetBrightness(up);
				}
				else
				{
					base.OnChangeTargetBrightness(up);
				}
			} 
			else 
			{
				base.OnChangeTargetBrightness(up);
			}
		}

		// This performs a fast test in object picking
		public override bool PickFastReject(Vector3D from, Vector3D to, Vector3D dir)
		{
			// Check if our ray starts at the correct side of the plane
			if((!innerside && level.plane.Distance(from) > 0.0f) || (innerside && level.plane.Distance(from) < 0.0f))
			{
				// Calculate the intersection
				if(level.plane.GetIntersection(from, to, ref pickrayu))
				{
					if(pickrayu > 0.0f)
					{
						pickintersect = from + (to - from) * pickrayu;
						
						// Intersection point within bbox?
						RectangleF bbox = Sector.Sector.BBox;
						return ((pickintersect.x >= bbox.Left) && (pickintersect.x <= bbox.Right) &&
								(pickintersect.y >= bbox.Top) && (pickintersect.y <= bbox.Bottom));
					}
				}
			}
			
			return false;
		}
		
		// This performs an accurate test for object picking
		public override bool PickAccurate(Vector3D from, Vector3D to, Vector3D dir, ref double u_ray)
		{
			u_ray = pickrayu;
			
			// Check on which side of the nearest sidedef we are
			Sidedef sd = MapSet.NearestSidedef(Sector.Sector.Sidedefs, pickintersect);
			double side = sd.Line.SideOfLine(pickintersect);

			//mxd. Alpha based picking. Used only on extrafloors with transparent or masked textures
			if((side <= 0.0f && sd.IsFront) || (side > 0.0f && !sd.IsFront))
			{
				if(!BuilderPlug.Me.AlphaBasedTextureHighlighting || !Texture.IsImageLoaded || extrafloor == null || RenderPass == RenderPass.Solid || (!Texture.IsTranslucent && !Texture.IsMasked))
					return true;

				// Some textures (e.g. HiResImage) may lie about their size, so use bitmap size instead
                int imageWidth = Texture.GetAlphaTestWidth();
                int imageHeight = Texture.GetAlphaTestHeight();

				// Fetch ZDoom fields
				double rotate = Angle2D.DegToRad(level.sector.Fields.GetValue("rotationfloor", 0.0));
                Vector2D offset = new Vector2D(level.sector.Fields.GetValue("xpanningfloor", 0.0), level.sector.Fields.GetValue("ypanningfloor", 0.0));
                Vector2D scale = new Vector2D(level.sector.Fields.GetValue("xscalefloor", 1.0), level.sector.Fields.GetValue("yscalefloor", 1.0));
                Vector2D texscale = new Vector2D(1.0 / Texture.ScaledWidth, 1.0 / Texture.ScaledHeight);

                // Texture coordinates
                Vector2D o = pickintersect;
                o = o.GetRotated(rotate);
                o.y = -o.y;
                o = (o + offset) * scale * texscale;
                o.x = (o.x * imageWidth) % imageWidth;
                o.y = (o.y * imageHeight) % imageHeight;

                // Make sure coordinates are inside of texture dimensions...
                if (o.x < 0) o.x += imageWidth;
                if (o.y < 0) o.y += imageHeight;

                // Make final texture coordinates...
                int ox = General.Clamp((int)Math.Floor(o.x), 0, imageWidth - 1);
                int oy = General.Clamp((int)Math.Floor(o.y), 0, imageHeight - 1);

                // Check pixel alpha
                return Texture.AlphaTestPixel(ox, oy);
			}

			return false;
		}
		
		// Return texture name
		public override string GetTextureName()
		{
			return level.sector.FloorTexture;
		}

		// This changes the texture
		protected override void SetTexture(string texturename)
		{
			// Set new texture
			level.sector.SetFloorTexture(texturename);
			General.Map.Data.UpdateUsedTextures();
		}

		//mxd
		public override void SelectNeighbours(bool select, bool withSameTexture, bool withSameHeight, bool stopatselected) 
		{
			if(!withSameTexture && !withSameHeight) return;

			if(select && !selected) 
			{
				selected = true;
				mode.AddSelectedObject(this);
			}
			else if(!select && selected)
			{
				selected = false;
				mode.RemoveSelectedObject(this);
			}
			
			List<Sector> neighbours = new List<Sector>();
			bool regularorvavoom = (extrafloor == null || extrafloor.VavoomType);

			//collect neighbour sectors
			foreach(Sidedef side in Sector.Sector.Sidedefs) 
			{
				if(side.Other != null && side.Other.Sector != Sector.Sector && !neighbours.Contains(side.Other.Sector))
				{
					BaseVisualSector vs = (BaseVisualSector)mode.GetVisualSector(side.Other.Sector);
					if(vs == null) continue;

					// When current floor is part of a 3d floor, it looks like a ceiling, so we need to select adjacent ceilings
					if(level.sector != Sector.Sector && !regularorvavoom)
					{
						if((!withSameTexture || side.Other.Sector.LongCeilTexture == level.sector.LongFloorTexture) &&
							(!withSameHeight || side.Other.Sector.CeilHeight == level.sector.FloorHeight)) 
						{
							neighbours.Add(side.Other.Sector);

							//(de)select regular visual ceiling?
							if(select != vs.Ceiling.Selected) 
								vs.Ceiling.SelectNeighbours(select, withSameTexture, withSameHeight, stopatselected);
						}
					}
					else // Regular floor or vavoom-type extrafloor
					{
						// (De)select adjacent floor
						if((!withSameTexture || side.Other.Sector.LongFloorTexture == level.sector.LongFloorTexture) &&
							(!withSameHeight || side.Other.Sector.FloorHeight == level.sector.FloorHeight)) 
						{
							neighbours.Add(side.Other.Sector);

							//(de)select regular visual floor?
							if(select != vs.Floor.Selected) 
								vs.Floor.SelectNeighbours(select, withSameTexture, withSameHeight, stopatselected);
						}
					}

					// (De)select adjacent extra floors
					foreach(VisualFloor ef in vs.ExtraFloors) 
					{
						if(select == ef.Selected || ef.extrafloor.VavoomType != regularorvavoom) continue;
						if((!withSameTexture || level.sector.LongFloorTexture == ef.level.sector.LongFloorTexture) &&
							(!withSameHeight || level.sector.FloorHeight == ef.level.sector.FloorHeight)) 
						{
							ef.SelectNeighbours(select, withSameTexture, withSameHeight, stopatselected);
						}
					}

					// (De)select adjacent vavoom type extra ceilings
					foreach(VisualCeiling ec in vs.ExtraCeilings) 
					{
						if(select == ec.Selected || ec.ExtraFloor.VavoomType == regularorvavoom) continue;
						if((!withSameTexture || level.sector.LongFloorTexture == ec.Level.sector.LongCeilTexture) &&
							(!withSameHeight || level.sector.FloorHeight == ec.Level.sector.CeilHeight)) 
						{
							ec.SelectNeighbours(select, withSameTexture, withSameHeight, stopatselected);
						}
					}
				}
			}
		}

		//mxd
		public void AlignTexture(bool alignx, bool aligny) 
		{
			if(!General.Map.UDMF) return;

			AlignTextureToClosestLine(alignx, aligny);
		}
		
		#endregion
	}
}
