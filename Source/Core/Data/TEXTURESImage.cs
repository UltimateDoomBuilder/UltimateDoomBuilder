
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
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using CodeImp.DoomBuilder.Controls;
using CodeImp.DoomBuilder.IO;
using CodeImp.DoomBuilder.Rendering;

#endregion

namespace CodeImp.DoomBuilder.Data
{
	public enum TextureNamespace
	{
		TEXTURE,
		WALLTEXTURE,
		FLAT,
		SPRITE,
		PATCH
	}

	internal sealed unsafe class TEXTURESImage : ImageData
	{
		#region ================== Variables

		private readonly List<TexturePatch> patches; //mxd
		private readonly bool optional; //mxd
		private readonly bool nulltexture; //mxd

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public TEXTURESImage(string name, string virtualpath, int width, int height, int offsetx, int offsety, float scalex, float scaley, 
			bool worldpanning, TextureNamespace texturenamespace, bool optional, bool nulltexture)
		{
			// Initialize
			this.width = width;
			this.height = height;
			this.offsetx = offsetx;
			this.offsety = offsety;
			this.scale.x = scalex;
			this.scale.y = scaley;
			this.worldpanning = worldpanning;
			this.optional = optional; //mxd
			this.nulltexture = nulltexture; //mxd
			this.patches = new List<TexturePatch>(1);
			this.texturenamespace = texturenamespace;

			//mxd
			SetName(name);
			this.virtualname = (!string.IsNullOrEmpty(virtualpath) ? virtualpath : "[TEXTURES]") + Path.AltDirectorySeparatorChar + this.name;
			
			// We have no destructor
			GC.SuppressFinalize(this);
		}

		#endregion

		#region ================== Methods

		//mxd
		protected override void SetName(string name) 
		{
			if(!General.Map.Config.UseLongTextureNames) 
			{
				if(name.Length > DataManager.CLASIC_IMAGE_NAME_LENGTH)
					name = name.Substring(0, DataManager.CLASIC_IMAGE_NAME_LENGTH);
				name = name.ToUpperInvariant();
			}
			
			base.SetName(name);

			this.shortname = this.displayname.ToUpperInvariant();
			if(this.shortname.Length > DataManager.CLASIC_IMAGE_NAME_LENGTH) 
			{
				this.shortname = this.shortname.Substring(0, DataManager.CLASIC_IMAGE_NAME_LENGTH);
			}

			ComputeNamesWidth(); // biwa
		}

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

			Graphics g = null;

            Bitmap bitmap = null;
            List<LogMessage> messages = new List<LogMessage>();

			// Create texture bitmap
			try
			{
				if(bitmap != null) bitmap.Dispose();
				bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
				BitmapData bitmapdata = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
				PixelColor* pixels = (PixelColor*)bitmapdata.Scan0.ToPointer();
				General.ZeroPixels(pixels, width * height);
				bitmap.UnlockBits(bitmapdata);
				g = Graphics.FromImage(bitmap);
			}
			catch(Exception e)
			{
				// Unable to make bitmap
				messages.Add(new LogMessage(ErrorType.Error, "Unable to load texture image \"" + this.Name + "\". " + e.GetType().Name + ": " + e.Message));
			}

