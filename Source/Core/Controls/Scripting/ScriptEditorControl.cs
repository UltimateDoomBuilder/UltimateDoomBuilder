
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
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Compilers;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Data.Scripting;
using CodeImp.DoomBuilder.GZBuilder.Data;
using CodeImp.DoomBuilder.IO;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Properties;
using CodeImp.DoomBuilder.Windows;
using ScintillaNET;

#endregion

namespace CodeImp.DoomBuilder.Controls
{
	internal enum ScriptStyleType
	{
		PlainText = 0,
		Keyword = 1,
		Constant = 2,
		Comment = 3,
		Literal = 4,
		LineNumber = 5,
		String = 6, //mxd
		Include = 7, //mxd
		Property = 8, //mxd
	}
	
	internal partial class ScriptEditorControl : UserControl
	{
		#region ================== Enums

		// Index for registered images
		internal enum ImageIndex
		{
			ScriptConstant = 0,
			ScriptKeyword = 1,
			ScriptError = 2,
			ScriptSnippet = 3, //mxd
			ScriptProperty = 4, //mxd
		}

		#endregion

		#region ================== Constants

		private const string LEXERS_RESOURCE = "Lexers.cfg";
		private const int HIGHLIGHT_INDICATOR = 8; //mxd. Indicators 0-7 could be in use by a lexer so we'll use indicator 8 to highlight words.
		private const string ENTRY_POSITION_MARKER = "[EP]"; //mxd
		private const string LINE_BREAK_MARKER = "[LB]"; //mxd

		const int SCI_ENSUREVISIBLEENFORCEPOLICY = 2234;
		const int SCI_SETBACKSPACEUNINDENTS = 2262;
		const int SCI_SETTABINDENTS = 2260;

		#endregion

		#region ================== Delegates / Events

		public delegate void ExplicitSaveTabDelegate();
		public delegate void OpenScriptBrowserDelegate();
		public delegate void OpenFindReplaceDelegate();
		public delegate bool FindNextDelegate();
		public delegate bool FindPreviousDelegate(); //mxd
		public delegate bool FindNextWrapAroundDelegate(FindReplaceOptions options); //mxd
		public delegate bool FindPreviousWrapAroundDelegate(FindReplaceOptions options); //mxd
		public delegate void GoToLineDelegate(); //mxd
		public delegate void CompileScriptDelegate(); //mxd

		public event ExplicitSaveTabDelegate OnExplicitSaveTab;
		public event OpenScriptBrowserDelegate OnOpenScriptBrowser;
		public event OpenFindReplaceDelegate OnOpenFindAndReplace;
		public event FindNextDelegate OnFindNext;
		public event FindPreviousDelegate OnFindPrevious; //mxd
		public event FindNextWrapAroundDelegate OnFindNextWrapAround; //mxd
		public event FindPreviousWrapAroundDelegate OnFindPreviousWrapAround; //mxd
		public new event EventHandler OnTextChanged; //mxd
		public event EventHandler OnFunctionBarDropDown; //mxd
		public event GoToLineDelegate OnGoToLine; //mxd
		public event CompileScriptDelegate OnCompileScript; //mxd

		#endregion

		#region ================== Variables
		
		// Script configuration
		private ScriptConfiguration scriptconfig;

		//mxd. Handles script type-specific stuff
		private ScriptHandler handler;

		// Style translation from Scintilla style to ScriptStyleType
		private Dictionary<int, ScriptStyleType> stylelookup;
		
		// Current position information
		private int linenumbercharlength; //mxd. Current max number of chars in the line number
		private int lastcaretpos; //mxd. Used in brace matching
		private int caretoffset; //mxd. Used to modify caret position after autogenerating stuff
		private bool expandcodeblock; //mxd. More gross hacks
		private string highlightedword; //mxd
		private static Encoding encoding = Encoding.GetEncoding(1251); //mxd. ASCII with cyrillic support...

		//mxd. Event propagation
		private bool preventchanges;
		
		#endregion

		#region ================== Properties

		public bool IsChanged { get { return scriptedit.Modified; } }
		public int SelectionStart { get { return scriptedit.SelectionStart; } set { scriptedit.SelectionStart = value; } }
		public int SelectionEnd { get { return scriptedit.SelectionEnd; } set { scriptedit.SelectionEnd = value; } }
		public new string Text { get { return scriptedit.Text; } set { scriptedit.Text = value; } } //mxd
		public string SelectedText { get { return scriptedit.SelectedText; } } //mxd
		public bool ShowWhitespace { get { return scriptedit.ViewWhitespace != WhitespaceMode.Invisible; } set { scriptedit.ViewWhitespace = value ? WhitespaceMode.VisibleAlways : WhitespaceMode.Invisible; } }
		public bool WrapLongLines { get { return scriptedit.WrapMode != WrapMode.None; } set { scriptedit.WrapMode = (value ? WrapMode.Char : WrapMode.None); } }
		internal Scintilla Scintilla { get { return scriptedit; } } //mxd
		internal static Encoding Encoding { get { return encoding; } } //mxd

		#endregion

		#region ================== Contructor / Disposer

