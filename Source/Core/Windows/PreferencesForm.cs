
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
using CodeImp.DoomBuilder.Actions;
using System.Globalization;
using CodeImp.DoomBuilder.Data;
using System.IO;
using CodeImp.DoomBuilder.Rendering;
using Action = CodeImp.DoomBuilder.Actions.Action;

#endregion

namespace CodeImp.DoomBuilder.Windows
{
	internal partial class PreferencesForm : DelayedForm
	{
		#region ================== Constants

		private const float VIEW_DISTANCE_STEP_SIZE = 500.0f;

		#endregion

		#region ================== Variables

		private PreferencesController controller;
		private bool allowapplycontrol;
		private bool disregardshift;
		private bool disregardcontrol;
		private readonly List<ListViewItem> allactionitems; //mxd
		private readonly List<int> allactionitemsgroups; //mxd

		private bool reloadresources;
		
		#endregion

		#region ================== Properties

		public bool ReloadResources { get { return reloadresources; } }

		#endregion

		#region ================== Constructor

		// Constructor
		public PreferencesForm()
		{
			// Initialize
			InitializeComponent();
			
			// Interface
			imagebrightness.Value = General.Settings.ImageBrightness;
			doublesidedalpha.Value = General.Clamp((int)((1.0f - General.Settings.DoubleSidedAlpha) * 10.0f), doublesidedalpha.Minimum, doublesidedalpha.Maximum);
			activethingsalpha.Value = General.Clamp((int)((1.0f - General.Settings.ActiveThingsAlpha) * 10.0f), activethingsalpha.Minimum, activethingsalpha.Maximum); //mxd
			inactivethingsalpha.Value = General.Clamp((int)((1.0f - General.Settings.InactiveThingsAlpha) * 10.0f), inactivethingsalpha.Minimum, inactivethingsalpha.Maximum); //mxd
			hiddenthingsalpha.Value = General.Clamp((int)((1.0f - General.Settings.HiddenThingsAlpha) * 10.0f), hiddenthingsalpha.Minimum, hiddenthingsalpha.Maximum); //mxd
			defaultviewmode.SelectedIndex = General.Settings.DefaultViewMode;
			fieldofview.Value = General.Clamp(General.Settings.VisualFOV / 10, fieldofview.Minimum, fieldofview.Maximum);
			mousespeed.Value = General.Clamp(General.Settings.MouseSpeed / 100, mousespeed.Minimum, mousespeed.Maximum);
			movespeed.Value = General.Clamp(General.Settings.MoveSpeed / 100, movespeed.Minimum, movespeed.Maximum);
			vertexScale3D.Value = General.Clamp((int)(General.Settings.GZVertexScale3D * 10), vertexScale3D.Minimum, vertexScale3D.Maximum); //mxd
			viewdistance.Value = General.Clamp((int)(General.Settings.ViewDistance / VIEW_DISTANCE_STEP_SIZE), viewdistance.Minimum, viewdistance.Maximum);
			invertyaxis.Checked = General.Settings.InvertYAxis;
			autoscrollspeed.Value = General.Clamp(General.Settings.AutoScrollSpeed, autoscrollspeed.Minimum, autoscrollspeed.Maximum);
			zoomfactor.Value = General.Clamp(General.Settings.ZoomFactor, zoomfactor.Minimum, zoomfactor.Maximum);
			animatevisualselection.Checked = General.Settings.AnimateVisualSelection;
			dockersposition.SelectedIndex = General.Settings.DockersPosition;
			collapsedockers.Checked = General.Settings.CollapseDockers;
			toolbar_file.Checked = General.Settings.ToolbarFile;
			toolbar_script.Checked = General.Settings.ToolbarScript;
			toolbar_undo.Checked = General.Settings.ToolbarUndo;
			toolbar_copy.Checked = General.Settings.ToolbarCopy;
			toolbar_prefabs.Checked = General.Settings.ToolbarPrefabs;
			toolbar_filter.Checked = General.Settings.ToolbarFilter;
			toolbar_viewmodes.Checked = General.Settings.ToolbarViewModes;
			toolbar_geometry.Checked = General.Settings.ToolbarGeometry;
			toolbar_testing.Checked = General.Settings.ToolbarTesting;
			showtexturesizes.Checked = General.Settings.ShowTextureSizes;
            texturesizesbelow.Checked = General.Settings.TextureSizesBelow;
			cbShowFPS.Checked = General.Settings.ShowFPS;
			autolaunchontest.Checked = General.Settings.AutoLaunchOnTest;
			cbFlatShadeVertices.Checked = General.Settings.FlatShadeVertices;

			//mxd
			locatetexturegroup.Checked = General.Settings.LocateTextureGroup;
			cbStoreEditTab.Checked = General.Settings.StoreSelectedEditTab;
			checkforupdates.Checked = General.Settings.CheckForUpdates;
			toolbar_gzdoom.Checked = General.Settings.GZToolbarGZDoom;
			cbSynchCameras.Checked = General.Settings.GZSynchCameras;
			tbDynLightCount.Value = General.Clamp(General.Settings.GZMaxDynamicLights / 16, tbDynLightCount.Minimum, tbDynLightCount.Maximum);
			labelDynLightCount.Text = General.Settings.GZMaxDynamicLights.ToString();
			cbStretchView.Checked = General.Settings.GZStretchView;
			cbOldHighlightMode.Checked = General.Settings.GZOldHighlightMode;
			vertexScale.Value = General.Clamp((int)(General.Settings.GZVertexScale2D * 4.0), vertexScale.Minimum, vertexScale.Maximum);
			vertexScaleLabel.Text = vertexScale.Value * 25 + "%" + (vertexScale.Value == 4 ? " (default)" : "");
			cbMarkExtraFloors.Checked = General.Settings.GZMarkExtraFloors;
			recentFiles.Value = General.Settings.MaxRecentFiles;
			screenshotsfolderpath.Text = General.Settings.ScreenshotsPath;
			if(Directory.Exists(General.Settings.ScreenshotsPath))
				browseScreenshotsFolderDialog.SelectedPath = General.Settings.ScreenshotsPath;

			//mxd. Anisotropic filtering
			anisotropicfiltering.Value = Math.Max(RenderDevice.AF_STEPS.IndexOf(General.Settings.FilterAnisotropy), 0); //mxd
			antialiasing.Value = Math.Max(RenderDevice.AA_STEPS.IndexOf(General.Settings.AntiAliasingSamples), 0); //mxd

			//mxd. Script editor
			scriptfontbold.Checked = General.Settings.ScriptFontBold;
			scriptontop.Checked = General.Settings.ScriptOnTop;
			scriptusetabs.Checked = General.Settings.ScriptUseTabs;
			scripttabwidth.Text = General.Settings.ScriptTabWidth.ToString();
			scriptautoindent.Checked = General.Settings.ScriptAutoIndent;
			scriptallmanstyle.Checked = General.Settings.ScriptAllmanStyle; //mxd
			scriptautoclosebrackets.Checked = General.Settings.ScriptAutoCloseBrackets; //mxd
			scriptshowfolding.Checked = General.Settings.ScriptShowFolding; //mxd
			scriptshowlinenumbers.Checked = General.Settings.ScriptShowLineNumbers; //mxd
			scriptautoshowautocompletion.Checked = General.Settings.ScriptAutoShowAutocompletion; //mxd

			// Fill script fonts list
			scriptfontname.BeginUpdate();
			foreach(FontFamily ff in FontFamily.Families) scriptfontname.Items.Add(ff.Name);
			scriptfontname.EndUpdate();
			
			// Select script font name
			for(int i = 0; i < scriptfontname.Items.Count; i++)
			{
				if(string.Compare(scriptfontname.Items[i].ToString(), General.Settings.ScriptFontName, true) == 0)
				{
					scriptfontname.SelectedIndex = i;
					break; //mxd
				}
			}

			// Select script font size
			for(int i = 0; i < scriptfontsize.Items.Count; i++)
			{
				string fs = General.Settings.ScriptFontSize.ToString(CultureInfo.InvariantCulture); //mxd
				if(string.Compare(scriptfontsize.Items[i].ToString(), fs, true) == 0)
				{
					scriptfontsize.SelectedIndex = i;
					break; //mxd
				}
			}

			//mxd. Text labels
			textlabelfontbold.Checked = General.Settings.TextLabelFontBold;

			//mxd. Fill text labels fonts list
			textlabelfontname.BeginUpdate();
			foreach(FontFamily ff in FontFamily.Families) textlabelfontname.Items.Add(ff.Name);
			textlabelfontname.EndUpdate();

			//mxd. Select text labels font name
			for(int i = 0; i < textlabelfontname.Items.Count; i++)
			{
				if(string.Compare(textlabelfontname.Items[i].ToString(), General.Settings.TextLabelFontName, true) == 0)
				{
					textlabelfontname.SelectedIndex = i;
					break;
				}
			}

			//mxd. Select text labels font size
			for(int i = 0; i < textlabelfontsize.Items.Count; i++)
			{
				string fs = General.Settings.TextLabelFontSize.ToString(CultureInfo.InvariantCulture);
				if(string.Compare(textlabelfontsize.Items[i].ToString(), fs, true) == 0)
				{
					textlabelfontsize.SelectedIndex = i;
					break;
				}
			}
			
			// Fill actions list with categories
			foreach(KeyValuePair<string, string> c in General.Actions.Categories)
				listactions.Groups.Add(c.Key, c.Value);
			
			// Fill list of actions
			Action[] actions = General.Actions.GetAllActions();
			allactionitems = new List<ListViewItem>(); //mxd
			allactionitemsgroups = new List<int>(); //mxd

			listactions.BeginUpdate(); //mxd
			foreach(Action a in actions)
			{
				// Create item
				ListViewItem item = listactions.Items.Add(a.Name, a.Title, 0);
				item.SubItems.Add(Action.GetShortcutKeyDesc(a.ShortcutKey));
				item.SubItems[1].Tag = a.ShortcutKey;

				// Put in category, if the category exists
				if(General.Actions.Categories.ContainsKey(a.Category)) 
				{
					item.Group = listactions.Groups[a.Category];
					allactionitemsgroups.Add(listactions.Groups.IndexOf(item.Group)); //mxd
				}
				else //mxd
				{ 
					allactionitemsgroups.Add(-1);
				}

				allactionitems.Add(item); //mxd
			}
			listactions.EndUpdate(); //mxd

			// Set the colors
			// TODO: Make this automated by using the collection
			colorbackcolor.Color = General.Colors.Background;
			colorvertices.Color = General.Colors.Vertices;
			colorlinedefs.Color = General.Colors.Linedefs;
			colorhighlight.Color = General.Colors.Highlight;
			colorselection.Color = General.Colors.Selection;
			colorindication.Color = General.Colors.Indication;
			colorgrid.Color = General.Colors.Grid;
			colorgrid64.Color = General.Colors.Grid64;

			//mxd
			colorMD3.Color = General.Colors.ModelWireframe;
			colorInfo.Color = General.Colors.InfoLine;
			colorguidelines.Color = General.Colors.Guideline;
			color3dFloors.Color = General.Colors.ThreeDFloor;

			// Script editor colors
			colorscriptbackground.Color = General.Colors.ScriptBackground;
			colorlinenumbers.Color = General.Colors.LineNumbers;
			colorplaintext.Color = General.Colors.PlainText;
			colorcomments.Color = General.Colors.Comments;
			colorkeywords.Color = General.Colors.Keywords;
			colorproperties.Color = General.Colors.Properties; //mxd
			colorliterals.Color = General.Colors.Literals;
			colorconstants.Color = General.Colors.Constants;
			colorstrings.Color = General.Colors.Strings; //mxd
			colorincludes.Color = General.Colors.Includes; //mxd
			colorselectionfore.Color = General.Colors.ScriptSelectionForeColor; //mxd
			colorselectionback.Color = General.Colors.ScriptSelectionBackColor; //mxd
			colorindicator.Color = General.Colors.ScriptIndicator; //mxd
			colorbrace.Color = General.Colors.ScriptBraceHighlight; //mxd
			colorbracebad.Color = General.Colors.ScriptBadBraceHighlight; //mxd
			colorwhitespace.Color = General.Colors.ScriptWhitespace; //mxd
			colorfoldfore.Color = General.Colors.ScriptFoldForeColor; //mxd
			colorfoldback.Color = General.Colors.ScriptFoldBackColor; //mxd

			//mxd. Select "Current colors" preset
			scriptcolorpresets.SelectedIndex = 0;

			blackbrowsers.Checked = General.Settings.BlackBrowsers;
			classicbilinear.Checked = General.Settings.ClassicBilinear;
			visualbilinear.Checked = General.Settings.VisualBilinear;
			qualitydisplay.Checked = General.Settings.QualityDisplay;
			
			// Paste options
			pasteoptions.Setup(General.Settings.PasteOptions.Copy());

			// Allow plugins to add tabs
			this.SuspendLayout();
			controller = new PreferencesController(this) { AllowAddTab = true };
			General.Plugins.OnShowPreferences(controller);
			controller.AllowAddTab = false;
			this.ResumeLayout(true);
			
			// Done
			allowapplycontrol = true;
		}

