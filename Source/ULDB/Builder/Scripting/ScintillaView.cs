// ULDB: the visible script editor: an AvaloniaEdit text editor attached to a Scintilla model
// (Scintilla.cs). It draws the model's styles (syntax colours from the lexer), line numbers, error
// markers, matching braces, highlighted words and folds, and gives the model what only a view
// knows: the selection, scrolling, coordinates, the clipboard, the autocompletion list and call tips.
// Typing goes through the model's events (InsertCheck, CharAdded, UpdateUI) so upstream's
// ScriptEditorControl behaves as written: auto indent, auto close, snippets and so on.

using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using AvaloniaEdit;
using AvaloniaEdit.CodeCompletion;
using AvaloniaEdit.Document;
using AvaloniaEdit.Editing;
using AvaloniaEdit.Folding;
using AvaloniaEdit.Rendering;
using ScintillaNET;
using ULDB.Input;
using ULDB.Shell;
using SD = System.Drawing;
using WF = System.Windows.Forms;

namespace ULDB.Scripting
{
	internal sealed class ScintillaView : UserControl, IScintillaHost
	{
		private const string FontUri = "avares://Builder/Fonts#JetBrains Mono";
		private const int HIGHLIGHT_INDICATOR = 8;
		private const int ERROR_MARKER = 2;

		private readonly Scintilla model;
		private readonly TextEditor editor;
		private readonly ScriptColorizer colorizer;
		private readonly MarkerMargin markermargin;
		private FoldingManager folding;
		private CompletionWindow completion;
		private readonly Popup calltip;
		private readonly TextBlock calltiptext = new TextBlock { FontFamily = new FontFamily(FontUri), FontSize = 12, TextWrapping = TextWrapping.NoWrap };
		private bool updatepending, foldpending;
		private readonly AvaloniaEdit.Search.SearchPanel searchpanel;
		private int lastcaret = -1;

		/// <summary>Offers shortcut keys (F3, Ctrl+F, Ctrl+S...) to the owning control; true when it used the key.</summary>
		public Func<WF.Keys, bool> CommandKey { get; set; }

		/// <summary>What to show for a word the pointer rests on, or in the completion list (a function's signature); null for nothing.</summary>
		public Func<string, string> WordInfo { get; set; }

		/// <summary>False for the preview in Preferences, which shows the settings' size.</summary>
		public bool UseSharedZoom { get; set; } = true;

		public TextEditor Editor { get { return editor; } }

		public ScintillaView(Scintilla model)
		{
			this.model = model;
			editor = new TextEditor
			{
				Document = model.Document,
				ShowLineNumbers = true,
				FontFamily = new FontFamily(FontUri + ", Cascadia Code, DejaVu Sans Mono, monospace"),
				FontSize = 14,
				HorizontalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto,
				VerticalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto,
			};
			editor.Options.HighlightCurrentLine = true;
			editor.Options.EnableHyperlinks = false;
			editor.Options.EnableEmailHyperlinks = false;
			editor.Options.AllowScrollBelowDocument = true;
			editor.Options.EnableTextDragDrop = true;
			editor.Options.ShowBoxForControlCharacters = true;
			Content = editor;

			colorizer = new ScriptColorizer(model);
			editor.TextArea.TextView.LineTransformers.Add(colorizer);
			editor.TextArea.TextView.BackgroundRenderers.Add(new HighlightRenderer(model, editor));

			markermargin = new MarkerMargin(model, editor);
			editor.TextArea.LeftMargins.Insert(0, markermargin);

			calltip = new Popup
			{
				PlacementTarget = editor,
				Placement = PlacementMode.AnchorAndGravity,
				IsLightDismissEnabled = false,
				Child = new Border
				{
					Padding = new Thickness(8, 4),
					CornerRadius = new CornerRadius(4),
					BorderThickness = new Thickness(1),
					Child = calltiptext,
				},
			};
			((Border)calltip.Child).Bind(Border.BackgroundProperty, this.GetResourceObservable("UldbCardBrush"));
			((Border)calltip.Child).Bind(Border.BorderBrushProperty, this.GetResourceObservable("UldbLineBrush"));

			model.Host = this;
			model.AppearanceChanged += ApplyAppearance;
			model.Document.Changed += (s, e) => { SchedulePrimary(); ScheduleFolds(); };
			editor.TextArea.Caret.PositionChanged += (s, e) => SchedulePrimary();
			editor.TextArea.SelectionChanged += (s, e) => SchedulePrimary();

			// Typing: model events as ScintillaNET raises them
			editor.TextArea.TextEntered += (s, e) =>
			{
				foreach(char c in e.Text ?? "") model.RaiseCharAdded(c);
			};
			editor.TextArea.AddHandler(InputElement.KeyDownEvent, OnKeyDown, Avalonia.Interactivity.RoutingStrategies.Tunnel);
			editor.TextArea.Caret.PositionChanged += (s, e) => { if(calltip.IsOpen && Math.Abs(editor.CaretOffset - lastcaret) > 1) CancelCallTip(); lastcaret = editor.CaretOffset; };

			// The dark scheme depends on the theme, known once the control is in the window
			ActualThemeVariantChanged += (s, e) => ApplyAppearance();
			AttachedToVisualTree += (s, e) =>
			{
				model.Host = this; // the view on screen is the one the model talks to
				SchedulePrimary(); // the status bar shows this script's caret position, not the previous tab's
				if(UseSharedZoom) zoom = ULDB.Platform.UiOptions.ScriptZoom;
				ULDB.Platform.UiOptions.ScriptViewChanged += OnSharedViewChanged;
				ApplyAppearance();
			};
			DetachedFromVisualTree += (s, e) => ULDB.Platform.UiOptions.ScriptViewChanged -= OnSharedViewChanged;

			// Resting the pointer on a function name shows its signature
			editor.TextArea.TextView.PointerHover += OnPointerHover;
			editor.TextArea.TextView.PointerHoverStopped += (s, e) => ToolTip.SetIsOpen(editor, false);

			// The find bar inside the editor (Ctrl+F, Ctrl+H to replace); upstream's Find and Replace window stays on
			// Ctrl+Shift+F and in the menu, for searching all open scripts
			searchpanel = AvaloniaEdit.Search.SearchPanel.Install(editor);

			ApplyAppearance();
			InstallFolding();
		}

