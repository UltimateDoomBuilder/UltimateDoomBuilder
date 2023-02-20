#region ================== Namespaces

using System;
using System.Drawing;
using System.Windows.Forms;

#endregion

namespace CodeImp.DoomBuilder.Controls
{
	// This is based on https://stackoverflow.com/questions/50918225/how-to-add-placeholder-text-to-toolstriptextbox
	[ToolboxBitmap(typeof(ToolStripTextBox))]
	public class PlaceholderToolStripTextBox : ToolStripTextBox
	{
		#region ================== Constants

		private const int EM_SETCUEBANNER = 0x1501;

		#endregion

		#region ================== Variables

		private string placeholder;

		#endregion

		#region ================== Properties

		public string PlaceholderText
		{
			get { return placeholder; }
			set
			{
				placeholder = value;
				UpdatePlaceholderText();
			}
		}

		#endregion

		#region ================== Constructors

		public PlaceholderToolStripTextBox()
		{
			Control.HandleCreated += Control_HandleCreated;
		}

		#endregion

		#region ================== Events

		private void Control_HandleCreated(object sender, EventArgs e)
		{
			if (!string.IsNullOrEmpty(placeholder))
				UpdatePlaceholderText();
		}

		#endregion

		#region ================== Methods

		private void UpdatePlaceholderText()
		{
			#if !MONO_WINFORMS
			SysCall.SendMessage(Control.Handle, EM_SETCUEBANNER, 0, placeholder);
			#endif
		}

		#endregion
	}
}