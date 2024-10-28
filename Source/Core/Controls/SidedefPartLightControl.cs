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

#region ================== Namespaces

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Types;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.VisualModes;
using CodeImp.DoomBuilder.Windows;

#endregion

namespace CodeImp.DoomBuilder.Controls
{
	public partial class SidedefPartLightControl : UserControl
	{
		#region ================== Variables

		private string fieldname;
		private string fieldabsolutename;
		private List<Sidedef> sidedefs;
		private Dictionary<Sidedef, int> originallight;
		private Dictionary<Sidedef, bool> originalabsolute;
		private bool preventchanges;

		#endregion

		#region ================== Constructors

		public SidedefPartLightControl()
		{
			InitializeComponent();
		}

		#endregion

		#region ================== Methods

		/// <summary>
		/// Sets up the control for the specified geometry type.
		/// </summary>
		/// <param name="geometrytype">Geometry type to set up the control with</param>
		public void Setup(VisualGeometryType geometrytype)
		{
			fieldname = string.Empty;
			fieldabsolutename = string.Empty;
			sidedefs = new List<Sidedef>();
			originallight = new Dictionary<Sidedef, int>();
			originalabsolute = new Dictionary<Sidedef, bool>();

			// Do not trigger the events that usually fire when textboxes or checkboxes are changed
			preventchanges = true;

			switch (geometrytype)
			{
				case VisualGeometryType.WALL_UPPER:
					fieldname = "light_top";
					fieldabsolutename = "lightabsolute_top";
					lbLight.Text = "Upper brightness:";
					break;
				case VisualGeometryType.WALL_MIDDLE:
				case VisualGeometryType.WALL_MIDDLE_3D:
					fieldname = "light_mid";
					fieldabsolutename = "lightabsolute_mid";
					lbLight.Text = "Middle brightness:";
					break;
				case VisualGeometryType.WALL_LOWER:
					fieldname = "light_bottom";
					fieldabsolutename = "lightabsolute_bottom";
					lbLight.Text = "Lower brightness:";
					break;
				default:
					throw new NotImplementedException("Unsupported geometry type: " + Enum.GetName(typeof(VisualGeometryType), geometrytype));
			}
		}

		/// <summary>
		/// Sets the light value and absolute stats of the control.
		/// </summary>
		/// <param name="sidedef">Sidedef to use the values of</param>
		/// <param name="first">If this is the first sidedef</param>
		public void SetValues(Sidedef sidedef, bool first)
		{
			if (!sidedefs.Contains(sidedef))
				sidedefs.Add(sidedef);

			originallight[sidedef] = sidedef.Fields.GetValue(fieldname, 0);
			originalabsolute[sidedef] = sidedef.Fields.GetValue(fieldabsolutename, false);

			string lightvalue = originallight[sidedef].ToString();
			bool isabsolute = sidedef.Fields.GetValue(fieldabsolutename, false);

			if (first)
			{
				light.Text = lightvalue;
				cbAbsolute.Checked = isabsolute;
			}
			else
			{
				if (light.Text != lightvalue)
					light.Text = string.Empty;

				if (cbAbsolute.Checked != isabsolute)
				{
					cbAbsolute.ThreeState = true;
					cbAbsolute.CheckState = CheckState.Indeterminate;
				}
			}
		}

		/// <summary>
		/// Finalize the control's setup, setting visibility of child controls and enable state
		/// </summary>
		public void FinalizeSetup()
		{
			reset.Visible = (cbAbsolute.CheckState != CheckState.Unchecked || light.GetResult(0) != 0);

			preventchanges = false;
		}

		#endregion

		#region ================== Events

		private void reset_Click(object sender, EventArgs e)
		{
			light.Text = "0";
			cbAbsolute.Checked = false;
			reset.Visible = false;
		}

		private void light_WhenTextChanged(object sender, EventArgs e)
		{
			if (preventchanges)
				return;

			((LinedefEditFormUDMF)ParentForm).MakeUndo();

			// Reset the increment step for +++/---
			light.ResetIncrementStep();

			if (string.IsNullOrEmpty(light.Text))
			{
				// Text is empty, use each sidedef's original light value
				foreach (Sidedef sd in sidedefs)
				{
					if (sd == null || sd.IsDisposed)
						continue;

					UniFields.SetInteger(sd.Fields, fieldname, originallight[sd]);
				}
			}
			else
			{
				foreach (Sidedef sd in sidedefs)
				{
					if (sd == null || sd.IsDisposed)
						continue;

					bool absolute = false;

					switch (cbAbsolute.CheckState)
					{
						case CheckState.Checked:
							absolute = true;
							break;
						case CheckState.Indeterminate:
							absolute = sd.Fields.GetValue(fieldabsolutename, false);
							break;
					}

					int value = General.Clamp(light.GetResult(originallight[sd]), absolute ? 0 : -255, 255);
					UniFields.SetInteger(sd.Fields, fieldname, value);
				}
			}

			reset.Visible = (cbAbsolute.CheckState != CheckState.Unchecked || light.Text != "0");
			General.Map.IsChanged = true;

			((LinedefEditFormUDMF)ParentForm).ValuesChangedExternal();
		}

		private void cbAbsolute_CheckedChanged(object sender, EventArgs e)
		{
			if (preventchanges)
				return;

			((LinedefEditFormUDMF)ParentForm).MakeUndo();

			if (cbAbsolute.Checked)
			{
				foreach(Sidedef sd in sidedefs)
				{
					sd.Fields[fieldabsolutename] = new UniValue(UniversalType.Boolean, true);
				}
			}
			else if(cbAbsolute.CheckState == CheckState.Indeterminate)
			{
				foreach(Sidedef sd in sidedefs)
				{
					if (originalabsolute[sd])
						sd.Fields[fieldabsolutename] = new UniValue(UniversalType.Boolean, true);
					else if (sd.Fields.ContainsKey(fieldabsolutename))
						sd.Fields.Remove(fieldabsolutename);
				}
			}
			else
			{
				foreach (Sidedef sd in sidedefs)
					if (sd.Fields.ContainsKey(fieldabsolutename))
						sd.Fields.Remove(fieldabsolutename);
			}

			((LinedefEditFormUDMF)ParentForm).ValuesChangedExternal();
		}

		#endregion
	}
}
