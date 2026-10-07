
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
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Drawing;
using CodeImp.DoomBuilder.Geometry;
using Font = System.Drawing.Font;

#endregion

namespace CodeImp.DoomBuilder.Rendering
{
	public interface ITextLabel //mxd. Methods and properties required to render a textlabel
	{
		// Required to render text label
		bool SkipRendering { get; }
		Texture Texture { get; } 
		VertexBuffer VertexBuffer { get; }

		// Access/setup
		Font Font { get; }
		string Text { get; set; }
		PixelColor Color { get; set; }
		PixelColor BackColor { get; set; }

		void Update(RenderDevice graphics, float translatex, float translatey, float scalex, float scaley);
	}
	
	public class TextLabel : IDisposable, IRenderResource, ITextLabel
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		// The text is stored as a polygon in a vertex buffer
		private VertexBuffer textbuffer;
		private Texture texture;
		private Font font; //mxd
		
		// Text settings
		private string text;
		private Vector2D location; //mxd
		private bool transformcoords;
		private PixelColor color;
		private PixelColor backcolor;
		private TextAlignmentX alignx;
		private TextAlignmentY aligny;
		private bool drawbg; //mxd
		
		//mxd. Label image settings...
		private SizeF textsize;
		private Size texturesize;
		private RectangleF textrect;
		private RectangleF bgrect;
		private PointF textorigin;
		
		// This keeps track if changes were made
		private bool updateneeded;
		private bool textureupdateneeded; //mxd
		private float lasttranslatex = float.MinValue;
		private float lasttranslatey;
		private float lastscalex;
		private float lastscaley;

		//mxd. Rendering
		private bool skiprendering;

		//mxd. Compatibility
		private float scale;
		
		// Disposing
		private bool isdisposed;

        // ano - static stuff to prevent often alloc/dealloc performance hits
        private static StringFormat strFormat;
        private static SolidBrush brush;
        private static Pen pen;

		#endregion

		#region ================== Properties

		// Properties
		public Vector2D Location { get { return location; } set { location = value; updateneeded = true; } } //mxd
		public string Text { get { return text; } set { if(text != value) { text = value; textsize = Size.Empty; textureupdateneeded = true; } } }
		public Font Font { get { return font; } set { font.Dispose(); font = value; textsize = Size.Empty; textureupdateneeded = true; } } //mxd
		public bool TransformCoords { get { return transformcoords; } set { transformcoords = value; updateneeded = true; } }
		public SizeF TextSize { get { if(textureupdateneeded) Update(General.Map.Graphics, General.Map.Renderer2D.TranslateX, General.Map.Renderer2D.TranslateY, General.Map.Renderer2D.Scale, -General.Map.Renderer2D.Scale); return textsize; } }
		public TextAlignmentX AlignX { get { return alignx; } set { alignx = value; updateneeded = true; } }
		public TextAlignmentY AlignY { get { return aligny; } set { aligny = value; updateneeded = true; } }
		public PixelColor Color { get { return color; } set { if(!color.Equals(value)) { color = value; textureupdateneeded = true; } } }
		public PixelColor BackColor { get { return backcolor; } set { if(!backcolor.Equals(value)) { backcolor = value; textureupdateneeded = true; } } }
		public bool DrawBackground { get { return drawbg; } set { if(drawbg != value) { drawbg = value; textureupdateneeded = true; } } } //mxd
		public Texture Texture { get { return texture; } } //mxd
		public VertexBuffer VertexBuffer { get { return textbuffer; } }
		public bool SkipRendering { get { return skiprendering; } } //mxd

		//mxd. Compatibility settings
		[Obsolete("Backcolor property is deprecated, please use BackColor property instead.")]
		public PixelColor Backcolor { get { return BackColor; } set { BackColor = value.WithAlpha(128); } }

		[Obsolete("Scale property is deprecated, please assign the font directly using Font property instead.")]
		public float Scale
		{
			get { return scale; } 
			set
			{
				scale = value;
				font.Dispose();
				font = new Font(GetFontFamily(), (float)Math.Round(scale * 0.75f), (General.Settings.TextLabelFontBold ? FontStyle.Bold : FontStyle.Regular));
				textsize = Size.Empty; 
				textureupdateneeded = true;
			} 
		}

