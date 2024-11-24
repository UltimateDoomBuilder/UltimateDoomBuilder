namespace CodeImp.DoomBuilder.TagRange
{
	partial class TagRangeForm
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
			this.rangestart = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.label1 = new System.Windows.Forms.Label();
			this.label1.AutoSize = true;
			this.doubletagwarning = new System.Windows.Forms.Label();
			this.doubletagwarning.AutoSize = true;
			this.skipdoubletags = new System.Windows.Forms.CheckBox();
			this.okbutton = new System.Windows.Forms.Button();
			this.cancelbutton = new System.Windows.Forms.Button();
			this.outoftagswarning = new System.Windows.Forms.Label();
			this.outoftagswarning.AutoSize = true;
			this.endtaglabel = new System.Windows.Forms.Label();
			this.endtaglabel.AutoSize = true;
			this.label4 = new System.Windows.Forms.Label();
			this.label4.AutoSize = true;
			this.label2 = new System.Windows.Forms.Label();
			this.label2.AutoSize = true;
			this.rangestep = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.relativemode = new System.Windows.Forms.CheckBox();
			this.bglabel = new System.Windows.Forms.Label();
			this.bglabel.AutoSize = true;
			this.SuspendLayout();
			// 
			// rangestart
			// 
			this.rangestart.AllowDecimal = false;
			this.rangestart.AllowNegative = false;
			this.rangestart.AllowRelative = false;
			this.rangestart.ButtonStep = 1;
			this.rangestart.ButtonStepBig = 10F;
			this.rangestart.ButtonStepFloat = 1F;
			this.rangestart.ButtonStepSmall = 0.1F;
			this.rangestart.ButtonStepsUseModifierKeys = false;
			this.rangestart.ButtonStepsWrapAround = false;
			this.rangestart.Location = new System.Drawing.Point(76, 12);
			this.rangestart.Name = "rangestart";
			this.rangestart.Size = new System.Drawing.Size(96, 24);
			this.rangestart.StepValues = null;
			this.rangestart.TabIndex = 0;
			this.rangestart.WhenTextChanged += new System.EventHandler(this.rangestart_WhenTextChanged);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(10, 17);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(62, 14);
			this.label1.TabIndex = 1;
			this.label1.Text = "Start Tag:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// doubletagwarning
			// 
			this.doubletagwarning.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.doubletagwarning.BackColor = System.Drawing.SystemColors.Info;
			this.doubletagwarning.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.doubletagwarning.Location = new System.Drawing.Point(12, 94);
			this.doubletagwarning.Name = "doubletagwarning";
			this.doubletagwarning.Padding = new System.Windows.Forms.Padding(3);
			this.doubletagwarning.Size = new System.Drawing.Size(273, 50);
			this.doubletagwarning.TabIndex = 4;
			this.doubletagwarning.Text = "Warning: The tag range contains already used tags.";
			this.doubletagwarning.Visible = false;
			// 
			// skipdoubletags
			// 
			this.skipdoubletags.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.skipdoubletags.AutoSize = true;
			this.skipdoubletags.BackColor = System.Drawing.SystemColors.Info;
			this.skipdoubletags.Location = new System.Drawing.Point(63, 119);
			this.skipdoubletags.Name = "skipdoubletags";
			this.skipdoubletags.Size = new System.Drawing.Size(157, 17);
			this.skipdoubletags.TabIndex = 5;
			this.skipdoubletags.Text = "Skip over already used tags";
			this.skipdoubletags.UseVisualStyleBackColor = false;
			this.skipdoubletags.Visible = false;
			this.skipdoubletags.CheckedChanged += new System.EventHandler(this.skipdoubletags_CheckedChanged);
			// 
			// okbutton
			// 
			this.okbutton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.okbutton.Location = new System.Drawing.Point(81, 151);
			this.okbutton.Name = "okbutton";
			this.okbutton.Size = new System.Drawing.Size(99, 26);
			this.okbutton.TabIndex = 6;
			this.okbutton.Text = "OK";
			this.okbutton.UseVisualStyleBackColor = true;
			this.okbutton.Click += new System.EventHandler(this.okbutton_Click);
			// 
			// cancelbutton
			// 
			this.cancelbutton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.cancelbutton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelbutton.Location = new System.Drawing.Point(186, 151);
			this.cancelbutton.Name = "cancelbutton";
			this.cancelbutton.Size = new System.Drawing.Size(99, 26);
			this.cancelbutton.TabIndex = 7;
			this.cancelbutton.Text = "Cancel";
			this.cancelbutton.UseVisualStyleBackColor = true;
			this.cancelbutton.Click += new System.EventHandler(this.cancelbutton_Click);
			// 
			// outoftagswarning
			// 
			this.outoftagswarning.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.outoftagswarning.BackColor = System.Drawing.SystemColors.Info;
			this.outoftagswarning.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.outoftagswarning.Location = new System.Drawing.Point(12, 94);
			this.outoftagswarning.Name = "outoftagswarning";
			this.outoftagswarning.Padding = new System.Windows.Forms.Padding(3);
			this.outoftagswarning.Size = new System.Drawing.Size(273, 50);
			this.outoftagswarning.TabIndex = 8;
			this.outoftagswarning.Text = "The range exceeds the maximum or minimum allowed tags and cannot be created.";
			this.outoftagswarning.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.outoftagswarning.Visible = false;
			// 
			// endtaglabel
			// 
			this.endtaglabel.AutoSize = true;
			this.endtaglabel.Location = new System.Drawing.Point(250, 17);
			this.endtaglabel.Name = "endtaglabel";
			this.endtaglabel.Size = new System.Drawing.Size(13, 13);
			this.endtaglabel.TabIndex = 10;
			this.endtaglabel.Text = "0";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(189, 17);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(51, 13);
			this.label4.TabIndex = 9;
			this.label4.Text = "End Tag:";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(10, 47);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(62, 14);
			this.label2.TabIndex = 11;
			this.label2.Text = "Increment:";
			this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// rangestep
			// 
			this.rangestep.AllowDecimal = false;
			this.rangestep.AllowNegative = true;
			this.rangestep.AllowRelative = false;
			this.rangestep.ButtonStep = 1;
			this.rangestep.ButtonStepBig = 10F;
			this.rangestep.ButtonStepFloat = 1F;
			this.rangestep.ButtonStepSmall = 0.1F;
			this.rangestep.ButtonStepsUseModifierKeys = false;
			this.rangestep.ButtonStepsWrapAround = false;
			this.rangestep.Location = new System.Drawing.Point(76, 42);
			this.rangestep.Name = "rangestep";
			this.rangestep.Size = new System.Drawing.Size(96, 24);
			this.rangestep.StepValues = null;
			this.rangestep.TabIndex = 12;
			this.rangestep.WhenTextChanged += new System.EventHandler(this.rangestep_WhenTextChanged);
			// 
			// relativemode
			// 
			this.relativemode.AutoSize = true;
			this.relativemode.Location = new System.Drawing.Point(76, 72);
			this.relativemode.Name = "relativemode";
			this.relativemode.Size = new System.Drawing.Size(138, 17);
			this.relativemode.TabIndex = 13;
			this.relativemode.Text = "Relative to existing tags";
			this.relativemode.UseVisualStyleBackColor = false;
			this.relativemode.CheckedChanged += new System.EventHandler(this.relativemode_CheckedChanged);
			// 
			// bglabel
			// 
			this.bglabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.bglabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.bglabel.Enabled = false;
			this.bglabel.Location = new System.Drawing.Point(12, 94);
			this.bglabel.Name = "bglabel";
			this.bglabel.Padding = new System.Windows.Forms.Padding(3);
			this.bglabel.Size = new System.Drawing.Size(273, 50);
			this.bglabel.TabIndex = 14;
			this.bglabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// TagRangeForm
			// 
			this.AcceptButton = this.okbutton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.CancelButton = this.cancelbutton;
			this.ClientSize = new System.Drawing.Size(298, 183);
			this.Controls.Add(this.relativemode);
			this.Controls.Add(this.rangestep);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.endtaglabel);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.cancelbutton);
			this.Controls.Add(this.okbutton);
			this.Controls.Add(this.skipdoubletags);
			this.Controls.Add(this.doubletagwarning);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.rangestart);
			this.Controls.Add(this.outoftagswarning);
			this.Controls.Add(this.bglabel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "TagRangeForm";
			this.Opacity = 0;
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Tag Selected Range";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox rangestart;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label doubletagwarning;
		private System.Windows.Forms.CheckBox skipdoubletags;
		private System.Windows.Forms.Button okbutton;
		private System.Windows.Forms.Button cancelbutton;
		private System.Windows.Forms.Label outoftagswarning;
		private System.Windows.Forms.Label endtaglabel;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label2;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox rangestep;
		private System.Windows.Forms.CheckBox relativemode;
		private System.Windows.Forms.Label bglabel;
	}
}
