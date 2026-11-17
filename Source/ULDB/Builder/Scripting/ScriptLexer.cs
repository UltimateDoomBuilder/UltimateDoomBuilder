// ULDB: the syntax highlighting lexer of the script editor. It gives every character the Scintilla
// style number upstream's Lexers.cfg maps to a ScriptStyleType (so ScriptEditorControl's style lookup
// and ScriptHandler's "is this position a comment or a string" checks work as upstream wrote them):
//
//   0 default   1 block comment   2 line comment   4 number   5 keyword (set 0)   6 string
//   7 character 9 preprocessor    10 operator      11 identifier   16 constant (set 1)
//   19 property / global class (set 3)
//
// Lexer 3 and 35 are the C-like lexers (case-sensitive and not), 6 is the Perl-like one
// (# comments), 1 styles nothing.

using System;
using System.Collections.Generic;
using ScintillaNET;

namespace ULDB.Scripting
{
	internal static class ScriptLexer
	{
		public const byte Default = 0, BlockComment = 1, LineComment = 2, Number = 4, Keyword = 5, String = 6, Character = 7,
			Preprocessor = 9, Operator = 10, Identifier = 11, Constant = 16, Property = 19;

		/// <summary>Styles <paramref name="text"/>; the result has one style per character.</summary>
		/// <param name="keywords">Keyword sets by index (0 keywords, 1 constants, 3 properties), lowercase when the script type isn't case sensitive.</param>
		public static byte[] Style(string text, Lexer lexer, IReadOnlyDictionary<int, HashSet<string>> keywords, bool casesensitive, string extrawordchars)
		{
			byte[] styles = new byte[text.Length];
			if(lexer == Lexer.Null || lexer == Lexer.Container || text.Length == 0) return styles;
			StyleRange(text, styles, 0, null, lexer, keywords, casesensitive, extrawordchars);
			return styles;
		}

		/// <summary>
		/// Styles the text after an edit, reusing the styles from before it. Only a block comment carries over a line
		/// break, so styling starts at the changed line (or at the comment it is in) and stops at the first line start
		/// after the change where the old styling was outside a comment too: from there on nothing can differ.
		/// </summary>
		/// <param name="old">The styles of the text before the edit.</param>
		/// <param name="start">Where the changed text begins.</param>
		/// <param name="newend">Where the changed text ends, in the new text.</param>
		/// <param name="delta">New length minus old length.</param>
		public static byte[] Restyle(string text, byte[] old, int start, int newend, int delta, Lexer lexer, IReadOnlyDictionary<int, HashSet<string>> keywords, bool casesensitive, string extrawordchars)
		{
			if(lexer == Lexer.Null || lexer == Lexer.Container || text.Length == 0) return new byte[text.Length];
			if(old == null || old.Length + delta != text.Length || start < 0 || start > text.Length || newend < start)
				return Style(text, lexer, keywords, casesensitive, extrawordchars);

			// Back to the start of the line, and out of a block comment that began before it
			int from = Math.Min(start, old.Length);
			while(true)
			{
				while(from > 0 && text[from - 1] != '\n' && text[from - 1] != '\r') from--;
				if(from == 0 || old[from - 1] != BlockComment) break; // a line start outside a comment
				while(from > 0 && old[from - 1] == BlockComment) from--;
			}

			byte[] styles = new byte[text.Length];
			Array.Copy(old, 0, styles, 0, from);
			int stopped = StyleRange(text, styles, from, linestart =>
			{
				// The line break before it has to be text from before the edit
				if(linestart - 1 < newend || linestart >= text.Length) return false;
				int oldbreak = linestart - delta - 1; // the line break before it, as it was styled
				return oldbreak >= 0 && oldbreak < old.Length && old[oldbreak] != BlockComment;
			}, lexer, keywords, casesensitive, extrawordchars);
			if(stopped < text.Length) Array.Copy(old, stopped - delta, styles, stopped, text.Length - stopped);
			LastRestyledLength = stopped - from;
			return styles;
		}

		/// <summary>How many characters the last Restyle went over (for tests).</summary>
		internal static int LastRestyledLength;

