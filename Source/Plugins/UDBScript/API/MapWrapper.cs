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

using System.Collections.Generic;
using System.Linq;
using CodeImp.DoomBuilder.BuilderModes;
using CodeImp.DoomBuilder.Editing;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.VisualModes;

#endregion

namespace CodeImp.DoomBuilder.UDBScript.Wrapper
{
	internal class MapWrapper
	{
		#region ================== Enums

		/// <summary>
		/// How geometry should be merged when geometry is stitched.
		/// ```
		/// UDB.Map.stitchGeometry(UDB.Map.MergeometryMode.MERGE);
		/// ```
		/// </summary>
		/// <enum name="CLASSIC">Merge vertices only</enum>
		/// <enum name="MERGE">Merge vertices and lines</enum>
		/// <enum name="REPLACE">Merge vertices and lines, replacing sector geometry</enum>
		[UDBScriptSettings(MinVersion = 5)]
		public enum MergeGeometryMode
		{
			CLASSIC = Map.MergeGeometryMode.CLASSIC,
			MERGE = Map.MergeGeometryMode.MERGE,
			REPLACE = Map.MergeGeometryMode.REPLACE
		}

		#endregion

		#region ================== Variables

		private MapSet map;
		private VisualCameraWrapper visualcamera;
		private Vector2D mousemappos;
		private object highlightedobject;

		#endregion

		#region ================== Properties

		/// <summary>
		/// `true` if the map is in Doom format, `false` if it isn't. Read-only.
		/// </summary>
		public bool isDoom
		{
			get
			{
				return General.Map.DOOM;
			}
		}

		/// <summary>
		/// `true` if the map is in Hexen format, `false` if it isn't. Read-only.
		/// </summary>
		public bool isHexen
		{
			get
			{
				return General.Map.HEXEN;
			}
		}

		/// <summary>
		/// `true` if the map is in UDMF, `false` if it isn't. Read-only.
		/// </summary>
		public bool isUDMF
		{
			get
			{
				return General.Map.UDMF;
			}
		}

		/// <summary>
		/// The map coordinates of the mouse position as a `Vector2D`. Read-only.
		/// </summary>
		public Vector2D mousePosition
		{
			get
			{
				return mousemappos;
			}
		}

		/// <summary>
		/// `VisualCamera` object with information about the position of the camera in visual mode. Read-only.
		/// </summary>
		public VisualCameraWrapper camera
		{
			get
			{
				return visualcamera;
			}
		}

		#endregion

		#region ================== Constructors

		internal MapWrapper()
		{
			map = General.Map.Map;
			visualcamera = new VisualCameraWrapper();

			// If the main window loses focus before the script is running General.Editing.Mode.HighlightedObject will always be null, so cache it here
			highlightedobject = General.Editing.Mode.HighlightedObject;

			if (General.Editing.Mode is ClassicMode)
				mousemappos = ((ClassicMode)General.Editing.Mode).MouseMapPos;
			else
				mousemappos = ((VisualMode)General.Editing.Mode).GetHitPosition();
		}

		#endregion

		#region ================== Methods

		/// <summary>
		/// Returns the given point snapped to the current grid.
		/// </summary>
		/// <param name="pos">Point that should be snapped to the grid</param>
		/// <returns>Snapped position as `Vector2D`</returns>
		public Vector2DWrapper snappedToGrid(object pos)
		{
			try
			{
				return new Vector2DWrapper(General.Map.Grid.SnappedToGrid(BuilderPlug.Me.GetVector3DFromObject(pos)));
			}
			catch (CantConvertToVectorException e)
			{
				throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException(e.Message);
			}
		}

		/// <summary>
		/// Returns an `Array` of all `Thing`s in the map.
		/// </summary>
		/// <returns>`Array` of `Thing`s</returns>
		public ThingWrapper[] getThings()
		{
			List<ThingWrapper> things = new List<ThingWrapper>(General.Map.Map.Things.Count);

			foreach (Thing t in General.Map.Map.Things)
				if(!t.IsDisposed)
					things.Add(new ThingWrapper(t));

			return things.ToArray();
		}

		/// <summary>
		/// Returns an `Array` of all `Sector`s in the map.
		/// </summary>
		/// <returns>`Array` of `Sector`s</returns>
		public SectorWrapper[] getSectors()
		{
			List<SectorWrapper> sectors = new List<SectorWrapper>(General.Map.Map.Sectors.Count);

			foreach (Sector s in General.Map.Map.Sectors)
				if (!s.IsDisposed)
					sectors.Add(new SectorWrapper(s));

			return sectors.ToArray();
		}

		/// <summary>
		/// Returns an `Array` of all `Sidedef`s in the map.
		/// </summary>
		/// <returns>`Array` of `Sidedef`s</returns>
		public SidedefWrapper[] getSidedefs()
		{
			List<SidedefWrapper> sidedefs = new List<SidedefWrapper>(General.Map.Map.Sidedefs.Count);

			foreach (Sidedef sd in General.Map.Map.Sidedefs)
				if (!sd.IsDisposed)
					sidedefs.Add(new SidedefWrapper(sd));

			return sidedefs.ToArray();
		}

