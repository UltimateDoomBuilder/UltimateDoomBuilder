
#region ================== Copyright (c) 2007 Pascal vd Heiden

/*
 * Copyright (c) 2007 Pascal vd Heiden, www.codeimp.com
 * This program is released under GNU General Public License
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 */

#endregion

#region ================== Namespaces

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Types;

#endregion

namespace CodeImp.DoomBuilder.Controls
{
	/// <summary>
	/// Control to view and/or change custom UDMF fields.
	/// </summary>
	public partial class FieldsEditorControl : UserControl
	{
		#region ================== Constants

		// Constants
		private const string ADD_FIELD_TEXT = "   (click to add custom field)";
		private const string FIELD_PREFIX_SUGGESTION = "user_";

		#endregion
		
		#region ================== Variables

		public delegate void SingleFieldNameEvent(string fieldname);
		public delegate void DualFieldNameEvent(string oldname, string newname);

		// Events
		public event SingleFieldNameEvent OnFieldInserted;
		public event DualFieldNameEvent OnFieldNameChanged;
		public event SingleFieldNameEvent OnFieldValueChanged;
		public event SingleFieldNameEvent OnFieldTypeChanged;
		public event SingleFieldNameEvent OnFieldDeleted;
		public event SingleFieldNameEvent OnFieldUndefined;

		// Variables
		private string elementname;
		private string lasteditfieldname;
		private bool autoinsertuserprefix;
		private Dictionary<string, UniversalType> uifields;//mxd
		private bool showfixedfields = true; //mxd
		
		#endregion

		#region ================== Properties

		public bool AllowInsert { get { return fieldslist.AllowUserToAddRows; } set { fieldslist.AllowUserToAddRows = value; SetupNewRowStyle(); } }
		public bool AutoInsertUserPrefix { get { return autoinsertuserprefix; } set { autoinsertuserprefix = value; } }
		public int PropertyColumnWidth { get { return fieldname.Width; } set { fieldname.Width = value; UpdateValueColumn(); UpdateBrowseButton(); } }
		public int TypeColumnWidth { get { return fieldtype.Width; } set { fieldtype.Width = value; UpdateValueColumn(); UpdateBrowseButton(); } }
		public bool PropertyColumnVisible { get { return fieldname.Visible; } set { fieldname.Visible = value; UpdateValueColumn(); UpdateBrowseButton(); } }
		public bool TypeColumnVisible { get { return fieldtype.Visible; } set { fieldtype.Visible = value; UpdateValueColumn(); UpdateBrowseButton(); } }
		public bool ValueColumnVisible { get { return fieldvalue.Visible; } set { fieldvalue.Visible = value; UpdateValueColumn(); UpdateBrowseButton(); } }
		public bool ShowFixedFields {get { return showfixedfields; } set { showfixedfields = value; UpdateFixedFieldsVisibility(); } } //mxd

		#endregion

		#region ================== Constructor

		// Constructor
		public FieldsEditorControl()
		{
			InitializeComponent();
			CodeImp.DoomBuilder.General.ApplyDataGridViewFix(fieldslist);
			autoinsertuserprefix = true;
			enumscombo.Location = new Point(-1000, 1);
		}

		#endregion

		#region ================== Setup / Apply

		// This sets up the control
		public void Setup(string elementname)
		{
			// Keep element name
			this.elementname = elementname;

			//mxd. Get proper UIFields
			uifields = General.Map.FormatInterface.UIFields[General.Map.FormatInterface.GetElementType(elementname)];
			
			// Make types list
			fieldtype.Items.Clear();
			fieldtype.Items.AddRange(General.Types.GetCustomUseAttributes());
		}
		
