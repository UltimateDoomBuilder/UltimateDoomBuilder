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
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Controls;

namespace CodeImp.DoomBuilder.UDBScript
{
	public partial class ScriptDockerControl : UserControl
	{
		#region ================== Variables

		private ImageList images;
		private ContextMenuStrip filecontextmenu;
		private ContextMenuStrip foldercontextmenu;
		ToolStripItem[] slotitems;

		#endregion

		#region ================== Properties

		public ImageList Images { get { return images; } }

		#endregion

		#region ================== Constructor

		public ScriptDockerControl(string foldername)
		{
			InitializeComponent();

			images = new ImageList();
			images.Images.Add("Folder", Properties.Resources.Folder);
			images.Images.Add("Script", Properties.Resources.Script);

			filetree.ImageList = images;
		}

		#endregion

		#region ================== Methods

		/// <summary>
		/// Creates the context menu for file items.
		/// </summary>
		private void CreateFileContextMenu()
		{
			ToolStripMenuItem edititem = new ToolStripMenuItem("Edit");
			edititem.Click += EditScriptClicked;

			ToolStripMenuItem deleteitem = new ToolStripMenuItem("Clear slot");
			deleteitem.Tag = "deleteitem";

			slotitems = new ToolStripItem[BuilderPlug.NUM_SCRIPT_SLOTS + 2];
			slotitems[0] = deleteitem;
			slotitems[1] = new ToolStripSeparator();
			for (int i=0; i < BuilderPlug.NUM_SCRIPT_SLOTS; i++)
			{
				slotitems[i+2] = new ToolStripMenuItem("Slot " + (i + 1));
				slotitems[i+2].Tag = i + 1;
			}

			ToolStripMenuItem setslot = new ToolStripMenuItem("Set slot");
			setslot.DropDownItems.AddRange(slotitems);
			setslot.DropDownItemClicked += ItemClicked;

			filecontextmenu = new ContextMenuStrip();
			filecontextmenu.Items.AddRange(new ToolStripItem[]
			{
				edititem,
				setslot
			});
		}

		/// <summary>
		/// Creates the context menu for folder items.
		/// </summary>
		private void CreateFolderContextMenu()
		{
			ToolStripMenuItem openitem = new ToolStripMenuItem("Open in Explorer");
			openitem.Click += (s, e) => { try { Process.Start("explorer.exe", ((ScriptDirectoryStructure)filetree.SelectedNodes[0].Tag).Path); } catch { } };

			foldercontextmenu = new ContextMenuStrip();
			foldercontextmenu.Items.AddRange(new ToolStripItem[]
			{
				openitem
			});
		}

		/// <summary>
		/// Returns the hotkey text for a script s lot.
		/// </summary>
		/// <param name="slot">Slot to get the hotkey text for</param>
		/// <returns>The hotkey text</returns>
		private string GetHotkeyText(int slot)
		{
			string actionname = "udbscript_udbscriptexecuteslot" + slot;
			string keytext = "no hotkey";

			Actions.Action action = General.Actions.GetActionByName(actionname);
			if (action.ShortcutKey != 0)
				keytext = Actions.Action.GetShortcutKeyDesc(actionname);

			return keytext;
		}

		/// <summary>
		/// Updates the context menu of the slots so that the items show the script name and hotkey (if applicable)
		/// </summary>
		private void UpdateFileContextMenu()
		{
			for (int i = 0; i < BuilderPlug.NUM_SCRIPT_SLOTS; i++)
			{
				ScriptInfo si = BuilderPlug.Me.GetScriptSlot(i + 1);

				if (si != null)
					slotitems[i+2].Text = "Slot " + (i+1) + ": " + si.Name + " [" + GetHotkeyText(i+1) + "]";
				else
					slotitems[i+2].Text = "Slot " + (i+1) + ": not assigned [" + GetHotkeyText(i + 1) + "]";
			}
		}

		/// <summary>
		/// Recursively updates the tree, so that the items show the hotkey (if applicable)
		/// </summary>
		/// <param name="node"></param>
		private void UpdateTree(TreeNode node)
		{
			ScriptInfo si = node.Tag as ScriptInfo;

			// Update the item
			if(si != null)
			{
				int slot = BuilderPlug.Me.GetScriptSlotByScriptInfo(si);

				if (slot == 0) // Not assigned to a slot, just set the name
					node.Text = si.Name;
				else // It's assigned to a slot, so set the name and the hotkey
					node.Text = si.Name + " [" + GetHotkeyText(slot) + "]";
			}

			// Update all children
			foreach(TreeNode childnode in node.Nodes)
				UpdateTree(childnode);
		}

		/// <summary>
		/// Assignes a script to a slot.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
			ScriptInfo si = filetree.SelectedNodes[0].Tag as ScriptInfo;

			if (si == null)
				return;

