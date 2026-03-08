using System;
using System.Windows.Forms;

namespace CodeImp.DoomBuilder.BuilderModes
{
	internal partial class DrawLineOptionsPanel : UserControl
	{
		public event EventHandler OnContinuousDrawingChanged;
		public event EventHandler OnAutoCloseDrawingChanged;
		public event EventHandler OnShowGuidelinesChanged;

		public bool ContinuousDrawing { get { return continuousdrawing.Checked; } set { continuousdrawing.Checked = value; } }
		public bool AutoCloseDrawing { get { return autoclosedrawing.Checked; } set { autoclosedrawing.Checked = value; } }
		public bool ShowGuidelines { get { return showguidelines.Checked; } set { showguidelines.Checked = value; } }
		
		public DrawLineOptionsPanel()
		{
			InitializeComponent();
		}

		public void Register()
		{
			General.Interface.BeginToolbarUpdate();

#if MONO_WINFORMS
			// Mono fix
			toolStrip1.Items.Clear();
#endif

			General.Interface.AddButton(continuousdrawing);
			General.Interface.AddButton(autoclosedrawing);
			General.Interface.AddButton(showguidelines);
			General.Interface.EndToolbarUpdate();
		}

		public void Unregister()
		{
			General.Interface.BeginToolbarUpdate();
			General.Interface.RemoveButton(showguidelines);
			General.Interface.RemoveButton(autoclosedrawing);
			General.Interface.RemoveButton(continuousdrawing);
			General.Interface.EndToolbarUpdate();
		}

		private void continuousdrawing_CheckedChanged(object sender, EventArgs e)
		{
			if(OnContinuousDrawingChanged != null) OnContinuousDrawingChanged(continuousdrawing.Checked, EventArgs.Empty);
		}

		private void autoclosedrawing_CheckedChanged(object sender, EventArgs e)
		{
			if(OnAutoCloseDrawingChanged != null) OnAutoCloseDrawingChanged(autoclosedrawing.Checked, EventArgs.Empty);
		}

		private void showguidelines_CheckedChanged(object sender, EventArgs e)
		{
			if(OnShowGuidelinesChanged != null) OnShowGuidelinesChanged(showguidelines.Checked, EventArgs.Empty);
		}
	}
}
