namespace CodeImp.DoomBuilder.BuilderModes.Interface
{
	partial class SectorDrawingOptionsPanel
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
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.ceiling = new CodeImp.DoomBuilder.Controls.FlatSelectorControl();
			this.floor = new CodeImp.DoomBuilder.Controls.FlatSelectorControl();
			this.cbOverrideFloorTexture = new System.Windows.Forms.CheckBox();
			this.cbOverrideCeilingTexture = new System.Windows.Forms.CheckBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label1.AutoSize = true;
			this.label14 = new System.Windows.Forms.Label();
			this.label14.AutoSize = true;
			this.cbBrightness = new System.Windows.Forms.CheckBox();
			this.brightness = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.floorHeight = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.cbCeilHeight = new System.Windows.Forms.CheckBox();
			this.cbFloorHeight = new System.Windows.Forms.CheckBox();
			this.ceilHeight = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.cbOverrideBottomTexture = new System.Windows.Forms.CheckBox();
			this.cbOverrideMiddleTexture = new System.Windows.Forms.CheckBox();
			this.bottom = new CodeImp.DoomBuilder.Controls.TextureSelectorControl();
			this.cbOverrideTopTexture = new System.Windows.Forms.CheckBox();
			this.middle = new CodeImp.DoomBuilder.Controls.TextureSelectorControl();
			this.top = new CodeImp.DoomBuilder.Controls.TextureSelectorControl();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.groupBox4 = new System.Windows.Forms.GroupBox();
			this.fillall = new System.Windows.Forms.Button();
			this.filllower = new System.Windows.Forms.Button();
			this.fillmiddle = new System.Windows.Forms.Button();
			this.fillupper = new System.Windows.Forms.Button();
			this.fillfloor = new System.Windows.Forms.Button();
			this.fillceiling = new System.Windows.Forms.Button();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.clearlower = new System.Windows.Forms.Button();
			this.clearmiddle = new System.Windows.Forms.Button();
			this.clearupper = new System.Windows.Forms.Button();
			this.clearfloor = new System.Windows.Forms.Button();
			this.clearceiling = new System.Windows.Forms.Button();
			this.clearall = new System.Windows.Forms.Button();
			this.groupBox5 = new System.Windows.Forms.GroupBox();
			this.getsidetexturesfromselection = new System.Windows.Forms.Button();
			this.getheightandbrightnessfromselection = new System.Windows.Forms.Button();
			this.getsectortexturesfromselection = new System.Windows.Forms.Button();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.groupBox4.SuspendLayout();
			this.groupBox5.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.getsectortexturesfromselection);
			this.groupBox1.Controls.Add(this.ceiling);
			this.groupBox1.Controls.Add(this.floor);
			this.groupBox1.Controls.Add(this.cbOverrideFloorTexture);
			this.groupBox1.Controls.Add(this.cbOverrideCeilingTexture);
			this.groupBox1.Location = new System.Drawing.Point(3, 3);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(243, 166);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Sector texture overrides:";
			// 
			// ceiling
			// 
			this.ceiling.Location = new System.Drawing.Point(6, 41);
			this.ceiling.MultipleTextures = false;
			this.ceiling.Name = "ceiling";
			this.ceiling.Size = new System.Drawing.Size(68, 90);
			this.ceiling.TabIndex = 30;
			this.ceiling.TextureName = "";
			this.ceiling.OnValueChanged += new System.EventHandler(this.ceiling_OnValueChanged);
			// 
			// floor
			// 
			this.floor.Location = new System.Drawing.Point(87, 41);
			this.floor.MultipleTextures = false;
			this.floor.Name = "floor";
			this.floor.Size = new System.Drawing.Size(68, 90);
			this.floor.TabIndex = 29;
			this.floor.TextureName = "";
			this.floor.OnValueChanged += new System.EventHandler(this.floor_OnValueChanged);
			// 
			// cbOverrideFloorTexture
			// 
			this.cbOverrideFloorTexture.AutoSize = true;
			this.cbOverrideFloorTexture.Location = new System.Drawing.Point(88, 19);
			this.cbOverrideFloorTexture.Name = "cbOverrideFloorTexture";
			this.cbOverrideFloorTexture.Size = new System.Drawing.Size(49, 17);
			this.cbOverrideFloorTexture.TabIndex = 21;
			this.cbOverrideFloorTexture.Text = "Floor";
			this.cbOverrideFloorTexture.UseVisualStyleBackColor = true;
			this.cbOverrideFloorTexture.CheckedChanged += new System.EventHandler(this.cbOverrideFloorTexture_CheckedChanged);
			// 
			// cbOverrideCeilingTexture
			// 
			this.cbOverrideCeilingTexture.AutoSize = true;
			this.cbOverrideCeilingTexture.Location = new System.Drawing.Point(7, 19);
			this.cbOverrideCeilingTexture.Name = "cbOverrideCeilingTexture";
			this.cbOverrideCeilingTexture.Size = new System.Drawing.Size(57, 17);
			this.cbOverrideCeilingTexture.TabIndex = 20;
			this.cbOverrideCeilingTexture.Text = "Ceiling";
			this.cbOverrideCeilingTexture.UseVisualStyleBackColor = true;
			this.cbOverrideCeilingTexture.CheckedChanged += new System.EventHandler(this.cbOverrideCeilingTexture_CheckedChanged);
			// 
			// groupBox2
			// 
			this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox2.Controls.Add(this.getheightandbrightnessfromselection);
			this.groupBox2.Controls.Add(this.label1);
			this.groupBox2.Controls.Add(this.label14);
			this.groupBox2.Controls.Add(this.cbBrightness);
			this.groupBox2.Controls.Add(this.brightness);
			this.groupBox2.Controls.Add(this.floorHeight);
			this.groupBox2.Controls.Add(this.cbCeilHeight);
			this.groupBox2.Controls.Add(this.cbFloorHeight);
			this.groupBox2.Controls.Add(this.ceilHeight);
			this.groupBox2.Location = new System.Drawing.Point(3, 347);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(243, 138);
			this.groupBox2.TabIndex = 21;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Geometry overrides:";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(182, 53);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(27, 13);
			this.label1.TabIndex = 29;
			this.label1.Text = "m.u.";
			// 
			// label14
			// 
			this.label14.AutoSize = true;
			this.label14.Location = new System.Drawing.Point(182, 24);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(27, 13);
			this.label14.TabIndex = 28;
			this.label14.Text = "m.u.";
			// 
			// cbBrightness
			// 
			this.cbBrightness.AutoSize = true;
			this.cbBrightness.Location = new System.Drawing.Point(7, 82);
			this.cbBrightness.Name = "cbBrightness";
			this.cbBrightness.Size = new System.Drawing.Size(78, 17);
			this.cbBrightness.TabIndex = 27;
			this.cbBrightness.Text = "Brightness:";
			this.cbBrightness.UseVisualStyleBackColor = true;
			this.cbBrightness.CheckedChanged += new System.EventHandler(this.cbBrightness_CheckedChanged);
			// 
			// brightness
			// 
			this.brightness.AllowDecimal = false;
			this.brightness.AllowNegative = false;
			this.brightness.AllowRelative = false;
			this.brightness.ButtonStep = 16;
			this.brightness.ButtonStepFloat = 1F;
			this.brightness.ButtonStepsWrapAround = false;
			this.brightness.Location = new System.Drawing.Point(105, 78);
			this.brightness.Name = "brightness";
			this.brightness.Size = new System.Drawing.Size(72, 24);
			this.brightness.StepValues = null;
			this.brightness.TabIndex = 26;
			this.brightness.WhenTextChanged += new System.EventHandler(this.brightness_WhenTextChanged);
			// 
			// floorHeight
			// 
			this.floorHeight.AllowDecimal = false;
			this.floorHeight.AllowNegative = true;
			this.floorHeight.AllowRelative = false;
			this.floorHeight.ButtonStep = 16;
			this.floorHeight.ButtonStepFloat = 1F;
			this.floorHeight.ButtonStepsWrapAround = false;
			this.floorHeight.Location = new System.Drawing.Point(105, 48);
			this.floorHeight.Name = "floorHeight";
			this.floorHeight.Size = new System.Drawing.Size(72, 24);
			this.floorHeight.StepValues = null;
			this.floorHeight.TabIndex = 25;
			this.floorHeight.WhenTextChanged += new System.EventHandler(this.floorHeight_WhenTextChanged);
			// 
			// cbCeilHeight
			// 
			this.cbCeilHeight.AutoSize = true;
			this.cbCeilHeight.Location = new System.Drawing.Point(7, 23);
			this.cbCeilHeight.Name = "cbCeilHeight";
			this.cbCeilHeight.Size = new System.Drawing.Size(92, 17);
			this.cbCeilHeight.TabIndex = 24;
			this.cbCeilHeight.Text = "Ceiling height:";
			this.cbCeilHeight.UseVisualStyleBackColor = true;
			this.cbCeilHeight.CheckedChanged += new System.EventHandler(this.cbCeilHeight_CheckedChanged);
			// 
			// cbFloorHeight
			// 
			this.cbFloorHeight.AutoSize = true;
			this.cbFloorHeight.Location = new System.Drawing.Point(7, 52);
			this.cbFloorHeight.Name = "cbFloorHeight";
			this.cbFloorHeight.Size = new System.Drawing.Size(84, 17);
			this.cbFloorHeight.TabIndex = 23;
			this.cbFloorHeight.Text = "Floor height:";
			this.cbFloorHeight.UseVisualStyleBackColor = true;
			this.cbFloorHeight.CheckedChanged += new System.EventHandler(this.cbFloorHeight_CheckedChanged);
			// 
			// ceilHeight
			// 
			this.ceilHeight.AllowDecimal = false;
			this.ceilHeight.AllowNegative = true;
			this.ceilHeight.AllowRelative = false;
			this.ceilHeight.ButtonStep = 16;
			this.ceilHeight.ButtonStepFloat = 1F;
			this.ceilHeight.ButtonStepsWrapAround = false;
			this.ceilHeight.Location = new System.Drawing.Point(105, 18);
			this.ceilHeight.Name = "ceilHeight";
			this.ceilHeight.Size = new System.Drawing.Size(72, 24);
			this.ceilHeight.StepValues = null;
			this.ceilHeight.TabIndex = 11;
			this.ceilHeight.WhenTextChanged += new System.EventHandler(this.ceilHeight_WhenTextChanged);
			// 
			// cbOverrideBottomTexture
			// 
			this.cbOverrideBottomTexture.AutoSize = true;
			this.cbOverrideBottomTexture.Location = new System.Drawing.Point(170, 19);
			this.cbOverrideBottomTexture.Name = "cbOverrideBottomTexture";
			this.cbOverrideBottomTexture.Size = new System.Drawing.Size(55, 17);
			this.cbOverrideBottomTexture.TabIndex = 28;
			this.cbOverrideBottomTexture.Text = "Lower";
			this.cbOverrideBottomTexture.UseVisualStyleBackColor = true;
			this.cbOverrideBottomTexture.CheckedChanged += new System.EventHandler(this.cbOverrideBottomTexture_CheckedChanged);
			// 
			// cbOverrideMiddleTexture
			// 
			this.cbOverrideMiddleTexture.AutoSize = true;
			this.cbOverrideMiddleTexture.Location = new System.Drawing.Point(88, 19);
			this.cbOverrideMiddleTexture.Name = "cbOverrideMiddleTexture";
			this.cbOverrideMiddleTexture.Size = new System.Drawing.Size(57, 17);
			this.cbOverrideMiddleTexture.TabIndex = 27;
			this.cbOverrideMiddleTexture.Text = "Middle";
			this.cbOverrideMiddleTexture.UseVisualStyleBackColor = true;
			this.cbOverrideMiddleTexture.CheckedChanged += new System.EventHandler(this.cbOverrideMiddleTexture_CheckedChanged);
			// 
			// bottom
			// 
			this.bottom.Location = new System.Drawing.Point(168, 41);
			this.bottom.MultipleTextures = false;
			this.bottom.Name = "bottom";
			this.bottom.Required = false;
			this.bottom.Size = new System.Drawing.Size(68, 90);
			this.bottom.TabIndex = 25;
			this.bottom.TextureName = "";
			this.bottom.OnValueChanged += new System.EventHandler(this.bottom_OnValueChanged);
			// 
			// cbOverrideTopTexture
			// 
			this.cbOverrideTopTexture.AutoSize = true;
			this.cbOverrideTopTexture.Location = new System.Drawing.Point(7, 19);
			this.cbOverrideTopTexture.Name = "cbOverrideTopTexture";
			this.cbOverrideTopTexture.Size = new System.Drawing.Size(55, 17);
			this.cbOverrideTopTexture.TabIndex = 26;
			this.cbOverrideTopTexture.Text = "Upper";
			this.cbOverrideTopTexture.UseVisualStyleBackColor = true;
			this.cbOverrideTopTexture.CheckedChanged += new System.EventHandler(this.cbOverrideTopTexture_CheckedChanged);
			// 
			// middle
			// 
			this.middle.Location = new System.Drawing.Point(87, 41);
			this.middle.MultipleTextures = false;
			this.middle.Name = "middle";
			this.middle.Required = false;
			this.middle.Size = new System.Drawing.Size(68, 90);
			this.middle.TabIndex = 24;
			this.middle.TextureName = "";
			this.middle.OnValueChanged += new System.EventHandler(this.middle_OnValueChanged);
			// 
			// top
			// 
			this.top.Location = new System.Drawing.Point(6, 41);
			this.top.MultipleTextures = false;
			this.top.Name = "top";
			this.top.Required = false;
			this.top.Size = new System.Drawing.Size(68, 90);
			this.top.TabIndex = 23;
			this.top.TextureName = "";
			this.top.OnValueChanged += new System.EventHandler(this.top_OnValueChanged);
			// 
			// groupBox3
			// 
			this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox3.Controls.Add(this.getsidetexturesfromselection);
			this.groupBox3.Controls.Add(this.cbOverrideBottomTexture);
			this.groupBox3.Controls.Add(this.top);
			this.groupBox3.Controls.Add(this.cbOverrideMiddleTexture);
			this.groupBox3.Controls.Add(this.middle);
			this.groupBox3.Controls.Add(this.bottom);
			this.groupBox3.Controls.Add(this.cbOverrideTopTexture);
			this.groupBox3.Location = new System.Drawing.Point(3, 175);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(243, 166);
			this.groupBox3.TabIndex = 29;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Sidedef texture overrides:";
			// 
			// groupBox4
			// 
			this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox4.Controls.Add(this.fillall);
			this.groupBox4.Controls.Add(this.filllower);
			this.groupBox4.Controls.Add(this.fillmiddle);
			this.groupBox4.Controls.Add(this.fillupper);
			this.groupBox4.Controls.Add(this.fillfloor);
			this.groupBox4.Controls.Add(this.fillceiling);
			this.groupBox4.Location = new System.Drawing.Point(3, 491);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Size = new System.Drawing.Size(243, 48);
			this.groupBox4.TabIndex = 30;
			this.groupBox4.TabStop = false;
			this.groupBox4.Text = "Fill Selection with Textures:";
			// 
			// fillall
			// 
			this.fillall.Location = new System.Drawing.Point(6, 19);
			this.fillall.Name = "fillall";
			this.fillall.Size = new System.Drawing.Size(32, 23);
			this.fillall.TabIndex = 5;
			this.fillall.Text = "All";
			this.toolTip1.SetToolTip(this.fillall, "Replace all textures with currently enabled overrides");
			this.fillall.UseVisualStyleBackColor = true;
			this.fillall.Click += new System.EventHandler(this.fillall_Click);
			// 
			// filllower
			// 
			this.filllower.Location = new System.Drawing.Point(171, 19);
			this.filllower.Name = "filllower";
			this.filllower.Size = new System.Drawing.Size(26, 23);
			this.filllower.TabIndex = 4;
			this.filllower.Text = "L";
			this.toolTip1.SetToolTip(this.filllower, "Fill all lower textures within selection \r\nwith current lower override texture");
			this.filllower.UseVisualStyleBackColor = true;
			this.filllower.Click += new System.EventHandler(this.filllower_Click);
			// 
			// fillmiddle
			// 
			this.fillmiddle.Location = new System.Drawing.Point(139, 19);
			this.fillmiddle.Name = "fillmiddle";
			this.fillmiddle.Size = new System.Drawing.Size(26, 23);
			this.fillmiddle.TabIndex = 3;
			this.fillmiddle.Text = "M";
			this.toolTip1.SetToolTip(this.fillmiddle, "Fill all middle textures within selection \r\nwith current middle override texture");
			this.fillmiddle.UseVisualStyleBackColor = true;
			this.fillmiddle.Click += new System.EventHandler(this.fillmiddle_Click);
			// 
			// fillupper
			// 
			this.fillupper.Location = new System.Drawing.Point(107, 19);
			this.fillupper.Name = "fillupper";
			this.fillupper.Size = new System.Drawing.Size(26, 23);
			this.fillupper.TabIndex = 2;
			this.fillupper.Text = "U";
			this.toolTip1.SetToolTip(this.fillupper, "Fill all upper textures within selection \r\nwith current upper override texture");
			this.fillupper.UseVisualStyleBackColor = true;
			this.fillupper.Click += new System.EventHandler(this.fillupper_Click);
			// 
			// fillfloor
			// 
			this.fillfloor.Location = new System.Drawing.Point(76, 19);
			this.fillfloor.Name = "fillfloor";
			this.fillfloor.Size = new System.Drawing.Size(26, 23);
			this.fillfloor.TabIndex = 1;
			this.fillfloor.Text = "F";
			this.toolTip1.SetToolTip(this.fillfloor, "Fill all floor textures within selection \r\nwith current floor override texture");
			this.fillfloor.UseVisualStyleBackColor = true;
			this.fillfloor.Click += new System.EventHandler(this.fillfloor_Click);
			// 
			// fillceiling
			// 
			this.fillceiling.Location = new System.Drawing.Point(44, 19);
			this.fillceiling.Name = "fillceiling";
			this.fillceiling.Size = new System.Drawing.Size(26, 23);
			this.fillceiling.TabIndex = 0;
			this.fillceiling.Text = "C";
			this.toolTip1.SetToolTip(this.fillceiling, "Fill all ceiling textures within selection \r\nwith current ceiling override textur" +
					"e");
			this.fillceiling.UseVisualStyleBackColor = true;
			this.fillceiling.Click += new System.EventHandler(this.fillceiling_Click);
			// 
			// clearlower
			// 
			this.clearlower.Location = new System.Drawing.Point(171, 19);
			this.clearlower.Name = "clearlower";
			this.clearlower.Size = new System.Drawing.Size(26, 23);
			this.clearlower.TabIndex = 4;
			this.clearlower.Text = "L";
			this.toolTip1.SetToolTip(this.clearlower, "Remove all lower textures within selection ");
			this.clearlower.UseVisualStyleBackColor = true;
			this.clearlower.Click += new System.EventHandler(this.clearlower_Click);
			// 
			// clearmiddle
			// 
			this.clearmiddle.Location = new System.Drawing.Point(139, 19);
			this.clearmiddle.Name = "clearmiddle";
			this.clearmiddle.Size = new System.Drawing.Size(26, 23);
			this.clearmiddle.TabIndex = 3;
			this.clearmiddle.Text = "M";
			this.toolTip1.SetToolTip(this.clearmiddle, "Remove all middle textures within selection");
			this.clearmiddle.UseVisualStyleBackColor = true;
			this.clearmiddle.Click += new System.EventHandler(this.clearmiddle_Click);
			// 
			// clearupper
			// 
			this.clearupper.Location = new System.Drawing.Point(107, 19);
			this.clearupper.Name = "clearupper";
			this.clearupper.Size = new System.Drawing.Size(26, 23);
			this.clearupper.TabIndex = 2;
			this.clearupper.Text = "U";
			this.toolTip1.SetToolTip(this.clearupper, "Remove all upper textures within selection");
			this.clearupper.UseVisualStyleBackColor = true;
			this.clearupper.Click += new System.EventHandler(this.clearupper_Click);
			// 
			// clearfloor
			// 
			this.clearfloor.Location = new System.Drawing.Point(76, 19);
			this.clearfloor.Name = "clearfloor";
			this.clearfloor.Size = new System.Drawing.Size(26, 23);
			this.clearfloor.TabIndex = 1;
			this.clearfloor.Text = "F";
			this.toolTip1.SetToolTip(this.clearfloor, "Remove all floor textures within selection");
			this.clearfloor.UseVisualStyleBackColor = true;
			this.clearfloor.Click += new System.EventHandler(this.clearfloor_Click);
			// 
			// clearceiling
			// 
			this.clearceiling.Location = new System.Drawing.Point(44, 19);
			this.clearceiling.Name = "clearceiling";
			this.clearceiling.Size = new System.Drawing.Size(26, 23);
			this.clearceiling.TabIndex = 0;
			this.clearceiling.Text = "C";
			this.toolTip1.SetToolTip(this.clearceiling, "Remove all ceiling textures within selection ");
			this.clearceiling.UseVisualStyleBackColor = true;
			this.clearceiling.Click += new System.EventHandler(this.clearceiling_Click);
			// 
			// clearall
			// 
			this.clearall.Location = new System.Drawing.Point(6, 19);
			this.clearall.Name = "clearall";
			this.clearall.Size = new System.Drawing.Size(32, 23);
			this.clearall.TabIndex = 6;
			this.clearall.Text = "All";
			this.toolTip1.SetToolTip(this.clearall, "Remove all textures from selected map elements");
			this.clearall.UseVisualStyleBackColor = true;
			this.clearall.Click += new System.EventHandler(this.clearall_Click);
			// 
			// groupBox5
			// 
			this.groupBox5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox5.Controls.Add(this.clearall);
			this.groupBox5.Controls.Add(this.clearlower);
			this.groupBox5.Controls.Add(this.clearmiddle);
			this.groupBox5.Controls.Add(this.clearupper);
			this.groupBox5.Controls.Add(this.clearfloor);
			this.groupBox5.Controls.Add(this.clearceiling);
			this.groupBox5.Location = new System.Drawing.Point(3, 545);
			this.groupBox5.Name = "groupBox5";
			this.groupBox5.Size = new System.Drawing.Size(243, 48);
			this.groupBox5.TabIndex = 31;
			this.groupBox5.TabStop = false;
			this.groupBox5.Text = "Remove Textures form Selection:";
			// 
			// getsidetexturesfromselection
			// 
			this.getsidetexturesfromselection.Location = new System.Drawing.Point(6, 137);
			this.getsidetexturesfromselection.Name = "getsidetexturesfromselection";
			this.getsidetexturesfromselection.Size = new System.Drawing.Size(230, 23);
			this.getsidetexturesfromselection.TabIndex = 31;
			this.getsidetexturesfromselection.Text = "Get From Selection";
			this.getsidetexturesfromselection.UseVisualStyleBackColor = true;
			this.getsidetexturesfromselection.Click += new System.EventHandler(this.getsidetexturesfromselection_Click);
			// 
			// getheightandbrightnessfromselection
			// 
			this.getheightandbrightnessfromselection.Location = new System.Drawing.Point(6, 108);
			this.getheightandbrightnessfromselection.Name = "getheightandbrightnessfromselection";
			this.getheightandbrightnessfromselection.Size = new System.Drawing.Size(230, 23);
			this.getheightandbrightnessfromselection.TabIndex = 32;
			this.getheightandbrightnessfromselection.Text = "Get From Selection";
			this.getheightandbrightnessfromselection.UseVisualStyleBackColor = true;
			this.getheightandbrightnessfromselection.Click += new System.EventHandler(this.getheightandbrightnessfromselection_Click);
			// 
			// getsectortexturesfromselection
			// 
			this.getsectortexturesfromselection.Location = new System.Drawing.Point(6, 137);
			this.getsectortexturesfromselection.Name = "getsectortexturesfromselection";
			this.getsectortexturesfromselection.Size = new System.Drawing.Size(230, 23);
			this.getsectortexturesfromselection.TabIndex = 32;
			this.getsectortexturesfromselection.Text = "Get From Selection";
			this.getsectortexturesfromselection.UseVisualStyleBackColor = true;
			this.getsectortexturesfromselection.Click += new System.EventHandler(this.getsectortexturesfromselection_Click);
			// 
			// SectorDrawingOptionsPanel
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.Controls.Add(this.groupBox5);
			this.Controls.Add(this.groupBox4);
			this.Controls.Add(this.groupBox3);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Name = "SectorDrawingOptionsPanel";
			this.Size = new System.Drawing.Size(249, 600);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			this.groupBox4.ResumeLayout(false);
			this.groupBox5.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.CheckBox cbOverrideCeilingTexture;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.CheckBox cbOverrideFloorTexture;
		private System.Windows.Forms.CheckBox cbBrightness;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox brightness;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox floorHeight;
		private System.Windows.Forms.CheckBox cbCeilHeight;
		private System.Windows.Forms.CheckBox cbFloorHeight;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox ceilHeight;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label14;
		private System.Windows.Forms.CheckBox cbOverrideBottomTexture;
		private System.Windows.Forms.CheckBox cbOverrideMiddleTexture;
		private CodeImp.DoomBuilder.Controls.TextureSelectorControl bottom;
		private System.Windows.Forms.CheckBox cbOverrideTopTexture;
		private CodeImp.DoomBuilder.Controls.TextureSelectorControl middle;
		private CodeImp.DoomBuilder.Controls.TextureSelectorControl top;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.Button filllower;
		private System.Windows.Forms.Button fillmiddle;
		private System.Windows.Forms.Button fillupper;
		private System.Windows.Forms.Button fillfloor;
		private System.Windows.Forms.Button fillceiling;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.GroupBox groupBox5;
		private System.Windows.Forms.Button clearlower;
		private System.Windows.Forms.Button clearmiddle;
		private System.Windows.Forms.Button clearupper;
		private System.Windows.Forms.Button clearfloor;
		private System.Windows.Forms.Button clearceiling;
		private CodeImp.DoomBuilder.Controls.FlatSelectorControl floor;
		private CodeImp.DoomBuilder.Controls.FlatSelectorControl ceiling;
		private System.Windows.Forms.Button fillall;
		private System.Windows.Forms.Button clearall;
		private System.Windows.Forms.Button getheightandbrightnessfromselection;
		private System.Windows.Forms.Button getsidetexturesfromselection;
		private System.Windows.Forms.Button getsectortexturesfromselection;
	}
}
