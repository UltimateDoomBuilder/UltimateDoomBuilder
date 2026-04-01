using System.Runtime.CompilerServices;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Windows;

namespace CodeImp.DoomBuilder.ThreeDFloorMode
{
	public partial class PreferencesForm : DelayedForm
	{
		public PreferencesForm()
		{
			InitializeComponent();

			sectorlabels.SelectedIndex = (int)BuilderPlug.Me.SectorLabelDisplayOption;
			slopevertexlabels.SelectedIndex = (int)BuilderPlug.Me.SlopeVertexLabelDisplayOption;
		}

		#region ================== Methods

		// When OK is pressed on the preferences dialog
		// Prevent inlining, otherwise there are unexpected interactions with Assembly.GetCallingAssembly
		// See https://docs.microsoft.com/en-us/dotnet/api/system.reflection.assembly.getcallingassembly?view=netframework-4.6.1#remarks
		[MethodImplAttribute(MethodImplOptions.NoInlining)]
		public void OnAccept(PreferencesController controller)
		{
			// Write preferred settings
			General.Settings.WritePluginSetting("sectorlabeldisplayoption", sectorlabels.SelectedIndex);
			General.Settings.WritePluginSetting("slopevertexlabeldisplayoption", slopevertexlabels.SelectedIndex);
		}

		// This sets up the form with the preferences controller
		public void Setup(PreferencesController controller)
		{
			// Add tab pages
			foreach (TabPage p in tabs.TabPages)
			{
				controller.AddTab(p);
			}

			// Bind events
			controller.OnAccept += OnAccept;
		}

		#endregion
	}
}
