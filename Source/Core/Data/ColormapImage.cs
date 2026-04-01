
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
using System.IO;

#endregion

namespace CodeImp.DoomBuilder.Data
{
	internal sealed class ColormapImage : ImageData
	{
		#region ================== Constructor / Disposer

		// Constructor
		public ColormapImage(string name)
		{
			// Initialize
			SetName(name);
			virtualname = "[Colormaps]/" + this.name; //mxd

			// We have no destructor
			GC.SuppressFinalize(this);
		}

		#endregion

		#region ================== Methods

		// This loads the image
		protected override LocalLoadResult LocalLoadImage()
		{
            Bitmap bitmap = null;
            string error = null;

			// Get the lump data stream
			Stream lumpdata = General.Map.Data.GetColormapData(Name);
			if(lumpdata != null)
			{
				// Copy lump data to memory
				lumpdata.Seek(0, SeekOrigin.Begin);
				byte[] membytes = new byte[(int)lumpdata.Length];
				lumpdata.Read(membytes, 0, (int)lumpdata.Length);
				MemoryStream mem = new MemoryStream(membytes);
				mem.Seek(0, SeekOrigin.Begin);

				// Get a reader for the data
				bitmap = ImageDataFormat.TryLoadImage(mem, ImageDataFormat.DOOMCOLORMAP, General.Map.Data.Palette);
				if(bitmap == null)
				{
					// Data is in an unknown format!
					error = "Colormap lump \"" + Name + "\" data format could not be read. Does this lump contain valid colormap data at all?";
				}

				// Done
				mem.Dispose();
			}
			else
			{
				// Missing a patch lump!
				error = "Missing colormap lump \"" + Name + "\". Did you forget to include required resources?";
			}

            return new LocalLoadResult(bitmap, error, () =>
            {
                scale.x = General.Map.Config.DefaultFlatScale;
                scale.y = General.Map.Config.DefaultFlatScale;
            });
		}

		#endregion
	}
}
