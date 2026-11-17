// ULDB: the view of upstream's ScriptEditorPreviewControl (Preferences' script editor colour
// preview): a read-only editor showing its sample text with the chosen colours.

using Avalonia.Controls;
using CodeImp.DoomBuilder.Controls;
using ScintillaNET;
using ULDB.Scripting;
using ULDB.Shell;

namespace ULDB.Controls
{
	internal sealed class ScriptEditorPreviewControlView : UserControl
	{
		public ScriptEditorPreviewControlView(ScriptEditorPreviewControl model)
		{
			ModelBinder binder = new ModelBinder(model);
			Content = new ScintillaView(binder.Field<Scintilla>("scriptedit")) { UseSharedZoom = false };
		}
	}
}
