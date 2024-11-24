namespace CodeImp.DoomBuilder.ThreeDFloorMode
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
			this.tabs = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label1.AutoSize = true;
			this.label2 = new System.Windows.Forms.Label();
			this.label2.AutoSize = true;
			this.sectorlabels = new System.Windows.Forms.ComboBox();
			this.slopevertexlabels = new System.Windows.Forms.ComboBox();
			this.label3 = new System.Windows.Forms.Label();
			this.label3.AutoSize = true;
			this.tabs.SuspendLayout();
			this.tabPage1.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// tabs
			// 
			this.tabs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tabs.Controls.Add(this.tabPage1);
			this.tabs.Location = new System.Drawing.Point(12, 12);
			this.tabs.Name = "tabs";
			this.tabs.SelectedIndex = 0;
			this.tabs.Size = new System.Drawing.Size(677, 454);
			this.tabs.TabIndex = 0;
			// 
			// tabPage1
			// 
			this.tabPage1.Controls.Add(this.groupBox1);
			this.tabPage1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tabPage1.Location = new System.Drawing.Point(4, 22);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage1.Size = new System.Drawing.Size(669, 428);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "3D Floor Plugin";
			this.tabPage1.UseVisualStyleBackColor = true;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.slopevertexlabels);
			this.groupBox1.Controls.Add(this.sectorlabels);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Location = new System.Drawing.Point(6, 6);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(305, 94);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Slope Mode";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(34, 22);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(99, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Show sector labels:";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(6, 49);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(127, 13);
			this.label2.TabIndex = 1;
			this.label2.Text = "Show slope vertex labels:";
			// 
			// sectorlabels
			// 
			this.sectorlabels.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.sectorlabels.FormattingEnabled = true;
			this.sectorlabels.Items.AddRange(new object[] {
            "Always",
            "Never",
            "On slope vertex highlight"});
			this.sectorlabels.Location = new System.Drawing.Point(139, 19);
			this.sectorlabels.Name = "sectorlabels";
			this.sectorlabels.Size = new System.Drawing.Size(155, 21);
			this.sectorlabels.TabIndex = 2;
			// 
			// slopevertexlabels
			// 
			this.slopevertexlabels.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.slopevertexlabels.FormattingEnabled = true;
			this.slopevertexlabels.Items.AddRange(new object[] {
            "Always",
            "Never",
            "On slope vertex highlight"});
			this.slopevertexlabels.Location = new System.Drawing.Point(139, 46);
			this.slopevertexlabels.Name = "slopevertexlabels";
			this.slopevertexlabels.Size = new System.Drawing.Size(155, 21);
			this.slopevertexlabels.TabIndex = 3;
			// 
			// label3
			// 
			this.label3.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.label3.Location = new System.Drawing.Point(3, 68);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(299, 23);
			this.label3.TabIndex = 4;
			this.label3.Text = "Holding the Alt key will always show the labels";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// PreferencesForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(701, 478);
			this.Controls.Add(this.tabs);
			this.Name = "PreferencesForm";
			this.Text = "PreferencesForm";
			this.tabs.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TabControl tabs;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.ComboBox slopevertexlabels;
		private System.Windows.Forms.ComboBox sectorlabels;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label3;
	}
}
