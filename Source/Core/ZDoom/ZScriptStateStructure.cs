using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CodeImp.DoomBuilder.ZDoom
{
    public sealed class ZScriptStateStructure : StateStructure
    {
        ZScriptToken TryReadSprite(ZScriptParser parser, Stream stream, ZScriptTokenizer tokenizer)
        {
            // this is a hack to read #### and ----.
            // also [\]
            string specialinvalid = "-#";
            string outs = "";
            long cpos = stream.Position;
            for (int i = 0; ; i++)
            {
                cpos = stream.Position;
                ZScriptToken token = tokenizer.ReadToken(true);

                if ((token == null) ||
                    (token.Type != ZScriptTokenType.Invalid) ||
                    (token.Value == ";") ||
                    ((outs.Length > 0 && token.Value[0] != outs[0]) && specialinvalid.Contains(token.Value[0])))
                {
                    if (outs.Length == 0 && (token.Type == ZScriptTokenType.String || token.Type == ZScriptTokenType.Name))
                        return token;

                    stream.Position = cpos;
                    break;
                }

                outs += token.Value[0];
            }

            if (outs.Length > 0)
            {
                ZScriptToken tok = new ZScriptToken();
                tok.Position = cpos;
                tok.Type = ZScriptTokenType.String;
                tok.Value = outs;
                return tok;
            }

            stream.Position = cpos;
            return null;
        }

        internal ZScriptStateStructure(ActorStructure actor, ZDTextParser zdparser)
        {
            ZScriptParser parser = (ZScriptParser)zdparser;
            Stream stream = parser.DataStream;
            ZScriptTokenizer tokenizer = new ZScriptTokenizer(parser.DataReader);
            parser.tokenizer = tokenizer;

            // todo: parse stuff
            // 
            string[] control_keywords = new string[] { "goto", "loop", "wait", "fail", "stop" };
			string[] data_types = new string[] { "double", "int", "uint" };
            
            while (true)
            {
                // expect identifier or string.
                // if it's an identifier, it can be goto/loop/wait/fail/stop.
                // if it's a string, it's always a sprite name.
                tokenizer.SkipWhitespace();
                long cpos = stream.Position;
                ZScriptToken token = tokenizer.ExpectToken(ZScriptTokenType.Identifier, ZScriptTokenType.String, ZScriptTokenType.CloseCurly);
                if (token == null || !token.IsValid)
                {
                    ZScriptToken _token = TryReadSprite(parser, stream, tokenizer);
                    if (_token == null)
                    {
                        parser.ReportError("Expected identifier or string, got " + ((Object)token ?? "<null>").ToString());
                        return;
                    }

                    token = _token;
                }

                if (token.Type == ZScriptTokenType.CloseCurly)
                {
                    stream.Position--;
                    break; // done
                }
                else if (token.Type == ZScriptTokenType.Identifier)
                {
                    string s_keyword = token.Value.ToLowerInvariant();
                    if (control_keywords.Contains(s_keyword))
                    {
                        if (s_keyword == "goto") // just use decorate goto here. should work. but check for semicolon!
                        {
                            gotostate = new ZScriptStateGoto(actor, parser);
                            parser.tokenizer = tokenizer;
                        }

                        //parser.LogWarning(string.Format("keyword {0}", s_keyword));

                        tokenizer.SkipWhitespace();
                        token = tokenizer.ExpectToken(ZScriptTokenType.Semicolon);
                        if (token == null || !token.IsValid)
                        {
                            parser.ReportError("Expected ;, got " + ((Object)token ?? "<null>").ToString());
                            return;
                        }

                        continue;
                    }

					tokenizer.SkipWhitespace(); // Skip whitepace because there might be whitepsace between the state label and the colon. See https://github.com/jewalky/UltimateDoomBuilder/issues/631
				}

                // make sure it's not a label of the next state. if it is, break out.
                long cpos2 = stream.Position; // local rewind point
                ZScriptToken token2 = tokenizer.ExpectToken(ZScriptTokenType.Colon, ZScriptTokenType.Dot);
                bool nextstate = (token2 != null && token2.IsValid);
                stream.Position = cpos2; // rewind to before state label read
                if (nextstate)
                {
                    stream.Position = cpos;
                    break;
                }

                // it's a frame definition. read it.
                string spritename = token.Value.ToLowerInvariant();
                if (spritename.Length != 4)
                {
                    parser.ReportError("Sprite name should be exactly 4 characters long (got " + spritename + ")");
                    return;
                }

                tokenizer.SkipWhitespace();

                token = TryReadSprite(parser, stream, tokenizer);
                if (token == null)
                {
                    parser.ReportError("Expected sprite frame(s)");
                    return;
                }

                string spriteframes = token.Value;

                //parser.LogWarning(string.Format("sprite {0} {1}", spritename, spriteframes));

                // duration
                int duration;
                tokenizer.SkipWhitespace();
                // this can be a function call, or a constant.
                token = tokenizer.ExpectToken(ZScriptTokenType.Identifier);
                if (token != null && token.IsValid)
                {
					// Known data type? Then it's hopefully the .min or .max value (like int.min or int.max)
					if (data_types.Contains(token.Value))
					{
						token = tokenizer.ExpectToken(ZScriptTokenType.Dot);
						if(token == null || !token.IsValid)
						{
							parser.ReportError("Expected ., got " + ((Object)token ?? "<null>").ToString());
							return;
						}

						token = tokenizer.ExpectToken(ZScriptTokenType.Identifier);
						if (token == null || !token.IsValid)
						{
							parser.ReportError("Expected an identifier, got " + ((Object)token ?? "<null>").ToString());
							return;
						}

						if (token.Value == "min")
						{
							duration = int.MinValue;
						}
						else if (token.Value == "max")
						{
							duration = int.MaxValue;
						}
						else
						{
							parser.ReportError("Expected min or max, got " + ((Object)token ?? "<null>").ToString());
							return;
						}
					}
					else // No known data type
					{
						duration = -1;
						tokenizer.SkipWhitespace();
						token = tokenizer.ExpectToken(ZScriptTokenType.OpenParen);
						if (token != null && token.IsValid)
						{
							List<ZScriptToken> tokens = parser.ParseExpression(true);
							tokenizer.SkipWhitespace();
							token = tokenizer.ExpectToken(ZScriptTokenType.CloseParen);
							if (token == null || !token.IsValid)
							{
								parser.ReportError("Expected ), got " + ((Object)token ?? "<null>").ToString());
								return;
							}
						}
					}
                }
                else
                {
                    if (!parser.ParseInteger(out duration))
                        return;
                }

                // now, it can also contain BRIGHT, LIGHT(), OFFSET()
                string[] allspecials = new string[] { "bright", "light", "offset", "fast", "slow", "nodelay", "canraise" };
                HashSet<string> specials = new HashSet<string>();
                // maybe something else. I don't know.
                FrameInfo info = new FrameInfo();

                // Make the sprite name
                string realspritename = (spritename + spriteframes[0]).ToUpperInvariant();

                // Ignore some odd ZDoom things
                if (/*!realspritename.StartsWith("TNT1") && */!realspritename.StartsWith("----") && !realspritename.Contains("#")) // [ZZ] some actors have only TNT1 state and receive a random image because of this
                {
                    info.Sprite = realspritename; //mxd
                    info.Duration = duration;
                    sprites.Add(info);
                }

                while (true)
                {
                    tokenizer.SkipWhitespace();
                    cpos2 = stream.Position;
                    token = tokenizer.ExpectToken(ZScriptTokenType.Identifier, ZScriptTokenType.Semicolon, ZScriptTokenType.OpenCurly);
                    if (token == null || !token.IsValid)
                    {
                        parser.ReportError("Expected identifier, ;, or {, got " + ((Object)token ?? "<null>").ToString());
                        return;
                    }

                    // if it's opencurly, it means that everything else is an anonymous block.
                    // skip/parse that.
                    // if it's semicolon, it means end of the frame.
                    // if it's BRIGHT, LIGHT() or OFFSET(), it should be processed.
                    // if it's something else (but identifier), then it's an action function call, process it.
                    if (token.Type == ZScriptTokenType.OpenCurly)
                    {
                        stream.Position--;
                        if (!parser.SkipBlock())
                            return;
                        break; // this block is done
                    }
                    else if (token.Type == ZScriptTokenType.Semicolon)
                    {
                        break; // done
                    }
                    else // identifier
                    {
                        string special = token.Value.ToLowerInvariant();
                        if (allspecials.Contains(special))
                        {
                            if (specials.Contains(special))
                            {
                                parser.ReportError("'" + special + "' cannot be used twice");
                                return;
                            }

                            specials.Add(special);

                            if (special == "bright")
                            {
                                info.Bright = true;
                            }
                            else if (special == "light" || special == "offset")
                            {
                                tokenizer.SkipWhitespace();
                                token = tokenizer.ExpectToken(ZScriptTokenType.OpenParen);
                                if (token == null || !token.IsValid)
                                {
                                    parser.ReportError("Expected (, got " + ((Object)token ?? "<null>").ToString());
                                    return;
                                }

                                List<ZScriptToken> tokens = parser.ParseExpression(true);
                                tokenizer.SkipWhitespace();
                                token = tokenizer.ExpectToken(ZScriptTokenType.CloseParen);
                                if (token == null || !token.IsValid)
                                {
                                    parser.ReportError("Expected ), got " + ((Object)token ?? "<null>").ToString());
                                    return;
                                }

                                // parse the light expression.
                                if (special == "light")
                                {
                                    if (tokens.Count != 1 || (tokens[0].Type != ZScriptTokenType.String && tokens[0].Type != ZScriptTokenType.Identifier))
                                    {
                                        parser.ReportError("Light() special takes one string argument");
                                        return;
                                    }

                                    info.LightName = tokens[0].Value;
                                }
                            }
                        }
                        else
                        {
                            // 
                            stream.Position = cpos2;
                            string actionfunction = parser.ParseDottedIdentifier();
                            //parser.LogWarning("actionfunction = " + actionfunction);
                            if (actionfunction == null)
                                return;
                            // 
                            tokenizer.SkipWhitespace();
                            token = tokenizer.ExpectToken(ZScriptTokenType.OpenParen);
                            if (token != null && token.IsValid)
                            {
                                List<ZScriptToken> tokens = parser.ParseExpression(true);
                                tokenizer.SkipWhitespace();
                                token = tokenizer.ExpectToken(ZScriptTokenType.CloseParen);
                                if (token == null || !token.IsValid)
                                {
                                    parser.ReportError("Expected ), got " + ((Object)token ?? "<null>").ToString());
                                    return;
                                }

                                // possibly do something with the arguments? not now though.
                            }

                            // expect semicolon and break.
                            tokenizer.SkipWhitespace();
                            token = tokenizer.ExpectToken(ZScriptTokenType.Semicolon);
                            if (token == null || !token.IsValid)
                            {
                                parser.ReportError("Expected ;, got " + ((Object)token ?? "<null>").ToString());
                                return;
                            }

                            break;
                        } // if not special
                    } // if identifier
                } // frame parsing loop (inner)
            } // state parsing loop (outer)

            TrimLeft();
        }
    }
}
