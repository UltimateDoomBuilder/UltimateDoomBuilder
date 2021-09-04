
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

#endregion

namespace CodeImp.DoomBuilder.Windows
{
	internal enum FindReplaceSearchMode //mxd
	{
		CURRENT_FILE,
		OPENED_TABS_ALL_SCRIPT_TYPES
	}
	
	internal struct FindReplaceOptions
	{
		public string FindText;
		public bool CaseSensitive;
		public bool WholeWord;
		public string ReplaceWith;
		public FindReplaceSearchMode SearchMode; //mxd
		public bool WrapAroundDisabled;

		//mxd. Copy constructor
		public FindReplaceOptions(FindReplaceOptions other)
		{
			FindText = other.FindText;
			CaseSensitive = other.CaseSensitive;
			WholeWord = other.WholeWord;
			ReplaceWith = other.ReplaceWith;
			SearchMode = other.SearchMode;
			WrapAroundDisabled = other.WrapAroundDisabled;
		}
	}
}
