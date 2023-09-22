
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
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Actions;
using CodeImp.DoomBuilder.BuilderModes.Interface;
using CodeImp.DoomBuilder.BuilderModes.IO;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Controls;
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.Editing;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Plugins;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Types;
using CodeImp.DoomBuilder.Windows;
using System.Runtime.CompilerServices;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	internal class ToastMessages
	{
		public static readonly string VISUALSLOPING = "visualsloping";
		public static readonly string CHANGEMAPELEMENTINDEX = "changemapelementindex";
	}

	public class BuilderPlug : Plug
	{
		#region ================== API Declarations

		[DllImport("user32.dll")]
		internal static extern int GetWindowLong(IntPtr hWnd, int nIndex);
		
		#endregion
		
		#region ================== Constants

		internal const int WS_HSCROLL = 0x100000;
		internal const int WS_VSCROLL = 0x200000;
		internal const int GWL_STYLE = -16;
		
		#endregion

		#region ================== Structs (mxd)

		public struct MakeDoorSettings
		{
			public readonly string DoorTexture;
			public readonly string TrackTexture;
			public readonly string CeilingTexture;
			public readonly bool ResetOffsets;
			public readonly bool ApplyActionSpecials;
			public readonly bool ApplyTag;

			public MakeDoorSettings(string doortexture, string tracktexture, string ceilingtexture, bool resetoffsets, bool applyactionspecials, bool applytag)
			{
				DoorTexture = doortexture;
				TrackTexture = tracktexture;
				CeilingTexture = ceilingtexture;
				ResetOffsets = resetoffsets;
				ApplyActionSpecials = applyactionspecials;
				ApplyTag = applytag;
			}
		}

		#endregion

		#region ================== Variables

		// Static instance
		private static BuilderPlug me;
		
		// Main objects
		private MenusForm menusform;
		private FindReplaceForm findreplaceform;
		private ErrorCheckForm errorcheckform;
		private PreferencesForm preferencesform;
		
		// Dockers
		private UndoRedoPanel undoredopanel;
		private Docker undoredodocker;
		private SectorDrawingOptionsPanel drawingOverridesPanel; //mxd
		private Docker drawingOverridesDocker; //mxd
		
		// Settings
		private int showvisualthings;			// 0 = none, 1 = sprite only, 2 = sprite caged
		private bool usegravity;
		private int changeheightbysidedef;		// 0 = nothing, 1 = change ceiling, 2 = change floor
		private bool editnewthing;
		private bool editnewsector;
		private bool additiveselect;
		private bool additivepaintselect;
		private bool autoclearselection;
		private bool visualmodeclearselection;
		private string copiedtexture;
		private string copiedflat;
		private Point copiedoffsets;
		private VertexProperties copiedvertexprops;
		private SectorProperties copiedsectorprops;
		private SidedefProperties copiedsidedefprops;
		private LinedefProperties copiedlinedefprops;
		private ThingProperties copiedthingprops;
		private bool viewselectionnumbers;
		private bool viewselectioneffects; //mxd
		private float stitchrange;
		private float highlightrange;
		private float highlightthingsrange;
		private float splitlinedefsrange;
		private float mouseselectionthreshold;
		private bool autodragonpaste;
		private bool autoAlignTextureOffsetsOnCreate;//mxd
		private bool dontMoveGeometryOutsideMapBoundary;//mxd
		private bool autoDrawOnEdit; //mxd
		private bool marqueSelectTouching; //mxd. Select elements partially/fully inside of marque selection?
		private bool syncSelection; //mxd. Sync selection between Visual and Classic modes.
		private bool lockSectorTextureOffsetsWhileDragging; //mxd
		private bool lock3DFloorSectorTextureOffsetsWhileDragging;
		private bool syncthingedit; //mxd
		private bool alphabasedtexturehighlighting; //mxd
		private bool showlightradii; //mxd
		private bool showsoundradii; //mxd
		private int scaletexturesonslopes; // 0 = base scale of 1, 1 = use current scale as base, 2 = don't scale
		private int eventlinelabelvisibility; // 0 = never show, 1 = forward only, 2 = reverse only, 3 = forward + reverse
		private int eventlinelabelstyle; // 0 = Action only, 1 = Action + short arguments, 2 = action + full arguments
		private bool eventlinedistinctcolors;
		private bool useoppositesmartpivothandle;
		private bool selectchangedafterundoredo;
		private bool selectadjacentvisualvertexslopehandles;

		#endregion

		#region ================== Properties

		public override string Name { get { return "Ultimate Doom Builder"; } } //mxd
		public static BuilderPlug Me { get { return me; } }

		//mxd. BuilderModes.dll revision should always match the main module revision
		public override bool StrictRevisionMatching { get { return true; } }
		public override int MinimumRevision { get { return Assembly.GetExecutingAssembly().GetName().Version.Revision; } }
		
		public MenusForm MenusForm { get { return menusform; } }
		public FindReplaceForm FindReplaceForm { get { return findreplaceform ?? (findreplaceform = new FindReplaceForm()); } }
		public ErrorCheckForm ErrorCheckForm { get { return errorcheckform ?? (errorcheckform = new ErrorCheckForm()); } }
		public PreferencesForm PreferencesForm { get { return preferencesform; } }

		// Settings
		public int ShowVisualThings { get { return showvisualthings; } set { showvisualthings = value; } }
		public bool UseGravity { get { return usegravity; } set { usegravity = value; } }
		public int ChangeHeightBySidedef { get { return changeheightbysidedef; } }
		public bool EditNewThing { get { return editnewthing; } }
		public bool EditNewSector { get { return editnewsector; } }
		public bool AdditiveSelect { get { return additiveselect; } }
		public bool AdditivePaintSelect { get { return additivepaintselect; } }
		public bool AutoClearSelection { get { return autoclearselection; } }
		public bool VisualModeClearSelection { get { return visualmodeclearselection; } }
		public string CopiedTexture { get { return copiedtexture; } set { copiedtexture = value; } }
		public string CopiedFlat { get { return copiedflat; } set { copiedflat = value; } }
		public Point CopiedOffsets { get { return copiedoffsets; } set { copiedoffsets = value; } }
		public VertexProperties CopiedVertexProps { get { return copiedvertexprops; } set { copiedvertexprops = value; } }
		public SectorProperties CopiedSectorProps { get { return copiedsectorprops; } set { copiedsectorprops = value; } }
		public SidedefProperties CopiedSidedefProps { get { return copiedsidedefprops; } set { copiedsidedefprops = value; } }
		public LinedefProperties CopiedLinedefProps { get { return copiedlinedefprops; } set { copiedlinedefprops = value; } }
		public ThingProperties CopiedThingProps { get { return copiedthingprops; } set { copiedthingprops = value; } }
		public bool ViewSelectionNumbers { get { return viewselectionnumbers; } set { viewselectionnumbers = value; } }
		public bool ViewSelectionEffects { get { return viewselectioneffects; } set { viewselectioneffects = value; } } //mxd
		public float StitchRange { get { return stitchrange; } internal set { stitchrange = value; } }
		public float HighlightRange { get { return highlightrange; } }
		public float HighlightThingsRange { get { return highlightthingsrange; } }
		public float SplitLinedefsRange { get { return splitlinedefsrange; } }
		public float MouseSelectionThreshold { get { return mouseselectionthreshold; } }
		public bool AutoDragOnPaste { get { return autodragonpaste; } set { autodragonpaste = value; } }
		public bool AutoDrawOnEdit { get { return autoDrawOnEdit; } set { autoDrawOnEdit = value; } } //mxd
		public bool AutoAlignTextureOffsetsOnCreate { get { return autoAlignTextureOffsetsOnCreate; } set { autoAlignTextureOffsetsOnCreate = value; } } //mxd
		public bool DontMoveGeometryOutsideMapBoundary { get { return dontMoveGeometryOutsideMapBoundary; } set { DontMoveGeometryOutsideMapBoundary = value; } } //mxd
		public bool MarqueSelectTouching { get { return marqueSelectTouching; } set { marqueSelectTouching = value; } } //mxd
		public bool SyncSelection { get { return syncSelection; } set { syncSelection = value; } } //mxd
		public bool LockSectorTextureOffsetsWhileDragging { get { return lockSectorTextureOffsetsWhileDragging; } internal set { lockSectorTextureOffsetsWhileDragging = value; } } //mxd
		public bool Lock3DFloorSectorTextureOffsetsWhileDragging { get { return lock3DFloorSectorTextureOffsetsWhileDragging; } internal set { lock3DFloorSectorTextureOffsetsWhileDragging = value; } } //mxd
		public bool SyncronizeThingEdit { get { return syncthingedit; } internal set { syncthingedit = value; } } //mxd
		public bool AlphaBasedTextureHighlighting { get { return alphabasedtexturehighlighting; } internal set { alphabasedtexturehighlighting = value; } } //mxd
		public bool ShowLightRadii { get { return showlightradii; } internal set { showlightradii = value; } } //mxd
		public bool ShowSoundRadii { get { return showsoundradii; } internal set { showsoundradii = value; } } //mxd
		public int ScaleTexturesOnSlopes { get { return scaletexturesonslopes; } internal set { scaletexturesonslopes = value; } }
		public int EventLineLabelVisibility { get { return eventlinelabelvisibility; } internal set { eventlinelabelvisibility = value; } }
		public int EventLineLabelStyle { get { return eventlinelabelstyle; } internal set { eventlinelabelstyle = value; } }
		public bool EventLineDistinctColors { get { return eventlinedistinctcolors; } internal set { eventlinedistinctcolors = value; } }
		public bool UseOppositeSmartPivotHandle { get { return useoppositesmartpivothandle; } internal set { useoppositesmartpivothandle = value; } }
		public bool SelectChangedafterUndoRedo { get { return selectchangedafterundoredo; } internal set { selectchangedafterundoredo = value; } }
		public bool SelectAdjacentVisualVertexSlopeHandles { get { return selectadjacentvisualvertexslopehandles; } internal set { selectadjacentvisualvertexslopehandles = value; } }

		//mxd. "Make Door" action persistent settings
		internal MakeDoorSettings MakeDoor;

		#endregion

		#region ================== Initialize / Dispose

		// When plugin is initialized
		public override void OnInitialize()
		{
			// Setup
			me = this;

			// Settings
			showvisualthings = 2;
			usegravity = false;
			LoadSettings();
			LoadUISettings(); //mxd
			
			// Load menus form and register it
			menusform = new MenusForm();
			menusform.Register();
			menusform.TextureOffsetLock.Checked = lockSectorTextureOffsetsWhileDragging; //mxd
			menusform.TextureOffset3DFloorLock.Checked = lock3DFloorSectorTextureOffsetsWhileDragging;
			menusform.SyncronizeThingEditButton.Checked = syncthingedit; //mxd
			menusform.SyncronizeThingEditSectorsItem.Checked = syncthingedit; //mxd
			menusform.SyncronizeThingEditLinedefsItem.Checked = syncthingedit; //mxd
			menusform.ItemLightRadii.Checked = showlightradii;
			menusform.ButtonLightRadii.Checked = showlightradii;
			menusform.ItemSoundRadii.Checked = showsoundradii;
			menusform.ButtonSoundRadii.Checked = showsoundradii;
			
			// Load Undo\Redo docker
			undoredopanel = new UndoRedoPanel();
			undoredodocker = new Docker("undoredo", "Undo / Redo", undoredopanel);
			General.Interface.AddDocker(undoredodocker);

			//mxd. Create Overrides docker
			drawingOverridesPanel = new SectorDrawingOptionsPanel();
			drawingOverridesDocker = new Docker("drawingoverrides", "Draw Settings", drawingOverridesPanel);

			//mxd
			General.Actions.BindMethods(this);

			// Register toasts
			General.ToastManager.RegisterToast(ToastMessages.VISUALSLOPING, "Visual sloping", "Toasts related to visual sloping");
			General.ToastManager.RegisterToast(ToastMessages.CHANGEMAPELEMENTINDEX, "Change map element index", "Toasts related to changing the index of map elements");
		}

		// Disposer
		public override void Dispose()
		{
			// Not already disposed?
			if(!IsDisposed)
			{
				// Clean up
				General.Interface.RemoveDocker(undoredodocker);

				undoredopanel.Dispose();
				drawingOverridesPanel.Dispose(); //mxd
				menusform.Unregister();
				menusform.Dispose();
				menusform = null;

				//mxd. These are created on demand, so they may be nulls.
				if(findreplaceform != null)
				{
					findreplaceform.Dispose();
					findreplaceform = null;
				}
				if(errorcheckform != null)
				{
					errorcheckform.Dispose();
					errorcheckform = null;
				}
				
				// Done
				me = null;
				base.Dispose();
			}
		}

		#endregion

		#region ================== Methods

		// This loads the plugin settings
		private void LoadSettings()
		{
			changeheightbysidedef = General.Settings.ReadPluginSetting("changeheightbysidedef", 0);
			editnewthing = General.Settings.ReadPluginSetting("editnewthing", true);
			editnewsector = General.Settings.ReadPluginSetting("editnewsector", false);
			additiveselect = General.Settings.ReadPluginSetting("additiveselect", false);
			additivepaintselect = General.Settings.ReadPluginSetting("additivepaintselect", additiveselect); // use the same value as additiveselect by default
			autoclearselection = General.Settings.ReadPluginSetting("autoclearselection", false);
			visualmodeclearselection = General.Settings.ReadPluginSetting("visualmodeclearselection", false);
			stitchrange = General.Settings.ReadPluginSetting("stitchrange", 20);
			highlightrange = General.Settings.ReadPluginSetting("highlightrange", 20);
			highlightthingsrange = General.Settings.ReadPluginSetting("highlightthingsrange", 10);
			splitlinedefsrange = General.Settings.ReadPluginSetting("splitlinedefsrange", 10);
			mouseselectionthreshold = General.Settings.ReadPluginSetting("mouseselectionthreshold", 2);
			autodragonpaste = General.Settings.ReadPluginSetting("autodragonpaste", false);
			autoDrawOnEdit = General.Settings.ReadPluginSetting("autodrawonedit", true); //mxd
			autoAlignTextureOffsetsOnCreate = General.Settings.ReadPluginSetting("autoaligntextureoffsetsoncreate", false); //mxd
			dontMoveGeometryOutsideMapBoundary = General.Settings.ReadPluginSetting("dontmovegeometryoutsidemapboundary", false); //mxd
			syncSelection = General.Settings.ReadPluginSetting("syncselection", false); //mxd
			scaletexturesonslopes = General.Settings.ReadPluginSetting("scaletexturesonslopes", 0);
			eventlinelabelvisibility = General.Settings.ReadPluginSetting("eventlinelabelvisibility", 3);
			eventlinelabelstyle = General.Settings.ReadPluginSetting("eventlinelabelstyle", 2);
			eventlinedistinctcolors = General.Settings.ReadPluginSetting("eventlinedistinctcolors", true);
			useoppositesmartpivothandle = General.Settings.ReadPluginSetting("useoppositesmartpivothandle", true);
			selectchangedafterundoredo = General.Settings.ReadPluginSetting("selectchangedafterundoredo", false);
		}

		//mxd. Load settings, which can be changed via UI
		private void LoadUISettings()
		{
			lockSectorTextureOffsetsWhileDragging = General.Settings.ReadPluginSetting("locktextureoffsets", false);
			lock3DFloorSectorTextureOffsetsWhileDragging = General.Settings.ReadPluginSetting("lock3dfloortextureoffsets", false);
			viewselectionnumbers = General.Settings.ReadPluginSetting("viewselectionnumbers", true);
			viewselectioneffects = General.Settings.ReadPluginSetting("viewselectioneffects", true);
			syncthingedit = General.Settings.ReadPluginSetting("syncthingedit", true);
			alphabasedtexturehighlighting = General.Settings.ReadPluginSetting("alphabasedtexturehighlighting", true);
			showlightradii = General.Settings.ReadPluginSetting("showlightradii", true);
			showsoundradii = General.Settings.ReadPluginSetting("showsoundradii", true);
		}

		//mxd. Save settings, which can be changed via UI
		private void SaveUISettings() 
		{
			General.Settings.WritePluginSetting("locktextureoffsets", lockSectorTextureOffsetsWhileDragging);
			General.Settings.WritePluginSetting("lock3dfloortextureoffsets", lock3DFloorSectorTextureOffsetsWhileDragging);
			General.Settings.WritePluginSetting("viewselectionnumbers", viewselectionnumbers);
			General.Settings.WritePluginSetting("viewselectioneffects", viewselectioneffects);
			General.Settings.WritePluginSetting("syncthingedit", syncthingedit);
			General.Settings.WritePluginSetting("alphabasedtexturehighlighting", alphabasedtexturehighlighting);
			General.Settings.WritePluginSetting("showlightradii", showlightradii);
			General.Settings.WritePluginSetting("showsoundradii", showsoundradii);
		}

		//mxd. These should be reset when changing maps
		private void ResetCopyProperties()
		{
			copiedvertexprops = null;
			copiedthingprops = null;
			copiedlinedefprops = null;
			copiedsidedefprops = null;
			copiedsectorprops = null;
		}

		#endregion

		#region ================== Events

		// When floor surface geometry is created for classic modes
		public override void OnSectorFloorSurfaceUpdate(Sector s, ref FlatVertex[] vertices)
		{
			ImageData img = General.Map.Data.GetFlatImage(s.LongFloorTexture);
			if((img != null) && img.IsImageLoaded)
			{
				//mxd. Merged from GZDoomEditing plugin
				if(General.Map.UDMF) 
				{
					// Fetch ZDoom fields
					Vector2D offset = new Vector2D(s.Fields.GetValue("xpanningfloor", 0.0),
												   s.Fields.GetValue("ypanningfloor", 0.0));
					Vector2D scale = new Vector2D(s.Fields.GetValue("xscalefloor", 1.0),
												  s.Fields.GetValue("yscalefloor", 1.0));
					double rotate = s.Fields.GetValue("rotationfloor", 0.0);
					int color, light;
					bool absolute;

					//mxd. Apply GLDEFS override?
					if(General.Map.Data.GlowingFlats.ContainsKey(s.LongFloorTexture) 
						&& General.Map.Data.GlowingFlats[s.LongFloorTexture].Fullbright)
					{
						color = -1;
						light = 255;
						absolute = true;
					}
					else
					{
                        color = PixelColor.Modulate(PixelColor.FromInt(s.Fields.GetValue("lightcolor", -1)), PixelColor.FromInt(s.Fields.GetValue("color_floor", -1))).ToInt();
						light = s.Fields.GetValue("lightfloor", 0);
						absolute = s.Fields.GetValue("lightfloorabsolute", false);
					}

					// Setup the vertices with the given settings
					SetupSurfaceVertices(vertices, s, img, offset, scale, rotate, color, light, absolute);
				} 
				else 
				{
					// Make scalars
					float sw = 1.0f / img.ScaledWidth;
					float sh = 1.0f / img.ScaledHeight;

					// Make proper texture coordinates
					for(int i = 0; i < vertices.Length; i++) 
					{
						vertices[i].u = vertices[i].u * sw;
						vertices[i].v = -vertices[i].v * sh;
					}
				}
			}
            else // [ZZ] proper fallback please.
            {
                for (int i = 0; i < vertices.Length; i++)
                {
                    vertices[i].u = vertices[i].u / 64;
                    vertices[i].v = -vertices[i].v / 64;
                }
            }
		}

		// When ceiling surface geometry is created for classic modes
		public override void OnSectorCeilingSurfaceUpdate(Sector s, ref FlatVertex[] vertices)
		{
			ImageData img = General.Map.Data.GetFlatImage(s.LongCeilTexture);
			if((img != null) && img.IsImageLoaded)
			{
				//mxd. Merged from GZDoomEditing plugin
				if(General.Map.UDMF) 
				{
					// Fetch ZDoom fields
					Vector2D offset = new Vector2D(s.Fields.GetValue("xpanningceiling", 0.0),
												   s.Fields.GetValue("ypanningceiling", 0.0));
					Vector2D scale = new Vector2D(s.Fields.GetValue("xscaleceiling", 1.0),
												  s.Fields.GetValue("yscaleceiling", 1.0));
					double rotate = s.Fields.GetValue("rotationceiling", 0.0);
					int color, light;
					bool absolute;

					//mxd. Apply GLDEFS override?
					if(General.Map.Data.GlowingFlats.ContainsKey(s.LongCeilTexture)
						&& General.Map.Data.GlowingFlats[s.LongCeilTexture].Fullbright)
					{
						color = -1;
						light = 255;
						absolute = true;
					} 
					else 
					{
                        color = PixelColor.Modulate(PixelColor.FromInt(s.Fields.GetValue("lightcolor", -1)), PixelColor.FromInt(s.Fields.GetValue("color_ceiling", -1))).ToInt();
                        light = s.Fields.GetValue("lightceiling", 0);
						absolute = s.Fields.GetValue("lightceilingabsolute", false);
					}

					// Setup the vertices with the given settings
					SetupSurfaceVertices(vertices, s, img, offset, scale, rotate, color, light, absolute);
				} 
				else 
				{
					// Make scalars
					float sw = 1.0f / img.ScaledWidth;
					float sh = 1.0f / img.ScaledHeight;

					// Make proper texture coordinates
					for(int i = 0; i < vertices.Length; i++) 
					{
						vertices[i].u = vertices[i].u * sw;
						vertices[i].v = -vertices[i].v * sh;
					}
				}
			}
            else // [ZZ] proper fallback please.
            {
                for (int i = 0; i < vertices.Length; i++)
                {
                    vertices[i].u = vertices[i].u / 64;
                    vertices[i].v = -vertices[i].v / 64;
                }
            }
        }

		// When the editing mode changes
		public override bool OnModeChange(EditMode oldmode, EditMode newmode)
		{
			// Show the correct menu for the new mode
			menusform.ShowEditingModeMenu(newmode);
			
			return base.OnModeChange(oldmode, newmode);
		}

		// When the Preferences dialog is shown
		public override void OnShowPreferences(PreferencesController controller)
		{
			base.OnShowPreferences(controller);

			// Load preferences
			preferencesform = new PreferencesForm();
			preferencesform.Setup(controller);
		}

		// When the Preferences dialog is closed
		public override void OnClosePreferences(PreferencesController controller)
		{
			base.OnClosePreferences(controller);

			// Apply settings that could have been changed
			LoadSettings();
			
			// Unload preferences
			preferencesform.Dispose();
			preferencesform = null;
		}
		
		// New map created
		public override void OnMapNewEnd()
		{
			base.OnMapNewEnd();
			undoredopanel.SetBeginDescription("New Map");
			undoredopanel.UpdateList();

			//mxd
			General.Interface.AddDocker(drawingOverridesDocker);
			drawingOverridesPanel.Setup();
			MakeDoor = new MakeDoorSettings(General.Map.Config.MakeDoorDoor, General.Map.Config.MakeDoorTrack, General.Map.Config.MakeDoorCeiling, MakeDoor.ResetOffsets, MakeDoor.ApplyActionSpecials, MakeDoor.ApplyTag);
			ResetCopyProperties();
		}
		
		// Map opened
		public override void OnMapOpenEnd()
		{
			base.OnMapOpenEnd();
			undoredopanel.SetBeginDescription("Opened Map");
			undoredopanel.UpdateList();

			//mxd
			General.Interface.AddDocker(drawingOverridesDocker);
			drawingOverridesPanel.Setup();
			General.Map.Renderer2D.UpdateExtraFloorFlag();
			MakeDoor = new MakeDoorSettings(General.Map.Config.MakeDoorDoor, General.Map.Config.MakeDoorTrack, General.Map.Config.MakeDoorCeiling, MakeDoor.ResetOffsets, MakeDoor.ApplyActionSpecials, MakeDoor.ApplyTag);
			ResetCopyProperties();
		}

		//mxd
		public override void OnMapCloseBegin()
		{
			drawingOverridesPanel.Terminate();
			General.Interface.RemoveDocker(drawingOverridesDocker);
		}

        // Map closed
        public override void OnMapCloseEnd()
		{
			base.OnMapCloseEnd();
			undoredopanel.UpdateList();
			errorcheckform = null; //mxd. Error checks may need to be reinitialized

			//mxd. Save settings
			SaveUISettings();
		}

		//mxd. Error checks may need to be reinitialized
		public override void OnMapReconfigure()
		{
			errorcheckform = null;
		}
		
		// Redo performed
		public override void OnRedoEnd()
		{
			base.OnRedoEnd();
			undoredopanel.UpdateList();
		}
		
		// Undo performed
		public override void OnUndoEnd()
		{
			base.OnUndoEnd();
			undoredopanel.UpdateList();
		}
		
		// Undo created
		public override void OnUndoCreated()
		{
			base.OnUndoCreated();
			undoredopanel.UpdateList();
		}
		
		// Undo withdrawn
		public override void OnUndoWithdrawn()
		{
			base.OnUndoWithdrawn();
			undoredopanel.UpdateList();
		}
		
		#endregion
		
		#region ================== Tools

		//mxd. merged from GZDoomEditing plugin
		// This applies the given values on the vertices
		private static void SetupSurfaceVertices(FlatVertex[] vertices, Sector s, ImageData img, Vector2D offset,
										  Vector2D scale, double rotate, int color, int light, bool absolute) 
		{
			// Prepare for math!
			rotate = Angle2D.DegToRad(rotate);
			Vector2D texscale = new Vector2D(1.0f / img.ScaledWidth, 1.0f / img.ScaledHeight);
			if(!absolute) light = s.Brightness + light;
			PixelColor lightcolor = PixelColor.FromInt(color);
			PixelColor brightness = PixelColor.FromInt(General.Map.Renderer2D.CalculateBrightness(light));
			PixelColor finalcolor = PixelColor.Modulate(lightcolor, brightness);
            color = finalcolor.WithAlpha(255).ToInt();

			// Do the math for all vertices
			for(int i = 0; i < vertices.Length; i++) 
			{
				Vector2D pos = new Vector2D(vertices[i].x, vertices[i].y);
				pos = pos.GetRotated(rotate);
				pos.y = -pos.y;
				pos = (pos + offset) * scale * texscale;
				vertices[i].u = (float)pos.x;
				vertices[i].v = (float)pos.y;
				vertices[i].c = color;
			}
		}
		
		// This finds all class types that inherits from the given type
		public Type[] FindClasses(Type t)
		{
			List<Type> found = new List<Type>();

			// Get all exported types
			Type[] types = Assembly.GetExecutingAssembly().GetTypes();
			foreach(Type it in types)
			{
				// Compare types
				if(t.IsAssignableFrom(it)) found.Add(it);
			}

			// Return list
			return found.ToArray();
		}

		#endregion

		#region ================== Actions (mxd)

		[BeginAction("exporttoobj")]
		private void ExportToObj() 
		{
			// Convert geometry selection to sectors
			General.Map.Map.ConvertSelection(SelectionType.Sectors);
			
			//get sectors
			ICollection<Sector> sectors = General.Map.Map.SelectedSectorsCount == 0 ? General.Map.Map.Sectors : General.Map.Map.GetSelectedSectors(true);
			if(sectors.Count == 0) 
			{
				General.Interface.DisplayStatus(StatusType.Warning, "OBJ export failed. Map has no sectors!");
				return;
			}

			//show settings form
			WavefrontSettingsForm form = new WavefrontSettingsForm(General.Map.Map.SelectedSectorsCount == 0 ? -1 : sectors.Count);
			if(form.ShowDialog() == DialogResult.OK) 
			{
				WavefrontExportSettings data = new WavefrontExportSettings(form);
				WavefrontExporter e = new WavefrontExporter();
				e.Export(sectors, data);
			}
		}

		[BeginAction("exporttoimage")]
		private void ExportToImage()
		{
			// Get sectors
			ICollection<Sector> sectors = General.Map.Map.SelectedSectorsCount == 0 ? General.Map.Map.Sectors : General.Map.Map.GetSelectedSectors(true);
			if (sectors.Count == 0)
			{
				General.Interface.DisplayStatus(StatusType.Warning, "Image export failed. Map has no sectors!");
				return;
			}

			ImageExportSettingsForm form = new ImageExportSettingsForm();
			form.ShowDialog();
		}

		#endregion
	}
}
