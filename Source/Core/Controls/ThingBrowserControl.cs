
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
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Data;

#endregion

namespace CodeImp.DoomBuilder.Controls
{
	public partial class ThingBrowserControl : UserControl
	{
		#region ================== Constants

		private const int WARNING_ICON_INDEX = 20; //mxd
		private const int FOLDER_ICON_OFFSET = 21; //mxd
		private const int FOLDER_OPEN_ICON_OFFSET = 41; //mxd

		#endregion

		#region ================== Events

		public delegate void TypeChangedDeletegate(ThingTypeInfo value);
		public delegate void TypeDoubleClickDeletegate();

		public event TypeChangedDeletegate OnTypeChanged;
		public event TypeDoubleClickDeletegate OnTypeDoubleClicked;

		#endregion

		#region ================== Variables

		private List<TreeNode> nodes;
		private List<TreeNode> validnodes; //mxd
		private ThingTypeInfo thinginfo;
		private bool doupdatenode;
		private bool doupdatetextbox;
		private TreeNode doubleclickednode; //mxd
		
		#endregion

		#region ================== Properties

		public string TypeStringValue { get { return typeid.Text; } }
		public bool UseMultiSelection { get { return typelist.SelectionMode == TreeViewSelectionMode.MultiSelectSameLevel; } set { typelist.SelectionMode = (value ? TreeViewSelectionMode.MultiSelectSameLevel : TreeViewSelectionMode.SingleSelect); } }

		#endregion

		#region ================== Constructor

		// Constructor
		public ThingBrowserControl()
		{
			InitializeComponent();
		}

		// This sets up the control
		public void Setup()
		{
			// Go for all predefined categories
			typelist.Nodes.Clear();
			nodes = new List<TreeNode>();
			validnodes = new List<TreeNode>(); //mxd
			AddThingCategories(General.Map.Data.ThingCategories, typelist.Nodes); //mxd
			doupdatenode = true;
			doupdatetextbox = true;
		}

		//mxd. This recursively creates thing category tree nodes. Returns true when a thing in this category is obsolete
		private bool AddThingCategories(ICollection<ThingCategory> categories, TreeNodeCollection collection)
		{
			bool containsobsoletethings = false;
			
			foreach(ThingCategory tc in categories) 
			{
				// Create category
				TreeNode cn = collection.Add(tc.Name, tc.Title);

				// Create subcategories
				bool isobsolete = AddThingCategories(tc.Children, cn.Nodes);

				// Create things
				foreach(ThingTypeInfo ti in tc.Things) 
				{
					// Create thing
					TreeNode n = cn.Nodes.Add(ti.Title);
					n.Tag = ti;

					if(ti.IsObsolete)
					{
						n.Text += " - OBSOLETE";
						n.BackColor = Color.MistyRose;
						n.ToolTipText = ti.ObsoleteMessage;

						// Set warning icon
						n.ImageIndex = WARNING_ICON_INDEX;
						n.SelectedImageIndex = WARNING_ICON_INDEX;
						isobsolete = true;
					}
					else
					{
						// Set regular icon
						if((ti.Color > -1) && (ti.Color < WARNING_ICON_INDEX)) n.ImageIndex = ti.Color;
						n.SelectedImageIndex = n.ImageIndex;
					}

					nodes.Add(n);
				}

				// Set category icon
				containsobsoletethings |= isobsolete;
				if(isobsolete)
				{
					cn.BackColor = Color.MistyRose;
					cn.ImageIndex = WARNING_ICON_INDEX;
					cn.SelectedImageIndex = WARNING_ICON_INDEX;
				}
				else
				{
					cn.ImageIndex = FOLDER_ICON_OFFSET; // Offset to folder icons
					if((tc.Color > -1) && (tc.Color < WARNING_ICON_INDEX)) cn.ImageIndex += tc.Color;
					cn.SelectedImageIndex = cn.ImageIndex;
				}
			}

			return containsobsoletethings;
		}

		#endregion
		
		#region ================== Methods

		// Select a type
		public void SelectType(int type)
		{
			// Set type index
			typeid.Text = type.ToString();
			typeid_TextChanged(this, EventArgs.Empty);
		}

