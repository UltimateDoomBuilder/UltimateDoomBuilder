namespace CodeImp.DoomBuilder.BuilderModes.Interface
{
	partial class CurveLinedefsOptionsPanel
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
			this.vertslabel = new System.Windows.Forms.ToolStripLabel();
			this.verts = new CodeImp.DoomBuilder.Controls.ToolStripNumericUpDown();
			this.distancelabel = new System.Windows.Forms.ToolStripLabel();
			this.distance = new CodeImp.DoomBuilder.Controls.ToolStripNumericUpDown();
			this.anglelabel = new System.Windows.Forms.ToolStripLabel();
			this.angle = new CodeImp.DoomBuilder.Controls.ToolStripNumericUpDown();
			this.flip = new System.Windows.Forms.ToolStripButton();
			this.reset = new System.Windows.Forms.ToolStripButton();
			this.separator1 = new System.Windows.Forms.ToolStripSeparator();
			this.fixedcurve = new System.Windows.Forms.ToolStripButton();
			this.separator2 = new System.Windows.Forms.ToolStripSeparator();
			this.apply = new System.Windows.Forms.ToolStripButton();
			this.cancel = new System.Windows.Forms.ToolStripButton();
			this.toolstrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// toolstrip
			// 
			this.toolstrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.vertslabel,
            this.verts,
            this.distancelabel,
            this.distance,
            this.anglelabel,
            this.angle,
            this.flip,
            this.reset,
            this.separator1,
            this.fixedcurve,
            this.separator2,
            this.cancel,
            this.apply});
			this.toolstrip.Location = new System.Drawing.Point(0, 0);
			this.toolstrip.Name = "toolstrip";
			this.toolstrip.Size = new System.Drawing.Size(760, 25);
			this.toolstrip.TabIndex = 0;
			this.toolstrip.Text = "toolStrip1";
			// 
			// vertslabel
			// 
			this.vertslabel.Name = "vertslabel";
			this.vertslabel.Size = new System.Drawing.Size(53, 22);
			this.vertslabel.Text = "Vertices: ";
			// 
			// verts
			// 
			this.verts.AutoSize = false;
			this.verts.Increment = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.verts.Margin = new System.Windows.Forms.Padding(3, 0, 6, 0);
			this.verts.Maximum = new decimal(new int[] {
            200,
            0,
            0,
            0});
			this.verts.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.verts.Name = "verts";
			this.verts.Size = new System.Drawing.Size(56, 22);
			this.verts.Text = "8";
			this.verts.Value = new decimal(new int[] {
            8,
            0,
            0,
            0});
			this.verts.ValueChanged += new System.EventHandler(this.OnUIValuesChanged);
			// 
			// distancelabel
			// 
			this.distancelabel.Name = "distancelabel";
			this.distancelabel.Size = new System.Drawing.Size(55, 22);
			this.distancelabel.Text = "Distance:";
			// 
			// distance
			// 
			this.distance.AutoSize = false;
			this.distance.Increment = new decimal(new int[] {
            8,
            0,
            0,
            0});
			this.distance.Margin = new System.Windows.Forms.Padding(3, 0, 6, 0);
			this.distance.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
			this.distance.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
			this.distance.Name = "distance";
			this.distance.Size = new System.Drawing.Size(56, 25);
			this.distance.Text = "128";
			this.distance.Value = new decimal(new int[] {
            128,
            0,
            0,
            0});
			this.distance.ValueChanged += new System.EventHandler(this.OnUIValuesChanged);
			// 
			// anglelabel
			// 
			this.anglelabel.Name = "anglelabel";
			this.anglelabel.Size = new System.Drawing.Size(41, 22);
			this.anglelabel.Text = "Angle:";
			// 
			// angle
			// 
			this.angle.AutoSize = false;
			this.angle.Increment = new decimal(new int[] {
            5,
            0,
            0,
            0});
			this.angle.Margin = new System.Windows.Forms.Padding(3, 0, 6, 0);
			this.angle.Maximum = new decimal(new int[] {
            350,
            0,
            0,
            0});
			this.angle.Minimum = new decimal(new int[] {
            0,
            0,
            0,
            0});
			this.angle.Name = "angle";
			this.angle.Size = new System.Drawing.Size(56, 22);
			this.angle.Text = "180";
			this.angle.Value = new decimal(new int[] {
            180,
            0,
            0,
            0});
			this.angle.ValueChanged += new System.EventHandler(this.OnUIValuesChanged);
			// 
			// flip
			// 
			this.flip.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.flip.Image = global::CodeImp.DoomBuilder.BuilderModes.Properties.Resources.Flip;
			this.flip.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.flip.Name = "flip";
			this.flip.Size = new System.Drawing.Size(23, 22);
			this.flip.Text = "Flip Curve";
			this.flip.Click += new System.EventHandler(this.flip_Click);
			// 
			// reset
			// 
			this.reset.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.reset.Image = global::CodeImp.DoomBuilder.BuilderModes.Properties.Resources.Reset;
			this.reset.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.reset.Margin = new System.Windows.Forms.Padding(0, 1, 3, 2);
			this.reset.Name = "reset";
			this.reset.Size = new System.Drawing.Size(23, 22);
			this.reset.Text = "Reset";
			this.reset.Click += new System.EventHandler(this.reset_Click);
			// 
			// separator1
			// 
			this.separator1.Name = "separator1";
			this.separator1.Size = new System.Drawing.Size(6, 25);
			// 
			// fixedcurve
			// 
			this.fixedcurve.CheckOnClick = true;
			this.fixedcurve.Image = global::CodeImp.DoomBuilder.BuilderModes.Properties.Resources.DrawCurveMode;
			this.fixedcurve.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.fixedcurve.Margin = new System.Windows.Forms.Padding(3, 1, 3, 2);
			this.fixedcurve.Name = "fixedcurve";
			this.fixedcurve.Size = new System.Drawing.Size(129, 22);
			this.fixedcurve.Text = "Fixed circular curve";
			this.fixedcurve.CheckedChanged += new System.EventHandler(this.fixedcurve_CheckedChanged);
			// 
			// separator2
			// 
			this.separator2.Name = "separator2";
			this.separator2.Size = new System.Drawing.Size(6, 25);
			// 
			// apply
			// 
			this.apply.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.apply.Image = global::CodeImp.DoomBuilder.BuilderModes.Properties.Resources.Check;
			this.apply.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.apply.Margin = new System.Windows.Forms.Padding(6, 1, 0, 2);
			this.apply.Name = "apply";
			this.apply.Size = new System.Drawing.Size(23, 22);
			this.apply.Text = "Apply";
			this.apply.Click += new System.EventHandler(this.apply_Click);
			// 
			// cancel
			// 
			this.cancel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.cancel.Image = global::CodeImp.DoomBuilder.BuilderModes.Properties.Resources.Close;
			this.cancel.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.cancel.Name = "cancel";
			this.cancel.Size = new System.Drawing.Size(23, 22);
			this.cancel.Text = "Cancel";
			this.cancel.Click += new System.EventHandler(this.cancel_Click);
			// 
			// CurveLinedefsOptionsPanel
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.Controls.Add(this.toolstrip);
			this.Name = "CurveLinedefsOptionsPanel";
			this.Size = new System.Drawing.Size(760, 60);
			this.toolstrip.ResumeLayout(false);
			this.toolstrip.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ToolStrip toolstrip;
		private System.Windows.Forms.ToolStripLabel vertslabel;
		private CodeImp.DoomBuilder.Controls.ToolStripNumericUpDown verts;
		private System.Windows.Forms.ToolStripLabel distancelabel;
		private CodeImp.DoomBuilder.Controls.ToolStripNumericUpDown distance;
		private System.Windows.Forms.ToolStripLabel anglelabel;
		private CodeImp.DoomBuilder.Controls.ToolStripNumericUpDown angle;
		private System.Windows.Forms.ToolStripSeparator separator1;
		private System.Windows.Forms.ToolStripButton fixedcurve;
		private System.Windows.Forms.ToolStripSeparator separator2;
		private System.Windows.Forms.ToolStripButton apply;
		private System.Windows.Forms.ToolStripButton cancel;
		private System.Windows.Forms.ToolStripButton reset;
		private System.Windows.Forms.ToolStripButton flip;
	}
}
