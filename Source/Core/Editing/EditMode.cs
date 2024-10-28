
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
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Forms;
using System.Reflection;
using System.Diagnostics;
using CodeImp.DoomBuilder.Actions;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Windows;

#endregion

namespace CodeImp.DoomBuilder.Editing
{
	/// <summary>
	/// Provides basic user input interface functionality for a Doom Builder editing mode.
	/// </summary>
	public abstract class EditMode
	{
		#region ================== Constants

		public const int DRAG_START_MOVE_PIXELS = 5;

		#endregion

		#region ================== Variables
		
		// Attributes
		protected EditModeAttribute attributes; //mxd. private -> protected
		
		// Disposing
		protected bool isdisposed;

		#endregion

		#region ================== Properties

		public bool IsDisposed { get { return isdisposed; } }

		public EditModeAttribute Attributes { get { return attributes; } }
		
		// Unless overriden, this returns the name of this mode
		// for checking the appropriate button on the toolbar.
		public virtual string EditModeButtonName { get { return GetType().Name; } }

		// Override this to provide a highlighted object, if applicable
		public virtual object HighlightedObject { get { return null; } }

		#endregion

		#region ================== Constructor / Disposer

		/// <summary>
		/// Provides basic user input interface functionality for a Doom Builder editing mode.
		/// </summary>
		protected EditMode()
		{
			// Fetch attributes
			object[] attrs = this.GetType().GetCustomAttributes(true);
			foreach(object a in attrs)
			{
				EditModeAttribute attribute = a as EditModeAttribute;
				if(attribute != null)
				{
					attributes = attribute;
					break;
				}
			}

			// No attributes found?
			if(attributes == null) throw new Exception("Editing mode \"" + this.GetType().Name + "\" is missing EditMode attributes!");
			
			// We have no destructor
			GC.SuppressFinalize(this);
		}

		// Disposer
		public virtual void Dispose()
		{
			// Not already disposed?
			if(!isdisposed)
			{
				// Clean up

				// Done
				isdisposed = true;
			}
		}

		#endregion

		#region ================== Static Methods

		// This creates an instance of a specific mode
		public static EditMode Create(Type modetype, object[] args)
		{
			try
			{
				// Create new mode
				return (EditMode)General.ThisAssembly.CreateInstance(modetype.FullName, false,
					BindingFlags.Default, null, args, CultureInfo.CurrentCulture, new object[0]);
			}
			// Catch errors
			catch(TargetInvocationException e)
			{
				// Throw the actual exception
				Debug.WriteLine(DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString());
				Debug.WriteLine(e.InnerException.Source + " throws " + e.InnerException.GetType().Name + ":");
				Debug.WriteLine(e.InnerException.Message);
				Debug.WriteLine(e.InnerException.StackTrace);
				throw e.InnerException;
			}
		}
		
		#endregion

		#region ================== Methods

		//mxd
		public virtual void UpdateSelectionInfo()
		{
			// Collect info
			List<string> info = new List<string>();

			if(General.Map.Map.SelectedSectorsCount > 0)
				info.Add(General.Map.Map.SelectedSectorsCount + (General.Map.Map.SelectedSectorsCount == 1 ? " sector" : " sectors"));

			if(General.Map.Map.SelectedLinedefsCount > 0)
				info.Add(General.Map.Map.SelectedLinedefsCount + (General.Map.Map.SelectedLinedefsCount == 1 ? " linedef" : " linedefs"));

			if(General.Map.Map.SelectedVerticessCount > 0)
				info.Add(General.Map.Map.SelectedVerticessCount + (General.Map.Map.SelectedVerticessCount == 1 ? " vertex" : " vertices"));

			if(General.Map.Map.SelectedThingsCount > 0)
				info.Add(General.Map.Map.SelectedThingsCount + (General.Map.Map.SelectedThingsCount == 1 ? " thing" : " things"));

			// Display results
			string result = string.Empty;
			if(info.Count > 0)
			{
				result = string.Join(", ", info.ToArray());
				int pos = result.LastIndexOf(",", StringComparison.Ordinal);
				if(pos != -1) result = result.Remove(pos, 1).Insert(pos, " and");
				result += " selected.";
			}

			General.Interface.DisplayStatus(StatusType.Selection, result);
		}

