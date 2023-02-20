
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
using System.Drawing;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Editing;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	public partial class UndoRedoPanel : UserControl
	{
		#region ================== Constants
		
		private const int MAX_DISPLAY_LEVELS = 400;
		
		#endregion
		
		#region ================== Variables
		
		private bool ignoreevents;
		private int currentselection;
		private int addindex;
		private string begindescription;
		
		#endregion
		
		#region ================== Constructor
		
		// Constructor
		public UndoRedoPanel()
		{
			InitializeComponent();
		}
		
		#endregion
		
		#region ================== Methods
		
		// This sets the description for the first item
		public void SetBeginDescription(string description)
		{
			begindescription = description;
		}
		
		// This refills the list
		public void UpdateList()
		{
			ignoreevents = true;
			currentselection = -1;
			
			// Check we have undo/redo capability
			if((General.Map == null) || General.Map.IsDisposed || (General.Map.UndoRedo == null))
			{
				// No, clear the list
				list.Items.Clear();
				return;
			}
			
			// Make complete list of levels
			List<UndoSnapshot> levels = General.Map.UndoRedo.GetUndoList();
			levels.Reverse();
			int numundos = levels.Count;
			levels.AddRange(General.Map.UndoRedo.GetRedoList());
			//int numredos = levels.Count - numundos;
			
			// Determine the offset to show items at
			int offset = numundos - (MAX_DISPLAY_LEVELS >> 1);
			if((offset + MAX_DISPLAY_LEVELS) > levels.Count) offset = levels.Count - MAX_DISPLAY_LEVELS;
			if(offset < 0) offset = 0;
			
			// Reset the list
			list.SelectedItems.Clear();
			list.BeginUpdate();
			addindex = 0;
			
			// Add beginning
			if(offset > 0)
			{
				// This indicates there is more above, but we don't display it
				// because when the list gets too long it becomes slow
				AddItem("...");
			}
			else
			{
				// Real beginning
				ListViewItem firstitem = AddItem(begindescription);
				
				// Are we at the first item?
				if(numundos == 0)
				{
					// Highlight the last undo level
					firstitem.BackColor = SystemColors.Highlight;
					firstitem.ForeColor = SystemColors.HighlightText;
					currentselection = 0;
				}
				else
				{
					// Normal undo level
					firstitem.ForeColor = SystemColors.WindowText;
					firstitem.BackColor = SystemColors.Window;
				}
			}
			
			// Add levels!
			for(int i = offset; i < levels.Count; i++)
			{
				// Add no more than the MAX_DISPLAY_LEVELS
				ListViewItem item;
				if((addindex - 1) == MAX_DISPLAY_LEVELS)
					item = AddItem("...");
				else
					item = AddItem(levels[i].Description);
				
				// Color item
				if(i == (numundos - 1))
				{
					// Highlight the last undo level
					item.BackColor = SystemColors.Highlight;
					item.ForeColor = SystemColors.HighlightText;
					currentselection = addindex - 1;
				}
				else if(i >= numundos)
				{
					// Make gray because this is a redo level
					item.ForeColor = SystemColors.GrayText;
					item.BackColor = SystemColors.Control;
				}
				else
				{
					// Normal undo level
					item.ForeColor = SystemColors.WindowText;
					item.BackColor = SystemColors.Window;
				}
				
				// Leave when list is full
				if((addindex - 1) > MAX_DISPLAY_LEVELS)
					break;
			}
			
			// Remove the excessive items
			for(int i = list.Items.Count - 1; i >= addindex; i--)
				list.Items.RemoveAt(i);
			
			// We must always have the "selected" item in the list
			if(currentselection == -1)
				throw new Exception("Where is the selection?");
				
			// Make sure we can see the highlighted item
			list.Items[currentselection].EnsureVisible();
			
			list.EndUpdate();
			UpdateColumnSizes();
			ignoreevents = false;
		}
		
		// This updates/adds an item in the list
		private ListViewItem AddItem(string text)
		{
			ListViewItem item;
			if(addindex < list.Items.Count)
			{
				item = list.Items[addindex];
				item.Text = text;
			}
			else
			{
				item = list.Items.Add(text);
			}
			addindex++;
			return item;
		}
		
		// This updates the list column size
		private void UpdateColumnSizes()
		{
			// Check if a vertical scrollbar exists and adjust the column in the listbox accordingly
			if((SysCall.GetWindowLong(list.Handle, BuilderPlug.GWL_STYLE) & BuilderPlug.WS_VSCROLL) != 0)
				coldescription.Width = list.ClientRectangle.Width - 2;
			else
				coldescription.Width = list.ClientRectangle.Width - SystemInformation.VerticalScrollBarWidth - 2;
		}
		
		#endregion
		
		#region ================== Events
		
		// When layout changes
		protected override void OnLayout(LayoutEventArgs e)
		{
			base.OnLayout(e);
			UpdateColumnSizes();
		}
		
		// Control resizes
		private void list_Resize(object sender, EventArgs e)
		{
			UpdateColumnSizes();
		}
		
		// Item selected
		private void list_SelectedIndexChanged(object sender, EventArgs e)
		{
			if(ignoreevents) return;
			
			ignoreevents = true;
			
			// We must have something selected
			if(list.SelectedIndices.Count > 0)
			{
				// Not the same as last selected?
				int selectedindex = list.SelectedIndices[0];
				if(selectedindex != currentselection)
				{
					// Recolor the elements in the list to match with the selection
					list.BeginUpdate();
					foreach(ListViewItem item in list.Items)
					{
						if(item.Index < selectedindex)
						{
							// Normal undo level
							item.ForeColor = SystemColors.WindowText;
							item.BackColor = SystemColors.Window;
						}
						else if(item.Index == selectedindex)
						{
							// Target level
							item.BackColor = SystemColors.Highlight;
							item.ForeColor = SystemColors.HighlightText;
						}
						else
						{
							// Make gray because this will become a redo level
							item.ForeColor = SystemColors.GrayText;
							item.BackColor = SystemColors.Control;
						}
					}
					list.EndUpdate();
				}
			}

			General.Interface.FocusDisplay();
			
			ignoreevents = false;
		}
		
		// Mouse released
		private void list_MouseUp(object sender, MouseEventArgs e)
		{
			ignoreevents = true;
			
			// We must have something selected
			if(list.SelectedIndices.Count > 0)
			{
				// Not the same as last selected?
				int selectedindex = list.SelectedIndices[0];
				if(selectedindex != currentselection)
				{
					// Perform the undo/redos, the list will be updated automatically
					int delta = currentselection - selectedindex;
					if(delta < 0)
						General.Map.UndoRedo.PerformRedo(-delta);
					else
						General.Map.UndoRedo.PerformUndo(delta);
				}
				else
				{
					list.SelectedIndices.Clear();
				}
			}

			General.Interface.FocusDisplay();
			
			ignoreevents = false;
		}
		
		// Key released
		private void list_KeyUp(object sender, KeyEventArgs e)
		{
			ignoreevents = true;
			
			list.SelectedIndices.Clear();
			General.Interface.FocusDisplay();
			
			ignoreevents = false;
		}
		
		#endregion
	}
}
