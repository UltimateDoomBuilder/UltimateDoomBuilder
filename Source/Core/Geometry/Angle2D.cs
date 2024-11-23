	
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

#endregion

namespace CodeImp.DoomBuilder.Geometry
{
	public struct Angle2D
	{
		#region ================== Constants

		public const double PI = Math.PI;
		public const double PIHALF = Math.PI * 0.5;
		public const double PI2 = Math.PI * 2;
		public const double PIDEG = 57.295779513082320876798154814105;
		public const double SQRT2 = 1.4142135623730950488016887242097;
		
		#endregion

		#region ================== Methods

		// This converts doom angle to real angle
		public static double DoomToReal(int doomangle)
		{
			return Math.Round(Normalized(DegToRad((doomangle + 90))), 4);
		}

		// This converts real angle to doom angle
		public static int RealToDoom(double realangle)
		{
			return (int)Math.Round(RadToDeg(Normalized(realangle - PIHALF)));
		}

		// This converts degrees to radians
		public static double DegToRad(double deg)
		{
			return deg / PIDEG;
		}

		// This converts radians to degrees
		public static double RadToDeg(double rad)
		{
			return rad * PIDEG;
		}

		// This normalizes an angle
		public static double Normalized(double a)
		{
			while(a < 0f) a += PI2;
			while(a >= PI2) a -= PI2;
			return a;
		}

		// This returns the difference between two angles
		public static double Difference(double a, double b)
		{
			// Calculate delta angle
			double d = Normalized(a) - Normalized(b);

			// Make corrections for zero barrier
			if(d < 0f) d += PI2;
			if(d > PI) d = PI2 - d;

			// Return result
			return d;
		}

		//mxd. Slade 3 MathStuff::angle2DRad ripoff...
		//Returns the angle between the 2d points [p1], [p2] and [p3] in range [0 - 2PI]
		public static double GetAngle(Vector2D p1, Vector2D p2, Vector2D p3)
		{
			Vector2D ab = new Vector2D(p1.x - p2.x, p1.y - p2.y);
			Vector2D cb = new Vector2D(p3.x - p2.x, p3.y - p2.y);

			double dot = (ab.x * cb.x + ab.y * cb.y);
			double cross = (ab.x * cb.y - ab.y * cb.x);
			double result = Math.Atan2(cross,dot);

			/* if angle can be in range [-PI : PI] then returning atan 2 is enough */
			return result < 0 ? result + PI2 : result;
		}
		
		#endregion
	}
}