		#endregion

		#region ================== OK / Cancel

		// OK clicked
		private void apply_Click(object sender, EventArgs e)
		{
			//mxd. Check if Screenshots folder is valid
			if(screenshotsfolderpath.Text != General.Settings.ScreenshotsPath && !Directory.Exists(screenshotsfolderpath.Text.Trim())) 
			{
				General.ShowErrorMessage("Screenshots folder does not exist!\nPlease enter a correct path.", MessageBoxButtons.OK);
				return;
			}
			
			// Let the plugins know
			controller.RaiseAccept();
			
			// Check if we need to reload the resources
			reloadresources |= (General.Settings.ImageBrightness != imagebrightness.Value);

			// Apply interface
			General.Settings.ImageBrightness = imagebrightness.Value;
			General.Settings.DoubleSidedAlpha = 1.0f - (doublesidedalpha.Value * 0.1f);
			General.Settings.ActiveThingsAlpha = 1.0f - (activethingsalpha.Value * 0.1f); //mxd
			General.Settings.InactiveThingsAlpha = 1.0f - (inactivethingsalpha.Value * 0.1f); //mxd
			General.Settings.HiddenThingsAlpha = 1.0f - (hiddenthingsalpha.Value * 0.1f); //mxd
			General.Settings.DefaultViewMode = defaultviewmode.SelectedIndex;
			General.Settings.VisualFOV = fieldofview.Value * 10;
			General.Settings.MouseSpeed = mousespeed.Value * 100;
			General.Settings.MoveSpeed = movespeed.Value * 100;
			General.Settings.GZVertexScale3D = vertexScale3D.Value * 0.1f; //mxd
			General.Settings.ViewDistance = viewdistance.Value * VIEW_DISTANCE_STEP_SIZE;
			General.Settings.InvertYAxis = invertyaxis.Checked;
			General.Settings.AutoScrollSpeed = autoscrollspeed.Value;
			General.Settings.ZoomFactor = zoomfactor.Value;
			General.Settings.AnimateVisualSelection = animatevisualselection.Checked;
			General.Settings.DockersPosition = dockersposition.SelectedIndex;
			General.Settings.CollapseDockers = collapsedockers.Checked;
			General.Settings.ToolbarFile = toolbar_file.Checked;
			General.Settings.ToolbarScript = toolbar_script.Checked;
			General.Settings.ToolbarUndo = toolbar_undo.Checked;
			General.Settings.ToolbarCopy = toolbar_copy.Checked;
			General.Settings.ToolbarPrefabs = toolbar_prefabs.Checked;
			General.Settings.ToolbarFilter = toolbar_filter.Checked;
			General.Settings.ToolbarViewModes = toolbar_viewmodes.Checked;
			General.Settings.ToolbarGeometry = toolbar_geometry.Checked;
			General.Settings.ToolbarTesting = toolbar_testing.Checked;
			General.Settings.GZToolbarGZDoom = toolbar_gzdoom.Checked; //mxd
			General.Settings.ShowTextureSizes = showtexturesizes.Checked;
            General.Settings.TextureSizesBelow = texturesizesbelow.Checked; // [ZZ]
			General.Settings.StoreSelectedEditTab = cbStoreEditTab.Checked; //mxd
			General.Settings.CheckForUpdates = checkforupdates.Checked; //mxd
			General.Settings.LocateTextureGroup = locatetexturegroup.Checked; //mxd
			General.Settings.MaxRecentFiles = recentFiles.Value; //mxd
			General.Settings.ScreenshotsPath = screenshotsfolderpath.Text.Trim(); //mxd
			General.Settings.ShowFPS = cbShowFPS.Checked;
			General.Settings.AutoLaunchOnTest = autolaunchontest.Checked;
			General.Settings.FlatShadeVertices = cbFlatShadeVertices.Checked;

			// Script settings
			General.Settings.ScriptFontBold = scriptfontbold.Checked;
			General.Settings.ScriptFontName = scriptfontname.Text;
			General.Settings.ScriptOnTop = scriptontop.Checked;
			General.Settings.ScriptUseTabs = scriptusetabs.Checked;
			General.Settings.ScriptTabWidth = scripttabwidth.GetResult(General.Settings.ScriptTabWidth);
			General.Settings.ScriptAutoIndent = scriptautoindent.Checked;
			General.Settings.ScriptAllmanStyle = scriptallmanstyle.Checked; //mxd
			General.Settings.ScriptAutoCloseBrackets = scriptautoclosebrackets.Checked; //mxd
			General.Settings.ScriptShowFolding = scriptshowfolding.Checked; //mxd
			General.Settings.ScriptShowLineNumbers = scriptshowlinenumbers.Checked; //mxd
			General.Settings.ScriptAutoShowAutocompletion = scriptautoshowautocompletion.Checked; //mxd
			
			// Script font size
			int fontsize;
			if(!int.TryParse(scriptfontsize.Text, out fontsize)) fontsize = 10;
			General.Settings.ScriptFontSize = fontsize;

			//mxd. Text label settings
			General.Settings.TextLabelFontBold = textlabelfontbold.Checked;
			General.Settings.TextLabelFontName = textlabelfontname.Text;

			//mxd. Text label font size
			if(!int.TryParse(textlabelfontsize.Text, out fontsize)) fontsize = 16;
			General.Settings.TextLabelFontSize = fontsize;
			
			// Apply control keys to actions
			foreach(ListViewItem item in allactionitems) //mxd
				General.Actions[item.Name].SetShortcutKey((int)item.SubItems[1].Tag);

			// Apply the colors
			// TODO: Make this automated by using the collection
			General.Colors.Background = colorbackcolor.Color;
			General.Colors.Vertices = colorvertices.Color;
			General.Colors.Linedefs = colorlinedefs.Color;

			General.Colors.Highlight = colorhighlight.Color;
			General.Colors.Selection = colorselection.Color;
			General.Colors.Indication = colorindication.Color;
			General.Colors.Grid = colorgrid.Color;
			General.Colors.Grid64 = colorgrid64.Color;

			// Script editor colors
			General.Colors.ScriptBackground = colorscriptbackground.Color;
			General.Colors.LineNumbers = colorlinenumbers.Color;
			General.Colors.PlainText = colorplaintext.Color;
			General.Colors.Comments = colorcomments.Color;
			General.Colors.Keywords = colorkeywords.Color;
			General.Colors.Properties = colorproperties.Color; //mxd
			General.Colors.Literals = colorliterals.Color;
			General.Colors.Constants = colorconstants.Color;
			General.Colors.Strings = colorstrings.Color; //mxd
			General.Colors.Includes = colorincludes.Color; //mxd
			General.Colors.ScriptSelectionForeColor = colorselectionfore.Color; //mxd
			General.Colors.ScriptSelectionBackColor = colorselectionback.Color; //mxd
			General.Colors.ScriptIndicator = colorindicator.Color; //mxd
			General.Colors.ScriptBraceHighlight = colorbrace.Color; //mxd
			General.Colors.ScriptBadBraceHighlight = colorbracebad.Color; //mxd
			General.Colors.ScriptWhitespace = colorwhitespace.Color; //mxd
			General.Colors.ScriptFoldForeColor = colorfoldfore.Color; //mxd
			General.Colors.ScriptFoldBackColor = colorfoldback.Color; //mxd
			
			//mxd
			General.Colors.ModelWireframe = colorMD3.Color;
			General.Colors.InfoLine = colorInfo.Color;
			General.Colors.Guideline = colorguidelines.Color;
			General.Colors.ThreeDFloor = color3dFloors.Color;

			General.Colors.CreateAssistColors();
			General.Settings.BlackBrowsers = blackbrowsers.Checked;
			General.Settings.ClassicBilinear = classicbilinear.Checked;
			General.Settings.VisualBilinear = visualbilinear.Checked;
			General.Settings.QualityDisplay = qualitydisplay.Checked;

			//mxd
			General.Settings.GZSynchCameras = cbSynchCameras.Checked;
			General.Settings.GZMaxDynamicLights = tbDynLightCount.Value * 16;
			General.Settings.FilterAnisotropy = RenderDevice.AF_STEPS[anisotropicfiltering.Value];
			General.Settings.AntiAliasingSamples = RenderDevice.AA_STEPS[antialiasing.Value];
			General.Settings.GZStretchView = cbStretchView.Checked;
			General.Settings.GZVertexScale2D = (float)vertexScale.Value / 4.0f;
			General.Settings.GZOldHighlightMode = cbOldHighlightMode.Checked;
			General.Settings.GZMarkExtraFloors = cbMarkExtraFloors.Checked;

			// Paste options
			General.Settings.PasteOptions = pasteoptions.GetOptions();
			
			// Let the plugins know we're closing
			General.Plugins.OnClosePreferences(controller);
			
			// Close
			this.DialogResult = DialogResult.OK;
			this.Close();
		}

