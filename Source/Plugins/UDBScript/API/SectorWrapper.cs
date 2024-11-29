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
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using CodeImp.DoomBuilder.BuilderModes;
using CodeImp.DoomBuilder.Editing;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.VisualModes;

#endregion

namespace CodeImp.DoomBuilder.UDBScript.Wrapper
{
	class SectorWrapper : MapElementWrapper, IMoreTags, IEquatable<SectorWrapper>
	{
		#region ================== Variables

		private Sector sector;

		#endregion

		#region IEquatable<SectorWrapper> members

		public bool Equals(SectorWrapper other)
		{
			return sector == other.sector;
		}
		
		public override bool Equals(object obj)
		{
			return Equals((SectorWrapper)obj);
		}
		
		public override int GetHashCode()
		{
			return sector.GetHashCode();
		}

		#endregion

		#region ================== Properties

		internal Sector Sector	{ get { return sector; } }

		/// <summary>
		/// The `Sector`'s index. Read-only.
		/// </summary>
		public int index
		{
			get
			{
				if (sector.IsDisposed)
					throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException("Sector is disposed, the index property can not be accessed.");

				return sector.Index;
			}
		}

		/// <summary>
		/// Floor height of the `Sector`.
		/// </summary>
		public int floorHeight
		{
			get
			{
				if (sector.IsDisposed)
					throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException("Sector is disposed, the floorHeight property can not be accessed.");

				return sector.FloorHeight;
			}
			set
			{
				if (sector.IsDisposed)
					throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException("Sector is disposed, the floorHeight property can not be accessed.");

				sector.FloorHeight = value;
			}
		}

		/// <summary>
		/// Ceiling height of the `Sector`.
		/// </summary>
		public int ceilingHeight
		{
			get
			{
				if (sector.IsDisposed)
					throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException("Sector is disposed, the ceilingHeight property can not be accessed.");

				return sector.CeilHeight;
			}
			set
			{
				if (sector.IsDisposed)
					throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException("Sector is disposed, the ceilingHeight property can not be accessed.");

				sector.CeilHeight = value;
			}
		}

		/// <summary>
		/// Floor texture of the `Sector`.
		/// </summary>
		public string floorTexture
		{
			get
			{
				if (sector.IsDisposed)
					throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException("Sector is disposed, the floorTexture property can not be accessed.");

				return sector.FloorTexture;
			}
			set
			{
				if (sector.IsDisposed)
					throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException("Sector is disposed, the floorTexture property can not be accessed.");

				sector.SetFloorTexture(value);

				// Make sure to update the used textures, so that they are shown immediately
				General.Map.Data.UpdateUsedTextures();
			}
		}

		/// <summary>
		/// Ceiling texture of the `Sector`.
		/// </summary>
		public string ceilingTexture
		{
			get
			{
				if (sector.IsDisposed)
					throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException("Sector is disposed, the ceilingTexture property can not be accessed.");

				return sector.CeilTexture;
			}
			set
			{
				if (sector.IsDisposed)
					throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException("Sector is disposed, the ceilingTexture property can not be accessed.");

				sector.SetCeilTexture(value);

				// Make sure to update the used textures, so that they are shown immediately
				General.Map.Data.UpdateUsedTextures();
			}
		}

		/// <summary>
		/// If the `Sector` is selected or not.
		/// </summary>
		public bool selected
		{
			get
			{
				if (sector.IsDisposed)
					throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException("Sector is disposed, the selected property can not be accessed.");

				return sector.Selected;
			}
			set
			{
				if (sector.IsDisposed)
					throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException("Sector is disposed, the selected property can not be accessed.");

				sector.Selected = value;

				// Make update lines selection
				foreach (Sidedef sd in sector.Sidedefs)
				{
					bool front, back;
					if (sd.Line.Front != null) front = sd.Line.Front.Sector.Selected; else front = false;
					if (sd.Line.Back != null) back = sd.Line.Back.Sector.Selected; else back = false;
					sd.Line.Selected = front | back;
				}
			}
		}

		/// <summary>
		/// If the `Sector`'s floor is selected or not. Will always return `true` in classic modes if the `Sector` is selected. Read-only.
		/// </summary>
		/// <version>3</version>
		public bool floorSelected
		{
			get
			{
				if (sector.IsDisposed)
					throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException("Sector is disposed, the floorSelected property can not be accessed.");

				if (General.Editing.Mode is BaseVisualMode)
				{
					bool f, c;

					((BaseVisualMode)General.Editing.Mode).GetSelectedSurfaceTypesBySector(sector, out f, out c);

					return f;
				}
				else
				{
					return sector.Selected;
				}
			}
		}

