
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
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.IO;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Windows;

#endregion

namespace CodeImp.DoomBuilder.Data
{
	public abstract unsafe class ImageData : IDisposable
	{
		#region ================== Constants

		public const int TEXTURE_INDEXED = 1;
		#endregion
		
		#region ================== Variables
		
		// Properties
		protected string name;
		protected long longname;
		protected int width;
		protected int height;
		protected Vector2D scale;
		protected bool worldpanning;
		private bool usecolorcorrection;
		protected string filepathname; //mxd. Absolute path to the image;
		protected string shortname; //mxd. Name in uppercase and clamped to DataManager.CLASIC_IMAGE_NAME_LENGTH
		protected string virtualname; //mxd. Path of this name is used in TextureBrowserForm
		protected string displayname; //mxd. Name to display in TextureBrowserForm
		protected bool isFlat; //mxd. If false, it's a texture
		protected bool istranslucent; //mxd. If true, has pixels with alpha > 0 && < 255 
		protected bool ismasked; //mxd. If true, has pixels with zero alpha
		protected bool hasLongName; //mxd. Texture name is longer than DataManager.CLASIC_IMAGE_NAME_LENGTH
		protected bool hasPatchWithSameName; //mxd
		protected int namewidth; // biwa
		protected int shortnamewidth; // biwa
		protected bool wantIndexed; // volte

		//mxd. Hashing
		private static int hashcounter;
		private readonly int hashcode;

        // Loading
        private ImageLoadState previewstate;
        private ImageLoadState imagestate;
        private bool loadfailed;

        // Alpha test
        private BitArray alphatest;
        private int alphatestWidth = 64;
        private int alphatestHeight = 64;

        // GDI bitmap
        private Bitmap loadedbitmap;
        private Bitmap uncorrectedbitmap;
        private Bitmap previewbitmap;
        private Bitmap spritepreviewbitmap;

        // Direct3D texture
        private int mipmaplevels;	// 0 = all mipmaps
		protected bool dynamictexture;
		private Texture texture;
		private Texture indexedTexture;
		
		// Disposing
		protected bool isdisposed;
		
		#endregion
		
		#region ================== Properties
		
		public string Name { get { return name; } }
		public long LongName { get { return longname; } }
		public string ShortName { get { return shortname; } } //mxd
		public string FilePathName { get { return filepathname; } } //mxd
		public string VirtualName { get { return virtualname; } } //mxd
		public string DisplayName { get { return displayname; } } //mxd
		public bool IsFlat { get { return isFlat; } } //mxd
		public bool IsTranslucent { get { return istranslucent; } } //mxd
		public bool IsMasked { get { return ismasked; } } //mxd
		public bool HasPatchWithSameName { get { return hasPatchWithSameName; } } //mxd
		internal bool HasLongName { get { return hasLongName; } } //mxd
		public bool UseColorCorrection { get { return usecolorcorrection; } set { usecolorcorrection = value; } }
		public Texture Texture { get { return GetTexture(false); } }
		public Texture IndexedTexture { get { return GetTexture(true); } }
		public bool IsPreviewLoaded
        {
            get
            {
                if (previewstate == ImageLoadState.None)
                    General.Map.Data.QueueLoadPreview(this);

                return (previewstate == ImageLoadState.Ready);
            }
        }

		public bool IsImageLoaded
        {
            get
            {
                if (imagestate == ImageLoadState.None)
                    General.Map.Data.QueueLoadImage(this);

                return (imagestate == ImageLoadState.Ready);
            }
        }
		public bool LoadFailed { get { return loadfailed; } }
		public bool IsDisposed { get { return isdisposed; } }
		public bool AllowUnload { get; set; }
		public ImageLoadState ImageState { get { return imagestate; } internal set { imagestate = value; } }
		public ImageLoadState PreviewState { get { return previewstate; } internal set { previewstate = value; } }
		public bool UsedInMap { get; internal set; }
		public int MipMapLevels { get { return mipmaplevels; } set { mipmaplevels = value; } }
		public virtual int Width { get { return width; } }
		public virtual int Height { get { return height; } }
		//mxd. Scaled texture size is integer in ZDoom.
		public virtual float ScaledWidth { get { return (float)Math.Round(width * scale.x); } }
		public virtual float ScaledHeight { get { return (float)Math.Round(height * scale.y); } }
		public virtual Vector2D Scale { get { return scale; } }
		public bool WorldPanning { get { return worldpanning; } }
		public int NameWidth {  get { return namewidth; } } // biwa
		public int ShortNameWidth { get { return shortnamewidth; } } // biwa

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		protected ImageData()
		{
			// Defaults
			usecolorcorrection = true;
			AllowUnload = true;

			//mxd. Hashing
			hashcode = hashcounter++;
		}

