namespace CodeImp.DoomBuilder.Windows
{
	partial class ExceptionDialog
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
            this.bQuit = new System.Windows.Forms.Button();
            this.bContinue = new System.Windows.Forms.Button();
            this.errorMessage = new System.Windows.Forms.TextBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.reportlink = new System.Windows.Forms.LinkLabel();
            this.newissue = new System.Windows.Forms.LinkLabel();
            this.bToClipboard = new System.Windows.Forms.Button();
            this.errorDescription = new System.Windows.Forms.Label();
            this.errorDescription.AutoSize = true;
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // bQuit
            // 
            this.bQuit.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bQuit.Image = global::CodeImp.DoomBuilder.Properties.Resources.SearchClear;
            this.bQuit.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bQuit.Location = new System.Drawing.Point(537, 212);
            this.bQuit.Name = "bQuit";
            this.bQuit.Size = new System.Drawing.Size(75, 28);
            this.bQuit.TabIndex = 0;
            this.bQuit.Text = "Quit";
            this.bQuit.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bQuit.UseVisualStyleBackColor = true;
            this.bQuit.Click += new System.EventHandler(this.bQuit_Click);
            // 
            // bContinue
            // 
            this.bContinue.Image = global::CodeImp.DoomBuilder.Properties.Resources.Test;
            this.bContinue.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bContinue.Location = new System.Drawing.Point(446, 212);
            this.bContinue.Name = "bContinue";
            this.bContinue.Size = new System.Drawing.Size(88, 28);
            this.bContinue.TabIndex = 1;
            this.bContinue.Text = "Continue";
            this.bContinue.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bContinue.UseVisualStyleBackColor = true;
            this.bContinue.Click += new System.EventHandler(this.bContinue_Click);
            // 
            // errorMessage
            // 
            this.errorMessage.Location = new System.Drawing.Point(77, 28);
            this.errorMessage.Multiline = true;
            this.errorMessage.Name = "errorMessage";
            this.errorMessage.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.errorMessage.Size = new System.Drawing.Size(535, 151);
            this.errorMessage.TabIndex = 3;
            this.errorMessage.Text = "Stack trace";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::CodeImp.DoomBuilder.Properties.Resources.MCrash;
            this.pictureBox1.Location = new System.Drawing.Point(7, 9);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(64, 64);
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            // 
            // reportlink
            // 
            this.reportlink.AutoSize = true;
            this.reportlink.LinkArea = new System.Windows.Forms.LinkArea(101, 16);
            this.reportlink.LinkColor = System.Drawing.SystemColors.HotTrack;
            this.reportlink.Location = new System.Drawing.Point(75, 211);
            this.reportlink.Name = "reportlink";
            this.reportlink.Size = new System.Drawing.Size(369, 30);
            this.reportlink.TabIndex = 5;
            this.reportlink.TabStop = true;
            this.reportlink.Text = "Including the steps required to reproduce this error, a test map/resource\r\nrequre" +
    "d to trigger it and the error report can immensely help fixing it.";
            this.reportlink.UseCompatibleTextRendering = true;
            this.reportlink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.reportlink_LinkClicked);
            // 
            // newissue
            // 
            this.newissue.AutoSize = true;
            this.newissue.LinkArea = new System.Windows.Forms.LinkArea(51, 21);
            this.newissue.LinkColor = System.Drawing.SystemColors.HotTrack;
            this.newissue.Location = new System.Drawing.Point(75, 188);
            this.newissue.Name = "newissue";
            this.newissue.Size = new System.Drawing.Size(359, 17);
            this.newissue.TabIndex = 8;
            this.newissue.TabStop = true;
            this.newissue.Text = "Help fixing this error by creating an Issue at the GitHub Issues Tracker.";
            this.newissue.UseCompatibleTextRendering = true;
            this.newissue.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.newissue_LinkClicked);
            // 
            // bToClipboard
            // 
            this.bToClipboard.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bToClipboard.Image = global::CodeImp.DoomBuilder.Properties.Resources.Copy;
            this.bToClipboard.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bToClipboard.Location = new System.Drawing.Point(446, 184);
            this.bToClipboard.Name = "bToClipboard";
            this.bToClipboard.Size = new System.Drawing.Size(166, 24);
            this.bToClipboard.TabIndex = 9;
            this.bToClipboard.Text = "Copy to clipboard";
            this.bToClipboard.UseVisualStyleBackColor = true;
            this.bToClipboard.Click += new System.EventHandler(this.bToClipboard_Click);
            // 
            // errorDescription
            // 
            this.errorDescription.AutoSize = true;
            this.errorDescription.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.errorDescription.Location = new System.Drawing.Point(77, 9);
            this.errorDescription.Name = "errorDescription";
            this.errorDescription.Size = new System.Drawing.Size(176, 13);
            this.errorDescription.TabIndex = 10;
            this.errorDescription.Text = "An application error occurred:";
            // 
            // ExceptionDialog
            // 
            this.AcceptButton = this.bContinue;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.bQuit;
            this.ClientSize = new System.Drawing.Size(624, 244);
            this.Controls.Add(this.reportlink);
            this.Controls.Add(this.errorDescription);
            this.Controls.Add(this.bToClipboard);
            this.Controls.Add(this.newissue);
            this.Controls.Add(this.errorMessage);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.bContinue);
            this.Controls.Add(this.bQuit);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ExceptionDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "OH NOES! MOAR ERRORS!";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button bQuit;
		private System.Windows.Forms.Button bContinue;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.TextBox errorMessage;
		private System.Windows.Forms.LinkLabel reportlink;
		private System.Windows.Forms.LinkLabel newissue;
		private System.Windows.Forms.Button bToClipboard;
		private System.Windows.Forms.Label errorDescription;
	}
}