		private void OnPointerHover(object sender, PointerEventArgs e)
		{
			if(WordInfo == null) return;
			TextViewPosition? pos = editor.TextArea.TextView.GetPositionFloor(e.GetPosition(editor.TextArea.TextView) + editor.TextArea.TextView.ScrollOffset);
			if(!pos.HasValue) return;
			int offset = editor.Document.GetOffset(pos.Value.Location);
			int style = model.GetStyleAt(offset);
			if(style == ScriptLexer.BlockComment || style == ScriptLexer.LineComment || style == ScriptLexer.String) return;
			string word = model.GetWordFromPosition(offset);
			string info = string.IsNullOrEmpty(word) ? null : WordInfo(word);
			if(string.IsNullOrEmpty(info)) return;
			ToolTip.SetTip(editor, new TextBlock { Text = info, FontFamily = new FontFamily(FontUri), FontSize = 12 });
			ToolTip.SetPlacement(editor, PlacementMode.Pointer);
			ToolTip.SetIsOpen(editor, true);
		}

		private void OpenFindBar(bool replace)
		{
			string selected = editor.SelectionLength > 0 && !editor.SelectedText.Contains('\n') ? editor.SelectedText : null;
			searchpanel.IsReplaceMode = replace && !model.ReadOnly;
			if(selected != null) searchpanel.SearchPattern = selected;
			searchpanel.Open();
			searchpanel.Reactivate();
		}

		#region ================== Appearance (from the model's styles)

		private static Color ToColor(SD.Color c) { return Color.FromArgb(c.A, c.R, c.G, c.B); }

		private static bool IsSet(SD.Color c) { return !c.IsEmpty; }

		private void ApplyAppearance()
		{
			Style def = model.Styles[Style.Default];
			if(!string.IsNullOrEmpty(def.Font)) editor.FontFamily = new FontFamily(ResolveFont(def.Font));
			if(def.Size > 0) basefontsize = def.Size * 96.0 / 72.0;
			editor.FontSize = Math.Max(7, Math.Min(48, basefontsize + zoom));
			editor.FontWeight = def.Bold ? FontWeight.Bold : FontWeight.Normal;
			editor.FontStyle = def.Italic ? FontStyle.Italic : FontStyle.Normal;
			colorizer.ItalicComments = ULDB.Platform.UiOptions.ScriptItalicComments;
			// UDB's default script colours are dark text on white. With the desktop in dark mode and those
			// defaults still in place, the editor uses a dark scheme instead; colours the user changed stay.
			bool defaultcolors = !IsSet(def.BackColor) || (def.BackColor.R == 255 && def.BackColor.G == 255 && def.BackColor.B == 255);
			bool darkscheme = defaultcolors && ActualThemeVariant == Avalonia.Styling.ThemeVariant.Dark;
			colorizer.Palette = darkscheme ? DarkPalette : null;
			colorizer.Reset();
			editor.TextArea.TextView.CurrentLineBackground = new SolidColorBrush(darkscheme ? Color.FromArgb(0x30, 0x80, 0x90, 0xA0) : Color.FromArgb(0x30, 0x40, 0xC0, 0xB0));
			editor.TextArea.TextView.CurrentLineBorder = new Pen(new SolidColorBrush(darkscheme ? Color.FromArgb(0x40, 0x80, 0x90, 0xA0) : Color.FromArgb(0x50, 0x40, 0xC0, 0xB0)), 1);

			if(darkscheme)
			{
				editor.Background = new SolidColorBrush(Color.FromRgb(0x1E, 0x1E, 0x1E));
				editor.Foreground = new SolidColorBrush(Color.FromRgb(0xD4, 0xD4, 0xD4));
				editor.LineNumbersForeground = new SolidColorBrush(Color.FromRgb(0x85, 0x85, 0x85));
				editor.TextArea.SelectionBrush = new SolidColorBrush(Color.FromRgb(0x26, 0x4F, 0x78));
				editor.TextArea.SelectionForeground = null;
				editor.TextArea.Caret.CaretBrush = new SolidColorBrush(Color.FromRgb(0xAE, 0xAF, 0xAD));
			}
			else
			{
				if(IsSet(def.BackColor)) editor.Background = new SolidColorBrush(ToColor(def.BackColor));
				if(IsSet(def.ForeColor)) editor.Foreground = new SolidColorBrush(ToColor(def.ForeColor));

				Style linenumbers = model.Styles[Style.LineNumber];
				editor.LineNumbersForeground = IsSet(linenumbers.ForeColor) ? new SolidColorBrush(ToColor(linenumbers.ForeColor)) : Brushes.Gray;

				if(IsSet(model.SelectionBackColor)) editor.TextArea.SelectionBrush = new SolidColorBrush(ToColor(model.SelectionBackColor));
				if(IsSet(model.SelectionForeColor)) editor.TextArea.SelectionForeground = new SolidColorBrush(ToColor(model.SelectionForeColor));
				if(IsSet(model.CaretForeColor)) editor.TextArea.Caret.CaretBrush = new SolidColorBrush(ToColor(model.CaretForeColor));
			}

			editor.Options.ConvertTabsToSpaces = !model.UseTabs;
			editor.Options.IndentationSize = Math.Max(1, model.TabWidth);
			editor.Options.ShowTabs = editor.Options.ShowSpaces = model.ViewWhitespace != WhitespaceMode.Invisible;
			editor.WordWrap = model.WrapMode != WrapMode.None;
			editor.IsReadOnly = model.ReadOnly;
			editor.ShowLineNumbers = model.Margins[1].Type == MarginType.Number && model.Margins[1].Width > 0;

			SetMarkers();
			editor.TextArea.TextView.Redraw();
			markermargin.InvalidateVisual();
			InstallFolding();
		}

