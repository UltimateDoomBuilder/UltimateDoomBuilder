namespace CodeImp.DoomBuilder.Controls
{
	partial class TagSelector
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
			this.components = new System.ComponentModel.Container();
			this.label1 = new System.Windows.Forms.Label();
			this.label1.AutoSize = true;
			this.cbTagPicker = new System.Windows.Forms.ComboBox();
			this.newTag = new System.Windows.Forms.Button();
			this.unusedTag = new System.Windows.Forms.Button();
			this.tooltip = new System.Windows.Forms.ToolTip(this.components);
			this.clear = new System.Windows.Forms.Button();
			this.buttons = new System.Windows.Forms.VScrollBar();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.label1.ForeColor = System.Drawing.SystemColors.HotTrack;
			this.label1.Location = new System.Drawing.Point(7, 10);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(29, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Tag:";
			this.tooltip.SetToolTip(this.label1, "Use \">=\" or \"<=\" prefixes to create\r\nascending or descending tags range.\r\nUse \"++" +
					"\" or \"--\" prefixes to increment\r\nor decrement already existing tags \r\nby given v" +
					"alue.");
			// 
			// cbTagPicker
			// 
			this.cbTagPicker.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
			this.cbTagPicker.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
			this.cbTagPicker.FormattingEnabled = true;
			this.cbTagPicker.Location = new System.Drawing.Point(42, 7);
			this.cbTagPicker.Name = "cbTagPicker";
			this.cbTagPicker.Size = new System.Drawing.Size(150, 21);
			this.cbTagPicker.TabIndex = 1;
			// 
			// newTag
			// 
			this.newTag.Location = new System.Drawing.Point(213, 5);
			this.newTag.Name = "newTag";
			this.newTag.Size = new System.Drawing.Size(54, 24);
			this.newTag.TabIndex = 2;
			this.newTag.Text = "New";
			this.tooltip.SetToolTip(this.newTag, "Find a tag, which is not used as a tag or tag action argument\r\nby any map element");
			this.newTag.UseVisualStyleBackColor = true;
			this.newTag.Click += new System.EventHandler(this.newTag_Click);
			// 
			// unusedTag
			// 
			this.unusedTag.Location = new System.Drawing.Point(270, 5);
			this.unusedTag.Name = "unusedTag";
			this.unusedTag.Size = new System.Drawing.Size(54, 24);
			this.unusedTag.TabIndex = 3;
			this.unusedTag.Text = "Unused";
			this.tooltip.SetToolTip(this.unusedTag, "Find a tag, which is not used as a tag\r\nby any map element of this type");
			this.unusedTag.UseVisualStyleBackColor = true;
			this.unusedTag.Click += new System.EventHandler(this.unusedTag_Click);
			// 
			// tooltip
			// 
			this.tooltip.AutomaticDelay = 10;
			this.tooltip.AutoPopDelay = 10000;
			this.tooltip.InitialDelay = 10;
			this.tooltip.ReshowDelay = 100;
			// 
			// clear
			// 
			this.clear.Image = global::CodeImp.DoomBuilder.Properties.Resources.Reset;
			this.clear.Location = new System.Drawing.Point(327, 5);
			this.clear.Name = "clear";
			this.clear.Size = new System.Drawing.Size(26, 24);
			this.clear.TabIndex = 4;
			this.tooltip.SetToolTip(this.clear, "Set tag to 0");
			this.clear.UseVisualStyleBackColor = true;
			this.clear.Click += new System.EventHandler(this.clear_Click);
			// 
			// buttons
			// 
			this.buttons.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttons.LargeChange = 10000;
			this.buttons.Location = new System.Drawing.Point(192, 5);
			this.buttons.Maximum = 10000;
			this.buttons.Minimum = -10000;
			this.buttons.Name = "buttons";
			this.buttons.Size = new System.Drawing.Size(18, 24);
			this.buttons.TabIndex = 5;
			this.buttons.ValueChanged += new System.EventHandler(this.buttons_ValueChanged);
			// 
			// TagSelector
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.Controls.Add(this.buttons);
			this.Controls.Add(this.clear);
			this.Controls.Add(this.unusedTag);
			this.Controls.Add(this.newTag);
			this.Controls.Add(this.cbTagPicker);
			this.Controls.Add(this.label1);
			this.Name = "TagSelector";
			this.Size = new System.Drawing.Size(356, 35);
			this.Resize += new System.EventHandler(this.TagSelector_Resize);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox cbTagPicker;
		private System.Windows.Forms.Button newTag;
		private System.Windows.Forms.Button unusedTag;
		private System.Windows.Forms.ToolTip tooltip;
		private System.Windows.Forms.Button clear;
		private System.Windows.Forms.VScrollBar buttons;
	}
}