		/// <summary>
		/// Returns an `Array` of all `Linedef`s in the map.
		/// </summary>
		/// <returns>`Array` of `Linedef`s</returns>
		public LinedefWrapper[] getLinedefs()
		{
			List<LinedefWrapper> linedefs = new List<LinedefWrapper>(General.Map.Map.Linedefs.Count);

			foreach (Linedef ld in General.Map.Map.Linedefs)
				if (!ld.IsDisposed)
					linedefs.Add(new LinedefWrapper(ld));

			return linedefs.ToArray();
		}

		/// <summary>
		/// Returns an `Array` of all `Vertex` in the map.
		/// </summary>
		/// <returns>`Array` of `Vertex`</returns>
		public VertexWrapper[] getVertices()
		{
			List<VertexWrapper> vertices = new List<VertexWrapper>(General.Map.Map.Vertices.Count);

			foreach (Vertex v in General.Map.Map.Vertices)
				if (!v.IsDisposed)
					vertices.Add(new VertexWrapper(v));

			return vertices.ToArray();
		}

		/// <summary>
		/// Stitches marked geometry with non-marked geometry.
		/// </summary>
		/// <param name="mergemode">Mode to merge by as `MergeGeometryMode`</param>
		/// <returns>`true` if successful, `false` if failed</returns>
		public bool stitchGeometry(MergeGeometryMode mergemode = MergeGeometryMode.CLASSIC)
		{
			if(mergemode == MergeGeometryMode.CLASSIC)
				return General.Map.Map.StitchGeometry(Map.MergeGeometryMode.CLASSIC);
			else if(mergemode == MergeGeometryMode.MERGE)
				return General.Map.Map.StitchGeometry(Map.MergeGeometryMode.MERGE);
			else if(mergemode == MergeGeometryMode.REPLACE)
				return General.Map.Map.StitchGeometry(Map.MergeGeometryMode.REPLACE);

			throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException("Unknown MergeGeometryMode value");
		}

		/// <summary>
		/// Snaps all vertices and things to the map format accuracy. Call this to ensure the vertices and things are at valid coordinates.
		/// </summary>
		/// <param name="usepreciseposition">`true` if decimal places defined by the map format should be used, `false` if no decimal places should be used</param>
		public void snapAllToAccuracy(bool usepreciseposition = true)
		{
			General.Map.Map.SnapAllToAccuracy(usepreciseposition);
		}

		/// <summary>
		/// Gets a new tag.
		/// </summary>
		/// <param name="usedtags">`Array` of tags to skip</param>
		/// <returns>The new tag</returns>
		public int getNewTag(int[] usedtags = null)
		{
			if (usedtags == null)
				return General.Map.Map.GetNewTag();
			else
				return General.Map.Map.GetNewTag(usedtags.ToList());
		}

		/// <summary>
		/// Gets multiple new tags.
		/// </summary>
		/// <param name="count">Number of tags to get</param>
		/// <returns>`Array` of the new tags</returns>
		public int[] getMultipleNewTags(int count)
		{
			return General.Map.Map.GetMultipleNewTags(count).ToArray();
		}

		/// <summary>
		/// Gets the `Linedef` that's nearest to the specified position.
		/// </summary>
		/// <param name="pos">Position to check against</param>
		/// <param name="maxrange">Maximum range (optional)</param>
		/// <returns>Nearest `Linedef`</returns>
		public LinedefWrapper nearestLinedef(object pos, double maxrange = double.NaN)
		{
			try
			{
				Vector2D v = BuilderPlug.Me.GetVector3DFromObject(pos);
				Linedef nearest = null;

				if (double.IsNaN(maxrange))
					nearest = General.Map.Map.NearestLinedef(v);
				else
					nearest = General.Map.Map.NearestLinedefRange(v, maxrange);

				if (nearest == null)
					return null;

				return new LinedefWrapper(nearest);
			}
			catch (CantConvertToVectorException e)
			{
				throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException(e.Message);
			}
		}

		/// <summary>
		/// Gets the `Thing` that's nearest to the specified position.
		/// </summary>
		/// <param name="pos">Position to check against</param>
		/// <param name="maxrange">Maximum range (optional)</param>
		/// <returns>Nearest `Linedef`</returns>
		public ThingWrapper nearestThing(object pos, double maxrange = double.NaN)
		{
			try
			{
				Vector2D v = BuilderPlug.Me.GetVector3DFromObject(pos);
				Thing nearest = null;

				if (double.IsNaN(maxrange))
					nearest = General.Map.Map.NearestThingSquareRange(v, double.MaxValue);
				else
					nearest = General.Map.Map.NearestThingSquareRange(v, maxrange);

				if (nearest == null)
					return null;

				return new ThingWrapper(nearest);
			}
			catch (CantConvertToVectorException e)
			{
				throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException(e.Message);
			}
		}

