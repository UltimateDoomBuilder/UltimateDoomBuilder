namespace CodeImp.DoomBuilder.Windows
{
	partial class PreAndPostCommandsForm
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.label2 = new System.Windows.Forms.Label();
			this.label2.AutoSize = true;
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.reloadpost = new CodeImp.DoomBuilder.Controls.ExternalCommandControl();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.reloadpre = new CodeImp.DoomBuilder.Controls.ExternalCommandControl();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.label1 = new System.Windows.Forms.Label();
			this.label1.AutoSize = true;
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.testpost = new CodeImp.DoomBuilder.Controls.ExternalCommandControl();
			this.groupBox4 = new System.Windows.Forms.GroupBox();
			this.testpre = new CodeImp.DoomBuilder.Controls.ExternalCommandControl();
			this.tabControl1.SuspendLayout();
			this.tabPage1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.tabPage2.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.groupBox4.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnOK
			// 
			this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnOK.Location = new System.Drawing.Point(524, 678);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(75, 23);
			this.btnOK.TabIndex = 3;
			this.btnOK.Text = "OK";
			this.btnOK.UseVisualStyleBackColor = true;
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(605, 678);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 3;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.Add(this.tabPage1);
			this.tabControl1.Controls.Add(this.tabPage2);
			this.tabControl1.Location = new System.Drawing.Point(12, 12);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(670, 657);
			this.tabControl1.TabIndex = 4;
			// 
			// tabPage1
			// 
			this.tabPage1.Controls.Add(this.label2);
			this.tabPage1.Controls.Add(this.groupBox2);
			this.tabPage1.Controls.Add(this.groupBox1);
			this.tabPage1.Location = new System.Drawing.Point(4, 22);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage1.Size = new System.Drawing.Size(662, 631);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "Reload resources commands";
			this.tabPage1.UseVisualStyleBackColor = true;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(6, 15);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(431, 13);
			this.label2.TabIndex = 8;
			this.label2.Text = "The commands will be automatically stored in a temporary CMD file and executed as" +
    " such.";
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.reloadpost);
			this.groupBox2.Location = new System.Drawing.Point(6, 336);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(649, 289);
			this.groupBox2.TabIndex = 4;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Post commands";
			// 
			// reloadpost
			// 
			this.reloadpost.Location = new System.Drawing.Point(6, 19);
			this.reloadpost.Name = "reloadpost";
			this.reloadpost.Size = new System.Drawing.Size(633, 268);
			this.reloadpost.TabIndex = 1;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.reloadpre);
			this.groupBox1.Location = new System.Drawing.Point(6, 41);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(649, 289);
			this.groupBox1.TabIndex = 3;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Pre commands";
			// 
			// reloadpre
			// 
			this.reloadpre.Location = new System.Drawing.Point(7, 19);
			this.reloadpre.Name = "reloadpre";
			this.reloadpre.Size = new System.Drawing.Size(633, 268);
			this.reloadpre.TabIndex = 1;
			// 
			// tabPage2
			// 
			this.tabPage2.Controls.Add(this.label1);
			this.tabPage2.Controls.Add(this.groupBox3);
			this.tabPage2.Controls.Add(this.groupBox4);
			this.tabPage2.Location = new System.Drawing.Point(4, 22);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage2.Size = new System.Drawing.Size(662, 631);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "Test map commands";
			this.tabPage2.UseVisualStyleBackColor = true;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(6, 15);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(637, 13);
			this.label1.TabIndex = 7;
			this.label1.Text = "The commands will be automatically stored in a temporary CMD file and executed as" +
    " such. %1 is the full path to the temporary map file.";
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.testpost);
			this.groupBox3.Enabled = false;
			this.groupBox3.Location = new System.Drawing.Point(6, 336);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(649, 289);
			this.groupBox3.TabIndex = 6;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Post commands";
			// 
			// testpost
			// 
			this.testpost.Location = new System.Drawing.Point(6, 19);
			this.testpost.Name = "testpost";
			this.testpost.Size = new System.Drawing.Size(633, 268);
			this.testpost.TabIndex = 1;
			// 
			// groupBox4
			// 
			this.groupBox4.Controls.Add(this.testpre);
			this.groupBox4.Location = new System.Drawing.Point(6, 41);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Size = new System.Drawing.Size(649, 289);
			this.groupBox4.TabIndex = 5;
			this.groupBox4.TabStop = false;
			this.groupBox4.Text = "Pre commands";
			// 
			// testpre
			// 
			this.testpre.Location = new System.Drawing.Point(7, 19);
			this.testpre.Name = "testpre";
			this.testpre.Size = new System.Drawing.Size(633, 268);
			this.testpre.TabIndex = 1;
			// 
			// PreAndPostCommandsForm
			// 
			this.AcceptButton = this.btnOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(692, 713);
			this.Controls.Add(this.tabControl1);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.btnCancel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "PreAndPostCommandsForm";
			this.Text = "Pre and post commands";
			this.tabControl1.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.tabPage1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.tabPage2.ResumeLayout(false);
			this.tabPage2.PerformLayout();
			this.groupBox3.ResumeLayout(false);
			this.groupBox4.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.GroupBox groupBox2;
		private Controls.ExternalCommandControl reloadpost;
		private System.Windows.Forms.GroupBox groupBox1;
		private Controls.ExternalCommandControl reloadpre;
		private System.Windows.Forms.TabPage tabPage2;
		private System.Windows.Forms.GroupBox groupBox3;
		private Controls.ExternalCommandControl testpost;
		private System.Windows.Forms.GroupBox groupBox4;
		private Controls.ExternalCommandControl testpre;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
	}
}
