namespace CodeImp.DoomBuilder.Windows
{
	partial class GridSetupForm
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
			System.Windows.Forms.GroupBox groupBox1;
			System.Windows.Forms.Label label1;
			System.Windows.Forms.GroupBox groupBox2;
			this.gridsize = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.backscaley = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.backscalex = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.backoffsety = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.backoffsetx = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.backscale = new System.Windows.Forms.Label();
			this.backscale.AutoSize = true;
			this.selectfile = new System.Windows.Forms.Button();
			this.showbackground = new System.Windows.Forms.CheckBox();
			this.backoffset = new System.Windows.Forms.Label();
			this.backoffset.AutoSize = true;
			this.selectflat = new System.Windows.Forms.Button();
			this.selecttexture = new System.Windows.Forms.Button();
			this.backgroundimage = new CodeImp.DoomBuilder.Controls.ConfigurablePictureBox();
			this.cancel = new System.Windows.Forms.Button();
			this.apply = new System.Windows.Forms.Button();
			this.browsefile = new System.Windows.Forms.OpenFileDialog();
			groupBox1 = new System.Windows.Forms.GroupBox();
			label1 = new System.Windows.Forms.Label();
			groupBox2 = new System.Windows.Forms.GroupBox();
			groupBox1.SuspendLayout();
			groupBox2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.backgroundimage)).BeginInit();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			groupBox1.Controls.Add(this.gridsize);
			groupBox1.Controls.Add(label1);
			groupBox1.Location = new System.Drawing.Point(12, 12);
			groupBox1.Name = "groupBox1";
			groupBox1.Size = new System.Drawing.Size(293, 71);
			groupBox1.TabIndex = 0;
			groupBox1.TabStop = false;
			groupBox1.Text = " Grid ";
			// 
			// gridsize
			// 
			this.gridsize.AllowDecimal = true;
			this.gridsize.AllowExpressions = false;
			this.gridsize.AllowNegative = false;
			this.gridsize.AllowRelative = false;
			this.gridsize.ButtonStep = 8;
			this.gridsize.ButtonStepBig = 10F;
			this.gridsize.ButtonStepFloat = 1F;
			this.gridsize.ButtonStepSmall = 0.25F;
			this.gridsize.ButtonStepsUseModifierKeys = false;
			this.gridsize.ButtonStepsWrapAround = false;
			this.gridsize.Location = new System.Drawing.Point(146, 26);
			this.gridsize.Name = "gridsize";
			this.gridsize.Size = new System.Drawing.Size(75, 24);
			this.gridsize.StepValues = null;
			this.gridsize.TabIndex = 1;
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(25, 31);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(110, 13);
			label1.TabIndex = 0;
			label1.Text = "Grid size in mappixels:";
			// 
			// groupBox2
			// 
			groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			groupBox2.Controls.Add(this.backscaley);
			groupBox2.Controls.Add(this.backscalex);
			groupBox2.Controls.Add(this.backoffsety);
			groupBox2.Controls.Add(this.backoffsetx);
			groupBox2.Controls.Add(this.backscale);
			groupBox2.Controls.Add(this.selectfile);
			groupBox2.Controls.Add(this.showbackground);
			groupBox2.Controls.Add(this.backoffset);
			groupBox2.Controls.Add(this.selectflat);
			groupBox2.Controls.Add(this.selecttexture);
			groupBox2.Controls.Add(this.backgroundimage);
			groupBox2.Location = new System.Drawing.Point(12, 89);
			groupBox2.Name = "groupBox2";
			groupBox2.Size = new System.Drawing.Size(293, 261);
			groupBox2.TabIndex = 1;
			groupBox2.TabStop = false;
			groupBox2.Text = " Background ";
			// 
			// backscaley
			// 
			this.backscaley.AllowDecimal = false;
			this.backscaley.AllowExpressions = false;
			this.backscaley.AllowNegative = false;
			this.backscaley.AllowRelative = true;
			this.backscaley.ButtonStep = 1;
			this.backscaley.ButtonStepBig = 10F;
			this.backscaley.ButtonStepFloat = 1F;
			this.backscaley.ButtonStepSmall = 0.1F;
			this.backscaley.ButtonStepsUseModifierKeys = false;
			this.backscaley.ButtonStepsWrapAround = false;
			this.backscaley.Enabled = false;
			this.backscaley.Location = new System.Drawing.Point(197, 212);
			this.backscaley.Name = "backscaley";
			this.backscaley.Size = new System.Drawing.Size(67, 24);
			this.backscaley.StepValues = null;
			this.backscaley.TabIndex = 13;
			// 
			// backscalex
			// 
			this.backscalex.AllowDecimal = false;
			this.backscalex.AllowExpressions = false;
			this.backscalex.AllowNegative = false;
			this.backscalex.AllowRelative = true;
			this.backscalex.ButtonStep = 1;
			this.backscalex.ButtonStepBig = 10F;
			this.backscalex.ButtonStepFloat = 1F;
			this.backscalex.ButtonStepSmall = 0.1F;
			this.backscalex.ButtonStepsUseModifierKeys = false;
			this.backscalex.ButtonStepsWrapAround = false;
			this.backscalex.Enabled = false;
			this.backscalex.Location = new System.Drawing.Point(124, 212);
			this.backscalex.Name = "backscalex";
			this.backscalex.Size = new System.Drawing.Size(67, 24);
			this.backscalex.StepValues = null;
			this.backscalex.TabIndex = 12;
			// 
			// backoffsety
			// 
			this.backoffsety.AllowDecimal = false;
			this.backoffsety.AllowExpressions = false;
			this.backoffsety.AllowNegative = true;
			this.backoffsety.AllowRelative = true;
			this.backoffsety.ButtonStep = 1;
			this.backoffsety.ButtonStepBig = 10F;
			this.backoffsety.ButtonStepFloat = 1F;
			this.backoffsety.ButtonStepSmall = 0.1F;
			this.backoffsety.ButtonStepsUseModifierKeys = false;
			this.backoffsety.ButtonStepsWrapAround = false;
			this.backoffsety.Enabled = false;
			this.backoffsety.Location = new System.Drawing.Point(197, 173);
			this.backoffsety.Name = "backoffsety";
			this.backoffsety.Size = new System.Drawing.Size(67, 24);
			this.backoffsety.StepValues = null;
			this.backoffsety.TabIndex = 11;
			// 
			// backoffsetx
			// 
			this.backoffsetx.AllowDecimal = false;
			this.backoffsetx.AllowExpressions = false;
			this.backoffsetx.AllowNegative = true;
			this.backoffsetx.AllowRelative = true;
			this.backoffsetx.ButtonStep = 1;
			this.backoffsetx.ButtonStepBig = 10F;
			this.backoffsetx.ButtonStepFloat = 1F;
			this.backoffsetx.ButtonStepSmall = 0.1F;
			this.backoffsetx.ButtonStepsUseModifierKeys = false;
			this.backoffsetx.ButtonStepsWrapAround = false;
			this.backoffsetx.Enabled = false;
			this.backoffsetx.Location = new System.Drawing.Point(124, 173);
			this.backoffsetx.Name = "backoffsetx";
			this.backoffsetx.Size = new System.Drawing.Size(67, 24);
			this.backoffsetx.StepValues = null;
			this.backoffsetx.TabIndex = 10;
			// 
			// backscale
			// 
			this.backscale.AutoSize = true;
			this.backscale.Enabled = false;
			this.backscale.Location = new System.Drawing.Point(30, 217);
			this.backscale.Name = "backscale";
			this.backscale.Size = new System.Drawing.Size(87, 13);
			this.backscale.TabIndex = 9;
			this.backscale.Text = "Scale in percent:";
			// 
			// selectfile
			// 
			this.selectfile.Enabled = false;
			this.selectfile.Location = new System.Drawing.Point(147, 122);
			this.selectfile.Name = "selectfile";
			this.selectfile.Size = new System.Drawing.Size(117, 25);
			this.selectfile.TabIndex = 3;
			this.selectfile.Text = "Select File...";
			this.selectfile.UseVisualStyleBackColor = true;
			this.selectfile.Click += new System.EventHandler(this.selectfile_Click);
			// 
			// showbackground
			// 
			this.showbackground.AutoSize = true;
			this.showbackground.Location = new System.Drawing.Point(28, 29);
			this.showbackground.Name = "showbackground";
			this.showbackground.Size = new System.Drawing.Size(144, 17);
			this.showbackground.TabIndex = 0;
			this.showbackground.Text = "Show background image";
			this.showbackground.UseVisualStyleBackColor = true;
			this.showbackground.CheckedChanged += new System.EventHandler(this.showbackground_CheckedChanged);
			// 
			// backoffset
			// 
			this.backoffset.AutoSize = true;
			this.backoffset.Enabled = false;
			this.backoffset.Location = new System.Drawing.Point(15, 178);
			this.backoffset.Name = "backoffset";
			this.backoffset.Size = new System.Drawing.Size(98, 13);
			this.backoffset.TabIndex = 4;
			this.backoffset.Text = "Offset in mappixels:";
			// 
			// selectflat
			// 
			this.selectflat.Enabled = false;
			this.selectflat.Location = new System.Drawing.Point(147, 91);
			this.selectflat.Name = "selectflat";
			this.selectflat.Size = new System.Drawing.Size(117, 25);
			this.selectflat.TabIndex = 2;
			this.selectflat.Text = "Select Flat...";
			this.selectflat.UseVisualStyleBackColor = true;
			this.selectflat.Click += new System.EventHandler(this.selectflat_Click);
			// 
			// selecttexture
			// 
			this.selecttexture.Enabled = false;
			this.selecttexture.Location = new System.Drawing.Point(147, 60);
			this.selecttexture.Name = "selecttexture";
			this.selecttexture.Size = new System.Drawing.Size(117, 25);
			this.selecttexture.TabIndex = 1;
			this.selecttexture.Text = "Select Texture...";
			this.selecttexture.UseVisualStyleBackColor = true;
			this.selecttexture.Click += new System.EventHandler(this.selecttexture_Click);
			// 
			// backgroundimage
			// 
			this.backgroundimage.BackColor = System.Drawing.SystemColors.AppWorkspace;
			this.backgroundimage.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.backgroundimage.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.Default;
			this.backgroundimage.Highlighted = false;
			this.backgroundimage.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
			this.backgroundimage.Location = new System.Drawing.Point(28, 60);
			this.backgroundimage.Name = "backgroundimage";
			this.backgroundimage.PageUnit = System.Drawing.GraphicsUnit.Pixel;
			this.backgroundimage.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.None;
			this.backgroundimage.Size = new System.Drawing.Size(87, 87);
			this.backgroundimage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.backgroundimage.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.Default;
			this.backgroundimage.TabIndex = 31;
			this.backgroundimage.TabStop = false;
			// 
			// cancel
			// 
			this.cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancel.Location = new System.Drawing.Point(193, 359);
			this.cancel.Name = "cancel";
			this.cancel.Size = new System.Drawing.Size(112, 25);
			this.cancel.TabIndex = 3;
			this.cancel.Text = "Cancel";
			this.cancel.UseVisualStyleBackColor = true;
			this.cancel.Click += new System.EventHandler(this.cancel_Click);
			// 
			// apply
			// 
			this.apply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.apply.Location = new System.Drawing.Point(75, 359);
			this.apply.Name = "apply";
			this.apply.Size = new System.Drawing.Size(112, 25);
			this.apply.TabIndex = 2;
			this.apply.Text = "OK";
			this.apply.UseVisualStyleBackColor = true;
			this.apply.Click += new System.EventHandler(this.apply_Click);
			// 
			// browsefile
			// 
			this.browsefile.Filter = "All supported images|*.bmp;*.gif;*.png|All Files|*.*";
			this.browsefile.RestoreDirectory = true;
			this.browsefile.Title = "Select Background Image File";
			// 
			// GridSetupForm
			// 
			this.AcceptButton = this.apply;
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.CancelButton = this.cancel;
			this.ClientSize = new System.Drawing.Size(317, 392);
			this.Controls.Add(this.cancel);
			this.Controls.Add(this.apply);
			this.Controls.Add(groupBox2);
			this.Controls.Add(groupBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "GridSetupForm";
			this.Opacity = 0D;
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Custom Grid Setup";
			this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.GridSetupForm_HelpRequested);
			groupBox1.ResumeLayout(false);
			groupBox1.PerformLayout();
			groupBox2.ResumeLayout(false);
			groupBox2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.backgroundimage)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion
		private System.Windows.Forms.Button selecttexture;
		private System.Windows.Forms.Button selectflat;
		private System.Windows.Forms.CheckBox showbackground;
		private System.Windows.Forms.Button cancel;
		private System.Windows.Forms.Button apply;
		private System.Windows.Forms.Label backoffset;
		private System.Windows.Forms.Button selectfile;
		private System.Windows.Forms.Label backscale;
		private System.Windows.Forms.OpenFileDialog browsefile;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox gridsize;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox backscaley;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox backscalex;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox backoffsety;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox backoffsetx;
		private Controls.ConfigurablePictureBox backgroundimage;
	}
}
