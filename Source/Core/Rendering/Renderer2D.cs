
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
using System.Net;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.Editing;
using CodeImp.DoomBuilder.GZBuilder.Data; //mxd
using CodeImp.DoomBuilder.Config; //mxd
using CodeImp.DoomBuilder.GZBuilder;

#endregion

namespace CodeImp.DoomBuilder.Rendering
{

	/* This renders a 2D presentation of the map. This is done in several
	 * layers which each are optimized for a different purpose. Set the
	 * PresentationLayer(s) to specify how to present these layers.
	 */

	internal sealed class Renderer2D : Renderer, IRenderer2D
	{
		#region ================== Constants

		private const float FSAA_FACTOR = 0.6f;
		private const int MAP_CENTER_SIZE = 16; //mxd
		private const float THING_ARROW_SIZE = 1.4f;
		//private const float THING_ARROW_SHRINK = 2f;
		//private const float THING_CIRCLE_SIZE = 1f;
		private const float THING_SPRITE_SHRINK = 2f;
		private const int THING_BUFFER_SIZE = 100;
		private const float MINIMUM_THING_RADIUS = 1.5f; //mxd
		private const float MINIMUM_SPRITE_RADIUS = 8.0f; //mxd
		internal const float FIXED_THING_SIZE = 48.0f; //mxd

		internal const int NUM_VIEW_MODES = 4;
		
		#endregion

		#region ================== Variables

		// Rendertargets
		private Plotter gridplotter;
        private Plotter plotter;
        private Texture thingstex;
		private Texture overlaytex;
		private Texture surfacetex;

		// Rendertarget sizes
		private Size windowsize;

		// Vertices to present the textures
		private VertexBuffer screenverts;
		private FlatVertex[] backimageverts;
		
		// Batch buffer for things rendering
		private VertexBuffer thingsvertices;
		
		// Render settings
		private int vertexsize;
		private RenderLayers renderlayer = RenderLayers.None;
		
		// Surfaces
		private SurfaceManager surfaces;
		
		// View settings (world coordinates)
		private ViewMode viewmode;
		private float scale;
		private float scaleinv;
		private float offsetx;
		private float offsety;
		private float translatex;
		private float translatey;
		private float linenormalsize;
		private float minlinelength; //mxd. Linedef should be longer than this to be rendered
		private float minlinenormallength; //mxd. Linedef direction indicator should be longer than this to be rendered 
		private bool drawmapcenter = true; //mxd
		private bool lastdrawmapcenter = true; //mxd
		private float lastgridscale = -1f;
		private float lastgridsize;
		private float lastgridx;
		private float lastgridy;
		private RectangleF viewport;
		private RectangleF yviewport;

        // Spaghetti
        Matrix viewmatrix = Matrix.Identity;
        Matrix worldmatrix = Matrix.Identity;

        // Presentation
        private Presentation present;
		
		#endregion

		#region ================== Properties

		public float OffsetX { get { return offsetx; } }
		public float OffsetY { get { return offsety; } }
		public float TranslateX { get { return translatex; } }
		public float TranslateY { get { return translatey; } }
		public float Scale { get { return scale; } }
		public int VertexSize { get { return vertexsize; } }
		public bool DrawMapCenter { get { return drawmapcenter; } set { drawmapcenter = value; } } //mxd
		public ViewMode ViewMode { get { return viewmode; } }
		public SurfaceManager Surfaces { get { return surfaces; } }
		public RectangleF Viewport { get { return viewport; } } //mxd

		private bool ShouldRenderVertices
		{
			get
			{
				if (!(General.Editing.Mode is ClassicMode mode))
				{
					return true;
				}
				return mode.AlwaysShowVertices || General.Settings.AlwaysShowVertices;
			}
		}

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		internal Renderer2D(RenderDevice graphics) : base(graphics)
		{
			// Create surface manager
			surfaces = new SurfaceManager();

			// Create rendertargets
			CreateRendertargets();
			
			// We have no destructor
			GC.SuppressFinalize(this);
		}

		// Disposer
		public override void Dispose()
		{
			// Not already disposed?
			if(!isdisposed)
			{
				// Destroy rendertargets
				DestroyRendertargets();
				
				// Dispose surface manager
				surfaces.Dispose();
				
				// Done
				base.Dispose();
			}
		}

		#endregion

		#region ================== Presenting

		// This sets the presentation to use
		public void SetPresentation(Presentation present)
		{
			this.present = new Presentation(present);
		}
		
		// This draws the image on screen
		public void Present()
		{
			General.Plugins.OnPresentDisplayBegin();

            // Start drawing
            graphics.StartRendering(true, General.Colors.Background.ToColorValue());

			// Renderstates that count for this whole sequence
			graphics.SetCullMode(Cull.None);
			graphics.SetZEnable(false);
			graphics.SetVertexBuffer(screenverts);
			worldmatrix = Matrix.Identity;

			// Go for all layers
			foreach(PresentLayer layer in present.layers)
			{
				ShaderName aapass;

				// Set blending mode
				switch(layer.blending)
				{
					case BlendingMode.None:
						graphics.SetAlphaBlendEnable(false);
						graphics.SetAlphaTestEnable(false);
						graphics.SetUniform(UniformName.texturefactor, new Color4(1f, 1f, 1f, 1f));
                        break;

					case BlendingMode.Mask:
						graphics.SetAlphaBlendEnable(false);
						graphics.SetAlphaTestEnable(true);
						graphics.SetUniform(UniformName.texturefactor, new Color4(1f, 1f, 1f, 1f));
                        break;

					case BlendingMode.Alpha:
						graphics.SetAlphaBlendEnable(true);
						graphics.SetAlphaTestEnable(false);
						graphics.SetSourceBlend(Blend.SourceAlpha);
						graphics.SetDestinationBlend(Blend.InverseSourceAlpha);
						graphics.SetUniform(UniformName.texturefactor, new Color4(1f, 1f, 1f, 1f));
						break;

					case BlendingMode.Additive:
						graphics.SetAlphaBlendEnable(true);
						graphics.SetAlphaTestEnable(false);
						graphics.SetSourceBlend(Blend.SourceAlpha);
						graphics.SetDestinationBlend(Blend.One);
						graphics.SetUniform(UniformName.texturefactor, new Color4(1f, 1f, 1f, 1f));
						break;
				}

				// Check which pass to use
				if(layer.antialiasing && General.Settings.QualityDisplay) aapass = ShaderName.display2d_fsaa; else aapass = ShaderName.display2d_normal;

				// Render layer
				switch(layer.layer)
				{
					// BACKGROUND
					case RendererLayer.Background:
						if((backimageverts == null) || (General.Map.Grid.Background.Texture == null)) break;
                        graphics.SetShader(aapass);
                        graphics.SetTexture(General.Map.Grid.Background.Texture);
						graphics.SetSamplerState(TextureAddress.Wrap);
						SetDisplay2DSettings(1f / windowsize.Width, 1f / windowsize.Height, FSAA_FACTOR, layer.alpha, false, true);
						graphics.Draw(PrimitiveType.TriangleStrip, 0, 2, backimageverts);
						graphics.SetVertexBuffer(screenverts);
						break;

					// GRID
					case RendererLayer.Grid:
                        graphics.SetShader(aapass);
                        graphics.SetTexture(gridplotter.Texture);
						graphics.SetSamplerState(TextureAddress.Wrap);
						SetDisplay2DSettings(1f / gridplotter.Width, 1f / gridplotter.Height, FSAA_FACTOR, layer.alpha, false, true);
						graphics.Draw(PrimitiveType.TriangleStrip, 0, 2);
						break;

					// GEOMETRY
					case RendererLayer.Geometry:
                        graphics.SetShader(aapass);
                        graphics.SetTexture(plotter.Texture);
						graphics.SetSamplerState(TextureAddress.Wrap);
						SetDisplay2DSettings(1f / plotter.Width, 1f / plotter.Height, FSAA_FACTOR, layer.alpha, false, false);
						graphics.Draw(PrimitiveType.TriangleStrip, 0, 2);
						break;

					// THINGS
					case RendererLayer.Things:
                        graphics.SetShader(aapass);
                        graphics.SetTexture(thingstex);
						graphics.SetSamplerState(TextureAddress.Clamp);
						SetDisplay2DSettings(1f / thingstex.Width, 1f / thingstex.Height, FSAA_FACTOR, layer.alpha, false, true);
						graphics.Draw(PrimitiveType.TriangleStrip, 0, 2);
						break;

					// OVERLAY
					case RendererLayer.Overlay:
                        graphics.SetShader(aapass);
                        graphics.SetTexture(overlaytex);
						graphics.SetSamplerState(TextureAddress.Wrap);
						SetDisplay2DSettings(1f / overlaytex.Width, 1f / overlaytex.Height, FSAA_FACTOR, layer.alpha, false, true);
						graphics.Draw(PrimitiveType.TriangleStrip, 0, 2);
						break;

					// SURFACE
					case RendererLayer.Surface:
                        graphics.SetShader(aapass);
                        graphics.SetTexture(surfacetex);
						graphics.SetSamplerState(TextureAddress.Wrap);
						SetDisplay2DSettings(1f / overlaytex.Width, 1f / overlaytex.Height, FSAA_FACTOR, layer.alpha, false, true);
						graphics.Draw(PrimitiveType.TriangleStrip, 0, 2);
						break;
				}
			}

			// Done
			graphics.FinishRendering();
			graphics.Present();

			// Release binds
			graphics.SetTexture(null);
			graphics.SetVertexBuffer(null);
		}
		
		#endregion

		#region ================== Management

		// This is called before a device is reset
		// (when resized or display adapter was changed)
		public override void UnloadResource()
		{
			// Destroy rendertargets
			DestroyRendertargets();
		}
		
		// This is called resets when the device is reset
		// (when resized or display adapter was changed)
		public override void ReloadResource()
		{
			// Re-create rendertargets
			CreateRendertargets();
		}

		// This resets the graphics
		/*public override void Reset()
		{
			UnloadResource();
			ReloadResource();
		}*/

		// This destroys the rendertargets
		public void DestroyRendertargets()
		{
			// Trash rendertargets
			if(plotter != null) plotter.Dispose();
			if(thingstex != null) thingstex.Dispose();
			if(overlaytex != null) overlaytex.Dispose();
			if(surfacetex != null) surfacetex.Dispose();
			if(gridplotter != null) gridplotter.Dispose();
			if(screenverts != null) screenverts.Dispose();
			thingstex = null;
            gridplotter = null;
			screenverts = null;
			overlaytex = null;
			surfacetex = null;
			
			// Trash things batch buffer
			if(thingsvertices != null) thingsvertices.Dispose();
			thingsvertices = null;
			lastgridscale = -1f;
			lastgridsize = 0.0f;
		}
		
