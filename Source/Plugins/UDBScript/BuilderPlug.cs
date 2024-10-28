#region ================== Copyright (c) 2020 Boris Iwanski

/*
 * This program is free software: you can redistribute it and/or modify
 *
 * it under the terms of the GNU General Public License as published by
 * 
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 *    but WITHOUT ANY WARRANTY; without even the implied warranty of
 * 
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
 * 
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.If not, see<http://www.gnu.org/licenses/>.
 */

#endregion

#region ================== Namespaces

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Windows.Forms;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using CodeImp.DoomBuilder.Actions;
using CodeImp.DoomBuilder.Controls;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.IO;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Plugins;
using CodeImp.DoomBuilder.Types;
using CodeImp.DoomBuilder.UDBScript.Wrapper;
using CodeImp.DoomBuilder.Windows;

#endregion

namespace CodeImp.DoomBuilder.UDBScript
{
	internal class ScriptDirectoryStructure
	{
		public string Path;
		public string Name;
		public bool Expanded;
		public string Hash;
		public List<ScriptDirectoryStructure> Directories;
		public List<ScriptInfo> Scripts;

		public ScriptDirectoryStructure(string path, string name, bool expanded, string hash)
		{
			Path = path;
			Name = name;
			Expanded = expanded;
			Hash = hash;
			Directories = new List<ScriptDirectoryStructure>();
			Scripts = new List<ScriptInfo>();
		}
	}

	public class BuilderPlug : Plug
	{
		#region ================== Constants

		private static readonly string SCRIPT_FOLDER = "UDBScript";
		public static readonly uint UDB_SCRIPT_VERSION = 5;

		#endregion

		private delegate void CallVoidMethodDeletage();

		#region ================== Constants

		public const int NUM_SCRIPT_SLOTS = 30;

		#endregion

		#region ================== Variables

		private static BuilderPlug me;
		private ScriptDockerControl panel;
		private Docker docker;
		private string currentscriptfile;
		private ScriptInfo currentscript;
		private ScriptRunner scriptrunner;
		private List<ScriptInfo> scriptinfo;
		private ScriptDirectoryStructure scriptdirectorystructure;
		private FileSystemWatcher watcher;
		private object lockobj;
		private Dictionary<int, ScriptInfo> scriptslots;
		private string editorexepath;
		private PreferencesForm preferencesform;
		private ScriptRunnerForm scriptrunnerform;

		#endregion

		#region ================== Properties

		public static BuilderPlug Me { get { return me; } }
		public string CurrentScriptFile { get { return currentscriptfile; } set { currentscriptfile = value; } }
		internal ScriptInfo CurrentScript { get { return currentscript; } set { currentscript = value; } }
		internal ScriptRunner ScriptRunner { get { return scriptrunner; } }
		internal ScriptDirectoryStructure ScriptDirectoryStructure { get { return scriptdirectorystructure; } }
		internal string EditorExePath { get { return editorexepath; } }
		public ScriptRunnerForm ScriptRunnerForm { get { return scriptrunnerform; } }

		#endregion

		public override void OnInitialize()
		{
			base.OnInitialize();

			me = this;

			lockobj = new object();

			scriptinfo = new List<ScriptInfo>();
			scriptslots = new Dictionary<int, ScriptInfo>();

			panel = new ScriptDockerControl(SCRIPT_FOLDER);
			docker = new Docker("udbscript", "Scripts", panel);
			General.Interface.AddDocker(docker);

			General.Actions.BindMethods(this);

			string scriptspath = Path.Combine(General.AppPath, SCRIPT_FOLDER, "Scripts");

			if (Directory.Exists(scriptspath))
			{
				watcher = new FileSystemWatcher(Path.Combine(General.AppPath, SCRIPT_FOLDER, "Scripts"));
				watcher.NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.DirectoryName | NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.Size;
				watcher.IncludeSubdirectories = true;
				watcher.Changed += OnWatcherEvent;
				watcher.Created += OnWatcherEvent;
				watcher.Deleted += OnWatcherEvent;
				watcher.Renamed += OnWatcherEvent;
			}

			editorexepath = General.Settings.ReadPluginSetting("externaleditor", string.Empty);

			scriptrunnerform = new ScriptRunnerForm();

			FindEditor();
		}

