
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
using System.Windows.Forms;
using CodeImp.DoomBuilder.Compilers;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Controls.Scripting;
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.Data.Scripting;
using CodeImp.DoomBuilder.Windows;
using ScintillaNET;

#endregion

namespace CodeImp.DoomBuilder.Controls
{
	internal partial class ScriptEditorPanel : UserControl
	{
		#region ================== Constants

		private static readonly Color QUICKSEARCH_FAIL_COLOR = Color.MistyRose; //mxd
		
		#endregion
		
		#region ================== Variables
		
		private List<ScriptConfiguration> scriptconfigs;
		private List<CompilerError> compilererrors;

		// Find/Replace
		private ScriptFindReplaceForm findreplaceform;
		private FindReplaceOptions findoptions;

		// Quick search bar settings (mxd)
		private static bool matchwholeword;
		private static bool matchcase;

		//mxd. Status update
		private ScriptStatusInfo status;
		private int statusflashcount;
		private bool statusflashicon;

		//mxd
		private ScriptEditorForm parentform;
		private ScriptIconsManager iconsmgr;

		//mxd. Text editor settings
		private bool showwhitespace;
		private bool wraplonglines;
		private bool blockupdate;
		
		#endregion
		
		#region ================== Properties
		
		public ScriptDocumentTab ActiveTab { get { return (tabs.SelectedTab as ScriptDocumentTab); } }
		internal ScriptIconsManager Icons { get { return iconsmgr; } }
		public bool ShowWhitespace { get { return showwhitespace; } }
		public bool WrapLongLines { get { return wraplonglines; } }

		#endregion
		
		#region ================== Constructor
		
		// Constructor
		public ScriptEditorPanel()
		{
			InitializeComponent();
			iconsmgr = new ScriptIconsManager(scripticons); //mxd
			tabs.ImageList = scripticons; //mxd
			PreviewKeyDown += new PreviewKeyDownEventHandler(ScriptEditorPanel_PreviewKeyDown);
			KeyDown += new KeyEventHandler(ScriptEditorPanel_KeyDown);
			KeyUp += new KeyEventHandler(ScriptEditorPanel_KeyDown);
		}
		
		// This initializes the control
		public void Initialize(ScriptEditorForm form)
		{
			parentform = form; //mxd
			
			// Make list of script configs
			scriptconfigs = new List<ScriptConfiguration>(General.ScriptConfigs.Values);
			scriptconfigs.Add(new ScriptConfiguration());
			scriptconfigs.Sort();

			// Load the script lumps
			ScriptDocumentTab activetab = null; //mxd
			foreach(MapLumpInfo maplumpinfo in General.Map.Config.MapLumps.Values)
			{
				// Is this a script lump?
				if(maplumpinfo.ScriptBuild) //mxd
				{
					ScriptConfiguration config = General.GetScriptConfiguration(ScriptType.ACS);
					if(config == null)
					{
						General.ErrorLogger.Add(ErrorType.Warning, "Unable to find script configuration for \"" + ScriptType.ACS + "\" script type. Using plain text configuration.");
						config = new ScriptConfiguration();
					}

					// Load this!
					ScriptLumpDocumentTab t = new ScriptLumpDocumentTab(this, maplumpinfo.Name, config);
					
					//mxd. Apply stored settings?
					if(General.Map.Options.ScriptDocumentSettings.ContainsKey(maplumpinfo.Name))
					{
						t.SetViewSettings(General.Map.Options.ScriptDocumentSettings[maplumpinfo.Name]);
						if(General.Map.Options.ScriptDocumentSettings[maplumpinfo.Name].IsActiveTab) 
							activetab = t;
					}
					else
					{
						t.SetDefaultViewSettings();
					}
					
					t.OnTextChanged += tabpage_OnLumpTextChanged; //mxd
					t.Editor.Scintilla.UpdateUI += scintilla_OnUpdateUI; //mxd
					tabs.TabPages.Add(t);
				} 
				else if(maplumpinfo.Script != null)
				{
					// Load this!
					ScriptLumpDocumentTab t = new ScriptLumpDocumentTab(this, maplumpinfo.Name, maplumpinfo.Script);

					//mxd. Apply stored settings?
					if(General.Map.Options.ScriptDocumentSettings.ContainsKey(maplumpinfo.Name))
					{
						t.SetViewSettings(General.Map.Options.ScriptDocumentSettings[maplumpinfo.Name]);
						if(General.Map.Options.ScriptDocumentSettings[maplumpinfo.Name].IsActiveTab)
							activetab = t;
					}
					else
					{
						t.SetDefaultViewSettings();
					}
					
					t.OnTextChanged += tabpage_OnLumpTextChanged; //mxd
					t.Editor.Scintilla.UpdateUI += scintilla_OnUpdateUI; //mxd
					tabs.TabPages.Add(t);
				}
			}

			//mxd. Reselect previously selected tab
			if(activetab != null)
			{
				tabs.SelectedTab = activetab;
			}
			//mxd. Select the "Scripts" tab, because that's what user will want 99% of time
			else if(tabs.TabPages.Count > 0)
			{
				int scriptsindex = -1;
				for(int i = 0; i < tabs.TabPages.Count; i++)
				{
					if(tabs.TabPages[i].Text == "SCRIPTS")
					{
						scriptsindex = i;
						break;
					}
				}

				tabs.SelectedIndex = (scriptsindex == -1 ? 0 : scriptsindex);
				activetab = tabs.TabPages[tabs.SelectedIndex] as ScriptDocumentTab;
			}

			//mxd. Apply quick search settings
			searchmatchcase.Checked = matchcase;
			searchwholeword.Checked = matchwholeword;
			searchbox_TextChanged(this, EventArgs.Empty);
			
			//mxd. If the map or script navigator has any compile errors, show them
			if(activetab != null)
			{
				List<CompilerError> errors = (General.Map.Errors.Count > 0 ? General.Map.Errors : activetab.UpdateNavigator());
				if(errors.Count > 0) ShowErrors(errors, false);
				else ClearErrors();
			}
			else
			{
				ClearErrors();
			}
			
			// Done
			UpdateInterface(true);
		}
		
