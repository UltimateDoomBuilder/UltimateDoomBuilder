
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
using CodeImp.DoomBuilder.Data;
using System.IO;
using CodeImp.DoomBuilder.Editing;
using System.Collections.Specialized;
using CodeImp.DoomBuilder.GZBuilder.Data;
using CodeImp.DoomBuilder.Rendering;

#endregion

namespace CodeImp.DoomBuilder.Config
{
	public class ConfigurationInfo : IComparable<ConfigurationInfo>
	{
		#region ================== Constants

		private const string MODE_DISABLED_KEY = "disabled";
		private const string MODE_ENABLED_KEY = "enabled";

		// The { and } are invalid key names in a configuration so this ensures this string is unique
		private const string MISSING_NODEBUILDER = "{missing nodebuilder}";
		private readonly string[] LINEDEF_COLOR_PRESET_FLAGS_SEPARATOR = new[] { "^" }; //mxd

		#endregion
		
		#region ================== Variables

		private string name;
		private string filename;
		private string settingskey;
		private string defaultlumpname;
		private string nodebuildersave;
		private string nodebuildertest;
		private string formatinterface; //mxd
		private readonly string defaultscriptcompiler; //mxd
		private DataLocationList resources;
		private Configuration config; //mxd
		private bool enabled; //mxd
		private bool changed; //mxd

		private List<EngineInfo> testEngines; //mxd
		private int currentEngineIndex; //mxd
		private LinedefColorPreset[] linedefColorPresets; //mxd

		private List<ThingsFilter> thingsfilters;
		private List<DefinedTextureSet> texturesets;
		private Dictionary<string, bool> editmodes;
		private string startmode;
		
		#endregion

		#region ================== Properties

		public string Name { get { return name; } }
		public string Filename { get { return filename; } }
		public string DefaultLumpName { get { return defaultlumpname; } }
		public string NodebuilderSave { get { return nodebuildersave; } internal set { nodebuildersave = value; } }
		public string NodebuilderTest { get { return nodebuildertest; } internal set { nodebuildertest = value; } }
		public string FormatInterface { get { return formatinterface; } } //mxd
		public string DefaultScriptCompiler { get { return defaultscriptcompiler; } } //mxd
		internal DataLocationList Resources { get { return resources; } }
		internal Configuration Configuration { get { return config; } } //mxd
		public bool Enabled { get { return enabled; } internal set { enabled = value; } } //mxd
		public bool Changed { get { return changed; } internal set { changed = value; } } //mxd

		//mxd
		public string TestProgramName { get { return testEngines[currentEngineIndex].TestProgramName; } internal set { testEngines[currentEngineIndex].TestProgramName = value; } }
		public string TestProgram { get { return testEngines[currentEngineIndex].TestProgram; } internal set { testEngines[currentEngineIndex].TestProgram = value; } }
		public string TestParameters { get { return testEngines[currentEngineIndex].TestParameters; } internal set { testEngines[currentEngineIndex].TestParameters = value; } }
		public bool TestShortPaths { get { return testEngines[currentEngineIndex].TestShortPaths; } internal set { testEngines[currentEngineIndex].TestShortPaths = value; } }
		public bool TestLinuxPaths { get { return testEngines[currentEngineIndex].TestLinuxPaths; } internal set { testEngines[currentEngineIndex].TestLinuxPaths = value; } }
		public int TestSkill { get { return testEngines[currentEngineIndex].TestSkill; } internal set { testEngines[currentEngineIndex].TestSkill = value; } }
		public string TestAdditionalParameters { get { return testEngines[currentEngineIndex].AdditionalParameters; } internal set { testEngines[currentEngineIndex].AdditionalParameters = value; } }
		public bool CustomParameters { get { return testEngines[currentEngineIndex].CustomParameters; } internal set { testEngines[currentEngineIndex].CustomParameters = value; } }
		public List<EngineInfo> TestEngines { get { return testEngines; } internal set { testEngines = value; } }
		public int CurrentEngineIndex { get { return currentEngineIndex; } internal set { currentEngineIndex = value; } }
		public LinedefColorPreset[] LinedefColorPresets { get { return linedefColorPresets; } internal set { linedefColorPresets = value; } }

