namespace CodeImp.DoomBuilder.BuilderEffects
{
	partial class JitterSectorsForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.bCancel = new System.Windows.Forms.Button();
			this.bApply = new System.Windows.Forms.Button();
			this.bUpdateTranslation = new System.Windows.Forms.Button();
			this.bUpdateCeilingHeight = new System.Windows.Forms.Button();
			this.bUpdateFloorHeight = new System.Windows.Forms.Button();
			this.gbUpperTexture = new System.Windows.Forms.GroupBox();
			this.textureUpper = new CodeImp.DoomBuilder.Controls.TextureSelectorControl();
			this.cbPegTop = new System.Windows.Forms.CheckBox();
			this.textureLower = new CodeImp.DoomBuilder.Controls.TextureSelectorControl();
			this.cbPegBottom = new System.Windows.Forms.CheckBox();
			this.cbUpperTexStyle = new System.Windows.Forms.ComboBox();
			this.cbLowerTexStyle = new System.Windows.Forms.ComboBox();
			this.gbLowerTexture = new System.Windows.Forms.GroupBox();
			this.cbKeepExistingTextures = new System.Windows.Forms.CheckBox();
			this.positionJitterAmmount = new IntControl();
			this.floorHeightAmmount = new IntControl();
			this.ceilingHeightAmmount = new IntControl();
			this.cbUseFloorVertexHeights = new System.Windows.Forms.CheckBox();
			this.cbUseCeilingVertexHeights = new System.Windows.Forms.CheckBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label1.AutoSize = true;
			this.ceiloffsetmode = new System.Windows.Forms.ComboBox();
			this.flooroffsetmode = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label2.AutoSize = true;
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.groupBox4 = new System.Windows.Forms.GroupBox();
			this.gbUpperTexture.SuspendLayout();
			this.gbLowerTexture.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.groupBox4.SuspendLayout();
			this.SuspendLayout();
			// 
			// bCancel
			// 
			this.bCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.bCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.bCancel.Location = new System.Drawing.Point(296, 525);
			this.bCancel.Name = "bCancel";
			this.bCancel.Size = new System.Drawing.Size(75, 25);
			this.bCancel.TabIndex = 10;
			this.bCancel.Text = "Cancel";
			this.bCancel.UseVisualStyleBackColor = true;
			this.bCancel.Click += new System.EventHandler(this.bCancel_Click);
			// 
			// bApply
			// 
			this.bApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.bApply.Location = new System.Drawing.Point(195, 525);
			this.bApply.Name = "bApply";
			this.bApply.Size = new System.Drawing.Size(95, 25);
			this.bApply.TabIndex = 9;
			this.bApply.Text = "Apply";
			this.bApply.UseVisualStyleBackColor = true;
			this.bApply.Click += new System.EventHandler(this.bApply_Click);
			// 
			// bUpdateTranslation
			// 
			this.bUpdateTranslation.Image = global::CodeImp.DoomBuilder.BuilderEffects.Properties.Resources.Update;
			this.bUpdateTranslation.Location = new System.Drawing.Point(327, 19);
			this.bUpdateTranslation.Name = "bUpdateTranslation";
			this.bUpdateTranslation.Size = new System.Drawing.Size(23, 25);
			this.bUpdateTranslation.TabIndex = 11;
			this.bUpdateTranslation.UseVisualStyleBackColor = true;
			this.bUpdateTranslation.Click += new System.EventHandler(this.bUpdateTranslation_Click);
			// 
			// bUpdateCeilingHeight
			// 
			this.bUpdateCeilingHeight.Image = global::CodeImp.DoomBuilder.BuilderEffects.Properties.Resources.Update;
			this.bUpdateCeilingHeight.Location = new System.Drawing.Point(327, 19);
			this.bUpdateCeilingHeight.Name = "bUpdateCeilingHeight";
			this.bUpdateCeilingHeight.Size = new System.Drawing.Size(23, 25);
			this.bUpdateCeilingHeight.TabIndex = 15;
			this.bUpdateCeilingHeight.UseVisualStyleBackColor = true;
			this.bUpdateCeilingHeight.Click += new System.EventHandler(this.bUpdateCeilingHeight_Click);
			// 
			// bUpdateFloorHeight
			// 
			this.bUpdateFloorHeight.Image = global::CodeImp.DoomBuilder.BuilderEffects.Properties.Resources.Update;
			this.bUpdateFloorHeight.Location = new System.Drawing.Point(327, 19);
			this.bUpdateFloorHeight.Name = "bUpdateFloorHeight";
			this.bUpdateFloorHeight.Size = new System.Drawing.Size(23, 25);
			this.bUpdateFloorHeight.TabIndex = 16;
			this.bUpdateFloorHeight.UseVisualStyleBackColor = true;
			this.bUpdateFloorHeight.Click += new System.EventHandler(this.bUpdateFloorHeight_Click);
			// 
			// gbUpperTexture
			// 
			this.gbUpperTexture.Controls.Add(this.textureUpper);
			this.gbUpperTexture.Controls.Add(this.cbPegTop);
			this.gbUpperTexture.Location = new System.Drawing.Point(6, 71);
			this.gbUpperTexture.Name = "gbUpperTexture";
			this.gbUpperTexture.Size = new System.Drawing.Size(171, 159);
			this.gbUpperTexture.TabIndex = 24;
			this.gbUpperTexture.TabStop = false;
			this.gbUpperTexture.Text = "Upper Texture:";
			// 
			// textureUpper
			// 
			this.textureUpper.Location = new System.Drawing.Point(47, 41);
			this.textureUpper.MultipleTextures = false;
			this.textureUpper.Name = "textureUpper";
			this.textureUpper.Required = false;
			this.textureUpper.Size = new System.Drawing.Size(83, 112);
			this.textureUpper.TabIndex = 2;
			this.textureUpper.TextureName = "";
			this.textureUpper.OnValueChanged += new System.EventHandler(this.textureUpper_OnValueChanged);
			// 
			// cbPegTop
			// 
			this.cbPegTop.AutoSize = true;
			this.cbPegTop.Location = new System.Drawing.Point(6, 19);
			this.cbPegTop.Name = "cbPegTop";
			this.cbPegTop.Size = new System.Drawing.Size(108, 17);
			this.cbPegTop.TabIndex = 26;
			this.cbPegTop.Text = "Upper Unpegged";
			this.cbPegTop.UseVisualStyleBackColor = true;
			this.cbPegTop.CheckedChanged += new System.EventHandler(this.cbPegTop_CheckedChanged);
			// 
			// textureLower
			// 
			this.textureLower.Location = new System.Drawing.Point(47, 41);
			this.textureLower.MultipleTextures = false;
			this.textureLower.Name = "textureLower";
			this.textureLower.Required = false;
			this.textureLower.Size = new System.Drawing.Size(83, 112);
			this.textureLower.TabIndex = 4;
			this.textureLower.TextureName = "";
			this.textureLower.OnValueChanged += new System.EventHandler(this.textureLower_OnValueChanged);
			// 
			// cbPegBottom
			// 
			this.cbPegBottom.AutoSize = true;
			this.cbPegBottom.Location = new System.Drawing.Point(6, 19);
			this.cbPegBottom.Name = "cbPegBottom";
			this.cbPegBottom.Size = new System.Drawing.Size(108, 17);
			this.cbPegBottom.TabIndex = 28;
			this.cbPegBottom.Text = "Lower Unpegged";
			this.cbPegBottom.UseVisualStyleBackColor = true;
			this.cbPegBottom.CheckedChanged += new System.EventHandler(this.cbPegBottom_CheckedChanged);
			// 
			// cbUpperTexStyle
			// 
			this.cbUpperTexStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbUpperTexStyle.FormattingEnabled = true;
			this.cbUpperTexStyle.Items.AddRange(new object[] {
            "Don\'t change upper texture",
            "Use ceiling texture",
            "Pick upper texture"});
			this.cbUpperTexStyle.Location = new System.Drawing.Point(6, 43);
			this.cbUpperTexStyle.Name = "cbUpperTexStyle";
			this.cbUpperTexStyle.Size = new System.Drawing.Size(171, 21);
			this.cbUpperTexStyle.TabIndex = 26;
			this.cbUpperTexStyle.SelectedIndexChanged += new System.EventHandler(this.cbUpperTexStyle_SelectedIndexChanged);
			// 
			// cbLowerTexStyle
			// 
			this.cbLowerTexStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbLowerTexStyle.FormattingEnabled = true;
			this.cbLowerTexStyle.Items.AddRange(new object[] {
            "Don\'t change lower texture",
            "Use floor texture",
            "Pick lower texture"});
			this.cbLowerTexStyle.Location = new System.Drawing.Point(181, 43);
			this.cbLowerTexStyle.Name = "cbLowerTexStyle";
			this.cbLowerTexStyle.Size = new System.Drawing.Size(171, 21);
			this.cbLowerTexStyle.TabIndex = 27;
			this.cbLowerTexStyle.SelectedIndexChanged += new System.EventHandler(this.cbLowerTexStyle_SelectedIndexChanged);
			// 
			// gbLowerTexture
			// 
			this.gbLowerTexture.Controls.Add(this.textureLower);
			this.gbLowerTexture.Controls.Add(this.cbPegBottom);
			this.gbLowerTexture.Location = new System.Drawing.Point(181, 71);
			this.gbLowerTexture.Name = "gbLowerTexture";
			this.gbLowerTexture.Size = new System.Drawing.Size(171, 159);
			this.gbLowerTexture.TabIndex = 29;
			this.gbLowerTexture.TabStop = false;
			this.gbLowerTexture.Text = "Lower texture:";
			// 
			// cbKeepExistingTextures
			// 
			this.cbKeepExistingTextures.AutoSize = true;
			this.cbKeepExistingTextures.Location = new System.Drawing.Point(8, 19);
			this.cbKeepExistingTextures.Name = "cbKeepExistingTextures";
			this.cbKeepExistingTextures.Size = new System.Drawing.Size(205, 17);
			this.cbKeepExistingTextures.TabIndex = 30;
			this.cbKeepExistingTextures.Text = "Don\'t change existing sidedef textures";
			this.cbKeepExistingTextures.UseVisualStyleBackColor = true;
			this.cbKeepExistingTextures.CheckedChanged += new System.EventHandler(this.cbKeepExistingTextures_CheckedChanged);
			// 
			// positionJitterAmmount
			// 
			this.positionJitterAmmount.AllowNegative = false;
			this.positionJitterAmmount.ExtendedLimits = true;
			this.positionJitterAmmount.Label = "Position:";
			this.positionJitterAmmount.Location = new System.Drawing.Point(6, 19);
			this.positionJitterAmmount.Maximum = 100;
			this.positionJitterAmmount.Minimum = 0;
			this.positionJitterAmmount.Name = "positionJitterAmmount";
			this.positionJitterAmmount.Size = new System.Drawing.Size(315, 22);
			this.positionJitterAmmount.TabIndex = 18;
			this.positionJitterAmmount.Value = 0;
			this.positionJitterAmmount.OnValueChanging += new System.EventHandler(this.positionJitterAmmount_OnValueChanging);
			// 
			// floorHeightAmmount
			// 
			this.floorHeightAmmount.AllowNegative = false;
			this.floorHeightAmmount.ExtendedLimits = false;
			this.floorHeightAmmount.Label = "Height:";
			this.floorHeightAmmount.Location = new System.Drawing.Point(6, 19);
			this.floorHeightAmmount.Maximum = 100;
			this.floorHeightAmmount.Minimum = 0;
			this.floorHeightAmmount.Name = "floorHeightAmmount";
			this.floorHeightAmmount.Size = new System.Drawing.Size(315, 22);
			this.floorHeightAmmount.TabIndex = 20;
			this.floorHeightAmmount.Value = 0;
			this.floorHeightAmmount.OnValueChanging += new System.EventHandler(this.floorHeightAmmount_OnValueChanging);
			// 
			// ceilingHeightAmmount
			// 
			this.ceilingHeightAmmount.AllowNegative = false;
			this.ceilingHeightAmmount.ExtendedLimits = false;
			this.ceilingHeightAmmount.Label = "Height:";
			this.ceilingHeightAmmount.Location = new System.Drawing.Point(6, 19);
			this.ceilingHeightAmmount.Maximum = 100;
			this.ceilingHeightAmmount.Minimum = 0;
			this.ceilingHeightAmmount.Name = "ceilingHeightAmmount";
			this.ceilingHeightAmmount.Size = new System.Drawing.Size(315, 22);
			this.ceilingHeightAmmount.TabIndex = 19;
			this.ceilingHeightAmmount.Value = 0;
			this.ceilingHeightAmmount.OnValueChanging += new System.EventHandler(this.ceilingHeightAmmount_OnValueChanging);
			// 
			// cbUseFloorVertexHeights
			// 
			this.cbUseFloorVertexHeights.AutoSize = true;
			this.cbUseFloorVertexHeights.Location = new System.Drawing.Point(102, 75);
			this.cbUseFloorVertexHeights.Name = "cbUseFloorVertexHeights";
			this.cbUseFloorVertexHeights.Size = new System.Drawing.Size(114, 17);
			this.cbUseFloorVertexHeights.TabIndex = 31;
			this.cbUseFloorVertexHeights.Text = "Use vertex heights";
			this.cbUseFloorVertexHeights.UseVisualStyleBackColor = true;
			this.cbUseFloorVertexHeights.CheckedChanged += new System.EventHandler(this.cbUseFloorVertexHeights_CheckedChanged);
			// 
			// cbUseCeilingVertexHeights
			// 
			this.cbUseCeilingVertexHeights.AutoSize = true;
			this.cbUseCeilingVertexHeights.Location = new System.Drawing.Point(102, 75);
			this.cbUseCeilingVertexHeights.Name = "cbUseCeilingVertexHeights";
			this.cbUseCeilingVertexHeights.Size = new System.Drawing.Size(114, 17);
			this.cbUseCeilingVertexHeights.TabIndex = 32;
			this.cbUseCeilingVertexHeights.Text = "Use vertex heights";
			this.cbUseCeilingVertexHeights.UseVisualStyleBackColor = true;
			this.cbUseCeilingVertexHeights.CheckedChanged += new System.EventHandler(this.cbUseCeilingVertexHeights_CheckedChanged);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(26, 51);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(67, 13);
			this.label1.TabIndex = 33;
			this.label1.Text = "Offset mode:";
			// 
			// ceiloffsetmode
			// 
			this.ceiloffsetmode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ceiloffsetmode.FormattingEnabled = true;
			this.ceiloffsetmode.Items.AddRange(new object[] {
            "Raise and lower",
            "Lower only",
            "Raise only"});
			this.ceiloffsetmode.Location = new System.Drawing.Point(102, 47);
			this.ceiloffsetmode.Name = "ceiloffsetmode";
			this.ceiloffsetmode.Size = new System.Drawing.Size(112, 21);
			this.ceiloffsetmode.TabIndex = 34;
			this.ceiloffsetmode.SelectedIndexChanged += new System.EventHandler(this.ceiloffsetmode_SelectedIndexChanged);
			// 
			// flooroffsetmode
			// 
			this.flooroffsetmode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.flooroffsetmode.FormattingEnabled = true;
			this.flooroffsetmode.Items.AddRange(new object[] {
            "Raise and lower",
            "Raise only",
            "Lower only"});
			this.flooroffsetmode.Location = new System.Drawing.Point(102, 47);
			this.flooroffsetmode.Name = "flooroffsetmode";
			this.flooroffsetmode.Size = new System.Drawing.Size(112, 21);
			this.flooroffsetmode.TabIndex = 36;
			this.flooroffsetmode.SelectedIndexChanged += new System.EventHandler(this.flooroffsetmode_SelectedIndexChanged);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(26, 51);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(67, 13);
			this.label2.TabIndex = 35;
			this.label2.Text = "Offset mode:";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.positionJitterAmmount);
			this.groupBox1.Controls.Add(this.bUpdateTranslation);
			this.groupBox1.Location = new System.Drawing.Point(12, 12);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(358, 54);
			this.groupBox1.TabIndex = 37;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Position:";
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.ceilingHeightAmmount);
			this.groupBox2.Controls.Add(this.bUpdateCeilingHeight);
			this.groupBox2.Controls.Add(this.ceiloffsetmode);
			this.groupBox2.Controls.Add(this.label1);
			this.groupBox2.Controls.Add(this.cbUseCeilingVertexHeights);
			this.groupBox2.Location = new System.Drawing.Point(12, 72);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(358, 100);
			this.groupBox2.TabIndex = 38;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Ceiling:";
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.floorHeightAmmount);
			this.groupBox3.Controls.Add(this.bUpdateFloorHeight);
			this.groupBox3.Controls.Add(this.flooroffsetmode);
			this.groupBox3.Controls.Add(this.cbUseFloorVertexHeights);
			this.groupBox3.Controls.Add(this.label2);
			this.groupBox3.Location = new System.Drawing.Point(12, 178);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(358, 100);
			this.groupBox3.TabIndex = 35;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Floor:";
			// 
			// groupBox4
			// 
			this.groupBox4.Controls.Add(this.cbKeepExistingTextures);
			this.groupBox4.Controls.Add(this.cbUpperTexStyle);
			this.groupBox4.Controls.Add(this.cbLowerTexStyle);
			this.groupBox4.Controls.Add(this.gbUpperTexture);
			this.groupBox4.Controls.Add(this.gbLowerTexture);
			this.groupBox4.Location = new System.Drawing.Point(12, 284);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Size = new System.Drawing.Size(358, 236);
			this.groupBox4.TabIndex = 39;
			this.groupBox4.TabStop = false;
			this.groupBox4.Text = "Textures:";
			// 
			// JitterSectorsForm
			// 
			this.AcceptButton = this.bApply;
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.CancelButton = this.bCancel;
			this.ClientSize = new System.Drawing.Size(382, 553);
			this.Controls.Add(this.groupBox4);
			this.Controls.Add(this.groupBox3);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.bCancel);
			this.Controls.Add(this.bApply);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "JitterSectorsForm";
			this.Opacity = 0;
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "Jitter Settings";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.JitterSectorsForm_FormClosing);
			this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.JitterSectorsForm_HelpRequested);
			this.gbUpperTexture.ResumeLayout(false);
			this.gbUpperTexture.PerformLayout();
			this.gbLowerTexture.ResumeLayout(false);
			this.gbLowerTexture.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			this.groupBox4.ResumeLayout(false);
			this.groupBox4.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button bUpdateTranslation;
		private System.Windows.Forms.Button bCancel;
		private System.Windows.Forms.Button bApply;
		private System.Windows.Forms.Button bUpdateCeilingHeight;
		private System.Windows.Forms.Button bUpdateFloorHeight;
		private IntControl positionJitterAmmount;
		private IntControl ceilingHeightAmmount;
		private IntControl floorHeightAmmount;
		private System.Windows.Forms.GroupBox gbUpperTexture;
		private CodeImp.DoomBuilder.Controls.TextureSelectorControl textureLower;
		private CodeImp.DoomBuilder.Controls.TextureSelectorControl textureUpper;
		private System.Windows.Forms.CheckBox cbPegBottom;
		private System.Windows.Forms.CheckBox cbPegTop;
		private System.Windows.Forms.ComboBox cbUpperTexStyle;
		private System.Windows.Forms.ComboBox cbLowerTexStyle;
		private System.Windows.Forms.GroupBox gbLowerTexture;
		private System.Windows.Forms.CheckBox cbKeepExistingTextures;
		private System.Windows.Forms.CheckBox cbUseFloorVertexHeights;
		private System.Windows.Forms.CheckBox cbUseCeilingVertexHeights;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox ceiloffsetmode;
		private System.Windows.Forms.ComboBox flooroffsetmode;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.GroupBox groupBox4;
	}
}
