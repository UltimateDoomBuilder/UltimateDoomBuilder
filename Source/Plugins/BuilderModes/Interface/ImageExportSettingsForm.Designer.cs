namespace CodeImp.DoomBuilder.BuilderModes.Interface
{
	partial class ImageExportSettingsForm
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
			this.components = new System.ComponentModel.Container();
			this.tbExportPath = new System.Windows.Forms.TextBox();
			this.browse = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.close = new System.Windows.Forms.Button();
			this.export = new System.Windows.Forms.Button();
			this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
			this.cbImageFormat = new System.Windows.Forms.ComboBox();
			this.cbPixelFormat = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.rbFloor = new System.Windows.Forms.RadioButton();
			this.rbCeiling = new System.Windows.Forms.RadioButton();
			this.cbFullbright = new System.Windows.Forms.CheckBox();
			this.cbBrightmap = new System.Windows.Forms.CheckBox();
			this.cbTiles = new System.Windows.Forms.CheckBox();
			this.cbScale = new System.Windows.Forms.ComboBox();
			this.label4 = new System.Windows.Forms.Label();
			this.progress = new System.Windows.Forms.ProgressBar();
			this.lbPhase = new System.Windows.Forms.Label();
			this.cbApplySectorColors = new System.Windows.Forms.CheckBox();
			this.cbTransparency = new System.Windows.Forms.CheckBox();
			this.toolTip = new System.Windows.Forms.ToolTip(this.components);
			this.SuspendLayout();
			// 
			// tbExportPath
			// 
			this.tbExportPath.Location = new System.Drawing.Point(50, 9);
			this.tbExportPath.Name = "tbExportPath";
			this.tbExportPath.Size = new System.Drawing.Size(344, 20);
			this.tbExportPath.TabIndex = 2;
			// 
			// browse
			// 
			this.browse.Image = global::CodeImp.DoomBuilder.BuilderModes.Properties.Resources.Folder;
			this.browse.Location = new System.Drawing.Point(400, 7);
			this.browse.Name = "browse";
			this.browse.Size = new System.Drawing.Size(30, 24);
			this.browse.TabIndex = 3;
			this.browse.UseVisualStyleBackColor = true;
			this.browse.Click += new System.EventHandler(this.browse_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 12);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(32, 13);
			this.label1.TabIndex = 4;
			this.label1.Text = "Path:";
			// 
			// close
			// 
			this.close.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.close.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.close.Location = new System.Drawing.Point(360, 172);
			this.close.Name = "close";
			this.close.Size = new System.Drawing.Size(75, 23);
			this.close.TabIndex = 7;
			this.close.Text = "Close";
			this.close.UseVisualStyleBackColor = true;
			this.close.Click += new System.EventHandler(this.close_Click);
			// 
			// export
			// 
			this.export.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.export.Location = new System.Drawing.Point(279, 172);
			this.export.Name = "export";
			this.export.Size = new System.Drawing.Size(75, 23);
			this.export.TabIndex = 6;
			this.export.Text = "Export";
			this.export.UseVisualStyleBackColor = true;
			this.export.Click += new System.EventHandler(this.export_Click);
			// 
			// saveFileDialog
			// 
			this.saveFileDialog.Filter = "PNG (*.png)|*.png|JPEG (*.jpg)|*.jpg|All files (*.*)|*.*";
			// 
			// cbImageFormat
			// 
			this.cbImageFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbImageFormat.FormattingEnabled = true;
			this.cbImageFormat.Items.AddRange(new object[] {
            "PNG",
            "JPG"});
			this.cbImageFormat.Location = new System.Drawing.Point(102, 35);
			this.cbImageFormat.Name = "cbImageFormat";
			this.cbImageFormat.Size = new System.Drawing.Size(71, 21);
			this.cbImageFormat.TabIndex = 8;
			this.cbImageFormat.SelectedIndexChanged += new System.EventHandler(this.cbImageFormat_SelectedIndexChanged);
			// 
			// cbPixelFormat
			// 
			this.cbPixelFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbPixelFormat.FormattingEnabled = true;
			this.cbPixelFormat.Items.AddRange(new object[] {
            "32 bit",
            "24 bit",
            "16 bit"});
			this.cbPixelFormat.Location = new System.Drawing.Point(102, 62);
			this.cbPixelFormat.Name = "cbPixelFormat";
			this.cbPixelFormat.Size = new System.Drawing.Size(71, 21);
			this.cbPixelFormat.TabIndex = 9;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(12, 38);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(71, 13);
			this.label2.TabIndex = 10;
			this.label2.Text = "Image format:";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(12, 65);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(64, 13);
			this.label3.TabIndex = 11;
			this.label3.Text = "Color depth:";
			// 
			// rbFloor
			// 
			this.rbFloor.AutoSize = true;
			this.rbFloor.Checked = true;
			this.rbFloor.Location = new System.Drawing.Point(197, 39);
			this.rbFloor.Name = "rbFloor";
			this.rbFloor.Size = new System.Drawing.Size(48, 17);
			this.rbFloor.TabIndex = 12;
			this.rbFloor.TabStop = true;
			this.rbFloor.Text = "Floor";
			this.rbFloor.UseVisualStyleBackColor = true;
			// 
			// rbCeiling
			// 
			this.rbCeiling.AutoSize = true;
			this.rbCeiling.Location = new System.Drawing.Point(197, 61);
			this.rbCeiling.Name = "rbCeiling";
			this.rbCeiling.Size = new System.Drawing.Size(56, 17);
			this.rbCeiling.TabIndex = 13;
			this.rbCeiling.Text = "Ceiling";
			this.rbCeiling.UseVisualStyleBackColor = true;
			// 
			// cbFullbright
			// 
			this.cbFullbright.AutoSize = true;
			this.cbFullbright.Checked = true;
			this.cbFullbright.CheckState = System.Windows.Forms.CheckState.Checked;
			this.cbFullbright.Location = new System.Drawing.Point(279, 40);
			this.cbFullbright.Name = "cbFullbright";
			this.cbFullbright.Size = new System.Drawing.Size(87, 17);
			this.cbFullbright.TabIndex = 14;
			this.cbFullbright.Text = "Use fullbright";
			this.cbFullbright.UseVisualStyleBackColor = true;
			// 
			// cbBrightmap
			// 
			this.cbBrightmap.AutoSize = true;
			this.cbBrightmap.Location = new System.Drawing.Point(279, 109);
			this.cbBrightmap.Name = "cbBrightmap";
			this.cbBrightmap.Size = new System.Drawing.Size(106, 17);
			this.cbBrightmap.TabIndex = 15;
			this.cbBrightmap.Text = "Create brightmap";
			this.cbBrightmap.UseVisualStyleBackColor = true;
			// 
			// cbTiles
			// 
			this.cbTiles.AutoSize = true;
			this.cbTiles.Location = new System.Drawing.Point(279, 132);
			this.cbTiles.Name = "cbTiles";
			this.cbTiles.Size = new System.Drawing.Size(110, 17);
			this.cbTiles.TabIndex = 16;
			this.cbTiles.Text = "Create 64x64 tiles";
			this.cbTiles.UseVisualStyleBackColor = true;
			// 
			// cbScale
			// 
			this.cbScale.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbScale.FormattingEnabled = true;
			this.cbScale.Items.AddRange(new object[] {
            "100%",
            "200%",
            "400%",
            "800%"});
			this.cbScale.Location = new System.Drawing.Point(102, 89);
			this.cbScale.Name = "cbScale";
			this.cbScale.Size = new System.Drawing.Size(71, 21);
			this.cbScale.TabIndex = 17;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(12, 92);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(37, 13);
			this.label4.TabIndex = 18;
			this.label4.Text = "Scale:";
			// 
			// progress
			// 
			this.progress.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.progress.Location = new System.Drawing.Point(12, 172);
			this.progress.Name = "progress";
			this.progress.Size = new System.Drawing.Size(261, 23);
			this.progress.Step = 1;
			this.progress.TabIndex = 19;
			this.progress.Visible = false;
			// 
			// lbPhase
			// 
			this.lbPhase.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.lbPhase.AutoSize = true;
			this.lbPhase.Location = new System.Drawing.Point(14, 146);
			this.lbPhase.Name = "lbPhase";
			this.lbPhase.Size = new System.Drawing.Size(45, 13);
			this.lbPhase.TabIndex = 20;
			this.lbPhase.Text = "lbPhase";
			this.lbPhase.Visible = false;
			// 
			// cbApplySectorColors
			// 
			this.cbApplySectorColors.AutoSize = true;
			this.cbApplySectorColors.Checked = true;
			this.cbApplySectorColors.CheckState = System.Windows.Forms.CheckState.Checked;
			this.cbApplySectorColors.Location = new System.Drawing.Point(279, 63);
			this.cbApplySectorColors.Name = "cbApplySectorColors";
			this.cbApplySectorColors.Size = new System.Drawing.Size(115, 17);
			this.cbApplySectorColors.TabIndex = 14;
			this.cbApplySectorColors.Text = "Apply sector colors";
			this.cbApplySectorColors.UseVisualStyleBackColor = true;
			// 
			// cbTransparency
			// 
			this.cbTransparency.AutoSize = true;
			this.cbTransparency.Location = new System.Drawing.Point(279, 86);
			this.cbTransparency.Name = "cbTransparency";
			this.cbTransparency.Size = new System.Drawing.Size(115, 17);
			this.cbTransparency.TabIndex = 21;
			this.cbTransparency.Text = "Allow transparency";
			this.toolTip.SetToolTip(this.cbTransparency, "Unoccupied parts of the images will be transparent.\r\nWorks with transparent textu" +
        "res, too.\r\n\r\nAntialiasing will be disabled.");
			this.cbTransparency.UseVisualStyleBackColor = true;
			// 
			// ImageExportSettingsForm
			// 
			this.AcceptButton = this.export;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.close;
			this.ClientSize = new System.Drawing.Size(447, 208);
			this.Controls.Add(this.cbTransparency);
			this.Controls.Add(this.lbPhase);
			this.Controls.Add(this.progress);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.cbScale);
			this.Controls.Add(this.cbTiles);
			this.Controls.Add(this.cbBrightmap);
			this.Controls.Add(this.cbApplySectorColors);
			this.Controls.Add(this.cbFullbright);
			this.Controls.Add(this.rbCeiling);
			this.Controls.Add(this.rbFloor);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.cbPixelFormat);
			this.Controls.Add(this.cbImageFormat);
			this.Controls.Add(this.close);
			this.Controls.Add(this.export);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.browse);
			this.Controls.Add(this.tbExportPath);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ImageExportSettingsForm";
			this.Text = "Image export settings";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ImageExportSettingsForm_FormClosing);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button browse;
		private System.Windows.Forms.TextBox tbExportPath;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button close;
		private System.Windows.Forms.Button export;
		private System.Windows.Forms.SaveFileDialog saveFileDialog;
		private System.Windows.Forms.ComboBox cbImageFormat;
		private System.Windows.Forms.ComboBox cbPixelFormat;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.RadioButton rbFloor;
		private System.Windows.Forms.RadioButton rbCeiling;
		private System.Windows.Forms.CheckBox cbFullbright;
		private System.Windows.Forms.CheckBox cbBrightmap;
		private System.Windows.Forms.CheckBox cbTiles;
		private System.Windows.Forms.ComboBox cbScale;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.ProgressBar progress;
		private System.Windows.Forms.Label lbPhase;
		private System.Windows.Forms.CheckBox cbApplySectorColors;
		private System.Windows.Forms.CheckBox cbTransparency;
		private System.Windows.Forms.ToolTip toolTip;
	}
}