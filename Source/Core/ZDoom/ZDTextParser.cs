
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
using System.Globalization;
using System.Text;
using System.IO;
using CodeImp.DoomBuilder.Compilers;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.Data.Scripting;
using CodeImp.DoomBuilder.Rendering;

#endregion

namespace CodeImp.DoomBuilder.ZDoom
{
	public abstract class ZDTextParser
	{
		#region ================== Constants

		protected static readonly string RELATIVE_PATH_MARKER = ".." + Path.DirectorySeparatorChar;
		protected static readonly string CURRENT_FOLDER_PATH_MARKER = "." + Path.DirectorySeparatorChar;
		protected static readonly string ALT_RELATIVE_PATH_MARKER = ".." + Path.AltDirectorySeparatorChar;
		protected static readonly string ALT_CURRENT_FOLDER_PATH_MARKER = "." + Path.AltDirectorySeparatorChar;
		
		#endregion
		
		#region ================== Variables
		
		// Parsing
		protected string whitespace = "\n \t\r\u00A0\0"; //mxd. non-breaking space is also space :)
		protected string specialtokens = ":{}+-\n;";
		protected bool skipregions; //mxd
		protected bool skipeditorcomments; //mxd
		
		// Input data stream
		protected Stream datastream;
		protected BinaryReader datareader;
		protected string sourcename;
		protected int sourcelumpindex; //mxd
		protected DataLocation datalocation; //mxd
		
		// Error report
		private int errorline;
		private string errordesc;
		private string errorsource; // Rooted path to the troubling file
		private string shorterrorsource; //mxd. Resource name + filename
		protected long prevstreamposition; //mxd. Text stream position storted before performing ReadToken.

		//mxd. Text lumps
		protected string textresourcepath;
		protected readonly Dictionary<string, ScriptResource> scriptresources;
		protected readonly HashSet<string> untrackedtextresources;
		#endregion
		
		#region ================== Properties
		
		internal Stream DataStream { get { return datastream; } }
		internal BinaryReader DataReader { get { return datareader; } }
		public int ErrorLine { get { return errorline; } }
		public string ErrorDescription { get { return errordesc; } }
		public string ErrorSource { get { return errorsource; } }
		public bool HasError { get { return (errordesc != null); } }
		internal abstract ScriptType ScriptType { get; } //mxd
		internal Dictionary<string, ScriptResource> ScriptResources { get { return scriptresources; } } //mxd

		#endregion
		
		#region ================== Constructor / Disposer
		
		// Constructor
		protected ZDTextParser()
		{
            // Initialize
			errordesc = null;
			scriptresources = new Dictionary<string, ScriptResource>(StringComparer.OrdinalIgnoreCase); //mxd
			untrackedtextresources = new HashSet<string>(StringComparer.OrdinalIgnoreCase); //mxd
			skipregions = true; //mxd
		}
		
		#endregion

		#region ================== Parsing

		//mxd. This parses the given decorate stream. Returns false on errors
		public virtual bool Parse(TextResourceData parsedata, bool clearerrors)
		{
			// Clear error status?
			if(clearerrors) ClearError();
			
			// Integrity checks
			// INFO: MapManager.CompileLump() prepends lumpname with "?" to distinguish between temporary files and files compiled in place
			// We don't want this to show up in error messages
			if(parsedata.Stream == null)
			{
				ReportError("Unable to load \"" + parsedata.Filename.Replace("?", "") + "\"");
				return false;
			}

			if(parsedata.Stream.Length == 0)
			{
				if(!string.IsNullOrEmpty(sourcename) && sourcename != parsedata.Filename)
				{
					LogWarning("Include file \"" + parsedata.Filename.Replace("?", "") + "\" is empty");
				}
				else
				{
					sourcename = parsedata.Filename; // LogWarning() needs "sourcename" property to properly log the warning...
					LogWarning("File is empty");
				}
			}

			datastream = parsedata.Stream;
			datareader = new BinaryReader(parsedata.Stream, Encoding.ASCII);
			sourcename = parsedata.Filename;
			sourcelumpindex = parsedata.LumpIndex;
			datalocation = parsedata.SourceLocation;
			datastream.Seek(0, SeekOrigin.Begin);

			return true;
		}

