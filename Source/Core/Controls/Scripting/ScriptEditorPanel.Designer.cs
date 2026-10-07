namespace CodeImp.DoomBuilder.Controls
{
	partial class ScriptEditorPanel
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ScriptEditorPanel));
			this.toolbar = new System.Windows.Forms.ToolStrip();
			this.buttonsave = new System.Windows.Forms.ToolStripButton();
			this.buttonsaveall = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.buttonundo = new System.Windows.Forms.ToolStripButton();
			this.buttonredo = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.buttoncut = new System.Windows.Forms.ToolStripButton();
			this.buttoncopy = new System.Windows.Forms.ToolStripButton();
			this.buttonpaste = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			this.buttonunindent = new System.Windows.Forms.ToolStripButton();
			this.buttonindent = new System.Windows.Forms.ToolStripButton();
			this.buttonwhitespace = new System.Windows.Forms.ToolStripButton();
			this.buttonwordwrap = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
			this.buttonsnippets = new System.Windows.Forms.ToolStripDropDownButton();
			this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
			this.buttonscriptconfig = new System.Windows.Forms.ToolStripDropDownButton();
			this.buttoncompile = new System.Windows.Forms.ToolStripButton();
			this.buttonkeywordhelp = new System.Windows.Forms.ToolStripButton();
			this.buttonsearch = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
			this.searchbox = new System.Windows.Forms.ToolStripTextBox();
			this.searchprev = new System.Windows.Forms.ToolStripButton();
			this.searchnext = new System.Windows.Forms.ToolStripButton();
			this.searchmatchcase = new System.Windows.Forms.ToolStripButton();
			this.searchwholeword = new System.Windows.Forms.ToolStripButton();
			this.openfile = new System.Windows.Forms.OpenFileDialog();
			this.savefile = new System.Windows.Forms.SaveFileDialog();
			this.errorimages = new System.Windows.Forms.ImageList(this.components);
			this.statusbar = new System.Windows.Forms.StatusStrip();
			this.statuslabel = new System.Windows.Forms.ToolStripStatusLabel();
			this.positionlabel = new System.Windows.Forms.ToolStripStatusLabel();
			this.scripttype = new System.Windows.Forms.ToolStripStatusLabel();
			this.statusflasher = new System.Windows.Forms.Timer(this.components);
			this.statusresetter = new System.Windows.Forms.Timer(this.components);
			this.menustrip = new System.Windows.Forms.MenuStrip();
			this.filemenuitem = new System.Windows.Forms.ToolStripMenuItem();
			this.menusave = new System.Windows.Forms.ToolStripMenuItem();
			this.menusaveall = new System.Windows.Forms.ToolStripMenuItem();
			this.editmenuitem = new System.Windows.Forms.ToolStripMenuItem();
			this.menuundo = new System.Windows.Forms.ToolStripMenuItem();
			this.menuredo = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator10 = new System.Windows.Forms.ToolStripSeparator();
			this.menucut = new System.Windows.Forms.ToolStripMenuItem();
			this.menucopy = new System.Windows.Forms.ToolStripMenuItem();
			this.menupaste = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
			this.menusnippets = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator12 = new System.Windows.Forms.ToolStripSeparator();
			this.menuindent = new System.Windows.Forms.ToolStripMenuItem();
			this.menuunindent = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator14 = new System.Windows.Forms.ToolStripSeparator();
			this.menugotoline = new System.Windows.Forms.ToolStripMenuItem();
			this.menuduplicateline = new System.Windows.Forms.ToolStripMenuItem();
			this.viewmenuitem = new System.Windows.Forms.ToolStripMenuItem();
			this.menuwhitespace = new System.Windows.Forms.ToolStripMenuItem();
			this.menuwordwrap = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator11 = new System.Windows.Forms.ToolStripSeparator();
			this.menustayontop = new System.Windows.Forms.ToolStripMenuItem();
			this.searchmenuitem = new System.Windows.Forms.ToolStripMenuItem();
			this.menufind = new System.Windows.Forms.ToolStripMenuItem();
			this.menufindnext = new System.Windows.Forms.ToolStripMenuItem();
			this.menufindprevious = new System.Windows.Forms.ToolStripMenuItem();
			this.toolsmenu = new System.Windows.Forms.ToolStripMenuItem();
			this.menucompile = new System.Windows.Forms.ToolStripMenuItem();
			this.scripticons = new System.Windows.Forms.ImageList(this.components);
			this.projecticons = new System.Windows.Forms.ImageList(this.components);
			this.infoicons = new System.Windows.Forms.ImageList(this.components);
			this.scriptsplitter = new CodeImp.DoomBuilder.Controls.CollapsibleSplitContainer();
			this.tabs = new CodeImp.DoomBuilder.Controls.VSTabControl();
#if !MONO_WINFORMS
			this.infotabs = new Dotnetrix.Controls.TabControlEX();
#else
			this.infotabs = new System.Windows.Forms.TabControl();
