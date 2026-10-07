#region ================== Namespaces

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Windows;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	[FindReplace("Sector Flags", BrowseButton = true)]
	internal class FindSectorFlags : BaseFindSector
	{
		#region ================== Properties

		public override Image BrowseImage { get { return Properties.Resources.List; } }

		#endregion

		#region ================== Methods

		// This is called to test if the item should be displayed
		public override bool DetermineVisiblity() 
		{
			return General.Map.Config.SectorFlags.Count > 0 
				|| General.Map.Config.CeilingPortalFlags.Count > 0 
				|| General.Map.Config.FloorPortalFlags.Count > 0;
		}

		// This is called when the browse button is pressed
		public override string Browse(string initialvalue)
		{
			return FlagsForm.ShowDialog(Form.ActiveForm, initialvalue, GetAllFlags());
		}

		// This is called to perform a search (and replace)
		// Returns a list of items to show in the results list
		// replacewith is null when not replacing
		public override FindReplaceObject[] Find(string value, bool withinselection, bool replace, string replacewith, bool keepselection)
		{
			List<FindReplaceObject> objs = new List<FindReplaceObject>();

			// Where to search?
			ICollection<Sector> list = withinselection ? General.Map.Map.GetSelectedSectors(true) : General.Map.Map.Sectors;

			// Combine all sector flags...
			Dictionary<string, string> allflags = GetAllFlags();

			// Find what? (mxd)
			Dictionary<string, bool> findflagslist = new Dictionary<string, bool>();
			foreach(string flag in value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)) 
			{
				string f = flag.Trim();
				bool setflag = true;
				if(f.StartsWith("!"))
				{
					setflag = false;
					f = f.Substring(1, f.Length - 1);
				}

				if(allflags.ContainsKey(f)) findflagslist.Add(f, setflag);
			}
			if(findflagslist.Count == 0) 
			{
				MessageBox.Show("Invalid value for this search type!", "Find and Replace", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return objs.ToArray();
			}

			// Replace with what? (mxd)
			Dictionary<string, bool> replaceflagslist = new Dictionary<string, bool>();
			if(replace) 
			{
				string[] replaceflags = replacewith.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
				foreach(string flag in replaceflags) 
				{
					string f = flag.Trim();
					bool setflag = true;
					if(f.StartsWith("!"))
					{
						setflag = false;
						f = f.Substring(1, f.Length - 1);
					}

					if(!allflags.ContainsKey(f))
					{
						MessageBox.Show("Invalid replace value \"" + f + "\" for this search type!", "Find and Replace", MessageBoxButtons.OK, MessageBoxIcon.Error);
						return objs.ToArray();
					}
					replaceflagslist.Add(f, setflag);
				}
			}

			// Go for all linedefs
			foreach(Sector s in list)
			{
				bool match = true;

				// Parse the value string...
				foreach(KeyValuePair<string, bool> group in findflagslist)
				{
					// ...and check if the flag doesn't match
					if(group.Value != s.IsFlagSet(group.Key))
					{
						match = false;
						break;
					}
				}

				// Flags matches?
				if(match)
				{
					// Replace flags (mxd)
					if(replace) 
					{
						// Set new flags
						foreach(KeyValuePair<string, bool> group in replaceflagslist) s.SetFlag(group.Key, group.Value);
					}
					
					// Add to list
					SectorEffectInfo info = General.Map.Config.GetSectorEffectInfo(s.Effect);
					if(!info.IsNull)
						objs.Add(new FindReplaceObject(s, "Sector " + s.Index + " (" + info.Title + ")"));
					else
						objs.Add(new FindReplaceObject(s, "Sector " + s.Index));
				}
			}

			return objs.ToArray();
		}

		private static Dictionary<string, string> GetAllFlags()
		{
			// Combine all sector flags...
			Dictionary<string, string> allflags = new Dictionary<string, string>(General.Map.Config.SectorFlags);
			
			foreach(var group in General.Map.Config.CeilingPortalFlags)
				allflags[group.Key] = group.Value + " (ceil. portal)";

			foreach(var group in General.Map.Config.FloorPortalFlags)
				allflags[group.Key] = group.Value + " (floor portal)";

			return allflags;
		} 

		#endregion
	}
}