		// Constructor
		public ScriptEditorControl()
		{
			// Initialize
			InitializeComponent();

			// Script editor properties
			//TODO: use ScintillaNET properties instead when they become available
			scriptedit.DirectMessage(SCI_SETBACKSPACEUNINDENTS, new IntPtr(1), IntPtr.Zero);
			//scriptedit.DirectMessage(SCI_SETMOUSEDOWNCAPTURES, new IntPtr(1), IntPtr.Zero);
			scriptedit.DirectMessage(SCI_SETTABINDENTS, new IntPtr(1), IntPtr.Zero);

			// Symbol margin
			scriptedit.Margins[0].Type = MarginType.Symbol;
			scriptedit.Margins[0].Width = 20;
			scriptedit.Margins[0].Mask = 1 << (int)ImageIndex.ScriptError; // Error marker only
			scriptedit.Margins[0].Cursor = MarginCursor.Arrow;
			scriptedit.Margins[0].Sensitive = true;
			
			// Line numbers margin
			if(General.Settings.ScriptShowLineNumbers)
			{
				scriptedit.Margins[1].Type = MarginType.Number;
				scriptedit.Margins[1].Width = 16;
			}
			scriptedit.Margins[1].Mask = 0; // No markers here

			// Spacing margin
			scriptedit.Margins[2].Type = MarginType.Symbol;
			scriptedit.Margins[2].Width = 5;
			scriptedit.Margins[2].Cursor = MarginCursor.Arrow;
			scriptedit.Margins[2].Mask = 0; // No markers here

			// Images
			RegisterAutoCompleteImage(ImageIndex.ScriptConstant, Resources.ScriptConstant);
			RegisterAutoCompleteImage(ImageIndex.ScriptKeyword, Resources.ScriptKeyword);
			RegisterAutoCompleteImage(ImageIndex.ScriptSnippet, Resources.ScriptSnippet); //mxd
			RegisterAutoCompleteImage(ImageIndex.ScriptProperty, Resources.ScriptProperty); //mxd
			RegisterMarkerImage(ImageIndex.ScriptError, Resources.ScriptError);

			// These key combinations put odd characters in the script. Let's disable them
			scriptedit.AssignCmdKey(Keys.Control | Keys.Q, Command.Null);
			scriptedit.AssignCmdKey(Keys.Control | Keys.W, Command.Null);
			scriptedit.AssignCmdKey(Keys.Control | Keys.E, Command.Null);
			scriptedit.AssignCmdKey(Keys.Control | Keys.R, Command.Null);
			scriptedit.AssignCmdKey(Keys.Control | Keys.I, Command.Null);
			scriptedit.AssignCmdKey(Keys.Control | Keys.P, Command.Null);
			scriptedit.AssignCmdKey(Keys.Control | Keys.G, Command.Null);
			scriptedit.AssignCmdKey(Keys.Control | Keys.H, Command.Null);
			scriptedit.AssignCmdKey(Keys.Control | Keys.K, Command.Null);
			scriptedit.AssignCmdKey(Keys.Control | Keys.B, Command.Null);
			scriptedit.AssignCmdKey(Keys.Control | Keys.N, Command.Null);
			scriptedit.AssignCmdKey(Keys.Control | Keys.Shift | Keys.Q, Command.Null);
			scriptedit.AssignCmdKey(Keys.Control | Keys.Shift | Keys.W, Command.Null);
			scriptedit.AssignCmdKey(Keys.Control | Keys.Shift | Keys.E, Command.Null);
			scriptedit.AssignCmdKey(Keys.Control | Keys.Shift | Keys.R, Command.Null);
			scriptedit.AssignCmdKey(Keys.Control | Keys.Shift | Keys.Y, Command.Null);
			scriptedit.AssignCmdKey(Keys.Control | Keys.Shift | Keys.O, Command.Null);
			scriptedit.AssignCmdKey(Keys.Control | Keys.Shift | Keys.P, Command.Null);
			scriptedit.AssignCmdKey(Keys.Control | Keys.Shift | Keys.A, Command.Null);
			scriptedit.AssignCmdKey(Keys.Control | Keys.Shift | Keys.S, Command.Null);
			scriptedit.AssignCmdKey(Keys.Control | Keys.Shift | Keys.D, Command.Null);
			scriptedit.AssignCmdKey(Keys.Control | Keys.Shift | Keys.F, Command.Null);
			scriptedit.AssignCmdKey(Keys.Control | Keys.Shift | Keys.G, Command.Null);
			scriptedit.AssignCmdKey(Keys.Control | Keys.Shift | Keys.H, Command.Null);
			scriptedit.AssignCmdKey(Keys.Control | Keys.Shift | Keys.K, Command.Null);
			scriptedit.AssignCmdKey(Keys.Control | Keys.Shift | Keys.Z, Command.Null);
			scriptedit.AssignCmdKey(Keys.Control | Keys.Shift | Keys.X, Command.Null);
			scriptedit.AssignCmdKey(Keys.Control | Keys.Shift | Keys.C, Command.Null);
			scriptedit.AssignCmdKey(Keys.Control | Keys.Shift | Keys.V, Command.Null);
			scriptedit.AssignCmdKey(Keys.Control | Keys.Shift | Keys.B, Command.Null);
			scriptedit.AssignCmdKey(Keys.Control | Keys.Shift | Keys.N, Command.Null);
		}
		
		#endregion
		
		#region ================== Public methods
		
		// This launches keyword help website
		public bool LaunchKeywordHelp()
		{
			string helpsite = scriptconfig.KeywordHelp;
			string currentword = GetCurrentWord();
			if(!string.IsNullOrEmpty(currentword) && (currentword.Length > 1) && !string.IsNullOrEmpty(helpsite))
			{
				currentword = scriptconfig.GetKeywordCase(currentword);
				helpsite = helpsite.Replace("%K", currentword);
				General.OpenWebsite(helpsite);
				return true;
			}

			return !string.IsNullOrEmpty(helpsite); //mxd
		}
		
		// This replaces the selection with the given text
		public void ReplaceSelection(string replacement)
		{
			scriptedit.ReplaceSelection(replacement); //mxd TODO: encoding check/conversion?
		}
		
		// This moves the caret to a given line and ensures the line is visible
		public void MoveToLine(int linenumber)
		{
			//mxd. Safety required
			linenumber = General.Clamp(linenumber, 0, scriptedit.Lines.Count);
			
			scriptedit.Lines[linenumber].Goto();
			EnsureLineVisible(linenumber);
			scriptedit.SetEmptySelection(scriptedit.Lines[linenumber].Position);
		}

		// This makes sure a line is visible
		public void EnsureLineVisible(int linenumber)
		{
			int caretpos = scriptedit.CurrentPosition;
			
			// Determine target lines range
			int startline = Math.Max(0, linenumber - 4);
			int endline = Math.Min(scriptedit.Lines.Count, Math.Max(linenumber, linenumber + scriptedit.LinesOnScreen - 6));

			// Go to target line
			scriptedit.DirectMessage(SCI_ENSUREVISIBLEENFORCEPOLICY, (IntPtr)startline, IntPtr.Zero); // Unfold the whole text block if needed
			scriptedit.ShowLines(startline, endline);

			// We may want to do some scrolling...
			if(scriptedit.FirstVisibleLine >= startline)
				scriptedit.Lines[startline].Goto();
			else if(scriptedit.FirstVisibleLine + scriptedit.LinesOnScreen <= endline)
				scriptedit.Lines[endline].Goto();
			
			// We don't want to change caret position
			scriptedit.CurrentPosition = caretpos;
		}