		/// <summary>
		/// Gets the `Vertex` that's nearest to the specified position.
		/// </summary>
		/// <param name="pos">Position to check against</param>
		/// <param name="maxrange">Maximum range (optional)</param>
		/// <returns>Nearest `Vertex`</returns>
		public VertexWrapper nearestVertex(object pos, double maxrange = double.NaN)
		{
			try
			{
				Vector2D v = BuilderPlug.Me.GetVector3DFromObject(pos);
				Vertex nearest = null;

				if (double.IsNaN(maxrange))
					nearest = General.Map.Map.NearestVertexSquareRange(v, double.MaxValue);
				else
					nearest = General.Map.Map.NearestVertexSquareRange(v, maxrange);

				if (nearest == null)
					return null;

				return new VertexWrapper(nearest);
			}
			catch (CantConvertToVectorException e)
			{
				throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException(e.Message);
			}
		}

		/// <summary>
		/// Gets the `Sidedef` that's nearest to the specified position.
		/// </summary>
		/// <param name="pos">Position to check against</param>
		/// <param name="maxrange">Maximum range (optional)</param>
		/// <returns>Nearest `Sidedef`</returns>
		public SidedefWrapper nearestSidedef(object pos)
		{
			try
			{
				Vector2D v = BuilderPlug.Me.GetVector3DFromObject(pos);
				Sidedef nearest = MapSet.NearestSidedef(General.Map.Map.Sidedefs, v);

				if (nearest == null)
					return null;

				return new SidedefWrapper(nearest);
			}
			catch (CantConvertToVectorException e)
			{
				throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException(e.Message);
			}
		}

		/// <summary>
		/// Draws lines. Data has to be an `Array` of `Array` of numbers, `Vector2D`s, `Vector3D`s, or objects with x and y properties. Note that the first and last element have to be at the same positions to make a complete drawing.
		/// ```js
		/// UDB.Map.drawLines([
		///		new UDB.Vector2D(64, 0),
		///		new UDB.Vector2D(128, 0),
		///		new UDB.Vector2D(128, 64),
		///		new UDB.Vector2D(64, 64),
		///		new UDB.Vector2D(64, 0)
		///	]);
		///	
		/// UDB.Map.drawLines([
		///		[ 0, 0 ],
		///		[ 64, 0 ],
		///		[ 64, 64 ],
		///		[ 0, 64 ],
		///		[ 0, 0 ]
		/// ]);
		/// ```
		/// </summary>
		/// <param name="data">`Array` of positions</param>
		/// <returns>`true` if drawing was successful, `false` if it wasn't</returns>
		public bool drawLines(object data)
		{
			if (!data.GetType().IsArray)
				throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException("Data must be supplied as an array");

			List<DrawnVertex> vertices = new List<DrawnVertex>();

			foreach(object item in (object[])data)
			{
				try
				{
					Vector2D v = BuilderPlug.Me.GetVector3DFromObject(item);
					DrawnVertex dv = new DrawnVertex();
					dv.pos = v;
					dv.stitch = dv.stitchline = true;
					vertices.Add(dv);
				}
				catch (CantConvertToVectorException e)
				{
					throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException(e.Message);
				}
			}

			if(vertices.Count < 2)
				throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException("Array must have at least 2 values");

			bool success = Tools.DrawLines(vertices);

			// Snap to map format accuracy
			General.Map.Map.SnapAllToAccuracy();

			// Update map. This has to run on the UI thread
			BuilderPlug.Me.ScriptRunnerForm.RunAction(() => General.Map.Map.Update());

			// Update textures
			General.Map.Data.UpdateUsedTextures();

			return success;
		}

		#endregion

		#region ================== Marks

		/// <summary>
		/// Sets the `marked` property of all map elements. Can be passed `true` to mark all map elements.
		/// </summary>
		/// <param name="mark">`false` to set the `marked` property to `false` (default), `true` to set the `marked` property to `true`</param>
		public void clearAllMarks(bool mark=false)
		{
			General.Map.Map.ClearAllMarks(mark);
		}

		/// <summary>
		/// Sets the `marked` property of all vertices. Can be passed `true` to mark all vertices.
		/// </summary>
		/// <param name="mark">`false` to set the `marked` property to `false` (default), `true` to set the `marked` property to `true`</param>
		public void clearMarkedVertices(bool mark=false)
		{
			General.Map.Map.ClearMarkedVertices(mark);
		}

		/// <summary>
		/// Sets the `marked` property of all `Thing`s. Can be passed `true` to mark all `Thing`s.
		/// </summary>
		/// <param name="mark">`false` to set the `marked` property to `false` (default), `true` to set the `marked` property to `true`</param>
		public void clearMarkedThings(bool mark=false)
		{
			General.Map.Map.ClearMarkedThings(mark);
		}

