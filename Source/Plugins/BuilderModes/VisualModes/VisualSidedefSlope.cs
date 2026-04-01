using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using CodeImp.DoomBuilder.BuilderModes;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Map;

namespace CodeImp.DoomBuilder.VisualModes
{
	internal class VisualSidedefSlope : BaseVisualSlope // VisualSlope, IVisualEventReceiver
	{
		#region ================== Variables

		private readonly Sidedef sidedef;

		#endregion

		#region ================== Constants

		private const int SIZE = 8;

		#endregion

		#region ================== Properties

		public Sidedef Sidedef { get { return sidedef; } }
		public int NormalizedAngleDeg { get { return (sidedef.Line.AngleDeg >= 180) ? (sidedef.Line.AngleDeg - 180) : sidedef.Line.AngleDeg; } }

		#endregion

		#region ================== Constructor / Destructor

		public VisualSidedefSlope(BaseVisualMode mode, SectorLevel level, Sidedef sidedef, bool up) : base(mode, level, up)
		{
			this.sidedef = sidedef;
			type = VisualSlopeType.Line;

			Update();

			// We have no destructor
			GC.SuppressFinalize(this);
		}

		#endregion

		#region ================== Methods

		public Vector3D GetCenterPoint()
		{
			Vector2D p = sidedef.Line.GetCenterPoint();
			return new Vector3D(p, level.plane.GetZ(p));
		}

		public override void Update()
		{
			plane = new Plane(level.plane.Normal, level.plane.Offset - 0.1f);

			if (!up)
				plane = plane.GetInverted();

			UpdatePosition();

			length = new Line3D(new Vector3D(sidedef.Line.Line.v1, plane.GetZ(sidedef.Line.Line.v1)), new Vector3D(sidedef.Line.Line.v2, plane.GetZ(sidedef.Line.Line.v2))).GetDelta().GetLength();
		}

		/// <summary>
		/// This is called when the thing must be tested for line intersection. This should reject
		/// as fast as possible to rule out all geometry that certainly does not touch the line.
		/// </summary>
		public override bool PickFastReject(Vector3D from, Vector3D to, Vector3D dir)
		{
			if (sidedef.IsDisposed || sidedef.Sector.IsDisposed)
				return false;

			RectangleF bbox = sidedef.Sector.BBox;

			if ((up && plane.Distance(from) > 0.0f) || (!up && plane.Distance(from) < 0.0f))
			{
				if (plane.GetIntersection(from, to, ref pickrayu))
				{
					if (pickrayu > 0.0f)
					{
						pickintersect = from + (to - from) * pickrayu;

						return ((pickintersect.x >= bbox.Left) && (pickintersect.x <= bbox.Right) &&
								(pickintersect.y >= bbox.Top) && (pickintersect.y <= bbox.Bottom));
					}
				}
			}

			return false;
		}

		/// <summary>
		/// This is called when the thing must be tested for line intersection. This should perform
		/// accurate hit detection and set u_ray to the position on the ray where this hits the geometry.
		/// </summary>
		public override bool PickAccurate(Vector3D from, Vector3D to, Vector3D dir, ref double u_ray)
		{
			u_ray = pickrayu;

			Sidedef sd = MapSet.NearestSidedef(sidedef.Sector.Sidedefs, pickintersect);
			if (sd == sidedef) {
				double side = sd.Line.SideOfLine(pickintersect);

				if ((side <= 0.0f && sd.IsFront) || (side > 0.0f && !sd.IsFront))
					return true;
			}

			return false;
		}

		/// <summary>
		/// Updates the position. Depending on 3D floors and which side of the linedef the slope handle is on the
		/// direction of the line used as a base has to be inverted
		/// </summary>
		public void UpdatePosition()
		{
			bool invertline = false;

			if (up)
			{
				if (level.extrafloor && level.type == SectorLevelType.Ceiling)
				{
					if (sidedef.IsFront)
						invertline = true;
				}
				else
				{
					if (!sidedef.IsFront)
						invertline = true;
				}
			}
			else
			{
				if (level.extrafloor && level.type == SectorLevelType.Floor)
				{
					if (!sidedef.IsFront)
						invertline = true;
				}
				else
				{
					if (sidedef.IsFront)
						invertline = true;
				}
			}

			if (invertline)
				SetPosition(new Line2D(sidedef.Line.End.Position, sidedef.Line.Start.Position), level.plane);
			else
				SetPosition(sidedef.Line.Line, level.plane);
		}

