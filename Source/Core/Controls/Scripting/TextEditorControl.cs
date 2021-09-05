using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

#if NO_SCINTILLA

namespace ScintillaNET
{
	public enum Lexer
	{
		Container = 0,
		Null = 1,
		Python = 2,
		Cpp = 3,
		Html = 4,
		Xml = 5,
		Perl = 6,
		Sql = 7,
		Vb = 8,
		Properties = 9,
		Batch = 12,
		Lua = 15,
		Pascal = 18,
		Ada = 20,
		Lisp = 21,
		Ruby = 22,
		VbScript = 28,
		Asm = 34,
		Fortran = 36,
		Css = 38,
		Verilog = 56,
		BlitzBasic = 66,
		PureBasic = 67,
		PhpScript = 69,
		Smalltalk = 72,
		FreeBasic = 75,
		R = 86,
		PowerShell = 88,
		Markdown = 98,
		Json = 120
	}

	public enum WhitespaceMode
    {
		Invisible,
		VisibleAlways,
		VisibleAfterIndent,
		VisibleOnlyIndent
	}

	public enum WrapMode
    {
		None,
		Word,
		Char,
		Whitespace
	}

	public enum Command
    {
		Null
	}

	public enum FoldAction
    {
		Contract
    }

	[Flags]
	public enum FoldFlags
    {
		LineAfterContracted
	}

	public class Line
    {
    		public Line()
    		{
    			Text = "";
    		}
    		
		public int Position { get; private set; }
		public int EndPosition { get; private set; }
		public string Text { get; private set; }
		public int WrapCount { get; private set; }
		public int Indentation { get; set; }
		public int FoldLevel { get; set; }
		public bool Expanded { get; set; }

		public void FoldLine(FoldAction action) { }
		public void Goto() { }
		public void MarkerAdd(int marker) { }
	}

	public class LineCollection // : IEnumerable<Line>
    {
		public LineCollection()
        {
			lines.Add(new Line());
        }

		public int Count { get { return lines.Count; } }
		public Line this[int index] { get { return lines[index]; } }

		internal List<Line> lines = new List<Line>();
	}

	public enum StyleCase
    {
		Mixed
    }

	public class Style
    {
		public const int Default = 0;
		public const int LineNumber = 1;
		public const int CallTip = 2;
		public const int IndentGuide = 3;
		public const int BraceLight = 4;
		public const int BraceBad = 5;
		public const int FoldDisplayText = 6;

		public Color BackColor { get; set; }
		public Color ForeColor { get; set; }
		public string Font { get; set; }
		public int Size { get; set; }
		public bool Bold { get; set; }
		public bool Italic { get; set; }
		public bool Underline { get; set; }
		public StyleCase Case { get; set; }
	}

	public class StyleCollection // : IEnumerable<Style>
    {
		public StyleCollection()
        {
			for (int i = 0; i < styles.Length; i++) styles[i] = new Style();
        }

		public Style this[int index] { get { return styles[index]; } }

		Style[] styles = new Style[256];
	}

	public enum MarginType
    {
		Symbol,
		Number
	}

	public enum MarginCursor
    {
		Arrow
    }

	public class Margin
    {
		public MarginType Type { get; set; }
		public int Width { get; set; }
		public uint Mask { get; set; }
		public MarginCursor Cursor { get; set; }
		public bool Sensitive { get; set; }
	}

	public class MarginCollection // : IEnumerable<Margin>
	{
		public MarginCollection()
        {
			for (int i = 0; i < margins.Length; i++) margins[i] = new Margin();
        }

		public int Count { get { return margins.Length; } }
		public Margin this[int index] { get { return margins[index]; } }

		Margin[] margins = new Margin[4];
	}

	public enum MarkerSymbol
    {
		BoxPlus,
		BoxMinus,
		BoxPlusConnected,
		BoxMinusConnected,
		TCorner,
		LCorner,
		VLine,
		RgbaImage
	}

	public class Marker
    {
		public const uint MaskFolders = 0xfe000000;
		public const int FolderEnd = 25;
		public const int FolderOpenMid = 26;
		public const int FolderMidTail = 27;
		public const int FolderTail = 28;
		public const int FolderSub = 29;
		public const int Folder = 30;
		public const int FolderOpen = 31;

		public MarkerSymbol Symbol { get; set; }

		public void SetBackColor(Color color) { }
		public void SetForeColor(Color color) { }
		public void DefineRgbaImage(Bitmap image) { }
	}

	public class MarkerCollection // : IEnumerable<Marker>
	{
		public MarkerCollection()
        {
			for (int i = 0; i < marker.Length; i++) marker[i] = new Marker();
        }

		public int Count { get { return marker.Length; } }
		public Marker this[int index] { get { return marker[index]; } }

		Marker[] marker = new Marker[32];
	}

	public enum IndicatorStyle
    {
		RoundBox
    }