		[Obsolete("Rectangle property is deprecated, please use Location property instead.")]
		public RectangleF Rectangle { get { return new RectangleF((float)location.x, (float)location.y, 0f, 0f); } set { location = new Vector2D(value.X, value.Y); updateneeded = true; } }

		// Disposing
		public bool IsDisposed { get { return isdisposed; } }

		#endregion

		#region ================== Constructor / Disposer

        private FontFamily GetFontFamily()
        {
            FontFamily ff;
            try
            {
                ff = new FontFamily(General.Settings.TextLabelFontName);
            }
            catch (Exception)
            {
                ff = General.MainWindow.Font.FontFamily;
            }
            return ff;
        }

		// Constructor
		public TextLabel()
		{
			// Initialize
			this.text = "";
			this.font = new Font(GetFontFamily(), General.Settings.TextLabelFontSize, (General.Settings.TextLabelFontBold ? FontStyle.Bold : FontStyle.Regular)); //General.Settings.TextLabelFont; //mxd
			this.location = new Vector2D(); //mxd
			this.color = new PixelColor(255, 255, 255, 255);
			this.backcolor = new PixelColor(128, 0, 0, 0);
			this.alignx = TextAlignmentX.Center;
			this.aligny = TextAlignmentY.Top;
			this.textsize = SizeF.Empty; //mxd
			this.texturesize = Size.Empty; //mxd
			this.updateneeded = true;
			this.textureupdateneeded = true; //mxd

            InitializeStatics();

            // Register as resource
            General.Map.Graphics.RegisterResource(this);
			
			// We have no destructor
			GC.SuppressFinalize(this);
		}

		//mxd. Compatibility constructor...
		[Obsolete("TextLabel(int capacity) is deprecated, please use TextLabel() instead.")]
		public TextLabel(int unused)
		{
			// Initialize
			this.text = "";
			this.font = new Font(GetFontFamily(), General.Settings.TextLabelFontSize, (General.Settings.TextLabelFontBold ? FontStyle.Bold : FontStyle.Regular)); // General.Settings.TextLabelFont;
			this.location = new Vector2D();
			this.color = new PixelColor(255, 255, 255, 255);
			this.backcolor = new PixelColor(128, 0, 0, 0);
			this.alignx = TextAlignmentX.Center;
			this.aligny = TextAlignmentY.Top;
			this.textsize = SizeF.Empty;
			this.texturesize = Size.Empty;
			this.updateneeded = true;
			this.textureupdateneeded = true;

            InitializeStatics();

            // Register as resource
            General.Map.Graphics.RegisterResource(this);

			// We have no destructor
			GC.SuppressFinalize(this);
		}

		// Diposer
		public void Dispose()
		{
			// Not already disposed?
			if(!isdisposed)
			{
				// Clean up
				UnloadResource();
				font.Dispose();
				
				// Unregister resource
				General.Map.Graphics.UnregisterResource(this);
				
				// Done
				isdisposed = true;
			}
		}

        #endregion

        #region ================== Methods

        // ano - share resources instead of constantly alloc/dealloc
        public void InitializeStatics()
        {
            if (strFormat == null)
            {
                strFormat = new StringFormat();
                strFormat.FormatFlags = StringFormatFlags.FitBlackBox | StringFormatFlags.NoWrap;
                strFormat.Alignment = StringAlignment.Center;
                strFormat.LineAlignment = StringAlignment.Center;
            }
            if (brush == null)
            {
                // if we actually see magenta, know we made a mistake somewhere
                brush = new SolidBrush(System.Drawing.Color.Magenta);
            }
            if (pen == null)
            {
                pen = new Pen(System.Drawing.Color.Magenta);
            }
        }