		// Destructor
		~ImageData()
		{
			this.Dispose();
		}
		
		// Disposer
		public virtual void Dispose()
		{
			// Not already disposed?
			if(!isdisposed)
			{
                // Clean up
                loadedbitmap?.Dispose();
                previewbitmap?.Dispose();
                uncorrectedbitmap?.Dispose();
                spritepreviewbitmap?.Dispose();
                texture?.Dispose();
                indexedTexture?.Dispose();
                loadedbitmap = null;
                previewbitmap = null;
                uncorrectedbitmap = null;
                spritepreviewbitmap = null;
				texture = null;
					
				// Done
				imagestate = ImageLoadState.None;
				previewstate = ImageLoadState.None;
				isdisposed = true;
			}
		}
		
		#endregion
		
		#region ================== Management
		
		// This adds a reference
		// This sets the name
		protected virtual void SetName(string name)
		{
			this.name = name;
			this.filepathname = name; //mxd
			this.shortname = name; //mxd
			this.virtualname = name; //mxd
			this.displayname = name; //mxd
			this.longname = Lump.MakeLongName(name); //mxd

			ComputeNamesWidth(); // biwa
		}
		
		// biwa. Computing the widths in the constructor of ImageBrowserItem accumulates to taking forever when loading many images,
		// like when showing the texture browser of huge texture sets like OTEX
		internal void ComputeNamesWidth()
		{
			//mxd. Calculate names width
			namewidth = (int)Math.Ceiling(General.Interface.MeasureString(name, SystemFonts.MessageBoxFont, 10000, StringFormat.GenericTypographic).Width) + 6;
			shortnamewidth = (int)Math.Ceiling(General.Interface.MeasureString(shortname, SystemFonts.MessageBoxFont, 10000, StringFormat.GenericTypographic).Width) + 6;
		}

        public int GetAlphaTestWidth()
        {
            return alphatestWidth;
        }

        public int GetAlphaTestHeight()
        {
            return alphatestHeight;
        }

        public bool AlphaTestPixel(int x, int y)
        {
            if (alphatest != null)
                return alphatest.Get(x + y * alphatestWidth);
            else
                return true;
        }

        public Image GetBackgroundBitmap()
        {
            return LocalGetBitmap(usecolorcorrection);
        }

        public Bitmap GetSkyboxBitmap()
        {
            return LocalGetBitmap(usecolorcorrection);
        }

        public Bitmap ExportBitmap()
        {
            return LocalGetBitmap(usecolorcorrection);
        }

        public Bitmap GetSpritePreview()
        {
            if (spritepreviewbitmap == null)
                spritepreviewbitmap = LocalGetBitmap(usecolorcorrection);
            return spritepreviewbitmap;
        }

        // Loads the image directly. This is needed by the background loader for some patches.
		// biwa. Just setting UseGammeCorrection before LocalGetBitmap was not enough, since its
		// state is subject to race conditions at load time when using a texture as a patch
		public Bitmap LocalGetBitmap(bool withcolorcorrection)
		{
			// Note: if this turns out to be too slow, do NOT try to make it use GetBitmap or bitmap.
			// Create a cache for the local background loader thread instead.

			LocalLoadResult result = LocalLoadImage();
			if (result.messages.Any(x => x.Type == ErrorType.Error))
			{
				return Properties.Resources.Failed;
			}
			ConvertImageFormat(result, withcolorcorrection);
			return result.bitmap;
		}

		public void LoadImageNow()
        {
			LoadImageNow(true);
        }

		public void LoadImageNow(bool notify)
		{
			if (imagestate != ImageLoadState.Ready)
			{
				imagestate = ImageLoadState.Loading;
				LoadImage(notify);
			}
		}

		internal void BackgroundLoadImage()
        {
            LoadImage(true);
        }

