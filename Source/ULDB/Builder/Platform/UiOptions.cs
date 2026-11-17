// ULDB: options that only ULDB has (Preferences, "ULDB" card on the Interface tab), stored in the program
// configuration under "uldb". Each answers a request upstream left open: texture browser background (#1216),
// thing rotation step (#1191), prefab tag renumbering (#1335), interface scale (#1146).

using System;
using System.IO;
using System.Text.RegularExpressions;
using CodeImp.DoomBuilder;
using CodeImp.DoomBuilder.Config;

namespace ULDB.Platform
{
	public static class UiOptions
	{
		public const string BROWSER_BACKGROUND = "uldb.browserbackground";
		public const string THING_ROTATION = "uldb.thingrotationstep";
		public const string PREFAB_TAGS = "uldb.prefabtags";
		public const string UI_SCALE_KEY = "uldb.uiscalepercent";
		public const string SCRIPT_ZOOM = "uldb.scriptzoom";
		public const string SCRIPT_ITALIC_COMMENTS = "uldb.scriptitaliccomments";
		private const string UI_SCALE = "uiscalepercent"; // its name in the file, where ScaleFromFile finds it before the UI exists

		/// <summary>Degrees a Things-mode rotate action (mouse wheel) turns by; 5 by default.</summary>
		public static int ThingRotationStep
		{
			get
			{
				int step = General.Settings != null ? General.Settings.ReadSetting(THING_ROTATION, 5) : 5;
				return step >= 1 && step <= 90 ? step : 5;
			}
		}

		/// <summary>Raised when the script editor's zoom or comment style changed, so every open editor follows.</summary>
		public static event Action ScriptViewChanged;

		/// <summary>The script editor's zoom (points added to the font size of the settings; Ctrl+Plus/Minus), kept between sessions.</summary>
		public static int ScriptZoom
		{
			get
			{
				int zoom = General.Settings != null ? General.Settings.ReadSetting(SCRIPT_ZOOM, 0) : 0;
				return Math.Max(-8, Math.Min(34, zoom));
			}
			set
			{
				if(General.Settings == null || value == ScriptZoom) return;
				General.Settings.WriteSetting(SCRIPT_ZOOM, value);
				ScriptViewChanged?.Invoke();
			}
		}

		/// <summary>Whether comments are drawn in italics in the script editor (on by default).</summary>
		public static bool ScriptItalicComments
		{
			get { return General.Settings == null || General.Settings.ReadSetting(SCRIPT_ITALIC_COMMENTS, true); }
			set
			{
				if(General.Settings == null || value == ScriptItalicComments) return;
				General.Settings.WriteSetting(SCRIPT_ITALIC_COMMENTS, value);
				ScriptViewChanged?.Invoke();
			}
		}

		/// <summary>Makes prefab insertion use ULDB's own tag choice (0 = the paste options' choice, 1 keep, 2 renumber, 3 remove).</summary>
		public static void ApplyPrefabTags(PasteOptions options)
		{
			int choice = General.Settings != null ? General.Settings.ReadSetting(PREFAB_TAGS, 0) : 0;
			switch(choice)
			{
				case 1: options.ChangeTags = PasteOptions.TAGS_KEEP; break;
				case 2: options.ChangeTags = PasteOptions.TAGS_RENUMBER; break;
				case 3: options.ChangeTags = PasteOptions.TAGS_REMOVE; break;
			}
		}

		/// <summary>
		/// The interface scale in percent saved by Preferences (0 = automatic), read straight from the configuration file:
		/// Avalonia's scale has to be set before the UI starts, which is before the program configuration is loaded.
		/// </summary>
		public static int ScaleFromFile(string configfile)
		{
			try
			{
				if(!File.Exists(configfile)) return 0;
				Match m = Regex.Match(File.ReadAllText(configfile), @"(?m)^\s*" + UI_SCALE + @"\s*=\s*(\d+)\s*;");
				if(m.Success && int.TryParse(m.Groups[1].Value, out int percent) && percent >= 50 && percent <= 400) return percent;
			}
			catch(Exception) { }
			return 0;
		}
	}
}