		// Cancel clicked
		private void cancel_Click(object sender, EventArgs e)
		{
			// Let the plugins know
			controller.RaiseCancel();

			// Let the plugins know we're closing
			General.Plugins.OnClosePreferences(controller);

			// Close
			this.DialogResult = DialogResult.Cancel;
			this.Close();
		}

		#endregion

		#region ================== Tabs

		// This adds a tab page
		public void AddTabPage(TabPage tab)
		{
			tabs.TabPages.Add(tab);
		}

		// Tab changes
		private void tabs_SelectedIndexChanged(object sender, EventArgs e)
		{
			// Enable/disable stuff with tabs
			if(tabs.SelectedTab != tabkeys)
			{
				this.AcceptButton = apply;
				this.CancelButton = cancel;
				apply.TabStop = true;
				cancel.TabStop = true;
				tabs.TabStop = true;
			}
			else
			{
				this.AcceptButton = null;
				this.CancelButton = null;
				apply.TabStop = false;
				cancel.TabStop = false;
				tabs.TabStop = false;
			}
			
			colorsgroup1.Visible = (tabs.SelectedTab == tabcolors);
			previewgroup.Visible = (tabs.SelectedTab == tabscripteditor);
		}

		#endregion

		#region ================== Interface Panel
		
		private void fieldofview_ValueChanged(object sender, EventArgs e)
		{
			int value = fieldofview.Value * 10;
			fieldofviewlabel.Text = value.ToString() + (char)176;
		}

