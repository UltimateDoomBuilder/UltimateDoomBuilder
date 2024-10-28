
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
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Actions;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.Editing;
using CodeImp.DoomBuilder.IO;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Plugins;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Types;
using CodeImp.DoomBuilder.Windows;

#endregion

namespace CodeImp.DoomBuilder
{
	public static class General
	{
		#region ================== API Declarations and Mono compatibility

#if MONO_WINFORMS
		public static void ApplyMonoListViewFix(System.Windows.Forms.ListView listview)
		{
			if (listview.View == System.Windows.Forms.View.List)
			{
				listview.View = System.Windows.Forms.View.SmallIcon;
			}
		}
		
		public static void ApplyDataGridViewFix(System.Windows.Forms.DataGridView gridview)
		{
			if (gridview.RowsDefaultCellStyle != null && gridview.RowsDefaultCellStyle.Padding != new System.Windows.Forms.Padding(0,0,0,0))
			{
				gridview.RowsDefaultCellStyle.Padding = new System.Windows.Forms.Padding(0,0,0,0);
			}
		}
#else
		public static void ApplyMonoListViewFix(System.Windows.Forms.ListView listview) {}
		public static void ApplyDataGridViewFix(System.Windows.Forms.DataGridView gridview) {}
#endif

#if NO_WIN32

	internal static void InvokeUIActions(MainForm mainform)
    {
		// This implementation really should work universally, but it seemed to hang sometimes on Windows.
		// Let's hope the mono implementation of Winforms works better.
		mainform.Invoke(new System.Action(() => { mainform.ProcessQueuedUIActions(); }));
	}

	internal static bool MessageBeep(MessageBeepType type)
	{
		System.Media.SystemSounds.Beep.Play();
		return true;
	}

	internal static bool LockWindowUpdate(IntPtr hwnd)
	{
		// This can be safely ignored. It is a performance/flicker optimization. It might not even be needed on Windows anymore.
		return true;
	}

	internal unsafe static void ZeroPixels(PixelColor* pixels, int size)
	{
		var transparent = new PixelColor(0,0,0,0);
		for (int i = 0; i < size; i++)
			pixels[i] = transparent;
	}

	internal static void SetComboBoxItemHeight(ComboBox combobox, int height)
	{
		// Only used by FieldsEditorControl. Not sure what its purpose is, might only be visual adjustment that isn't strictly needed?
	}

#else
		[DllImport("user32.dll")]
		internal static extern bool LockWindowUpdate(IntPtr hwnd);

		[DllImport("kernel32.dll", EntryPoint = "RtlZeroMemory", SetLastError = false)]
		static extern void ZeroMemory(IntPtr dest, int size);

		internal unsafe static void ZeroPixels(PixelColor* pixels, int size) { ZeroMemory(new IntPtr(pixels), size * sizeof(PixelColor)); }

		[DllImport("user32.dll", EntryPoint = "SendMessage", SetLastError = true, CallingConvention = CallingConvention.StdCall)]
		static extern int SendMessage(IntPtr hwnd, uint Msg, IntPtr wParam, IntPtr lParam);

		internal static void SetComboBoxItemHeight(ComboBox combobox, int height)
		{
			SendMessage(combobox.Handle, General.CB_SETITEMHEIGHT, new IntPtr(-1), new IntPtr(height));
		}

