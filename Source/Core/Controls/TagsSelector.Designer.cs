namespace CodeImp.DoomBuilder.Controls
{
	partial class TagsSelector
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			this.curtaglabel = new System.Windows.Forms.Label();
			this.curtaglabel.AutoSize = true;
			this.tagpicker = new System.Windows.Forms.ComboBox();
			this.newtag = new System.Windows.Forms.Button();
			this.unusedtag = new System.Windows.Forms.Button();
			this.tooltip = new System.Windows.Forms.ToolTip(this.components);
			this.removetag = new System.Windows.Forms.Button();
			this.addtag = new System.Windows.Forms.Button();
			this.clear = new System.Windows.Forms.Button();
			this.clearalltags = new System.Windows.Forms.Button();
			this.tagslist = new System.Windows.Forms.LinkLabel();
			this.label1 = new System.Windows.Forms.Label();
			this.label1.AutoSize = true;
			this.buttons = new System.Windows.Forms.VScrollBar();
			this.SuspendLayout();
			// 
			// curtaglabel
			// 
			this.curtaglabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.curtaglabel.ForeColor = System.Drawing.SystemColors.HotTrack;
			this.curtaglabel.Location = new System.Drawing.Point(8, 10);
			this.curtaglabel.Name = "curtaglabel";
			this.curtaglabel.Size = new System.Drawing.Size(52, 13);
			this.curtaglabel.TabIndex = 0;
			this.curtaglabel.Text = "Tag 1:";
			this.curtaglabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
			this.tooltip.SetToolTip(this.curtaglabel, "Use \">=\" or \"<=\" prefixes to create\r\nascending or descending tags range.\r\nUse \"++" +
					"\" or \"--\" prefixes to increment\r\nor decrement already existing tags \r\nby given v" +
					"alue.");
			// 
			// tagpicker
			// 
			this.tagpicker.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
			this.tagpicker.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
			this.tagpicker.FormattingEnabled = true;
			this.tagpicker.Location = new System.Drawing.Point(65, 7);
			this.tagpicker.Name = "tagpicker";
			this.tagpicker.Size = new System.Drawing.Size(210, 21);
			this.tagpicker.TabIndex = 1;
			this.tagpicker.TextChanged += new System.EventHandler(this.tagpicker_TextChanged);
			// 
			// newtag
			// 
			this.newtag.Location = new System.Drawing.Point(299, 5);
			this.newtag.Name = "newtag";
			this.newtag.Size = new System.Drawing.Size(54, 24);
			this.newtag.TabIndex = 2;
			this.newtag.Text = "New";
			this.tooltip.SetToolTip(this.newtag, "Find number, which is not used as a tag\r\nor tag action argument by any map elemen" +
					"t");
			this.newtag.UseVisualStyleBackColor = true;
			this.newtag.Click += new System.EventHandler(this.newtag_Click);
			// 
			// unusedtag
			// 
			this.unusedtag.Location = new System.Drawing.Point(356, 5);
			this.unusedtag.Name = "unusedtag";
			this.unusedtag.Size = new System.Drawing.Size(54, 24);
			this.unusedtag.TabIndex = 3;
			this.unusedtag.Text = "Unused";
			this.tooltip.SetToolTip(this.unusedtag, "Find number, which is not used as a tag \r\nby any map element of this type");
			this.unusedtag.UseVisualStyleBackColor = true;
			this.unusedtag.Click += new System.EventHandler(this.unusedtag_Click);
			// 
			// removetag
			// 
			this.removetag.Image = global::CodeImp.DoomBuilder.Properties.Resources.SearchClear;
			this.removetag.Location = new System.Drawing.Point(413, 33);
			this.removetag.Name = "removetag";
			this.removetag.Size = new System.Drawing.Size(26, 24);
			this.removetag.TabIndex = 7;
			this.tooltip.SetToolTip(this.removetag, "Remove selected tag");
			this.removetag.UseVisualStyleBackColor = true;
			this.removetag.Click += new System.EventHandler(this.removetag_Click);
			// 
			// addtag
			// 
			this.addtag.Image = global::CodeImp.DoomBuilder.Properties.Resources.Add;
			this.addtag.Location = new System.Drawing.Point(384, 33);
			this.addtag.Name = "addtag";
			this.addtag.Size = new System.Drawing.Size(26, 24);
			this.addtag.TabIndex = 8;
			this.tooltip.SetToolTip(this.addtag, "Add new tag");
			this.addtag.UseVisualStyleBackColor = true;
			this.addtag.Click += new System.EventHandler(this.addtag_Click);
			// 
			// clear
			// 
			this.clear.Image = global::CodeImp.DoomBuilder.Properties.Resources.Reset;
			this.clear.Location = new System.Drawing.Point(413, 5);
			this.clear.Name = "clear";
			this.clear.Size = new System.Drawing.Size(26, 24);
			this.clear.TabIndex = 9;
			this.tooltip.SetToolTip(this.clear, "Set current tag to 0");
			this.clear.UseVisualStyleBackColor = true;
			this.clear.Click += new System.EventHandler(this.clear_Click);
			// 
			// clearalltags
			// 
			this.clearalltags.Image = global::CodeImp.DoomBuilder.Properties.Resources.Clear;
			this.clearalltags.Location = new System.Drawing.Point(0, 33);
			this.clearalltags.Name = "clearalltags";
			this.clearalltags.Size = new System.Drawing.Size(24, 24);
			this.clearalltags.TabIndex = 10;
			this.tooltip.SetToolTip(this.clearalltags, "Remove all tags");
			this.clearalltags.UseVisualStyleBackColor = true;
			this.clearalltags.Click += new System.EventHandler(this.clearalltags_Click);
			// 
			// tagslist
			// 
			this.tagslist.AutoSize = true;
			this.tagslist.Location = new System.Drawing.Point(63, 39);
			this.tagslist.Name = "tagslist";
			this.tagslist.Size = new System.Drawing.Size(82, 13);
			this.tagslist.TabIndex = 5;
			this.tagslist.TabStop = true;
			this.tagslist.Text = "12 ,  ??? , [667]";
			this.tagslist.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.tagslist_LinkClicked);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(28, 39);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(34, 13);
			this.label1.TabIndex = 6;
			this.label1.Text = "Tags:";
			// 
			// buttons
			// 
			this.buttons.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttons.LargeChange = 10000;
			this.buttons.Location = new System.Drawing.Point(278, 5);
			this.buttons.Maximum = 10000;
			this.buttons.Minimum = -10000;
			this.buttons.Name = "buttons";
			this.buttons.Size = new System.Drawing.Size(18, 24);
			this.buttons.TabIndex = 11;
			this.buttons.ValueChanged += new System.EventHandler(this.buttons_ValueChanged);
			// 
			// TagsSelector
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.Controls.Add(this.buttons);
			this.Controls.Add(this.clearalltags);
			this.Controls.Add(this.clear);
			this.Controls.Add(this.addtag);
			this.Controls.Add(this.removetag);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.tagslist);
			this.Controls.Add(this.unusedtag);
			this.Controls.Add(this.newtag);
			this.Controls.Add(this.tagpicker);
			this.Controls.Add(this.curtaglabel);
			this.Name = "TagsSelector";
			this.Size = new System.Drawing.Size(442, 60);
			this.Resize += new System.EventHandler(this.TagsSelector_Resize);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label curtaglabel;
		private System.Windows.Forms.ComboBox tagpicker;
		private System.Windows.Forms.Button newtag;
		private System.Windows.Forms.Button unusedtag;
		private System.Windows.Forms.ToolTip tooltip;
		private System.Windows.Forms.LinkLabel tagslist;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button removetag;
		private System.Windows.Forms.Button addtag;
		private System.Windows.Forms.Button clear;
		private System.Windows.Forms.Button clearalltags;
		private System.Windows.Forms.VScrollBar buttons;
	}
}
