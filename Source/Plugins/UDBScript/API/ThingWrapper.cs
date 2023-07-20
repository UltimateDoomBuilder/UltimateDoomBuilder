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
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Map;

#endregion

namespace CodeImp.DoomBuilder.UDBScript.Wrapper
{
	internal class ThingWrapper : MapElementWrapper, IEquatable<ThingWrapper>
	{
		#region ================== Variables

		private Thing thing;
		private MapElementArgumentsWrapper elementargs;

		#endregion

		#region IEquatable<ThingWrapper> members

		public bool Equals(ThingWrapper other)
		{
			return thing == other.thing;
		}

		public override bool Equals(object obj)
		{
			return Equals((ThingWrapper)obj);
		}

		public override int GetHashCode()
		{
			return thing.GetHashCode();
		}

		#endregion

		#region ================== Properties

		/// <summary>
		/// Index of the `Thing`. Read-only.
		/// </summary>
		public int index
		{
			get
			{
				if (thing.IsDisposed)
					throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException("Thing is disposed, the index property can not be accessed.");

				return thing.Index;
			}
		}

		/// <summary>
		/// Type of the `Thing`.
		/// </summary>
		public int type
		{
			get
			{
				if (thing.IsDisposed)
					throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException("Thing is disposed, the type property can not be accessed.");

				return thing.Type;
			}
			set
			{
				if (thing.IsDisposed)
					throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException("Thing is disposed, the type property can not be accessed.");

				thing.Type = value;
				thing.UpdateConfiguration();
			}
		}

		/// <summary>
		/// Angle of the `Thing` in degrees, see https://doomwiki.org/wiki/Angle.
		/// </summary>
		public int angle
		{
			get
			{
				if (thing.IsDisposed)
					throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException("Thing is disposed, the angle property can not be accessed.");

				return thing.AngleDoom;
			}
			set
			{
				if (thing.IsDisposed)
					throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException("Thing is disposed, the angle property can not be accessed.");

				thing.Rotate(value);
			}
		}

		/// <summary>
		/// Angle of the `Thing` in radians.
		/// </summary>
		public double angleRad
		{
			get
			{
				if (thing.IsDisposed)
					throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException("Thing is disposed, the angleRad property can not be accessed.");

				return thing.Angle;
			}
			set
			{
				if (thing.IsDisposed)
					throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException("Thing is disposed, the angleRad property can not be accessed.");

				thing.Rotate(value);
			}
		}

		/// <summary>
		/// `Array` of arguments of the `Thing`. Number of arguments depends on game config (usually 5). Hexen format and UDMF only.
		/// </summary>
		/// <fakedtstype>number[]</fakedtstype>
		public MapElementArgumentsWrapper args
		{
			get
			{
				if (thing.IsDisposed)
					throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException("Thing is disposed, the args property can not be accessed.");

				return elementargs;
			}
		}

		/// <summary>
		/// `Thing` action. Hexen and UDMF only.
		/// </summary>
		public int action
		{
			get
			{
				if (thing.IsDisposed)
					throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException("Thing is disposed, the action property can not be accessed.");

				return thing.Action;
			}
			set
			{
				if (thing.IsDisposed)
					throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException("Thing is disposed, the angle property can not be accessed.");

				thing.Action = value;
			}
		}

		/// <summary>
		/// `Thing` tag. UDMF only.
		/// </summary>
		public int tag
		{
			get
			{
				if (thing.IsDisposed)
					throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException("Thing is disposed, the tag property can not be accessed.");

				return thing.Tag;
			}
			set
			{
				if (thing.IsDisposed)
					throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException("Thing is disposed, the tag property can not be accessed.");

				thing.Tag = value;
			}
		}

		/// <summary>
		/// If the `Thing` is selected or not.
		/// </summary>
		public bool selected
		{
			get
			{
				if (thing.IsDisposed)
					throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException("Thing is disposed, the selected property can not be accessed.");

				return thing.Selected;
			}
			set
			{
				if (thing.IsDisposed)
					throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException("Thing is disposed, the selected property can not be accessed.");

				thing.Selected = value;
			}
		}

		/// <summary>
		/// If the `Thing` is marked or not. It is used to mark map elements that were created or changed (for example after drawing new geometry).
		/// </summary>
		public bool marked
		{
			get
			{
				if (thing.IsDisposed)
					throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException("Thing is disposed, the marked property can not be accessed.");

				return thing.Marked;
			}
			set
			{
				if (thing.IsDisposed)
					throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException("Thing is disposed, the marked property can not be accessed.");

				thing.Marked = value;
			}
		}

