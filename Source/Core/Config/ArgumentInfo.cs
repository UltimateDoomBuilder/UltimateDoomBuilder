
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
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using CodeImp.DoomBuilder.IO;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Types;
using CodeImp.DoomBuilder.ZDoom;

#endregion

namespace CodeImp.DoomBuilder.Config
{
	public class ArgumentInfo
	{
		#region ================== Constants

		private const int HELPER_SHAPE_ALPHA = 192; //mxd
		private const int RANGE_SHAPE_ALPHA = 96; //mxd

		#endregion

		#region ================== Enums (mxd)

		public enum ArgumentRenderStyle
		{
			NONE,
			CIRCLE,
			RECTANGLE,
		}

		#endregion

		#region ================== Variables

		private readonly string title;
		private readonly string tooltip; //mxd
		private readonly bool used;
		private readonly int type;
		private EnumList enumlist;
		private EnumList flagslist; //mxd
		private readonly object defaultvalue; //mxd
		private readonly HashSet<string> targetclasses; //mxd
		private readonly ArgumentRenderStyle renderstyle; //mxd
		private readonly PixelColor rendercolor; //mxd
		private readonly PixelColor minrangecolor; //mxd
		private readonly PixelColor maxrangecolor; //mxd
		private readonly int minrange; //mxd
		private readonly int maxrange; //mxd
        private readonly bool str; // [ZZ]
        private readonly string titlestr; // [ZZ]

		#endregion

		#region ================== Properties

		public string Title { get { return title; } }
		public string ToolTip { get { return tooltip; } } //mxd
		public bool Used { get { return used; } }
		public int Type { get { return type; } }
		public HashSet<string> TargetClasses { get { return targetclasses; } } //mxd
		public EnumList Enum { get { return enumlist; } internal set { enumlist = value; } }
		public EnumList Flags { get { return flagslist; } internal set { flagslist = value; } } //mxd
		public object DefaultValue { get { return defaultvalue; } } //mxd
		public ArgumentRenderStyle RenderStyle { get { return renderstyle; } } //mxd
		public PixelColor RenderColor { get { return rendercolor; } } //mxd
		public PixelColor MinRangeColor { get { return minrangecolor; } } //mxd
		public PixelColor MaxRangeColor { get { return maxrangecolor; } } //mxd
		public int MinRange { get { return minrange; } } //mxd
		public int MaxRange { get { return maxrange; } } //mxd
        public bool Str { get { return str; } } // [ZZ]
        public string TitleStr { get { return titlestr; } } // [ZZ]

		#endregion

		#region ================== Constructor / Disposer

