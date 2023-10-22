
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
using System.IO;
using System.Linq;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Actions;
using CodeImp.DoomBuilder.Compilers;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.Data.Scripting;
using CodeImp.DoomBuilder.Editing;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.GZBuilder.Data; //mxd
using CodeImp.DoomBuilder.IO;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.VisualModes;
using CodeImp.DoomBuilder.Windows;
using CodeImp.DoomBuilder.ZDoom.Scripting;


#endregion

namespace CodeImp.DoomBuilder 
{
	public sealed class MapManager : IDisposable
	{
		#region ================== Constants

		// Map header name in temporary file
		internal const string TEMP_MAP_HEADER = "TEMPMAP";
		internal const string BUILD_MAP_HEADER = "MAP01";
		public const string CONFIG_MAP_HEADER = "~MAP";
		public const string CONFIG_MAP_NAME_FORMAT_EPISODE = "ExMy";
		public const string CONFIG_MAP_NAME_FORMAT_NO_EPISODE = "MAPxy";
		private const int REPLACE_TARGET_MAP = -1; //mxd

		#endregion

		#region ================== Variables

		// Status
		private bool changed;
		private bool scriptschanged;
		private bool maploading; //mxd

		// Map information
		private string filetitle;
		private string filepathname;
		private string temppath;
		private string origmapconfigname; //mxd. Map configuration, which was used to open the map.

		// Main objects
		private MapSet map;
		private MapSetIO io;
		private MapOptions options;
		private ConfigurationInfo configinfo;
		private GameConfiguration config;
		private DataManager data;
		private RenderDevice graphics;
		private Renderer2D renderer2d;
		private Renderer3D renderer3d;
		private WADReader tempwadreader;
		private GridSetup grid;
		private UndoManager undoredo;
		private CopyPasteManager copypaste;
		private Launcher launcher;
		private ThingsFilter thingsfilter;
		private ScriptEditorForm scriptwindow;
		private List<CompilerError> errors;
		private VisualCamera visualcamera;

		//mxd
		private Dictionary<string, ScriptItem> namedscripts;
		private Dictionary<int, ScriptItem> numberedscripts;

		// Disposing
		private bool isdisposed;

		#endregion

		#region ================== Properties

		public string FilePathName { get { return filepathname; } }
		public string FileTitle { get { return filetitle; } internal set { filetitle = value; } } //mxd. Added setter
		public string TempPath { get { return temppath; } }
		public MapOptions Options { get { return options; } }
		public MapSet Map { get { return map; } }
		public DataManager Data { get { return data; } }
		public bool IsChanged { get { return changed | CheckScriptChanged(); } set { changed |= value; if(!maploading) General.MainWindow.UpdateMapChangedStatus(); } }
		public bool IsDisposed { get { return isdisposed; } }
		internal RenderDevice Graphics { get { return graphics; } }
		public IRenderer2D Renderer2D { get { return renderer2d; } }
		public IRenderer3D Renderer3D { get { return renderer3d; } }
		internal Renderer2D CRenderer2D { get { return renderer2d; } }
		internal Renderer3D CRenderer3D { get { return renderer3d; } }
		public GameConfiguration Config { get { return config; } }
		public ConfigurationInfo ConfigSettings { get { return configinfo; } }
		public GridSetup Grid { get { return grid; } }
		public UndoManager UndoRedo { get { return undoredo; } }
		internal CopyPasteManager CopyPaste { get { return copypaste; } }
		public IMapSetIO FormatInterface { get { return io; } }
		internal Launcher Launcher { get { return launcher; } }
		public ThingsFilter ThingsFilter { get { return thingsfilter; } }
		internal List<CompilerError> Errors { get { return errors; } }
		internal ScriptEditorForm ScriptEditor { get { return scriptwindow; } }
		public VisualCamera VisualCamera { get { return visualcamera; } set { visualcamera = value; } }
		public bool IsScriptsWindowOpen { get { return (scriptwindow != null) && !scriptwindow.IsDisposed; } }
		internal WADReader TemporaryMapFile { get { return tempwadreader; } } //mxd

		//mxd. Map format
		public bool UDMF { get { return config.UDMF; } }
		public bool HEXEN { get { return config.HEXEN; } }
		public bool DOOM { get { return config.DOOM; } }

		//mxd. Scripts
		internal Dictionary<string, ScriptItem> NamedScripts { get { return namedscripts; } }
		internal Dictionary<int, ScriptItem> NumberedScripts { get { return numberedscripts; } }

