#region ================== Copyright (c) 2023 Boris Iwanski

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

using CodeImp.DoomBuilder.Windows;
using System;

namespace CodeImp.DoomBuilder.BuilderModes.Interface
{
	public partial class ChangeMapElementIndexForm : DelayedForm
	{
		private int currentindex;
		private int maxindex;

		public ChangeMapElementIndexForm(string typetitle, int currentindex, int maxindex)
		{
			InitializeComponent();

			Text = $"Change {typetitle} index";

			this.currentindex = currentindex;
			this.maxindex = maxindex;

			lbCurrentIndex.Text = currentindex.ToString();
			lbMaximumIndex.Text = maxindex.ToString();
			bntNewIndex.Text = "0";
		}

		public int GetNewIndex()
		{
			return bntNewIndex.GetResult(0);
		}

		private void bntNewIndex_WhenTextChanged(object sender, EventArgs e)
		{
			int targetindex = bntNewIndex.GetResult(0);

			if(targetindex > maxindex)
			{
				toolTip.SetToolTip(pbWarning, "The new index is too high");
				btnOk.Enabled = false;
				pbWarning.Visible = true;
			}
			else if(targetindex == currentindex)
			{
				toolTip.SetToolTip(pbWarning, "The new and old indices are the same");
				btnOk.Enabled = false;
				pbWarning.Visible = true;
			}
			else
			{
				btnOk.Enabled = true;
				pbWarning.Visible = false;
			}
		}
	}
}