		// This applies last sort order
		private void Sort()
		{
			// Sort
			int sortcolumn = General.Settings.ReadSetting("customfieldssortcolumn", 0);
			int sortorder = General.Settings.ReadSetting("customfieldssortorder", (int)ListSortDirection.Ascending);

			switch(sortorder)
			{
				case (int)SortOrder.Ascending:
					fieldslist.Sort(fieldslist.Columns[sortcolumn], ListSortDirection.Ascending);
					break;
				case (int)SortOrder.Descending:
					fieldslist.Sort(fieldslist.Columns[sortcolumn], ListSortDirection.Descending);
					break;
			}
		}
		
		// This adds a list of fixed fields (in undefined state)
		public void ListFixedFields(List<UniversalFieldInfo> list)
		{
			// Add all fields
			foreach(UniversalFieldInfo uf in list) 
			{
				//if(uifields.ContainsKey(uf.Name)) continue; //mxd
				// Only add fields that are not managed by the UI
				if(!uf.Managed)
					fieldslist.Rows.Add(new FieldsEditorRow(fieldslist, uf));
			}

			// Sort fields
			Sort();

			// Update new row
			SetupNewRowStyle();
		}
		
		// Use this in case you don't want the fixed fields
		public void ListNoFixedFields()
		{
			// Update new row
			SetupNewRowStyle();
		}
		
		// Clear all fields
		public void ClearFields()
		{
			// Trash rows
			fieldslist.Rows.Clear();
			
			// Update new row
			SetupNewRowStyle();
		}
		
		// This sets up the fields and values from a UniFields object
		// When first is true, the values are applied unconditionally
		// When first is false, the values in the grid are erased when
		// they differ from the given values (for multiselection)
		public void SetValues(UniFields fromfields, bool first)
		{
			// Go for all the fields
			foreach(KeyValuePair<string, UniValue> f in fromfields)
			{
				if(uifields.ContainsKey(f.Key)) continue; //mxd
				
				// Go for all rows
				bool foundrow = false;
				bool skiprow = false; //mxd
				foreach(DataGridViewRow row in fieldslist.Rows)
				{
					// Row is a field?
					if(row is FieldsEditorRow)
					{
						FieldsEditorRow frow = row as FieldsEditorRow;
						
						// Row name matches with field
						if(frow.Name == f.Key)
						{
							//mxd. User vars are set separately
							if(frow.RowType == FieldsEditorRowType.USERVAR)
							{
								skiprow = true;
								break;
							}
							
							// First time?
							if(first)
							{
								// Set type when row is not fixed
								if(frow.RowType == FieldsEditorRowType.DYNAMIC) frow.ChangeType(f.Value.Type);

								// Apply value of field to row
								frow.Define(f.Value.Value);
							}
							else
							{
								// Check if the value is different
								if(!frow.TypeHandler.GetValue().Equals(f.Value.Value))
								{
									// Clear the value in the row
									frow.Define(f.Value.Value);
									frow.Clear();
								}
							}
							
							// Done
							foundrow = true;
							break;
						}
					}
				}

				//mxd. User vars are set separately
				if(skiprow) continue;
				
				// Row not found?
				if(!foundrow)
				{
					// Make new row
					FieldsEditorRow frow = new FieldsEditorRow(fieldslist, f.Key, f.Value.Type, f.Value.Value, false);
					fieldslist.Rows.Insert(fieldslist.Rows.Count - 1, frow);
					
					// When not the first, clear the field because the others did not define this one
					if(!first) frow.Clear();
				}
			}
			
			// Now check for rows that the givens fields do NOT have
			foreach(DataGridViewRow row in fieldslist.Rows)
			{
				// Row is a field?
				if(row is FieldsEditorRow)
				{
					FieldsEditorRow frow = row as FieldsEditorRow;

					// Is this row defined previously?
					if(frow.IsDefined)
					{
						// Check if this row can not be found in the fields at all
						if(!fromfields.ContainsKey(frow.Name))
						{
							// It is not defined in these fields, clear the value
							frow.Clear();
						}
					}
				}
			}
			
			// Sort fields
			Sort();
		}

