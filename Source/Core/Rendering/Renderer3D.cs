
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
using System.Drawing.Drawing2D;
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.GZBuilder.Data;
using CodeImp.DoomBuilder.GZBuilder.MD3;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.VisualModes;
using CodeImp.DoomBuilder.GZBuilder;
using ColorMap = System.Drawing.Imaging.ColorMap;

#endregion

namespace CodeImp.DoomBuilder.Rendering
{
	internal sealed class Renderer3D : Renderer, IRenderer3D
	{
		#region ================== Constants

		private const float PROJ_NEAR_PLANE = 1f;
		private const float FOG_RANGE = 0.9f;
        private const int MAX_DYNLIGHTS_PER_SURFACE = 64;

		#endregion

		#region ================== Variables

		// Matrices
		private Matrix projection;
		private Matrix view3d;
		private Matrix billboard;
		private Matrix view2d;
		private Matrix world;
		private Vector3D cameraposition;
        private Vector3D cameravector;
		private ShaderName shaderpass;

        // Window size
        private Size windowsize;
		
		// Frustum
		private ProjectedFrustum2D frustum;
		
		// Thing cage
		private bool renderthingcages;
		//mxd
		private VisualVertexHandle vertexhandle;
		private int[] lightOffsets;
		
		private Data.ColorMap classicLightingColorMap = null;
		private Texture classicLightingColorMapTex = null;
		
		// Slope handle
		private VisualSlopeHandle visualslopehandle;

		// Crosshair
		private FlatVertex[] crosshairverts;
		private bool crosshairbusy;

		// Highlighting
		private IVisualPickable highlighted;
		private float highlightglow;
		private float highlightglowinv;
		private bool showselection;
		private bool showhighlight;
		
		//mxd. Solid geometry to be rendered. Must be sorted by sector.
		private Dictionary<ImageData, List<VisualGeometry>> solidgeo;

		//mxd. Masked geometry to be rendered. Must be sorted by sector.
		private Dictionary<ImageData, List<VisualGeometry>> maskedgeo;

		//mxd. Translucent geometry to be rendered. Must be sorted by camera distance.
		private List<VisualGeometry> translucentgeo;

		//mxd. Geometry to be rendered as skybox.
		private List<VisualGeometry> skygeo;

		//mxd. Solid things to be rendered (currently(?) there won't be any). Must be sorted by sector.
		private Dictionary<ImageData, List<VisualThing>> solidthings;

		//mxd. Masked things to be rendered. Must be sorted by sector.
		private Dictionary<ImageData, List<VisualThing>> maskedthings;

		//mxd. Translucent things to be rendered. Must be sorted by camera distance.
		private List<VisualThing> translucentthings;

		//mxd. Things with attached dynamic lights
		private List<VisualThing> lightthings;
		
		//mxd. Things, which should be rendered as models
		private Dictionary<ModelData, List<VisualThing>> maskedmodelthings;

		//mxd. Things, which should be rendered as translucent models
		private List<VisualThing> translucentmodelthings;

		//mxd. All things. Used to render thing cages
		private List<VisualThing> allthings;

		//mxd. Visual vertices
		private List<VisualVertex> visualvertices;

		// Visual slope handles
		private ICollection<VisualSlope> visualslopehandles;

		//mxd. Event lines
		private List<Line3D> eventlines;

        // FPS-related
        private int fps = 0;
        private System.Diagnostics.Stopwatch fpsWatch;
        private TextLabel fpsLabel;

        #endregion

        #region ================== Properties

        public ProjectedFrustum2D Frustum2D { get { return frustum; } }
		public bool DrawThingCages { get { return renderthingcages; } set { renderthingcages = value; } }
		public bool ShowSelection { get { return showselection; } set { showselection = value; } }
		public bool ShowHighlight { get { return showhighlight; } set { showhighlight = value; } }
		
		protected bool UseIndexedTexture
		{
			get { return General.Settings.ClassicRendering && !fullbrightness; }
		}
		
		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		internal Renderer3D(RenderDevice graphics) : base(graphics)
		{
			// Initialize
			//CreateProjection(); // [ZZ] don't do undefined things once not even ready
			CreateMatrices2D();
			renderthingcages = true;
			showselection = true;
			showhighlight = true;
			eventlines = new List<Line3D>(); //mxd
			
			// Dummy frustum
			frustum = new ProjectedFrustum2D(new Vector2D(), 0.0f, 0.0f, PROJ_NEAR_PLANE,
				General.Settings.ViewDistance, (float)Angle2D.DegToRad(General.Settings.VisualFOV));

            fpsLabel = new TextLabel();
            fpsLabel.AlignX = TextAlignmentX.Left;
            fpsLabel.AlignY = TextAlignmentY.Top;
            fpsLabel.Text = "(FPS unavailable)";
            fpsWatch = new System.Diagnostics.Stopwatch();

            // We have no destructor
            GC.SuppressFinalize(this);
		}

		// Disposer
		public override void Dispose()
		{
			// Not already disposed?
			if(!isdisposed)
			{
				// Clean up
				if(vertexhandle != null) vertexhandle.Dispose(); //mxd
				if (visualslopehandle != null) visualslopehandle.Dispose();

				// Done
				base.Dispose();
			}
		}

		#endregion

		#region ================== Management

		// This is called before a device is reset
		// (when resized or display adapter was changed)
		public override void UnloadResource()
		{
			crosshairverts = null;
		}
		
		// This is called resets when the device is reset
		// (when resized or display adapter was changed)
		public override void ReloadResource()
		{
			CreateMatrices2D();
		}

		// This makes screen vertices for display
		private void CreateCrosshairVerts(Size texturesize)
		{
			// Determine coordinates
			float width = windowsize.Width;
			float height = windowsize.Height;
			RectangleF rect = new RectangleF((float)Math.Round((width - texturesize.Width) * 0.5f), (float)Math.Round((height - texturesize.Height) * 0.5f), texturesize.Width, texturesize.Height);
			
			// Make vertices
			crosshairverts = new FlatVertex[4];
			crosshairverts[0].x = rect.Left;
			crosshairverts[0].y = rect.Top;
			crosshairverts[0].c = -1;
			crosshairverts[1].x = rect.Right;
			crosshairverts[1].y = rect.Top;
			crosshairverts[1].c = -1;
			crosshairverts[1].u = 1.0f;
			crosshairverts[2].x = rect.Left;
			crosshairverts[2].y = rect.Bottom;
			crosshairverts[2].c = -1;
			crosshairverts[2].v = 1.0f;
			crosshairverts[3].x = rect.Right;
			crosshairverts[3].y = rect.Bottom;
			crosshairverts[3].c = -1;
			crosshairverts[3].u = 1.0f;
			crosshairverts[3].v = 1.0f;
		}
		
		#endregion

		#region ================== Resources

		//mxd
		internal void UpdateVertexHandle()
		{
			if(vertexhandle != null)
			{
				vertexhandle.UnloadResource();
				vertexhandle.ReloadResource();
			}
		}

		internal void UpdateVisualSlopeHandle()
		{
			if (visualslopehandle != null)
			{
				visualslopehandle.UnloadResource();
				visualslopehandle.ReloadResource();
			}
		}

		#endregion

		#region ================== Presentation

		// This creates the projection
		internal void CreateProjection()
		{
			// Calculate aspect
			float screenheight = General.Map.Graphics.RenderTarget.ClientSize.Height * (General.Settings.GZStretchView ? General.Map.Data.InvertedVerticalViewStretch : 1.0f); //mxd
			float aspect = General.Map.Graphics.RenderTarget.ClientSize.Width / screenheight;
			
			// The DirectX PerspectiveFovRH matrix method calculates the scaling in X and Y as follows:
			// yscale = 1 / tan(fovY / 2)
			// xscale = yscale / aspect
			// The fov specified in the method is the FOV over Y, but we want the user to specify the FOV
			// over X, so calculate what it would be over Y first;
			float fov = (float)Angle2D.DegToRad(General.Settings.VisualFOV);
			float reversefov = 1.0f / (float)Math.Tan(fov / 2.0f);
			float reversefovy = reversefov * aspect;
			float fovy = (float)Math.Atan(1.0f / reversefovy);
			
			// Make the projection matrix
			projection = Matrix.PerspectiveFov(fovy, aspect, PROJ_NEAR_PLANE, General.Settings.ViewDistance);
		}
		
		// This creates matrices for a camera view
		public void PositionAndLookAt(Vector3D pos, Vector3D lookat)
		{
			// Calculate delta vector
			cameraposition = pos;
            Vector3D delta = lookat - pos;
            cameravector = delta.GetNormal();
			double anglexy = delta.GetAngleXY();
			double anglez = delta.GetAngleZ();

			// Create frustum
			frustum = new ProjectedFrustum2D(pos, (float)anglexy, (float)anglez, PROJ_NEAR_PLANE,
				General.Settings.ViewDistance, (float)Angle2D.DegToRad(General.Settings.VisualFOV));

            // Make the view matrix
            view3d = Matrix.LookAt(RenderDevice.V3(pos), RenderDevice.V3(lookat), new Vector3f(0f, 0f, 1f));
			
			// Make the billboard matrix
			billboard = Matrix.RotationZ((float)(anglexy + Angle2D.PI));
		}
		
		
		// volte: Set the active colormap for classic lighting
		public void SetClassicLightingColorMap(Data.ColorMap colormap)
		{
			if (colormap == classicLightingColorMap) 
			{
				return;
			}
			classicLightingColorMap = colormap;
			if (classicLightingColorMap == null)
			{
				classicLightingColorMapTex = null;
			}
			else 
			{
				classicLightingColorMapTex = new Texture(graphics, classicLightingColorMap.CreateBitmap());
			}
		}
		
