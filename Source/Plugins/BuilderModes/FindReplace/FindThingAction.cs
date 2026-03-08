
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
using System.Windows.Forms;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Rendering;
using System.Drawing;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Types;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	[FindReplace("Thing Action and Arguments", BrowseButton = true)]
	internal class FindThingAction : BaseFindThing
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		#endregion

		#region ================== Properties

		public override Presentation RenderPresentation { get { return Presentation.Things; } }
		public override Image BrowseImage { get { return Properties.Resources.List; } }
		public override string UsageHint { get { return "Usage: action [arg1 [arg2 [arg3 [arg4 [arg5]]]]]" + Environment.NewLine
			+ "Arg value can be \"*\" (any value)" + Environment.NewLine
			+ "Arg1 can be script name when searching for ACS specials"; } }

		#endregion

		#region ================== Constructor / Destructor

		#endregion

		#region ================== Methods

		// This is called to test if the item should be displayed
		public override bool DetermineVisiblity()
		{
			return General.Map.FormatInterface.HasThingAction;
		}

		// This is called when the browse button is pressed
		public override string Browse(string initialvalue)
		{
			int action;
			int.TryParse(initialvalue, out action);
			return General.Interface.BrowseLinedefActions(BuilderPlug.Me.FindReplaceForm, action, true).ToString();
		}

		// This is called when the browse replace button is pressed
		public override string BrowseReplace(string initialvalue)
		{
			int action;
			int.TryParse(initialvalue, out action);
			return General.Interface.BrowseLinedefActions(BuilderPlug.Me.FindReplaceForm, action).ToString();
		}

		// This is called to perform a search (and replace)
		// Returns a list of items to show in the results list
		// replacewith is null when not replacing
		public override FindReplaceObject[] Find(string value, bool withinselection, bool replace, string replacewith, bool keepselection)
		{
			List<FindReplaceObject> objs = new List<FindReplaceObject>();

			// Interpret the replacement
			int replaceaction = 0;
			string replacearg0str = string.Empty; //mxd
			int[] replaceargs = null; //mxd
			if(replace) 
			{
				string[] replaceparts = replacewith.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

				// If it cannot be interpreted, set replacewith to null (not replacing at all)
				if(replaceparts.Length == 0) replacewith = null; //mxd
				if(!int.TryParse(replaceparts[0], out replaceaction)) replacewith = null;
				if(replaceaction < 0) replacewith = null;
				if(replaceaction > Int16.MaxValue) replacewith = null;
				if(replacewith == null)
				{
					MessageBox.Show("Invalid replace value for this search type!", "Find and Replace", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return objs.ToArray();
				}

				//mxd. Now try parsing the args
				if(replaceparts.Length > 1) 
				{
					replaceargs = new[] { int.MinValue, int.MinValue, int.MinValue, int.MinValue, int.MinValue };
					int i = 1;

                    //mxd. Named script search support...
                    if (General.Map.UDMF)
                    {
                        string possiblescriptname = replaceparts[1].Trim();
                        int tmp;
                        if (!string.IsNullOrEmpty(possiblescriptname) && possiblescriptname != "*" && !int.TryParse(possiblescriptname, out tmp))
                        {
                            replacearg0str = possiblescriptname.Replace("\"", "");
                            i = 2;
                        }
                    }

                    for (; i < replaceparts.Length && i < replaceargs.Length + 1; i++) 
					{
						int argout;
						if(replaceparts[i].Trim() == "*") continue; //mxd. Any arg value support
						if(int.TryParse(replaceparts[i].Trim(), out argout)) replaceargs[i - 1] = argout;
					}
				}
			}

			// Interpret the number given
			int action;
			string[] parts = value.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

			//For the search, the user may make the following queries:
			//	action arg0 arg1 arg2 arg3 arg4
			//	action arg0str arg1 arg2 arg3 arg4
			//
			//this allows users to search for lines that contain actions with specific arguments.
			//useful for locating script lines

			if(int.TryParse(parts[0], out action)) 
			{
				int[] args = null;
				string arg0str = string.Empty; //mxd

				//parse the arg values out
				if(parts.Length > 1) 
				{
					args = new[] { int.MinValue, int.MinValue, int.MinValue, int.MinValue, int.MinValue };
					int i = 1;

                    //mxd. Named script search support...
                    if (General.Map.UDMF)
                    {
                        // [ZZ] edit: we can enclose number with "" to signify a named script called "1".
                        //      this is achieved by trying to parse the number before removing "'s.
                        string possiblescriptname = parts[1].Trim();
                        int tmp;
                        if (!string.IsNullOrEmpty(possiblescriptname) && possiblescriptname != "*" && !int.TryParse(possiblescriptname, out tmp))
                        {
                            arg0str = possiblescriptname.Replace("\"", "").ToLowerInvariant();
                            i = 2;
                        }
                    }

                    for (; i < parts.Length && i < args.Length + 1; i++) 
					{
						int argout;
						if(parts[i].Trim() == "*") continue; //mxd. Any arg value support
						if(int.TryParse(parts[i].Trim(), out argout)) args[i - 1] = argout;
					}
				}

				// Where to search?
				ICollection<Thing> list = withinselection ? General.Map.Map.GetSelectedThings(true) : General.Map.Map.Things;

				// Go for all things
				foreach(Thing t in list) 
				{
					// Action matches? -1 means any action (mxd)
					if((action == -1 && t.Action == 0) || (action > -1 && t.Action != action)) continue;

					bool match = true;
					string argtext = "";

					//if args were specified, then process them
					if(args != null) 
					{
						int x = 0;
						argtext = ". Args: ";

						//mxd. Check script name...
						if(!string.IsNullOrEmpty(arg0str)) 
						{
							string s = t.Fields.GetValue("arg0str", string.Empty);
							if(s.ToLowerInvariant() != arg0str)
								match = false;
							else
								argtext += "\"" + s + "\"";

							x = 1;
						}

						for(; x < args.Length; x++) 
						{
							if(args[x] != int.MinValue && args[x] != t.Args[x]) 
							{
								match = false;
								break;
							}
							argtext += (x == 0 ? "" : ", ") + t.Args[x];
						}
					}
					//mxd. Add action args
					else
					{
						List<string> argslist = new List<string>();
						LinedefActionInfo info = General.Map.Config.GetLinedefActionInfo(t.Action);

						// Process args
						for (int i = t.Args.Length - 1; i >= 0; i--)
						{
							if (info.Args[i].Used)
								argslist.Insert(0, t.Args[i].ToString());
							else if (argslist.Count != 0) // Show unused args as "-" if they are not at the last args
								argslist.Insert(0, "-");
						}

						// Process arg0str...
						//if(Array.IndexOf(GZGeneral.ACS_SPECIALS, t.Action) != -1)
						{
							string s = t.Fields.GetValue("arg0str", string.Empty);
							if(!string.IsNullOrEmpty(s))
							{
								if(argslist.Count > 0)
									argslist[0] = "\"" + s + "\"";
								else
									argslist.Add("\"" + s + "\"");
							}
						}

						// Create args string
						if(argslist.Count > 0)
						{
							argtext = ". Args: " + string.Join(", ", argslist.ToArray());
						}
					}

					if(match) 
					{
                        // Replace
                        ThingTypeInfo ti = General.Map.Data.GetThingInfo(t.Type);

                        if (replace) 
						{
							t.Action = replaceaction;
                            LinedefActionInfo info = (t.Action > 0) ? General.Map.Config.GetLinedefActionInfo(t.Action) : null;
                            ArgumentInfo[] arginfo = (info != null) ? info.Args : ti.Args;

                            //mxd. Replace args as well?
                            if (replaceargs != null) 
							{
								int i = 0;
								if(!string.IsNullOrEmpty(replacearg0str) && arginfo[0].Str) 
								{
									t.Fields["arg0str"] = new UniValue(UniversalType.String, replacearg0str);
									i = 1;
								}

								for(; i < replaceargs.Length; i++) 
								{
									if(replaceargs[i] != int.MinValue) t.Args[i] = replaceargs[i];
								}
							}
						}

						// Add to list
						objs.Add(new FindReplaceObject(t, "Thing " + t.Index + " (" + ti.Title + argtext + ")"));
					}
				}
			}

			return objs.ToArray();
		}

		#endregion
	}
}
