
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
using System.IO;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Editing;
using System.Reflection;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Windows;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.IO;
using System.Collections.Specialized;

#endregion

namespace CodeImp.DoomBuilder.Plugins
{
	internal class PluginManager
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		// Plugins
		private List<Plugin> plugins;
		
		// Disposing
		private bool isdisposed;

		#endregion

		#region ================== Properties
		
		internal List<Plugin> Plugins { get { return plugins; } }
		public bool IsDisposed { get { return isdisposed; } }
		
		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public PluginManager()
		{
			// Make lists
			this.plugins = new List<Plugin>();

			// We have no destructor
			GC.SuppressFinalize(this);
		}

		// Disposer
		public void Dispose()
		{
			// Not already disposed?
			if(!isdisposed)
			{
				// Clean up
				foreach(Plugin p in plugins) p.Dispose();
				
				// Done
				isdisposed = true;
			}
		}

		#endregion

		#region ================== Methods
		
		// This creates a list of assemblies
		public List<Assembly> GetPluginAssemblies()
		{
			List<Assembly> asms = new List<Assembly>(plugins.Count);
			foreach(Plugin p in plugins)
				asms.Add(p.Assembly);
			return asms;
		}
		
		
		// This loads all plugins
		public void LoadAllPlugins()
		{
			IDictionary loadorderfiles = new ListDictionary();
			
			try
			{
				// Load the load order cfg
				Configuration cfg = new Configuration(Path.Combine(General.PluginsPath, "Loadorder.cfg"), true);

				// Check for erors
				if(cfg.ErrorResult)
				{
					// Error in configuration
					General.ErrorLogger.Add(ErrorType.Error, "Unable to read the load order configuration file \"Loadorder.cfg\". " +
											"Error in file \"" + cfg.ErrorFile + "\" near line " + cfg.ErrorLine + ": " + cfg.ErrorDescription);
				}
				else
				{
					loadorderfiles = cfg.ReadSetting("loadorder", new ListDictionary());
				}
			}
			catch(Exception e)
			{
				// Unable to load configuration
				General.ErrorLogger.Add(ErrorType.Error, "Unable to read the load order configuration file \"Loadorder.cfg\". " + e.GetType().Name + ": " + e.Message);
				General.WriteLogLine(e.StackTrace);
			}
			
			// Find all .dll files
			List<string> filenames = new List<string>(Directory.GetFiles(General.PluginsPath, "*.dll", SearchOption.TopDirectoryOnly));
			
			// Load the ones in order as specified by the load order cfg
			foreach(DictionaryEntry de in loadorderfiles)
			{
				string loadfilename = de.Key.ToString();
				
				// Find the file in the list
				int filenameindex = -1;
				for(int i = 0; i < filenames.Count; i++)
					if(string.Compare(Path.GetFileName(filenames[i]), loadfilename, true) == 0)
						filenameindex = i;
				
				if(filenameindex > -1)
				{
					// Load plugin from this file
					Plugin p;
					try
					{
						p = new Plugin(filenames[filenameindex]);
					}
					catch(InvalidProgramException)
					{
						p = null;
					}
					
					// Continue if no errors
					if((p != null) && (!p.IsDisposed))
					{
						// Add to plugins
						this.plugins.Add(p);
						
						// Load actions
						General.Actions.LoadActions(p.Assembly);

						//mxd. And hints
						General.Hints.LoadHints(p.Assembly);
						
						// Plugin is now initialized
						p.Plug.OnInitialize();
					}
					
					// Remove the plugin from list
					filenames.RemoveAt(filenameindex);
				}
			}
			
			// Now load the remaining files
			foreach(string fn in filenames)
			{
				// Load plugin from this file
				Plugin p;
				try
				{
					p = new Plugin(fn);
				}
				catch(InvalidProgramException)
				{
					p = null;
				}
				
				// Continue if no errors
				if((p != null) && (!p.IsDisposed))
				{
					// Add to plugins
					this.plugins.Add(p);
					
					// Load actions
					General.Actions.LoadActions(p.Assembly);

					//mxd. And hints
					General.Hints.LoadHints(p.Assembly);
					
					// Plugin is now initialized
					p.Plug.OnInitialize();
				}
			}
		}
		
