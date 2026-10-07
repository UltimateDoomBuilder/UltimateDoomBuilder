#region ================== Namespaces

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using CodeImp.DoomBuilder.Rendering;

#endregion

namespace CodeImp.DoomBuilder.Data
{
	public sealed class VoxelImage : ImageData, ISpriteImage
	{
		#region ================== Variables

		private readonly string voxelname;
		private bool overridepalette;
		private int angleoffset;
		
		#endregion

		#region ================== Properties

		public string VoxelName { get { return voxelname; } }
		public bool OverridePalette { get { return overridepalette; } internal set { overridepalette = value; } }
		public int AngleOffset { get { return angleoffset; } internal set { angleoffset = value; } }

		#endregion
		
		#region ================== Constructor / Disposer

		// Constructor
		internal VoxelImage(string name, string voxelname)
		{
			// Initialize
			SetName(name);
			this.voxelname = voxelname;

			// We have no destructor
			GC.SuppressFinalize(this);
		}

		#endregion

		#region ================== Methods

		// This loads the image
		protected unsafe override LocalLoadResult LocalLoadImage()
		{
            Bitmap bitmap = null;
            string error = null;

            int imgoffsetx = 0;
            int pivotz = 0;

            // Get the lump data stream
            string voxellocation = string.Empty; //mxd
			Stream lumpdata = General.Map.Data.GetVoxelData(voxelname, ref voxellocation);
			if(lumpdata != null)
			{
				// Copy lump data to memory
				lumpdata.Seek(0, SeekOrigin.Begin);
				byte[] membytes = new byte[(int)lumpdata.Length];
				lumpdata.Read(membytes, 0, (int)lumpdata.Length);
					
				using(MemoryStream mem = new MemoryStream(membytes))
				{
					mem.Seek(0, SeekOrigin.Begin);
					PixelColor[] palette = new PixelColor[256];

					// Create front projection image from the KVX
					using(BinaryReader reader = new BinaryReader(mem, Encoding.ASCII))
					{
						reader.ReadInt32(); //numbytes, we don't use that
						int xsize = reader.ReadInt32();
						int ysize = reader.ReadInt32();
						int zsize = reader.ReadInt32();

						// Sanity check
						if(xsize == 0 || ysize == 0 || zsize == 0)
						{
							error =  "Cannot create sprite image for voxel \"" + Path.Combine(voxellocation, voxelname) 
								+ "\" for voxel drawing: voxel has invalid size (width: " + xsize + ", height: " + zsize + ", depth: " + ysize;

                            return new LocalLoadResult(null, error);
						}

						int pivotx = (int)Math.Round(reader.ReadInt32() / 256f);
						int pivoty = (int)Math.Round(reader.ReadInt32() / 256f);
						pivotz = (int)Math.Round(reader.ReadInt32() / 256f);

						// Read offsets
						int[] xoffset = new int[xsize + 1]; // why is it xsize + 1, not xsize?..
						short[,] xyoffset = new short[xsize, ysize + 1]; // why is it ysize + 1, not ysize?..

						for(int i = 0; i < xoffset.Length; i++)
						{
							xoffset[i] = reader.ReadInt32();
						}

						for(int x = 0; x < xsize; x++)
						{
							for(int y = 0; y < ysize + 1; y++)
							{
								xyoffset[x, y] = reader.ReadInt16();
							}
						}

						// Read slabs
						List<int> offsets = new List<int>(xsize * ysize);
						for(int x = 0; x < xsize; x++)
						{
							for(int y = 0; y < ysize; y++)
							{
								offsets.Add(xoffset[x] + xyoffset[x, y] + 28); // for some reason offsets are counted from start of xoffset[]...
							}
						}

						int counter = 0;
						int slabsend = (int)(reader.BaseStream.Length - 768);

						// Read palette
						if(!overridepalette)
						{
							reader.BaseStream.Position = slabsend;
							for(int i = 0; i < 256; i++)
							{
								byte r = (byte)(reader.ReadByte() * 4);
								byte g = (byte)(reader.ReadByte() * 4);
								byte b = (byte)(reader.ReadByte() * 4);
								palette[i] = new PixelColor(255, r, g, b);
							}
						}
						else
						{
							for(int i = 0; i < 256; i++) palette[i] = General.Map.Data.Palette[i];
						}

						// Populate projection pixels array
						int imgwidth, imgheight;
						bool checkalpha = false;

						// Convert angleoffsets to the nearest cardinal direction...
						angleoffset = General.ClampAngle((angleoffset + 45) / 90 * 90);

						switch(angleoffset)
						{
							case 0:
								imgwidth = xsize;
								imgheight = zsize;
								imgoffsetx = pivotx;
								break;

							case 90:
								imgwidth = ysize;
								imgheight = zsize;
								imgoffsetx = imgwidth - pivoty;
								checkalpha = true;
								break;

							case 180:
								imgwidth = xsize;
								imgheight = zsize;
								imgoffsetx = imgwidth - pivotx;
								checkalpha = true;
								break;

							case 270:
								imgwidth = ysize;
								imgheight = zsize;
								imgoffsetx = pivoty;
								break;

							default: throw new InvalidDataException("Invalid AngleOffset");
						}

						int numpixels = imgwidth * imgheight;
						PixelColor[] pixelsarr = new PixelColor[numpixels];

						// Read pixel colors
						for(int x = 0; x < xsize; x++)
						{
							for(int y = 0; y < ysize; y++)
							{
								reader.BaseStream.Position = offsets[counter];
								int next = (counter < offsets.Count - 1 ? offsets[counter + 1] : slabsend);

								// Read first color from the slab
								while(reader.BaseStream.Position < next)
								{
									int ztop = reader.ReadByte();
									int zleng = reader.ReadByte();
									if(ztop + zleng > zsize) break;
									byte flags = reader.ReadByte();

									if(zleng > 0)
									{
										// Skip slab if no flags are given (otherwise some garbage pixels may be drawn)
										if(flags == 0)
										{
											reader.BaseStream.Position += zleng;
											continue;
										}
											
										List<int> colorindices = new List<int>(zleng);
										for(int i = 0; i < zleng; i++)
										{
											colorindices.Add(reader.ReadByte());
										}

										int z = ztop;
										int cstart = 0;
										while(z < ztop + zleng)
										{
											// Get pixel position
											int pixelpos;
											switch(angleoffset)
											{
												case 0:   pixelpos = x + z * xsize; break;
												case 90:  pixelpos = y + z * ysize; break;
												case 180: pixelpos = xsize - x - 1 + z * xsize; break;
												case 270: pixelpos = ysize - y - 1 + z * ysize; break;
												default: throw new InvalidDataException("Invalid AngleOffset");
											}

											// Add to projection pixels array
											if((checkalpha && pixelsarr[pixelpos].a == 0) || !checkalpha)
												pixelsarr[pixelpos] = palette[colorindices[cstart]];

											// Increment counters
											cstart++;
											z++;
										}
									}
								}

								counter++;
							}
						}

						// Draw to bitmap
						bitmap = new Bitmap(imgwidth, imgheight, PixelFormat.Format32bppArgb);
						BitmapData bmpdata = null;

						try
						{
							bmpdata = bitmap.LockBits(new Rectangle(0, 0, imgwidth, imgheight), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
						}
						catch(Exception e)
						{
							error = "Cannot lock image for drawing voxel \""
								+ Path.Combine(voxellocation, voxelname) + "\". " + e.GetType().Name + ": " + e.Message;
							bitmap = null;
						}

						if(bmpdata != null)
						{
							// Apply pixels to image
							PixelColor* pixels = (PixelColor*)bmpdata.Scan0.ToPointer();
							int i = 0;

							for(PixelColor* cp = pixels; cp < pixels + numpixels; cp++, i++)
							{
								if(pixelsarr[i].a == 255)
								{
									cp->r = pixelsarr[i].r;
									cp->g = pixelsarr[i].g;
									cp->b = pixelsarr[i].b;
									cp->a = 255;
								}
							}

							bitmap.UnlockBits(bmpdata);
						}
					}
				}

				lumpdata.Dispose();
			}
			else
			{
				// Missing voxel lump!
				error = "Missing voxel lump \"" + voxelname + "\". Forgot to include required resources?";
			}

            return new LocalLoadResult(bitmap, error, () =>
            {
                scale.x = 1.0f;
                scale.y = 1.0f;
                offsetx = imgoffsetx;
                offsety = pivotz;
            });
		}

		#endregion
	}
}
