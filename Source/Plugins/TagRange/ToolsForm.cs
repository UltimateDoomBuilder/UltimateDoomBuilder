using System;
using System.Windows.Forms;
using CodeImp.DoomBuilder.IO;

namespace CodeImp.DoomBuilder.TagRange
{
	public partial class ToolsForm : Form
	{
		bool buttonontoolbar;
		
		// Constructor
		public ToolsForm()
		{
			InitializeComponent();
			buttonontoolbar = false;

			// Mono fix
#if MONO_WINFORMS
			toolstrip.Items.Clear();
#endif
		}
		
		// This invokes an action from control event
		private void InvokeTaggedAction(object sender, EventArgs e)
		{
			General.Interface.InvokeTaggedAction(sender, e);
		}
		
		// This adds or removes the button from the DB toolbar
		public void UpdateButton()
		{
			if(buttonontoolbar)
			{
				General.Interface.RemoveButton(seperator1);
				General.Interface.RemoveButton(tagrangebutton);
				General.Interface.RemoveButton(seperator2);
				buttonontoolbar = false;
			}
			
			if(General.Editing.Mode == null)
				return;
			
			string modename = General.Editing.Mode.GetType().Name;
			IMapSetIO mapset = General.Map.FormatInterface;
			if((modename == "SectorsMode") ||
			   ((modename == "LinedefsMode") && mapset.HasLinedefTag) ||
			   ((modename == "ThingsMode") && mapset.HasThingTag))
			{
				General.Interface.AddButton(seperator1);
				General.Interface.AddButton(tagrangebutton);
				General.Interface.AddButton(seperator2);
				buttonontoolbar = true;
			}
		}
	}
}
