// ULDB: ScintillaNET's Scintilla control as a model: the script text (an AvaloniaEdit document),
// the selection, styles, margins, markers, indicators, folds, search and the like, with the members
// upstream's script editor code calls (ScriptEditorControl, ScriptHandler, the document tabs and
// the panel). The visible editor is ScriptEditorView (AvaloniaEdit), which attaches through
// IScintillaHost and draws what is stored here. Everything works without a view (tests).

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using AvaloniaEdit.Document;
using ULDB.Scripting;

namespace ScintillaNET
{
	/// <summary>What the editor view provides to the model (screen-dependent things).</summary>
	internal interface IScintillaHost
	{
		int FirstVisibleLine { get; set; }
		int LinesOnScreen { get; }
		int PointX(int position);
		int PointY(int position);
		int PositionFromPoint(int x, int y);
		void Focus();
		void Clipboard(string action); // "cut", "copy", "paste", "selectall"
		bool CanPaste { get; }
		void ShowAutoComplete(int lenentered, string[] items);
		void CancelAutoComplete();
		bool AutoCompleteActive { get; }
		void ShowCallTip(int position, string text, int highlightstart, int highlightend);
		void CancelCallTip();
		bool CallTipActive { get; }
		void GetSelection(out int anchor, out int caret);
		void SetSelection(int anchor, int caret);
		void ScrollToPosition(int position);
		/// <summary>Marks drawn behind or beside the text changed (indicators, markers, braces): a cheap redraw.</summary>
		void Refresh();
		/// <summary>The text's colours changed (lexer, keywords): the lines are built again.</summary>
		void Restyle();
		/// <summary>Whether the fold starting on the line is closed; null when the view has no fold there (yet).</summary>
		bool? IsFolded(int line);
		/// <summary>Closes or opens the fold starting on the line; false when the view has no fold there (yet).</summary>
		bool SetFolded(int line, bool folded);
		/// <summary>Opens every fold that hides one of the lines.</summary>
		void Unfold(int firstline, int lastline);
	}

	public class Scintilla : UserControl
	{
		public const int InvalidPosition = -1;

		internal readonly TextDocument Document = new TextDocument();
		private IScintillaHost host;

		// Selection when no view is attached
		private int anchor, caret;

		// Search target
		private int targetstart, targetend;

		// Highlighting
		private Lexer lexer = Lexer.Null;
		private readonly Dictionary<int, HashSet<string>> keywordsets = new Dictionary<int, HashSet<string>>();
		private bool casesensitive = true;
		private byte[] stylecache;
		private int stylecacheversion = -1, docversion;
		private string wordchars = "";

		// Marks
		private readonly Dictionary<int, HashSet<int>> markers = new Dictionary<int, HashSet<int>>();
		private readonly Dictionary<int, List<int[]>> indicatorranges = new Dictionary<int, List<int[]>>();
		private int bracehighlight1 = InvalidPosition, bracehighlight2 = InvalidPosition;
		private bool bracebad;
		private readonly HashSet<int> folded = new HashSet<int>();
		private int updategroup;
		private readonly Dictionary<int, Bitmap> autocompleteimages = new Dictionary<int, Bitmap>();

		public Scintilla()
		{
			Lines = new LineCollection(this);
			Styles = new StyleCollection();
			Styles.StyleChanged += ScheduleChanged;
			Margins = new MarginCollection();
			Margins.Changed += ScheduleChanged;
			Markers = new MarkerCollection();
			Indicators = new IndicatorCollection();
			Document.Changed += OnDocumentChanged;
			// TextChanged after the change is complete and recorded for undo (Modified, CanUndo are right then)
			Document.UpdateFinished += (s, e) => OnTextChanged(EventArgs.Empty);
			Document.UndoStack.SizeLimit = 1000;
		}

		#region ================== Host

		internal IScintillaHost Host { get { return host; } set { host = value; } }

