namespace CodeImp.DoomBuilder.Controls
{
	partial class ThingBrowserControl
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ThingBrowserControl));
			this.sizecaption = new System.Windows.Forms.Label();
			this.sizecaption.AutoSize = true;
			this.blockingcaption = new System.Windows.Forms.Label();
			this.blockingcaption.AutoSize = true;
			this.positioncaption = new System.Windows.Forms.Label();
			this.positioncaption.AutoSize = true;
			this.typecaption = new System.Windows.Forms.Label();
			this.typecaption.AutoSize = true;
			this.sizelabel = new System.Windows.Forms.Label();
			this.sizelabel.AutoSize = true;
			this.blockinglabel = new System.Windows.Forms.Label();
			this.blockinglabel.AutoSize = true;
			this.positionlabel = new System.Windows.Forms.Label();
			this.positionlabel.AutoSize = true;
			this.thingimages = new System.Windows.Forms.ImageList(this.components);
			this.infopanel = new System.Windows.Forms.Panel();
			this.spritepanel = new System.Windows.Forms.Panel();
			this.classname = new System.Windows.Forms.LinkLabel();
			this.labelclassname = new System.Windows.Forms.Label();
			this.labelclassname.AutoSize = true;
			this.label1 = new System.Windows.Forms.Label();
			this.label1.AutoSize = true;
			this.tbFilter = new System.Windows.Forms.TextBox();
			this.bClear = new System.Windows.Forms.Button();
			this.updatetimer = new System.Windows.Forms.Timer(this.components);
			this.typelist = new CodeImp.DoomBuilder.Controls.MultiSelectTreeview();
			this.spritetex = new CodeImp.DoomBuilder.Controls.ConfigurablePictureBox();
			this.typeid = new CodeImp.DoomBuilder.Controls.NumericTextbox();
			this.infopanel.SuspendLayout();
			this.spritepanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.spritetex)).BeginInit();
			this.SuspendLayout();
			// 
			// sizecaption
			// 
			this.sizecaption.Location = new System.Drawing.Point(0, 38);
			this.sizecaption.Name = "sizecaption";
			this.sizecaption.Size = new System.Drawing.Size(54, 13);
			this.sizecaption.TabIndex = 16;
			this.sizecaption.Text = "Size:";
			this.sizecaption.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// blockingcaption
			// 
			this.blockingcaption.Location = new System.Drawing.Point(0, 54);
			this.blockingcaption.Name = "blockingcaption";
			this.blockingcaption.Size = new System.Drawing.Size(54, 13);
			this.blockingcaption.TabIndex = 14;
			this.blockingcaption.Text = "Blocking:";
			this.blockingcaption.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// positioncaption
			// 
			this.positioncaption.Location = new System.Drawing.Point(0, 22);
			this.positioncaption.Name = "positioncaption";
			this.positioncaption.Size = new System.Drawing.Size(54, 13);
			this.positioncaption.TabIndex = 12;
			this.positioncaption.Text = "Position:";
			this.positioncaption.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// typecaption
			// 
			this.typecaption.Location = new System.Drawing.Point(0, 6);
			this.typecaption.Name = "typecaption";
			this.typecaption.Size = new System.Drawing.Size(54, 13);
			this.typecaption.TabIndex = 10;
			this.typecaption.Text = "Type:";
			this.typecaption.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// sizelabel
			// 
			this.sizelabel.AutoSize = true;
			this.sizelabel.Location = new System.Drawing.Point(60, 38);
			this.sizelabel.Name = "sizelabel";
			this.sizelabel.Size = new System.Drawing.Size(42, 13);
			this.sizelabel.TabIndex = 17;
			this.sizelabel.Text = "16 x 96";
			// 
			// blockinglabel
			// 
			this.blockinglabel.AutoSize = true;
			this.blockinglabel.Location = new System.Drawing.Point(60, 54);
			this.blockinglabel.Name = "blockinglabel";
			this.blockinglabel.Size = new System.Drawing.Size(63, 13);
			this.blockinglabel.TabIndex = 15;
			this.blockinglabel.Text = "True-Height";
			// 
			// positionlabel
			// 
			this.positionlabel.AutoSize = true;
			this.positionlabel.Location = new System.Drawing.Point(60, 22);
			this.positionlabel.Name = "positionlabel";
			this.positionlabel.Size = new System.Drawing.Size(38, 13);
			this.positionlabel.TabIndex = 13;
			this.positionlabel.Text = "Ceiling";
			// 
			// thingimages
			// 
			this.thingimages.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("thingimages.ImageStream")));
			this.thingimages.TransparentColor = System.Drawing.SystemColors.Window;
			this.thingimages.Images.SetKeyName(0, "thing00.png");
			this.thingimages.Images.SetKeyName(1, "thing01.png");
			this.thingimages.Images.SetKeyName(2, "thing02.png");
			this.thingimages.Images.SetKeyName(3, "thing03.png");
			this.thingimages.Images.SetKeyName(4, "thing04.png");
			this.thingimages.Images.SetKeyName(5, "thing05.png");
			this.thingimages.Images.SetKeyName(6, "thing06.png");
			this.thingimages.Images.SetKeyName(7, "thing07.png");
			this.thingimages.Images.SetKeyName(8, "thing08.png");
			this.thingimages.Images.SetKeyName(9, "thing09.png");
			this.thingimages.Images.SetKeyName(10, "thing10.png");
			this.thingimages.Images.SetKeyName(11, "thing11.png");
			this.thingimages.Images.SetKeyName(12, "thing12.png");
			this.thingimages.Images.SetKeyName(13, "thing13.png");
			this.thingimages.Images.SetKeyName(14, "thing14.png");
			this.thingimages.Images.SetKeyName(15, "thing15.png");
			this.thingimages.Images.SetKeyName(16, "thing16.png");
			this.thingimages.Images.SetKeyName(17, "thing17.png");
			this.thingimages.Images.SetKeyName(18, "thing18.png");
			this.thingimages.Images.SetKeyName(19, "thing19.png");
			this.thingimages.Images.SetKeyName(20, "Warning.png");
			this.thingimages.Images.SetKeyName(21, "category00.png");
			this.thingimages.Images.SetKeyName(22, "category01.png");
			this.thingimages.Images.SetKeyName(23, "category02.png");
			this.thingimages.Images.SetKeyName(24, "category03.png");
			this.thingimages.Images.SetKeyName(25, "category04.png");
			this.thingimages.Images.SetKeyName(26, "category05.png");
			this.thingimages.Images.SetKeyName(27, "category06.png");
			this.thingimages.Images.SetKeyName(28, "category07.png");
			this.thingimages.Images.SetKeyName(29, "category08.png");
			this.thingimages.Images.SetKeyName(30, "category09.png");
			this.thingimages.Images.SetKeyName(31, "category10.png");
			this.thingimages.Images.SetKeyName(32, "category11.png");
			this.thingimages.Images.SetKeyName(33, "category12.png");
			this.thingimages.Images.SetKeyName(34, "category13.png");
			this.thingimages.Images.SetKeyName(35, "category14.png");
			this.thingimages.Images.SetKeyName(36, "category15.png");
			this.thingimages.Images.SetKeyName(37, "category16.png");
			this.thingimages.Images.SetKeyName(38, "category17.png");
			this.thingimages.Images.SetKeyName(39, "category18.png");
			this.thingimages.Images.SetKeyName(40, "category19.png");
			this.thingimages.Images.SetKeyName(41, "category_open00.png");
			this.thingimages.Images.SetKeyName(42, "category_open01.png");
			this.thingimages.Images.SetKeyName(43, "category_open02.png");
			this.thingimages.Images.SetKeyName(44, "category_open03.png");
			this.thingimages.Images.SetKeyName(45, "category_open04.png");
			this.thingimages.Images.SetKeyName(46, "category_open05.png");
			this.thingimages.Images.SetKeyName(47, "category_open06.png");
			this.thingimages.Images.SetKeyName(48, "category_open07.png");
			this.thingimages.Images.SetKeyName(49, "category_open08.png");
			this.thingimages.Images.SetKeyName(50, "category_open09.png");
			this.thingimages.Images.SetKeyName(51, "category_open10.png");
			this.thingimages.Images.SetKeyName(52, "category_open11.png");
			this.thingimages.Images.SetKeyName(53, "category_open12.png");
			this.thingimages.Images.SetKeyName(54, "category_open13.png");
			this.thingimages.Images.SetKeyName(55, "category_open14.png");
			this.thingimages.Images.SetKeyName(56, "category_open15.png");
			this.thingimages.Images.SetKeyName(57, "category_open16.png");
			this.thingimages.Images.SetKeyName(58, "category_open17.png");
			this.thingimages.Images.SetKeyName(59, "category_open18.png");
			this.thingimages.Images.SetKeyName(60, "category_open19.png");
			// 
			// infopanel
			// 
			this.infopanel.Controls.Add(this.spritepanel);
			this.infopanel.Controls.Add(this.classname);
			this.infopanel.Controls.Add(this.labelclassname);
			this.infopanel.Controls.Add(this.sizelabel);
			this.infopanel.Controls.Add(this.typecaption);
			this.infopanel.Controls.Add(this.sizecaption);
			this.infopanel.Controls.Add(this.typeid);
			this.infopanel.Controls.Add(this.blockinglabel);
			this.infopanel.Controls.Add(this.positioncaption);
			this.infopanel.Controls.Add(this.blockingcaption);
			this.infopanel.Controls.Add(this.positionlabel);
			this.infopanel.Location = new System.Drawing.Point(0, 233);
			this.infopanel.Name = "infopanel";
			this.infopanel.Size = new System.Drawing.Size(304, 87);
			this.infopanel.TabIndex = 18;
			// 
			// spritepanel
			// 
			this.spritepanel.BackColor = System.Drawing.SystemColors.AppWorkspace;
			this.spritepanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.spritepanel.Controls.Add(this.spritetex);
			this.spritepanel.Location = new System.Drawing.Point(235, 2);
			this.spritepanel.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
			this.spritepanel.Name = "spritepanel";
			this.spritepanel.Size = new System.Drawing.Size(68, 68);
			this.spritepanel.TabIndex = 23;
			// 
			// classname
			// 
			this.classname.ActiveLinkColor = System.Drawing.SystemColors.Highlight;
			this.classname.AutoSize = true;
			this.classname.LinkColor = System.Drawing.SystemColors.HotTrack;
			this.classname.Location = new System.Drawing.Point(60, 70);
			this.classname.Name = "classname";
			this.classname.Size = new System.Drawing.Size(165, 13);
			this.classname.TabIndex = 27;
			this.classname.TabStop = true;
			this.classname.Text = "SuperTurboTurkeyPuncherPlayer";
			this.classname.VisitedLinkColor = System.Drawing.SystemColors.HotTrack;
			this.classname.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.classname_LinkClicked);
			// 
			// labelclassname
			// 
			this.labelclassname.Location = new System.Drawing.Point(0, 70);
			this.labelclassname.Name = "labelclassname";
			this.labelclassname.Size = new System.Drawing.Size(54, 13);
			this.labelclassname.TabIndex = 25;
			this.labelclassname.Text = "Class:";
			this.labelclassname.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(3, 6);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(32, 13);
			this.label1.TabIndex = 19;
			this.label1.Text = "Filter:";
			// 
			// tbFilter
			// 
			this.tbFilter.Location = new System.Drawing.Point(42, 3);
			this.tbFilter.Name = "tbFilter";
			this.tbFilter.Size = new System.Drawing.Size(232, 20);
			this.tbFilter.TabIndex = 20;
			this.tbFilter.TextChanged += new System.EventHandler(this.tbFilter_TextChanged);
			this.tbFilter.KeyUp += new System.Windows.Forms.KeyEventHandler(this.tbFilter_KeyUp);
			// 
			// bClear
			// 
			this.bClear.Image = global::CodeImp.DoomBuilder.Properties.Resources.SearchClear;
			this.bClear.Location = new System.Drawing.Point(277, 1);
			this.bClear.Name = "bClear";
			this.bClear.Size = new System.Drawing.Size(24, 23);
			this.bClear.TabIndex = 21;
			this.bClear.UseVisualStyleBackColor = true;
			this.bClear.Click += new System.EventHandler(this.bClear_Click);
			// 
			// updatetimer
			// 
			this.updatetimer.Tick += new System.EventHandler(this.updatetimer_Tick);
			// 
			// typelist
			// 
			this.typelist.HideSelection = false;
			this.typelist.ImageIndex = 0;
			this.typelist.ImageList = this.thingimages;
			this.typelist.Location = new System.Drawing.Point(0, 28);
			this.typelist.Margin = new System.Windows.Forms.Padding(8, 8, 9, 8);
			this.typelist.Name = "typelist";
			this.typelist.SelectedImageIndex = 0;
			this.typelist.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			this.typelist.SelectionMode = CodeImp.DoomBuilder.Controls.TreeViewSelectionMode.SingleSelect;
			this.typelist.ShowNodeToolTips = true;
			this.typelist.Size = new System.Drawing.Size(304, 203);
			this.typelist.TabIndex = 22;
			this.typelist.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.typelist_MouseDoubleClick);
			this.typelist.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.typelist_BeforeExpand);
			this.typelist.BeforeCollapse += new System.Windows.Forms.TreeViewCancelEventHandler(this.typelist_BeforeCollapse);
			this.typelist.MouseEnter += new System.EventHandler(this.typelist_MouseEnter);
			this.typelist.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.typelist_KeyPress);
			this.typelist.SelectionsChanged += new System.EventHandler(this.typelist_SelectionsChanged);
			// 
			// spritetex
			// 
			this.spritetex.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.Default;
			this.spritetex.Dock = System.Windows.Forms.DockStyle.Fill;
			this.spritetex.Highlighted = false;
			this.spritetex.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
			this.spritetex.Location = new System.Drawing.Point(0, 0);
			this.spritetex.Name = "spritetex";
			this.spritetex.PageUnit = System.Drawing.GraphicsUnit.Pixel;
			this.spritetex.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.None;
			this.spritetex.Size = new System.Drawing.Size(64, 64);
			this.spritetex.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.spritetex.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.Default;
			this.spritetex.TabIndex = 0;
			this.spritetex.TabStop = false;
			// 
			// typeid
			// 
			this.typeid.AllowDecimal = false;
			this.typeid.AllowNegative = false;
			this.typeid.AllowRelative = false;
			this.typeid.ForeColor = System.Drawing.SystemColors.WindowText;
			this.typeid.ImeMode = System.Windows.Forms.ImeMode.Off;
			this.typeid.Location = new System.Drawing.Point(60, 2);
			this.typeid.Name = "typeid";
			this.typeid.Size = new System.Drawing.Size(68, 20);
			this.typeid.TabIndex = 1;
			this.typeid.TextChanged += new System.EventHandler(this.typeid_TextChanged);
			// 
			// ThingBrowserControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.Controls.Add(this.typelist);
			this.Controls.Add(this.bClear);
			this.Controls.Add(this.tbFilter);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.infopanel);
			this.Name = "ThingBrowserControl";
			this.Size = new System.Drawing.Size(304, 320);
			this.Resize += new System.EventHandler(this.ThingBrowserControl_Resize);
			this.infopanel.ResumeLayout(false);
			this.infopanel.PerformLayout();
			this.spritepanel.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.spritetex)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label sizelabel;
		private System.Windows.Forms.Label blockinglabel;
		private System.Windows.Forms.Label positionlabel;
		private NumericTextbox typeid;
		private System.Windows.Forms.ImageList thingimages;
		private System.Windows.Forms.Panel infopanel;
		private System.Windows.Forms.Label sizecaption;
		private System.Windows.Forms.Label blockingcaption;
		private System.Windows.Forms.Label positioncaption;
		private System.Windows.Forms.Label typecaption;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox tbFilter;
		private System.Windows.Forms.Button bClear;
		private CodeImp.DoomBuilder.Controls.MultiSelectTreeview typelist;
		private System.Windows.Forms.Panel spritepanel;
		private System.Windows.Forms.Timer updatetimer;
		private ConfigurablePictureBox spritetex;
		private System.Windows.Forms.LinkLabel classname;
		private System.Windows.Forms.Label labelclassname;
	}
}
