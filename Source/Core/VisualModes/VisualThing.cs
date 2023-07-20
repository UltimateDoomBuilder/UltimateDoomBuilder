
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
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.GZBuilder.Data; //mxd
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Rendering;
using Plane = CodeImp.DoomBuilder.Geometry.Plane;
using CodeImp.DoomBuilder.GZBuilder;
using CodeImp.DoomBuilder.GZBuilder.Models;

#endregion

namespace CodeImp.DoomBuilder.VisualModes
{
	public abstract class VisualThing : IVisualPickable, IRenderResource, IDisposable
	{
		#region ================== Constants

		protected const int FIXED_RADIUS = 8; //mxd. Used to render things with zero width and radius
		private const float DYNLIGHT_INTENSITY_SCALER = 255.0f;
		private const float SUBLIGHT_INTENSITY_SCALER = 255.0f * 1.5f; // Scaler for subtractive dynamic lights

		#endregion
		
		#region ================== Variables
		
		// Thing
		private readonly Thing thing;

		//mxd. Info
		protected ThingTypeInfo info;
		
		// Textures
		protected ImageData[] textures;
		
		// Geometry
		private WorldVertex[][] vertices;
		private VertexBuffer[] geobuffers;
		private VertexBuffer cagebuffer; //mxd
		private int cagelength; //mxd
		private bool updategeo;
		private bool updatecage; //mxd
		private int[] triangles;
		private int spriteframe; //mxd
		
		// Rendering
		private RenderPass renderpass;
		private Matrix position;
		private double cameradistance;
		private Color4 cagecolor;
		protected bool sizeless; //mxd. Used to render visual things with 0 width and height
		protected float fogfactor; //mxd

		// Selected?
		protected bool selected;

		// Disposing
		private bool isdisposed;

		//mxd
		protected float thingheight;

		//mxd. light properties
		private GZGeneral.LightData lightType;
		private Color4 lightColor;
		private float lightRadius; //current radius. used in light animation
        private float lightSpotRadius1;
        private float lightSpotRadius2;
        private float lightPrimaryRadius;
		private float lightSecondaryRadius;
		private Vector3f position_v3;
		private float lightDelta; //used in light animation
		private Vector3D[] boundingBox;
		
		//gldefs light
		private Vector3f lightOffset;
		private int lightInterval;
		private bool isGldefsLight;

        // [ZZ]
        protected PixelColor stencilColor;
		
		#endregion
		
		#region ================== Properties
		
		internal VertexBuffer GeometryBuffer { get {
				if (geobuffers != null)
				{
					return geobuffers[spriteframe];
				}
				else
				{
					return null;
				}
			}
		}
		internal VertexBuffer CageBuffer { get { return cagebuffer; } } //mxd
		internal int CageLength { get { return cagelength; } } //mxd
		internal bool NeedsUpdateGeo { get { return updategeo; } }
		internal int Triangles { get { return triangles[spriteframe]; } }
		internal Matrix Position { get { return position; } }
		internal Color4 CageColor { get { return cagecolor; } }
		public ThingTypeInfo Info { get { return info; } } //mxd
		
		//mxd
		internal int VertexColor { get { return vertices.Length > 0 && vertices[0].Length > 0 ? vertices[0][0].c : 0; } }
		public double CameraDistance { get { return cameradistance; } }
		public float FogFactor { get { return fogfactor; } }
		public Vector3f Center
		{ 
			get
			{
                if (isGldefsLight) return position_v3 + lightOffset;
                else if (Thing.DynamicLightType != null) return position_v3; // fixes GZDoomBuilder-Bugfix#137
				return new Vector3f(position_v3.X, position_v3.Y, position_v3.Z + thingheight / 2f); 
			} 
		}
		public Vector3D CenterV3D { get { return RenderDevice.V3D(Center); } }
		public float LocalCenterZ { get { return thingheight / 2f; } } //mxd
		public Vector3f PositionV3 { get { return position_v3; } }
		public Vector3D[] BoundingBox { get { return boundingBox; } }
		
		//mxd. light properties
		public GZGeneral.LightData LightType { get { return lightType; } }
		public float LightRadius { get { return lightRadius; } }
        public float LightSpotRadius1 { get { return lightSpotRadius1; } }
        public float LightSpotRadius2 { get { return lightSpotRadius2; } }
        public Color4 LightColor { get { return lightColor; } }

        // [ZZ]
        public PixelColor StencilColor { get { return stencilColor; } }