		// This returns a plugin by assembly, or null when plugin cannot be found
		public Plugin FindPluginByAssembly(Assembly assembly)
		{
			// Go for all plugins the find the one with matching assembly
			foreach(Plugin p in plugins)
			{
				if(Equals(p.Assembly, assembly)) return p;
			}

			// Nothing found
			return null;
		}

		#endregion

		#region ================== Events
		
		public bool ModeChanges(EditMode oldmode, EditMode newmode)
		{
			bool result = true;
			foreach(Plugin p in plugins) result &= p.Plug.OnModeChange(oldmode, newmode);
			return result;
		}

		public bool OnCopyBegin()
		{
			bool result = true;
			foreach(Plugin p in plugins) result &= p.Plug.OnCopyBegin(result);
			return result;
		}

		public bool OnPasteBegin(PasteOptions options)
		{
			bool result = true;
			foreach(Plugin p in plugins) result &= p.Plug.OnPasteBegin(options.Copy(), result);
			return result;
		}

		public bool OnUndoBegin()
		{
			bool result = true;
			foreach(Plugin p in plugins) result &= p.Plug.OnUndoBegin(result);
			return result;
		}

		public bool OnRedoBegin()
		{
			bool result = true;
			foreach(Plugin p in plugins) result &= p.Plug.OnRedoBegin(result);
			return result;
		}

