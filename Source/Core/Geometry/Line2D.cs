
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
using System.Windows.Forms;
using CodeImp.DoomBuilder.Map;

#endregion

namespace CodeImp.DoomBuilder.Geometry
{
	public struct Line2D
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		// Coordinates
		public Vector2D v1;
		public Vector2D v2;
		
		#endregion
		
		#region ================== Constructors
		
		// Constructor
		public Line2D(Vector2D v1, Vector2D v2)
		{
			this.v1 = v1;
			this.v2 = v2;
		}
		
		// Constructor
		public Line2D(Vector2D v1, double x2, double y2)
		{
			this.v1 = v1;
			this.v2 = new Vector2D(x2, y2);
		}

		// Constructor
		public Line2D(double x1, double y1, Vector2D v2)
		{
			this.v1 = new Vector2D(x1, y1);
			this.v2 = v2;
		}
		
		// Constructor
		public Line2D(double x1, double y1, double x2, double y2)
		{
			this.v1 = new Vector2D(x1, y1);
			this.v2 = new Vector2D(x2, y2);
		}

		//mxd. Constructor
		public Line2D(Linedef line)
		{
			this.v1 = line.Start.Position;
			this.v2 = line.End.Position;
		}
		
		#endregion

		#region ================== Statics

		// This calculates the length
		public static double GetLength(double dx, double dy)
		{
			// Calculate and return the length
			return Math.Sqrt(GetLengthSq(dx, dy));
		}

		// This calculates the square of the length
		public static double GetLengthSq(double dx, double dy)
		{
			// Calculate and return the length
			return dx * dx + dy * dy;
		}

		// This calculates the normal of a line
		public static Vector2D GetNormal(double dx, double dy)
		{
			return new Vector2D(dx, dy).GetNormal();
		}

		//mxd. This tests if given lines intersects
		public static bool GetIntersection(Line2D line1, Line2D line2) 
		{
			return GetIntersection(line1.v1, line1.v2, line2.v1.x, line2.v1.y, line2.v2.x, line2.v2.y);
		}

		// This tests if the line intersects with the given line coordinates
		public static bool GetIntersection(Vector2D v1, Vector2D v2, double x3, double y3, double x4, double y4)
		{
			double u_ray, u_line;
			return GetIntersection(v1, v2, x3, y3, x4, y4, out u_ray, out u_line);
		}

		// This tests if the line intersects with the given line coordinates
		public static bool GetIntersection(Vector2D v1, Vector2D v2, double x3, double y3, double x4, double y4, out double u_ray)
		{
			double u_line;
			return GetIntersection(v1, v2, x3, y3, x4, y4, out u_ray, out u_line, true);
		}

		//mxd. This tests if the line intersects with the given line coordinates
		public static bool GetIntersection(Vector2D v1, Vector2D v2, double x3, double y3, double x4, double y4, out double u_ray, bool bounded)
		{
			double u_line;
			return GetIntersection(v1, v2, x3, y3, x4, y4, out u_ray, out u_line, bounded);
		}

		//mxd. Gets intersection point between given lines
		public static Vector2D GetIntersectionPoint(Line2D line1, Line2D line2, bool bounded)
		{
			double u_ray, u_line;
			if(GetIntersection(line1.v1, line1.v2, line2.v1.x, line2.v1.y, line2.v2.x, line2.v2.y, out u_ray, out u_line, bounded))
				return GetCoordinatesAt(line2.v1, line2.v2, u_ray);

			// No dice...
			return new Vector2D(double.NaN, double.NaN);
		}

		// This tests if the line intersects with the given line coordinates
		public static bool GetIntersection(Vector2D v1, Vector2D v2, double x3, double y3, double x4, double y4, out double u_ray, out double u_line)
		{
			return GetIntersection(v1, v2, x3, y3, x4, y4, out u_ray, out u_line, true);
		}

		// This tests if the line intersects with the given line coordinates
		public static bool GetIntersection(Vector2D v1, Vector2D v2, double x3, double y3, double x4, double y4, out double u_ray, out double u_line, bool bounded)
		{
			// Calculate divider
			double div = (y4 - y3) * (v2.x - v1.x) - (x4 - x3) * (v2.y - v1.y);

			// Can this be tested?
			if(div != 0.0)
			{
				// Calculate the intersection distance from the line
				u_line = ((x4 - x3) * (v1.y - y3) - (y4 - y3) * (v1.x - x3)) / div;

				// Calculate the intersection distance from the ray
				u_ray = ((v2.x - v1.x) * (v1.y - y3) - (v2.y - v1.y) * (v1.x - x3)) / div;

				// Return if intersecting
				if(bounded && (u_ray < 0.0 || u_ray > 1.0 || u_line < 0.0 || u_line > 1.0)) return false; //mxd
				return true;
			}

			// Unable to detect intersection
			u_line = double.NaN;
			u_ray = double.NaN;
			return false;
		}