		/// <summary>
		/// Sets the `marked` property of all `Linedef`s. Can be passed `true` to mark all `Linedef`s.
		/// </summary>
		/// <param name="mark">`false` to set the `marked` property to `false` (default), `true` to set the `marked` property to `true`</param>
		public void clearMarkeLinedefs(bool mark=false)
		{
			General.Map.Map.ClearMarkedLinedefs(mark);
		}

		/// <summary>
		/// Sets the `marked` property of all `Sidedef`s. Can be passed `true` to mark all `Sidedef`s.
		/// </summary>
		/// <param name="mark">`false` to set the `marked` property to `false` (default), `true` to set the `marked` property to `true`</param>
		public void clearMarkeSidedefs(bool mark = false)
		{
			General.Map.Map.ClearMarkedSidedefs(mark);
		}

		/// <summary>
		/// Sets the `marked` property of all `Sector`s. Can be passed `true` to mark all `Sector`s.
		/// </summary>
		/// <param name="mark">`false` to set the `marked` property to `false` (default), `true` to set the `marked` property to `true`</param>
		public void clearMarkeSectors(bool mark = false)
		{
			General.Map.Map.ClearMarkedSectors(mark);
		}

		/// <summary>
		/// Inverts all marks of all map elements.
		/// </summary>
		public void invertAllMarks()
		{
			General.Map.Map.InvertAllMarks();
		}

		/// <summary>
		/// Inverts the `marked` property of all vertices.
		/// </summary>
		public void invertMarkedVertices()
		{
			General.Map.Map.InvertMarkedVertices();
		}

		/// <summary>
		/// Inverts the `marked` property of all `Thing`s.
		/// </summary>
		public void invertMarkedThings()
		{
			General.Map.Map.InvertMarkedThings();
		}

		/// <summary>
		/// Inverts the `marked` property of all `Linedef`s.
		/// </summary>
		public void invertMarkedLinedefs()
		{
			General.Map.Map.InvertMarkedLinedefs();
		}

		/// <summary>
		/// Inverts the `marked` property of all `Sidedef`s.
		/// </summary>
		public void invertMarkedSidedefs()
		{
			General.Map.Map.InvertMarkedSidedefs();
		}

		/// <summary>
		/// Inverts the `marked` property of all `Sector`s.
		/// </summary>
		public void invertMarkedSectors()
		{
			General.Map.Map.InvertMarkedSectors();
		}

		/// <summary>
		/// Gets all marked (default) or unmarked vertices.
		/// </summary>
		/// <param name="mark">`true` to get all marked vertices (default), `false` to get all unmarked vertices</param>
		/// <returns></returns>
		public VertexWrapper[] getMarkedVertices(bool mark=true)
		{
			List<VertexWrapper> vertices = new List<VertexWrapper>();

			foreach (Vertex v in General.Map.Map.Vertices)
				if(v.Marked == mark)
					vertices.Add(new VertexWrapper(v));

			return vertices.ToArray();
		}

		/// <summary>
		/// Gets all marked (default) or unmarked `Thing`s.
		/// </summary>
		/// <param name="mark">`true` to get all marked `Thing`s (default), `false` to get all unmarked `Thing`s</param>
		/// <returns></returns>
		public ThingWrapper[] getMarkedThings(bool mark = true)
		{
			List<ThingWrapper> things = new List<ThingWrapper>();

			foreach (Thing t in General.Map.Map.Things)
				if (t.Marked == mark)
					things.Add(new ThingWrapper(t));

			return things.ToArray();
		}

		/// <summary>
		/// Gets all marked (default) or unmarked `Linedef`s.
		/// </summary>
		/// <param name="mark">`true` to get all marked `Linedef`s (default), `false` to get all unmarked `Linedef`s</param>
		/// <returns></returns>
		public LinedefWrapper[] getMarkedLinedefs(bool mark = true)
		{
			List<LinedefWrapper> linedefs = new List<LinedefWrapper>();

			foreach (Linedef ld in General.Map.Map.Linedefs)
				if (ld.Marked == mark)
					linedefs.Add(new LinedefWrapper(ld));

			return linedefs.ToArray();
		}

		/// <summary>
		/// Gets all marked (default) or unmarked `Sidedef`s.
		/// </summary>
		/// <param name="mark">`true` to get all marked `Sidedef`s (default), `false` to get all unmarked `Sidedef`s</param>
		/// <returns></returns>
		public SidedefWrapper[] getMarkedSidedefs(bool mark = true)
		{
			List<SidedefWrapper> sidedefs = new List<SidedefWrapper>();

			foreach (Sidedef sd in General.Map.Map.Sidedefs)
				if (sd.Marked == mark)
					sidedefs.Add(new SidedefWrapper(sd));

			return sidedefs.ToArray();
		}