		// The dark scheme (after VS Code's Dark+), by the lexer's style numbers.
		private static readonly Dictionary<int, IBrush> DarkPalette = new Dictionary<int, IBrush>
		{
			{ ScriptLexer.BlockComment, new SolidColorBrush(Color.FromRgb(0x6A, 0x99, 0x55)) },
			{ ScriptLexer.LineComment, new SolidColorBrush(Color.FromRgb(0x6A, 0x99, 0x55)) },
			{ ScriptLexer.Number, new SolidColorBrush(Color.FromRgb(0xB5, 0xCE, 0xA8)) },
			{ ScriptLexer.Keyword, new SolidColorBrush(Color.FromRgb(0x56, 0x9C, 0xD6)) },
			{ ScriptLexer.String, new SolidColorBrush(Color.FromRgb(0xCE, 0x91, 0x78)) },
			{ ScriptLexer.Character, new SolidColorBrush(Color.FromRgb(0xCE, 0x91, 0x78)) },
			{ ScriptLexer.Preprocessor, new SolidColorBrush(Color.FromRgb(0xC5, 0x86, 0xC0)) },
			{ ScriptLexer.Constant, new SolidColorBrush(Color.FromRgb(0x4E, 0xC9, 0xB0)) },
			{ ScriptLexer.Property, new SolidColorBrush(Color.FromRgb(0x9C, 0xDC, 0xFE)) },
			{ ScriptLexer.Operator, new SolidColorBrush(Color.FromRgb(0xD4, 0xD4, 0xD4)) },
			{ ScriptLexer.Identifier, new SolidColorBrush(Color.FromRgb(0xD4, 0xD4, 0xD4)) },
		};

		// "JetBrains Mono" is the editor's own font: the setting's name is only honoured when it's another installed one.
		private static string ResolveFont(string name)
		{
			if(string.IsNullOrWhiteSpace(name) || name.Equals("Courier New", StringComparison.OrdinalIgnoreCase) || name.Equals("Consolas", StringComparison.OrdinalIgnoreCase) || name.Equals("JetBrains Mono", StringComparison.OrdinalIgnoreCase))
				return FontUri + ", Cascadia Code, DejaVu Sans Mono, monospace";
			return name + ", " + FontUri + ", monospace";
		}

		private void SetMarkers()
		{
			// Keeps line number margin readable: nothing to do, colours follow the theme
		}

		#endregion

		#region ================== Folding

		private void InstallFolding()
		{
			bool wanted = model.AutomaticFold != AutomaticFold.None && model.Margins[2].Width >= 12;
			if(wanted && folding == null)
			{
				folding = FoldingManager.Install(editor.TextArea);
				ScheduleFolds();
			}
			else if(!wanted && folding != null)
			{
				FoldingManager.Uninstall(folding);
				folding = null;
			}
		}

		private void ScheduleFolds()
		{
			if(folding == null || foldpending) return;
			foldpending = true;
			DispatcherTimer.RunOnce(UpdateFolds, TimeSpan.FromMilliseconds(250));
		}

		private void UpdateFolds()
		{
			foldpending = false;
			if(folding == null) return;

			// Remember what's folded (by line of the fold's start), including the user's clicks
			HashSet<int> wasfolded = new HashSet<int>(model.FoldedLines);
			HashSet<int> known = new HashSet<int>();
			foreach(FoldingSection section in folding.AllFoldings)
			{
				int line = model.Document.GetLineByOffset(section.StartOffset).LineNumber - 1;
				known.Add(line);
				if(section.IsFolded) wasfolded.Add(line); else wasfolded.Remove(line);
			}

			List<NewFolding> foldings = new List<NewFolding>();
			foreach(int[] r in model.GetFoldRegions())
			{
				DocumentLine start = model.Document.GetLineByNumber(r[0] + 1), end = model.Document.GetLineByNumber(r[1] + 1);
				foldings.Add(new NewFolding(start.EndOffset, end.EndOffset) { Name = " ... ", DefaultClosed = wasfolded.Contains(r[0]) });
			}
			foldings.Sort((a, b) => a.StartOffset.CompareTo(b.StartOffset));
			folding.UpdateFoldings(foldings, -1);
			// DefaultClosed only counts for new sections; a fold asked for on a line whose section moved is closed here
			foreach(FoldingSection section in folding.AllFoldings)
			{
				int line = model.Document.GetLineByOffset(section.StartOffset).LineNumber - 1;
				if(!section.IsFolded && wasfolded.Contains(line) && !known.Contains(line)) section.IsFolded = true;
			}

			model.FoldedLines.Clear();
			foreach(int l in wasfolded) model.FoldedLines.Add(l);
		}

		#endregion

		#region ================== UpdateUI

