namespace CodeImp.DoomBuilder.Windows
{
    partial class AboutForm
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
            System.Windows.Forms.PictureBox pictureBox5;
            System.Windows.Forms.PictureBox pictureBox3;
            System.Windows.Forms.Label label4;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutForm));
            this.close = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.panel1 = new System.Windows.Forms.Panel();
            this.version = new System.Windows.Forms.Label();
            this.version.AutoSize = true;
            this.copyversion = new System.Windows.Forms.Button();
            this.gitlink = new System.Windows.Forms.LinkLabel();
            this.label2 = new System.Windows.Forms.Label();
            this.label2.AutoSize = true;
            this.zdoomorglink = new System.Windows.Forms.LinkLabel();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.linkLabel4 = new System.Windows.Forms.LinkLabel();
            this.linkLabel3 = new System.Windows.Forms.LinkLabel();
            this.codeimplink = new System.Windows.Forms.LinkLabel();
            this.pictureBox4 = new System.Windows.Forms.PictureBox();
            this.builderlink = new System.Windows.Forms.LinkLabel();
            this.label3 = new System.Windows.Forms.Label();
            this.label3.AutoSize = true;
            this.pictureBox6 = new System.Windows.Forms.PictureBox();
            pictureBox5 = new System.Windows.Forms.PictureBox();
            pictureBox3 = new System.Windows.Forms.PictureBox();
            label4 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(pictureBox5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(pictureBox3)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox6)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox5
            // 
            pictureBox5.BackgroundImage = global::CodeImp.DoomBuilder.Properties.Resources.GZDB_Logo_small;
            pictureBox5.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            pictureBox5.Location = new System.Drawing.Point(12, 10);
            pictureBox5.Name = "pictureBox5";
            pictureBox5.Size = new System.Drawing.Size(226, 80);
            pictureBox5.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            pictureBox5.TabIndex = 19;
            pictureBox5.TabStop = false;
            // 
            // pictureBox3
            // 
            pictureBox3.BackgroundImage = global::CodeImp.DoomBuilder.Properties.Resources.Splash3_small;
            pictureBox3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            pictureBox3.Location = new System.Drawing.Point(12, 129);
            pictureBox3.Name = "pictureBox3";
            pictureBox3.Size = new System.Drawing.Size(226, 80);
            pictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            pictureBox3.TabIndex = 23;
            pictureBox3.TabStop = false;
            // 
            // label4
            // 
            label4.BackColor = System.Drawing.SystemColors.Window;
            label4.FlatStyle = System.Windows.Forms.FlatStyle.System;
            label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            label4.Location = new System.Drawing.Point(244, 129);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(224, 80);
            label4.TabIndex = 24;
            label4.Text = "Doom Builder is designed and programmed by Pascal vd Heiden.\r\nSeveral game config" +
    "urations were written by various members of the Doom community. See the website " +
    "for a complete list of credits.";
            // 
            // close
            // 
            this.close.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.close.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.close.Location = new System.Drawing.Point(472, 298);
            this.close.Name = "close";
            this.close.Size = new System.Drawing.Size(116, 25);
            this.close.TabIndex = 5;
            this.close.Text = "Close";
            this.close.UseVisualStyleBackColor = true;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.Padding = new System.Drawing.Point(24, 3);
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(576, 280);
            this.tabControl1.TabIndex = 11;
            // 
            // tabPage1
            // 
            this.tabPage1.BackgroundImage = global::CodeImp.DoomBuilder.Properties.Resources.AboutBack;
            this.tabPage1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.tabPage1.Controls.Add(this.panel1);
            this.tabPage1.Controls.Add(this.gitlink);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.zdoomorglink);
            this.tabPage1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(568, 254);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "About Ultimate Doom Builder";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Window;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.version);
            this.panel1.Controls.Add(this.copyversion);
            this.panel1.Location = new System.Drawing.Point(17, 16);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(417, 38);
            this.panel1.TabIndex = 19;
            // 
            // version
            // 
            this.version.AutoSize = true;
            this.version.BackColor = System.Drawing.SystemColors.Window;
            this.version.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.version.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.version.Location = new System.Drawing.Point(10, 12);
            this.version.Name = "version";
            this.version.Size = new System.Drawing.Size(210, 13);
            this.version.TabIndex = 11;
            this.version.Text = "Ultimate Doom Builder some version";
            // 
            // copyversion
            // 
            this.copyversion.Location = new System.Drawing.Point(328, 5);
            this.copyversion.Name = "copyversion";
            this.copyversion.Size = new System.Drawing.Size(81, 25);
            this.copyversion.TabIndex = 13;
            this.copyversion.Text = "Copy Version";
            this.copyversion.UseVisualStyleBackColor = true;
            this.copyversion.Click += new System.EventHandler(this.copyversion_Click);
            // 
            // gitlink
            // 
            this.gitlink.AutoSize = true;
            this.gitlink.LinkColor = System.Drawing.Color.White;
            this.gitlink.Location = new System.Drawing.Point(174, 225);
            this.gitlink.Name = "gitlink";
            this.gitlink.Size = new System.Drawing.Size(134, 13);
            this.gitlink.TabIndex = 18;
            this.gitlink.TabStop = true;
            this.gitlink.Text = "Project page at github.com";
            this.gitlink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.gitlink_LinkClicked);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(19, 65);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(343, 143);
            this.label2.TabIndex = 17;
            this.label2.Text = resources.GetString("label2.Text");
            // 
            // zdoomorglink
            // 
            this.zdoomorglink.AutoSize = true;
            this.zdoomorglink.LinkColor = System.Drawing.Color.White;
            this.zdoomorglink.Location = new System.Drawing.Point(19, 225);
            this.zdoomorglink.Name = "zdoomorglink";
            this.zdoomorglink.Size = new System.Drawing.Size(140, 13);
            this.zdoomorglink.TabIndex = 15;
            this.zdoomorglink.TabStop = true;
            this.zdoomorglink.Text = "Official thread at ZDoom.org";
            this.zdoomorglink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.zdoomorglink_LinkClicked);
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.linkLabel4);
            this.tabPage3.Controls.Add(this.linkLabel3);
            this.tabPage3.Controls.Add(pictureBox3);
            this.tabPage3.Controls.Add(this.codeimplink);
            this.tabPage3.Controls.Add(label4);
            this.tabPage3.Controls.Add(this.pictureBox4);
            this.tabPage3.Controls.Add(this.builderlink);
            this.tabPage3.Controls.Add(this.label3);
            this.tabPage3.Controls.Add(pictureBox5);
            this.tabPage3.Controls.Add(this.pictureBox6);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(568, 254);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Based on...";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // linkLabel4
            // 
            this.linkLabel4.ActiveLinkColor = System.Drawing.Color.Firebrick;
            this.linkLabel4.AutoSize = true;
            this.linkLabel4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.linkLabel4.LinkColor = System.Drawing.SystemColors.HotTrack;
            this.linkLabel4.Location = new System.Drawing.Point(158, 99);
            this.linkLabel4.Name = "linkLabel4";
            this.linkLabel4.Size = new System.Drawing.Size(134, 13);
            this.linkLabel4.TabIndex = 29;
            this.linkLabel4.TabStop = true;
            this.linkLabel4.Text = "Project page at github.com";
            this.linkLabel4.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel4_LinkClicked);
            // 
            // linkLabel3
            // 
            this.linkLabel3.ActiveLinkColor = System.Drawing.Color.Firebrick;
            this.linkLabel3.AutoSize = true;
            this.linkLabel3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.linkLabel3.LinkColor = System.Drawing.SystemColors.HotTrack;
            this.linkLabel3.Location = new System.Drawing.Point(12, 99);
            this.linkLabel3.Name = "linkLabel3";
            this.linkLabel3.Size = new System.Drawing.Size(140, 13);
            this.linkLabel3.TabIndex = 28;
            this.linkLabel3.TabStop = true;
            this.linkLabel3.Text = "Official thread at ZDoom.org";
            this.linkLabel3.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel3_LinkClicked);
            // 
            // codeimplink
            // 
            this.codeimplink.ActiveLinkColor = System.Drawing.Color.Firebrick;
            this.codeimplink.AutoSize = true;
            this.codeimplink.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.codeimplink.LinkColor = System.Drawing.SystemColors.HotTrack;
            this.codeimplink.Location = new System.Drawing.Point(13, 233);
            this.codeimplink.Name = "codeimplink";
            this.codeimplink.Size = new System.Drawing.Size(97, 13);
            this.codeimplink.TabIndex = 26;
            this.codeimplink.TabStop = true;
            this.codeimplink.Text = "www.codeimp.com";
            // 
            // pictureBox4
            // 
            this.pictureBox4.BackgroundImage = global::CodeImp.DoomBuilder.Properties.Resources.CLogo;
            this.pictureBox4.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox4.Location = new System.Drawing.Point(474, 129);
            this.pictureBox4.Name = "pictureBox4";
            this.pictureBox4.Size = new System.Drawing.Size(88, 80);
            this.pictureBox4.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox4.TabIndex = 27;
            this.pictureBox4.TabStop = false;
            // 
            // builderlink
            // 
            this.builderlink.ActiveLinkColor = System.Drawing.Color.Firebrick;
            this.builderlink.AutoSize = true;
            this.builderlink.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.builderlink.LinkColor = System.Drawing.SystemColors.HotTrack;
            this.builderlink.Location = new System.Drawing.Point(13, 213);
            this.builderlink.Name = "builderlink";
            this.builderlink.Size = new System.Drawing.Size(114, 13);
            this.builderlink.TabIndex = 25;
            this.builderlink.TabStop = true;
            this.builderlink.Text = "www.doombuilder.com";
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label3.Location = new System.Drawing.Point(244, 10);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(226, 59);
            this.label3.TabIndex = 22;
            this.label3.Text = "GZDoom Builder is designed and programmed by MaxED; GZDoom Builder uses game conf" +
    "igurations created by Gez.";
            // 
            // pictureBox6
            // 
            this.pictureBox6.BackgroundImage = global::CodeImp.DoomBuilder.Properties.Resources.MLogo;
            this.pictureBox6.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox6.Location = new System.Drawing.Point(476, 6);
            this.pictureBox6.Name = "pictureBox6";
            this.pictureBox6.Size = new System.Drawing.Size(86, 88);
            this.pictureBox6.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox6.TabIndex = 21;
            this.pictureBox6.TabStop = false;
            // 
            // AboutForm
            // 
            this.AcceptButton = this.close;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.close;
            this.ClientSize = new System.Drawing.Size(600, 334);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.close);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AboutForm";
            this.Opacity = 0D;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "About";
            ((System.ComponentModel.ISupportInitialize)(pictureBox5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(pictureBox3)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox6)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button close;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.LinkLabel zdoomorglink;
        private System.Windows.Forms.Button copyversion;
        private System.Windows.Forms.Label version;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.LinkLabel gitlink;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.PictureBox pictureBox6;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.LinkLabel codeimplink;
        private System.Windows.Forms.PictureBox pictureBox4;
        private System.Windows.Forms.LinkLabel builderlink;
        private System.Windows.Forms.LinkLabel linkLabel4;
        private System.Windows.Forms.LinkLabel linkLabel3;
    }
}