		//mxd
		protected bool AddTextResource(TextResourceData parsedata)
		{
			// Script Editor resources don't have actual path and should always be parsed
			if(string.IsNullOrEmpty(parsedata.SourceLocation.location))
			{
				if(parsedata.Trackable) throw new NotSupportedException("Trackable TextResource must have a valid path.");
				return true;
			}

			string path = Path.Combine(parsedata.SourceLocation.location, parsedata.Filename + (parsedata.LumpIndex != -1 ? "#" + parsedata.LumpIndex : ""));
			if(scriptresources.ContainsKey(path) || untrackedtextresources.Contains(path))
				return false;

			//mxd. Create TextResource for this file
			if(parsedata.Trackable)
			{
				textresourcepath = path;
				ScriptResource res = new ScriptResource(parsedata, this.ScriptType);
				scriptresources.Add(textresourcepath, res);
			}
			// Track the untrackable!
			else
			{
				untrackedtextresources.Add(path);
				textresourcepath = string.Empty;
			}

			return true;
		}
		
		// This returns true if the given character is whitespace
		protected internal bool IsWhitespace(char c)
		{
			return (whitespace.IndexOf(c) > -1);
		}

		// This returns true if the given character is a special token
		private bool IsSpecialToken(char c)
		{
			return (specialtokens.IndexOf(c) > -1);
		}

		// This returns true if the given character is a special token
		protected internal bool IsSpecialToken(string s)
		{
			if(s.Length > 0) return (specialtokens.IndexOf(s[0]) > -1);
			return false;
		}

		//mxd. This removes beginning and ending quotes from a token
		protected internal string StripTokenQuotes(string token) 
		{
			return StripQuotes(token);
		}
		
		// This removes beginning and ending quotes from a token
		internal static string StripQuotes(string token)
		{
			// Remove first character, if it is a quote
			if(!string.IsNullOrEmpty(token) && (token[0] == '"'))
				token = token.Substring(1);
			
			// Remove last character, if it is a quote
			if(!string.IsNullOrEmpty(token) && (token[token.Length - 1] == '"'))
				token = token.Substring(0, token.Length - 1);
			
			return token;
		}

		//mxd
		internal bool ReadTextureName(out string name) { return ReadTextureName(out name, TextureNamespace.WALLTEXTURE); }
		internal bool ReadTextureName(out string name, TextureNamespace texturenamespace)
		{
			string token = ReadToken(false);
			name = StripQuotes(token);

			if(!string.IsNullOrEmpty(name)
				&& name.Length > DataManager.CLASIC_IMAGE_NAME_LENGTH
				&& name.Length == token.Length)
			{
				ReportError("Long " + texturenamespace.ToString() + " names must be quoted. See \"" + token + "\"");
				return false;
			}

			return true;
		}
		