		/// <summary>
		/// If the `Sector`'s floor is highlighted or not. Will always return `true` in classic modes if the `Sector` is highlighted. Read-only.
		/// </summary>
		/// <version>3</version>
		public bool floorHighlighted
		{
			get
			{
				if (sector.IsDisposed)
					throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException("Sector is disposed, the floorHighlighted property can not be accessed.");

				if (General.Editing.Mode is BaseVisualMode)
				{
					VisualGeometry vs = (VisualGeometry)((BaseVisualMode)General.Editing.Mode).Highlighted;

					if (vs == null)
						return false;

					return (vs.Sector.Sector == sector && vs.GeometryType == VisualGeometryType.FLOOR);
				}
				else
				{
					Sector s = ((ClassicMode)General.Editing.Mode).HighlightedObject as Sector;

					if(s == null)
						return false;

					return s == sector;
				}
			}
		}

		/// <summary>
		/// If the `Sector`'s ceiling is selected or not. Will always return `true` in classic modes if the `Sector` is selected. Read-only.
		/// </summary>
		/// <version>3</version>
		public bool ceilingSelected
		{
			get
			{
				if (sector.IsDisposed)
					throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException("Sector is disposed, the ceilingSelected property can not be accessed.");

				if (General.Editing.Mode is BaseVisualMode)
				{
					bool f, c;

					((BaseVisualMode)General.Editing.Mode).GetSelectedSurfaceTypesBySector(sector, out f, out c);

					return c;
				}
				else
				{
					return sector.Selected;
				}
			}
		}

		/// <summary>
		/// If the `Sector`'s ceiling is highlighted or not. Will always return `true` in classic modes if the `Sector` is highlighted. Read-only.
		/// </summary>
		/// <version>3</version>
		public bool ceilingHighlighted
		{
			get
			{
				if (sector.IsDisposed)
					throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException("Sector is disposed, the ceilingHighlighted property can not be accessed.");

				if (General.Editing.Mode is BaseVisualMode)
				{
					VisualGeometry vs = (VisualGeometry)((BaseVisualMode)General.Editing.Mode).Highlighted;

					if (vs == null)
						return false;

					return (vs.Sector.Sector == sector && vs.GeometryType == VisualGeometryType.CEILING);
				}
				else
				{
					Sector s = ((ClassicMode)General.Editing.Mode).HighlightedObject as Sector;

					if (s == null)
						return false;

					return s == sector;
				}
			}
		}

		/// <summary>
		/// If the `Sector` is marked or not. It is used to mark map elements that were created or changed (for example after drawing new geometry).
		/// </summary>
		public bool marked
		{
			get
			{
				if (sector.IsDisposed)
					throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException("Sector is disposed, the marked property can not be accessed.");

				return sector.Marked;
			}
			set
			{
				if (sector.IsDisposed)
					throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException("Sector is disposed, the marked property can not be accessed.");

				sector.Marked = value;
			}
		}

		/// <summary>
		/// `Sector` flags. It's an object with the flags as properties. Only available in UDMF.
		///
		/// ```
		/// s.flags['noattack'] = true; // Monsters in this sector don't attack
		/// s.flags.noattack = true; // Also works
		/// ```
		/// </summary>
		public ExpandoObject flags
		{
			get
			{
				if (sector.IsDisposed)
					throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException("Sector is disposed, the flags property can not be accessed.");

				dynamic eo = new ExpandoObject();
				IDictionary<string, object> o = eo as IDictionary<string, object>;

				foreach (KeyValuePair<string, bool> kvp in sector.GetFlags())
				{
					o.Add(kvp.Key, kvp.Value);
				}

				// Create event that gets called when a property is changed. This sets the flag
				((INotifyPropertyChanged)eo).PropertyChanged += new PropertyChangedEventHandler((sender, ea) => {
					PropertyChangedEventArgs pcea = ea as PropertyChangedEventArgs;
					IDictionary<string, object> so = sender as IDictionary<string, object>;

					// Only allow known flags to be set
					if (!General.Map.Config.SectorFlags.Keys.Contains(pcea.PropertyName))
						throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException("Flag name '" + pcea.PropertyName + "' is not valid.");

					// New value must be bool
					if (!(so[pcea.PropertyName] is bool))
						throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException("Flag values must be bool.");

					sector.SetFlag(pcea.PropertyName, (bool)so[pcea.PropertyName]);
				});

				return eo;
			}
		}

