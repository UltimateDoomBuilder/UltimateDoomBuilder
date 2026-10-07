#region ================== Namespaces

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Windows;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	internal struct FitTextureOptions
	{
		public double HorizontalRepeat;
		public double VerticalRepeat;
		public int PatternWidth;
		public int PatternHeight;
		public bool FitWidth;
		public bool FitHeight;
		public bool FitAcrossSurfaces;
		public bool AutoWidth;
		public bool AutoHeight;
		public Rectangle GlobalBounds;
		public Rectangle Bounds;

		//Initial texture coordinats
		public double InitialOffsetX;
		public double InitialOffsetY;
		public double ControlSideOffsetX;
		public double ControlSideOffsetY;
		public double InitialScaleX;
		public double InitialScaleY;
	}
	
	internal partial class FitTexturesForm : DelayedForm
	{
		#region ================== Event handlers

		#endregion

		#region ================== Variables

		private static Point location = Point.Empty;
		private bool blockupdate;
		private double prevhorizrepeat;
		private double prevvertrepeat;

		// Settings
		private static double horizontalrepeat = 1.0;
		private static double verticalrepeat = 1.0;
		private static bool fitacrosssurfaces = true;
		private static bool fitwidth = true;
		private static bool fitheight = true;

		//Surface stuff
		private List<SortedVisualSide> strips;

		#endregion

		#region ================== Constructor

		public FitTexturesForm() 
		{
			InitializeComponent();

			if(!location.IsEmpty)
			{
				this.StartPosition = FormStartPosition.Manual;
				this.Location = location;
			}
		}

		#endregion

		#region ================== Methods

		public bool Setup(IEnumerable<BaseVisualGeometrySidedef> sides)
		{
			// Get shapes
			strips = BuilderModesTools.SortVisualSides(sides);

			// No dice...
			if(strips.Count == 0)
			{
				General.Interface.DisplayStatus(StatusType.Warning, "Failed to setup sidedef chains...");
				this.DialogResult = DialogResult.Cancel;
				this.Close();
				return false;
			}

			// Restore settings
			blockupdate = true;

			// Make sure we start with sensible values for horizontal and vertical repeat
			if (horizontalrepeat == 0.0)
				horizontalrepeat = 1.0;

			if (verticalrepeat == 0.0)
				verticalrepeat = 1.0;

			horizrepeat.Text = horizontalrepeat.ToString();
			vertrepeat.Text = verticalrepeat.ToString();
			prevhorizrepeat = horizontalrepeat;
			prevvertrepeat = verticalrepeat;
			cbfitconnected.Checked = fitacrosssurfaces;
			cbfitconnected.Enabled = (strips.Count > 1);
			cbfitwidth.Checked = fitwidth;
			cbfitheight.Checked = fitheight;
			UpdateRepeatGroup();

			blockupdate = false;

			//trigger update
			UpdateChanges();

			return true;
		}

		private void UpdateChanges()
		{
			// Apply changes
			FitTextureOptions options = new FitTextureOptions
			                            {
											FitAcrossSurfaces = (cbfitconnected.Enabled && cbfitconnected.Checked),
											FitWidth = cbfitwidth.Checked,
											FitHeight = cbfitheight.Checked,
											PatternWidth = (int)patternwidth.GetResultFloat(0),
											PatternHeight = (int)patternheight.GetResultFloat(0),
											AutoWidth = cbautowidth.Checked,
											AutoHeight = cbautoheight.Checked
			                            };

			// Default horizonal repeat to 1 if  the value can't be parsed or if it's 0
			double hrepeat = horizrepeat.GetResultFloat(double.NaN);
			if (double.IsNaN(hrepeat) || hrepeat == 0.0)
				hrepeat = 1.0;

			// Default vertical repeat to 1 if  the value can't be parsed or if it's 0
			double vrepeat = vertrepeat.GetResultFloat(double.NaN);
			if (double.IsNaN(vrepeat) || vrepeat == 0.0)
				vrepeat = 1.0;

			options.HorizontalRepeat = hrepeat;
			options.VerticalRepeat = vrepeat;

			foreach (SortedVisualSide side in strips) side.OnTextureFit(options);
		}

		private void UpdateRepeatGroup()
		{
			// Disable whole group?
			repeatgroup.Enabled = cbfitwidth.Checked || cbfitheight.Checked;
			if(!repeatgroup.Enabled) return;

			// Update control status
			cbautowidth.Enabled = cbfitwidth.Checked;
			patternwidth.Enabled = cbfitwidth.Checked && (cbautowidth.Enabled && cbautowidth.Checked);
			labelpatternwidth.Enabled = patternwidth.Enabled;
			labelhorizrepeat.Enabled = cbfitwidth.Checked;
			horizrepeat.Enabled = cbfitwidth.Checked && !cbautowidth.Checked;
			resethoriz.Enabled = cbfitwidth.Checked && !cbautowidth.Checked;

			cbautoheight.Enabled = cbfitheight.Checked;
			patternheight.Enabled = cbfitheight.Checked && (cbautoheight.Enabled && cbautoheight.Checked);
			labelpatternheight.Enabled = patternheight.Enabled;
			labelvertrepeat.Enabled = cbfitheight.Checked;
			vertrepeat.Enabled = cbfitheight.Checked && !cbautoheight.Checked;
			resetvert.Enabled = cbfitheight.Checked && !cbautoheight.Checked;
		}

		#endregion

		#region ================== Events

		private void FitTexturesForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			location = this.Location;

			// Store settings
			if(this.DialogResult == DialogResult.OK)
			{
				horizontalrepeat = horizrepeat.GetResultFloat(0);
				verticalrepeat = vertrepeat.GetResultFloat(0);
				fitacrosssurfaces = cbfitwidth.Checked;
				fitwidth = cbfitwidth.Checked;
				fitheight = cbfitheight.Checked;
			}
		}

		private void resethoriz_Click(object sender, EventArgs e)
		{
			prevhorizrepeat = 1;
			horizrepeat.Text = "1";
		}

		private void resetvert_Click(object sender, EventArgs e)
		{
			prevvertrepeat = 1;
			vertrepeat.Text = "1";
		}

		private void horizrepeat_ValueChanged(object sender, EventArgs e) 
		{
			if(blockupdate) return;

			prevhorizrepeat = horizrepeat.GetResultFloat(1);
			UpdateChanges();
		}

		private void vertrepeat_ValueChanged(object sender, EventArgs e) 
		{
			if(blockupdate) return;

			prevvertrepeat = vertrepeat.GetResultFloat(1);
			UpdateChanges();
		}

		private void accept_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.OK;
			this.Close();
		}

		private void cancel_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;
			this.Close();
		}

		private void cb_CheckedChanged(object sender, EventArgs e) 
		{
			if(blockupdate) return;
			UpdateRepeatGroup();
			UpdateChanges();
		}

		private void patternsize_ValueChanged(object sender, EventArgs e)
		{
			if (blockupdate) return;
			UpdateChanges();
		}

		#endregion
	}
}
