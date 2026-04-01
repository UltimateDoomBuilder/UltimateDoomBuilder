using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Drawing;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Config;

namespace CodeImp.DoomBuilder.UDBScript
{
	public partial class ScriptOptionsControl : UserControl
	{
		public DataGridView ParametersView { get { return parametersview; } }

		public ScriptOptionsControl()
		{
			InitializeComponent();

			enumscombo.Visible = false;
			browsebutton.Visible = false;
		}

		/// <summary>
		/// Gets an object with all script options with their values. This can then be easily used to access script options by name in the script
		/// </summary>
		/// <returns>Object containing all script options with their values</returns>
		public ExpandoObject GetScriptOptions()
		{
			// We have to jump through some hoops here to be able to access the elements by name
			ExpandoObject eo = new ExpandoObject();
			var options = eo as IDictionary<string, object>;

			foreach (DataGridViewRow row in parametersview.Rows)
			{
				if (row.Tag is ScriptOption)
				{
					ScriptOption so = (ScriptOption)row.Tag;
					options[so.name] = so.typehandler.GetValue();
				}
			}

			return eo;
		}

		private void parametersview_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
		{
			ScriptOption so = (ScriptOption)parametersview.Rows[e.RowIndex].Tag;

			// Enumerable?
			if (so.typehandler.IsEnumerable)
			{
				// Reload type handler so that potentionally changed enums are updated
				so.ReloadTypeHandler();

				// Fill combo with enums
				enumscombo.SelectedItem = null;
				enumscombo.Text = "";
				enumscombo.Items.Clear();
				enumscombo.Items.AddRange(so.typehandler.GetEnumList().ToArray());
				enumscombo.Tag = parametersview.Rows[e.RowIndex];

				// Lock combo to enums?
				//if (so.typehandler.IsLimitedToEnums)
					enumscombo.DropDownStyle = ComboBoxStyle.DropDownList;
				//else
				//	enumscombo.DropDownStyle = ComboBoxStyle.DropDown;

				// Position combobox
				Rectangle cellrect = parametersview.GetCellDisplayRectangle(1, e.RowIndex, false);
				enumscombo.Location = new Point(cellrect.Left, cellrect.Top);
				enumscombo.Width = cellrect.Width;
				int internalheight = cellrect.Height - (enumscombo.Height - enumscombo.ClientRectangle.Height) - 6;
				//General.SendMessage(enumscombo.Handle, General.CB_SETITEMHEIGHT, new IntPtr(-1), new IntPtr(internalheight));

				// Select the value of this field (for DropDownList style combo)
				foreach (EnumItem i in enumscombo.Items)
				{
					// Matches?
					if (string.Compare(i.Title, so.typehandler.GetStringValue(), StringComparison.OrdinalIgnoreCase) == 0)
					{
						// Select this item
						enumscombo.SelectedItem = i;
						break;
					}
				}

				// Nothing found, try the values
				if (enumscombo.SelectedItem == null)
				{
					foreach (EnumItem i in enumscombo.Items)
					{
						// Matches?
						if (string.Compare(i.Value, so.typehandler.GetStringValue(), StringComparison.OrdinalIgnoreCase) == 0)
						{
							// Select this item
							enumscombo.SelectedItem = i;
							break;
						}
					}
				}

				// Put the display text in the text (for DropDown style combo)
				enumscombo.Text = so.typehandler.GetStringValue();

				// Show combo
				enumscombo.Show();
			}
		}

		private void parametersview_CellClick(object sender, DataGridViewCellEventArgs e)
		{
			ApplyEnums(true);

			if (e.ColumnIndex == 1)
				parametersview.BeginEdit(true);
		}

		public void EndEdit()
		{
			ApplyEnums(true);

			parametersview.EndEdit();

			parametersview.Focus();
		}

		public void EndAddingOptions()
		{
			UpdateBrowseButton();
		}

		// This hides the browse button
		private void HideBrowseButton()
		{
			browsebutton.Visible = false;
		}

		// This updates the button
		private void UpdateBrowseButton()
		{
			// Any row selected?
			if (parametersview.SelectedRows.Count > 0)
			{
				// Get selected row
				DataGridViewRow row = parametersview.SelectedRows[0];

				// Not the new row and FieldsEditorRow available?
				if (row != null && row.Tag != null)
				{
					ScriptOption so = (ScriptOption)row.Tag;

					// Browse button available for this type?
					if (so.typehandler.IsBrowseable && !so.typehandler.IsEnumerable)
					{
						Rectangle cellrect = parametersview.GetCellDisplayRectangle(1, row.Index, false);

						// Show button
						enumscombo.Visible = false;
						browsebutton.Image = so.typehandler.BrowseImage;
						browsebutton.Location = new Point(cellrect.Right - browsebutton.Width, cellrect.Top);
						browsebutton.Height = cellrect.Height;
						browsebutton.Visible = true;
					}
					else
					{
						HideBrowseButton();
					}
				}
				else
				{
					HideBrowseButton();
				}
			}
			else
			{
				HideBrowseButton();
			}
		}

