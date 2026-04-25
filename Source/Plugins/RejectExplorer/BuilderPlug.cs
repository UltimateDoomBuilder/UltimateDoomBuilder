
#region ================== Copyright (c) 2026 Boris Iwanski

/*
 * Copyright (c) 2026 Boris Iwanski 
 *
 * This file is part of Ultimate Doom Builder.
 *
 * Ultimate Doom Builder is free software: you can redistribute it and/or
 * modify it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or (at your
 * option) any later version.
 *
 * Ultimate Doom Builder is distributed in the hope that it will be useful, but
 * WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
 * or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for
 * more details.
 *
 * You should have received a copy of the GNU General Public License along with
 * Ultimate Doom Builder. If not, see <https://www.gnu.org/licenses/>. 
 * 
 */

#endregion

#region ================== Namespaces

using CodeImp.DoomBuilder.Plugins;
using CodeImp.DoomBuilder.Rendering;


#endregion

namespace CodeImp.DoomBuilder.RejectExplorer
{
	public struct ColorSettings
	{
		public int Default;
		public int Highlight;
		public int Bidirectional;
		public int UnidirectionalFrom;
		public int UnidirectionalTo;
	}

	internal class ToastMessages
	{
		public static readonly string REJECTEXPLORER = "rejectexplorer";
	}

	public class BuilderPlug : Plug
	{
		#region ================== Variables

		// Static instance. We can't use a real static class, because BuilderPlug must
		// be instantiated by the core, so we keep a static reference. (this technique
		// should be familiar to object-oriented programmers)
		private static BuilderPlug me;

		private MenusForm menusform;

		private ColorSettings defaultColorSettings;
		private ColorSettings colorSettings;

		#endregion

		#region ================== Properties

		// This plugin relies on some functionality that wasn't there in older versions
		public override int MinimumRevision { get { return 2651; } }

		// Static property to access the BuilderPlug
		public static BuilderPlug Me { get { return me; } }

		public MenusForm MenusForm { get { return menusform; } }

		public ColorSettings ColorSettings { get { return colorSettings; } internal set { colorSettings = value; } }
		public ColorSettings DefaultColorSettings { get { return defaultColorSettings; } }

		#endregion

		#region ================== Methods

		// This event is called when the plugin is initialized
		public override void OnInitialize()
		{
			base.OnInitialize();

			General.Actions.BindMethods(this);

			// Register toasts
			General.ToastManager.RegisterToast(ToastMessages.REJECTEXPLORER, "Reject Explorer", "Toasts related to Reject Explorer mode");

			defaultColorSettings = new ColorSettings
			{
				Default = new PixelColor(255, 160, 160, 160).ToInt(), // Grey
				Highlight = new PixelColor(255, 0, 192, 0).ToInt(), // Green
				Bidirectional = new PixelColor(255, 0, 160, 0).ToInt(), // Darker green
				UnidirectionalFrom = new PixelColor(255, 160, 160, 0).ToInt(), // Yellow
				UnidirectionalTo = new PixelColor(255, 160, 0, 160).ToInt() // Purple
			};

			colorSettings = new ColorSettings
			{
				Default =General.Settings.ReadPluginSetting("colors.default", defaultColorSettings.Default),
				Highlight = General.Settings.ReadPluginSetting("colors.highlight", defaultColorSettings.Highlight),
				Bidirectional = General.Settings.ReadPluginSetting("colors.bidirectional", defaultColorSettings.Bidirectional),
				UnidirectionalFrom = General.Settings.ReadPluginSetting("colors.unidirectionalfrom", defaultColorSettings.UnidirectionalFrom),
				UnidirectionalTo = General.Settings.ReadPluginSetting("colors.unidirectionalto", defaultColorSettings.UnidirectionalTo)
			};

			menusform = new MenusForm();

			// Keep a static reference
			me = this;
		}

		// This is called when the plugin is terminated
		public override void Dispose()
		{
			base.Dispose();

			// This must be called to remove bound methods for actions.
			General.Actions.UnbindMethods(this);
		}

		#endregion
	}
}