		/// <summary>
		/// Gets all marked (default) or unmarked `Sector`s.
		/// </summary>
		/// <param name="mark">`true` to get all marked `Sector`s (default), `false` to get all unmarked `Sector`s</param>
		/// <returns></returns>
		public SectorWrapper[] getMarkedSectors(bool mark = true)
		{
			List<SectorWrapper> sectors = new List<SectorWrapper>();

			foreach (Sector s in General.Map.Map.Sectors)
				if (s.Marked == mark)
					sectors.Add(new SectorWrapper(s));

			return sectors.ToArray();
		}

		/// <summary>
		/// Marks (default) or unmarks all selected vertices.
		/// </summary>
		/// <param name="mark">`true` to mark all selected vertices (default), `false` to unmark</param>
		public void markSelectedVertices(bool mark=true)
		{
			General.Map.Map.MarkSelectedVertices(true, mark);
		}

		/// <summary>
		/// Marks (default) or unmarks all selected `Linedef`s.
		/// </summary>
		/// <param name="mark">`true` to mark all selected `Linedef`s (default), `false` to unmark</param>
		public void markSelectedLinedefs(bool mark = true)
		{
			General.Map.Map.MarkSelectedLinedefs(true, mark);
		}

		/// <summary>
		/// Marks (default) or unmarks all selected `Sector`s.
		/// </summary>
		/// <param name="mark">`true` to mark all selected `Sector`s (default), `false` to unmark</param>
		public void markSelectedSectors(bool mark = true)
		{
			General.Map.Map.MarkSelectedSectors(true, mark);
		}

		/// <summary>
		/// Marks (default) or unmarks all selected `Thing`s.
		/// </summary>
		/// <param name="mark">`true` to mark all selected `Thing`s (default), `false` to unmark</param>
		public void markSelectedThings(bool mark = true)
		{
			General.Map.Map.MarkSelectedThings(true, mark);
		}

		#endregion

		#region ================== Selected

		/// <summary>
		/// Gets all selected (default) or unselected vertices.
		/// </summary>
		/// <param name="selected">`true` to get all selected vertices, `false` to get all unselected ones</param>
		/// <returns>`Array` of `Vertex`</returns>
		public VertexWrapper[] getSelectedVertices(bool selected=true)
		{
			List<VertexWrapper> vertices = new List<VertexWrapper>();

			if (General.Editing.Mode is BaseVisualMode)
			{
				List<Vertex> selectedvertices = ((BaseVisualMode)General.Editing.Mode).GetSelectedVertices();

				foreach (Vertex v in General.Map.Map.Vertices)
					if (selectedvertices.Contains(v) == selected)
						vertices.Add(new VertexWrapper(v));
			}
			else
			{
				foreach (Vertex v in map.GetSelectedVertices(selected))
					if (v.Selected == selected)
						vertices.Add(new VertexWrapper(v));
			}

			return vertices.ToArray();
		}

		/// <summary>
		/// Get the currently highlighted `Vertex`.
		/// </summary>
		/// <returns>The currently highlighted `Vertex` or `null` if no `Vertex` is highlighted</returns>
		public VertexWrapper getHighlightedVertex()
		{
			Vertex v = highlightedobject as Vertex;

			if (v != null)
				return new VertexWrapper(v);

			return null;
		}

		/// <summary>
		/// Gets the currently selected `Vertex`s *or*, if no `Vertex`s are selected, a currently highlighted `Vertex`.
		/// </summary>
		/// <returns>`Array` of `Vertex`</returns>
		public VertexWrapper[] getSelectedOrHighlightedVertices()
		{
			if (General.Map.Map.SelectedVerticessCount > 0)
			{
				List<VertexWrapper> vertices = new List<VertexWrapper>();

				foreach (Vertex v in General.Map.Map.Vertices)
					if (v.Selected)
						vertices.Add(new VertexWrapper(v));

				return vertices.ToArray();
			}
			else
			{
				Vertex v = General.Editing.Mode.HighlightedObject as Vertex;

				if (v != null)
					return new VertexWrapper[] { new VertexWrapper(v) };

			}

			return new VertexWrapper[] { };
		}

		/// <summary>
		/// Gets all selected (default) or unselected `Thing`s.
		/// </summary>
		/// <param name="selected">`true` to get all selected `Thing`s, `false` to get all unselected ones</param>
		/// <returns>`Array` of `Thing`s</returns>
		public ThingWrapper[] getSelectedThings(bool selected = true)
		{
			List<ThingWrapper> things = new List<ThingWrapper>();

			if (General.Editing.Mode is BaseVisualMode)
			{
				List<Thing> selectedthings = ((BaseVisualMode)General.Editing.Mode).GetSelectedThings();

				foreach (Thing t in General.Map.Map.Things)
					if (selectedthings.Contains(t) == selected)
						things.Add(new ThingWrapper(t));
			}
			else
			{
				foreach (Thing t in map.GetSelectedThings(selected))
					if (t.Selected == selected)
						things.Add(new ThingWrapper(t));
			}

			return things.ToArray();
		}