		//mxd
		public void SetUserVars(Dictionary<string, UniversalType> vars, Dictionary<string, object> defaults, UniFields fromfields, bool first)
		{
			foreach(KeyValuePair<string, UniversalType> group in vars)
			{
                // Go for all rows
                bool foundrow = false;
				TypeHandler vartype = General.Types.GetFieldHandler((int)group.Value, 0);

                object defaultvalue;
                if (defaults.ContainsKey(group.Key))
                    defaultvalue = defaults[group.Key];
                else defaultvalue = vartype.GetDefaultValue();

                object value = fromfields.ContainsKey(group.Key) ? fromfields[group.Key].Value : defaultvalue;
				
				foreach(DataGridViewRow row in fieldslist.Rows)
				{
					// Row is a field?
					if(row is FieldsEditorRow)
					{
						FieldsEditorRow frow = row as FieldsEditorRow;
						
						// Row name matches with user var?
						if(frow.RowType == FieldsEditorRowType.USERVAR && frow.Name == group.Key)
						{
							// First time?
							if(first)
							{
								frow.Define(value);
							}
							// Check if the value is different
							else if(!frow.TypeHandler.GetValue().Equals(value))
							{
								// Clear the value in the row
								frow.Define(value);
								frow.Clear();
							}
							
							// Done
							foundrow = true;
							break;
						}
					}
				}
				
				// Row not found?
				if(!foundrow)
				{
                    // Make new row
                    // [ZZ] 24.07.08: add support for custom defaults for user vars. ZScript only for now.
					FieldsEditorRow frow = new FieldsEditorRow(fieldslist, group.Key, (int)group.Value, defaultvalue, true);
					if(!value.Equals(defaultvalue)) frow.Define(value);
					fieldslist.Rows.Insert(fieldslist.Rows.Count - 1, frow);
				}
			}
			
			// Now check for rows that the givens fields do NOT have
			foreach(DataGridViewRow row in fieldslist.Rows)
			{
				// Row is a field?
				if(row is FieldsEditorRow)
				{
					FieldsEditorRow frow = row as FieldsEditorRow;
					
					// Don't undefine user var rows defined by other actor types
					if(frow.RowType == FieldsEditorRowType.USERVAR || vars.ContainsKey(frow.Name)) continue;

					// Is this row defined previously?
					if(frow.IsDefined)
					{
						// Check if this row can not be found in the fields at all
						if(!fromfields.ContainsKey(frow.Name))
						{
							// It is not defined in these fields, undefine the value
							frow.Undefine();
						}
					}
				}
			}

			// Sort fields
			Sort();
		}
		
		// This applies the current fields to a UniFields object
		public void Apply(UniFields tofields)
		{
			tofields.BeforeFieldsChange();
			
			// Go for all the fields
			UniFields tempfields = new UniFields(tofields);
			foreach(KeyValuePair<string, UniValue> f in tempfields) 
			{
				if(uifields.ContainsKey(f.Key)) continue; //mxd
				
				// Go for all rows
				bool foundrow = false;
				bool skiprow = false; //mxd
				foreach(DataGridViewRow row in fieldslist.Rows)
				{
					// Row is a field and matches field name?
					if((row is FieldsEditorRow) && (row.Cells[0].Value.ToString() == f.Key))
					{
						FieldsEditorRow frow = row as FieldsEditorRow;

						//mxd. User vars are stored separately
						if(frow.RowType == FieldsEditorRowType.USERVAR)
						{
							skiprow = true;
							break;
						}

						// Field is defined?
						if(frow.IsDefined)
						{
							foundrow = true;
							break;
						}
					}
				}

				//mxd. User vars are stored separately
				if(skiprow) continue;

				// No such row?
				if(!foundrow)
				{
					// Remove the definition from the fields
					tofields.Remove(f.Key);
				}
			}
			
			// Go for all rows
			foreach(DataGridViewRow row in fieldslist.Rows)
			{
				// Row is a field?
				if(row is FieldsEditorRow)
				{
					FieldsEditorRow frow = row as FieldsEditorRow;
					
					// Field is defined and not empty?
					if(frow.RowType != FieldsEditorRowType.USERVAR && frow.IsDefined && !frow.IsEmpty)
					{
						// Apply field
						object oldvalue = null;
						if(tofields.ContainsKey(frow.Name)) oldvalue = tofields[frow.Name].Value;
						tofields[frow.Name] = new UniValue(frow.TypeHandler.Index, frow.GetResult(oldvalue));

						// Custom row?
						if(frow.RowType == FieldsEditorRowType.DYNAMIC)
						{
							// Write type to map configuration
							General.Map.Options.SetUniversalFieldType(elementname, frow.Name, frow.TypeHandler.Index);
						}
					}
				}
			}
		}

