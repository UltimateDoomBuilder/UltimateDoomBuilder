#region ================== Copyright (c) 2014 Boris Iwanski

/*
 * Copyright (c) 2014 Boris Iwanski
 * This program is released under GNU General Public License
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 */

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using CodeImp.DoomBuilder.BuilderModes;
using CodeImp.DoomBuilder.Windows;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Types;

namespace CodeImp.DoomBuilder.ThreeDFloorMode
{
	public class ThreeDFloor
	{
		private Sector sector;
		private List<Sector> taggedsectors;
        private List<Sector> sectorstotag;
        private List<Sector> sectorstountag;
		private string bordertexture;
		private string topflat;
		private string bottomflat;
		private Vector3D floorslope;
		private double floorslopeoffset;
		private Vector3D ceilingslope;
		private double ceilingslopeoffset;
		private int type;
		private int flags;
		private int alpha;
		private int brightness;
		private int topheight;
		private int bottomheight;
		private bool isnew;
		private int udmftag;
		private List<int> tags;
		private LinedefProperties linedefproperties;
		private SectorProperties sectorproperties;

		public static Rectangle controlsectorarea = new Rectangle(-512, 512, 512, -512);

		public Sector Sector { get { return sector; } }
		public List<Sector> TaggedSectors { get { return taggedsectors; } set { taggedsectors = value; } }
        public List<Sector> SectorsToTag { get { return sectorstotag; } set { sectorstotag = value; } }
        public List<Sector> SectorsToUntag { get { return sectorstountag; } set { sectorstountag = value; } }
		public string BorderTexture { get { return bordertexture; } set { bordertexture = value; } }
		public string TopFlat { get { return topflat; } set { topflat = value; } }
		public string BottomFlat { get { return bottomflat; } set { bottomflat = value; } }
		public int Type { get { return type; } set { type = value; } }
		public int Flags { get { return flags; } set { flags = value; } }
		public int Alpha { get { return alpha; } set { alpha = value; } }
		public int Brightness { get { return brightness; }set { brightness = value; } }
		public int TopHeight { get { return topheight; } set { topheight = value; } }
		public int BottomHeight { get { return bottomheight; } set { bottomheight = value; } }
		public bool IsNew { get { return isnew; } set { isnew = value; } }
		public int UDMFTag { get { return udmftag; } set { udmftag = value; } }
		public List<int> Tags { get { return tags; } set { tags = value; } }
		public Vector3D FloorSlope {  get { return floorslope; } set { floorslope = value; } }
		public double FloorSlopeOffset { get { return floorslopeoffset; } set { floorslopeoffset = value; } }
		public Vector3D CeilingSlope { get { return ceilingslope; } set { ceilingslope = value; } }
		public double CeilingSlopeOffset { get { return ceilingslopeoffset; } set { ceilingslopeoffset = value; } }
		public LinedefProperties LinedefProperties { get { return linedefproperties; } }
		public SectorProperties SectorProperties { get { return sectorproperties; } }

		public ThreeDFloor()
		{
			sector = null;
			taggedsectors = new List<Sector>();
			topflat = General.Settings.DefaultCeilingTexture;
			bottomflat = General.Settings.DefaultFloorTexture;
			topheight = General.Settings.DefaultCeilingHeight;
			bottomheight = General.Settings.DefaultFloorHeight;
			bordertexture = General.Settings.DefaultTexture;
			type = 1;
			flags = 0;
			tags = new List<int>();
			floorslope = new Vector3D(0.0f, 0.0f, 0.0f);
			floorslopeoffset = 0.0f;
			ceilingslope = new Vector3D(0.0f, 0.0f, 0.0f);
			ceilingslopeoffset = 0.0f;

			linedefproperties = null;
			sectorproperties = null;

			alpha = 255;
		}

		public  ThreeDFloor(Sector sector) : this(sector, General.Map.Map.Sectors)
		{
			// Nothing extra do do here
		}

