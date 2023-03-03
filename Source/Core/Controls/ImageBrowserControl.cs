
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
using System.Linq; // biwa
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.Windows;

#endregion

namespace CodeImp.DoomBuilder.Controls
{
	/// <summary>
	/// Holds a title of an item group and its items
	/// </summary>
	internal struct ImageBrowserItemGroup
	{
		public string Title;
		public List<ImageBrowserItem> Items;

		public ImageBrowserItemGroup(string title)
		{
			Title = title;
			Items = new List<ImageBrowserItem>();
		}
	}

	internal partial class ImageBrowserControl : UserControl
	{
		#region ================== Constants
		
		private static readonly HashSet<char> AllowedSpecialChars = new HashSet<char>("!@#$%^&*()-_=+<>,.?/'\"\\;:[]{}`~".ToCharArray()); //mxd

		#endregion
		
		#region ================== Delegates / Events

		public delegate void SelectedItemChangedDelegate(ImageBrowserItem item);
		public delegate void SelectedItemDoubleClickDelegate(ImageBrowserItem item);

		public event SelectedItemChangedDelegate SelectedItemChanged;
		public event SelectedItemDoubleClickDelegate SelectedItemDoubleClicked;
		
		#endregion

		#region ================== Variables
		
		// Properties
		private bool preventselection;
        private int imagesize;
		
		// States
		private int keepselected;
		private bool browseflats; //mxd
		private bool uselongtexturenames; //mxd
		private bool blockupdate; //mxd
		
		//mxd. All items
		private List<ImageBrowserItem> items;
		private string usedfirstgroup;
		private string availgroup;

		//mxd
		private int texturetype;
		
		#endregion

		#region ================== Properties

		public bool PreventSelection { get { return preventselection; } set { preventselection = value; } }
		public bool HideInputBox { get { return splitter.Panel2Collapsed; } set { splitter.Panel2Collapsed = value; } }
		public List<ImageBrowserItem> SelectedItems { get { return list.SelectedItems; } } //mxd
		public ImageBrowserItem SelectedItem { get { return (list.SelectedItems.Count > 0 ? list.SelectedItems[0] : null); } }
		public string ElementName //mxd
		{
			set
			{
				usedfirstgroup = "Available " + value + " (used first):";
				availgroup = "Available " + value + ":";
                list.ContentType = value;
            }
		}

        public int ImageSize
        {
            get { return imagesize; }
            set { imagesize = value; }
        }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public ImageBrowserControl()
		{
			// Initialize
			InitializeComponent();
			items = new List<ImageBrowserItem>();

			//mxd
			StepsList sizes = new StepsList { 4, 8, 16, 32, 48, 64, 96, 128, 192, 256, 512, 1024 };
			filterWidth.StepValues = sizes;
			filterHeight.StepValues = sizes;

			//mxd. Looks like SplitterDistance is unaffected by DPI scaling. Let's fix that...
			if(MainForm.DPIScaler.Height != 1.0f)
			{
				splitter.SplitterDistance = splitter.Height - splitter.Panel2.Height - (int)Math.Round(splitter.SplitterWidth * MainForm.DPIScaler.Height);
			}

			//mxd
			list.SelectionChanged += list_SelectionChanged;
		}

		// This applies the application settings
		public void ApplySettings(string settingpath, bool browseflats)
		{
			blockupdate = true;

			this.browseflats = browseflats;
            uselongtexturenames = General.Map.Options.UseLongTextureNames;
			texturetype = General.Settings.ReadSetting(settingpath + ".texturetype", 0);
            ElementName = (texturetype == 2 || (texturetype == 3 && browseflats)) ? "flats" : "textures";
            list.UsedTexturesFirst = usedtexturesfirst.Checked = General.Settings.ReadSetting(settingpath + ".showusedtexturesfirst", false);
            list.ClassicView = classicview.Checked = General.Settings.ReadSetting(settingpath + ".classicview", false);
			list.CenterItem = centeritem.Checked = General.Settings.ReadSetting(settingpath + ".verticallycenteritem", true);
			
			int _imagesize = General.Settings.ReadSetting(settingpath + ".imagesize", 128);
			sizecombo.Text = (_imagesize == 0 ? sizecombo.Items[0].ToString() : _imagesize.ToString());
			list.ImageSize = _imagesize;

			ApplySettings();

			blockupdate = false;
		}

