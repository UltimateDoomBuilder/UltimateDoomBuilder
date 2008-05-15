#region ================== Namespaces

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.IO;
using CodeImp.DoomBuilder.Editing;

#endregion

namespace CodeImp.DoomBuilder.Plugins
{
	/// <summary>
	/// This is the key link between the Doom Builder core and the plugin.
	/// Every plugin must expose a single class that inherits this class.
	/// </summary>
	public class Plug : IDisposable
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		// Internals
		private Plugin plugin;
		
		// Disposing
		private bool isdisposed = false;

		#endregion

		#region ================== Properties

		// Internals
		internal Plugin Plugin { get { return plugin; } set { plugin = value; } }
		
		/// <summary>
		/// Indicates if the plugin has been disposed.
		/// </summary>
		public bool IsDisposed { get { return isdisposed; } }

		#endregion

		#region ================== Constructor / Disposer

		/// <summary>
		/// This is the key link between the Doom Builder core and the plugin.
		/// Every plugin must expose a single class that inherits this class.
		/// <para>
		/// NOTE: Some methods cannot be used in this constructor, because the plugin
		/// is not yet fully initialized. Instead, use the Initialize method to do
		/// your initializations.
		/// </para>
		/// </summary>
		public Plug()
		{
			// Initialize

			// We have no destructor
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// This is called by the Doom Builder core when the plugin is being disposed.
		/// </summary>
		public virtual void Dispose()
		{
			// Not already disposed?
			if(!isdisposed)
			{
				// Clean up
				plugin = null;
				
				// Done
				isdisposed = true;
			}
		}

		#endregion

		#region ================== Methods

		/// <summary>
		/// This finds the embedded resource with the specified name in the plugin and creates
		/// a Stream from it. Returns null when the embedded resource cannot be found.
		/// </summary>
		/// <param name="resourcename">Name of the resource in the plugin.</param>
		/// <returns>Returns a Stream of the embedded resource,
		/// or null when the resource cannot be found.</returns>
		public Stream GetResourceStream(string resourcename)
		{
			return plugin.GetResourceStream(resourcename);
		}

		#endregion

		#region ================== Events

		/// <summary>
		/// This is called after the constructor to allow a plugin to initialize.
		/// </summary>
		public virtual void OnInitialize()
		{
		}

		/// <summary>
		/// This is called by the Doom Builder core when the user chose to reload the resources.
		/// </summary>
		public virtual void OnReloadResources()
		{
		}

		/// <summary>
		/// This is called by the Doom Builder core when the editing mode changes.
		/// </summary>
		/// <param name="oldmode">The previous editing mode</param>
		/// <param name="newmode">The new editing mode</param>
		public virtual void OnModeChange(EditMode oldmode, EditMode newmode)
		{
		}

		/// <summary>
		/// Called by the Doom Builder core when the user changes the program configuration (F5).
		/// </summary>
		public virtual void OnProgramReconfigure()
		{
		}

		/// <summary>
		/// Called by the Doom Builder core when the user changes the map settings (F2).
		/// </summary>
		public virtual void OnMapReconfigure()
		{
		}
		
		#endregion
	}
}