		[DllImport("user32.dll", EntryPoint = "PostMessage", SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        static extern int PostMessage(IntPtr hwnd, uint Msg, IntPtr wParam, IntPtr lParam);

		internal static void InvokeUIActions(MainForm mainform)
        {
			PostMessage(mainform.Handle, General.WM_UIACTION, IntPtr.Zero, IntPtr.Zero);
		}

		[DllImport("user32.dll", SetLastError = true)]
		internal static extern bool MessageBeep(MessageBeepType type);

		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern uint GetShortPathName([MarshalAs(UnmanagedType.LPTStr)] string longpath, [MarshalAs(UnmanagedType.LPTStr)]StringBuilder shortpath, uint buffersize);
#endif

		#endregion

		#region ================== Constants

		// SendMessage API
		internal const int WM_USER = 0x400;
        internal const int WM_UIACTION = WM_USER + 1;
		internal const int WM_SYSCOMMAND = 0x112;
        internal const int WM_MOUSEHWHEEL = 0x020E; // [ZZ]
		internal const int WM_MOUSEWHEEL = 0x20A;
        internal const int SC_KEYMENU = 0xF100;
		internal const int CB_SETITEMHEIGHT = 0x153;
		//internal const int CB_SHOWDROPDOWN = 0x14F;
		//internal const int EM_GETSCROLLPOS = WM_USER + 221;
		//internal const int EM_SETSCROLLPOS = WM_USER + 222;
		//internal const int SB_HORZ = 0;
		//internal const int SB_VERT = 1;
		//internal const int SB_CTL = 2;
		//internal const int SIF_RANGE = 0x1;
		//internal const int SIF_PAGE = 0x2;
		//internal const int SIF_POS = 0x4;
		//internal const int SIF_DISABLENOSCROLL = 0x8;
		//internal const int SIF_TRACKPOS = 0x16;
		//internal const int SIF_ALL = SIF_RANGE + SIF_PAGE + SIF_POS + SIF_TRACKPOS;
		
		// Files and Folders
		private const string LEGACY_SETTINGS_FILE = "GZBuilder.cfg"; // To make transision from GZDB* easier
		private const string SETTINGS_FILE = "UDBuilder.cfg";
		private const string DEFAULT_SETTINGS_FILE = "UDBuilder.default.cfg"; //mxd
		private const string SETTINGS_DIR = "Doom Builder";
		private const string LOG_FILE = "UDBuilder.log";
		private const string GAME_CONFIGS_DIR = "Configurations";
		private const string COMPILERS_DIR = "Compilers";
		private const string PLUGINS_DIR = "Plugins";
		private const string SCRIPTS_DIR = "Scripting";
		private const string SCREENSHOTS_DIR = "Screenshots"; //mxd
		private const string SNIPPETS_DIR = "Snippets"; //mxd
		private const string MAP_RESTORE_DIR = "Restore"; //mxd
		private const string SPRITES_DIR = "Sprites";
		private const string TEXTURES_DIR = "Textures"; //mxd
		private const string HELP_FILE = "Refmanual.chm";

		#endregion

		#region ================== Variables

		// Files and Folders
		private static string apppath;
		private static string settingspath;
		private static string restorepath; //mxd
		private static string logfile;
		private static string temppath;
		private static string configspath;
		private static string compilerspath;
		private static string scriptspath;
		private static string snippetspath; //mxd
		private static string screenshotspath; //mxd
		private static string pluginspath;
		private static string spritespath;
		private static string texturespath; //mxd
		
		// Main objects
		private static Assembly thisasm;
		private static MainForm mainwindow;
		private static ProgramConfiguration settings;
		private static MapManager map;
		private static EditingManager editing;
		private static ActionManager actions;
		private static HintsManager hints; //mxd
		private static PluginManager plugins;
		private static ColorCollection colors;
		private static TypesManager types;
		private static ErrorLogger errorlogger;
		private static string commithash; //mxd. Git commit hash
		//private static Mutex appmutex;
		
		// Configurations
		private static List<ConfigurationInfo> configs;
		private static List<CompilerInfo> compilers;
		private static List<NodebuilderInfo> nodebuilders;
		private static Dictionary<string, ScriptConfiguration> scriptconfigs;
		private static Dictionary<string, ScriptConfiguration> compiledscriptconfigs; //mxd
		
		// States
		private static bool debugbuild;
		
		// Command line arguments
		private static string[] cmdargs;
		private static string autoloadfile;
		private static string autoloadmap;
		private static string autoloadconfig;
		private static bool autoloadstrictpatches;
		private static DataLocationList autoloadresources;
		private static bool delaymainwindow;
		private static bool nosettings;
		private static bool portablemode; //mxd
		private static bool debugrenderdevice;

		//misc
		private static readonly Random random = new Random(); //mxd

		// Toasts
		private static ToastManager toastmanager;

		// Autosaving
		private static AutoSaver autosaver;

		#endregion

		#region ================== Properties

		public static Assembly ThisAssembly { get { return thisasm; } }
		public static string AppPath { get { return apppath; } }
		public static string TempPath { get { return temppath; } }
		public static string ConfigsPath { get { return configspath; } }
		internal static string SettingsPath { get { return settingspath; } } //mxd
		internal static string MapRestorePath { get { return restorepath; } } //mxd
		internal static string LogFile { get { return logfile; } } //mxd
		public static string CompilersPath { get { return compilerspath; } }
		public static string PluginsPath { get { return pluginspath; } }
		public static string SpritesPath { get { return spritespath; } }
		internal static string TexturesPath { get { return texturespath; } } //mxd
		public static string SnippetsPath { get { return snippetspath; } } //mxd
		public static string DefaultScreenshotsPath { get { return screenshotspath; } } //mxd
		public static ICollection<string> CommandArgs { get { return Array.AsReadOnly(cmdargs); } }
		internal static MainForm MainWindow { get { return mainwindow; } }
		public static IMainForm Interface { get { return mainwindow; } }
		public static ProgramConfiguration Settings { get { return settings; } }
		public static ColorCollection Colors { get { return colors; } }
		internal static List<ConfigurationInfo> Configs { get { return configs; } }
		internal static List<NodebuilderInfo> Nodebuilders { get { return nodebuilders; } }
		internal static List<CompilerInfo> Compilers { get { return compilers; } }
		internal static Dictionary<string, ScriptConfiguration> ScriptConfigs { get { return scriptconfigs; } }
		internal static Dictionary<string, ScriptConfiguration> CompiledScriptConfigs { get { return compiledscriptconfigs; } } //mxd
		public static MapManager Map { get { return map; } }
		public static ActionManager Actions { get { return actions; } }
		public static HintsManager Hints { get { return hints; } } //mxd
		internal static PluginManager Plugins { get { return plugins; } }
		public static bool DebugBuild { get { return debugbuild; } }
		public static TypesManager Types { get { return types; } }
		public static string AutoLoadFile { get { return autoloadfile; } }
		public static string AutoLoadMap { get { return autoloadmap; } }
		public static string AutoLoadConfig { get { return autoloadconfig; } }
		public static bool AutoLoadStrictPatches { get { return autoloadstrictpatches; } }
		public static DataLocationList AutoLoadResources { get { return new DataLocationList(autoloadresources); } }
		public static bool DelayMainWindow { get { return delaymainwindow; } }
		public static bool NoSettings { get { return nosettings; } }
		public static bool DebugRenderDevice { get { return debugrenderdevice; } }
		public static EditingManager Editing { get { return editing; } }
		public static ErrorLogger ErrorLogger { get { return errorlogger; } }
		public static string CommitHash { get { return commithash; } } //mxd
		public static ToastManager ToastManager { get => toastmanager; }
		internal static AutoSaver AutoSaver { get => autosaver; }

		#endregion

		#region ================== Configurations

		/// <summary>
		/// Checks if a given game configuration file exists.
		/// </summary>
		/// <param name="filename">The file name of the game configuration file.</param>
		/// <returns>true if the game configuration exists, false if it doesn't</returns>
		internal static bool ConfigurationInfoExist(string filename) => configs.Any(ci => string.Compare(Path.GetFileNameWithoutExtension(ci.Filename), Path.GetFileNameWithoutExtension(filename), true) == 0);

		// This returns the game configuration info by filename
		internal static ConfigurationInfo GetConfigurationInfo(string filename)
		{
			// Go for all config infos
			foreach(ConfigurationInfo ci in configs)
			{
				// Check if filename matches
				if(string.Compare(Path.GetFileNameWithoutExtension(ci.Filename),
								  Path.GetFileNameWithoutExtension(filename), true) == 0)
				{
					// Return this info
					return ci;
				}
			}

			// None found
			return null;
		}

		// This loads and returns a game configuration
		private static Configuration LoadGameConfiguration(string filename)
		{
			// Make the full filepathname
			string filepathname = Path.Combine(configspath, filename);
			
			// Load configuration
			try
			{
				// Try loading the configuration
				Configuration cfg = new Configuration(filepathname, true);

				// Check for erors
				if(cfg.ErrorResult)
				{
					// Error in configuration
					errorlogger.Add(ErrorType.Error, "Unable to load the game configuration file \"" + filename + "\". " +
													 "Error in file \"" + cfg.ErrorFile + "\" near line " + cfg.ErrorLine + ": " + cfg.ErrorDescription);
					return null;
				}
				// Check if this is a Doom Builder 2 config
				if(cfg.ReadSetting("type", "") != "Doom Builder 2 Game Configuration")
				{
					// Old configuration
					errorlogger.Add(ErrorType.Error, "Unable to load the game configuration file \"" + filename + "\". " +
													 "This configuration is not a Doom Builder 2 game configuration.");
					return null;
				}

				// Return config
				return cfg;
			}
			catch(Exception e)
			{
				// Unable to load configuration
				errorlogger.Add(ErrorType.Error, "Unable to load the game configuration file \"" + filename + "\". " + e.GetType().Name + ": " + e.Message);
				General.WriteLog(e.StackTrace);
				return null;
			}
		}

		// This loads all game configurations
		private static void LoadAllGameConfigurations()
		{
			// Display status
			mainwindow.DisplayStatus(StatusType.Busy, "Loading game configurations...");

			// Make array
			configs = new List<ConfigurationInfo>();

			// Go for all cfg files in the configurations directory
			string[] filenames = Directory.GetFiles(configspath, "*.cfg", SearchOption.TopDirectoryOnly);

			foreach(string filepath in filenames)
			{
				// Check if it can be loaded
				Configuration cfg = LoadGameConfiguration(Path.GetFileName(filepath));
				if(cfg != null)
				{
					string fullfilename = Path.GetFileName(filepath);
					ConfigurationInfo cfginfo = new ConfigurationInfo(cfg, fullfilename);
					
					// Add to lists
					General.WriteLogLine("Registered game configuration \"" + cfginfo.Name + "\" from \"" + fullfilename + "\"");
					configs.Add(cfginfo);
				}
			}

			// Sort the configs
			configs.Sort();
		}

		// This loads all nodebuilder configurations
		private static void LoadAllNodebuilderConfigurations()
		{
			// Display status
			mainwindow.DisplayStatus(StatusType.Busy, "Loading nodebuilder configurations...");

			// Make array
			nodebuilders = new List<NodebuilderInfo>();

			// Go for all cfg files in the compilers directory
			string[] filenames = Directory.GetFiles(compilerspath, "*.cfg", SearchOption.AllDirectories);
			foreach(string filepath in filenames)
			{
				try
				{
					// Try loading the configuration
					Configuration cfg = new Configuration(filepath, true);

					// Check for erors
					if(cfg.ErrorResult)
					{
						// Error in configuration
						errorlogger.Add(ErrorType.Error, "Unable to load the compiler configuration file \"" + Path.GetFileName(filepath) + "\". " +
														 "Error in file \"" + cfg.ErrorFile + "\" near line " + cfg.ErrorLine + ": " + cfg.ErrorDescription);
					}
					else
					{
						// Get structures
						IDictionary builderslist = cfg.ReadSetting("nodebuilders", new Hashtable());
						foreach(DictionaryEntry de in builderslist)
						{
							// Check if this is a structure
							if(de.Value is IDictionary)
							{
								try
								{
									// Make nodebuilder info
									nodebuilders.Add(new NodebuilderInfo(Path.GetFileName(filepath), de.Key.ToString(), cfg));
								}
								catch(Exception e)
								{
									// Unable to load configuration
									errorlogger.Add(ErrorType.Error, "Unable to load the nodebuilder configuration \"" + de.Key + "\" from \"" + Path.GetFileName(filepath) + "\". Error: " + e.Message);
								}
							}
						}
					}
				}
				catch(Exception)
				{
					// Unable to load configuration
					errorlogger.Add(ErrorType.Error, "Unable to load the compiler configuration file \"" + Path.GetFileName(filepath) + "\".");
				}
			}

			// Sort the list
			nodebuilders.Sort();
		}

		// This loads all script configurations
		private static void LoadAllScriptConfigurations()
		{
			// Display status
			mainwindow.DisplayStatus(StatusType.Busy, "Loading script configurations...");
			
			// Make collection
			scriptconfigs = new Dictionary<string, ScriptConfiguration>(StringComparer.Ordinal);
			compiledscriptconfigs = new Dictionary<string, ScriptConfiguration>(StringComparer.Ordinal); //mxd
			
			// Go for all cfg files in the scripts directory
			string[] filenames = Directory.GetFiles(scriptspath, "*.cfg", SearchOption.TopDirectoryOnly);
			foreach(string filepath in filenames)
			{
				try
				{
					// Try loading the configuration
					Configuration cfg = new Configuration(filepath, true);

					// Check for erors
					if(cfg.ErrorResult)
					{
						// Error in configuration
						errorlogger.Add(ErrorType.Error, "Unable to load the script configuration file \"" + Path.GetFileName(filepath) + "\". " +
														"Error in file \"" + cfg.ErrorFile + "\" near line " + cfg.ErrorLine + ": " + cfg.ErrorDescription);
					}
					else
					{
						try
						{
							// Make script configuration
							ScriptConfiguration scfg = new ScriptConfiguration(cfg);
							string filename = Path.GetFileName(filepath);
							scriptconfigs.Add(filename.ToLowerInvariant(), scfg);

							//mxd. Store acc compilers in a separate dictionary
							if(scfg.ScriptType == ScriptType.ACS)
								compiledscriptconfigs.Add(filename.ToLowerInvariant(), scfg);
						}
						catch(Exception e)
						{
							// Unable to load configuration
							errorlogger.Add(ErrorType.Error, "Unable to load the script configuration \"" + Path.GetFileName(filepath) + "\". Error: " + e.Message);
						}
					}
				}
				catch(Exception e)
				{
					// Unable to load configuration
					errorlogger.Add(ErrorType.Error, "Unable to load the script configuration file \"" + Path.GetFileName(filepath) + "\". Error: " + e.Message);
					General.WriteLogLine(e.StackTrace);
				}
			}
		}

		// This loads all compiler configurations
		private static void LoadAllCompilerConfigurations()
		{
			Dictionary<string, CompilerInfo> addedcompilers = new Dictionary<string, CompilerInfo>(StringComparer.Ordinal);

			// Display status
			mainwindow.DisplayStatus(StatusType.Busy, "Loading compiler configurations...");

			// Make array
			compilers = new List<CompilerInfo>();

			// Go for all cfg files in the compilers directory
			string[] filenames = Directory.GetFiles(compilerspath, "*.cfg", SearchOption.AllDirectories);
			foreach(string filepath in filenames)
			{
				try
				{
					// Try loading the configuration
					Configuration cfg = new Configuration(filepath, true);

					// Check for erors
					if(cfg.ErrorResult)
					{
						// Error in configuration
						errorlogger.Add(ErrorType.Error, "Unable to load the compiler configuration file \"" + Path.GetFileName(filepath) + "\". " +
														 "Error in file \"" + cfg.ErrorFile + "\" near line " + cfg.ErrorLine + ": " + cfg.ErrorDescription);
					}
					else
					{
						// Get structures
						IDictionary compilerslist = cfg.ReadSetting("compilers", new Hashtable());
						foreach(DictionaryEntry de in compilerslist)
						{
							// Check if this is a structure
							if(de.Value is IDictionary)
							{
								// Make compiler info
								CompilerInfo info = new CompilerInfo(Path.GetFileName(filepath), de.Key.ToString(), Path.GetDirectoryName(filepath), cfg);
								if(!addedcompilers.ContainsKey(info.Name))
								{
									compilers.Add(info);
									addedcompilers.Add(info.Name, info);
								}
								else
								{
									errorlogger.Add(ErrorType.Error, "Compiler \"" + info.Name + "\" is defined more than once. The first definition in " + addedcompilers[info.Name].FileName + " will be used.");
								}
							}
						}
					}
				}
				catch(Exception e)
				{
					// Unable to load configuration
					errorlogger.Add(ErrorType.Error, "Unable to load the compiler configuration file \"" + Path.GetFileName(filepath) + "\". " + e.GetType().Name + ": " + e.Message);
					General.WriteLogLine(e.StackTrace);
				}
			}
		}
		
		// This returns a nodebuilder by name
		internal static NodebuilderInfo GetNodebuilderByName(string name)
		{
			// Go for all nodebuilders
			foreach(NodebuilderInfo n in nodebuilders)
			{
				// Name matches?
				if(n.Name == name) return n;
			}

			// Cannot find that nodebuilder
			return null;
		}

		/// <summary>
		/// Saves the program's configuration
		/// </summary>
		internal static void SaveSettings()
		{
			// Save settings configuration
			if (!General.NoSettings)
				General.Settings.Save(Path.Combine(settingspath, SETTINGS_FILE));
		}

		/// <summary>
		/// Saves the game configuration settings, like engine, resources etc.
		/// </summary>
		internal static void SaveGameSettings()
		{
			// Save game configuration settings
			if (configs != null) foreach (ConfigurationInfo ci in configs) ci.SaveSettings();
		}
		
		#endregion

		#region ================== Startup

		// Main program entry
		[STAThread]
		internal static void Main(string[] args)
		{
			// Determine states
			#if DEBUG
				debugbuild = true;
			#else
				debugbuild = false;
				//mxd. Custom exception dialog.
				AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
				Application.ThreadException += Application_ThreadException;
			#endif

			// Enable OS visual styles
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false); //mxd
			//Application.DoEvents();		// This must be here to work around a .NET bug

			//mxd. Set CultureInfo
			Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

			// biwa. If the default culture for threads is not set it'll screw with the culture
			// in the FileSystemWatcher thread, which can result in incorrect string outputs
			// See: https://github.com/jewalky/UltimateDoomBuilder/issues/858
			CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;

			// Set current thread name
			Thread.CurrentThread.Name = "Main Application";

			// Application is running
			//appmutex = new Mutex(false, "gzdoombuilder"); //"doombuilder2"
			
			// Get a reference to this assembly
			thisasm = Assembly.GetExecutingAssembly();
			
			// Find application path
			apppath = Path.GetDirectoryName(Application.ExecutablePath); //mxd. What was the point of using Uri here (other than to prevent lauching from a shared folder)?

			// Parse command-line arguments
			ParseCommandLineArgs(args);

			// Setup directories
			temppath = Path.GetTempPath();
			settingspath = (portablemode ? apppath : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), SETTINGS_DIR)); //mxd
			restorepath = Path.Combine(settingspath, MAP_RESTORE_DIR);
			configspath = Path.Combine(apppath, GAME_CONFIGS_DIR);
			compilerspath = Path.Combine(apppath, COMPILERS_DIR);
			pluginspath = Path.Combine(apppath, PLUGINS_DIR);
			scriptspath = Path.Combine(apppath, SCRIPTS_DIR);
			snippetspath = Path.Combine(apppath, SNIPPETS_DIR); //mxd
			screenshotspath = Path.Combine(apppath, SCREENSHOTS_DIR).Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar); //mxd
			spritespath = Path.Combine(apppath, SPRITES_DIR);
			texturespath = Path.Combine(apppath, TEXTURES_DIR); //mxd
			logfile = Path.Combine(settingspath, LOG_FILE);
			
