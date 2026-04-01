
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
using System.Collections.Generic;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Geometry;
using System.Drawing;

#endregion

namespace CodeImp.DoomBuilder.VisualModes
{
    public sealed class VisualBlockMap
    {
		// This returns all blocks along the given line
		public List<VisualBlockEntry> GetLineBlocks(Vector2D v1, Vector2D v2)
        {
            int x0 = (int)Math.Floor(Math.Min(v1.x, v2.x));
            int y0 = (int)Math.Floor(Math.Min(v1.y, v2.y));
            int x1 = (int)Math.Floor(Math.Max(v1.x, v2.x)) + 1;
            int y1 = (int)Math.Floor(Math.Max(v1.y, v2.y)) + 1;

            var result = new List<VisualBlockEntry>();
            root.GetBlocks(new Rectangle(x0, y0, x1 - x0, y1 - y0), ref result);
            return result;
        }

        public List<VisualBlockEntry> GetBlocks(RectangleF box)
        {
            var result = new List<VisualBlockEntry>();
            root.GetBlocks(ToRectangle(box), ref result);
            return result;
        }

        public List<VisualBlockEntry> GetBlocks(Vector2D pos)
        {
            var result = new List<VisualBlockEntry>();
            root.GetBlocks(new Point((int)Math.Floor(pos.x), (int)Math.Floor(pos.y)), ref result);
            return result;
        }

        // This returns a range of blocks in a frustum
        public List<VisualBlockEntry> GetFrustumRange(ProjectedFrustum2D frustum2D)
        {
            var frustum = new Frustum();
            frustum.planes = new Plane[4]
            {
                new Plane(frustum2D.Lines[0]),
                new Plane(frustum2D.Lines[1]),
                new Plane(frustum2D.Lines[2]),
                new Plane(frustum2D.Lines[3])
            };

            var result = new List<VisualBlockEntry>();
            root.GetBlocks(frustum, ref result);
            return result;
        }

        public Sector GetSectorAt(Vector2D pos)
        {
			List<Sector> sectors = new List<Sector>(1);

			foreach (VisualBlockEntry e in GetBlocks(pos))
                foreach (Sector s in e.Sectors)
                    if (s.Intersect(pos))
						sectors.Add(s);

			if (sectors.Count == 0)
			{
				return null;
			}
			else if (sectors.Count == 1)
			{
				return sectors[0];
			}
			else
			{
				// Having multiple intersections indicates that there are self-referencing sectors in this spot.
				// In this case we have to check which side of the nearest linedef pos is on, and then use that sector
				HashSet<Linedef> linedefs = new HashSet<Linedef>(sectors[0].Sidedefs.Count * sectors.Count);

				foreach (Sector s in sectors)
					foreach (Sidedef sd in s.Sidedefs)
						linedefs.Add(sd.Line);

				Linedef nearest = MapSet.NearestLinedef(linedefs, pos);
				double d = nearest.SideOfLine(pos);

				if (d <= 0.0 && nearest.Front != null)
					return nearest.Front.Sector;
				else if (nearest.Back != null)
					return nearest.Back.Sector;
			}

            return null;
        }

        public void Clear()
        {
            root = new Node(new Rectangle(General.Map.Config.LeftBoundary, General.Map.Config.BottomBoundary, General.Map.Config.RightBoundary - General.Map.Config.LeftBoundary, General.Map.Config.TopBoundary - General.Map.Config.BottomBoundary));
        }

		public void AddSectorsSet(ICollection<Sector> sectors)
		{
            foreach (Sector s in sectors) AddSector(s);
		}

        public void AddLinedefsSet(ICollection<Linedef> lines)
        {
            foreach (Linedef line in lines) AddLinedef(line);
        }

        public void AddThingsSet(ICollection<Thing> things)
        {
            foreach (Thing t in things) AddThing(t);
        }

        public void AddSector(Sector sector)
        {
            root.GetEntry(ToRectangle(sector.BBox)).Sectors.Add(sector);
        }

        public void AddLinedef(Linedef line)
        {
            int x0 = (int)Math.Floor(Math.Min(line.Start.Position.x, line.End.Position.x));
            int y0 = (int)Math.Floor(Math.Min(line.Start.Position.y, line.End.Position.y));
            int x1 = (int)Math.Floor(Math.Max(line.Start.Position.x, line.End.Position.x)) + 1;
            int y1 = (int)Math.Floor(Math.Max(line.Start.Position.y, line.End.Position.y)) + 1;
            root.GetEntry(new Rectangle(x0, y0, x1 - x0, y1 - y0)).Lines.Add(line);
        }

        public void AddThing(Thing thing)
        {
            int x0 = (int)Math.Floor(thing.Position.x - thing.Size);
            int x1 = (int)Math.Floor(thing.Position.x + thing.Size) + 1;
            int y0 = (int)Math.Floor(thing.Position.y - thing.Size);
            int y1 = (int)Math.Floor(thing.Position.y + thing.Size) + 1;
            root.GetEntry(new Rectangle(x0, y0, x1 - x0, y1 - y0)).Things.Add(thing);
        }

        internal void Dispose()
		{
            Clear();
		}

        static Rectangle ToRectangle(RectangleF bbox)
        {
            int x0 = (int)Math.Floor(bbox.Left);
            int y0 = (int)Math.Floor(bbox.Top);
            int x1 = (int)Math.Floor(bbox.Right) + 1;
            int y1 = (int)Math.Floor(bbox.Bottom) + 1;
            return new Rectangle(x0, y0, x1 - x0, y1 - y0);
        }

        const int MaxLevels = 8;

        Node root = new Node(new Rectangle(General.Map.Config.LeftBoundary, General.Map.Config.BottomBoundary, General.Map.Config.RightBoundary - General.Map.Config.LeftBoundary, General.Map.Config.TopBoundary - General.Map.Config.BottomBoundary));