		//mxd
		public void ApplyUserVars(Dictionary<string, UniversalType> vars, Dictionary<string, object> vardefaults, UniFields tofields)
		{
			// Apply user variables when target map element contains user var definition and the value is not default
			foreach(DataGridViewRow row in fieldslist.Rows)
			{
				// Row is a field?
				if(row is FieldsEditorRow)
				{
					FieldsEditorRow frow = row as FieldsEditorRow;
					if(frow.RowType != FieldsEditorRowType.USERVAR || !vars.ContainsKey(frow.Name)) continue;

					object oldvalue = (tofields.ContainsKey(frow.Name) ? tofields[frow.Name].Value : null);
					object newvalue = frow.GetResult(oldvalue);

					// Skip field when mixed values
					if(newvalue == null) continue;

					object typedefault = frow.TypeHandler.GetDefaultValue();
					object userdefault = vardefaults.ContainsKey(frow.Name) ? vardefaults[frow.Name] : typedefault;

					// Remove field, but only if the type's default value is the same as the user var's default value
					if (newvalue.Equals(typedefault) && typedefault.Equals(userdefault))
					{
						if(tofields.ContainsKey(frow.Name)) tofields.Remove(frow.Name);
					}
					// Add field
					else if(!newvalue.Equals(oldvalue))
					{
						tofields[frow.Name] = new UniValue(frow.TypeHandler.Index, newvalue);
					}
				}
			}
		}

		#endregion

		#region ================== Events

		// Column header clicked
		private void fieldslist_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
		{
			// Save sort order
			if(fieldslist.SortedColumn != null)
			{
				int sortcolumn = fieldslist.SortedColumn.Index;
				int sortorder = (int)fieldslist.SortOrder;
				General.Settings.WriteSetting("customfieldssortcolumn", sortcolumn);
				General.Settings.WriteSetting("customfieldssortorder", sortorder);
			}
			
			// Stop any cell editing
			ApplyEnums(true);
			fieldslist.EndEdit();
			HideBrowseButton();
		}
		
		// Resized
		private void FieldsEditorControl_Resize(object sender, EventArgs e)
		{
			// Rearrange controls
			fieldslist.Size = this.ClientSize;
			UpdateValueColumn();
			UpdateBrowseButton();
		}

		// Layout change
		private void FieldsEditorControl_Layout(object sender, LayoutEventArgs e)
		{
			FieldsEditorControl_Resize(sender, EventArgs.Empty);
		}

		// Cell clicked
		private void fieldslist_CellClick(object sender, DataGridViewCellEventArgs e)
		{
			ApplyEnums(true);

			// Anything selected
			if(fieldslist.SelectedCells.Count > 0)
			{
				if(e.RowIndex == fieldslist.NewRowIndex)
					fieldslist.BeginEdit(false);
				else if(e.ColumnIndex > 0)
					fieldslist.BeginEdit(true);
			}
		}