		private void mousespeed_ValueChanged(object sender, EventArgs e)
		{
			int value = mousespeed.Value * 100;
			mousespeedlabel.Text = value.ToString();
		}

		private void movespeed_ValueChanged(object sender, EventArgs e)
		{
			int value = movespeed.Value * 100;
			movespeedlabel.Text = value.ToString();
		}

		//mxd
		private void vertexScale3D_ValueChanged(object sender, EventArgs e) 
		{
			vertexScale3DLabel.Text = (vertexScale3D.Value * 10) + "%";
		}

		private void viewdistance_ValueChanged(object sender, EventArgs e)
		{
			int value = viewdistance.Value * (int)VIEW_DISTANCE_STEP_SIZE;
			viewdistancelabel.Text = value + " mp";
		}

		private void autoscrollspeed_ValueChanged(object sender, EventArgs e)
		{
			if(autoscrollspeed.Value == 0)
				autoscrollspeedlabel.Text = "Off";
			else
				autoscrollspeedlabel.Text = autoscrollspeed.Value + "x";
		}

		private void zoomfactor_ValueChanged(object sender, EventArgs e)
		{
			zoomfactorlabel.Text = (zoomfactor.Value * 10) + "%";
		}

		//mxd
		private void vertexScale_ValueChanged(object sender, EventArgs e) 
		{
			vertexScaleLabel.Text = (vertexScale.Value * 25) + "%";
		}

		//mxd
		private void recentFiles_ValueChanged(object sender, EventArgs e) 
		{
			labelRecentFiles.Text = recentFiles.Value.ToString();
		}

		#endregion
		
		#region ================== Controls Panel
		
		// This updates the used keys info
		private void UpdateKeyUsedActions()
		{
			List<string> usedactions = new List<string>();
			
			// Anything selected?
			if(listactions.SelectedItems.Count > 0)
			{
				// Get info
				int thiskey = (int)listactions.SelectedItems[0].SubItems[1].Tag;
				if(thiskey != 0)
				{
					// Find actions with same key
					foreach(ListViewItem item in allactionitems)
					{
						// Don't count the selected action
						if(item != listactions.SelectedItems[0])
						{
							Action a = General.Actions[item.Name];
							int akey = (int)item.SubItems[1].Tag;

							// Check if the key combination matches
							if((thiskey & a.ShortcutMask) == (akey & a.ShortcutMask))
								usedactions.Add(a.Title + "  (" + General.Actions.Categories[a.Category] + ")");
						}
					}
				}
			}
			
			// Update info
			if(usedactions.Count == 0)
			{
				keyusedlabel.Visible = false;
				keyusedlist.Visible = false;
				keyusedlist.Items.Clear();
			}
			else
			{
				keyusedlist.Items.Clear();
				foreach(string a in usedactions) keyusedlist.Items.Add(a);
				keyusedlabel.Visible = true;
				keyusedlist.Visible = true;
			}
		}
		
