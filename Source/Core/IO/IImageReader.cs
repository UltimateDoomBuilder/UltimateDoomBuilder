
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

using System.IO;
using System.Drawing;

#endregion

namespace CodeImp.DoomBuilder.IO
{
	internal unsafe interface IImageReader
	{
		Bitmap ReadAsBitmap(Stream stream, out int offsetx, out int offsety);
	}
}