		public override void OnMapNewEnd()
		{
			base.OnMapNewEnd();

			// Methods called by LoadScripts might sleep for some time, so call LoadScripts asynchronously
			new Task(LoadScripts).Start();

			if (watcher != null)
				watcher.EnableRaisingEvents = true;
		}

		public override void OnMapOpenEnd()
		{
			base.OnMapOpenEnd();

			// Methods called by LoadScripts might sleep for some time, so call LoadScripts asynchronously
			new Task(LoadScripts).Start();

			if (watcher != null)
				watcher.EnableRaisingEvents = true;
		}

		public override void OnMapCloseBegin()
		{
			if (watcher != null)
				watcher.EnableRaisingEvents = false;

			SaveScriptSlotsAndOptions();
			SaveScriptDirectoryExpansionStatus(scriptdirectorystructure);
		}

		public override void OnShowPreferences(PreferencesController controller)
		{
			base.OnShowPreferences(controller);

			preferencesform = new PreferencesForm();
			preferencesform.Setup(controller);
		}

		public override void OnClosePreferences(PreferencesController controller)
		{
			base.OnClosePreferences(controller);

			preferencesform.Dispose();
			preferencesform = null;
		}

		private void OnWatcherEvent(object sender, FileSystemEventArgs e)
		{
			// We can't use the filter on the watcher, since for whatever reason that filter also applies to
			// directory names. So we have to do some filtering ourselves.
			bool load = false;
			if (e.ChangeType == WatcherChangeTypes.Deleted || (Directory.Exists(e.FullPath) && e.ChangeType != WatcherChangeTypes.Changed) || Path.GetExtension(e.FullPath).ToLowerInvariant() == ".js")
				load = true;

			if(load)
				LoadScripts();
		}

		// This is called when the plugin is terminated
		public override void Dispose()
		{
			base.Dispose();

			// This must be called to remove bound methods for actions.
			General.Actions.UnbindMethods(this);
		}

		internal void SaveScriptSlotsAndOptions()
		{
			// Save the script option values
			foreach (ScriptInfo si in scriptinfo)
				si.SaveOptionValues();

			// Save the script slots
			foreach (KeyValuePair<int, ScriptInfo> kvp in scriptslots)
			{
				if (kvp.Value == null || string.IsNullOrWhiteSpace(kvp.Value.ScriptFile))
					continue;

				General.Settings.WritePluginSetting("scriptslots.slot" + kvp.Key, kvp.Value.ScriptFile);
			}
		}

		internal void SaveScriptDirectoryExpansionStatus(ScriptDirectoryStructure root)
		{
			if (root == null)
				return;

			if(root.Expanded)
			{
				General.Settings.DeletePluginSetting("directoryexpand." + root.Hash);
			}
			else
			{
				General.Settings.WritePluginSetting("directoryexpand." + root.Hash, false);
			}

			foreach (ScriptDirectoryStructure sds in root.Directories)
				SaveScriptDirectoryExpansionStatus(sds);
		}

		private void FindEditor()
		{
			if (!string.IsNullOrWhiteSpace(editorexepath))
				return;

			string editor = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "notepad.exe");

			if (!File.Exists(editor))
				return;

			editorexepath = editor;
		}

		/// <summary>
		/// Sets the new external editor exe path.
		/// </summary>
		/// <param name="exepath">Path and file name of the external editor</param>
		internal void SetEditor(string exepath)
		{
			if (!string.IsNullOrWhiteSpace(exepath))
			{
				editorexepath = exepath;
				General.Settings.WritePluginSetting("externaleditor", editorexepath);
			}
		}

		/// <summary>
		/// Opens a script in the external editor.
		/// </summary>
		/// <param name="file"></param>
		internal void EditScript(string file)
		{
			if(string.IsNullOrWhiteSpace(editorexepath))
			{
				MessageBox.Show("No external editor set. Please set the external editor in the UDBScript tab in the preferences.");
				return;
			}

			Process p = new Process();
			p.StartInfo.FileName = editorexepath;
			p.StartInfo.Arguments = "\"" + file + "\""; // File name might contain spaces, so put it in quotes
			p.Start();
		}

