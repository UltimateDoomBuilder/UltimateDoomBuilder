
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
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Controls;

#endregion

namespace CodeImp.DoomBuilder.Windows
{
	public partial class ScriptFindReplaceForm : DelayedForm
	{
		#region ================== Constants

		private const int MAX_DROPDOWN_ITEMS = 20;

		#endregion

		#region ================== Variables

		private bool appclose;
		private bool canreplace;

		#endregion

		#region ================== Properties

		internal bool CanReplace //mxd
		{
			get { return canreplace; }
			set
			{
				canreplace = value;

				if(!canreplace)
				{
					if(tabs.TabPages.Contains(tabreplace))
						tabs.TabPages.Remove(tabreplace);
				}
				else if(!tabs.TabPages.Contains(tabreplace))
				{
					tabs.TabPages.Add(tabreplace);
				}
			}
		} 
		
		#endregion

		#region ================== Constructor

		// Constructor
		public ScriptFindReplaceForm()
		{
			InitializeComponent();
			LoadSettings(); //mxd
		}

		#endregion

		#region ================== Methods

		// This makes the Find & Replace options
		private FindReplaceOptions MakeOptions()
		{
			FindReplaceOptions options = new FindReplaceOptions();

			if(tabs.SelectedTab == tabfind)
			{
				options.FindText = findbox.Text;
				options.CaseSensitive = findmatchcase.Checked;
				options.WholeWord = findwholeword.Checked;
				options.ReplaceWith = null;
				options.SearchMode = (FindReplaceSearchMode)findinbox.SelectedIndex;
			}
			else if(tabs.SelectedTab == tabreplace)
			{
				options.FindText = replacefindbox.Text;
				options.CaseSensitive = replacematchcase.Checked;
				options.WholeWord = replacewholeword.Checked;
				options.ReplaceWith = replacebox.Text;
				options.SearchMode = (FindReplaceSearchMode)replaceinbox.SelectedIndex;
			}
			else
			{
				throw new NotImplementedException("Unsupported tab type");
			}
			
			return options;
		}

		// Close the window
		new public void Close()
		{
			appclose = true;
			base.Close();
		}

		// This sets the text to find
		public void SetFindText(string text)
		{
			ComboBox target; //mxd
			
			if(tabs.SelectedTab == tabfind)
				target = findbox;
			else if(tabs.SelectedTab == tabreplace)
				target = replacefindbox;
			else
				throw new NotImplementedException("Unsupported tab type");

			target.Text = text;
			target.SelectAll();

			//mxd. Add to combobox
			AddComboboxText(target, text);
		}

		//mxd
		private static void AddComboboxText(ComboBox target, string text)
		{
			if(!string.IsNullOrEmpty(text) && !target.Items.Contains(text))
			{
				target.Items.Insert(0, text);
				while(target.Items.Count > MAX_DROPDOWN_ITEMS)
				{
					target.Items.RemoveAt(target.Items.Count - 1);
				}
			}
		}

		//mxd
		private void LoadSettings()
		{
			// Load generic search settings
			bool matchcase = General.Settings.ReadSetting("windows." + configname + ".matchcase", false);
			bool matchwholeword = General.Settings.ReadSetting("windows." + configname + ".matchwholeword", false);
			int searchmode = General.Settings.ReadSetting("windows." + configname + ".searchmode", (int)FindReplaceSearchMode.CURRENT_FILE);

			// Load find settings
			string findtext = General.Settings.ReadSetting("windows." + configname + ".findtext", string.Empty);
			List<string> findtexts = new List<string>();
			IDictionary findtextdic = General.Settings.ReadSetting("windows." + configname + ".findtexts", new Hashtable());
			foreach(DictionaryEntry cde in findtextdic)
				findtexts.Add(cde.Value.ToString());

			// Load replace settings
			string replacetext = General.Settings.ReadSetting("windows." + configname + ".replacetext", string.Empty);
			List<string> replacetexts = new List<string>();
			IDictionary replacetextdic = General.Settings.ReadSetting("windows." + configname + ".replacetexts", new Hashtable());
			foreach(DictionaryEntry cde in replacetextdic)
				replacetexts.Add(cde.Value.ToString());

			// Apply find settings...
			findbox.MaxDropDownItems = MAX_DROPDOWN_ITEMS;
			findbox.Text = findtext;
			findbox.SelectAll();
			findbox.Items.AddRange(findtexts.ToArray());
			findinbox.SelectedIndex = searchmode > findinbox.Items.Count - 1 ? 0 : searchmode;
			findmatchcase.Checked = matchcase;
			findwholeword.Checked = matchwholeword;

			// Apply replace settings...
			replacefindbox.MaxDropDownItems = MAX_DROPDOWN_ITEMS;
			replacefindbox.Text = findtext;
			replacefindbox.SelectAll();
			replacefindbox.Items.AddRange(findtexts.ToArray());
			replacebox.MaxDropDownItems = MAX_DROPDOWN_ITEMS;
			replacebox.Text = replacetext;
			replacebox.Items.AddRange(replacetexts.ToArray());
			replaceinbox.SelectedIndex = searchmode > replaceinbox.Items.Count - 1 ? 0 : searchmode;
			replacematchcase.Checked = matchcase;
			replacewholeword.Checked = matchwholeword;

			// Set selected tab
			tabs.SelectedIndex = General.Clamp(General.Settings.ReadSetting("windows." + configname + ".selectedtab", 0), 0, tabs.TabCount);
		}

