
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
	internal sealed class VisualMiddleDouble : BaseVisualGeometrySidedef
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		private bool repeatmidtex;
		private Plane topclipplane;
		private Plane bottomclipplane;
		
		#endregion

		#region ================== Properties

		#endregion

		#region ================== Constructor / Setup

		// Constructor
		public VisualMiddleDouble(BaseVisualMode mode, VisualSector vs, Sidedef s) : base(mode, vs, s)
		{
			//mxd
			geometrytype = VisualGeometryType.WALL_MIDDLE;
			partname = "mid";
			
			// Set render pass
			this.RenderPass = RenderPass.Mask;
			
			// We have no destructor
			GC.SuppressFinalize(this);
		}
		
		// This builds the geometry. Returns false when no geometry created.
		public override bool Setup()
		{
			//mxd
			if(Sidedef.LongMiddleTexture == MapSet.EmptyLongName)
			{
				base.SetVertices(null);
				return false;
			}
			
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
			SectorData sd = mode.GetSectorData(Sidedef.Sector);
			SectorData osd = mode.GetSectorData(Sidedef.Other.Sector);
			if(!osd.Updated) osd.Update();

			// Load texture
			if(Sidedef.LongMiddleTexture != MapSet.EmptyLongName) 
			{
				base.Texture = General.Map.Data.GetTextureImage(Sidedef.LongMiddleTexture);
				if(base.Texture == null || base.Texture is UnknownImage) 
				{
					base.Texture = General.Map.Data.UnknownTexture3D;
					setuponloadedtexture = Sidedef.LongMiddleTexture;
				} 
				else if(!base.Texture.IsImageLoaded) 
				{
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
			double geotop = Math.Min(Sidedef.Sector.CeilHeight, Sidedef.Other.Sector.CeilHeight);
			double geobottom = Math.Max(Sidedef.Sector.FloorHeight, Sidedef.Other.Sector.FloorHeight);
			double zoffset = Sidedef.Sector.CeilHeight - Sidedef.Other.Sector.CeilHeight; //mxd

			// When lower unpegged is set, the middle texture is bound to the bottom
			if(Sidedef.Line.IsFlagSet(General.Map.Config.LowerUnpeggedFlag)) 
				tp.tlt.y = tsz.y - (geotop - geobottom);
			
			if(zoffset > 0) tp.tlt.y -= zoffset; //mxd
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

			// Keep top and bottom planes for intersection testing
			top = sd.Ceiling.plane;
			bottom = sd.Floor.plane;

			// Create initial polygon, which is just a quad between floor and ceiling
			WallPolygon poly = new WallPolygon();
			poly.Add(new Vector3D(vl.x, vl.y, sd.Floor.plane.GetZ(vl)));
			poly.Add(new Vector3D(vl.x, vl.y, sd.Ceiling.plane.GetZ(vl)));
			poly.Add(new Vector3D(vr.x, vr.y, sd.Ceiling.plane.GetZ(vr)));
			poly.Add(new Vector3D(vr.x, vr.y, sd.Floor.plane.GetZ(vr)));

			// Determine initial color
			int lightlevel = lightabsolute ? lightvalue : sd.Ceiling.brightnessbelow + lightvalue;

			//mxd. This calculates light with doom-style wall shading
			PixelColor wallbrightness = PixelColor.FromInt(mode.CalculateBrightness(lightlevel, Sidedef));
			PixelColor wallcolor = PixelColor.Modulate(sd.Ceiling.colorbelow, wallbrightness);
			fogfactor = CalculateFogFactor(lightlevel);
			poly.color = wallcolor.WithAlpha(255).ToInt();

			// Cut off the part below the other floor and above the other ceiling
			CropPoly(ref poly, osd.Ceiling.plane, true);
			CropPoly(ref poly, osd.Floor.plane, true);

			// Determine if we should repeat the middle texture. In UDMF this is done with a flag, in Hexen with
			// a argument to the 121:Line_SetIdentification. See https://www.zdoom.org/w/index.php?title=Line_SetIdentification
			if (General.Map.UDMF)
				repeatmidtex = Sidedef.IsFlagSet("wrapmidtex") || Sidedef.Line.IsFlagSet("wrapmidtex"); //mxd
			else if (General.Map.HEXEN)
				repeatmidtex = Sidedef.Line.Action == 121 && (Sidedef.Line.Args[1] & 16) == 16;
			else
				repeatmidtex = false;

			if(!repeatmidtex) 
			{
				// First determine the visible portion of the texture
				double textop;

				// Determine top portion height
				if(Sidedef.Line.IsFlagSet(General.Map.Config.LowerUnpeggedFlag))
					textop = geobottom + tof.y + Math.Abs(tsz.y);
				else
					textop = geotop + tof.y;

				// Calculate bottom portion height
				double texbottom = textop - Math.Abs(tsz.y);

				// Create crop planes (we also need these for intersection testing)
				if (General.Map.Config.SidedefTextureSkewing)
				{
					(topclipplane, bottomclipplane) = CreateSkewClipPlanes(textop, texbottom, sd, osd);
				}
				else
				{
					topclipplane = new Plane(new Vector3D(0, 0, -1), textop);
					bottomclipplane = new Plane(new Vector3D(0, 0, 1), -texbottom);
				}

				// Crop polygon by these heights
				CropPoly(ref poly, topclipplane, true);
				CropPoly(ref poly, bottomclipplane, true);
			}

			//mxd. In(G)ZDoom, middle sidedef parts are not clipped by extrafloors of any type...
			List<WallPolygon> polygons = new List<WallPolygon> { poly };
			//ClipExtraFloors(polygons, sd.ExtraFloors, true); //mxd
			//ClipExtraFloors(polygons, osd.ExtraFloors, true); //mxd

			//if(polygons.Count > 0) 
			//{
				// Keep top and bottom planes for intersection testing
				top = osd.Ceiling.plane;
				bottom = osd.Floor.plane;

				// Process the polygon and create vertices
				List<WorldVertex> verts = CreatePolygonVertices(polygons, tp, sd, lightvalue, lightabsolute);
				if(verts.Count > 2) 
				{
					// Apply alpha to vertices
					byte alpha = SetLinedefRenderstyle(true);
					if(alpha < 255) 
					{
						for(int i = 0; i < verts.Count; i++) 
						{
							WorldVertex v = verts[i];
							v.c = PixelColor.FromInt(v.c).WithAlpha(alpha).ToInt();
							verts[i] = v;
						}
					}

					base.SetVertices(verts);

					// Set skewing
					UpdateSkew();

					return true;
				}
			//}
			
			base.SetVertices(null); //mxd
			return false;
		}
		
		#endregion

		#region ================== Methods

		// This performs a fast test in object picking
		public override bool PickFastReject(Vector3D from, Vector3D to, Vector3D dir)
		{
			if(!repeatmidtex)
			{
				// When the texture is not repeated, leave when outside crop planes
				if((pickintersect.z < bottomclipplane.GetZ(pickintersect)) ||
				   (pickintersect.z > topclipplane.GetZ(pickintersect)))
				   return false;
			}
			
			return base.PickFastReject(from, to, dir);
		}

		//mxd. Alpha based picking
		public override bool PickAccurate(Vector3D from, Vector3D to, Vector3D dir, ref double u_ray) 
		{
			if(!BuilderPlug.Me.AlphaBasedTextureHighlighting || !Texture.IsImageLoaded || (!Texture.IsTranslucent && !Texture.IsMasked)) return base.PickAccurate(from, to, dir, ref u_ray);

			double u;
			new Line2D(from, to).GetIntersection(Sidedef.Line.Line, out u);
			if(Sidedef != Sidedef.Line.Front) u = 1.0f - u;

            // Some textures (e.g. HiResImage) may lie about their size, so use bitmap size instead
            int imageWidth = Texture.GetAlphaTestWidth();
            int imageHeight = Texture.GetAlphaTestHeight();

            // Determine texture scale...
            Vector2D imgscale = new Vector2D((double)Texture.Width / imageWidth, (double)Texture.Height / imageHeight);
            Vector2D texscale = (Texture is HiResImage) ? imgscale * Texture.Scale : Texture.Scale;

            // Get correct offset to texture space...
            int ox = (int)Math.Floor((u * Sidedef.Line.Length * UniFields.GetFloat(Sidedef.Fields, "scalex_mid", 1.0f) / texscale.x
                + ((Sidedef.OffsetX + UniFields.GetFloat(Sidedef.Fields, "offsetx_mid")) / imgscale.x))
                % imageWidth);

            int oy;
            if (repeatmidtex)
            {
                bool pegbottom = Sidedef.Line.IsFlagSet(General.Map.Config.LowerUnpeggedFlag);
				double zoffset = (pegbottom ? Sidedef.Sector.FloorHeight : Sidedef.Sector.CeilHeight);
                oy = (int)Math.Floor(((pickintersect.z - zoffset) * UniFields.GetFloat(Sidedef.Fields, "scaley_mid", 1.0f) / texscale.y
                    - ((Sidedef.OffsetY - UniFields.GetFloat(Sidedef.Fields, "offsety_mid")) / imgscale.y))
                    % imageHeight);
            }
            else
            {
				double zoffset = bottomclipplane.GetZ(pickintersect);
                oy = (int)Math.Ceiling(((pickintersect.z - zoffset) * UniFields.GetFloat(Sidedef.Fields, "scaley_mid", 1.0f) / texscale.y) % imageHeight);
            }

            // Make sure offsets are inside of texture dimensions...
            if (ox < 0) ox += imageWidth;
            if (oy < 0) oy += imageHeight;

            // Check pixel alpha
            Point pixelpos = new Point(General.Clamp(ox, 0, imageWidth - 1), General.Clamp(imageHeight - oy, 0, imageHeight - 1));
            return (Texture.AlphaTestPixel(pixelpos.X, pixelpos.Y) && base.PickAccurate(from, to, dir, ref u_ray));
		}
		
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

			//mxd. Don't clamp offsetY of clipped mid textures
			bool dontClamp = (!textureloaded || (!Sidedef.IsFlagSet("wrapmidtex") && !Sidedef.Line.IsFlagSet("wrapmidtex")));
			Sidedef.Fields["offsety_mid"] = new UniValue(UniversalType.Float, GetNewTexutreOffset(oldy, offsety, dontClamp ? double.MaxValue : Texture.Height)); // biwa
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
				int skewtype = Sidedef.Fields.GetValue("skew_middle", 0);

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
						(float)((rightz - leftz) / Sidedef.Line.Length * ((double)Texture.Width / Texture.Height) * Sidedef.Fields.GetValue("scaley_mid", 1.0) / Sidedef.Fields.GetValue("scalex_mid", 1.0))
						);
				}
			}
			else if (General.Map.Config.SkewStyle == Config.SkewStyle.EternityEngine)
			{
				string skewtype = Sidedef.Fields.GetValue("skew_middle_type", "none");

				if ((skewtype == "front_floor" || skewtype == "front_ceiling" || skewtype == "back_floor" || skewtype == "back_ceiling") && Texture != null)
				{
					double leftz, rightz;

					if (skewtype == "front_floor")
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
					else if (skewtype == "front_ceiling")
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
					else if (skewtype == "back_floor")
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
					else // Back ceiling
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

					skew = new Vector2f(
						Vertices.Min(v => v.u), // Get the lowest horizontal texture offset
						(float)((rightz - leftz) / Sidedef.Line.Length * ((double)Texture.Width / Texture.Height))
						);
				}
			}
		}

		/// <summary>
		/// Creates clipping planes for skewed sidedefs
		/// </summary>
		/// <param name="textop">The texture's top position</param>
		/// <param name="texbottom">The texture's bottom position</param>
		/// <param name="sd">This sidedef's sector data</param>
		/// <param name="osd">The other sidedef's sector data</param>
		/// <returns>The top and bottom clipping planes</returns>
		private (Plane, Plane) CreateSkewClipPlanes(double textop, double texbottom, SectorData sd, SectorData osd)
		{
			if (General.Map.Config.SkewStyle == Config.SkewStyle.GZDoom)
			{
				int skewtype = Sidedef.Fields.GetValue("skew_middle", 0);

				if(skewtype > 0 && skewtype <= 4)
				{
					double diff;
					Line2D line;

					if (skewtype == 1)
						(diff, line) = GetZDiff(true, Sidedef.IsFront);
					else if(skewtype == 2)
						(diff, line) = GetZDiff(false, Sidedef.IsFront);
					else if(skewtype == 3)
						(diff, line) = GetZDiff(true, !Sidedef.IsFront);
					else // skewtype 4
						(diff, line) = GetZDiff(false, !Sidedef.IsFront);

					Vector2D v3 = line.GetPerpendicular() * 10 + line.v1;

					Plane topplane = new Plane(
						new Vector3D(line.v1, textop),
						new Vector3D(line.v2, textop + diff),
						new Vector3D(v3, textop),
						false);

					Plane bottomplane = new Plane(
						new Vector3D(line.v1, texbottom),
						new Vector3D(line.v2, texbottom + diff),
						new Vector3D(v3, textop),
						true);

					return (topplane, bottomplane);
				}
				else // Invalid skew type
				{
					return (
						new Plane(new Vector3D(0, 0, -1), textop),
						new Plane(new Vector3D(0, 0, 1), -texbottom)
					);
				}
			}
			else if (General.Map.Config.SkewStyle == Config.SkewStyle.EternityEngine)
			{
				string skewtype = Sidedef.Fields.GetValue("skew_middle_type", "none");
				if ((skewtype == "front_floor" || skewtype == "front_ceiling" || skewtype == "back_floor" || skewtype == "back_ceiling") && Texture != null)
				{
					double diff;
					Line2D line;

					if (skewtype == "front_ceiling")
						(diff, line) = GetZDiff(false, true);
					else if (skewtype == "back_ceiling")
						(diff, line) = GetZDiff(false, false);
					else if (skewtype == "front_floor")
						(diff, line) = GetZDiff(true, true);
					else // back_floor
						(diff, line) = GetZDiff(true, false);

					Vector2D v3 = line.GetPerpendicular() * 10 + line.v1;

					Plane topplane = new Plane(
						new Vector3D(line.v1, textop),
						new Vector3D(line.v2, textop + diff),
						new Vector3D(v3, textop),
						false);

					Plane bottomplane = new Plane(
						new Vector3D(line.v1, texbottom),
						new Vector3D(line.v2, texbottom + diff),
						new Vector3D(v3, textop),
						true);

					return (topplane, bottomplane);

				}
				else // Invalid skew type
				{
					return (
						new Plane(new Vector3D(0, 0, -1), textop),
						new Plane(new Vector3D(0, 0, 1), -texbottom)
					);
				}
			}
			else // No matching skew style
			{
				return (
					new Plane(new Vector3D(0, 0, -1), textop),
					new Plane(new Vector3D(0, 0, 1), -texbottom)
				);
			}

			// Returns the z position at the start and end vertices of the line, and a line that always goes from left to right
			(double, Line2D) GetZDiff(bool floor, bool front)
			{
				double leftz, rightz;
				Vector2D ls, le;

				if (Sidedef.IsFront)
				{
					ls = Sidedef.Line.Start.Position;
					le = Sidedef.Line.End.Position;

					if (floor)
					{
						if (front)
						{
							leftz = sd.Floor.plane.GetZ(Sidedef.Line.Start.Position);
							rightz = sd.Floor.plane.GetZ(Sidedef.Line.End.Position);
						}
						else
						{
							leftz = osd.Floor.plane.GetZ(Sidedef.Line.Start.Position);
							rightz = osd.Floor.plane.GetZ(Sidedef.Line.End.Position);
						}
					}
					else
					{
						if (front)
						{
							leftz = sd.Ceiling.plane.GetZ(Sidedef.Line.Start.Position);
							rightz = sd.Ceiling.plane.GetZ(Sidedef.Line.End.Position);
						}
						else
						{
							leftz = osd.Ceiling.plane.GetZ(Sidedef.Line.Start.Position);
							rightz = osd.Ceiling.plane.GetZ(Sidedef.Line.End.Position);
						}
					}
				}
				else
				{
					ls = Sidedef.Line.End.Position;
					le = Sidedef.Line.Start.Position;

					if (floor)
					{
						if (front)
						{
							leftz = osd.Floor.plane.GetZ(Sidedef.Line.End.Position);
							rightz = osd.Floor.plane.GetZ(Sidedef.Line.Start.Position);
						}
						else
						{
							leftz = sd.Floor.plane.GetZ(Sidedef.Line.End.Position);
							rightz = sd.Floor.plane.GetZ(Sidedef.Line.Start.Position);
						}
					}
					else
					{
						if (front)
						{
							leftz = osd.Ceiling.plane.GetZ(Sidedef.Line.End.Position);
							rightz = osd.Ceiling.plane.GetZ(Sidedef.Line.Start.Position);
						}
						else
						{
							leftz = sd.Ceiling.plane.GetZ(Sidedef.Line.End.Position);
							rightz = sd.Ceiling.plane.GetZ(Sidedef.Line.Start.Position);
						}
					}
				}

				return (rightz - leftz, new Line2D(ls, le));
			}
		}

		#endregion
	}
}
