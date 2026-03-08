
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
using ScintillaNET;
using System.Collections.Generic;
using System.Linq;
using CodeImp.DoomBuilder.Map;

#endregion

namespace CodeImp.DoomBuilder.Map
{
	public abstract class MapElement3D
	{
		public abstract bool IsDisposed { get; }

		public abstract override bool Equals(object o);
		public abstract override int GetHashCode();
		public abstract string Serialize();

		public static bool operator ==(MapElement3D a, MapElement3D b) => Equals(a, b);
		public static bool operator !=(MapElement3D a, MapElement3D b) => !Equals(a, b);

		protected static T ParseMapElementByString<T>(string s, ICollection<T> elements) where T : MapElement
		{
			if (!int.TryParse(s, out int index)) return null;
			return (index >= 0 && index < elements.Count) ? elements.ElementAt(index) : null;
		}

		public static string Serialize(MapElement3D e)
		{
			string prefix;

			if (e is Floor3D)
				prefix = "floor";
			else if (e is Ceiling3D)
				prefix = "ceiling";
			else if (e is Lower3D)
				prefix = "low";
			else if (e is Middle3D)
				prefix = "mid";
			else if (e is Upper3D)
				prefix = "up";
			else if (e is Vertex3D)
				prefix = "vertex";
			else if (e is Thing3D)
				prefix = "thing";
			else if (e is ThreeDFloorBottom3D)
				prefix = "tdbottom";
			else if (e is ThreeDFloorTop3D)
				prefix = "tdtop";
			else if (e is ThreeDFloorSide3D)
				prefix = "tdside";
			else if (e is SidedefSlope3D)
				prefix = "sideslope";
			else if (e is VertexSlope3D)
				prefix = "vertexslope";
			else
				return null;

			return prefix + "(" + e.Serialize() + ")";
		}

		public static MapElement3D Deserialize(string s, MapSet map)
		{
			s = s.Trim();

			int startindex = s.IndexOf('(');
			if (startindex < 0) return null;

			string type = s.Substring(0, startindex);
			string content = s.Substring(startindex + 1, s.Length - startindex - 1);

			if (type == "floor")
				return Floor3D.Deserialize(content, map);
			else if (type == "ceiling")
				return Ceiling3D.Deserialize(content, map);
			else if (type == "low")
				return Lower3D.Deserialize(content, map);
			else if (type == "mid")
				return Middle3D.Deserialize(content, map);
			else if (type == "up")
				return Upper3D.Deserialize(content, map);
			else if (type == "vertex")
				return Vertex3D.Deserialize(content, map);
			else if (type == "thing")
				return Thing3D.Deserialize(content, map);
			else if (type == "tdbottom")
				return ThreeDFloorBottom3D.Deserialize(content, map);
			else if (type == "tdtop")
				return ThreeDFloorTop3D.Deserialize(content, map);
			else if (type == "tdside")
				return ThreeDFloorSide3D.Deserialize(content, map);
			else if (type == "sideslope")
				return SidedefSlope3D.Deserialize(content, map);
			else if (type == "vertexslope")
				return VertexSlope3D.Deserialize(content, map);
			else
				return null;
		}
	}

	public abstract class SectorSurface3D : MapElement3D
	{
		public Sector Sector { get; protected set; }
		public override bool IsDisposed { get => Sector.IsDisposed; }

		protected SectorSurface3D(Sector sector)
		{
			Sector = sector;
		}

		public override string Serialize() => Sector.Index.ToString();
	}

	public class Floor3D : SectorSurface3D
	{
		public Floor3D(Sector sector) : base(sector) { }

		public override bool Equals(object o) => o is Floor3D f && Sector == f.Sector;
		public override int GetHashCode() => Sector.GetHashCode();

		public new static Floor3D Deserialize(string s, MapSet map)
		{
			var sector = MapElement3D.ParseMapElementByString(s, map.Sectors);
			if (sector == null) return null;

			return new Floor3D(sector);
		}
	}