		/// <summary>
		/// `Thing` flags. It's an object with the flags as properties. In Doom format and Hexen format they are identified by numbers, in UDMF by their name.
		/// Doom and Hexen:
		/// ```
		/// t.flags["8"] = true; // Set the ambush flag
		/// ```
		/// UDMF:
		/// ```
		/// t.flags['ambush'] = true; // Set the ambush flag
		/// t.flags.ambush = true; // Also works
		/// ```
		/// </summary>
		public ExpandoObject flags
		{
			get
			{
				if (thing.IsDisposed)
					throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException("Thing is disposed, the flags property can not be accessed.");

				dynamic eo = new ExpandoObject();
				IDictionary<string, object> o = eo as IDictionary<string, object>;

				foreach (KeyValuePair<string, bool> kvp in thing.GetFlags())
				{
					o.Add(kvp.Key, kvp.Value);
				}

				// Create event that gets called when a property is changed. This sets the flag
				((INotifyPropertyChanged)eo).PropertyChanged += new PropertyChangedEventHandler((sender, ea) => {
					PropertyChangedEventArgs pcea = ea as PropertyChangedEventArgs;
					IDictionary<string, object> so = sender as IDictionary<string, object>;

					// Only allow known flags to be set
					if(!General.Map.Config.ThingFlags.Keys.Contains(pcea.PropertyName))
						throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException("Flag name '" + pcea.PropertyName + "' is not valid.");

					// New value must be bool
					if (!(so[pcea.PropertyName] is bool))
						throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException("Flag values must be bool.");

					thing.SetFlag(pcea.PropertyName, (bool)so[pcea.PropertyName]);
				});

				return eo;
			}
		}

		/// <summary>
		/// Position of the `Thing`. It's an object with `x`, `y`, and `z` properties. The latter is only relevant in Hexen format and UDMF.
		/// The `x`, `y`, and `z` accept numbers:
		/// ```
		/// t.position.x = 32;
		/// t.position.y = 64;
		/// ```
		/// It's also possible to set all fields immediately by assigning either a `Vector2D`, `Vector3D`, or an array of numbers:
		/// ```
		/// t.position = new UDB.Vector2D(32, 64);
		/// t.position = new UDB.Vector3D(32, 64, 128);
		/// t.position = [ 32, 64 ];
		/// t.position = [ 32, 64, 128 ];
		/// ```
		/// </summary>
		public object position
		{
			get
			{
				if (thing.IsDisposed)
					throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException("Vertex is disposed, the position property can not be accessed.");

				return new Vector3DWrapper(thing.Position, thing);
			}
			set
			{
				if (thing.IsDisposed)
					throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException("Vertex is disposed, the position property can not be accessed.");

				try
				{
					Vector3D v = BuilderPlug.Me.GetVector3DFromObject(value);
					thing.Move(v);
				}
				catch (CantConvertToVectorException e)
				{
					throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException(e.Message);
				}
			}
		}

		/// <summary>
		/// Pitch of the `Thing` in degrees. Only valid for supporting game configurations.
		/// </summary>
		public int pitch
		{
			get
			{
				if (thing.IsDisposed)
					throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException("Thing is disposed, the pitch property can not be accessed.");

				return thing.Pitch;
			}
			set
			{
				if (thing.IsDisposed)
					throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException("Thing is disposed, the pitch property can not be accessed.");

				thing.SetPitch(value);
			}
		}

		/// <summary>
		/// Roll of the `Thing` in degrees. Only valid for supporting game configurations.
		/// </summary>
		public int roll
		{
			get
			{
				if (thing.IsDisposed)
					throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException("Thing is disposed, the roll property can not be accessed.");

				return thing.Roll;
			}
			set
			{
				if (thing.IsDisposed)
					throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException("Thing is disposed, the roll property can not be accessed.");

				thing.SetRoll(value);
			}
		}

		#endregion

		#region ================== Constructors

		internal ThingWrapper(Thing thing) : base(thing)
		{
			this.thing = thing;
			elementargs = new MapElementArgumentsWrapper(thing);
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
			// The Thing class doesn't have a proper ToString method, so we have to output something sensible ourself
			return "Thing " + thing.Index.ToString();
		}

		/// <summary>
		/// Copies the properties from this `Thing` to another.
		/// </summary>
		/// <param name="t">The `Thing` to copy the properties to</param>
		public void copyPropertiesTo(ThingWrapper t)
		{
			if (thing.IsDisposed)
				throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException("Thing is disposed, the copyPropertiesTo method can not be accessed.");

			thing.CopyPropertiesTo(t.thing);
		}

