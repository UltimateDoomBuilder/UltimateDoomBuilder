using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CodeImp.DoomBuilder.ZDoom
{
    internal class ZScriptTokenString : System.Attribute
    {
        public string Value { get; private set; }

        public ZScriptTokenString(string value)
        {
            Value = value;
        }
    }

    internal enum ZScriptTokenType
    {
        // generic tokens
        Identifier, // meow
        Integer, // -666
        Double, // 1.3
        String, // "..."
        Name, // '...'

        // comments
        LineComment, // // blablabla
        BlockComment, // /* blablabla */
        Whitespace, // whitespace is a legit token.
        Region, // #region, #endregion

        // invalid token
        Invalid,

        [ZScriptTokenString("#")] Preprocessor,
        [ZScriptTokenString("\n")] Newline,

        [ZScriptTokenString("{")] OpenCurly,
        [ZScriptTokenString("}")] CloseCurly,

        [ZScriptTokenString("(")] OpenParen,
        [ZScriptTokenString(")")] CloseParen,

        [ZScriptTokenString("[")] OpenSquare,
        [ZScriptTokenString("]")] CloseSquare,

        [ZScriptTokenString(".")] Dot,
        [ZScriptTokenString(",")] Comma,

        // == != < > <= >=
        [ZScriptTokenString("==")] OpEquals,
        [ZScriptTokenString("~==")] OpEqualsCaseInsensitive,
        [ZScriptTokenString("!=")] OpNotEquals,
        [ZScriptTokenString("<")] OpLessThan,
        [ZScriptTokenString(">")] OpGreaterThan,
        [ZScriptTokenString("<=")] OpLessOrEqual,
        [ZScriptTokenString(">=")] OpGreaterOrEqual,

        // ternary operator (x ? y : z), also the colon after state labels
        [ZScriptTokenString("?")] Questionmark,
        [ZScriptTokenString(":")] Colon,
        [ZScriptTokenString("::")] DoubleColon,

        // + - * / << >> ~ ^ & |
        [ZScriptTokenString("+")] OpAdd,
        [ZScriptTokenString("-")] OpSubtract,
        [ZScriptTokenString("*")] OpMultiply,
        [ZScriptTokenString("/")] OpDivide,
        [ZScriptTokenString("<<")] OpLeftShift,
        [ZScriptTokenString(">>")] OpRightShift,
        [ZScriptTokenString("~")] OpNegate,
        [ZScriptTokenString("^")] OpXor,
        [ZScriptTokenString("&")] OpAnd,
        [ZScriptTokenString("|")] OpOr,
        [ZScriptTokenString("..")] OpStringConcat,

        // = += -= *= /= <<= >>= ~= ^= &= |=
        [ZScriptTokenString("=")] OpAssign,
        [ZScriptTokenString("+=")] OpAssignAdd,
        [ZScriptTokenString("-=")] OpAssignSubtract,
        [ZScriptTokenString("*=")] OpAssignMultiply,
        [ZScriptTokenString("/=")] OpAssignDivide,
        [ZScriptTokenString("<<=")] OpAssignLeftShift,
        [ZScriptTokenString(">>=")] OpAssignRightShift,
        [ZScriptTokenString("~=")] OpAssignNegate,
        [ZScriptTokenString("^=")] OpAssignXor,
        [ZScriptTokenString("&=")] OpAssignAnd,
        [ZScriptTokenString("|=")] OpAssignOr,

        // unary: !
        [ZScriptTokenString("!")] OpUnaryNot,

        // semicolon
        [ZScriptTokenString(";")] Semicolon
    }

    internal class ZScriptToken
    {
        public ZScriptToken()
        {
            IsValid = true;
			WarningMessage = String.Empty;
        }

        public ZScriptToken(ZScriptToken other)
        {
            Type = other.Type;
            Value = other.Value;
            ValueInt = other.ValueInt;
            ValueDouble = other.ValueDouble;
            IsValid = other.IsValid;
            WarningMessage = other.WarningMessage;
            Position = other.Position;
        }

        public ZScriptTokenType Type { get; internal set; }
        public string Value { get; internal set; }
        public int ValueInt { get; internal set; }
        public double ValueDouble { get; internal set; }
        public bool IsValid { get; internal set; }
		public string WarningMessage { get; internal set; }
        public long Position { get; internal set; }

        public override string ToString()
        {
            return string.Format("<Token.{0} ({1})>", Type.ToString(), Value);
        }
    }

    internal class ZScriptTokenizer
    {
        private BinaryReader reader;
        private static Dictionary<string, ZScriptTokenType> namedtokentypes; // these are tokens that have precise equivalent in the enum (like operators)
        private static Dictionary<ZScriptTokenType, string> namedtokentypesreverse; // these are tokens that have precise equivalent in the enum (like operators)
        private static List<string> namedtokentypesorder; // this is the list of said tokens ordered by length.
        private StringBuilder SB = new StringBuilder();

        public BinaryReader Reader { get { return reader; } }
        public long LastPosition { get; private set; }

        private List<long> LinePositions;

        public ZScriptTokenizer(BinaryReader br)
        {
            reader = br;

            long cpos = br.BaseStream.Position;
            LinePositions = new List<long>();
            br.BaseStream.Position = 0;
            while (br.BaseStream.Position < br.BaseStream.Length)
            {
                byte b = br.ReadByte();
                if (b == '\n')
                    LinePositions.Add(br.BaseStream.Position);
            }
            br.BaseStream.Position = cpos;

            if (namedtokentypes == null || namedtokentypesreverse == null || namedtokentypesorder == null)
            {
                namedtokentypes = new Dictionary<string, ZScriptTokenType>();
                namedtokentypesreverse = new Dictionary<ZScriptTokenType, string>();
                namedtokentypesorder = new List<string>();
                // initialize the token type list.
                IEnumerable<ZScriptTokenType> tokentypes = Enum.GetValues(typeof(ZScriptTokenType)).Cast<ZScriptTokenType>();
                foreach (ZScriptTokenType tokentype in tokentypes)
                {
                    // 
                    FieldInfo fi = typeof(ZScriptTokenType).GetField(tokentype.ToString());
                    ZScriptTokenString[] attrs = (ZScriptTokenString[])fi.GetCustomAttributes(typeof(ZScriptTokenString), false);
                    if (attrs.Length == 0) continue;
                    // 
                    namedtokentypes.Add(attrs[0].Value, tokentype);
                    namedtokentypesreverse.Add(tokentype, attrs[0].Value);
                    namedtokentypesorder.Add(attrs[0].Value);
                }

                namedtokentypesorder.Sort(delegate (string a, string b)
                {
                    if (a.Length > b.Length)
                        return -1;
                    if (a.Length < b.Length)
                        return 1;
                    return 0;
                });
            }
        }

        public int PositionToLine(long pos)
        {
            for (int i = 0; i < LinePositions.Count; i++)
                if (pos <= LinePositions[i])
                    return i+1;
            return LinePositions.Count;
        }

        public void SkipWhitespace() // note that this skips both whitespace, newlines AND comments
        {
            while (true)
            {
                ZScriptToken tok = ExpectToken(ZScriptTokenType.Newline, ZScriptTokenType.BlockComment, ZScriptTokenType.LineComment, ZScriptTokenType.Whitespace, ZScriptTokenType.Region);
                if (tok == null || !tok.IsValid) break;
            }
        }

        private ZScriptToken TryReadWhitespace()
        {
            long cpos = LastPosition = reader.BaseStream.Position;
            char c;
            try
            {
                c = reader.ReadChar();
            }
            catch (EndOfStreamException)
            {
                return null;
            }

            // 
            string whitespace = " \r\t\u00A0";

            // check whitespace
            if (whitespace.Contains(c))
            {
                //string ws_content = "";
                SB.Length = 0;
                SB.Append(c);
                while (true)
                {
                    char cnext;
                    try
                    {
                        cnext = reader.ReadChar();
                    }
                    catch (EndOfStreamException)
                    {
                        break;
                    }
                    if (whitespace.Contains(cnext))
                    {
                        SB.Append(cnext);
                        continue;
                    }

                    reader.BaseStream.Position--;
                    break;
                }

                ZScriptToken tok = new ZScriptToken();
                tok.Position = cpos;
                tok.Type = ZScriptTokenType.Whitespace;
                tok.Value = SB.ToString();
                return tok;
            }

            reader.BaseStream.Position = cpos;
            return null;
        }

        private ZScriptToken TryReadIdentifier()
        {
            long cpos = LastPosition = reader.BaseStream.Position;
            char c;
            try
            {
                c = reader.ReadChar();
            }
            catch (EndOfStreamException)
            {
                return null;
            }

            // check identifier
            if ((c >= 'a' && c <= 'z') ||
                (c >= 'A' && c <= 'Z') ||
                (c == '_'))
            {
                SB.Length = 0;
                SB.Append(c);
                while (true)
                {
                    char cnext;
                    try
                    {
                        cnext = reader.ReadChar();
                    }
                    catch (EndOfStreamException)
                    {
                        break;
                    }
                    if ((cnext >= 'a' && cnext <= 'z') ||
                        (cnext >= 'A' && cnext <= 'Z') ||
                        (cnext == '_') ||
                        (cnext >= '0' && cnext <= '9'))
                    {
                        SB.Append(cnext);
                        continue;
                    }

                    reader.BaseStream.Position--;
                    break;
                }

                ZScriptToken tok = new ZScriptToken();
                tok.Position = cpos;
                tok.Type = ZScriptTokenType.Identifier;
                tok.Value = SB.ToString();
                return tok;
            }

            reader.BaseStream.Position = cpos;
            return null;
        }

        private ZScriptToken TryReadNumber()
        {
            long cpos = LastPosition = reader.BaseStream.Position;
            char c;
            try
            {
                c = reader.ReadChar();
            }
            catch (EndOfStreamException)
            {
                return null;
            }

            // check integer
            if ((c >= '0' && c <= '9') || c == '.')
            {
                bool isint = true;
                bool isdouble = (c == '.');
                bool isexponent = false;
                if (isdouble) // make sure next character is an integer, otherwise its probably a member access
                {
                    char cnext = reader.ReadChar();
                    if (!(cnext >= '0' && cnext <= '9'))
                    {
                        isint = false;
                        reader.BaseStream.Position--;
                    }
                }

                if (isint)
                {
                    bool isoctal = (c == '0');
                    bool ishex = false;
                    SB.Length = 0;
                    SB.Append(c);
                    while (true)
                    {
                        char cnext;
                        try
                        {
                            cnext = reader.ReadChar();
                        }
                        catch (EndOfStreamException)
                        {
                            break;
                        }
                        if (!isdouble && (cnext == 'x') && SB.Length == 1)
                        {
                            isoctal = false;
                            ishex = true;
                        }
                        else if ((cnext >= '0' && cnext <= '7') ||
                                 (!isoctal && cnext >= '8' && cnext <= '9') ||
                                 (ishex && ((cnext >= 'a' && cnext <= 'f') || (cnext >= 'A' && cnext <= 'F'))))
                        {
                            SB.Append(cnext);
                        }
                        else if (!ishex && !isdouble && !isexponent && cnext == '.')
                        {
                            isdouble = true;
                            isoctal = false;
                            SB.Append('.');
                        }
                        else if (!isoctal && !ishex && !isexponent && (cnext == 'e' || cnext == 'E'))
                        {
                            isexponent = true;
                            isdouble = true;
                            SB.Append('e');
                            try
                            {
                                cnext = reader.ReadChar();
                            }
                            catch (EndOfStreamException)
                            {
                                reader.BaseStream.Position = cpos;
                                return null; // bad exponent notation
                            }
                            if (cnext == '-') SB.Append('-');
                            else reader.BaseStream.Position--;
                        }
                        else
                        {
                            reader.BaseStream.Position--;
                            break;
                        }
                    }

                    ZScriptToken tok = new ZScriptToken();
                    tok.Position = cpos;
                    tok.Type = (isdouble ? ZScriptTokenType.Double : ZScriptTokenType.Integer);
                    tok.Value = SB.ToString();
                    try
                    {
                        if (ishex || isoctal || !isdouble)
                        {
                            int numbase = 10;
                            if (ishex) numbase = 16;
                            else if (isoctal) numbase = 8;
                            tok.ValueInt = Convert.ToInt32(tok.Value, numbase);
                            tok.ValueDouble = tok.ValueInt;
                        }
                        else if (isdouble)
                        {
                            string dval = (tok.Value[0] == '.') ? "0" + tok.Value : tok.Value;
                            tok.ValueDouble = Convert.ToDouble(dval);
                            tok.ValueInt = (int)tok.ValueDouble;
                        }
                    }
					catch (OverflowException) // biwa. If the value is too small or too big set it to the min or max, and set a warning message
					{
						tok.WarningMessage = "Number " + tok.Value + " too " + (tok.Value[0] == '-' ? "small" : "big") + ". Set to ";

						if (ishex || isoctal || !isdouble)
						{
							if (tok.Value[0] == '-')
								tok.ValueInt = Int32.MinValue;
							else
								tok.ValueInt = Int32.MaxValue;

							tok.ValueDouble = tok.ValueInt;
							tok.WarningMessage += tok.ValueInt;
						}
						else if (isdouble)
						{
							if (tok.Value[0] == '-')
								tok.ValueDouble = Double.MinValue;
							else
								tok.ValueDouble = Double.MaxValue;

							tok.ValueInt = (int)tok.ValueDouble;
							tok.WarningMessage += tok.ValueDouble;
						}
					}
                    catch (Exception)
                    {
                        reader.BaseStream.Position = cpos;
                        return null;
                    }

                    return tok;
                }
            }

            reader.BaseStream.Position = cpos;
            return null;
        }

        private ZScriptToken TryReadStringOrComment(bool allowstring, bool allowname, bool allowblock, bool allowline, bool allowregion)
        {
            long cpos = LastPosition = reader.BaseStream.Position;
            char c;
            try
            {
                c = reader.ReadChar();
            }
            catch (EndOfStreamException)
            {
                return null;
            }

            switch (c)
            {
                case '/': // comment
                    {
                        if (!allowblock && !allowline) break;
                        char cnext;
                        try
                        {
                            cnext = reader.ReadChar();
                        }
                        catch (EndOfStreamException)
                        {
                            break; // invalid
                        }
                        if (cnext == '/')
                        {
                            if (!allowline) break;
                            // line comment: read until newline but not including it
                            SB.Length = 0;
                            while (true)
                            {
                                try
                                {
                                    cnext = reader.ReadChar();
                                }
                                catch (EndOfStreamException)
                                {
                                    break;
                                }
                                if (cnext == '\n')
                                {
                                    reader.BaseStream.Position--;
                                    break;
                                }

                                SB.Append(cnext);
                            }

                            ZScriptToken tok = new ZScriptToken();
                            tok.Position = cpos;
                            tok.Type = ZScriptTokenType.LineComment;
                            tok.Value = SB.ToString();
                            return tok;
                        }
                        else if (cnext == '*')
                        {
                            if (!allowblock) break;
                            // block comment: read until closing sequence
                            SB.Length = 0;
                            while (true)
                            {
                                try
                                {
                                    cnext = reader.ReadChar();
                                }
                                catch (EndOfStreamException)
                                {
                                    break;
                                }
                                if (cnext == '*')
                                {
                                    char cnext2 = reader.ReadChar();
                                    if (cnext2 == '/')
                                        break;

                                    reader.BaseStream.Position--;
                                }

                                SB.Append(cnext);
                            }

                            ZScriptToken tok = new ZScriptToken();
                            tok.Position = cpos;
                            tok.Type = ZScriptTokenType.BlockComment;
                            tok.Value = SB.ToString();
                            return tok;
                        }
                        break;
                    }
                case '#': // #region and #endregion
                    {
                        if (!allowregion) break;
                        char cnext;
                        SB.Length = 0;

                        // Read until the end of the line
                        while (true)
                        {
                            try
							{
                                cnext = reader.ReadChar();
							}
                            catch(EndOfStreamException)
							{
                                break;
							}
                            if(cnext == '\n')
							{
                                reader.BaseStream.Position--;
                                break;
							}

                            SB.Append(cnext);
                        }

                        string line = SB.ToString();

                        // GZDoom actually doesn't care what's coming after #region or #endregion, so something like #regionlalala is valid.
                        // But they have to be in lower case
                        if (line.StartsWith("region") || line.StartsWith("endregion"))
                            return new ZScriptToken() { Position = cpos, Type = ZScriptTokenType.Region, Value = "" };

                        break;
                    }
                case '"':
                case '\'':
                    {
                        if ((c == '"' && !allowstring) || (c == '\'' && !allowname)) break;
                        ZScriptTokenType type = (c == '"' ? ZScriptTokenType.String : ZScriptTokenType.Name);
                        SB.Length = 0;
                        while (true)
                        {
                            try
                            {
                                // todo: parse escape sequences properly
                                char cnext = reader.ReadChar();
                                if (cnext == '\\') // escape sequence. right now, do nothing
                                {
                                    cnext = reader.ReadChar();
                                    SB.Append(cnext);
                                }
                                else if (cnext == c)
                                {
                                    ZScriptToken tok = new ZScriptToken();
                                    tok.Position = cpos;
                                    tok.Type = type;
                                    tok.Value = SB.ToString();
                                    return tok;
                                }
                                else SB.Append(cnext);
                            } catch (EndOfStreamException)
                            {
                                reader.BaseStream.Position = cpos;
                                return null; // bad string, unterminated and ends with EOF
                            }
                        }
                    }
            }

            reader.BaseStream.Position = cpos;
            return null;
        }

        private ZScriptToken TryReadNamedToken()
        {
            long cpos = LastPosition = reader.BaseStream.Position;

            // named tokens
            char[] namedtoken_buf = reader.ReadChars(namedtokentypesorder[0].Length);
            string namedtoken = new string(namedtoken_buf);
            foreach (string namedtokentype in namedtokentypesorder)
            {
                if (namedtoken.StartsWith(namedtokentype))
                {
                    // found the token.
                    reader.BaseStream.Position = cpos + namedtokentype.Length;
                    ZScriptToken tok = new ZScriptToken();
                    tok.Position = cpos;
                    tok.Type = namedtokentypes[namedtokentype];
                    tok.Value = namedtokentype;
                    return tok;
                }
            }

            reader.BaseStream.Position = cpos;
            return null;
        }

        public ZScriptToken ExpectToken(params ZScriptTokenType[] oneof)
        {
            long cpos = reader.BaseStream.Position;

            try
            {
                // try to expect whitespace
                if (oneof.Contains(ZScriptTokenType.Whitespace))
                {
                    ZScriptToken tok = TryReadWhitespace();
                    if (tok != null) return tok;
                }

                // try to expect an identifier
                if (oneof.Contains(ZScriptTokenType.Identifier))
                {
                    ZScriptToken tok = TryReadIdentifier();
                    if (tok != null) return tok;
                }

                bool blinecomment = oneof.Contains(ZScriptTokenType.LineComment);
                bool bblockcomment = oneof.Contains(ZScriptTokenType.BlockComment);
                bool bstring = oneof.Contains(ZScriptTokenType.String);
                bool bname = oneof.Contains(ZScriptTokenType.Name);
                bool bregion = oneof.Contains(ZScriptTokenType.Region);

                if (bstring || bname || bblockcomment || blinecomment || bregion)
                {
                    // try to expect a string
                    ZScriptToken tok = TryReadStringOrComment(bstring, bname, bblockcomment, blinecomment, bregion);
                    if (tok != null) return tok;
                }

                // if we are expecting a number (double or int), try to read it
                if (oneof.Contains(ZScriptTokenType.Integer) || oneof.Contains(ZScriptTokenType.Double))
                {
                    ZScriptToken tok = TryReadNumber();
                    if (tok != null && oneof.Contains(tok.Type)) return tok;
                }

                // try to expect special tokens
                // read max
                char[] namedtoken_buf = reader.ReadChars(namedtokentypesorder[0].Length);
                string namedtoken = new string(namedtoken_buf);
                foreach (ZScriptTokenType expected in oneof)
                {
                    // check if there is a value for this one
                    if (!namedtokentypesreverse.ContainsKey(expected))
                        continue;
                    string namedtokentype = namedtokentypesreverse[expected];
                    if (namedtoken.StartsWith(namedtokentype))
                    {
                        // found the token.
                        reader.BaseStream.Position = cpos + namedtokentype.Length;
                        ZScriptToken tok = new ZScriptToken();
                        tok.Position = cpos;
                        tok.Type = namedtokentypes[namedtokentype];
                        tok.Value = namedtokentype;
                        return tok;
                    }
                }
            }
            catch (Exception)
            {
                try
                {
                    reader.BaseStream.Position = cpos;
                }
                catch (Exception)
                {
                    /* ... */
                }

                return null;
            }

            // token was not found, try to read the token that was actually found and return that
            reader.BaseStream.Position = cpos;
            ZScriptToken invalid = ReadToken();
            if (invalid != null) invalid.IsValid = false;
            reader.BaseStream.Position = cpos;
            return invalid;
        }

        // short_circuit only checks for string literals and "everything else", reporting "everything else" as invalid tokens
        public ZScriptToken ReadToken(bool short_circuit = false)
        {
            try
            {
                ZScriptToken tok;

                // 
                tok = TryReadWhitespace();
                if (tok != null) return tok;

                if (!short_circuit)
                {
                    tok = TryReadIdentifier();
                    if (tok != null) return tok;

                    tok = TryReadNumber();
                    if (tok != null) return tok;
                }

                tok = TryReadStringOrComment(true, true, true, true, true);
                if (tok != null) return tok;

                if (!short_circuit)
                {
                    tok = TryReadNamedToken();
                    if (tok != null) return tok;
                }

                // token not found.
                tok = new ZScriptToken();
                tok.Position = reader.BaseStream.Position;
                tok.Type = ZScriptTokenType.Invalid;
                tok.Value = "" + reader.ReadChar();
                tok.IsValid = false;
                return tok;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string TokensToString(IEnumerable<ZScriptToken> tokens)
        {
            string outs = "";
            foreach (ZScriptToken tok in tokens)
                outs += tok.Value;
            return outs;
        }
    }
}
