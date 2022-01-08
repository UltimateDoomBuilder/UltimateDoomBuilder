#region ================== Copyright

/*
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
 */

#endregion

#region ================== Namespaces

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.CompilerServices;
using CodeImp.DoomBuilder.Rendering;

#endregion

namespace CodeImp.DoomBuilder.Data
{
	public sealed class ColorMap
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		private PixelColor[] colors;

		#endregion

		#region ================== Properties

		public PixelColor this[int index] { get { return colors[index]; } }
		
		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public ColorMap()
		{
			colors = new PixelColor[34 * 256];
			for(int i = 0; i < 34 * 256; i++)
			{
				// Set colors to gray
				colors[i].r = 127;
				colors[i].g = 127;
				colors[i].b = 127;
				colors[i].a = 255;
			}
		}

		// Constructor
		public ColorMap(Stream stream, Playpal palette)
		{
			BinaryReader reader = new BinaryReader(stream);
			var colors = new List<PixelColor>();
			
			// Read all palette entries
			stream.Seek(0, SeekOrigin.Begin);
			while (stream.Position < stream.Length)
			{
				// Read colors
				var index = reader.ReadByte();
				if (index < palette.Length)
				{
					var color = palette[index];
					colors.Add(color);					
				}
				else
				{
					colors.Add(PixelColor.Transparent);
				}
			}

			this.colors = colors.ToArray();
		}

		#endregion

		#region ================== Methods

		public Bitmap CreateBitmap() {
			var width = 256;
			var height = (int)Math.Ceiling((float)colors.Length / 256);
			var bitmap = new Bitmap(width, height, PixelFormat.Format32bppRgb);
			
			for (int y = 0; y < height; y++) {
				for (int x = 0; x < width; x++) {
					int index = width * y + x;
					if (index < colors.Length)
					{
						bitmap.SetPixel(x, y, colors[index].ToColor());						
					}
				}
			}
			return bitmap;
		}

		#endregion
	}
}
