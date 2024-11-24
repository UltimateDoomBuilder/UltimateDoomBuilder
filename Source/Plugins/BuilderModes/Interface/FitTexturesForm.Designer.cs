namespace CodeImp.DoomBuilder.BuilderModes
{
	partial class FitTexturesForm
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
			this.labelhorizrepeat = new System.Windows.Forms.Label();
			this.labelhorizrepeat.AutoSize = true;
			this.repeatgroup = new System.Windows.Forms.GroupBox();
			this.vertrepeat = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.horizrepeat = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.cbautoheight = new System.Windows.Forms.CheckBox();
			this.cbautowidth = new System.Windows.Forms.CheckBox();
			this.resetvert = new System.Windows.Forms.Button();
			this.resethoriz = new System.Windows.Forms.Button();
			this.labelvertrepeat = new System.Windows.Forms.Label();
			this.labelvertrepeat.AutoSize = true;
			this.cbfitwidth = new System.Windows.Forms.CheckBox();
			this.accept = new System.Windows.Forms.Button();
			this.cancel = new System.Windows.Forms.Button();
			this.cbfitheight = new System.Windows.Forms.CheckBox();
			this.cbfitconnected = new System.Windows.Forms.CheckBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.patternheight = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.patternwidth = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.labelpatternheight = new System.Windows.Forms.Label();
			this.labelpatternheight.AutoSize = true;
			this.labelpatternwidth = new System.Windows.Forms.Label();
			this.labelpatternwidth.AutoSize = true;
			this.repeatgroup.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// labelhorizrepeat
			// 
			this.labelhorizrepeat.Location = new System.Drawing.Point(14, 24);
			this.labelhorizrepeat.Name = "labelhorizrepeat";
			this.labelhorizrepeat.Size = new System.Drawing.Size(64, 13);
			this.labelhorizrepeat.TabIndex = 0;
			this.labelhorizrepeat.Text = "Horizontal:";
			this.labelhorizrepeat.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// repeatgroup
			// 
			this.repeatgroup.Controls.Add(this.vertrepeat);
			this.repeatgroup.Controls.Add(this.horizrepeat);
			this.repeatgroup.Controls.Add(this.cbautoheight);
			this.repeatgroup.Controls.Add(this.cbautowidth);
			this.repeatgroup.Controls.Add(this.resetvert);
			this.repeatgroup.Controls.Add(this.resethoriz);
			this.repeatgroup.Controls.Add(this.labelvertrepeat);
			this.repeatgroup.Controls.Add(this.labelhorizrepeat);
			this.repeatgroup.Location = new System.Drawing.Point(12, 118);
			this.repeatgroup.Name = "repeatgroup";
			this.repeatgroup.Size = new System.Drawing.Size(251, 80);
			this.repeatgroup.TabIndex = 1;
			this.repeatgroup.TabStop = false;
			this.repeatgroup.Text = " Texture Repeating ";
			// 
			// vertrepeat
			// 
			this.vertrepeat.AllowDecimal = true;
			this.vertrepeat.AllowExpressions = false;
			this.vertrepeat.AllowNegative = true;
			this.vertrepeat.AllowRelative = false;
			this.vertrepeat.ButtonStep = 1;
			this.vertrepeat.ButtonStepBig = 10F;
			this.vertrepeat.ButtonStepFloat = 1F;
			this.vertrepeat.ButtonStepSmall = 1F;
			this.vertrepeat.ButtonStepsUseModifierKeys = true;
			this.vertrepeat.ButtonStepsWrapAround = false;
			this.vertrepeat.Location = new System.Drawing.Point(84, 44);
			this.vertrepeat.Name = "vertrepeat";
			this.vertrepeat.Size = new System.Drawing.Size(63, 24);
			this.vertrepeat.StepValues = null;
			this.vertrepeat.TabIndex = 16;
			this.vertrepeat.WhenTextChanged += new System.EventHandler(this.vertrepeat_ValueChanged);
			// 
			// horizrepeat
			// 
			this.horizrepeat.AllowDecimal = true;
			this.horizrepeat.AllowExpressions = false;
			this.horizrepeat.AllowNegative = true;
			this.horizrepeat.AllowRelative = false;
			this.horizrepeat.ButtonStep = 1;
			this.horizrepeat.ButtonStepBig = 10F;
			this.horizrepeat.ButtonStepFloat = 1F;
			this.horizrepeat.ButtonStepSmall = 1F;
			this.horizrepeat.ButtonStepsUseModifierKeys = true;
			this.horizrepeat.ButtonStepsWrapAround = false;
			this.horizrepeat.Location = new System.Drawing.Point(84, 19);
			this.horizrepeat.Name = "horizrepeat";
			this.horizrepeat.Size = new System.Drawing.Size(63, 24);
			this.horizrepeat.StepValues = null;
			this.horizrepeat.TabIndex = 15;
			this.horizrepeat.WhenTextChanged += new System.EventHandler(this.horizrepeat_ValueChanged);
			// 
			// cbautoheight
			// 
			this.cbautoheight.AutoSize = true;
			this.cbautoheight.Location = new System.Drawing.Point(180, 51);
			this.cbautoheight.Name = "cbautoheight";
			this.cbautoheight.Size = new System.Drawing.Size(48, 17);
			this.cbautoheight.TabIndex = 11;
			this.cbautoheight.Text = "Auto";
			this.cbautoheight.UseVisualStyleBackColor = true;
			this.cbautoheight.CheckedChanged += new System.EventHandler(this.cb_CheckedChanged);
			// 
			// cbautowidth
			// 
			this.cbautowidth.AutoSize = true;
			this.cbautowidth.Location = new System.Drawing.Point(180, 24);
			this.cbautowidth.Name = "cbautowidth";
			this.cbautowidth.Size = new System.Drawing.Size(48, 17);
			this.cbautowidth.TabIndex = 10;
			this.cbautowidth.Text = "Auto";
			this.cbautowidth.UseVisualStyleBackColor = true;
			this.cbautowidth.CheckedChanged += new System.EventHandler(this.cb_CheckedChanged);
			// 
			// resetvert
			// 
			this.resetvert.Image = global::CodeImp.DoomBuilder.BuilderModes.Properties.Resources.Reset;
			this.resetvert.Location = new System.Drawing.Point(150, 46);
			this.resetvert.Name = "resetvert";
			this.resetvert.Size = new System.Drawing.Size(24, 24);
			this.resetvert.TabIndex = 5;
			this.resetvert.UseVisualStyleBackColor = true;
			this.resetvert.Click += new System.EventHandler(this.resetvert_Click);
			// 
			// resethoriz
			// 
			this.resethoriz.Image = global::CodeImp.DoomBuilder.BuilderModes.Properties.Resources.Reset;
			this.resethoriz.Location = new System.Drawing.Point(150, 20);
			this.resethoriz.Name = "resethoriz";
			this.resethoriz.Size = new System.Drawing.Size(24, 24);
			this.resethoriz.TabIndex = 4;
			this.resethoriz.UseVisualStyleBackColor = true;
			this.resethoriz.Click += new System.EventHandler(this.resethoriz_Click);
			// 
			// labelvertrepeat
			// 
			this.labelvertrepeat.Location = new System.Drawing.Point(15, 50);
			this.labelvertrepeat.Name = "labelvertrepeat";
			this.labelvertrepeat.Size = new System.Drawing.Size(64, 13);
			this.labelvertrepeat.TabIndex = 1;
			this.labelvertrepeat.Text = "Vertical:";
			this.labelvertrepeat.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// cbfitwidth
			// 
			this.cbfitwidth.AutoSize = true;
			this.cbfitwidth.Location = new System.Drawing.Point(10, 19);
			this.cbfitwidth.Name = "cbfitwidth";
			this.cbfitwidth.Size = new System.Drawing.Size(65, 17);
			this.cbfitwidth.TabIndex = 2;
			this.cbfitwidth.Text = "Fit width";
			this.cbfitwidth.UseVisualStyleBackColor = true;
			this.cbfitwidth.CheckedChanged += new System.EventHandler(this.cb_CheckedChanged);
			// 
			// accept
			// 
			this.accept.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.accept.Location = new System.Drawing.Point(51, 204);
			this.accept.Name = "accept";
			this.accept.Size = new System.Drawing.Size(88, 24);
			this.accept.TabIndex = 6;
			this.accept.Text = "Apply";
			this.accept.UseVisualStyleBackColor = true;
			this.accept.Click += new System.EventHandler(this.accept_Click);
			// 
			// cancel
			// 
			this.cancel.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.cancel.Location = new System.Drawing.Point(145, 204);
			this.cancel.Name = "cancel";
			this.cancel.Size = new System.Drawing.Size(88, 24);
			this.cancel.TabIndex = 7;
			this.cancel.Text = "Cancel";
			this.cancel.UseVisualStyleBackColor = true;
			this.cancel.Click += new System.EventHandler(this.cancel_Click);
			// 
			// cbfitheight
			// 
			this.cbfitheight.AutoSize = true;
			this.cbfitheight.Location = new System.Drawing.Point(10, 44);
			this.cbfitheight.Name = "cbfitheight";
			this.cbfitheight.Size = new System.Drawing.Size(69, 17);
			this.cbfitheight.TabIndex = 8;
			this.cbfitheight.Text = "Fit height";
			this.cbfitheight.UseVisualStyleBackColor = true;
			this.cbfitheight.CheckedChanged += new System.EventHandler(this.cb_CheckedChanged);
			// 
			// cbfitconnected
			// 
			this.cbfitconnected.AutoSize = true;
			this.cbfitconnected.Location = new System.Drawing.Point(10, 69);
			this.cbfitconnected.Name = "cbfitconnected";
			this.cbfitconnected.Size = new System.Drawing.Size(168, 17);
			this.cbfitconnected.TabIndex = 9;
			this.cbfitconnected.Text = "Fit across connected surfaces";
			this.cbfitconnected.UseVisualStyleBackColor = true;
			this.cbfitconnected.CheckedChanged += new System.EventHandler(this.cb_CheckedChanged);
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.patternheight);
			this.groupBox2.Controls.Add(this.patternwidth);
			this.groupBox2.Controls.Add(this.labelpatternheight);
			this.groupBox2.Controls.Add(this.labelpatternwidth);
			this.groupBox2.Controls.Add(this.cbfitwidth);
			this.groupBox2.Controls.Add(this.cbfitconnected);
			this.groupBox2.Controls.Add(this.cbfitheight);
			this.groupBox2.Location = new System.Drawing.Point(12, 12);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(251, 100);
			this.groupBox2.TabIndex = 10;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = " Options ";
			// 
			// patternheight
			// 
			this.patternheight.AllowDecimal = true;
			this.patternheight.AllowExpressions = false;
			this.patternheight.AllowNegative = false;
			this.patternheight.AllowRelative = false;
			this.patternheight.ButtonStep = 8;
			this.patternheight.ButtonStepBig = 16F;
			this.patternheight.ButtonStepFloat = 8F;
			this.patternheight.ButtonStepSmall = 8F;
			this.patternheight.ButtonStepsUseModifierKeys = true;
			this.patternheight.ButtonStepsWrapAround = false;
			this.patternheight.Location = new System.Drawing.Point(177, 40);
			this.patternheight.Name = "patternheight";
			this.patternheight.Size = new System.Drawing.Size(63, 24);
			this.patternheight.StepValues = null;
			this.patternheight.TabIndex = 18;
			this.patternheight.WhenTextChanged += new System.EventHandler(this.patternsize_ValueChanged);
			// 
			// patternwidth
			// 
			this.patternwidth.AllowDecimal = true;
			this.patternwidth.AllowExpressions = false;
			this.patternwidth.AllowNegative = false;
			this.patternwidth.AllowRelative = false;
			this.patternwidth.ButtonStep = 8;
			this.patternwidth.ButtonStepBig = 16F;
			this.patternwidth.ButtonStepFloat = 8F;
			this.patternwidth.ButtonStepSmall = 8F;
			this.patternwidth.ButtonStepsUseModifierKeys = true;
			this.patternwidth.ButtonStepsWrapAround = false;
			this.patternwidth.Location = new System.Drawing.Point(177, 15);
			this.patternwidth.Name = "patternwidth";
			this.patternwidth.Size = new System.Drawing.Size(63, 24);
			this.patternwidth.StepValues = null;
			this.patternwidth.TabIndex = 17;
			this.patternwidth.WhenTextChanged += new System.EventHandler(this.patternsize_ValueChanged);
			// 
			// labelpatternheight
			// 
			this.labelpatternheight.AutoSize = true;
			this.labelpatternheight.Location = new System.Drawing.Point(98, 45);
			this.labelpatternheight.Name = "labelpatternheight";
			this.labelpatternheight.Size = new System.Drawing.Size(76, 13);
			this.labelpatternheight.TabIndex = 11;
			this.labelpatternheight.Text = "Pattern height:";
			// 
			// labelpatternwidth
			// 
			this.labelpatternwidth.AutoSize = true;
			this.labelpatternwidth.Location = new System.Drawing.Point(102, 20);
			this.labelpatternwidth.Name = "labelpatternwidth";
			this.labelpatternwidth.Size = new System.Drawing.Size(72, 13);
			this.labelpatternwidth.TabIndex = 10;
			this.labelpatternwidth.Text = "Pattern width:";
			// 
			// FitTexturesForm
			// 
			this.AcceptButton = this.accept;
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.CancelButton = this.cancel;
			this.ClientSize = new System.Drawing.Size(275, 233);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.cancel);
			this.Controls.Add(this.accept);
			this.Controls.Add(this.repeatgroup);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FitTexturesForm";
			this.Opacity = 0D;
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Fit Textures";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FitTexturesForm_FormClosing);
			this.TextChanged += new System.EventHandler(this.vertrepeat_ValueChanged);
			this.repeatgroup.ResumeLayout(false);
			this.repeatgroup.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label labelhorizrepeat;
		private System.Windows.Forms.GroupBox repeatgroup;
		private System.Windows.Forms.Button resethoriz;
		private System.Windows.Forms.Label labelvertrepeat;
		private System.Windows.Forms.Button resetvert;
		private System.Windows.Forms.CheckBox cbfitwidth;
		private System.Windows.Forms.Button accept;
		private System.Windows.Forms.Button cancel;
		private System.Windows.Forms.CheckBox cbfitheight;
		private System.Windows.Forms.CheckBox cbfitconnected;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.CheckBox cbautowidth;
		private System.Windows.Forms.CheckBox cbautoheight;
		private System.Windows.Forms.Label labelpatternheight;
		private System.Windows.Forms.Label labelpatternwidth;
		private Controls.ButtonsNumericTextbox horizrepeat;
		private Controls.ButtonsNumericTextbox vertrepeat;
		private Controls.ButtonsNumericTextbox patternwidth;
		private Controls.ButtonsNumericTextbox patternheight;
	}
}