			// Make program settings directory if missing
			if(!portablemode && !Directory.Exists(settingspath)) Directory.CreateDirectory(settingspath);

			//mxd. Get git commit hash
			var hashes = (AssemblyHashAttribute[])thisasm.GetCustomAttributes(typeof(AssemblyHashAttribute), false);
			if(hashes.Length == 1)
			{
				commithash = hashes[0].CommitHash;
			}
			else
			{
				WriteLogLine("Unable to determine commit hash. Missing AssemblyHashAttribute?");
				commithash = "0000000";
			}
			
			// Remove the previous log file and start logging
			if(File.Exists(logfile)) File.Delete(logfile);
            string platform = Environment.Is64BitProcess ? "x64" : "x86";
			General.WriteLogLine("Ultimate Doom Builder R" + thisasm.GetName().Version.Revision + " (" + platform + ", " + commithash + ") startup"); //mxd
			General.WriteLogLine("Application path:        \"" + apppath + "\"");
			General.WriteLogLine("Temporary path:          \"" + temppath + "\"");
			General.WriteLogLine("Local settings path:     \"" + settingspath + "\"");
			General.WriteLogLine("Command-line arguments:  \"" + string.Join(" ", args) + "\""); //mxd

			// Load configuration
			General.WriteLogLine("Loading program configuration...");
			settings = new ProgramConfiguration();
			string defaultsettingsfile = Path.Combine(apppath, DEFAULT_SETTINGS_FILE);
			string usersettingsfile = nosettings ? defaultsettingsfile : Path.Combine(settingspath, SETTINGS_FILE);
			string legacysettingsfile = nosettings ? String.Empty : Path.Combine(settingspath, LEGACY_SETTINGS_FILE);

