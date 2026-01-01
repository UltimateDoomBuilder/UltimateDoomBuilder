using System;
using System.Collections.Generic;

using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Geometry;

namespace CodeImp.DoomBuilder.ThreeDFloorMode
{
	class SectorLabelInfo
	{
		public bool Floor { get { return (planetypes & PlaneType.Floor) == PlaneType.Floor; } }
		public bool Ceiling { get { return (planetypes & PlaneType.Ceiling) == PlaneType.Ceiling; } }
		public bool Bottom { get { return (planetypes & PlaneType.Bottom) == PlaneType.Bottom; } }
		public bool Top { get { return (planetypes & PlaneType.Top) == PlaneType.Top; } }

		private PlaneType planetypes;
		private Dictionary<PlaneType, List<SlopeVertexGroup>> slopevertexgroups;

		public SectorLabelInfo()
		{
			planetypes = 0;
			slopevertexgroups = new Dictionary<PlaneType, List<SlopeVertexGroup>>();

			foreach (PlaneType pt in Enum.GetValues(typeof(PlaneType)))
				slopevertexgroups.Add(pt, new List<SlopeVertexGroup>());
		}

		public void AddSlopeVertexGroup(PlaneType pt, SlopeVertexGroup svg)
		{
			if (!slopevertexgroups[pt].Contains(svg))
				slopevertexgroups[pt].Add(svg);

			planetypes |= pt;
		}

		public TextLabel[] CreateLabels(Sector sector, SlopeVertex slopevertex, float scale)
		{
			int numlabels = 0;
			int loopcounter = 0;
			PixelColor white = new PixelColor(255, 255, 255, 255);

			// Count how many actual labels we have to create at each label position
			foreach (PlaneType pt in Enum.GetValues(typeof(PlaneType)))
				if ((planetypes & pt) == pt)
					numlabels++;

			// Split sectors can have multiple label positions, so we have to create
			// numlabels labels for each position
			TextLabel[] labels = new TextLabel[sector.Labels.Count * numlabels];

			foreach (PlaneType pt in Enum.GetValues(typeof(PlaneType)))
			{
				if ((planetypes & pt) != pt)
					continue;

				bool highlighted = false;

				foreach (SlopeVertexGroup svg in slopevertexgroups[pt])
					if (svg.Vertices.Contains(slopevertex))
						highlighted = true;

				for (int i = 0; i < sector.Labels.Count; i++)
				{
					int apos = sector.Labels.Count * loopcounter + i;
					Vector2D location = sector.Labels[i].position;

					location.x += ((-6 * (numlabels-1)) + (loopcounter * 12)) * (1/scale);

					labels[apos] = new TextLabel();
					labels[apos].TransformCoords = true;
					labels[apos].AlignX = TextAlignmentX.Center;
					labels[apos].AlignY = TextAlignmentY.Middle;
					labels[apos].BackColor = General.Colors.Background.WithAlpha(128);
					labels[apos].Location = location;

					if (highlighted)
						labels[apos].Color = General.Colors.Highlight.WithAlpha(255);
					else
						labels[apos].Color = white;

					if (pt == PlaneType.Floor)
						labels[apos].Text = "F";
					else if (pt == PlaneType.Ceiling)
						labels[apos].Text = "C";
					else if (pt == PlaneType.Bottom)
						labels[apos].Text = "B";
					else if (pt == PlaneType.Top)
						labels[apos].Text = "T";
				}

				loopcounter++;
			}

			return labels;
		}

	}
}
