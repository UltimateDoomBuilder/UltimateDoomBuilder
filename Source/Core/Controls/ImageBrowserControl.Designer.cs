namespace CodeImp.DoomBuilder.Controls
{
	partial class ImageBrowserControl
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
				CleanUp();
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
			this.labelMixMode = new System.Windows.Forms.Label();
			this.label = new System.Windows.Forms.Label();
			this.splitter = new System.Windows.Forms.SplitContainer();
			this.list = new CodeImp.DoomBuilder.Controls.ImageSelectorPanel();
			this.centeritem = new System.Windows.Forms.CheckBox();
			this.classicview = new System.Windows.Forms.CheckBox();
			this.objectclear = new System.Windows.Forms.Button();
			this.sizecombo = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.usedtexturesfirst = new System.Windows.Forms.CheckBox();
			this.longtexturenames = new System.Windows.Forms.CheckBox();
			this.filterheightlabel = new System.Windows.Forms.Label();
			this.filterHeight = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.filterwidthlabel = new System.Windows.Forms.Label();
			this.filterWidth = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.texturetypecombo = new System.Windows.Forms.ComboBox();
			this.objectname = new System.Windows.Forms.TextBox();
			this.refreshtimer = new System.Windows.Forms.Timer(this.components);
			((System.ComponentModel.ISupportInitialize)(this.splitter)).BeginInit();
			this.splitter.Panel1.SuspendLayout();
			this.splitter.Panel2.SuspendLayout();
			this.splitter.SuspendLayout();
			this.SuspendLayout();
			// 
			// labelMixMode
			// 
			this.labelMixMode.AutoSize = true;
			this.labelMixMode.Location = new System.Drawing.Point(3, 8);
			this.labelMixMode.Name = "labelMixMode";
			this.labelMixMode.Size = new System.Drawing.Size(37, 13);
			this.labelMixMode.TabIndex = 0;
			this.labelMixMode.Text = "Show:";
			// 
			// label
			// 
			this.label.AutoSize = true;
			this.label.Location = new System.Drawing.Point(164, 9);
			this.label.Name = "label";
			this.label.Size = new System.Drawing.Size(32, 13);
			this.label.TabIndex = 0;
			this.label.Text = "Filter:";
			// 
			// splitter
			// 
			this.splitter.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitter.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
			this.splitter.IsSplitterFixed = true;
			this.splitter.Location = new System.Drawing.Point(0, 0);
			this.splitter.Name = "splitter";
			this.splitter.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitter.Panel1
			// 
			this.splitter.Panel1.Controls.Add(this.list);
			// 
			// splitter.Panel2
			// 
			this.splitter.Panel2.Controls.Add(this.centeritem);
			this.splitter.Panel2.Controls.Add(this.classicview);
			this.splitter.Panel2.Controls.Add(this.objectclear);
			this.splitter.Panel2.Controls.Add(this.sizecombo);
			this.splitter.Panel2.Controls.Add(this.label1);
			this.splitter.Panel2.Controls.Add(this.usedtexturesfirst);
			this.splitter.Panel2.Controls.Add(this.longtexturenames);
			this.splitter.Panel2.Controls.Add(this.filterheightlabel);
			this.splitter.Panel2.Controls.Add(this.filterHeight);
			this.splitter.Panel2.Controls.Add(this.filterwidthlabel);
			this.splitter.Panel2.Controls.Add(this.filterWidth);
			this.splitter.Panel2.Controls.Add(this.texturetypecombo);
			this.splitter.Panel2.Controls.Add(this.labelMixMode);
			this.splitter.Panel2.Controls.Add(this.objectname);
			this.splitter.Panel2.Controls.Add(this.label);
			this.splitter.Size = new System.Drawing.Size(840, 346);
			this.splitter.SplitterDistance = 284;
			this.splitter.TabIndex = 0;
			this.splitter.TabStop = false;
			// 
			// list
			// 
			this.list.AutoScroll = true;
			this.list.BackColor = System.Drawing.Color.White;
			this.list.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.list.ClassicView = false;
			this.list.ContentType = "Textures";
			this.list.Dock = System.Windows.Forms.DockStyle.Fill;
			this.list.HideSelection = false;
			this.list.ImageSize = 128;
			this.list.Location = new System.Drawing.Point(0, 0);
			this.list.MultiSelect = false;
			this.list.Name = "list";
			this.list.Size = new System.Drawing.Size(840, 284);
			this.list.TabIndex = 1;
			this.list.Title = "Default group";
			this.list.UsedTexturesFirst = false;
			this.list.ItemDoubleClicked += new CodeImp.DoomBuilder.Controls.ImageSelectorPanel.ItemSelectedEventHandler(this.list_ItemDoubleClicked);
			this.list.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.list_KeyPress);
			// 
			// centeritem
			// 
			this.centeritem.AutoSize = true;
			this.centeritem.Location = new System.Drawing.Point(510, 34);
			this.centeritem.Name = "centeritem";
			this.centeritem.Size = new System.Drawing.Size(146, 17);
			this.centeritem.TabIndex = 5;
			this.centeritem.TabStop = false;
			this.centeritem.Text = "Vertically center selection";
			this.centeritem.UseVisualStyleBackColor = true;
			this.centeritem.CheckedChanged += new System.EventHandler(this.centeritem_CheckedChanged);
			// 
			// classicview
			// 
			this.classicview.AutoSize = true;
			this.classicview.Location = new System.Drawing.Point(510, 8);
			this.classicview.Name = "classicview";
			this.classicview.Size = new System.Drawing.Size(84, 17);
			this.classicview.TabIndex = 4;
			this.classicview.TabStop = false;
			this.classicview.Text = "Classic view";
			this.classicview.UseVisualStyleBackColor = true;
			this.classicview.CheckedChanged += new System.EventHandler(this.classicview_CheckedChanged);
			// 
			// objectclear
			// 
			this.objectclear.Image = global::CodeImp.DoomBuilder.Properties.Resources.Close;
			this.objectclear.Location = new System.Drawing.Point(330, 4);
			this.objectclear.Name = "objectclear";
			this.objectclear.Size = new System.Drawing.Size(26, 23);
			this.objectclear.TabIndex = 3;
			this.objectclear.TabStop = false;
			this.objectclear.UseVisualStyleBackColor = true;
			this.objectclear.Click += new System.EventHandler(this.objectclear_Click);
			// 
			// sizecombo
			// 
			this.sizecombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.sizecombo.FormattingEnabled = true;
			this.sizecombo.Items.AddRange(new object[] {
            "1:1",
            "64",
            "80",
            "96",
            "128",
            "192",
            "256",
			"384",
			"512"});
			this.sizecombo.Location = new System.Drawing.Point(43, 32);
			this.sizecombo.Margin = new System.Windows.Forms.Padding(3, 3, 6, 3);
			this.sizecombo.Name = "sizecombo";
			this.sizecombo.Size = new System.Drawing.Size(107, 21);
			this.sizecombo.TabIndex = 2;
			this.sizecombo.TabStop = false;
			this.sizecombo.SelectedIndexChanged += new System.EventHandler(this.sizecombo_SelectedIndexChanged);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(10, 35);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(30, 13);
			this.label1.TabIndex = 1;
			this.label1.Text = "Size:";
			// 
			// usedtexturesfirst
			// 
			this.usedtexturesfirst.AutoSize = true;
			this.usedtexturesfirst.Location = new System.Drawing.Point(365, 8);
			this.usedtexturesfirst.Name = "usedtexturesfirst";
			this.usedtexturesfirst.Size = new System.Drawing.Size(139, 17);
			this.usedtexturesfirst.TabIndex = 0;
			this.usedtexturesfirst.TabStop = false;
			this.usedtexturesfirst.Text = "Used textures at the top";
			this.usedtexturesfirst.UseVisualStyleBackColor = true;
			this.usedtexturesfirst.CheckedChanged += new System.EventHandler(this.usedtexturesfirst_CheckedChanged);
			// 
			// longtexturenames
			// 
			this.longtexturenames.AutoSize = true;
			this.longtexturenames.Location = new System.Drawing.Point(365, 34);
			this.longtexturenames.Name = "longtexturenames";
			this.longtexturenames.Size = new System.Drawing.Size(119, 17);
			this.longtexturenames.TabIndex = 0;
			this.longtexturenames.TabStop = false;
			this.longtexturenames.Text = "Long texture names";
			this.longtexturenames.UseVisualStyleBackColor = true;
			this.longtexturenames.CheckedChanged += new System.EventHandler(this.longtexturenames_CheckedChanged);
			// 
			// filterheightlabel
			// 
			this.filterheightlabel.AutoSize = true;
			this.filterheightlabel.Location = new System.Drawing.Point(258, 35);
			this.filterheightlabel.Name = "filterheightlabel";
			this.filterheightlabel.Size = new System.Drawing.Size(41, 13);
			this.filterheightlabel.TabIndex = 0;
			this.filterheightlabel.Text = "Height:";
			// 
			// filterHeight
			// 
			this.filterHeight.AllowDecimal = false;
			this.filterHeight.AllowExpressions = false;
			this.filterHeight.AllowNegative = false;
			this.filterHeight.AllowRelative = false;
			this.filterHeight.ButtonStep = 1;
			this.filterHeight.ButtonStepBig = 10F;
			this.filterHeight.ButtonStepFloat = 1F;
			this.filterHeight.ButtonStepSmall = 0.1F;
			this.filterHeight.ButtonStepsUseModifierKeys = false;
			this.filterHeight.ButtonStepsWrapAround = false;
			this.filterHeight.Location = new System.Drawing.Point(301, 30);
			this.filterHeight.Name = "filterHeight";
			this.filterHeight.Size = new System.Drawing.Size(54, 24);
			this.filterHeight.StepValues = null;
			this.filterHeight.TabIndex = 0;
			this.filterHeight.TabStop = false;
			this.filterHeight.WhenTextChanged += new System.EventHandler(this.filterSize_WhenTextChanged);
			// 
			// filterwidthlabel
			// 
			this.filterwidthlabel.AutoSize = true;
			this.filterwidthlabel.Location = new System.Drawing.Point(159, 35);
			this.filterwidthlabel.Name = "filterwidthlabel";
			this.filterwidthlabel.Size = new System.Drawing.Size(38, 13);
			this.filterwidthlabel.TabIndex = 0;
			this.filterwidthlabel.Text = "Width:";
			// 
			// filterWidth
			// 
			this.filterWidth.AllowDecimal = false;
			this.filterWidth.AllowExpressions = false;
			this.filterWidth.AllowNegative = false;
			this.filterWidth.AllowRelative = false;
			this.filterWidth.ButtonStep = 1;
			this.filterWidth.ButtonStepBig = 10F;
			this.filterWidth.ButtonStepFloat = 1F;
			this.filterWidth.ButtonStepSmall = 0.1F;
			this.filterWidth.ButtonStepsUseModifierKeys = false;
			this.filterWidth.ButtonStepsWrapAround = false;
			this.filterWidth.Location = new System.Drawing.Point(199, 30);
			this.filterWidth.Name = "filterWidth";
			this.filterWidth.Size = new System.Drawing.Size(54, 24);
			this.filterWidth.StepValues = null;
			this.filterWidth.TabIndex = 0;
			this.filterWidth.TabStop = false;
			this.filterWidth.WhenTextChanged += new System.EventHandler(this.filterSize_WhenTextChanged);
			// 
			// texturetypecombo
			// 
			this.texturetypecombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.texturetypecombo.FormattingEnabled = true;
			this.texturetypecombo.Items.AddRange(new object[] {
            "All",
            "Textures",
            "Flats",
            "By selection type"});
			this.texturetypecombo.Location = new System.Drawing.Point(43, 5);
			this.texturetypecombo.Margin = new System.Windows.Forms.Padding(3, 3, 6, 3);
			this.texturetypecombo.Name = "texturetypecombo";
			this.texturetypecombo.Size = new System.Drawing.Size(107, 21);
			this.texturetypecombo.TabIndex = 0;
			this.texturetypecombo.TabStop = false;
			this.texturetypecombo.SelectedIndexChanged += new System.EventHandler(this.texturetypecombo_SelectedIndexChanged);
			// 
			// objectname
			// 
			this.objectname.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
			this.objectname.HideSelection = false;
			this.objectname.Location = new System.Drawing.Point(199, 6);
			this.objectname.Margin = new System.Windows.Forms.Padding(3, 3, 6, 3);
			this.objectname.Name = "objectname";
			this.objectname.Size = new System.Drawing.Size(128, 20);
			this.objectname.TabIndex = 0;
			this.objectname.TabStop = false;
			this.objectname.TextChanged += new System.EventHandler(this.objectname_TextChanged);
			this.objectname.KeyDown += new System.Windows.Forms.KeyEventHandler(this.objectname_KeyDown);
			// 
			// refreshtimer
			// 
			this.refreshtimer.Interval = 500;
			this.refreshtimer.Tick += new System.EventHandler(this.refreshtimer_Tick);
			// 
			// ImageBrowserControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.Controls.Add(this.splitter);
			this.Name = "ImageBrowserControl";
			this.Size = new System.Drawing.Size(840, 346);
			this.splitter.Panel1.ResumeLayout(false);
			this.splitter.Panel2.ResumeLayout(false);
			this.splitter.Panel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitter)).EndInit();
			this.splitter.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.SplitContainer splitter;
		private CodeImp.DoomBuilder.Controls.ImageSelectorPanel list;
		private System.Windows.Forms.Timer refreshtimer;
		private System.Windows.Forms.TextBox objectname;
		private System.Windows.Forms.ComboBox texturetypecombo;
		private System.Windows.Forms.Label label;
		private System.Windows.Forms.Label labelMixMode;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox filterWidth;
		private System.Windows.Forms.Label filterheightlabel;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox filterHeight;
		private System.Windows.Forms.Label filterwidthlabel;
		private System.Windows.Forms.CheckBox longtexturenames;
		private System.Windows.Forms.CheckBox usedtexturesfirst;
		private System.Windows.Forms.ComboBox sizecombo;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button objectclear;
        private System.Windows.Forms.CheckBox classicview;
		private System.Windows.Forms.CheckBox centeritem;
	}
}
