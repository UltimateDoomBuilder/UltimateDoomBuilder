namespace CodeImp.DoomBuilder.Windows
{
	partial class CenterOnCoordinatesForm
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
			this.label1 = new System.Windows.Forms.Label();
			this.label1.AutoSize = true;
			this.label2 = new System.Windows.Forms.Label();
			this.label2.AutoSize = true;
			this.accept = new System.Windows.Forms.Button();
			this.cancel = new System.Windows.Forms.Button();
			this.gotoy = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.gotox = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 17);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(17, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "X:";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(12, 47);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(17, 13);
			this.label2.TabIndex = 1;
			this.label2.Text = "Y:";
			// 
			// accept
			// 
			this.accept.Location = new System.Drawing.Point(109, 12);
			this.accept.Name = "accept";
			this.accept.Size = new System.Drawing.Size(65, 24);
			this.accept.TabIndex = 2;
			this.accept.Text = "OK";
			this.accept.UseVisualStyleBackColor = true;
			this.accept.Click += new System.EventHandler(this.accept_Click);
			// 
			// cancel
			// 
			this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancel.Location = new System.Drawing.Point(109, 42);
			this.cancel.Name = "cancel";
			this.cancel.Size = new System.Drawing.Size(65, 24);
			this.cancel.TabIndex = 3;
			this.cancel.Text = "Cancel";
			this.cancel.UseVisualStyleBackColor = true;
			this.cancel.Click += new System.EventHandler(this.cancel_Click);
			// 
			// gotoy
			// 
			this.gotoy.AllowDecimal = false;
			this.gotoy.AllowNegative = true;
			this.gotoy.AllowRelative = false;
			this.gotoy.ButtonStep = 1;
			this.gotoy.ButtonStepBig = 10F;
			this.gotoy.ButtonStepFloat = 1F;
			this.gotoy.ButtonStepSmall = 0.1F;
			this.gotoy.ButtonStepsUseModifierKeys = false;
			this.gotoy.ButtonStepsWrapAround = false;
			this.gotoy.Location = new System.Drawing.Point(32, 42);
			this.gotoy.Name = "gotoy";
			this.gotoy.Size = new System.Drawing.Size(71, 24);
			this.gotoy.StepValues = null;
			this.gotoy.TabIndex = 1;
			// 
			// gotox
			// 
			this.gotox.AllowDecimal = false;
			this.gotox.AllowNegative = true;
			this.gotox.AllowRelative = false;
			this.gotox.ButtonStep = 1;
			this.gotox.ButtonStepBig = 10F;
			this.gotox.ButtonStepFloat = 1F;
			this.gotox.ButtonStepSmall = 0.1F;
			this.gotox.ButtonStepsUseModifierKeys = false;
			this.gotox.ButtonStepsWrapAround = false;
			this.gotox.Location = new System.Drawing.Point(32, 12);
			this.gotox.Name = "gotox";
			this.gotox.Size = new System.Drawing.Size(71, 24);
			this.gotox.StepValues = null;
			this.gotox.TabIndex = 0;
			// 
			// CenterOnCoordinatesForm
			// 
			this.AcceptButton = this.accept;
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.CancelButton = this.cancel;
			this.ClientSize = new System.Drawing.Size(184, 74);
			this.Controls.Add(this.cancel);
			this.Controls.Add(this.accept);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.gotoy);
			this.Controls.Add(this.gotox);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "CenterOnCoordinatesForm";
			this.Opacity = 0;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Go To Coordinates:";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox gotox;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox gotoy;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button accept;
		private System.Windows.Forms.Button cancel;
	}
}