        // [ZZ] this is used for spotlights
        public Vector3f VectorLookAt
        {
            get
            {
                // this esoteric value (1.5708) is 90 degrees but in radians
                return new Vector3f((float)(Math.Cos(Thing.Angle+1.5708) * Math.Cos(Angle2D.DegToRad(Thing.Pitch))), (float)(Math.Sin(Thing.Angle+1.5708) * Math.Cos(Angle2D.DegToRad(Thing.Pitch))), (float)Math.Sin(Angle2D.DegToRad(Thing.Pitch)));
            }
        }

		/// <summary>
		/// Returns the Thing that this VisualThing is created for.
		/// </summary>
		public Thing Thing { get { return thing; } }

		/// <summary>
		/// Render pass in which this geometry must be rendered. Default is Mask.
		/// </summary>
		public RenderPass RenderPass { get { return renderpass; } set { renderpass = value; } }
		
		/// <summary>
		/// Image to use as texture on the geometry.
		/// </summary>
		public ImageData Texture { get { return textures[spriteframe]; } }

		/// <summary>
		/// Disposed or not?
		/// </summary>
		public bool IsDisposed { get { return isdisposed; } }

		/// <summary>
		/// Selected or not? This is only used by the core to determine what color to draw it with.
		/// </summary>
		public bool Selected { get { return selected; } set { selected = value; } }
		
		#endregion
		
		#region ================== Constructor / Destructor
		
		// Constructor
		protected VisualThing(Thing t)
		{
			// Initialize
			this.thing = t;
			this.renderpass = RenderPass.Mask;
			this.position = Matrix.Identity;

            //mxd
            lightType = null;
			lightPrimaryRadius = -1;
			lightSecondaryRadius = -1;
			lightInterval = -1;
			lightColor = new Color4();
			boundingBox = new Vector3D[9];
			
			// Register as resource
			General.Map.Graphics.RegisterResource(this);
		}

		// Disposer
		public virtual void Dispose()
		{
			// Not already disposed?
			if(!isdisposed)
			{
				// Clean up
				if(geobuffers != null) //mxd
				{
					foreach(VertexBuffer buffer in geobuffers) buffer.Dispose();
					geobuffers = null;
				}

				if(cagebuffer != null) cagebuffer.Dispose(); //mxd
				cagebuffer = null; //mxd

				// Unregister resource
				General.Map.Graphics.UnregisterResource(this);

				// Done
				isdisposed = true;
			}
		}
		
		#endregion
		
		#region ================== Methods

		//mxd
		internal void CalculateCameraDistance(Vector3D campos) 
		{
			cameradistance = (CenterV3D - campos).GetLengthSq();
		}
		
		// This is called before a device is reset (when resized or display adapter was changed)
		public void UnloadResource()
		{
			// Trash geometry buffers
			if(geobuffers != null) //mxd
			{
				foreach(VertexBuffer buffer in geobuffers) buffer.Dispose();
				geobuffers = null;
			}

			if(cagebuffer != null) cagebuffer.Dispose(); //mxd
			cagebuffer = null; //mxd
			updategeo = true;
			updatecage = true; //mxd
		}
		
		// This is called resets when the device is reset
		// (when resized or display adapter was changed)
		public void ReloadResource()
		{
			// Make new geometry
			//Update();
		}

		/// <summary>
		/// Sets the color of the cage around the thing geometry and rebuilds the thing cage.
		/// </summary>
		protected void SetCageColor(PixelColor color)
		{
			cagecolor = color.ToColorValue();
			updatecage = true;
		}

		/// <summary>
		/// This sets the position to use for the thing geometry.
		/// </summary>
		public void SetPosition(Vector3D pos)
		{
			position_v3 = RenderDevice.V3(pos); //mxd
			position = Matrix.Translation(position_v3);
			updategeo = true;
			updatecage = true; //mxd

			//mxd. update bounding box?
            if (lightType != null && lightRadius > thing.Size)
			{
				UpdateBoundingBox(lightRadius, lightRadius * 2);
			} 
		}