			if (e.ClickedItem.Tag is string && (string)e.ClickedItem.Tag == "deleteitem")
			{
				int slot = BuilderPlug.Me.GetScriptSlotByScriptInfo(si);

				if(slot != 0)
					BuilderPlug.Me.SetScriptSlot(slot, null);
			}
			else
			{
				if (e.ClickedItem.Tag is int)
					BuilderPlug.Me.SetScriptSlot((int)e.ClickedItem.Tag, si);
			}

			UpdateFileContextMenu();

			foreach (TreeNode node in filetree.Nodes)
				UpdateTree(node);
		}

		/// <summary>
		/// Edits a script.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void EditScriptClicked(object sender, EventArgs e)
		{
			ScriptInfo si = filetree.SelectedNodes[0].Tag as ScriptInfo;

			if (si == null)
				return;

			//MessageBox.Show("Edit script " + si.ScriptFile);
			BuilderPlug.Me.EditScript(si.ScriptFile);
		}

		/// <summary>
		/// Fills the tree of all available scripts. Tries to re-select a previously selected script
		/// </summary>
		public void FillTree()
		{
			string previousscriptfile = string.Empty;
			NodesCollection nc = filetree.SelectedNodes;
			string filtertext = tbFilter.Text.ToLowerInvariant().Trim();

			scriptoptions.ParametersView.Rows.Clear();
			scriptoptions.ParametersView.Refresh();

			if(nc.Count > 0 && nc[0].Tag is ScriptInfo)
				previousscriptfile = ((ScriptInfo)nc[0].Tag).ScriptFile;

			filetree.BeginUpdate();

			filetree.Nodes.Clear();
			filetree.Nodes.AddRange(AddToTree(filtertext, BuilderPlug.Me.ScriptDirectoryStructure));

			foreach(TreeNode node in filetree.Nodes)
			{
				TreeNode result = FindScriptTreeNode(previousscriptfile, node);

				if (result != null)
				{
					filetree.SelectedNodes.Add(result);
					result.EnsureVisible();
					break;
				}
			}

			filetree.EndUpdate();
		}

		/// <summary>
		/// Recursively tries to find a tree node by the script file name. Based on https://stackoverflow.com/a/19227024 by user "King King"
		/// </summary>
		/// <param name="name">Script file name to look for</param>
		/// <param name="root">TreeNode node to start looking at</param>
		/// <returns>Found TreeNode or null</returns>
		private TreeNode FindScriptTreeNode(string name, TreeNode root)
		{
			// The "root" node might already be the one we're looking for
			if (root.Tag is ScriptInfo && ((ScriptInfo)root.Tag).ScriptFile == name)
				return root;

			foreach (TreeNode node in root.Nodes)
			{
				if (node.Tag is ScriptInfo && ((ScriptInfo)node.Tag).ScriptFile == name)
					return node;

				TreeNode next = FindScriptTreeNode(name, node);

				if (next != null)
					return next;
			}

			return null;
		}

		/// <summary>
		/// Recursively adds nodes to the tree. Optionally filters script, showing only the ones containing the filter text in
		/// the script name or description.
		/// </summary>
		/// <param name="filtertext">Text to filter by. null or an empty string will not filter at all</param>
		/// <param name="sds">Directory structur or crawl through</param>
		/// <returns></returns>
		private TreeNode[] AddToTree(string filtertext, ScriptDirectoryStructure sds)
		{
			List<TreeNode> newnodes = new List<TreeNode>();

			if (sds == null)
				return newnodes.ToArray();

			// Go through folders and add files (and other folders) recusrively
			foreach (ScriptDirectoryStructure subsds in sds.Directories.OrderBy(s => s.Name))
			{
				TreeNode[] children = AddToTree(filtertext, subsds);
				TreeNode tn = new TreeNode(subsds.Name, AddToTree(filtertext, subsds));

				tn.Tag = subsds;
				tn.SelectedImageKey = tn.ImageKey = "Folder";
				tn.ContextMenuStrip = foldercontextmenu;

				if (subsds.Expanded)
					tn.Expand();

				newnodes.Add(tn);
			}

			// Add the scripts in to folder to the tree
			foreach(ScriptInfo si in sds.Scripts.OrderBy(s => s.Name))
			{
				// Check if there's a text to filter scripts by, and if there is, skip scripts that do not contain
				// the filter in the script name or description
				if (!string.IsNullOrWhiteSpace(filtertext))
				{
					if (!si.Name.ToLowerInvariant().Contains(filtertext) && !si.Description.ToLowerInvariant().Contains(filtertext))
						continue;
				}

				TreeNode tn = new TreeNode();
				int slot = BuilderPlug.Me.GetScriptSlotByScriptInfo(si);

				if (slot == 0) // Not assigned to a slot, just set the name
					tn.Text = si.Name;
				else // It's assigned to a slot, so set the name and the hotkey
					tn.Text = si.Name + " [" + GetHotkeyText(slot) + "]";

				tn.Tag = si;
				tn.SelectedImageKey = tn.ImageKey = "Script";
				tn.ContextMenuStrip = filecontextmenu;

				newnodes.Add(tn);
			}

			return newnodes.ToArray();
		}

