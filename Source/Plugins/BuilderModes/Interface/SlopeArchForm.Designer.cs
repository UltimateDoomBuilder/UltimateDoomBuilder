namespace CodeImp.DoomBuilder.BuilderModes
{
	partial class SlopeArchForm
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
			this.theta = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.label1 = new System.Windows.Forms.Label();
			this.label1.AutoSize = true;
			this.offset = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.label2 = new System.Windows.Forms.Label();
			this.label2.AutoSize = true;
			this.cancel = new System.Windows.Forms.Button();
			this.accept = new System.Windows.Forms.Button();
			this.up = new System.Windows.Forms.RadioButton();
			this.down = new System.Windows.Forms.RadioButton();
			this.scale = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.label3 = new System.Windows.Forms.Label();
			this.label3.AutoSize = true;
			this.heightoffset = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.label4 = new System.Windows.Forms.Label();
			this.label4.AutoSize = true;
			this.halfcircle = new System.Windows.Forms.Button();
			this.quartercircleright = new System.Windows.Forms.Button();
			this.quartercircleleft = new System.Windows.Forms.Button();
			this.lockoffset = new System.Windows.Forms.CheckBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.invert = new System.Windows.Forms.Button();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// theta
			// 
			this.theta.AllowDecimal = true;
			this.theta.AllowExpressions = false;
			this.theta.AllowNegative = false;
			this.theta.AllowRelative = false;
			this.theta.ButtonStep = 5;
			this.theta.ButtonStepBig = 15F;
			this.theta.ButtonStepFloat = 5F;
			this.theta.ButtonStepSmall = 15F;
			this.theta.ButtonStepsUseModifierKeys = true;
			this.theta.ButtonStepsWrapAround = false;
			this.theta.Location = new System.Drawing.Point(87, 7);
			this.theta.Name = "theta";
			this.theta.Size = new System.Drawing.Size(63, 24);
			this.theta.StepValues = null;
			this.theta.TabIndex = 18;
			this.theta.WhenButtonsClicked += new System.EventHandler(this.theta_changed);
			this.theta.WhenEnterPressed += new System.EventHandler(this.theta_changed);
			this.theta.Leave += new System.EventHandler(this.theta_changed);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(44, 12);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(37, 13);
			this.label1.TabIndex = 19;
			this.label1.Text = "Angle:";
			// 
			// offset
			// 
			this.offset.AllowDecimal = true;
			this.offset.AllowExpressions = false;
			this.offset.AllowNegative = false;
			this.offset.AllowRelative = false;
			this.offset.ButtonStep = 5;
			this.offset.ButtonStepBig = 15F;
			this.offset.ButtonStepFloat = 5F;
			this.offset.ButtonStepSmall = 15F;
			this.offset.ButtonStepsUseModifierKeys = true;
			this.offset.ButtonStepsWrapAround = false;
			this.offset.Enabled = false;
			this.offset.Location = new System.Drawing.Point(87, 40);
			this.offset.Name = "offset";
			this.offset.Size = new System.Drawing.Size(63, 24);
			this.offset.StepValues = null;
			this.offset.TabIndex = 20;
			this.offset.WhenButtonsClicked += new System.EventHandler(this.offset_changed);
			this.offset.WhenEnterPressed += new System.EventHandler(this.offset_changed);
			this.offset.Leave += new System.EventHandler(this.offset_changed);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(15, 45);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(66, 13);
			this.label2.TabIndex = 21;
			this.label2.Text = "Angle offset:";
			// 
			// cancel
			// 
			this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancel.Location = new System.Drawing.Point(147, 194);
			this.cancel.Name = "cancel";
			this.cancel.Size = new System.Drawing.Size(88, 24);
			this.cancel.TabIndex = 22;
			this.cancel.Text = "Cancel";
			this.cancel.UseVisualStyleBackColor = true;
			// 
			// accept
			// 
			this.accept.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.accept.Location = new System.Drawing.Point(53, 194);
			this.accept.Name = "accept";
			this.accept.Size = new System.Drawing.Size(88, 24);
			this.accept.TabIndex = 23;
			this.accept.Text = "Apply";
			this.accept.UseVisualStyleBackColor = true;
			// 
			// up
			// 
			this.up.AutoSize = true;
			this.up.Checked = true;
			this.up.Location = new System.Drawing.Point(11, 19);
			this.up.Name = "up";
			this.up.Size = new System.Drawing.Size(39, 17);
			this.up.TabIndex = 24;
			this.up.TabStop = true;
			this.up.Text = "Up";
			this.up.UseVisualStyleBackColor = true;
			this.up.CheckedChanged += new System.EventHandler(this.up_CheckedChanged);
			// 
			// down
			// 
			this.down.AutoSize = true;
			this.down.Location = new System.Drawing.Point(11, 42);
			this.down.Name = "down";
			this.down.Size = new System.Drawing.Size(53, 17);
			this.down.TabIndex = 25;
			this.down.Text = "Down";
			this.down.UseVisualStyleBackColor = true;
			this.down.CheckedChanged += new System.EventHandler(this.down_CheckedChanged);
			// 
			// scale
			// 
			this.scale.AllowDecimal = true;
			this.scale.AllowExpressions = false;
			this.scale.AllowNegative = false;
			this.scale.AllowRelative = false;
			this.scale.ButtonStep = 5;
			this.scale.ButtonStepBig = 15F;
			this.scale.ButtonStepFloat = 5F;
			this.scale.ButtonStepSmall = 15F;
			this.scale.ButtonStepsUseModifierKeys = true;
			this.scale.ButtonStepsWrapAround = false;
			this.scale.Location = new System.Drawing.Point(87, 106);
			this.scale.Name = "scale";
			this.scale.Size = new System.Drawing.Size(63, 24);
			this.scale.StepValues = null;
			this.scale.TabIndex = 26;
			this.scale.WhenTextChanged += new System.EventHandler(this.scale_changed);
			this.scale.WhenEnterPressed += new System.EventHandler(this.scale_changed);
			this.scale.Leave += new System.EventHandler(this.scale_changed);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(33, 111);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(48, 13);
			this.label3.TabIndex = 27;
			this.label3.Text = "Scale %:";
			// 
			// heightoffset
			// 
			this.heightoffset.AllowDecimal = true;
			this.heightoffset.AllowExpressions = false;
			this.heightoffset.AllowNegative = true;
			this.heightoffset.AllowRelative = false;
			this.heightoffset.ButtonStep = 8;
			this.heightoffset.ButtonStepBig = 16F;
			this.heightoffset.ButtonStepFloat = 8F;
			this.heightoffset.ButtonStepSmall = 16F;
			this.heightoffset.ButtonStepsUseModifierKeys = true;
			this.heightoffset.ButtonStepsWrapAround = false;
			this.heightoffset.Location = new System.Drawing.Point(87, 73);
			this.heightoffset.Name = "heightoffset";
			this.heightoffset.Size = new System.Drawing.Size(63, 24);
			this.heightoffset.StepValues = null;
			this.heightoffset.TabIndex = 31;
			this.heightoffset.WhenTextChanged += new System.EventHandler(this.heightoffset_changed);
			this.heightoffset.WhenEnterPressed += new System.EventHandler(this.heightoffset_changed);
			this.heightoffset.Leave += new System.EventHandler(this.heightoffset_changed);
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(11, 78);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(70, 13);
			this.label4.TabIndex = 32;
			this.label4.Text = "Height offset:";
			// 
			// halfcircle
			// 
			this.halfcircle.Image = global::CodeImp.DoomBuilder.BuilderModes.Properties.Resources.HalfCircle;
			this.halfcircle.Location = new System.Drawing.Point(194, 8);
			this.halfcircle.Name = "halfcircle";
			this.halfcircle.Size = new System.Drawing.Size(32, 23);
			this.halfcircle.TabIndex = 30;
			this.halfcircle.UseVisualStyleBackColor = true;
			this.halfcircle.Click += new System.EventHandler(this.halfcircle_Click);
			// 
			// quartercircleright
			// 
			this.quartercircleright.Image = global::CodeImp.DoomBuilder.BuilderModes.Properties.Resources.QuarterCircleTopRight;
			this.quartercircleright.Location = new System.Drawing.Point(232, 8);
			this.quartercircleright.Name = "quartercircleright";
			this.quartercircleright.Size = new System.Drawing.Size(32, 23);
			this.quartercircleright.TabIndex = 29;
			this.quartercircleright.UseVisualStyleBackColor = true;
			this.quartercircleright.Click += new System.EventHandler(this.quartercircleright_Click);
			// 
			// quartercircleleft
			// 
			this.quartercircleleft.Image = global::CodeImp.DoomBuilder.BuilderModes.Properties.Resources.QuarterCircleTopLeft;
			this.quartercircleleft.Location = new System.Drawing.Point(156, 8);
			this.quartercircleleft.Name = "quartercircleleft";
			this.quartercircleleft.Size = new System.Drawing.Size(32, 23);
			this.quartercircleleft.TabIndex = 28;
			this.quartercircleleft.UseVisualStyleBackColor = true;
			this.quartercircleleft.Click += new System.EventHandler(this.quartercircleleft_Click);
			// 
			// lockoffset
			// 
			this.lockoffset.AutoSize = true;
			this.lockoffset.Checked = true;
			this.lockoffset.CheckState = System.Windows.Forms.CheckState.Checked;
			this.lockoffset.Location = new System.Drawing.Point(156, 42);
			this.lockoffset.Name = "lockoffset";
			this.lockoffset.Size = new System.Drawing.Size(106, 17);
			this.lockoffset.TabIndex = 34;
			this.lockoffset.Text = "Auto angle offset";
			this.lockoffset.UseVisualStyleBackColor = true;
			this.lockoffset.CheckedChanged += new System.EventHandler(this.lockoffset_CheckedChanged);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.invert);
			this.groupBox1.Controls.Add(this.up);
			this.groupBox1.Controls.Add(this.down);
			this.groupBox1.Location = new System.Drawing.Point(156, 65);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(108, 96);
			this.groupBox1.TabIndex = 35;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Direction";
			// 
			// invert
			// 
			this.invert.Location = new System.Drawing.Point(6, 65);
			this.invert.Name = "invert";
			this.invert.Size = new System.Drawing.Size(96, 23);
			this.invert.TabIndex = 36;
			this.invert.Text = "Invert";
			this.invert.UseVisualStyleBackColor = true;
			this.invert.Click += new System.EventHandler(this.invert_Click);
			// 
			// SlopeArchForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.cancel;
			this.ClientSize = new System.Drawing.Size(286, 228);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.lockoffset);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.heightoffset);
			this.Controls.Add(this.halfcircle);
			this.Controls.Add(this.quartercircleright);
			this.Controls.Add(this.quartercircleleft);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.scale);
			this.Controls.Add(this.accept);
			this.Controls.Add(this.cancel);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.offset);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.theta);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SlopeArchForm";
			this.ShowIcon = false;
			this.Text = "Create arch";
			this.Shown += new System.EventHandler(this.SlopeArchForm_Shown);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private Controls.ButtonsNumericTextbox theta;
		private System.Windows.Forms.Label label1;
		private Controls.ButtonsNumericTextbox offset;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button cancel;
		private System.Windows.Forms.Button accept;
		private System.Windows.Forms.RadioButton up;
		private System.Windows.Forms.RadioButton down;
		private Controls.ButtonsNumericTextbox scale;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button quartercircleleft;
		private System.Windows.Forms.Button quartercircleright;
		private System.Windows.Forms.Button halfcircle;
		private Controls.ButtonsNumericTextbox heightoffset;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.CheckBox lockoffset;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Button invert;
	}
}
