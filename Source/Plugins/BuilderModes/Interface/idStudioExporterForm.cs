/*
MIT License

Copyright (c) 2024 FlavorfulGecko5

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE. 
*/

using System;
using System.IO;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Controls;
using CodeImp.DoomBuilder.Windows;

namespace CodeImp.DoomBuilder.BuilderModes.Interface
{
	public partial class idStudioExporterForm : DelayedForm
	{
		public string ModPath { get { return gui_ModPath.Text; } }

		public string MapName { get { return gui_MapName.Text; } }

		public float Downscale { get { return (float)gui_Downscale.Value; } }

		public float xShift { get { return (float)gui_xShift.Value; } }

		public float yShift { get { return (float)gui_yShift.Value; } }

		public float zShift { get { return (float)gui_zShift.Value; } }

		public bool ExportTextures { get { return gui_ExportTextures.Checked; } }

		public idStudioExporterForm()
		{
			InitializeComponent();

			gui_ModPath.Text = Path.GetDirectoryName(General.Map.FilePathName);
			gui_MapName.Text = General.Map.Options.LevelName.ToLower();
			gui_Downscale.Value = 20;
			gui_xShift.Value = 0;
			gui_yShift.Value = 0;
			gui_zShift.Value = 0;

			int imageCount = General.Map.Data.Textures.Count + General.Map.Data.Flats.Count;
			gui_ShowTextCount.Text = String.Format("{0} TGA images and {1} material2 decls will be created.",
				imageCount + 1, imageCount);
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
			// Validate mapname
			{
				string mname = gui_MapName.Text;
				bool validname = true;
				if (mname.Length == 0)
					validname = false;
				if (mname[0] < 'a' || mname[0] > 'z')
					validname = false;
				foreach(char c in mname)
				{
					if (c >= 'a' && c <= 'z')
						continue;
					if (c >= '0' && c <= '9')
						continue;
					if (c == '_')
						continue;
					validname = false;
					break;
				}

				if (!validname)
				{
					MessageBox.Show("Map names must be all lowercase, numbers and underscores only. First char must be letter.",
						"Invalid Map Name", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
					return;
				}
					
			}

			this.DialogResult = DialogResult.OK;
			this.Close();
		}

		private void evt_CancelButton(object sender, EventArgs e)
		{
			this.Close();
		}
	}
}