		/// <summary>Raised when the view should redraw styles/indicators/markers.</summary>
		internal event Action AppearanceChanged;

		private bool changedscheduled;

		// Styles and options (fonts, colours, tabs, wrapping, margins) arrive one property at a time (a colour preset sets
		// dozens): the view takes them over once, after them. This is the expensive path; marks that change while the
		// caret moves (braces, highlighted words) go through MarksChanged instead.
		private void ScheduleChanged()
		{
			if(changedscheduled) return;
			changedscheduled = true;
			Avalonia.Threading.Dispatcher.UIThread.Post(() => { changedscheduled = false; AppearanceChanged?.Invoke(); }, Avalonia.Threading.DispatcherPriority.Background);
		}

		private void MarksChanged() { host?.Refresh(); }
		private void TextStyleChanged() { host?.Restyle(); }

		internal IDictionary<int, Bitmap> AutoCompleteImages { get { return autocompleteimages; } }
		internal IEnumerable<KeyValuePair<int, HashSet<int>>> LineMarkers { get { return markers; } }
		internal IEnumerable<int[]> IndicatorRanges(int indicator) { return indicatorranges.TryGetValue(indicator, out List<int[]> l) ? l : (IEnumerable<int[]>)new int[0][]; }
		internal int BraceHighlight1 { get { return bracehighlight1; } }
		internal int BraceHighlight2 { get { return bracehighlight2; } }
		internal bool BraceIsBad { get { return bracebad; } }
		internal HashSet<int> FoldedLines { get { return folded; } }
		internal bool CaseSensitive { get { return casesensitive; } }

		#endregion

		#region ================== Events

		public event EventHandler<CharAddedEventArgs> CharAdded;
		public event EventHandler<AutoCSelectionEventArgs> AutoCCompleted;
		public event EventHandler<InsertCheckEventArgs> InsertCheck;
		public event EventHandler<UpdateUIEventArgs> UpdateUI;

		internal void RaiseCharAdded(char c) { CharAdded?.Invoke(this, new CharAddedEventArgs { Char = c }); }
		internal void RaiseUpdateUI() { UpdateUI?.Invoke(this, new UpdateUIEventArgs()); }
		internal void RaiseAutoCCompleted(string text) { AutoCCompleted?.Invoke(this, new AutoCSelectionEventArgs { Text = text }); }

		/// <summary>The text about to be inserted when the user types or pastes (handlers may change it).</summary>
		internal string RaiseInsertCheck(string text)
		{
			InsertCheckEventArgs e = new InsertCheckEventArgs { Text = text };
			InsertCheck?.Invoke(this, e);
			return e.Text;
		}

		private void OnDocumentChanged(object sender, DocumentChangeEventArgs e)
		{
			docversion++;
			// Without a view nothing tracks the folds (the view's fold sections follow edits by themselves)
			if(host == null && e.InsertionLength != e.RemovalLength) folded.Clear();
			ShiftMarks(e);
			// Keep the stored selection valid when there's no view
			if(host == null)
			{
				anchor = AdjustOffset(anchor, e);
				caret = AdjustOffset(caret, e);
			}
		}

