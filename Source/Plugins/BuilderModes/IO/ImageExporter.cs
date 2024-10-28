#region ================== Copyright (c) 2020 Boris Iwanski

/*
 * This program is free software: you can redistribute it and/or modify
 *
 * it under the terms of the GNU General Public License as published by
 * 
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 *    but WITHOUT ANY WARRANTY; without even the implied warranty of
 * 
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
 * 
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.If not, see<http://www.gnu.org/licenses/>.
 */

#endregion

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.IO;
using System.Runtime.InteropServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Map;
using System.Diagnostics;

namespace CodeImp.DoomBuilder.BuilderModes.IO
{
	#region ================== Structs

	internal struct ImageExportSettings
	{
		public string Path;
		public string Name;
		public string Extension;
		public bool Floor;
		public bool Fullbright;
		public bool ApplySectorColors;
		public bool Brightmap;
		public bool Tiles;
		public bool Transparency;
		public PixelFormat PixelFormat;
		public ImageFormat ImageFormat;
		public float Scale;

		public ImageExportSettings(string path, string name, string extension, bool floor, bool fullbright, bool applysectorcolors, bool brightmap, bool transparency, bool tiles, float scale, PixelFormat pformat, ImageFormat iformat)
		{
			Path = path;
			Name = name;
			Extension = extension;
			Floor = floor;
			Brightmap = brightmap;
			Tiles = tiles;
			Fullbright = fullbright;
			Transparency = transparency;
			ApplySectorColors = applysectorcolors;
			PixelFormat = pformat;
			ImageFormat = iformat;
			Scale = scale;
		}
	}

	#endregion

	#region ================== Exceptions

	[Serializable]
	public class ImageExportCanceledException : Exception { }

	[Serializable]
	public class ImageExportImageTooBigException : Exception { }

	#endregion

	internal class ImageExporter
	{
		#region ================== Variables

		private ICollection<Sector> sectors;
		private ImageExportSettings settings;
		private int numitems;
		private int doneitems;
		private int donepercent;
		private Action addprogress;
		private Action<string> showphase;
		private Func<bool> checkcanelexport;

		#endregion

		#region ================== Constants

		private const int TILE_SIZE = 64;

		#endregion

		#region ================== Constructors

		public ImageExporter(ICollection<Sector> sectors, ImageExportSettings settings, Action addprogress, Action<string> showphase, Func<bool> checkcanelexport)
		{
			this.sectors = sectors;
			this.settings = settings;
			this.addprogress = addprogress;
			this.showphase = showphase;
			this.checkcanelexport = checkcanelexport;
		}

		#endregion

		#region ================== Methods

		/// <summary>
		/// Exports the sectors to images
		/// </summary>
		public void Export()
		{
			Vector2D size;
			Vector2D offset;

			GetSizeAndOffset(out size, out offset);

			// Count the number of triangles for reporting progress
			numitems = 0;
			doneitems = 0;
			donepercent = 0;
			foreach (Sector s in sectors)
				numitems += s.Triangles.Vertices.Count / 3;

			if (settings.Tiles)
				numitems += GetNumTiles();

			// If exporting a brightmap everything has to be done twice
			if (settings.Brightmap)
				numitems *= 2;

			// Use the same image for the normal texture and the brightmap because of memory concerns
			showphase("Preparing");
			using (Bitmap image = new Bitmap((int)(size.x * settings.Scale), (int)(size.y * settings.Scale), settings.PixelFormat))
			{
				showphase("Creating normal image");
				// Normal texture image
				CreateImage(image, offset, settings.Scale, false);

				if (settings.Tiles)
				{
					showphase("Saving 64x64 tile images (" + GetNumTiles() + ")");
					SaveImageAsTiles(image);
				}
				else
				{
					showphase("Saving normal image");
					try
					{
						image.Save(Path.Combine(settings.Path, settings.Name) + settings.Extension, settings.ImageFormat);
					}
					catch(ExternalException)
					{
						throw new ImageExportImageTooBigException();
					}
				}

				// The brightmap
				if (settings.Brightmap)
				{
					showphase("Creating brightmap image");
					CreateImage(image, offset, settings.Scale, true);

					showphase("Saving brightmap image");
					if (settings.Tiles)
					{
						showphase("Saving 64x64 tile images (" + GetNumTiles() + ")");
						SaveImageAsTiles(image, "_brightmap");
					}
					else
					{
						image.Save(Path.Combine(settings.Path, settings.Name) + "_brightmap" + settings.Extension, settings.ImageFormat);
						showphase("Saving normal image");
					}
				}
			}
		}

