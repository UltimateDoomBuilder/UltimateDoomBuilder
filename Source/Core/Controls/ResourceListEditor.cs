
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
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.Windows;

#endregion

namespace CodeImp.DoomBuilder.Controls
{
	internal partial class ResourceListEditor : UserControl
	{
		#region ================== Internal warning struct
		class Warning
        {
			public Panel Wrapper;
			public PictureBox Picture;
			public Label Text;

			public Warning(Panel wrapper, PictureBox picture, Label text)
            {
				Wrapper = wrapper;
				Picture = picture;
				Text = text;
            }
        }
		#endregion

		#region ================== Delegates / Events

		public delegate void ContentChanged();
		public delegate void WarningsChanged(int size);
		public event ContentChanged OnContentChanged;
		public event WarningsChanged OnWarningsChanged;
		public string StartPath; //mxd
		public bool IsMapControl = false;

		#endregion

		#region ================== Variables

		private Point dialogoffset = new Point(40, 20);
		private readonly DataLocationList copiedresources; //mxd
		private readonly int copyactionkey;
		private readonly int cutactionkey;
		private readonly int pasteactionkey;
		private readonly int pastespecialactionkey;
		private readonly int deleteactionkey;
		private readonly Dictionary<string, CancellationTokenSource> loadingrequiredarchives;
		private readonly List<Warning> warnings;

		#endregion

		#region ================== Properties

		public Point DialogOffset { get { return dialogoffset; } set { dialogoffset = value; } }
		public GameConfiguration GameConfiguration { get; set; }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public ResourceListEditor()
		{
			// Initialize
			InitializeComponent();
			ResizeColumnHeader();

			loadingrequiredarchives = new Dictionary<string, CancellationTokenSource>();
			warnings = new List<Warning>();

			if (General.Actions != null)
			{
				// Get key shortcuts (mxd)
				copyactionkey = General.Actions.GetActionByName("builder_copyselection").ShortcutKey;
				cutactionkey = General.Actions.GetActionByName("builder_cutselection").ShortcutKey;
				pasteactionkey = General.Actions.GetActionByName("builder_pasteselection").ShortcutKey;
				pastespecialactionkey = General.Actions.GetActionByName("builder_pasteselectionspecial").ShortcutKey;
				deleteactionkey = General.Actions.GetActionByName("builder_deleteitem").ShortcutKey;

				// Set displayed shortcuts (mxd)
				copyresources.ShortcutKeyDisplayString = Actions.Action.GetShortcutKeyDesc(copyactionkey);
				cutresources.ShortcutKeyDisplayString = Actions.Action.GetShortcutKeyDesc(cutactionkey);
				pasteresources.ShortcutKeyDisplayString = Actions.Action.GetShortcutKeyDesc(pasteactionkey);
				replaceresources.ShortcutKeyDisplayString = Actions.Action.GetShortcutKeyDesc(pastespecialactionkey);
				removeresources.ShortcutKeyDisplayString = Actions.Action.GetShortcutKeyDesc(deleteactionkey);
			}

			// Start with a clear list
			resourceitems.Items.Clear();
			copiedresources = new DataLocationList(); //mxd

			Resize += HandleResize;
			HandleResize(null, null);
		}

		#endregion

		#region ================== Methods

		// This gets the icon index for a resource location type
		private int GetIconIndex(int locationtype, bool locked)
		{
			int lockedaddition;
			
			// Locked?
			if(locked) lockedaddition = (images.Images.Count / 2);
				else lockedaddition = 0;
			
			// What type?
			switch(locationtype)
			{
				case DataLocation.RESOURCE_DIRECTORY: return 0 + lockedaddition;
				case DataLocation.RESOURCE_WAD: return 1 + lockedaddition;
				case DataLocation.RESOURCE_PK3: return 2 + lockedaddition;
				default: return -1;
			}
		}

		private List<string> RunCheckRequiredArchives(DataLocation loc, CancellationToken token)
		{
			return ResourceOptionsForm.CheckRequiredArchives(GameConfiguration, loc, token);
		}

