using System;
using System.Windows.Forms;

namespace CodeImp.DoomBuilder.ThreeDFloorMode
{
	public partial class ControlSectorAreaConfig : Form
	{
		private ControlSectorArea csa;

		public ControlSectorAreaConfig(ControlSectorArea csa)
		{
			this.csa = csa;

			InitializeComponent();

			useTagRange.Checked = csa.UseCustomTagRnage;
			firstTag.Text = csa.FirstTag.ToString();
			lastTag.Text = csa.LastTag.ToString();
		}

		private void useTagRange_CheckedChanged(object sender, EventArgs e)
		{
			if (useTagRange.Checked)
			{
				firstTag.Enabled = true;
				lastTag.Enabled = true;
			}
			else
			{
				firstTag.Enabled = false;
				lastTag.Enabled = false;
			}
		}

		private void cancelButton_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;
			this.Close();
		}

		private void okButton_Click(object sender, EventArgs e)
		{
			if (useTagRange.Checked && int.Parse(lastTag.Text) < int.Parse(firstTag.Text))
			{
				MessageBox.Show("Last tag of range must be bigger than first tag of range", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			csa.UseCustomTagRnage = useTagRange.Checked;

			if (useTagRange.Checked)
			{
				csa.FirstTag = int.Parse(firstTag.Text);
				csa.LastTag = int.Parse(lastTag.Text);
			}

			this.DialogResult = DialogResult.OK;
			this.Close();
		}
	}
}
