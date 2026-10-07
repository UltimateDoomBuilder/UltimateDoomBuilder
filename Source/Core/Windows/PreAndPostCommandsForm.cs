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

namespace CodeImp.DoomBuilder.Windows
{
	public partial class PreAndPostCommandsForm : DelayedForm
	{
		#region ================== Constructors

		public PreAndPostCommandsForm(ExternalCommandSettings reloadresourceprecommand, ExternalCommandSettings reloadresourcepostcommand, ExternalCommandSettings testprecommand, ExternalCommandSettings testpostcommand)
		{
			InitializeComponent();

			reloadpre.Setup(reloadresourceprecommand);
			reloadpost.Setup(reloadresourcepostcommand);
			testpre.Setup(testprecommand);
			testpost.Setup(testpostcommand);
		}

		#endregion

		#region ================== Methods

		public ExternalCommandSettings GetReloadResourcePreCommand()
		{
			return reloadpre.GetSettings();
		}

		public ExternalCommandSettings GetReloadResourcePostCommand()
		{
			return reloadpost.GetSettings();
		}

		public ExternalCommandSettings GetTestPreCommand()
		{
			return testpre.GetSettings();
		}

		public ExternalCommandSettings GetTestPostCommand()
		{
			return testpost.GetSettings();
		}

		#endregion

		#region ================== Events

		private void btnCancel_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
			Close();
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.OK;
			Close();
		}

		#endregion
	}
}