		private void SchedulePrimary()
		{
			if(updatepending) return;
			updatepending = true;
			Dispatcher.UIThread.Post(() =>
			{
				updatepending = false;
				model.RaiseUpdateUI();
			}, DispatcherPriority.Background);
		}

		#endregion

		#region ================== Keys

		private void OnKeyDown(object sender, KeyEventArgs e)
		{
			WF.Keys keys = KeyMapping.ToKeys(e.Key, e.KeyModifiers);

			// The find bar
			bool ctrlonly = e.KeyModifiers == KeyModifiers.Control;
			if(ctrlonly && e.Key == Key.F) { OpenFindBar(false); e.Handled = true; return; }
			if(ctrlonly && e.Key == Key.H) { OpenFindBar(true); e.Handled = true; return; }
			if(e.KeyModifiers == (KeyModifiers.Control | KeyModifiers.Shift) && e.Key == Key.F && CommandKey != null)
			{
				// Upstream's window (it opens on Ctrl+F there)
				if(CommandKey(WF.Keys.Control | WF.Keys.F)) { e.Handled = true; return; }
			}
			if(searchpanel.IsOpened)
			{
				if(e.Key == Key.F3 && e.KeyModifiers == KeyModifiers.None) { searchpanel.FindNext(); e.Handled = true; return; }
				if(e.Key == Key.F3 && e.KeyModifiers == KeyModifiers.Shift) { searchpanel.FindPrevious(); e.Handled = true; return; }
				if(e.Key == Key.Escape && e.KeyModifiers == KeyModifiers.None && !(completion != null && completion.IsOpen)) { searchpanel.Close(); e.Handled = true; return; }
			}

			// Shortcuts of the editor (F3, Ctrl+F, Ctrl+S, Ctrl+Space, snippets on Tab, ...)
			if(CommandKey != null && keys != WF.Keys.None && !IsPlainTyping(e))
			{
				// Tab with a selection or without a snippet is the editor's: ProcessCmdKey answers false then
				if(CommandKey(keys)) { e.Handled = true; return; }
			}

			// Editing shortcuts beyond upstream's: comment, move, duplicate and delete lines, zoom
			if(HandleEditorShortcut(e)) { e.Handled = true; return; }

			// Enter: upstream's InsertCheck handler decides the indentation
			if(e.Key == Key.Return && e.KeyModifiers == KeyModifiers.None && !model.ReadOnly)
			{
				if(completion != null && completion.IsOpen) return; // the completion window takes it
				string text = model.RaiseInsertCheck("\r\n");
				string eol = DominantNewline();
				text = text.Replace("\r\n", "\n").Replace("\n", eol);
				editor.Document.BeginUpdate();
				try
				{
					int start = editor.SelectionStart;
					editor.Document.Replace(start, editor.SelectionLength, text);
					editor.CaretOffset = start + text.Length;
					editor.TextArea.ClearSelection();
				}
				finally { editor.Document.EndUpdate(); }
				model.RaiseCharAdded('\n');
				editor.TextArea.Caret.BringCaretToView();
				e.Handled = true;
				return;
			}

			// Backspace in the indentation removes one level, Tab inserts one (Scintilla's tab/backspace indents)
			if(e.Key == Key.Back && e.KeyModifiers == KeyModifiers.None && editor.SelectionLength == 0 && !model.ReadOnly)
			{
				DocumentLine line = editor.Document.GetLineByOffset(editor.CaretOffset);
				int col = editor.CaretOffset - line.Offset;
				if(col > 0 && IsAllWhitespace(editor.Document.GetText(line.Offset, col)) && !editor.Document.GetText(line.Offset, col).Contains('\t'))
				{
					int remove = col % Math.Max(1, model.TabWidth);
					if(remove == 0) remove = Math.Min(col, Math.Max(1, model.TabWidth));
					editor.Document.Remove(editor.CaretOffset - remove, remove);
					e.Handled = true;
				}
			}
		}

		// Ctrl+/ toggle line comment, Alt+Up/Down move lines, Ctrl+D duplicate, Ctrl+Shift+K delete line,
		// Ctrl+Plus/Minus/0 zoom the text.
		private bool HandleEditorShortcut(KeyEventArgs e)
		{
			bool ctrl = (e.KeyModifiers & KeyModifiers.Control) != 0, alt = (e.KeyModifiers & KeyModifiers.Alt) != 0, shift = (e.KeyModifiers & KeyModifiers.Shift) != 0;

			if(ctrl && !alt && !shift && (e.Key == Key.OemPlus || e.Key == Key.Add)) { Zoom(1); return true; }
			if(ctrl && !alt && !shift && (e.Key == Key.OemMinus || e.Key == Key.Subtract)) { Zoom(-1); return true; }
			if(ctrl && !alt && !shift && (e.Key == Key.D0 || e.Key == Key.NumPad0)) { SetZoom(0); return true; }

			if(model.ReadOnly) return false;

			if(ctrl && !alt && !shift && (e.Key == Key.OemQuestion || e.Key == Key.Divide)) { ToggleComment(); return true; }
			if(alt && !ctrl && !shift && e.Key == Key.Up) { MoveLines(-1); return true; }
			if(alt && !ctrl && !shift && e.Key == Key.Down) { MoveLines(1); return true; }
			if(ctrl && !alt && !shift && e.Key == Key.D) { DuplicateLines(); return true; }
			if(ctrl && !alt && shift && e.Key == Key.K) { DeleteLines(); return true; }
			return false;
		}

		// The size from the settings and the user's zoom on top of it (kept apart: the settings' size is applied
		// again whenever the appearance is)
		private double basefontsize = 14;
		private double zoom;

		private void Zoom(int step) { SetZoom(zoom + step); }

