
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
using System.Reflection;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.Editing;
using CodeImp.DoomBuilder.GZBuilder.Data;

#endregion

namespace CodeImp.DoomBuilder.Windows
{
	internal partial class ConfigForm : DelayedForm
	{
		// Variables
		private GameConfiguration gameconfig;
		private ConfigurationInfo configinfo;
		private List<DefinedTextureSet> copiedsets;
		private bool preventchanges;
		private bool reloadresources;

		//mxd. "Copy/Paste" stuff
		private ConfigurationInfo configinfocopy;

		// Properties
		public bool ReloadResources { get { return reloadresources; } }

		// Constructor
		public ConfigForm()
		{
			ListViewItem lvi;
			
			// Initialize
			InitializeComponent();
			CodeImp.DoomBuilder.General.ApplyMonoListViewFix(listtextures);

			#if NO_WIN32
			// Linux doesn't require .exe or .bat file extensions
			testprogramdialog.Filter = "All files|*";
			#endif

			// Make list column header full width
			columnname.Width = listconfigs.ClientRectangle.Width - SystemInformation.VerticalScrollBarWidth - 2;
			
			// Fill list of configurations
			foreach(ConfigurationInfo ci in General.Configs)
			{
				// Add a copy
				lvi = listconfigs.Items.Add(ci.Name);
				lvi.Tag = ci.Clone();
				lvi.Checked = ci.Enabled; //mxd
				lvi.ForeColor = (ci.Enabled ? SystemColors.WindowText : SystemColors.InactiveCaptionText); //mxd

				// This is the current configuration?
				if((General.Map != null) && (General.Map.ConfigSettings.Filename == ci.Filename))
					lvi.Selected = true;
			}
			
			// No skill
			skill.Value = 0;
			
			// Nodebuilders are allowed to be empty
			nodebuildersave.Items.Add(new NodebuilderInfo());
			nodebuildertest.Items.Add(new NodebuilderInfo());
			
			// Fill comboboxes with nodebuilders
			nodebuildersave.Items.AddRange(General.Nodebuilders.ToArray());
			nodebuildertest.Items.AddRange(General.Nodebuilders.ToArray());
			
			// Fill list of editing modes
			foreach(EditModeInfo emi in General.Editing.ModesInfo)
			{
				// Is this mode selectable by the user?
				if(emi.IsOptional)
				{
					lvi = listmodes.Items.Add(emi.Attributes.DisplayName + (emi.Attributes.IsDeprecated ? " (deprecated)" : ""));
					lvi.Tag = emi;
					lvi.SubItems.Add(emi.Plugin.Plug.Name);
					lvi.UseItemStyleForSubItems = true; //mxd
				}
			}

			//mxd
			listconfigs.ItemChecked += listconfigs_ItemChecked;
			listconfigs.SelectedIndexChanged += listconfigs_SelectedIndexChanged;

			//mxd. Trigger change to update the right panel...
			listconfigs_MouseUp(this, new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0));
		}
		
		// This shows a specific page
		public void ShowTab(int index)
		{
			tabs.SelectedIndex = index;
		}