#endif
			this.taberrors = new System.Windows.Forms.TabPage();
			this.errorlist = new System.Windows.Forms.ListView();
			this.colIndex = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.colDescription = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.colFile = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.tabsearchresults = new System.Windows.Forms.TabPage();
			this.toolbar.SuspendLayout();
			this.statusbar.SuspendLayout();
			this.menustrip.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.scriptsplitter)).BeginInit();
			this.scriptsplitter.Panel1.SuspendLayout();
			this.scriptsplitter.Panel2.SuspendLayout();
			this.scriptsplitter.SuspendLayout();
			this.infotabs.SuspendLayout();
			this.taberrors.SuspendLayout();
			this.SuspendLayout();
			// 
			// toolbar
			// 
			this.toolbar.AllowMerge = false;
			this.toolbar.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.toolbar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buttonsave,
            this.buttonsaveall,
            this.toolStripSeparator1,
            this.buttonundo,
            this.buttonredo,
            this.toolStripSeparator2,
            this.buttoncut,
            this.buttoncopy,
            this.buttonpaste,
            this.toolStripSeparator3,
            this.buttonunindent,
            this.buttonindent,
            this.buttonwhitespace,
            this.buttonwordwrap,
            this.toolStripSeparator6,
            this.buttonsnippets,
            this.toolStripSeparator4,
            this.buttonscriptconfig,
            this.buttoncompile,
            this.buttonkeywordhelp,
            this.buttonsearch,
            this.toolStripSeparator5,
            this.searchbox,
            this.searchprev,
            this.searchnext,
            this.searchmatchcase,
            this.searchwholeword});
			this.toolbar.Location = new System.Drawing.Point(0, 24);
			this.toolbar.Name = "toolbar";
			this.toolbar.Size = new System.Drawing.Size(928, 25);
			this.toolbar.TabIndex = 1;
			// 
			// buttonsave
			// 
			this.buttonsave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonsave.Enabled = false;
			this.buttonsave.Image = global::CodeImp.DoomBuilder.Properties.Resources.SaveMap;
			this.buttonsave.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonsave.Name = "buttonsave";
			this.buttonsave.Size = new System.Drawing.Size(23, 22);
			this.buttonsave.Text = "Save File";
			this.buttonsave.Click += new System.EventHandler(this.buttonsave_Click);
			// 
			// buttonsaveall
			// 
			this.buttonsaveall.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonsaveall.Enabled = false;
			this.buttonsaveall.Image = global::CodeImp.DoomBuilder.Properties.Resources.SaveAll;
			this.buttonsaveall.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonsaveall.Name = "buttonsaveall";
			this.buttonsaveall.Size = new System.Drawing.Size(23, 22);
			this.buttonsaveall.Text = "Save All Files";
			this.buttonsaveall.Click += new System.EventHandler(this.buttonsaveall_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
			// 
			// buttonundo
			// 
			this.buttonundo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonundo.Image = global::CodeImp.DoomBuilder.Properties.Resources.Undo;
			this.buttonundo.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonundo.Name = "buttonundo";
			this.buttonundo.Size = new System.Drawing.Size(23, 22);
			this.buttonundo.Text = "Undo";
			this.buttonundo.Click += new System.EventHandler(this.buttonundo_Click);
			// 
			// buttonredo
			// 
			this.buttonredo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonredo.Image = global::CodeImp.DoomBuilder.Properties.Resources.Redo;
			this.buttonredo.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonredo.Name = "buttonredo";
			this.buttonredo.Size = new System.Drawing.Size(23, 22);
			this.buttonredo.Text = "Redo";
			this.buttonredo.Click += new System.EventHandler(this.buttonredo_Click);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
			// 
			// buttoncut
			// 
			this.buttoncut.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttoncut.Image = global::CodeImp.DoomBuilder.Properties.Resources.Cut;
			this.buttoncut.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttoncut.Name = "buttoncut";
			this.buttoncut.Size = new System.Drawing.Size(23, 22);
			this.buttoncut.Text = "Cut Selection";
			this.buttoncut.Click += new System.EventHandler(this.buttoncut_Click);
			// 
			// buttoncopy
			// 
			this.buttoncopy.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttoncopy.Image = global::CodeImp.DoomBuilder.Properties.Resources.Copy;
			this.buttoncopy.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttoncopy.Name = "buttoncopy";
			this.buttoncopy.Size = new System.Drawing.Size(23, 22);
			this.buttoncopy.Text = "Copy Selection";
			this.buttoncopy.Click += new System.EventHandler(this.buttoncopy_Click);
			// 
			// buttonpaste
			// 
			this.buttonpaste.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonpaste.Image = global::CodeImp.DoomBuilder.Properties.Resources.Paste;
			this.buttonpaste.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonpaste.Name = "buttonpaste";
			this.buttonpaste.Size = new System.Drawing.Size(23, 22);
			this.buttonpaste.Text = "Paste";
			this.buttonpaste.Click += new System.EventHandler(this.buttonpaste_Click);
			// 
			// toolStripSeparator3
			// 
			this.toolStripSeparator3.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
			// 
			// buttonunindent
			// 
			this.buttonunindent.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonunindent.Image = global::CodeImp.DoomBuilder.Properties.Resources.TextUnindent;
			this.buttonunindent.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonunindent.Name = "buttonunindent";
			this.buttonunindent.Size = new System.Drawing.Size(23, 22);
			this.buttonunindent.Text = "Unindent selection";
			this.buttonunindent.Click += new System.EventHandler(this.buttonunindent_Click);
			// 
			// buttonindent
			// 
			this.buttonindent.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonindent.Image = global::CodeImp.DoomBuilder.Properties.Resources.TextIndent;
			this.buttonindent.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonindent.Name = "buttonindent";
			this.buttonindent.Size = new System.Drawing.Size(23, 22);
			this.buttonindent.Text = "Indent selection";
			this.buttonindent.Click += new System.EventHandler(this.buttonindent_Click);
			// 
			// buttonwhitespace
			// 
			this.buttonwhitespace.CheckOnClick = true;
			this.buttonwhitespace.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonwhitespace.Image = global::CodeImp.DoomBuilder.Properties.Resources.TextWhitespace;
			this.buttonwhitespace.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonwhitespace.Name = "buttonwhitespace";
			this.buttonwhitespace.Size = new System.Drawing.Size(23, 22);
			this.buttonwhitespace.Text = "Show whitespace";
			this.buttonwhitespace.Click += new System.EventHandler(this.buttonwhitespace_Click);
			// 
			// buttonwordwrap
			// 
			this.buttonwordwrap.CheckOnClick = true;
			this.buttonwordwrap.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonwordwrap.Image = global::CodeImp.DoomBuilder.Properties.Resources.WordWrap;
			this.buttonwordwrap.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonwordwrap.Name = "buttonwordwrap";
			this.buttonwordwrap.Size = new System.Drawing.Size(23, 22);
			this.buttonwordwrap.Text = "Wrap long lines";
			this.buttonwordwrap.Click += new System.EventHandler(this.buttonwordwrap_Click);
			// 
			// toolStripSeparator6
			// 
			this.toolStripSeparator6.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
			this.toolStripSeparator6.Name = "toolStripSeparator6";
			this.toolStripSeparator6.Size = new System.Drawing.Size(6, 25);
			// 
			// buttonsnippets
			// 
			this.buttonsnippets.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonsnippets.Enabled = false;
			this.buttonsnippets.Image = global::CodeImp.DoomBuilder.Properties.Resources.PuzzlePiece;
			this.buttonsnippets.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonsnippets.Name = "buttonsnippets";
			this.buttonsnippets.Size = new System.Drawing.Size(29, 22);
			this.buttonsnippets.Text = "Insert a Code Snippet";
			// 
			// toolStripSeparator4
			// 
			this.toolStripSeparator4.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
			this.toolStripSeparator4.Name = "toolStripSeparator4";
			this.toolStripSeparator4.Size = new System.Drawing.Size(6, 25);
			// 
			// buttonscriptconfig
			// 
			this.buttonscriptconfig.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonscriptconfig.Enabled = false;
			this.buttonscriptconfig.Image = global::CodeImp.DoomBuilder.Properties.Resources.ScriptPalette;
			this.buttonscriptconfig.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonscriptconfig.Name = "buttonscriptconfig";
			this.buttonscriptconfig.Size = new System.Drawing.Size(29, 22);
			this.buttonscriptconfig.Text = "Change Script Type";
			// 
			// buttoncompile
			// 
			this.buttoncompile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttoncompile.Image = global::CodeImp.DoomBuilder.Properties.Resources.ScriptCompile;
			this.buttoncompile.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttoncompile.Name = "buttoncompile";
			this.buttoncompile.Size = new System.Drawing.Size(23, 22);
			this.buttoncompile.Text = "Compile Script";
			this.buttoncompile.Click += new System.EventHandler(this.buttoncompile_Click);
			// 
			// buttonkeywordhelp
			// 
			this.buttonkeywordhelp.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
			this.buttonkeywordhelp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonkeywordhelp.Image = global::CodeImp.DoomBuilder.Properties.Resources.ScriptHelp;
			this.buttonkeywordhelp.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonkeywordhelp.Name = "buttonkeywordhelp";
			this.buttonkeywordhelp.Size = new System.Drawing.Size(23, 22);
			this.buttonkeywordhelp.Text = "Keyword Help";
			this.buttonkeywordhelp.Click += new System.EventHandler(this.buttonkeywordhelp_Click);
			// 
			// buttonsearch
			// 
			this.buttonsearch.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
			this.buttonsearch.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonsearch.Image = global::CodeImp.DoomBuilder.Properties.Resources.Search;
			this.buttonsearch.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonsearch.Name = "buttonsearch";
			this.buttonsearch.Size = new System.Drawing.Size(23, 22);
			this.buttonsearch.Text = "Open Find and Replace Window";
			this.buttonsearch.Click += new System.EventHandler(this.buttonsearch_Click);
			// 
			// toolStripSeparator5
			// 
			this.toolStripSeparator5.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
			this.toolStripSeparator5.Name = "toolStripSeparator5";
			this.toolStripSeparator5.Size = new System.Drawing.Size(6, 25);
			// 
			// searchbox
			// 
			this.searchbox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.searchbox.Name = "searchbox";
			this.searchbox.Size = new System.Drawing.Size(100, 25);
			this.searchbox.ToolTipText = "Quick search";
			this.searchbox.TextChanged += new System.EventHandler(this.searchbox_TextChanged);
			// 
			// searchprev
			// 
			this.searchprev.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.searchprev.Enabled = false;
			this.searchprev.Image = global::CodeImp.DoomBuilder.Properties.Resources.SearchPrev;
			this.searchprev.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.searchprev.Name = "searchprev";
			this.searchprev.Size = new System.Drawing.Size(23, 22);
			this.searchprev.ToolTipText = "Previous Match";
			this.searchprev.Click += new System.EventHandler(this.searchprev_Click);
			// 
			// searchnext
			// 
			this.searchnext.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.searchnext.Enabled = false;
			this.searchnext.Image = global::CodeImp.DoomBuilder.Properties.Resources.SearchNext;
			this.searchnext.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.searchnext.Name = "searchnext";
			this.searchnext.Size = new System.Drawing.Size(23, 22);
			this.searchnext.ToolTipText = "Next Match";
			this.searchnext.Click += new System.EventHandler(this.searchnext_Click);
			// 
			// searchmatchcase
			// 
			this.searchmatchcase.CheckOnClick = true;
			this.searchmatchcase.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.searchmatchcase.Image = global::CodeImp.DoomBuilder.Properties.Resources.SearchMatchCase;
			this.searchmatchcase.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.searchmatchcase.Name = "searchmatchcase";
			this.searchmatchcase.Size = new System.Drawing.Size(23, 22);
			this.searchmatchcase.ToolTipText = "Match Case";
			this.searchmatchcase.Click += new System.EventHandler(this.searchbox_TextChanged);
			// 
			// searchwholeword
			// 
			this.searchwholeword.CheckOnClick = true;
			this.searchwholeword.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.searchwholeword.Image = global::CodeImp.DoomBuilder.Properties.Resources.SearchMatch;
			this.searchwholeword.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.searchwholeword.Name = "searchwholeword";
			this.searchwholeword.Size = new System.Drawing.Size(23, 22);
			this.searchwholeword.ToolTipText = "Match Whole Word";
			this.searchwholeword.Click += new System.EventHandler(this.searchbox_TextChanged);
			// 
			// openfile
			// 
			this.openfile.Multiselect = true;
			this.openfile.Title = "Open Script";
			// 
			// savefile
			// 
			this.savefile.Title = "Save Script As";
			// 
			// errorimages
			// 
			this.errorimages.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("errorimages.ImageStream")));
			this.errorimages.TransparentColor = System.Drawing.Color.Transparent;
			this.errorimages.Images.SetKeyName(0, "ScriptError3.png");
			// 
			// statusbar
			// 
			this.statusbar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statuslabel,
            this.positionlabel,
            this.scripttype});
			this.statusbar.Location = new System.Drawing.Point(0, 498);
			this.statusbar.Name = "statusbar";
			this.statusbar.Size = new System.Drawing.Size(928, 22);
			this.statusbar.TabIndex = 3;
			this.statusbar.Text = "statusStrip1";
			// 
			// statuslabel
			// 
			this.statuslabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
			this.statuslabel.Image = global::CodeImp.DoomBuilder.Properties.Resources.Status0;
			this.statuslabel.Margin = new System.Windows.Forms.Padding(3, 3, 0, 2);
			this.statuslabel.Name = "statuslabel";
			this.statuslabel.Size = new System.Drawing.Size(59, 17);
			this.statuslabel.Text = "Ready.";
			// 
			// positionlabel
			// 
			this.positionlabel.Name = "positionlabel";
			this.positionlabel.Size = new System.Drawing.Size(764, 17);
			this.positionlabel.Spring = true;
			this.positionlabel.Text = "100 : 12 (120)";
			this.positionlabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// scripttype
			// 
			this.scripttype.Margin = new System.Windows.Forms.Padding(30, 3, 0, 2);
			this.scripttype.Name = "scripttype";
			this.scripttype.Size = new System.Drawing.Size(57, 17);
			this.scripttype.Text = "Plain Text";
			this.scripttype.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// statusflasher
			// 
			this.statusflasher.Tick += new System.EventHandler(this.statusflasher_Tick);
			// 
			// statusresetter
			// 
			this.statusresetter.Tick += new System.EventHandler(this.statusresetter_Tick);
			// 
			// menustrip
			// 
			this.menustrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.filemenuitem,
            this.editmenuitem,
            this.viewmenuitem,
            this.searchmenuitem,
            this.toolsmenu});
			this.menustrip.Location = new System.Drawing.Point(0, 0);
			this.menustrip.Name = "menustrip";
			this.menustrip.Size = new System.Drawing.Size(928, 24);
			this.menustrip.TabIndex = 4;
			this.menustrip.Text = "menuStrip1";
			// 
			// filemenuitem
			// 
			this.filemenuitem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menusave,
            this.menusaveall});
			this.filemenuitem.Name = "filemenuitem";
			this.filemenuitem.Size = new System.Drawing.Size(37, 20);
			this.filemenuitem.Text = "File";
			this.filemenuitem.DropDownOpening += new System.EventHandler(this.filemenuitem_DropDownOpening);
			// 
			// menusave
			// 
			this.menusave.Image = global::CodeImp.DoomBuilder.Properties.Resources.SaveMap;
			this.menusave.Name = "menusave";
			this.menusave.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
			this.menusave.Size = new System.Drawing.Size(185, 22);
			this.menusave.Text = "Save";
			this.menusave.Click += new System.EventHandler(this.buttonsave_Click);
			// 
			// menusaveall
			// 
			this.menusaveall.Image = global::CodeImp.DoomBuilder.Properties.Resources.SaveAll;
			this.menusaveall.Name = "menusaveall";
			this.menusaveall.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.S)));
			this.menusaveall.Size = new System.Drawing.Size(185, 22);
			this.menusaveall.Text = "Save all";
			this.menusaveall.Click += new System.EventHandler(this.buttonsaveall_Click);
			// 
			// editmenuitem
			// 
			this.editmenuitem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuundo,
            this.menuredo,
            this.toolStripSeparator10,
            this.menucut,
            this.menucopy,
            this.menupaste,
            this.toolStripSeparator9,
            this.menusnippets,
            this.toolStripSeparator12,
            this.menuindent,
            this.menuunindent,
            this.toolStripSeparator14,
            this.menugotoline,
            this.menuduplicateline});
			this.editmenuitem.Name = "editmenuitem";
			this.editmenuitem.Size = new System.Drawing.Size(39, 20);
			this.editmenuitem.Text = "Edit";
			this.editmenuitem.DropDownOpening += new System.EventHandler(this.editmenuitem_DropDownOpening);
			// 
			// menuundo
			// 
			this.menuundo.Image = global::CodeImp.DoomBuilder.Properties.Resources.Undo;
			this.menuundo.Name = "menuundo";
			this.menuundo.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
			this.menuundo.Size = new System.Drawing.Size(188, 22);
			this.menuundo.Text = "Undo";
			this.menuundo.Click += new System.EventHandler(this.buttonundo_Click);
			// 
			// menuredo
			// 
			this.menuredo.Image = global::CodeImp.DoomBuilder.Properties.Resources.Redo;
			this.menuredo.Name = "menuredo";
			this.menuredo.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Y)));
			this.menuredo.Size = new System.Drawing.Size(188, 22);
			this.menuredo.Text = "Redo";
			this.menuredo.Click += new System.EventHandler(this.buttonredo_Click);
			// 
			// toolStripSeparator10
			// 
			this.toolStripSeparator10.Name = "toolStripSeparator10";
			this.toolStripSeparator10.Size = new System.Drawing.Size(185, 6);
			// 
			// menucut
			// 
			this.menucut.Image = global::CodeImp.DoomBuilder.Properties.Resources.Cut;
			this.menucut.Name = "menucut";
			this.menucut.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
			this.menucut.Size = new System.Drawing.Size(188, 22);
			this.menucut.Text = "Cut";
			this.menucut.Click += new System.EventHandler(this.buttoncut_Click);
			// 
			// menucopy
			// 
			this.menucopy.Image = global::CodeImp.DoomBuilder.Properties.Resources.Copy;
			this.menucopy.Name = "menucopy";
			this.menucopy.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
			this.menucopy.Size = new System.Drawing.Size(188, 22);
			this.menucopy.Text = "Copy";
			this.menucopy.Click += new System.EventHandler(this.buttoncopy_Click);
			// 
			// menupaste
			// 
			this.menupaste.Image = global::CodeImp.DoomBuilder.Properties.Resources.Paste;
			this.menupaste.Name = "menupaste";
			this.menupaste.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
			this.menupaste.Size = new System.Drawing.Size(188, 22);
			this.menupaste.Text = "Paste";
			this.menupaste.Click += new System.EventHandler(this.buttonpaste_Click);
			// 
			// toolStripSeparator9
			// 
			this.toolStripSeparator9.Name = "toolStripSeparator9";
			this.toolStripSeparator9.Size = new System.Drawing.Size(185, 6);
			// 
			// menusnippets
			// 
			this.menusnippets.Image = global::CodeImp.DoomBuilder.Properties.Resources.PuzzlePiece;
			this.menusnippets.Name = "menusnippets";
			this.menusnippets.Size = new System.Drawing.Size(188, 22);
			this.menusnippets.Text = "Insert snippet";
			// 
			// toolStripSeparator12
			// 
			this.toolStripSeparator12.Name = "toolStripSeparator12";
			this.toolStripSeparator12.Size = new System.Drawing.Size(185, 6);
			// 
			// menuindent
			// 
			this.menuindent.Image = global::CodeImp.DoomBuilder.Properties.Resources.TextIndent;
			this.menuindent.Name = "menuindent";
			this.menuindent.Size = new System.Drawing.Size(188, 22);
			this.menuindent.Text = "Increase indentation";
			this.menuindent.Click += new System.EventHandler(this.buttonindent_Click);
			// 
			// menuunindent
			// 
			this.menuunindent.Image = global::CodeImp.DoomBuilder.Properties.Resources.TextUnindent;
			this.menuunindent.Name = "menuunindent";
			this.menuunindent.Size = new System.Drawing.Size(188, 22);
			this.menuunindent.Text = "Decrease indentation";
			this.menuunindent.Click += new System.EventHandler(this.buttonunindent_Click);
			// 
			// toolStripSeparator14
			// 
			this.toolStripSeparator14.Name = "toolStripSeparator14";
			this.toolStripSeparator14.Size = new System.Drawing.Size(185, 6);
			// 
			// menugotoline
			// 
			this.menugotoline.Name = "menugotoline";
			this.menugotoline.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.G)));
			this.menugotoline.Size = new System.Drawing.Size(188, 22);
			this.menugotoline.Text = "Go to line...";
			this.menugotoline.Click += new System.EventHandler(this.menugotoline_Click);
			// 
			// menuduplicateline
			// 
			this.menuduplicateline.Name = "menuduplicateline";
			this.menuduplicateline.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D)));
			this.menuduplicateline.Size = new System.Drawing.Size(188, 22);
			this.menuduplicateline.Text = "Duplicate line";
			this.menuduplicateline.Click += new System.EventHandler(this.menuduplicateline_Click);
			// 
			// viewmenuitem
			// 
			this.viewmenuitem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuwhitespace,
            this.menuwordwrap,
            this.toolStripSeparator11,
            this.menustayontop});
			this.viewmenuitem.Name = "viewmenuitem";
			this.viewmenuitem.Size = new System.Drawing.Size(44, 20);
			this.viewmenuitem.Text = "View";
			// 
			// menuwhitespace
			// 
			this.menuwhitespace.CheckOnClick = true;
			this.menuwhitespace.Image = global::CodeImp.DoomBuilder.Properties.Resources.TextWhitespace;
			this.menuwhitespace.Name = "menuwhitespace";
			this.menuwhitespace.Size = new System.Drawing.Size(165, 22);
			this.menuwhitespace.Text = "Show whitespace";
			this.menuwhitespace.Click += new System.EventHandler(this.buttonwhitespace_Click);
			// 
			// menuwordwrap
			// 
			this.menuwordwrap.CheckOnClick = true;
			this.menuwordwrap.Image = global::CodeImp.DoomBuilder.Properties.Resources.WordWrap;
			this.menuwordwrap.Name = "menuwordwrap";
			this.menuwordwrap.Size = new System.Drawing.Size(165, 22);
			this.menuwordwrap.Text = "Show word wrap";
			this.menuwordwrap.Click += new System.EventHandler(this.buttonwordwrap_Click);
			// 
			// toolStripSeparator11
			// 
			this.toolStripSeparator11.Name = "toolStripSeparator11";
			this.toolStripSeparator11.Size = new System.Drawing.Size(162, 6);
			// 
			// menustayontop
			// 
			this.menustayontop.CheckOnClick = true;
			this.menustayontop.Image = global::CodeImp.DoomBuilder.Properties.Resources.Pin;
			this.menustayontop.Name = "menustayontop";
			this.menustayontop.Size = new System.Drawing.Size(165, 22);
			this.menustayontop.Text = "Always on top";
			this.menustayontop.Click += new System.EventHandler(this.menustayontop_Click);
			// 
			// searchmenuitem
			// 
			this.searchmenuitem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menufind,
            this.menufindnext,
            this.menufindprevious});
			this.searchmenuitem.Name = "searchmenuitem";
			this.searchmenuitem.Size = new System.Drawing.Size(54, 20);
			this.searchmenuitem.Text = "Search";
			this.searchmenuitem.DropDownOpening += new System.EventHandler(this.searchmenuitem_DropDownOpening);
			// 
			// menufind
			// 
			this.menufind.Image = global::CodeImp.DoomBuilder.Properties.Resources.Search;
			this.menufind.Name = "menufind";
			this.menufind.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F)));
			this.menufind.Size = new System.Drawing.Size(164, 22);
			this.menufind.Text = "Find...";
			this.menufind.Click += new System.EventHandler(this.buttonsearch_Click);
			// 
			// menufindnext
			// 
			this.menufindnext.Image = global::CodeImp.DoomBuilder.Properties.Resources.SearchNext;
			this.menufindnext.Name = "menufindnext";
			this.menufindnext.ShortcutKeys = System.Windows.Forms.Keys.F3;
			this.menufindnext.Size = new System.Drawing.Size(164, 22);
			this.menufindnext.Text = "Find next";
			this.menufindnext.Click += new System.EventHandler(this.searchnext_Click);
			// 
			// menufindprevious
			// 
			this.menufindprevious.Image = global::CodeImp.DoomBuilder.Properties.Resources.SearchPrev;
			this.menufindprevious.Name = "menufindprevious";
			this.menufindprevious.ShortcutKeys = System.Windows.Forms.Keys.F2;
			this.menufindprevious.Size = new System.Drawing.Size(164, 22);
			this.menufindprevious.Text = "Find previous";
			this.menufindprevious.Click += new System.EventHandler(this.searchprev_Click);
			// 
			// toolsmenu
			// 
			this.toolsmenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menucompile});
			this.toolsmenu.Name = "toolsmenu";
			this.toolsmenu.Size = new System.Drawing.Size(46, 20);
			this.toolsmenu.Text = "Tools";
			this.toolsmenu.DropDownOpening += new System.EventHandler(this.toolsmenu_DropDownOpening);
			// 
			// menucompile
			// 
			this.menucompile.Image = global::CodeImp.DoomBuilder.Properties.Resources.ScriptCompile;
			this.menucompile.Name = "menucompile";
			this.menucompile.ShortcutKeys = System.Windows.Forms.Keys.F5;
			this.menucompile.Size = new System.Drawing.Size(138, 22);
			this.menucompile.Text = "Compile";
			this.menucompile.Click += new System.EventHandler(this.buttoncompile_Click);
			// 
			// scripticons
			// 
			this.scripticons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("scripticons.ImageStream")));
			this.scripticons.TransparentColor = System.Drawing.Color.Transparent;
			this.scripticons.Images.SetKeyName(0, "File.ico");
			this.scripticons.Images.SetKeyName(1, "GroupUnknown.png");
			this.scripticons.Images.SetKeyName(2, "PK3.ico");
			this.scripticons.Images.SetKeyName(3, "ResourceMAP.png");
			this.scripticons.Images.SetKeyName(4, "Script.png");
			this.scripticons.Images.SetKeyName(5, "ScriptACS.png");
			this.scripticons.Images.SetKeyName(6, "ScriptMODELDEF.png");
			this.scripticons.Images.SetKeyName(7, "ScriptDECORATE.png");
			this.scripticons.Images.SetKeyName(8, "ScriptGLDEFS.png");
			this.scripticons.Images.SetKeyName(9, "ScriptSNDSEQ.png");
			this.scripticons.Images.SetKeyName(10, "ScriptMAPINFO.png");
			this.scripticons.Images.SetKeyName(11, "ScriptVOXELDEF.png");
			this.scripticons.Images.SetKeyName(12, "ScriptTEXTURES.png");
			this.scripticons.Images.SetKeyName(13, "ScriptANIMDEFS.png");
			this.scripticons.Images.SetKeyName(14, "ScriptREVERBS.png");
			this.scripticons.Images.SetKeyName(15, "ScriptTERRAIN.png");
			this.scripticons.Images.SetKeyName(16, "ScriptX11.png");
			this.scripticons.Images.SetKeyName(17, "ScriptCVARINFO.png");
			this.scripticons.Images.SetKeyName(18, "ScriptSNDINFO.png");
			this.scripticons.Images.SetKeyName(19, "ScriptLOCKDEFS.png");
			this.scripticons.Images.SetKeyName(20, "ScriptMENUDEF.png");
			this.scripticons.Images.SetKeyName(21, "ScriptSBARINFO.png");
			this.scripticons.Images.SetKeyName(22, "ScriptUSDF.png");
			this.scripticons.Images.SetKeyName(23, "ScriptGAMEINFO.png");
			this.scripticons.Images.SetKeyName(24, "ScriptKEYCONF.png");
			this.scripticons.Images.SetKeyName(25, "ScriptFONTDEFS.png");
			this.scripticons.Images.SetKeyName(26, "ScriptZSCRIPT.png");
			this.scripticons.Images.SetKeyName(27, "ScriptDECALDEF.png");
			this.scripticons.Images.SetKeyName(28, "GroupUnknown.png");
			this.scripticons.Images.SetKeyName(29, "GroupACS.png");
			this.scripticons.Images.SetKeyName(30, "GroupMODELDEF.png");
			this.scripticons.Images.SetKeyName(31, "GroupDECORATE.png");
			this.scripticons.Images.SetKeyName(32, "GroupGLDEFS.png");
			this.scripticons.Images.SetKeyName(33, "GroupSNDSEQ.png");
			this.scripticons.Images.SetKeyName(34, "GroupMAPINFO.png");
			this.scripticons.Images.SetKeyName(35, "GroupVOXELDEF.png");
			this.scripticons.Images.SetKeyName(36, "GroupTEXTURES.png");
			this.scripticons.Images.SetKeyName(37, "GroupANIMDEFS.png");
			this.scripticons.Images.SetKeyName(38, "GroupREVERBS.png");
			this.scripticons.Images.SetKeyName(39, "GroupTERRAIN.png");
			this.scripticons.Images.SetKeyName(40, "GroupX11.png");
			this.scripticons.Images.SetKeyName(41, "GroupCVARINFO.png");
			this.scripticons.Images.SetKeyName(42, "GroupSNDINFO.png");
			this.scripticons.Images.SetKeyName(43, "GroupLOCKDEFS.png");
			this.scripticons.Images.SetKeyName(44, "GroupMENUDEF.png");
			this.scripticons.Images.SetKeyName(45, "GroupSBARINFO.png");
			this.scripticons.Images.SetKeyName(46, "GroupUSDF.png");
			this.scripticons.Images.SetKeyName(47, "GroupGAMEINFO.png");
			this.scripticons.Images.SetKeyName(48, "GroupKEYCONF.png");
			this.scripticons.Images.SetKeyName(49, "GroupFONTDEFS.png");
			this.scripticons.Images.SetKeyName(50, "GroupZSCRIPT.png");
			this.scripticons.Images.SetKeyName(51, "GroupDECALDEF.png");
			this.scripticons.Images.SetKeyName(52, "GroupOpen.png");
			this.scripticons.Images.SetKeyName(53, "OpenGroupACS.png");
			this.scripticons.Images.SetKeyName(54, "OpenGroupMODELDEF.png");
			this.scripticons.Images.SetKeyName(55, "OpenGroupDECORATE.png");
			this.scripticons.Images.SetKeyName(56, "OpenGroupGLDEFS.png");
			this.scripticons.Images.SetKeyName(57, "OpenGroupSNDSEQ.png");
			this.scripticons.Images.SetKeyName(58, "OpenGroupMAPINFO.png");
			this.scripticons.Images.SetKeyName(59, "OpenGroupVOXELDEF.png");
			this.scripticons.Images.SetKeyName(60, "OpenGroupTEXTURES.png");
			this.scripticons.Images.SetKeyName(61, "OpenGroupANIMDEFS.png");
			this.scripticons.Images.SetKeyName(62, "OpenGroupREVERBS.png");
			this.scripticons.Images.SetKeyName(63, "OpenGroupTERRAIN.png");
			this.scripticons.Images.SetKeyName(64, "OpenGroupX11.png");
			this.scripticons.Images.SetKeyName(65, "OpenGroupCVARINFO.png");
			this.scripticons.Images.SetKeyName(66, "OpenGroupSNDINFO.png");
			this.scripticons.Images.SetKeyName(67, "OpenGroupLOCKDEFS.png");
			this.scripticons.Images.SetKeyName(68, "OpenGroupMENUDEF.png");
			this.scripticons.Images.SetKeyName(69, "OpenGroupSBARINFO.png");
			this.scripticons.Images.SetKeyName(70, "OpenGroupUSDF.png");
			this.scripticons.Images.SetKeyName(71, "OpenGroupGAMEINFO.png");
			this.scripticons.Images.SetKeyName(72, "OpenGroupKEYCONF.png");
			this.scripticons.Images.SetKeyName(73, "OpenGroupFONTDEFS.png");
			this.scripticons.Images.SetKeyName(74, "OpenGroupZSCRIPT.png");
			this.scripticons.Images.SetKeyName(75, "OpenGroupDECALDEF.png");
			// 
			// projecticons
			// 
			this.projecticons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("projecticons.ImageStream")));
			this.projecticons.TransparentColor = System.Drawing.Color.Transparent;
			this.projecticons.Images.SetKeyName(0, "Resources.png");
			// 
			// infoicons
			// 
			this.infoicons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("infoicons.ImageStream")));
			this.infoicons.TransparentColor = System.Drawing.Color.Transparent;
			this.infoicons.Images.SetKeyName(0, "ScriptError.png");
			this.infoicons.Images.SetKeyName(1, "Search.png");
			// 
			// scriptsplitter
			// 
			this.scriptsplitter.Cursor = System.Windows.Forms.Cursors.Default;
			this.scriptsplitter.Dock = System.Windows.Forms.DockStyle.Fill;
			this.scriptsplitter.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
			this.scriptsplitter.Location = new System.Drawing.Point(0, 49);
			this.scriptsplitter.Name = "scriptsplitter";
			this.scriptsplitter.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// scriptsplitter.Panel1
			// 
			this.scriptsplitter.Panel1.Controls.Add(this.tabs);
			// 
			// scriptsplitter.Panel2
			// 
			this.scriptsplitter.Panel2.Controls.Add(this.infotabs);
			this.scriptsplitter.Panel2MinSize = 100;
			this.scriptsplitter.Size = new System.Drawing.Size(928, 449);
			this.scriptsplitter.SplitterDistance = 254;
			this.scriptsplitter.SplitterWidth = 8;
			this.scriptsplitter.TabIndex = 5;
			// 
			// tabs
			// 
			this.tabs.ActiveColor = System.Drawing.SystemColors.HotTrack;
			this.tabs.AllowDrop = true;
			this.tabs.BackTabColor = System.Drawing.SystemColors.Control;
			this.tabs.BorderColor = System.Drawing.SystemColors.ControlDark;
			this.tabs.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabs.HighlightColor = System.Drawing.SystemColors.Highlight;
			this.tabs.ItemSize = new System.Drawing.Size(240, 22);
			this.tabs.Location = new System.Drawing.Point(0, 0);
			this.tabs.Name = "tabs";
			this.tabs.Padding = new System.Drawing.Point(12, 3);
			this.tabs.SelectedIndex = 0;
			this.tabs.SelectedTextColor = System.Drawing.SystemColors.HighlightText;
			this.tabs.ShowClosingButton = true;
			this.tabs.ShowToolTips = true;
			this.tabs.Size = new System.Drawing.Size(928, 254);
			this.tabs.TabIndex = 0;
			this.tabs.TabStop = false;
			this.tabs.TextColor = System.Drawing.SystemColors.WindowText;
			this.tabs.Selecting += new System.Windows.Forms.TabControlCancelEventHandler(this.tabs_Selecting);
			// 
			// infotabs
			// 
