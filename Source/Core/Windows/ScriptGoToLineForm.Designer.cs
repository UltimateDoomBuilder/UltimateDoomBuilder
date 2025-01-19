namespace CodeImp.DoomBuilder.Windows
{
	partial class ScriptGoToLineForm
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
			this.label = new System.Windows.Forms.Label();
			this.label.AutoSize = true;
			this.linenumber = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.panel1 = new System.Windows.Forms.Panel();
			this.cancel = new System.Windows.Forms.Button();
			this.accept = new System.Windows.Forms.Button();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// label
			// 
			this.label.AutoSize = true;
			this.label.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.label.Location = new System.Drawing.Point(13, 19);
			this.label.Name = "label";
			this.label.Size = new System.Drawing.Size(68, 13);
			this.label.TabIndex = 0;
			this.label.Text = "Line number:";
			// 
			// linenumber
			// 
			this.linenumber.AllowDecimal = false;
			this.linenumber.AllowExpressions = false;
			this.linenumber.AllowNegative = false;
			this.linenumber.AllowRelative = false;
			this.linenumber.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.linenumber.ButtonStep = 1;
			this.linenumber.ButtonStepBig = 10F;
			this.linenumber.ButtonStepFloat = 1F;
			this.linenumber.ButtonStepSmall = 0.1F;
			this.linenumber.ButtonStepsUseModifierKeys = false;
			this.linenumber.ButtonStepsWrapAround = false;
			this.linenumber.Location = new System.Drawing.Point(87, 14);
			this.linenumber.Name = "linenumber";
			this.linenumber.Size = new System.Drawing.Size(186, 24);
			this.linenumber.StepValues = null;
			this.linenumber.TabIndex = 1;
			// 
			// panel1
			// 
			this.panel1.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.panel1.Controls.Add(this.linenumber);
			this.panel1.Controls.Add(this.label);
			this.panel1.Location = new System.Drawing.Point(-1, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(286, 50);
			this.panel1.TabIndex = 2;
			// 
			// cancel
			// 
			this.cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancel.Location = new System.Drawing.Point(185, 58);
			this.cancel.Name = "cancel";
			this.cancel.Size = new System.Drawing.Size(94, 26);
			this.cancel.TabIndex = 2;
			this.cancel.Text = "Cancel";
			this.cancel.UseVisualStyleBackColor = true;
			this.cancel.Click += new System.EventHandler(this.cancel_Click);
			// 
			// accept
			// 
			this.accept.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.accept.Location = new System.Drawing.Point(85, 58);
			this.accept.Name = "accept";
			this.accept.Size = new System.Drawing.Size(94, 26);
			this.accept.TabIndex = 3;
			this.accept.Text = "OK";
			this.accept.UseVisualStyleBackColor = true;
			this.accept.Click += new System.EventHandler(this.accept_Click);
			// 
			// ScriptGoToLineForm
			// 
			this.AcceptButton = this.accept;
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.CancelButton = this.cancel;
			this.ClientSize = new System.Drawing.Size(284, 89);
			this.Controls.Add(this.accept);
			this.Controls.Add(this.cancel);
			this.Controls.Add(this.panel1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ScriptGoToLineForm";
			this.Opacity = 0;
			this.Text = "Go To Line";
			this.Shown += new System.EventHandler(this.ScriptGoToLineForm_Shown);
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label label;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox linenumber;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Button cancel;
		private System.Windows.Forms.Button accept;
	}
}
