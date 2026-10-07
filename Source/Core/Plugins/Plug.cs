#region ================== Namespaces

using System;
using System.IO;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Editing;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Windows;
using CodeImp.DoomBuilder.Config;

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
		private bool isdisposed;

		#endregion
		
		#region ================== Properties
		
		// Internals
		internal Plugin Plugin { get { return plugin; } set { plugin = value; } }
		
		/// <summary>
		/// Indicates if the plugin has been disposed.
		/// </summary>
		public bool IsDisposed { get { return isdisposed; } }
		
		/// <summary>
		/// Override this to return a more descriptive name for your plugin.
		/// Default is the library filename without extension.
		/// </summary>
		public virtual string Name { get { return plugin.Name; } }

		/// <summary>
		/// Override this to return the minimum revision of the Doom Builder core that is
		/// required to use this plugin. You can find the revision number in the About dialog,
		/// it is the right most part of the version number.
		/// </summary>
		public virtual int MinimumRevision { get { return 0; } }

		/// <summary>
		/// Set to true to indicate that plugin revision number must match the main module revision number.
		/// </summary>
		public virtual bool StrictRevisionMatching { get { return false; } } //mxd
		
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
		/// This iscalled when the ceiling surface buffer is updated for a sector. The plugin can
		/// modify the vertices to change how the surface is presented to the user.
		/// </summary>
		public virtual void OnSectorCeilingSurfaceUpdate(Sector s, ref FlatVertex[] vertices) { }

		/// <summary>
		/// This iscalled when the floor surface buffer is updated for a sector. The plugin can
		/// modify the vertices to change how the surface is presented to the user.
		/// </summary>
		public virtual void OnSectorFloorSurfaceUpdate(Sector s, ref FlatVertex[] vertices) { }

		/// <summary>
		/// Occurs before a map is opened.
		/// </summary>
		public virtual void OnMapOpenBegin() { }

		/// <summary>
		/// Occurs after a map is opened.
		/// </summary>
		public virtual void OnMapOpenEnd() { }

		/// <summary>
		/// Occurs before a new map is created.
		/// </summary>
		public virtual void OnMapNewBegin() { }

		/// <summary>
		/// Occurs after a new map is created.
		/// </summary>
		public virtual void OnMapNewEnd() { }

		/// <summary>
		/// Occurs before the map is closed.
		/// </summary>
		public virtual void OnMapCloseBegin() { }

        /// <summary>
        /// Occurs after a the map is closed.
        /// </summary>
        public virtual void OnMapCloseEnd() { }

		/// <summary>
		/// Occurs before a map is saved.
		/// </summary>
		public virtual void OnMapSaveBegin(SavePurpose purpose) { }

		/// <summary>
		/// Occurs after a map is saved.
		/// </summary>
		public virtual void OnMapSaveEnd(SavePurpose purpose) { }

		/// <summary>
		/// Occurs before the MapSet is changed. This means that the active MapSet will be disposed and changed to a new one.
		/// </summary>
		public virtual void OnMapSetChangeBegin() { }

		/// <summary>
		/// Occurs after the MapSet is changed.
		/// </summary>
		public virtual void OnMapSetChangeEnd() { }

		/// <summary>
		/// This is called after the constructor to allow a plugin to initialize.
		/// </summary>
		public virtual void OnInitialize() { }

		/// <summary>
		/// This is called when the user chose to reload the resources.
		/// </summary>
		public virtual void OnReloadResources() { }

		/// <summary>
		/// This is called by the Doom Builder core when the editing mode changes.
		/// Return false to abort the mode change.
		/// </summary>
		/// <param name="oldmode">The previous editing mode</param>
		/// <param name="newmode">The new editing mode</param>
		public virtual bool OnModeChange(EditMode oldmode, EditMode newmode) { return true; }

		/// <summary>
		/// Called by the Doom Builder core when the user changes the program configuration (F5).
		/// </summary>
		public virtual void OnProgramReconfigure() { }

		/// <summary>
		/// Called by the Doom Builder core when the user changes the map settings (F2).
		/// </summary>
		public virtual void OnMapReconfigure() { }

		/// <summary>
		/// Called by the Doom Builder core when the user wants to copy selected geometry.
		/// Return false to abort the copy operation.
		/// The result parameter is false when the operation was already aborted by another plugin.
		/// </summary>
		public virtual bool OnCopyBegin(bool result) { return true; }

		/// <summary>
		/// Called by the Doom Builder core when the user has copied geometry.
		/// </summary>
		public virtual void OnCopyEnd() { }

		/// <summary>
		/// Called by the Doom Builder core when the user wants to paste geometry into the map.
		/// Return false to abort the paste operation.
		/// The result parameter is false when the operation was already aborted by another plugin.
		/// </summary>
		public virtual bool OnPasteBegin(PasteOptions options, bool result) { return true; }

		/// <summary>
		/// Called by the Doom Builder core when the user pastes geometry into the map. The new geometry is created and marked before this method is called.
		/// </summary>
		public virtual void OnPasteEnd(PasteOptions options) { }

		/// <summary>
		/// Called by the Doom Builder core when the user wants to undo the previous action.
		/// Return false to abort the operation.
		/// The result parameter is false when the operation was already aborted by another plugin.
		/// </summary>
		public virtual bool OnUndoBegin(bool result) { return true; }

		/// <summary>
		/// Called by the Doom Builder core when the user has undone the previous action.
		/// </summary>
		public virtual void OnUndoEnd() { }

		/// <summary>
		/// Called by the Doom Builder core when the user wants to redo the previously undone action.
		/// Return false to abort the operation.
		/// The result parameter is false when the operation was already aborted by another plugin.
		/// </summary>
		public virtual bool OnRedoBegin(bool result) { return true; }

		/// <summary>
		/// Called by the Doom Builder core when the user has redone the action.
		/// </summary>
		public virtual void OnRedoEnd() { }

		/// <summary>
		/// Called by the Doom Builder core when a new undo level has been created.
		/// </summary>
		public virtual void OnUndoCreated() { }

		/// <summary>
		/// Called by the Doom Builder core when an undo level has been withdrawn.
		/// </summary>
		public virtual void OnUndoWithdrawn() { }

		/// <summary>
		/// Called when the user opens the Preferences dialog.
		/// </summary>
		public virtual void OnShowPreferences(PreferencesController controller) { }

		/// <summary>
		/// Called when the user closes the Preferences dialog by either accepting the changes or cancelling.
		/// </summary>
		public virtual void OnClosePreferences(PreferencesController controller) { }

		/// <summary>
		/// Called when an Action begins.
		/// </summary>
		public virtual void OnActionBegin(Actions.Action action) { }

		/// <summary>
		/// Called when an Action ends.
		/// </summary>
		public virtual void OnActionEnd(Actions.Action action) { }

		/// <summary>
		/// Called when an Editing Mode engages
		/// </summary>
		public virtual void OnEditEngage(EditMode oldmode, EditMode newmode) { }

		/// <summary>
		/// Called when an Editing Mode disengages
		/// </summary>
		public virtual void OnEditDisengage(EditMode oldmode, EditMode newmode) { }

		/// <summary>
		/// Called when an Editing Mode is cancelled
		/// </summary>
		public virtual void OnEditCancel() { }

		/// <summary>
		/// Called when an Editing Mode is accepted
		/// </summary>
		public virtual void OnEditAccept() { }

		/// <summary>
		/// Called just after the nodes have been (re)built. This allows a plugin to intervene with
		/// the nodebuilder output using GetLumpData and SetLumpData. If the map is being saved, this
		/// is called after the nodes are rebuilt and just before they are stored in the target file.
		/// </summary>
		public virtual void OnMapNodesRebuilt() { }

		// Interface events
		public virtual void OnEditMouseClick(MouseEventArgs e) { }
		public virtual void OnEditMouseDoubleClick(MouseEventArgs e) { }
		public virtual void OnEditMouseDown(MouseEventArgs e) { }
		public virtual void OnEditMouseEnter(EventArgs e) { }
		public virtual void OnEditMouseLeave(EventArgs e) { }
		public virtual void OnEditMouseMove(MouseEventArgs e) { }
		public virtual void OnEditMouseUp(MouseEventArgs e) { }
		public virtual void OnEditKeyDown(KeyEventArgs e) { }
		public virtual void OnEditKeyUp(KeyEventArgs e) { }
		public virtual void OnEditMouseInput(Vector2D delta) { }

		// Rendering events
		public virtual void OnEditRedrawDisplayBegin() { }
		public virtual void OnEditRedrawDisplayEnd() { }
		public virtual void OnPresentDisplayBegin() { }

		//mxd. Highlight events
		public virtual void OnHighlightSector(Sector s) { }
		public virtual void OnHighlightLinedef(Linedef l) { }
		public virtual void OnHighlightThing(Thing t) { }
		public virtual void OnHighlightVertex(Vertex v) { }
		public virtual void OnHighlightRefreshed(object o) { }
		public virtual void OnHighlightLost() { }
		
		#endregion
	}
}
