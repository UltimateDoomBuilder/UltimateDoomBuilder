using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Map;

namespace CodeImp.DoomBuilder.ThreeDFloorMode
{
	public partial class SlopeVertexEditForm : Form
	{
		private List<SlopeVertex> vertices;
		private List<Sector> sectors;
		private string undodescription;
		private bool canaddsectors;
		private bool canremovesectors;

		public SlopeVertexEditForm()
		{
			InitializeComponent();
		}

		public void Setup(List<SlopeVertex> vertices)
		{
			this.vertices = vertices;

			SlopeVertex fv = vertices[0];
			SlopeVertexGroup fsvg = BuilderPlug.Me.GetSlopeVertexGroup(fv);

			sectors = new List<Sector>();

			undodescription = "Edit slope vertex";

			if (vertices.Count > 1)
				undodescription = "Edit " + vertices.Count + " slope vertices";
			
			positionx.Text = fv.Pos.x.ToString();
			positiony.Text = fv.Pos.y.ToString();
			positionz.Text = fv.Z.ToString();

			foreach (Sector s in fsvg.Sectors)
				if (!sectors.Contains(s))
					sectors.Add(s);

			reposition.Checked = fsvg.Reposition;
			spline.Checked = fsvg.Spline;

			canaddsectors = true;
			canremovesectors = true;

			if (vertices.Count > 1)
			{
				List<SlopeVertexGroup> listsvgs = new List<SlopeVertexGroup>();

				this.Text = "Edit slope vertices (" + vertices.Count.ToString() + ")";

				foreach (SlopeVertex sv in vertices)
				{
					SlopeVertexGroup svg = BuilderPlug.Me.GetSlopeVertexGroup(sv);

					if (!listsvgs.Contains(svg))
						listsvgs.Add(svg);

					if (sv.Pos.x.ToString() != positionx.Text)
						positionx.Text = "";

					if (sv.Pos.y.ToString() != positiony.Text)
						positiony.Text = "";

					if (sv.Z.ToString() != positionz.Text)
						positionz.Text = "";

					if (svg.Reposition != reposition.Checked)
						reposition.CheckState = CheckState.Indeterminate;

					if (svg.Spline)
						spline.Enabled = true;

					if (svg.Spline != spline.Checked)
						spline.CheckState = CheckState.Indeterminate;

					foreach (Sector s in svg.Sectors)
						if (!sectors.Contains(s))
							sectors.Add(s);
				}

				if (listsvgs.Count > 2)
				{
					canaddsectors = false;
					canremovesectors = false;
				}
			}

			foreach (Sector s in sectors.OrderBy(x => x.Index))
			{
				checkedListBoxSectors.Items.Add(s);
			}

			if (General.Map.Map.SelectedSectorsCount == 0)
			{
				addselectedsectorsceiling.Enabled = false;
				removeselectedsectorsceiling.Enabled = false;
				addselectedsectorsfloor.Enabled = false;
				removeselectedsectorsfloor.Enabled = false;
			}
			else
			{
				addselectedsectorsceiling.Enabled = canaddsectors;
				removeselectedsectorsceiling.Enabled = canremovesectors;
				addselectedsectorsfloor.Enabled = canaddsectors;
				removeselectedsectorsfloor.Enabled = canremovesectors;
			}
		}

		private void apply_Click(object sender, EventArgs e)
		{
			List<SlopeVertexGroup> groups = new List<SlopeVertexGroup>();

			// undodescription was set in the Setup method
			General.Map.UndoRedo.CreateUndo(undodescription);

			foreach (SlopeVertex sv in vertices)
			{
				SlopeVertexGroup svg = BuilderPlug.Me.GetSlopeVertexGroup(sv);
				double x = positionx.GetResultFloat(sv.Pos.x);
				double y = positiony.GetResultFloat(sv.Pos.y);

				sv.Pos = new Vector2D(x, y);

				sv.Z = positionz.GetResultFloat(sv.Z);

				if (!groups.Contains(svg))
					groups.Add(svg);
			}

			foreach (SlopeVertexGroup svg in groups)
			{
				if (reposition.CheckState != CheckState.Indeterminate)
					svg.Reposition = reposition.Checked;

				if (spline.CheckState != CheckState.Indeterminate && svg.Vertices.Count == 3)
					svg.Spline = spline.Checked;

				// Ceiling
				if (addselectedsectorsceiling.Checked)
					foreach (Sector s in General.Map.Map.GetSelectedSectors(true).ToList())
						svg.AddSector(s, PlaneType.Ceiling);

				if (removeselectedsectorsceiling.Checked)
					foreach (Sector s in General.Map.Map.GetSelectedSectors(true).ToList())
						if (svg.Sectors.Contains(s))
							svg.RemoveSector(s, PlaneType.Ceiling);

				// Floor
				if (addselectedsectorsfloor.Checked)
					foreach (Sector s in General.Map.Map.GetSelectedSectors(true).ToList())
						svg.AddSector(s, PlaneType.Floor);

				if (removeselectedsectorsfloor.Checked)
					foreach (Sector s in General.Map.Map.GetSelectedSectors(true).ToList())
						if (svg.Sectors.Contains(s))
							svg.RemoveSector(s, PlaneType.Floor);

				foreach (Sector s in checkedListBoxSectors.CheckedItems)
				{
					if (svg.Sectors.Contains(s))
					{
						svg.RemoveSector(s, PlaneType.Floor);
						svg.RemoveSector(s, PlaneType.Ceiling);
					}
				}
					
				svg.ApplyToSectors();
			}

			BuilderPlug.Me.StoreSlopeVertexGroupsInSector();

			this.DialogResult = DialogResult.OK;
		}

		private void cancel_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;
		}

		private void addselectedsectorsceiling_CheckedChanged(object sender, EventArgs e)
		{
			// Adding and removing selected sectors at the same time doesn't make sense,
			// so make sure only one of the checkboxes is checked at most
			if (addselectedsectorsceiling.Checked)
				removeselectedsectorsceiling.Checked = false;
		}

		private void removeselectedsectorsceiling_CheckedChanged(object sender, EventArgs e)
		{
			// Adding and removing selected sectors at the same time doesn't make sense,
			// so make sure only one of the checkboxes is checked at most
			if (removeselectedsectorsceiling.Checked)
				addselectedsectorsceiling.Checked = false;
		}

		private void addselectedsectorsfloor_CheckedChanged(object sender, EventArgs e)
		{
			// Adding and removing selected sectors at the same time doesn't make sense,
			// so make sure only one of the checkboxes is checked at most
			if (addselectedsectorsfloor.Checked)
				removeselectedsectorsfloor.Checked = false;
		}

		private void removeselectedsectorsfloor_CheckedChanged(object sender, EventArgs e)
		{
			// Adding and removing selected sectors at the same time doesn't make sense,
			// so make sure only one of the checkboxes is checked at most
			if (removeselectedsectorsfloor.Checked)
				addselectedsectorsfloor.Checked = false;
		}
	}
}