		// Allocates new image memory to render on
		public void CreateRendertargets()
		{
			// Destroy rendertargets
			DestroyRendertargets();

			// Get new width and height
			windowsize.Width = graphics.RenderTarget.ClientSize.Width;
			windowsize.Height = graphics.RenderTarget.ClientSize.Height;

			// Create rendertargets textures
			plotter = new Plotter(windowsize.Width, windowsize.Height);
            gridplotter = new Plotter(windowsize.Width, windowsize.Height);
            thingstex = new Texture(windowsize.Width, windowsize.Height, TextureFormat.Rgba8);
			overlaytex = new Texture(windowsize.Width, windowsize.Height, TextureFormat.Rgba8);
			surfacetex = new Texture(windowsize.Width, windowsize.Height, TextureFormat.Rgba8);
			
			// Clear rendertargets
			graphics.ClearTexture(General.Colors.Background.WithAlpha(0).ToColorValue(), thingstex);
			graphics.ClearTexture(General.Colors.Background.WithAlpha(0).ToColorValue(), overlaytex);
			
			// Create vertex buffers
			screenverts = new VertexBuffer();
			thingsvertices = new VertexBuffer();
            graphics.SetBufferData(thingsvertices, THING_BUFFER_SIZE * 12, VertexFormat.Flat);

			// Make screen vertices
			FlatVertex[] verts = CreateScreenVerts(new Size(windowsize.Width, windowsize.Height));
            graphics.SetBufferData(screenverts, verts);
			
			// Force update of view
			lastgridscale = -1f;
			lastgridsize = 0.0f;
			lastgridx = 0.0f;
			lastgridy = 0.0f;
			UpdateTransformations();

            if (General.Map != null && General.Map.Config != null)
            {
                // [ZZ] old texture is Gone here. Redraw
                plotter.Clear();
                gridplotter.Clear();
                RenderBackgroundGrid();
                SetupBackground();
                gridplotter.DrawContents(graphics);
                plotter.DrawContents(graphics);
            }
		}

		// This makes screen vertices for display
		private static FlatVertex[] CreateScreenVerts(Size texturesize)
		{
			FlatVertex[] screenverts = new FlatVertex[4];
            screenverts[0].x = 0.0f;
            screenverts[0].y = 0.0f;
            screenverts[0].c = -1;
            screenverts[0].u = 0.0f;
            screenverts[0].v = 0.0f;
            screenverts[1].x = texturesize.Width;
            screenverts[1].y = 0.0f;
            screenverts[1].c = -1;
            screenverts[1].u = 1.0f;
            screenverts[1].v = 0.0f;
            screenverts[2].x = 0.0f;
            screenverts[2].y = texturesize.Height;
            screenverts[2].c = -1;
            screenverts[2].u = 0.0f;
            screenverts[2].v = 1.0f;
            screenverts[3].x = texturesize.Width;
            screenverts[3].y = texturesize.Height;
            screenverts[3].c = -1;
            screenverts[3].u = 1.0f;
            screenverts[3].v = 1.0f;
            return screenverts;
		}

		#endregion
		
		#region ================== View

		// This changes view mode
		public void SetViewMode(ViewMode mode)
		{
			viewmode = mode;
		}
		
		// This changes view position
		public void PositionView(float x, float y)
		{
			// Change position in world coordinates
			offsetx = x;
			offsety = y;
			UpdateTransformations();
		}
		
		// This changes zoom
		public void ScaleView(float scale)
		{
			// Change zoom scale
			this.scale = scale;
			UpdateTransformations();
			
			// Show zoom on main window
			General.MainWindow.UpdateZoom(scale);
		}

		// This updates some maths
		private void UpdateTransformations()
		{
			scaleinv = 1f / scale;
			translatex = -offsetx + (windowsize.Width * 0.5f) * scaleinv;
			translatey = -offsety - (windowsize.Height * 0.5f) * scaleinv;
			linenormalsize = 10f * scaleinv;
			minlinelength = linenormalsize * 0.0625f; //mxd
			minlinenormallength = linenormalsize * 2f; //mxd

			vertexsize = (int)(1.7f * General.Settings.GZVertexScale2D * scale + 0.5f); //mxd. added GZVertexScale2D
			if(vertexsize < 0) vertexsize = 0;
			if(vertexsize > 4) vertexsize = 4;

            viewmatrix = Matrix.Scaling(2.0f / windowsize.Width, -2.0f / windowsize.Height, 1.0f) * Matrix.Translation(-1.0f, 1.0f, 0.0f);
			Vector2D lt = DisplayToMap(new Vector2D(0.0f, 0.0f));
			Vector2D rb = DisplayToMap(new Vector2D(windowsize.Width, windowsize.Height));
			viewport = new RectangleF((float)lt.x, (float)lt.y, (float)(rb.x - lt.x), (float)(rb.y - lt.y));
			yviewport = new RectangleF((float)lt.x, (float)rb.y, (float)(rb.x - lt.x), (float)(lt.y - rb.y));
		}

		// This sets the world matrix for transformation
		private void SetWorldTransformation(bool transform)
		{
			if(transform)
			{
				Matrix translate = Matrix.Translation(translatex, translatey, 0f);
				Matrix scaling = Matrix.Scaling(scale, -scale, 1f);
				worldmatrix = translate * scaling;
			}
			else
			{
				worldmatrix = Matrix.Identity;
			}
		}

        private void SetDisplay2DSettings(float texelx, float texely, float fsaafactor, float alpha, bool bilinear, bool flipY = false)
        {
            Vector4f values = new Vector4f(texelx, texely, fsaafactor, alpha);
            graphics.SetUniform(UniformName.rendersettings, values);
            if (flipY)
                graphics.SetUniform(UniformName.projection, worldmatrix * viewmatrix * Matrix.Scaling(1f, -1f, 1f));
            else
                graphics.SetUniform(UniformName.projection, worldmatrix * viewmatrix);
            graphics.SetSamplerFilter(bilinear ? TextureFilter.Linear : TextureFilter.Nearest);
        }

        private void SetThings2DSettings(float alpha)
        {
            Vector4f values = new Vector4f(0.0f, 0.0f, 1.0f, alpha);
            graphics.SetUniform(UniformName.rendersettings, values);
            graphics.SetUniform(UniformName.projection, worldmatrix * viewmatrix);
        }

        //mxd. Used to render models
        private void SetThings2DTransformSettings(Matrix world)
        {
            graphics.SetUniform(UniformName.projection, world * viewmatrix);
        }

        /// <summary>
        /// This unprojects display coordinates (screen space) to map coordinates
        /// </summary>
        public Vector2D DisplayToMap(Vector2D mousepos)
		{
			return mousepos.GetInvTransformed(-translatex, -translatey, scaleinv, -scaleinv);
		}
		
		/// <summary>
		/// This projects map coordinates to display coordinates (screen space)
		/// </summary>
		public Vector2D MapToDisplay(Vector2D mappos)
		{
			return mappos.GetTransformed(translatex, translatey, scale, -scale);
		}
		
		#endregion

		#region ================== Colors

		// This returns the color for a thing
		public PixelColor DetermineThingColor(Thing t)
		{
			// Determine color
			if(t.Selected) return General.Colors.Selection;
			
			//mxd. If thing is light, set it's color to light color:
			if(t.DynamicLightType != null)
			{
				if (t.DynamicLightType.LightDef == GZGeneral.LightDef.VAVOOM_GENERIC) //vavoom light
					return new PixelColor(255, 255, 255, 255);
				if (t.DynamicLightType.LightDef == GZGeneral.LightDef.VAVOOM_COLORED) //vavoom colored light
					return new PixelColor(255, (byte)t.Args[1], (byte)t.Args[2], (byte)t.Args[3]);
				if (t.DynamicLightType.LightType == GZGeneral.LightType.SPOT)
				{
					if (t.Fields.ContainsKey("arg0str"))
					{
						PixelColor pc;
						ZDoom.ZDTextParser.GetColorFromString(t.Fields["arg0str"].Value.ToString(), out pc);
						pc.a = 255;
						return pc;
					}
					return new PixelColor(255, (byte)((t.Args[0] & 0xFF0000) >> 16), (byte)((t.Args[0] & 0x00FF00) >> 8), (byte)((t.Args[0] & 0x0000FF)));
				}
				return new PixelColor(255, (byte)t.Args[0], (byte)t.Args[1], (byte)t.Args[2]);
			}

			return t.Color;
		}

		// This returns the color for a vertex
		public int DetermineVertexColor(Vertex v)
		{
			// Determine color
			if(v.Selected) return ColorCollection.SELECTION;
			return ColorCollection.VERTICES;
		}

		// This returns the color for a linedef
		public PixelColor DetermineLinedefColor(Linedef l)
		{
			if(l.Selected) return General.Colors.Selection;

			//mxd. Impassable lines
			if(l.ImpassableFlag) 
			{
				if(l.ColorPresetIndex != -1)
					return General.Map.ConfigSettings.LinedefColorPresets[l.ColorPresetIndex].Color;
				return General.Colors.Linedefs;
			}

			//mxd. Passable lines
			if(l.ColorPresetIndex != -1)
				return General.Map.ConfigSettings.LinedefColorPresets[l.ColorPresetIndex].Color.WithAlpha(General.Settings.DoubleSidedAlphaByte);
			return General.Colors.Linedefs.WithAlpha(General.Settings.DoubleSidedAlphaByte);
		}

		//mxd. This collects indices of linedefs, which are parts of sectors with 3d floors
		public void UpdateExtraFloorFlag() 
		{
			HashSet<int> tags = new HashSet<int>();
			
			//find lines with 3d floor action and collect sector tags
			foreach(Linedef l in General.Map.Map.Linedefs)
			{
				if(l.Action == 160) 
				{
					int sectortag = (General.Map.UDMF || (l.Args[1] & 8) != 0) ? l.Args[0] : l.Args[0] + (l.Args[4] << 8);
					if(sectortag != 0 && !tags.Contains(sectortag)) tags.Add(sectortag);
				}
			}

			//find lines, which are related to sectors with 3d floors, and collect their valuable indices
			foreach(Linedef l in General.Map.Map.Linedefs) 
			{
				if(l.Front != null && l.Front.Sector != null && l.Front.Sector.Tag != 0 && tags.Overlaps(l.Front.Sector.Tags)) 
				{
					l.ExtraFloorFlag = true;
					continue;
				}
				if(l.Back != null && l.Back.Sector != null && l.Back.Sector.Tag != 0 && tags.Overlaps(l.Back.Sector.Tags)) 
				{
					l.ExtraFloorFlag = true;
					continue;
				}

				l.ExtraFloorFlag = false;
			}
		}

