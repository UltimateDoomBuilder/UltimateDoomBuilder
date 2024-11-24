namespace mxd.GZDBUpdater
{
	partial class UpdateBlockedForm
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
			this.accept = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.label1.AutoSize = true;
			this.panel1 = new System.Windows.Forms.Panel();
			this.panel2 = new System.Windows.Forms.Panel();
			this.proceed = new System.Windows.Forms.RadioButton();
			this.cancel = new System.Windows.Forms.RadioButton();
			this.panel1.SuspendLayout();
			this.panel2.SuspendLayout();
			this.SuspendLayout();
			// 
			// accept
			// 
			this.accept.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.accept.Location = new System.Drawing.Point(244, 114);
			this.accept.Name = "accept";
			this.accept.Size = new System.Drawing.Size(120, 28);
			this.accept.TabIndex = 0;
			this.accept.Text = "Continue";
			this.accept.UseVisualStyleBackColor = true;
			this.accept.Click += new System.EventHandler(this.accept_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.label1.Location = new System.Drawing.Point(12, 24);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(245, 13);
			this.label1.TabIndex = 1;
			this.label1.Text = "The editor needs to be closed to apply the update.";
			// 
			// panel1
			// 
			this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.panel1.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.panel1.Controls.Add(this.panel2);
			this.panel1.Controls.Add(this.label1);
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Margin = new System.Windows.Forms.Padding(0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(369, 109);
			this.panel1.TabIndex = 2;
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.proceed);
			this.panel2.Controls.Add(this.cancel);
			this.panel2.Location = new System.Drawing.Point(12, 55);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(354, 51);
			this.panel2.TabIndex = 4;
			// 
			// proceed
			// 
			this.proceed.AutoSize = true;
			this.proceed.Checked = true;
			this.proceed.Location = new System.Drawing.Point(3, 3);
			this.proceed.Name = "proceed";
			this.proceed.Size = new System.Drawing.Size(322, 17);
			this.proceed.TabIndex = 2;
			this.proceed.TabStop = true;
			this.proceed.Text = "Proceed with the update (this will automatically close the editor)";
			this.proceed.UseVisualStyleBackColor = true;
			// 
			// cancel
			// 
			this.cancel.AutoSize = true;
			this.cancel.Location = new System.Drawing.Point(3, 26);
			this.cancel.Name = "cancel";
			this.cancel.Size = new System.Drawing.Size(94, 17);
			this.cancel.TabIndex = 3;
			this.cancel.Text = "Cancel update";
			this.cancel.UseVisualStyleBackColor = true;
			// 
			// UpdateBlockedForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.ClientSize = new System.Drawing.Size(369, 147);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.accept);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "UpdateBlockedForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Ultimate Doom Builder Updater";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.UpdateBlockedForm_FormClosing);
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.panel2.ResumeLayout(false);
			this.panel2.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button accept;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.RadioButton cancel;
		private System.Windows.Forms.RadioButton proceed;
		private System.Windows.Forms.Panel panel2;
	}
}