			int missingpatches = 0; //mxd
			if(patches.Count == 0) //mxd
			{
				//mxd. Empty image will suffice here, I suppose...
				if(nulltexture)
				{
                    return new LocalLoadResult(bitmap, messages);
				}

                // No patches!
                messages.Add(new LogMessage(ErrorType.Error, "No patches are defined for texture \"" + this.Name + "\""));
			}
			else if(!messages.Any(x => x.Type == ErrorType.Error))
			{
				// Go for all patches
				foreach(TexturePatch p in patches)
				{
					//mxd. Some patches (like "TNT1A0") should be skipped
					if(p.Skip) continue;
						
					// Get the patch data stream
					string patchlocation = string.Empty; //mxd
					Stream patchdata = General.Map.Data.GetPatchData(p.LumpName, p.HasLongName, ref patchlocation);
					if(patchdata != null)
					{
						// Copy patch data to memory
						byte[] membytes = new byte[(int)patchdata.Length];

						lock(patchdata) //mxd
						{
							patchdata.Seek(0, SeekOrigin.Begin);
							patchdata.Read(membytes, 0, (int)patchdata.Length);
						}
							
						MemoryStream mem = new MemoryStream(membytes);
						mem.Seek(0, SeekOrigin.Begin);

						Bitmap patchbmp = ImageDataFormat.TryLoadImage(mem, ImageDataFormat.DOOMPICTURE, General.Map.Data.Palette);
						if(patchbmp == null)
						{
							//mxd. Probably that's a flat?..
							if(General.Map.Config.MixTexturesFlats) 
							{
								patchbmp = ImageDataFormat.TryLoadImage(mem, ImageDataFormat.DOOMFLAT, General.Map.Data.Palette);
							}

							if(patchbmp == null) 
							{
								// Data is in an unknown format!
								if(!nulltexture) messages.Add(new LogMessage(optional ? ErrorType.Warning : ErrorType.Error, "Patch lump \"" + Path.Combine(patchlocation, p.LumpName) + "\" data format could not be read, while loading texture \"" + this.Name + "\""));
								missingpatches++; //mxd
							}
						}

						if(patchbmp != null) 
						{
							//mxd. Apply transformations from TexturePatch 
							patchbmp = TransformPatch(bitmap, p, patchbmp);

							// Draw the patch on the texture image
							Rectangle tgtrect = new Rectangle(p.X, p.Y, patchbmp.Size.Width, patchbmp.Size.Height);
							g.DrawImageUnscaledAndClipped(patchbmp, tgtrect);
							patchbmp.Dispose();
						}

						// Done
						mem.Dispose();
					}
					else
					{
						//mxd. ZDoom can use any known graphic as patch
						if(General.Map.Config.MixTexturesFlats)
						{
							ImageData img = General.Map.Data.GetTextureImage(p.LumpName);
							if(!(img is UnknownImage) && img != this)
							{
								//mxd. Apply transformations from TexturePatch. We don't want to modify the original bitmap here, so make a copy
								// biwa. Make sure to get the image without color correction, as the final texture would be too bright if the patch
								// is also a texture
								Bitmap bmp = new Bitmap(img.LocalGetBitmap(false));
								Bitmap patchbmp = TransformPatch(bitmap, p, bmp);

								// Draw the patch on the texture image
								Rectangle tgtrect = new Rectangle(p.X, p.Y, patchbmp.Size.Width, patchbmp.Size.Height);
								g.DrawImageUnscaledAndClipped(patchbmp, tgtrect);
								patchbmp.Dispose();

								continue;
							}
						}
							
						// Missing a patch lump!
						if(!nulltexture) messages.Add(new LogMessage(optional ? ErrorType.Warning : ErrorType.Error, "Missing patch lump \"" + p.LumpName + "\" while loading texture \"" + this.Name + "\""));
						missingpatches++; //mxd
					}
				}
			}
				
			// Dispose bitmap if load failed
			if(!nulltexture && (bitmap != null) && (messages.Any(x => x.Type == ErrorType.Error) || missingpatches >= patches.Count)) //mxd. We can still display texture if at least one of the patches was loaded
			{
				bitmap.Dispose();
				bitmap = null;
			}

            return new LocalLoadResult(bitmap, messages);
        }

