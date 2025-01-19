namespace CodeImp.DoomBuilder.BuilderEffects
{
	partial class ObjImportSettingsForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.label2 = new System.Windows.Forms.Label();
			this.label2.AutoSize = true;
			this.nudScale = new System.Windows.Forms.NumericUpDown();
			this.cancel = new System.Windows.Forms.Button();
			this.import = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.label1.AutoSize = true;
			this.tbImportPath = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.label3.AutoSize = true;
			this.axisx = new System.Windows.Forms.RadioButton();
			this.axisy = new System.Windows.Forms.RadioButton();
			this.axisz = new System.Windows.Forms.RadioButton();
			this.panel1 = new System.Windows.Forms.Panel();
			this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.cbusevertexheight = new System.Windows.Forms.CheckBox();
			this.browse = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.nudScale)).BeginInit();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(12, 40);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(37, 13);
			this.label2.TabIndex = 16;
			this.label2.Text = "Scale:";
			// 
			// nudScale
			// 
			this.nudScale.DecimalPlaces = 4;
			this.nudScale.Location = new System.Drawing.Point(55, 38);
			this.nudScale.Maximum = new decimal(new int[] {
            2048,
            0,
            0,
            0});
			this.nudScale.Minimum = new decimal(new int[] {
            2048,
            0,
            0,
            -2147483648});
			this.nudScale.Name = "nudScale";
			this.nudScale.Size = new System.Drawing.Size(94, 20);
			this.nudScale.TabIndex = 15;
			this.nudScale.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// cancel
			// 
			this.cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancel.Location = new System.Drawing.Point(307, 82);
			this.cancel.Name = "cancel";
			this.cancel.Size = new System.Drawing.Size(75, 23);
			this.cancel.TabIndex = 14;
			this.cancel.Text = "Cancel";
			this.cancel.UseVisualStyleBackColor = true;
			this.cancel.Click += new System.EventHandler(this.cancel_Click);
			// 
			// import
			// 
			this.import.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.import.Location = new System.Drawing.Point(226, 82);
			this.import.Name = "import";
			this.import.Size = new System.Drawing.Size(75, 23);
			this.import.TabIndex = 13;
			this.import.Text = "Import";
			this.import.UseVisualStyleBackColor = true;
			this.import.Click += new System.EventHandler(this.import_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(18, 14);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(32, 13);
			this.label1.TabIndex = 12;
			this.label1.Text = "Path:";
			// 
			// tbImportPath
			// 
			this.tbImportPath.Location = new System.Drawing.Point(55, 12);
			this.tbImportPath.Name = "tbImportPath";
			this.tbImportPath.Size = new System.Drawing.Size(299, 20);
			this.tbImportPath.TabIndex = 10;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(183, 40);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(45, 13);
			this.label3.TabIndex = 17;
			this.label3.Text = "Up axis:";
			// 
			// axisx
			// 
			this.axisx.AutoSize = true;
			this.axisx.Location = new System.Drawing.Point(3, 3);
			this.axisx.Name = "axisx";
			this.axisx.Size = new System.Drawing.Size(32, 17);
			this.axisx.TabIndex = 18;
			this.axisx.TabStop = true;
			this.axisx.Text = "X";
			this.axisx.UseVisualStyleBackColor = true;
			// 
			// axisy
			// 
			this.axisy.AutoSize = true;
			this.axisy.Location = new System.Drawing.Point(41, 3);
			this.axisy.Name = "axisy";
			this.axisy.Size = new System.Drawing.Size(32, 17);
			this.axisy.TabIndex = 19;
			this.axisy.TabStop = true;
			this.axisy.Text = "Y";
			this.axisy.UseVisualStyleBackColor = true;
			// 
			// axisz
			// 
			this.axisz.AutoSize = true;
			this.axisz.Location = new System.Drawing.Point(79, 3);
			this.axisz.Name = "axisz";
			this.axisz.Size = new System.Drawing.Size(32, 17);
			this.axisz.TabIndex = 20;
			this.axisz.TabStop = true;
			this.axisz.Text = "Z";
			this.axisz.UseVisualStyleBackColor = true;
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.axisx);
			this.panel1.Controls.Add(this.axisz);
			this.panel1.Controls.Add(this.axisy);
			this.panel1.Location = new System.Drawing.Point(234, 36);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(133, 24);
			this.panel1.TabIndex = 21;
			// 
			// openFileDialog
			// 
			this.openFileDialog.DefaultExt = "obj";
			this.openFileDialog.FileName = "openFileDialog1";
			this.openFileDialog.Filter = "Wavefront obj files|*.obj";
			this.openFileDialog.Title = "Choose .obj file to import:";
			// 
			// cbusevertexheight
			// 
			this.cbusevertexheight.AutoSize = true;
			this.cbusevertexheight.Location = new System.Drawing.Point(55, 64);
			this.cbusevertexheight.Name = "cbusevertexheight";
			this.cbusevertexheight.Size = new System.Drawing.Size(95, 17);
			this.cbusevertexheight.TabIndex = 22;
			this.cbusevertexheight.Text = "Sloped Terrain";
			this.cbusevertexheight.UseVisualStyleBackColor = true;
			// 
			// browse
			// 
			this.browse.Image = global::CodeImp.DoomBuilder.BuilderEffects.Properties.Resources.Folder;
			this.browse.Location = new System.Drawing.Point(360, 10);
			this.browse.Name = "browse";
			this.browse.Size = new System.Drawing.Size(28, 23);
			this.browse.TabIndex = 11;
			this.browse.UseVisualStyleBackColor = true;
			this.browse.Click += new System.EventHandler(this.browse_Click);
			// 
			// ObjImportSettingsForm
			// 
			this.AcceptButton = this.import;
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.CancelButton = this.cancel;
			this.ClientSize = new System.Drawing.Size(394, 109);
			this.Controls.Add(this.cbusevertexheight);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.nudScale);
			this.Controls.Add(this.cancel);
			this.Controls.Add(this.import);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.browse);
			this.Controls.Add(this.tbImportPath);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ObjImportSettingsForm";
			this.Opacity = 0;
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.Text = "Import Wavefront .obj";
			((System.ComponentModel.ISupportInitialize)(this.nudScale)).EndInit();
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.NumericUpDown nudScale;
		private System.Windows.Forms.Button cancel;
		private System.Windows.Forms.Button import;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button browse;
		private System.Windows.Forms.TextBox tbImportPath;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.RadioButton axisx;
		private System.Windows.Forms.RadioButton axisy;
		private System.Windows.Forms.RadioButton axisz;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.OpenFileDialog openFileDialog;
		private System.Windows.Forms.CheckBox cbusevertexheight;
	}
}
