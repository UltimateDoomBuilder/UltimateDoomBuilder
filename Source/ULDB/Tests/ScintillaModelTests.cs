using System.Collections.Generic;
using ScintillaNET;
using ULDB.Scripting;

namespace ULDB.Tests
{
	// The script editor's engine: upstream's Linux build stubs ScintillaNET over a plain text box, which
	// crashes on search; ULDB implements the model for real (Scripting/Scintilla.cs). These run it
	// headless, without a view.
	public class ScintillaModelTests
	{
		private static Scintilla Make(string text)
		{
			Scintilla s = new Scintilla();
			s.Text = text;
			return s;
		}

		[Fact]
		public void SearchFindsMatchesForwardAndBackward()
		{
			Scintilla s = Make("one two three two one");
			s.TargetStart = 0; s.TargetEnd = s.TextLength;
			Assert.Equal(4, s.SearchInTarget("two"));
			Assert.Equal(7, s.TargetEnd);

			s.TargetStart = s.TextLength; s.TargetEnd = 0;
			Assert.Equal(14, s.SearchInTarget("two"));

			s.TargetStart = 13; s.TargetEnd = 0; // backwards from before the second match
			Assert.Equal(4, s.SearchInTarget("two"));
		}

		[Fact]
		public void SearchHonoursCaseAndWholeWord()
		{
			Scintilla s = Make("Foo foobar FOO");
			s.TargetStart = 0; s.TargetEnd = s.TextLength;
			s.SearchFlags = SearchFlags.MatchCase;
			Assert.Equal(0, s.SearchInTarget("Foo"));

			s.TargetStart = 1; s.TargetEnd = s.TextLength;
			Assert.Equal(-1, s.SearchInTarget("Foo")); // no other exact-case match

			s.TargetStart = 1; s.TargetEnd = s.TextLength;
			s.SearchFlags = SearchFlags.WholeWord;
			Assert.Equal(11, s.SearchInTarget("foo")); // skips "foobar"
		}

		[Fact]
		public void SearchInEmptyDocumentOrWithNothingToFindDoesNotThrow()
		{
			Scintilla s = Make("");
			s.TargetStart = 0; s.TargetEnd = 0;
			Assert.Equal(-1, s.SearchInTarget("x"));
			Assert.Equal(-1, s.SearchInTarget(""));
			s.SelectionEnd = -1;   // what crashed upstream's Linux editor
			s.SelectionStart = 99;
			Assert.Equal(0, s.CurrentPosition);
		}

		[Fact]
		public void SelectionAndReplacementWork()
		{
			Scintilla s = Make("hello world");
			s.SelectionStart = 6; s.SelectionEnd = 11;
			Assert.Equal("world", s.SelectedText);
			s.ReplaceSelection("there");
			Assert.Equal("hello there", s.Text);
			Assert.Equal(11, s.CurrentPosition);
			Assert.Equal(0, s.SelectionEnd - s.SelectionStart);
		}

		[Fact]
		public void WordsAndLinesAreFoundByPosition()
		{
			Scintilla s = Make("int x = 5;\r\n\tfoo_bar(1);\r\n");
			Assert.Equal("foo_bar", s.GetWordFromPosition(14));
			Assert.Equal(1, s.LineFromPosition(14));
			Assert.Equal(3, s.Lines.Count);
			Assert.Equal(4, s.Lines[1].Indentation);
			s.Lines[1].Indentation = 8;
			Assert.StartsWith("\t\tfoo_bar", s.Lines[1].Text);
		}

		[Fact]
		public void ModifiedFollowsTheSavePoint()
		{
			Scintilla s = Make("a");
			s.EmptyUndoBuffer();
			s.SetSavePoint();
			Assert.False(s.Modified);
			s.InsertText(1, "b");
			Assert.True(s.Modified);
			Assert.True(s.CanUndo);
			s.Undo();
			Assert.False(s.Modified);
			Assert.True(s.CanRedo);
		}