		// This creates 2D view matrix
		private void CreateMatrices2D()
		{
			windowsize = graphics.RenderTarget.ClientSize;
			Matrix scaling = Matrix.Scaling((1f / windowsize.Width) * 2f, (1f / windowsize.Height) * -2f, 1f);
			Matrix translate = Matrix.Translation(-(float)windowsize.Width * 0.5f, -(float)windowsize.Height * 0.5f, 0f);
			view2d = translate * scaling;
		}

		#endregion

		#region ================== Start / Finish

		// This starts rendering
		public bool Start()
		{
            if (General.Settings.ShowFPS && !fpsWatch.IsRunning)
                fpsWatch.Start();

            // Start drawing
            graphics.StartRendering(true, General.Colors.Background.ToColorValue());

			// Beginning renderstates
			graphics.SetCullMode(Cull.None);
			graphics.SetZEnable(false);
			graphics.SetAlphaBlendEnable(false);
			graphics.SetAlphaTestEnable(false);
			graphics.SetSourceBlend(Blend.SourceAlpha);
			graphics.SetDestinationBlend(Blend.InverseSourceAlpha);
			graphics.SetUniform(UniformName.fogsettings, new Vector4f(-1.0f));
			graphics.SetUniform(UniformName.fogcolor, General.Colors.Background.ToColorValue());
			graphics.SetUniform(UniformName.texturefactor, new Color4(1f, 1f, 1f, 1f));
			graphics.SetUniform(UniformName.doomlightlevels, General.Map.Config.DoomLightLevels);
            graphics.SetUniform(UniformName.highlightcolor, new Color4()); //mxd
            TextureFilter texFilter = (!General.Settings.ClassicRendering && General.Settings.VisualBilinear) ? TextureFilter.Linear : TextureFilter.Nearest;
            MipmapFilter mipFilter = General.Settings.ClassicRendering ? MipmapFilter.None : MipmapFilter.Linear;
            float aniso = General.Settings.ClassicRendering ? 0 : General.Settings.FilterAnisotropy;
            graphics.SetSamplerFilter(texFilter, texFilter, mipFilter, aniso);

            // Texture addressing
            graphics.SetSamplerState(TextureAddress.Wrap);

            // Matrices
			world = Matrix.Identity;
            graphics.SetUniform(UniformName.projection, ref projection);
            graphics.SetUniform(UniformName.world, ref world);
            graphics.SetUniform(UniformName.view, ref view3d);

			// Highlight
			if(General.Settings.AnimateVisualSelection)
			{
				highlightglow = (float)Math.Sin(Clock.CurrentTime / 100.0f) * 0.1f + 0.4f;
				highlightglowinv = -highlightglow + 0.8f;
			}
			else
			{
				highlightglow = 0.4f;
				highlightglowinv = 0.3f;
			}
				
			// Determine shader pass to use
			if (fullbrightness)
			{
				shaderpass = ShaderName.world3d_fullbright;
			} 
			else if (General.Settings.ClassicRendering)
			{
				shaderpass = ShaderName.world3d_classic;
			} 
			else
			{
				shaderpass = ShaderName.world3d_main;
			}

			// Create crosshair vertices
			if(crosshairverts == null)
				CreateCrosshairVerts(new Size(General.Map.Data.Crosshair3D.Width, General.Map.Data.Crosshair3D.Height));

			//mxd. Crate vertex handle
			if(vertexhandle == null) vertexhandle = new VisualVertexHandle();

			// Create slope handle
			if (visualslopehandle == null) visualslopehandle = new VisualSlopeHandle();

			// Ready
			return true;
		}
		
		// This begins rendering world geometry
		public void StartGeometry()
		{
			// Make collections
			solidgeo = new Dictionary<ImageData, List<VisualGeometry>>(); //mxd
			maskedgeo = new Dictionary<ImageData, List<VisualGeometry>>(); //mxd
			translucentgeo = new List<VisualGeometry>(); //mxd
			skygeo = new List<VisualGeometry>(); //mxd

			solidthings = new Dictionary<ImageData, List<VisualThing>>(); //mxd
			maskedthings = new Dictionary<ImageData, List<VisualThing>>(); //mxd
			translucentthings = new List<VisualThing>(); //mxd
			
			maskedmodelthings = new Dictionary<ModelData, List<VisualThing>>(); //mxd
			translucentmodelthings = new List<VisualThing>(); //mxd
			lightthings = new List<VisualThing>(); //mxd
			allthings = new List<VisualThing>(); //mxd
		}

		// This ends rendering world geometry
		public void FinishGeometry()
		{
			//mxd. Sort lights
			if(General.Settings.GZDrawLightsMode != LightRenderMode.NONE && !fullbrightness && lightthings.Count > 0)
				UpdateLights();

			// Initial renderstates
			graphics.SetCullMode(Cull.Clockwise);
			graphics.SetZEnable(true);
			graphics.SetZWriteEnable(true);
			graphics.SetAlphaBlendEnable(false);
			graphics.SetAlphaTestEnable(false);
			graphics.SetUniform(UniformName.texturefactor, new Color4(1f, 1f, 1f, 1f));

            //mxd. SKY PASS
            if (skygeo.Count > 0)
			{
				world = Matrix.Identity;
                graphics.SetUniform(UniformName.world, ref world);
				RenderSky(skygeo);
			}

			// SOLID PASS
			world = Matrix.Identity;
            graphics.SetUniform(UniformName.world, ref world);
            RenderSinglePass(solidgeo, solidthings, lightthings);

			//mxd. Render models, without backface culling
			if(maskedmodelthings.Count > 0)
			{
				graphics.SetAlphaTestEnable(true);
				graphics.SetCullMode(Cull.None);
				RenderModels(false, lightthings);
                graphics.SetCullMode(Cull.Clockwise);
			}

			// MASK PASS
			if(maskedgeo.Count > 0 || maskedthings.Count > 0)
			{
				world = Matrix.Identity;
                graphics.SetUniform(UniformName.world, ref world);
                graphics.SetAlphaTestEnable(true);
				RenderSinglePass(maskedgeo, maskedthings, lightthings);
			}

			// ALPHA AND ADDITIVE PASS
			if(translucentgeo.Count > 0 || translucentthings.Count > 0)
			{
				world = Matrix.Identity;
                graphics.SetUniform(UniformName.world, ref world);
                graphics.SetAlphaBlendEnable(true);
				graphics.SetAlphaTestEnable(false);
				graphics.SetZWriteEnable(false);
				graphics.SetSourceBlend(Blend.SourceAlpha);
				RenderTranslucentPass(translucentgeo, translucentthings, lightthings);
			}

			//mxd. Render translucent models, with backface culling
			if(translucentmodelthings.Count > 0)
			{
				graphics.SetAlphaBlendEnable(true);
				graphics.SetAlphaTestEnable(false);
				graphics.SetZWriteEnable(false);
				graphics.SetSourceBlend(Blend.SourceAlpha);
                RenderModels(true, lightthings);
            }

            // THING CAGES
            if (renderthingcages)
			{
				world = Matrix.Identity;
                graphics.SetUniform(UniformName.world, ref world);
                RenderThingCages();
			}

			//mxd. Visual vertices
			RenderVertices();

			// Slope handles
			if (General.Map.UDMF /* && General.Settings.ShowVisualSlopeHandles */)
				RenderSlopeHandles();

			//mxd. Event lines
			if (General.Settings.GZShowEventLines) RenderArrows(eventlines);
			
			// Remove references
			graphics.SetTexture(null);
			
			//mxd. Trash collections
			solidgeo = null;
			maskedgeo = null;
			translucentgeo = null;
			skygeo = null;

			solidthings = null;
			maskedthings = null;
			translucentthings = null;
			
			allthings = null;
			lightthings = null;
			maskedmodelthings = null;
			translucentmodelthings = null;

			visualvertices = null;

			if (General.Settings.ShowFPS)
			{
				fps++;
				if (fpsWatch.ElapsedMilliseconds > 1000)
				{
					fpsLabel.Text = string.Format("{0} FPS", fps);
					fps = 0;
					fpsWatch.Restart();
				}
			}
        }