		// This tests on which side of the line the given coordinates are
		// returns < 0 for front (right) side, > 0 for back (left) side and 0 if on the line
		public static double GetSideOfLine(Vector2D v1, Vector2D v2, Vector2D p)
		{
			// Calculate and return side information
			return (p.y - v1.y) * (v2.x - v1.x) - (p.x - v1.x) * (v2.y - v1.y);
		}

		// This returns the shortest distance from given coordinates to line
		public static double GetDistanceToLine(Vector2D v1, Vector2D v2, Vector2D p, bool bounded)
		{
			return Math.Sqrt(GetDistanceToLineSq(v1, v2, p, bounded));
		}

		// This returns the shortest distance from given coordinates to line
		public static double GetDistanceToLineSq(Vector2D v1, Vector2D v2, Vector2D p, bool bounded)
		{
			// Calculate intersection offset
			double u = ((p.x - v1.x) * (v2.x - v1.x) + (p.y - v1.y) * (v2.y - v1.y)) / GetLengthSq(v2.x - v1.x, v2.y - v1.y);

			if(bounded)
			{
				/*
				// Limit intersection offset to the line
				float lbound = 1f / GetLength(v2.x - v1.x, v2.y - v1.y);
				float ubound = 1f - lbound;
				if(u < lbound) u = lbound;
				if(u > ubound) u = ubound;
				*/
				if(u < 0f) u = 0f; else if(u > 1f) u = 1f;
			}

			// Calculate intersection point
			Vector2D i = v1 + u * (v2 - v1);

			// Return distance between intersection and point
			// which is the shortest distance to the line
			double ldx = p.x - i.x;
			double ldy = p.y - i.y;
			return ldx * ldx + ldy * ldy;
		}

		// This returns the offset coordinates on the line nearest to the given coordinates
		public static double GetNearestOnLine(Vector2D v1, Vector2D v2, Vector2D p)
		{
			// Calculate and return intersection offset
			return ((p.x - v1.x) * (v2.x - v1.x) + (p.y - v1.y) * (v2.y - v1.y)) / GetLengthSq(v2.x - v1.x, v2.y - v1.y);
		}

		// This returns the coordinates at a specific position on the line
		public static Vector2D GetCoordinatesAt(Vector2D v1, Vector2D v2, double u)
		{
			// Calculate and return intersection offset
			return new Vector2D(v1.x + u * (v2.x - v1.x), v1.y + u * (v2.y - v1.y));
		}

        private static bool IsEqualFloat(double a, double b)
        {
            return Math.Abs(a - b) < 0.0001f;
        }

        // Some random self-written aglrithm instead of Cohen-Sutherland algorithm which used to hang up randomly
        public static Line2D ClipToRectangle(Line2D line, RectangleF rect, out bool intersects)
		{
            double rateXY = 0f;
            if (line.v2.y != line.v1.y)
            {
                double dx = line.v2.x - line.v1.x;
                double dy = line.v2.y - line.v1.y;
                rateXY = dx / dy;
            }

            double x1 = line.v1.x, y1 = line.v1.y;
            double x2 = line.v2.x, y2 = line.v2.y;

            for (int i = 0; i < 2; i++)
            {
                // check x1,y1
                if (y1 < rect.Top)
                {
                    x1 += (rect.Top - y1) * rateXY;
                    y1 = rect.Top;
                }
                if (x1 < rect.Left)
                {
                    if (rateXY != 0) y1 += (rect.Left - x1) / rateXY;
                    x1 = rect.Left;
                }
                // check x2,y2
                if (y2 < rect.Top)
                {
                    x2 += (rect.Top - y2) * rateXY;
                    y2 = rect.Top;
                }
                if (x2 < rect.Left)
                {
                    if (rateXY != 0) y2 += (rect.Left - x2) / rateXY;
                    x2 = rect.Left;
                }
                // check x1,y1
                if (y1 > rect.Bottom)
                {
                    x1 -= (y1 - rect.Bottom) * rateXY;
                    y1 = rect.Bottom;
                }
                if (x1 > rect.Right)
                {
                    if (rateXY != 0) y1 -= (x1 - rect.Right) / rateXY;
                    x1 = rect.Right;
                }
                // check x2,y2
                if (y2 > rect.Bottom)
                {
                    x2 -= (y2 - rect.Bottom) * rateXY;
                    y2 = rect.Bottom;
                }
                if (x2 > rect.Right)
                {
                    if (rateXY != 0) y2 -= (x2 - rect.Right) / rateXY;
                    x2 = rect.Right;
                }
            }

            if ((IsEqualFloat(x1, x2) && (IsEqualFloat(x1, rect.Left) || IsEqualFloat(x1, rect.Right)) ||
                (IsEqualFloat(y1, y2) && (IsEqualFloat(y1, rect.Bottom) || IsEqualFloat(y1, rect.Top)))))
            {
                intersects = false;
                return new Line2D();
            }

            intersects = true;
            return new Line2D(x1, y1, x2, y2);
        }
		
