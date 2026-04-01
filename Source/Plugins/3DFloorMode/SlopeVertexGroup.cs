#region ================== Namespaces

using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Types;

#endregion

namespace CodeImp.DoomBuilder.ThreeDFloorMode
{
	public class SlopeVertexGroup
	{
		#region ================== Variables
		private List<SlopeVertex> vertices;
		private List<Sector> sectors;
		private Dictionary<Sector, PlaneType> sectorplanes;
		private List<Sector> taggedsectors; // For highlighting 3D floors
		private int id;
		private int height;
		private Vertex anchorvertex;
		private Vector2D anchor;
		private bool reposition;
		private bool spline;

		#endregion

		#region ================== Enums

		#endregion

		#region ================== Properties

		public List<SlopeVertex> Vertices { get { return vertices; } set { vertices = value; ComputeHeight(); } }
		public List<Sector> Sectors { get { return sectors; } set { sectors = value; } }
		public Dictionary<Sector, PlaneType> SectorPlanes { get { return sectorplanes; } }
		public List<Sector> TaggedSectors { get { return taggedsectors; } set { taggedsectors = value; } }
		public int Id { get { return id; } }
		public int Height { get { return height; } set { height = value; } }
		public bool Reposition { get { return reposition; } set { reposition = value; } }
		public bool Spline { get { return spline; }set { spline = value; } }

		#endregion

		#region ================== Constructors

		public SlopeVertexGroup(int id, Sector sector)
		{
			List<string> list = new List<string> { "floor", "ceiling" };
			Type type = typeof(SlopeVertexGroup);

			this.id = id;
			sectors = new List<Sector>();
			sectorplanes = new Dictionary<Sector, PlaneType>();
			taggedsectors = new List<Sector>();
			vertices = new List<SlopeVertex>();
			anchorvertex = null;

			// There will always be at least two slope vertices, so add them here
			vertices.Add(new SlopeVertex(sector, id, 0));
			vertices.Add(new SlopeVertex(sector, id, 1));

			// Check if there's a third slope vertex, and add it if there is
			string vertexidentifier = String.Format("user_svg{0}_v2_x", id);

			foreach (KeyValuePair<string, UniValue> kvp in sector.Fields)
			{
				if (kvp.Key == vertexidentifier)
				{
					vertices.Add(new SlopeVertex(sector, id, 2));
					break;
				}
			}

			// Get reposition value
			reposition = sector.Fields.GetValue(String.Format("user_svg{0}_reposition", id), true);

			// Get spline value
			spline = sector.Fields.GetValue(String.Format("user_svg{0}_spline", id), false);

			spline = false;

			ComputeHeight();
			FindSectors();
		}

		public SlopeVertexGroup(int id, List<SlopeVertex> vertices)
		{
			this.vertices = vertices;
			this.id = id;
			sectors = new List<Sector>();
			sectorplanes = new Dictionary<Sector, PlaneType>();
			taggedsectors = new List<Sector>();
			anchorvertex = null;
			height = 0;

			ComputeHeight();
		}

		#endregion

		#region ================== Methods

		public void FindSectors()
		{
			if (sectors == null)
				sectors = new List<Sector>();
			else
				sectors.Clear();

			if (taggedsectors == null)
				taggedsectors = new List<Sector>();
			else
				taggedsectors.Clear();

			sectorplanes.Clear();

			foreach (Sector s in General.Map.Map.Sectors)
			{
				bool onfloor = s.Fields.GetValue("user_floorplane_id", -1) == id;
				bool onceiling = s.Fields.GetValue("user_ceilingplane_id", -1) == id;
				PlaneType pt = 0;

				if (!onfloor && !onceiling)
					continue;

				sectors.Add(s);

				if (onfloor && onceiling)
					pt = PlaneType.Floor | PlaneType.Ceiling;
				else if (onfloor)
					pt = PlaneType.Floor;
				else if (onceiling)
					pt = PlaneType.Ceiling;

				sectorplanes.Add(s, pt);

				GetTaggesSectors(s, pt);
			}
		}

        public void RemovePlanes()
        {
            foreach (Sector s in sectors.ToList())
            {
                RemoveSector(s, this.sectorplanes[s]);
            }
        }

        public void RemoveFromSectors()
		{
			foreach (Sector s in sectors.ToList())
			{
				RemoveSector(s, PlaneType.Floor);
				RemoveSector(s, PlaneType.Ceiling);
			}
		}

