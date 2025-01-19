namespace CodeImp.DoomBuilder.BuilderModes
{
	partial class DrawGridOptionsPanel
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
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.gridlockmode = new System.Windows.Forms.ComboBox();
			this.label5 = new System.Windows.Forms.Label();
			this.label5.AutoSize = true;
			this.triangulate = new System.Windows.Forms.CheckBox();
			this.reset = new System.Windows.Forms.Button();
			this.slicesV = new System.Windows.Forms.NumericUpDown();
			this.slicesH = new System.Windows.Forms.NumericUpDown();
			this.label2 = new System.Windows.Forms.Label();
			this.label2.AutoSize = true;
			this.label1 = new System.Windows.Forms.Label();
			this.label1.AutoSize = true;
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.interpvmode = new System.Windows.Forms.ComboBox();
			this.interphmode = new System.Windows.Forms.ComboBox();
			this.label3 = new System.Windows.Forms.Label();
			this.label3.AutoSize = true;
			this.label4 = new System.Windows.Forms.Label();
			this.label4.AutoSize = true;
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.relativeinterpolation = new System.Windows.Forms.CheckBox();
			this.showguidelines = new System.Windows.Forms.CheckBox();
			this.continuousdrawing = new System.Windows.Forms.CheckBox();
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.slicesV)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.slicesH)).BeginInit();
			this.groupBox2.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.gridlockmode);
			this.groupBox1.Controls.Add(this.label5);
			this.groupBox1.Controls.Add(this.triangulate);
			this.groupBox1.Controls.Add(this.reset);
			this.groupBox1.Controls.Add(this.slicesV);
			this.groupBox1.Controls.Add(this.slicesH);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Location = new System.Drawing.Point(3, 3);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(200, 128);
			this.groupBox1.TabIndex = 9;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = " Number of slices: ";
			// 
			// gridlockmode
			// 
			this.gridlockmode.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridlockmode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.gridlockmode.FormattingEnabled = true;
			this.gridlockmode.Items.AddRange(new object[] {
            "None",
            "Horizontal",
            "Vertical",
            "Both"});
			this.gridlockmode.Location = new System.Drawing.Point(118, 77);
			this.gridlockmode.Name = "gridlockmode";
			this.gridlockmode.Size = new System.Drawing.Size(76, 21);
			this.gridlockmode.TabIndex = 16;
			this.gridlockmode.SelectedIndexChanged += new System.EventHandler(this.gridlockmode_SelectedIndexChanged);
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(17, 80);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(95, 13);
			this.label5.TabIndex = 15;
			this.label5.Text = "Lock slices to grid:";
			// 
			// triangulate
			// 
			this.triangulate.AutoSize = true;
			this.triangulate.Location = new System.Drawing.Point(20, 103);
			this.triangulate.Name = "triangulate";
			this.triangulate.Size = new System.Drawing.Size(79, 17);
			this.triangulate.TabIndex = 14;
			this.triangulate.Text = "Triangulate";
			this.triangulate.UseVisualStyleBackColor = true;
			this.triangulate.CheckedChanged += new System.EventHandler(this.ValueChanged);
			// 
			// reset
			// 
			this.reset.Image = global::CodeImp.DoomBuilder.BuilderModes.Properties.Resources.Reset;
			this.reset.Location = new System.Drawing.Point(140, 22);
			this.reset.Name = "reset";
			this.reset.Size = new System.Drawing.Size(24, 45);
			this.reset.TabIndex = 10;
			this.reset.UseVisualStyleBackColor = true;
			this.reset.Click += new System.EventHandler(this.reset_Click);
			// 
			// slicesV
			// 
			this.slicesV.Location = new System.Drawing.Point(77, 47);
			this.slicesV.Name = "slicesV";
			this.slicesV.Size = new System.Drawing.Size(57, 20);
			this.slicesV.TabIndex = 12;
			this.slicesV.ValueChanged += new System.EventHandler(this.ValueChanged);
			// 
			// slicesH
			// 
			this.slicesH.Location = new System.Drawing.Point(77, 22);
			this.slicesH.Name = "slicesH";
			this.slicesH.Size = new System.Drawing.Size(57, 20);
			this.slicesH.TabIndex = 10;
			this.slicesH.ValueChanged += new System.EventHandler(this.ValueChanged);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(29, 49);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(42, 13);
			this.label2.TabIndex = 11;
			this.label2.Text = "Vertical";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(17, 24);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(54, 13);
			this.label1.TabIndex = 10;
			this.label1.Text = "Horizontal";
			// 
			// groupBox2
			// 
			this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox2.Controls.Add(this.interpvmode);
			this.groupBox2.Controls.Add(this.interphmode);
			this.groupBox2.Controls.Add(this.label3);
			this.groupBox2.Controls.Add(this.label4);
			this.groupBox2.Location = new System.Drawing.Point(3, 137);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(200, 82);
			this.groupBox2.TabIndex = 10;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = " Slices spacing interpolation: ";
			// 
			// interpvmode
			// 
			this.interpvmode.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.interpvmode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.interpvmode.FormattingEnabled = true;
			this.interpvmode.Location = new System.Drawing.Point(77, 46);
			this.interpvmode.Name = "interpvmode";
			this.interpvmode.Size = new System.Drawing.Size(117, 21);
			this.interpvmode.TabIndex = 17;
			this.interpvmode.SelectedIndexChanged += new System.EventHandler(this.ValueChanged);
			this.interpvmode.DropDownClosed += new System.EventHandler(this.interpmode_DropDownClosed);
			// 
			// interphmode
			// 
			this.interphmode.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.interphmode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.interphmode.FormattingEnabled = true;
			this.interphmode.Location = new System.Drawing.Point(77, 19);
			this.interphmode.Name = "interphmode";
			this.interphmode.Size = new System.Drawing.Size(117, 21);
			this.interphmode.TabIndex = 11;
			this.interphmode.SelectedIndexChanged += new System.EventHandler(this.ValueChanged);
			this.interphmode.DropDownClosed += new System.EventHandler(this.interpmode_DropDownClosed);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(29, 49);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(42, 13);
			this.label3.TabIndex = 16;
			this.label3.Text = "Vertical";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(17, 24);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(54, 13);
			this.label4.TabIndex = 15;
			this.label4.Text = "Horizontal";
			// 
			// groupBox3
			// 
			this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox3.Controls.Add(this.relativeinterpolation);
			this.groupBox3.Controls.Add(this.showguidelines);
			this.groupBox3.Controls.Add(this.continuousdrawing);
			this.groupBox3.Location = new System.Drawing.Point(3, 225);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(200, 98);
			this.groupBox3.TabIndex = 11;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Additional options";
			// 
			// relativeinterpolation
			// 
			this.relativeinterpolation.AutoSize = true;
			this.relativeinterpolation.Location = new System.Drawing.Point(20, 70);
			this.relativeinterpolation.Name = "relativeinterpolation";
			this.relativeinterpolation.Size = new System.Drawing.Size(125, 17);
			this.relativeinterpolation.TabIndex = 17;
			this.relativeinterpolation.Text = "Relative interpolation";
			this.relativeinterpolation.UseVisualStyleBackColor = true;
			this.relativeinterpolation.CheckedChanged += new System.EventHandler(this.relativeinterpolation_CheckedChanged);
			// 
			// showguidelines
			// 
			this.showguidelines.AutoSize = true;
			this.showguidelines.Location = new System.Drawing.Point(20, 47);
			this.showguidelines.Name = "showguidelines";
			this.showguidelines.Size = new System.Drawing.Size(75, 17);
			this.showguidelines.TabIndex = 16;
			this.showguidelines.Text = "Guidelines";
			this.showguidelines.UseVisualStyleBackColor = true;
			this.showguidelines.CheckedChanged += new System.EventHandler(this.showguidelines_CheckedChanged);
			// 
			// continuousdrawing
			// 
			this.continuousdrawing.AutoSize = true;
			this.continuousdrawing.Location = new System.Drawing.Point(20, 24);
			this.continuousdrawing.Name = "continuousdrawing";
			this.continuousdrawing.Size = new System.Drawing.Size(119, 17);
			this.continuousdrawing.TabIndex = 15;
			this.continuousdrawing.Text = "Continuous drawing";
			this.continuousdrawing.UseVisualStyleBackColor = true;
			this.continuousdrawing.CheckedChanged += new System.EventHandler(this.continuousdrawing_CheckedChanged);
			// 
			// DrawGridOptionsPanel
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.Controls.Add(this.groupBox3);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Name = "DrawGridOptionsPanel";
			this.Size = new System.Drawing.Size(206, 426);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.slicesV)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.slicesH)).EndInit();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.CheckBox triangulate;
		private System.Windows.Forms.Button reset;
		private System.Windows.Forms.NumericUpDown slicesV;
		private System.Windows.Forms.NumericUpDown slicesH;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.ComboBox interpvmode;
		private System.Windows.Forms.ComboBox interphmode;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.CheckBox continuousdrawing;
		private System.Windows.Forms.ComboBox gridlockmode;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.CheckBox showguidelines;
		private System.Windows.Forms.CheckBox relativeinterpolation;
	}
}