		// This updates the text if needed
		public void Update(RenderDevice graphics, float translatex, float translatey, float scalex, float scaley)
		{
			// Check if transformation changed and needs to be updated
			if(transformcoords && (translatex != lasttranslatex || translatey != lasttranslatey ||
			   scalex != lastscalex || scaley != lastscaley))
			{
				lasttranslatex = translatex; //mxd
				lasttranslatey = translatey; //mxd
				lastscalex = scalex; //mxd
				lastscaley = scaley; //mxd
				updateneeded = true;
			}

			// Update if needed
			if(updateneeded || textureupdateneeded)
			{
				// Only build when there are any vertices
				if(text.Length > 0)
				{
					// Transform?
					Vector2D abspos = (transformcoords ? location.GetTransformed(translatex, translatey, scalex, scaley) : location);

					// Update text and texture sizes
					if(textsize.IsEmpty || texturesize.IsEmpty)
					{
						textorigin = new PointF(4, 3);
						textrect = new RectangleF(textorigin, General.Interface.MeasureString(text, font));
						textrect.Width = (float)Math.Round(textrect.Width);
						textrect.Height = (float)Math.Round(textrect.Height);
						bgrect = new RectangleF(0, 0, textrect.Width + textorigin.X * 2, textrect.Height + textorigin.Y * 2);

						// Store calculated text size...
						textsize = new SizeF(textrect.Width + textorigin.X * 2, textrect.Height + textorigin.Y * 2);

						// Make PO2 image, for speed and giggles...
						texturesize = new Size(General.NextPowerOf2((int)textsize.Width), General.NextPowerOf2((int)textsize.Height));

						switch(alignx)
						{
							case TextAlignmentX.Center: bgrect.X = (texturesize.Width - bgrect.Width) / 2; break;
							case TextAlignmentX.Right: bgrect.X = texturesize.Width - bgrect.Width; break;
						}

						switch(aligny)
						{
							case TextAlignmentY.Middle: bgrect.Y = (texturesize.Height - bgrect.Height) / 2; break;
							case TextAlignmentY.Bottom: bgrect.Y = texturesize.Height - bgrect.Height; break;
						}

						textrect.X += bgrect.X;
						textrect.Y += bgrect.Y;
					}

					// Align the text horizontally
					double beginx = 0;
					switch(alignx)
					{
						case TextAlignmentX.Left: beginx = abspos.x; break;
						case TextAlignmentX.Center: beginx = abspos.x - texturesize.Width * 0.5f; break;
						case TextAlignmentX.Right: beginx = abspos.x - texturesize.Width; break;
					}

					// Align the text vertically
					double beginy = 0;
					switch(aligny)
					{
						case TextAlignmentY.Top: beginy = abspos.y; break;
						case TextAlignmentY.Middle: beginy = abspos.y - texturesize.Height * 0.5f; break;
						case TextAlignmentY.Bottom: beginy = abspos.y - texturesize.Height; break;
					}

					//mxd. Skip when not on screen...
					RectangleF abssize = new RectangleF((float)beginx, (float)beginy, texturesize.Width, texturesize.Height);
					Size windowsize = General.Map.Graphics.RenderTarget.ClientSize;
					skiprendering = (abssize.Right < 0.1f) || (abssize.Left > windowsize.Width) || (abssize.Bottom < 0.1f) || (abssize.Top > windowsize.Height);
					if(skiprendering) return;

					//mxd. Update texture if needed
					if(textureupdateneeded)
					{
						// Get rid of old texture
						if(texture != null)
						{
							texture.Dispose();
							texture = null;
						}

                        // Create label image
                        using (Bitmap img = CreateLabelImage(text, font, color, backcolor, drawbg, textrect, bgrect, texturesize, textorigin))
                        {
                            texture = new Texture(graphics, img);
                        }
					}

					//mxd. Create the buffer
					if(textbuffer == null || textbuffer.Disposed)
					{
						textbuffer = new VertexBuffer();
					}

					FlatQuad quad = new FlatQuad(PrimitiveType.TriangleStrip, (float)beginx, (float)beginy, (float)(beginx + texturesize.Width), (float)(beginy + texturesize.Height));
                    graphics.SetBufferData(textbuffer, quad.Vertices);
				}
				else
				{
					// No faces in polygon
					textsize = SizeF.Empty; //mxd
					texturesize = Size.Empty; //mxd
					skiprendering = true; //mxd
				}

				// Text updated
				updateneeded = false;
				textureupdateneeded = false; //mxd
			}
		}