		/// <summary>
		/// Create the image ready to be exported
		/// </summary>
		/// <param name="texturebitmap">The image the graphics will be drawn to</param>
		/// <param name="offset">The offset of the selection in map space</param>
		/// <param name="asbrightmap">True if the image should be a brightmap, false if normally textured</param>
		/// <returns>The image to be exported</returns>
		private void CreateImage(Bitmap texturebitmap, Vector2D offset, float scale, bool asbrightmap)
		{
			// The texture
			using (Graphics gtexture = Graphics.FromImage(texturebitmap))
			{
				if (!settings.Transparency)
				{
					gtexture.Clear(Color.Black); // If we don't clear to black we'll see seams where the sectors touch, due to the AA
					gtexture.SmoothingMode = SmoothingMode.AntiAlias; // Without AA the sector edges will be quite rough
				}

				gtexture.InterpolationMode = InterpolationMode.HighQualityBilinear;
				gtexture.CompositingQuality = CompositingQuality.HighQuality;
				gtexture.PixelOffsetMode = PixelOffsetMode.HighQuality;				

				using (GraphicsPath gpath = new GraphicsPath())
				{
					foreach (Sector s in sectors)
					{
						float rotation = (float)s.Fields.GetValue("rotationfloor", 0.0);

						// If a sector is rotated any offset is on the rotated axes. But we need to offset by
						// map coordinates. We'll use this vector to compute that offset
						Vector2D rotationvector = Vector2D.FromAngle(Angle2D.DegToRad(rotation) + Angle2D.PIHALF);

						// Sectors are triangulated, so draw every triangle
						for (int i = 0; i < s.Triangles.Vertices.Count / 3; i++)
						{
							// The GDI image has the 0/0 coordinate in the top left, so invert the y component
							Vector2D v1 = (s.Triangles.Vertices[i * 3] - offset) * scale; v1.y *= -1.0;
							Vector2D v2 = (s.Triangles.Vertices[i * 3 + 1] - offset) * scale; v2.y *= -1.0;
							Vector2D v3 = (s.Triangles.Vertices[i * 3 + 2] - offset) * scale; v3.y *= -1.0;

							gpath.AddLine((float)v1.x, (float)v1.y, (float)v2.x, (float)v2.y);
							gpath.AddLine((float)v2.x, (float)v2.y, (float)v3.x, (float)v3.y);
							gpath.CloseFigure();

							doneitems++;

							int newpercent = (int)(((double)doneitems / numitems) * 100);
							if (newpercent > donepercent)
							{
								donepercent = newpercent;
								addprogress();

								if (checkcanelexport())
									throw new ImageExportCanceledException();
							}
						}

						if (asbrightmap)
						{
							// Create the brightmap based on the sector brightness
							int brightness = General.Clamp(s.Brightness, 0, 255);
							using (SolidBrush sbrush = new SolidBrush(Color.FromArgb(255, brightness, brightness, brightness)))
								gtexture.FillPath(sbrush, gpath);
						}
						else
						{
							Bitmap brushtexture;
							Vector2D textureoffset = new Vector2D();
							Vector2D texturescale = new Vector2D();

							if (settings.Floor)
							{
								// The image might have a color correction applied, but we need it without. So we use LocalGetBitmap, because it reloads the image,
								// but doesn't applie the color correction if we set UseColorCorrection to false first
								ImageData imagedata = General.Map.Data.GetFlatImage(s.FloorTexture);
								brushtexture = new Bitmap(imagedata.LocalGetBitmap(false));

								textureoffset.x = s.Fields.GetValue("xpanningfloor", 0.0) * scale;
								textureoffset.y = s.Fields.GetValue("ypanningfloor", 0.0) * scale;

								// GZDoom uses bigger numbers for smaller scales (i.e. a scale of 2 will halve the size), so we need to change the scale
								texturescale.x = 1.0 / s.Fields.GetValue("xscalefloor", 1.0);
								texturescale.y = 1.0 / s.Fields.GetValue("yscalefloor", 1.0);

								// Take texture scale (for example from the TEXTURES lump) into account
								texturescale *= 1.0 / imagedata.Scale;
							}
							else
							{
								// The image might have a color correction applied, but we need it without. So we use LocalGetBitmap, because it reloads the image,
								// but doesn't applie the color correction if we set UseColorCorrection to false first
								ImageData imagedata = General.Map.Data.GetFlatImage(s.CeilTexture);
								brushtexture = new Bitmap(imagedata.LocalGetBitmap(false));

								textureoffset.x = s.Fields.GetValue("xpanningceiling", 0.0) * scale;
								textureoffset.y = s.Fields.GetValue("ypanningceiling", 0.0) * scale;

								// GZDoom uses bigger numbers for smaller scales (i.e. a scale of 2 will halve the size), so we need to change the scale
								texturescale.x = 1.0 / s.Fields.GetValue("xscaleceiling", 1.0);
								texturescale.y = 1.0 / s.Fields.GetValue("yscaleceiling", 1.0);

								// Take texture scale (for example from the TEXTURES lump) into account
								texturescale *= 1.0 / imagedata.Scale;
							}

							// Create the transformation matrix
							Matrix matrix = new Matrix();
							matrix.Rotate(rotation);
							matrix.Translate((float)(-offset.x * scale * rotationvector.x), (float)(offset.x * scale * rotationvector.y)); // Left/right offset from the map origin
							matrix.Translate((float)(offset.y * scale * rotationvector.y), (float)(offset.y * scale * rotationvector.x)); // Up/down offset from the map origin
							matrix.Translate(-(float)textureoffset.x, -(float)textureoffset.y); // Texture offset 

							// Resize the brush texture if the texture is scaled
							if (texturescale.x != 1.0 || texturescale.y != 1.0)
								ResizeImage(ref brushtexture, (int)(brushtexture.Width * texturescale.x), (int)(brushtexture.Height * texturescale.y));

							if (!settings.Fullbright)
							{
								int brightness = General.Clamp(s.Brightness, 0, 255);
								AdjustBrightness(ref brushtexture, brightness > 0 ? brightness / 255.0f : 0.0f);
							}

							// Take sector colors into account
							if (settings.ApplySectorColors)
							{
								int lightcolor = s.Fields.GetValue("lightcolor", 0xffffff);
								int surfacecolor = settings.Floor ? s.Fields.GetValue("color_floor", 0xffffff) : s.Fields.GetValue("color_ceiling", 0xffffff);
								Rendering.Color4 color = Rendering.PixelColor.Modulate(Rendering.PixelColor.FromInt(lightcolor), Rendering.PixelColor.FromInt(surfacecolor)).ToColorValue();
								Colorize(ref brushtexture, color.Red, color.Green, color.Blue);
							}

							if (scale > 1.0f)
								ResizeImage(ref brushtexture, brushtexture.Width * (int)scale, brushtexture.Height * (int)scale);

							// Create the texture brush and apply the matrix
							TextureBrush tbrush = new TextureBrush(brushtexture);
							tbrush.Transform = matrix;

							// Draw the islands of the sector
							gtexture.FillPath(tbrush, gpath);

							// Dispose unneeded objects
							brushtexture.Dispose();
							tbrush.Dispose();
							matrix.Dispose();
						}

						// Reset the graphics path
						gpath.Reset();
					}
				}
			}
		}