        //mxd
        private Bitmap TransformPatch(Bitmap bitmap, TexturePatch p, Bitmap patchbmp)
		{
			//mxd. Flip
			if(p.FlipX || p.FlipY)
			{
				RotateFlipType flip;
				if(p.FlipX && !p.FlipY) flip = RotateFlipType.RotateNoneFlipX;
				else if(!p.FlipX && p.FlipY) flip = RotateFlipType.RotateNoneFlipY;
				else flip = RotateFlipType.RotateNoneFlipXY;
				patchbmp.RotateFlip(flip);
			}

			//mxd. Then rotate. I do it this way because RotateFlip function rotates THEN flips, and GZDoom does it the other way around.
			if(p.Rotate != 0)
			{
				RotateFlipType rotate;
				switch(p.Rotate)
				{
					case 90:  rotate = RotateFlipType.Rotate90FlipNone; break;
					case 180: rotate = RotateFlipType.Rotate180FlipNone; break;
					default:  rotate = RotateFlipType.Rotate270FlipNone; break;
				}
				patchbmp.RotateFlip(rotate);
			}

			// Adjust patch alpha, apply tint or blend
			if(p.BlendStyle != TexturePathBlendStyle.NONE || p.RenderStyle != TexturePathRenderStyle.COPY)
			{
				BitmapData bmpdata = null;

				try
				{
					bmpdata = patchbmp.LockBits(new Rectangle(0, 0, patchbmp.Size.Width, patchbmp.Size.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
				}
				catch(Exception e)
				{
					General.ErrorLogger.Add(ErrorType.Error, "Cannot lock image \"" + p.LumpName + "\" for alpha adjustment. " + e.GetType().Name + ": " + e.Message);
				}

				if(bmpdata != null)
				{
					PixelColor* pixels = (PixelColor*)(bmpdata.Scan0.ToPointer());
					int numpixels = bmpdata.Width * bmpdata.Height;
					int patchalpha = (int)Math.Round(General.Clamp(p.Alpha, 0f, 1f) * 255); //convert alpha to [0-255] range

					//mxd. Blend/Tint support
					if(p.BlendStyle == TexturePathBlendStyle.BLEND)
					{
						for(PixelColor* cp = pixels + numpixels - 1; cp >= pixels; cp--)
						{
							cp->r = (byte)((cp->r * p.BlendColor.r) * PixelColor.BYTE_TO_FLOAT);
							cp->g = (byte)((cp->g * p.BlendColor.g) * PixelColor.BYTE_TO_FLOAT);
							cp->b = (byte)((cp->b * p.BlendColor.b) * PixelColor.BYTE_TO_FLOAT);
						}
					}
					else if(p.BlendStyle == TexturePathBlendStyle.TINT)
					{
						float tintammount = p.BlendColor.a * PixelColor.BYTE_TO_FLOAT;// -0.1f;

						if(tintammount > 0)
						{
							float br = p.BlendColor.r * PixelColor.BYTE_TO_FLOAT * tintammount;
							float bg = p.BlendColor.g * PixelColor.BYTE_TO_FLOAT * tintammount;
							float bb = p.BlendColor.b * PixelColor.BYTE_TO_FLOAT * tintammount;
							float invtintammount = 1.0f - tintammount;

							for(PixelColor* cp = pixels + numpixels - 1; cp >= pixels; cp--)
							{
								cp->r = (byte)(((cp->r * PixelColor.BYTE_TO_FLOAT) * invtintammount + br) * 255.0f);
								cp->g = (byte)(((cp->g * PixelColor.BYTE_TO_FLOAT) * invtintammount + bg) * 255.0f);
								cp->b = (byte)(((cp->b * PixelColor.BYTE_TO_FLOAT) * invtintammount + bb) * 255.0f);
							}
						}
					}

					//mxd. Apply RenderStyle
					if(p.RenderStyle == TexturePathRenderStyle.BLEND)
					{
						for(PixelColor* cp = pixels + numpixels - 1; cp >= pixels; cp--)
							cp->a = (byte)((cp->a * patchalpha) * PixelColor.BYTE_TO_FLOAT);
					}
					//mxd. We need a copy of underlying part of texture for these styles
					else if(p.RenderStyle != TexturePathRenderStyle.COPY)
					{
						// Copy portion of texture
						int lockWidth = (p.X + patchbmp.Size.Width > bitmap.Width) ? bitmap.Width - p.X : patchbmp.Size.Width;
						int lockHeight = (p.Y + patchbmp.Size.Height > bitmap.Height) ? bitmap.Height - p.Y : patchbmp.Size.Height;

						Bitmap source = new Bitmap(patchbmp.Size.Width, patchbmp.Size.Height);
						using(Graphics sg = Graphics.FromImage(source))
							sg.DrawImageUnscaled(bitmap, new Rectangle(-p.X, -p.Y, lockWidth, lockHeight));

						// Lock texture
						BitmapData texturebmpdata = null;

						try
						{
							texturebmpdata = source.LockBits(new Rectangle(0, 0, source.Width, source.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
						}
						catch(Exception e)
						{
							General.ErrorLogger.Add(ErrorType.Error, "Cannot lock texture \"" + this.Name + "\" to apply render style. " + e.GetType().Name + ": " + e.Message);
						}

						if(texturebmpdata != null)
						{
							PixelColor* texturepixels = (PixelColor*)(texturebmpdata.Scan0.ToPointer());
							PixelColor* tcp = texturepixels + numpixels - 1;

							switch(p.RenderStyle)
							{
								case TexturePathRenderStyle.ADD:
									for(PixelColor* cp = pixels + numpixels - 1; cp >= pixels; cp--)
									{
										cp->r = (byte)Math.Min(255, cp->r + tcp->r);
										cp->g = (byte)Math.Min(255, cp->g + tcp->g);
										cp->b = (byte)Math.Min(255, cp->b + tcp->b);
										cp->a = (byte)((cp->a * patchalpha) * PixelColor.BYTE_TO_FLOAT);
										tcp--;
									}
									break;

								case TexturePathRenderStyle.SUBTRACT:
									for(PixelColor* cp = pixels + numpixels - 1; cp >= pixels; cp--)
									{
										cp->r = (byte)Math.Max(0, tcp->r - cp->r);
										cp->g = (byte)Math.Max(0, tcp->g - cp->g);
										cp->b = (byte)Math.Max(0, tcp->b - cp->b);
										cp->a = (byte)((cp->a * patchalpha) * PixelColor.BYTE_TO_FLOAT);
										tcp--;
									}
									break;

								case TexturePathRenderStyle.REVERSE_SUBTRACT:
									for(PixelColor* cp = pixels + numpixels - 1; cp >= pixels; cp--)
									{
										cp->r = (byte)Math.Max(0, cp->r - tcp->r);
										cp->g = (byte)Math.Max(0, cp->g - tcp->g);
										cp->b = (byte)Math.Max(0, cp->b - tcp->b);
										cp->a = (byte)((cp->a * patchalpha) * PixelColor.BYTE_TO_FLOAT);
										tcp--;
									}
									break;

								case TexturePathRenderStyle.MODULATE:
									for(PixelColor* cp = pixels + numpixels - 1; cp >= pixels; cp--)
									{
										cp->r = (byte)((cp->r * tcp->r) * PixelColor.BYTE_TO_FLOAT);
										cp->g = (byte)((cp->g * tcp->g) * PixelColor.BYTE_TO_FLOAT);
										cp->b = (byte)((cp->b * tcp->b) * PixelColor.BYTE_TO_FLOAT);
										tcp--;
									}
									break;

							}

							source.UnlockBits(texturebmpdata);
						}
					}

					patchbmp.UnlockBits(bmpdata);
				}
			}

			return patchbmp;
		}

		#endregion
	}
}
