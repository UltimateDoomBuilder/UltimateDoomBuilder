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
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Windows;
using CodeImp.DoomBuilder.UDBScript.Wrapper;
using Jint;
using Jint.Runtime;
using Jint.Runtime.Interop;
using Esprima;

#endregion

namespace CodeImp.DoomBuilder.UDBScript
{
	class ScriptRunner
	{
		#region ================== Variables

		private ScriptInfo scriptinfo;
		Engine engine;
		Stopwatch stopwatch;
		int oldprocessingcount;

		#endregion

		#region ================== Constructor

		public ScriptRunner(ScriptInfo scriptoption)
		{
			this.scriptinfo = scriptoption;
			stopwatch = new Stopwatch();
		}

		#endregion

		#region ================== Methods

		/// <summary>
		/// Stops the timer, pausing the script's runtime constraint
		/// </summary>
		public void StopTimer()
		{
			stopwatch.Stop();
		}

		/// <summary>
		/// Resumes the timer, resuming the script's runtime constraint
		/// </summary>
		public void ResumeTimer()
		{
			stopwatch.Start();
		}

		/// <summary>
		/// Shows a message box with an "OK" button
		/// </summary>
		/// <param name="message">Message to show</param>
		public void ShowMessage(object message)
		{
			BuilderPlug.Me.ScriptRunnerForm.InvokePaused(new Action(() => {
				if (message == null)
					message = string.Empty;

				stopwatch.Stop();
				MessageForm mf = new MessageForm("OK", null, message.ToString());
				DialogResult result = mf.ShowDialog();
				stopwatch.Start();

				if (result == DialogResult.Abort)
					throw new UserScriptAbortException();
			}));
		}

		/// <summary>
		/// Shows a message box with an "Yes" and "No" button
		/// </summary>
		/// <param name="message">Message to show</param>
		/// <returns>true if "Yes" was clicked, false if "No" was clicked</returns>
		public bool ShowMessageYesNo(object message)
		{
			return (bool)BuilderPlug.Me.ScriptRunnerForm.InvokePaused(new Func<bool>(() =>
			{
				if (message == null)
					message = string.Empty;

				stopwatch.Stop();
				MessageForm mf = new MessageForm("Yes", "No", message.ToString());
				DialogResult result = mf.ShowDialog();
				stopwatch.Start();

				if (result == DialogResult.Abort)
					throw new UserScriptAbortException();

				return result == DialogResult.OK ? true : false;
			}));
		}

		/// <summary>
		/// Exist the script prematurely without undoing its changes.
		/// </summary>
		/// <param name="s"></param>
		private void ExitScript(string s = null)
		{
			if (string.IsNullOrEmpty(s))
				throw new ExitScriptException();

			throw new ExitScriptException(s);
		}

		/// <summary>
		/// Exist the script prematurely with undoing its changes.
		/// </summary>
		/// <param name="s"></param>
		private void DieScript(string s = null)
		{
			if (string.IsNullOrEmpty(s))
				throw new DieScriptException();

			throw new DieScriptException(s);
		}

		public ScriptRuntimeException CreateRuntimeException(string message)
		{
			return new ScriptRuntimeException(message);
		}

