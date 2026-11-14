// ULDB: code-behind for ScriptFindReplaceFormView.axaml, the window of upstream's ScriptFindReplaceForm (model-ported).

using Avalonia.Controls;
using ULDB.Shell;

namespace CodeImp.DoomBuilder.Windows
{
	public partial class ScriptFindReplaceFormView : Window
	{
		public ScriptFindReplaceFormView()
		{
			InitializeComponent();
		}

		internal ScriptFindReplaceFormView(ScriptFindReplaceForm form) : this()
		{
			new ModelBinder(form).BindByName(this).UpdateNow();
			// Upstream closes the dialog on Escape from the form's ProcessCmdKey (upstream issue #972); keys here go to the view
			AddHandler(KeyDownEvent, (s, e) =>
			{
				if(e.Key == Avalonia.Input.Key.Escape && e.KeyModifiers == Avalonia.Input.KeyModifiers.None) { e.Handled = true; form.Close(); }
			}, Avalonia.Interactivity.RoutingStrategies.Tunnel);
		}
	}
}