		//mxd
		private void SelectAndShow(int startpos, int endpos)
		{
			// Select the result
			int startline = scriptedit.LineFromPosition(startpos);
			int endline = scriptedit.LineFromPosition(endpos);

			// Go to target line
			scriptedit.DirectMessage(SCI_ENSUREVISIBLEENFORCEPOLICY, (IntPtr)startline, IntPtr.Zero); // Unfold the whole text block if needed
			scriptedit.ShowLines(startline, endline);
			scriptedit.GotoPosition(startpos);

			// We may want to do some extra scrolling...
			if(startline > 1 && scriptedit.FirstVisibleLine >= startline - 1)
				scriptedit.Lines[startline - 1].Goto();
			else if(endline < scriptedit.Lines.Count - 1 && scriptedit.FirstVisibleLine + scriptedit.LinesOnScreen <= endline + 1)
				scriptedit.Lines[endline + 1].Goto();

			// Update selection
			scriptedit.SelectionStart = startpos;
			scriptedit.SelectionEnd = endpos;
		}

		// This returns the line for a position
		public int LineFromPosition(int position)
		{
			return scriptedit.LineFromPosition(position);
		}
		
		// This clears all marks
		public void ClearMarks()
		{
			scriptedit.MarkerDeleteAll((int)ImageIndex.ScriptError);
		}

		// This adds a mark on the given line
		public void AddMark(int linenumber)
		{
			scriptedit.Lines[linenumber].MarkerAdd((int)ImageIndex.ScriptError);
		}

		// This refreshes the style setup
		public void RefreshStyle()
		{
			// Re-setup with the same config
			SetupStyles(scriptconfig);
		}

		// This sets up the script editor with a script configuration
		public void SetupStyles(ScriptConfiguration config)
		{
			//mxd. Update script handler
			handler = General.Types.GetScriptHandler(config.ScriptType);
			handler.Initialize(this, config);

			//mxd
			functionbar.Enabled = (config.ScriptType != ScriptType.UNKNOWN);

			Configuration lexercfg = new Configuration();

			// Make collections
			stylelookup = new Dictionary<int, ScriptStyleType>();
			
			// Keep script configuration
			scriptconfig = config;
			
			// Find a resource named Lexers.cfg
			string[] resnames = General.ThisAssembly.GetManifestResourceNames();
			foreach(string rn in resnames)
			{
				// Found one?
				if(rn.EndsWith(LEXERS_RESOURCE, StringComparison.OrdinalIgnoreCase))
				{
					// Get a stream from the resource
					Stream lexersdata = General.ThisAssembly.GetManifestResourceStream(rn);
					if(lexersdata != null)
					{
						StreamReader lexersreader = new StreamReader(lexersdata, Encoding.ASCII);

						// Load configuration from stream
						lexercfg.InputConfiguration(lexersreader.ReadToEnd());

						// Done with the resource
						lexersreader.Dispose();
					}

					//mxd. We are done here
					break;
				}
			}

			//mxd. Reset document slyle
			scriptedit.ClearDocumentStyle();
			scriptedit.StyleResetDefault();
			
			// Check if specified lexer exists and set the lexer to use
			string lexername = "lexer" + (int)scriptconfig.Lexer;
			if(!lexercfg.SettingExists(lexername)) throw new InvalidOperationException("Unknown lexer " + scriptconfig.Lexer + " specified in script configuration!");
			scriptedit.Lexer = scriptconfig.Lexer;

			//mxd. Set extra word chars?
			if(!string.IsNullOrEmpty(scriptconfig.ExtraWordCharacters))
				scriptedit.WordChars += scriptconfig.ExtraWordCharacters;
			
			// Set the default style and settings
			scriptedit.Styles[Style.Default].Font = General.Settings.ScriptFontName;
			scriptedit.Styles[Style.Default].Size = General.Settings.ScriptFontSize;
			scriptedit.Styles[Style.Default].Bold = General.Settings.ScriptFontBold;
			scriptedit.Styles[Style.Default].Italic = false;
			scriptedit.Styles[Style.Default].Underline = false;
			scriptedit.Styles[Style.Default].Case = StyleCase.Mixed;
			scriptedit.Styles[Style.Default].ForeColor = General.Colors.PlainText.ToColor();
			scriptedit.Styles[Style.Default].BackColor = General.Colors.ScriptBackground.ToColor();
			scriptedit.CaretPeriod = SystemInformation.CaretBlinkTime;
			scriptedit.CaretForeColor = General.Colors.ScriptBackground.Inverse().ToColor();
			
			// Set tabulation settings
			scriptedit.UseTabs = General.Settings.ScriptUseTabs;
			scriptedit.TabWidth = General.Settings.ScriptTabWidth;
			//scriptedit.IndentWidth = General.Settings.ScriptTabWidth; // Equals to TabWidth by default
			//TODO: use ScintillaNET properties instead when they become available
			scriptedit.DirectMessage(SCI_SETTABINDENTS, new IntPtr(1), IntPtr.Zero);
			scriptedit.DirectMessage(SCI_SETBACKSPACEUNINDENTS, new IntPtr(1), IntPtr.Zero);
			
			// This applies the default style to all styles
			scriptedit.StyleClearAll();

			// Set the code page to use. [mxd] No longer needed?
			//scriptedit.CodePage = scriptconfig.CodePage;

			//mxd. We can't change Font or Size here because this will screw displayed tab width (because it's based on character width)...
			// Set the default to something normal (this is used by the autocomplete list)
			//scriptedit.Styles[Style.Default].Font = this.Font.Name;
			scriptedit.Styles[Style.Default].Bold = this.Font.Bold;
			scriptedit.Styles[Style.Default].Italic = this.Font.Italic;
			scriptedit.Styles[Style.Default].Underline = this.Font.Underline;
			//scriptedit.Styles[Style.Default].Size = (int)Math.Round(this.Font.SizeInPoints);

			// Set style for linenumbers and margins
			scriptedit.Styles[Style.LineNumber].BackColor = General.Colors.ScriptBackground.ToColor();
			scriptedit.SetFoldMarginColor(true, General.Colors.ScriptFoldBackColor.ToColor());
			scriptedit.SetFoldMarginHighlightColor(true, General.Colors.ScriptFoldBackColor.ToColor());
			for(int i = 25; i < 32; i++)
			{
				scriptedit.Markers[i].SetForeColor(General.Colors.ScriptFoldBackColor.ToColor());
				scriptedit.Markers[i].SetBackColor(General.Colors.ScriptFoldForeColor.ToColor());
			}
 
			//mxd. Set style for (mis)matching braces
			scriptedit.Styles[Style.BraceLight].BackColor = General.Colors.ScriptBraceHighlight.ToColor();
			scriptedit.Styles[Style.BraceBad].BackColor = General.Colors.ScriptBadBraceHighlight.ToColor();

			//mxd. Set whitespace color
			scriptedit.SetWhitespaceForeColor(true, General.Colors.ScriptWhitespace.ToColor());

			//mxd. Set selection colors
			scriptedit.SetSelectionForeColor(true, General.Colors.ScriptSelectionForeColor.ToColor());
			scriptedit.SetSelectionBackColor(true, General.Colors.ScriptSelectionBackColor.ToColor());

			// Clear all keywords
			for(int i = 0; i < 9; i++) scriptedit.SetKeywords(i, null);
			
			// Now go for all elements in the lexer configuration
			// We are looking for the numeric keys, because these are the
			// style index to set and the value is our ScriptStyleType
			IDictionary dic = lexercfg.ReadSetting(lexername, new Hashtable());
			foreach(DictionaryEntry de in dic)
			{
				// Check if this is a numeric key
				int stylenum;
				if(int.TryParse(de.Key.ToString(), out stylenum))
				{
					// Add style to lookup table
					stylelookup.Add(stylenum, (ScriptStyleType)(int)de.Value);
					
					// Apply color to style
					int colorindex;
					ScriptStyleType type = (ScriptStyleType)(int)de.Value;
					switch(type)
					{
						case ScriptStyleType.PlainText: colorindex = ColorCollection.PLAINTEXT; break;
						case ScriptStyleType.Comment: colorindex = ColorCollection.COMMENTS; break;
						case ScriptStyleType.Constant: colorindex = ColorCollection.CONSTANTS; break;
						case ScriptStyleType.Keyword: colorindex = ColorCollection.KEYWORDS; break;
						case ScriptStyleType.LineNumber: colorindex = ColorCollection.LINENUMBERS; break;
						case ScriptStyleType.Literal: colorindex = ColorCollection.LITERALS; break;
						case ScriptStyleType.String: colorindex = ColorCollection.STRINGS; break;
						case ScriptStyleType.Include: colorindex = ColorCollection.INCLUDES; break;
						case ScriptStyleType.Property: colorindex = ColorCollection.PROPERTIES; break;
						default: colorindex = ColorCollection.PLAINTEXT; break;
					}

					scriptedit.Styles[stylenum].ForeColor = General.Colors.Colors[colorindex].ToColor();
				}
			}

			//mxd. Set keywords
			handler.SetKeywords(lexercfg, lexername);

			// Setup folding (https://github.com/jacobslusser/ScintillaNET/wiki/Automatic-Code-Folding)
			if(General.Settings.ScriptShowFolding && (scriptconfig.Lexer == Lexer.Cpp || (int)scriptconfig.Lexer == 35)) // 35 - custom CPP case insensitive style lexer
			{
				// Instruct the lexer to calculate folding
				scriptedit.SetProperty("fold", "1");
				scriptedit.SetProperty("fold.compact", "0"); // 1 = folds blank lines
				scriptedit.SetProperty("fold.comment", "1"); // Enable block comment folding
				scriptedit.SetProperty("fold.preprocessor", "1"); // Enable #region folding
				scriptedit.SetFoldFlags(FoldFlags.LineAfterContracted); // Draw line below if not expanded

				// Configure a margin to display folding symbols
				scriptedit.Margins[2].Type = MarginType.Symbol;
				scriptedit.Margins[2].Mask = Marker.MaskFolders;
				scriptedit.Margins[2].Sensitive = true;
				scriptedit.Margins[2].Width = 12;

				// Configure folding markers with respective symbols
				scriptedit.Markers[Marker.Folder].Symbol = MarkerSymbol.BoxPlus;
				scriptedit.Markers[Marker.FolderOpen].Symbol = MarkerSymbol.BoxMinus;
				scriptedit.Markers[Marker.FolderEnd].Symbol = MarkerSymbol.BoxPlusConnected;
				scriptedit.Markers[Marker.FolderMidTail].Symbol = MarkerSymbol.TCorner;
				scriptedit.Markers[Marker.FolderOpenMid].Symbol = MarkerSymbol.BoxMinusConnected;
				scriptedit.Markers[Marker.FolderSub].Symbol = MarkerSymbol.VLine;
				scriptedit.Markers[Marker.FolderTail].Symbol = MarkerSymbol.LCorner;

				// Enable automatic folding
				scriptedit.AutomaticFold = (AutomaticFold.Show | AutomaticFold.Click | AutomaticFold.Change);
			}
			else
			{
				// Disable folding
				scriptedit.SetProperty("fold", "0");
				scriptedit.SetProperty("fold.compact", "0");

				scriptedit.Margins[2].Type = MarginType.Symbol;
				scriptedit.Margins[2].Mask = 0; // No markers here
				scriptedit.Margins[2].Sensitive = false;
				scriptedit.Margins[2].Width = 5;

				scriptedit.AutomaticFold = AutomaticFold.None;
			}

			// Rearrange the layout
			this.PerformLayout();
		}
		
