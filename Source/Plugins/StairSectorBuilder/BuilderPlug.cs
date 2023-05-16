
#region ================== Copyright (c) 2009 Boris Iwanski

/*
 * Copyright (c) 2009 Boris Iwanski
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

using System.Collections.Generic;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Plugins;
using CodeImp.DoomBuilder.Actions;

#endregion

namespace CodeImp.DoomBuilder.StairSectorBuilderMode
{
	//
	// MANDATORY: The plug!
	// This is an important class to the Doom Builder core. Every plugin must
	// have exactly 1 class that inherits from Plug. When the plugin is loaded,
	// this class is instantiated and used to receive events from the core.
	// Make sure the class is public, because only public classes can be seen
	// by the core.
	//

	internal class ToastMessages
	{
		public static readonly string ENGAGEFAILED = "engagefailed";
	}

	public class BuilderPlug : Plug
	{
        public struct Prefab
        {
            public string name;

            // General
            public int numberofsectors;
            public int innervertexmultiplier;
            public int outervertexmultiplier;
            public int stairtype;

            // Straight stairs
            public int sectordepth;
            public int spacing;
            public bool frontside;
            public bool singlesteps;
			public bool distinctsectors;
            public bool singledirection;
			public bool distinctbaseheights;

            // Auto curve
            public int flipping;

            // Catmull Rom spline
            public int numberofcontrolpoints;

            // Height info
            public bool applyfloormod;
            public int floormod;
			public int floorbase;
            public bool applyceilingmod;
            public int ceilingmod;
			public int ceilingbase;

            // Textures
            public bool applyfloortexture;
            public string floortexture;
            public bool applyceilingtexture;
            public string ceilingtexture;

            public bool applyuppertexture;
            public string uppertexture;
			public bool upperunpegged;
			public bool applymiddletexture;
			public string middletexture;
            public bool applylowertexture;
            public string lowertexture;
			public bool lowerunpegged;
        }

		// Static instance. We can't use a real static class, because BuilderPlug must
		// be instantiated by the core, so we keep a static reference. (this technique
		// should be familiar to object-oriented programmers)
		private static BuilderPlug me;

		// Static property to access the BuilderPlug
		public static BuilderPlug Me { get { return me; } }

        private List<Prefab> prefabs;

        public List<Prefab> Prefabs { get { return prefabs; } }

		// This plugin relies on some functionality that wasn't there in older versions
		public override int MinimumRevision { get { return 1310; } }

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

            prefabs = new List<Prefab>();

			// TODO: Add DB2 version check so that old DB2 versions won't crash
			// General.ErrorLogger.Add(ErrorType.Error, "zomg!");

			// Keep a static reference
            me = this;

			// Register toasts
			General.ToastManager.RegisterToast(ToastMessages.ENGAGEFAILED, "Stair Sector Builder Mode starting failed", "When no linedefs or sectors are selected when entering Stair Sector Builder Mode");
		}

		// This is called when the plugin is terminated
		public override void Dispose()
		{
			base.Dispose();

			// This must be called to remove bound methods for actions.
            General.Actions.UnbindMethods(this);
        }

        #region ================== Actions

		[BeginAction("selectsectorsoutline")]
		public void SelectSectorsOutline()
		{
			// Must have sectors selected. Having sectors selected implies
			// that there are also linedefs selected
			if(General.Map.Map.SelectedSectorsCount == 0) return;

			// Get the list of selected sectors since it'll be empty after
			// switching to linedefs mode
			List<Sector> sectors = new List<Sector>(General.Map.Map.GetSelectedSectors(true));

			General.Editing.ChangeMode("LinedefsMode");

			// Go through all selected linedefs and unselect/unmark all which
			// have a selected sector on both sides
			foreach(Linedef ld in General.Map.Map.GetSelectedLinedefs(true))
			{
				if(sectors.Contains(ld.Front.Sector) && ld.Back != null && sectors.Contains(ld.Back.Sector))
				{
					ld.Selected = false;
					ld.Marked = false;
				}
			}

			General.Editing.Mode.UpdateSelectionInfo();
			General.Interface.RedrawDisplay();
		}

        #endregion

		#region ================== Methods

		#endregion
	}
}
