
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
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.Types;

#endregion

namespace CodeImp.DoomBuilder.ZDoom
{
	public class ActorStructure
	{
		#region ================== Constants
		
		private readonly string[] SPRITE_CHECK_STATES = { "idle", "see", "inactive", "spawn" }; //mxd
		internal const string ACTOR_CLASS_SPECIAL_TOKENS = ":{}\n;,"; //mxd

        #endregion

        #region ================== Variables

        // Declaration
        internal string classname;
        internal string inheritclass;
        internal string replaceclass;
        internal int doomednum = -1;

        // Inheriting
        internal ActorStructure baseclass;
        internal bool skipsuper;

        // Flags
        internal Dictionary<string, bool> flags;

        // Properties
        internal Dictionary<string, List<string>> props;
        internal Dictionary<string, UniversalType> uservars; //mxd
        internal Dictionary<string, object> uservar_defaults; // [ZZ] should correspond to UniversalType

        //mxd. Categories
        internal DecorateCategoryInfo catinfo;

        // [ZZ] direct ArgumentInfos (from game configuration), or own ArgumentInfos (from props)
        internal ArgumentInfo[] args = new ArgumentInfo[5];

        // States
        internal Dictionary<string, StateStructure> states;
		
		#endregion
		
		#region ================== Properties
		
		public Dictionary<string, bool> Flags { get { return flags; } }
		public Dictionary<string, List<string>> Properties { get { return props; } }
		public string ClassName { get { return classname; } }
		public string InheritsClass { get { return inheritclass; } }
		public string ReplacesClass { get { return replaceclass; } }
		public ActorStructure BaseClass { get { return baseclass; } }
		internal int DoomEdNum { get { return doomednum; } set { doomednum = value; } }
		public Dictionary<string, UniversalType> UserVars { get { return uservars; } } //mxd
        public Dictionary<string, object> UserVarDefaults { get { return uservar_defaults; } } // [ZZ]
		internal DecorateCategoryInfo CategoryInfo { get { return catinfo; } } //mxd

		#endregion
		
		#region ================== Constructor / Disposer
		
		// Constructor
		internal ActorStructure()
		{
			// Initialize
			flags = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);
			props = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
			states = new Dictionary<string, StateStructure>(StringComparer.OrdinalIgnoreCase);
			uservars = new Dictionary<string, UniversalType>(StringComparer.OrdinalIgnoreCase);//mxd
            uservar_defaults = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);// [ZZ]
			
			// Always define a game property, but default to 0 values
			props["game"] = new List<string>();
			
