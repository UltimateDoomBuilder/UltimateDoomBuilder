#region ================== Copyright (c) 2020 Boris Iwanski

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

using System;
using System.Collections.Generic;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.VisualModes;
using CodeImp.DoomBuilder.Geometry;

namespace CodeImp.DoomBuilder.BuilderModes
{
	internal class SlopeArcher
	{
		BaseVisualMode mode;
		private List<IVisualEventReceiver> sectors;
		private VisualSidedefSlope handle1;
		private VisualSidedefSlope handle2;
		private double theta;
		private double offsetangle;
		private double scale;
		private int baseheight;
		private double baseheightoffset;
		private double heightoffset;
		private Line2D handleline;
		private double length;

		public double Theta { get { return theta; } set { theta = value; } }
		public double OffsetAngle { get { return offsetangle; } set { offsetangle = value; } }
		public double Scale { get { return scale; } set { scale = value; } }
		public int Baseheight { get { return baseheight; } }
		public double HeightOffset { get { return heightoffset; } set { heightoffset = value; } }

		public SlopeArcher(BaseVisualMode mode, List<IVisualEventReceiver> sectors, VisualSidedefSlope handle1, VisualSidedefSlope handle2, double theta, double offsetangle, double scale)
		{
			this.mode = mode;
			this.sectors = sectors;
			this.handle1 = handle1;
			this.handle2 = handle2;
			this.theta = theta;
			this.offsetangle = offsetangle;
			this.scale = scale;
			heightoffset = 0.0;

			handleline = new Line2D(handle1.GetCenterPoint(), handle2.GetCenterPoint());
			length = handleline.GetLength();

			if (handle1.Level.type == SectorLevelType.Ceiling)
				baseheight = handle1.Level.extrafloor != null ? handle1.Level.sector.FloorHeight : handle1.Level.sector.CeilHeight;
			else
				baseheight = handle1.Level.extrafloor != null ? handle1.Level.sector.CeilHeight : handle1.Level.sector.FloorHeight;

			baseheightoffset = 0.0;
		}

		/// <summary>
		/// Applies the slopes to the sectors.
		/// 
		/// We have:
		/// - theta
		/// - offset angle ("offset")
		/// - horizontal line length ("length")
		///
		/// What we need to compute:
		/// - x coordinate where the line starts in the circle ("left", this is cos(theta + offset angle))
		/// - x coordinate where the line ends in the circle ("middle", this is cos(offset angle))
		///
		/// With this data we can calculate some more required variables:
		/// - radius: length / (middle - left)
		/// - left delimiter: cos(offset + theta) * radius
		/// - right delimiter: cos(rotation) * radius (should be same as left delimiter + length)
		/// - section start, in map units: cos(offset + theta) * radius
		/// - base height offset (where the slope starts)
		///
		/// Then we can simply use pythagoras to compute the y position for an x position on the length
		/// </summary>
		public void ApplySlope()
		{
			double left = Math.Cos(theta + offsetangle);
			double middle = Math.Cos(offsetangle);

			double radius = length / (middle - left);
			double leftdelimiter = Math.Cos(offsetangle + theta);
			double rightdelimiter = Math.Cos(offsetangle);

			double sectionstart = Math.Cos(offsetangle + theta) * radius;

			baseheightoffset = Math.Sqrt(radius * radius - sectionstart * sectionstart) * scale;

			foreach (BaseVisualGeometrySector bvgs in sectors)
			{
				HashSet<Vertex> vertices = new HashSet<Vertex>(bvgs.Sector.Sides.Count * 2);
				double u1 = 1.0;
				double u2 = 0.0;

				foreach (Sidedef sd in bvgs.Sector.Sides.Keys)
				{
					vertices.Add(sd.Line.Start);
					vertices.Add(sd.Line.End);
				}

				// Get the two points that are the furthest apart on the line between the slope handles
				foreach(Vertex v in vertices)
				{
					double intersection = handleline.GetNearestOnLine(v.Position);

					if (intersection < u1)
						u1 = intersection;
					if (intersection > u2)
						u2 = intersection;
				}

				// Compute the x position and the corrosponding height of the coordinates
				double xpos1 = sectionstart + (u1 * length);
				double xpos2 = sectionstart + (u2 * length);
				double height1 = Math.Sqrt(radius * radius - xpos1 * xpos1) * scale;
				double height2 = Math.Sqrt(radius * radius - xpos2 * xpos2) * scale;

				if (double.IsNaN(height1))
					height1 = 0.0;

				if (double.IsNaN(height2))
					height2 = 0.0;

				// Adjust the heights
				height1 = height1 - baseheightoffset + baseheight + heightoffset;
				height2 = height2 - baseheightoffset + baseheight + heightoffset;

				// Get the angle of the slope. We cheat a bit and substitute the y value of the vectors with the height of the points
				double slopeangle = Vector2D.GetAngle(new Vector2D(xpos1, height1), new Vector2D(xpos2, height2));

				// Always let the plane point up, VisualSidedefSlope.ApplySlope will invert it if necessary
				Plane plane = new Plane(new Vector3D(handleline.GetCoordinatesAt(u1), height1), handleline.GetAngle() + Angle2D.PIHALF, slopeangle, true);

				VisualSidedefSlope.ApplySlope(bvgs.Level, plane, mode);

				bvgs.Sector.UpdateSectorGeometry(true);
			}
		}
	}
}