		// This returns the current word (where the caret is at)
		public string GetCurrentWord()
		{
			return GetWordAt(scriptedit.CurrentPosition);
		}

		// This returns the word at the given position
		public string GetWordAt(int position)
		{
			return scriptedit.GetWordFromPosition(position);
		}

		// Save one undo checkpoint when making many changes in the editor.
		public void UndoTransaction(Action callback)
		{
			scriptedit.BeginUndoAction();

			try
			{
				callback.Invoke();
			}
			finally
			{
				scriptedit.EndUndoAction();
			}
		}

		// Perform undo
		public void Undo()
		{
			scriptedit.Undo();
		}

		// Perform redo
		public void Redo()
		{
			scriptedit.Redo();
		}

		// This clears all undo levels
		public void ClearUndoRedo()
		{
			scriptedit.EmptyUndoBuffer();
		}

		//mxd. This marks the current document as unmodified
		public void SetSavePoint()
		{
			scriptedit.SetSavePoint();
		}

		// Perform cut
		public void Cut()
		{
			scriptedit.Cut();
		}

		// Perform copy
		public void Copy()
		{
			scriptedit.Copy();
		}

		// Perform paste
		public void Paste()
		{
			scriptedit.Paste();
		}
		
		// This steals the focus (use with care!)
		public void GrabFocus()
		{
			scriptedit.Focus();
		}

		public byte[] GetText()
		{
			return encoding.GetBytes(scriptedit.Text); //mxd TODO: other encodings?..
		}

		public void SetText(byte[] text)
		{
			scriptedit.Text = encoding.GetString(text); //mxd TODO: other encodings?..
		}