		/// <summary>
		/// Saves the image in several uniform sized tiles. It will add numbers starting from 1, going from top left to bottom right, to the filename
		/// </summary>
		/// <param name="image">the image to split into tiles</param>
		/// <param name="suffix">additional suffix for filenames</param>
		private void SaveImageAsTiles(Bitmap image, string suffix="")
		{
			int xnum = (int)Math.Ceiling(image.Size.Width / (double)TILE_SIZE);
			int ynum = (int)Math.Ceiling(image.Size.Height / (double)TILE_SIZE);
			int imagenum = 1;

			for (int y = 0; y < ynum; y++)
			{
				for (int x = 0; x < xnum; x++)
				{
					int width = TILE_SIZE;
					int height = TILE_SIZE;

					// If the width and height are not divisible without remainder make sure only part of the source image is copied
					if (x * TILE_SIZE + TILE_SIZE > image.Size.Width)
						width = image.Size.Width - (x * TILE_SIZE);

					if(y * TILE_SIZE + TILE_SIZE > image.Size.Height)
						height = image.Size.Height - (y * TILE_SIZE);

					using (Bitmap bitmap = new Bitmap(TILE_SIZE, TILE_SIZE))
					using (Graphics g = Graphics.FromImage(bitmap))
					{
						g.Clear(Color.Black);
						g.DrawImage(image, new Rectangle(0, 0, width, height), new Rectangle(x * TILE_SIZE, y * TILE_SIZE, width, height), GraphicsUnit.Pixel);

						bitmap.Save(string.Format("{0}{1}{2}{3}", Path.Combine(settings.Path, settings.Name), suffix, imagenum, settings.Extension));
					}

					imagenum++;

					doneitems++;

					int newpercent = (int)(((double)doneitems / numitems) * 100);
					if (newpercent > donepercent)
					{
						donepercent = newpercent;
						addprogress();

						if (checkcanelexport())
							throw new ImageExportCanceledException();
					}

					if (checkcanelexport())
						throw new ImageExportCanceledException();
				}
			}
		}

