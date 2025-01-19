namespace CodeImp.DoomBuilder.StairSectorBuilderMode
{
	partial class StairSectorBuilderForm
	{
		/// <summary>
		/// Erforderliche Designervariable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Verwendete Ressourcen bereinigen.
		/// </summary>
		/// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
		protected override void Dispose(bool disposing)
		{
			if(disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Vom Windows Form-Designer generierter Code

		/// <summary>
		/// Erforderliche Methode für die Designerunterstützung.
		/// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
		/// </summary>
		private void InitializeComponent()
		{
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.numberofsectors = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.label4 = new System.Windows.Forms.Label();
			this.label4.AutoSize = true;
			this.tabcontrol = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.distinctbaseheights = new System.Windows.Forms.CheckBox();
			this.spacing = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.label11 = new System.Windows.Forms.Label();
			this.label11.AutoSize = true;
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.distinctsectors = new System.Windows.Forms.CheckBox();
			this.singledirection = new System.Windows.Forms.CheckBox();
			this.singlesteps = new System.Windows.Forms.CheckBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.sideback = new System.Windows.Forms.RadioButton();
			this.sidefront = new System.Windows.Forms.RadioButton();
			this.sectordepth = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.label1 = new System.Windows.Forms.Label();
			this.label1.AutoSize = true;
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.autocurveflipping = new System.Windows.Forms.ComboBox();
			this.label12 = new System.Windows.Forms.Label();
			this.label12.AutoSize = true;
			this.autocurveoutervertexmultiplier = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.autocurveinnervertexmultiplier = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.label6 = new System.Windows.Forms.Label();
			this.label6.AutoSize = true;
			this.label5 = new System.Windows.Forms.Label();
			this.label5.AutoSize = true;
			this.tabPage3 = new System.Windows.Forms.TabPage();
			this.splineflipping = new System.Windows.Forms.ComboBox();
			this.label13 = new System.Windows.Forms.Label();
			this.label13.AutoSize = true;
			this.splineoutervertexmultiplier = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.splineinnervertexmultiplier = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.label16 = new System.Windows.Forms.Label();
			this.label16.AutoSize = true;
			this.label17 = new System.Windows.Forms.Label();
			this.label17.AutoSize = true;
			this.numberofcontrolpoints = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.label10 = new System.Windows.Forms.Label();
			this.label10.AutoSize = true;
			this.groupBox4 = new System.Windows.Forms.GroupBox();
			this.floorflattexture = new CodeImp.DoomBuilder.Controls.FlatSelectorControl();
			this.floorflat = new System.Windows.Forms.CheckBox();
			this.ceilingflat = new System.Windows.Forms.CheckBox();
			this.groupBox5 = new System.Windows.Forms.GroupBox();
			this.ceilingflattexture = new CodeImp.DoomBuilder.Controls.FlatSelectorControl();
			this.groupBox6 = new System.Windows.Forms.GroupBox();
			this.label14 = new System.Windows.Forms.Label();
			this.label14.AutoSize = true;
			this.floorbase = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.floorheightmodification = new System.Windows.Forms.CheckBox();
			this.floorlast = new System.Windows.Forms.Label();
			this.floorlast.AutoSize = true;
			this.floorfirst = new System.Windows.Forms.Label();
			this.floorfirst.AutoSize = true;
			this.label7 = new System.Windows.Forms.Label();
			this.label7.AutoSize = true;
			this.label3 = new System.Windows.Forms.Label();
			this.label3.AutoSize = true;
			this.floorheightmod = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.label = new System.Windows.Forms.Label();
			this.label.AutoSize = true;
			this.groupBox7 = new System.Windows.Forms.GroupBox();
			this.lowerunpegged = new System.Windows.Forms.CheckBox();
			this.lowertexturetexture = new CodeImp.DoomBuilder.Controls.TextureSelectorControl();
			this.lowertexture = new System.Windows.Forms.CheckBox();
			this.groupBox8 = new System.Windows.Forms.GroupBox();
			this.upperunpegged = new System.Windows.Forms.CheckBox();
			this.uppertexturetexture = new CodeImp.DoomBuilder.Controls.TextureSelectorControl();
			this.uppertexture = new System.Windows.Forms.CheckBox();
			this.ceilinglast = new System.Windows.Forms.Label();
			this.ceilinglast.AutoSize = true;
			this.ceilingfirst = new System.Windows.Forms.Label();
			this.ceilingfirst.AutoSize = true;
			this.label8 = new System.Windows.Forms.Label();
			this.label8.AutoSize = true;
			this.label9 = new System.Windows.Forms.Label();
			this.label9.AutoSize = true;
			this.ceilingheightmod = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.label2 = new System.Windows.Forms.Label();
			this.label2.AutoSize = true;
			this.groupBox9 = new System.Windows.Forms.GroupBox();
			this.label15 = new System.Windows.Forms.Label();
			this.label15.AutoSize = true;
			this.ceilingbase = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.ceilingheightmodification = new System.Windows.Forms.CheckBox();
			this.groupBox11 = new System.Windows.Forms.GroupBox();
			this.prefabload = new System.Windows.Forms.Button();
			this.prefabdelete = new System.Windows.Forms.Button();
			this.prefabsave = new System.Windows.Forms.Button();
			this.prefabname = new System.Windows.Forms.TextBox();
			this.prefabs = new System.Windows.Forms.ListView();
			this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.groupBox10 = new System.Windows.Forms.GroupBox();
			this.middletexturetexture = new CodeImp.DoomBuilder.Controls.TextureSelectorControl();
			this.middletexture = new System.Windows.Forms.CheckBox();
			this.prefabdefault = new System.Windows.Forms.Button();
			this.groupBox1.SuspendLayout();
			this.tabcontrol.SuspendLayout();
			this.tabPage1.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.tabPage2.SuspendLayout();
			this.tabPage3.SuspendLayout();
			this.groupBox4.SuspendLayout();
			this.groupBox5.SuspendLayout();
			this.groupBox6.SuspendLayout();
			this.groupBox7.SuspendLayout();
			this.groupBox8.SuspendLayout();
			this.groupBox9.SuspendLayout();
			this.groupBox11.SuspendLayout();
			this.groupBox10.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnOK
			// 
			this.btnOK.Location = new System.Drawing.Point(333, 532);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(112, 27);
			this.btnOK.TabIndex = 4;
			this.btnOK.Text = "OK";
			this.btnOK.UseVisualStyleBackColor = true;
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(448, 532);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(112, 27);
			this.btnCancel.TabIndex = 5;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.numberofsectors);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Location = new System.Drawing.Point(12, 9);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(220, 50);
			this.groupBox1.TabIndex = 9;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "General";
			// 
			// numberofsectors
			// 
			this.numberofsectors.AllowDecimal = false;
			this.numberofsectors.AllowExpressions = false;
			this.numberofsectors.AllowNegative = false;
			this.numberofsectors.AllowRelative = false;
			this.numberofsectors.ButtonStep = 1;
			this.numberofsectors.ButtonStepBig = 10F;
			this.numberofsectors.ButtonStepFloat = 1F;
			this.numberofsectors.ButtonStepSmall = 0.1F;
			this.numberofsectors.ButtonStepsUseModifierKeys = false;
			this.numberofsectors.ButtonStepsWrapAround = false;
			this.numberofsectors.Location = new System.Drawing.Point(112, 17);
			this.numberofsectors.Name = "numberofsectors";
			this.numberofsectors.Size = new System.Drawing.Size(56, 24);
			this.numberofsectors.StepValues = null;
			this.numberofsectors.TabIndex = 1;
			this.numberofsectors.WhenTextChanged += new System.EventHandler(this.numberofsectors_WhenTextChanged);
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(9, 22);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(97, 14);
			this.label4.TabIndex = 0;
			this.label4.Text = "Number of sectors";
			// 
			// tabcontrol
			// 
			this.tabcontrol.Controls.Add(this.tabPage1);
			this.tabcontrol.Controls.Add(this.tabPage2);
			this.tabcontrol.Controls.Add(this.tabPage3);
			this.tabcontrol.Location = new System.Drawing.Point(12, 65);
			this.tabcontrol.Name = "tabcontrol";
			this.tabcontrol.SelectedIndex = 0;
			this.tabcontrol.Size = new System.Drawing.Size(220, 271);
			this.tabcontrol.TabIndex = 10;
			this.tabcontrol.SelectedIndexChanged += new System.EventHandler(this.tabcontrol_SelectedIndexChanged);
			// 
			// tabPage1
			// 
			this.tabPage1.Controls.Add(this.distinctbaseheights);
			this.tabPage1.Controls.Add(this.spacing);
			this.tabPage1.Controls.Add(this.label11);
			this.tabPage1.Controls.Add(this.groupBox3);
			this.tabPage1.Controls.Add(this.groupBox2);
			this.tabPage1.Controls.Add(this.sectordepth);
			this.tabPage1.Controls.Add(this.label1);
			this.tabPage1.Location = new System.Drawing.Point(4, 23);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage1.Size = new System.Drawing.Size(212, 244);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "Straight";
			this.tabPage1.UseVisualStyleBackColor = true;
			// 
			// distinctbaseheights
			// 
			this.distinctbaseheights.AutoSize = true;
			this.distinctbaseheights.Location = new System.Drawing.Point(10, 70);
			this.distinctbaseheights.Name = "distinctbaseheights";
			this.distinctbaseheights.Size = new System.Drawing.Size(126, 18);
			this.distinctbaseheights.TabIndex = 6;
			this.distinctbaseheights.Text = "Distinct base heights";
			this.distinctbaseheights.UseVisualStyleBackColor = true;
			this.distinctbaseheights.CheckedChanged += new System.EventHandler(this.distinctbaseheights_CheckedChanged);
			// 
			// spacing
			// 
			this.spacing.AllowDecimal = false;
			this.spacing.AllowExpressions = false;
			this.spacing.AllowNegative = false;
			this.spacing.AllowRelative = false;
			this.spacing.ButtonStep = 8;
			this.spacing.ButtonStepBig = 10F;
			this.spacing.ButtonStepFloat = 1F;
			this.spacing.ButtonStepSmall = 0.1F;
			this.spacing.ButtonStepsUseModifierKeys = false;
			this.spacing.ButtonStepsWrapAround = false;
			this.spacing.Location = new System.Drawing.Point(81, 33);
			this.spacing.Name = "spacing";
			this.spacing.Size = new System.Drawing.Size(56, 24);
			this.spacing.StepValues = null;
			this.spacing.TabIndex = 5;
			this.spacing.WhenTextChanged += new System.EventHandler(this.spacing_WhenTextChanged);
			// 
			// label11
			// 
			this.label11.AutoSize = true;
			this.label11.Location = new System.Drawing.Point(7, 37);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(46, 14);
			this.label11.TabIndex = 4;
			this.label11.Text = "Spacing";
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.distinctsectors);
			this.groupBox3.Controls.Add(this.singledirection);
			this.groupBox3.Controls.Add(this.singlesteps);
			this.groupBox3.Location = new System.Drawing.Point(82, 94);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(124, 93);
			this.groupBox3.TabIndex = 3;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Appearance";
			// 
			// distinctsectors
			// 
			this.distinctsectors.AutoSize = true;
			this.distinctsectors.Enabled = false;
			this.distinctsectors.Location = new System.Drawing.Point(10, 43);
			this.distinctsectors.Name = "distinctsectors";
			this.distinctsectors.Size = new System.Drawing.Size(101, 18);
			this.distinctsectors.TabIndex = 2;
			this.distinctsectors.Text = "Distinct sectors";
			this.distinctsectors.UseVisualStyleBackColor = true;
			this.distinctsectors.CheckedChanged += new System.EventHandler(this.distinctsectors_CheckedChanged);
			// 
			// singledirection
			// 
			this.singledirection.AutoSize = true;
			this.singledirection.Enabled = false;
			this.singledirection.Location = new System.Drawing.Point(10, 64);
			this.singledirection.Name = "singledirection";
			this.singledirection.Size = new System.Drawing.Size(99, 18);
			this.singledirection.TabIndex = 1;
			this.singledirection.Text = "Single direction";
			this.singledirection.UseVisualStyleBackColor = true;
			this.singledirection.CheckedChanged += new System.EventHandler(this.singledirection_CheckedChanged);
			// 
			// singlesteps
			// 
			this.singlesteps.AutoSize = true;
			this.singlesteps.Enabled = false;
			this.singlesteps.Location = new System.Drawing.Point(10, 22);
			this.singlesteps.Name = "singlesteps";
			this.singlesteps.Size = new System.Drawing.Size(85, 18);
			this.singlesteps.TabIndex = 0;
			this.singlesteps.Text = "Single steps";
			this.singlesteps.UseVisualStyleBackColor = true;
			this.singlesteps.CheckedChanged += new System.EventHandler(this.singleseteps_CheckedChanged);
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.sideback);
			this.groupBox2.Controls.Add(this.sidefront);
			this.groupBox2.Location = new System.Drawing.Point(11, 94);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(65, 93);
			this.groupBox2.TabIndex = 2;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Side";
			// 
			// sideback
			// 
			this.sideback.AutoSize = true;
			this.sideback.Location = new System.Drawing.Point(7, 54);
			this.sideback.Name = "sideback";
			this.sideback.Size = new System.Drawing.Size(49, 18);
			this.sideback.TabIndex = 1;
			this.sideback.TabStop = true;
			this.sideback.Text = "Back";
			this.sideback.UseVisualStyleBackColor = true;
			this.sideback.CheckedChanged += new System.EventHandler(this.sideback_CheckedChanged);
			// 
			// sidefront
			// 
			this.sidefront.AutoSize = true;
			this.sidefront.Checked = true;
			this.sidefront.Location = new System.Drawing.Point(7, 30);
			this.sidefront.Name = "sidefront";
			this.sidefront.Size = new System.Drawing.Size(50, 18);
			this.sidefront.TabIndex = 0;
			this.sidefront.TabStop = true;
			this.sidefront.Text = "Front";
			this.sidefront.UseVisualStyleBackColor = true;
			this.sidefront.CheckedChanged += new System.EventHandler(this.sidefront_CheckedChanged);
			// 
			// sectordepth
			// 
			this.sectordepth.AllowDecimal = false;
			this.sectordepth.AllowExpressions = false;
			this.sectordepth.AllowNegative = false;
			this.sectordepth.AllowRelative = false;
			this.sectordepth.ButtonStep = 8;
			this.sectordepth.ButtonStepBig = 10F;
			this.sectordepth.ButtonStepFloat = 1F;
			this.sectordepth.ButtonStepSmall = 0.1F;
			this.sectordepth.ButtonStepsUseModifierKeys = false;
			this.sectordepth.ButtonStepsWrapAround = false;
			this.sectordepth.Location = new System.Drawing.Point(81, 3);
			this.sectordepth.Name = "sectordepth";
			this.sectordepth.Size = new System.Drawing.Size(56, 24);
			this.sectordepth.StepValues = null;
			this.sectordepth.TabIndex = 1;
			this.sectordepth.WhenTextChanged += new System.EventHandler(this.sectordepth_WhenTextChanged);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(7, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(69, 14);
			this.label1.TabIndex = 0;
			this.label1.Text = "Sector depth";
			// 
			// tabPage2
			// 
			this.tabPage2.Controls.Add(this.autocurveflipping);
			this.tabPage2.Controls.Add(this.label12);
			this.tabPage2.Controls.Add(this.autocurveoutervertexmultiplier);
			this.tabPage2.Controls.Add(this.autocurveinnervertexmultiplier);
			this.tabPage2.Controls.Add(this.label6);
			this.tabPage2.Controls.Add(this.label5);
			this.tabPage2.Location = new System.Drawing.Point(4, 23);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage2.Size = new System.Drawing.Size(212, 244);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "Auto curve";
			this.tabPage2.UseVisualStyleBackColor = true;
			// 
			// autocurveflipping
			// 
			this.autocurveflipping.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.autocurveflipping.FormattingEnabled = true;
			this.autocurveflipping.Items.AddRange(new object[] {
            "None",
            "Source direction",
            "Destination direction"});
			this.autocurveflipping.Location = new System.Drawing.Point(55, 9);
			this.autocurveflipping.Name = "autocurveflipping";
			this.autocurveflipping.Size = new System.Drawing.Size(151, 22);
			this.autocurveflipping.TabIndex = 11;
			this.autocurveflipping.SelectedIndexChanged += new System.EventHandler(this.autocurveflipping_SelectedIndexChanged);
			// 
			// label12
			// 
			this.label12.AutoSize = true;
			this.label12.Location = new System.Drawing.Point(6, 12);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(43, 14);
			this.label12.TabIndex = 10;
			this.label12.Text = "Flipping";
			// 
			// autocurveoutervertexmultiplier
			// 
			this.autocurveoutervertexmultiplier.AllowDecimal = false;
			this.autocurveoutervertexmultiplier.AllowExpressions = false;
			this.autocurveoutervertexmultiplier.AllowNegative = false;
			this.autocurveoutervertexmultiplier.AllowRelative = false;
			this.autocurveoutervertexmultiplier.ButtonStep = 1;
			this.autocurveoutervertexmultiplier.ButtonStepBig = 10F;
			this.autocurveoutervertexmultiplier.ButtonStepFloat = 1F;
			this.autocurveoutervertexmultiplier.ButtonStepSmall = 0.1F;
			this.autocurveoutervertexmultiplier.ButtonStepsUseModifierKeys = false;
			this.autocurveoutervertexmultiplier.ButtonStepsWrapAround = false;
			this.autocurveoutervertexmultiplier.Location = new System.Drawing.Point(128, 72);
			this.autocurveoutervertexmultiplier.Name = "autocurveoutervertexmultiplier";
			this.autocurveoutervertexmultiplier.Size = new System.Drawing.Size(56, 24);
			this.autocurveoutervertexmultiplier.StepValues = null;
			this.autocurveoutervertexmultiplier.TabIndex = 9;
			this.autocurveoutervertexmultiplier.WhenTextChanged += new System.EventHandler(this.autocurveoutervertexmultiplier_WhenTextChanged);
			// 
			// autocurveinnervertexmultiplier
			// 
			this.autocurveinnervertexmultiplier.AllowDecimal = false;
			this.autocurveinnervertexmultiplier.AllowExpressions = false;
			this.autocurveinnervertexmultiplier.AllowNegative = false;
			this.autocurveinnervertexmultiplier.AllowRelative = false;
			this.autocurveinnervertexmultiplier.ButtonStep = 1;
			this.autocurveinnervertexmultiplier.ButtonStepBig = 10F;
			this.autocurveinnervertexmultiplier.ButtonStepFloat = 1F;
			this.autocurveinnervertexmultiplier.ButtonStepSmall = 0.1F;
			this.autocurveinnervertexmultiplier.ButtonStepsUseModifierKeys = false;
			this.autocurveinnervertexmultiplier.ButtonStepsWrapAround = false;
			this.autocurveinnervertexmultiplier.Location = new System.Drawing.Point(128, 45);
			this.autocurveinnervertexmultiplier.Name = "autocurveinnervertexmultiplier";
			this.autocurveinnervertexmultiplier.Size = new System.Drawing.Size(56, 24);
			this.autocurveinnervertexmultiplier.StepValues = null;
			this.autocurveinnervertexmultiplier.TabIndex = 8;
			this.autocurveinnervertexmultiplier.WhenTextChanged += new System.EventHandler(this.autocurveinnervertexmultiplier_WhenTextChanged);
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(6, 77);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(112, 14);
			this.label6.TabIndex = 7;
			this.label6.Text = "Outer vertex multiplier";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(6, 50);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(109, 14);
			this.label5.TabIndex = 6;
			this.label5.Text = "Inner vertex multiplier";
			// 
			// tabPage3
			// 
			this.tabPage3.Controls.Add(this.splineflipping);
			this.tabPage3.Controls.Add(this.label13);
			this.tabPage3.Controls.Add(this.splineoutervertexmultiplier);
			this.tabPage3.Controls.Add(this.splineinnervertexmultiplier);
			this.tabPage3.Controls.Add(this.label16);
			this.tabPage3.Controls.Add(this.label17);
			this.tabPage3.Controls.Add(this.numberofcontrolpoints);
			this.tabPage3.Controls.Add(this.label10);
			this.tabPage3.Location = new System.Drawing.Point(4, 23);
			this.tabPage3.Name = "tabPage3";
			this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage3.Size = new System.Drawing.Size(212, 244);
			this.tabPage3.TabIndex = 2;
			this.tabPage3.Text = "Catmull Rom";
			this.tabPage3.UseVisualStyleBackColor = true;
			// 
			// splineflipping
			// 
			this.splineflipping.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.splineflipping.FormattingEnabled = true;
			this.splineflipping.Items.AddRange(new object[] {
            "None",
            "Source direction",
            "Destination direction"});
			this.splineflipping.Location = new System.Drawing.Point(55, 9);
			this.splineflipping.Name = "splineflipping";
			this.splineflipping.Size = new System.Drawing.Size(151, 22);
			this.splineflipping.TabIndex = 13;
			this.splineflipping.SelectedIndexChanged += new System.EventHandler(this.splineflipping_SelectedIndexChanged);
			// 
			// label13
			// 
			this.label13.AutoSize = true;
			this.label13.Location = new System.Drawing.Point(6, 12);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(43, 14);
			this.label13.TabIndex = 12;
			this.label13.Text = "Flipping";
			// 
			// splineoutervertexmultiplier
			// 
			this.splineoutervertexmultiplier.AllowDecimal = false;
			this.splineoutervertexmultiplier.AllowExpressions = false;
			this.splineoutervertexmultiplier.AllowNegative = false;
			this.splineoutervertexmultiplier.AllowRelative = false;
			this.splineoutervertexmultiplier.ButtonStep = 1;
			this.splineoutervertexmultiplier.ButtonStepBig = 10F;
			this.splineoutervertexmultiplier.ButtonStepFloat = 1F;
			this.splineoutervertexmultiplier.ButtonStepSmall = 0.1F;
			this.splineoutervertexmultiplier.ButtonStepsUseModifierKeys = false;
			this.splineoutervertexmultiplier.ButtonStepsWrapAround = false;
			this.splineoutervertexmultiplier.Location = new System.Drawing.Point(128, 72);
			this.splineoutervertexmultiplier.Name = "splineoutervertexmultiplier";
			this.splineoutervertexmultiplier.Size = new System.Drawing.Size(56, 24);
			this.splineoutervertexmultiplier.StepValues = null;
			this.splineoutervertexmultiplier.TabIndex = 9;
			this.splineoutervertexmultiplier.WhenTextChanged += new System.EventHandler(this.splineoutervertexmultiplier_WhenTextChanged);
			// 
			// splineinnervertexmultiplier
			// 
			this.splineinnervertexmultiplier.AllowDecimal = false;
			this.splineinnervertexmultiplier.AllowExpressions = false;
			this.splineinnervertexmultiplier.AllowNegative = false;
			this.splineinnervertexmultiplier.AllowRelative = false;
			this.splineinnervertexmultiplier.ButtonStep = 1;
			this.splineinnervertexmultiplier.ButtonStepBig = 10F;
			this.splineinnervertexmultiplier.ButtonStepFloat = 1F;
			this.splineinnervertexmultiplier.ButtonStepSmall = 0.1F;
			this.splineinnervertexmultiplier.ButtonStepsUseModifierKeys = false;
			this.splineinnervertexmultiplier.ButtonStepsWrapAround = false;
			this.splineinnervertexmultiplier.Location = new System.Drawing.Point(128, 45);
			this.splineinnervertexmultiplier.Name = "splineinnervertexmultiplier";
			this.splineinnervertexmultiplier.Size = new System.Drawing.Size(56, 24);
			this.splineinnervertexmultiplier.StepValues = null;
			this.splineinnervertexmultiplier.TabIndex = 8;
			this.splineinnervertexmultiplier.WhenTextChanged += new System.EventHandler(this.splineinnervertexmultiplier_WhenTextChanged);
			// 
			// label16
			// 
			this.label16.AutoSize = true;
			this.label16.Location = new System.Drawing.Point(6, 77);
			this.label16.Name = "label16";
			this.label16.Size = new System.Drawing.Size(112, 14);
			this.label16.TabIndex = 7;
			this.label16.Text = "Outer vertex multiplier";
			// 
			// label17
			// 
			this.label17.AutoSize = true;
			this.label17.Location = new System.Drawing.Point(6, 50);
			this.label17.Name = "label17";
			this.label17.Size = new System.Drawing.Size(109, 14);
			this.label17.TabIndex = 6;
			this.label17.Text = "Inner vertex multiplier";
			// 
			// numberofcontrolpoints
			// 
			this.numberofcontrolpoints.AllowDecimal = false;
			this.numberofcontrolpoints.AllowExpressions = false;
			this.numberofcontrolpoints.AllowNegative = false;
			this.numberofcontrolpoints.AllowRelative = false;
			this.numberofcontrolpoints.ButtonStep = 1;
			this.numberofcontrolpoints.ButtonStepBig = 10F;
			this.numberofcontrolpoints.ButtonStepFloat = 1F;
			this.numberofcontrolpoints.ButtonStepSmall = 0.1F;
			this.numberofcontrolpoints.ButtonStepsUseModifierKeys = false;
			this.numberofcontrolpoints.ButtonStepsWrapAround = false;
			this.numberofcontrolpoints.Location = new System.Drawing.Point(128, 99);
			this.numberofcontrolpoints.Name = "numberofcontrolpoints";
			this.numberofcontrolpoints.Size = new System.Drawing.Size(56, 24);
			this.numberofcontrolpoints.StepValues = null;
			this.numberofcontrolpoints.TabIndex = 3;
			this.numberofcontrolpoints.WhenTextChanged += new System.EventHandler(this.numberofcontrolpoints_WhenTextChanged);
			// 
			// label10
			// 
			this.label10.AutoSize = true;
			this.label10.Location = new System.Drawing.Point(6, 104);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(73, 14);
			this.label10.TabIndex = 2;
			this.label10.Text = "Control points";
			// 
			// groupBox4
			// 
			this.groupBox4.Controls.Add(this.floorflattexture);
			this.groupBox4.Controls.Add(this.floorflat);
			this.groupBox4.Location = new System.Drawing.Point(12, 343);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Size = new System.Drawing.Size(105, 183);
			this.groupBox4.TabIndex = 11;
			this.groupBox4.TabStop = false;
			// 
			// floorflattexture
			// 
			this.floorflattexture.Enabled = false;
			this.floorflattexture.Location = new System.Drawing.Point(8, 23);
			this.floorflattexture.MultipleTextures = false;
			this.floorflattexture.Name = "floorflattexture";
			this.floorflattexture.Size = new System.Drawing.Size(90, 127);
			this.floorflattexture.TabIndex = 1;
			this.floorflattexture.TextureName = "";
			// 
			// floorflat
			// 
			this.floorflat.AutoSize = true;
			this.floorflat.Location = new System.Drawing.Point(9, -1);
			this.floorflat.Name = "floorflat";
			this.floorflat.Size = new System.Drawing.Size(68, 18);
			this.floorflat.TabIndex = 0;
			this.floorflat.Text = "Floor flat";
			this.floorflat.UseVisualStyleBackColor = true;
			this.floorflat.CheckedChanged += new System.EventHandler(this.floorflat_CheckedChanged);
			// 
			// ceilingflat
			// 
			this.ceilingflat.AutoSize = true;
			this.ceilingflat.Location = new System.Drawing.Point(9, -1);
			this.ceilingflat.Name = "ceilingflat";
			this.ceilingflat.Size = new System.Drawing.Size(75, 18);
			this.ceilingflat.TabIndex = 0;
			this.ceilingflat.Text = "Ceiling flat";
			this.ceilingflat.UseVisualStyleBackColor = true;
			this.ceilingflat.CheckedChanged += new System.EventHandler(this.ceilingflat_CheckedChanged);
			// 
			// groupBox5
			// 
			this.groupBox5.Controls.Add(this.ceilingflattexture);
			this.groupBox5.Controls.Add(this.ceilingflat);
			this.groupBox5.Location = new System.Drawing.Point(123, 343);
			this.groupBox5.Name = "groupBox5";
			this.groupBox5.Size = new System.Drawing.Size(105, 183);
			this.groupBox5.TabIndex = 12;
			this.groupBox5.TabStop = false;
			// 
			// ceilingflattexture
			// 
			this.ceilingflattexture.Enabled = false;
			this.ceilingflattexture.Location = new System.Drawing.Point(8, 23);
			this.ceilingflattexture.MultipleTextures = false;
			this.ceilingflattexture.Name = "ceilingflattexture";
			this.ceilingflattexture.Size = new System.Drawing.Size(90, 127);
			this.ceilingflattexture.TabIndex = 2;
			this.ceilingflattexture.TextureName = "";
			// 
			// groupBox6
			// 
			this.groupBox6.Controls.Add(this.label14);
			this.groupBox6.Controls.Add(this.floorbase);
			this.groupBox6.Controls.Add(this.floorheightmodification);
			this.groupBox6.Controls.Add(this.floorlast);
			this.groupBox6.Controls.Add(this.floorfirst);
			this.groupBox6.Controls.Add(this.label7);
			this.groupBox6.Controls.Add(this.label3);
			this.groupBox6.Controls.Add(this.floorheightmod);
			this.groupBox6.Controls.Add(this.label);
			this.groupBox6.Location = new System.Drawing.Point(238, 203);
			this.groupBox6.Name = "groupBox6";
			this.groupBox6.Size = new System.Drawing.Size(110, 134);
			this.groupBox6.TabIndex = 13;
			this.groupBox6.TabStop = false;
			this.groupBox6.Text = "c";
			// 
			// label14
			// 
			this.label14.AutoSize = true;
			this.label14.Location = new System.Drawing.Point(4, 53);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(32, 14);
			this.label14.TabIndex = 17;
			this.label14.Text = "Base";
			// 
			// floorbase
			// 
			this.floorbase.AllowDecimal = false;
			this.floorbase.AllowExpressions = false;
			this.floorbase.AllowNegative = true;
			this.floorbase.AllowRelative = false;
			this.floorbase.ButtonStep = 8;
			this.floorbase.ButtonStepBig = 10F;
			this.floorbase.ButtonStepFloat = 1F;
			this.floorbase.ButtonStepSmall = 0.1F;
			this.floorbase.ButtonStepsUseModifierKeys = false;
			this.floorbase.ButtonStepsWrapAround = false;
			this.floorbase.Location = new System.Drawing.Point(45, 48);
			this.floorbase.Name = "floorbase";
			this.floorbase.Size = new System.Drawing.Size(56, 24);
			this.floorbase.StepValues = null;
			this.floorbase.TabIndex = 16;
			this.floorbase.WhenTextChanged += new System.EventHandler(this.floorbase_WhenTextChanged);
			// 
			// floorheightmodification
			// 
			this.floorheightmodification.AutoSize = true;
			this.floorheightmodification.Checked = true;
			this.floorheightmodification.CheckState = System.Windows.Forms.CheckState.Checked;
			this.floorheightmodification.Location = new System.Drawing.Point(9, -1);
			this.floorheightmodification.Name = "floorheightmodification";
			this.floorheightmodification.Size = new System.Drawing.Size(82, 18);
			this.floorheightmodification.TabIndex = 14;
			this.floorheightmodification.Text = "Floor height";
			this.floorheightmodification.UseVisualStyleBackColor = true;
			this.floorheightmodification.CheckedChanged += new System.EventHandler(this.floorheightmodification_CheckedChanged);
			// 
			// floorlast
			// 
			this.floorlast.AutoSize = true;
			this.floorlast.Location = new System.Drawing.Point(41, 108);
			this.floorlast.Name = "floorlast";
			this.floorlast.Size = new System.Drawing.Size(46, 14);
			this.floorlast.TabIndex = 11;
			this.floorlast.Text = "floorlast";
			// 
			// floorfirst
			// 
			this.floorfirst.AutoSize = true;
			this.floorfirst.Location = new System.Drawing.Point(41, 81);
			this.floorfirst.Name = "floorfirst";
			this.floorfirst.Size = new System.Drawing.Size(48, 14);
			this.floorfirst.TabIndex = 10;
			this.floorfirst.Text = "floorfirst";
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(4, 108);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(31, 14);
			this.label7.TabIndex = 7;
			this.label7.Text = "Last:";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(4, 81);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(31, 14);
			this.label3.TabIndex = 6;
			this.label3.Text = "First:";
			// 
			// floorheightmod
			// 
			this.floorheightmod.AllowDecimal = false;
			this.floorheightmod.AllowExpressions = false;
			this.floorheightmod.AllowNegative = true;
			this.floorheightmod.AllowRelative = false;
			this.floorheightmod.ButtonStep = 8;
			this.floorheightmod.ButtonStepBig = 10F;
			this.floorheightmod.ButtonStepFloat = 1F;
			this.floorheightmod.ButtonStepSmall = 0.1F;
			this.floorheightmod.ButtonStepsUseModifierKeys = false;
			this.floorheightmod.ButtonStepsWrapAround = false;
			this.floorheightmod.Location = new System.Drawing.Point(45, 22);
			this.floorheightmod.Name = "floorheightmod";
			this.floorheightmod.Size = new System.Drawing.Size(56, 24);
			this.floorheightmod.StepValues = null;
			this.floorheightmod.TabIndex = 3;
			this.floorheightmod.WhenTextChanged += new System.EventHandler(this.floorheightmod_WhenTextChanged);
			// 
			// label
			// 
			this.label.AutoSize = true;
			this.label.Location = new System.Drawing.Point(4, 27);
			this.label.Name = "label";
			this.label.Size = new System.Drawing.Size(39, 14);
			this.label.TabIndex = 2;
			this.label.Text = "Modify";
			// 
			// groupBox7
			// 
			this.groupBox7.Controls.Add(this.lowerunpegged);
			this.groupBox7.Controls.Add(this.lowertexturetexture);
			this.groupBox7.Controls.Add(this.lowertexture);
			this.groupBox7.Location = new System.Drawing.Point(456, 342);
			this.groupBox7.Name = "groupBox7";
			this.groupBox7.Size = new System.Drawing.Size(105, 183);
			this.groupBox7.TabIndex = 14;
			this.groupBox7.TabStop = false;
			// 
			// lowerunpegged
			// 
			this.lowerunpegged.AutoSize = true;
			this.lowerunpegged.Checked = true;
			this.lowerunpegged.CheckState = System.Windows.Forms.CheckState.Checked;
			this.lowerunpegged.Location = new System.Drawing.Point(9, 156);
			this.lowerunpegged.Name = "lowerunpegged";
			this.lowerunpegged.Size = new System.Drawing.Size(75, 18);
			this.lowerunpegged.TabIndex = 3;
			this.lowerunpegged.Text = "Unpegged";
			this.lowerunpegged.UseVisualStyleBackColor = true;
			// 
			// lowertexturetexture
			// 
			this.lowertexturetexture.Location = new System.Drawing.Point(8, 23);
			this.lowertexturetexture.MultipleTextures = false;
			this.lowertexturetexture.Name = "lowertexturetexture";
			this.lowertexturetexture.Required = false;
			this.lowertexturetexture.Size = new System.Drawing.Size(90, 127);
			this.lowertexturetexture.TabIndex = 2;
			this.lowertexturetexture.TextureName = "";
			// 
			// lowertexture
			// 
			this.lowertexture.AutoSize = true;
			this.lowertexture.Checked = true;
			this.lowertexture.CheckState = System.Windows.Forms.CheckState.Checked;
			this.lowertexture.Location = new System.Drawing.Point(9, -1);
			this.lowertexture.Name = "lowertexture";
			this.lowertexture.Size = new System.Drawing.Size(95, 18);
			this.lowertexture.TabIndex = 0;
			this.lowertexture.Text = "Lower texture";
			this.lowertexture.UseVisualStyleBackColor = true;
			this.lowertexture.CheckedChanged += new System.EventHandler(this.lowertexture_CheckedChanged);
			// 
			// groupBox8
			// 
			this.groupBox8.Controls.Add(this.upperunpegged);
			this.groupBox8.Controls.Add(this.uppertexturetexture);
			this.groupBox8.Controls.Add(this.uppertexture);
			this.groupBox8.Location = new System.Drawing.Point(234, 343);
			this.groupBox8.Name = "groupBox8";
			this.groupBox8.Size = new System.Drawing.Size(105, 183);
			this.groupBox8.TabIndex = 13;
			this.groupBox8.TabStop = false;
			// 
			// upperunpegged
			// 
			this.upperunpegged.AutoSize = true;
			this.upperunpegged.Checked = true;
			this.upperunpegged.CheckState = System.Windows.Forms.CheckState.Checked;
			this.upperunpegged.Location = new System.Drawing.Point(9, 156);
			this.upperunpegged.Name = "upperunpegged";
			this.upperunpegged.Size = new System.Drawing.Size(75, 18);
			this.upperunpegged.TabIndex = 4;
			this.upperunpegged.Text = "Unpegged";
			this.upperunpegged.UseVisualStyleBackColor = true;
			// 
			// uppertexturetexture
			// 
			this.uppertexturetexture.Location = new System.Drawing.Point(8, 23);
			this.uppertexturetexture.MultipleTextures = false;
			this.uppertexturetexture.Name = "uppertexturetexture";
			this.uppertexturetexture.Required = false;
			this.uppertexturetexture.Size = new System.Drawing.Size(90, 127);
			this.uppertexturetexture.TabIndex = 1;
			this.uppertexturetexture.TextureName = "";
			// 
			// uppertexture
			// 
			this.uppertexture.AutoSize = true;
			this.uppertexture.Checked = true;
			this.uppertexture.CheckState = System.Windows.Forms.CheckState.Checked;
			this.uppertexture.Location = new System.Drawing.Point(9, -1);
			this.uppertexture.Name = "uppertexture";
			this.uppertexture.Size = new System.Drawing.Size(92, 18);
			this.uppertexture.TabIndex = 0;
			this.uppertexture.Text = "Upper texture";
			this.uppertexture.UseVisualStyleBackColor = true;
			this.uppertexture.CheckedChanged += new System.EventHandler(this.uppertexture_CheckedChanged);
			// 
			// ceilinglast
			// 
			this.ceilinglast.AutoSize = true;
			this.ceilinglast.Location = new System.Drawing.Point(41, 108);
			this.ceilinglast.Name = "ceilinglast";
			this.ceilinglast.Size = new System.Drawing.Size(54, 14);
			this.ceilinglast.TabIndex = 24;
			this.ceilinglast.Text = "ceilinglast";
			// 
			// ceilingfirst
			// 
			this.ceilingfirst.AutoSize = true;
			this.ceilingfirst.Location = new System.Drawing.Point(41, 81);
			this.ceilingfirst.Name = "ceilingfirst";
			this.ceilingfirst.Size = new System.Drawing.Size(56, 14);
			this.ceilingfirst.TabIndex = 23;
			this.ceilingfirst.Text = "ceilingfirst";
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(4, 108);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(31, 14);
			this.label8.TabIndex = 22;
			this.label8.Text = "Last:";
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point(4, 81);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(31, 14);
			this.label9.TabIndex = 21;
			this.label9.Text = "First:";
			// 
			// ceilingheightmod
			// 
			this.ceilingheightmod.AllowDecimal = false;
			this.ceilingheightmod.AllowExpressions = false;
			this.ceilingheightmod.AllowNegative = true;
			this.ceilingheightmod.AllowRelative = false;
			this.ceilingheightmod.ButtonStep = 8;
			this.ceilingheightmod.ButtonStepBig = 10F;
			this.ceilingheightmod.ButtonStepFloat = 1F;
			this.ceilingheightmod.ButtonStepSmall = 0.1F;
			this.ceilingheightmod.ButtonStepsUseModifierKeys = false;
			this.ceilingheightmod.ButtonStepsWrapAround = false;
			this.ceilingheightmod.Location = new System.Drawing.Point(45, 22);
			this.ceilingheightmod.Name = "ceilingheightmod";
			this.ceilingheightmod.Size = new System.Drawing.Size(56, 24);
			this.ceilingheightmod.StepValues = null;
			this.ceilingheightmod.TabIndex = 20;
			this.ceilingheightmod.WhenTextChanged += new System.EventHandler(this.ceilingheightmod_WhenTextChanged);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(4, 27);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(39, 14);
			this.label2.TabIndex = 19;
			this.label2.Text = "Modify";
			// 
			// groupBox9
			// 
			this.groupBox9.Controls.Add(this.label15);
			this.groupBox9.Controls.Add(this.ceilingbase);
			this.groupBox9.Controls.Add(this.ceilingheightmodification);
			this.groupBox9.Controls.Add(this.ceilingheightmod);
			this.groupBox9.Controls.Add(this.label2);
			this.groupBox9.Controls.Add(this.ceilinglast);
			this.groupBox9.Controls.Add(this.label9);
			this.groupBox9.Controls.Add(this.ceilingfirst);
			this.groupBox9.Controls.Add(this.label8);
			this.groupBox9.Location = new System.Drawing.Point(354, 203);
			this.groupBox9.Name = "groupBox9";
			this.groupBox9.Size = new System.Drawing.Size(110, 134);
			this.groupBox9.TabIndex = 18;
			this.groupBox9.TabStop = false;
			this.groupBox9.Text = "c";
			// 
			// label15
			// 
			this.label15.AutoSize = true;
			this.label15.Location = new System.Drawing.Point(4, 53);
			this.label15.Name = "label15";
			this.label15.Size = new System.Drawing.Size(32, 14);
			this.label15.TabIndex = 27;
			this.label15.Text = "Base";
			// 
			// ceilingbase
			// 
			this.ceilingbase.AllowDecimal = false;
			this.ceilingbase.AllowExpressions = false;
			this.ceilingbase.AllowNegative = true;
			this.ceilingbase.AllowRelative = false;
			this.ceilingbase.ButtonStep = 8;
			this.ceilingbase.ButtonStepBig = 10F;
			this.ceilingbase.ButtonStepFloat = 1F;
			this.ceilingbase.ButtonStepSmall = 0.1F;
			this.ceilingbase.ButtonStepsUseModifierKeys = false;
			this.ceilingbase.ButtonStepsWrapAround = false;
			this.ceilingbase.Location = new System.Drawing.Point(45, 48);
			this.ceilingbase.Name = "ceilingbase";
			this.ceilingbase.Size = new System.Drawing.Size(56, 24);
			this.ceilingbase.StepValues = null;
			this.ceilingbase.TabIndex = 26;
			this.ceilingbase.WhenTextChanged += new System.EventHandler(this.ceilingbase_WhenTextChanged);
			// 
			// ceilingheightmodification
			// 
			this.ceilingheightmodification.AutoSize = true;
			this.ceilingheightmodification.Checked = true;
			this.ceilingheightmodification.CheckState = System.Windows.Forms.CheckState.Checked;
			this.ceilingheightmodification.Location = new System.Drawing.Point(9, -1);
			this.ceilingheightmodification.Name = "ceilingheightmodification";
			this.ceilingheightmodification.Size = new System.Drawing.Size(89, 18);
			this.ceilingheightmodification.TabIndex = 14;
			this.ceilingheightmodification.Text = "Ceiling height";
			this.ceilingheightmodification.UseVisualStyleBackColor = true;
			this.ceilingheightmodification.CheckedChanged += new System.EventHandler(this.ceilingheightmodification_CheckedChanged);
			// 
			// groupBox11
			// 
			this.groupBox11.Controls.Add(this.prefabdefault);
			this.groupBox11.Controls.Add(this.prefabload);
			this.groupBox11.Controls.Add(this.prefabdelete);
			this.groupBox11.Controls.Add(this.prefabsave);
			this.groupBox11.Controls.Add(this.prefabname);
			this.groupBox11.Controls.Add(this.prefabs);
			this.groupBox11.Location = new System.Drawing.Point(238, 9);
			this.groupBox11.Name = "groupBox11";
			this.groupBox11.Size = new System.Drawing.Size(323, 187);
			this.groupBox11.TabIndex = 24;
			this.groupBox11.TabStop = false;
			this.groupBox11.Text = "Prefabs";
			// 
			// prefabload
			// 
			this.prefabload.Location = new System.Drawing.Point(80, 156);
			this.prefabload.Name = "prefabload";
			this.prefabload.Size = new System.Drawing.Size(65, 23);
			this.prefabload.TabIndex = 28;
			this.prefabload.Text = "Load";
			this.prefabload.UseVisualStyleBackColor = true;
			this.prefabload.Click += new System.EventHandler(this.prefabload_Click);
			// 
			// prefabdelete
			// 
			this.prefabdelete.Location = new System.Drawing.Point(151, 156);
			this.prefabdelete.Name = "prefabdelete";
			this.prefabdelete.Size = new System.Drawing.Size(65, 23);
			this.prefabdelete.TabIndex = 27;
			this.prefabdelete.Text = "Delete";
			this.prefabdelete.UseVisualStyleBackColor = true;
			this.prefabdelete.Click += new System.EventHandler(this.prefabdelete_Click);
			// 
			// prefabsave
			// 
			this.prefabsave.Location = new System.Drawing.Point(9, 156);
			this.prefabsave.Name = "prefabsave";
			this.prefabsave.Size = new System.Drawing.Size(65, 23);
			this.prefabsave.TabIndex = 26;
			this.prefabsave.Text = "Save";
			this.prefabsave.UseVisualStyleBackColor = true;
			this.prefabsave.Click += new System.EventHandler(this.prefabsave_Click);
			// 
			// prefabname
			// 
			this.prefabname.Location = new System.Drawing.Point(10, 19);
			this.prefabname.Name = "prefabname";
			this.prefabname.Size = new System.Drawing.Size(306, 20);
			this.prefabname.TabIndex = 25;
			// 
			// prefabs
			// 
			this.prefabs.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
			this.prefabs.FullRowSelect = true;
			this.prefabs.Location = new System.Drawing.Point(9, 41);
			this.prefabs.MultiSelect = false;
			this.prefabs.Name = "prefabs";
			this.prefabs.Size = new System.Drawing.Size(307, 109);
			this.prefabs.TabIndex = 24;
			this.prefabs.UseCompatibleStateImageBehavior = false;
			this.prefabs.View = System.Windows.Forms.View.Details;
			this.prefabs.DoubleClick += new System.EventHandler(this.prefabs_DoubleClick);
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "Name";
			this.columnHeader1.Width = 200;
			// 
			// columnHeader2
			// 
			this.columnHeader2.Text = "Type";
			this.columnHeader2.Width = 80;
			// 
			// groupBox10
			// 
			this.groupBox10.Controls.Add(this.middletexturetexture);
			this.groupBox10.Controls.Add(this.middletexture);
			this.groupBox10.Location = new System.Drawing.Point(345, 343);
			this.groupBox10.Name = "groupBox10";
			this.groupBox10.Size = new System.Drawing.Size(105, 183);
			this.groupBox10.TabIndex = 25;
			this.groupBox10.TabStop = false;
			// 
			// middletexturetexture
			// 
			this.middletexturetexture.Enabled = false;
			this.middletexturetexture.Location = new System.Drawing.Point(8, 23);
			this.middletexturetexture.MultipleTextures = false;
			this.middletexturetexture.Name = "middletexturetexture";
			this.middletexturetexture.Required = false;
			this.middletexturetexture.Size = new System.Drawing.Size(90, 127);
			this.middletexturetexture.TabIndex = 2;
			this.middletexturetexture.TextureName = "";
			// 
			// middletexture
			// 
			this.middletexture.AutoSize = true;
			this.middletexture.Location = new System.Drawing.Point(9, -1);
			this.middletexture.Name = "middletexture";
			this.middletexture.Size = new System.Drawing.Size(93, 18);
			this.middletexture.TabIndex = 0;
			this.middletexture.Text = "Middle texture";
			this.middletexture.UseVisualStyleBackColor = true;
			this.middletexture.CheckedChanged += new System.EventHandler(this.middletexture_CheckedChanged);
			// 
			// prefabdefault
			// 
			this.prefabdefault.Location = new System.Drawing.Point(247, 156);
			this.prefabdefault.Name = "prefabdefault";
			this.prefabdefault.Size = new System.Drawing.Size(70, 23);
			this.prefabdefault.TabIndex = 29;
			this.prefabdefault.Text = "Set default";
			this.prefabdefault.UseVisualStyleBackColor = true;
			this.prefabdefault.Click += new System.EventHandler(this.prefabdefault_Click);
			// 
			// StairSectorBuilderForm
			// 
			this.AcceptButton = this.btnOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(573, 570);
			this.Controls.Add(this.groupBox10);
			this.Controls.Add(this.groupBox11);
			this.Controls.Add(this.groupBox9);
			this.Controls.Add(this.groupBox7);
			this.Controls.Add(this.groupBox8);
			this.Controls.Add(this.groupBox6);
			this.Controls.Add(this.groupBox5);
			this.Controls.Add(this.groupBox4);
			this.Controls.Add(this.tabcontrol);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "StairSectorBuilderForm";
			this.Opacity = 0D;
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "Stair Sector Builder";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.StairSectorBuilderForm_FormClosing);
			this.Load += new System.EventHandler(this.StairSectorBuilderForm_Load);
			this.Shown += new System.EventHandler(this.StairSectorBuilderForm_Shown);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.tabcontrol.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.tabPage1.PerformLayout();
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.tabPage2.ResumeLayout(false);
			this.tabPage2.PerformLayout();
			this.tabPage3.ResumeLayout(false);
			this.tabPage3.PerformLayout();
			this.groupBox4.ResumeLayout(false);
			this.groupBox4.PerformLayout();
			this.groupBox5.ResumeLayout(false);
			this.groupBox5.PerformLayout();
			this.groupBox6.ResumeLayout(false);
			this.groupBox6.PerformLayout();
			this.groupBox7.ResumeLayout(false);
			this.groupBox7.PerformLayout();
			this.groupBox8.ResumeLayout(false);
			this.groupBox8.PerformLayout();
			this.groupBox9.ResumeLayout(false);
			this.groupBox9.PerformLayout();
			this.groupBox11.ResumeLayout(false);
			this.groupBox11.PerformLayout();
			this.groupBox10.ResumeLayout(false);
			this.groupBox10.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.GroupBox groupBox1;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox numberofsectors;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TabControl tabcontrol;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.TabPage tabPage2;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox sectordepth;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.RadioButton sideback;
		private System.Windows.Forms.RadioButton sidefront;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.CheckBox singledirection;
		private System.Windows.Forms.CheckBox singlesteps;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.CheckBox floorflat;
		private CodeImp.DoomBuilder.Controls.FlatSelectorControl floorflattexture;
		private System.Windows.Forms.GroupBox groupBox5;
		private System.Windows.Forms.CheckBox ceilingflat;
		private System.Windows.Forms.TabPage tabPage3;
		private System.Windows.Forms.GroupBox groupBox6;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox floorheightmod;
		private System.Windows.Forms.Label label;
		private CodeImp.DoomBuilder.Controls.FlatSelectorControl ceilingflattexture;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label floorlast;
		private System.Windows.Forms.Label floorfirst;
		private System.Windows.Forms.Label label7;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox numberofcontrolpoints;
		private System.Windows.Forms.Label label10;
        private System.Windows.Forms.CheckBox floorheightmodification;
        private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox spacing;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.CheckBox lowertexture;
        private System.Windows.Forms.GroupBox groupBox8;
        private System.Windows.Forms.CheckBox uppertexture;
        private CodeImp.DoomBuilder.Controls.TextureSelectorControl lowertexturetexture;
		private CodeImp.DoomBuilder.Controls.TextureSelectorControl uppertexturetexture;
		private System.Windows.Forms.Label ceilinglast;
		private System.Windows.Forms.Label ceilingfirst;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label9;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox ceilingheightmod;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.GroupBox groupBox9;
		private System.Windows.Forms.CheckBox ceilingheightmodification;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox floorbase;
        private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox ceilingbase;
        private System.Windows.Forms.GroupBox groupBox11;
        private System.Windows.Forms.Button prefabload;
        private System.Windows.Forms.Button prefabdelete;
        private System.Windows.Forms.Button prefabsave;
        private System.Windows.Forms.TextBox prefabname;
        private System.Windows.Forms.ListView prefabs;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox autocurveoutervertexmultiplier;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox autocurveinnervertexmultiplier;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label5;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox splineoutervertexmultiplier;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox splineinnervertexmultiplier;
		private System.Windows.Forms.Label label16;
		private System.Windows.Forms.Label label17;
		private System.Windows.Forms.CheckBox distinctbaseheights;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.ComboBox autocurveflipping;
		private System.Windows.Forms.ComboBox splineflipping;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.Label label14;
		private System.Windows.Forms.Label label15;
		private System.Windows.Forms.CheckBox lowerunpegged;
		private System.Windows.Forms.CheckBox upperunpegged;
		private System.Windows.Forms.GroupBox groupBox10;
		private CodeImp.DoomBuilder.Controls.TextureSelectorControl middletexturetexture;
		private System.Windows.Forms.CheckBox middletexture;
		private System.Windows.Forms.CheckBox distinctsectors;
		private System.Windows.Forms.Button prefabdefault;
	}
}