		/// <summary>
		/// Tries to find a slope handle to pivot around. If possible if finds the handle belonging to a line that has the
		/// same angle as the start handle, and is the furthest away. If such a handle does not exist it finds one that's
		/// closest to those specs
		/// </summary>
		/// <returns></returns>
		public override VisualSlope GetSmartPivotHandle()
		{
			List<IVisualEventReceiver> selectedsectors = mode.GetSelectedObjects(true, false, false, false, false);

			// Special handling for triangular sectors
			if (selectedsectors.Count == 0 && BuilderPlug.Me.UseOppositeSmartPivotHandle && sidedef.Sector.Sidedefs.Count == 3)
			{
				foreach(VisualVertexSlope vvs in mode.VertexSlopeHandles[sidedef.Sector])
				{
					if (vvs.Level == level && !vvs.Vertex.Linedefs.Contains(sidedef.Line))
						return vvs;
				}
			}

			VisualSlope handle = this;
			List<VisualSidedefSlope> potentialhandles = new List<VisualSidedefSlope>();

			if (selectedsectors.Count == 0)
			{
				// No sectors selected, so find all handles that belong to the same level
				foreach (VisualSidedefSlope checkhandle in mode.SidedefSlopeHandles[sidedef.Sector])
				{
					if (checkhandle != this && checkhandle.Level == level)
						potentialhandles.Add(checkhandle);
				}
			}
			else
			{
				// Sectors are selected, get all handles from those sectors that have the same level
				HashSet<Sector> sectors = new HashSet<Sector>();

				foreach (BaseVisualGeometrySector bvgs in selectedsectors)
					sectors.Add(bvgs.Sector.Sector);

				foreach (Sector s in sectors)
					foreach (VisualSidedefSlope checkhandle in mode.SidedefSlopeHandles[s])
					{
						if (checkhandle != this)
							foreach (BaseVisualGeometrySector bvgs in selectedsectors)
								if (bvgs.Level == checkhandle.Level)
									potentialhandles.Add(checkhandle);
					}
			}

			// Sort potential handles by their angle difference to the start handle. That means that handles with less angle difference will be at the beginning of the list
			List<VisualSidedefSlope> anglediffsortedhandles = potentialhandles.OrderBy(h => Math.Abs(NormalizedAngleDeg - h.NormalizedAngleDeg)).ToList();

			// Get all potential handles that have to same angle as the one that's closest to the start handle, then sort them by distance, and take the one that's furthest away
			if (anglediffsortedhandles.Count > 0)
				handle = anglediffsortedhandles.Where(h => h.NormalizedAngleDeg == anglediffsortedhandles[0].NormalizedAngleDeg).OrderByDescending(h => Math.Abs(sidedef.Line.Line.GetDistanceToLine(h.sidedef.Line.GetCenterPoint(), false))).First();

			if (handle == this)
				return null;

			return handle;
		}

		public static void ApplySlope(SectorLevel level, Plane plane, BaseVisualMode mode)
		{
			bool applytoceiling = false;
			bool reset = false;
			int height = 0;
			

			Vector2D center = new Vector2D(level.sector.BBox.X + level.sector.BBox.Width / 2,
											   level.sector.BBox.Y + level.sector.BBox.Height / 2);

			if (level.extrafloor)
			{
				// The top side of 3D floors is the ceiling of the sector, but it's a "floor" in UDB, so the
				// ceiling of the control sector has to be modified
				if (level.type == SectorLevelType.Floor)
					applytoceiling = true;
			}
			else
			{
				if (level.type == SectorLevelType.Ceiling)
					applytoceiling = true;
			}

			// If the plane horizontal remove the slope and set the sector height instead
			// Rounding errors can result in offsets of horizontal planes to be a tiny, tiny bit off a whole number,
			// assume we want to remove the plane in this case
			double diff = Math.Abs(Math.Round(plane.d) - plane.d);
			if (plane.Normal.z == 1.0 && diff < 0.000000001)
			{
				reset = true;
				height = -Convert.ToInt32(plane.d);
			}

			if (applytoceiling)
			{
				if (reset)
				{
					level.sector.CeilHeight = height;
					level.sector.CeilSlope = new Vector3D();
					level.sector.CeilSlopeOffset = double.NaN;
				}
				else
				{
					Plane downplane = plane.GetInverted();
					level.sector.CeilSlope = downplane.Normal;
					level.sector.CeilSlopeOffset = downplane.Offset;
				}
			}
			else
			{
				if (reset)
				{
					level.sector.FloorHeight = height;
					level.sector.FloorSlope = new Vector3D();
					level.sector.FloorSlopeOffset = double.NaN;
				}
				else
				{
					level.sector.FloorSlope = plane.Normal;
					level.sector.FloorSlopeOffset = plane.Offset;
				}
			}

			// Rebuild sector
			BaseVisualSector vs;
			if (mode.VisualSectorExists(level.sector))
			{
				vs = (BaseVisualSector)mode.GetVisualSector(level.sector);
			}
			else
			{
				vs = mode.CreateBaseVisualSector(level.sector);
			}

			if (vs != null) vs.UpdateSectorGeometry(true);
		}