		// This applies the application settings
		public void ApplySettings()
		{
			// Force black background?
			if(General.Settings.BlackBrowsers)
			{
				list.BackColor = Color.Black;
				list.ForeColor = Color.White;
			}

			// Set the size of preview images
			if(General.Map != null)
			{
				//mxd
				if(General.Map.Config.MixTexturesFlats) 
				{
					texturetypecombo.SelectedIndex = texturetype;
				} 
				else 
				{
					labelMixMode.Enabled = false;
					texturetypecombo.Enabled = false;
					texturetype = 0;
				}

				//mxd. Use long texture names?
				longtexturenames.Checked = (uselongtexturenames && General.Map.Config.UseLongTextureNames);
				longtexturenames.Enabled = General.Map.Config.UseLongTextureNames;
			}
			else
			{
				longtexturenames.Enabled = false; //mxd
				uselongtexturenames = false; //mxd
			}

            // If we have override for preview images, set this here.
            if (imagesize > 0) list.ImageSize = imagesize;

			//mxd
			objectname.CharacterCasing = (longtexturenames.Checked ? CharacterCasing.Normal : CharacterCasing.Upper);
		}

		//mxd. Save settings
		public virtual void OnClose(string settingpath)
		{
			General.Settings.WriteSetting(settingpath + ".showusedtexturesfirst", usedtexturesfirst.Checked);
            General.Settings.WriteSetting(settingpath + ".classicview", classicview.Checked);
			General.Settings.WriteSetting(settingpath + ".verticallycenteritem", list.CenterItem);
			General.Settings.WriteSetting(settingpath + ".imagesize", list.ImageSize);
			General.Settings.WriteSetting(settingpath + ".texturetype", texturetype);
			
			if (General.Map.Config.UseLongTextureNames) General.Map.Options.UseLongTextureNames = uselongtexturenames;

			CleanUp();
		}

		// This cleans everything up
		public virtual void CleanUp()
		{
			// Stop refresh timer
			refreshtimer.Enabled = false;
		}

		#endregion

		#region ================== Rendering

		// Refresher
		private void refreshtimer_Tick(object sender, EventArgs e)
		{
			bool allpreviewsloaded = true;
			bool redrawneeded = false; //mxd
			
			// Go for all items
			foreach(ImageBrowserItem i in list.Items)
			{
				// Check if there are still previews that are not loaded
				allpreviewsloaded &= i.IsPreviewLoaded;

				//mxd. Item needs to be redrawn?
				redrawneeded |= i.CheckRedrawNeeded();
			}

			// If all previews were loaded, stop this timer
			if(allpreviewsloaded) refreshtimer.Stop();

			// Redraw the list if needed
			if(redrawneeded) list.Invalidate();
		}

		#endregion

		#region ================== Events

		// Name typed
		private void objectname_TextChanged(object sender, EventArgs e)
		{
			// Update list
			RefillList(false);

			// No item selected?
			if(list.SelectedItems.Count == 0)
			{
				// Select first
				SelectFirstItem();
			}
		}

		// Key pressed in textbox
		private void objectname_KeyDown(object sender, KeyEventArgs e)
		{
			// Toggle used items sorting
			if(e.KeyData == Keys.Tab)
			{
				usedtexturesfirst.Checked = !usedtexturesfirst.Checked;
				e.SuppressKeyPress = true;
			}
			//mxd. Clear text field instead of typing strange chars...
			else if(e.KeyData == (Keys.Back | Keys.Control))
			{
				if(objectname.Text.Length > 0) objectname.Clear();
				e.SuppressKeyPress = true;
			}
		}

		//mxd
		private void objectclear_Click(object sender, EventArgs e)
		{
			objectname.Clear();
			list.Focus();
		}

		//mxd
		private void filterSize_WhenTextChanged(object sender, EventArgs e) 
		{
			objectname_TextChanged(sender, e);
		}

		//mxd
		protected override bool ProcessTabKey(bool forward)
		{
			if (list.SelectedItems.Count == 0)
				return false;

			list.JumpBetweenItems(list.SelectedItems[0]);

			return false;
		}
		