        // [ZZ] black renderer magic here.
        //      todo maybe implement proper frustum culling eventually?
        //      Frustum2D.IntersectCircle doesn't seem to work here.
        private bool CullLight(VisualThing t)
        {
            Vector3D lightToCamera = (cameraposition - t.CenterV3D).GetNormal();
            double angdiff = Vector3D.DotProduct(lightToCamera, cameravector);
            if (angdiff <= 0)
                return true; // light in front of the camera. it's not negative because I don't want to calculate things twice and need the vector to point at camera.
            // otherwise check light size: large lights might have center on the back, but radius in front of the camera.
            Vector3D lightToCameraWithRadius = (cameraposition - (t.CenterV3D + lightToCamera * t.LightRadius)).GetNormal();
            double angdiffWithRadius = Vector3D.DotProduct(lightToCameraWithRadius, cameravector);
            if (angdiffWithRadius <= 0)
                return true; // light's radius extension is in front of the camera.
            return false;
        }

        //mxd
        private void UpdateLights()
		{
			// Calculate distance to camera
			foreach(VisualThing t in lightthings) t.CalculateCameraDistance(cameraposition);

			// Sort by it, closer ones first
			lightthings.Sort((t1, t2) => Math.Sign(t1.CameraDistance - t2.CameraDistance));

            // Gather the closest
            List<VisualThing> tl = new List<VisualThing>(lightthings.Count);
            // Break on either end of things of max dynamic lights reached
            for (int i = 0; i < lightthings.Count && tl.Count < General.Settings.GZMaxDynamicLights; i++)
            {
                // Make sure we can see this light at all
                if (!CullLight(lightthings[i]))
                    continue;
                tl.Add(lightthings[i]);
            }

            // Update the array
			lightthings = tl;

			// Sort things by light render style
			lightthings.Sort((t1, t2) => Math.Sign(t1.LightType.LightRenderStyle - t2.LightType.LightRenderStyle));
			lightOffsets = new int[4];

			foreach(VisualThing t in lightthings) 
			{
				//add light to apropriate array.
				switch(t.LightType.LightRenderStyle) 
				{
					case GZGeneral.LightRenderStyle.NORMAL:
					case GZGeneral.LightRenderStyle.VAVOOM: lightOffsets[0]++; break;
					case GZGeneral.LightRenderStyle.ADDITIVE: lightOffsets[2]++; break;
                    case GZGeneral.LightRenderStyle.SUBTRACTIVE: lightOffsets[3]++; break;
					default: lightOffsets[1]++; break; // attenuated
				}
			}
		}

		//mxd.
		//I never particularly liked old ThingCages, so I wrote this instead.
		//It should render faster and it has fancy arrow! :)
		private void RenderThingCages() 
		{
			graphics.SetAlphaBlendEnable(true);
			graphics.SetAlphaTestEnable(false);
			graphics.SetZWriteEnable(false);
			graphics.SetSourceBlend(Blend.SourceAlpha);
			graphics.SetDestinationBlend(Blend.SourceAlpha);

			graphics.SetShader(ShaderName.world3d_constant_color);

			foreach(VisualThing t in allthings)
			{
				// Setup color
				Color4 thingcolor;
				if(t.Selected && showselection) 
				{
					thingcolor = General.Colors.Selection3D.ToColorValue();
				} 
				else
				{
					thingcolor = t.CageColor;
					if(t != highlighted) thingcolor.Alpha = 0.6f;
				}
				graphics.SetUniform(UniformName.vertexColor, thingcolor);

				//Render cage
				graphics.SetVertexBuffer(t.CageBuffer);
				graphics.Draw(PrimitiveType.LineList, 0, t.CageLength);
			}

			// Done
			graphics.SetUniform(UniformName.texturefactor, new Color4(1f, 1f, 1f, 1f));
        }

		//mxd
		private void RenderVertices() 
		{
			if(visualvertices == null) return;

			graphics.SetAlphaBlendEnable(true);
			graphics.SetAlphaTestEnable(false);
			graphics.SetZWriteEnable(false);
			graphics.SetSourceBlend(Blend.SourceAlpha);
			graphics.SetDestinationBlend(Blend.SourceAlpha);

            graphics.SetShader(ShaderName.world3d_constant_color);

			foreach(VisualVertex v in visualvertices) 
			{
				world = v.Position;
                graphics.SetUniform(UniformName.world, ref world);

                // Setup color
                Color4 color;
				if(v.Selected && showselection) 
				{
					color = General.Colors.Selection3D.ToColorValue();
				} 
				else 
				{
					color = v.HaveHeightOffset ? General.Colors.InfoLine.ToColorValue() : General.Colors.Vertices.ToColorValue();
					if(v != highlighted) color.Alpha = 0.6f;
				}
                graphics.SetUniform(UniformName.vertexColor, color);

				//Commence drawing!!11
				graphics.SetVertexBuffer(v.CeilingVertex ? vertexhandle.Upper : vertexhandle.Lower);
				graphics.Draw(PrimitiveType.LineList, 0, 8);
			}

			// Done
			graphics.SetUniform(UniformName.texturefactor, new Color4(1f, 1f, 1f, 1f));
        }

		private void RenderSlopeHandles()
		{
			if (visualslopehandles == null || !showselection) return;

			graphics.SetAlphaBlendEnable(true);
			graphics.SetAlphaTestEnable(false);
			graphics.SetZWriteEnable(false);
			graphics.SetSourceBlend(Blend.SourceAlpha);
			graphics.SetDestinationBlend(Blend.InverseSourceAlpha);

			graphics.SetShader(ShaderName.world3d_slope_handle);

			foreach (VisualSlope handle in visualslopehandles)
			{
				PixelColor color = General.Colors.Vertices;

				if (handle.Pivot)
					color = General.Colors.Guideline;
				else if (handle.Selected)
					color = General.Colors.Selection3D;
				else if (handle == highlighted)
					color = General.Colors.Highlight3D;
				else if (handle.SmartPivot)
					color = General.Colors.Vertices;

				world = handle.Position;
				graphics.SetUniform(UniformName.world, ref world);
				graphics.SetUniform(UniformName.vertexColor, color.ToColorValue());

				if (handle.Type == VisualSlopeType.Line)
				{
					graphics.SetUniform(UniformName.slopeHandleLength, (float)handle.Length);
					graphics.SetVertexBuffer(visualslopehandle.LineGeometry);
					graphics.Draw(PrimitiveType.TriangleList, 0, 2);
				}
				else if (handle.Type == VisualSlopeType.Vertex)
				{
					graphics.SetUniform(UniformName.slopeHandleLength, 1.0f);
					graphics.SetVertexBuffer(visualslopehandle.VertexGeometry);
					graphics.Draw(PrimitiveType.TriangleList, 0, 1);
				}
			}

			// Done
			graphics.SetUniform(UniformName.texturefactor, new Color4(1f, 1f, 1f, 1f));
		}

		//mxd
		private void RenderArrows(ICollection<Line3D> lines) 
		{
			// Calculate required points count
			if(lines.Count == 0) return;
			int pointscount = 0;
			foreach(Line3D line in lines) pointscount += (line.RenderArrowhead ? 6 : 2); // 4 extra points for the arrowhead
			if(pointscount < 2) return;
			
			//create vertices
			WorldVertex[] verts = new WorldVertex[pointscount];
			const float scaler = 20f;
			pointscount = 0;

			foreach(Line3D line in lines)
			{
				int color = line.Color.ToInt();

				// Add regular points
				verts[pointscount].x = (float)line.Start.x;
				verts[pointscount].y = (float)line.Start.y;
				verts[pointscount].z = (float)line.Start.z;
				verts[pointscount].c = color;
				pointscount++;

				verts[pointscount].x = (float)line.End.x;
				verts[pointscount].y = (float)line.End.y;
				verts[pointscount].z = (float)line.End.z;
				verts[pointscount].c = color;
				pointscount++;

				// Add arrowhead
				if(line.RenderArrowhead)
				{
					double nz = line.GetDelta().GetNormal().z * scaler;
					double angle = line.GetAngle();
					Vector3D a1 = new Vector3D(line.End.x - scaler * Math.Sin(angle - 0.46f), line.End.y + scaler * Math.Cos(angle - 0.46f), line.End.z - nz);
					Vector3D a2 = new Vector3D(line.End.x - scaler * Math.Sin(angle + 0.46f), line.End.y + scaler * Math.Cos(angle + 0.46f), line.End.z - nz);

					verts[pointscount] = verts[pointscount - 1];
					verts[pointscount + 1].x = (float)a1.x;
					verts[pointscount + 1].y = (float)a1.y;
					verts[pointscount + 1].z = (float)a1.z;
					verts[pointscount + 1].c = color;

					verts[pointscount + 2] = verts[pointscount - 1];
					verts[pointscount + 3].x = (float)a2.x;
					verts[pointscount + 3].y = (float)a2.y;
					verts[pointscount + 3].z = (float)a2.z;
					verts[pointscount + 3].c = color;

					pointscount += 4;
				}
			}

			VertexBuffer vb = new VertexBuffer();
			graphics.SetBufferData(vb, verts);
			
			//begin rendering
			graphics.SetAlphaBlendEnable(true);
			graphics.SetAlphaTestEnable(false);
			graphics.SetZWriteEnable(false);
			graphics.SetSourceBlend(Blend.SourceAlpha);
			graphics.SetDestinationBlend(Blend.SourceAlpha);

            graphics.SetShader(ShaderName.world3d_vertex_color);

			world = Matrix.Identity;
            graphics.SetUniform(UniformName.world, ref world);

            //render
            graphics.SetVertexBuffer(vb);
			graphics.Draw(PrimitiveType.LineList, 0, pointscount / 2);

			// Done
			graphics.SetUniform(UniformName.texturefactor, new Color4(1f, 1f, 1f, 1f));
            vb.Dispose();
		}

