using System;
using System.Collections.Generic;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using CodeImp.DoomBuilder.Map;

namespace CodeImp.DoomBuilder.ThreeDFloorMode
{
	public partial class ThreeDFloorHelperControl : UserControl
	{
		private ThreeDFloor threeDFloor;
		public Linedef linedef;
		private bool isnew;
		private Sector sector;
		private List<int> checkedsectors;
		private bool used;

		public ThreeDFloor ThreeDFloor { get { return threeDFloor; } }
		public bool IsNew { get { return isnew; } }
		public Sector Sector { get { return sector; } }
		public List<int> CheckedSectors { get { return checkedsectors; } }
		public bool Used { get { return used; } set { used = value; } }

		// Create the control from an existing linedef
		public ThreeDFloorHelperControl(ThreeDFloor threeDFloor)
		{
			InitializeComponent();

			sectorTopFlat.Initialize();
			sectorBorderTexture.Initialize();
			sectorBottomFlat.Initialize();

			Update(threeDFloor);
		}

		// Create a duplicate of the given control
		public ThreeDFloorHelperControl(ThreeDFloorHelperControl ctrl) : this()
		{
			Update(ctrl);
		}

		// Create a blank control for a new 3D floor
		public ThreeDFloorHelperControl()
		{
			InitializeComponent();

			sectorTopFlat.Initialize();
			sectorBorderTexture.Initialize();
			sectorBottomFlat.Initialize();

			SetDefaults();
		}
		
		public void SetDefaults()
		{
			isnew = true;

			threeDFloor = new ThreeDFloor();

			sectorBorderTexture.TextureName = General.Settings.DefaultTexture;
			sectorTopFlat.TextureName = General.Settings.DefaultCeilingTexture;
			sectorBottomFlat.TextureName = General.Settings.DefaultFloorTexture;
			sectorCeilingHeight.Text = General.Settings.DefaultCeilingHeight.ToString();
			sectorFloorHeight.Text = General.Settings.DefaultFloorHeight.ToString();

			typeArgument.Setup(General.Map.Config.LinedefActions[160].Args[1]);
			flagsArgument.Setup(General.Map.Config.LinedefActions[160].Args[2]);
			alphaArgument.Setup(General.Map.Config.LinedefActions[160].Args[3]);

			typeArgument.SetDefaultValue();
			flagsArgument.SetDefaultValue();
			alphaArgument.SetDefaultValue();

			tagsLabel.Text = "0";

			AddSectorCheckboxes();

			for (int i = 0; i < checkedListBoxSectors.Items.Count; i++)
				checkedListBoxSectors.SetItemChecked(i, true);

			//When creating a NEW 3d sector, find information about what is selected to populate the defaults
			int FloorHeight = int.MinValue;
			int SectorDarkest = int.MaxValue;
			foreach (Sector s in BuilderPlug.TDFEW.SelectedSectors)
			{
				if (s.FloorHeight > FloorHeight)
					FloorHeight = s.FloorHeight;
				if (s.Brightness < SectorDarkest)
					SectorDarkest = s.Brightness;
			}

			//set the floor height to match the lowest sector selected, then offset the height by the configured default
			if (FloorHeight != int.MinValue)
			{
				int DefaultHeight = General.Settings.DefaultCeilingHeight - General.Settings.DefaultFloorHeight;
				sectorFloorHeight.Text = FloorHeight.ToString();
				sectorCeilingHeight.Text = (FloorHeight + DefaultHeight).ToString();
			}

			//set the brightness to match the darkest of all the selected sectors by default
			if (SectorDarkest != int.MaxValue) {
				sectorBrightness.Text = SectorDarkest.ToString();
			} else {
				sectorBrightness.Text = General.Settings.DefaultBrightness.ToString();
			}

			sector = General.Map.Map.CreateSector();
		}

