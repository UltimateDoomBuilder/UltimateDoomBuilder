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

namespace CodeImp.DoomBuilder.UDBScript.Wrapper
{
	class VisualCameraWrapper
	{
		/// <summary>
		/// Position of the camera as `Vector3D`. Read-only.
		/// </summary>
		public Vector3DWrapper position
		{
			get
			{
				return new Vector3DWrapper(General.Map.VisualCamera.Position);
			}
		}

		/// <summary>
		/// Angle of the camera on the X/Y axes. Read-only.
		/// </summary>
		public double angleXY
		{
			get
			{
				return General.Map.VisualCamera.AngleXY;
			}
		}

		/// <summary>
		/// Angle of the camera on the Z axis. Read-only.
		/// </summary>
		public double angleZ
		{
			get
			{
				return General.Map.VisualCamera.AngleZ;
			}
		}
	}
}