		// Like Scintilla: line markers (compile errors) and indicator ranges (highlighted words) stay on their text
		private void ShiftMarks(DocumentChangeEventArgs e)
		{
			foreach(List<int[]> ranges in indicatorranges.Values)
			{
				for(int i = ranges.Count - 1; i >= 0; i--)
				{
					int[] r = ranges[i];
					if(r[1] <= e.Offset) continue;
					r[0] = AdjustOffset(r[0], e);
					r[1] = r[1] >= e.Offset + e.RemovalLength ? r[1] - e.RemovalLength + e.InsertionLength : e.Offset;
					if(r[1] <= r[0]) ranges.RemoveAt(i);
				}
			}

			if(markers.Count == 0 && folded.Count == 0) return;
			int delta = CountLineBreaks(e.InsertedText?.Text) - CountLineBreaks(e.RemovedText?.Text);
			if(delta == 0) return;
			// The change began on this line (as it is now, when lines were added: the first of them)
			int changeline = Document.GetLineByOffset(Math.Min(e.Offset, Document.TextLength)).LineNumber - 1;
			// A change at the very start of a line moves that line too
			bool atlinestart = Document.GetLineByOffset(Math.Min(e.Offset, Document.TextLength)).Offset == e.Offset;
			int Moved(int line)
			{
				if(line > changeline || (line == changeline && atlinestart && delta > 0)) return Math.Max(changeline, line + delta);
				return line;
			}

			Dictionary<int, HashSet<int>> moved = new Dictionary<int, HashSet<int>>();
			foreach(KeyValuePair<int, HashSet<int>> m in markers)
			{
				int target = Moved(m.Key);
				if(moved.TryGetValue(target, out HashSet<int> existing)) existing.UnionWith(m.Value); else moved[target] = m.Value;
			}
			markers.Clear();
			foreach(KeyValuePair<int, HashSet<int>> m in moved) markers[m.Key] = m.Value;

			// Folds asked for that the view hasn't made sections for yet
			if(folded.Count > 0)
			{
				List<int> lines = new List<int>(folded);
				folded.Clear();
				foreach(int line in lines) folded.Add(Moved(line));
			}
		}

		private static int CountLineBreaks(string text)
		{
			if(string.IsNullOrEmpty(text)) return 0;
			int count = 0;
			for(int i = 0; i < text.Length; i++)
			{
				if(text[i] == '\n') count++;
				else if(text[i] == '\r' && (i + 1 >= text.Length || text[i + 1] != '\n')) count++;
			}
			return count;
		}

		private static int AdjustOffset(int offset, DocumentChangeEventArgs e)
		{
			if(offset > e.Offset) offset = Math.Max(e.Offset, offset - e.RemovalLength) + (offset >= e.Offset + e.RemovalLength ? e.InsertionLength : 0);
			return offset;
		}

		#endregion

		#region ================== Text and lines

		public override string Text
		{
			get { return Document.Text; }
			set { Document.Text = (value ?? "").Replace("\0", ""); }
		}

		public int TextLength { get { return Document.TextLength; } }

		public int GetCharAt(int position)
		{
			return (position < 0 || position >= Document.TextLength) ? 0 : Document.GetCharAt(position);
		}

		public string GetTextRange(int position, int length)
		{
			position = Math.Max(0, Math.Min(position, Document.TextLength));
			length = Math.Max(0, Math.Min(length, Document.TextLength - position));
			return Document.GetText(position, length);
		}

		public LineCollection Lines { get; private set; }
		internal int LineCount { get { return Document.LineCount; } }
		internal int LineStart(int index) { return Document.GetLineByNumber(index + 1).Offset; }
		internal int LineEndWithBreak(int index) { DocumentLine l = Document.GetLineByNumber(index + 1); return l.Offset + l.TotalLength; }

		public int LineFromPosition(int position)
		{
			position = Math.Max(0, Math.Min(position, Document.TextLength));
			return Document.GetLineByOffset(position).LineNumber - 1;
		}

		public int CurrentLine { get { return LineFromPosition(CurrentPosition); } }

		/// <summary>Display rows the line wraps to beyond its first (0 when it isn't wrapped).</summary>
		internal int WrapCountOf(int line)
		{
			if(host == null) return 0;
			DocumentLine l = Document.GetLineByNumber(line + 1);
			return host.PointY(l.EndOffset) > host.PointY(l.Offset) ? 1 : 0;
		}

		internal int FoldLevelOf(int line) { return 0; }

		public string WordChars
		{
			get { return wordchars; }
			set { wordchars = value ?? ""; docversion++; }
		}

		private bool IsWordChar(char c) { return char.IsLetterOrDigit(c) || c == '_' || wordchars.IndexOf(c) != -1; }