		/// <summary>
		/// Sets a ScriptInfo to a specific slot.
		/// </summary>
		/// <param name="slot">The slot</param>
		/// <param name="si">The ScriptInfo to assign to the slot. Pass null to clear the slot</param>
		public void SetScriptSlot(int slot, ScriptInfo si)
		{
			if (si == null)
			{
				scriptslots.Remove(slot);
			}
			else
			{
				// Check if the ScriptInfo is already assigned to a slot, and remove it if so
				// Have to use ToList because otherwise the collection would be changed while iterating over it
				foreach (int s in scriptslots.Keys.ToList())
					if (scriptslots[s] == si)
						scriptslots[s] = null;

				scriptslots[slot] = si;
			}

			SaveScriptSlotsAndOptions();
		}

		/// <summary>
		/// Gets a ScriptInfo for a specific script slot.
		/// </summary>
		/// <param name="slot">The slot to get the ScriptInfo for</param>
		/// <returns>The ScriptInfo for the slot, or null if the ScriptInfo is at no slot</returns>
		public ScriptInfo GetScriptSlot(int slot)
		{
			if (scriptslots.ContainsKey(slot))
				return scriptslots[slot];
			else
				return null;
		}

		/// <summary>
		/// Gets the script slot by a ScriptInfo.
		/// </summary>
		/// <param name="si">The ScriptInfo to get the slot of</param>
		/// <returns>The slot the ScriptInfo is in, or 0 if the ScriptInfo is not assigned to a slot</returns>
		public int GetScriptSlotByScriptInfo(ScriptInfo si)
		{
			if (!scriptslots.Values.Contains(si))
				return 0;

			return scriptslots.FirstOrDefault(i => i.Value == si).Key;
		}

		/// <summary>
		/// Loads all scripts and fills the docker panel.
		/// </summary>
		public void LoadScripts()
		{
			lock (lockobj)
			{
				scriptinfo = new List<ScriptInfo>();
				scriptdirectorystructure = LoadScriptDirectoryStructure(Path.Combine(General.AppPath, SCRIPT_FOLDER, "Scripts"));

				scriptslots = new Dictionary<int, ScriptInfo>();
				for(int i=0; i < NUM_SCRIPT_SLOTS; i++)
				{
					int num = i + 1;
					string file = General.Settings.ReadPluginSetting("scriptslots.slot" + num, string.Empty);

					if (string.IsNullOrWhiteSpace(file))
						continue;

					foreach(ScriptInfo si in scriptinfo)
					{
						if (si.ScriptFile == file)
							scriptslots[num] = si;
					}
				}

				// This might not be called from the main thread when called by the file system watcher, so use a delegate
				// to run it cleanly
				if (panel.InvokeRequired)
				{
					CallVoidMethodDeletage d = panel.FillTree;
					panel.Invoke(d);
				}
				else
				{
					panel.FillTree();
				}
			}
		}

		/// <summary>
		/// Recursively load information about the script files in a directory and its subdirectories.
		/// </summary>
		/// <param name="path">Path to process</param>
		/// <returns>ScriptDirectoryStructure for the given path</returns>
		private ScriptDirectoryStructure LoadScriptDirectoryStructure(string path)
		{
			string hash = SHA256Hash.Get(path);
			bool expanded = General.Settings.ReadPluginSetting("directoryexpand." + hash, true);
			string name = path.TrimEnd(Path.DirectorySeparatorChar).Split(Path.DirectorySeparatorChar).Last();
			ScriptDirectoryStructure sds = new ScriptDirectoryStructure(path, name, expanded, hash);

			foreach (string directory in Directory.GetDirectories(path))
				sds.Directories.Add(LoadScriptDirectoryStructure(directory));

			foreach (string filename in Directory.GetFiles(path, "*.js"))
			{
				bool retry = true;
				int retrycounter = 5;

				while (retry)
				{
					try
					{
						ScriptInfo si = new ScriptInfo(filename);
						sds.Scripts.Add(si);
						scriptinfo.Add(si);
						retry = false;
					}
					catch (IOException)
					{
						// The FileSystemWatcher can fire the event while the file is still being written, in that case we'll get
						// an IOException (file is locked by another process). So just try to load the file a couple times
						Thread.Sleep(100);
						retrycounter--;
						if (retrycounter == 0)
							retry = false;
					}
					catch (Exception e)
					{
						General.ErrorLogger.Add(ErrorType.Warning, "Failed to process " + filename + ": " + e.Message);
						General.WriteLogLine("Failed to process " + filename + ": " + e.Message);
						retry = false;
					}
				}
			}

			return sds;
		}

