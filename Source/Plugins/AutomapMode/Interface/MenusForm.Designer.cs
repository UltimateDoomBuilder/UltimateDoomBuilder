namespace CodeImp.DoomBuilder.AutomapMode
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.showhiddenlines = new System.Windows.Forms.ToolStripButton();
            this.showsecretsectors = new System.Windows.Forms.ToolStripButton();
            this.showlocks = new System.Windows.Forms.ToolStripButton();
            this.colorpresetseparator = new System.Windows.Forms.ToolStripSeparator();
            this.colorpresetlabel = new System.Windows.Forms.ToolStripLabel();
            this.colorpreset = new System.Windows.Forms.ToolStripComboBox();
            this.showtextures = new System.Windows.Forms.ToolStripButton();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showhiddenlines,
            this.showsecretsectors,
            this.showlocks,
            this.showtextures,
            this.colorpresetseparator,
            this.colorpresetlabel,
            this.colorpreset});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(731, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // showhiddenlines
            // 
            this.showhiddenlines.CheckOnClick = true;
            this.showhiddenlines.Image = global::CodeImp.DoomBuilder.AutomapMode.Properties.Resources.ShowHiddenLines;
            this.showhiddenlines.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.showhiddenlines.Margin = new System.Windows.Forms.Padding(0, 1, 2, 2);
            this.showhiddenlines.Name = "showhiddenlines";
            this.showhiddenlines.Size = new System.Drawing.Size(123, 22);
            this.showhiddenlines.Text = "Show hidden lines";
            this.showhiddenlines.CheckedChanged += new System.EventHandler(this.showhiddenlines_CheckedChanged);
            // 
            // showsecretsectors
            // 
            this.showsecretsectors.CheckOnClick = true;
            this.showsecretsectors.Image = global::CodeImp.DoomBuilder.AutomapMode.Properties.Resources.ShowSecrets;
            this.showsecretsectors.Margin = new System.Windows.Forms.Padding(0, 1, 2, 2);
            this.showsecretsectors.Name = "showsecretsectors";
            this.showsecretsectors.Size = new System.Drawing.Size(95, 22);
            this.showsecretsectors.Text = "Show secrets";
            this.showsecretsectors.CheckedChanged += new System.EventHandler(this.showsecretsectors_CheckedChanged);
            // 
            // showlocks
            // 
            this.showlocks.CheckOnClick = true;
            this.showlocks.Image = global::CodeImp.DoomBuilder.AutomapMode.Properties.Resources.ShowLocks;
            this.showlocks.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.showlocks.Name = "showlocks";
            this.showlocks.Size = new System.Drawing.Size(86, 22);
            this.showlocks.Text = "Show locks";
            this.showlocks.CheckedChanged += new System.EventHandler(this.showlocks_CheckedChanged);
            // 
            // colorpresetseparator
            // 
            this.colorpresetseparator.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.colorpresetseparator.Name = "colorpresetseparator";
            this.colorpresetseparator.Size = new System.Drawing.Size(6, 25);
            // 
            // colorpresetlabel
            // 
            this.colorpresetlabel.Name = "colorpresetlabel";
            this.colorpresetlabel.Size = new System.Drawing.Size(74, 22);
            this.colorpresetlabel.Text = "Color preset:";
            // 
            // colorpreset
            // 
            this.colorpreset.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.colorpreset.Items.AddRange(new object[] {
            "Doom",
            "Hexen",
            "Strife"});
            this.colorpreset.Name = "colorpreset";
            this.colorpreset.Size = new System.Drawing.Size(75, 25);
            this.colorpreset.SelectedIndexChanged += new System.EventHandler(this.colorpreset_SelectedIndexChanged);
            // 
            // showtextures
            // 
            this.showtextures.CheckOnClick = true;
            this.showtextures.Image = global::CodeImp.DoomBuilder.AutomapMode.Properties.Resources.ShowTextures;
            this.showtextures.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.showtextures.Name = "showtextures";
            this.showtextures.Size = new System.Drawing.Size(101, 22);
            this.showtextures.Text = "Show textures";
            this.showtextures.CheckedChanged += new System.EventHandler(this.showtextures_CheckedChanged);
            // 
            // MenusForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.toolStrip1);
            this.Name = "MenusForm";
            this.Size = new System.Drawing.Size(731, 65);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ToolStrip toolStrip1;
		private System.Windows.Forms.ToolStripButton showhiddenlines;
		private System.Windows.Forms.ToolStripButton showsecretsectors;
		private System.Windows.Forms.ToolStripSeparator colorpresetseparator;
		private System.Windows.Forms.ToolStripLabel colorpresetlabel;
		private System.Windows.Forms.ToolStripComboBox colorpreset;
		private System.Windows.Forms.ToolStripButton showlocks;
        private System.Windows.Forms.ToolStripButton showtextures;
    }
}
