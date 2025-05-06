#region ================== Namespaces

using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.Rendering;

#endregion

namespace CodeImp.DoomBuilder.ZDoom
{
	internal sealed class CvarInfoParser : ZDTextParser
	{
		#region ================== Variables

		private CvarsCollection cvars;

		#endregion

		#region ================== Properties

		internal override ScriptType ScriptType { get { return ScriptType.CVARINFO; } }

		public CvarsCollection Cvars { get { return cvars; } }

		#endregion

		#region ================== Constructor

		internal CvarInfoParser()
		{
			cvars = new CvarsCollection();

			// Required for the "handlertoken" format
			specialtokens += "()";
		}

		#endregion

		#region ================== Parsing

		public override bool Parse(TextResourceData data, bool clearerrors)
		{
			// Already parsed?
			if(!base.AddTextResource(data))
			{
				if(clearerrors) ClearError();
				return true;
			}

			// Cannot process?
			if(!base.Parse(data, clearerrors)) return false;

			// Continue until at the end of the stream
			HashSet<string> knowntypes = new HashSet<string> { "int", "float", "color", "bool", "string" };
			HashSet<string> flags = new HashSet<string> { "user", "server", "nosave", "noarchive", "cheat", "latch", "local" };
			while(SkipWhitespace(true))
			{
				string token = ReadToken().ToLowerInvariant();
				if(string.IsNullOrEmpty(token)) continue;

				// According to the ZDoom wikie (https://zdoom.org/wiki/CVARINFO) the format has to be
				//   <scope> [noarchive] [cheat] [latch] [handlerclass("<classname>")] <type> <name> [= <defaultvalue>];
				// where <scope> is one of "user", "server", or "nosave". This it just the intended format, GZDoom actually
				// accepts and combination of the scope variables (apparently for backwards compatibility), even when it
				// doesn't make sense.
				// Zandronum also has the "local" scope, which is not mentioned in the ZDoom wiki.
				// See https://github.com/jewalky/UltimateDoomBuilder/issues/748
				// handlerclass doesn't actually have to be in quotes

				if (flags.Contains(token))
				{
					// read (skip) flags and handlerclass
					while (true)
					{
						SkipWhitespace(true);
						token = ReadToken().ToLowerInvariant();

						if(token == "handlerclass")
						{
							if (!ParseHandlerClass())
								return false;
						}
						else if (!flags.Contains(token))
						{
							DataStream.Seek(-token.Length - 1, SeekOrigin.Current);
							break;
						}
					}

					// Next should be the type
					SkipWhitespace(true);
					string type = ReadToken().ToLowerInvariant();

					if (!knowntypes.Contains(type))
					{
						ReportError($"Unknown token '{type}'. Expected type of " + string.Join(", ", knowntypes));
						return false;
					}

					// Name
					SkipWhitespace(true);
					string name = ReadToken();

					if (string.IsNullOrEmpty(name))
					{
						ReportError("Expected cvar name");
						return false;
					}

					// Either "=" or ";"
					SkipWhitespace(true);
					token = ReadToken();

					switch (token)
					{
						case "=":
							SkipWhitespace(true);
							string value = ReadToken();

							if (string.IsNullOrEmpty(value))
							{
								ReportError("Expected \"" + name + "\" cvar value");
								return false;
							}

							// Add to collection
							if (!AddValue(name, type, value)) return false;

							// Next should be ";"
							if (!NextTokenIs(";")) return false;
							break;

						case ";":
							if (!AddValue(name, type, string.Empty)) return false;
							break;
					}
				}
				else
				{
					ReportError("Unknown keyword");
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Reads a handler class in the form of '(TheHandlerClass)' or '("TheHandlerClass")'.
		/// </summary>
		/// <returns>true if the correct fromat was detected, false if not</returns>
		private bool ParseHandlerClass()
		{
			string token = ReadToken();
			if (string.IsNullOrEmpty(token) || token != "(")
			{
				ReportError($"Expected token '(', got {token}");
				return false;
			}

			// The class can be an identifier or string literal
			token = ReadToken();
			if(!token.All(c => char.IsLetterOrDigit(c) || c == '"' || c == '_'))
			{
				ReportError($"Expected an alphanumeric string, got {token}");
				return false;
			}

			token = ReadToken();
			if (string.IsNullOrEmpty(token) || token != ")")
			{
				ReportError($"Expected token ')', got {token}");
				return false;
			}

			return true;
		}

		private bool AddValue(string name, string type, string value)
		{
			switch(type)
			{
				case "int":
					int iv = 0;
					if(!string.IsNullOrEmpty(value) && !ReadSignedInt(value, ref iv))
					{
						ReportError("Cvar \"" + name + "\" has invalid integer value: \"" + value + "\"");
						return false;
					}
					if(!cvars.AddValue(name, iv))
					{
						ReportError("Cvar \"" + name + "\" is double defined");
						return false;
					}
					break;

				case "float":
					float fv = 0f;
					if(!string.IsNullOrEmpty(value) && !ReadSignedFloat(value, ref fv))
					{
						ReportError("Cvar \"" + name + "\" has invalid decimal value: \"" + value + "\"");
						return false;
					}
					if(!cvars.AddValue(name, fv))
					{
						ReportError("Cvar \"" + name + "\" is double defined");
						return false;
					}
					break;

				case "color":
					PixelColor cv = new PixelColor();
					if(!string.IsNullOrEmpty(value) && !GetColorFromString(value, out cv))
					{
						ReportError("Cvar \"" + name + "\" has invalid color value: \"" + value + "\"");
						return false;
					}
					if(!cvars.AddValue(name, cv))
					{
						ReportError("Cvar \"" + name + "\" is double defined");
						return false;
					}
					break;
				
				case "bool":
					bool bv = false;
					if(!string.IsNullOrEmpty(value))
					{
						string sv = value.ToLowerInvariant();
						if(sv != "true" && sv != "false")
						{
							ReportError("Cvar \"" + name + "\" has invalid boolean value: \"" + value + "\"");
							return false;
						}
						bv = (sv == "true");
					}
					if(!cvars.AddValue(name, bv))
					{
						ReportError("Cvar \"" + name + "\" is double defined");
						return false;
					}
					break;
				
				case "string":
					if(!cvars.AddValue(name, StripQuotes(value)))
					{
						ReportError("Cvar \"" + name + "\" is double defined");
						return false;
					}
					break;
			}

			return true;
		}

		#endregion
	}
}
