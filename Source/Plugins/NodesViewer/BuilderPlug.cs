
#region ================== Copyright (c) 2012 Pascal vd Heiden

/*
 * Copyright (c) 2012 Pascal vd Heiden, www.codeimp.com
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

#endregion

using System;

namespace CodeImp.DoomBuilder.Plugins.NodesViewer
{
	internal class ToastMessages
	{
		public static readonly string NODESVIEWER = "nodesviewer";
	}

	internal class ZNodesStreamException : Exception
	{
		public ZNodesStreamException(string message) : base(message) { }
		public ZNodesStreamException(string message, Exception inner) : base(message, inner) { }
	}

	public class BuilderPlug : Plug
	{
		#region ================== Variables

		// Objects
		private static BuilderPlug me;
		
		#endregion

		#region ================== Properties
		
		// Properties
		public static BuilderPlug Me { get { return me; } }
		public override string Name { get { return "NodesViewer"; } }
		public override int MinimumRevision { get { return 1545; } }
		
		#endregion

		#region ================== Initialize / Dispose

		// This event is called when the plugin is initialized
		public override void OnInitialize()
		{
			base.OnInitialize();
			
			General.Actions.BindMethods(this);

			// Keep a static reference
			me = this;

			// Register toasts
			General.ToastManager.RegisterToast(ToastMessages.NODESVIEWER, "Nodes Viewer Mode", "Toasts related to Nodes Viewer Mode");
		}

		// This is called when the plugin is terminated
		public override void Dispose()
		{
			// Clean up
			General.Actions.UnbindMethods(this);
			base.Dispose();
		}

		#endregion

		#region ================== Methods

		// This returns a unique temp filename
		/*public static string MakeTempFilename(string extension)
		{
			string filename;
			const string chars = "abcdefghijklmnopqrstuvwxyz1234567890";
			Random rnd = new Random();

			do
			{
				// Generate a filename
				filename = "";
				for(int i = 0; i < 8; i++) filename += chars[rnd.Next(chars.Length)];
				filename = Path.Combine(General.TempPath, filename + extension);
			}
			// Continue while file is not unique
			while(File.Exists(filename) || Directory.Exists(filename));

			// Return the filename
			return filename;
		}*/

		#endregion
	}
}
