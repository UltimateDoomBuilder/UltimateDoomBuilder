using System;
using System.Windows.Forms;

namespace CodeImp.DoomBuilder.AutomapMode
{
	public partial class MenusForm : UserControl
	{
		public event EventHandler OnShowHiddenLinesChanged;
		public event EventHandler OnShowSecretSectorsChanged;
		public event EventHandler OnShowLocksChanged;
        public event EventHandler OnShowTexturesChanged;
        internal event EventHandler OnColorPresetChanged;

		public bool ShowHiddenLines { get { return showhiddenlines.Checked; } set { showhiddenlines.Checked = value; } }
		public bool ShowSecretSectors { get { return showsecretsectors.Checked; } set { showsecretsectors.Checked = value; } }
		public bool ShowLocks { get { return showlocks.Checked; } set { showlocks.Checked = value; } }
		public bool ShowTextures { get { return showtextures.Checked; } set { showtextures.Checked = value; } }
		internal AutomapMode.ColorPreset ColorPreset { get { return (AutomapMode.ColorPreset)colorpreset.SelectedIndex; } set { colorpreset.SelectedIndex = (int)value; } }
		
		public MenusForm()
		{
			InitializeComponent();
		}

		public void Register()
		{
			General.Interface.BeginToolbarUpdate(); //mxd
			General.Interface.AddButton(showhiddenlines);
			General.Interface.AddButton(showsecretsectors);
			if(!General.Map.DOOM) General.Interface.AddButton(showlocks);
			General.Interface.AddButton(showtextures);
			General.Interface.AddButton(colorpresetseparator);
			General.Interface.AddButton(colorpresetlabel);
			General.Interface.AddButton(colorpreset);
			General.Interface.EndToolbarUpdate(); //mxd
		}

		public void Unregister()
		{
			General.Interface.BeginToolbarUpdate(); //mxd
			General.Interface.RemoveButton(colorpreset);
			General.Interface.RemoveButton(colorpresetlabel);
			General.Interface.RemoveButton(colorpresetseparator);
			General.Interface.RemoveButton(showlocks);
            General.Interface.RemoveButton(showtextures);
            General.Interface.RemoveButton(showsecretsectors);
			General.Interface.RemoveButton(showhiddenlines);
			General.Interface.EndToolbarUpdate(); //mxd
		}

		private void showhiddenlines_CheckedChanged(object sender, EventArgs e)
		{
			if(OnShowHiddenLinesChanged != null) OnShowHiddenLinesChanged(showhiddenlines.Checked, EventArgs.Empty);
		}

		private void showsecretsectors_CheckedChanged(object sender, EventArgs e)
		{
			if(OnShowSecretSectorsChanged != null) OnShowSecretSectorsChanged(showsecretsectors.Checked, EventArgs.Empty);
		}

		private void showlocks_CheckedChanged(object sender, EventArgs e)
		{
			if(OnShowLocksChanged != null) OnShowLocksChanged(showlocks.Checked, EventArgs.Empty);
        }

        private void showtextures_CheckedChanged(object sender, EventArgs e)
        {
            if (OnShowTexturesChanged != null) OnShowTexturesChanged(showtextures.Checked, EventArgs.Empty);
        }

        private void colorpreset_SelectedIndexChanged(object sender, EventArgs e)
		{
			if(OnColorPresetChanged != null) OnColorPresetChanged(colorpreset.SelectedIndex, EventArgs.Empty);
		}
	}
}