		// This skips whitespace on the stream, placing the read
		// position right before the first non-whitespace character
		// Returns false when the end of the stream is reached
		protected internal bool SkipWhitespace(bool skipnewline)
		{
			int offset = skipnewline ? 0 : 1;
			char c;
			prevstreamposition = datastream.Position; //mxd
			
			do
			{
				if(datastream.Position == datastream.Length) return false;
				c = (char)datareader.ReadByte();

				// Check if this is comment
				if(c == '/')
				{
					if(datastream.Position == datastream.Length) return false;
					char c2 = (char)datareader.ReadByte();
					if(c2 == '/')
					{
						// Check if not a special comment with a token
						if(datastream.Position == datastream.Length) return false;
						char c3 = (char)datareader.ReadByte();
						if(skipeditorcomments || c3 != '$') //mxd. Added skipeditorcomments
						{
							// Skip entire line
							char c4 = c3;
							while((c4 != '\n') && (datastream.Position < datastream.Length)) { c4 = (char)datareader.ReadByte(); }
							
							// Reached end of file?
							if (datastream.Position == datastream.Length)
								return true;

							c = c4;
						}
						else
						{
							// Not a comment
							c = c3;
						}
					}
					else if(c2 == '*')
					{
						// Skip until */
						char c4, c3 = '\0';
						prevstreamposition = datastream.Position; //mxd
						do
						{
							if(datastream.Position == datastream.Length) //mxd
							{
								// ZDoom doesn't give even a warning message about this, so we shouldn't report error or fail parsing.
								string message = ScriptType + " warning in \"" + sourcename + "\", line " + GetCurrentLineNumber() + ". Block comment is not closed.";
								TextResourceErrorItem error = new TextResourceErrorItem(ErrorType.Warning, ScriptType, datalocation, sourcename, sourcelumpindex, GetCurrentLineNumber(), message);
								General.ErrorLogger.Add(error);
								return false;
							}

							c4 = c3;
							c3 = (char)datareader.ReadByte();
						}
						while((c4 != '*') || (c3 != '/'));
						c = ' ';
					}
					else
					{
						// Not a comment, rewind from reading c2
						datastream.Seek(-1, SeekOrigin.Current);
					}
				}
				//mxd. Region/endregion handling
				else if(skipregions && c == '#')
				{
					long startpos = datastream.Position - 1;
					string s = ReadToken(false).ToLowerInvariant();
					if(s == "region" || s == "endregion")
					{
						// Skip entire line
						char ch = ' ';
						while((ch != '\n') && (datastream.Position < datastream.Length)) { ch = (char)datareader.ReadByte(); }
						c = ch;
					}
					else
					{
						// Rewind so this structure can be read again
						DataStream.Seek(startpos, SeekOrigin.Begin);
						return true;
					}
				}
			}
			while(whitespace.IndexOf(c, offset) > -1);
			
			// Go one character back so we can read this non-whitespace character again
			datastream.Seek(-1, SeekOrigin.Current);
			return true;
		}
		
		// This reads a token (all sequential non-whitespace characters or a single character)
		// Returns null when the end of the stream has been reached
		protected internal string ReadToken() { return ReadToken(true); } //mxd. Added "multiline" param
		protected internal string ReadToken(bool multiline)
		{
			//mxd. Return empty string when the end of the stream has been reached
			if(datastream.Position == datastream.Length) return string.Empty;
			
			//mxd. Store starting position 
			prevstreamposition = datastream.Position;
			
			string token = "";
			bool quotedstring = false;
			
			// Start reading
			char c = (char)datareader.ReadByte();
			while(!IsWhitespace(c) || quotedstring || IsSpecialToken(c))
			{
				//mxd. Break at newline?
				if(!multiline && c == '\r')
				{
					// Go one character back so line number is correct
					datastream.Seek(-1, SeekOrigin.Current);
					return token;
				}
				
				// Special token?
				if(!quotedstring && IsSpecialToken(c))
				{
					// Not reading a token yet?
					if(token.Length == 0)
					{
						// This is our whole token
						token += c;
						break;
					}
					else
					{
						// This is a new token and shouldn't be read now
						// Go one character back so we can read this token again
						datastream.Seek(-1, SeekOrigin.Current);
						break;
					}
				}
				else
				{
					// Quote?
					if(c == '"')
					{
						// Quote to end the string?
						if(quotedstring) quotedstring = false;
						
						// First character is a quote?
						if(token.Length == 0) quotedstring = true;
						
						token += c;

						//mxd. Break after closing quote
						if(!quotedstring) break;
					}
					// Potential comment?
					else if((c == '/') && !quotedstring)
					{
						// Check the next byte
						if(datastream.Position == datastream.Length) return token;
						char c2 = (char)datareader.ReadByte();
						if((c2 == '/') || (c2 == '*'))
						{
							// This is a comment start, so the token ends here
							// Go two characters back so we can read this comment again
							datastream.Seek(-2, SeekOrigin.Current);
							break;
						}
						else
						{
							// Not a comment
							// Go one character back so we can read this char again
							datastream.Seek(-1, SeekOrigin.Current);
							token += c;
						}
					}
					else
					{
						token += c;
					}
				}
				
				// Next character
				if(datastream.Position < datastream.Length)
					c = (char)datareader.Read();
				else
					break;
			}
			
			return token;
		}