		// This applies the contents of the enums combobox and hides (if opened)
		private void ApplyEnums(bool hide)
		{
			// Enums combobox shown?
			if (enumscombo.Visible && enumscombo.Tag is DataGridViewRow)
			{
				// Get the row
				DataGridViewRow row = (DataGridViewRow)enumscombo.Tag;

				row.Cells["Value"].Value = enumscombo.Text;

				ScriptOption so = (ScriptOption)row.Tag;

				/*if (so.typehandler.IsEnumerable)
				{
					EnumList list = so.typehandler.GetEnumList();
					so.typehandler.SetValue(so.typehandler.GetEnumList().GetByEnumIndex(enumscombo.Text).Value);
				}
				else */
				{
					so.typehandler.SetValue(enumscombo.Text);
				}

				// Take the selected value and apply it
				//ApplyValue(frow, enumscombo.Text);

				// Updated
				//frow.CellChanged();
			}

			if (hide)
			{
				// Hide combobox
				enumscombo.Tag = null;
				enumscombo.Visible = false;
				enumscombo.Items.Clear();
			}
		}

		private void enumscombo_Validating(object sender, CancelEventArgs e)
		{
			ApplyEnums(false);
		}

		// Mouse up event
		private void parametersview_MouseUp(object sender, MouseEventArgs e)
		{
			// Focus to enums combobox when visible
			if (enumscombo.Visible)
			{
				enumscombo.Focus();
				enumscombo.SelectAll();
			}
		}

		private void parametersview_SelectionChanged(object sender, EventArgs e)
		{
			browsebutton.Visible = false;
			ApplyEnums(true);

			// Update button
			UpdateBrowseButton();
		}

		private void browsebutton_Click(object sender, EventArgs e)
		{
			// Any row selected?
			if (parametersview.SelectedRows.Count > 0)
			{
				// Get selected row
				DataGridViewRow row = parametersview.SelectedRows[0];

				ScriptOption so = (ScriptOption)row.Tag;

				so.typehandler.Browse(this.ParentForm);
				row.Cells["Value"].Value = so.typehandler.GetStringValue();

				if (so.typehandler.DynamicImage) browsebutton.Image = so.typehandler.BrowseImage;
				parametersview.Focus();
			}
		}

		private void parametersview_CellEndEdit(object sender, DataGridViewCellEventArgs e)
		{
			ScriptOption so = (ScriptOption)parametersview.Rows[e.RowIndex].Tag;

			so.typehandler.SetValue(parametersview.Rows[e.RowIndex].Cells["Value"].Value);
			so.value = parametersview.Rows[e.RowIndex].Cells["Value"].Value;
		}

		private void parametersview_Resize(object sender, EventArgs e)
		{
			// Update the browse button so that it stays at the correct position when the control is resized
			UpdateBrowseButton();
		}

		/// <summary>
		/// Makes sure the edited cell value is valid. Also stores the value in the editor's configuration file so that it is remembered
		/// </summary>
		/// <param name="sender">the sender</param>
		/// <param name="e">the event</param>
		private void parametersview_CellValueChanged(object sender, DataGridViewCellEventArgs e)
		{
			if (e.RowIndex < 0 || e.ColumnIndex == 0 || parametersview.Rows[e.RowIndex].Tag == null)
				return;

			ScriptOption so = (ScriptOption)parametersview.Rows[e.RowIndex].Tag;

			object newvalue = parametersview.Rows[e.RowIndex].Cells["Value"].Value;

			// If the new value is empty reset it to the default value. Don't fire this event again, though
			if (newvalue == null || string.IsNullOrWhiteSpace(newvalue.ToString()))
			{
				newvalue = so.defaultvalue;
				parametersview.CellValueChanged -= parametersview_CellValueChanged;
				parametersview.Rows[e.RowIndex].Cells["Value"].Value = newvalue.ToString();
				parametersview.CellValueChanged += parametersview_CellValueChanged;
			}

			so.typehandler.SetValue(newvalue);

			so.value = newvalue;

			// Make the text lighter if it's the default value, and store the setting in the config file if it's not the default
			if (so.value.ToString() == so.defaultvalue.ToString())
			{
				parametersview.Rows[e.RowIndex].Cells["Value"].Style.ForeColor = SystemColors.GrayText;
			}
			else
			{
				parametersview.Rows[e.RowIndex].Cells["Value"].Style.ForeColor = SystemColors.WindowText;
			}
		}
	}
}
