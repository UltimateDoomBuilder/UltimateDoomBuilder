
namespace CodeImp.DoomBuilder.Controls
{
	partial class ToastControl
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
			this.lbText = new System.Windows.Forms.Label();
			this.lbText.AutoSize = true;
			this.icon = new System.Windows.Forms.Panel();
			this.lbTitle = new System.Windows.Forms.Label();
			this.lbTitle.AutoSize = true;
			this.btnClose = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// lbText
			// 
			this.lbText.AutoSize = true;
			this.lbText.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lbText.Location = new System.Drawing.Point(48, 28);
			this.lbText.MaximumSize = new System.Drawing.Size(200, 0);
			this.lbText.Name = "lbText";
			this.lbText.Size = new System.Drawing.Size(46, 18);
			this.lbText.TabIndex = 0;
			this.lbText.Text = "label1";
			// 
			// icon
			// 
			this.icon.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.icon.Location = new System.Drawing.Point(10, 10);
			this.icon.Name = "icon";
			this.icon.Size = new System.Drawing.Size(32, 32);
			this.icon.TabIndex = 1;
			// 
			// lbTitle
			// 
			this.lbTitle.AutoSize = true;
			this.lbTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lbTitle.Location = new System.Drawing.Point(48, 10);
			this.lbTitle.Name = "lbTitle";
			this.lbTitle.Size = new System.Drawing.Size(40, 18);
			this.lbTitle.TabIndex = 2;
			this.lbTitle.Text = "Title";
			// 
			// btnClose
			// 
			this.btnClose.BackgroundImage = global::CodeImp.DoomBuilder.Properties.Resources.Close;
			this.btnClose.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.btnClose.Location = new System.Drawing.Point(385, 3);
			this.btnClose.Name = "btnClose";
			this.btnClose.Size = new System.Drawing.Size(16, 16);
			this.btnClose.TabIndex = 3;
			this.btnClose.UseVisualStyleBackColor = true;
			this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
			// 
			// ToastControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.Control;
			this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.Controls.Add(this.btnClose);
			this.Controls.Add(this.lbTitle);
			this.Controls.Add(this.icon);
			this.Controls.Add(this.lbText);
			this.DoubleBuffered = true;
			this.Margin = new System.Windows.Forms.Padding(6);
			this.Name = "ToastControl";
			this.Size = new System.Drawing.Size(404, 49);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label lbText;
		private System.Windows.Forms.Panel icon;
		private System.Windows.Forms.Label lbTitle;
		private System.Windows.Forms.Button btnClose;
	}
}