		/// <summary>
		/// Get the currently highlighted `Thing`.
		/// </summary>
		/// <returns>The currently highlighted `Thing` or `null` if no `Thing` is highlighted</returns>
		public ThingWrapper getHighlightedThing()
		{
			if (General.Editing.Mode is BaseVisualMode)
			{
				VisualThing t = highlightedobject as VisualThing;

				if (t != null)
					return new ThingWrapper(t.Thing);
			}
			else
			{
				Thing t = highlightedobject as Thing;

				if (t != null)
					return new ThingWrapper(t);
			}

			return null;
		}

		/// <summary>
		/// Gets the currently selected `Thing`s *or*, if no `Thing`s are selected, a currently highlighted `Thing`.
		/// </summary>
		/// <returns>`Array` of `Thing`s</returns>
		public ThingWrapper[] getSelectedOrHighlightedThings()
		{
			ThingWrapper[] things = getSelectedThings(true);
			if (things.Length > 0)
				return things;

			ThingWrapper highlight = getHighlightedThing();
			if (highlight != null)
				return new ThingWrapper[] { highlight };

			return new ThingWrapper[] { };
		}

		/// <summary>
		/// Gets all selected (default) or unselected `Sector`s.
		/// </summary>
		/// <param name="selected">`true` to get all selected `Sector`s, `false` to get all unselected ones</param>
		/// <returns>`Array` of `Sector`s</returns>
		public SectorWrapper[] getSelectedSectors(bool selected = true)
		{
			List<SectorWrapper> sectors = new List<SectorWrapper>();

			if (General.Editing.Mode is BaseVisualMode)
			{
				List<Sector> selectedsectors = ((BaseVisualMode)General.Editing.Mode).GetSelectedSectors();

				foreach (Sector s in General.Map.Map.Sectors)
					if (selectedsectors.Contains(s) == selected)
						sectors.Add(new SectorWrapper(s));
			}
			else
			{
				foreach (Sector s in map.GetSelectedSectors(selected))
					if (s.Selected == selected)
						sectors.Add(new SectorWrapper(s));
			}

			return sectors.ToArray();
		}

		/// <summary>
		/// Get the currently highlighted `Sector`.
		/// </summary>
		/// <returns>The currently highlighted `Sector` or `null` if no `Sector` is highlighted</returns>
		public SectorWrapper getHighlightedSector()
		{
			if (General.Editing.Mode is BaseVisualMode)
			{
				VisualSector s = highlightedobject as VisualSector;

				if (s != null)
					return new SectorWrapper(s.Sector);
			}
			else
			{
				Sector s = highlightedobject as Sector;

				if (s != null)
					return new SectorWrapper(s);
			}

			return null;
		}

		/// <summary>
		/// Gets the currently selected `Sector`s *or*, if no `Sector`s are selected, a currently highlighted `Sector`.
		/// </summary>
		/// <returns>`Array` of `Sector`s</returns>
		public SectorWrapper[] getSelectedOrHighlightedSectors()
		{
			SectorWrapper[] sectors = getSelectedSectors(true);
			if (sectors.Length > 0)
				return sectors;

			SectorWrapper highlight = getHighlightedSector();
			if (highlight != null)
				return new SectorWrapper[] { highlight };

			return new SectorWrapper[] { };
		}

		/// <summary>
		/// Gets all selected (default) or unselected `Linedef`s.
		/// </summary>
		/// <param name="selected">`true` to get all selected `Linedef`s, `false` to get all unselected ones</param>
		/// <returns>`Array` of `Linedef`s</returns>
		public LinedefWrapper[] getSelectedLinedefs(bool selected = true)
		{
			List<LinedefWrapper> linedefs = new List<LinedefWrapper>();

			if (General.Editing.Mode is BaseVisualMode)
			{
				List<Linedef> selectedlinedefs = ((BaseVisualMode)General.Editing.Mode).GetSelectedLinedefs();

				foreach (Linedef ld in General.Map.Map.Linedefs)
					if(selectedlinedefs.Contains(ld) == selected)
						linedefs.Add(new LinedefWrapper(ld));
			}
			else
			{
				foreach (Linedef ld in map.GetSelectedLinedefs(selected))
					if (ld.Selected == selected)
						linedefs.Add(new LinedefWrapper(ld));
			}

			return linedefs.ToArray();
		}

		/// <summary>
		/// Get the currently highlighted `Linedef`.
		/// </summary>
		/// <returns>The currently highlighted `Linedef` or `null` if no `Linedef` is highlighted</returns>
		public LinedefWrapper getHighlightedLinedef()
		{
			if (General.Editing.Mode is BaseVisualMode)
			{
				Sidedef sd = highlightedobject as Sidedef;

				if (sd != null)
					return new LinedefWrapper(sd.Line);
			}
			else
			{
				Linedef ld = highlightedobject as Linedef;

				if (ld != null)
					return new LinedefWrapper(ld);
			}

			return null;
		}

