
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
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using CodeImp.DoomBuilder.Actions;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Controls;
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.Editing;
using CodeImp.DoomBuilder.GZBuilder.Data;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.IO;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Plugins;
using CodeImp.DoomBuilder.Properties;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.VisualModes;

#endregion

namespace CodeImp.DoomBuilder.Windows
{
	public partial class MainForm : DelayedForm, IMainForm
	{
		#region ================== Constants
		
		// Recent files
		private const int MAX_RECENT_FILES_PIXELS = 250;
		
		// Status bar
		internal const int WARNING_FLASH_COUNT = 10;
		internal const int WARNING_FLASH_INTERVAL = 100;
		internal const int WARNING_RESET_DELAY = 5000;
		internal const int INFO_RESET_DELAY = 5000;
		internal const int ACTION_FLASH_COUNT = 3;
		internal const int ACTION_FLASH_INTERVAL = 50;
		internal const int ACTION_RESET_DELAY = 5000;
		
		internal readonly Image[,] STATUS_IMAGES = new Image[,]
		{
			// Normal versions
			{
			  Resources.Status0, Resources.Status1,
			  Resources.Status2, Resources.Warning
			},
			
			// Flashing versions
			{
			  Resources.Status10, Resources.Status11,
			  Resources.Status12, Resources.WarningOff
			}
		};
		
		#endregion 

		#region ================== Delegates

		//private delegate void CallUpdateStatusIcon();
		//private delegate void CallImageDataLoaded(ImageData img);
		private delegate void CallBlink(); //mxd

		#endregion

		#region ================== mxd. Events

		public event EventHandler OnEditFormValuesChanged; //mxd

		#endregion

		#region ================== Variables

		// Position/size
		private bool displayresized = true;
		private bool windowactive;
		
		// Mouse in display
		private bool mouseinside;
		
		// Input
		private bool shift, ctrl, alt;
		private MouseButtons mousebuttons;
		private MouseInput mouseinput;
		private bool mouseexclusive;
		private int mouseexclusivebreaklevel;
		
		// Last info on panels
		private object lastinfoobject;
		
		// Recent files
		private ToolStripMenuItem[] recentitems;
		
		// View modes
		private ToolStripButton[] viewmodesbuttons;
		private ToolStripMenuItem[] viewmodesitems;

		//mxd. Geometry merge modes
		private ToolStripButton[] geomergemodesbuttons;
		private ToolStripMenuItem[] geomergemodesitems;
		
		// Edit modes
		private List<ToolStripItem> editmodeitems;
		
		// Toolbar
		private List<PluginToolbarButton> pluginbuttons;
		private EventHandler buttonvisiblechangedhandler;
		private bool preventupdateseperators;
		private bool updatingfilters;
		private bool toolbarContextMenuShiftPressed; //mxd
		
		// Statusbar
		private StatusInfo status;
		private int statusflashcount;
		private bool statusflashicon;
		
		// Properties
		private IntPtr windowptr;
		
		// Processing
		private int processingcount;
		private long lastupdatetime;

		// Updating
		private int lockupdatecount;
		private bool mapchanged; //mxd

		//mxd. Hints
		private Docker hintsDocker;
		private HintsPanel hintsPanel;

		//mxd
		private System.Timers.Timer blinkTimer; 
		private bool editformopen;

		//mxd. Misc drawing
		private Graphics graphics;
		
		#endregion

		#region ================== Properties

		public bool ShiftState { get { return shift; } }
		public bool CtrlState { get { return ctrl; } }
		public bool AltState { get { return alt; } }
		new public MouseButtons MouseButtons { get { return mousebuttons; } }
		public bool MouseInDisplay { get { return mouseinside; } }
		public RenderTargetControl Display { get { return display; } }
		public bool SnapToGrid { get { return buttonsnaptogrid.Checked; } }
		public bool AutoMerge { get { return buttonautomerge.Checked; } }
		public bool MouseExclusive { get { return mouseexclusive; } }
		new public IntPtr Handle { get { return windowptr; } }
		public bool IsInfoPanelExpanded { get { return (panelinfo.Height == heightpanel1.Height); } }
		public string ActiveDockerTabName { get { return dockerspanel.IsCollpased ? "None" : dockerspanel.SelectedTabName; } }
		public bool IsActiveWindow { get { return windowactive; } }
		public StatusInfo Status { get { return status; } }
		public static Size ScaledIconSize = new Size(16, 16); //mxd
		public static SizeF DPIScaler = new SizeF(1.0f, 1.0f); //mxd
		public int ProcessingCount { get { return processingcount; } }
		
		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		internal MainForm()
		{
			// Fetch pointer
			windowptr = base.Handle;
			
			//mxd. Graphics
			graphics = Graphics.FromHwndInternal(windowptr);
			
			//mxd. Set DPI-aware icon size
			DPIScaler = new SizeF(graphics.DpiX / 96, graphics.DpiY / 96);

			if(DPIScaler.Width != 1.0f || DPIScaler.Height != 1.0f)
			{
				ScaledIconSize.Width = (int)Math.Round(ScaledIconSize.Width * DPIScaler.Width);
				ScaledIconSize.Height = (int)Math.Round(ScaledIconSize.Height * DPIScaler.Height);
			}
			
			// Setup controls
			InitializeComponent();

			//mxd. Resize status labels
			if(DPIScaler.Width != 1.0f)
			{
				gridlabel.Width = (int)Math.Round(gridlabel.Width * DPIScaler.Width);
				zoomlabel.Width = (int)Math.Round(zoomlabel.Width * DPIScaler.Width);
				xposlabel.Width = (int)Math.Round(xposlabel.Width * DPIScaler.Width);
				yposlabel.Width = (int)Math.Round(yposlabel.Width * DPIScaler.Width);
				warnsLabel.Width = (int)Math.Round(warnsLabel.Width * DPIScaler.Width);
			}

			pluginbuttons = new List<PluginToolbarButton>();
			editmodeitems = new List<ToolStripItem>();
			labelcollapsedinfo.Text = "";
			display.Dock = DockStyle.Fill;
			
			// Make array for view modes
			viewmodesbuttons = new ToolStripButton[Renderer2D.NUM_VIEW_MODES];
			viewmodesbuttons[(int)ViewMode.Normal] = buttonviewnormal;
			viewmodesbuttons[(int)ViewMode.Brightness] = buttonviewbrightness;
			viewmodesbuttons[(int)ViewMode.FloorTextures] = buttonviewfloors;
			viewmodesbuttons[(int)ViewMode.CeilingTextures] = buttonviewceilings;
			viewmodesitems = new ToolStripMenuItem[Renderer2D.NUM_VIEW_MODES];
			viewmodesitems[(int)ViewMode.Normal] = itemviewnormal;
			viewmodesitems[(int)ViewMode.Brightness] = itemviewbrightness;
			viewmodesitems[(int)ViewMode.FloorTextures] = itemviewfloors;
			viewmodesitems[(int)ViewMode.CeilingTextures] = itemviewceilings;

			//mxd. Make arrays for geometry merge modes
			int numgeomodes = Enum.GetValues(typeof(MergeGeometryMode)).Length;
			geomergemodesbuttons = new ToolStripButton[numgeomodes];
			geomergemodesbuttons[(int)MergeGeometryMode.CLASSIC] = buttonmergegeoclassic;
			geomergemodesbuttons[(int)MergeGeometryMode.MERGE] = buttonmergegeo;
			geomergemodesbuttons[(int)MergeGeometryMode.REPLACE] = buttonplacegeo;
			geomergemodesitems = new ToolStripMenuItem[numgeomodes];
			geomergemodesitems[(int)MergeGeometryMode.CLASSIC] = itemmergegeoclassic;
			geomergemodesitems[(int)MergeGeometryMode.MERGE] = itemmergegeo;
			geomergemodesitems[(int)MergeGeometryMode.REPLACE] = itemreplacegeo;
			
			// Visual Studio IDE doesn't let me set these in the designer :(
			buttonzoom.Font = menufile.Font;
			buttonzoom.DropDownDirection = ToolStripDropDownDirection.AboveLeft;
			buttongrid.Font = menufile.Font;
			buttongrid.DropDownDirection = ToolStripDropDownDirection.AboveLeft;

			// Event handlers
			buttonvisiblechangedhandler = ToolbarButtonVisibleChanged;
			//mxd
			display.OnKeyReleased += display_OnKeyReleased;
			toolbarContextMenu.KeyDown += toolbarContextMenu_KeyDown;
			toolbarContextMenu.KeyUp += toolbarContextMenu_KeyUp;
			linedefcolorpresets.DropDown.MouseLeave += linedefcolorpresets_MouseLeave;
			this.MouseCaptureChanged += MainForm_MouseCaptureChanged;
			
			// Apply shortcut keys
			ApplyShortcutKeys();
			
			// Make recent items list
			CreateRecentFiles();
			
			// Show splash
			ShowSplashDisplay();

			//mxd
			blinkTimer = new System.Timers.Timer {Interval = 500};
			blinkTimer.Elapsed += blinkTimer_Elapsed;

			//mxd. Debug Console
#if DEBUG
			modename.Visible = false;
#else
			console.Visible = false;
#endif

			//mxd. Hints
			hintsPanel = new HintsPanel();
			hintsDocker = new Docker("hints", "Help", hintsPanel);

			KeyPreview = true;
			PreviewKeyDown += new PreviewKeyDownEventHandler(MainForm_PreviewKeyDown);
		}
		
		#endregion
		
		#region ================== General

		// Editing mode changed!
		internal void EditModeChanged()
		{
			// Check appropriate button on interface
			// And show the mode name
			if(General.Editing.Mode != null)
			{
				General.MainWindow.CheckEditModeButton(General.Editing.Mode.EditModeButtonName);
				General.MainWindow.DisplayModeName(General.Editing.Mode.Attributes.DisplayName + (General.Editing.Mode.Attributes.IsDeprecated ? " (deprecated)" : ""));
			}
			else
			{
				General.MainWindow.CheckEditModeButton("");
				General.MainWindow.DisplayModeName("");
			}

			// View mode only matters in classic editing modes
			bool isclassicmode = (General.Editing.Mode is ClassicMode);
			for(int i = 0; i < Renderer2D.NUM_VIEW_MODES; i++)
			{
				viewmodesitems[i].Enabled = isclassicmode;
				viewmodesbuttons[i].Enabled = isclassicmode;
			}

			//mxd. Merge geometry mode only matters in classic editing modes
			for(int i = 0; i < geomergemodesbuttons.Length; i++)
			{
				geomergemodesbuttons[i].Enabled = isclassicmode;
				geomergemodesitems[i].Enabled = isclassicmode;
			}

			UpdateEditMenu();
			UpdatePrefabsMenu();
		}

		// This makes a beep sound
		public void MessageBeep(MessageBeepType type)
		{
			General.MessageBeep(type);
		}

		// This sets up the interface
		internal void SetupInterface()
		{
			// Setup docker
			if(General.Settings.DockersPosition != 2 && General.Map != null)
			{
				LockUpdate();
				dockerspanel.Visible = true;
				dockersspace.Visible = true;

				// We can't place the docker easily when collapsed
				dockerspanel.Expand();

				// Setup docker width
				if(General.Settings.DockersWidth < dockerspanel.GetCollapsedWidth())
					General.Settings.DockersWidth = dockerspanel.GetCollapsedWidth();

				// Determine fixed space required
				if(General.Settings.CollapseDockers)
					dockersspace.Width = dockerspanel.GetCollapsedWidth();
				else
					dockersspace.Width = General.Settings.DockersWidth;

				// Setup docker
				int targetindex = this.Controls.IndexOf(display) + 1; //mxd
				if(General.Settings.DockersPosition == 0)
				{
					modestoolbar.Dock = DockStyle.Right; //mxd
					dockersspace.Dock = DockStyle.Left;
					AdjustDockersSpace(targetindex); //mxd
					dockerspanel.Setup(false);
					dockerspanel.Location = dockersspace.Location;
					dockerspanel.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Bottom;
				}
				else
				{
					modestoolbar.Dock = DockStyle.Left; //mxd
					dockersspace.Dock = DockStyle.Right;
					AdjustDockersSpace(targetindex); //mxd
					dockerspanel.Setup(true);
					dockerspanel.Location = new Point(dockersspace.Right - General.Settings.DockersWidth, dockersspace.Top);
					dockerspanel.Anchor = AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
				}

				dockerspanel.Width = General.Settings.DockersWidth;
				dockerspanel.Height = dockersspace.Height;
				dockerspanel.BringToFront();

				if(General.Settings.CollapseDockers) dockerspanel.Collapse();

				UnlockUpdate();
			}
			else
			{
				dockerspanel.Visible = false;
				dockersspace.Visible = false;
				modestoolbar.Dock = DockStyle.Left; //mxd
			}
		}

		//mxd. dockersspace display index gets messed up while re-docking. This fixes it...
		private void AdjustDockersSpace(int targetindex)
		{
			while(this.Controls.IndexOf(dockersspace) != targetindex)
			{
				this.Controls.SetChildIndex(dockersspace, targetindex);
			}
		}
		
		// This updates all menus for the current status
		internal void UpdateInterface()
		{
			//mxd. Update title
			UpdateTitle();

			// Update the status bar
			UpdateStatusbar();
			
			// Update menus and toolbar icons
			UpdateFileMenu();
			UpdateEditMenu();
			UpdateViewMenu();
			UpdateModeMenu();
			UpdatePrefabsMenu();
			UpdateToolsMenu();
			UpdateToolbar();
			UpdateSkills();
			UpdateHelpMenu();
		}

		//mxd
		private void UpdateTitle()
		{
			string programname = this.Text = Application.ProductName + " R" + General.ThisAssembly.GetName().Version.Revision;
            if (Environment.Is64BitProcess)
                programname += " (64-bit)";
            else programname += " (32-bit)";

			// Map opened?
			if (General.Map != null)
			{
				// Get nice name
				string maptitle = (!string.IsNullOrEmpty(General.Map.Data.MapInfo.Title) ? ": " + General.Map.Data.MapInfo.Title : "");
				
				// Show map name and filename in caption
				this.Text = (mapchanged ? "\u25CF " : "") + General.Map.FileTitle + " (" + General.Map.Options.CurrentName + maptitle + ") - " + programname;
			}
			else
			{
				// Show normal caption
				this.Text = programname;
			}
		}
		
		// Generic event that invokes the tagged action
		public void InvokeTaggedAction(object sender, EventArgs e)
		{
			this.Update();
			
			if(sender is ToolStripItem)
				General.Actions.InvokeAction(((ToolStripItem)sender).Tag.ToString());
			else if(sender is Control)
				General.Actions.InvokeAction(((Control)sender).Tag.ToString());
			else
				General.Fail("InvokeTaggedAction used on an unexpected control.");
			
			this.Update();
		}
		
		#endregion
		
		#region ================== Window
		
		// This locks the window for updating
		internal void LockUpdate()
		{
			lockupdatecount++;
			if(lockupdatecount == 1) General.LockWindowUpdate(this.Handle);
		}

		// This unlocks for updating
		internal void UnlockUpdate()
		{
			lockupdatecount--;
			if(lockupdatecount == 0) General.LockWindowUpdate(IntPtr.Zero);
			if(lockupdatecount < 0) lockupdatecount = 0;
		}

		// This unlocks for updating
		/*internal void ForceUnlockUpdate()
		{
			if(lockupdatecount > 0) General.LockWindowUpdate(IntPtr.Zero);
			lockupdatecount = 0;
		}*/

		//mxd
		internal void UpdateMapChangedStatus()
		{
			if(General.Map == null || General.Map.IsChanged == mapchanged) return;
			mapchanged = General.Map.IsChanged;
			UpdateTitle();
		}
		
		// This sets the focus on the display for correct key input
		public bool FocusDisplay()
		{
			return display.Focus();
		}

		// Window is first shown
		private void MainForm_Shown(object sender, EventArgs e)
		{
			// Perform auto map loading action when the window is not delayed
			if(!General.DelayMainWindow) PerformAutoMapLoading();
		}

		// Auto map loading that must be done when the window is first shown after loading
		// but also before the window is shown when the -delaywindow parameter is given
		internal void PerformAutoMapLoading()
		{
			// Check if the command line arguments tell us to load something
			if(General.AutoLoadFile != null)
			{
				bool showdialog = false;
				MapOptions options = new MapOptions();
				
				// Any of the options already given?
				if(General.AutoLoadMap != null)
				{
					Configuration mapsettings;
					
					// Try to find existing options in the settings file
					string dbsfile = General.AutoLoadFile.Substring(0, General.AutoLoadFile.Length - 4) + ".dbs";
					if(File.Exists(dbsfile))
						try { mapsettings = new Configuration(dbsfile, true); }
						catch(Exception) { mapsettings = new Configuration(true); }
					else
						mapsettings = new Configuration(true);

					//mxd. Get proper configuration file
					bool longtexturenamessupported = false;
					string configfile = General.AutoLoadConfig;
					if(string.IsNullOrEmpty(configfile)) configfile = mapsettings.ReadSetting("gameconfig", "");
					if(configfile.Trim().Length == 0)
					{
						showdialog = true;
					}
					else
					{
						// Get if long texture names are supported from the game configuration
						ConfigurationInfo configinfo = General.GetConfigurationInfo(configfile);
						longtexturenamessupported = configinfo.Configuration.ReadSetting("longtexturenames", false);
					}

					// Set map name and other options
					options = new MapOptions(mapsettings, General.AutoLoadMap, longtexturenamessupported);

					// Set resource data locations
					options.CopyResources(General.AutoLoadResources);

					// Set strict patches
					options.StrictPatches = General.AutoLoadStrictPatches;
					
					// Set configuration file (constructor already does this, but we want this info from the cmd args if possible)
					options.ConfigFile = configfile;
				}
				else
				{
					// No options given
					showdialog = true;
				}

				// Show open map dialog?
				if(showdialog)
				{
					// Show open dialog
					General.OpenMapFile(General.AutoLoadFile, null);
				}
				else
				{
					// Open with options
					General.OpenMapFileWithOptions(General.AutoLoadFile, options);
				}
			}
		}

		// Window is loaded
		private void MainForm_Load(object sender, EventArgs e)
		{
			//mxd. Enable drag and drop
			this.AllowDrop = true;
			this.DragEnter += OnDragEnter;
			this.DragDrop += OnDragDrop;

			// For checking if the drop down should really be closed
			buttontest.DropDown.Closing += ButtonTestDropDown_Closing;

			// Info panel state?
			bool expandedpanel = General.Settings.ReadSetting("windows." + configname + ".expandedinfopanel", true);
			if(expandedpanel != IsInfoPanelExpanded) ToggleInfoPanel();
		}

		// Window receives focus
		private void MainForm_Activated(object sender, EventArgs e)
		{
			windowactive = true;

			//UpdateInterface();
			ResumeExclusiveMouseInput();
			ReleaseAllKeys();
			FocusDisplay();
		}
		
		// Window loses focus
		private void MainForm_Deactivate(object sender, EventArgs e)
		{
			windowactive = false;
			
			BreakExclusiveMouseInput();
			ReleaseAllKeys();
		}

		//mxd. Looks like in some cases StartMouseExclusive is called before app aquires the mouse
		// which results in setting Cursor.Clip not taking effect.
		private void MainForm_MouseCaptureChanged(object sender, EventArgs e)
		{
			if(mouseexclusive && windowactive && mouseinside && Cursor.Clip != display.RectangleToScreen(display.ClientRectangle))
				Cursor.Clip = display.RectangleToScreen(display.ClientRectangle);
		}

		// Window is being closed
		protected override void OnFormClosing(FormClosingEventArgs e) 
		{
			base.OnFormClosing(e);
			if(e.CloseReason == CloseReason.ApplicationExitCall) return;

			// Close the map
			if(General.CloseMap()) 
			{
				General.WriteLogLine("Closing main interface window...");

				// Stop timers
				statusflasher.Stop();
				statusresetter.Stop();
				blinkTimer.Stop(); //mxd

				// Stop exclusive mode, if any is active
				StopExclusiveMouseInput();
				StopProcessing();

				// Unbind methods
				General.Actions.UnbindMethods(this);

				// Determine window state to save
				General.Settings.WriteSetting("windows." + configname + ".expandedinfopanel", IsInfoPanelExpanded);

				// Save recent files
				SaveRecentFiles();

				// Terminate the program
				General.Terminate(true);
			} 
			else 
			{
				// Cancel the close
				e.Cancel = true;
			}
		}