		private async void StartRequiredArchivesCheck(string location)
		{
			if (GameConfiguration == null) return;

			DataLocation loc = new DataLocation();
			bool found = false;

			foreach (ListViewItem item in resourceitems.Items)
            {
				DataLocation dl = (DataLocation)item.Tag;
				if (dl.location == location)
                {
					loc = dl;
					found = true;
					break;
                }
            }

			if (!found) return;

			var cancellation = new CancellationTokenSource();

			General.WriteLogLine(string.Format("Resource check started for: {0}", loc.location));

			loadingrequiredarchives.Add(location, cancellation);
			RefreshLoading();

			try
			{
				loc.requiredarchives = await Task.Run(() => RunCheckRequiredArchives(loc, cancellation.Token));
				
				// in case of dir, option1/2 should be erased
				if (loc.type == DataLocation.RESOURCE_DIRECTORY)
					loc.option1 = loc.option2 = false;
				// check if it has to be force-excluded from testing
				foreach (var arc in GameConfiguration.RequiredArchives)
				{
					if (loc.requiredarchives.Contains(arc.ID) && arc.ExcludeFromTesting)
						loc.notfortesting = true;
				}

				foreach (ListViewItem item in resourceitems.Items)
				{
					if (((DataLocation)item.Tag).location == location)
					{
						item.Tag = loc;
						if (OnContentChanged != null) OnContentChanged();
						break;
					}
				}
			}
			catch (Exception e)
			{
				loc.requiredarchives = new List<string>();
				General.WriteLogLine(e.ToString());
			}

			cancellation.Dispose();

			// Loading might have been canceled outside of here, so make sure that the location is still in the list.
			// See https://github.com/jewalky/UltimateDoomBuilder/issues/813
			if (loadingrequiredarchives.ContainsKey(location) && loadingrequiredarchives[location] == cancellation)
			{
				General.WriteLogLine(string.Format("Resource check completed for: {0} (Match = {1}, RequiredArchives = {2})", location, loadingrequiredarchives[location] == cancellation, string.Join(",", loc.requiredarchives)));
				loadingrequiredarchives.Remove(location);
				RefreshLoading();
			}

			// if nothing is loading, update warnings if any
			if (loadingrequiredarchives.Count == 0)
				UpdateWarnings();
		}

		private void ShowWarning(string text, bool loading)
        {
			Panel p = new Panel();
			Controls.Add(p);

			// find offset
			int lastTop = 0;
			foreach (Warning w in warnings)
				lastTop = Math.Max(lastTop, w.Wrapper.Bottom + 8);

			p.Top = lastTop;
			p.Left = 0;
			p.Width = Width;
			p.Height = 48;
			p.BackColor = SystemColors.Info;
			p.BorderStyle = BorderStyle.FixedSingle;
			p.ForeColor = SystemColors.InfoText;

			PictureBox pb = new PictureBox();
			pb.Width = 16;
			pb.Height = 16;
			pb.Left = 8;
			pb.Top = 8;
			pb.Image = loading ? Properties.Resources.Loader: Properties.Resources.Warning;
			p.Controls.Add(pb);

			Label l = new Label();
			l.Left = 8 + 16 + 8;
			l.Top = 10;
			l.MinimumSize = new Size(Width - 32 - 8, 0);
			l.MaximumSize = new Size(Width - 32 - 8, 640);
			l.Width = l.MinimumSize.Width;
			l.Height = 48;
			l.Text = text;
			l.AutoSize = true;
			p.Controls.Add(l);

			// resize panel
			p.Height = 22 + l.Height;

			Controls.SetChildIndex(p, 0);

			warnings.Add(new Warning(p, pb, l));

			lastTop = p.Bottom + 8;

			resourceitems.Height = Height - lastTop - 32;
			resourceitems.Top = lastTop;
        }

		private int GetWarningsHeight()
        {
			int lastTop = 0;
			foreach (Warning w in warnings)
				lastTop = Math.Max(lastTop, w.Wrapper.Bottom + 8);

			return lastTop;
		}

