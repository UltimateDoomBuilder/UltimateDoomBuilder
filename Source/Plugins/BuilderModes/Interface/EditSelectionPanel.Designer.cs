namespace CodeImp.DoomBuilder.BuilderModes
{
	partial class EditSelectionPanel
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
				mode = null; //mxd
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditSelectionPanel));
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.preciseposition = new System.Windows.Forms.CheckBox();
			this.orgposy = new System.Windows.Forms.Button();
			this.orgposx = new System.Windows.Forms.Button();
			this.label16 = new System.Windows.Forms.Label();
			this.label16.AutoSize = true;
			this.label19 = new System.Windows.Forms.Label();
			this.label19.AutoSize = true;
			this.label6 = new System.Windows.Forms.Label();
			this.label6.AutoSize = true;
			this.label5 = new System.Windows.Forms.Label();
			this.label5.AutoSize = true;
			this.label2 = new System.Windows.Forms.Label();
			this.label2.AutoSize = true;
			this.label1 = new System.Windows.Forms.Label();
			this.label1.AutoSize = true;
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.orgsizey = new System.Windows.Forms.Button();
			this.orgsizex = new System.Windows.Forms.Button();
			this.label12 = new System.Windows.Forms.Label();
			this.label12.AutoSize = true;
			this.label9 = new System.Windows.Forms.Label();
			this.label9.AutoSize = true;
			this.label8 = new System.Windows.Forms.Label();
			this.label8.AutoSize = true;
			this.label7 = new System.Windows.Forms.Label();
			this.label7.AutoSize = true;
			this.label4 = new System.Windows.Forms.Label();
			this.label4.AutoSize = true;
			this.label3 = new System.Windows.Forms.Label();
			this.label3.AutoSize = true;
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.heightmode = new System.Windows.Forms.ComboBox();
			this.label10 = new System.Windows.Forms.Label();
			this.label10.AutoSize = true;
			this.label14 = new System.Windows.Forms.Label();
			this.label14.AutoSize = true;
			this.flipv = new System.Windows.Forms.Button();
			this.fliph = new System.Windows.Forms.Button();
			this.label13 = new System.Windows.Forms.Label();
			this.label13.AutoSize = true;
			this.label11 = new System.Windows.Forms.Label();
			this.label11.AutoSize = true;
			this.tooltip = new System.Windows.Forms.ToolTip(this.components);
			this.pinfloortextures = new System.Windows.Forms.CheckBox();
			this.gbTextures = new System.Windows.Forms.GroupBox();
			this.pinceilingtextures = new System.Windows.Forms.CheckBox();
			this.absrot = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.relsizey = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.relsizex = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.abssizey = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.abssizex = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.relposy = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.relposx = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.absposy = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.absposx = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.gbTextures.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.preciseposition);
			this.groupBox1.Controls.Add(this.orgposy);
			this.groupBox1.Controls.Add(this.orgposx);
			this.groupBox1.Controls.Add(this.label16);
			this.groupBox1.Controls.Add(this.label19);
			this.groupBox1.Controls.Add(this.label6);
			this.groupBox1.Controls.Add(this.label5);
			this.groupBox1.Controls.Add(this.relposy);
			this.groupBox1.Controls.Add(this.relposx);
			this.groupBox1.Controls.Add(this.absposy);
			this.groupBox1.Controls.Add(this.absposx);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Location = new System.Drawing.Point(3, 3);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(243, 140);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Position:";
			// 
			// preciseposition
			// 
			this.preciseposition.AutoSize = true;
			this.preciseposition.Location = new System.Drawing.Point(58, 115);
			this.preciseposition.Name = "preciseposition";
			this.preciseposition.Size = new System.Drawing.Size(146, 17);
			this.preciseposition.TabIndex = 6;
			this.preciseposition.Text = "High precision positioning";
			this.tooltip.SetToolTip(this.preciseposition, "When checked, thing and vertex positions will be set using floating point precisi" +
        "on.\r\nOtherwise, they will be rounded to the nearest integer.");
			this.preciseposition.UseVisualStyleBackColor = true;
			this.preciseposition.CheckedChanged += new System.EventHandler(this.preciseposition_CheckedChanged);
			// 
			// orgposy
			// 
			this.orgposy.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.orgposy.Location = new System.Drawing.Point(136, 23);
			this.orgposy.Name = "orgposy";
			this.orgposy.Size = new System.Drawing.Size(72, 24);
			this.orgposy.TabIndex = 1;
			this.orgposy.Text = "-2000";
			this.orgposy.UseVisualStyleBackColor = true;
			this.orgposy.Click += new System.EventHandler(this.orgposy_Click);
			// 
			// orgposx
			// 
			this.orgposx.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.orgposx.Location = new System.Drawing.Point(58, 23);
			this.orgposx.Name = "orgposx";
			this.orgposx.Size = new System.Drawing.Size(72, 24);
			this.orgposx.TabIndex = 0;
			this.orgposx.Text = "-2000";
			this.orgposx.UseVisualStyleBackColor = true;
			this.orgposx.Click += new System.EventHandler(this.orgposx_Click);
			// 
			// label16
			// 
			this.label16.AutoSize = true;
			this.label16.Location = new System.Drawing.Point(214, 28);
			this.label16.Name = "label16";
			this.label16.Size = new System.Drawing.Size(21, 13);
			this.label16.TabIndex = 26;
			this.label16.Text = "mu";
			// 
			// label19
			// 
			this.label19.AutoSize = true;
			this.label19.Location = new System.Drawing.Point(9, 28);
			this.label19.Name = "label19";
			this.label19.Size = new System.Drawing.Size(45, 13);
			this.label19.TabIndex = 23;
			this.label19.Text = "Original:";
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(214, 88);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(21, 13);
			this.label6.TabIndex = 13;
			this.label6.Text = "mp";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(214, 58);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(21, 13);
			this.label5.TabIndex = 12;
			this.label5.Text = "mp";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(7, 88);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(49, 13);
			this.label2.TabIndex = 1;
			this.label2.Text = "Relative:";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(6, 58);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(51, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Absolute:";
			// 
			// groupBox2
			// 
			this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox2.Controls.Add(this.orgsizey);
			this.groupBox2.Controls.Add(this.orgsizex);
			this.groupBox2.Controls.Add(this.label12);
			this.groupBox2.Controls.Add(this.label9);
			this.groupBox2.Controls.Add(this.label8);
			this.groupBox2.Controls.Add(this.label7);
			this.groupBox2.Controls.Add(this.relsizey);
			this.groupBox2.Controls.Add(this.relsizex);
			this.groupBox2.Controls.Add(this.label4);
			this.groupBox2.Controls.Add(this.abssizey);
			this.groupBox2.Controls.Add(this.abssizex);
			this.groupBox2.Controls.Add(this.label3);
			this.groupBox2.Location = new System.Drawing.Point(3, 149);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(243, 117);
			this.groupBox2.TabIndex = 1;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Size:";
			// 
			// orgsizey
			// 
			this.orgsizey.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.orgsizey.Location = new System.Drawing.Point(136, 23);
			this.orgsizey.Name = "orgsizey";
			this.orgsizey.Size = new System.Drawing.Size(72, 24);
			this.orgsizey.TabIndex = 1;
			this.orgsizey.Text = "-2000";
			this.orgsizey.UseVisualStyleBackColor = true;
			this.orgsizey.Click += new System.EventHandler(this.orgsizey_Click);
			// 
			// orgsizex
			// 
			this.orgsizex.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.orgsizex.Location = new System.Drawing.Point(58, 23);
			this.orgsizex.Name = "orgsizex";
			this.orgsizex.Size = new System.Drawing.Size(72, 24);
			this.orgsizex.TabIndex = 0;
			this.orgsizex.Text = "-2000";
			this.orgsizex.UseVisualStyleBackColor = true;
			this.orgsizex.Click += new System.EventHandler(this.orgsizex_Click);
			// 
			// label12
			// 
			this.label12.AutoSize = true;
			this.label12.Location = new System.Drawing.Point(214, 28);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(21, 13);
			this.label12.TabIndex = 21;
			this.label12.Text = "mp";
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point(9, 28);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(45, 13);
			this.label9.TabIndex = 18;
			this.label9.Text = "Original:";
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(214, 88);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(15, 13);
			this.label8.TabIndex = 17;
			this.label8.Text = "%";
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(214, 58);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(21, 13);
			this.label7.TabIndex = 16;
			this.label7.Text = "mp";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(15, 88);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(37, 13);
			this.label4.TabIndex = 13;
			this.label4.Text = "Scale:";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(2, 58);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(51, 13);
			this.label3.TabIndex = 10;
			this.label3.Text = "Absolute:";
			// 
			// groupBox3
			// 
			this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox3.Controls.Add(this.heightmode);
			this.groupBox3.Controls.Add(this.label10);
			this.groupBox3.Controls.Add(this.label14);
			this.groupBox3.Controls.Add(this.flipv);
			this.groupBox3.Controls.Add(this.fliph);
			this.groupBox3.Controls.Add(this.label13);
			this.groupBox3.Controls.Add(this.label11);
			this.groupBox3.Controls.Add(this.absrot);
			this.groupBox3.Location = new System.Drawing.Point(3, 272);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(243, 122);
			this.groupBox3.TabIndex = 2;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Transform:";
			// 
			// heightmode
			// 
			this.heightmode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.heightmode.FormattingEnabled = true;
			this.heightmode.Items.AddRange(new object[] {
            "Don\'t adjust height",
            "Adjust floor height",
            "Adjust ceiling height",
            "Adjust floor and ceiling heights"});
			this.heightmode.Location = new System.Drawing.Point(58, 53);
			this.heightmode.Name = "heightmode";
			this.heightmode.Size = new System.Drawing.Size(171, 21);
			this.heightmode.TabIndex = 29;
			this.heightmode.SelectedIndexChanged += new System.EventHandler(this.heightmode_SelectedIndexChanged);
			// 
			// label10
			// 
			this.label10.AutoSize = true;
			this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.label10.ForeColor = System.Drawing.SystemColors.HotTrack;
			this.label10.Location = new System.Drawing.Point(14, 56);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(41, 13);
			this.label10.TabIndex = 28;
			this.label10.Text = "Height:";
			this.tooltip.SetToolTip(this.label10, resources.GetString("label10.ToolTip"));
			// 
			// label14
			// 
			this.label14.AutoSize = true;
			this.label14.Location = new System.Drawing.Point(9, 88);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(46, 13);
			this.label14.TabIndex = 27;
			this.label14.Text = "Flipping:";
			// 
			// flipv
			// 
			this.flipv.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.flipv.Image = global::CodeImp.DoomBuilder.BuilderModes.Properties.Resources.FlipSelectionV;
			this.flipv.Location = new System.Drawing.Point(94, 80);
			this.flipv.Name = "flipv";
			this.flipv.Size = new System.Drawing.Size(30, 30);
			this.flipv.TabIndex = 2;
			this.tooltip.SetToolTip(this.flipv, "Flip Selection Vertically");
			this.flipv.UseVisualStyleBackColor = true;
			this.flipv.Click += new System.EventHandler(this.flipv_Click);
			// 
			// fliph
			// 
			this.fliph.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.fliph.Image = global::CodeImp.DoomBuilder.BuilderModes.Properties.Resources.FlipSelectionH;
			this.fliph.Location = new System.Drawing.Point(58, 80);
			this.fliph.Name = "fliph";
			this.fliph.Size = new System.Drawing.Size(30, 30);
			this.fliph.TabIndex = 1;
			this.tooltip.SetToolTip(this.fliph, "Flip Selection Horizontally");
			this.fliph.UseVisualStyleBackColor = true;
			this.fliph.Click += new System.EventHandler(this.fliph_Click);
			// 
			// label13
			// 
			this.label13.AutoSize = true;
			this.label13.Location = new System.Drawing.Point(6, 28);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(50, 13);
			this.label13.TabIndex = 23;
			this.label13.Text = "Rotation:";
			// 
			// label11
			// 
			this.label11.AutoSize = true;
			this.label11.Location = new System.Drawing.Point(146, 28);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(28, 13);
			this.label11.TabIndex = 22;
			this.label11.Text = "deg.";
			// 
			// pinfloortextures
			// 
			this.pinfloortextures.AutoSize = true;
			this.pinfloortextures.Location = new System.Drawing.Point(9, 42);
			this.pinfloortextures.Name = "pinfloortextures";
			this.pinfloortextures.Size = new System.Drawing.Size(104, 17);
			this.pinfloortextures.TabIndex = 39;
			this.pinfloortextures.Text = "Pin floor textures";
			this.pinfloortextures.UseVisualStyleBackColor = true;
			this.pinfloortextures.CheckedChanged += new System.EventHandler(this.cbPinFloorTextures_CheckedChanged);
			// 
			// gbTextures
			// 
			this.gbTextures.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gbTextures.Controls.Add(this.pinceilingtextures);
			this.gbTextures.Controls.Add(this.pinfloortextures);
			this.gbTextures.Location = new System.Drawing.Point(3, 400);
			this.gbTextures.Name = "gbTextures";
			this.gbTextures.Size = new System.Drawing.Size(243, 68);
			this.gbTextures.TabIndex = 40;
			this.gbTextures.TabStop = false;
			this.gbTextures.Text = "Textures";
			// 
			// pinceilingtextures
			// 
			this.pinceilingtextures.AutoSize = true;
			this.pinceilingtextures.Location = new System.Drawing.Point(9, 19);
			this.pinceilingtextures.Name = "pinceilingtextures";
			this.pinceilingtextures.Size = new System.Drawing.Size(114, 17);
			this.pinceilingtextures.TabIndex = 40;
			this.pinceilingtextures.Text = "Pin ceiling textures";
			this.pinceilingtextures.UseVisualStyleBackColor = true;
			this.pinceilingtextures.CheckedChanged += new System.EventHandler(this.pinceilingtextures_CheckedChanged);
			// 
			// absrot
			// 
			this.absrot.AllowDecimal = true;
			this.absrot.AllowExpressions = false;
			this.absrot.AllowNegative = true;
			this.absrot.AllowRelative = true;
			this.absrot.ButtonStep = 1;
			this.absrot.ButtonStepBig = 10F;
			this.absrot.ButtonStepFloat = 1F;
			this.absrot.ButtonStepSmall = 0.1F;
			this.absrot.ButtonStepsUseModifierKeys = false;
			this.absrot.ButtonStepsWrapAround = false;
			this.absrot.Location = new System.Drawing.Point(58, 23);
			this.absrot.Name = "absrot";
			this.absrot.Size = new System.Drawing.Size(82, 24);
			this.absrot.StepValues = null;
			this.absrot.TabIndex = 0;
			this.absrot.WhenTextChanged += new System.EventHandler(this.WhenTextChanged);
			this.absrot.WhenButtonsClicked += new System.EventHandler(this.absrot_Validated);
			this.absrot.WhenEnterPressed += new System.EventHandler(this.absrot_Validated);
			this.absrot.Validated += new System.EventHandler(this.absrot_Validated);
			// 
			// relsizey
			// 
			this.relsizey.AllowDecimal = true;
			this.relsizey.AllowExpressions = false;
			this.relsizey.AllowNegative = true;
			this.relsizey.AllowRelative = true;
			this.relsizey.ButtonStep = 1;
			this.relsizey.ButtonStepBig = 10F;
			this.relsizey.ButtonStepFloat = 1F;
			this.relsizey.ButtonStepSmall = 0.1F;
			this.relsizey.ButtonStepsUseModifierKeys = false;
			this.relsizey.ButtonStepsWrapAround = false;
			this.relsizey.Location = new System.Drawing.Point(136, 83);
			this.relsizey.Name = "relsizey";
			this.relsizey.Size = new System.Drawing.Size(72, 24);
			this.relsizey.StepValues = null;
			this.relsizey.TabIndex = 5;
			this.relsizey.WhenTextChanged += new System.EventHandler(this.WhenTextChanged);
			this.relsizey.WhenButtonsClicked += new System.EventHandler(this.relsizey_Validated);
			this.relsizey.WhenEnterPressed += new System.EventHandler(this.relsizey_Validated);
			this.relsizey.Validated += new System.EventHandler(this.relsizey_Validated);
			// 
			// relsizex
			// 
			this.relsizex.AllowDecimal = true;
			this.relsizex.AllowExpressions = false;
			this.relsizex.AllowNegative = true;
			this.relsizex.AllowRelative = true;
			this.relsizex.ButtonStep = 1;
			this.relsizex.ButtonStepBig = 10F;
			this.relsizex.ButtonStepFloat = 1F;
			this.relsizex.ButtonStepSmall = 0.1F;
			this.relsizex.ButtonStepsUseModifierKeys = false;
			this.relsizex.ButtonStepsWrapAround = false;
			this.relsizex.Location = new System.Drawing.Point(58, 83);
			this.relsizex.Name = "relsizex";
			this.relsizex.Size = new System.Drawing.Size(72, 24);
			this.relsizex.StepValues = null;
			this.relsizex.TabIndex = 4;
			this.relsizex.WhenTextChanged += new System.EventHandler(this.WhenTextChanged);
			this.relsizex.WhenButtonsClicked += new System.EventHandler(this.relsizex_Validated);
			this.relsizex.WhenEnterPressed += new System.EventHandler(this.relsizex_Validated);
			this.relsizex.Validated += new System.EventHandler(this.relsizex_Validated);
			// 
			// abssizey
			// 
			this.abssizey.AllowDecimal = true;
			this.abssizey.AllowExpressions = false;
			this.abssizey.AllowNegative = true;
			this.abssizey.AllowRelative = true;
			this.abssizey.ButtonStep = 1;
			this.abssizey.ButtonStepBig = 10F;
			this.abssizey.ButtonStepFloat = 1F;
			this.abssizey.ButtonStepSmall = 0.1F;
			this.abssizey.ButtonStepsUseModifierKeys = false;
			this.abssizey.ButtonStepsWrapAround = false;
			this.abssizey.Location = new System.Drawing.Point(136, 53);
			this.abssizey.Name = "abssizey";
			this.abssizey.Size = new System.Drawing.Size(72, 24);
			this.abssizey.StepValues = null;
			this.abssizey.TabIndex = 3;
			this.abssizey.WhenTextChanged += new System.EventHandler(this.WhenTextChanged);
			this.abssizey.WhenButtonsClicked += new System.EventHandler(this.abssizey_Validated);
			this.abssizey.WhenEnterPressed += new System.EventHandler(this.abssizey_Validated);
			this.abssizey.Validated += new System.EventHandler(this.abssizey_Validated);
			// 
			// abssizex
			// 
			this.abssizex.AllowDecimal = true;
			this.abssizex.AllowExpressions = false;
			this.abssizex.AllowNegative = true;
			this.abssizex.AllowRelative = true;
			this.abssizex.ButtonStep = 1;
			this.abssizex.ButtonStepBig = 10F;
			this.abssizex.ButtonStepFloat = 1F;
			this.abssizex.ButtonStepSmall = 0.1F;
			this.abssizex.ButtonStepsUseModifierKeys = false;
			this.abssizex.ButtonStepsWrapAround = false;
			this.abssizex.Location = new System.Drawing.Point(58, 53);
			this.abssizex.Name = "abssizex";
			this.abssizex.Size = new System.Drawing.Size(72, 24);
			this.abssizex.StepValues = null;
			this.abssizex.TabIndex = 2;
			this.abssizex.WhenTextChanged += new System.EventHandler(this.WhenTextChanged);
			this.abssizex.WhenButtonsClicked += new System.EventHandler(this.abssizex_Validated);
			this.abssizex.WhenEnterPressed += new System.EventHandler(this.abssizex_Validated);
			this.abssizex.Validated += new System.EventHandler(this.abssizex_Validated);
			// 
			// relposy
			// 
			this.relposy.AllowDecimal = true;
			this.relposy.AllowExpressions = false;
			this.relposy.AllowNegative = true;
			this.relposy.AllowRelative = true;
			this.relposy.ButtonStep = 1;
			this.relposy.ButtonStepBig = 10F;
			this.relposy.ButtonStepFloat = 1F;
			this.relposy.ButtonStepSmall = 0.1F;
			this.relposy.ButtonStepsUseModifierKeys = false;
			this.relposy.ButtonStepsWrapAround = false;
			this.relposy.Location = new System.Drawing.Point(136, 83);
			this.relposy.Name = "relposy";
			this.relposy.Size = new System.Drawing.Size(72, 24);
			this.relposy.StepValues = null;
			this.relposy.TabIndex = 5;
			this.relposy.WhenTextChanged += new System.EventHandler(this.WhenTextChanged);
			this.relposy.WhenButtonsClicked += new System.EventHandler(this.relposy_Validated);
			this.relposy.WhenEnterPressed += new System.EventHandler(this.relposy_Validated);
			this.relposy.Validated += new System.EventHandler(this.relposy_Validated);
			// 
			// relposx
			// 
			this.relposx.AllowDecimal = true;
			this.relposx.AllowExpressions = false;
			this.relposx.AllowNegative = true;
			this.relposx.AllowRelative = true;
			this.relposx.ButtonStep = 1;
			this.relposx.ButtonStepBig = 10F;
			this.relposx.ButtonStepFloat = 1F;
			this.relposx.ButtonStepSmall = 0.1F;
			this.relposx.ButtonStepsUseModifierKeys = false;
			this.relposx.ButtonStepsWrapAround = false;
			this.relposx.Location = new System.Drawing.Point(58, 83);
			this.relposx.Name = "relposx";
			this.relposx.Size = new System.Drawing.Size(72, 24);
			this.relposx.StepValues = null;
			this.relposx.TabIndex = 4;
			this.relposx.WhenTextChanged += new System.EventHandler(this.WhenTextChanged);
			this.relposx.WhenButtonsClicked += new System.EventHandler(this.relposx_Validated);
			this.relposx.WhenEnterPressed += new System.EventHandler(this.relposx_Validated);
			this.relposx.Validated += new System.EventHandler(this.relposx_Validated);
			// 
			// absposy
			// 
			this.absposy.AllowDecimal = true;
			this.absposy.AllowExpressions = false;
			this.absposy.AllowNegative = true;
			this.absposy.AllowRelative = true;
			this.absposy.ButtonStep = 1;
			this.absposy.ButtonStepBig = 10F;
			this.absposy.ButtonStepFloat = 1F;
			this.absposy.ButtonStepSmall = 0.1F;
			this.absposy.ButtonStepsUseModifierKeys = false;
			this.absposy.ButtonStepsWrapAround = false;
			this.absposy.Location = new System.Drawing.Point(136, 53);
			this.absposy.Name = "absposy";
			this.absposy.Size = new System.Drawing.Size(72, 24);
			this.absposy.StepValues = null;
			this.absposy.TabIndex = 3;
			this.absposy.WhenTextChanged += new System.EventHandler(this.WhenTextChanged);
			this.absposy.WhenButtonsClicked += new System.EventHandler(this.absposy_Validated);
			this.absposy.WhenEnterPressed += new System.EventHandler(this.absposy_Validated);
			this.absposy.Validated += new System.EventHandler(this.absposy_Validated);
			// 
			// absposx
			// 
			this.absposx.AllowDecimal = true;
			this.absposx.AllowExpressions = false;
			this.absposx.AllowNegative = true;
			this.absposx.AllowRelative = true;
			this.absposx.ButtonStep = 1;
			this.absposx.ButtonStepBig = 10F;
			this.absposx.ButtonStepFloat = 1F;
			this.absposx.ButtonStepSmall = 0.1F;
			this.absposx.ButtonStepsUseModifierKeys = false;
			this.absposx.ButtonStepsWrapAround = false;
			this.absposx.Location = new System.Drawing.Point(58, 53);
			this.absposx.Name = "absposx";
			this.absposx.Size = new System.Drawing.Size(72, 24);
			this.absposx.StepValues = null;
			this.absposx.TabIndex = 2;
			this.absposx.WhenTextChanged += new System.EventHandler(this.WhenTextChanged);
			this.absposx.WhenButtonsClicked += new System.EventHandler(this.absposx_Validated);
			this.absposx.WhenEnterPressed += new System.EventHandler(this.absposx_Validated);
			this.absposx.Validated += new System.EventHandler(this.absposx_Validated);
			// 
			// EditSelectionPanel
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.Controls.Add(this.gbTextures);
			this.Controls.Add(this.groupBox3);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Name = "EditSelectionPanel";
			this.Size = new System.Drawing.Size(249, 652);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			this.gbTextures.ResumeLayout(false);
			this.gbTextures.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox relposy;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox relposx;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox absposy;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox absposx;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label7;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox relsizey;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox relsizex;
		private System.Windows.Forms.Label label4;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox abssizey;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox abssizex;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label label12;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox absrot;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Label label14;
		private System.Windows.Forms.Button flipv;
		private System.Windows.Forms.Button fliph;
		private System.Windows.Forms.Label label16;
		private System.Windows.Forms.Label label19;
		private System.Windows.Forms.Button orgposx;
		private System.Windows.Forms.Button orgposy;
		private System.Windows.Forms.Button orgsizey;
		private System.Windows.Forms.Button orgsizex;
		private System.Windows.Forms.CheckBox preciseposition;
		private System.Windows.Forms.ToolTip tooltip;
		private System.Windows.Forms.ComboBox heightmode;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.CheckBox pinfloortextures;
		private System.Windows.Forms.GroupBox gbTextures;
		private System.Windows.Forms.CheckBox pinceilingtextures;
	}
}
