using System;
using System.Windows.Forms;

// As per http://stackoverflow.com/questions/10362988/treeview-flickering
// Gets rid of the flickering default TreeView

namespace CodeImp.DoomBuilder.Controls
{
	public class BufferedTreeView : TreeView
	{
		// Pinvoke:
		private const int TVM_SETEXTENDEDSTYLE = 0x1100 + 44;
		//private const int TVM_GETEXTENDEDSTYLE = 0x1100 + 45;
		private const int TVS_EX_DOUBLEBUFFER = 0x0004;

		// Methods
		protected override void OnHandleCreated(EventArgs e)
		{
			SysCall.SendMessage(this.Handle, TVM_SETEXTENDEDSTYLE, (IntPtr)TVS_EX_DOUBLEBUFFER, (IntPtr)TVS_EX_DOUBLEBUFFER);
			base.OnHandleCreated(e);
		}
	}
}