			if(settings.Load(usersettingsfile, defaultsettingsfile, legacysettingsfile))
			{
				// Create error logger
				errorlogger = new ErrorLogger();
				
				// Create action manager
				actions = new ActionManager();
				
				// Bind static methods to actions
				General.Actions.BindMethods(typeof(General));

				//mxd. Create hints manager
				hints = new HintsManager();

				// Initialize static classes
				MapSet.Initialize();

				// Create main window
				General.WriteLogLine("Loading main interface window...");
				mainwindow = new MainForm();
				mainwindow.SetupInterface();
				mainwindow.UpdateInterface();
				mainwindow.UpdateThingsFilters();

				if(!delaymainwindow)
				{
					// Show main window
					General.WriteLogLine("Showing main interface window...");
					mainwindow.Show();
					mainwindow.Update();
				}

				// Create the toast manager after the main windows, but before plugins are loaded,
				// since the plugins can register toasts. Also register toasts for the core
				toastmanager = new ToastManager(mainwindow.Display);
				RegisterToasts();
				
				// Load plugin manager
				General.WriteLogLine("Loading plugins...");
				plugins = new PluginManager();
				plugins.LoadAllPlugins();

				// Register toasts from actions. This has to be done after all plugins are loaded
				toastmanager.RegisterActions();
				toastmanager.LoadSettings(settings.Config);
				
				// Load game configurations
				General.WriteLogLine("Loading game configurations...");
				LoadAllGameConfigurations();

				// Create editing modes
				General.WriteLogLine("Creating editing modes manager...");
				editing = new EditingManager();
				
				// Now that all settings have been combined (core & plugins) apply the defaults
				General.WriteLogLine("Applying configuration settings...");
				actions.ApplyDefaultShortcutKeys();
				mainwindow.ApplyShortcutKeys();
				foreach(ConfigurationInfo info in configs) info.ApplyDefaults(null);
				
				// Load compiler configurations
				General.WriteLogLine("Loading compiler configurations...");
				LoadAllCompilerConfigurations();

				// Load nodebuilder configurations
				General.WriteLogLine("Loading nodebuilder configurations...");
				LoadAllNodebuilderConfigurations();
				
				// Load script configurations
				General.WriteLogLine("Loading script configurations...");
				LoadAllScriptConfigurations();
				
				// Load color settings
				General.WriteLogLine("Loading color settings...");
				colors = new ColorCollection(settings.Config);
				
				// Create types manager
				General.WriteLogLine("Creating types manager...");
				types = new TypesManager();
				
				// Do auto map loading when window is delayed
				if(delaymainwindow) mainwindow.PerformAutoMapLoading();
				
				// All done
				General.WriteLogLine("Startup done");
				mainwindow.DisplayReady();
				
				// Show any errors if preferred
				if(errorlogger.IsErrorAdded)
				{
					mainwindow.DisplayStatus(StatusType.Warning, "There were errors during program startup!");
					if(!delaymainwindow && General.Settings.ShowErrorsWindow) mainwindow.ShowErrors();
				}

				//mxd. Check enabled game configuration
				bool noneenabled = true;
				for(int i = 0; i < configs.Count; i++) 
				{
					if(configs[i].Enabled) 
					{
						noneenabled = false;
						break;
					}
				}

				if(noneenabled) 
				{
					if(MessageBox.Show("No game configurations are currently enabled.\nPlease enable at least one game configuration", "Warning", MessageBoxButtons.OK) == DialogResult.OK) 
						mainwindow.ShowConfiguration();
				}

				//mxd. Check backup files
				if(Directory.Exists(restorepath))
				{
					foreach(string backup in Directory.GetFiles(restorepath, "*.restore"))
					{
						// Remove if created more than a month ago
						if((DateTime.Now - File.GetLastWriteTime(backup)).TotalDays > 30)
						{
							File.Delete(backup);
							WriteLogLine("Removed \"" + backup + "\" map backup.");
						}
					}
				}

				//mxd. Check for updates?
#if !NO_UPDATER
				if(General.Settings.CheckForUpdates) UpdateChecker.PerformCheck(false);
#endif

				// Prepare autosaving
				autosaver = new AutoSaver();

				// Run application from the main window
				Application.Run(mainwindow);
			}
			else
			{
				// Terminate
				Terminate(false);
			}
		}

		private static void RegisterToasts()
		{
			toastmanager.RegisterToast("resourcewarningsanderrors", "Resource warnings and errors", "When there are errors or warning while (re)loading the resources");
			toastmanager.RegisterToast("autosave", "Autosave", "Notifications related to autosaving");
		}

		// This parses the command line arguments
		private static void ParseCommandLineArgs(string[] args)
		{
			autoloadresources = new DataLocationList();
			
			// Keep a copy
			cmdargs = args;
			
			// Make a queue so we can parse the values from left to right
			Queue<string> argslist = new Queue<string>(args);
			
			// Parse list
			while(argslist.Count > 0)
			{
				// Get next arg
				string curarg = argslist.Dequeue();
				
				// Delay window?
				if(string.Compare(curarg, "-DELAYWINDOW", true) == 0)
				{
					// Delay showing the main window
					delaymainwindow = true;
				}
				// No settings?
				else if(string.Compare(curarg, "-NOSETTINGS", true) == 0)
				{
					// Don't load or save program settings
					nosettings = true;
				}
				// Map name info?
				else if(string.Compare(curarg, "-MAP", true) == 0)
				{
					// Store next arg as map name information
					autoloadmap = argslist.Dequeue()?.ToUpperInvariant();
				}
				// Config name info?
				else if((string.Compare(curarg, "-CFG", true) == 0) ||
						(string.Compare(curarg, "-CONFIG", true) == 0))
				{
					// Store next arg as config filename information
					autoloadconfig = argslist.Dequeue();
				}
				// Strict patches rules?
				else if(string.Compare(curarg, "-STRICTPATCHES", true) == 0)
				{
					autoloadstrictpatches = true;
				}
				//mxd. Portable mode?
				else if(string.Compare(curarg, "-PORTABLE", true) == 0)
				{
					// Can we write stuff to apppath?
					try
					{
						WindowsIdentity identity = WindowsIdentity.GetCurrent();
						if(identity != null)
						{
							WindowsPrincipal principal = new WindowsPrincipal(identity);
							DirectorySecurity security = Directory.GetAccessControl(apppath);
							AuthorizationRuleCollection authrules = security.GetAccessRules(true, true, typeof(SecurityIdentifier));

							foreach(FileSystemAccessRule accessrule in authrules)
							{
								SecurityIdentifier id = accessrule.IdentityReference as SecurityIdentifier;
								if(id == null || !principal.IsInRole(id)) continue;
								if((FileSystemRights.WriteData & accessrule.FileSystemRights) != FileSystemRights.WriteData) continue;
								
								if(accessrule.AccessControlType == AccessControlType.Allow) 
								{
									portablemode = true;
								} 
								else if(accessrule.AccessControlType == AccessControlType.Deny) 
								{
									//Deny usually overrides any Allow
									portablemode = false;
									break;
								}
							}
						}
					} 
					catch(Exception) { }

					// Warn the user?
					if(!portablemode) ShowWarningMessage("Failed to enable portable mode.\nMake sure you have write premission for \"" + apppath + "\" directory.", MessageBoxButtons.OK);
				}
				// Resource?
				else if(string.Compare(curarg, "-RESOURCE", true) == 0)
				{
					DataLocation dl = new DataLocation();

					// Parse resource type
					string resourcetype = argslist.Dequeue();
					if(string.Compare(resourcetype, "WAD", true) == 0)
						dl.type = DataLocation.RESOURCE_WAD;
					else if(string.Compare(resourcetype, "DIR", true) == 0)
						dl.type = DataLocation.RESOURCE_DIRECTORY;
					else if(string.Compare(resourcetype, "PK3", true) == 0)
						dl.type = DataLocation.RESOURCE_PK3;
					else
					{
						General.WriteLogLine("Unexpected resource type \"" + resourcetype + "\" in program parameters. Expected \"wad\", \"dir\" or \"pk3\".");
						break;
					}

					// We continue parsing args until an existing filename is found
					// all other arguments must be one of the optional keywords.
					while(string.IsNullOrEmpty(dl.location))
					{
						curarg = argslist.Dequeue();

						if((string.Compare(curarg, "ROOTTEXTURES", true) == 0) &&
						   (dl.type == DataLocation.RESOURCE_DIRECTORY))
						{
							// Load images in the root directory of the resource as textures
							dl.option1 = true;
						}
						else if((string.Compare(curarg, "ROOTFLATS", true) == 0) &&
								(dl.type == DataLocation.RESOURCE_DIRECTORY))
						{
							// Load images in the root directory of the resource as flats
							dl.option2 = true;
						}
						else if((string.Compare(curarg, "STRICTPATCHES", true) == 0) &&
								(dl.type == DataLocation.RESOURCE_WAD))
						{
							// Use strict rules for patches
							dl.option1 = true;
						}
						else if(string.Compare(curarg, "NOTEST", true) == 0)
						{
							// Exclude this resource from testing parameters
							dl.notfortesting = true;
						}
						else
						{
							// This must be an existing file, or it is an invalid argument
							if(dl.type == DataLocation.RESOURCE_DIRECTORY)
							{
								if(Directory.Exists(curarg))
									dl.location = curarg;
							}
							else
							{
								if(File.Exists(curarg))
									dl.location = curarg;
							}
							
							if(string.IsNullOrEmpty(dl.location))
							{
								General.WriteLogLine("Unexpected argument \"" + curarg + "\" in program parameters. Expected a valid resource option or a resource filename.");
								break;
							}
						}
					}

					// Add resource to list
					if(!string.IsNullOrEmpty(dl.location))
						autoloadresources.Add(dl);
				}
				else if (string.Compare(curarg, "-DEBUGRENDERDEVICE", true) == 0)
				{
					debugrenderdevice = true;
				}
				// Every other arg
				else
				{
					// No command to load file yet?
					if(autoloadfile == null)
					{
						// Check if this is a file we can load
						if(File.Exists(curarg))
						{
							// Load this file!
							autoloadfile = curarg.Trim();
						}
						else
						{
							// Note in the log that we cannot find this file
							General.WriteLogLine("Cannot find the specified file \"" + curarg + "\"");
						}
					}
				}
			}
		}
		
		// This cancels automatic map loading
		internal static void CancelAutoMapLoad()
		{
			autoloadfile = null;
		}
		
#endregion
		
#region ================== Terminate

		// This is for plugins to use
		public static void Exit(bool properexit)
		{
			// Plugin wants to exit nicely?
			if(properexit)
			{
				// Close dialog forms first
				while((Form.ActiveForm != mainwindow) && (Form.ActiveForm != null))
					Form.ActiveForm.Close();

				// Close main window
				mainwindow.Close();
			}
			else
			{
				// Terminate, no questions asked
				Terminate(true);
			}
		}
		
		// This terminates the program
		internal static void Terminate(bool properexit)
		{
			// Terminate properly?
			if(properexit)
			{
				General.WriteLogLine("Termination requested");
				
				// Unbind static methods from actions
				General.Actions.UnbindMethods(typeof(General));
				
				// Save colors
				if(colors != null) colors.SaveColors(settings.Config);
				
				// Save action controls
				actions.SaveSettings();

				// Save game settings
				SaveGameSettings();

				// Save program configuration
				SaveSettings();
				
				// Clean up
				if(map != null) { map.Dispose(); map = null; }
				if(editing != null) { editing.Dispose(); editing = null; }
				if(plugins != null) { plugins.Dispose(); plugins = null; }
				if(mainwindow != null) { mainwindow.Dispose(); mainwindow = null; }
				if(actions != null) { actions.Dispose(); actions = null; }
				if(types != null) { types.Dispose(); types = null; }

				// Application ends here and now
				General.WriteLogLine("Termination done");
				Application.Exit();
			}
			else
			{
				// Just end now
				General.WriteLogLine("Immediate program termination");
				Application.Exit();
			}

			// Die.
			Process.GetCurrentProcess().Kill();
		}
		
