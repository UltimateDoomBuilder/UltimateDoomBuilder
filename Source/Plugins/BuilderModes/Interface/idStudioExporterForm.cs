using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CodeImp.DoomBuilder.BuilderModes.IO;
using CodeImp.DoomBuilder.Controls;
using CodeImp.DoomBuilder.Windows;

namespace CodeImp.DoomBuilder.BuilderModes.Interface
{
	public partial class idStudioExporterForm : DelayedForm
	{
		public string ModPath { get { return gui_ModPath.Text; } }

		public float xyDownscale { get { return (float)gui_xyDownscale.Value; } }

		public float zDownscale { get { return (float)gui_zDownscale.Value; } }

		public float xShift { get { return (float)gui_xShift.Value; } }

		public float yShift { get { return (float)gui_yShift.Value; } }

		public bool ExportTextures { get { return gui_ExportTextures.Checked; } }

		public idStudioExporterForm()
		{
			InitializeComponent();

			gui_ModPath.Text = Path.GetDirectoryName(General.Map.FilePathName);
			gui_xShift.Value = 0;
			gui_yShift.Value = 0;
		}

		private void evt_FolderButton(object sender, EventArgs e)
		{
			FolderSelectDialog folderDialog = new FolderSelectDialog();
			folderDialog.Title = "Select Mod Folder";
			folderDialog.InitialDirectory = gui_ModPath.Text;

			if(folderDialog.ShowDialog(this.Handle))
			{
				gui_ModPath.Text = folderDialog.FileName;
			}
		}

		private void evt_ButtonExport(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.OK;
			this.Close();
		}

		private void evt_CancelButton(object sender, EventArgs e)
		{
			this.Close();
		}
	}
}