		// This performs a single render pass
		private void RenderSinglePass(Dictionary<ImageData, List<VisualGeometry>> geopass, Dictionary<ImageData, List<VisualThing>> thingspass, List<VisualThing> lights)
		{
			ImageData curtexture;
			ShaderName currentshaderpass = shaderpass;
			ShaderName highshaderpass = (ShaderName)(shaderpass + 2);

            // Begin rendering with this shader
            graphics.SetShader(shaderpass);

            // Light data arrays
            Vector4f[] lightColor = new Vector4f[MAX_DYNLIGHTS_PER_SURFACE];
            Vector4f[] lightPosAndRadius = new Vector4f[MAX_DYNLIGHTS_PER_SURFACE];
            Vector4f[] lightOrientation = new Vector4f[MAX_DYNLIGHTS_PER_SURFACE];
            Vector2f[] light2Radius = new Vector2f[MAX_DYNLIGHTS_PER_SURFACE];

            graphics.SetUniform(UniformName.lightsEnabled, lights.Count > 0);
            graphics.SetUniform(UniformName.ignoreNormals, false);

            bool hadlights = false;

            // volte: Set textures for classic lighting
            if (classicLightingColorMap != null)
            {
	            graphics.SetUniform(UniformName.colormapSize, new Vector2i(classicLightingColorMapTex.Width, classicLightingColorMapTex.Height));
	            graphics.SetTexture(classicLightingColorMapTex, 1);
	            graphics.SetSamplerFilter(TextureFilter.Nearest, TextureFilter.Nearest, MipmapFilter.None, 0, 1);
	            graphics.SetSamplerState(TextureAddress.Clamp, 1);
            }

            // Render the geometry collected
            foreach (KeyValuePair<ImageData, List<VisualGeometry>> group in geopass)
			{
				curtexture = group.Key;

        // Apply texture
        Texture texture = UseIndexedTexture ? curtexture.IndexedTexture : curtexture.Texture;
        graphics.SetTexture(texture);
        graphics.SetUniform(UniformName.drawPaletted, texture.UserData == ImageData.TEXTURE_INDEXED);

				//mxd. Sort geometry by sector index
				group.Value.Sort((g1, g2) => g1.Sector.Sector.FixedIndex - g2.Sector.Sector.FixedIndex);

				// Go for all geometry that uses this texture
				VisualSector sector = null;
				
				foreach(VisualGeometry g in group.Value)
				{

                    int lightIndex = 0;

                    foreach (VisualThing light in lights)
                    {
                        if (BoundingBoxesIntersect(g.BoundingBox, light.BoundingBox))
                        {

                            //t.LightType.LightRenderStyle
                            //Vector4f lightColor, lightPosAndRadius, lightOrientation, light2Radius;

                            lightColor[lightIndex] = light.LightColor.ToVector();
                            lightPosAndRadius[lightIndex] = new Vector4f(light.Center, light.LightRadius);

                            // set type of light
                            if (light.LightType.LightType == GZGeneral.LightType.SPOT)
                            {
                                lightOrientation[lightIndex] = new Vector4f(light.VectorLookAt, 1f);
                                light2Radius[lightIndex] = new Vector2f(CosDeg(light.LightSpotRadius1), CosDeg(light.LightSpotRadius2));
                            }
                            else lightOrientation[lightIndex].W = 0f;

                            lightIndex++;

                            if (lightIndex >= lightColor.Length)
                                break;

                        }
                    }

                    bool havelights = (lightIndex > 0);

                    for (int i = lightIndex; i < lightColor.Length; i++)
                        lightColor[i].W = 0;

                    if (havelights != hadlights || havelights)
                    {
                        graphics.SetUniform(UniformName.lightColor, lightColor);
                        if (havelights)
                        {
                            graphics.SetUniform(UniformName.lightPosAndRadius, lightPosAndRadius);
                            graphics.SetUniform(UniformName.lightOrientation, lightOrientation);
                            graphics.SetUniform(UniformName.light2Radius, light2Radius);
                        }
                    }

                    hadlights = havelights;

					// Changing sector?
					if(!object.ReferenceEquals(g.Sector, sector))
					{
						// Update the sector if needed
						if(g.Sector.NeedsUpdateGeo) g.Sector.Update(graphics);

						// Only do this sector when a vertexbuffer is created
						//mxd. No Map means that sector was deleted recently, I suppose
						if(g.Sector.GeometryBuffer != null && g.Sector.Sector.Map != null) 
						{
							// Change current sector
							sector = g.Sector;

							// Set stream source
							graphics.SetVertexBuffer(sector.GeometryBuffer);
						}
						else
						{
							sector = null;
						}
					}

                    graphics.SetUniform(UniformName.desaturation, 0.0f);
                    if (sector != null) 
					{
						// Determine the shader pass we want to use for this object
						ShaderName wantedshaderpass = (((g == highlighted) && showhighlight) || (g.Selected && showselection)) ? highshaderpass : shaderpass;

						//mxd. Render fog?
						if(General.Settings.GZDrawFog && !fullbrightness && !General.Settings.ClassicRendering && sector.Sector.FogMode != SectorFogMode.NONE)
							wantedshaderpass += 8;

						// Switch shader pass?
						if(currentshaderpass != wantedshaderpass)
						{
							graphics.SetShader(wantedshaderpass);
							currentshaderpass = wantedshaderpass;

							//mxd. Set variables for fog rendering?
							if(wantedshaderpass > ShaderName.world3d_p7)
							{
                                graphics.SetUniform(UniformName.modelnormal, Matrix.Identity);
                            }
						}
						
						// volte: set sector light level for classic rendering mode
						graphics.SetUniform(UniformName.sectorLightLevel, sector.Sector.Brightness);

						//mxd. Set variables for fog rendering?
						if(wantedshaderpass > ShaderName.world3d_p7)
						{
							graphics.SetUniform(UniformName.campos, new Vector4f((float)cameraposition.x, (float)cameraposition.y, (float)cameraposition.z, g.FogFactor));
							graphics.SetUniform(UniformName.sectorfogcolor, sector.Sector.FogColor);
						}
                        
						// Set the colors to use
						graphics.SetUniform(UniformName.highlightcolor, CalculateHighlightColor((g == highlighted) && showhighlight, (g.Selected && showselection)));

                        // [ZZ] include desaturation factor
                        graphics.SetUniform(UniformName.desaturation, (float)sector.Sector.Desaturation);

						// Render!
						graphics.Draw(PrimitiveType.TriangleList, g.VertexOffset, g.Triangles);
					}
				}
			}

            graphics.SetUniform(UniformName.lightsEnabled, false);

            // Get things for this pass
            if (thingspass.Count > 0)
			{
				// Texture addressing
				graphics.SetSamplerState(TextureAddress.Clamp);
				graphics.SetCullMode(Cull.None); //mxd. Disable backside culling, because otherwise sprites with positive ScaleY and negative ScaleX will be facing away from the camera...

				Color4 vertexcolor = new Color4(); //mxd

				// Render things collected
				foreach(KeyValuePair<ImageData, List<VisualThing>> group in thingspass)
				{
					if(group.Key is UnknownImage) continue;
					
					curtexture = group.Key;

          // Apply texture
          Texture texture = UseIndexedTexture ? curtexture.IndexedTexture : curtexture.Texture;
          graphics.SetTexture(texture);
          graphics.SetUniform(UniformName.drawPaletted, texture.UserData == ImageData.TEXTURE_INDEXED);

					// Render all things with this texture
					foreach(VisualThing t in group.Value)
					{
						// Update buffer if needed
						t.Update();

						//mxd. Check 3D distance. Check against MaxValue to save doing GetLenthSq if there's not DistanceCheck defined
						if (t.Info.DistanceCheckSq < double.MaxValue)
						{
							t.CalculateCameraDistance(cameraposition);

							if (t.CameraDistance > t.Info.DistanceCheckSq)
								continue;
						}

						// Only do this sector when a vertexbuffer is created
						if (t.GeometryBuffer != null) 
						{
							// Determine the shader pass we want to use for this object
							ShaderName wantedshaderpass = (((t == highlighted) && showhighlight) || (t.Selected && showselection)) ? highshaderpass : shaderpass;

							//mxd. If fog is enagled, switch to shader, which calculates it
							if(General.Settings.GZDrawFog && !fullbrightness && !General.Settings.ClassicRendering && t.Thing.Sector != null && t.Thing.Sector.FogMode != SectorFogMode.NONE)
								wantedshaderpass += 8;

							//mxd. Create the matrix for positioning 
							world = CreateThingPositionMatrix(t);

							//mxd. If current thing is light - set it's color to light color
							if(t.LightType != null && t.LightType.LightInternal && !fullbrightness && !General.Settings.ClassicRendering) 
							{
								wantedshaderpass += 4; // Render using one of passes, which uses World3D.VertexColor
								vertexcolor = t.LightColor;
							}
							//mxd. Check if Thing is affected by dynamic lights and set color accordingly
							else if(General.Settings.GZDrawLightsMode != LightRenderMode.NONE && !fullbrightness && !General.Settings.ClassicRendering && lightthings.Count > 0)
							{
								Color4 litcolor = GetLitColorForThing(t);
								if(litcolor.ToArgb() != 0)
								{
									wantedshaderpass += 4; // Render using one of passes, which uses World3D.VertexColor
									vertexcolor = new Color4(t.VertexColor) + litcolor;
								}
							}
							else
							{
								vertexcolor = new Color4();
							}

							// Switch shader pass?
							if(currentshaderpass != wantedshaderpass) 
							{
								graphics.SetShader(wantedshaderpass);
								currentshaderpass = wantedshaderpass;
							}

							//mxd. Set variables for fog rendering?
							if(wantedshaderpass > ShaderName.world3d_p7)
							{
                                graphics.SetUniform(UniformName.modelnormal, Matrix.Identity);
                                graphics.SetUniform(UniformName.campos, new Vector4f((float)cameraposition.x, (float)cameraposition.y, (float)cameraposition.z, t.FogFactor));
							}

							// Set the colors to use
							if (t.Thing.Sector != null)
							{
								graphics.SetUniform(UniformName.sectorLightLevel, t.Thing.Sector.Brightness);
								graphics.SetUniform(UniformName.sectorfogcolor, t.Thing.Sector.FogColor);
							}
                            graphics.SetUniform(UniformName.vertexColor, vertexcolor);
                            graphics.SetUniform(UniformName.highlightcolor, CalculateHighlightColor((t == highlighted) && showhighlight, (t.Selected && showselection)));

                            // [ZZ] check if we want stencil
                            graphics.SetUniform(UniformName.stencilColor, t.StencilColor.ToColorValue());

                            // [ZZ] apply desaturation
                            if (t.Thing.Sector != null)
                                graphics.SetUniform(UniformName.desaturation, (float)t.Thing.Sector.Desaturation);
                            else graphics.SetUniform(UniformName.desaturation, 0.0f);

                            // Apply changes
                            graphics.SetUniform(UniformName.world, world);

                            // Apply buffer
                            graphics.SetVertexBuffer(t.GeometryBuffer);

							// Render!
							graphics.Draw(PrimitiveType.TriangleList, 0, t.Triangles);
                        }
					}

                    // [ZZ]
                    graphics.SetUniform(UniformName.stencilColor, new Color4(1f, 1f, 1f, 0f));
                }

                // Texture addressing
                graphics.SetSamplerState(TextureAddress.Wrap);
				graphics.SetCullMode(Cull.Clockwise); //mxd
			}
		}