#if !MONO_WINFORMS
			this.infotabs.Alignment = System.Windows.Forms.TabAlignment.Bottom;
			this.infotabs.Appearance = Dotnetrix.Controls.TabAppearanceEX.FlatTab;
			this.infotabs.Controls.Add(this.taberrors);
			this.infotabs.Controls.Add(this.tabsearchresults);
			this.infotabs.Dock = System.Windows.Forms.DockStyle.Fill;
			this.infotabs.FlatBorderColor = System.Drawing.SystemColors.ControlDark;
			this.infotabs.ImageList = this.infoicons;
			this.infotabs.ItemSize = new System.Drawing.Size(74, 19);
			this.infotabs.Location = new System.Drawing.Point(0, 0);
			this.infotabs.Name = "infotabs";
			this.infotabs.SelectedIndex = 1;
			this.infotabs.SelectedTabColor = System.Drawing.SystemColors.ControlLightLight;
			this.infotabs.Size = new System.Drawing.Size(928, 187);
			this.infotabs.TabIndex = 0;
			this.infotabs.UseVisualStyles = false;
#else
            this.infotabs.Alignment = System.Windows.Forms.TabAlignment.Bottom;
            this.infotabs.Controls.Add(this.taberrors);
            this.infotabs.Controls.Add(this.tabsearchresults);
            this.infotabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.infotabs.ImageList = this.infoicons;
            this.infotabs.ItemSize = new System.Drawing.Size(74, 19);
            this.infotabs.Location = new System.Drawing.Point(0, 0);
            this.infotabs.Name = "infotabs";
            this.infotabs.SelectedIndex = 1;
            this.infotabs.Size = new System.Drawing.Size(928, 187);
            this.infotabs.TabIndex = 0;