		// This loads the image
		protected void LoadImage(bool notify)
		{
            if (imagestate == ImageLoadState.Ready && previewstate != ImageLoadState.Loading)
                return;

            // Do the loading
            LocalLoadResult loadResult = LocalLoadImage();

            MakeUncorrectedImage(loadResult, usecolorcorrection);
            ConvertImageFormat(loadResult, usecolorcorrection);
            MakeImagePreview(loadResult);
            MakeAlphaTestImage(loadResult);

            // Save memory by disposing the original image immediately if we only used it to load a preview image
            bool onlyPreview = false;
            if (imagestate != ImageLoadState.Loading)
            {
                loadResult.bitmap?.Dispose();
                loadResult.bitmap = null;
                loadResult.uncorrected?.Dispose();
                loadResult.uncorrected = null;
                onlyPreview = true;
            }

            General.MainWindow.RunOnUIThread(() =>
            {
                if (imagestate == ImageLoadState.Loading && !onlyPreview)
                {
                    // Log errors and warnings
                    foreach (LogMessage message in loadResult.messages)
                    {
                        General.ErrorLogger.Add(message.Type, message.Text);
                    }

                    if (loadResult.messages.Any(x => x.Type == ErrorType.Error))
                    {
                        loadfailed = true;
                    }

                    loadedbitmap?.Dispose();
                    uncorrectedbitmap?.Dispose();
                    texture?.Dispose();
                    indexedTexture?.Dispose();
                    imagestate = ImageLoadState.Ready;
                    loadedbitmap = loadResult.bitmap;
                    uncorrectedbitmap = loadResult.uncorrected;
                    alphatest = loadResult.alphatest;
                    alphatestWidth = loadResult.alphatestWidth;
                    alphatestHeight = loadResult.alphatestHeight;

                    if (loadResult.uiThreadWork != null)
                        loadResult.uiThreadWork();
                }
                else
                {
                    loadResult.bitmap?.Dispose();
                }

                if (previewstate == ImageLoadState.Loading)
                {
                    previewbitmap?.Dispose();
                    previewstate = ImageLoadState.Ready;
                    previewbitmap = loadResult.preview;
                }
                else
                {
                    loadResult.preview?.Dispose();
                }
            });

            // Notify the main thread about the change so that sectors can update their buffers
            if (notify)
            {
                if (this is SpriteImage || this is VoxelImage) General.MainWindow.SpriteDataLoaded(this.Name);
                else General.MainWindow.ImageDataLoaded(this.name);
            }
        }

        protected class LocalLoadResult
        {
            public LocalLoadResult(Bitmap bitmap, string error = null, Action uiThreadWork = null)
            {
                this.bitmap = bitmap;
                messages = new List<LogMessage>();
                if (error != null)
                    messages.Add(new LogMessage(ErrorType.Error, error));
                this.uiThreadWork = uiThreadWork;
            }

            public LocalLoadResult(Bitmap bitmap, IEnumerable<LogMessage> messages, Action uiThreadWork = null)
            {
                this.bitmap = bitmap;
                this.messages = messages.ToList();
                this.uiThreadWork = uiThreadWork;
            }

            public Bitmap bitmap;
            public Bitmap preview;
            public Bitmap uncorrected;
            public BitArray alphatest;
            public int alphatestWidth;
            public int alphatestHeight;
            public List<LogMessage> messages;
            public Action uiThreadWork;
        }

        protected abstract LocalLoadResult LocalLoadImage();
		
        protected class LogMessage
        {
            public LogMessage(ErrorType type, string text) { Type = type; Text = text; }
            public ErrorType Type { get; set; }
            public string Text { get; set; }
        }