		//mxd
		private void OnDragEnter(object sender, DragEventArgs e) 
		{
			if(e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				e.Effect = DragDropEffects.Copy;
			} 
			else 
			{
				e.Effect = DragDropEffects.None;
			}
		}

		//mxd
		private void OnDragDrop(object sender, DragEventArgs e)
		{
			if(e.Data.GetDataPresent(DataFormats.FileDrop)) 
			{
				string[] filepaths = (string[])e.Data.GetData(DataFormats.FileDrop);
				if(filepaths.Length != 1) 
				{
					General.Interface.DisplayStatus(StatusType.Warning, "Cannot open multiple files at once!");
					return;
				}

				if(!File.Exists(filepaths[0])) 
				{
					General.Interface.DisplayStatus(StatusType.Warning, "Cannot open \"" + filepaths[0] + "\": file does not exist!");
					return;
				}

				string ext = Path.GetExtension(filepaths[0]);
				if(string.IsNullOrEmpty(ext) || ext.ToLower() != ".wad") 
				{
					General.Interface.DisplayStatus(StatusType.Warning, "Cannot open \"" + filepaths[0] + "\": only WAD files can be loaded this way!");
					return;
				}

				// If we call General.OpenMapFile here, it will lock the source window in the waiting state untill OpenMapOptionsForm is closed.
				Timer t = new Timer { Tag = filepaths[0], Interval = 10 };
				t.Tick += OnDragDropTimerTick;
				t.Start();
			}
		}

		private void OnDragDropTimerTick(object sender, EventArgs e)
		{
			Timer t = sender as Timer;
			if(t != null)
			{
				t.Stop();
				string targetwad = t.Tag.ToString();
				this.Update(); // Update main window
				General.OpenMapFile(targetwad, null);
				UpdateGZDoomPanel();
			}
		}

		#endregion
		
		#region ================== Statusbar
		
		// This updates the status bar
		private void UpdateStatusbar()
		{
			// Map open?
			if(General.Map != null)
			{
				// Enable items
				xposlabel.Enabled = true;
				yposlabel.Enabled = true;
				poscommalabel.Enabled = true;
				zoomlabel.Enabled = true;
				buttonzoom.Enabled = true;
				gridlabel.Enabled = true;
				itemgrid05.Visible = General.Map.UDMF; //mxd
				itemgrid025.Visible = General.Map.UDMF; //mxd
				itemgrid0125.Visible = General.Map.UDMF; //mxd
				buttongrid.Enabled = true;
				configlabel.Text = General.Map.Config.Name;
				
				//mxd. Raise grid size to 1 if it was lower and the map isn't in UDMF
				if(!General.Map.UDMF && General.Map.Grid.GridSizeF < GridSetup.MINIMUM_GRID_SIZE)
					General.Map.Grid.SetGridSize(GridSetup.MINIMUM_GRID_SIZE);
			}
			else
			{
				// Disable items
				xposlabel.Text = "--";
				yposlabel.Text = "--";
				xposlabel.Enabled = false;
				yposlabel.Enabled = false;
				poscommalabel.Enabled = false;
				zoomlabel.Enabled = false;
				buttonzoom.Enabled = false;
				gridlabel.Enabled = false;
				buttongrid.Enabled = false;
				configlabel.Text = "";
			}
			
			UpdateStatusIcon();
		}
		
		// This flashes the status icon
		private void statusflasher_Tick(object sender, EventArgs e)
		{
			statusflashicon = !statusflashicon;
			UpdateStatusIcon();
			statusflashcount--;
			if(statusflashcount == 0) statusflasher.Stop();
		}
		
		// This resets the status to ready
		private void statusresetter_Tick(object sender, EventArgs e)
		{
			DisplayReady();
		}
		
		// This changes status text
		public void DisplayStatus(StatusType type, string message) { DisplayStatus(new StatusInfo(type, message)); }
		public void DisplayStatus(StatusInfo newstatus)
		{
			// Stop timers
			if(!newstatus.displayed)
			{
				statusresetter.Stop();
				statusflasher.Stop();
				statusflashicon = false;
			}
			
			// Determine what to do specifically for this status type
			switch(newstatus.type)
			{
				// Shows information without flashing the icon.
				case StatusType.Ready: //mxd
				case StatusType.Selection: //mxd
				case StatusType.Info:
					if(!newstatus.displayed)
					{
						statusresetter.Interval = INFO_RESET_DELAY;
						statusresetter.Start();
					}
					break;
					
				// Shows action information and flashes up the status icon once.	
				case StatusType.Action:
					if(!newstatus.displayed)
					{
						statusflashicon = true;
						statusflasher.Interval = ACTION_FLASH_INTERVAL;
						statusflashcount = ACTION_FLASH_COUNT;
						statusflasher.Start();
						statusresetter.Interval = ACTION_RESET_DELAY;
						statusresetter.Start();
					}
					break;
					
				// Shows a warning, makes a warning sound and flashes a warning icon.
				case StatusType.Warning:
					if(!newstatus.displayed)
					{
						MessageBeep(MessageBeepType.Warning);
						statusflasher.Interval = WARNING_FLASH_INTERVAL;
						statusflashcount = WARNING_FLASH_COUNT;
						statusflasher.Start();
						statusresetter.Interval = WARNING_RESET_DELAY;
						statusresetter.Start();
					}
					break;
			}
			
			// Update status description
			status = newstatus;
			status.displayed = true;
			statuslabel.Text = status.ToString(); //mxd. message -> ToString()
			
			// Update icon as well
			UpdateStatusIcon();
			
			// Refresh
			statusbar.Invalidate();
			//this.Update(); // ano - this is unneeded afaict and slow
		}
		
		// This changes status text to Ready
		public void DisplayReady()
		{
			DisplayStatus(StatusType.Ready, null);
		}
		
		// This updates the status icon
		private void UpdateStatusIcon()
		{
			int statusicon = 0;
			int statusflashindex = statusflashicon ? 1 : 0;
			
			// Loading icon?
			if((General.Map != null) && (General.Map.Data != null) && General.Map.Data.IsLoading)
				statusicon = 1;
			
			// Status type
			switch(status.type)
			{
				case StatusType.Ready:
				case StatusType.Info:
				case StatusType.Action:
				case StatusType.Selection: //mxd
					statuslabel.Image = STATUS_IMAGES[statusflashindex, statusicon];
					break;
				
				case StatusType.Busy:
					statuslabel.Image = STATUS_IMAGES[statusflashindex, 2];
					break;
					
				case StatusType.Warning:
					statuslabel.Image = STATUS_IMAGES[statusflashindex, 3];
					break;
			}
		}
		
		// This changes coordinates display
		public void UpdateCoordinates(Vector2D coords){ UpdateCoordinates(coords, false); } //mxd
		public void UpdateCoordinates(Vector2D coords, bool snaptogrid)
		{
			//mxd
			if(snaptogrid) coords = General.Map.Grid.SnappedToGrid(coords);
			
			// X position
			xposlabel.Text = (double.IsNaN(coords.x) ? "--" : coords.x.ToString("####0"));

			// Y position
			yposlabel.Text = (double.IsNaN(coords.y) ? "--" : coords.y.ToString("####0"));
		}

		// This changes zoom display
		internal void UpdateZoom(float scale)
		{
			// Update scale label
			zoomlabel.Text = (float.IsNaN(scale) ? "--" : (scale * 100).ToString("##0") + "%");
		}

		// Zoom to a specified level
		private void itemzoomto_Click(object sender, EventArgs e)
		{
			// In classic mode?
			if(General.Map != null && General.Editing.Mode is ClassicMode)
			{
				// Requested from menu?
				ToolStripMenuItem item = sender as ToolStripMenuItem;
				if(item != null)
				{
					// Get integral zoom level
					int zoom = int.Parse(item.Tag.ToString(), CultureInfo.InvariantCulture);

					// Zoom now
					((ClassicMode)General.Editing.Mode).SetZoom(zoom / 100f);
				}
			}
		}

		// Zoom to fit in screen
		private void itemzoomfittoscreen_Click(object sender, EventArgs e)
		{
			// In classic mode?
			if(General.Map != null && General.Editing.Mode is ClassicMode)
				((ClassicMode)General.Editing.Mode).CenterInScreen();
		}

		// This changes grid display
		internal void UpdateGrid(double gridsize)
		{
			// Update grid label
			gridlabel.Text = (gridsize == 0 ? "--" : gridsize + " mp");
		}

		// Set grid to a specified size
		private void itemgridsize_Click(object sender, EventArgs e)
		{
			if(General.Map == null) return;

			// In classic mode?
			if(General.Editing.Mode is ClassicMode)
			{
				// Requested from menu?
				ToolStripMenuItem item = sender as ToolStripMenuItem;
				if(item != null)
				{
					//mxd. Get decimal zoom level
					float size = float.Parse(item.Tag.ToString(), CultureInfo.InvariantCulture);

					//mxd. Disable automatic grid resizing
					DisableDynamicGridResize();

					// Change grid size
					General.Map.Grid.SetGridSize(size);
					
					// Redraw display
					RedrawDisplay();
				}
			}
		}

		// Show grid setup
		private void itemgridcustom_Click(object sender, EventArgs e)
		{
			if(General.Map != null) GridSetup.ShowGridSetup();
		}
		
		#endregion

		#region ================== Display

		// This shows the splash screen on display
		internal void ShowSplashDisplay()
		{
			// Change display to show splash logo
			display.SetSplashLogoDisplay();
			display.Cursor = Cursors.Default;
			this.Update();
		}
		
		// This clears the display
		internal void ClearDisplay()
		{
			// Clear the display
			display.SetManualRendering();
			this.Update();
		}

		// This sets the display cursor
		public void SetCursor(Cursor cursor)
		{
			// Only when a map is open
			if(General.Map != null) display.Cursor = cursor;
		}

		// This redraws the display on the next paint event
		public void RedrawDisplay()
		{
			if((General.Map != null) && (General.Editing.Mode != null))
			{
				General.Plugins.OnEditRedrawDisplayBegin();
				General.Editing.Mode.OnRedrawDisplay();
				General.Plugins.OnEditRedrawDisplayEnd();
				statistics.UpdateStatistics(); //mxd
			}
			else
			{
				display.Invalidate();
			}
		}

		// This event is called when a repaint is needed
		private void display_Paint(object sender, PaintEventArgs e)
		{
			if(General.Map != null)
			{
				if(General.Editing.Mode != null)
				{
					if(!displayresized) General.Editing.Mode.OnPresentDisplay();
				}
				else
				{
					if(General.Colors != null)
						e.Graphics.Clear(Color.FromArgb(General.Colors.Background.ToInt()));
					else
						e.Graphics.Clear(SystemColors.ControlDarkDark);
				}
			}
		}
		
		// Redraw requested
		private void redrawtimer_Tick(object sender, EventArgs e)
		{
			// Disable timer (only redraw once)
			redrawtimer.Enabled = false;

			// Don't do anything when minimized (mxd)
			if(this.WindowState == FormWindowState.Minimized) return;

			// Resume control layouts
			//if(displayresized) General.LockWindowUpdate(IntPtr.Zero);

			// Map opened?
			if(General.Map != null)
			{
				// Display was resized?
				if(displayresized)
				{
					//mxd. Aspect ratio may've been changed
					General.Map.CRenderer3D.CreateProjection();
				}

				// This is a dirty trick to give the display a new mousemove event with correct arguments
				if(mouseinside)
				{
					Point mousepos = Cursor.Position;
					Cursor.Position = new Point(mousepos.X + 1, mousepos.Y + 1);
					Cursor.Position = mousepos;
				}
				
				// Redraw now
				RedrawDisplay();
			}

			// Display resize is done
			displayresized = false;
		}

		// Display size changes
		private void display_Resize(object sender, EventArgs e)
		{
			// Resizing
			//if(!displayresized) General.LockWindowUpdate(display.Handle);
			displayresized = true;

			//mxd. Separators may need updating
			UpdateSeparators();
			
			// Request redraw
			if(!redrawtimer.Enabled) redrawtimer.Enabled = true;
		}
		
		// This requests a delayed redraw
		public void DelayedRedraw()
		{
			// Request redraw
			if(!redrawtimer.Enabled) redrawtimer.Enabled = true;
		}
		
		// Mouse click
		private void display_MouseClick(object sender, MouseEventArgs e)
		{
			if((General.Map != null) && (General.Editing.Mode != null))
			{
				General.Plugins.OnEditMouseClick(e);
				General.Editing.Mode.OnMouseClick(e);
			}
		}

		// Mouse doubleclick
		private void display_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			if((General.Map != null) && (General.Editing.Mode != null))
			{
				General.Plugins.OnEditMouseDoubleClick(e);
				General.Editing.Mode.OnMouseDoubleClick(e);
			}
		}

		// Mouse down
		private void display_MouseDown(object sender, MouseEventArgs e)
		{
			int key = 0;
			
			LoseFocus(this, EventArgs.Empty);
			
			int mod = 0;
			if(alt) mod |= (int)Keys.Alt;
			if(shift) mod |= (int)Keys.Shift;
			if(ctrl) mod |= (int)Keys.Control;
			
			// Apply button
			mousebuttons |= e.Button;
			
			// Create key
			switch(e.Button)
			{
				case MouseButtons.Left: key = (int)Keys.LButton; break;
				case MouseButtons.Middle: key = (int)Keys.MButton; break;
				case MouseButtons.Right: key = (int)Keys.RButton; break;
				case MouseButtons.XButton1: key = (int)Keys.XButton1; break;
				case MouseButtons.XButton2: key = (int)Keys.XButton2; break;
			}
			
			// Invoke any actions associated with this key
			General.Actions.KeyPressed(key | mod);
			
			// Invoke on editing mode
			if((General.Map != null) && (General.Editing.Mode != null))
			{
				General.Plugins.OnEditMouseDown(e);
				General.Editing.Mode.OnMouseDown(e);
			}
		}

		// Mouse enters
		private void display_MouseEnter(object sender, EventArgs e)
		{
			mouseinside = true;
			//mxd. Skip when in mouseexclusive (e.g. Visual) mode to avoid mouse disappearing when moving it
			// on top of inactive editor window while Visual mode is active
			if((General.Map != null) && (mouseinput == null) && (General.Editing.Mode != null) && !mouseexclusive)
			{
				General.Plugins.OnEditMouseEnter(e);
				General.Editing.Mode.OnMouseEnter(e);
				if(Application.OpenForms.Count == 1 || editformopen) display.Focus(); //mxd
			}
		}

		// Mouse leaves
		private void display_MouseLeave(object sender, EventArgs e)
		{
			mouseinside = false;
			if((General.Map != null) && (mouseinput == null) && (General.Editing.Mode != null))
			{
				General.Plugins.OnEditMouseLeave(e);
				General.Editing.Mode.OnMouseLeave(e);
			}
		}

		// Mouse moves
		private void display_MouseMove(object sender, MouseEventArgs e)
		{
			if((General.Map != null) && (mouseinput == null) && (General.Editing.Mode != null))
			{
				General.Plugins.OnEditMouseMove(e);
				General.Editing.Mode.OnMouseMove(e);
			}
		}

		// Mouse up
		private void display_MouseUp(object sender, MouseEventArgs e)
		{
			int key = 0;
			
			int mod = 0;
			if(alt) mod |= (int)Keys.Alt;
			if(shift) mod |= (int)Keys.Shift;
			if(ctrl) mod |= (int)Keys.Control;
			
			// Apply button
			mousebuttons &= ~e.Button;
			
			// Create key
			switch(e.Button)
			{
				case MouseButtons.Left: key = (int)Keys.LButton; break;
				case MouseButtons.Middle: key = (int)Keys.MButton; break;
				case MouseButtons.Right: key = (int)Keys.RButton; break;
				case MouseButtons.XButton1: key = (int)Keys.XButton1; break;
				case MouseButtons.XButton2: key = (int)Keys.XButton2; break;
			}
			
			// Invoke any actions associated with this key
			General.Actions.KeyReleased(key | mod);

			// Invoke on editing mode
			if((General.Map != null) && (General.Editing.Mode != null))
			{
				General.Plugins.OnEditMouseUp(e);
				General.Editing.Mode.OnMouseUp(e);
			}
		}
		
		#endregion

		#region ================== Input
		
		// This is a tool to lock the mouse in exclusive mode
		private void StartMouseExclusive()
		{
			// Not already locked?
			if(mouseinput == null)
			{
				// Start special input device
				mouseinput = new MouseInput(this);

				// Lock and hide the mouse in window
				Cursor.Position = display.PointToScreen(new Point(display.ClientSize.Width / 2, display.ClientSize.Height / 2)); //mxd
				Cursor.Clip = display.RectangleToScreen(display.ClientRectangle);
				Cursor.Hide();
				
				#if MONO_WINFORMS
				// A beautiful transparent cursor, just for you mono!
				string emptycursor =
					"AAACAAEAICACAAAAAAAwAQAAFgAAACgAAAAgAAAAQAAAAAEAAQAAAAAAgAAAAAAAAAAAAAAAAgAA" +
					"AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" +
					"AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" +
					"AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA////////////////////////////////////////" +
					"////////////////////////////////////////////////////////////////////////////" +
					"//////////////////////////////////////////////////////8=";
				using (var stream = new MemoryStream(System.Convert.FromBase64String(emptycursor)))
				{
					var cursor = new Cursor(stream);
					Cursor.Current = cursor;
					display.Cursor = cursor;
				}
				Application.DoEvents();
				#endif
			}
		}

		// This is a tool to unlock the mouse
		private void StopMouseExclusive()
		{
			// Locked?
			if(mouseinput != null)
			{
				// Stop special input device
				mouseinput.Dispose();
				mouseinput = null;

				// Release and show the mouse
				Cursor.Clip = Rectangle.Empty;
				Cursor.Position = display.PointToScreen(new Point(display.ClientSize.Width / 2, display.ClientSize.Height / 2));
				#if MONO_WINFORMS
				Cursor.Current = Cursors.Default;
				display.Cursor = Cursors.Default;
				Application.DoEvents();
				#endif
				Cursor.Show();
			}
		}
		
		// This requests exclusive mouse input
		public void StartExclusiveMouseInput()
		{
			// Only when not already in exclusive mode
			if(!mouseexclusive)
			{
				General.WriteLogLine("Starting exclusive mouse input mode...");
				
				// Start special input device
				StartMouseExclusive();
				mouseexclusive = true;
				mouseexclusivebreaklevel = 0;
			}
		}
		
		// This stops exclusive mouse input
		public void StopExclusiveMouseInput()
		{
			// Only when in exclusive mode
			if(mouseexclusive)
			{
				General.WriteLogLine("Stopping exclusive mouse input mode...");

				// Stop special input device
				StopMouseExclusive();
				mouseexclusive = false;
				mouseexclusivebreaklevel = 0;
			}
		}

		// This temporarely breaks exclusive mode and counts the break level
		public void BreakExclusiveMouseInput()
		{
			// Only when in exclusive mode
			if(mouseexclusive)
			{
				// Stop special input device
				StopMouseExclusive();
				
				// Count the break level
				mouseexclusivebreaklevel++;
			}
		}

		// This resumes exclusive mode from a break when all breaks have been called to resume
		public void ResumeExclusiveMouseInput()
		{
			// Only when in exclusive mode
			if(mouseexclusive && (mouseexclusivebreaklevel > 0))
			{
				// Decrease break level
				mouseexclusivebreaklevel--;

				// All break levels resumed? Then lock the mouse again.
				if(mouseexclusivebreaklevel == 0)
					StartMouseExclusive();
			}
		}

		// This releases all keys
		internal void ReleaseAllKeys()
		{
			General.Actions.ReleaseAllKeys();
			mousebuttons = MouseButtons.None;
			shift = false;
			ctrl = false;
			alt = false;
		}
		
