
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
using System.Drawing;
using System.IO;
using CodeImp.DoomBuilder.Actions;
using CodeImp.DoomBuilder.Plugins;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.VisualModes;

#endregion

namespace CodeImp.DoomBuilder.Editing
{
	internal class EditModeInfo : IComparable<EditModeInfo>
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		// Mode type
		private Plugin plugin;
		private readonly Type type;
		private readonly EditModeAttribute attribs;
		
		// Mode switching
		private readonly BeginActionAttribute switchactionattr;
		private ActionDelegate switchactiondel;

		// Mode button
		private readonly Image buttonimage;
		private readonly string buttondesc;
		private readonly int buttonorder = int.MaxValue;

		//mxd. Disposing
		private bool isdisposed;
		
		#endregion

		#region ================== Properties

		public Plugin Plugin { get { return plugin; } }
		public Type Type { get { return type; } }
		public bool IsOptional { get { return ((switchactionattr != null) || (buttonimage != null)) && attribs.Optional; } }
		public BeginActionAttribute SwitchAction { get { return switchactionattr; } }
		public Image ButtonImage { get { return buttonimage; } }
		public string ButtonDesc { get { return buttondesc; } }
		public EditModeAttribute Attributes { get { return attribs; } }
		
		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public EditModeInfo(Plugin plugin, Type type, EditModeAttribute attr)
		{
			// Initialize
			this.plugin = plugin;
			this.type = type;
			this.attribs = attr;
			
			// Make switch action info
			if(!string.IsNullOrEmpty(attribs.SwitchAction))
				switchactionattr = new BeginActionAttribute(attribs.SwitchAction);
			
			// Make button info
			if(!string.IsNullOrEmpty(attr.ButtonImage))
			{
				using(Stream stream = plugin.GetResourceStream(attr.ButtonImage))
				{
					if(stream != null)
					{
						buttonimage = Image.FromStream(stream);
						buttondesc = attr.DisplayName + (attribs.IsDeprecated ? " (deprecated)" : "");
						buttonorder = attr.ButtonOrder;
					}
				}
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
				// Dispose
				UnbindSwitchAction();
				if(buttonimage != null) buttonimage.Dispose();

				// Clean up
				plugin = null;

				// Done
				isdisposed = true;
			}
		}
		
		#endregion

		#region ================== Methods
		
		// This binds the action to switch to this editing mode
		public void BindSwitchAction()
		{
			if((switchactiondel == null) && (switchactionattr != null))
			{
				switchactiondel = UserSwitchToMode;
				General.Actions.BindBeginDelegate(plugin.Assembly, switchactiondel, switchactionattr);
			}
		}
		
		// This unbind the switch action
		public void UnbindSwitchAction()
		{
			if(switchactiondel != null)
			{
				General.Actions.UnbindBeginDelegate(plugin.Assembly, switchactiondel, switchactionattr);
				switchactiondel = null;
			}
		}
		
		// This switches to the mode by user command (when user presses shortcut key)
		public void UserSwitchToMode()
		{
			// Only when a map is opened
			if(General.Map != null)
			{
				//mxd. Not the same mode?
				if(type != General.Editing.Mode.GetType())
				{
					// Switching from volatile mode to a different volatile mode?
					if((General.Editing.Mode != null) && General.Editing.Mode.Attributes.Volatile && this.attribs.Volatile)
					{
						// First cancel previous volatile mode
						General.Editing.CancelVolatileMode();
					}
					
					// Create instance
					EditMode newmode = plugin.CreateObject<EditMode>(type);

					//mxd. Switch mode?
					if(newmode != null) General.Editing.ChangeMode(newmode);
				}
				// When in VisualMode and switching to the same VisualMode, switch back to the previous classic mode
				else if(General.Editing.Mode is VisualMode)
				{
					// Switch back to last classic mode
					General.Editing.ChangeMode(General.Editing.PreviousClassicMode.Name);
				}
				//mxd. Switch between view floor and view ceiling textures?
				else if(General.Editing.Mode is ClassicMode && General.Settings.SwitchViewModes)
				{
					ClassicMode.SetViewMode(General.Map.Renderer2D.ViewMode == ViewMode.FloorTextures ? ViewMode.CeilingTextures : ViewMode.FloorTextures);
				}
			}
		}
		
		// This switches to the mode
		public void SwitchToMode()
		{
			// Only when a map is opened
			if(General.Map != null)
			{
				// Create instance
				EditMode newmode = plugin.CreateObject<EditMode>(type);

				//mxd. Switch mode?
				if(newmode != null) General.Editing.ChangeMode(newmode);
			}
		}

		// This switches to the mode with arguments
		public void SwitchToMode(object[] args)
		{
			// Only when a map is opened
			if(General.Map != null)
			{
				// Create instance
				EditMode newmode = plugin.CreateObjectA<EditMode>(type, args);

				// Switch mode
				if(!General.Editing.ChangeMode(newmode))
				{
					// When cancelled, dispose mode
					newmode.Dispose();
				}
			}
		}
		
		// String representation
		public override string ToString()
		{
			return attribs.DisplayName;
		}

		// Compare by button order
		public int CompareTo(EditModeInfo other)
		{
			if(this.buttonorder > other.buttonorder) return 1;
			if(this.buttonorder < other.buttonorder) return -1;
			return 0;
		}
		
		#endregion
	}
}