		/// <summary>
		/// Imports the code of all script library files in a single string
		/// </summary>
		/// <param name="engine">Scripting engine to load the code into</param>
		/// <param name="errortext">Errors that occured while loading the library code</param>
		/// <returns>true if there were no errors, false if there were errors</returns>
		private bool ImportLibraryCode(Engine engine, out string errortext)
		{
			string path = Path.Combine(General.AppPath, "UDBScript", "Libraries");
			string[] files = Directory.GetFiles(path, "*.js", SearchOption.AllDirectories);

			errortext = string.Empty;

			foreach (string file in files)
			{
				try
				{
					engine.Execute(File.ReadAllText(file), file.Remove(0, General.AppPath.Length));
				}
				catch (ParserException e)
				{
					MessageBox.Show("There was an error while loading the library " + file + ":\n\n" + e.Message, "Script error", MessageBoxButtons.OK, MessageBoxIcon.Error);

					return false;
				}
				catch (JavaScriptException e)
				{
					if (e.Error.Type != Jint.Runtime.Types.String)
					{
						UDBScriptErrorForm sef = new UDBScriptErrorForm(e.Message, e.JavaScriptStackTrace, e.StackTrace);
						sef.ShowDialog();
					}
					else
						General.Interface.DisplayStatus(StatusType.Warning, e.Message); // We get here if "throw" is used in a script

					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Handles the different exceptions we're expecting, and withdraws the undo snapshot if necessary.
		/// </summary>
		/// <param name="e">The exception to handle</param>
		public void HandleExceptions(Exception e)
		{
			bool abort = false;

			if(e is UserScriptAbortException)
			{
				General.Interface.DisplayStatus(StatusType.Warning, "Script aborted");
				abort = true;
			}
			else if(e is ParserException)
			{
				MessageBox.Show("There is an error while parsing the script:\n\n" + e.Message, "Script error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				abort = true;
			}
			else if(e is JavaScriptException jse)
			{
				if (jse.Error.Type != Jint.Runtime.Types.String)
				{
					UDBScriptErrorForm sef = new UDBScriptErrorForm(jse.Message, jse.JavaScriptStackTrace, jse.StackTrace);
					sef.ShowDialog();
				}
				else
					General.Interface.DisplayStatus(StatusType.Warning, jse.Message); // We get here if "throw" is used in a script

				abort = true;
			}
			else if(e is ExitScriptException)
			{
				if (!string.IsNullOrEmpty(e.Message))
					General.Interface.DisplayStatus(StatusType.Ready, e.Message);
			}
			else if(e is DieScriptException)
			{
				if (!string.IsNullOrEmpty(e.Message))
					General.Interface.DisplayStatus(StatusType.Warning, e.Message);

				abort = true;
			}
			else if(e is ExecutionCanceledException)
			{
				abort = true;
			}
			else // Catch anything else we didn't think about
			{
				UDBScriptErrorForm sef = new UDBScriptErrorForm(e.Message, string.Empty, e.StackTrace);
				sef.ShowDialog();

				abort = true;
			}

			if (abort)
				General.Map.UndoRedo.WithdrawUndo();
		}

		/// <summary>
		/// Makes sure that only properties for the currect feature version are available to scripts.
		/// </summary>
		/// <param name="info">MemberInfo about the property that's being accessed</param>
		/// <returns>true if property can be accessed, false otherwise</returns>
		private bool MemberFilter(MemberInfo info)
		{
			if (info.Name == nameof(GetType))
				return false;

			if (info.GetCustomAttribute(typeof(UDBScriptSettingsAttribute)) is UDBScriptSettingsAttribute sa)
				return sa.MinVersion <= scriptinfo.Version;

			return true;
		}

		/*
		private JsValue GetObjectMember(Engine engine, object target, string memberName)
		{
			Type t = target.GetType();
			MethodInfo mi = t.GetMethod(memberName);
			if (mi != null)
			{
				var attr = mi.GetCustomAttribute<UDBScriptSettingsAttribute>(false);
				if (attr != null && scriptinfo.Version < attr.MinVersion)
					throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException($"{t.Name} requires UDBScript version {attr.MinVersion} or higher.");

			}
			if (t.GetCustomAttribute(typeof(UDBScriptSettingsAttribute)) is UDBScriptSettingsAttribute sa)
			{
				if (scriptinfo.Version < sa.MinVersion)
					throw BuilderPlug.Me.ScriptRunner.CreateRuntimeException($"{t.Name} requires UDBScript version {sa.MinVersion} or higher.");
			}

			return null;
		}
		*/

		/// <summary>
		/// Sets everything up for running the script. This has to be done on the UI thread.
		/// </summary>
		/// <param name="cancellationtoken">Cancellation token to cancel the running script</param>
		public void PreRun(CancellationToken cancellationtoken, IProgress<int> progress, IProgress<string> status, IProgress<string> log)
		{
			string importlibraryerrors;

			// If the script requires a higher version of UDBScript than this version ask the user if they want
			// to execute it anyways. Remember the choice for this session if "yes" was selected.
			if (scriptinfo.Version > BuilderPlug.UDB_SCRIPT_VERSION && !scriptinfo.IgnoreVersion)
			{
				if (MessageBox.Show("The script requires a higher version of the feature set than this version of UDBScript supports. Executing this script might fail\n\nRequired feature version: " + scriptinfo.Version + "\nUDBScript feature version: " + BuilderPlug.UDB_SCRIPT_VERSION + "\n\nExecute anyway?", "UDBScript feature version too low", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
					return;

				scriptinfo.IgnoreVersion = true;
			}

			// Make sure the option value gets saved if an option is currently being edited
			BuilderPlug.Me.EndOptionEdit();
			General.Interface.Focus();

			// Set engine options
			Options options = new Options();
			options.CancellationToken(cancellationtoken);
			options.AllowOperatorOverloading();

			options.SetTypeResolver(new TypeResolver
			{
				MemberFilter = MemberFilter// member => member.Name != nameof(GetType)
			});

			//options.SetMemberAccessor(GetObjectMember);

			/*
			options.SetWrapObjectHandler((eng, obj) =>
			{
				var wrapper = new ObjectWrapper(eng, obj);
				if (wrapper.IsArrayLike || obj is BlockMapQueryResult)
				{
					wrapper.SetPrototypeOf(eng.Realm.Intrinsics.Array.PrototypeObject);
				}
				return wrapper;
			});
			*/

			options.CatchClrExceptions(e => e is ScriptRuntimeException || e is CantConvertToVectorException);

			// Create the script engine
			engine = new Engine(options);

			// Scripts with API version smaller than 4 will use the old global objects, starting from API
			// version 4 the new global "UDB" object
			if (scriptinfo.Version < 4)
			{
				engine.SetValue("showMessage", new Action<object>(ShowMessage));
				engine.SetValue("showMessageYesNo", new Func<object, bool>(ShowMessageYesNo));
				engine.SetValue("exit", new Action<string>(ExitScript));
				engine.SetValue("die", new Action<string>(DieScript));
				engine.SetValue("QueryOptions", TypeReference.CreateTypeReference(engine, typeof(QueryOptions)));
				engine.SetValue("ScriptOptions", scriptinfo.GetScriptOptionsObject());
				engine.SetValue("Map", new MapWrapper());
				engine.SetValue("GameConfiguration", new GameConfigurationWrapper());
				engine.SetValue("Angle2D", TypeReference.CreateTypeReference(engine, typeof(Angle2DWrapper)));
				engine.SetValue("Vector3D", TypeReference.CreateTypeReference(engine, typeof(Vector3DWrapper)));
				engine.SetValue("Vector2D", TypeReference.CreateTypeReference(engine, typeof(Vector2DWrapper)));
				engine.SetValue("Line2D", TypeReference.CreateTypeReference(engine, typeof(Line2DWrapper)));
				engine.SetValue("UniValue", TypeReference.CreateTypeReference(engine, typeof(UniValue)));
				engine.SetValue("Data", TypeReference.CreateTypeReference(engine, typeof(DataWrapper)));

				// These can not be directly instanciated and don't have static method, but it's required to
				// for example use "instanceof" in scripts
				engine.SetValue("Linedef", TypeReference.CreateTypeReference(engine, typeof(LinedefWrapper)));
				engine.SetValue("Sector", TypeReference.CreateTypeReference(engine, typeof(SectorWrapper)));
				engine.SetValue("Sidedef", TypeReference.CreateTypeReference(engine, typeof(SidedefWrapper)));
				engine.SetValue("Thing", TypeReference.CreateTypeReference(engine, typeof(ThingWrapper)));
				engine.SetValue("Vertex", TypeReference.CreateTypeReference(engine, typeof(VertexWrapper)));
			}
			else
			{
				engine.SetValue("UDB", new UDBWrapper(engine, scriptinfo, progress, status, log));
			}

#if DEBUG
			engine.SetValue("log", new Action<object>(Console.WriteLine));
#endif

			// Import all library files into the current engine
			if (ImportLibraryCode(engine, out importlibraryerrors) == false)
				return;

			// Tell the mode that a script is about to be run
			General.Editing.Mode.OnScriptRunBegin();

			General.Map.UndoRedo.CreateUndo("Run script " + scriptinfo.Name);
			General.Map.Map.ClearAllMarks(false);

			General.Map.Map.IsSafeToAccess = false;

			// Disable all processing. Has to be done as many times as it was enabled.
			// Save old value since after running the script we need to enable it as many times
			oldprocessingcount = General.Interface.ProcessingCount;
			for (int i = 0; i < oldprocessingcount; i++)
				General.Interface.DisableProcessing();
		}

		/// <summary>
		/// Runs the script
		/// </summary>
		public void Run()
		{
			//engine.SetValue("ProgressInfo", new ProgressInfo(progress, status, log));
			// Read the current script file
			string script = File.ReadAllText(scriptinfo.ScriptFile);

			// Run the script file
			stopwatch.Reset();
			stopwatch.Start();
			engine.Execute(script, scriptinfo.ScriptFile.Remove(0, General.AppPath.Length));
			stopwatch.Stop();
		}

		/// <summary>
		/// Cleanups and updates after the script stopped running. Has to be called from the UI thread.
		/// </summary>
		public void PostRun()
		{
			General.Map.Map.IsSafeToAccess = true;

			// Do some updates
			General.Map.Map.Update();
			General.Map.ThingsFilter.Update();
			//General.Interface.RedrawDisplay();

			// Tell the mode that running the script ended
			General.Editing.Mode.OnScriptRunEnd();

			// Enable processing again, if required
			for (int i = 0; i < oldprocessingcount; i++)
				General.Interface.EnableProcessing();
		}

		public string GetRuntimeString()
		{
			return string.Format("{0:D2}:{1:D2}:{2:D2}.{3:D}", stopwatch.Elapsed.Hours, stopwatch.Elapsed.Minutes, stopwatch.Elapsed.Seconds, stopwatch.Elapsed.Milliseconds);
		}

		#endregion
	}
}
