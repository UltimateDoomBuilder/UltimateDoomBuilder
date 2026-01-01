#region ================== Copyright (c) 2021 Boris Iwanski

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

#region ================== Namespaces

#endregion

namespace CodeImp.DoomBuilder.UDBScript.Wrapper
{
	class DataWrapper
	{
		#region ================== Constructors

		internal DataWrapper() { }

		#endregion

		#region ================== Methods

		/// <summary>
		/// Returns an `Array` of all texture names.
		/// </summary>
		/// <returns>`Array` of all texture names</returns>
		public string[] getTextureNames()
		{
			return General.Map.Data.TextureNames.ToArray();
		}

		/// <summary>
		/// Checks if a texture with the given name exists.
		/// </summary>
		/// <param name="name">Texture name to check</param>
		/// <returns>`true` if the texture exists, `false` if it doesn't</returns>
		public bool textureExists(string name)
		{
			return General.Map.Data.GetTextureExists(name);
		}

		/// <summary>
		/// Returns an `ImageInfo` object for the given texture name.
		/// </summary>
		/// <param name="name">Texture name to get the info for</param>
		/// <returns>`ImageInfo` object containing information about the texture</returns>
		public ImageInfo getTextureInfo(string name)
		{
			return new ImageInfo(General.Map.Data.GetTextureImage(name));
		}

		/// <summary>
		/// Returns an `Array`of all flat names.
		/// </summary>
		/// <returns>`Array` of all flat names</returns>
		public string[] getFlatNames()
		{
			return General.Map.Data.FlatNames.ToArray();
		}

		/// <summary>
		/// Checks if a flat with the given name exists.
		/// </summary>
		/// <param name="name">Flat name to check</param>
		/// <returns>`true` if the flat exists, `false` if it doesn't</returns>
		public bool flatExists(string name)
		{
			return General.Map.Data.GetFlatExists(name);
		}

		/// <summary>
		/// Returns an `ImageInfo` object for the given flat name.
		/// </summary>
		/// <param name="name">Flat name to get the info for</param>
		/// <returns>`ImageInfo` object containing information about the flat</returns>
		public ImageInfo getFlatInfo(string name)
		{
			return new ImageInfo(General.Map.Data.GetFlatImage(name));
		}

		#endregion
	}
}