		// This fills the list of available controls for the specified action
		private void FillControlsList(Action a)
		{
			actioncontrol.Items.Clear();
			
			// Fill combobox with special controls
			if(a.AllowMouse)
			{
				actioncontrol.Items.Add(new KeyControl(Keys.LButton, "LButton"));
				actioncontrol.Items.Add(new KeyControl(Keys.MButton, "MButton"));
				actioncontrol.Items.Add(new KeyControl(Keys.RButton, "RButton"));
				actioncontrol.Items.Add(new KeyControl(Keys.XButton1, "XButton1"));
				actioncontrol.Items.Add(new KeyControl(Keys.XButton2, "XButton2"));
			}
			if(a.AllowScroll)
			{
				actioncontrol.Items.Add(new KeyControl(SpecialKeys.MScrollUp, "ScrollUp"));
				actioncontrol.Items.Add(new KeyControl(SpecialKeys.MScrollDown, "ScrollDown"));
                actioncontrol.Items.Add(new KeyControl(SpecialKeys.MScrollLeft, "ScrollLeft"));
                actioncontrol.Items.Add(new KeyControl(SpecialKeys.MScrollRight, "ScrollRight"));
            }

			//mxd. Alt
			if(a.AllowMouse && !a.DisregardAlt) 
			{
				actioncontrol.Items.Add(new KeyControl(Keys.LButton | Keys.Alt, "Alt+LButton"));
				actioncontrol.Items.Add(new KeyControl(Keys.MButton | Keys.Alt, "Alt+MButton"));
				actioncontrol.Items.Add(new KeyControl(Keys.RButton | Keys.Alt, "Alt+RButton"));
				actioncontrol.Items.Add(new KeyControl(Keys.XButton1 | Keys.Alt, "Alt+XButton1"));
				actioncontrol.Items.Add(new KeyControl(Keys.XButton2 | Keys.Alt, "Alt+XButton2"));
			}
			if(a.AllowScroll && !a.DisregardAlt) 
			{
				actioncontrol.Items.Add(new KeyControl((int)SpecialKeys.MScrollUp | (int)Keys.Alt, "Alt+ScrollUp"));
				actioncontrol.Items.Add(new KeyControl((int)SpecialKeys.MScrollDown | (int)Keys.Alt, "Alt+ScrollDown"));
                actioncontrol.Items.Add(new KeyControl((int)SpecialKeys.MScrollLeft | (int)Keys.Alt, "Alt+ScrollLeft"));
                actioncontrol.Items.Add(new KeyControl((int)SpecialKeys.MScrollRight | (int)Keys.Alt, "Alt+ScrollRight"));
            }

			//Ctrl
			if(a.AllowMouse && !a.DisregardControl) 
			{
				actioncontrol.Items.Add(new KeyControl(Keys.LButton | Keys.Control, "Ctrl+LButton"));
				actioncontrol.Items.Add(new KeyControl(Keys.MButton | Keys.Control, "Ctrl+MButton"));
				actioncontrol.Items.Add(new KeyControl(Keys.RButton | Keys.Control, "Ctrl+RButton"));
				actioncontrol.Items.Add(new KeyControl(Keys.XButton1 | Keys.Control, "Ctrl+XButton1"));
				actioncontrol.Items.Add(new KeyControl(Keys.XButton2 | Keys.Control, "Ctrl+XButton2"));
			}

			if(a.AllowScroll && !a.DisregardControl) 
			{
				actioncontrol.Items.Add(new KeyControl((int)SpecialKeys.MScrollUp | (int)Keys.Control, "Ctrl+ScrollUp"));
				actioncontrol.Items.Add(new KeyControl((int)SpecialKeys.MScrollDown | (int)Keys.Control, "Ctrl+ScrollDown"));
                actioncontrol.Items.Add(new KeyControl((int)SpecialKeys.MScrollLeft | (int)Keys.Control, "Ctrl+ScrollLeft"));
                actioncontrol.Items.Add(new KeyControl((int)SpecialKeys.MScrollRight | (int)Keys.Control, "Ctrl+ScrollRight"));
            }

			//Shift
			if(a.AllowMouse && !a.DisregardShift)
			{
				actioncontrol.Items.Add(new KeyControl(Keys.LButton | Keys.Shift, "Shift+LButton"));
				actioncontrol.Items.Add(new KeyControl(Keys.MButton | Keys.Shift, "Shift+MButton"));
				actioncontrol.Items.Add(new KeyControl(Keys.RButton | Keys.Shift, "Shift+RButton"));
				actioncontrol.Items.Add(new KeyControl(Keys.XButton1 | Keys.Shift, "Shift+XButton1"));
				actioncontrol.Items.Add(new KeyControl(Keys.XButton2 | Keys.Shift, "Shift+XButton2"));
			}
			if(a.AllowScroll && !a.DisregardShift)
			{
				actioncontrol.Items.Add(new KeyControl((int)SpecialKeys.MScrollUp | (int)Keys.Shift, "Shift+ScrollUp"));
				actioncontrol.Items.Add(new KeyControl((int)SpecialKeys.MScrollDown | (int)Keys.Shift, "Shift+ScrollDown"));
                actioncontrol.Items.Add(new KeyControl((int)SpecialKeys.MScrollLeft | (int)Keys.Shift, "Shift+ScrollLeft"));
                actioncontrol.Items.Add(new KeyControl((int)SpecialKeys.MScrollRight | (int)Keys.Shift, "Shift+ScrollRight"));
            }

			//mxd. Alt-Shift
			if(a.AllowMouse && !a.DisregardShift && !a.DisregardAlt) 
			{
				actioncontrol.Items.Add(new KeyControl(Keys.LButton | Keys.Shift | Keys.Alt, "Alt+Shift+LButton"));
				actioncontrol.Items.Add(new KeyControl(Keys.MButton | Keys.Shift | Keys.Alt, "Alt+Shift+MButton"));
				actioncontrol.Items.Add(new KeyControl(Keys.RButton | Keys.Shift | Keys.Alt, "Alt+Shift+RButton"));
				actioncontrol.Items.Add(new KeyControl(Keys.XButton1 | Keys.Shift | Keys.Alt, "Alt+Shift+XButton1"));
				actioncontrol.Items.Add(new KeyControl(Keys.XButton2 | Keys.Shift | Keys.Alt, "Alt+Shift+XButton2"));
			}
			if(a.AllowScroll && !a.DisregardShift && !a.DisregardAlt) 
			{
				actioncontrol.Items.Add(new KeyControl((int)SpecialKeys.MScrollUp | (int)Keys.Shift | (int)Keys.Alt, "Alt+Shift+ScrollUp"));
				actioncontrol.Items.Add(new KeyControl((int)SpecialKeys.MScrollDown | (int)Keys.Shift | (int)Keys.Alt, "Alt+Shift+ScrollDown"));
                actioncontrol.Items.Add(new KeyControl((int)SpecialKeys.MScrollLeft | (int)Keys.Shift | (int)Keys.Alt, "Alt+Shift+ScrollLeft"));
                actioncontrol.Items.Add(new KeyControl((int)SpecialKeys.MScrollRight | (int)Keys.Shift | (int)Keys.Alt, "Alt+Shift+ScrollRight"));
            }

			//mxd. Ctrl-Alt
			if(a.AllowMouse && !a.DisregardAlt && !a.DisregardControl) 
			{
				actioncontrol.Items.Add(new KeyControl(Keys.LButton | Keys.Alt | Keys.Control, "Ctrl+Alt+LButton"));
				actioncontrol.Items.Add(new KeyControl(Keys.MButton | Keys.Alt | Keys.Control, "Ctrl+Alt+MButton"));
				actioncontrol.Items.Add(new KeyControl(Keys.RButton | Keys.Alt | Keys.Control, "Ctrl+Alt+RButton"));
				actioncontrol.Items.Add(new KeyControl(Keys.XButton1 | Keys.Alt | Keys.Control, "Ctrl+Alt+XButton1"));
				actioncontrol.Items.Add(new KeyControl(Keys.XButton2 | Keys.Alt | Keys.Control, "Ctrl+Alt+XButton2"));
			}
			if(a.AllowScroll && !a.DisregardAlt && !a.DisregardControl) 
			{
				actioncontrol.Items.Add(new KeyControl((int)SpecialKeys.MScrollUp | (int)Keys.Control | (int)Keys.Alt, "Ctrl+Alt+ScrollUp"));
				actioncontrol.Items.Add(new KeyControl((int)SpecialKeys.MScrollDown | (int)Keys.Control | (int)Keys.Alt, "Ctrl+Alt+ScrollDown"));
                actioncontrol.Items.Add(new KeyControl((int)SpecialKeys.MScrollLeft | (int)Keys.Control | (int)Keys.Alt, "Ctrl+Alt+ScrollLeft"));
                actioncontrol.Items.Add(new KeyControl((int)SpecialKeys.MScrollRight | (int)Keys.Control | (int)Keys.Alt, "Ctrl+Alt+ScrollRight"));
            }
			
			//Ctrl-Shift
			if(a.AllowMouse && !a.DisregardShift && !a.DisregardControl)
			{
				actioncontrol.Items.Add(new KeyControl(Keys.LButton | Keys.Shift | Keys.Control, "Ctrl+Shift+LButton"));
				actioncontrol.Items.Add(new KeyControl(Keys.MButton | Keys.Shift | Keys.Control, "Ctrl+Shift+MButton"));
				actioncontrol.Items.Add(new KeyControl(Keys.RButton | Keys.Shift | Keys.Control, "Ctrl+Shift+RButton"));
				actioncontrol.Items.Add(new KeyControl(Keys.XButton1 | Keys.Shift | Keys.Control, "Ctrl+Shift+XButton1"));
				actioncontrol.Items.Add(new KeyControl(Keys.XButton2 | Keys.Shift | Keys.Control, "Ctrl+Shift+XButton2"));
			}
			if(a.AllowScroll && !a.DisregardShift && !a.DisregardControl)
			{
				actioncontrol.Items.Add(new KeyControl((int)SpecialKeys.MScrollUp | (int)Keys.Shift | (int)Keys.Control, "Ctrl+Shift+ScrollUp"));
				actioncontrol.Items.Add(new KeyControl((int)SpecialKeys.MScrollDown | (int)Keys.Shift | (int)Keys.Control, "Ctrl+Shift+ScrollDown"));
                actioncontrol.Items.Add(new KeyControl((int)SpecialKeys.MScrollLeft | (int)Keys.Shift | (int)Keys.Control, "Ctrl+Shift+ScrollLeft"));
                actioncontrol.Items.Add(new KeyControl((int)SpecialKeys.MScrollRight | (int)Keys.Shift | (int)Keys.Control, "Ctrl+Shift+ScrollRight"));
            }

			//mxd. Ctrl-Alt-Shift
			if(a.AllowMouse && !a.DisregardShift && !a.DisregardControl && !a.DisregardAlt) 
			{
				actioncontrol.Items.Add(new KeyControl(Keys.LButton | Keys.Shift | Keys.Control | Keys.Alt, "Ctrl+Alt+Shift+LButton"));
				actioncontrol.Items.Add(new KeyControl(Keys.MButton | Keys.Shift | Keys.Control | Keys.Alt, "Ctrl+Alt+Shift+MButton"));
				actioncontrol.Items.Add(new KeyControl(Keys.RButton | Keys.Shift | Keys.Control | Keys.Alt, "Ctrl+Alt+Shift+RButton"));
				actioncontrol.Items.Add(new KeyControl(Keys.XButton1 | Keys.Shift | Keys.Control | Keys.Alt, "Ctrl+Alt+Shift+XButton1"));
				actioncontrol.Items.Add(new KeyControl(Keys.XButton2 | Keys.Shift | Keys.Control | Keys.Alt, "Ctrl+Alt+Shift+XButton2"));
			}
			if(a.AllowScroll && !a.DisregardShift && !a.DisregardControl && !a.DisregardAlt) 
			{
				actioncontrol.Items.Add(new KeyControl((int)SpecialKeys.MScrollUp | (int)Keys.Shift | (int)Keys.Control | (int)Keys.Alt, "Ctrl+Alt+Shift+ScrollUp"));
				actioncontrol.Items.Add(new KeyControl((int)SpecialKeys.MScrollDown | (int)Keys.Shift | (int)Keys.Control | (int)Keys.Alt, "Ctrl+Alt+Shift+ScrollDown"));
                actioncontrol.Items.Add(new KeyControl((int)SpecialKeys.MScrollLeft | (int)Keys.Shift | (int)Keys.Control | (int)Keys.Alt, "Ctrl+Alt+Shift+ScrollLeft"));
                actioncontrol.Items.Add(new KeyControl((int)SpecialKeys.MScrollRight | (int)Keys.Shift | (int)Keys.Control | (int)Keys.Alt, "Ctrl+Alt+Shift+ScrollRight"));
            }
		}
		