		/// <summary>
		/// Gets the name of the script file. This is either read from the .cfg file of the script or taken from the file name
		/// </summary>
		/// <param name="filename">Full path with file name of the script</param>
		/// <returns></returns>
		public static string GetScriptName(string filename)
		{
			string configfile = Path.Combine(Path.GetDirectoryName(filename), Path.GetFileNameWithoutExtension(filename)) + ".cfg";

			if (File.Exists(configfile))
			{
				Configuration cfg = new Configuration(configfile, true);
				string name = cfg.ReadSetting("name", string.Empty);

				if (!string.IsNullOrEmpty(name))
					return name;
			}

			return Path.GetFileNameWithoutExtension(filename);
		}

		public void EndOptionEdit()
		{
			panel.EndEdit();
		}

		internal Vector3D GetVector3DFromObject(object data)
		{
			if (data is Vector2D)
				return (Vector2D)data;
			else if (data is Vector2DWrapper)
				return new Vector2D(((Vector2DWrapper)data)._x, ((Vector2DWrapper)data)._y);
			else if (data is Vector3D)
				return (Vector3D)data;
			else if (data is Vector3DWrapper)
				return new Vector3D(((Vector3DWrapper)data)._x, ((Vector3DWrapper)data)._y, ((Vector3DWrapper)data)._z);
			else if (data.GetType().IsArray)
			{
				object[] rawvals = (object[])data;
				List<double> vals = new List<double>(rawvals.Length);

				// Make sure all values in the array are doubles or BigIntegers
				foreach (object rv in rawvals)
				{
					if (!(rv is double || rv is BigInteger))
						throw new CantConvertToVectorException("Values in array must be numbers.");

					if (rv is double d)
						vals.Add(d);
					else if(rv is BigInteger bi)
						vals.Add((double)bi);
				}

				if (vals.Count == 2)
					return new Vector2D(vals[0], vals[1]);
				if (vals.Count == 3)
					return new Vector3D(vals[0], vals[1], vals[2]);
			}
			else if (data is ExpandoObject)
			{
				IDictionary<string, object> eo = data as IDictionary<string, object>;
				double x = double.NaN;
				double y = double.NaN;
				double z = 0.0;

				if (eo.ContainsKey("x"))
				{
					try
					{
						x = Convert.ToDouble(eo["x"]);
					}
					catch (Exception e)
					{
						throw new CantConvertToVectorException("Can not convert 'x' property of data: " + e.Message);
					}
				}

				if (eo.ContainsKey("y"))
				{
					try
					{
						y = Convert.ToDouble(eo["y"]);
					}
					catch (Exception e)
					{
						throw new CantConvertToVectorException("Can not convert 'y' property of data: " + e.Message);
					}
				}

				if (eo.ContainsKey("z"))
				{
					try
					{
						z = Convert.ToDouble(eo["z"]);
					}
					catch (Exception e)
					{
						throw new CantConvertToVectorException("Can not convert 'z' property of data: " + e.Message);
					}
				}

				if (!double.IsNaN(x) && !double.IsNaN(y) && !double.IsNaN(z))
					return new Vector3D(x, y, z);
			}

			throw new CantConvertToVectorException("Data must be a Vector2D, Vector3D, an array of numbers, or an object with (x, y, z) members.");
		}

		internal object GetConvertedUniValue(UniValue uv)
		{
			switch ((UniversalType)uv.Type)
			{
				case UniversalType.AngleRadians:
				case UniversalType.AngleDegreesFloat:
				case UniversalType.Float:
					return Convert.ToDouble(uv.Value);
				case UniversalType.AngleDegrees:
				case UniversalType.AngleByte: //mxd
				case UniversalType.Color:
				case UniversalType.EnumBits:
				case UniversalType.EnumOption:
				case UniversalType.Integer:
				case UniversalType.LinedefTag:
				case UniversalType.LinedefType:
				case UniversalType.SectorEffect:
				case UniversalType.SectorTag:
				case UniversalType.ThingTag:
				case UniversalType.ThingType:
					return Convert.ToInt32(uv.Value);
				case UniversalType.Boolean:
					return Convert.ToBoolean(uv.Value);
				case UniversalType.Flat:
				case UniversalType.String:
				case UniversalType.Texture:
				case UniversalType.EnumStrings:
				case UniversalType.ThingClass:
					return Convert.ToString(uv.Value);
			}