		// This applies user preferences
		public void ApplySettings()
		{
			// Set Errors panel settings
			errorlist.Columns[0].Width = General.Settings.ReadSetting("scriptspanel.errorscolumn0width", errorlist.Columns[0].Width);
			errorlist.Columns[1].Width = General.Settings.ReadSetting("scriptspanel.errorscolumn1width", errorlist.Columns[1].Width);
			errorlist.Columns[2].Width = General.Settings.ReadSetting("scriptspanel.errorscolumn2width", errorlist.Columns[2].Width);

			//mxd. Set script splitter position and state
			if(General.Settings.ReadSetting("scriptspanel.scriptsplittercollapsed", false))
				scriptsplitter.IsCollapsed = true;

            int splitterdistance = General.Settings.ReadSetting("scriptspanel.scriptsplitterdistance", int.MinValue);
			if(splitterdistance == int.MinValue)
			{
				splitterdistance = 250;
				if(MainForm.DPIScaler.Width != 1.0f)
					splitterdistance = (int)Math.Round(splitterdistance * MainForm.DPIScaler.Width);
			}
			scriptsplitter.SplitPosition = splitterdistance;

			//mxd. Selected info tab
			infotabs.SelectedIndex = General.Settings.ReadSetting("scriptspanel.infotabindex", 0);

			//mxd. Set text editor settings
			showwhitespace = General.Settings.ReadSetting("scriptspanel.showwhitespace", false);
			buttonwhitespace.Checked = showwhitespace;
			menuwhitespace.Checked = showwhitespace;
			wraplonglines = General.Settings.ReadSetting("scriptspanel.wraplonglines", false);
			buttonwordwrap.Checked = wraplonglines;
			menuwordwrap.Checked = wraplonglines;
			menustayontop.Checked = General.Settings.ScriptOnTop;
			ApplyTabSettings();
		}
		
		// This saves user preferences
		public void SaveSettings()
		{
			General.Settings.WriteSetting("scriptspanel.errorscolumn0width", errorlist.Columns[0].Width);
			General.Settings.WriteSetting("scriptspanel.errorscolumn1width", errorlist.Columns[1].Width);
			General.Settings.WriteSetting("scriptspanel.errorscolumn2width", errorlist.Columns[2].Width); //mxd
			General.Settings.WriteSetting("scriptspanel.scriptsplittercollapsed", scriptsplitter.IsCollapsed); //mxd
			General.Settings.WriteSetting("scriptspanel.scriptsplitterdistance", scriptsplitter.SplitPosition); //mxd
			General.Settings.WriteSetting("scriptspanel.infotabindex", infotabs.SelectedIndex); //mxd
			General.Settings.WriteSetting("scriptspanel.showwhitespace", showwhitespace); //mxd
			General.Settings.WriteSetting("scriptspanel.wraplonglines", wraplonglines); //mxd
		}

		//mxd
		private void ApplyTabSettings()
		{
			foreach(var tp in tabs.TabPages)
			{
				ScriptDocumentTab scripttab = (tp as ScriptDocumentTab);
				if(scripttab != null)
				{
					scripttab.WrapLongLines = buttonwordwrap.Checked;
					scripttab.ShowWhitespace = buttonwhitespace.Checked;
				}
			}
		}

		//mxd. Handle heavy resource loss
		internal void OnScriptResourceLost(ScriptResourceDocumentTab sourcetab)
		{
			// Resource was lost. Remove tab
			if(!sourcetab.IsChanged)
			{
				tabs.TabPages.Remove(sourcetab);
			}
			// Resource was lost, but the tab contains unsaved changes. Replace it with ScriptFileDocumentTab
			else
			{
				int tabindex = tabs.TabPages.IndexOf(sourcetab);
				var newtab = new ScriptFileDocumentTab(sourcetab);

				tabs.SuspendLayout();
				tabs.TabPages.Remove(sourcetab);
				tabs.TabPages.Insert(tabindex, newtab);
				tabs.ResumeLayout();
			}
		}

		#endregion

		#region ================== Methods

		// [ZZ] Find and Replace
		//      Search the editor text with wrap-around disabled to avoid infinite
		//      recursion when the searched phrase is a substring of the replacement.
		public int FindReplace(FindReplaceOptions options)
        {
            FindReplaceOptions singlesearchoptions = new FindReplaceOptions(options)
			{
				SearchMode = FindReplaceSearchMode.CURRENT_FILE,
				WrapAroundDisabled = true
			};
            List<ScriptDocumentTab> rtabs = new List<ScriptDocumentTab>();

            switch (options.SearchMode)
            {
                case FindReplaceSearchMode.CURRENT_FILE:
                    if (ActiveTab == null)
                        return 0;
                    rtabs.Add(ActiveTab);
                    break;

                case FindReplaceSearchMode.OPENED_TABS_ALL_SCRIPT_TYPES:
                    foreach (ScriptDocumentTab tab in tabs.TabPages)
                        rtabs.Add(tab);
                    break;
            }

            int replacements = 0;
            foreach (ScriptDocumentTab tab in rtabs)
			{
				// Reset editor cursor to the start, searching until the end
				tab.SelectionStart = tab.SelectionEnd = 0;

				// Count the number of replacements made in this tab.
				while (tab.FindNext(singlesearchoptions))
                {
					tab.ReplaceSelection(options.ReplaceWith);

                    replacements++;
                }
            }

            return replacements;
        }

		// Find Next
		public bool FindNext(FindReplaceOptions options)
		{
			// Save the options
			findoptions = options;
			return FindNext();
		}

		// Find Next with saved options
		public bool FindNext()
		{
			if(!string.IsNullOrEmpty(findoptions.FindText) && (ActiveTab != null))
			{
				if(!ActiveTab.FindNext(findoptions))
				{
					DisplayStatus(ScriptStatusType.Warning, "Can't find any occurrence of \"" + findoptions.FindText + "\".");
					return false;
				}

				return true;
			}
			else
			{
				General.MessageBeep(MessageBeepType.Default);
				return false;
			}
		}

		// Find Previous
		public bool FindPrevious(FindReplaceOptions options) 
		{
			// Save the options
			findoptions = options;
			return FindPrevious();
		}