		// When the mouse wheel is changed
		protected override void OnMouseWheel(MouseEventArgs e)
		{
			int mod = 0;
			if(alt) mod |= (int)Keys.Alt;
			if(shift) mod |= (int)Keys.Shift;
			if(ctrl) mod |= (int)Keys.Control;

			// Only send key events when the main window can be focused (i.e. no modal dialogs are open)
			if (CanFocus)
			{
				// Scrollwheel up?
				if (e.Delta > 0)
				{
					// Invoke actions for scrollwheel
					//for(int i = 0; i < e.Delta; i += 120)
					General.Actions.KeyPressed((int)SpecialKeys.MScrollUp | mod);
					General.Actions.KeyReleased((int)SpecialKeys.MScrollUp | mod);
				}
				// Scrollwheel down?
				else if (e.Delta < 0)
				{
					// Invoke actions for scrollwheel
					//for(int i = 0; i > e.Delta; i -= 120)
					General.Actions.KeyPressed((int)SpecialKeys.MScrollDown | mod);
					General.Actions.KeyReleased((int)SpecialKeys.MScrollDown | mod);
				}
			}
			
			// Let the base know
			base.OnMouseWheel(e);
		}

		// [ZZ]
		private void OnMouseHWheel(int delta)
		{
			int mod = 0;
			if (alt) mod |= (int)Keys.Alt;
			if (shift) mod |= (int)Keys.Shift;
			if (ctrl) mod |= (int)Keys.Control;

			// Scrollwheel left?
			if (delta < 0)
			{
				General.Actions.KeyPressed((int)SpecialKeys.MScrollLeft | mod);
				General.Actions.KeyReleased((int)SpecialKeys.MScrollLeft | mod);
			}
			else if (delta > 0)
			{
				General.Actions.KeyPressed((int)SpecialKeys.MScrollRight | mod);
				General.Actions.KeyReleased((int)SpecialKeys.MScrollRight | mod);
			}

			// base? what base?
		}
		