		// Select multiple thing types
		public void SelectMultipleTypes(int[] types)
		{
			if (types.Length == 0)
			{
				ClearSelectedType();
				typeid_TextChanged(this, EventArgs.Empty);
				return;
			}

			if (types.Length == 1)
			{
				SelectType(types[0]);
				return;
			}

			ClearSelectedType();
			typeid_TextChanged(this, EventArgs.Empty);

			doupdatetextbox = false;
			doupdatenode = false;
			HashSet<int> indexSet = types.ToHashSet();
			foreach (TreeNode n in nodes)
			{
				// Node in index input?
				if (indexSet.Contains((n.Tag as ThingTypeInfo).Index))
				{
					// Add this to selection
					if (n.TreeView != null) //mxd. Tree node may've been removed during filtering
					{
						if (n.Parent != null) n.Parent.Expand(); // node won't have parent when the list is prefiltered
						typelist.SelectedNodes.Add(n);
						n.EnsureVisible();
					}
				}
			}
			doupdatenode = true;
			doupdatetextbox = true;
		}

		// Return selected type info
		public ThingTypeInfo GetSelectedInfo()
		{
			return thinginfo;
		}

		// This clears the type
		public void ClearSelectedType()
		{
			doupdatenode = false;

			// Clear selection
			typelist.SelectedNodes.Clear(); //mxd
			validnodes.Clear(); //mxd
			typeid.Text = "";

			// Collapse nodes
			foreach(TreeNode n in nodes)
				if(n.Parent.IsExpanded) n.Parent.Collapse();
			
			doupdatenode = true;
		}

		// Result
		public int GetResult(int original)
		{
			//mxd. Get a random ThingTypeInfo from valid nodes?
			if(typelist.SelectionMode == TreeViewSelectionMode.MultiSelectSameLevel && validnodes.Count > 0) 
			{
				return (validnodes[General.Random(0, validnodes.Count - 1)].Tag as ThingTypeInfo).Index;
			}
			
			return typeid.GetResult(original);
		}

		//jaeden
		public int[] GetMultiResult(int[] original)
		{
			if (typelist.SelectionMode == TreeViewSelectionMode.MultiSelectSameLevel && validnodes.Count > 0)
			{
				return validnodes.Select(node => (node.Tag as ThingTypeInfo).Index).ToArray();
			}

			return original.Select(id => typeid.GetResult(id)).ToArray();
		}

		//mxd
		public void FocusTextbox()
		{
			tbFilter.Focus();
		}

		//mxd
		private List<TreeNode> GetValidNodes() 
		{
			Dictionary<string, TreeNode> vn = new Dictionary<string, TreeNode>(StringComparer.Ordinal);
			foreach(TreeNode n in typelist.SelectedNodes) GetValidNodes(n, ref vn);
			return new List<TreeNode>(vn.Values);
		}

		private static void GetValidNodes(TreeNode root, ref Dictionary<string, TreeNode> vn)
		{
			if(root.Nodes.Count == 0)
			{
				if(root.Tag is ThingTypeInfo && !vn.ContainsKey(root.Text)) vn.Add(root.Text, root);
			}
			else
			{
				foreach(TreeNode n in root.Nodes) GetValidNodes(n, ref vn);
			}
		}

		// Update preview image (mxd)
		private void UpdateThingSprite() 
		{
			if(General.Map == null) return;
			
			if(thinginfo != null) 
			{
				if(thinginfo.Sprite.ToLowerInvariant().StartsWith(DataManager.INTERNAL_PREFIX) &&
				   (thinginfo.Sprite.Length > DataManager.INTERNAL_PREFIX.Length)) 
				{
					spritetex.Image = General.Map.Data.GetSpriteImage(thinginfo.Sprite).GetSpritePreview();
					return;
				} 

				if((thinginfo.Sprite.Length < 9) && (thinginfo.Sprite.Length > 0))
				{
					ImageData sprite = General.Map.Data.GetSpriteImage(thinginfo.Sprite);
					spritetex.Image = sprite.GetPreview();
					if(!sprite.IsPreviewLoaded) updatetimer.Start();
					return;
				}
			}

			//Show Mixed Things icon?
			if(validnodes.Count > 1)
			{
				spritetex.Image = Properties.Resources.MixedThings;
				return;
			}

			spritetex.Image = null;
		}

		#endregion

		#region ================== Events