		//mxd
		private void RenderTranslucentPass(List<VisualGeometry> geopass, List<VisualThing> thingspass, List<VisualThing> lights)
		{
			ShaderName currentshaderpass = shaderpass;
			ShaderName highshaderpass = (ShaderName)(shaderpass + 2);

			// Sort geometry by camera distance. First vertex of the BoundingBox is it's center
            geopass.Sort(delegate(VisualGeometry vg1, VisualGeometry vg2)
			{

                if (vg1 == vg2)
                    return 0;

                double dist1, dist2;
                Vector3D cameraPos = General.Map.VisualCamera.Position;
                Vector2D cameraPos2 = new Vector2D(cameraPos);

                // if one of the things being compared is a plane, use easier formula. (3d floor compatibility)
                if (vg1.GeometryType == VisualGeometryType.FLOOR || vg1.GeometryType == VisualGeometryType.CEILING ||
                    vg2.GeometryType == VisualGeometryType.FLOOR || vg2.GeometryType == VisualGeometryType.CEILING)
                {
                    // more magic
                    dist1 = Math.Abs(vg1.BoundingBox[0].z - cameraPos.z);
                    dist2 = Math.Abs(vg2.BoundingBox[0].z - cameraPos.z);
                }
                else
                {
                    dist1 = (General.Map.VisualCamera.Position - vg1.BoundingBox[0]).GetLengthSq();
                    dist2 = (General.Map.VisualCamera.Position - vg2.BoundingBox[0]).GetLengthSq();
                }

                return (int)(dist2 - dist1);

			});

			ImageData curtexture;
			VisualSector sector = null;
			RenderPass currentpass = RenderPass.Solid;
			long curtexturename = 0;
			float fogfactor = -1;

			// Begin rendering with this shader
			graphics.SetShader(shaderpass);


            // Light data arrays
            Vector4f[] lightColor = new Vector4f[MAX_DYNLIGHTS_PER_SURFACE];
            Vector4f[] lightPosAndRadius = new Vector4f[MAX_DYNLIGHTS_PER_SURFACE];
            Vector4f[] lightOrientation = new Vector4f[MAX_DYNLIGHTS_PER_SURFACE];
            Vector2f[] light2Radius = new Vector2f[MAX_DYNLIGHTS_PER_SURFACE];

            graphics.SetUniform(UniformName.lightsEnabled, lights.Count > 0);
            graphics.SetUniform(UniformName.ignoreNormals, false);

            bool hadlights = false;

            // Go for all geometry
            foreach (VisualGeometry g in geopass)
			{

                int lightIndex = 0;

                foreach (VisualThing light in lights)
                {
                    if (BoundingBoxesIntersect(g.BoundingBox, light.BoundingBox))
                    {

                        //t.LightType.LightRenderStyle
                        //Vector4f lightColor, lightPosAndRadius, lightOrientation, light2Radius;

                        lightColor[lightIndex] = light.LightColor.ToVector();
                        lightPosAndRadius[lightIndex] = new Vector4f(light.Center, light.LightRadius);

                        // set type of light
                        if (light.LightType.LightType == GZGeneral.LightType.SPOT)
                        {
                            lightOrientation[lightIndex] = new Vector4f(light.VectorLookAt, 1f);
                            light2Radius[lightIndex] = new Vector2f(CosDeg(light.LightSpotRadius1), CosDeg(light.LightSpotRadius2));
                        }
                        else lightOrientation[lightIndex].W = 0f;

                        lightIndex++;

                        if (lightIndex >= lightColor.Length)
                            break;

                    }
                }

                bool havelights = (lightIndex > 0);

                for (int i = lightIndex; i < lightColor.Length; i++)
                    lightColor[i].W = 0;

                if (havelights != hadlights || havelights)
                {
                    graphics.SetUniform(UniformName.lightColor, lightColor);
                    if (havelights)
                    {
                        graphics.SetUniform(UniformName.lightPosAndRadius, lightPosAndRadius);
                        graphics.SetUniform(UniformName.lightOrientation, lightOrientation);
                        graphics.SetUniform(UniformName.light2Radius, light2Radius);
                    }
                }

                hadlights = havelights;

                // Change blend mode?
                if (g.RenderPass != currentpass)
				{
					switch(g.RenderPass)
					{
						case RenderPass.Additive:
							graphics.SetDestinationBlend(Blend.One);
							break;

						case RenderPass.Alpha:
							graphics.SetDestinationBlend(Blend.InverseSourceAlpha);
							break;
					}

					currentpass = g.RenderPass;
				}

				// Change texture?
				if(g.Texture.LongName != curtexturename)
				{
					curtexture = g.Texture;

					// Apply texture
					Texture texture = UseIndexedTexture ? curtexture.IndexedTexture : curtexture.Texture;
					graphics.SetTexture(texture);
					graphics.SetUniform(UniformName.drawPaletted, texture.UserData == ImageData.TEXTURE_INDEXED);
					
					curtexturename = g.Texture.LongName;
				}

				// Changing sector?
				if(!object.ReferenceEquals(g.Sector, sector))
				{
					// Update the sector if needed
					if(g.Sector.NeedsUpdateGeo) g.Sector.Update(graphics);

					// Only do this sector when a vertexbuffer is created
					//mxd. No Map means that sector was deleted recently, I suppose
					if(g.Sector.GeometryBuffer != null && g.Sector.Sector.Map != null)
					{
						// Change current sector
						sector = g.Sector;

						// Set stream source
						graphics.SetVertexBuffer(sector.GeometryBuffer);
					}
					else
					{
						sector = null;
					}
				}

                if (sector != null)
                {
                    // Determine the shader pass we want to use for this object
                    ShaderName wantedshaderpass = (((g == highlighted) && showhighlight) || (g.Selected && showselection)) ? highshaderpass : shaderpass;

                    //mxd. Render fog?
                    if (General.Settings.GZDrawFog && !fullbrightness && !General.Settings.ClassicRendering && sector.Sector.FogMode != SectorFogMode.NONE)
                        wantedshaderpass += 8;

                    // Switch shader pass?
                    if (currentshaderpass != wantedshaderpass)
                    {
                        graphics.SetShader(wantedshaderpass);
                        currentshaderpass = wantedshaderpass;

                        //mxd. Set variables for fog rendering?
                        if (wantedshaderpass > ShaderName.world3d_p7)
                        {
                            graphics.SetUniform(UniformName.modelnormal, Matrix.Identity);
                        }
                    }

                    // Set variables for fog rendering?
                    if (wantedshaderpass > ShaderName.world3d_p7 && g.FogFactor != fogfactor)
                    {
                        graphics.SetUniform(UniformName.campos, new Vector4f((float)cameraposition.x, (float)cameraposition.y, (float)cameraposition.z, g.FogFactor));
                        fogfactor = g.FogFactor;
                    }

                    //
                    graphics.SetUniform(UniformName.desaturation, (float)sector.Sector.Desaturation);

                    // Set the colors to use
                    graphics.SetUniform(UniformName.sectorfogcolor, sector.Sector.FogColor);
                    graphics.SetUniform(UniformName.highlightcolor, CalculateHighlightColor((g == highlighted) && showhighlight, (g.Selected && showselection)));

                    // Render!
                    graphics.Draw(PrimitiveType.TriangleList, g.VertexOffset, g.Triangles);
                }
                else graphics.SetUniform(UniformName.desaturation, 0.0f);
            }

            graphics.SetUniform(UniformName.lightsEnabled, false);

            // Get things for this pass
            if (thingspass.Count > 0)
			{
				// Texture addressing
				graphics.SetSamplerState(TextureAddress.Clamp);
				graphics.SetCullMode(Cull.None); //mxd. Disable backside culling, because otherwise sprites with positive ScaleY and negative ScaleX will be facing away from the camera...

				// Sort geometry by camera distance. First vertex of the BoundingBox is it's center
				thingspass.Sort(delegate(VisualThing vt1, VisualThing vt2)
				{
					if(vt1 == vt2) return 0;
					return (int)((General.Map.VisualCamera.Position - vt2.BoundingBox[0]).GetLengthSq()
							   - (General.Map.VisualCamera.Position - vt1.BoundingBox[0]).GetLengthSq());
				});

				// Reset vars
				currentpass = RenderPass.Solid;
				curtexturename = 0;
				Color4 vertexcolor = new Color4();
				fogfactor = -1;

				// Render things collected
				foreach(VisualThing t in thingspass)
				{
					// Update buffer if needed
					t.Update();

					//mxd. Check 3D distance. Check against MaxValue to save doing GetLenthSq if there's not DistanceCheck defined
					if (t.Info.DistanceCheckSq < double.MaxValue)
					{
						t.CalculateCameraDistance(cameraposition);

						if (t.CameraDistance > t.Info.DistanceCheckSq)
							continue;
					}

					t.UpdateSpriteFrame(); // Set correct texture, geobuffer and triangles count
					if(t.Texture is UnknownImage) continue;
					
					// Change blend mode?
					if(t.RenderPass != currentpass)
					{
						switch(t.RenderPass)
						{
							case RenderPass.Additive:
								graphics.SetDestinationBlend(Blend.One);
								break;

							case RenderPass.Alpha:
								graphics.SetDestinationBlend(Blend.InverseSourceAlpha);
								break;
						}

						currentpass = t.RenderPass;
					}

					// Change texture?
					if(t.Texture.LongName != curtexturename)
					{
						curtexture = t.Texture;

						// Apply texture
						Texture texture = UseIndexedTexture ? curtexture.IndexedTexture : curtexture.Texture;
						graphics.SetTexture(texture);
						graphics.SetUniform(UniformName.drawPaletted, texture.UserData == ImageData.TEXTURE_INDEXED);
						
						curtexturename = t.Texture.LongName;
					}

					// Only do this sector when a vertexbuffer is created
					if(t.GeometryBuffer != null)
					{
						// Determine the shader pass we want to use for this object
						ShaderName wantedshaderpass = (((t == highlighted) && showhighlight) || (t.Selected && showselection)) ? highshaderpass : shaderpass;

						//mxd. if fog is enagled, switch to shader, which calculates it
						if(General.Settings.GZDrawFog && !fullbrightness && !General.Settings.ClassicRendering && t.Thing.Sector != null && t.Thing.Sector.FogMode != SectorFogMode.NONE)
							wantedshaderpass += 8;

						//mxd. Create the matrix for positioning 
						world = CreateThingPositionMatrix(t);

						//mxd. If current thing is light - set it's color to light color
						if(t.LightType != null && t.LightType.LightInternal && !fullbrightness && !General.Settings.ClassicRendering)
						{
							wantedshaderpass += 4; // Render using one of passes, which uses World3D.VertexColor
							vertexcolor = t.LightColor;
						}
						//mxd. Check if Thing is affected by dynamic lights and set color accordingly
						else if(General.Settings.GZDrawLightsMode != LightRenderMode.NONE && !fullbrightness && !General.Settings.ClassicRendering && lightthings.Count > 0)
						{
							Color4 litcolor = GetLitColorForThing(t);
							if(litcolor.ToArgb() != 0)
							{
								wantedshaderpass += 4; // Render using one of passes, which uses World3D.VertexColor
								vertexcolor = new Color4(t.VertexColor) + litcolor;
							}
						}
						else
						{
							vertexcolor = new Color4();
						}

						// Switch shader pass?
						if(currentshaderpass != wantedshaderpass)
						{
							graphics.SetShader(wantedshaderpass);
							currentshaderpass = wantedshaderpass;
						}

						//mxd. Set variables for fog rendering?
						if(wantedshaderpass > ShaderName.world3d_p7)
						{
                            graphics.SetUniform(UniformName.modelnormal, Matrix.Identity);
                            if (t.FogFactor != fogfactor)
							{
                                graphics.SetUniform(UniformName.campos, new Vector4f((float)cameraposition.x, (float)cameraposition.y, (float)cameraposition.z, t.FogFactor));
								fogfactor = t.FogFactor;
							}
						}

                        // Set the colors to use
                        graphics.SetUniform(UniformName.sectorfogcolor, t.Thing.Sector.FogColor);
                        graphics.SetUniform(UniformName.vertexColor, vertexcolor);
                        graphics.SetUniform(UniformName.highlightcolor, CalculateHighlightColor((t == highlighted) && showhighlight, (t.Selected && showselection)));

                        // [ZZ] check if we want stencil
                        graphics.SetUniform(UniformName.stencilColor, t.StencilColor.ToColorValue());

                        //
                        graphics.SetUniform(UniformName.desaturation, (float)t.Thing.Sector.Desaturation);

                        // Apply changes
                        graphics.SetUniform(UniformName.world, world);

                        // Apply buffer
                        graphics.SetVertexBuffer(t.GeometryBuffer);

						// Render!
						graphics.Draw(PrimitiveType.TriangleList, 0, t.Triangles);
					}
				}

                // [ZZ] check if we want stencil
                graphics.SetUniform(UniformName.stencilColor, new Color4(1f, 1f, 1f, 0f));

                // Texture addressing
                graphics.SetSamplerState(TextureAddress.Wrap);
				graphics.SetCullMode(Cull.Clockwise); //mxd
			}
		}