		// Styles from a line start (or the start of a block comment) on; canstop is asked at every later line start
		// reached outside a comment. Returns where it stopped.
		private static int StyleRange(string text, byte[] styles, int from, Func<int, bool> canstop, Lexer lexer, IReadOnlyDictionary<int, HashSet<string>> keywords, bool casesensitive, string extrawordchars)
		{
			bool perl = lexer == Lexer.Perl;
			int n = text.Length;
			int i = from;
			bool linestart = true;

			while(i < n)
			{
				char c = text[i];

				if(c == '\n' || c == '\r')
				{
					styles[i++] = Default;
					if(c == '\n' || (c == '\r' && (i >= n || text[i] != '\n')))
					{
						linestart = true;
						if(canstop != null && canstop(i)) return i;
					}
					continue;
				}

				if(c == ' ' || c == '\t')
				{
					styles[i++] = Default;
					continue;
				}

				bool wasatlinestart = linestart;
				linestart = false;

				// Comments
				if(perl ? c == '#' : (c == '/' && i + 1 < n && text[i + 1] == '/'))
				{
					int end = LineEnd(text, i);
					Fill(styles, i, end, LineComment);
					i = end;
					continue;
				}
				if(!perl && c == '/' && i + 1 < n && text[i + 1] == '*')
				{
					int end = text.IndexOf("*/", i + 2, StringComparison.Ordinal);
					end = end < 0 ? n : end + 2;
					Fill(styles, i, end, BlockComment);
					i = end;
					continue;
				}

				// Preprocessor directives: the line, up to a comment
				if(!perl && c == '#' && wasatlinestart)
				{
					int end = LineEnd(text, i);
					int comment = FindComment(text, i, end);
					Fill(styles, i, comment, Preprocessor);
					i = comment;
					continue;
				}

				// Strings and characters
				if(c == '"' || c == '\'')
				{
					int j = i + 1;
					while(j < n && text[j] != c && text[j] != '\n' && text[j] != '\r')
					{
						if(text[j] == '\\' && j + 1 < n && text[j + 1] != '\n' && text[j + 1] != '\r') j++;
						j++;
					}
					if(j < n && text[j] == c) j++;
					Fill(styles, i, j, c == '"' ? String : Character);
					i = j;
					continue;
				}

				// Numbers
				if(char.IsDigit(c) || (c == '.' && i + 1 < n && char.IsDigit(text[i + 1])))
				{
					int j = i + 1;
					if(c == '0' && j < n && (text[j] == 'x' || text[j] == 'X'))
					{
						j++;
						while(j < n && Uri.IsHexDigit(text[j])) j++;
					}
					else
					{
						while(j < n && (char.IsDigit(text[j]) || text[j] == '.')) j++;
						if(j < n && (text[j] == 'e' || text[j] == 'E') && j + 1 < n && (char.IsDigit(text[j + 1]) || text[j + 1] == '-' || text[j + 1] == '+'))
						{
							j += 2;
							while(j < n && char.IsDigit(text[j])) j++;
						}
					}
					while(j < n && (text[j] == 'f' || text[j] == 'F' || text[j] == 'l' || text[j] == 'L' || text[j] == 'u' || text[j] == 'U')) j++;
					Fill(styles, i, j, Number);
					i = j;
					continue;
				}

				// Identifiers and keywords
				if(IsWordChar(c, extrawordchars) && !char.IsDigit(c))
				{
					int j = i + 1;
					while(j < n && IsWordChar(text[j], extrawordchars)) j++;
					string word = text.Substring(i, j - i);
					if(!casesensitive) word = word.ToLowerInvariant();
					byte style = Identifier;
					if(keywords != null)
					{
						if(Has(keywords, 0, word)) style = Keyword;
						else if(Has(keywords, 1, word)) style = Constant;
						else if(Has(keywords, 3, word)) style = Property;
					}
					Fill(styles, i, j, style);
					i = j;
					continue;
				}

				styles[i++] = Operator;
			}

			return n;
		}

		public static bool IsWordChar(char c, string extra)
		{
			return char.IsLetterOrDigit(c) || c == '_' || (!string.IsNullOrEmpty(extra) && extra.IndexOf(c) != -1);
		}

		private static bool Has(IReadOnlyDictionary<int, HashSet<string>> sets, int set, string word)
		{
			return sets.TryGetValue(set, out HashSet<string> words) && words.Contains(word);
		}

		private static void Fill(byte[] styles, int from, int to, byte style)
		{
			for(int i = from; i < to && i < styles.Length; i++) styles[i] = style;
		}

		private static int LineEnd(string text, int from)
		{
			int i = from;
			while(i < text.Length && text[i] != '\n' && text[i] != '\r') i++;
			return i;
		}

		// Where a // or /* comment starts in text[from..to) (outside strings), or to.
		private static int FindComment(string text, int from, int to)
		{
			bool instring = false;
			for(int i = from; i < to - 1; i++)
			{
				if(text[i] == '"') instring = !instring;
				else if(!instring && text[i] == '/' && (text[i + 1] == '/' || text[i + 1] == '*')) return i;
			}
			return to;
		}
	}
}