		// Cell doubleclicked
		private void fieldslist_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
		{
			FieldsEditorRow frow = null;
			
			// Anything selected
			if(fieldslist.SelectedRows.Count > 0 && e.RowIndex > -1)
			{
				// Get the row
				DataGridViewRow row = fieldslist.Rows[e.RowIndex];
				if(row is FieldsEditorRow) frow = row as FieldsEditorRow;
				
				// First column?
				if(e.ColumnIndex == 0)
				{
					// Dynamic field?
					if((frow != null) && frow.RowType == FieldsEditorRowType.DYNAMIC)
					{
						lasteditfieldname = frow.Name;
						fieldslist.CurrentCell = fieldslist.SelectedRows[0].Cells[0];
						fieldslist.CurrentCell.ReadOnly = false;

						if((e.RowIndex == fieldslist.NewRowIndex) ||
						   frow.Name.StartsWith(FIELD_PREFIX_SUGGESTION, StringComparison.OrdinalIgnoreCase))
							fieldslist.BeginEdit(false);
						else
							fieldslist.BeginEdit(true);
					}
				}
			}
		}
		
		// User deletes a row
		private void fieldslist_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
		{
			// Get the row
			FieldsEditorRow row = e.Row as FieldsEditorRow;
			if(row == null) return;
			
			// Fixed/uservar field?
			if(row.RowType == FieldsEditorRowType.FIXED || row.RowType == FieldsEditorRowType.USERVAR)
			{
				// Just undefine the field
				row.Undefine();
				e.Cancel = true;

				if(OnFieldUndefined != null) OnFieldUndefined(row.Name);
			}
			else
			{
				if(OnFieldDeleted != null) OnFieldDeleted(row.Name);
			}
		}

		// User selects a cell for editing
		private void fieldslist_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
		{
			// Field name cell?
			if(e.ColumnIndex == 0)
			{
				// New row index?
				if(e.RowIndex == fieldslist.NewRowIndex)
				{
					// Remove all text
					fieldslist.Rows[e.RowIndex].Cells[0].Style.ForeColor = SystemColors.WindowText;
					fieldslist.Rows[e.RowIndex].Cells[0].Value = (autoinsertuserprefix ? FIELD_PREFIX_SUGGESTION : string.Empty);
				}
			}
			// Value cell?
			else if(e.ColumnIndex == 2)
			{
				// Get the row
				DataGridViewRow row = fieldslist.Rows[e.RowIndex];
				if(row is FieldsEditorRow)
				{
					// Get specializedrow
					FieldsEditorRow frow = row as FieldsEditorRow;

					// Enumerable?
					if(frow.TypeHandler.IsEnumerable)
					{
						// Fill combo with enums
						enumscombo.SelectedItem = null;
						enumscombo.Text = "";
						enumscombo.Items.Clear();
						enumscombo.Items.AddRange(frow.TypeHandler.GetEnumList().ToArray());
						enumscombo.Tag = frow;
						
						// Lock combo to enums?
						if(frow.TypeHandler.IsLimitedToEnums)
							enumscombo.DropDownStyle = ComboBoxStyle.DropDownList;
						else
							enumscombo.DropDownStyle = ComboBoxStyle.DropDown;
						
						// Position combobox
						Rectangle cellrect = fieldslist.GetCellDisplayRectangle(2, row.Index, false);
						enumscombo.Location = new Point(cellrect.Left, cellrect.Top);
						enumscombo.Width = cellrect.Width;
						int internalheight = cellrect.Height - (enumscombo.Height - enumscombo.ClientRectangle.Height) - 6;
						General.SetComboBoxItemHeight(enumscombo, internalheight);
						
						// Select the value of this field (for DropDownList style combo)
						foreach(EnumItem i in enumscombo.Items)
						{
							// Matches?
							if(string.Compare(i.Title, frow.TypeHandler.GetStringValue(), StringComparison.OrdinalIgnoreCase) == 0)
							{
								// Select this item
								enumscombo.SelectedItem = i;
								break; //mxd
							}
						}

						// Put the display text in the text (for DropDown style combo)
						enumscombo.Text = frow.TypeHandler.GetStringValue();
						
						// Show combo
						enumscombo.Show();
					}
				}
			}
		}