		//mxd
		public void InsertSnippet(string[] lines) 
		{
			// Insert the snippet
			List<string> processedlines = new List<string>(lines.Length);
			int curline = scriptedit.LineFromPosition(scriptedit.SelectionStart);
			int indent = scriptedit.Lines[scriptedit.CurrentLine].Indentation;
			string tabs = Environment.NewLine + GetIndentationString(indent);
			string spaces = new String(' ', General.Settings.ScriptTabWidth);
			string[] linebreak = { LINE_BREAK_MARKER };
			int entrypos = -1;
			int entryline = -1;

			// Process line breaks
			foreach(string line in lines)
			{
				if(line.IndexOf(linebreak[0], StringComparison.Ordinal) != -1)
				{
					if(General.Settings.ScriptAllmanStyle)
						processedlines.AddRange(line.Split(linebreak, StringSplitOptions.RemoveEmptyEntries));
					else
						processedlines.Add(line.Replace(linebreak[0], " "));
				}
				else
				{
					processedlines.Add(line);
				}
			}

			// Process special chars, try to find entry position marker
			for(int i = 0; i < processedlines.Count; i++) 
			{
				if(!scriptedit.UseTabs) processedlines[i] = processedlines[i].Replace("\t", spaces);

				// Check if we have the [EP] marker
				if(entrypos == -1) 
				{
					int pos = processedlines[i].IndexOf(ENTRY_POSITION_MARKER, StringComparison.OrdinalIgnoreCase);
					if(pos != -1) 
					{
						processedlines[i] = processedlines[i].Remove(pos, 4);
						entryline = curline + i;
						entrypos = processedlines[i].Length - pos;
					}
				}
			}

			// Replace the text
			string text = string.Join(tabs, processedlines.ToArray());
			scriptedit.SelectionStart = scriptedit.WordStartPosition(scriptedit.CurrentPosition, true);
			scriptedit.SelectionEnd = scriptedit.WordEndPosition(scriptedit.CurrentPosition, true);
			scriptedit.ReplaceSelection(text);

			// Move the cursor if we had the [EP] marker
			if(entrypos != -1) 
			{
				// Count from the end of the line, because I don't see a reliable way to count indentation chars...
				int pos = scriptedit.Lines[entryline].EndPosition - entrypos;
				if(scriptedit.Lines[entryline].Text.EndsWith(Environment.NewLine)) pos -= 2;
				scriptedit.SetEmptySelection(pos);
			}
		}

		//mxd. Find next result
		public bool FindNext(FindReplaceOptions options, bool useselectionstart)
		{
			if(string.IsNullOrEmpty(options.FindText)) return false;

			// Find next match/abort when trying to replace in read-only tab
			if(scriptedit.ReadOnly && options.ReplaceWith != null)
			{
				if(options.SearchMode != FindReplaceSearchMode.CURRENT_FILE && OnFindNextWrapAround != null)
					return OnFindNextWrapAround(options);
				return false;
			}

			int startpos = (useselectionstart ? Math.Min(scriptedit.SelectionStart, scriptedit.SelectionEnd) : Math.Max(scriptedit.SelectionStart, scriptedit.SelectionEnd));

			// Search the document
			scriptedit.TargetStart = startpos;
			scriptedit.TargetEnd = scriptedit.TextLength;
			scriptedit.SearchFlags = options.CaseSensitive ? SearchFlags.MatchCase : SearchFlags.None;
			if(options.WholeWord) scriptedit.SearchFlags |= SearchFlags.WholeWord;

			int result = scriptedit.SearchInTarget(options.FindText);
			
			// Wrap around?
			if(result == -1)
			{
				if (options.WrapAroundDisabled)
					return false;

				if (options.SearchMode != FindReplaceSearchMode.CURRENT_FILE 
					&& OnFindNextWrapAround != null && OnFindNextWrapAround(options))
				{
					return true;
				}

				scriptedit.TargetStart = 0;
				scriptedit.TargetEnd = startpos;
				result = scriptedit.SearchInTarget(options.FindText);
			}

			// Found something
			if(result != -1)
			{
				// Select the result
				SelectAndShow(result, result + options.FindText.Length);

				// Update extra highlights
				HighlightWord(options.FindText);

				// All done
				return true;
			}

			// Nothing found...
			return false;
		}

		//mxd. Find previous result
		public bool FindPrevious(FindReplaceOptions options)
		{
			if(string.IsNullOrEmpty(options.FindText)) return false;

			// Find previous match/abort when trying to replace in read-only tab
			if(scriptedit.ReadOnly && options.ReplaceWith != null)
			{
				if(options.SearchMode != FindReplaceSearchMode.CURRENT_FILE && OnFindPreviousWrapAround != null)
					return OnFindPreviousWrapAround(options);
				return false;
			}

			int endpos = Math.Max(0, Math.Min(scriptedit.SelectionStart, scriptedit.SelectionEnd) - 1);

			// Search the document
			scriptedit.TargetStart = endpos;
			scriptedit.TargetEnd = 0;
			scriptedit.SearchFlags = options.CaseSensitive ? SearchFlags.MatchCase : SearchFlags.None;
			if(options.WholeWord) scriptedit.SearchFlags |= SearchFlags.WholeWord;

			int result = scriptedit.SearchInTarget(options.FindText);

			// Wrap around?
			if(result == -1)
			{
				if(options.SearchMode != FindReplaceSearchMode.CURRENT_FILE 
					&& OnFindPreviousWrapAround != null && OnFindPreviousWrapAround(options))
				{
					return true;
				}

				scriptedit.TargetStart = scriptedit.TextLength;
				scriptedit.TargetEnd = endpos;
				result = scriptedit.SearchInTarget(options.FindText);
			}

			// Found something
			if(result != -1)
			{
				// Select the result
				SelectAndShow(result, result + options.FindText.Length);

				// Update extra highlights
				HighlightWord(options.FindText);

				// All done
				return true;
			}

			// Nothing found...
			return false;
		}

		//mxd. (Un)indents selection
		public void IndentSelection(bool indent)
		{
			// Get selected range of lines
			int startline = scriptedit.LineFromPosition(scriptedit.SelectionStart);
			int endline = scriptedit.LineFromPosition(scriptedit.SelectionEnd);

			for(int i = startline; i < endline + 1; i++)
			{
				scriptedit.Lines[i].Indentation += (indent ? General.Settings.ScriptTabWidth : -General.Settings.ScriptTabWidth);
			}
		}

		//mxd
		public void DuplicateLine()
		{
			//scriptedit.DirectMessage(NativeMethods.SCI_LINEDUPLICATE);
			
			// Do it manually instead of using Scintilla's builtin variant to avoid triggering InsertCheck event, 
			// which conatains only "\r\n" text, which in turn messes with our scriptedit_InsertCheck handler logic
			// resulting in extra indentation...
			var curline = scriptedit.Lines[scriptedit.CurrentLine];
			scriptedit.InsertText(curline.EndPosition, curline.Text);

			// Offset selection by line length
			scriptedit.SetEmptySelection(curline.EndPosition + (scriptedit.SelectionStart - curline.Position));
		}