		public ThreeDFloor(Sector sector, IEnumerable<Sector> potentialsectors)
		{
			if (sector == null)
				throw new Exception("Sector can't be null");

			this.sector = sector;
			taggedsectors = new List<Sector>();
			topflat = sector.CeilTexture;
			bottomflat = sector.FloorTexture;
			topheight = sector.CeilHeight;
			bottomheight = sector.FloorHeight;
			brightness = sector.Brightness;
			tags = new List<int>();
			floorslope = sector.FloorSlope;
			floorslopeoffset = sector.FloorSlopeOffset;
			ceilingslope = sector.CeilSlope;
			ceilingslopeoffset = sector.CeilSlopeOffset;

			foreach (Sidedef sd in sector.Sidedefs)
			{
				if (sd.Line.Action == 160)
				{
					bordertexture = sd.MiddleTexture;
					udmftag = sd.Line.Args[0];
					type = sd.Line.Args[1];
					flags = sd.Line.Args[2];
					alpha = sd.Line.Args[3];
					linedefproperties = new LinedefProperties(sd.Line);
					sectorproperties = new SectorProperties(sector);

					foreach (Sector s in BuilderPlug.GetSectorsByTag(potentialsectors, sd.Line.Args[0]))
					{
						if(!taggedsectors.Contains(s))
							taggedsectors.Add(s);
					}
				}
			}
		}

		public void BindTag(int tag, LinedefProperties ldprops)
		{
			Linedef line = null;

			// try to find an line without an action
			foreach (Sidedef sd in sector.Sidedefs)
			{
				if (sd.Line.Action == 0 && sd.Line.Tag == 0 && line == null)
					line = sd.Line;

				// if a line of the control sector already has the tag
				// nothing has to be done
				if (sd.Line.Args[0] == tag)
				{
					return;
				}
			}

			// no lines without an action, so a line has to get split
			// find the longest line to split
			if (line == null)
			{
				line = sector.Sidedefs.First().Line;

				foreach (Sidedef sd in sector.Sidedefs)
				{
					if (sd.Line.Length > line.Length)
						line = sd.Line;
				}

				// Lines may not have a length of less than 1 after splitting
				if (line.Length / 2 < 1)
					throw new Exception("Can't split more lines in Sector " + line.Front.Sector.Index.ToString() + ".");

				Vertex v = General.Map.Map.CreateVertex(line.Line.GetCoordinatesAt(0.5f));
				v.SnapToAccuracy();

				line = line.Split(v);

				General.Map.Map.Update();
				General.Interface.RedrawDisplay();
			}

			if(ldprops != null)
				ldprops.Apply(new List<Linedef>() { line }, false);

			line.Action = 160;
			line.Args[0] = tag;
			line.Args[1] = type;
			line.Args[2] = flags;
			line.Args[3] = alpha;
		}

		public void UpdateGeometry()
		{
			if (sector == null)
				throw new Exception("3D floor has no geometry");

			sector.CeilHeight = topheight;
			sector.FloorHeight = bottomheight;
			sector.SetCeilTexture(topflat);
			sector.SetFloorTexture(bottomflat);
			sector.Brightness = brightness;
			sector.Tags = tags;
			sector.FloorSlope = floorslope;
			sector.FloorSlopeOffset = floorslopeoffset;
			sector.CeilSlope = ceilingslope;
			sector.CeilSlopeOffset = ceilingslopeoffset;

			foreach (Sidedef sd in sector.Sidedefs)
			{
				sd.SetTextureMid(bordertexture);

				if (sd.Line.Action == 160)
				{
					// We need to update the linedef's args, but we can't do it directly because otherwise their state will not be saved for the undo snapshot,
					// so we're using the linedef's update method
					sd.Line.Update(sd.Line.GetFlags(), sd.Line.RawFlags, sd.Line.Activate, sd.Line.Tags, sd.Line.Action, new int[] { sd.Line.Args[0], type, flags, alpha, sd.Line.Args[4] });
				}
			}
		}

		public bool CreateGeometry(List<int> tagblacklist, List<DrawnVertex> alldrawnvertices)
		{
			int newtag;

			return CreateGeometry(tagblacklist, alldrawnvertices, null, null, false, out newtag);
		}