        void ConvertImageFormat(LocalLoadResult loadResult, bool withcolorcorrection)
		{
            // Bitmap loaded successfully?
            Bitmap bitmap = loadResult.bitmap;
			if(bitmap != null)
			{
				// Bitmap has incorrect format?
				if(bitmap.PixelFormat != PixelFormat.Format32bppArgb)
				{
					//General.ErrorLogger.Add(ErrorType.Warning, "Image '" + name + "' does not have A8R8G8B8 pixel format. Conversion was needed.");
					Bitmap oldbitmap = bitmap;
					try
					{
						// Convert to desired pixel format
						bitmap = new Bitmap(oldbitmap.Size.Width, oldbitmap.Size.Height, PixelFormat.Format32bppArgb);
						Graphics g = Graphics.FromImage(bitmap);
						g.PageUnit = GraphicsUnit.Pixel;
						g.CompositingQuality = CompositingQuality.HighQuality;
						g.InterpolationMode = InterpolationMode.NearestNeighbor;
						g.SmoothingMode = SmoothingMode.None;
						g.PixelOffsetMode = PixelOffsetMode.None;
						g.Clear(Color.Transparent);
						g.DrawImage(oldbitmap, 0, 0, oldbitmap.Size.Width, oldbitmap.Size.Height);
						g.Dispose();
						oldbitmap.Dispose();
					}
					catch(Exception e)
					{
						bitmap = oldbitmap;
						loadResult.messages.Add(new LogMessage(ErrorType.Warning, "Cannot lock image \"" + name + "\" for pixel format conversion. The image may not be displayed correctly.\n" + e.GetType().Name + ": " + e.Message));
					}
				}
					
				// This applies brightness correction on the image
				if(withcolorcorrection)
				{
					BitmapData bmpdata = null;
						
					try
					{
						// Try locking the bitmap
						bmpdata = bitmap.LockBits(new Rectangle(0, 0, bitmap.Size.Width, bitmap.Size.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
					}
					catch(Exception e)
					{
                        loadResult.messages.Add(new LogMessage(ErrorType.Warning, "Cannot lock image \"" + name + "\" for color correction. The image may not be displayed correctly.\n" + e.GetType().Name + ": " + e.Message));
					}

					// Bitmap locked?
					if(bmpdata != null)
					{
						// Apply color correction
						PixelColor* pixels = (PixelColor*)(bmpdata.Scan0.ToPointer());
						General.Colors.ApplyColorCorrection(pixels, bmpdata.Width * bmpdata.Height);
						bitmap.UnlockBits(bmpdata);
					}
				}
			}
			else
			{
				// Loading failed
				// We still mark the image as ready so that it will
				// not try loading again until Reload Resources is used
				bitmap = new Bitmap(Properties.Resources.Failed);
			}

			if(bitmap != null)
			{
				width = bitmap.Size.Width;
				height = bitmap.Size.Height;

				// Do we still have to set a scale?
				if((scale.x == 0.0f) && (scale.y == 0.0f))
				{
					if((General.Map != null) && (General.Map.Config != null))
					{
						scale.x = General.Map.Config.DefaultTextureScale;
						scale.y = General.Map.Config.DefaultTextureScale;
					}
					else
					{
						scale.x = 1.0f;
						scale.y = 1.0f;
					}
				}

				if(!loadfailed)
				{
					//mxd. Check translucency and calculate average color?
					if(General.Map != null && General.Map.Data != null && General.Map.Data.GlowingFlats != null &&
						General.Map.Data.GlowingFlats.ContainsKey(longname) &&
						General.Map.Data.GlowingFlats[longname].CalculateTextureColor)
					{
						BitmapData bmpdata = null;
						try
						{
							bmpdata = bitmap.LockBits(new Rectangle(0, 0, bitmap.Size.Width, bitmap.Size.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
						}
						catch(Exception e)
						{
                            loadResult.messages.Add(new LogMessage(ErrorType.Error, "Cannot lock image \"" + this.filepathname + "\" for glow color calculation. " + e.GetType().Name + ": " + e.Message));
						}

						if(bmpdata != null)
						{
							PixelColor* pixels = (PixelColor*)(bmpdata.Scan0.ToPointer());
							int numpixels = bmpdata.Width * bmpdata.Height;
							uint r = 0;
							uint g = 0;
							uint b = 0;

							for(PixelColor* cp = pixels + numpixels - 1; cp >= pixels; cp--)
							{
								r += cp->r;
								g += cp->g;
								b += cp->b;

								// Also check alpha
								if(cp->a > 0 && cp->a < 255) istranslucent = true;
								else if(cp->a == 0) ismasked = true;
							}

							// Update glow data
							int br = (int)(r / numpixels);
							int bg = (int)(g / numpixels);
							int bb = (int)(b / numpixels);

							int max = Math.Max(br, Math.Max(bg, bb));

							// Black can't glow...
							if(max == 0)
							{
								General.Map.Data.GlowingFlats.Remove(longname);
							}
							else
							{
								// That's how it's done in GZDoom (and I may be totally wrong about this)
								br = Math.Min(255, br * 153 / max);
								bg = Math.Min(255, bg * 153 / max);
								bb = Math.Min(255, bb * 153 / max);

								General.Map.Data.GlowingFlats[longname].Color = new PixelColor(255, (byte)br, (byte)bg, (byte)bb);
								General.Map.Data.GlowingFlats[longname].CalculateTextureColor = false;
								if(!General.Map.Data.GlowingFlats[longname].Fullbright) General.Map.Data.GlowingFlats[longname].Brightness = (br + bg + bb) / 3;
							}

							// Release the data
							bitmap.UnlockBits(bmpdata);
						}
					}
					//mxd. Check if the texture is translucent
					else
					{
						BitmapData bmpdata = null;
						try
						{
							bmpdata = bitmap.LockBits(new Rectangle(0, 0, bitmap.Size.Width, bitmap.Size.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
						}
						catch(Exception e)
						{
                            loadResult.messages.Add(new LogMessage(ErrorType.Error, "Cannot lock image \"" + this.filepathname + "\" for translucency check. " + e.GetType().Name + ": " + e.Message));
						}

						if(bmpdata != null)
						{
							PixelColor* pixels = (PixelColor*)(bmpdata.Scan0.ToPointer());
							int numpixels = bmpdata.Width * bmpdata.Height;

							for(PixelColor* cp = pixels + numpixels - 1; cp >= pixels; cp--)
							{
								// Check alpha
								if(cp->a > 0 && cp->a < 255) istranslucent = true;
								else if(cp->a == 0) ismasked = true;
							}

							// Release the data
							bitmap.UnlockBits(bmpdata);
						}
					}
				}
			}

            loadResult.bitmap = bitmap;
		}

        // Dimensions of a single preview image
        const int MAX_PREVIEW_SIZE = 256; //mxd

        // This makes a copy of the image before color correction or, if color correction is disabled for this image,
        // just references the original bitmap.
        private void MakeUncorrectedImage(LocalLoadResult loadResult, bool usecolorcorrection)
        {
	        if (loadResult.bitmap == null)
		        return;
	        if (usecolorcorrection)
	        {
		        loadResult.uncorrected = new Bitmap(loadResult.bitmap);
	        }
	        else
	        {
		        loadResult.uncorrected = loadResult.bitmap;
	        }
        }

        // This makes a preview for the given image and updates the image settings
        private void MakeImagePreview(LocalLoadResult loadResult)
        {
            if (loadResult.bitmap == null)
                return;

            Bitmap image = loadResult.bitmap;
            Bitmap preview;

            int imagewidth = image.Width;
            int imageheight = image.Height;

            // Determine preview size
            float scalex = (imagewidth > MAX_PREVIEW_SIZE) ? (MAX_PREVIEW_SIZE / (float)imagewidth) : 1.0f;
            float scaley = (imageheight > MAX_PREVIEW_SIZE) ? (MAX_PREVIEW_SIZE / (float)imageheight) : 1.0f;
            float scale = Math.Min(scalex, scaley);
            int previewwidth = (int)(imagewidth * scale);
            int previewheight = (int)(imageheight * scale);
            if (previewwidth < 1) previewwidth = 1;
            if (previewheight < 1) previewheight = 1;

            //mxd. Expected and actual image sizes and format match?
            if (previewwidth == imagewidth && previewheight == imageheight && image.PixelFormat == PixelFormat.Format32bppArgb)
            {
                preview = new Bitmap(image);
            }
            else
            {
                // Make new image
                preview = new Bitmap(previewwidth, previewheight, PixelFormat.Format32bppArgb);
                Graphics g = Graphics.FromImage(preview);
                g.PageUnit = GraphicsUnit.Pixel;
                //g.CompositingQuality = CompositingQuality.HighQuality; //mxd
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                //g.SmoothingMode = SmoothingMode.HighQuality; //mxd
                g.PixelOffsetMode = PixelOffsetMode.None;
                //g.Clear(Color.Transparent); //mxd

                // Draw image onto atlas
                Rectangle atlasrect = new Rectangle(0, 0, previewwidth, previewheight);
                RectangleF imgrect = General.MakeZoomedRect(new Size(imagewidth, imageheight), atlasrect);
                if (imgrect.Width < 1.0f)
                {
                    imgrect.X -= 0.5f - imgrect.Width * 0.5f;
                    imgrect.Width = 1.0f;
                }
                if (imgrect.Height < 1.0f)
                {
                    imgrect.Y -= 0.5f - imgrect.Height * 0.5f;
                    imgrect.Height = 1.0f;
                }
                g.DrawImage(image, imgrect);
                g.Dispose();
            }

            loadResult.preview = preview;
        }

        unsafe void MakeAlphaTestImage(LocalLoadResult loadResult)
        {
            if (loadResult.bitmap == null)
                return;

            int width = loadResult.bitmap.Width;
            int height = loadResult.bitmap.Height;
            loadResult.alphatestWidth = width;
            loadResult.alphatestHeight = height;

			BitmapData bmpdata = loadResult.bitmap.LockBits(new Rectangle(0, 0, loadResult.bitmap.Size.Width, loadResult.bitmap.Size.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
			PixelColor* pixels = (PixelColor*)(bmpdata.Scan0.ToPointer());

			for (int y = 0; y < height; y++)
            {
				PixelColor* line = pixels + y * width;
				for (int x = 0; x < width; x++)
                {
                    if (line[x].a == 0)
                    {
                        if (loadResult.alphatest == null)
                            loadResult.alphatest = new BitArray(width * height, true);
                        loadResult.alphatest.Set(x + y * width, false);
                    }
                }
            }

			loadResult.bitmap.UnlockBits(bmpdata);
		}

		Texture GetTexture(bool indexed = false)
		{
			if (indexed && indexedTexture != null)
				return indexedTexture;
			if (!indexed && texture != null)
          return texture;

			if (indexed && !wantIndexed)
			{
				// indexed texture requested, but we didn't generate it, so we have to reload the image
				ReleaseTexture();
				imagestate = ImageLoadState.None;
				wantIndexed = true;
			}
			
			if (imagestate == ImageLoadState.Loading)
          return General.Map.Data.LoadingTexture;
      if (loadfailed)
          return General.Map.Data.FailedTexture;
      
      if (imagestate == ImageLoadState.None)
      {
	      General.Map.Data.QueueLoadImage(this);
        return General.Map.Data.LoadingTexture;
      }
      
      if (loadedbitmap == null)
      {
	      return General.Map.Data.LoadingTexture;
      }
      
      if (wantIndexed)
      {
	      Bitmap indexedBitmap = CreateIndexedBitmap(uncorrectedbitmap, General.Map.Data.Palette);
	      indexedTexture = new Texture(General.Map.Graphics, indexedBitmap);
	      indexedTexture.UserData = TEXTURE_INDEXED;
      }
      
      texture = new Texture(General.Map.Graphics, loadedbitmap);

      loadedbitmap.Dispose();
      loadedbitmap = null;
      uncorrectedbitmap.Dispose();
      uncorrectedbitmap = null;

#if DEBUG
			texture.Tag = name; //mxd. Helps with tracking undisposed resources...
#endif
			
      return indexed ? indexedTexture : texture;
		}

		Bitmap CreateIndexedBitmap(Bitmap original, Playpal palette)
		{
			int[] indices = new int[original.Width * original.Height];
			
			Bitmap indexed = new Bitmap(original.Width, original.Height, PixelFormat.Format32bppArgb);
			BitmapData indata = original.LockBits(new Rectangle(0, 0, original.Size.Width, original.Size.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
			PixelColor* inpixels = (PixelColor*)indata.Scan0.ToPointer();
			General.Colors.QuantizeColorsToPlaypal(inpixels, indices, palette);

			BitmapData outdata = indexed.LockBits(new Rectangle(0, 0, indexed.Size.Width, indexed.Size.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
			PixelColor *outpixels = (PixelColor*)outdata.Scan0.ToPointer();
			for (int i = 0; i < indices.Length; i++)
			{
				outpixels[i].r = (byte)indices[i];
				outpixels[i].g = 0;
				outpixels[i].b = 0;
				outpixels[i].a = inpixels[i].a;
			}
			
			original.UnlockBits(indata);
			indexed.UnlockBits(outdata);

			return indexed;
		}

		// This updates a dynamic texture
		public void UpdateTexture(Bitmap canvas)
		{
			if (canvas.PixelFormat != PixelFormat.Format32bppArgb)
				throw new Exception("Dynamic images must be in 32 bits ARGB format.");
			if(!dynamictexture)
				throw new Exception("The image must be a dynamic image to support direct updating.");

            General.Map.Graphics.SetPixels(GetTexture(), canvas);
		}
		
		// This destroys the Direct3D texture
		public void ReleaseTexture()
		{
			texture?.Dispose();
			texture = null;
			indexedTexture?.Dispose();
			indexedTexture = null;
		}

		// This returns a preview image
		public virtual Image GetPreview()
		{
			// Preview ready?
			if(previewstate == ImageLoadState.Ready)
			{
				// Make a copy
				return new Bitmap(previewbitmap);
			}

            // Loading failed?
            if (loadfailed)
			{
				// Return error bitmap
				return Properties.Resources.Failed;
			}

            if (previewstate == ImageLoadState.None)
            {
                General.Map.Data.QueueLoadPreview(this);
            }

            // Return loading bitmap
            return Properties.Resources.Hourglass;
		}

		//mxd. This greatly speeds up Dictionary lookups
		public override int GetHashCode()
		{
			return hashcode;
		}
		
		#endregion
	}
}
