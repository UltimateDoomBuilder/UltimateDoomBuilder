
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
using System.IO;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Data;

#endregion

namespace CodeImp.DoomBuilder.ZDoom
{
	public sealed class DecorateParser : ZDTextParser
	{
		#region ================== Delegates

		public delegate void IncludeDelegate(DecorateParser parser, string includefile);
		
		public IncludeDelegate OnInclude;

		#endregion
		
		#region ================== Constants
		
		#endregion
		
		#region ================== Variables

		//mxd. Script type
		internal override ScriptType ScriptType { get { return ScriptType.DECORATE; } }

		// These are actors we want to keep
		private Dictionary<string, ActorStructure> actors;
		
		// These are all parsed actors, also those from other games
		private Dictionary<string, ActorStructure> archivedactors;

        // These are actors from ZScript. Don't even try to expose this list.
        private Dictionary<string, ActorStructure> zscriptactors;

        //mxd. Includes tracking
        private HashSet<string> parsedlumps;

		//mxd. Custom damagetypes
		private HashSet<string> damagetypes;

		//mxd. Disposing. Is that really needed?..
		private bool isdisposed;

		//
		public bool NoWarnings = false;

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
		/// mxd. Custom DamageTypes (http://zdoom.org/wiki/Damage_types).
		/// </summary>
		public IEnumerable<string> DamageTypes { get { return damagetypes; } }

		/// <summary>
		/// This is used to find out what classes were parsed from specific archive
		/// </summary>
		public HashSet<string> LastClasses { get; internal set; }
		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public DecorateParser(Dictionary<string, ActorStructure> _zscriptactors)
		{
			// Syntax
			whitespace = "\n \t\r\u00A0"; //mxd. non-breaking space is also space :)
			specialtokens = ":{}[]()+-\n;,";
			skipregions = false; //mxd

            ClearActors();
            zscriptactors = _zscriptactors;
		}
		
		// Disposer
		public void Dispose()
		{
			//mxd. Not already disposed?
			if(!isdisposed)
			{
				foreach(KeyValuePair<string, ActorStructure> a in archivedactors)
					a.Value.Dispose();

				actors = null;
				archivedactors = null;

				isdisposed = true;
			}
		}

		#endregion

		#region ================== Parsing

		protected internal override void LogWarning(string message, int linenumber)
		{
			if (NoWarnings)
				return;
			base.LogWarning(message, linenumber);
		}

