#region ================== Copyright (c) 2020 Boris Iwanski

/*
 * This program is free software: you can redistribute it and/or modify
 *
 * it under the terms of the GNU General Public License as published by
 * 
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 *    but WITHOUT ANY WARRANTY; without even the implied warranty of
 * 
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
 * 
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.If not, see<http://www.gnu.org/licenses/>.
 */

#endregion

using System;
using System.Drawing.Imaging;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.BuilderModes.IO;

namespace CodeImp.DoomBuilder.BuilderModes.Interface
{
	enum ImageExportResult
	{
		OK,
		Canceled,
		OutOfMemory,
		ImageTooBig
	}

	public partial class ImageExportSettingsForm : Form
	{
		#region ================== Properties

		public string FilePath { get { return tbExportPath.Text.Trim(); } }
		public bool Floor { get { return rbFloor.Checked; } }
		public bool Fullbright { get { return cbFullbright.Checked; } }
		public bool Transparency { get { return cbTransparency.Checked; } }
		public bool ApplySectorColors { get { return cbApplySectorColors.Checked; } }
		public bool Brightmap { get { return cbBrightmap.Checked; } }
		public bool Tiles { get { return cbTiles.Checked; } }
		public float ImageScale { get { return (float)Math.Pow(2, cbScale.SelectedIndex); } }

		#endregion

		#region ================== Delegates

		private delegate void CallVoidMethodDeletage();
		private delegate void CallStringMethodDeletage(string s);
		private delegate void CallImageExportResultMethodDeletage(ImageExportResult ier);

		#endregion

		#region ================== Variables

		Thread exportthread;
		bool exporting;
		bool cancelexport;

		#endregion

		#region ================== Constructor

		public ImageExportSettingsForm()
		{
			InitializeComponent();

			cbImageFormat.SelectedIndex = 0;
			cbPixelFormat.SelectedIndex = 0;
			exporting = false;
			cancelexport = false;

			string name = Path.GetFileNameWithoutExtension(General.Map.FileTitle) + "_" + General.Map.Options.LevelName + "_" + Path.GetFileNameWithoutExtension(Path.GetRandomFileName());

			if (string.IsNullOrEmpty(General.Map.FilePathName))
			{
				saveFileDialog.FileName = name;
			}
			else
			{
				saveFileDialog.InitialDirectory = Path.GetDirectoryName(General.Map.FilePathName);
				saveFileDialog.FileName = Path.GetDirectoryName(General.Map.FilePathName) + Path.DirectorySeparatorChar + name + ".png";
				tbExportPath.Text = saveFileDialog.FileName;
			}

			cbFullbright.Checked = General.Settings.ReadPluginSetting("imageexportfullbright", true);
			cbApplySectorColors.Checked = General.Settings.ReadPluginSetting("imageexportapplysectorcolors", true);
			cbTransparency.Checked = General.Settings.ReadPluginSetting("imageexporttransparency", false);
			cbBrightmap.Checked = General.Settings.ReadPluginSetting("imageexportbrightmap", false);
			cbTiles.Checked = General.Settings.ReadPluginSetting("imageexporttiles", false);
			cbScale.SelectedIndex = General.Settings.ReadPluginSetting("imageexportscale", 0);
		}

		#endregion

		#region ================== Methods

		public ImageFormat GetImageFormat()
		{
			switch(cbImageFormat.SelectedIndex)
			{
				case 1: // JPG
					return ImageFormat.Jpeg;
				default: // PNG
					return ImageFormat.Png;
			}
		}

		public PixelFormat GetPixelFormat()
		{
			switch(cbPixelFormat.SelectedIndex)
			{
				case 1: // 24 bit
					return PixelFormat.Format24bppRgb;
				case 2: // 16 bit
					return PixelFormat.Format16bppRgb555;
				default: // 32 bit
					return PixelFormat.Format32bppArgb;
			}
		}

		/// <summary>
		/// Starts exporting the image(s). Disables all controls and starts the thread that does the actual exporting.
		/// </summary>
		private void StartExport()
		{
			ICollection<Sector> sectors = General.Map.Map.SelectedSectorsCount == 0 ? General.Map.Map.Sectors : General.Map.Map.GetSelectedSectors(true);

			exporting = true;
			cancelexport = false;

			progress.Maximum = 100; //sectors.Count * (Brightmap ? 2 : 1);
			progress.Value = 0;
			progress.Visible = true;

			lbPhase.Text = "";
			lbPhase.Visible = true;

			foreach (Control c in Controls)
			{
				if (!(c is ProgressBar || c is Label))
					c.Enabled = false;
			}

			export.Enabled = true;
			export.Text = "Cancel";

			ImageExportSettings settings = new ImageExportSettings(Path.GetDirectoryName(FilePath), Path.GetFileNameWithoutExtension(FilePath), Path.GetExtension(FilePath), Floor, Fullbright, ApplySectorColors, Brightmap, Transparency, Tiles, ImageScale, GetPixelFormat(), GetImageFormat());

			exportthread = new Thread(() => RunExport(settings))
			{
				Name = "Image export",
				Priority = ThreadPriority.Normal
			};
			exportthread.Start();
		}

