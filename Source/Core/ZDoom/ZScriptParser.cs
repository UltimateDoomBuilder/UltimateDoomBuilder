using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace CodeImp.DoomBuilder.ZDoom
{
    public sealed class ZScriptParser : ZDTextParser
    {
        #region ================== Internal classes

        public class ZScriptClassStructure
        {
            // these are used for the class itself
            public string ClassName { get; internal set; }
            public string ReplacementName { get; internal set; }
            public string ParentName { get; internal set; }
            public ZScriptActorStructure Actor { get; internal set; }
            internal DecorateCategoryInfo Region;
			public bool IsMixin { get; internal set; }
			public bool IsExtension { get; internal set; }
			public List<ZScriptClassStructure> Extensions { get; internal set; }

            // these are used for parsing and error reporting
            public ZScriptParser Parser { get; internal set; }
            public Stream Stream { get; internal set; }
            public long Position { get; internal set; }
            public BinaryReader DataReader { get; internal set; }
            public string SourceName { get; internal set; }
            public DataLocation DataLocation { get; internal set; }

            // textresourcepath
            public string TextResourcePath { get; internal set; }

            internal ZScriptClassStructure(ZScriptParser parser, string classname, string replacesname, string parentname, bool ismixin, bool isextension, DecorateCategoryInfo region)
            {
                Parser = parser;

                Stream = parser.datastream;
                Position = parser.datastream.Position;
                DataReader = parser.datareader;
                SourceName = parser.sourcename;
                DataLocation = parser.datalocation;
                TextResourcePath = parser.textresourcepath;

                ClassName = classname;
                ReplacementName = replacesname;
                ParentName = parentname;
                Actor = null;
                Region = region;

				IsMixin = ismixin;
				IsExtension = isextension;
				Extensions = new List<ZScriptClassStructure>();
            }

            internal void RestoreStreamData()
            {
                Parser.datastream = Stream;
                Parser.datastream.Position = Position;
                Parser.datareader = DataReader;
                Parser.sourcename = SourceName;
                Parser.datalocation = DataLocation;
                Parser.prevstreamposition = -1;
            }

            internal bool Process()
            {
                RestoreStreamData();

                bool isactor = false;
                ZScriptClassStructure _pstruct = this;
                while (_pstruct != null)
                {
                    if (_pstruct.ClassName.ToLowerInvariant() == "actor")
                    {
                        isactor = true;
                        break;
                    }

                    if (_pstruct.ParentName != null)
                    {
                        string _pname = _pstruct.ParentName.ToLowerInvariant();

						if(_pname == _pstruct.ClassName.ToLowerInvariant())
						{
							Parser.ReportError("Fatal: Class \"" + _pstruct.ClassName + "\" is trying to inherit from itself.");
							return false;
						}

                        string _cname = _pstruct.ClassName;

                        Parser.allclasses.TryGetValue(_pname, out _pstruct);
                        if (_pstruct == null)
                        {
                            Parser.ReportError("Fatal: Class \"" + _cname + "\" is trying to inherit from \"" + _pname + "\" which does not exist.");
                            return false;
                        }
                    }
                    else _pstruct = null;
                }

                string log_inherits = ((ParentName != null) ? "inherits " + ParentName : "");
                if (ReplacementName != null) log_inherits += ((log_inherits.Length > 0) ? ", " : "") + "replaces " + ReplacementName;
                if (log_inherits.Length > 0) log_inherits = " (" + log_inherits + ")";

                if (isactor || IsMixin || IsExtension)
                {
                    Actor = new ZScriptActorStructure(Parser, Region, ClassName, ReplacementName, ParentName);
                    if (Parser.HasError)
                    {
                        Actor = null;
                        return false;
                    }

					if (!IsExtension)
					{
						// check actor replacement.
						Parser.archivedactors[Actor.ClassName.ToLowerInvariant()] = Actor;
						Parser.realarchivedactors[Actor.ClassName.ToLowerInvariant()] = Actor;
						if (Actor.CheckActorSupported())
							Parser.actors[Actor.ClassName.ToLowerInvariant()] = Actor;

						// Replace an actor?
						if (Actor.ReplacesClass != null)
						{
							if (Parser.GetArchivedActorByName(Actor.ReplacesClass, false) != null)
								Parser.archivedactors[Actor.ReplacesClass.ToLowerInvariant()] = Actor;
							else
								Parser.LogWarning("Unable to find \"" + Actor.ReplacesClass + "\" class to replace, while parsing \"" + Actor.ClassName + "\"");

							if (Actor.CheckActorSupported() && Parser.GetActorByName(Actor.ReplacesClass) != null)
								Parser.actors[Actor.ReplacesClass.ToLowerInvariant()] = Actor;
						}

						//mxd. Add to current text resource
						if (!Parser.scriptresources[TextResourcePath].Entries.Contains(Actor.ClassName)) Parser.scriptresources[TextResourcePath].Entries.Add(Actor.ClassName);
					}
                }

                //Parser.LogWarning(string.Format("Parsed {0}class {1}{2}", isactor?"actor ":"", ClassName, log_inherits));

                return true;
            }
        }

        #endregion

        #region ================== Delegates

        public delegate void IncludeDelegate(ZScriptParser parser, string includefile);

        public IncludeDelegate OnInclude;

        #endregion

        #region ================== Constants

        #endregion

        #region ================== Variables

        //mxd. Script type
        internal override ScriptType ScriptType { get { return ScriptType.ZSCRIPT; } }

        // These are actors we want to keep
        private Dictionary<string, ActorStructure> actors;

        // These are all parsed actors, also those from other games
        private Dictionary<string, ActorStructure> archivedactors;

        // These are archivedactors, but also without replacements, for inheritance purposes
        private Dictionary<string, ActorStructure> realarchivedactors;

        // In ZScript, you can inherit an actor before defining it. So we need to postpone all processing after the classes have been gathered.
        private Dictionary<string, ZScriptClassStructure> allclasses;
        private List<ZScriptClassStructure> allclasseslist;

		// Mixin classes
		private Dictionary<string, ZScriptClassStructure> mixinclasses;
		private List<ZScriptClassStructure> mixinclasseslist;

        //mxd. Includes tracking
        private HashSet<string> parsedlumps;

        //mxd. Disposing. Is that really needed?..
        private bool isdisposed;

        // [ZZ] custom tokenizer class
        internal ZScriptTokenizer tokenizer;

        #endregion

        #region ================== Properties

        /// <summary>
        /// All actors that are supported by the current game.
        /// </summary>
        public IEnumerable<ActorStructure> Actors { get { return actors.Values; } }

        /// <summary>
        /// All actors defined in the loaded DECORATE structures. This includes actors not supported in the current game.
        /// </summary>
        public ICollection<ActorStructure> AllActors { get { return archivedactors.Values; } }

        /// <summary>
        /// mxd. All actors that are supported by the current game.
        /// </summary>
        internal Dictionary<string, ActorStructure> ActorsByClass { get { return actors; } }

        /// <summary>
        /// mxd. All actors defined in the loaded DECORATE structures. This includes actors not supported in the current game.
        /// </summary>
        internal Dictionary<string, ActorStructure> AllActorsByClass { get { return archivedactors; } }


        /// <summary>
        /// This is used to find out what classes were parsed from specific archive
        /// </summary>
        public HashSet<string> LastClasses { get; internal set; }

        #endregion

        #region ================== Constructor / Disposer

        // Constructor
        public ZScriptParser()
        {
            ClearActors();
        }

        // Disposer
        public void Dispose()
        {
            //mxd. Not already disposed?
            if (!isdisposed)
            {
                foreach (KeyValuePair<string, ActorStructure> a in archivedactors)
                    a.Value.Dispose();

                actors = null;
                archivedactors = null;

                isdisposed = true;
            }
        }

        #endregion

        #region ================== Parsing

        private bool ParseInclude(string filename)
        {
            Stream localstream = datastream;
            string localsourcename = sourcename;
            BinaryReader localreader = datareader;
            DataLocation locallocation = datalocation; //mxd
            string localtextresourcepath = textresourcepath; //mxd
            ZScriptTokenizer localtokenizer = tokenizer; // [ZZ]

            //INFO: ZDoom DECORATE include paths can't be relative ("../actor.txt")
            //or absolute ("d:/project/actor.txt")
            //or have backward slashes ("info\actor.txt")
            //include paths are relative to the first parsed entry, not the current one
            //also include paths may or may not be quoted
            //mxd. Sanity checks
            if (string.IsNullOrEmpty(filename))
            {
                ReportError("Expected file name to include");
                return false;
            }

            //mxd. Check invalid path chars
            if (!CheckInvalidPathChars(filename)) return false;

            //mxd. Absolute paths are not supported...
            if (Path.IsPathRooted(filename))
            {
                ReportError("Absolute include paths are not supported by ZDoom");
                return false;
            }

            //mxd. Relative paths are not supported
            if (filename.StartsWith(RELATIVE_PATH_MARKER) || filename.StartsWith(CURRENT_FOLDER_PATH_MARKER) ||
                filename.StartsWith(ALT_RELATIVE_PATH_MARKER) || filename.StartsWith(ALT_CURRENT_FOLDER_PATH_MARKER))
            {
                ReportError("Relative include paths are not supported by ZDoom");
                return false;
            }

            //mxd. Backward slashes are not supported
            if (filename.Contains("\\"))
            {
                ReportError("Only forward slashes are supported by ZDoom");
                return false;
            }

            //mxd. Already parsed?
            if (parsedlumps.Contains(filename))
            {
                ReportError("Already parsed \"" + filename + "\". Check your include directives");
                return false;
            }

            //mxd. Add to collection
            parsedlumps.Add(filename);

            // Callback to parse this file now
            if (OnInclude != null) OnInclude(this, filename);

            //mxd. Bail out on error
            if (this.HasError) return false;

            // Set our buffers back to continue parsing
            datastream = localstream;
            datareader = localreader;
            sourcename = localsourcename;
            datalocation = locallocation; //mxd
            textresourcepath = localtextresourcepath; //mxd
            tokenizer = localtokenizer;

            return true;
        }

        // read in an expression as a token list.
        internal List<ZScriptToken> ParseExpression(bool betweenparen = false)
        {
            List<ZScriptToken> ol = new List<ZScriptToken>();
            //
            int nestingLevel = 0;
            //
            while (true)
            {
                long cpos = datastream.Position;
                ZScriptToken token = tokenizer.ReadToken();
                if (token == null)
                {
                    ReportError("Expected a token");
                    return null;
                }

				// biwa. Report a recoverable parsing problem
				if (!string.IsNullOrEmpty(token.WarningMessage))
					LogWarning(token.WarningMessage);

                if ((token.Type == ZScriptTokenType.Semicolon ||
                     token.Type == ZScriptTokenType.Comma) && nestingLevel == 0 && !betweenparen)
                {
                    datastream.Position = cpos;
                    return ol;
                }

                if (token.Type == ZScriptTokenType.OpenParen)
                {
                    nestingLevel++;
                }
                else if (token.Type == ZScriptTokenType.CloseParen)
                {
                    nestingLevel--;
                    if (nestingLevel < 0) // for example, function call
                    {
                        datastream.Position = cpos;
                        return ol;
                    }
                }

                ol.Add(token);
            }
        }

        internal bool SkipBlock(bool eatSemicolon = true)
        {
            int nestingLevel = 0;
            long cpos = datastream.Position;
            ZScriptToken token = tokenizer.ExpectToken(ZScriptTokenType.OpenCurly);
            if (token == null || !token.IsValid)
            {
                ReportError("Expected {, got " + ((Object)token ?? "<null>").ToString());
                return false;
            }

            // parse everything between { and }
            nestingLevel = 1;
            while (nestingLevel > 0)
            {
                cpos = datastream.Position;
                token = tokenizer.ReadToken(true);
                //LogWarning(token.ToString());
                if (token == null)
                {
                    ReportError("Expected a token");
                    return false;
                }

                if (token.Type != ZScriptTokenType.Invalid)
                    continue;

                if (token.Value == "{")
                {
                    nestingLevel++;
                }
                else if (token.Value == "}")
                {
                    nestingLevel--;
                    if (nestingLevel < 0)
                    {
                        ReportError("Closing parenthesis without an opening one");
                        return false;
                    }
                }
            }

            // there is POTENTIALLY a semicolon after the class definition. it's not supposed to be there, but it's acceptable (GZDoom.pk3 has this)
            cpos = datastream.Position;
            ZScriptToken tailtoken = tokenizer.ReadToken();
            if (tailtoken == null || !eatSemicolon || tailtoken.Type != ZScriptTokenType.Semicolon)
                datastream.Position = cpos;

            return true;
        }

        internal bool SkipClassBlock()
        {
            int nestingLevel = 0;
            long cpos = datastream.Position;
            ZScriptToken token = tokenizer.ExpectToken(ZScriptTokenType.OpenCurly, ZScriptTokenType.Semicolon);
            bool semico = token.Type == ZScriptTokenType.Semicolon;
            if (token == null || !token.IsValid)
            {
                ReportError("Expected { or ;, got " + ((Object)token ?? "<null>").ToString());
                return false;
            }

            // parse everything between { and } or ; and eof
            nestingLevel = 1;
            while (nestingLevel > 0)
            {
                cpos = datastream.Position;
                token = tokenizer.ReadToken(true);
                //LogWarning(token.ToString());
                if (token == null)
                {
                    if (semico)
                    {
                        nestingLevel--;
                        break;
                    }
                    else
                    {
                        ReportError("Expected a token");
                        return false;
                    }
                }

                if (token.Type != ZScriptTokenType.Invalid)
                    continue;

                if (token.Value == "{")
                {
                    nestingLevel++;
                }
                else if (token.Value == "}")
                {
                    nestingLevel--;
                    if (nestingLevel < 0)
                    {
                        ReportError("Closing parenthesis without an opening one");
                        return false;
                    }
                }
            }

            // there is POTENTIALLY a semicolon after the class definition. it's not supposed to be there, but it's acceptable (GZDoom.pk3 has this)
            cpos = datastream.Position;
            ZScriptToken tailtoken = tokenizer.ReadToken();
            if (tailtoken == null || tailtoken.Type != ZScriptTokenType.Semicolon)
                datastream.Position = cpos;

            return true;
        }

        internal List<ZScriptToken> ParseBlock(bool allowsingle)
        {
            List<ZScriptToken> ol = new List<ZScriptToken>();
            //
            int nestingLevel = 0;
            //
            long cpos = datastream.Position;
            ZScriptToken token = tokenizer.ReadToken();
            if (token == null)
            {
                ReportError("Expected a code block, got <null>");
                return null;
            }

            if (token.Type != ZScriptTokenType.OpenCurly)
            {
                if (!allowsingle)
                {
                    ReportError("Expected {, got " + token);
                    return null;
                }

                // otherwise this is an expression
                datastream.Position = cpos;
                List<ZScriptToken> ol_expression = ParseExpression();
                token = tokenizer.ReadToken();
                if (token == null || token.Type != ZScriptTokenType.Semicolon)
                {
                    ReportError("Expected ;, got " + ((Object)token ?? "<null>").ToString());
                    return null;
                }

                ol_expression.Add(token);
                return ol_expression;
            }

            // parse everything between { and }
            nestingLevel = 1;
            while (nestingLevel > 0)
            {
                cpos = datastream.Position;
                token = tokenizer.ReadToken();
                if (token == null)
                {
                    ReportError("Expected a token");
                    return null;
                }

                if (token.Type == ZScriptTokenType.OpenCurly)
                {
                    nestingLevel++;
                }
                else if (token.Type == ZScriptTokenType.CloseCurly)
                {
                    nestingLevel--;
                    if (nestingLevel < 0)
                    {
                        ReportError("Closing parenthesis without an opening one");
                        return null;
                    }
                }

                ol.Add(token);
            }

            // there is POTENTIALLY a semicolon after the class definition. it's not supposed to be there, but it's acceptable (GZDoom.pk3 has this)
            ZScriptToken tailtoken = tokenizer.ReadToken();
            cpos = datastream.Position;
            if (tailtoken == null || tailtoken.Type != ZScriptTokenType.Semicolon)
                datastream.Position = cpos;
            else ol.Add(tailtoken);

            return ol;
        }

        internal bool ParseConst()
        {
            // const blablabla = <expression>;
            tokenizer.SkipWhitespace();
            ZScriptToken token = tokenizer.ExpectToken(ZScriptTokenType.Identifier);
            if (token == null || !token.IsValid)
            {
                ReportError("Expected const name, got " + ((Object)token ?? "<null>").ToString());
                return false;
            }
            string constname = token.Value;
            tokenizer.SkipWhitespace();
            token = tokenizer.ExpectToken(ZScriptTokenType.OpAssign);
            if (token == null || !token.IsValid)
            {
                ReportError("Expected =, got " + ((Object)token ?? "<null>").ToString());
                return false;
            }
            tokenizer.SkipWhitespace();
            if (ParseExpression() == null) return false; // anything until a semicolon or a comma, + anything between parentheses
            tokenizer.SkipWhitespace();
            token = tokenizer.ExpectToken(ZScriptTokenType.Semicolon);
            if (token == null || !token.IsValid)
            {
                ReportError("Expected ;, got " + ((Object)token ?? "<null>").ToString());
                return false;
            }
            //LogWarning(string.Format("Parsed const {0}", constname));
            return true;
        }

        internal bool ParseEnum()
        {
            // enum blablabla {}
            tokenizer.SkipWhitespace();
            ZScriptToken token = tokenizer.ExpectToken(ZScriptTokenType.Identifier);
            if (token == null || !token.IsValid)
            {
                ReportError("Expected enum name, got " + ((Object)token ?? "<null>").ToString());
                return false;
            }
            tokenizer.SkipWhitespace();
            if (ParseBlock(false) == null) return false; // anything between { and }
            //LogWarning(string.Format("Parsed enum {0}", token.Value));
            return true;
        }

        internal bool ParseInteger(out int value)
        {
            value = 1;

            ZScriptToken token = tokenizer.ExpectToken(ZScriptTokenType.Integer, ZScriptTokenType.OpAdd, ZScriptTokenType.OpSubtract);
            if (token == null || !token.IsValid)
            {
                ReportError("Expected integer, got " + ((Object)token ?? "<null>").ToString());
                return false;
            }

            if (token.Type == ZScriptTokenType.OpSubtract) value = -1;

            if (token.Type != ZScriptTokenType.Integer)
            {
                token = tokenizer.ExpectToken(ZScriptTokenType.Integer);
                if (token == null || !token.IsValid)
                {
                    ReportError("Expected integer, got " + ((Object)token ?? "<null>").ToString());
                    return false;
                }

                value *= token.ValueInt;
                return true;
            }
            else
            {
                value = token.ValueInt;
                return true;
            }
        }

        internal string ParseDottedIdentifier()
        {
            string name = "";
            while (true)
            {
                ZScriptToken token = tokenizer.ExpectToken(ZScriptTokenType.Identifier);
                if (token == null || !token.IsValid)
                {
                    ReportError("Expected identifier, got " + ((Object)token ?? "<null>").ToString());
                    return null;
                }

                if (name.Length > 0) name += '.';
                name += token.Value.ToLowerInvariant();

                long cpos = datastream.Position;
                token = tokenizer.ReadToken();
                if (token.Type != ZScriptTokenType.Dot)
                {
                    datastream.Position = cpos;
                    break;
                }
            }

            return name;
        }

        internal bool ParseClassOrStruct(bool isstruct, bool extend, bool mixin, DecorateCategoryInfo region)
        {
            // 'class' keyword is already parsed
            tokenizer.SkipWhitespace();
            ZScriptToken tok_classname = tokenizer.ExpectToken(ZScriptTokenType.Identifier);
            if (tok_classname == null || !tok_classname.IsValid)
            {
                ReportError("Expected class name, got " + ((Object)tok_classname ?? "<null>").ToString());
                return false;
            }

            // name [replaces name] [: name] [native]
            ZScriptToken tok_replacename = null;
            ZScriptToken tok_parentname = null;
            ZScriptToken tok_native = null;
            ZScriptToken tok_scope = null;
            ZScriptToken tok_version = null;
            string[] class_scope_modifiers = new string[] { "clearscope", "ui", "play" };
            string[] other_modifiers = new string[] { "abstract" };
            while (true)
            {
                tokenizer.SkipWhitespace();
                ZScriptToken token = tokenizer.ReadToken();

                if (token == null)
                {
                    ReportError("Expected a token");
                    return false;
                }

                if (token.Type == ZScriptTokenType.Identifier)
                {
                    if (token.Value.ToLowerInvariant() == "replaces")
                    {
                        if (tok_native != null)
                        {
                            ReportError("Cannot have replacement after native");
                            return false;
                        }

                        if (tok_replacename != null)
                        {
                            ReportError("Cannot have two replacements per class");
                            return false;
                        }

                        tokenizer.SkipWhitespace();
                        tok_replacename = tokenizer.ExpectToken(ZScriptTokenType.Identifier);
                        if (tok_replacename == null || !tok_replacename.IsValid)
                        {
                            ReportError("Expected replacement class name, got " + ((Object)tok_replacename ?? "<null>").ToString());
                            return false;
                        }
                    }
                    else if (token.Value.ToLowerInvariant() == "native")
                    {
                        if (tok_native != null)
                        {
                            ReportError("Cannot have two native keywords");
                            return false;
                        }

                        tok_native = token;
                    }
                    else if (Array.IndexOf(class_scope_modifiers, token.Value.ToLowerInvariant()) >= 0)
                    {
                        if (tok_scope != null)
                        {
                            ReportError("Cannot have two scope qualifiers");
                            return false;
                        }

                        tok_scope = token;
                    }
                    else if (!isstruct && Array.IndexOf(other_modifiers, token.Value.ToLowerInvariant()) >= 0)
                    {
                        // don't save these. whatever.
                    }
                    else if (token.Value.ToLowerInvariant() == "version")
                    {
                        if (tok_version != null)
                        {
                            ReportError("Cannot have two versions");
                            return false;
                        }

                        // read in the version.
                        tokenizer.SkipWhitespace();
                        token = tokenizer.ExpectToken(ZScriptTokenType.OpenParen);
                        if (token == null || !token.IsValid)
                        {
                            ReportError("Expected (, got " + ((Object)token ?? "<null>").ToString());
                            return false;
                        }

                        tokenizer.SkipWhitespace();
                        token = tokenizer.ExpectToken(ZScriptTokenType.String);
                        if (token == null || !token.IsValid)
                        {
                            ReportError("Expected version, got " + ((Object)token ?? "<null>").ToString());
                            return false;
                        }

                        tok_version = token;
                        tokenizer.SkipWhitespace();
                        token = tokenizer.ExpectToken(ZScriptTokenType.CloseParen);
                        if (token == null || !token.IsValid)
                        {
                            ReportError("Expected ), got " + ((Object)token ?? "<null>").ToString());
                            return false;
                        }
                    }
                    else
                    {
                        ReportError("Unexpected token " + ((Object)token ?? "<null>").ToString());
                    }
                }
                else if (token.Type == ZScriptTokenType.Colon)
                {
                    if (tok_parentname != null)
                    {
                        ReportError("Cannot have two parent classes");
                        return false;
                    }

                    if (tok_replacename != null || tok_native != null)
                    {
                        ReportError("Cannot have parent class after replacement class or native keyword");
                        return false;
                    }

                    tokenizer.SkipWhitespace();
                    tok_parentname = tokenizer.ExpectToken(ZScriptTokenType.Identifier);
                    if (tok_parentname == null || !tok_parentname.IsValid)
                    {
                        ReportError("Expected replacement class name, got " + ((Object)tok_parentname ?? "<null>").ToString());
                        return false;
                    }
                }
                else if (token.Type == ZScriptTokenType.Semicolon || token.Type == ZScriptTokenType.OpenCurly)
                {
                    datastream.Position--;
                    break;
                }
            }

            // do nothing else atm, except remember the position to put it into the class parsing code
            tokenizer.SkipWhitespace();
            long cpos = datastream.Position;
            //List<ZScriptToken> classblocktokens = ParseBlock(false);
            //if (classblocktokens == null) return false;
            if (!SkipClassBlock()) return false;

            string log_inherits = ((tok_parentname != null) ? "inherits " + tok_parentname.Value : "");
            if (tok_replacename != null) log_inherits += ((log_inherits.Length > 0) ? ", " : "") + "replaces " + tok_replacename.Value;
            if (extend) log_inherits += ((log_inherits.Length > 0) ? ", " : "") + "extends";
            if (log_inherits.Length > 0) log_inherits = " (" + log_inherits + ")";

			// now if we are a class, and we inherit actor, parse this entry as an actor. don't process extensions.
			if (!isstruct && !extend && !mixin)
			{
				ZScriptClassStructure cls = new ZScriptClassStructure(this, tok_classname.Value, (tok_replacename != null) ? tok_replacename.Value : null, (tok_parentname != null) ? tok_parentname.Value : null, false, false, region);
				cls.Position = cpos;
				string clskey = cls.ClassName.ToLowerInvariant();
				if (allclasses.ContainsKey(clskey))
				{
					ReportError("Class " + cls.ClassName + " is double-defined");
					return false;
				}

				allclasses.Add(cls.ClassName.ToLowerInvariant(), cls);
				allclasseslist.Add(cls);
                LastClasses.Add(cls.ClassName.ToLowerInvariant());
            }
			else if(!isstruct && extend)
			{
				string clskey = tok_classname.Value.ToLowerInvariant();

				if(!allclasses.ContainsKey(clskey))
				{
					ReportError("Trying to extend class " + tok_classname.Value + " before it was defined");
					return false;
				}

				// GZDoom doesn't allow extending classes that are from another archive
				if(!allclasses[clskey].DataLocation.Equals(datalocation))
				{
					ReportError("Trying to extend class " + tok_classname.Value + " that's not part of data location \"" + datalocation + "\", but \"" + allclasses[clskey].DataLocation + "\"");
					return false;
				}

				ZScriptClassStructure cls = new ZScriptClassStructure(this, tok_classname.Value, null, null, false, true, region);
				cls.Position = cpos;
				allclasses[clskey].Extensions.Add(cls);
			}
			else if (mixin)
			{
				// This is a bit ugly. We're treating mixin classes as actors, even though they aren't. But otherwise the parser
				// doesn't parse all the actor info we need
				ZScriptClassStructure cls = new ZScriptClassStructure(this, tok_classname.Value, null, null, true, false, region);
				cls.Position = cpos;
				string clskey = cls.ClassName.ToLowerInvariant();
				if(mixinclasses.ContainsKey(clskey))
				{
					ReportError("Mixin class " + cls.ClassName + " is double-defines");
					return false;
				}

				mixinclasses.Add(cls.ClassName.ToLowerInvariant(), cls);
				mixinclasseslist.Add(cls);
			}

            //LogWarning(string.Format("Parsed {0} {1}{2}", (isstruct ? "struct" : "class"), tok_classname.Value, log_inherits));

            return true;
        }

        // This parses the given decorate stream
        // Returns false on errors
        public override bool Parse(TextResourceData data, bool clearerrors)
        {
            if (clearerrors) LastClasses = new HashSet<string>();

            //mxd. Already parsed?
            if (!base.AddTextResource(data))
            {
                if (clearerrors) ClearError();
                return true;
            }

            // Cannot process?
            if (!base.Parse(data, clearerrors)) return false;

			List<string> includes = new List<string>();

            // [ZZ] For whatever reason, the parser is closely tied to the tokenizer, and to the general scripting lumps framework (see scripttype).
            //      For this reason I have to still inherit the old tokenizer while only using the new one.
            //ReportError("found zscript? :)");
            prevstreamposition = -1;
            tokenizer = new ZScriptTokenizer(datareader);

            // region-as-category, ZScript ver
            List<DecorateCategoryInfo> regions = new List<DecorateCategoryInfo>();

            while (true)
            {
                ZScriptToken token = tokenizer.ExpectToken(ZScriptTokenType.Identifier, // const, enum, class, etc
                                                           ZScriptTokenType.Whitespace,
                                                           ZScriptTokenType.Newline,
                                                           ZScriptTokenType.BlockComment, ZScriptTokenType.LineComment,
                                                           ZScriptTokenType.Preprocessor);

				if (token == null) // EOF reached
					break;

                if (!token.IsValid)
                {
                    ReportError("Expected preprocessor statement, const, enum or class declaraction, got " + token);
                    return false;
                }

                // If $GZDB_SKIP is encountered we stop parsing. Not that we can't "return" here yet, because the includes still have to be parsed
                if (token.Type == ZScriptTokenType.LineComment && token.Value.Trim().ToLowerInvariant() == "$gzdb_skip")
                    break;

                // toplevel tokens allowed are only Preprocessor and Identifier.
                switch (token.Type)
                {
                    case ZScriptTokenType.Whitespace:
                    case ZScriptTokenType.Newline:
                    case ZScriptTokenType.BlockComment:
                        break;

                    case ZScriptTokenType.LineComment:
                        {
                            string cmtval = token.Value.TrimStart();
                            if (cmtval.Length <= 0 || cmtval[0] != '$')
                                break;
                            // if we are in a region, read property using function from ZScriptActorStructure
                            if (regions.Count > 0)
                                ZScriptActorStructure.ParseGZDBComment(regions.Last().Properties, cmtval);
                        }
                        break;

                    case ZScriptTokenType.Preprocessor:
                        {
                            tokenizer.SkipWhitespace();
                            ZScriptToken directive = tokenizer.ExpectToken(ZScriptTokenType.Identifier);
                            if (directive == null || !directive.IsValid)
                            {
                                ReportError("Expected preprocessor directive, got " + ((Object)directive ?? "<null>").ToString());
                                return false;
                            }

                            string d_value = directive.Value.ToLowerInvariant();
                            if (d_value == "include")
                            {
                                tokenizer.SkipWhitespace();
                                ZScriptToken include_name = tokenizer.ExpectToken(ZScriptTokenType.Identifier, ZScriptTokenType.String, ZScriptTokenType.Name);
                                if (include_name == null || !include_name.IsValid)
                                {
                                    ReportError("Cannot include: expected a string value, got " + ((Object)include_name ?? "<null>").ToString());
                                    return false;
                                }

								// GZDoom parses includes *after* the rest of the file is parsed, so just store the files to include and parse them later
								includes.Add(include_name.Value);
                            }
                            else if (d_value == "region")
                            {
                                // just read everything until newline.
                                string region_name = "";
                                while (true)
                                {
                                    token = tokenizer.ReadToken();
                                    if (token == null || token.Type == ZScriptTokenType.Newline)
                                        break;
                                    region_name += token.Value;
                                }
                                DecorateCategoryInfo region = new DecorateCategoryInfo();
                                string[] cattitle = region_name.Split(DataManager.CATEGORY_SPLITTER, StringSplitOptions.RemoveEmptyEntries);
                                if (regions.Count > 0)
                                {
                                    region.Category.AddRange(regions.Last().Category);
                                    region.Properties = new Dictionary<string, List<string>>(regions.Last().Properties, StringComparer.OrdinalIgnoreCase);
                                }
                                region.Category.AddRange(cattitle);
                                regions.Add(region);
                            }
                            else if (d_value == "endregion")
                            {
                                // read everything until newline too?
                                // - only if it causes problems
                                if (regions.Count > 0)
                                    regions.RemoveAt(regions.Count - 1); // remove last region from the list
                                else
                                {
                                    LogWarning("Superfluous #endregion found without corresponding #region");
                                }
                            }
                            else
                            {
                                ReportError("Unknown preprocessor directive: " + directive.Value);
                                return false;
                            }
                            break;
                        }

                    case ZScriptTokenType.Identifier:
                        {
                            // identifier can be one of: class, enum, const, struct
                            // the only type that we really care about is class, as it's the one that has all actors.
                            switch (token.Value.ToLowerInvariant())
                            {
                                case "extend":
                                    tokenizer.SkipWhitespace();
                                    token = tokenizer.ExpectToken(ZScriptTokenType.Identifier);
                                    if (token == null || !token.IsValid || ((token.Value.ToLowerInvariant() != "class") && (token.Value.ToLowerInvariant() != "struct")))
                                    {
                                        ReportError("Expected class or struct, got " + ((Object)token ?? "<null>").ToString());
                                        return false;
                                    }
                                    if (!ParseClassOrStruct((token.Value.ToLowerInvariant() == "struct"), true, false, (regions.Count > 0 ? regions.Last() : null)))
                                        return false;
                                    break;
								case "mixin":
									tokenizer.SkipWhitespace();
									token = tokenizer.ExpectToken(ZScriptTokenType.Identifier);
									if(token == null || !token.IsValid ||((token.Value.ToLower() != "class")))
									{
										ReportError("Expected class, got " + ((Object)token ?? "<null>").ToString());
										return false;
									}
									if (!ParseClassOrStruct(false, false, true, (regions.Count > 0 ? regions.Last() : null)))
										return false;
									break;
								case "class":
                                    // todo parse class
                                    if (!ParseClassOrStruct(false, false, false, (regions.Count > 0 ? regions.Last() : null)))
                                        return false;
                                    break;
                                case "struct":
                                    // todo parse struct
                                    if (!ParseClassOrStruct(true, false, false, null))
                                        return false;
                                    break;
                                case "const":
                                    if (!ParseConst())
                                        return false;
                                    break;
                                case "enum":
                                    if (!ParseEnum())
                                        return false;
                                    break;
                                case "version":
                                    // expect a string. do nothing about it.
                                    tokenizer.SkipWhitespace();
                                    token = tokenizer.ExpectToken(ZScriptTokenType.String);
                                    if (token == null || !token.IsValid)
                                    {
                                        ReportError("Expected version string, got " + ((Object)token ?? "<null>").ToString());
                                        return false;
                                    }
                                    break;
                                default:
                                    ReportError("Expected preprocessor statement, const, enum or class declaraction, got " + token);
                                    return false;
                            }
                            break;
                        }
                }
            }

            // Now parse all included files
            foreach (string include in includes)
            {
                if (!ParseInclude(include))
                    return false;
            }

            return true;
        }

        public bool Finalize()
        {
            ClearError();

            // parse class data
            foreach (ZScriptClassStructure cls in allclasseslist)
            {
                if (!cls.Process())
                    return false;

				// Process extensions
				foreach (ZScriptClassStructure extension in cls.Extensions)
					if (!extension.Process())
						return false;
            }

			// Parse mixin class data
			foreach(ZScriptClassStructure cls in mixinclasseslist)
			{
				if (!cls.Process())
					return false;
			}

            // set datastream to null so that log messages aren't output using incorrect line numbers
            Stream odatastream = datastream;
            datastream = null;

            // inject superclasses, and mixins, since everything is parsed by now
            Dictionary<int, ThingTypeInfo> things = General.Map.Config.GetThingTypes();
            foreach (ZScriptClassStructure cls in allclasseslist)
            {
                ActorStructure actor = cls.Actor;
				if (actor != null)
				{
					// Inheritance
					if (cls.ParentName != null && cls.ParentName.ToLowerInvariant() != "thinker") // don't try to inherit this one)
					{
						actor.baseclass = GetArchivedActorByName(cls.ParentName, true);
						string inheritclass = cls.ParentName;

						//check if this class inherits from a class defined in game configuration
						string inheritclasscheck = inheritclass.ToLowerInvariant();

						// inherit args from base class
						if (actor.baseclass != null)
						{
							for (int i = 0; i < 5; i++)
							{
								if (actor.args[i] == null)
									actor.args[i] = actor.baseclass.args[i];
							}
						}

						bool thingfound = false;
						foreach (KeyValuePair<int, ThingTypeInfo> ti in things)
						{
							if (!string.IsNullOrEmpty(ti.Value.ClassName) && ti.Value.ClassName.ToLowerInvariant() == inheritclasscheck)
							{
								//states
								// [ZZ] allow internal prefix here. it can inherit MapSpot, light, or other internal stuff.
								if (actor.states.Count == 0 && !string.IsNullOrEmpty(ti.Value.Sprite))
									actor.states.Add("spawn", new StateStructure(ti.Value.Sprite.StartsWith(DataManager.INTERNAL_PREFIX) ? ti.Value.Sprite : ti.Value.Sprite.Substring(0, 5)));

								if (actor.baseclass == null)
								{
									//flags
									if (ti.Value.Hangs && !actor.flags.ContainsKey("spawnceiling"))
										actor.flags["spawnceiling"] = true;

									if (ti.Value.Blocking > 0 && !actor.flags.ContainsKey("solid"))
										actor.flags["solid"] = true;

									//properties
									if (!actor.props.ContainsKey("height"))
										actor.props["height"] = new List<string> { ti.Value.Height.ToString() };

									if (!actor.props.ContainsKey("radius"))
										actor.props["radius"] = new List<string> { ti.Value.Radius.ToString() };
								}

								// [ZZ] inherit arguments from game configuration
								//
								if (!actor.props.ContainsKey("$clearargs"))
								{
									for (int i = 0; i < 5; i++)
									{
										if (actor.args[i] != null)
											continue; // don't touch it if we already have overrides

										ArgumentInfo arg = ti.Value.Args[i];
										if (arg != null && arg.Used)
											actor.args[i] = arg;
									}
								}

								thingfound = true;
								break;
							}

							if (actor.baseclass == null && !thingfound)
								LogWarning("Unable to find \"" + inheritclass + "\" class to inherit from, while parsing \"" + cls.ClassName + "\"");
						}
					}

					// Mixins. https://zdoom.org/wiki/ZScript_mixins
					if (((ZScriptActorStructure)actor).Mixins.Count > 0)
					{
						foreach(string mixinclassname in ((ZScriptActorStructure)actor).Mixins)
						{
							if(!mixinclasses.ContainsKey(mixinclassname))
							{
								LogWarning("Unable to find \"" + mixinclassname + "\" mixin class while parsing \"" + actor.ClassName + "\"");
								continue;
							}

							ZScriptClassStructure mixincls = mixinclasses[mixinclassname];

							// States
							if(actor.states.Count == 0 && mixincls.Actor.states.Count != 0)
							{
								// Can't use HasState and GetState here, because it does some magic that will not work for mixins
								if (!actor.states.ContainsKey("spawn") && mixincls.Actor.states.ContainsKey("spawn"))
									actor.states.Add("spawn", mixincls.Actor.GetState("spawn"));
							}

							// Properties
							if (!actor.props.ContainsKey("height") && mixincls.Actor.props.ContainsKey("height"))
								actor.props["height"] = new List<string>(mixincls.Actor.props["height"]);

							if (!actor.props.ContainsKey("radius") && mixincls.Actor.props.ContainsKey("radius"))
								actor.props["radius"] = new List<string>(mixincls.Actor.props["radius"]);

							// Flags
							if (!actor.flags.ContainsKey("spawnceiling") && mixincls.Actor.flags.ContainsKey("spawnceiling"))
								actor.flags["spawnceiling"] = true;

							if (!actor.flags.ContainsKey("solid") && mixincls.Actor.flags.ContainsKey("solid"))
								actor.flags["solid"] = true;

							// user_ variables
							foreach (string uservarname in mixincls.Actor.uservars.Keys)
								actor.uservars[uservarname] = mixincls.Actor.uservars[uservarname];
						}
					}

					// Extensions. https://zdoom.org/wiki/ZScript_classes#Extending_Classes
					if (cls.Extensions.Count > 0)
					{
						foreach(ZScriptClassStructure extension in cls.Extensions)
						{
							ActorStructure extenseionactor = extension.Actor;

							if (extenseionactor == null)
								continue;

							// States
							if (extenseionactor.states.ContainsKey("spawn"))
								actor.states["spawn"] = extenseionactor.GetState("spawn");

							// Properties
							if (extenseionactor.props.ContainsKey("height"))
								actor.props["height"] = new List<string>(extenseionactor.props["height"]);

							if (extenseionactor.props.ContainsKey("radius"))
								actor.props["radius"] = new List<string>(extenseionactor.props["radius"]);

							// Flags
							if (extenseionactor.flags.ContainsKey("spawnceiling"))
								actor.flags["spawnceiling"] = true;

							if (extenseionactor.flags.ContainsKey("solid"))
								actor.flags["solid"] = true;

							// user_ variables
							foreach (string uservarname in extenseionactor.uservars.Keys)
							{
								actor.uservars[uservarname] = extenseionactor.uservars[uservarname];

								if (extenseionactor.uservar_defaults.ContainsKey(uservarname))
									actor.uservar_defaults[uservarname] = extenseionactor.uservar_defaults[uservarname];
							}
						}
					}
				}
            }

            // validate user variables (no variables should shadow parent variables)
            foreach (ZScriptClassStructure cls in allclasseslist)
            {
                ActorStructure actor = cls.Actor;
                if (actor == null)
                    continue;
                ActorStructure actorbase = actor.baseclass;
                while (actorbase != null)
                {
                    foreach (string uservar in actor.uservars.Keys)
                    {
                        if (actorbase.uservars.ContainsKey(uservar))
                        {
                            actor.uservars.Clear();
                            ReportError("Variable \"" + uservar + "\" in class \"" + actor.classname + "\" shadows variable \"" + uservar + "\" in base class \"" + actorbase.classname + "\". This is not supported");
                            goto stopValidatingCompletely;
                        }
                    }

                    actorbase = actorbase.baseclass;
                }
            }
        stopValidatingCompletely:
            datastream = odatastream;
            return true;
        }

        #endregion

        #region ================== Methods

        protected override int GetCurrentLineNumber()
        {
            prevstreamposition = (tokenizer != null) ? tokenizer.LastPosition : -1;
            return base.GetCurrentLineNumber();
        }

        /// <summary>
        /// This returns a supported actor by name. Returns null when no supported actor with the specified name can be found. This operation is of O(1) complexity.
        /// </summary>
        public ActorStructure GetActorByName(string name)
        {
            name = name.ToLowerInvariant();
            return actors.ContainsKey(name) ? actors[name] : null;
        }

        /// <summary>
        /// This returns a supported actor by DoomEdNum. Returns null when no supported actor with the specified name can be found. Please note that this operation is of O(n) complexity!
        /// </summary>
        public ActorStructure GetActorByDoomEdNum(int doomednum)
        {
            foreach (ActorStructure a in actors.Values)
                if (a.DoomEdNum == doomednum) return a;
            return null;
        }

        // This returns an actor by name
        // Returns null when actor cannot be found
        internal ActorStructure GetArchivedActorByName(string name, bool unique)
        {
            Dictionary<string, ActorStructure> dict = (unique) ? realarchivedactors : archivedactors;
            name = name.ToLowerInvariant();
            return (dict.ContainsKey(name) ? dict[name] : null);
        }

        internal void ClearActors()
        {
            // Initialize
            actors = new Dictionary<string, ActorStructure>(StringComparer.OrdinalIgnoreCase);
            archivedactors = new Dictionary<string, ActorStructure>(StringComparer.OrdinalIgnoreCase);
            realarchivedactors = new Dictionary<string, ActorStructure>(StringComparer.OrdinalIgnoreCase);
            parsedlumps = new HashSet<string>(StringComparer.OrdinalIgnoreCase); //mxd
            allclasses = new Dictionary<string, ZScriptClassStructure>();
            allclasseslist = new List<ZScriptClassStructure>();
			mixinclasses = new Dictionary<string, ZScriptClassStructure>();
			mixinclasseslist = new List<ZScriptClassStructure>();
        }

        #endregion

    }
}