		//mxd
		internal List<CompilerError> UpdateNavigator(ScriptDocumentTab tab)
		{
			List<CompilerError> result = new List<CompilerError>();

			// Just clear the navigator when current tab has no text
			if(scriptedit.Text.Length == 0)
			{
				functionbar.Items.Clear();
				functionbar.Enabled = false;
				return result;
			}

			// Store currently selected item name
			string prevtext = functionbar.Text;

			// Repopulate FunctionBar
			result = handler.UpdateFunctionBarItems(tab, new MemoryStream(GetText()), functionbar);

			// Put some text in the navigator (but don't actually trigger selection event)
			functionbar.Enabled = (functionbar.Items.Count > 0);
			if(functionbar.Items.Count > 0)
			{
				preventchanges = true;

				// Put the text back if we still have the corresponding item
				if(!string.IsNullOrEmpty(prevtext))
				{
					foreach(var item in functionbar.Items)
					{
						if(item.ToString() == prevtext)
						{
							functionbar.Text = item.ToString();
							break;
						}
					}
				}

				// No dice. Use the first item
				if(string.IsNullOrEmpty(functionbar.Text))
					functionbar.Text = functionbar.Items[0].ToString();

				preventchanges = false;
			}

			return result;
		}

		#endregion

		#region ================== Utility methods

		// This returns the ScriptStyleType for a given Scintilla style
		internal ScriptStyleType GetScriptStyle(int scintillastyle)
		{
			return (stylelookup.ContainsKey(scintillastyle) ? stylelookup[scintillastyle] : ScriptStyleType.PlainText);
		}
		
		// This registers an image for the autocomplete list
		private void RegisterAutoCompleteImage(ImageIndex index, Bitmap image)
		{
			// Register image
			scriptedit.RegisterRgbaImage((int)index, image);
		}

		// This registers an image for the markes list
		private void RegisterMarkerImage(ImageIndex index, Bitmap image)
		{
			// Register image
			scriptedit.Markers[(int)index].DefineRgbaImage(image);
			scriptedit.Markers[(int)index].Symbol = MarkerSymbol.RgbaImage;
		}

		//mxd
		private string GetIndentationString(int indent)
		{
			if(scriptedit.UseTabs)
			{
				string indentstr = string.Empty;
				int numtabs = indent / scriptedit.TabWidth;
				if(numtabs > 0) indentstr = new string('\t', numtabs);

				// Mixed padding? Add spaces
				if(numtabs * scriptedit.TabWidth < indent)
				{
					int numspaces = indent - numtabs * scriptedit.TabWidth;
					indentstr += new string(' ', numspaces);
				}

				return indentstr;
			}
			else
			{
				return new string(' ', indent);
			}
		}

		//mxd. https://github.com/jacobslusser/ScintillaNET/wiki/Find-and-Highlight-Words
		private void HighlightWord(string text)
		{
			// Remove all uses of our indicator
			scriptedit.IndicatorCurrent = HIGHLIGHT_INDICATOR;
			scriptedit.IndicatorClearRange(0, scriptedit.TextLength);

			// Update indicator appearance
			scriptedit.Indicators[HIGHLIGHT_INDICATOR].Style = IndicatorStyle.RoundBox;
			scriptedit.Indicators[HIGHLIGHT_INDICATOR].Under = true;
			scriptedit.Indicators[HIGHLIGHT_INDICATOR].ForeColor = General.Colors.ScriptIndicator.ToColor();
			scriptedit.Indicators[HIGHLIGHT_INDICATOR].OutlineAlpha = 50;
			scriptedit.Indicators[HIGHLIGHT_INDICATOR].Alpha = 30;

			// Search the document
			scriptedit.TargetStart = 0;
			scriptedit.TargetEnd = scriptedit.TextLength;
			scriptedit.SearchFlags = SearchFlags.WholeWord;

			while(scriptedit.SearchInTarget(text) != -1)
			{
				//mxd. Don't mark currently selected word
				if(scriptedit.SelectionStart != scriptedit.TargetStart && scriptedit.SelectionEnd != scriptedit.TargetEnd)
				{
					// Mark the search results with the current indicator
					scriptedit.IndicatorFillRange(scriptedit.TargetStart, scriptedit.TargetEnd - scriptedit.TargetStart);
				}

				// Search the remainder of the document
				scriptedit.TargetStart = scriptedit.TargetEnd;
				scriptedit.TargetEnd = scriptedit.TextLength;
			}
		}

		//mxd. Handle keyboard shortcuts
		protected override bool ProcessCmdKey(ref Message msg, Keys keydata)
		{
			// F3 for Find Next
			if(keydata == Keys.F3)
			{
				if(OnFindNext != null) OnFindNext();
				return true;
			}

			//mxd. F2 for Find Previous
			if(keydata == Keys.F2)
			{
				if(OnFindPrevious != null) OnFindPrevious();
				return true;
			}

			//mxd. F5 for Compile Script
			if(keydata == Keys.F5)
			{
				if(OnCompileScript != null) OnCompileScript();
				return true;
			}

			// CTRL+F for find & replace
			if(keydata == (Keys.Control | Keys.F))
			{
				if(OnOpenFindAndReplace != null) OnOpenFindAndReplace();
				return true;
			}

			// CTRL+G for go to line
			if(keydata == (Keys.Control | Keys.G))
			{
				if(OnGoToLine != null) OnGoToLine();
				return true;
			}

			// CTRL+S for save
			if(!scriptedit.ReadOnly && keydata == (Keys.Control | Keys.S))
			{
				if(OnExplicitSaveTab != null) OnExplicitSaveTab();
				return true;
			}

			// CTRL+O for open
			if(keydata == (Keys.Control | Keys.O))
			{
				if(OnOpenScriptBrowser != null) OnOpenScriptBrowser();
				return true;
			}

			// CTRL+Space to autocomplete
			if(!scriptedit.ReadOnly && keydata == (Keys.Control | Keys.Space))
			{
				// Hide call tip if any
				scriptedit.CallTipCancel();

				// Show autocomplete
				handler.ShowAutoCompletionList();
				return true;
			}

			//mxd. Tab to expand code snippet. Do it only when the text cursor is at the end of a keyword.
			if(!scriptedit.ReadOnly && keydata == Keys.Tab && !scriptedit.AutoCActive)
			{
				string curword = GetCurrentWord().ToLowerInvariant();
				if(scriptconfig.Snippets.Contains(curword) && scriptedit.CurrentPosition == scriptedit.WordEndPosition(scriptedit.CurrentPosition, true))
				{
					InsertSnippet(scriptconfig.GetSnippet(curword));
					return true;
				}
			}

			//mxd. Skip text insert when "save screenshot" action keys are pressed
			Actions.Action[] actions = General.Actions.GetActionsByKey((int)keydata);
			foreach(Actions.Action action in actions)
			{
				if(action.ShortName == "savescreenshot" || action.ShortName == "saveeditareascreenshot") return true;
			}

			// Pass to base
			return base.ProcessCmdKey(ref msg, keydata);
		}