		// This reads a token (all sequential non-whitespace characters or a single character) using custom set of special tokens
		// Returns null when the end of the stream has been reached (mxd)
		protected internal string ReadToken(string specialTokens) 
		{
			// Return null when the end of the stream has been reached
			if(datastream.Position == datastream.Length) return null;

			//mxd. Store starting position 
			prevstreamposition = datastream.Position;
			
			string token = "";
			bool quotedstring = false;

			// Start reading
			char c = (char)datareader.ReadByte();
			while(!IsWhitespace(c) || quotedstring || specialTokens.IndexOf(c) != -1) 
			{
				// Special token?
				if(!quotedstring && specialTokens.IndexOf(c) != -1) 
				{
					// Not reading a token yet?
					if(token.Length == 0) 
					{
						// This is our whole token
						token += c;
						break;
					} 

					// This is a new token and shouldn't be read now
					// Go one character back so we can read this token again
					datastream.Seek(-1, SeekOrigin.Current);
					break;
				} 
				else 
				{
					// Quote?
					if(c == '"') 
					{
						// Quote to end the string?
						if(quotedstring) quotedstring = false;

						// First character is a quote?
						if(token.Length == 0) quotedstring = true;

						token += c;

						//mxd. Break after closing quote
						if(!quotedstring) break;
					}
					// Potential comment?
					else if((c == '/') && !quotedstring) 
					{
						// Check the next byte
						if(datastream.Position == datastream.Length) return token;
						char c2 = (char)datareader.ReadByte();
						if((c2 == '/') || (c2 == '*')) 
						{
							// This is a comment start, so the token ends here
							// Go two characters back so we can read this comment again
							datastream.Seek(-2, SeekOrigin.Current);
							break;
						} 
						else 
						{
							// Not a comment
							// Go one character back so we can read this char again
							datastream.Seek(-1, SeekOrigin.Current);
							token += c;
						}
					} 
					else 
					{
						token += c;
					}
				}

				// Next character
				if(datastream.Position < datastream.Length)
					c = (char)datareader.Read();
				else
					break;
			}

			return token;
		}

		// This reads the rest of the line
		// Returns null when the end of the stream has been reached
		protected internal string ReadLine()
		{
			string token = "";

			// Return null when the end of the stream has been reached
			if(datastream.Position == datastream.Length) return null;

			// Start reading
			char c = (char)datareader.ReadByte();
			while(c != '\n')
			{
				token += c;
				
				// Next character
				if(datastream.Position < datastream.Length)
					c = (char)datareader.Read();
				else
					break;
			}

			return token.Trim();
		}

		//mxd
		internal bool NextTokenIs(string expectedtoken) 
		{
			return NextTokenIs(expectedtoken, true);
		}

		//mxd
		internal bool NextTokenIs(string expectedtoken, bool reporterror) 
		{
			if(!SkipWhitespace(true))
			{
				if(reporterror) ReportError("Unexpected end of the structure");
				return false;
			}

			long prevstreamposition = DataStream.Position;

			string token = ReadToken();

			if(string.Compare(token, expectedtoken, true) != 0)
			{
				if(reporterror) ReportError("Expected \"" + expectedtoken + "\", but got \"" + token + "\"");

				// Rewind so this structure can be read again
				DataStream.Seek(prevstreamposition, SeekOrigin.Begin);
				return false;
			}

			return true;
		}

		//mxd
		protected internal bool ReadSignedFloat(ref float value) { return ReadSignedFloat(ReadToken(false), ref value); }
		protected internal bool ReadSignedFloat(string token, ref float value) 
		{
			int sign = 1;
			if(token == "-") 
			{
				sign = -1;
				token = ReadToken(false);
			}

			float val;
			bool success = float.TryParse(token, NumberStyles.Float, CultureInfo.InvariantCulture, out val);
			if(success) value = val * sign;
			return success;
		}

		//mxd
		protected internal bool ReadSignedInt(ref int value) { return ReadSignedInt(ReadToken(false), ref value); }
		protected internal bool ReadSignedInt(string token, ref int value) 
		{
			int sign = 1;
			if(token == "-") 
			{
				sign = -1;
				token = ReadToken(false);
			}

			int val;
			bool success = int.TryParse(token, NumberStyles.Integer, CultureInfo.InvariantCulture, out val);
			if(success) value = val * sign;
			return success;
		}

		//mxd
		protected internal bool ReadByte(ref byte value) { return ReadByte(ReadToken(false), ref value); }
		protected internal bool ReadByte(string token, ref byte value)
		{
			if(token == "-") return false;

			int result;
			if(!int.TryParse(token, NumberStyles.Integer, CultureInfo.InvariantCulture, out result) || result < 0 || result > 255)
			{
				return false;
			}

			value = (byte)result;
			return true;
		}