		private void SetZoom(double value)
		{
			zoom = Math.Max(7 - basefontsize, Math.Min(48 - basefontsize, value));
			editor.FontSize = basefontsize + zoom;
			// Remembered, and the same in every open script
			if(UseSharedZoom) ULDB.Platform.UiOptions.ScriptZoom = (int)Math.Round(zoom);
		}

		private void OnSharedViewChanged()
		{
			if(UseSharedZoom) zoom = ULDB.Platform.UiOptions.ScriptZoom;
			ApplyAppearance();
		}

		private void SelectedLines(out DocumentLine first, out DocumentLine last)
		{
			int start = editor.SelectionStart, end = editor.SelectionStart + editor.SelectionLength;
			first = editor.Document.GetLineByOffset(start);
			// A selection that ends at the start of a line doesn't include that line
			last = editor.Document.GetLineByOffset(editor.SelectionLength > 0 && end > start && editor.Document.GetLineByOffset(end).Offset == end ? end - 1 : end);
		}

		private void ToggleComment()
		{
			SelectedLines(out DocumentLine first, out DocumentLine last);
			string marker = model.Lexer == Lexer.Perl ? "#" : "//";
			TextDocument doc = editor.Document;
			doc.BeginUpdate();
			try
			{
				// Uncomment when every non-empty line is commented
				bool allcommented = true;
				for(int n = first.LineNumber; n <= last.LineNumber; n++)
				{
					DocumentLine l = doc.GetLineByNumber(n);
					string text = doc.GetText(l.Offset, l.Length).TrimStart();
					if(text.Length > 0 && !text.StartsWith(marker)) { allcommented = false; break; }
				}
				for(int n = last.LineNumber; n >= first.LineNumber; n--)
				{
					DocumentLine l = doc.GetLineByNumber(n);
					string text = doc.GetText(l.Offset, l.Length);
					int indent = text.Length - text.TrimStart().Length;
					if(text.Trim().Length == 0) continue;
					if(allcommented)
					{
						int remove = marker.Length + (text.Length > indent + marker.Length && text[indent + marker.Length] == ' ' ? 1 : 0);
						doc.Remove(l.Offset + indent, remove);
					}
					else doc.Insert(l.Offset + indent, marker + " ");
				}
			}
			finally { doc.EndUpdate(); }
		}

		private void MoveLines(int direction)
		{
			SelectedLines(out DocumentLine first, out DocumentLine last);
			TextDocument doc = editor.Document;
			if(direction < 0 && first.LineNumber == 1) return;
			if(direction > 0 && last.LineNumber == doc.LineCount) return;

			DocumentLine other = doc.GetLineByNumber(direction < 0 ? first.LineNumber - 1 : last.LineNumber + 1);
			string eol = DominantNewline();
			int selstart = editor.SelectionStart - first.Offset, sellen = editor.SelectionLength;
			string block = doc.GetText(first.Offset, last.EndOffset - first.Offset);
			string neighbour = doc.GetText(other.Offset, other.Length);
			doc.BeginUpdate();
			try
			{
				if(direction < 0)
				{
					doc.Replace(other.Offset, last.EndOffset - other.Offset, block + eol + neighbour);
					editor.Select(other.Offset + selstart, sellen);
				}
				else
				{
					int newoffset = first.Offset + neighbour.Length + eol.Length;
					doc.Replace(first.Offset, other.EndOffset - first.Offset, neighbour + eol + block);
					editor.Select(newoffset + selstart, sellen);
				}
			}
			finally { doc.EndUpdate(); }
		}

		private void DuplicateLines()
		{
			SelectedLines(out DocumentLine first, out DocumentLine last);
			TextDocument doc = editor.Document;
			string eol = DominantNewline();
			string block = doc.GetText(first.Offset, last.EndOffset - first.Offset);
			int caret = editor.CaretOffset;
			doc.Insert(last.EndOffset, eol + block);
			editor.CaretOffset = caret + eol.Length + block.Length;
			editor.TextArea.ClearSelection();
		}

		private void DeleteLines()
		{
			SelectedLines(out DocumentLine first, out DocumentLine last);
			TextDocument doc = editor.Document;
			doc.Remove(first.Offset, last.NextLine != null ? last.NextLine.Offset - first.Offset : last.EndOffset - first.Offset);
		}

		private static bool IsPlainTyping(KeyEventArgs e)
		{
			// Letters, digits and symbols without Ctrl/Alt are typing, not shortcuts
			bool modified = (e.KeyModifiers & (KeyModifiers.Control | KeyModifiers.Alt | KeyModifiers.Meta)) != 0;
			if(modified) return false;
			return e.Key != Key.Tab && !(e.Key >= Key.F1 && e.Key <= Key.F24);
		}

		private static bool IsAllWhitespace(string s) { return s.All(c => c == ' ' || c == '\t'); }

		private string DominantNewline()
		{
			// The document's first line break (a script from Windows keeps its CR LF)
			DocumentLine first = model.Document.GetLineByNumber(1);
			if(first.DelimiterLength == 0) return "\n";
			return first.DelimiterLength == 2 ? "\r\n" : model.Document.GetText(first.EndOffset, 1);
		}

		#endregion

		#region ================== IScintillaHost

		int IScintillaHost.FirstVisibleLine
		{
			get { return (int)(editor.TextArea.TextView.VerticalOffset / Math.Max(1, editor.TextArea.TextView.DefaultLineHeight)); }
			set { editor.ScrollToVerticalOffset(Math.Max(0, value) * editor.TextArea.TextView.DefaultLineHeight); }
		}