		/// <summary>
		/// The `Sector`'s special type. 
		/// </summary>
		public int special
		{
			get
			{
				if (sector.IsDisposed)
					throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException("Sector is disposed, the special property can not be accessed.");

				return sector.Effect;
			}
			set
			{
				if (sector.IsDisposed)
					throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException("Sector is disposed, the special property can not be accessed.");

				sector.Effect = value;
			}
		}

		/// <summary>
		/// The `Sector`'s tag.
		/// </summary>
		public int tag
		{
			get
			{
				if (sector.IsDisposed)
					throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException("Sector is disposed, the tag property can not be accessed.");

				return sector.Tag;
			}
			set
			{
				if (sector.IsDisposed)
					throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException("Sector is disposed, the tag property can not be accessed.");

				sector.Tag = value;
			}
		}

		/// <summary>
		/// The `Sector`'s brightness.
		/// </summary>
		public int brightness
		{
			get
			{
				if (sector.IsDisposed)
					throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException("Sector is disposed, the brightness property can not be accessed.");

				return sector.Brightness;
			}
			set
			{
				if (sector.IsDisposed)
					throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException("Sector is disposed, the brightness property can not be accessed.");

				sector.Brightness = value;
			}
		}

		/// <summary>
		/// The floor's slope offset.
		/// </summary>
		public double floorSlopeOffset
		{
			get
			{
				if (sector.IsDisposed)
					throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException("Sector is disposed, the floorSlopeOffset property can not be accessed.");

				return sector.FloorSlopeOffset;
			}
			set
			{
				sector.FloorSlopeOffset = value;
			}
		}

		/// <summary>
		/// The ceiling's slope offset.
		/// </summary>
		public double ceilingSlopeOffset
		{
			get
			{
				if (sector.IsDisposed)
					throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException("Sector is disposed, the ceilingSlopeOffset property can not be accessed.");

				return sector.CeilSlopeOffset;
			}
			set
			{
				sector.CeilSlopeOffset = value;
			}
		}

		#endregion

		#region ================== Constructors

		internal SectorWrapper(Sector sector) : base(sector)
		{
			this.sector = sector;
		}

		#endregion

		#region ================== Update

		internal override void AfterFieldsUpdate()
		{
			sector.UpdateNeeded = true;
		}

		#endregion

		#region ================== Methods

		public override string ToString()
		{
			return sector.ToString();
		}

		/// <summary>
		/// Returns an `Array` of all `Sidedef`s of the `Sector`.
		/// </summary>
		/// <returns>`Array` of the `Sector`'s `Sidedef`s</returns>
		public SidedefWrapper[] getSidedefs()
		{
			if (sector.IsDisposed)
				throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException("Sector is disposed, the getSidedefs method can not be accessed.");

			List<SidedefWrapper> sidedefs = new List<SidedefWrapper>(sector.Sidedefs.Count);

			foreach (Sidedef sd in sector.Sidedefs)
				if (!sd.IsDisposed)
					sidedefs.Add(new SidedefWrapper(sd));

			return sidedefs.ToArray();
		}

		/// <summary>
		/// Clears all flags.
		/// </summary>
		public void clearFlags()
		{
			if (sector.IsDisposed)
				throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException("Sector is disposed, the cleaFlags method can not be accessed.");

			sector.ClearFlags();
		}

		/// <summary>
		/// Copies the properties from this `Sector` to another.
		/// </summary>
		/// <param name="s">the `Sector` to copy the properties to</param>
		public void copyPropertiesTo(SectorWrapper s)
		{
			if (sector.IsDisposed)
				throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException("Sector is disposed, the cleaFlags method can not be accessed.");

			sector.CopyPropertiesTo(s.sector);
		}

		/// <summary>
		/// Checks if the given point is in this `Sector` or not. The given point can be a `Vector2D` or an `Array` of two numbers.
		/// ```
		/// if(s.intersect(new Vector2D(32, 64)))
		///		UDB.showMessage('Point is in the sector!');
		///		
		/// if(s.intersect([ 32, 64 ]))
		///		UDB.showMessage('Point is in the sector!');
		///	```
		/// </summary>
		/// <param name="p">Point to test</param>
		/// <returns>`true` if the point is in the `Sector`, `false` if it isn't</returns>
		public bool intersect(object p)
		{
			if (sector.IsDisposed)
				throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException("Sector is disposed, the intersect method can not be accessed.");

			try
			{
				Vector2D v = BuilderPlug.Me.GetVector3DFromObject(p);

				return sector.Intersect(v);
			}
			catch (CantConvertToVectorException e)
			{
				throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException(e.Message);
			}

			throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException("Position values must be a Vector2D, or an array of numbers.");
		}