		/// <summary>
		/// Gets the pivor point for this slope handle
		/// </summary>
		/// <returns>The pivot point as Vector3D</returns>
		public override Vector3D GetPivotPoint()
		{
			return new Vector3D(sidedef.Line.Line.GetCoordinatesAt(0.5), level.plane.GetZ(sidedef.Line.Line.GetCoordinatesAt(0.5)));
		}

		public List<Vector3D> GetPivotPoints()
		{
			return new List<Vector3D>()
			{
				new Vector3D(sidedef.Line.Start.Position, level.plane.GetZ(sidedef.Line.Start.Position)),
				new Vector3D(sidedef.Line.End.Position, level.plane.GetZ(sidedef.Line.End.Position))
			};
		}

		#endregion

		#region ================== Events

		public override void OnChangeTargetHeight(int amount)
		{
			VisualSlope pivothandle = null;
			List<IVisualEventReceiver> selectedsectors = mode.GetSelectedObjects(true, false, false, false, false);
			List<SectorLevel> levels = new List<SectorLevel>();

			if (selectedsectors.Count == 0)
				levels.Add(level);
			else
			{
				foreach (BaseVisualGeometrySector bvgs in selectedsectors)
					levels.Add(bvgs.Level);

				if (!levels.Contains(level))
					levels.Add(level);
			}

			// Try to find a slope handle the user set to be the pivot handle
			// TODO: doing this every time is kind of stupid. Maybe store the pivot handle in the mode?
			foreach (KeyValuePair<Sector, List<VisualSlope>> kvp in mode.AllSlopeHandles)
			{
				foreach (VisualSlope handle in kvp.Value)
				{
					if (handle.Pivot)
					{
						pivothandle = handle;
						break;
					}
				}
			}

			// Can't pivot around itself
			if(pivothandle == this)
			{
				General.Interface.DisplayStatus(Windows.StatusType.Warning, "Slope handle to modify can't be the same as the pivot handle");
				return;
			}

			// User didn't set a pivot handle, try to find the smart pivot handle
			if(pivothandle == null)
				pivothandle = GetSmartPivotHandle();

			// Still no pivot handle, cancle
			if (pivothandle == null)
				return;

			// Build a new plane. p1 and p2 are the points of the slope handle that is modified, with the changed amound added; p3 is on the line of the pivot handle
			Vector3D p1 = new Vector3D(sidedef.Line.Start.Position, level.plane.GetZ(sidedef.Line.Start.Position) + amount);
			Vector3D p2 = new Vector3D(sidedef.Line.End.Position, level.plane.GetZ(sidedef.Line.End.Position) + amount);
			Vector3D p3 = pivothandle.GetPivotPoint();

			Plane plane = new Plane(p1, p2, p3, true);

			// Completely vertical planes are not possible. This can for example happen when trying to pivot around the slope handle on the opposite side of a line
			if (Math.Abs(plane.a) == 1.0 || Math.Abs(plane.b) == 1.0)
			{
				General.Interface.DisplayStatus(Windows.StatusType.Warning, "Resulting plane is completely vertical, which is impossible. Aborting");
				return;
			}

			mode.CreateUndo("Change slope");

			// Apply slope to surfaces
			foreach (SectorLevel l in levels)
				ApplySlope(l, plane, mode);

			mode.SetActionResult("Changed slope.");
		}

		#endregion
	}
}