		#endregion

		#region ================== Events

		//
		// Order in which events occur for the old and new modes:
		// 
		// - Constructor of new mode is called
		// - Disengage of old mode is called
		// ----- Mode switches -----
		// - Engage of new mode is called
		// - Dispose of old mode is called
		//
		
		// Mode engages
		public virtual void OnEngage()
		{
			// Bind any methods
			General.Actions.BindMethods(this);

			//mxd. Show hints for this mode
			General.Hints.ShowHints(this.GetType(), HintsManager.GENERAL);

			//mxd. Display new mode name
			General.Interface.HideInfo();
		}

		// Mode disengages
		public virtual void OnDisengage()
		{
			// Unbind any methods
			General.Actions.UnbindMethods(this);
		}

		// Called when the user presses F1 for Help
		public virtual void OnHelp() { }

		// This forces the mode to cancel and return to the "parent" mode
		public virtual void OnCancel() { }
		public virtual void OnAccept() { }

		// Called before copying. Return false when copying should be cancelled.
		// The edit mode should mark all vertices, lines and sectors
		// that need to be copied.
		public virtual bool OnCopyBegin() { return false; }

		// Called when the marked geometry has been copied.
		public virtual void OnCopyEnd() { }
		
		// Called before pasting. Override this and return true to indicate that paste is allowed to contiue.
		public virtual bool OnPasteBegin(PasteOptions options) { return false; }

		// Called after new geometry has been pasted in. The new geometry is marked.
		public virtual void OnPasteEnd(PasteOptions options) { }
		
		// Called when undo/redo is used
		// Return false to cancel undo action
		public virtual bool OnUndoBegin() { return true; }
		public virtual bool OnRedoBegin() { return true; }
		public virtual void OnUndoEnd() { General.Map.Renderer2D.UpdateExtraFloorFlag(); } //mxd
		public virtual void OnRedoEnd() { General.Map.Renderer2D.UpdateExtraFloorFlag(); } //mxd
		
		// Interface events
		public virtual void OnMouseClick(MouseEventArgs e) { }
		public virtual void OnMouseDoubleClick(MouseEventArgs e) { }
		public virtual void OnMouseDown(MouseEventArgs e) { }
		public virtual void OnMouseEnter(EventArgs e) { }
		public virtual void OnMouseLeave(EventArgs e) { }
		public virtual void OnMouseMove(MouseEventArgs e) { }
		public virtual void OnMouseUp(MouseEventArgs e) { }
		public virtual void OnKeyDown(KeyEventArgs e) { }
		public virtual void OnKeyUp(KeyEventArgs e) { }
		public virtual void OnMouseInput(Vector2D delta) { }
		
		// Rendering events
		public virtual void OnRedrawDisplay() { }
		public virtual void OnPresentDisplay() { }

		// Processing events
		public virtual void OnProcess(long deltatime) { }
		public virtual void OnClockReset() { } //mxd
		
		// Generic events
		public virtual void OnReloadResources() { }
		public virtual void OnMapSetChangeBegin() { }
		public virtual void OnMapSetChangeEnd() { }

		//mxd. map testing events
		public virtual bool OnMapTestBegin(bool testFromCurrentPosition) { return true; } //called before test map is launched. Returns false if map launch is impossible
		public virtual void OnMapTestEnd(bool testFromCurrentPosition) { } //called after game engine is closed

		// Script events
		public virtual bool OnScriptRunBegin() { return true; }
		public virtual void OnScriptRunEnd() { }

		// This should be called by global actions (i.e. that are not part of an editing mode) when they changed map elements,
		// so that the mode can react to it (for example by rebuilding a blockmap)
		public virtual void OnMapElementsChanged() { }

		public virtual bool OnAutoSaveBegin() { return attributes.Volatile ? false : true; } // Called before autosave is done. Returns false if autosave should not be done. By default volatile modes prevent autosave
		public virtual void OnAutoSaveEnd() { }
		
		#endregion
	}
}