		// Find Previous with saved options (mxd)
		public bool FindPrevious() 
		{
			if(!string.IsNullOrEmpty(findoptions.FindText) && (ActiveTab != null)) 
			{
				if(!ActiveTab.FindPrevious(findoptions))
				{
					DisplayStatus(ScriptStatusType.Warning, "Can't find any occurrence of \"" + findoptions.FindText + "\".");
					return false;
				}

				return true;
			} 
			else 
			{
				General.MessageBeep(MessageBeepType.Default);
				return false;
			}
		}

		//mxd
		public bool FindNextWrapAround(FindReplaceOptions options)
		{
			var curtab = ActiveTab;
			if(curtab == null) return false; // Boilerplate
			switch(options.SearchMode)
			{
				case FindReplaceSearchMode.CURRENT_FILE: return false; // Let the tab handle wrap-around

				case FindReplaceSearchMode.OPENED_TABS_ALL_SCRIPT_TYPES:
					ScriptType targettabtype = curtab.Config.ScriptType;
					bool checktabtype = false;

					// Search in processed tab only
					var searchoptions = new FindReplaceOptions(options) { SearchMode = FindReplaceSearchMode.CURRENT_FILE };

					// Find next suitable tab...
					int start = tabs.TabPages.IndexOf(curtab);

					// Search after current tab
					for(int i = start + 1; i < tabs.TabPages.Count; i++)
					{
						var t = tabs.TabPages[i] as ScriptDocumentTab;
						if(t != null && (!checktabtype || t.Config.ScriptType == targettabtype) && t.FindNext(searchoptions))
						{
							// Next match found!
							tabs.SelectTab(t);
							return true;
						}
					}

					// Search before current tab
					if(start > 0)
					{
						for(int i = 0; i < start; i++)
						{
							var t = tabs.TabPages[i] as ScriptDocumentTab;
							if(t != null && (!checktabtype || t.Config.ScriptType == targettabtype) && t.FindNext(searchoptions))
							{
								// Next match found!
								tabs.SelectTab(t);
								return true;
							}
						}
					}

					// No dice
					return false;

				default: throw new NotImplementedException("Unknown FindReplaceSearchMode!");
			}
		}

		//mxd
		public bool FindPreviousWrapAround(FindReplaceOptions options)
		{
			var curtab = ActiveTab;
			if(curtab == null) return false; // Boilerplate
			switch(options.SearchMode)
			{
				case FindReplaceSearchMode.CURRENT_FILE: return false; // Let the tab handle wrap-around

				case FindReplaceSearchMode.OPENED_TABS_ALL_SCRIPT_TYPES:
					ScriptType targettabtype = curtab.Config.ScriptType;
					bool checktabtype = false;

					// Search in processed tab only
					var searchoptions = new FindReplaceOptions(options) { SearchMode = FindReplaceSearchMode.CURRENT_FILE };

					// Find previous suitable tab...
					int start = tabs.TabPages.IndexOf(curtab);

					// Search before current tab
					for(int i = start - 1; i > -1; i--)
					{
						var t = tabs.TabPages[i] as ScriptDocumentTab;
						if(t != null && (!checktabtype || t.Config.ScriptType == targettabtype) && t.FindPrevious(searchoptions))
						{
							// Previous match found!
							tabs.SelectTab(t);
							return true;
						}
					}

					// Search after current tab
					if(start < tabs.TabPages.Count - 1)
					{
						for(int i = tabs.TabPages.Count - 1; i > start; i--)
						{
							var t = tabs.TabPages[i] as ScriptDocumentTab;
							if(t != null && (!checktabtype || t.Config.ScriptType == targettabtype) && t.FindPrevious(searchoptions))
							{
								// Previous match found!
								tabs.SelectTab(t);
								return true;
							}
						}
					}

					// No dice
					return false;

				default: throw new NotImplementedException("Unknown FindReplaceSearchMode!");
			}
		}
		
		// Replace if possible
		public bool Replace(FindReplaceOptions options)
		{
			ScriptDocumentTab curtab = ActiveTab; //mxd
			if(!string.IsNullOrEmpty(options.FindText) && options.ReplaceWith != null && curtab != null && !curtab.IsReadOnly)
			{
				if(string.Compare(curtab.SelectedText, options.FindText, !options.CaseSensitive) == 0)
				{
					// Replace selection
					curtab.ReplaceSelection(options.ReplaceWith);
					return true;
				}
			}

			return false;
		}

		// This closed the Find & Replace subwindow
		public void CloseFindReplace(bool closing)
		{
			if(findreplaceform != null)
			{
				if(!closing) findreplaceform.Close();
				findreplaceform = null;
			}
		}

		// This opens the Find & Replace subwindow
		public void OpenFindAndReplace()
		{
			if(findreplaceform == null)
				findreplaceform = new ScriptFindReplaceForm();

			try
			{
				findreplaceform.CanReplace = !ActiveTab.IsReadOnly; //mxd
				
				if(findreplaceform.Visible)
					findreplaceform.Focus();
				else
					findreplaceform.Show(this.ParentForm);

				if(ActiveTab.SelectionEnd != ActiveTab.SelectionStart)
					findreplaceform.SetFindText(ActiveTab.SelectedText);
			}
			catch { } // If we can't pop up the find/replace form right now, thats just too bad.
		}

		//mxd
		public void GoToLine()
		{
			if(ActiveTab == null) return;

			var form = new ScriptGoToLineForm { LineNumber = ActiveTab.Editor.Scintilla.CurrentLine };
			if(form.ShowDialog(this.parentform) == DialogResult.OK)
			{
				ActiveTab.MoveToLine(form.LineNumber - 1);
			}
		}

		// This refreshes all settings
		public void RefreshSettings()
		{
			foreach(ScriptDocumentTab t in tabs.TabPages)
			{
				t.RefreshSettings();
			}
		}

		// This clears all error marks and hides the errors list
		public void ClearErrors()
		{
			// Hide list
			if(infotabs.SelectedTab == taberrors) scriptsplitter.IsCollapsed = true;
			errorlist.Items.Clear();

			// Clear marks
			foreach(ScriptDocumentTab t in tabs.TabPages)
			{
				t.ClearMarks();
			}
		}
		