		//mxd
		private Matrix CreateThingPositionMatrix(VisualThing t)
		{
			// Use normal ThingRenderMode when model rendering is disabled for this thing
			ThingRenderMode rendermode = t.Thing.RenderMode;
			if((t.Thing.RenderMode == ThingRenderMode.MODEL || t.Thing.RenderMode == ThingRenderMode.VOXEL) &&
			   (General.Settings.GZDrawModelsMode == ModelRenderMode.NONE ||
			   (General.Settings.GZDrawModelsMode == ModelRenderMode.SELECTION && !t.Selected)))
			{
				rendermode = ThingRenderMode.NORMAL;
			}
			
			// Create the matrix for positioning
			switch(rendermode)
			{
				case ThingRenderMode.NORMAL:
					if(t.Info.XYBillboard) // Apply billboarding?
					{
						return Matrix.Translation(0f, 0f, -t.LocalCenterZ)
							* Matrix.RotationX((float)(Angle2D.PI - General.Map.VisualCamera.AngleZ))
							* Matrix.Translation(0f, 0f, t.LocalCenterZ)
							* billboard
							* t.Position;
					}
					return billboard * t.Position;

				case ThingRenderMode.FLATSPRITE:
				case ThingRenderMode.WALLSPRITE:
				case ThingRenderMode.MODEL:
				case ThingRenderMode.VOXEL:
					return t.Position;

				default: throw new NotImplementedException("Unknown ThingRenderMode");
			}
		}

        private float CosDeg(float angle)
        {
            return (float)Math.Cos(Angle2D.DegToRad(angle));
        }