		// Selection changed
		private void list_SelectionChanged(object sender, List<ImageBrowserItem> selection)
		{
			// Prevent selecting?
			if(preventselection)
			{
				if(selection.Count > 0) list.ClearSelection(); //mxd
			}
			else
			{
				// Raise event
				if(SelectedItemChanged != null)
					SelectedItemChanged(list.SelectedItems.Count > 0 ? list.SelectedItems[0] : null);
			}
		}
		
		// Doublelicking an item
		private void list_ItemDoubleClicked(object sender, ImageBrowserItem item)
		{
			if(!preventselection && (list.SelectedItems.Count > 0))
				if(SelectedItemDoubleClicked != null) SelectedItemDoubleClicked(item);
		}

		//mxd. Transfer input to Filter textbox
		private void list_KeyPress(object sender, KeyPressEventArgs e)
		{
			if(e.KeyChar == 8) // Backspace
			{
				if(objectname.Text.Length > 0)
				{
					if(objectname.SelectionLength > 0)
					{
						objectname.Text = objectname.Text.Substring(0, objectname.SelectionStart) +
							objectname.Text.Substring(objectname.SelectionStart + objectname.SelectionLength);
					}
					else
					{
						objectname.Text = objectname.Text.Substring(0, objectname.Text.Length - 1);
					}
				}
			}
			else if(e.KeyChar == 127) // Ctrl-Backspace
			{
				if(objectname.Text.Length > 0) objectname.Clear();
			}
			else if((e.KeyChar >= 'a' && e.KeyChar <= 'z') || (e.KeyChar >= '0' && e.KeyChar <= '9') || AllowedSpecialChars.Contains(e.KeyChar))
			{
				if(objectname.SelectionLength > 0)
				{
					objectname.Text = objectname.Text.Substring(0, objectname.SelectionStart) +
										 e.KeyChar +
										 objectname.Text.Substring(objectname.SelectionStart + objectname.SelectionLength);
				}
				else
				{
					objectname.Text += e.KeyChar;
				}
			}
		}

		//mxd
		private void texturetypecombo_SelectedIndexChanged(object sender, EventArgs e) 
		{
			texturetype = texturetypecombo.SelectedIndex;
            ElementName = (texturetype == 2 || (texturetype == 3 && browseflats)) ? "flats" : "textures";

            RefillList(false);
		}

		//mxd
		private void sizecombo_SelectedIndexChanged(object sender, EventArgs e)
		{
			if(blockupdate) return;
			list.ImageSize = (sizecombo.SelectedIndex == 0 ? 0 : Convert.ToInt32(sizecombo.SelectedItem));
			list.Focus();
        }

		//mxd
		private void longtexturenames_CheckedChanged(object sender, EventArgs e)
		{
			if(!blockupdate)
			{
				uselongtexturenames = longtexturenames.Checked;
				objectname.CharacterCasing = (uselongtexturenames ? CharacterCasing.Normal : CharacterCasing.Upper);

				foreach(var item in items) item.ShowFullName = uselongtexturenames;
				list.UpdateRectangles();
				list.Focus();
			}
		}

		//mxd
		private void usedtexturesfirst_CheckedChanged(object sender, EventArgs e)
		{
			if(!blockupdate)
			{
                list.UsedTexturesFirst = usedtexturesfirst.Checked;
                RefillList(false);
				list.UpdateRectangles();
				list.Focus();
			}
		}

        //
        private void classicview_CheckedChanged(object sender, EventArgs e)
        {
            if(!blockupdate)
            {
                list.ClassicView = classicview.Checked;
                list.Focus();
            }
        }

        #endregion

        #region ================== Methods

        // This selects an item by longname (mxd - changed from name to longname)
        public void SelectItem(long longname)
		{
			// Not when selecting is prevented
			if(preventselection) return;

			// Search for item
			ImageBrowserItem target = null; //mxd
			foreach(ImageBrowserItem item in items)
			{
				if(item.Icon.LongName == longname) //mxd
				{
					target = item;
					break;
				}
			}
			
			if(target != null)
			{
				// Select the item
				list.SetSelectedItem(target);
			}
		}
		