		/// <summary>
		/// Enables all controls. This has to be called when the export is finished (either successfully or unsuccessfully)
		/// </summary>
		/// <param name="ier">Image export result</param>
		private void StopExport(ImageExportResult ier)
		{
			if (this.InvokeRequired)
			{
				CallImageExportResultMethodDeletage d = StopExport;
				this.Invoke(d, ier);
			}
			else
			{
				progress.Visible = false;
				lbPhase.Visible = false;

				foreach (Control c in Controls)
				{
					if (!(c is ProgressBar || c is Label))
						c.Enabled = true;
				}

				export.Text = "Export";

				if (ier == ImageExportResult.OK)
					MessageBox.Show("Export successful.", "Export to image", MessageBoxButtons.OK, MessageBoxIcon.Information);
				else if(ier == ImageExportResult.Canceled)
					MessageBox.Show("Export canceled.", "Export to image", MessageBoxButtons.OK, MessageBoxIcon.Information);
				else if (ier == ImageExportResult.OutOfMemory)
					MessageBox.Show("Exporting failed. There's likely not enough consecutive free memory to create the image. Try a lower color depth or file format", "Export failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
				else if(ier == ImageExportResult.ImageTooBig)
					MessageBox.Show("Exporting failed. The image is likely too big for the current settings. Try a lower color depth or file format", "Export failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}

			exporting = false;
		}

		private bool CheckCancelExport()
		{
			return cancelexport;
		}

		/// <summary>
		/// Shows the current phase in textual form. Is called by the exporter
		/// </summary>
		/// <param name="text"></param>
		private void ShowPhase(string text)
		{
			if (this.InvokeRequired)
			{
				CallStringMethodDeletage d = ShowPhase;
				this.Invoke(d, text);
			}
			else
			{
				lbPhase.Text = text;
			}
		}

		/// <summary>
		/// Adds progress to the progress bar. Is called by the exporter
		/// </summary>
		private void AddProgress()
		{
			if (progress.InvokeRequired)
			{
				CallVoidMethodDeletage d = AddProgress;
				try { progress.Invoke(d); }
				catch (ThreadInterruptedException) { }
			}
			else
			{
				// Just winforms things to make the progress bar animation not lag behind
				int value = progress.Value + 1;
				progress.Value = value;
				progress.Value = value - 1;
				progress.Value = value;
			}
		}

		/// <summary>
		/// Runs the actual exporter
		/// </summary>
		/// <param name="settings">Export settings</param>
		private void RunExport(ImageExportSettings settings)
		{
			ICollection<Sector> sectors = General.Map.Map.SelectedSectorsCount == 0 ? General.Map.Map.Sectors : General.Map.Map.GetSelectedSectors(true);

			ImageExporter exporter = new ImageExporter(sectors, settings, AddProgress, ShowPhase, CheckCancelExport);

			try
			{
				exporter.Export();
			}
			catch (ArgumentException) // Happens if there's not enough consecutive memory to create the file
			{
				StopExport(ImageExportResult.OutOfMemory);
				return;
			}
			catch(ImageExportCanceledException)
			{
				StopExport(ImageExportResult.Canceled);
				return;
			}
			catch(ImageExportImageTooBigException)
			{
				StopExport(ImageExportResult.ImageTooBig);
				return;
			}

			StopExport(ImageExportResult.OK);
		}

		#endregion

		#region ================== Events

		private void browse_Click(object sender, EventArgs e)
		{
			if (saveFileDialog.ShowDialog() == DialogResult.OK)
			{
				tbExportPath.Text = saveFileDialog.FileName;

				string extension = Path.GetExtension(saveFileDialog.FileName);
				
				switch(extension)
				{
					case ".jpg":
						cbImageFormat.SelectedIndex = 1;
						break;
					default:
						cbImageFormat.SelectedIndex = 0;
						break;
				}
			}
		}

		private void close_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;
			this.Close();
		}

		private void export_Click(object sender, EventArgs e)
		{
			if (exporting)
			{
				cancelexport = true;
				export.Enabled = false;
			}
			else
			{
				General.Settings.WritePluginSetting("imageexportfullbright", cbFullbright.Checked);
				General.Settings.WritePluginSetting("imageexportapplysectorcolors", cbApplySectorColors.Checked);
				General.Settings.WritePluginSetting("imageexportbrightmap", cbBrightmap.Checked);
				General.Settings.WritePluginSetting("imageexporttransparency", cbTransparency.Checked);
				General.Settings.WritePluginSetting("imageexporttiles", cbTiles.Checked);
				General.Settings.WritePluginSetting("imageexportscale", cbScale.SelectedIndex);

				// Exporting works like this:
				// In here StartExport() is called
				// StartExport() disables all controls and creates a thread that runs RunExport() in the background; then the StartExport method ends
				// RunExport() creates an instance of ImageExporter and starts the actual export
				// When ImageExporter finishes its job it runs StopExport()
				// StopExport() enables all controls again

				StartExport();
			}
		}

		private void cbImageFormat_SelectedIndexChanged(object sender, EventArgs e)
		{
			string newextension = "";

			switch (cbImageFormat.SelectedIndex)
			{
				case 1: // JPG
					newextension = ".jpg";
					break;
				default: // PNG
					newextension = ".png";
					break;
			}

			tbExportPath.Text = Path.ChangeExtension(tbExportPath.Text, newextension);
		}


		private void ImageExportSettingsForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			// Do not allow closing the form while the export is running
			if (exporting)
				e.Cancel = true;
		}

		#endregion
	}
}