		// List double-clicked. e.Node and typelist.SelectedNodes[0] may contain incorrect node, 
		// so we set the correct one in typelist_AfterSelect handler (mxd)
		private void typelist_MouseDoubleClick(object sender, MouseEventArgs e) 
		{
			if(typelist.SelectedNodes.Count == 1
			    && doubleclickednode != null
			    && doubleclickednode.Nodes.Count == 0
			    && doubleclickednode.Tag is ThingTypeInfo
			    && OnTypeDoubleClicked != null
			    && typeid.Text.Length > 0)
			{
				OnTypeDoubleClicked();
			}
		}
		
		// Thing type selection changed
		private void typelist_SelectionsChanged(object sender, EventArgs e) 
		{
			doubleclickednode = null; //mxd
			if(!doupdatetextbox) return;

			//mxd
			validnodes = GetValidNodes();

			//mxd. Got a valid multiselection? Well, can't show any useful info about that...
			if(typelist.SelectionMode == TreeViewSelectionMode.MultiSelectSameLevel && validnodes.Count > 1) 
			{
				doupdatenode = false;
				if(!string.IsNullOrEmpty(typeid.Text))
				{
					// Event will be raised in typeid_OnTextChanged
					typeid.Text = "";
				}
				else if(OnTypeChanged != null)
				{
					// Or raise event here
					UpdateThingSprite();
					OnTypeChanged(thinginfo);
				}
				doupdatenode = true;
			}
			else if(validnodes.Count == 1) //Anything selected?
			{
				// Show info
				doupdatenode = false;
				typeid.Text = (validnodes[0].Tag as ThingTypeInfo).Index.ToString();
				doupdatenode = true;

				// Set as double-clicked only if a single child node is selected
				if(typelist.SelectedNodes.Count == 1 && typelist.SelectedNodes[0].Nodes.Count == 0)
				{
					doubleclickednode = validnodes[0]; //mxd
				}
			}
			else
			{
				UpdateThingSprite(); //mxd
			}
		}

		// Thing type index changed
		private void typeid_TextChanged(object sender, EventArgs e)
		{
			bool knownthing = false;

			// Any text?
			if(typeid.Text.Length > 0)
			{
				// Get the info
				int typeindex = typeid.GetResult(0);
				thinginfo = General.Map.Data.GetThingInfoEx(typeindex);
				if(thinginfo != null)
				{
					knownthing = true;

					// Size
					sizelabel.Text = (thinginfo.Radius * 2) + " x " + thinginfo.Height;

					// Hangs from ceiling
					if(thinginfo.Hangs) positionlabel.Text = "Ceiling"; else positionlabel.Text = "Floor";

					// Blocking
					switch(thinginfo.Blocking)
					{
						case ThingTypeInfo.THING_BLOCKING_NONE: blockinglabel.Text = "No"; break;
						case ThingTypeInfo.THING_BLOCKING_FULL: blockinglabel.Text = "Completely"; break;
						case ThingTypeInfo.THING_BLOCKING_HEIGHT: blockinglabel.Text = "True-Height"; break;
						default: blockinglabel.Text = "Unknown"; break;
					}
				}

				if(doupdatenode)
				{
					doupdatetextbox = false;
					typelist.SelectedNodes.Clear();
					validnodes.Clear(); //mxd
					foreach(TreeNode n in nodes)
					{
						// Matching node?
						if((n.Tag as ThingTypeInfo).Index == typeindex)
						{
							// Select this
							if(n.TreeView != null) //mxd. Tree node may've been removed during filtering
							{
								if(n.Parent != null) n.Parent.Expand(); // node won't have parent when the list is prefiltered
								typelist.SelectedNodes.Add(n);
								n.EnsureVisible();
								break;
							}
						}
					}
					doupdatetextbox = true;
				}
			}
			else
			{
				thinginfo = null;
				if(doupdatenode)
				{
					typelist.SelectedNodes.Clear();
					validnodes.Clear(); //mxd
				}
			}

			// No known thing?
			if(!knownthing)
			{
				sizelabel.Text = "-";
				positionlabel.Text = "-";
				blockinglabel.Text = "-";
			}

			//mxd. Update help link
			bool displayclassname = (thinginfo != null && !string.IsNullOrEmpty(thinginfo.ClassName) && !thinginfo.ClassName.StartsWith("$"));
			classname.Enabled = (displayclassname && !string.IsNullOrEmpty(General.Map.Config.ThingClassHelp));
			classname.Text = (displayclassname ? thinginfo.ClassName : "--");
			labelclassname.Enabled = classname.Enabled;

			// Update icon (mxd)
			UpdateThingSprite();

			// Raise event
			if(OnTypeChanged != null) OnTypeChanged(thinginfo);
		}