	public class Indicator
    {
		public IndicatorStyle Style { get; set; }
		public bool Under { get; set; }
		public Color ForeColor { get; set; }
		public int OutlineAlpha { get; set; }
		public int Alpha { get; set; }
	}

	public class IndicatorCollection // : IEnumerable<Indicator>
    {
		public IndicatorCollection()
        {
			for (int i = 0; i < indicators.Length; i++) indicators[i] = new Indicator();
        }

		public Indicator this[int index] { get { return indicators[index]; } }

		Indicator[] indicators = new Indicator[32];
	}

	[Flags]
	public enum AutomaticFold
    {
		None = 0,
		Show = 1,
		Click = 2,
		Change = 4
	}

	[Flags]
	public enum SearchFlags
    {
		None = 0,
		MatchCase = 1,
		WholeWord = 2
	}

	public enum FontQuality
    {
		Default,
		LcdOptimized
	}

	public enum Order
    {
		Presorted,
		PerformSort,
		Custom
	}

	public class Scintilla : UserControl
	{
		TextBox textbox;

		public Scintilla()
		{
			Lines = new LineCollection();
			Styles = new StyleCollection();
			Margins = new MarginCollection();
			Markers = new MarkerCollection();
			Indicators = new IndicatorCollection();

			textbox = new TextBox();
			textbox.Location = new Point(0, 0);
			textbox.Multiline = true;
			textbox.Font = new Font(FontFamily.GenericMonospace, 10.0f);

			Controls.Add(textbox);
			Size = textbox.PreferredSize;
		}

		protected override void OnResize(EventArgs e)
        {
			base.OnResize(e);
			textbox.Size = Size;
        }

		public const int InvalidPosition = -1;

		[DefaultValue(BorderStyle.Fixed3D)]
		[Category("Appearance")]
		[Description("Indicates whether the control should have a border.")]
		public new BorderStyle BorderStyle { get { return textbox.BorderStyle; } set { textbox.BorderStyle = value; } }

		[DefaultValue(1)]
		[Category("Caret")]
		[Description("The width of the caret line measured in pixels (between 0 and 3).")]
		public int CaretWidth { get; set; }

		[DefaultValue(0)]
		[Category("Whitespace")]
		[Description("Extra whitespace added to the ascent (top) of each line.")]
		public int ExtraAscent { get; set; }

		[DefaultValue(0)]
		[Category("Whitespace")]
		[Description("Extra whitespace added to the descent (bottom) of each line.")]
		public int ExtraDescent { get; set; }

		[DefaultValue(FontQuality.Default)]
		[Category("Misc")]
		[Description("Specifies the anti-aliasing method to use when rendering fonts.")]
		public FontQuality FontQuality { get; set; }

		[DefaultValue(2000)]
		[Category("Scrolling")]
		[Description("The range in pixels of the horizontal scroll bar.")]
		public int ScrollWidth { get; set; }

		[DefaultValue(1)]
		[Category("Whitespace")]
		[Description("The size of whitespace dots.")]
		public int WhitespaceSize { get; set; }

		[DefaultValue(false)]
		[Category("Autocompletion")]
		[Description("Whether autocompletion word matching can ignore case.")]
		public bool AutoCIgnoreCase { get; set; }

		[DefaultValue(5)]
		[Category("Autocompletion")]
		[Description("The maximum number of rows to display in an autocompletion list.")]
		public int AutoCMaxHeight { get; set; }

		[DefaultValue(Order.Presorted)]
		[Category("Autocompletion")]
		[Description("The order of words in an autocompletion list.")]
		public Order AutoCOrder { get; set; }

		public override string Text { get { return textbox.Text; } set { textbox.Text = value.Replace("\r\n", "\n").Replace("\n", "\r\n"); } }
		public int TextLength { get { return textbox.TextLength; } }
		public int GetCharAt(int position) { return textbox.Text[position]; }
		public int CurrentPosition { get; set; }
		public int CurrentLine { get; private set; }
		public int IndicatorCurrent { get; set; }
		public int TargetStart { get; set; }
		public int TargetEnd { get; set; }
		public int FirstVisibleLine { get; set; }
		public int LinesOnScreen { get; private set; }
		public bool AutoCActive { get; private set; }
		public bool CallTipActive { get; private set; }
		public Lexer Lexer { get; set; }
		public int SelectionStart { get { return textbox.SelectionStart; } set { textbox.SelectionStart = value; } }
		public int SelectionEnd { get { return textbox.SelectionStart + textbox.SelectionLength; } set { textbox.SelectionLength = SelectionStart - value; } }
		public string SelectedText { get { return textbox.SelectedText; } }
		public int TabWidth { get; set; }
		public bool ReadOnly { get { return textbox.ReadOnly; } set { textbox.ReadOnly = value; } }
		public bool CanUndo { get { return textbox.CanUndo; } }
		public bool CanRedo { get { return false; } }
		public bool CanPaste { get { return true; } }
		public bool Modified { get { return textbox.Modified; } }
		public Color CaretForeColor { get; set; }
		public int CaretPeriod { get; set; }
		public bool UseTabs { get; set; }
		public WhitespaceMode ViewWhitespace { get; set; }
		public WrapMode WrapMode { get; set; }
		public AutomaticFold AutomaticFold { get; set; }
		public SearchFlags SearchFlags { get; set; }