		internal ICollection<ThingsFilter> ThingsFilters { get { return thingsfilters; } }
		internal List<DefinedTextureSet> TextureSets { get { return texturesets; } }
		internal Dictionary<string, bool> EditModes { get { return editmodes; } }
		public string StartMode { get { return startmode; } internal set { startmode = value; } }
		
		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		internal ConfigurationInfo(Configuration cfg, string filename)
		{
			// Initialize
			this.filename = filename;
			this.config = cfg; //mxd
			this.settingskey = Path.GetFileNameWithoutExtension(filename).ToLower();
			
			// Load settings from game configuration
			this.name = config.ReadSetting("game", "<unnamed game>");
			this.defaultlumpname = config.ReadSetting("defaultlumpname", "");
			
			// Load settings from program configuration
			this.nodebuildersave = General.Settings.ReadSetting("configurations." + settingskey + ".nodebuildersave", MISSING_NODEBUILDER);
			this.nodebuildertest = General.Settings.ReadSetting("configurations." + settingskey + ".nodebuildertest", MISSING_NODEBUILDER);
			this.formatinterface = config.ReadSetting("formatinterface", "").ToLowerInvariant(); //mxd
			this.defaultscriptcompiler = cfg.ReadSetting("defaultscriptcompiler", ""); //mxd
			this.resources = new DataLocationList(General.Settings.Config, "configurations." + settingskey + ".resources");
			this.startmode = General.Settings.ReadSetting("configurations." + settingskey + ".startmode", "VerticesMode");
			this.enabled = General.Settings.ReadSetting("configurations." + settingskey + ".enabled", config.ReadSetting("enabledbydefault", false)); //mxd
			
			//mxd. Read test engines
			testEngines = new List<EngineInfo>();
			IDictionary list = General.Settings.ReadSetting("configurations." + settingskey + ".engines", new ListDictionary());
			currentEngineIndex = Math.Max(0, General.Settings.ReadSetting("configurations." + settingskey + ".currentengineindex", 0));
			
			// No engine list found? Use old engine properties
			if(list.Count == 0) 
			{
				EngineInfo info = new EngineInfo();
				info.TestProgram = General.Settings.ReadSetting("configurations." + settingskey + ".testprogram", "");
				info.TestProgramName = General.Settings.ReadSetting("configurations." + settingskey + ".testprogramname", EngineInfo.DEFAULT_ENGINE_NAME);
				info.TestParameters = General.Settings.ReadSetting("configurations." + settingskey + ".testparameters", "");
				info.TestShortPaths = General.Settings.ReadSetting("configurations." + settingskey + ".testshortpaths", false);
				info.TestLinuxPaths = General.Settings.ReadSetting("configurations." + settingskey + ".testlinuxpaths", false);
				info.CustomParameters = General.Settings.ReadSetting("configurations." + settingskey + ".customparameters", false);
				info.TestSkill = General.Settings.ReadSetting("configurations." + settingskey + ".testskill", 3);
				info.AdditionalParameters = General.Settings.ReadSetting("configurations." + settingskey + ".additionalparameters", "");
				testEngines.Add(info);
				currentEngineIndex = 0;
			} 
			else 
			{
				//read engines settings from config
				foreach(DictionaryEntry de in list) 
				{
					string path = "configurations." + settingskey + ".engines." + de.Key;
					EngineInfo info = new EngineInfo();
					info.TestProgram = General.Settings.ReadSetting(path + ".testprogram", "");
					info.TestProgramName = General.Settings.ReadSetting(path + ".testprogramname", EngineInfo.DEFAULT_ENGINE_NAME);
					info.TestParameters = General.Settings.ReadSetting(path + ".testparameters", "");
					info.TestShortPaths = General.Settings.ReadSetting(path + ".testshortpaths", false);
					info.TestLinuxPaths = General.Settings.ReadSetting(path + ".testlinuxpaths", false);
					info.CustomParameters = General.Settings.ReadSetting(path + ".customparameters", false);
					info.TestSkill = General.Settings.ReadSetting(path + ".testskill", 3);
					info.AdditionalParameters = General.Settings.ReadSetting(path + ".additionalparameters", "");
					testEngines.Add(info);
				}

				if(currentEngineIndex >= testEngines.Count)	currentEngineIndex = 0;
			}

			//mxd. read custom linedef colors 
			List<LinedefColorPreset> colorPresets = new List<LinedefColorPreset>();
			list = General.Settings.ReadSetting("configurations." + settingskey + ".linedefcolorpresets", new ListDictionary());

			//no presets? add "classic" ones then.
			if(list.Count == 0) 
			{
				colorPresets.Add(new LinedefColorPreset("Any action", PixelColor.FromColor(System.Drawing.Color.PaleGreen), -1, 0, new List<string>(), new List<string>(), true));
			} 
			else 
			{
				//read custom linedef colors from config
				foreach(DictionaryEntry de in list) 
				{
					string path = "configurations." + settingskey + ".linedefcolorpresets." + de.Key;
					string presetname = General.Settings.ReadSetting(path + ".name", "Unnamed");
					bool presetenabled = General.Settings.ReadSetting(path + ".enabled", true);
					PixelColor color = PixelColor.FromInt(General.Settings.ReadSetting(path + ".color", -1));
					int action = General.Settings.ReadSetting(path + ".action", 0);
					int activation = General.Settings.ReadSetting(path + ".activation", 0);
					List<string> flags = new List<string>();
					flags.AddRange(General.Settings.ReadSetting(path + ".flags", "").Split(LINEDEF_COLOR_PRESET_FLAGS_SEPARATOR, StringSplitOptions.RemoveEmptyEntries));
					List<string> restrictedFlags = new List<string>();
					restrictedFlags.AddRange(General.Settings.ReadSetting(path + ".restrictedflags", "").Split(LINEDEF_COLOR_PRESET_FLAGS_SEPARATOR, StringSplitOptions.RemoveEmptyEntries));
					LinedefColorPreset preset = new LinedefColorPreset(presetname, color, action, activation, flags, restrictedFlags, presetenabled);
					colorPresets.Add(preset);
				}
			}
			linedefColorPresets = colorPresets.ToArray();

			// Make list of things filters
			thingsfilters = new List<ThingsFilter>();
			IDictionary cfgfilters = General.Settings.ReadSetting("configurations." + settingskey + ".thingsfilters", new Hashtable());
			foreach(DictionaryEntry de in cfgfilters)
			{
				thingsfilters.Add(new ThingsFilter(General.Settings.Config, "configurations." + settingskey + ".thingsfilters." + de.Key));
			}

			// Make list of texture sets
			texturesets = new List<DefinedTextureSet>();
			IDictionary sets = General.Settings.ReadSetting("configurations." + settingskey + ".texturesets", new Hashtable());
			foreach(DictionaryEntry de in sets)
			{
				texturesets.Add(new DefinedTextureSet(General.Settings.Config, "configurations." + settingskey + ".texturesets." + de.Key));
			}
			
			// Make list of edit modes
			this.editmodes = new Dictionary<string, bool>(StringComparer.Ordinal);
			IDictionary modes = General.Settings.ReadSetting("configurations." + settingskey + ".editmodes", new Hashtable());
			foreach(DictionaryEntry de in modes)
			{
				if(de.Key.ToString().StartsWith(MODE_ENABLED_KEY))
					editmodes.Add(de.Value.ToString(), true);
				else if(de.Key.ToString().StartsWith(MODE_DISABLED_KEY))
					editmodes.Add(de.Value.ToString(), false);
			}
		}
		