		public int WordStartPosition(int position, bool onlyWordCharacters)
		{
			position = Math.Max(0, Math.Min(position, Document.TextLength));
			if(onlyWordCharacters)
			{
				while(position > 0 && IsWordChar(Document.GetCharAt(position - 1))) position--;
			}
			else
			{
				while(position > 0 && !char.IsWhiteSpace(Document.GetCharAt(position - 1))) position--;
			}
			return position;
		}

		public int WordEndPosition(int position, bool onlyWordCharacters)
		{
			position = Math.Max(0, Math.Min(position, Document.TextLength));
			if(onlyWordCharacters)
			{
				while(position < Document.TextLength && IsWordChar(Document.GetCharAt(position))) position++;
			}
			else
			{
				while(position < Document.TextLength && !char.IsWhiteSpace(Document.GetCharAt(position))) position++;
			}
			return position;
		}

		public string GetWordFromPosition(int position)
		{
			int start = WordStartPosition(position, true), end = WordEndPosition(position, true);
			return GetTextRange(start, end - start);
		}

		private int tabwidthvalue = 4;
		public int TabWidth { get { return tabwidthvalue; } set { if(!Equals(tabwidthvalue, value)) { tabwidthvalue = value; ScheduleChanged(); } } }
		private bool usetabsvalue = true;
		public bool UseTabs { get { return usetabsvalue; } set { if(!Equals(usetabsvalue, value)) { usetabsvalue = value; ScheduleChanged(); } } }

		internal int GetIndentation(int line)
		{
			DocumentLine l = Document.GetLineByNumber(line + 1);
			int column = 0;
			for(int i = l.Offset; i < l.Offset + l.Length; i++)
			{
				char c = Document.GetCharAt(i);
				if(c == ' ') column++;
				else if(c == '\t') column += TabWidth - (column % TabWidth);
				else break;
			}
			return column;
		}

		internal void SetIndentation(int line, int indentation)
		{
			indentation = Math.Max(0, indentation);
			DocumentLine l = Document.GetLineByNumber(line + 1);
			int end = l.Offset;
			while(end < l.Offset + l.Length && (Document.GetCharAt(end) == ' ' || Document.GetCharAt(end) == '\t')) end++;
			StringBuilder sb = new StringBuilder();
			if(UseTabs && TabWidth > 0) sb.Append('\t', indentation / TabWidth).Append(' ', indentation % TabWidth);
			else sb.Append(' ', indentation);
			Document.Replace(l.Offset, end - l.Offset, sb.ToString());
		}

		#endregion

		#region ================== Selection and caret

		private void GetSelection(out int a, out int c)
		{
			if(host != null) host.GetSelection(out a, out c);
			else { a = anchor; c = caret; }
			int len = Document.TextLength;
			a = Math.Max(0, Math.Min(a, len));
			c = Math.Max(0, Math.Min(c, len));
		}

		private void SetSelection(int a, int c)
		{
			int len = Document.TextLength;
			a = Math.Max(0, Math.Min(a, len));
			c = Math.Max(0, Math.Min(c, len));
			anchor = a; caret = c;
			host?.SetSelection(a, c);
		}

		public int CurrentPosition
		{
			get { GetSelection(out _, out int c); return c; }
			set { GetSelection(out int a, out _); SetSelection(a, value); }
		}

		public int SelectionStart
		{
			get { GetSelection(out int a, out int c); return Math.Min(a, c); }
			set { int end = Math.Max(SelectionEnd, value); SetSelection(value, end); }
		}

		public int SelectionEnd
		{
			get { GetSelection(out int a, out int c); return Math.Max(a, c); }
			set { int start = Math.Min(SelectionStart, value); SetSelection(start, value); }
		}

		public string SelectedText { get { int s = SelectionStart; return GetTextRange(s, SelectionEnd - s); } }

		public void SetEmptySelection(int pos) { SetSelection(pos, pos); }
		public void GotoPosition(int position) { SetSelection(position, position); host?.ScrollToPosition(position); }