		// Done editing cell contents
		private void fieldslist_CellEndEdit(object sender, DataGridViewCellEventArgs e)
		{
			FieldsEditorRow frow = null;

			// Get the row
			DataGridViewRow row = fieldslist.Rows[e.RowIndex];
			if(row is FieldsEditorRow) frow = row as FieldsEditorRow;
			
			// Renaming a field?
			if(e.ColumnIndex == 0)
			{
				// Row is a new row?
				if(frow == null)
				{
					// Name given?
					if((row.Cells[0].Value != null) && (row.Cells[0].Value.ToString() != FIELD_PREFIX_SUGGESTION))
					{
						// Make a valid UDMF field name
						string validname = UniValue.ValidateName(row.Cells[0].Value.ToString());
						if(validname.Length > 0) 
						{
							if(uifields.ContainsKey(validname)) //mxd
							{ 
								MessageBox.Show("Please set this field's value via user interface.");
							} 
							else 
							{
								// Check if no other row already has this name
								foreach(DataGridViewRow r in fieldslist.Rows) 
								{
									// Name matches and not the same row?
									if((r.Index != row.Index) && (r.Cells.Count > 0) && (r.Cells[0].Value != null) &&
									    (r.Cells[0].Value.ToString().ToLowerInvariant() == validname)) 
									{
										// Cannot have two rows with same name
										validname = "";
										General.ShowWarningMessage("Fields must have unique names!", MessageBoxButtons.OK);
										break;
									}
								}

								// Still valid?
								if(validname.Length > 0) 
								{
									// Try to find the type in the map options
									int type = General.Map.Options.GetUniversalFieldType(elementname, validname, 0);

									// Make new row
									frow = new FieldsEditorRow(fieldslist, validname, type, null, false);
									frow.Visible = false;
									fieldslist.Rows.Insert(e.RowIndex + 1, frow);

									if(OnFieldInserted != null) OnFieldInserted(validname);
								}
							}
						}
					}

					// Mark the row for delete
					row.ReadOnly = true;
					deleterowstimer.Start();
				}
				else
				{
					// Name given?
					if(row.Cells[0].Value != null)
					{
						// Make a valid UDMF field name
						string validname = UniValue.ValidateName(row.Cells[0].Value.ToString());
						if(validname.Length > 0 && !uifields.ContainsKey(validname)) //mxd
						{
							// Check if no other row already has this name
							foreach(DataGridViewRow r in fieldslist.Rows)
							{
								// Name matches and not the same row?
								if((r.Index != row.Index) && (r.Cells.Count > 0) && (r.Cells[0].Value != null) &&
								   (r.Cells[0].Value.ToString().ToLowerInvariant() == validname))
								{
									// Cannot have two rows with same name
									validname = "";
									row.Cells[0].Value = lasteditfieldname;
									General.ShowWarningMessage("Fields must have unique names!", MessageBoxButtons.OK);
									break;
								}
							}

							// Still valid?
							if(validname.Length > 0)
							{
								// Try to find the type in the map options
								int type = General.Map.Options.GetUniversalFieldType(elementname, validname, -1);

								// Rename row and change type
								row.Cells[0].Value = validname;
								if(type != -1) frow.ChangeType(type);

								if(OnFieldNameChanged != null) OnFieldNameChanged(lasteditfieldname, validname);
								if(OnFieldTypeChanged != null) OnFieldTypeChanged(validname);
							}
							else
							{
								// Keep old name
								row.Cells[0].Value = lasteditfieldname;
							}
						}
						else
						{
							// Keep old name
							row.Cells[0].Value = lasteditfieldname;
						}
					}
					else
					{
						// Keep old name
						row.Cells[0].Value = lasteditfieldname;
					}
				}
			}
			// Changing field type?
			if((e.ColumnIndex == 1) && (frow != null))
			{
				if(OnFieldTypeChanged != null) OnFieldTypeChanged(frow.Name);
			}
			// Changing field value?
			if((e.ColumnIndex == 2) && (frow != null))
			{
				// Apply changes
				ApplyValue(frow, row.Cells[2].Value);
			}
			
			// Updated
			if(frow != null) frow.CellChanged();
			
			// Update button
			UpdateBrowseButton();
		}

