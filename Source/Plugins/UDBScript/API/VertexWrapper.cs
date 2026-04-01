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

using System;
using System.Collections.Generic;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Map;

#endregion

namespace CodeImp.DoomBuilder.UDBScript.Wrapper
{
	class VertexWrapper : MapElementWrapper, IEquatable<VertexWrapper>
	{
		#region ================== Variables

		Vertex vertex;

		#endregion

		#region IEquatable<SectorWrapper> members

		public bool Equals(VertexWrapper other)
		{
			return vertex == other.vertex;
		}

		public override bool Equals(object obj)
		{
			return Equals((VertexWrapper)obj);
		}

		public override int GetHashCode()
		{
			return vertex.GetHashCode();
		}

		#endregion

		#region ================== Properties

		internal Vertex Vertex { get { return vertex; } }

		/// <summary>
		/// The vertex index. Read-only.
		/// </summary>
		public int index
		{
			get
			{
				if (vertex.IsDisposed)
					throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException("Vertex is disposed, the index property can not be accessed.");

				return vertex.Index;
			}
		}

		/// <summary>
		/// Position of the `Vertex`. It's an object with `x` and `y` properties. 
		/// The `x` and `y` accept numbers:
		/// ```
		/// v.position.x = 32;
		/// v.position.y = 64;
		/// ```
		/// It's also possible to set all fields immediately by assigning either a `Vector2D`, or an array of numbers:
		/// ```
		/// v.position = new UDB.Vector2D(32, 64);
		/// v.position = [ 32, 64 ];
		/// ```
		/// </summary>
		public object position
		{
			get
			{
				if (vertex.IsDisposed)
					throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException("Vertex is disposed, the position property can not be accessed.");

				return new Vector2DWrapper(vertex.Position, vertex);
			}
			set
			{
				if (vertex.IsDisposed)
					throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException("Vertex is disposed, the position property can not be accessed.");

				try
				{
					Vector2D v = BuilderPlug.Me.GetVector3DFromObject(value);

					vertex.Move(v);

					foreach (Linedef ld in vertex.Linedefs)
						ld.UpdateCache();
				}
				catch (CantConvertToVectorException e)
				{
					throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException(e.Message);
				}
			}
		}

		/// <summary>
		/// If the `Vertex` is selected or not.
		/// </summary>
		public bool selected
		{
			get
			{
				if (vertex.IsDisposed)
					throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException("Vertex is disposed, the selected property can not be accessed.");

				return vertex.Selected;
			}
			set
			{
				if (vertex.IsDisposed)
					throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException("Vertex is disposed, the selected property can not be accessed.");

				vertex.Selected = value;
			}
		}

		/// <summary>
		/// If the `Vertex` is marked or not. It is used to mark map elements that were created or changed (for example after drawing new geometry).
		/// </summary>
		public bool marked
		{
			get
			{
				if (vertex.IsDisposed)
					throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException("Vertex is disposed, the marked property can not be accessed.");

				return vertex.Marked;
			}
			set
			{
				if (vertex.IsDisposed)
					throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException("Vertex is disposed, the marked property can not be accessed.");

				vertex.Marked = value;
			}
		}

		/// <summary>
		/// The ceiling z position of the `Vertex`. Only available in UDMF. Only available for supported game configurations.
		/// </summary>
		public double ceilingZ
		{
			get
			{
				if (vertex.IsDisposed)
					throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException("Vertex is disposed, the ceilingZ property can not be accessed.");

				return vertex.ZCeiling;
			}
			set
			{
				if (vertex.IsDisposed)
					throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException("Vertex is disposed, the ceilingZ property can not be accessed.");

				vertex.ZCeiling = value;
			}
		}

		/// <summary>
		/// The floor z position of the `Vertex`. Only available in UDMF. Only available for supported game configurations.
		/// </summary>
		public double floorZ
		{
			get
			{
				if (vertex.IsDisposed)
					throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException("Vertex is disposed, the floorZ property can not be accessed.");

				return vertex.ZFloor;
			}
			set
			{
				if (vertex.IsDisposed)
					throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException("Vertex is disposed, the floorZ property can not be accessed.");

				vertex.ZFloor = value;
			}
		}

		#endregion

		#region ================== Constructors

		internal VertexWrapper(Vertex vertex) : base(vertex)
		{
			this.vertex = vertex;
		}

		#endregion

		#region ================== Update

		internal override void AfterFieldsUpdate()
		{
		}

		#endregion

		#region ================== Methods

		public override string ToString()
		{
			return vertex.ToString();
		}