		public void SelectAll() { if(host != null) host.Clipboard("selectall"); else SetSelection(0, Document.TextLength); }

		public void ReplaceSelection(string text)
		{
			int s = SelectionStart, e = SelectionEnd;
			Document.Replace(s, e - s, text ?? "");
			SetEmptySelection(s + (text ?? "").Length);
		}

		public void InsertText(int position, string text)
		{
			if(position < 0) position = CurrentPosition;
			position = Math.Min(position, Document.TextLength);
			GetSelection(out int a, out int c);
			Document.Insert(position, text ?? "");
			int len = (text ?? "").Length;
			// Like Scintilla: a selection after the insertion point moves with the text
			if(position < a || (position == a && a != c)) a += len;
			if(position < c) c += len;
			SetSelection(a, c);
		}

		public void DeleteRange(int position, int length)
		{
			position = Math.Max(0, Math.Min(position, Document.TextLength));
			length = Math.Max(0, Math.Min(length, Document.TextLength - position));
			GetSelection(out int a, out int c);
			Document.Remove(position, length);
			a = a > position ? Math.Max(position, a - length) : a;
			c = c > position ? Math.Max(position, c - length) : c;
			SetSelection(a, c);
		}

		#endregion

		#region ================== View

		public int FirstVisibleLine { get { return host != null ? host.FirstVisibleLine : 0; } set { if(host != null) host.FirstVisibleLine = value; } }
		public int LinesOnScreen { get { return host != null ? host.LinesOnScreen : 40; } }
		public int PointXFromPosition(int pos) { return host != null ? host.PointX(pos) : 0; }
		public int PointYFromPosition(int pos) { return host != null ? host.PointY(pos) : 0; }
		public int CharPositionFromPointClose(int x, int y) { return host != null ? host.PositionFromPoint(x, y) : InvalidPosition; }
		public int TextWidth(int style, string text) { return (text ?? "").Length * 9; }

		public void ShowLines(int lineStart, int lineEnd)
		{
			// Unfold what hides the lines
			foreach(int start in new List<int>(folded))
			{
				if(GetFoldEnd(start, out int end) && start < lineEnd && end >= lineStart) folded.Remove(start);
			}
			host?.Unfold(lineStart, lineEnd);
		}

		public new bool Focus() { host?.Focus(); return host != null; }

		private bool readonlyvalue;
		public bool ReadOnly { get { return readonlyvalue; } set { if(!Equals(readonlyvalue, value)) { readonlyvalue = value; ScheduleChanged(); } } }
		public int CaretWidth { get; set; } = 1;
		public int ExtraAscent { get; set; }
		public int ExtraDescent { get; set; }
		public FontQuality FontQuality { get; set; }
		public int ScrollWidth { get; set; } = 2000;
		public int WhitespaceSize { get; set; } = 1;
		private Color caretforecolorvalue;
		public Color CaretForeColor { get { return caretforecolorvalue; } set { if(!Equals(caretforecolorvalue, value)) { caretforecolorvalue = value; ScheduleChanged(); } } }
		public int CaretPeriod { get; set; }
		private WhitespaceMode viewwhitespacevalue;
		public WhitespaceMode ViewWhitespace { get { return viewwhitespacevalue; } set { if(!Equals(viewwhitespacevalue, value)) { viewwhitespacevalue = value; ScheduleChanged(); } } }
		private WrapMode wrapmodevalue;
		public WrapMode WrapMode { get { return wrapmodevalue; } set { if(!Equals(wrapmodevalue, value)) { wrapmodevalue = value; ScheduleChanged(); } } }
		private AutomaticFold automaticfoldvalue;
		public AutomaticFold AutomaticFold { get { return automaticfoldvalue; } set { if(!Equals(automaticfoldvalue, value)) { automaticfoldvalue = value; ScheduleChanged(); } } }
		public SearchFlags SearchFlags { get; set; }
		public new BorderStyle BorderStyle { get; set; }
		public StyleCollection Styles { get; private set; }
		public MarginCollection Margins { get; private set; }
		public MarkerCollection Markers { get; private set; }
		public IndicatorCollection Indicators { get; private set; }