		// Time to delete rows
		private void deleterowstimer_Tick(object sender, EventArgs e)
		{
			// Stop timer
			deleterowstimer.Stop();
			
			// Delete all rows that must be deleted
			for(int i = fieldslist.Rows.Count - 1; i >= 0; i--)
			{
				if(fieldslist.Rows[i].ReadOnly)
				{
					try { fieldslist.Rows.RemoveAt(i); } catch { }
				}
				else
				{
					//mxd. Preserve fixed fields visibility setting
					FieldsEditorRow frow = (fieldslist.Rows[i] as FieldsEditorRow);
					if(frow != null && frow.RowType == FieldsEditorRowType.FIXED) frow.Visible = showfixedfields;
					else fieldslist.Rows[i].Visible = true;
				}
			}

			// Update new row
			SetupNewRowStyle();
			
			// Update button
			UpdateBrowseButton();
		}

		// Selection changes
		private void fieldslist_SelectionChanged(object sender, EventArgs e)
		{
			browsebutton.Visible = false;
			ApplyEnums(true);
			
			// Update button
			UpdateBrowseButton();
		}

		// Browse clicked
		private void browsebutton_Click(object sender, EventArgs e)
		{
			// Any row selected?
			if(fieldslist.SelectedRows.Count > 0)
			{
				// Get selected row
				DataGridViewRow row = fieldslist.SelectedRows[0];
				if(row is FieldsEditorRow)
				{
					// Browse
					FieldsEditorRow frow = (FieldsEditorRow)row;
					frow.Browse(this.ParentForm);
					if(frow.TypeHandler.DynamicImage) browsebutton.Image = frow.TypeHandler.BrowseImage; //mxd
					fieldslist.Focus();
				}
			}
		}

		// This handles field data errors
		private void fieldslist_DataError(object sender, DataGridViewDataErrorEventArgs e)
		{
			// Ignore this, because we want to display values
			// in the type column that are not in their combobox
			e.ThrowException = false;
		}

		// Validate value in enums combobox
		private void enumscombo_Validating(object sender, CancelEventArgs e)
		{
			ApplyEnums(false);
		}

		// Scrolling
		private void fieldslist_Scroll(object sender, ScrollEventArgs e)
		{
			// Stop any cell editing
			ApplyEnums(true);
			fieldslist.EndEdit();
			HideBrowseButton();
		}

		// Mouse up event
		private void fieldslist_MouseUp(object sender, MouseEventArgs e)
		{
			// Focus to enums combobox when visible
			if(enumscombo.Visible)
			{
				enumscombo.Focus();
				enumscombo.SelectAll();
			}
		}
		
		#endregion
		
		#region ================== Private Methods

		// This applies a value to a row
		private void ApplyValue(FieldsEditorRow frow, object value)
		{
			// Defined?
			if((value != null) && (frow.RowType == FieldsEditorRowType.DYNAMIC || frow.RowType == FieldsEditorRowType.USERVAR 
				|| !frow.Info.Default.Equals(value)))
			{
				frow.Define(value);
			}
			else if(frow.RowType == FieldsEditorRowType.FIXED)
			{
				frow.Undefine();
			}
			
			if(OnFieldValueChanged != null) OnFieldValueChanged(frow.Name);
		}
		
		// This applies the contents of the enums combobox and hides (if opened)
		private void ApplyEnums(bool hide)
		{
			// Enums combobox shown?
			if(enumscombo.Visible && (enumscombo.Tag is FieldsEditorRow))
			{
				// Get the row
				FieldsEditorRow frow = (FieldsEditorRow)enumscombo.Tag;

				// Take the selected value and apply it
				ApplyValue(frow, enumscombo.Text);
				
				// Updated
				frow.CellChanged();
			}
			
			if(hide)
			{
				// Hide combobox
				enumscombo.Tag = null;
				enumscombo.Visible = false;
				enumscombo.Items.Clear();
			}
		}
		
