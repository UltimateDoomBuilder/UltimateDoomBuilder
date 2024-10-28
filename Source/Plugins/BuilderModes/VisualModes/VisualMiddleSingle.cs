
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
	internal sealed class VisualMiddleSingle : BaseVisualGeometrySidedef
	{
		#region ================== Constants

		#endregion
		
		#region ================== Variables

		#endregion
		
		#region ================== Properties

		#endregion
		
		#region ================== Constructor / Setup
		
		// Constructor
		public VisualMiddleSingle(BaseVisualMode mode, VisualSector vs, Sidedef s) : base(mode, vs, s)
		{
			//mxd
			geometrytype = VisualGeometryType.WALL_MIDDLE;
			partname = "mid";
			
			// We have no destructor
			GC.SuppressFinalize(this);
		}

		// This builds the geometry. Returns false when no geometry created.
		public override bool Setup()
		{
			Vector2D vl, vr;

			//mxd. lightfog flag support
			int lightvalue;
			bool lightabsolute;
			GetLightValue(out lightvalue, out lightabsolute);

			Vector2D tscale = new Vector2D(Sidedef.Fields.GetValue("scalex_mid", 1.0),
										   Sidedef.Fields.GetValue("scaley_mid", 1.0));
			Vector2D tscaleAbs = new Vector2D(Math.Abs(tscale.x), Math.Abs(tscale.y));
			Vector2D toffset = new Vector2D(Sidedef.Fields.GetValue("offsetx_mid", 0.0),
											Sidedef.Fields.GetValue("offsety_mid", 0.0));

			// Left and right vertices for this sidedef
			if (Sidedef.IsFront)
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
			SectorData sd = mode.GetSectorData(Sidedef.Sector);

			// Texture given?
			if (Sidedef.LongMiddleTexture != MapSet.EmptyLongName)
			{
				// Load texture
				base.Texture = General.Map.Data.GetTextureImage(Sidedef.LongMiddleTexture);
				if (base.Texture == null || base.Texture is UnknownImage)
				{
					base.Texture = General.Map.Data.UnknownTexture3D;
					setuponloadedtexture = Sidedef.LongMiddleTexture;
				}
				else
				{
					if (!base.Texture.IsImageLoaded)
						setuponloadedtexture = Sidedef.LongMiddleTexture;
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
			double floorbias = (Sidedef.Sector.CeilHeight == Sidedef.Sector.FloorHeight) ? 1.0 : 0.0;
			if (Sidedef.Line.IsFlagSet(General.Map.Config.LowerUnpeggedFlag))
			{
				// When lower unpegged is set, the middle texture is bound to the bottom
				tp.tlt.y = tsz.y - (Sidedef.Sector.CeilHeight - Sidedef.Sector.FloorHeight);
			}
			tp.trb.x = tp.tlt.x + Math.Round(Sidedef.Line.Length); //mxd. (G)ZDoom snaps texture coordinates to integral linedef length
			tp.trb.y = tp.tlt.y + (Sidedef.Sector.CeilHeight - (Sidedef.Sector.FloorHeight + floorbias));

			// Apply texture offset
			tp.tlt += tof;
			tp.trb += tof;

			// Transform pixel coordinates to texture coordinates
			tp.tlt /= tsz;
			tp.trb /= tsz;

			// Left top and right bottom of the geometry that
			tp.vlt = new Vector3D(vl.x, vl.y, Sidedef.Sector.CeilHeight);
			tp.vrb = new Vector3D(vr.x, vr.y, Sidedef.Sector.FloorHeight + floorbias);

			// Make the right-top coordinates
			tp.trt = new Vector2D(tp.trb.x, tp.tlt.y);
			tp.vrt = new Vector3D(tp.vrb.x, tp.vrb.y, tp.vlt.z);

			// Get ceiling and floor heights
			double fl = sd.Floor.plane.GetZ(vl);
			double fr = sd.Floor.plane.GetZ(vr);
			double cl = sd.Ceiling.plane.GetZ(vl);
			double cr = sd.Ceiling.plane.GetZ(vr);

			// Anything to see?
			if (((cl - fl) > 0.01f) || ((cr - fr) > 0.01f))
			{
				// Keep top and bottom planes for intersection testing
				top = sd.Ceiling.plane;
				bottom = sd.Floor.plane;

				// Create initial polygon, which is just a quad between floor and ceiling
				WallPolygon poly = new WallPolygon();
				poly.Add(new Vector3D(vl.x, vl.y, fl));
				poly.Add(new Vector3D(vl.x, vl.y, cl));
				poly.Add(new Vector3D(vr.x, vr.y, cr));
				poly.Add(new Vector3D(vr.x, vr.y, fr));

				// Determine initial color
				int lightlevel = lightabsolute ? lightvalue : sd.Ceiling.brightnessbelow + lightvalue;

				//mxd. This calculates light with doom-style wall shading
				PixelColor wallbrightness = PixelColor.FromInt(mode.CalculateBrightness(lightlevel, Sidedef));
				PixelColor wallcolor = PixelColor.Modulate(sd.Ceiling.colorbelow, wallbrightness);
				fogfactor = CalculateFogFactor(lightlevel);
				poly.color = wallcolor.WithAlpha(255).ToInt();

				// Cut out pieces that overlap 3D floors in this sector
				List<WallPolygon> polygons = new List<WallPolygon> { poly };
				ClipExtraFloors(polygons, sd.ExtraFloors, false); //mxd

				if (polygons.Count > 0)
				{
					// Process the polygon and create vertices
					List<WorldVertex> verts = CreatePolygonVertices(polygons, tp, sd, lightvalue, lightabsolute);
					if (verts.Count > 2)
					{
						base.SetVertices(verts);

						// Set skewing
						UpdateSkew();

						return true;
					}
				}
			}

			base.SetVertices(null); //mxd
			return false;
		}

		#endregion
		
		#region ================== Methods

		// Return texture name
		public override string GetTextureName()
		{
			return this.Sidedef.MiddleTexture;
		}

		// This changes the texture
		protected override void SetTexture(string texturename)
		{
			this.Sidedef.SetTextureMid(texturename);
			General.Map.Data.UpdateUsedTextures();
			this.Setup();
		}

		protected override void SetTextureOffsetX(int x)
		{
			Sidedef.Fields.BeforeFieldsChange();
			Sidedef.Fields["offsetx_mid"] = new UniValue(UniversalType.Float, (double)x);
		}

		protected override void SetTextureOffsetY(int y)
		{
			Sidedef.Fields.BeforeFieldsChange();
			Sidedef.Fields["offsety_mid"] = new UniValue(UniversalType.Float, (double)y);
		}

		protected override void MoveTextureOffset(int offsetx, int offsety)
		{
			Sidedef.Fields.BeforeFieldsChange();
			bool worldpanning = this.Texture.WorldPanning || General.Map.Data.MapInfo.ForceWorldPanning;
			double oldx = Sidedef.Fields.GetValue("offsetx_mid", 0.0);
			double oldy = Sidedef.Fields.GetValue("offsety_mid", 0.0);
			double scalex = Sidedef.Fields.GetValue("scalex_mid", 1.0);
			double scaley = Sidedef.Fields.GetValue("scaley_mid", 1.0);
			bool textureloaded = (Texture != null && Texture.IsImageLoaded); //mxd
			double width = textureloaded ? (worldpanning ? this.Texture.ScaledWidth / scalex : this.Texture.Width) : -1; // biwa
			double height = textureloaded ? (worldpanning ? this.Texture.ScaledHeight / scaley : this.Texture.Height) : -1; // biwa

			Sidedef.Fields["offsetx_mid"] = new UniValue(UniversalType.Float, GetNewTexutreOffset(oldx, offsetx, width)); //mxd // biwa
			Sidedef.Fields["offsety_mid"] = new UniValue(UniversalType.Float, GetNewTexutreOffset(oldy, offsety, height)); //mxd // biwa
		}

		protected override Point GetTextureOffset()
		{
			double oldx = Sidedef.Fields.GetValue("offsetx_mid", 0.0);
			double oldy = Sidedef.Fields.GetValue("offsety_mid", 0.0);
			return new Point((int)oldx, (int)oldy);
		}

		//mxd
		protected override void ResetTextureScale() 
		{
			Sidedef.Fields.BeforeFieldsChange();
			if(Sidedef.Fields.ContainsKey("scalex_mid")) Sidedef.Fields.Remove("scalex_mid");
			if(Sidedef.Fields.ContainsKey("scaley_mid")) Sidedef.Fields.Remove("scaley_mid");
		}

		//mxd
		public override void OnTextureFit(FitTextureOptions options) 
		{
			if(!General.Map.UDMF) return;
			if(string.IsNullOrEmpty(Sidedef.MiddleTexture) || Sidedef.MiddleTexture == "-" || !Texture.IsImageLoaded) return;
			FitTexture(options);
			Setup();

			// Update linked effects
			SectorData sd = mode.GetSectorDataEx(Sector.Sector);
			if(sd != null) sd.Reset(true);
		}

		/// <summary>
		/// Updates the value for texture skewing. Has to be done after the texture and vertices are set.
		/// </summary>
		public void UpdateSkew()
		{
			// Reset
			skew = new Vector2f(0.0f);

			if (!General.Map.Config.SidedefTextureSkewing)
				return;

			if (General.Map.Config.SkewStyle == Config.SkewStyle.GZDoom)
			{
				int skewtype = Sidedef.Fields.GetValue("skew_middle", 0);

				if (skewtype > 0 && skewtype <= 2 && Texture != null)
				{
					Plane plane;
					Vector2D start = Sidedef.IsFront ? Sidedef.Line.Start.Position : Sidedef.Line.End.Position;
					Vector2D end = Sidedef.IsFront ? Sidedef.Line.End.Position : Sidedef.Line.Start.Position;

					if (skewtype == 1)
						plane = Sector.GetSectorData().Floor.plane;
					else // skewtype 2
						plane = Sector.GetSectorData().Ceiling.plane;

					double leftz = plane.GetZ(start);
					double rightz = plane.GetZ(end);

					skew = new Vector2f(
						Vertices.Min(v => v.u), // Get the lowest horizontal texture offset
						(float)((rightz - leftz) / Sidedef.Line.Length * ((double)Texture.Width / Texture.Height) * Sidedef.Fields.GetValue("scaley_mid", 1.0) / Sidedef.Fields.GetValue("scalex_mid", 1.0))
					);
				}
			}
			else if (General.Map.Config.SkewStyle == Config.SkewStyle.EternityEngine)
			{
				string skewtype = Sidedef.Fields.GetValue("skew_middle_type", "none");

				// We don't have to check for back because this it's single-sided
				if ((skewtype == "front_floor" || skewtype == "front_ceiling") && Texture != null)
				{
					double leftz, rightz;
					Plane plane = skewtype == "front_floor" ? Sector.GetSectorData().Floor.plane : Sector.GetSectorData().Ceiling.plane;

					leftz = plane.GetZ(Sidedef.Line.Start.Position);
					rightz = plane.GetZ(Sidedef.Line.End.Position);

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