        //mxd
		private static Bitmap CreateLabelImage(string text, Font font, PixelColor color, PixelColor backcolor, bool drawbg, RectangleF textrect, RectangleF bgrect, Size texturesize, PointF textorigin)
		{
			Bitmap result = new Bitmap(texturesize.Width, texturesize.Height);
			using(Graphics g = Graphics.FromImage(result))
			{
				g.SmoothingMode = SmoothingMode.HighQuality;
				g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
				g.CompositingQuality = CompositingQuality.HighQuality;

				// Draw text
				// Draw text with BG
				if(drawbg)
				{
					GraphicsPath p = new GraphicsPath();
					float radius = textorigin.X;
					const float outlinewidth = 1;

					RectangleF pathrect = bgrect;
					pathrect.Width -= 1;
					pathrect.Height -= 1;

					// Left line
					p.AddLine(pathrect.Left, pathrect.Bottom - radius + outlinewidth, pathrect.Left, pathrect.Top + radius);
					p.AddArc(pathrect.Left, pathrect.Top, radius, radius, 180, 90);

					// Top line
					p.AddLine(pathrect.Left + radius, pathrect.Top, pathrect.Right - radius, pathrect.Top);
					p.AddArc(pathrect.Right - radius, pathrect.Top, radius, radius, 270, 90);

					// Right line
					p.AddLine(pathrect.Right, pathrect.Top + radius, pathrect.Right, pathrect.Bottom - radius);
					p.AddArc(pathrect.Right - radius, pathrect.Bottom - radius, radius, radius, 0, 90);

					// Bottom line
					p.AddLine(pathrect.Left + radius, pathrect.Bottom, pathrect.Left + radius, pathrect.Bottom);
					p.AddArc(pathrect.Left, pathrect.Bottom - radius, radius, radius, 90, 90);

                    // Fill'n'draw bg
                    brush.Color = color.ToColor();

                    g.FillPath(brush, p);

                    pen.Color = backcolor.ToColor();

                    g.DrawPath(pen, p);
                    
					// Draw text
					textrect.Inflate(4, 2);
                    brush.Color = backcolor.ToColor();

                    g.DrawString(text, font, brush, textrect, strFormat);
				}
				// Draw plain text
				else
				{
					RectangleF plainbgrect = textrect;
					if(text.Length > 1) plainbgrect.Inflate(6, 2);

					RectangleF plaintextrect = textrect;
					plaintextrect.Inflate(6, 4);

                    brush.Color = backcolor.ToColor();
                    g.FillRectangle(brush, plainbgrect);

                    brush.Color = color.ToColor();
                    g.DrawString(text, font, brush, plaintextrect, strFormat);
				}
				
			}

			return result;
		}

		// This unloads the resources
		public void UnloadResource()
		{
			// Clean up
			if(textbuffer != null)
			{
				textbuffer.Dispose();
				textbuffer = null;
			}

			if(texture != null) //mxd
			{
				texture.Dispose(); 
				texture = null;
			}

			// Need to update before we can render
			updateneeded = true;
			textureupdateneeded = true; //mxd
		}

		// This (re)loads the resources
		public void ReloadResource() { }

		/// <summary>
		/// Checks if the whole label is in the viewport.
		/// </summary>
		/// <returns>true if the label in in the viewport, false if it isn't</returns>
		public bool IsInViewport()
		{
			(double width, double height) = texturesize.IsEmpty ? ( 0, 0 ) : (texturesize.Width, texturesize.Height);

			return
				location.x >= (General.Map.CRenderer2D.Viewport.X - width) &&
				location.x < (General.Map.CRenderer2D.Viewport.X + General.Map.CRenderer2D.Viewport.Width + width) &&
				location.y <= (General.Map.CRenderer2D.Viewport.Y - height) &&
				location.y > (General.Map.CRenderer2D.Viewport.Y + General.Map.CRenderer2D.Viewport.Height + height);
		}
		
		#endregion
	}
}