	public class Ceiling3D : SectorSurface3D
	{
		public Ceiling3D(Sector sector) : base(sector) { }

		public override bool Equals(object o) => o is Ceiling3D c && Sector == c.Sector;
		public override int GetHashCode() => Sector.GetHashCode();

		public new static Ceiling3D Deserialize(string s, MapSet map)
		{
			var sector = MapElement3D.ParseMapElementByString(s, map.Sectors);
			if (sector == null) return null;

			return new Ceiling3D(sector);
		}
	}

	public abstract class SidedefPart3D : MapElement3D
	{
		public Sidedef Sidedef { get; protected set; }
		public override bool IsDisposed { get => Sidedef.IsDisposed; }

		protected SidedefPart3D(Sidedef sidedef)
		{
			Sidedef = sidedef;
		}

		public override string Serialize() => Sidedef.Index.ToString();
	}

	public class Lower3D : SidedefPart3D {
		public Lower3D(Sidedef sidedef) : base(sidedef) { }

		public override bool Equals(object o) => o is Lower3D l && Sidedef == l.Sidedef;
		public override int GetHashCode() => Sidedef.GetHashCode();

		public new static Lower3D Deserialize(string s, MapSet map)
		{
			var side = MapElement3D.ParseMapElementByString(s, map.Sidedefs);
			if (side == null) return null;

			return new Lower3D(side);
		}
	}

	public class Middle3D : SidedefPart3D {
		public Middle3D(Sidedef sidedef) : base(sidedef) { }

		public override bool Equals(object o) => o is Middle3D m && Sidedef == m.Sidedef;
		public override int GetHashCode() => Sidedef.GetHashCode();

		public new static Middle3D Deserialize(string s, MapSet map)
		{
			var side = MapElement3D.ParseMapElementByString(s, map.Sidedefs);
			if (side == null) return null;

			return new Middle3D(side);
		}
	}

	public class Upper3D : SidedefPart3D {
		public Upper3D(Sidedef sidedef) : base(sidedef) { }

		public override bool Equals(object o) => o is Upper3D u && Sidedef == u.Sidedef;
		public override int GetHashCode() => Sidedef.GetHashCode();

		public new static Upper3D Deserialize(string s, MapSet map)
		{
			var side = MapElement3D.ParseMapElementByString(s, map.Sidedefs);
			if (side == null) return null;

			return new Upper3D(side);
		}
	}

	public class Vertex3D : MapElement3D
	{
		public Vertex Vertex { get; protected set; }
		public bool IsFloor { get => !IsCeiling; }
		public bool IsCeiling { get; protected set; }
		public override bool IsDisposed { get => Vertex.IsDisposed; }

		public Vertex3D(Vertex vertex, bool ceiling)
		{
			Vertex = vertex;
			IsCeiling = ceiling;
		}

		public override bool Equals(object o) => o is Vertex3D v && Vertex == v.Vertex && IsCeiling == v.IsCeiling;
		public override int GetHashCode() => Vertex.GetHashCode();
		public override string Serialize() => $"{(IsCeiling ? "c" : "f")} {Vertex.Index}";

		public new static Vertex3D Deserialize(string s, MapSet map)
		{
			var parts = s.Split(' ');
			if (parts.Length != 2) return null;

			bool ceiling = (parts[0] == "c");

			var vertex = MapElement3D.ParseMapElementByString(parts[1], map.Vertices);
			if (vertex == null) return null;

			return new Vertex3D(vertex, ceiling);
		}
	}

	public class Thing3D : MapElement3D
	{
		public Thing Thing { get; protected set; }
		public override bool IsDisposed { get => Thing.IsDisposed; }

		public Thing3D(Thing vertex)
		{
			Thing = vertex;
		}

		public override bool Equals(object o) => o is Thing3D t && Thing == t.Thing;
		public override int GetHashCode() => Thing.GetHashCode();
		public override string Serialize() => Thing.Index.ToString();

