namespace CodeImp.DoomBuilder.Windows
{
	partial class ScriptFindReplaceForm
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ScriptFindReplaceForm));
			this.label1 = new System.Windows.Forms.Label();
			this.findmatchcase = new System.Windows.Forms.CheckBox();
			this.findwholeword = new System.Windows.Forms.CheckBox();
			this.findnextbutton = new System.Windows.Forms.Button();
			this.replaceallbutton = new System.Windows.Forms.Button();
			this.replacebutton = new System.Windows.Forms.Button();
			this.findpreviousbutton = new System.Windows.Forms.Button();
			this.tabs = new System.Windows.Forms.TabControl();
			this.tabfind = new System.Windows.Forms.TabPage();
			this.bookmarkallbutton = new System.Windows.Forms.Button();
			this.findinbox = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.findbox = new System.Windows.Forms.ComboBox();
			this.tabreplace = new System.Windows.Forms.TabPage();
			this.replacebox = new System.Windows.Forms.ComboBox();
			this.label5 = new System.Windows.Forms.Label();
			this.replaceinbox = new System.Windows.Forms.ComboBox();
			this.label3 = new System.Windows.Forms.Label();
			this.replacefindbox = new System.Windows.Forms.ComboBox();
			this.label4 = new System.Windows.Forms.Label();
			this.replacematchcase = new System.Windows.Forms.CheckBox();
			this.replacewholeword = new System.Windows.Forms.CheckBox();
			this.imagelist = new System.Windows.Forms.ImageList(this.components);
			this.tabs.SuspendLayout();
			this.tabfind.SuspendLayout();
			this.tabreplace.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(6, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(56, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Find what:";
			// 
			// findmatchcase
			// 
			this.findmatchcase.AutoSize = true;
			this.findmatchcase.Location = new System.Drawing.Point(9, 116);
			this.findmatchcase.Name = "findmatchcase";
			this.findmatchcase.Size = new System.Drawing.Size(82, 17);
			this.findmatchcase.TabIndex = 2;
			this.findmatchcase.Text = "Match case";
			this.findmatchcase.UseVisualStyleBackColor = true;
			// 
			// findwholeword
			// 
			this.findwholeword.AutoSize = true;
			this.findwholeword.Location = new System.Drawing.Point(97, 116);
			this.findwholeword.Name = "findwholeword";
			this.findwholeword.Size = new System.Drawing.Size(113, 17);
			this.findwholeword.TabIndex = 3;
			this.findwholeword.Text = "Match whole word";
			this.findwholeword.UseVisualStyleBackColor = true;
			// 
			// findnextbutton
			// 
			this.findnextbutton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.findnextbutton.Location = new System.Drawing.Point(103, 185);
			this.findnextbutton.Name = "findnextbutton";
			this.findnextbutton.Size = new System.Drawing.Size(88, 25);
			this.findnextbutton.TabIndex = 5;
			this.findnextbutton.Text = "Find Next";
			this.findnextbutton.UseVisualStyleBackColor = true;
			this.findnextbutton.Click += new System.EventHandler(this.findnextbutton_Click);
			// 
			// replaceallbutton
			// 
			this.replaceallbutton.Location = new System.Drawing.Point(197, 185);
			this.replaceallbutton.Name = "replaceallbutton";
			this.replaceallbutton.Size = new System.Drawing.Size(88, 25);
			this.replaceallbutton.TabIndex = 6;
			this.replaceallbutton.Text = "Replace All";
			this.replaceallbutton.UseVisualStyleBackColor = true;
			this.replaceallbutton.Click += new System.EventHandler(this.replaceallbutton_Click);
			// 
			// replacebutton
			// 
			this.replacebutton.Location = new System.Drawing.Point(103, 185);
			this.replacebutton.Name = "replacebutton";
			this.replacebutton.Size = new System.Drawing.Size(88, 25);
			this.replacebutton.TabIndex = 5;
			this.replacebutton.Text = "Replace";
			this.replacebutton.UseVisualStyleBackColor = true;
			this.replacebutton.Click += new System.EventHandler(this.replacebutton_Click);
			// 
			// findpreviousbutton
			// 
			this.findpreviousbutton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.findpreviousbutton.Location = new System.Drawing.Point(197, 185);
			this.findpreviousbutton.Name = "findpreviousbutton";
			this.findpreviousbutton.Size = new System.Drawing.Size(88, 25);
			this.findpreviousbutton.TabIndex = 6;
			this.findpreviousbutton.Text = "Find Previous";
			this.findpreviousbutton.UseVisualStyleBackColor = true;
			this.findpreviousbutton.Click += new System.EventHandler(this.findpreviousbutton_Click);
			// 
			// tabs
			// 
			this.tabs.Controls.Add(this.tabfind);
			this.tabs.Controls.Add(this.tabreplace);
			this.tabs.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.tabs.ImageList = this.imagelist;
			this.tabs.Location = new System.Drawing.Point(3, 3);
			this.tabs.Margin = new System.Windows.Forms.Padding(0);
			this.tabs.Name = "tabs";
			this.tabs.Padding = new System.Drawing.Point(10, 3);
			this.tabs.SelectedIndex = 0;
			this.tabs.Size = new System.Drawing.Size(299, 242);
			this.tabs.TabIndex = 9;
			this.tabs.SelectedIndexChanged += new System.EventHandler(this.tabs_SelectedIndexChanged);
			// 
			// tabfind
			// 
			this.tabfind.Controls.Add(this.bookmarkallbutton);
			this.tabfind.Controls.Add(this.findinbox);
			this.tabfind.Controls.Add(this.findpreviousbutton);
			this.tabfind.Controls.Add(this.label2);
			this.tabfind.Controls.Add(this.findbox);
			this.tabfind.Controls.Add(this.label1);
			this.tabfind.Controls.Add(this.findnextbutton);
			this.tabfind.Controls.Add(this.findmatchcase);
			this.tabfind.Controls.Add(this.findwholeword);
			this.tabfind.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.tabfind.ImageIndex = 0;
			this.tabfind.Location = new System.Drawing.Point(4, 23);
			this.tabfind.Name = "tabfind";
			this.tabfind.Padding = new System.Windows.Forms.Padding(3, 16, 3, 3);
			this.tabfind.Size = new System.Drawing.Size(291, 215);
			this.tabfind.TabIndex = 0;
			this.tabfind.Text = "Find";
			this.tabfind.UseVisualStyleBackColor = true;
			// 
			// bookmarkallbutton
			// 
			this.bookmarkallbutton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.bookmarkallbutton.Location = new System.Drawing.Point(9, 185);
			this.bookmarkallbutton.Name = "bookmarkallbutton";
			this.bookmarkallbutton.Size = new System.Drawing.Size(88, 25);
			this.bookmarkallbutton.TabIndex = 4;
			this.bookmarkallbutton.Text = "Find Usages";
			this.bookmarkallbutton.UseVisualStyleBackColor = true;
			this.bookmarkallbutton.Click += new System.EventHandler(this.bookmarkallbutton_Click);
			// 
			// findinbox
			// 
			this.findinbox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.findinbox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.findinbox.FormattingEnabled = true;
			this.findinbox.Items.AddRange(new object[] {
            "Current tab",
            "All opened tabs"});
			this.findinbox.Location = new System.Drawing.Point(9, 80);
			this.findinbox.Name = "findinbox";
			this.findinbox.Size = new System.Drawing.Size(276, 21);
			this.findinbox.TabIndex = 1;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(6, 62);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(45, 13);
			this.label2.TabIndex = 2;
			this.label2.Text = "Look in:";
			// 
			// findbox
			// 
			this.findbox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.findbox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
			this.findbox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
			this.findbox.FormattingEnabled = true;
			this.findbox.Location = new System.Drawing.Point(9, 34);
			this.findbox.Name = "findbox";
			this.findbox.Size = new System.Drawing.Size(276, 21);
			this.findbox.TabIndex = 0;
			this.findbox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.findbox_KeyDown);
			// 
			// tabreplace
			// 
			this.tabreplace.Controls.Add(this.replacebox);
			this.tabreplace.Controls.Add(this.label5);
			this.tabreplace.Controls.Add(this.replaceinbox);
			this.tabreplace.Controls.Add(this.replacebutton);
			this.tabreplace.Controls.Add(this.label3);
			this.tabreplace.Controls.Add(this.replacefindbox);
			this.tabreplace.Controls.Add(this.replaceallbutton);
			this.tabreplace.Controls.Add(this.label4);
			this.tabreplace.Controls.Add(this.replacematchcase);
			this.tabreplace.Controls.Add(this.replacewholeword);
			this.tabreplace.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.tabreplace.ImageIndex = 1;
			this.tabreplace.Location = new System.Drawing.Point(4, 23);
			this.tabreplace.Name = "tabreplace";
			this.tabreplace.Padding = new System.Windows.Forms.Padding(3);
			this.tabreplace.Size = new System.Drawing.Size(291, 215);
			this.tabreplace.TabIndex = 1;
			this.tabreplace.Text = "Replace";
			this.tabreplace.UseVisualStyleBackColor = true;
			// 
			// replacebox
			// 
			this.replacebox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.replacebox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
			this.replacebox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
			this.replacebox.FormattingEnabled = true;
			this.replacebox.Location = new System.Drawing.Point(9, 79);
			this.replacebox.Name = "replacebox";
			this.replacebox.Size = new System.Drawing.Size(276, 21);
			this.replacebox.TabIndex = 1;
			this.replacebox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.replacebox_KeyDown);
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(6, 61);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(72, 13);
			this.label5.TabIndex = 10;
			this.label5.Text = "Replace with:";
			// 
			// replaceinbox
			// 
			this.replaceinbox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.replaceinbox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.replaceinbox.FormattingEnabled = true;
			this.replaceinbox.Items.AddRange(new object[] {
            "Current tab",
            "All opened tabs"});
			this.replaceinbox.Location = new System.Drawing.Point(9, 124);
			this.replaceinbox.Name = "replaceinbox";
			this.replaceinbox.Size = new System.Drawing.Size(276, 21);
			this.replaceinbox.TabIndex = 2;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(6, 106);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(45, 13);
			this.label3.TabIndex = 7;
			this.label3.Text = "Look in:";
			// 
			// replacefindbox
			// 
			this.replacefindbox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.replacefindbox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
			this.replacefindbox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
			this.replacefindbox.FormattingEnabled = true;
			this.replacefindbox.Location = new System.Drawing.Point(9, 34);
			this.replacefindbox.Name = "replacefindbox";
			this.replacefindbox.Size = new System.Drawing.Size(276, 21);
			this.replacefindbox.TabIndex = 0;
			this.replacefindbox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.replacefindbox_KeyDown);
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(6, 16);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(56, 13);
			this.label4.TabIndex = 4;
			this.label4.Text = "Find what:";
			// 
			// replacematchcase
			// 
			this.replacematchcase.AutoSize = true;
			this.replacematchcase.Location = new System.Drawing.Point(9, 160);
			this.replacematchcase.Name = "replacematchcase";
			this.replacematchcase.Size = new System.Drawing.Size(82, 17);
			this.replacematchcase.TabIndex = 3;
			this.replacematchcase.Text = "Match case";
			this.replacematchcase.UseVisualStyleBackColor = true;
			// 
			// replacewholeword
			// 
			this.replacewholeword.AutoSize = true;
			this.replacewholeword.Location = new System.Drawing.Point(97, 160);
			this.replacewholeword.Name = "replacewholeword";
			this.replacewholeword.Size = new System.Drawing.Size(113, 17);
			this.replacewholeword.TabIndex = 4;
			this.replacewholeword.Text = "Match whole word";
			this.replacewholeword.UseVisualStyleBackColor = true;
			// 
			// imagelist
			// 
			this.imagelist.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imagelist.ImageStream")));
			this.imagelist.TransparentColor = System.Drawing.Color.Transparent;
			this.imagelist.Images.SetKeyName(0, "Search.png");
			this.imagelist.Images.SetKeyName(1, "Replace.png");
			// 
			// ScriptFindReplaceForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.ClientSize = new System.Drawing.Size(304, 247);
			this.Controls.Add(this.tabs);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ScriptFindReplaceForm";
			this.Opacity = 0D;
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.Text = "Find and Replace";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ScriptFindReplaceForm_FormClosing);
			this.Shown += new System.EventHandler(this.ScriptFindReplaceForm_Shown);
			this.tabs.ResumeLayout(false);
			this.tabfind.ResumeLayout(false);
			this.tabfind.PerformLayout();
			this.tabreplace.ResumeLayout(false);
			this.tabreplace.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.CheckBox findmatchcase;
		private System.Windows.Forms.CheckBox findwholeword;
		private System.Windows.Forms.Button findnextbutton;
		private System.Windows.Forms.Button replaceallbutton;
		private System.Windows.Forms.Button replacebutton;
		private System.Windows.Forms.Button findpreviousbutton;
		private System.Windows.Forms.TabControl tabs;
		private System.Windows.Forms.TabPage tabfind;
		private System.Windows.Forms.TabPage tabreplace;
		private System.Windows.Forms.ImageList imagelist;
		private System.Windows.Forms.ComboBox findinbox;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ComboBox findbox;
		private System.Windows.Forms.ComboBox replacebox;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.ComboBox replaceinbox;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ComboBox replacefindbox;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.CheckBox replacematchcase;
		private System.Windows.Forms.CheckBox replacewholeword;
		private System.Windows.Forms.Button bookmarkallbutton;
	}
}