		// Constructor
		private ConfigurationInfo()
		{
		}

		//mxd. Destructor
		~ConfigurationInfo()
		{
			// biwa. There have been crash reports because of null references
			// https://github.com/jewalky/UltimateDoomBuilder/issues/251
			// https://github.com/jewalky/UltimateDoomBuilder/issues/352
			// https://github.com/jewalky/UltimateDoomBuilder/issues/514
			// Can't reproduce, but add a safeguard anyway.
			if (thingsfilters != null) foreach(ThingsFilter tf in thingsfilters) if(tf != null) tf.Dispose();
			if (testEngines != null) foreach(EngineInfo ei in testEngines) if(ei != null) ei.Dispose();
		}
		
		#endregion

		#region ================== Methods

		/// <summary>
		/// This returns the resource locations as configured.
		/// </summary>
		public DataLocationList GetResources()
		{
			return new DataLocationList(resources);
		}

		// This compares it to other ConfigurationInfo objects
		public int CompareTo(ConfigurationInfo other)
		{
			// Compare
			return String.Compare(name, other.name, StringComparison.Ordinal);
		}

		// This saves the settings to program configuration
		internal void SaveSettings()
		{
			//mxd
			General.Settings.WriteSetting("configurations." + settingskey + ".enabled", enabled);
			if(!changed) return;
			
			// Write to configuration
			General.Settings.WriteSetting("configurations." + settingskey + ".nodebuildersave", nodebuildersave);
			General.Settings.WriteSetting("configurations." + settingskey + ".nodebuildertest", nodebuildertest);
			
			//mxd. Test Engines
			General.Settings.WriteSetting("configurations." + settingskey + ".currentengineindex", currentEngineIndex);
			SaveTestEngines();

			//mxd. Custom linedef colors
			SaveLinedefColorPresets();

			General.Settings.WriteSetting("configurations." + settingskey + ".startmode", startmode);
			resources.WriteToConfig(General.Settings.Config, "configurations." + settingskey + ".resources");
			
			// Write filters to configuration
			General.Settings.DeleteSetting("configurations." + settingskey + ".thingsfilters");
			for(int i = 0; i < thingsfilters.Count; i++)
			{
				thingsfilters[i].WriteSettings(General.Settings.Config,
					"configurations." + settingskey + ".thingsfilters.filter" + i.ToString(CultureInfo.InvariantCulture));
			}

			// Write texturesets to configuration
			General.Settings.DeleteSetting("configurations." + settingskey + ".texturesets"); //mxd
			for(int i = 0; i < texturesets.Count; i++)
			{
				texturesets[i].WriteToConfig(General.Settings.Config,
					"configurations." + settingskey + ".texturesets.set" + i.ToString(CultureInfo.InvariantCulture));
			}
			
			// Write edit modes to configuration
			ListDictionary modeslist = new ListDictionary();
			int index = 0;
			foreach(KeyValuePair<string, bool> em in editmodes)
			{
				if(em.Value)
					modeslist.Add(MODE_ENABLED_KEY + index.ToString(CultureInfo.InvariantCulture), em.Key);
				else
					modeslist.Add(MODE_DISABLED_KEY + index.ToString(CultureInfo.InvariantCulture), em.Key);

				index++;
			}
			General.Settings.WriteSetting("configurations." + settingskey + ".editmodes", modeslist);
		}