		#endregion

		#region ================== Start / Finish

		// This begins a drawing session
		public bool StartPlotter(bool clear)
		{
			if(renderlayer != RenderLayers.None)
			{
#if DEBUG
				throw new InvalidOperationException("Renderer starting called before finished previous layer. Call Finish() first!");
#else
				return false; //mxd. Can't render. Most probably because previous frame or render layer wasn't finished yet.
#endif
			}

			renderlayer = RenderLayers.Plotter;
			
			// Rendertargets available?
			if(plotter != null)
			{
				// Redraw grid when structures image was cleared
				if(clear)
				{
					plotter.Clear();
					RenderBackgroundGrid();
					SetupBackground();
				}
				
				// Ready for rendering
				UpdateTransformations();
				return true;
			}

			// Can't render!
			Finish();
			return false;
		}

		// This begins a drawing session
		public bool StartThings(bool clear)
		{
			if(renderlayer != RenderLayers.None)
			{
#if DEBUG
				throw new InvalidOperationException("Renderer starting called before finished previous layer. Call Finish() first!");
#else
				return false; //mxd. Can't render. Most probably because previous frame or render layer wasn't finished yet.
#endif
			}

			renderlayer = RenderLayers.Things;
			
			// Rendertargets available?
			if(thingstex != null)
			{
				// Set the rendertarget to the things texture
                graphics.StartRendering(clear, General.Colors.Background.WithAlpha(0).ToColorValue(), thingstex, false);

				// Ready for rendering
				UpdateTransformations();
				return true;
			}

			// Can't render!
			Finish();
			return false;
		}

		// This begins a drawing session
		public bool StartOverlay(bool clear)
		{
			if(renderlayer != RenderLayers.None)
			{
#if DEBUG
				throw new InvalidOperationException("Renderer starting called before finished previous layer. Call Finish() first!");
#else
				return false; //mxd. Can't render. Most probably because previous frame or render layer wasn't finished yet.
#endif
			}

			renderlayer = RenderLayers.Overlay;
			
			// Rendertargets available?
			if(overlaytex != null)
			{
				// Set the rendertarget to the things texture
                graphics.StartRendering(clear, General.Colors.Background.WithAlpha(0).ToColorValue(), overlaytex, false);

				// Ready for rendering
				UpdateTransformations();
				return true;
			}

			// Can't render!
			Finish();
			return false;
		}

		// This ends a drawing session
		public void Finish()
		{
			// Draw plotter contents
			if(renderlayer == RenderLayers.Plotter)
			{
				plotter.DrawContents(graphics);
			}
			
			// Clean up things / overlay
			if((renderlayer == RenderLayers.Things) || (renderlayer == RenderLayers.Overlay) || (renderlayer == RenderLayers.Surface))
			{
				// Stop rendering
				graphics.FinishRendering();
			}
			
			// Done
			renderlayer = RenderLayers.None;
		}

		#endregion

		#region ================== Background

		// This sets up background image vertices
		private void SetupBackground()
		{
			// Only if a background image is set
			if((General.Map.Grid.Background != null) && !(General.Map.Grid.Background is UnknownImage))
			{
				Vector2D backoffset = new Vector2D(General.Map.Grid.BackgroundX, General.Map.Grid.BackgroundY);
				Vector2D backimagesize = new Vector2D(General.Map.Grid.Background.ScaledWidth, General.Map.Grid.Background.ScaledHeight);
				Vector2D backimagescale = new Vector2D(General.Map.Grid.BackgroundScaleX, General.Map.Grid.BackgroundScaleY);

				// Scale the background image size
				backimagesize *= backimagescale;
				
				// Make vertices
				backimageverts = CreateScreenVerts(windowsize);

				// Determine map coordinates for view window
				Vector2D lbpos = DisplayToMap(new Vector2D(0f, windowsize.Height));
				Vector2D rtpos = DisplayToMap(new Vector2D(windowsize.Width, 0));

				// Offset by given background offset
				lbpos -= backoffset;
				rtpos -= backoffset;
			
				// Calculate UV coordinates
				// NOTE: backimagesize.y is made negative to match Doom's coordinate system
				backimageverts[0].u = (float)(lbpos.x / backimagesize.x);
				backimageverts[0].v = (float)(lbpos.y / -backimagesize.y);
				backimageverts[1].u = (float)(rtpos.x / backimagesize.x);
				backimageverts[1].v = (float)(lbpos.y / -backimagesize.y);
				backimageverts[2].u = (float)(lbpos.x / backimagesize.x);
				backimageverts[2].v = (float)(rtpos.y / -backimagesize.y);
				backimageverts[3].u = (float)(rtpos.x / backimagesize.x);
				backimageverts[3].v = (float)(rtpos.y / -backimagesize.y);
			}
			else
			{
				// No background image
				backimageverts = null;
			}
		}

		// This renders all grid
		private void RenderBackgroundGrid()
		{
			// Do we need to redraw grid?
			if(lastgridsize != General.Map.Grid.GridSizeF || lastgridscale != scale ||
			   lastgridx != offsetx || lastgridy != offsety || drawmapcenter != lastdrawmapcenter)
			{
				gridplotter.Clear();

				if(General.Settings.RenderGrid) //mxd
				{
					bool transformed = Math.Abs(General.Map.Grid.GridOriginX) > 1e-4 || Math.Abs(General.Map.Grid.GridOriginY) > 1e-4 || Math.Abs(General.Map.Grid.GridRotate) > 1e-4;

					if (transformed)
					{
						// Render normal grid
						RenderGridTransformed(General.Map.Grid.GridSizeF, General.Map.Grid.GridRotate,
							General.Map.Grid.GridOriginX, General.Map.Grid.GridOriginY, General.Colors.Grid, gridplotter);

						// Render 64 grid
						if(General.Map.Grid.GridSizeF <= 64)
						{
							RenderGridTransformed(64f, General.Map.Grid.GridRotate,
								General.Map.Grid.GridOriginX, General.Map.Grid.GridOriginY, General.Colors.Grid64, gridplotter);
						}
					} 
					else
					{
						// Render normal grid
						RenderGrid((float)General.Map.Grid.GridSizeF, General.Colors.Grid, gridplotter);

						// Render 64 grid
						if(General.Map.Grid.GridSizeF <= 64) RenderGrid(64f, General.Colors.Grid64, gridplotter);
					}
					
				}
				else
				{
					//mxd. Render map format bounds
					Vector2D tl = new Vector2D(General.Map.Config.LeftBoundary, General.Map.Config.TopBoundary).GetTransformed(translatex, translatey, scale, -scale);
					Vector2D rb = new Vector2D(General.Map.Config.RightBoundary, General.Map.Config.BottomBoundary).GetTransformed(translatex, translatey, scale, -scale);
					PixelColor g = General.Colors.Grid64;
					gridplotter.DrawGridLineH((int)tl.y, (int)tl.x, (int)rb.x, ref g);
					gridplotter.DrawGridLineH((int)rb.y, (int)tl.x, (int)rb.x, ref g);
					gridplotter.DrawGridLineV((int)tl.x, (int)tl.y, (int)rb.y, ref g);
					gridplotter.DrawGridLineV((int)rb.x, (int)tl.y, (int)rb.y, ref g);
				}

				//mxd. Render center of map
				if(drawmapcenter)
				{
					Vector2D center = new Vector2D(0, 0).GetTransformed(translatex, translatey, scale, -scale);
					int cx = (int)center.x;
					int cy = (int)center.y;
					PixelColor c = General.Colors.Highlight;
					gridplotter.DrawLineSolid(cx, cy + MAP_CENTER_SIZE, cx, cy - MAP_CENTER_SIZE, ref c);
					gridplotter.DrawLineSolid(cx - MAP_CENTER_SIZE, cy, cx + MAP_CENTER_SIZE, cy, ref c);
				}

                // Done
                gridplotter.DrawContents(graphics);
				lastgridscale = scale;
				lastgridsize = (float)General.Map.Grid.GridSizeF;
				lastgridx = offsetx;
				lastgridy = offsety;
				lastdrawmapcenter = drawmapcenter; //mxd
			}
		}
		