		// This sets the vertices for the thing sprite
		protected void SetVertices(WorldVertex[][] verts, Vector2D[] offsets/*, Plane floor, Plane ceiling*/)
		{
			// Copy vertices
			vertices = new WorldVertex[verts.Length][];
			triangles = new int[verts.Length];

			//mxd
			for(int i = 0; i < verts.Length; i++)
			{
				vertices[i] = new WorldVertex[verts[i].Length];
				verts[i].CopyTo(vertices[i], 0);
				triangles[i] = vertices[i].Length / 3;
			}

			updategeo = true;
			
			//mxd. Do some special GZDoom rendering shenanigans...
			for(int c = 0; c < vertices.Length; c++)
			{
				if(triangles[c] < 2) continue;

				Matrix transform, rotation;
				double centerx, centerz;

				// ROLLCENTER flag support
				if(info.RollSprite && info.RollCenter && thing.Roll != 0)
				{
					// Rotate around sprite center
					centerx = offsets[c].x;
					centerz = vertices[c][1].z * 0.5f - offsets[c].y;
				}
				else
				{
					// Sprite center is already where it needs to be
					centerx = 0f;
					centerz = 0f;
				}

				switch(thing.RenderMode)
				{
					// Don't do anything
					case ThingRenderMode.MODEL: break;
					case ThingRenderMode.VOXEL: break;
					
					// Actor becomes a flat sprite which can be tilted with the use of the Pitch actor property.
					case ThingRenderMode.FLATSPRITE:
						transform = Matrix.Scaling((float)thing.ScaleX, (float)thing.ScaleX, (float)thing.ScaleY);

						// Apply roll?
						if(thing.Roll != 0)
						{
							if(info.RollCenter)
							{
								rotation = Matrix.RotationY((float)-thing.RollRad);
								transform *= Matrix.Translation((float)-centerx, (float)-centerx, (float)-centerz) * rotation * Matrix.Translation((float)centerx, (float)centerx, (float)centerz);
							}
							else
							{
								// Sprite center is already where it needs to be
								transform *= Matrix.RotationY((float)-thing.RollRad);
							}
						}

						// Apply pitch
						transform *= Matrix.RotationX((float)(thing.PitchRad + Angle2D.PIHALF));

						// Apply angle
						transform *= Matrix.RotationZ((float)thing.Angle);

						// Apply transform
						float zoffset = ((thing.Pitch == 0f && thing.Position.z == 0f) ? 0.1f : 0f); // Slight offset to avoid z-fighting...
						for(int i = 0; i < vertices[c].Length; i++)
						{
							Vector4f transformed = Vector3f.Transform(new Vector3f(vertices[c][i].x, vertices[c][i].y, vertices[c][i].z), transform);
							vertices[c][i].x = transformed.X;
							vertices[c][i].y = transformed.Y;
							vertices[c][i].z = transformed.Z + zoffset;
						}
						break;

					// Similar to FLATSPRITE but is not affected by pitch.
					case ThingRenderMode.WALLSPRITE:
						transform = Matrix.Scaling((float)thing.ScaleX, (float)thing.ScaleX, (float)thing.ScaleY);
						
						// Apply roll?
						if(thing.Roll != 0)
						{
							rotation = Matrix.RotationY((float)-thing.RollRad) * Matrix.RotationZ((float)thing.Angle);
							if(info.RollCenter)
								transform *= Matrix.Translation((float)-centerx, (float)-centerx, (float)-centerz) * rotation * Matrix.Translation((float)centerx, (float)centerx, (float)centerz);
							else
								transform *= rotation; // Sprite center is already where it needs to be
						}
						else
						{
							transform *= Matrix.RotationZ((float)thing.Angle);
						}

						// Apply transform
						for(int i = 0; i < vertices[c].Length; i++)
						{
							Vector4f transformed = Vector3f.Transform(new Vector3f(vertices[c][i].x, vertices[c][i].y, vertices[c][i].z), transform);
							vertices[c][i].x = transformed.X;
							vertices[c][i].y = transformed.Y;
							vertices[c][i].z = transformed.Z;
						}
						break;

					#region Some old GLOOME FLOOR_SPRITE/CEILING_SPRITE support code
					/*case Thing.SpriteRenderMode.FLOOR_SPRITE:
						Matrix floorrotation = Matrix.RotationZ(info.RollSprite ? Thing.RollRad : 0f)
											 * Matrix.RotationY(Thing.Angle)
											 * Matrix.RotationX(Angle2D.PIHALF);

						m = Matrix.Translation(0f, 0f, -localcenterz) * floorrotation * Matrix.Translation(0f, 0f, localcenterz);

						for(int i = 0; i < vertices[c].Length; i++)
						{
							Vector4 transformed = Vector3.Transform(new Vector3(vertices[c][i].x, vertices[c][i].y, vertices[c][i].z), m);
							vertices[c][i].x = transformed.X;
							vertices[c][i].y = transformed.Y;
							vertices[c][i].z = transformed.Z;
						}

						// TODO: this won't work on things with AbsoluteZ flag
						// TODO: +ROLLSPRITE implies +STICKTOPLANE?
						if(info.StickToPlane || info.RollSprite)
						{
							// Calculate vertical offset
							float floorz = floor.GetZ(Thing.Position);
							float ceilz = ceiling.GetZ(Thing.Position);

							if(!float.IsNaN(floorz) && !float.IsNaN(ceilz))
							{
								float voffset;
								if(info.Hangs)
								{
									float thingz = ceilz - Thing.Position.z + Thing.Height;
									voffset = 0.01f - floorz - General.Clamp(thingz, 0, ceilz - floorz);
								}
								else
								{
									voffset = 0.01f - floorz - General.Clamp(Thing.Position.z, 0, ceilz - floorz);
								}

								// Apply it
								for(int i = 0; i < vertices[c].Length; i++)
									vertices[c][i].z = floor.GetZ(vertices[c][i].x + Thing.Position.x, vertices[c][i].y + Thing.Position.y) + voffset;
							}
						}
						break;

					case Thing.SpriteRenderMode.CEILING_SPRITE:
						Matrix ceilrotation = Matrix.RotationZ(info.RollSprite ? Thing.RollRad : 0f)
											* Matrix.RotationY(Thing.Angle)
											* Matrix.RotationX(Angle2D.PIHALF);

						m = Matrix.Translation(0f, 0f, -localcenterz) * ceilrotation * Matrix.Translation(0f, 0f, localcenterz);

						for(int i = 0; i < vertices[c].Length; i++)
						{
							Vector4 transformed = Vector3.Transform(new Vector3(vertices[c][i].x, vertices[c][i].y, vertices[c][i].z), m);
							vertices[c][i].x = transformed.X;
							vertices[c][i].y = transformed.Y;
							vertices[c][i].z = transformed.Z;
						}

						// TODO: this won't work on things with AbsoluteZ flag
						// TODO: +ROLLSPRITE implies +STICKTOPLANE?
						if(info.StickToPlane || info.RollSprite)
						{
							// Calculate vertical offset
							float floorz = floor.GetZ(Thing.Position);
							float ceilz = ceiling.GetZ(Thing.Position);

							if(!float.IsNaN(floorz) && !float.IsNaN(ceilz))
							{
								float voffset;
								if(info.Hangs)
								{
									float thingz = ceilz - Math.Max(0, Thing.Position.z) - Thing.Height;
									voffset = -0.01f - General.Clamp(thingz, 0, ceilz - floorz);
								}
								else
								{
									voffset = -0.01f - floorz - General.Clamp(Thing.Position.z, 0, ceilz - floorz);
								}

								// Apply it
								for(int i = 0; i < vertices[c].Length; i++)
									vertices[c][i].z = ceiling.GetZ(vertices[c][i].x + Thing.Position.x, vertices[c][i].y + Thing.Position.y) + voffset;
							}
						}
						break;*/
					#endregion

					case ThingRenderMode.NORMAL:
						transform = Matrix.Scaling((float)thing.ScaleX, (float)thing.ScaleX, (float)thing.ScaleY);

						// Apply roll?
						if(info.RollSprite && thing.Roll != 0)
						{
							rotation = Matrix.RotationY((float)-thing.RollRad);
							if(info.RollCenter)
								transform *= Matrix.Translation((float)-centerx, (float)-centerx, (float)-centerz) * rotation * Matrix.Translation((float)centerx, (float)centerx, (float)centerz);
							else
								transform *= rotation; // Sprite center is already where it needs to be
						}

						// Apply transform
						for(int i = 0; i < vertices[c].Length; i++)
						{
							Vector4f transformed = Vector3f.Transform(new Vector3f(vertices[c][i].x, vertices[c][i].y, vertices[c][i].z), transform);
							vertices[c][i].x = transformed.X;
							vertices[c][i].y = transformed.Y;
							vertices[c][i].z = transformed.Z;
						}
						break;

					default: throw new NotImplementedException("Unknown ThingRenderMode");
				}
			}
		}
		
