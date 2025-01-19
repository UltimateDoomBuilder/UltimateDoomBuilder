namespace CodeImp.DoomBuilder.BuilderModes.Interface
{
	partial class WavefrontSettingsForm
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
			this.tbExportPath = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label1.AutoSize = true;
			this.cbExportForGZDoom = new System.Windows.Forms.CheckBox();
			this.export = new System.Windows.Forms.Button();
			this.cancel = new System.Windows.Forms.Button();
			this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
			this.cbExportTextures = new System.Windows.Forms.CheckBox();
			this.nudScale = new System.Windows.Forms.NumericUpDown();
			this.label2 = new System.Windows.Forms.Label();
			this.label2.AutoSize = true;
			this.gbGZDoom = new System.Windows.Forms.GroupBox();
			this.cbGenerateCode = new System.Windows.Forms.CheckBox();
			this.cbGenerateModeldef = new System.Windows.Forms.CheckBox();
			this.actorNameError = new System.Windows.Forms.PictureBox();
			this.gbActorFormat = new System.Windows.Forms.GroupBox();
			this.rbZScript = new System.Windows.Forms.RadioButton();
			this.rbDecorate = new System.Windows.Forms.RadioButton();
			this.gbActorSettings = new System.Windows.Forms.GroupBox();
			this.tbSprite = new System.Windows.Forms.TextBox();
			this.label7 = new System.Windows.Forms.Label();
			this.label7.AutoSize = true;
			this.cbSolid = new System.Windows.Forms.CheckBox();
			this.cbSpawnOnCeiling = new System.Windows.Forms.CheckBox();
			this.cbNoGravity = new System.Windows.Forms.CheckBox();
			this.bResetPaths = new System.Windows.Forms.Button();
			this.tbActorName = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.label6.AutoSize = true;
			this.label5 = new System.Windows.Forms.Label();
			this.label5.AutoSize = true;
			this.bBrowseModelPath = new System.Windows.Forms.Button();
			this.tbModelPath = new System.Windows.Forms.TextBox();
			this.bBrowseActorPath = new System.Windows.Forms.Button();
			this.tbActorPath = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.label4.AutoSize = true;
			this.bBrowseBasePath = new System.Windows.Forms.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.label3.AutoSize = true;
			this.tbBasePath = new System.Windows.Forms.TextBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.bRemoveTexture = new System.Windows.Forms.Button();
			this.bAddFlat = new System.Windows.Forms.Button();
			this.bAddTexture = new System.Windows.Forms.Button();
			this.lbSkipTextures = new System.Windows.Forms.ListBox();
			this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
			this.groupBox4 = new System.Windows.Forms.GroupBox();
			this.cbNormalizeLowestVertex = new System.Windows.Forms.CheckBox();
			this.cbCenterModel = new System.Windows.Forms.CheckBox();
			this.cbIgnoreControlSectors = new System.Windows.Forms.CheckBox();
			this.browse = new System.Windows.Forms.Button();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			((System.ComponentModel.ISupportInitialize)(this.nudScale)).BeginInit();
			this.gbGZDoom.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.actorNameError)).BeginInit();
			this.gbActorFormat.SuspendLayout();
			this.gbActorSettings.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.groupBox4.SuspendLayout();
			this.SuspendLayout();
			// 
			// tbExportPath
			// 
			this.tbExportPath.Location = new System.Drawing.Point(55, 12);
			this.tbExportPath.Name = "tbExportPath";
			this.tbExportPath.Size = new System.Drawing.Size(344, 20);
			this.tbExportPath.TabIndex = 1;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(18, 14);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(32, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Path:";
			// 
			// cbExportForGZDoom
			// 
			this.cbExportForGZDoom.AutoSize = true;
			this.cbExportForGZDoom.Location = new System.Drawing.Point(6, 0);
			this.cbExportForGZDoom.Name = "cbExportForGZDoom";
			this.cbExportForGZDoom.Size = new System.Drawing.Size(117, 17);
			this.cbExportForGZDoom.TabIndex = 0;
			this.cbExportForGZDoom.Text = "Export for GZDoom";
			this.cbExportForGZDoom.UseVisualStyleBackColor = true;
			this.cbExportForGZDoom.CheckedChanged += new System.EventHandler(this.cbFixScale_CheckedChanged);
			// 
			// export
			// 
			this.export.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.export.Location = new System.Drawing.Point(279, 575);
			this.export.Name = "export";
			this.export.Size = new System.Drawing.Size(75, 23);
			this.export.TabIndex = 7;
			this.export.Text = "Export";
			this.export.UseVisualStyleBackColor = true;
			this.export.Click += new System.EventHandler(this.export_Click);
			// 
			// cancel
			// 
			this.cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancel.Location = new System.Drawing.Point(360, 575);
			this.cancel.Name = "cancel";
			this.cancel.Size = new System.Drawing.Size(75, 23);
			this.cancel.TabIndex = 8;
			this.cancel.Text = "Cancel";
			this.cancel.UseVisualStyleBackColor = true;
			this.cancel.Click += new System.EventHandler(this.cancel_Click);
			// 
			// saveFileDialog
			// 
			this.saveFileDialog.DefaultExt = "obj";
			this.saveFileDialog.Filter = "Wavefront obj files|*.obj";
			this.saveFileDialog.Title = "Select save location:";
			// 
			// cbExportTextures
			// 
			this.cbExportTextures.AutoSize = true;
			this.cbExportTextures.Location = new System.Drawing.Point(200, 19);
			this.cbExportTextures.Name = "cbExportTextures";
			this.cbExportTextures.Size = new System.Drawing.Size(96, 17);
			this.cbExportTextures.TabIndex = 2;
			this.cbExportTextures.Text = "Export textures";
			this.cbExportTextures.UseVisualStyleBackColor = true;
			// 
			// nudScale
			// 
			this.nudScale.DecimalPlaces = 4;
			this.nudScale.Location = new System.Drawing.Point(55, 38);
			this.nudScale.Maximum = new decimal(new int[] {
            2048,
            0,
            0,
            0});
			this.nudScale.Minimum = new decimal(new int[] {
            2048,
            0,
            0,
            -2147483648});
			this.nudScale.Name = "nudScale";
			this.nudScale.Size = new System.Drawing.Size(94, 20);
			this.nudScale.TabIndex = 4;
			this.nudScale.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(12, 40);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(37, 13);
			this.label2.TabIndex = 3;
			this.label2.Text = "Scale:";
			// 
			// gbGZDoom
			// 
			this.gbGZDoom.Controls.Add(this.cbGenerateCode);
			this.gbGZDoom.Controls.Add(this.cbGenerateModeldef);
			this.gbGZDoom.Controls.Add(this.actorNameError);
			this.gbGZDoom.Controls.Add(this.gbActorFormat);
			this.gbGZDoom.Controls.Add(this.gbActorSettings);
			this.gbGZDoom.Controls.Add(this.bResetPaths);
			this.gbGZDoom.Controls.Add(this.tbActorName);
			this.gbGZDoom.Controls.Add(this.label6);
			this.gbGZDoom.Controls.Add(this.label5);
			this.gbGZDoom.Controls.Add(this.bBrowseModelPath);
			this.gbGZDoom.Controls.Add(this.tbModelPath);
			this.gbGZDoom.Controls.Add(this.bBrowseActorPath);
			this.gbGZDoom.Controls.Add(this.tbActorPath);
			this.gbGZDoom.Controls.Add(this.label4);
			this.gbGZDoom.Controls.Add(this.bBrowseBasePath);
			this.gbGZDoom.Controls.Add(this.label3);
			this.gbGZDoom.Controls.Add(this.tbBasePath);
			this.gbGZDoom.Controls.Add(this.groupBox1);
			this.gbGZDoom.Controls.Add(this.cbExportForGZDoom);
			this.gbGZDoom.Location = new System.Drawing.Point(15, 141);
			this.gbGZDoom.Name = "gbGZDoom";
			this.gbGZDoom.Size = new System.Drawing.Size(420, 425);
			this.gbGZDoom.TabIndex = 6;
			this.gbGZDoom.TabStop = false;
			// 
			// cbGenerateCode
			// 
			this.cbGenerateCode.AutoSize = true;
			this.cbGenerateCode.Location = new System.Drawing.Point(8, 161);
			this.cbGenerateCode.Name = "cbGenerateCode";
			this.cbGenerateCode.Size = new System.Drawing.Size(171, 17);
			this.cbGenerateCode.TabIndex = 25;
			this.cbGenerateCode.Text = "Generate ZScript/DECORATE";
			this.cbGenerateCode.UseVisualStyleBackColor = true;
			this.cbGenerateCode.CheckedChanged += new System.EventHandler(this.cbGenerateCode_CheckedChanged);
			// 
			// cbGenerateModeldef
			// 
			this.cbGenerateModeldef.AutoSize = true;
			this.cbGenerateModeldef.Location = new System.Drawing.Point(8, 184);
			this.cbGenerateModeldef.Name = "cbGenerateModeldef";
			this.cbGenerateModeldef.Size = new System.Drawing.Size(132, 17);
			this.cbGenerateModeldef.TabIndex = 25;
			this.cbGenerateModeldef.Text = "Generate MODELDEF";
			this.cbGenerateModeldef.UseVisualStyleBackColor = true;
			this.cbGenerateModeldef.CheckedChanged += new System.EventHandler(this.cbGenerateModeldef_CheckedChanged);
			// 
			// actorNameError
			// 
			this.actorNameError.Image = global::CodeImp.DoomBuilder.BuilderModes.Properties.Resources.Warning;
			this.actorNameError.Location = new System.Drawing.Point(376, 25);
			this.actorNameError.Name = "actorNameError";
			this.actorNameError.Size = new System.Drawing.Size(16, 16);
			this.actorNameError.TabIndex = 24;
			this.actorNameError.TabStop = false;
			this.actorNameError.Visible = false;
			// 
			// gbActorFormat
			// 
			this.gbActorFormat.Controls.Add(this.rbZScript);
			this.gbActorFormat.Controls.Add(this.rbDecorate);
			this.gbActorFormat.Location = new System.Drawing.Point(10, 340);
			this.gbActorFormat.Name = "gbActorFormat";
			this.gbActorFormat.Size = new System.Drawing.Size(170, 73);
			this.gbActorFormat.TabIndex = 14;
			this.gbActorFormat.TabStop = false;
			this.gbActorFormat.Text = "Actor format";
			// 
			// rbZScript
			// 
			this.rbZScript.AutoSize = true;
			this.rbZScript.Checked = true;
			this.rbZScript.Location = new System.Drawing.Point(6, 20);
			this.rbZScript.Name = "rbZScript";
			this.rbZScript.Size = new System.Drawing.Size(59, 17);
			this.rbZScript.TabIndex = 0;
			this.rbZScript.TabStop = true;
			this.rbZScript.Text = "ZScript";
			this.rbZScript.UseVisualStyleBackColor = true;
			// 
			// rbDecorate
			// 
			this.rbDecorate.AutoSize = true;
			this.rbDecorate.Location = new System.Drawing.Point(6, 44);
			this.rbDecorate.Name = "rbDecorate";
			this.rbDecorate.Size = new System.Drawing.Size(84, 17);
			this.rbDecorate.TabIndex = 1;
			this.rbDecorate.TabStop = true;
			this.rbDecorate.Text = "DECORATE";
			this.rbDecorate.UseVisualStyleBackColor = true;
			// 
			// gbActorSettings
			// 
			this.gbActorSettings.Controls.Add(this.tbSprite);
			this.gbActorSettings.Controls.Add(this.label7);
			this.gbActorSettings.Controls.Add(this.cbSolid);
			this.gbActorSettings.Controls.Add(this.cbSpawnOnCeiling);
			this.gbActorSettings.Controls.Add(this.cbNoGravity);
			this.gbActorSettings.Location = new System.Drawing.Point(11, 212);
			this.gbActorSettings.Name = "gbActorSettings";
			this.gbActorSettings.Size = new System.Drawing.Size(169, 122);
			this.gbActorSettings.TabIndex = 13;
			this.gbActorSettings.TabStop = false;
			this.gbActorSettings.Text = "Actor settings";
			// 
			// tbSprite
			// 
			this.tbSprite.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
			this.tbSprite.Location = new System.Drawing.Point(50, 88);
			this.tbSprite.MaxLength = 4;
			this.tbSprite.Name = "tbSprite";
			this.tbSprite.Size = new System.Drawing.Size(69, 20);
			this.tbSprite.TabIndex = 4;
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(7, 91);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(37, 13);
			this.label7.TabIndex = 3;
			this.label7.Text = "Sprite:";
			// 
			// cbSolid
			// 
			this.cbSolid.AutoSize = true;
			this.cbSolid.Location = new System.Drawing.Point(7, 67);
			this.cbSolid.Name = "cbSolid";
			this.cbSolid.Size = new System.Drawing.Size(49, 17);
			this.cbSolid.TabIndex = 2;
			this.cbSolid.Text = "Solid";
			this.cbSolid.UseVisualStyleBackColor = true;
			// 
			// cbSpawnOnCeiling
			// 
			this.cbSpawnOnCeiling.AutoSize = true;
			this.cbSpawnOnCeiling.Location = new System.Drawing.Point(7, 43);
			this.cbSpawnOnCeiling.Name = "cbSpawnOnCeiling";
			this.cbSpawnOnCeiling.Size = new System.Drawing.Size(107, 17);
			this.cbSpawnOnCeiling.TabIndex = 1;
			this.cbSpawnOnCeiling.Text = "Spawn on ceiling";
			this.cbSpawnOnCeiling.UseVisualStyleBackColor = true;
			this.cbSpawnOnCeiling.CheckedChanged += new System.EventHandler(this.cbSpawnOnCeiling_CheckedChanged);
			// 
			// cbNoGravity
			// 
			this.cbNoGravity.AutoSize = true;
			this.cbNoGravity.Location = new System.Drawing.Point(7, 20);
			this.cbNoGravity.Name = "cbNoGravity";
			this.cbNoGravity.Size = new System.Drawing.Size(74, 17);
			this.cbNoGravity.TabIndex = 0;
			this.cbNoGravity.Text = "No gravity";
			this.cbNoGravity.UseVisualStyleBackColor = true;
			// 
			// bResetPaths
			// 
			this.bResetPaths.Location = new System.Drawing.Point(74, 127);
			this.bResetPaths.Name = "bResetPaths";
			this.bResetPaths.Size = new System.Drawing.Size(75, 23);
			this.bResetPaths.TabIndex = 12;
			this.bResetPaths.Text = "Reset paths";
			this.bResetPaths.UseVisualStyleBackColor = true;
			this.bResetPaths.Click += new System.EventHandler(this.bResetPaths_Click);
			// 
			// tbActorName
			// 
			this.tbActorName.Location = new System.Drawing.Point(74, 23);
			this.tbActorName.Name = "tbActorName";
			this.tbActorName.Size = new System.Drawing.Size(296, 20);
			this.tbActorName.TabIndex = 2;
			this.tbActorName.TextChanged += new System.EventHandler(this.tbActorName_TextChanged);
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(4, 26);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(64, 13);
			this.label6.TabIndex = 1;
			this.label6.Text = "Actor name:";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(5, 104);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(63, 13);
			this.label5.TabIndex = 9;
			this.label5.Text = "Model path:";
			// 
			// bBrowseModelPath
			// 
			this.bBrowseModelPath.Image = global::CodeImp.DoomBuilder.BuilderModes.Properties.Resources.Folder;
			this.bBrowseModelPath.Location = new System.Drawing.Point(376, 99);
			this.bBrowseModelPath.Name = "bBrowseModelPath";
			this.bBrowseModelPath.Size = new System.Drawing.Size(30, 24);
			this.bBrowseModelPath.TabIndex = 11;
			this.bBrowseModelPath.UseVisualStyleBackColor = true;
			this.bBrowseModelPath.Click += new System.EventHandler(this.bBrowseModelPath_Click);
			// 
			// tbModelPath
			// 
			this.tbModelPath.Location = new System.Drawing.Point(74, 101);
			this.tbModelPath.Name = "tbModelPath";
			this.tbModelPath.Size = new System.Drawing.Size(296, 20);
			this.tbModelPath.TabIndex = 10;
			// 
			// bBrowseActorPath
			// 
			this.bBrowseActorPath.Image = global::CodeImp.DoomBuilder.BuilderModes.Properties.Resources.Folder;
			this.bBrowseActorPath.Location = new System.Drawing.Point(376, 73);
			this.bBrowseActorPath.Name = "bBrowseActorPath";
			this.bBrowseActorPath.Size = new System.Drawing.Size(30, 24);
			this.bBrowseActorPath.TabIndex = 8;
			this.bBrowseActorPath.UseVisualStyleBackColor = true;
			this.bBrowseActorPath.Click += new System.EventHandler(this.bBrowseActorPath_Click);
			// 
			// tbActorPath
			// 
			this.tbActorPath.Location = new System.Drawing.Point(74, 75);
			this.tbActorPath.Name = "tbActorPath";
			this.tbActorPath.Size = new System.Drawing.Size(296, 20);
			this.tbActorPath.TabIndex = 7;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(9, 78);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(59, 13);
			this.label4.TabIndex = 6;
			this.label4.Text = "Actor path:";
			// 
			// bBrowseBasePath
			// 
			this.bBrowseBasePath.Image = global::CodeImp.DoomBuilder.BuilderModes.Properties.Resources.Folder;
			this.bBrowseBasePath.Location = new System.Drawing.Point(376, 47);
			this.bBrowseBasePath.Name = "bBrowseBasePath";
			this.bBrowseBasePath.Size = new System.Drawing.Size(30, 24);
			this.bBrowseBasePath.TabIndex = 5;
			this.bBrowseBasePath.UseVisualStyleBackColor = true;
			this.bBrowseBasePath.Click += new System.EventHandler(this.bBrowseBasePath_Click);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(10, 52);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(58, 13);
			this.label3.TabIndex = 3;
			this.label3.Text = "Base path:";
			// 
			// tbBasePath
			// 
			this.tbBasePath.Location = new System.Drawing.Point(74, 49);
			this.tbBasePath.Name = "tbBasePath";
			this.tbBasePath.Size = new System.Drawing.Size(296, 20);
			this.tbBasePath.TabIndex = 4;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.bRemoveTexture);
			this.groupBox1.Controls.Add(this.bAddFlat);
			this.groupBox1.Controls.Add(this.bAddTexture);
			this.groupBox1.Controls.Add(this.lbSkipTextures);
			this.groupBox1.Location = new System.Drawing.Point(186, 212);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(222, 201);
			this.groupBox1.TabIndex = 15;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Skip textures";
			// 
			// bRemoveTexture
			// 
			this.bRemoveTexture.Location = new System.Drawing.Point(7, 140);
			this.bRemoveTexture.Name = "bRemoveTexture";
			this.bRemoveTexture.Size = new System.Drawing.Size(209, 23);
			this.bRemoveTexture.TabIndex = 1;
			this.bRemoveTexture.Text = "Remove selected";
			this.bRemoveTexture.UseVisualStyleBackColor = true;
			this.bRemoveTexture.Click += new System.EventHandler(this.bRemoveTexture_Click);
			// 
			// bAddFlat
			// 
			this.bAddFlat.Location = new System.Drawing.Point(114, 169);
			this.bAddFlat.Name = "bAddFlat";
			this.bAddFlat.Size = new System.Drawing.Size(102, 23);
			this.bAddFlat.TabIndex = 3;
			this.bAddFlat.Text = "Add flat";
			this.bAddFlat.UseVisualStyleBackColor = true;
			this.bAddFlat.Click += new System.EventHandler(this.bAddFlat_Click);
			// 
			// bAddTexture
			// 
			this.bAddTexture.Location = new System.Drawing.Point(7, 169);
			this.bAddTexture.Name = "bAddTexture";
			this.bAddTexture.Size = new System.Drawing.Size(102, 23);
			this.bAddTexture.TabIndex = 2;
			this.bAddTexture.Text = "Add texture";
			this.bAddTexture.UseVisualStyleBackColor = true;
			this.bAddTexture.Click += new System.EventHandler(this.bAddTexture_Click);
			// 
			// lbSkipTextures
			// 
			this.lbSkipTextures.FormattingEnabled = true;
			this.lbSkipTextures.Location = new System.Drawing.Point(6, 19);
			this.lbSkipTextures.Name = "lbSkipTextures";
			this.lbSkipTextures.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this.lbSkipTextures.Size = new System.Drawing.Size(210, 108);
			this.lbSkipTextures.TabIndex = 0;
			// 
			// groupBox4
			// 
			this.groupBox4.Controls.Add(this.cbNormalizeLowestVertex);
			this.groupBox4.Controls.Add(this.cbCenterModel);
			this.groupBox4.Controls.Add(this.cbIgnoreControlSectors);
			this.groupBox4.Controls.Add(this.cbExportTextures);
			this.groupBox4.Location = new System.Drawing.Point(15, 64);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Size = new System.Drawing.Size(420, 71);
			this.groupBox4.TabIndex = 5;
			this.groupBox4.TabStop = false;
			this.groupBox4.Text = "General settings";
			// 
			// cbNormalizeLowestVertex
			// 
			this.cbNormalizeLowestVertex.AutoSize = true;
			this.cbNormalizeLowestVertex.Checked = true;
			this.cbNormalizeLowestVertex.CheckState = System.Windows.Forms.CheckState.Checked;
			this.cbNormalizeLowestVertex.Location = new System.Drawing.Point(6, 44);
			this.cbNormalizeLowestVertex.Name = "cbNormalizeLowestVertex";
			this.cbNormalizeLowestVertex.Size = new System.Drawing.Size(166, 17);
			this.cbNormalizeLowestVertex.TabIndex = 1;
			this.cbNormalizeLowestVertex.Text = "Normalize lowest vertex z to 0";
			this.cbNormalizeLowestVertex.UseVisualStyleBackColor = true;
			// 
			// cbCenterModel
			// 
			this.cbCenterModel.AutoSize = true;
			this.cbCenterModel.Checked = true;
			this.cbCenterModel.CheckState = System.Windows.Forms.CheckState.Checked;
			this.cbCenterModel.Location = new System.Drawing.Point(6, 21);
			this.cbCenterModel.Name = "cbCenterModel";
			this.cbCenterModel.Size = new System.Drawing.Size(88, 17);
			this.cbCenterModel.TabIndex = 0;
			this.cbCenterModel.Text = "Center model";
			this.cbCenterModel.UseVisualStyleBackColor = true;
			// 
			// cbIgnoreControlSectors
			// 
			this.cbIgnoreControlSectors.AutoSize = true;
			this.cbIgnoreControlSectors.Checked = true;
			this.cbIgnoreControlSectors.CheckState = System.Windows.Forms.CheckState.Checked;
			this.cbIgnoreControlSectors.Location = new System.Drawing.Point(200, 42);
			this.cbIgnoreControlSectors.Name = "cbIgnoreControlSectors";
			this.cbIgnoreControlSectors.Size = new System.Drawing.Size(168, 17);
			this.cbIgnoreControlSectors.TabIndex = 3;
			this.cbIgnoreControlSectors.Text = "Ignore 3D floor control sectors";
			this.cbIgnoreControlSectors.UseVisualStyleBackColor = true;
			// 
			// browse
			// 
			this.browse.Image = global::CodeImp.DoomBuilder.BuilderModes.Properties.Resources.Folder;
			this.browse.Location = new System.Drawing.Point(405, 10);
			this.browse.Name = "browse";
			this.browse.Size = new System.Drawing.Size(30, 24);
			this.browse.TabIndex = 2;
			this.browse.UseVisualStyleBackColor = true;
			this.browse.Click += new System.EventHandler(this.browse_Click);
			// 
			// WavefrontSettingsForm
			// 
			this.AcceptButton = this.export;
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.CancelButton = this.cancel;
			this.ClientSize = new System.Drawing.Size(447, 610);
			this.Controls.Add(this.groupBox4);
			this.Controls.Add(this.gbGZDoom);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.nudScale);
			this.Controls.Add(this.cancel);
			this.Controls.Add(this.export);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.browse);
			this.Controls.Add(this.tbExportPath);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "WavefrontSettingsForm";
			this.Opacity = 0D;
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.Text = "Export to Wavefront .obj";
			((System.ComponentModel.ISupportInitialize)(this.nudScale)).EndInit();
			this.gbGZDoom.ResumeLayout(false);
			this.gbGZDoom.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.actorNameError)).EndInit();
			this.gbActorFormat.ResumeLayout(false);
			this.gbActorFormat.PerformLayout();
			this.gbActorSettings.ResumeLayout(false);
			this.gbActorSettings.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			this.groupBox4.ResumeLayout(false);
			this.groupBox4.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox tbExportPath;
		private System.Windows.Forms.Button browse;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.CheckBox cbExportForGZDoom;
		private System.Windows.Forms.Button export;
		private System.Windows.Forms.Button cancel;
		private System.Windows.Forms.SaveFileDialog saveFileDialog;
		private System.Windows.Forms.CheckBox cbExportTextures;
		private System.Windows.Forms.NumericUpDown nudScale;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.GroupBox gbGZDoom;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.ListBox lbSkipTextures;
		private System.Windows.Forms.Button bRemoveTexture;
		private System.Windows.Forms.Button bAddFlat;
		private System.Windows.Forms.Button bAddTexture;
		private System.Windows.Forms.Button bBrowseBasePath;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox tbBasePath;
		private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
		private System.Windows.Forms.Button bBrowseActorPath;
		private System.Windows.Forms.TextBox tbActorPath;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Button bBrowseModelPath;
		private System.Windows.Forms.TextBox tbModelPath;
		private System.Windows.Forms.TextBox tbActorName;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Button bResetPaths;
		private System.Windows.Forms.GroupBox gbActorSettings;
		private System.Windows.Forms.CheckBox cbNoGravity;
		private System.Windows.Forms.CheckBox cbSpawnOnCeiling;
		private System.Windows.Forms.CheckBox cbSolid;
		private System.Windows.Forms.RadioButton rbDecorate;
		private System.Windows.Forms.RadioButton rbZScript;
		private System.Windows.Forms.GroupBox gbActorFormat;
		private System.Windows.Forms.TextBox tbSprite;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.CheckBox cbNormalizeLowestVertex;
		private System.Windows.Forms.CheckBox cbCenterModel;
		private System.Windows.Forms.CheckBox cbIgnoreControlSectors;
		private System.Windows.Forms.PictureBox actorNameError;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.CheckBox cbGenerateCode;
		private System.Windows.Forms.CheckBox cbGenerateModeldef;
	}
}
