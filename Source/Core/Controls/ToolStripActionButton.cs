#region ================== Copyright (c) 2022 Boris Iwanski

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
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using CodeImp.DoomBuilder.Editing;
using CodeImp.DoomBuilder.Plugins;

#endregion

namespace CodeImp.DoomBuilder.Controls
{
	[ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.ToolStrip)]
	public class ToolStripActionButton : ToolStripButton
	{
		#region ================== Variables

		private string baseToolTipText;
		private Assembly parentAssembly;

		#endregion

		#region ================== Constructors

		public ToolStripActionButton() : base()
		{
			parentAssembly = Assembly.GetCallingAssembly();
		}

		public ToolStripActionButton(string text) : base(text)
		{
			parentAssembly = Assembly.GetCallingAssembly();
		}

		public ToolStripActionButton(string text, Image image) : base(text, image)
		{
			parentAssembly = Assembly.GetCallingAssembly();
		}

		public ToolStripActionButton(string text, Image image, EventHandler onClick) : base(text, image, onClick)
		{
			parentAssembly = Assembly.GetCallingAssembly();
		}

		public ToolStripActionButton(string text, Image image, EventHandler onClick, string name) : base(text, image, onClick, name)
		{
			parentAssembly = Assembly.GetCallingAssembly();
		}

		#endregion

		#region ================== Methods

		/// <summary>
		/// Updates the tooltip of the button with its action shortcut key description. This has to be called after the button was added to the MainForm unless the tag contains the full action name (including the assembly).
		/// </summary>
		public void UpdateToolTip()
		{
			string actionName = string.Empty;

			// The shotcut key can change at runtime, so we need to remember the original tooltip without the shotcut key
			if (string.IsNullOrWhiteSpace(baseToolTipText))
			{
#if !MONO_WINFORMS
				baseToolTipText = ToolTipText;
#else
				// Mono fix. ToolTipText is empty when AutoToolTip is set to true.
				if (string.IsNullOrWhiteSpace(ToolTipText))
					baseToolTipText = Text;
				else
					baseToolTipText = ToolTipText;
#endif
			}

			// Try to figure out what's the real action name is. The action name can either be directly stored in the Tag, or
			// (in case it's a button for edit modes) can be extracted from the EditModeInfo, which is also stored in the Tag.
			if (Tag is string)
			{
				actionName = (string)Tag;

				// If it's not a known action we might be missing the plugin name as a prefix, so try to add it. This only works
				// if this method is called directly from the plugin!
				if (!General.Actions.Exists(actionName))
				{
					Plugin plugin = General.Plugins.FindPluginByAssembly(parentAssembly);

					if (plugin != null)
						actionName = plugin.Name.ToLowerInvariant() + "_" + actionName;
				}
			}
			else if (Tag is EditModeInfo modeInfo) // It's a mode button
			{
				actionName = modeInfo.SwitchAction.ActionName;

				// If it's not a known action we might be missing the plugin name as a prefix, so add it from the mode's plugin
				if (!General.Actions.Exists(actionName))
					actionName = General.Plugins.FindPluginByAssembly(modeInfo.Plugin.Assembly).Name.ToLowerInvariant() + "_" + actionName;
			}

			// Only try to add the shortcut key if the action is valid
			if (!string.IsNullOrWhiteSpace(actionName) && General.Actions.Exists(actionName))
			{
				Actions.Action action = General.Actions.GetActionByName(actionName);
				string shortcutKey = Actions.Action.GetShortcutKeyDesc(action.ShortcutKey);

				if (string.IsNullOrWhiteSpace(shortcutKey))
					ToolTipText = baseToolTipText;
				else
					ToolTipText = $"{baseToolTipText} ({shortcutKey})";
			}
			else
				ToolTipText = baseToolTipText;
		}

#endregion
	}
}
