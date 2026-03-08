#region ================== Copyright (c) 2025 Boris Iwanski

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

using System.Windows.Forms;

namespace CodeImp.DoomBuilder.Controls
{
	public static class ControlExtensionMethods
	{
		/// <summary>
		/// Sets the text of a label while keeping its right edge in the same position.
		/// </summary>
		/// <param name="label">The label to set the text for.</param>
		/// <param name="text">The new text to set.</param>
		public static void SetLeftExpandText(this Label label, string text)
		{
			int right = label.Right;
			label.Text = text;
			label.Left = right - label.Width;
		}
	}
}