		public void ReloadResources() { foreach(Plugin p in plugins) p.Plug.OnReloadResources(); }
		public void ProgramReconfigure() { foreach(Plugin p in plugins) p.Plug.OnProgramReconfigure(); }
		public void MapReconfigure() { foreach(Plugin p in plugins) p.Plug.OnMapReconfigure(); }
		public void OnCopyEnd() { foreach(Plugin p in plugins) p.Plug.OnCopyEnd(); }
		public void OnPasteEnd(PasteOptions options) { foreach(Plugin p in plugins) p.Plug.OnPasteEnd(options.Copy()); }
		public void OnUndoEnd() { foreach(Plugin p in plugins) p.Plug.OnUndoEnd(); }
		public void OnRedoEnd() { foreach(Plugin p in plugins) p.Plug.OnRedoEnd(); }
		public void OnUndoCreated() { foreach(Plugin p in plugins) p.Plug.OnUndoCreated(); }
		public void OnUndoWithdrawn() { foreach(Plugin p in plugins) p.Plug.OnUndoWithdrawn(); }
		public void OnMapOpenBegin() { foreach(Plugin p in plugins) p.Plug.OnMapOpenBegin(); }
		public void OnMapOpenEnd() { foreach(Plugin p in plugins) p.Plug.OnMapOpenEnd(); }
		public void OnMapNewBegin() { foreach(Plugin p in plugins) p.Plug.OnMapNewBegin(); }
		public void OnMapNewEnd() { foreach(Plugin p in plugins) p.Plug.OnMapNewEnd(); }
		public void OnMapCloseBegin() { foreach(Plugin p in plugins) p.Plug.OnMapCloseBegin(); }
        public void OnMapCloseEnd() { foreach(Plugin p in plugins) p.Plug.OnMapCloseEnd(); }
		public void OnMapSaveBegin(SavePurpose purpose) { foreach(Plugin p in plugins) p.Plug.OnMapSaveBegin(purpose); }
		public void OnMapSaveEnd(SavePurpose purpose) { foreach(Plugin p in plugins) p.Plug.OnMapSaveEnd(purpose); }
		public void OnMapSetChangeBegin() { foreach(Plugin p in plugins) p.Plug.OnMapSetChangeBegin(); }
		public void OnMapSetChangeEnd() { foreach(Plugin p in plugins) p.Plug.OnMapSetChangeEnd(); }
		public void OnSectorCeilingSurfaceUpdate(Sector s, ref FlatVertex[] vertices) { foreach(Plugin p in plugins) p.Plug.OnSectorCeilingSurfaceUpdate(s, ref vertices); }
		public void OnSectorFloorSurfaceUpdate(Sector s, ref FlatVertex[] vertices) { foreach(Plugin p in plugins) p.Plug.OnSectorFloorSurfaceUpdate(s, ref vertices); }
		public void OnShowPreferences(PreferencesController controller) { foreach(Plugin p in plugins) p.Plug.OnShowPreferences(controller); }
		public void OnClosePreferences(PreferencesController controller) { foreach(Plugin p in plugins) p.Plug.OnClosePreferences(controller); }
		public void OnActionBegin(Actions.Action action) { foreach(Plugin p in plugins) p.Plug.OnActionBegin(action); }
		public void OnActionEnd(Actions.Action action) { foreach(Plugin p in plugins) p.Plug.OnActionEnd(action); }
		public void OnEditEngage(EditMode oldmode, EditMode newmode) { foreach(Plugin p in plugins) p.Plug.OnEditEngage(oldmode, newmode); }
		public void OnEditDisengage(EditMode oldmode, EditMode newmode) { foreach(Plugin p in plugins) p.Plug.OnEditDisengage(oldmode, newmode); }
		public void OnEditCancel() { foreach(Plugin p in plugins) p.Plug.OnEditCancel(); }
		public void OnEditAccept() { foreach(Plugin p in plugins) p.Plug.OnEditAccept(); }
		public void OnEditMouseClick(MouseEventArgs e) { foreach(Plugin p in plugins) p.Plug.OnEditMouseClick(e); }
		public void OnEditMouseDoubleClick(MouseEventArgs e) { foreach(Plugin p in plugins) p.Plug.OnEditMouseDoubleClick(e); }
		public void OnEditMouseDown(MouseEventArgs e) { foreach(Plugin p in plugins) p.Plug.OnEditMouseDown(e); }
		public void OnEditMouseEnter(EventArgs e) { foreach(Plugin p in plugins) p.Plug.OnEditMouseEnter(e); }
		public void OnEditMouseLeave(EventArgs e) { foreach(Plugin p in plugins) p.Plug.OnEditMouseLeave(e); }
		public void OnEditMouseMove(MouseEventArgs e) { foreach(Plugin p in plugins) p.Plug.OnEditMouseMove(e); }
		public void OnEditMouseUp(MouseEventArgs e) { foreach(Plugin p in plugins) p.Plug.OnEditMouseUp(e); }
		public void OnEditKeyDown(KeyEventArgs e) { foreach(Plugin p in plugins) p.Plug.OnEditKeyDown(e); }
		public void OnEditKeyUp(KeyEventArgs e) { foreach(Plugin p in plugins) p.Plug.OnEditKeyUp(e); }
		public void OnEditMouseInput(Vector2D delta) { foreach(Plugin p in plugins) p.Plug.OnEditMouseInput(delta); }
		public void OnEditRedrawDisplayBegin() { foreach(Plugin p in plugins) p.Plug.OnEditRedrawDisplayBegin(); }
		public void OnEditRedrawDisplayEnd() { foreach(Plugin p in plugins) p.Plug.OnEditRedrawDisplayEnd(); }
		public void OnPresentDisplayBegin() { foreach(Plugin p in plugins) p.Plug.OnPresentDisplayBegin(); }
		public void OnMapNodesRebuilt() { foreach(Plugin p in plugins) p.Plug.OnMapNodesRebuilt(); }

		//mxd. Highlight events
		public void OnHighlightSector(Sector s) { foreach(Plugin p in plugins) p.Plug.OnHighlightSector(s); }
		public void OnHighlightLinedef(Linedef l) { foreach(Plugin p in plugins) p.Plug.OnHighlightLinedef(l); }
		public void OnHighlightThing(Thing t) { foreach(Plugin p in plugins) p.Plug.OnHighlightThing(t); }
		public void OnHighlightVertex(Vertex v) { foreach(Plugin p in plugins) p.Plug.OnHighlightVertex(v); }
		public void OnHighlightRefreshed(object o) { foreach(Plugin p in plugins) p.Plug.OnHighlightRefreshed(o); }
		public void OnHighlightLost() { foreach(Plugin p in plugins) p.Plug.OnHighlightLost(); }
		
		#endregion
	}
}
