namespace CodeImp.DoomBuilder.Windows
{
	partial class SectorEditFormUDMF
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
			System.Windows.Forms.GroupBox groupaction;
			System.Windows.Forms.GroupBox groupeffect;
			System.Windows.Forms.Label label4;
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SectorEditFormUDMF));
			System.Windows.Forms.Label label14;
			System.Windows.Forms.Label label9;
			System.Windows.Forms.Label label2;
			System.Windows.Forms.Label label8;
			System.Windows.Forms.GroupBox groupfloorceiling;
			System.Windows.Forms.Label label15;
			System.Windows.Forms.Label label6;
			System.Windows.Forms.Label label5;
			System.Windows.Forms.Label label17;
			System.Windows.Forms.Label label16;
			System.Windows.Forms.Label label18;
			System.Windows.Forms.Label label19;
			System.Windows.Forms.Label label13;
			this.tagsselector = new CodeImp.DoomBuilder.Controls.TagsSelector();
			this.fogdensity = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.soundsequence = new System.Windows.Forms.ComboBox();
			this.resetsoundsequence = new System.Windows.Forms.Button();
			this.brightness = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.gravity = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.browseeffect = new System.Windows.Forms.Button();
			this.effect = new CodeImp.DoomBuilder.Controls.ActionSelectorControl();
			this.heightoffset = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.ceilingheight = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.sectorheightlabel = new System.Windows.Forms.Label();
			this.sectorheight = new System.Windows.Forms.Label();
			this.floorheight = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.tabs = new System.Windows.Forms.TabControl();
			this.tabproperties = new System.Windows.Forms.TabPage();
			this.groupdamage = new System.Windows.Forms.GroupBox();
			this.leakiness = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.damageinterval = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.resetdamagetype = new System.Windows.Forms.Button();
			this.damageamount = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.damagetype = new System.Windows.Forms.ComboBox();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.flags = new CodeImp.DoomBuilder.Controls.CheckboxArrayControl();
			this.tabColors = new System.Windows.Forms.TabPage();
			this.groupBox11 = new System.Windows.Forms.GroupBox();
			this.floorGlowEnabled = new System.Windows.Forms.CheckBox();
			this.floorglowheightrequired = new System.Windows.Forms.PictureBox();
			this.resetfloorglowheight = new System.Windows.Forms.Button();
			this.floorglowheightlabel = new System.Windows.Forms.Label();
			this.floorglowheight = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.floorglowcolor = new CodeImp.DoomBuilder.Controls.ColorFieldsControl();
			this.groupBox10 = new System.Windows.Forms.GroupBox();
			this.ceilingGlowEnabled = new System.Windows.Forms.CheckBox();
			this.ceilingglowheightrequired = new System.Windows.Forms.PictureBox();
			this.resetceilingglowheight = new System.Windows.Forms.Button();
			this.ceilingglowheightlabel = new System.Windows.Forms.Label();
			this.ceilingglowheight = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.ceilingglowcolor = new CodeImp.DoomBuilder.Controls.ColorFieldsControl();
			this.groupBox9 = new System.Windows.Forms.GroupBox();
			this.thingsColor = new CodeImp.DoomBuilder.Controls.ColorFieldsControl();
			this.lowerWallColor = new CodeImp.DoomBuilder.Controls.ColorFieldsControl();
			this.upperWallColor = new CodeImp.DoomBuilder.Controls.ColorFieldsControl();
			this.floorColor = new CodeImp.DoomBuilder.Controls.ColorFieldsControl();
			this.ceilingColor = new CodeImp.DoomBuilder.Controls.ColorFieldsControl();
			this.groupBox8 = new System.Windows.Forms.GroupBox();
			this.desaturation = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.lightColor = new CodeImp.DoomBuilder.Controls.ColorFieldsControl();
			this.fadeColor = new CodeImp.DoomBuilder.Controls.ColorFieldsControl();
			this.tabSurfaces = new System.Windows.Forms.TabPage();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.reset_floor_reflect = new System.Windows.Forms.Button();
			this.label23 = new System.Windows.Forms.Label();
			this.floor_reflect = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.resetfloorterrain = new System.Windows.Forms.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.floorterrain = new System.Windows.Forms.ComboBox();
			this.resetfloorlight = new System.Windows.Forms.Button();
			this.labelFloorOffsets = new System.Windows.Forms.Label();
			this.labelFloorScale = new System.Windows.Forms.Label();
			this.cbUseFloorLineAngles = new System.Windows.Forms.CheckBox();
			this.floorAngleControl = new CodeImp.DoomBuilder.Controls.AngleControlEx();
			this.labelfloorrenderstyle = new System.Windows.Forms.Label();
			this.label11 = new System.Windows.Forms.Label();
			this.floorRotation = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.floorLightAbsolute = new System.Windows.Forms.CheckBox();
			this.label12 = new System.Windows.Forms.Label();
			this.floorBrightness = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.floorRenderStyle = new System.Windows.Forms.ComboBox();
			this.floorScale = new CodeImp.DoomBuilder.Controls.PairedFieldsControl();
			this.floorOffsets = new CodeImp.DoomBuilder.Controls.PairedFieldsControl();
			this.floortex = new CodeImp.DoomBuilder.Controls.FlatSelectorControl();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.reset_ceiling_reflect = new System.Windows.Forms.Button();
			this.label20 = new System.Windows.Forms.Label();
			this.ceiling_reflect = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.resetceilterrain = new System.Windows.Forms.Button();
			this.label7 = new System.Windows.Forms.Label();
			this.ceilterrain = new System.Windows.Forms.ComboBox();
			this.resetceillight = new System.Windows.Forms.Button();
			this.labelCeilOffsets = new System.Windows.Forms.Label();
			this.labelCeilScale = new System.Windows.Forms.Label();
			this.cbUseCeilLineAngles = new System.Windows.Forms.CheckBox();
			this.ceilAngleControl = new CodeImp.DoomBuilder.Controls.AngleControlEx();
			this.labelceilrenderstyle = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.ceilRotation = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.ceilLightAbsolute = new System.Windows.Forms.CheckBox();
			this.labelLightFront = new System.Windows.Forms.Label();
			this.ceilBrightness = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.ceilRenderStyle = new System.Windows.Forms.ComboBox();
			this.ceilScale = new CodeImp.DoomBuilder.Controls.PairedFieldsControl();
			this.ceilOffsets = new CodeImp.DoomBuilder.Controls.PairedFieldsControl();
			this.ceilingtex = new CodeImp.DoomBuilder.Controls.FlatSelectorControl();
			this.tabslopes = new System.Windows.Forms.TabPage();
			this.groupBox7 = new System.Windows.Forms.GroupBox();
			this.resetalphafloor = new System.Windows.Forms.Button();
			this.floorportalflags = new CodeImp.DoomBuilder.Controls.CheckboxArrayControl();
			this.label22 = new System.Windows.Forms.Label();
			this.alphafloor = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.floorportalrenderstylelabel = new System.Windows.Forms.Label();
			this.floorportalrenderstyle = new System.Windows.Forms.ComboBox();
			this.groupBox6 = new System.Windows.Forms.GroupBox();
			this.resetalphaceiling = new System.Windows.Forms.Button();
			this.ceilportalflags = new CodeImp.DoomBuilder.Controls.CheckboxArrayControl();
			this.label21 = new System.Windows.Forms.Label();
			this.alphaceiling = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.ceilportalrenderstylelabel = new System.Windows.Forms.Label();
			this.ceilportalrenderstyle = new System.Windows.Forms.ComboBox();
			this.gbFloorSlope = new System.Windows.Forms.GroupBox();
			this.floorslopecontrol = new CodeImp.DoomBuilder.Controls.SectorSlopeControl();
			this.gbCeilingSLope = new System.Windows.Forms.GroupBox();
			this.ceilingslopecontrol = new CodeImp.DoomBuilder.Controls.SectorSlopeControl();
			this.tabcomment = new System.Windows.Forms.TabPage();
			this.commenteditor = new CodeImp.DoomBuilder.Controls.CommentEditor();
			this.tabcustom = new System.Windows.Forms.TabPage();
			this.fieldslist = new CodeImp.DoomBuilder.Controls.FieldsEditorControl();
			this.cancel = new System.Windows.Forms.Button();
			this.apply = new System.Windows.Forms.Button();
			this.tooltip = new System.Windows.Forms.ToolTip(this.components);
			groupaction = new System.Windows.Forms.GroupBox();
			groupeffect = new System.Windows.Forms.GroupBox();
			label4 = new System.Windows.Forms.Label();
			label14 = new System.Windows.Forms.Label();
			label9 = new System.Windows.Forms.Label();
			label2 = new System.Windows.Forms.Label();
			label8 = new System.Windows.Forms.Label();
			groupfloorceiling = new System.Windows.Forms.GroupBox();
			label15 = new System.Windows.Forms.Label();
			label6 = new System.Windows.Forms.Label();
			label5 = new System.Windows.Forms.Label();
			label17 = new System.Windows.Forms.Label();
			label16 = new System.Windows.Forms.Label();
			label18 = new System.Windows.Forms.Label();
			label19 = new System.Windows.Forms.Label();
			label13 = new System.Windows.Forms.Label();
			groupaction.SuspendLayout();
			groupeffect.SuspendLayout();
			groupfloorceiling.SuspendLayout();
			this.tabs.SuspendLayout();
			this.tabproperties.SuspendLayout();
			this.groupdamage.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.tabColors.SuspendLayout();
			this.groupBox11.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.floorglowheightrequired)).BeginInit();
			this.groupBox10.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.ceilingglowheightrequired)).BeginInit();
			this.groupBox9.SuspendLayout();
			this.groupBox8.SuspendLayout();
			this.tabSurfaces.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.tabslopes.SuspendLayout();
			this.groupBox7.SuspendLayout();
			this.groupBox6.SuspendLayout();
			this.gbFloorSlope.SuspendLayout();
			this.gbCeilingSLope.SuspendLayout();
			this.tabcomment.SuspendLayout();
			this.tabcustom.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupaction
			// 
			groupaction.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			groupaction.Controls.Add(this.tagsselector);
			groupaction.Location = new System.Drawing.Point(7, 445);
			groupaction.Name = "groupaction";
			groupaction.Size = new System.Drawing.Size(557, 80);
			groupaction.TabIndex = 2;
			groupaction.TabStop = false;
			groupaction.Text = " Identification ";
			// 
			// tagsselector
			// 
			this.tagsselector.Location = new System.Drawing.Point(6, 15);
			this.tagsselector.Name = "tagsselector";
			this.tagsselector.Size = new System.Drawing.Size(545, 60);
			this.tagsselector.TabIndex = 0;
			// 
			// groupeffect
			// 
			groupeffect.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			groupeffect.Controls.Add(this.fogdensity);
			groupeffect.Controls.Add(label4);
			groupeffect.Controls.Add(this.soundsequence);
			groupeffect.Controls.Add(this.resetsoundsequence);
			groupeffect.Controls.Add(this.brightness);
			groupeffect.Controls.Add(label14);
			groupeffect.Controls.Add(label9);
			groupeffect.Controls.Add(this.gravity);
			groupeffect.Controls.Add(label2);
			groupeffect.Controls.Add(this.browseeffect);
			groupeffect.Controls.Add(this.effect);
			groupeffect.Controls.Add(label8);
			groupeffect.Location = new System.Drawing.Point(7, 330);
			groupeffect.Name = "groupeffect";
			groupeffect.Size = new System.Drawing.Size(557, 109);
			groupeffect.TabIndex = 1;
			groupeffect.TabStop = false;
			groupeffect.Text = " Effects ";
			// 
			// fogdensity
			// 
			this.fogdensity.AllowDecimal = false;
			this.fogdensity.AllowExpressions = false;
			this.fogdensity.AllowNegative = false;
			this.fogdensity.AllowRelative = false;
			this.fogdensity.ButtonStep = 8;
			this.fogdensity.ButtonStepBig = 16F;
			this.fogdensity.ButtonStepFloat = 1F;
			this.fogdensity.ButtonStepSmall = 1F;
			this.fogdensity.ButtonStepsUseModifierKeys = true;
			this.fogdensity.ButtonStepsWrapAround = false;
			this.fogdensity.Location = new System.Drawing.Point(350, 74);
			this.fogdensity.Name = "fogdensity";
			this.fogdensity.Size = new System.Drawing.Size(81, 24);
			this.fogdensity.StepValues = null;
			this.fogdensity.TabIndex = 10;
			this.fogdensity.Tag = "fogdensity";
			this.fogdensity.WhenTextChanged += new System.EventHandler(this.fogdensity_WhenTextChanged);
			// 
			// label4
			// 
			label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			label4.ForeColor = System.Drawing.SystemColors.HotTrack;
			label4.Location = new System.Drawing.Point(270, 79);
			label4.Name = "label4";
			label4.Size = new System.Drawing.Size(74, 14);
			label4.TabIndex = 9;
			label4.Tag = "fogdensity";
			label4.Text = "Fog density:";
			label4.TextAlign = System.Drawing.ContentAlignment.TopRight;
			this.tooltip.SetToolTip(label4, resources.GetString("label4.ToolTip"));
			// 
			// soundsequence
			// 
			this.soundsequence.FormattingEnabled = true;
			this.soundsequence.Location = new System.Drawing.Point(350, 48);
			this.soundsequence.Name = "soundsequence";
			this.soundsequence.Size = new System.Drawing.Size(167, 21);
			this.soundsequence.TabIndex = 12;
			this.soundsequence.Tag = "soundsequence";
			this.soundsequence.TextChanged += new System.EventHandler(this.soundsequence_TextChanged);
			this.soundsequence.MouseDown += new System.Windows.Forms.MouseEventHandler(this.soundsequence_MouseDown);
			// 
			// resetsoundsequence
			// 
			this.resetsoundsequence.Image = ((System.Drawing.Image)(resources.GetObject("resetsoundsequence.Image")));
			this.resetsoundsequence.Location = new System.Drawing.Point(523, 46);
			this.resetsoundsequence.Name = "resetsoundsequence";
			this.resetsoundsequence.Size = new System.Drawing.Size(28, 25);
			this.resetsoundsequence.TabIndex = 13;
			this.resetsoundsequence.Tag = "soundsequence";
			this.resetsoundsequence.Text = " ";
			this.tooltip.SetToolTip(this.resetsoundsequence, "Reset");
			this.resetsoundsequence.UseVisualStyleBackColor = true;
			this.resetsoundsequence.Click += new System.EventHandler(this.resetsoundsequence_Click);
			// 
			// brightness
			// 
			this.brightness.AllowDecimal = false;
			this.brightness.AllowExpressions = false;
			this.brightness.AllowNegative = false;
			this.brightness.AllowRelative = true;
			this.brightness.ButtonStep = 8;
			this.brightness.ButtonStepBig = 16F;
			this.brightness.ButtonStepFloat = 1F;
			this.brightness.ButtonStepSmall = 1F;
			this.brightness.ButtonStepsUseModifierKeys = true;
			this.brightness.ButtonStepsWrapAround = false;
			this.brightness.Location = new System.Drawing.Point(89, 46);
			this.brightness.Name = "brightness";
			this.brightness.Size = new System.Drawing.Size(81, 24);
			this.brightness.StepValues = null;
			this.brightness.TabIndex = 4;
			this.brightness.WhenTextChanged += new System.EventHandler(this.brightness_WhenTextChanged);
			// 
			// label14
			// 
			label14.AutoSize = true;
			label14.Location = new System.Drawing.Point(253, 52);
			label14.Name = "label14";
			label14.Size = new System.Drawing.Size(91, 13);
			label14.TabIndex = 11;
			label14.Tag = "soundsequence";
			label14.Text = "Sound sequence:";
			// 
			// label9
			// 
			label9.AutoSize = true;
			label9.Location = new System.Drawing.Point(21, 51);
			label9.Name = "label9";
			label9.Size = new System.Drawing.Size(59, 13);
			label9.TabIndex = 3;
			label9.Text = "Brightness:";
			// 
			// gravity
			// 
			this.gravity.AllowDecimal = true;
			this.gravity.AllowExpressions = false;
			this.gravity.AllowNegative = true;
			this.gravity.AllowRelative = true;
			this.gravity.ButtonStep = 1;
			this.gravity.ButtonStepBig = 1F;
			this.gravity.ButtonStepFloat = 0.1F;
			this.gravity.ButtonStepSmall = 0.01F;
			this.gravity.ButtonStepsUseModifierKeys = true;
			this.gravity.ButtonStepsWrapAround = false;
			this.gravity.Location = new System.Drawing.Point(89, 74);
			this.gravity.Name = "gravity";
			this.gravity.Size = new System.Drawing.Size(81, 24);
			this.gravity.StepValues = null;
			this.gravity.TabIndex = 6;
			this.gravity.Tag = "gravity";
			// 
			// label2
			// 
			label2.Location = new System.Drawing.Point(9, 79);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(74, 14);
			label2.TabIndex = 5;
			label2.Tag = "gravity";
			label2.Text = "Gravity:";
			label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// browseeffect
			// 
			this.browseeffect.Image = ((System.Drawing.Image)(resources.GetObject("browseeffect.Image")));
			this.browseeffect.Location = new System.Drawing.Point(523, 16);
			this.browseeffect.Name = "browseeffect";
			this.browseeffect.Size = new System.Drawing.Size(28, 25);
			this.browseeffect.TabIndex = 2;
			this.browseeffect.Text = " ";
			this.browseeffect.UseVisualStyleBackColor = true;
			this.browseeffect.Click += new System.EventHandler(this.browseeffect_Click);
			// 
			// effect
			// 
			this.effect.BackColor = System.Drawing.Color.Transparent;
			this.effect.Cursor = System.Windows.Forms.Cursors.Default;
			this.effect.Empty = false;
			this.effect.GeneralizedCategories = null;
			this.effect.GeneralizedOptions = null;
			this.effect.Location = new System.Drawing.Point(89, 18);
			this.effect.Name = "effect";
			this.effect.Size = new System.Drawing.Size(428, 21);
			this.effect.TabIndex = 1;
			this.effect.Value = 402;
			// 
			// label8
			// 
			label8.AutoSize = true;
			label8.Location = new System.Drawing.Point(35, 22);
			label8.Name = "label8";
			label8.Size = new System.Drawing.Size(45, 13);
			label8.TabIndex = 0;
			label8.Text = "Special:";
			// 
			// groupfloorceiling
			// 
			groupfloorceiling.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			groupfloorceiling.Controls.Add(label15);
			groupfloorceiling.Controls.Add(label6);
			groupfloorceiling.Controls.Add(label5);
			groupfloorceiling.Controls.Add(this.heightoffset);
			groupfloorceiling.Controls.Add(this.ceilingheight);
			groupfloorceiling.Controls.Add(this.sectorheightlabel);
			groupfloorceiling.Controls.Add(this.sectorheight);
			groupfloorceiling.Controls.Add(this.floorheight);
			groupfloorceiling.Location = new System.Drawing.Point(7, 186);
			groupfloorceiling.Name = "groupfloorceiling";
			groupfloorceiling.Size = new System.Drawing.Size(254, 138);
			groupfloorceiling.TabIndex = 0;
			groupfloorceiling.TabStop = false;
			groupfloorceiling.Text = " Heights ";
			// 
			// label15
			// 
			label15.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			label15.ForeColor = System.Drawing.SystemColors.HotTrack;
			label15.Location = new System.Drawing.Point(9, 83);
			label15.Name = "label15";
			label15.Size = new System.Drawing.Size(74, 14);
			label15.TabIndex = 4;
			label15.Text = "Height offset:";
			label15.TextAlign = System.Drawing.ContentAlignment.TopRight;
			this.tooltip.SetToolTip(label15, "Changes floor and ceiling height by given value.\r\nUse \"++\" to raise by sector hei" +
        "ght.\r\nUse \"--\" to lower by sector height.");
			// 
			// label6
			// 
			label6.Location = new System.Drawing.Point(9, 23);
			label6.Name = "label6";
			label6.Size = new System.Drawing.Size(74, 14);
			label6.TabIndex = 0;
			label6.Text = "Ceiling height:";
			label6.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label5
			// 
			label5.Location = new System.Drawing.Point(9, 53);
			label5.Name = "label5";
			label5.Size = new System.Drawing.Size(74, 14);
			label5.TabIndex = 2;
			label5.Text = "Floor height:";
			label5.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// heightoffset
			// 
			this.heightoffset.AllowDecimal = false;
			this.heightoffset.AllowExpressions = true;
			this.heightoffset.AllowNegative = true;
			this.heightoffset.AllowRelative = true;
			this.heightoffset.ButtonStep = 8;
			this.heightoffset.ButtonStepBig = 16F;
			this.heightoffset.ButtonStepFloat = 1F;
			this.heightoffset.ButtonStepSmall = 1F;
			this.heightoffset.ButtonStepsUseModifierKeys = true;
			this.heightoffset.ButtonStepsWrapAround = false;
			this.heightoffset.Location = new System.Drawing.Point(89, 78);
			this.heightoffset.Name = "heightoffset";
			this.heightoffset.Size = new System.Drawing.Size(81, 24);
			this.heightoffset.StepValues = null;
			this.heightoffset.TabIndex = 5;
			this.heightoffset.WhenTextChanged += new System.EventHandler(this.heightoffset_WhenTextChanged);
			// 
			// ceilingheight
			// 
			this.ceilingheight.AllowDecimal = false;
			this.ceilingheight.AllowExpressions = true;
			this.ceilingheight.AllowNegative = true;
			this.ceilingheight.AllowRelative = true;
			this.ceilingheight.ButtonStep = 8;
			this.ceilingheight.ButtonStepBig = 16F;
			this.ceilingheight.ButtonStepFloat = 1F;
			this.ceilingheight.ButtonStepSmall = 1F;
			this.ceilingheight.ButtonStepsUseModifierKeys = true;
			this.ceilingheight.ButtonStepsWrapAround = false;
			this.ceilingheight.Location = new System.Drawing.Point(89, 18);
			this.ceilingheight.Name = "ceilingheight";
			this.ceilingheight.Size = new System.Drawing.Size(81, 24);
			this.ceilingheight.StepValues = null;
			this.ceilingheight.TabIndex = 1;
			this.ceilingheight.WhenTextChanged += new System.EventHandler(this.ceilingheight_WhenTextChanged);
			// 
			// sectorheightlabel
			// 
			this.sectorheightlabel.Location = new System.Drawing.Point(9, 113);
			this.sectorheightlabel.Name = "sectorheightlabel";
			this.sectorheightlabel.Size = new System.Drawing.Size(74, 14);
			this.sectorheightlabel.TabIndex = 6;
			this.sectorheightlabel.Text = "Sector height:";
			this.sectorheightlabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// sectorheight
			// 
			this.sectorheight.AutoSize = true;
			this.sectorheight.Location = new System.Drawing.Point(89, 114);
			this.sectorheight.Name = "sectorheight";
			this.sectorheight.Size = new System.Drawing.Size(13, 13);
			this.sectorheight.TabIndex = 7;
			this.sectorheight.Text = "0";
			// 
			// floorheight
			// 
			this.floorheight.AllowDecimal = false;
			this.floorheight.AllowExpressions = true;
			this.floorheight.AllowNegative = true;
			this.floorheight.AllowRelative = true;
			this.floorheight.ButtonStep = 8;
			this.floorheight.ButtonStepBig = 16F;
			this.floorheight.ButtonStepFloat = 1F;
			this.floorheight.ButtonStepSmall = 1F;
			this.floorheight.ButtonStepsUseModifierKeys = true;
			this.floorheight.ButtonStepsWrapAround = false;
			this.floorheight.Location = new System.Drawing.Point(89, 48);
			this.floorheight.Name = "floorheight";
			this.floorheight.Size = new System.Drawing.Size(81, 24);
			this.floorheight.StepValues = null;
			this.floorheight.TabIndex = 3;
			this.floorheight.WhenTextChanged += new System.EventHandler(this.floorheight_WhenTextChanged);
			// 
			// label17
			// 
			label17.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			label17.ForeColor = System.Drawing.SystemColors.HotTrack;
			label17.Location = new System.Drawing.Point(10, 51);
			label17.Name = "label17";
			label17.Size = new System.Drawing.Size(74, 14);
			label17.TabIndex = 3;
			label17.Tag = "damageamount";
			label17.Text = "Amount:";
			label17.TextAlign = System.Drawing.ContentAlignment.TopRight;
			this.tooltip.SetToolTip(label17, "Amount of damage inflicted by this sector.\r\nIf this is 0, all other damage proper" +
        "ties will be ignored.\r\nSetting this to a negative value will create a healing se" +
        "ctor.");
			// 
			// label16
			// 
			label16.Location = new System.Drawing.Point(10, 23);
			label16.Name = "label16";
			label16.Size = new System.Drawing.Size(74, 14);
			label16.TabIndex = 0;
			label16.Tag = "damagetype";
			label16.Text = "Type:";
			label16.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label18
			// 
			label18.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			label18.ForeColor = System.Drawing.SystemColors.HotTrack;
			label18.Location = new System.Drawing.Point(10, 81);
			label18.Name = "label18";
			label18.Size = new System.Drawing.Size(74, 14);
			label18.TabIndex = 5;
			label18.Text = "Interval:";
			label18.TextAlign = System.Drawing.ContentAlignment.TopRight;
			this.tooltip.SetToolTip(label18, "Interval in tics between damage application.");
			// 
			// label19
			// 
			label19.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			label19.ForeColor = System.Drawing.SystemColors.HotTrack;
			label19.Location = new System.Drawing.Point(10, 111);
			label19.Name = "label19";
			label19.Size = new System.Drawing.Size(74, 14);
			label19.TabIndex = 7;
			label19.Text = "Leakiness:";
			label19.TextAlign = System.Drawing.ContentAlignment.TopRight;
			this.tooltip.SetToolTip(label19, "Probability of leaking through radiation suit\r\n(0 = never, 256 = always)");
			// 
			// label13
			// 
			label13.Location = new System.Drawing.Point(35, 83);
			label13.Name = "label13";
			label13.Size = new System.Drawing.Size(74, 14);
			label13.TabIndex = 18;
			label13.Tag = "desaturation";
			label13.Text = "Desaturation:";
			label13.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// tabs
			// 
			this.tabs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tabs.Controls.Add(this.tabproperties);
			this.tabs.Controls.Add(this.tabColors);
			this.tabs.Controls.Add(this.tabSurfaces);
			this.tabs.Controls.Add(this.tabslopes);
			this.tabs.Controls.Add(this.tabcomment);
			this.tabs.Controls.Add(this.tabcustom);
			this.tabs.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.tabs.Location = new System.Drawing.Point(10, 10);
			this.tabs.Margin = new System.Windows.Forms.Padding(1);
			this.tabs.Name = "tabs";
			this.tabs.Padding = new System.Drawing.Point(20, 3);
			this.tabs.SelectedIndex = 0;
			this.tabs.Size = new System.Drawing.Size(578, 552);
			this.tabs.TabIndex = 1;
			// 
			// tabproperties
			// 
			this.tabproperties.Controls.Add(this.groupdamage);
			this.tabproperties.Controls.Add(this.groupBox3);
			this.tabproperties.Controls.Add(groupaction);
			this.tabproperties.Controls.Add(groupeffect);
			this.tabproperties.Controls.Add(groupfloorceiling);
			this.tabproperties.Cursor = System.Windows.Forms.Cursors.Arrow;
			this.tabproperties.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tabproperties.Location = new System.Drawing.Point(4, 22);
			this.tabproperties.Name = "tabproperties";
			this.tabproperties.Padding = new System.Windows.Forms.Padding(3);
			this.tabproperties.Size = new System.Drawing.Size(570, 526);
			this.tabproperties.TabIndex = 0;
			this.tabproperties.Text = "Properties";
			this.tabproperties.UseVisualStyleBackColor = true;
			// 
			// groupdamage
			// 
			this.groupdamage.Controls.Add(label19);
			this.groupdamage.Controls.Add(this.leakiness);
			this.groupdamage.Controls.Add(label18);
			this.groupdamage.Controls.Add(this.damageinterval);
			this.groupdamage.Controls.Add(label16);
			this.groupdamage.Controls.Add(label17);
			this.groupdamage.Controls.Add(this.resetdamagetype);
			this.groupdamage.Controls.Add(this.damageamount);
			this.groupdamage.Controls.Add(this.damagetype);
			this.groupdamage.Location = new System.Drawing.Point(267, 186);
			this.groupdamage.Name = "groupdamage";
			this.groupdamage.Size = new System.Drawing.Size(297, 138);
			this.groupdamage.TabIndex = 4;
			this.groupdamage.TabStop = false;
			this.groupdamage.Text = " Sector damage ";
			// 
			// leakiness
			// 
			this.leakiness.AllowDecimal = false;
			this.leakiness.AllowExpressions = false;
			this.leakiness.AllowNegative = false;
			this.leakiness.AllowRelative = true;
			this.leakiness.ButtonStep = 8;
			this.leakiness.ButtonStepBig = 16F;
			this.leakiness.ButtonStepFloat = 1F;
			this.leakiness.ButtonStepSmall = 1F;
			this.leakiness.ButtonStepsUseModifierKeys = true;
			this.leakiness.ButtonStepsWrapAround = false;
			this.leakiness.Location = new System.Drawing.Point(90, 106);
			this.leakiness.Name = "leakiness";
			this.leakiness.Size = new System.Drawing.Size(81, 24);
			this.leakiness.StepValues = null;
			this.leakiness.TabIndex = 8;
			this.leakiness.Tag = "leakiness";
			// 
			// damageinterval
			// 
			this.damageinterval.AllowDecimal = false;
			this.damageinterval.AllowExpressions = false;
			this.damageinterval.AllowNegative = false;
			this.damageinterval.AllowRelative = true;
			this.damageinterval.ButtonStep = 8;
			this.damageinterval.ButtonStepBig = 16F;
			this.damageinterval.ButtonStepFloat = 1F;
			this.damageinterval.ButtonStepSmall = 1F;
			this.damageinterval.ButtonStepsUseModifierKeys = true;
			this.damageinterval.ButtonStepsWrapAround = false;
			this.damageinterval.Location = new System.Drawing.Point(90, 76);
			this.damageinterval.Name = "damageinterval";
			this.damageinterval.Size = new System.Drawing.Size(81, 24);
			this.damageinterval.StepValues = null;
			this.damageinterval.TabIndex = 6;
			this.damageinterval.Tag = "damageinterval";
			// 
			// resetdamagetype
			// 
			this.resetdamagetype.Image = ((System.Drawing.Image)(resources.GetObject("resetdamagetype.Image")));
			this.resetdamagetype.Location = new System.Drawing.Point(263, 17);
			this.resetdamagetype.Name = "resetdamagetype";
			this.resetdamagetype.Size = new System.Drawing.Size(28, 25);
			this.resetdamagetype.TabIndex = 2;
			this.resetdamagetype.Tag = "damagetype";
			this.resetdamagetype.Text = " ";
			this.tooltip.SetToolTip(this.resetdamagetype, "Reset");
			this.resetdamagetype.UseVisualStyleBackColor = true;
			this.resetdamagetype.Click += new System.EventHandler(this.resetdamagetype_Click);
			// 
			// damageamount
			// 
			this.damageamount.AllowDecimal = false;
			this.damageamount.AllowExpressions = false;
			this.damageamount.AllowNegative = true;
			this.damageamount.AllowRelative = true;
			this.damageamount.ButtonStep = 8;
			this.damageamount.ButtonStepBig = 16F;
			this.damageamount.ButtonStepFloat = 1F;
			this.damageamount.ButtonStepSmall = 1F;
			this.damageamount.ButtonStepsUseModifierKeys = true;
			this.damageamount.ButtonStepsWrapAround = false;
			this.damageamount.Location = new System.Drawing.Point(90, 46);
			this.damageamount.Name = "damageamount";
			this.damageamount.Size = new System.Drawing.Size(81, 24);
			this.damageamount.StepValues = null;
			this.damageamount.TabIndex = 4;
			this.damageamount.Tag = "damageamount";
			// 
			// damagetype
			// 
			this.damagetype.FormattingEnabled = true;
			this.damagetype.Location = new System.Drawing.Point(90, 19);
			this.damagetype.Name = "damagetype";
			this.damagetype.Size = new System.Drawing.Size(167, 21);
			this.damagetype.TabIndex = 1;
			this.damagetype.Tag = "damagetype";
			this.damagetype.TextChanged += new System.EventHandler(this.damagetype_TextChanged);
			this.damagetype.MouseDown += new System.Windows.Forms.MouseEventHandler(this.damagetype_MouseDown);
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.flags);
			this.groupBox3.Location = new System.Drawing.Point(7, 6);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(557, 174);
			this.groupBox3.TabIndex = 0;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = " Flags ";
			// 
			// flags
			// 
			this.flags.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.flags.AutoScroll = true;
			this.flags.Columns = 2;
			this.flags.Location = new System.Drawing.Point(15, 21);
			this.flags.Name = "flags";
			this.flags.Size = new System.Drawing.Size(536, 147);
			this.flags.TabIndex = 0;
			this.flags.VerticalSpacing = 1;
			// 
			// tabColors
			// 
			this.tabColors.Controls.Add(this.groupBox11);
			this.tabColors.Controls.Add(this.groupBox10);
			this.tabColors.Controls.Add(this.groupBox9);
			this.tabColors.Controls.Add(this.groupBox8);
			this.tabColors.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.tabColors.Location = new System.Drawing.Point(4, 22);
			this.tabColors.Name = "tabColors";
			this.tabColors.Size = new System.Drawing.Size(570, 526);
			this.tabColors.TabIndex = 5;
			this.tabColors.Text = "Colors";
			this.tabColors.UseVisualStyleBackColor = true;
			// 
			// groupBox11
			// 
			this.groupBox11.Controls.Add(this.floorGlowEnabled);
			this.groupBox11.Controls.Add(this.floorglowheightrequired);
			this.groupBox11.Controls.Add(this.resetfloorglowheight);
			this.groupBox11.Controls.Add(this.floorglowheightlabel);
			this.groupBox11.Controls.Add(this.floorglowheight);
			this.groupBox11.Controls.Add(this.floorglowcolor);
			this.groupBox11.Location = new System.Drawing.Point(286, 205);
			this.groupBox11.Name = "groupBox11";
			this.groupBox11.Size = new System.Drawing.Size(281, 85);
			this.groupBox11.TabIndex = 20;
			this.groupBox11.TabStop = false;
			this.groupBox11.Text = "                       ";
			// 
			// floorGlowEnabled
			// 
			this.floorGlowEnabled.AutoSize = true;
			this.floorGlowEnabled.Checked = true;
			this.floorGlowEnabled.CheckState = System.Windows.Forms.CheckState.Checked;
			this.floorGlowEnabled.Location = new System.Drawing.Point(11, -1);
			this.floorGlowEnabled.Name = "floorGlowEnabled";
			this.floorGlowEnabled.Size = new System.Drawing.Size(74, 17);
			this.floorGlowEnabled.TabIndex = 33;
			this.floorGlowEnabled.Tag = "floorglowcolor";
			this.floorGlowEnabled.Text = "Floor glow";
			this.floorGlowEnabled.UseVisualStyleBackColor = true;
			this.floorGlowEnabled.CheckedChanged += new System.EventHandler(this.floorGlowEnabled_CheckedChanged);
			// 
			// floorglowheightrequired
			// 
			this.floorglowheightrequired.Image = ((System.Drawing.Image)(resources.GetObject("floorglowheightrequired.Image")));
			this.floorglowheightrequired.Location = new System.Drawing.Point(8, 53);
			this.floorglowheightrequired.Name = "floorglowheightrequired";
			this.floorglowheightrequired.Size = new System.Drawing.Size(16, 16);
			this.floorglowheightrequired.TabIndex = 33;
			this.floorglowheightrequired.TabStop = false;
			this.floorglowheightrequired.Tag = "floorglowheight";
			this.tooltip.SetToolTip(this.floorglowheightrequired, "Non-zero glow height required\r\nfor the glow to be shown ingame!");
			this.floorglowheightrequired.Visible = false;
			// 
			// resetfloorglowheight
			// 
			this.resetfloorglowheight.Image = ((System.Drawing.Image)(resources.GetObject("resetfloorglowheight.Image")));
			this.resetfloorglowheight.Location = new System.Drawing.Point(163, 50);
			this.resetfloorglowheight.Name = "resetfloorglowheight";
			this.resetfloorglowheight.Size = new System.Drawing.Size(23, 23);
			this.resetfloorglowheight.TabIndex = 32;
			this.resetfloorglowheight.Tag = "floorglowheight";
			this.tooltip.SetToolTip(this.resetfloorglowheight, "Reset");
			this.resetfloorglowheight.UseVisualStyleBackColor = true;
			this.resetfloorglowheight.Click += new System.EventHandler(this.resetfloorglowheight_Click);
			// 
			// floorglowheightlabel
			// 
			this.floorglowheightlabel.Location = new System.Drawing.Point(10, 55);
			this.floorglowheightlabel.Name = "floorglowheightlabel";
			this.floorglowheightlabel.Size = new System.Drawing.Size(80, 14);
			this.floorglowheightlabel.TabIndex = 30;
			this.floorglowheightlabel.Tag = "floorglowheight";
			this.floorglowheightlabel.Text = "Glow height:";
			this.floorglowheightlabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// floorglowheight
			// 
			this.floorglowheight.AllowDecimal = true;
			this.floorglowheight.AllowExpressions = false;
			this.floorglowheight.AllowNegative = false;
			this.floorglowheight.AllowRelative = false;
			this.floorglowheight.ButtonStep = 16;
			this.floorglowheight.ButtonStepBig = 64F;
			this.floorglowheight.ButtonStepFloat = 16F;
			this.floorglowheight.ButtonStepSmall = 1F;
			this.floorglowheight.ButtonStepsUseModifierKeys = true;
			this.floorglowheight.ButtonStepsWrapAround = false;
			this.floorglowheight.Location = new System.Drawing.Point(97, 50);
			this.floorglowheight.Name = "floorglowheight";
			this.floorglowheight.Size = new System.Drawing.Size(62, 24);
			this.floorglowheight.StepValues = null;
			this.floorglowheight.TabIndex = 31;
			this.floorglowheight.Tag = "floorglowheight";
			this.floorglowheight.WhenTextChanged += new System.EventHandler(this.floorglowheight_WhenTextChanged);
			// 
			// floorglowcolor
			// 
			this.floorglowcolor.DefaultValue = 0;
			this.floorglowcolor.Field = "floorglowcolor";
			this.floorglowcolor.Label = "Glow color:";
			this.floorglowcolor.Location = new System.Drawing.Point(28, 19);
			this.floorglowcolor.Name = "floorglowcolor";
			this.floorglowcolor.Size = new System.Drawing.Size(210, 29);
			this.floorglowcolor.TabIndex = 28;
			this.floorglowcolor.Tag = "floorglowcolor";
			this.floorglowcolor.OnValueChanged += new System.EventHandler(this.floorglowcolor_OnValueChanged);
			// 
			// groupBox10
			// 
			this.groupBox10.Controls.Add(this.ceilingGlowEnabled);
			this.groupBox10.Controls.Add(this.ceilingglowheightrequired);
			this.groupBox10.Controls.Add(this.resetceilingglowheight);
			this.groupBox10.Controls.Add(this.ceilingglowheightlabel);
			this.groupBox10.Controls.Add(this.ceilingglowheight);
			this.groupBox10.Controls.Add(this.ceilingglowcolor);
			this.groupBox10.Location = new System.Drawing.Point(3, 205);
			this.groupBox10.Name = "groupBox10";
			this.groupBox10.Size = new System.Drawing.Size(277, 85);
			this.groupBox10.TabIndex = 19;
			this.groupBox10.TabStop = false;
			this.groupBox10.Text = "                          ";
			// 
			// ceilingGlowEnabled
			// 
			this.ceilingGlowEnabled.AutoSize = true;
			this.ceilingGlowEnabled.Checked = true;
			this.ceilingGlowEnabled.CheckState = System.Windows.Forms.CheckState.Checked;
			this.ceilingGlowEnabled.Location = new System.Drawing.Point(11, -1);
			this.ceilingGlowEnabled.Name = "ceilingGlowEnabled";
			this.ceilingGlowEnabled.Size = new System.Drawing.Size(82, 17);
			this.ceilingGlowEnabled.TabIndex = 21;
			this.ceilingGlowEnabled.Tag = "ceilingglowcolor";
			this.ceilingGlowEnabled.Text = "Ceiling glow";
			this.ceilingGlowEnabled.UseVisualStyleBackColor = true;
			this.ceilingGlowEnabled.CheckedChanged += new System.EventHandler(this.ceilingGlowEnabled_CheckedChanged);
			// 
			// ceilingglowheightrequired
			// 
			this.ceilingglowheightrequired.Image = ((System.Drawing.Image)(resources.GetObject("ceilingglowheightrequired.Image")));
			this.ceilingglowheightrequired.Location = new System.Drawing.Point(10, 53);
			this.ceilingglowheightrequired.Name = "ceilingglowheightrequired";
			this.ceilingglowheightrequired.Size = new System.Drawing.Size(16, 16);
			this.ceilingglowheightrequired.TabIndex = 32;
			this.ceilingglowheightrequired.TabStop = false;
			this.ceilingglowheightrequired.Tag = "ceilingglowheight";
			this.tooltip.SetToolTip(this.ceilingglowheightrequired, "Non-zero glow height required\r\nfor the glow to be shown ingame!");
			this.ceilingglowheightrequired.Visible = false;
			// 
			// resetceilingglowheight
			// 
			this.resetceilingglowheight.Image = ((System.Drawing.Image)(resources.GetObject("resetceilingglowheight.Image")));
			this.resetceilingglowheight.Location = new System.Drawing.Point(165, 50);
			this.resetceilingglowheight.Name = "resetceilingglowheight";
			this.resetceilingglowheight.Size = new System.Drawing.Size(23, 23);
			this.resetceilingglowheight.TabIndex = 31;
			this.resetceilingglowheight.Tag = "ceilingglowheight";
			this.tooltip.SetToolTip(this.resetceilingglowheight, "Reset");
			this.resetceilingglowheight.UseVisualStyleBackColor = true;
			this.resetceilingglowheight.Click += new System.EventHandler(this.resetceilingglowheight_Click);
			// 
			// ceilingglowheightlabel
			// 
			this.ceilingglowheightlabel.Location = new System.Drawing.Point(12, 55);
			this.ceilingglowheightlabel.Name = "ceilingglowheightlabel";
			this.ceilingglowheightlabel.Size = new System.Drawing.Size(80, 14);
			this.ceilingglowheightlabel.TabIndex = 29;
			this.ceilingglowheightlabel.Tag = "ceilingglowheight";
			this.ceilingglowheightlabel.Text = "Glow height:";
			this.ceilingglowheightlabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// ceilingglowheight
			// 
			this.ceilingglowheight.AllowDecimal = true;
			this.ceilingglowheight.AllowExpressions = false;
			this.ceilingglowheight.AllowNegative = false;
			this.ceilingglowheight.AllowRelative = false;
			this.ceilingglowheight.ButtonStep = 16;
			this.ceilingglowheight.ButtonStepBig = 64F;
			this.ceilingglowheight.ButtonStepFloat = 16F;
			this.ceilingglowheight.ButtonStepSmall = 1F;
			this.ceilingglowheight.ButtonStepsUseModifierKeys = true;
			this.ceilingglowheight.ButtonStepsWrapAround = false;
			this.ceilingglowheight.Location = new System.Drawing.Point(99, 50);
			this.ceilingglowheight.Name = "ceilingglowheight";
			this.ceilingglowheight.Size = new System.Drawing.Size(62, 24);
			this.ceilingglowheight.StepValues = null;
			this.ceilingglowheight.TabIndex = 30;
			this.ceilingglowheight.Tag = "ceilingglowheight";
			this.ceilingglowheight.WhenTextChanged += new System.EventHandler(this.ceilingglowheight_WhenTextChanged);
			// 
			// ceilingglowcolor
			// 
			this.ceilingglowcolor.DefaultValue = 0;
			this.ceilingglowcolor.Field = "ceilingglowcolor";
			this.ceilingglowcolor.Label = "Glow color:";
			this.ceilingglowcolor.Location = new System.Drawing.Point(30, 19);
			this.ceilingglowcolor.Name = "ceilingglowcolor";
			this.ceilingglowcolor.Size = new System.Drawing.Size(210, 29);
			this.ceilingglowcolor.TabIndex = 27;
			this.ceilingglowcolor.Tag = "ceilingglowcolor";
			this.ceilingglowcolor.OnValueChanged += new System.EventHandler(this.ceilingglowcolor_OnValueChanged);
			// 
			// groupBox9
			// 
			this.groupBox9.Controls.Add(this.thingsColor);
			this.groupBox9.Controls.Add(this.lowerWallColor);
			this.groupBox9.Controls.Add(this.upperWallColor);
			this.groupBox9.Controls.Add(this.floorColor);
			this.groupBox9.Controls.Add(this.ceilingColor);
			this.groupBox9.Location = new System.Drawing.Point(286, 3);
			this.groupBox9.Name = "groupBox9";
			this.groupBox9.Size = new System.Drawing.Size(281, 196);
			this.groupBox9.TabIndex = 19;
			this.groupBox9.TabStop = false;
			this.groupBox9.Text = "Doom 64-like sector colors";
			// 
			// thingsColor
			// 
			this.thingsColor.DefaultValue = 16777215;
			this.thingsColor.Field = "color_sprites";
			this.thingsColor.Label = "Things:";
			this.thingsColor.Location = new System.Drawing.Point(6, 89);
			this.thingsColor.Name = "thingsColor";
			this.thingsColor.Size = new System.Drawing.Size(207, 29);
			this.thingsColor.TabIndex = 22;
			this.thingsColor.Tag = "color_sprites";
			this.thingsColor.OnValueChanged += new System.EventHandler(this.d64color_OnValueChanged);
			// 
			// lowerWallColor
			// 
			this.lowerWallColor.DefaultValue = 16777215;
			this.lowerWallColor.Field = "color_wallbottom";
			this.lowerWallColor.Label = "Lower wall:";
			this.lowerWallColor.Location = new System.Drawing.Point(6, 124);
			this.lowerWallColor.Name = "lowerWallColor";
			this.lowerWallColor.Size = new System.Drawing.Size(207, 29);
			this.lowerWallColor.TabIndex = 21;
			this.lowerWallColor.Tag = "color_wallbottom";
			this.lowerWallColor.OnValueChanged += new System.EventHandler(this.d64color_OnValueChanged);
			// 
			// upperWallColor
			// 
			this.upperWallColor.DefaultValue = 16777215;
			this.upperWallColor.Field = "color_walltop";
			this.upperWallColor.Label = "Upper wall:";
			this.upperWallColor.Location = new System.Drawing.Point(6, 54);
			this.upperWallColor.Name = "upperWallColor";
			this.upperWallColor.Size = new System.Drawing.Size(207, 29);
			this.upperWallColor.TabIndex = 20;
			this.upperWallColor.Tag = "color_walltop";
			this.upperWallColor.OnValueChanged += new System.EventHandler(this.d64color_OnValueChanged);
			// 
			// floorColor
			// 
			this.floorColor.DefaultValue = 16777215;
			this.floorColor.Field = "color_floor";
			this.floorColor.Label = "Floor:";
			this.floorColor.Location = new System.Drawing.Point(6, 159);
			this.floorColor.Name = "floorColor";
			this.floorColor.Size = new System.Drawing.Size(207, 29);
			this.floorColor.TabIndex = 19;
			this.floorColor.Tag = "color_floor";
			this.floorColor.OnValueChanged += new System.EventHandler(this.d64color_OnValueChanged);
			// 
			// ceilingColor
			// 
			this.ceilingColor.DefaultValue = 16777215;
			this.ceilingColor.Field = "color_ceiling";
			this.ceilingColor.Label = "Ceiling:";
			this.ceilingColor.Location = new System.Drawing.Point(6, 19);
			this.ceilingColor.Name = "ceilingColor";
			this.ceilingColor.Size = new System.Drawing.Size(207, 29);
			this.ceilingColor.TabIndex = 18;
			this.ceilingColor.Tag = "color_ceiling";
			this.ceilingColor.OnValueChanged += new System.EventHandler(this.d64color_OnValueChanged);
			// 
			// groupBox8
			// 
			this.groupBox8.Controls.Add(this.desaturation);
			this.groupBox8.Controls.Add(label13);
			this.groupBox8.Controls.Add(this.lightColor);
			this.groupBox8.Controls.Add(this.fadeColor);
			this.groupBox8.Location = new System.Drawing.Point(3, 3);
			this.groupBox8.Name = "groupBox8";
			this.groupBox8.Size = new System.Drawing.Size(277, 196);
			this.groupBox8.TabIndex = 18;
			this.groupBox8.TabStop = false;
			this.groupBox8.Text = "Global sector colors";
			// 
			// desaturation
			// 
			this.desaturation.AllowDecimal = true;
			this.desaturation.AllowExpressions = false;
			this.desaturation.AllowNegative = false;
			this.desaturation.AllowRelative = false;
			this.desaturation.ButtonStep = 1;
			this.desaturation.ButtonStepBig = 0.25F;
			this.desaturation.ButtonStepFloat = 0.1F;
			this.desaturation.ButtonStepSmall = 0.01F;
			this.desaturation.ButtonStepsUseModifierKeys = true;
			this.desaturation.ButtonStepsWrapAround = false;
			this.desaturation.Location = new System.Drawing.Point(115, 78);
			this.desaturation.Name = "desaturation";
			this.desaturation.Size = new System.Drawing.Size(84, 24);
			this.desaturation.StepValues = null;
			this.desaturation.TabIndex = 19;
			this.desaturation.Tag = "desaturation";
			// 
			// lightColor
			// 
			this.lightColor.DefaultValue = 16777215;
			this.lightColor.Field = "lightcolor";
			this.lightColor.Label = "Light:";
			this.lightColor.Location = new System.Drawing.Point(6, 19);
			this.lightColor.Name = "lightColor";
			this.lightColor.Size = new System.Drawing.Size(207, 29);
			this.lightColor.TabIndex = 16;
			this.lightColor.Tag = "lightcolor";
			this.lightColor.OnValueChanged += new System.EventHandler(this.lightColor_OnValueChanged);
			// 
			// fadeColor
			// 
			this.fadeColor.DefaultValue = 0;
			this.fadeColor.Field = "fadecolor";
			this.fadeColor.Label = "Fade:";
			this.fadeColor.Location = new System.Drawing.Point(6, 47);
			this.fadeColor.Name = "fadeColor";
			this.fadeColor.Size = new System.Drawing.Size(207, 31);
			this.fadeColor.TabIndex = 17;
			this.fadeColor.Tag = "fadecolor";
			this.fadeColor.OnValueChanged += new System.EventHandler(this.fadeColor_OnValueChanged);
			// 
			// tabSurfaces
			// 
			this.tabSurfaces.Controls.Add(this.groupBox2);
			this.tabSurfaces.Controls.Add(this.groupBox1);
			this.tabSurfaces.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tabSurfaces.Location = new System.Drawing.Point(4, 22);
			this.tabSurfaces.Name = "tabSurfaces";
			this.tabSurfaces.Size = new System.Drawing.Size(570, 526);
			this.tabSurfaces.TabIndex = 2;
			this.tabSurfaces.Text = "Surfaces";
			this.tabSurfaces.UseVisualStyleBackColor = true;
			// 
			// groupBox2
			// 
			this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox2.Controls.Add(this.reset_floor_reflect);
			this.groupBox2.Controls.Add(this.label23);
			this.groupBox2.Controls.Add(this.floor_reflect);
			this.groupBox2.Controls.Add(this.resetfloorterrain);
			this.groupBox2.Controls.Add(this.label3);
			this.groupBox2.Controls.Add(this.floorterrain);
			this.groupBox2.Controls.Add(this.resetfloorlight);
			this.groupBox2.Controls.Add(this.labelFloorOffsets);
			this.groupBox2.Controls.Add(this.labelFloorScale);
			this.groupBox2.Controls.Add(this.cbUseFloorLineAngles);
			this.groupBox2.Controls.Add(this.floorAngleControl);
			this.groupBox2.Controls.Add(this.labelfloorrenderstyle);
			this.groupBox2.Controls.Add(this.label11);
			this.groupBox2.Controls.Add(this.floorRotation);
			this.groupBox2.Controls.Add(this.floorLightAbsolute);
			this.groupBox2.Controls.Add(this.label12);
			this.groupBox2.Controls.Add(this.floorBrightness);
			this.groupBox2.Controls.Add(this.floorRenderStyle);
			this.groupBox2.Controls.Add(this.floorScale);
			this.groupBox2.Controls.Add(this.floorOffsets);
			this.groupBox2.Controls.Add(this.floortex);
			this.groupBox2.Location = new System.Drawing.Point(3, 244);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(564, 235);
			this.groupBox2.TabIndex = 55;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = " Floor ";
			// 
			// reset_floor_reflect
			// 
			this.reset_floor_reflect.Image = ((System.Drawing.Image)(resources.GetObject("reset_floor_reflect.Image")));
			this.reset_floor_reflect.Location = new System.Drawing.Point(179, 198);
			this.reset_floor_reflect.Name = "reset_floor_reflect";
			this.reset_floor_reflect.Size = new System.Drawing.Size(23, 23);
			this.reset_floor_reflect.TabIndex = 19;
			this.reset_floor_reflect.Tag = "floor_reflect";
			this.tooltip.SetToolTip(this.reset_floor_reflect, "Reset");
			this.reset_floor_reflect.UseVisualStyleBackColor = true;
			this.reset_floor_reflect.Click += new System.EventHandler(this.reset_floor_reflect_Click);
			// 
			// label23
			// 
			this.label23.Location = new System.Drawing.Point(26, 202);
			this.label23.Name = "label23";
			this.label23.Size = new System.Drawing.Size(80, 14);
			this.label23.TabIndex = 17;
			this.label23.Tag = "floor_reflect";
			this.label23.Text = "Reflectivity:";
			this.label23.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// floor_reflect
			// 
			this.floor_reflect.AllowDecimal = true;
			this.floor_reflect.AllowExpressions = false;
			this.floor_reflect.AllowNegative = false;
			this.floor_reflect.AllowRelative = false;
			this.floor_reflect.ButtonStep = 1;
			this.floor_reflect.ButtonStepBig = 0.25F;
			this.floor_reflect.ButtonStepFloat = 0.1F;
			this.floor_reflect.ButtonStepSmall = 0.01F;
			this.floor_reflect.ButtonStepsUseModifierKeys = true;
			this.floor_reflect.ButtonStepsWrapAround = false;
			this.floor_reflect.Location = new System.Drawing.Point(113, 197);
			this.floor_reflect.Name = "floor_reflect";
			this.floor_reflect.Size = new System.Drawing.Size(62, 24);
			this.floor_reflect.StepValues = null;
			this.floor_reflect.TabIndex = 18;
			this.floor_reflect.Tag = "floor_reflect";
			this.floor_reflect.WhenTextChanged += new System.EventHandler(this.floor_reflect_WhenTextChanged);
			// 
			// resetfloorterrain
			// 
			this.resetfloorterrain.Image = ((System.Drawing.Image)(resources.GetObject("resetfloorterrain.Image")));
			this.resetfloorterrain.Location = new System.Drawing.Point(246, 110);
			this.resetfloorterrain.Name = "resetfloorterrain";
			this.resetfloorterrain.Size = new System.Drawing.Size(23, 23);
			this.resetfloorterrain.TabIndex = 8;
			this.resetfloorterrain.Tag = "floorterrain";
			this.resetfloorterrain.Text = " ";
			this.tooltip.SetToolTip(this.resetfloorterrain, "Reset");
			this.resetfloorterrain.UseVisualStyleBackColor = true;
			this.resetfloorterrain.Click += new System.EventHandler(this.resetfloorterrain_Click);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(26, 114);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(80, 14);
			this.label3.TabIndex = 6;
			this.label3.Tag = "floorterrain";
			this.label3.Text = "Terrain:";
			this.label3.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// floorterrain
			// 
			this.floorterrain.FormattingEnabled = true;
			this.floorterrain.Location = new System.Drawing.Point(113, 111);
			this.floorterrain.Name = "floorterrain";
			this.floorterrain.Size = new System.Drawing.Size(130, 21);
			this.floorterrain.TabIndex = 7;
			this.floorterrain.Tag = "floorterrain";
			this.floorterrain.TextChanged += new System.EventHandler(this.floorterrain_TextChanged);
			this.floorterrain.MouseDown += new System.Windows.Forms.MouseEventHandler(this.floorterrain_MouseDown);
			// 
			// resetfloorlight
			// 
			this.resetfloorlight.Image = ((System.Drawing.Image)(resources.GetObject("resetfloorlight.Image")));
			this.resetfloorlight.Location = new System.Drawing.Point(246, 138);
			this.resetfloorlight.Name = "resetfloorlight";
			this.resetfloorlight.Size = new System.Drawing.Size(23, 23);
			this.resetfloorlight.TabIndex = 12;
			this.resetfloorlight.Tag = "lightfloor";
			this.tooltip.SetToolTip(this.resetfloorlight, "Reset");
			this.resetfloorlight.UseVisualStyleBackColor = true;
			this.resetfloorlight.Click += new System.EventHandler(this.resetfloorlight_Click);
			// 
			// labelFloorOffsets
			// 
			this.labelFloorOffsets.Location = new System.Drawing.Point(8, 27);
			this.labelFloorOffsets.Name = "labelFloorOffsets";
			this.labelFloorOffsets.Size = new System.Drawing.Size(98, 14);
			this.labelFloorOffsets.TabIndex = 0;
			this.labelFloorOffsets.Tag = "xpanningfloor";
			this.labelFloorOffsets.Text = "Texture offsets:";
			this.labelFloorOffsets.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// labelFloorScale
			// 
			this.labelFloorScale.Location = new System.Drawing.Point(8, 59);
			this.labelFloorScale.Name = "labelFloorScale";
			this.labelFloorScale.Size = new System.Drawing.Size(98, 14);
			this.labelFloorScale.TabIndex = 2;
			this.labelFloorScale.Tag = "xscalefloor";
			this.labelFloorScale.Text = "Texture scale:";
			this.labelFloorScale.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// cbUseFloorLineAngles
			// 
			this.cbUseFloorLineAngles.AutoSize = true;
			this.cbUseFloorLineAngles.Location = new System.Drawing.Point(181, 172);
			this.cbUseFloorLineAngles.Name = "cbUseFloorLineAngles";
			this.cbUseFloorLineAngles.Size = new System.Drawing.Size(113, 17);
			this.cbUseFloorLineAngles.TabIndex = 16;
			this.cbUseFloorLineAngles.Tag = "rotationfloor";
			this.cbUseFloorLineAngles.Text = "Use linedef angles";
			this.cbUseFloorLineAngles.UseVisualStyleBackColor = true;
			this.cbUseFloorLineAngles.CheckedChanged += new System.EventHandler(this.cbUseFloorLineAngles_CheckedChanged);
			// 
			// floorAngleControl
			// 
			this.floorAngleControl.Angle = -2700;
			this.floorAngleControl.AngleOffset = 90;
			this.floorAngleControl.DoomAngleClamping = false;
			this.floorAngleControl.Location = new System.Drawing.Point(6, 156);
			this.floorAngleControl.Name = "floorAngleControl";
			this.floorAngleControl.Size = new System.Drawing.Size(44, 44);
			this.floorAngleControl.TabIndex = 13;
			this.floorAngleControl.Tag = "rotationfloor";
			this.floorAngleControl.AngleChanged += new System.EventHandler(this.floorAngleControl_AngleChanged);
			// 
			// labelfloorrenderstyle
			// 
			this.labelfloorrenderstyle.Location = new System.Drawing.Point(26, 88);
			this.labelfloorrenderstyle.Name = "labelfloorrenderstyle";
			this.labelfloorrenderstyle.Size = new System.Drawing.Size(80, 14);
			this.labelfloorrenderstyle.TabIndex = 4;
			this.labelfloorrenderstyle.Tag = "renderstylefloor";
			this.labelfloorrenderstyle.Text = "Render style:";
			this.labelfloorrenderstyle.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label11
			// 
			this.label11.Location = new System.Drawing.Point(26, 172);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(80, 14);
			this.label11.TabIndex = 14;
			this.label11.Tag = "rotationfloor";
			this.label11.Text = "Rotation:";
			this.label11.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// floorRotation
			// 
			this.floorRotation.AllowDecimal = true;
			this.floorRotation.AllowExpressions = false;
			this.floorRotation.AllowNegative = true;
			this.floorRotation.AllowRelative = true;
			this.floorRotation.ButtonStep = 5;
			this.floorRotation.ButtonStepBig = 15F;
			this.floorRotation.ButtonStepFloat = 1F;
			this.floorRotation.ButtonStepSmall = 0.1F;
			this.floorRotation.ButtonStepsUseModifierKeys = true;
			this.floorRotation.ButtonStepsWrapAround = false;
			this.floorRotation.Location = new System.Drawing.Point(113, 167);
			this.floorRotation.Name = "floorRotation";
			this.floorRotation.Size = new System.Drawing.Size(62, 24);
			this.floorRotation.StepValues = null;
			this.floorRotation.TabIndex = 15;
			this.floorRotation.Tag = "rotationfloor";
			this.floorRotation.WhenTextChanged += new System.EventHandler(this.floorRotation_WhenTextChanged);
			// 
			// floorLightAbsolute
			// 
			this.floorLightAbsolute.AutoSize = true;
			this.floorLightAbsolute.Location = new System.Drawing.Point(181, 142);
			this.floorLightAbsolute.Name = "floorLightAbsolute";
			this.floorLightAbsolute.Size = new System.Drawing.Size(67, 17);
			this.floorLightAbsolute.TabIndex = 11;
			this.floorLightAbsolute.Tag = "lightfloorabsolute";
			this.floorLightAbsolute.Text = "Absolute";
			this.floorLightAbsolute.UseVisualStyleBackColor = true;
			this.floorLightAbsolute.CheckedChanged += new System.EventHandler(this.floorLightAbsolute_CheckedChanged);
			// 
			// label12
			// 
			this.label12.Location = new System.Drawing.Point(26, 142);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(80, 14);
			this.label12.TabIndex = 9;
			this.label12.Tag = "lightfloor";
			this.label12.Text = "Brightness:";
			this.label12.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// floorBrightness
			// 
			this.floorBrightness.AllowDecimal = false;
			this.floorBrightness.AllowExpressions = false;
			this.floorBrightness.AllowNegative = true;
			this.floorBrightness.AllowRelative = true;
			this.floorBrightness.ButtonStep = 16;
			this.floorBrightness.ButtonStepBig = 32F;
			this.floorBrightness.ButtonStepFloat = 1F;
			this.floorBrightness.ButtonStepSmall = 1F;
			this.floorBrightness.ButtonStepsUseModifierKeys = true;
			this.floorBrightness.ButtonStepsWrapAround = false;
			this.floorBrightness.Location = new System.Drawing.Point(113, 137);
			this.floorBrightness.Name = "floorBrightness";
			this.floorBrightness.Size = new System.Drawing.Size(62, 24);
			this.floorBrightness.StepValues = null;
			this.floorBrightness.TabIndex = 10;
			this.floorBrightness.Tag = "lightfloor";
			this.floorBrightness.WhenTextChanged += new System.EventHandler(this.floorBrightness_WhenTextChanged);
			// 
			// floorRenderStyle
			// 
			this.floorRenderStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.floorRenderStyle.FormattingEnabled = true;
			this.floorRenderStyle.Location = new System.Drawing.Point(113, 85);
			this.floorRenderStyle.Name = "floorRenderStyle";
			this.floorRenderStyle.Size = new System.Drawing.Size(130, 21);
			this.floorRenderStyle.TabIndex = 5;
			this.floorRenderStyle.Tag = "renderstylefloor";
			// 
			// floorScale
			// 
			this.floorScale.AllowDecimal = true;
			this.floorScale.AllowValueLinking = true;
			this.floorScale.ButtonStep = 1;
			this.floorScale.ButtonStepBig = 1F;
			this.floorScale.ButtonStepFloat = 0.1F;
			this.floorScale.ButtonStepSmall = 0.01F;
			this.floorScale.ButtonStepsUseModifierKeys = true;
			this.floorScale.DefaultValue = 1F;
			this.floorScale.Field1 = "xscalefloor";
			this.floorScale.Field2 = "yscalefloor";
			this.floorScale.LinkValues = false;
			this.floorScale.Location = new System.Drawing.Point(110, 53);
			this.floorScale.Name = "floorScale";
			this.floorScale.Size = new System.Drawing.Size(186, 26);
			this.floorScale.TabIndex = 3;
			this.floorScale.Tag = "xscalefloor";
			this.floorScale.OnValuesChanged += new System.EventHandler(this.floorScale_OnValuesChanged);
			// 
			// floorOffsets
			// 
			this.floorOffsets.AllowDecimal = true;
			this.floorOffsets.AllowValueLinking = false;
			this.floorOffsets.ButtonStep = 1;
			this.floorOffsets.ButtonStepBig = 32F;
			this.floorOffsets.ButtonStepFloat = 16F;
			this.floorOffsets.ButtonStepSmall = 1F;
			this.floorOffsets.ButtonStepsUseModifierKeys = true;
			this.floorOffsets.DefaultValue = 0F;
			this.floorOffsets.Field1 = "xpanningfloor";
			this.floorOffsets.Field2 = "ypanningfloor";
			this.floorOffsets.LinkValues = false;
			this.floorOffsets.Location = new System.Drawing.Point(110, 21);
			this.floorOffsets.Name = "floorOffsets";
			this.floorOffsets.Size = new System.Drawing.Size(186, 26);
			this.floorOffsets.TabIndex = 1;
			this.floorOffsets.Tag = "xpanningfloor";
			this.floorOffsets.OnValuesChanged += new System.EventHandler(this.floorOffsets_OnValuesChanged);
			// 
			// floortex
			// 
			this.floortex.Location = new System.Drawing.Point(356, 21);
			this.floortex.MultipleTextures = false;
			this.floortex.Name = "floortex";
			this.floortex.Size = new System.Drawing.Size(176, 200);
			this.floortex.TabIndex = 25;
			this.floortex.TextureName = "";
			this.floortex.OnValueChanged += new System.EventHandler(this.floortex_OnValueChanged);
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.reset_ceiling_reflect);
			this.groupBox1.Controls.Add(this.label20);
			this.groupBox1.Controls.Add(this.ceiling_reflect);
			this.groupBox1.Controls.Add(this.resetceilterrain);
			this.groupBox1.Controls.Add(this.label7);
			this.groupBox1.Controls.Add(this.ceilterrain);
			this.groupBox1.Controls.Add(this.resetceillight);
			this.groupBox1.Controls.Add(this.labelCeilOffsets);
			this.groupBox1.Controls.Add(this.labelCeilScale);
			this.groupBox1.Controls.Add(this.cbUseCeilLineAngles);
			this.groupBox1.Controls.Add(this.ceilAngleControl);
			this.groupBox1.Controls.Add(this.labelceilrenderstyle);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Controls.Add(this.ceilRotation);
			this.groupBox1.Controls.Add(this.ceilLightAbsolute);
			this.groupBox1.Controls.Add(this.labelLightFront);
			this.groupBox1.Controls.Add(this.ceilBrightness);
			this.groupBox1.Controls.Add(this.ceilRenderStyle);
			this.groupBox1.Controls.Add(this.ceilScale);
			this.groupBox1.Controls.Add(this.ceilOffsets);
			this.groupBox1.Controls.Add(this.ceilingtex);
			this.groupBox1.Location = new System.Drawing.Point(3, 3);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(564, 235);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = " Ceiling ";
			// 
			// reset_ceiling_reflect
			// 
			this.reset_ceiling_reflect.Image = ((System.Drawing.Image)(resources.GetObject("reset_ceiling_reflect.Image")));
			this.reset_ceiling_reflect.Location = new System.Drawing.Point(179, 198);
			this.reset_ceiling_reflect.Name = "reset_ceiling_reflect";
			this.reset_ceiling_reflect.Size = new System.Drawing.Size(23, 23);
			this.reset_ceiling_reflect.TabIndex = 19;
			this.reset_ceiling_reflect.Tag = "ceiling_reflect";
			this.tooltip.SetToolTip(this.reset_ceiling_reflect, "Reset");
			this.reset_ceiling_reflect.UseVisualStyleBackColor = true;
			this.reset_ceiling_reflect.Click += new System.EventHandler(this.reset_ceiling_reflect_Click);
			// 
			// label20
			// 
			this.label20.Location = new System.Drawing.Point(26, 202);
			this.label20.Name = "label20";
			this.label20.Size = new System.Drawing.Size(80, 14);
			this.label20.TabIndex = 17;
			this.label20.Tag = "ceiling_reflect";
			this.label20.Text = "Reflectivity:";
			this.label20.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// ceiling_reflect
			// 
			this.ceiling_reflect.AllowDecimal = true;
			this.ceiling_reflect.AllowExpressions = false;
			this.ceiling_reflect.AllowNegative = false;
			this.ceiling_reflect.AllowRelative = false;
			this.ceiling_reflect.ButtonStep = 1;
			this.ceiling_reflect.ButtonStepBig = 0.25F;
			this.ceiling_reflect.ButtonStepFloat = 0.1F;
			this.ceiling_reflect.ButtonStepSmall = 0.01F;
			this.ceiling_reflect.ButtonStepsUseModifierKeys = true;
			this.ceiling_reflect.ButtonStepsWrapAround = false;
			this.ceiling_reflect.Location = new System.Drawing.Point(113, 197);
			this.ceiling_reflect.Name = "ceiling_reflect";
			this.ceiling_reflect.Size = new System.Drawing.Size(62, 24);
			this.ceiling_reflect.StepValues = null;
			this.ceiling_reflect.TabIndex = 18;
			this.ceiling_reflect.Tag = "ceiling_reflect";
			this.ceiling_reflect.WhenTextChanged += new System.EventHandler(this.ceiling_reflect_WhenTextChanged);
			// 
			// resetceilterrain
			// 
			this.resetceilterrain.Image = ((System.Drawing.Image)(resources.GetObject("resetceilterrain.Image")));
			this.resetceilterrain.Location = new System.Drawing.Point(246, 110);
			this.resetceilterrain.Name = "resetceilterrain";
			this.resetceilterrain.Size = new System.Drawing.Size(23, 23);
			this.resetceilterrain.TabIndex = 8;
			this.resetceilterrain.Tag = "ceilingterrain";
			this.resetceilterrain.Text = " ";
			this.tooltip.SetToolTip(this.resetceilterrain, "Reset");
			this.resetceilterrain.UseVisualStyleBackColor = true;
			this.resetceilterrain.Click += new System.EventHandler(this.resetceilterrain_Click);
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(26, 114);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(80, 14);
			this.label7.TabIndex = 6;
			this.label7.Tag = "ceilingterrain";
			this.label7.Text = "Terrain:";
			this.label7.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// ceilterrain
			// 
			this.ceilterrain.FormattingEnabled = true;
			this.ceilterrain.Location = new System.Drawing.Point(113, 111);
			this.ceilterrain.Name = "ceilterrain";
			this.ceilterrain.Size = new System.Drawing.Size(130, 21);
			this.ceilterrain.TabIndex = 7;
			this.ceilterrain.Tag = "ceilingterrain";
			this.ceilterrain.TextChanged += new System.EventHandler(this.ceilterrain_TextChanged);
			this.ceilterrain.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ceilterrain_MouseDown);
			// 
			// resetceillight
			// 
			this.resetceillight.Image = ((System.Drawing.Image)(resources.GetObject("resetceillight.Image")));
			this.resetceillight.Location = new System.Drawing.Point(246, 138);
			this.resetceillight.Name = "resetceillight";
			this.resetceillight.Size = new System.Drawing.Size(23, 23);
			this.resetceillight.TabIndex = 12;
			this.resetceillight.Tag = "lightceiling";
			this.tooltip.SetToolTip(this.resetceillight, "Reset");
			this.resetceillight.UseVisualStyleBackColor = true;
			this.resetceillight.Click += new System.EventHandler(this.resetceillight_Click);
			// 
			// labelCeilOffsets
			// 
			this.labelCeilOffsets.Location = new System.Drawing.Point(8, 27);
			this.labelCeilOffsets.Name = "labelCeilOffsets";
			this.labelCeilOffsets.Size = new System.Drawing.Size(98, 14);
			this.labelCeilOffsets.TabIndex = 0;
			this.labelCeilOffsets.Tag = "xpanningceiling";
			this.labelCeilOffsets.Text = "Texture offsets:";
			this.labelCeilOffsets.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// labelCeilScale
			// 
			this.labelCeilScale.Location = new System.Drawing.Point(8, 59);
			this.labelCeilScale.Name = "labelCeilScale";
			this.labelCeilScale.Size = new System.Drawing.Size(98, 14);
			this.labelCeilScale.TabIndex = 2;
			this.labelCeilScale.Tag = "xscaleceiling";
			this.labelCeilScale.Text = "Texture scale:";
			this.labelCeilScale.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// cbUseCeilLineAngles
			// 
			this.cbUseCeilLineAngles.AutoSize = true;
			this.cbUseCeilLineAngles.Location = new System.Drawing.Point(181, 172);
			this.cbUseCeilLineAngles.Name = "cbUseCeilLineAngles";
			this.cbUseCeilLineAngles.Size = new System.Drawing.Size(113, 17);
			this.cbUseCeilLineAngles.TabIndex = 16;
			this.cbUseCeilLineAngles.Tag = "rotationceiling";
			this.cbUseCeilLineAngles.Text = "Use linedef angles";
			this.cbUseCeilLineAngles.UseVisualStyleBackColor = true;
			this.cbUseCeilLineAngles.CheckedChanged += new System.EventHandler(this.cbUseCeilLineAngles_CheckedChanged);
			// 
			// ceilAngleControl
			// 
			this.ceilAngleControl.Angle = -2700;
			this.ceilAngleControl.AngleOffset = 90;
			this.ceilAngleControl.DoomAngleClamping = false;
			this.ceilAngleControl.Location = new System.Drawing.Point(6, 156);
			this.ceilAngleControl.Name = "ceilAngleControl";
			this.ceilAngleControl.Size = new System.Drawing.Size(44, 44);
			this.ceilAngleControl.TabIndex = 13;
			this.ceilAngleControl.Tag = "rotationceiling";
			this.ceilAngleControl.AngleChanged += new System.EventHandler(this.ceilAngleControl_AngleChanged);
			// 
			// labelceilrenderstyle
			// 
			this.labelceilrenderstyle.Location = new System.Drawing.Point(26, 88);
			this.labelceilrenderstyle.Name = "labelceilrenderstyle";
			this.labelceilrenderstyle.Size = new System.Drawing.Size(80, 14);
			this.labelceilrenderstyle.TabIndex = 4;
			this.labelceilrenderstyle.Tag = "renderstyleceiling";
			this.labelceilrenderstyle.Text = "Render style:";
			this.labelceilrenderstyle.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(26, 172);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(80, 14);
			this.label1.TabIndex = 14;
			this.label1.Tag = "rotationceiling";
			this.label1.Text = "Rotation:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// ceilRotation
			// 
			this.ceilRotation.AllowDecimal = true;
			this.ceilRotation.AllowExpressions = false;
			this.ceilRotation.AllowNegative = true;
			this.ceilRotation.AllowRelative = true;
			this.ceilRotation.ButtonStep = 5;
			this.ceilRotation.ButtonStepBig = 15F;
			this.ceilRotation.ButtonStepFloat = 1F;
			this.ceilRotation.ButtonStepSmall = 0.1F;
			this.ceilRotation.ButtonStepsUseModifierKeys = true;
			this.ceilRotation.ButtonStepsWrapAround = true;
			this.ceilRotation.Location = new System.Drawing.Point(113, 167);
			this.ceilRotation.Name = "ceilRotation";
			this.ceilRotation.Size = new System.Drawing.Size(62, 24);
			this.ceilRotation.StepValues = null;
			this.ceilRotation.TabIndex = 15;
			this.ceilRotation.Tag = "rotationceiling";
			this.ceilRotation.WhenTextChanged += new System.EventHandler(this.ceilRotation_WhenTextChanged);
			// 
			// ceilLightAbsolute
			// 
			this.ceilLightAbsolute.AutoSize = true;
			this.ceilLightAbsolute.Location = new System.Drawing.Point(181, 142);
			this.ceilLightAbsolute.Name = "ceilLightAbsolute";
			this.ceilLightAbsolute.Size = new System.Drawing.Size(67, 17);
			this.ceilLightAbsolute.TabIndex = 11;
			this.ceilLightAbsolute.Tag = "lightceilingabsolute";
			this.ceilLightAbsolute.Text = "Absolute";
			this.ceilLightAbsolute.UseVisualStyleBackColor = true;
			this.ceilLightAbsolute.CheckedChanged += new System.EventHandler(this.ceilLightAbsolute_CheckedChanged);
			// 
			// labelLightFront
			// 
			this.labelLightFront.Location = new System.Drawing.Point(26, 142);
			this.labelLightFront.Name = "labelLightFront";
			this.labelLightFront.Size = new System.Drawing.Size(80, 14);
			this.labelLightFront.TabIndex = 9;
			this.labelLightFront.Tag = "lightceiling";
			this.labelLightFront.Text = "Brightness:";
			this.labelLightFront.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// ceilBrightness
			// 
			this.ceilBrightness.AllowDecimal = false;
			this.ceilBrightness.AllowExpressions = false;
			this.ceilBrightness.AllowNegative = true;
			this.ceilBrightness.AllowRelative = true;
			this.ceilBrightness.ButtonStep = 16;
			this.ceilBrightness.ButtonStepBig = 32F;
			this.ceilBrightness.ButtonStepFloat = 1F;
			this.ceilBrightness.ButtonStepSmall = 1F;
			this.ceilBrightness.ButtonStepsUseModifierKeys = true;
			this.ceilBrightness.ButtonStepsWrapAround = false;
			this.ceilBrightness.Location = new System.Drawing.Point(113, 137);
			this.ceilBrightness.Name = "ceilBrightness";
			this.ceilBrightness.Size = new System.Drawing.Size(62, 24);
			this.ceilBrightness.StepValues = null;
			this.ceilBrightness.TabIndex = 10;
			this.ceilBrightness.Tag = "lightceiling";
			this.ceilBrightness.WhenTextChanged += new System.EventHandler(this.ceilBrightness_WhenTextChanged);
			// 
			// ceilRenderStyle
			// 
			this.ceilRenderStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ceilRenderStyle.FormattingEnabled = true;
			this.ceilRenderStyle.Location = new System.Drawing.Point(113, 85);
			this.ceilRenderStyle.Name = "ceilRenderStyle";
			this.ceilRenderStyle.Size = new System.Drawing.Size(130, 21);
			this.ceilRenderStyle.TabIndex = 5;
			this.ceilRenderStyle.Tag = "renderstyleceiling";
			// 
			// ceilScale
			// 
			this.ceilScale.AllowDecimal = true;
			this.ceilScale.AllowValueLinking = true;
			this.ceilScale.ButtonStep = 1;
			this.ceilScale.ButtonStepBig = 1F;
			this.ceilScale.ButtonStepFloat = 0.1F;
			this.ceilScale.ButtonStepSmall = 0.01F;
			this.ceilScale.ButtonStepsUseModifierKeys = true;
			this.ceilScale.DefaultValue = 1F;
			this.ceilScale.Field1 = "xscaleceiling";
			this.ceilScale.Field2 = "yscaleceiling";
			this.ceilScale.LinkValues = false;
			this.ceilScale.Location = new System.Drawing.Point(110, 53);
			this.ceilScale.Name = "ceilScale";
			this.ceilScale.Size = new System.Drawing.Size(186, 26);
			this.ceilScale.TabIndex = 3;
			this.ceilScale.Tag = "xscaleceiling";
			this.ceilScale.OnValuesChanged += new System.EventHandler(this.ceilScale_OnValuesChanged);
			// 
			// ceilOffsets
			// 
			this.ceilOffsets.AllowDecimal = true;
			this.ceilOffsets.AllowValueLinking = false;
			this.ceilOffsets.ButtonStep = 1;
			this.ceilOffsets.ButtonStepBig = 32F;
			this.ceilOffsets.ButtonStepFloat = 16F;
			this.ceilOffsets.ButtonStepSmall = 1F;
			this.ceilOffsets.ButtonStepsUseModifierKeys = true;
			this.ceilOffsets.DefaultValue = 0F;
			this.ceilOffsets.Field1 = "xpanningceiling";
			this.ceilOffsets.Field2 = "ypanningceiling";
			this.ceilOffsets.LinkValues = false;
			this.ceilOffsets.Location = new System.Drawing.Point(110, 21);
			this.ceilOffsets.Name = "ceilOffsets";
			this.ceilOffsets.Size = new System.Drawing.Size(186, 26);
			this.ceilOffsets.TabIndex = 1;
			this.ceilOffsets.Tag = "xpanningceiling";
			this.ceilOffsets.OnValuesChanged += new System.EventHandler(this.ceilOffsets_OnValuesChanged);
			// 
			// ceilingtex
			// 
			this.ceilingtex.Location = new System.Drawing.Point(356, 21);
			this.ceilingtex.MultipleTextures = false;
			this.ceilingtex.Name = "ceilingtex";
			this.ceilingtex.Size = new System.Drawing.Size(176, 200);
			this.ceilingtex.TabIndex = 25;
			this.ceilingtex.TextureName = "";
			this.ceilingtex.OnValueChanged += new System.EventHandler(this.ceilingtex_OnValueChanged);
			// 
			// tabslopes
			// 
			this.tabslopes.Controls.Add(this.groupBox7);
			this.tabslopes.Controls.Add(this.groupBox6);
			this.tabslopes.Controls.Add(this.gbFloorSlope);
			this.tabslopes.Controls.Add(this.gbCeilingSLope);
			this.tabslopes.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tabslopes.Location = new System.Drawing.Point(4, 22);
			this.tabslopes.Name = "tabslopes";
			this.tabslopes.Size = new System.Drawing.Size(570, 526);
			this.tabslopes.TabIndex = 3;
			this.tabslopes.Text = "Slopes / Portals";
			this.tabslopes.UseVisualStyleBackColor = true;
			// 
			// groupBox7
			// 
			this.groupBox7.Controls.Add(this.resetalphafloor);
			this.groupBox7.Controls.Add(this.floorportalflags);
			this.groupBox7.Controls.Add(this.label22);
			this.groupBox7.Controls.Add(this.alphafloor);
			this.groupBox7.Controls.Add(this.floorportalrenderstylelabel);
			this.groupBox7.Controls.Add(this.floorportalrenderstyle);
			this.groupBox7.Location = new System.Drawing.Point(307, 261);
			this.groupBox7.Name = "groupBox7";
			this.groupBox7.Size = new System.Drawing.Size(260, 266);
			this.groupBox7.TabIndex = 60;
			this.groupBox7.TabStop = false;
			this.groupBox7.Text = " Floor portal ";
			// 
			// resetalphafloor
			// 
			this.resetalphafloor.Image = ((System.Drawing.Image)(resources.GetObject("resetalphafloor.Image")));
			this.resetalphafloor.Location = new System.Drawing.Point(149, 49);
			this.resetalphafloor.Name = "resetalphafloor";
			this.resetalphafloor.Size = new System.Drawing.Size(23, 23);
			this.resetalphafloor.TabIndex = 70;
			this.resetalphafloor.Tag = "alphafloor";
			this.tooltip.SetToolTip(this.resetalphafloor, "Reset");
			this.resetalphafloor.UseVisualStyleBackColor = true;
			this.resetalphafloor.Click += new System.EventHandler(this.resetalphafloor_Click);
			// 
			// floorportalflags
			// 
			this.floorportalflags.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.floorportalflags.AutoScroll = true;
			this.floorportalflags.Columns = 2;
			this.floorportalflags.Location = new System.Drawing.Point(9, 79);
			this.floorportalflags.Name = "floorportalflags";
			this.floorportalflags.Size = new System.Drawing.Size(245, 181);
			this.floorportalflags.TabIndex = 59;
			this.floorportalflags.VerticalSpacing = 1;
			// 
			// label22
			// 
			this.label22.Location = new System.Drawing.Point(6, 54);
			this.label22.Name = "label22";
			this.label22.Size = new System.Drawing.Size(72, 14);
			this.label22.TabIndex = 58;
			this.label22.Tag = "";
			this.label22.Text = "Alpha:";
			this.label22.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// alphafloor
			// 
			this.alphafloor.AllowDecimal = true;
			this.alphafloor.AllowExpressions = false;
			this.alphafloor.AllowNegative = false;
			this.alphafloor.AllowRelative = false;
			this.alphafloor.ButtonStep = 1;
			this.alphafloor.ButtonStepBig = 0.25F;
			this.alphafloor.ButtonStepFloat = 0.1F;
			this.alphafloor.ButtonStepSmall = 0.01F;
			this.alphafloor.ButtonStepsUseModifierKeys = true;
			this.alphafloor.ButtonStepsWrapAround = false;
			this.alphafloor.Location = new System.Drawing.Point(84, 49);
			this.alphafloor.Name = "alphafloor";
			this.alphafloor.Size = new System.Drawing.Size(62, 24);
			this.alphafloor.StepValues = null;
			this.alphafloor.TabIndex = 57;
			this.alphafloor.Tag = "alphafloor";
			this.alphafloor.WhenTextChanged += new System.EventHandler(this.alphafloor_WhenTextChanged);
			// 
			// floorportalrenderstylelabel
			// 
			this.floorportalrenderstylelabel.Location = new System.Drawing.Point(6, 26);
			this.floorportalrenderstylelabel.Name = "floorportalrenderstylelabel";
			this.floorportalrenderstylelabel.Size = new System.Drawing.Size(72, 14);
			this.floorportalrenderstylelabel.TabIndex = 56;
			this.floorportalrenderstylelabel.Tag = "";
			this.floorportalrenderstylelabel.Text = "Render style:";
			this.floorportalrenderstylelabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// floorportalrenderstyle
			// 
			this.floorportalrenderstyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.floorportalrenderstyle.FormattingEnabled = true;
			this.floorportalrenderstyle.Location = new System.Drawing.Point(84, 22);
			this.floorportalrenderstyle.Name = "floorportalrenderstyle";
			this.floorportalrenderstyle.Size = new System.Drawing.Size(103, 21);
			this.floorportalrenderstyle.TabIndex = 55;
			this.floorportalrenderstyle.Tag = "portal_floor_overlaytype";
			// 
			// groupBox6
			// 
			this.groupBox6.Controls.Add(this.resetalphaceiling);
			this.groupBox6.Controls.Add(this.ceilportalflags);
			this.groupBox6.Controls.Add(this.label21);
			this.groupBox6.Controls.Add(this.alphaceiling);
			this.groupBox6.Controls.Add(this.ceilportalrenderstylelabel);
			this.groupBox6.Controls.Add(this.ceilportalrenderstyle);
			this.groupBox6.Location = new System.Drawing.Point(307, 3);
			this.groupBox6.Name = "groupBox6";
			this.groupBox6.Size = new System.Drawing.Size(260, 252);
			this.groupBox6.TabIndex = 2;
			this.groupBox6.TabStop = false;
			this.groupBox6.Text = " Ceiling portal ";
			// 
			// resetalphaceiling
			// 
			this.resetalphaceiling.Image = ((System.Drawing.Image)(resources.GetObject("resetalphaceiling.Image")));
			this.resetalphaceiling.Location = new System.Drawing.Point(149, 49);
			this.resetalphaceiling.Name = "resetalphaceiling";
			this.resetalphaceiling.Size = new System.Drawing.Size(23, 23);
			this.resetalphaceiling.TabIndex = 68;
			this.resetalphaceiling.Tag = "alphaceiling";
			this.tooltip.SetToolTip(this.resetalphaceiling, "Reset");
			this.resetalphaceiling.UseVisualStyleBackColor = true;
			this.resetalphaceiling.Click += new System.EventHandler(this.resetalphaceiling_Click);
			// 
			// ceilportalflags
			// 
			this.ceilportalflags.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.ceilportalflags.AutoScroll = true;
			this.ceilportalflags.Columns = 2;
			this.ceilportalflags.Location = new System.Drawing.Point(9, 79);
			this.ceilportalflags.Name = "ceilportalflags";
			this.ceilportalflags.Size = new System.Drawing.Size(245, 167);
			this.ceilportalflags.TabIndex = 59;
			this.ceilportalflags.VerticalSpacing = 1;
			// 
			// label21
			// 
			this.label21.Location = new System.Drawing.Point(6, 54);
			this.label21.Name = "label21";
			this.label21.Size = new System.Drawing.Size(72, 14);
			this.label21.TabIndex = 58;
			this.label21.Tag = "";
			this.label21.Text = "Alpha:";
			this.label21.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// alphaceiling
			// 
			this.alphaceiling.AllowDecimal = true;
			this.alphaceiling.AllowExpressions = false;
			this.alphaceiling.AllowNegative = false;
			this.alphaceiling.AllowRelative = false;
			this.alphaceiling.ButtonStep = 1;
			this.alphaceiling.ButtonStepBig = 0.25F;
			this.alphaceiling.ButtonStepFloat = 0.1F;
			this.alphaceiling.ButtonStepSmall = 0.01F;
			this.alphaceiling.ButtonStepsUseModifierKeys = true;
			this.alphaceiling.ButtonStepsWrapAround = false;
			this.alphaceiling.Location = new System.Drawing.Point(84, 49);
			this.alphaceiling.Name = "alphaceiling";
			this.alphaceiling.Size = new System.Drawing.Size(62, 24);
			this.alphaceiling.StepValues = null;
			this.alphaceiling.TabIndex = 57;
			this.alphaceiling.Tag = "alphaceiling";
			this.alphaceiling.WhenTextChanged += new System.EventHandler(this.alphaceiling_WhenTextChanged);
			// 
			// ceilportalrenderstylelabel
			// 
			this.ceilportalrenderstylelabel.Location = new System.Drawing.Point(6, 26);
			this.ceilportalrenderstylelabel.Name = "ceilportalrenderstylelabel";
			this.ceilportalrenderstylelabel.Size = new System.Drawing.Size(72, 14);
			this.ceilportalrenderstylelabel.TabIndex = 56;
			this.ceilportalrenderstylelabel.Tag = "";
			this.ceilportalrenderstylelabel.Text = "Render style:";
			this.ceilportalrenderstylelabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// ceilportalrenderstyle
			// 
			this.ceilportalrenderstyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ceilportalrenderstyle.FormattingEnabled = true;
			this.ceilportalrenderstyle.Location = new System.Drawing.Point(84, 22);
			this.ceilportalrenderstyle.Name = "ceilportalrenderstyle";
			this.ceilportalrenderstyle.Size = new System.Drawing.Size(103, 21);
			this.ceilportalrenderstyle.TabIndex = 55;
			this.ceilportalrenderstyle.Tag = "portal_ceil_overlaytype";
			// 
			// gbFloorSlope
			// 
			this.gbFloorSlope.Controls.Add(this.floorslopecontrol);
			this.gbFloorSlope.Location = new System.Drawing.Point(3, 261);
			this.gbFloorSlope.Name = "gbFloorSlope";
			this.gbFloorSlope.Size = new System.Drawing.Size(298, 266);
			this.gbFloorSlope.TabIndex = 1;
			this.gbFloorSlope.TabStop = false;
			this.gbFloorSlope.Text = " Floor slope ";
			// 
			// floorslopecontrol
			// 
			this.floorslopecontrol.Location = new System.Drawing.Point(4, 19);
			this.floorslopecontrol.Name = "floorslopecontrol";
			this.floorslopecontrol.Size = new System.Drawing.Size(290, 178);
			this.floorslopecontrol.TabIndex = 0;
			this.floorslopecontrol.UseLineAngles = false;
			this.floorslopecontrol.OnAnglesChanged += new System.EventHandler(this.floorslopecontrol_OnAnglesChanged);
			this.floorslopecontrol.OnUseLineAnglesChanged += new System.EventHandler(this.floorslopecontrol_OnUseLineAnglesChanged);
			this.floorslopecontrol.OnPivotModeChanged += new System.EventHandler(this.floorslopecontrol_OnPivotModeChanged);
			this.floorslopecontrol.OnResetClicked += new System.EventHandler(this.floorslopecontrol_OnResetClicked);
			// 
			// gbCeilingSLope
			// 
			this.gbCeilingSLope.Controls.Add(this.ceilingslopecontrol);
			this.gbCeilingSLope.Location = new System.Drawing.Point(3, 3);
			this.gbCeilingSLope.Name = "gbCeilingSLope";
			this.gbCeilingSLope.Size = new System.Drawing.Size(298, 252);
			this.gbCeilingSLope.TabIndex = 0;
			this.gbCeilingSLope.TabStop = false;
			this.gbCeilingSLope.Text = " Ceiling slope ";
			// 
			// ceilingslopecontrol
			// 
			this.ceilingslopecontrol.Location = new System.Drawing.Point(4, 19);
			this.ceilingslopecontrol.Name = "ceilingslopecontrol";
			this.ceilingslopecontrol.Size = new System.Drawing.Size(290, 178);
			this.ceilingslopecontrol.TabIndex = 1;
			this.ceilingslopecontrol.UseLineAngles = false;
			this.ceilingslopecontrol.OnAnglesChanged += new System.EventHandler(this.ceilingslopecontrol_OnAnglesChanged);
			this.ceilingslopecontrol.OnUseLineAnglesChanged += new System.EventHandler(this.ceilingslopecontrol_OnUseLineAnglesChanged);
			this.ceilingslopecontrol.OnPivotModeChanged += new System.EventHandler(this.ceilingslopecontrol_OnPivotModeChanged);
			this.ceilingslopecontrol.OnResetClicked += new System.EventHandler(this.ceilingslopecontrol_OnResetClicked);
			// 
			// tabcomment
			// 
			this.tabcomment.Controls.Add(this.commenteditor);
			this.tabcomment.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.tabcomment.Location = new System.Drawing.Point(4, 22);
			this.tabcomment.Name = "tabcomment";
			this.tabcomment.Size = new System.Drawing.Size(570, 526);
			this.tabcomment.TabIndex = 4;
			this.tabcomment.Text = "Comment";
			this.tabcomment.UseVisualStyleBackColor = true;
			// 
			// commenteditor
			// 
			this.commenteditor.Dock = System.Windows.Forms.DockStyle.Fill;
			this.commenteditor.Location = new System.Drawing.Point(0, 0);
			this.commenteditor.Name = "commenteditor";
			this.commenteditor.Size = new System.Drawing.Size(570, 526);
			this.commenteditor.TabIndex = 0;
			// 
			// tabcustom
			// 
			this.tabcustom.Controls.Add(this.fieldslist);
			this.tabcustom.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tabcustom.Location = new System.Drawing.Point(4, 22);
			this.tabcustom.Name = "tabcustom";
			this.tabcustom.Padding = new System.Windows.Forms.Padding(3);
			this.tabcustom.Size = new System.Drawing.Size(570, 526);
			this.tabcustom.TabIndex = 1;
			this.tabcustom.Text = "Custom";
			this.tabcustom.UseVisualStyleBackColor = true;
			this.tabcustom.MouseEnter += new System.EventHandler(this.tabcustom_MouseEnter);
			// 
			// fieldslist
			// 
			this.fieldslist.AllowInsert = true;
			this.fieldslist.AutoInsertUserPrefix = true;
			this.fieldslist.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.fieldslist.Dock = System.Windows.Forms.DockStyle.Fill;
			this.fieldslist.Location = new System.Drawing.Point(3, 3);
			this.fieldslist.Margin = new System.Windows.Forms.Padding(8);
			this.fieldslist.Name = "fieldslist";
			this.fieldslist.PropertyColumnVisible = true;
			this.fieldslist.PropertyColumnWidth = 150;
			this.fieldslist.ShowFixedFields = true;
			this.fieldslist.Size = new System.Drawing.Size(564, 520);
			this.fieldslist.TabIndex = 1;
			this.fieldslist.TypeColumnVisible = true;
			this.fieldslist.TypeColumnWidth = 100;
			this.fieldslist.ValueColumnVisible = true;
			// 
			// cancel
			// 
			this.cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancel.Location = new System.Drawing.Point(474, 566);
			this.cancel.Name = "cancel";
			this.cancel.Size = new System.Drawing.Size(112, 25);
			this.cancel.TabIndex = 4;
			this.cancel.Text = "Cancel";
			this.cancel.UseVisualStyleBackColor = true;
			this.cancel.Click += new System.EventHandler(this.cancel_Click);
			// 
			// apply
			// 
			this.apply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.apply.Location = new System.Drawing.Point(356, 566);
			this.apply.Name = "apply";
			this.apply.Size = new System.Drawing.Size(112, 25);
			this.apply.TabIndex = 3;
			this.apply.Text = "OK";
			this.apply.UseVisualStyleBackColor = true;
			this.apply.Click += new System.EventHandler(this.apply_Click);
			// 
			// tooltip
			// 
			this.tooltip.AutomaticDelay = 10;
			this.tooltip.AutoPopDelay = 10000;
			this.tooltip.InitialDelay = 10;
			this.tooltip.ReshowDelay = 100;
			// 
			// SectorEditFormUDMF
			// 
			this.AcceptButton = this.apply;
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.CancelButton = this.cancel;
			this.ClientSize = new System.Drawing.Size(598, 592);
			this.Controls.Add(this.cancel);
			this.Controls.Add(this.apply);
			this.Controls.Add(this.tabs);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SectorEditFormUDMF";
			this.Opacity = 0D;
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Edit Sector";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SectorEditFormUDMF_FormClosing);
			this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.SectorEditFormUDMF_HelpRequested);
			groupaction.ResumeLayout(false);
			groupeffect.ResumeLayout(false);
			groupeffect.PerformLayout();
			groupfloorceiling.ResumeLayout(false);
			groupfloorceiling.PerformLayout();
			this.tabs.ResumeLayout(false);
			this.tabproperties.ResumeLayout(false);
			this.groupdamage.ResumeLayout(false);
			this.groupBox3.ResumeLayout(false);
			this.tabColors.ResumeLayout(false);
			this.groupBox11.ResumeLayout(false);
			this.groupBox11.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.floorglowheightrequired)).EndInit();
			this.groupBox10.ResumeLayout(false);
			this.groupBox10.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.ceilingglowheightrequired)).EndInit();
			this.groupBox9.ResumeLayout(false);
			this.groupBox8.ResumeLayout(false);
			this.tabSurfaces.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.tabslopes.ResumeLayout(false);
			this.groupBox7.ResumeLayout(false);
			this.groupBox6.ResumeLayout(false);
			this.gbFloorSlope.ResumeLayout(false);
			this.gbCeilingSLope.ResumeLayout(false);
			this.tabcomment.ResumeLayout(false);
			this.tabcustom.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TabControl tabs;
		private System.Windows.Forms.TabPage tabproperties;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox brightness;
		private System.Windows.Forms.Button browseeffect;
		private CodeImp.DoomBuilder.Controls.ActionSelectorControl effect;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox ceilingheight;
		private System.Windows.Forms.Label sectorheightlabel;
		private System.Windows.Forms.Label sectorheight;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox floorheight;
		private System.Windows.Forms.TabPage tabcustom;
		private CodeImp.DoomBuilder.Controls.FieldsEditorControl fieldslist;
		private System.Windows.Forms.Button cancel;
		private System.Windows.Forms.Button apply;
		private System.Windows.Forms.TabPage tabSurfaces;
		private System.Windows.Forms.GroupBox groupBox1;
		private CodeImp.DoomBuilder.Controls.PairedFieldsControl ceilOffsets;
		private CodeImp.DoomBuilder.Controls.FlatSelectorControl ceilingtex;
		private CodeImp.DoomBuilder.Controls.PairedFieldsControl ceilScale;
		private System.Windows.Forms.ComboBox ceilRenderStyle;
		private System.Windows.Forms.Label label1;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox ceilRotation;
		private System.Windows.Forms.CheckBox ceilLightAbsolute;
		private System.Windows.Forms.Label labelLightFront;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox ceilBrightness;
		private System.Windows.Forms.Label labelceilrenderstyle;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Label labelfloorrenderstyle;
		private System.Windows.Forms.Label label11;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox floorRotation;
		private System.Windows.Forms.CheckBox floorLightAbsolute;
		private System.Windows.Forms.Label label12;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox floorBrightness;
		private System.Windows.Forms.ComboBox floorRenderStyle;
		private CodeImp.DoomBuilder.Controls.PairedFieldsControl floorScale;
		private CodeImp.DoomBuilder.Controls.PairedFieldsControl floorOffsets;
		private CodeImp.DoomBuilder.Controls.FlatSelectorControl floortex;
		private CodeImp.DoomBuilder.Controls.AngleControlEx floorAngleControl;
		private CodeImp.DoomBuilder.Controls.AngleControlEx ceilAngleControl;
		private System.Windows.Forms.GroupBox groupBox3;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox gravity;
		private CodeImp.DoomBuilder.Controls.CheckboxArrayControl flags;
		private System.Windows.Forms.CheckBox cbUseFloorLineAngles;
		private System.Windows.Forms.CheckBox cbUseCeilLineAngles;
		private System.Windows.Forms.TabPage tabslopes;
		private System.Windows.Forms.GroupBox gbFloorSlope;
		private System.Windows.Forms.GroupBox gbCeilingSLope;
		private CodeImp.DoomBuilder.Controls.SectorSlopeControl floorslopecontrol;
		private CodeImp.DoomBuilder.Controls.SectorSlopeControl ceilingslopecontrol;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox heightoffset;
		private System.Windows.Forms.ToolTip tooltip;
		private System.Windows.Forms.Label labelFloorOffsets;
		private System.Windows.Forms.Label labelFloorScale;
		private System.Windows.Forms.Label labelCeilOffsets;
		private System.Windows.Forms.Label labelCeilScale;
		private System.Windows.Forms.Button resetsoundsequence;
		private System.Windows.Forms.ComboBox soundsequence;
		private System.Windows.Forms.TabPage tabcomment;
		private CodeImp.DoomBuilder.Controls.CommentEditor commenteditor;
		private CodeImp.DoomBuilder.Controls.TagsSelector tagsselector;
		private System.Windows.Forms.Button resetfloorlight;
		private System.Windows.Forms.Button resetceillight;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ComboBox floorterrain;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.ComboBox ceilterrain;
		private System.Windows.Forms.GroupBox groupdamage;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox leakiness;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox damageinterval;
		private System.Windows.Forms.Button resetdamagetype;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox damageamount;
		private System.Windows.Forms.ComboBox damagetype;
		private System.Windows.Forms.Button resetfloorterrain;
		private System.Windows.Forms.Button resetceilterrain;
		private System.Windows.Forms.GroupBox groupBox6;
		private System.Windows.Forms.Label ceilportalrenderstylelabel;
		private System.Windows.Forms.ComboBox ceilportalrenderstyle;
		private System.Windows.Forms.Label label21;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox alphaceiling;
		private CodeImp.DoomBuilder.Controls.CheckboxArrayControl ceilportalflags;
		private System.Windows.Forms.GroupBox groupBox7;
		private CodeImp.DoomBuilder.Controls.CheckboxArrayControl floorportalflags;
		private System.Windows.Forms.Label label22;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox alphafloor;
		private System.Windows.Forms.Label floorportalrenderstylelabel;
		private System.Windows.Forms.ComboBox floorportalrenderstyle;
		private System.Windows.Forms.Label label20;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox ceiling_reflect;
		private System.Windows.Forms.Button reset_floor_reflect;
		private System.Windows.Forms.Label label23;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox floor_reflect;
		private System.Windows.Forms.Button reset_ceiling_reflect;
		private System.Windows.Forms.Button resetalphafloor;
		private System.Windows.Forms.Button resetalphaceiling;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox fogdensity;
        private System.Windows.Forms.TabPage tabColors;
        private Controls.ColorFieldsControl fadeColor;
        private Controls.ColorFieldsControl lightColor;
        private System.Windows.Forms.GroupBox groupBox8;
        private System.Windows.Forms.GroupBox groupBox9;
        private System.Windows.Forms.GroupBox groupBox11;
        private System.Windows.Forms.PictureBox floorglowheightrequired;
        private System.Windows.Forms.Button resetfloorglowheight;
        private System.Windows.Forms.Label floorglowheightlabel;
        private Controls.ButtonsNumericTextbox floorglowheight;
        private Controls.ColorFieldsControl floorglowcolor;
        private System.Windows.Forms.GroupBox groupBox10;
        private System.Windows.Forms.PictureBox ceilingglowheightrequired;
        private System.Windows.Forms.Button resetceilingglowheight;
        private System.Windows.Forms.Label ceilingglowheightlabel;
        private Controls.ButtonsNumericTextbox ceilingglowheight;
        private Controls.ColorFieldsControl ceilingglowcolor;
        private Controls.ColorFieldsControl thingsColor;
        private Controls.ColorFieldsControl lowerWallColor;
        private Controls.ColorFieldsControl upperWallColor;
        private Controls.ColorFieldsControl floorColor;
        private Controls.ColorFieldsControl ceilingColor;
        private System.Windows.Forms.CheckBox ceilingGlowEnabled;
        private System.Windows.Forms.CheckBox floorGlowEnabled;
        private Controls.ButtonsNumericTextbox desaturation;
    }
}