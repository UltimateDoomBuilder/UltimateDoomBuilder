namespace CodeImp.DoomBuilder.Windows
{
	partial class ConfigForm
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
            System.Windows.Forms.Label label5;
            System.Windows.Forms.Label label6;
            System.Windows.Forms.Label label3;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigForm));
            System.Windows.Forms.Label label2;
            System.Windows.Forms.Label label7;
            System.Windows.Forms.Label label9;
            System.Windows.Forms.Label label1;
            System.Windows.Forms.Label label8;
            System.Windows.Forms.Label label4;
            System.Windows.Forms.Label label10;
            this.linuxpaths = new System.Windows.Forms.CheckBox();
            this.labelparameters = new System.Windows.Forms.Label();
            this.labelparameters.AutoSize = true;
            this.cancel = new System.Windows.Forms.Button();
            this.apply = new System.Windows.Forms.Button();
            this.tabs = new System.Windows.Forms.TabControl();
            this.tabresources = new System.Windows.Forms.TabPage();
            this.configdata = new CodeImp.DoomBuilder.Controls.ResourceListEditor();
            this.tabnodebuilder = new System.Windows.Forms.TabPage();
            this.nodebuildertest = new System.Windows.Forms.ComboBox();
            this.nodebuildersave = new System.Windows.Forms.ComboBox();
            this.tabtesting = new System.Windows.Forms.TabPage();
            this.btnRemoveEngine = new System.Windows.Forms.Button();
            this.btnNewEngine = new System.Windows.Forms.Button();
            this.cbEngineSelector = new System.Windows.Forms.ComboBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label13.AutoSize = true;
            this.shortpaths = new System.Windows.Forms.CheckBox();
            this.customparameters = new System.Windows.Forms.CheckBox();
            this.skill = new CodeImp.DoomBuilder.Controls.ActionSelectorControl();
            this.browsetestprogram = new System.Windows.Forms.Button();
            this.noresultlabel = new System.Windows.Forms.Label();
            this.noresultlabel.AutoSize = true;
            this.testresult = new System.Windows.Forms.TextBox();
            this.labelresult = new System.Windows.Forms.Label();
            this.labelresult.AutoSize = true;
            this.testparameters = new System.Windows.Forms.TextBox();
            this.testapplication = new System.Windows.Forms.TextBox();
            this.tabtextures = new System.Windows.Forms.TabPage();
            this.exporttexturesets = new System.Windows.Forms.Button();
            this.importtexturesets = new System.Windows.Forms.Button();
            this.listtextures = new System.Windows.Forms.ListView();
            this.smallimages = new System.Windows.Forms.ImageList(this.components);
            this.restoretexturesets = new System.Windows.Forms.Button();
            this.edittextureset = new System.Windows.Forms.Button();
            this.pastetexturesets = new System.Windows.Forms.Button();
            this.copytexturesets = new System.Windows.Forms.Button();
            this.removetextureset = new System.Windows.Forms.Button();
            this.addtextureset = new System.Windows.Forms.Button();
            this.tabmodes = new System.Windows.Forms.TabPage();
            this.startmode = new System.Windows.Forms.ComboBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label11.AutoSize = true;
            this.listmodes = new System.Windows.Forms.ListView();
            this.colmodename = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colmodeplugin = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.listconfigs = new System.Windows.Forms.ListView();
            this.columnname = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.copypastemenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.copyall = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.pasteall = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteresources = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteengines = new System.Windows.Forms.ToolStripMenuItem();
            this.pastecolorpresets = new System.Windows.Forms.ToolStripMenuItem();
            this.testprogramdialog = new System.Windows.Forms.OpenFileDialog();
            this.hintlabel = new System.Windows.Forms.Label();
            this.hintlabel.AutoSize = true;
            this.hint = new System.Windows.Forms.PictureBox();
            this.tooltip = new System.Windows.Forms.ToolTip(this.components);
            this.importtexturesetdialog = new System.Windows.Forms.OpenFileDialog();
            this.exporttexturesetdialog = new System.Windows.Forms.SaveFileDialog();
            label5 = new System.Windows.Forms.Label();
            label6 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            label7 = new System.Windows.Forms.Label();
            label9 = new System.Windows.Forms.Label();
            label1 = new System.Windows.Forms.Label();
            label8 = new System.Windows.Forms.Label();
            label4 = new System.Windows.Forms.Label();
            label10 = new System.Windows.Forms.Label();
            this.tabs.SuspendLayout();
            this.tabresources.SuspendLayout();
            this.tabnodebuilder.SuspendLayout();
            this.tabtesting.SuspendLayout();
            this.tabtextures.SuspendLayout();
            this.tabmodes.SuspendLayout();
            this.copypastemenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.hint)).BeginInit();
            this.SuspendLayout();
            // 
            // label5
            // 
            label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(12, 272);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(299, 39);
            label5.TabIndex = 19;
            label5.Text = "Drag && drop resources to add them.\r\nDrag items to change order (lower items over" +
    "ride higher items).\r\nUse the context menu to cut, copy, paste or remove items.";
            // 
            // label6
            // 
            label6.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            label6.AutoEllipsis = true;
            label6.Location = new System.Drawing.Point(12, 15);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(457, 37);
            label6.TabIndex = 21;
            label6.Text = "These are the resources that will be loaded when this configuration is chosen for" +
    " editing. Usually you add your IWAD (like DOOM.WAD or DOOM2.WAD) here.";
            // 
            // label3
            // 
            label3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            label3.AutoEllipsis = true;
            label3.Location = new System.Drawing.Point(12, 15);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(468, 54);
            label3.TabIndex = 22;
            label3.Text = resources.GetString("label3.Text");
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(12, 86);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(144, 13);
            label2.TabIndex = 24;
            label2.Text = "Configuration for saving map:";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new System.Drawing.Point(35, 125);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(121, 13);
            label7.TabIndex = 26;
            label7.Text = "Configuration for testing:";
            // 
            // label9
            // 
            label9.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            label9.AutoEllipsis = true;
            label9.Location = new System.Drawing.Point(12, 15);
            label9.Name = "label9";
            label9.Size = new System.Drawing.Size(477, 54);
            label9.TabIndex = 23;
            label9.Text = "Here you can specify the program settings to use for launching a game engine when" +
    " testing the map. Press F1 for help with custom parameters.";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(15, 89);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(62, 13);
            label1.TabIndex = 24;
            label1.Text = "Application:";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new System.Drawing.Point(21, 119);
            label8.Name = "label8";
            label8.Size = new System.Drawing.Size(58, 13);
            label8.TabIndex = 34;
            label8.Text = "Skill Level:";
            // 
            // label4
            // 
            label4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            label4.AutoEllipsis = true;
            label4.Location = new System.Drawing.Point(12, 15);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(483, 46);
            label4.TabIndex = 24;
            label4.Text = "Texture Sets are a way to group textures and flats into categories, so that you c" +
    "an easily find a texture for the specific style or purpose you need by selecting" +
    " one of the categories.";
            // 
            // label10
            // 
            label10.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            label10.AutoEllipsis = true;
            label10.Location = new System.Drawing.Point(12, 15);
            label10.Name = "label10";
            label10.Size = new System.Drawing.Size(470, 58);
            label10.TabIndex = 25;
            label10.Text = resources.GetString("label10.Text");
            // 
            // linuxpaths
            // 
            this.linuxpaths.AutoSize = true;
            this.linuxpaths.Location = new System.Drawing.Point(375, 217);
            this.linuxpaths.Name = "linuxpaths";
            this.linuxpaths.Size = new System.Drawing.Size(102, 17);
            this.linuxpaths.TabIndex = 41;
            this.linuxpaths.Text = "Use Linux paths";
            this.linuxpaths.UseVisualStyleBackColor = true;
            this.linuxpaths.Visible = false;
            this.linuxpaths.CheckedChanged += new System.EventHandler(this.Linuxpaths_CheckedChanged);
            // 
            // labelparameters
            // 
            this.labelparameters.AutoSize = true;
            this.labelparameters.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelparameters.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.labelparameters.Location = new System.Drawing.Point(16, 169);
            this.labelparameters.Name = "labelparameters";
            this.labelparameters.Size = new System.Drawing.Size(63, 13);
            this.labelparameters.TabIndex = 27;
            this.labelparameters.Text = "Parameters:";
            this.tooltip.SetToolTip(this.labelparameters, resources.GetString("labelparameters.ToolTip"));
            this.labelparameters.Visible = false;
            // 
            // cancel
            // 
            this.cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancel.Location = new System.Drawing.Point(671, 379);
            this.cancel.Name = "cancel";
            this.cancel.Size = new System.Drawing.Size(112, 25);
            this.cancel.TabIndex = 3;
            this.cancel.Text = "Cancel";
            this.cancel.UseVisualStyleBackColor = true;
            this.cancel.Click += new System.EventHandler(this.cancel_Click);
            // 
            // apply
            // 
            this.apply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.apply.Location = new System.Drawing.Point(553, 379);
            this.apply.Name = "apply";
            this.apply.Size = new System.Drawing.Size(112, 25);
            this.apply.TabIndex = 2;
            this.apply.Text = "OK";
            this.apply.UseVisualStyleBackColor = true;
            this.apply.Click += new System.EventHandler(this.apply_Click);
            // 
            // tabs
            // 
            this.tabs.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabs.Controls.Add(this.tabresources);
            this.tabs.Controls.Add(this.tabnodebuilder);
            this.tabs.Controls.Add(this.tabtesting);
            this.tabs.Controls.Add(this.tabtextures);
            this.tabs.Controls.Add(this.tabmodes);
            this.tabs.Enabled = false;
            this.tabs.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabs.Location = new System.Drawing.Point(277, 12);
            this.tabs.Name = "tabs";
            this.tabs.Padding = new System.Drawing.Point(24, 3);
            this.tabs.SelectedIndex = 0;
            this.tabs.Size = new System.Drawing.Size(506, 358);
            this.tabs.TabIndex = 1;
            // 
            // tabresources
            // 
            this.tabresources.Controls.Add(label6);
            this.tabresources.Controls.Add(this.configdata);
            this.tabresources.Controls.Add(label5);
            this.tabresources.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabresources.Location = new System.Drawing.Point(4, 22);
            this.tabresources.Name = "tabresources";
            this.tabresources.Padding = new System.Windows.Forms.Padding(6);
            this.tabresources.Size = new System.Drawing.Size(498, 332);
            this.tabresources.TabIndex = 0;
            this.tabresources.Text = "Resources";
            this.tabresources.UseVisualStyleBackColor = true;
            // 
            // configdata
            // 
            this.configdata.AllowDrop = true;
            this.configdata.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.configdata.DialogOffset = new System.Drawing.Point(-120, 10);
            this.configdata.GameConfiguration = null;
            this.configdata.Location = new System.Drawing.Point(15, 55);
            this.configdata.Name = "configdata";
            this.configdata.Size = new System.Drawing.Size(467, 204);
            this.configdata.TabIndex = 0;
            this.configdata.OnContentChanged += new CodeImp.DoomBuilder.Controls.ResourceListEditor.ContentChanged(this.resourcelocations_OnContentChanged);
            this.configdata.OnWarningsChanged += new CodeImp.DoomBuilder.Controls.ResourceListEditor.WarningsChanged(this.configdata_OnWarningsChanged);
            // 
            // tabnodebuilder
            // 
            this.tabnodebuilder.Controls.Add(label7);
            this.tabnodebuilder.Controls.Add(this.nodebuildertest);
            this.tabnodebuilder.Controls.Add(label2);
            this.tabnodebuilder.Controls.Add(this.nodebuildersave);
            this.tabnodebuilder.Controls.Add(label3);
            this.tabnodebuilder.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabnodebuilder.Location = new System.Drawing.Point(4, 22);
            this.tabnodebuilder.Name = "tabnodebuilder";
            this.tabnodebuilder.Padding = new System.Windows.Forms.Padding(6);
            this.tabnodebuilder.Size = new System.Drawing.Size(498, 332);
            this.tabnodebuilder.TabIndex = 1;
            this.tabnodebuilder.Text = "Nodebuilder";
            this.tabnodebuilder.UseVisualStyleBackColor = true;
            // 
            // nodebuildertest
            // 
            this.nodebuildertest.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.nodebuildertest.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.nodebuildertest.FormattingEnabled = true;
            this.nodebuildertest.Location = new System.Drawing.Point(167, 122);
            this.nodebuildertest.Name = "nodebuildertest";
            this.nodebuildertest.Size = new System.Drawing.Size(313, 21);
            this.nodebuildertest.Sorted = true;
            this.nodebuildertest.TabIndex = 1;
            this.nodebuildertest.SelectedIndexChanged += new System.EventHandler(this.nodebuildertest_SelectedIndexChanged);
            // 
            // nodebuildersave
            // 
            this.nodebuildersave.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.nodebuildersave.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.nodebuildersave.FormattingEnabled = true;
            this.nodebuildersave.Location = new System.Drawing.Point(167, 83);
            this.nodebuildersave.Name = "nodebuildersave";
            this.nodebuildersave.Size = new System.Drawing.Size(313, 21);
            this.nodebuildersave.Sorted = true;
            this.nodebuildersave.TabIndex = 0;
            this.nodebuildersave.SelectedIndexChanged += new System.EventHandler(this.nodebuildersave_SelectedIndexChanged);
            // 
            // tabtesting
            // 
            this.tabtesting.Controls.Add(this.linuxpaths);
            this.tabtesting.Controls.Add(this.btnRemoveEngine);
            this.tabtesting.Controls.Add(this.btnNewEngine);
            this.tabtesting.Controls.Add(this.cbEngineSelector);
            this.tabtesting.Controls.Add(this.label13);
            this.tabtesting.Controls.Add(this.shortpaths);
            this.tabtesting.Controls.Add(this.customparameters);
            this.tabtesting.Controls.Add(this.skill);
            this.tabtesting.Controls.Add(label8);
            this.tabtesting.Controls.Add(this.browsetestprogram);
            this.tabtesting.Controls.Add(this.noresultlabel);
            this.tabtesting.Controls.Add(this.testresult);
            this.tabtesting.Controls.Add(this.labelresult);
            this.tabtesting.Controls.Add(this.testparameters);
            this.tabtesting.Controls.Add(this.labelparameters);
            this.tabtesting.Controls.Add(this.testapplication);
            this.tabtesting.Controls.Add(label1);
            this.tabtesting.Controls.Add(label9);
            this.tabtesting.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabtesting.Location = new System.Drawing.Point(4, 22);
            this.tabtesting.Name = "tabtesting";
            this.tabtesting.Padding = new System.Windows.Forms.Padding(6);
            this.tabtesting.Size = new System.Drawing.Size(498, 332);
            this.tabtesting.TabIndex = 2;
            this.tabtesting.Text = "Testing";
            this.tabtesting.UseVisualStyleBackColor = true;
            // 
            // btnRemoveEngine
            // 
            this.btnRemoveEngine.Image = global::CodeImp.DoomBuilder.Properties.Resources.SearchClear;
            this.btnRemoveEngine.Location = new System.Drawing.Point(463, 51);
            this.btnRemoveEngine.Name = "btnRemoveEngine";
            this.btnRemoveEngine.Size = new System.Drawing.Size(26, 24);
            this.btnRemoveEngine.TabIndex = 40;
            this.tooltip.SetToolTip(this.btnRemoveEngine, "Remove currently selected game engine");
            this.btnRemoveEngine.UseVisualStyleBackColor = true;
            this.btnRemoveEngine.Click += new System.EventHandler(this.btnRemoveEngine_Click);
            // 
            // btnNewEngine
            // 
            this.btnNewEngine.Image = global::CodeImp.DoomBuilder.Properties.Resources.Add;
            this.btnNewEngine.Location = new System.Drawing.Point(433, 51);
            this.btnNewEngine.Name = "btnNewEngine";
            this.btnNewEngine.Size = new System.Drawing.Size(26, 24);
            this.btnNewEngine.TabIndex = 39;
            this.tooltip.SetToolTip(this.btnNewEngine, "Add new game engine");
            this.btnNewEngine.UseVisualStyleBackColor = true;
            this.btnNewEngine.Click += new System.EventHandler(this.btnNewEngine_Click);
            // 
            // cbEngineSelector
            // 
            this.cbEngineSelector.FormattingEnabled = true;
            this.cbEngineSelector.Location = new System.Drawing.Point(87, 53);
            this.cbEngineSelector.Name = "cbEngineSelector";
            this.cbEngineSelector.Size = new System.Drawing.Size(340, 21);
            this.cbEngineSelector.TabIndex = 38;
            this.cbEngineSelector.DropDown += new System.EventHandler(this.cbEngineSelector_DropDown);
            this.cbEngineSelector.SelectedIndexChanged += new System.EventHandler(this.cbEngineSelector_SelectedIndexChanged);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(36, 56);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(43, 13);
            this.label13.TabIndex = 37;
            this.label13.Text = "Engine:";
            // 
            // shortpaths
            // 
            this.shortpaths.AutoSize = true;
            this.shortpaths.Location = new System.Drawing.Point(87, 217);
            this.shortpaths.Name = "shortpaths";
            this.shortpaths.Size = new System.Drawing.Size(269, 17);
            this.shortpaths.TabIndex = 5;
            this.shortpaths.Text = "Use short paths and file names (MSDOS 8.3 format)";
            this.shortpaths.UseVisualStyleBackColor = true;
            this.shortpaths.Visible = false;
            this.shortpaths.CheckedChanged += new System.EventHandler(this.shortpaths_CheckedChanged);
            // 
            // customparameters
            // 
            this.customparameters.AutoSize = true;
            this.customparameters.Location = new System.Drawing.Point(87, 146);
            this.customparameters.Name = "customparameters";
            this.customparameters.Size = new System.Drawing.Size(129, 17);
            this.customparameters.TabIndex = 3;
            this.customparameters.Text = "Customize parameters";
            this.customparameters.UseVisualStyleBackColor = true;
            this.customparameters.CheckedChanged += new System.EventHandler(this.customparameters_CheckedChanged);
            // 
            // skill
            // 
            this.skill.BackColor = System.Drawing.Color.Transparent;
            this.skill.Cursor = System.Windows.Forms.Cursors.Default;
            this.skill.Empty = false;
            this.skill.GeneralizedCategories = null;
            this.skill.GeneralizedOptions = null;
            this.skill.Location = new System.Drawing.Point(87, 116);
            this.skill.Name = "skill";
            this.skill.Size = new System.Drawing.Size(402, 21);
            this.skill.TabIndex = 2;
            this.skill.Value = 402;
            this.skill.ValueChanges += new System.EventHandler(this.skill_ValueChanges);
            // 
            // browsetestprogram
            // 
            this.browsetestprogram.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.browsetestprogram.Image = global::CodeImp.DoomBuilder.Properties.Resources.Folder;
            this.browsetestprogram.Location = new System.Drawing.Point(459, 84);
            this.browsetestprogram.Name = "browsetestprogram";
            this.browsetestprogram.Size = new System.Drawing.Size(30, 24);
            this.browsetestprogram.TabIndex = 1;
            this.browsetestprogram.Text = " ";
            this.tooltip.SetToolTip(this.browsetestprogram, "Browse game engine");
            this.browsetestprogram.UseVisualStyleBackColor = true;
            this.browsetestprogram.Click += new System.EventHandler(this.browsetestprogram_Click);
            // 
            // noresultlabel
            // 
            this.noresultlabel.Location = new System.Drawing.Point(84, 244);
            this.noresultlabel.Name = "noresultlabel";
            this.noresultlabel.Size = new System.Drawing.Size(272, 43);
            this.noresultlabel.TabIndex = 32;
            this.noresultlabel.Text = "An example result cannot be displayed, because it requires a map to be loaded.";
            this.noresultlabel.Visible = false;
            // 
            // testresult
            // 
            this.testresult.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.testresult.BackColor = System.Drawing.SystemColors.Control;
            this.testresult.Location = new System.Drawing.Point(86, 241);
            this.testresult.Multiline = true;
            this.testresult.Name = "testresult";
            this.testresult.ReadOnly = true;
            this.testresult.Size = new System.Drawing.Size(403, 79);
            this.testresult.TabIndex = 6;
            this.testresult.Visible = false;
            // 
            // labelresult
            // 
            this.labelresult.AutoSize = true;
            this.labelresult.Location = new System.Drawing.Point(38, 244);
            this.labelresult.Name = "labelresult";
            this.labelresult.Size = new System.Drawing.Size(40, 13);
            this.labelresult.TabIndex = 30;
            this.labelresult.Text = "Result:";
            this.labelresult.Visible = false;
            // 
            // testparameters
            // 
            this.testparameters.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.testparameters.Location = new System.Drawing.Point(87, 166);
            this.testparameters.Multiline = true;
            this.testparameters.Name = "testparameters";
            this.testparameters.Size = new System.Drawing.Size(402, 41);
            this.testparameters.TabIndex = 4;
            this.testparameters.Visible = false;
            this.testparameters.TextChanged += new System.EventHandler(this.testparameters_TextChanged);
            // 
            // testapplication
            // 
            this.testapplication.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.testapplication.Location = new System.Drawing.Point(87, 86);
            this.testapplication.Name = "testapplication";
            this.testapplication.ReadOnly = true;
            this.testapplication.Size = new System.Drawing.Size(366, 20);
            this.testapplication.TabIndex = 0;
            this.testapplication.TextChanged += new System.EventHandler(this.testapplication_TextChanged);
            // 
            // tabtextures
            // 
            this.tabtextures.Controls.Add(this.exporttexturesets);
            this.tabtextures.Controls.Add(this.importtexturesets);
            this.tabtextures.Controls.Add(this.listtextures);
            this.tabtextures.Controls.Add(this.restoretexturesets);
            this.tabtextures.Controls.Add(this.edittextureset);
            this.tabtextures.Controls.Add(this.pastetexturesets);
            this.tabtextures.Controls.Add(this.copytexturesets);
            this.tabtextures.Controls.Add(this.removetextureset);
            this.tabtextures.Controls.Add(this.addtextureset);
            this.tabtextures.Controls.Add(label4);
            this.tabtextures.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabtextures.Location = new System.Drawing.Point(4, 22);
            this.tabtextures.Name = "tabtextures";
            this.tabtextures.Size = new System.Drawing.Size(498, 332);
            this.tabtextures.TabIndex = 3;
            this.tabtextures.Text = "Textures";
            this.tabtextures.UseVisualStyleBackColor = true;
            // 
            // exporttexturesets
            // 
            this.exporttexturesets.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.exporttexturesets.Enabled = false;
            this.exporttexturesets.Location = new System.Drawing.Point(414, 282);
            this.exporttexturesets.Name = "exporttexturesets";
            this.exporttexturesets.Size = new System.Drawing.Size(68, 24);
            this.exporttexturesets.TabIndex = 25;
            this.exporttexturesets.Text = "Export";
            this.exporttexturesets.UseVisualStyleBackColor = true;
            this.exporttexturesets.Click += new System.EventHandler(this.exporttexturesets_Click);
            // 
            // importtexturesets
            // 
            this.importtexturesets.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.importtexturesets.Location = new System.Drawing.Point(340, 282);
            this.importtexturesets.Name = "importtexturesets";
            this.importtexturesets.Size = new System.Drawing.Size(68, 24);
            this.importtexturesets.TabIndex = 25;
            this.importtexturesets.Text = "Import";
            this.importtexturesets.UseVisualStyleBackColor = true;
            this.importtexturesets.Click += new System.EventHandler(this.importtexturesets_Click);
            // 
            // listtextures
            // 
            this.listtextures.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listtextures.FullRowSelect = true;
            this.listtextures.HideSelection = false;
            this.listtextures.Location = new System.Drawing.Point(15, 64);
            this.listtextures.Name = "listtextures";
            this.listtextures.ShowGroups = false;
            this.listtextures.Size = new System.Drawing.Size(467, 174);
            this.listtextures.SmallImageList = this.smallimages;
            this.listtextures.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.listtextures.TabIndex = 0;
            this.listtextures.UseCompatibleStateImageBehavior = false;
            this.listtextures.View = System.Windows.Forms.View.List;
            this.listtextures.SelectedIndexChanged += new System.EventHandler(this.listtextures_SelectedIndexChanged);
            this.listtextures.DoubleClick += new System.EventHandler(this.listtextures_DoubleClick);
            // 
            // smallimages
            // 
            this.smallimages.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("smallimages.ImageStream")));
            this.smallimages.TransparentColor = System.Drawing.Color.Transparent;
            this.smallimages.Images.SetKeyName(0, "KnownTextureSet.ico");
            // 
            // restoretexturesets
            // 
            this.restoretexturesets.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.restoretexturesets.Location = new System.Drawing.Point(15, 282);
            this.restoretexturesets.Name = "restoretexturesets";
            this.restoretexturesets.Size = new System.Drawing.Size(140, 24);
            this.restoretexturesets.TabIndex = 6;
            this.restoretexturesets.Text = "Add Default Sets";
            this.restoretexturesets.UseVisualStyleBackColor = true;
            this.restoretexturesets.Click += new System.EventHandler(this.restoretexturesets_Click);
            // 
            // edittextureset
            // 
            this.edittextureset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.edittextureset.Enabled = false;
            this.edittextureset.Location = new System.Drawing.Point(88, 244);
            this.edittextureset.Name = "edittextureset";
            this.edittextureset.Size = new System.Drawing.Size(67, 24);
            this.edittextureset.TabIndex = 2;
            this.edittextureset.Text = "Edit...";
            this.edittextureset.UseVisualStyleBackColor = true;
            this.edittextureset.Click += new System.EventHandler(this.edittextureset_Click);
            // 
            // pastetexturesets
            // 
            this.pastetexturesets.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.pastetexturesets.Enabled = false;
            this.pastetexturesets.Location = new System.Drawing.Point(414, 244);
            this.pastetexturesets.Name = "pastetexturesets";
            this.pastetexturesets.Size = new System.Drawing.Size(68, 24);
            this.pastetexturesets.TabIndex = 5;
            this.pastetexturesets.Text = "Paste";
            this.pastetexturesets.UseVisualStyleBackColor = true;
            this.pastetexturesets.Click += new System.EventHandler(this.pastetexturesets_Click);
            // 
            // copytexturesets
            // 
            this.copytexturesets.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.copytexturesets.Enabled = false;
            this.copytexturesets.Location = new System.Drawing.Point(340, 244);
            this.copytexturesets.Name = "copytexturesets";
            this.copytexturesets.Size = new System.Drawing.Size(68, 24);
            this.copytexturesets.TabIndex = 4;
            this.copytexturesets.Text = "Copy";
            this.copytexturesets.UseVisualStyleBackColor = true;
            this.copytexturesets.Click += new System.EventHandler(this.copytexturesets_Click);
            // 
            // removetextureset
            // 
            this.removetextureset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.removetextureset.Enabled = false;
            this.removetextureset.Location = new System.Drawing.Point(161, 244);
            this.removetextureset.Name = "removetextureset";
            this.removetextureset.Size = new System.Drawing.Size(68, 24);
            this.removetextureset.TabIndex = 3;
            this.removetextureset.Text = "Remove";
            this.removetextureset.UseVisualStyleBackColor = true;
            this.removetextureset.Click += new System.EventHandler(this.removetextureset_Click);
            // 
            // addtextureset
            // 
            this.addtextureset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.addtextureset.Location = new System.Drawing.Point(15, 244);
            this.addtextureset.Name = "addtextureset";
            this.addtextureset.Size = new System.Drawing.Size(67, 24);
            this.addtextureset.TabIndex = 1;
            this.addtextureset.Text = "Add...";
            this.addtextureset.UseVisualStyleBackColor = true;
            this.addtextureset.Click += new System.EventHandler(this.addtextureset_Click);
            // 
            // tabmodes
            // 
            this.tabmodes.Controls.Add(this.startmode);
            this.tabmodes.Controls.Add(this.label11);
            this.tabmodes.Controls.Add(this.listmodes);
            this.tabmodes.Controls.Add(label10);
            this.tabmodes.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabmodes.Location = new System.Drawing.Point(4, 22);
            this.tabmodes.Name = "tabmodes";
            this.tabmodes.Size = new System.Drawing.Size(498, 332);
            this.tabmodes.TabIndex = 4;
            this.tabmodes.Text = "Modes";
            this.tabmodes.UseVisualStyleBackColor = true;
            // 
            // startmode
            // 
            this.startmode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.startmode.FormattingEnabled = true;
            this.startmode.Location = new System.Drawing.Point(215, 288);
            this.startmode.Name = "startmode";
            this.startmode.Size = new System.Drawing.Size(267, 21);
            this.startmode.TabIndex = 27;
            this.startmode.SelectedIndexChanged += new System.EventHandler(this.startmode_SelectedIndexChanged);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(12, 291);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(197, 13);
            this.label11.TabIndex = 26;
            this.label11.Text = "When opening a map, start in this mode:";
            // 
            // listmodes
            // 
            this.listmodes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listmodes.CheckBoxes = true;
            this.listmodes.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colmodename,
            this.colmodeplugin});
            this.listmodes.FullRowSelect = true;
            this.listmodes.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listmodes.HideSelection = false;
            this.listmodes.Location = new System.Drawing.Point(15, 70);
            this.listmodes.MultiSelect = false;
            this.listmodes.Name = "listmodes";
            this.listmodes.ShowGroups = false;
            this.listmodes.Size = new System.Drawing.Size(467, 201);
            this.listmodes.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.listmodes.TabIndex = 0;
            this.listmodes.UseCompatibleStateImageBehavior = false;
            this.listmodes.View = System.Windows.Forms.View.Details;
            this.listmodes.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.listmodes_ItemChecked);
            // 
            // colmodename
            // 
            this.colmodename.Text = "Editing Mode";
            this.colmodename.Width = 179;
            // 
            // colmodeplugin
            // 
            this.colmodeplugin.Text = "Plugin";
            this.colmodeplugin.Width = 221;
            // 
            // listconfigs
            // 
            this.listconfigs.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.listconfigs.CheckBoxes = true;
            this.listconfigs.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnname});
            this.listconfigs.ContextMenuStrip = this.copypastemenu;
            this.listconfigs.FullRowSelect = true;
            this.listconfigs.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listconfigs.HideSelection = false;
            this.listconfigs.Location = new System.Drawing.Point(12, 12);
            this.listconfigs.MultiSelect = false;
            this.listconfigs.Name = "listconfigs";
            this.listconfigs.ShowGroups = false;
            this.listconfigs.Size = new System.Drawing.Size(259, 358);
            this.listconfigs.TabIndex = 0;
            this.listconfigs.UseCompatibleStateImageBehavior = false;
            this.listconfigs.View = System.Windows.Forms.View.Details;
            this.listconfigs.KeyUp += new System.Windows.Forms.KeyEventHandler(this.listconfigs_KeyUp);
            this.listconfigs.MouseUp += new System.Windows.Forms.MouseEventHandler(this.listconfigs_MouseUp);
            // 
            // columnname
            // 
            this.columnname.Text = "Configuration";
            this.columnname.Width = 200;
            // 
            // copypastemenu
            // 
            this.copypastemenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyall,
            this.toolStripSeparator1,
            this.pasteall,
            this.pasteresources,
            this.pasteengines,
            this.pastecolorpresets});
            this.copypastemenu.Name = "copypastemenu";
            this.copypastemenu.Size = new System.Drawing.Size(175, 120);
            this.copypastemenu.Opening += new System.ComponentModel.CancelEventHandler(this.copypastemenu_Opening);
            // 
            // copyall
            // 
            this.copyall.Image = global::CodeImp.DoomBuilder.Properties.Resources.Copy;
            this.copyall.Name = "copyall";
            this.copyall.Size = new System.Drawing.Size(174, 22);
            this.copyall.Text = "Copy";
            this.copyall.Click += new System.EventHandler(this.copyall_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(171, 6);
            // 
            // pasteall
            // 
            this.pasteall.Image = global::CodeImp.DoomBuilder.Properties.Resources.Paste;
            this.pasteall.Name = "pasteall";
            this.pasteall.Size = new System.Drawing.Size(174, 22);
            this.pasteall.Text = "Paste";
            this.pasteall.Click += new System.EventHandler(this.pasteall_Click);
            // 
            // pasteresources
            // 
            this.pasteresources.Image = global::CodeImp.DoomBuilder.Properties.Resources.PasteSpecial;
            this.pasteresources.Name = "pasteresources";
            this.pasteresources.Size = new System.Drawing.Size(174, 22);
            this.pasteresources.Text = "Paste Resources";
            this.pasteresources.Click += new System.EventHandler(this.pasteresources_Click);
            // 
            // pasteengines
            // 
            this.pasteengines.Image = global::CodeImp.DoomBuilder.Properties.Resources.PasteSpecial;
            this.pasteengines.Name = "pasteengines";
            this.pasteengines.Size = new System.Drawing.Size(174, 22);
            this.pasteengines.Text = "Paste Test Engines";
            this.pasteengines.Click += new System.EventHandler(this.pasteengines_Click);
            // 
            // pastecolorpresets
            // 
            this.pastecolorpresets.Image = global::CodeImp.DoomBuilder.Properties.Resources.PasteSpecial;
            this.pastecolorpresets.Name = "pastecolorpresets";
            this.pastecolorpresets.Size = new System.Drawing.Size(174, 22);
            this.pastecolorpresets.Text = "Paste Color Presets";
            this.pastecolorpresets.Click += new System.EventHandler(this.pastecolorpresets_Click);
            // 
            // testprogramdialog
            // 
            this.testprogramdialog.Filter = "Executable Files (*.exe)|*.exe|Batch Files (*.bat)|*.bat";
            this.testprogramdialog.Title = "Browse Test Program";
            // 
            // hintlabel
            // 
            this.hintlabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.hintlabel.AutoSize = true;
            this.hintlabel.Location = new System.Drawing.Point(30, 384);
            this.hintlabel.Name = "hintlabel";
            this.hintlabel.Size = new System.Drawing.Size(276, 13);
            this.hintlabel.TabIndex = 6;
            this.hintlabel.Text = "Use the context menu to copy-paste game configurations";
            // 
            // hint
            // 
            this.hint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.hint.Image = global::CodeImp.DoomBuilder.Properties.Resources.Lightbulb;
            this.hint.Location = new System.Drawing.Point(12, 383);
            this.hint.Name = "hint";
            this.hint.Size = new System.Drawing.Size(16, 16);
            this.hint.TabIndex = 5;
            this.hint.TabStop = false;
            // 
            // tooltip
            // 
            this.tooltip.AutomaticDelay = 0;
            this.tooltip.AutoPopDelay = 30000;
            this.tooltip.InitialDelay = 10;
            this.tooltip.ReshowDelay = 100;
            this.tooltip.ToolTipTitle = "Supported Placeholders:";
            this.tooltip.UseAnimation = false;
            this.tooltip.UseFading = false;
            // 
            // importtexturesetdialog
            // 
            this.importtexturesetdialog.Filter = "Texture Set Configuration File|*.cfg|All files|*.*";
            this.importtexturesetdialog.RestoreDirectory = true;
            // 
            // exporttexturesetdialog
            // 
            this.exporttexturesetdialog.DefaultExt = "cfg";
            this.exporttexturesetdialog.FileName = "ExportedTextureSets.cfg";
            this.exporttexturesetdialog.Filter = "Texture Set Configuration File|*.cfg|All files|*.*";
            this.exporttexturesetdialog.RestoreDirectory = true;
            // 
            // ConfigForm
            // 
            this.AcceptButton = this.apply;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.cancel;
            this.ClientSize = new System.Drawing.Size(794, 416);
            this.Controls.Add(this.hintlabel);
            this.Controls.Add(this.hint);
            this.Controls.Add(this.listconfigs);
            this.Controls.Add(this.tabs);
            this.Controls.Add(this.cancel);
            this.Controls.Add(this.apply);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ConfigForm";
            this.Opacity = 0D;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Game Configurations";
            this.Shown += new System.EventHandler(this.ConfigForm_Shown);
            this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.ConfigForm_HelpRequested);
            this.tabs.ResumeLayout(false);
            this.tabresources.ResumeLayout(false);
            this.tabresources.PerformLayout();
            this.tabnodebuilder.ResumeLayout(false);
            this.tabnodebuilder.PerformLayout();
            this.tabtesting.ResumeLayout(false);
            this.tabtesting.PerformLayout();
            this.tabtextures.ResumeLayout(false);
            this.tabmodes.ResumeLayout(false);
            this.tabmodes.PerformLayout();
            this.copypastemenu.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.hint)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button cancel;
		private System.Windows.Forms.Button apply;
		private System.Windows.Forms.TabControl tabs;
		private System.Windows.Forms.TabPage tabresources;
		private System.Windows.Forms.TabPage tabnodebuilder;
		private System.Windows.Forms.TabPage tabtesting;
		private CodeImp.DoomBuilder.Controls.ResourceListEditor configdata;
		private System.Windows.Forms.ComboBox nodebuildertest;
		private System.Windows.Forms.ComboBox nodebuildersave;
		private System.Windows.Forms.TextBox testparameters;
		private System.Windows.Forms.TextBox testapplication;
		private System.Windows.Forms.TextBox testresult;
		private System.Windows.Forms.Label labelresult;
		private System.Windows.Forms.ListView listconfigs;
		private System.Windows.Forms.ColumnHeader columnname;
		private System.Windows.Forms.Label noresultlabel;
		private System.Windows.Forms.Button browsetestprogram;
		private System.Windows.Forms.OpenFileDialog testprogramdialog;
		private System.Windows.Forms.CheckBox customparameters;
		private CodeImp.DoomBuilder.Controls.ActionSelectorControl skill;
		private System.Windows.Forms.Label labelparameters;
		private System.Windows.Forms.TabPage tabtextures;
		private System.Windows.Forms.Button addtextureset;
		private System.Windows.Forms.Button restoretexturesets;
		private System.Windows.Forms.Button edittextureset;
		private System.Windows.Forms.Button pastetexturesets;
		private System.Windows.Forms.Button copytexturesets;
		private System.Windows.Forms.Button removetextureset;
		private System.Windows.Forms.ListView listtextures;
		private System.Windows.Forms.ImageList smallimages;
		private System.Windows.Forms.TabPage tabmodes;
		private System.Windows.Forms.ListView listmodes;
		private System.Windows.Forms.ColumnHeader colmodename;
		private System.Windows.Forms.ColumnHeader colmodeplugin;
		private System.Windows.Forms.CheckBox shortpaths;
		private System.Windows.Forms.ComboBox startmode;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Button btnRemoveEngine;
		private System.Windows.Forms.Button btnNewEngine;
		private System.Windows.Forms.ComboBox cbEngineSelector;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.ContextMenuStrip copypastemenu;
		private System.Windows.Forms.ToolStripMenuItem copyall;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripMenuItem pasteall;
		private System.Windows.Forms.ToolStripMenuItem pasteresources;
		private System.Windows.Forms.ToolStripMenuItem pasteengines;
		private System.Windows.Forms.ToolStripMenuItem pastecolorpresets;
		private System.Windows.Forms.Label hintlabel;
		private System.Windows.Forms.PictureBox hint;
		private System.Windows.Forms.ToolTip tooltip;
        private System.Windows.Forms.CheckBox linuxpaths;
		private System.Windows.Forms.Button importtexturesets;
		private System.Windows.Forms.OpenFileDialog importtexturesetdialog;
		private System.Windows.Forms.Button exporttexturesets;
		private System.Windows.Forms.SaveFileDialog exporttexturesetdialog;
	}
}