		/// <summary>
		/// Ends editing the currently edited grid view cell. This is required so that the value is applied before running the script if the cell is currently
		/// being editing (i.e. typing in a value, then running the script without clicking somewhere else first)
		/// </summary>
		public void EndEdit()
		{
			scriptoptions.EndEdit();
		}

		#endregion

		#region ================== Events

		/// <summary>
		/// Sets up the the script options control for the currently selected script
		/// </summary>
		/// <param name="sender">the sender</param>
		/// <param name="e">the event</param>
		private void filetree_AfterSelect(object sender, TreeViewEventArgs e)
		{
			if (e.Node.Tag == null)
			{
				tbDescription.Text = string.Empty;
				scriptoptions.ParametersView.Rows.Clear();
				return;
			}

			if(e.Node.Tag is ScriptInfo)
			{
				BuilderPlug.Me.CurrentScript = (ScriptInfo)e.Node.Tag;
				scriptoptions.ParametersView.Rows.Clear();

				foreach (ScriptOption so in ((ScriptInfo)e.Node.Tag).Options)
				{
					int index = scriptoptions.ParametersView.Rows.Add();
					scriptoptions.ParametersView.Rows[index].Tag = so;
					scriptoptions.ParametersView.Rows[index].Cells["Value"].Value = so.value;
					scriptoptions.ParametersView.Rows[index].Cells["Description"].Value = so.description;
				}

				scriptoptions.EndAddingOptions();

				tbDescription.Text = ((ScriptInfo)e.Node.Tag).Description;
			}
			else
			{
				scriptoptions.ParametersView.Rows.Clear();
				scriptoptions.ParametersView.Refresh();
			}
		}

		/// <summary>
		/// Runs the currently selected script immediately
		/// </summary>
		/// <param name="sender">the sender</param>
		/// <param name="e">the event</param>
		private void btnRunScript_Click(object sender, EventArgs e)
		{
			BuilderPlug.Me.ScriptExecute();
		}

		/// <summary>
		/// Resets all options of the currently selected script to their default values
		/// </summary>
		/// <param name="sender">the sender</param>
		/// <param name="e">the event</param>
		private void btnResetToDefaults_Click(object sender, EventArgs e)
		{
			foreach (DataGridViewRow row in scriptoptions.ParametersView.Rows)
			{
				if (row.Tag is ScriptOption)
				{
					ScriptOption so = (ScriptOption)row.Tag;

					row.Cells["Value"].Value = so.defaultvalue.ToString();
					so.typehandler.SetValue(so.defaultvalue);

					General.Settings.DeletePluginSetting("scriptoptions." + BuilderPlug.Me.CurrentScript.GetScriptPathHash() + "." + so.name);
				}
			}
		}

		/// <summary>
		/// Sets the node that was clicked on as the selected node. This is needed for the context menu, because it's the easiest
		/// way to know which node the context menu was opened for.
		/// </summary>
		/// <param name="sender">The sender</param>
		/// <param name="e">The event</param>
		private void filetree_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
		{
			((TreeView)sender).SelectedNode = e.Node;
		}

		private void ScriptDockerControl_VisibleChanged(object sender, EventArgs e)
		{
			if (!Visible || Disposing)
				return;

			CreateFileContextMenu();
			UpdateFileContextMenu();
			CreateFolderContextMenu();

			FillTree();
		}

		private void btnClearFilter_Click(object sender, EventArgs e)
		{
			tbFilter.Clear();
		}

		private void tbFilter_TextChanged(object sender, EventArgs e)
		{
			// TreeNodes can't by dynamically hidden, so it's easier to just fill the tholw tree again
			FillTree();
		}

		#endregion

		private void filetree_BeforeCollapse(object sender, TreeViewCancelEventArgs e)
		{
			ScriptDirectoryStructure sds;

			if(e.Node.Tag is ScriptDirectoryStructure)
			{
				sds = (ScriptDirectoryStructure)e.Node.Tag;
				sds.Expanded = false;
			}

			// Immedeiately save the status, otherwise folders will be expanded/collapsed incorrectly on hot reload
			BuilderPlug.Me.SaveScriptDirectoryExpansionStatus(BuilderPlug.Me.ScriptDirectoryStructure);
		}

		private void filetree_BeforeExpand(object sender, TreeViewCancelEventArgs e)
		{
			ScriptDirectoryStructure sds;

			if (e.Node.Tag is ScriptDirectoryStructure)
			{
				sds = (ScriptDirectoryStructure)e.Node.Tag;
				sds.Expanded = true;
			}

			// Immedeiately save the status, otherwise folders will be expanded/collapsed incorrectly on hot reload
			BuilderPlug.Me.SaveScriptDirectoryExpansionStatus(BuilderPlug.Me.ScriptDirectoryStructure);
		}
	}
}