		// This shows the errors panel with the given errors
		// Also updates the scripts with markers for the given errors
		public void ShowErrors(IEnumerable<CompilerError> errors, bool combine)
		{
			// Copy list
			if(combine) //mxd
			{
				if(compilererrors == null) compilererrors = new List<CompilerError>();
				if(errors != null)
				{
					// Combine 2 error lists...
					foreach(CompilerError err in errors)
					{
						bool alreadyadded = false;
						foreach(CompilerError compilererror in compilererrors)
						{
							if(compilererror.Equals(err))
							{
								alreadyadded = true;
								break;
							}
						}

						if(!alreadyadded) compilererrors.Add(err);
					}
				}
			}
			else
			{
				compilererrors = (errors != null ? new List<CompilerError>(errors) : new List<CompilerError>());
			}
			
			// Fill list
			errorlist.BeginUpdate();
			errorlist.Items.Clear();
			int listindex = 1;
			foreach(CompilerError e in compilererrors)
			{
				ListViewItem ei = new ListViewItem(listindex.ToString());
				ei.ImageIndex = 0;
				ei.SubItems.Add(e.description);
				string filename = (e.filename.StartsWith("?") ? e.filename.Replace("?", "") : Path.GetFileName(e.filename)); //mxd
				string linenumber = (e.linenumber != CompilerError.NO_LINE_NUMBER ? " (line " + (e.linenumber + 1) + ")" : String.Empty); //mxd
				ei.SubItems.Add(filename + linenumber);
				ei.Tag = e;
				errorlist.Items.Add(ei);
				listindex++;
			}
			errorlist.EndUpdate();
			
			// Show marks on scripts
			foreach(ScriptDocumentTab t in tabs.TabPages)
			{
				t.MarkScriptErrors(compilererrors);
			}
			
			//mxd. Show/hide panel
			if(errorlist.Items.Count > 0)
			{
				infotabs.SelectedTab = taberrors;
				if(scriptsplitter.IsCollapsed) scriptsplitter.IsCollapsed = false; // biwa. Only toggle if it's not shown
			}
			else if(infotabs.SelectedTab == taberrors)
			{
				if(!scriptsplitter.IsCollapsed) scriptsplitter.IsCollapsed = true; // biwa. Only toggle if it's shown
			}
		}

		//mxd
		internal void ShowError(TextResourceErrorItem item)
		{
			// Resource exists?
			DataReader dr = null;

			// Resource is a temporary file?
			if(item.ResourceLocation.StartsWith(General.Map.TempPath))
			{
				// Search in PK3-embedded wads only
				foreach(DataReader reader in General.Map.Data.Containers)
				{
					if(!(reader is PK3Reader)) continue;
					PK3Reader pkr = (PK3Reader)reader;
					foreach(WADReader wadreader in pkr.Wads)
					{
						if(wadreader.Location.location == item.ResourceLocation)
						{
							dr = wadreader;
							break;
						}
					}
				}
			}
			else
			{
				// Search among resources
				foreach(DataReader reader in General.Map.Data.Containers)
				{
					if(reader.Location.location == item.ResourceLocation)
					{
						dr = reader;
						break;
					}
				}
			}

			// Target lump exists?
			if(dr == null || !dr.FileExists(item.LumpName, item.LumpIndex)) return;

			TextResourceData trd = new TextResourceData(dr, dr.LoadFile(item.LumpName, item.LumpIndex), item.LumpName, item.LumpIndex, false);
			var targettab = OpenResource(new ScriptResource(trd, item.ScriptType));

			if(targettab != null)
			{
				// Go to error line
				if(item.LineNumber != CompilerError.NO_LINE_NUMBER) targettab.MoveToLine(item.LineNumber);
			}
		}

		//mxd
		/*internal void ShowError(TextFileErrorItem item)
		{
			// File exists?
			ScriptDocumentTab targettab = OpenFile(item.Filename, item.ScriptType);

			// Go to error line
			if(targettab != null && item.LineNumber != CompilerError.NO_LINE_NUMBER)
				targettab.MoveToLine(item.LineNumber);
		}*/

		// This writes all explicitly opened files to the configuration
		public void WriteOpenFilesToConfiguration()
		{
			General.Map.Options.ScriptDocumentSettings.Clear(); //mxd
			foreach(ScriptDocumentTab t in tabs.TabPages) //mxd
			{
				if(t is ScriptFileDocumentTab)
				{
					// Don't store tabs, which were never saved (this only happens when a new tab was created and no text 
					// was entered into it before closing the script editor)
					if(t.ExplicitSave && !t.IsSaveAsRequired)
					{
						var settings = t.GetViewSettings();
						General.Map.Options.ScriptDocumentSettings[settings.Filename] = settings;
					}
				}
				else if(t is ScriptLumpDocumentTab || t is ScriptResourceDocumentTab)
				{
					var settings = t.GetViewSettings();
					General.Map.Options.ScriptDocumentSettings[settings.Filename] = settings;
				}
				else
				{
					throw new NotImplementedException("Unknown ScriptDocumentTab type");
				}
			}
		}
		
		// This asks to save files and returns the result
		public bool AskSaveAll()
		{
			foreach(ScriptDocumentTab t in tabs.TabPages)
			{
				if(t.ExplicitSave)
				{
					if(!CloseScript(t, true)) return false;
				}
			}
			
			return true;
		}
		
