using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.Types;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace CodeImp.DoomBuilder.ZDoom
{

    public sealed class DecorateActorStructure : ActorStructure
    {
        #region ================== DECORATE Actor Structure parsing

        private DecorateParser parser;

        internal DecorateActorStructure(ZDTextParser zdparser, DecorateCategoryInfo catinfo)
        {
            this.catinfo = catinfo; //mxd

            parser = (DecorateParser)zdparser;
            bool done = false; //mxd

            // First next token is the class name
            parser.SkipWhitespace(true);
            classname = parser.StripTokenQuotes(parser.ReadToken(ACTOR_CLASS_SPECIAL_TOKENS));

            if (string.IsNullOrEmpty(classname))
            {
                parser.ReportError("Expected actor class name");
                return;
            }

            //mxd. Fail on duplicates // [ZZ] archived +zscript
            if (parser.GetArchivedActorByName(classname) != null)
            {
                parser.ReportError("Actor \"" + classname + "\" is double-defined");
                return;
            }

            // Parse tokens before entering the actor scope
            while (parser.SkipWhitespace(true))
            {
                string token = parser.ReadToken();
                if (!string.IsNullOrEmpty(token))
                {
                    token = token.ToLowerInvariant();

                    switch (token)
                    {
                        case ":":
                            // The next token must be the class to inherit from
                            parser.SkipWhitespace(true);
                            inheritclass = parser.StripTokenQuotes(parser.ReadToken());
                            if (string.IsNullOrEmpty(inheritclass))
                            {
                                parser.ReportError("Expected class name to inherit from");
                                return;
                            }

                            // Find the actor to inherit from
                            baseclass = parser.GetArchivedActorByName(inheritclass);
                            break;

                        case "replaces":
                            // The next token must be the class to replace
                            parser.SkipWhitespace(true);
                            replaceclass = parser.StripTokenQuotes(parser.ReadToken());
                            if (string.IsNullOrEmpty(replaceclass))
                            {
                                parser.ReportError("Expected class name to replace");
                                return;
                            }
                            break;

                        case "native":
                            // Igore this token
                            break;

                        case "{":
                            // Actor scope begins here,
                            // break out of this parse loop
                            done = true;
                            break;

                        case "-":
                            // This could be a negative doomednum (but our parser sees the - as separate token)
                            // So read whatever is after this token and ignore it (negative doomednum indicates no doomednum)
                            parser.ReadToken();
                            break;

                        default:
                            //mxd. Property begins with $? Then the whole line is a single value
                            if (token.StartsWith("$"))
                            {
                                // This is for editor-only properties such as $sprite and $category
                                props[token] = new List<string> { (parser.SkipWhitespace(false) ? parser.ReadLine() : "") };
                                continue;
                            }

                            if (!int.TryParse(token, NumberStyles.Integer, CultureInfo.InvariantCulture, out doomednum)) // Check if numeric
                            {
                                // Not numeric!
                                parser.ReportError("Expected editor number or start of actor scope while parsing \"" + classname + "\"");
                                return;
                            }

                            //mxd. Range check
                            // Only check In a map is opened, otherwise we run into problems with the resource checker
                            if (General.Map != null && ((doomednum < General.Map.FormatInterface.MinThingType) || (doomednum > General.Map.FormatInterface.MaxThingType)))
                            {
                                // Out of bounds!
                                parser.ReportError("Actor \"" + classname + "\" has invalid editor number. Editor number must be between "
                                    + General.Map.FormatInterface.MinThingType + " and " + General.Map.FormatInterface.MaxThingType);
                                return;
                            }
                            break;
                    }

                    if (done) break; //mxd
                }
                else
                {
                    parser.ReportError("Unexpected end of structure");
                    return;
                }
            }

            // Now parse the contents of actor structure
            string previoustoken = "";
            done = false; //mxd
            while (parser.SkipWhitespace(true))
            {
                string token = parser.ReadToken();
                token = token.ToLowerInvariant();

                switch (token)
                {
                    case "+":
                    case "-":
                        // Next token is a flag (option) to set or remove
                        bool flagvalue = (token == "+");
                        parser.SkipWhitespace(true);
                        string flagname = parser.ReadToken();
                        if (!string.IsNullOrEmpty(flagname))
                        {
                            // Add the flag with its value
                            flagname = flagname.ToLowerInvariant();
                            flags[flagname] = flagvalue;
                        }
                        else
                        {
                            parser.ReportError("Expected flag name");
                            return;
                        }
                        break;

                    case "action":
                    case "native":
                        // We don't need this, ignore up to the first next ;
                        while (parser.SkipWhitespace(true))
                        {
                            string t = parser.ReadToken();
                            if (string.IsNullOrEmpty(t) || t == ";") break;
                        }
                        break;

                    case "skip_super":
                        skipsuper = true;
                        break;

                    case "states":
                        // Skip cast type if there is one
                        if (!SkipCastTypes())
                            return;

						if (!parser.NextTokenIs("{", true))
							return;

                        // Now parse actor states until we reach the end of the states structure
                        while (parser.SkipWhitespace(true))
                        {
                            string statetoken = parser.ReadToken();
                            if (!string.IsNullOrEmpty(statetoken))
                            {
                                // End of scope?
                                if (statetoken == "}")
                                {
                                    // Done with the states,
                                    // break out of this parse loop
                                    break;
                                }
                                // State label?
                                else if (statetoken == ":")
                                {
                                    if (!string.IsNullOrEmpty(previoustoken))
                                    {
                                        // Parse actor state
                                        StateStructure st = new DecorateStateStructure(this, parser);
                                        if (parser.HasError) return;
                                        states[previoustoken.ToLowerInvariant()] = st;
                                    }
                                    else
                                    {
                                        parser.ReportError("Expected actor state name");
                                        return;
                                    }
                                }
                                else
                                {
                                    // Keep token
                                    previoustoken = statetoken;
                                }
                            }
                            else
                            {
                                parser.ReportError("Unexpected end of structure");
                                return;
                            }
                        }
                        break;

                    case "var": //mxd
                                // Type
                        parser.SkipWhitespace(true);
                        string typestr = parser.ReadToken().ToUpperInvariant();
                        UniversalType type = UniversalType.EnumOption; // There is no Unknown type, so let's use something impossiburu...
                        switch (typestr)
                        {
                            case "INT": type = UniversalType.Integer; break;
                            case "FLOAT": type = UniversalType.Float; break;
                            default: parser.LogWarning("Unknown user variable type"); break;
                        }

                        // Name
                        parser.SkipWhitespace(true);
                        string name = parser.ReadToken();
                        if (string.IsNullOrEmpty(name))
                        {
                            parser.ReportError("Expected User Variable name");
                            return;
                        }
                        if (!name.StartsWith("user_", StringComparison.OrdinalIgnoreCase))
                        {
                            parser.ReportError("User Variable name must start with \"user_\" prefix");
                            return;
                        }
                        if (uservars.ContainsKey(name))
                        {
                            parser.ReportError("User Variable \"" + name + "\" is double defined");
                            return;
                        }
                        if (!skipsuper && baseclass != null && baseclass.uservars.ContainsKey(name))
                        {
                            parser.ReportError("User variable \"" + name + "\" is already defined in one of the parent classes");
                            return;
                        }

                        // Rest
                        parser.SkipWhitespace(true);
                        string next = parser.ReadToken();
                        if (next == "[") // that's User Array. Let's skip it...
                        {
                            int arrlen = -1;
                            if (!parser.ReadSignedInt(ref arrlen))
                            {
                                parser.ReportError("Expected User Array length");
                                return;
                            }
                            if (arrlen < 1)
                            {
                                parser.ReportError("User Array length must be a positive value");
                                return;
                            }
                            if (!parser.NextTokenIs("]") || !parser.NextTokenIs(";"))
                            {
                                return;
                            }
                        }
                        else if (next != ";")
                        {
                            parser.ReportError("Expected \";\", but got \"" + next + "\"");
                            return;
                        }
                        else
                        {
                            // Add to collection
                            uservars.Add(name, type);
                        }
                        break;

                    case "}":
                        //mxd. Get user vars from the BaseClass, if we have one
                        if (!skipsuper && baseclass != null && baseclass.uservars.Count > 0)
                        {
                            foreach (var group in baseclass.uservars)
                                uservars.Add(group.Key, group.Value);
                        }

                        // Actor scope ends here, break out of this parse loop
                        done = true;
                        break;

                    // Monster property?
                    case "monster":
                        // This sets certain flags we are interested in
                        flags["shootable"] = true;
                        flags["countkill"] = true;
                        flags["solid"] = true;
                        flags["canpushwalls"] = true;
                        flags["canusewalls"] = true;
                        flags["activatemcross"] = true;
                        flags["canpass"] = true;
                        flags["ismonster"] = true;
                        break;

                    // Projectile property?
                    case "projectile":
                        // This sets certain flags we are interested in
                        flags["noblockmap"] = true;
                        flags["nogravity"] = true;
                        flags["dropoff"] = true;
                        flags["missile"] = true;
                        flags["activateimpact"] = true;
                        flags["activatepcross"] = true;
                        flags["noteleport"] = true;
                        break;

                    // Clearflags property?
                    case "clearflags":
                        // Clear all flags
                        flags.Clear();
                        break;

                    // Game property?
                    case "game":
                        // Include all tokens on the same line
                        List<string> games = new List<string>();
                        while (parser.SkipWhitespace(false))
                        {
                            string v = parser.ReadToken();
                            if (string.IsNullOrEmpty(v))
                            {
                                parser.ReportError("Expected \"Game\" property value");
                                return;
                            }
                            if (v == "\n") break;
                            if (v == "}") return; //mxd
                            if (v != ",") games.Add(v.ToLowerInvariant());
                        }
                        props[token] = games;
                        break;

                    // Property
                    default:
                        // Property begins with $? Then the whole line is a single value
                        if (token.StartsWith("$"))
                        {
                            // This is for editor-only properties such as $sprite and $category
                            props[token] = new List<string> { (parser.SkipWhitespace(false) ? parser.ReadLine() : "") };
                        }
                        else
                        {
                            // Next tokens up until the next newline are values
                            List<string> values = new List<string>();
                            while (parser.SkipWhitespace(false))
                            {
                                string v = parser.ReadToken();
                                if (string.IsNullOrEmpty(v))
                                {
                                    parser.ReportError("Unexpected end of structure");
                                    return;
                                }
                                if (v == "\n") break;
                                if (v == "}") return; //mxd
                                if (v != ",") values.Add(v);
                            }

                            //mxd. Translate scale to xscale and yscale
                            if (token == "scale")
                            {
                                props["xscale"] = values;
                                props["yscale"] = values;
                            }
                            else
                            {
                                props[token] = values;
                            }
                        }
                        break;
                }

                if (done) break; //mxd

                // Keep token
                previoustoken = token;
            }

            // parsing done, process thing arguments
            ParseCustomArguments();

            // Only check if a map is opened. Otherwise we run into problems with the resource checker
            if (General.Map != null)
            {
                //mxd. Check if baseclass is valid
                if (inheritclass.ToLowerInvariant() != "actor" && doomednum > -1)
                {
                    //check if this class inherits from a class defined in game configuration
                    Dictionary<int, ThingTypeInfo> things = General.Map.Config.GetThingTypes();
                    string inheritclasscheck = inheritclass.ToLowerInvariant();

                    foreach (KeyValuePair<int, ThingTypeInfo> ti in things)
                    {
                        if (!string.IsNullOrEmpty(ti.Value.ClassName) && ti.Value.ClassName.ToLowerInvariant() == inheritclasscheck)
                        {
                            //states
                            // [ZZ] allow internal prefix here. it can inherit MapSpot, light, or other internal stuff.
                            if (states.Count == 0 && !string.IsNullOrEmpty(ti.Value.Sprite))
                                states.Add("spawn", new StateStructure(ti.Value.Sprite.StartsWith(DataManager.INTERNAL_PREFIX) ? ti.Value.Sprite : ti.Value.Sprite.Substring(0, 5)));

                            if (baseclass == null)
                            {
                                //flags
                                if (ti.Value.Hangs && !flags.ContainsKey("spawnceiling"))
                                    flags["spawnceiling"] = true;

                                if (ti.Value.Blocking > 0 && !flags.ContainsKey("solid"))
                                    flags["solid"] = true;

                                //properties
                                if (!props.ContainsKey("height"))
                                    props["height"] = new List<string> { ti.Value.Height.ToString() };

                                if (!props.ContainsKey("radius"))
                                    props["radius"] = new List<string> { ti.Value.Radius.ToString() };
                            }

                            // [ZZ] inherit arguments from game configuration
                            //      
                            if (!props.ContainsKey("$clearargs"))
                            {
                                for (int i = 0; i < 5; i++)
                                {
                                    if (args[i] != null)
                                        continue; // don't touch it if we already have overrides

                                    ArgumentInfo arg = ti.Value.Args[i];
                                    if (arg != null && arg.Used)
                                        args[i] = arg;
                                }
                            }

                            return;
                        }
                    }

                    if (baseclass == null)
                        parser.LogWarning("Unable to find \"" + inheritclass + "\" class to inherit from, while parsing \"" + classname + ":" + doomednum + "\"");
                }
            }
        }

        /// <summary>
        /// Skips state cast type, it one exists.
        /// </summary>
        /// <returns>true if there was nothing to skip or if skippen succeeded, false if it failed</returns>
		private bool SkipCastTypes()
		{
            // Alloed casts, see https://zdoom.org/wiki/Converting_DECORATE_code_to_ZScript#Cast_types
            string[] allowedcasts = { "actor", "overlay", "weapon", "item" };

            // Skip cast types, which can be defined like this:
            //   States(Actor, Item)
            // See https://github.com/UltimateDoomBuilder/UltimateDoomBuilder/issues/977
            if (parser.NextTokenIs("(", false))
            {
                while (parser.SkipWhitespace(true))
                {
                    string token = parser.ReadToken().ToLowerInvariant();

                    if(!allowedcasts.Contains(token))
					{
                        parser.ReportError($"Unexpected cast type \"{token}\", expected one of: " + string.Join(", ", allowedcasts));
                        return false;
					}

                    parser.SkipWhitespace(true);

                    // Get next token
                    token = parser.ReadToken();

                    // End of cast type?
                    if(token == ")")
                        return true;

                    // More cast types coming?
                    if (token == ",")
                        continue;

                    // No comma or brace after a cast type, so report an error and return
                    parser.ReportError($"Expected \",\", or \")\", got {token}");
                    return false;
                }
            }
            else
            {
                // We get here if there was no cast type
                return true;
            }

            // We should never get here, since we'll return in the parsing loop,
            // but we need to make the compiler happy
            return false;
        }

		#endregion
	}
}