		public bool CreateGeometry(List<int> tagblacklist, List<DrawnVertex> alldrawnvertices, LinedefProperties ldprops, SectorProperties sectorprops, bool forcenewtag, out int newtag)
		{
			List<Vertex> vertices = new List<Vertex>();
			Vector3D slopetopthingpos = new Vector3D(0, 0, 0);
			Vector3D slopebottomthingpos = new Vector3D(0, 0, 0);
			Line2D slopeline = new Line2D(0, 0, 0, 0);

			newtag = -1;

			// We need 5 vertices to draw the control sector
			if(alldrawnvertices.Count < 5)
			{
				General.Interface.DisplayStatus(StatusType.Warning, "Could not draw new sector: not enough vertices");
				return false;
			}

			// Get the first 5 vertices in the list and also remove them from the list, so that creating further
			// control sectors won't use them
			List<DrawnVertex> drawnvertices = alldrawnvertices.GetRange(0, 5);
			alldrawnvertices.RemoveRange(0, 5);

			// drawnvertices = BuilderPlug.Me.ControlSectorArea.GetNewControlSectorVertices();

			if (Tools.DrawLines(drawnvertices) == false)
			{
				General.Interface.DisplayStatus(StatusType.Warning, "Could not draw new sector");
				return false;
			}

			sector = General.Map.Map.GetMarkedSectors(true)[0];

			if (sectorprops != null)
				sectorprops.Apply(new List<Sector>() { sector }, false);

			sector.FloorHeight = bottomheight;
			sector.CeilHeight = topheight;
			sector.SetFloorTexture(bottomflat);
			sector.SetCeilTexture(topflat);
			sector.Brightness = brightness;
			sector.FloorSlope = floorslope;
			sector.FloorSlopeOffset = floorslopeoffset;
			sector.CeilSlope = ceilingslope;
			sector.CeilSlopeOffset = ceilingslopeoffset;

			foreach (Sidedef sd in sector.Sidedefs)
			{
				sd.Line.Front.SetTextureMid(bordertexture);
			}

			if (!sector.Fields.ContainsKey("user_managed_3d_floor"))
				sector.Fields.Add("user_managed_3d_floor", new UniValue(UniversalType.Boolean, true));

			sector.Fields["comment"] = new UniValue(UniversalType.String, "[!]DO NOT DELETE! This sector is managed by the 3D floor plugin.");

			// With multiple tag support in UDMF only one tag is needed, so bind it right away
			if (General.Map.UDMF == true && General.Map.Config.SectorMultiTag)
			{
				if (isnew || forcenewtag)
				{
					newtag = udmftag = BuilderPlug.Me.ControlSectorArea.GetNewSectorTag(tagblacklist);
					tagblacklist.Add(udmftag);
				}					

				BindTag(udmftag, ldprops);
			}

			return true;
		}

		public void Cleanup()
		{
			int taggedLines = 0;

			foreach (Sidedef sd in sector.Sidedefs)
			{
				if (sd.Line.Action == 160 && BuilderPlug.GetSectorsByTag(sd.Line.Args[0]).Count == 0)
				{
					sd.Line.Action = 0;

					for (int i = 0; i < 5; i++)
						sd.Line.Args[i] = 0;
				}

				if (sd.Line.Action != 0)
					taggedLines++;
			}

			if (taggedLines == 0)
			{
				DeleteControlSector(sector);
			}
		}

		private void DeleteControlSector(Sector sector)
		{
			if (sector == null)
				return;

			General.Map.Map.BeginAddRemove();

			// Get all the linedefs
			List<Linedef> lines = new List<Linedef>(sector.Sidedefs.Count);
			foreach (Sidedef side in sector.Sidedefs) lines.Add(side.Line);


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

					// Check textures.
					if (lines[i].Front.MiddleRequired() && (lines[i].Front.MiddleTexture.Length == 0 || lines[i].Front.MiddleTexture == "-"))
					{
						if (lines[i].Front.HighTexture.Length > 0 && lines[i].Front.HighTexture != "-")
						{
							lines[i].Front.SetTextureMid(lines[i].Front.HighTexture);
						}
						else if (lines[i].Front.LowTexture.Length > 0 && lines[i].Front.LowTexture != "-")
						{
							lines[i].Front.SetTextureMid(lines[i].Front.LowTexture);
						}
					}

					// Do we still need high/low textures?
					lines[i].Front.RemoveUnneededTextures(false);

					// Update sided flags
					lines[i].ApplySidedFlags();
				}
			}

			General.Map.Map.EndAddRemove();
		}

		public void DeleteControlSector()
		{
			DeleteControlSector(sector);
		}
	}
}
