namespace CodeImp.DoomBuilder.ThreeDFloorMode
{
	partial class ThreeDFloorHelperControl
	{
		/// <summary> 
		/// Erforderliche Designervariable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Verwendete Ressourcen bereinigen.
		/// </summary>
		/// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Vom Komponenten-Designer generierter Code

		/// <summary> 
		/// Erforderliche Methode für die Designerunterstützung. 
		/// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
		/// </summary>
		private void InitializeComponent()
		{
			this.sectorTopFlat = new CodeImp.DoomBuilder.Controls.FlatSelectorControl();
			this.sectorBorderTexture = new CodeImp.DoomBuilder.Controls.TextureSelectorControl();
			this.sectorBottomFlat = new CodeImp.DoomBuilder.Controls.FlatSelectorControl();
			this.sectorCeilingHeight = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.sectorFloorHeight = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.label1 = new System.Windows.Forms.Label();
			this.label1.AutoSize = true;
			this.label2 = new System.Windows.Forms.Label();
			this.label2.AutoSize = true;
			this.label3 = new System.Windows.Forms.Label();
			this.label3.AutoSize = true;
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.checkedListBoxSectors = new System.Windows.Forms.CheckedListBox();
			this.buttonDuplicate = new System.Windows.Forms.Button();
			this.buttonSplit = new System.Windows.Forms.Button();
			this.buttonCheckAll = new System.Windows.Forms.Button();
			this.buttonUncheckAll = new System.Windows.Forms.Button();
			this.borderHeightLabel = new System.Windows.Forms.Label();
			this.borderHeightLabel.AutoSize = true;
			this.typeArgument = new CodeImp.DoomBuilder.Controls.ArgumentBox();
			this.flagsArgument = new CodeImp.DoomBuilder.Controls.ArgumentBox();
			this.buttonEditSector = new System.Windows.Forms.Button();
			this.label6 = new System.Windows.Forms.Label();
			this.label6.AutoSize = true;
			this.alphaArgument = new CodeImp.DoomBuilder.Controls.ArgumentBox();
			this.label4 = new System.Windows.Forms.Label();
			this.label4.AutoSize = true;
			this.label5 = new System.Windows.Forms.Label();
			this.label5.AutoSize = true;
			this.label7 = new System.Windows.Forms.Label();
			this.label7.AutoSize = true;
			this.label8 = new System.Windows.Forms.Label();
			this.label8.AutoSize = true;
			this.sectorBrightness = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.tagsLabel = new System.Windows.Forms.Label();
			this.tagsLabel.AutoSize = true;
			this.buttonDetach = new System.Windows.Forms.Button();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// sectorTopFlat
			// 
			this.sectorTopFlat.Location = new System.Drawing.Point(10, 30);
			this.sectorTopFlat.MultipleTextures = false;
			this.sectorTopFlat.Name = "sectorTopFlat";
			this.sectorTopFlat.Size = new System.Drawing.Size(115, 136);
			this.sectorTopFlat.TabIndex = 2;
			this.sectorTopFlat.TextureName = "";
			// 
			// sectorBorderTexture
			// 
			this.sectorBorderTexture.Location = new System.Drawing.Point(131, 30);
			this.sectorBorderTexture.MultipleTextures = false;
			this.sectorBorderTexture.Name = "sectorBorderTexture";
			this.sectorBorderTexture.Required = false;
			this.sectorBorderTexture.Size = new System.Drawing.Size(115, 136);
			this.sectorBorderTexture.TabIndex = 3;
			this.sectorBorderTexture.TextureName = "";
			// 
			// sectorBottomFlat
			// 
			this.sectorBottomFlat.Location = new System.Drawing.Point(252, 30);
			this.sectorBottomFlat.MultipleTextures = false;
			this.sectorBottomFlat.Name = "sectorBottomFlat";
			this.sectorBottomFlat.Size = new System.Drawing.Size(115, 136);
			this.sectorBottomFlat.TabIndex = 4;
			this.sectorBottomFlat.TextureName = "";
			// 
			// sectorCeilingHeight
			// 
			this.sectorCeilingHeight.AllowDecimal = false;
			this.sectorCeilingHeight.AllowExpressions = false;
			this.sectorCeilingHeight.AllowNegative = true;
			this.sectorCeilingHeight.AllowRelative = true;
			this.sectorCeilingHeight.BackColor = System.Drawing.Color.Transparent;
			this.sectorCeilingHeight.ButtonStep = 8;
			this.sectorCeilingHeight.ButtonStepBig = 10F;
			this.sectorCeilingHeight.ButtonStepFloat = 1F;
			this.sectorCeilingHeight.ButtonStepSmall = 0.1F;
			this.sectorCeilingHeight.ButtonStepsUseModifierKeys = false;
			this.sectorCeilingHeight.ButtonStepsWrapAround = false;
			this.sectorCeilingHeight.Location = new System.Drawing.Point(55, 0);
			this.sectorCeilingHeight.Name = "sectorCeilingHeight";
			this.sectorCeilingHeight.Size = new System.Drawing.Size(70, 24);
			this.sectorCeilingHeight.StepValues = null;
			this.sectorCeilingHeight.TabIndex = 0;
			this.sectorCeilingHeight.WhenTextChanged += new System.EventHandler(this.RecomputeBorderHeight);
			// 
			// sectorFloorHeight
			// 
			this.sectorFloorHeight.AllowDecimal = false;
			this.sectorFloorHeight.AllowExpressions = false;
			this.sectorFloorHeight.AllowNegative = true;
			this.sectorFloorHeight.AllowRelative = true;
			this.sectorFloorHeight.BackColor = System.Drawing.Color.Transparent;
			this.sectorFloorHeight.ButtonStep = 8;
			this.sectorFloorHeight.ButtonStepBig = 10F;
			this.sectorFloorHeight.ButtonStepFloat = 1F;
			this.sectorFloorHeight.ButtonStepSmall = 0.1F;
			this.sectorFloorHeight.ButtonStepsUseModifierKeys = false;
			this.sectorFloorHeight.ButtonStepsWrapAround = false;
			this.sectorFloorHeight.Location = new System.Drawing.Point(297, 0);
			this.sectorFloorHeight.Name = "sectorFloorHeight";
			this.sectorFloorHeight.Size = new System.Drawing.Size(70, 24);
			this.sectorFloorHeight.StepValues = null;
			this.sectorFloorHeight.TabIndex = 1;
			this.sectorFloorHeight.WhenTextChanged += new System.EventHandler(this.RecomputeBorderHeight);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.BackColor = System.Drawing.Color.Transparent;
			this.label1.Location = new System.Drawing.Point(7, 5);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(26, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Top";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.BackColor = System.Drawing.Color.Transparent;
			this.label2.Location = new System.Drawing.Point(251, 5);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(40, 13);
			this.label2.TabIndex = 6;
			this.label2.Text = "Bottom";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.BackColor = System.Drawing.Color.Transparent;
			this.label3.Location = new System.Drawing.Point(131, 5);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(38, 13);
			this.label3.TabIndex = 7;
			this.label3.Text = "Border";
			// 
			// groupBox2
			// 
			this.groupBox2.BackColor = System.Drawing.Color.Transparent;
			this.groupBox2.Controls.Add(this.checkedListBoxSectors);
			this.groupBox2.Location = new System.Drawing.Point(503, 0);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(124, 166);
			this.groupBox2.TabIndex = 10;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Sectors";
			// 
			// checkedListBoxSectors
			// 
			this.checkedListBoxSectors.CheckOnClick = true;
			this.checkedListBoxSectors.FormattingEnabled = true;
			this.checkedListBoxSectors.Location = new System.Drawing.Point(6, 19);
			this.checkedListBoxSectors.Name = "checkedListBoxSectors";
			this.checkedListBoxSectors.Size = new System.Drawing.Size(110, 139);
			this.checkedListBoxSectors.TabIndex = 0;
			this.checkedListBoxSectors.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.checkedListBoxSectors_ItemCheck);
			// 
			// buttonDuplicate
			// 
			this.buttonDuplicate.Location = new System.Drawing.Point(633, 5);
			this.buttonDuplicate.Name = "buttonDuplicate";
			this.buttonDuplicate.Size = new System.Drawing.Size(75, 23);
			this.buttonDuplicate.TabIndex = 11;
			this.buttonDuplicate.Text = "Duplicate";
			this.buttonDuplicate.UseVisualStyleBackColor = true;
			this.buttonDuplicate.Click += new System.EventHandler(this.buttonDuplicate_Click);
			// 
			// buttonSplit
			// 
			this.buttonSplit.Location = new System.Drawing.Point(633, 34);
			this.buttonSplit.Name = "buttonSplit";
			this.buttonSplit.Size = new System.Drawing.Size(75, 23);
			this.buttonSplit.TabIndex = 12;
			this.buttonSplit.Text = "Split";
			this.buttonSplit.UseVisualStyleBackColor = true;
			this.buttonSplit.Click += new System.EventHandler(this.buttonSplit_Click);
			// 
			// buttonCheckAll
			// 
			this.buttonCheckAll.Location = new System.Drawing.Point(633, 92);
			this.buttonCheckAll.Name = "buttonCheckAll";
			this.buttonCheckAll.Size = new System.Drawing.Size(75, 23);
			this.buttonCheckAll.TabIndex = 14;
			this.buttonCheckAll.Text = "Check all";
			this.buttonCheckAll.UseVisualStyleBackColor = true;
			this.buttonCheckAll.Click += new System.EventHandler(this.buttonCheckAll_Click);
			// 
			// buttonUncheckAll
			// 
			this.buttonUncheckAll.Location = new System.Drawing.Point(633, 121);
			this.buttonUncheckAll.Name = "buttonUncheckAll";
			this.buttonUncheckAll.Size = new System.Drawing.Size(75, 23);
			this.buttonUncheckAll.TabIndex = 15;
			this.buttonUncheckAll.Text = "Uncheck all";
			this.buttonUncheckAll.UseVisualStyleBackColor = true;
			this.buttonUncheckAll.Click += new System.EventHandler(this.buttonUncheckAll_Click);
			// 
			// borderHeightLabel
			// 
			this.borderHeightLabel.Location = new System.Drawing.Point(196, 5);
			this.borderHeightLabel.Name = "borderHeightLabel";
			this.borderHeightLabel.Size = new System.Drawing.Size(50, 13);
			this.borderHeightLabel.TabIndex = 17;
			this.borderHeightLabel.Text = "0";
			this.borderHeightLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// typeArgument
			// 
			this.typeArgument.BackColor = System.Drawing.Color.Transparent;
			this.typeArgument.Location = new System.Drawing.Point(438, 0);
			this.typeArgument.Name = "typeArgument";
			this.typeArgument.Size = new System.Drawing.Size(59, 24);
			this.typeArgument.TabIndex = 5;
			// 
			// flagsArgument
			// 
			this.flagsArgument.BackColor = System.Drawing.Color.Transparent;
			this.flagsArgument.Location = new System.Drawing.Point(438, 30);
			this.flagsArgument.Name = "flagsArgument";
			this.flagsArgument.Size = new System.Drawing.Size(59, 24);
			this.flagsArgument.TabIndex = 6;
			// 
			// buttonEditSector
			// 
			this.buttonEditSector.Location = new System.Drawing.Point(376, 143);
			this.buttonEditSector.Name = "buttonEditSector";
			this.buttonEditSector.Size = new System.Drawing.Size(121, 23);
			this.buttonEditSector.TabIndex = 9;
			this.buttonEditSector.Text = "Edit control sector";
			this.buttonEditSector.UseVisualStyleBackColor = true;
			this.buttonEditSector.Click += new System.EventHandler(this.buttonEditSector_Click);
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.BackColor = System.Drawing.Color.Transparent;
			this.label6.Location = new System.Drawing.Point(395, 65);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(37, 13);
			this.label6.TabIndex = 23;
			this.label6.Text = "Alpha:";
			// 
			// alphaArgument
			// 
			this.alphaArgument.BackColor = System.Drawing.Color.Transparent;
			this.alphaArgument.Location = new System.Drawing.Point(438, 60);
			this.alphaArgument.Name = "alphaArgument";
			this.alphaArgument.Size = new System.Drawing.Size(59, 24);
			this.alphaArgument.TabIndex = 7;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.BackColor = System.Drawing.Color.Transparent;
			this.label4.Location = new System.Drawing.Point(396, 5);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(34, 13);
			this.label4.TabIndex = 21;
			this.label4.Text = "Type:";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.BackColor = System.Drawing.Color.Transparent;
			this.label5.Location = new System.Drawing.Point(395, 35);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(35, 13);
			this.label5.TabIndex = 22;
			this.label5.Text = "Flags:";
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.BackColor = System.Drawing.Color.Transparent;
			this.label7.Location = new System.Drawing.Point(373, 95);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(59, 13);
			this.label7.TabIndex = 25;
			this.label7.Text = "Brightness:";
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.BackColor = System.Drawing.Color.Transparent;
			this.label8.Location = new System.Drawing.Point(390, 125);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(40, 13);
			this.label8.TabIndex = 26;
			this.label8.Text = "Tag(s):";
			// 
			// sectorBrightness
			// 
			this.sectorBrightness.AllowDecimal = false;
			this.sectorBrightness.AllowExpressions = false;
			this.sectorBrightness.AllowNegative = true;
			this.sectorBrightness.AllowRelative = true;
			this.sectorBrightness.BackColor = System.Drawing.Color.Transparent;
			this.sectorBrightness.ButtonStep = 8;
			this.sectorBrightness.ButtonStepBig = 10F;
			this.sectorBrightness.ButtonStepFloat = 1F;
			this.sectorBrightness.ButtonStepSmall = 0.1F;
			this.sectorBrightness.ButtonStepsUseModifierKeys = false;
			this.sectorBrightness.ButtonStepsWrapAround = false;
			this.sectorBrightness.Location = new System.Drawing.Point(438, 90);
			this.sectorBrightness.Name = "sectorBrightness";
			this.sectorBrightness.Size = new System.Drawing.Size(59, 24);
			this.sectorBrightness.StepValues = null;
			this.sectorBrightness.TabIndex = 8;
			// 
			// tagsLabel
			// 
			this.tagsLabel.AutoSize = true;
			this.tagsLabel.Location = new System.Drawing.Point(436, 125);
			this.tagsLabel.Name = "tagsLabel";
			this.tagsLabel.Size = new System.Drawing.Size(35, 13);
			this.tagsLabel.TabIndex = 28;
			this.tagsLabel.Text = "label9";
			// 
			// buttonDetach
			// 
			this.buttonDetach.Location = new System.Drawing.Point(633, 63);
			this.buttonDetach.Name = "buttonDetach";
			this.buttonDetach.Size = new System.Drawing.Size(75, 23);
			this.buttonDetach.TabIndex = 13;
			this.buttonDetach.Text = "Detach";
			this.buttonDetach.UseVisualStyleBackColor = true;
			this.buttonDetach.Click += new System.EventHandler(this.buttonDetach_Click);
			// 
			// ThreeDFloorHelperControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.buttonDetach);
			this.Controls.Add(this.tagsLabel);
			this.Controls.Add(this.sectorBrightness);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.typeArgument);
			this.Controls.Add(this.flagsArgument);
			this.Controls.Add(this.buttonEditSector);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.alphaArgument);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.borderHeightLabel);
			this.Controls.Add(this.buttonUncheckAll);
			this.Controls.Add(this.buttonCheckAll);
			this.Controls.Add(this.buttonSplit);
			this.Controls.Add(this.buttonDuplicate);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.sectorFloorHeight);
			this.Controls.Add(this.sectorCeilingHeight);
			this.Controls.Add(this.sectorBottomFlat);
			this.Controls.Add(this.sectorBorderTexture);
			this.Controls.Add(this.sectorTopFlat);
			this.Margin = new System.Windows.Forms.Padding(3, 3, 3, 10);
			this.Name = "ThreeDFloorHelperControl";
			this.Size = new System.Drawing.Size(714, 172);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.ThreeDFloorHelperControl_Paint);
			this.groupBox2.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		public CodeImp.DoomBuilder.Controls.TextureSelectorControl sectorBorderTexture;
		public CodeImp.DoomBuilder.Controls.FlatSelectorControl sectorTopFlat;
		public CodeImp.DoomBuilder.Controls.FlatSelectorControl sectorBottomFlat;
		public CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox sectorCeilingHeight;
		public CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox sectorFloorHeight;
		private System.Windows.Forms.GroupBox groupBox2;
		public System.Windows.Forms.CheckedListBox checkedListBoxSectors;
		private System.Windows.Forms.Button buttonDuplicate;
		private System.Windows.Forms.Button buttonSplit;
		private System.Windows.Forms.Button buttonCheckAll;
		private System.Windows.Forms.Button buttonUncheckAll;
		private System.Windows.Forms.Label borderHeightLabel;
		public Controls.ArgumentBox typeArgument;
		public Controls.ArgumentBox flagsArgument;
		private System.Windows.Forms.Button buttonEditSector;
		private System.Windows.Forms.Label label6;
		public Controls.ArgumentBox alphaArgument;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label8;
		public Controls.ButtonsNumericTextbox sectorBrightness;
		private System.Windows.Forms.Label tagsLabel;
		private System.Windows.Forms.Button buttonDetach;
	}
}