		// This renders the grid with a transform applied
		private void RenderGridTransformed(double size, double angle, double originx, double originy, PixelColor c, Plotter gridplotter)
		{
            uint mask = 0x55555555;

			//mxd. Increase rendered grid size if needed
			if(!General.Settings.DynamicGridSize && size * scale <= 6f)
				do { size *= 2; } while(size * scale <= 6f);
			double sizeinv = 1f / size;

			if (double.IsInfinity(size) || size < 1e-10)
			{
				return;
			}

			// Determine map coordinates for view window
			Vector2D ltview = DisplayToMap(new Vector2D(0, 0));
			Vector2D rbview = DisplayToMap(new Vector2D(windowsize.Width, windowsize.Height));
			Vector2D mapsize = rbview - ltview;

			Vector2D ltbound = new Vector2D(General.Map.Config.LeftBoundary, General.Map.Config.TopBoundary);
			Vector2D rbbound = new Vector2D(General.Map.Config.RightBoundary, General.Map.Config.BottomBoundary);

			// Translate top left boundary and right bottom boundary of map to screen coords
			Vector2D tlb = ltbound.GetTransformed(translatex, translatey, scale, -scale);
			Vector2D rbb = rbbound.GetTransformed(translatex, translatey, scale, -scale);

			Vector2D center = GridSetup.SnappedToGrid(0.5f * (ltview + rbview), size, sizeinv, angle, originx, originy);

			// Get the angle vectors for the gridlines
			Vector2D dx = new Vector2D(Math.Cos(angle), Math.Sin(angle));
			Vector2D dy = new Vector2D(-Math.Sin(angle), Math.Cos(angle));

			double maxextent = Math.Max(mapsize.x, mapsize.y);
			RectangleF bounds = new RectangleF((float)tlb.x, (float)tlb.y, (float)(rbb.x - tlb.x), (float)(rbb.y - tlb.y));
			bounds.Intersect(new RectangleF(0, 0, windowsize.Width, windowsize.Height));

			bool xminintersect = true, xmaxintersect = true, yminintersect = true, ymaxintersect = true;

			int num = 0;            
			while (xminintersect || xmaxintersect || yminintersect || ymaxintersect) {
				if (num > 1e6)
				{
					// just in case garbage inputs breaks the algorithm and causes an infinite loop
					break;
				}

				Vector2D xminstart = center - num * size * dy;
				Vector2D xmaxstart = center + num * size * dy;
				Vector2D yminstart = center - num * size * dx;
				Vector2D ymaxstart = center + num * size * dx;

				Line2D xminscanline = new Line2D(xminstart - dx * maxextent, xminstart + dx * maxextent);
				Line2D xmaxscanline = new Line2D(xmaxstart - dx * maxextent, xmaxstart + dx * maxextent);
				Line2D yminscanline = new Line2D(yminstart - dy * maxextent, yminstart + dy * maxextent);
				Line2D ymaxscanline = new Line2D(ymaxstart - dy * maxextent, ymaxstart + dy * maxextent);

				Line2D xminplotline = xminscanline.GetTransformed(translatex, translatey, scale, -scale);
				Line2D xmaxplotline = xmaxscanline.GetTransformed(translatex, translatey, scale, -scale);
				Line2D yminplotline = yminscanline.GetTransformed(translatex, translatey, scale, -scale);
				Line2D ymaxplotline = ymaxscanline.GetTransformed(translatex, translatey, scale, -scale);
				xminplotline = Line2D.ClipToRectangle(xminplotline, bounds, out xminintersect);
				xmaxplotline = Line2D.ClipToRectangle(xmaxplotline, bounds, out xmaxintersect);
				yminplotline = Line2D.ClipToRectangle(yminplotline, bounds, out yminintersect);
				ymaxplotline = Line2D.ClipToRectangle(ymaxplotline, bounds, out ymaxintersect);

				if (xminintersect)
				{
					gridplotter.DrawLineSolid((int)xminplotline.v1.x, (int)Math.Round(xminplotline.v1.y + 0.499999), (int)xminplotline.v2.x, (int)Math.Round(xminplotline.v2.y + 0.499999), ref c, mask);
				}
				if (xmaxintersect)
				{
					gridplotter.DrawLineSolid((int)xmaxplotline.v1.x, (int)Math.Round(xmaxplotline.v1.y + 0.499999), (int)xmaxplotline.v2.x, (int)Math.Round(xmaxplotline.v2.y + 0.499999), ref c, mask);
				}
				if (yminintersect)
				{
					gridplotter.DrawLineSolid((int)yminplotline.v1.x, (int)Math.Round(yminplotline.v1.y + 0.499999), (int)yminplotline.v2.x, (int)Math.Round(yminplotline.v2.y + 0.499999), ref c, mask);
				}
				if (ymaxintersect)
				{
					gridplotter.DrawLineSolid((int)ymaxplotline.v1.x, (int)Math.Round(ymaxplotline.v1.y + 0.499999), (int)ymaxplotline.v2.x, (int)Math.Round(ymaxplotline.v2.y + 0.499999), ref c, mask);
				}

				num++;
			}
		}

		// This renders the grid
		private void RenderGrid(float size, PixelColor c, Plotter gridplotter)
		{
			Vector2D pos = new Vector2D();

			//mxd. Increase rendered grid size if needed
			if(!General.Settings.DynamicGridSize && size * scale <= 6f)
				do { size *= 2; } while(size * scale <= 6f);
			float sizeinv = 1f / size;

			// Determine map coordinates for view window
			Vector2D ltpos = DisplayToMap(new Vector2D(0, 0));
			Vector2D rbpos = DisplayToMap(new Vector2D(windowsize.Width, windowsize.Height));

			// Clip to nearest grid
			ltpos = GridSetup.SnappedToGrid(ltpos, size, sizeinv);
			rbpos = GridSetup.SnappedToGrid(rbpos, size, sizeinv);

			// Translate top left boundary and right bottom boundary of map to screen coords
			Vector2D tlb = new Vector2D(General.Map.Config.LeftBoundary, General.Map.Config.TopBoundary).GetTransformed(translatex, translatey, scale, -scale);
			Vector2D rbb = new Vector2D(General.Map.Config.RightBoundary, General.Map.Config.BottomBoundary).GetTransformed(translatex, translatey, scale, -scale);

			// Draw all horizontal grid lines
			float ystart = (float)(rbpos.y > General.Map.Config.BottomBoundary ? rbpos.y : General.Map.Config.BottomBoundary);
			float yend = (float)(ltpos.y < General.Map.Config.TopBoundary ? ltpos.y : General.Map.Config.TopBoundary);

			for(float y = ystart; y < yend + size; y += size) 
			{
				if(y > General.Map.Config.TopBoundary) y = General.Map.Config.TopBoundary;
				else if(y < General.Map.Config.BottomBoundary) y = General.Map.Config.BottomBoundary;

				float from = (float)(tlb.x < 0 ? 0 : tlb.x);
				float to = (float)(rbb.x > windowsize.Width ? windowsize.Width : rbb.x);

				pos.y = y;
				pos = pos.GetTransformed(translatex, translatey, scale, -scale);

                gridplotter.DrawGridLineH((int)Math.Round(pos.y + 0.49999f), (int)from, (int)to, ref c);
            }

			// Draw all vertical grid lines
			float xstart = (float)(ltpos.x > General.Map.Config.LeftBoundary ? ltpos.x : General.Map.Config.LeftBoundary);
			float xend = (float)(rbpos.x < General.Map.Config.RightBoundary ? rbpos.x : General.Map.Config.RightBoundary);

			for(float x = xstart; x < xend + size; x += size) 
			{
				if(x > General.Map.Config.RightBoundary) x = General.Map.Config.RightBoundary;
				else if(x < General.Map.Config.LeftBoundary) x = General.Map.Config.LeftBoundary;

				float from = (float)(tlb.y < 0 ? 0 : tlb.y);
				float to = (float)(rbb.y > windowsize.Height ? windowsize.Height : rbb.y);

				pos.x = x;
				pos = pos.GetTransformed(translatex, translatey, scale, -scale);

                gridplotter.DrawGridLineV((int)pos.x, (int)Math.Round(from + 0.49999f), (int)Math.Round(to + 0.49999f), ref c);
            }
		}

		//mxd
		internal void GridVisibilityChanged()
		{
			lastgridscale = -1;
		}

		#endregion

		#region ================== Things

		// This makes vertices for a thing
		// Returns false when not on the screen
		private bool CreateThingBoxVerts(Thing t, ref FlatVertex[] verts, ref List<Line3D> bboxes, Dictionary<Thing, Vector3D> thingsByPosition, int offset, PixelColor c, byte bboxalpha)
		{
			if(t.Size * scale < MINIMUM_THING_RADIUS) return false; //mxd. Don't render tiny little things

			// Determine sizes
			float circlesize, bboxsize;

			if(t.FixedSize && scale > 1.0f)
			{
				circlesize = t.Size;
				bboxsize = -1;
			}
			else if(General.Settings.FixedThingsScale && t.Size * scale > FIXED_THING_SIZE)
			{
				circlesize = FIXED_THING_SIZE;
				bboxsize = t.Size * scale;
			}
			else
			{
				circlesize = t.Size * scale;
				bboxsize = -1;
			}

			float screensize = (bboxsize > 0 ? bboxsize : circlesize); //mxd
			
			// Transform to screen coordinates
			Vector2D screenpos = ((Vector2D)t.Position).GetTransformed(translatex, translatey, scale, -scale);
			
			// Check if the thing is actually on screen
			if(((screenpos.x + screensize) <= 0.0f) || ((screenpos.x - screensize) >= windowsize.Width) ||
			   ((screenpos.y + screensize) <= 0.0f) || ((screenpos.y - screensize) >= windowsize.Height))
				return false;

			// Get integral color
			int color = c.ToInt();

			// Setup fixed rect for circle
			verts[offset].x = (float)screenpos.x - circlesize;
			verts[offset].y = (float)screenpos.y - circlesize;
			verts[offset].c = color;
			verts[offset].u = 0f;
			verts[offset].v = 0f;
			offset++;
			verts[offset].x = (float)screenpos.x + circlesize;
			verts[offset].y = (float)screenpos.y - circlesize;
			verts[offset].c = color;
			verts[offset].u = 0.5f;
			verts[offset].v = 0f;
			offset++;
			verts[offset].x = (float)screenpos.x - circlesize;
			verts[offset].y = (float)screenpos.y + circlesize;
			verts[offset].c = color;
			verts[offset].u = 0f;
			verts[offset].v = 1f;
			offset++;
			verts[offset] = verts[offset - 2];
			offset++;
			verts[offset] = verts[offset - 2];
			offset++;
			verts[offset].x = (float)screenpos.x + circlesize;
			verts[offset].y = (float)screenpos.y + circlesize;
			verts[offset].c = color;
			verts[offset].u = 0.5f;
			verts[offset].v = 1f;

			//mxd. Add to list
			thingsByPosition.Add(t, screenpos);

			//mxd. Add bounding box?
			if(bboxsize > 0)
			{
				PixelColor boxcolor = c.WithAlpha(bboxalpha);

				Vector2D tl = new Vector2D(screenpos.x - bboxsize, screenpos.y - bboxsize);
				Vector2D tr = new Vector2D(screenpos.x + bboxsize, screenpos.y - bboxsize);
				Vector2D bl = new Vector2D(screenpos.x - bboxsize, screenpos.y + bboxsize);
				Vector2D br = new Vector2D(screenpos.x + bboxsize, screenpos.y + bboxsize);

				bboxes.Add(new Line3D(tl, tr, boxcolor, false));
				bboxes.Add(new Line3D(tr, br, boxcolor, false));
				bboxes.Add(new Line3D(bl, br, boxcolor, false));
				bboxes.Add(new Line3D(tl, bl, boxcolor, false));
			}

			// Done
			return true;
		}

