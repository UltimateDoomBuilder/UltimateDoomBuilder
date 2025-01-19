namespace CodeImp.DoomBuilder.BuilderEffects
{
	partial class JitterThingsForm
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			this.bApply = new System.Windows.Forms.Button();
			this.bCancel = new System.Windows.Forms.Button();
			this.bUpdateTranslation = new System.Windows.Forms.Button();
			this.positionJitterAmmount = new IntControl();
			this.bUpdateAngle = new System.Windows.Forms.Button();
			this.rotationJitterAmmount = new IntControl();
			this.heightJitterAmmount = new IntControl();
			this.bUpdateHeight = new System.Windows.Forms.Button();
			this.pitchAmmount = new IntControl();
			this.rollAmmount = new IntControl();
			this.bUpdatePitch = new System.Windows.Forms.Button();
			this.bUpdateRoll = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.cbNegativeRoll = new System.Windows.Forms.CheckBox();
			this.cbNegativePitch = new System.Windows.Forms.CheckBox();
			this.cbRelativeRoll = new System.Windows.Forms.CheckBox();
			this.cbRelativePitch = new System.Windows.Forms.CheckBox();
			this.scalegroup = new System.Windows.Forms.GroupBox();
			this.cbNegativeScaleY = new System.Windows.Forms.CheckBox();
			this.cbNegativeScaleX = new System.Windows.Forms.CheckBox();
			this.cbUniformScale = new System.Windows.Forms.CheckBox();
			this.cbRelativeScale = new System.Windows.Forms.CheckBox();
			this.bUpdateScaleY = new System.Windows.Forms.Button();
			this.maxScaleYLabel = new System.Windows.Forms.Label();
			this.maxScaleYLabel.AutoSize = true;
			this.maxScaleY = new System.Windows.Forms.NumericUpDown();
			this.minScaleYLabel = new System.Windows.Forms.Label();
			this.minScaleYLabel.AutoSize = true;
			this.minScaleY = new System.Windows.Forms.NumericUpDown();
			this.bUpdateScaleX = new System.Windows.Forms.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.label3.AutoSize = true;
			this.maxScaleX = new System.Windows.Forms.NumericUpDown();
			this.label2 = new System.Windows.Forms.Label();
			this.label2.AutoSize = true;
			this.minScaleX = new System.Windows.Forms.NumericUpDown();
			this.toolTip = new System.Windows.Forms.ToolTip(this.components);
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.scalegroup.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.maxScaleY)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.minScaleY)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.maxScaleX)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.minScaleX)).BeginInit();
			this.SuspendLayout();
			// 
			// bApply
			// 
			this.bApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.bApply.Location = new System.Drawing.Point(115, 399);
			this.bApply.Name = "bApply";
			this.bApply.Size = new System.Drawing.Size(95, 23);
			this.bApply.TabIndex = 0;
			this.bApply.Text = "Apply";
			this.bApply.UseVisualStyleBackColor = true;
			this.bApply.Click += new System.EventHandler(this.bApply_Click);
			// 
			// bCancel
			// 
			this.bCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.bCancel.Location = new System.Drawing.Point(216, 399);
			this.bCancel.Name = "bCancel";
			this.bCancel.Size = new System.Drawing.Size(75, 23);
			this.bCancel.TabIndex = 1;
			this.bCancel.Text = "Cancel";
			this.bCancel.UseVisualStyleBackColor = true;
			this.bCancel.Click += new System.EventHandler(this.bCancel_Click);
			// 
			// bUpdateTranslation
			// 
			this.bUpdateTranslation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.bUpdateTranslation.Image = global::CodeImp.DoomBuilder.BuilderEffects.Properties.Resources.Update;
			this.bUpdateTranslation.Location = new System.Drawing.Point(247, 18);
			this.bUpdateTranslation.Name = "bUpdateTranslation";
			this.bUpdateTranslation.Size = new System.Drawing.Size(23, 23);
			this.bUpdateTranslation.TabIndex = 5;
			this.bUpdateTranslation.UseVisualStyleBackColor = true;
			this.bUpdateTranslation.Click += new System.EventHandler(this.bUpdateTranslation_Click);
			// 
			// positionJitterAmmount
			// 
			this.positionJitterAmmount.AllowNegative = false;
			this.positionJitterAmmount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.positionJitterAmmount.ExtendedLimits = true;
			this.positionJitterAmmount.Label = "Position:";
			this.positionJitterAmmount.Location = new System.Drawing.Point(-25, 19);
			this.positionJitterAmmount.Maximum = 100;
			this.positionJitterAmmount.Minimum = 0;
			this.positionJitterAmmount.Name = "positionJitterAmmount";
			this.positionJitterAmmount.Size = new System.Drawing.Size(266, 22);
			this.positionJitterAmmount.TabIndex = 6;
			this.positionJitterAmmount.Value = 0;
			this.positionJitterAmmount.OnValueChanging += new System.EventHandler(this.positionJitterAmmount_OnValueChanged);
			// 
			// bUpdateAngle
			// 
			this.bUpdateAngle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.bUpdateAngle.Image = global::CodeImp.DoomBuilder.BuilderEffects.Properties.Resources.Update;
			this.bUpdateAngle.Location = new System.Drawing.Point(247, 18);
			this.bUpdateAngle.Name = "bUpdateAngle";
			this.bUpdateAngle.Size = new System.Drawing.Size(23, 23);
			this.bUpdateAngle.TabIndex = 5;
			this.bUpdateAngle.UseVisualStyleBackColor = true;
			this.bUpdateAngle.Click += new System.EventHandler(this.bUpdateAngle_Click);
			// 
			// rotationJitterAmmount
			// 
			this.rotationJitterAmmount.AllowNegative = false;
			this.rotationJitterAmmount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.rotationJitterAmmount.ExtendedLimits = false;
			this.rotationJitterAmmount.Label = "Angle:";
			this.rotationJitterAmmount.Location = new System.Drawing.Point(-25, 19);
			this.rotationJitterAmmount.Maximum = 359;
			this.rotationJitterAmmount.Minimum = 0;
			this.rotationJitterAmmount.Name = "rotationJitterAmmount";
			this.rotationJitterAmmount.Size = new System.Drawing.Size(266, 22);
			this.rotationJitterAmmount.TabIndex = 8;
			this.rotationJitterAmmount.Value = 0;
			this.rotationJitterAmmount.OnValueChanging += new System.EventHandler(this.rotationJitterAmmount_OnValueChanged);
			// 
			// heightJitterAmmount
			// 
			this.heightJitterAmmount.AllowNegative = false;
			this.heightJitterAmmount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.heightJitterAmmount.ExtendedLimits = false;
			this.heightJitterAmmount.Label = "Height:";
			this.heightJitterAmmount.Location = new System.Drawing.Point(-25, 47);
			this.heightJitterAmmount.Maximum = 100;
			this.heightJitterAmmount.Minimum = 0;
			this.heightJitterAmmount.Name = "heightJitterAmmount";
			this.heightJitterAmmount.Size = new System.Drawing.Size(266, 22);
			this.heightJitterAmmount.TabIndex = 6;
			this.heightJitterAmmount.Value = 0;
			this.heightJitterAmmount.OnValueChanging += new System.EventHandler(this.heightJitterAmmount_OnValueChanging);
			// 
			// bUpdateHeight
			// 
			this.bUpdateHeight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.bUpdateHeight.Image = global::CodeImp.DoomBuilder.BuilderEffects.Properties.Resources.Update;
			this.bUpdateHeight.Location = new System.Drawing.Point(247, 46);
			this.bUpdateHeight.Name = "bUpdateHeight";
			this.bUpdateHeight.Size = new System.Drawing.Size(23, 23);
			this.bUpdateHeight.TabIndex = 5;
			this.bUpdateHeight.UseVisualStyleBackColor = true;
			this.bUpdateHeight.Click += new System.EventHandler(this.bUpdateHeight_Click);
			// 
			// pitchAmmount
			// 
			this.pitchAmmount.AllowNegative = false;
			this.pitchAmmount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.pitchAmmount.ExtendedLimits = false;
			this.pitchAmmount.Label = "Pitch:";
			this.pitchAmmount.Location = new System.Drawing.Point(-25, 47);
			this.pitchAmmount.Maximum = 359;
			this.pitchAmmount.Minimum = 0;
			this.pitchAmmount.Name = "pitchAmmount";
			this.pitchAmmount.Size = new System.Drawing.Size(266, 24);
			this.pitchAmmount.TabIndex = 13;
			this.pitchAmmount.Value = 0;
			this.pitchAmmount.OnValueChanging += new System.EventHandler(this.pitchAmmount_OnValueChanging);
			// 
			// rollAmmount
			// 
			this.rollAmmount.AllowNegative = false;
			this.rollAmmount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.rollAmmount.ExtendedLimits = false;
			this.rollAmmount.Label = "Roll:";
			this.rollAmmount.Location = new System.Drawing.Point(-25, 77);
			this.rollAmmount.Maximum = 359;
			this.rollAmmount.Minimum = 0;
			this.rollAmmount.Name = "rollAmmount";
			this.rollAmmount.Size = new System.Drawing.Size(266, 24);
			this.rollAmmount.TabIndex = 14;
			this.rollAmmount.Value = 0;
			this.rollAmmount.OnValueChanging += new System.EventHandler(this.rollAmmount_OnValueChanging);
			// 
			// bUpdatePitch
			// 
			this.bUpdatePitch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.bUpdatePitch.Image = global::CodeImp.DoomBuilder.BuilderEffects.Properties.Resources.Update;
			this.bUpdatePitch.Location = new System.Drawing.Point(247, 47);
			this.bUpdatePitch.Name = "bUpdatePitch";
			this.bUpdatePitch.Size = new System.Drawing.Size(23, 23);
			this.bUpdatePitch.TabIndex = 15;
			this.bUpdatePitch.UseVisualStyleBackColor = true;
			this.bUpdatePitch.Click += new System.EventHandler(this.bUpdatePitch_Click);
			// 
			// bUpdateRoll
			// 
			this.bUpdateRoll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.bUpdateRoll.Image = global::CodeImp.DoomBuilder.BuilderEffects.Properties.Resources.Update;
			this.bUpdateRoll.Location = new System.Drawing.Point(247, 76);
			this.bUpdateRoll.Name = "bUpdateRoll";
			this.bUpdateRoll.Size = new System.Drawing.Size(23, 23);
			this.bUpdateRoll.TabIndex = 16;
			this.bUpdateRoll.UseVisualStyleBackColor = true;
			this.bUpdateRoll.Click += new System.EventHandler(this.bUpdateRoll_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.positionJitterAmmount);
			this.groupBox1.Controls.Add(this.bUpdateTranslation);
			this.groupBox1.Controls.Add(this.bUpdateHeight);
			this.groupBox1.Controls.Add(this.heightJitterAmmount);
			this.groupBox1.Location = new System.Drawing.Point(12, 12);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(279, 82);
			this.groupBox1.TabIndex = 17;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = " Position: ";
			// 
			// groupBox2
			// 
			this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox2.Controls.Add(this.cbNegativeRoll);
			this.groupBox2.Controls.Add(this.cbNegativePitch);
			this.groupBox2.Controls.Add(this.cbRelativeRoll);
			this.groupBox2.Controls.Add(this.cbRelativePitch);
			this.groupBox2.Controls.Add(this.rotationJitterAmmount);
			this.groupBox2.Controls.Add(this.bUpdateAngle);
			this.groupBox2.Controls.Add(this.bUpdateRoll);
			this.groupBox2.Controls.Add(this.pitchAmmount);
			this.groupBox2.Controls.Add(this.rollAmmount);
			this.groupBox2.Controls.Add(this.bUpdatePitch);
			this.groupBox2.Location = new System.Drawing.Point(12, 100);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(279, 159);
			this.groupBox2.TabIndex = 18;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = " Rotation: ";
			// 
			// cbNegativeRoll
			// 
			this.cbNegativeRoll.AutoSize = true;
			this.cbNegativeRoll.Location = new System.Drawing.Point(150, 134);
			this.cbNegativeRoll.Name = "cbNegativeRoll";
			this.cbNegativeRoll.Size = new System.Drawing.Size(105, 17);
			this.cbNegativeRoll.TabIndex = 20;
			this.cbNegativeRoll.Text = "Use negative roll";
			this.toolTip.SetToolTip(this.cbNegativeRoll, "When checked, 50% of the time \r\nnegative roll will be used");
			this.cbNegativeRoll.UseVisualStyleBackColor = true;
			// 
			// cbNegativePitch
			// 
			this.cbNegativePitch.AutoSize = true;
			this.cbNegativePitch.Location = new System.Drawing.Point(150, 110);
			this.cbNegativePitch.Name = "cbNegativePitch";
			this.cbNegativePitch.Size = new System.Drawing.Size(115, 17);
			this.cbNegativePitch.TabIndex = 19;
			this.cbNegativePitch.Text = "Use negative pitch";
			this.toolTip.SetToolTip(this.cbNegativePitch, "When checked, 50% of the time \r\nnegative pitch will be used.");
			this.cbNegativePitch.UseVisualStyleBackColor = true;
			// 
			// cbRelativeRoll
			// 
			this.cbRelativeRoll.AutoSize = true;
			this.cbRelativeRoll.Location = new System.Drawing.Point(9, 134);
			this.cbRelativeRoll.Name = "cbRelativeRoll";
			this.cbRelativeRoll.Size = new System.Drawing.Size(119, 17);
			this.cbRelativeRoll.TabIndex = 18;
			this.cbRelativeRoll.Text = "Relative to initial roll";
			this.cbRelativeRoll.UseVisualStyleBackColor = true;
			// 
			// cbRelativePitch
			// 
			this.cbRelativePitch.AutoSize = true;
			this.cbRelativePitch.Location = new System.Drawing.Point(9, 110);
			this.cbRelativePitch.Name = "cbRelativePitch";
			this.cbRelativePitch.Size = new System.Drawing.Size(129, 17);
			this.cbRelativePitch.TabIndex = 17;
			this.cbRelativePitch.Text = "Relative to initial pitch";
			this.cbRelativePitch.UseVisualStyleBackColor = true;
			// 
			// scalegroup
			// 
			this.scalegroup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.scalegroup.Controls.Add(this.cbNegativeScaleY);
			this.scalegroup.Controls.Add(this.cbNegativeScaleX);
			this.scalegroup.Controls.Add(this.cbUniformScale);
			this.scalegroup.Controls.Add(this.cbRelativeScale);
			this.scalegroup.Controls.Add(this.bUpdateScaleY);
			this.scalegroup.Controls.Add(this.maxScaleYLabel);
			this.scalegroup.Controls.Add(this.maxScaleY);
			this.scalegroup.Controls.Add(this.minScaleYLabel);
			this.scalegroup.Controls.Add(this.minScaleY);
			this.scalegroup.Controls.Add(this.bUpdateScaleX);
			this.scalegroup.Controls.Add(this.label3);
			this.scalegroup.Controls.Add(this.maxScaleX);
			this.scalegroup.Controls.Add(this.label2);
			this.scalegroup.Controls.Add(this.minScaleX);
			this.scalegroup.Location = new System.Drawing.Point(12, 265);
			this.scalegroup.Name = "scalegroup";
			this.scalegroup.Size = new System.Drawing.Size(279, 127);
			this.scalegroup.TabIndex = 19;
			this.scalegroup.TabStop = false;
			this.scalegroup.Text = " Scale: ";
			// 
			// cbNegativeScaleY
			// 
			this.cbNegativeScaleY.AutoSize = true;
			this.cbNegativeScaleY.Location = new System.Drawing.Point(150, 102);
			this.cbNegativeScaleY.Name = "cbNegativeScaleY";
			this.cbNegativeScaleY.Size = new System.Drawing.Size(121, 17);
			this.cbNegativeScaleY.TabIndex = 25;
			this.cbNegativeScaleY.Text = "Use negative height";
			this.toolTip.SetToolTip(this.cbNegativeScaleY, "When checked, height scale will be picked from\r\n[-max .. -min] - [min .. max] ran" +
					"ge");
			this.cbNegativeScaleY.UseVisualStyleBackColor = true;
			// 
			// cbNegativeScaleX
			// 
			this.cbNegativeScaleX.AutoSize = true;
			this.cbNegativeScaleX.Location = new System.Drawing.Point(150, 78);
			this.cbNegativeScaleX.Name = "cbNegativeScaleX";
			this.cbNegativeScaleX.Size = new System.Drawing.Size(117, 17);
			this.cbNegativeScaleX.TabIndex = 24;
			this.cbNegativeScaleX.Text = "Use negative width";
			this.toolTip.SetToolTip(this.cbNegativeScaleX, "When checked, width scale will be picked from\r\n[-max .. -min] - [min .. max] rang" +
					"e");
			this.cbNegativeScaleX.UseVisualStyleBackColor = true;
			// 
			// cbUniformScale
			// 
			this.cbUniformScale.AutoSize = true;
			this.cbUniformScale.Location = new System.Drawing.Point(9, 102);
			this.cbUniformScale.Name = "cbUniformScale";
			this.cbUniformScale.Size = new System.Drawing.Size(134, 17);
			this.cbUniformScale.TabIndex = 23;
			this.cbUniformScale.Text = "Same width and height";
			this.cbUniformScale.UseVisualStyleBackColor = true;
			// 
			// cbRelativeScale
			// 
			this.cbRelativeScale.AutoSize = true;
			this.cbRelativeScale.Location = new System.Drawing.Point(9, 78);
			this.cbRelativeScale.Name = "cbRelativeScale";
			this.cbRelativeScale.Size = new System.Drawing.Size(131, 17);
			this.cbRelativeScale.TabIndex = 13;
			this.cbRelativeScale.Text = "Relative to initial scale";
			this.cbRelativeScale.UseVisualStyleBackColor = true;
			// 
			// bUpdateScaleY
			// 
			this.bUpdateScaleY.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.bUpdateScaleY.Image = global::CodeImp.DoomBuilder.BuilderEffects.Properties.Resources.Update;
			this.bUpdateScaleY.Location = new System.Drawing.Point(247, 45);
			this.bUpdateScaleY.Name = "bUpdateScaleY";
			this.bUpdateScaleY.Size = new System.Drawing.Size(23, 23);
			this.bUpdateScaleY.TabIndex = 22;
			this.bUpdateScaleY.UseVisualStyleBackColor = true;
			this.bUpdateScaleY.Click += new System.EventHandler(this.bUpdateScaleY_Click);
			// 
			// maxScaleYLabel
			// 
			this.maxScaleYLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.maxScaleYLabel.AutoSize = true;
			this.maxScaleYLabel.Location = new System.Drawing.Point(147, 51);
			this.maxScaleYLabel.Name = "maxScaleYLabel";
			this.maxScaleYLabel.Size = new System.Drawing.Size(32, 13);
			this.maxScaleYLabel.TabIndex = 21;
			this.maxScaleYLabel.Text = "max.:";
			// 
			// maxScaleY
			// 
			this.maxScaleY.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.maxScaleY.DecimalPlaces = 2;
			this.maxScaleY.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
			this.maxScaleY.Location = new System.Drawing.Point(186, 48);
			this.maxScaleY.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
			this.maxScaleY.Name = "maxScaleY";
			this.maxScaleY.Size = new System.Drawing.Size(55, 20);
			this.maxScaleY.TabIndex = 20;
			this.maxScaleY.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.maxScaleY.ValueChanged += new System.EventHandler(this.minScaleY_ValueChanged);
			// 
			// minScaleYLabel
			// 
			this.minScaleYLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.minScaleYLabel.AutoSize = true;
			this.minScaleYLabel.Location = new System.Drawing.Point(16, 51);
			this.minScaleYLabel.Name = "minScaleYLabel";
			this.minScaleYLabel.Size = new System.Drawing.Size(63, 13);
			this.minScaleYLabel.TabIndex = 19;
			this.minScaleYLabel.Text = "Height min.:";
			// 
			// minScaleY
			// 
			this.minScaleY.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.minScaleY.DecimalPlaces = 2;
			this.minScaleY.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
			this.minScaleY.Location = new System.Drawing.Point(84, 48);
			this.minScaleY.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
			this.minScaleY.Name = "minScaleY";
			this.minScaleY.Size = new System.Drawing.Size(55, 20);
			this.minScaleY.TabIndex = 18;
			this.minScaleY.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.minScaleY.ValueChanged += new System.EventHandler(this.minScaleY_ValueChanged);
			// 
			// bUpdateScaleX
			// 
			this.bUpdateScaleX.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.bUpdateScaleX.Image = global::CodeImp.DoomBuilder.BuilderEffects.Properties.Resources.Update;
			this.bUpdateScaleX.Location = new System.Drawing.Point(247, 19);
			this.bUpdateScaleX.Name = "bUpdateScaleX";
			this.bUpdateScaleX.Size = new System.Drawing.Size(23, 23);
			this.bUpdateScaleX.TabIndex = 17;
			this.bUpdateScaleX.UseVisualStyleBackColor = true;
			this.bUpdateScaleX.Click += new System.EventHandler(this.bUpdateScaleX_Click);
			// 
			// label3
			// 
			this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(147, 25);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(32, 13);
			this.label3.TabIndex = 4;
			this.label3.Text = "max.:";
			// 
			// maxScaleX
			// 
			this.maxScaleX.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.maxScaleX.DecimalPlaces = 2;
			this.maxScaleX.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
			this.maxScaleX.Location = new System.Drawing.Point(186, 22);
			this.maxScaleX.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
			this.maxScaleX.Name = "maxScaleX";
			this.maxScaleX.Size = new System.Drawing.Size(55, 20);
			this.maxScaleX.TabIndex = 3;
			this.maxScaleX.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.maxScaleX.ValueChanged += new System.EventHandler(this.minScaleX_ValueChanged);
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(19, 25);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(60, 13);
			this.label2.TabIndex = 2;
			this.label2.Text = "Width min.:";
			// 
			// minScaleX
			// 
			this.minScaleX.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.minScaleX.DecimalPlaces = 2;
			this.minScaleX.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
			this.minScaleX.Location = new System.Drawing.Point(84, 22);
			this.minScaleX.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
			this.minScaleX.Name = "minScaleX";
			this.minScaleX.Size = new System.Drawing.Size(55, 20);
			this.minScaleX.TabIndex = 1;
			this.minScaleX.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.minScaleX.ValueChanged += new System.EventHandler(this.minScaleX_ValueChanged);
			// 
			// JitterThingsForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.ClientSize = new System.Drawing.Size(303, 428);
			this.Controls.Add(this.scalegroup);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.bCancel);
			this.Controls.Add(this.bApply);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "JitterThingsForm";
			this.Opacity = 0;
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "Randomize Things!";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.JitterThingsForm_FormClosing);
			this.groupBox1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.scalegroup.ResumeLayout(false);
			this.scalegroup.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.maxScaleY)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.minScaleY)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.maxScaleX)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.minScaleX)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button bApply;
		private System.Windows.Forms.Button bCancel;
		private System.Windows.Forms.Button bUpdateTranslation;
		private CodeImp.DoomBuilder.BuilderEffects.IntControl positionJitterAmmount;
		private System.Windows.Forms.Button bUpdateAngle;
		private CodeImp.DoomBuilder.BuilderEffects.IntControl rotationJitterAmmount;
		private CodeImp.DoomBuilder.BuilderEffects.IntControl heightJitterAmmount;
		private System.Windows.Forms.Button bUpdateHeight;
		private CodeImp.DoomBuilder.BuilderEffects.IntControl pitchAmmount;
		private CodeImp.DoomBuilder.BuilderEffects.IntControl rollAmmount;
		private System.Windows.Forms.Button bUpdatePitch;
		private System.Windows.Forms.Button bUpdateRoll;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.GroupBox scalegroup;
		private System.Windows.Forms.NumericUpDown minScaleX;
		private System.Windows.Forms.CheckBox cbUniformScale;
		private System.Windows.Forms.CheckBox cbRelativeScale;
		private System.Windows.Forms.Button bUpdateScaleY;
		private System.Windows.Forms.Label maxScaleYLabel;
		private System.Windows.Forms.NumericUpDown maxScaleY;
		private System.Windows.Forms.Label minScaleYLabel;
		private System.Windows.Forms.NumericUpDown minScaleY;
		private System.Windows.Forms.Button bUpdateScaleX;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.NumericUpDown maxScaleX;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.CheckBox cbNegativeScaleX;
		private System.Windows.Forms.CheckBox cbRelativeRoll;
		private System.Windows.Forms.CheckBox cbRelativePitch;
		private System.Windows.Forms.CheckBox cbNegativeScaleY;
		private System.Windows.Forms.ToolTip toolTip;
		private System.Windows.Forms.CheckBox cbNegativeRoll;
		private System.Windows.Forms.CheckBox cbNegativePitch;
	}
}