		// This closes a script and returns true when closed
		private bool CloseScript(ScriptDocumentTab t, bool saveonly)
		{
			if(t.IsChanged)
			{
				// Ask to save
				DialogResult result = MessageBox.Show(this.ParentForm, "Do you want to save changes to " + t.Title + "?", "Close File", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
				switch(result)
				{
					case DialogResult.Yes:
						if(!SaveScript(t)) return false;
						break;
					case DialogResult.Cancel:
						return false;
				}
			}
			
			if(!saveonly)
			{
				//mxd. Select tab to the left of the one we are going to close
				if(t == tabs.SelectedTab && tabs.SelectedIndex > 0)
					tabs.SelectedIndex--;
				
				// Close file
				tabs.TabPages.Remove(t);
				t.Dispose();
			}
			return true;
		}
		
		// This returns true when any of the implicit-save scripts are changed
		public bool CheckImplicitChanges()
		{
			foreach(ScriptDocumentTab t in tabs.TabPages)
			{
				if(!t.ExplicitSave && t.IsChanged) return true;
			}
			return false;
		}
		
		// This forces the focus to the script editor
		public void ForceFocus()
		{
			ScriptDocumentTab t = (tabs.SelectedTab as ScriptDocumentTab);
			tabs.Focus();
			if(t != null) t.Focus();
		}
		
		// This does an implicit save on all documents that use implicit saving
		// Call this to save the lumps before disposing the panel!
		public void ImplicitSave()
		{
			// Save all scripts
			foreach(ScriptDocumentTab t in tabs.TabPages)
			{
				if(!t.ExplicitSave) t.Save();
			}
			
			UpdateInterface(false);
		}
		
		// This updates the toolbar for the current status
		private void UpdateInterface(bool focuseditor)
		{
			menustrip.Enabled = false;
			menustrip.Enabled = true;

			int numscriptsopen = tabs.TabPages.Count;
			int explicitsavescripts = 0;
			ScriptDocumentTab t = null;
			
			// Any explicit save scripts?
			foreach(ScriptDocumentTab dt in tabs.TabPages)
				if(dt.ExplicitSave && !dt.IsReadOnly) explicitsavescripts++;
			
			// Get current script, if any are open
			if(numscriptsopen > 0) t = (tabs.SelectedTab as ScriptDocumentTab);
			
			// Enable/disable buttons
			bool tabiseditable = (t != null && !t.IsReadOnly); //mxd

			buttonsave.Enabled = (tabiseditable && t.ExplicitSave && t.IsChanged);
			buttonsaveall.Enabled = (explicitsavescripts > 0);
			buttoncompile.Enabled = (tabiseditable && t.Config.Compiler != null);
			buttonsearch.Enabled = (t != null); //mxd
			buttonkeywordhelp.Enabled = (t != null && !string.IsNullOrEmpty(t.Config.KeywordHelp));
			buttonscriptconfig.Enabled = (tabiseditable && t.IsReconfigurable);
			
			// Undo/Redo
			buttonundo.Enabled = (tabiseditable && t.Editor.Scintilla.CanUndo);
			buttonredo.Enabled = (tabiseditable && t.Editor.Scintilla.CanRedo);

			// Cut/Copy/Paste
			buttoncopy.Enabled = (t != null && t.Editor.Scintilla.SelectionStart < t.Editor.Scintilla.SelectionEnd);
			buttoncut.Enabled = (tabiseditable && t.Editor.Scintilla.SelectionStart < t.Editor.Scintilla.SelectionEnd);
			buttonpaste.Enabled = (tabiseditable && t.Editor.Scintilla.CanPaste);
			
			//mxd. Snippets
			buttonsnippets.DropDownItems.Clear();
			menusnippets.DropDownItems.Clear();

			bool havesnippets = (tabiseditable && t.Config.Snippets.Count > 0);
			buttonsnippets.Enabled = havesnippets;
			menusnippets.Enabled = havesnippets;

			//mxd. Indent/Unindent
			buttonindent.Enabled = tabiseditable;
			buttonunindent.Enabled = (tabiseditable && t.Editor.Scintilla.Lines[t.Editor.Scintilla.CurrentLine].Indentation > 0);

			//mxd. Whitespace
			buttonwhitespace.Enabled = (t != null);
			menuwhitespace.Enabled = (t != null);

			//mxd. Wordwrap
			buttonwordwrap.Enabled = (t != null);
			menuwordwrap.Enabled = (t != null);

			//mxd. Quick search options
			searchmatchcase.Enabled = (t != null);
			searchwholeword.Enabled = (t != null);
			
			if(t != null)
			{
				//mxd. Update quick search controls
				searchbox.Enabled = true;
				if(searchbox.Text.Length > 0)
				{
					if(t.Editor.Scintilla.Text.IndexOf(searchbox.Text, searchmatchcase.Checked ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase) != -1)
					{
						searchprev.Enabled = true;
						searchnext.Enabled = true;
						searchbox.BackColor = SystemColors.Window;
					}
					else
					{
						searchprev.Enabled = false;
						searchnext.Enabled = false;
						searchbox.BackColor = QUICKSEARCH_FAIL_COLOR;
					}
				}
				else
				{
					searchprev.Enabled = false;
					searchnext.Enabled = false;
				}

				// Check the according script config in menu
				foreach(ToolStripMenuItem item in buttonscriptconfig.DropDownItems)
				{
					ScriptConfiguration config = (item.Tag as ScriptConfiguration);
					item.Checked = (config == t.Config);
				}

				//mxd. Add snippets
				if(t.Config != null && t.Config.Snippets.Count > 0)
				{
					if(t.Config.Snippets.Count > 0)
					{
						foreach(string snippetname in t.Config.Snippets)
						{
							buttonsnippets.DropDownItems.Add(snippetname).Click += OnInsertSnippetClick;
							menusnippets.DropDownItems.Add(snippetname).Click += OnInsertSnippetClick;
						}
					}
				}
				
				// Focus to script editor
				if(focuseditor) ForceFocus();
			}
			else
			{
				//mxd. Disable quick search controls
				searchbox.Enabled = false; 
				searchprev.Enabled = false;
				searchnext.Enabled = false;
			}

			//mxd. Update script type description
			scripttype.Text = ((t != null && t.Config != null) ? t.Config.Description : "Plain Text");
		}

		//mxd
		internal ScriptResourceDocumentTab OpenResource(ScriptResource resource)
		{
			// Check if we already have this file opened
			foreach (var tab in tabs.TabPages)
			{
				if (!(tab is ScriptResourceDocumentTab)) continue;
				ScriptResourceDocumentTab restab = (ScriptResourceDocumentTab)tab;

				if (restab.Resource.LumpIndex == resource.LumpIndex && restab.Resource.FilePathName == resource.FilePathName)
				{
					tabs.SelectedTab = restab;
					return restab;
				}
			}

			return null;
		}

		// This saves the current open script
		public void ExplicitSaveCurrentTab()
		{
			ScriptDocumentTab t = (tabs.SelectedTab as ScriptDocumentTab);
			if((t != null))
			{
				if(t.ExplicitSave)
					buttonsave_Click(this, EventArgs.Empty);
				else if(t.Config.Compiler != null) //mxd
					buttoncompile_Click(this, EventArgs.Empty);
				else
					General.MessageBeep(MessageBeepType.Default);
			}
			else
			{
				General.MessageBeep(MessageBeepType.Default);
			}
		}
		
		//mxd. This launches keyword help website
		public bool LaunchKeywordHelp() 
		{
			// Get script
			ScriptDocumentTab t = (tabs.SelectedTab as ScriptDocumentTab);
			return (t != null && t.LaunchKeywordHelp());
		}

		//mxd. This changes status text
		internal void DisplayStatus(ScriptStatusType type, string message) { DisplayStatus(new ScriptStatusInfo(type, message)); }
		internal void DisplayStatus(ScriptStatusInfo newstatus)
		{
			// Stop timers
			if(!newstatus.displayed)
			{
				statusresetter.Stop();
				statusflasher.Stop();
				statusflashicon = false;
			}

			// Determine what to do specifically for this status type
			switch(newstatus.type)
			{
				// Shows information without flashing the icon.
				case ScriptStatusType.Ready:
				case ScriptStatusType.Info:
					if(!newstatus.displayed)
					{
						statusresetter.Interval = MainForm.INFO_RESET_DELAY;
						statusresetter.Start();
					}
					break;

				// Shows a warning, makes a warning sound and flashes a warning icon.
				case ScriptStatusType.Warning:
					if(!newstatus.displayed)
					{
						General.MessageBeep(MessageBeepType.Warning);
						statusflasher.Interval = MainForm.WARNING_FLASH_INTERVAL;
						statusflashcount = MainForm.WARNING_FLASH_COUNT;
						statusflasher.Start();
						statusresetter.Interval = MainForm.WARNING_RESET_DELAY;
						statusresetter.Start();
					}
					break;
			}

			// Update status description
			status = newstatus;
			status.displayed = true;
			statuslabel.Text = status.message;

			// Update icon as well
			UpdateStatusIcon();

			// Refresh
			statusbar.Invalidate();
			this.Update();
		}

		// This updates the status icon
		private void UpdateStatusIcon()
		{
			int statusflashindex = (statusflashicon ? 1 : 0);

			// Status type
			switch(status.type)
			{
				case ScriptStatusType.Ready:
				case ScriptStatusType.Info:
					statuslabel.Image = General.MainWindow.STATUS_IMAGES[statusflashindex, 0];
					break;

				case ScriptStatusType.Busy:
					statuslabel.Image = General.MainWindow.STATUS_IMAGES[statusflashindex, 2];
					break;

				case ScriptStatusType.Warning:
					statuslabel.Image = General.MainWindow.STATUS_IMAGES[statusflashindex, 3];
					break;

				default:
					throw new NotImplementedException("Unsupported Script Status Type!");
			}
		}

		#endregion

		#region ================== Events

		private void ScriptEditorPanel_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			if (e.KeyCode == Keys.F10)
				e.IsInputKey = true;
		}