		int IScintillaHost.LinesOnScreen { get { return (int)Math.Max(1, editor.TextArea.TextView.Bounds.Height / Math.Max(1, editor.TextArea.TextView.DefaultLineHeight)); } }

		int IScintillaHost.PointX(int position)
		{
			Point? p = editor.TextArea.TextView.GetVisualPosition(new TextViewPosition(editor.Document.GetLocation(Clamp(position))), VisualYPosition.TextTop) is Point v ? v : (Point?)null;
			return p.HasValue ? (int)(p.Value.X - editor.TextArea.TextView.HorizontalOffset) : 0;
		}

		int IScintillaHost.PointY(int position)
		{
			Point p = editor.TextArea.TextView.GetVisualPosition(new TextViewPosition(editor.Document.GetLocation(Clamp(position))), VisualYPosition.TextTop);
			return (int)(p.Y - editor.TextArea.TextView.VerticalOffset);
		}

		int IScintillaHost.PositionFromPoint(int x, int y)
		{
			TextViewPosition? p = editor.TextArea.TextView.GetPosition(new Point(x + editor.TextArea.TextView.HorizontalOffset, y + editor.TextArea.TextView.VerticalOffset));
			return p.HasValue ? editor.Document.GetOffset(p.Value.Location) : Scintilla.InvalidPosition;
		}

		void IScintillaHost.Focus() { editor.TextArea.Focus(); }

		void IScintillaHost.Clipboard(string action)
		{
			switch(action)
			{
				case "cut": editor.Cut(); break;
				case "copy": editor.Copy(); break;
				case "paste": editor.Paste(); break;
				case "selectall": editor.SelectAll(); break;
			}
		}

		bool IScintillaHost.CanPaste { get { return !model.ReadOnly; } }

		void IScintillaHost.GetSelection(out int anchor, out int caret)
		{
			caret = editor.CaretOffset;
			int start = editor.SelectionStart, length = editor.SelectionLength;
			if(length == 0) anchor = caret;
			else anchor = caret == start ? start + length : start;
		}

		void IScintillaHost.SetSelection(int anchor, int caret)
		{
			if(anchor == caret)
			{
				editor.TextArea.ClearSelection();
				editor.CaretOffset = caret;
			}
			else
			{
				// Select puts the caret at the end; only a backwards selection needs it moved
				editor.Select(Math.Min(anchor, caret), Math.Abs(anchor - caret));
				if(caret < anchor) editor.TextArea.Caret.Offset = caret;
			}
		}

		void IScintillaHost.ScrollToPosition(int position)
		{
			TextLocation loc = editor.Document.GetLocation(Clamp(position));
			editor.ScrollTo(loc.Line, loc.Column);
		}

		// Braces, highlighted words and error lines are drawn on a layer behind the text: only that is drawn again
		// (this runs on every caret move).
		void IScintillaHost.Refresh()
		{
			editor.TextArea.TextView.InvalidateLayer(KnownLayer.Selection);
			editor.TextArea.TextView.InvalidateLayer(KnownLayer.Background);
			markermargin.InvalidateVisual();
		}

		void IScintillaHost.Restyle()
		{
			editor.TextArea.TextView.Redraw();
		}

		private FoldingSection FoldAt(int line)
		{
			if(folding == null || line < 0 || line >= model.Document.LineCount) return null;
			int offset = model.Document.GetLineByNumber(line + 1).EndOffset;
			foreach(FoldingSection section in folding.GetFoldingsAt(offset)) return section;
			return null;
		}

		bool? IScintillaHost.IsFolded(int line) { return FoldAt(line)?.IsFolded; }

		bool IScintillaHost.SetFolded(int line, bool folded)
		{
			FoldingSection section = FoldAt(line);
			if(section == null) { ScheduleFolds(); return false; } // closed when the sections are built
			section.IsFolded = folded;
			return true;
		}

		void IScintillaHost.Unfold(int firstline, int lastline)
		{
			if(folding == null || model.Document.LineCount == 0) return;
			int from = model.Document.GetLineByNumber(Math.Max(1, Math.Min(firstline + 1, model.Document.LineCount))).Offset;
			int to = model.Document.GetLineByNumber(Math.Max(1, Math.Min(lastline + 1, model.Document.LineCount))).EndOffset;
			foreach(FoldingSection section in folding.AllFoldings)
				if(section.IsFolded && section.StartOffset < to && section.EndOffset > from) section.IsFolded = false;
		}

		private int Clamp(int position) { return Math.Max(0, Math.Min(position, editor.Document.TextLength)); }

		#endregion

		#region ================== Autocompletion

		bool IScintillaHost.AutoCompleteActive { get { return completion != null && completion.IsOpen; } }

		void IScintillaHost.CancelAutoComplete()
		{
			if(completion != null) { completion.Close(); completion = null; }
		}

		void IScintillaHost.ShowAutoComplete(int lenentered, string[] items)
		{
			if(completion != null) { completion.Close(); completion = null; }
			if(model.ReadOnly) return;

			completion = new CompletionWindow(editor.TextArea) { CloseWhenCaretAtBeginning = true, MaxHeight = 280, MinWidth = 220 };
			completion.StartOffset = Math.Max(0, editor.CaretOffset - lenentered);
			foreach(string item in items)
			{
				string name = item;
				int image = -1;
				int q = item.IndexOf('?');
				if(q > 0 && int.TryParse(item.Substring(q + 1), out int index)) { name = item.Substring(0, q); image = index; }
				completion.CompletionList.CompletionData.Add(new ScriptCompletion(name, image >= 0 && model.AutoCompleteImages.TryGetValue(image, out SD.Bitmap bmp) ? Convert(bmp) : null, model, WordInfo));
			}
			if(completion.CompletionList.CompletionData.Count == 0) { completion = null; return; }
			completion.Closed += (s, e) => completion = null;
			completion.Show();
		}