		/// <summary>
		/// Gets all `Linedefs` that are connected to this `Vertex`.
		/// </summary>
		/// <returns>Array of linedefs</returns>
		public LinedefWrapper[] getLinedefs()
		{
			if (vertex.IsDisposed)
				throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException("Vertex is disposed, the getLinedefs method can not be accessed.");

			List<LinedefWrapper> linedefs = new List<LinedefWrapper>(vertex.Linedefs.Count);

			foreach (Linedef ld in vertex.Linedefs)
				if (!ld.IsDisposed)
					linedefs.Add(new LinedefWrapper(ld));

			return linedefs.ToArray();
		}

		/// <summary>
		/// Copies the properties from this `Vertex` to another.
		/// </summary>
		/// <param name="v">the vertex to copy the properties to</param>
		public void copyPropertiesTo(VertexWrapper v)
		{
			if (vertex.IsDisposed)
				throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException("Vertex is disposed, the copyPropertiesTo method can not be accessed.");

			vertex.CopyPropertiesTo(v.vertex);
		}

		/// <summary>
		/// Gets the squared distance between this `Vertex` and the given point.
		/// The point can be either a `Vector2D` or an array of numbers.
		/// ```
		/// v.distanceToSq(new UDB.Vector2D(32, 64));
		/// v.distanceToSq([ 32, 64 ]);
		/// ```
		/// </summary>
		/// <param name="pos">Point to calculate the squared distance to.</param>
		/// <returns>Squared distance to `pos`</returns>
		public double distanceToSq(object pos)
		{
			if (vertex.IsDisposed)
				throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException("Vertex is disposed, the distanceToSq method can not be accessed.");

			try
			{
				Vector2D v = BuilderPlug.Me.GetVector3DFromObject(pos);
				return vertex.DistanceToSq(v);
			}
			catch(CantConvertToVectorException e)
			{
				throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException(e.Message);
			}
		}

		/// <summary>
		/// Gets the distance between this `Vertex` and the given point.
		/// The point can be either a `Vector2D` or an array of numbers.
		/// ```
		/// v.distanceTo(new UDB.Vector2D(32, 64));
		/// v.distanceTo([ 32, 64 ]);
		/// ```
		/// </summary>
		/// <param name="pos">Point to calculate the distance to.</param>
		/// <returns>Distance to `pos`</returns>
		public double distanceTo(object pos)
		{
			if (vertex.IsDisposed)
				throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException("Vertex is disposed, the distanceTo method can not be accessed.");

			try
			{
				Vector2D v = BuilderPlug.Me.GetVector3DFromObject(pos);
				return vertex.DistanceTo(v);
			}
			catch (CantConvertToVectorException e)
			{
				throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException(e.Message);
			}
		}

		/// <summary>
		/// Returns the `Linedef` that is connected to this `Vertex` that is closest to the given point.
		/// </summary>
		/// <param name="pos">Point to get the nearest `Linedef` connected to this `Vertex` from</param>
		/// <returns></returns>
		public LinedefWrapper nearestLinedef(object pos)
		{
			if (vertex.IsDisposed)
				throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException("Vertex is disposed, the nearestLinedef method can not be accessed.");

			try
			{
				Vector2D v = BuilderPlug.Me.GetVector3DFromObject(pos);
				return new LinedefWrapper(vertex.NearestLinedef(v));
			}
			catch (CantConvertToVectorException e)
			{
				throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException(e.Message);
			}
		}

		/// <summary>
		/// Snaps the `Vertex`'s position to the map format's accuracy.
		/// </summary>
		public void snapToAccuracy()
		{
			if (vertex.IsDisposed)
				throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException("Vertex is disposed, the snapToAccuracy method can not be accessed.");

			vertex.SnapToAccuracy();
		}

		/// <summary>
		/// Snaps the `Vertex`'s position to the grid.
		/// </summary>
		public void snapToGrid()
		{
			if (vertex.IsDisposed)
				throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException("Vertex is disposed, the snapToGrid method can not be accessed.");

			vertex.SnapToGrid();
		}

		/// <summary>
		/// Joins this `Vertex` with another `Vertex`, deleting this `Vertex` and keeping the other.
		/// </summary>
		/// <param name="other">`Vertex` to join with</param>
		public void join(VertexWrapper other)
		{
			if (vertex.IsDisposed)
				throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException("Vertex is disposed, the join method can not be accessed.");

			vertex.Join(other.vertex);
		}

		/// <summary>
		/// Deletes the `Vertex`. Note that this can result in unclosed sectors.
		/// </summary>
		public void delete()
		{
			if (vertex.IsDisposed)
				return;

			vertex.Dispose();
		}

		#endregion
	}
}