		// Configuration item selected
		private void listconfigs_SelectedIndexChanged(object sender, EventArgs e)
		{
			// Item selected?
			if(listconfigs.SelectedItems.Count > 0)
			{
				// Enable panels
				tabs.Enabled = true;

				preventchanges = true;

				//mxd. Store current engine name
				if(configinfo != null && !String.IsNullOrEmpty(cbEngineSelector.Text))
					configinfo.TestProgramName = cbEngineSelector.Text;
				
				// Get config info of selected item
				configinfo = listconfigs.SelectedItems[0].Tag as ConfigurationInfo;
				
				//mxd. Load the game configuration
				gameconfig = new GameConfiguration(configinfo.Configuration);

				// Set defaults
				configinfo.ApplyDefaults(gameconfig);
				
				// Fill resources list
				configdata.EditResourceLocationList(configinfo.Resources);
				
				// Go for all nodebuilder save items
				nodebuildersave.SelectedIndex = -1;
				for(int i = 0; i < nodebuildersave.Items.Count; i++)
				{
					// Get item
					NodebuilderInfo ni = nodebuildersave.Items[i] as NodebuilderInfo;
					
					// Item matches configuration setting?
					if(String.CompareOrdinal(ni.Name, configinfo.NodebuilderSave) == 0)
					{
						// Select this item
						nodebuildersave.SelectedIndex = i;
						break;
					}
				}
				
				// Go for all nodebuilder test items
				nodebuildertest.SelectedIndex = -1;
				for(int i = 0; i < nodebuildertest.Items.Count; i++)
				{
					// Get item
					NodebuilderInfo ni = nodebuildertest.Items[i] as NodebuilderInfo;
					
					// Item matches configuration setting?
					if(String.CompareOrdinal(ni.Name, configinfo.NodebuilderTest) == 0)
					{
						// Select this item
						nodebuildertest.SelectedIndex = i;
						break;
					}
				}
				
				// Fill skills list
				skill.ClearInfo();
				skill.AddInfo(gameconfig.Skills.ToArray());

				//mxd. Fill engines list
				cbEngineSelector.Items.Clear();
				foreach(EngineInfo info in configinfo.TestEngines)
					cbEngineSelector.Items.Add(info.TestProgramName);

				cbEngineSelector.SelectedIndex = configinfo.CurrentEngineIndex;
				btnRemoveEngine.Enabled = configinfo.TestEngines.Count > 1;
				
				// Fill texture sets list
				listtextures.Items.Clear();
				foreach(DefinedTextureSet ts in configinfo.TextureSets)
				{
					ListViewItem item = listtextures.Items.Add(ts.Name);
					item.Tag = ts;
					item.ImageIndex = 0;
				}
				listtextures.Sort();
				
				// Go for all the editing modes in the list
				foreach(ListViewItem lvi in listmodes.Items)
				{
					EditModeInfo emi = (lvi.Tag as EditModeInfo);

					//mxd. Disable item if the mode does not support current map format
					if (emi.Attributes.SupportedMapFormats != null &&
						Array.IndexOf(emi.Attributes.SupportedMapFormats, gameconfig.FormatInterface) == -1)
					{
						lvi.Text = emi.Attributes.DisplayName + " (map format not supported" + (emi.Attributes.IsDeprecated ? ", deprecated" : "") + ")";
						lvi.ForeColor = SystemColors.GrayText;
						lvi.BackColor = SystemColors.InactiveBorder;
						lvi.Checked = false;

						continue;
					}
					else if (emi.Attributes.RequiredMapFeatures != null && !gameconfig.SupportsMapFeatures(emi.Attributes.RequiredMapFeatures))
					{
						lvi.Text = emi.Attributes.DisplayName + " (map feature not supported)";
						lvi.ForeColor = SystemColors.GrayText;
						lvi.BackColor = SystemColors.InactiveBorder;
						lvi.Checked = false;
					}
					else
					{
						lvi.Text = emi.Attributes.DisplayName + (emi.Attributes.IsDeprecated ? " (deprecated)" : "");
						lvi.ForeColor = SystemColors.WindowText;
						lvi.BackColor = SystemColors.Window;
						lvi.Checked = (configinfo.EditModes.ContainsKey(emi.Type.FullName) && configinfo.EditModes[emi.Type.FullName]);
					}
				}

				// Update listmodes columns width (mxd)
				listmodes.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
				
				// Fill start modes
				RefillStartModes();

				// Done
				preventchanges = false;
			}
		}
		