		#endregion

		#region ================== Undo and clipboard

		public bool Modified { get { return !Document.UndoStack.IsOriginalFile; } }
		public bool CanUndo { get { return Document.UndoStack.CanUndo; } }
		public bool CanRedo { get { return Document.UndoStack.CanRedo; } }
		public bool CanPaste { get { return host == null || host.CanPaste; } }
		public void SetSavePoint() { Document.UndoStack.MarkAsOriginalFile(); }
		public void EmptyUndoBuffer() { Document.UndoStack.ClearAll(); Document.UndoStack.MarkAsOriginalFile(); }
		public void Undo() { if(Document.UndoStack.CanUndo) Document.UndoStack.Undo(); }
		public void Redo() { if(Document.UndoStack.CanRedo) Document.UndoStack.Redo(); }

		public void BeginUndoAction() { if(updategroup++ == 0) Document.BeginUpdate(); }
		public void EndUndoAction() { if(updategroup > 0 && --updategroup == 0) Document.EndUpdate(); }

		public void Cut() { host?.Clipboard("cut"); }
		public void Copy() { host?.Clipboard("copy"); }
		public void Paste() { host?.Clipboard("paste"); }

		#endregion

		#region ================== Search

		public int TargetStart { get { return targetstart; } set { targetstart = value; } }
		public int TargetEnd { get { return targetend; } set { targetend = value; } }

		/// <summary>Searches the target (backwards when the end is before the start); the target becomes the match.</summary>
		public int SearchInTarget(string text)
		{
			if(string.IsNullOrEmpty(text)) return -1;
			StringComparison comparison = (SearchFlags & SearchFlags.MatchCase) != 0 ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
			bool whole = (SearchFlags & SearchFlags.WholeWord) != 0;
			string doc = Document.Text;
			int from = Math.Max(0, Math.Min(targetstart, doc.Length)), to = Math.Max(0, Math.Min(targetend, doc.Length));

			if(to >= from)
			{
				int pos = from;
				while(pos <= to - text.Length)
				{
					int found = doc.IndexOf(text, pos, to - pos, comparison);
					if(found < 0) return -1;
					if(!whole || IsWholeWord(doc, found, text.Length)) return Found(found, text.Length);
					pos = found + 1;
				}
				return -1;
			}

			// Backwards: matches that start between the end and the start
			for(int pos = Math.Min(from, doc.Length - text.Length); pos >= to; pos--)
			{
				if(string.Compare(doc, pos, text, 0, text.Length, comparison) == 0 && (!whole || IsWholeWord(doc, pos, text.Length)))
					return Found(pos, text.Length);
			}
			return -1;
		}

		private int Found(int start, int length) { targetstart = start; targetend = start + length; return start; }

		private bool IsWholeWord(string doc, int start, int length)
		{
			if(start > 0 && IsWordChar(doc[start - 1])) return false;
			if(start + length < doc.Length && IsWordChar(doc[start + length])) return false;
			return true;
		}

		#endregion

		#region ================== Markers, indicators, braces and folds

		internal void AddMarker(int line, int marker)
		{
			if(!markers.TryGetValue(line, out HashSet<int> set)) markers[line] = set = new HashSet<int>();
			set.Add(marker);
			MarksChanged();
		}

		public void MarkerDeleteAll(int marker)
		{
			foreach(HashSet<int> set in markers.Values) set.Remove(marker);
			MarksChanged();
		}

		public int IndicatorCurrent { get; set; }

		public void IndicatorClearRange(int position, int length)
		{
			if(!indicatorranges.TryGetValue(IndicatorCurrent, out List<int[]> ranges)) return;
			if(position <= 0 && length >= Document.TextLength) ranges.Clear();
			else ranges.RemoveAll(r => r[0] < position + length && r[1] > position);
			MarksChanged();
		}

