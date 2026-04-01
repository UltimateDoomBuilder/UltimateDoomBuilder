#region ================== Namespaces

using System;
using System.Collections.Generic;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.VisualModes;
using CodeImp.DoomBuilder.Data;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	//mxd. Used to render translucent 3D floor's inner sides
	internal sealed class VisualMiddleBack : VisualMiddle3D 
	{
		#region ================== Constructor / Setup
		
		// Constructor
		public VisualMiddleBack(BaseVisualMode mode, VisualSector vs, Sidedef s) : base(mode, vs, s) { }
		
		// This builds the geometry. Returns false when no geometry created.
		public override bool Setup() { return this.Setup(this.extrafloor); }
		public new bool Setup(Effect3DFloor extrafloor)
		{
			Sidedef sourceside = extrafloor.Linedef.Front;
			this.extrafloor = extrafloor;

			//mxd. Extrafloor may've become invalid during undo/redo...
			if(sourceside == null) return false;

			Vector2D vl, vr;

			//mxd. lightfog flag support
			int lightvalue;
			bool lightabsolute;
			GetLightValue(out lightvalue, out lightabsolute);

			Vector2D tscale = new Vector2D(sourceside.Fields.GetValue("scalex_mid", 1.0),
										   sourceside.Fields.GetValue("scaley_mid", 1.0));
            Vector2D tscaleAbs = new Vector2D(Math.Abs(tscale.x), Math.Abs(tscale.y));
            Vector2D toffset1 = new Vector2D(Sidedef.Fields.GetValue("offsetx_mid", 0.0),
											 Sidedef.Fields.GetValue("offsety_mid", 0.0));
			Vector2D toffset2 = new Vector2D(sourceside.Fields.GetValue("offsetx_mid", 0.0),
											 sourceside.Fields.GetValue("offsety_mid", 0.0));
			
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
			SectorData sd = mode.GetSectorData(Sidedef.Other.Sector);

			//mxd. which texture we must use?
			long texturelong = 0;
			if((sourceside.Line.Args[2] & (int)Effect3DFloor.Flags.UseUpperTexture) != 0) 
			{
				if(Sidedef.LongHighTexture != MapSet.EmptyLongName)
					texturelong = Sidedef.LongHighTexture;
			} 
			else if((sourceside.Line.Args[2] & (int)Effect3DFloor.Flags.UseLowerTexture) != 0) 
			{
				if(Sidedef.LongLowTexture != MapSet.EmptyLongName)
					texturelong = Sidedef.LongLowTexture;
			} 
			else if(sourceside.LongMiddleTexture != MapSet.EmptyLongName) 
			{
				texturelong = sourceside.LongMiddleTexture;
			}

			// Texture given?
			if(texturelong != 0)
			{
				// Load texture
				base.Texture = General.Map.Data.GetTextureImage(texturelong);
				if(base.Texture == null || base.Texture is UnknownImage)
				{
					base.Texture = General.Map.Data.UnknownTexture3D;
					setuponloadedtexture = texturelong;
				} 
				else if(!base.Texture.IsImageLoaded) 
				{
					setuponloadedtexture = texturelong;
				}
			} 
			else 
			{
				// Use missing texture
				base.Texture = General.Map.Data.MissingTexture3D;
				setuponloadedtexture = 0;
			}

			// Get texture scaled size
			Vector2D tsz = new Vector2D(base.Texture.ScaledWidth / tscale.x, base.Texture.ScaledHeight / tscale.y);
			
			// Get texture offsets
			Vector2D tof = new Vector2D(Sidedef.OffsetX, Sidedef.OffsetY) + new Vector2D(sourceside.OffsetX, sourceside.OffsetY);
			tof = tof + toffset1 + toffset2;
			tof = tof / tscaleAbs;
			if(General.Map.Config.ScaledTextureOffsets && !base.Texture.WorldPanning)
				tof = tof * base.Texture.Scale;

			// For Vavoom type 3D floors the ceiling is lower than floor and they are reversed.
			// We choose here.
			double sourcetopheight = extrafloor.VavoomType ? sourceside.Sector.FloorHeight : sourceside.Sector.CeilHeight;
			double sourcebottomheight = extrafloor.VavoomType ? sourceside.Sector.CeilHeight : sourceside.Sector.FloorHeight;
			
			// Determine texture coordinates plane as they would be in normal circumstances.
			// We can then use this plane to find any texture coordinate we need.
			// The logic here is the same as in the original VisualMiddleSingle (except that
			// the values are stored in a TexturePlane)
			// NOTE: I use a small bias for the floor height, because if the difference in
			// height is 0 then the TexturePlane doesn't work!
			TexturePlane tp = new TexturePlane();
			double floorbias = (sourcetopheight == sourcebottomheight) ? 1.0 : 0.0;

			tp.trb.x = tp.tlt.x + Math.Round(Sidedef.Line.Length); //mxd. (G)ZDoom snaps texture coordinates to integral linedef length
			tp.trb.y = tp.tlt.y + (sourcetopheight - sourcebottomheight) + floorbias;
			
			// Apply texture offset
			tp.tlt += tof;
			tp.trb += tof;
			
			// Transform pixel coordinates to texture coordinates
			tp.tlt /= tsz;
			tp.trb /= tsz;
			
			// Left top and right bottom of the geometry that
			tp.vlt = new Vector3D(vl.x, vl.y, sourcetopheight);
			tp.vrb = new Vector3D(vr.x, vr.y, sourcebottomheight + floorbias);
			
			// Make the right-top coordinates
			tp.trt = new Vector2D(tp.trb.x, tp.tlt.y);
			tp.vrt = new Vector3D(tp.vrb.x, tp.vrb.y, tp.vlt.z);

			// Get ceiling and floor heights
			double flo = Sector.Floor.Level.plane.GetZ(vl);
			double fro = Sector.Floor.Level.plane.GetZ(vr);
			double clo = Sector.Ceiling.Level.plane.GetZ(vl);
			double cro = Sector.Ceiling.Level.plane.GetZ(vr);

			double fle = sd.Floor.plane.GetZ(vl);
			double fre = sd.Floor.plane.GetZ(vr);
			double cle = sd.Ceiling.plane.GetZ(vl);
			double cre = sd.Ceiling.plane.GetZ(vr);

			double fl = flo > fle ? flo : fle;
			double fr = fro > fre ? fro : fre;
			double cl = clo < cle ? clo : cle;
			double cr = cro < cre ? cro : cre;
			
			// Anything to see?
			if(((cl - fl) > 0.01f) || ((cr - fr) > 0.01f))
			{
				// Keep top and bottom planes for intersection testing
				top = extrafloor.Floor.plane;
				bottom = extrafloor.Ceiling.plane;
				
				// Create initial polygon, which is just a quad between floor and ceiling
				WallPolygon poly = new WallPolygon();
				poly.Add(new Vector3D(vl.x, vl.y, fl));
				poly.Add(new Vector3D(vl.x, vl.y, cl));
				poly.Add(new Vector3D(vr.x, vr.y, cr));
				poly.Add(new Vector3D(vr.x, vr.y, fr));
				
				// Determine initial color. Inside parts are shaded using control sector's brightness
				int lightlevel;
				PixelColor levelcolor; //mxd
				if(((sourceside.Line.Args[2] & (int)Effect3DFloor.Flags.DisableLighting) != 0))
				{
					lightlevel = lightabsolute ? lightvalue : sd.Ceiling.brightnessbelow + lightvalue;
					levelcolor = sd.Ceiling.colorbelow; //mxd
				}
				else
				{
					lightlevel = lightabsolute ? lightvalue : sourceside.Sector.Brightness + lightvalue;
					levelcolor = extrafloor.Floor.colorbelow; //mxd
				}

				//mxd. This calculates light with doom-style wall shading
				PixelColor wallbrightness = PixelColor.FromInt(mode.CalculateBrightness(lightlevel, Sidedef));
				int wallcolor = PixelColor.Modulate(levelcolor, wallbrightness).WithAlpha((byte)extrafloor.Alpha).ToInt();
				fogfactor = CalculateFogFactor(lightlevel);

				// Cut off the part above the 3D floor and below the 3D ceiling
				CropPoly(ref poly, bottom, false);
				CropPoly(ref poly, top, false);

				// Cut out pieces that overlap 3D floors in this sector
				List<WallPolygon> polygons = new List<WallPolygon> { poly };
				foreach(Effect3DFloor ef in sd.ExtraFloors) 
				{
					//mxd. Our poly should be clipped when our ond other extrafloors are both solid or both translucent,
					// or when only our extrafloor is translucent
					if(ef.ClipSidedefs == extrafloor.ClipSidedefs || ef.ClipSidedefs)
					{
						//TODO: [this crashed on me once when performing auto-align on myriad of textures on BoA C1M0]
						if(ef.Floor == null || ef.Ceiling == null) ef.Update();
						
						int num = polygons.Count;
						for(int pi = 0; pi < num; pi++)
						{
							// Split by floor plane of 3D floor
							WallPolygon p = polygons[pi];
							WallPolygon np = SplitPoly(ref p, ef.Ceiling.plane, true);

							if(np.Count > 0)
							{
								// Split part below floor by the ceiling plane of 3D floor
								// and keep only the part below the ceiling (front)
								SplitPoly(ref np, ef.Floor.plane, true);

								if(p.Count == 0)
								{
									polygons[pi] = np;
								}
								else
								{
									polygons[pi] = p;
									polygons.Add(np);
								}
							}
							else
							{
								polygons[pi] = p;
							}
						}
					}
				}

				// Process the polygon and create vertices
				if(polygons.Count > 0)
				{
					List<WorldVertex> verts = CreatePolygonVertices(polygons, tp, sd, lightvalue, lightabsolute);
					if(verts.Count > 2)
					{
						if(extrafloor.Sloped3dFloor) this.RenderPass = RenderPass.Mask; //mxd
						else if(extrafloor.RenderAdditive) this.RenderPass = RenderPass.Additive; //mxd
						else if(extrafloor.Alpha < 255) this.RenderPass = RenderPass.Alpha;
						else this.RenderPass = RenderPass.Mask;

						//mxd. Inner sides always have extrafloor color
						for(int i = 0; i < verts.Count; i++)
						{
							WorldVertex v = verts[i];
							v.c = wallcolor;
							verts[i] = v;
						}

						base.SetVertices(verts);
						return true;
					}
				}
			}
			
			base.SetVertices(null); //mxd
			return false;
		}
		
		#endregion
	}
}