		private void GetTaggesSectors(Sector s, PlaneType pt)
		{
			// Check if the current sector is a 3D floor control sector. If that's the case also store the
			// tagged sector(s). They will be used for highlighting in slope mode
			foreach (Sidedef sd in s.Sidedefs)
			{
				if (sd.Line.Action == 160)
				{
					foreach (Sector ts in BuilderPlug.GetSectorsByTag(sd.Line.Args[0]))
					{
						if (!taggedsectors.Contains(ts))
							taggedsectors.Add(ts);

						if(!sectorplanes.ContainsKey(ts))
							sectorplanes.Add(ts, pt);
					}
				}
			}
		}

		public void AddSector(Sector s, PlaneType pt)
		{
			if (sectorplanes.ContainsKey(s))
			{
				pt |= sectorplanes[s];
				sectorplanes.Remove(s);
			}

			if (sectors.Contains(s))
				sectors.Remove(s);

			sectorplanes.Add(s, pt);
			sectors.Add(s);

			GetTaggesSectors(s, pt);

			ApplyToSectors();
		}

		public void RemoveSector(Sector s, PlaneType pt)
		{
			Debug.WriteLine("Removing from Sector " + s.Index.ToString() + ": " + pt.ToString());

			if (sectorplanes.ContainsKey(s))
			{
				if (sectors.Contains(s) && sectorplanes[s] == pt)
				{
					sectors.Remove(s);
					sectorplanes.Remove(s);
				}
				else
					sectorplanes[s] &= ~pt;
			}

			if ((pt & PlaneType.Floor) == PlaneType.Floor)
			{
				s.FloorSlope = new Vector3D();
				s.FloorSlopeOffset = 0;
				s.Fields.Remove("user_floorplane_id");
			}

			if ((pt & PlaneType.Ceiling) == PlaneType.Ceiling)
			{
				s.CeilSlope = new Vector3D();
				s.CeilSlopeOffset = 0;
				s.Fields.Remove("user_ceilingplane_id");
			}
		}

		public void RemoveUndoRedoUDMFFields(Sector s)
		{
			string fieldname = "";
			string[] comp = new string[] { "x", "y", "z" };

			if (s == null || s.IsDisposed)
				return;

			s.Fields.BeforeFieldsChange();

			for (int i = 0; i < 3; i++)
			{
				foreach (string c in comp)
				{
					fieldname = string.Format("user_svg{0}_v{1}_{2}", id, i, c);

					if (s.Fields.ContainsKey(fieldname))
						s.Fields.Remove(fieldname);
				}
			}

			// Remove reposition field
			fieldname = string.Format("user_svg{0}_reposition", id);
			if (s.Fields.ContainsKey(fieldname))
				s.Fields.Remove(fieldname);
		}

		public void ApplyToSectors()
		{
			List<Sector> removesectors = new List<Sector>();

			ComputeHeight();

			foreach (Sector s in sectors)
			{
				bool hasplane = false;

				if (sectorplanes.ContainsKey(s) && (sectorplanes[s] & PlaneType.Floor) == PlaneType.Floor)
				{
					hasplane = true;

					if (s.Fields.ContainsKey("user_floorplane_id"))
						s.Fields["user_floorplane_id"] = new UniValue(UniversalType.Integer, id);
					else
						s.Fields.Add("user_floorplane_id", new UniValue(UniversalType.Integer, id));
				}
				else if (s.Fields.ContainsKey("user_floorplane_id") && s.Fields.GetValue("user_floorplane_id", -1) == id)
				{
					s.Fields.Remove("user_floorplane_id");
				}

				if (sectorplanes.ContainsKey(s) && (sectorplanes[s] & PlaneType.Ceiling) == PlaneType.Ceiling)
				{
					hasplane = true;

					if (s.Fields.ContainsKey("user_ceilingplane_id"))
						s.Fields["user_ceilingplane_id"] = new UniValue(UniversalType.Integer, id);
					else
						s.Fields.Add("user_ceilingplane_id", new UniValue(UniversalType.Integer, id));
				}
				else if (s.Fields.ContainsKey("user_ceilingplane_id") && s.Fields.GetValue("user_ceilingplane_id", -1) == id)
				{
					s.Fields.Remove("user_ceilingplane_id");
				}

				if (!hasplane)
					removesectors.Add(s);
			}

			foreach (Sector s in removesectors)
				sectors.Remove(s);

			foreach (Sector s in sectors)
				BuilderPlug.Me.UpdateSlopes(s);
		}