		public void Update(ThreeDFloorHelperControl ctrl)
		{
			sectorBorderTexture.TextureName = threeDFloor.BorderTexture = ctrl.threeDFloor.BorderTexture;
			sectorTopFlat.TextureName = threeDFloor.TopFlat = ctrl.threeDFloor.TopFlat;
			sectorBottomFlat.TextureName = threeDFloor.BottomFlat = ctrl.threeDFloor.BottomFlat;
			sectorCeilingHeight.Text = ctrl.threeDFloor.TopHeight.ToString();
			sectorFloorHeight.Text = ctrl.threeDFloor.BottomHeight.ToString();
			borderHeightLabel.Text = (ctrl.threeDFloor.TopHeight - ctrl.threeDFloor.BottomHeight).ToString();

			threeDFloor.TopHeight = ctrl.threeDFloor.TopHeight;
			threeDFloor.BottomHeight = ctrl.threeDFloor.BottomHeight;

			typeArgument.SetValue(ctrl.threeDFloor.Type);
			flagsArgument.SetValue(ctrl.threeDFloor.Flags);
			alphaArgument.SetValue(ctrl.threeDFloor.Alpha);
			sectorBrightness.Text = ctrl.threeDFloor.Brightness.ToString();

			threeDFloor.FloorSlope = ctrl.ThreeDFloor.FloorSlope;
			threeDFloor.FloorSlopeOffset = ctrl.ThreeDFloor.FloorSlopeOffset;
			threeDFloor.CeilingSlope = ctrl.ThreeDFloor.CeilingSlope;
			threeDFloor.CeilingSlopeOffset = ctrl.ThreeDFloor.CeilingSlopeOffset;

			for (int i = 0; i < checkedListBoxSectors.Items.Count; i++)
				checkedListBoxSectors.SetItemChecked(i, ctrl.checkedListBoxSectors.GetItemChecked(i));
		}

		public void Update(ThreeDFloor threeDFloor)
		{
			isnew = false;

			this.threeDFloor = threeDFloor;

			sectorBorderTexture.TextureName = threeDFloor.BorderTexture;
			sectorTopFlat.TextureName = threeDFloor.TopFlat;
			sectorBottomFlat.TextureName = threeDFloor.BottomFlat;
			sectorCeilingHeight.Text = threeDFloor.TopHeight.ToString();
			sectorFloorHeight.Text = threeDFloor.BottomHeight.ToString();
			borderHeightLabel.Text = (threeDFloor.TopHeight - threeDFloor.BottomHeight).ToString();

			typeArgument.Setup(General.Map.Config.LinedefActions[160].Args[1]);
			flagsArgument.Setup(General.Map.Config.LinedefActions[160].Args[2]);
			alphaArgument.Setup(General.Map.Config.LinedefActions[160].Args[3]);

			typeArgument.SetValue(threeDFloor.Type);
			flagsArgument.SetValue(threeDFloor.Flags);
			alphaArgument.SetValue(threeDFloor.Alpha);
			sectorBrightness.Text = threeDFloor.Brightness.ToString();

			AddSectorCheckboxes();

			if(sector == null || sector.IsDisposed)
				sector = General.Map.Map.CreateSector();

			if (threeDFloor.Sector != null)
			{
				threeDFloor.Sector.CopyPropertiesTo(sector);
				tagsLabel.Text = String.Join(", ", sector.Tags.Select(o => o.ToString()).ToArray());
			}

			if (sector != null && !sector.IsDisposed)
				sector.Selected = false;
		}

		public void ApplyToThreeDFloor()
		{
			Regex r = new Regex(@"\d+");

			threeDFloor.TopHeight = sectorCeilingHeight.GetResult(threeDFloor.TopHeight);
			threeDFloor.BottomHeight = sectorFloorHeight.GetResult(threeDFloor.BottomHeight);
			threeDFloor.TopFlat = sectorTopFlat.TextureName;
			threeDFloor.BottomFlat = sectorBottomFlat.TextureName;
			threeDFloor.BorderTexture = sectorBorderTexture.TextureName;

			threeDFloor.Type = typeArgument.GetResult(threeDFloor.Type);
			threeDFloor.Flags = flagsArgument.GetResult(threeDFloor.Flags);
			threeDFloor.Alpha = alphaArgument.GetResult(threeDFloor.Alpha);
			threeDFloor.Brightness = sectorBrightness.GetResult(threeDFloor.Brightness);

			threeDFloor.Tags = sector.Tags;

			threeDFloor.IsNew = isnew;

			if (threeDFloor.Sector != null)
			{
				sector.CopyPropertiesTo(threeDFloor.Sector);
				tagsLabel.Text = String.Join(", ", sector.Tags.Select(o => o.ToString()).ToArray());
			}

			threeDFloor.TaggedSectors = new List<Sector>();

			for (int i = 0; i < checkedListBoxSectors.Items.Count; i++)
			{
				string text = checkedListBoxSectors.Items[i].ToString();
				bool ischecked = !(checkedListBoxSectors.GetItemCheckState(i) == CheckState.Unchecked);

				if (ischecked)
				{
					var matches = r.Matches(text);
					Sector s = General.Map.Map.GetSectorByIndex(int.Parse(matches[0].ToString()));
					threeDFloor.TaggedSectors.Add(s);
				}
			}
		}

