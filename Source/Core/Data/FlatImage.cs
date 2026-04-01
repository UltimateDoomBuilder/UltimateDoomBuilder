
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
	internal sealed class FlatImage : ImageData
	{
		#region ================== Constructor / Disposer

		// Constructor
		public FlatImage(string name)
		{
			// Initialize
			SetName(name);
			virtualname = "[Flats]/" + this.name; //mxd
			texturenamespace = TextureNamespace.FLAT;
			
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
			string flatlocation = string.Empty; //mxd
			Stream lumpdata = General.Map.Data.GetFlatData(Name, hasLongName, ref flatlocation);
			if(lumpdata != null)
			{
				// Copy lump data to memory
				byte[] membytes = new byte[(int)lumpdata.Length];

				lock(lumpdata) //mxd
				{
					lumpdata.Seek(0, SeekOrigin.Begin);
					lumpdata.Read(membytes, 0, (int)lumpdata.Length);
				}
					
				MemoryStream mem = new MemoryStream(membytes);
				mem.Seek(0, SeekOrigin.Begin);

				// Get a reader for the data
				bitmap = ImageDataFormat.TryLoadImage(mem, ImageDataFormat.DOOMFLAT, General.Map.Data.Palette);
				if(bitmap == null)
				{
					// Data is in an unknown format!
					error = "Flat lump \"" + Path.Combine(flatlocation, Name) + "\" data format could not be read. Does this lump contain valid picture data at all?";
				}

				// Done
				mem.Dispose();
			}
			else
			{
				// Missing a patch lump!
				error = "Missing flat lump \"" + Name + "\". Did you forget to include required resources?";
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
