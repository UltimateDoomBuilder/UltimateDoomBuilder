using System;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Windows;

namespace CodeImp.DoomBuilder.BuilderEffects
{
	public partial class MenusForm : Form
	{
		public MenusForm() 
		{
			InitializeComponent();
		}

		// This invokes an action from control event
		private void InvokeTaggedAction(object sender, EventArgs e) 
		{
			General.Interface.InvokeTaggedAction(sender, e);
		}

		// This registers with the core
		public void Register() 
		{
			// Add the menus to the core
			General.Interface.BeginToolbarUpdate();

#if MONO_WINFORMS
			// Mono fix
			menuStrip.Items.Clear();
			toolStrip.Items.Clear();
			stripimport.DropDownItems.Clear();
			stripmodes.DropDownItems.Clear();
#endif

			General.Interface.AddModesMenu(menujitter, "002_modify");
			General.Interface.AddModesButton(buttonjitter, "002_modify");
			General.Interface.AddModesMenu(menusectorflatshading, "002_modify");
			General.Interface.AddModesButton(buttonsectorflatshading, "002_modify");
			General.Interface.AddMenu(toolStripMenuItem1, MenuSection.FileImport);

			General.Interface.EndToolbarUpdate();
		}

		// This unregisters from the core
		public void Unregister() 
		{
			// Remove the menus from the core
			General.Interface.BeginToolbarUpdate();

			General.Interface.RemoveMenu(menujitter);
			General.Interface.RemoveButton(buttonjitter);
			General.Interface.RemoveMenu(menusectorflatshading);
			General.Interface.RemoveButton(buttonsectorflatshading);
			General.Interface.RemoveMenu(toolStripMenuItem1);

			General.Interface.EndToolbarUpdate();
		}
	}
}
