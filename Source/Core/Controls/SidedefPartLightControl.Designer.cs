namespace CodeImp.DoomBuilder.Controls
{
	partial class SidedefPartLightControl
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.lbLight = new System.Windows.Forms.Label();
			this.lbLight.AutoSize = true;
			this.cbAbsolute = new System.Windows.Forms.CheckBox();
			this.reset = new System.Windows.Forms.Button();
			this.light = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.SuspendLayout();
			// 
			// lbLight
			// 
			this.lbLight.Location = new System.Drawing.Point(3, 6);
			this.lbLight.Name = "lbLight";
			this.lbLight.Size = new System.Drawing.Size(92, 14);
			this.lbLight.TabIndex = 29;
			this.lbLight.Tag = "";
			this.lbLight.Text = "Brightness:";
			this.lbLight.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// cbAbsolute
			// 
			this.cbAbsolute.AutoSize = true;
			this.cbAbsolute.Location = new System.Drawing.Point(167, 7);
			this.cbAbsolute.Name = "cbAbsolute";
			this.cbAbsolute.Size = new System.Drawing.Size(67, 17);
			this.cbAbsolute.TabIndex = 31;
			this.cbAbsolute.Tag = "lightabsolute";
			this.cbAbsolute.Text = "Absolute";
			this.cbAbsolute.UseVisualStyleBackColor = true;
			this.cbAbsolute.CheckedChanged += new System.EventHandler(this.cbAbsolute_CheckedChanged);
			// 
			// reset
			// 
			this.reset.Image = global::CodeImp.DoomBuilder.Properties.Resources.Reset;
			this.reset.Location = new System.Drawing.Point(236, 3);
			this.reset.Name = "reset";
			this.reset.Size = new System.Drawing.Size(23, 23);
			this.reset.TabIndex = 32;
			this.reset.UseVisualStyleBackColor = true;
			this.reset.Click += new System.EventHandler(this.reset_Click);
			// 
			// light
			// 
			this.light.AllowDecimal = false;
			this.light.AllowExpressions = false;
			this.light.AllowNegative = true;
			this.light.AllowRelative = true;
			this.light.ButtonStep = 16;
			this.light.ButtonStepBig = 32F;
			this.light.ButtonStepFloat = 1F;
			this.light.ButtonStepSmall = 1F;
			this.light.ButtonStepsUseModifierKeys = true;
			this.light.ButtonStepsWrapAround = false;
			this.light.Location = new System.Drawing.Point(99, 2);
			this.light.Name = "light";
			this.light.Size = new System.Drawing.Size(62, 24);
			this.light.StepValues = null;
			this.light.TabIndex = 30;
			this.light.Tag = "";
			this.light.WhenTextChanged += new System.EventHandler(this.light_WhenTextChanged);
			// 
			// SidedefPartLightControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.reset);
			this.Controls.Add(this.light);
			this.Controls.Add(this.lbLight);
			this.Controls.Add(this.cbAbsolute);
			this.Name = "SidedefPartLightControl";
			this.Size = new System.Drawing.Size(262, 29);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button reset;
		private ButtonsNumericTextbox light;
		private System.Windows.Forms.Label lbLight;
		private System.Windows.Forms.CheckBox cbAbsolute;
	}
}