		private void updatetimer_Tick(object sender, EventArgs e) 
		{
			updatetimer.Stop();
			UpdateThingSprite();
		}

		//mxd
		private void typelist_MouseEnter(object sender, EventArgs e) 
		{
			typelist.Focus();
		}

		//mxd. Transfer focus to Filter textbox
		private void typelist_KeyPress(object sender, KeyPressEventArgs e)
		{
			tbFilter.Focus();
			if(e.KeyChar == '\b') // Any better way to check for Backspace?..
			{
				if(!string.IsNullOrEmpty(tbFilter.Text) && tbFilter.SelectionStart > 0 && tbFilter.SelectionLength == 0)
				{
					int s = tbFilter.SelectionStart - 1;
					tbFilter.Text = tbFilter.Text.Remove(s, 1);
					tbFilter.SelectionStart = s;
				}
			}
			else
			{
				tbFilter.AppendText(e.KeyChar.ToString(CultureInfo.InvariantCulture));
			}
		}

		//mxd
		private void bClear_Click(object sender, EventArgs e) 
		{
			tbFilter.Clear();
		}

		//mxd
		private void tbFilter_TextChanged(object sender, EventArgs e) 
		{
			typelist.SuspendLayout();

			if(string.IsNullOrEmpty(tbFilter.Text.Trim()))
			{
				Setup();
				typeid_TextChanged(this, EventArgs.Empty);
			}
			else
			{
				// Go for all predefined categories
				typelist.SelectedNodes.Clear();
				typelist.Nodes.Clear();
				validnodes.Clear();

				string match = tbFilter.Text.ToUpperInvariant();
				HashSet<string> added = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
				
				// First add nodes, which titles start with given text
				foreach(TreeNode node in nodes)
				{
					if(node.Text.ToUpperInvariant().StartsWith(match))
					{
						typelist.Nodes.Add(node);
						added.Add(node.Text);
					}
				}

				// Then add nodes, which titles contain given text
				foreach(TreeNode node in nodes)
				{
					if(!added.Contains(node.Text) && node.Text.ToUpperInvariant().Contains(match)) 
						typelist.Nodes.Add(node);
				}

				doupdatenode = true;
				doupdatetextbox = true;
			}

			typelist.ResumeLayout();
		}

		//mxd. Switch focus to types list?
		private void tbFilter_KeyUp(object sender, KeyEventArgs e)
		{
			if(e.KeyCode == Keys.Down && typelist.Nodes.Count > 0)
			{
				typelist.SelectedNodes.Clear();
				typelist.SelectedNodes.Add(typelist.Nodes[0]);
				typelist.Focus();
			}
		}

		//mxd. Because anchor-based alignment fails when using high-Dpi settings...
		private void ThingBrowserControl_Resize(object sender, EventArgs e)
		{
			infopanel.Top = this.Height - infopanel.Height;
			infopanel.Width = this.Width;
			spritepanel.Left = infopanel.Width - spritepanel.Width;
			typelist.Height = infopanel.Top - typelist.Top;
			typelist.Width = this.Width;
			bClear.Left = this.Width - bClear.Width - bClear.Margin.Right;
			tbFilter.Width = bClear.Left - tbFilter.Left - bClear.Margin.Left;
		}

		//mxd. If it's clickable, all data is valid.
		private void classname_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) 
		{
			General.OpenWebsite(General.Map.Config.ThingClassHelp.Replace("%K", thinginfo.ClassName));
		}

		//mxd. Switch to Open Folder icon
		private void typelist_BeforeExpand(object sender, TreeViewCancelEventArgs e)
		{
			// Category node?
			if(e.Node.ImageIndex > WARNING_ICON_INDEX)
				e.Node.ImageIndex = e.Node.ImageIndex - FOLDER_ICON_OFFSET + FOLDER_OPEN_ICON_OFFSET;
		}

		//mxd. Switch to Closed Folder icon
		private void typelist_BeforeCollapse(object sender, TreeViewCancelEventArgs e)
		{
			// Category node?
			if(e.Node.ImageIndex > WARNING_ICON_INDEX)
				e.Node.ImageIndex = e.Node.ImageIndex - FOLDER_OPEN_ICON_OFFSET + FOLDER_ICON_OFFSET;
		}
		
		#endregion
	}
}