#endregion
		
#region ================== Management
		
		// This creates a new map
		[BeginAction("newmap")]
		internal static void NewMap()
		{
			MapOptions newoptions = new MapOptions();

			// Cancel volatile mode, if any
			editing.DisengageVolatileMode();
			
			// Ask the user to save changes (if any)
			if(AskSaveMap())
			{
				// Open map options dialog
				MapOptionsForm optionswindow = new MapOptionsForm(newoptions, true);
				if(optionswindow.ShowDialog(mainwindow) == DialogResult.OK)
				{
					// Display status
					mainwindow.DisplayStatus(StatusType.Busy, "Creating new map...");
					Cursor.Current = Cursors.WaitCursor;
					
					// Clear the display
					mainwindow.ClearDisplay();
					mainwindow.RemoveHintsDocker(); //mxd

					// Trash the current map, if any
					if(map != null) map.Dispose();
					
					// Let the plugins know
					plugins.OnMapNewBegin();

					// Clear old errors (mxd)
					errorlogger.Clear();
					
					// Create map manager with given options
					map = new MapManager();
					if(map.InitializeNewMap(newoptions))
					{
						settings.FindDefaultDrawSettings(); //mxd

						// Let the plugins know
						plugins.OnMapNewEnd();

						// All done
						mainwindow.SetupInterface();
						mainwindow.RedrawDisplay();
						mainwindow.UpdateThingsFilters();
						mainwindow.UpdateLinedefColorPresets(); //mxd
						mainwindow.UpdateInterface();
						mainwindow.AddHintsDocker(); //mxd
						mainwindow.UpdateGZDoomPanel(); //mxd
						mainwindow.HideInfo(); //mxd
					}
					else
					{
						// Unable to create map manager
						map.Dispose();
						map = null;

						// Show splash logo on display
						mainwindow.ShowSplashDisplay();
					}

					if(errorlogger.IsErrorAdded)
					{
						// Show any errors if preferred
						mainwindow.DisplayStatus(StatusType.Warning, "There were errors during loading!");
						if(!delaymainwindow && settings.ShowErrorsWindow) mainwindow.ShowErrors();
					}
					else
						mainwindow.DisplayReady();

					//mxd. Also reset the clock...
					MainWindow.ResetClock();

					Cursor.Current = Cursors.Default;
				}
			}
		}

		// This closes the current map
		[BeginAction("closemap")]
		internal static void ActionCloseMap() { CloseMap(); }
		internal static bool CloseMap()
		{
			// Cancel volatile mode, if any
			editing.DisengageVolatileMode();

			// Ask the user to save changes (if any)
			if(AskSaveMap())
			{
				// Display status
				mainwindow.DisplayStatus(StatusType.Busy, "Closing map...");
				WriteLogLine("Unloading map...");
				Cursor.Current = Cursors.WaitCursor;
				
				// Trash the current map
				if(map != null) map.Dispose();
				map = null;
				
				// Clear errors
				errorlogger.Clear();

				//mxd. Clear Console
#if DEBUG
				DebugConsole.Clear();
#endif
				
				// Show splash logo on display
				mainwindow.ShowSplashDisplay();
				
				// Done
				Cursor.Current = Cursors.Default;
				editing.UpdateCurrentEditModes();
				mainwindow.SetupInterface();
				mainwindow.RedrawDisplay();
				mainwindow.HideInfo();
				mainwindow.UpdateThingsFilters();
				//mxd
				mainwindow.UpdateLinedefColorPresets();
				mainwindow.RemoveHintsDocker();
				mainwindow.UpdateGZDoomPanel();
				mainwindow.UpdateInterface();
				mainwindow.DisplayReady();
				WriteLogLine("Map unload done");
				return true;
			}
			else
			{
				// User cancelled
				return false;
			}
		}

		// This loads a map from file
		[BeginAction("openmap")]
		internal static void OpenMap()
		{
			// Cancel volatile mode, if any
			editing.DisengageVolatileMode();

			// Open map file dialog
			OpenFileDialog openfile = new OpenFileDialog();
			openfile.Title = "Open Map";

#if NO_WIN32
			// No easy way to have case-insesitivity for non-Windows platforms
			openfile.Filter = "Doom WAD Files (*.wad)|*.wad;*.Wad;*.wAd;*.WAd;*.waD;*.WaD;*.wAD;*.WAD";
#else
			openfile.Filter = "Doom WAD Files (*.wad)|*.wad";
#endif			

			if (!string.IsNullOrEmpty(settings.LastUsedMapFolder) && Directory.Exists(settings.LastUsedMapFolder)) //mxd
			{
				openfile.RestoreDirectory = true;
				openfile.InitialDirectory = settings.LastUsedMapFolder;
			} 
			openfile.AddExtension = false;
			openfile.CheckFileExists = true;
			openfile.Multiselect = false;
			openfile.ValidateNames = true;
			if(openfile.ShowDialog(mainwindow) == DialogResult.OK)
			{
				// Update main window
				mainwindow.Update();

				// Open map file
				OpenMapFile(openfile.FileName, null);
			}

			openfile.Dispose();
		}

		//mxd. This loads a different map from same wad file without reloading resources
		[BeginAction("openmapincurrentwad")]
		internal static void OpenMapInCurrentWad() 
		{
			if(map == null || string.IsNullOrEmpty(map.FilePathName) || !File.Exists(map.FilePathName))
			{
				Interface.DisplayStatus(StatusType.Warning, "Unable to open map from current WAD!");
				return;
			}

			// Cancel volatile mode, if any
			Editing.DisengageVolatileMode();

			// Ask the user to save changes (if any)
			if(!AskSaveMap()) return;

			// Open map options dialog
			ChangeMapForm changemapwindow = new ChangeMapForm(map.FilePathName, map.Options);
			if(changemapwindow.ShowDialog(mainwindow) != DialogResult.OK) return;

			// Display status
			mainwindow.DisplayStatus(StatusType.Busy, "Switching to map \"" + changemapwindow.Options.CurrentName + "\"...");
			WriteLogLine("Switching to map \"" + changemapwindow.Options.CurrentName + "\"...");
			
			Cursor.Current = Cursors.WaitCursor;

			// Let the plugins know
			plugins.OnMapCloseBegin();

			// Clear the display
			mainwindow.ClearDisplay();
			mainwindow.RemoveHintsDocker(); //mxd

			//mxd. Close the script editor
			map.CloseScriptEditor(false);

			// Let the plugins know
			plugins.OnMapCloseEnd();
			plugins.OnMapOpenBegin();

			// Clear old errors
			ErrorLogger.Clear();

			if(!map.InitializeSwitchMap(changemapwindow.Options)) return;

			// Clear undo history
			map.UndoRedo.ClearAllUndos();
			map.UndoRedo.ClearAllRedos();

			settings.FindDefaultDrawSettings(); //mxd

			// Let the plugins know
			plugins.OnMapOpenEnd();

			// All done
			mainwindow.SetupInterface();
			mainwindow.RedrawDisplay();
			mainwindow.UpdateThingsFilters();
			mainwindow.UpdateLinedefColorPresets(); //mxd
			mainwindow.UpdateInterface();
			mainwindow.HideInfo();
			mainwindow.AddHintsDocker(); //mxd
			mainwindow.UpdateGZDoomPanel(); //mxd

			if(errorlogger.IsErrorAdded)
			{
				// Show any errors if preferred
				mainwindow.DisplayStatus(StatusType.Warning, "There were errors during loading!");
				if(!delaymainwindow && Settings.ShowErrorsWindow)
					mainwindow.ShowErrors();
			} 
			else 
			{
				mainwindow.DisplayReady();
			}

			Cursor.Current = Cursors.Default;
		}

		// This opens the specified file
		internal static void OpenMapFile(string filename, MapOptions options)
		{
			// Cancel volatile mode, if any
			editing.DisengageVolatileMode();
			
			// Ask the user to save changes (if any)
			if(AskSaveMap())
			{
				// Open map options dialog
				OpenMapOptionsForm openmapwindow = (options != null ? new OpenMapOptionsForm(filename, options) : new OpenMapOptionsForm(filename));

				if(openmapwindow.ShowDialog(mainwindow) == DialogResult.OK)
					OpenMapFileWithOptions(filename, openmapwindow.Options);
			}
		}
		
		// This opens the specified file without dialog
		internal static void OpenMapFileWithOptions(string filename, MapOptions options)
		{
			// Display status
			mainwindow.DisplayStatus(StatusType.Busy, "Opening map file...");
			Cursor.Current = Cursors.WaitCursor;

			// Clear the display
			mainwindow.ClearDisplay();
			mainwindow.RemoveHintsDocker(); //mxd

			// Trash the current map, if any
			if(map != null) map.Dispose();

			// Let the plugins know
			plugins.OnMapOpenBegin();

			// mxd. Clear old errors
			errorlogger.Clear();

			// Create map manager with given options
			map = new MapManager();
			if(map.InitializeOpenMap(filename, options))
			{
				// Add recent file
				mainwindow.AddRecentFile(filename);

				//mxd
				mainwindow.UpdateGZDoomPanel();
				settings.LastUsedMapFolder = Path.GetDirectoryName(filename);
				settings.FindDefaultDrawSettings();

				// Let the plugins know
				plugins.OnMapOpenEnd();

				// All done
				mainwindow.SetupInterface();
				mainwindow.UpdateThingsFilters();
				mainwindow.UpdateLinedefColorPresets(); //mxd
				mainwindow.UpdateInterface();
				mainwindow.HideInfo();
				mainwindow.AddHintsDocker(); //mxd

				//mxd. Center map in screen or on stored coordinates. Done here to avoid the view jerking around when updating the interface.
				ClassicMode mode = Editing.Mode as ClassicMode;
				if(mode != null)
				{
					if(options != null && options.ViewPosition.IsFinite() && !float.IsNaN(options.ViewScale))
						mode.CenterOnCoordinates(options.ViewPosition, options.ViewScale);
					else
						mode.CenterInScreen();
				}

				mainwindow.RedrawDisplay();
			}
			else
			{
				// Unable to create map manager
				map.Dispose();
				map = null;

				// Show splash logo on display
				mainwindow.ShowSplashDisplay();
			}

			if(errorlogger.IsErrorAdded)
			{
				// Show any errors if preferred
				mainwindow.DisplayStatus(StatusType.Warning, "There were errors during loading!");
				if(!delaymainwindow && settings.ShowErrorsWindow) mainwindow.ShowErrors();
			}
			else
				mainwindow.DisplayReady();
			
			Cursor.Current = Cursors.Default;
		}
		
		// This saves the current map
		// Returns tre when saved, false when cancelled or failed
		[BeginAction("savemap")]
		internal static void ActionSaveMap() { SaveMap(); }
		internal static bool SaveMap()
		{
			if(map == null) return false;
			bool result = false;
			
			// Cancel volatile mode, if any
			editing.DisengageVolatileMode();
			
			// Check if a wad file is known
			if(string.IsNullOrEmpty(map.FilePathName))
			{
				// Call to SaveMapAs
				result = SaveMapAs();
			}
			else
			{
				//mxd. Do we need to save the map?
				if(!map.MapSaveRequired(map.FilePathName, SavePurpose.Normal))
				{
					// Still save settings file
					result = map.SaveSettingsFile(map.FilePathName);
					
					// Display status
					mainwindow.DisplayStatus(StatusType.Info, "Map is up to date. Updated map settings file.");

					// All done
					mainwindow.UpdateInterface();
					return result;
				}
				
				// Display status
				mainwindow.DisplayStatus(StatusType.Busy, "Saving map file...");
				Cursor.Current = Cursors.WaitCursor;

				// Set this to false so we can see if errors are added
				errorlogger.IsErrorAdded = false;
				
				// Save the map
				plugins.OnMapSaveBegin(SavePurpose.Normal);
				if(map.SaveMap(map.FilePathName, SavePurpose.Normal))
				{
					// Add recent file
					mainwindow.AddRecentFile(map.FilePathName);
					result = true;
				}
				plugins.OnMapSaveEnd(SavePurpose.Normal);

				// All done
				mainwindow.UpdateInterface();

				if(errorlogger.IsErrorAdded)
				{
					// Show any errors if preferred
					mainwindow.DisplayStatus(StatusType.Warning, "There were errors during saving!");
					if(!delaymainwindow && settings.ShowErrorsWindow) mainwindow.ShowErrors();
				}
				else if(result)
				{
					mainwindow.DisplayStatus(StatusType.Info, "Map saved in " + map.FileTitle + ".");
				}
				else
				{
					mainwindow.DisplayStatus(StatusType.Info, "Map saving cancelled."); //mxd
				}

				Cursor.Current = Cursors.Default;
			}

			//mxd. Also reset the clock...
			MainWindow.ResetClock();

			return result;
		}


		// This saves the current map as a different file
		// Returns tre when saved, false when cancelled or failed
		[BeginAction("savemapas")]
		internal static void ActionSaveMapAs() { SaveMapAs(); }
		internal static bool SaveMapAs()
		{
			if(map == null) return false;
			bool result = false;

			// Cancel volatile mode, if any
			editing.DisengageVolatileMode();

			// Show save as dialog
			SaveFileDialog savefile = new SaveFileDialog();
#if NO_WIN32
			// No easy way to have case-insesitivity for non-Windows platforms
			savefile.Filter = "Doom WAD Files (*.wad)|*.wad;*.Wad;*.wAd;*.WAd;*.waD;*.WaD;*.wAD;*.WAD";
#else
			savefile.Filter = "Doom WAD Files (*.wad)|*.wad";
#endif
			savefile.Title = "Save Map As";
			savefile.AddExtension = true;
			savefile.CheckPathExists = true;
			savefile.OverwritePrompt = true;
			savefile.ValidateNames = true;
			savefile.FileName = map.FileTitle; //mxd
			if(map.FilePathName.Length > 0) //mxd
			{
				savefile.RestoreDirectory = true;
				savefile.InitialDirectory = Path.GetDirectoryName(map.FilePathName);
			}

			if(savefile.ShowDialog(mainwindow) == DialogResult.OK)
			{
				// Check if we're saving to the same file as the original.
				// Because some muppets use Save As even when saving to the same file.
				string currentfilename = (map.FilePathName.Length > 0) ? Path.GetFullPath(map.FilePathName).ToLowerInvariant() : "";
				string savefilename = Path.GetFullPath(savefile.FileName).ToLowerInvariant();
				if(currentfilename == savefilename)
				{
					SaveMap();
				}
				else
				{
					// Display status
					mainwindow.DisplayStatus(StatusType.Busy, "Saving map file...");
					Cursor.Current = Cursors.WaitCursor;
					
					// Set this to false so we can see if errors are added
					errorlogger.IsErrorAdded = false;
					
					// Save the map
					plugins.OnMapSaveBegin(SavePurpose.AsNewFile);
					if(map.SaveMap(savefile.FileName, SavePurpose.AsNewFile))
					{
						// Add recent file
						mainwindow.AddRecentFile(map.FilePathName);
						settings.LastUsedMapFolder = Path.GetDirectoryName(map.FilePathName); //mxd
						result = true;
					}
					plugins.OnMapSaveEnd(SavePurpose.AsNewFile);
					
					// All done
					mainwindow.UpdateInterface();
					
					if(errorlogger.IsErrorAdded)
					{
						// Show any errors if preferred
						mainwindow.DisplayStatus(StatusType.Warning, "There were errors during saving!");
						if(!delaymainwindow && settings.ShowErrorsWindow) mainwindow.ShowErrors();
					}
					else if(result)
					{
						mainwindow.DisplayStatus(StatusType.Info, "Map saved in " + map.FileTitle + ".");
					}
					else
					{
						mainwindow.DisplayStatus(StatusType.Info, "Map saving cancelled."); //mxd
					}
					
					Cursor.Current = Cursors.Default;
				}
			}
			
			savefile.Dispose();

			//mxd. Also reset the clock...
			MainWindow.ResetClock();

			return result;
		}


		// This saves the current map as a different file
		// Returns tre when saved, false when cancelled or failed
		[BeginAction("savemapinto")]
		internal static void ActionSaveMapInto() { SaveMapInto(); }
		internal static bool SaveMapInto()
		{
			if(map == null) return false;
			bool result = false;

			// Cancel volatile mode, if any
			editing.DisengageVolatileMode();

			// Show save as dialog
			SaveFileDialog savefile = new SaveFileDialog();
#if NO_WIN32
			// No easy way to have case-insesitivity for non-Windows platforms
			savefile.Filter = "Doom WAD Files (*.wad)|*.wad;*.Wad;*.wAd;*.WAd;*.waD;*.WaD;*.wAD;*.WAD";
#else
			savefile.Filter = "Doom WAD Files (*.wad)|*.wad";
#endif
			savefile.Title = "Save Map Into";
			savefile.AddExtension = true;
			savefile.CheckPathExists = true;
			savefile.OverwritePrompt = false;
			savefile.ValidateNames = true;
			if(savefile.ShowDialog(mainwindow) == DialogResult.OK)
			{
				// Display status
				mainwindow.DisplayStatus(StatusType.Busy, "Saving map file...");
				Cursor.Current = Cursors.WaitCursor;

				// Set this to false so we can see if errors are added
				errorlogger.IsErrorAdded = false;
				
				// Save the map
				plugins.OnMapSaveBegin(SavePurpose.IntoFile);
				if(map.SaveMap(savefile.FileName, SavePurpose.IntoFile))
				{
					// Add recent file
					mainwindow.AddRecentFile(map.FilePathName);
					result = true;
				}
				plugins.OnMapSaveEnd(SavePurpose.IntoFile);

				// All done
				mainwindow.UpdateInterface();

				if(errorlogger.IsErrorAdded)
				{
					// Show any errors if preferred
					mainwindow.DisplayStatus(StatusType.Warning, "There were errors during saving!");
					if(!delaymainwindow && settings.ShowErrorsWindow) mainwindow.ShowErrors();
				}
				else if(result)
				{
					mainwindow.DisplayStatus(StatusType.Info, "Map saved in " + map.FileTitle + ".");
				}
				else
				{
					mainwindow.DisplayStatus(StatusType.Info, "Map saving cancelled."); //mxd
				}

				Cursor.Current = Cursors.Default;
			}

			savefile.Dispose();

			//mxd. Also reset the clock...
			MainWindow.ResetClock();

			return result;
		}
		
		// This asks to save the map if needed
		// Returns false when action was cancelled
		internal static bool AskSaveMap()
		{
			// Map open and not saved?
			if(map != null)
			{
				if(map.IsChanged)
				{
					// Ask to save changes
					DialogResult result = MessageBox.Show(mainwindow, "Do you want to save changes to " + map.FileTitle + " (" + map.Options.CurrentName + ")?", Application.ProductName, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
					if(result == DialogResult.Yes)
					{
						// Save map
						if(SaveMap())
						{
							// Ask to save changes to scripts
							return map.AskSaveScriptChanges();
						}
						else
						{
							// Failed to save map
							return false;
						}
					}
					else if(result == DialogResult.Cancel)
					{
						// Abort
						return false;
					}
					else
					{
						// Ask to save changes to scripts
						return map.AskSaveScriptChanges();
					}
				}
				else
				{
					// Ask to save changes to scripts
					return map.AskSaveScriptChanges();
				}
			}
			else
			{
				return true;
			}
		}
		
#endregion

#region ================== Debug
		
		// This shows a major failure
		public static void Fail(string message)
		{
			WriteLogLine("FAIL: " + message);
#if DEBUG
			Debug.Fail(message);
#else
			//mxd. Lets notify the user about our Epic Failure before crashing...
			ShowErrorMessage(message, MessageBoxButtons.OK);
#endif
			Terminate(false);
		}
		
		// This outputs log information
		public static void WriteLogLine(string line)
		{
			lock (random)
			{
#if DEBUG
				// Output to consoles
				Console.WriteLine(line);
				DebugConsole.WriteLine(DebugMessageType.LOG, line); //mxd
#endif
				// Write to log file
				try { File.AppendAllText(logfile, line + Environment.NewLine); }
				catch (Exception) { }
			}
		}

		// This outputs log information
		public static void WriteLog(string text)
		{
			lock (random)
			{
#if DEBUG
				// Output to consoles
				Console.Write(text);
				DebugConsole.Write(DebugMessageType.LOG, text);
#endif

				// Write to log file
				try { File.AppendAllText(logfile, text); }
				catch (Exception) { }
			}
		}
		
#endregion

#region ================== Tools
		
		// This swaps two pointers
		public static void Swap<T>(ref T a, ref T b)
		{
			T t = a;
			a = b;
			b = t;
		}

		// This calculates the bits needed for a number
		public static int BitsForInt(int v)
		{
			int[] LOGTABLE = new[] {
			  0, 0, 1, 1, 2, 2, 2, 2, 3, 3, 3, 3, 3, 3, 3, 3,
			  4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4,
			  5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
			  5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
			  6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6,
			  6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6,
			  6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6,
			  6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6,
			  7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7,
			  7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7,
			  7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7,
			  7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7,
			  7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7,
			  7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7,
			  7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7,
			  7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7 };
			
			int r;	// r will be lg(v)
			int t, tt;

			if(Int2Bool(tt = v >> 16))
			{
				r = Int2Bool(t = tt >> 8) ? 24 + LOGTABLE[t] : 16 + LOGTABLE[tt];
			}
			else
			{
				r = Int2Bool(t = v >> 8) ? 8 + LOGTABLE[t] : LOGTABLE[v];
			}
			
			return r;
		}
		
		// This clamps a value
		public static float Clamp(float value, float min, float max)
		{
			return Math.Min(Math.Max(min, value), max);
		}

		// This clamps a value
		public static double Clamp(double value, double min, double max)
		{
			return Math.Min(Math.Max(min, value), max);
		}

		// This clamps a value
		public static int Clamp(int value, int min, int max)
		{
			return Math.Min(Math.Max(min, value), max);
		}

		// This clamps a value
		public static byte Clamp(byte value, byte min, byte max)
		{
			return Math.Min(Math.Max(min, value), max);
		}

		//mxd. This clamps angle between 0 and 359
		public static int ClampAngle(int angle) 
		{
			angle %= 360;
			if(angle < 0) angle += 360;
			return angle;
		}

		//mxd. This clamps angle between 0 and 359
		public static float ClampAngle(float angle) 
		{
			angle %= 360;
			if(angle < 0) angle += 360;
			return angle;
		}

		// This clamps angle between 0 and 359
		public static double ClampAngle(double angle)
		{
			angle %= 360;
			if (angle < 0) angle += 360;
			return angle;
		}

		//mxd
		public static int Random(int min, int max) 
		{
			return random.Next(min, max + 1); //because max is never rolled
		}

		//mxd
		public static float Random(float min, float max) 
		{
			return (float)Math.Round(min + (max - min) * random.NextDouble(), 2);
		}

		public static double Random(double min, double max)
		{
			return Math.Round(min + (max - min) * random.NextDouble(), 2);
		}

		// This returns an element from a collection by index
		public static T GetByIndex<T>(ICollection<T> collection, int index)
		{
			IEnumerator<T> e = collection.GetEnumerator();
			for(int i = -1; i < index; i++) e.MoveNext();
			return e.Current;
		}

		// This returns the next power of 2
		/*public static int NextPowerOf2(int v)
		{
			int p = 0;

			// Continue increasing until higher than v
			while(Math.Pow(2, p) < v) p++;

			// Return power
			return (int)Math.Pow(2, p);
		}*/

		//mxd. This returns the next power of 2. Taken from http://bits.stephan-brumme.com/roundUpToNextPowerOfTwo.html
		public static int NextPowerOf2(int x)
		{
			x--;
			x |= x >> 1;  // handle  2 bit numbers
			x |= x >> 2;  // handle  4 bit numbers
			x |= x >> 4;  // handle  8 bit numbers
			x |= x >> 8;  // handle 16 bit numbers
			x |= x >> 16; // handle 32 bit numbers
			x++;

			return x;
		}
		
		// Convert bool to integer
		internal static int Bool2Int(bool v)
		{
			return v ? 1 : 0;
		}

		// Convert integer to bool
		internal static bool Int2Bool(int v)
		{
			return (v != 0);
		}

		// This shows a message and logs the message
		public static DialogResult ShowErrorMessage(string message, MessageBoxButtons buttons)
		{
			return ShowErrorMessage(message, buttons, true);
		}

		// This shows a message and logs the message
		public static DialogResult ShowErrorMessage(string message, MessageBoxButtons buttons, bool log)
		{
			//mxd. Log the message?
			if(log) WriteLogLine(message);
			
			// Use normal cursor
			Cursor oldcursor = Cursor.Current;
			Cursor.Current = Cursors.Default;
			
			// Show message
			IWin32Window window = null;
			if((Form.ActiveForm != null) && Form.ActiveForm.Visible) window = Form.ActiveForm;
			DialogResult result = MessageBox.Show(window, message, Application.ProductName, buttons, MessageBoxIcon.Error);

			// Restore old cursor
			Cursor.Current = oldcursor;
			
			// Return result
			return result;
		}

		// This shows a message and logs the message
		public static DialogResult ShowWarningMessage(string message, MessageBoxButtons buttons)
		{
			return ShowWarningMessage(message, buttons, MessageBoxDefaultButton.Button1, true);
		}

		// This shows a message and logs the message
		public static DialogResult ShowWarningMessage(string message, MessageBoxButtons buttons, MessageBoxDefaultButton defaultbutton)
		{
			return ShowWarningMessage(message, buttons, defaultbutton, true);
		}

		// This shows a message and logs the message
		public static DialogResult ShowWarningMessage(string message, MessageBoxButtons buttons, MessageBoxDefaultButton defaultbutton, bool log)
		{
			//mxd. Log the message?
			if(log) WriteLogLine(message);

			// Use normal cursor
			Cursor oldcursor = Cursor.Current;
			Cursor.Current = Cursors.Default;

			// Show message
			IWin32Window window = null;
			if((Form.ActiveForm != null) && Form.ActiveForm.Visible) window = Form.ActiveForm;
			DialogResult result = MessageBox.Show(window, message, Application.ProductName, buttons, MessageBoxIcon.Warning, defaultbutton);

			// Restore old cursor
			Cursor.Current = oldcursor;

			// Return result
			return result;
		}

		// This shows the reference manual
		public static void ShowHelp(string pagefile)
		{
			ShowHelp(pagefile, HELP_FILE);
		}

		// This shows the reference manual
		public static void ShowHelp(string pagefile, string chmfile)
		{
			// Check if the file can be found in the root
			string filepathname = Path.Combine(apppath, chmfile);
			if(!File.Exists(filepathname))
			{
				// Check if the file exists in the plugins directory
				filepathname = Path.Combine(pluginspath, chmfile);
				if(!File.Exists(filepathname))
				{
					// Fail
					WriteLogLine("ERROR: Can't find the help file \"" + chmfile + "\"");
					return;
				}
			}
			
			// Show help file
			Help.ShowHelp(mainwindow, filepathname, HelpNavigator.Topic, pagefile);
		}

		// This returns a unique temp filename
		internal static string MakeTempFilename(string tempdir)
		{
			return MakeTempFilename(tempdir, "tmp");
		}

		// This returns a unique temp filename
		internal static string MakeTempFilename(string tempdir, string extension)
		{
			string filename;
			const string chars = "abcdefghijklmnopqrstuvwxyz1234567890";

			do
			{
				// Generate a filename
				filename = "";
				for(int i = 0; i < 8; i++) filename += chars[Random(0, chars.Length - 1)];
				filename = Path.Combine(tempdir, filename + "." + extension);
			}
			// Continue while file is not unique
			while(File.Exists(filename) || Directory.Exists(filename));

			// Return the filename
			return filename;
		}

		// This returns a unique temp directory name
		internal static string MakeTempDirname()
		{
			string dirname;
			const string chars = "abcdefghijklmnopqrstuvwxyz1234567890";

			do
			{
				// Generate a filename
				dirname = "";
				for(int i = 0; i < 8; i++) dirname += chars[Random(0, chars.Length - 1)];
				dirname = Path.Combine(temppath, dirname);
			}
			// Continue while file is not unique
			while(File.Exists(dirname) || Directory.Exists(dirname));

			// Return the filename
			return dirname;
		}

		// This shows an image in a panel either zoomed or centered depending on size
		public static void DisplayZoomedImage(Panel panel, Image image)
		{
			// Image not null?
			if(image != null)
			{
				// Set the image
				panel.BackgroundImage = image;

				// Display zoomed
				panel.BackgroundImageLayout = ImageLayout.Zoom;
			}
		}

		// This calculates the new rectangle when one is scaled into another keeping aspect ratio
		public static RectangleF MakeZoomedRect(Size source, RectangleF target)
		{
			return MakeZoomedRect(new SizeF(source.Width, source.Height), target);
		}

		// This calculates the new rectangle when one is scaled into another keeping aspect ratio
		public static RectangleF MakeZoomedRect(Size source, Rectangle target)
		{
			return MakeZoomedRect(new SizeF(source.Width, source.Height),
								  new RectangleF(target.Left, target.Top, target.Width, target.Height));
		}
		
		// This calculates the new rectangle when one is scaled into another keeping aspect ratio
		public static RectangleF MakeZoomedRect(SizeF source, RectangleF target)
		{
			float scale;
			
			// Image fits?
			if((source.Width <= target.Width) && (source.Height <= target.Height))
			{
				// Just center
				scale = 1.0f;
			}
			// Image is wider than tall?
			else if((source.Width - target.Width) > (source.Height - target.Height))
			{
				// Scale down by width
				scale = target.Width / source.Width;
			}
			else
			{
				// Scale down by height
				scale = target.Height / source.Height;
			}
			
			// Return centered and scaled
			return new RectangleF(target.Left + (target.Width - source.Width * scale) * 0.5f,
								  target.Top + (target.Height - source.Height * scale) * 0.5f,
								  source.Width * scale, source.Height * scale);
		}

		// This opens a URL in the default browser
		public static void OpenWebsite(string url)
		{
            // [ZZ] note: it may break. no idea why it was done like it was done.
            string url2 = url.ToLowerInvariant();
            if (!url2.StartsWith("http://") && !url2.StartsWith("https://") && !url2.StartsWith("ftp://") && !url2.StartsWith("mailto:"))
                return;
            System.Diagnostics.Process.Start(url);
            /*

			RegistryKey key = null;
			Process p = null;
			string browser;

			try
			{
				// Get the registry key where default browser is stored
				key = Registry.ClassesRoot.OpenSubKey(@"HTTP\shell\open\command", false);

				// Trim off quotes
				browser = key.GetValue(null).ToString().ToLower().Replace("\"", "");

				// String doesnt end in EXE?
				if(!browser.EndsWith("exe"))
				{
					// Get rid of everything after the ".exe"
					browser = browser.Substring(0, browser.LastIndexOf(".exe") + 4);
				}
			}
			finally
			{
				// Clean up
				if(key != null) key.Close();
			}

			try
			{
				// Fork a process
				p = new Process();
				p.StartInfo.FileName = browser;
				p.StartInfo.Arguments = url;
				p.Start();
			}
			catch(Exception) { }

			// Clean up
			if(p != null) p.Dispose();*/
        }
		
		// This returns the short path name for a file
		public static string GetShortFilePath(string longpath)
		{
#if NO_WIN32
			return longpath;
#else
			const int maxlen = 256;
			StringBuilder shortname = new StringBuilder(maxlen);
			GetShortPathName(longpath, shortname, maxlen);
			return shortname.ToString();
#endif
		}

		public static string GetLinuxFilePath(string longpath)
		{
			string linuxpath;
			linuxpath = longpath.Replace('\\', '/');
			string wineprefix = Environment.GetEnvironmentVariable("WINEPREFIX");

			if (linuxpath.Substring(0, 2) == "C:")
			{
				linuxpath = wineprefix + "/drive_c" + linuxpath.Substring(2);
			}
			else if (linuxpath.Substring(0,2) == "Z:")
			{
				linuxpath = linuxpath.Substring(2);
			}
			return linuxpath;
        }
		
		//mxd
		internal static ScriptConfiguration GetScriptConfiguration(ScriptType type)
		{
			if(type == ScriptType.ACS)
			{
				// Return map-defined compiler
				string compiler = (!string.IsNullOrEmpty(Map.Options.ScriptCompiler) ? Map.Options.ScriptCompiler : Map.ConfigSettings.DefaultScriptCompiler);
				foreach(KeyValuePair<string, ScriptConfiguration> group in scriptconfigs)
				{
					if(group.Key == compiler) return group.Value;
				}
			}
			else
			{
				// Just pick the first one from the list
				foreach(ScriptConfiguration cfg in scriptconfigs.Values)
				{
					if(cfg.ScriptType == type) return cfg;
				}
			}

			return null;
		}

		//mxd
		public static bool CheckWritePremissions(string path)
		{
            try
            {
                string testFile = path + "/GZDBWriteTest.tmp";
                if (File.Exists(testFile))
                    File.Delete(testFile);
                FileStream fs = File.OpenWrite(testFile);
                fs.Close();
                File.Delete(testFile);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

		public static List<Assembly> GetPluginAssemblies()
		{
			return plugins.GetPluginAssemblies();
		}
		
#endregion

#region ==================  mxd. Uncaught exceptions handling

		// In some cases the program can remain operational after these
		private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e) 
		{
			try 
			{
				// Try handling it in user-friendy way...
				ExceptionDialog dlg = new ExceptionDialog(e);
				dlg.Setup();
				if(dlg.ShowDialog() == DialogResult.Cancel) Terminate(false);
			} 
			catch
			{
				string exceptionmsg;

				// Try getting exception details...
				try { exceptionmsg = "Fatal Windows Forms error occurred: " + e.Exception.Message + "\n\nStack Trace:\n" + e.Exception.StackTrace; }
				catch(Exception exc) { exceptionmsg = "Failed to get initial exception details: " + exc.Message + "\n\nStack Trace:\n" + exc.StackTrace; }

				// Try logging it...
				try { WriteLogLine(exceptionmsg); } catch { }

				// Try displaying it to the user...
				try { MessageBox.Show(exceptionmsg, "Fatal Windows Forms Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); }
				finally { Process.GetCurrentProcess().Kill(); }
			}
		}

		// These are usually unrecoverable
		private static void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs e) 
		{
			try
			{
				// Try handling it in user-friendy way...
				ExceptionDialog dlg = new ExceptionDialog(e);
				dlg.Setup();
				if(dlg.ShowDialog() == DialogResult.Cancel) Terminate(false);
			}
			catch
			{
				string exceptionmsg;

				// Try getting exception details...
				try
				{
					Exception ex = (Exception)e.ExceptionObject;
					exceptionmsg = "Fatal Non-UI error:\n" + ex.Message + "\n\nStack Trace:\n" + ex.StackTrace;
				}
				catch(Exception exc)
				{
					exceptionmsg = "Failed to get initial exception details:\n" + exc.Message + "\n\nStack Trace:\n" + exc.StackTrace;
				}

				// Try logging it...
				try { WriteLogLine(exceptionmsg); } catch {}

				// Try displaying it to the user...
				try { MessageBox.Show(exceptionmsg, "Fatal Non-UI Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); }
				finally { Process.GetCurrentProcess().Kill(); }
			}
		}

#endregion

	}
}