		private void ScriptEditorPanel_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.F10)
			{
				e.SuppressKeyPress = true;
				e.Handled = true;
			}
		}

		// Called when the window that contains this panel closes
		public void OnClose()
		{
			//mxd. Store quick search settings
			matchcase = searchmatchcase.Checked;
			matchwholeword = searchwholeword.Checked;

			//mxd. Stop status timers
			statusresetter.Stop();
			statusflasher.Stop();

			// Close the sub windows now
			if(findreplaceform != null) findreplaceform.Dispose();
		}

		// Keyword help requested
		private void buttonkeywordhelp_Click(object sender, EventArgs e)
		{
			LaunchKeywordHelp();
		}

		// When the user changes the script configuration
		private void buttonscriptconfig_Click(object sender, EventArgs e)
		{
			// Get the tab and new script config
			ScriptDocumentTab t = (tabs.SelectedTab as ScriptDocumentTab);
			ScriptConfiguration scriptconfig = ((sender as ToolStripMenuItem).Tag as ScriptConfiguration);
			
			// Change script config
			t.ChangeScriptConfig(scriptconfig);

			//mxd. Update script type description
			scripttype.Text = scriptconfig.Description;

			// Done
			UpdateInterface(true);
		}
		
		// Save script clicked
		private void buttonsave_Click(object sender, EventArgs e)
		{
			// Save the current script
			ScriptDocumentTab t = (tabs.SelectedTab as ScriptDocumentTab);
			SaveScript(t);
			UpdateInterface(true);
		}

		// Save All clicked
		private void buttonsaveall_Click(object sender, EventArgs e)
		{
			// Save all scripts
			foreach(ScriptDocumentTab t in tabs.TabPages)
			{
				// Use explicit save for this script?
				if(t.ExplicitSave)
				{
					if(!SaveScript(t)) break;
				}
			}
			
			UpdateInterface(true);
		}

		// This is called by Save and Save All to save a script
		// Returns false when cancelled by the user
		private bool SaveScript(ScriptDocumentTab t)
		{
			// Do we have to do a save as?
			if(t.IsSaveAsRequired)
			{
				// Setup save dialog
				string scriptfilter = t.Config.Description + "|*." + string.Join(";*.", t.Config.Extensions);
				savefile.Filter = scriptfilter + "|All files|*.*";
				if(savefile.ShowDialog(this.ParentForm) == DialogResult.OK)
				{
					// Save to new filename
					t.SaveAs(savefile.FileName);

					//mxd. Also compile if needed
					if(t.Config.Compiler != null) t.Compile();

					return true;
				}
				
				// Cancelled
				return false;
			}

			// Save to same filename
			t.Save();

			return true;
		}
		
		// A tab is selected
		private void tabs_Selecting(object sender, TabControlCancelEventArgs e)
		{
			//mxd. Update script navigator
			ScriptDocumentTab tab = e.TabPage as ScriptDocumentTab;
			if(tab != null)
			{
				// Show all errors...
				ShowErrors(tab.UpdateNavigator(), true);
			}

			UpdateInterface(true);
		}
		
		// Compile Script clicked
		private void buttoncompile_Click(object sender, EventArgs e)
		{
			// First save all implicit scripts to the temporary wad file
			ImplicitSave();
			
			// Get script
			ScriptDocumentTab t = (tabs.SelectedTab as ScriptDocumentTab);

			// Check if it must be saved as a new file
			if(t.ExplicitSave && t.IsSaveAsRequired)
			{
				// Save the script first!
				if(MessageBox.Show(this.ParentForm, "You must save your script before you can compile it. Do you want to save your script now?", "Compile Script", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
				{
					if(!SaveScript(t)) return;
				}
				else
				{
					return;
				}
			}
			else if(t.ExplicitSave && t.IsChanged)
			{
				// We can only compile when the script is saved
				if(!SaveScript(t)) return;
			}

			// Compile now
			DisplayStatus(ScriptStatusType.Busy, "Compiling script \"" + t.Title + "\"...");
			Cursor.Current = Cursors.WaitCursor;
			t.Compile();

			// Show warning
			if((compilererrors != null) && (compilererrors.Count > 0))
				DisplayStatus(ScriptStatusType.Warning, compilererrors.Count + " errors while compiling \"" + t.Title + "\"!");
			else
				DisplayStatus(ScriptStatusType.Info, "Script \"" + t.Title + "\" compiled without errors.");

			Cursor.Current = Cursors.Default;
			UpdateInterface(true);
		}
		
		// Undo clicked
		private void buttonundo_Click(object sender, EventArgs e)
		{
			//mxd. Special cases...
			if(searchbox.Focused)
			{
				searchbox.Undo();
				return;
			}
			
			ScriptDocumentTab t = (tabs.SelectedTab as ScriptDocumentTab);
			t.Undo();
			UpdateInterface(true);
		}
		
		// Redo clicked
		private void buttonredo_Click(object sender, EventArgs e)
		{
			//mxd. Special cases...
			if(searchbox.Focused)
			{
				searchbox.Undo();
				return;
			}
			
			ScriptDocumentTab t = (tabs.SelectedTab as ScriptDocumentTab);
			t.Redo();
			UpdateInterface(true);
		}
		
		// Cut clicked
		private void buttoncut_Click(object sender, EventArgs e)
		{
			//mxd. Special cases...
			if(searchbox.Focused)
			{
				if(searchbox.TextBox != null) searchbox.TextBox.Cut();
				return;
			}

			ScriptDocumentTab t = (tabs.SelectedTab as ScriptDocumentTab);
			t.Cut();
			UpdateInterface(true);
		}
		
		// Copy clicked
		private void buttoncopy_Click(object sender, EventArgs e)
		{
			//mxd. Special cases...
			if(searchbox.Focused)
			{
				searchbox.Copy();
				return;
			}

			ScriptDocumentTab t = (tabs.SelectedTab as ScriptDocumentTab);
			t.Copy();
			UpdateInterface(true);
		}

		// Paste clicked
		private void buttonpaste_Click(object sender, EventArgs e)
		{
			//mxd. Special cases...
			if(searchbox.Focused)
			{
				searchbox.Paste();
				return; 
			}

			ScriptDocumentTab t = (tabs.SelectedTab as ScriptDocumentTab);
			t.Paste();
			UpdateInterface(true);
		}

		//mxd
		private void buttonunindent_Click(object sender, EventArgs e)
		{
			ScriptDocumentTab t = (tabs.SelectedTab as ScriptDocumentTab);
			t.IndentSelection(false);
		}

		//mxd
		private void buttonindent_Click(object sender, EventArgs e)
		{
			ScriptDocumentTab t = (tabs.SelectedTab as ScriptDocumentTab);
			t.IndentSelection(true);
		}

		//mxd
		private void buttonwhitespace_Click(object sender, EventArgs e)
		{
			if(blockupdate) return;

			blockupdate = true;
			showwhitespace = !showwhitespace;
			buttonwhitespace.Checked = showwhitespace;
			menuwhitespace.Checked = showwhitespace;
			blockupdate = false;
			
			ApplyTabSettings();
		}

		//mxd
		private void buttonwordwrap_Click(object sender, EventArgs e)
		{
			if(blockupdate) return;

			blockupdate = true;
			wraplonglines = !wraplonglines;
			buttonwordwrap.Checked = wraplonglines;
			menuwordwrap.Checked = wraplonglines;
			blockupdate = false;
			
			ApplyTabSettings();
		}

		//mxd. Search clicked
		private void buttonsearch_Click(object sender, EventArgs e) 
		{
			OpenFindAndReplace();
		}

		//mxd
		private void menugotoline_Click(object sender, EventArgs e)
		{
			GoToLine();
		}

		//mxd
		private void menuduplicateline_Click(object sender, EventArgs e)
		{
			if(ActiveTab != null) ActiveTab.Editor.DuplicateLine();
		}

		//mxd
		private void menustayontop_Click(object sender, EventArgs e)
		{
			General.Settings.ScriptOnTop = menustayontop.Checked;
			parentform.TopMost = General.Settings.ScriptOnTop;
		}

		//mxd
		private void OnInsertSnippetClick(object sender, EventArgs eventArgs) 
		{
			ScriptDocumentTab t = (tabs.SelectedTab as ScriptDocumentTab);
			t.InsertSnippet( ((ToolStripItem)sender).Text );
		}
		
		// Mouse released on tabs
		private void tabs_MouseUp(object sender, MouseEventArgs e)
		{
			ForceFocus();
		}

		//mxd
		private void tabs_OnCloseTabClicked(object sender, TabControlEventArgs e)
		{
			ScriptDocumentTab t = (e.TabPage as ScriptDocumentTab);
			
			//TODO: allow any tab to be closed.
			if(!t.IsClosable) return;
			
			CloseScript(t, false);
			UpdateInterface(true);
		}

		//mxd. Text in ScriptFileDocumentTab was changed
		private void tabpage_OnTextChanged(object sender, EventArgs eventArgs)
		{
			if(tabs.SelectedTab != null)
			{
				ScriptDocumentTab curtab = tabs.SelectedTab as ScriptDocumentTab;
				if(curtab != null)
				{
					buttonsave.Enabled = (curtab.ExplicitSave && curtab.IsChanged);
					buttonundo.Enabled = curtab.Editor.Scintilla.CanUndo;
					buttonredo.Enabled = curtab.Editor.Scintilla.CanRedo;
				}
			}
		}

		//mxd. Text in ScriptLumpDocumentTab was changed
		private void tabpage_OnLumpTextChanged(object sender, EventArgs e)
		{
			if(tabs.SelectedTab != null)
			{
				ScriptDocumentTab curtab = tabs.SelectedTab as ScriptDocumentTab;
				if(curtab != null)
				{
					buttonundo.Enabled = curtab.Editor.Scintilla.CanUndo;
					buttonredo.Enabled = curtab.Editor.Scintilla.CanRedo;
				}
			}
		}

		//mxd
		private void scintilla_OnUpdateUI(object sender, UpdateUIEventArgs e)
		{
			Scintilla s = sender as Scintilla;
			if(s != null)
			{
				// Update caret position info [line] : [caret pos start] OR [caret pos start x selection length] ([total lines])
				positionlabel.Text = (s.CurrentLine + 1) + " : " 
					+ (s.SelectionStart + 1 - s.Lines[s.LineFromPosition(s.SelectionStart)].Position) 
					+ (s.SelectionStart != s.SelectionEnd ? "x" + (s.SelectionEnd - s.SelectionStart) : "") 
					+ " (" + s.Lines.Count + ")";

				// Update copy-paste buttons
				buttoncut.Enabled = (s.SelectionEnd > s.SelectionStart);
				buttoncopy.Enabled = (s.SelectionEnd > s.SelectionStart);
				buttonpaste.Enabled = s.CanPaste;
				buttonunindent.Enabled = s.Lines[s.CurrentLine].Indentation > 0;
			}
		}
		
		#endregion

		#region ================== Quick Search (mxd)

		private FindReplaceOptions GetQuickSearchOptions()
		{
			return new FindReplaceOptions 
			{
				CaseSensitive = searchmatchcase.Checked,
				WholeWord = searchwholeword.Checked,
				FindText = searchbox.Text
			};
		}

		private void searchbox_TextChanged(object sender, EventArgs e)
		{
			bool success = (searchbox.Text.Length > 0 && ActiveTab.FindNext(GetQuickSearchOptions(), true));
			searchbox.BackColor = ((success || searchbox.Text.Length == 0) ? SystemColors.Window : QUICKSEARCH_FAIL_COLOR);
			searchnext.Enabled = success;
			searchprev.Enabled = success;
		}

		private void searchnext_Click(object sender, EventArgs e)
		{
			if(!ActiveTab.FindNext(GetQuickSearchOptions(), false)) General.MessageBeep(MessageBeepType.Default);
		}

		private void searchprev_Click(object sender, EventArgs e) 
		{
			if(!ActiveTab.FindPrevious(GetQuickSearchOptions())) General.MessageBeep(MessageBeepType.Default);
		}

		// This flashes the status icon
		private void statusflasher_Tick(object sender, EventArgs e)
		{
			statusflashicon = !statusflashicon;
			UpdateStatusIcon();
			statusflashcount--;
			if(statusflashcount == 0) statusflasher.Stop();
		}

		// This resets the status to ready
		private void statusresetter_Tick(object sender, EventArgs e)
		{
			DisplayStatus(ScriptStatusType.Ready, null);
		}

		#endregion

		#region ================== Menu opening events (mxd)

		private void filemenuitem_DropDownOpening(object sender, EventArgs e)
		{
			ScriptDocumentTab t = ActiveTab;
			menusave.Enabled = (t != null && !t.IsReadOnly && t.ExplicitSave && t.IsChanged);

			// Any explicit save scripts?
			int explicitsavescripts = 0;
			foreach(ScriptDocumentTab dt in tabs.TabPages)
				if(dt.ExplicitSave && !dt.IsReadOnly) explicitsavescripts++;

			menusaveall.Enabled = (explicitsavescripts > 0);
		}

		private void editmenuitem_DropDownOpening(object sender, EventArgs e)
		{
			ScriptDocumentTab t = ActiveTab;
			if(t != null)
			{
				Scintilla s = t.Editor.Scintilla;
				
				menuundo.Enabled = s.CanUndo;
				menuredo.Enabled = s.CanRedo;

				menucut.Enabled = (s.SelectionEnd > s.SelectionStart);
				menucopy.Enabled = (s.SelectionEnd > s.SelectionStart);
				menupaste.Enabled = s.CanPaste;

				menuindent.Enabled = true;
				menuunindent.Enabled = s.Lines[s.CurrentLine].Indentation > 0;

				menugotoline.Enabled = true;
				menuduplicateline.Enabled = true;
			}
			else
			{
				menuundo.Enabled = false;
				menuredo.Enabled = false;

				menucut.Enabled = false;
				menucopy.Enabled = false;
				menupaste.Enabled = false;

				menusnippets.Enabled = false;

				menuindent.Enabled = false;
				menuunindent.Enabled = false;

				menugotoline.Enabled = false;
				menuduplicateline.Enabled = false;
			}
		}

		private void searchmenuitem_DropDownOpening(object sender, EventArgs e)
		{
			ScriptDocumentTab t = ActiveTab;
			menufind.Enabled = (t != null);

			bool enable = (!string.IsNullOrEmpty(findoptions.FindText) && t != null);
			menufindnext.Enabled = enable;
			menufindprevious.Enabled = enable;
		}

		private void toolsmenu_DropDownOpening(object sender, EventArgs e)
		{
			ScriptDocumentTab t = ActiveTab;
			menucompile.Enabled = (ActiveTab != null && !t.IsReadOnly && t.Config.Compiler != null);
		}

		#endregion

		private void ScriptEditorPanel_Load(object sender, EventArgs e)
		{
			// biwa. The designer is setting the properties in the wrong order, which
			// results in them not being set correctly at all. Set them here manually
			// scriptsplitter.SplitterDistance = 100;
			scriptsplitter.Panel1MinSize = 100;
			scriptsplitter.Panel2MinSize = 100;
			scriptsplitter.SetSizes();
		}
	}
}
