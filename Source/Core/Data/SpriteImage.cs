
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
using System.Runtime.InteropServices;
using CodeImp.DoomBuilder.IO;
using CodeImp.DoomBuilder.Windows;

#endregion

namespace CodeImp.DoomBuilder.Data
{
	public interface ISpriteImage //mxd
	{
		int OffsetX { get; }
		int OffsetY { get; }
	}

	public sealed class SpriteImage : ImageData, ISpriteImage
	{
		#region ================== Constructor / Disposer

		// Constructor
		internal SpriteImage(string name)
		{
			// Initialize
			SetName(name);

			texturenamespace = TextureNamespace.SPRITE;

            AllowUnload = false;

			// We have no destructor
			GC.SuppressFinalize(this);
		}

		#endregion

		#region ================== Methods

		// This loads the image
		protected override LocalLoadResult LocalLoadImage()
		{
            // Get the lump data stream
            Bitmap bitmap = null;
            string error = null;
			string spritelocation = string.Empty; //mxd
			Stream lumpdata = General.Map.Data.GetSpriteData(Name, ref spritelocation);
			if(lumpdata != null)
			{
				// Get a reader for the data
				bitmap = ImageDataFormat.TryLoadImage(lumpdata, ImageDataFormat.DOOMPICTURE, General.Map.Data.Palette, out offsetx, out offsety);
				if(bitmap == null)
				{
					// Data is in an unknown format!
					error = "Sprite lump \"" + Path.Combine(spritelocation, Name) + "\" data format could not be read. Does this lump contain valid picture data at all?";
				}
					
				// Done
                lumpdata.Close();
			}
			else
			{
				// Missing a patch lump!
				error = "Missing sprite lump \"" + Name + "\". Forgot to include required resources?";
			}

            return new LocalLoadResult(bitmap, error, () =>
            {
                scale.x = 1.0f;
                scale.y = 1.0f;

                // Set the offset if the offset was not given
                if ((offsetx == int.MinValue) || (offsety == int.MinValue))
                {
					offsetx = offsety = 0;
                }
            });
        }

        #endregion
    }
}