		//mxd
		private void SaveSettings()
		{
			// Save generic search settings
			General.Settings.WriteSetting("windows." + configname + ".matchcase", findmatchcase.Checked);
			General.Settings.WriteSetting("windows." + configname + ".matchwholeword", findwholeword.Checked);
			General.Settings.WriteSetting("windows." + configname + ".searchmode", findinbox.SelectedIndex);
			General.Settings.WriteSetting("windows." + configname + ".selectedtab", tabs.SelectedIndex);

			// Save find settings
			General.Settings.WriteSetting("windows." + configname + ".findtext", findbox.Text);
			ListDictionary finddata = new ListDictionary();
			for(int i = 0; i < findbox.Items.Count; i++)
			{
				finddata.Add("findtext" + i, findbox.Items[i].ToString());
			}
			if(finddata.Count > 0)
			{
				General.Settings.WriteSetting("windows." + configname + ".findtexts", finddata);
			}

			// Save replace settings
			General.Settings.WriteSetting("windows." + configname + ".replacetext", replacebox.Text);
			ListDictionary replacedata = new ListDictionary();
			for(int i = 0; i < replacebox.Items.Count; i++)
			{
				replacedata.Add("replacetext" + i, replacebox.Items[i].ToString());
			}
			if(replacedata.Count > 0)
			{
				General.Settings.WriteSetting("windows." + configname + ".replacetexts", replacedata);
			}
		}

		// The form doesn't have a regular "close" button, so we have to intercept the Esc key
		protected override bool ProcessDialogKey(Keys keyData)
		{
			if (ModifierKeys == Keys.None && keyData == Keys.Escape)
			{
				this.Hide();
				return true;
			}
			return base.ProcessDialogKey(keyData);
		}

		#endregion

		#region ================== Events

		// Form is closing
		private void ScriptFindReplaceForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			SaveSettings(); //mxd

			if(!appclose)
			{
				General.Map.ScriptEditor.Editor.CloseFindReplace(true);
			}
		}

		// Find Next
		private void findnextbutton_Click(object sender, EventArgs e)
		{
			FindReplaceOptions options = MakeOptions(); //mxd
			AddComboboxText(findbox, options.FindText); //mxd
			General.Map.ScriptEditor.Editor.FindNext(options);
		}

		//mxd. Find Previous
		private void findpreviousbutton_Click(object sender, EventArgs e) 
		{
			FindReplaceOptions options = MakeOptions();
			AddComboboxText(findbox, options.FindText);
			General.Map.ScriptEditor.Editor.FindPrevious(options);
		}

		//mxd. Bookmark all
		private void bookmarkallbutton_Click(object sender, EventArgs e)
		{
			FindReplaceOptions options = MakeOptions();
			AddComboboxText(findbox, options.FindText);

			// Determine script type
			ScriptType scripttype = ScriptType.UNKNOWN;
			switch(options.SearchMode)
			{
				case FindReplaceSearchMode.CURRENT_FILE:
					ScriptDocumentTab t = General.Map.ScriptEditor.Editor.ActiveTab;
					if(t != null) scripttype = t.Config.ScriptType;
					break;
			}
		}
		