		private void AddSectorCheckboxes()
		{
			List<Sector> sectors = new List<Sector>(BuilderPlug.TDFEW.SelectedSectors.OrderBy(o => o.Index));

			checkedsectors = new List<int>();

			checkedListBoxSectors.Items.Clear();

			foreach (Sector s in ThreeDFloor.TaggedSectors)
			{
				if (!sectors.Contains(s))
					sectors.Add(s);
			}

			if (sectors == null)
				return;

			foreach (Sector s in sectors)
			{
				int i = checkedListBoxSectors.Items.Add("Sector " + s.Index.ToString(), ThreeDFloor.TaggedSectors.Contains(s));

				if(ThreeDFloor.TaggedSectors.Contains(s))
					checkedsectors.Add(s.Index);

				if (!BuilderPlug.TDFEW.SelectedSectors.Contains(s))
				{
					checkedListBoxSectors.SetItemCheckState(i, CheckState.Indeterminate);
				}
			}
		}

		private void buttonDuplicate_Click(object sender, EventArgs e)
		{
			((ThreeDFloorEditorWindow)this.ParentForm).DuplicateThreeDFloor(this);
		}

		private void buttonSplit_Click(object sender, EventArgs e)
		{
			((ThreeDFloorEditorWindow)this.ParentForm).SplitThreeDFloor(this);
		}

		private void buttonCheckAll_Click(object sender, EventArgs e)
		{
			for (int i = 0; i < checkedListBoxSectors.Items.Count; i++)
				checkedListBoxSectors.SetItemChecked(i, true);
		}

		private void buttonUncheckAll_Click(object sender, EventArgs e)
		{
			for (int i = 0; i < checkedListBoxSectors.Items.Count; i++)
				checkedListBoxSectors.SetItemChecked(i, false);
		}

		private void buttonEditSector_Click(object sender, EventArgs e)
		{
			sector.SetCeilTexture(sectorTopFlat.TextureName);
			sector.SetFloorTexture(sectorBottomFlat.TextureName);
			sector.CeilHeight = sectorCeilingHeight.GetResult(sector.CeilHeight);
			sector.FloorHeight = sectorFloorHeight.GetResult(sector.FloorHeight);
			sector.Brightness = sectorBrightness.GetResult(sector.Brightness);

			DialogResult result = General.Interface.ShowEditSectors(new List<Sector> { sector });

			if (result == DialogResult.OK)
			{
				sectorTopFlat.TextureName = sector.CeilTexture;
				sectorBottomFlat.TextureName = sector.FloorTexture;
				sectorCeilingHeight.Text = sector.CeilHeight.ToString();
				sectorFloorHeight.Text = sector.FloorHeight.ToString();
				sectorBrightness.Text = sector.Brightness.ToString();
				tagsLabel.Text = String.Join(", ", sector.Tags.Select(o => o.ToString()).ToArray());
			}
		}

		private void checkedListBoxSectors_ItemCheck(object sender, ItemCheckEventArgs e)
		{
			if (e.CurrentValue == CheckState.Indeterminate)
			{
				e.NewValue = CheckState.Indeterminate;
			}
			else
			{
				Regex r = new Regex(@"\d+");

				if (((ListBox)sender).SelectedItem == null)
					return;

				var matches = r.Matches(((ListBox)sender).SelectedItem.ToString());

				int sectornum = int.Parse(matches[0].ToString());

				if (e.NewValue == CheckState.Checked)
					checkedsectors.Add(sectornum);
				else
					checkedsectors.Remove(sectornum);
			}
		}

		private void ThreeDFloorHelperControl_Paint(object sender, PaintEventArgs e)
		{
			Color c = Color.FromArgb(0, 192, 0); //  Color.FromArgb(255, Color.Green);

			if (isnew)
				ControlPaint.DrawBorder(
					e.Graphics,
					this.ClientRectangle,
					c, // leftColor
					5, // leftWidth
					ButtonBorderStyle.Solid, // leftStyle
					c, // topColor
					0, // topWidth
					ButtonBorderStyle.None, // topStyle
					c, // rightColor
					0, // rightWidth
					ButtonBorderStyle.None, // rightStyle
					c, // bottomColor
					0, // bottomWidth
					ButtonBorderStyle.None // bottomStyle
				);
		}

		private void RecomputeBorderHeight(object sender, EventArgs e)
		{
			borderHeightLabel.Text = (sectorCeilingHeight.GetResult(threeDFloor.TopHeight) - sectorFloorHeight.GetResult(threeDFloor.BottomHeight)).ToString();
		}

		private void buttonDetach_Click(object sender, EventArgs e)
		{
			((ThreeDFloorEditorWindow)this.ParentForm).DetachThreeDFloor(this);
		}
	}
}