		// This updates the visual thing
		public virtual void Update()
		{
            RenderDevice graphics = General.Map.Graphics;

			// Do we need to update the geometry buffer?
			if(updategeo)
			{
				//mxd. Trash geometry buffers
				if(geobuffers != null)
					foreach(VertexBuffer geobuffer in geobuffers) geobuffer.Dispose();

				// Any vertics?
				if(vertices.Length > 0) 
				{
					geobuffers = new VertexBuffer[vertices.Length];
					for(int i = 0; i < vertices.Length; i++)
					{
						// Make a new buffer
						geobuffers[i] = new VertexBuffer();

                        // Fill the buffer
                        graphics.SetBufferData(geobuffers[i], vertices[i]);
					}
				}
				
				//mxd. Check if thing is light
				CheckLightState();

				// Done
				updategeo = false;
			}

			//mxd. Need to update thing cage?
			if(updatecage)
			{
				// Trash cage buffer
				if(cagebuffer != null) cagebuffer.Dispose();
				cagebuffer = null;

				// Make a new cage
				List<WorldVertex> cageverts;
				if(sizeless)
				{
					WorldVertex v0 = new WorldVertex(-thing.Size + position_v3.X, -thing.Size + position_v3.Y, position_v3.Z);
					WorldVertex v1 = new WorldVertex(thing.Size + position_v3.X, thing.Size + position_v3.Y, position_v3.Z);
					WorldVertex v2 = new WorldVertex(thing.Size + position_v3.X, -thing.Size + position_v3.Y, position_v3.Z);
					WorldVertex v3 = new WorldVertex(-thing.Size + position_v3.X, thing.Size + position_v3.Y, position_v3.Z);
					WorldVertex v4 = new WorldVertex(position_v3.X, position_v3.Y, thing.Size + position_v3.Z);
					WorldVertex v5 = new WorldVertex(position_v3.X, position_v3.Y, -thing.Size + position_v3.Z);

					cageverts = new List<WorldVertex>(new[] { v0, v1, v2, v3, v4, v5 });
				}
				else
				{
					float top = position_v3.Z + thing.Height;
					float bottom = position_v3.Z;

					WorldVertex v0 = new WorldVertex(-thing.Size + position_v3.X, -thing.Size + position_v3.Y, bottom);
					WorldVertex v1 = new WorldVertex(-thing.Size + position_v3.X, thing.Size + position_v3.Y, bottom);
					WorldVertex v2 = new WorldVertex(thing.Size + position_v3.X, thing.Size + position_v3.Y, bottom);
					WorldVertex v3 = new WorldVertex(thing.Size + position_v3.X, -thing.Size + position_v3.Y, bottom);

					WorldVertex v4 = new WorldVertex(-thing.Size + position_v3.X, -thing.Size + position_v3.Y, top);
					WorldVertex v5 = new WorldVertex(-thing.Size + position_v3.X, thing.Size + position_v3.Y, top);
					WorldVertex v6 = new WorldVertex(thing.Size + position_v3.X, thing.Size + position_v3.Y, top);
					WorldVertex v7 = new WorldVertex(thing.Size + position_v3.X, -thing.Size + position_v3.Y, top);

					cageverts = new List<WorldVertex>(new[] { v0, v1,	
															  v1, v2,
															  v2, v3,
															  v3, v0,
															  v4, v5, 
															  v5, v6,
															  v6, v7,
															  v7, v4,
															  v0, v4,
															  v1, v5,
															  v2, v6,
															  v3, v7 });
				}

				// Make new arrow
				if(Thing.IsDirectional)
				{
					Matrix transform = Matrix.Scaling(thing.Size, thing.Size, thing.Size)
						* (Matrix.RotationY((float)-Thing.RollRad) * Matrix.RotationX((float)-Thing.PitchRad) * Matrix.RotationZ((float)Thing.Angle))
						* (sizeless ? position : position * Matrix.Translation(0.0f, 0.0f, thingheight / 2f));

					WorldVertex a0 = new WorldVertex(Vector3D.Transform(0.0f, 0.0f, 0.0f, transform)); //start
					WorldVertex a1 = new WorldVertex(Vector3D.Transform(0.0f, -1.5f, 0.0f, transform)); //end
					WorldVertex a2 = new WorldVertex(Vector3D.Transform(0.2f, -1.1f, 0.2f, transform));
					WorldVertex a3 = new WorldVertex(Vector3D.Transform(-0.2f, -1.1f, 0.2f, transform));
					WorldVertex a4 = new WorldVertex(Vector3D.Transform(0.2f, -1.1f, -0.2f, transform));
					WorldVertex a5 = new WorldVertex(Vector3D.Transform(-0.2f, -1.1f, -0.2f, transform));

					cageverts.AddRange(new[] { a0, a1,
											   a1, a2,
											   a1, a3,
											   a1, a4,
											   a1, a5 });
				}

				// Create buffer
				WorldVertex[] cv = cageverts.ToArray();
				cagelength = cv.Length / 2;
				cagebuffer = new VertexBuffer();
                graphics.SetBufferData(cagebuffer, cv);

				// Done
				updatecage = false;
			}
		}