		//mxd. This replicates ZDoom's V_GetColorFromString method
		public static bool GetColorFromString(string name, out PixelColor color)
		{
			name = StripQuotes(name.Replace(" ", ""));

			// Check for HTML-style #RRGGBB or #RGB color string
			bool ishtmlcolor = false;
			if(name.StartsWith("#"))
			{
				ishtmlcolor = true;
				name = name.Remove(0, 1);

				// Expand RGB to RRGGBB
				if(name.Length == 3)
				{
					name = name[0].ToString() + name[0] + name[1] + name[1] + name[2] + name[2];
				}
				else if(name.Length != 6)
				{
					// Bad HTML-style; pretend it's black.
					color = new PixelColor();
					return true;
				}
			}

			// Probably it's a hex color (like FFCC11)?
			int ci;
			if(int.TryParse(name, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out ci))
			{
				color = PixelColor.FromInt(ci).WithAlpha(255);
				return true;
			}

			// Probably it's a color name?
			if(!ishtmlcolor && General.Map.Data.KnownColors.ContainsKey(name))
			{
				color = General.Map.Data.KnownColors[name];
				return true;
			}

            color = new PixelColor();
            return false;
		}

		//mxd
		protected void SkipStructure() { SkipStructure(new HashSet<string>()); }
		protected void SkipStructure(HashSet<string> breakat)
		{
			// We need it to be case-insensitive
			if(breakat.Count > 0) breakat = new HashSet<string>(breakat, StringComparer.OrdinalIgnoreCase);
			
			string token;
			do
			{
				if(!SkipWhitespace(true)) break;
				token = ReadToken();
				if(string.IsNullOrEmpty(token)) break;
				if(breakat.Contains(token))
				{
					DataStream.Seek(-token.Length - 1, SeekOrigin.Current);
					return;
				}
			}
			while(token != "{");
			int scopelevel = 1;
			do
			{
				if(!SkipWhitespace(true)) break;
				token = ReadToken();
				if(string.IsNullOrEmpty(token)) break;
				if(token == "{") scopelevel++;
				if(token == "}") scopelevel--;
			}
			while(scopelevel > 0);
		}
		
		// This reports an error
		protected internal void ReportError(string message)
		{
			// Set error information
			errordesc = message;
			errorline = (datastream != null ? GetCurrentLineNumber() : CompilerError.NO_LINE_NUMBER); //mxd
			
			//mxd
			if(ScriptType == ScriptType.ACS && sourcename.StartsWith("?"))
			{
				shorterrorsource = sourcename.Substring(1);
				errorsource = sourcename;
			}
			else
			{
				shorterrorsource = Path.Combine(datalocation.GetDisplayName(), sourcename).Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
				errorsource = Path.Combine(datalocation.location, sourcename);
			}
			
			if(sourcelumpindex != -1) //mxd
			{
				errorsource += ":" + sourcelumpindex;
				shorterrorsource += ":" + sourcelumpindex;
			}
		}

		//mxd. This adds a warning to the ErrorLogger
		protected internal void LogWarning(string message) { LogWarning(message, CompilerError.NO_LINE_NUMBER); }
		protected internal virtual void LogWarning(string message, int linenumber)
		{
			// Add a warning
			int errline = (linenumber != CompilerError.NO_LINE_NUMBER 
				? linenumber 
				: (datastream != null ? GetCurrentLineNumber() : CompilerError.NO_LINE_NUMBER));
			string errsource = (ScriptType == ScriptType.ACS && sourcename.StartsWith("?") ? sourcename.Substring(1) : Path.Combine(datalocation.GetDisplayName(), sourcename));
			if(sourcelumpindex != -1) errsource += ":" + sourcelumpindex;

			message = ScriptType + " warning in \"" + errsource
			          + (errline != CompilerError.NO_LINE_NUMBER ? "\", line " + (errline + 1) : "\"") + ". "
			          + message + ".";

			TextResourceErrorItem error = new TextResourceErrorItem(ErrorType.Warning,
				ScriptType, datalocation,
				(sourcename.StartsWith("?") ? sourcename.Substring(1) : sourcename),
				sourcelumpindex, errline, message);

			General.ErrorLogger.Add(error);
		}

