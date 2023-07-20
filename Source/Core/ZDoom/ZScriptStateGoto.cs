using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CodeImp.DoomBuilder.ZDoom
{
    internal sealed class ZScriptStateGoto : StateGoto
    {
        internal ZScriptStateGoto(ActorStructure actor, ZDTextParser zdparser)
        {
            // goto syntax that is accepted by GZDB is [classname::]statename[+offset]

            ZScriptParser parser = (ZScriptParser)zdparser;
            Stream stream = parser.DataStream;
            ZScriptTokenizer tokenizer = new ZScriptTokenizer(parser.DataReader);
            parser.tokenizer = tokenizer;

            tokenizer.SkipWhitespace();
            string firsttarget = parser.ParseDottedIdentifier();
            if (firsttarget == null)
                return;

            ZScriptToken token;

            string secondtarget = null;
            int offset = 0;

            tokenizer.SkipWhitespace();
            token = tokenizer.ExpectToken(ZScriptTokenType.DoubleColon);
            if (token != null && token.IsValid)
            {
                secondtarget = parser.ParseDottedIdentifier();
                if (secondtarget == null)
                    return;
            }

            // TODO: Zscript allows complex mathematical opeartions, not use addition.
            // example: Goto Spawn + 5 * 2 - 7;
            // Values of identifies are not used right now and always use 0
            tokenizer.SkipWhitespace();
            token = tokenizer.ExpectToken(ZScriptTokenType.OpAdd);
            if (token != null && token.IsValid)
            {
                tokenizer.SkipWhitespace();
                token = tokenizer.ExpectToken(ZScriptTokenType.Integer, ZScriptTokenType.Identifier);
                if (token == null || !token.IsValid)
                {
                    parser.ReportError("Expected state offset, got " + ((Object)token ?? "<null>").ToString());
                    return;
                }

                offset = token.ValueInt;
            }

            // Check if we don't have the class specified
            if (string.IsNullOrEmpty(secondtarget))
            {
                // First target is the state to go to
                classname = actor.ClassName;
                statename = firsttarget.ToLowerInvariant().Trim();
            }
            else
            {
                // First target is the base class to use
                // Second target is the state to go to
                classname = firsttarget.ToLowerInvariant().Trim();
                statename = secondtarget.ToLowerInvariant().Trim();
            }

            spriteoffset = offset;

            if ((classname == "super") && (actor.BaseClass != null))
                classname = actor.BaseClass.ClassName;
        }
    }
}