		public void StoreInSector(Sector sector)
		{
			// Make sure the field work with undo/redo
			sector.Fields.BeforeFieldsChange();

			// Also store all slope vertices in the sector
			for (int i = 0; i < vertices.Count; i++)
				vertices[i].StoreInSector(sector, id, i);

			string identifier = String.Format("user_svg{0}_reposition", id);

			// Add, update, or delete the reposition field
			if (reposition)
			{
				//default action
				if (sector.Fields.ContainsKey(identifier))
					sector.Fields.Remove(identifier);
			}
			else
			{
				if (sector.Fields.ContainsKey(identifier))
					sector.Fields[identifier] = new UniValue(UniversalType.Boolean, reposition);
				else
					sector.Fields.Add(identifier, new UniValue(UniversalType.Boolean, reposition));
			}

			// Spline
			identifier = String.Format("user_svg{0}_spline", id);

			if (!spline && sector.Fields.ContainsKey(identifier))
				sector.Fields.Remove(identifier);
			else if (spline)
			{
				if (sector.Fields.ContainsKey(identifier))
					sector.Fields[identifier] = new UniValue(UniversalType.Boolean, spline);
				else
					sector.Fields.Add(identifier, new UniValue(UniversalType.Boolean, spline));
			}
		}

		public void SelectVertices(bool select)
		{
			foreach (SlopeVertex sv in vertices)
				sv.Selected = select;
		}

		public bool GetAnchor()
		{
			anchorvertex = null;

			if (sectors.Count == 0)
				return false;

			// Try to find a sector that contains a SV
			/*
			foreach (Sector s in sectors)
			{
				foreach (SlopeVertex sv in vertices)
				{
					if (s.Intersect(sv.Pos))
					{
						anchorvertex = s.Sidedefs.First().Line.Start;
						anchor = new Vector2D(anchorvertex.Position);
						return true;
					}
				}
			}
			*/

			// Just grab the next best vertex
			foreach (Sector s in sectors)
			{
				foreach (Sidedef sd in s.Sidedefs)
				{
					anchorvertex = sd.Line.Start;
					anchor = new Vector2D(anchorvertex.Position);
					return true;
				}
			}

			return false;
		}

		public void RepositionByAnchor()
		{
			if (anchorvertex == null || !reposition)
				return;

			Vector2D diff = anchorvertex.Position - anchor;

			if (diff.x == 0.0f && diff.y == 0.0f)
				return;

			foreach (SlopeVertex sv in vertices)
			{
				sv.Pos += diff;
			}

			anchorvertex = null;
		}

		public void ComputeHeight()
		{
			List<Vector3D> sp = new List<Vector3D>();

			for (int i = 0; i < vertices.Count; i++)
			{
				sp.Add(new Vector3D(vertices[i].Pos.x, vertices[i].Pos.y,vertices[i].Z));
			}

			if (vertices.Count == 2)
			{
				double z = sp[0].z;
				Line2D line = new Line2D(sp[0], sp[1]);
				Vector3D perpendicular = line.GetPerpendicular();

				Vector2D v = sp[0] + perpendicular;

				sp.Add(new Vector3D(v.x, v.y, z));
			}

			Plane p = new Plane(sp[0], sp[1], sp[2], true);

			double fheight = p.GetZ(GetCircumcenter(sp));

			// If something went wrong with computing the height use the height
			// of the first vertex as a workaround
			if (double.IsNaN(fheight))
				height = Convert.ToInt32(sp[0].z);
			else
				height = Convert.ToInt32(fheight);
		}

		private Vector2D GetCircumcenter(List<Vector3D> points)
		{
			double u_ray;

			Line2D line1 = new Line2D(points[0], points[1]);
			Line2D line2 = new Line2D(points[2], points[0]);

			// Perpendicular bisectors
			Line2D bisector1 = new Line2D(line1.GetCoordinatesAt(0.5f), line1.GetCoordinatesAt(0.5f) + line1.GetPerpendicular());
			Line2D bisector2 = new Line2D(line2.GetCoordinatesAt(0.5f), line2.GetCoordinatesAt(0.5f) + line2.GetPerpendicular());

			bisector1.GetIntersection(bisector2, out u_ray);

			return bisector1.GetCoordinatesAt(u_ray);
		}

		public bool VerticesAreValid()
		{
			if (vertices.Count == 2 && vertices[0].Pos == vertices[1].Pos)
				return false;

			if (vertices.Count == 3)
			{
				double side = Line2D.GetSideOfLine(vertices[0].Pos, vertices[1].Pos, vertices[3].Pos);

				if (side == 0.0f)
					return false;
			}

			return true;
		}

		#endregion
	}
}