		/// <summary>
		/// Gets the currently selected `Linedef`s *or*, if no `Linede`f`s are selected, a currently highlighted `Linedef`.
		/// </summary>
		/// <returns>`Array` of `Linedef`s</returns>
		public LinedefWrapper[] getSelectedOrHighlightedLinedefs()
		{
			LinedefWrapper[] linedefs = getSelectedLinedefs(true);
			if (linedefs.Length > 0)
				return linedefs;

			LinedefWrapper highlight = getHighlightedLinedef();
			if (highlight != null)
				return new LinedefWrapper[] { highlight };

			return new LinedefWrapper[] { };
		}

		/*
		public LinedefWrapper[] getAnyLinedefs()
		{
			if (General.Map.Map.SelectedLinedefsCount > 0)
			{
				List<LinedefWrapper> linedefs = new List<LinedefWrapper>();

				foreach (Linedef ld in General.Map.Map.Linedefs)
					if (ld.Selected)
						linedefs.Add(new LinedefWrapper(ld));

				return linedefs.ToArray();
			}
			else if (General.Editing.Mode.HighlightedObject != null && General.Editing.Mode.HighlightedObject is Linedef)
			{
				return new LinedefWrapper[] { new LinedefWrapper((Linedef)General.Editing.Mode.HighlightedObject) };
			}
			else
			{
				List<LinedefWrapper> linedefs = new List<LinedefWrapper>();

				foreach (Linedef ld in General.Map.Map.Linedefs)
					if (ld.Selected)
						linedefs.Add(new LinedefWrapper(ld));

				return linedefs.ToArray();
			}
		}
		*/

		/// <summary>
		/// Gets all `Sidedef`s from the selected `Linedef`s.
		/// In classic modes this will return both sidedefs of 2-sided lines, in visual mode it will only return the actually selected `Sidedef`.
		/// </summary>
		/// <param name="selected">`true` to get all `Sidedef`s of all selected `Linedef`s, `false` to get all `Sidedef`s of all unselected `Linedef`s</param>
		/// <returns>`Array` of `Sidedef`</returns>
		public SidedefWrapper[] getSidedefsFromSelectedLinedefs(bool selected = true)
		{
			List<SidedefWrapper> sidedefs = new List<SidedefWrapper>();

			if (General.Editing.Mode is BaseVisualMode)
			{
				List<Sidedef> selectedsidedefs = ((BaseVisualMode)General.Editing.Mode).GetSelectedSidedefs();

				foreach (Sidedef sd in General.Map.Map.Sidedefs)
					if (selectedsidedefs.Contains(sd) == selected)
						sidedefs.Add(new SidedefWrapper(sd));
			}
			else
			{
				foreach (Sidedef sd in General.Map.Map.GetSidedefsFromSelectedLinedefs(selected))
					sidedefs.Add(new SidedefWrapper(sd));
			}

			return sidedefs.ToArray();
		}

		/// <summary>
		/// Gets the `Sidedef`s of the currently selected `Linedef`s *or*, if no `Linedef`s are selected, the `Sidedef`s of the currently highlighted `Linedef`.
		/// In classic modes this will return both sidedefs of 2-sided lines, in visual mode it will only return the actually selected `Sidedef`.
		/// </summary>
		/// <returns>`Array` of `Sidedef`s</returns>
		/// <version>3</version>
		public SidedefWrapper[] getSidedefsFromSelectedOrHighlightedLinedefs()
		{
			List<SidedefWrapper> sidedefs = new List<SidedefWrapper>(getSidedefsFromSelectedLinedefs(true));

			if(sidedefs.Count > 0)
				return sidedefs.ToArray();

			// Nothing selected, so let's see if anything is highlighted
			LinedefWrapper highlight = getHighlightedLinedef();
			if (highlight != null)
			{
				if (highlight.front != null)
					sidedefs.Add(highlight.front);
				if (highlight.back != null)
					sidedefs.Add(highlight.back);
			}

			return sidedefs.ToArray();
		}

		/// <summary>
		/// Clears all selected map elements.
		/// </summary>
		public void clearAllSelected()
		{
			General.Map.Map.ClearAllSelected();
		}

		/// <summary>
		/// Clears all selected vertices.
		/// </summary>
		public void clearSelectedVertices()
		{
			General.Map.Map.ClearSelectedVertices();
		}

		/// <summary>
		/// Clears all selected `Thing`s.
		/// </summary>
		public void clearSelectedThings()
		{
			General.Map.Map.ClearSelectedThings();
		}

		/// <summary>
		/// Clears all selected `Sector`s.
		/// </summary>
		public void clearSelectedSectors()
		{
			General.Map.Map.ClearSelectedSectors();
		}

		#endregion

		#region ================== Creation