		//mxd
		protected void CheckLightState() 
		{
            //mxd. Check if thing is light
            if (thing.DynamicLightType != null)
			{
				isGldefsLight = false;
				lightInterval = -1;
				UpdateLight();
			}
			//check if we have light from GLDEFS
			else if(General.Map.Data.GldefsEntries.ContainsKey(thing.Type)) 
			{
				isGldefsLight = true;
				UpdateGldefsLight();
				UpdateBoundingBox(lightRadius, lightRadius * 2);
			} 
			else 
			{
				UpdateBoundingBox((int)thing.Size, thingheight);

                lightType = null;
				lightRadius = -1;
                lightSpotRadius1 = lightSpotRadius2 = -1;
				lightPrimaryRadius = -1;
				lightSecondaryRadius = -1;
				lightInterval = -1;
				isGldefsLight = false;
			}
		}

		//mxd. Update light info
		public void UpdateLight()
		{
            lightType = thing.DynamicLightType;
            if (lightType == null || lightType.LightType == GZGeneral.LightType.SUN)
                return;
            GZGeneral.LightData ld = lightType;
			if (ld.LightDef != GZGeneral.LightDef.VAVOOM_GENERIC &&
                ld.LightDef != GZGeneral.LightDef.VAVOOM_COLORED) //if it's gzdoom light
			{
                if (ld.LightType == GZGeneral.LightType.POINT)
                {
                    if (ld.LightDef != GZGeneral.LightDef.POINT_SUBTRACTIVE) // normal, additive, attenuated
                    {
						// ZDRay static lights have an intensity that's set through the thing's alpha value
						float intensity = ld.LightRenderStyle == GZGeneral.LightRenderStyle.STATIC ? (float)thing.Fields.GetValue("alpha", 1.0) : 1.0f;

                        //lightColor.Alpha used in shader to perform some calculations based on light type
                        lightColor = new Color4(
                            thing.Args[0] / DYNLIGHT_INTENSITY_SCALER * intensity,
                            thing.Args[1] / DYNLIGHT_INTENSITY_SCALER * intensity,
                            thing.Args[2] / DYNLIGHT_INTENSITY_SCALER * intensity,
                            (float)ld.LightRenderStyle / 100.0f);
                    }
                    else // negative
                    {
                        lightColor = new Color4(
                            thing.Args[0] / SUBLIGHT_INTENSITY_SCALER,
                            thing.Args[1] / SUBLIGHT_INTENSITY_SCALER,
                            thing.Args[2] / SUBLIGHT_INTENSITY_SCALER,
                            (float)ld.LightRenderStyle / 100.0f);
                    }
                }
                else
                {
                    int c1, c2, c3;
                    if (thing.Fields.ContainsKey("arg0str"))
                    {
                        PixelColor pc;
                        ZDoom.ZDTextParser.GetColorFromString(thing.Fields["arg0str"].Value.ToString(), out pc);
                        c1 = pc.r;
                        c2 = pc.g;
                        c3 = pc.b;
                    }
                    else
                    {
                        c1 = (thing.Args[0] & 0xFF0000) >> 16;
                        c2 = (thing.Args[0] & 0x00FF00) >> 8;
                        c3 = (thing.Args[0] & 0x0000FF);
                    }

                    if (ld.LightDef != GZGeneral.LightDef.SPOT_SUBTRACTIVE)
                    {
						// ZDRay static lights have an intensity that's set through the thing's alpha value
						float intensity = ld.LightRenderStyle == GZGeneral.LightRenderStyle.STATIC ? (float)thing.Fields.GetValue("alpha", 1.0) : 1.0f;

						lightColor = new Color4(
                            c1 / DYNLIGHT_INTENSITY_SCALER * intensity,
                            c2 / DYNLIGHT_INTENSITY_SCALER * intensity,
                            c3 / DYNLIGHT_INTENSITY_SCALER * intensity,
                            (float)ld.LightRenderStyle / 100.0f);
                    }
                    else
                    {
                        lightColor = new Color4(
                            c1 / SUBLIGHT_INTENSITY_SCALER,
                            c2 / SUBLIGHT_INTENSITY_SCALER,
                            c3 / SUBLIGHT_INTENSITY_SCALER,
                            (float)ld.LightRenderStyle / 100.0f);
                    }
                }

				if(lightType.LightModifier == GZGeneral.LightModifier.SECTOR) 
				{
					int scaler = 1;
					if(thing.Sector != null) scaler = thing.Sector.Brightness / 4;
					lightPrimaryRadius = (thing.Args[3] * scaler);
				} 
				else 
				{
					lightPrimaryRadius = (thing.Args[3] * 2); //works... that.. way in GZDoom
                    if (lightType.LightAnimated)
					    lightSecondaryRadius = (thing.Args[4] * 2);
				}

                if (lightType.LightType == GZGeneral.LightType.SPOT)
                {
                    lightSpotRadius1 = (thing.Args[1]);
                    lightSpotRadius2 = (thing.Args[2]);
                }
			}
			else //it's one of vavoom lights
			{ 
				if(lightType.LightDef == GZGeneral.LightDef.VAVOOM_COLORED)
				{
					lightColor = new Color4(
						thing.Args[1] / DYNLIGHT_INTENSITY_SCALER,
						thing.Args[2] / DYNLIGHT_INTENSITY_SCALER,
						thing.Args[3] / DYNLIGHT_INTENSITY_SCALER,
                        (float)ld.LightRenderStyle / 100.0f);
				}
				else
				{
					lightColor = new Color4(0.5f, 0.5f, 0.5f, (float)ld.LightRenderStyle / 100.0f);
				}
					
				lightPrimaryRadius = (thing.Args[0] * 8);
			}

			UpdateLightRadius();
            UpdateBoundingBox(lightRadius, lightRadius * 2);
        }