#endif
			// 
			// taberrors
			// 
			this.taberrors.Controls.Add(this.errorlist);
			this.taberrors.ImageIndex = 0;
			this.taberrors.Location = new System.Drawing.Point(4, 4);
			this.taberrors.Name = "taberrors";
			this.taberrors.Padding = new System.Windows.Forms.Padding(3);
			this.taberrors.Size = new System.Drawing.Size(920, 160);
			this.taberrors.TabIndex = 0;
			this.taberrors.Text = "Errors";
			this.taberrors.UseVisualStyleBackColor = true;
			// 
			// errorlist
			// 
			this.errorlist.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colIndex,
            this.colDescription,
            this.colFile});
			this.errorlist.Dock = System.Windows.Forms.DockStyle.Fill;
			this.errorlist.FullRowSelect = true;
			this.errorlist.GridLines = true;
			this.errorlist.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.errorlist.HideSelection = false;
			this.errorlist.LabelWrap = false;
			this.errorlist.Location = new System.Drawing.Point(3, 3);
			this.errorlist.MultiSelect = false;
			this.errorlist.Name = "errorlist";
			this.errorlist.ShowGroups = false;
			this.errorlist.Size = new System.Drawing.Size(914, 154);
			this.errorlist.SmallImageList = this.errorimages;
			this.errorlist.TabIndex = 0;
			this.errorlist.TabStop = false;
			this.errorlist.UseCompatibleStateImageBehavior = false;
			this.errorlist.View = System.Windows.Forms.View.Details;
			// 
			// colIndex
			// 
			this.colIndex.Text = "";
			this.colIndex.Width = 45;
			// 
			// colDescription
			// 
			this.colDescription.Text = "Description";
			this.colDescription.Width = 500;
			// 
			// colFile
			// 
			this.colFile.Text = "File";
			this.colFile.Width = 150;
			// 
			// tabsearchresults
			// 
			this.tabsearchresults.ImageIndex = 1;
			this.tabsearchresults.Location = new System.Drawing.Point(4, 4);
			this.tabsearchresults.Name = "tabsearchresults";
			this.tabsearchresults.Padding = new System.Windows.Forms.Padding(3);
			this.tabsearchresults.Size = new System.Drawing.Size(920, 160);
			this.tabsearchresults.TabIndex = 1;
			this.tabsearchresults.Text = "Find results";
			this.tabsearchresults.UseVisualStyleBackColor = true;
			// 
			// ScriptEditorPanel
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.BackColor = System.Drawing.SystemColors.Control;
			this.Controls.Add(this.scriptsplitter);
			this.Controls.Add(this.statusbar);
			this.Controls.Add(this.toolbar);
			this.Controls.Add(this.menustrip);
			this.Name = "ScriptEditorPanel";
			this.Size = new System.Drawing.Size(928, 520);
			this.Load += new System.EventHandler(this.ScriptEditorPanel_Load);
			this.toolbar.ResumeLayout(false);
			this.toolbar.PerformLayout();
			this.statusbar.ResumeLayout(false);
			this.statusbar.PerformLayout();
			this.menustrip.ResumeLayout(false);
			this.menustrip.PerformLayout();
			this.scriptsplitter.Panel1.ResumeLayout(false);
			this.scriptsplitter.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.scriptsplitter)).EndInit();
			this.scriptsplitter.ResumeLayout(false);
			this.infotabs.ResumeLayout(false);
			this.taberrors.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