		public void IndicatorFillRange(int position, int length)
		{
			if(!indicatorranges.TryGetValue(IndicatorCurrent, out List<int[]> ranges)) indicatorranges[IndicatorCurrent] = ranges = new List<int[]>();
			ranges.Add(new[] { position, position + length });
			MarksChanged();
		}

		public void BraceHighlight(int position1, int position2) { bracehighlight1 = position1; bracehighlight2 = position2; bracebad = false; MarksChanged(); }
		public void BraceBadLight(int position) { bracehighlight1 = position; bracehighlight2 = InvalidPosition; bracebad = true; MarksChanged(); }

		/// <summary>The position of the brace matching the one at <paramref name="position"/>, or InvalidPosition.</summary>
		public int BraceMatch(int position)
		{
			char c = (char)GetCharAt(position);
			int dir; char other;
			switch(c)
			{
				case '(': dir = 1; other = ')'; break;
				case '[': dir = 1; other = ']'; break;
				case '{': dir = 1; other = '}'; break;
				case ')': dir = -1; other = '('; break;
				case ']': dir = -1; other = '['; break;
				case '}': dir = -1; other = '{'; break;
				default: return InvalidPosition;
			}

			byte[] styles = Styled();
			byte own = position < styles.Length ? styles[position] : (byte)0;
			int depth = 0;
			for(int i = position; i >= 0 && i < Document.TextLength; i += dir)
			{
				byte s = i < styles.Length ? styles[i] : (byte)0;
				// Like Scintilla: only braces of the same style match (code with code, a string's with that string)
				if(s != own) continue;
				char ch = Document.GetCharAt(i);
				if(ch == c) depth++;
				else if(ch == other && --depth == 0) return i;
			}
			return InvalidPosition;
		}

		/// <summary>Start line to end line of every foldable block (braces and block comments spanning lines).</summary>
		internal List<int[]> GetFoldRegions()
		{
			List<int[]> regions = new List<int[]>();
			byte[] styles = Styled();
			Stack<int> open = new Stack<int>();
			string text = Document.Text;
			for(int i = 0; i < text.Length; i++)
			{
				byte s = i < styles.Length ? styles[i] : (byte)0;
				if(s == ScriptLexer.BlockComment && (i == 0 || styles[i - 1] != ScriptLexer.BlockComment))
				{
					int end = i;
					while(end < styles.Length && styles[end] == ScriptLexer.BlockComment) end++;
					int a = LineFromPosition(i), b = LineFromPosition(end - 1);
					if(b > a) regions.Add(new[] { a, b });
					i = end - 1;
					continue;
				}
				if(s == ScriptLexer.Operator)
				{
					if(text[i] == '{') open.Push(LineFromPosition(i));
					else if(text[i] == '}' && open.Count > 0)
					{
						int a = open.Pop(), b = LineFromPosition(i);
						if(b > a) regions.Add(new[] { a, b });
					}
				}
			}
			regions.Sort((x, y) => x[0] != y[0] ? x[0].CompareTo(y[0]) : y[1].CompareTo(x[1]));
			return regions;
		}

		private bool GetFoldEnd(int startline, out int endline)
		{
			foreach(int[] r in GetFoldRegions()) if(r[0] == startline) { endline = r[1]; return true; }
			endline = -1;
			return false;
		}

		// The view's fold sections are what counts once they exist (the user folds by clicking there, and they follow
		// edits). The set here holds folds asked for before that: the view closes them when it builds its sections.
		internal bool IsFolded(int line) { return host?.IsFolded(line) ?? folded.Contains(line); }

		internal void SetFolded(int line, bool fold)
		{
			if(fold && GetFoldEnd(line, out _)) folded.Add(line);
			else folded.Remove(line);
			host?.SetFolded(line, fold);
		}