		// Key released
		private void listconfigs_KeyUp(object sender, KeyEventArgs e)
		{
			// Nothing selected?
			if(listconfigs.SelectedItems.Count == 0)
			{
				// Disable panels
				gameconfig = null;
				configinfo = null;
				configdata.FixedResourceLocationList(new DataLocationList());
				configdata.EditResourceLocationList(new DataLocationList());
				nodebuildersave.SelectedIndex = -1;
				nodebuildertest.SelectedIndex = -1;
				testapplication.Text = "";
				testparameters.Text = "";
				shortpaths.Checked = false;
				linuxpaths.Checked = false;
				skill.Value = 0;
				skill.ClearInfo();
				customparameters.Checked = false;
				tabs.Enabled = false;
				listtextures.Items.Clear();
			}
		}
		
		// Mouse released
		private void listconfigs_MouseUp(object sender, MouseEventArgs e)
		{
			listconfigs_KeyUp(sender, new KeyEventArgs(Keys.None));
		}

		//mxd
		private void listconfigs_ItemChecked(object sender, ItemCheckedEventArgs e) 
		{
			e.Item.ForeColor = (e.Item.Checked ? SystemColors.WindowText : SystemColors.InactiveCaptionText);
		}
		
		// Resource locations changed
		private void resourcelocations_OnContentChanged()
		{
			// Leave when no configuration selected
			if(configinfo == null) return;
			
			// Apply to selected configuration
			configinfo.Resources.Clear();
			configinfo.Resources.AddRange(configdata.GetResources());
			if(!preventchanges) reloadresources = true;
		}

		// Nodebuilder selection changed
		private void nodebuildersave_SelectedIndexChanged(object sender, EventArgs e)
		{
			// Leave during setup or when no configuration selected
			if(preventchanges || configinfo == null || nodebuildersave.SelectedItem == null) return;
			
			// Apply to selected configuration
			configinfo.NodebuilderSave = (nodebuildersave.SelectedItem as NodebuilderInfo).Name;
			configinfo.Changed = true; //mxd
		}

		// Nodebuilder selection changed
		private void nodebuildertest_SelectedIndexChanged(object sender, EventArgs e)
		{
			// Leave during setup or when no configuration selected
			if(preventchanges || configinfo == null || nodebuildertest.SelectedItem == null) return;

			// Apply to selected configuration
			configinfo.NodebuilderTest = (nodebuildertest.SelectedItem as NodebuilderInfo).Name;
			configinfo.Changed = true; //mxd
		}
		
		// Test application changed
		private void testapplication_TextChanged(object sender, EventArgs e)
		{
			// Leave during setup or when no configuration is selected
			if(preventchanges || configinfo == null) return;

			// Apply to selected configuration
			configinfo.TestProgram = testapplication.Text;
			
			//mxd. User entered engine name before picking the engine?
			if(cbEngineSelector.SelectedIndex == -1 || string.IsNullOrEmpty(configinfo.TestProgram))
			{
				ApplyTestEngineNameChange();
			}
			// Update engine name
			else
			{
				// Use engine directory name?
				string enginename = Path.GetDirectoryName(configinfo.TestProgram);
				if(!string.IsNullOrEmpty(enginename))
				{
					int pos = enginename.LastIndexOf(Path.DirectorySeparatorChar);
					if(pos != -1) enginename = enginename.Substring(pos + 1);
				}
				// Use engine filename
				else
				{
					enginename = Path.GetFileNameWithoutExtension(configinfo.TestProgram);
				}
				
				configinfo.TestProgramName = enginename;
				cbEngineSelector.Items[cbEngineSelector.SelectedIndex] = enginename;
			}

			configinfo.Changed = true; //mxd
		}

		// Test parameters changed
		private void testparameters_TextChanged(object sender, EventArgs e)
		{
			// Leave when no configuration selected
			if(preventchanges || configinfo == null) return;

			// Apply to selected configuration
			configinfo = listconfigs.SelectedItems[0].Tag as ConfigurationInfo;
			configinfo.TestParameters = testparameters.Text;
			configinfo.Changed = true; //mxd

			// Show example result
			CreateParametersExample();
		}
		
