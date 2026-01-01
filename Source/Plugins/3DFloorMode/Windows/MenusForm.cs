using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Controls;

namespace CodeImp.DoomBuilder.ThreeDFloorMode
{
	public partial class MenusForm : Form
	{
		public ToolStripButton FloorSlope { get { return floorslope; } }
		public ToolStripButton CeilingSlope { get { return ceilingslope; } }
		public ToolStripButton FloorAndCeilingSlope { get { return floorandceilingslope; } }
		public ToolStripButton UpdateSlopes { get { return updateslopes; } }
		public ToolStripActionButton RelocateControlSectors { get { return relocatecontrolsectors; } }
		public ContextMenuStrip AddSectorsContextMenu { get { return addsectorscontextmenu; } }

		public MenusForm()
		{
			InitializeComponent();
		}

		public void UpdateToolTips()
		{
			relocatecontrolsectors.UpdateToolTip();
		}

		private void InvokeTaggedAction(object sender, EventArgs e)
		{
			General.Interface.InvokeTaggedAction(sender, e);
		}

		private void floorslope_Click(object sender, EventArgs e)
		{
			if (floorslope.Checked)
				return;

			General.Interface.InvokeTaggedAction(sender, e);
		}

		private void ceilingslope_Click(object sender, EventArgs e)
		{
			if (ceilingslope.Checked)
				return;

			General.Interface.InvokeTaggedAction(sender, e);
		}

		private void floorandceilingslope_Click(object sender, EventArgs e)
		{
			if (floorandceilingslope.Checked)
				return;

			General.Interface.InvokeTaggedAction(sender, e);
		}

		private void toolStripButton1_Click(object sender, EventArgs e)
		{
			BuilderPlug.Me.UpdateSlopes();
		}

		private void floorToolStripMenuItem_Click(object sender, EventArgs e)
		{
			List<SlopeVertexGroup> svgs = ((SlopeMode)General.Editing.Mode).GetSelectedSlopeVertexGroups();

			// Can only add sectors to one slope vertex group
			if (svgs.Count != 1)
				return;

			foreach (Sector s in (List<Sector>)addsectorscontextmenu.Tag)
			{
				SlopeVertexGroup rsvg = BuilderPlug.Me.GetSlopeVertexGroup(s);

				if (rsvg != null)
					rsvg.RemoveSector(s, PlaneType.Floor);

				svgs[0].AddSector(s, PlaneType.Floor);
				BuilderPlug.Me.UpdateSlopes(s);
			}

			General.Interface.RedrawDisplay();
		}

		private void removeSlopeFromFloorToolStripMenuItem_Click(object sender, EventArgs e)
		{
			foreach (Sector s in (List<Sector>)addsectorscontextmenu.Tag)
			{
				SlopeVertexGroup svg = BuilderPlug.Me.GetSlopeVertexGroup(s);

				if (svg != null)
					svg.RemoveSector(s, PlaneType.Floor);
			}

			General.Interface.RedrawDisplay();
		}

		private void ceilingToolStripMenuItem_Click(object sender, EventArgs e)
		{
			List<SlopeVertexGroup> svgs = ((SlopeMode)General.Editing.Mode).GetSelectedSlopeVertexGroups();

			// Can only add sectors to one slope vertex group
			if (svgs.Count != 1)
				return;

			foreach (Sector s in (List<Sector>)addsectorscontextmenu.Tag)
			{
				SlopeVertexGroup rsvg = BuilderPlug.Me.GetSlopeVertexGroup(s);

				if (rsvg != null)
					rsvg.RemoveSector(s, PlaneType.Ceiling);

				svgs[0].AddSector(s, PlaneType.Ceiling);
				BuilderPlug.Me.UpdateSlopes(s);
			}

			General.Interface.RedrawDisplay();
		}

		private void removeSlopeFromCeilingToolStripMenuItem_Click(object sender, EventArgs e)
		{
			foreach (Sector s in (List<Sector>)addsectorscontextmenu.Tag)
			{
				SlopeVertexGroup svg = BuilderPlug.Me.GetSlopeVertexGroup(s);

				if(svg != null)
					svg.RemoveSector(s, PlaneType.Ceiling);
			}

			General.Interface.RedrawDisplay();
		}

		private void addsectorscontextmenu_Opening(object sender, CancelEventArgs e)
		{
			// Disable adding if more than one slope vertex group is selected,
			// otherwise enable adding
			List<SlopeVertexGroup> svgs = ((SlopeMode)General.Editing.Mode).GetSelectedSlopeVertexGroups();

			addslopefloor.Enabled = svgs.Count == 1;
			addslopeceiling.Enabled = svgs.Count == 1;
		}

		private void addsectorscontextmenu_Closing(object sender, ToolStripDropDownClosingEventArgs e)
		{
			if(	e.CloseReason != ToolStripDropDownCloseReason.ItemClicked &&
				e.CloseReason != ToolStripDropDownCloseReason.Keyboard &&
				e.CloseReason != ToolStripDropDownCloseReason.AppFocusChange)
				((SlopeMode)General.Editing.Mode).ContextMenuClosing = true;
		}

		private void relocatecontrolsectors_Click(object sender, EventArgs e)
		{
			General.Interface.InvokeTaggedAction(sender, e);
		}
	}
}