		[Fact]
		public void BracesMatchAcrossStringsAndComments()
		{
			Scintilla s = Make("{ \"}\" // }\r\n  { } }");
			s.Lexer = Lexer.Cpp;
			Assert.Equal(s.TextLength - 1, s.BraceMatch(0));
			Assert.Equal(0, s.BraceMatch(s.TextLength - 1));
			Assert.Equal(Scintilla.InvalidPosition, s.BraceMatch(3)); // a brace in a string isn't one
		}

		[Fact]
		public void TheLexerStylesLikeScintillasCppLexer()
		{
			Scintilla s = Make("script 1 (int x) { // go\r\n  int y = 0x1F + 2; str(\"a\"); /* b */\r\n}\r\n#include \"zcommon.acs\"\r\n");
			s.Lexer = Lexer.Cpp;
			s.SetKeywords(0, "script int");
			s.SetKeywords(1, "str");
			string t = s.Text;
			Assert.Equal(ScriptLexer.Keyword, s.GetStyleAt(t.IndexOf("script")));
			Assert.Equal(ScriptLexer.Number, s.GetStyleAt(t.IndexOf("1 (")));
			Assert.Equal(ScriptLexer.LineComment, s.GetStyleAt(t.IndexOf("go")));
			Assert.Equal(ScriptLexer.Number, s.GetStyleAt(t.IndexOf("0x1F")));
			Assert.Equal(ScriptLexer.Constant, s.GetStyleAt(t.IndexOf("str")));
			Assert.Equal(ScriptLexer.String, s.GetStyleAt(t.IndexOf("\"a\"") + 1));
			Assert.Equal(ScriptLexer.BlockComment, s.GetStyleAt(t.IndexOf("/* b")));
			Assert.Equal(ScriptLexer.Preprocessor, s.GetStyleAt(t.IndexOf("#include")));
			Assert.Equal(ScriptLexer.Identifier, s.GetStyleAt(t.IndexOf("y =")));
		}

		[Fact]
		public void FoldRegionsFollowBraces()
		{
			Scintilla s = Make("a\r\n{\r\n  b\r\n  {\r\n    c\r\n  }\r\n}\r\n");
			s.Lexer = Lexer.Cpp;
			List<int[]> regions = s.GetFoldRegions();
			Assert.Equal(2, regions.Count);
			Assert.Equal(new[] { 1, 6 }, regions[0]);
			Assert.Equal(new[] { 3, 5 }, regions[1]);
			s.Lines[1].FoldLine(FoldAction.Contract);
			Assert.False(s.Lines[1].Expanded);
		}

		[Fact]
		public void ErrorMarkersAndHighlightsFollowEdits()
		{
			Scintilla s = Make("a\nb\nword c\nd");
			s.Lines[2].MarkerAdd(2);
			s.IndicatorCurrent = 8;
			s.IndicatorFillRange(4, 4); // "word"

			s.InsertText(0, "new\nlines\n"); // two lines above
			Assert.Contains(s.LineMarkers, m => m.Key == 4 && m.Value.Contains(2));
			Assert.DoesNotContain(s.LineMarkers, m => m.Key == 2 && m.Value.Contains(2));
			int[] range = System.Linq.Enumerable.Single(s.IndicatorRanges(8));
			Assert.Equal("word", s.GetTextRange(range[0], range[1] - range[0]));

			s.DeleteRange(0, 4); // one line gone
			Assert.Contains(s.LineMarkers, m => m.Key == 3 && m.Value.Contains(2));
			range = System.Linq.Enumerable.Single(s.IndicatorRanges(8));
			Assert.Equal("word", s.GetTextRange(range[0], range[1] - range[0]));

			// Text after the marks leaves them alone; deleting the highlighted text removes the highlight
			s.InsertText(s.TextLength, "\nend");
			Assert.Contains(s.LineMarkers, m => m.Key == 3 && m.Value.Contains(2));
			s.DeleteRange(range[0], 4);
			Assert.Empty(s.IndicatorRanges(8));
		}
	}
}