		// Constructor for argument info from configuration
		internal ArgumentInfo(Configuration cfg, string argspath, int argindex, IDictionary<string, EnumList> enums)
		{
			// Read
			string istr = argindex.ToString(CultureInfo.InvariantCulture);
			this.used = cfg.SettingExists(argspath + ".arg" + istr);
			this.title = cfg.ReadSetting(argspath + ".arg" + istr + ".title", "Argument " + (argindex + 1));
			this.tooltip = cfg.ReadSetting(argspath + ".arg" + istr + ".tooltip", string.Empty); //mxd
			this.type = cfg.ReadSetting(argspath + ".arg" + istr + ".type", 0);
            this.str = cfg.ReadSetting(argspath + ".arg" + istr + ".str", false);
            this.titlestr = cfg.ReadSetting(argspath + ".arg" + istr + ".titlestr", this.title);
            this.defaultvalue = cfg.ReadSetting(argspath + ".arg" + istr + ".default", 0);

            //mxd. Get rendering hint settings
            string renderstyle = cfg.ReadSetting(argspath + ".arg" + istr + ".renderstyle", string.Empty);
			switch(renderstyle.ToLowerInvariant())
			{
				case "circle":
					this.renderstyle = ArgumentRenderStyle.CIRCLE;
					break;
				case "rectangle":
					this.renderstyle = ArgumentRenderStyle.RECTANGLE;
					break;
				default:
					this.renderstyle = ArgumentRenderStyle.NONE;
					if(!string.IsNullOrEmpty(renderstyle))
						General.ErrorLogger.Add(ErrorType.Error, "\"" + argspath + ".arg" + istr + "\": action argument \"" + this.title + "\" has unknown renderstyle \"" + renderstyle + "\"!");
					break;
			}

			if(this.renderstyle != ArgumentRenderStyle.NONE)
			{
				// Get rendercolor
				string rendercolor = cfg.ReadSetting(argspath + ".arg" + istr + ".rendercolor", string.Empty);
				this.rendercolor = General.Colors.InfoLine;

				if(!string.IsNullOrEmpty(rendercolor) && !ZDTextParser.GetColorFromString(rendercolor, out this.rendercolor))
					General.ErrorLogger.Add(ErrorType.Error, "\"" + argspath + ".arg" + istr + "\": action argument \"" + this.title + "\": unable to get rendercolor from value \"" + rendercolor + "\"!");

				this.rendercolor.a = HELPER_SHAPE_ALPHA;

				// Get minrange settings
				string minrange = cfg.ReadSetting(argspath + ".arg" + istr + ".minrange", string.Empty);
				if(int.TryParse(minrange, NumberStyles.Integer, CultureInfo.InvariantCulture, out this.minrange) && this.minrange > 0f)
				{
					// Get minrangecolor
					string minrangecolor = cfg.ReadSetting(argspath + ".arg" + istr + ".minrangecolor", string.Empty);
					this.minrangecolor = General.Colors.Indication;

					if(!string.IsNullOrEmpty(minrangecolor) && !ZDTextParser.GetColorFromString(minrangecolor, out this.minrangecolor))
						General.ErrorLogger.Add(ErrorType.Error, "\"" + argspath + ".arg" + istr + "\": action argument \"" + this.title + "\": unable to get minrangecolor from value \"" + minrangecolor + "\"!");

					this.minrangecolor.a = RANGE_SHAPE_ALPHA;
				}

				// Get maxrange settings
				string maxrange = cfg.ReadSetting(argspath + ".arg" + istr + ".maxrange", string.Empty);
				if(int.TryParse(maxrange, NumberStyles.Integer, CultureInfo.InvariantCulture, out this.maxrange) && this.maxrange > 0f)
				{
					// Get minrangecolor
					string maxrangecolor = cfg.ReadSetting(argspath + ".arg" + istr + ".maxrangecolor", string.Empty);
					this.maxrangecolor = General.Colors.Indication;

					if(!string.IsNullOrEmpty(maxrangecolor) && !ZDTextParser.GetColorFromString(maxrangecolor, out this.maxrangecolor))
						General.ErrorLogger.Add(ErrorType.Error, "\"" + argspath + ".arg" + istr + "\": action argument \"" + this.title + "\": unable to get maxrangecolor from value \"" + maxrangecolor + "\"!");

					this.maxrangecolor.a = RANGE_SHAPE_ALPHA;
				}

				// Update tooltip?
				if(this.minrange > 0f || this.maxrange > 0f)
				{
					if(!string.IsNullOrEmpty(this.tooltip)) this.tooltip += Environment.NewLine;

					if(this.minrange > 0f && this.maxrange > 0f)
						this.tooltip += "Range: " + this.minrange + " - " + this.maxrange;
					else if(this.minrange > 0f)
						this.tooltip += "Minimum range: " + this.minrange;
					else
						this.tooltip += "Maximum range: " + this.maxrange;
				}
			}

			//mxd. Check for TargetClass?
			this.targetclasses = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
			if(this.type == (int)UniversalType.ThingTag)
			{
				string s = cfg.ReadSetting(argspath + ".arg" + istr + ".targetclasses", string.Empty);
				if(!string.IsNullOrEmpty(s))
				{
					foreach(string tclass in s.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)) 
						targetclasses.Add(tclass.Trim());
				}
			}

			// Determine enum type
			IDictionary argdic = cfg.ReadSetting(argspath + ".arg" + istr, new Hashtable());
			if(argdic.Contains("enum"))
			{
				// Enum fully specified?
				if(argdic["enum"] is IDictionary)
				{
					// Create anonymous enum
					this.enumlist = new EnumList((IDictionary)argdic["enum"]);
				}
				else
				{
					// Check if referenced enum exists
					if((argdic["enum"].ToString().Length > 0) && enums.ContainsKey(argdic["enum"].ToString()))
					{
						// Get the enum list
						this.enumlist = enums[argdic["enum"].ToString()];
					}
					else
					{
						General.ErrorLogger.Add(ErrorType.Warning, "\"" + argspath + ".arg" + istr + "\" references unknown enumeration \"" + argdic["enum"] + "\".");
					}
				}
			}