		/// <summary>
		/// Generates a list of images file names that will be creates
		/// </summary>
		/// <returns>List of image file names</returns>
		public List<string> GetImageNames()
		{
			List<string> imagenames = new List<string>();
			Vector2D offset;
			Vector2D size;

			GetSizeAndOffset(out size, out offset);

			if(settings.Tiles)
			{
				int x = (int)Math.Ceiling(size.x / TILE_SIZE);
				int y = (int)Math.Ceiling(size.y / TILE_SIZE);

				for (int i = 1; i <= x * y; i++)
					imagenames.Add(string.Format("{0}{1}{2}", Path.Combine(settings.Path, settings.Name), i, settings.Extension));

				if(settings.Brightmap)
					for (int i = 1; i <= x * y; i++)
						imagenames.Add(string.Format("{0}{1}_brightmap{2}", Path.Combine(settings.Path, settings.Name), i, settings.Extension));
			}
			else
			{
				imagenames.Add(string.Format("{0}{1}", Path.Combine(settings.Path, settings.Name), settings.Extension));

				if(settings.Brightmap)
					imagenames.Add(string.Format("{0}_brightmap{1}", Path.Combine(settings.Path, settings.Name), settings.Extension));
			}

			return imagenames;
		}

		/// <summary>
		/// Gets the total number of tiles
		/// </summary>
		/// <returns>Number of tiles</returns>
		public int GetNumTiles()
		{
			Vector2D size;
			Vector2D offset;

			GetSizeAndOffset(out size, out offset);

			int xnum = (int)Math.Ceiling(size.x * settings.Scale / (double)TILE_SIZE);
			int ynum = (int)Math.Ceiling(size.y * settings.Scale / (double)TILE_SIZE);

			return xnum * ynum;
		}

		/// <summary>
		/// Gets the size of the image, based on the sectors, and the offset from the map origin
		/// </summary>
		/// <param name="size">stores the size of the size of the image</param>
		/// <param name="offset">stores the offset from the map origin</param>
		private void GetSizeAndOffset(out Vector2D size, out Vector2D offset)
		{
			offset = new Vector2D(double.MaxValue, double.MinValue);
			size = new Vector2D(double.MinValue, double.MaxValue);

			// Find the top left and bottom right corners of the selection
			foreach (Sector s in sectors)
			{
				foreach (Sidedef sd in s.Sidedefs)
				{
					foreach (Vertex v in new Vertex[] { sd.Line.Start, sd.Line.End })
					{
						if (v.Position.x < offset.x)
							offset.x = v.Position.x;

						if (v.Position.x > size.x)
							size.x = v.Position.x;

						if (v.Position.y > offset.y)
							offset.y = v.Position.y;

						if (v.Position.y < size.y)
							size.y = v.Position.y;
					}
				}
			}

			// Right now "size" is the bottom right corener of the selection, so subtract the offset
			// (top left corner of the selection). y will always be negative, so make it positive
			size -= offset;
			size.y *= -1.0;
		}