		public new static Thing3D Deserialize(string s, MapSet map)
		{
			var thing = MapElement3D.ParseMapElementByString(s, map.Things);
			if (thing == null) return null;

			return new Thing3D(thing);
		}
	}

	public abstract class ThreeDFloorSurface3D : MapElement3D
	{
		public Linedef ControlLinedef { get; protected set; }
		public Sector Sector { get; protected set; }
		public override bool IsDisposed { get => ControlLinedef.IsDisposed || Sector.IsDisposed; }

		protected ThreeDFloorSurface3D(Linedef controllinedef, Sector sidedef)
		{
			ControlLinedef = controllinedef;
			Sector = sidedef;
		}

		public override string Serialize() => ControlLinedef.Index + " " + Sector.Index;
	}

	public class ThreeDFloorBottom3D : ThreeDFloorSurface3D {
		public ThreeDFloorBottom3D(Linedef controllinedef, Sector sector) : base(controllinedef, sector) { }

		public override bool Equals(object o) => o is ThreeDFloorBottom3D b && ControlLinedef == b.ControlLinedef && Sector == b.Sector;
		public override int GetHashCode() => (ControlLinedef?.GetHashCode() ?? 0) + Sector.GetHashCode();

		public new static ThreeDFloorBottom3D Deserialize(string s, MapSet map)
		{
			var parts = s.Split(' ');
			if (parts.Length != 2) return null;

			var controlline = MapElement3D.ParseMapElementByString(parts[0], map.Linedefs);
			if (controlline == null && parts[0] != "-1") return null;

			var sector = MapElement3D.ParseMapElementByString(parts[1], map.Sectors);
			if (sector == null) return null;

			return new ThreeDFloorBottom3D(controlline, sector);
		}
	}

	public class ThreeDFloorTop3D : ThreeDFloorSurface3D {
		public ThreeDFloorTop3D(Linedef controllinedef, Sector sector) : base(controllinedef, sector) { }

		public override bool Equals(object o) => o is ThreeDFloorTop3D t && ControlLinedef == t.ControlLinedef && Sector == t.Sector;
		public override int GetHashCode() => (ControlLinedef?.GetHashCode() ?? 0) + Sector.GetHashCode();

		public new static ThreeDFloorTop3D Deserialize(string s, MapSet map)
		{
			var parts = s.Split(' ');
			if (parts.Length != 2) return null;

			var controlline = MapElement3D.ParseMapElementByString(parts[0], map.Linedefs);
			if (controlline == null && parts[0] != "-1") return null;

			var sector = MapElement3D.ParseMapElementByString(parts[1], map.Sectors);
			if (sector == null) return null;

			return new ThreeDFloorTop3D(controlline, sector);
		}
	}

	public class ThreeDFloorSide3D : MapElement3D
	{
		public Linedef ControlLinedef { get; protected set; }
		public Sidedef Sidedef { get; protected set; }
		public override bool IsDisposed { get => ControlLinedef.IsDisposed || Sidedef.IsDisposed; }

		public ThreeDFloorSide3D(Linedef controllinedef, Sidedef sidedef)
		{
			ControlLinedef = controllinedef;
			Sidedef = sidedef;
		}

		public override bool Equals(object o) => o is ThreeDFloorSide3D s && ControlLinedef == s.ControlLinedef && Sidedef == s.Sidedef;
		public override int GetHashCode() => (ControlLinedef?.GetHashCode() ?? 0) + Sidedef.GetHashCode();
		public override string Serialize() => ControlLinedef.Index + " " + Sidedef.Index;

		public new static ThreeDFloorSide3D Deserialize(string s, MapSet map)
		{
			var parts = s.Split(' ');
			if (parts.Length != 2) return null;

			var controlline = MapElement3D.ParseMapElementByString(parts[0], map.Linedefs);
			if (controlline == null && parts[0] != "-1") return null;

			var side = MapElement3D.ParseMapElementByString(parts[1], map.Sidedefs);
			if (side == null) return null;

			return new ThreeDFloorSide3D(controlline, side);
		}
	}

