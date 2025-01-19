namespace CodeImp.DoomBuilder.Controls
{
	partial class ExternalCommandControl
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.tbFolder = new System.Windows.Forms.TextBox();
			this.tbCommand = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label1.AutoSize = true;
			this.label2 = new System.Windows.Forms.Label();
			this.label2.AutoSize = true;
			this.btnBrowseFolder = new System.Windows.Forms.Button();
			this.cbAutoclose = new System.Windows.Forms.CheckBox();
			this.cbExitCode = new System.Windows.Forms.CheckBox();
			this.cbStdErr = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// tbFolder
			// 
			this.tbFolder.Location = new System.Drawing.Point(101, 5);
			this.tbFolder.Name = "tbFolder";
			this.tbFolder.Size = new System.Drawing.Size(498, 20);
			this.tbFolder.TabIndex = 0;
			// 
			// tbCommand
			// 
			this.tbCommand.AcceptsReturn = true;
			this.tbCommand.Location = new System.Drawing.Point(101, 31);
			this.tbCommand.Multiline = true;
			this.tbCommand.Name = "tbCommand";
			this.tbCommand.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.tbCommand.Size = new System.Drawing.Size(498, 160);
			this.tbCommand.TabIndex = 1;
			this.tbCommand.WordWrap = false;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(33, 31);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(62, 13);
			this.label1.TabIndex = 2;
			this.label1.Text = "Commands:";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(2, 8);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(93, 13);
			this.label2.TabIndex = 2;
			this.label2.Text = "Working directory:";
			// 
			// btnBrowseFolder
			// 
			this.btnBrowseFolder.Image = global::CodeImp.DoomBuilder.Properties.Resources.Folder;
			this.btnBrowseFolder.Location = new System.Drawing.Point(605, 4);
			this.btnBrowseFolder.Name = "btnBrowseFolder";
			this.btnBrowseFolder.Size = new System.Drawing.Size(23, 23);
			this.btnBrowseFolder.TabIndex = 3;
			this.btnBrowseFolder.UseVisualStyleBackColor = true;
			this.btnBrowseFolder.Click += new System.EventHandler(this.button1_Click);
			// 
			// cbAutoclose
			// 
			this.cbAutoclose.AutoSize = true;
			this.cbAutoclose.Location = new System.Drawing.Point(101, 197);
			this.cbAutoclose.Name = "cbAutoclose";
			this.cbAutoclose.Size = new System.Drawing.Size(243, 17);
			this.cbAutoclose.TabIndex = 4;
			this.cbAutoclose.Text = "Automatically close status window on success";
			this.cbAutoclose.UseVisualStyleBackColor = true;
			// 
			// cbExitCode
			// 
			this.cbExitCode.AutoSize = true;
			this.cbExitCode.Location = new System.Drawing.Point(101, 221);
			this.cbExitCode.Name = "cbExitCode";
			this.cbExitCode.Size = new System.Drawing.Size(175, 17);
			this.cbExitCode.TabIndex = 5;
			this.cbExitCode.Text = "Exit code not equal 0 is an error";
			this.cbExitCode.UseVisualStyleBackColor = true;
			// 
			// cbStdErr
			// 
			this.cbStdErr.AutoSize = true;
			this.cbStdErr.Location = new System.Drawing.Point(101, 244);
			this.cbStdErr.Name = "cbStdErr";
			this.cbStdErr.Size = new System.Drawing.Size(149, 17);
			this.cbStdErr.TabIndex = 5;
			this.cbStdErr.Text = "Writing to stderr is an error";
			this.cbStdErr.UseVisualStyleBackColor = true;
			// 
			// ExternalCommandControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.cbStdErr);
			this.Controls.Add(this.cbExitCode);
			this.Controls.Add(this.cbAutoclose);
			this.Controls.Add(this.btnBrowseFolder);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.tbCommand);
			this.Controls.Add(this.tbFolder);
			this.Name = "ExternalCommandControl";
			this.Size = new System.Drawing.Size(633, 268);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox tbFolder;
		private System.Windows.Forms.TextBox tbCommand;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button btnBrowseFolder;
		private System.Windows.Forms.CheckBox cbAutoclose;
		private System.Windows.Forms.CheckBox cbExitCode;
		private System.Windows.Forms.CheckBox cbStdErr;
	}
}
