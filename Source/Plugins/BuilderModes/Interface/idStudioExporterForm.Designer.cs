namespace CodeImp.DoomBuilder.BuilderModes.Interface
{
	partial class idStudioExporterForm
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
			if (disposing && (components != null))
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
            this.gui_ModPath = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.gui_FolderBtn = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.gui_zShift = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.gui_yShift = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.gui_xShift = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.gui_Downscale = new System.Windows.Forms.NumericUpDown();
            this.gbTextureControls = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.gui_ExportTextures = new System.Windows.Forms.CheckBox();
            this.gui_CancelBtn = new System.Windows.Forms.Button();
            this.gui_ExportBtn = new System.Windows.Forms.Button();
            this.gui_fileTree = new System.Windows.Forms.TreeView();
            this.label8 = new System.Windows.Forms.Label();
            this.gui_MapName = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.gui_ExpMapTextures = new System.Windows.Forms.RadioButton();
            this.gui_ExpAllTextures = new System.Windows.Forms.RadioButton();
            this.gui_TextCountMap = new System.Windows.Forms.Label();
            this.gui_TextCountAll = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gui_zShift)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gui_yShift)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gui_xShift)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gui_Downscale)).BeginInit();
            this.gbTextureControls.SuspendLayout();
            this.SuspendLayout();
            // 
            // gui_ModPath
            // 
            this.gui_ModPath.Location = new System.Drawing.Point(72, 13);
            this.gui_ModPath.Name = "gui_ModPath";
            this.gui_ModPath.ReadOnly = true;
            this.gui_ModPath.Size = new System.Drawing.Size(327, 20);
            this.gui_ModPath.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Mod Folder:";
            // 
            // gui_FolderBtn
            // 
            this.gui_FolderBtn.Image = global::CodeImp.DoomBuilder.BuilderModes.Properties.Resources.Folder;
            this.gui_FolderBtn.Location = new System.Drawing.Point(405, 10);
            this.gui_FolderBtn.Name = "gui_FolderBtn";
            this.gui_FolderBtn.Size = new System.Drawing.Size(30, 24);
            this.gui_FolderBtn.TabIndex = 2;
            this.gui_FolderBtn.UseVisualStyleBackColor = true;
            this.gui_FolderBtn.Click += new System.EventHandler(this.evt_FolderButton);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.gui_zShift);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.gui_yShift);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.gui_xShift);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.gui_Downscale);
            this.groupBox1.Location = new System.Drawing.Point(12, 71);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(423, 75);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Transformations";
            // 
            // gui_zShift
            // 
            this.gui_zShift.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.gui_zShift.Location = new System.Drawing.Point(315, 43);
            this.gui_zShift.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.gui_zShift.Minimum = new decimal(new int[] {
            1000000,
            0,
            0,
            -2147483648});
            this.gui_zShift.Name = "gui_zShift";
            this.gui_zShift.Size = new System.Drawing.Size(67, 20);
            this.gui_zShift.TabIndex = 13;
            this.gui_zShift.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(268, 45);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "Z Shift:";
            // 
            // gui_yShift
            // 
            this.gui_yShift.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.gui_yShift.Location = new System.Drawing.Point(195, 43);
            this.gui_yShift.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.gui_yShift.Minimum = new decimal(new int[] {
            1000000,
            0,
            0,
            -2147483648});
            this.gui_yShift.Name = "gui_yShift";
            this.gui_yShift.Size = new System.Drawing.Size(67, 20);
            this.gui_yShift.TabIndex = 11;
            this.gui_yShift.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(148, 45);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(41, 13);
            this.label6.TabIndex = 10;
            this.label6.Text = "Y Shift:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(28, 45);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(41, 13);
            this.label7.TabIndex = 9;
            this.label7.Text = "X Shift:";
            // 
            // gui_xShift
            // 
            this.gui_xShift.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.gui_xShift.Location = new System.Drawing.Point(75, 43);
            this.gui_xShift.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.gui_xShift.Minimum = new decimal(new int[] {
            1000000,
            0,
            0,
            -2147483648});
            this.gui_xShift.Name = "gui_xShift";
            this.gui_xShift.Size = new System.Drawing.Size(67, 20);
            this.gui_xShift.TabIndex = 8;
            this.gui_xShift.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 21);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(63, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Downscale:";
            // 
            // gui_Downscale
            // 
            this.gui_Downscale.DecimalPlaces = 2;
            this.gui_Downscale.Location = new System.Drawing.Point(75, 19);
            this.gui_Downscale.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.gui_Downscale.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.gui_Downscale.Name = "gui_Downscale";
            this.gui_Downscale.Size = new System.Drawing.Size(67, 20);
            this.gui_Downscale.TabIndex = 0;
            this.gui_Downscale.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // gbTextureControls
            // 
            this.gbTextureControls.Controls.Add(this.gui_TextCountAll);
            this.gbTextureControls.Controls.Add(this.gui_TextCountMap);
            this.gbTextureControls.Controls.Add(this.gui_ExpAllTextures);
            this.gbTextureControls.Controls.Add(this.gui_ExpMapTextures);
            this.gbTextureControls.Controls.Add(this.label3);
            this.gbTextureControls.Controls.Add(this.gui_ExportTextures);
            this.gbTextureControls.Location = new System.Drawing.Point(12, 162);
            this.gbTextureControls.Name = "gbTextureControls";
            this.gbTextureControls.Size = new System.Drawing.Size(423, 100);
            this.gbTextureControls.TabIndex = 4;
            this.gbTextureControls.TabStop = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 75);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(190, 13);
            this.label3.TabIndex = 14;
            this.label3.Text = "Exporting textures may take some time.";
            // 
            // gui_ExportTextures
            // 
            this.gui_ExportTextures.AutoSize = true;
            this.gui_ExportTextures.Location = new System.Drawing.Point(6, 0);
            this.gui_ExportTextures.Name = "gui_ExportTextures";
            this.gui_ExportTextures.Size = new System.Drawing.Size(100, 17);
            this.gui_ExportTextures.TabIndex = 0;
            this.gui_ExportTextures.Text = "Export Textures";
            this.gui_ExportTextures.UseVisualStyleBackColor = true;
            // 
            // gui_CancelBtn
            // 
            this.gui_CancelBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.gui_CancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.gui_CancelBtn.Location = new System.Drawing.Point(360, 575);
            this.gui_CancelBtn.Name = "gui_CancelBtn";
            this.gui_CancelBtn.Size = new System.Drawing.Size(75, 23);
            this.gui_CancelBtn.TabIndex = 10;
            this.gui_CancelBtn.Text = "Cancel";
            this.gui_CancelBtn.UseVisualStyleBackColor = true;
            this.gui_CancelBtn.Click += new System.EventHandler(this.evt_CancelButton);
            // 
            // gui_ExportBtn
            // 
            this.gui_ExportBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.gui_ExportBtn.Location = new System.Drawing.Point(279, 575);
            this.gui_ExportBtn.Name = "gui_ExportBtn";
            this.gui_ExportBtn.Size = new System.Drawing.Size(75, 23);
            this.gui_ExportBtn.TabIndex = 9;
            this.gui_ExportBtn.Text = "Export";
            this.gui_ExportBtn.UseVisualStyleBackColor = true;
            this.gui_ExportBtn.Click += new System.EventHandler(this.evt_ButtonExport);
            // 
            // gui_fileTree
            // 
            this.gui_fileTree.Location = new System.Drawing.Point(12, 264);
            this.gui_fileTree.Name = "gui_fileTree";
            this.gui_fileTree.Size = new System.Drawing.Size(423, 305);
            this.gui_fileTree.TabIndex = 11;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(9, 42);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(57, 13);
            this.label8.TabIndex = 12;
            this.label8.Text = "File Name:";
            // 
            // gui_MapName
            // 
            this.gui_MapName.Location = new System.Drawing.Point(72, 39);
            this.gui_MapName.Name = "gui_MapName";
            this.gui_MapName.Size = new System.Drawing.Size(327, 20);
            this.gui_MapName.TabIndex = 13;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 575);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(149, 13);
            this.label5.TabIndex = 14;
            this.label5.Text = "This tool is still in development";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(12, 588);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(208, 13);
            this.label9.TabIndex = 15;
            this.label9.Text = "Not all map features may convert correctly.";
            // 
            // gui_ExpMapTextures
            // 
            this.gui_ExpMapTextures.AutoSize = true;
            this.gui_ExpMapTextures.Checked = true;
            this.gui_ExpMapTextures.Location = new System.Drawing.Point(9, 21);
            this.gui_ExpMapTextures.Name = "gui_ExpMapTextures";
            this.gui_ExpMapTextures.Size = new System.Drawing.Size(114, 17);
            this.gui_ExpMapTextures.TabIndex = 15;
            this.gui_ExpMapTextures.TabStop = true;
            this.gui_ExpMapTextures.Text = "Map Textures Only";
            this.gui_ExpMapTextures.UseVisualStyleBackColor = true;
            // 
            // gui_ExpAllTextures
            // 
            this.gui_ExpAllTextures.AutoSize = true;
            this.gui_ExpAllTextures.Location = new System.Drawing.Point(9, 41);
            this.gui_ExpAllTextures.Name = "gui_ExpAllTextures";
            this.gui_ExpAllTextures.Size = new System.Drawing.Size(80, 17);
            this.gui_ExpAllTextures.TabIndex = 16;
            this.gui_ExpAllTextures.Text = "All Textures";
            this.gui_ExpAllTextures.UseVisualStyleBackColor = true;
            // 
            // gui_TextCountMap
            // 
            this.gui_TextCountMap.AutoSize = true;
            this.gui_TextCountMap.Location = new System.Drawing.Point(129, 25);
            this.gui_TextCountMap.Name = "gui_TextCountMap";
            this.gui_TextCountMap.Size = new System.Drawing.Size(98, 13);
            this.gui_TextCountMap.TabIndex = 17;
            this.gui_TextCountMap.Text = "[Map Export Count]";
            // 
            // gui_TextCountAll
            // 
            this.gui_TextCountAll.AutoSize = true;
            this.gui_TextCountAll.Location = new System.Drawing.Point(129, 43);
            this.gui_TextCountAll.Name = "gui_TextCountAll";
            this.gui_TextCountAll.Size = new System.Drawing.Size(88, 13);
            this.gui_TextCountAll.TabIndex = 18;
            this.gui_TextCountAll.Text = "[All Export Count]";
            // 
            // idStudioExporterForm
            // 
            this.AcceptButton = this.gui_ExportBtn;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.gui_CancelBtn;
            this.ClientSize = new System.Drawing.Size(447, 610);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.gui_MapName);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.gui_fileTree);
            this.Controls.Add(this.gui_CancelBtn);
            this.Controls.Add(this.gui_ExportBtn);
            this.Controls.Add(this.gbTextureControls);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.gui_FolderBtn);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.gui_ModPath);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "idStudioExporterForm";
            this.Opacity = 0D;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Export to idStudio";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gui_zShift)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gui_yShift)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gui_xShift)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gui_Downscale)).EndInit();
            this.gbTextureControls.ResumeLayout(false);
            this.gbTextureControls.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox gui_ModPath;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button gui_FolderBtn;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.NumericUpDown gui_Downscale;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.NumericUpDown gui_yShift;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.NumericUpDown gui_xShift;
		private System.Windows.Forms.GroupBox gbTextureControls;
		private System.Windows.Forms.CheckBox gui_ExportTextures;
		private System.Windows.Forms.Button gui_CancelBtn;
		private System.Windows.Forms.Button gui_ExportBtn;
		private System.Windows.Forms.TreeView gui_fileTree;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.TextBox gui_MapName;
		private System.Windows.Forms.NumericUpDown gui_zShift;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.RadioButton gui_ExpAllTextures;
		private System.Windows.Forms.RadioButton gui_ExpMapTextures;
		private System.Windows.Forms.Label gui_TextCountAll;
		private System.Windows.Forms.Label gui_TextCountMap;
	}
}