		private static Bitmap Convert(SD.Bitmap bitmap) { return AvaloniaImages.From(bitmap); }

		private sealed class ScriptCompletion : ICompletionData
		{
			private readonly Scintilla owner;
			private readonly Func<string, string> info;
			public ScriptCompletion(string text, Bitmap image, Scintilla owner, Func<string, string> info) { Text = text; Image = image; this.owner = owner; this.info = info; }
			public IImage Image { get; }
			public string Text { get; }
			public object Content { get { return Text; } }
			public object Description { get { string d = info?.Invoke(Text); return string.IsNullOrEmpty(d) ? null : d; } }
			public double Priority { get { return 0; } }
			public void Complete(TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs)
			{
				textArea.Document.Replace(completionSegment, Text);
				owner.RaiseAutoCCompleted(Text);
			}
		}

		#endregion

		#region ================== Call tips

		bool IScintillaHost.CallTipActive { get { return calltip.IsOpen; } }

		void IScintillaHost.CancelCallTip() { CancelCallTip(); }

		private void CancelCallTip() { calltip.IsOpen = false; }

		void IScintillaHost.ShowCallTip(int position, string text, int hlstart, int hlend)
		{
			calltiptext.Inlines?.Clear();
			text = text ?? "";
			hlstart = Math.Max(0, Math.Min(hlstart, text.Length));
			hlend = Math.Max(hlstart, Math.Min(hlend, text.Length));
			if(hlend > hlstart)
			{
				calltiptext.Inlines.Add(new Avalonia.Controls.Documents.Run(text.Substring(0, hlstart)));
				calltiptext.Inlines.Add(new Avalonia.Controls.Documents.Run(text.Substring(hlstart, hlend - hlstart)) { FontWeight = FontWeight.Bold, Foreground = Brushes.DodgerBlue });
				calltiptext.Inlines.Add(new Avalonia.Controls.Documents.Run(text.Substring(hlend)));
			}
			else calltiptext.Text = text;

			Point p = editor.TextArea.TextView.GetVisualPosition(new TextViewPosition(editor.Document.GetLocation(Clamp(position))), VisualYPosition.LineBottom);
			Point origin = editor.TextArea.TextView.TranslatePoint(new Point(p.X - editor.TextArea.TextView.HorizontalOffset, p.Y - editor.TextArea.TextView.VerticalOffset), editor) ?? new Point();
			calltip.PlacementRect = new Rect(origin.X, origin.Y, 1, 1);
			calltip.IsOpen = true;
		}

		#endregion
	}

	/// <summary>Colours the text with the model's lexer styles.</summary>
	internal sealed class ScriptColorizer : DocumentColorizingTransformer
	{
		private readonly Scintilla model;
		private readonly Dictionary<int, IBrush> brushes = new Dictionary<int, IBrush>();
		private int cachedfor = -1;

		public ScriptColorizer(Scintilla model) { this.model = model; }

		/// <summary>Colours by style number overriding the model's styles (the dark default scheme), or null.</summary>
		public Dictionary<int, IBrush> Palette { get; set; }

		/// <summary>Comments in italics (a ULDB preference).</summary>
		public bool ItalicComments { get; set; } = true;

		private IBrush BrushFor(int style)
		{
			if(Palette != null) return Palette.TryGetValue(style, out IBrush p) ? p : null;
			if(!brushes.TryGetValue(style, out IBrush brush))
			{
				SD.Color c = model.Styles[style].ForeColor;
				if(c.IsEmpty) c = model.Styles[Style.Default].ForeColor;
				brush = c.IsEmpty ? null : new SolidColorBrush(Color.FromArgb(c.A, c.R, c.G, c.B));
				brushes[style] = brush;
			}
			return brush;
		}

		protected override void ColorizeLine(DocumentLine line)
		{
			if(line.Length == 0) return;
			byte[] styles = model.Styled();
			int offset = line.Offset, end = line.Offset + line.Length;
			int runstart = offset;
			while(runstart < end)
			{
				int style = runstart < styles.Length ? styles[runstart] : 0;
				int runend = runstart + 1;
				while(runend < end && (runend < styles.Length ? styles[runend] : 0) == style) runend++;
				IBrush brush = BrushFor(style);
				bool italic = ItalicComments && (style == ScriptLexer.BlockComment || style == ScriptLexer.LineComment);
				if(brush != null || italic)
				{
					ChangeLinePart(runstart, runend, element =>
					{
						if(brush != null) element.TextRunProperties.SetForegroundBrush(brush);
						if(italic) element.TextRunProperties.SetTypeface(new Typeface(element.TextRunProperties.Typeface.FontFamily, FontStyle.Italic, element.TextRunProperties.Typeface.Weight));
					});
				}
				runstart = runend;
			}
		}

		public void Reset() { brushes.Clear(); }
	}

	/// <summary>Draws the matching brace, the highlighted words and the error lines behind the text.</summary>
	internal sealed class HighlightRenderer : IBackgroundRenderer
	{
		private readonly Scintilla model;
		private readonly TextEditor editor;
		public HighlightRenderer(Scintilla model, TextEditor editor) { this.model = model; this.editor = editor; }
		public KnownLayer Layer { get { return KnownLayer.Selection; } }