        //mxd. Render models
        private void RenderModels(bool trans, List<VisualThing> lights) 
		{
			ShaderName shaderpass = (fullbrightness ? ShaderName.world3d_fullbright : ShaderName.world3d_main_vertexcolor);
			ShaderName currentshaderpass = shaderpass;
			ShaderName highshaderpass = (ShaderName)(shaderpass + 2);

            RenderPass currentpass = RenderPass.Solid;

            // Begin rendering with this shader
            graphics.SetShader(currentshaderpass);

            // Light data arrays
            Vector4f[] lightColor = new Vector4f[MAX_DYNLIGHTS_PER_SURFACE];
            Vector4f[] lightPosAndRadius = new Vector4f[MAX_DYNLIGHTS_PER_SURFACE];
            Vector4f[] lightOrientation = new Vector4f[MAX_DYNLIGHTS_PER_SURFACE];
            Vector2f[] light2Radius = new Vector2f[MAX_DYNLIGHTS_PER_SURFACE];

            graphics.SetUniform(UniformName.lightsEnabled, lights.Count > 0);
            graphics.SetUniform(UniformName.ignoreNormals, true);

            bool hadlights = false;

            List<VisualThing> things;
            if (trans)
            {
                // Sort models by camera distance. First vertex of the BoundingBox is it's center
                translucentmodelthings.Sort((vt1, vt2) => (int)((General.Map.VisualCamera.Position - vt2.BoundingBox[0]).GetLengthSq()
                                              - (General.Map.VisualCamera.Position - vt1.BoundingBox[0]).GetLengthSq()));
                things = translucentmodelthings;
            }
            else
            {
                things = new List<VisualThing>();
                foreach (KeyValuePair<ModelData, List<VisualThing>> group in maskedmodelthings)
                    foreach (VisualThing t in group.Value)
                        things.Add(t);
            }
            
			foreach(VisualThing t in things) 
			{

                if (trans)
                {
                    // Change blend mode?
                    if (t.RenderPass != currentpass)
                    {
                        switch (t.RenderPass)
                        {
                            case RenderPass.Additive:
                                graphics.SetDestinationBlend(Blend.One);
                                break;

                            case RenderPass.Alpha:
                                graphics.SetDestinationBlend(Blend.InverseSourceAlpha);
                                break;
                        }

                        currentpass = t.RenderPass;
                    }
                }

				// Update buffer if needed
				t.Update();

				// Check 3D distance. Check against MaxValue to save doing GetLenthSq if there's not DistanceCheck defined
				if (t.Info.DistanceCheckSq < double.MaxValue)
				{
					t.CalculateCameraDistance(cameraposition);

					if (t.CameraDistance > t.Info.DistanceCheckSq)
						continue;
				}
					
				Color4 vertexcolor = new Color4(t.VertexColor);

                // Check if model is affected by dynamic lights and set color accordingly
                graphics.SetUniform(UniformName.vertexColor, vertexcolor);

				// Determine the shader pass we want to use for this object
				ShaderName wantedshaderpass = ((((t == highlighted) && showhighlight) || (t.Selected && showselection)) ? highshaderpass : shaderpass);

				// If fog is enagled, switch to shader, which calculates it
				if (General.Settings.GZDrawFog && !fullbrightness && !General.Settings.ClassicRendering && t.Thing.Sector != null && t.Thing.Sector.FogMode != SectorFogMode.NONE)
					wantedshaderpass += 8;

				// Switch shader pass?
				if (currentshaderpass != wantedshaderpass)
				{
					graphics.SetShader(wantedshaderpass);
					currentshaderpass = wantedshaderpass;
				}

                // Set the colors to use
                graphics.SetUniform(UniformName.highlightcolor, CalculateHighlightColor((t == highlighted) && showhighlight, (t.Selected && showselection)));

				// Create the matrix for positioning / rotation
				double sx = t.Thing.ScaleX * t.Thing.ActorScale.Width;
				double sy = t.Thing.ScaleY * t.Thing.ActorScale.Height;
                
				Matrix modelscale = Matrix.Scaling((float)sx, (float)sx, (float)sy);
				Matrix modelrotation = Matrix.RotationY((float)-t.Thing.RollRad) * Matrix.RotationX((float)-t.Thing.PitchRad) * Matrix.RotationZ((float)t.Thing.Angle);

				world = General.Map.Data.ModeldefEntries[t.Thing.Type].Transform * modelscale * modelrotation * t.Position;
                graphics.SetUniform(UniformName.world, world);

                // Set variables for fog rendering
                if (wantedshaderpass > ShaderName.world3d_p7)
				{
                    // this is not right...
                    graphics.SetUniform(UniformName.modelnormal, General.Map.Data.ModeldefEntries[t.Thing.Type].TransformRotation * modelrotation);
                    if (t.Thing.Sector != null) graphics.SetUniform(UniformName.sectorfogcolor, t.Thing.Sector.FogColor);
                    graphics.SetUniform(UniformName.campos, new Vector4f((float)cameraposition.x, (float)cameraposition.y, (float)cameraposition.z, t.FogFactor));
				}

                if (t.Thing.Sector != null)
                    graphics.SetUniform(UniformName.desaturation, (float)t.Thing.Sector.Desaturation);
                else graphics.SetUniform(UniformName.desaturation, 0.0f);

                int lightIndex = 0;

                foreach (VisualThing light in lights)
                {
                    if (BoundingBoxesIntersect(t.BoundingBox, light.BoundingBox))
                    {

                        //t.LightType.LightRenderStyle
                        //Vector4f lightColor, lightPosAndRadius, lightOrientation, light2Radius;

                        lightColor[lightIndex] = light.LightColor.ToVector();
                        lightPosAndRadius[lightIndex] = new Vector4f(light.Center, light.LightRadius);

                        // set type of light
                        if (light.LightType.LightType == GZGeneral.LightType.SPOT)
                        {
                            lightOrientation[lightIndex] = new Vector4f(light.VectorLookAt, 1f);
                            light2Radius[lightIndex] = new Vector2f(CosDeg(light.LightSpotRadius1), CosDeg(light.LightSpotRadius2));
                        }
                        else lightOrientation[lightIndex].W = 0f;

                        lightIndex++;

                        if (lightIndex >= lightColor.Length)
                            break;

                    }
                }

                bool havelights = (lightIndex > 0);

                for (int i = lightIndex; i < lightColor.Length; i++)
                    lightColor[i].W = 0;

                if (hadlights != havelights || havelights)
                {
                    graphics.SetUniform(UniformName.lightColor, lightColor);
                    if (havelights)
                    {
                        graphics.SetUniform(UniformName.lightPosAndRadius, lightPosAndRadius);
                        graphics.SetUniform(UniformName.lightOrientation, lightOrientation);
                        graphics.SetUniform(UniformName.light2Radius, light2Radius);
                    }
                }

                hadlights = havelights;

                GZModel model = General.Map.Data.ModeldefEntries[t.Thing.Type].Model;
                for (int j = 0; j < model.Meshes.Count; j++)
                {
                    graphics.SetTexture(model.Textures[j]);

                    // Render!
                    model.Meshes[j].Draw(graphics);
                }
			}

            graphics.SetUniform(UniformName.lightsEnabled, false);

        }

		//mxd
		private void RenderSky(IEnumerable<VisualGeometry> geo)
		{
			VisualSector sector = null;
			
			// Set render settings
			graphics.SetShader(ShaderName.world3d_skybox);
            graphics.SetTexture(General.Map.Data.SkyBox);
			graphics.SetUniform(UniformName.campos, new Vector4f((float)cameraposition.x, (float)cameraposition.y, (float)cameraposition.z, 0f));

			foreach(VisualGeometry g in geo)
			{
				// Changing sector?
				if(!object.ReferenceEquals(g.Sector, sector))
				{
					// Update the sector if needed
					if(g.Sector.NeedsUpdateGeo) g.Sector.Update(graphics);

					// Only do this sector when a vertexbuffer is created
					//mxd. No Map means that sector was deleted recently, I suppose
					if(g.Sector.GeometryBuffer != null && g.Sector.Sector.Map != null)
					{
						// Change current sector
						sector = g.Sector;

						// Set stream source
						graphics.SetVertexBuffer(sector.GeometryBuffer);
					}
					else
					{
						sector = null;
					}
				}

				if(sector != null)
				{
					graphics.SetUniform(UniformName.highlightcolor, CalculateHighlightColor((g == highlighted) && showhighlight, (g.Selected && showselection)));
					graphics.Draw(PrimitiveType.TriangleList, g.VertexOffset, g.Triangles);
				}
			}
		}

        // [ZZ] this is copied from GZDoom
        private float Smoothstep(float edge0, float edge1, float x)
        {
            double t = Math.Min(Math.Max((x - edge0) / (edge1 - edge0), 0.0), 1.0);
            return (float)(t * t * (3.0 - 2.0 * t));
        }

