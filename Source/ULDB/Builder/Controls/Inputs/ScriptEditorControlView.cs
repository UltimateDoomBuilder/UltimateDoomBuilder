// ULDB: the view of upstream's ScriptEditorControl (model-ported): the function bar (a drop-down of
// the script's functions and blocks) above the text editor (ScintillaView, shown for the control's
// "scriptedit" Scintilla). Upstream's code runs unchanged on the Scintilla model.

using Avalonia.Controls;
using CodeImp.DoomBuilder.Controls;
using ScintillaNET;
using ULDB.Scripting;
using ULDB.Shell;
using WF = System.Windows.Forms;

namespace ULDB.Controls
{
	internal sealed class ScriptEditorControlView : UserControl
	{
		public ScriptEditorControlView(ScriptEditorControl model)
		{
			ComboBox functionbar = new ComboBox { HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch, Margin = new Avalonia.Thickness(0, 0, 0, 4), Name = "functionbar" };
			ContentControl host = new ContentControl();
			DockPanel.SetDock(functionbar, Dock.Top);
			DockPanel root = new DockPanel();
			root.Children.Add(functionbar);
			root.Children.Add(host);
			Content = root;

			ModelBinder binder = new ModelBinder(model);
			binder.BindComboBox(functionbar, binder.Field<WF.ComboBox>("functionbar"));
			binder.Custom(() => functionbar.IsVisible = binder.Field<WF.ComboBox>("functionbar").Visible, binder.Field<WF.ComboBox>("functionbar"));

			Scintilla scintilla = binder.Field<Scintilla>("scriptedit");
			ScintillaView view = new ScintillaView(scintilla) { CommandKey = keys => model.RaiseProcessCmdKey(keys) };
			// A function's signature, from the script configuration in use (it changes with the script type)
			view.WordInfo = word =>
			{
				CodeImp.DoomBuilder.Config.ScriptConfiguration config = binder.Field<CodeImp.DoomBuilder.Config.ScriptConfiguration>("scriptconfig");
				if(config == null || string.IsNullOrEmpty(word) || word.StartsWith("$")) return null;
				string definition = config.GetFunctionDefinition(config.GetKeywordCase(word));
				return string.IsNullOrWhiteSpace(definition) ? null : definition.Replace("[EP]", "").Trim();
			};
			WF.ContextMenuStrip menu = binder.Field<WF.ContextMenuStrip>("contextmenu");
			if(menu != null) view.ContextMenu = ToolStripViews.CreateContextMenu(menu);
			host.Content = view;
			binder.UpdateNow();
		}
	}
}