		private void UpdateWarnings()
        {
			int lastH = GetWarningsHeight();

			foreach (Warning w in warnings)
				w.Wrapper.Dispose();

			warnings.Clear();

			HandleResize(this, null);

			List<string> requiredarchives = new List<string>();
			foreach (ListViewItem item in resourceitems.Items)
			{
				DataLocation loc = (DataLocation)item.Tag;
				if (loc.requiredarchives != null)
					requiredarchives.AddRange(loc.requiredarchives);
			}

			General.WriteLogLine(string.Format("Archive check: RequiredArchives = {0}", string.Join(",", requiredarchives)));

			// warning 1: you do not have a required file
			if (GameConfiguration != null)
            {
				foreach (RequiredArchive arc in GameConfiguration.RequiredArchives)
                {
					if (!requiredarchives.Contains(arc.ID))
						ShowWarning(string.Format("Warning: a resource archive is required for this game configuration, but not present:\n  \"{0}\"\nWithout it, UDB will have severely limited capabilities.", arc.FileName), false);
                }
            }

			// warning 2: map without any resources. this makes sense only on map open dialog and not game configurations dialog
			if (IsMapControl)
            {
				if (resourceitems.Items.Count == 0)
					ShowWarning("Warning: you are about to edit a map without any resources.\nTextures, flats and sprites may not be shown correctly or may not show up at all.", false);
            }

			// warning 3: multiple instances of the same required file
			if (GameConfiguration != null)
			{
				for (int i = 0; i < requiredarchives.Count; i++)
				{
					if (requiredarchives.IndexOf(requiredarchives[i]) != i)
					{
						foreach (RequiredArchive arc in GameConfiguration.RequiredArchives)
						{
							if (arc.ID == requiredarchives[i])
								ShowWarning(string.Format("Warning: required archive was added more than once:\n  \"{0}\"\nThis will most likely not work.", arc.FileName), false);
						}
					}
				}
			}

			int h = GetWarningsHeight();
			if (lastH != h && OnWarningsChanged != null)
			{
				OnWarningsChanged(h);
				// possibly recalculate size
				HandleResize(this, null);
			}
        }

		private void RefreshLoading()
        {
			resourceitems.BeginUpdate();

			bool anyLoading = false;

			foreach (ListViewItem item in resourceitems.Items)
            {
				DataLocation dl = (DataLocation)item.Tag;
				if (IsLoading(dl.location))
				{
					item.ImageIndex = GetLoaderIndex();
					anyLoading |= true;
				}
				else item.ImageIndex = GetIconIndex(dl.type, item.ForeColor != SystemColors.WindowText);
            }

			resourceitems.EndUpdate();

			if (anyLoading)
            {
				foreach (Warning w in warnings)
					w.Picture.Image = Properties.Resources.Loader;
            }
			else
            {
				foreach (Warning w in warnings)
					w.Picture.Image = Properties.Resources.Warning;
			}
		}

		private void CancelLoading(string location)
        {
			General.WriteLogLine(string.Format("Resource check cancelled for: {0}", location));
			if (loadingrequiredarchives.ContainsKey(location))
			{
				loadingrequiredarchives[location].Cancel();
				loadingrequiredarchives.Remove(location);
			}
        }

		private bool IsLoading(string location)
        {
			return loadingrequiredarchives.ContainsKey(location);
        }

		private int GetLoaderIndex()
        {
			return 6;
        }
		
		// This will show a fixed list
		public void FixedResourceLocationList(DataLocationList list)
		{
			// Start editing list
			resourceitems.BeginUpdate();

			//mxd
			HashSet<DataLocation> currentitems = new HashSet<DataLocation>();
			
			// Go for all items
			for(int i = resourceitems.Items.Count - 1; i >= 0; i--)
			{
				// Remove item if not fixed
				if (resourceitems.Items[i].ForeColor != SystemColors.WindowText)
				{
					CancelLoading(((DataLocation)resourceitems.Items[i].Tag).location);
					resourceitems.Items.RemoveAt(i);
				}
				else
					currentitems.Add((DataLocation)resourceitems.Items[i].Tag); //mxd
			}
			
			// Go for all items
			for(int i = list.Count - 1; i >= 0; i--)
			{
				if(currentitems.Contains(list[i])) continue; //mxd
				currentitems.Add(list[i]); //mxd
				
				// Add item as fixed
				resourceitems.Items.Insert(0, new ListViewItem(list[i].location));
				resourceitems.Items[0].Tag = list[i];
				resourceitems.Items[0].ImageIndex = IsLoading(list[i].location) ? GetLoaderIndex() : GetIconIndex(list[i].type, true);

				// Set disabled
				resourceitems.Items[0].ForeColor = SystemColors.GrayText;

				// Validate path (mxd)
				resourceitems.Items[0].BackColor = (list[i].IsValid() ? resourceitems.BackColor : Color.MistyRose);

				// Check if resource has no info about membership in Game configuration's requiredarchives
				// This normally happens if it was imported from old DBS or drag-dropped
				if (list[i].requiredarchives == null)
					StartRequiredArchivesCheck(list[i].location);
			}

			// Done
			resourceitems.EndUpdate();

			if (loadingrequiredarchives.Count == 0)
				UpdateWarnings();
		}

