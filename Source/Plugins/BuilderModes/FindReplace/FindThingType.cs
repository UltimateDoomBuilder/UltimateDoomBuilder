
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
using System.Linq;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	[FindReplace("Thing Type", BrowseButton = true)]
	internal class FindThingType : BaseFindThing
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		#endregion

		#region ================== Properties

		public override Presentation RenderPresentation { get { return Presentation.Things; } }
		public override Image BrowseImage { get { return Properties.Resources.List; } }
		
		#endregion

		#region ================== Constructor / Destructor

		#endregion

		#region ================== Methods

		// This is called when the browse button is pressed
		public override string Browse(string initialvalue)
		{
			// jaeden: enabled multiselect for thing type find
			int[] types;
			types = ParseTypes(initialvalue);
			types = Windows.ThingMultipleBrowserForm.BrowseThings(BuilderPlug.Me.FindReplaceForm, types);
			return TypesToString(types);
		}

		// Converts array of thing type numbers to a string
		private string TypesToString(int[] types)
		{
			return string.Join(",", types);
		}

		// Converts string of thing type numbers to number array
		private int[] ParseTypes(string typestring)
		{
			string[] typestrings = typestring.Split(',');

			List<int> types = new List<int>();
			foreach (var ts in typestrings)
			{
				string trimmed = ts.Trim();
				if (trimmed.Length == 0)
					continue;

				if (int.TryParse(trimmed, out int parsed))
					types.Add(parsed);
				else
					return Array.Empty<int>();
			}
			return types.ToArray();
		}

		// This is called to perform a search (and replace)
		// Returns a list of items to show in the results list
		// replacewith is null when not replacing
		public override FindReplaceObject[] Find(string value, bool withinselection, bool replace, string replacewith, bool keepselection)
		{
			List<FindReplaceObject> objs = new List<FindReplaceObject>();

			// Interpret the replacement
			int[] replacetypes = ParseTypes(replacewith);
			if(replace)
			{
				if (replacetypes.Length == 0 || replacetypes.Any(rt => rt < General.Map.FormatInterface.MinThingType || rt > General.Map.FormatInterface.MaxThingType))
				{
					MessageBox.Show("Invalid replace value for this search type!", "Find and Replace", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return objs.ToArray();
				}
			}

			// Interpret the values to find
			HashSet<int> valuestofind = ParseTypes(value).ToHashSet();
			if (valuestofind.Count > 0)
			{
				// Where to search?
				ICollection<Thing> list = withinselection ? General.Map.Map.GetSelectedThings(true) : General.Map.Map.Things;

				// Go for all things
				foreach(Thing t in list)
				{
					// Match?
					if(valuestofind.Contains(t.Type))
					{
						// Replace
						if(replace && replacetypes.Length > 0)
						{
							t.Type = replacetypes[General.Random(0, replacetypes.Length - 1)];
							t.UpdateConfiguration();
						}
						
						// Add to list
						ThingTypeInfo ti = General.Map.Data.GetThingInfo(t.Type);
						objs.Add(new FindReplaceObject(t, "Thing " + t.Index + " (" + ti.Title + ")"));
					}
				}
			}
			
			return objs.ToArray();
		}

		#endregion
	}
}
