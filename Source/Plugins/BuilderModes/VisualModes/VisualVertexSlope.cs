using System;
using System.Collections.Generic;
using System.Drawing;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CodeImp.DoomBuilder.BuilderModes;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Map;

namespace CodeImp.DoomBuilder.VisualModes
{
	/// <summary>
	/// Stores the normalized angle (clockwise from the vertex) and if the original line's front points in a clockwise or counter clockwise direction
	/// </summary>
	internal struct LineAngleInfo
	{
		public double angle;
		public bool clockwise;

		public LineAngleInfo(Linedef ld, Vertex v, Sector s)
		{
			Line2D line;

			if (ld.Start == v)
			{
				line = ld.Line;
				clockwise = true;
			}
			else
			{
				line = new Line2D(ld.End.Position, ld.Start.Position);
				clockwise = false;
			}

			angle = line.GetAngle();

			if (ld.Front?.Sector != s)
				clockwise = !clockwise;
		}
	}

	internal class VisualVertexSlope : BaseVisualSlope
	{
		#region ================== Variables

		private readonly Vertex vertex;
		private readonly Sector sector;
		private double angle;

		#endregion

		#region ================== Properties

		public Vertex Vertex { get { return vertex; } }
		public Sector Sector { get { return sector; } }

		#endregion

		#region ================== Constructor / Destructor

		public VisualVertexSlope(BaseVisualMode mode, SectorLevel level, Vertex vertex, Sector sector, bool up) : base(mode, level, up)
		{
			this.vertex = vertex;
			this.sector = sector;

			type = VisualSlopeType.Vertex;

			ComputeAngle();

			Update();

			GC.SuppressFinalize(this);
		}

		#endregion

		#region ================== Methods

		private void ComputeAngle()
		{
			List<LineAngleInfo> lines = new List<LineAngleInfo>();
			List<double> angles = new List<double>();

			// Get all lines that we have to take into account. Ignore linedefs where both sides
			// don't belong to the sector or don't belong to the sector
			foreach (Linedef ld in vertex.Linedefs)
			{
				if (ld.IsDisposed)
					continue;
				else
				{
					bool frontsame = false;
					bool backsame = false;

					if (ld.Front != null && ld.Front.Sector == sector)
						frontsame = true;

					if (ld.Back != null && ld.Back.Sector == sector)
						backsame = true;

					if (frontsame == backsame)
						continue;
				}

				lines.Add(new LineAngleInfo(ld, vertex, sector));
			}

			// Special case handling
			if (lines.Count < 2)
			{
				if (vertex.Linedefs.Count == 1)
				{
					angle = vertex.Linedefs.First().Angle + Angle2D.PIHALF;
					return;
				}

				angle = 0.0;
				return;
			}

			// Sort lines by their normalized angle
			lines.Sort((a, b) => a.angle.CompareTo(b.angle));

			// Get the other line we want to compute the angle between
			int other = 1;
			if (!lines[0].clockwise)
				other = lines.Count - 1;

			Vector2D v1 = Vector2D.FromAngle(lines[0].angle);
			Vector2D v2 = Vector2D.FromAngle(lines[other].angle);

			angle = lines[0].angle + (Math.Atan2(v2.y, v2.x) - Math.Atan2(v1.y, v1.x)) / 2.0;

			// If the first line is going clockwise we have to add 180°
			if (lines[0].clockwise)
				angle += Angle2D.PI;

			// Add 90° to get it in line with Doom's angles
			angle += Angle2D.PIHALF;

			// Also need to modify the angle for ceilings
			if (level.type == SectorLevelType.Ceiling)
				angle += Angle2D.PI;

			angle = Angle2D.Normalized(angle);
		}

		public override void Update()
		{
			plane = new Plane(level.plane.Normal, level.plane.Offset - 0.1f);

			if (!up)
				plane = plane.GetInverted();

			UpdatePosition();
		}

		public void UpdatePosition()
		{
			Vector2D av = Vector2D.FromAngle(angle);

			SetPosition(new Line2D(vertex.Position, vertex.Position + av), level.plane);
		}

		/// <summary>
		/// Finds a slope handle to pivot around. It takes the vertex that's furthest away from the given handle
		/// </summary>
		/// <returns></returns>
		public override VisualSlope GetSmartPivotHandle()
		{
			List<IVisualEventReceiver> selectedsectors = mode.GetSelectedObjects(true, false, false, false, false);

			// Special handling for triangular sectors
			if (selectedsectors.Count == 0 && BuilderPlug.Me.UseOppositeSmartPivotHandle && sector.Sidedefs.Count == 3)
			{
				foreach (VisualSidedefSlope vss in mode.SidedefSlopeHandles[sector])
				{
					if (vss.Level == level && !(vss.Sidedef.Line.Start == vertex || vss.Sidedef.Line.End == vertex))
						return vss;
				}
			}

			VisualSlope handle = this;
			List<VisualVertexSlope> potentialhandles = new List<VisualVertexSlope>();

			if (selectedsectors.Count == 0)
			{
				// No sectors selected, so find all handles that belong to the same level
				foreach (VisualVertexSlope checkhandle in mode.VertexSlopeHandles[sector])
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
					foreach (VisualVertexSlope checkhandle in mode.VertexSlopeHandles[s])
					{
						if (checkhandle != this)
							foreach (BaseVisualGeometrySector bvgs in selectedsectors)
								if (bvgs.Level == checkhandle.Level)
									potentialhandles.Add(checkhandle);
					}
			}

			handle = potentialhandles.OrderByDescending(h => Vector2D.Distance(h.Vertex.Position, vertex.Position)).First();

			if (handle == this)
				return null;

			return handle;
		}

