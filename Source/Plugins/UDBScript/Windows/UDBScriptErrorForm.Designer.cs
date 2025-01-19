namespace CodeImp.DoomBuilder.UDBScript
{
	partial class UDBScriptErrorForm
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
			this.label1 = new System.Windows.Forms.Label();
			this.label1.AutoSize = true;
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.tbStackTrace = new System.Windows.Forms.TextBox();
			this.tbInternalStackTrace = new System.Windows.Forms.TextBox();
			this.tabControl1.SuspendLayout();
			this.tabPage1.SuspendLayout();
			this.tabPage2.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnOK
			// 
			this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnOK.Location = new System.Drawing.Point(447, 226);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(75, 23);
			this.btnOK.TabIndex = 1;
			this.btnOK.Text = "OK";
			this.btnOK.UseVisualStyleBackColor = true;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(3, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(221, 13);
			this.label1.TabIndex = 2;
			this.label1.Text = "There was an error while executing the script:";
			// 
			// tabControl1
			// 
			this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tabControl1.Controls.Add(this.tabPage1);
			this.tabControl1.Controls.Add(this.tabPage2);
			this.tabControl1.Location = new System.Drawing.Point(6, 38);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(516, 182);
			this.tabControl1.TabIndex = 3;
			// 
			// tabPage1
			// 
			this.tabPage1.Controls.Add(this.tbStackTrace);
			this.tabPage1.Location = new System.Drawing.Point(4, 22);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage1.Size = new System.Drawing.Size(508, 156);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "JavaScript stack trace";
			this.tabPage1.UseVisualStyleBackColor = true;
			// 
			// tabPage2
			// 
			this.tabPage2.Controls.Add(this.tbInternalStackTrace);
			this.tabPage2.Location = new System.Drawing.Point(4, 22);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage2.Size = new System.Drawing.Size(508, 169);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "Internal stack trace";
			this.tabPage2.UseVisualStyleBackColor = true;
			// 
			// tbStackTrace
			// 
			this.tbStackTrace.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tbStackTrace.Location = new System.Drawing.Point(3, 3);
			this.tbStackTrace.Multiline = true;
			this.tbStackTrace.Name = "tbStackTrace";
			this.tbStackTrace.ReadOnly = true;
			this.tbStackTrace.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.tbStackTrace.Size = new System.Drawing.Size(502, 150);
			this.tbStackTrace.TabIndex = 1;
			// 
			// tbInternalStackTrace
			// 
			this.tbInternalStackTrace.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tbInternalStackTrace.Location = new System.Drawing.Point(3, 3);
			this.tbInternalStackTrace.Multiline = true;
			this.tbInternalStackTrace.Name = "tbInternalStackTrace";
			this.tbInternalStackTrace.ReadOnly = true;
			this.tbInternalStackTrace.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.tbInternalStackTrace.Size = new System.Drawing.Size(502, 163);
			this.tbInternalStackTrace.TabIndex = 2;
			// 
			// UDBScriptErrorForm
			// 
			this.AcceptButton = this.btnOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnOK;
			this.ClientSize = new System.Drawing.Size(534, 261);
			this.Controls.Add(this.tabControl1);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.btnOK);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(550, 300);
			this.Name = "UDBScriptErrorForm";
			this.ShowIcon = false;
			this.Text = "Script Error";
			this.tabControl1.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.tabPage1.PerformLayout();
			this.tabPage2.ResumeLayout(false);
			this.tabPage2.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.TextBox tbStackTrace;
		private System.Windows.Forms.TabPage tabPage2;
		private System.Windows.Forms.TextBox tbInternalStackTrace;
	}
}
