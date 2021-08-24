
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
using System.Globalization;
using CodeImp.DoomBuilder.IO;
using CodeImp.DoomBuilder.Map;

#endregion

namespace CodeImp.DoomBuilder.Config
{
	public struct ErrorCheckerExemptions
	{
		public bool IgnoreUpperTexture;
		public bool IgnoreMiddleTexture;
		public bool IgnoreLowerTexture;
	}

	public class LinedefActionInfo : INumberedTitle, IComparable<LinedefActionInfo>
	{
		#region ================== Constants

		#endregion

		#region ================== Constants



		#endregion

		#region ================== Variables

		// Properties
		private readonly int index;
		private readonly string prefix;
		private readonly string category;
		private readonly string name;
		private readonly string title;
		private readonly string id; //mxd. wiki-compatible name 
		private readonly ArgumentInfo[] args;
		private readonly bool isgeneralized;
		private readonly bool isknown;
		private readonly bool requiresactivation; //mxd
		private readonly bool linetolinetag;
		private readonly bool linetolinesameaction;
		private readonly ErrorCheckerExemptions errorcheckerexemptions;
		
		#endregion

		#region ================== Properties

		public int Index { get { return index; } }
		public string Prefix { get { return prefix; } }
		public string Category { get { return category; } }
		public string Name { get { return name; } }
		public string Title { get { return title; } }
		public string Id { get { return id; } } //mxd
		public bool IsGeneralized { get { return isgeneralized; } }
		public bool IsKnown { get { return isknown; } }
		public bool IsNull { get { return (index == 0); } }
		public bool RequiresActivation { get { return requiresactivation; } } //mxd
		public ArgumentInfo[] Args { get { return args; } }
		public bool LineToLineTag { get { return linetolinetag; } }
		public bool LineToLineSameAction { get { return linetolinesameaction; } }
		public ErrorCheckerExemptions ErrorCheckerExemptions { get { return errorcheckerexemptions; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		internal LinedefActionInfo(int index, Configuration cfg, string categoryname, IDictionary<string, EnumList> enums)
		{
			string actionsetting = "linedeftypes." + categoryname + "." + index.ToString(CultureInfo.InvariantCulture);
			
			// Initialize
			this.index = index;
			this.category = categoryname;
			this.args = new ArgumentInfo[Linedef.NUM_ARGS];
			this.isgeneralized = false;
			this.isknown = true;
			this.errorcheckerexemptions = new ErrorCheckerExemptions();
			
			// Read settings
			this.name = cfg.ReadSetting(actionsetting + ".title", "Unnamed");
			this.id = cfg.ReadSetting(actionsetting + ".id", string.Empty); //mxd
			this.prefix = cfg.ReadSetting(actionsetting + ".prefix", "");
			this.requiresactivation = cfg.ReadSetting(actionsetting + ".requiresactivation", true); //mxd
			this.title = this.prefix + " " + this.name;
			this.title = this.title.Trim();

			// Read line-to-line associations
			this.linetolinetag = cfg.ReadSetting(actionsetting + ".linetolinetag", false);
			this.linetolinesameaction = cfg.ReadSetting(actionsetting + ".linetolinesameaction", false);

			// Error checker exemptions
			this.errorcheckerexemptions.IgnoreUpperTexture = cfg.ReadSetting(actionsetting + ".errorchecker.ignoreuppertexture", false);
			this.errorcheckerexemptions.IgnoreMiddleTexture = cfg.ReadSetting(actionsetting + ".errorchecker.ignoremiddletexture", false);
			this.errorcheckerexemptions.IgnoreLowerTexture = cfg.ReadSetting(actionsetting + ".errorchecker.ignorelowertexture", false);

			// Read the args
			for (int i = 0; i < Linedef.NUM_ARGS; i++)
				this.args[i] = new ArgumentInfo(cfg, actionsetting, i, enums);
			
			// We have no destructor
			GC.SuppressFinalize(this);
		}
		
		// Constructor for generalized type display
		internal LinedefActionInfo(int index, string title, bool isknown, bool isgeneralized)
		{
			this.index = index;
			this.isgeneralized = isgeneralized;
			this.isknown = isknown;
			this.requiresactivation = true; //mxd. Unused, set for consistency sake.
			this.title = title;
			this.args = new ArgumentInfo[Linedef.NUM_ARGS];
			for(int i = 0; i < Linedef.NUM_ARGS; i++)
				this.args[i] = new ArgumentInfo(i);
		}

		#endregion

		#region ================== Methods

		// This presents the item as string
		public override string ToString()
		{
			return index + " - " + title;
		}

		// This compares against another action info
		public int CompareTo(LinedefActionInfo other)
		{
			if(this.index < other.index) return -1;
			else if(this.index > other.index) return 1;
			else return 0;
		}

		#endregion
	}
}
