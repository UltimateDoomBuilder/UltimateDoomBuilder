#region ================== Copyright (c) 2010 Pascal vd Heiden

/*
 * Copyright (c) 2010 Pascal vd Heiden, www.codeimp.com
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
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Editing;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Types;

#endregion

namespace CodeImp.DoomBuilder.CommentsPanel
{
	public partial class CommentsDocker : UserControl
	{
		#region ================== Variables

		private readonly Dictionary<string, CommentInfo> v_comments = new Dictionary<string, CommentInfo>(StringComparer.Ordinal);
		private readonly Dictionary<string, CommentInfo> l_comments = new Dictionary<string, CommentInfo>(StringComparer.Ordinal);
		private readonly Dictionary<string, CommentInfo> s_comments = new Dictionary<string, CommentInfo>(StringComparer.Ordinal);
		private readonly Dictionary<string, CommentInfo> t_comments = new Dictionary<string, CommentInfo>(StringComparer.Ordinal);
		private bool preventupdate;
		
		#endregion
		
		#region ================== Constructor

		// Constructor
		public CommentsDocker()
		{
			InitializeComponent();
			CodeImp.DoomBuilder.General.ApplyDataGridViewFix(grid);

			filtermode.Checked = General.Settings.ReadPluginSetting("filtermode", false);
			clickselects.Checked = General.Settings.ReadPluginSetting("clickselects", false);
		}
		
		// Disposer
		protected override void Dispose(bool disposing)
		{
			General.Settings.WritePluginSetting("filtermode", filtermode.Checked);
			General.Settings.WritePluginSetting("clickselects", clickselects.Checked);
			
			if(disposing && (components != null))
			{
				components.Dispose();
			}
			
			base.Dispose(disposing);
		}
		
		#endregion

		#region ================== Methods

		// When attached to the docker
		public void Setup()
		{
			if(this.ParentForm != null)
			{
				this.ParentForm.Activated += ParentForm_Activated;
			}

			grid.CurrentCell = null;
			updatetimer.Start();
			enabledtimer.Start();
		}

		// Before detached from the docker
		public void Terminate()
		{
			preventupdate = true; //mxd
			if(this.ParentForm != null) this.ParentForm.Activated -= ParentForm_Activated;
			updatetimer.Tick -= updatetimer_Tick; //mxd
			enabledtimer.Tick -= enabledtimer_Tick; //mxd
			updatetimer.Stop();
			enabledtimer.Stop();
		}
		
		// This sends the focus back to the display and removes grid selection
		private void LoseFocus()
		{
			preventupdate = false;
			grid.CurrentCell = null;
			grid.ClearSelection();
			General.Interface.FocusDisplay();
		}
		
		// This updates the list for a specific kind of group
		private void UpdateGroupList(Dictionary<string, CommentInfo> newcomments, Dictionary<string, CommentInfo> comments, Image icon, List<DataGridViewRow> rowBatch)
		{
			// Remove old comments
			List<CommentInfo> commentslist = new List<CommentInfo>(comments.Values);
			foreach(CommentInfo c in commentslist)
			{
				if(!newcomments.ContainsKey(c.Comment))
				{
					rowBatch.Remove(c.Row);
					comments.Remove(c.Comment);
				}
			}
			
			// Update the list with comments
			foreach (KeyValuePair<string, CommentInfo> c in newcomments)
			{
				CommentInfo cc = c.Value;
				
				if(!comments.ContainsKey(c.Key))
				{
					// Create grid row
					DataGridViewRow row = new DataGridViewRow(); //grid.Rows[index];
					row.CreateCells(grid);
					row.Cells[0].Value = icon;
					row.Cells[0].Style.Alignment = DataGridViewContentAlignment.TopCenter;
					row.Cells[0].Style.Padding = new Padding(0, 5, 0, 0);
					row.Cells[1].Value = cc.Comment;
					row.Cells[1].Style.WrapMode = DataGridViewTriState.True;
					row.Cells[1].Tag = cc;
					rowBatch.Add(row);
					// Add to list of comments
					cc.Row = row;
					comments.Add(c.Key, cc);
				}
				else
				{
					cc = comments[c.Key];
					//row = cc.Row;
					cc.ReplaceElements(c.Value);
				}
			}
		}
		
		// This sets the timer to update the list very soon
		public void UpdateListSoon()
		{
			updatetimer.Stop();
			updatetimer.Start();
		}
		
		// This finds all comments and updates the list
		public void UpdateList()
		{
			
			if (!preventupdate && General.Map.Map.IsSafeToAccess)
			{
				grid.SuspendLayout();
				var rowBatch = new List<DataGridViewRow>();
				foreach (DataGridViewRow r in grid.Rows)
				{
					rowBatch.Add(r);
				}
				// Update vertices
				Dictionary<string, CommentInfo> newcomments = new Dictionary<string, CommentInfo>(StringComparer.Ordinal);
				if(!filtermode.Checked || (General.Editing.Mode.GetType().Name == "VerticesMode"))
				{
					foreach(Vertex v in General.Map.Map.Vertices) AddComments(v, newcomments);
				}
				UpdateGroupList(newcomments, v_comments, Properties.Resources.VerticesMode, rowBatch);

				// Update linedefs/sidedefs
				newcomments.Clear();
				if(!filtermode.Checked || (General.Editing.Mode.GetType().Name == "LinedefsMode"))
				{
					foreach(Linedef l in General.Map.Map.Linedefs) AddComments(l, newcomments);
					foreach(Sidedef sd in General.Map.Map.Sidedefs) AddComments(sd, newcomments);
				}
				UpdateGroupList(newcomments, l_comments, Properties.Resources.LinesMode, rowBatch);

				// Update sectors
				newcomments.Clear();
				if(!filtermode.Checked || (General.Editing.Mode.GetType().Name == "SectorsMode"))
				{
					foreach(Sector s in General.Map.Map.Sectors) AddComments(s, newcomments);
				}
				UpdateGroupList(newcomments, s_comments, Properties.Resources.SectorsMode, rowBatch);

				// Update things
				newcomments.Clear();
				if(!filtermode.Checked || (General.Editing.Mode.GetType().Name == "ThingsMode"))
				{
					foreach(Thing t in General.Map.Map.Things) AddComments(t, newcomments);
				}
				UpdateGroupList(newcomments, t_comments, Properties.Resources.ThingsMode, rowBatch);
				grid.Rows.Clear();
				grid.Rows.AddRange(rowBatch.ToArray());
				
				// Sort grid
				grid.Sort(grid.Columns[1], ListSortDirection.Ascending);
				grid.ResumeLayout();
				// Update
				grid.Update();

				grid.CurrentCell = null;
				grid.ClearSelection();
			}
		}
		
		// This adds comments from a MapElement
		private static void AddComments(MapElement e, Dictionary<string, CommentInfo> comments)
		{
			if(e.Fields.ContainsKey("comment"))
			{
				string c = e.Fields["comment"].Value.ToString();
				if(comments.ContainsKey(c))
					comments[c].AddElement(e);
				else
					comments[c] = new CommentInfo(c, e);
			}
		}
		
		// This changes the view to see the objects of a comment
		private static void ViewComment(CommentInfo c)
		{
			List<Vector2D> points = new List<Vector2D>();
			RectangleF area = MapSet.CreateEmptyArea();
			
			// Add all points to a list
			foreach(MapElement obj in c.Elements)
			{
				if(obj is Vertex)
				{
					points.Add((obj as Vertex).Position);
				}
				else if(obj is Linedef)
				{
					points.Add((obj as Linedef).Start.Position);
					points.Add((obj as Linedef).End.Position);
				}
				else if(obj is Sidedef)
				{
					points.Add((obj as Sidedef).Line.Start.Position);
					points.Add((obj as Sidedef).Line.End.Position);
				}
				else if(obj is Sector)
				{
					Sector s = (obj as Sector);
					foreach(Sidedef sd in s.Sidedefs)
					{
						points.Add(sd.Line.Start.Position);
						points.Add(sd.Line.End.Position);
					}
				}
				else if(obj is Thing)
				{
					Thing t = (obj as Thing);
					Vector2D p = t.Position;
					points.Add(p);
					points.Add(p + new Vector2D(t.Size * 2.0f, t.Size * 2.0f));
					points.Add(p + new Vector2D(t.Size * 2.0f, -t.Size * 2.0f));
					points.Add(p + new Vector2D(-t.Size * 2.0f, t.Size * 2.0f));
					points.Add(p + new Vector2D(-t.Size * 2.0f, -t.Size * 2.0f));
				}
				else
				{
					General.Fail("Unknown object given to zoom in on.");
				}
			}
			
			// Make a view area from the points
			foreach(Vector2D p in points) area = MapSet.IncreaseArea(area, p);
			
			// Make the area square, using the largest side
			if(area.Width > area.Height)
			{
				float delta = area.Width - area.Height;
				area.Y -= delta * 0.5f;
				area.Height += delta;
			}
			else
			{
				float delta = area.Height - area.Width;
				area.X -= delta * 0.5f;
				area.Width += delta;
			}
			
			// Add padding
			area.Inflate(100f, 100f);
			
			// Zoom to area
			ClassicMode editmode = (General.Editing.Mode as ClassicMode);
			editmode.CenterOnArea(area, 0.6f);
		}
		
		// This selects the elements in a comment
		private static void SelectComment(CommentInfo c, bool clear)
		{
			//string editmode = "";

			// Leave any volatile mode
			General.Editing.CancelVolatileMode();

			if(clear)
			{
				General.Map.Map.ClearAllSelected();

				if(c.Elements[0] is Thing)
					General.Editing.ChangeMode("ThingsMode");
				else if(c.Elements[0] is Vertex)
					General.Editing.ChangeMode("VerticesMode");
				else if((c.Elements[0] is Linedef) || (c.Elements[0] is Sidedef))
					General.Editing.ChangeMode("LinedefsMode");
				else if(c.Elements[0] is Sector)
					General.Editing.ChangeMode("SectorsMode");
			}
			else
			{
				if(!(c.Elements[0] is Thing))
				{
					// Sectors mode is a bitch because it deals with selections somewhat aggressively
					// so we have to switch to linedefs to make this work right
					if((General.Editing.Mode.GetType().Name == "VerticesMode") ||
					   (General.Editing.Mode.GetType().Name == "SectorsMode") ||
					   (General.Editing.Mode.GetType().Name == "MakeSectorMode"))
						General.Editing.ChangeMode("LinedefsMode");
				}
			}
			
			// Select the map elements
			foreach(MapElement obj in c.Elements)
			{
				if(obj is SelectableElement)
				{
					(obj as SelectableElement).Selected = true;
					
					if(obj is Sector)
					{
						foreach(Sidedef sd in (obj as Sector).Sidedefs)
							sd.Line.Selected = true;
					}
				}
			}

			General.Editing.Mode.UpdateSelectionInfo(); //mxd
			General.Interface.RedrawDisplay();
		}
		
		// This removes the comments from elements in a comment
		private void RemoveComment(CommentInfo c)
		{
			// Create undo
			if(c.Elements.Count == 1)
				General.Map.UndoRedo.CreateUndo("Remove comment");
			else
				General.Map.UndoRedo.CreateUndo("Remove " + c.Elements.Count + " comments");
			
			// Erase comment fields
			foreach(MapElement obj in c.Elements)
			{
				obj.Fields.BeforeFieldsChange();
				obj.Fields.Remove("comment");
			}
			
			UpdateList();
			General.Interface.RedrawDisplay();
		}
		
		// This gets the currently selected comment or returns null when none is selected
		private CommentInfo GetSelectedComment()
		{
			if(grid.SelectedCells.Count > 0)
			{
				foreach(DataGridViewCell c in grid.SelectedCells)
				{
					if((c.ValueType != typeof(Image)) && (c.Tag is CommentInfo))
						return (CommentInfo)(c.Tag);
				}
			}
			
			return null;
		}

		#endregion
		
		#region ================== Events
		
		// This gives a good idea when comments could have been changed
		// as it is called every time a dialog window closes.
		private void ParentForm_Activated(object sender, EventArgs e)
		{
			UpdateListSoon();
		}
		
		// Update regulary
		private void updatetimer_Tick(object sender, EventArgs e)
		{
			updatetimer.Stop();

			if (!BuilderPlug.Me.IsDockerActive())
				return;

			UpdateList();
		}
		
		// Mouse pressed
		private void grid_MouseDown(object sender, MouseEventArgs e)
		{
			preventupdate = true;
			
			if(e.Button == MouseButtons.Right)
			{
				DataGridView.HitTestInfo h = grid.HitTest(e.X, e.Y);
				if(h.Type == DataGridViewHitTestType.Cell)
				{
					grid.ClearSelection();
					grid.Rows[h.RowIndex].Selected = true;
				}
			}
		}
		
		// Mouse released
		private void grid_MouseUp(object sender, MouseEventArgs e)
		{
			CommentInfo c = GetSelectedComment();
			
			// Show popup menu?
			if((e.Button == MouseButtons.Right) && (c != null))
			{
				// Determine objects type
				string objname = "";
				if((c.Elements[0] is Vertex) && (c.Elements.Count > 1)) objname = "Vertices";
				else if((c.Elements[0] is Vertex) && (c.Elements.Count == 1)) objname = "Vertex";
				else if(((c.Elements[0] is Linedef) || (c.Elements[0] is Sidedef)) && (c.Elements.Count > 1)) objname = "Linedefs";
				else if(((c.Elements[0] is Linedef) || (c.Elements[0] is Sidedef)) && (c.Elements.Count == 1)) objname = "Linedef";
				else if((c.Elements[0] is Sector) && (c.Elements.Count > 1)) objname = "Sectors";
				else if((c.Elements[0] is Sector) && (c.Elements.Count == 1)) objname = "Sector";
				else if((c.Elements[0] is Thing) && (c.Elements.Count > 1)) objname = "Things";
				else if((c.Elements[0] is Thing) && (c.Elements.Count == 1)) objname = "Thing";
				editobjectitem.Text = "Edit " + objname + "...";
				
				// Show popup menu
				Point screenpos = grid.PointToScreen(new Point(e.X, e.Y));
				contextmenu.Tag = c;
				contextmenu.Show(screenpos);
			}
			else
			{
				// View selected comment
				if(c != null)
				{
					ViewComment(c);
					
					if(clickselects.Checked)
						SelectComment(c, true);
				}
				
				LoseFocus();
			}
		}
		
		// Select
		private void selectitem_Click(object sender, EventArgs e)
		{
			// Select comment
			CommentInfo c = (CommentInfo)contextmenu.Tag;
			if(c != null) SelectComment(c, true);
		}
		
		// Select additive
		private void selectadditiveitem_Click(object sender, EventArgs e)
		{
			// Select comment
			CommentInfo c = (CommentInfo)contextmenu.Tag;
			if(c != null) SelectComment(c, false);
		}
		
		// Remove comments
		private void removecommentsitem_Click(object sender, EventArgs e)
		{
			// Remove comment
			CommentInfo c = (CommentInfo)contextmenu.Tag;
			if(c != null) RemoveComment(c);
		}
		
		// Edit objects
		private void editobjectitem_Click(object sender, EventArgs e)
		{
			CommentInfo c = (CommentInfo)contextmenu.Tag;
			if(c != null)
			{
				if(c.Elements[0] is Vertex)
				{
					List<Vertex> vertices = new List<Vertex>();
					foreach(MapElement m in c.Elements) vertices.Add((Vertex)m);
					General.Interface.ShowEditVertices(vertices);
				}
				else if((c.Elements[0] is Linedef) || (c.Elements[0] is Sidedef))
				{
					List<Linedef> linedefs = new List<Linedef>();
					foreach(MapElement m in c.Elements)
					{
						if(m is Sidedef)
							linedefs.Add((m as Sidedef).Line);
						else
							linedefs.Add((Linedef)m);
					}
					General.Interface.ShowEditLinedefs(linedefs);
				}
				else if(c.Elements[0] is Sector)
				{
					List<Sector> sectors = new List<Sector>();
					foreach(MapElement m in c.Elements) sectors.Add((Sector)m);
					General.Interface.ShowEditSectors(sectors);
				}
				else if(c.Elements[0] is Thing)
				{
					List<Thing> things = new List<Thing>();
					foreach(MapElement m in c.Elements) things.Add((Thing)m);
					General.Interface.ShowEditThings(things);
				}

				General.Map.Map.Update();
				UpdateList();
				LoseFocus();
				General.Interface.RedrawDisplay();
			}
		}
		
		// Menu closes
		private void contextmenu_Closed(object sender, ToolStripDropDownClosedEventArgs e)
		{
			LoseFocus();
		}
		
		// Mode filter option changed
		private void filtermode_CheckedChanged(object sender, EventArgs e)
		{
			UpdateList();
			LoseFocus();
		}
		
		// Click selects changes
		private void clickselects_CheckedChanged(object sender, EventArgs e)
		{
			LoseFocus();
		}

		// This sets the comment on the current selection
		private void addcomment_Click(object sender, EventArgs e)
		{
			List<MapElement> elements = new List<MapElement>();

			if(General.Editing.Mode.GetType().Name == "VerticesMode")
			{
				ICollection<Vertex> vs = General.Map.Map.GetSelectedVertices(true);
				foreach(Vertex v in vs) elements.Add(v);
			}

			if(General.Editing.Mode.GetType().Name == "LinedefsMode")
			{
				ICollection<Linedef> ls = General.Map.Map.GetSelectedLinedefs(true);
				foreach(Linedef l in ls) elements.Add(l);
			}

			if(General.Editing.Mode.GetType().Name == "SectorsMode")
			{
				ICollection<Sector> ss = General.Map.Map.GetSelectedSectors(true);
				foreach(Sector s in ss) elements.Add(s);
			}

			if(General.Editing.Mode.GetType().Name == "ThingsMode")
			{
				ICollection<Thing> ts = General.Map.Map.GetSelectedThings(true);
				foreach(Thing t in ts) elements.Add(t);
			}

			if(elements.Count > 0)
			{
				// Create undo
				if(elements.Count == 1)
					General.Map.UndoRedo.CreateUndo("Add comment");
				else
					General.Map.UndoRedo.CreateUndo("Add " + elements.Count + " comments");

				// Set comment on elements
				foreach(MapElement el in elements)
				{
					el.Fields.BeforeFieldsChange();
					el.Fields["comment"] = new UniValue((int)UniversalType.String, addcommenttext.Text);
				}
			}
			
			addcommenttext.Text = "";
			UpdateList();
			LoseFocus();
			General.Interface.RedrawDisplay();
		}

		// Text typed in add comment box
		private void addcommenttext_KeyDown(object sender, KeyEventArgs e)
		{
			if((e.KeyCode == Keys.Enter) && (e.Modifiers == Keys.None) && addcomment.Enabled)
			{
				addcomment.PerformClick();
				e.Handled = true;
				e.SuppressKeyPress = true;
			}
		}

		// Check if the add comment box should be enabled
		private void enabledtimer_Tick(object sender, EventArgs e)
		{
			if(General.Editing.Mode == null) return; //mxd
			switch(General.Editing.Mode.GetType().Name)
			{
				case "VerticesMode":
					addcommentgroup.Enabled = (General.Map.Map.SelectedVerticessCount > 0);
					break;
				case "LinedefsMode":
					addcommentgroup.Enabled = (General.Map.Map.SelectedLinedefsCount > 0);
					break;
				case "SectorsMode":
					addcommentgroup.Enabled = (General.Map.Map.SelectedSectorsCount > 0);
					break;
				case "ThingsMode":
					addcommentgroup.Enabled = (General.Map.Map.SelectedThingsCount > 0);
					break;
			}
		}

		// Focus lost
		private void grid_Leave(object sender, EventArgs e)
		{
			preventupdate = false;
		}

		private void CommentsDocker_VisibleChanged(object sender, EventArgs e)
		{
			if (Visible)
				UpdateList();
		}

		#endregion
	}
}