		/// <summary>
		/// Adjusts the brightness of an image. Code by Rod Stephens http://csharphelper.com/blog/2014/10/use-an-imageattributes-object-to-adjust-an-images-brightness-in-c/
		/// </summary>
		/// <param name="image">The image to adjust</param>
		/// <param name="brightness">Brightness between 0.0f and 1.0f</param>
		private void AdjustBrightness(ref Bitmap image, float brightness)
		{
			// Make the ColorMatrix.
			float b = brightness;
			ColorMatrix cm = new ColorMatrix(new float[][] {
				new float[] {b, 0, 0, 0, 0},
				new float[] {0, b, 0, 0, 0},
				new float[] {0, 0, b, 0, 0},
				new float[] {0, 0, 0, 1, 0},
				new float[] {0, 0, 0, 0, 1},
			});
			ImageAttributes attributes = new ImageAttributes();
			attributes.SetColorMatrix(cm);

			// Draw the image onto the new bitmap while applying the new ColorMatrix.
			Point[] points = {
				new Point(0, 0),
				new Point(image.Width, 0),
				new Point(0, image.Height),
			};
			Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);

			// Make the result bitmap.
			Bitmap bm = new Bitmap(image.Width, image.Height);
			using (Graphics gr = Graphics.FromImage(bm))
			{
				gr.DrawImage(image, points, rect, GraphicsUnit.Pixel, attributes);
			}

			// Dispose the original...
			image.Dispose();

			// ... and set it as the adjusted image
			image = bm;
		}

		private void Colorize(ref Bitmap image, float r, float g, float b)
		{
			// Make the ColorMatrix.
			ColorMatrix cm = new ColorMatrix(new float[][] {
				new float[] {r, 0, 0, 0, 0},
				new float[] {0, g, 0, 0, 0},
				new float[] {0, 0, b, 0, 0},
				new float[] {0, 0, 0, 1, 0},
				new float[] {0, 0, 0, 0, 1},
			});
			ImageAttributes attributes = new ImageAttributes();
			attributes.SetColorMatrix(cm);

			// Draw the image onto the new bitmap while applying the new ColorMatrix.
			Point[] points = {
				new Point(0, 0),
				new Point(image.Width, 0),
				new Point(0, image.Height),
			};
			Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);

			// Make the result bitmap.
			Bitmap bm = new Bitmap(image.Width, image.Height);
			using (Graphics gr = Graphics.FromImage(bm))
			{
				gr.DrawImage(image, points, rect, GraphicsUnit.Pixel, attributes);
			}

			// Dispose the original...
			image.Dispose();

			// ... and set it as the adjusted image
			image = bm;
		}

		/// <summary>
		/// Resize the image to the specified width and height. Taken from https://stackoverflow.com/a/24199315 (with some modifications)
		/// </summary>
		/// <param name="image">The image to resize.</param>
		/// <param name="width">The width to resize to.</param>
		/// <param name="height">The height to resize to.</param>
		/// <returns>The resized image.</returns>
		private void ResizeImage(ref Bitmap image, int width, int height)
		{
			var destRect = new Rectangle(0, 0, width, height);
			var destImage = new Bitmap(width, height);

			destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

			using (var graphics = Graphics.FromImage(destImage))
			{
				graphics.CompositingMode = CompositingMode.SourceCopy;
				graphics.CompositingQuality = CompositingQuality.HighQuality;
				graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
				graphics.SmoothingMode = SmoothingMode.HighQuality;
				graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

				using (var wrapMode = new ImageAttributes())
				{
					wrapMode.SetWrapMode(WrapMode.TileFlipXY);
					graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
				}
			}

			image.Dispose();

			image = destImage;
		}

		#endregion
	}
}