		/// <summary>
		/// This is called when the thing must be tested for line intersection. This should reject
		/// as fast as possible to rule out all geometry that certainly does not touch the line.
		/// </summary>
		public override bool PickFastReject(Vector3D from, Vector3D to, Vector3D dir)
		{
			if (vertex.IsDisposed || sector.IsDisposed)
				return false;

			RectangleF bbox = sector.BBox;

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

			Sidedef sd = MapSet.NearestSidedef(sector.Sidedefs, pickintersect);
			Vertex v = MapSet.NearestVertex(new Vertex[] { sd.Line.Start, sd.Line.End }, pickintersect);
			if (v == vertex)
			{
				double side = sd.Line.SideOfLine(pickintersect);

				if ((side <= 0.0f && sd.IsFront) || (side > 0.0f && !sd.IsFront))
					return true;
			}

			return false;
		}

		/// <summary>
		/// Gets the pivor point for this slope handle
		/// </summary>
		/// <returns>The pivot point as Vector3D</returns>
		public override Vector3D GetPivotPoint()
		{
			return new Vector3D(vertex.Position, level.plane.GetZ(vertex.Position));
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
			if (pivothandle == this)
			{
				General.Interface.DisplayStatus(Windows.StatusType.Warning, "Slope handle to modify can't be the same as the pivot handle");
				return;
			}

			// User didn't set a pivot handle, try to find the smart pivot handle
			if (pivothandle == null)
				pivothandle = GetSmartPivotHandle();

			// Still no pivot handle, cancle
			if (pivothandle == null)
				return;

			Vector3D p1, p2, p3;

			if (pivothandle is VisualVertexSlope)
			{
				// Build a new plane. Since we only got 2 points (the pivot point of the pivot handle and the vertex slope vertex) we need
				// to create a third point. That's done by getting the perpendicular of the line between the aforementioned 2 points, then
				// add the perpendicular to the vertex position of the vertex slope vertex
				p3 = pivothandle.GetPivotPoint();
				Vector2D perp = new Line2D(vertex.Position, p3).GetPerpendicular();

				p1 = new Vector3D(vertex.Position, level.plane.GetZ(vertex.Position) + amount);
				p2 = new Vector3D(vertex.Position + perp, level.plane.GetZ(vertex.Position + perp) + amount);
			}
			else // VisualSidedefSlope
			{
				List<Vector3D> pivotpoints = ((VisualSidedefSlope)pivothandle).GetPivotPoints();
				p1 = new Vector3D(vertex.Position, level.plane.GetZ(vertex.Position) + amount);
				p2 = pivotpoints[0];
				p3 = pivotpoints[1];
			}

			Plane plane = new Plane(p1, p2, p3, true);

			// Completely vertical planes are not possible
			if (Math.Abs(plane.a) == 1.0 || Math.Abs(plane.b) == 1.0)
			{
				General.Interface.DisplayStatus(Windows.StatusType.Warning, "Resulting plane is completely vertical, which is impossible. Aborting");
				return;
			}

			mode.CreateUndo("Change slope");

			// Apply slope to surfaces
			foreach (SectorLevel l in levels)
				VisualSidedefSlope.ApplySlope(l, plane, mode);

			mode.SetActionResult("Changed slope.");
		}

		public override void OnSelectEnd()
		{
			// The base's OnSelectEnd might not change the selection status if the user tries to select
			// a pivot handle, so we have to check if the selection status changed
			bool oldselected = selected;

			base.OnSelectEnd();

			if (selected != oldselected && BuilderPlug.Me.SelectAdjacentVisualVertexSlopeHandles)
			{
				HashSet<Sector> sectors = new HashSet<Sector>();

				// Get the z position of this vertex slope handle. By comparing the t position of
				// other vertex slope handles we can find the ones that are really adjacent, since
				// there can be many vertex slope handles on one vertex on different heights especially 
				// with 3D floors. The rounding is done since there can be the tiniest differences in
				// the planes, which lead to different z positions even when they are the "same".
				double z = Math.Round(level.plane.GetZ(vertex.Position), 5);

				// Get all sectors around the this slope handle's vertex
				foreach (Linedef ld in vertex.Linedefs)
				{
					if (ld.Front != null)
						sectors.Add(ld.Front.Sector);
					if (ld.Back != null)
						sectors.Add(ld.Back.Sector);
				}

				foreach (Sector s in sectors)
				{
					foreach (VisualVertexSlope vs in mode.VertexSlopeHandles[s])
					{
						if (vs.vertex != vertex || vs == this)
							continue;

						// See above for the explanation of the rounding
						double z1 = Math.Round(vs.level.plane.GetZ(vertex.Position), 5);

						if (z == z1)
						{
							if (selected)
							{
								if (!vs.selected)
								{
									vs.selected = true;
									mode.AddSelectedObject(vs);
									mode.UsedSlopeHandles.Add(vs);
								}
							}
							else
							{
								if (vs.selected)
								{
									vs.selected = false;
									mode.RemoveSelectedObject(vs);
									mode.UsedSlopeHandles.Remove(vs);
								}
							}
						}
					}
				}
			}
		} 

		#endregion
	}
}