		// This will edit the given list
		public void EditResourceLocationList(DataLocationList list)
		{
			// Start editing list
			resourceitems.BeginUpdate();

			// Scroll to top
			if(resourceitems.Items.Count > 0)
				resourceitems.TopItem = resourceitems.Items[0];
			
			// Go for all items
			for(int i = resourceitems.Items.Count - 1; i >= 0; i--)
			{
				// Remove item unless fixed
				if (resourceitems.Items[i].ForeColor == SystemColors.WindowText)
				{
					CancelLoading(((DataLocation)resourceitems.Items[i].Tag).location);
					resourceitems.Items.RemoveAt(i);
				}
			}

			// Go for all items
			foreach(DataLocation dl in list)
			{
				// Add item
				AddItem(dl);
			}

			if (loadingrequiredarchives.Count == 0)
				UpdateWarnings();

			// Done
			resourceitems.EndUpdate();
			ResizeColumnHeader();
			
			// Raise content changed event
			if(OnContentChanged != null) OnContentChanged();
		}

		// This adds a normal item
		/*public void AddResourceLocation(DataLocation rl)
		{
			// Add it
			AddItem(rl);

			// Raise content changed event
			if(OnContentChanged != null) OnContentChanged();
		}*/

		// This adds a normal item
		private bool AddItem(DataLocation rl)
		{
			//mxd. No duplicates ples
			foreach(ListViewItem item in resourceitems.Items)
				if(((DataLocation)item.Tag).location == rl.location) return false;

			// Start editing list
			resourceitems.BeginUpdate();

			// Add item
			int index = resourceitems.Items.Count;
			resourceitems.Items.Add(new ListViewItem(rl.location));
			resourceitems.Items[index].Tag = rl;
			resourceitems.Items[index].ImageIndex = IsLoading(rl.location) ? GetLoaderIndex() : GetIconIndex(rl.type, false);
			
			// Set normal color
			resourceitems.Items[index].ForeColor = SystemColors.WindowText;

			// Validate path (mxd)
			resourceitems.Items[index].BackColor = (rl.IsValid() ? resourceitems.BackColor : Color.MistyRose);

			// Done
			resourceitems.EndUpdate();

			// Check if resource has no info about membership in Game configuration's requiredarchives
			// This normally happens if it was imported from old DBS or drag-dropped
			if (rl.requiredarchives == null)
				StartRequiredArchivesCheck(rl.location);

			return true;
		}
		
		// This fixes the column header in the list
		private void ResizeColumnHeader()
		{
			// Resize column header to full extend
			column.Width = resourceitems.ClientSize.Width - SystemInformation.VerticalScrollBarWidth;
		}

		// When the resources list resizes
		private void resources_SizeChanged(object sender, EventArgs e)
		{
			// Resize column header also
			ResizeColumnHeader();
		}

