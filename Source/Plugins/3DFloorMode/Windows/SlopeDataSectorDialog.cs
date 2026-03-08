using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Geometry;

namespace CodeImp.DoomBuilder.ThreeDFloorMode
{
	public partial class SlopeDataSectorDialog : Form
	{
		public SlopeDataSectorDialog()
		{
			InitializeComponent();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			if (General.Map.Map.SelectedSectorsCount == 0)
				MessageBox.Show("No sectors selected. Please select exactly one sector", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			else if (General.Map.Map.SelectedSectorsCount > 1)
				MessageBox.Show(General.Map.Map.SelectedSectorsCount.ToString() + " sectors selected. Please select exactly one sector", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			else
			{
				General.Map.Map.ClearAllMarks(false);
				General.Map.Map.GetSelectedSectors(true).First().Marked = true;

				this.DialogResult = DialogResult.OK;
				this.Close();
			}
		}

		private void SlopeDataSectorDialog_Load(object sender, EventArgs e)
		{
			webBrowser1.DocumentText = string.Format(System.Globalization.CultureInfo.GetCultureInfo("en-US"), @"
<style>
	body {{ background-color:rgb({0},{1},{2}); font-family:{3}; font-size:12 }}
</style>

The map does not contain a slope data sector. This sector is required by the slope mode to store data for the slope vertex groups.

You have two options:
<ul>
<li>Use selected sector: uses the currently selected sector to store the slope data. <span style='color:red'>Make sure that this sector is only
used for this purpose! Do not edit or delete it!</span></li>
<li>Create sector in CSA: automatically creates a new sector in the control sector area. <span style='color:red'>Do not edit or delete it!</span></li>
</ul>

<b>Creating the sector in the CSA is the recommended method to use.</b>", this.BackColor.R, this.BackColor.G, this.BackColor.B, this.Font.Name);
		}

		private void createnewsector_Click(object sender, EventArgs e)
		{
			List<DrawnVertex> drawnvertices = new List<DrawnVertex>();

			try
			{
				drawnvertices = BuilderPlug.Me.ControlSectorArea.GetNewControlSectorVertices();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			General.Map.Map.ClearAllMarks(false);

			// DrawLines automatically marks the new sector, so we don't have to do it manually
			if (Tools.DrawLines(drawnvertices) == false)
			{
				MessageBox.Show("Could not draw new sector", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			// Update textures
			General.Map.Data.UpdateUsedTextures();

			// Update caches
			General.Map.Map.Update();
			General.Interface.RedrawDisplay();
			General.Map.IsChanged = true;

			this.DialogResult = DialogResult.OK;
			this.Close();
		}
	}
}