		private void MainForm_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			if (e.KeyCode == Keys.F10)
				e.IsInputKey = true;
		}

		// When a key is pressed
		private void MainForm_KeyDown(object sender, KeyEventArgs e)
		{
			int mod = 0;
			
			// Keep key modifiers
			alt = e.Alt;
			shift = e.Shift;
			ctrl = e.Control;
			if(alt) mod |= (int)Keys.Alt;
			if(shift) mod |= (int)Keys.Shift;
			if(ctrl) mod |= (int)Keys.Control;
			
			// Don't process any keys when they are meant for other input controls
			if((e.KeyData != Keys.None) && ((ActiveControl == null) || (ActiveControl == display)))
			{
				// Invoke any actions associated with this key
				General.Actions.UpdateModifiers(mod);
				e.Handled = General.Actions.KeyPressed((int)e.KeyData);
				
				// Invoke on editing mode
				if((General.Map != null) && (General.Editing.Mode != null))
				{
					General.Plugins.OnEditKeyDown(e);
					General.Editing.Mode.OnKeyDown(e);
				}

				// Handled
				if(e.Handled)
					e.SuppressKeyPress = true;
			}
			
			// F1 pressed?
			if((e.KeyCode == Keys.F1) && (e.Modifiers == Keys.None))
			{
				// No action bound to F1?
				Actions.Action[] f1actions = General.Actions.GetActionsByKey((int)e.KeyData);
				if(f1actions.Length == 0)
				{
					// If we don't have any map open, show the Main Window help
					// otherwise, give the help request to the editing mode so it
					// can open the appropriate help file.
					if((General.Map == null) || (General.Editing.Mode == null))
					{
						General.ShowHelp("introduction.html");
					}
					else
					{
						General.Editing.Mode.OnHelp();
					}
				}
			}

			if (e.KeyCode == Keys.F10)
			{
				Actions.Action[] f10actions = General.Actions.GetActionsByKey((int)e.KeyData);
				if (f10actions.Length > 0)
				{
					e.SuppressKeyPress = true;
					e.Handled = true;
				}
			}
		}

		// When a key is released
		private void MainForm_KeyUp(object sender, KeyEventArgs e)
		{
			int mod = 0;
			
			// Keep key modifiers
			alt = e.Alt;
			shift = e.Shift;
			ctrl = e.Control;
			if(alt) mod |= (int)Keys.Alt;
			if(shift) mod |= (int)Keys.Shift;
			if(ctrl) mod |= (int)Keys.Control;
			
			// Don't process any keys when they are meant for other input controls
			if((ActiveControl == null) || (ActiveControl == display))
			{
				// Invoke any actions associated with this key
				General.Actions.UpdateModifiers(mod);
				e.Handled = General.Actions.KeyReleased((int)e.KeyData);
				
				// Invoke on editing mode
				if((General.Map != null) && (General.Editing.Mode != null))
				{
					General.Plugins.OnEditKeyUp(e);
					General.Editing.Mode.OnKeyUp(e);
				}
				
				// Handled
				if(e.Handled)
					e.SuppressKeyPress = true;
			}

			if (e.KeyCode == Keys.F10)
			{
				Actions.Action[] f10actions = General.Actions.GetActionsByKey((int)e.KeyData);
				if (f10actions.Length > 0)
				{
					e.SuppressKeyPress = true;
					e.Handled = true;
				}
			}
		}

		//mxd. Sometimes it's handeled by RenderTargetControl, not by MainForm leading to keys being "stuck"
		private void display_OnKeyReleased(object sender, KeyEventArgs e)
		{
			MainForm_KeyUp(sender, e);
		}
		
		// These prevent focus changes by way of TAB or Arrow keys
		protected override bool IsInputChar(char charCode) { return false; }
		protected override bool IsInputKey(Keys keyData) { return false; }
		protected override bool ProcessKeyPreview(ref Message m) { return false; }
		protected override bool ProcessDialogKey(Keys keyData) { return false; }
		protected override bool ProcessCmdKey(ref Message msg, Keys keyData) { return false; }
		
		// This fixes some odd input behaviour
		private void display_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			if((ActiveControl == null) || (ActiveControl == display))
			{
				LoseFocus(this, EventArgs.Empty);
				KeyEventArgs ea = new KeyEventArgs(e.KeyData);
				MainForm_KeyDown(sender, ea);
			}
		}
		
		#endregion

		#region ================== Toolbar
		
		// This updates the skills list
		private void UpdateSkills()
		{
			// Clear list
			buttontest.DropDownItems.Clear();

			// Map loaded?
			if (General.Map != null)
			{
				int maxwidth = 0;

				// Make the new items list
				List<ToolStripItem> items = new List<ToolStripItem>(General.Map.Config.Skills.Count * 2 + General.Map.ConfigSettings.TestEngines.Count + 2);
				
				// Positive skills are with monsters
				foreach(SkillInfo si in General.Map.Config.Skills)
				{
					ToolStripMenuItem menuitem = new ToolStripMenuItem(si.ToString());
					menuitem.Image = Resources.Monster2;
					menuitem.Click += TestSkill_Click;
					menuitem.Tag = si.Index;
					menuitem.Checked = (General.Settings.TestMonsters && (General.Map.ConfigSettings.TestSkill == si.Index));
					items.Add(menuitem);

					int width = (int)MeasureString(si.ToString(), menuitem.Font).Width;
					if (width > maxwidth)
						maxwidth = width;
				}

				// Add separator
				items.Add(new ToolStripSeparator { Padding = new Padding(0, 3, 0, 3) });

				// Negative skills are without monsters
				foreach(SkillInfo si in General.Map.Config.Skills)
				{
					ToolStripMenuItem menuitem = new ToolStripMenuItem(si.ToString());
					menuitem.Image = Resources.Monster3;
					menuitem.Click += TestSkill_Click;
					menuitem.Tag = -si.Index;
					menuitem.Checked = (!General.Settings.TestMonsters && (General.Map.ConfigSettings.TestSkill == si.Index));
					items.Add(menuitem);

					int width = (int)MeasureString(si.ToString(), menuitem.Font).Width;
					if (width > maxwidth)
						maxwidth = width;
				}

				//mxd. Add separator
				items.Add(new ToolStripSeparator { Padding = new Padding(0, 3, 0, 3) });

				//mxd. Add test engines
				for(int i = 0; i < General.Map.ConfigSettings.TestEngines.Count; i++)
				{
					if(General.Map.ConfigSettings.TestEngines[i].TestProgramName == EngineInfo.DEFAULT_ENGINE_NAME) continue;
					ToolStripMenuItem menuitem = new ToolStripMenuItem(General.Map.ConfigSettings.TestEngines[i].TestProgramName);
					menuitem.Image = General.Map.ConfigSettings.TestEngines[i].TestProgramIcon;
					menuitem.Click += TestEngine_Click;
					menuitem.Tag = i;
					menuitem.Checked = (i == General.Map.ConfigSettings.CurrentEngineIndex);
					items.Add(menuitem);

					int width = (int)MeasureString(General.Map.ConfigSettings.TestEngines[i].TestProgramName, menuitem.Font).Width;
					if (width > maxwidth)
						maxwidth = width;
				}

				// Add separator
				items.Add(new ToolStripSeparator { Padding = new Padding(0, 3, 0, 3) });

				// Text box for additional parameters
				PlaceholderToolStripTextBox ptstb = new PlaceholderToolStripTextBox();
				ptstb.BorderStyle = BorderStyle.FixedSingle;
				ptstb.PlaceholderText = "Additional parameters";
				ptstb.AutoSize = false;
				ptstb.Width = maxwidth;
				ptstb.Text = General.Map.ConfigSettings.TestAdditionalParameters;
				ptstb.TextChanged += (sender, e) => { General.Map.ConfigSettings.TestAdditionalParameters = ptstb.Text; };
				ptstb.KeyDown += (sender, e) => { if(e.KeyCode == Keys.Enter) General.Map.Launcher.TestAtSkill(General.Map.ConfigSettings.TestSkill); };
				items.Add(ptstb);

				// Add to list
				buttontest.DropDownItems.AddRange(items.ToArray());
			}
		}

		//mxd
		internal void DisableDynamicGridResize()
		{
			if(General.Settings.DynamicGridSize)
			{
				General.Settings.DynamicGridSize = false;
				itemdynamicgridsize.Checked = false;
				buttontoggledynamicgrid.Checked = false;
			}
		}

		//mxd
		private void TestEngine_Click(object sender, EventArgs e)
		{
			General.Map.ConfigSettings.CurrentEngineIndex = (int)(((ToolStripMenuItem)sender).Tag);
			General.Map.ConfigSettings.Changed = true;

			if(General.Settings.AutoLaunchOnTest)
				General.Map.Launcher.TestAtSkill(General.Map.ConfigSettings.TestSkill);

			UpdateSkills();
		}
		
		// Event handler for testing at a specific skill
		private void TestSkill_Click(object sender, EventArgs e)
		{
			int skill = (int)((sender as ToolStripMenuItem).Tag);
			General.Settings.TestMonsters = (skill > 0);
			General.Map.ConfigSettings.TestSkill = Math.Abs(skill);

			if(General.Settings.AutoLaunchOnTest)
				General.Map.Launcher.TestAtSkill(Math.Abs(skill));

			UpdateSkills();
		}
		
		// This loses focus
		private void LoseFocus(object sender, EventArgs e)
		{
			// Lose focus!
			try { display.Focus(); } catch(Exception) { }
			this.ActiveControl = null;
		}

		//mxd. Things filter selected
		private void thingfilters_DropDownItemClicked(object sender, EventArgs e)
		{
			// Only possible when a map is open
			if((General.Map != null) && !updatingfilters)
			{
				updatingfilters = true;
				ToolStripMenuItem clickeditem = sender as ToolStripMenuItem;

				// Keep already selected items selected
				if(!clickeditem.Checked)
				{
					clickeditem.Checked = true;
					updatingfilters = false;
					return;
				}

				// Change filter
				ThingsFilter f = clickeditem.Tag as ThingsFilter;
				General.Map.ChangeThingFilter(f);

				// Deselect other items...
				foreach(var item in thingfilters.DropDown.Items)
				{
					if(item != clickeditem) ((ToolStripMenuItem)item).Checked = false;
				}

				// Update button text
				thingfilters.Text = f.Name;

				updatingfilters = false;
			}
			
			// Lose focus
			LoseFocus(sender, e);
		}
		
		//mxd. This updates the things filter on the toolbar
		internal void UpdateThingsFilters()
		{
			// Only possible to list filters when a map is open
			if(General.Map != null)
			{
				ThingsFilter oldfilter = null;

				// Anything selected?
				foreach(var item in thingfilters.DropDown.Items)
				{
					if(((ToolStripMenuItem)item).Checked)
					{
						oldfilter = ((ToolStripMenuItem)item).Tag as ThingsFilter;
						break;
					}
				}
				
				updatingfilters = true;

				// Clear the list
				thingfilters.DropDown.Items.Clear();

				// Add null filter
				if(General.Map.ThingsFilter is NullThingsFilter)
					thingfilters.DropDown.Items.Add(CreateThingsFilterMenuItem(General.Map.ThingsFilter));
				else
					thingfilters.DropDown.Items.Add(CreateThingsFilterMenuItem(new NullThingsFilter()));

				// Add all filters, select current one
				foreach(ThingsFilter f in General.Map.ConfigSettings.ThingsFilters)
					thingfilters.DropDown.Items.Add(CreateThingsFilterMenuItem(f));

				updatingfilters = false;
				
				// No filter selected?
				ToolStripMenuItem selecteditem = null;
				foreach(var i in thingfilters.DropDown.Items)
				{
					ToolStripMenuItem item = i as ToolStripMenuItem;
					if(item.Checked)
					{
						selecteditem = item;
						break;
					}
				}

				if(selecteditem == null)
				{
					ToolStripMenuItem first = thingfilters.DropDown.Items[0] as ToolStripMenuItem;
					first.Checked = true;
				}
				// Another filter got selected?
				else if(selecteditem.Tag != oldfilter)
				{
					selecteditem.Checked = true;
				}

				// Update button text
				if(selecteditem != null)
					thingfilters.Text = ((ThingsFilter)selecteditem.Tag).Name;
			}
			else
			{
				// Clear the list
				thingfilters.DropDown.Items.Clear();
				thingfilters.Text = "(show all)";
			}
		}

		// This selects the things filter based on the filter set on the map manager
		internal void ReflectThingsFilter()
		{
			if(!updatingfilters)
			{
				updatingfilters = true;
				
				// Select current filter
				bool selecteditemfound = false;
				foreach(var i in thingfilters.DropDown.Items)
				{
					ToolStripMenuItem item = i as ToolStripMenuItem;
					ThingsFilter f = item.Tag as ThingsFilter;

					if(f == General.Map.ThingsFilter)
					{
						item.Checked = true;
						thingfilters.Text = f.Name;
						selecteditemfound = true;
					}
					else
					{
						item.Checked = false;
					}
				}

				// Not in the list?
				if(!selecteditemfound)
				{
					// Select nothing
					thingfilters.Text = "(show all)"; //mxd
				}

				updatingfilters = false;
			}
		}

		//mxd
		private ToolStripMenuItem CreateThingsFilterMenuItem(ThingsFilter f)
		{
			// Make decorated name
			string name = f.Name;
			if(f.Invert) name = "!" + name;
			switch(f.DisplayMode)
			{
				case ThingsFilterDisplayMode.CLASSIC_MODES_ONLY: name += " [2D]"; break;
				case ThingsFilterDisplayMode.VISUAL_MODES_ONLY: name += " [3D]"; break;
			}

			// Create and select the item
			ToolStripMenuItem item = new ToolStripMenuItem(name) { CheckOnClick = true, Tag = f };
			item.CheckedChanged += thingfilters_DropDownItemClicked;
			item.Checked = (f == General.Map.ThingsFilter);
			
			// Update icon
			if(!(f is NullThingsFilter) && !f.IsValid())
			{
				item.Image = Resources.Warning;
				//item.ImageScaling = ToolStripItemImageScaling.None;
			}

			return item;
		}

		//mxd. Linedef color preset (de)selected
		private void linedefcolorpresets_ItemClicked(object sender, EventArgs e)
		{
			ToolStripMenuItem item = sender as ToolStripMenuItem;
			((LinedefColorPreset)item.Tag).Enabled = item.Checked;

			List<string> enablednames = new List<string>();
			foreach(LinedefColorPreset p in General.Map.ConfigSettings.LinedefColorPresets)
			{
				if(p.Enabled) enablednames.Add(p.Name);
			}

			// Update button text
			UpdateColorPresetsButtonText(linedefcolorpresets, enablednames);
			
			General.Map.Map.UpdateCustomLinedefColors();
			General.Map.ConfigSettings.Changed = true;

			// Update display
			if(General.Editing.Mode is ClassicMode) General.Interface.RedrawDisplay();
		}

		//mxd. Handle Shift key...
		private void linedefcolorpresets_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
			linedefcolorpresets.DropDown.AutoClose = (ModifierKeys != Keys.Shift);
		}

		//mxd. Handles the mouse leaving linedefcolorpresets.DropDown and clicking on linedefcolorpresets button
		private void linedefcolorpresets_MouseLeave(object sender, EventArgs e)
		{
			linedefcolorpresets.DropDown.AutoClose = true;
		}

		//mxd. This updates linedef color presets selector on the toolbar
		internal void UpdateLinedefColorPresets()
		{
			// Refill the list
			List<string> enablednames = new List<string>();
			linedefcolorpresets.DropDown.Items.Clear();

			if(General.Map != null)
			{
				foreach(LinedefColorPreset p in General.Map.ConfigSettings.LinedefColorPresets)
				{
					// Create menu item
					ToolStripMenuItem item = new ToolStripMenuItem(p.Name)
					{
						CheckOnClick = true,
						Tag = p,
						//ImageScaling = ToolStripItemImageScaling.None,
						Checked = p.Enabled,
						ToolTipText = "Hold Shift to toggle several items at once"
					};

					// Create icon
					if(p.IsValid())
					{
						Bitmap icon = new Bitmap(16, 16);
						using(Graphics g = Graphics.FromImage(icon))
						{
							g.FillRectangle(new SolidBrush(p.Color.ToColor()), 2, 3, 12, 10);
							g.DrawRectangle(Pens.Black, 2, 3, 11, 9);
						}

						item.Image = icon;
					}
					// Or use the warning icon
					else
					{
						item.Image = Resources.Warning;
					}

					item.CheckedChanged += linedefcolorpresets_ItemClicked;
					linedefcolorpresets.DropDown.Items.Add(item);
					if(p.Enabled) enablednames.Add(p.Name);
				}
			}

			// Update button text
			UpdateColorPresetsButtonText(linedefcolorpresets, enablednames);
		}

		//mxd
		private static void UpdateColorPresetsButtonText(ToolStripItem button, List<string> names)
		{
			if(names.Count == 0)
			{
				button.Text = "No active presets";
			}
			else
			{
				string text = string.Join(", ", names.ToArray());
				if(TextRenderer.MeasureText(text, button.Font).Width > button.Width)
					button.Text = names.Count + (names.Count.ToString(CultureInfo.InvariantCulture).EndsWith("1") ? " preset" : " presets") + " active";
				else
					button.Text = text;
			}
		}

		//mxd
		public void BeginToolbarUpdate()
		{
			toolbar.SuspendLayout();
			modestoolbar.SuspendLayout();
			modecontrolsloolbar.SuspendLayout();
		}

		//mxd
		public void EndToolbarUpdate()
		{
			toolbar.ResumeLayout(true);
			modestoolbar.ResumeLayout(true);
			modecontrolsloolbar.ResumeLayout(true);
		}

		// This adds a button to the toolbar
		public void AddButton(ToolStripItem button) { AddButton(button, ToolbarSection.Custom, General.Plugins.FindPluginByAssembly(Assembly.GetCallingAssembly())); }
		public void AddButton(ToolStripItem button, ToolbarSection section) { AddButton(button, section, General.Plugins.FindPluginByAssembly(Assembly.GetCallingAssembly())); }
		private void AddButton(ToolStripItem button, ToolbarSection section, Plugin plugin)
		{
			// Fix tags to full action names
			ToolStripItemCollection items = new ToolStripItemCollection(toolbar, new ToolStripItem[0]);
			items.Add(button);
			RenameTagsToFullActions(items, plugin);

			// Add to the list so we can update it as needed
			PluginToolbarButton buttoninfo = new PluginToolbarButton();
			buttoninfo.button = button;
			buttoninfo.section = section;
			pluginbuttons.Add(buttoninfo);
			
			// Bind visible changed event
			if(!(button is ToolStripSeparator)) button.VisibleChanged += buttonvisiblechangedhandler;
			
			// Insert the button in the right section
			switch(section)
			{
				case ToolbarSection.File: toolbar.Items.Insert(toolbar.Items.IndexOf(seperatorfile), button); break;
				case ToolbarSection.Script: toolbar.Items.Insert(toolbar.Items.IndexOf(seperatorscript), button); break;
				case ToolbarSection.UndoRedo: toolbar.Items.Insert(toolbar.Items.IndexOf(seperatorundo), button); break;
				case ToolbarSection.CopyPaste: toolbar.Items.Insert(toolbar.Items.IndexOf(seperatorcopypaste), button); break;
				case ToolbarSection.Prefabs: toolbar.Items.Insert(toolbar.Items.IndexOf(seperatorprefabs), button); break;
				case ToolbarSection.Things: toolbar.Items.Insert(toolbar.Items.IndexOf(buttonviewnormal), button); break;
				case ToolbarSection.Views: toolbar.Items.Insert(toolbar.Items.IndexOf(seperatorviews), button); break;
				case ToolbarSection.Geometry: toolbar.Items.Insert(toolbar.Items.IndexOf(seperatorgeometry), button); break;
				case ToolbarSection.Helpers: toolbar.Items.Insert(toolbar.Items.IndexOf(separatorgzmodes), button); break; //mxd
				case ToolbarSection.Testing: toolbar.Items.Insert(toolbar.Items.IndexOf(seperatortesting), button); break;
				case ToolbarSection.Modes: modestoolbar.Items.Add(button); break; //mxd
				case ToolbarSection.Custom: modecontrolsloolbar.Items.Add(button); modecontrolsloolbar.Visible = true; break; //mxd
			}
			
			UpdateToolbar();
		}

		//mxd
		public void AddModesButton(ToolStripItem button, string group) 
		{
			// Set proper styling
			button.Padding = new Padding(0, 1, 0, 1);
			button.Margin = new Padding();
			
			// Fix tags to full action names
			ToolStripItemCollection items = new ToolStripItemCollection(toolbar, new ToolStripItem[0]);
			items.Add(button);
			RenameTagsToFullActions(items, General.Plugins.FindPluginByAssembly(Assembly.GetCallingAssembly()));

			// Add to the list so we can update it as needed
			PluginToolbarButton buttoninfo = new PluginToolbarButton();
			buttoninfo.button = button;
			buttoninfo.section = ToolbarSection.Modes;
			pluginbuttons.Add(buttoninfo);

			button.VisibleChanged += buttonvisiblechangedhandler;

			//find the separator we need
			for(int i = 0; i < modestoolbar.Items.Count; i++) 
			{
				if(modestoolbar.Items[i] is ToolStripSeparator && modestoolbar.Items[i].Text == group) 
				{
					modestoolbar.Items.Insert(i + 1, button);
					break;
				}
			}

			UpdateToolbar();
		}

		// Removes a button
		public void RemoveButton(ToolStripItem button)
		{
			// Find in the list and remove it
			PluginToolbarButton buttoninfo = new PluginToolbarButton();
			for(int i = 0; i < pluginbuttons.Count; i++)
			{
				if(pluginbuttons[i].button == button)
				{
					buttoninfo = pluginbuttons[i];
					pluginbuttons.RemoveAt(i);
					break;
				}
			}

			if(buttoninfo.button != null)
			{
				// Unbind visible changed event
				if(!(button is ToolStripSeparator)) button.VisibleChanged -= buttonvisiblechangedhandler;

				//mxd. Remove button from toolbars
				switch(buttoninfo.section) 
				{
					case ToolbarSection.Modes:
						modestoolbar.Items.Remove(button);
						break;
					case ToolbarSection.Custom:
						modecontrolsloolbar.Items.Remove(button);
						modecontrolsloolbar.Visible = (modecontrolsloolbar.Items.Count > 0);
						break;
					default:
						toolbar.Items.Remove(button);
						break;
				}
				
				UpdateSeparators();
			}
		}

		// This handle visibility changes in the toolbar buttons
		private void ToolbarButtonVisibleChanged(object sender, EventArgs e)
		{
			if(!preventupdateseperators)
			{
				// Update the seeprators
				UpdateSeparators();
			}
		}

		// This hides redundant separators
		internal void UpdateSeparators()
		{
			UpdateToolStripSeparators(toolbar.Items, false);
			UpdateToolStripSeparators(menumode.DropDownItems, true);

			//mxd
			UpdateToolStripSeparators(modestoolbar.Items, true);
			UpdateToolStripSeparators(modecontrolsloolbar.Items, true);
		}
		
		// This hides redundant separators
		private static void UpdateToolStripSeparators(ToolStripItemCollection items, bool defaultvisible)
		{
			ToolStripItem pvi = null;
			foreach(ToolStripItem i in items) 
			{
				bool separatorvisible = false;

				// This is a seperator?
				if(i is ToolStripSeparator) 
				{
					// Make visible when previous item was not a seperator
					separatorvisible = !(pvi is ToolStripSeparator) && (pvi != null);
					i.Visible = separatorvisible;
				}

				// Keep as previous visible item
				if(i.Visible || separatorvisible || (defaultvisible && !(i is ToolStripSeparator))) pvi = i;
			}

			// Hide last item if it is a seperator
			if(pvi is ToolStripSeparator) pvi.Visible = false;
		}
		
		// This enables or disables all editing mode items and toolbar buttons
		private void UpdateToolbar()
		{
			preventupdateseperators = true;
			
			// Show/hide items based on preferences
			bool maploaded = (General.Map != null); //mxd
			buttonnewmap.Visible = General.Settings.ToolbarFile;
			buttonopenmap.Visible = General.Settings.ToolbarFile;
			buttonsavemap.Visible = General.Settings.ToolbarFile;
			buttonscripteditor.Visible = General.Settings.ToolbarScript && maploaded && General.Map.Config.HasScriptLumps(); // Only show script editor if there a script lumps defined
			buttonundo.Visible = General.Settings.ToolbarUndo && maploaded;
			buttonredo.Visible = General.Settings.ToolbarUndo && maploaded;
			buttoncut.Visible = General.Settings.ToolbarCopy && maploaded;
			buttoncopy.Visible = General.Settings.ToolbarCopy && maploaded;
			buttonpaste.Visible = General.Settings.ToolbarCopy && maploaded;
			buttoninsertprefabfile.Visible = General.Settings.ToolbarPrefabs && maploaded;
			buttoninsertpreviousprefab.Visible = General.Settings.ToolbarPrefabs && maploaded;
			buttonthingsfilter.Visible = General.Settings.ToolbarFilter && maploaded;
			thingfilters.Visible = General.Settings.ToolbarFilter && maploaded;
			separatorlinecolors.Visible = General.Settings.ToolbarFilter && maploaded; //mxd
			buttonlinededfcolors.Visible = General.Settings.ToolbarFilter && maploaded; //mxd
			linedefcolorpresets.Visible = General.Settings.ToolbarFilter && maploaded; //mxd
			separatorfilters.Visible = General.Settings.ToolbarViewModes && maploaded; //mxd
			buttonfullbrightness.Visible = General.Settings.ToolbarViewModes && maploaded; //mxd
			buttonfullbrightness.Checked = Renderer.FullBrightness; //mxd
			buttontogglegrid.Visible = General.Settings.ToolbarViewModes && maploaded; //mxd
			buttontogglegrid.Checked = General.Settings.RenderGrid; //mxd
			buttontogglecomments.Visible = General.Settings.ToolbarViewModes && maploaded && General.Map.UDMF; //mxd
			buttontogglecomments.Checked = General.Settings.RenderComments; //mxd
			buttontogglefixedthingsscale.Visible = General.Settings.ToolbarViewModes && maploaded; //mxd
			buttontogglefixedthingsscale.Checked = General.Settings.FixedThingsScale; //mxd
			separatorfullbrightness.Visible = General.Settings.ToolbarViewModes && maploaded; //mxd
			buttonviewbrightness.Visible = General.Settings.ToolbarViewModes && maploaded;
			buttonviewceilings.Visible = General.Settings.ToolbarViewModes && maploaded;
			buttonviewfloors.Visible = General.Settings.ToolbarViewModes && maploaded;
			buttonviewnormal.Visible = General.Settings.ToolbarViewModes && maploaded;
			separatorgeomergemodes.Visible = General.Settings.ToolbarGeometry && maploaded; //mxd
			buttonmergegeoclassic.Visible = General.Settings.ToolbarGeometry && maploaded; //mxd
			buttonmergegeo.Visible = General.Settings.ToolbarGeometry && maploaded; //mxd
			buttonplacegeo.Visible = General.Settings.ToolbarGeometry && maploaded; //mxd
			buttonsnaptogrid.Visible = General.Settings.ToolbarGeometry && maploaded;
			buttontoggledynamicgrid.Visible = General.Settings.ToolbarGeometry && maploaded; //mxd
			buttontoggledynamicgrid.Checked = General.Settings.DynamicGridSize; //mxd
			buttonautomerge.Visible = General.Settings.ToolbarGeometry && maploaded;
			buttonsplitjoinedsectors.Visible = General.Settings.ToolbarGeometry && maploaded; //mxd
			buttonsplitjoinedsectors.Checked = General.Settings.SplitJoinedSectors; //mxd
			buttonautoclearsidetextures.Visible = General.Settings.ToolbarGeometry && maploaded; //mxd
			buttontest.Visible = General.Settings.ToolbarTesting && maploaded;
			buttontoggleclassicrendering.Visible = General.Settings.ToolbarViewModes && maploaded;

			//mxd
			modelrendermode.Visible = General.Settings.GZToolbarGZDoom && maploaded;
			dynamiclightmode.Visible = General.Settings.GZToolbarGZDoom && maploaded;
			buttontogglefog.Visible = General.Settings.GZToolbarGZDoom && maploaded;
			buttontogglesky.Visible = General.Settings.GZToolbarGZDoom && maploaded;
			buttontoggleeventlines.Visible = General.Settings.GZToolbarGZDoom && maploaded;
			buttontogglevisualvertices.Visible = General.Settings.GZToolbarGZDoom && maploaded && General.Map.UDMF;
			separatorgzmodes.Visible = General.Settings.GZToolbarGZDoom && maploaded;

			//mxd. Show/hide additional panels
			modestoolbar.Visible = maploaded;
			panelinfo.Visible = maploaded;
			modecontrolsloolbar.Visible = (maploaded && modecontrolsloolbar.Items.Count > 0);
			
			//mxd. modestoolbar index in Controls gets messed up when it's invisible. This fixes it.
			//TODO: find out why this happens in the first place
			if(modestoolbar.Visible) 
			{
				int toolbarpos = this.Controls.IndexOf(toolbar);
				if(this.Controls.IndexOf(modestoolbar) > toolbarpos) 
				{
					this.Controls.SetChildIndex(modestoolbar, toolbarpos);
				}
			}

			// Update plugin buttons
			foreach(PluginToolbarButton p in pluginbuttons)
			{
				switch(p.section)
				{
					case ToolbarSection.File: p.button.Visible = General.Settings.ToolbarFile; break;
					case ToolbarSection.Script: p.button.Visible = General.Settings.ToolbarScript; break;
					case ToolbarSection.UndoRedo: p.button.Visible = General.Settings.ToolbarUndo; break;
					case ToolbarSection.CopyPaste: p.button.Visible = General.Settings.ToolbarCopy; break;
					case ToolbarSection.Prefabs: p.button.Visible = General.Settings.ToolbarPrefabs; break;
					case ToolbarSection.Things: p.button.Visible = General.Settings.ToolbarFilter; break;
					case ToolbarSection.Views: p.button.Visible = General.Settings.ToolbarViewModes; break;
					case ToolbarSection.Geometry: p.button.Visible = General.Settings.ToolbarGeometry; break;
					case ToolbarSection.Testing: p.button.Visible = General.Settings.ToolbarTesting; break;
				}
			}

			preventupdateseperators = false;

			UpdateSeparators();
		}

		// This checks one of the edit mode items (and unchecks all others)
		internal void CheckEditModeButton(string modeclassname)
		{
			// Go for all items
			//foreach(ToolStripItem item in editmodeitems)
			int itemCount = editmodeitems.Count;
			for(int i = 0; i < itemCount; i++)
			{
				ToolStripItem item = editmodeitems[i];
				// Check what type it is
				if(item is ToolStripMenuItem)
				{
					// Check if mode type matches with given name
					(item as ToolStripMenuItem).Checked = ((item.Tag as EditModeInfo).Type.Name == modeclassname);
				}
				else if(item is ToolStripButton)
				{
					// Check if mode type matches with given name
					(item as ToolStripButton).Checked = ((item.Tag as EditModeInfo).Type.Name == modeclassname);
				}
			}
		}
		
		// This removes the config-specific editing mode buttons
		internal void RemoveEditModeButtons()
		{
			// Go for all items
			//foreach(ToolStripItem item in editmodeitems)
			int itemCount = editmodeitems.Count;
			for (int i = 0; i < itemCount; i++)
			{
				ToolStripItem item = editmodeitems[i];
				// Remove it and restart
				menumode.DropDownItems.Remove(item);
				item.Dispose();
			}
			
			// Done
			modestoolbar.Items.Clear(); //mxd
			editmodeitems.Clear();
			UpdateSeparators();
		}
		
		// This adds an editing mode seperator on the toolbar and menu
		internal void AddEditModeSeperator(string group)
		{
			// Create a button
			ToolStripSeparator item = new ToolStripSeparator();
			item.Text = group; //mxd
			item.Margin = new Padding(0, 3, 0, 3); //mxd
			modestoolbar.Items.Add(item); //mxd
			editmodeitems.Add(item);
			
			// Create menu item
			int index = menumode.DropDownItems.Count;
			item = new ToolStripSeparator();
			item.Text = group; //mxd
			item.Margin = new Padding(0, 3, 0, 3);
			menumode.DropDownItems.Insert(index, item);
			editmodeitems.Add(item);
			
			UpdateSeparators();
		}
		
		// This adds an editing mode button to the toolbar and edit menu
		internal void AddEditModeButton(EditModeInfo modeinfo)
		{
			string controlname = modeinfo.ButtonDesc.Replace("&", "&&");
			
			// Create a button
			ToolStripItem item = new ToolStripButton(modeinfo.ButtonDesc, modeinfo.ButtonImage, EditModeButtonHandler);
			item.DisplayStyle = ToolStripItemDisplayStyle.Image;
			item.Padding = new Padding(0, 2, 0, 2);
			item.Margin = new Padding();
			item.Tag = modeinfo;
			modestoolbar.Items.Add(item); //mxd
			editmodeitems.Add(item);
			
			// Create menu item
			int index = menumode.DropDownItems.Count;
			item = new ToolStripMenuItem(controlname, modeinfo.ButtonImage, EditModeButtonHandler);
			item.Tag = modeinfo;
			menumode.DropDownItems.Insert(index, item);
			editmodeitems.Add(item);
			item.Visible = true;
			
			ApplyShortcutKeys(menumode.DropDownItems);
			UpdateSeparators();
		}

		// This handles edit mode button clicks
		private void EditModeButtonHandler(object sender, EventArgs e)
		{
			this.Update();
			EditModeInfo modeinfo = (EditModeInfo)((sender as ToolStripItem).Tag);
			General.Actions.InvokeAction(modeinfo.SwitchAction.GetFullActionName(modeinfo.Plugin.Assembly));
			this.Update();
		}

		//mxd
		public void UpdateGZDoomPanel() 
		{
			if(General.Map != null && General.Settings.GZToolbarGZDoom) 
			{
				foreach(ToolStripMenuItem item in modelrendermode.DropDownItems)
				{
					item.Checked = ((ModelRenderMode)item.Tag == General.Settings.GZDrawModelsMode);
					if(item.Checked) modelrendermode.Image = item.Image;
				}

				foreach(ToolStripMenuItem item in dynamiclightmode.DropDownItems)
				{
					item.Checked = ((LightRenderMode)item.Tag == General.Settings.GZDrawLightsMode);
					if(item.Checked) dynamiclightmode.Image = item.Image;
				}
				
				buttontogglefog.Checked = General.Settings.GZDrawFog;
				buttontogglesky.Checked = General.Settings.GZDrawSky;
				buttontoggleclassicrendering.Checked = General.Settings.ClassicRendering;
				buttontoggleeventlines.Checked = General.Settings.GZShowEventLines;
				buttontogglevisualvertices.Visible = General.Map.UDMF;
				buttontogglevisualvertices.Checked = General.Settings.GZShowVisualVertices;
			} 
		}

		/// <summary>
		/// Called when the test button drop down wants to close. Prevents closing when auto-launching is disabled
		/// </summary>
		/// <param name="sender">The sneder</param>
		/// <param name="e">The event</param>
		private void ButtonTestDropDown_Closing(object sender, ToolStripDropDownClosingEventArgs e)
		{
			if (General.Settings.AutoLaunchOnTest == false && e.CloseReason == ToolStripDropDownCloseReason.ItemClicked)
			{
				e.Cancel = true;
			}
		}

		#endregion

		#region ================== Toolbar context menu (mxd)

		private void toolbarContextMenu_Opening(object sender, CancelEventArgs e)
		{
			if(General.Map == null)
			{
				e.Cancel = true;
				return;
			}

			toggleFile.Image = General.Settings.ToolbarFile ? Resources.Check : null;
			toggleScript.Image = General.Settings.ToolbarScript ? Resources.Check : null;
			toggleUndo.Image = General.Settings.ToolbarUndo ? Resources.Check : null;
			toggleCopy.Image = General.Settings.ToolbarCopy ? Resources.Check : null;
			togglePrefabs.Image = General.Settings.ToolbarPrefabs ? Resources.Check : null;
			toggleFilter.Image = General.Settings.ToolbarFilter ? Resources.Check : null;
			toggleViewModes.Image = General.Settings.ToolbarViewModes ? Resources.Check : null;
			toggleGeometry.Image = General.Settings.ToolbarGeometry ? Resources.Check : null;
			toggleTesting.Image = General.Settings.ToolbarTesting ? Resources.Check : null;
			toggleRendering.Image = General.Settings.GZToolbarGZDoom ? Resources.Check : null;
		}

		private void toolbarContextMenu_Closing(object sender, ToolStripDropDownClosingEventArgs e) 
		{
			e.Cancel = (e.CloseReason == ToolStripDropDownCloseReason.ItemClicked && toolbarContextMenuShiftPressed);
		}

		private void toolbarContextMenu_KeyDown(object sender, KeyEventArgs e) 
		{
			toolbarContextMenuShiftPressed = (e.KeyCode == Keys.ShiftKey);
		}

		private void toolbarContextMenu_KeyUp(object sender, KeyEventArgs e) 
		{
			toolbarContextMenuShiftPressed = (e.KeyCode != Keys.ShiftKey);
		}

		private void toggleFile_Click(object sender, EventArgs e) 
		{
			General.Settings.ToolbarFile = !General.Settings.ToolbarFile;
			UpdateToolbar();

			if(toolbarContextMenuShiftPressed) 
				toggleFile.Image = General.Settings.ToolbarFile ? Resources.Check : null;
		}

		private void toggleScript_Click(object sender, EventArgs e) 
		{
			General.Settings.ToolbarScript = !General.Settings.ToolbarScript;
			UpdateToolbar();

			if(toolbarContextMenuShiftPressed) 
				toggleScript.Image = General.Settings.ToolbarScript ? Resources.Check : null;
		}

		private void toggleUndo_Click(object sender, EventArgs e) 
		{
			General.Settings.ToolbarUndo = !General.Settings.ToolbarUndo;
			UpdateToolbar();

			if(toolbarContextMenuShiftPressed) 
				toggleUndo.Image = General.Settings.ToolbarUndo ? Resources.Check : null;
		}

		private void toggleCopy_Click(object sender, EventArgs e) 
		{
			General.Settings.ToolbarCopy = !General.Settings.ToolbarCopy;
			UpdateToolbar();

			if(toolbarContextMenuShiftPressed) 
				toggleCopy.Image = General.Settings.ToolbarCopy ? Resources.Check : null;
		}

		private void togglePrefabs_Click(object sender, EventArgs e) 
		{
			General.Settings.ToolbarPrefabs = !General.Settings.ToolbarPrefabs;
			UpdateToolbar();

			if(toolbarContextMenuShiftPressed) 
				togglePrefabs.Image = General.Settings.ToolbarPrefabs ? Resources.Check : null;
		}

		private void toggleFilter_Click(object sender, EventArgs e) 
		{
			General.Settings.ToolbarFilter = !General.Settings.ToolbarFilter;
			UpdateToolbar();

			if(toolbarContextMenuShiftPressed) 
				toggleFilter.Image = General.Settings.ToolbarFilter ? Resources.Check : null;
		}

		private void toggleViewModes_Click(object sender, EventArgs e) 
		{
			General.Settings.ToolbarViewModes = !General.Settings.ToolbarViewModes;
			UpdateToolbar();

			if(toolbarContextMenuShiftPressed) 
				toggleViewModes.Image = General.Settings.ToolbarViewModes ? Resources.Check : null;
		}

		private void toggleGeometry_Click(object sender, EventArgs e) 
		{
			General.Settings.ToolbarGeometry = !General.Settings.ToolbarGeometry;
			UpdateToolbar();

			if(toolbarContextMenuShiftPressed) 
				toggleGeometry.Image = General.Settings.ToolbarGeometry ? Resources.Check : null;
		}

		private void toggleTesting_Click(object sender, EventArgs e) 
		{
			General.Settings.ToolbarTesting = !General.Settings.ToolbarTesting;
			UpdateToolbar();

			if(toolbarContextMenuShiftPressed) 
				toggleTesting.Image = General.Settings.ToolbarTesting ? Resources.Check : null;
		}

		private void toggleRendering_Click(object sender, EventArgs e) 
		{
			General.Settings.GZToolbarGZDoom = !General.Settings.GZToolbarGZDoom;
			UpdateToolbar();

			if(toolbarContextMenuShiftPressed) 
				toggleRendering.Image = General.Settings.GZToolbarGZDoom ? Resources.Check : null;
		}

		#endregion

		#region ================== Menus

		// This adds a menu to the menus bar
		public void AddMenu(ToolStripItem menu) { AddMenu(menu, MenuSection.Top, General.Plugins.FindPluginByAssembly(Assembly.GetCallingAssembly())); }
		public void AddMenu(ToolStripItem menu, MenuSection section) { AddMenu(menu, section, General.Plugins.FindPluginByAssembly(Assembly.GetCallingAssembly())); }
		private void AddMenu(ToolStripItem menu, MenuSection section, Plugin plugin)
		{
			// Fix tags to full action names
			ToolStripItemCollection items = new ToolStripItemCollection(this.menumain, new ToolStripItem[0]);
			items.Add(menu);
			RenameTagsToFullActions(items, plugin);
			
			// Insert the menu in the right location
			switch(section)
			{
				case MenuSection.FileNewOpenClose: menufile.DropDownItems.Insert(menufile.DropDownItems.IndexOf(seperatorfileopen), menu); break;
				case MenuSection.FileSave: menufile.DropDownItems.Insert(menufile.DropDownItems.IndexOf(seperatorfilesave), menu); break;
				case MenuSection.FileImport: itemimport.DropDownItems.Add(menu); break; //mxd
				case MenuSection.FileExport: itemexport.DropDownItems.Add(menu); break; //mxd
				case MenuSection.FileRecent: menufile.DropDownItems.Insert(menufile.DropDownItems.IndexOf(seperatorfilerecent), menu); break;
				case MenuSection.FileExit: menufile.DropDownItems.Insert(menufile.DropDownItems.IndexOf(itemexit), menu); break;
				case MenuSection.EditUndoRedo: menuedit.DropDownItems.Insert(menuedit.DropDownItems.IndexOf(seperatoreditundo), menu); break;
				case MenuSection.EditCopyPaste: menuedit.DropDownItems.Insert(menuedit.DropDownItems.IndexOf(seperatoreditcopypaste), menu); break;
				case MenuSection.EditGeometry: menuedit.DropDownItems.Insert(menuedit.DropDownItems.IndexOf(seperatoreditgeometry), menu); break;
				case MenuSection.EditGrid: menuedit.DropDownItems.Insert(menuedit.DropDownItems.IndexOf(seperatoreditgrid), menu); break;
				case MenuSection.EditMapOptions: menuedit.DropDownItems.Add(menu); break;
				case MenuSection.ViewHelpers: menuview.DropDownItems.Insert(menuview.DropDownItems.IndexOf(separatorhelpers), menu); break; //mxd
				case MenuSection.ViewRendering: menuview.DropDownItems.Insert(menuview.DropDownItems.IndexOf(separatorrendering), menu); break; //mxd
				case MenuSection.ViewThings: menuview.DropDownItems.Insert(menuview.DropDownItems.IndexOf(seperatorviewthings), menu); break;
				case MenuSection.ViewViews: menuview.DropDownItems.Insert(menuview.DropDownItems.IndexOf(seperatorviewviews), menu); break;
				case MenuSection.ViewZoom: menuview.DropDownItems.Insert(menuview.DropDownItems.IndexOf(seperatorviewzoom), menu); break;
				case MenuSection.ViewScriptEdit: menuview.DropDownItems.Add(menu); break;
				case MenuSection.PrefabsInsert: menuprefabs.DropDownItems.Insert(menuprefabs.DropDownItems.IndexOf(seperatorprefabsinsert), menu); break;
				case MenuSection.PrefabsCreate: menuprefabs.DropDownItems.Add(menu); break;
				case MenuSection.ToolsResources: menutools.DropDownItems.Insert(menutools.DropDownItems.IndexOf(seperatortoolsresources), menu); break;
				case MenuSection.ToolsConfiguration: menutools.DropDownItems.Insert(menutools.DropDownItems.IndexOf(seperatortoolsconfig), menu); break;
				case MenuSection.ToolsTesting: menutools.DropDownItems.Add(menu); break;
				case MenuSection.HelpManual: menuhelp.DropDownItems.Insert(menuhelp.DropDownItems.IndexOf(seperatorhelpmanual), menu); break;
				case MenuSection.HelpAbout: menuhelp.DropDownItems.Add(menu); break;
				case MenuSection.Top: menumain.Items.Insert(menumain.Items.IndexOf(menutools), menu); break;
			}
			
			ApplyShortcutKeys(items);
		}

		//mxd
		public void AddModesMenu(ToolStripItem menu, string group) 
		{
			// Fix tags to full action names
			ToolStripItemCollection items = new ToolStripItemCollection(this.menumain, new ToolStripItem[0]);
			items.Add(menu);
			RenameTagsToFullActions(items, General.Plugins.FindPluginByAssembly(Assembly.GetCallingAssembly()));
			
			//find the separator we need
			for(int i = 0; i < menumode.DropDownItems.Count; i++) 
			{
				if(menumode.DropDownItems[i] is ToolStripSeparator && menumode.DropDownItems[i].Text == group) 
				{
					menumode.DropDownItems.Insert(i + 1, menu);
					break;
				}
			}

			ApplyShortcutKeys(items);
		}
		
		// Removes a menu
		public void RemoveMenu(ToolStripItem menu)
		{
			// We actually have no idea in which menu this item is,
			// so try removing from all menus and the top strip
			menufile.DropDownItems.Remove(menu);
			menuedit.DropDownItems.Remove(menu);
			menumode.DropDownItems.Remove(menu); //mxd
			menuview.DropDownItems.Remove(menu);
			menuprefabs.DropDownItems.Remove(menu);
			menutools.DropDownItems.Remove(menu);
			menuhelp.DropDownItems.Remove(menu);
			menumain.Items.Remove(menu);
		}
		
		// Public method to apply shortcut keys
		internal void ApplyShortcutKeys()
		{
			// Apply shortcut keys to menus
			ApplyShortcutKeys(menumain.Items);
		}
		
		// This sets the shortcut keys on menu items
		private static void ApplyShortcutKeys(ToolStripItemCollection items)
		{
			// Go for all controls to find menu items
			foreach(ToolStripItem item in items)
			{
				// This is a menu item?
				if(item is ToolStripMenuItem)
				{
					// Get the item in proper type
					ToolStripMenuItem menuitem = (item as ToolStripMenuItem);

					// Tag set for this item?
					if(menuitem.Tag is string)
					{
						// Action with this name available?
						string actionname = menuitem.Tag.ToString();
						if(General.Actions.Exists(actionname))
						{
							// Put the action shortcut key on the menu item
							menuitem.ShortcutKeyDisplayString = Actions.Action.GetShortcutKeyDesc(General.Actions[actionname].ShortcutKey);
						}
					}
					// Edit mode info set for this item?
					else if(menuitem.Tag is EditModeInfo)
					{
						// Action with this name available?
						EditModeInfo modeinfo = (EditModeInfo)menuitem.Tag;
						string actionname = modeinfo.SwitchAction.GetFullActionName(modeinfo.Plugin.Assembly);
						if(General.Actions.Exists(actionname))
						{
							// Put the action shortcut key on the menu item
							menuitem.ShortcutKeyDisplayString = Actions.Action.GetShortcutKeyDesc(General.Actions[actionname].ShortcutKey);
						}
					}

					// Recursively apply shortcut keys to child menu items as well
					ApplyShortcutKeys(menuitem.DropDownItems);
				}
			}
		}

		// This fixes short action names to fully qualified
		// action names on menu item tags
		private static void RenameTagsToFullActions(ToolStripItemCollection items, Plugin plugin)
		{
			// Go for all controls to find menu items
			foreach(ToolStripItem item in items)
			{
				// Tag set for this item?
				if(item.Tag is string)
				{
					// Check if the tag does not already begin with the assembly name
					if(!((string)item.Tag).StartsWith(plugin.Name + "_", StringComparison.OrdinalIgnoreCase))
					{
						// Change the tag to a fully qualified action name
						item.Tag = plugin.Name.ToLowerInvariant() + "_" + (string)item.Tag;
					}
				}

				// This is a menu item?
				if(item is ToolStripMenuItem)
				{
					// Get the item in proper type
					ToolStripMenuItem menuitem = (item as ToolStripMenuItem);
					
					// Recursively perform operation on child menu items
					RenameTagsToFullActions(menuitem.DropDownItems, plugin);
				}
			}
		}
		
		#endregion

		#region ================== File Menu

		// This sets up the file menu
		private void UpdateFileMenu()
		{
			//mxd. Show/hide items
			bool show = (General.Map != null); //mxd
			itemclosemap.Visible = show;
			itemsavemap.Visible = show;
			itemsavemapas.Visible = show;
			itemsavemapinto.Visible = show;
			itemopenmapincurwad.Visible = show; //mxd
			itemimport.Visible = show; //mxd
			itemexport.Visible = show; //mxd
			seperatorfileopen.Visible = show; //mxd
			seperatorfilesave.Visible = show; //mxd

			// Toolbar icons
			buttonsavemap.Enabled = show;
		}

		// This sets the recent files from configuration
		private void CreateRecentFiles()
		{
			bool anyitems = false;

			// Where to insert
			int insertindex = menufile.DropDownItems.IndexOf(itemnorecent);
			
			// Create all items
			recentitems = new ToolStripMenuItem[General.Settings.MaxRecentFiles];
			for(int i = 0; i < General.Settings.MaxRecentFiles; i++)
			{
				// Create item
				recentitems[i] = new ToolStripMenuItem("");
				recentitems[i].Tag = "";
				recentitems[i].Click += recentitem_Click;
				menufile.DropDownItems.Insert(insertindex + i, recentitems[i]);

				// Get configuration setting
				string filename = General.Settings.ReadSetting("recentfiles.file" + i, "");
				if(!string.IsNullOrEmpty(filename) && File.Exists(filename))
				{
					// Set up item
					int number = i + 1;
					recentitems[i].Text = "&" + number + "  " + GetDisplayFilename(filename);
					recentitems[i].Tag = filename;
					recentitems[i].Visible = true;
					anyitems = true;
				}
				else
				{
					// Hide item
					recentitems[i].Visible = false;
				}
			}

			// Hide the no recent item when there are items
			itemnorecent.Visible = !anyitems;
		}
		
		// This saves the recent files list
		private void SaveRecentFiles()
		{
			// Go for all items
			for(int i = 0; i < recentitems.Length; i++)
			{
				// Recent file set?
				if(!string.IsNullOrEmpty(recentitems[i].Text))
				{
					// Save to configuration
					General.Settings.WriteSetting("recentfiles.file" + i, recentitems[i].Tag.ToString());
				}
			}

			// Save program configuration
			General.SaveSettings();
		}
		
		// This adds a recent file to the list
		internal void AddRecentFile(string filename)
		{
			//mxd. Recreate recent files list
			if(recentitems.Length != General.Settings.MaxRecentFiles)
			{
				UpdateRecentItems();
			}

			int movedownto = General.Settings.MaxRecentFiles - 1;
			
			// Check if this file is already in the list
			for(int i = 0; i < General.Settings.MaxRecentFiles; i++)
			{
				// File same as this item?
				if(string.Compare(filename, recentitems[i].Tag.ToString(), true) == 0)
				{
					// Move down to here so that this item will disappear
					movedownto = i;
					break;
				}
			}
			
			// Go for all items, except the last one, backwards
			for(int i = movedownto - 1; i >= 0; i--)
			{
				// Move recent file down the list
				int number = i + 2;
				recentitems[i + 1].Text = "&" + number + "  " + GetDisplayFilename(recentitems[i].Tag.ToString());
				recentitems[i + 1].Tag = recentitems[i].Tag.ToString();
				recentitems[i + 1].Visible = !string.IsNullOrEmpty(recentitems[i].Tag.ToString());
			}

			// Add new file at the top
			recentitems[0].Text = "&1  " + GetDisplayFilename(filename);
			recentitems[0].Tag = filename;
			recentitems[0].Visible = true;

			// Hide the no recent item
			itemnorecent.Visible = false;

			SaveRecentFiles();
		}

		//mxd
		private void UpdateRecentItems()
		{
			foreach(ToolStripMenuItem item in recentitems)
				menufile.DropDownItems.Remove(item);

			//SaveRecentFiles();
			CreateRecentFiles();
		}

		// This returns the trimmed file/path string
		private string GetDisplayFilename(string filename)
		{
			// String doesnt fit?
			if(MeasureString(filename, this.Font).Width > MAX_RECENT_FILES_PIXELS)
			{
				// Start chopping off characters
				for(int i = filename.Length - 6; i >= 0; i--)
				{
					// Does it fit now?
					string newname = filename.Substring(0, 3) + "..." + filename.Substring(filename.Length - i, i);
					if(MeasureString(newname, this.Font).Width <= MAX_RECENT_FILES_PIXELS) return newname;
				}

				// Cant find anything that fits (most unlikely!)
				return "wtf?!";
			}
			else
			{
				// The whole string fits
				return filename;
			}
		}
		
		// Exit clicked
		private void itemexit_Click(object sender, EventArgs e) { this.Close(); }

		// Recent item clicked
		private void recentitem_Click(object sender, EventArgs e)
		{
			// Get the item that was clicked
			ToolStripItem item = (sender as ToolStripItem);

			// Open this file
			General.OpenMapFile(item.Tag.ToString(), null);
		}

		//mxd
		private void menufile_DropDownOpening(object sender, EventArgs e)
		{
			UpdateRecentItems();
		}
		
		#endregion

		#region ================== Edit Menu

		// This sets up the edit menu
		private void UpdateEditMenu()
		{
			// No edit menu when no map open
			menuedit.Visible = (General.Map != null);
			
			// Enable/disable items
			itemundo.Enabled = (General.Map != null) && (General.Map.UndoRedo.NextUndo != null);
			itemredo.Enabled = (General.Map != null) && (General.Map.UndoRedo.NextRedo != null);
			itemcut.Enabled = (General.Map != null) && (General.Editing.Mode != null) && General.Editing.Mode.Attributes.AllowCopyPaste;
			itemcopy.Enabled = (General.Map != null) && (General.Editing.Mode != null) && General.Editing.Mode.Attributes.AllowCopyPaste;
			itempaste.Enabled = (General.Map != null) && (General.Editing.Mode != null) && General.Editing.Mode.Attributes.AllowCopyPaste;
			itempastespecial.Enabled = (General.Map != null) && (General.Editing.Mode != null) && General.Editing.Mode.Attributes.AllowCopyPaste;
			itemsplitjoinedsectors.Checked = General.Settings.SplitJoinedSectors; //mxd
			itemautoclearsidetextures.Checked = General.Settings.AutoClearSidedefTextures; //mxd
			itemdynamicgridsize.Enabled = (General.Map != null); //mxd
			itemdynamicgridsize.Checked = General.Settings.DynamicGridSize; //mxd

			// Determine undo description
			if(itemundo.Enabled)
				itemundo.Text = "Undo " + General.Map.UndoRedo.NextUndo.Description;
			else
				itemundo.Text = "Undo";

			// Determine redo description
			if(itemredo.Enabled)
				itemredo.Text = "Redo " + General.Map.UndoRedo.NextRedo.Description;
			else
				itemredo.Text = "Redo";
			
			// Toolbar icons
			buttonundo.Enabled = itemundo.Enabled;
			buttonredo.Enabled = itemredo.Enabled;
			buttonundo.ToolTipText = itemundo.Text;
			buttonredo.ToolTipText = itemredo.Text;
			buttonautoclearsidetextures.Checked = itemautoclearsidetextures.Checked; //mxd
			buttoncut.Enabled = itemcut.Enabled;
			buttoncopy.Enabled = itemcopy.Enabled;
			buttonpaste.Enabled = itempaste.Enabled;

			//mxd. Geometry merge mode items
			if(General.Map != null)
			{
				for(int i = 0; i < geomergemodesbuttons.Length; i++)
				{
					// Check the correct item
					geomergemodesbuttons[i].Checked = (i == (int)General.Settings.MergeGeometryMode);
					geomergemodesitems[i].Checked = (i == (int)General.Settings.MergeGeometryMode);
				}
			}
		}

		//mxd
		private void menuedit_DropDownOpening(object sender, EventArgs e) 
		{
			if(General.Map == null) 
			{
				selectGroup.Enabled = false;
				clearGroup.Enabled = false;
				addToGroup.Enabled = false;
				return;
			}

			//get data
			ToolStripItem item;
			GroupInfo[] infos = new GroupInfo[10];
			for(int i = 0; i < infos.Length; i++) infos[i] = General.Map.Map.GetGroupInfo(i);

			//update "Add to group" menu
			addToGroup.Enabled = true;
			addToGroup.DropDownItems.Clear();
			foreach(GroupInfo gi in infos) 
			{
				item = addToGroup.DropDownItems.Add(gi.ToString());
				item.Tag = "builder_assigngroup" + gi.Index;
				item.Click += InvokeTaggedAction;
			}

			//update "Select group" menu
			selectGroup.DropDownItems.Clear();
			foreach(GroupInfo gi in infos) 
			{
				if(gi.Empty) continue;
				item = selectGroup.DropDownItems.Add(gi.ToString());
				item.Tag = "builder_selectgroup" + gi.Index;
				item.Click += InvokeTaggedAction;
			}

			//update "Clear group" menu
			clearGroup.DropDownItems.Clear();
			foreach(GroupInfo gi in infos) 
			{
				if(gi.Empty) continue;
				item = clearGroup.DropDownItems.Add(gi.ToString());
				item.Tag = "builder_cleargroup" + gi.Index;
				item.Click += InvokeTaggedAction;
			}

			selectGroup.Enabled = selectGroup.DropDownItems.Count > 0;
			clearGroup.Enabled = clearGroup.DropDownItems.Count > 0;
		}

		//mxd. Action to toggle comments rendering
		[BeginAction("togglecomments")]
		internal void ToggleComments()
		{
			buttontogglecomments.Checked = !buttontogglecomments.Checked;
			itemtogglecomments.Checked = buttontogglecomments.Checked;
			General.Settings.RenderComments = buttontogglecomments.Checked;
			DisplayStatus(StatusType.Action, "Comment icons are " + (buttontogglecomments.Checked ? "SHOWN" : "HIDDEN"));

			// Redraw display to show changes
			RedrawDisplay();
		}

		//mxd. Action to toggle fixed things scale
		[BeginAction("togglefixedthingsscale")]
		internal void ToggleFixedThingsScale()
		{
			buttontogglefixedthingsscale.Checked = !buttontogglefixedthingsscale.Checked;
			itemtogglefixedthingsscale.Checked = buttontogglefixedthingsscale.Checked;
			General.Settings.FixedThingsScale = buttontogglefixedthingsscale.Checked;
			DisplayStatus(StatusType.Action, "Fixed things scale is " + (buttontogglefixedthingsscale.Checked ? "ENABLED" : "DISABLED"));

			// Redraw display to show changes
			RedrawDisplay();
		}
		
		//mxd. Action to toggle fixed things scale
		[BeginAction("togglealwaysshowvertices")]
		internal void ToggleAlwaysShowVertices()
		{
			General.Settings.AlwaysShowVertices = !General.Settings.AlwaysShowVertices;
			itemtogglealwaysshowvertices.Checked = General.Settings.AlwaysShowVertices;
			
			DisplayStatus(StatusType.Action, "Always show vertices is " + (General.Settings.AlwaysShowVertices ? "ENABLED" : "DISABLED"));

			// Redraw display to show changes
			RedrawDisplay();
		}

		// Action to toggle snap to grid
		[BeginAction("togglesnap")]
		internal void ToggleSnapToGrid()
		{
			buttonsnaptogrid.Checked = !buttonsnaptogrid.Checked;
			itemsnaptogrid.Checked = buttonsnaptogrid.Checked;
			DisplayStatus(StatusType.Action, "Snap to grid is " + (buttonsnaptogrid.Checked ? "ENABLED" : "DISABLED"));
		}

		// Action to toggle auto merge
		[BeginAction("toggleautomerge")]
		internal void ToggleAutoMerge()
		{
			buttonautomerge.Checked = !buttonautomerge.Checked;
			itemautomerge.Checked = buttonautomerge.Checked;
			DisplayStatus(StatusType.Action, "Snap to geometry is " + (buttonautomerge.Checked ? "ENABLED" : "DISABLED"));
		}

		//mxd
		[BeginAction("togglejoinedsectorssplitting")]
		internal void ToggleJoinedSectorsSplitting()
		{
			buttonsplitjoinedsectors.Checked = !buttonsplitjoinedsectors.Checked;
			itemsplitjoinedsectors.Checked = buttonsplitjoinedsectors.Checked;
			General.Settings.SplitJoinedSectors = buttonsplitjoinedsectors.Checked;
			DisplayStatus(StatusType.Action, "Joined sectors splitting is " + (General.Settings.SplitJoinedSectors ? "ENABLED" : "DISABLED"));
		}

		//mxd
		[BeginAction("togglebrightness")]
		internal void ToggleBrightness() 
		{
			Renderer.FullBrightness = !Renderer.FullBrightness;
			buttonfullbrightness.Checked = Renderer.FullBrightness;
			itemfullbrightness.Checked = Renderer.FullBrightness;
			General.Interface.DisplayStatus(StatusType.Action, "Full Brightness is now " + (Renderer.FullBrightness ? "ON" : "OFF"));

			// Redraw display to show changes
			General.Interface.RedrawDisplay();
		}

		//mxd
		[BeginAction("togglegrid")]
		protected void ToggleGrid()
		{
			General.Settings.RenderGrid = !General.Settings.RenderGrid;
			itemtogglegrid.Checked = General.Settings.RenderGrid;
			buttontogglegrid.Checked = General.Settings.RenderGrid;
			General.Interface.DisplayStatus(StatusType.Action, "Grid rendering is " + (General.Settings.RenderGrid ? "ENABLED" : "DISABLED"));

			// Redraw display to show changes
			General.Map.CRenderer2D.GridVisibilityChanged();
			General.Interface.RedrawDisplay();
		}
		
		[BeginAction("aligngridtolinedef")]
		protected void AlignGridToLinedef()
		{
			if (General.Map == null)
				return;

			if (General.Map.Map.SelectedLinedefsCount != 1)
			{
				General.Interface.DisplayStatus(StatusType.Warning, "Exactly one linedef must be selected");
				General.Interface.MessageBeep(MessageBeepType.Warning);
				return;
			}
			Linedef line = General.Map.Map.SelectedLinedefs.First.Value;
			Vertex vertex = line.Start;
			General.Map.Grid.SetGridRotation(line.Angle);
			General.Map.Grid.SetGridOrigin(vertex.Position.x, vertex.Position.y);
			General.Map.CRenderer2D.GridVisibilityChanged();
			General.Interface.RedrawDisplay();
		}

		[BeginAction("setgridorigintovertex")]
		protected void SetGridOriginToVertex()
		{
			if (General.Map == null)
				return;

			if (General.Map.Map.SelectedVerticessCount != 1)
			{
				General.Interface.DisplayStatus(StatusType.Warning, "Exactly one vertex must be selected");
				General.Interface.MessageBeep(MessageBeepType.Warning);
				return;
			}
			Vertex vertex = General.Map.Map.SelectedVertices.First.Value;
			General.Map.Grid.SetGridOrigin(vertex.Position.x, vertex.Position.y);
			General.Map.CRenderer2D.GridVisibilityChanged();
			General.Interface.RedrawDisplay();
		}

		[BeginAction("resetgrid")]
		protected void ResetGrid()
		{
			General.Map.Grid.SetGridRotation(0.0f);
			General.Map.Grid.SetGridOrigin(0, 0);
			General.Map.CRenderer2D.GridVisibilityChanged();
			General.Interface.RedrawDisplay();
		}

		//mxd
		[BeginAction("toggledynamicgrid")]
		protected void ToggleDynamicGrid()
		{
			General.Settings.DynamicGridSize = !General.Settings.DynamicGridSize;
			itemdynamicgridsize.Checked = General.Settings.DynamicGridSize;
			buttontoggledynamicgrid.Checked = General.Settings.DynamicGridSize;
			General.Interface.DisplayStatus(StatusType.Action, "Dynamic grid size is " + (General.Settings.DynamicGridSize ? "ENABLED" : "DISABLED"));

			// Redraw display to show changes
			if(General.Editing.Mode is ClassicMode) ((ClassicMode)General.Editing.Mode).MatchGridSizeToDisplayScale();
			General.Interface.RedrawDisplay();
		}

		//mxd
		[BeginAction("toggleautoclearsidetextures")]
		internal void ToggleAutoClearSideTextures() 
		{
			buttonautoclearsidetextures.Checked = !buttonautoclearsidetextures.Checked;
			itemautoclearsidetextures.Checked = buttonautoclearsidetextures.Checked;
			General.Settings.AutoClearSidedefTextures = buttonautoclearsidetextures.Checked;
			DisplayStatus(StatusType.Action, "Auto removal of unused sidedef textures is " + (buttonautoclearsidetextures.Checked ? "ENABLED" : "DISABLED"));
		}

		//mxd
		[BeginAction("viewusedtags")]
		internal void ViewUsedTags() 
		{
			TagStatisticsForm f = new TagStatisticsForm();
			f.ShowDialog(this);
		}

		//mxd
		[BeginAction("viewthingtypes")]
		internal void ViewThingTypes()
		{
			ThingStatisticsForm f = new ThingStatisticsForm();
			f.ShowDialog(this);
		}

		//mxd
		[BeginAction("geomergeclassic")]
		private void GeoMergeClassic()
		{
			General.Settings.MergeGeometryMode = MergeGeometryMode.CLASSIC;
			UpdateToolbar();
			UpdateEditMenu();
			DisplayStatus(StatusType.Action, "\"Merge Dragged Vertices Only\" mode selected");
		}

		//mxd
		[BeginAction("geomerge")]
		private void GeoMerge()
		{
			General.Settings.MergeGeometryMode = MergeGeometryMode.MERGE;
			UpdateToolbar();
			UpdateEditMenu();
			DisplayStatus(StatusType.Action, "\"Merge Dragged Geometry\" mode selected");
		}

		//mxd
		[BeginAction("georeplace")]
		private void GeoReplace()
		{
			General.Settings.MergeGeometryMode = MergeGeometryMode.REPLACE;
			UpdateToolbar();
			UpdateEditMenu();
			DisplayStatus(StatusType.Action, "\"Replace with Dragged Geometry\" mode selected");
		}
		
		#endregion

		#region ================== View Menu

		// This sets up the View menu
		private void UpdateViewMenu()
		{
			menuview.Visible = (General.Map != null); //mxd
			
			// Menu items
			itemfullbrightness.Checked = Renderer.FullBrightness; //mxd
			itemtogglegrid.Checked = General.Settings.RenderGrid; //mxd
			itemtoggleinfo.Checked = IsInfoPanelExpanded;
			itemtogglecomments.Visible = (General.Map != null && General.Map.UDMF); //mxd
			itemtogglecomments.Checked = General.Settings.RenderComments; //mxd
			itemtogglefixedthingsscale.Visible = (General.Map != null); //mxd
			itemtogglefixedthingsscale.Checked = General.Settings.FixedThingsScale; //mxd
			itemtogglefog.Checked = General.Settings.GZDrawFog;
			itemtogglesky.Checked = General.Settings.GZDrawSky;
			itemtoggleclassicrendering.Checked = General.Settings.ClassicRendering;
			itemtoggleeventlines.Checked = General.Settings.GZShowEventLines;
			itemtogglevisualverts.Visible = (General.Map != null && General.Map.UDMF);
			itemtogglevisualverts.Checked = General.Settings.GZShowVisualVertices;

			// Update Model Rendering Mode items...
			foreach(ToolStripMenuItem item in itemmodelmodes.DropDownItems)
			{
				item.Checked = ((ModelRenderMode)item.Tag == General.Settings.GZDrawModelsMode);
				if(item.Checked) itemmodelmodes.Image = item.Image;
			}

			// Update Dynamic Light Mode items...
			foreach(ToolStripMenuItem item in itemdynlightmodes.DropDownItems)
			{
				item.Checked = ((LightRenderMode)item.Tag == General.Settings.GZDrawLightsMode);
				if(item.Checked) itemdynlightmodes.Image = item.Image;
			}
			
			// View mode items
			if(General.Map != null)
			{
				for(int i = 0; i < Renderer2D.NUM_VIEW_MODES; i++)
				{
					// Check the correct item
					viewmodesbuttons[i].Checked = (i == (int)General.Map.CRenderer2D.ViewMode);
					viewmodesitems[i].Checked = (i == (int)General.Map.CRenderer2D.ViewMode);
				}
			}
		}

		//mxd
		[BeginAction("gztoggleenhancedrendering")]
		public void ToggleEnhancedRendering()
		{
			General.Settings.EnhancedRenderingEffects = !General.Settings.EnhancedRenderingEffects;

			General.Settings.GZDrawFog = General.Settings.EnhancedRenderingEffects;
			General.Settings.GZDrawSky = General.Settings.EnhancedRenderingEffects;
			General.Settings.GZDrawLightsMode = (General.Settings.EnhancedRenderingEffects ? LightRenderMode.ALL : LightRenderMode.NONE);
			General.Settings.GZDrawModelsMode = (General.Settings.EnhancedRenderingEffects ? ModelRenderMode.ALL : ModelRenderMode.NONE);

			UpdateGZDoomPanel();
			UpdateViewMenu();
			DisplayStatus(StatusType.Info, "Enhanced rendering effects are " + (General.Settings.EnhancedRenderingEffects ? "ENABLED" : "DISABLED"));
		}

		//mxd
		[BeginAction("gztogglefog")]
		internal void ToggleFog()
		{
			General.Settings.GZDrawFog = !General.Settings.GZDrawFog;

			itemtogglefog.Checked = General.Settings.GZDrawFog;
			buttontogglefog.Checked = General.Settings.GZDrawFog;

			General.MainWindow.DisplayStatus(StatusType.Action, "Fog rendering is " + (General.Settings.GZDrawFog ? "ENABLED" : "DISABLED"));
			General.MainWindow.RedrawDisplay();
			General.MainWindow.UpdateGZDoomPanel();
		}

		//mxd
		[BeginAction("gztogglesky")]
		internal void ToggleSky()
		{
			General.Settings.GZDrawSky = !General.Settings.GZDrawSky;

			itemtogglesky.Checked = General.Settings.GZDrawSky;
			buttontogglesky.Checked = General.Settings.GZDrawSky;

			General.MainWindow.DisplayStatus(StatusType.Action, "Sky rendering is " + (General.Settings.GZDrawSky ? "ENABLED" : "DISABLED"));
			General.MainWindow.RedrawDisplay();
			General.MainWindow.UpdateGZDoomPanel();
		}
		
		//mxd
		[BeginAction("toggleclassicrendering")]
		internal void ToggleClassicRendering()
		{
			General.Settings.ClassicRendering = !General.Settings.ClassicRendering;

			itemtoggleclassicrendering.Checked = General.Settings.ClassicRendering;
			buttontoggleclassicrendering.Checked = General.Settings.ClassicRendering;

			General.MainWindow.DisplayStatus(StatusType.Action, "Classic rendering is " + (General.Settings.ClassicRendering ? "ENABLED" : "DISABLED"));
			General.MainWindow.RedrawDisplay();
			General.MainWindow.UpdateGZDoomPanel();
		}

		[BeginAction("gztoggleeventlines")]
		internal void ToggleEventLines()
		{
			General.Settings.GZShowEventLines = !General.Settings.GZShowEventLines;

			itemtoggleeventlines.Checked = General.Settings.GZShowEventLines;
			buttontoggleeventlines.Checked = General.Settings.GZShowEventLines;

			General.MainWindow.DisplayStatus(StatusType.Action, "Event lines are " + (General.Settings.GZShowEventLines ? "ENABLED" : "DISABLED"));
			General.MainWindow.RedrawDisplay();
			General.MainWindow.UpdateGZDoomPanel();
		}

		[BeginAction("gztogglevisualvertices")]
		internal void ToggleVisualVertices()
		{
			General.Settings.GZShowVisualVertices = !General.Settings.GZShowVisualVertices;

			itemtogglevisualverts.Checked = General.Settings.GZShowVisualVertices;
			buttontogglevisualvertices.Checked = General.Settings.GZShowVisualVertices;

			General.MainWindow.DisplayStatus(StatusType.Action, "Visual vertices are " + (General.Settings.GZShowVisualVertices ? "ENABLED" : "DISABLED"));
			General.MainWindow.RedrawDisplay();
			General.MainWindow.UpdateGZDoomPanel();
		}

		#endregion

		#region ================== Mode Menu

		// This sets up the modes menu
		private void UpdateModeMenu()
		{
			menumode.Visible = (General.Map != null);
		}
		
		#endregion

		#region ================== Help Menu
		
		// This sets up the help menu
		private void UpdateHelpMenu()
		{
			itemhelpeditmode.Visible = (General.Map != null); //mxd
			itemhelpeditmode.Enabled = (General.Map != null && General.Editing.Mode != null);
		}

		//mxd. Check updates clicked
		private void itemhelpcheckupdates_Click(object sender, EventArgs e)
		{
            UpdateChecker.PerformCheck(true);
		}

		//mxd. Github issues clicked
		private void itemhelpissues_Click(object sender, EventArgs e)
		{
			General.OpenWebsite("https://github.com/jewalky/GZDoom-Builder-Bugfix/issues");
		}
		
		// About clicked
		private void itemhelpabout_Click(object sender, EventArgs e)
		{
			// Show about dialog
			AboutForm aboutform = new AboutForm();
			aboutform.ShowDialog(this);
		}

		// Reference Manual clicked
		private void itemhelprefmanual_Click(object sender, EventArgs e)
		{
			General.ShowHelp("introduction.html");
		}

		// About this Editing Mode
		private void itemhelpeditmode_Click(object sender, EventArgs e)
		{
			if((General.Map != null) && (General.Editing.Mode != null))
				General.Editing.Mode.OnHelp();
		}

		//mxd
		private void itemShortcutReference_Click(object sender, EventArgs e) 
		{
			const string columnLabels = "<tr><td width=\"240px;\"><strong>Action</strong></td><td width=\"120px;\"><div align=\"center\"><strong>Shortcut</strong></div></td><td width=\"120px;\"><div align=\"center\"><strong>Modifiers</strong></div></td><td><strong>Description</strong></td></tr>";
			const string categoryPadding = "<tr><td colspan=\"4\"></td></tr>";
			const string categoryStart = "<tr><td colspan=\"4\" bgcolor=\"#333333\"><strong style=\"color:#FFFFFF\">";
			const string categoryEnd = "</strong><div style=\"text-align:right; float:right\"><a style=\"color:#FFFFFF\" href=\"#top\">[to top]</a></div></td></tr>";
			const string fileName = "GZDB Actions Reference.html";

			Actions.Action[] actions = General.Actions.GetAllActions();
			Dictionary<string, List<Actions.Action>> sortedActions = new Dictionary<string, List<Actions.Action>>(StringComparer.Ordinal);

			foreach(Actions.Action action in actions) 
			{
				if(!sortedActions.ContainsKey(action.Category))
					sortedActions.Add(action.Category, new List<Actions.Action>());
				sortedActions[action.Category].Add(action);
			}

			System.Text.StringBuilder html = new System.Text.StringBuilder();

			//head
			html.AppendLine("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">" + Environment.NewLine +
								"<html xmlns=\"http://www.w3.org/1999/xhtml\">" + Environment.NewLine +
								"<head><meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\" /><title>GZDoom Builder Actions Reference</title></head>" + Environment.NewLine +
								"<body bgcolor=\"#666666\">" + Environment.NewLine +
									"<div style=\"padding-left:60px; padding-right:60px; padding-top:20px; padding-bottom:20px;\">" + Environment.NewLine);

			//table header
			html.AppendLine("<table bgcolor=\"#FFFFFF\" width=\"100%\" border=\"0\" cellspacing=\"6\" cellpadding=\"6\" style=\"font-family: 'Trebuchet MS',georgia,Verdana,Sans-serif;\">" + Environment.NewLine +
							"<tr><td colspan=\"4\" bgcolor=\"#333333\"><span style=\"font-size: 24px\"><a name=\"top\" id=\"top\"></a><strong style=\"color:#FFFFFF\">GZDoom Builder Actions Reference</strong></span></td></tr>");

			//categories navigator
			List<string> catnames = new List<string>(sortedActions.Count);
			int counter = 0;
			int numActions = 0;
			foreach(KeyValuePair<string, List<Actions.Action>> category in sortedActions) 
			{
				catnames.Add("<a href=\"#cat" + (counter++) + "\">" + General.Actions.Categories[category.Key] + "</a>");
				numActions += category.Value.Count;
			}

			html.AppendLine("<tr><td colspan=\"4\"><strong>Total number of actions:</strong> " + numActions + "<br/><strong>Jump to:</strong> ");
			html.AppendLine(string.Join(" | ", catnames.ToArray()));
			html.AppendLine("</td></tr>" + Environment.NewLine);

			//add descriptions
			counter = 0;
			foreach(KeyValuePair<string, List<Actions.Action>> category in sortedActions) 
			{
				//add category title
				html.AppendLine(categoryPadding);
				html.AppendLine(categoryStart + "<a name=\"cat" + counter + "\" id=\"cat" + counter + "\"></a>" + General.Actions.Categories[category.Key] + categoryEnd);
				html.AppendLine(columnLabels);
				counter++;

				Dictionary<string, Actions.Action> actionsByTitle = new Dictionary<string, Actions.Action>(StringComparer.Ordinal);
				List<string> actionTitles = new List<string>();

				foreach(Actions.Action action in category.Value) 
				{
					actionsByTitle.Add(action.Title, action);
					actionTitles.Add(action.Title);
				}

				actionTitles.Sort();

				foreach(string title in actionTitles) 
				{
					Actions.Action a = actionsByTitle[title];
					List<string> modifiers = new List<string>();

					html.AppendLine("<tr>");
					html.AppendLine("<td>" + title + "</td>");
					html.AppendLine("<td><div align=\"center\">" + Actions.Action.GetShortcutKeyDesc(a.ShortcutKey) + "</div></td>");

					if(a.DisregardControl) modifiers.Add("Ctrl");
					if(a.DisregardAlt) modifiers.Add("Alt");
					if(a.DisregardShift) modifiers.Add("Shift");

					html.AppendLine("<td><div align=\"center\">" + string.Join(", ", modifiers.ToArray()) + "</div></td>");
					html.AppendLine("<td>" + a.Description + "</td>");
					html.AppendLine("</tr>");
				}
			}

			//add bottom
			html.AppendLine("</table></div></body></html>");

			//write
			string path;
			try 
			{
				path = Path.Combine(General.AppPath, fileName);
				using(StreamWriter writer = File.CreateText(path)) 
				{
					writer.Write(html.ToString());
				}
			} 
			catch(Exception) 
			{
				//Configurtions path SHOULD be accessible and not read-only, right?
				path = Path.Combine(General.SettingsPath, fileName);
				using(StreamWriter writer = File.CreateText(path)) 
				{
					writer.Write(html.ToString());
				}
			}

			//open file
			DisplayStatus(StatusType.Info, "Shortcut reference saved to \"" + path + "\"");
			Process.Start(path);
		}

		//mxd
		private void itemopenconfigfolder_Click(object sender, EventArgs e)
		{
			if(Directory.Exists(General.SettingsPath)) Process.Start(General.SettingsPath);
			else General.ShowErrorMessage("Huh? Where did Settings folder go?.." + Environment.NewLine
				+ "I swear it was here: \"" + General.SettingsPath + "\"!", MessageBoxButtons.OK); // I don't think this will ever happen
		}
		
		#endregion

		#region ================== Prefabs Menu

		// This sets up the prefabs menu
		private void UpdatePrefabsMenu()
		{
			menuprefabs.Visible = (General.Map != null); //mxd
			
			// Enable/disable items
			itemcreateprefab.Enabled = (General.Map != null) && (General.Editing.Mode != null) && General.Editing.Mode.Attributes.AllowCopyPaste;
			iteminsertprefabfile.Enabled = (General.Map != null) && (General.Editing.Mode != null) && General.Editing.Mode.Attributes.AllowCopyPaste;
			iteminsertpreviousprefab.Enabled = (General.Map != null) && (General.Editing.Mode != null) && General.Map.CopyPaste.IsPreviousPrefabAvailable && General.Editing.Mode.Attributes.AllowCopyPaste;
			
			// Toolbar icons
			buttoninsertprefabfile.Enabled = iteminsertprefabfile.Enabled;
			buttoninsertpreviousprefab.Enabled = iteminsertpreviousprefab.Enabled;
		}
		
		#endregion
		
		#region ================== Tools Menu

		// This sets up the tools menu
		private void UpdateToolsMenu()
		{
			//mxd. Enable/disable items
			bool enabled = (General.Map != null);
			itemreloadresources.Visible = enabled;
			seperatortoolsconfig.Visible = enabled;
			itemsavescreenshot.Visible = enabled;
			itemsaveeditareascreenshot.Visible = enabled;
			separatortoolsscreenshots.Visible = enabled;
			itemtestmap.Visible = enabled;

			bool supported = (enabled && !string.IsNullOrEmpty(General.Map.Config.DecorateGames));
			itemReloadGldefs.Visible = supported;
			itemReloadModedef.Visible = supported;
		}
		
		// Errors and Warnings
		[BeginAction("showerrors")]
		internal void ShowErrors()
		{
			ErrorsForm errform = new ErrorsForm();
			errform.ShowDialog(this);
			errform.Dispose();
			//mxd
			SetWarningsCount(General.ErrorLogger.ErrorsCount, false);
		}
		
		// Game Configuration action
		[BeginAction("configuration")]
		internal void ShowConfiguration()
		{
			// Show configuration dialog
			ShowConfigurationPage(-1);
		}

		// This shows the configuration on a specific page
		internal void ShowConfigurationPage(int pageindex)
		{
			// Show configuration dialog
			ConfigForm cfgform = new ConfigForm();
			if(pageindex > -1) cfgform.ShowTab(pageindex);
			if(cfgform.ShowDialog(this) == DialogResult.OK)
			{
				// Update stuff
				SetupInterface();
				UpdateInterface();
				General.Editing.UpdateCurrentEditModes();
				General.Plugins.ProgramReconfigure();

				// Save program configuration
				General.SaveSettings();

				// Reload resources if a map is open
				if ((General.Map != null) && cfgform.ReloadResources) General.Actions.InvokeAction("builder_reloadresources");
				
				// Redraw display
				RedrawDisplay();
			}

			// Done
			cfgform.Dispose();
		}

		// Preferences action
		[BeginAction("preferences")]
		internal void ShowPreferences()
		{
			// Show preferences dialog
			PreferencesForm prefform = new PreferencesForm();
			if(prefform.ShowDialog(this) == DialogResult.OK)
			{
				// Update stuff
				SetupInterface();
				UpdateInterface();
				ApplyShortcutKeys();
				General.Colors.CreateCorrectionTable();
				General.Plugins.ProgramReconfigure();

				// Save program configuration
				General.SaveSettings();
				
				// Map opened?
				if(General.Map != null)
				{
					// Reload resources!
					if(General.Map.ScriptEditor != null) General.Map.ScriptEditor.Editor.RefreshSettings();
					General.Map.Graphics.SetupSettings();
					General.Map.UpdateConfiguration();
					if(prefform.ReloadResources) General.Actions.InvokeAction("builder_reloadresources");
				}
				
				// Redraw display
				RedrawDisplay();
			}

			// Done
			prefform.Dispose();
		}

		//mxd
		internal void SaveScreenshot(bool activeControlOnly) 
		{
			//pick a valid folder
			string folder = General.Settings.ScreenshotsPath;
			if(!Directory.Exists(folder)) 
			{
				if(folder != General.DefaultScreenshotsPath
					&& General.ShowErrorMessage("Screenshots save path \"" + folder
					+ "\" does not exist!\nPress OK to save to the default folder (\"" 
					+ General.DefaultScreenshotsPath
					+ "\").\nPress Cancel to abort.", MessageBoxButtons.OKCancel) == DialogResult.Cancel) return;


				folder = General.DefaultScreenshotsPath;
				if(!Directory.Exists(folder)) Directory.CreateDirectory(folder);
			}

			// Create name and bounds
			string name;
			Rectangle bounds;
			bool displayextrainfo = false;
			string mapname = (General.Map != null ? Path.GetFileNameWithoutExtension(General.Map.FileTitle) : General.ThisAssembly.GetName().Name);

			if(activeControlOnly)
			{
				if(Form.ActiveForm != null && Form.ActiveForm != this)
				{
					name = mapname + " (" + Form.ActiveForm.Text + ") at ";
					bounds = (Form.ActiveForm.WindowState == FormWindowState.Maximized ? 
						Screen.GetWorkingArea(Form.ActiveForm) : 
						Form.ActiveForm.Bounds);
				}
				else
				{
					name = mapname + " (edit area) at ";
					bounds = this.display.Bounds;
					bounds.Offset(this.PointToScreen(new Point()));
					displayextrainfo = true;
				}
			} 
			else
			{
				name = mapname + " at ";
				bounds = (this.WindowState == FormWindowState.Maximized ? Screen.GetWorkingArea(this) : this.Bounds);
			}

			Point cursorLocation = Point.Empty;
			//dont want to render the cursor in VisualMode
			if(General.Editing.Mode == null || !(General.Editing.Mode is VisualMode))
				cursorLocation = Cursor.Position - new Size(bounds.Location);

			//create path
			string date = DateTime.Now.ToString("yyyy.MM.dd HH-mm-ss.fff");
			string revision = (General.DebugBuild ? "DEVBUILD" : "R" + General.ThisAssembly.GetName().Version.MinorRevision);
			string path = Path.Combine(folder, name + date + " [" + revision + "].jpg");

			//save image
			using(Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height)) 
			{
				using(Graphics g = Graphics.FromImage(bitmap)) 
				{
					g.CopyFromScreen(new Point(bounds.Left, bounds.Top), Point.Empty, bounds.Size);

					//draw the cursor
					if(!cursorLocation.IsEmpty) g.DrawImage(Resources.Cursor, cursorLocation);

					//gather some info
					string info;
					if(displayextrainfo && General.Editing.Mode != null) 
					{
						info = General.Map.FileTitle + " | " + General.Map.Options.CurrentName + " | ";

						//get map coordinates
						if(General.Editing.Mode is ClassicMode) 
						{
							Vector2D pos = ((ClassicMode) General.Editing.Mode).MouseMapPos;

							//mouse inside the view?
							if(pos.IsFinite()) 
							{
								info += "X:" + Math.Round(pos.x) + " Y:" + Math.Round(pos.y);
							} 
							else 
							{
								info += "X:" + Math.Round(General.Map.Renderer2D.TranslateX) + " Y:" + Math.Round(General.Map.Renderer2D.TranslateY);
							}
						} 
						else 
						{ //should be visual mode
							info += "X:" + Math.Round(General.Map.VisualCamera.Position.x) + " Y:" + Math.Round(General.Map.VisualCamera.Position.y) + " Z:" + Math.Round(General.Map.VisualCamera.Position.z);
						}

						//add the revision number
						info += " | " + revision;
					} 
					else 
					{
						//just use the revision number
						info = revision;
					}

					//draw info
					Font font = new Font("Tahoma", 10);
					SizeF rect = g.MeasureString(info, font);
					float px = bounds.Width - rect.Width - 4;
					float py = 4;

					g.FillRectangle(Brushes.Black, px, py, rect.Width, rect.Height + 3);
					using(SolidBrush brush = new SolidBrush(Color.White))
					{
						g.DrawString(info, font, brush, px + 2, py + 2);
					}
				}

				try 
				{
					ImageCodecInfo jpegCodec = null;
					ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
					foreach(ImageCodecInfo codec in codecs) 
					{
						if(codec.FormatID == ImageFormat.Jpeg.Guid) 
						{
							jpegCodec = codec;
							break;
						}
					}

					EncoderParameter qualityParam = new EncoderParameter(Encoder.Quality, 90L);
					EncoderParameters encoderParams = new EncoderParameters(1);
					encoderParams.Param[0] = qualityParam;

					bitmap.Save(path, jpegCodec, encoderParams);
					DisplayStatus(StatusType.Info, "Screenshot saved to \"" + path + "\"");
				} 
				catch(ExternalException e) 
				{
					DisplayStatus(StatusType.Warning, "Failed to save screenshot...");
					General.ErrorLogger.Add(ErrorType.Error, "Failed to save screenshot: " + e.Message);
				}
			}
		}
		
		#endregion

		#region ================== Models and Lights mode (mxd)

		private void ChangeModelRenderingMode(object sender, EventArgs e)
		{
			General.Settings.GZDrawModelsMode = (ModelRenderMode)((ToolStripMenuItem)sender).Tag;

			switch(General.Settings.GZDrawModelsMode) 
			{
				case ModelRenderMode.NONE:
					General.MainWindow.DisplayStatus(StatusType.Action, "Models rendering mode: NONE");
					break;

				case ModelRenderMode.SELECTION:
					General.MainWindow.DisplayStatus(StatusType.Action, "Models rendering mode: SELECTION ONLY");
					break;

				case ModelRenderMode.ACTIVE_THINGS_FILTER:
					General.MainWindow.DisplayStatus(StatusType.Action, "Models rendering mode: ACTIVE THINGS FILTER ONLY");
					break;

				case ModelRenderMode.ALL:
					General.MainWindow.DisplayStatus(StatusType.Action, "Models rendering mode: ALL");
					break;
			}

			UpdateViewMenu();
			UpdateGZDoomPanel();
			RedrawDisplay();
		}

		private void ChangeLightRenderingMode(object sender, EventArgs e) 
		{
			General.Settings.GZDrawLightsMode = (LightRenderMode)((ToolStripMenuItem)sender).Tag;

			switch(General.Settings.GZDrawLightsMode) 
			{
				case LightRenderMode.NONE:
					General.MainWindow.DisplayStatus(StatusType.Action, "Dynamic lights rendering mode: NONE");
					break;

				case LightRenderMode.ALL:
					General.MainWindow.DisplayStatus(StatusType.Action, "Models rendering mode: ALL");
					break;

				case LightRenderMode.ALL_ANIMATED:
					General.MainWindow.DisplayStatus(StatusType.Action, "Models rendering mode: ANIMATED");
					break;
			}

			UpdateViewMenu();
			UpdateGZDoomPanel();
			RedrawDisplay();
		}


		#endregion

		#region ================== Info Panels

		// This toggles the panel expanded / collapsed
		[BeginAction("toggleinfopanel")]
		internal void ToggleInfoPanel()
		{
			if(IsInfoPanelExpanded)
			{
				panelinfo.Height = buttontoggleinfo.Height + buttontoggleinfo.Top;
				buttontoggleinfo.Image = Resources.InfoPanelExpand; //mxd
				if(linedefinfo.Visible) linedefinfo.Hide();
				if(vertexinfo.Visible) vertexinfo.Hide();
				if(sectorinfo.Visible) sectorinfo.Hide();
				if(thinginfo.Visible) thinginfo.Hide();
				modename.Visible = false;
#if DEBUG
				console.Visible = false; //mxd
#endif
				statistics.Visible = false; //mxd
				labelcollapsedinfo.Visible = true;
				itemtoggleinfo.Checked = false;
			}
			else
			{
				panelinfo.Height = heightpanel1.Height;
				buttontoggleinfo.Image = Resources.InfoPanelCollapse; //mxd
				labelcollapsedinfo.Visible = false;
				itemtoggleinfo.Checked = true;
				if(lastinfoobject is Vertex) ShowVertexInfo((Vertex)lastinfoobject);
				else if(lastinfoobject is Linedef) ShowLinedefInfo((Linedef)lastinfoobject);
				else if(lastinfoobject is Sector) ShowSectorInfo((Sector)lastinfoobject);
				else if(lastinfoobject is Thing) ShowThingInfo((Thing)lastinfoobject);
				else HideInfo();
			}

			dockerspanel.Height = dockersspace.Height; //mxd
			FocusDisplay();
		}

		// Mouse released on info panel toggle button
		private void buttontoggleinfo_MouseUp(object sender, MouseEventArgs e)
		{
			dockerspanel.Height = dockersspace.Height; //mxd
			FocusDisplay();
		}
		
		// This displays the current mode name
		internal void DisplayModeName(string name)
		{
			if(lastinfoobject == null) 
			{
				labelcollapsedinfo.Text = name;
				labelcollapsedinfo.Refresh();
			}
			modename.Text = name;
			modename.Refresh();
		}
		
		// This hides all info panels
		public void HideInfo()
		{
			// Hide them all
			// [ZZ]
			panelinfo.SuspendLayout();
			bool showModeName = ((General.Map != null) && IsInfoPanelExpanded); //mxd
			lastinfoobject = null;
			if(linedefinfo.Visible) linedefinfo.Hide();
			if(vertexinfo.Visible) vertexinfo.Hide();
			if(sectorinfo.Visible) sectorinfo.Hide();
			if(thinginfo.Visible) thinginfo.Hide();
			labelcollapsedinfo.Text = modename.Text;
			labelcollapsedinfo.Refresh();
#if DEBUG
			console.Visible = true;
#else
			modename.Visible = showModeName;
#endif
			modename.Refresh();
			statistics.Visible = showModeName; //mxd

			//mxd. Let the plugins know
			General.Plugins.OnHighlightLost();
			// [ZZ]
			panelinfo.ResumeLayout();
		}
		
		// This refreshes info
		public void RefreshInfo()
		{
			if(lastinfoobject is Vertex) ShowVertexInfo((Vertex)lastinfoobject);
			else if(lastinfoobject is Linedef) ShowLinedefInfo((Linedef)lastinfoobject);
			else if(lastinfoobject is Sector) ShowSectorInfo((Sector)lastinfoobject);
			else if(lastinfoobject is Thing) ShowThingInfo((Thing)lastinfoobject);

			//mxd. Let the plugins know
			// [ZZ]
			panelinfo.SuspendLayout();
			General.Plugins.OnHighlightRefreshed(lastinfoobject);
			panelinfo.ResumeLayout();
		}

		//mxd
		public void ShowHints(string hintsText) 
		{
			if(!string.IsNullOrEmpty(hintsText)) 
			{
				hintsPanel.SetHints(hintsText);
			} 
			else 
			{
				ClearHints();
			}
		}

		//mxd
		public void ClearHints() 
		{
			hintsPanel.ClearHints();
		}

		//mxd
		internal void AddHintsDocker() 
		{
			if(!dockerspanel.Contains(hintsDocker)) dockerspanel.Add(hintsDocker, false);
		}

		//mxd
		internal void RemoveHintsDocker() 
		{
			dockerspanel.Remove(hintsDocker);
		}

		//mxd. Show linedef info
		public void ShowLinedefInfo(Linedef l) 
		{
			ShowLinedefInfo(l, null);
		}
		
		//mxd. Show linedef info and highlight given sidedef
		public void ShowLinedefInfo(Linedef l, Sidedef highlightside)
		{
			if(l.IsDisposed)
			{
				HideInfo();
				return;
			}

			// [ZZ]
			panelinfo.SuspendLayout();
			lastinfoobject = l;
			modename.Visible = false;
#if DEBUG
			console.Visible = console.AlwaysOnTop; //mxd
#endif
			statistics.Visible = false; //mxd
			if(vertexinfo.Visible) vertexinfo.Hide();
			if(sectorinfo.Visible) sectorinfo.Hide();
			if(thinginfo.Visible) thinginfo.Hide();
			if(IsInfoPanelExpanded) linedefinfo.ShowInfo(l, highlightside);

			// Show info on collapsed label
			if(General.Map.Config.LinedefActions.ContainsKey(l.Action)) 
			{
				LinedefActionInfo act = General.Map.Config.LinedefActions[l.Action];
				labelcollapsedinfo.Text = act.ToString();
			} 
			else if(l.Action == 0)
			{
				labelcollapsedinfo.Text = l.Action + " - None";
			}
			else
			{
				labelcollapsedinfo.Text = l.Action + " - Unknown";
			}
			labelcollapsedinfo.Refresh();

			//mxd. let the plugins know
			General.Plugins.OnHighlightLinedef(l);
			// [ZZ]
			panelinfo.ResumeLayout();
		}

		// Show vertex info
		public void ShowVertexInfo(Vertex v) 
		{
			if(v.IsDisposed) 
			{
				HideInfo();
				return;
			}
			
			// [ZZ]
			panelinfo.SuspendLayout();
			lastinfoobject = v;
			modename.Visible = false;
#if DEBUG
			console.Visible = console.AlwaysOnTop; //mxd
#endif
			statistics.Visible = false; //mxd
			if(linedefinfo.Visible) linedefinfo.Hide();
			if(sectorinfo.Visible) sectorinfo.Hide();
			if(thinginfo.Visible) thinginfo.Hide();
			if(IsInfoPanelExpanded) vertexinfo.ShowInfo(v);

			// Show info on collapsed label
			labelcollapsedinfo.Text = v.Position.x.ToString("0.##") + ", " + v.Position.y.ToString("0.##");
			labelcollapsedinfo.Refresh();

			//mxd. let the plugins know
			General.Plugins.OnHighlightVertex(v);
			// [ZZ]
			panelinfo.ResumeLayout();
		}

		//mxd. Show sector info
		public void ShowSectorInfo(Sector s) 
		{
			ShowSectorInfo(s, false, false);
		}

		// Show sector info
		public void ShowSectorInfo(Sector s, bool highlightceiling, bool highlightfloor) 
		{
			if(s.IsDisposed) 
			{
				HideInfo();
				return;
			}

			// [ZZ]
			panelinfo.SuspendLayout();
			lastinfoobject = s;
			modename.Visible = false;
#if DEBUG
			console.Visible = console.AlwaysOnTop; //mxd
#endif
			statistics.Visible = false; //mxd
			if(linedefinfo.Visible) linedefinfo.Hide();
			if(vertexinfo.Visible) vertexinfo.Hide();
			if(thinginfo.Visible) thinginfo.Hide();
			if(IsInfoPanelExpanded) sectorinfo.ShowInfo(s, highlightceiling, highlightfloor); //mxd

			// Show info on collapsed label
			if(General.Map.Config.SectorEffects.ContainsKey(s.Effect))
				labelcollapsedinfo.Text = General.Map.Config.SectorEffects[s.Effect].ToString();
			else if(s.Effect == 0)
				labelcollapsedinfo.Text = s.Effect + " - Normal";
			else
				labelcollapsedinfo.Text = s.Effect + " - Unknown";

			labelcollapsedinfo.Refresh();

			//mxd. let the plugins know
			General.Plugins.OnHighlightSector(s);
			// [ZZ]
			panelinfo.ResumeLayout();
		}

		// Show thing info
		public void ShowThingInfo(Thing t)
		{
			if(t.IsDisposed)
			{
				HideInfo();
				return;
			}

			// [ZZ]
			panelinfo.SuspendLayout();
			lastinfoobject = t;
			modename.Visible = false;
#if DEBUG
			console.Visible = console.AlwaysOnTop; //mxd
#endif
			statistics.Visible = false; //mxd
			if(linedefinfo.Visible) linedefinfo.Hide();
			if(vertexinfo.Visible) vertexinfo.Hide();
			if(sectorinfo.Visible) sectorinfo.Hide();
			if(IsInfoPanelExpanded) thinginfo.ShowInfo(t);

			// Show info on collapsed label
			ThingTypeInfo ti = General.Map.Data.GetThingInfo(t.Type);
			labelcollapsedinfo.Text = t.Type + " - " + ti.Title;
			labelcollapsedinfo.Refresh();

			//mxd. let the plugins know
			General.Plugins.OnHighlightThing(t);
			// [ZZ]
			panelinfo.ResumeLayout();
		}

		#endregion

		#region ================== Dialogs

		// This browses for a texture
		// Returns the new texture name or the same texture name when cancelled
		public string BrowseTexture(IWin32Window owner, string initialvalue)
		{
			return TextureBrowserForm.Browse(owner, initialvalue, false);//mxd
		}

		// This browses for a flat
		// Returns the new flat name or the same flat name when cancelled
		public string BrowseFlat(IWin32Window owner, string initialvalue)
		{
			return TextureBrowserForm.Browse(owner, initialvalue, true); //mxd. was FlatBrowserForm
		}
		
		// This browses the lindef types
		// Returns the new action or the same action when cancelled
		public int BrowseLinedefActions(IWin32Window owner, int initialvalue)
		{
			return ActionBrowserForm.BrowseAction(owner, initialvalue, false);
		}
		
		//mxd. This browses the lindef types
		// Returns the new action or the same action when cancelled
		public int BrowseLinedefActions(IWin32Window owner, int initialvalue, bool addanyaction)
		{
			return ActionBrowserForm.BrowseAction(owner, initialvalue, addanyaction);
		}

		// This browses sector effects
		// Returns the new effect or the same effect when cancelled
		public int BrowseSectorEffect(IWin32Window owner, int initialvalue)
		{
			return EffectBrowserForm.BrowseEffect(owner, initialvalue, false);
		}

		//mxd. This browses sector effects
		// Returns the new effect or the same effect when cancelled
		public int BrowseSectorEffect(IWin32Window owner, int initialvalue, bool addanyeffect)
		{
			return EffectBrowserForm.BrowseEffect(owner, initialvalue, addanyeffect);
		}

		// This browses thing types
		// Returns the new thing type or the same thing type when cancelled
		public int BrowseThingType(IWin32Window owner, int initialvalue)
		{
			return ThingBrowserForm.BrowseThing(owner, initialvalue);
		}

		//mxd
		public DialogResult ShowEditVertices(ICollection<Vertex> vertices) 
		{
			return ShowEditVertices(vertices, true);
		}

		//mxd. This shows the dialog to edit vertices
		public DialogResult ShowEditVertices(ICollection<Vertex> vertices, bool allowPositionChange)
		{
			// Show sector edit dialog
			VertexEditForm f = new VertexEditForm();
			DisableProcessing(); //mxd
			#if NO_WIN32
			BreakExclusiveMouseInput();
			f.Closed += (object sender, EventArgs e) => {
				ResumeExclusiveMouseInput();
				EnableProcessing(); //mxd
			};
			#endif
			f.Setup(vertices, allowPositionChange);
			#if !NO_WIN32
			EnableProcessing(); //mxd
			#endif
			f.OnValuesChanged += EditForm_OnValuesChanged;
			editformopen = true; //mxd
			DialogResult result = f.ShowDialog(this);
			editformopen = false; //mxd
			f.Dispose();

			return result;
		}
		
		// This shows the dialog to edit lines
		public DialogResult ShowEditLinedefs(ICollection<Linedef> lines)
		{
			return ShowEditLinedefs(lines, false, false);
		}
		
		// This shows the dialog to edit lines
		public DialogResult ShowEditLinedefs(ICollection<Linedef> lines, bool selectfront, bool selectback)
		{
			DialogResult result;
			
			// Show line edit dialog
			if(General.Map.UDMF) //mxd
			{
				LinedefEditFormUDMF f = new LinedefEditFormUDMF(selectfront, selectback);
				DisableProcessing(); //mxd
				#if NO_WIN32
				BreakExclusiveMouseInput();
				f.Closed += (object sender, EventArgs e) => {
					ResumeExclusiveMouseInput();
					EnableProcessing(); //mxd
				};
				#endif
				f.Setup(lines, selectfront, selectback);
				#if !NO_WIN32
				EnableProcessing();
				#endif
				f.OnValuesChanged += EditForm_OnValuesChanged;
				editformopen = true; //mxd
				result = f.ShowDialog(this);
				editformopen = false; //mxd
				f.Dispose();
			}
			else
			{
				LinedefEditForm f = new LinedefEditForm();
				DisableProcessing(); //mxd
				#if NO_WIN32
				BreakExclusiveMouseInput();
				f.Closed += (object sender, EventArgs e) => {
					ResumeExclusiveMouseInput();
					EnableProcessing(); //mxd
				};
				#endif
				f.Setup(lines);
				#if !NO_WIN32
				EnableProcessing();
				#endif
				f.OnValuesChanged += EditForm_OnValuesChanged;
				editformopen = true; //mxd
				result = f.ShowDialog(this);
				editformopen = false; //mxd
				f.Dispose();
			}

			return result;
		}

		// This shows the dialog to edit sectors
		public DialogResult ShowEditSectors(ICollection<Sector> sectors)
		{
			DialogResult result;

			// Show sector edit dialog
			if(General.Map.UDMF) //mxd
			{ 
				SectorEditFormUDMF f = new SectorEditFormUDMF();
				DisableProcessing(); //mxd
				#if NO_WIN32
				BreakExclusiveMouseInput();
				f.Closed += (object sender, EventArgs e) => {
					ResumeExclusiveMouseInput();
					EnableProcessing(); //mxd
				};
				#endif
				f.Setup(sectors);
				#if !NO_WIN32
				EnableProcessing(); //mxd
				#endif
				f.OnValuesChanged += EditForm_OnValuesChanged;
				editformopen = true; //mxd
				result = f.ShowDialog(this);
				editformopen = false; //mxd
				f.Dispose();
			}
			else
			{
				SectorEditForm f = new SectorEditForm();
				DisableProcessing(); //mxd
				#if NO_WIN32
				BreakExclusiveMouseInput();
				f.Closed += (object sender, EventArgs e) => {
					ResumeExclusiveMouseInput();
					EnableProcessing(); //mxd
				};
				#endif
				f.Setup(sectors);
				#if !NO_WIN32
				EnableProcessing(); //mxd
				#endif
				f.OnValuesChanged += EditForm_OnValuesChanged;
				editformopen = true; //mxd
				result = f.ShowDialog(this);
				editformopen = false; //mxd
				f.Dispose();
			}

			return result;
		}

		// This shows the dialog to edit things
		public DialogResult ShowEditThings(ICollection<Thing> things) 
		{
			DialogResult result;

			// Show thing edit dialog
			if(General.Map.UDMF) 
			{
				ThingEditFormUDMF f = new ThingEditFormUDMF();
				DisableProcessing(); //mxd
				#if NO_WIN32
				BreakExclusiveMouseInput();
				f.Closed += (object sender, EventArgs e) => {
					ResumeExclusiveMouseInput();
					EnableProcessing(); //mxd
				};
				#endif
				f.Setup(things);
				#if !NO_WIN32
				EnableProcessing(); //mxd
				#endif
				f.OnValuesChanged += EditForm_OnValuesChanged;
				editformopen = true; //mxd
				result = f.ShowDialog(this);
				editformopen = false; //mxd
				f.Dispose();
			} 
			else 
			{
				ThingEditForm f = new ThingEditForm();
				DisableProcessing(); //mxd
				#if NO_WIN32
				BreakExclusiveMouseInput();
				f.Closed += (object sender, EventArgs e) => {
					ResumeExclusiveMouseInput();
					EnableProcessing(); //mxd
				};
				#endif
				f.Setup(things);
				#if !NO_WIN32
				EnableProcessing(); //mxd
				#endif
				f.OnValuesChanged += EditForm_OnValuesChanged;
				editformopen = true; //mxd
				result = f.ShowDialog(this);
				editformopen = false; //mxd
				f.Dispose();
			}

			return result;
		}

		//mxd
		private void EditForm_OnValuesChanged(object sender, EventArgs e) 
		{
			if(OnEditFormValuesChanged != null) 
			{
				OnEditFormValuesChanged(sender, e);
			} 
			else 
			{
				//If current mode doesn't handle this event, let's at least update the map and redraw display.
				General.Map.Map.Update();
				RedrawDisplay();
			}
		}

        #endregion

        #region ================== Threadsafe updates

        object syncobject = new object();
        List<System.Action> queuedActions = new List<System.Action>();

        internal void ProcessQueuedUIActions()
        {
            List<System.Action> queue;
            lock (syncobject)
            {
                queue = queuedActions;
                queuedActions = new List<System.Action>();
            }

            foreach (System.Action action in queue)
            {
                action();
            }
        }

        public void RunOnUIThread(System.Action action)
        {
            if (!InvokeRequired)
            {
                action();
            }
            else
            {
                bool notify;
                lock (syncobject)
                {
                    notify = queuedActions.Count == 0;
                    queuedActions.Add(action);
                }

                if (notify)
                    General.InvokeUIActions(this);
            }
        }

        public void UpdateStatus()
        {
            RunOnUIThread(() =>
            {
                DisplayStatus(status);
            });
        }

        public void ImageDataLoaded(string imagename)
        {
            RunOnUIThread(() =>
            {
                if ((General.Map != null) && (General.Map.Data != null))
                {
                    ImageData img = General.Map.Data.GetFlatImage(imagename);
                    ImageDataLoaded(img);
                }
            });
        }

        public void SpriteDataLoaded(string spritename)
        {
            RunOnUIThread(() =>
            {
                if ((General.Map != null) && (General.Map.Data != null))
                {
                    ImageData img = General.Map.Data.GetSpriteImage(spritename);
                    if (img != null && img.UsedInMap && !img.IsDisposed)
                    {
                        DelayedRedraw();
                    }
                }
            });
        }

        #endregion

        #region ================== Message Pump

        // This handles messages
        protected override void WndProc(ref Message m)
		{
			// Notify message?
			switch(m.Msg)
			{
                case General.WM_UIACTION:
                    ProcessQueuedUIActions();
                    break;

                case General.WM_SYSCOMMAND:
					// We don't want to open a menu when ALT is pressed
					if(m.WParam.ToInt32() != General.SC_KEYMENU)
					{
						base.WndProc(ref m);
					}
					break;

				case General.WM_MOUSEHWHEEL:
                    int delta = unchecked((short)(m.WParam.ToInt64() >> 16));
                    OnMouseHWheel(delta);
					m.Result = new IntPtr(delta);
					break;

				default:
					// Let the base handle the message
					base.WndProc(ref m);
					break;
			}
		}

		//mxd. Warnings panel
		private delegate void SetWarningsCountCallback(int count, bool blink);
		internal void SetWarningsCount(int count, bool blink) 
		{
            RunOnUIThread(() =>
            {
                // Update icon, start annoying blinking if necessary
                if (count > 0)
                {
                    if (blink && !blinkTimer.Enabled) blinkTimer.Start();
                    warnsLabel.Image = Resources.Warning;
                }
                else
                {
                    blinkTimer.Stop();
                    warnsLabel.Image = Resources.WarningOff;
                    warnsLabel.BackColor = SystemColors.Control;
                }

                // Update errors count
                warnsLabel.Text = count.ToString();
            });
		}

		//mxd. Bliks warnings indicator
		private void Blink() 
		{
			warnsLabel.BackColor = (warnsLabel.BackColor == Color.Red ? SystemColors.Control : Color.Red);
		}

		//mxd
		private void warnsLabel_Click(object sender, EventArgs e) 
		{
			ShowErrors();
		}

		//mxd
		private void blinkTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e) 
		{
			if(!blinkTimer.Enabled) return;
			try 
			{
                RunOnUIThread(() =>
                {
                    Blink();
                });
			} catch(ObjectDisposedException) { } //la-la-la. We don't care.
		}
		
		#endregion
		
		#region ================== Processing
		
		// This is called from the background thread when images are loaded
		// but only when first loaded or when dimensions were changed
		internal void ImageDataLoaded(ImageData img)
		{
			// Image is used in the map?
			if ((img != null) && img.UsedInMap && !img.IsDisposed)
			{
				// Go for all setors
				bool updated = false;
				long imgshorthash = General.Map.Data.GetShortLongFlatName(img.LongName); //mxd. Part of long name support shennanigans

				foreach(Sector s in General.Map.Map.Sectors)
				{
					// Update floor buffer if needed
					if(s.LongFloorTexture == img.LongName || s.LongFloorTexture == imgshorthash)
					{
						s.UpdateFloorSurface();
						updated = true;
					}
					
					// Update ceiling buffer if needed
					if(s.LongCeilTexture == img.LongName || s.LongCeilTexture == imgshorthash)
					{
						s.UpdateCeilingSurface();
						updated = true;
					}
				}
				
				// If we made updates, redraw the screen
				if(updated) DelayedRedraw();
			}
		}

		public void EnableProcessing()
		{
			// Increase count
			processingcount++;

			// If not already enabled, enable processing now
			if(!processor.Enabled)
			{
				processor.Enabled = true;
				lastupdatetime = Clock.CurrentTime;
			}
		}

		public void DisableProcessing()
		{
			// Increase count
			processingcount--;
			if(processingcount < 0) processingcount = 0;
			
			// Turn off
			if(processor.Enabled && (processingcount == 0))
				processor.Enabled = false;
		}

		internal void StopProcessing()
		{
			// Turn off
			processingcount = 0;
			processor.Enabled = false;
		}

		//mxd
		internal void ResetClock()
		{
			Clock.Reset();
			lastupdatetime = 0;
			
			// Let the mode know...
			if(General.Editing.Mode != null)
				General.Editing.Mode.OnClockReset();
		}
		
		// Processor event
		private void processor_Tick(object sender, EventArgs e)
		{
			long curtime = Clock.CurrentTime;
			long deltatime = curtime - lastupdatetime;
			lastupdatetime = curtime;
			
			if((General.Map != null) && (General.Editing.Mode != null))
			{
				// In exclusive mouse mode?
				if(mouseinput != null)
				{
					Vector2D deltamouse = mouseinput.Process();
					General.Plugins.OnEditMouseInput(deltamouse);
					General.Editing.Mode.OnMouseInput(deltamouse);
				}

				// Process signal
				General.Editing.Mode.OnProcess(deltatime);
			}
		}

		#endregion

		#region ================== Dockers
		
		// This adds a docker
		public void AddDocker(Docker d)
		{
			if(dockerspanel.Contains(d)) return; //mxd
			
			// Make sure the full name is set with the plugin name as prefix
			Plugin plugin = General.Plugins.FindPluginByAssembly(Assembly.GetCallingAssembly());
			d.MakeFullName(plugin.Name.ToLowerInvariant());

			dockerspanel.Add(d, false);
		}

		//mxd. This also adds a docker
		public void AddDocker(Docker d, bool notify)
		{
			if(dockerspanel.Contains(d)) return; //mxd

			// Make sure the full name is set with the plugin name as prefix
			Plugin plugin = General.Plugins.FindPluginByAssembly(Assembly.GetCallingAssembly());
			d.MakeFullName(plugin.Name.ToLowerInvariant());
			
			dockerspanel.Add(d, notify);
		}
		
		// This removes a docker
		public bool RemoveDocker(Docker d)
		{
			if(!dockerspanel.Contains(d)) return true; //mxd. Already removed/never added
			
			// Make sure the full name is set with the plugin name as prefix
			//Plugin plugin = General.Plugins.FindPluginByAssembly(Assembly.GetCallingAssembly());
			//d.MakeFullName(plugin.Name.ToLowerInvariant());
			
			// We must release all keys because the focus may be stolen when
			// this was the selected docker (the previous docker is automatically selected)
			ReleaseAllKeys();
			
			return dockerspanel.Remove(d);
		}
		
		// This selects a docker
		public bool SelectDocker(Docker d)
		{
			if(!dockerspanel.Contains(d)) return false; //mxd
			
			// Make sure the full name is set with the plugin name as prefix
			Plugin plugin = General.Plugins.FindPluginByAssembly(Assembly.GetCallingAssembly());
			d.MakeFullName(plugin.Name.ToLowerInvariant());
			
			// We must release all keys because the focus will be stolen
			ReleaseAllKeys();
			
			return dockerspanel.SelectDocker(d);
		}
		
		// This selects the previous selected docker
		public void SelectPreviousDocker()
		{
			// We must release all keys because the focus will be stolen
			ReleaseAllKeys();
			
			dockerspanel.SelectPrevious();
		}
		
		// Mouse enters dockers window
		private void dockerspanel_MouseContainerEnter(object sender, EventArgs e)
		{
			if(General.Settings.CollapseDockers)
				dockerscollapser.Start();
			
			dockerspanel.Expand();
		}
		
		// Automatic collapsing
		private void dockerscollapser_Tick(object sender, EventArgs e)
		{
			if(General.Settings.CollapseDockers)
			{
				if(!dockerspanel.IsFocused)
				{
					Point p = this.PointToClient(Cursor.Position);
					Rectangle r = new Rectangle(dockerspanel.Location, dockerspanel.Size);
					if(!r.IntersectsWith(new Rectangle(p, Size.Empty)))
					{
						dockerspanel.Collapse();
						dockerscollapser.Stop();
					}
				}
			}
			else
			{
				dockerscollapser.Stop();
			}
		}
		
		// User resizes the docker
		private void dockerspanel_UserResize(object sender, EventArgs e)
		{
			General.Settings.DockersWidth = dockerspanel.Width;

			if(!General.Settings.CollapseDockers)
			{
				dockersspace.Width = dockerspanel.Width;
				dockerspanel.Left = dockersspace.Left;
			}
		}
		
		#endregion

		#region ================== Updater (mxd)

		private delegate void UpdateAvailableCallback(int remoterev, string changelog);
        internal void UpdateAvailable(int remoterev, string changelog)
        {
            RunOnUIThread(() => {
                // Show the window
                UpdateForm form = new UpdateForm(remoterev, changelog);
                form.FormClosing += delegate
                {
                    // Update ignored revision number
                    General.Settings.IgnoredRemoteRevision = (form.IgnoreThisUpdate ? remoterev : 0);
                };
                form.Show(this);
            });
		}

		#endregion

		#region ================== Graphics (mxd)

		public SizeF MeasureString(string text, Font font)
		{
			SizeF length;

			// Be thread safe
			lock(graphics)
			{
				length = graphics.MeasureString(text, font);
			}

			return length;
		}

		public SizeF MeasureString(string text, Font font, int width, StringFormat format)
		{
			SizeF length;

			// Be thread safe
			lock (graphics)
			{

				length = graphics.MeasureString(text, font, width, format);
			}

			return length;
		}

		#endregion
	}
}
