using System;
using System.Windows.Forms;

namespace CodeImp.DoomBuilder.RejectExplorer
{
	public partial class MenusForm : Form
	{
		public ToolStripButton ColorConfiguration { get { return colorconfiguration; } }

		public MenusForm()
		{
			InitializeComponent();
		}

		// This invokes an action from control event
		private void InvokeTaggedAction(object sender, EventArgs e)
		{
			General.Interface.InvokeTaggedAction(sender, e);
		}
	}
}
