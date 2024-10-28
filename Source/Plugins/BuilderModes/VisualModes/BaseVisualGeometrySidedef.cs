
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
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.GZBuilder.Data;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.VisualModes;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	internal abstract class BaseVisualGeometrySidedef : VisualGeometry, IVisualEventReceiver
	{
		#region ================== Constants
		
		private const float DRAG_ANGLE_TOLERANCE = 0.06f;
		
		#endregion

		#region ================== Variables

		protected readonly BaseVisualMode mode;

		protected Plane top;
		protected Plane bottom;
		protected long setuponloadedtexture;
        private long lastsetuponloadedtexture;

        // UV dragging
        private double dragstartanglexy;
		private double dragstartanglez;
		private Vector3D dragorigin;
		private Vector3D deltaxy;
		private Vector3D deltaz;
		private int startoffsetx;
		private int startoffsety;
		protected bool uvdragging;
		private int prevoffsetx;		// We have to provide delta offsets, but I don't
		private int prevoffsety;		// want to calculate with delta offsets to prevent
										// inaccuracy in the dragging.
		private static List<BaseVisualSector> updatelist; //mxd
		private bool performautoselection; //mxd

		// Undo/redo
		protected int undoticket;
		
		#endregion
		
		#region ================== Properties
		
		public bool IsDraggingUV { get { return uvdragging; } }
		new public BaseVisualSector Sector { get { return (BaseVisualSector)base.Sector; } }
		
		#endregion
		
		#region ================== Constructor / Destructor
		
		// Constructor for sidedefs
		protected BaseVisualGeometrySidedef(BaseVisualMode mode, VisualSector vs, Sidedef sd) : base(vs, sd)
		{
			this.mode = mode;
			this.deltaz = new Vector3D(0.0f, 0.0f, 1.0f);
			this.deltaxy = (sd.Line.End.Position - sd.Line.Start.Position) * sd.Line.LengthInv;
			if(!sd.IsFront) this.deltaxy = -this.deltaxy;
			this.performautoselection = (mode.UseSelectionFromClassicMode && sd.Line.Selected); //mxd
		}
		
		#endregion

		#region ================== Methods

		//mxd
		public override void PerformAutoSelection()
		{
			if(!performautoselection) return;
			if(Triangles > 0)
			{
				dragstartanglexy = General.Map.VisualCamera.AngleXY;
				dragstartanglez = General.Map.VisualCamera.AngleZ;
				dragorigin = pickintersect;

				Point texoffset = GetTextureOffset();
				startoffsetx = texoffset.X;
				startoffsety = texoffset.Y;
				prevoffsetx = texoffset.X;
				prevoffsety = texoffset.Y;

				this.selected = true;
				mode.AddSelectedObject(this);
			}

			performautoselection = false;
		}

		// This sets the renderstyle from linedef information and returns the alpha value or the vertices
		protected byte SetLinedefRenderstyle(bool solidasmask)
		{
			byte alpha;
			bool canhavealpha = (this is VisualMiddleDouble || this is VisualMiddle3D || this is VisualMiddleBack); //mxd

			// From TranslucentLine action
			if(Sidedef.Line.Action == 208)
			{
				alpha = (byte)General.Clamp(Sidedef.Line.Args[1], 0, 255);

				if(canhavealpha && Sidedef.Line.Args[2] == 1)
					this.RenderPass = RenderPass.Additive;
				else if(canhavealpha && (alpha < 255 || Texture.IsTranslucent))
					this.RenderPass = RenderPass.Alpha;
				else if(solidasmask)
					this.RenderPass = RenderPass.Mask;
				else
					this.RenderPass = RenderPass.Solid;
			}
			// From UDMF field
			else
			{
				alpha = (byte)(Sidedef.Line.Fields.GetValue("alpha", 1.0) * 255.0);
				if(alpha == 255 && Sidedef.Line.IsFlagSet("transparent")) alpha = 64; //mxd

				if(canhavealpha && Sidedef.Line.Fields.GetValue("renderstyle", "translucent") == "add")
					this.RenderPass = RenderPass.Additive;
				else if(canhavealpha && (alpha < 255 || Texture.IsTranslucent))
					this.RenderPass = RenderPass.Alpha;
				else if(solidasmask)
					this.RenderPass = RenderPass.Mask;
				else
					this.RenderPass = RenderPass.Solid;
			}
			
			return alpha;
		}

		// This performs a fast test in object picking
		public override bool PickFastReject(Vector3D from, Vector3D to, Vector3D dir)
		{
			// Check if intersection point is between top and bottom
			return (pickintersect.z >= bottom.GetZ(pickintersect)) && (pickintersect.z <= top.GetZ(pickintersect));
		}
		
		
		// This performs an accurate test for object picking
		public override bool PickAccurate(Vector3D from, Vector3D to, Vector3D dir, ref double u_ray)
		{
			// The fast reject pass is already as accurate as it gets,
			// so we just return the intersection distance here
			u_ray = pickrayu;
			return true;
		}
		
		
		// This creates vertices from a wall polygon and applies lighting
		/*protected List<WorldVertex> CreatePolygonVertices(WallPolygon poly, TexturePlane tp, SectorData sd, int lightvalue, bool lightabsolute)
		{
			List<WallPolygon> polylist = new List<WallPolygon>(1);
			polylist.Add(poly);
			return CreatePolygonVertices(polylist, tp, sd, lightvalue, lightabsolute);
		}*/

		// This creates vertices from a wall polygon and applies lighting
		protected List<WorldVertex> CreatePolygonVertices(List<WallPolygon> poly, TexturePlane tp, SectorData sd, int lightvalue, bool lightabsolute)
		{
			List<WallPolygon> polygons = new List<WallPolygon>(poly);
			List<WorldVertex> verts = new List<WorldVertex>();

			//mxd. Do complicated light level shenanigans only when there are extrafloors
			if(sd.LightLevels.Count > 2)
			{
				SectorLevel prevlight = null; //mxd

				// Go for all levels to build geometry
				for(int i = sd.LightLevels.Count - 1; i >= 0; i--)
				{
					SectorLevel l = sd.LightLevels[i];

					//mxd. Skip current light level when it's between TYPE1 and TYPE1_BOTTOM
					if(prevlight != null
					   && prevlight.type == SectorLevelType.Light && l.type == SectorLevelType.Light
					   && (prevlight.lighttype == LightLevelType.TYPE1 && l.lighttype != LightLevelType.TYPE1_BOTTOM)) continue;

					if((l != sd.Floor) && (l != sd.Ceiling) && (l.type != SectorLevelType.Floor || l.splitsides /*(l.alpha < 255)*/))
					{
						// Go for all polygons
						int num = polygons.Count;
						Plane plane = (l.type == SectorLevelType.Floor ? l.plane.GetInverted() : l.plane); //mxd

						for(int pi = 0; pi < num; pi++)
						{
							// Split by plane
							WallPolygon p = polygons[pi];
							WallPolygon np = SplitPoly(ref p, plane, false);
							if(np.Count > 0)
							{
								if(l.type == SectorLevelType.Glow)
								{
									//mxd. Glow levels should not affect light level
									np.color = p.color;
								}
								else
								{
									//mxd. Determine color
									int lightlevel;

									// Sidedef part is not affected by 3d floor brightness
									if(l.type != SectorLevelType.Light && (l.disablelighting || !l.extrafloor))
										lightlevel = (lightabsolute ? lightvalue : l.brightnessbelow + lightvalue);
									// 3d floors and light transfer effects transfers brightness below them ignoring sidedef's brightness
									else
										lightlevel = l.brightnessbelow;

									PixelColor wallbrightness = PixelColor.FromInt(mode.CalculateBrightness(lightlevel, Sidedef)); //mxd
									np.color = PixelColor.Modulate(l.colorbelow, wallbrightness).WithAlpha(255).ToInt();
								}

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

					//mxd
					if(l.type == SectorLevelType.Light) prevlight = l;
				}
			}

			// Go for all polygons to make geometry
			foreach(WallPolygon p in polygons)
			{
				// Find texture coordinates for each vertex in the polygon
				List<Vector2D> texc = new List<Vector2D>(p.Count);
				foreach(Vector3D v in p) texc.Add(tp.GetTextureCoordsAt(v));
				
				// Now we create triangles from the polygon.
				// The polygon is convex and clockwise, so this is a piece of cake.
				if(p.Count > 2)
				{
					for(int k = 1; k < (p.Count - 1); k++)
					{
						verts.Add(new WorldVertex(p[0], p.color, texc[0]));
						verts.Add(new WorldVertex(p[k], p.color, texc[k]));
						verts.Add(new WorldVertex(p[k + 1], p.color, texc[k + 1]));
					}
				}
			}

			//mxd. Interpolate vertex colors?
			for(int i = 0; i < verts.Count; i++)
			{
				//if(verts[i].c == PixelColor.INT_WHITE) continue; // Fullbright verts are not affected by glows.
				verts[i] = InterpolateVertexColor(verts[i], sd);
			}
			
			return verts;
		}

		//mxd
		private static WorldVertex InterpolateVertexColor(WorldVertex v, SectorData data) 
		{
            // don't process glows if fullbright.
            //if (v.c == PixelColor.INT_WHITE)
            //    return v;

			// Apply ceiling glow?
			if(data.CeilingGlow != null)
			{
				double cgz = data.CeilingGlowPlane.GetZ(v.x, v.y);

				// Vertex is above ceiling glow plane?
				if(v.z > cgz)
				{
					double cz = data.Ceiling.plane.GetZ(v.x, v.y);
					double delta = 1.0f - (((v.z - cgz) / (cz - cgz)) * 0.9f);
					PixelColor vertexcolor = PixelColor.FromInt(v.c);
					PixelColor glowcolor = PixelColor.Add(vertexcolor, data.CeilingGlow.Color);
					v.c = InterpolationTools.InterpolateColor(glowcolor, vertexcolor, delta).WithAlpha(255).ToInt();
				}
			}

			// Apply floor glow?
			if(data.FloorGlow != null)
			{
				double fgz = data.FloorGlowPlane.GetZ(v.x, v.y);

				// Vertex is below floor glow plane?
				if(v.z < fgz)
				{
					double fz = data.Floor.plane.GetZ(v.x, v.y);
					double delta = 1.0f - (((v.z - fz) / (fgz - fz)) * 0.9f);
					PixelColor vertexcolor = PixelColor.FromInt(v.c);
					PixelColor glowcolor = PixelColor.Add(vertexcolor, data.FloorGlow.Color);
					v.c = InterpolationTools.InterpolateColor(vertexcolor, glowcolor, delta).WithAlpha(255).ToInt();
				}
			}

            // [ZZ] process sector top and bottom colors.
            // block
            {
				double cz = data.Ceiling.plane.GetZ(v.x, v.y);
				double cgz = data.Floor.plane.GetZ(v.x, v.y);
				double delta = 1.0f - (((v.z - cgz) / (cz - cgz)) * 0.9f);
                PixelColor vertexcolor = PixelColor.FromInt(v.c);
                PixelColor topcolor = PixelColor.Modulate(vertexcolor, data.ColorWallTop);
                PixelColor bottomcolor = PixelColor.Modulate(vertexcolor, data.ColorWallBottom);
                v.c = InterpolationTools.InterpolateColor(topcolor, bottomcolor, delta).WithAlpha(255).ToInt();
            }

            return v;
		}
		
		// This splits a polygon with a plane and returns the other part as a new polygon
		// The polygon is expected to be convex and clockwise
		protected static WallPolygon SplitPoly(ref WallPolygon poly, Plane p, bool keepfront)
		{
			const double NEAR_ZERO = 0.01f;
			WallPolygon front = new WallPolygon(poly.Count);
			WallPolygon back = new WallPolygon(poly.Count);
			poly.CopyProperties(front);
			poly.CopyProperties(back);
			
			if(poly.Count > 0)
			{
				// Go for all vertices to see which side they have to be on
				Vector3D v1 = poly[poly.Count - 1];
				double side1 = p.Distance(v1);
				for(int i = 0; i < poly.Count; i++)
				{
					// Fetch vertex and determine side
					Vector3D v2 = poly[i];
					double side2 = p.Distance(v2);
					
					// Front?
					if(side2 > NEAR_ZERO)
					{
						if(side1 < -NEAR_ZERO)
						{
							// Split line with plane and insert the vertex
							double u = 0.0f;
							p.GetIntersection(v1, v2, ref u);
							Vector3D v3 = v1 + (v2 - v1) * u;
							front.Add(v3);
							back.Add(v3);
						}
						
						front.Add(v2);
					}
					// Back?
					else if(side2 < -NEAR_ZERO)
					{
						if(side1 > NEAR_ZERO)
						{
							// Split line with plane and insert the vertex
							double u = 0.0f;
							p.GetIntersection(v1, v2, ref u);
							Vector3D v3 = v1 + (v2 - v1) * u;
							front.Add(v3);
							back.Add(v3);
						}
						
						back.Add(v2);
					}
					else
					{
						// On the plane, add to both polygons
						front.Add(v2);
						back.Add(v2);
					}
					
					// Next
					v1 = v2;
					side1 = side2;
				}
			}
			
			if(keepfront)
			{
				poly = front;
				return back;
			}
			else
			{
				poly = back;
				return front;
			}
		}
		
		
		// This crops a polygon with a plane and keeps only a certain part of the polygon
		protected static void CropPoly(ref WallPolygon poly, Plane p, bool keepfront)
		{
			const double NEAR_ZERO = 0.01f;
			double sideswitch = keepfront ? 1 : -1;
			WallPolygon newp = new WallPolygon(poly.Count);
			poly.CopyProperties(newp);
			
			if(poly.Count > 0)
			{
				// First split lines that cross the plane so that we have vertices on the plane where the lines cross
				Vector3D v1 = poly[poly.Count - 1];
				double side1 = p.Distance(v1) * sideswitch;
				for(int i = 0; i < poly.Count; i++)
				{
					// Fetch vertex and determine side
					Vector3D v2 = poly[i];
					double side2 = p.Distance(v2) * sideswitch;
					
					// Front?
					if(side2 > NEAR_ZERO)
					{
						if(side1 < -NEAR_ZERO)
						{
							// Split line with plane and insert the vertex
							double u = 0.0f;
							p.GetIntersection(v1, v2, ref u);
							Vector3D v3 = v1 + (v2 - v1) * u;
							newp.Add(v3);
						}
						
						newp.Add(v2);
					}
					// Back?
					else if(side2 < -NEAR_ZERO)
					{
						if(side1 > NEAR_ZERO)
						{
							// Split line with plane and insert the vertex
							double u = 0.0f;
							p.GetIntersection(v1, v2, ref u);
							Vector3D v3 = v1 + (v2 - v1) * u;
							newp.Add(v3);
						}
					}
					else
					{
						// On the plane
						newp.Add(v2);
					}
					
					// Next
					v1 = v2;
					side1 = side2;
				}
			}
			
			poly = newp;
		}

		//mxd. This clips given polys by extrafloors
		protected void ClipExtraFloors(List<WallPolygon> polygons, List<Effect3DFloor> extrafloors, bool clipalways)
		{
			foreach(Effect3DFloor ef in extrafloors)
			{
				//mxd. Walls should be clipped by 3D floors
				if(ef.ClipSidedefs || clipalways)
				{
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
		}

		//mxd
		protected void GetLightValue(out int lightvalue, out bool lightabsolute)
		{
			string partstr = string.Empty, partstrabs = string.Empty;

			if (General.Map.Config.DistinctSidedefPartBrightness)
			{
				switch (geometrytype)
				{
					case VisualGeometryType.WALL_UPPER:
						partstr = "light_top";
						partstrabs = "lightabsolute_top";
						break;
					case VisualGeometryType.WALL_LOWER:
						partstr = "light_bottom";
						partstrabs = "lightabsolute_bottom";
						break;
					case VisualGeometryType.WALL_MIDDLE:
					case VisualGeometryType.WALL_MIDDLE_3D:
						partstr = "light_mid";
						partstrabs = "lightabsolute_mid";
						break;
				}
			}

			bool lightglobalabsolute = Sidedef.Fields.GetValue("lightabsolute", false);
			bool lightpartabsolute = General.Map.Config.DistinctSidedefPartBrightness ? Sidedef.Fields.GetValue(partstrabs, false) : false;
			lightabsolute = lightglobalabsolute || lightpartabsolute;
			bool affectedbyfog = General.Map.Data.MapInfo.HasFadeColor || (Sector.Sector.HasSkyCeiling && General.Map.Data.MapInfo.HasOutsideFogColor) || Sector.Sector.Fields.ContainsKey("fadecolor");
			bool ignorelight = affectedbyfog && !Sidedef.IsFlagSet("lightfog") && !lightabsolute;
			lightvalue = ignorelight ? 0 : Sidedef.Fields.GetValue("light", 0); //mxd

			if(ignorelight)
			{
				lightvalue = 0;
			}
			else
			{
				// Absolute value of upper/middle/lower always has precedence
				if(lightpartabsolute)
					lightvalue = Sidedef.Fields.GetValue(partstr, 0);
				else
					lightvalue = Sidedef.Fields.GetValue("light", 0) + (General.Map.Config.DistinctSidedefPartBrightness ? Sidedef.Fields.GetValue(partstr, 0) : 0);
			}

			if(ignorelight) lightabsolute = false;
		}

		// biwa
		protected static double GetNewTexutreOffset(double oldValue, double offset, double textureSize)
		{
			return GetRoundedTextureOffset(oldValue, offset, 1.0, textureSize);
		}

		//mxd
		protected static double GetRoundedTextureOffset(double oldValue, double offset, double scale, double textureSize) 
		{
			if(offset == 0 || offset % textureSize == 0) return oldValue;
			double scaledOffset = offset * Math.Abs(scale);
			double result = Math.Round(oldValue + scaledOffset);
			if(textureSize > 0) result %= textureSize;
			if(result == oldValue) result += (scaledOffset < 0 ? -1 : 1); // biwa. Why?
			return result;
		}

		//mxd
        public void SelectNeighbours(bool select, bool matchtexture, bool matchheight, bool stopatselected)
        {
            SelectNeighbours(select, matchtexture, matchheight, true, true, stopatselected);
        }

		private void SelectNeighbours(bool select, bool matchtexture, bool matchheight, bool clearlinedefs, bool forward, bool stopatselected)
		{
			if(Sidedef.Sector == null || Triangles < 1 || (!matchtexture && !matchheight)) return;

			Rectangle rect = BuilderModesTools.GetSidedefPartSize(this);
			if(rect.Height == 0) return;

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

			// [ZZ] use the marking system.
			if (clearlinedefs)
			{
				General.Map.Map.ClearMarkedLinedefs(false);
				General.Map.Map.ClearMarkedSidedefs(false);
			}

            Sidedef.Line.Marked = true;

            // Select
            Vertex v;

            if (forward)
            {
                v = Sidedef.IsFront ? Sidedef.Line.End : Sidedef.Line.Start;
                SelectNeighbourLines(v.Linedefs, v, rect, select, matchtexture, matchheight, true, stopatselected);
                v = Sidedef.IsFront ? Sidedef.Line.Start : Sidedef.Line.End;
                SelectNeighbourLines(v.Linedefs, v, rect, select, matchtexture, matchheight, false, stopatselected);
            }
            else
            {
                v = Sidedef.IsFront ? Sidedef.Line.Start : Sidedef.Line.End;
                SelectNeighbourLines(v.Linedefs, v, rect, select, matchtexture, matchheight, false, stopatselected);
                v = Sidedef.IsFront ? Sidedef.Line.End : Sidedef.Line.Start;
                SelectNeighbourLines(v.Linedefs, v, rect, select, matchtexture, matchheight, true, stopatselected);
            }
		}

		//mxd
		private void SelectNeighbourLines(IEnumerable<Linedef> lines, Vertex v, Rectangle sourcerect, bool select, bool matchtexture, bool matchheight, bool forward, bool stopatselected)
		{
			foreach(Linedef line in lines)
			{
                // [ZZ] decide which side of the next linedef to iterate.
                //      do NOT do both
                Sidedef next = null;
                Sidedef side1 = forward ? line.Front : line.Back;
                Sidedef side2 = forward ? line.Back : line.Front;

                if (line.Start == v)
                    next = side1;
                else if (line.End == v)
                    next = side2;

                if (next == null || next.Sector == null)
                    continue;

                SelectNeighbourSideParts(next, sourcerect, select, matchtexture, matchheight, forward, stopatselected);
			}
		}

		//mxd
		private void SelectNeighbourSideParts(Sidedef side, Rectangle sourcerect, bool select, bool matchtexture, bool matchheight, bool forward, bool stopatselected)
		{
            if (side.Line.Marked)
                return;

			BaseVisualSector s = (BaseVisualSector)mode.GetVisualSector(side.Sector);
			if(s != null)
			{
				VisualSidedefParts parts = s.GetSidedefParts(side);
				SelectNeighbourSidePart(parts.lower, sourcerect, select, matchtexture, matchheight, forward, stopatselected);
				SelectNeighbourSidePart(parts.middlesingle, sourcerect, select, matchtexture, matchheight, forward, stopatselected);
				SelectNeighbourSidePart(parts.middledouble, sourcerect, select, matchtexture, matchheight, forward, stopatselected);
				SelectNeighbourSidePart(parts.upper, sourcerect, select, matchtexture, matchheight, forward, stopatselected);

				if(parts.middle3d != null)
				{
					foreach(VisualMiddle3D middle3D in parts.middle3d)
						SelectNeighbourSidePart(middle3D, sourcerect, select, matchtexture, matchheight, forward, stopatselected);
				}
			}
		}

		//mxd
		private void SelectNeighbourSidePart(BaseVisualGeometrySidedef visualside, Rectangle sourcerect, bool select, bool matchtexture, bool matchheight, bool forward, bool stopatselected)
		{
			if (visualside != null && visualside.Triangles > 0 && !visualside.Sidedef.Marked && (!stopatselected || (visualside.Selected != select)))
			{
				Rectangle r = BuilderModesTools.GetSidedefPartSize(visualside);
				if(r.Width == 0 || r.Height == 0) return;
				if((!matchtexture || (visualside.Texture == Texture && r.IntersectsWith(sourcerect))) &&
				   (!matchheight || (sourcerect.Height == r.Height && sourcerect.Y == r.Y)))
				{
					visualside.SelectNeighbours(select, matchtexture, matchheight, false, forward, stopatselected);
				}
			}
		}

		//mxd
		protected void FitTexture(FitTextureOptions options)
		{
			// Create undo name
			string s;
			if(options.FitWidth && options.FitHeight) s = "width and height";
			else if(options.FitWidth) s = "width";
			else s = "height";

			// Create undo
			mode.CreateUndo("Fit texture (" + s + ")", UndoGroup.TextureOffsetChange, Sector.Sector.FixedIndex);
			Sidedef.Fields.BeforeFieldsChange();
			
			// Get proper control side...
			Linedef controlline = GetControlLinedef();
			Sidedef controlside;
			if(controlline != Sidedef.Line)
			{
				controlside = controlline.Front;
				controlside.Fields.BeforeFieldsChange();
			}
			else
			{
				controlside = Sidedef;
			}

			// Fit width
			if(options.FitWidth) 
			{
				double scalex, offsetx;
				double linelength = Math.Round(Sidedef.Line.Length); // Let's use ZDoom-compatible line length here
				double patternwidth = (options.AutoWidth && options.PatternWidth > 0) ? options.PatternWidth : Texture.Width;
				double horizontalrepeat = options.HorizontalRepeat;

				if (options.FitAcrossSurfaces)
				{
					if (options.AutoWidth)
					{
						horizontalrepeat = Math.Round((double)options.GlobalBounds.Width / patternwidth);

						if (horizontalrepeat == 0)
							horizontalrepeat = 1.0f;

						if (options.PatternWidth > 0)
							horizontalrepeat /= Texture.Width / patternwidth;
					}

					scalex = Texture.ScaledWidth / (linelength * (options.GlobalBounds.Width / linelength)) * horizontalrepeat;
					offsetx = Math.Round((options.Bounds.X * scalex - Sidedef.OffsetX - options.ControlSideOffsetX), General.Map.FormatInterface.VertexDecimals);
					if(Texture.IsImageLoaded) offsetx %= Texture.Width;
				} 
				else 
				{
					if (options.AutoWidth)
					{
						horizontalrepeat = Math.Round(linelength / patternwidth);

						if (horizontalrepeat == 0)
							horizontalrepeat = 1.0f;

						if (options.PatternWidth > 0)
							horizontalrepeat /= Texture.Width / patternwidth;
					}


					scalex = Texture.ScaledWidth / linelength * horizontalrepeat;
					offsetx = -Sidedef.OffsetX - options.ControlSideOffsetX;
				}

				UniFields.SetFloat(controlside.Fields, "scalex_" + partname, Math.Round(scalex, General.Map.FormatInterface.VertexDecimals), 1.0);
				UniFields.SetFloat(Sidedef.Fields, "offsetx_" + partname, offsetx, 0.0);
			} 
			else 
			{
				// Restore initial offsets
				UniFields.SetFloat(controlside.Fields, "scalex_" + partname, options.InitialScaleX, 1.0);
				UniFields.SetFloat(Sidedef.Fields, "offsetx_" + partname, options.InitialOffsetX, 0.0);
			}

			// Fit height
			if(options.FitHeight) 
			{
				if(Sidedef.Sector != null) 
				{
					double scaley, offsety;
					double patternheight = (options.AutoHeight && options.PatternHeight > 0) ? options.PatternHeight : Texture.Height;
					double verticalrepeat = options.VerticalRepeat;

					if (options.FitAcrossSurfaces) 
					{
						if (options.AutoHeight)
						{
							verticalrepeat = Math.Round((float)options.GlobalBounds.Height / patternheight);

							if (verticalrepeat == 0)
								verticalrepeat = 1.0f;

							if (options.PatternHeight > 0)
								verticalrepeat /= Texture.Height / patternheight;
						}

						scaley = Texture.ScaledHeight / (options.Bounds.Height * ((double)options.GlobalBounds.Height / options.Bounds.Height)) * verticalrepeat;

						if(this is VisualLower) // Special cases, special cases...
						{ 
							offsety = GetLowerOffsetY(scaley);
						}
						else if(this is VisualMiddleDouble)
						{
							if(Sidedef.Line.IsFlagSet(General.Map.Config.LowerUnpeggedFlag))
								offsety = (options.Bounds.Y - Sidedef.GetHighHeight() - Sidedef.GetLowHeight()) * scaley - Sidedef.OffsetY;
							else
								offsety = options.Bounds.Y * scaley - Sidedef.OffsetY;
						}
						else
						{
							offsety = Tools.GetSidedefOffsetY(Sidedef, geometrytype, options.Bounds.Y * scaley - Sidedef.OffsetY - options.ControlSideOffsetY, scaley, true);
							if(Texture.IsImageLoaded) offsety %= Texture.Height;
						}
					} 
					else 
					{
						if (options.AutoHeight)
						{
							verticalrepeat = Math.Round((double)options.Bounds.Height / patternheight);

							if (verticalrepeat == 0)
								verticalrepeat = 1.0;

							if (options.PatternHeight > 0)
								verticalrepeat /= Texture.Height / patternheight;
						}

						scaley = Texture.ScaledHeight / options.Bounds.Height * verticalrepeat;

						// Special cases, special cases...
						if(this is VisualLower)
						{
							offsety = GetLowerOffsetY(scaley);
						} 
						else
						{
							offsety = Tools.GetSidedefOffsetY(Sidedef, geometrytype, -Sidedef.OffsetY - options.ControlSideOffsetY, scaley, true);
							if(Texture.IsImageLoaded) offsety %= Texture.Height;
						}
					}

					UniFields.SetFloat(controlside.Fields, "scaley_" + partname, (float)Math.Round(scaley, General.Map.FormatInterface.VertexDecimals), 1.0);
					UniFields.SetFloat(Sidedef.Fields, "offsety_" + partname, (float)Math.Round(offsety, General.Map.FormatInterface.VertexDecimals), 0.0);
				}
			} 
			else 
			{
				// Restore initial offsets
				UniFields.SetFloat(controlside.Fields, "scaley_" + partname, options.InitialScaleY, 1.0);
				UniFields.SetFloat(Sidedef.Fields, "offsety_" + partname, options.InitialOffsetY, 0.0);
			}
		}

		//mxd. Oh so special cases...
		private double GetLowerOffsetY(double scaley) 
		{
			double offsety;
			if(Sidedef.Line.IsFlagSet(General.Map.Config.LowerUnpeggedFlag))
				offsety = (-Sidedef.OffsetY - Sidedef.GetMiddleHeight() - Sidedef.GetHighHeight()) * scaley;
			else
				offsety = -Sidedef.OffsetY * scaley;

			if(Texture.IsImageLoaded) offsety %= Texture.Height;
			return offsety;
		}
		
		#endregion

		#region ================== Events
		
		// Unused
		public virtual void OnEditBegin() { }
		protected virtual void SetTexture(string texturename) { }
		public abstract bool Setup();
		protected abstract void SetTextureOffsetX(int x);
		protected abstract void SetTextureOffsetY(int y);
		protected virtual void ResetTextureScale() { } //mxd
		protected abstract void MoveTextureOffset(int offsetx, int offsety);
		protected abstract Point GetTextureOffset();
		public virtual void OnTextureFit(FitTextureOptions options) { } //mxd
		public virtual void OnPaintSelectEnd() { } // biwa

		// Insert middle texture
		public virtual void OnInsert()
		{
			// No middle texture yet?
			if(!Sidedef.MiddleRequired() && (string.IsNullOrEmpty(Sidedef.MiddleTexture) || (Sidedef.MiddleTexture == "-")))
			{
				// Make it now
				mode.CreateUndo("Create middle texture");
				mode.SetActionResult("Created middle texture.");
				General.Settings.FindDefaultDrawSettings();
				Sidedef.SetTextureMid(General.Map.Options.DefaultWallTexture);

				// Update
				Sector.Changed = true;
				
				// Other side as well
				if(string.IsNullOrEmpty(Sidedef.Other.MiddleTexture) || (Sidedef.Other.MiddleTexture == "-"))
				{
					Sidedef.Other.SetTextureMid(General.Map.Options.DefaultWallTexture);

					// Update
					VisualSector othersector = mode.GetVisualSector(Sidedef.Other.Sector);
					if(othersector is BaseVisualSector) ((BaseVisualSector)othersector).Changed = true;
				}
			}
		}

		// Delete texture
		public virtual void OnDelete()
		{
			// Remove texture
			mode.CreateUndo("Delete texture");
			mode.SetActionResult("Deleted a texture.");
			SetTexture("-");

			//mxd. Update linked effects
			SectorData sd = mode.GetSectorDataEx(Sector.Sector);
			if(sd != null) sd.Reset(true);
		}
		
		// Processing
		public virtual void OnProcess(long deltatime)
		{
            // If the texture was not loaded, but is loaded now, then re-setup geometry
            if (setuponloadedtexture != lastsetuponloadedtexture)
            {
                if (setuponloadedtexture != 0)
                {
                    ImageData t = General.Map.Data.GetTextureImage(setuponloadedtexture);
                    if (t != null && t.IsImageLoaded)
                    {
                        lastsetuponloadedtexture = setuponloadedtexture;
                        Setup();
                    }
                }
                else
                {
                    lastsetuponloadedtexture = setuponloadedtexture;
                }
            }
        }
		
		// Change target height
		public virtual void OnChangeTargetHeight(int amount)
		{
			switch(BuilderPlug.Me.ChangeHeightBySidedef)
			{
				// Change ceiling
				case 1:
					if(!this.Sector.Ceiling.Changed) this.Sector.Ceiling.OnChangeTargetHeight(amount);
					break;

				// Change floor
				case 2:
					if(!this.Sector.Floor.Changed) this.Sector.Floor.OnChangeTargetHeight(amount);
					break;

				// Change both
				case 3:
					if(!this.Sector.Floor.Changed) this.Sector.Floor.OnChangeTargetHeight(amount);
					if(!this.Sector.Ceiling.Changed) this.Sector.Ceiling.OnChangeTargetHeight(amount);
					break;
			}
		}
		
		// Reset texture offsets
		public virtual void OnResetTextureOffset()
		{
			mode.CreateUndo("Reset texture offsets");
			mode.SetActionResult("Texture offsets reset.");

			// Apply offsets
			Sidedef.OffsetX = 0;
			Sidedef.OffsetY = 0;
			
			// Update sidedef geometry
			VisualSidedefParts parts = Sector.GetSidedefParts(Sidedef);
			parts.SetupAllParts();
		}

		//mxd
		public virtual void OnResetLocalTextureOffset() 
		{
			if(!General.Map.UDMF) 
			{
				OnResetTextureOffset();
				return;
			}

			mode.CreateUndo("Reset local texture offsets, scale and brightness");
			mode.SetActionResult("Local texture offsets, scale and brightness reset.");

			// Reset texture offsets
			SetTextureOffsetX(0);
			SetTextureOffsetY(0);

			// Scale
			ResetTextureScale();

			// And brightness
			bool setupallparts = false;
			if(Sidedef.Fields.ContainsKey("light"))
			{
				Sidedef.Fields.Remove("light");
				setupallparts = true;
			}
			if(Sidedef.Fields.ContainsKey("lightabsolute"))
			{
				Sidedef.Fields.Remove("lightabsolute");
				setupallparts = true;
			}

			if(setupallparts)
			{
				// Update all sidedef geometry
				VisualSidedefParts parts = Sector.GetSidedefParts(Sidedef);
				parts.SetupAllParts();
			}
			else
			{
				// Update this part only
				this.Setup();
			}
		}
		
		// Toggle upper-unpegged
		public virtual void OnToggleUpperUnpegged()
		{
			if(this.Sidedef.Line.IsFlagSet(General.Map.Config.UpperUnpeggedFlag))
			{
				// Remove flag
				mode.ApplyUpperUnpegged(false);
			}
			else
			{
				// Add flag
				mode.ApplyUpperUnpegged(true);
			}
		}

		// Toggle lower-unpegged
		public virtual void OnToggleLowerUnpegged()
		{
			if(this.Sidedef.Line.IsFlagSet(General.Map.Config.LowerUnpeggedFlag))
			{
				// Remove flag
				mode.ApplyLowerUnpegged(false);
			}
			else
			{
				// Add flag
				mode.ApplyLowerUnpegged(true);
			}
		}

		
		// This sets the Upper Unpegged flag
		public virtual void ApplyUpperUnpegged(bool set)
		{
			if(!set)
			{
				// Remove flag
				mode.CreateUndo("Remove upper-unpegged setting");
				mode.SetActionResult("Removed upper-unpegged setting.");
				this.Sidedef.Line.SetFlag(General.Map.Config.UpperUnpeggedFlag, false);
			}
			else
			{
				// Add flag
				mode.CreateUndo("Set upper-unpegged setting");
				mode.SetActionResult("Set upper-unpegged setting.");
				this.Sidedef.Line.SetFlag(General.Map.Config.UpperUnpeggedFlag, true);
			}

			// Update sidedef geometry
			VisualSidedefParts parts = Sector.GetSidedefParts(Sidedef);
			parts.SetupAllParts();
			
			// Update other sidedef geometry
			if(Sidedef.Other != null)
			{
				BaseVisualSector othersector = (BaseVisualSector)mode.GetVisualSector(Sidedef.Other.Sector);
				parts = othersector.GetSidedefParts(Sidedef.Other);
				parts.SetupAllParts();
			}
		}
		
		
		// This sets the Lower Unpegged flag
		public virtual void ApplyLowerUnpegged(bool set)
		{
			if(!set)
			{
				// Remove flag
				mode.CreateUndo("Remove lower-unpegged setting");
				mode.SetActionResult("Removed lower-unpegged setting.");
				this.Sidedef.Line.SetFlag(General.Map.Config.LowerUnpeggedFlag, false);
			}
			else
			{
				// Add flag
				mode.CreateUndo("Set lower-unpegged setting");
				mode.SetActionResult("Set lower-unpegged setting.");
				this.Sidedef.Line.SetFlag(General.Map.Config.LowerUnpeggedFlag, true);
			}

			// Update sidedef geometry
			VisualSidedefParts parts = Sector.GetSidedefParts(Sidedef);
			parts.SetupAllParts();

			// Update other sidedef geometry
			if(Sidedef.Other != null)
			{
				BaseVisualSector othersector = (BaseVisualSector)mode.GetVisualSector(Sidedef.Other.Sector);
				parts = othersector.GetSidedefParts(Sidedef.Other);
				parts.SetupAllParts();
			}
		}


		// Flood-fill textures
		public virtual void OnTextureFloodfill()
		{
			if(BuilderPlug.Me.CopiedTexture != null)
			{
				string oldtexture = GetTextureName();
				string newtexture = BuilderPlug.Me.CopiedTexture;
				if(newtexture != oldtexture)
				{
					mode.CreateUndo("Flood-fill textures with " + newtexture);
					mode.SetActionResult("Flood-filled textures with " + newtexture + ".");
					
					mode.Renderer.SetCrosshairBusy(true);
					General.Interface.RedrawDisplay();

					// Get the texture
					ImageData newtextureimage = General.Map.Data.GetTextureImage(newtexture);
					if(newtextureimage != null)
					{
						if(mode.IsSingleSelection)
						{
							// Clear all marks, this will align everything it can
							General.Map.Map.ClearMarkedSidedefs(false);
						}
						else
						{
							// Limit the alignment to selection only
							General.Map.Map.ClearMarkedSidedefs(true);
							List<Sidedef> sides = mode.GetSelectedSidedefs();
                            foreach (Sidedef sd in sides)
                            {
                                sd.Marked = false;
                                if (sd.Other != null)
                                    sd.Other.Marked = false;
                            }
						}

						//mxd. We potentially need to deal with 2 textures (because of long and short texture names)...
						HashSet<long> texturehashes = new HashSet<long> { Texture.LongName };
						switch(this.GeometryType)
						{
							case VisualGeometryType.WALL_LOWER:
								texturehashes.Add(Sidedef.LongLowTexture);
								break;

							case VisualGeometryType.WALL_MIDDLE:
							case VisualGeometryType.WALL_MIDDLE_3D:
								texturehashes.Add(Sidedef.LongMiddleTexture);
								break;

							case VisualGeometryType.WALL_UPPER:
								texturehashes.Add(Sidedef.LongHighTexture);
								break;
						}
						
						// Do the alignment
						BuilderModesTools.FloodfillTextures(mode, Sidedef, texturehashes, newtexture, false);

						// Get the changed sidedefs
						List<Sidedef> changes = General.Map.Map.GetMarkedSidedefs(true);
						foreach(Sidedef sd in changes)
						{
							// Update the parts for this sidedef!
							if(mode.VisualSectorExists(sd.Sector))
							{
								BaseVisualSector vs = (BaseVisualSector)mode.GetVisualSector(sd.Sector);
								VisualSidedefParts parts = vs.GetSidedefParts(sd);
								parts.SetupAllParts();
							}
						}

						General.Map.Data.UpdateUsedTextures();
						mode.Renderer.SetCrosshairBusy(false);
						mode.ShowTargetInfo();
					}
				}
			}
		}
		
		// Auto-align texture offsets
		public virtual void OnTextureAlign(bool alignx, bool aligny)
		{
			//mxd
			string rest;
			if(alignx && aligny) rest = "(X and Y)";
			else if(alignx)	rest = "(X)";
			else rest = "(Y)";

			mode.CreateUndo("Auto-align textures " + rest);
			mode.SetActionResult("Auto-aligned textures " + rest + ".");
			
			// Make sure the texture is loaded (we need the texture size)
			if(!base.Texture.IsImageLoaded) base.Texture.LoadImageNow();
			
			if(mode.IsSingleSelection)
			{
				// Clear all marks, this will align everything it can
				General.Map.Map.ClearMarkedSidedefs(false);
			}
			else
			{
				// Limit the alignment to selection only
				General.Map.Map.ClearMarkedSidedefs(true);
				List<Sidedef> sides = mode.GetSelectedSidedefs();
                foreach (Sidedef sd in sides)
                {
                    sd.Marked = false;
                    if (sd.Other != null)
                        sd.Other.Marked = false;
                }
			}
			
			// Do the alignment
			mode.AutoAlignTextures(this, base.Texture, alignx, aligny, false, true);

			// Get the changed sidedefs
			List<Sidedef> changes = General.Map.Map.GetMarkedSidedefs(true);
			foreach(Sidedef sd in changes)
			{
				// Update the parts for this sidedef!
				if(mode.VisualSectorExists(sd.Sector))
				{
					BaseVisualSector vs = (BaseVisualSector)mode.GetVisualSector(sd.Sector);
					VisualSidedefParts parts = vs.GetSidedefParts(sd);
					parts.SetupAllParts();
				}
			}
		}
		
		// Select texture
		public virtual void OnSelectTexture()
		{
			if(General.Interface.IsActiveWindow)
			{
				string oldtexture = GetTextureName();
				string newtexture = General.Interface.BrowseTexture(General.Interface, oldtexture);
				if(newtexture != oldtexture)
				{
					mode.ApplySelectTexture(newtexture, false);
				}
			}
		}

		// Apply Texture
		public virtual void ApplyTexture(string texture)
		{
			mode.CreateUndo("Change texture " + texture);
			SetTexture(texture);
			
			//mxd. Update linked effects
			SectorData sd = mode.GetSectorDataEx(Sector.Sector);
			if(sd != null) sd.Reset(true);
		}
		
		// Paste texture
		public virtual void OnPasteTexture()
		{
			if(BuilderPlug.Me.CopiedTexture != null)
			{
				mode.CreateUndo("Paste texture \"" + BuilderPlug.Me.CopiedTexture + "\"");
				mode.SetActionResult("Pasted texture \"" + BuilderPlug.Me.CopiedTexture + "\".");
				SetTexture(BuilderPlug.Me.CopiedTexture);

				//mxd. Update linked effects
				SectorData sd = mode.GetSectorDataEx(Sector.Sector);
				if(sd != null) sd.Reset(true);
			}
		}
		
		// Paste texture offsets
		public virtual void OnPasteTextureOffsets()
		{
			mode.CreateUndo("Paste texture offsets");
			if(General.Map.UDMF && General.Map.Config.UseLocalSidedefTextureOffsets) 
			{
				SetTextureOffsetX(BuilderPlug.Me.CopiedOffsets.X);
				SetTextureOffsetY(BuilderPlug.Me.CopiedOffsets.Y);

				// Update sidedef part geometry
				this.Setup();
			} 
			else 
			{
				Sidedef.OffsetX = BuilderPlug.Me.CopiedOffsets.X;
				Sidedef.OffsetY = BuilderPlug.Me.CopiedOffsets.Y;

				// Update sidedef geometry
				VisualSidedefParts parts = Sector.GetSidedefParts(Sidedef);
				parts.SetupAllParts();
			}

			//mxd. Update linked effects
			SectorData sd = mode.GetSectorDataEx(Sector.Sector);
			if(sd != null) sd.Reset(true);

			mode.SetActionResult("Pasted texture offsets " + BuilderPlug.Me.CopiedOffsets.X + ", " + BuilderPlug.Me.CopiedOffsets.Y + ".");
		}
		
		// Copy texture
		public virtual void OnCopyTexture()
		{
			//mxd. When UseLongTextureNames is enabled and the image filename is longer than 8 chars, use full name, 
			// otherwise use texture name as stored in Sidedef
			string texturename = ((General.Map.Options.UseLongTextureNames && Texture != null && Texture.UsedInMap
				&& Path.GetFileNameWithoutExtension(Texture.Name).Length > DataManager.CLASIC_IMAGE_NAME_LENGTH) 
				? Texture.Name : GetTextureName());

			BuilderPlug.Me.CopiedTexture = texturename;
			if(General.Map.Config.MixTexturesFlats) BuilderPlug.Me.CopiedFlat = texturename;
			mode.SetActionResult("Copied texture \"" + texturename + "\".");
		}
		
		// Copy texture offsets
		public virtual void OnCopyTextureOffsets()
		{
			//mxd
			BuilderPlug.Me.CopiedOffsets = (General.Map.UDMF && General.Map.Config.UseLocalSidedefTextureOffsets) ? GetTextureOffset() : new Point(Sidedef.OffsetX, Sidedef.OffsetY);
			mode.SetActionResult("Copied texture offsets " + BuilderPlug.Me.CopiedOffsets.X + ", " + BuilderPlug.Me.CopiedOffsets.Y + ".");
		}

		// Copy properties
		public virtual void OnCopyProperties()
		{
			BuilderPlug.Me.CopiedLinedefProps = new LinedefProperties(Sidedef.Line); //mxd
			BuilderPlug.Me.CopiedSidedefProps = new SidedefProperties(Sidedef);
			mode.SetActionResult("Copied linedef and sidedef properties.");
		}

		// Paste properties
		public virtual void OnPasteProperties(bool usecopysettings)
		{
			if(BuilderPlug.Me.CopiedLinedefProps != null)
			{
				bool pastesideprops = (BuilderPlug.Me.CopiedSidedefProps != null); //mxd
				string pastetarget = (pastesideprops ? "linedef and sidedef" : "linedef"); //mxd
				mode.CreateUndo("Paste " + pastetarget + " properties");
				mode.SetActionResult("Pasted " + pastetarget + " properties.");
				BuilderPlug.Me.CopiedLinedefProps.Apply(new List<Linedef> { Sidedef.Line }, usecopysettings, false); //mxd
				if(pastesideprops) BuilderPlug.Me.CopiedSidedefProps.Apply(new List<Sidedef> { Sidedef }, usecopysettings); //mxd. Added "usecopysettings"
				
				// Update sectors on both sides
				BaseVisualSector front = (BaseVisualSector)mode.GetVisualSector(Sidedef.Sector);
				if(front != null) front.Changed = true;
				if(Sidedef.Other != null)
				{
					BaseVisualSector back = (BaseVisualSector)mode.GetVisualSector(Sidedef.Other.Sector);
					if(back != null) back.Changed = true;
				}
				mode.ShowTargetInfo();
			}
		}
		
		// Return texture name
		public virtual string GetTextureName() { return ""; }
		
		// Select button pressed
		public virtual void OnSelectBegin()
		{
			mode.LockTarget();
			dragstartanglexy = General.Map.VisualCamera.AngleXY;
			dragstartanglez = General.Map.VisualCamera.AngleZ;
			dragorigin = pickintersect;

			Point texoffset = GetTextureOffset(); //mxd
			startoffsetx = texoffset.X;
			startoffsety = texoffset.Y;
			prevoffsetx = texoffset.X;
			prevoffsety = texoffset.Y;
		}
		
		// Select button released
		public virtual void OnSelectEnd()
		{
			mode.UnlockTarget();
			
			// Was dragging?
			if(uvdragging)
			{
				// Dragging stops now
				uvdragging = false;
			}
			else
			{
				// Add/remove selection
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
		}
		
		// Edit button released
		public virtual void OnEditEnd()
		{
			if(General.Interface.IsActiveWindow)
			{
				List<Linedef> linedefs = mode.GetSelectedLinedefs();
				updatelist = new List<BaseVisualSector>(); //mxd
				foreach(Linedef l in linedefs) 
				{
					if(l.Front != null && mode.VisualSectorExists(l.Front.Sector)) 
						updatelist.Add((BaseVisualSector)mode.GetVisualSector(l.Front.Sector));
					if(l.Back != null && mode.VisualSectorExists(l.Back.Sector))
						updatelist.Add((BaseVisualSector)mode.GetVisualSector(l.Back.Sector));
				}

				//mxd. Always select front side for extrafloors
				Linedef sourceline = GetControlLinedef();
				Sidedef target = (sourceline != Sidedef.Line && sourceline.Front != null ? sourceline.Front : Sidedef);

				General.Interface.OnEditFormValuesChanged += Interface_OnEditFormValuesChanged;
				mode.StartRealtimeInterfaceUpdate(SelectionType.Linedefs);
				DialogResult result = General.Interface.ShowEditLinedefs(linedefs, target.IsFront, !target.IsFront);
				mode.StopRealtimeInterfaceUpdate(SelectionType.Linedefs);
				General.Interface.OnEditFormValuesChanged -= Interface_OnEditFormValuesChanged;

				updatelist.Clear();
				updatelist = null;

				//mxd. Effects may need updating...
				if(result == DialogResult.OK) mode.RebuildElementData();
			}
		}

		//mxd
		private void Interface_OnEditFormValuesChanged(object sender, EventArgs e) 
		{
			foreach(BaseVisualSector vs in updatelist) vs.UpdateSectorGeometry(false);
		}
		
		// Mouse moves
		public virtual void OnMouseMove(MouseEventArgs e)
		{
			// Dragging UV?
			if(uvdragging)
			{
				UpdateDragUV();
			}
			else if (mode.PaintSelectPressed) // biwa. Paint selection going on?
			{
				if (mode.PaintSelectType == this.GetType().BaseType && mode.Highlighted != this) // using BaseType so that middle, upper, lower, etc can be selecting in one go
				{
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

				return;
			}
			else
			{
				// Select button pressed?
				if(General.Actions.CheckActionActive(General.ThisAssembly, "visualselect"))
				{
					// Check if tolerance is exceeded to start UV dragging
					double deltaxy = General.Map.VisualCamera.AngleXY - dragstartanglexy;
					double deltaz = General.Map.VisualCamera.AngleZ - dragstartanglez;
					if((Math.Abs(deltaxy) + Math.Abs(deltaz)) > DRAG_ANGLE_TOLERANCE)
					{
						mode.PreAction(UndoGroup.TextureOffsetChange);
						mode.CreateUndo("Change texture offsets");

						// Start drag now
						uvdragging = true;
						mode.Renderer.ShowSelection = false;
						mode.Renderer.ShowHighlight = false;
						UpdateDragUV();
					}
				}
			}
		}
		
		// This is called to update UV dragging
		protected virtual void UpdateDragUV()
		{
			double u_ray;
			
			// Calculate intersection position
			Line2D ray = new Line2D(General.Map.VisualCamera.Position, General.Map.VisualCamera.Target);
			Sidedef.Line.Line.GetIntersection(ray, out u_ray);
			Vector3D intersect = General.Map.VisualCamera.Position + (General.Map.VisualCamera.Target - General.Map.VisualCamera.Position) * u_ray;
			
			// Calculate offsets
			Vector3D dragdelta = intersect - dragorigin;
			Vector3D dragdeltaxy = dragdelta * deltaxy;
			Vector3D dragdeltaz = dragdelta * deltaz;
			double offsetx = dragdeltaxy.GetLength();
			double offsety = dragdeltaz.GetLength();
			if((Math.Sign(dragdeltaxy.x) < 0) || (Math.Sign(dragdeltaxy.y) < 0) || (Math.Sign(dragdeltaxy.z) < 0)) offsetx = -offsetx;
			if((Math.Sign(dragdeltaz.x) < 0) || (Math.Sign(dragdeltaz.y) < 0) || (Math.Sign(dragdeltaz.z) < 0)) offsety = -offsety;
			
			// Apply offsets
			if(General.Interface.CtrlState && General.Interface.ShiftState) 
			{ 
				//mxd. Clamp to grid size?
				int newoffsetx = startoffsetx - (int)Math.Round(offsetx);
				int newoffsety = startoffsety + (int)Math.Round(offsety);
				int dx = prevoffsetx - newoffsetx;
				int dy = prevoffsety - newoffsety;

				if(Math.Abs(dx) >= General.Map.Grid.GridSize) 
				{
					dx = General.Map.Grid.GridSize * Math.Sign(dx);
					prevoffsetx = newoffsetx;
				} 
				else 
				{
					dx = 0;
				}

				if(Math.Abs(dy) >= General.Map.Grid.GridSize) 
				{
					dy = General.Map.Grid.GridSize * Math.Sign(dy);
					prevoffsety = newoffsety;
				} 
				else 
				{
					dy = 0;
				}

				if(dx != 0 || dy != 0) mode.ApplyTextureOffsetChange(dx, dy);
			} 
			else 
			{ 
				//mxd. Constraint to axis?
				int newoffsetx = (General.Interface.CtrlState ? startoffsetx : startoffsetx - (int)Math.Round(offsetx)); //mxd
				int newoffsety = (General.Interface.ShiftState ? startoffsety : startoffsety + (int)Math.Round(offsety)); //mxd
				mode.ApplyTextureOffsetChange(prevoffsetx - newoffsetx, prevoffsety - newoffsety);
				prevoffsetx = newoffsetx;
				prevoffsety = newoffsety;
			}
			
			mode.ShowTargetInfo();
		}
		
		// Sector brightness change
		public virtual void OnChangeTargetBrightness(bool up)
		{
			//mxd. Change UDMF wall light?
			if(General.Map.UDMF && (General.Map.Config.DistinctWallBrightness || General.Map.Config.DistinctSidedefPartBrightness))
			{
				string fieldname = "light";
				string fieldabsolutename = "lightabsolute";

				if(General.Map.Config.DistinctSidedefPartBrightness)
				{
					fieldname += "_" + partname;
					fieldabsolutename += "_" + partname;
				}

				int light = Sidedef.Fields.GetValue(fieldname, 0);
				bool absolute = Sidedef.Fields.GetValue(fieldabsolutename, false);
				int newlight;

				if(up)
					newlight = General.Map.Config.BrightnessLevels.GetNextHigher(light, absolute);
				else
					newlight = General.Map.Config.BrightnessLevels.GetNextLower(light, absolute);

				if(newlight == light) return;

				// Create undo
				mode.CreateUndo("Change wall brightness", UndoGroup.SurfaceBrightnessChange, Sector.Sector.FixedIndex);
				Sidedef.Fields.BeforeFieldsChange();

				// Apply changes
				UniFields.SetInteger(Sidedef.Fields, fieldname, newlight, (absolute ? int.MinValue : 0));
				Tools.UpdateLightFogFlag(Sidedef);
				mode.SetActionResult("Changed wall brightness to " + newlight + ".");

                // Update this part only
                //this.Setup();
                // [ZZ] why the hell was maxed updating only this part? sidedef change is global per sidedef, not only upper/lower/middle part.
                //      find this sidedef in sector, update all parts.
                VisualSidedefParts parts = Sector.GetSidedefParts(Sidedef);
                parts.SetupAllParts();
			}
			else if(!Sector.Changed)
			{
				// Change brightness
				mode.CreateUndo("Change sector brightness", UndoGroup.SectorBrightnessChange, Sector.Sector.FixedIndex);

				if(up)
					Sector.Sector.Brightness = General.Map.Config.BrightnessLevels.GetNextHigher(Sector.Sector.Brightness);
				else
					Sector.Sector.Brightness = General.Map.Config.BrightnessLevels.GetNextLower(Sector.Sector.Brightness);
				
				mode.SetActionResult("Changed sector brightness to " + Sector.Sector.Brightness + ".");

				Sector.Sector.UpdateCache();
				
				// Rebuild sector
				Sector.UpdateSectorGeometry(false);

				// Go for all things in this sector
				foreach(Thing t in General.Map.Map.Things)
				{
					if(t.Sector == Sector.Sector)
					{
						if(mode.VisualThingExists(t))
						{
							// Update thing
							BaseVisualThing vt = (BaseVisualThing)mode.GetVisualThing(t);
							vt.Changed = true;
						}
					}
				}
			}
		}
		
		// Texture offset change
		public virtual bool OnChangeTextureOffset(int horizontal, int vertical, bool doSurfaceAngleCorrection)
		{
			if((General.Map.UndoRedo.NextUndo == null) || (General.Map.UndoRedo.NextUndo.TicketID != undoticket))
				undoticket = mode.CreateUndo("Change texture offsets");
			
			//mxd
			if(General.Map.UDMF && General.Map.Config.UseLocalSidedefTextureOffsets)
			{
				// Apply per-texture offsets
				MoveTextureOffset(-horizontal, -vertical);
				Point p = GetTextureOffset();
				mode.SetActionResult("Changed texture offsets to " + p.X + ", " + p.Y + ".");

				// Update this part only
				this.Setup();
			} 
			else 
			{
				//mxd. Apply classic offsets
				bool textureloaded = (Texture != null && Texture.IsImageLoaded);
				Sidedef.OffsetX = (Sidedef.OffsetX - horizontal);
				if(textureloaded) Sidedef.OffsetX %= Texture.Width;
				Sidedef.OffsetY = (Sidedef.OffsetY - vertical);
				if(geometrytype != VisualGeometryType.WALL_MIDDLE && textureloaded) Sidedef.OffsetY %= Texture.Height;

				mode.SetActionResult("Changed texture offsets to " + Sidedef.OffsetX + ", " + Sidedef.OffsetY + ".");

				// Update all sidedef geometry
				VisualSidedefParts parts = Sector.GetSidedefParts(Sidedef);
				parts.SetupAllParts();
			}

			//mxd. Update linked effects
			SectorData sd = mode.GetSectorDataEx(Sector.Sector);
			if(sd != null) sd.Reset(true);

			return true;
		}

		//mxd
		public virtual void OnChangeScale(int incrementX, int incrementY) 
		{
			if(!General.Map.UDMF || Texture == null || !Texture.IsImageLoaded) return;

			if((General.Map.UndoRedo.NextUndo == null) || (General.Map.UndoRedo.NextUndo.TicketID != undoticket))
				undoticket = mode.CreateUndo("Change wall scale");

			string keyX;
			string keyY;

			switch(GeometryType) 
			{
				case VisualGeometryType.WALL_UPPER:
					keyX = "scalex_top";
					keyY = "scaley_top";
					break;

				case VisualGeometryType.WALL_MIDDLE:
					keyX = "scalex_mid";
					keyY = "scaley_mid";
					break;

				case VisualGeometryType.WALL_LOWER:
					keyX = "scalex_bottom";
					keyY = "scaley_bottom";
					break;

				default:
					throw new Exception("OnChangeTextureScale(): Got unknown GeometryType: " + GeometryType);
			}

			double scaleX = Sidedef.Fields.GetValue(keyX, 1.0);
			double scaleY = Sidedef.Fields.GetValue(keyY, 1.0);

			Sidedef.Fields.BeforeFieldsChange();

			if(incrementX != 0)
			{
				double pix = (int)Math.Round(Texture.Width * scaleX) - incrementX;
				double newscaleX = Math.Round(pix / Texture.Width, 3);
				scaleX = (newscaleX == 0 ? scaleX * -1 : newscaleX);
				UniFields.SetFloat(Sidedef.Fields, keyX, scaleX, 1.0);
			}

			if(incrementY != 0) 
			{
				double pix = (int)Math.Round(Texture.Height * scaleY) - incrementY;
				double newscaleY = Math.Round(pix / Texture.Height, 3);
				scaleY = (newscaleY == 0 ? scaleY * -1 : newscaleY);
				UniFields.SetFloat(Sidedef.Fields, keyY, scaleY, 1.0);
			}

			// Update geometry
			Setup();

			//mxd. Update linked effects
			SectorData sd = mode.GetSectorDataEx(Sector.Sector);
			if(sd != null) sd.Reset(true);

			mode.SetActionResult("Wall scale changed to " + scaleX.ToString("F03", CultureInfo.InvariantCulture) + ", " + scaleY.ToString("F03", CultureInfo.InvariantCulture) + " (" + (int)Math.Round(Texture.Width / scaleX) + " x " + (int)Math.Round(Texture.Height / scaleY) + ").");
		}

		// biwa
		public virtual void OnPaintSelectBegin()
		{
			mode.PaintSelectType = this.GetType().BaseType; // using BaseType so that middle, upper, lower, etc can be selecting in one go

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

		#endregion
	}
}