		// Item selected
		private void listactions_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
		{
			//mxd. Leave when not allowed to update
			if(!allowapplycontrol) return;

			string disregardkeys = "";

			// Anything selected?
			if(listactions.SelectedItems.Count > 0)
			{
				// Begin updating
				allowapplycontrol = false;

				// Get the selected action
				Action action = General.Actions[listactions.SelectedItems[0].Name];
				int key = (int)listactions.SelectedItems[0].SubItems[1].Tag;
				disregardshift = action.DisregardShift;
				disregardcontrol = action.DisregardControl;
				
				// Enable panel
				actioncontrolpanel.Enabled = true;
				actiontitle.Text = action.Title;
				actiondescription.Text = action.Description;
				actioncontrol.SelectedIndex = -1;
				actionkey.Text = "";
				
				if(disregardshift && disregardcontrol)
					disregardkeys = "Shift and Control";
				else if(disregardshift)
					disregardkeys = "Shift";
				else if(disregardcontrol)
					disregardkeys = "Control";

				disregardshiftlabel.Text = disregardshiftlabel.Tag.ToString().Replace("%s", disregardkeys);
				disregardshiftlabel.Visible = disregardshift | disregardcontrol;
				
				// Fill special controls list
				FillControlsList(action);
				
				// See if the key is in the combobox
				for(int i = 0; i < actioncontrol.Items.Count; i++)
				{
					// Select it when the key is found here
					KeyControl keycontrol = (KeyControl)actioncontrol.Items[i];
					if(keycontrol.key == key) actioncontrol.SelectedIndex = i;
				}

				// Otherwise display the key in the textbox
				if(actioncontrol.SelectedIndex == -1)
					actionkey.Text = Action.GetShortcutKeyDesc(key);
				
				// Show actions with same key
				UpdateKeyUsedActions();
				
				// Focus to the input box
				actionkey.Focus();

				// Done
				allowapplycontrol = true;
			}
		}

		// Key released
		private void listactions_KeyUp(object sender, KeyEventArgs e)
		{
			// Nothing selected?
			if(listactions.SelectedItems.Count == 0)
			{
				// Disable panel
				actioncontrolpanel.Enabled = false;
				actiontitle.Text = "(select an action from the list)";
				actiondescription.Text = "";
				actionkey.Text = "";
				actioncontrol.SelectedIndex = -1;
				disregardshiftlabel.Visible = false;
			}
			
			// Show actions with same key
			UpdateKeyUsedActions();
		}

		// Mouse released
		private void listactions_MouseUp(object sender, MouseEventArgs e)
		{
			listactions_KeyUp(sender, new KeyEventArgs(Keys.None));

			// Focus to the input box
			actionkey.Focus();
		}

		// Key combination pressed
		private void actionkey_KeyDown(object sender, KeyEventArgs e)
		{
			int key = (int)e.KeyData;
			e.SuppressKeyPress = true;

			// Leave when not allowed to update
			if(!allowapplycontrol) return;

			// Anything selected?
			if(listactions.SelectedItems.Count > 0)
			{
				// Begin updating
				allowapplycontrol = false;
				
				// Remove modifier keys from the key if needed
				if(disregardshift) key &= ~(int)Keys.Shift;
				if(disregardcontrol) key &= ~(int)Keys.Control;
				
				// Deselect anything from the combobox
				actioncontrol.SelectedIndex = -1;
				
				// Apply the key combination
				listactions.SelectedItems[0].SubItems[1].Text = Action.GetShortcutKeyDesc(key);
				listactions.SelectedItems[0].SubItems[1].Tag = key;
				actionkey.Text = Action.GetShortcutKeyDesc(key);
				
				// Show actions with same key
				UpdateKeyUsedActions();
				
				// Done
				allowapplycontrol = true;
			}
		}

		// Special key selected
		private void actioncontrol_SelectedIndexChanged(object sender, EventArgs e)
		{
			// Leave when not allowed to update
			if(!allowapplycontrol) return;

			// Anything selected?
			if((actioncontrol.SelectedIndex > -1) && (listactions.SelectedItems.Count > 0))
			{
				// Begin updating
				allowapplycontrol = false;

				// Remove text from textbox
				actionkey.Text = "";

				// Get the key control
				KeyControl key = (KeyControl)actioncontrol.SelectedItem;

				// Apply the key combination
				listactions.SelectedItems[0].SubItems[1].Text = Action.GetShortcutKeyDesc(key.key);
				listactions.SelectedItems[0].SubItems[1].Tag = key.key;
				
				// Show actions with same key
				UpdateKeyUsedActions();
				
				// Focus to the input box
				actionkey.Focus();

				// Done
				allowapplycontrol = true;
			}
		}

		// Clear clicked
		private void actioncontrolclear_Click(object sender, EventArgs e)
		{
			// Begin updating
			allowapplycontrol = false;

			// Clear textbox and combobox
			actionkey.Text = "";
			actioncontrol.SelectedIndex = -1;

			// Apply the key combination
			listactions.SelectedItems[0].SubItems[1].Text = "";
			listactions.SelectedItems[0].SubItems[1].Tag = 0;
			
			// Show actions with same key
			UpdateKeyUsedActions();
			
			// Focus to the input box
			actionkey.Focus();

			// Done
			allowapplycontrol = true;
		}

		//mxd
		private void bClearActionFilter_Click(object sender, EventArgs e) 
		{
			tbFilterActions.Clear();
		}

		//mxd
		private void tbFilterActions_TextChanged(object sender, EventArgs e)
		{
			ListViewItem curselection = (listactions.SelectedItems.Count > 0 ? listactions.SelectedItems[0] : null);
			ListViewItem toselect = null;

			allowapplycontrol = false;
			listactions.BeginUpdate();
			listactions.Items.Clear();
			
			// Restore everything
			if(string.IsNullOrEmpty(tbFilterActions.Text))
			{
				// Restore items and groups
				for(int i = 0; i < allactionitems.Count; i++)
				{
					if(allactionitemsgroups[i] != -1)
						allactionitems[i].Group = listactions.Groups[allactionitemsgroups[i]];

					// Item sould be added AFTER restoring it's group, otherwise item sorting will be screwed!
					listactions.Items.Add(allactionitems[i]);

					// Restore selection?
					if(allactionitems[i] == curselection) toselect = curselection;
				}
			}
			// Apply filtering
			else
			{
				string match = tbFilterActions.Text.ToUpperInvariant();
				for(int i = 0; i < allactionitems.Count; i++) 
				{
					if(allactionitems[i].Text.ToUpperInvariant().Contains(match))
					{
						// Restore group
						if(allactionitemsgroups[i] != -1)
							allactionitems[i].Group = listactions.Groups[allactionitemsgroups[i]];

						// Add item
						listactions.Items.Add(allactionitems[i]);

						// Restore selection?
						if(allactionitems[i] == curselection) toselect = curselection;
					} 
				}
			}

			// Restore selection?
			if(toselect != null)
			{
				toselect.Selected = true;
				listactions.EnsureVisible(toselect.Index);
			}

			listactions.EndUpdate();
			allowapplycontrol = true;
		}