		//mxd. Replace
		private void replacebutton_Click(object sender, EventArgs e)
		{
			var editor = General.Map.ScriptEditor.Editor;
			
			FindReplaceOptions options = MakeOptions();
			AddComboboxText(replacefindbox, options.FindText);
			AddComboboxText(replacebox, options.ReplaceWith);

			ScriptDocumentTab curtab = editor.ActiveTab;
			if(curtab == null) return;

			// Search from selection start, then replace
			if(!curtab.FindNext(options, true) || !editor.Replace(options))
			{
				editor.DisplayStatus(ScriptStatusType.Warning, "Can't find any occurrence of \"" + options.FindText + "\".");
				return;
			}

			// Find & show next match
			curtab.FindNext(options);
		}
		
		//mxd. Replace All
		private void replaceallbutton_Click(object sender, EventArgs e)
		{
			var editor = General.Map.ScriptEditor.Editor;

			FindReplaceOptions options = MakeOptions();
			AddComboboxText(replacefindbox, options.FindText);
			AddComboboxText(replacebox, options.ReplaceWith);

            int replacements = editor.FindReplace(options);

			// Show result
			if(replacements == 0)
			{
				editor.DisplayStatus(ScriptStatusType.Warning, "Can't find any occurrence of \"" + options.FindText + "\".");
			}
			else
			{
				editor.DisplayStatus(ScriptStatusType.Info, "Replaced " + replacements + " occurrence" + (replacements == 1 ? "" : "s") + " of \"" + options.FindText + "\" with \"" + options.ReplaceWith + "\".");

                // Find & select the last match on the now-current tab
                ScriptDocumentTab curtab = editor.ActiveTab;
				if(curtab != null)
				{
					options.FindText = options.ReplaceWith;
					options.ReplaceWith = null;

					curtab.SelectionStart = curtab.Text.Length;
					curtab.SelectionEnd = curtab.SelectionStart;
					curtab.FindPrevious(options);
				}
			}
		}

		//mxd
		private void tabs_SelectedIndexChanged(object sender, EventArgs e)
		{
			// Transfer settings...
			if(tabs.SelectedTab == tabfind)
			{
				findbox.Text = replacefindbox.Text;
				findbox.Items.Clear();
				if(replacefindbox.Items.Count > 0)
				{
					string[] items = new string[replacefindbox.Items.Count];
					replacefindbox.Items.CopyTo(items, 0);
					findbox.Items.AddRange(items);
				}
				findbox.SelectAll();
				findbox.Focus();

				findinbox.SelectedIndex = replaceinbox.SelectedIndex;

				findmatchcase.Checked = replacematchcase.Checked;
				findwholeword.Checked = replacewholeword.Checked;
			}
			else if(tabs.SelectedTab == tabreplace)
			{
				replacefindbox.Text = findbox.Text;
				replacefindbox.Items.Clear();
				if(findbox.Items.Count > 0)
				{
					string[] items = new string[findbox.Items.Count];
					findbox.Items.CopyTo(items, 0);
					replacefindbox.Items.AddRange(items);
				}
				replacefindbox.SelectAll();
				replacefindbox.Focus();

				replaceinbox.SelectedIndex = findinbox.SelectedIndex;

				replacematchcase.Checked = findmatchcase.Checked;
				replacewholeword.Checked = findwholeword.Checked;
			}
			else
			{
				throw new NotImplementedException("Unsupported tab type");
			}
		}

		//mxd. Focus text input
		private void ScriptFindReplaceForm_Shown(object sender, EventArgs e)
		{
			if(tabs.SelectedTab == tabfind) findbox.Focus();
			else if(tabs.SelectedTab == tabreplace) replacefindbox.Focus();
			else throw new NotImplementedException("Unsupported tab type");
		}

		private void findbox_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
				findnextbutton_Click(sender, EventArgs.Empty);
		}

		private void replacefindbox_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
				findnextbutton_Click(sender, EventArgs.Empty);
		}

		private void replacebox_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
				replacebutton_Click(sender, EventArgs.Empty);
		}

		#endregion
	}
}