		/// <summary>
		/// Joins this `Sector` with another `Sector`. Lines shared between the sectors will not be removed.
		/// </summary>
		/// <param name="other">Sector to join with</param>
		public void join(SectorWrapper other)
		{
			if (sector.IsDisposed)
				throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException("Sector is disposed, the join method can not be accessed.");

			if(other.sector.IsDisposed)
				throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException("Sector to join with is disposed, the join method can not be used.");

			sector.Join(other.sector);
		}

		/// <summary>
		/// Deletes the `Sector` and its `Sidedef`s.
		/// </summary>
		public void delete()
		{
			if (sector.IsDisposed)
				return;

			// Taken right from SectorsMode.DeleteItem()
			List<Linedef> lines = new List<Linedef>(sector.Sidedefs.Count);
			foreach (Sidedef side in sector.Sidedefs) lines.Add(side.Line);

			General.Map.Map.BeginAddRemove();

			// Dispose the sector
			sector.Dispose();

			// Check all the lines
			for (int i = lines.Count - 1; i >= 0; i--)
			{
				// If the line has become orphaned, remove it
				if ((lines[i].Front == null) && (lines[i].Back == null))
				{
					// Remove line
					lines[i].Dispose();
				}
				else
				{
					// If the line only has a back side left, flip the line and sides
					if ((lines[i].Front == null) && (lines[i].Back != null))
					{
						lines[i].FlipVertices();
						lines[i].FlipSidedefs();
					}

					//mxd. Check textures.
					if (lines[i].Front.MiddleRequired() && lines[i].Front.LongMiddleTexture == MapSet.EmptyLongName)
					{
						if (lines[i].Front.LongHighTexture != MapSet.EmptyLongName)
						{
							lines[i].Front.SetTextureMid(lines[i].Front.HighTexture);
						}
						else if (lines[i].Front.LongLowTexture != MapSet.EmptyLongName)
						{
							lines[i].Front.SetTextureMid(lines[i].Front.LowTexture);
						}
					}

					//mxd. Do we still need high/low textures?
					lines[i].Front.RemoveUnneededTextures(false);

					// Update sided flags
					lines[i].ApplySidedFlags();
				}
			}

			General.Map.Map.EndAddRemove();
		}

		/// <summary>
		/// Gets an array of `Vector2D` arrays, representing the vertices of the triangulated sector. Note that for sectors with islands some triangles may not always have their points on existing vertices.
		/// </summary>
		/// <returns>Array of `Vector2D` arrays</returns>
		public Vector2DWrapper[][] getTriangles()
		{
			if (sector.IsDisposed)
				throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException("Sector is disposed, the getTriangles method can not be accessed.");

			// Updating the cache will also triangulate the sector (if necessary)
			sector.UpdateCache();

			// The triangles are stored in a flat list of Vector2D, so there should always be a multiple of 3 Vector2D
			if(sector.Triangles.Vertices.Count % 3 != 0)
				throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException("Sector triangle vertices is not a multiple of 3.");

			int numtriangles = sector.Triangles.Vertices.Count / 3;

			Vector2DWrapper[][] triangles = new Vector2DWrapper[numtriangles][];

			for(int i=0; i < numtriangles; i++)
			{
				triangles[i] = new Vector2DWrapper[3] {
					new Vector2DWrapper(sector.Triangles.Vertices[i * 3]),
					new Vector2DWrapper(sector.Triangles.Vertices[i * 3 + 1]),
					new Vector2DWrapper(sector.Triangles.Vertices[i * 3 + 2]),
				};
			}

			return triangles;
		}

		/// <summary>
		/// Gets the floor's slope vector.
		/// </summary>
		/// <returns>The floor's slope normal as a `Vector3D`</returns>
		public Vector3DWrapper getFloorSlope()
		{
			if (sector.IsDisposed)
				throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException("Sector is disposed, the getFloorSlope method can not be accessed.");

			return new Vector3DWrapper(sector.FloorSlope.GetNormal());
		}

		/// <summary>
		/// Gets the floor's plane, accounting for flat planes, UDMF slopes and line slopes.
		/// </summary>
		/// <returns>The floor's plane as a `Plane`</returns>
		public PlaneWrapper getFloorPlane()
		{
			if (sector.IsDisposed)
				throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException("Sector is disposed, the getFloorPlane method can not be accessed.");
			Plane plane = Sector.GetFloorPlane(sector);

			return new PlaneWrapper(new Vector3D(plane.a, plane.b, plane.c), plane.d);
		}