		public ViewMode ViewMode { get { return renderer2d.ViewMode; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		internal MapManager() 
		{
			// We have no destructor
			GC.SuppressFinalize(this);

			// Create temporary path
			temppath = General.MakeTempDirname();
			Directory.CreateDirectory(temppath);
			General.WriteLogLine("Temporary directory:  " + temppath);

			// Basic objects
			grid = new GridSetup();
			undoredo = new UndoManager();
			copypaste = new CopyPasteManager();
			launcher = new Launcher(this);
			thingsfilter = new NullThingsFilter();
			errors = new List<CompilerError>();

			//mxd
			numberedscripts = new Dictionary<int, ScriptItem>();
			namedscripts = new Dictionary<string, ScriptItem>(StringComparer.OrdinalIgnoreCase);
		}

		// Disposer
		public void Dispose()
		{
			// Not already disposed?
			if(!isdisposed) 
			{
				// Let the plugins know
				General.Plugins.OnMapCloseBegin();

				// Stop processing
				General.MainWindow.StopProcessing();

				// Close script editor
				CloseScriptEditor(false);

				// Change to no mode
				General.Editing.ChangeMode((EditMode)null);

				// Unbind any methods
				General.Actions.UnbindMethods(this);

				// Dispose
				maploading = true; //mxd
				if(grid != null) grid.Dispose();
				if(launcher != null) launcher.Dispose();
				if(copypaste != null) copypaste.Dispose();
				if(undoredo != null) undoredo.Dispose();
				General.WriteLogLine("Unloading data resources...");
				if(data != null) data.Dispose();
				General.WriteLogLine("Closing temporary file...");
				if(tempwadreader != null) tempwadreader.Dispose(); //mxd
				General.WriteLogLine("Unloading map data...");
				if(map != null) map.Dispose();
				General.WriteLogLine("Stopping graphics device...");
				if(renderer2d != null) renderer2d.Dispose();
				if(renderer3d != null) renderer3d.Dispose();
				if(graphics != null) graphics.Dispose();
				visualcamera = null;
				grid = null;
				launcher = null;
				copypaste = null;
				undoredo = null;
				data = null;
				tempwadreader = null; //mxd
				map = null;
				renderer2d = null;
				renderer3d = null;
				graphics = null;

				// We may spend some time to clean things up here
				GC.Collect();
				GC.WaitForPendingFinalizers(); //mxd
				GC.Collect(); //mxd

				// Remove temp file
				General.WriteLogLine("Removing temporary directory...");
				try
				{
					Directory.Delete(temppath, true);
				} 
				catch(Exception e) 
				{
					General.WriteLogLine(e.GetType().Name + ": " + e.Message);
					General.WriteLogLine("Failed to remove temporary directory!");
				}

				// Let the plugins know
				General.Plugins.OnMapCloseEnd();

				// Done
				isdisposed = true;
			}
		}

		#endregion

		#region ================== New / Open

		// Initializes for a new map
		internal bool InitializeNewMap(MapOptions options) 
		{
#if DEBUG
			DebugConsole.Clear();
#endif
			
			// Apply settings
			this.filetitle = options.CurrentName + ".wad";
			this.filepathname = string.Empty;
			this.maploading = true; //mxd
			this.changed = false;
			this.options = options;

			General.WriteLogLine("Creating new map \"" + options.CurrentName + "\" with configuration \"" + options.ConfigFile + "\"");

			// Initiate graphics
			General.WriteLogLine("Initializing graphics device...");
			graphics = new RenderDevice(General.MainWindow.Display);

			// Create renderers
			renderer2d = new Renderer2D(graphics);
			renderer3d = new Renderer3D(graphics);

			// Load game configuration
			General.WriteLogLine("Loading game configuration...");
			configinfo = General.GetConfigurationInfo(options.ConfigFile);
			config = new GameConfiguration(configinfo.Configuration); //mxd
			origmapconfigname = configinfo.Filename;//mxd
			configinfo.ApplyDefaults(config);
			General.Editing.UpdateCurrentEditModes();

			//mxd. Check if default script compiler is required
			if(string.IsNullOrEmpty(configinfo.DefaultScriptCompiler))
			{
				foreach(MapLumpInfo info in config.MapLumps.Values)
				{
					if(info.ScriptBuild)
					{
						General.ErrorLogger.Add(ErrorType.Error, "\"DefaultScriptCompiler\" property is not set in \"" + configinfo + "\" game configuration. The editor may fail to compile ACC scripts.");
						break;
					}
				}
			}

			// Create map data
			map = new MapSet();

			// Create temp wadfile
			DataLocation templocation = new DataLocation(DataLocation.RESOURCE_WAD, General.MakeTempFilename(temppath), false, false, false, null); //mxd
			General.WriteLogLine("Creating temporary file: " + templocation.location);
#if DEBUG
			tempwadreader = new WADReader(templocation, General.Map.Config, false, true);
#else
			try { tempwadreader = new WADReader(templocation, General.Map.Config, false, true); }
			catch(Exception e)
			{
				General.ShowErrorMessage("Error while creating a temporary wad file:\n" + e.GetType().Name + ": " + e.Message, MessageBoxButtons.OK);
				return false;
			}
#endif

			// Read the map from temp file
			General.WriteLogLine("Initializing map format interface " + config.FormatInterface + "...");
			io = MapSetIO.Create(config.FormatInterface, tempwadreader.WadFile, this);

			// Create required lumps
			General.WriteLogLine("Creating map data structures...");
			tempwadreader.WadFile.Insert(TEMP_MAP_HEADER, 0, 0);
			io.Write(map, TEMP_MAP_HEADER, 1);
			CreateRequiredLumps(tempwadreader.WadFile, TEMP_MAP_HEADER);

			// Load data manager
			General.WriteLogLine("Loading data resources...");
			data = new DataManager();
			data.Load(configinfo.Resources, options.Resources);

			// Update structures
			options.ApplyGridSettings();
			map.UpdateConfiguration();
			map.Update();
			thingsfilter.Update();

			namedscripts = new Dictionary<string, ScriptItem>(StringComparer.OrdinalIgnoreCase); //mxd
			numberedscripts = new Dictionary<int, ScriptItem>(); //mxd

			// Bind any methods
			General.Actions.BindMethods(this);

			// Set defaults
			this.visualcamera = new VisualCamera();
			General.Editing.ChangeMode(configinfo.StartMode);
			ClassicMode cmode = (General.Editing.Mode as ClassicMode);
			if(cmode != null) cmode.SetZoom(0.5f);
			renderer2d.SetViewMode((ViewMode)General.Settings.DefaultViewMode);
			General.Settings.SetDefaultThingFlags(config.DefaultThingFlags);

			// Success
			this.changed = false;
			this.maploading = false; //mxd
			General.WriteLogLine("Map creation done");
			General.MainWindow.UpdateMapChangedStatus(); //mxd
			return true;
		}

		// Initializes for an existing map
		internal bool InitializeOpenMap(string filepathname, MapOptions options) 
		{
			WAD mapwad;

#if DEBUG
			DebugConsole.Clear();
#endif

			// Apply settings
			this.filetitle = Path.GetFileName(filepathname);
			this.filepathname = filepathname;
			this.changed = false;
			this.maploading = true; //mxd
			this.options = options;

			General.WriteLogLine("Opening map \"" + options.CurrentName + "\" with configuration \"" + options.ConfigFile + "\"");

			// Initiate graphics
			General.WriteLogLine("Initializing graphics device...");
			graphics = new RenderDevice(General.MainWindow.Display);

			// Create renderers
			renderer2d = new Renderer2D(graphics);
			renderer3d = new Renderer3D(graphics);

			// Load game configuration
			General.WriteLogLine("Loading game configuration...");
			configinfo = General.GetConfigurationInfo(options.ConfigFile);
			config = new GameConfiguration(configinfo.Configuration);
			origmapconfigname = configinfo.Filename;//mxd
			configinfo.ApplyDefaults(config);
			General.Editing.UpdateCurrentEditModes();

			// Create map data
			map = new MapSet();

			// Create temp wadfile
			DataLocation templocation = new DataLocation(DataLocation.RESOURCE_WAD, General.MakeTempFilename(temppath), false, false, false, null); //mxd
			General.WriteLogLine("Creating temporary file: " + templocation.location);
#if DEBUG
			tempwadreader = new WADReader(templocation, General.Map.Config, false, true);
#else
			try { tempwadreader = new WADReader(templocation, General.Map.Config, false, true); } catch(Exception e) 
			{
				General.ShowErrorMessage("Error while creating a temporary wad file:\n" + e.GetType().Name + ": " + e.Message, MessageBoxButtons.OK);
				return false;
			}
#endif

			// Now open the map file
			General.WriteLogLine("Opening source file: " + filepathname);
#if DEBUG
			mapwad = new WAD(filepathname, true);
#else
			try { mapwad = new WAD(filepathname, true); } catch(Exception e) 
			{
				General.ShowErrorMessage("Error while opening source wad file:\n" + e.GetType().Name + ": " + e.Message, MessageBoxButtons.OK);
				return false;
			}
#endif

			// Copy the map lumps to the temp file
			General.WriteLogLine("Copying map lumps to temporary file...");
			CopyLumpsByType(mapwad, options.CurrentName, tempwadreader.WadFile, TEMP_MAP_HEADER, true, true, true, true);

			// Close the map file
			mapwad.Dispose();

			//mxd. Create MapSet
			bool maprestored;
			if(!CreateMapSet(map, filepathname, options, out maprestored)) return false;

			// Load data manager
			General.WriteLogLine("Loading data resources...");
			data = new DataManager();
			DataLocation maplocation = new DataLocation(DataLocation.RESOURCE_WAD, filepathname, options.StrictPatches, false, false, null);
			data.Load(configinfo.Resources, options.Resources, maplocation);

			// Remove unused sectors
			map.RemoveUnusedSectors(true);

			//mxd. Flip linedefs with only back side
			int flipsdone = MapSet.FlipBackwardLinedefs(map.Linedefs);
			if(flipsdone > 0) General.WriteLogLine(flipsdone + " single-sided linedefs were flipped.");

			// Update structures
			options.ApplyGridSettings();
			map.UpdateConfiguration();
			map.SnapAllToAccuracy();
			map.Update();
			thingsfilter.Update();

			//mxd. Update script names
			LoadACS();

			//mxd. Restore selection groups
			options.ReadSelectionGroups();

			// Bind any methods
			General.Actions.BindMethods(this);

			// Set defaults
			this.visualcamera = new VisualCamera();
			General.Editing.ChangeMode(configinfo.StartMode);
			renderer2d.SetViewMode((ViewMode)General.Settings.DefaultViewMode);
			General.Settings.SetDefaultThingFlags(config.DefaultThingFlags);

			// Center map in screen
			//if(General.Editing.Mode is ClassicMode) (General.Editing.Mode as ClassicMode).CenterInScreen();

			// Success
			this.changed = maprestored; //mxd
			this.maploading = false; //mxd
			General.WriteLogLine("Map loading done");
			General.MainWindow.UpdateMapChangedStatus(); //mxd
			return true;
		}

		//mxd. This switches to another map in the same wad 
		internal bool InitializeSwitchMap(MapOptions options)
		{
#if DEBUG
			DebugConsole.Clear();
#endif
			
			this.changed = false;
			this.maploading = true;
			this.options = options;

			// Create map data
			MapSet newmap = new MapSet();
			WAD mapwad;

			// Create temp wadfile
			DataLocation templocation = new DataLocation(DataLocation.RESOURCE_WAD, General.MakeTempFilename(temppath), false, false, false, null); //mxd
			General.WriteLogLine("Creating temporary file: " + templocation.location);
			if(tempwadreader != null) tempwadreader.Dispose();

#if DEBUG
			tempwadreader = new WADReader(templocation, General.Map.Config, false, true);
#else
			try { tempwadreader = new WADReader(templocation, General.Map.Config, false, true); } catch(Exception e) 
			{
				General.ShowErrorMessage("Error while creating a temporary wad file:\n" + e.GetType().Name + ": " + e.Message, MessageBoxButtons.OK);
				return false;
			}
#endif

			// Now open the map file
			General.WriteLogLine("Opening source file: " + filepathname);
			
#if DEBUG
			mapwad = new WAD(filepathname, true);
#else
			try { mapwad = new WAD(filepathname, true); } catch(Exception e) 
			{
				General.ShowErrorMessage("Error while opening source wad file:\n" + e.GetType().Name + ": " + e.Message, MessageBoxButtons.OK);
				return false;
			}
#endif

			// Copy the map lumps to the temp file
			General.WriteLogLine("Copying map lumps to temporary file...");
			CopyLumpsByType(mapwad, options.CurrentName, tempwadreader.WadFile, TEMP_MAP_HEADER, true, true, true, true);

			// Close the map file
			mapwad.Dispose();

			// Create MapSet
			bool maprestored;
			if(!CreateMapSet(newmap, filepathname, options, out maprestored)) return false;

			// And switch to it
			ChangeMapSet(newmap);

			// Sector textures may've been changed 
			data.UpdateUsedTextures();

			// This will update DataManager.mapinfo only
			data.ReloadMapInfoPartial();

			// Skybox may've been changed
			data.SetupSkybox();

			// Update script names
			LoadACS();

			// Restore selection groups
			options.ReadSelectionGroups();

			if(General.Editing.Mode != null)
			{
				if (General.Editing.Mode is ClassicMode)
				{
					ClassicMode mode = (ClassicMode)General.Editing.Mode;
					mode.OnRedoEnd();

					// biwa. Cancel current mode. This will re-engage non-volatile modes to make sure it's
					// properly initialized for the new map. Fixes issues mentioned in #196
					General.Editing.CancelMode();

					// Center map in screen or on stored coordinates
					if (options.ViewPosition.IsFinite() && !float.IsNaN(options.ViewScale))
						mode.CenterOnCoordinates(options.ViewPosition, options.ViewScale);
					else
						mode.CenterInScreen();
				}
				else if(General.Editing.Mode is VisualMode)
				{
					VisualMode mode = (VisualMode)General.Editing.Mode;

					// biwa. Cancel current mode. This will re-engage non-volatile modes to make sure it's
					// properly initialized for the new map. Fixes issues mentioned in #196
					General.Editing.CancelMode();

					// This will rebuild blockmap, among the other things
					General.Editing.Mode.OnReloadResources();

					// Update camera position
					if (options.ViewPosition.IsFinite()) mode.CenterOnCoordinates(options.ViewPosition);
				}
			}

			// Success
			this.changed = maprestored;
			this.maploading = false;
			General.WriteLogLine("Map switching done");
			General.MainWindow.UpdateMapChangedStatus();
			return true;
		}

		//mxd
		private bool CreateMapSet(MapSet newmap, string filepathname, MapOptions options, out bool maprestored)
		{
			maprestored = false;
			string wadname = Path.GetFileNameWithoutExtension(filepathname);
			if(!string.IsNullOrEmpty(wadname))
			{
				string hash = MurmurHash2.Hash(wadname + options.LevelName + File.GetLastWriteTime(filepathname)).ToString();
				string backuppath = Path.Combine(General.MapRestorePath, wadname + "." + hash + ".restore");

				// Backup exists and it's newer than the map itself?
				if(File.Exists(backuppath) && File.GetLastWriteTime(backuppath) > File.GetLastWriteTime(filepathname))
				{
					if(General.ShowWarningMessage("Looks like your previous editing session has gone terribly wrong." + Environment.NewLine
						   + "Would you like to restore the map from the backup?", MessageBoxButtons.YesNo) == DialogResult.Yes)
					{
						General.WriteLogLine("Initializing map format interface " + config.FormatInterface + "...");
						io = MapSetIO.Create(config.FormatInterface, tempwadreader.WadFile, this);
						General.WriteLogLine("Restoring map from \"" + backuppath + "\"...");
						
#if DEBUG
						// Restore map
						newmap.Deserialize(SharpCompressHelper.DecompressStream(new MemoryStream(File.ReadAllBytes(backuppath))));
#else
						try
						{
							// Restore map
							newmap.Deserialize(SharpCompressHelper.DecompressStream(new MemoryStream(File.ReadAllBytes(backuppath))));

							// Delete the backup
							File.Delete(backuppath);
						}
						catch(Exception e) 
						{
							General.ErrorLogger.Add(ErrorType.Error, "Unable to restore the map data structures from the backup. " + e.GetType().Name + ": " + e.Message);
							General.ShowErrorMessage("Unable to restore the map data structures from the backup.", MessageBoxButtons.OK);
							return false;
						}	
#endif
						maprestored = true;
					}
				}
			}

			// Read the map from temp file
			if(!maprestored)
			{
				newmap.BeginAddRemove();
				General.WriteLogLine("Initializing map format interface " + config.FormatInterface + "...");
				io = MapSetIO.Create(config.FormatInterface, tempwadreader.WadFile, this);
				General.WriteLogLine("Reading map data from file...");
#if DEBUG
				newmap = io.Read(newmap, TEMP_MAP_HEADER);
#else
				try { newmap = io.Read(newmap, TEMP_MAP_HEADER); } catch(Exception e) 
				{
					General.ErrorLogger.Add(ErrorType.Error, "Unable to read the map data with the specified configuration. " + e.GetType().Name + ": " + e.Message);
					General.ShowErrorMessage("Unable to read the map data with the specified configuration." + Environment.NewLine + Environment.NewLine + e.Message, MessageBoxButtons.OK);
					return false;
				}
#endif
				newmap.EndAddRemove();
			}

			return true;
		}

		#endregion

		#region ================== Save

		/// <summary>
		/// This exports the structures from memory into a WAD file with the current map format.
		/// </summary>
		public bool ExportToFile(string filepathname) 
		{
			General.Plugins.OnMapSaveBegin(SavePurpose.Testing);
			bool result = SaveMap(filepathname, SavePurpose.Testing);
			General.Plugins.OnMapSaveEnd(SavePurpose.Testing);
			return result;
		}

		/// <summary>
		/// This writes the map structures to the temporary file.
		/// </summary>
		private bool WriteMapToTempFile()
		{
			StatusInfo oldstatus = General.MainWindow.Status;

			// Make a copy of the map data
			MapSet outputset = map.Clone();

			// Remove all flags from all 3D Start things
			foreach(Thing t in outputset.Things)
			{
				if(t.Type == config.Start3DModeThingType)
				{
					// We're not using SetFlag here, this doesn't have to be undone.
					// Please note that this is totally exceptional!
					List<string> flagkeys = new List<string>(t.Flags.Keys);
					foreach(string k in flagkeys) t.Flags[k] = false;
				}
			}

			// Do we need sidedefs compression?
			if(map.Sidedefs.Count > io.MaxSidedefs)
			{
				// Compress sidedefs
				int initialsidescount = outputset.Sidedefs.Count; //mxd
				General.MainWindow.DisplayStatus(StatusType.Busy, "Compressing sidedefs...");
				outputset.CompressSidedefs();

				// Check if it still doesnt
				if(outputset.Sidedefs.Count > io.MaxSidedefs)
				{
					// Problem! Can't save the map like this!
					General.ShowErrorMessage("Unable to save the map: there are too many unique sidedefs!" + Environment.NewLine + Environment.NewLine
						+ "Sidedefs before compression: " + initialsidescount + Environment.NewLine
						+ "Sidedefs after compression: " + outputset.Sidedefs.Count
						+ " (" + (outputset.Sidedefs.Count - io.MaxSidedefs) + " sidedefs above the limit)", MessageBoxButtons.OK);
					General.MainWindow.DisplayStatus(oldstatus);
					return false;
				}
			}

			// Check things
			if(map.Things.Count > io.MaxThings)
			{
				General.ShowErrorMessage("Unable to save the map: There are too many things!", MessageBoxButtons.OK);
				General.MainWindow.DisplayStatus(oldstatus);
				return false;
			}

			// Check sectors
			if(map.Sectors.Count > io.MaxSectors)
			{
				General.ShowErrorMessage("Unable to save the map: There are too many sectors!", MessageBoxButtons.OK);
				General.MainWindow.DisplayStatus(oldstatus);
				return false;
			}

			// Check linedefs
			if(map.Linedefs.Count > io.MaxLinedefs)
			{
				General.ShowErrorMessage("Unable to save the map: There are too many linedefs!", MessageBoxButtons.OK);
				General.MainWindow.DisplayStatus(oldstatus);
				return false;
			}

			// Check vertices
			if(map.Vertices.Count > io.MaxVertices)
			{
				General.ShowErrorMessage("Unable to save the map: There are too many vertices!", MessageBoxButtons.OK);
				General.MainWindow.DisplayStatus(oldstatus);
				return false;
			}

			// TODO: Check for more limitations

			// Write to temporary file
			General.WriteLogLine("Writing map data structures to file...");
			int index = Math.Max(0, tempwadreader.WadFile.FindLumpIndex(TEMP_MAP_HEADER));
			io.Write(outputset, TEMP_MAP_HEADER, index);
			outputset.Dispose();

			General.MainWindow.DisplayStatus(oldstatus);
			return true;
		}

		// Initializes for an existing map
		internal bool SaveMap(string newfilepathname, SavePurpose purpose) 
		{
			string settingsfile;
			WAD targetwad = null;
			bool includenodes;
			bool fileexists = File.Exists(newfilepathname); //mxd

			General.WriteLogLine("Saving map to file: " + newfilepathname);

			//mxd. Official IWAD check...
			if(fileexists)
			{
				WAD hashtest = new WAD(newfilepathname, true);
				if(hashtest.IsOfficialIWAD)
				{
					General.WriteLogLine("Map saving aborted: attempt to modify an official IWAD");
					General.ShowErrorMessage("Official IWADs should not be modified.\nConsider making a PWAD instead", MessageBoxButtons.OK);
					return false;
				}

				hashtest.Dispose();
			}

			// Scripts changed?
			bool localscriptschanged = CheckScriptChanged();

			// If the scripts window is open, save the scripts first
			if(IsScriptsWindowOpen) scriptwindow.Editor.ImplicitSave();

			// Only recompile scripts when the scripts have changed or there are compiler errors (mxd)
			// (not when only the map changed)
			if((localscriptschanged || errors.Count > 0) && !CompileScriptLumps()) 
			{
				// Compiler failure
				if(errors.Count > 0)
					General.ShowErrorMessage("Error while compiling scripts: " + errors[0].description, MessageBoxButtons.OK);
				else
					General.ShowErrorMessage("Unknown compiler error while compiling scripts!", MessageBoxButtons.OK);
			}

			// Show script window if there are any errors and we are going to test the map
			// and always update the errors on the scripts window.
			if((errors.Count > 0) && (scriptwindow == null) && (purpose == SavePurpose.Testing)) ShowScriptEditor();
			if(scriptwindow != null) scriptwindow.Editor.ShowErrors(errors, false);

			// Only write the map and rebuild nodes when the actual map has changed
			// (not when only scripts have changed)
			if(changed) 
			{
				// Write the current map structures to the temp file
				if(!WriteMapToTempFile()) return false;

				// Get the corresponding nodebuilder
				string nodebuildername = (purpose == SavePurpose.Testing) ? configinfo.NodebuilderTest : configinfo.NodebuilderSave;

				// Build the nodes
				StatusInfo oldstatus = General.MainWindow.Status;
				General.MainWindow.DisplayStatus(StatusType.Busy, "Building map nodes...");
				includenodes = (!string.IsNullOrEmpty(nodebuildername) && BuildNodes(nodebuildername, true));
				General.MainWindow.DisplayStatus(oldstatus);

				//mxd. Compress temp file...
				tempwadreader.WadFile.Compress();
			}
			else 
			{
				// Check if we have nodebuilder lumps
				includenodes = VerifyNodebuilderLumps(tempwadreader.WadFile, TEMP_MAP_HEADER);
			}

			//mxd. Target file is read-only?
			if(fileexists)
			{
				FileInfo info = new FileInfo(newfilepathname);
				if(info.IsReadOnly)
				{
					if(General.ShowWarningMessage("Unable to save the map: target file is read-only.\nRemove read-only flag and save the map anyway?", MessageBoxButtons.YesNo) == DialogResult.Yes)
					{
						General.WriteLogLine("Removing read-only flag from the map file...");
						try
						{
							info.IsReadOnly = false;
						}
						catch(Exception e)
						{
							General.ShowErrorMessage("Failed to remove read-only flag from \"" + filepathname + "\":" + Environment.NewLine + Environment.NewLine + e.Message, MessageBoxButtons.OK);
							General.WriteLogLine("Failed to remove read-only flag from \"" + filepathname + "\":" + e.Message);
							return false;
						}
					}
					else
					{
						General.WriteLogLine("Map saving cancelled...");
						return false;
					}
				}
			}

			// Suspend data resources
			data.Suspend();

			//mxd. Check if the target file is locked
			if(fileexists)
			{
				FileLockChecker.FileLockCheckResult checkresult = FileLockChecker.CheckFile(newfilepathname);
				if(!string.IsNullOrEmpty(checkresult.Error))
				{
					if(checkresult.Processes.Count > 0)
					{
						string rest = Environment.NewLine + "Press 'Retry' to close " + (checkresult.Processes.Count > 1 ? "all processes" : "the process")
							+ " and retry." + Environment.NewLine + "Press 'Cancel' to cancel saving.";

						if(General.ShowErrorMessage(checkresult.Error + rest, MessageBoxButtons.RetryCancel) == DialogResult.Retry)
						{
							// Close all processes
							foreach(Process process in checkresult.Processes)
							{
								try
								{
									if(!process.HasExited) process.Kill();
								}
								catch(Exception e)
								{
									General.ShowErrorMessage("Failed to close " + Path.GetFileName(process.MainModule.FileName) + ":" + Environment.NewLine + Environment.NewLine + e.Message, MessageBoxButtons.OK);
									data.Resume();
									General.WriteLogLine("Map saving failed: failed to close " + Path.GetFileName(process.MainModule.FileName));
									return false;
								}
							}

							// Retry
							data.Resume();
							General.WriteLogLine("Map saving restarted...");
							return SaveMap(newfilepathname, purpose);
						}
						else
						{
							data.Resume();
							General.WriteLogLine("Map saving cancelled...");
							return false;
						}
					}
					else
					{
						General.ShowErrorMessage(checkresult.Error, MessageBoxButtons.OK);
						data.Resume();
						General.WriteLogLine("Map saving failed: " + checkresult.Error);
						return false;
					}
				}
			}

			// Determine original map name
			string origmapname = (!string.IsNullOrEmpty(options.PreviousName) && purpose != SavePurpose.IntoFile) ? options.PreviousName : options.CurrentName;
			string origwadfile = string.Empty; //mxd

			try 
			{
				if(fileexists) 
				{
					// mxd. Check if target wad already has a map with the same name
					if(purpose == SavePurpose.IntoFile) 
					{
						WAD wad = new WAD(newfilepathname, true);
                        int mapindex = FindAndRemoveMap(wad, origmapname, false);
						wad.Dispose();

						if(mapindex != -1 && MessageBox.Show(General.MainWindow, "Target file already contains map \"" + origmapname + "\"\nDo you want to replace it?", "Map already exists!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No) 
						{
							data.Resume();
							General.WriteLogLine("Map saving cancelled...");
							return false;
						}
					}

					// Backup existing file, if any
					if(File.Exists(newfilepathname + ".backup3")) File.Delete(newfilepathname + ".backup3");
					if(File.Exists(newfilepathname + ".backup2")) File.Move(newfilepathname + ".backup2", newfilepathname + ".backup3");
					if(File.Exists(newfilepathname + ".backup1")) File.Move(newfilepathname + ".backup1", newfilepathname + ".backup2");
					File.Copy(newfilepathname, newfilepathname + ".backup1");
				}

				// Except when saving INTO another file,
				// kill the target file if it is different from source file
				if((purpose != SavePurpose.IntoFile) && (newfilepathname != filepathname)) 
				{
					// Kill target file
					if(File.Exists(newfilepathname)) File.Delete(newfilepathname);

					// Kill .dbs settings file
					settingsfile = newfilepathname.Substring(0, newfilepathname.Length - 4) + ".dbs";
					if(File.Exists(settingsfile)) File.Delete(settingsfile);
				}

                // [ZZ] We really want to tell apart saving into current archive from saving into a new one.
                //      Treat "save as" into the current archive as normal save.
                bool isSaveAs = (purpose == SavePurpose.AsNewFile) && (newfilepathname != filepathname);
				// On Save AS we have to copy the previous file to the new file
				if (isSaveAs)
				{
					// Copy if original file still exists
					if (File.Exists(filepathname))
						File.Copy(filepathname, newfilepathname, true);
				}

				if (File.Exists(newfilepathname))
				{
					// If the target file exists, we need to rebuild it
					// biwa. This somewhat restores the behavior that was change with commit b996d8a380056ed9cb3a3f90f3c534e29e77e53c
					// Not rebuilding the WAD can cause issues with WADs that do not have their directory at the bottom.
					// See https://github.com/jewalky/UltimateDoomBuilder/issues/531
					origwadfile = newfilepathname + ".temp";
					File.Move(newfilepathname, origwadfile);

					// Open original file
					WAD origwad = new WAD(origwadfile, true);

					// Create new target file
					targetwad = new WAD(newfilepathname) { IsIWAD = origwad.IsIWAD }; //mxd. Let's preserve wad type

					CopyAllLumps(origwad, targetwad);

					// Close original file and delete it
					origwad.Dispose();
					File.Delete(origwadfile);
				}
				else
				{
					// Create new target file
					targetwad = new WAD(newfilepathname);
				}
			} 
			catch(Exception e) 
			{
				General.ShowErrorMessage("Unable to write the map to target file \"" + newfilepathname + "\":\n" + e.Message, MessageBoxButtons.OK);
				if(!string.IsNullOrEmpty(origwadfile) && File.Exists(origwadfile))
				{
					//mxd. Clean-up
					if(File.Exists(newfilepathname))
					{
						//mxd. We MAY've just deleted the map from the target file. Let's pretend this never happened
						if(targetwad != null) targetwad.Dispose();
						File.Delete(newfilepathname);
						File.Move(origwadfile, newfilepathname);
					}
					else
					{
						File.Delete(origwadfile); 
					}
				}

				data.Resume();
				General.WriteLogLine("Map saving failed: " + e.Message);
				return false;
			}

			// Copy map lumps to target file
			CopyLumpsByType(tempwadreader.WadFile, TEMP_MAP_HEADER, targetwad, origmapname, true, true, includenodes, true, true);

			// mxd. Was the map renamed?
			if(options.LevelNameChanged) 
			{
				if(purpose != SavePurpose.IntoFile) 
				{
					General.WriteLogLine("Changing map name from \"" + options.PreviousName + "\" to \"" + options.CurrentName + "\"");

					// Find the map header in target
					int index = targetwad.FindLumpIndex(options.PreviousName);
					if(index > -1) 
					{
						// Rename the map lump name
						targetwad.Lumps[index].Rename(options.CurrentName);
					} 
					else 
					{
						// Houston, we've got a problem!
						General.ShowErrorMessage("Error renaming map lump name: the original map lump could not be found!", MessageBoxButtons.OK);
						options.CurrentName = options.PreviousName;
					}
				}

				options.PreviousName = string.Empty;
			}

			// Done with the target file
			targetwad.Dispose();

			// Resume data resources
			data.Resume();

			// Not saved for testing purpose?
			if(purpose != SavePurpose.Testing) 
			{
				// Saved in a different file?
				if(newfilepathname != filepathname) 
				{
					// Keep new filename
					filepathname = newfilepathname;
					filetitle = Path.GetFileName(filepathname);

					// Reload resources
					ReloadResources(true, false);
				}

				try 
				{
					// Open or create the map settings
					settingsfile = newfilepathname.Substring(0, newfilepathname.Length - 4) + ".dbs";
					options.WriteConfiguration(settingsfile);
				} 
				catch(Exception e) 
				{
					// Warning only
					General.ErrorLogger.Add(ErrorType.Warning, "Could not write the map settings configuration file. " + e.GetType().Name + ": " + e.Message);
				}

				// Changes saved
				changed = false;
				scriptschanged = false;
			}

			// Success!
			General.WriteLogLine("Map saving done");
			General.MainWindow.UpdateMapChangedStatus(); //mxd
			return true;
		}

		//mxd. Don't save the map if it was not changed
		internal bool MapSaveRequired(string newfilepathname, SavePurpose purpose)
		{
			return (changed || scriptschanged || CheckScriptChanged() || options.LevelNameChanged || newfilepathname != filepathname || purpose != SavePurpose.Normal);
		}

		//mxd. Saves .dbs file
		internal bool SaveSettingsFile(string newfilepathname)
		{
			try 
			{
				string settingsfile = newfilepathname.Substring(0, newfilepathname.Length - 4) + ".dbs";
				options.WriteConfiguration(settingsfile);
			} 
			catch(Exception e) 
			{
				// Warning only
				General.ErrorLogger.Add(ErrorType.Warning, "Could not write the map settings configuration file. " + e.GetType().Name + ": " + e.Message);
				return false;
			}

			return true;
		}

		//mxd
		internal void SaveMapBackup()
		{
			if(isdisposed || map == null || map.IsDisposed || string.IsNullOrEmpty(filepathname) || options == null)
			{
				General.WriteLogLine("Map backup saving failed: required structures already disposed...");
				return;
			}

#if !DEBUG
			try
			{
#endif
				string wadname = Path.GetFileNameWithoutExtension(filepathname);
				if(!string.IsNullOrEmpty(wadname))
				{
					// Make backup file path
					if(!Directory.Exists(General.MapRestorePath)) Directory.CreateDirectory(General.MapRestorePath);
					string hash = MurmurHash2.Hash(wadname + options.LevelName + File.GetLastWriteTime(filepathname)).ToString();
					string backuppath = Path.Combine(General.MapRestorePath, wadname + "." + hash + ".restore");

					// Export map
					MemoryStream ms = map.Serialize();
					ms.Seek(0, SeekOrigin.Begin);
					File.WriteAllBytes(backuppath, SharpCompressHelper.CompressStream(ms).ToArray());

					// Log it
					General.WriteLogLine("Map backup saved to \"" + backuppath + "\"");
				}
				else
				{
					// Log it
					General.WriteLogLine("Map backup saving failed: invalid map WAD name");
				}
#if !DEBUG
			}
			catch(Exception e)
			{
				// Log it
				General.WriteLogLine("Map backup saving failed: " + e.Source + ": " + e.Message);
			}
#endif
		}

		#endregion

		#region ================== Nodebuild

		/// <summary>
		/// This stores the current structures in memory to the temporary file and rebuilds the nodes.
		/// The 'nodebuildername' must be a valid nodebuilder configuration profile.
		/// Returns True on success, False when failed.
		/// </summary>
		public bool RebuildNodes(string nodebuildername, bool failaswarning)
		{
			bool result;

			// Write the current map structures to the temp file
			if(!WriteMapToTempFile()) return false;

			// Build the nodes
			StatusInfo oldstatus = General.MainWindow.Status;
			General.MainWindow.DisplayStatus(StatusType.Busy, "Building map nodes...");
			if(!string.IsNullOrEmpty(nodebuildername))
				result = BuildNodes(nodebuildername, failaswarning);
			else
				result = false;
			General.MainWindow.DisplayStatus(oldstatus);

			//mxd. Compress temp file...
			tempwadreader.WadFile.Compress();

			return result;
		}

		// This builds the nodes in the temproary file with the given configuration name
		private bool BuildNodes(string nodebuildername, bool failaswarning) 
		{
			bool lumpscomplete = false;
			WAD buildwad;

			// Find the nodebuilder
			NodebuilderInfo nodebuilder = General.GetNodebuilderByName(nodebuildername);
			if(nodebuilder == null) 
			{
				// Problem! Can't find that nodebuilder!
				General.ShowWarningMessage("Unable to build the nodes: The configured nodebuilder cannot be found.\nPlease check your game configuration settings!", MessageBoxButtons.OK);
				return false;
			}
			else 
			{
				// Create the compiler interface that will run the nodebuilder
				// This automatically creates a temporary directory for us
				Compiler compiler = nodebuilder.CreateCompiler();

				// Make temporary filename
				string tempfile1 = General.MakeTempFilename(compiler.Location);

				// Make the temporary WAD file
				General.WriteLogLine("Creating temporary build file: " + tempfile1);
#if DEBUG
				buildwad = new WAD(tempfile1);
#else
				try { buildwad = new WAD(tempfile1); }
				catch(Exception e)
				{
					General.ShowErrorMessage("Error while creating a temporary wad file:\n" + e.GetType().Name + ": " + e.Message, MessageBoxButtons.OK);
					return false;
				}
#endif

				// Determine source file
				string sourcefile = (filepathname.Length > 0 ? filepathname : tempwadreader.WadFile.Filename);

				//mxd.
				RemoveUnneededLumps(tempwadreader.WadFile, TEMP_MAP_HEADER, true);

				// Copy lumps to buildwad
				General.WriteLogLine("Copying map lumps to temporary build file...");
				CopyLumpsByType(tempwadreader.WadFile, TEMP_MAP_HEADER, buildwad, BUILD_MAP_HEADER, true, false, false, true);

				// Close buildwad
				buildwad.Dispose();

				// Does the nodebuilder require an output file?
				string tempfile2;
				if(nodebuilder.HasSpecialOutputFile) 
				{
					// Make a temporary output file for the nodebuilder
					tempfile2 = General.MakeTempFilename(compiler.Location);
					General.WriteLogLine("Temporary output file: " + tempfile2);
				}
				else 
				{
					// Output file is same as input file
					tempfile2 = tempfile1;
				}

				// Run the nodebuilder
				compiler.Parameters = nodebuilder.Parameters;
				compiler.InputFile = Path.GetFileName(tempfile1);
				compiler.OutputFile = Path.GetFileName(tempfile2);
				compiler.SourceFile = sourcefile;
				compiler.WorkingDirectory = Path.GetDirectoryName(tempfile1);
				if(compiler.Run()) 
				{
					// Open the output file
					try { buildwad = new WAD(tempfile2); } 
					catch(Exception e) 
					{
						General.WriteLogLine(e.GetType().Name + " while reading build wad file: " + e.Message);
						buildwad = null;
					}

					if(buildwad != null) 
					{
						// Output lumps complete?
						lumpscomplete = VerifyNodebuilderLumps(buildwad, BUILD_MAP_HEADER);
					}

					if(lumpscomplete) 
					{
						// Copy nodebuilder lumps to temp file
						General.WriteLogLine("Copying nodebuilder lumps to temporary file...");
						CopyLumpsByType(buildwad, BUILD_MAP_HEADER, tempwadreader.WadFile, TEMP_MAP_HEADER, false, false, true, false);
					}
					else 
					{
						//mxd. collect errors
						string compilererrors = string.Empty;
						foreach(CompilerError e in compiler.Errors)
							compilererrors += Environment.NewLine + e.description;

						// Nodebuilder did not build the lumps!
						if(failaswarning)
							General.ShowWarningMessage("Unable to build the nodes: The nodebuilder failed to build the expected data structures.\nThe map will be saved without the nodes." + (compiler.Errors.Length > 0 ? Environment.NewLine + compilererrors : string.Empty), MessageBoxButtons.OK);
						else
							General.ShowErrorMessage("Unable to build the nodes: The nodebuilder failed to build the expected data structures." + (compiler.Errors.Length > 0 ? Environment.NewLine + compilererrors : string.Empty), MessageBoxButtons.OK);
					}

					// Done with the build wad
					if(buildwad != null) buildwad.Dispose();
				}
				else //mxd
				{
					//collect errors
					string compilererrors = string.Empty;
					foreach(CompilerError e in compiler.Errors)
						compilererrors += Environment.NewLine + e.description;

					// Nodebuilder did not build the lumps!
					General.ShowErrorMessage("Unable to build the nodes: The nodebuilder failed to build the expected data structures" + (compiler.Errors.Length > 0 ? ":" + Environment.NewLine + compilererrors : "."), MessageBoxButtons.OK);
				}

				// Clean up
				compiler.Dispose();

				// Let the plugins know
				if(lumpscomplete) General.Plugins.OnMapNodesRebuilt();

				// Return result
				return lumpscomplete;
			}
		}

		// This verifies if the nodebuilder lumps exist in a WAD file
		private bool VerifyNodebuilderLumps(WAD wad, string mapheader) 
		{
			bool lumpscomplete = false;

			// Find the map header in source
			int srcindex = wad.FindLumpIndex(mapheader);
			
			if(srcindex > -1) 
			{
				// Go for all the map lump names
				lumpscomplete = true;
				
				foreach(KeyValuePair<string, MapLumpInfo> group in config.MapLumps) 
				{
					// Check if this lump should exist
					if(group.Value.NodeBuild && !group.Value.AllowEmpty && group.Value.Required) 
					{
						//mxd
						string lumpname = group.Key;
						if(lumpname.Contains(CONFIG_MAP_HEADER)) lumpname = lumpname.Replace(CONFIG_MAP_HEADER, mapheader);
						
						// Find the lump in the source
						if(wad.FindLump(lumpname, srcindex, srcindex + config.MapLumps.Count + 2) == null) 
						{
							// Missing a lump!
							lumpscomplete = false;
							break;
						}
					}
				}
			}

			return lumpscomplete;
		}

		#endregion

		#region ================== Lumps

		// This returns a copy of the requested lump stream data
		// This is copied from the temp wad file and returns null when the lump is not found
		public MemoryStream GetLumpData(string lumpname) 
		{
			Lump l = tempwadreader.WadFile.FindLump(lumpname);
			if(l != null) 
			{
				l.Stream.Seek(0, SeekOrigin.Begin);
				return new MemoryStream(l.Stream.ReadAllBytes());
			}
			return null;
		}

		// This writes a copy of the data to a lump in the temp file
		public void SetLumpData(string lumpname, MemoryStream lumpdata) 
		{
			int insertindex = tempwadreader.WadFile.Lumps.Count;

			// Remove the lump if it already exists
			int li = tempwadreader.WadFile.FindLumpIndex(lumpname);
			if(li > -1)
			{
				insertindex = li;
				tempwadreader.WadFile.RemoveAt(li);
			}

			// Insert new lump
			Lump l = tempwadreader.WadFile.Insert(lumpname, insertindex, (int)lumpdata.Length);
			l.Stream.Seek(0, SeekOrigin.Begin);
			lumpdata.WriteTo(l.Stream);

			//mxd. Mark the map as changed (will also update the title)
			IsChanged = true;
		}

		// This checks if the specified lump exists in the temp file
		public bool LumpExists(string lumpname) 
		{
			return (tempwadreader.WadFile.FindLumpIndex(lumpname) > -1);
		}

		// This creates empty lumps for those required
		private void CreateRequiredLumps(WAD target, string mapname) 
		{
			// Find the map header in target
			int headerindex = target.FindLumpIndex(mapname);
			if(headerindex == -1) 
			{
				// If this header doesnt exists in the target
				// then insert at the end of the target
				headerindex = target.Lumps.Count;
			}

			// Begin inserting at target header index
			int insertindex = headerindex;

			// Go for all the map lump names
			foreach(KeyValuePair<string, MapLumpInfo> group in config.MapLumps)
			{
				// Check if this lump is required
				if(group.Value.Required) 
				{
					// Get the lump name
					string lumpname = (group.Key.Contains(CONFIG_MAP_HEADER) ? group.Key.Replace(CONFIG_MAP_HEADER, mapname) : group.Key); //mxd

					// Check if the lump is missing at the target
					int targetindex = FindSpecificLump(target, lumpname, headerindex, mapname, config.MapLumps);
					if(targetindex == -1) 
					{
						// Determine target index
						insertindex++;
						if(insertindex > target.Lumps.Count) insertindex = target.Lumps.Count;

						// Create new, emtpy lump
						General.WriteLogLine(lumpname + " is required! Created empty lump.");
						target.Insert(lumpname, insertindex, 0, false);
					}
					else
					{
						// Move insert index
						insertindex = targetindex;
					}
				}
			}

			target.WriteHeaders(); //mxd
		}

		//mxd. This is called on tempwad, which should only have the current map inside it.
		private void RemoveUnneededLumps(WAD target, string mapname, bool glnodesonly) 
		{
			//Get the list of lumps required by current map format
			List<string> requiredLumps = new List<string>();
			foreach(KeyValuePair<string, MapLumpInfo> group in config.MapLumps) 
			{
				//this lump well be recreated by a nodebuilder when saving the map 
				//(or it won't be if the new map format or nodebuilder doesn't require / build this lump, 
				//so it will just stay there, possibly messing things up)
				if(group.Value.NodeBuild && (!glnodesonly || group.Key.ToUpperInvariant().StartsWith("GL_"))) continue; 

				string lumpname = group.Key;
				if(lumpname == CONFIG_MAP_HEADER) lumpname = mapname;
				requiredLumps.Add(lumpname);
			}

			//Remove lumps, which are not required
			List<Lump> toRemove = new List<Lump>();
			foreach(Lump lump in target.Lumps)
				if(!requiredLumps.Contains(lump.Name)) toRemove.Add(lump);

			foreach(Lump lump in toRemove) target.Remove(lump);
		}

        // [ZZ] this is the more specific case of MatchConfiguration from OpenMapOptionsForm.
        //      that code is not possible to reuse for this purpose though.
        private int FindAndRemoveMap(WAD target, string targetmapname, bool reallyremove)
        {
            int tgtheaderindex = -1;
            int lastindex = -1;
            
            bool writeheaders = false;
            // [ZZ] Detect and delete target map from the target archive. Note that it can be in different format compared to current one, and there can be multiple maps with the same name as well.
            //
            while (true)
            {
                int nextindex = target.FindLumpIndex(targetmapname, lastindex+1);
                // note that the original algorithm would break if you have a patch or a texture named MAP01 for example...
                // this is the case for multiple megawads that have level selection screen (Duel40-style), but luckily most of them are using the PK3 format.
                // we are trying to detect if this is a valid map and shouldn't try to open or save over something that uses the map name as well.
                if (nextindex < 0) break; // next lump not found
                lastindex = nextindex;

                // try to detect the format used for this map.
                // if more than one format matches, do... idk what actually.
                // todo: move this code out and call it something like DetectMapConfiguration
                List<List<MapLumpInfo>> trylists = new List<List<MapLumpInfo>>();
                foreach (ConfigurationInfo cinfo in General.Configs)
                {
                    List<MapLumpInfo> maplumps = new List<MapLumpInfo>();
                    // parse only the map lumps section of the config.
                    Configuration cfg = cinfo.Configuration;
                    IDictionary dic = cfg.ReadSetting("maplumpnames", new Hashtable());
                    foreach (string k in dic.Keys)
                        maplumps.Add(new MapLumpInfo(k, cfg));

                    // check if we already have this lump list. don't duplicate.
                    bool found = false;
                    foreach (List<MapLumpInfo> ctrylist in trylists)
                    {
                        if (ctrylist.Count == maplumps.Count &&
                            ctrylist.SequenceEqual(maplumps))
                        {
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                        trylists.Add(maplumps);
                }

                // currently outputs 6 in 43. seems to work correctly.
                General.WriteLogLine(string.Format("Found {0} different map lump lists in the {1} registered game configurations", trylists.Count, General.Configs.Count));

                // find the most probable lump list.
                int maxmatches = 0;
                List<MapLumpInfo> trylist = null;
                foreach (List<MapLumpInfo> lst in trylists)
                {
                    int matches = 0;
                    int maxcnt = lst.Count;
                    int checkindex = nextindex+1;
                    for (int i = 0; i < lst.Count; i++)
                    {
                        if (checkindex >= target.Lumps.Count)
                            break;
                        int mliIdx = lst.FindIndex(e => e.Name == target.Lumps[checkindex].Name);
                        if (mliIdx < 0) break; // stop matching on first non-matching lump
                        MapLumpInfo mli = lst[mliIdx];
                        if (mli.Forbidden)
                        {
                            matches = 0;
                            break; // completely stop matching on first forbidden lump - definitely not this configuration
                        }

                        matches++;
                        checkindex++;
                    }

                    if (matches > maxmatches)
                    {
                        trylist = lst;
                        maxmatches = matches;
                    }
                }

                // if we didn't find anything it's weird...
                if (trylist != null)
                {
                    if (reallyremove)
                    {
                        int checkindex = nextindex + 1;
                        for (int i = 0; i < trylist.Count; i++)
                        {
                            if (checkindex >= target.Lumps.Count)
                                break;
                            bool match = (trylist.FindIndex(e => e.Name == target.Lumps[checkindex].Name) >= 0);
                            if (match) target.RemoveAt(checkindex);
                            else break; // stop deleting on first non-matching lump
                        }

                        // now if trylist was not null, we need to remove the header as well.
                        target.RemoveAt(nextindex, false);
                        writeheaders = true;
                        lastindex--;
                    }

                    tgtheaderindex = nextindex;
                }
            }

            if (writeheaders) target.WriteHeaders();
            return tgtheaderindex;
        }

		/// <summary>
		/// Copies all lumps from one WAD to another
		/// </summary>
		/// <param name="source">Source WAD</param>
		/// <param name="target">Target WAD</param>
		private static void CopyAllLumps(WAD source, WAD target)
		{
			// Go for all lumps
			for (int i = 0; i < source.Lumps.Count; i++)
			{
				Lump srclump = source.Lumps[i];

				// Copy lump over!
				Lump tgtlump = target.Insert(srclump.Name, target.Lumps.Count, srclump.Length, false);
				srclump.CopyTo(tgtlump);
			}

			target.WriteHeaders();
		}

		// This copies specific map lumps from one WAD to another
		private void CopyLumpsByType(WAD source, string sourcemapname,
									 WAD target, string targetmapname,
									 bool copyrequired, bool copyblindcopy,
									 bool copynodebuild, bool copyscript,
                                     bool deleteold = false) // [ZZ] deleteold=true means we need to delete the old map in-place. we only call this in SaveMap()
		{
            int tgtheaderindex = FindAndRemoveMap(target, targetmapname, deleteold); // this is unobvious, but this function also performs search, not just deletion :)
            bool replacetargetmap = !(deleteold && (tgtheaderindex > -1)); // this is unobvious, but we set replacetargetmap to false when we are completely overwriting it

            if (tgtheaderindex == -1)
            {
                // If this header doesnt exists in the target
                // then insert at the end of the target
                tgtheaderindex = target.Lumps.Count;
            }

            // Begin inserting at target header index
            int targetindex = tgtheaderindex;

            // Find the map header in source
            int srcheaderindex = FindAndRemoveMap(source, sourcemapname, false);
			if(srcheaderindex > -1) 
			{
				// Go for all the map lump names
				foreach(KeyValuePair<string, MapLumpInfo> group in config.MapLumps) 
				{
					// Check if this lump should be copied
					if((group.Value.Required && copyrequired) || (group.Value.BlindCopy && copyblindcopy) ||
					   (group.Value.NodeBuild && copynodebuild) || ((group.Value.Script != null || group.Value.ScriptBuild) && copyscript)) 
					{
						// Get the lump name
						string srclumpname = (group.Key.Contains(CONFIG_MAP_HEADER) ? group.Key.Replace(CONFIG_MAP_HEADER, sourcemapname) : group.Key); //mxd
						string tgtlumpname = (group.Key.Contains(CONFIG_MAP_HEADER) ? group.Key.Replace(CONFIG_MAP_HEADER, targetmapname) : group.Key); //mxd

						// Find the lump in the source
						int sourceindex = FindSpecificLump(source, srclumpname, srcheaderindex, sourcemapname, config.MapLumps);
						if(sourceindex > -1) 
						{
                            //mxd. Don't do this when inserting a map (SaveMap() removes the old version of the map before calling CopyLumpsByType())
                            if (replacetargetmap)
                            {
                                // Remove lump at target
                                int lumpindex = RemoveSpecificLump(target, tgtlumpname, tgtheaderindex, targetmapname, config.MapLumps);

                                // Determine target index
                                // When original lump was found and removed then insert at that position
                                // otherwise insert after last insertion position
                                if (lumpindex > -1) targetindex = lumpindex; else targetindex++;
                            }
                            if (targetindex > target.Lumps.Count) targetindex = target.Lumps.Count;

                            // Copy the lump to the target
                            //General.WriteLogLine(srclumpname + " copying as " + tgtlumpname);
                            Lump lump = source.Lumps[sourceindex];
                            Lump newlump = target.Insert(tgtlumpname, targetindex, lump.Length, false);
                            lump.CopyTo(newlump);

                            //mxd. We still need to increment targetindex...
                            if (!replacetargetmap) targetindex++;
                        }
						else 
						{
							// We don't want to bother the user with this. There are a lot of lumps in
							// the game configs that are trivial and don't need to be found.
							if(group.Value.Required) 
							{
								General.ErrorLogger.Add(ErrorType.Warning, group.Key + " (required lump) should be read but was not found in the WAD file.");
							}
						}
					}
				}

				target.WriteHeaders(); //mxd
                target.Compress(); // [ZZ]
			}
		}

		// This finds a lump within the range of known lump names
		// Returns -1 when the lump cannot be found
		private static int FindSpecificLump(WAD source, string lumpname, int mapheaderindex, string mapheadername, Dictionary<string, MapLumpInfo> maplumps) 
		{
			// Use the configured map lump names to find the specific lump within range,
			// because when an unknown lump is met, this search must stop.

			// Go for all lumps in order to find the specified lump
			for(int i = 0; i < maplumps.Count + 1; i++) 
			{
				// Still within bounds?
				if((mapheaderindex + i) < source.Lumps.Count) 
				{
					// Check if this is a known lump name
					string srclumpname = source.Lumps[mapheaderindex + i].Name; //mxd
					if(srclumpname.Contains(mapheadername)) srclumpname = srclumpname.Replace(mapheadername, CONFIG_MAP_HEADER);

					if(maplumps.ContainsKey(srclumpname)) //mxd
					{
						// Is this the lump we are looking for?
						if(source.Lumps[mapheaderindex + i].Name == lumpname) 
						{
							// Return this index
							return mapheaderindex + i;
						}
					}
					else
					{
						// Unknown lump hit, abort search
						break;
					}
				}
			}

			// Nothing found
			return -1;
		}

		// This removes a specific lump and returns the position where the lump was removed
		// Returns -1 when the lump could not be found
		internal static int RemoveSpecificLump(WAD source, string lumpname, int mapheaderindex, string mapheadername, Dictionary<string, MapLumpInfo> maplumps) 
		{
			// Find the specific lump index
			int lumpindex = FindSpecificLump(source, lumpname, mapheaderindex, mapheadername, maplumps);
			if(lumpindex > -1) 
			{
				// Remove this lump
				//General.WriteLogLine(lumpname + " removed");
				source.RemoveAt(lumpindex);
			}
			/*else 
			{
				// Lump not found
				General.ErrorLogger.Add(ErrorType.Warning, lumpname + " should be removed but was not found!");
			}*/

			// Return result
			return lumpindex;
		}

		#endregion

		#region ================== Selection Groups

		// This adds selection to a group
		private void AddSelectionToGroup(int groupindex) 
		{
			General.Interface.SetCursor(Cursors.WaitCursor);

			// Make selection
			map.AddSelectionToGroup(groupindex); //mxd. switched groupmask to groupindex

			General.Interface.DisplayStatus(StatusType.Action, "Assigned selection to group " + (groupindex + 1));
			General.Interface.SetCursor(Cursors.Default);
		}

		// This selects a group
		private void SelectGroup(int groupindex)
		{
			// Select
			int groupmask = 0x01 << groupindex;
			map.SelectVerticesByGroup(groupmask);
			map.SelectLinedefsByGroup(groupmask);
			map.SelectSectorsByGroup(groupmask);
			map.SelectThingsByGroup(groupmask);

			// Redraw to show selection
			General.Interface.DisplayStatus(StatusType.Action, "Selected group " + (groupindex + 1));
			General.Interface.RedrawDisplay();
		}

		//mxd. This clears a group
		private void ClearGroup(int groupindex)
		{
			General.Interface.SetCursor(Cursors.WaitCursor);

			// Clear group
			map.ClearGroup(0x01 << groupindex);

			General.Interface.DisplayStatus(StatusType.Action, "Cleared group " + (groupindex + 1));
			General.Interface.SetCursor(Cursors.Default);
		}

		// Select actions
		[BeginAction("selectgroup1")]
		internal void SelectGroup1() { SelectGroup(0); }
		[BeginAction("selectgroup2")]
		internal void SelectGroup2() { SelectGroup(1); }
		[BeginAction("selectgroup3")]
		internal void SelectGroup3() { SelectGroup(2); }
		[BeginAction("selectgroup4")]
		internal void SelectGroup4() { SelectGroup(3); }
		[BeginAction("selectgroup5")]
		internal void SelectGroup5() { SelectGroup(4); }
		[BeginAction("selectgroup6")]
		internal void SelectGroup6() { SelectGroup(5); }
		[BeginAction("selectgroup7")]
		internal void SelectGroup7() { SelectGroup(6); }
		[BeginAction("selectgroup8")]
		internal void SelectGroup8() { SelectGroup(7); }
		[BeginAction("selectgroup9")]
		internal void SelectGroup9() { SelectGroup(8); }
		[BeginAction("selectgroup10")]
		internal void SelectGroup10() { SelectGroup(9); }

		// Assign actions
		[BeginAction("assigngroup1")]
		internal void AssignGroup1() { AddSelectionToGroup(0); }
		[BeginAction("assigngroup2")]
		internal void AssignGroup2() { AddSelectionToGroup(1); }
		[BeginAction("assigngroup3")]
		internal void AssignGroup3() { AddSelectionToGroup(2); }
		[BeginAction("assigngroup4")]
		internal void AssignGroup4() { AddSelectionToGroup(3); }
		[BeginAction("assigngroup5")]
		internal void AssignGroup5() { AddSelectionToGroup(4); }
		[BeginAction("assigngroup6")]
		internal void AssignGroup6() { AddSelectionToGroup(5); }
		[BeginAction("assigngroup7")]
		internal void AssignGroup7() { AddSelectionToGroup(6); }
		[BeginAction("assigngroup8")]
		internal void AssignGroup8() { AddSelectionToGroup(7); }
		[BeginAction("assigngroup9")]
		internal void AssignGroup9() { AddSelectionToGroup(8); }
		[BeginAction("assigngroup10")]
		internal void AssignGroup10() { AddSelectionToGroup(9); }

		//mxd. Clear actions
		[BeginAction("cleargroup1")]
		internal void ClearGroup1() { ClearGroup(0); }
		[BeginAction("cleargroup2")]
		internal void ClearGroup2() { ClearGroup(1); }
		[BeginAction("cleargroup3")]
		internal void ClearGroup3() { ClearGroup(2); }
		[BeginAction("cleargroup4")]
		internal void ClearGroup4() { ClearGroup(3); }
		[BeginAction("cleargroup5")]
		internal void ClearGroup5() { ClearGroup(4); }
		[BeginAction("cleargroup6")]
		internal void ClearGroup6() { ClearGroup(5); }
		[BeginAction("cleargroup7")]
		internal void ClearGroup7() { ClearGroup(6); }
		[BeginAction("cleargroup8")]
		internal void ClearGroup8() { ClearGroup(7); }
		[BeginAction("cleargroup9")]
		internal void ClearGroup9() { ClearGroup(8); }
		[BeginAction("cleargroup10")]
		internal void ClearGroup10() { ClearGroup(9); }

		#endregion

		#region ================== [mxd] GZDB actions

		[BeginAction("gztogglemodels")]
		internal void ToggleModelsRenderingMode()
		{
			switch(General.Settings.GZDrawModelsMode)
			{
				case ModelRenderMode.NONE:
					General.Settings.GZDrawModelsMode = ModelRenderMode.SELECTION;
					General.MainWindow.DisplayStatus(StatusType.Action, "Models rendering mode: SELECTION ONLY");
					break;

				case ModelRenderMode.SELECTION:
					General.Settings.GZDrawModelsMode = ModelRenderMode.ACTIVE_THINGS_FILTER;
					General.MainWindow.DisplayStatus(StatusType.Action, "Models rendering mode: ACTIVE THINGS FILTER ONLY");
					break;

				case ModelRenderMode.ACTIVE_THINGS_FILTER:
					General.Settings.GZDrawModelsMode = ModelRenderMode.ALL;
					General.MainWindow.DisplayStatus(StatusType.Action, "Models rendering mode: ALL");
					break;

				case ModelRenderMode.ALL:
					General.Settings.GZDrawModelsMode = ModelRenderMode.NONE;
					General.MainWindow.DisplayStatus(StatusType.Action, "Models rendering mode: NONE");
					break;
			}

			General.MainWindow.RedrawDisplay();
			General.MainWindow.UpdateGZDoomPanel();
		}

		[BeginAction("gztogglelights")]
		internal void ToggleLightsRenderingMode()
		{
			if(General.Editing.Mode is ClassicMode)
			{
				switch(General.Settings.GZDrawLightsMode)
				{
					case LightRenderMode.NONE:
						General.Settings.GZDrawLightsMode = LightRenderMode.ALL;
						General.MainWindow.DisplayStatus(StatusType.Action, "Dynamic lights rendering mode: ALL");
						break;

					default:
						General.Settings.GZDrawLightsMode = LightRenderMode.NONE;
						General.MainWindow.DisplayStatus(StatusType.Action, "Dynamic lights rendering mode: NONE");
						break;
				}
			}
			else
			{
				switch(General.Settings.GZDrawLightsMode)
				{
					case LightRenderMode.NONE:
						General.Settings.GZDrawLightsMode = LightRenderMode.ALL;
						General.MainWindow.DisplayStatus(StatusType.Action, "Dynamic lights rendering mode: ALL");
						break;

					case LightRenderMode.ALL:
						General.Settings.GZDrawLightsMode = LightRenderMode.ALL_ANIMATED;
						General.MainWindow.DisplayStatus(StatusType.Action, "Dynamic lights rendering mode: ANIMATED");
						break;

					case LightRenderMode.ALL_ANIMATED:
						General.Settings.GZDrawLightsMode = LightRenderMode.NONE;
						General.MainWindow.DisplayStatus(StatusType.Action, "Dynamic lights rendering mode: NONE");
						break;
				}
			}

			General.MainWindow.RedrawDisplay();
			General.MainWindow.UpdateGZDoomPanel();
		}

		[BeginAction("gzreloadmodeldef")]
		internal void ReloadModeldef()
		{
			data.ReloadModeldef();
		}

		[BeginAction("gzreloadgldefs")]
		internal void ReloadGldefs()
		{
			data.ReloadGldefs();
		}

		#endregion

		#region ================== Script Editing

		// Show the script editor
		[BeginAction("openscripteditor")]
		internal void ShowScriptEditor() 
		{
			// Only show the script editor if there a script lumps defined
			if (!General.Map.Config.HasScriptLumps())
				return;

			Cursor.Current = Cursors.WaitCursor;

			// Load the window?
			if(scriptwindow == null) scriptwindow = new ScriptEditorForm();

			// Window not yet visible?
			if(!scriptwindow.Visible) scriptwindow.Show();

			scriptwindow.TopMost = General.Settings.ScriptOnTop; //mxd
			if(scriptwindow.WindowState == FormWindowState.Minimized)
				scriptwindow.WindowState = FormWindowState.Normal; //mxd

			scriptwindow.Activate();
			scriptwindow.Focus();

			Cursor.Current = Cursors.Default;
		}

		// This asks the user to save changes in script files
		// Returns false when cancelled by the user
		internal bool AskSaveScriptChanges() 
		{
			// Window open?
			if(scriptwindow != null) 
			{
				// Ask to save changes
				// This also saves implicitly
				return scriptwindow.AskSaveAll();
			}

			// No problems
			return true;
		}

		// This applies the changed status for internal scripts
		internal void ApplyScriptChanged() 
		{
			// Remember if lumps are changed
			if(scriptwindow != null)
			{
				scriptschanged |= scriptwindow.Editor.CheckImplicitChanges();
			}
		}

		// Close the script editor
		// Specify true for the closing parameter when
		// the window is already in the closing process
		internal void CloseScriptEditor(bool closing) 
		{
			if(scriptwindow != null) 
			{
				if(!scriptwindow.IsDisposed) 
				{
					// Remember what files were open
					scriptwindow.Editor.WriteOpenFilesToConfiguration();

					// Close now
					if(!closing) scriptwindow.Close();
				}

				// Done
				scriptwindow = null;
			}
		}

		// This checks if the scripts are changed
		private bool CheckScriptChanged() 
		{
			if(scriptwindow != null) 
			{
				// Check if scripts are changed			
				return scriptschanged || scriptwindow.Editor.CheckImplicitChanges();
			}
	
			return scriptschanged;
		}

		// This compiles all lumps that require compiling and stores the results
		// Returns true when our code worked properly (even when the compiler returned errors)
		private bool CompileScriptLumps() 
		{
			bool success = true;
			errors.Clear();

			// Go for all the map lumps
			foreach(MapLumpInfo lumpinfo in config.MapLumps.Values) 
			{
				// Is this a script lump?
				if(lumpinfo.Script != null)
				{
					// Compile it now (but only if it's required or exists)
					if(lumpinfo.Required || tempwadreader.WadFile.FindLumpIndex(lumpinfo.Name) != -1)
						success &= tempwadreader.CompileLump(lumpinfo.Name, lumpinfo.Script, errors);
				}
				//mxd. Is this ACS script?
				else if(lumpinfo.ScriptBuild)
				{
					// Compile it using selected script compiler
					ScriptConfiguration cfg = General.GetScriptConfiguration(ScriptType.ACS);
					if(cfg != null)
					{
						success &= tempwadreader.CompileLump(lumpinfo.Name, cfg, errors);
					}
					else
					{
						General.ErrorLogger.Add(ErrorType.Error, "Unable to compile \"" + lumpinfo.Name + "\" script lump: unable to find suitable Script Configuration!");
						success = false;
					}
				}
			}

			return success;
		}

		//mxd. Update script numbers and names, collect loose ACS files.
		private void LoadACS()
		{
			///////////////////////////////////////////
			// Step 1: Update script numbers and names
			///////////////////////////////////////////
			
			General.Map.Data.ScriptResources[ScriptType.ACS] = new HashSet<ScriptResource>();

			// Find SCRIPTS lump and parse it
			foreach(MapLumpInfo maplumpinfo in config.MapLumps.Values) 
			{
				// Is this a script lump?
				if((maplumpinfo.ScriptBuild || maplumpinfo.Script != null) && maplumpinfo.Name == "SCRIPTS") 
				{
					ScriptConfiguration scriptconfig;
					if(maplumpinfo.ScriptBuild)
					{
						//mxd. More boilderplate
						if(!General.CompiledScriptConfigs.ContainsKey(General.Map.Options.ScriptCompiler))
						{
							string error = "Unable to compile lump \"" + maplumpinfo.Name +
							               "\". Unable to find required script compiler configuration (\"" +
							               General.Map.Options.ScriptCompiler + "\").";

							General.ErrorLogger.Add(ErrorType.Error, error);
							return;
						}

						scriptconfig = General.CompiledScriptConfigs[General.Map.Options.ScriptCompiler];
					}
					else
					{
						scriptconfig = maplumpinfo.Script;
					}
					
					// Load the lump data
					MemoryStream stream = GetLumpData(maplumpinfo.Name);
					if(stream != null && stream.Length > 0 && scriptconfig != null && scriptconfig.Compiler != null && scriptconfig.ScriptType == ScriptType.ACS)
					{
						// Get script names
						AcsParserSE parser = new AcsParserSE
						{
							IsMapScriptsLump = true,
							IgnoreErrors = true,
							OnInclude = delegate(AcsParserSE se, string includefile, AcsParserSE.IncludeType includetype)
							{
								TextResourceData includedata = General.Map.Data.GetTextResourceData(includefile);
								if (includedata == null)
								{
									General.ErrorLogger.Add(ErrorType.Error, "Could not find file to include while loading " + maplumpinfo.Name + ": " + includefile);
									return false; // Fail
								}
								return se.Parse(includedata, true, includetype, false);
							}
						};

						//INFO: CompileLump() prepends lumpname with "?" to distinguish between temporary files and files compiled in place
						DataLocation location = new DataLocation { location = tempwadreader.WadFile.Filename, type = DataLocation.RESOURCE_WAD };
						TextResourceData trd = new TextResourceData(stream, location, "?SCRIPTS");
						if(parser.Parse(trd, scriptconfig.Compiler.Files, true, AcsParserSE.IncludeType.NONE, false))
						{
							// Add to text resource list
							General.Map.Data.ScriptResources[parser.ScriptType].UnionWith(parser.ScriptResources.Values);

							// Update the names
							UpdateScriptNames(parser);
						}
						else
						{
							// Clear collections
							namedscripts.Clear();
							numberedscripts.Clear();
						}

						// Log errors, if any
						if(parser.HasError) parser.LogError();

						// Done here
						break;
					}
				}
			}

			///////////////////////////////////////////
			// Step 2: Try to load unused ACS files
			///////////////////////////////////////////

			Dictionary<string, HashSet<string>> existingacsfiles = new Dictionary<string, HashSet<string>>();

			// Gather already parsed files...
			foreach(ScriptResource sr in General.Map.Data.ScriptResources[ScriptType.ACS])
			{
				if(!existingacsfiles.ContainsKey(sr.Resource.Location.location))
					existingacsfiles.Add(sr.Resource.Location.location, new HashSet<string>());

				existingacsfiles[sr.Resource.Location.location].Add(sr.Filename);
			}

			HashSet<ScriptResource> looseacsfiles = new HashSet<ScriptResource>();
			foreach(DataReader dr in General.Map.Data.Containers)
			{
				//TODO: implement for WADs
				if(dr is PK3StructuredReader)
				{
					PK3StructuredReader pkr = (PK3StructuredReader)dr;
					string[] acsfiles = pkr.GetFilesWithExt("", "acs", true);
					foreach(string acsfile in acsfiles)
					{
						if(!existingacsfiles.ContainsKey(pkr.Location.location) || !existingacsfiles[pkr.Location.location].Contains(acsfile))
						{
							TextResourceData trd = new TextResourceData(dr, dr.LoadFile(acsfile), acsfile, true);
							looseacsfiles.Add(new ScriptResource(trd, ScriptType.ACS));
						}
					}
				}
			}

			// Add to the main collection
			General.Map.Data.ScriptResources[ScriptType.ACS].UnionWith(looseacsfiles);
		}

		//mxd
		internal void UpdateScriptNames(AcsParserSE parser)
		{
			if(parser.HasError)
			{
				// Clear collections
				namedscripts.Clear();
				numberedscripts.Clear();
			}
			else
			{
				// Add to collections
				namedscripts = new Dictionary<string, ScriptItem>(parser.NamedScripts.Count, StringComparer.OrdinalIgnoreCase);
				numberedscripts = new Dictionary<int, ScriptItem>(parser.NumberedScripts.Count);

				// Sort script names
				parser.NamedScripts.Sort(ScriptItem.SortByName);
				parser.NumberedScripts.Sort(ScriptItem.SortByIndex);

				// Add scripts, but only those the user didn't want to skip (using the //$Skip editor key)
				foreach(ScriptItem item in parser.NamedScripts)
				{
					if(!item.Skip && !namedscripts.ContainsKey(item.Name.ToLowerInvariant()))
						namedscripts.Add(item.Name.ToLowerInvariant(), item);
				}

				// Add scripts, but only those the user didn't want to skip (using the //$Skip editor key)
				foreach (ScriptItem item in parser.NumberedScripts)
				{
					if(!item.Skip && !numberedscripts.ContainsKey(item.Index))
						numberedscripts.Add(item.Index, item);
				}
			}
		}

		//mxd
		public bool ScriptNumberExists(int scriptnumber)
		{
			return numberedscripts.ContainsKey(scriptnumber);
		}

		//mxd
		public bool ScriptNameExists(string scriptname)
		{
			return namedscripts.ContainsKey(scriptname);
		}

		#endregion

		#region ================== Methods

		// This updates everything after the configuration or settings have been changed
		internal void UpdateConfiguration() 
		{
			// Update map
			map.UpdateConfiguration();

			// Update settings
			renderer3d.CreateProjection();
			renderer3d.UpdateVertexHandle(); //mxd
			renderer3d.UpdateVisualSlopeHandle();

			// Things filters
			General.MainWindow.UpdateThingsFilters();
		}

		// This changes thing filter
		public void ChangeThingFilter(ThingsFilter newfilter) 
		{
			// We have a special filter for null
			if(newfilter == null) newfilter = new NullThingsFilter();

			// Deactivate old filter
			if(thingsfilter != null) thingsfilter.Deactivate();

			// Change
			thingsfilter = newfilter;

			// Activate filter
			thingsfilter.Activate();

			// Update interface
			General.MainWindow.ReflectThingsFilter();

			// Redraw
			General.MainWindow.RedrawDisplay();
		}

		// This sets a new mapset for editing
		private void ChangeMapSet(MapSet newmap) 
		{
			// Let the plugin and editing mode know
			General.Plugins.OnMapSetChangeBegin();
			if(General.Editing.Mode != null) General.Editing.Mode.OnMapSetChangeBegin();
			this.visualcamera.Sector = null;

			// Can't have a selection in an old map set
			map.ClearAllSelected();

			// Reset surfaces
			renderer2d.Surfaces.Reset();

			// Apply
			map.Dispose();
			map = newmap;
			map.UpdateConfiguration();
			map.SnapAllToAccuracy();
			map.Update();
			thingsfilter.Update();

			// Let the plugin and editing mode know
			General.Plugins.OnMapSetChangeEnd();
			if(General.Editing.Mode != null) General.Editing.Mode.OnMapSetChangeEnd();
		}

		// This reloads resources
		[BeginAction("reloadresources")]
		internal void DoReloadResource() 
		{
			//mxd. Get rid of old errors
			General.ErrorLogger.Clear();
			
			// Set this to false so we can see if errors are added
			General.ErrorLogger.IsErrorAdded = false;

#if DEBUG 
			DebugConsole.Clear();
#endif

			ReloadResources(true, true);

			if(General.ErrorLogger.IsErrorAdded)
			{
				// Show any errors if preferred
				General.MainWindow.DisplayStatus(StatusType.Warning, "There were errors during resources loading!");
				if(General.Settings.ShowErrorsWindow) General.MainWindow.ShowErrors();
			}
			else
			{
				General.MainWindow.DisplayReady();
			}

		}

		internal void ReloadResources(bool clearerrors, bool runprepostcommands) //mxd. clearerrors flag
		{
			// Keep old display info
			StatusInfo oldstatus = General.MainWindow.Status;
			Cursor oldcursor = Cursor.Current;

			// Check if there's a pre command to run, and try to execute it
			if (runprepostcommands && !string.IsNullOrWhiteSpace(options.ReloadResourcePreCommand.Commands))
			{
				if(!ExecuteExternalCommand(options.ReloadResourcePreCommand))
				{
					General.WriteLogLine("Reloading resources was canceled when executing the reload resource pre command.");

					// Reset status
					General.MainWindow.DisplayStatus(StatusType.Warning, "Reloading resourcess was canceled.");
					Cursor.Current = oldcursor;

					return;
				}
			}

			// Show status
			General.MainWindow.DisplayStatus(StatusType.Busy, "Reloading data resources...");
			Cursor.Current = Cursors.WaitCursor;

			// Clean up
			data.Dispose();
			data = null;
			config = null;
			configinfo = null;
			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect(); //mxd

			// Clear errors?
			if(clearerrors) General.ErrorLogger.Clear();

			// Reload game configuration
			General.WriteLogLine("Reloading game configuration...");
			configinfo = General.GetConfigurationInfo(options.ConfigFile);
			config = new GameConfiguration(configinfo.Configuration); //mxd
			General.Editing.UpdateCurrentEditModes();

			// Reload data resources
			General.WriteLogLine("Reloading data resources...");
			data = new DataManager();
			if(!string.IsNullOrEmpty(filepathname)) 
			{
				DataLocation maplocation = new DataLocation(DataLocation.RESOURCE_WAD, filepathname, options.StrictPatches, false, false, null);
				data.Load(configinfo.Resources, options.Resources, maplocation);
			}
			else 
			{
				data.Load(configinfo.Resources, options.Resources);
			}

			// Apply new settings to map elements
			map.UpdateConfiguration();

			// Re-link the background image
			grid.LinkBackground();

			// Inform all plugins that the resources are reloaded
			General.Plugins.ReloadResources();

			// Inform editing mode that the resources are reloaded
			if(General.Editing.Mode != null)
			{
				General.Editing.Mode.OnReloadResources();

				//mxd. Also Check appropriate button on interface
				General.MainWindow.CheckEditModeButton(General.Editing.Mode.EditModeButtonName); 
			}

			//mxd. Update script names
			LoadACS();

			// Check if there's a post command to run, and try to execute it
			if (runprepostcommands && !string.IsNullOrWhiteSpace(options.ReloadResourcePostCommand.Commands))
			{
				if (!ExecuteExternalCommand(options.ReloadResourcePostCommand))
				{
					General.WriteLogLine("Failed to execute the reload resource post command successfully.");
				}
			}

			// Reset status
			General.MainWindow.DisplayStatus(oldstatus);
			Cursor.Current = oldcursor;
		}

		public bool ExecuteExternalCommand(ExternalCommandSettings cmdsettings, string arguments = "")
		{
			string filename;

			do
			{
				filename = Path.ChangeExtension(Path.GetTempFileName(), ".cmd");
			}
			while (File.Exists(filename));

			File.WriteAllText(filename, cmdsettings.Commands);
			
			ProcessStartInfo startinfo = new ProcessStartInfo();
			startinfo.FileName = "cmd.exe";
			startinfo.Arguments = "/C " + filename + " " + arguments;
			if(!string.IsNullOrWhiteSpace(cmdsettings.WorkingDirectory))
				startinfo.WorkingDirectory = cmdsettings.WorkingDirectory;

			RunExternalCommandForm f = new RunExternalCommandForm(startinfo, cmdsettings);
			f.ShowDialog();

			File.Delete(filename);

			return f.DialogResult == DialogResult.OK;
		}

		// Game Configuration action
		[BeginAction("mapoptions")]
		internal void ShowMapOptions() 
		{
			// Cancel volatile mode, if any
			General.Editing.DisengageVolatileMode();

			// Show map options dialog
			MapOptionsForm optionsform = new MapOptionsForm(options, false);
			if(optionsform.ShowDialog(General.MainWindow) == DialogResult.OK) 
			{
				// Update interface
				//General.MainWindow.UpdateInterface();

				// Stop data manager
				data.Dispose();

				//mxd. Clear errors
				General.ErrorLogger.Clear();

				// Apply new options
				this.options = optionsform.Options;

				// Load new game configuration
				General.WriteLogLine("Loading game configuration \"" + options.ConfigFile + "\"...");
				configinfo = General.GetConfigurationInfo(options.ConfigFile);
				Type oldiotype = io.GetType(); //mxd

				//mxd. Step 1 of hackish way to translate SP/MP thing flags to Hexen / UDMF formats...
				//TODO: add proper Doom -> Hexen thing flags translation to the configs?
				if(oldiotype == typeof(DoomMapSetIO) && configinfo.FormatInterface != "doommapsetio")
				{
					// Translate to UDMF using Doom things flags translation table
					foreach(Thing t in General.Map.Map.Things) t.TranslateToUDMF();
				}

				config = new GameConfiguration(configinfo.Configuration); //mxd
				configinfo.ApplyDefaults(config);
				General.Editing.UpdateCurrentEditModes();

				// Setup new map format IO
				General.WriteLogLine("Initializing map format interface " + config.FormatInterface + "...");
				io = MapSetIO.Create(config.FormatInterface, tempwadreader.WadFile, this);

				//mxd. Some lumps may've become unneeded during map format conversion. 
				if(oldiotype != io.GetType())
					RemoveUnneededLumps(tempwadreader.WadFile, TEMP_MAP_HEADER, false);

				// Create required lumps if they don't exist yet
				CreateRequiredLumps(tempwadreader.WadFile, TEMP_MAP_HEADER);

				// Let the plugins know
				General.Plugins.MapReconfigure();

				//mxd. Update linedef color presets and flags if required
				if(oldiotype == typeof(UniversalMapSetIO) && !(io is UniversalMapSetIO)) 
				{
					foreach(Linedef l in General.Map.Map.Linedefs) l.TranslateFromUDMF();
					foreach(Thing t in General.Map.Map.Things) t.TranslateFromUDMF();
				} 
				else if(oldiotype == typeof(DoomMapSetIO))
				{
					if(io is UniversalMapSetIO)
					{
						//Thing flags were already translated in Setp 1...
						//TODO: linedef actions will require the same handling...
						foreach(Linedef l in General.Map.Map.Linedefs) l.TranslateToUDMF(oldiotype);
					}
					else if(io is HexenMapSetIO)
					{
						// Step 2 of hackish way to translate SP/MP thing flags to Hexen map format...
						foreach(Thing t in General.Map.Map.Things) t.TranslateFromUDMF();
					}
				}
				else if(oldiotype != typeof(UniversalMapSetIO) && io is UniversalMapSetIO)
				{
					foreach(Linedef l in General.Map.Map.Linedefs) l.TranslateToUDMF(oldiotype);
					foreach(Thing t in General.Map.Map.Things) t.TranslateToUDMF();
				}

				// Drop all arguments
				if(oldiotype != typeof(DoomMapSetIO) && io is DoomMapSetIO)
				{
					
					foreach(Linedef l in General.Map.Map.Linedefs)
						for(int i = 0; i < l.Args.Length; i++) l.Args[i] = 0;
					foreach(Thing t in General.Map.Map.Things)
						for(int i = 0; i < t.Args.Length; i++) t.Args[i] = 0;
				}

				map.UpdateCustomLinedefColors();

				// Reload resources
				ReloadResources(false, false);

				// Update interface
				General.MainWindow.SetupInterface();
				General.MainWindow.UpdateThingsFilters();
				General.MainWindow.UpdateLinedefColorPresets(); //mxd
				General.MainWindow.UpdateInterface();

				// Done
				General.MainWindow.DisplayReady();
				General.MainWindow.RedrawDisplay(); //mxd
			}

			// Done
			optionsform.Dispose();
		}

		// This shows the things filters setup
		[BeginAction("thingsfilterssetup")]
		internal void ShowThingsFiltersSetup() 
		{
			new ThingsFiltersForm().ShowDialog(General.MainWindow);
		}

		//mxd. This shows the linedef color presets window
		[BeginAction("linedefcolorssetup")]
		internal void ShowLinedefColorsSetup()
		{
			// Show things filter dialog
			new LinedefColorPresetsForm().ShowDialog(General.MainWindow);
		}

		// This returns true is the given type matches
		public bool IsType(Type t) 
		{
			return io.GetType() == t;
		}

		//mxd
		[BeginAction("snapvertstogrid")]
		private void SnapSelectedMapElementsToGrid() 
		{
			// Get selected elements
			ICollection<Vertex> verts = map.GetSelectedVertices(true);
			ICollection<Linedef> lines = map.GetSelectedLinedefs(true); // Sector lines are auto-selected when a sector is selected
			ICollection<Thing> things = map.GetSelectedThings(true);

			// Get vertices from selection
			Dictionary<int, Vertex> vertstosnap = new Dictionary<int, Vertex>(verts.Count);
			foreach(Vertex v in verts) vertstosnap.Add(v.Index, v);
			foreach(Linedef l in lines)
			{
				if(!vertstosnap.ContainsKey(l.Start.Index)) vertstosnap.Add(l.Start.Index, l.Start);
				if(!vertstosnap.ContainsKey(l.End.Index)) vertstosnap.Add(l.End.Index, l.End);
			}

			// Anything to snap?
			if(vertstosnap.Count == 0 && things.Count == 0) 
			{
				General.Interface.DisplayStatus(StatusType.Warning, "Select any map element first!");
				return;
			}

			// Make undo
			undoredo.CreateUndo("Snap map elements to grid");

			// Do the snapping
			Cursor.Current = Cursors.AppStarting;

			// Snap vertices?
			int snappedverts = (vertstosnap.Count > 0 ? SnapVertices(vertstosnap.Values) : 0);

			// Snap things?..
			int snappedthings = (things.Count > 0 ? SnapThings(things) : 0);

			// Assemble status message
			List<string> message = new List<string>();
			if(snappedverts > 0) message.Add(snappedverts + " vertices");
			if(snappedthings > 0) message.Add(snappedthings + " things");

			// Map changed?
			if(message.Count > 0) 
			{
				// Display status
				General.Interface.DisplayStatus(StatusType.Info, "Snapped " + string.Join(" and ", message.ToArray()));

				// Warn the user
				/*if(snappedverts > 0) 
				{
					MessageBox.Show("Snapped " + snappedverts + " vertices to grid." + Environment.NewLine +
					                "It's a good idea to run Map Analysis Mode now.");
				}*/

				// Invoke clear selection to update sector highlight overlay
				General.Actions.InvokeAction("builder_clearselection");

				// Update cached values
				General.Map.Map.Update();

				// Map is changed
				General.Map.IsChanged = true;
			}
			else 
			{
				// Display status
				General.Interface.DisplayStatus(StatusType.Info, "Selected map elements were already on the grid.");

				// Withdraw undo
				undoredo.WithdrawUndo();
			}

			// Let the current editing mode know that we changed something
			General.Editing.Mode.OnMapElementsChanged();

			// Done
			General.Interface.RedrawDisplay();
			Cursor.Current = Cursors.Default;
		}

		//mxd
		private int SnapVertices(IEnumerable<Vertex> verts)
		{
			int snappedCount = 0;
			List<Vertex> movedVerts = new List<Vertex>();
			List<Linedef> movedLines = new List<Linedef>();

			//snap them all!
			foreach(Vertex v in verts) 
			{
				Vector2D pos = v.Position;
				v.SnapToGrid();

				if(v.Position.x != pos.x || v.Position.y != pos.y) 
				{
					snappedCount++;
					movedVerts.Add(v);
					foreach(Linedef l in v.Linedefs) 
					{
						if(!movedLines.Contains(l)) movedLines.Add(l);
					}
				}
			}

			//Create blockmap
			RectangleF area = MapSet.CreateArea(General.Map.Map.Vertices);
			BlockMap<BlockEntry> blockmap = new BlockMap<BlockEntry>(area);
			blockmap.AddVerticesSet(General.Map.Map.Vertices);

			//merge overlapping vertices using teh power of BLOCKMAP!!!11
			foreach(Vertex v in movedVerts) 
			{
				BlockEntry block = blockmap.GetBlockAt(v.Position);
				if(block == null) continue;

				foreach(Vertex blockVert in block.Vertices) 
				{
					if(blockVert.IsDisposed || blockVert.Index == v.Index || blockVert.Position != v.Position)
						continue;

					foreach(Linedef l in blockVert.Linedefs) 
					{
						if(!movedLines.Contains(l)) movedLines.Add(l);
					}
					v.Join(blockVert);
					break;
				}
			}

			// Update cached values of lines because we may need their length/angle
			General.Map.Map.Update(true, false);

			General.Map.Map.BeginAddRemove();
			MapSet.RemoveLoopedLinedefs(movedLines);
			MapSet.JoinOverlappingLines(movedLines);
			General.Map.Map.EndAddRemove();

			//get changed sectors
			List<Sector> changedSectors = new List<Sector>();
			foreach(Linedef l in movedLines) 
			{
				if(l == null || l.IsDisposed) continue;
				if(l.Front != null && l.Front.Sector != null && !changedSectors.Contains(l.Front.Sector))
					changedSectors.Add(l.Front.Sector);
				if(l.Back != null && l.Back.Sector != null && !changedSectors.Contains(l.Back.Sector))
					changedSectors.Add(l.Back.Sector);
			}

			// Now update area of sectors
			General.Map.Map.Update(false, true);

			//fix invalid sectors
			foreach(Sector s in changedSectors) 
			{
				if(s.BBox.IsEmpty) 
				{
					s.Dispose();
				} 
				else if(s.Sidedefs.Count < 3) 
				{
					bool merged = false;
					foreach(Sidedef side in s.Sidedefs) 
					{
						if(side.Other != null && side.Other.Sector != null) 
						{
							s.Join(side.Other.Sector);
							merged = true;
							break;
						}
					}

					//oh well, I don't know what else I can do here...
					if(!merged) s.Dispose();
				}
			}

			return snappedCount;
		}

		//mxd
		private static int SnapThings(IEnumerable<Thing> things)
		{
			int snappedCount = 0;

			//snap them all!
			foreach(Thing t in things) 
			{
				Vector2D pos = t.Position;
				t.SnapToGrid();
				if(t.Position.x != pos.x || t.Position.y != pos.y) snappedCount++;
			}

			return snappedCount;
		}

		public void GridVisibilityChanged()
		{
			renderer2d.GridVisibilityChanged();
		}

		/// <summary>
		/// This forces the "changed" variable to "false", which is normally not possible when setting the "IsChanged" property.
		/// USE WITH CARE! This should only be used in very specific circumstances, where the "IsChanged" property is set to true
		/// because of internal behavior
		/// </summary>
		internal void ForceMapIsChangedFalse()
		{
			changed = false;
			General.MainWindow.UpdateMapChangedStatus();
		}

		#endregion
	}
}