		/// <summary>
		/// Clears all flags.
		/// </summary>
		public void clearFlags()
		{
			if (thing.IsDisposed)
				throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException("Thing is disposed, the cleaFlags method can not be accessed.");

			thing.ClearFlags();
		}

		/// <summary>
		/// Snaps the `Thing`'s position to the grid.
		/// </summary>
		public void snapToGrid()
		{
			if (thing.IsDisposed)
				throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException("Thing is disposed, the snapToGrid method can not be accessed.");

			thing.SnapToGrid();
		}

		/// <summary>
		/// Snaps the `Thing`'s position to the map format's accuracy.
		/// </summary>
		public void snapToAccuracy()
		{
			if (thing.IsDisposed)
				throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException("Thing is disposed, the snapToAccuracy method can not be accessed.");

			thing.SnapToAccuracy();
		}

		/// <summary>
		/// Gets the squared distance between this `Thing` and the given point.
		/// The point can be either a `Vector2D` or an array of numbers.
		/// ```
		/// t.distanceToSq(new UDB.Vector2D(32, 64));
		/// t.distanceToSq([ 32, 64 ]);
		/// ```
		/// </summary>
		/// <param name="pos">Point to calculate the squared distance to.</param>
		/// <returns>Distance to `pos`</returns>
		public double distanceToSq(object pos)
		{
			if (thing.IsDisposed)
				throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException("Thing is disposed, the distanceToSq method can not be accessed.");

			try
			{
				Vector2D v = BuilderPlug.Me.GetVector3DFromObject(pos);
				return thing.DistanceToSq(v);
			}
			catch (CantConvertToVectorException e)
			{
				throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException(e.Message);
			}
		}

		/// <summary>
		/// Gets the distance between this `Thing` and the given point. The point can be either a `Vector2D` or an array of numbers.
		/// ```
		/// t.distanceToSq(new UDB.Vector2D(32, 64));
		/// t.distanceToSq([ 32, 64 ]);
		/// ```
		/// </summary>
		/// <param name="pos">Point to calculate the distance to.</param>
		/// <returns>Distance to `pos`</returns>
		public double distanceTo(object pos)
		{
			if (thing.IsDisposed)
				throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException("Thing is disposed, the distanceTo method can not be accessed.");

			try
			{
				Vector2D v = BuilderPlug.Me.GetVector3DFromObject(pos);
				return thing.DistanceTo(v);
			}
			catch (CantConvertToVectorException e)
			{
				throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException(e.Message);
			}
		}

		/// <summary>
		/// Deletes the `Thing`.
		/// </summary>
		public void delete()
		{
			if (!thing.IsDisposed)
			{
				thing.Dispose();
				General.Map.ThingsFilter.Update();
			}
		}

		/// <summary>
		/// Determines and returns the `Sector` the `Thing` is in.
		/// </summary>
		/// <returns>The `Sector` the `Thing` is in</returns>
		public SectorWrapper getSector()
		{
			thing.DetermineSector();
			if (thing.Sector == null)
				return null;
			return new SectorWrapper(thing.Sector);
		}

		#endregion

		#region ================== Management

		/// <summary>
		/// Adds fields to the dictionary that are handled directly by UDB, but changing them is emulated through the UDMF fields.
		/// </summary>
		/// <param name="fields">UniFields of the map element</param>
		internal override void AddManagedFields(IDictionary<string, object> fields)
		{
			if (thing.ScaleX != 1.0)
				fields.Add("scalex", thing.ScaleX);

			if (thing.ScaleY != 1.0)
				fields.Add("scaley", thing.ScaleY);
		}

		/// <summary>
		/// Processed a managed UDMF field, setting the managed value to what the user set in the UDMF field.
		/// </summary>
		/// <param name="fields">UniFields of the map element</param>
		/// <param name="pname">field property name</param>
		/// <param name="newvalue">field value</param>
		/// <returns>true if the field needed to be processed, false if it didn't</returns>
		internal override bool ProcessManagedField(UniFields fields, string pname, object newvalue)
		{
			switch(pname)
			{
				case "scalex":
					if (newvalue == null) thing.SetScale(1.0, thing.ScaleY);
					else thing.SetScale((double)newvalue, thing.ScaleY);
					return true;
				case "scaley":
					if(newvalue == null) thing.SetScale(thing.ScaleX, 1.0);
					else thing.SetScale(thing.ScaleX, (double)newvalue);
					return true;
			}

			return false;
		}

		#endregion
	}
}
