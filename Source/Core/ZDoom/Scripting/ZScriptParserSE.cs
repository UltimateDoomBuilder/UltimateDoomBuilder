using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.GZBuilder.Data;
using System.Collections.Generic;

namespace CodeImp.DoomBuilder.ZDoom.Scripting
{
    // [ZZ] this is literally copypasted DecorateParserSE.
    //      possibly extend it later.
    internal sealed class ZScriptParserSE : ZDTextParser
    {
        internal override ScriptType ScriptType { get { return ScriptType.ZSCRIPT; } }

        private readonly List<ScriptItem> types;
        public List<ScriptItem> Types { get { return types; } }

        public ZScriptParserSE()
        {
            types = new List<ScriptItem>();
        }

        private bool CheckClassDefinition(string token)
        {
            return token == "CLASS" || token == "STRUCT" || token == "ENUM";
        }

        public override bool Parse(TextResourceData data, bool clearerrors)
        {
            //mxd. Already parsed?
            if (!base.AddTextResource(data))
            {
                if (clearerrors) ClearError();
                return true;
            }

            // Cannot process?
            if (!base.Parse(data, clearerrors)) return false;

            // Continue until at the end of the stream
            while (SkipWhitespace(true))
            {
                int startpos = (int)datastream.Position;
                string token = ReadToken();
                if (string.IsNullOrEmpty(token) || !CheckClassDefinition(token.ToUpperInvariant())) continue;

                SkipWhitespace(true);
                List<string> definition = new List<string>();
                definition.Add(token);

                do
                {
                    token = ReadToken(false); // Don't skip newline
                    if (string.IsNullOrEmpty(token) || token == "{" || token == "}") break;
                    definition.Add(token);
                } while (SkipWhitespace(false)); // Don't skip newline

                string name = string.Join(" ", definition.ToArray());
                if (!string.IsNullOrEmpty(name)) types.Add(new ScriptItem(name, startpos, false));
            }

            // Sort nodes
            types.Sort(ScriptItem.SortByName);
            return true;
        }
    }
}
