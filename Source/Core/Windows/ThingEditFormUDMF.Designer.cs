namespace CodeImp.DoomBuilder.Windows
{
	partial class ThingEditFormUDMF
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
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.thingtype = new CodeImp.DoomBuilder.Controls.ThingBrowserControl();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.cbrandomroll = new System.Windows.Forms.CheckBox();
			this.cbrandompitch = new System.Windows.Forms.CheckBox();
			this.cbrandomangle = new System.Windows.Forms.CheckBox();
			this.roll = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.labelroll = new System.Windows.Forms.Label();
			this.pitch = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.labelpitch = new System.Windows.Forms.Label();
			this.angle = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.labelAngle = new System.Windows.Forms.Label();
			this.anglecontrol = new CodeImp.DoomBuilder.Controls.AngleControlEx();
			this.labelGravity = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.tabs = new System.Windows.Forms.TabControl();
			this.tabproperties = new System.Windows.Forms.TabPage();
			this.settingsgroup = new System.Windows.Forms.GroupBox();
			this.missingflags = new System.Windows.Forms.PictureBox();
			this.flags = new CodeImp.DoomBuilder.Controls.CheckboxArrayControl();
			this.grouproll = new System.Windows.Forms.GroupBox();
			this.rollControl = new CodeImp.DoomBuilder.Controls.AngleControlEx();
			this.grouppitch = new System.Windows.Forms.GroupBox();
			this.pitchControl = new CodeImp.DoomBuilder.Controls.AngleControlEx();
			this.groupangle = new System.Windows.Forms.GroupBox();
			this.groupBox4 = new System.Windows.Forms.GroupBox();
			this.cbAbsoluteHeight = new System.Windows.Forms.CheckBox();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.posX = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.posY = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.posZ = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.zlabel = new System.Windows.Forms.Label();
			this.tabeffects = new System.Windows.Forms.TabPage();
			this.groupbehaviour = new System.Windows.Forms.GroupBox();
			this.floatbobphase = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.label1 = new System.Windows.Forms.Label();
			this.conversationID = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.labelID = new System.Windows.Forms.Label();
			this.health = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.label10 = new System.Windows.Forms.Label();
			this.score = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.label9 = new System.Windows.Forms.Label();
			this.gravity = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.grouprendering = new System.Windows.Forms.GroupBox();
			this.resetalpha = new System.Windows.Forms.Button();
			this.labelScale = new System.Windows.Forms.Label();
			this.scale = new CodeImp.DoomBuilder.Controls.PairedFloatControl();
			this.color = new CodeImp.DoomBuilder.Controls.ColorFieldsControl();
			this.alpha = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.label8 = new System.Windows.Forms.Label();
			this.renderStyle = new System.Windows.Forms.ComboBox();
			this.labelrenderstyle = new System.Windows.Forms.Label();
			this.actiongroup = new System.Windows.Forms.GroupBox();
			this.argscontrol = new CodeImp.DoomBuilder.Controls.ArgumentsControl();
			this.actionhelp = new CodeImp.DoomBuilder.Controls.ActionSpecialHelpButton();
			this.action = new CodeImp.DoomBuilder.Controls.ActionSelectorControl();
			this.browseaction = new System.Windows.Forms.Button();
			this.grouptag = new System.Windows.Forms.GroupBox();
			this.tagSelector = new CodeImp.DoomBuilder.Controls.TagSelector();
			this.tabcomment = new System.Windows.Forms.TabPage();
			this.commenteditor = new CodeImp.DoomBuilder.Controls.CommentEditor();
			this.tabcustom = new System.Windows.Forms.TabPage();
			this.hidefixedfields = new System.Windows.Forms.CheckBox();
			this.fieldslist = new CodeImp.DoomBuilder.Controls.FieldsEditorControl();
			this.cancel = new System.Windows.Forms.Button();
			this.apply = new System.Windows.Forms.Button();
			this.hint = new System.Windows.Forms.PictureBox();
			this.hintlabel = new System.Windows.Forms.Label();
			this.tooltip = new System.Windows.Forms.ToolTip(this.components);
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.tabs.SuspendLayout();
			this.tabproperties.SuspendLayout();
			this.settingsgroup.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.missingflags)).BeginInit();
			this.grouproll.SuspendLayout();
			this.grouppitch.SuspendLayout();
			this.groupangle.SuspendLayout();
			this.groupBox4.SuspendLayout();
			this.tabeffects.SuspendLayout();
			this.groupbehaviour.SuspendLayout();
			this.grouprendering.SuspendLayout();
			this.actiongroup.SuspendLayout();
			this.grouptag.SuspendLayout();
			this.tabcomment.SuspendLayout();
			this.tabcustom.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.hint)).BeginInit();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.thingtype);
			this.groupBox1.Location = new System.Drawing.Point(6, 6);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(230, 390);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = " Thing ";
			// 
			// thingtype
			// 
			this.thingtype.Location = new System.Drawing.Point(9, 13);
			this.thingtype.Margin = new System.Windows.Forms.Padding(6);
			this.thingtype.Name = "thingtype";
			this.thingtype.Size = new System.Drawing.Size(212, 374);
			this.thingtype.TabIndex = 0;
			this.thingtype.UseMultiSelection = true;
			this.thingtype.OnTypeChanged += new CodeImp.DoomBuilder.Controls.ThingBrowserControl.TypeChangedDeletegate(this.thingtype_OnTypeChanged);
			this.thingtype.OnTypeDoubleClicked += new CodeImp.DoomBuilder.Controls.ThingBrowserControl.TypeDoubleClickDeletegate(this.thingtype_OnTypeDoubleClicked);
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.cbrandomroll);
			this.groupBox2.Controls.Add(this.cbrandompitch);
			this.groupBox2.Controls.Add(this.cbrandomangle);
			this.groupBox2.Controls.Add(this.roll);
			this.groupBox2.Controls.Add(this.labelroll);
			this.groupBox2.Controls.Add(this.pitch);
			this.groupBox2.Controls.Add(this.labelpitch);
			this.groupBox2.Controls.Add(this.angle);
			this.groupBox2.Controls.Add(this.labelAngle);
			this.groupBox2.Location = new System.Drawing.Point(428, 298);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(193, 98);
			this.groupBox2.TabIndex = 6;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = " Rotation ";
			// 
			// cbrandomroll
			// 
			this.cbrandomroll.AutoSize = true;
			this.cbrandomroll.Location = new System.Drawing.Point(120, 71);
			this.cbrandomroll.Name = "cbrandomroll";
			this.cbrandomroll.Size = new System.Drawing.Size(66, 17);
			this.cbrandomroll.TabIndex = 5;
			this.cbrandomroll.Tag = "roll";
			this.cbrandomroll.Text = "Random";
			this.cbrandomroll.UseVisualStyleBackColor = true;
			this.cbrandomroll.CheckedChanged += new System.EventHandler(this.cbrandomroll_CheckedChanged);
			// 
			// cbrandompitch
			// 
			this.cbrandompitch.AutoSize = true;
			this.cbrandompitch.Location = new System.Drawing.Point(120, 46);
			this.cbrandompitch.Name = "cbrandompitch";
			this.cbrandompitch.Size = new System.Drawing.Size(66, 17);
			this.cbrandompitch.TabIndex = 3;
			this.cbrandompitch.Tag = "pitch";
			this.cbrandompitch.Text = "Random";
			this.cbrandompitch.UseVisualStyleBackColor = true;
			this.cbrandompitch.CheckedChanged += new System.EventHandler(this.cbrandompitch_CheckedChanged);
			// 
			// cbrandomangle
			// 
			this.cbrandomangle.AutoSize = true;
			this.cbrandomangle.Location = new System.Drawing.Point(120, 21);
			this.cbrandomangle.Name = "cbrandomangle";
			this.cbrandomangle.Size = new System.Drawing.Size(66, 17);
			this.cbrandomangle.TabIndex = 1;
			this.cbrandomangle.Text = "Random";
			this.cbrandomangle.UseVisualStyleBackColor = true;
			this.cbrandomangle.CheckedChanged += new System.EventHandler(this.cbrandomangle_CheckedChanged);
			// 
			// roll
			// 
			this.roll.AllowDecimal = false;
			this.roll.AllowExpressions = true;
			this.roll.AllowNegative = true;
			this.roll.AllowRelative = true;
			this.roll.ButtonStep = 5;
			this.roll.ButtonStepBig = 15F;
			this.roll.ButtonStepFloat = 1F;
			this.roll.ButtonStepSmall = 1F;
			this.roll.ButtonStepsUseModifierKeys = true;
			this.roll.ButtonStepsWrapAround = false;
			this.roll.Location = new System.Drawing.Point(55, 66);
			this.roll.Name = "roll";
			this.roll.Size = new System.Drawing.Size(60, 24);
			this.roll.StepValues = null;
			this.roll.TabIndex = 4;
			this.roll.Tag = "roll";
			this.roll.WhenTextChanged += new System.EventHandler(this.roll_WhenTextChanged);
			// 
			// labelroll
			// 
			this.labelroll.Location = new System.Drawing.Point(5, 71);
			this.labelroll.Name = "labelroll";
			this.labelroll.Size = new System.Drawing.Size(44, 14);
			this.labelroll.TabIndex = 23;
			this.labelroll.Text = "Roll:";
			this.labelroll.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// pitch
			// 
			this.pitch.AllowDecimal = false;
			this.pitch.AllowExpressions = true;
			this.pitch.AllowNegative = true;
			this.pitch.AllowRelative = true;
			this.pitch.ButtonStep = 5;
			this.pitch.ButtonStepBig = 15F;
			this.pitch.ButtonStepFloat = 1F;
			this.pitch.ButtonStepSmall = 1F;
			this.pitch.ButtonStepsUseModifierKeys = true;
			this.pitch.ButtonStepsWrapAround = false;
			this.pitch.Location = new System.Drawing.Point(55, 41);
			this.pitch.Name = "pitch";
			this.pitch.Size = new System.Drawing.Size(60, 24);
			this.pitch.StepValues = null;
			this.pitch.TabIndex = 2;
			this.pitch.Tag = "pitch";
			this.pitch.WhenTextChanged += new System.EventHandler(this.pitch_WhenTextChanged);
			// 
			// labelpitch
			// 
			this.labelpitch.Location = new System.Drawing.Point(5, 46);
			this.labelpitch.Name = "labelpitch";
			this.labelpitch.Size = new System.Drawing.Size(44, 14);
			this.labelpitch.TabIndex = 21;
			this.labelpitch.Text = "Pitch:";
			this.labelpitch.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// angle
			// 
			this.angle.AllowDecimal = false;
			this.angle.AllowExpressions = true;
			this.angle.AllowNegative = true;
			this.angle.AllowRelative = true;
			this.angle.ButtonStep = 5;
			this.angle.ButtonStepBig = 15F;
			this.angle.ButtonStepFloat = 1F;
			this.angle.ButtonStepSmall = 1F;
			this.angle.ButtonStepsUseModifierKeys = true;
			this.angle.ButtonStepsWrapAround = false;
			this.angle.Location = new System.Drawing.Point(55, 16);
			this.angle.Name = "angle";
			this.angle.Size = new System.Drawing.Size(60, 24);
			this.angle.StepValues = null;
			this.angle.TabIndex = 0;
			this.angle.WhenTextChanged += new System.EventHandler(this.angle_WhenTextChanged);
			// 
			// labelAngle
			// 
			this.labelAngle.Location = new System.Drawing.Point(5, 21);
			this.labelAngle.Name = "labelAngle";
			this.labelAngle.Size = new System.Drawing.Size(44, 14);
			this.labelAngle.TabIndex = 8;
			this.labelAngle.Text = "Angle:";
			this.labelAngle.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// anglecontrol
			// 
			this.anglecontrol.Angle = 0;
			this.anglecontrol.AngleOffset = 0;
			this.anglecontrol.DoomAngleClamping = false;
			this.anglecontrol.Location = new System.Drawing.Point(7, 17);
			this.anglecontrol.Name = "anglecontrol";
			this.anglecontrol.Size = new System.Drawing.Size(64, 64);
			this.anglecontrol.TabIndex = 20;
			this.anglecontrol.AngleChanged += new System.EventHandler(this.anglecontrol_AngleChanged);
			// 
			// labelGravity
			// 
			this.labelGravity.AutoSize = true;
			this.labelGravity.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelGravity.ForeColor = System.Drawing.SystemColors.HotTrack;
			this.labelGravity.Location = new System.Drawing.Point(19, 27);
			this.labelGravity.Name = "labelGravity";
			this.labelGravity.Size = new System.Drawing.Size(43, 13);
			this.labelGravity.TabIndex = 18;
			this.labelGravity.Tag = "gravity";
			this.labelGravity.Text = "Gravity:";
			this.tooltip.SetToolTip(this.labelGravity, "Positive values are multiplied with the class\'s property.\r\nNegative values are us" +
        "ed as their absolute.\r\nDefault is 1.0.");
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(15, 30);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(40, 13);
			this.label7.TabIndex = 9;
			this.label7.Text = "Action:";
			// 
			// tabs
			// 
			this.tabs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tabs.Controls.Add(this.tabproperties);
			this.tabs.Controls.Add(this.tabeffects);
			this.tabs.Controls.Add(this.tabcomment);
			this.tabs.Controls.Add(this.tabcustom);
			this.tabs.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tabs.Location = new System.Drawing.Point(10, 10);
			this.tabs.Margin = new System.Windows.Forms.Padding(1);
			this.tabs.Name = "tabs";
			this.tabs.Padding = new System.Drawing.Point(24, 3);
			this.tabs.SelectedIndex = 0;
			this.tabs.Size = new System.Drawing.Size(635, 425);
			this.tabs.TabIndex = 0;
			// 
			// tabproperties
			// 
			this.tabproperties.Controls.Add(this.settingsgroup);
			this.tabproperties.Controls.Add(this.grouproll);
			this.tabproperties.Controls.Add(this.grouppitch);
			this.tabproperties.Controls.Add(this.groupangle);
			this.tabproperties.Controls.Add(this.groupBox4);
			this.tabproperties.Controls.Add(this.groupBox2);
			this.tabproperties.Controls.Add(this.groupBox1);
			this.tabproperties.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tabproperties.Location = new System.Drawing.Point(4, 22);
			this.tabproperties.Name = "tabproperties";
			this.tabproperties.Padding = new System.Windows.Forms.Padding(3);
			this.tabproperties.Size = new System.Drawing.Size(627, 399);
			this.tabproperties.TabIndex = 0;
			this.tabproperties.Text = "Properties";
			this.tabproperties.UseVisualStyleBackColor = true;
			// 
			// settingsgroup
			// 
			this.settingsgroup.Controls.Add(this.missingflags);
			this.settingsgroup.Controls.Add(this.flags);
			this.settingsgroup.Location = new System.Drawing.Point(242, 6);
			this.settingsgroup.Name = "settingsgroup";
			this.settingsgroup.Size = new System.Drawing.Size(295, 286);
			this.settingsgroup.TabIndex = 1;
			this.settingsgroup.TabStop = false;
			this.settingsgroup.Text = " Flags ";
			// 
			// missingflags
			// 
			this.missingflags.BackColor = System.Drawing.SystemColors.Window;
			this.missingflags.Image = global::CodeImp.DoomBuilder.Properties.Resources.Warning;
			this.missingflags.Location = new System.Drawing.Point(42, -2);
			this.missingflags.Name = "missingflags";
			this.missingflags.Size = new System.Drawing.Size(16, 16);
			this.missingflags.TabIndex = 5;
			this.missingflags.TabStop = false;
			this.missingflags.Visible = false;
			// 
			// flags
			// 
			this.flags.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.flags.AutoScroll = true;
			this.flags.Columns = 2;
			this.flags.Location = new System.Drawing.Point(6, 19);
			this.flags.Name = "flags";
			this.flags.Size = new System.Drawing.Size(283, 260);
			this.flags.TabIndex = 0;
			this.flags.VerticalSpacing = 1;
			this.flags.OnValueChanged += new System.EventHandler(this.flags_OnValueChanged);
			// 
			// grouproll
			// 
			this.grouproll.Controls.Add(this.rollControl);
			this.grouproll.Location = new System.Drawing.Point(543, 16);
			this.grouproll.Name = "grouproll";
			this.grouproll.Size = new System.Drawing.Size(78, 88);
			this.grouproll.TabIndex = 2;
			this.grouproll.TabStop = false;
			this.grouproll.Text = " Roll ";
			// 
			// rollControl
			// 
			this.rollControl.Angle = -90;
			this.rollControl.AngleOffset = 0;
			this.rollControl.DoomAngleClamping = false;
			this.rollControl.Location = new System.Drawing.Point(7, 17);
			this.rollControl.Name = "rollControl";
			this.rollControl.Size = new System.Drawing.Size(64, 64);
			this.rollControl.TabIndex = 20;
			this.rollControl.Tag = "roll";
			this.rollControl.AngleChanged += new System.EventHandler(this.rollControl_AngleChanged);
			// 
			// grouppitch
			// 
			this.grouppitch.Controls.Add(this.pitchControl);
			this.grouppitch.Location = new System.Drawing.Point(543, 110);
			this.grouppitch.Name = "grouppitch";
			this.grouppitch.Size = new System.Drawing.Size(78, 88);
			this.grouppitch.TabIndex = 3;
			this.grouppitch.TabStop = false;
			this.grouppitch.Text = " Pitch ";
			// 
			// pitchControl
			// 
			this.pitchControl.Angle = -90;
			this.pitchControl.AngleOffset = 0;
			this.pitchControl.DoomAngleClamping = false;
			this.pitchControl.Location = new System.Drawing.Point(7, 17);
			this.pitchControl.Name = "pitchControl";
			this.pitchControl.Size = new System.Drawing.Size(64, 64);
			this.pitchControl.TabIndex = 20;
			this.pitchControl.Tag = "pitch";
			this.pitchControl.AngleChanged += new System.EventHandler(this.pitchControl_AngleChanged);
			// 
			// groupangle
			// 
			this.groupangle.Controls.Add(this.anglecontrol);
			this.groupangle.Location = new System.Drawing.Point(543, 204);
			this.groupangle.Name = "groupangle";
			this.groupangle.Size = new System.Drawing.Size(78, 88);
			this.groupangle.TabIndex = 4;
			this.groupangle.TabStop = false;
			this.groupangle.Text = " Angle";
			// 
			// groupBox4
			// 
			this.groupBox4.Controls.Add(this.cbAbsoluteHeight);
			this.groupBox4.Controls.Add(this.label4);
			this.groupBox4.Controls.Add(this.label5);
			this.groupBox4.Controls.Add(this.posX);
			this.groupBox4.Controls.Add(this.posY);
			this.groupBox4.Controls.Add(this.posZ);
			this.groupBox4.Controls.Add(this.zlabel);
			this.groupBox4.Location = new System.Drawing.Point(242, 298);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Size = new System.Drawing.Size(180, 98);
			this.groupBox4.TabIndex = 5;
			this.groupBox4.TabStop = false;
			this.groupBox4.Text = " Position";
			// 
			// cbAbsoluteHeight
			// 
			this.cbAbsoluteHeight.AutoSize = true;
			this.cbAbsoluteHeight.Location = new System.Drawing.Point(109, 71);
			this.cbAbsoluteHeight.Name = "cbAbsoluteHeight";
			this.cbAbsoluteHeight.Size = new System.Drawing.Size(67, 17);
			this.cbAbsoluteHeight.TabIndex = 3;
			this.cbAbsoluteHeight.Text = "Absolute";
			this.cbAbsoluteHeight.UseVisualStyleBackColor = true;
			this.cbAbsoluteHeight.CheckedChanged += new System.EventHandler(this.cbAbsoluteHeight_CheckedChanged);
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(4, 21);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(22, 14);
			this.label4.TabIndex = 15;
			this.label4.Text = "X:";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(4, 46);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(22, 14);
			this.label5.TabIndex = 14;
			this.label5.Text = "Y:";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// posX
			// 
			this.posX.AllowDecimal = true;
			this.posX.AllowExpressions = true;
			this.posX.AllowNegative = true;
			this.posX.AllowRelative = true;
			this.posX.ButtonStep = 8;
			this.posX.ButtonStepBig = 8F;
			this.posX.ButtonStepFloat = 1F;
			this.posX.ButtonStepSmall = 0.1F;
			this.posX.ButtonStepsUseModifierKeys = true;
			this.posX.ButtonStepsWrapAround = false;
			this.posX.Location = new System.Drawing.Point(32, 16);
			this.posX.Name = "posX";
			this.posX.Size = new System.Drawing.Size(72, 24);
			this.posX.StepValues = null;
			this.posX.TabIndex = 0;
			this.posX.WhenTextChanged += new System.EventHandler(this.posX_WhenTextChanged);
			// 
			// posY
			// 
			this.posY.AllowDecimal = true;
			this.posY.AllowExpressions = true;
			this.posY.AllowNegative = true;
			this.posY.AllowRelative = true;
			this.posY.ButtonStep = 8;
			this.posY.ButtonStepBig = 8F;
			this.posY.ButtonStepFloat = 1F;
			this.posY.ButtonStepSmall = 0.1F;
			this.posY.ButtonStepsUseModifierKeys = true;
			this.posY.ButtonStepsWrapAround = false;
			this.posY.Location = new System.Drawing.Point(32, 41);
			this.posY.Name = "posY";
			this.posY.Size = new System.Drawing.Size(72, 24);
			this.posY.StepValues = null;
			this.posY.TabIndex = 1;
			this.posY.WhenTextChanged += new System.EventHandler(this.posY_WhenTextChanged);
			// 
			// posZ
			// 
			this.posZ.AllowDecimal = true;
			this.posZ.AllowExpressions = true;
			this.posZ.AllowNegative = true;
			this.posZ.AllowRelative = true;
			this.posZ.ButtonStep = 8;
			this.posZ.ButtonStepBig = 8F;
			this.posZ.ButtonStepFloat = 1F;
			this.posZ.ButtonStepSmall = 0.1F;
			this.posZ.ButtonStepsUseModifierKeys = true;
			this.posZ.ButtonStepsWrapAround = false;
			this.posZ.Location = new System.Drawing.Point(32, 66);
			this.posZ.Name = "posZ";
			this.posZ.Size = new System.Drawing.Size(72, 24);
			this.posZ.StepValues = null;
			this.posZ.TabIndex = 2;
			this.posZ.WhenTextChanged += new System.EventHandler(this.posZ_WhenTextChanged);
			// 
			// zlabel
			// 
			this.zlabel.Location = new System.Drawing.Point(4, 71);
			this.zlabel.Name = "zlabel";
			this.zlabel.Size = new System.Drawing.Size(22, 14);
			this.zlabel.TabIndex = 9;
			this.zlabel.Text = "Z:";
			this.zlabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// tabeffects
			// 
			this.tabeffects.Controls.Add(this.groupbehaviour);
			this.tabeffects.Controls.Add(this.grouprendering);
			this.tabeffects.Controls.Add(this.actiongroup);
			this.tabeffects.Controls.Add(this.grouptag);
			this.tabeffects.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tabeffects.Location = new System.Drawing.Point(4, 22);
			this.tabeffects.Name = "tabeffects";
			this.tabeffects.Padding = new System.Windows.Forms.Padding(3);
			this.tabeffects.Size = new System.Drawing.Size(627, 399);
			this.tabeffects.TabIndex = 1;
			this.tabeffects.Text = "Action / Tag / Misc.";
			this.tabeffects.UseVisualStyleBackColor = true;
			// 
			// groupbehaviour
			// 
			this.groupbehaviour.Controls.Add(this.floatbobphase);
			this.groupbehaviour.Controls.Add(this.label1);
			this.groupbehaviour.Controls.Add(this.conversationID);
			this.groupbehaviour.Controls.Add(this.labelID);
			this.groupbehaviour.Controls.Add(this.health);
			this.groupbehaviour.Controls.Add(this.label10);
			this.groupbehaviour.Controls.Add(this.score);
			this.groupbehaviour.Controls.Add(this.label9);
			this.groupbehaviour.Controls.Add(this.gravity);
			this.groupbehaviour.Controls.Add(this.labelGravity);
			this.groupbehaviour.Location = new System.Drawing.Point(285, 6);
			this.groupbehaviour.Name = "groupbehaviour";
			this.groupbehaviour.Size = new System.Drawing.Size(333, 158);
			this.groupbehaviour.TabIndex = 23;
			this.groupbehaviour.TabStop = false;
			this.groupbehaviour.Text = " Behaviour ";
			// 
			// floatbobphase
			// 
			this.floatbobphase.AllowDecimal = false;
			this.floatbobphase.AllowExpressions = false;
			this.floatbobphase.AllowNegative = true;
			this.floatbobphase.AllowRelative = false;
			this.floatbobphase.ButtonStep = 1;
			this.floatbobphase.ButtonStepBig = 8F;
			this.floatbobphase.ButtonStepFloat = 1F;
			this.floatbobphase.ButtonStepSmall = 1F;
			this.floatbobphase.ButtonStepsUseModifierKeys = false;
			this.floatbobphase.ButtonStepsWrapAround = false;
			this.floatbobphase.Location = new System.Drawing.Point(251, 52);
			this.floatbobphase.Name = "floatbobphase";
			this.floatbobphase.Size = new System.Drawing.Size(72, 24);
			this.floatbobphase.StepValues = null;
			this.floatbobphase.TabIndex = 4;
			this.floatbobphase.Tag = "floatbobphase";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(159, 57);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(86, 13);
			this.label1.TabIndex = 27;
			this.label1.Tag = "floatbobphase";
			this.label1.Text = "Float bob phase:";
			// 
			// conversationID
			// 
			this.conversationID.AllowDecimal = false;
			this.conversationID.AllowExpressions = false;
			this.conversationID.AllowNegative = false;
			this.conversationID.AllowRelative = false;
			this.conversationID.ButtonStep = 1;
			this.conversationID.ButtonStepBig = 8F;
			this.conversationID.ButtonStepFloat = 1F;
			this.conversationID.ButtonStepSmall = 1F;
			this.conversationID.ButtonStepsUseModifierKeys = false;
			this.conversationID.ButtonStepsWrapAround = false;
			this.conversationID.Location = new System.Drawing.Point(251, 22);
			this.conversationID.Name = "conversationID";
			this.conversationID.Size = new System.Drawing.Size(72, 24);
			this.conversationID.StepValues = null;
			this.conversationID.TabIndex = 3;
			this.conversationID.Tag = "conversation";
			// 
			// labelID
			// 
			this.labelID.AutoSize = true;
			this.labelID.Location = new System.Drawing.Point(159, 27);
			this.labelID.Name = "labelID";
			this.labelID.Size = new System.Drawing.Size(86, 13);
			this.labelID.TabIndex = 25;
			this.labelID.Tag = "conversation";
			this.labelID.Text = "Conversation ID:";
			// 
			// health
			// 
			this.health.AllowDecimal = true;
			this.health.AllowExpressions = false;
			this.health.AllowNegative = true;
			this.health.AllowRelative = false;
			this.health.ButtonStep = 8;
			this.health.ButtonStepBig = 16F;
			this.health.ButtonStepFloat = 0.1F;
			this.health.ButtonStepSmall = 1F;
			this.health.ButtonStepsUseModifierKeys = true;
			this.health.ButtonStepsWrapAround = false;
			this.health.Location = new System.Drawing.Point(68, 82);
			this.health.Name = "health";
			this.health.Size = new System.Drawing.Size(72, 24);
			this.health.StepValues = null;
			this.health.TabIndex = 2;
			this.health.Tag = "health";
			// 
			// label10
			// 
			this.label10.AutoSize = true;
			this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label10.ForeColor = System.Drawing.SystemColors.HotTrack;
			this.label10.Location = new System.Drawing.Point(21, 87);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(41, 13);
			this.label10.TabIndex = 22;
			this.label10.Tag = "health";
			this.label10.Text = "Health:";
			this.tooltip.SetToolTip(this.label10, "Positive values are multiplied with the class\'s property.\r\nNegative values are us" +
        "ed as their absolute.\r\nDefault is 1.");
			// 
			// score
			// 
			this.score.AllowDecimal = false;
			this.score.AllowExpressions = false;
			this.score.AllowNegative = false;
			this.score.AllowRelative = false;
			this.score.ButtonStep = 8;
			this.score.ButtonStepBig = 16F;
			this.score.ButtonStepFloat = 0.1F;
			this.score.ButtonStepSmall = 1F;
			this.score.ButtonStepsUseModifierKeys = true;
			this.score.ButtonStepsWrapAround = false;
			this.score.Location = new System.Drawing.Point(68, 52);
			this.score.Name = "score";
			this.score.Size = new System.Drawing.Size(72, 24);
			this.score.StepValues = null;
			this.score.TabIndex = 1;
			this.score.Tag = "score";
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point(24, 57);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(38, 13);
			this.label9.TabIndex = 20;
			this.label9.Tag = "score";
			this.label9.Text = "Score:";
			// 
			// gravity
			// 
			this.gravity.AllowDecimal = true;
			this.gravity.AllowExpressions = false;
			this.gravity.AllowNegative = true;
			this.gravity.AllowRelative = false;
			this.gravity.ButtonStep = 8;
			this.gravity.ButtonStepBig = 0.25F;
			this.gravity.ButtonStepFloat = 0.1F;
			this.gravity.ButtonStepSmall = 0.01F;
			this.gravity.ButtonStepsUseModifierKeys = true;
			this.gravity.ButtonStepsWrapAround = false;
			this.gravity.Location = new System.Drawing.Point(68, 22);
			this.gravity.Name = "gravity";
			this.gravity.Size = new System.Drawing.Size(72, 24);
			this.gravity.StepValues = null;
			this.gravity.TabIndex = 0;
			this.gravity.Tag = "gravity";
			// 
			// grouprendering
			// 
			this.grouprendering.Controls.Add(this.resetalpha);
			this.grouprendering.Controls.Add(this.labelScale);
			this.grouprendering.Controls.Add(this.scale);
			this.grouprendering.Controls.Add(this.color);
			this.grouprendering.Controls.Add(this.alpha);
			this.grouprendering.Controls.Add(this.label8);
			this.grouprendering.Controls.Add(this.renderStyle);
			this.grouprendering.Controls.Add(this.labelrenderstyle);
			this.grouprendering.Location = new System.Drawing.Point(3, 6);
			this.grouprendering.Name = "grouprendering";
			this.grouprendering.Size = new System.Drawing.Size(276, 158);
			this.grouprendering.TabIndex = 22;
			this.grouprendering.TabStop = false;
			this.grouprendering.Text = " Rendering ";
			// 
			// resetalpha
			// 
			this.resetalpha.Image = global::CodeImp.DoomBuilder.Properties.Resources.Reset;
			this.resetalpha.Location = new System.Drawing.Point(166, 86);
			this.resetalpha.Name = "resetalpha";
			this.resetalpha.Size = new System.Drawing.Size(23, 23);
			this.resetalpha.TabIndex = 69;
			this.resetalpha.Tag = "alpha";
			this.tooltip.SetToolTip(this.resetalpha, "Reset");
			this.resetalpha.UseVisualStyleBackColor = true;
			this.resetalpha.Click += new System.EventHandler(this.resetalpha_Click);
			// 
			// labelScale
			// 
			this.labelScale.Location = new System.Drawing.Point(5, 27);
			this.labelScale.Name = "labelScale";
			this.labelScale.Size = new System.Drawing.Size(80, 14);
			this.labelScale.TabIndex = 32;
			this.labelScale.Tag = "scalex";
			this.labelScale.Text = "Scale:";
			this.labelScale.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// scale
			// 
			this.scale.ButtonStep = 0.1F;
			this.scale.ButtonStepBig = 0.25F;
			this.scale.ButtonStepSmall = 0.01F;
			this.scale.ButtonStepsUseModifierKeys = true;
			this.scale.DefaultValue = 1D;
			this.scale.LinkValues = false;
			this.scale.Location = new System.Drawing.Point(89, 22);
			this.scale.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
			this.scale.Name = "scale";
			this.scale.Size = new System.Drawing.Size(186, 26);
			this.scale.TabIndex = 0;
			this.scale.Tag = "scalex";
			this.scale.OnValuesChanged += new System.EventHandler(this.scale_OnValuesChanged);
			// 
			// color
			// 
			this.color.DefaultValue = 0;
			this.color.Field = "fillcolor";
			this.color.Label = "Color:";
			this.color.Location = new System.Drawing.Point(22, 115);
			this.color.Name = "color";
			this.color.Size = new System.Drawing.Size(207, 31);
			this.color.TabIndex = 3;
			this.color.Tag = "fillcolor";
			// 
			// alpha
			// 
			this.alpha.AllowDecimal = true;
			this.alpha.AllowExpressions = false;
			this.alpha.AllowNegative = true;
			this.alpha.AllowRelative = false;
			this.alpha.ButtonStep = 8;
			this.alpha.ButtonStepBig = 0.25F;
			this.alpha.ButtonStepFloat = 0.1F;
			this.alpha.ButtonStepSmall = 0.01F;
			this.alpha.ButtonStepsUseModifierKeys = true;
			this.alpha.ButtonStepsWrapAround = false;
			this.alpha.Location = new System.Drawing.Point(91, 85);
			this.alpha.Name = "alpha";
			this.alpha.Size = new System.Drawing.Size(72, 24);
			this.alpha.StepValues = null;
			this.alpha.TabIndex = 2;
			this.alpha.Tag = "alpha";
			this.alpha.WhenTextChanged += new System.EventHandler(this.alpha_WhenTextChanged);
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(48, 89);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(37, 13);
			this.label8.TabIndex = 25;
			this.label8.Tag = "alpha";
			this.label8.Text = "Alpha:";
			// 
			// renderStyle
			// 
			this.renderStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.renderStyle.FormattingEnabled = true;
			this.renderStyle.Location = new System.Drawing.Point(91, 57);
			this.renderStyle.Name = "renderStyle";
			this.renderStyle.Size = new System.Drawing.Size(156, 21);
			this.renderStyle.TabIndex = 1;
			this.renderStyle.Tag = "renderstyle";
			this.renderStyle.SelectedIndexChanged += new System.EventHandler(this.renderStyle_SelectedIndexChanged);
			// 
			// labelrenderstyle
			// 
			this.labelrenderstyle.AutoSize = true;
			this.labelrenderstyle.Location = new System.Drawing.Point(16, 60);
			this.labelrenderstyle.Name = "labelrenderstyle";
			this.labelrenderstyle.Size = new System.Drawing.Size(69, 13);
			this.labelrenderstyle.TabIndex = 23;
			this.labelrenderstyle.Tag = "renderstyle";
			this.labelrenderstyle.Text = "Render style:";
			// 
			// actiongroup
			// 
			this.actiongroup.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.actiongroup.Controls.Add(this.argscontrol);
			this.actiongroup.Controls.Add(this.actionhelp);
			this.actiongroup.Controls.Add(this.label7);
			this.actiongroup.Controls.Add(this.action);
			this.actiongroup.Controls.Add(this.browseaction);
			this.actiongroup.Location = new System.Drawing.Point(3, 170);
			this.actiongroup.Name = "actiongroup";
			this.actiongroup.Size = new System.Drawing.Size(615, 154);
			this.actiongroup.TabIndex = 22;
			this.actiongroup.TabStop = false;
			this.actiongroup.Text = " Action ";
			// 
			// argscontrol
			// 
			this.argscontrol.Location = new System.Drawing.Point(6, 56);
			this.argscontrol.Name = "argscontrol";
			this.argscontrol.Size = new System.Drawing.Size(603, 80);
			this.argscontrol.TabIndex = 15;
			// 
			// actionhelp
			// 
			this.actionhelp.Location = new System.Drawing.Point(581, 24);
			this.actionhelp.Name = "actionhelp";
			this.actionhelp.Size = new System.Drawing.Size(28, 26);
			this.actionhelp.TabIndex = 14;
			// 
			// action
			// 
			this.action.BackColor = System.Drawing.SystemColors.Control;
			this.action.Cursor = System.Windows.Forms.Cursors.Default;
			this.action.Empty = false;
			this.action.GeneralizedCategories = null;
			this.action.GeneralizedOptions = null;
			this.action.Location = new System.Drawing.Point(62, 27);
			this.action.Name = "action";
			this.action.Size = new System.Drawing.Size(485, 21);
			this.action.TabIndex = 0;
			this.action.Value = 402;
			this.action.ValueChanges += new System.EventHandler(this.action_ValueChanges);
			// 
			// browseaction
			// 
			this.browseaction.Image = global::CodeImp.DoomBuilder.Properties.Resources.List;
			this.browseaction.Location = new System.Drawing.Point(551, 24);
			this.browseaction.Name = "browseaction";
			this.browseaction.Size = new System.Drawing.Size(28, 26);
			this.browseaction.TabIndex = 1;
			this.browseaction.Text = " ";
			this.tooltip.SetToolTip(this.browseaction, "Browse Action");
			this.browseaction.UseVisualStyleBackColor = true;
			this.browseaction.Click += new System.EventHandler(this.browseaction_Click);
			// 
			// grouptag
			// 
			this.grouptag.Controls.Add(this.tagSelector);
			this.grouptag.Location = new System.Drawing.Point(3, 330);
			this.grouptag.Name = "grouptag";
			this.grouptag.Size = new System.Drawing.Size(615, 66);
			this.grouptag.TabIndex = 0;
			this.grouptag.TabStop = false;
			this.grouptag.Text = " Identification ";
			// 
			// tagSelector
			// 
			this.tagSelector.Location = new System.Drawing.Point(6, 21);
			this.tagSelector.Name = "tagSelector";
			this.tagSelector.Size = new System.Drawing.Size(603, 35);
			this.tagSelector.TabIndex = 8;
			// 
			// tabcomment
			// 
			this.tabcomment.Controls.Add(this.commenteditor);
			this.tabcomment.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tabcomment.Location = new System.Drawing.Point(4, 22);
			this.tabcomment.Name = "tabcomment";
			this.tabcomment.Size = new System.Drawing.Size(627, 399);
			this.tabcomment.TabIndex = 3;
			this.tabcomment.Text = "Comment";
			this.tabcomment.UseVisualStyleBackColor = true;
			// 
			// commenteditor
			// 
			this.commenteditor.Location = new System.Drawing.Point(3, 3);
			this.commenteditor.Name = "commenteditor";
			this.commenteditor.Size = new System.Drawing.Size(621, 396);
			this.commenteditor.TabIndex = 0;
			// 
			// tabcustom
			// 
			this.tabcustom.Controls.Add(this.hidefixedfields);
			this.tabcustom.Controls.Add(this.fieldslist);
			this.tabcustom.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tabcustom.Location = new System.Drawing.Point(4, 22);
			this.tabcustom.Name = "tabcustom";
			this.tabcustom.Size = new System.Drawing.Size(627, 399);
			this.tabcustom.TabIndex = 2;
			this.tabcustom.Text = "Custom";
			this.tabcustom.UseVisualStyleBackColor = true;
			this.tabcustom.MouseEnter += new System.EventHandler(this.tabcustom_MouseEnter);
			// 
			// hidefixedfields
			// 
			this.hidefixedfields.AutoSize = true;
			this.hidefixedfields.Location = new System.Drawing.Point(10, 381);
			this.hidefixedfields.Name = "hidefixedfields";
			this.hidefixedfields.Size = new System.Drawing.Size(195, 17);
			this.hidefixedfields.TabIndex = 2;
			this.hidefixedfields.Text = "Show user-added custom fields only";
			this.hidefixedfields.UseVisualStyleBackColor = true;
			this.hidefixedfields.CheckedChanged += new System.EventHandler(this.hidefixedfields_CheckedChanged);
			// 
			// fieldslist
			// 
			this.fieldslist.AllowInsert = true;
			this.fieldslist.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.fieldslist.AutoInsertUserPrefix = true;
			this.fieldslist.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.fieldslist.Location = new System.Drawing.Point(8, 9);
			this.fieldslist.Margin = new System.Windows.Forms.Padding(8, 9, 8, 9);
			this.fieldslist.Name = "fieldslist";
			this.fieldslist.PropertyColumnVisible = true;
			this.fieldslist.PropertyColumnWidth = 150;
			this.fieldslist.ShowFixedFields = true;
			this.fieldslist.Size = new System.Drawing.Size(611, 368);
			this.fieldslist.TabIndex = 1;
			this.fieldslist.TypeColumnVisible = true;
			this.fieldslist.TypeColumnWidth = 100;
			this.fieldslist.ValueColumnVisible = true;
			// 
			// cancel
			// 
			this.cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancel.Location = new System.Drawing.Point(533, 442);
			this.cancel.Name = "cancel";
			this.cancel.Size = new System.Drawing.Size(112, 25);
			this.cancel.TabIndex = 2;
			this.cancel.Text = "Cancel";
			this.cancel.UseVisualStyleBackColor = true;
			this.cancel.Click += new System.EventHandler(this.cancel_Click);
			// 
			// apply
			// 
			this.apply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.apply.Location = new System.Drawing.Point(415, 442);
			this.apply.Name = "apply";
			this.apply.Size = new System.Drawing.Size(112, 25);
			this.apply.TabIndex = 1;
			this.apply.Text = "OK";
			this.apply.UseVisualStyleBackColor = true;
			this.apply.Click += new System.EventHandler(this.apply_Click);
			// 
			// hint
			// 
			this.hint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.hint.Image = global::CodeImp.DoomBuilder.Properties.Resources.Lightbulb;
			this.hint.Location = new System.Drawing.Point(10, 446);
			this.hint.Name = "hint";
			this.hint.Size = new System.Drawing.Size(16, 16);
			this.hint.TabIndex = 3;
			this.hint.TabStop = false;
			// 
			// hintlabel
			// 
			this.hintlabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.hintlabel.AutoSize = true;
			this.hintlabel.Location = new System.Drawing.Point(24, 447);
			this.hintlabel.Name = "hintlabel";
			this.hintlabel.Size = new System.Drawing.Size(365, 13);
			this.hintlabel.TabIndex = 4;
			this.hintlabel.Text = "Select categories or several thing types to randomly assign them to selection";
			// 
			// tooltip
			// 
			this.tooltip.AutomaticDelay = 10;
			this.tooltip.AutoPopDelay = 10000;
			this.tooltip.InitialDelay = 10;
			this.tooltip.ReshowDelay = 100;
			// 
			// ThingEditFormUDMF
			// 
			this.AcceptButton = this.apply;
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.CancelButton = this.cancel;
			this.ClientSize = new System.Drawing.Size(655, 470);
			this.Controls.Add(this.hint);
			this.Controls.Add(this.cancel);
			this.Controls.Add(this.apply);
			this.Controls.Add(this.hintlabel);
			this.Controls.Add(this.tabs);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ThingEditFormUDMF";
			this.Opacity = 0D;
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Edit Thing";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ThingEditForm_FormClosing);
			this.Shown += new System.EventHandler(this.ThingEditFormUDMF_Shown);
			this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.ThingEditForm_HelpRequested);
			this.groupBox1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.tabs.ResumeLayout(false);
			this.tabproperties.ResumeLayout(false);
			this.settingsgroup.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.missingflags)).EndInit();
			this.grouproll.ResumeLayout(false);
			this.grouppitch.ResumeLayout(false);
			this.groupangle.ResumeLayout(false);
			this.groupBox4.ResumeLayout(false);
			this.groupBox4.PerformLayout();
			this.tabeffects.ResumeLayout(false);
			this.groupbehaviour.ResumeLayout(false);
			this.groupbehaviour.PerformLayout();
			this.grouprendering.ResumeLayout(false);
			this.grouprendering.PerformLayout();
			this.actiongroup.ResumeLayout(false);
			this.actiongroup.PerformLayout();
			this.grouptag.ResumeLayout(false);
			this.tabcomment.ResumeLayout(false);
			this.tabcustom.ResumeLayout(false);
			this.tabcustom.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.hint)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TabControl tabs;
		private System.Windows.Forms.TabPage tabproperties;
		private System.Windows.Forms.TabPage tabeffects;
		private System.Windows.Forms.Button cancel;
		private System.Windows.Forms.Button apply;
        private System.Windows.Forms.TabPage tabcustom;
		private System.Windows.Forms.GroupBox grouptag;
		private System.Windows.Forms.GroupBox actiongroup;
		private CodeImp.DoomBuilder.Controls.ActionSelectorControl action;
		private System.Windows.Forms.Button browseaction;
		private CodeImp.DoomBuilder.Controls.FieldsEditorControl fieldslist;
		private CodeImp.DoomBuilder.Controls.ThingBrowserControl thingtype;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox angle;
		private System.Windows.Forms.Label labelAngle;
		private CodeImp.DoomBuilder.Controls.TagSelector tagSelector;
		private System.Windows.Forms.Label labelGravity;
		private CodeImp.DoomBuilder.Controls.AngleControlEx anglecontrol;
		private System.Windows.Forms.PictureBox hint;
		private System.Windows.Forms.Label hintlabel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.CheckBox cbAbsoluteHeight;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private Controls.ButtonsNumericTextbox posX;
        private Controls.ButtonsNumericTextbox posY;
        private Controls.ButtonsNumericTextbox posZ;
        private System.Windows.Forms.Label zlabel;
        private Controls.ButtonsNumericTextbox gravity;
        private Controls.ButtonsNumericTextbox roll;
        private System.Windows.Forms.Label labelroll;
        private Controls.ButtonsNumericTextbox pitch;
        private System.Windows.Forms.Label labelpitch;
        private System.Windows.Forms.GroupBox grouprendering;
        private Controls.ButtonsNumericTextbox alpha;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox renderStyle;
        private System.Windows.Forms.Label labelrenderstyle;
        private System.Windows.Forms.GroupBox groupbehaviour;
        private Controls.ButtonsNumericTextbox health;
        private System.Windows.Forms.Label label10;
        private Controls.ButtonsNumericTextbox score;
        private System.Windows.Forms.Label label9;
		private CodeImp.DoomBuilder.Controls.ColorFieldsControl color;
        private System.Windows.Forms.GroupBox settingsgroup;
        private Controls.CheckboxArrayControl flags;
        private Controls.ButtonsNumericTextbox conversationID;
        private System.Windows.Forms.Label labelID;
        private System.Windows.Forms.GroupBox grouproll;
        private CodeImp.DoomBuilder.Controls.AngleControlEx rollControl;
        private System.Windows.Forms.GroupBox grouppitch;
        private CodeImp.DoomBuilder.Controls.AngleControlEx pitchControl;
        private System.Windows.Forms.GroupBox groupangle;
        private CodeImp.DoomBuilder.Controls.PairedFloatControl scale;
		private System.Windows.Forms.PictureBox missingflags;
		private System.Windows.Forms.ToolTip tooltip;
		private CodeImp.DoomBuilder.Controls.ActionSpecialHelpButton actionhelp;
		private System.Windows.Forms.Label labelScale;
		private System.Windows.Forms.CheckBox cbrandomangle;
		private System.Windows.Forms.CheckBox cbrandomroll;
		private System.Windows.Forms.CheckBox cbrandompitch;
		private System.Windows.Forms.TabPage tabcomment;
		private CodeImp.DoomBuilder.Controls.CommentEditor commenteditor;
		private CodeImp.DoomBuilder.Controls.ArgumentsControl argscontrol;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox floatbobphase;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.CheckBox hidefixedfields;
		private System.Windows.Forms.Button resetalpha;
	}
}