
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
using System.Drawing.Imaging;
using CodeImp.DoomBuilder.Rendering;
using System.IO;
using System.Linq;

#endregion

namespace CodeImp.DoomBuilder.Data
{
	internal sealed unsafe class TextureImage : ImageData
	{
		#region ================== Variables

		private List<TexturePatch> patches;
		
		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public TextureImage(string group, string name, int width, int height, float scalex, float scaley, bool worldpanning)
		{
			// Initialize
			this.width = width;
			this.height = height;
			this.scale.x = scalex;
			this.scale.y = scaley;
			this.worldpanning = worldpanning; //mxd
			this.texturenamespace = TextureNamespace.WALLTEXTURE;
			this.patches = new List<TexturePatch>();
			SetName(name);
			virtualname = "[" + group + "]/" + this.name; //mxd
			
			// We have no destructor
			GC.SuppressFinalize(this);
		}

		#endregion

		#region ================== Methods

		// This adds a patch to the texture
		public void AddPatch(TexturePatch patch)
		{
			// Add it
			patches.Add(patch);

			if(patch.LumpName == Name) hasPatchWithSameName = true; //mxd
		}
		
		// This loads the image
		protected override LocalLoadResult LocalLoadImage()
		{
			// Checks
			if(width == 0 || height == 0) return new LocalLoadResult(null);

			BitmapData bitmapdata = null;
			PixelColor* pixels = (PixelColor*)0;

            Bitmap bitmap = null;
            List<LogMessage> messages = new List<LogMessage>();
			int[] columnumpatches = new int[width];
			BitArray columnmasked = new BitArray(width, false);
			Dictionary<TexturePatch, Bitmap> patchbmps = new Dictionary<TexturePatch, Bitmap>();

			bool fixnegativeoffsets = General.Map.Config.Compatibility.FixNegativePatchOffsets;
			bool fixmaskedoffsets = General.Map.Config.Compatibility.FixMaskedPatchOffsets;

			// Create texture bitmap
			try
			{
				if(bitmap != null) bitmap.Dispose();
				bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
				bitmapdata = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
				pixels = (PixelColor*)bitmapdata.Scan0.ToPointer();
                SysCall.ZeroPixels(pixels, width * height);
			}
			catch(Exception e)
			{
				// Unable to make bitmap
				messages.Add(new LogMessage(ErrorType.Error, "Unable to load texture image \"" + this.Name + "\". " + e.GetType().Name + ": " + e.Message));
			}

			int missingpatches = 0; //mxd

			if(!messages.Any(x => x.Type == ErrorType.Error))
			{
				// Load all patch bitmaps, and see if the patch has a negative vertical offset
				foreach (TexturePatch p in patches)
				{
					// Get the patch data stream
					string patchlocation = string.Empty; //mxd
					Stream patchdata = General.Map.Data.GetPatchData(p.LumpName, p.HasLongName, ref patchlocation);
					if (patchdata != null)
					{
						// Get a reader for the data
						Bitmap patchbmp = ImageDataFormat.TryLoadImage(patchdata, ImageDataFormat.DOOMPICTURE, General.Map.Data.Palette);
						if (patchbmp == null)
						{
							//mxd. Probably that's a flat?..
							if (General.Map.Config.MixTexturesFlats)
							{
								patchbmp = ImageDataFormat.TryLoadImage(patchdata, ImageDataFormat.DOOMFLAT, General.Map.Data.Palette);
							}
							if (patchbmp == null)
							{
								// Data is in an unknown format!
								messages.Add(new LogMessage(ErrorType.Error, "Patch lump \"" + Path.Combine(patchlocation, p.LumpName) + "\" data format could not be read, while loading texture \"" + this.Name + "\". Does this lump contain valid picture data at all?"));
								missingpatches++; //mxd
							}
						}

						// Store the bitmap
						patchbmps[p] = patchbmp;

						// Done
						patchdata.Dispose();
					}
					else
					{
						// Missing a patch lump!
						messages.Add(new LogMessage(ErrorType.Error, "Missing patch lump \"" + p.LumpName + "\" while loading texture \"" + this.Name + "\". Did you forget to include required resources?"));
						missingpatches++; //mxd
					}
				}

				// There's a bug in vanilla Doom where negative patch offsets are ignored (the patch y offset is set to 0). If
				// the configuration is for an engine that doesn't fix the bug we have to emulate that behavior
				// See https://doomwiki.org/wiki/Vertical_offsets_are_ignored_in_texture_patches
				if (!fixnegativeoffsets || !fixmaskedoffsets)
				{
					// Check which columns have more than one patch
					foreach(TexturePatch p in patches)
					{
						if (!patchbmps.ContainsKey(p) || patchbmps[p] == null) continue;

						bool ismaked = BitmapIsMasked(patchbmps[p]);

						for (int x = 0; x < patchbmps[p].Width; x++)
						{
							int ox = p.X + x;
							if (ox >= 0 && ox < columnumpatches.Length)
							{
								if(!fixnegativeoffsets)
									columnumpatches[ox]++;

								if (!fixmaskedoffsets && ismaked)
									columnmasked[ox] = true;
							}
						}
					}
				}

				// Go for all patches
				foreach(TexturePatch p in patches)
				{
					if(patchbmps.ContainsKey(p) && patchbmps[p] != null)
					{
                        // Draw the patch
						DrawToPixelData(patchbmps[p], pixels, width, height, p.X, p.Y, fixnegativeoffsets, fixmaskedoffsets, columnumpatches, columnmasked);
					}
				}

				// Don't need the bitmaps anymore
				foreach (Bitmap bmp in patchbmps.Values)
					bmp?.Dispose();

				// Done
				bitmap.UnlockBits(bitmapdata);
			}
				
			// Dispose bitmap if load failed
			if((bitmap != null) && (messages.Any(x => x.Type == ErrorType.Error) || missingpatches >= patches.Count)) //mxd. We can still display texture if at least one of the patches was loaded
			{
				bitmap.Dispose();
				bitmap = null;
			}

            return new LocalLoadResult(bitmap, messages);
		}