		//mxd
		private void SaveTestEngines() 
		{
			// Fill structure
			IDictionary resinfo = new ListDictionary();

			for(int i = 0; i < testEngines.Count; i++) 
			{
				IDictionary rlinfo = new ListDictionary();
				rlinfo.Add("testprogramname", testEngines[i].TestProgramName);
				rlinfo.Add("testprogram", testEngines[i].TestProgram);
				rlinfo.Add("testparameters", testEngines[i].TestParameters);
				rlinfo.Add("testshortpaths", testEngines[i].TestShortPaths);
				rlinfo.Add("testlinuxpaths", testEngines[i].TestLinuxPaths);
				rlinfo.Add("customparameters", testEngines[i].CustomParameters);
				rlinfo.Add("testskill", testEngines[i].TestSkill);
				rlinfo.Add("additionalparameters", testEngines[i].AdditionalParameters);

				// Add structure
				resinfo.Add("engine" + i.ToString(CultureInfo.InvariantCulture), rlinfo);
			}

			// Write to config
			General.Settings.Config.WriteSetting("configurations." + settingskey + ".engines", resinfo);
		}

		//mxd
		private void SaveLinedefColorPresets() 
		{
			// Fill structure
			IDictionary resinfo = new ListDictionary();

			for(int i = 0; i < linedefColorPresets.Length; i++) 
			{
				IDictionary rlinfo = new ListDictionary();
				rlinfo.Add("name", linedefColorPresets[i].Name);
				rlinfo.Add("enabled", linedefColorPresets[i].Enabled);
				rlinfo.Add("color", linedefColorPresets[i].Color.ToInt());
				rlinfo.Add("action", linedefColorPresets[i].Action);
				rlinfo.Add("activation", linedefColorPresets[i].Activation);
				rlinfo.Add("flags", string.Join(LINEDEF_COLOR_PRESET_FLAGS_SEPARATOR[0], linedefColorPresets[i].Flags.ToArray()));
				rlinfo.Add("restrictedflags", string.Join(LINEDEF_COLOR_PRESET_FLAGS_SEPARATOR[0], linedefColorPresets[i].RestrictedFlags.ToArray()));

				// Add structure
				resinfo.Add("preset" + i.ToString(CultureInfo.InvariantCulture), rlinfo);
			}

			// Write to config
			General.Settings.Config.WriteSetting("configurations." + settingskey + ".linedefcolorpresets", resinfo);
		}


