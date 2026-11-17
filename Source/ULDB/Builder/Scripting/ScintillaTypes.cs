// ULDB: the ScintillaNET types upstream's script editor code is written against. Upstream's Linux
// build stubs these over a plain TextBox (TextEditorControl.cs, #if NO_SCINTILLA), which is why the
// editor there draws lines through the text and crashes in the search code. These implement them
// for real over AvaloniaEdit's document (see Scintilla.cs), with the same names and members, so
// upstream's ScriptEditorControl, ScriptHandler and the document tabs run unchanged.

using System;
using System.Collections.Generic;
using System.Drawing;

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

	public enum WhitespaceMode { Invisible, VisibleAlways, VisibleAfterIndent, VisibleOnlyIndent }
	public enum WrapMode { None, Word, Char, Whitespace }
	public enum Command { Null }
	public enum FoldAction { Contract, Expand, Toggle }

	[Flags]
	public enum FoldFlags { LineAfterContracted = 16 }

	public enum StyleCase { Mixed }

	public enum MarginType { Symbol, Number }
	public enum MarginCursor { Arrow }
	public enum MarkerSymbol { BoxPlus, BoxMinus, BoxPlusConnected, BoxMinusConnected, TCorner, LCorner, VLine, RgbaImage }
	public enum IndicatorStyle { RoundBox }

	[Flags]
	public enum AutomaticFold { None = 0, Show = 1, Click = 2, Change = 4 }

	[Flags]
	public enum SearchFlags { None = 0, MatchCase = 1, WholeWord = 2 }

	public enum FontQuality { Default, LcdOptimized }
	public enum Order { Presorted, PerformSort, Custom }

	public class Style
	{
		// ScintillaNET's numbers (lexer styles use 0-31 and 33 is a mapped style in Lexers.cfg)
		public const int Default = 32;
		public const int LineNumber = 33;
		public const int BraceLight = 34;
		public const int BraceBad = 35;
		public const int IndentGuide = 37;
		public const int CallTip = 38;
		public const int FoldDisplayText = 39;

		// Changing a style redraws the editor that shows it (Preferences' colour preview changes styles one by one).
		internal event System.Action Changed;
		private Color backcolor, forecolor;
		private string font;
		private int size;
		private bool bold, italic, underline;
		private StyleCase stylecase;

		public Color BackColor { get { return backcolor; } set { backcolor = value; Changed?.Invoke(); } }
		public Color ForeColor { get { return forecolor; } set { forecolor = value; Changed?.Invoke(); } }
		public string Font { get { return font; } set { font = value; Changed?.Invoke(); } }
		public int Size { get { return size; } set { size = value; Changed?.Invoke(); } }
		public bool Bold { get { return bold; } set { bold = value; Changed?.Invoke(); } }
		public bool Italic { get { return italic; } set { italic = value; Changed?.Invoke(); } }
		public bool Underline { get { return underline; } set { underline = value; Changed?.Invoke(); } }
		public StyleCase Case { get { return stylecase; } set { stylecase = value; Changed?.Invoke(); } }
	}

	public class StyleCollection
	{
		private readonly Style[] styles = new Style[256];
		internal event System.Action StyleChanged;
		public StyleCollection()
		{
			for(int i = 0; i < styles.Length; i++)
			{
				styles[i] = new Style();
				styles[i].Changed += () => StyleChanged?.Invoke();
			}
		}
		public Style this[int index] { get { return styles[index]; } }
	}

	public class Margin
	{
		private MarginType type;
		private int width;
		internal event System.Action Changed;
		public MarginType Type { get { return type; } set { if(type != value) { type = value; Changed?.Invoke(); } } }
		public int Width { get { return width; } set { if(width != value) { width = value; Changed?.Invoke(); } } }
		public uint Mask { get; set; }
		public MarginCursor Cursor { get; set; }
		public bool Sensitive { get; set; }
	}

	public class MarginCollection
	{
		private readonly Margin[] margins = new Margin[5];
		internal event System.Action Changed;
		public MarginCollection() { for(int i = 0; i < margins.Length; i++) { margins[i] = new Margin(); margins[i].Changed += () => Changed?.Invoke(); } }
		public int Count { get { return margins.Length; } }
		public Margin this[int index] { get { return margins[index]; } }
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
		public Color Back { get; private set; }
		public Color Fore { get; private set; }
		public Bitmap Image { get; private set; }

		public void SetBackColor(Color color) { Back = color; }
		public void SetForeColor(Color color) { Fore = color; }
		public void DefineRgbaImage(Bitmap image) { Image = image; }
	}

	public class MarkerCollection
	{
		private readonly Marker[] marker = new Marker[32];
		public MarkerCollection() { for(int i = 0; i < marker.Length; i++) marker[i] = new Marker(); }
		public int Count { get { return marker.Length; } }
		public Marker this[int index] { get { return marker[index]; } }
	}

	public class Indicator
	{
		public IndicatorStyle Style { get; set; }
		public bool Under { get; set; }
		public Color ForeColor { get; set; }
		public int OutlineAlpha { get; set; }
		public int Alpha { get; set; }
	}

	public class IndicatorCollection
	{
		private readonly Indicator[] indicators = new Indicator[32];
		public IndicatorCollection() { for(int i = 0; i < indicators.Length; i++) indicators[i] = new Indicator(); }
		public Indicator this[int index] { get { return indicators[index]; } }
	}

	public class UpdateUIEventArgs : EventArgs { }
	public class CharAddedEventArgs : EventArgs { public char Char { get; set; } }
	public class InsertCheckEventArgs : EventArgs { public string Text { get; set; } }
	public class AutoCSelectionEventArgs : EventArgs { public string Text { get; set; } }

	/// <summary>One line of the document (a view on the editor; positions are character offsets).</summary>
	public class Line
	{
		private readonly Scintilla owner;
		private readonly int index;

		internal Line(Scintilla owner, int index) { this.owner = owner; this.index = index; }

		public int Index { get { return index; } }
		/// <summary>Offset of the first character.</summary>
		public int Position { get { return owner.LineStart(index); } }
		/// <summary>Offset after the line's end-of-line characters.</summary>
		public int EndPosition { get { return owner.LineEndWithBreak(index); } }
		/// <summary>The line's text including its end-of-line characters.</summary>
		public string Text { get { return owner.GetTextRange(Position, EndPosition - Position); } }
		public int Length { get { return EndPosition - Position; } }
		public int WrapCount { get { return owner.WrapCountOf(index); } }

		/// <summary>Width of the leading whitespace in columns (a tab counts as the tab width).</summary>
		public int Indentation
		{
			get { return owner.GetIndentation(index); }
			set { owner.SetIndentation(index, value); }
		}

		public int FoldLevel { get { return owner.FoldLevelOf(index); } set { } }

		public bool Expanded
		{
			get { return !owner.IsFolded(index); }
			set { owner.SetFolded(index, !value); }
		}

		public void FoldLine(FoldAction action)
		{
			switch(action)
			{
				case FoldAction.Contract: owner.SetFolded(index, true); break;
				case FoldAction.Expand: owner.SetFolded(index, false); break;
				default: owner.SetFolded(index, !owner.IsFolded(index)); break;
			}
		}

		public void Goto() { owner.GotoPosition(Position); }
		public void MarkerAdd(int marker) { owner.AddMarker(index, marker); }
	}

	public class LineCollection
	{
		private readonly Scintilla owner;
		internal LineCollection(Scintilla owner) { this.owner = owner; }
		public int Count { get { return owner.LineCount; } }

		public Line this[int index]
		{
			get
			{
				if(index < 0 || index >= owner.LineCount) throw new ArgumentOutOfRangeException(nameof(index));
				return new Line(owner, index);
			}
		}
	}
}