		#endregion

		#region ================== Colors Panel

		private void imagebrightness_ValueChanged(object sender, EventArgs e)
		{
			imagebrightnesslabel.Text = "+ " + imagebrightness.Value + " y";
		}

		private void doublesidedalpha_ValueChanged(object sender, EventArgs e)
		{
			int percent = doublesidedalpha.Value * 10;
			doublesidedalphalabel.Text = percent + "%";
		}

		//mxd
		private void activethingsalpha_ValueChanged(object sender, EventArgs e)
		{
			int percent = activethingsalpha.Value * 10;
			activethingsalphalabel.Text = percent + "%";
		}

		//mxd
		private void inactivethingsalpha_ValueChanged(object sender, EventArgs e)
		{
			int percent = inactivethingsalpha.Value * 10;
			inactivethingsalphalabel.Text = percent + "%";
		}

		//mxd
		private void hiddenthingsalpha_ValueChanged(object sender, EventArgs e)
		{
			int percent = hiddenthingsalpha.Value * 10;
			hiddenthingsalphalabel.Text = percent + "%";
		}

		//mxd
		private void tbDynLightCount_ValueChanged(object sender, EventArgs e) 
		{
			labelDynLightCount.Text = (tbDynLightCount.Value * 16).ToString();
		}

		//mxd
		private void anisotropicfiltering_ValueChanged(object sender, EventArgs e)
		{
			labelanisotropicfiltering.Text = anisotropicfiltering.Value == 0 ? "None" : (int)RenderDevice.AF_STEPS[anisotropicfiltering.Value] + "x";
		}

		//mxd
		private void antialiasing_ValueChanged(object sender, EventArgs e)
		{
			labelantialiasing.Text = (antialiasing.Value == 0 ? "None" : RenderDevice.AA_STEPS[antialiasing.Value] + " samples");
		}

		#endregion

		#region ================== Script Editor Panel (mxd)

		private void scriptfontbold_CheckedChanged(object sender, EventArgs e)
		{
			if(allowapplycontrol) scriptedit.FontBold = scriptfontbold.Checked;
		}

		private void scriptfontsize_SelectedIndexChanged(object sender, EventArgs e) 
		{
			if(allowapplycontrol)
			{
				int fontsize;
				if(int.TryParse(scriptfontsize.Text, out fontsize))
					scriptedit.FontSize = fontsize;
			}
		}

		private void scripttabwidth_WhenTextChanged(object sender, EventArgs e)
		{
			if(allowapplycontrol)
			{
				int tabwidth;
				if(int.TryParse(scripttabwidth.Text, out tabwidth))
					scriptedit.TabWidth = tabwidth;
			}
		}

		private void scriptfontname_SelectedIndexChanged(object sender, EventArgs e)
		{
			if(allowapplycontrol) scriptedit.FontName = scriptfontname.Text;
		}

		private void scriptshowlinenumbers_CheckedChanged(object sender, EventArgs e)
		{
			if(allowapplycontrol) scriptedit.ShowLineNumbers = scriptshowlinenumbers.Checked;
		}

		private void scriptshowfolding_CheckedChanged(object sender, EventArgs e)
		{
			if(allowapplycontrol) scriptedit.ShowFolding = scriptshowfolding.Checked;
		}

		private void colorscriptbackground_ColorChanged(object sender, EventArgs e)
		{
			if(allowapplycontrol) scriptedit.ScriptBackground = colorscriptbackground.Color.ToColor();
		}

		private void colorlinenumbers_ColorChanged(object sender, EventArgs e)
		{
			if(allowapplycontrol) scriptedit.LineNumbers = colorlinenumbers.Color.ToColor();
		}

		private void colorplaintext_ColorChanged(object sender, EventArgs e)
		{
			if(allowapplycontrol) scriptedit.PlainText = colorplaintext.Color.ToColor();
		}

		private void colorcomments_ColorChanged(object sender, EventArgs e)
		{
			if(allowapplycontrol) scriptedit.Comments = colorcomments.Color.ToColor();
		}

		private void colorkeywords_ColorChanged(object sender, EventArgs e)
		{
			if(allowapplycontrol) scriptedit.Keywords = colorkeywords.Color.ToColor();
		}

		private void colorproperties_ColorChanged(object sender, EventArgs e)
		{
			if(allowapplycontrol) scriptedit.Properties = colorproperties.Color.ToColor();
		}

		private void colorstrings_ColorChanged(object sender, EventArgs e)
		{
			if(allowapplycontrol) scriptedit.Strings = colorstrings.Color.ToColor();
		}

		private void colorliterals_ColorChanged(object sender, EventArgs e)
		{
			if(allowapplycontrol) scriptedit.Literals = colorliterals.Color.ToColor();
		}

		private void colorconstants_ColorChanged(object sender, EventArgs e)
		{
			if(allowapplycontrol) scriptedit.Constants = colorconstants.Color.ToColor();
		}

		private void colorincludes_ColorChanged(object sender, EventArgs e)
		{
			if(allowapplycontrol) scriptedit.Includes = colorincludes.Color.ToColor();
		}

		private void colorselectionfore_ColorChanged(object sender, EventArgs e)
		{
			if(allowapplycontrol) scriptedit.SelectionForeColor = colorselectionfore.Color.ToColor();
		}

		private void colorselectionback_ColorChanged(object sender, EventArgs e)
		{
			if(allowapplycontrol) scriptedit.SelectionBackColor = colorselectionback.Color.ToColor();
		}

		private void colorindicator_ColorChanged(object sender, EventArgs e)
		{
			if(allowapplycontrol) scriptedit.ScriptIndicator = colorindicator.Color.ToColor();
		}

		private void colorbrace_ColorChanged(object sender, EventArgs e)
		{
			if(allowapplycontrol) scriptedit.BraceHighlight = colorbrace.Color.ToColor();
		}

		private void colorbracebad_ColorChanged(object sender, EventArgs e)
		{
			if(allowapplycontrol) scriptedit.BadBraceHighlight = colorbracebad.Color.ToColor();
		}

		private void colorwhitespace_ColorChanged(object sender, EventArgs e)
		{
			if(allowapplycontrol) scriptedit.WhitespaceColor = colorwhitespace.Color.ToColor();
		}

		private void colorfoldfore_ColorChanged(object sender, EventArgs e)
		{
			if(allowapplycontrol) scriptedit.FoldForeColor = colorfoldfore.Color.ToColor();
		}

		private void colorfoldback_ColorChanged(object sender, EventArgs e)
		{
			if(allowapplycontrol) scriptedit.FoldBackColor = colorfoldback.Color.ToColor();
		}