		//mxd
		private void CreateThingArrowVerts(Thing t, ref FlatVertex[] verts, Vector3D screenpos, int offset) 
		{
			// Determine size
			float arrowsize;
			if(t.FixedSize && scale > 1.0f)
				arrowsize = t.Size * THING_ARROW_SIZE;
			else if(General.Settings.FixedThingsScale && t.Size * scale > FIXED_THING_SIZE)
				arrowsize = FIXED_THING_SIZE * THING_ARROW_SIZE;
			else
				arrowsize = t.Size * scale * THING_ARROW_SIZE;

			// Setup rotated rect for arrow
			float sinarrowsize = (float)Math.Sin(t.Angle + Angle2D.PI * 0.25f) * arrowsize;
			float cosarrowsize = (float)Math.Cos(t.Angle + Angle2D.PI * 0.25f) * arrowsize;

			// Sprite is not rendered?
			float ut, ub, ul, ur;
			if(screenpos.z < 0)
			{
				ul = 0.625f;
				ur = 0.874f;
				ut = -0.039f;
				ub = 0.46f;
			}
			else
			{
				ul = 0.501f;
				ur = 0.999f;
				ut = 0.001f;
				ub = 0.999f;
			}

			verts[offset].x = (float)screenpos.x + sinarrowsize;
			verts[offset].y = (float)screenpos.y + cosarrowsize;
			verts[offset].c = -1;
			verts[offset].u = ul;
			verts[offset].v = ut;
			offset++;
			verts[offset].x = (float)screenpos.x - cosarrowsize;
			verts[offset].y = (float)screenpos.y + sinarrowsize;
			verts[offset].c = -1;
			verts[offset].u = ur;
			verts[offset].v = ut;
			offset++;
			verts[offset].x = (float)screenpos.x + cosarrowsize;
			verts[offset].y = (float)screenpos.y - sinarrowsize;
			verts[offset].c = -1;
			verts[offset].u = ul;
			verts[offset].v = ub;
			offset++;
			verts[offset] = verts[offset - 2];
			offset++;
			verts[offset] = verts[offset - 2];
			offset++;
			verts[offset].x = (float)screenpos.x - sinarrowsize;
			verts[offset].y = (float)screenpos.y - cosarrowsize;
			verts[offset].c = -1;
			verts[offset].u = ur;
			verts[offset].v = ub;
		}

		//mxd
		private static void CreateThingSpriteVerts(Vector2D screenpos, float width, float height, ref FlatVertex[] verts, int offset, int color, bool mirror)
		{
			float ul = (mirror ? 1f : 0f);
			float ur = (mirror ? 0f : 1f);
			
			// Setup fixed rect for circle
			verts[offset].x = (float)screenpos.x - width;
			verts[offset].y = (float)screenpos.y - height;
			verts[offset].c = color;
			verts[offset].u = ul;
			verts[offset].v = 0;
			offset++;
			verts[offset].x = (float)screenpos.x + width;
			verts[offset].y = (float)screenpos.y - height;
			verts[offset].c = color;
			verts[offset].u = ur;
			verts[offset].v = 0;
			offset++;
			verts[offset].x = (float)screenpos.x - width;
			verts[offset].y = (float)screenpos.y + height;
			verts[offset].c = color;
			verts[offset].u = ul;
			verts[offset].v = 1;
			offset++;
			verts[offset] = verts[offset - 2];
			offset++;
			verts[offset] = verts[offset - 2];
			offset++;
			verts[offset].x = (float)screenpos.x + width;
			verts[offset].y = (float)screenpos.y + height;
			verts[offset].c = color;
			verts[offset].u = ur;
			verts[offset].v = 1;
		}
		
		// This draws a set of things
		private void RenderThingsBatch(ICollection<Thing> things, float alpha, bool fixedcolor, PixelColor c)
		{
			// Anything to render?
			if(things.Count > 0)
			{
				// Make alpha color
				Color4 alphacolor = new Color4(1.0f, 1.0f, 1.0f, alpha);
				bool isthingsmode = (General.Editing.Mode.GetType().Name == "ThingsMode");
				
				// Set renderstates for things rendering
				graphics.SetCullMode(Cull.None);
				graphics.SetZEnable(false);
				graphics.SetAlphaBlendEnable(true);
				graphics.SetSourceBlend(Blend.SourceAlpha);
				graphics.SetDestinationBlend(Blend.InverseSourceAlpha);
				graphics.SetAlphaTestEnable(false);
                graphics.SetUniform(UniformName.texturefactor, alphacolor);
				graphics.SetVertexBuffer(thingsvertices);
				
				// Set things texture
				graphics.SetTexture(General.Map.Data.ThingTexture.Texture); //mxd
				SetWorldTransformation(false);
                graphics.SetShader(ShaderName.things2d_thing);
				SetThings2DSettings(alpha);
				
				// Determine next lock size
				int locksize = (things.Count > THING_BUFFER_SIZE) ? THING_BUFFER_SIZE : things.Count;
				FlatVertex[] verts = new FlatVertex[THING_BUFFER_SIZE * 6];
				List<Line3D> bboxes = new List<Line3D>(locksize); //mxd

				//mxd
				Dictionary<int, List<Thing>> thingsByType = new Dictionary<int, List<Thing>>();
				Dictionary<int, List<Thing>> modelsByType = new Dictionary<int, List<Thing>>();
				Dictionary<Thing, Vector3D> thingsByPosition = new Dictionary<Thing, Vector3D>();

				// Go for all things
				int buffercount = 0;
				int totalcount = 0;
				foreach(Thing t in things)
				{
					//mxd. Highlighted thing should be rendered separately
					if(!fixedcolor && t.Highlighted) continue;
					
					// Collect models
					if(t.RenderMode == ThingRenderMode.MODEL || t.RenderMode == ThingRenderMode.VOXEL) 
					{
						if(!modelsByType.ContainsKey(t.Type)) modelsByType.Add(t.Type, new List<Thing>());
						modelsByType[t.Type].Add(t);
					}
					
					// Create vertices
					PixelColor tc = fixedcolor ? c : DetermineThingColor(t);
					byte bboxalpha = (byte)(alpha * ((!fixedcolor && !t.Selected && isthingsmode) ? 128 : 255));
					if(CreateThingBoxVerts(t, ref verts, ref bboxes, thingsByPosition, buffercount * 6, tc, bboxalpha))
					{
						buffercount++;

						//mxd
						if(!thingsByType.ContainsKey(t.Type)) thingsByType.Add(t.Type, new List<Thing>());
						thingsByType[t.Type].Add(t);
					}
					
					totalcount++;
					
					// Buffer filled?
					if(buffercount == locksize)
					{
						// Write to buffer
                        graphics.SetBufferSubdata(thingsvertices, verts, buffercount * 6);
						
						// Draw!
						graphics.Draw(PrimitiveType.TriangleList, 0, buffercount * 2);
						buffercount = 0;
						
						// Determine next lock size
						locksize = ((things.Count - totalcount) > THING_BUFFER_SIZE) ? THING_BUFFER_SIZE : (things.Count - totalcount);
					}
				}

				// Write to buffer
				if(buffercount > 0) graphics.SetBufferSubdata(thingsvertices, verts, buffercount * 6);
				
				// Draw what's still remaining
				if(buffercount > 0)
					graphics.Draw(PrimitiveType.TriangleList, 0, buffercount * 2);

				//mxd. Render sprites
				int selectionColor = General.Colors.Selection.ToInt();
                graphics.SetShader(ShaderName.things2d_sprite);

				foreach(KeyValuePair<int, List<Thing>> group in thingsByType)
				{
					// Skip when all things of this type will be rendered as models
					if((group.Value[0].RenderMode == ThingRenderMode.MODEL || group.Value[0].RenderMode == ThingRenderMode.VOXEL)
						&& (General.Settings.GZDrawModelsMode == ModelRenderMode.ALL)) continue;
					
					// Find thing information
					ThingTypeInfo info = General.Map.Data.GetThingInfo(group.Key);

					// Find sprite texture
					if(info.Sprite.Length == 0) continue;

					// Sort by sprite angle...
					Dictionary<int, List<Thing>> thingsbyangle = new Dictionary<int, List<Thing>>(group.Value.Count);
					if(info.SpriteFrame.Length == 8)
					{
						foreach(Thing t in group.Value)
						{
							// Choose which sprite angle to show
							int spriteangle = General.ClampAngle(-t.AngleDoom + 270) / 45;  // Convert to [0..7] range

							// Add to collection
							if(!thingsbyangle.ContainsKey(spriteangle)) thingsbyangle.Add(spriteangle, new List<Thing>());
							thingsbyangle[spriteangle].Add(t);
						}
					}
					else
					{
						thingsbyangle[0] = group.Value;
					}

					foreach(KeyValuePair<int, List<Thing>> framegroup in thingsbyangle)
					{
						SpriteFrameInfo sfi = info.SpriteFrame[framegroup.Key];
						ImageData sprite = General.Map.Data.GetSpriteImage(sfi.Sprite);
						if(sprite == null) continue;

						graphics.SetTexture(sprite.Texture);

						// Determine next lock size
						locksize = (framegroup.Value.Count > THING_BUFFER_SIZE) ? THING_BUFFER_SIZE : framegroup.Value.Count;
						verts = new FlatVertex[THING_BUFFER_SIZE * 6];

						// Go for all things
						buffercount = 0;
						totalcount = 0;

						foreach(Thing t in framegroup.Value)
						{
							if((t.RenderMode == ThingRenderMode.MODEL || t.RenderMode == ThingRenderMode.VOXEL)
								&& ((General.Settings.GZDrawModelsMode == ModelRenderMode.SELECTION && t.Selected) || (General.Settings.GZDrawModelsMode == ModelRenderMode.ACTIVE_THINGS_FILTER && alpha == 1.0f)))
								continue;

							bool forcespriterendering;
							float spritewidth, spriteheight, spritescale;

							// Determine sizes
							if(t.FixedSize && scale > 1.0f)
							{
								spritescale = 1.0f;
								forcespriterendering = true; // Always render sprite when thing size is affected by FixedSize setting
							}
							else if(General.Settings.FixedThingsScale && t.Size * scale > FIXED_THING_SIZE)
							{
								spritescale = FIXED_THING_SIZE / t.Size;
								forcespriterendering = true; // Always render sprite when thing size is affected by FixedThingsScale setting
							}
							else
							{
								spritescale = scale;
								forcespriterendering = false;
							}

							// Calculate scaled sprite size
							if(sprite.Width > sprite.Height)
							{
								spritewidth = (t.Size - THING_SPRITE_SHRINK) * spritescale;
								spriteheight = spritewidth * ((float)sprite.Height / sprite.Width);
							}
							else if(sprite.Width < sprite.Height)
							{
								spriteheight = (t.Size - THING_SPRITE_SHRINK) * spritescale;
								spritewidth = spriteheight * ((float)sprite.Width / sprite.Height);
							}
							else
							{
								spritewidth = (t.Size - THING_SPRITE_SHRINK) * spritescale;
								spriteheight = spritewidth;
							}

							float spritesize = Math.Max(spritewidth, spriteheight);

							if(!forcespriterendering && spritesize < MINIMUM_SPRITE_RADIUS)
							{
								// Hackish way to tell arrow rendering code to draw bigger arrow...
								Vector3D v = thingsByPosition[t];
								v.z = -1;
								thingsByPosition[t] = v;

								// Don't render tiny little sprites
								continue;
							}

							CreateThingSpriteVerts(thingsByPosition[t], spritewidth, spriteheight, ref verts, buffercount * 6, (t.Selected ? selectionColor : 0xFFFFFF), sfi.Mirror);
							buffercount++;
							totalcount++;

							// Buffer filled?
							if(buffercount == locksize)
							{
								// Write to buffer
                                graphics.SetBufferSubdata(thingsvertices, verts, buffercount * 6);

								// Draw!
								graphics.Draw(PrimitiveType.TriangleList, 0, buffercount * 2);

								buffercount = 0;

								// Determine next lock size
								locksize = ((framegroup.Value.Count - totalcount) > THING_BUFFER_SIZE) ? THING_BUFFER_SIZE : (framegroup.Value.Count - totalcount);
							}
						}

						// Write to buffer
                        graphics.SetBufferSubdata(thingsvertices, verts, buffercount * 6);

						// Draw what's still remaining
						if(buffercount > 0) graphics.Draw(PrimitiveType.TriangleList, 0, buffercount * 2);
					}
				}

				//mxd. Render thing arrows
				graphics.SetTexture(General.Map.Data.ThingTexture.Texture);
                graphics.SetShader(ShaderName.things2d_thing);

				// Determine next lock size
				locksize = (thingsByPosition.Count > THING_BUFFER_SIZE) ? THING_BUFFER_SIZE : thingsByPosition.Count;
				verts = new FlatVertex[THING_BUFFER_SIZE * 6];

				// Go for all things
				buffercount = 0;
				totalcount = 0;

				foreach(KeyValuePair<Thing, Vector3D> group in thingsByPosition) 
				{
					if(!group.Key.IsDirectional) continue;

					CreateThingArrowVerts(group.Key, ref verts, group.Value, buffercount * 6);
					buffercount++;
					totalcount++;

					// Buffer filled?
					if(buffercount == locksize) 
					{
						// Write to buffer
                        graphics.SetBufferSubdata(thingsvertices, verts, buffercount * 6);

						// Draw!
						graphics.Draw(PrimitiveType.TriangleList, 0, buffercount * 2);
						buffercount = 0;

						// Determine next lock size
						locksize = ((thingsByPosition.Count - totalcount) > THING_BUFFER_SIZE) ? THING_BUFFER_SIZE : (thingsByPosition.Count - totalcount);
					}
				}

				// Write to buffer
				if(buffercount > 0) graphics.SetBufferSubdata(thingsvertices, verts, buffercount * 6);

				// Draw what's still remaining
				if(buffercount > 0) 
					graphics.Draw(PrimitiveType.TriangleList, 0, buffercount * 2);

				//mxd. Render models
				if(General.Settings.GZDrawModelsMode != ModelRenderMode.NONE) 
				{
					// Set renderstates for rendering
					graphics.SetAlphaBlendEnable(false);
					graphics.SetUniform(UniformName.texturefactor, new Color4(1f, 1f, 1f, 1f));
					graphics.SetFillMode(FillMode.Wireframe);

                    graphics.SetShader(ShaderName.things2d_fill);

					Color4 cSelection = General.Colors.Selection.ToColorValue();
					Color4 cWire = ((c.ToInt() == General.Colors.Highlight.ToInt()) ? General.Colors.Highlight.ToColorValue() : General.Colors.ModelWireframe.ToColorValue());

					cSelection.Alpha = ((alpha < 1.0f) ? alpha * 0.25f : 0.6f);
					cWire.Alpha = cSelection.Alpha;

					Matrix viewscale = Matrix.Scaling(scale, -scale, 0.0f);

					foreach(KeyValuePair<int, List<Thing>> group in modelsByType)
					{
						ModelData mde = General.Map.Data.ModeldefEntries[group.Key];
						foreach(Thing t in group.Value) 
						{
							if((General.Settings.GZDrawModelsMode == ModelRenderMode.SELECTION && !t.Selected) || (General.Settings.GZDrawModelsMode == ModelRenderMode.ACTIVE_THINGS_FILTER && alpha < 1.0f)) continue;
							Vector2D screenpos = ((Vector2D)t.Position).GetTransformed(translatex, translatey, scale, -scale);
							double modelScale = scale * t.ActorScale.Width * t.ScaleX;

							//should we render this model?
							if(((screenpos.x + mde.Model.Radius * modelScale) <= 0.0f) || ((screenpos.x - mde.Model.Radius * modelScale) >= windowsize.Width) ||
							((screenpos.y + mde.Model.Radius * modelScale) <= 0.0f) || ((screenpos.y - mde.Model.Radius * modelScale) >= windowsize.Height))
								continue;

							graphics.SetUniform(UniformName.FillColor, (t.Selected ? cSelection : cWire));

							// Set transform settings
							double sx = t.ScaleX * t.ActorScale.Width;
							double sy = t.ScaleY * t.ActorScale.Height;
							
							Matrix modelscale = Matrix.Scaling((float)sx, (float)sx, (float)sy);
							Matrix rotation = Matrix.RotationY((float)-t.RollRad) * Matrix.RotationX((float)-t.PitchRad) * Matrix.RotationZ((float)t.Angle);
							Matrix position = Matrix.Translation((float)screenpos.x, (float)screenpos.y, 0.0f);
							Matrix world = General.Map.Data.ModeldefEntries[t.Type].Transform * modelscale * rotation * viewscale * position;

							SetThings2DTransformSettings(world);

                            // Draw
							foreach(Mesh mesh in mde.Model.Meshes) mesh.Draw(graphics);
						}
					}

					//Done with this pass
					graphics.SetFillMode(FillMode.Solid);
				}

				//mxd. Render thing boxes
				RenderArrows(bboxes, false);
			}
		}
		
