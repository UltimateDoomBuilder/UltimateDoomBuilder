namespace CodeImp.DoomBuilder.Controls
{
	partial class ColorControl
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
			this.label = new System.Windows.Forms.Label();
			this.label.AutoSize = true;
			this.panel = new System.Windows.Forms.Panel();
			this.dialog = new System.Windows.Forms.ColorDialog();
			this.SuspendLayout();
			// 
			// label
			// 
			this.label.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.label.Location = new System.Drawing.Point(-3, 0);
			this.label.Name = "label";
			this.label.Size = new System.Drawing.Size(175, 23);
			this.label.TabIndex = 0;
			this.label.Text = "Color name:";
			this.label.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// panel
			// 
			this.panel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.panel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
			this.panel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel.Cursor = System.Windows.Forms.Cursors.Hand;
			this.panel.Location = new System.Drawing.Point(179, 2);
			this.panel.Name = "panel";
			this.panel.Size = new System.Drawing.Size(32, 19);
			this.panel.TabIndex = 1;
			this.panel.Click += new System.EventHandler(this.panel_Click);
			// 
			// dialog
			// 
			this.dialog.FullOpen = true;
			// 
			// ColorControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.BackColor = System.Drawing.Color.Transparent;
			this.Controls.Add(this.panel);
			this.Controls.Add(this.label);
			this.MaximumSize = new System.Drawing.Size(10000, 23);
			this.MinimumSize = new System.Drawing.Size(100, 16);
			this.Name = "ColorControl";
			this.Size = new System.Drawing.Size(213, 23);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label label;
		private System.Windows.Forms.Panel panel;
		private System.Windows.Forms.ColorDialog dialog;
	}
}
