namespace CodeImp.DoomBuilder.RejectExplorer
{
	partial class MenusForm
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
			this.toolStrip1 = new System.Windows.Forms.ToolStrip();
			this.colorconfiguration = new CodeImp.DoomBuilder.Controls.ToolStripActionButton();
			this.toolStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// toolStrip1
			// 
			this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.colorconfiguration});
			this.toolStrip1.Location = new System.Drawing.Point(0, 0);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.Size = new System.Drawing.Size(284, 25);
			this.toolStrip1.TabIndex = 0;
			this.toolStrip1.Text = "toolStrip1";
			// 
			// colorconfiguration
			// 
			this.colorconfiguration.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.colorconfiguration.Image = global::CodeImp.DoomBuilder.RejectExplorer.Properties.Resources.ColorManagement;
			this.colorconfiguration.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.colorconfiguration.Name = "colorconfiguration";
			this.colorconfiguration.Size = new System.Drawing.Size(23, 22);
			this.colorconfiguration.Tag = "rejectexplorercolorconfiguration";
			this.colorconfiguration.Text = "Configure colors";
			this.colorconfiguration.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// MenusForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.ClientSize = new System.Drawing.Size(284, 262);
			this.Controls.Add(this.toolStrip1);
			this.Name = "MenusForm";
			this.Text = "MenuForm";
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ToolStrip toolStrip1;
		private CodeImp.DoomBuilder.Controls.ToolStripActionButton colorconfiguration;
	}
}