
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
using System.Linq;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Types;
using CodeImp.DoomBuilder.VisualModes;
using CodeImp.DoomBuilder.Data;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	internal sealed class VisualUpper : BaseVisualGeometrySidedef
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		#endregion

		#region ================== Properties

		#endregion

		#region ================== Constructor / Setup

		// Constructor
		public VisualUpper(BaseVisualMode mode, VisualSector vs, Sidedef s) : base(mode, vs, s)
		{
			//mxd
			geometrytype = VisualGeometryType.WALL_UPPER;
			partname = "top";
			
			// We have no destructor
			GC.SuppressFinalize(this);
		}

		// This builds the geometry. Returns false when no geometry created.
		public override bool Setup()
		{
			Vector2D vl, vr;
			
			// Left and right vertices for this sidedef
			if(Sidedef.IsFront)
			{
				vl = new Vector2D(Sidedef.Line.Start.Position.x, Sidedef.Line.Start.Position.y);
				vr = new Vector2D(Sidedef.Line.End.Position.x, Sidedef.Line.End.Position.y);
			}
			else
			{
				vl = new Vector2D(Sidedef.Line.End.Position.x, Sidedef.Line.End.Position.y);
				vr = new Vector2D(Sidedef.Line.Start.Position.x, Sidedef.Line.Start.Position.y);
			}
			
			// Load sector data
			SectorData sd = Sector.GetSectorData();
			SectorData osd = mode.GetSectorData(Sidedef.Other.Sector);
			if(!osd.Updated) osd.Update();

			//mxd
			double vlzc = sd.Ceiling.plane.GetZ(vl);
			double vrzc = sd.Ceiling.plane.GetZ(vr);

			//mxd. Side is visible when our sector's ceiling is higher than the other's at any vertex
			if (!(vlzc > osd.Ceiling.plane.GetZ(vl) || vrzc > osd.Ceiling.plane.GetZ(vr)))
			{
				base.SetVertices(null);
				return false;
			}

			//mxd. Apply sky hack?
			UpdateSkyRenderFlag();

			//mxd. lightfog flag support
			int lightvalue;
			bool lightabsolute;
			GetLightValue(out lightvalue, out lightabsolute);

			Vector2D tscale = new Vector2D(Sidedef.Fields.GetValue("scalex_top", 1.0),
										   Sidedef.Fields.GetValue("scaley_top", 1.0));
            Vector2D tscaleAbs = new Vector2D(Math.Abs(tscale.x), Math.Abs(tscale.y));
            Vector2D toffset = new Vector2D(Sidedef.Fields.GetValue("offsetx_top", 0.0),
											Sidedef.Fields.GetValue("offsety_top", 0.0));
			
			// Texture given?
			if((Sidedef.LongHighTexture != MapSet.EmptyLongName))
			{
				// Load texture
				base.Texture = General.Map.Data.GetTextureImage(Sidedef.LongHighTexture);
				if(base.Texture == null || base.Texture is UnknownImage)
				{
					base.Texture = General.Map.Data.UnknownTexture3D;
					setuponloadedtexture = Sidedef.LongHighTexture;
				}
				else if (!base.Texture.IsImageLoaded)
                {
					setuponloadedtexture = Sidedef.LongHighTexture;
				}
			}
			else
			{
				// Use missing texture
				base.Texture = General.Map.Data.MissingTexture3D;
				setuponloadedtexture = 0;
			}

			// Get texture scaled size. Round up, because that's apparently what GZDoom does
			Vector2D tsz = new Vector2D(Math.Ceiling(base.Texture.ScaledWidth / tscale.x), Math.Ceiling(base.Texture.ScaledHeight / tscale.y));

			// Get texture offsets
			Vector2D tof = new Vector2D(Sidedef.OffsetX, Sidedef.OffsetY);

			tof = tof + toffset;

			// biwa. Also take the ForceWorldPanning MAPINFO entry into account
			if (General.Map.Config.ScaledTextureOffsets && (!base.Texture.WorldPanning && !General.Map.Data.MapInfo.ForceWorldPanning))
			{
				tof = tof / tscaleAbs;
				tof = tof * base.Texture.Scale;

				// If the texture gets replaced with a "hires" texture it adds more fuckery
				if (base.Texture is HiResImage)
					tof *= tscaleAbs;

				// Round up, since that's apparently what GZDoom does. Not sure if this is the right place or if it also has to be done earlier
				tof = new Vector2D(Math.Ceiling(tof.x), Math.Ceiling(tof.y));
			}

			// Determine texture coordinates plane as they would be in normal circumstances.
			// We can then use this plane to find any texture coordinate we need.
			// The logic here is the same as in the original VisualMiddleSingle (except that
			// the values are stored in a TexturePlane)
			// NOTE: I use a small bias for the floor height, because if the difference in
			// height is 0 then the TexturePlane doesn't work!
			TexturePlane tp = new TexturePlane();
			double ceilbias = (Sidedef.Other.Sector.CeilHeight == Sidedef.Sector.CeilHeight) ? 1.0 : 0.0;
			if(!Sidedef.Line.IsFlagSet(General.Map.Config.UpperUnpeggedFlag))
			{
				// When lower unpegged is set, the lower texture is bound to the bottom
				tp.tlt.y = tsz.y - (Sidedef.Sector.CeilHeight - Sidedef.Other.Sector.CeilHeight);
			}
			tp.trb.x = tp.tlt.x + Math.Round(Sidedef.Line.Length); //mxd. (G)ZDoom snaps texture coordinates to integral linedef length
			tp.trb.y = tp.tlt.y + (Sidedef.Sector.CeilHeight - (Sidedef.Other.Sector.CeilHeight + ceilbias));
			
			// Apply texture offset
			tp.tlt += tof;
			tp.trb += tof;
			
			// Transform pixel coordinates to texture coordinates
			tp.tlt /= tsz;
			tp.trb /= tsz;
			
			// Left top and right bottom of the geometry that
			tp.vlt = new Vector3D(vl.x, vl.y, Sidedef.Sector.CeilHeight);
			tp.vrb = new Vector3D(vr.x, vr.y, Sidedef.Other.Sector.CeilHeight + ceilbias);
			
			// Make the right-top coordinates
			tp.trt = new Vector2D(tp.trb.x, tp.tlt.y);
			tp.vrt = new Vector3D(tp.vrb.x, tp.vrb.y, tp.vlt.z);
			
			// Create initial polygon, which is just a quad between floor and ceiling
			WallPolygon poly = new WallPolygon();
			poly.Add(new Vector3D(vl.x, vl.y, sd.Floor.plane.GetZ(vl)));
			poly.Add(new Vector3D(vl.x, vl.y, vlzc));
			poly.Add(new Vector3D(vr.x, vr.y, vrzc));
			poly.Add(new Vector3D(vr.x, vr.y, sd.Floor.plane.GetZ(vr)));
			
			// Determine initial color
			int lightlevel = lightabsolute ? lightvalue : sd.Ceiling.brightnessbelow + lightvalue;

			//mxd. This calculates light with doom-style wall shading
			PixelColor wallbrightness = PixelColor.FromInt(mode.CalculateBrightness(lightlevel, Sidedef));
			PixelColor wallcolor = PixelColor.Modulate(sd.Ceiling.colorbelow, wallbrightness);
			fogfactor = CalculateFogFactor(lightlevel);
			poly.color = wallcolor.WithAlpha(255).ToInt();
			
			// Cut off the part below the other ceiling
			CropPoly(ref poly, osd.Ceiling.plane, false);
			
			// Cut out pieces that overlap 3D floors in this sector
			List<WallPolygon> polygons = new List<WallPolygon> { poly };
			ClipExtraFloors(polygons, sd.ExtraFloors, false); //mxd

			if(polygons.Count > 0)
			{
				// Keep top and bottom planes for intersection testing
				Vector2D linecenter = Sidedef.Line.GetCenterPoint(); //mxd. Our sector's floor can be higher than the other sector's ceiling!
				top = sd.Ceiling.plane;
				bottom = (osd.Ceiling.plane.GetZ(linecenter) > sd.Floor.plane.GetZ(linecenter) ? osd.Ceiling.plane : sd.Floor.plane);
				
				// Process the polygon and create vertices
				List<WorldVertex> verts = CreatePolygonVertices(polygons, tp, sd, lightvalue, lightabsolute);
				if(verts.Count > 2)
				{
					base.SetVertices(verts);

					// Set skewing
					UpdateSkew();

					return true;
				}
			}
			
			base.SetVertices(null); //mxd
			return false;
		}

		//mxd
		internal void UpdateSkyRenderFlag()
		{
			renderassky = (Sidedef.Other != null && Sidedef.Sector != null && Sidedef.Other.Sector != null
				&& Sidedef.Other.Sector.HasSkyCeiling
				&& (Sidedef.Sector.HasSkyCeiling || Sidedef.HighTexture == "-")
				);
		}
		
		#endregion

		#region ================== Methods

		// Return texture name
		public override string GetTextureName()
		{
			return this.Sidedef.HighTexture;
		}

		// This changes the texture
		protected override void SetTexture(string texturename)
		{
			this.Sidedef.SetTextureHigh(texturename);
			General.Map.Data.UpdateUsedTextures();
			this.Setup();

			//mxd. Other sector also may require updating
			SectorData sd = mode.GetSectorData(Sidedef.Sector);
			if(sd.ExtraFloors.Count > 0)
				((BaseVisualSector)mode.GetVisualSector(Sidedef.Sector)).Rebuild();
		}

		protected override void SetTextureOffsetX(int x)
		{
			Sidedef.Fields.BeforeFieldsChange();
			Sidedef.Fields["offsetx_top"] = new UniValue(UniversalType.Float, (double)x);
		}

		protected override void SetTextureOffsetY(int y)
		{
			Sidedef.Fields.BeforeFieldsChange();
			Sidedef.Fields["offsety_top"] = new UniValue(UniversalType.Float, (double)y);
		}

		protected override void MoveTextureOffset(int offsetx, int offsety)
		{
			Sidedef.Fields.BeforeFieldsChange();
			bool worldpanning = this.Texture.WorldPanning || General.Map.Data.MapInfo.ForceWorldPanning;
			double oldx = Sidedef.Fields.GetValue("offsetx_top", 0.0);
			double oldy = Sidedef.Fields.GetValue("offsety_top", 0.0);
			double scalex = Sidedef.Fields.GetValue("scalex_top", 1.0);
			double scaley = Sidedef.Fields.GetValue("scaley_top", 1.0);
			bool textureloaded = (Texture != null && Texture.IsImageLoaded); //mxd
			double width = textureloaded ? (worldpanning ? this.Texture.ScaledWidth / scalex : this.Texture.Width) : -1; // biwa
			double height = textureloaded ? (worldpanning ? this.Texture.ScaledHeight / scaley : this.Texture.Height) : -1; // biwa

			Sidedef.Fields["offsetx_top"] = new UniValue(UniversalType.Float, GetNewTexutreOffset(oldx, offsetx, width)); //mxd // biwa
			Sidedef.Fields["offsety_top"] = new UniValue(UniversalType.Float, GetNewTexutreOffset(oldy, offsety, height)); //mxd // biwa
		}

		protected override Point GetTextureOffset()
		{
			double oldx = Sidedef.Fields.GetValue("offsetx_top", 0.0);
			double oldy = Sidedef.Fields.GetValue("offsety_top", 0.0);
			return new Point((int)oldx, (int)oldy);
		}

		//mxd
		protected override void ResetTextureScale() 
		{
			Sidedef.Fields.BeforeFieldsChange();
			if(Sidedef.Fields.ContainsKey("scalex_top")) Sidedef.Fields.Remove("scalex_top");
			if(Sidedef.Fields.ContainsKey("scaley_top")) Sidedef.Fields.Remove("scaley_top");
		}

		//mxd
		public override void OnTextureFit(FitTextureOptions options) 
		{
			if(!General.Map.UDMF) return;
			if(!Sidedef.HighRequired() || string.IsNullOrEmpty(Sidedef.HighTexture) || Sidedef.HighTexture == "-" || !Texture.IsImageLoaded) return;
			FitTexture(options);
			Setup();
		}

		/// <summary>
		/// Updates the value for texture skewing. Has to be done after the texture is set.
		/// </summary>
		public void UpdateSkew()
		{
			// Reset
			skew = new Vector2f(0.0f);

			if (!General.Map.Config.SidedefTextureSkewing)
				return;

			if (General.Map.Config.SkewStyle == Config.SkewStyle.GZDoom)
			{
				int skewtype = Sidedef.Fields.GetValue("skew_top", 0);

				if (skewtype > 0 && skewtype <= 4 && Texture != null)
				{
					Plane plane;
					Vector2D start = Sidedef.IsFront ? Sidedef.Line.Start.Position : Sidedef.Line.End.Position;
					Vector2D end = Sidedef.IsFront ? Sidedef.Line.End.Position : Sidedef.Line.Start.Position;

					if (skewtype == 1)
						plane = Sector.GetSectorData().Floor.plane;
					else if (skewtype == 2)
						plane = Sector.GetSectorData().Ceiling.plane;
					else if (skewtype == 3)
						plane = mode.GetSectorData(Sidedef.Other.Sector).Floor.plane;
					else // skewtype 4
						plane = mode.GetSectorData(Sidedef.Other.Sector).Ceiling.plane;

					double leftz = plane.GetZ(start);
					double rightz = plane.GetZ(end);

					skew = new Vector2f(
						Vertices.Min(v => v.u), // Get the lowest horizontal texture offset
						(float)((rightz - leftz) / Sidedef.Line.Length * ((double)Texture.Width / Texture.Height) * Sidedef.Fields.GetValue("scaley_top", 1.0) / Sidedef.Fields.GetValue("scalex_top", 1.0))
						);
				}
			}
			else if (General.Map.Config.SkewStyle == Config.SkewStyle.EternityEngine)
			{
				string skewtype = Sidedef.Fields.GetValue("skew_top_type", "none");

				if ((skewtype == "front_floor" || skewtype == "front_ceiling" || skewtype == "back_floor" || skewtype == "back_ceiling") && Texture != null)
				{
					double leftz, rightz;

					if (skewtype == "front_ceiling")
					{
						if (Sidedef.IsFront)
						{
							Plane plane = Sector.GetSectorData().Ceiling.plane;
							leftz = plane.GetZ(Sidedef.Line.Start.Position);
							rightz = plane.GetZ(Sidedef.Line.End.Position);
						}
						else
						{
							Plane plane = mode.GetSectorData(Sidedef.Other.Sector).Ceiling.plane;
							leftz = plane.GetZ(Sidedef.Line.End.Position);
							rightz = plane.GetZ(Sidedef.Line.Start.Position);
						}
					}
					else if (skewtype == "back_ceiling")
					{
						if (Sidedef.IsFront)
						{
							Plane plane = mode.GetSectorData(Sidedef.Other.Sector).Ceiling.plane;
							leftz = plane.GetZ(Sidedef.Line.Start.Position);
							rightz = plane.GetZ(Sidedef.Line.End.Position);
						}
						else
						{
							Plane plane = Sector.GetSectorData().Ceiling.plane;
							leftz = plane.GetZ(Sidedef.Line.End.Position);
							rightz = plane.GetZ(Sidedef.Line.Start.Position);
						}
					}
					else if (skewtype == "front_floor")
					{
						if (Sidedef.IsFront)
						{
							Plane plane = Sector.GetSectorData().Floor.plane;
							leftz = plane.GetZ(Sidedef.Line.Start.Position);
							rightz = plane.GetZ(Sidedef.Line.End.Position);
						}
						else
						{
							Plane plane = mode.GetSectorData(Sidedef.Other.Sector).Floor.plane;
							leftz = plane.GetZ(Sidedef.Line.End.Position);
							rightz = plane.GetZ(Sidedef.Line.Start.Position);
						}
					}
					else // Back floor
					{
						if (Sidedef.IsFront)
						{
							Plane plane = mode.GetSectorData(Sidedef.Other.Sector).Floor.plane;
							leftz = plane.GetZ(Sidedef.Line.Start.Position);
							rightz = plane.GetZ(Sidedef.Line.End.Position);
						}
						else
						{
							Plane plane = Sector.GetSectorData().Floor.plane;
							leftz = plane.GetZ(Sidedef.Line.End.Position);
							rightz = plane.GetZ(Sidedef.Line.Start.Position);
						}
					}

					skew = new Vector2f(
						Vertices.Min(v => v.u), // Get the lowest horizontal texture offset
						(float)((rightz - leftz) / Sidedef.Line.Length * ((double)Texture.Width / Texture.Height))
						);
				}
			}
		}
		
		#endregion
	}
}