			inheritclass = "actor";
			replaceclass = null;
			baseclass = null;
			skipsuper = false;
		}
		
		// Disposer
		public void Dispose()
		{
			baseclass = null;
			flags = null;
			props = null;
			states = null;
		}
		
		#endregion
		
		#region ================== Methods
		
		/// <summary>
		/// This checks if the actor has a specific property.
		/// </summary>
		public bool HasProperty(string propname)
		{
			if(props.ContainsKey(propname)) return true;
			if(!skipsuper && (baseclass != null)) return baseclass.HasProperty(propname);
			return false;
		}
		
		/// <summary>
		/// This checks if the actor has a specific property with at least one value.
		/// </summary>
		public bool HasPropertyWithValue(string propname)
		{
			if(props.ContainsKey(propname) && (props[propname].Count > 0)) return true;
			if(!skipsuper && (baseclass != null)) return baseclass.HasPropertyWithValue(propname);
			return false;
		}
		
		/// <summary>
		/// This returns values of a specific property as a complete string. Returns an empty string when the propery has no values.
		/// </summary>
		public string GetPropertyAllValues(string propname)
		{
			if(props.ContainsKey(propname) && (props[propname].Count > 0))
				return string.Join(" ", props[propname].ToArray());
			if(!skipsuper && (baseclass != null))
				return baseclass.GetPropertyAllValues(propname);
			return "";
		}
		
		/// <summary>
		/// This returns a specific value of a specific property as a string. Returns an empty string when the propery does not have the specified value.
		/// </summary>
		public string GetPropertyValueString(string propname, int valueindex) { return GetPropertyValueString(propname, valueindex, true); } //mxd. Added "stripquotes" parameter
		public string GetPropertyValueString(string propname, int valueindex, bool stripquotes)
		{
			if(props.ContainsKey(propname) && (props[propname].Count > valueindex))
				return (stripquotes ? ZDTextParser.StripQuotes(props[propname][valueindex]) : props[propname][valueindex]);
			if(!skipsuper && (baseclass != null))
				return baseclass.GetPropertyValueString(propname, valueindex, stripquotes);
			return "";
		}
		
		/// <summary>
		/// This returns a specific value of a specific property as an integer. Returns 0 when the propery does not have the specified value.
		/// </summary>
		public int GetPropertyValueInt(string propname, int valueindex)
		{
			string str = GetPropertyValueString(propname, valueindex, false);

			//mxd. It can be negative...
			if(str == "-" && props.Count > valueindex + 1)
				str += GetPropertyValueString(propname, valueindex + 1, false);
			
			int intvalue;
			if(int.TryParse(str, NumberStyles.Integer, CultureInfo.InvariantCulture, out intvalue))
				return intvalue;
			return 0;
		}
		
		/// <summary>
		/// This returns a specific value of a specific property as a float. Returns 0.0f when the propery does not have the specified value.
		/// </summary>
		public float GetPropertyValueFloat(string propname, int valueindex)
		{
			string str = GetPropertyValueString(propname, valueindex, false);

			//mxd. It can be negative...
			if(str == "-" && props.Count > valueindex + 1)
				str += GetPropertyValueString(propname, valueindex + 1, false);

			float fvalue;
			if(float.TryParse(str, NumberStyles.Float, CultureInfo.InvariantCulture, out fvalue))
				return fvalue;
			return 0.0f;
		}

		/// <summary>
		/// Gets all user vars from this actor and all actors in the inheritance chain.
		/// </summary>
		/// <returns>Dictionary of all user vars</returns>
		public Dictionary<string, UniversalType> GetAllUserVars()
		{
			Dictionary<string, UniversalType> result = new Dictionary<string, UniversalType>(uservars);
			ActorStructure parent = baseclass;

			while(parent != null)
			{
				foreach(string key in parent.UserVars.Keys)
					if (!result.ContainsKey(key))
						result[key] = parent.UserVars[key];

				parent = parent.baseclass;
			}

			return result;
		}

		/// <summary>
		/// Gets all user var defauls from this actor and all actors in the inheritance chain.
		/// </summary>
		/// <returns>Dictionary of all user var defaults</returns>
		public Dictionary<string, object> GetAllUserVarDefaults()
		{
			Dictionary<string, object> result = new Dictionary<string, object>(uservar_defaults);
			ActorStructure parent = baseclass;

			while (parent != null)
			{
				foreach (string key in parent.UserVarDefaults.Keys)
					if (!result.ContainsKey(key))
						result[key] = parent.UserVarDefaults[key];

				parent = parent.baseclass;
			}

			return result;
		}

		/// <summary>
		/// This returns the status of a flag.
		/// </summary>
		public bool HasFlagValue(string flag)
		{
			if(flags.ContainsKey(flag)) return true;
			if(!skipsuper && (baseclass != null)) return baseclass.HasFlagValue(flag);
			return false;
		}
		
		/// <summary>
		/// This returns the status of a flag.
		/// </summary>
		public bool GetFlagValue(string flag, bool defaultvalue)
		{
			if(flags.ContainsKey(flag)) return flags[flag];
			if(!skipsuper && (baseclass != null)) return baseclass.GetFlagValue(flag, defaultvalue);
			return defaultvalue;
		}
		
		/// <summary>
		/// This checks if a state has been defined.
		/// </summary>
		public bool HasState(string statename)
		{
            // [ZZ] we should NOT pick up any states from Actor. (except Spawn, as the very default state)
            //      for the greater good. (otherwise POL5 from GenericCrush is displayed for actors without frames, preferred before TNT1)
            if (classname.ToLowerInvariant() == "actor" && statename.ToLowerInvariant() != "spawn")
                return false;

			if(states.ContainsKey(statename)) return true;
			if(!skipsuper && (baseclass != null)) return baseclass.HasState(statename);
			return false;
		}
		
		/// <summary>
		/// This returns a specific state, or null when the state can't be found.
		/// </summary>
		public StateStructure GetState(string statename)
		{
            // [ZZ] we should NOT pick up any states from Actor. (except Spawn, as the very default state)
            //      for the greater good. (otherwise POL5 from GenericCrush is displayed for actors without frames, preferred before TNT1)
            if (classname.ToLowerInvariant() == "actor" && statename.ToLowerInvariant() != "spawn")
                return null;

            if (states.ContainsKey(statename)) return states[statename];
			if(!skipsuper && (baseclass != null)) return baseclass.GetState(statename);
			return null;
		}
		
		/// <summary>
		/// This creates a list of all states, also those inherited from the base class.
		/// </summary>
		public Dictionary<string, StateStructure> GetAllStates()
		{
            // [ZZ] we should NOT pick up any states from Actor. (except Spawn, as the very default state)
            //      for the greater good. (otherwise POL5 from GenericCrush is displayed for actors without frames, preferred before TNT1)
            if (classname.ToLowerInvariant() == "actor")
            {
                Dictionary<string, StateStructure> list2 = new Dictionary<string, StateStructure>(StringComparer.OrdinalIgnoreCase);
                if (states.ContainsKey("spawn"))
                    list2.Add("spawn", states["spawn"]);
                return list2;
            }

            Dictionary<string, StateStructure> list = new Dictionary<string, StateStructure>(states, StringComparer.OrdinalIgnoreCase);

            if (!skipsuper && (baseclass != null))
			{
				Dictionary<string, StateStructure> baselist = baseclass.GetAllStates();
				foreach(KeyValuePair<string, StateStructure> s in baselist)
					if(!list.ContainsKey(s.Key)) list.Add(s.Key, s.Value);
			}
			
			return list;
		}
		
		/// <summary>
		/// This checks if this actor is meant for the current decorate game support
		/// </summary>
		public bool CheckActorSupported()
		{
			// Only check if a map is opened. Otherwise we run into problems with the resource checker
			if (General.Map != null)
			{
				// Check if we want to include this actor
				string includegames = General.Map.Config.DecorateGames.ToLowerInvariant();
				bool includeactor = (props["game"].Count == 0);
				foreach (string g in props["game"])
					includeactor |= includegames.Contains(g);

				return includeactor;
			}
			else
			{
				return true;
			}
		}
		
		/// <summary>
		/// This finds the best suitable sprite to use when presenting this actor to the user.
		/// </summary>
		public StateStructure.FrameInfo FindSuitableSprite()
		{
			// Info: actual sprites are resolved in ThingTypeInfo.SetupSpriteFrame() - mxd
			// Sprite forced?
			if(HasPropertyWithValue("$sprite"))
			{
				string sprite = GetPropertyValueString("$sprite", 0, true); //mxd

				//mxd. Valid when internal or exists
				if(sprite.StartsWith(DataManager.INTERNAL_PREFIX, StringComparison.OrdinalIgnoreCase) || General.Map.Data.GetSpriteExists(sprite))
					return new StateStructure.FrameInfo { Sprite = sprite };

				//mxd. Bitch and moan
				General.ErrorLogger.Add(ErrorType.Warning, "DECORATE warning in " + classname + ":" + doomednum + ". The sprite \"" + sprite + "\" assigned by the \"$sprite\" property does not exist.");
			}

            StateStructure.FrameInfo firstNonTntInfo = null;
            StateStructure.FrameInfo firstInfo = null;
            // Pick the first we can find (including and not including TNT1)
            Dictionary<string, StateStructure> list = GetAllStates();
            foreach (StateStructure s in list.Values)
            {
                StateStructure.FrameInfo info = s.GetSprite(0);
                if (string.IsNullOrEmpty(info.Sprite)) continue;

                if (!info.IsEmpty()) firstNonTntInfo = info;
                if (firstInfo == null) firstInfo = info;

                if (firstNonTntInfo != null)
                    break;
            }

            //mxd. Try to get a suitable sprite from our hardcoded states list
            StateStructure.FrameInfo lastNonTntInfo = null;
            StateStructure.FrameInfo lastInfo = null;
            foreach (string state in SPRITE_CHECK_STATES)
			{
				if(!HasState(state)) continue;

				StateStructure s = GetState(state);
				StateStructure.FrameInfo info = s.GetSprite(0);
                //if(!string.IsNullOrEmpty(info.Sprite)) return info;
                if (string.IsNullOrEmpty(info.Sprite)) continue;

                if (!info.IsEmpty()) lastNonTntInfo = info;
                if (lastInfo == null) lastInfo = info;

                if (lastNonTntInfo != null)
                    break;
			}

            // [ZZ] return whatever is there by priority. try to pick non-TNT1 frames.
            StateStructure.FrameInfo[] infos = new StateStructure.FrameInfo[] { lastNonTntInfo, lastInfo, firstNonTntInfo, firstInfo };
            foreach (StateStructure.FrameInfo info in infos)
                if (info != null) return info;
			
			//mxd. No dice...
			return null;
		}

        /// <summary>
        /// This method parses $argN into argumentinfos.
        /// </summary>
        public void ParseCustomArguments()
        {
            for (int i = 0; i < 5; i++)
            {
                if (HasProperty("$arg" + i) || (i == 0 && HasProperty("$arg" + i + "str")))
                    args[i] = new ArgumentInfo(this, i);
                else args[i] = null;
            }
        }

        public ArgumentInfo GetArgumentInfo(int idx)
        {
            if (args[idx] != null)
                return args[idx];
            // if we have $clearargs, don't inherit anything!
            if (props.ContainsKey("$clearargs"))
                return null;
            if (baseclass != null)
                return baseclass.GetArgumentInfo(idx);
            return null;
        }
		
		#endregion
	}
}