		//mxd
		private void UpdateGldefsLight() 
		{
			DynamicLightData light = General.Map.Data.GldefsEntries[thing.Type];
            GZGeneral.LightData ld = light.Type;

            //apply settings
			lightColor = new Color4(light.Color.Red, light.Color.Green, light.Color.Blue, (float)ld.LightRenderStyle / 100.0f);
			Vector2D o = new Vector2D(light.Offset.X, light.Offset.Y).GetRotated(thing.Angle - Angle2D.PIHALF);
			lightOffset = new Vector3f((float)o.x, (float)o.y, light.Offset.Z);
			lightType = light.Type;

			if(ld.LightModifier == GZGeneral.LightModifier.SECTOR)
			{
				lightPrimaryRadius = light.Interval * thing.Sector.Brightness / 5.0f;
			} 
			else 
			{
				lightPrimaryRadius = light.PrimaryRadius;
				lightSecondaryRadius = light.SecondaryRadius;
			}

			lightInterval = light.Interval;
			UpdateLightRadius(lightInterval);
		}

		//mxd
		public void UpdateLightRadius() 
		{
			UpdateLightRadius( (lightInterval != -1 ? lightInterval : thing.AngleDoom) );
		}

		//mxd
		private void UpdateLightRadius(int interval) 
		{
			if(lightType == null) return;

			if(General.Settings.GZDrawLightsMode == LightRenderMode.ALL || !lightType.LightAnimated) 
			{
				lightRadius = lightPrimaryRadius;
				return;
			}

			if(interval == 0) 
			{
				lightRadius = 0;
				return;
			}

			float rMin = Math.Min(lightPrimaryRadius, lightSecondaryRadius);
			float rMax = Math.Max(lightPrimaryRadius, lightSecondaryRadius);
			float diff = rMax - rMin;

			switch(lightType.LightModifier) 
			{
				case GZGeneral.LightModifier.PULSE:
					lightDelta = ((float)Math.Sin(Clock.CurrentTime / (interval * 4.0f)) + 1.0f) / 2.0f; //just playing by the eye here... in [0.0 ... 1.0] interval
					lightRadius = rMin + diff * lightDelta;
					break;

				case GZGeneral.LightModifier.FLICKER:
					float fdelta = (float)Math.Sin(Clock.CurrentTime / 0.1f); //just playing by the eye here...
					if(Math.Sign(fdelta) != Math.Sign(lightDelta)) 
					{
						lightDelta = fdelta;
						lightRadius = (General.Random(0, 359) < interval ? rMax : rMin);
					}
					break;

				case GZGeneral.LightModifier.FLICKERRANDOM:
					float rdelta = (float)Math.Sin(Clock.CurrentTime / (interval * 9.0f)); //just playing by the eye here...
					if(Math.Sign(rdelta) != Math.Sign(lightDelta)) 
					{
						lightRadius = rMin + (General.Random(0, (int) (diff * 10))) / 10.0f;
					}
					lightDelta = rdelta;
					break;
			}
		}

