using System;
using System.Windows.Forms;

namespace CodeImp.DoomBuilder.BuilderModes
{
	internal partial class DrawEllipseOptionsPanel : UserControl
	{
		public event EventHandler OnValueChanged;
		public event EventHandler OnContinuousDrawingChanged;
		public event EventHandler OnShowGuidelinesChanged;
		public event EventHandler OnRadialDrawingChanged;

		private bool blockevents;

		public int Spikiness { get { return (int)spikiness.Value; } set { blockevents = true; spikiness.Value = value; blockevents = false; } }
		public int Subdivisions { get { return (int)subdivs.Value; } set { blockevents = true; subdivs.Value = value; blockevents = false; } }
		public int Angle { get { return (int)angle.Value; } set { blockevents = true; angle.Value = value; blockevents = false; } }
		public int MaxSubdivisions { get { return (int)subdivs.Maximum; } set { subdivs.Maximum = value; } }
		public int MinSubdivisions { get { return (int)subdivs.Minimum;  } set { subdivs.Minimum = value; } }
		public int MaxSpikiness { get { return (int)spikiness.Maximum; } set { spikiness.Maximum = value; } }
		public int MinSpikiness { get { return (int)spikiness.Minimum; } set { spikiness.Minimum = value; } }
		public bool ContinuousDrawing { get { return continuousdrawing.Checked; } set { continuousdrawing.Checked = value; } }
		public bool ShowGuidelines { get { return showguidelines.Checked; } set { showguidelines.Checked = value; } }
		public bool RadialDrawing { get { return radialdrawing.Checked; } set { radialdrawing.Checked = value; } }
		
		public DrawEllipseOptionsPanel() 
		{
			InitializeComponent();
		}

		public void Register() 
		{
			spikiness.ValueChanged += ValueChanged;
			subdivs.ValueChanged += ValueChanged;
			angle.ValueChanged += ValueChanged;

			General.Interface.BeginToolbarUpdate();
			General.Interface.AddButton(continuousdrawing);
			General.Interface.AddButton(showguidelines);
			General.Interface.AddButton(radialdrawing);
			General.Interface.AddButton(toolStripSeparator1);
			General.Interface.AddButton(subdivslabel);
			General.Interface.AddButton(subdivs);
			General.Interface.AddButton(spikinesslabel);
			General.Interface.AddButton(spikiness);
			General.Interface.AddButton(anglelabel);
			General.Interface.AddButton(angle);
			General.Interface.AddButton(reset);
			General.Interface.EndToolbarUpdate();
		}

		public void Unregister() 
		{
			General.Interface.BeginToolbarUpdate();
			General.Interface.RemoveButton(reset);
			General.Interface.RemoveButton(angle);
			General.Interface.RemoveButton(anglelabel);
			General.Interface.RemoveButton(spikiness);
			General.Interface.RemoveButton(spikinesslabel);
			General.Interface.RemoveButton(subdivs);
			General.Interface.RemoveButton(subdivslabel);
			General.Interface.RemoveButton(toolStripSeparator1);
			General.Interface.RemoveButton(showguidelines);
			General.Interface.RemoveButton(continuousdrawing);
			General.Interface.RemoveButton(radialdrawing);
			General.Interface.EndToolbarUpdate();
		}

		private void ValueChanged(object sender, EventArgs e) 
		{
			if(!blockevents && OnValueChanged != null) OnValueChanged(this, EventArgs.Empty);
		}

		private void reset_Click(object sender, EventArgs e) 
		{
			// Reset values
			blockevents = true;
			spikiness.Value = 0;
			angle.Value = 0;
			subdivs.Value = 6;
			blockevents = false;

			// Dispatch event
			OnValueChanged(this, EventArgs.Empty);
		}

		private void continuousdrawing_CheckedChanged(object sender, EventArgs e)
		{
			if(OnContinuousDrawingChanged != null) OnContinuousDrawingChanged(continuousdrawing.Checked, EventArgs.Empty);
		}

		private void showguidelines_CheckedChanged(object sender, EventArgs e)
		{
			if(OnShowGuidelinesChanged != null) OnShowGuidelinesChanged(showguidelines.Checked, EventArgs.Empty);
		}
		
		private void radialdrawing_CheckedChanged(object sender, EventArgs e)
		{
			if(OnRadialDrawingChanged != null) OnRadialDrawingChanged(radialdrawing.Checked, EventArgs.Empty);
		}
	}
}
