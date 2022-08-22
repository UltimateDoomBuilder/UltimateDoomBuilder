
#region ================== Namespaces

using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Data;

#endregion

//mxd. Parser used to determine which script type given text is.
namespace CodeImp.DoomBuilder.ZDoom.Scripting
{
	internal sealed class ScriptTypeParserSE : ZDTextParser
	{
		internal override ScriptType ScriptType { get { return scripttype; } }
		private ScriptType scripttype;

		internal ScriptTypeParserSE()
		{
			scripttype = ScriptType.UNKNOWN;
		}

		public override bool Parse(TextResourceData data, bool clearerrors)
		{
			//mxd. Already parsed?
			if(!base.AddTextResource(data))
			{
				if(clearerrors) ClearError();
				return true;
			}

			// Cannot process?
			if(!base.Parse(data, clearerrors)) return false;

			// Continue until at the end of the stream
			while(SkipWhitespace(true))
			{
				string token = ReadToken();
                long cpos = datastream.Position;

				if(!string.IsNullOrEmpty(token))
				{
					token = token.ToUpperInvariant();

					if(token == "MODEL")
					{
						SkipWhitespace(true);
						ReadToken(); //should be model name
						SkipWhitespace(true);
						token = ReadToken();//should be opening brace

						if(token == "{")
						{
							scripttype = ScriptType.MODELDEF;
							return true;
						}

					}
					else if(token == "SCRIPT")
					{
						SkipWhitespace(true);
						ReadToken(); //should be script name or number
						SkipWhitespace(true);
						ReadToken(); //should be script parameters/type
						SkipWhitespace(true);
						token = ReadToken(); //should be opening brace

						if(token == "{")
						{
							scripttype = ScriptType.ACS;
							return true;
						}

					}
					else if(token == "ACTOR") // [ZZ] note: by the looks of it, this doesn't handle the case when we write actor with DoomEdNum.
					{
						SkipWhitespace(true);
						ReadToken(); //should be actor name

						SkipWhitespace(true);
						token = ReadToken();

                        // [ZZ] note: original code compared token to REPLACES without doing ToUpper
						if(token == ":" || token == "{" || (token != null && token.ToUpperInvariant() == "REPLACES"))
						{
							scripttype = ScriptType.DECORATE;
							return true;
						}

						SkipWhitespace(true);
						token = ReadToken(); //should be actor name

                        // [ZZ]
                        if (token != "{") // actor bla : bla2 10666 {
                        {
                            SkipWhitespace(true);
                            token = ReadToken();
                        }

						if(token == "{")
						{
							scripttype = ScriptType.DECORATE;
							return true;
						}
					}
                    else if(token == "CLASS" || token == "STRUCT" || token == "ENUM" || token == "EXTEND")
                    {
                        if (token == "EXTEND")
                        {
                            SkipWhitespace(true);
                            token = ReadToken();
                            if (!string.IsNullOrEmpty(token))
                                token = token.ToUpperInvariant();
                        }

                        string otoken = token; // original token

                        SkipWhitespace(true);
                        ReadToken(); //should be actor name

                        SkipWhitespace(true);
                        token = ReadToken();

                        if ((otoken != "ENUM" && (token == ":" || token == ";")) || token == "{" || (otoken == "CLASS" && (token != null && token.ToUpperInvariant() == "REPLACES")))
                        {
                            scripttype = ScriptType.ZSCRIPT;
                            return true;
                        }

                        SkipWhitespace(true);
                        token = ReadToken(); //should be actor name

                        if (token == "{" || token == ";")
                        {
                            scripttype = ScriptType.ZSCRIPT;
                            return true;
                        }
                        return true;
                    }
				}

                datastream.Position = cpos; // [ZZ] read next token, not whatever is left after possibly unsuccessful parsing.
			}

			return false;
		}
	}
}