		// This sets up the new row
		private void SetupNewRowStyle()
		{
			if(fieldslist.AllowUserToAddRows)
			{
				// Show text for new row
				fieldslist.Rows[fieldslist.NewRowIndex].Cells[0].Value = ADD_FIELD_TEXT;
				fieldslist.Rows[fieldslist.NewRowIndex].Cells[0].Style.ForeColor = SystemColors.GrayText;
				fieldslist.Rows[fieldslist.NewRowIndex].Cells[0].ReadOnly = false;

				// Make sure user can only enter property name in a new row
				fieldslist.Rows[fieldslist.NewRowIndex].Cells[1].ReadOnly = true;
				fieldslist.Rows[fieldslist.NewRowIndex].Cells[2].ReadOnly = true;
			}
		}

		// This hides the browse button
		private void HideBrowseButton()
		{
			browsebutton.Visible = false;
		}

		// This updates the Value column width
		private void UpdateValueColumn()
		{
			int fieldnamewidth = fieldname.Visible ? fieldname.Width : 0;
			int fieldtypewidth = fieldtype.Visible ? fieldtype.Width : 0;
			fieldvalue.Width = fieldslist.ClientRectangle.Width - fieldnamewidth - fieldtypewidth - SystemInformation.VerticalScrollBarWidth - 10;
		}

		// This updates the button
		private void UpdateBrowseButton()
		{
			FieldsEditorRow frow = null;

			// Any row selected?
			if(fieldslist.SelectedRows.Count > 0)
			{
				// Get selected row
				DataGridViewRow row = fieldslist.SelectedRows[0];
				if(row is FieldsEditorRow) frow = row as FieldsEditorRow;

				// Not the new row and FieldsEditorRow available?
				if((row.Index < fieldslist.NewRowIndex) && (frow != null))
				{
					// Browse button available for this type?
					if(frow.TypeHandler.IsBrowseable && !frow.TypeHandler.IsEnumerable)
					{
						Rectangle cellrect = fieldslist.GetCellDisplayRectangle(2, row.Index, false);

						// Show button
						enumscombo.Visible = false;
						browsebutton.Image = frow.TypeHandler.BrowseImage;
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

		//mxd
		private void UpdateFixedFieldsVisibility()
		{
			foreach(var row in fieldslist.Rows)
			{
				FieldsEditorRow frow = (row as FieldsEditorRow);
				if(frow != null && frow.RowType == FieldsEditorRowType.FIXED) frow.Visible = showfixedfields;
			}
		}

		/// <summary>
		/// Removes an field by its name.
		/// </summary>
		/// <param name="name">Name of the field to remove</param>
		public void RemoveField(string name)
		{
			int index = -1;

			foreach(DataGridViewRow dgvr in fieldslist.Rows)
			{
				if(dgvr.Cells[0].Value.ToString().ToLowerInvariant() == name.ToLowerInvariant())
				{
					index = dgvr.Index;
					break;
				}
			}

			if (index >= 0)
				fieldslist.Rows.RemoveAt(index);
		}

		/// <summary>
		/// Removes all user vars that have their default values.
		/// </summary>
		public void RemoveUserVarsWithDefaultValue()
		{
			List<int> removeindices = new List<int>();

			// Go through all rows and find the ones to remove. We can't remove them immediately since that would
			// change the collection while the loop is going through it.
			foreach (DataGridViewRow dgvr in fieldslist.Rows)
			{
				if(dgvr is FieldsEditorRow frow)
				{
					if (frow.RowType == FieldsEditorRowType.USERVAR && frow.Info != null && frow.TypeHandler.GetValue().Equals(frow.Info.Default))
						removeindices.Add(dgvr.Index);
				}
			}

			// Remove rows. Do it from behind since otherwise the indices would not match
			for (int i = removeindices.Count - 1; i >= 0; i--)
				fieldslist.Rows.RemoveAt(removeindices[i]);
		}
		
		#endregion
	}
}