		// This selectes the first item
		private void SelectFirstItem()
		{
			// Not when selecting is prevented
			if(preventselection) return;

			// Select first texture
			if (list.Items.Count > 0)
			{
				foreach (ImageBrowserItem item in list.Items)
					if (item.ItemType == ImageBrowserItemType.IMAGE)
					{
						list.SetSelectedItem(item);
						break;
					}
			}
		}
		
		// This begins adding items
		public void BeginAdding(bool keepselectedindex)
		{
			if(keepselectedindex && (list.SelectedItems.Count > 0))
				keepselected = list.Items.IndexOf(list.SelectedItems[0]);
			else
				keepselected = -1;
			
			// Clean list
			items.Clear();
			
			// Stop updating
			refreshtimer.Enabled = false;
		}

		// This ends adding items
		public void EndAdding()
		{
			// Fill list with items
			RefillList(true);

			// Start updating
			refreshtimer.Enabled = true;
		}

		//mxd. This adds a category item
		public void AddFolder(ImageBrowserItemType itemtype, string categoryname)
		{
			switch(itemtype)
			{
				case ImageBrowserItemType.FOLDER: case ImageBrowserItemType.FOLDER_UP:
					items.Add(new ImageBrowserCategoryItem(itemtype, categoryname));
					break;

				default: throw new Exception("Unsupported ImageBrowserItemType");
			}
		}
		
		// This adds an item
        // [ZZ] having nice string.Empty does not justify having two functions doing the same thing, with one parameter difference.
        //      C# not Java.
		public void AddItem(ImageData image, string tooltip = "")
		{
            ImageBrowserItem newItem = new ImageBrowserItem(image, tooltip, uselongtexturenames);
			items.Add(newItem);
		}

		// biwa. Removes all duplicates. That was done each time in AddItem before. Much faster
		// to do it in one go. Not sure when there are actually duplicates
		// Depending on whether a floor or a wall is selected the corrosponding images come first
		// in the list, so alawys keep the first occurence
		public void MakeTexturesUnique()
		{
			items = items.GroupBy(item => item.TextureName).Select(item => item.First()).ToList();
		}