		#endregion
		
		#region ================== Events
		
		// Layout needs to be re-organized
		protected override void OnLayout(LayoutEventArgs e)
		{
			base.OnLayout(e);

			// With or without functions bar?
			if(functionbar.Visible)
			{
				scriptpanel.Top = functionbar.Bottom + 6;
				scriptpanel.Height = this.ClientSize.Height - scriptpanel.Top;
			}
			else
			{
				scriptpanel.Top = 0;
				scriptpanel.Height = this.ClientSize.Height;
			}
		}

		//mxd. Script text changed
		private void scriptedit_TextChanged(object sender, EventArgs e)
		{
			// Line number margin width needs changing?
			int curlinenumbercharlength = scriptedit.Lines.Count.ToString().Length;

			// Calculate the width required to display the last line number
			// and include some padding for good measure.
			if(curlinenumbercharlength != linenumbercharlength)
			{
				const int padding = 2;
				scriptedit.Margins[1].Width = scriptedit.TextWidth(Style.LineNumber, new string('9', curlinenumbercharlength + 1)) + padding;
				linenumbercharlength = curlinenumbercharlength;
			}
			
			if(OnTextChanged != null) OnTextChanged(this, EventArgs.Empty);
		}

		//mxd
		private void scriptedit_CharAdded(object sender, CharAddedEventArgs e)
		{
			// Hide call tip if any
			scriptedit.CallTipCancel();

			// Offset caret if needed
			if(caretoffset != 0)
			{
				scriptedit.SetEmptySelection(scriptedit.SelectionStart + caretoffset);
				caretoffset = 0;
				if(!expandcodeblock) return;
			}

			// Move CodeBlockOpen to the new line?
			if(expandcodeblock)
			{
				if(scriptedit.CurrentLine > 0)
				{
					string linetext = scriptedit.Lines[scriptedit.CurrentLine - 1].Text;
					int blockopenpos = (string.IsNullOrEmpty(scriptconfig.CodeBlockOpen) ? -1 : linetext.LastIndexOf(scriptconfig.CodeBlockOpen, StringComparison.Ordinal));
					if(blockopenpos != -1)
					{
						// Do it only if initial line doesn't start with CodeBlockOpen
						string linestart = linetext.Substring(0, blockopenpos).Trim();
						if(linestart.Length > 0)
						{
							scriptedit.InsertText(scriptedit.Lines[scriptedit.CurrentLine - 1].Position + blockopenpos, 
								Environment.NewLine + GetIndentationString(scriptedit.Lines[scriptedit.CurrentLine - 1].Indentation));
						}
					}
				}

				expandcodeblock = false;
				return;
			}

			// Auto-match braces
			if(General.Settings.ScriptAutoCloseBrackets)
			{
				//TODO: Auto-match quotes
				bool endpos = (scriptedit.CurrentPosition == scriptedit.TextLength);
				if(!string.IsNullOrEmpty(scriptconfig.CodeBlockOpen) && e.Char == scriptconfig.CodeBlockOpen[0] && !string.IsNullOrEmpty(scriptconfig.CodeBlockClose) &&
					(endpos || (char)scriptedit.GetCharAt(scriptedit.CurrentPosition + 1) != scriptconfig.CodeBlockClose[0]))
				{
					scriptedit.InsertText(scriptedit.CurrentPosition, scriptconfig.CodeBlockClose);
					return;
				}
				
				if(!string.IsNullOrEmpty(scriptconfig.FunctionOpen) && e.Char == scriptconfig.FunctionOpen[0] && !string.IsNullOrEmpty(scriptconfig.FunctionClose) &&
					(endpos || (char)scriptedit.GetCharAt(scriptedit.CurrentPosition + 1) != scriptconfig.FunctionClose[0]))
				{
					scriptedit.InsertText(scriptedit.CurrentPosition, scriptconfig.FunctionClose);
					return;
				}
				
				if(!string.IsNullOrEmpty(scriptconfig.ArrayOpen) && e.Char == scriptconfig.ArrayOpen[0] && !string.IsNullOrEmpty(scriptconfig.ArrayClose) &&
					(endpos || (char)scriptedit.GetCharAt(scriptedit.CurrentPosition + 1) != scriptconfig.ArrayClose[0]))
				{
					scriptedit.InsertText(scriptedit.CurrentPosition, scriptconfig.ArrayClose);
					return;
				}
			}

			if(!scriptedit.ReadOnly && General.Settings.ScriptAutoShowAutocompletion)
			{
				// Display the autocompletion list
				handler.ShowAutoCompletionList();
			}
		}

		//mxd
		private void scriptedit_UpdateUI(object sender, UpdateUIEventArgs e)
		{
			// If a word is selected, highlight the same words
			if(scriptedit.SelectedText != highlightedword)
			{
				// Highlight only when whole word is selected
				if(!string.IsNullOrEmpty(scriptedit.SelectedText) && scriptedit.GetWordFromPosition(scriptedit.SelectionStart) == scriptedit.SelectedText)
				{
					HighlightWord(scriptedit.SelectedText);
				}
				else
				{
					// Clear highlight
					scriptedit.IndicatorCurrent = HIGHLIGHT_INDICATOR;
					scriptedit.IndicatorClearRange(0, scriptedit.TextLength);
				}

				highlightedword = scriptedit.SelectedText;
			}
			
			// Has the caret changed position?
			int caretpos = scriptedit.CurrentPosition;
			if(lastcaretpos != caretpos && scriptconfig.BraceChars.Count > 0)
			{
				// Perform brace matching (https://github.com/jacobslusser/ScintillaNET/wiki/Brace-Matching)
				lastcaretpos = caretpos;
				int bracepos1 = -1;

				// Is there a brace to the left or right?
				if(caretpos > 0 && scriptconfig.BraceChars.Contains((char)scriptedit.GetCharAt(caretpos - 1)))
					bracepos1 = (caretpos - 1);
				else if(scriptconfig.BraceChars.Contains((char)(scriptedit.GetCharAt(caretpos))))
					bracepos1 = caretpos;

				if(bracepos1 > -1)
				{
					// Find the matching brace
					int bracepos2 = scriptedit.BraceMatch(bracepos1);
					if(bracepos2 == Scintilla.InvalidPosition)
						scriptedit.BraceBadLight(bracepos1);
					else
						scriptedit.BraceHighlight(bracepos1, bracepos2);
				}
				else
				{
					// Turn off brace matching
					scriptedit.BraceHighlight(Scintilla.InvalidPosition, Scintilla.InvalidPosition);
				}
			}
		}

