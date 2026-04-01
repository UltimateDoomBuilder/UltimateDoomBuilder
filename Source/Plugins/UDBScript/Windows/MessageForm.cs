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
using CodeImp.DoomBuilder.Windows;

#endregion

namespace CodeImp.DoomBuilder.UDBScript
{
	public partial class MessageForm : DelayedForm
	{
		#region ================== Constructor

		public MessageForm(string option1text, string option2text, string message)
		{
			InitializeComponent();

			if (option2text == null)
			{
				btnButton1.Text = option1text;
				btnButton1.DialogResult = DialogResult.OK;
				btnButton2.Visible = false;
			}
			else
			{
				btnButton1.Text = option2text;
				btnButton1.DialogResult = DialogResult.Cancel;
				btnButton2.Text = option1text;
				btnButton2.DialogResult = DialogResult.OK;
			}

			tbMessage.Text = message.Replace("\n", Environment.NewLine);
		}

		#endregion

		#region ================== Events

		private void button_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void btnAbortScript_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show("Are you sure you want to abort the script?", "Abort script", MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				DialogResult = DialogResult.Abort;
				Close();
			}
		}

		#endregion
	}
}