		// Add a resource
		private void addresource_Click(object sender, EventArgs e)
		{
			// Open resource options dialog
			ResourceOptionsForm resoptions = new ResourceOptionsForm(new DataLocation(), "Add Resource", StartPath);
			resoptions.GameConfiguration = GameConfiguration;
			resoptions.StartPosition = FormStartPosition.Manual;
			Rectangle startposition = new Rectangle(dialogoffset.X, dialogoffset.Y, 1, 1);
			startposition = this.RectangleToScreen(startposition);
			Screen screen = Screen.FromPoint(startposition.Location);
			if(startposition.X + resoptions.Size.Width > screen.WorkingArea.Right)
				startposition.X = screen.WorkingArea.Right - resoptions.Size.Width;
			if(startposition.Y + resoptions.Size.Height > screen.WorkingArea.Bottom)
				startposition.Y = screen.WorkingArea.Bottom - resoptions.Size.Height;
			resoptions.Location = startposition.Location;
			if(resoptions.ShowDialog(this) == DialogResult.OK)
			{
				// Add resource
				if(!AddItem(resoptions.ResourceLocation))
				{
					General.Interface.DisplayStatus(StatusType.Warning, "Resource already added!"); //mxd
					return; //mxd
				}

				if (loadingrequiredarchives.Count == 0)
					UpdateWarnings();
			}

			// Raise content changed event
			if(OnContentChanged != null) OnContentChanged();
		}

		// Edit resource
		private void editresource_Click(object sender, EventArgs e)
		{
			// Anything selected?
			if(resourceitems.SelectedItems.Count == 1)
			{
				// Get selected item
				ListViewItem selecteditem = resourceitems.SelectedItems[0];

				// Open resource options dialog
				ResourceOptionsForm resoptions = new ResourceOptionsForm((DataLocation)selecteditem.Tag, "Resource Options", StartPath);
				resoptions.GameConfiguration = GameConfiguration;
				resoptions.StartPosition = FormStartPosition.Manual;
				Rectangle startposition = new Rectangle(dialogoffset.X, dialogoffset.Y, 1, 1);
				startposition = this.RectangleToScreen(startposition);
				Screen screen = Screen.FromPoint(startposition.Location);
				if(startposition.X + resoptions.Size.Width > screen.WorkingArea.Right)
					startposition.X = screen.WorkingArea.Right - resoptions.Size.Width;
				if(startposition.Y + resoptions.Size.Height > screen.WorkingArea.Bottom)
					startposition.Y = screen.WorkingArea.Bottom - resoptions.Size.Height;
				resoptions.Location = startposition.Location;
				if(resoptions.ShowDialog(this) == DialogResult.OK)
				{
					// Start editing list
					resourceitems.BeginUpdate();

					// Update item
					DataLocation rl = resoptions.ResourceLocation;
					selecteditem.Text = rl.location;
					selecteditem.Tag = rl;
					selecteditem.ImageIndex = IsLoading(rl.location) ? GetLoaderIndex() : GetIconIndex(rl.type, false);
					
					// Done
					resourceitems.EndUpdate();

					// Check if resource has no info about membership in Game configuration's requiredarchives
					// This normally happens if it was imported from old DBS or drag-dropped
					if (rl.requiredarchives == null)
						StartRequiredArchivesCheck(rl.location);
					else if (loadingrequiredarchives.Count == 0)
						UpdateWarnings();

					// Raise content changed event
					if (OnContentChanged != null) OnContentChanged();
				}
			}
		}

		// Remove resource
		private void deleteresources_Click(object sender, EventArgs e)
		{
			DeleteSelectedResources(); //mxd
		}
		
		// Item selected
		private void resourceitems_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
		{
			// Anything selected
			if(resourceitems.SelectedItems.Count > 0)
			{
				// Go for all selected items
				for(int i = resourceitems.SelectedItems.Count - 1; i >= 0; i--)
				{
					// Item grayed? Then deselect.
					if(resourceitems.SelectedItems[i].ForeColor != SystemColors.WindowText)
						resourceitems.SelectedItems[i].Selected = false;
				}
			}

			// Anything selected
			if(resourceitems.SelectedItems.Count > 0)
			{
				// Enable buttons
				editresource.Enabled = (resourceitems.SelectedItems.Count == 1);
				deleteresources.Enabled = true;
			}
			else
			{
				// Disable buttons
				editresource.Enabled = false;
				deleteresources.Enabled = false;
			}
		}

		// When an item is double clicked
		private void resourceitems_DoubleClick(object sender, EventArgs e)
		{
			// Click the edit resource button
			if(editresource.Enabled) editresource_Click(sender, e);
		}

