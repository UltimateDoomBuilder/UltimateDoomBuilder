namespace CodeImp.DoomBuilder.RejectExplorer
{
	partial class ColorConfiguration
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
			this.highlightcolor = new CodeImp.DoomBuilder.Controls.ColorControl();
			this.okbutton = new System.Windows.Forms.Button();
			this.cancelbutton = new System.Windows.Forms.Button();
			this.resetcolors = new System.Windows.Forms.Button();
			this.defaultcolor = new CodeImp.DoomBuilder.Controls.ColorControl();
			this.bidirectionalcolor = new CodeImp.DoomBuilder.Controls.ColorControl();
			this.unidirectionalfromcolor = new CodeImp.DoomBuilder.Controls.ColorControl();
			this.unidirectionaltocolor = new CodeImp.DoomBuilder.Controls.ColorControl();
			this.SuspendLayout();
			// 
			// highlightcolor
			// 
			this.highlightcolor.BackColor = System.Drawing.Color.Transparent;
			this.highlightcolor.Label = "Highlight color:";
			this.highlightcolor.Location = new System.Drawing.Point(6, 41);
			this.highlightcolor.MaximumSize = new System.Drawing.Size(10000, 23);
			this.highlightcolor.MinimumSize = new System.Drawing.Size(100, 23);
			this.highlightcolor.Name = "highlightcolor";
			this.highlightcolor.Size = new System.Drawing.Size(176, 23);
			this.highlightcolor.TabIndex = 0;
			// 
			// okbutton
			// 
			this.okbutton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.okbutton.Location = new System.Drawing.Point(20, 207);
			this.okbutton.Name = "okbutton";
			this.okbutton.Size = new System.Drawing.Size(73, 23);
			this.okbutton.TabIndex = 6;
			this.okbutton.Text = "OK";
			this.okbutton.UseVisualStyleBackColor = true;
			this.okbutton.Click += new System.EventHandler(this.okbutton_Click);
			// 
			// cancelbutton
			// 
			this.cancelbutton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelbutton.Location = new System.Drawing.Point(97, 207);
			this.cancelbutton.Name = "cancelbutton";
			this.cancelbutton.Size = new System.Drawing.Size(73, 23);
			this.cancelbutton.TabIndex = 7;
			this.cancelbutton.Text = "Cancel";
			this.cancelbutton.UseVisualStyleBackColor = true;
			// 
			// resetcolors
			// 
			this.resetcolors.Location = new System.Drawing.Point(20, 178);
			this.resetcolors.Name = "resetcolors";
			this.resetcolors.Size = new System.Drawing.Size(150, 23);
			this.resetcolors.TabIndex = 5;
			this.resetcolors.Text = "Reset colors";
			this.resetcolors.UseVisualStyleBackColor = true;
			this.resetcolors.Click += new System.EventHandler(this.resetcolors_Click);
			// 
			// defaultcolor
			// 
			this.defaultcolor.BackColor = System.Drawing.Color.Transparent;
			this.defaultcolor.Label = "Default color:";
			this.defaultcolor.Location = new System.Drawing.Point(6, 12);
			this.defaultcolor.MaximumSize = new System.Drawing.Size(10000, 23);
			this.defaultcolor.MinimumSize = new System.Drawing.Size(100, 23);
			this.defaultcolor.Name = "defaultcolor";
			this.defaultcolor.Size = new System.Drawing.Size(176, 23);
			this.defaultcolor.TabIndex = 8;
			// 
			// bidirectionalcolor
			// 
			this.bidirectionalcolor.BackColor = System.Drawing.Color.Transparent;
			this.bidirectionalcolor.Label = "Bidirectional color:";
			this.bidirectionalcolor.Location = new System.Drawing.Point(6, 70);
			this.bidirectionalcolor.MaximumSize = new System.Drawing.Size(10000, 23);
			this.bidirectionalcolor.MinimumSize = new System.Drawing.Size(100, 23);
			this.bidirectionalcolor.Name = "bidirectionalcolor";
			this.bidirectionalcolor.Size = new System.Drawing.Size(176, 23);
			this.bidirectionalcolor.TabIndex = 9;
			// 
			// unidirectionalfromcolor
			// 
			this.unidirectionalfromcolor.BackColor = System.Drawing.Color.Transparent;
			this.unidirectionalfromcolor.Label = "Unidirectional from color:";
			this.unidirectionalfromcolor.Location = new System.Drawing.Point(6, 99);
			this.unidirectionalfromcolor.MaximumSize = new System.Drawing.Size(10000, 23);
			this.unidirectionalfromcolor.MinimumSize = new System.Drawing.Size(100, 23);
			this.unidirectionalfromcolor.Name = "unidirectionalfromcolor";
			this.unidirectionalfromcolor.Size = new System.Drawing.Size(176, 23);
			this.unidirectionalfromcolor.TabIndex = 10;
			// 
			// unidirectionaltocolor
			// 
			this.unidirectionaltocolor.BackColor = System.Drawing.Color.Transparent;
			this.unidirectionaltocolor.Label = "Unidirectional to color:";
			this.unidirectionaltocolor.Location = new System.Drawing.Point(6, 128);
			this.unidirectionaltocolor.MaximumSize = new System.Drawing.Size(10000, 23);
			this.unidirectionaltocolor.MinimumSize = new System.Drawing.Size(100, 23);
			this.unidirectionaltocolor.Name = "unidirectionaltocolor";
			this.unidirectionaltocolor.Size = new System.Drawing.Size(176, 23);
			this.unidirectionaltocolor.TabIndex = 11;
			// 
			// ColorConfiguration
			// 
			this.AcceptButton = this.okbutton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.CancelButton = this.cancelbutton;
			this.ClientSize = new System.Drawing.Size(194, 242);
			this.Controls.Add(this.unidirectionaltocolor);
			this.Controls.Add(this.unidirectionalfromcolor);
			this.Controls.Add(this.bidirectionalcolor);
			this.Controls.Add(this.defaultcolor);
			this.Controls.Add(this.resetcolors);
			this.Controls.Add(this.cancelbutton);
			this.Controls.Add(this.okbutton);
			this.Controls.Add(this.highlightcolor);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ColorConfiguration";
			this.Opacity = 0D;
			this.ShowIcon = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Color Configuration";
			this.ResumeLayout(false);

		}

		#endregion

		private CodeImp.DoomBuilder.Controls.ColorControl highlightcolor;
		private System.Windows.Forms.Button okbutton;
		private System.Windows.Forms.Button cancelbutton;
		private System.Windows.Forms.Button resetcolors;
		private Controls.ColorControl defaultcolor;
		private Controls.ColorControl bidirectionalcolor;
		private Controls.ColorControl unidirectionalfromcolor;
		private Controls.ColorControl unidirectionaltocolor;
	}
}