		//mxd
		private void scriptedit_InsertCheck(object sender, InsertCheckEventArgs e)
		{
			// Do we want auto-indentation?
			if(!expandcodeblock && General.Settings.ScriptAutoIndent && e.Text == "\r\n")
			{
				// Get current line indentation up to the cursor position
				string linetext = scriptedit.Lines[scriptedit.CurrentLine].Text;
				int selectionpos = scriptedit.SelectionStart - scriptedit.Lines[scriptedit.CurrentLine].Position;
				int indent = 0;
				for(int i = 0; i < selectionpos; i++)
				{
					switch(linetext[i])
					{
						case  ' ': indent++; break;
						case '\t': indent += scriptedit.TabWidth; break;
						default: i = selectionpos; break; // break the loop
					}
				}

				// Store initial indentation
				int initialindent = indent;

				// Need to increase indentation? We do this when:
				// 1. Line contains '{' and '}' and the cursor is between them
				// 2. Line either doesn't contain '}', or it's before '{', or the line contains '{' and the cursor is after it 
				int blockopenpos = (string.IsNullOrEmpty(scriptconfig.CodeBlockOpen) ? -1 : linetext.LastIndexOf(scriptconfig.CodeBlockOpen, selectionpos, StringComparison.Ordinal));
				int blockclosepos = (string.IsNullOrEmpty(scriptconfig.CodeBlockOpen) ? -1 : linetext.IndexOf(scriptconfig.CodeBlockClose, selectionpos, StringComparison.Ordinal));

				// Add indentation when the cursor is between { and }
				bool addindent = (blockopenpos != -1 && blockopenpos < selectionpos) && (blockclosepos == -1 || (blockopenpos < blockclosepos && blockclosepos >= selectionpos));
				if(addindent) indent += scriptedit.TabWidth;

				// Calculate indentation
				string indentstr = GetIndentationString(indent);

				// Move CodeBlockOpen to the new line? (will be applied in scriptedit_CharAdded)
				expandcodeblock = General.Settings.ScriptAllmanStyle;

				// Offset closing block char?
				if(addindent && blockclosepos != -1)
				{
					string initialindentstr = GetIndentationString(initialindent);
					indentstr += Environment.NewLine + initialindentstr;
					
					// Offset cursor position (will be performed in scriptedit_CharAdded)
					caretoffset = -(initialindentstr.Length + Environment.NewLine.Length);
				}

				// Apply new indentation
				e.Text += indentstr;
			}
		}

		//mxd
		private void scriptedit_AutoCCompleted(object sender, AutoCSelectionEventArgs e)
		{
			// Expand snippet?
			string[] lines = scriptconfig.GetSnippet(e.Text);
			if(lines != null)
			{
				InsertSnippet(lines);
			}
			else
			{
				string definition = scriptconfig.GetFunctionDefinition(e.Text);
				if(!string.IsNullOrEmpty(definition))
				{
					int entrypos = definition.IndexOf(ENTRY_POSITION_MARKER, StringComparison.OrdinalIgnoreCase);
					
					// Replace inserted text with expanded version?
					if(e.Text.StartsWith("$") || entrypos != -1)
					{
						// Remove the marker
						if(entrypos != -1) definition = definition.Remove(entrypos, 4);
						
						// Replace insterted text with expanded comment
						int startpos = scriptedit.WordStartPosition(scriptedit.CurrentPosition, true);
						scriptedit.SelectionStart = startpos;
						scriptedit.SelectionEnd = scriptedit.WordEndPosition(scriptedit.CurrentPosition, true);
						scriptedit.ReplaceSelection(definition);

						// Update caret position
						if(entrypos != -1) scriptedit.SetEmptySelection(startpos + entrypos);
					}
				}
			}
		}

		//mxd
		private void functionbar_SelectedIndexChanged(object sender, EventArgs e)
		{
			if(!preventchanges && functionbar.SelectedItem is ScriptItem)
			{
				ScriptItem si = (ScriptItem)functionbar.SelectedItem;
				EnsureLineVisible(LineFromPosition(si.CursorPosition));
				scriptedit.SelectionStart = si.CursorPosition;
				scriptedit.SelectionEnd = si.CursorPosition;

				// Focus to the editor!
				scriptedit.Focus();
			}
		}

		private void functionbar_DropDown(object sender, EventArgs e)
		{
			if(OnFunctionBarDropDown != null) OnFunctionBarDropDown(sender, e);
		}
		
		#endregion

		#region ================== Context menu Events

		private void contextmenu_Opening(object sender, System.ComponentModel.CancelEventArgs e)
		{
			menuundo.Enabled = scriptedit.CanUndo;
			menuredo.Enabled = scriptedit.CanRedo;

			bool cancopy = (scriptedit.SelectionEnd > scriptedit.SelectionStart);
			menucut.Enabled = cancopy;
			menucopy.Enabled = cancopy;
			menupaste.Enabled = scriptedit.CanPaste;
			menudelete.Enabled = cancopy;

			menufindusages.Enabled = !string.IsNullOrEmpty(scriptedit.GetWordFromPosition(scriptedit.SelectionStart));
		}

		private void menuundo_Click(object sender, EventArgs e)
		{
			scriptedit.Undo();
		}

		private void menuredo_Click(object sender, EventArgs e)
		{
			scriptedit.Redo();
		}

		private void menucut_Click(object sender, EventArgs e)
		{
			scriptedit.Cut();
		}

		private void menucopy_Click(object sender, EventArgs e)
		{
			scriptedit.Copy();
		}

		private void menupaste_Click(object sender, EventArgs e)
		{
			scriptedit.Paste();
		}

		private void menudelete_Click(object sender, EventArgs e)
		{
			scriptedit.DeleteRange(scriptedit.SelectionStart, scriptedit.SelectionEnd - scriptedit.SelectionStart);
		}

		private void menuduplicateline_Click(object sender, EventArgs e)
		{
			DuplicateLine();
		}

		private void menuselectall_Click(object sender, EventArgs e)
		{
			scriptedit.SelectAll();
		}

		#endregion

	}
}