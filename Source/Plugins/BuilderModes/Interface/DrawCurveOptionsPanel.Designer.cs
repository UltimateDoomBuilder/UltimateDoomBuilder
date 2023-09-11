namespace CodeImp.DoomBuilder.BuilderModes
{
	partial class DrawCurveOptionsPanel
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
			this.toolstrip = new System.Windows.Forms.ToolStrip();
			this.continuousdrawing = new System.Windows.Forms.ToolStripButton();
			this.autoclosedrawing = new System.Windows.Forms.ToolStripButton();
			this.placethingsatvertices = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.seglabel = new System.Windows.Forms.ToolStripLabel();
			this.seglen = new CodeImp.DoomBuilder.Controls.ToolStripNumericUpDown();
			this.reset = new System.Windows.Forms.ToolStripButton();
			this.toolstrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// toolstrip
			// 
			this.toolstrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.continuousdrawing,
            this.autoclosedrawing,
			this.placethingsatvertices,
			this.toolStripSeparator1,
            this.seglabel,
            this.seglen,
            this.reset});
			this.toolstrip.Location = new System.Drawing.Point(0, 0);
			this.toolstrip.Name = "toolstrip";
			this.toolstrip.Size = new System.Drawing.Size(562, 25);
			this.toolstrip.TabIndex = 7;
			this.toolstrip.Text = "toolStrip1";
			// 
			// continuousdrawing
			// 
			this.continuousdrawing.CheckOnClick = true;
			this.continuousdrawing.Image = global::CodeImp.DoomBuilder.BuilderModes.Properties.Resources.Repeat;
			this.continuousdrawing.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.continuousdrawing.Name = "continuousdrawing";
			this.continuousdrawing.Size = new System.Drawing.Size(135, 22);
			this.continuousdrawing.Text = "Continuous drawing";
			this.continuousdrawing.CheckedChanged += new System.EventHandler(this.continuousdrawing_CheckedChanged);
			// 
			// autoclosedrawing
			// 
			this.autoclosedrawing.CheckOnClick = true;
			this.autoclosedrawing.Image = global::CodeImp.DoomBuilder.BuilderModes.Properties.Resources.NewSector2;
			this.autoclosedrawing.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.autoclosedrawing.Margin = new System.Windows.Forms.Padding(2, 1, 0, 2);
			this.autoclosedrawing.Name = "autoclosedrawing";
			this.autoclosedrawing.Size = new System.Drawing.Size(131, 22);
			this.autoclosedrawing.Text = "Auto-close drawing";
			this.autoclosedrawing.CheckedChanged += new System.EventHandler(this.autoclosedrawing_CheckedChanged);
			// 
			// placethingsatvertices
			// 
			this.placethingsatvertices.CheckOnClick = true;
			this.placethingsatvertices.Image = global::CodeImp.DoomBuilder.BuilderModes.Properties.Resources.PlaceThings;
			this.placethingsatvertices.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.placethingsatvertices.Name = "placethingsatvertices";
			this.placethingsatvertices.Size = new System.Drawing.Size(135, 22);
			this.placethingsatvertices.Text = "Place things";
			this.placethingsatvertices.CheckedChanged += new System.EventHandler(this.placethingsatvertices_CheckedChanged);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
			// 
			// seglabel
			// 
			this.seglabel.Name = "seglabel";
			this.seglabel.Size = new System.Drawing.Size(97, 22);
			this.seglabel.Text = "Segment Length:";
			// 
			// seglen
			// 
			this.seglen.AutoSize = false;
			this.seglen.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
			this.seglen.Maximum = new decimal(new int[] {
            100,
            0,
            0,
            0});
			this.seglen.Minimum = new decimal(new int[] {
            0,
            0,
            0,
            0});
			this.seglen.Name = "seglen";
			this.seglen.Size = new System.Drawing.Size(56, 23);
			this.seglen.Text = "0";
			this.seglen.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
			this.seglen.ValueChanged += new System.EventHandler(this.seglen_ValueChanged);
			// 
			// reset
			// 
			this.reset.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.reset.Image = global::CodeImp.DoomBuilder.BuilderModes.Properties.Resources.Reset;
			this.reset.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.reset.Name = "reset";
			this.reset.Size = new System.Drawing.Size(23, 22);
			this.reset.Text = "Reset";
			this.reset.Click += new System.EventHandler(this.reset_Click);
			// 
			// DrawCurveOptionsPanel
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.Controls.Add(this.toolstrip);
			this.Name = "DrawCurveOptionsPanel";
			this.Size = new System.Drawing.Size(562, 60);
			this.toolstrip.ResumeLayout(false);
			this.toolstrip.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ToolStripLabel seglabel;
		internal CodeImp.DoomBuilder.Controls.ToolStripNumericUpDown seglen;
		private System.Windows.Forms.ToolStrip toolstrip;
		private System.Windows.Forms.ToolStripButton reset;
		private System.Windows.Forms.ToolStripButton continuousdrawing;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripButton autoclosedrawing;
		private System.Windows.Forms.ToolStripButton placethingsatvertices;
	}
}