		// This adds a thing in the things buffer for rendering
		public void RenderThing(Thing t, PixelColor c, float alpha)
		{
			List<Thing> things = new List<Thing>(1);
			things.Add(t);
			RenderThingsBatch(things, alpha, true, c);
		}
		
		// This adds a thing in the things buffer for rendering
		public void RenderThingSet(ICollection<Thing> things, float alpha)
		{
			RenderThingsBatch(things, alpha, false, new PixelColor());
		}
		
		#endregion

		#region ================== Surface

		// This redraws the surface
		public void RedrawSurface()
		{
			if(renderlayer != RenderLayers.None) return; //mxd
			renderlayer = RenderLayers.Surface;

            // Recreate render targets if the window was resized
            if (windowsize.Width != graphics.RenderTarget.ClientSize.Width || windowsize.Height != graphics.RenderTarget.ClientSize.Height)
                CreateRendertargets();

			// Rendertargets available?
			if(surfacetex != null)
			{
				// Set the rendertarget to the surface texture
                graphics.StartRendering(true, General.Colors.Background.WithAlpha(0).ToColorValue(), surfacetex, false);

				// Set transformations
				UpdateTransformations();

				// Set states
				graphics.SetCullMode(Cull.None);
				graphics.SetZEnable(false);
				graphics.SetAlphaBlendEnable(false);
				graphics.SetAlphaTestEnable(false);
				graphics.SetUniform(UniformName.texturefactor, new Color4(1f, 1f, 1f, 1f));
                SetWorldTransformation(true);
				SetDisplay2DSettings(1f, 1f, 0f, 1f, General.Settings.ClassicBilinear);
					
				// Prepare for rendering
				switch(viewmode)
				{
					case ViewMode.Brightness:
						surfaces.RenderSectorBrightness(yviewport);
						surfaces.RenderSectorSurfaces(graphics);
						break;
							
					case ViewMode.FloorTextures:
						surfaces.RenderSectorFloors(yviewport);
						surfaces.RenderSectorSurfaces(graphics);
						break;
							
					case ViewMode.CeilingTextures:
						surfaces.RenderSectorCeilings(yviewport);
						surfaces.RenderSectorSurfaces(graphics);
						break;
				}
			}
			
			// Done
			Finish();
		}

		#endregion

		#region ================== Overlay

		// This renders geometry
		// The geometry must be a triangle list
		public void RenderGeometry(FlatVertex[] vertices, ImageData texture, bool transformcoords)
		{
			if(vertices.Length > 0)
			{
				Texture t;
				
				if(texture != null)
				{
					t = texture.Texture;
				}
				else
				{
					t = General.Map.Data.WhiteTexture.Texture;
				}

				// Set renderstates for rendering
				graphics.SetCullMode(Cull.None);
				graphics.SetZEnable(false);
				graphics.SetAlphaBlendEnable(false);
				graphics.SetAlphaTestEnable(false);
				graphics.SetUniform(UniformName.texturefactor, new Color4(1f, 1f, 1f, 1f));
                graphics.SetShader(ShaderName.display2d_normal);
				graphics.SetTexture(t);
				SetWorldTransformation(transformcoords);
				SetDisplay2DSettings(1f, 1f, 0f, 1f, General.Settings.ClassicBilinear);
				
				// Draw
				graphics.Draw(PrimitiveType.TriangleList, 0, vertices.Length / 3, vertices);
			}
		}

		//mxd
		public void RenderHighlight(FlatVertex[] vertices, int color) 
		{
			if(vertices.Length < 3) return;

			// Set renderstates for rendering
			graphics.SetCullMode(Cull.None);
			graphics.SetZEnable(false);
			graphics.SetAlphaBlendEnable(false);
			graphics.SetAlphaTestEnable(false);
			graphics.SetUniform(UniformName.texturefactor, new Color4(1f, 1f, 1f, 1f));

            SetWorldTransformation(true);
			graphics.SetUniform(UniformName.FillColor, new Color4(color));
			SetThings2DSettings(1.0f);

            // Draw
            graphics.SetShader(ShaderName.things2d_fill);
			graphics.Draw(PrimitiveType.TriangleList, 0, vertices.Length / 3, vertices);
		}

