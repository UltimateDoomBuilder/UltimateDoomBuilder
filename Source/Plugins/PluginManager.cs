
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
using System.Text;
using System.IO;
using CodeImp.DoomBuilder.Editing;
using System.Reflection;

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
		
		// Modes
		private List<EditModeInfo> editmodes;
		
		// Disposing
		private bool isdisposed = false;

		#endregion

		#region ================== Properties

		public ICollection<EditModeInfo> EditModes { get { return editmodes; } }
		public bool IsDisposed { get { return isdisposed; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public PluginManager()
		{
			string[] filenames;
			Type[] editclasses;
			EditModeAttribute[] emattrs;
			EditModeInfo editmodeinfo;
			Plugin p;
			
			// Make lists
			this.plugins = new List<Plugin>();
			this.editmodes = new List<EditModeInfo>();
			
			// Find all .dll files
			filenames = Directory.GetFiles(General.PluginsPath, "*.dll", SearchOption.TopDirectoryOnly);
			foreach(string fn in filenames)
			{
				// Load plugin from this file
				General.MainWindow.DisplayStatus("Loading plugin '" + Path.GetFileName(fn) + "'...");
				try
				{
					p = new Plugin(fn);
				}
				catch(InvalidProgramException)
				{
					General.WriteLogLine("WARNING: Plugin file '" + Path.GetFileName(fn) + "' was not loaded.");
					p = null;
				}

				// Continue if no errors
				if((p != null) && (!p.IsDisposed))
				{
					// Add to plugins
					this.plugins.Add(p);

					// For all classes that inherit from EditMode
					editclasses = p.FindClasses(typeof(EditMode));
					foreach(Type t in editclasses)
					{
						// For all defined EditMode attributes
						emattrs = (EditModeAttribute[])t.GetCustomAttributes(typeof(EditModeAttribute), true);
						foreach(EditModeAttribute a in emattrs)
						{
							// Make edit mode information
							editmodeinfo = new EditModeInfo(p, t, a);
							editmodes.Add(editmodeinfo);
						}
					}
				}
			}

			// Sort the list in order for buttons
			editmodes.Sort();

			// Go for all edit modes to add buttons
			foreach(EditModeInfo emi in editmodes)
			{
				// Add all non-config-specific buttons to interface
				if((emi.ButtonImage != null) && (emi.ButtonDesc != null) && !emi.ConfigSpecific)
					General.MainWindow.AddEditModeButton(emi);
			}

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

		// This returns specific editing mode info by name
		public EditModeInfo GetEditModeInfo(string editmodename)
		{
			// Find the edit mode
			foreach(EditModeInfo emi in editmodes)
			{
				// Mode matches class name?
				if(emi.ToString() == editmodename) return emi;
			}

			// No such mode found
			return null;
		}
		
		// This is called when the game canfiguration is set or changed
		public void GameConfigurationChanged()
		{
			// Remove all config-specific editing mode buttons from toolbar
			General.MainWindow.RemoveSpecificEditModeButtons();

			// Go for all edit modes to add buttons
			foreach(EditModeInfo emi in editmodes)
			{
				// Add only non-config-specific buttons to interface
				if((emi.ButtonImage != null) && (emi.ButtonDesc != null) && emi.ConfigSpecific)
				{
					// Add if this button is specified by the game config
					if(General.Map.Config.IsEditModeSpecified(emi.Type.Name))
						General.MainWindow.AddEditModeButton(emi);
				}
			}
		}
		
		// This returns a plugin by assembly, or null when plugin cannot be found
		public Plugin FindPluginByAssembly(Assembly assembly)
		{
			// Go for all plugins the find the one with matching assembly
			foreach(Plugin p in plugins)
			{
				if(p.Assembly == assembly) return p;
			}

			// Nothing found
			return null;
		}

		// This calls ReloadResources on all plugins
		public void ReloadResources()
		{
			foreach(Plugin p in plugins)
				p.Plug.OnReloadResources();
		}

		// This calls ModeChanges on all plugins
		public void ModeChanges(EditMode oldmode, EditMode newmode)
		{
			foreach(Plugin p in plugins)
				p.Plug.OnModeChange(oldmode, newmode);
		}
		
		#endregion
	}
}
