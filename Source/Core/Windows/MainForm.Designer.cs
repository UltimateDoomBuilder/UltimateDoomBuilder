using System;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Controls;
using System.Reflection;

namespace CodeImp.DoomBuilder.Windows
{
	partial class MainForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if(disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
			System.Windows.Forms.ToolStripSeparator toolStripSeparator9;
			System.Windows.Forms.ToolStripSeparator toolStripSeparator12;
			System.Windows.Forms.ToolStripSeparator toolStripMenuItem4;
			System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
			System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			this.seperatorfileopen = new System.Windows.Forms.ToolStripSeparator();
			this.seperatorfilerecent = new System.Windows.Forms.ToolStripSeparator();
			this.seperatoreditgrid = new System.Windows.Forms.ToolStripSeparator();
			this.seperatoreditcopypaste = new System.Windows.Forms.ToolStripSeparator();
			this.seperatorfile = new System.Windows.Forms.ToolStripSeparator();
			this.seperatorscript = new System.Windows.Forms.ToolStripSeparator();
			this.seperatorprefabs = new System.Windows.Forms.ToolStripSeparator();
			this.seperatorundo = new System.Windows.Forms.ToolStripSeparator();
			this.seperatorcopypaste = new System.Windows.Forms.ToolStripSeparator();
			this.poscommalabel = new System.Windows.Forms.ToolStripStatusLabel();
			this.menumain = new System.Windows.Forms.MenuStrip();
			this.menufile = new System.Windows.Forms.ToolStripMenuItem();
			this.itemnewmap = new System.Windows.Forms.ToolStripMenuItem();
			this.itemopenmap = new System.Windows.Forms.ToolStripMenuItem();
			this.itemopenmapincurwad = new System.Windows.Forms.ToolStripMenuItem();
			this.itemclosemap = new System.Windows.Forms.ToolStripMenuItem();
			this.itemsavemap = new System.Windows.Forms.ToolStripMenuItem();
			this.itemsavemapas = new System.Windows.Forms.ToolStripMenuItem();
			this.itemsavemapinto = new System.Windows.Forms.ToolStripMenuItem();
			this.seperatorfilesave = new System.Windows.Forms.ToolStripSeparator();
			this.itemimport = new System.Windows.Forms.ToolStripMenuItem();
			this.itemexport = new System.Windows.Forms.ToolStripMenuItem();
			this.separatorio = new System.Windows.Forms.ToolStripSeparator();
			this.itemnorecent = new System.Windows.Forms.ToolStripMenuItem();
			this.itemexit = new System.Windows.Forms.ToolStripMenuItem();
			this.menuedit = new System.Windows.Forms.ToolStripMenuItem();
			this.itemundo = new System.Windows.Forms.ToolStripMenuItem();
			this.itemredo = new System.Windows.Forms.ToolStripMenuItem();
			this.seperatoreditundo = new System.Windows.Forms.ToolStripSeparator();
			this.itemcut = new System.Windows.Forms.ToolStripMenuItem();
			this.itemcopy = new System.Windows.Forms.ToolStripMenuItem();
			this.itempaste = new System.Windows.Forms.ToolStripMenuItem();
			this.itempastespecial = new System.Windows.Forms.ToolStripMenuItem();
			this.itemsnaptogrid = new System.Windows.Forms.ToolStripMenuItem();
			this.itemautomerge = new System.Windows.Forms.ToolStripMenuItem();
			this.itemsplitjoinedsectors = new System.Windows.Forms.ToolStripMenuItem();
			this.itemautoclearsidetextures = new System.Windows.Forms.ToolStripMenuItem();
			this.seperatoreditgeometry = new System.Windows.Forms.ToolStripSeparator();
			this.itemgridinc = new System.Windows.Forms.ToolStripMenuItem();
			this.itemgriddec = new System.Windows.Forms.ToolStripMenuItem();
			this.itemdosnaptogrid = new System.Windows.Forms.ToolStripMenuItem();
			this.itemgridsetup = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
			this.addToGroup = new System.Windows.Forms.ToolStripMenuItem();
			this.selectGroup = new System.Windows.Forms.ToolStripMenuItem();
			this.clearGroup = new System.Windows.Forms.ToolStripMenuItem();
			this.itemmapoptions = new System.Windows.Forms.ToolStripMenuItem();
			this.itemviewusedtags = new System.Windows.Forms.ToolStripMenuItem();
			this.itemviewthingtypes = new System.Windows.Forms.ToolStripMenuItem();
			this.menuview = new System.Windows.Forms.ToolStripMenuItem();
			this.itemthingsfilter = new System.Windows.Forms.ToolStripMenuItem();
			this.itemlinedefcolors = new System.Windows.Forms.ToolStripMenuItem();
			this.seperatorviewthings = new System.Windows.Forms.ToolStripSeparator();
			this.itemviewnormal = new System.Windows.Forms.ToolStripMenuItem();
			this.itemviewbrightness = new System.Windows.Forms.ToolStripMenuItem();
			this.itemviewfloors = new System.Windows.Forms.ToolStripMenuItem();
			this.itemviewceilings = new System.Windows.Forms.ToolStripMenuItem();
			this.seperatorviewviews = new System.Windows.Forms.ToolStripSeparator();
			this.itemmergegeoclassic = new System.Windows.Forms.ToolStripMenuItem();
			this.itemmergegeo = new System.Windows.Forms.ToolStripMenuItem();
			this.itemreplacegeo = new System.Windows.Forms.ToolStripMenuItem();
			this.separatorgeomerge = new System.Windows.Forms.ToolStripSeparator();
			this.itemfullbrightness = new System.Windows.Forms.ToolStripMenuItem();
			this.itemtogglegrid = new System.Windows.Forms.ToolStripMenuItem();
			this.separatorhelpers = new System.Windows.Forms.ToolStripSeparator();
			this.menuzoom = new System.Windows.Forms.ToolStripMenuItem();
			this.item2zoom800 = new System.Windows.Forms.ToolStripMenuItem();
			this.item2zoom400 = new System.Windows.Forms.ToolStripMenuItem();
			this.item2zoom200 = new System.Windows.Forms.ToolStripMenuItem();
			this.item2zoom100 = new System.Windows.Forms.ToolStripMenuItem();
			this.item2zoom50 = new System.Windows.Forms.ToolStripMenuItem();
			this.item2zoom25 = new System.Windows.Forms.ToolStripMenuItem();
			this.item2zoom10 = new System.Windows.Forms.ToolStripMenuItem();
			this.item2zoom5 = new System.Windows.Forms.ToolStripMenuItem();
			this.itemgotocoords = new System.Windows.Forms.ToolStripMenuItem();
			this.itemfittoscreen = new System.Windows.Forms.ToolStripMenuItem();
			this.itemtoggleinfo = new System.Windows.Forms.ToolStripMenuItem();
			this.seperatorviewzoom = new System.Windows.Forms.ToolStripSeparator();
			this.itemscripteditor = new System.Windows.Forms.ToolStripMenuItem();
			this.menumode = new System.Windows.Forms.ToolStripMenuItem();
			this.separatorDrawModes = new System.Windows.Forms.ToolStripSeparator();
			this.separatorTransformModes = new System.Windows.Forms.ToolStripSeparator();
			this.menuprefabs = new System.Windows.Forms.ToolStripMenuItem();
			this.iteminsertprefabfile = new System.Windows.Forms.ToolStripMenuItem();
			this.iteminsertpreviousprefab = new System.Windows.Forms.ToolStripMenuItem();
			this.seperatorprefabsinsert = new System.Windows.Forms.ToolStripSeparator();
			this.itemcreateprefab = new System.Windows.Forms.ToolStripMenuItem();
			this.menutools = new System.Windows.Forms.ToolStripMenuItem();
			this.itemreloadresources = new System.Windows.Forms.ToolStripMenuItem();
			this.itemReloadModedef = new System.Windows.Forms.ToolStripMenuItem();
			this.itemReloadGldefs = new System.Windows.Forms.ToolStripMenuItem();
			this.itemshowerrors = new System.Windows.Forms.ToolStripMenuItem();
			this.seperatortoolsresources = new System.Windows.Forms.ToolStripSeparator();
			this.configurationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.preferencesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.seperatortoolsconfig = new System.Windows.Forms.ToolStripSeparator();
			this.itemsavescreenshot = new System.Windows.Forms.ToolStripMenuItem();
			this.itemsaveeditareascreenshot = new System.Windows.Forms.ToolStripMenuItem();
			this.separatortoolsscreenshots = new System.Windows.Forms.ToolStripSeparator();
			this.itemtestmap = new System.Windows.Forms.ToolStripMenuItem();
			this.menuhelp = new System.Windows.Forms.ToolStripMenuItem();
			this.itemhelprefmanual = new System.Windows.Forms.ToolStripMenuItem();
			this.itemShortcutReference = new System.Windows.Forms.ToolStripMenuItem();
			this.itemopenconfigfolder = new System.Windows.Forms.ToolStripMenuItem();
			this.itemhelpeditmode = new System.Windows.Forms.ToolStripMenuItem();
			this.itemhelpcheckupdates = new System.Windows.Forms.ToolStripMenuItem();
			this.seperatorhelpmanual = new System.Windows.Forms.ToolStripSeparator();
			this.itemhelpissues = new System.Windows.Forms.ToolStripMenuItem();
			this.itemhelpabout = new System.Windows.Forms.ToolStripMenuItem();
			this.toolbar = new System.Windows.Forms.ToolStrip();
			this.toolbarContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.toggleFile = new System.Windows.Forms.ToolStripMenuItem();
			this.toggleScript = new System.Windows.Forms.ToolStripMenuItem();
			this.toggleUndo = new System.Windows.Forms.ToolStripMenuItem();
			this.toggleCopy = new System.Windows.Forms.ToolStripMenuItem();
			this.togglePrefabs = new System.Windows.Forms.ToolStripMenuItem();
			this.toggleFilter = new System.Windows.Forms.ToolStripMenuItem();
			this.toggleViewModes = new System.Windows.Forms.ToolStripMenuItem();
			this.toggleGeometry = new System.Windows.Forms.ToolStripMenuItem();
			this.toggleTesting = new System.Windows.Forms.ToolStripMenuItem();
			this.toggleRendering = new System.Windows.Forms.ToolStripMenuItem();
			this.buttonnewmap = new System.Windows.Forms.ToolStripButton();
			this.buttonopenmap = new System.Windows.Forms.ToolStripButton();
			this.buttonsavemap = new System.Windows.Forms.ToolStripButton();
			this.buttonscripteditor = new System.Windows.Forms.ToolStripButton();
			this.buttonundo = new System.Windows.Forms.ToolStripButton();
			this.buttonredo = new System.Windows.Forms.ToolStripButton();
			this.buttoncut = new System.Windows.Forms.ToolStripButton();
			this.buttoncopy = new System.Windows.Forms.ToolStripButton();
			this.buttonpaste = new System.Windows.Forms.ToolStripButton();
			this.buttoninsertprefabfile = new System.Windows.Forms.ToolStripButton();
			this.buttoninsertpreviousprefab = new System.Windows.Forms.ToolStripButton();
			this.buttonthingsfilter = new System.Windows.Forms.ToolStripButton();
			this.thingfilters = new System.Windows.Forms.ToolStripDropDownButton();
			this.separatorlinecolors = new System.Windows.Forms.ToolStripSeparator();
			this.buttonlinededfcolors = new System.Windows.Forms.ToolStripButton();
			this.linedefcolorpresets = new System.Windows.Forms.ToolStripDropDownButton();
			this.separatorfilters = new System.Windows.Forms.ToolStripSeparator();
			this.separatorrendering = new System.Windows.Forms.ToolStripSeparator();
			this.itemdynlightmodes = new System.Windows.Forms.ToolStripMenuItem();
			this.itemnodynlights = new System.Windows.Forms.ToolStripMenuItem();
			this.itemdynlights = new System.Windows.Forms.ToolStripMenuItem();
			this.itemdynlightsanim = new System.Windows.Forms.ToolStripMenuItem();
			this.itemmodelmodes = new System.Windows.Forms.ToolStripMenuItem();
			this.itemnomdl = new System.Windows.Forms.ToolStripMenuItem();
			this.itemselmdl = new System.Windows.Forms.ToolStripMenuItem();
			this.itemfiltermdl = new System.Windows.Forms.ToolStripMenuItem();
			this.itemallmdl = new System.Windows.Forms.ToolStripMenuItem();
			this.itemtogglefog = new System.Windows.Forms.ToolStripMenuItem();
			this.itemtogglesky = new System.Windows.Forms.ToolStripMenuItem();
			this.itemtoggleclassicrendering = new System.Windows.Forms.ToolStripMenuItem();
			this.itemtoggleeventlines = new System.Windows.Forms.ToolStripMenuItem();
			this.itemtogglevisualverts = new System.Windows.Forms.ToolStripMenuItem();
			this.buttonfullbrightness = new System.Windows.Forms.ToolStripButton();
			this.buttontogglegrid = new System.Windows.Forms.ToolStripButton();
			this.buttontoggledynamicgrid = new System.Windows.Forms.ToolStripButton();
			this.separatorfullbrightness = new System.Windows.Forms.ToolStripSeparator();
			this.buttonviewnormal = new System.Windows.Forms.ToolStripButton();
			this.buttonviewbrightness = new System.Windows.Forms.ToolStripButton();
			this.buttonviewfloors = new System.Windows.Forms.ToolStripButton();
			this.buttonviewceilings = new System.Windows.Forms.ToolStripButton();
			this.separatorgeomergemodes = new System.Windows.Forms.ToolStripSeparator();
			this.buttonmergegeoclassic = new System.Windows.Forms.ToolStripButton();
			this.buttonmergegeo = new System.Windows.Forms.ToolStripButton();
			this.buttonplacegeo = new System.Windows.Forms.ToolStripButton();
			this.seperatorviews = new System.Windows.Forms.ToolStripSeparator();
			this.buttontogglecomments = new System.Windows.Forms.ToolStripButton();
			this.buttontogglefixedthingsscale = new System.Windows.Forms.ToolStripButton();
			this.buttonsnaptogrid = new System.Windows.Forms.ToolStripButton();
			this.buttonautomerge = new System.Windows.Forms.ToolStripButton();
			this.buttonsplitjoinedsectors = new System.Windows.Forms.ToolStripButton();
			this.buttonautoclearsidetextures = new System.Windows.Forms.ToolStripButton();
			this.seperatorgeometry = new System.Windows.Forms.ToolStripSeparator();
			this.dynamiclightmode = new System.Windows.Forms.ToolStripSplitButton();
			this.sightsdontshow = new System.Windows.Forms.ToolStripMenuItem();
			this.lightsshow = new System.Windows.Forms.ToolStripMenuItem();
			this.lightsshowanimated = new System.Windows.Forms.ToolStripMenuItem();
			this.modelrendermode = new System.Windows.Forms.ToolStripSplitButton();
			this.modelsdontshow = new System.Windows.Forms.ToolStripMenuItem();
			this.modelsshowselection = new System.Windows.Forms.ToolStripMenuItem();
			this.modelsshowfiltered = new System.Windows.Forms.ToolStripMenuItem();
			this.modelsshowall = new System.Windows.Forms.ToolStripMenuItem();
			this.buttontogglefog = new System.Windows.Forms.ToolStripButton();
			this.buttontogglesky = new System.Windows.Forms.ToolStripButton();
			this.buttontoggleclassicrendering = new System.Windows.Forms.ToolStripButton();
			this.buttontoggleeventlines = new System.Windows.Forms.ToolStripButton();
			this.buttontogglevisualvertices = new System.Windows.Forms.ToolStripButton();
			this.separatorgzmodes = new System.Windows.Forms.ToolStripSeparator();
			this.buttontest = new System.Windows.Forms.ToolStripSplitButton();
			this.seperatortesting = new System.Windows.Forms.ToolStripSeparator();
			this.statusbar = new System.Windows.Forms.StatusStrip();
			this.statuslabel = new System.Windows.Forms.ToolStripStatusLabel();
			this.configlabel = new System.Windows.Forms.ToolStripStatusLabel();
			this.gridlabel = new System.Windows.Forms.ToolStripStatusLabel();
			this.buttongrid = new System.Windows.Forms.ToolStripDropDownButton();
			this.itemgrid1024 = new System.Windows.Forms.ToolStripMenuItem();
			this.itemgrid512 = new System.Windows.Forms.ToolStripMenuItem();
			this.itemgrid256 = new System.Windows.Forms.ToolStripMenuItem();
			this.itemgrid128 = new System.Windows.Forms.ToolStripMenuItem();
			this.itemgrid64 = new System.Windows.Forms.ToolStripMenuItem();
			this.itemgrid32 = new System.Windows.Forms.ToolStripMenuItem();
			this.itemgrid16 = new System.Windows.Forms.ToolStripMenuItem();
			this.itemgrid8 = new System.Windows.Forms.ToolStripMenuItem();
			this.itemgrid4 = new System.Windows.Forms.ToolStripMenuItem();
			this.itemgrid1 = new System.Windows.Forms.ToolStripMenuItem();
			this.itemgrid05 = new System.Windows.Forms.ToolStripMenuItem();
			this.itemgrid025 = new System.Windows.Forms.ToolStripMenuItem();
			this.itemgrid0125 = new System.Windows.Forms.ToolStripMenuItem();
			this.itemgridcustom = new System.Windows.Forms.ToolStripMenuItem();
			this.zoomlabel = new System.Windows.Forms.ToolStripStatusLabel();
			this.buttonzoom = new System.Windows.Forms.ToolStripDropDownButton();
			this.itemzoom800 = new System.Windows.Forms.ToolStripMenuItem();
			this.itemzoom400 = new System.Windows.Forms.ToolStripMenuItem();
			this.itemzoom200 = new System.Windows.Forms.ToolStripMenuItem();
			this.itemzoom100 = new System.Windows.Forms.ToolStripMenuItem();
			this.itemzoom50 = new System.Windows.Forms.ToolStripMenuItem();
			this.itemzoom25 = new System.Windows.Forms.ToolStripMenuItem();
			this.itemzoom10 = new System.Windows.Forms.ToolStripMenuItem();
			this.itemzoom5 = new System.Windows.Forms.ToolStripMenuItem();
			this.itemzoomfittoscreen = new System.Windows.Forms.ToolStripMenuItem();
			this.xposlabel = new System.Windows.Forms.ToolStripStatusLabel();
			this.yposlabel = new System.Windows.Forms.ToolStripStatusLabel();
			this.warnsLabel = new System.Windows.Forms.ToolStripStatusLabel();
			this.panelinfo = new System.Windows.Forms.Panel();
			this.statistics = new CodeImp.DoomBuilder.Controls.StatisticsControl();
			this.heightpanel1 = new System.Windows.Forms.Panel();
			this.labelcollapsedinfo = new System.Windows.Forms.Label();
			this.modename = new System.Windows.Forms.Label();
			this.buttontoggleinfo = new System.Windows.Forms.Button();
			this.console = new CodeImp.DoomBuilder.DebugConsole();
			this.vertexinfo = new CodeImp.DoomBuilder.Controls.VertexInfoPanel();
			this.linedefinfo = new CodeImp.DoomBuilder.Controls.LinedefInfoPanel();
			this.thinginfo = new CodeImp.DoomBuilder.Controls.ThingInfoPanel();
			this.sectorinfo = new CodeImp.DoomBuilder.Controls.SectorInfoPanel();
			this.redrawtimer = new System.Windows.Forms.Timer(this.components);
			this.display = new RenderTargetControl();
			this.processor = new System.Windows.Forms.Timer(this.components);
			this.statusflasher = new System.Windows.Forms.Timer(this.components);
			this.statusresetter = new System.Windows.Forms.Timer(this.components);
			this.dockersspace = new System.Windows.Forms.Panel();
			this.modestoolbar = new System.Windows.Forms.ToolStrip();
			this.dockerspanel = new CodeImp.DoomBuilder.Controls.DockersControl();
			this.dockerscollapser = new System.Windows.Forms.Timer(this.components);
			this.flowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
			this.modecontrolsloolbar = new System.Windows.Forms.ToolStrip();
			this.itemtogglecomments = new System.Windows.Forms.ToolStripMenuItem();
			this.itemtogglefixedthingsscale = new System.Windows.Forms.ToolStripMenuItem();
			this.itemtogglealwaysshowvertices = new System.Windows.Forms.ToolStripMenuItem();
			this.itemdynamicgridsize = new System.Windows.Forms.ToolStripMenuItem();
			this.itemaligngridtolinedef = new System.Windows.Forms.ToolStripMenuItem();
			this.itemsetgridorigintovertex = new System.Windows.Forms.ToolStripMenuItem();
			this.itemresetgrid = new System.Windows.Forms.ToolStripMenuItem();
			toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
			toolStripSeparator12 = new System.Windows.Forms.ToolStripSeparator();
			toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
			toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			this.menumain.SuspendLayout();
			this.toolbar.SuspendLayout();
			this.toolbarContextMenu.SuspendLayout();
			this.statusbar.SuspendLayout();
			this.panelinfo.SuspendLayout();
			this.flowLayoutPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// toolStripSeparator1
			// 
			toolStripSeparator1.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
			toolStripSeparator1.Name = "toolStripSeparator1";
			toolStripSeparator1.Size = new System.Drawing.Size(6, 23);
			// 
			// toolStripSeparator9
			// 
			toolStripSeparator9.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
			toolStripSeparator9.Name = "toolStripSeparator9";
			toolStripSeparator9.Size = new System.Drawing.Size(6, 23);
			// 
			// toolStripSeparator12
			// 
			toolStripSeparator12.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
			toolStripSeparator12.Name = "toolStripSeparator12";
			toolStripSeparator12.Size = new System.Drawing.Size(6, 23);
			// 
			// toolStripMenuItem4
			// 
			toolStripMenuItem4.Name = "toolStripMenuItem4";
			toolStripMenuItem4.Size = new System.Drawing.Size(150, 6);
			// 
			// toolStripSeparator2
			// 
			toolStripSeparator2.Name = "toolStripSeparator2";
			toolStripSeparator2.Size = new System.Drawing.Size(153, 6);
			// 
			// toolStripSeparator3
			// 
			toolStripSeparator3.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
			toolStripSeparator3.Name = "toolStripSeparator3";
			toolStripSeparator3.Size = new System.Drawing.Size(6, 23);
			// 
			// seperatorfileopen
			// 
			this.seperatorfileopen.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
			this.seperatorfileopen.Name = "seperatorfileopen";
			this.seperatorfileopen.Size = new System.Drawing.Size(222, 6);
			// 
			// seperatorfilerecent
			// 
			this.seperatorfilerecent.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
			this.seperatorfilerecent.Name = "seperatorfilerecent";
			this.seperatorfilerecent.Size = new System.Drawing.Size(222, 6);
			// 
			// seperatoreditgrid
			// 
			this.seperatoreditgrid.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
			this.seperatoreditgrid.Name = "seperatoreditgrid";
			this.seperatoreditgrid.Size = new System.Drawing.Size(216, 6);
			// 
			// seperatoreditcopypaste
			// 
			this.seperatoreditcopypaste.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
			this.seperatoreditcopypaste.Name = "seperatoreditcopypaste";
			this.seperatoreditcopypaste.Size = new System.Drawing.Size(216, 6);
			// 
			// seperatorfile
			// 
			this.seperatorfile.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
			this.seperatorfile.Name = "seperatorfile";
			this.seperatorfile.Size = new System.Drawing.Size(6, 25);
			// 
			// seperatorscript
			// 
			this.seperatorscript.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
			this.seperatorscript.Name = "seperatorscript";
			this.seperatorscript.Size = new System.Drawing.Size(6, 25);
			// 
			// seperatorprefabs
			// 
			this.seperatorprefabs.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
			this.seperatorprefabs.Name = "seperatorprefabs";
			this.seperatorprefabs.Size = new System.Drawing.Size(6, 25);
			// 
			// seperatorundo
			// 
			this.seperatorundo.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
			this.seperatorundo.Name = "seperatorundo";
			this.seperatorundo.Size = new System.Drawing.Size(6, 25);
			// 
			// seperatorcopypaste
			// 
			this.seperatorcopypaste.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
			this.seperatorcopypaste.Name = "seperatorcopypaste";
			this.seperatorcopypaste.Size = new System.Drawing.Size(6, 25);
			// 
			// poscommalabel
			// 
			this.poscommalabel.Name = "poscommalabel";
			this.poscommalabel.Size = new System.Drawing.Size(11, 18);
			this.poscommalabel.Tag = "builder_centeroncoordinates";
			this.poscommalabel.Text = ",";
			this.poscommalabel.ToolTipText = "Current X, Y coordinates on map.\r\nClick to set specific coordinates.";
			this.poscommalabel.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// menumain
			// 
			this.menumain.Dock = System.Windows.Forms.DockStyle.None;
			this.menumain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.menufile,
			this.menuedit,
			this.menuview,
			this.menumode,
			this.menuprefabs,
			this.menutools,
			this.menuhelp});
			this.menumain.Location = new System.Drawing.Point(0, 0);
			this.menumain.Name = "menumain";
			this.menumain.Size = new System.Drawing.Size(328, 24);
			this.menumain.ImageScalingSize = MainForm.ScaledIconSize;
			this.menumain.TabIndex = 0;
			// 
			// menufile
			// 
			this.menufile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.itemnewmap,
			this.itemopenmap,
			this.itemopenmapincurwad,
			this.itemclosemap,
			this.seperatorfileopen,
			this.itemsavemap,
			this.itemsavemapas,
			this.itemsavemapinto,
			this.seperatorfilesave,
			this.itemimport,
			this.itemexport,
			this.separatorio,
			this.itemnorecent,
			this.seperatorfilerecent,
			this.itemexit});
			this.menufile.Name = "menufile";
			this.menufile.Size = new System.Drawing.Size(37, 20);
			this.menufile.Text = "&File";
			this.menufile.DropDownOpening += menufile_DropDownOpening;
			// 
			// itemnewmap
			// 
			this.itemnewmap.Image = global::CodeImp.DoomBuilder.Properties.Resources.File;
			this.itemnewmap.Name = "itemnewmap";
			this.itemnewmap.ShortcutKeyDisplayString = "";
			this.itemnewmap.Size = new System.Drawing.Size(225, 22);
			this.itemnewmap.Tag = "builder_newmap";
			this.itemnewmap.Text = "&New Map";
			this.itemnewmap.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// itemopenmap
			// 
			this.itemopenmap.Image = global::CodeImp.DoomBuilder.Properties.Resources.OpenMap;
			this.itemopenmap.Name = "itemopenmap";
			this.itemopenmap.Size = new System.Drawing.Size(225, 22);
			this.itemopenmap.Tag = "builder_openmap";
			this.itemopenmap.Text = "&Open Map...";
			this.itemopenmap.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// itemopenmapincurwad
			// 
			this.itemopenmapincurwad.Name = "itemopenmapincurwad";
			this.itemopenmapincurwad.Size = new System.Drawing.Size(225, 22);
			this.itemopenmapincurwad.Tag = "builder_openmapincurrentwad";
			this.itemopenmapincurwad.Text = "Open Map in Current &WAD...";
			this.itemopenmapincurwad.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// itemclosemap
			// 
			this.itemclosemap.Name = "itemclosemap";
			this.itemclosemap.Size = new System.Drawing.Size(225, 22);
			this.itemclosemap.Tag = "builder_closemap";
			this.itemclosemap.Text = "&Close Map";
			this.itemclosemap.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// itemsavemap
			// 
			this.itemsavemap.Image = global::CodeImp.DoomBuilder.Properties.Resources.SaveMap;
			this.itemsavemap.Name = "itemsavemap";
			this.itemsavemap.Size = new System.Drawing.Size(225, 22);
			this.itemsavemap.Tag = "builder_savemap";
			this.itemsavemap.Text = "&Save Map";
			this.itemsavemap.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// itemsavemapas
			// 
			this.itemsavemapas.Name = "itemsavemapas";
			this.itemsavemapas.Size = new System.Drawing.Size(225, 22);
			this.itemsavemapas.Tag = "builder_savemapas";
			this.itemsavemapas.Text = "Save Map &As...";
			this.itemsavemapas.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// itemsavemapinto
			// 
			this.itemsavemapinto.Name = "itemsavemapinto";
			this.itemsavemapinto.Size = new System.Drawing.Size(225, 22);
			this.itemsavemapinto.Tag = "builder_savemapinto";
			this.itemsavemapinto.Text = "Save Map &Into...";
			this.itemsavemapinto.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// seperatorfilesave
			// 
			this.seperatorfilesave.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
			this.seperatorfilesave.Name = "seperatorfilesave";
			this.seperatorfilesave.Size = new System.Drawing.Size(222, 6);
			// 
			// itemimport
			// 
			this.itemimport.Name = "itemimport";
			this.itemimport.Size = new System.Drawing.Size(225, 22);
			this.itemimport.Text = "Import";
			// 
			// itemexport
			// 
			this.itemexport.Name = "itemexport";
			this.itemexport.Size = new System.Drawing.Size(225, 22);
			this.itemexport.Text = "Export";
			// 
			// separatorio
			// 
			this.separatorio.Name = "separatorio";
			this.separatorio.Size = new System.Drawing.Size(222, 6);
			// 
			// itemnorecent
			// 
			this.itemnorecent.Enabled = false;
			this.itemnorecent.Name = "itemnorecent";
			this.itemnorecent.Size = new System.Drawing.Size(225, 22);
			this.itemnorecent.Text = "No recently opened files";
			// 
			// itemexit
			// 
			this.itemexit.Name = "itemexit";
			this.itemexit.Size = new System.Drawing.Size(225, 22);
			this.itemexit.Text = "E&xit";
			this.itemexit.Click += new System.EventHandler(this.itemexit_Click);
			// 
			// menuedit
			// 
			this.menuedit.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.itemundo,
			this.itemredo,
			this.seperatoreditundo,
			this.itemcut,
			this.itemcopy,
			this.itempaste,
			this.itempastespecial,
			this.seperatoreditcopypaste,
			this.itemmergegeoclassic,
			this.itemmergegeo,
			this.itemreplacegeo,
			this.separatorgeomerge,
			this.itemsnaptogrid,
			this.itemdynamicgridsize,
			this.itemautomerge,
			this.itemsplitjoinedsectors,
			this.itemautoclearsidetextures,
			this.seperatoreditgeometry,
			this.itemgridinc,
			this.itemgriddec,
			this.itemdosnaptogrid,
			this.itemaligngridtolinedef,
			this.itemsetgridorigintovertex,
			this.itemresetgrid,
			this.itemgridsetup,
			this.toolStripSeparator5,
			this.addToGroup,
			this.selectGroup,
			this.clearGroup,
			this.seperatoreditgrid,
			this.itemmapoptions,
			this.itemviewusedtags,
			this.itemviewthingtypes});
			this.menuedit.Name = "menuedit";
			this.menuedit.Size = new System.Drawing.Size(39, 20);
			this.menuedit.Text = "&Edit";
			this.menuedit.DropDownOpening += new System.EventHandler(this.menuedit_DropDownOpening);
			// 
			// itemundo
			// 
			this.itemundo.Image = global::CodeImp.DoomBuilder.Properties.Resources.Undo;
			this.itemundo.Name = "itemundo";
			this.itemundo.Size = new System.Drawing.Size(219, 22);
			this.itemundo.Tag = "builder_undo";
			this.itemundo.Text = "&Undo";
			this.itemundo.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// itemredo
			// 
			this.itemredo.Image = global::CodeImp.DoomBuilder.Properties.Resources.Redo;
			this.itemredo.Name = "itemredo";
			this.itemredo.Size = new System.Drawing.Size(219, 22);
			this.itemredo.Tag = "builder_redo";
			this.itemredo.Text = "&Redo";
			this.itemredo.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// seperatoreditundo
			// 
			this.seperatoreditundo.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
			this.seperatoreditundo.Name = "seperatoreditundo";
			this.seperatoreditundo.Size = new System.Drawing.Size(216, 6);
			// 
			// itemcut
			// 
			this.itemcut.Image = global::CodeImp.DoomBuilder.Properties.Resources.Cut;
			this.itemcut.Name = "itemcut";
			this.itemcut.Size = new System.Drawing.Size(219, 22);
			this.itemcut.Tag = "builder_cutselection";
			this.itemcut.Text = "Cu&t";
			this.itemcut.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// itemcopy
			// 
			this.itemcopy.Image = global::CodeImp.DoomBuilder.Properties.Resources.Copy;
			this.itemcopy.Name = "itemcopy";
			this.itemcopy.Size = new System.Drawing.Size(219, 22);
			this.itemcopy.Tag = "builder_copyselection";
			this.itemcopy.Text = "&Copy";
			this.itemcopy.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// itempaste
			// 
			this.itempaste.Image = global::CodeImp.DoomBuilder.Properties.Resources.Paste;
			this.itempaste.Name = "itempaste";
			this.itempaste.Size = new System.Drawing.Size(219, 22);
			this.itempaste.Tag = "builder_pasteselection";
			this.itempaste.Text = "&Paste";
			this.itempaste.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// itempastespecial
			// 
			this.itempastespecial.Image = global::CodeImp.DoomBuilder.Properties.Resources.PasteSpecial;
			this.itempastespecial.Name = "itempastespecial";
			this.itempastespecial.Size = new System.Drawing.Size(219, 22);
			this.itempastespecial.Tag = "builder_pasteselectionspecial";
			this.itempastespecial.Text = "Paste Special...";
			this.itempastespecial.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// itemsnaptogrid
			// 
			this.itemsnaptogrid.Checked = true;
			this.itemsnaptogrid.CheckState = System.Windows.Forms.CheckState.Checked;
			this.itemsnaptogrid.Image = global::CodeImp.DoomBuilder.Properties.Resources.Grid4;
			this.itemsnaptogrid.Name = "itemsnaptogrid";
			this.itemsnaptogrid.Size = new System.Drawing.Size(219, 22);
			this.itemsnaptogrid.Tag = "builder_togglesnap";
			this.itemsnaptogrid.Text = "&Snap to Grid";
			this.itemsnaptogrid.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// itemautomerge
			// 
			this.itemautomerge.Checked = true;
			this.itemautomerge.CheckState = System.Windows.Forms.CheckState.Checked;
			this.itemautomerge.Image = global::CodeImp.DoomBuilder.Properties.Resources.mergegeometry2;
			this.itemautomerge.Name = "itemautomerge";
			this.itemautomerge.Size = new System.Drawing.Size(219, 22);
			this.itemautomerge.Tag = "builder_toggleautomerge";
			this.itemautomerge.Text = "Snap to &Geometry";
			this.itemautomerge.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// itemsplitjoinedsectors
			// 
			this.itemsplitjoinedsectors.Checked = true;
			this.itemsplitjoinedsectors.CheckState = System.Windows.Forms.CheckState.Checked;
			this.itemsplitjoinedsectors.Image = global::CodeImp.DoomBuilder.Properties.Resources.SplitSectors;
			this.itemsplitjoinedsectors.Name = "itemsplitjoinedsectors";
			this.itemsplitjoinedsectors.Size = new System.Drawing.Size(219, 22);
			this.itemsplitjoinedsectors.Tag = "builder_togglejoinedsectorssplitting";
			this.itemsplitjoinedsectors.Text = "Split &Joined Sectors when Drawing Lines";
			this.itemsplitjoinedsectors.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// itemautoclearsidetextures
			// 
			this.itemautoclearsidetextures.Checked = true;
			this.itemautoclearsidetextures.CheckState = System.Windows.Forms.CheckState.Checked;
			this.itemautoclearsidetextures.Image = global::CodeImp.DoomBuilder.Properties.Resources.ClearTextures;
			this.itemautoclearsidetextures.Name = "itemautoclearsidetextures";
			this.itemautoclearsidetextures.Size = new System.Drawing.Size(219, 22);
			this.itemautoclearsidetextures.Tag = "builder_toggleautoclearsidetextures";
			this.itemautoclearsidetextures.Text = "&Auto Clear Sidedef Textures";
			this.itemautoclearsidetextures.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// seperatoreditgeometry
			// 
			this.seperatoreditgeometry.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
			this.seperatoreditgeometry.Name = "seperatoreditgeometry";
			this.seperatoreditgeometry.Size = new System.Drawing.Size(216, 6);
			// 
			// itemgridinc
			// 
			this.itemgridinc.Image = global::CodeImp.DoomBuilder.Properties.Resources.GridIncrease;
			this.itemgridinc.Name = "itemgridinc";
			this.itemgridinc.Size = new System.Drawing.Size(219, 22);
			this.itemgridinc.Tag = "builder_griddec";
			this.itemgridinc.Text = "&Increase Grid Size";
			this.itemgridinc.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// itemgriddec
			// 
			this.itemgriddec.Image = global::CodeImp.DoomBuilder.Properties.Resources.GridDecrease;
			this.itemgriddec.Name = "itemgriddec";
			this.itemgriddec.Size = new System.Drawing.Size(219, 22);
			this.itemgriddec.Tag = "builder_gridinc";
			this.itemgriddec.Text = "&Decrease Grid Size";
			this.itemgriddec.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// itemdosnaptogrid
			// 
			this.itemdosnaptogrid.Image = global::CodeImp.DoomBuilder.Properties.Resources.SnapVerts;
			this.itemdosnaptogrid.Name = "itemdosnaptogrid";
			this.itemdosnaptogrid.Size = new System.Drawing.Size(219, 22);
			this.itemdosnaptogrid.Tag = "builder_snapvertstogrid";
			this.itemdosnaptogrid.Text = "Snap Selection to Grid";
			this.itemdosnaptogrid.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// itemgridsetup
			// 
			this.itemgridsetup.Image = global::CodeImp.DoomBuilder.Properties.Resources.Grid2;
			this.itemgridsetup.Name = "itemgridsetup";
			this.itemgridsetup.Size = new System.Drawing.Size(219, 22);
			this.itemgridsetup.Tag = "builder_gridsetup";
			this.itemgridsetup.Text = "&Grid and Backdrop Setup...";
			this.itemgridsetup.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// toolStripSeparator5
			// 
			this.toolStripSeparator5.Name = "toolStripSeparator5";
			this.toolStripSeparator5.Size = new System.Drawing.Size(216, 6);
			// 
			// addToGroup
			// 
			this.addToGroup.Image = global::CodeImp.DoomBuilder.Properties.Resources.GroupAdd;
			this.addToGroup.Name = "addToGroup";
			this.addToGroup.Size = new System.Drawing.Size(219, 22);
			this.addToGroup.Text = "Add Selection to Group";
			// 
			// selectGroup
			// 
			this.selectGroup.Image = global::CodeImp.DoomBuilder.Properties.Resources.Group;
			this.selectGroup.Name = "selectGroup";
			this.selectGroup.Size = new System.Drawing.Size(219, 22);
			this.selectGroup.Text = "Select Group";
			// 
			// clearGroup
			// 
			this.clearGroup.Image = global::CodeImp.DoomBuilder.Properties.Resources.GroupRemove;
			this.clearGroup.Name = "clearGroup";
			this.clearGroup.Size = new System.Drawing.Size(219, 22);
			this.clearGroup.Text = "Clear Group";
			// 
			// itemmapoptions
			// 
			this.itemmapoptions.Image = global::CodeImp.DoomBuilder.Properties.Resources.Properties;
			this.itemmapoptions.Name = "itemmapoptions";
			this.itemmapoptions.Size = new System.Drawing.Size(219, 22);
			this.itemmapoptions.Tag = "builder_mapoptions";
			this.itemmapoptions.Text = "Map &Options...";
			this.itemmapoptions.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// itemviewusedtags
			// 
			this.itemviewusedtags.Image = global::CodeImp.DoomBuilder.Properties.Resources.TagStatistics;
			this.itemviewusedtags.Name = "itemviewusedtags";
			this.itemviewusedtags.Size = new System.Drawing.Size(219, 22);
			this.itemviewusedtags.Tag = "builder_viewusedtags";
			this.itemviewusedtags.Text = "View Used Tags...";
			this.itemviewusedtags.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// itemviewthingtypes
			// 
			this.itemviewthingtypes.Image = global::CodeImp.DoomBuilder.Properties.Resources.ThingStatistics;
			this.itemviewthingtypes.Name = "itemviewthingtypes";
			this.itemviewthingtypes.Size = new System.Drawing.Size(219, 22);
			this.itemviewthingtypes.Tag = "builder_viewthingtypes";
			this.itemviewthingtypes.Text = "View Thing Types...";
			this.itemviewthingtypes.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// menuview
			// 
			this.menuview.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.itemthingsfilter,
			this.itemlinedefcolors,
			this.seperatorviewthings,
			this.itemviewnormal,
			this.itemviewbrightness,
			this.itemviewfloors,
			this.itemviewceilings,
			this.seperatorviewviews,
			this.itemfullbrightness,
			this.itemtogglegrid,
			this.itemtogglecomments,
			this.itemtogglefixedthingsscale,
			this.itemtogglealwaysshowvertices,
			this.separatorrendering,
			this.itemdynlightmodes,
			this.itemmodelmodes,
			this.itemtogglefog,
			this.itemtogglesky,
			this.itemtoggleclassicrendering,
			this.itemtoggleeventlines,
			this.itemtogglevisualverts,
			this.separatorhelpers,
			this.menuzoom,
			this.itemgotocoords,
			this.itemfittoscreen,
			this.itemtoggleinfo,
			this.seperatorviewzoom,
			this.itemscripteditor});
			this.menuview.Name = "menuview";
			this.menuview.Size = new System.Drawing.Size(44, 20);
			this.menuview.Text = "&View";
			// 
			// itemthingsfilter
			// 
			this.itemthingsfilter.Image = global::CodeImp.DoomBuilder.Properties.Resources.Filter;
			this.itemthingsfilter.Name = "itemthingsfilter";
			this.itemthingsfilter.Size = new System.Drawing.Size(215, 22);
			this.itemthingsfilter.Tag = "builder_thingsfilterssetup";
			this.itemthingsfilter.Text = "Configure &Things Filters...";
			this.itemthingsfilter.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// itemlinedefcolors
			// 
			this.itemlinedefcolors.Image = global::CodeImp.DoomBuilder.Properties.Resources.LinedefColorPresets;
			this.itemlinedefcolors.Name = "itemlinedefcolors";
			this.itemlinedefcolors.Size = new System.Drawing.Size(215, 22);
			this.itemlinedefcolors.Tag = "builder_linedefcolorssetup";
			this.itemlinedefcolors.Text = "Configure &Linedef Colors...";
			this.itemlinedefcolors.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// seperatorviewthings
			// 
			this.seperatorviewthings.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
			this.seperatorviewthings.Name = "seperatorviewthings";
			this.seperatorviewthings.Size = new System.Drawing.Size(212, 6);
			// 
			// itemviewnormal
			// 
			this.itemviewnormal.Image = global::CodeImp.DoomBuilder.Properties.Resources.ViewNormal;
			this.itemviewnormal.Name = "itemviewnormal";
			this.itemviewnormal.Size = new System.Drawing.Size(215, 22);
			this.itemviewnormal.Tag = "builder_viewmodenormal";
			this.itemviewnormal.Text = "&Wireframe";
			this.itemviewnormal.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// itemviewbrightness
			// 
			this.itemviewbrightness.Image = global::CodeImp.DoomBuilder.Properties.Resources.ViewBrightness;
			this.itemviewbrightness.Name = "itemviewbrightness";
			this.itemviewbrightness.Size = new System.Drawing.Size(215, 22);
			this.itemviewbrightness.Tag = "builder_viewmodebrightness";
			this.itemviewbrightness.Text = "&Brightness Levels";
			this.itemviewbrightness.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// itemviewfloors
			// 
			this.itemviewfloors.Image = global::CodeImp.DoomBuilder.Properties.Resources.ViewTextureFloor;
			this.itemviewfloors.Name = "itemviewfloors";
			this.itemviewfloors.Size = new System.Drawing.Size(215, 22);
			this.itemviewfloors.Tag = "builder_viewmodefloors";
			this.itemviewfloors.Text = "&Floor Textures";
			this.itemviewfloors.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// itemviewceilings
			// 
			this.itemviewceilings.Image = global::CodeImp.DoomBuilder.Properties.Resources.ViewTextureCeiling;
			this.itemviewceilings.Name = "itemviewceilings";
			this.itemviewceilings.Size = new System.Drawing.Size(215, 22);
			this.itemviewceilings.Tag = "builder_viewmodeceilings";
			this.itemviewceilings.Text = "&Ceiling Textures";
			this.itemviewceilings.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// seperatorviewviews
			// 
			this.seperatorviewviews.Name = "seperatorviewviews";
			this.seperatorviewviews.Size = new System.Drawing.Size(212, 6);
			// 
			// itemmergegeoclassic
			// 
			this.itemmergegeoclassic.Image = global::CodeImp.DoomBuilder.Properties.Resources.MergeGeoClassic;
			this.itemmergegeoclassic.Name = "itemmergegeoclassic";
			this.itemmergegeoclassic.Size = new System.Drawing.Size(215, 22);
			this.itemmergegeoclassic.Tag = "builder_geomergeclassic";
			this.itemmergegeoclassic.Text = "Merge Dragged Vertices Only";
			this.itemmergegeoclassic.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// itemmergegeo
			// 
			this.itemmergegeo.Image = global::CodeImp.DoomBuilder.Properties.Resources.MergeGeo;
			this.itemmergegeo.Name = "itemmergegeo";
			this.itemmergegeo.Size = new System.Drawing.Size(215, 22);
			this.itemmergegeo.Tag = "builder_geomerge";
			this.itemmergegeo.Text = "Merge Dragged Geometry";
			this.itemmergegeo.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// itemreplacegeo
			// 
			this.itemreplacegeo.Image = global::CodeImp.DoomBuilder.Properties.Resources.MergeGeoRemoveLines;
			this.itemreplacegeo.Name = "itemreplacegeo";
			this.itemreplacegeo.Size = new System.Drawing.Size(215, 22);
			this.itemreplacegeo.Tag = "builder_georeplace";
			this.itemreplacegeo.Text = "Replace with Dragged Geometry";
			this.itemreplacegeo.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// separatorgeomerge
			// 
			this.separatorgeomerge.Name = "separatorgeomerge";
			this.separatorgeomerge.Size = new System.Drawing.Size(212, 6);
			// 
			// itemfullbrightness
			// 
			this.itemfullbrightness.Checked = true;
			this.itemfullbrightness.CheckOnClick = true;
			this.itemfullbrightness.CheckState = System.Windows.Forms.CheckState.Checked;
			this.itemfullbrightness.Image = global::CodeImp.DoomBuilder.Properties.Resources.Brightness;
			this.itemfullbrightness.Name = "itemfullbrightness";
			this.itemfullbrightness.Size = new System.Drawing.Size(215, 22);
			this.itemfullbrightness.Tag = "builder_togglebrightness";
			this.itemfullbrightness.Text = "Full Brightness";
			this.itemfullbrightness.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// itemtogglegrid
			// 
			this.itemtogglegrid.Checked = true;
			this.itemtogglegrid.CheckOnClick = true;
			this.itemtogglegrid.CheckState = System.Windows.Forms.CheckState.Checked;
			this.itemtogglegrid.Image = global::CodeImp.DoomBuilder.Properties.Resources.Grid2;
			this.itemtogglegrid.Name = "itemtogglegrid";
			this.itemtogglegrid.Size = new System.Drawing.Size(215, 22);
			this.itemtogglegrid.Tag = "builder_togglegrid";
			this.itemtogglegrid.Text = "&Render Grid";
			this.itemtogglegrid.Click += new System.EventHandler(this.InvokeTaggedAction);

			// 
			// itemaligngridtolinedef
			//
			this.itemaligngridtolinedef.Name = "itemaligngridtolinedef";
			this.itemaligngridtolinedef.Size = new System.Drawing.Size(215, 22);
			this.itemaligngridtolinedef.Tag = "builder_aligngridtolinedef";
			this.itemaligngridtolinedef.Text = "Align Grid To Selected Linedef";
			this.itemaligngridtolinedef.Click += new System.EventHandler(this.InvokeTaggedAction);

			// 
			// itemsetgridorigintovertex
			//
			this.itemsetgridorigintovertex.Name = "itemsetgridorigintovertex";
			this.itemsetgridorigintovertex.Size = new System.Drawing.Size(215, 22);
			this.itemsetgridorigintovertex.Tag = "builder_setgridorigintovertex";
			this.itemsetgridorigintovertex.Text = "Set Grid Origin To Selected Vertex";
			this.itemsetgridorigintovertex.Click += new System.EventHandler(this.InvokeTaggedAction);

			// 
			// itemresetgrid
			//
			this.itemresetgrid.Name = "itemresetgrid";
			this.itemresetgrid.Size = new System.Drawing.Size(215, 22);
			this.itemresetgrid.Tag = "builder_resetgrid";
			this.itemresetgrid.Text = "Reset Grid Transform";
			this.itemresetgrid.Click += new System.EventHandler(this.InvokeTaggedAction);

			// 
			// toolStripSeparator4
			// 
			this.separatorhelpers.Name = "separatorhelpers";
			this.separatorhelpers.Size = new System.Drawing.Size(212, 6);
			// 
			// menuzoom
			// 
			this.menuzoom.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.item2zoom800,
			this.item2zoom400,
			this.item2zoom200,
			this.item2zoom100,
			this.item2zoom50,
			this.item2zoom25,
			this.item2zoom10,
			this.item2zoom5});
			this.menuzoom.Image = global::CodeImp.DoomBuilder.Properties.Resources.Zoom;
			this.menuzoom.Name = "menuzoom";
			this.menuzoom.Size = new System.Drawing.Size(215, 22);
			this.menuzoom.Text = "&Zoom";
			// 
			// item2zoom800
			// 
			this.item2zoom800.Name = "item2zoom800";
			this.item2zoom800.Size = new System.Drawing.Size(102, 22);
			this.item2zoom800.Tag = "800";
			this.item2zoom800.Text = "800%";
			this.item2zoom800.Click += new System.EventHandler(this.itemzoomto_Click);
			// 
			// item2zoom400
			// 
			this.item2zoom400.Name = "item2zoom400";
			this.item2zoom400.Size = new System.Drawing.Size(102, 22);
			this.item2zoom400.Tag = "400";
			this.item2zoom400.Text = "400%";
			this.item2zoom400.Click += new System.EventHandler(this.itemzoomto_Click);
			// 
			// item2zoom200
			// 
			this.item2zoom200.Name = "item2zoom200";
			this.item2zoom200.Size = new System.Drawing.Size(102, 22);
			this.item2zoom200.Tag = "200";
			this.item2zoom200.Text = "200%";
			this.item2zoom200.Click += new System.EventHandler(this.itemzoomto_Click);
			// 
			// item2zoom100
			// 
			this.item2zoom100.Name = "item2zoom100";
			this.item2zoom100.Size = new System.Drawing.Size(102, 22);
			this.item2zoom100.Tag = "100";
			this.item2zoom100.Text = "100%";
			this.item2zoom100.Click += new System.EventHandler(this.itemzoomto_Click);
			// 
			// item2zoom50
			// 
			this.item2zoom50.Name = "item2zoom50";
			this.item2zoom50.Size = new System.Drawing.Size(102, 22);
			this.item2zoom50.Tag = "50";
			this.item2zoom50.Text = "50%";
			this.item2zoom50.Click += new System.EventHandler(this.itemzoomto_Click);
			// 
			// item2zoom25
			// 
			this.item2zoom25.Name = "item2zoom25";
			this.item2zoom25.Size = new System.Drawing.Size(102, 22);
			this.item2zoom25.Tag = "25";
			this.item2zoom25.Text = "25%";
			this.item2zoom25.Click += new System.EventHandler(this.itemzoomto_Click);
			// 
			// item2zoom10
			// 
			this.item2zoom10.Name = "item2zoom10";
			this.item2zoom10.Size = new System.Drawing.Size(102, 22);
			this.item2zoom10.Tag = "10";
			this.item2zoom10.Text = "10%";
			this.item2zoom10.Click += new System.EventHandler(this.itemzoomto_Click);
			// 
			// item2zoom5
			// 
			this.item2zoom5.Name = "item2zoom5";
			this.item2zoom5.Size = new System.Drawing.Size(102, 22);
			this.item2zoom5.Tag = "5";
			this.item2zoom5.Text = "5%";
			this.item2zoom5.Click += new System.EventHandler(this.itemzoomto_Click);
			// 
			// itemgotocoords
			// 
			this.itemgotocoords.Name = "itemgotocoords";
			this.itemgotocoords.Size = new System.Drawing.Size(215, 22);
			this.itemgotocoords.Tag = "builder_centeroncoordinates";
			this.itemgotocoords.Text = "Go To Coordinates...";
			this.itemgotocoords.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// itemfittoscreen
			// 
			this.itemfittoscreen.Name = "itemfittoscreen";
			this.itemfittoscreen.Size = new System.Drawing.Size(215, 22);
			this.itemfittoscreen.Tag = "builder_centerinscreen";
			this.itemfittoscreen.Text = "Fit to Screen";
			this.itemfittoscreen.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// itemtoggleinfo
			// 
			this.itemtoggleinfo.Name = "itemtoggleinfo";
			this.itemtoggleinfo.Size = new System.Drawing.Size(215, 22);
			this.itemtoggleinfo.Tag = "builder_toggleinfopanel";
			this.itemtoggleinfo.Text = "&Expanded Info Panel";
			this.itemtoggleinfo.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// seperatorviewzoom
			// 
			this.seperatorviewzoom.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
			this.seperatorviewzoom.Name = "seperatorviewzoom";
			this.seperatorviewzoom.Size = new System.Drawing.Size(212, 6);
			// 
			// itemscripteditor
			// 
			this.itemscripteditor.Image = global::CodeImp.DoomBuilder.Properties.Resources.Script2;
			this.itemscripteditor.Name = "itemscripteditor";
			this.itemscripteditor.Size = new System.Drawing.Size(215, 22);
			this.itemscripteditor.Tag = "builder_openscripteditor";
			this.itemscripteditor.Text = "&Script Editor...";
			this.itemscripteditor.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// menumode
			// 
			this.menumode.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.separatorDrawModes,
			this.separatorTransformModes});
			this.menumode.Name = "menumode";
			this.menumode.Size = new System.Drawing.Size(50, 20);
			this.menumode.Text = "&Mode";
			// 
			// separatorDrawModes
			// 
			this.separatorDrawModes.Name = "separatorDrawModes";
			this.separatorDrawModes.Size = new System.Drawing.Size(57, 6);
			// 
			// separatorTransformModes
			// 
			this.separatorTransformModes.Name = "separatorTransformModes";
			this.separatorTransformModes.Size = new System.Drawing.Size(57, 6);
			// 
			// menuprefabs
			// 
			this.menuprefabs.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.iteminsertprefabfile,
			this.iteminsertpreviousprefab,
			this.seperatorprefabsinsert,
			this.itemcreateprefab});
			this.menuprefabs.Name = "menuprefabs";
			this.menuprefabs.Size = new System.Drawing.Size(58, 20);
			this.menuprefabs.Text = "&Prefabs";
			// 
			// iteminsertprefabfile
			// 
			this.iteminsertprefabfile.Image = global::CodeImp.DoomBuilder.Properties.Resources.Prefab;
			this.iteminsertprefabfile.Name = "iteminsertprefabfile";
			this.iteminsertprefabfile.Size = new System.Drawing.Size(199, 22);
			this.iteminsertprefabfile.Tag = "builder_insertprefabfile";
			this.iteminsertprefabfile.Text = "&Insert Prefab from File...";
			this.iteminsertprefabfile.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// iteminsertpreviousprefab
			// 
			this.iteminsertpreviousprefab.Image = global::CodeImp.DoomBuilder.Properties.Resources.Prefab2;
			this.iteminsertpreviousprefab.Name = "iteminsertpreviousprefab";
			this.iteminsertpreviousprefab.Size = new System.Drawing.Size(199, 22);
			this.iteminsertpreviousprefab.Tag = "builder_insertpreviousprefab";
			this.iteminsertpreviousprefab.Text = "Insert &Previous Prefab";
			this.iteminsertpreviousprefab.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// seperatorprefabsinsert
			// 
			this.seperatorprefabsinsert.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
			this.seperatorprefabsinsert.Name = "seperatorprefabsinsert";
			this.seperatorprefabsinsert.Size = new System.Drawing.Size(196, 6);
			// 
			// itemcreateprefab
			// 
			this.itemcreateprefab.Name = "itemcreateprefab";
			this.itemcreateprefab.Size = new System.Drawing.Size(199, 22);
			this.itemcreateprefab.Tag = "builder_createprefab";
			this.itemcreateprefab.Text = "&Create From Selection...";
			this.itemcreateprefab.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// menutools
			// 
			this.menutools.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.itemreloadresources,
			this.itemReloadModedef,
			this.itemReloadGldefs,
			this.itemshowerrors,
			this.seperatortoolsresources,
			this.configurationToolStripMenuItem,
			this.preferencesToolStripMenuItem,
			this.seperatortoolsconfig,
			this.itemsavescreenshot,
			this.itemsaveeditareascreenshot,
			this.separatortoolsscreenshots,
			this.itemtestmap});
			this.menutools.Name = "menutools";
			this.menutools.Size = new System.Drawing.Size(48, 20);
			this.menutools.Text = "&Tools";
			// 
			// itemreloadresources
			// 
			this.itemreloadresources.Image = global::CodeImp.DoomBuilder.Properties.Resources.Reload;
			this.itemreloadresources.Name = "itemreloadresources";
			this.itemreloadresources.Size = new System.Drawing.Size(246, 22);
			this.itemreloadresources.Tag = "builder_reloadresources";
			this.itemreloadresources.Text = "&Reload Resources";
			this.itemreloadresources.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// itemReloadModedef
			// 
			this.itemReloadModedef.Image = global::CodeImp.DoomBuilder.Properties.Resources.Reload;
			this.itemReloadModedef.Name = "itemReloadModedef";
			this.itemReloadModedef.Size = new System.Drawing.Size(246, 22);
			this.itemReloadModedef.Tag = "builder_gzreloadmodeldef";
			this.itemReloadModedef.Text = "Reload MODELDEF/VOXELDEF";
			this.itemReloadModedef.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// itemReloadGldefs
			// 
			this.itemReloadGldefs.Image = global::CodeImp.DoomBuilder.Properties.Resources.Reload;
			this.itemReloadGldefs.Name = "itemReloadGldefs";
			this.itemReloadGldefs.Size = new System.Drawing.Size(246, 22);
			this.itemReloadGldefs.Tag = "builder_gzreloadgldefs";
			this.itemReloadGldefs.Text = "Reload GLDEFS";
			this.itemReloadGldefs.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// itemshowerrors
			// 
			this.itemshowerrors.Image = global::CodeImp.DoomBuilder.Properties.Resources.Warning;
			this.itemshowerrors.Name = "itemshowerrors";
			this.itemshowerrors.Size = new System.Drawing.Size(246, 22);
			this.itemshowerrors.Tag = "builder_showerrors";
			this.itemshowerrors.Text = "&Errors and Warnings...";
			this.itemshowerrors.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// seperatortoolsresources
			// 
			this.seperatortoolsresources.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
			this.seperatortoolsresources.Name = "seperatortoolsresources";
			this.seperatortoolsresources.Size = new System.Drawing.Size(243, 6);
			// 
			// configurationToolStripMenuItem
			// 
			this.configurationToolStripMenuItem.Image = global::CodeImp.DoomBuilder.Properties.Resources.Configuration;
			this.configurationToolStripMenuItem.Name = "configurationToolStripMenuItem";
			this.configurationToolStripMenuItem.Size = new System.Drawing.Size(246, 22);
			this.configurationToolStripMenuItem.Tag = "builder_configuration";
			this.configurationToolStripMenuItem.Text = "&Game Configurations...";
			this.configurationToolStripMenuItem.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// preferencesToolStripMenuItem
			// 
			this.preferencesToolStripMenuItem.Image = global::CodeImp.DoomBuilder.Properties.Resources.Preferences;
			this.preferencesToolStripMenuItem.Name = "preferencesToolStripMenuItem";
			this.preferencesToolStripMenuItem.Size = new System.Drawing.Size(246, 22);
			this.preferencesToolStripMenuItem.Tag = "builder_preferences";
			this.preferencesToolStripMenuItem.Text = "Preferences...";
			this.preferencesToolStripMenuItem.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// seperatortoolsconfig
			// 
			this.seperatortoolsconfig.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
			this.seperatortoolsconfig.Name = "seperatortoolsconfig";
			this.seperatortoolsconfig.Size = new System.Drawing.Size(243, 6);
			// 
			// itemsavescreenshot
			// 
			this.itemsavescreenshot.Image = global::CodeImp.DoomBuilder.Properties.Resources.Screenshot;
			this.itemsavescreenshot.Name = "itemsavescreenshot";
			this.itemsavescreenshot.Size = new System.Drawing.Size(246, 22);
			this.itemsavescreenshot.Tag = "builder_savescreenshot";
			this.itemsavescreenshot.Text = "Save Screenshot";
			this.itemsavescreenshot.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// itemsaveeditareascreenshot
			// 
			this.itemsaveeditareascreenshot.Image = global::CodeImp.DoomBuilder.Properties.Resources.ScreenshotActiveWindow;
			this.itemsaveeditareascreenshot.Name = "itemsaveeditareascreenshot";
			this.itemsaveeditareascreenshot.Size = new System.Drawing.Size(246, 22);
			this.itemsaveeditareascreenshot.Tag = "builder_saveeditareascreenshot";
			this.itemsaveeditareascreenshot.Text = "Save Screenshot (active window)";
			this.itemsaveeditareascreenshot.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// separatortoolsscreenshots
			// 
			this.separatortoolsscreenshots.Name = "separatortoolsscreenshots";
			this.separatortoolsscreenshots.Size = new System.Drawing.Size(243, 6);
			// 
			// itemtestmap
			// 
			this.itemtestmap.Image = global::CodeImp.DoomBuilder.Properties.Resources.Test;
			this.itemtestmap.Name = "itemtestmap";
			this.itemtestmap.Size = new System.Drawing.Size(246, 22);
			this.itemtestmap.Tag = "builder_testmap";
			this.itemtestmap.Text = "&Test Map";
			this.itemtestmap.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// menuhelp
			// 
			this.menuhelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.itemhelprefmanual,
			this.itemShortcutReference,
			this.itemopenconfigfolder,
			this.itemhelpeditmode,
			this.itemhelpissues,
			this.itemhelpcheckupdates,
			this.seperatorhelpmanual,
			this.itemhelpabout});
			this.menuhelp.Name = "menuhelp";
			this.menuhelp.Size = new System.Drawing.Size(44, 20);
			this.menuhelp.Text = "&Help";
			// 
			// itemhelprefmanual
			// 
			this.itemhelprefmanual.Image = global::CodeImp.DoomBuilder.Properties.Resources.Help;
			this.itemhelprefmanual.Name = "itemhelprefmanual";
			this.itemhelprefmanual.Size = new System.Drawing.Size(232, 22);
			this.itemhelprefmanual.Text = "Reference &Manual";
			this.itemhelprefmanual.Click += new System.EventHandler(this.itemhelprefmanual_Click);
			// 
			// itemShortcutReference
			// 
			this.itemShortcutReference.Image = global::CodeImp.DoomBuilder.Properties.Resources.Keyboard;
			this.itemShortcutReference.Name = "itemShortcutReference";
			this.itemShortcutReference.Size = new System.Drawing.Size(232, 22);
			this.itemShortcutReference.Tag = "";
			this.itemShortcutReference.Text = "Keyboard Shortcuts Reference";
			this.itemShortcutReference.Click += new System.EventHandler(this.itemShortcutReference_Click);
			// 
			// itemopenconfigfolder
			//
			this.itemopenconfigfolder.Image = global::CodeImp.DoomBuilder.Properties.Resources.FolderExplore;
			this.itemopenconfigfolder.Name = "itemopenconfigfolder";
			this.itemopenconfigfolder.Size = new System.Drawing.Size(232, 22);
			this.itemopenconfigfolder.Tag = "";
			this.itemopenconfigfolder.Text = "Program Configuration Folder";
			this.itemopenconfigfolder.Click += new System.EventHandler(this.itemopenconfigfolder_Click);
			// 
			// itemhelpeditmode
			// 
			this.itemhelpeditmode.Image = global::CodeImp.DoomBuilder.Properties.Resources.Question;
			this.itemhelpeditmode.Name = "itemhelpeditmode";
			this.itemhelpeditmode.Size = new System.Drawing.Size(232, 22);
			this.itemhelpeditmode.Text = "About this &Editing Mode";
			this.itemhelpeditmode.Click += new System.EventHandler(this.itemhelpeditmode_Click);
			// 
			// itemhelpcheckupdates
			// 
			this.itemhelpcheckupdates.Image = global::CodeImp.DoomBuilder.Properties.Resources.Update;
			this.itemhelpcheckupdates.Name = "itemhelpcheckupdates";
			this.itemhelpcheckupdates.Size = new System.Drawing.Size(232, 22);
			this.itemhelpcheckupdates.Text = "&Check for updates...";
			this.itemhelpcheckupdates.Click += new System.EventHandler(this.itemhelpcheckupdates_Click);
			// 
			// seperatorhelpmanual
			// 
			this.seperatorhelpmanual.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
			this.seperatorhelpmanual.Name = "seperatorhelpmanual";
			this.seperatorhelpmanual.Size = new System.Drawing.Size(229, 6);
			// 
			// itemhelpissues
			// 
			this.itemhelpissues.Image = global::CodeImp.DoomBuilder.Properties.Resources.Github;
			this.itemhelpissues.Name = "itemhelpissues";
			this.itemhelpissues.Size = new System.Drawing.Size(232, 22);
			this.itemhelpissues.Text = "&GitHub issues tracker";
			this.itemhelpissues.Click += new System.EventHandler(this.itemhelpissues_Click);
			// 
			// itemhelpabout
			// 
			this.itemhelpabout.Image = global::CodeImp.DoomBuilder.Properties.Resources.About;
			this.itemhelpabout.Name = "itemhelpabout";
			this.itemhelpabout.Size = new System.Drawing.Size(232, 22);
			this.itemhelpabout.Text = "&About Ultimate Doom Builder...";
			this.itemhelpabout.Click += new System.EventHandler(this.itemhelpabout_Click);
			// 
			// toolbar
			// 
			this.toolbar.AutoSize = false;
			this.toolbar.ImageScalingSize = MainForm.ScaledIconSize;
			this.toolbar.ContextMenuStrip = this.toolbarContextMenu;
			this.toolbar.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.toolbar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.buttonnewmap,
			this.buttonopenmap,
			this.buttonsavemap,
			this.seperatorfile,
			this.buttonscripteditor,
			this.seperatorscript,
			this.buttonundo,
			this.buttonredo,
			this.seperatorundo,
			this.buttoncut,
			this.buttoncopy,
			this.buttonpaste,
			this.seperatorcopypaste,
			this.buttoninsertprefabfile,
			this.buttoninsertpreviousprefab,
			this.seperatorprefabs,
			this.buttonthingsfilter,
			this.thingfilters,
			this.separatorlinecolors,
			this.buttonlinededfcolors,
			this.linedefcolorpresets,
			this.separatorfilters,
			this.buttonfullbrightness,
			this.buttontogglegrid,
			this.buttontogglecomments,
			this.buttontogglefixedthingsscale,
			this.separatorfullbrightness,
			this.buttonviewnormal,
			this.buttonviewbrightness,
			this.buttonviewfloors,
			this.buttonviewceilings,
			this.separatorgeomergemodes,
			this.buttonmergegeoclassic,
			this.buttonmergegeo,
			this.buttonplacegeo,
			this.seperatorviews,
			this.buttonsnaptogrid,
			this.buttontoggledynamicgrid,
			this.buttonautomerge,
			this.buttonsplitjoinedsectors,
			this.buttonautoclearsidetextures,
			this.seperatorgeometry,
			this.dynamiclightmode,
			this.modelrendermode,
			this.buttontogglefog,
			this.buttontogglesky,
			this.buttontoggleclassicrendering,
			this.buttontoggleeventlines,
			this.buttontogglevisualvertices,
			this.separatorgzmodes,
			this.buttontest,
			this.seperatortesting});
			this.toolbar.Location = new System.Drawing.Point(0, 24);
			this.toolbar.Name = "toolbar";
			this.toolbar.Size = new System.Drawing.Size(1012, 25);
			this.toolbar.TabIndex = 1;
			// 
			// toolbarContextMenu
			// 
			this.toolbarContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.toggleFile,
			this.toggleScript,
			this.toggleUndo,
			this.toggleCopy,
			this.togglePrefabs,
			this.toggleFilter,
			this.toggleViewModes,
			this.toggleGeometry,
			this.toggleTesting,
			this.toggleRendering});
			this.toolbarContextMenu.Name = "toolbarContextMenu";
			this.toolbarContextMenu.Size = new System.Drawing.Size(227, 224);
			this.toolbarContextMenu.ImageScalingSize = MainForm.ScaledIconSize;
			this.toolbarContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.toolbarContextMenu_Opening);
			this.toolbarContextMenu.Closing += new System.Windows.Forms.ToolStripDropDownClosingEventHandler(this.toolbarContextMenu_Closing);
			// 
			// toggleFile
			// 
			this.toggleFile.Name = "toggleFile";
			this.toggleFile.Size = new System.Drawing.Size(226, 22);
			this.toggleFile.Text = "New / Open / Save";
			this.toggleFile.Click += new System.EventHandler(this.toggleFile_Click);
			// 
			// toggleScript
			// 
			this.toggleScript.Name = "toggleScript";
			this.toggleScript.Size = new System.Drawing.Size(226, 22);
			this.toggleScript.Text = "Script Editor";
			this.toggleScript.Click += new System.EventHandler(this.toggleScript_Click);
			// 
			// toggleUndo
			// 
			this.toggleUndo.Name = "toggleUndo";
			this.toggleUndo.Size = new System.Drawing.Size(226, 22);
			this.toggleUndo.Text = "Undo / Redo";
			this.toggleUndo.Click += new System.EventHandler(this.toggleUndo_Click);
			// 
			// toggleCopy
			// 
			this.toggleCopy.Name = "toggleCopy";
			this.toggleCopy.Size = new System.Drawing.Size(226, 22);
			this.toggleCopy.Text = "Cut / Copy / Paste";
			this.toggleCopy.Click += new System.EventHandler(this.toggleCopy_Click);
			// 
			// togglePrefabs
			// 
			this.togglePrefabs.Name = "togglePrefabs";
			this.togglePrefabs.Size = new System.Drawing.Size(226, 22);
			this.togglePrefabs.Text = "Prefabs";
			this.togglePrefabs.Click += new System.EventHandler(this.togglePrefabs_Click);
			// 
			// toggleFilter
			// 
			this.toggleFilter.Name = "toggleFilter";
			this.toggleFilter.Size = new System.Drawing.Size(226, 22);
			this.toggleFilter.Text = "Things Filter / Linedef Colors";
			this.toggleFilter.Click += new System.EventHandler(this.toggleFilter_Click);
			// 
			// toggleViewModes
			// 
			this.toggleViewModes.Name = "toggleViewModes";
			this.toggleViewModes.Size = new System.Drawing.Size(226, 22);
			this.toggleViewModes.Text = "View Modes";
			this.toggleViewModes.Click += new System.EventHandler(this.toggleViewModes_Click);
			// 
			// toggleGeometry
			// 
			this.toggleGeometry.Name = "toggleGeometry";
			this.toggleGeometry.Size = new System.Drawing.Size(226, 22);
			this.toggleGeometry.Text = "Snap / Merge";
			this.toggleGeometry.Click += new System.EventHandler(this.toggleGeometry_Click);
			// 
			// toggleTesting
			// 
			this.toggleTesting.Name = "toggleTesting";
			this.toggleTesting.Size = new System.Drawing.Size(226, 22);
			this.toggleTesting.Text = "Testing";
			this.toggleTesting.Click += new System.EventHandler(this.toggleTesting_Click);
			// 
			// toggleRendering
			// 
			this.toggleRendering.Name = "toggleRendering";
			this.toggleRendering.Size = new System.Drawing.Size(226, 22);
			this.toggleRendering.Text = "Rendering";
			this.toggleRendering.Click += new System.EventHandler(this.toggleRendering_Click);
			// 
			// buttonnewmap
			// 
			this.buttonnewmap.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonnewmap.Image = global::CodeImp.DoomBuilder.Properties.Resources.NewMap;
			this.buttonnewmap.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonnewmap.Margin = new System.Windows.Forms.Padding(6, 1, 0, 2);
			this.buttonnewmap.Name = "buttonnewmap";
			this.buttonnewmap.Size = new System.Drawing.Size(23, 22);
			this.buttonnewmap.Tag = "builder_newmap";
			this.buttonnewmap.Text = "New Map";
			this.buttonnewmap.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// buttonopenmap
			// 
			this.buttonopenmap.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonopenmap.Image = global::CodeImp.DoomBuilder.Properties.Resources.OpenMap;
			this.buttonopenmap.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonopenmap.Name = "buttonopenmap";
			this.buttonopenmap.Size = new System.Drawing.Size(23, 22);
			this.buttonopenmap.Tag = "builder_openmap";
			this.buttonopenmap.Text = "Open Map";
			this.buttonopenmap.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// buttonsavemap
			// 
			this.buttonsavemap.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonsavemap.Image = global::CodeImp.DoomBuilder.Properties.Resources.SaveMap;
			this.buttonsavemap.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonsavemap.Name = "buttonsavemap";
			this.buttonsavemap.Size = new System.Drawing.Size(23, 22);
			this.buttonsavemap.Tag = "builder_savemap";
			this.buttonsavemap.Text = "Save Map";
			this.buttonsavemap.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// buttonscripteditor
			// 
			this.buttonscripteditor.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonscripteditor.Image = global::CodeImp.DoomBuilder.Properties.Resources.Script2;
			this.buttonscripteditor.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonscripteditor.Name = "buttonscripteditor";
			this.buttonscripteditor.Size = new System.Drawing.Size(23, 22);
			this.buttonscripteditor.Tag = "builder_openscripteditor";
			this.buttonscripteditor.Text = "Open Script Editor";
			this.buttonscripteditor.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// buttonundo
			// 
			this.buttonundo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonundo.Image = global::CodeImp.DoomBuilder.Properties.Resources.Undo;
			this.buttonundo.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonundo.Name = "buttonundo";
			this.buttonundo.Size = new System.Drawing.Size(23, 22);
			this.buttonundo.Tag = "builder_undo";
			this.buttonundo.Text = "Undo";
			this.buttonundo.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// buttonredo
			// 
			this.buttonredo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonredo.Image = global::CodeImp.DoomBuilder.Properties.Resources.Redo;
			this.buttonredo.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonredo.Name = "buttonredo";
			this.buttonredo.Size = new System.Drawing.Size(23, 22);
			this.buttonredo.Tag = "builder_redo";
			this.buttonredo.Text = "Redo";
			this.buttonredo.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// buttoncut
			// 
			this.buttoncut.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttoncut.Image = global::CodeImp.DoomBuilder.Properties.Resources.Cut;
			this.buttoncut.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttoncut.Name = "buttoncut";
			this.buttoncut.Size = new System.Drawing.Size(23, 22);
			this.buttoncut.Tag = "builder_cutselection";
			this.buttoncut.Text = "Cut Selection";
			this.buttoncut.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// buttoncopy
			// 
			this.buttoncopy.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttoncopy.Image = global::CodeImp.DoomBuilder.Properties.Resources.Copy;
			this.buttoncopy.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttoncopy.Name = "buttoncopy";
			this.buttoncopy.Size = new System.Drawing.Size(23, 22);
			this.buttoncopy.Tag = "builder_copyselection";
			this.buttoncopy.Text = "Copy Selection";
			this.buttoncopy.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// buttonpaste
			// 
			this.buttonpaste.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonpaste.Image = global::CodeImp.DoomBuilder.Properties.Resources.Paste;
			this.buttonpaste.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonpaste.Name = "buttonpaste";
			this.buttonpaste.Size = new System.Drawing.Size(23, 22);
			this.buttonpaste.Tag = "builder_pasteselection";
			this.buttonpaste.Text = "Paste Selection";
			this.buttonpaste.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// buttoninsertprefabfile
			// 
			this.buttoninsertprefabfile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttoninsertprefabfile.Image = global::CodeImp.DoomBuilder.Properties.Resources.Prefab;
			this.buttoninsertprefabfile.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttoninsertprefabfile.Name = "buttoninsertprefabfile";
			this.buttoninsertprefabfile.Size = new System.Drawing.Size(23, 22);
			this.buttoninsertprefabfile.Tag = "builder_insertprefabfile";
			this.buttoninsertprefabfile.Text = "Insert Prefab from File";
			this.buttoninsertprefabfile.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// buttoninsertpreviousprefab
			// 
			this.buttoninsertpreviousprefab.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttoninsertpreviousprefab.Image = global::CodeImp.DoomBuilder.Properties.Resources.Prefab2;
			this.buttoninsertpreviousprefab.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttoninsertpreviousprefab.Name = "buttoninsertpreviousprefab";
			this.buttoninsertpreviousprefab.Size = new System.Drawing.Size(23, 22);
			this.buttoninsertpreviousprefab.Tag = "builder_insertpreviousprefab";
			this.buttoninsertpreviousprefab.Text = "Insert Previous Prefab";
			this.buttoninsertpreviousprefab.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// buttonthingsfilter
			// 
			this.buttonthingsfilter.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonthingsfilter.Image = global::CodeImp.DoomBuilder.Properties.Resources.Filter;
			this.buttonthingsfilter.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonthingsfilter.Name = "buttonthingsfilter";
			this.buttonthingsfilter.Size = new System.Drawing.Size(23, 22);
			this.buttonthingsfilter.Tag = "builder_thingsfilterssetup";
			this.buttonthingsfilter.Text = "Configure Things Filters";
			this.buttonthingsfilter.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// thingfilters
			// 
			this.thingfilters.AutoSize = false;
			this.thingfilters.AutoToolTip = false;
			this.thingfilters.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.thingfilters.Image = ((System.Drawing.Image)(resources.GetObject("thingfilters.Image")));
			this.thingfilters.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.thingfilters.Margin = new System.Windows.Forms.Padding(1, 1, 0, 2);
			this.thingfilters.Name = "thingfilters";
			this.thingfilters.Size = new System.Drawing.Size((int)(120 * MainForm.DPIScaler.Width), (int)(22 * MainForm.DPIScaler.Height));
			this.thingfilters.Text = "(show all)";
			this.thingfilters.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.thingfilters.DropDownClosed += new System.EventHandler(this.LoseFocus);
			// 
			// separatorlinecolors
			// 
			this.separatorlinecolors.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
			this.separatorlinecolors.Name = "separatorlinecolors";
			this.separatorlinecolors.Size = new System.Drawing.Size(6, 25);
			// 
			// buttonlinededfcolors
			// 
			this.buttonlinededfcolors.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonlinededfcolors.Image = global::CodeImp.DoomBuilder.Properties.Resources.LinedefColorPresets;
			this.buttonlinededfcolors.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonlinededfcolors.Name = "buttonlinededfcolors";
			this.buttonlinededfcolors.Size = new System.Drawing.Size(23, 22);
			this.buttonlinededfcolors.Tag = "builder_linedefcolorssetup";
			this.buttonlinededfcolors.Text = "Configure Linedef Colors";
			this.buttonlinededfcolors.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// linedefcolorpresets
			// 
			this.linedefcolorpresets.AutoSize = false;
			this.linedefcolorpresets.AutoToolTip = false;
			this.linedefcolorpresets.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.linedefcolorpresets.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.linedefcolorpresets.Margin = new System.Windows.Forms.Padding(1, 1, 0, 2);
			this.linedefcolorpresets.Name = "linedefcolorpresets";
			this.linedefcolorpresets.Size = new System.Drawing.Size((int)(120 * MainForm.DPIScaler.Width), (int)(22 * MainForm.DPIScaler.Height));
			this.linedefcolorpresets.Text = "No presets";
			this.linedefcolorpresets.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.linedefcolorpresets.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.linedefcolorpresets_DropDownItemClicked);
			this.linedefcolorpresets.DropDownClosed += new System.EventHandler(this.LoseFocus);
			this.linedefcolorpresets.Click += new System.EventHandler(this.linedefcolorpresets_MouseLeave);
			// 
			// separatorfilters
			// 
			this.separatorfilters.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
			this.separatorfilters.Name = "separatorfilters";
			this.separatorfilters.Size = new System.Drawing.Size(6, 25);
			// 
			// separatorrendering
			// 
			this.separatorrendering.Name = "separatorrendering";
			this.separatorrendering.Size = new System.Drawing.Size(270, 6);
			// 
			// itemdynlightmodes
			// 
			this.itemdynlightmodes.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.itemnodynlights,
			this.itemdynlights,
			this.itemdynlightsanim});
			this.itemdynlightmodes.Image = global::CodeImp.DoomBuilder.Properties.Resources.Light;
			this.itemdynlightmodes.Name = "itemdynlightmodes";
			this.itemdynlightmodes.Size = new System.Drawing.Size(273, 22);
			this.itemdynlightmodes.Text = "Dynamic light rendering mode";
			// 
			// itemnodynlights
			// 
			this.itemnodynlights.CheckOnClick = true;
			this.itemnodynlights.Image = global::CodeImp.DoomBuilder.Properties.Resources.LightDisabled;
			this.itemnodynlights.Name = "itemnodynlights";
			this.itemnodynlights.Size = new System.Drawing.Size(237, 22);
			this.itemnodynlights.Tag = 0;
			this.itemnodynlights.Text = "Don\'t show dynamic lights";
			this.itemnodynlights.Click += new System.EventHandler(this.ChangeLightRenderingMode);
			// 
			// itemdynlights
			// 
			this.itemdynlights.CheckOnClick = true;
			this.itemdynlights.Image = global::CodeImp.DoomBuilder.Properties.Resources.Light;
			this.itemdynlights.Name = "itemdynlights";
			this.itemdynlights.Size = new System.Drawing.Size(237, 22);
			this.itemdynlights.Tag = 1;
			this.itemdynlights.Text = "Show dynamic lights";
			this.itemdynlights.Click += new System.EventHandler(this.ChangeLightRenderingMode);
			// 
			// itemdynlightsanim
			// 
			this.itemdynlightsanim.CheckOnClick = true;
			this.itemdynlightsanim.Image = global::CodeImp.DoomBuilder.Properties.Resources.Light_animate;
			this.itemdynlightsanim.Name = "itemdynlightsanim";
			this.itemdynlightsanim.Size = new System.Drawing.Size(237, 22);
			this.itemdynlightsanim.Tag = 2;
			this.itemdynlightsanim.Text = "Show animated dynamic lights";
			this.itemdynlightsanim.Click += new System.EventHandler(this.ChangeLightRenderingMode);
			// 
			// itemmodelmodes
			// 
			this.itemmodelmodes.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.itemnomdl,
			this.itemselmdl,
			this.itemfiltermdl,
			this.itemallmdl});
			this.itemmodelmodes.Image = global::CodeImp.DoomBuilder.Properties.Resources.Model;
			this.itemmodelmodes.Name = "itemmodelmodes";
			this.itemmodelmodes.Size = new System.Drawing.Size(273, 22);
			this.itemmodelmodes.Text = "Model rendering mode";
			// 
			// itemnomdl
			// 
			this.itemnomdl.CheckOnClick = true;
			this.itemnomdl.Image = global::CodeImp.DoomBuilder.Properties.Resources.ModelDisabled;
			this.itemnomdl.Name = "itemnomdl";
			this.itemnomdl.Size = new System.Drawing.Size(298, 22);
			this.itemnomdl.Tag = 0;
			this.itemnomdl.Text = "Don\'t show models";
			this.itemnomdl.Click += new System.EventHandler(this.ChangeModelRenderingMode);
			// 
			// itemselmdl
			// 
			this.itemselmdl.CheckOnClick = true;
			this.itemselmdl.Image = global::CodeImp.DoomBuilder.Properties.Resources.Model_selected;
			this.itemselmdl.Name = "itemselmdl";
			this.itemselmdl.Size = new System.Drawing.Size(298, 22);
			this.itemselmdl.Tag = 1;
			this.itemselmdl.Text = "Show models for selected Things only";
			this.itemselmdl.Click += new System.EventHandler(this.ChangeModelRenderingMode);
			// 
			// itemfiltermdl
			// 
			this.itemfiltermdl.CheckOnClick = true;
			this.itemfiltermdl.Image = global::CodeImp.DoomBuilder.Properties.Resources.ModelFiltered;
			this.itemfiltermdl.Name = "itemfiltermdl";
			this.itemfiltermdl.Size = new System.Drawing.Size(298, 22);
			this.itemfiltermdl.Tag = 2;
			this.itemfiltermdl.Text = "Show models for current Things Filter only";
			this.itemfiltermdl.Click += new System.EventHandler(this.ChangeModelRenderingMode);
			// 
			// itemallmdl
			// 
			this.itemallmdl.CheckOnClick = true;
			this.itemallmdl.Image = global::CodeImp.DoomBuilder.Properties.Resources.Model;
			this.itemallmdl.Name = "itemallmdl";
			this.itemallmdl.Size = new System.Drawing.Size(298, 22);
			this.itemallmdl.Tag = 3;
			this.itemallmdl.Text = "Always show models";
			this.itemallmdl.Click += new System.EventHandler(this.ChangeModelRenderingMode);
			// 
			// itemfog
			// 
			this.itemtogglefog.CheckOnClick = true;
			this.itemtogglefog.Image = global::CodeImp.DoomBuilder.Properties.Resources.fog;
			this.itemtogglefog.Name = "itemtogglefog";
			this.itemtogglefog.Size = new System.Drawing.Size(273, 22);
			this.itemtogglefog.Tag = "builder_gztogglefog";
			this.itemtogglefog.Text = "Render fog (Visual mode)";
			this.itemtogglefog.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// itemsky
			// 
			this.itemtogglesky.CheckOnClick = true;
			this.itemtogglesky.Image = global::CodeImp.DoomBuilder.Properties.Resources.Sky;
			this.itemtogglesky.Name = "itemtogglesky";
			this.itemtogglesky.Size = new System.Drawing.Size(273, 22);
			this.itemtogglesky.Tag = "builder_gztogglesky";
			this.itemtogglesky.Text = "Render sky (Visual mode)";
			this.itemtogglesky.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// itemclassicrendering
			// 
			this.itemtoggleclassicrendering.CheckOnClick = true;
			this.itemtoggleclassicrendering.Image = global::CodeImp.DoomBuilder.Properties.Resources.ClassicRendering;
			this.itemtoggleclassicrendering.Name = "itemtoggleclassicrendering";
			this.itemtoggleclassicrendering.Size = new System.Drawing.Size(273, 22);
			this.itemtoggleclassicrendering.Tag = "builder_toggleclassicrendering";
			this.itemtoggleclassicrendering.Text = "Classic rendering (Visual mode)";
			this.itemtoggleclassicrendering.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// itemeventlines
			// 
			this.itemtoggleeventlines.CheckOnClick = true;
			this.itemtoggleeventlines.Image = global::CodeImp.DoomBuilder.Properties.Resources.InfoLine;
			this.itemtoggleeventlines.Name = "itemtoggleeventlines";
			this.itemtoggleeventlines.Size = new System.Drawing.Size(273, 22);
			this.itemtoggleeventlines.Tag = "builder_gztoggleeventlines";
			this.itemtoggleeventlines.Text = "Show Event Lines";
			this.itemtoggleeventlines.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// itemvisualverts
			// 
			this.itemtogglevisualverts.CheckOnClick = true;
			this.itemtogglevisualverts.Image = global::CodeImp.DoomBuilder.Properties.Resources.VisualVertices;
			this.itemtogglevisualverts.Name = "itemtogglevisualverts";
			this.itemtogglevisualverts.Size = new System.Drawing.Size(273, 22);
			this.itemtogglevisualverts.Tag = "builder_gztogglevisualvertices";
			this.itemtogglevisualverts.Text = "Show Editable Vertices (Visual mode)";
			this.itemtogglevisualverts.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// buttonfullbrightness
			// 
			this.buttonfullbrightness.Checked = true;
			this.buttonfullbrightness.CheckOnClick = true;
			this.buttonfullbrightness.CheckState = System.Windows.Forms.CheckState.Checked;
			this.buttonfullbrightness.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonfullbrightness.Image = global::CodeImp.DoomBuilder.Properties.Resources.Brightness;
			this.buttonfullbrightness.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonfullbrightness.Name = "buttonfullbrightness";
			this.buttonfullbrightness.Size = new System.Drawing.Size(23, 22);
			this.buttonfullbrightness.Tag = "builder_togglebrightness";
			this.buttonfullbrightness.Text = "Full Brightness";
			this.buttonfullbrightness.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// buttontogglegrid
			// 
			this.buttontogglegrid.Checked = true;
			this.buttontogglegrid.CheckOnClick = true;
			this.buttontogglegrid.CheckState = System.Windows.Forms.CheckState.Checked;
			this.buttontogglegrid.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttontogglegrid.Image = global::CodeImp.DoomBuilder.Properties.Resources.Grid2;
			this.buttontogglegrid.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttontogglegrid.Name = "buttontogglegrid";
			this.buttontogglegrid.Size = new System.Drawing.Size(23, 22);
			this.buttontogglegrid.Tag = "builder_togglegrid";
			this.buttontogglegrid.Text = "Render Grid";
			this.buttontogglegrid.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// buttontoggledynamicgrid
			// 
			this.buttontoggledynamicgrid.Checked = true;
			this.buttontoggledynamicgrid.CheckOnClick = true;
			this.buttontoggledynamicgrid.CheckState = System.Windows.Forms.CheckState.Checked;
			this.buttontoggledynamicgrid.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttontoggledynamicgrid.Image = global::CodeImp.DoomBuilder.Properties.Resources.GridDynamic;
			this.buttontoggledynamicgrid.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttontoggledynamicgrid.Name = "buttontoggledynamicgrid";
			this.buttontoggledynamicgrid.Size = new System.Drawing.Size(23, 22);
			this.buttontoggledynamicgrid.Tag = "builder_toggledynamicgrid";
			this.buttontoggledynamicgrid.Text = "Dynamic Grid Size";
			this.buttontoggledynamicgrid.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// separatorfullbrightness
			// 
			this.separatorfullbrightness.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
			this.separatorfullbrightness.Name = "separatorfullbrightness";
			this.separatorfullbrightness.Size = new System.Drawing.Size(6, 25);
			// 
			// buttonviewnormal
			// 
			this.buttonviewnormal.CheckOnClick = true;
			this.buttonviewnormal.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonviewnormal.Image = global::CodeImp.DoomBuilder.Properties.Resources.ViewNormal;
			this.buttonviewnormal.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonviewnormal.Name = "buttonviewnormal";
			this.buttonviewnormal.Size = new System.Drawing.Size(23, 22);
			this.buttonviewnormal.Tag = "builder_viewmodenormal";
			this.buttonviewnormal.Text = "View Wireframe";
			this.buttonviewnormal.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// buttonviewbrightness
			// 
			this.buttonviewbrightness.CheckOnClick = true;
			this.buttonviewbrightness.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonviewbrightness.Image = global::CodeImp.DoomBuilder.Properties.Resources.ViewBrightness;
			this.buttonviewbrightness.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonviewbrightness.Name = "buttonviewbrightness";
			this.buttonviewbrightness.Size = new System.Drawing.Size(23, 22);
			this.buttonviewbrightness.Tag = "builder_viewmodebrightness";
			this.buttonviewbrightness.Text = "View Brightness Levels";
			this.buttonviewbrightness.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// buttonviewfloors
			// 
			this.buttonviewfloors.CheckOnClick = true;
			this.buttonviewfloors.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonviewfloors.Image = global::CodeImp.DoomBuilder.Properties.Resources.ViewTextureFloor;
			this.buttonviewfloors.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonviewfloors.Name = "buttonviewfloors";
			this.buttonviewfloors.Size = new System.Drawing.Size(23, 22);
			this.buttonviewfloors.Tag = "builder_viewmodefloors";
			this.buttonviewfloors.Text = "View Floor Textures";
			this.buttonviewfloors.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// buttonviewceilings
			// 
			this.buttonviewceilings.CheckOnClick = true;
			this.buttonviewceilings.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonviewceilings.Image = global::CodeImp.DoomBuilder.Properties.Resources.ViewTextureCeiling;
			this.buttonviewceilings.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonviewceilings.Name = "buttonviewceilings";
			this.buttonviewceilings.Size = new System.Drawing.Size(23, 22);
			this.buttonviewceilings.Tag = "builder_viewmodeceilings";
			this.buttonviewceilings.Text = "View Ceiling Textures";
			this.buttonviewceilings.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// separatorgeomergemodes
			// 
			this.separatorgeomergemodes.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
			this.separatorgeomergemodes.Name = "separatorgeomergemodes";
			this.separatorgeomergemodes.Size = new System.Drawing.Size(6, 25);
			// 
			// buttonmergegeoclassic
			// 
			this.buttonmergegeoclassic.CheckOnClick = true;
			this.buttonmergegeoclassic.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonmergegeoclassic.Image = global::CodeImp.DoomBuilder.Properties.Resources.MergeGeoClassic;
			this.buttonmergegeoclassic.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonmergegeoclassic.Name = "buttonmergegeoclassic";
			this.buttonmergegeoclassic.Size = new System.Drawing.Size(23, 22);
			this.buttonmergegeoclassic.Tag = "builder_geomergeclassic";
			this.buttonmergegeoclassic.Text = "Merge Dragged Vertices Only";
			this.buttonmergegeoclassic.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// buttonmergegeoclassic
			// 
			this.buttonmergegeo.CheckOnClick = true;
			this.buttonmergegeo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonmergegeo.Image = global::CodeImp.DoomBuilder.Properties.Resources.MergeGeo;
			this.buttonmergegeo.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonmergegeo.Name = "buttonmergegeo";
			this.buttonmergegeo.Size = new System.Drawing.Size(23, 22);
			this.buttonmergegeo.Tag = "builder_geomerge";
			this.buttonmergegeo.Text = "Merge Dragged Geometry";
			this.buttonmergegeo.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// buttonmergegeoclassic
			// 
			this.buttonplacegeo.CheckOnClick = true;
			this.buttonplacegeo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonplacegeo.Image = global::CodeImp.DoomBuilder.Properties.Resources.MergeGeoRemoveLines;
			this.buttonplacegeo.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonplacegeo.Name = "buttonmergegeoclassic";
			this.buttonplacegeo.Size = new System.Drawing.Size(23, 22);
			this.buttonplacegeo.Tag = "builder_georeplace";
			this.buttonplacegeo.Text = "Replace with Dragged Geometry";
			this.buttonplacegeo.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// seperatorviews
			// 
			this.seperatorviews.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
			this.seperatorviews.Name = "seperatorviews";
			this.seperatorviews.Size = new System.Drawing.Size(6, 25);
			// 
			// buttontogglecomments
			// 
			this.buttontogglecomments.Checked = true;
			this.buttontogglecomments.CheckState = System.Windows.Forms.CheckState.Checked;
			this.buttontogglecomments.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttontogglecomments.Image = global::CodeImp.DoomBuilder.Properties.Resources.Comment;
			this.buttontogglecomments.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttontogglecomments.Name = "buttontogglecomments";
			this.buttontogglecomments.Size = new System.Drawing.Size(23, 22);
			this.buttontogglecomments.Tag = "builder_togglecomments";
			this.buttontogglecomments.Text = "Show Comments";
			this.buttontogglecomments.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// buttontogglefixedthingsscale
			// 
			this.buttontogglefixedthingsscale.Checked = true;
			this.buttontogglefixedthingsscale.CheckState = System.Windows.Forms.CheckState.Checked;
			this.buttontogglefixedthingsscale.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttontogglefixedthingsscale.Image = global::CodeImp.DoomBuilder.Properties.Resources.FixedThingsScale;
			this.buttontogglefixedthingsscale.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttontogglefixedthingsscale.Name = "buttontogglefixedthingsscale";
			this.buttontogglefixedthingsscale.Size = new System.Drawing.Size(23, 22);
			this.buttontogglefixedthingsscale.Tag = "builder_togglefixedthingsscale";
			this.buttontogglefixedthingsscale.Text = "Fixed Things Scale";
			this.buttontogglefixedthingsscale.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// buttonsnaptogrid
			// 
			this.buttonsnaptogrid.Checked = true;
			this.buttonsnaptogrid.CheckState = System.Windows.Forms.CheckState.Checked;
			this.buttonsnaptogrid.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonsnaptogrid.Image = global::CodeImp.DoomBuilder.Properties.Resources.Grid4;
			this.buttonsnaptogrid.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonsnaptogrid.Name = "buttonsnaptogrid";
			this.buttonsnaptogrid.Size = new System.Drawing.Size(23, 22);
			this.buttonsnaptogrid.Tag = "builder_togglesnap";
			this.buttonsnaptogrid.Text = "Snap to Grid";
			this.buttonsnaptogrid.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// buttonautomerge
			// 
			this.buttonautomerge.Checked = true;
			this.buttonautomerge.CheckState = System.Windows.Forms.CheckState.Checked;
			this.buttonautomerge.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonautomerge.Image = global::CodeImp.DoomBuilder.Properties.Resources.mergegeometry2;
			this.buttonautomerge.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonautomerge.Name = "buttonautomerge";
			this.buttonautomerge.Size = new System.Drawing.Size(23, 22);
			this.buttonautomerge.Tag = "builder_toggleautomerge";
			this.buttonautomerge.Text = "Snap to Geometry";
			this.buttonautomerge.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// buttonsplitjoinedsectors
			// 
			this.buttonsplitjoinedsectors.Checked = true;
			this.buttonsplitjoinedsectors.CheckState = System.Windows.Forms.CheckState.Checked;
			this.buttonsplitjoinedsectors.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonsplitjoinedsectors.Image = global::CodeImp.DoomBuilder.Properties.Resources.SplitSectors;
			this.buttonsplitjoinedsectors.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonsplitjoinedsectors.Name = "buttonsplitjoinedsectors";
			this.buttonsplitjoinedsectors.Size = new System.Drawing.Size(23, 22);
			this.buttonsplitjoinedsectors.Tag = "builder_togglejoinedsectorssplitting";
			this.buttonsplitjoinedsectors.Text = "Split Joined Sectors when Drawing Lines";
			this.buttonsplitjoinedsectors.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// buttonautoclearsidetextures
			// 
			this.buttonautoclearsidetextures.Checked = true;
			this.buttonautoclearsidetextures.CheckState = System.Windows.Forms.CheckState.Checked;
			this.buttonautoclearsidetextures.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonautoclearsidetextures.Image = global::CodeImp.DoomBuilder.Properties.Resources.ClearTextures;
			this.buttonautoclearsidetextures.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonautoclearsidetextures.Name = "buttonautoclearsidetextures";
			this.buttonautoclearsidetextures.Size = new System.Drawing.Size(23, 22);
			this.buttonautoclearsidetextures.Tag = "builder_toggleautoclearsidetextures";
			this.buttonautoclearsidetextures.Text = "Auto Clear Sidedef Textures";
			this.buttonautoclearsidetextures.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// seperatorgeometry
			// 
			this.seperatorgeometry.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
			this.seperatorgeometry.Name = "seperatorgeometry";
			this.seperatorgeometry.Size = new System.Drawing.Size(6, 25);
			// 
			// dynamiclightmode
			// 
			this.dynamiclightmode.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.dynamiclightmode.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.sightsdontshow,
			this.lightsshow,
			this.lightsshowanimated});
			this.dynamiclightmode.Image = global::CodeImp.DoomBuilder.Properties.Resources.Light;
			this.dynamiclightmode.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.dynamiclightmode.Name = "dynamiclightmode";
			this.dynamiclightmode.Size = new System.Drawing.Size(32, 20);
			this.dynamiclightmode.Tag = "builder_gztogglelights";
			this.dynamiclightmode.Text = "Dynamic light mode";
			this.dynamiclightmode.ButtonClick += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// sightsdontshow
			// 
			this.sightsdontshow.CheckOnClick = true;
			this.sightsdontshow.Image = global::CodeImp.DoomBuilder.Properties.Resources.LightDisabled;
			this.sightsdontshow.Name = "sightsdontshow";
			this.sightsdontshow.Size = new System.Drawing.Size(237, 22);
			this.sightsdontshow.Tag = 0;
			this.sightsdontshow.Text = "Don\'t show dynamic lights";
			this.sightsdontshow.Click += new System.EventHandler(this.ChangeLightRenderingMode);
			// 
			// lightsshow
			// 
			this.lightsshow.CheckOnClick = true;
			this.lightsshow.Image = global::CodeImp.DoomBuilder.Properties.Resources.Light;
			this.lightsshow.Name = "lightsshow";
			this.lightsshow.Size = new System.Drawing.Size(237, 22);
			this.lightsshow.Tag = 1;
			this.lightsshow.Text = "Show dynamic lights";
			this.lightsshow.Click += new System.EventHandler(this.ChangeLightRenderingMode);
			// 
			// lightsshowanimated
			// 
			this.lightsshowanimated.CheckOnClick = true;
			this.lightsshowanimated.Image = global::CodeImp.DoomBuilder.Properties.Resources.Light_animate;
			this.lightsshowanimated.Name = "lightsshowanimated";
			this.lightsshowanimated.Size = new System.Drawing.Size(237, 22);
			this.lightsshowanimated.Tag = 2;
			this.lightsshowanimated.Text = "Show animated dynamic lights";
			this.lightsshowanimated.Click += new System.EventHandler(this.ChangeLightRenderingMode);
			// 
			// modelrendermode
			// 
			this.modelrendermode.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.modelrendermode.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.modelsdontshow,
			this.modelsshowselection,
			this.modelsshowfiltered,
			this.modelsshowall});
			this.modelrendermode.Image = global::CodeImp.DoomBuilder.Properties.Resources.Model;
			this.modelrendermode.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.modelrendermode.Name = "modelrendermode";
			this.modelrendermode.Size = new System.Drawing.Size(32, 20);
			this.modelrendermode.Tag = "builder_gztogglemodels";
			this.modelrendermode.Text = "Model rendering mode";
			this.modelrendermode.ButtonClick += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// modelsdontshow
			// 
			this.modelsdontshow.CheckOnClick = true;
			this.modelsdontshow.Image = global::CodeImp.DoomBuilder.Properties.Resources.ModelDisabled;
			this.modelsdontshow.Name = "modelsdontshow";
			this.modelsdontshow.Size = new System.Drawing.Size(293, 22);
			this.modelsdontshow.Tag = 0;
			this.modelsdontshow.Text = "Don\'t show models";
			this.modelsdontshow.Click += new System.EventHandler(this.ChangeModelRenderingMode);
			// 
			// modelsshowselection
			// 
			this.modelsshowselection.CheckOnClick = true;
			this.modelsshowselection.Image = global::CodeImp.DoomBuilder.Properties.Resources.Model_selected;
			this.modelsshowselection.Name = "modelsshowselection";
			this.modelsshowselection.Size = new System.Drawing.Size(293, 22);
			this.modelsshowselection.Tag = 1;
			this.modelsshowselection.Text = "Show models for selected things only";
			this.modelsshowselection.Click += new System.EventHandler(this.ChangeModelRenderingMode);
			// 
			// modelsshowfiltered
			// 
			this.modelsshowfiltered.CheckOnClick = true;
			this.modelsshowfiltered.Image = global::CodeImp.DoomBuilder.Properties.Resources.ModelFiltered;
			this.modelsshowfiltered.Name = "modelsshowfiltered";
			this.modelsshowfiltered.Size = new System.Drawing.Size(293, 22);
			this.modelsshowfiltered.Tag = 2;
			this.modelsshowfiltered.Text = "Show models for current things filter only";
			this.modelsshowfiltered.Click += new System.EventHandler(this.ChangeModelRenderingMode);
			// 
			// modelsshowall
			// 
			this.modelsshowall.CheckOnClick = true;
			this.modelsshowall.Image = global::CodeImp.DoomBuilder.Properties.Resources.Model;
			this.modelsshowall.Name = "modelsshowall";
			this.modelsshowall.Size = new System.Drawing.Size(293, 22);
			this.modelsshowall.Tag = 3;
			this.modelsshowall.Text = "Always show models";
			this.modelsshowall.Click += new System.EventHandler(this.ChangeModelRenderingMode);
			// 
			// buttontogglefog
			// 
			this.buttontogglefog.CheckOnClick = true;
			this.buttontogglefog.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttontogglefog.Image = global::CodeImp.DoomBuilder.Properties.Resources.fog;
			this.buttontogglefog.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttontogglefog.Name = "buttontogglefog";
			this.buttontogglefog.Size = new System.Drawing.Size(23, 20);
			this.buttontogglefog.Tag = "builder_gztogglefog";
			this.buttontogglefog.Text = "Render Fog (Visual mode)";
			this.buttontogglefog.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// buttontogglesky
			// 
			this.buttontogglesky.CheckOnClick = true;
			this.buttontogglesky.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttontogglesky.Image = global::CodeImp.DoomBuilder.Properties.Resources.Sky;
			this.buttontogglesky.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttontogglesky.Name = "buttontogglesky";
			this.buttontogglesky.Size = new System.Drawing.Size(23, 20);
			this.buttontogglesky.Tag = "builder_gztogglesky";
			this.buttontogglesky.Text = "Render Sky (Visual mode)";
			this.buttontogglesky.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// buttontoggleclassicrendering
			// 
			this.buttontoggleclassicrendering.CheckOnClick = true;
			this.buttontoggleclassicrendering.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttontoggleclassicrendering.Image = global::CodeImp.DoomBuilder.Properties.Resources.ClassicRendering;
			this.buttontoggleclassicrendering.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttontoggleclassicrendering.Name = "buttontoggleclassicrendering";
			this.buttontoggleclassicrendering.Size = new System.Drawing.Size(23, 20);
			this.buttontoggleclassicrendering.Tag = "builder_toggleclassicrendering";
			this.buttontoggleclassicrendering.Text = "Classic Rendering (Visual mode)";
			this.buttontoggleclassicrendering.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// buttontoggleeventlines
			// 
			this.buttontoggleeventlines.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttontoggleeventlines.Image = global::CodeImp.DoomBuilder.Properties.Resources.InfoLine;
			this.buttontoggleeventlines.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttontoggleeventlines.Name = "buttontoggleeventlines";
			this.buttontoggleeventlines.Size = new System.Drawing.Size(23, 20);
			this.buttontoggleeventlines.Tag = "builder_gztoggleeventlines";
			this.buttontoggleeventlines.Text = "Show Event Lines";
			this.buttontoggleeventlines.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// buttontogglevisualvertices
			// 
			this.buttontogglevisualvertices.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttontogglevisualvertices.Image = global::CodeImp.DoomBuilder.Properties.Resources.VisualVertices;
			this.buttontogglevisualvertices.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttontogglevisualvertices.Name = "buttontogglevisualvertices";
			this.buttontogglevisualvertices.Size = new System.Drawing.Size(23, 20);
			this.buttontogglevisualvertices.Tag = "builder_gztogglevisualvertices";
			this.buttontogglevisualvertices.Text = "Show Editable Vertices (Visual mode)";
			this.buttontogglevisualvertices.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// separatorgzmodes
			// 
			this.separatorgzmodes.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
			this.separatorgzmodes.Name = "separatorgzmodes";
			this.separatorgzmodes.Size = new System.Drawing.Size(6, 25);
			// 
			// buttontest
			// 
			this.buttontest.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttontest.Image = global::CodeImp.DoomBuilder.Properties.Resources.Test;
			this.buttontest.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttontest.Name = "buttontest";
			this.buttontest.Size = new System.Drawing.Size(32, 20);
			this.buttontest.Tag = "builder_testmap";
			this.buttontest.Text = "Test Map";
			this.buttontest.ButtonClick += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// seperatortesting
			// 
			this.seperatortesting.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
			this.seperatortesting.Name = "seperatortesting";
			this.seperatortesting.Size = new System.Drawing.Size(6, 25);
			// 
			// statusbar
			// 
			this.statusbar.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.statusbar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.statuslabel,
			this.configlabel,
			toolStripSeparator12,
			this.gridlabel,
			this.buttongrid,
			toolStripSeparator1,
			this.zoomlabel,
			this.buttonzoom,
			toolStripSeparator3,
			this.xposlabel,
			this.poscommalabel,
			this.yposlabel,
			toolStripSeparator9,
			this.warnsLabel});
			this.statusbar.Location = new System.Drawing.Point(0, 670);
			this.statusbar.Name = "statusbar";
			this.statusbar.ShowItemToolTips = true;
			this.statusbar.Size = new System.Drawing.Size(1012, 23);
			this.statusbar.ImageScalingSize = MainForm.ScaledIconSize;
			this.statusbar.TabIndex = 2;
			// 
			// statuslabel
			// 
			this.statuslabel.Image = global::CodeImp.DoomBuilder.Properties.Resources.Status2;
			this.statuslabel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.statuslabel.Margin = new System.Windows.Forms.Padding(2, 3, 0, 2);
			this.statuslabel.Name = "statuslabel";
			this.statuslabel.Size = new System.Drawing.Size(340, 18);
			this.statuslabel.Spring = true;
			this.statuslabel.Text = "Initializing user interface...";
			this.statuslabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// configlabel
			// 
			this.configlabel.AutoSize = false;
			this.configlabel.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.configlabel.Name = "configlabel";
			this.configlabel.Size = new System.Drawing.Size((int)(280 * MainForm.DPIScaler.Width), (int)(18 * MainForm.DPIScaler.Height));
			this.configlabel.Text = "ZDoom (Doom in Hexen Format)";
			this.configlabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.configlabel.ToolTipText = "Current Game Configuration";
			// 
			// gridlabel
			// 
			this.gridlabel.AutoSize = false;
			this.gridlabel.AutoToolTip = true;
			this.gridlabel.Name = "gridlabel";
			this.gridlabel.Size = new System.Drawing.Size(64, 18);
			this.gridlabel.Text = "32 mp";
			this.gridlabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.gridlabel.TextImageRelation = System.Windows.Forms.TextImageRelation.Overlay;
			this.gridlabel.ToolTipText = "Grid size";
			// 
			// buttongrid
			// 
			this.buttongrid.AutoToolTip = false;
			this.buttongrid.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttongrid.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.itemgrid1024,
			this.itemgrid512,
			this.itemgrid256,
			this.itemgrid128,
			this.itemgrid64,
			this.itemgrid32,
			this.itemgrid16,
			this.itemgrid8,
			this.itemgrid4,
			this.itemgrid1,
			this.itemgrid05,
			this.itemgrid025,
			this.itemgrid0125,
			toolStripMenuItem4,
			this.itemgridcustom});
			this.buttongrid.Image = global::CodeImp.DoomBuilder.Properties.Resources.Grid2_arrowup;
			this.buttongrid.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.buttongrid.ImageTransparentColor = System.Drawing.Color.Transparent;
			this.buttongrid.Name = "buttongrid";
			this.buttongrid.ShowDropDownArrow = false;
			this.buttongrid.Size = new System.Drawing.Size(29, 21);
			this.buttongrid.Text = "Grid";
			// 
			// itemgrid1024
			// 
			this.itemgrid1024.Name = "itemgrid1024";
			this.itemgrid1024.Size = new System.Drawing.Size(153, 22);
			this.itemgrid1024.Tag = "1024";
			this.itemgrid1024.Text = "1024 mp";
			this.itemgrid1024.Click += new System.EventHandler(this.itemgridsize_Click);
			// 
			// itemgrid512
			// 
			this.itemgrid512.Name = "itemgrid512";
			this.itemgrid512.Size = new System.Drawing.Size(153, 22);
			this.itemgrid512.Tag = "512";
			this.itemgrid512.Text = "512 mp";
			this.itemgrid512.Click += new System.EventHandler(this.itemgridsize_Click);
			// 
			// itemgrid256
			// 
			this.itemgrid256.Name = "itemgrid256";
			this.itemgrid256.Size = new System.Drawing.Size(153, 22);
			this.itemgrid256.Tag = "256";
			this.itemgrid256.Text = "256 mp";
			this.itemgrid256.Click += new System.EventHandler(this.itemgridsize_Click);
			// 
			// itemgrid128
			// 
			this.itemgrid128.Name = "itemgrid128";
			this.itemgrid128.Size = new System.Drawing.Size(153, 22);
			this.itemgrid128.Tag = "128";
			this.itemgrid128.Text = "128 mp";
			this.itemgrid128.Click += new System.EventHandler(this.itemgridsize_Click);
			// 
			// itemgrid64
			// 
			this.itemgrid64.Name = "itemgrid64";
			this.itemgrid64.Size = new System.Drawing.Size(153, 22);
			this.itemgrid64.Tag = "64";
			this.itemgrid64.Text = "64 mp";
			this.itemgrid64.Click += new System.EventHandler(this.itemgridsize_Click);
			// 
			// itemgrid32
			// 
			this.itemgrid32.Name = "itemgrid32";
			this.itemgrid32.Size = new System.Drawing.Size(153, 22);
			this.itemgrid32.Tag = "32";
			this.itemgrid32.Text = "32 mp";
			this.itemgrid32.Click += new System.EventHandler(this.itemgridsize_Click);
			// 
			// itemgrid16
			// 
			this.itemgrid16.Name = "itemgrid16";
			this.itemgrid16.Size = new System.Drawing.Size(153, 22);
			this.itemgrid16.Tag = "16";
			this.itemgrid16.Text = "16 mp";
			this.itemgrid16.Click += new System.EventHandler(this.itemgridsize_Click);
			// 
			// itemgrid8
			// 
			this.itemgrid8.Name = "itemgrid8";
			this.itemgrid8.Size = new System.Drawing.Size(153, 22);
			this.itemgrid8.Tag = "8";
			this.itemgrid8.Text = "8 mp";
			this.itemgrid8.Click += new System.EventHandler(this.itemgridsize_Click);
			// 
			// itemgrid4
			// 
			this.itemgrid4.Name = "itemgrid4";
			this.itemgrid4.Size = new System.Drawing.Size(153, 22);
			this.itemgrid4.Tag = "4";
			this.itemgrid4.Text = "4 mp";
			this.itemgrid4.Click += new System.EventHandler(this.itemgridsize_Click);
			// 
			// itemgrid1
			// 
			this.itemgrid1.Name = "itemgrid1";
			this.itemgrid1.Size = new System.Drawing.Size(153, 22);
			this.itemgrid1.Tag = "1";
			this.itemgrid1.Text = "1 mp";
			this.itemgrid1.Click += new System.EventHandler(this.itemgridsize_Click);
			// 
			// itemgrid05
			// 
			this.itemgrid05.Name = "itemgrid05";
			this.itemgrid05.Size = new System.Drawing.Size(153, 22);
			this.itemgrid05.Tag = "0.5";
			this.itemgrid05.Text = "0.5 mp";
			this.itemgrid05.Click += new System.EventHandler(this.itemgridsize_Click);
			// 
			// itemgrid025
			// 
			this.itemgrid025.Name = "itemgrid025";
			this.itemgrid025.Size = new System.Drawing.Size(153, 22);
			this.itemgrid025.Tag = "0.25";
			this.itemgrid025.Text = "0.25 mp";
			this.itemgrid025.Click += new System.EventHandler(this.itemgridsize_Click);
			// 
			// itemgrid0125
			// 
			this.itemgrid0125.Name = "itemgrid0125";
			this.itemgrid0125.Size = new System.Drawing.Size(153, 22);
			this.itemgrid0125.Tag = "0.125";
			this.itemgrid0125.Text = "0.125 mp";
			this.itemgrid0125.Click += new System.EventHandler(this.itemgridsize_Click);
			// 
			// itemgridcustom
			// 
			this.itemgridcustom.Name = "itemgridcustom";
			this.itemgridcustom.Size = new System.Drawing.Size(153, 22);
			this.itemgridcustom.Text = "Customize...";
			this.itemgridcustom.Click += new System.EventHandler(this.itemgridcustom_Click);
			// 
			// zoomlabel
			// 
			this.zoomlabel.AutoSize = false;
			this.zoomlabel.AutoToolTip = true;
			this.zoomlabel.Name = "zoomlabel";
			this.zoomlabel.Size = new System.Drawing.Size(54, 18);
			this.zoomlabel.Text = "50%";
			this.zoomlabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.zoomlabel.TextImageRelation = System.Windows.Forms.TextImageRelation.Overlay;
			this.zoomlabel.ToolTipText = "Zoom level";
			// 
			// buttonzoom
			// 
			this.buttonzoom.AutoToolTip = false;
			this.buttonzoom.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonzoom.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.itemzoom800,
			this.itemzoom400,
			this.itemzoom200,
			this.itemzoom100,
			this.itemzoom50,
			this.itemzoom25,
			this.itemzoom10,
			this.itemzoom5,
			toolStripSeparator2,
			this.itemzoomfittoscreen});
			this.buttonzoom.Image = global::CodeImp.DoomBuilder.Properties.Resources.Zoom_arrowup;
			this.buttonzoom.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.buttonzoom.ImageTransparentColor = System.Drawing.Color.Transparent;
			this.buttonzoom.Name = "buttonzoom";
			this.buttonzoom.ShowDropDownArrow = false;
			this.buttonzoom.Size = new System.Drawing.Size(29, 21);
			this.buttonzoom.Text = "Zoom";
			// 
			// itemzoom800
			// 
			this.itemzoom800.Name = "itemzoom800";
			this.itemzoom800.Size = new System.Drawing.Size(156, 22);
			this.itemzoom800.Tag = "800";
			this.itemzoom800.Text = "800%";
			this.itemzoom800.Click += new System.EventHandler(this.itemzoomto_Click);
			// 
			// itemzoom400
			// 
			this.itemzoom400.Name = "itemzoom400";
			this.itemzoom400.Size = new System.Drawing.Size(156, 22);
			this.itemzoom400.Tag = "400";
			this.itemzoom400.Text = "400%";
			this.itemzoom400.Click += new System.EventHandler(this.itemzoomto_Click);
			// 
			// itemzoom200
			// 
			this.itemzoom200.Name = "itemzoom200";
			this.itemzoom200.Size = new System.Drawing.Size(156, 22);
			this.itemzoom200.Tag = "200";
			this.itemzoom200.Text = "200%";
			this.itemzoom200.Click += new System.EventHandler(this.itemzoomto_Click);
			// 
			// itemzoom100
			// 
			this.itemzoom100.Name = "itemzoom100";
			this.itemzoom100.Size = new System.Drawing.Size(156, 22);
			this.itemzoom100.Tag = "100";
			this.itemzoom100.Text = "100%";
			this.itemzoom100.Click += new System.EventHandler(this.itemzoomto_Click);
			// 
			// itemzoom50
			// 
			this.itemzoom50.Name = "itemzoom50";
			this.itemzoom50.Size = new System.Drawing.Size(156, 22);
			this.itemzoom50.Tag = "50";
			this.itemzoom50.Text = "50%";
			this.itemzoom50.Click += new System.EventHandler(this.itemzoomto_Click);
			// 
			// itemzoom25
			// 
			this.itemzoom25.Name = "itemzoom25";
			this.itemzoom25.Size = new System.Drawing.Size(156, 22);
			this.itemzoom25.Tag = "25";
			this.itemzoom25.Text = "25%";
			this.itemzoom25.Click += new System.EventHandler(this.itemzoomto_Click);
			// 
			// itemzoom10
			// 
			this.itemzoom10.Name = "itemzoom10";
			this.itemzoom10.Size = new System.Drawing.Size(156, 22);
			this.itemzoom10.Tag = "10";
			this.itemzoom10.Text = "10%";
			this.itemzoom10.Click += new System.EventHandler(this.itemzoomto_Click);
			// 
			// itemzoom5
			// 
			this.itemzoom5.Name = "itemzoom5";
			this.itemzoom5.Size = new System.Drawing.Size(156, 22);
			this.itemzoom5.Tag = "5";
			this.itemzoom5.Text = "5%";
			this.itemzoom5.Click += new System.EventHandler(this.itemzoomto_Click);
			// 
			// itemzoomfittoscreen
			// 
			this.itemzoomfittoscreen.Name = "itemzoomfittoscreen";
			this.itemzoomfittoscreen.Size = new System.Drawing.Size(156, 22);
			this.itemzoomfittoscreen.Text = "Fit to screen";
			this.itemzoomfittoscreen.Click += new System.EventHandler(this.itemzoomfittoscreen_Click);
			// 
			// xposlabel
			// 
			this.xposlabel.AutoSize = false;
			this.xposlabel.Name = "xposlabel";
			this.xposlabel.Size = new System.Drawing.Size(50, 18);
			this.xposlabel.Tag = "builder_centeroncoordinates";
			this.xposlabel.Text = "0";
			this.xposlabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.xposlabel.ToolTipText = "Current X, Y coordinates on map.\r\nClick to set specific coordinates.";
			this.xposlabel.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// yposlabel
			// 
			this.yposlabel.AutoSize = false;
			this.yposlabel.Name = "yposlabel";
			this.yposlabel.Size = new System.Drawing.Size(50, 18);
			this.yposlabel.Tag = "builder_centeroncoordinates";
			this.yposlabel.Text = "0";
			this.yposlabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.yposlabel.ToolTipText = "Current X, Y coordinates on map.\r\nClick to set specific coordinates.";
			this.yposlabel.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// warnsLabel
			// 
			this.warnsLabel.AutoSize = false;
			this.warnsLabel.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.warnsLabel.Image = global::CodeImp.DoomBuilder.Properties.Resources.WarningOff;
			this.warnsLabel.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.warnsLabel.Name = "warnsLabel";
			this.warnsLabel.Size = new System.Drawing.Size(44, 18);
			this.warnsLabel.Text = "0";
			this.warnsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.warnsLabel.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
			this.warnsLabel.Click += new System.EventHandler(this.warnsLabel_Click);
			// 
			// panelinfo
			// 
			this.panelinfo.Controls.Add(this.statistics);
			this.panelinfo.Controls.Add(this.heightpanel1);
			this.panelinfo.Controls.Add(this.labelcollapsedinfo);
			this.panelinfo.Controls.Add(this.modename);
			this.panelinfo.Controls.Add(this.buttontoggleinfo);
			this.panelinfo.Controls.Add(this.console);
			this.panelinfo.Controls.Add(this.vertexinfo);
			this.panelinfo.Controls.Add(this.linedefinfo);
			this.panelinfo.Controls.Add(this.thinginfo);
			this.panelinfo.Controls.Add(this.sectorinfo);
			this.panelinfo.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panelinfo.Location = new System.Drawing.Point(0, 564);
			this.panelinfo.Name = "panelinfo";
			this.panelinfo.Size = new System.Drawing.Size(1012, 106);
			this.panelinfo.TabIndex = 4;
			// 
			// statistics
			// 
			this.statistics.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.statistics.ForeColor = System.Drawing.SystemColors.GrayText;
			this.statistics.Location = new System.Drawing.Point(849, 2);
			this.statistics.Name = "statistics";
			this.statistics.Size = new System.Drawing.Size(138, 102);
			this.statistics.TabIndex = 9;
			this.statistics.Visible = false;
			// 
			// heightpanel1
			// 
			this.heightpanel1.BackColor = System.Drawing.Color.Navy;
			this.heightpanel1.ForeColor = System.Drawing.SystemColors.ControlText;
			this.heightpanel1.Location = new System.Drawing.Point(0, 0);
			this.heightpanel1.Name = "heightpanel1";
			this.heightpanel1.Size = new System.Drawing.Size(29, 106);
			this.heightpanel1.TabIndex = 7;
			this.heightpanel1.Visible = false;
			// 
			// labelcollapsedinfo
			// 
			this.labelcollapsedinfo.AutoSize = true;
			this.labelcollapsedinfo.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelcollapsedinfo.Location = new System.Drawing.Point(2, 2);
			this.labelcollapsedinfo.Name = "labelcollapsedinfo";
			this.labelcollapsedinfo.Size = new System.Drawing.Size(155, 13);
			this.labelcollapsedinfo.TabIndex = 6;
			this.labelcollapsedinfo.Text = "Collapsed Descriptions";
			this.labelcollapsedinfo.Visible = false;
			// 
			// modename
			// 
			this.modename.AutoSize = true;
			this.modename.Font = new System.Drawing.Font("Verdana", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.modename.ForeColor = System.Drawing.SystemColors.GrayText;
			this.modename.Location = new System.Drawing.Point(12, 20);
			this.modename.Name = "modename";
			this.modename.Size = new System.Drawing.Size(476, 59);
			this.modename.TabIndex = 8;
			this.modename.Text = "Hi. I missed you.";
			this.modename.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.modename.UseMnemonic = false;
			this.modename.Visible = false;
			// 
			// buttontoggleinfo
			// 
			this.buttontoggleinfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttontoggleinfo.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.buttontoggleinfo.Image = global::CodeImp.DoomBuilder.Properties.Resources.InfoPanelCollapse;
			this.buttontoggleinfo.Location = new System.Drawing.Point(988, 1);
			this.buttontoggleinfo.Name = "buttontoggleinfo";
			this.buttontoggleinfo.Size = new System.Drawing.Size(22, 19);
			this.buttontoggleinfo.TabIndex = 5;
			this.buttontoggleinfo.TabStop = false;
			this.buttontoggleinfo.Tag = "builder_toggleinfopanel";
			this.buttontoggleinfo.UseVisualStyleBackColor = true;
			this.buttontoggleinfo.Click += new System.EventHandler(this.InvokeTaggedAction);
			this.buttontoggleinfo.MouseUp += new System.Windows.Forms.MouseEventHandler(this.buttontoggleinfo_MouseUp);
			// 
			// console
			// 
			this.console.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.console.Location = new System.Drawing.Point(3, 3);
			this.console.Name = "console";
			this.console.Size = new System.Drawing.Size(851, 98);
			this.console.TabIndex = 10;
			// 
			// vertexinfo
			// 
			this.vertexinfo.Location = new System.Drawing.Point(0, 0);
			this.vertexinfo.MaximumSize = new System.Drawing.Size(10000, 100);
			this.vertexinfo.MinimumSize = new System.Drawing.Size(100, 100);
			this.vertexinfo.Name = "vertexinfo";
			this.vertexinfo.Size = new System.Drawing.Size(310, 100);
			this.vertexinfo.TabIndex = 1;
			this.vertexinfo.Visible = false;
			// 
			// linedefinfo
			// 
			this.linedefinfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.linedefinfo.Location = new System.Drawing.Point(3, 3);
			this.linedefinfo.MaximumSize = new System.Drawing.Size(10000, 100);
			this.linedefinfo.MinimumSize = new System.Drawing.Size(100, 100);
			this.linedefinfo.Name = "linedefinfo";
			this.linedefinfo.Size = new System.Drawing.Size(1006, 100);
			this.linedefinfo.TabIndex = 0;
			this.linedefinfo.Visible = false;
			// 
			// thinginfo
			// 
			this.thinginfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.thinginfo.Location = new System.Drawing.Point(3, 3);
			this.thinginfo.MaximumSize = new System.Drawing.Size(10000, 100);
			this.thinginfo.MinimumSize = new System.Drawing.Size(100, 100);
			this.thinginfo.Name = "thinginfo";
			this.thinginfo.Size = new System.Drawing.Size(1006, 100);
			this.thinginfo.TabIndex = 3;
			this.thinginfo.Visible = false;
			// 
			// sectorinfo
			// 
			this.sectorinfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.sectorinfo.Location = new System.Drawing.Point(3, 3);
			this.sectorinfo.MaximumSize = new System.Drawing.Size(10000, 100);
			this.sectorinfo.MinimumSize = new System.Drawing.Size(100, 100);
			this.sectorinfo.Name = "sectorinfo";
			this.sectorinfo.Size = new System.Drawing.Size(1006, 100);
			this.sectorinfo.TabIndex = 2;
			this.sectorinfo.Visible = false;
			// 
			// redrawtimer
			// 
			this.redrawtimer.Interval = 1;
			this.redrawtimer.Tick += new System.EventHandler(this.redrawtimer_Tick);
			// 
			// display
			// 
			this.display.BackColor = System.Drawing.SystemColors.ControlDarkDark;
			this.display.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.display.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.display.CausesValidation = false;
			this.display.Location = new System.Drawing.Point(373, 141);
			this.display.Name = "display";
			this.display.Size = new System.Drawing.Size(542, 307);
			this.display.TabIndex = 5;
			this.display.MouseUp += new System.Windows.Forms.MouseEventHandler(this.display_MouseUp);
			this.display.MouseLeave += new System.EventHandler(this.display_MouseLeave);
			this.display.Paint += new System.Windows.Forms.PaintEventHandler(this.display_Paint);
			this.display.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.display_PreviewKeyDown);
			this.display.MouseMove += new System.Windows.Forms.MouseEventHandler(this.display_MouseMove);
			this.display.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.display_MouseDoubleClick);
			this.display.MouseClick += new System.Windows.Forms.MouseEventHandler(this.display_MouseClick);
			this.display.MouseDown += new System.Windows.Forms.MouseEventHandler(this.display_MouseDown);
			this.display.Resize += new System.EventHandler(this.display_Resize);
			this.display.MouseEnter += new System.EventHandler(this.display_MouseEnter);
			// 
			// processor
			// 
			this.processor.Interval = 10;
			this.processor.Tick += new System.EventHandler(this.processor_Tick);
			// 
			// statusflasher
			// 
			this.statusflasher.Tick += new System.EventHandler(this.statusflasher_Tick);
			// 
			// statusresetter
			// 
			this.statusresetter.Tick += new System.EventHandler(this.statusresetter_Tick);
			// 
			// dockersspace
			// 
			this.dockersspace.Dock = System.Windows.Forms.DockStyle.Left;
			this.dockersspace.Location = new System.Drawing.Point(0, 49);
			this.dockersspace.Name = "dockersspace";
			this.dockersspace.Size = new System.Drawing.Size(26, 515);
			this.dockersspace.TabIndex = 6;
			// 
			// modestoolbar
			// 
			this.modestoolbar.AutoSize = false;
			this.modestoolbar.ImageScalingSize = MainForm.ScaledIconSize;
			this.modestoolbar.Dock = System.Windows.Forms.DockStyle.Left;
			this.modestoolbar.Location = new System.Drawing.Point(0, 49);
			this.modestoolbar.Name = "modestoolbar";
			this.modestoolbar.Padding = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.modestoolbar.Size = new System.Drawing.Size(30, 515);
			this.modestoolbar.TabIndex = 8;
			this.modestoolbar.Text = "Editing Modes";
			// 
			// dockerspanel
			// 
			this.dockerspanel.Location = new System.Drawing.Point(62, 67);
			this.dockerspanel.Name = "dockerspanel";
			this.dockerspanel.Size = new System.Drawing.Size(236, 467);
			this.dockerspanel.TabIndex = 7;
			this.dockerspanel.TabStop = false;
			this.dockerspanel.UserResize += new System.EventHandler(this.dockerspanel_UserResize);
			this.dockerspanel.Collapsed += new System.EventHandler(this.LoseFocus);
			this.dockerspanel.MouseContainerEnter += new System.EventHandler(this.dockerspanel_MouseContainerEnter);
			// 
			// dockerscollapser
			// 
			this.dockerscollapser.Interval = 200;
			this.dockerscollapser.Tick += new System.EventHandler(this.dockerscollapser_Tick);
			// 
			// flowLayoutPanel
			// 
			this.flowLayoutPanel.Controls.Add(this.menumain);
			this.flowLayoutPanel.Controls.Add(this.modecontrolsloolbar);
			this.flowLayoutPanel.Dock = System.Windows.Forms.DockStyle.Top;
			this.flowLayoutPanel.Location = new System.Drawing.Point(0, 0);
			this.flowLayoutPanel.Name = "flowLayoutPanel";
			this.flowLayoutPanel.Size = new System.Drawing.Size(1012, 24);
			this.flowLayoutPanel.TabIndex = 9;
			// 
			// modecontrolsloolbar
			// 
			this.modecontrolsloolbar.Dock = System.Windows.Forms.DockStyle.None;
			this.modecontrolsloolbar.Location = new System.Drawing.Point(328, 0);
			this.modecontrolsloolbar.Name = "modecontrolsloolbar";
			this.modecontrolsloolbar.Size = new System.Drawing.Size(43, 24);
			this.modecontrolsloolbar.ImageScalingSize = MainForm.ScaledIconSize;
			this.modecontrolsloolbar.TabIndex = 1;
			this.modecontrolsloolbar.Text = "toolStrip1";
			this.modecontrolsloolbar.Visible = false;
			// 
			// itemtogglecomments
			// 
			this.itemtogglecomments.Checked = true;
			this.itemtogglecomments.CheckOnClick = true;
			this.itemtogglecomments.CheckState = System.Windows.Forms.CheckState.Checked;
			this.itemtogglecomments.Image = global::CodeImp.DoomBuilder.Properties.Resources.Comment;
			this.itemtogglecomments.Name = "itemtogglecomments";
			this.itemtogglecomments.Size = new System.Drawing.Size(215, 22);
			this.itemtogglecomments.Tag = "builder_togglecomments";
			this.itemtogglecomments.Text = "Show Comments";
			this.itemtogglecomments.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// itemtogglefixedthingsscale
			// 
			this.itemtogglefixedthingsscale.Checked = true;
			this.itemtogglefixedthingsscale.CheckOnClick = true;
			this.itemtogglefixedthingsscale.CheckState = System.Windows.Forms.CheckState.Checked;
			this.itemtogglefixedthingsscale.Image = global::CodeImp.DoomBuilder.Properties.Resources.FixedThingsScale;
			this.itemtogglefixedthingsscale.Name = "itemtogglefixedthingsscale";
			this.itemtogglefixedthingsscale.Size = new System.Drawing.Size(215, 22);
			this.itemtogglefixedthingsscale.Tag = "builder_togglefixedthingsscale";
			this.itemtogglefixedthingsscale.Text = "Fixed Things Scale";
			this.itemtogglefixedthingsscale.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// itemtogglealwaysshowvertices
			// 
			this.itemtogglealwaysshowvertices.Checked = true;
			this.itemtogglealwaysshowvertices.CheckOnClick = true;
			this.itemtogglealwaysshowvertices.CheckState = System.Windows.Forms.CheckState.Checked;
			this.itemtogglealwaysshowvertices.Name = "itemtogglealwaysshowvertices";
			this.itemtogglealwaysshowvertices.Size = new System.Drawing.Size(215, 22);
			this.itemtogglealwaysshowvertices.Tag = "builder_togglealwaysshowvertices";
			this.itemtogglealwaysshowvertices.Text = "Always Show Vertices";
			this.itemtogglealwaysshowvertices.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// itemdynamicgridsize
			// 
			this.itemdynamicgridsize.Checked = true;
			this.itemdynamicgridsize.CheckOnClick = true;
			this.itemdynamicgridsize.CheckState = System.Windows.Forms.CheckState.Checked;
			this.itemdynamicgridsize.Image = global::CodeImp.DoomBuilder.Properties.Resources.GridDynamic;
			this.itemdynamicgridsize.Name = "itemdynamicgridsize";
			this.itemdynamicgridsize.Size = new System.Drawing.Size(219, 22);
			this.itemdynamicgridsize.Tag = "builder_toggledynamicgrid";
			this.itemdynamicgridsize.Text = "Dynamic Grid Size";
			this.itemdynamicgridsize.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.ClientSize = new System.Drawing.Size(1012, 693);
			this.Controls.Add(this.dockerspanel);
			this.Controls.Add(this.display);
			this.Controls.Add(this.dockersspace);
			this.Controls.Add(this.modestoolbar);
			this.Controls.Add(this.toolbar);
			this.Controls.Add(this.flowLayoutPanel);
			this.Controls.Add(this.panelinfo);
			this.Controls.Add(this.statusbar);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MainMenuStrip = this.menumain;
			this.Name = "MainForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = Application.ProductName + " R" + General.ThisAssembly.GetName().Version.Revision;
			this.Deactivate += new System.EventHandler(this.MainForm_Deactivate);
			this.Load += new System.EventHandler(this.MainForm_Load);
			this.Shown += new System.EventHandler(this.MainForm_Shown);
			this.Activated += new System.EventHandler(this.MainForm_Activated);
			this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyUp);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyDown);
			this.menumain.ResumeLayout(false);
			this.menumain.PerformLayout();
			this.toolbar.ResumeLayout(false);
			this.toolbar.PerformLayout();
			this.toolbarContextMenu.ResumeLayout(false);
			this.statusbar.ResumeLayout(false);
			this.statusbar.PerformLayout();
			this.panelinfo.ResumeLayout(false);
			this.panelinfo.PerformLayout();
			this.flowLayoutPanel.ResumeLayout(false);
			this.flowLayoutPanel.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.MenuStrip menumain;
		private System.Windows.Forms.ToolStrip toolbar;
		private System.Windows.Forms.StatusStrip statusbar;
		private System.Windows.Forms.Panel panelinfo;
		private System.Windows.Forms.ToolStripMenuItem menufile;
		private System.Windows.Forms.ToolStripMenuItem itemnewmap;
		private System.Windows.Forms.ToolStripMenuItem itemopenmap;
		private System.Windows.Forms.ToolStripMenuItem itemsavemap;
		private System.Windows.Forms.ToolStripMenuItem itemsavemapas;
		private System.Windows.Forms.ToolStripMenuItem itemsavemapinto;
		private System.Windows.Forms.ToolStripMenuItem itemexit;
		private System.Windows.Forms.ToolStripStatusLabel statuslabel;
		private System.Windows.Forms.ToolStripMenuItem itemclosemap;
		private System.Windows.Forms.Timer redrawtimer;
		private System.Windows.Forms.ToolStripMenuItem menuhelp;
		private System.Windows.Forms.ToolStripMenuItem itemhelpissues;
		private System.Windows.Forms.ToolStripMenuItem itemhelpabout;
		private System.Windows.Forms.ToolStripMenuItem itemhelpcheckupdates;
		private CodeImp.DoomBuilder.Controls.RenderTargetControl display;
		private System.Windows.Forms.ToolStripMenuItem itemnorecent;
		private System.Windows.Forms.ToolStripStatusLabel xposlabel;
		private System.Windows.Forms.ToolStripStatusLabel yposlabel;
		private System.Windows.Forms.ToolStripButton buttonnewmap;
		private System.Windows.Forms.ToolStripButton buttonopenmap;
		private System.Windows.Forms.ToolStripButton buttonsavemap;
		private System.Windows.Forms.ToolStripStatusLabel zoomlabel;
		private System.Windows.Forms.ToolStripDropDownButton buttonzoom;
		private System.Windows.Forms.ToolStripMenuItem itemzoomfittoscreen;
		private System.Windows.Forms.ToolStripMenuItem itemzoom100;
		private System.Windows.Forms.ToolStripMenuItem itemzoom200;
		private System.Windows.Forms.ToolStripMenuItem itemzoom50;
		private System.Windows.Forms.ToolStripMenuItem itemzoom25;
		private System.Windows.Forms.ToolStripMenuItem itemzoom10;
		private System.Windows.Forms.ToolStripMenuItem itemzoom5;
		private System.Windows.Forms.ToolStripMenuItem menutools;
		private System.Windows.Forms.ToolStripMenuItem configurationToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem preferencesToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem menuedit;
		private System.Windows.Forms.ToolStripMenuItem itemmapoptions;
		private System.Windows.Forms.ToolStripMenuItem itemreloadresources;
		private CodeImp.DoomBuilder.Controls.LinedefInfoPanel linedefinfo;
		private CodeImp.DoomBuilder.Controls.VertexInfoPanel vertexinfo;
		private CodeImp.DoomBuilder.Controls.SectorInfoPanel sectorinfo;
		private CodeImp.DoomBuilder.Controls.ThingInfoPanel thinginfo;
		private System.Windows.Forms.ToolStripButton buttonthingsfilter;
		private System.Windows.Forms.ToolStripSeparator seperatorviews;
		private System.Windows.Forms.ToolStripStatusLabel gridlabel;
		private System.Windows.Forms.ToolStripDropDownButton buttongrid;
		private System.Windows.Forms.ToolStripMenuItem itemgrid1024;
		private System.Windows.Forms.ToolStripMenuItem itemgrid256;
		private System.Windows.Forms.ToolStripMenuItem itemgrid128;
		private System.Windows.Forms.ToolStripMenuItem itemgrid64;
		private System.Windows.Forms.ToolStripMenuItem itemgrid32;
		private System.Windows.Forms.ToolStripMenuItem itemgrid16;
		private System.Windows.Forms.ToolStripMenuItem itemgrid4;
		private System.Windows.Forms.ToolStripMenuItem itemgrid8;
		private System.Windows.Forms.ToolStripMenuItem itemgridcustom;
		private System.Windows.Forms.ToolStripMenuItem itemgrid512;
		private System.Windows.Forms.ToolStripStatusLabel poscommalabel;
		private System.Windows.Forms.ToolStripMenuItem itemundo;
		private System.Windows.Forms.ToolStripMenuItem itemredo;
		private System.Windows.Forms.ToolStripButton buttonundo;
		private System.Windows.Forms.ToolStripButton buttonredo;
		private System.Windows.Forms.ToolStripButton buttonsnaptogrid;
		private System.Windows.Forms.ToolStripMenuItem itemsnaptogrid;
		private System.Windows.Forms.ToolStripButton buttonautomerge;
		private System.Windows.Forms.ToolStripMenuItem itemautomerge;
		private System.Windows.Forms.ToolStripButton buttonsplitjoinedsectors;
		private System.Windows.Forms.ToolStripMenuItem itemsplitjoinedsectors;
		private System.Windows.Forms.Timer processor;
		private System.Windows.Forms.ToolStripSeparator separatorgzmodes;
		private System.Windows.Forms.ToolStripSeparator seperatorfilesave;
		private System.Windows.Forms.ToolStripSeparator seperatortesting;
		private System.Windows.Forms.ToolStripSeparator seperatoreditgeometry;
		private System.Windows.Forms.ToolStripMenuItem itemgridinc;
		private System.Windows.Forms.ToolStripMenuItem itemgriddec;
		private System.Windows.Forms.ToolStripMenuItem itemgridsetup;
		private System.Windows.Forms.Timer statusflasher;
		private System.Windows.Forms.ToolStripSplitButton buttontest;
		private System.Windows.Forms.ToolStripButton buttoncut;
		private System.Windows.Forms.ToolStripButton buttoncopy;
		private System.Windows.Forms.ToolStripButton buttonpaste;
		private System.Windows.Forms.ToolStripSeparator seperatoreditundo;
		private System.Windows.Forms.ToolStripMenuItem itemcut;
		private System.Windows.Forms.ToolStripMenuItem itemcopy;
		private System.Windows.Forms.ToolStripMenuItem itempaste;
		private System.Windows.Forms.ToolStripStatusLabel configlabel;
		private System.Windows.Forms.ToolStripMenuItem menumode;
		private System.Windows.Forms.ToolStripButton buttonviewnormal;
		private System.Windows.Forms.ToolStripButton buttonviewbrightness;
		private System.Windows.Forms.ToolStripButton buttonviewfloors;
		private System.Windows.Forms.ToolStripButton buttonviewceilings;
		private System.Windows.Forms.ToolStripSeparator separatorgeomergemodes;
		private System.Windows.Forms.ToolStripButton buttonmergegeoclassic;
		private System.Windows.Forms.ToolStripButton buttonmergegeo;
		private System.Windows.Forms.ToolStripButton buttonplacegeo;
		private System.Windows.Forms.ToolStripSeparator seperatortoolsresources;
		private System.Windows.Forms.ToolStripButton buttonscripteditor;
		private System.Windows.Forms.ToolStripMenuItem menuview;
		private System.Windows.Forms.ToolStripMenuItem itemthingsfilter;
		private System.Windows.Forms.ToolStripSeparator seperatorviewthings;
		private System.Windows.Forms.ToolStripMenuItem itemviewnormal;
		private System.Windows.Forms.ToolStripMenuItem itemviewbrightness;
		private System.Windows.Forms.ToolStripMenuItem itemviewfloors;
		private System.Windows.Forms.ToolStripMenuItem itemviewceilings;
		private System.Windows.Forms.ToolStripSeparator seperatorviewzoom;
		private System.Windows.Forms.ToolStripMenuItem itemmergegeoclassic;
		private System.Windows.Forms.ToolStripMenuItem itemmergegeo;
		private System.Windows.Forms.ToolStripMenuItem itemreplacegeo;
		private System.Windows.Forms.ToolStripSeparator separatorgeomerge;
		private System.Windows.Forms.ToolStripMenuItem itemscripteditor;
		private System.Windows.Forms.ToolStripSeparator seperatortoolsconfig;
		private System.Windows.Forms.ToolStripMenuItem itemtestmap;
		private System.Windows.Forms.ToolStripMenuItem menuprefabs;
		private System.Windows.Forms.ToolStripMenuItem itemcreateprefab;
		private System.Windows.Forms.ToolStripSeparator seperatorprefabsinsert;
		private System.Windows.Forms.ToolStripMenuItem iteminsertprefabfile;
		private System.Windows.Forms.ToolStripMenuItem iteminsertpreviousprefab;
		private System.Windows.Forms.ToolStripButton buttoninsertprefabfile;
		private System.Windows.Forms.ToolStripButton buttoninsertpreviousprefab;
		private System.Windows.Forms.Button buttontoggleinfo;
		private System.Windows.Forms.Label labelcollapsedinfo;
		private System.Windows.Forms.Timer statusresetter;
		private System.Windows.Forms.ToolStripMenuItem itemshowerrors;
		private System.Windows.Forms.ToolStripSeparator seperatorviewviews;
		private System.Windows.Forms.ToolStripMenuItem menuzoom;
		private System.Windows.Forms.ToolStripMenuItem item2zoom5;
		private System.Windows.Forms.ToolStripMenuItem item2zoom10;
		private System.Windows.Forms.ToolStripMenuItem itemfittoscreen;
		private System.Windows.Forms.ToolStripMenuItem item2zoom200;
		private System.Windows.Forms.ToolStripMenuItem item2zoom100;
		private System.Windows.Forms.ToolStripMenuItem item2zoom50;
		private System.Windows.Forms.ToolStripMenuItem item2zoom25;
		private System.Windows.Forms.ToolStripMenuItem itemhelprefmanual;
		private System.Windows.Forms.ToolStripSeparator seperatorhelpmanual;
		private System.Windows.Forms.ToolStripMenuItem itemhelpeditmode;
		private System.Windows.Forms.ToolStripMenuItem itemtoggleinfo;
		private System.Windows.Forms.ToolStripMenuItem itempastespecial;
		private System.Windows.Forms.Panel heightpanel1;
		private System.Windows.Forms.Panel dockersspace;
		private CodeImp.DoomBuilder.Controls.DockersControl dockerspanel;
		private System.Windows.Forms.Timer dockerscollapser;
		private System.Windows.Forms.ToolStripSeparator seperatorfile;
		private System.Windows.Forms.ToolStripSeparator seperatorscript;
		private System.Windows.Forms.ToolStripSeparator seperatorprefabs;
		private System.Windows.Forms.ToolStripSeparator seperatorundo;
		private System.Windows.Forms.ToolStripSeparator seperatorcopypaste;
		private System.Windows.Forms.ToolStripSeparator seperatorfileopen;
		private System.Windows.Forms.ToolStripSeparator seperatorfilerecent;
		private System.Windows.Forms.ToolStripSeparator seperatoreditgrid;
		private System.Windows.Forms.ToolStripSeparator seperatoreditcopypaste;
		private System.Windows.Forms.ToolStripSeparator seperatorgeometry;
		private System.Windows.Forms.ToolStripButton buttontogglefog;
		private System.Windows.Forms.ToolStripButton buttontogglesky;
		private System.Windows.Forms.ToolStripButton buttontoggleclassicrendering;
		private System.Windows.Forms.ToolStripStatusLabel warnsLabel;
		private System.Windows.Forms.ToolStripMenuItem itemReloadModedef;
		private System.Windows.Forms.ToolStripMenuItem itemReloadGldefs;
		private System.Windows.Forms.ToolStripSeparator separatorDrawModes;
		private System.Windows.Forms.ToolStripButton buttontoggleeventlines;
		private System.Windows.Forms.ToolStripButton buttontogglevisualvertices;
		private System.Windows.Forms.ToolStripMenuItem itemviewusedtags;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
		private System.Windows.Forms.ToolStripMenuItem addToGroup;
		private System.Windows.Forms.ToolStripMenuItem selectGroup;
		private System.Windows.Forms.ToolStripMenuItem clearGroup;
		private System.Windows.Forms.ContextMenuStrip toolbarContextMenu;
		private System.Windows.Forms.ToolStripMenuItem toggleFile;
		private System.Windows.Forms.ToolStripMenuItem toggleScript;
		private System.Windows.Forms.ToolStripMenuItem toggleUndo;
		private System.Windows.Forms.ToolStripMenuItem toggleCopy;
		private System.Windows.Forms.ToolStripMenuItem togglePrefabs;
		private System.Windows.Forms.ToolStripMenuItem toggleFilter;
		private System.Windows.Forms.ToolStripMenuItem toggleViewModes;
		private System.Windows.Forms.ToolStripMenuItem toggleGeometry;
		private System.Windows.Forms.ToolStripMenuItem toggleTesting;
		private System.Windows.Forms.ToolStripMenuItem toggleRendering;
		private System.Windows.Forms.ToolStripSeparator separatortoolsscreenshots;
		private System.Windows.Forms.ToolStripMenuItem itemsavescreenshot;
		private System.Windows.Forms.ToolStripMenuItem itemsaveeditareascreenshot;
		private System.Windows.Forms.ToolStripMenuItem itemShortcutReference;
		private System.Windows.Forms.ToolStripMenuItem itemopenconfigfolder;
		private System.Windows.Forms.ToolStripMenuItem itemopenmapincurwad;
		private System.Windows.Forms.ToolStripMenuItem itemgrid1;
		private System.Windows.Forms.ToolStripMenuItem itemgrid05;
		private System.Windows.Forms.ToolStripMenuItem itemgrid025;
		private System.Windows.Forms.ToolStripMenuItem itemgrid0125;
		private System.Windows.Forms.ToolStripMenuItem itemzoom400;
		private System.Windows.Forms.Label modename;
		private System.Windows.Forms.ToolStripMenuItem itemautoclearsidetextures;
		private System.Windows.Forms.ToolStripButton buttonautoclearsidetextures;
		private System.Windows.Forms.ToolStripMenuItem itemgotocoords;
		private System.Windows.Forms.ToolStripSeparator separatorTransformModes;
		private System.Windows.Forms.ToolStripMenuItem itemdosnaptogrid;
		private System.Windows.Forms.ToolStrip modestoolbar;
		private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel;
		private System.Windows.Forms.ToolStrip modecontrolsloolbar;
		private System.Windows.Forms.ToolStripMenuItem itemfullbrightness;
		private System.Windows.Forms.ToolStripSeparator separatorhelpers;
		private System.Windows.Forms.ToolStripButton buttonfullbrightness;
		private System.Windows.Forms.ToolStripSeparator separatorfullbrightness;
		private System.Windows.Forms.ToolStripSeparator separatorfilters;
		private System.Windows.Forms.ToolStripSeparator separatorrendering;
		private System.Windows.Forms.ToolStripMenuItem itemdynlightmodes;
		private System.Windows.Forms.ToolStripMenuItem itemnodynlights;
		private System.Windows.Forms.ToolStripMenuItem itemdynlights;
		private System.Windows.Forms.ToolStripMenuItem itemdynlightsanim;
		private System.Windows.Forms.ToolStripMenuItem itemmodelmodes;
		private System.Windows.Forms.ToolStripMenuItem itemnomdl;
		private System.Windows.Forms.ToolStripMenuItem itemselmdl;
		private System.Windows.Forms.ToolStripMenuItem itemfiltermdl;
		private System.Windows.Forms.ToolStripMenuItem itemallmdl;
		private System.Windows.Forms.ToolStripMenuItem itemtogglefog;
		private System.Windows.Forms.ToolStripMenuItem itemtogglesky;
		private System.Windows.Forms.ToolStripMenuItem itemtoggleclassicrendering;
		private System.Windows.Forms.ToolStripMenuItem itemtoggleeventlines;
		private System.Windows.Forms.ToolStripMenuItem itemtogglevisualverts;
		private ToolStripMenuItem itemimport;
		private ToolStripMenuItem itemexport;
		private ToolStripSeparator separatorio;
		private ToolStripMenuItem itemviewthingtypes;
		private StatisticsControl statistics;
		private ToolStripSplitButton dynamiclightmode;
		private ToolStripMenuItem sightsdontshow;
		private ToolStripMenuItem lightsshow;
		private ToolStripMenuItem lightsshowanimated;
		private ToolStripSplitButton modelrendermode;
		private ToolStripMenuItem modelsdontshow;
		private ToolStripMenuItem modelsshowselection;
		private ToolStripMenuItem modelsshowfiltered;
		private ToolStripMenuItem modelsshowall;
		private DebugConsole console;
		private ToolStripMenuItem item2zoom400;
		private ToolStripMenuItem item2zoom800;
		private ToolStripMenuItem itemzoom800;
		private ToolStripButton buttontogglecomments;
		private ToolStripButton buttontogglefixedthingsscale;
		private ToolStripMenuItem itemlinedefcolors;
		private ToolStripSeparator separatorlinecolors;
		private ToolStripButton buttonlinededfcolors;
		private ToolStripDropDownButton linedefcolorpresets;
		private ToolStripDropDownButton thingfilters;
		private ToolStripMenuItem itemtogglegrid;
		private ToolStripMenuItem itemaligngridtolinedef;
		private ToolStripMenuItem itemsetgridorigintovertex;
		private ToolStripMenuItem itemresetgrid;
		private ToolStripButton buttontogglegrid;
		private ToolStripButton buttontoggledynamicgrid;
		private ToolStripMenuItem itemdynamicgridsize;
		private ToolStripMenuItem itemtogglecomments;
		private ToolStripMenuItem itemtogglefixedthingsscale;
		private ToolStripMenuItem itemtogglealwaysshowvertices;
	}
}