		//mxd. This renders text (DB2 compatibility)
		[Obsolete("Method is deprecated, please use RenderText(ITextLabel label) method instead.")]
		public void RenderText(TextLabel label){ RenderText((ITextLabel)label); }

		// This renders text
		public void RenderText(ITextLabel label)
		{
			//mxd. Update the text if needed
			label.Update(graphics, translatex, translatey, scale, -scale);
			if(label.SkipRendering) return;
			
			// Set renderstates for rendering
			graphics.SetCullMode(Cull.None);
			graphics.SetZEnable(false);
			graphics.SetAlphaBlendEnable(true);
			graphics.SetAlphaTestEnable(false);
			graphics.SetUniform(UniformName.texturefactor, new Color4(1f, 1f, 1f, 1f));
            graphics.SetShader(ShaderName.display2d_normal);
			graphics.SetTexture(label.Texture);
			SetWorldTransformation(false);
			SetDisplay2DSettings(1f, 1f, 0f, 1f, false);
			graphics.SetVertexBuffer(label.VertexBuffer);

			// Draw
			graphics.Draw(PrimitiveType.TriangleStrip, 0, 2);
		}

		//mxd. This renders text
		public void RenderText(IList<ITextLabel> labels)
		{
			// Update labels
			int skipped = 0;
			foreach(ITextLabel label in labels)
			{
				// Update the text if needed
				label.Update(graphics, translatex, translatey, scale, -scale);
				if(label.SkipRendering) skipped++;
			}

			if(labels.Count == skipped) return;
			
			// Set renderstates for rendering
			graphics.SetCullMode(Cull.None);
			graphics.SetZEnable(false);
			graphics.SetAlphaBlendEnable(true);
			graphics.SetAlphaTestEnable(false);
			graphics.SetUniform(UniformName.texturefactor, new Color4(1f, 1f, 1f, 1f));
            SetWorldTransformation(false);
            graphics.SetShader(ShaderName.display2d_normal);
			SetDisplay2DSettings(1f, 1f, 0f, 1f, false);
			
			foreach(ITextLabel label in labels)
			{
				// Text is created?
				if(!label.SkipRendering)
				{
					graphics.SetTexture(label.Texture);
					graphics.SetVertexBuffer(label.VertexBuffer);

					// Draw
					graphics.Draw(PrimitiveType.TriangleStrip, 0, 2);
				}
			}
		}
		
		// This renders a rectangle with given border size and color
		public void RenderRectangle(RectangleF rect, float bordersize, PixelColor c, bool transformrect)
		{
			FlatQuad[] quads = new FlatQuad[4];
			
			/*
			 * Rectangle setup:
			 * 
			 *  --------------------------
			 *  |___________0____________|
			 *  |  |                  |  |
			 *  |  |                  |  |
			 *  |  |                  |  |
			 *  | 2|                  |3 |
			 *  |  |                  |  |
			 *  |  |                  |  |
			 *  |__|__________________|__|
			 *  |           1            |
			 *  --------------------------
			 * 
			 * Don't you just love ASCII art?
			 */
			
			// Calculate positions
			Vector2D lt = new Vector2D(rect.Left, rect.Top);
			Vector2D rb = new Vector2D(rect.Right, rect.Bottom);
			if(transformrect)
			{
				lt = lt.GetTransformed(translatex, translatey, scale, -scale);
				rb = rb.GetTransformed(translatex, translatey, scale, -scale);
			}
			
			// Make quads
			quads[0] = new FlatQuad(PrimitiveType.TriangleStrip, (float)lt.x, (float)lt.y, (float)rb.x, (float)lt.y - bordersize);
			quads[1] = new FlatQuad(PrimitiveType.TriangleStrip, (float)lt.x, (float)rb.y + bordersize, (float)rb.x, (float)rb.y);
			quads[2] = new FlatQuad(PrimitiveType.TriangleStrip, (float)lt.x, (float)lt.y - bordersize, (float)lt.x + bordersize, (float)rb.y + bordersize);
			quads[3] = new FlatQuad(PrimitiveType.TriangleStrip, (float)rb.x - bordersize, (float)lt.y - bordersize, (float)rb.x, (float)rb.y + bordersize);
			quads[0].SetColors(c.ToInt());
			quads[1].SetColors(c.ToInt());
			quads[2].SetColors(c.ToInt());
			quads[3].SetColors(c.ToInt());
			
			// Set renderstates for rendering
			graphics.SetCullMode(Cull.None);
			graphics.SetZEnable(false);
			graphics.SetAlphaBlendEnable(false);
			graphics.SetAlphaTestEnable(false);
			graphics.SetUniform(UniformName.texturefactor, new Color4(1f, 1f, 1f, 1f));
            SetWorldTransformation(false);
            graphics.SetShader(ShaderName.display2d_normal);
			graphics.SetTexture(General.Map.Data.WhiteTexture.Texture);
			SetDisplay2DSettings(1f, 1f, 0f, 1f, General.Settings.ClassicBilinear);
			
			// Draw
			quads[0].Render(graphics);
			quads[1].Render(graphics);
			quads[2].Render(graphics);
			quads[3].Render(graphics);
		}

		// This renders a filled rectangle with given color
		public void RenderRectangleFilled(RectangleF rect, PixelColor c, bool transformrect)
		{
			// Calculate positions
			Vector2D lt = new Vector2D(rect.Left, rect.Top);
			Vector2D rb = new Vector2D(rect.Right, rect.Bottom);
			if(transformrect)
			{
				lt = lt.GetTransformed(translatex, translatey, scale, -scale);
				rb = rb.GetTransformed(translatex, translatey, scale, -scale);
			}

			// Make quad
			FlatQuad quad = new FlatQuad(PrimitiveType.TriangleStrip, (float)lt.x, (float)lt.y, (float)rb.x, (float)rb.y);
			quad.SetColors(c.ToInt());
			
			// Set renderstates for rendering
			graphics.SetCullMode(Cull.None);
			graphics.SetZEnable(false);
			graphics.SetAlphaBlendEnable(false);
			graphics.SetAlphaTestEnable(false);
			graphics.SetUniform(UniformName.texturefactor, new Color4(1f, 1f, 1f, 1f));
            SetWorldTransformation(false);
            graphics.SetShader(ShaderName.display2d_normal);
			graphics.SetTexture(General.Map.Data.WhiteTexture.Texture);
			SetDisplay2DSettings(1f, 1f, 0f, 1f, General.Settings.ClassicBilinear);

			// Draw
			quad.Render(graphics);
		}

		// This renders a filled rectangle with given color
		public void RenderRectangleFilled(RectangleF rect, PixelColor c, bool transformrect, ImageData texture)
		{
			// Calculate positions
			Vector2D lt = new Vector2D(rect.Left, rect.Top);
			Vector2D rb = new Vector2D(rect.Right, rect.Bottom);
			if(transformrect)
			{
				lt = lt.GetTransformed(translatex, translatey, scale, -scale);
				rb = rb.GetTransformed(translatex, translatey, scale, -scale);
			}

			// Make quad
			FlatQuad quad = new FlatQuad(PrimitiveType.TriangleStrip, (float)lt.x, (float)lt.y, (float)rb.x, (float)rb.y);
			quad.SetColors(c.ToInt());

			// Set renderstates for rendering
			graphics.SetCullMode(Cull.None);
			graphics.SetZEnable(false);
			graphics.SetAlphaBlendEnable(false);
			graphics.SetAlphaTestEnable(false);
			graphics.SetUniform(UniformName.texturefactor, new Color4(1f, 1f, 1f, 1f));
            SetWorldTransformation(false);
            graphics.SetShader(ShaderName.display2d_normal);
			graphics.SetTexture(texture.Texture);
			SetDisplay2DSettings(1f, 1f, 0f, 1f, General.Settings.ClassicBilinear);

			// Draw
			quad.Render(graphics);
		}

		//mxd
		public void RenderArrows(ICollection<Line3D> lines) { RenderArrows(lines, true); }
		public void RenderArrows(ICollection<Line3D> lines, bool transformcoords) 
		{
			if(lines.Count == 0) return;
			int pointscount = 0;

			// Translate to screen coords, determine renderability
			foreach(Line3D line in lines)
			{
				// Calculate screen positions?
				if(transformcoords)
				{
					line.Start2D = ((Vector2D)line.Start).GetTransformed(translatex, translatey, scale, -scale); //start
					line.End2D = ((Vector2D)line.End).GetTransformed(translatex, translatey, scale, -scale); //end
				}

				float maxx = (float)Math.Max(line.Start2D.x, line.End2D.x);
				float minx = (float)Math.Min(line.Start2D.x, line.End2D.x);
				float maxy = (float)Math.Max(line.Start2D.y, line.End2D.y);
				float miny = (float)Math.Min(line.Start2D.y, line.End2D.y);

				// Too small / not on screen?
				if(((line.End2D - line.Start2D).GetLengthSq() < MINIMUM_SPRITE_RADIUS) || ((maxx <= 0.0f) || (minx >= windowsize.Width) || (maxy <= 0.0f) || (miny >= windowsize.Height)))
				{
					line.SkipRendering = true;
				}
				else
				{
					pointscount += (line.RenderArrowhead ? 6 : 2); // 4 extra points for the arrowhead
					line.SkipRendering = false;
				}
			}

			// Anything to do?
			if(pointscount < 2) return;

			FlatVertex[] verts = new FlatVertex[pointscount];
			float scaler = 16f / scale;

			// Create verts array
			pointscount = 0;
			foreach(Line3D line in lines)
			{
				if(line.SkipRendering) continue;
				int color = line.Color.ToInt();

				// Add regular points
				verts[pointscount].x = (float)line.Start2D.x;
				verts[pointscount].y = (float)line.Start2D.y;
				verts[pointscount].c = color;
				pointscount++;

				verts[pointscount].x = (float)line.End2D.x;
				verts[pointscount].y = (float)line.End2D.y;
				verts[pointscount].c = color;
				pointscount++;

				// Add arrowhead
				if(line.RenderArrowhead)
				{
					double angle = line.GetAngle();
					Vector2D a1 = new Vector2D(line.End.x - scaler * Math.Sin(angle - 0.46f), line.End.y + scaler * Math.Cos(angle - 0.46f)).GetTransformed(translatex, translatey, scale, -scale); //arrowhead end 1
					Vector2D a2 = new Vector2D(line.End.x - scaler * Math.Sin(angle + 0.46f), line.End.y + scaler * Math.Cos(angle + 0.46f)).GetTransformed(translatex, translatey, scale, -scale); //arrowhead end 2
					
					verts[pointscount] = verts[pointscount - 1];
					verts[pointscount + 1].x = (float)a1.x;
					verts[pointscount + 1].y = (float)a1.y;
					verts[pointscount + 1].c = color;

					verts[pointscount + 2] = verts[pointscount - 1];
					verts[pointscount + 3].x = (float)a2.x;
					verts[pointscount + 3].y = (float)a2.y;
					verts[pointscount + 3].c = color;

					pointscount += 4;
				}
			}

			// Write to buffer
			VertexBuffer vb = new VertexBuffer();
			graphics.SetBufferData(vb, verts);

			// Set renderstates for rendering
			graphics.SetCullMode(Cull.None);
			graphics.SetZEnable(false);
			graphics.SetAlphaBlendEnable(false);
			graphics.SetAlphaTestEnable(false);
			graphics.SetUniform(UniformName.texturefactor, new Color4(1f, 1f, 1f, 1f));
            SetWorldTransformation(false);
            graphics.SetShader(ShaderName.display2d_normal);
			graphics.SetTexture(General.Map.Data.WhiteTexture.Texture);
			SetDisplay2DSettings(1f, 1f, 0f, 1f, General.Settings.ClassicBilinear);

			// Draw
			graphics.SetVertexBuffer(vb);
			graphics.Draw(PrimitiveType.LineList, 0, pointscount / 2);
			vb.Dispose();
		}