		/// <summary>
		/// Sets the floor's slope vector. The vector has to be normalized.
		/// </summary>
		/// <param name="normal">The new slope vector as `Vector3D`</param>
		public void setFloorSlope(object normal)
		{
			if (sector.IsDisposed)
				throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException("Sector is disposed, the setFloorSlope method can not be accessed.");

			try
			{
				sector.FloorSlope = BuilderPlug.Me.GetVector3DFromObject(normal);
			}
			catch (CantConvertToVectorException e)
			{
				throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException(e.Message);
			}
		}

		/// <summary>
		/// Gets the ceiling's slope vector.
		/// </summary>
		/// <returns>The ceiling's slope normal as a `Vector3D`</returns>
		public Vector3DWrapper getCeilingSlope()
		{
			if (sector.IsDisposed)
				throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException("Sector is disposed, the getCeilingSlope method can not be accessed.");

			return new Vector3DWrapper(sector.CeilSlope.GetNormal());
		}

		/// <summary>
		/// Gets the ceiling's plane, accounting for flat planes, UDMF slopes and line slopes.
		/// </summary>
		/// <returns>The ceiling's plane as a `Plane`</returns>
		public PlaneWrapper getCeilingPlane()
		{
			if (sector.IsDisposed)
				throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException("Sector is disposed, the getCeilingPlane method can not be accessed.");
			Plane plane = Sector.GetCeilingPlane(sector);

			return new PlaneWrapper(new Vector3D(plane.a, plane.b, plane.c), plane.d);
		}

		/// <summary>
		/// Sets the ceiling's slope vector. The vector has to be normalized.
		/// </summary>
		/// <param name="normal">The new slope vector as `Vector3D`</param>
		public void setCeilingSlope(object normal)
		{
			if (sector.IsDisposed)
				throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException("Sector is disposed, the setCeilingSlope method can not be accessed.");

			try
			{
				sector.CeilSlope = BuilderPlug.Me.GetVector3DFromObject(normal);
			}
			catch (CantConvertToVectorException e)
			{
				throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException(e.Message);
			}
		}

		/// <summary>
		/// Returns an `Array` of `Vector2D` of label positions for the `Sector`. This are the positions where for example selection number or tags are shown.
		/// 
		/// This example adds an imp to the label position of each sector in the map:
		/// ```
		/// UDB.Map.getSectors().forEach(s => {
		///		const positions = s.getLabelPositions();
		///		if(positions.length > 0)
        ///			UDB.Map.createThing(positions[0], 3001);
		///	});
		///	```
		/// </summary>
		/// <returns>`Array` of `Vector2D` of all label positions</returns>
		/// <version>5</version>
		[UDBScriptSettings(MinVersion = 5)]
		public Vector2DWrapper[] getLabelPositions()
		{
			return Tools.FindLabelPositions(sector).Select(lpi => new Vector2DWrapper(lpi.position)).ToArray();
		}

		#endregion

		#region ================== Interface implementations

		/// <summary>
		/// Returns an `Array` of the `Sector`'s tags. UDMF only. Supported game configurations only.
		/// </summary>
		/// <returns>`Array` of tags</returns>
		public int[] getTags()
		{
			return sector.Tags.ToArray();
		}

		/// <summary>
		/// Adds a tag to the `Sector`. UDMF only. Supported game configurations only.
		/// </summary>
		/// <param name="tag">Tag to add</param>
		/// <returns>`true` when the tag was added, `false` when the tag already exists</returns>
		public bool addTag(int tag)
		{
			if (sector.Tags.Contains(tag))
				return false;

			// We have to take the detour by creating a new list and assigning that because otherwise the
			// BeforePropsChange will not be triggered
			List<int> tags = new List<int>(sector.Tags);
			tags.Add(tag);
			tags.Remove(0);

			sector.Tags = tags;

			return true;
		}

		/// <summary>
		/// Removes a tag from the `Sector`. UDMF only. Supported game configurations only.
		/// </summary>
		/// <param name="tag">Tag to remove</param>
		/// <returns>`true` when the tag was removed successfully, `false` when the tag did not exist</returns>
		public bool removeTag(int tag)
		{
			if(sector.Tags.Contains(tag))
			{
				// If it's the only tag just set it to 0, otherwise remove the tag completely
				if (sector.Tags.Count == 1)
					sector.Tag = 0;
				else
				{
					// We have to take the detour by creating a new list and assigning that because otherwise the
					// BeforePropsChange will not be triggered
					List<int> tags = new List<int>(sector.Tags);
					tags.Remove(tag);
					sector.Tags = tags;
				}

				return true;
			}

			return false;
		}

		#endregion
	}
}