		//mxd. This adds an error to the ErrorLogger
		public void LogError()
		{
			string message = ScriptType + " error in \"" + shorterrorsource
			                 + (errorline != CompilerError.NO_LINE_NUMBER ? "\", line " + (errorline + 1) : "\"") + ". "
			                 + errordesc + ".";
			
			TextResourceErrorItem error = new TextResourceErrorItem(ErrorType.Error,
				ScriptType, datalocation,
				(sourcename.StartsWith("?") ? sourcename.Substring(1) : sourcename),
				sourcelumpindex, errorline, message);

			General.ErrorLogger.Add(error);
		}

		//mxd
		protected void ClearError()
		{
			errordesc = null;
			errorsource = null;
			shorterrorsource = null;
			errorline = CompilerError.NO_LINE_NUMBER;
		}

		//mxd 
		protected virtual int GetCurrentLineNumber()
		{
			long pos = datastream.Position;
			long finishpos = (prevstreamposition >= 0 ? Math.Min(prevstreamposition, pos) : pos);
			long readpos = 0;
			int linenumber = -1;

			// Find the line on which we found this error
			datastream.Seek(0, SeekOrigin.Begin);
			StreamReader textreader = new StreamReader(datastream, Encoding.ASCII);
			while(readpos < finishpos + 1) 
			{
				string line = textreader.ReadLine();
				if(line == null) break;
				readpos += line.Length + 2;
				linenumber++;
			}

			// Return to original position
			datastream.Seek(pos, SeekOrigin.Begin);

			return Math.Max(linenumber, 0);
		}

		//mxd. This converts given path to be relative to "filename"
		protected string GetRootedPath(string filename, string includefilename)
		{
			// Construct root-relative path...
			filename = filename.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);

			// Filename absolute? Try to convert it to resource-rooted
			if(Path.IsPathRooted(filename))
			{
				foreach(DataReader reader in General.Map.Data.Containers)
				{
					if(reader is DirectoryReader && filename.StartsWith(reader.Location.location, StringComparison.OrdinalIgnoreCase))
					{
						filename = filename.Substring(reader.Location.location.Length + 1, filename.Length - reader.Location.location.Length - 1);
						break;
					}
				}
			}
			
			string filepath = Path.GetDirectoryName(filename);
			string result = includefilename.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);

			if(result.StartsWith(RELATIVE_PATH_MARKER) && !string.IsNullOrEmpty(filepath))
			{
				string[] parts = filepath.Split(Path.DirectorySeparatorChar);
				int index = parts.Length - 1;
				int pos;

				// Count & trim relative path markers
				while((pos = result.LastIndexOf(RELATIVE_PATH_MARKER, StringComparison.Ordinal)) != -1)
				{
					// includefilename references something above the root?
					if(index-- < 0)
					{
						ReportError("Unable to construct rooted path from \"" + includefilename + "\"");
						return string.Empty;
					}

					string start = result.Substring(0, pos);
					string end = result.Substring(pos + RELATIVE_PATH_MARKER.Length, result.Length - pos - RELATIVE_PATH_MARKER.Length);
					result = start + end;
				}

				// Construct absolute path relative to current filename
				while(index > -1)
				{
					result = parts[index--] + Path.DirectorySeparatorChar + result;
				}
			}
			else
			{
				// Trim "this folder" marker
				if(result.StartsWith(CURRENT_FOLDER_PATH_MARKER))
					result = result.TrimStart(CURRENT_FOLDER_PATH_MARKER.ToCharArray());

				// Treat as relative path
				if(!string.IsNullOrEmpty(filepath))
					result = Path.Combine(filepath, result);
			}

			return result;
		}

		//mxd. This replicates System.IO.Path.CheckInvalidPathChars() internal function
		public bool CheckInvalidPathChars(string path)
		{
			foreach(char c in path)
			{
				int num = c;
				switch(num)
				{
					case 34: case 60: case 62: case 124:
						ReportError("Unsupported character \"" + c + "\" in path \"" + path + "\"");
						return false;

					default:
						if(num >= 32) continue;
						goto case 34;
				}
			}

			return true;
		}

		#endregion
	}
}
