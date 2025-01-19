namespace CodeImp.DoomBuilder.Controls
{
	partial class HintsPanel
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() 
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HintsPanel));
			this.label1 = new System.Windows.Forms.Label();
			this.label1.AutoSize = true;
			this.hints = new System.Windows.Forms.RichTextBox();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(3, 3);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(110, 13);
			this.label1.TabIndex = 1;
			this.label1.Text = "hidden focus catcher!";
			// 
			// hints
			// 
			this.hints.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.hints.BackColor = System.Drawing.SystemColors.Window;
			this.hints.Cursor = System.Windows.Forms.Cursors.Arrow;
			this.hints.DetectUrls = false;
			this.hints.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.hints.Location = new System.Drawing.Point(6, 3);
			this.hints.Name = "hints";
			this.hints.ReadOnly = true;
			this.hints.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.hints.ShortcutsEnabled = false;
			this.hints.Size = new System.Drawing.Size(381, 542);
			this.hints.TabIndex = 2;
			this.hints.Text = resources.GetString("hints.Text");
			this.hints.Enter += new System.EventHandler(this.hints_Enter);
			// 
			// HintsPanel
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.Controls.Add(this.hints);
			this.Controls.Add(this.label1);
			this.Name = "HintsPanel";
			this.Size = new System.Drawing.Size(390, 548);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.RichTextBox hints;

	}
}