        // This draws the picture to the given pixel color data
        static unsafe void DrawToPixelData(Bitmap bmp, PixelColor* target, int targetwidth, int targetheight, int x, int y, bool fixnegativeoffsets, bool fixmaskedoffsets, int[] columnumpatches, BitArray columnmasked)
        {
            // Get bitmap
            int width = bmp.Size.Width;
            int height = bmp.Size.Height;
			int patchy = y;

            // Lock bitmap pixels
            BitmapData bmpdata = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            PixelColor* pixels = (PixelColor*)bmpdata.Scan0.ToPointer();

            // Go for all pixels in the original image
            for (int ox = 0; ox < width; ox++)
            {
				int tx = x + ox;
				int drawheight = height;

				// If we have to emulate the negative vertical offset bug we also have to recalculate the height of the
				// patch that is actually drawn, since it'll only draw as many pixels as it'd draw as if the negative
				// vertical offset was taken into account
				if ((patchy < 0 && !fixnegativeoffsets && tx < width && tx >= 0 && tx < columnumpatches.Length && columnumpatches[tx] > 1) && !(!fixmaskedoffsets && tx >= 0 && tx < columnmasked.Length && columnmasked[tx] == true))
					drawheight = height + patchy;

				for (int oy = 0; oy < drawheight; oy++)
                {
                    // Copy this pixel?
                    if (pixels[oy * width + ox].a > 0.5f)
                    {
						int realy = y;

						if (!fixmaskedoffsets && tx >= 0 && tx < columnmasked.Length && columnmasked[tx] == true)
						{
							if (tx >= 0 && tx < columnumpatches.Length && columnumpatches[tx] == 1)
								realy = 0;
						}
						else if (y < 0 && !fixnegativeoffsets)
							realy = 0;

                        // Calculate target pixel and copy when within bounds
                        int ty = realy + oy;
                        if ((tx >= 0) && (tx < targetwidth) && (ty >= 0) && (ty < targetheight))
                            target[ty * targetwidth + tx] = pixels[oy * width + ox];
                    }
                }
            }

            // Done
            bmp.UnlockBits(bmpdata);
        }

		/// <summary>
		/// Checks if the given bitmap has masked pixels
		/// </summary>
		/// <param name="bmp">Bitmap to check</param>
		/// <returns>true if masked, false if not masked</returns>
		internal static unsafe bool BitmapIsMasked(Bitmap bmp)
		{
			// Get bitmap
			int width = bmp.Size.Width;
			int height = bmp.Size.Height;

			// Lock bitmap pixels
			BitmapData bmpdata = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
			PixelColor* pixels = (PixelColor*)bmpdata.Scan0.ToPointer();

			for (int ox = 0; ox < width; ox++)
			{
				for (int oy = 0; oy < height; oy++)
				{
					if (pixels[oy * width + ox].a <= 0.5f)
					{
						bmp.UnlockBits(bmpdata);
						return true;
					}
				}
			}

			bmp.UnlockBits(bmpdata);
			return false;
		}

        #endregion
    }
}
