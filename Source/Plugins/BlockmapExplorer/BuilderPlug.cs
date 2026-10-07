


#region ================== Copyright (c) 2016 Boris Iwanski

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

#endregion

namespace CodeImp.DoomBuilder.BlockmapExplorer
{
	internal class ToastMessages
	{
		public static readonly string BLOCKMAPEXPLORER = "blockmapexplorer";
	}

	public class BuilderPlug : Plug
	{
		#region ================== Variables

		private float highlightrange;

		// Static instance. We can't use a real static class, because BuilderPlug must
		// be instantiated by the core, so we keep a static reference. (this technique
		// should be familiar to object-oriented programmers)
		private static BuilderPlug me;

		#endregion

		#region ================== Properties

		public float HighlightRange { get { return highlightrange; } }
		public string SecretFlag { get { return General.Map.UDMF ? "secret" : "32"; } }
		public string HiddenFlag { get { return General.Map.UDMF ? "dontdraw" : "128"; } }

		// This plugin relies on some functionality that wasn't there in older versions
		public override int MinimumRevision { get { return 2651; } }

		// Static property to access the BuilderPlug
		public static BuilderPlug Me { get { return me; } }

		#endregion

		#region ================== Methods

		// This event is called when the plugin is initialized
		public override void OnInitialize()
		{
			base.OnInitialize();

			// This binds the methods in this class that have the BeginAction
			// and EndAction attributes with their actions. Without this, the
			// attributes are useless. Note that in classes derived from EditMode
			// this is not needed, because they are bound automatically when the
			// editing mode is engaged.
			General.Actions.BindMethods(this);

			// TODO: Add DB2 version check so that old DB2 versions won't crash
			// General.ErrorLogger.Add(ErrorType.Error, "zomg!");

			// Keep a static reference
			me = this;

			// Register toasts
			General.ToastManager.RegisterToast(ToastMessages.BLOCKMAPEXPLORER, "Blockmap Explorer", "Toasts related to Blockmap Explorer mode");

			LoadSettings();
		}

		//mxd
		public override void OnMapOpenEnd()
		{
			BlockmapExplorerMode mode = General.Editing.Mode as BlockmapExplorerMode;
			if(mode != null)
			{
				//mode.UpdateValidLinedefs();
				//mode.UpdateSecretSectors();
			}
			
			base.OnMapOpenEnd();
		}

		//mxd
		public override void OnMapNewEnd()
		{
			BlockmapExplorerMode mode = General.Editing.Mode as BlockmapExplorerMode;
			if(mode != null)
			{
				//mode.UpdateValidLinedefs();
				//mode.UpdateSecretSectors();
			}
			
			base.OnMapNewEnd();
		}

		// This is called when the plugin is terminated
		public override void Dispose()
		{
			base.Dispose();

			// This must be called to remove bound methods for actions.
			General.Actions.UnbindMethods(this);
		}

		private void LoadSettings()
		{
			highlightrange = General.Settings.ReadPluginSetting("buildermodes", "highlightrange", 20);
		}

		#endregion
	}
}