#endregion
		private System.Windows.Forms.ToolStrip toolbar;
		private System.Windows.Forms.OpenFileDialog openfile;
		private System.Windows.Forms.SaveFileDialog savefile;
		private System.Windows.Forms.ToolStripButton buttonsave;
		private System.Windows.Forms.ToolStripButton buttonsaveall;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripButton buttoncompile;
		private System.Windows.Forms.ToolStripButton buttonundo;
		private System.Windows.Forms.ToolStripButton buttonredo;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripButton buttoncut;
		private System.Windows.Forms.ToolStripButton buttoncopy;
		private System.Windows.Forms.ToolStripButton buttonpaste;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
		private System.Windows.Forms.ToolStripDropDownButton buttonscriptconfig;
		private System.Windows.Forms.ImageList errorimages;
		private System.Windows.Forms.ToolStripButton buttonkeywordhelp;
		private System.Windows.Forms.ToolStripButton buttonsearch;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
		private System.Windows.Forms.ToolStripDropDownButton buttonsnippets;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
		private System.Windows.Forms.ToolStripTextBox searchbox;
		private System.Windows.Forms.ToolStripButton searchnext;
		private System.Windows.Forms.ToolStripButton searchprev;
		private System.Windows.Forms.ToolStripButton searchmatchcase;
		private System.Windows.Forms.ToolStripButton searchwholeword;
		private System.Windows.Forms.StatusStrip statusbar;
		private System.Windows.Forms.ToolStripStatusLabel statuslabel;
		private System.Windows.Forms.ToolStripStatusLabel scripttype;
		private System.Windows.Forms.Timer statusflasher;
		private System.Windows.Forms.Timer statusresetter;
		private System.Windows.Forms.ToolStripStatusLabel positionlabel;
		private System.Windows.Forms.ToolStripButton buttonunindent;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
		private System.Windows.Forms.ToolStripButton buttonindent;
		private System.Windows.Forms.ToolStripButton buttonwhitespace;
		private System.Windows.Forms.ToolStripButton buttonwordwrap;
		private System.Windows.Forms.MenuStrip menustrip;
		private System.Windows.Forms.ToolStripMenuItem filemenuitem;
		private System.Windows.Forms.ToolStripMenuItem menusave;
		private System.Windows.Forms.ToolStripMenuItem menusaveall;
		private System.Windows.Forms.ToolStripMenuItem editmenuitem;
		private System.Windows.Forms.ToolStripMenuItem menuundo;
		private System.Windows.Forms.ToolStripMenuItem menuredo;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator10;
		private System.Windows.Forms.ToolStripMenuItem menucut;
		private System.Windows.Forms.ToolStripMenuItem menucopy;
		private System.Windows.Forms.ToolStripMenuItem menupaste;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator9;
		private System.Windows.Forms.ToolStripMenuItem menusnippets;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator12;
		private System.Windows.Forms.ToolStripMenuItem menuindent;
		private System.Windows.Forms.ToolStripMenuItem menuunindent;
		private System.Windows.Forms.ToolStripMenuItem viewmenuitem;
		private System.Windows.Forms.ToolStripMenuItem menuwhitespace;
		private System.Windows.Forms.ToolStripMenuItem menuwordwrap;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator11;
		private System.Windows.Forms.ToolStripMenuItem menustayontop;
		private System.Windows.Forms.ToolStripMenuItem searchmenuitem;
		private System.Windows.Forms.ToolStripMenuItem menufind;
		private System.Windows.Forms.ToolStripMenuItem menufindnext;
		private System.Windows.Forms.ToolStripMenuItem menufindprevious;
		private System.Windows.Forms.ToolStripMenuItem toolsmenu;
		private System.Windows.Forms.ToolStripMenuItem menucompile;
		private System.Windows.Forms.ImageList scripticons;
		private System.Windows.Forms.ImageList projecticons;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator14;
		private System.Windows.Forms.ToolStripMenuItem menugotoline;
		private System.Windows.Forms.ToolStripMenuItem menuduplicateline;
		private System.Windows.Forms.ImageList infoicons;
        private CollapsibleSplitContainer scriptsplitter;
        private VSTabControl tabs;
#if !MONO_WINFORMS
        private Dotnetrix.Controls.TabControlEX infotabs;
#else
        private System.Windows.Forms.TabControl infotabs;
#endif
        private System.Windows.Forms.TabPage taberrors;
        private System.Windows.Forms.ListView errorlist;
        private System.Windows.Forms.ColumnHeader colIndex;
        private System.Windows.Forms.ColumnHeader colDescription;
        private System.Windows.Forms.ColumnHeader colFile;
        private System.Windows.Forms.TabPage tabsearchresults;
    }
}