		// This parses the given decorate stream
		// Returns false on errors
		public override bool Parse(TextResourceData data, bool clearerrors)
		{
			if (clearerrors) LastClasses = new HashSet<string>();

			//mxd. Already parsed?
			if(!base.AddTextResource(data))
			{
				if(clearerrors) ClearError();
				return true;
			}

			// Cannot process?
			if(!base.Parse(data, clearerrors)) return false;

			//mxd. Region-as-category stuff...
			List<DecorateCategoryInfo> regions = new List<DecorateCategoryInfo>();
			
			//mxd. Regions tracking...
			List<KeyValuePair<int, string>> regionlines = new List<KeyValuePair<int, string>>(); // <line number, region title>
			
			// Keep local data
			Stream localstream = datastream;
			string localsourcename = sourcename;
			BinaryReader localreader = datareader;
			DataLocation locallocation = datalocation; //mxd
			string localtextresourcepath = textresourcepath; //mxd
			
			// Continue until at the end of the stream
			while(SkipWhitespace(true))
			{
				// Read a token
				string objdeclaration = ReadToken();
				if(!string.IsNullOrEmpty(objdeclaration))
				{
					objdeclaration = objdeclaration.ToLowerInvariant();
					if(objdeclaration == "$gzdb_skip") break;
					switch(objdeclaration)
					{
						case "actor":
						{
							// Read actor structure
							ActorStructure actor = new DecorateActorStructure(this, (regions.Count > 0 ? regions[regions.Count - 1] : null));
							if(this.HasError) return false;

							LastClasses.Add(actor.ClassName.ToLowerInvariant());
						
							// Add the actor
							archivedactors[actor.ClassName.ToLowerInvariant()] = actor;
							if(actor.CheckActorSupported())
								actors[actor.ClassName.ToLowerInvariant()] = actor;
						
							// Replace an actor?
							if(actor.ReplacesClass != null)
							{
								if(GetArchivedActorByName(actor.ReplacesClass) != null)
									archivedactors[actor.ReplacesClass.ToLowerInvariant()] = actor;
								else
									LogWarning("Unable to find \"" + actor.ReplacesClass + "\" class to replace, while parsing \"" + actor.ClassName + "\"");
							
								if(actor.CheckActorSupported() && GetActorByName(actor.ReplacesClass) != null)
									actors[actor.ReplacesClass.ToLowerInvariant()] = actor;
							}

							//mxd. Add to current text resource
							if(!scriptresources[textresourcepath].Entries.Contains(actor.ClassName)) scriptresources[textresourcepath].Entries.Add(actor.ClassName);
						}
						break;

						case "#include":
						{
							//INFO: ZDoom DECORATE include paths can't be relative ("../actor.txt") 
							//or absolute ("d:/project/actor.txt") 
							//or have backward slashes ("info\actor.txt")
							//include paths are relative to the first parsed entry, not the current one 
							//also include paths may or may not be quoted
							SkipWhitespace(true);
							string filename = StripQuotes(ReadToken(false)); //mxd. Don't skip newline

							//mxd. Sanity checks
							if(string.IsNullOrEmpty(filename))
							{
								ReportError("Expected file name to include");
								return false;
							}

							//mxd. Check invalid path chars
							if(!CheckInvalidPathChars(filename)) return false;

							//mxd. Absolute paths are not supported...
							if(Path.IsPathRooted(filename))
							{
								ReportError("Absolute include paths are not supported by ZDoom");
								return false;
							}

							//mxd. Relative paths are not supported
							if(filename.StartsWith(RELATIVE_PATH_MARKER) || filename.StartsWith(CURRENT_FOLDER_PATH_MARKER) ||
							   filename.StartsWith(ALT_RELATIVE_PATH_MARKER) || filename.StartsWith(ALT_CURRENT_FOLDER_PATH_MARKER))
							{
								ReportError("Relative include paths are not supported by ZDoom");
								return false;
							}

							//mxd. Backward slashes are not supported
							if(filename.Contains("\\"))
							{
								ReportError("Only forward slashes are supported by ZDoom");
								return false;
							}

							//mxd. Already parsed?
							if(parsedlumps.Contains(filename))
							{
								ReportError("Already parsed \"" + filename + "\". Check your include directives");
								return false;
							}

							//mxd. Add to collection
							parsedlumps.Add(filename);

							// Callback to parse this file now
							if(OnInclude != null) OnInclude(this, filename);

							//mxd. Bail out on error
							if(this.HasError) return false;

							// Set our buffers back to continue parsing
							datastream = localstream;
							datareader = localreader;
							sourcename = localsourcename;
							datalocation = locallocation; //mxd
							textresourcepath = localtextresourcepath; //mxd
						}
						break;

						case "damagetype": //mxd
							// Get DamageType name
							SkipWhitespace(true);
							string damagetype = StripQuotes(ReadToken(false));
							if(string.IsNullOrEmpty(damagetype))
							{
								ReportError("Expected DamageType name");
								return false;
							}

							// Next should be "{"
							SkipWhitespace(true);
							if(!NextTokenIs("{")) return false;

							// Skip the structure
							while(SkipWhitespace(true))
							{
								string t = ReadToken();
								if(string.IsNullOrEmpty(t) || t == "}") break;
							}

							// Add to collection
							if(!damagetypes.Contains(damagetype)) damagetypes.Add(damagetype);
							break;

						case "enum":
						case "native":
						case "const":
							while(SkipWhitespace(true))
							{
								string t = ReadToken();
								if(string.IsNullOrEmpty(t) || t == ";") break;
							}
							break;

						//mxd. Region-as-category handling
						case "#region":
							int line = GetCurrentLineNumber();
							SkipWhitespace(false);
							string cattitle = ReadLine();
							regionlines.Add(new KeyValuePair<int, string>(line, cattitle));
							if(!string.IsNullOrEmpty(cattitle))
							{
								// Make new category info
								string[] parts = cattitle.Split(DataManager.CATEGORY_SPLITTER, StringSplitOptions.RemoveEmptyEntries);

								DecorateCategoryInfo info = new DecorateCategoryInfo();
								if(regions.Count > 0)
								{
									// Preserve nesting
									info.Category.AddRange(regions[regions.Count - 1].Category);
									info.Properties = new Dictionary<string, List<string>>(regions[regions.Count - 1].Properties, StringComparer.OrdinalIgnoreCase);
								}
								info.Category.AddRange(parts);

								// Add to collection
								regions.Add(info);
							}
							break;

						//mxd. Region-as-category handling
						case "#endregion":
							if(regions.Count > 0) regions.RemoveAt(regions.Count - 1);
							if(regionlines.Count > 0)
								regionlines.RemoveAt(regionlines.Count - 1);
							else
								LogWarning("Unexpected #endregion");
							break;

						default:
						{
							//mxd. In some special cases (like the whole actor commented using "//") our special comments will be detected here...
							if(objdeclaration.StartsWith("$"))
							{
								if(regions.Count > 0)
								{
									// Store region property
									regions[regions.Count - 1].Properties[objdeclaration] = new List<string> { (SkipWhitespace(false) ? ReadLine() : "") };
								}
								else
								{
									// Skip the whole line, then carry on
									ReadLine();
								}
								break;
							}
							
							// Unknown structure!
							// Best we can do now is just find the first { and then
							// follow the scopes until the matching } is found
							string token2;
							do
							{
								if(!SkipWhitespace(true)) break;
								token2 = ReadToken();
								if(string.IsNullOrEmpty(token2)) break;
							}
							while(token2 != "{");

							int scopelevel = 1;
							do
							{
								if(!SkipWhitespace(true)) break;
								token2 = ReadToken();
								if(string.IsNullOrEmpty(token2)) break;
								if(token2 == "{") scopelevel++;
								if(token2 == "}") scopelevel--;
							}
							while(scopelevel > 0);
						}
						break;
					}
				}
			}

			//mxd. Check unclosed #regions
			if(regionlines.Count > 0)
			{
				foreach(var group in regionlines)
				{
					if(!string.IsNullOrEmpty(group.Value))
						LogWarning("Unclosed #region \"" + group.Value + "\"", group.Key);
					else
						LogWarning("Unclosed #region", group.Key);
				}
			}
			
			// Return true when no errors occurred
			return (ErrorDescription == null);
		}
		
		#endregion
		
		#region ================== Methods
		
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
			foreach(ActorStructure a in actors.Values)
				if(a.DoomEdNum == doomednum) return a;
			return null;
		}

		// This returns an actor by name
		// Returns null when actor cannot be found
		internal ActorStructure GetArchivedActorByName(string name)
		{
			name = name.ToLowerInvariant();
            ActorStructure zscriptactor = (zscriptactors.ContainsKey(name) ? zscriptactors[name] : null);
            if (zscriptactor != null) return zscriptactor;
			return (archivedactors.ContainsKey(name) ? archivedactors[name] : null);
		}

        internal void ClearActors()
        {
            // Initialize
            actors = new Dictionary<string, ActorStructure>(StringComparer.OrdinalIgnoreCase);
            archivedactors = new Dictionary<string, ActorStructure>(StringComparer.OrdinalIgnoreCase);
            parsedlumps = new HashSet<string>(StringComparer.OrdinalIgnoreCase); //mxd
            damagetypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase); //mxd
        }

        #endregion
    }
}