		//mxd. This gets color from dynamic lights based on distance to thing. 
		//thing position must be in absolute cordinates 
		//(thing.Position.Z value is relative to floor of the sector the thing is in)
		private Color4 GetLitColorForThing(VisualThing t) 
		{
			Color4 litColor = new Color4();
			foreach(VisualThing lt in lightthings)
			{
				// Don't light self
				if(General.Map.Data.GldefsEntries.ContainsKey(t.Thing.Type) && General.Map.Data.GldefsEntries[t.Thing.Type].DontLightSelf && t.Thing.Index == lt.Thing.Index)
					continue;

				float distSquared = Vector3f.DistanceSquared(lt.Center, t.Center);
                float radiusSquared = lt.LightRadius * lt.LightRadius;
				if(distSquared < radiusSquared) 
				{
                    int sign = (lt.LightType.LightRenderStyle == GZGeneral.LightRenderStyle.SUBTRACTIVE ? -1 : 1);
                    Vector3f L = (t.Center - lt.Center);
                    float dist = L.Length();
                    float scaler = 1 - dist / lt.LightRadius * lt.LightColor.Alpha;

                    if (lt.LightType.LightType == GZGeneral.LightType.SPOT)
                    {
                        Vector3f lookAt = lt.VectorLookAt;
                        L.Normalize();
                        float cosDir = Vector3f.Dot(-L, lookAt);
                        scaler *= (float)Smoothstep(CosDeg(lt.LightSpotRadius2), CosDeg(lt.LightSpotRadius1), cosDir);
                    }

                    if (scaler > 0)
                    {
                        litColor.Red += lt.LightColor.Red * scaler * sign;
                        litColor.Green += lt.LightColor.Green * scaler * sign;
                        litColor.Blue += lt.LightColor.Blue * scaler * sign;
                    }
				}
			}

			return litColor;
		}

		// This calculates the highlight/selection color
		private Color4 CalculateHighlightColor(bool ishighlighted, bool isselected)
		{
			if(!ishighlighted && !isselected) return new Color4(); //mxd
			Color4 highlightcolor = isselected ? General.Colors.Selection.ToColorValue() : General.Colors.Highlight.ToColorValue();
			highlightcolor.Alpha = ishighlighted ? highlightglowinv : highlightglow;
			return highlightcolor;
		}
		
		// This finishes rendering
		public void Finish()
		{
			General.Plugins.OnPresentDisplayBegin();

			// Done
			graphics.FinishRendering();
			graphics.Present();
			highlighted = null;
		}
		
		#endregion
		
		#region ================== Rendering
		
		// This sets the highlighted object for the rendering
		public void SetHighlightedObject(IVisualPickable obj)
		{
			highlighted = obj;
		}
		
		// This collects a visual sector's geometry for rendering
		public void AddSectorGeometry(VisualGeometry g)
		{
			// Must have a texture and vertices
			if(g.Texture != null && g.Triangles > 0)
			{
				if(g.RenderAsSky && General.Settings.GZDrawSky)
				{
					skygeo.Add(g);
				}
				else
				{
					switch(g.RenderPass)
					{
						case RenderPass.Solid:
							if(!solidgeo.ContainsKey(g.Texture))
								solidgeo.Add(g.Texture, new List<VisualGeometry>());
							solidgeo[g.Texture].Add(g);
							break;

						case RenderPass.Mask:
							if(!maskedgeo.ContainsKey(g.Texture))
								maskedgeo.Add(g.Texture, new List<VisualGeometry>());
							maskedgeo[g.Texture].Add(g);
							break;

						case RenderPass.Additive:
						case RenderPass.Alpha:
							translucentgeo.Add(g);
							break;

						default:
							throw new NotImplementedException("Geometry rendering of " + g.RenderPass + " render pass is not implemented!");
					}
				}
			}
		}

		// This collects a visual sector's geometry for rendering
		public void AddThingGeometry(VisualThing t)
		{
			// The thing might have changed, especially when doing realtime editing from the edit thing dialog, so update it if necessary
			t.Update();

			//mxd. Gather lights
			if (General.Settings.GZDrawLightsMode != LightRenderMode.NONE && !fullbrightness && t.LightType != null)
			{
				t.UpdateLightRadius();
                if (t.LightRadius > 0)
				{
                    if (t.LightType != null && t.LightType.LightAnimated)
                        t.UpdateBoundingBox();
					lightthings.Add(t);
				}
			}

			//mxd. Gather models
			if((t.Thing.RenderMode == ThingRenderMode.MODEL || t.Thing.RenderMode == ThingRenderMode.VOXEL) && 
				(General.Settings.GZDrawModelsMode == ModelRenderMode.ALL ||
				 General.Settings.GZDrawModelsMode == ModelRenderMode.ACTIVE_THINGS_FILTER ||
				(General.Settings.GZDrawModelsMode == ModelRenderMode.SELECTION && t.Selected))) 
			{
                if (t.RenderPass == RenderPass.Mask ||
                    t.RenderPass == RenderPass.Solid ||
                    (t.RenderPass == RenderPass.Alpha && (t.VertexColor & 0xFF000000) == 0xFF000000))
                {
                    ModelData mde = General.Map.Data.ModeldefEntries[t.Thing.Type];
                    if (!maskedmodelthings.ContainsKey(mde)) maskedmodelthings.Add(mde, new List<VisualThing>());
                    maskedmodelthings[mde].Add(t);
                }
                else if (t.RenderPass == RenderPass.Alpha || t.RenderPass == RenderPass.Additive)
                {
                    translucentmodelthings.Add(t);
                }
                else
                {
                    throw new NotImplementedException("Thing model rendering of " + t.RenderPass + " render pass is not implemented!");
                }
			}
			// Gather regular things
			else 
			{
				//mxd. Set correct texture, geobuffer and triangles count
				t.UpdateSpriteFrame();

				//Must have a texture!
				if(t.Texture != null)
				{
					//mxd
					switch(t.RenderPass)
					{
						case RenderPass.Solid:
							if(!solidthings.ContainsKey(t.Texture)) solidthings.Add(t.Texture, new List<VisualThing>());
							solidthings[t.Texture].Add(t);
							break;

						case RenderPass.Mask:
							if(!maskedthings.ContainsKey(t.Texture)) maskedthings.Add(t.Texture, new List<VisualThing>());
							maskedthings[t.Texture].Add(t);
							break;

						case RenderPass.Additive:
						case RenderPass.Alpha:
							translucentthings.Add(t);
							break;

						default:
							throw new NotImplementedException("Thing rendering of " + t.RenderPass + " render pass is not implemented!");
					}
				}
			}

			//mxd. Add to the plain list
			allthings.Add(t);
		}

		//mxd
		public void SetVisualVertices(List<VisualVertex> verts) { visualvertices = verts; }

		public void SetVisualSlopeHandles(ICollection<VisualSlope> handles) { visualslopehandles = handles; }

		//mxd
		public void SetEventLines(List<Line3D> lines) { eventlines = lines; }

		//mxd
		private static bool BoundingBoxesIntersect(Vector3D[] bbox1, Vector3D[] bbox2) 
		{
			Vector3D dist = bbox1[0] - bbox2[0];

			Vector3D halfSize1 = bbox1[0] - bbox1[1];
			Vector3D halfSize2 = bbox2[0] - bbox2[1];

			return (halfSize1.x + halfSize2.x >= Math.Abs(dist.x) && halfSize1.y + halfSize2.y >= Math.Abs(dist.y) && halfSize1.z + halfSize2.z >= Math.Abs(dist.z));
		}

		// This renders the crosshair
		public void RenderCrosshair()
		{
			//mxd
			world = Matrix.Identity;
            graphics.SetUniform(UniformName.world, world);

            // Set renderstates
            graphics.SetCullMode(Cull.None);
			graphics.SetZEnable(false);
			graphics.SetAlphaBlendEnable(true);
			graphics.SetAlphaTestEnable(false);
			graphics.SetSourceBlend(Blend.SourceAlpha);
			graphics.SetDestinationBlend(Blend.InverseSourceAlpha);
            graphics.SetShader(ShaderName.display2d_normal);
            graphics.SetUniform(UniformName.projection, world * view2d);
            graphics.SetUniform(UniformName.texturefactor, new Color4(1f, 1f, 1f, 1f));
            graphics.SetUniform(UniformName.rendersettings, new Vector4f(1.0f, 1.0f, 0.0f, 1.0f));
            graphics.SetSamplerFilter(General.Settings.VisualBilinear ? TextureFilter.Linear : TextureFilter.Nearest);

            // Texture
            if (crosshairbusy)
			{
				graphics.SetTexture(General.Map.Data.CrosshairBusy3D.Texture);
			}
			else
			{
				graphics.SetTexture(General.Map.Data.Crosshair3D.Texture);
			}
			
			// Draw
            graphics.Draw(PrimitiveType.TriangleStrip, 0, 2, crosshairverts);

			if (General.Settings.ShowFPS)
				General.Map.Renderer2D.RenderText(fpsLabel);
        }

        // This switches fog on and off
        public void SetFogMode(bool usefog)
		{
            if (usefog)
            {
                graphics.SetUniform(UniformName.fogsettings, new Vector4f(General.Settings.ViewDistance * FOG_RANGE, General.Settings.ViewDistance, 0.0f, 0.0f));
            }
            else
            {
                graphics.SetUniform(UniformName.fogsettings, new Vector4f(-1.0f));
            }
		}

		// This siwtches crosshair busy icon on and off
		public void SetCrosshairBusy(bool busy)
		{
			crosshairbusy = busy;
		}
		
		#endregion
	}
}