			return null;
		}

		internal Type GetTypeFromUniversalType(int type)
		{
			switch ((UniversalType)type)
			{
				case UniversalType.AngleRadians:
				case UniversalType.AngleDegreesFloat:
				case UniversalType.Float:
					return typeof(double);
				case UniversalType.AngleDegrees:
				case UniversalType.AngleByte: //mxd
				case UniversalType.Color:
				case UniversalType.EnumBits:
				case UniversalType.EnumOption:
				case UniversalType.Integer:
				case UniversalType.LinedefTag:
				case UniversalType.LinedefType:
				case UniversalType.SectorEffect:
				case UniversalType.SectorTag:
				case UniversalType.ThingTag:
				case UniversalType.ThingType:
					return typeof(int);
				case UniversalType.Boolean:
					return typeof(bool);
				case UniversalType.Flat:
				case UniversalType.String:
				case UniversalType.Texture:
				case UniversalType.EnumStrings:
				case UniversalType.ThingClass:
					return typeof(string);
			}

			return null;
		}

		#region ================== Actions

		[BeginAction("udbscriptexecute")]
		public void ScriptExecute()
		{
			if (currentscript == null)
				return;

			scriptrunner = new ScriptRunner(currentscript);
			scriptrunnerform.ShowDialog();
		}

		[BeginAction("udbscriptexecuteslot1")]
		[BeginAction("udbscriptexecuteslot2")]
		[BeginAction("udbscriptexecuteslot3")]
		[BeginAction("udbscriptexecuteslot4")]
		[BeginAction("udbscriptexecuteslot5")]
		[BeginAction("udbscriptexecuteslot6")]
		[BeginAction("udbscriptexecuteslot7")]
		[BeginAction("udbscriptexecuteslot8")]
		[BeginAction("udbscriptexecuteslot9")]
		[BeginAction("udbscriptexecuteslot10")]
		[BeginAction("udbscriptexecuteslot11")]
		[BeginAction("udbscriptexecuteslot12")]
		[BeginAction("udbscriptexecuteslot13")]
		[BeginAction("udbscriptexecuteslot14")]
		[BeginAction("udbscriptexecuteslot15")]
		[BeginAction("udbscriptexecuteslot16")]
		[BeginAction("udbscriptexecuteslot17")]
		[BeginAction("udbscriptexecuteslot18")]
		[BeginAction("udbscriptexecuteslot19")]
		[BeginAction("udbscriptexecuteslot20")]
		[BeginAction("udbscriptexecuteslot21")]
		[BeginAction("udbscriptexecuteslot22")]
		[BeginAction("udbscriptexecuteslot23")]
		[BeginAction("udbscriptexecuteslot24")]
		[BeginAction("udbscriptexecuteslot25")]
		[BeginAction("udbscriptexecuteslot26")]
		[BeginAction("udbscriptexecuteslot27")]
		[BeginAction("udbscriptexecuteslot28")]
		[BeginAction("udbscriptexecuteslot29")]
		[BeginAction("udbscriptexecuteslot30")]
		public void ScriptExecuteSlot()
		{
			// Extract the slot number from the action name. The action name is something like udbscript__udbscriptexecuteslot1.
			// Not super nice, but better than having 30 identical methods for each slot.
			Regex re = new Regex(@"(\d+)$");
			Match m = re.Match(General.Actions.Current.Name);

			if(m.Success)
			{
				int slot = int.Parse(m.Value);

				// Check if there's a ScriptInfo in the slot and run it if so
				if (scriptslots.ContainsKey(slot) && scriptslots[slot] != null)
				{
					scriptrunner = new ScriptRunner(scriptslots[slot]);
					scriptrunnerform.ShowDialog();
				}
			}
		}

		#endregion
	}
}