		#endregion

		#region ================== Methods

		// This returns the perpendicular vector by simply making a normal
		public Vector2D GetPerpendicular()
		{
			Vector2D d = GetDelta();
			return new Vector2D(-d.y, d.x);
		}

		// This calculates the angle
		public double GetAngle()
		{
			// Calculate and return the angle
			Vector2D d = GetDelta();
			return -Math.Atan2(-d.y, d.x) + Angle2D.PIHALF;
		}
		
		public Vector2D GetDelta() { return v2 - v1; }
		
		public double GetLength() { return Line2D.GetLength(v2.x - v1.x, v2.y - v1.y); }
		public double GetLengthSq() { return Line2D.GetLengthSq(v2.x - v1.x, v2.y - v1.y); }

		// Output
		public override string ToString()
		{
			return "(" + v1 + ") - (" + v2 + ")";
		}
		
		public bool GetIntersection(double x3, double y3, double x4, double y4)
		{
			return Line2D.GetIntersection(v1, v2, x3, y3, x4, y4);
		}

		public bool GetIntersection(double x3, double y3, double x4, double y4, out double u_ray)
		{
			return Line2D.GetIntersection(v1, v2, x3, y3, x4, y4, out u_ray, true);
		}

		public bool GetIntersection(double x3, double y3, double x4, double y4, out double u_ray, bool bounded)
		{
			return Line2D.GetIntersection(v1, v2, x3, y3, x4, y4, out u_ray, bounded);
		}

		public bool GetIntersection(double x3, double y3, double x4, double y4, out double u_ray, out double u_line)
		{
			return Line2D.GetIntersection(v1, v2, x3, y3, x4, y4, out u_ray, out u_line);
		}

		public bool GetIntersection(Line2D ray)
		{
			return Line2D.GetIntersection(v1, v2, ray.v1.x, ray.v1.y, ray.v2.x, ray.v2.y);
		}

		public bool GetIntersection(Line2D ray, out double u_ray)
		{
			return Line2D.GetIntersection(v1, v2, ray.v1.x, ray.v1.y, ray.v2.x, ray.v2.y, out u_ray, true);
		}

		public bool GetIntersection(Line2D ray, out double u_ray, bool bounded)
		{
			return Line2D.GetIntersection(v1, v2, ray.v1.x, ray.v1.y, ray.v2.x, ray.v2.y, out u_ray, bounded);
		}

		public bool GetIntersection(Line2D ray, out double u_ray, out double u_line)
		{
			return Line2D.GetIntersection(v1, v2, ray.v1.x, ray.v1.y, ray.v2.x, ray.v2.y, out u_ray, out u_line);
		}

		public double GetSideOfLine(Vector2D p)
		{
			return Line2D.GetSideOfLine(v1, v2, p);
		}

		public double GetDistanceToLine(Vector2D p, bool bounded)
		{
			return Line2D.GetDistanceToLine(v1, v2, p, bounded);
		}

		public double GetDistanceToLineSq(Vector2D p, bool bounded)
		{
			return Line2D.GetDistanceToLineSq(v1, v2, p, bounded);
		}

		public double GetNearestOnLine(Vector2D p)
		{
			return Line2D.GetNearestOnLine(v1, v2, p);
		}

		public Vector2D GetCoordinatesAt(double u)
		{
			return Line2D.GetCoordinatesAt(v1, v2, u);
		}

		public Line2D GetTransformed(double offsetx, double offsety, double scalex, double scaley)
		{
			return new Line2D(v1.GetTransformed(offsetx, offsety, scalex, scaley),
				v2.GetTransformed(offsetx, offsety, scalex, scaley));
		}

		// Inverse Transform
		public Line2D GetInvTransformed(double invoffsetx, double invoffsety, double invscalex, double invscaley)
		{
			return new Line2D(v1.GetInvTransformed(invoffsetx, invoffsety, invscalex, invscaley), 
				v2.GetInvTransformed(invoffsetx, invoffsety, invscalex, invscaley));
		}
		
		#endregion
	}
}