		/// <summary>
		/// Creates a new `Vertex` at the given position. The position can be a `Vector2D` or an `Array` of two numbers.
		/// ```
		/// var v1 = UDB.Map.createVertex(new Vector2D(32, 64));
		/// var v2 = UDB.Map.createVertex([ 32, 64 ]);
		/// ```
		/// </summary>
		/// <param name="pos">Position where the `Vertex` should be created at</param>
		/// <returns>The created `Vertex`</returns>
		public VertexWrapper createVertex(object pos)
		{
			try
			{
				Vector2D v = BuilderPlug.Me.GetVector3DFromObject(pos);
				Vertex newvertex = General.Map.Map.CreateVertex(v);

				if(newvertex == null)
					throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException("Failed to create new vertex");

				return new VertexWrapper(newvertex);
			}
			catch (CantConvertToVectorException e)
			{
				throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException(e.Message);
			}
		}

		/// <summary>
		/// Creates a new `Thing` at the given position. The position can be a `Vector2D`, `Vector3D`, or an `Array` of two numbers or three numbers (note that the z position only works for game configurations that support vertical pos. A thing type can be supplied optionally.
		/// ```
		/// var t1 = UDB.Map.createThing(new UDB.Vector2D(32, 64));
		/// var t2 = UDB.Map.createThing([ 32, 64 ]);
		/// var t3 = UDB.Map.createThing(new UDB.Vector2D(32, 64), 3001); // Create an Imp
		/// var t4 = UDB.Map.createThing([ 32, 64 ], 3001); // Create an Imp
		/// ```
		/// </summary>
		/// <param name="pos">Position where the `Thing` should be created at</param>
		/// <param name="type">Thing type (optional)</param>
		/// <returns>The new `Thing`</returns>
		public ThingWrapper createThing(object pos, int type=0)
		{
			try
			{
				if(type < 0)
					throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException("Thing type can not be negative.");

				Vector2D v = BuilderPlug.Me.GetVector3DFromObject(pos);
				Thing t = General.Map.Map.CreateThing();

				if(t == null)
					throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException("Failed to create new thing.");

				General.Settings.ApplyCleanThingSettings(t, type);

				t.Move(v);

				t.UpdateConfiguration();

				return new ThingWrapper(t);
			}
			catch (CantConvertToVectorException e)
			{
				throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException(e.Message);
			}
		}

		#endregion

		#region ================== Merging/Joining

		/// <summary>
		/// Joins `Sector`s, keeping lines shared by the `Sector`s. All `Sector`s will be joined with the first `Sector` in the array.
		/// </summary>
		/// <param name="sectors">`Array` of `Sector`s</param>
		public void joinSectors(SectorWrapper[] sectors)
		{
			List<Sector> secs = new List<Sector>();

			foreach (SectorWrapper sw in sectors)
				secs.Add(sw.Sector);

			JoinMergeSectors(secs, false);
		}

		/// <summary>
		/// Merges `Sector`s, deleting lines shared by the `Sector`s. All `Sector`s will be merged into the first `Sector` in the array.
		/// </summary>
		/// <param name="sectors">`Array` of `Sector`s</param>
		public void mergeSectors(SectorWrapper[] sectors)
		{
			List<Sector> secs = new List<Sector>();

			foreach (SectorWrapper sw in sectors)
				secs.Add(sw.Sector);

			JoinMergeSectors(secs, true);
		}

		// Helper method for joining/merging sectors. Pretty much a direct copy from sectors mode
		private void JoinMergeSectors(List<Sector> sectors, bool removelines)
		{
			// Remove lines in betwen joining sectors?
			if (removelines)
			{
				// Go for all selected linedefs
				List<Linedef> selectedlines = new List<Linedef>();
				foreach (Sector s in sectors)
					foreach (Sidedef sd in s.Sidedefs)
						if (!selectedlines.Contains(sd.Line))
							selectedlines.Add(sd.Line);

				foreach (Linedef ld in selectedlines)
				{
					// Front and back side?
					if ((ld.Front != null) && (ld.Back != null))
					{
						// Both a selected sector, but not the same?
						if (sectors.Contains(ld.Front.Sector) && sectors.Contains(ld.Back.Sector) &&
						   (ld.Front.Sector != ld.Back.Sector))
						{
							// Remove this line
							ld.Dispose();
						}
					}
				}
			}

			// Find the first sector that is not disposed
			//List<Sector> orderedselection = new List<Sector>(General.Map.Map.GetSelectedSectors(true));
			Sector first = null;
			foreach (Sector s in sectors)
				if (!s.IsDisposed) { first = s; break; }

			// Join all selected sectors with the first
			for (int i = 0; i < sectors.Count; i++)
				if ((sectors[i] != first) && !sectors[i].IsDisposed)
					sectors[i].Join(first);

			// Update
			BuilderPlug.Me.ScriptRunnerForm.RunAction(() => General.Map.Map.Update());
		}

		#endregion
	}
}
