using System;
using System.Windows.Forms;

namespace CodeImp.DoomBuilder.BuilderModes.Interface
{
	public partial class CurveLinedefsOptionsPanel : UserControl
	{
		#region ================== Event Handlers

		public event EventHandler OnValueChanged;

		#endregion

		#region ================== Variables

		private bool blockevents;
		private bool fixedcurveoutwards;

		#endregion

		#region ================== Properties

		public int Vertices { get { return (int)verts.Value; } set { verts.Value = General.Clamp(value, (int)verts.Minimum, (int)verts.Maximum); } }
		public int Distance { get { return (int)distance.Value; } set { distance.Value = General.Clamp(value, (int)distance.Minimum, (int)distance.Maximum); } }
		public int DistanceIncrement { get { return (int)distance.Increment; } }
		public int Angle { get { return (int)angle.Value; } set { angle.Value = (decimal)General.Clamp(value, (float)angle.Minimum, (float)angle.Maximum); } }
		public int AngleIncrement { get { return (int)angle.Increment; } }
		public int MaximumAngle { get { return (int)angle.Maximum; } }
		public bool FixedCurve { get { return fixedcurve.Checked; } }
		public bool FixedCurveOutwards { get { return fixedcurveoutwards; } }

		#endregion
		
		#region ================== Constructor

		public CurveLinedefsOptionsPanel()
		{
			InitializeComponent();
		}

		#endregion

		#region ================== Mathods

		public void SetValues(int verts, int distance, int angle, bool fixedcurve, bool fixeddirection)
		{
			blockevents = true;

			this.verts.Value = General.Clamp(verts, (int)this.verts.Minimum, (int)this.verts.Maximum);
			this.distance.Value = General.Clamp(distance, (int)this.distance.Minimum, (int)this.distance.Maximum);
			this.angle.Value = General.Clamp(angle, (int)this.angle.Minimum, (int)this.angle.Maximum);
			this.fixedcurve.Checked = fixedcurve;
			this.fixedcurveoutwards = fixeddirection;

			blockevents = false;
		}

		public void Register()
		{
			General.Interface.BeginToolbarUpdate();

#if MONO_WINFORMS
			// Mono fix
			toolstrip.Items.Clear();
#endif

			General.Interface.AddButton(vertslabel);
			General.Interface.AddButton(verts);
			General.Interface.AddButton(distancelabel);
			General.Interface.AddButton(distance);
			General.Interface.AddButton(anglelabel);
			General.Interface.AddButton(angle);
			General.Interface.AddButton(flip);
			General.Interface.AddButton(reset);
			General.Interface.AddButton(separator1);
			General.Interface.AddButton(fixedcurve);
			General.Interface.AddButton(separator2);
			General.Interface.AddButton(apply);
			General.Interface.AddButton(cancel);
			General.Interface.EndToolbarUpdate();
		}

		public void Unregister()
		{
			General.Interface.BeginToolbarUpdate();
			General.Interface.RemoveButton(cancel);
			General.Interface.RemoveButton(apply);
			General.Interface.RemoveButton(anglelabel);
			General.Interface.RemoveButton(separator2);
			General.Interface.RemoveButton(fixedcurve);
			General.Interface.RemoveButton(separator1);
			General.Interface.RemoveButton(reset);
			General.Interface.RemoveButton(flip);
			General.Interface.RemoveButton(angle);
			General.Interface.RemoveButton(anglelabel);
			General.Interface.RemoveButton(distance);
			General.Interface.RemoveButton(distancelabel);
			General.Interface.RemoveButton(verts);
			General.Interface.RemoveButton(vertslabel);
			General.Interface.EndToolbarUpdate();
		}

#endregion

		#region ================== Events

		private void apply_Click(object sender, EventArgs e)
		{
			// Apply now
			General.Editing.AcceptMode();
		}

		private void cancel_Click(object sender, EventArgs e)
		{
			// Cancel now
			General.Editing.CancelMode();
		}

		private void OnUIValuesChanged(object sender, EventArgs e)
		{
			if(!blockevents && OnValueChanged != null) OnValueChanged(this, EventArgs.Empty);
		}

		private void fixedcurve_CheckedChanged(object sender, EventArgs e)
		{
			// Enable/disable controls
			distance.Enabled = !fixedcurve.Checked;
			distancelabel.Enabled = !fixedcurve.Checked;

			if(!blockevents && OnValueChanged != null) OnValueChanged(this, EventArgs.Empty);
		}

		private void flip_Click(object sender, EventArgs e)
		{
			if (fixedcurve.Checked)
			{
				fixedcurveoutwards = !fixedcurveoutwards;
				OnValueChanged(this, EventArgs.Empty);
			}
			else
				distance.Value = -distance.Value;
			
		}

		private void reset_Click(object sender, EventArgs e)
		{
			SetValues(CurveLinedefsMode.DEFAULT_VERTICES_COUNT, CurveLinedefsMode.DEFAULT_DISTANCE, CurveLinedefsMode.DEFAULT_ANGLE, false, true);
			if(OnValueChanged != null) OnValueChanged(this, EventArgs.Empty);
		}

		#endregion

	}
}