		// This renders a line with given color
		public void RenderLine(Vector2D start, Vector2D end, float thickness, PixelColor c, bool transformcoords)
		{
			FlatVertex[] verts = new FlatVertex[4];
			
			// Calculate positions
			if(transformcoords)
			{
				start = start.GetTransformed(translatex, translatey, scale, -scale);
				end = end.GetTransformed(translatex, translatey, scale, -scale);
			}

			// Calculate offsets
			Vector2D delta = end - start;
			Vector2D dn = delta.GetNormal() * thickness;
			
			// Make vertices
			verts[0].x = (float)(start.x - dn.x + dn.y);
			verts[0].y = (float)(start.y - dn.y - dn.x);
			verts[0].z = 0.0f;
			verts[0].c = c.ToInt();
			verts[1].x = (float)(start.x - dn.x - dn.y);
			verts[1].y = (float)(start.y - dn.y + dn.x);
			verts[1].z = 0.0f;
			verts[1].c = c.ToInt();
			verts[2].x = (float)(end.x + dn.x + dn.y);
			verts[2].y = (float)(end.y + dn.y - dn.x);
			verts[2].z = 0.0f;
			verts[2].c = c.ToInt();
			verts[3].x = (float)(end.x + dn.x - dn.y);
			verts[3].y = (float)(end.y + dn.y + dn.x);
			verts[3].z = 0.0f;
			verts[3].c = c.ToInt();
			
			// Set renderstates for rendering
			graphics.SetCullMode(Cull.None);
			graphics.SetZEnable(false);
			graphics.SetAlphaBlendEnable(false);
			graphics.SetAlphaTestEnable(false);
			graphics.SetUniform(UniformName.texturefactor, new Color4(1f, 1f, 1f, 1f));
            SetWorldTransformation(false);
            graphics.SetShader(ShaderName.display2d_normal);
			graphics.SetTexture(General.Map.Data.WhiteTexture.Texture);
			SetDisplay2DSettings(1f, 1f, 0f, 1f, General.Settings.ClassicBilinear);

			// Draw
			graphics.Draw(PrimitiveType.TriangleStrip, 0, 2, verts);
		}

		#endregion

		#region ================== Geometry

		// This renders the linedefs of a sector with special color
		public void PlotSector(Sector s, PixelColor c)
		{
			// Go for all sides in the sector
			foreach(Sidedef sd in s.Sidedefs)
			{
				// Render this linedef
				PlotLinedef(sd.Line, c);

				// Render the two vertices on top
				PlotVertex(sd.Line.Start, DetermineVertexColor(sd.Line.Start));
				PlotVertex(sd.Line.End, DetermineVertexColor(sd.Line.End));
			}
		}

		// This renders the linedefs of a sector
		public void PlotSector(Sector s)
		{
			// Go for all sides in the sector
			foreach(Sidedef sd in s.Sidedefs)
			{
				// Render this linedef
				PlotLinedef(sd.Line, DetermineLinedefColor(sd.Line));

				// Render the two vertices on top
				PlotVertex(sd.Line.Start, DetermineVertexColor(sd.Line.Start));
				PlotVertex(sd.Line.End, DetermineVertexColor(sd.Line.End));
			}
		}

		// This renders a simple line
		public void PlotLine(Vector2D start, Vector2D end, PixelColor c) { PlotLine(start, end, c, 0.0625f); }
		public void PlotLine(Vector2D start, Vector2D end, PixelColor c, float lengthscaler)
		{
			// Transform coordinates
			Vector2D v1 = start.GetTransformed(translatex, translatey, scale, -scale);
			Vector2D v2 = end.GetTransformed(translatex, translatey, scale, -scale);
			
			//mxd. Should we bother?
			if((v2 - v1).GetLengthSq() < linenormalsize * lengthscaler) return;

			// Draw line
			plotter.DrawLineSolid((int)v1.x, TransformY((int)v1.y), (int)v2.x, TransformY((int)v2.y), ref c);
		}
		
        private Vector2D TransformY(Vector2D v)
        {
            return new Vector2D(v.x, TransformY(v.y));
        }

        private double TransformY(double y)
        {
            return windowsize.Height - y;
        }

        private int TransformY(int y)
        {
            return windowsize.Height - y;
        }

		// This renders a single linedef
		public void PlotLinedef(Linedef l, PixelColor c)
		{
			// Transform vertex coordinates
			Vector2D v1 = l.Start.Position.GetTransformed(translatex, translatey, scale, -scale);
			Vector2D v2 = l.End.Position.GetTransformed(translatex, translatey, scale, -scale);

			//mxd. Should we bother?
			double lengthsq = (v2 - v1).GetLengthSq();
			if(lengthsq < minlinelength) return; //mxd

			// Draw line. mxd: added 3d-floor indication
			if(l.ExtraFloorFlag && General.Settings.GZMarkExtraFloors)
				plotter.DrawLine3DFloor((int)v1.x, TransformY((int)v1.y), (int)v2.x, TransformY((int)v2.y), ref c, General.Colors.ThreeDFloor);
			else
				plotter.DrawLineSolid((int)v1.x, TransformY((int)v1.y), (int)v2.x, TransformY((int)v2.y), ref c);

			//mxd. Should we bother?
			if(lengthsq < minlinenormallength) return; //mxd

			// Calculate normal indicator
			double mx = (v2.x - v1.x) * 0.5f;
			double my = (v2.y - v1.y) * 0.5f;

			// Draw normal indicator
			plotter.DrawLineSolid((int)(v1.x + mx), TransformY((int)(v1.y + my)),
								  (int)((v1.x + mx) - (my * l.LengthInv) * linenormalsize),
								  TransformY((int)((v1.y + my) + (mx * l.LengthInv) * linenormalsize)), ref c);
		}
		
		// This renders a set of linedefs
		public void PlotLinedefSet(ICollection<Linedef> linedefs)
		{
			// Go for all linedefs
			foreach(Linedef l in linedefs)
			{
				// Transform vertex coordinates
				Vector2D v1 = l.Start.Position.GetTransformed(translatex, translatey, scale, -scale);
				Vector2D v2 = l.End.Position.GetTransformed(translatex, translatey, scale, -scale);

				//mxd. Should we bother?
				double lengthsq = (v2 - v1).GetLengthSq();
				if(lengthsq < minlinelength) continue; //mxd

				// Determine color
				PixelColor c = DetermineLinedefColor(l);

				// Draw line. mxd: added 3d-floor indication
				if(l.ExtraFloorFlag && General.Settings.GZMarkExtraFloors)
					plotter.DrawLine3DFloor((int)v1.x, TransformY((int)v1.y), (int)v2.x, TransformY((int)v2.y), ref c, General.Colors.ThreeDFloor);
				else
					plotter.DrawLineSolid((int)v1.x, TransformY((int)v1.y), (int)v2.x, TransformY((int)v2.y), ref c);

				//mxd. Should we bother?
				if(lengthsq < minlinenormallength) continue; //mxd

				// Calculate normal indicator
				double mx = (v2.x - v1.x) * 0.5f;
				double my = (v2.y - v1.y) * 0.5f;

				// Draw normal indicator
				plotter.DrawLineSolid((int)(v1.x + mx), TransformY((int)(v1.y + my)),
									  (int)((v1.x + mx) - (my * l.LengthInv) * linenormalsize),
									  TransformY((int)((v1.y + my) + (mx * l.LengthInv) * linenormalsize)), ref c);
			}
		}

		// This renders a single vertex
		public void PlotVertex(Vertex v, int colorindex, bool checkMode = true)
		{
			if (checkMode && !ShouldRenderVertices)
			{
				return;
			}
			// Transform vertex coordinates
			Vector2D nv = v.Position.GetTransformed(translatex, translatey, scale, -scale);

			// Draw pixel here
			plotter.DrawVertexSolid((int)nv.x, TransformY((int)nv.y), vertexsize, ref General.Colors.Colors[colorindex], ref General.Colors.BrightColors[colorindex], ref General.Colors.DarkColors[colorindex]);
		}

		// This renders a single vertex at specified coordinates
		public void PlotVertexAt(Vector2D v, int colorindex, bool checkMode = true)
		{
			if (checkMode && !ShouldRenderVertices)
			{
				return;
			}
			
			// Transform vertex coordinates
			Vector2D nv = v.GetTransformed(translatex, translatey, scale, -scale);

			// Draw pixel here
			plotter.DrawVertexSolid((int)nv.x, TransformY((int)nv.y), vertexsize, ref General.Colors.Colors[colorindex], ref General.Colors.BrightColors[colorindex], ref General.Colors.DarkColors[colorindex]);
		}
		
		// This renders a set of vertices
		public void PlotVerticesSet(ICollection<Vertex> vertices, bool checkMode = true)
		{
			if (checkMode && !ShouldRenderVertices)
			{
				return;
			}
			
			// Go for all vertices
			foreach(Vertex v in vertices) PlotVertex(v, DetermineVertexColor(v));
		}

		#endregion
	}
}
