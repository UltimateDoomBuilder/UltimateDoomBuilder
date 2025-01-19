namespace CodeImp.DoomBuilder.Windows
{
	partial class UpdateForm
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UpdateForm));
			this.cancel = new System.Windows.Forms.Button();
			this.downloadupdate = new System.Windows.Forms.Button();
			this.label = new System.Windows.Forms.Label();
			this.label.AutoSize = true;
			this.label2 = new System.Windows.Forms.Label();
			this.label2.AutoSize = true;
			this.changelog = new System.Windows.Forms.RichTextBox();
			this.hint = new System.Windows.Forms.PictureBox();
			this.ignorethisupdate = new System.Windows.Forms.CheckBox();
			((System.ComponentModel.ISupportInitialize)(this.hint)).BeginInit();
			this.SuspendLayout();
			// 
			// cancel
			// 
			this.cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancel.Location = new System.Drawing.Point(549, 227);
			this.cancel.Name = "cancel";
			this.cancel.Size = new System.Drawing.Size(90, 28);
			this.cancel.TabIndex = 0;
			this.cancel.Text = "Cancel";
			this.cancel.UseVisualStyleBackColor = true;
			this.cancel.Click += new System.EventHandler(this.cancel_Click);
			// 
			// downloadupdate
			// 
			this.downloadupdate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.downloadupdate.Location = new System.Drawing.Point(423, 227);
			this.downloadupdate.Name = "downloadupdate";
			this.downloadupdate.Size = new System.Drawing.Size(120, 28);
			this.downloadupdate.TabIndex = 1;
			this.downloadupdate.Text = "Update";
			this.downloadupdate.UseVisualStyleBackColor = true;
			this.downloadupdate.Click += new System.EventHandler(this.downloadupdate_Click);
			// 
			// label
			// 
			this.label.AutoSize = true;
			this.label.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.label.Location = new System.Drawing.Point(15, 18);
			this.label.Name = "label";
			this.label.Size = new System.Drawing.Size(393, 13);
			this.label.TabIndex = 4;
			this.label.Text = "Ultimate Doom Builder R[rev] is available. Changes since current revision:";
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(33, 226);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(219, 26);
			this.label2.TabIndex = 6;
			this.label2.Text = "Notice: the editor will be closed and restarted\r\nduring the update process.";
			// 
			// changelog
			// 
			this.changelog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.changelog.BackColor = System.Drawing.SystemColors.Window;
			this.changelog.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.changelog.Location = new System.Drawing.Point(12, 41);
			this.changelog.Name = "changelog";
			this.changelog.ReadOnly = true;
			this.changelog.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.changelog.Size = new System.Drawing.Size(627, 175);
			this.changelog.TabIndex = 7;
			this.changelog.Text = "";
			// 
			// hint
			// 
			this.hint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.hint.Image = global::CodeImp.DoomBuilder.Properties.Resources.Lightbulb;
			this.hint.Location = new System.Drawing.Point(13, 231);
			this.hint.Name = "hint";
			this.hint.Size = new System.Drawing.Size(16, 16);
			this.hint.TabIndex = 8;
			this.hint.TabStop = false;
			// 
			// ignorethisupdate
			// 
			this.ignorethisupdate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.ignorethisupdate.AutoSize = true;
			this.ignorethisupdate.Location = new System.Drawing.Point(306, 233);
			this.ignorethisupdate.Name = "ignorethisupdate";
			this.ignorethisupdate.Size = new System.Drawing.Size(111, 17);
			this.ignorethisupdate.TabIndex = 9;
			this.ignorethisupdate.Text = "Ignore this update";
			this.ignorethisupdate.UseVisualStyleBackColor = true;
			// 
			// UpdateForm
			// 
			this.AcceptButton = this.downloadupdate;
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.CancelButton = this.cancel;
			this.ClientSize = new System.Drawing.Size(651, 262);
			this.Controls.Add(this.ignorethisupdate);
			this.Controls.Add(this.hint);
			this.Controls.Add(this.changelog);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label);
			this.Controls.Add(this.downloadupdate);
			this.Controls.Add(this.cancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MinimumSize = new System.Drawing.Size(667, 250);
			this.Name = "UpdateForm";
			this.Opacity = 0;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Newsflash (R[rev])!";
			this.Shown += new System.EventHandler(this.UpdateForm_Shown);
			((System.ComponentModel.ISupportInitialize)(this.hint)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button cancel;
		private System.Windows.Forms.Button downloadupdate;
		private System.Windows.Forms.Label label;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.RichTextBox changelog;
		private System.Windows.Forms.PictureBox hint;
		private System.Windows.Forms.CheckBox ignorethisupdate;
	}
}
