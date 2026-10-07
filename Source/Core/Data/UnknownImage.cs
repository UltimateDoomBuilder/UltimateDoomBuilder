
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

using System.Drawing;

#endregion

namespace CodeImp.DoomBuilder.Data
{
	public sealed class UnknownImage : ImageData
	{
		#region ================== Variables

		private readonly Bitmap loadbitmap;
		
		#endregion
		
		#region ================== Constructor / Disposer

		// Constructor
		public UnknownImage()
		{
			// Initialize
			this.width = 0;
			this.height = 0;
			this.loadbitmap = new Bitmap(Properties.Resources.UnknownImage);
			SetName("");
			
			LoadImageNow();
		}

		#endregion

		#region ================== Methods
		
		// This 'loads' the image
		protected override LocalLoadResult LocalLoadImage()
		{
            return new LocalLoadResult(new Bitmap(loadbitmap));
        }

        // This returns a preview image
        public override Image GetPreview()
		{
			// To do: do we actually need a copy here?
			return new Bitmap(loadbitmap);
		}

		#endregion
	}
}
