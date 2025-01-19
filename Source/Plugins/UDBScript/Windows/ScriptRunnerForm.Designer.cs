namespace CodeImp.DoomBuilder.UDBScript
{
	partial class ScriptRunnerForm
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
			this.progressbar = new System.Windows.Forms.ProgressBar();
			this.lbStatus = new System.Windows.Forms.Label();
			this.lbStatus.AutoSize = true;
			this.btnAction = new System.Windows.Forms.Button();
			this.tbLog = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// progressbar
			// 
			this.progressbar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.progressbar.Location = new System.Drawing.Point(12, 25);
			this.progressbar.Name = "progressbar";
			this.progressbar.Size = new System.Drawing.Size(419, 23);
			this.progressbar.Step = 1;
			this.progressbar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
			this.progressbar.TabIndex = 0;
			// 
			// lbStatus
			// 
			this.lbStatus.AutoSize = true;
			this.lbStatus.Location = new System.Drawing.Point(12, 9);
			this.lbStatus.Name = "lbStatus";
			this.lbStatus.Size = new System.Drawing.Size(84, 13);
			this.lbStatus.TabIndex = 1;
			this.lbStatus.Text = "Running script...";
			// 
			// btnAction
			// 
			this.btnAction.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnAction.Location = new System.Drawing.Point(437, 25);
			this.btnAction.Name = "btnAction";
			this.btnAction.Size = new System.Drawing.Size(75, 23);
			this.btnAction.TabIndex = 2;
			this.btnAction.Text = "Cancel";
			this.btnAction.UseVisualStyleBackColor = true;
			this.btnAction.Click += new System.EventHandler(this.btnAction_Click);
			// 
			// tbLog
			// 
			this.tbLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tbLog.Location = new System.Drawing.Point(12, 54);
			this.tbLog.Multiline = true;
			this.tbLog.Name = "tbLog";
			this.tbLog.ReadOnly = true;
			this.tbLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.tbLog.Size = new System.Drawing.Size(500, 118);
			this.tbLog.TabIndex = 3;
			// 
			// ScriptRunnerForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(524, 184);
			this.ControlBox = false;
			this.Controls.Add(this.tbLog);
			this.Controls.Add(this.btnAction);
			this.Controls.Add(this.lbStatus);
			this.Controls.Add(this.progressbar);
			this.MinimumSize = new System.Drawing.Size(540, 200);
			this.Name = "ScriptRunnerForm";
			this.ShowIcon = false;
			this.Text = "Running script";
			this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ScriptRunnerForm_FormClosed);
			this.Shown += new System.EventHandler(this.ScriptRunnerForm_Shown);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ProgressBar progressbar;
		private System.Windows.Forms.Label lbStatus;
		private System.Windows.Forms.Button btnAction;
		private System.Windows.Forms.TextBox tbLog;
	}
}
