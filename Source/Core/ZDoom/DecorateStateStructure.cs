using System.IO;

namespace CodeImp.DoomBuilder.ZDoom
{

    public sealed class DecorateStateStructure : StateStructure
    {
        #region ================== DECORATE State Structure parsing

        internal DecorateStateStructure(ActorStructure actor, ZDTextParser zdparser)
        {
            DecorateParser parser = (DecorateParser)zdparser;
            string lasttoken = "";

            // Skip whitespace
            while (parser.SkipWhitespace(true))
            {
                // Read first token
                string token = parser.ReadToken().ToLowerInvariant();

                // One of the flow control statements?
                if ((token == "loop") || (token == "stop") || (token == "wait") || (token == "fail"))
                {
                    // Ignore flow control
                    // [ZZ] sometimes "fail" is a sprite name... (Skulltag, Zandronum)
                    //      probably the same can happen to other single-word flow control keywords.
                        
                    // check if next token is newline.
                    long cpos = parser.DataStream.Position;
                    parser.SkipWhitespace(false);
                    string newline = parser.ReadToken();
                    parser.DataStream.Position = cpos;
                    if (newline == "\n") // this is actually a loop/stop/wait/fail directive and not a sprite name or something
                    {
                        lasttoken = token;
                        continue;
                    }
                }

                // Goto?
                if (token == "goto")
                {
                    gotostate = new DecorateStateGoto(actor, parser);
                    if (parser.HasError) return;
                }
                // Label?
                else if (token == ":")
                {
                    // Rewind so that this label can be read again
                    if (!string.IsNullOrEmpty(lasttoken))
                        parser.DataStream.Seek(-(lasttoken.Length + 1), SeekOrigin.Current);

                    // Done here
                    goto endofallthings;
                }
                //mxd. Start of inner scope?
                else if (token == "{")
                {
                    int bracelevel = 1;
                    while (!string.IsNullOrEmpty(token) && bracelevel > 0)
                    {
                        parser.SkipWhitespace(false);
                        token = parser.ReadToken();
                        switch (token)
                        {
                            case "{": bracelevel++; break;
                            case "}": bracelevel--; break;
                        }
                    }
                }
                // End of scope?
                else if (token == "}")
                {
                    // Rewind so that this scope end can be read again
                    parser.DataStream.Seek(-1, SeekOrigin.Current);

                    // Done here
                    goto endofallthings;
                }
                else
                {
                    // First part of the sprite name
                    token = parser.StripTokenQuotes(token); //mxd. First part of the sprite name can be quoted
                    if (string.IsNullOrEmpty(token))
                    {
                        parser.ReportError("Expected sprite name");
                        return;
                    }

                    // Frames of the sprite name
                    parser.SkipWhitespace(true);
                    string spriteframes = parser.StripTokenQuotes(parser.ReadToken()); //mxd. Frames can be quoted
                    if (string.IsNullOrEmpty(spriteframes))
                    {
                        parser.ReportError("Expected sprite frame");
                        return;
                    }

                    // Label?
                    if (spriteframes == ":")
                    {
                        // Rewind so that this label can be read again
                        parser.DataStream.Seek(-(token.Length + 1), SeekOrigin.Current);

                        // Done here
                        goto endofallthings;
                    }

                    // No first sprite yet?
                    FrameInfo info = new FrameInfo(); //mxd
                    if (spriteframes.Length > 0)
                    {
                        //mxd. I'm not even 50% sure the parser handles all bizzare cases without shifting sprite name / frame blocks,
                        // so let's log it as a warning, not an error...
                        if (token.Length != 4)
                        {
                            parser.LogWarning("Invalid sprite name \"" + token.ToUpperInvariant() + "\". Sprite names must be exactly 4 characters long");
                        }
                        else
                        {
                            // Make the sprite name
                            string spritename = (token + spriteframes[0]).ToUpperInvariant();

                            // Ignore some odd ZDoom things
                            if (/*!realspritename.StartsWith("TNT1") && */!spritename.StartsWith("----") && !spritename.Contains("#")) // [ZZ] some actors have only TNT1 state and receive a random image because of this
                            {
                                info.Sprite = spritename; //mxd
                                int duration = -1;
                                parser.SkipWhitespace(false);
                                string durationstr = parser.ReadToken();
                                if (durationstr == "-")
                                    durationstr += parser.ReadToken();
                                if (string.IsNullOrEmpty(durationstr) || durationstr == "\n")
                                {
                                    parser.ReportError("Expected frame duration");
                                    return;
                                }
                                if (!int.TryParse(durationstr.Trim(), out duration))
                                    parser.DataStream.Seek(-(durationstr.Length), SeekOrigin.Current);
                                info.Duration = duration;
                                sprites.Add(info);
                            }
                        }
                    }

                    // Continue until the end of the line
                    parser.SkipWhitespace(false);
                    string t = parser.ReadToken();
                    while (!string.IsNullOrEmpty(t) && t != "\n")
                    {
                        //mxd. Bright keyword support...
                        if (t == "bright")
                        {
                            info.Bright = true;
                        }
                        //mxd. Light() expression support...
                        else if (t == "light")
                        {
                            if (!parser.NextTokenIs("(")) return;

                            if (!parser.SkipWhitespace(true))
                            {
                                parser.ReportError("Unexpected end of the structure");
                                return;
                            }

                            info.LightName = parser.StripTokenQuotes(parser.ReadToken());
                            if (string.IsNullOrEmpty(info.LightName))
                            {
                                parser.ReportError("Expected dynamic light name");
                                return;
                            }

                            if (!parser.SkipWhitespace(true))
                            {
                                parser.ReportError("Unexpected end of the structure");
                                return;
                            }

                            if (!parser.NextTokenIs(")"))
                            {
                                parser.ReportError("Expected closing parenthesis in Light()");
                                return;
                            }
                        }
                        //mxd. Inner scope start. Step back and reparse using parent loop
                        else if (t == "{")
                        {
                            // Rewind so that this scope end can be read again
                            parser.DataStream.Seek(-1, SeekOrigin.Current);

                            // Break out of this loop
                            break;
                        }
                        //mxd. Function params start (those can span multiple lines)
                        else if (t == "(")
                        {
                            int bracelevel = 1;
                            while (!string.IsNullOrEmpty(token) && bracelevel > 0)
                            {
                                parser.SkipWhitespace(true);
                                token = parser.ReadToken();
                                switch (token)
                                {
                                    case "(": bracelevel++; break;
                                    case ")": bracelevel--; break;
                                }
                            }
                        }
                        //mxd. Because stuff like this is also valid: "Actor Oneliner { States { Spawn: WOOT A 1 A_FadeOut(0.1) Loop }}"
                        else if (t == "}")
                        {
                            // Rewind so that this scope end can be read again
                            parser.DataStream.Seek(-1, SeekOrigin.Current);

                            // Done here
                            goto endofallthings;
                        }

                        // Read next token
                        parser.SkipWhitespace(false);
                        t = parser.ReadToken().ToLowerInvariant();
                    }
                }

                lasttoken = token;
            }

            // return
        endofallthings:

            TrimLeft();
        }

        #endregion
    }
}