		// This creates a new parameters example
		private void CreateParametersExample()
		{
			// Map loaded?
			if(General.Map != null)
			{
				// Make converted parameters
				testresult.Text = General.Map.Launcher.ConvertParameters(testparameters.Text, skill.Value, shortpaths.Checked, linuxpaths.Checked);
			}
		}

		//mxd
		private void ApplyTestEngineNameChange() 
		{
			int index = (int)cbEngineSelector.Tag;
			if(index != -1 && cbEngineSelector.Text != cbEngineSelector.Items[index].ToString()) 
			{
				cbEngineSelector.Items[index] = cbEngineSelector.Text;
				configinfo.TestProgramName = cbEngineSelector.Text;
				configinfo.Changed = true; //mxd
			}
		}
		
		// OK clicked
		private void apply_Click(object sender, EventArgs e)
		{
			ConfigurationInfo ci;

			//mxd. Check resources
			for(int i = 0; i < listconfigs.Items.Count; i++)
			{
				// Get configuration item
				ci = listconfigs.Items[i].Tag as ConfigurationInfo;
				if(listconfigs.Items[i].Checked && !ci.Resources.IsValid())
				{
					MessageBox.Show(this, "At least one resource doesn't exist in \"" + ci.Name + "\" game configuration!", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
					tabs.SelectedTab = tabresources;
					listconfigs.Focus();
					listconfigs.Items[i].Selected = true;
					return;
				}
			}

			//mxd. Apply changes of current test engine name, if there are any
			//TODO: move engine selector stuff into separate component!
			if(configinfo != null) ApplyTestEngineNameChange();

			//mxd. Apply configuration items. They should be in the same order, riiiight?
			for(int i = 0; i < listconfigs.Items.Count; i++) 
			{
				// Get configuration item
				ci = listconfigs.Items[i].Tag as ConfigurationInfo;
				ci.Enabled = listconfigs.Items[i].Checked;

				// Apply settings
				General.Configs[i].Enabled = ci.Enabled;
				if(ci.Changed) General.Configs[i].Apply(ci);
			}

			General.SaveGameSettings();
			
			// Close
			this.DialogResult = DialogResult.OK;
			this.Close();
		}

		// Cancel clicked
		private void cancel_Click(object sender, EventArgs e)
		{
			// Close
			this.DialogResult = DialogResult.Cancel;
			this.Close();
		}

		// Browse test program
		private void browsetestprogram_Click(object sender, EventArgs e)
		{
			// Set initial directory
			if(testapplication.Text.Length > 0)
			{
				try { testprogramdialog.InitialDirectory = Path.GetDirectoryName(testapplication.Text); }
				catch(Exception) { }
			}
			
			// Browse for test program
			if(testprogramdialog.ShowDialog() == DialogResult.OK)
			{
				// Apply
				testapplication.Text = testprogramdialog.FileName;
			}
		}

		// Customize parameters (un)checked
		private void customparameters_CheckedChanged(object sender, EventArgs e)
		{
			// Leave when no configuration selected
			if(configinfo == null) return;

			// Apply to selected configuration
			configinfo.CustomParameters = customparameters.Checked;
			configinfo.Changed = true; //mxd

			// Update interface
			labelparameters.Visible = customparameters.Checked;
			testparameters.Visible = customparameters.Checked;
			shortpaths.Visible = customparameters.Checked;
			linuxpaths.Visible = customparameters.Checked;

			// Check if a map is loaded
			if(General.Map != null)
			{
				// Show parameters example result
				labelresult.Visible = customparameters.Checked;
				testresult.Visible = customparameters.Checked;
				noresultlabel.Visible = false;
			}
			else
			{
				// Cannot show parameters example result
				labelresult.Visible = false;
				testresult.Visible = false;
				noresultlabel.Visible = customparameters.Checked;
			}
		}
		
		// Use short paths changed
		private void shortpaths_CheckedChanged(object sender, EventArgs e)
		{
			// Leave when no configuration selected
			if(configinfo == null) return;
			if (linuxpaths.Checked && shortpaths.Checked)
			{
				linuxpaths.Checked = false;
			}
			// Apply to selected configuration
			configinfo.TestShortPaths = shortpaths.Checked;
			configinfo.Changed = true; //mxd
			
			CreateParametersExample();
		}
        private void Linuxpaths_CheckedChanged(object sender, EventArgs e)
        {
            // Leave when no configuration selected
            if (configinfo == null) return;
			if (linuxpaths.Checked && shortpaths.Checked)
			{
				shortpaths.Checked = false;
			}
            // Apply to selected configuration
            configinfo.TestLinuxPaths = linuxpaths.Checked;
            configinfo.Changed = true; //mxd

            CreateParametersExample();

        }
		// Skill changes
		private void skill_ValueChanges(object sender, EventArgs e)
		{
			// Leave when no configuration selected
			if(configinfo == null) return;
			
			// Apply to selected configuration
			configinfo.TestSkill = skill.Value;
			configinfo.Changed = true; //mxd
			
			CreateParametersExample();
		}

		// Make new texture set
		private void addtextureset_Click(object sender, EventArgs e)
		{
			DefinedTextureSet s = new DefinedTextureSet("New Texture Set");
			TextureSetForm form = new TextureSetForm();
			form.Setup(s);
			if(form.ShowDialog(this) == DialogResult.OK)
			{
				// Add to texture sets
				configinfo.TextureSets.Add(s);
				configinfo.Changed = true; //mxd
				ListViewItem item = listtextures.Items.Add(s.Name);
				item.Tag = s;
				item.ImageIndex = 0;
				listtextures.Sort();
				reloadresources = true;
			}
		}

		// Edit texture set
		private void edittextureset_Click(object sender, EventArgs e)
		{
			// Texture Set selected?
			if(listtextures.SelectedItems.Count > 0)
			{
				DefinedTextureSet s = (listtextures.SelectedItems[0].Tag as DefinedTextureSet);
				TextureSetForm form = new TextureSetForm();
				form.Setup(s);
				form.ShowDialog(this);
				listtextures.SelectedItems[0].Text = s.Name;
				listtextures.Sort();
				reloadresources = true;
			}
		}
		
		// Remove texture set
		private void removetextureset_Click(object sender, EventArgs e)
		{
			// Texture Set selected?
			while(listtextures.SelectedItems.Count > 0)
			{
				// Remove from config info and list
				DefinedTextureSet s = (listtextures.SelectedItems[0].Tag as DefinedTextureSet);
				configinfo.TextureSets.Remove(s);
				configinfo.Changed = true; //mxd
				listtextures.SelectedItems[0].Remove();
				reloadresources = true;
			}
		}
		
		// Texture Set selected/deselected
		private void listtextures_SelectedIndexChanged(object sender, EventArgs e)
		{
			edittextureset.Enabled = (listtextures.SelectedItems.Count > 0);
			removetextureset.Enabled = (listtextures.SelectedItems.Count > 0);
			copytexturesets.Enabled = (listtextures.SelectedItems.Count > 0);
		}
		
		// Doubleclicking a texture set
		private void listtextures_DoubleClick(object sender, EventArgs e)
		{
			edittextureset_Click(sender, e);
		}
		
		// Copy selected texture sets
		private void copytexturesets_Click(object sender, EventArgs e)
		{
			// Make copies
			copiedsets = new List<DefinedTextureSet>();
			foreach(ListViewItem item in listtextures.SelectedItems)
			{
				DefinedTextureSet s = (item.Tag as DefinedTextureSet);
				copiedsets.Add(s.Copy());
			}
			
			// Enable button
			pastetexturesets.Enabled = true;
		}
		
		// Paste copied texture sets
		private void pastetexturesets_Click(object sender, EventArgs e)
		{
			if(copiedsets != null)
			{
				// Add copies
				foreach(DefinedTextureSet ts in copiedsets)
				{
					DefinedTextureSet s = ts.Copy();
					ListViewItem item = listtextures.Items.Add(s.Name);
					item.Tag = s;
					item.ImageIndex = 0;
					configinfo.TextureSets.Add(s);
				}
				listtextures.Sort();
				reloadresources = true;
				configinfo.Changed = true; //mxd
			}
		}
		
		// This will add the default sets from game configuration
		private void restoretexturesets_Click(object sender, EventArgs e)
		{
			// Ask nicely first
			if(MessageBox.Show(this, "This will add the default Texture Sets from the Game Configuration. Do you want to continue?",
				"Add Default Sets", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				// Add copies
				foreach(DefinedTextureSet ts in gameconfig.TextureSets)
				{
					DefinedTextureSet s = ts.Copy();
					ListViewItem item = listtextures.Items.Add(s.Name);
					item.Tag = s;
					item.ImageIndex = 0;
					configinfo.TextureSets.Add(s);
				}
				listtextures.Sort();
				reloadresources = true;
				configinfo.Changed = true; //mxd
			}
		}
		
		// This is called when an editing mode item is checked or unchecked
		private void listmodes_ItemChecked(object sender, ItemCheckedEventArgs e)
		{
			if(preventchanges) return; //mxd

			// Leave when no configuration selected
			if(configinfo == null) return;

			// mxd. Not the best way to detect a disabled item, but we will go with that...
			if(e.Item.ForeColor == SystemColors.GrayText) e.Item.Checked = false;
			
			// Apply changes
			EditModeInfo emi = (e.Item.Tag as EditModeInfo);
			bool currentstate = (configinfo.EditModes.ContainsKey(emi.Type.FullName) && configinfo.EditModes[emi.Type.FullName]);
			if(e.Item.Checked && !currentstate)
			{
				// Add
				configinfo.EditModes[emi.Type.FullName] = true;
				configinfo.Changed = true; //mxd
			}
			else if(!e.Item.Checked && currentstate)
			{
				// Remove
				configinfo.EditModes[emi.Type.FullName] = false;
				configinfo.Changed = true; //mxd
			}
			
			preventchanges = true;
			RefillStartModes();
			preventchanges = false;
		}

		// Help requested
		private void ConfigForm_HelpRequested(object sender, HelpEventArgs hlpevent)
		{
			General.ShowHelp("w_gameconfigurations.html");
			hlpevent.Handled = true;
		}
		
		// This refills the start mode cobobox
		private void RefillStartModes()
		{
			// Refill the startmode combobox
			startmode.Items.Clear();
			foreach(ListViewItem item in listmodes.Items)
			{
				if(item.Checked)
				{
					EditModeInfo emi = (item.Tag as EditModeInfo);
					if(emi.Attributes.SafeStartMode)
					{
						int newindex = startmode.Items.Add(emi);
						if(emi.Type.Name == configinfo.StartMode) startmode.SelectedIndex = newindex;
					}
				}
			}
			
			// Select the first in the combobox if none are selected
			if((startmode.SelectedItem == null) && (startmode.Items.Count > 0))
			{
				startmode.SelectedIndex = 0;
				EditModeInfo emi = (startmode.SelectedItem as EditModeInfo);
				configinfo.StartMode = emi.Type.Name;
				configinfo.Changed = true; //mxd
			}
		}
		
		// Start mode combobox changed
		private void startmode_SelectedIndexChanged(object sender, EventArgs e)
		{
			if(preventchanges || (configinfo == null)) return;
			
			// Apply start mode
			if(startmode.SelectedItem != null)
			{
				EditModeInfo emi = (startmode.SelectedItem as EditModeInfo);
				configinfo.StartMode = emi.Type.Name;
				configinfo.Changed = true; //mxd
			}
		}

		//mxd
		private void btnNewEngine_Click(object sender, EventArgs e) 
		{
			// Set initial directory?
			if(testapplication.Text.Length > 0)
			{
				try { testprogramdialog.InitialDirectory = Path.GetDirectoryName(testapplication.Text); }
				catch(Exception) { }
			}

			// Browse for test program
			if(testprogramdialog.ShowDialog() == DialogResult.OK)
			{
				preventchanges = true;

				// Remove EngineInfos without program path
				configinfo.TestEngines.RemoveAll(info => string.IsNullOrEmpty(info.TestProgram));

				// Add new EngineInfo
				EngineInfo newInfo = new EngineInfo();
				newInfo.TestSkill = (int)Math.Ceiling(gameconfig.Skills.Count / 2f); // Set Medium skill level
				configinfo.TestEngines.Add(newInfo);
				configinfo.Changed = true;

				// Refresh engines list
				cbEngineSelector.Items.Clear();
				foreach(EngineInfo info in configinfo.TestEngines)
					cbEngineSelector.Items.Add(info.TestProgramName);

				cbEngineSelector.SelectedIndex = configinfo.TestEngines.Count - 1;
				btnRemoveEngine.Enabled = (configinfo.TestEngines.Count > 1);

				preventchanges = false;

				// Set engine path (will also update current engine name)
				testapplication.Text = testprogramdialog.FileName;
			}
		}

		//mxd
		private void btnRemoveEngine_Click(object sender, EventArgs e) 
		{
			preventchanges = true;
			
			//remove params
			int index = cbEngineSelector.SelectedIndex;
			cbEngineSelector.SelectedIndex = -1;
			configinfo.TestEngines.RemoveAt(index);
			configinfo.Changed = true; //mxd
			
			//refresh engines list
			cbEngineSelector.Items.Clear();
			foreach(EngineInfo info in configinfo.TestEngines)
				cbEngineSelector.Items.Add(info.TestProgramName);

			if(index >= configinfo.TestEngines.Count)
				index = configinfo.TestEngines.Count - 1;

			cbEngineSelector.SelectedIndex = index;

			if(configinfo.TestEngines.Count < 2)
				btnRemoveEngine.Enabled = false;

			preventchanges = false;
		}

		//mxd
		private void cbEngineSelector_SelectedIndexChanged(object sender, EventArgs e) 
		{
			if(cbEngineSelector.SelectedIndex == -1) return;

			preventchanges = true;
			
			//set new values
			configinfo.CurrentEngineIndex = cbEngineSelector.SelectedIndex;
			configinfo.Changed = true; //mxd
			cbEngineSelector.Tag = cbEngineSelector.SelectedIndex; //store for later use

			// Set test application and parameters
			if(!configinfo.CustomParameters)
			{
				configinfo.TestParameters = gameconfig.TestParameters;
				configinfo.TestShortPaths = gameconfig.TestShortPaths;
				configinfo.TestLinuxPaths = gameconfig.TestLinuxPaths;
			}

			configinfo.TestProgramName = cbEngineSelector.Text;
			testapplication.Text = configinfo.TestProgram;
			testparameters.Text = configinfo.TestParameters;
			shortpaths.Checked = configinfo.TestShortPaths;
			linuxpaths.Checked = configinfo.TestLinuxPaths;

			int skilllevel = configinfo.TestSkill;
			skill.Value = skilllevel - 1; //mxd. WHY???
			skill.Value = skilllevel;
			customparameters.Checked = configinfo.CustomParameters;

			preventchanges = false;
		}

		//mxd
		private void cbEngineSelector_DropDown(object sender, EventArgs e) 
		{
			ApplyTestEngineNameChange();
		}

		//mxd
		private void ConfigForm_Shown(object sender, EventArgs e) 
		{
			if(listconfigs.SelectedItems.Count > 0) listconfigs.SelectedItems[0].EnsureVisible();
		}

#region ============= Copy/Paste context menu (mxd)

		private void copypastemenu_Opening(object sender, System.ComponentModel.CancelEventArgs e) 
		{
			if(listconfigs.SelectedIndices.Count < 1) 
			{
				e.Cancel = true;
				return;
			}

			ConfigurationInfo current = listconfigs.SelectedItems[0].Tag as ConfigurationInfo;
			bool havecopiedconfig = configinfocopy != null;
			bool formatinterfacesmatch = havecopiedconfig && current.FormatInterface == configinfocopy.FormatInterface;

			pasteall.Enabled = formatinterfacesmatch;
			pasteengines.Enabled = (havecopiedconfig && configinfocopy.TestEngines.Count > 0);
			pasteresources.Enabled = (havecopiedconfig && configinfocopy.Resources.Count > 0);
			pastecolorpresets.Enabled = (formatinterfacesmatch && configinfocopy.LinedefColorPresets.Length > 0);
		}

		private void copyall_Click(object sender, EventArgs e) 
		{
			if(listconfigs.SelectedIndices.Count < 1) return;
			ConfigurationInfo current = listconfigs.SelectedItems[0].Tag as ConfigurationInfo;
			configinfocopy = current.Clone();

			//display info
			General.Interface.DisplayStatus(StatusType.Info, "Copied \"" + configinfocopy.Name + "\" game configuration");
		}

		private void pasteall_Click(object sender, EventArgs e) 
		{
			if(listconfigs.SelectedIndices.Count < 1) return;

			// Get current configinfo
			ConfigurationInfo current = listconfigs.SelectedItems[0].Tag as ConfigurationInfo;
			current.PasteFrom(configinfocopy);

			// Update display
			cbEngineSelector.Text = string.Empty; // Otherwise current text from cbEngineSelector will override the pasted one
			listconfigs_SelectedIndexChanged(listconfigs, EventArgs.Empty);
			
			// Resources need reloading?
			if(General.Map != null && General.Map.ConfigSettings.Name == current.Name)
				reloadresources = true;
			
			General.Interface.DisplayStatus(StatusType.Info, "Pasted game configuration from \"" + configinfocopy.Name + "\"");
		}

		private void pasteresources_Click(object sender, EventArgs e) 
		{
			if(listconfigs.SelectedIndices.Count < 1) return;

			// Get current configinfo
			ConfigurationInfo current = listconfigs.SelectedItems[0].Tag as ConfigurationInfo;
			current.PasteResourcesFrom(configinfocopy);

			// Update display
			listconfigs_SelectedIndexChanged(listconfigs, EventArgs.Empty);

			// Resources need reloading?
			if(General.Map != null && General.Map.ConfigSettings.Name == current.Name)
				reloadresources = true;
			
			General.Interface.DisplayStatus(StatusType.Info, "Pasted resources from \"" + configinfocopy.Name + "\"");
		}

		private void pasteengines_Click(object sender, EventArgs e) 
		{
			if(listconfigs.SelectedIndices.Count < 1) return;

			// Get current configinfo
			ConfigurationInfo current = listconfigs.SelectedItems[0].Tag as ConfigurationInfo;
			current.PasteTestEnginesFrom(configinfocopy);

			// Update display
			cbEngineSelector.Text = string.Empty; // Otherwise current text from cbEngineSelector will override the pasted one
			listconfigs_SelectedIndexChanged(listconfigs, EventArgs.Empty);
			General.Interface.DisplayStatus(StatusType.Info, "Pasted engines list from \"" + configinfocopy.Name + "\"");
		}

		private void pastecolorpresets_Click(object sender, EventArgs e) 
		{
			if(listconfigs.SelectedIndices.Count < 1) return;

			// Get current configinfo
			ConfigurationInfo current = listconfigs.SelectedItems[0].Tag as ConfigurationInfo;
			current.PasteColorPresetsFrom(configinfocopy);

			// Update display
			listconfigs_SelectedIndexChanged(listconfigs, EventArgs.Empty);
			General.Interface.DisplayStatus(StatusType.Info, "Pasted color presets from \"" + configinfocopy.Name + "\"");
		}

#endregion
	}
}