        struct Plane
        {
            public Plane(Line2D line)
            {
                Vector2D dir = line.v2 - line.v1;
                A = -dir.y;
                B = dir.x;
                D = -(line.v1.x * A + line.v1.y * B);
            }

            public double A, B, D;
        }

        class Frustum
        {
            public Plane[] planes;
        }

        class Node
        {
            enum Visibility { Inside, Intersecting, Outside };

            public Node(Rectangle bbox)
            {
                this.bbox = bbox;
                extents = new Vector2D(bbox.Width * 0.5f, bbox.Height * 0.5f);
                center = new Vector2D(bbox.X + extents.x, bbox.Y + extents.y);
            }

            public void GetBlocks(Frustum frustum, ref List<VisualBlockEntry> list)
            {
                Visibility vis = TestVisibility(frustum);
                if (vis == Visibility.Inside)
                {
                    GetAllBlocks(ref list);
                }
                else if (vis == Visibility.Intersecting)
                {
                    if (visualBlock != null)
                        list.Add(visualBlock);

                    if (topLeft != null)
                    {
                        topLeft.GetBlocks(frustum, ref list);
                        topRight.GetBlocks(frustum, ref list);
                        bottomLeft.GetBlocks(frustum, ref list);
                        bottomRight.GetBlocks(frustum, ref list);
                    }
                }
            }

            void GetAllBlocks(ref List<VisualBlockEntry> list)
            {
                if (visualBlock != null)
                    list.Add(visualBlock);

                if (topLeft != null)
                {
                    topLeft.GetAllBlocks(ref list);
                    topRight.GetAllBlocks(ref list);
                    bottomLeft.GetAllBlocks(ref list);
                    bottomRight.GetAllBlocks(ref list);
                }
            }

            Visibility TestVisibility(Frustum frustum)
            {
                Visibility result = Visibility.Inside;
                for (int i = 0; i < 4; i++)
                {
                    Visibility vis = TestFrustumLineVisibility(frustum.planes[i]);
                    if (vis == Visibility.Outside)
                        return Visibility.Outside;
                    else if (vis == Visibility.Intersecting)
                        result = Visibility.Intersecting;
                }
                return result;
            }

            Visibility TestFrustumLineVisibility(Plane plane)
            {
				double e = extents.x * Math.Abs(plane.A) + extents.y * Math.Abs(plane.B);
				double s = center.x * plane.A + center.y * plane.B + plane.D;
                if (s - e > 0.0)
                    return Visibility.Inside;
                else if (s + e < 0)
                    return Visibility.Outside;
                else
                    return Visibility.Intersecting;
            }

            public void GetBlocks(Point pos, ref List<VisualBlockEntry> list)
            {
                if (visualBlock != null)
                    list.Add(visualBlock);

                if (topLeft != null)
                {
                    if (topLeft.bbox.Contains(pos)) topLeft.GetBlocks(pos, ref list);
                    if (topRight.bbox.Contains(pos)) topRight.GetBlocks(pos, ref list);
                    if (bottomLeft.bbox.Contains(pos)) bottomLeft.GetBlocks(pos, ref list);
                    if (bottomRight.bbox.Contains(pos)) bottomRight.GetBlocks(pos, ref list);
                }
            }

            public void GetBlocks(Rectangle box, ref List<VisualBlockEntry> list)
            {
                if (visualBlock != null)
                    list.Add(visualBlock);

                if (topLeft != null)
                {
                    if (topLeft.bbox.IntersectsWith(box)) topLeft.GetBlocks(box, ref list);
                    if (topRight.bbox.IntersectsWith(box)) topRight.GetBlocks(box, ref list);
                    if (bottomLeft.bbox.IntersectsWith(box)) bottomLeft.GetBlocks(box, ref list);
                    if (bottomRight.bbox.IntersectsWith(box)) bottomRight.GetBlocks(box, ref list);
                }
            }

            public VisualBlockEntry GetEntry(Rectangle box, int level = 0)
            {
                if (level == MaxLevels)
                {
                    if (visualBlock == null)
                        visualBlock = new VisualBlockEntry();
                    return visualBlock;
                }

                if (topLeft == null)
                    CreateChildren();

                if (topLeft.bbox.Contains(box)) return topLeft.GetEntry(box, level + 1);
                if (topRight.bbox.Contains(box)) return topRight.GetEntry(box, level + 1);
                if (bottomLeft.bbox.Contains(box)) return bottomLeft.GetEntry(box, level + 1);
                if (bottomRight.bbox.Contains(box)) return bottomRight.GetEntry(box, level + 1);

                if (visualBlock == null)
                    visualBlock = new VisualBlockEntry();
                return visualBlock;
            }

            void CreateChildren()
            {
                int x0 = bbox.X;
                int x1 = bbox.X + bbox.Width / 2;
                int x2 = bbox.X + bbox.Width;
                int y0 = bbox.Y;
                int y1 = bbox.Y + bbox.Height / 2;
                int y2 = bbox.Y + bbox.Height;
                topLeft = new Node(new Rectangle(x0, y0, x1 - x0, y1 - y0));
                topRight = new Node(new Rectangle(x1, y0, x2 - x1, y1 - y0));
                bottomLeft = new Node(new Rectangle(x0, y1, x1 - x0, y2 - y1));
                bottomRight = new Node(new Rectangle(x1, y1, x2 - x1, y2 - y1));
            }

            Rectangle bbox;
            Vector2D extents;
            Vector2D center;

            Node topLeft;
            Node topRight;
            Node bottomLeft;
            Node bottomRight;
            VisualBlockEntry visualBlock;
        }
    }
}