		public void Draw(TextView textView, DrawingContext drawingContext)
		{
			if(textView.Document == null || !textView.VisualLinesValid) return;

			// Highlighted word occurrences (indicator 8)
			Indicator ind = model.Indicators[8];
			SD.Color ic = ind.ForeColor.IsEmpty ? SD.Color.Orange : ind.ForeColor;
			IBrush fill = new SolidColorBrush(Color.FromArgb((byte)Math.Max(40, ind.Alpha * 3), ic.R, ic.G, ic.B));
			Pen outline = new Pen(new SolidColorBrush(Color.FromArgb(160, ic.R, ic.G, ic.B)), 1);
			int visiblestart = textView.VisualLines.Count > 0 ? textView.VisualLines[0].FirstDocumentLine.Offset : 0;
			int visibleend = textView.VisualLines.Count > 0 ? textView.VisualLines[textView.VisualLines.Count - 1].LastDocumentLine.EndOffset : 0;
			foreach(int[] r in model.IndicatorRanges(8).ToList())
				if(r[1] >= visiblestart && r[0] <= visibleend) DrawRange(textView, drawingContext, r[0], r[1], fill, outline);

			// Braces
			if(model.BraceHighlight1 != Scintilla.InvalidPosition)
			{
				Style style = model.BraceIsBad ? model.Styles[Style.BraceBad] : model.Styles[Style.BraceLight];
				SD.Color c = style.BackColor.IsEmpty ? (model.BraceIsBad ? SD.Color.Red : SD.Color.Gray) : style.BackColor;
				IBrush brush = new SolidColorBrush(Color.FromArgb(c.A == 0 ? (byte)255 : c.A, c.R, c.G, c.B));
				DrawRange(textView, drawingContext, model.BraceHighlight1, model.BraceHighlight1 + 1, brush, null);
				if(model.BraceHighlight2 != Scintilla.InvalidPosition) DrawRange(textView, drawingContext, model.BraceHighlight2, model.BraceHighlight2 + 1, brush, null);
			}

			// Error lines
			foreach(KeyValuePair<int, HashSet<int>> m in model.LineMarkers)
			{
				if(!m.Value.Contains(2) || m.Key >= textView.Document.LineCount) continue;
				DocumentLine line = textView.Document.GetLineByNumber(m.Key + 1);
				DrawRange(textView, drawingContext, line.Offset, Math.Max(line.EndOffset, line.Offset + 1), new SolidColorBrush(Color.FromArgb(46, 232, 60, 60)), null, true);
				DrawErrorUnderline(textView, drawingContext, line);
			}
		}

		// A red zigzag under the text of a line with a compile error
		private static void DrawErrorUnderline(TextView textView, DrawingContext dc, DocumentLine line)
		{
			string text = textView.Document.GetText(line.Offset, line.Length);
			int first = text.Length - text.TrimStart().Length, last = text.TrimEnd().Length;
			if(last <= first) return;
			TextSegment segment = new TextSegment { StartOffset = line.Offset + first, EndOffset = line.Offset + last };
			Pen pen = new Pen(new SolidColorBrush(Color.FromRgb(232, 60, 60)), 1);
			foreach(Rect r in BackgroundGeometryBuilder.GetRectsForSegment(textView, segment))
			{
				StreamGeometry zigzag = new StreamGeometry();
				using(StreamGeometryContext ctx = zigzag.Open())
				{
					double y = r.Bottom - 1;
					ctx.BeginFigure(new Point(r.Left, y), false);
					bool up = true;
					for(double x = r.Left + 2; x <= r.Right; x += 2) { ctx.LineTo(new Point(x, up ? y - 2 : y)); up = !up; }
					ctx.EndFigure(false);
				}
				dc.DrawGeometry(null, pen, zigzag);
			}
		}

		private static void DrawRange(TextView textView, DrawingContext dc, int start, int end, IBrush fill, Pen outline, bool wholeline = false)
		{
			if(end <= start || start < 0 || end > textView.Document.TextLength + 1) return;
			TextSegment segment = new TextSegment { StartOffset = start, EndOffset = Math.Min(end, textView.Document.TextLength) };
			BackgroundGeometryBuilder builder = new BackgroundGeometryBuilder { AlignToWholePixels = true, CornerRadius = 2 };
			builder.AddSegment(textView, segment);
			Geometry geometry = builder.CreateGeometry();
			if(geometry != null) dc.DrawGeometry(fill, outline, geometry);
		}
	}

	/// <summary>The margin left of the line numbers: the error markers (upstream's ScriptError image).</summary>
	internal sealed class MarkerMargin : AbstractMargin
	{
		private readonly Scintilla model;
		private readonly TextEditor editor;

		public MarkerMargin(Scintilla model, TextEditor editor)
		{
			this.model = model;
			this.editor = editor;
			Width = 18;
		}

		protected override Size MeasureOverride(Size availableSize) { return new Size(18, 0); }

		public override void Render(DrawingContext context)
		{
			TextView textView = TextView;
			if(textView == null || !textView.VisualLinesValid) return;
			IBrush brush = new SolidColorBrush(Color.FromRgb(232, 60, 60));
			foreach(VisualLine visual in textView.VisualLines)
			{
				int line = visual.FirstDocumentLine.LineNumber - 1;
				bool error = false;
				foreach(KeyValuePair<int, HashSet<int>> m in model.LineMarkers) if(m.Key == line && m.Value.Contains(2)) { error = true; break; }
				if(!error) continue;
				double y = visual.GetTextLineVisualYPosition(visual.TextLines[0], VisualYPosition.TextTop) - textView.VerticalOffset;
				double h = visual.Height > 0 ? Math.Min(visual.Height, 22) : 16;
				context.DrawEllipse(brush, null, new Point(9, y + h / 2), 5, 5);
			}
		}
	}
}