		// String representation
		public override string ToString()
		{
			return name;
		}

		// This clones the object
		internal ConfigurationInfo Clone()
		{
			ConfigurationInfo ci = new ConfigurationInfo();
			ci.name = this.name;
			ci.filename = this.filename;
			ci.settingskey = this.settingskey;
			ci.nodebuildersave = this.nodebuildersave;
			ci.nodebuildertest = this.nodebuildertest;
			ci.formatinterface = this.formatinterface; //mxd
			ci.resources = new DataLocationList(this.resources);
			
			//mxd
			ci.testEngines = new List<EngineInfo>();
			foreach(EngineInfo info in testEngines) ci.testEngines.Add(new EngineInfo(info));
			ci.currentEngineIndex = this.currentEngineIndex;
			ci.linedefColorPresets = new LinedefColorPreset[linedefColorPresets.Length];
			for(int i = 0; i < linedefColorPresets.Length; i++)
				ci.linedefColorPresets[i] = new LinedefColorPreset(linedefColorPresets[i]);

			ci.startmode = this.startmode;
			ci.config = this.config; //mxd
			ci.enabled = this.enabled; //mxd
			ci.changed = this.changed; //mxd
			ci.texturesets = new List<DefinedTextureSet>();
			foreach(DefinedTextureSet s in this.texturesets) ci.texturesets.Add(s.Copy());
			ci.thingsfilters = new List<ThingsFilter>();
			foreach(ThingsFilter f in this.thingsfilters) ci.thingsfilters.Add(new ThingsFilter(f));
			ci.editmodes = new Dictionary<string, bool>(this.editmodes);
			return ci;
		}
		
		// This applies settings from an object
		internal void Apply(ConfigurationInfo ci)
		{
			this.name = ci.name;
			this.filename = ci.filename;
			this.settingskey = ci.settingskey;
			this.nodebuildersave = ci.nodebuildersave;
			this.nodebuildertest = ci.nodebuildertest;
			this.formatinterface = ci.formatinterface; //mxd
			this.currentEngineIndex = ci.currentEngineIndex; //mxd
			this.resources = new DataLocationList(ci.resources);
			
			//mxd
			this.testEngines = new List<EngineInfo>();
			foreach(EngineInfo info in ci.testEngines) testEngines.Add(new EngineInfo(info));
			if(this.currentEngineIndex >= testEngines.Count) this.currentEngineIndex = Math.Max(0, testEngines.Count - 1);
			this.linedefColorPresets = new LinedefColorPreset[ci.linedefColorPresets.Length];
			for(int i = 0; i < ci.linedefColorPresets.Length; i++)
				this.linedefColorPresets[i] = new LinedefColorPreset(ci.linedefColorPresets[i]);

			this.startmode = ci.startmode;
			this.config = ci.config; //mxd
			this.enabled = ci.enabled; //mxd
			this.changed = ci.changed;
			this.texturesets = new List<DefinedTextureSet>();
			foreach(DefinedTextureSet s in ci.texturesets) this.texturesets.Add(s.Copy());
			this.thingsfilters = new List<ThingsFilter>();
			foreach(ThingsFilter f in ci.thingsfilters) this.thingsfilters.Add(new ThingsFilter(f));
			this.editmodes = new Dictionary<string, bool>(ci.editmodes);
		}
		