		public LineCollection Lines { get; private set; }
		public StyleCollection Styles { get; private set; }
		public MarginCollection Margins { get; private set; }
		public MarkerCollection Markers { get; private set; }
		public IndicatorCollection Indicators { get; private set; }

		public int WordStartPosition(int position, bool onlyWordCharacters) { return 0; }
		public int WordEndPosition(int position, bool onlyWordCharacters) { return 0; }
		public int LineFromPosition(int position) { return 0; }
		public string GetWordFromPosition(int position) { return ""; }
		public string GetTextRange(int position, int length) { return textbox.Text.Substring(position, length); }
		public void SetEmptySelection(int pos) { textbox.DeselectAll(); }
		public void ReplaceSelection(string text) { textbox.Text = textbox.Text.Substring(0, textbox.SelectionStart) + text + textbox.Text.Substring(textbox.SelectionStart + textbox.SelectionLength); }
		public int PointXFromPosition(int pos) { return 0; }
		public int PointYFromPosition(int pos) { return 0; }
		public int CharPositionFromPointClose(int x, int y) { return 0; }
		public void ShowLines(int lineStart, int lineEnd) { }
		public void GotoPosition(int position) { }
		public void MarkerDeleteAll(int marker) { }
		public void IndicatorClearRange(int position, int length) { }
		public void IndicatorFillRange(int position, int length) { }
		public void EmptyUndoBuffer() { textbox.ClearUndo(); }
		public void BeginUndoAction() { }
		public void EndUndoAction() { }
		public void Undo() { textbox.Undo(); }
		public void Redo() { }
		public void Cut() { textbox.Cut(); }
		public void Copy() { textbox.Copy(); }
		public void Paste() { textbox.Paste(); }
		public void SelectAll() { textbox.SelectAll(); }
		public void DeleteRange(int position, int length) { }
		public int SearchInTarget(string text) { return 0; }
		public void InsertText(int position, string text) { textbox.Text = textbox.Text.Substring(0, position) + text + textbox.Text.Substring(position); }
		public void SetSavePoint() { textbox.Modified = false; }

		public void RegisterRgbaImage(int type, Bitmap image) { }
		public int TextWidth(int style, string text) { return 0; }

		public void BraceHighlight(int position1, int position2) { }
		public int BraceMatch(int position) { return 0; }
		public void BraceBadLight(int position) { }

		public string WordChars { get; set; }

		public int GetStyleAt(int position) { return 0; }
		public void StyleClearAll() { }
		public void ClearDocumentStyle() { }
		public void StyleResetDefault() { }
		public void SetKeywords(int set, string keywords) { }
		public void SetProperty(string name, string value) { }
		public void SetWhitespaceBackColor(bool use, Color color) { }
		public void SetWhitespaceForeColor(bool use, Color color) { }
		public void SetSelectionBackColor(bool use, Color color) { }
		public void SetSelectionForeColor(bool use, Color color) { }
		public void SetFoldMarginColor(bool use, Color color) { }
		public void SetFoldMarginHighlightColor(bool use, Color color) { }
		public void SetFoldFlags(FoldFlags flags) { }

		public void AutoCCancel() { }
		public void AutoCShow(int lenEntered, string list) { }

		public void CallTipShow(int posStart, string definition) { }
		public void CallTipCancel() { }
		public void CallTipSetHlt(int hlStart, int hlEnd) { }

		public void DirectMessage(int message, IntPtr param1, IntPtr param2) { }

		public void AssignCmdKey(Keys keyDefinition, Command sciCommand) { }

		[Category("Notifications")]
		[Description("Occurs when the user types a character.")]
		public event EventHandler<CharAddedEventArgs> CharAdded;

		[Category("Notifications")]
		[Description("Occurs after autocompleted text has been inserted.")]
		public event EventHandler<AutoCSelectionEventArgs> AutoCCompleted;

		[Category("Notifications")]
		[Description("Occurs before text is inserted. Permits changing the inserted text.")]
		public event EventHandler<InsertCheckEventArgs> InsertCheck;

		[Category("Notifications")]
		[Description("Occurs when the control UI is updated.")]
		public event EventHandler<UpdateUIEventArgs> UpdateUI;
	}

	public class UpdateUIEventArgs
	{
	}

	public class CharAddedEventArgs
	{
		public char Char { get; set; }
	}

	public class InsertCheckEventArgs
	{
		public string Text { get; set; }
	}

	public class AutoCSelectionEventArgs
	{
		public string Text { get; set; }
	}
}

#endif
