#region ================== Copyright (c) 2021 Boris Iwanski

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

#region ================== Namespaces

using System;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Config;

#endregion

namespace CodeImp.DoomBuilder.Controls
{
	public partial class ExternalCommandControl : UserControl
	{
		#region ================== Constructors

		public ExternalCommandControl()
		{
			InitializeComponent();
		}

		#endregion

		#region ================== Methods

		/// <summary>
		/// Fills the controls with the settings from the ExternalCommandSettings.
		/// </summary>
		/// <param name="ecs">The settings</param>
		public void Setup(ExternalCommandSettings ecs)
		{
			tbFolder.Text = ecs.WorkingDirectory;
			tbCommand.Text = ecs.Commands;
			cbAutoclose.Checked = ecs.AutoCloseOnSuccess;
			cbExitCode.Checked = ecs.ExitCodeIsError;
			cbStdErr.Checked = ecs.StdErrIsError;
		}

		/// <summary>
		/// Returns the external command settings.
		/// </summary>
		/// <returns>The external command settings</returns>
		public ExternalCommandSettings GetSettings()
		{
			ExternalCommandSettings ecs = new ExternalCommandSettings();
			ecs.WorkingDirectory = tbFolder.Text.Trim();
			ecs.Commands = tbCommand.Text.Trim();
			ecs.AutoCloseOnSuccess = cbAutoclose.Checked;
			ecs.ExitCodeIsError = cbExitCode.Checked;
			ecs.StdErrIsError = cbStdErr.Checked;

			return ecs;
		}

		#endregion

		#region ================== Events

		private void button1_Click(object sender, EventArgs e)
		{
			FolderSelectDialog dirdialog = new FolderSelectDialog();
			dirdialog.Title = "Select base folder";
			dirdialog.InitialDirectory = tbFolder.Text;

			if (dirdialog.ShowDialog(this.Handle))
			{
				tbFolder.Text = dirdialog.FileName;
			}
		}

		#endregion
	}
}