		// Returns a list of the resources
		public DataLocationList GetResources()
		{
			DataLocationList list = new DataLocationList();

			// Go for all items
			for(int i = 0; i < resourceitems.Items.Count; i++)
			{
				// Item not grayed?
				if(resourceitems.Items[i].ForeColor == SystemColors.WindowText)
				{
					// Add item to list
					DataLocation dl = (DataLocation)resourceitems.Items[i].Tag;
					if(!list.Contains(dl)) list.Add(dl); //mxd. Duplicates check
				}
			}

			// Return result
			return list;
		}

		//mxd
		public bool ResourcesAreValid()
		{
			foreach(ListViewItem item in resourceitems.Items)
			{
				if(!((DataLocation)item.Tag).IsValid()) return false;
			}
			return true;
		}

		// Item dragged
		private void resourceitems_DragOver(object sender, DragEventArgs e)
		{
			// Raise content changed event
			if(OnContentChanged != null) OnContentChanged();
		}

		// Item dropped
		private void resourceitems_DragDrop(object sender, DragEventArgs e)
		{
			//mxd. Accept filesystem drop
			if(e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				string[] paths = (string[])e.Data.GetData(DataFormats.FileDrop);
				int addedfiles = 0;
				foreach(string path in paths)
				{
					if(File.Exists(path))
					{
						string ext = Path.GetExtension(path);
						if(string.IsNullOrEmpty(ext)) continue;
						switch(ext.ToLower())
						{
							case ".wad":
                            case ".iwad":
								if(AddItem(new DataLocation(DataLocation.RESOURCE_WAD, path, false, false, false, null))) addedfiles++;
								break;
							case ".pk7":
							case ".pk3":
							case ".pke":
                            case ".ipk3":
                            case ".ipk7":
								if(AddItem(new DataLocation(DataLocation.RESOURCE_PK3, path, false, false, false, null))) addedfiles++;
								break;
						}
					}
					else if(Directory.Exists(path))
					{
						if(AddItem(new DataLocation(DataLocation.RESOURCE_DIRECTORY, path, false, false, false, null))) addedfiles++;
					}
				}

				if(addedfiles == 0)
				{
					General.Interface.DisplayStatus(StatusType.Warning, "Invalid or duplicate resources!");
					return;
				}
			}

			if (loadingrequiredarchives.Count == 0)
				UpdateWarnings();

			// Raise content changed event
			if (OnContentChanged != null) OnContentChanged();
		}

		// Client size changed
		private void resourceitems_ClientSizeChanged(object sender, EventArgs e)
		{
			// Resize column header
			ResizeColumnHeader();
		}

		#endregion

		#region ================== Copy / Paste (mxd)

		private void CopySelectedResources()
		{
			// Don't do stupid things
			if(resourceitems.SelectedItems.Count == 0) return;

			copiedresources.Clear();
			foreach(ListViewItem item in resourceitems.SelectedItems) 
				if(item.Tag is DataLocation) copiedresources.Add((DataLocation)item.Tag);

			// Display notification
			General.Interface.DisplayStatus(StatusType.Info, copiedresources.Count + " Resource" + (copiedresources.Count > 1 ? "s" : "") + " Copied to Clipboard");
		}

		private void PasteResources()
		{
			// Don't do stupid things
			if(copiedresources.Count == 0) return;

			int pastedcount = 0;
			foreach(DataLocation dl in copiedresources) 
				if(AddItem(dl)) pastedcount++;

			if(pastedcount > 0) 
			{
				ResizeColumnHeader();

				// Display notification
				General.Interface.DisplayStatus(StatusType.Info, pastedcount + " Resource" + (pastedcount > 1 ? "s" : "") + " Pasted");

				if (loadingrequiredarchives.Count == 0)
					UpdateWarnings();

				// Raise content changed event
				if (OnContentChanged != null) OnContentChanged();
			}
		}