		//mxd. update bounding box
		public void UpdateBoundingBox() 
		{
			if(lightType != null && lightRadius > thing.Size)
				UpdateBoundingBox(lightRadius, lightRadius * 2);
		}

		private void UpdateBoundingBox(float width, float height) 
		{
			boundingBox = new Vector3D[9];
			boundingBox[0] = CenterV3D;

            if (Thing.RenderMode != ThingRenderMode.MODEL && Thing.RenderMode != ThingRenderMode.VOXEL)
            {

                float h2 = height / 2.0f;

                boundingBox[1] = new Vector3D(position_v3.X - width, position_v3.Y - width, Center.Z - h2);
                boundingBox[2] = new Vector3D(position_v3.X + width, position_v3.Y - width, Center.Z - h2);
                boundingBox[3] = new Vector3D(position_v3.X - width, position_v3.Y + width, Center.Z - h2);
                boundingBox[4] = new Vector3D(position_v3.X + width, position_v3.Y + width, Center.Z - h2);

                boundingBox[5] = new Vector3D(position_v3.X - width, position_v3.Y - width, Center.Z + h2);
                boundingBox[6] = new Vector3D(position_v3.X + width, position_v3.Y - width, Center.Z + h2);
                boundingBox[7] = new Vector3D(position_v3.X - width, position_v3.Y + width, Center.Z + h2);
                boundingBox[8] = new Vector3D(position_v3.X + width, position_v3.Y + width, Center.Z + h2);

            }
            else
            {

                GZModel model = General.Map?.Data?.ModeldefEntries[Thing.Type]?.Model;

                if (model != null)
                {

                    Vector3D offs = new Vector3D(position_v3.X, position_v3.Y, position_v3.Z);

                    boundingBox[5] = new Vector3D(model.BBox.MinX, model.BBox.MinY, model.BBox.MaxZ) + offs;
                    boundingBox[6] = new Vector3D(model.BBox.MaxX, model.BBox.MinY, model.BBox.MaxZ) + offs;
                    boundingBox[7] = new Vector3D(model.BBox.MinX, model.BBox.MaxY, model.BBox.MaxZ) + offs;
                    boundingBox[8] = new Vector3D(model.BBox.MaxX, model.BBox.MaxY, model.BBox.MaxZ) + offs;

                    boundingBox[1] = new Vector3D(model.BBox.MinX, model.BBox.MinY, model.BBox.MinZ) + offs;
                    boundingBox[2] = new Vector3D(model.BBox.MaxX, model.BBox.MinY, model.BBox.MinZ) + offs;
                    boundingBox[3] = new Vector3D(model.BBox.MinX, model.BBox.MaxY, model.BBox.MinZ) + offs;
                    boundingBox[4] = new Vector3D(model.BBox.MaxX, model.BBox.MaxY, model.BBox.MinZ) + offs;

                    boundingBox[0] = new Vector3D(0.5f * (model.BBox.MinX + model.BBox.MaxX),
                                                  0.5f * (model.BBox.MinY + model.BBox.MaxY),
                                                  0.5f * (model.BBox.MinZ + model.BBox.MaxZ)) + offs;

                }

            }
		}

		//mxd. This updates the sprite frame to be rendered
		internal void UpdateSpriteFrame()
		{
			if(textures.Length != 8)
				spriteframe = 0;
			else
				spriteframe = (General.ClampAngle((int)Angle2D.RadToDeg((General.Map.VisualCamera.Position - thing.Position).GetAngleXY()) - thing.AngleDoom + 292)) / 45; // Convert to [0..7] range; 292 == 270 + 45/2
		}
		
		/// <summary>
		/// This is called when the thing must be tested for line intersection. This should reject
		/// as fast as possible to rule out all geometry that certainly does not touch the line.
		/// </summary>
		public virtual bool PickFastReject(Vector3D from, Vector3D to, Vector3D dir)
		{
			return false;
		}
		
		/// <summary>
		/// This is called when the thing must be tested for line intersection. This should perform
		/// accurate hit detection and set u_ray to the position on the ray where this hits the geometry.
		/// </summary>
		public virtual bool PickAccurate(Vector3D from, Vector3D to, Vector3D dir, ref double u_ray)
		{
			return false;
		}
		
		#endregion
	}
}