		#endregion

		#region ================== Styles and keywords

		public Lexer Lexer { get { return lexer; } set { lexer = value; docversion++; TextStyleChanged(); } }

		public void ClearDocumentStyle() { docversion++; }
		public void StyleResetDefault() { }
		public void StyleClearAll() { ScheduleChanged(); }
		public void SetProperty(string name, string value) { }
		public void SetWhitespaceBackColor(bool use, Color color) { }
		public void SetWhitespaceForeColor(bool use, Color color) { WhitespaceColor = color; ScheduleChanged(); }
		public void SetSelectionBackColor(bool use, Color color) { SelectionBackColor = color; ScheduleChanged(); }
		public void SetSelectionForeColor(bool use, Color color) { SelectionForeColor = color; ScheduleChanged(); }
		public void SetFoldMarginColor(bool use, Color color) { }
		public void SetFoldMarginHighlightColor(bool use, Color color) { }
		public void SetFoldFlags(FoldFlags flags) { }
		public void RegisterRgbaImage(int type, Bitmap image) { autocompleteimages[type] = image; }
		public void DirectMessage(int message, IntPtr param1, IntPtr param2) { }
		public void AssignCmdKey(Keys keyDefinition, Command sciCommand) { }
		public int AutoCMaxHeight { get; set; } = 5;
		public bool AutoCIgnoreCase { get; set; }
		public Order AutoCOrder { get; set; }

		internal Color WhitespaceColor { get; private set; }
		internal Color SelectionBackColor { get; private set; }
		internal Color SelectionForeColor { get; private set; }

		/// <summary>Sets a keyword list (space separated; 0 keywords, 1 constants, 2 snippets, 3 properties).</summary>
		public void SetKeywords(int set, string keywords)
		{
			if(string.IsNullOrEmpty(keywords)) { keywordsets.Remove(set); }
			else
			{
				HashSet<string> words = new HashSet<string>();
				foreach(string w in keywords.Split(new[] { ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)) words.Add(w);
				keywordsets[set] = words;
			}
			docversion++;
			TextStyleChanged();
		}

		/// <summary>Whether keywords match without regard to case (upstream lowercases the lists then).</summary>
		internal void SetCaseSensitive(bool value) { casesensitive = value; docversion++; }

		internal byte[] Styled()
		{
			if(stylecache == null || stylecacheversion != docversion)
			{
				stylecache = ScriptLexer.Style(Document.Text, lexer, keywordsets, casesensitive, wordchars);
				stylecacheversion = docversion;
			}
			return stylecache;
		}

		public int GetStyleAt(int position)
		{
			byte[] styles = Styled();
			return position >= 0 && position < styles.Length ? styles[position] : 0;
		}

		#endregion

		#region ================== Autocompletion and call tips

		public bool AutoCActive { get { return host != null && host.AutoCompleteActive; } }
		public bool CallTipActive { get { return host != null && host.CallTipActive; } }
		public void AutoCCancel() { host?.CancelAutoComplete(); }
		public void CallTipCancel() { host?.CancelCallTip(); }
		private int tiphighlightstart, tiphighlightend;
		private int tippos;
		private string tiptext;

		/// <summary>Shows the list (items separated by spaces, optionally "item?imageindex").</summary>
		public void AutoCShow(int lenEntered, string list)
		{
			if(host == null || string.IsNullOrEmpty(list)) return;
			host.ShowAutoComplete(lenEntered, list.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
		}

		public void CallTipShow(int posStart, string definition)
		{
			tippos = posStart; tiptext = definition; tiphighlightstart = tiphighlightend = 0;
			host?.ShowCallTip(posStart, definition, 0, 0);
		}

		public void CallTipSetHlt(int hlStart, int hlEnd)
		{
			tiphighlightstart = hlStart; tiphighlightend = hlEnd;
			host?.ShowCallTip(tippos, tiptext, hlStart, hlEnd);
		}

		#endregion
	}
}
