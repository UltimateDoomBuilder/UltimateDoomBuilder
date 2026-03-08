#region ================== Copyright (c) 2022 Boris Iwanski

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

using CodeImp.DoomBuilder.Geometry;

#endregion

namespace CodeImp.DoomBuilder.UDBScript.Wrapper
{
	class PlaneWrapper
	{
		#region ================== Variables

		private Plane plane;

		#endregion

		#region ================== Properties

		/// <summary>
		/// The plane's normal vector.
		/// </summary>
		[UDBScriptSettings(MinVersion = 5)]
		public Vector3D normal
		{
			get
			{
				return plane.Normal;
			}
		}

		/// <summary>
		/// The distance of the plane along the normal vector.
		/// </summary>
		[UDBScriptSettings(MinVersion = 5)]
		public double offset
		{
			get
			{
				return plane.Offset;
			}
			set
			{
				plane.Offset = value;
			}
		}

		/// <summary>
		/// The `a` value of the plane equation. This is the `x` value of the normal vector.
		/// </summary>
		[UDBScriptSettings(MinVersion = 5)]
		public double a
		{
			get
			{
				return plane.Normal.x;
			}
		}

		/// <summary>
		/// The `b` value of the plane equation. This is the `y` value of the normal vector.
		/// </summary>
		[UDBScriptSettings(MinVersion = 5)]
		public double b
		{
			get
			{
				return plane.Normal.y;
			}
		}

		/// <summary>
		/// The `c` value of the plane equation. This is the `z` value of the normal vector.
		/// </summary>
		[UDBScriptSettings(MinVersion = 5)]
		public double c
		{
			get
			{
				return plane.Normal.z;
			}
		}

		/// <summary>
		/// The `d` value of the plane equation. This is the same as the `offset` value.
		/// </summary>
		[UDBScriptSettings(MinVersion = 5)]
		public double d
		{
			get
			{
				return plane.Offset;
			}
			set
			{
				plane.Offset = value;
			}
		}

		#endregion

		#region ================== Constructors

		/// <summary>
		/// Creates a new `Plane` from a normal and an offset. The normal vector has to be `Vector3D`, `Array`s of 3 numbers, or an object with x, y, and z properties.
		/// ```
		/// let plane1 = new UDB.Plane(new Vector3D(0.0, -0.707, 0.707), 32);
		/// let plane2 = new UDB.Plane([ 0.0, -0.707, 0.707 ], 32);
		/// ```
		/// </summary>
		/// <param name="normal">Normal vector of the plane</param>
		/// <param name="offset">Distance of the plane from the origin</param>
		[UDBScriptSettings(MinVersion = 5)]
		public PlaneWrapper(object normal, double offset)
		{
			plane = new Plane(BuilderPlug.Me.GetVector3DFromObject(normal), offset);
		}

		private object bla()
		{
			return new Vector2D(1, 2);
		}

		/// <summary>
		/// Creates a new `Plane` from 3 points. The points have to be `Vector3D`, `Array`s of 3 numbers, or an object with x, y, and z properties.
		/// ```
		/// let plane1 = new UDB.Plane(new Vector3D(0, 0, 0), new Vector3D(64, 0, 0), new Vector3D(64, 64, 32), true);
		/// let plane2 = new UDB.Plane([ 0, 0, 0 ], [ 64, 0, 0 ], [ 64, 64, 32 ], true);
		/// ```
		/// </summary>
		/// <param name="p1">First point</param>
		/// <param name="p2">Second point</param>
		/// <param name="p3">Thrid point</param>
		/// <param name="up">`true` if plane is pointing up, `false` if pointing down</param>
		[UDBScriptSettings(MinVersion = 5)]
		public PlaneWrapper(object p1, object p2, object p3, bool up)
		{
			//Vector2D a2 = new Vector2D(1, 2);
			Vector3D a3 = (Vector2D)bla();
			try
			{
				Vector3D v1 = BuilderPlug.Me.GetVector3DFromObject(p1);
				Vector3D v2 = BuilderPlug.Me.GetVector3DFromObject(p2);
				Vector3D v3 = BuilderPlug.Me.GetVector3DFromObject(p3);

				plane = new Plane(v1, v2, v3, up);
			}
			catch (CantConvertToVectorException e)
			{
				throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException(e.Message);
			}
		}

