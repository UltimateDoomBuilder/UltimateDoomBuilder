namespace CodeImp.DoomBuilder.Windows
{
	partial class ThingsFiltersForm
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ThingsFiltersForm));
			this.listfilters = new System.Windows.Forms.ListView();
			this.columnname = new System.Windows.Forms.ColumnHeader();
			this.imagelist = new System.Windows.Forms.ImageList(this.components);
			this.addfilter = new System.Windows.Forms.Button();
			this.deletefilter = new System.Windows.Forms.Button();
			this.filtergroup = new System.Windows.Forms.GroupBox();
			this.filtermode = new System.Windows.Forms.ComboBox();
			this.label9 = new System.Windows.Forms.Label();
			this.label9.AutoSize = true;
			this.invert = new System.Windows.Forms.CheckBox();
			this.tabs = new System.Windows.Forms.TabControl();
			this.tabbasic = new System.Windows.Forms.TabPage();
			this.label6 = new System.Windows.Forms.Label();
			this.label6.AutoSize = true;
			this.labelzheight = new System.Windows.Forms.Label();
			this.labelzheight.AutoSize = true;
			this.filterzheight = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.browseangle = new System.Windows.Forms.Button();
			this.filterangle = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.filtertype = new CodeImp.DoomBuilder.Controls.ActionSelectorControl();
			this.label5 = new System.Windows.Forms.Label();
			this.label5.AutoSize = true;
			this.browsetype = new System.Windows.Forms.Button();
			this.label4 = new System.Windows.Forms.Label();
			this.label4.AutoSize = true;
			this.filtercategory = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label2.AutoSize = true;
			this.tabflags = new System.Windows.Forms.TabPage();
			this.filterfields = new CodeImp.DoomBuilder.Controls.CheckboxArrayControl();
			this.label3 = new System.Windows.Forms.Label();
			this.label3.AutoSize = true;
			this.tabaction = new System.Windows.Forms.TabPage();
			this.argumentspanel = new System.Windows.Forms.Panel();
			this.label8 = new System.Windows.Forms.Label();
			this.label8.AutoSize = true;
			this.arg2 = new CodeImp.DoomBuilder.Controls.ArgumentBox();
			this.arg1 = new CodeImp.DoomBuilder.Controls.ArgumentBox();
			this.arg0 = new CodeImp.DoomBuilder.Controls.ArgumentBox();
			this.arg3 = new CodeImp.DoomBuilder.Controls.ArgumentBox();
			this.arg4 = new CodeImp.DoomBuilder.Controls.ArgumentBox();
			this.arg1label = new System.Windows.Forms.Label();
			this.arg1label.AutoSize = true;
			this.arg0label = new System.Windows.Forms.Label();
			this.arg0label.AutoSize = true;
			this.arg3label = new System.Windows.Forms.Label();
			this.arg3label.AutoSize = true;
			this.arg2label = new System.Windows.Forms.Label();
			this.arg2label.AutoSize = true;
			this.arg4label = new System.Windows.Forms.Label();
			this.arg4label.AutoSize = true;
			this.filteraction = new CodeImp.DoomBuilder.Controls.ActionSelectorControl();
			this.browseaction = new System.Windows.Forms.Button();
			this.label7 = new System.Windows.Forms.Label();
			this.label7.AutoSize = true;
			this.filtertag = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.labeltag = new System.Windows.Forms.Label();
			this.labeltag.AutoSize = true;
			this.tabcustom = new System.Windows.Forms.TabPage();
			this.fieldslist = new CodeImp.DoomBuilder.Controls.FieldsEditorControl();
			this.filtername = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label1.AutoSize = true;
			this.cancel = new System.Windows.Forms.Button();
			this.apply = new System.Windows.Forms.Button();
			this.filtergroup.SuspendLayout();
			this.tabs.SuspendLayout();
			this.tabbasic.SuspendLayout();
			this.tabflags.SuspendLayout();
			this.tabaction.SuspendLayout();
			this.argumentspanel.SuspendLayout();
			this.tabcustom.SuspendLayout();
			this.SuspendLayout();
			// 
			// listfilters
			// 
			this.listfilters.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)));
			this.listfilters.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnname});
			this.listfilters.FullRowSelect = true;
			this.listfilters.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.listfilters.HideSelection = false;
			this.listfilters.Location = new System.Drawing.Point(12, 12);
			this.listfilters.MultiSelect = false;
			this.listfilters.Name = "listfilters";
			this.listfilters.ShowGroups = false;
			this.listfilters.Size = new System.Drawing.Size(202, 410);
			this.listfilters.SmallImageList = this.imagelist;
			this.listfilters.Sorting = System.Windows.Forms.SortOrder.Ascending;
			this.listfilters.TabIndex = 0;
			this.listfilters.UseCompatibleStateImageBehavior = false;
			this.listfilters.View = System.Windows.Forms.View.Details;
			this.listfilters.SelectedIndexChanged += new System.EventHandler(this.listfilters_SelectedIndexChanged);
			// 
			// columnname
			// 
			this.columnname.Text = "Configuration";
			this.columnname.Width = 177;
			// 
			// imagelist
			// 
			this.imagelist.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imagelist.ImageStream")));
			this.imagelist.TransparentColor = System.Drawing.Color.Transparent;
			this.imagelist.Images.SetKeyName(0, "Warning.png");
			// 
			// addfilter
			// 
			this.addfilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.addfilter.Location = new System.Drawing.Point(12, 428);
			this.addfilter.Name = "addfilter";
			this.addfilter.Size = new System.Drawing.Size(98, 25);
			this.addfilter.TabIndex = 1;
			this.addfilter.Text = "New Filter";
			this.addfilter.UseVisualStyleBackColor = true;
			this.addfilter.Click += new System.EventHandler(this.addfilter_Click);
			// 
			// deletefilter
			// 
			this.deletefilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.deletefilter.Enabled = false;
			this.deletefilter.Location = new System.Drawing.Point(116, 428);
			this.deletefilter.Name = "deletefilter";
			this.deletefilter.Size = new System.Drawing.Size(98, 25);
			this.deletefilter.TabIndex = 2;
			this.deletefilter.Text = "Delete Selected";
			this.deletefilter.UseVisualStyleBackColor = true;
			this.deletefilter.Click += new System.EventHandler(this.deletefilter_Click);
			// 
			// filtergroup
			// 
			this.filtergroup.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.filtergroup.Controls.Add(this.filtermode);
			this.filtergroup.Controls.Add(this.label9);
			this.filtergroup.Controls.Add(this.invert);
			this.filtergroup.Controls.Add(this.tabs);
			this.filtergroup.Controls.Add(this.filtername);
			this.filtergroup.Controls.Add(this.label1);
			this.filtergroup.Enabled = false;
			this.filtergroup.Location = new System.Drawing.Point(220, 6);
			this.filtergroup.Name = "filtergroup";
			this.filtergroup.Size = new System.Drawing.Size(465, 417);
			this.filtergroup.TabIndex = 3;
			this.filtergroup.TabStop = false;
			this.filtergroup.Text = " Selected Filter ";
			// 
			// filtermode
			// 
			this.filtermode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.filtermode.FormattingEnabled = true;
			this.filtermode.Items.AddRange(new object[] {
            "Apply always",
            "Apply in Classic modes only",
            "Apply in Visual modes only"});
			this.filtermode.Location = new System.Drawing.Point(92, 50);
			this.filtermode.Name = "filtermode";
			this.filtermode.Size = new System.Drawing.Size(232, 21);
			this.filtermode.TabIndex = 8;
			this.filtermode.SelectedIndexChanged += new System.EventHandler(this.filtermode_SelectedIndexChanged);
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point(49, 53);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(37, 13);
			this.label9.TabIndex = 7;
			this.label9.Text = "Mode:";
			// 
			// invert
			// 
			this.invert.AutoSize = true;
			this.invert.Location = new System.Drawing.Point(352, 26);
			this.invert.Name = "invert";
			this.invert.Size = new System.Drawing.Size(75, 17);
			this.invert.TabIndex = 6;
			this.invert.Text = "Invert filter";
			this.invert.UseVisualStyleBackColor = true;
			this.invert.CheckedChanged += new System.EventHandler(this.invert_CheckedChanged);
			// 
			// tabs
			// 
			this.tabs.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.tabs.Controls.Add(this.tabbasic);
			this.tabs.Controls.Add(this.tabflags);
			this.tabs.Controls.Add(this.tabaction);
			this.tabs.Controls.Add(this.tabcustom);
			this.tabs.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.tabs.Location = new System.Drawing.Point(6, 82);
			this.tabs.Name = "tabs";
			this.tabs.Padding = new System.Drawing.Point(24, 3);
			this.tabs.SelectedIndex = 0;
			this.tabs.Size = new System.Drawing.Size(453, 329);
			this.tabs.TabIndex = 5;
			// 
			// tabbasic
			// 
			this.tabbasic.Controls.Add(this.label6);
			this.tabbasic.Controls.Add(this.labelzheight);
			this.tabbasic.Controls.Add(this.filterzheight);
			this.tabbasic.Controls.Add(this.browseangle);
			this.tabbasic.Controls.Add(this.filterangle);
			this.tabbasic.Controls.Add(this.filtertype);
			this.tabbasic.Controls.Add(this.label5);
			this.tabbasic.Controls.Add(this.browsetype);
			this.tabbasic.Controls.Add(this.label4);
			this.tabbasic.Controls.Add(this.filtercategory);
			this.tabbasic.Controls.Add(this.label2);
			this.tabbasic.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tabbasic.Location = new System.Drawing.Point(4, 22);
			this.tabbasic.Name = "tabbasic";
			this.tabbasic.Padding = new System.Windows.Forms.Padding(3);
			this.tabbasic.Size = new System.Drawing.Size(445, 303);
			this.tabbasic.TabIndex = 0;
			this.tabbasic.Text = "Properties";
			this.tabbasic.UseVisualStyleBackColor = true;
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(34, 235);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(331, 13);
			this.label6.TabIndex = 15;
			this.label6.Text = "Note:  Clear the input fields which you do not want to use in this filter.";
			// 
			// labelzheight
			// 
			this.labelzheight.AutoSize = true;
			this.labelzheight.Location = new System.Drawing.Point(19, 167);
			this.labelzheight.Name = "labelzheight";
			this.labelzheight.Size = new System.Drawing.Size(88, 13);
			this.labelzheight.TabIndex = 14;
			this.labelzheight.Text = "Filter by Z height:";
			// 
			// filterzheight
			// 
			this.filterzheight.AllowDecimal = false;
			this.filterzheight.AllowNegative = true;
			this.filterzheight.AllowRelative = false;
			this.filterzheight.ButtonStep = 1;
			this.filterzheight.ButtonStepBig = 10F;
			this.filterzheight.ButtonStepFloat = 1F;
			this.filterzheight.ButtonStepSmall = 0.1F;
			this.filterzheight.ButtonStepsUseModifierKeys = false;
			this.filterzheight.ButtonStepsWrapAround = false;
			this.filterzheight.Location = new System.Drawing.Point(111, 162);
			this.filterzheight.Name = "filterzheight";
			this.filterzheight.Size = new System.Drawing.Size(72, 24);
			this.filterzheight.StepValues = null;
			this.filterzheight.TabIndex = 13;
			this.filterzheight.WhenTextChanged += new System.EventHandler(this.filterzheight_WhenTextChanged);
			// 
			// browseangle
			// 
			this.browseangle.Image = global::CodeImp.DoomBuilder.Properties.Resources.Angle;
			this.browseangle.Location = new System.Drawing.Point(189, 119);
			this.browseangle.Name = "browseangle";
			this.browseangle.Size = new System.Drawing.Size(28, 25);
			this.browseangle.TabIndex = 12;
			this.browseangle.Text = " ";
			this.browseangle.UseVisualStyleBackColor = true;
			this.browseangle.Click += new System.EventHandler(this.browseangle_Click);
			// 
			// filterangle
			// 
			this.filterangle.AllowDecimal = false;
			this.filterangle.AllowNegative = true;
			this.filterangle.AllowRelative = false;
			this.filterangle.ButtonStep = 45;
			this.filterangle.ButtonStepBig = 10F;
			this.filterangle.ButtonStepFloat = 1F;
			this.filterangle.ButtonStepSmall = 0.1F;
			this.filterangle.ButtonStepsUseModifierKeys = false;
			this.filterangle.ButtonStepsWrapAround = false;
			this.filterangle.Location = new System.Drawing.Point(111, 119);
			this.filterangle.Name = "filterangle";
			this.filterangle.Size = new System.Drawing.Size(72, 24);
			this.filterangle.StepValues = null;
			this.filterangle.TabIndex = 11;
			this.filterangle.WhenTextChanged += new System.EventHandler(this.filterangle_WhenTextChanged);
			// 
			// filtertype
			// 
			this.filtertype.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.filtertype.BackColor = System.Drawing.Color.Transparent;
			this.filtertype.Cursor = System.Windows.Forms.Cursors.Default;
			this.filtertype.Empty = false;
			this.filtertype.GeneralizedCategories = null;
			this.filtertype.GeneralizedOptions = null;
			this.filtertype.Location = new System.Drawing.Point(111, 76);
			this.filtertype.Name = "filtertype";
			this.filtertype.Size = new System.Drawing.Size(294, 21);
			this.filtertype.TabIndex = 7;
			this.filtertype.Value = 402;
			this.filtertype.ValueChanges += new System.EventHandler(this.filtertype_ValueChanges);
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(32, 124);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(75, 13);
			this.label5.TabIndex = 6;
			this.label5.Text = "Filter by angle:";
			// 
			// browsetype
			// 
			this.browsetype.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.browsetype.Image = global::CodeImp.DoomBuilder.Properties.Resources.List;
			this.browsetype.Location = new System.Drawing.Point(411, 73);
			this.browsetype.Name = "browsetype";
			this.browsetype.Size = new System.Drawing.Size(28, 25);
			this.browsetype.TabIndex = 5;
			this.browsetype.UseVisualStyleBackColor = true;
			this.browsetype.Click += new System.EventHandler(this.browsetype_Click);
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(37, 79);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(69, 13);
			this.label4.TabIndex = 3;
			this.label4.Text = "Filter by type:";
			// 
			// filtercategory
			// 
			this.filtercategory.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.filtercategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.filtercategory.FormattingEnabled = true;
			this.filtercategory.Location = new System.Drawing.Point(111, 30);
			this.filtercategory.Name = "filtercategory";
			this.filtercategory.Size = new System.Drawing.Size(328, 21);
			this.filtercategory.TabIndex = 1;
			this.filtercategory.SelectedIndexChanged += new System.EventHandler(this.filtercategory_SelectedIndexChanged);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(15, 33);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(90, 13);
			this.label2.TabIndex = 2;
			this.label2.Text = "Filter by category:";
			// 
			// tabflags
			// 
			this.tabflags.Controls.Add(this.filterfields);
			this.tabflags.Controls.Add(this.label3);
			this.tabflags.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tabflags.Location = new System.Drawing.Point(4, 22);
			this.tabflags.Name = "tabflags";
			this.tabflags.Padding = new System.Windows.Forms.Padding(3);
			this.tabflags.Size = new System.Drawing.Size(445, 303);
			this.tabflags.TabIndex = 1;
			this.tabflags.Text = "Flags";
			this.tabflags.UseVisualStyleBackColor = true;
			// 
			// filterfields
			// 
			this.filterfields.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.filterfields.AutoScroll = true;
			this.filterfields.Columns = 2;
			this.filterfields.Location = new System.Drawing.Point(20, 39);
			this.filterfields.Name = "filterfields";
			this.filterfields.Size = new System.Drawing.Size(402, 233);
			this.filterfields.TabIndex = 5;
			this.filterfields.VerticalSpacing = 1;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(17, 20);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(85, 13);
			this.label3.TabIndex = 6;
			this.label3.Text = "Filter by settings:";
			// 
			// tabaction
			// 
			this.tabaction.Controls.Add(this.argumentspanel);
			this.tabaction.Controls.Add(this.filteraction);
			this.tabaction.Controls.Add(this.browseaction);
			this.tabaction.Controls.Add(this.label7);
			this.tabaction.Controls.Add(this.filtertag);
			this.tabaction.Controls.Add(this.labeltag);
			this.tabaction.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tabaction.Location = new System.Drawing.Point(4, 22);
			this.tabaction.Name = "tabaction";
			this.tabaction.Padding = new System.Windows.Forms.Padding(3);
			this.tabaction.Size = new System.Drawing.Size(445, 303);
			this.tabaction.TabIndex = 2;
			this.tabaction.Text = "Action";
			this.tabaction.UseVisualStyleBackColor = true;
			// 
			// argumentspanel
			// 
			this.argumentspanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.argumentspanel.Controls.Add(this.label8);
			this.argumentspanel.Controls.Add(this.arg2);
			this.argumentspanel.Controls.Add(this.arg1);
			this.argumentspanel.Controls.Add(this.arg0);
			this.argumentspanel.Controls.Add(this.arg3);
			this.argumentspanel.Controls.Add(this.arg4);
			this.argumentspanel.Controls.Add(this.arg1label);
			this.argumentspanel.Controls.Add(this.arg0label);
			this.argumentspanel.Controls.Add(this.arg3label);
			this.argumentspanel.Controls.Add(this.arg2label);
			this.argumentspanel.Controls.Add(this.arg4label);
			this.argumentspanel.Location = new System.Drawing.Point(6, 70);
			this.argumentspanel.Name = "argumentspanel";
			this.argumentspanel.Size = new System.Drawing.Size(418, 157);
			this.argumentspanel.TabIndex = 17;
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(0, 0);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(98, 13);
			this.label8.TabIndex = 21;
			this.label8.Text = "Filter by arguments:";
			// 
			// arg2
			// 
			this.arg2.Location = new System.Drawing.Point(236, 75);
			this.arg2.Name = "arg2";
			this.arg2.Size = new System.Drawing.Size(128, 24);
			this.arg2.TabIndex = 2;
			this.arg2.Tag = "2";
			this.arg2.Validated += new System.EventHandler(this.arg_Validated);
			// 
			// arg1
			// 
			this.arg1.Location = new System.Drawing.Point(236, 49);
			this.arg1.Name = "arg1";
			this.arg1.Size = new System.Drawing.Size(128, 24);
			this.arg1.TabIndex = 1;
			this.arg1.Tag = "1";
			this.arg1.Validated += new System.EventHandler(this.arg_Validated);
			// 
			// arg0
			// 
			this.arg0.Location = new System.Drawing.Point(236, 23);
			this.arg0.Name = "arg0";
			this.arg0.Size = new System.Drawing.Size(128, 24);
			this.arg0.TabIndex = 0;
			this.arg0.Tag = "0";
			this.arg0.Validated += new System.EventHandler(this.arg_Validated);
			// 
			// arg3
			// 
			this.arg3.Location = new System.Drawing.Point(236, 102);
			this.arg3.Name = "arg3";
			this.arg3.Size = new System.Drawing.Size(128, 24);
			this.arg3.TabIndex = 3;
			this.arg3.Tag = "3";
			this.arg3.Validated += new System.EventHandler(this.arg_Validated);
			// 
			// arg4
			// 
			this.arg4.Location = new System.Drawing.Point(236, 128);
			this.arg4.Name = "arg4";
			this.arg4.Size = new System.Drawing.Size(128, 24);
			this.arg4.TabIndex = 4;
			this.arg4.Tag = "4";
			this.arg4.Validated += new System.EventHandler(this.arg_Validated);
			// 
			// arg1label
			// 
			this.arg1label.Location = new System.Drawing.Point(33, 54);
			this.arg1label.Name = "arg1label";
			this.arg1label.Size = new System.Drawing.Size(197, 14);
			this.arg1label.TabIndex = 14;
			this.arg1label.Text = "Argument 2:";
			this.arg1label.TextAlign = System.Drawing.ContentAlignment.TopRight;
			this.arg1label.UseMnemonic = false;
			// 
			// arg0label
			// 
			this.arg0label.Location = new System.Drawing.Point(33, 28);
			this.arg0label.Name = "arg0label";
			this.arg0label.Size = new System.Drawing.Size(197, 14);
			this.arg0label.TabIndex = 12;
			this.arg0label.Text = "Argument 1:";
			this.arg0label.TextAlign = System.Drawing.ContentAlignment.TopRight;
			this.arg0label.UseMnemonic = false;
			// 
			// arg3label
			// 
			this.arg3label.Location = new System.Drawing.Point(33, 107);
			this.arg3label.Name = "arg3label";
			this.arg3label.Size = new System.Drawing.Size(197, 14);
			this.arg3label.TabIndex = 20;
			this.arg3label.Text = "Argument 4:";
			this.arg3label.TextAlign = System.Drawing.ContentAlignment.TopRight;
			this.arg3label.UseMnemonic = false;
			// 
			// arg2label
			// 
			this.arg2label.Location = new System.Drawing.Point(33, 80);
			this.arg2label.Name = "arg2label";
			this.arg2label.Size = new System.Drawing.Size(197, 14);
			this.arg2label.TabIndex = 18;
			this.arg2label.Text = "Argument 3:";
			this.arg2label.TextAlign = System.Drawing.ContentAlignment.TopRight;
			this.arg2label.UseMnemonic = false;
			// 
			// arg4label
			// 
			this.arg4label.Location = new System.Drawing.Point(33, 133);
			this.arg4label.Name = "arg4label";
			this.arg4label.Size = new System.Drawing.Size(197, 14);
			this.arg4label.TabIndex = 16;
			this.arg4label.Text = "Argument 5:";
			this.arg4label.TextAlign = System.Drawing.ContentAlignment.TopRight;
			this.arg4label.UseMnemonic = false;
			// 
			// filteraction
			// 
			this.filteraction.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.filteraction.BackColor = System.Drawing.Color.Transparent;
			this.filteraction.Cursor = System.Windows.Forms.Cursors.Default;
			this.filteraction.Empty = false;
			this.filteraction.GeneralizedCategories = null;
			this.filteraction.GeneralizedOptions = null;
			this.filteraction.Location = new System.Drawing.Point(90, 30);
			this.filteraction.Name = "filteraction";
			this.filteraction.Size = new System.Drawing.Size(315, 21);
			this.filteraction.TabIndex = 16;
			this.filteraction.Value = 402;
			this.filteraction.ValueChanges += new System.EventHandler(this.filteraction_ValueChanges);
			// 
			// browseaction
			// 
			this.browseaction.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.browseaction.Image = global::CodeImp.DoomBuilder.Properties.Resources.List;
			this.browseaction.Location = new System.Drawing.Point(411, 28);
			this.browseaction.Name = "browseaction";
			this.browseaction.Size = new System.Drawing.Size(28, 25);
			this.browseaction.TabIndex = 15;
			this.browseaction.UseVisualStyleBackColor = true;
			this.browseaction.Click += new System.EventHandler(this.browseaction_Click);
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(6, 34);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(78, 13);
			this.label7.TabIndex = 14;
			this.label7.Text = "Filter by action:";
			// 
			// filtertag
			// 
			this.filtertag.AllowDecimal = false;
			this.filtertag.AllowNegative = true;
			this.filtertag.AllowRelative = false;
			this.filtertag.ButtonStep = 1;
			this.filtertag.ButtonStepBig = 10F;
			this.filtertag.ButtonStepFloat = 1F;
			this.filtertag.ButtonStepSmall = 0.1F;
			this.filtertag.ButtonStepsUseModifierKeys = false;
			this.filtertag.ButtonStepsWrapAround = false;
			this.filtertag.Location = new System.Drawing.Point(90, 244);
			this.filtertag.Name = "filtertag";
			this.filtertag.Size = new System.Drawing.Size(72, 24);
			this.filtertag.StepValues = null;
			this.filtertag.TabIndex = 13;
			this.filtertag.WhenTextChanged += new System.EventHandler(this.filtertag_WhenTextChanged);
			// 
			// labeltag
			// 
			this.labeltag.AutoSize = true;
			this.labeltag.Location = new System.Drawing.Point(20, 249);
			this.labeltag.Name = "labeltag";
			this.labeltag.Size = new System.Drawing.Size(64, 13);
			this.labeltag.TabIndex = 12;
			this.labeltag.Text = "Filter by tag:";
			// 
			// tabcustom
			// 
			this.tabcustom.Controls.Add(this.fieldslist);
			this.tabcustom.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tabcustom.Location = new System.Drawing.Point(4, 22);
			this.tabcustom.Name = "tabcustom";
			this.tabcustom.Padding = new System.Windows.Forms.Padding(3);
			this.tabcustom.Size = new System.Drawing.Size(445, 303);
			this.tabcustom.TabIndex = 3;
			this.tabcustom.Text = "Custom";
			this.tabcustom.UseVisualStyleBackColor = true;
			// 
			// fieldslist
			// 
			this.fieldslist.AllowInsert = true;
			this.fieldslist.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.fieldslist.AutoInsertUserPrefix = false;
			this.fieldslist.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.fieldslist.Location = new System.Drawing.Point(8, 9);
			this.fieldslist.Margin = new System.Windows.Forms.Padding(8, 9, 8, 9);
			this.fieldslist.Name = "fieldslist";
			this.fieldslist.PropertyColumnVisible = true;
			this.fieldslist.PropertyColumnWidth = 150;
			this.fieldslist.ShowFixedFields = true;
			this.fieldslist.Size = new System.Drawing.Size(426, 251);
			this.fieldslist.TabIndex = 2;
			this.fieldslist.TypeColumnVisible = true;
			this.fieldslist.TypeColumnWidth = 100;
			this.fieldslist.ValueColumnVisible = true;
			this.fieldslist.Validated += new System.EventHandler(this.fieldslist_Validated);
			// 
			// filtername
			// 
			this.filtername.Location = new System.Drawing.Point(92, 24);
			this.filtername.MaxLength = 50;
			this.filtername.Name = "filtername";
			this.filtername.Size = new System.Drawing.Size(232, 20);
			this.filtername.TabIndex = 0;
			this.filtername.Enter += new System.EventHandler(this.filtername_Enter);
			this.filtername.Validating += new System.ComponentModel.CancelEventHandler(this.filtername_Validating);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(48, 27);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(38, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Name:";
			// 
			// cancel
			// 
			this.cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancel.Location = new System.Drawing.Point(573, 428);
			this.cancel.Name = "cancel";
			this.cancel.Size = new System.Drawing.Size(112, 25);
			this.cancel.TabIndex = 5;
			this.cancel.Text = "Cancel";
			this.cancel.UseVisualStyleBackColor = true;
			this.cancel.Click += new System.EventHandler(this.cancel_Click);
			// 
			// apply
			// 
			this.apply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.apply.Location = new System.Drawing.Point(455, 428);
			this.apply.Name = "apply";
			this.apply.Size = new System.Drawing.Size(112, 25);
			this.apply.TabIndex = 4;
			this.apply.Text = "OK";
			this.apply.UseVisualStyleBackColor = true;
			this.apply.Click += new System.EventHandler(this.apply_Click);
			// 
			// ThingsFiltersForm
			// 
			this.AcceptButton = this.apply;
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.CancelButton = this.cancel;
			this.ClientSize = new System.Drawing.Size(696, 459);
			this.Controls.Add(this.cancel);
			this.Controls.Add(this.apply);
			this.Controls.Add(this.filtergroup);
			this.Controls.Add(this.deletefilter);
			this.Controls.Add(this.addfilter);
			this.Controls.Add(this.listfilters);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ThingsFiltersForm";
			this.Opacity = 0;
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Things Filters";
			this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.ThingsFiltersForm_HelpRequested);
			this.filtergroup.ResumeLayout(false);
			this.filtergroup.PerformLayout();
			this.tabs.ResumeLayout(false);
			this.tabbasic.ResumeLayout(false);
			this.tabbasic.PerformLayout();
			this.tabflags.ResumeLayout(false);
			this.tabflags.PerformLayout();
			this.tabaction.ResumeLayout(false);
			this.tabaction.PerformLayout();
			this.argumentspanel.ResumeLayout(false);
			this.argumentspanel.PerformLayout();
			this.tabcustom.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ListView listfilters;
		private System.Windows.Forms.ColumnHeader columnname;
		private System.Windows.Forms.Button addfilter;
		private System.Windows.Forms.Button deletefilter;
		private System.Windows.Forms.GroupBox filtergroup;
		private System.Windows.Forms.Button cancel;
		private System.Windows.Forms.Button apply;
		private System.Windows.Forms.TextBox filtername;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ComboBox filtercategory;
		private System.Windows.Forms.TabControl tabs;
		private System.Windows.Forms.TabPage tabbasic;
		private System.Windows.Forms.TabPage tabflags;
		private System.Windows.Forms.Label label4;
		private CodeImp.DoomBuilder.Controls.CheckboxArrayControl filterfields;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button browsetype;
		private System.Windows.Forms.TabPage tabaction;
		private System.Windows.Forms.TabPage tabcustom;
		private System.Windows.Forms.Label label5;
		private CodeImp.DoomBuilder.Controls.ActionSelectorControl filtertype;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox filterangle;
		private System.Windows.Forms.Button browseangle;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox filterzheight;
		private System.Windows.Forms.Label labelzheight;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox filtertag;
		private System.Windows.Forms.Label labeltag;
		private CodeImp.DoomBuilder.Controls.ActionSelectorControl filteraction;
		private System.Windows.Forms.Button browseaction;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Panel argumentspanel;
		private System.Windows.Forms.Label label8;
		private CodeImp.DoomBuilder.Controls.ArgumentBox arg2;
		private CodeImp.DoomBuilder.Controls.ArgumentBox arg1;
		private CodeImp.DoomBuilder.Controls.ArgumentBox arg0;
		private CodeImp.DoomBuilder.Controls.ArgumentBox arg3;
		private CodeImp.DoomBuilder.Controls.ArgumentBox arg4;
		private System.Windows.Forms.Label arg1label;
		private System.Windows.Forms.Label arg0label;
		private System.Windows.Forms.Label arg3label;
		private System.Windows.Forms.Label arg2label;
		private System.Windows.Forms.Label arg4label;
		private CodeImp.DoomBuilder.Controls.FieldsEditorControl fieldslist;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.CheckBox invert;
		private System.Windows.Forms.ComboBox filtermode;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.ImageList imagelist;
	}
}