		// This applies the defaults
		internal void ApplyDefaults(GameConfiguration gameconfig)
		{
			// Some of the defaults can only be applied from game configuration
			if(gameconfig != null)
			{
				// No nodebuildes set?
				if(nodebuildersave == MISSING_NODEBUILDER) nodebuildersave = gameconfig.DefaultSaveCompiler;
				if(nodebuildertest == MISSING_NODEBUILDER) nodebuildertest = gameconfig.DefaultTestCompiler;
				
				// No texture sets?
				if(texturesets.Count == 0)
				{
					// Copy the default texture sets from the game configuration
					foreach(DefinedTextureSet s in gameconfig.TextureSets)
					{
						// Add a copy to our list
						texturesets.Add(s.Copy());
					}
				}
				
				// No things filters?
				if(thingsfilters.Count == 0)
				{
					// Copy the things filters from game configuration
					foreach(ThingsFilter f in gameconfig.ThingsFilters)
						thingsfilters.Add(new ThingsFilter(f));
				}

				//mxd. Validate filters. Do it only for currently used ConfigInfo
				if(General.Map != null && General.Map.ConfigSettings == this)
				{
					foreach(ThingsFilter f in thingsfilters) f.Validate();
				}
			}
			
			// Go for all available editing modes
			foreach(EditModeInfo info in General.Editing.ModesInfo)
			{
				// Is this a mode that is optional?
				if(info.IsOptional)
				{
					// Add if not listed yet
					if(!editmodes.ContainsKey(info.Type.FullName))
						editmodes.Add(info.Type.FullName, info.Attributes.UseByDefault);
				}
			}
		}

		//mxd
		internal void PasteResourcesFrom(ConfigurationInfo source) 
		{
			resources = new DataLocationList(source.resources);
			changed = true;
		}

		//mxd
		internal void PasteTestEnginesFrom(ConfigurationInfo source) 
		{
			currentEngineIndex = source.currentEngineIndex;
			testEngines = new List<EngineInfo>();
			foreach(EngineInfo info in source.testEngines) testEngines.Add(new EngineInfo(info));
			if(currentEngineIndex >= testEngines.Count) currentEngineIndex = Math.Max(0, testEngines.Count - 1);
			changed = true;
		}

		//mxd
		internal void PasteColorPresetsFrom(ConfigurationInfo source) 
		{
			linedefColorPresets = new LinedefColorPreset[source.linedefColorPresets.Length];
			for(int i = 0; i < source.linedefColorPresets.Length; i++)
				linedefColorPresets[i] = new LinedefColorPreset(source.linedefColorPresets[i]);
			changed = true;
		}

		//mxd. Not all properties should be pasted
		internal void PasteFrom(ConfigurationInfo source) 
		{
			nodebuildersave = source.nodebuildersave;
			nodebuildertest = source.nodebuildertest;
			currentEngineIndex = source.currentEngineIndex;
			resources = new DataLocationList(source.resources);

			testEngines = new List<EngineInfo>();
			foreach(EngineInfo info in source.testEngines)
				testEngines.Add(new EngineInfo(info)); 
			if(currentEngineIndex >= testEngines.Count) currentEngineIndex = Math.Max(0, testEngines.Count - 1);
			linedefColorPresets = new LinedefColorPreset[source.linedefColorPresets.Length];
			for(int i = 0; i < source.linedefColorPresets.Length; i++)
				linedefColorPresets[i] = new LinedefColorPreset(source.linedefColorPresets[i]);

			startmode = source.startmode;
			changed = true;
			texturesets = new List<DefinedTextureSet>();
			foreach(DefinedTextureSet s in source.texturesets) texturesets.Add(s.Copy());
			thingsfilters = new List<ThingsFilter>();
			foreach(ThingsFilter f in source.thingsfilters) thingsfilters.Add(new ThingsFilter(f));
			editmodes = new Dictionary<string, bool>(source.editmodes);
		}

		//mxd. This checks if given map name can cause problems
		internal bool ValidateMapName(string name)
		{
			// Get the map lump names
			IDictionary maplumpnames = config.ReadSetting("maplumpnames", new Hashtable());

			// Check if given map name overlaps with maplumpnames defined for this game configuration
			foreach(DictionaryEntry ml in maplumpnames) 
			{
				// Ignore the map header (it will not be found because the name is different)
				string lumpname = ml.Key.ToString().ToUpperInvariant();
				if(lumpname.Contains(name)) return false;
			}

			return true;
		}
		
		#endregion
	}
}