	public abstract class Slope3D : MapElement3D
	{
		public Linedef ControlLinedef { get; protected set; }
		public abstract Sector Sector { get; protected set; }
		public bool IsFloor { get => !IsCeiling; }
		public bool IsCeiling { get; protected set; }
		public bool Is3DFloor { get => ControlLinedef != null; }

		protected Slope3D(bool ceiling, Linedef controllinedef = null)
		{
			IsCeiling = ceiling;
			ControlLinedef = controllinedef;
		}
	}

	public class SidedefSlope3D : Slope3D
	{
		public Sidedef Sidedef { get; protected set; }
		public override Sector Sector { get => Sidedef.Sector; protected set => Sector = value; }
		public override bool IsDisposed { get => Sidedef.IsDisposed || (ControlLinedef?.IsDisposed ?? false); }

		public SidedefSlope3D(Sidedef sidedef, bool ceiling, Linedef controllinedef = null) : base(ceiling, controllinedef)
		{
			Sidedef = sidedef;
		}

		public override bool Equals(object o) => o is SidedefSlope3D s && Sidedef == s.Sidedef && IsCeiling == s.IsCeiling && ControlLinedef == s.ControlLinedef;
		public override int GetHashCode() => Sidedef.GetHashCode() + IsCeiling.GetHashCode() + (ControlLinedef?.GetHashCode() ?? 0);
		public override string Serialize() => $"{(IsCeiling ? "c" : "f")} {Sidedef.Index} {ControlLinedef?.Index ?? -1}";

		public new static SidedefSlope3D Deserialize(string s, MapSet map)
		{
			var parts = s.Split(' ');
			if (parts.Length != 3) return null;

			bool ceiling = (parts[0] == "c");

			var side = MapElement3D.ParseMapElementByString(parts[1], map.Sidedefs);
			if (side == null) return null;

			var controlline = MapElement3D.ParseMapElementByString(parts[2], map.Linedefs);
			if (controlline == null && parts[2] != "-1") return null;

			return new SidedefSlope3D(side, ceiling, controlline);
		}
	}

	public class VertexSlope3D : Slope3D
	{
		public Vertex Vertex { get; protected set; }
		public override Sector Sector { get; protected set; }
		public override bool IsDisposed { get => Vertex.IsDisposed || Sector.IsDisposed || (ControlLinedef?.IsDisposed ?? false); }

		public VertexSlope3D(Vertex vertex, Sector sector, bool ceiling, Linedef controllinedef = null) : base(ceiling, controllinedef)
		{
			Vertex = vertex;
			Sector = sector;
		}

		public override bool Equals(object o) => o is VertexSlope3D s && Vertex == s.Vertex && Sector == s.Sector && IsFloor == s.IsFloor && ControlLinedef == s.ControlLinedef;
		public override int GetHashCode() => Vertex.GetHashCode() + IsFloor.GetHashCode() + (ControlLinedef?.GetHashCode() ?? 0);
		public override string Serialize() => $"{(IsCeiling ? "c" : "f")} {Vertex.Index} {Sector.Index} {ControlLinedef?.Index ?? -1}";

		public new static VertexSlope3D Deserialize(string s, MapSet map)
		{
			var parts = s.Split(' ');
			if (parts.Length != 4) return null;

			bool ceiling = (parts[0] == "c");

			var vertex = MapElement3D.ParseMapElementByString(parts[1], map.Vertices);
			if (vertex == null) return null;

			var sector = MapElement3D.ParseMapElementByString(parts[2], map.Sectors);
			if (sector == null) return null;

			var controlline = MapElement3D.ParseMapElementByString(parts[3], map.Linedefs);
			if (controlline == null && parts[3] != "-1") return null;

			return new VertexSlope3D(vertex, sector, ceiling, controlline);
		}
	}

	public class Group3D : List<MapElement3D> { }
}