		private void scriptcolorpresets_SelectedIndexChanged(object sender, EventArgs e)
		{
			if(!allowapplycontrol) return;
			switch(scriptcolorpresets.SelectedIndex)
			{
				case 0: // Restore current colors
					colorscriptbackground.Color = General.Colors.ScriptBackground;
					colorlinenumbers.Color = General.Colors.LineNumbers;
					colorplaintext.Color = General.Colors.PlainText;
					colorcomments.Color = General.Colors.Comments;
					colorkeywords.Color = General.Colors.Keywords;
					colorproperties.Color = General.Colors.Properties;
					colorliterals.Color = General.Colors.Literals;
					colorconstants.Color = General.Colors.Constants;
					colorstrings.Color = General.Colors.Strings;
					colorincludes.Color = General.Colors.Includes;
					colorindicator.Color = General.Colors.ScriptIndicator;
					colorbrace.Color = General.Colors.ScriptBraceHighlight;
					colorbracebad.Color = General.Colors.ScriptBadBraceHighlight;
					colorwhitespace.Color = General.Colors.ScriptWhitespace;
					colorfoldfore.Color = General.Colors.ScriptFoldForeColor;
					colorfoldback.Color = General.Colors.ScriptFoldBackColor;
					colorselectionfore.Color = General.Colors.ScriptSelectionForeColor;
					colorselectionback.Color = General.Colors.ScriptSelectionBackColor;
					break;

				case 1: // Light theme
					colorscriptbackground.Color = PixelColor.FromInt(-1);
					colorlinenumbers.Color = PixelColor.FromInt(-13921873);
					colorplaintext.Color = PixelColor.FromInt(-16777216);
					colorcomments.Color = PixelColor.FromInt(-16744448);
					colorkeywords.Color = PixelColor.FromInt(-16741493);
					colorproperties.Color = PixelColor.FromInt(-16752191);
					colorliterals.Color = PixelColor.FromInt(-16776999);
					colorconstants.Color = PixelColor.FromInt(-8372160);
					colorstrings.Color = PixelColor.FromInt(-8388608);
					colorincludes.Color = PixelColor.FromInt(-9868951);
					colorindicator.Color = PixelColor.FromInt(-16711936);
					colorbrace.Color = PixelColor.FromInt(-16711681);
					colorbracebad.Color = PixelColor.FromInt(-65536);
					colorwhitespace.Color = PixelColor.FromInt(-8355712);
					colorfoldfore.Color = PixelColor.FromColor(SystemColors.ControlDark);
					colorfoldback.Color = PixelColor.FromColor(SystemColors.ControlLightLight);
					colorselectionfore.Color = PixelColor.FromInt(-1);
					colorselectionback.Color = PixelColor.FromInt(-13395457);
					break;

				case 2: // Dark theme
					colorscriptbackground.Color = new PixelColor(255, 34, 40, 42);
					colorlinenumbers.Color = new PixelColor(255, 63, 78, 73);
					colorplaintext.Color = new PixelColor(255, 241, 242, 243);
					colorcomments.Color = new PixelColor(255, 102, 116, 123);
					colorkeywords.Color = new PixelColor(255, 103, 140, 177);
					colorproperties.Color = PixelColor.FromColor(Color.LightSkyBlue);
					colorliterals.Color = new PixelColor(255, 255, 205, 34);
					colorconstants.Color = new PixelColor(255, 147, 199, 99);
					colorstrings.Color = new PixelColor(255, 236, 118, 0);
					colorincludes.Color = new PixelColor(255, 160, 130, 189);
					colorindicator.Color = new PixelColor(255, 211, 209, 85);
					colorbrace.Color = new PixelColor(255, 135, 211, 85);
					colorbracebad.Color = new PixelColor(255, 150, 58, 70);
					colorwhitespace.Color = new PixelColor(255, 241, 242, 243);
					colorfoldfore.Color = new PixelColor(255, 37, 92, 111);
					colorfoldback.Color = new PixelColor(255, 41, 49, 52);
					colorselectionfore.Color = new PixelColor(255, 255, 255, 255);
					colorselectionback.Color = new PixelColor(255, 71, 71, 71);
					break;
			}

			// Apply changes
			scriptedit.ScriptBackground = colorscriptbackground.Color.ToColor();
			scriptedit.LineNumbers = colorlinenumbers.Color.ToColor();
			scriptedit.PlainText = colorplaintext.Color.ToColor();
			scriptedit.Comments = colorcomments.Color.ToColor();
			scriptedit.Keywords = colorkeywords.Color.ToColor();
			scriptedit.Properties = colorproperties.Color.ToColor();
			scriptedit.Strings = colorstrings.Color.ToColor();
			scriptedit.Literals = colorliterals.Color.ToColor();
			scriptedit.Constants = colorconstants.Color.ToColor();
			scriptedit.Includes = colorincludes.Color.ToColor();
			scriptedit.SelectionForeColor = colorselectionfore.Color.ToColor();
			scriptedit.SelectionBackColor = colorselectionback.Color.ToColor();
			scriptedit.ScriptIndicator = colorindicator.Color.ToColor();
			scriptedit.BraceHighlight = colorbrace.Color.ToColor();
			scriptedit.BadBraceHighlight = colorbracebad.Color.ToColor();
			scriptedit.WhitespaceColor = colorwhitespace.Color.ToColor();
			scriptedit.FoldForeColor = colorfoldfore.Color.ToColor();
			scriptedit.FoldBackColor = colorfoldback.Color.ToColor();
		}

		#endregion

		#region ================== Screenshots Stuff (mxd)

		private void resetscreenshotsdir_Click(object sender, EventArgs e) 
		{
			screenshotsfolderpath.Text = General.DefaultScreenshotsPath;
			browseScreenshotsFolderDialog.SelectedPath = General.DefaultScreenshotsPath;
		}

		private void browsescreenshotsdir_Click(object sender, EventArgs e) 
		{
			if(browseScreenshotsFolderDialog.ShowDialog(General.MainWindow) == DialogResult.OK) 
				screenshotsfolderpath.Text = browseScreenshotsFolderDialog.SelectedPath;
		}

		#endregion

		// Help
		private void PreferencesForm_HelpRequested(object sender, HelpEventArgs hlpevent)
		{
			if(!actionkey.Focused)
			{
				General.ShowHelp("w_preferences.html");
				hlpevent.Handled = true;
			}
		}

        /*
		// This writes all action help files using a template and some basic info from the actions.
		// Also writes actioncontents.txt with all files to be inserted into Contents.hhc.
		// Only used during development. Actual button to call this has been removed.
		private void gobutton_Click(object sender, EventArgs e)
		{
			string template = File.ReadAllText(Path.Combine(General.AppPath, "..\\Help\\a_template.html"));
			StringBuilder contents = new StringBuilder("\t<UL>\r\n");
			string filename;
			
			// Go for all actions
			Action[] actions = General.Actions.GetAllActions();
			foreach(Action a in actions)
			{
				StringBuilder actionhtml = new StringBuilder(template);
				actionhtml.Replace("ACTIONTITLE", a.Title);
				actionhtml.Replace("ACTIONDESCRIPTION", a.Description);
				actionhtml.Replace("ACTIONCATEGORY", General.Actions.Categories[a.Category]);
				filename = Path.Combine(General.AppPath, "..\\Help\\a_" + a.Name + ".html");
				File.WriteAllText(filename, actionhtml.ToString());
				
				contents.Append("\t\t<LI> <OBJECT type=\"text/sitemap\">\r\n");
				contents.Append("\t\t\t<param name=\"Name\" value=\"" + a.Title + "\">\r\n");
				contents.Append("\t\t\t<param name=\"Local\" value=\"a_" + a.Name + ".html\">\r\n");
				contents.Append("\t\t\t</OBJECT>\r\n");
			}
			
			contents.Append("\t</UL>\r\n");
			filename = Path.Combine(General.AppPath, "..\\Help\\actioncontents.txt");
			File.WriteAllText(filename, contents.ToString());
		}
		*/
    }
}