		// This fills the list based on the objectname filter
		private void RefillList(bool selectfirst)
		{
			ImageBrowserItemGroup directoryitems = new ImageBrowserItemGroup("Directories");
			ImageBrowserItemGroup useditems = new ImageBrowserItemGroup("Used textures");
			ImageBrowserItemGroup visibleitems = new ImageBrowserItemGroup("All textures");

			// Clear list first
			list.Clear();
			list.Title = (usedtexturesfirst.Checked ? usedfirstgroup : availgroup);

			//mxd. Anything to do?
			if(items.Count == 0) return;

			//mxd. Filtering by texture size?
			int w = filterWidth.GetResult(-1);
			int h = filterHeight.GetResult(-1);
			
			// Go for all items
			ImageBrowserItem previtem = null; //mxd
			for(int i = items.Count - 1; i > -1; i--)
			{
				// Add item if valid
				items[i].ShowFullName = uselongtexturenames; //mxd
				switch(items[i].ItemType)
				{
					case ImageBrowserItemType.IMAGE:
						if(ValidateItem(items[i], previtem) && ValidateItemSize(items[i], w, h))
						{
							visibleitems.Items.Add(items[i]);

							if (items[i].Icon.UsedInMap)
								useditems.Items.Add(items[i]);

							previtem = items[i];
						}
						break;

					case ImageBrowserItemType.FOLDER_UP: //mxd. "Browse Up" items are always valid
						directoryitems.Items.Add(items[i]);
						break;

					case ImageBrowserItemType.FOLDER: //mxd. Only apply name filtering to "Folder" items
						if(items[i].TextureName.ToUpperInvariant().Contains(objectname.Text.ToUpperInvariant()))
							directoryitems.Items.Add(items[i]);
						break;

					default: throw new NotImplementedException("Unknown ImageBrowserItemType");
				}
			}

			// Fill list
			directoryitems.Items.Sort();
			useditems.Items.Sort();
			visibleitems.Items.Sort();

			List<ImageBrowserItemGroup> itemgroups = new List<ImageBrowserItemGroup>();

			// Always add the directories
			itemgroups.Add(directoryitems);

			// Add used textures if the user wants to and there are any
			if (usedtexturesfirst.Checked && useditems.Items.Count > 0)
				itemgroups.Add(useditems);

			// Finally add all items
			if (visibleitems.Items.Count > 0)
				itemgroups.Add(visibleitems);

			list.SetItems(itemgroups);

			// Make selection?
			if (!preventselection && list.Items.Count > 0)
			{
				// Select specific item?
				if(keepselected > -1)
				{
					// If the last item doesn't exist anymore select the new last item
					if(keepselected >= list.Items.Count)
						list.SetSelectedItem(list.Items[list.Items.Count-1]);
					else
						list.SetSelectedItem(list.Items[keepselected]);
				}
				// Select first item?
				else if(selectfirst)
				{
					SelectFirstItem();
				}
				//mxd. Try reselecting the same/next closest item
				else if(!string.IsNullOrEmpty(objectname.Text))
				{
					ImageBrowserItem bestmatch = null;
					int charsmatched = 1;
					foreach(ImageBrowserItem item in list.Items)
					{
						if(item.ItemType == ImageBrowserItemType.IMAGE && item.Icon.Name[0] == objectname.Text[0])
						{
							if (bestmatch == null)
								bestmatch = item;

							if(item.Icon.Name == objectname.Text)
							{
								bestmatch = item;
								break;
							}

							for(int i = 1; i < Math.Min(item.Icon.Name.Length, objectname.Text.Length); i++)
							{
								if(item.Icon.Name[i] == objectname.Text[i])
								{
									if(i > charsmatched)
									{
										bestmatch = item;
										charsmatched = i;
									}
									break;
								}
							}
						}
					}
					
					// Select found item
					if (bestmatch != null)
					{
						list.SetSelectedItem(bestmatch);
					}
					else
					{
						SelectFirstItem();
					}
				}
			}
			
			// Raise event
			if((SelectedItemChanged != null) && !preventselection)
				SelectedItemChanged(list.SelectedItems.Count > 0 ? list.SelectedItems[0] : null);
		}

		// This validates an item
		private bool ValidateItem(ImageBrowserItem item, ImageBrowserItem previtem)
		{
            //mxd. mixMode: 0 = All, 1 = Textures, 2 = Flats, 3 = Based on BrowseFlats
            //if (!splitter.Panel2Collapsed) 
			{
                if (texturetype == 0 && previtem != null && item.TextureName == previtem.TextureName) return false;
				if (texturetype == 1 && item.Icon.TextureNamespace == TextureNamespace.FLAT) return false;
				if (texturetype == 2 && !(item.Icon.TextureNamespace == TextureNamespace.FLAT)) return false;
				if (texturetype == 3 && (browseflats != (item.Icon.TextureNamespace == TextureNamespace.FLAT))) return false;
			}
            //else if (previtem != null && item.TextureName == previtem.TextureName) return false;

            return item.TextureName.ToUpperInvariant().Contains(objectname.Text.ToUpperInvariant());
		}

		//mxd. This validates an item's texture size
		private static bool ValidateItemSize(ImageBrowserItem i, int w, int h) 
		{
			if(!i.Icon.IsPreviewLoaded) return true;
			if(w > 0 && i.Icon.Width != w) return false;
			if(h > 0 && i.Icon.Height != h) return false;
			return true;
		}

		//mxd
		private int SortItems(ImageBrowserItem item1, ImageBrowserItem item2)
		{
			if(usedtexturesfirst.Checked 
				&& item1.ItemType == ImageBrowserItemType.IMAGE 
				&& item2.ItemType == ImageBrowserItemType.IMAGE 
				&& item1.Icon.UsedInMap != item2.Icon.UsedInMap)
			{
				// Push used items to the top
				return (item1.Icon.UsedInMap ? -1 : 1);
			}

			return item1.CompareTo(item2);
		}
		
		//mxd. This sends the focus to the textures list
		public void FocusList()
		{
			list.Focus();
		}

		private void centeritem_CheckedChanged(object sender, EventArgs e)
		{
			list.CenterItem = centeritem.Checked;
		}

		#endregion
	}
}