			//mxd. Determine flags type
			if(argdic.Contains("flags"))
			{
				// Enum fully specified?
				if(argdic["flags"] is IDictionary)
				{
					// Create anonymous enum
					this.flagslist = new EnumList((IDictionary)argdic["flags"]);
				}
				else
				{
					// Check if referenced enum exists
					if((argdic["flags"].ToString().Length > 0) && enums.ContainsKey(argdic["flags"].ToString()))
					{
						// Get the enum list
						this.flagslist = enums[argdic["flags"].ToString()];
					}
					else
					{
						General.ErrorLogger.Add(ErrorType.Warning, "\"" + argspath + ".arg" + istr + "\" references unknown flags enumeration \"" + argdic["flags"] + "\".");
					}
				}
			}
			
			if(this.enumlist == null) this.enumlist = new EnumList(); //mxd
			if(this.flagslist == null) this.flagslist = new EnumList(); //mxd
		}

		//mxd. Constructor for an argument info defined in DECORATE
        // [ZZ] Constructor for an argument info defined in DECORATE/ZScript. reworked.
        internal ArgumentInfo(ActorStructure actor, int i)
        {
            if(!actor.HasPropertyWithValue("$arg" + i) && !actor.HasPropertyWithValue("$arg" + i + "str"))
            {
                used = false;
                return;
            }

			string argtitle = ZDTextParser.StripQuotes(actor.GetPropertyAllValues("$arg" + i));
			string tooltip = ZDTextParser.StripQuotes(actor.GetPropertyAllValues("$arg" + i + "tooltip").Replace("\\n", Environment.NewLine));
			int type = actor.GetPropertyValueInt("$arg" + i + "type", 0);
			string targetclasses = ZDTextParser.StripQuotes(actor.GetPropertyAllValues("$arg" + i + "targetclasses"));
			int defaultvalue = actor.GetPropertyValueInt("$arg" + i + "default", 0);
			string enumstr = ZDTextParser.StripQuotes(actor.GetPropertyAllValues("$arg" + i + "enum"));
			string renderstyle = ZDTextParser.StripQuotes(actor.GetPropertyAllValues("$arg" + i + "renderstyle"));
			string rendercolor, minrange, maxrange, minrangecolor, maxrangecolor;
            bool str = (actor.HasProperty("$arg" + i + "str"));
            string argtitlestr = ZDTextParser.StripQuotes(actor.GetPropertyAllValues("$arg" + i + "str"));
            if (string.IsNullOrEmpty(argtitlestr)) argtitlestr = argtitle;
			if (!string.IsNullOrEmpty(renderstyle))
			{
                rendercolor = ZDTextParser.StripQuotes(actor.GetPropertyAllValues("$arg" + i + "rendercolor"));
				minrange = ZDTextParser.StripQuotes(actor.GetPropertyAllValues("$arg" + i + "minrange"));
				minrangecolor = ZDTextParser.StripQuotes(actor.GetPropertyAllValues("$arg" + i + "minrangecolor"));
				maxrange = ZDTextParser.StripQuotes(actor.GetPropertyAllValues("$arg" + i + "maxrange"));
				maxrangecolor = ZDTextParser.StripQuotes(actor.GetPropertyAllValues("$arg" + i + "maxrangecolor"));
			}
			else
			{
                rendercolor = string.Empty; minrange = string.Empty; maxrange = string.Empty; minrangecolor = string.Empty; maxrangecolor = string.Empty;
			}

			// Fallback to default titles if not specified
			if (string.IsNullOrEmpty(argtitle))
				argtitle = "Argument " + (i + 1);
			if (string.IsNullOrEmpty(argtitlestr))
				argtitlestr = "Argument " + (i + 1);

			string actorname = actor.ClassName;
            IDictionary<string, EnumList> enums = General.Map.Config.Enums;
            
			this.used = true;
			this.title = argtitle;
			this.tooltip = tooltip;
			this.defaultvalue = defaultvalue;
			this.flagslist = new EnumList(); //mxd
            this.str = str;
            this.titlestr = argtitlestr;

			// Get rendering hint settings
			switch(renderstyle.ToLowerInvariant())
			{
				case "circle": this.renderstyle = ArgumentRenderStyle.CIRCLE; break;
				case "rectangle": this.renderstyle = ArgumentRenderStyle.RECTANGLE; break;
				default:
					this.renderstyle = ArgumentRenderStyle.NONE;
					if(!string.IsNullOrEmpty(renderstyle))
						General.ErrorLogger.Add(ErrorType.Error, actorname + ": action argument \"" + argtitle + "\" has unknown renderstyle \"" + renderstyle + "\"!");
					break;
			}

			if(this.renderstyle != ArgumentRenderStyle.NONE)
			{
				// Get rendercolor
				this.rendercolor = General.Colors.InfoLine;

				if(!string.IsNullOrEmpty(rendercolor) && !ZDTextParser.GetColorFromString(rendercolor, out this.rendercolor))
					General.ErrorLogger.Add(ErrorType.Error, actorname + ": action argument \"" + argtitle + "\": unable to get rendercolor from value \"" + rendercolor + "\"!");

				this.rendercolor.a = HELPER_SHAPE_ALPHA;

				// Get minrange settings
				if(int.TryParse(minrange, NumberStyles.Integer, CultureInfo.InvariantCulture, out this.minrange) && this.minrange > 0f)
				{
					// Get minrangecolor
					this.minrangecolor = General.Colors.Indication;

					if(!string.IsNullOrEmpty(minrangecolor) && !ZDTextParser.GetColorFromString(minrangecolor, out this.minrangecolor))
						General.ErrorLogger.Add(ErrorType.Error, actorname + ": action argument \"" + this.title + "\": unable to get minrangecolor from value \"" + minrangecolor + "\"!");

					this.minrangecolor.a = RANGE_SHAPE_ALPHA;
				}

				// Get maxrange settings
				if(int.TryParse(maxrange, NumberStyles.Integer, CultureInfo.InvariantCulture, out this.maxrange) && this.maxrange > 0f)
				{
					// Get minrangecolor
					this.maxrangecolor = General.Colors.Indication;

					if(!string.IsNullOrEmpty(maxrangecolor) && !ZDTextParser.GetColorFromString(maxrangecolor, out this.maxrangecolor))
						General.ErrorLogger.Add(ErrorType.Error, actorname + ": action argument \"" + this.title + "\": unable to get maxrangecolor from value \"" + maxrangecolor + "\"!");

					this.maxrangecolor.a = RANGE_SHAPE_ALPHA;
				}

				// Update tooltip?
				if(this.minrange > 0f || this.maxrange > 0f)
				{
					if(!string.IsNullOrEmpty(this.tooltip)) this.tooltip += Environment.NewLine + Environment.NewLine;

					if(this.minrange > 0f && this.maxrange > 0f)
						this.tooltip += "Expected range: " + this.minrange + " - " + this.maxrange;
					else if(this.minrange > 0f)
						this.tooltip += "Minimum: " + this.minrange;
					else
						this.tooltip += "Maximum: " + this.maxrange;
				}
			}

			//Check for TargetClass
			this.targetclasses = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
			if(type == (int)UniversalType.ThingTag)
			{
				if(!string.IsNullOrEmpty(targetclasses))
				{
					foreach(string tclass in targetclasses.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)) 
						this.targetclasses.Add(tclass.Trim());
				}
			}

