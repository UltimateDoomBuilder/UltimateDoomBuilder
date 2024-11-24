namespace CodeImp.DoomBuilder.UDBScript
{
	partial class PreferencesForm
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
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.btnSelectExe = new System.Windows.Forms.Button();
			this.exepath = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label1.AutoSize = true;
			this.tabs = new System.Windows.Forms.TabControl();
			this.filedialog = new System.Windows.Forms.OpenFileDialog();
			this.tabPage1.SuspendLayout();
			this.tabs.SuspendLayout();
			this.SuspendLayout();
			// 
			// tabPage1
			// 
			this.tabPage1.Controls.Add(this.btnSelectExe);
			this.tabPage1.Controls.Add(this.exepath);
			this.tabPage1.Controls.Add(this.label1);
			this.tabPage1.Location = new System.Drawing.Point(4, 22);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage1.Size = new System.Drawing.Size(662, 346);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "UDBScript";
			this.tabPage1.UseVisualStyleBackColor = true;
			// 
			// btnSelectExe
			// 
			this.btnSelectExe.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnSelectExe.Image = global::CodeImp.DoomBuilder.UDBScript.Properties.Resources.Folder;
			this.btnSelectExe.Location = new System.Drawing.Point(633, 24);
			this.btnSelectExe.Name = "btnSelectExe";
			this.btnSelectExe.Size = new System.Drawing.Size(23, 22);
			this.btnSelectExe.TabIndex = 2;
			this.btnSelectExe.UseVisualStyleBackColor = true;
			this.btnSelectExe.Click += new System.EventHandler(this.btnSelectExe_Click);
			// 
			// exepath
			// 
			this.exepath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.exepath.Location = new System.Drawing.Point(117, 25);
			this.exepath.Name = "exepath";
			this.exepath.Size = new System.Drawing.Size(510, 20);
			this.exepath.TabIndex = 1;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(6, 28);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(105, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "External script editor:";
			// 
			// tabs
			// 
			this.tabs.Controls.Add(this.tabPage1);
			this.tabs.Location = new System.Drawing.Point(12, 12);
			this.tabs.Name = "tabs";
			this.tabs.SelectedIndex = 0;
			this.tabs.Size = new System.Drawing.Size(670, 372);
			this.tabs.TabIndex = 0;
			// 
			// filedialog
			// 
			this.filedialog.Filter = "Executables (*.exe, *.cmd, *.bat)|*.exe;*.cmd;*.bat|All files (*.*)|*.*";
			// 
			// PreferencesForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(759, 467);
			this.Controls.Add(this.tabs);
			this.Name = "PreferencesForm";
			this.Text = "PreferencesForm";
			this.tabPage1.ResumeLayout(false);
			this.tabPage1.PerformLayout();
			this.tabs.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.TabControl tabs;
		private System.Windows.Forms.TextBox exepath;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button btnSelectExe;
		private System.Windows.Forms.OpenFileDialog filedialog;
	}
}