		#endregion

		#region ================== Methods

		/// <summary>
		/// Checks if the line between `from` and `to` intersects the plane.
		/// 
		/// It returns an `Array`, where the first element is a `bool` vaue indicating if there is an intersector, and the second element is the position of the intersection on the line between the two points.
		/// 
		/// ```
		/// const plane = new UDB.Plane([ 0, 0, 1 ], 0);
		/// const [intersecting, u] = plane.getIntersection([0, 0, 32], [0, 0, -32]);
		/// UDB.log(`${intersecting} / ${u}`); // Prints "true / 0.5"
		/// ```
		/// </summary>
		/// <param name="from">`Vector3D` of the start of the line</param>
		/// <param name="to">`Vector3D` of the end of the line</param>
		/// <returns></returns>
		[UDBScriptSettings(MinVersion = 5)]
		public object[] getIntersection(object from, object to)
		{
			Vector3D f = BuilderPlug.Me.GetVector3DFromObject(from);
			Vector3D t = BuilderPlug.Me.GetVector3DFromObject(to);

			double u_ray = double.NaN;

			bool r = plane.GetIntersection(f, t, ref u_ray);

			return new object[] { r, u_ray };
		}

		/// <summary>
		/// Computes the distance between the `Plane` and a point. The given point can be a `Vector3D` or an `Array` of three numbers. A result greater than 0 means the point is on the front of the plane, less than 0 means the point is behind the plane.
		/// ```
		/// const plane = new UDB.Plane([ 0, 0, 0 ], [ 32, 0, 0 ], [ 32, 32, 16 ], true);
		/// UDB.log(plane.distance([ 16, 16, 32 ])); // Prints '21.466252583998'
		/// ```
		/// </summary>
		/// <param name="p">Point to compute the distnace to</param>
		/// <returns>Distance between the `Plane` and the point as `number`</returns>
		[UDBScriptSettings(MinVersion = 5)]
		public double distance(object p)
		{
			Vector3D v = BuilderPlug.Me.GetVector3DFromObject(p);

			return plane.Distance(v);
		}

		/// <summary>
		/// Returns the point that's closest to the given point on the `Plane`. The given point can be a `Vector3D` or an `Array` of three numbers.
		/// ```
		/// const plane = new UDB.Plane([ 0, 0, 0 ], [ 32, 0, 0 ], [ 32, 32, 16 ], true);
		/// UDB.log(plane.closestOnPlane([ 16, 16, 32 ])); // Prints '16, 25.6, 12.8'
		/// ```
		/// </summary>
		/// <param name="p">Point to get the closest position from</param>
		/// <returns>Point as `Vector3D` on the plane closest to the given point</returns>
		[UDBScriptSettings(MinVersion = 5)]
		public Vector3DWrapper closestOnPlane(object p)
		{
			Vector3D v = BuilderPlug.Me.GetVector3DFromObject(p);

			return new Vector3DWrapper(plane.ClosestOnPlane(v));
		}

		/// <summary>
		/// Returns the position on the z axis of the plane for the given point. The given point can be a `Vector2D` or an `Array` of two numbers.
		/// ```
		/// const plane = new UDB.Plane([ 0, 0, 0 ], [ 32, 0, 0 ], [ 32, 32, 16 ], true);
		/// UDB.log(plane.getZ([ 16, 16 ])); // Prints '8'
		/// ```
		/// </summary>
		/// <param name="p">Point to get the z position from</param>
		/// <returns></returns>
		[UDBScriptSettings(MinVersion = 5)]
		public double getZ(object p)
		{
			Vector2D v = BuilderPlug.Me.GetVector3DFromObject(p);

			return plane.GetZ(v);
		}

		public override bool Equals(object obj)
		{
			if (!(obj is PlaneWrapper other)) return false;

			return plane.Equals(other.plane);
		}

		public override int GetHashCode()
		{
			return plane.GetHashCode();
		}

		#endregion

		#region ================== Statics

		public static bool operator ==(PlaneWrapper a, PlaneWrapper b) => a.plane == b.plane;

		public static bool operator !=(PlaneWrapper a, PlaneWrapper b) => a.plane != b.plane;

		#endregion
	}
}
