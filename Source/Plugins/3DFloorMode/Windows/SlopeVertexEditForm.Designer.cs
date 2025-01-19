namespace CodeImp.DoomBuilder.ThreeDFloorMode
{
	partial class SlopeVertexEditForm
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
			this.apply = new System.Windows.Forms.Button();
			this.cancel = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.label3 = new System.Windows.Forms.Label();
			this.label3.AutoSize = true;
			this.label2 = new System.Windows.Forms.Label();
			this.label2.AutoSize = true;
			this.label1 = new System.Windows.Forms.Label();
			this.label1.AutoSize = true;
			this.positionz = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.positiony = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.positionx = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.checkedListBoxSectors = new System.Windows.Forms.CheckedListBox();
			this.reposition = new System.Windows.Forms.CheckBox();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.removeselectedsectorsceiling = new System.Windows.Forms.CheckBox();
			this.addselectedsectorsceiling = new System.Windows.Forms.CheckBox();
			this.groupBox4 = new System.Windows.Forms.GroupBox();
			this.removeselectedsectorsfloor = new System.Windows.Forms.CheckBox();
			this.addselectedsectorsfloor = new System.Windows.Forms.CheckBox();
			this.spline = new System.Windows.Forms.CheckBox();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.groupBox4.SuspendLayout();
			this.SuspendLayout();
			// 
			// apply
			// 
			this.apply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.apply.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.apply.Location = new System.Drawing.Point(260, 174);
			this.apply.Name = "apply";
			this.apply.Size = new System.Drawing.Size(112, 25);
			this.apply.TabIndex = 0;
			this.apply.Text = "OK";
			this.apply.UseVisualStyleBackColor = true;
			this.apply.Click += new System.EventHandler(this.apply_Click);
			// 
			// cancel
			// 
			this.cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancel.Location = new System.Drawing.Point(380, 174);
			this.cancel.Name = "cancel";
			this.cancel.Size = new System.Drawing.Size(112, 25);
			this.cancel.TabIndex = 1;
			this.cancel.Text = "Cancel";
			this.cancel.UseVisualStyleBackColor = true;
			this.cancel.Click += new System.EventHandler(this.cancel_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Controls.Add(this.positionz);
			this.groupBox1.Controls.Add(this.positiony);
			this.groupBox1.Controls.Add(this.positionx);
			this.groupBox1.Location = new System.Drawing.Point(18, 13);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(149, 114);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Position";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label3.Location = new System.Drawing.Point(6, 87);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(17, 14);
			this.label3.TabIndex = 15;
			this.label3.Text = "Z:";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label2.Location = new System.Drawing.Point(6, 57);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(17, 14);
			this.label2.TabIndex = 14;
			this.label2.Text = "Y:";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(6, 25);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(17, 14);
			this.label1.TabIndex = 13;
			this.label1.Text = "X:";
			// 
			// positionz
			// 
			this.positionz.AllowDecimal = true;
			this.positionz.AllowExpressions = false;
			this.positionz.AllowNegative = true;
			this.positionz.AllowRelative = true;
			this.positionz.BackColor = System.Drawing.Color.Transparent;
			this.positionz.ButtonStep = 8;
			this.positionz.ButtonStepBig = 16F;
			this.positionz.ButtonStepFloat = 8F;
			this.positionz.ButtonStepSmall = 1F;
			this.positionz.ButtonStepsUseModifierKeys = true;
			this.positionz.ButtonStepsWrapAround = false;
			this.positionz.Location = new System.Drawing.Point(29, 82);
			this.positionz.Name = "positionz";
			this.positionz.Size = new System.Drawing.Size(115, 24);
			this.positionz.StepValues = null;
			this.positionz.TabIndex = 0;
			// 
			// positiony
			// 
			this.positiony.AllowDecimal = true;
			this.positiony.AllowExpressions = false;
			this.positiony.AllowNegative = true;
			this.positiony.AllowRelative = true;
			this.positiony.BackColor = System.Drawing.Color.Transparent;
			this.positiony.ButtonStep = 8;
			this.positiony.ButtonStepBig = 16F;
			this.positiony.ButtonStepFloat = 8F;
			this.positiony.ButtonStepSmall = 1F;
			this.positiony.ButtonStepsUseModifierKeys = true;
			this.positiony.ButtonStepsWrapAround = false;
			this.positiony.Location = new System.Drawing.Point(29, 52);
			this.positiony.Name = "positiony";
			this.positiony.Size = new System.Drawing.Size(115, 24);
			this.positiony.StepValues = null;
			this.positiony.TabIndex = 3;
			// 
			// positionx
			// 
			this.positionx.AllowDecimal = true;
			this.positionx.AllowExpressions = false;
			this.positionx.AllowNegative = true;
			this.positionx.AllowRelative = true;
			this.positionx.BackColor = System.Drawing.Color.Transparent;
			this.positionx.ButtonStep = 8;
			this.positionx.ButtonStepBig = 16F;
			this.positionx.ButtonStepFloat = 8F;
			this.positionx.ButtonStepSmall = 1F;
			this.positionx.ButtonStepsUseModifierKeys = true;
			this.positionx.ButtonStepsWrapAround = false;
			this.positionx.Location = new System.Drawing.Point(29, 20);
			this.positionx.Name = "positionx";
			this.positionx.Size = new System.Drawing.Size(115, 24);
			this.positionx.StepValues = null;
			this.positionx.TabIndex = 2;
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.checkedListBoxSectors);
			this.groupBox2.Location = new System.Drawing.Point(174, 13);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(113, 149);
			this.groupBox2.TabIndex = 2;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Sectors to remove";
			// 
			// checkedListBoxSectors
			// 
			this.checkedListBoxSectors.CheckOnClick = true;
			this.checkedListBoxSectors.FormattingEnabled = true;
			this.checkedListBoxSectors.Location = new System.Drawing.Point(6, 19);
			this.checkedListBoxSectors.Name = "checkedListBoxSectors";
			this.checkedListBoxSectors.Size = new System.Drawing.Size(100, 124);
			this.checkedListBoxSectors.TabIndex = 1;
			// 
			// reposition
			// 
			this.reposition.AutoSize = true;
			this.reposition.Location = new System.Drawing.Point(18, 178);
			this.reposition.Name = "reposition";
			this.reposition.Size = new System.Drawing.Size(187, 18);
			this.reposition.TabIndex = 5;
			this.reposition.Text = "Reposition after dragging sectors";
			this.reposition.UseVisualStyleBackColor = true;
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.removeselectedsectorsceiling);
			this.groupBox3.Controls.Add(this.addselectedsectorsceiling);
			this.groupBox3.Location = new System.Drawing.Point(297, 13);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(195, 71);
			this.groupBox3.TabIndex = 8;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Ceiling";
			// 
			// removeselectedsectorsceiling
			// 
			this.removeselectedsectorsceiling.AutoSize = true;
			this.removeselectedsectorsceiling.Location = new System.Drawing.Point(6, 43);
			this.removeselectedsectorsceiling.Name = "removeselectedsectorsceiling";
			this.removeselectedsectorsceiling.Size = new System.Drawing.Size(174, 18);
			this.removeselectedsectorsceiling.TabIndex = 6;
			this.removeselectedsectorsceiling.Text = "Remove from selected sectors";
			this.removeselectedsectorsceiling.UseVisualStyleBackColor = true;
			this.removeselectedsectorsceiling.CheckedChanged += new System.EventHandler(this.removeselectedsectorsceiling_CheckedChanged);
			// 
			// addselectedsectorsceiling
			// 
			this.addselectedsectorsceiling.AutoSize = true;
			this.addselectedsectorsceiling.Location = new System.Drawing.Point(6, 19);
			this.addselectedsectorsceiling.Name = "addselectedsectorsceiling";
			this.addselectedsectorsceiling.Size = new System.Drawing.Size(150, 18);
			this.addselectedsectorsceiling.TabIndex = 5;
			this.addselectedsectorsceiling.Text = "Apply to selected sectors";
			this.addselectedsectorsceiling.UseVisualStyleBackColor = true;
			this.addselectedsectorsceiling.CheckedChanged += new System.EventHandler(this.addselectedsectorsceiling_CheckedChanged);
			// 
			// groupBox4
			// 
			this.groupBox4.Controls.Add(this.removeselectedsectorsfloor);
			this.groupBox4.Controls.Add(this.addselectedsectorsfloor);
			this.groupBox4.Location = new System.Drawing.Point(297, 90);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Size = new System.Drawing.Size(195, 72);
			this.groupBox4.TabIndex = 9;
			this.groupBox4.TabStop = false;
			this.groupBox4.Text = "Floor";
			// 
			// removeselectedsectorsfloor
			// 
			this.removeselectedsectorsfloor.AutoSize = true;
			this.removeselectedsectorsfloor.Location = new System.Drawing.Point(6, 43);
			this.removeselectedsectorsfloor.Name = "removeselectedsectorsfloor";
			this.removeselectedsectorsfloor.Size = new System.Drawing.Size(174, 18);
			this.removeselectedsectorsfloor.TabIndex = 9;
			this.removeselectedsectorsfloor.Text = "Remove from selected sectors";
			this.removeselectedsectorsfloor.UseVisualStyleBackColor = true;
			this.removeselectedsectorsfloor.CheckedChanged += new System.EventHandler(this.removeselectedsectorsfloor_CheckedChanged);
			// 
			// addselectedsectorsfloor
			// 
			this.addselectedsectorsfloor.AutoSize = true;
			this.addselectedsectorsfloor.Location = new System.Drawing.Point(6, 19);
			this.addselectedsectorsfloor.Name = "addselectedsectorsfloor";
			this.addselectedsectorsfloor.Size = new System.Drawing.Size(150, 18);
			this.addselectedsectorsfloor.TabIndex = 8;
			this.addselectedsectorsfloor.Text = "Apply to selected sectors";
			this.addselectedsectorsfloor.UseVisualStyleBackColor = true;
			this.addselectedsectorsfloor.CheckedChanged += new System.EventHandler(this.addselectedsectorsfloor_CheckedChanged);
			// 
			// spline
			// 
			this.spline.AutoSize = true;
			this.spline.Location = new System.Drawing.Point(18, 154);
			this.spline.Name = "spline";
			this.spline.Size = new System.Drawing.Size(55, 18);
			this.spline.TabIndex = 10;
			this.spline.Text = "Spline";
			this.spline.UseVisualStyleBackColor = true;
			this.spline.Visible = false;
			// 
			// SlopeVertexEditForm
			// 
			this.AcceptButton = this.apply;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.cancel;
			this.ClientSize = new System.Drawing.Size(504, 211);
			this.Controls.Add(this.spline);
			this.Controls.Add(this.groupBox4);
			this.Controls.Add(this.groupBox3);
			this.Controls.Add(this.reposition);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.cancel);
			this.Controls.Add(this.apply);
			this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Name = "SlopeVertexEditForm";
			this.ShowIcon = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Edit Slope Vertex";
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			this.groupBox4.ResumeLayout(false);
			this.groupBox4.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button apply;
		private System.Windows.Forms.Button cancel;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		public CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox positionz;
		public CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox positiony;
		public CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox positionx;
		private System.Windows.Forms.GroupBox groupBox2;
		public System.Windows.Forms.CheckedListBox checkedListBoxSectors;
		private System.Windows.Forms.CheckBox reposition;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.CheckBox removeselectedsectorsceiling;
		private System.Windows.Forms.CheckBox addselectedsectorsceiling;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.CheckBox removeselectedsectorsfloor;
		private System.Windows.Forms.CheckBox addselectedsectorsfloor;
		private System.Windows.Forms.CheckBox spline;
	}
}