		private void ReplaceResources()
		{
			// Don't do stupid things
			if(copiedresources.Count == 0) return;

			int pastedcount = 0;

			// Delete non-fixed resources
			for(int i = resourceitems.Items.Count - 1; i > -1; i--) 
			{
				if(resourceitems.Items[i].ForeColor != SystemColors.WindowText) break;
				resourceitems.Items.Remove(resourceitems.Items[i]);
				pastedcount++;
			}

			// Paste new resources
			foreach(DataLocation dl in copiedresources) 
				if(AddItem(dl)) pastedcount++;

			if(pastedcount > 0) 
			{
				ResizeColumnHeader();

				// Display notification
				General.Interface.DisplayStatus(StatusType.Info, pastedcount + " Resource" + (pastedcount > 1 ? "s" : "") + " Replaced");

				if (loadingrequiredarchives.Count == 0)
					UpdateWarnings();

				// Raise content changed event
				if (OnContentChanged != null) OnContentChanged();
			}
		}

		private void DeleteSelectedResources()
		{
			// Don't do stupid things
			if(resourceitems.SelectedItems.Count == 0) return;

			// Remove them (mxd)
			foreach(ListViewItem item in resourceitems.SelectedItems) 
			{
				// Remove item unless fixed
				if(item.ForeColor == SystemColors.WindowText) resourceitems.Items.Remove(item);
			}

			ResizeColumnHeader();

			if (loadingrequiredarchives.Count == 0)
				UpdateWarnings();

			// Raise content changed event
			if (OnContentChanged != null) OnContentChanged();
		}

		#endregion

		#region ================== Copy / Paste Events (mxd)

		private void copyresources_Click(object sender, EventArgs e)
		{
			CopySelectedResources();
		}

		private void cutresources_Click(object sender, EventArgs e) 
		{
			CopySelectedResources();
			DeleteSelectedResources();
		}

		private void pasteresources_Click(object sender, EventArgs e)
		{
			PasteResources();
		}

		private void replaceresources_Click(object sender, EventArgs e)
		{
			ReplaceResources();
		}

		private void removeresources_Click(object sender, EventArgs e) 
		{
			DeleteSelectedResources();
		}

		// Update copy/paste menu buttons
		private void copypastemenu_Opening(object sender, System.ComponentModel.CancelEventArgs e)
		{
			copyresources.Enabled = resourceitems.SelectedItems.Count > 0;
			cutresources.Enabled = resourceitems.SelectedItems.Count > 0;
			pasteresources.Enabled = copiedresources.Count > 0;
			replaceresources.Enabled = copiedresources.Count > 0;
			removeresources.Enabled = resourceitems.SelectedItems.Count > 0;
		}

		private void resourceitems_KeyUp(object sender, KeyEventArgs e) 
		{
			if(sender != resourceitems) return;

			if((int)e.KeyData == copyactionkey) CopySelectedResources();
			else if((int)e.KeyData == pasteactionkey) PasteResources();
			else if((int)e.KeyData == pastespecialactionkey) ReplaceResources();
			else if((int)e.KeyData == deleteactionkey) DeleteSelectedResources();
			else if((int)e.KeyData == cutactionkey)
			{
				CopySelectedResources();
				DeleteSelectedResources();
			}
		}

		#endregion


		#region ================== Events (mxd)

		//mxd. Because anchor-based alignment fails when using high-Dpi settings...
		private void HandleResize(object sender, EventArgs e)
		{
			int warningsH = GetWarningsHeight();

			resourceitems.Width = this.Width;
			resourceitems.Top = warningsH;
			resourceitems.Left = 0;
			resourceitems.Height = this.Height - addresource.Height - addresource.Margin.Top - addresource.Margin.Bottom - resourceitems.Top;

			addresource.Top = resourceitems.Bottom + addresource.Margin.Top;
			editresource.Top = addresource.Top;
			deleteresources.Top = addresource.Top;

			int buttonwidth = (this.Width - addresource.Margin.Left * 2) / 3;
			addresource.Width = buttonwidth;
			editresource.Width = buttonwidth;
			deleteresources.Width = buttonwidth;

			addresource.Left = 0;
			editresource.Left = addresource.Right + addresource.Margin.Left;
			deleteresources.Left = editresource.Right + addresource.Margin.Left;
		}

		#endregion
	}
}