			// Get argument type
			if(System.Enum.IsDefined(typeof(UniversalType), type))
			{
				this.type = type;
			}
			else
			{
				General.ErrorLogger.Add(ErrorType.Error, actorname + ": action argument \"" + argtitle + "\" has unknown type " + type + "!");
				this.type = 0;
			}

			// Get or create enum
			if(!string.IsNullOrEmpty(enumstr))
			{
				if(enums.ContainsKey(enumstr.ToLowerInvariant()))
				{
					this.enumlist = enums[enumstr.ToLowerInvariant()];
				}
				else
				{
					Configuration cfg = new Configuration();
					if(cfg.InputConfiguration("enum" + enumstr, true)) 
					{
						IDictionary argdic = cfg.ReadSetting("enum", new Hashtable());
						if(argdic.Keys.Count > 0)
							this.enumlist = new EnumList(argdic);
						else
							General.ErrorLogger.Add(ErrorType.Error, actorname + ": unable to parse explicit enum structure for argument \"" + argtitle + "\"!");
					} 
					else 
					{
						General.ErrorLogger.Add(ErrorType.Error, actorname + ": unable to parse enum structure for argument \"" + argtitle + "\"!");
					}
				}
			}

			if(this.enumlist == null) this.enumlist = new EnumList();
		}

		// Constructor for unknown argument info
		internal ArgumentInfo(int argindex)
		{
			this.used = false;
			this.title = "Argument " + (argindex + 1);
			this.type = 0;
			this.enumlist = new EnumList();
			this.flagslist = new EnumList(); //mxd
			this.defaultvalue = 0; //mxd
		}

		#endregion

		#region ================== Methods

		// This gets the description for an argument value
		public string GetValueDescription(int value)
		{
			// TODO: Use the registered type editor to get the description!

			return value.ToString();
		}

		#endregion
	}
}
