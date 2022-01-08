
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
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using CodeImp.DoomBuilder.Rendering;

#endregion

namespace CodeImp.DoomBuilder.Data
{
	public sealed class Playpal
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		private PixelColor[] colors;

		#endregion

		#region ================== Properties

		public PixelColor this[int index] { get { return colors[index]; } }
		public int Length { get { return colors.Length;  } }
		
		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public Playpal()
		{
			// Create array
			colors = new PixelColor[256];

			// Set all palette entries
			for(int i = 0; i < 256; i++)
			{
				// Set colors to gray
				colors[i].r = 127;
				colors[i].g = 127;
				colors[i].b = 127;
				colors[i].a = 255;
			}
		}

		// Constructor
		public Playpal(Stream stream)
		{
			BinaryReader reader = new BinaryReader(stream);
			
			// Create array
			colors = new PixelColor[256];

			// Read all palette entries
			stream.Seek(0, SeekOrigin.Begin);
			for(int i = 0; i < 256; i++)
			{
				// Read colors
				colors[i].r = reader.ReadByte();
				colors[i].g = reader.ReadByte();
				colors[i].b = reader.ReadByte();
				colors[i].a = 255;
			}
		}

		#endregion

		#region ================== Methods

		public Bitmap CreateBitmap() {
			var bitmap = new Bitmap(16, 16, PixelFormat.Format32bppRgb);
			for (int y = 0; y < 16; y++) {
				for (int x = 0; x < 16; x++) {
					int index = 16 * y + x;
					if (index < colors.Length)
					{
						bitmap.SetPixel(x, y, colors[index].ToColor());						
					}
				}
			}
			return bitmap;
		}

		public int FindClosestColor(PixelColor match)
		{
			float minDist = 99999;
			int minIndex = 0;
			for (int i = 0; i < colors.Length; i++)
			{
				PixelColor color = colors[i];
				float dr = (float)match.r - (float)color.r;
				float dg = (float)match.g - (float)color.g;
				float db = (float)match.b - (float)color.b;
				float sqDist = dr * dr + dg * dg + db * db;
				if (sqDist < minDist)
				{
					minIndex = i;
					minDist = sqDist;
				}
			}

			return minIndex;
		}

		#endregion
	}
}
