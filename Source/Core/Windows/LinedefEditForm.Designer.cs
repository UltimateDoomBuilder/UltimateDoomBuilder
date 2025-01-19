namespace CodeImp.DoomBuilder.Windows
{
	partial class LinedefEditForm
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
			System.Windows.Forms.Label label11;
			System.Windows.Forms.Label label5;
			System.Windows.Forms.Label label4;
			System.Windows.Forms.Label label3;
			System.Windows.Forms.Label label12;
			System.Windows.Forms.Label label8;
			System.Windows.Forms.Label label9;
			System.Windows.Forms.Label label10;
			System.Windows.Forms.Label activationlabel;
			System.Windows.Forms.Label label2;
			this.tooltip = new System.Windows.Forms.ToolTip(this.components);
			this.browseaction = new System.Windows.Forms.Button();
			this.apply = new System.Windows.Forms.Button();
			this.cancel = new System.Windows.Forms.Button();
			this.panel = new System.Windows.Forms.Panel();
			this.backside = new System.Windows.Forms.CheckBox();
			this.frontside = new System.Windows.Forms.CheckBox();
			this.frontgroup = new System.Windows.Forms.GroupBox();
			this.labelFrontTextureOffset = new System.Windows.Forms.Label();
			this.labelFrontTextureOffset.AutoSize = true;
			this.frontsector = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.frontlow = new CodeImp.DoomBuilder.Controls.TextureSelectorControl();
			this.frontmid = new CodeImp.DoomBuilder.Controls.TextureSelectorControl();
			this.fronthigh = new CodeImp.DoomBuilder.Controls.TextureSelectorControl();
			this.frontTextureOffset = new CodeImp.DoomBuilder.Controls.PairedIntControl();
			this.backgroup = new System.Windows.Forms.GroupBox();
			this.labelBackTextureOffset = new System.Windows.Forms.Label();
			this.labelBackTextureOffset.AutoSize = true;
			this.backsector = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.backlow = new CodeImp.DoomBuilder.Controls.TextureSelectorControl();
			this.backmid = new CodeImp.DoomBuilder.Controls.TextureSelectorControl();
			this.backhigh = new CodeImp.DoomBuilder.Controls.TextureSelectorControl();
			this.backTextureOffset = new CodeImp.DoomBuilder.Controls.PairedIntControl();
			this.flagsgroup = new System.Windows.Forms.GroupBox();
			this.flags = new CodeImp.DoomBuilder.Controls.CheckboxArrayControl();
			this.actiongroup = new System.Windows.Forms.GroupBox();
			this.argscontrol = new CodeImp.DoomBuilder.Controls.ArgumentsControl();
			this.actionhelp = new CodeImp.DoomBuilder.Controls.ActionSpecialHelpButton();
			this.hexenpanel = new System.Windows.Forms.Panel();
			this.activation = new System.Windows.Forms.ComboBox();
			this.action = new CodeImp.DoomBuilder.Controls.ActionSelectorControl();
			this.idgroup = new System.Windows.Forms.GroupBox();
			this.tagSelector = new CodeImp.DoomBuilder.Controls.TagSelector();
			label11 = new System.Windows.Forms.Label();
			label5 = new System.Windows.Forms.Label();
			label4 = new System.Windows.Forms.Label();
			label3 = new System.Windows.Forms.Label();
			label12 = new System.Windows.Forms.Label();
			label8 = new System.Windows.Forms.Label();
			label9 = new System.Windows.Forms.Label();
			label10 = new System.Windows.Forms.Label();
			activationlabel = new System.Windows.Forms.Label();
			label2 = new System.Windows.Forms.Label();
			this.panel.SuspendLayout();
			this.frontgroup.SuspendLayout();
			this.backgroup.SuspendLayout();
			this.flagsgroup.SuspendLayout();
			this.actiongroup.SuspendLayout();
			this.hexenpanel.SuspendLayout();
			this.idgroup.SuspendLayout();
			this.SuspendLayout();
			// 
			// label11
			// 
			label11.Location = new System.Drawing.Point(6, 40);
			label11.Name = "label11";
			label11.Size = new System.Drawing.Size(80, 14);
			label11.TabIndex = 13;
			label11.Text = "Sector index:";
			label11.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label5
			// 
			label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			label5.Location = new System.Drawing.Point(437, 13);
			label5.Name = "label5";
			label5.Size = new System.Drawing.Size(83, 16);
			label5.TabIndex = 5;
			label5.Text = "Lower";
			label5.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// label4
			// 
			label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			label4.Location = new System.Drawing.Point(346, 13);
			label4.Name = "label4";
			label4.Size = new System.Drawing.Size(83, 16);
			label4.TabIndex = 4;
			label4.Text = "Middle";
			label4.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// label3
			// 
			label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			label3.Location = new System.Drawing.Point(255, 13);
			label3.Name = "label3";
			label3.Size = new System.Drawing.Size(83, 16);
			label3.TabIndex = 3;
			label3.Text = "Upper";
			label3.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// label12
			// 
			label12.Location = new System.Drawing.Point(6, 40);
			label12.Name = "label12";
			label12.Size = new System.Drawing.Size(80, 14);
			label12.TabIndex = 16;
			label12.Text = "Sector index:";
			label12.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label8
			// 
			label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			label8.Location = new System.Drawing.Point(437, 13);
			label8.Name = "label8";
			label8.Size = new System.Drawing.Size(83, 16);
			label8.TabIndex = 5;
			label8.Text = "Lower";
			label8.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// label9
			// 
			label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			label9.Location = new System.Drawing.Point(346, 13);
			label9.Name = "label9";
			label9.Size = new System.Drawing.Size(83, 16);
			label9.TabIndex = 4;
			label9.Text = "Middle";
			label9.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// label10
			// 
			label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			label10.Location = new System.Drawing.Point(255, 13);
			label10.Name = "label10";
			label10.Size = new System.Drawing.Size(83, 16);
			label10.TabIndex = 3;
			label10.Text = "Upper";
			label10.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// activationlabel
			// 
			activationlabel.AutoSize = true;
			activationlabel.Location = new System.Drawing.Point(6, 11);
			activationlabel.Name = "activationlabel";
			activationlabel.Size = new System.Drawing.Size(43, 13);
			activationlabel.TabIndex = 10;
			activationlabel.Text = "Trigger:";
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.Location = new System.Drawing.Point(15, 26);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(40, 13);
			label2.TabIndex = 9;
			label2.Text = "Action:";
			// 
			// browseaction
			// 
			this.browseaction.Image = global::CodeImp.DoomBuilder.Properties.Resources.List;
			this.browseaction.Location = new System.Drawing.Point(476, 21);
			this.browseaction.Name = "browseaction";
			this.browseaction.Size = new System.Drawing.Size(28, 25);
			this.browseaction.TabIndex = 1;
			this.browseaction.Text = " ";
			this.tooltip.SetToolTip(this.browseaction, "Browse Action");
			this.browseaction.UseVisualStyleBackColor = true;
			this.browseaction.Click += new System.EventHandler(this.browseaction_Click);
			// 
			// apply
			// 
			this.apply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.apply.Location = new System.Drawing.Point(335, 708);
			this.apply.Name = "apply";
			this.apply.Size = new System.Drawing.Size(112, 25);
			this.apply.TabIndex = 1;
			this.apply.Text = "OK";
			this.apply.UseVisualStyleBackColor = true;
			this.apply.Click += new System.EventHandler(this.apply_Click);
			// 
			// cancel
			// 
			this.cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancel.Location = new System.Drawing.Point(453, 708);
			this.cancel.Name = "cancel";
			this.cancel.Size = new System.Drawing.Size(112, 25);
			this.cancel.TabIndex = 2;
			this.cancel.Text = "Cancel";
			this.cancel.UseVisualStyleBackColor = true;
			this.cancel.Click += new System.EventHandler(this.cancel_Click);
			// 
			// panel
			// 
			this.panel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.panel.BackColor = System.Drawing.SystemColors.Control;
			this.panel.Controls.Add(this.backside);
			this.panel.Controls.Add(this.frontside);
			this.panel.Controls.Add(this.frontgroup);
			this.panel.Controls.Add(this.backgroup);
			this.panel.Controls.Add(this.flagsgroup);
			this.panel.Controls.Add(this.actiongroup);
			this.panel.Controls.Add(this.idgroup);
			this.panel.Location = new System.Drawing.Point(12, 12);
			this.panel.Name = "panel";
			this.panel.Size = new System.Drawing.Size(553, 686);
			this.panel.TabIndex = 5;
			// 
			// backside
			// 
			this.backside.AutoSize = true;
			this.backside.Location = new System.Drawing.Point(17, 158);
			this.backside.Name = "backside";
			this.backside.Size = new System.Drawing.Size(73, 17);
			this.backside.TabIndex = 0;
			this.backside.Text = "Back side";
			this.backside.UseVisualStyleBackColor = true;
			this.backside.CheckStateChanged += new System.EventHandler(this.backside_CheckStateChanged);
			// 
			// frontside
			// 
			this.frontside.AutoSize = true;
			this.frontside.Location = new System.Drawing.Point(17, 4);
			this.frontside.Name = "frontside";
			this.frontside.Size = new System.Drawing.Size(72, 17);
			this.frontside.TabIndex = 0;
			this.frontside.Text = "Front side";
			this.frontside.UseVisualStyleBackColor = true;
			this.frontside.CheckStateChanged += new System.EventHandler(this.frontside_CheckStateChanged);
			// 
			// frontgroup
			// 
			this.frontgroup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.frontgroup.Controls.Add(this.labelFrontTextureOffset);
			this.frontgroup.Controls.Add(this.frontsector);
			this.frontgroup.Controls.Add(label11);
			this.frontgroup.Controls.Add(this.frontlow);
			this.frontgroup.Controls.Add(this.frontmid);
			this.frontgroup.Controls.Add(this.fronthigh);
			this.frontgroup.Controls.Add(this.frontTextureOffset);
			this.frontgroup.Controls.Add(label5);
			this.frontgroup.Controls.Add(label4);
			this.frontgroup.Controls.Add(label3);
			this.frontgroup.Enabled = false;
			this.frontgroup.Location = new System.Drawing.Point(6, 6);
			this.frontgroup.Name = "frontgroup";
			this.frontgroup.Size = new System.Drawing.Size(541, 148);
			this.frontgroup.TabIndex = 1;
			this.frontgroup.TabStop = false;
			this.frontgroup.Text = "     ";
			// 
			// labelFrontTextureOffset
			// 
			this.labelFrontTextureOffset.Location = new System.Drawing.Point(6, 70);
			this.labelFrontTextureOffset.Name = "labelFrontTextureOffset";
			this.labelFrontTextureOffset.Size = new System.Drawing.Size(80, 14);
			this.labelFrontTextureOffset.TabIndex = 42;
			this.labelFrontTextureOffset.Text = "Texture offset:";
			this.labelFrontTextureOffset.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// frontsector
			// 
			this.frontsector.AllowDecimal = false;
			this.frontsector.AllowExpressions = false;
			this.frontsector.AllowNegative = false;
			this.frontsector.AllowRelative = false;
			this.frontsector.ButtonStep = 1;
			this.frontsector.ButtonStepBig = 10F;
			this.frontsector.ButtonStepFloat = 1F;
			this.frontsector.ButtonStepSmall = 0.1F;
			this.frontsector.ButtonStepsUseModifierKeys = false;
			this.frontsector.ButtonStepsWrapAround = false;
			this.frontsector.Location = new System.Drawing.Point(90, 35);
			this.frontsector.Name = "frontsector";
			this.frontsector.Size = new System.Drawing.Size(130, 24);
			this.frontsector.StepValues = null;
			this.frontsector.TabIndex = 14;
			// 
			// frontlow
			// 
			this.frontlow.Location = new System.Drawing.Point(437, 32);
			this.frontlow.MultipleTextures = false;
			this.frontlow.Name = "frontlow";
			this.frontlow.Required = false;
			this.frontlow.Size = new System.Drawing.Size(83, 107);
			this.frontlow.TabIndex = 6;
			this.frontlow.TextureName = "";
			this.frontlow.OnValueChanged += new System.EventHandler(this.frontlow_OnValueChanged);
			// 
			// frontmid
			// 
			this.frontmid.Location = new System.Drawing.Point(346, 32);
			this.frontmid.MultipleTextures = false;
			this.frontmid.Name = "frontmid";
			this.frontmid.Required = false;
			this.frontmid.Size = new System.Drawing.Size(83, 107);
			this.frontmid.TabIndex = 5;
			this.frontmid.TextureName = "";
			this.frontmid.OnValueChanged += new System.EventHandler(this.frontmid_OnValueChanged);
			// 
			// fronthigh
			// 
			this.fronthigh.Location = new System.Drawing.Point(255, 32);
			this.fronthigh.MultipleTextures = false;
			this.fronthigh.Name = "fronthigh";
			this.fronthigh.Required = false;
			this.fronthigh.Size = new System.Drawing.Size(83, 107);
			this.fronthigh.TabIndex = 4;
			this.fronthigh.TextureName = "";
			this.fronthigh.OnValueChanged += new System.EventHandler(this.fronthigh_OnValueChanged);
			// 
			// frontTextureOffset
			// 
			this.frontTextureOffset.ButtonStep = 16;
			this.frontTextureOffset.ButtonStepBig = 32F;
			this.frontTextureOffset.ButtonStepSmall = 1F;
			this.frontTextureOffset.ButtonStepsUseModifierKeys = true;
			this.frontTextureOffset.DefaultValue = 0;
			this.frontTextureOffset.Location = new System.Drawing.Point(87, 65);
			this.frontTextureOffset.Name = "frontTextureOffset";
			this.frontTextureOffset.Size = new System.Drawing.Size(186, 26);
			this.frontTextureOffset.TabIndex = 41;
			this.frontTextureOffset.OnValuesChanged += new System.EventHandler(this.frontTextureOffset_OnValuesChanged);
			// 
			// backgroup
			// 
			this.backgroup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.backgroup.Controls.Add(this.labelBackTextureOffset);
			this.backgroup.Controls.Add(this.backsector);
			this.backgroup.Controls.Add(label12);
			this.backgroup.Controls.Add(this.backlow);
			this.backgroup.Controls.Add(this.backmid);
			this.backgroup.Controls.Add(this.backhigh);
			this.backgroup.Controls.Add(this.backTextureOffset);
			this.backgroup.Controls.Add(label8);
			this.backgroup.Controls.Add(label9);
			this.backgroup.Controls.Add(label10);
			this.backgroup.Enabled = false;
			this.backgroup.Location = new System.Drawing.Point(6, 160);
			this.backgroup.Name = "backgroup";
			this.backgroup.Size = new System.Drawing.Size(541, 148);
			this.backgroup.TabIndex = 1;
			this.backgroup.TabStop = false;
			this.backgroup.Text = "     ";
			// 
			// labelBackTextureOffset
			// 
			this.labelBackTextureOffset.Location = new System.Drawing.Point(6, 70);
			this.labelBackTextureOffset.Name = "labelBackTextureOffset";
			this.labelBackTextureOffset.Size = new System.Drawing.Size(80, 14);
			this.labelBackTextureOffset.TabIndex = 43;
			this.labelBackTextureOffset.Text = "Texture offset:";
			this.labelBackTextureOffset.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// backsector
			// 
			this.backsector.AllowDecimal = false;
			this.backsector.AllowExpressions = false;
			this.backsector.AllowNegative = false;
			this.backsector.AllowRelative = false;
			this.backsector.ButtonStep = 1;
			this.backsector.ButtonStepBig = 10F;
			this.backsector.ButtonStepFloat = 1F;
			this.backsector.ButtonStepSmall = 0.1F;
			this.backsector.ButtonStepsUseModifierKeys = false;
			this.backsector.ButtonStepsWrapAround = false;
			this.backsector.Location = new System.Drawing.Point(90, 35);
			this.backsector.Name = "backsector";
			this.backsector.Size = new System.Drawing.Size(130, 24);
			this.backsector.StepValues = null;
			this.backsector.TabIndex = 17;
			// 
			// backlow
			// 
			this.backlow.Location = new System.Drawing.Point(437, 32);
			this.backlow.MultipleTextures = false;
			this.backlow.Name = "backlow";
			this.backlow.Required = false;
			this.backlow.Size = new System.Drawing.Size(83, 107);
			this.backlow.TabIndex = 6;
			this.backlow.TextureName = "";
			this.backlow.OnValueChanged += new System.EventHandler(this.backlow_OnValueChanged);
			// 
			// backmid
			// 
			this.backmid.Location = new System.Drawing.Point(346, 32);
			this.backmid.MultipleTextures = false;
			this.backmid.Name = "backmid";
			this.backmid.Required = false;
			this.backmid.Size = new System.Drawing.Size(83, 107);
			this.backmid.TabIndex = 5;
			this.backmid.TextureName = "";
			this.backmid.OnValueChanged += new System.EventHandler(this.backmid_OnValueChanged);
			// 
			// backhigh
			// 
			this.backhigh.Location = new System.Drawing.Point(255, 32);
			this.backhigh.MultipleTextures = false;
			this.backhigh.Name = "backhigh";
			this.backhigh.Required = false;
			this.backhigh.Size = new System.Drawing.Size(83, 107);
			this.backhigh.TabIndex = 4;
			this.backhigh.TextureName = "";
			this.backhigh.OnValueChanged += new System.EventHandler(this.backhigh_OnValueChanged);
			// 
			// backTextureOffset
			// 
			this.backTextureOffset.ButtonStep = 16;
			this.backTextureOffset.ButtonStepBig = 32F;
			this.backTextureOffset.ButtonStepSmall = 1F;
			this.backTextureOffset.ButtonStepsUseModifierKeys = true;
			this.backTextureOffset.DefaultValue = 0;
			this.backTextureOffset.Location = new System.Drawing.Point(87, 65);
			this.backTextureOffset.Name = "backTextureOffset";
			this.backTextureOffset.Size = new System.Drawing.Size(186, 26);
			this.backTextureOffset.TabIndex = 42;
			this.backTextureOffset.OnValuesChanged += new System.EventHandler(this.backTextureOffset_OnValuesChanged);
			// 
			// flagsgroup
			// 
			this.flagsgroup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.flagsgroup.Controls.Add(this.flags);
			this.flagsgroup.Location = new System.Drawing.Point(6, 314);
			this.flagsgroup.Name = "flagsgroup";
			this.flagsgroup.Size = new System.Drawing.Size(541, 118);
			this.flagsgroup.TabIndex = 0;
			this.flagsgroup.TabStop = false;
			this.flagsgroup.Text = " Flags";
			// 
			// flags
			// 
			this.flags.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.flags.AutoScroll = true;
			this.flags.Columns = 3;
			this.flags.Location = new System.Drawing.Point(8, 17);
			this.flags.Name = "flags";
			this.flags.Size = new System.Drawing.Size(532, 96);
			this.flags.TabIndex = 0;
			this.flags.VerticalSpacing = 1;
			this.flags.OnValueChanged += new System.EventHandler(this.flags_OnValueChanged);
			// 
			// actiongroup
			// 
			this.actiongroup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.actiongroup.Controls.Add(this.argscontrol);
			this.actiongroup.Controls.Add(this.actionhelp);
			this.actiongroup.Controls.Add(this.hexenpanel);
			this.actiongroup.Controls.Add(label2);
			this.actiongroup.Controls.Add(this.action);
			this.actiongroup.Controls.Add(this.browseaction);
			this.actiongroup.Location = new System.Drawing.Point(6, 438);
			this.actiongroup.Name = "actiongroup";
			this.actiongroup.Size = new System.Drawing.Size(541, 176);
			this.actiongroup.TabIndex = 1;
			this.actiongroup.TabStop = false;
			this.actiongroup.Text = " Action ";
			// 
			// argscontrol
			// 
			this.argscontrol.Location = new System.Drawing.Point(9, 52);
			this.argscontrol.Name = "argscontrol";
			this.argscontrol.Size = new System.Drawing.Size(526, 76);
			this.argscontrol.TabIndex = 12;
			this.argscontrol.Visible = false;
			// 
			// actionhelp
			// 
			this.actionhelp.Location = new System.Drawing.Point(505, 21);
			this.actionhelp.Name = "actionhelp";
			this.actionhelp.Size = new System.Drawing.Size(28, 25);
			this.actionhelp.TabIndex = 11;
			// 
			// hexenpanel
			// 
			this.hexenpanel.Controls.Add(this.activation);
			this.hexenpanel.Controls.Add(activationlabel);
			this.hexenpanel.Location = new System.Drawing.Point(6, 134);
			this.hexenpanel.Name = "hexenpanel";
			this.hexenpanel.Size = new System.Drawing.Size(529, 36);
			this.hexenpanel.TabIndex = 3;
			this.hexenpanel.Visible = false;
			// 
			// activation
			// 
			this.activation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.activation.FormattingEnabled = true;
			this.activation.Location = new System.Drawing.Point(56, 7);
			this.activation.Name = "activation";
			this.activation.Size = new System.Drawing.Size(470, 21);
			this.activation.TabIndex = 0;
			// 
			// action
			// 
			this.action.BackColor = System.Drawing.Color.Transparent;
			this.action.Cursor = System.Windows.Forms.Cursors.Default;
			this.action.Empty = false;
			this.action.GeneralizedCategories = null;
			this.action.GeneralizedOptions = null;
			this.action.Location = new System.Drawing.Point(62, 23);
			this.action.Name = "action";
			this.action.Size = new System.Drawing.Size(412, 21);
			this.action.TabIndex = 0;
			this.action.Value = 402;
			this.action.ValueChanges += new System.EventHandler(this.action_ValueChanges);
			// 
			// idgroup
			// 
			this.idgroup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.idgroup.Controls.Add(this.tagSelector);
			this.idgroup.Location = new System.Drawing.Point(6, 620);
			this.idgroup.Name = "idgroup";
			this.idgroup.Size = new System.Drawing.Size(541, 58);
			this.idgroup.TabIndex = 2;
			this.idgroup.TabStop = false;
			this.idgroup.Text = " Identification ";
			// 
			// tagSelector
			// 
			this.tagSelector.Location = new System.Drawing.Point(19, 19);
			this.tagSelector.Name = "tagSelector";
			this.tagSelector.Size = new System.Drawing.Size(518, 34);
			this.tagSelector.TabIndex = 0;
			// 
			// LinedefEditForm
			// 
			this.AcceptButton = this.apply;
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.CancelButton = this.cancel;
			this.ClientSize = new System.Drawing.Size(577, 740);
			this.Controls.Add(this.cancel);
			this.Controls.Add(this.apply);
			this.Controls.Add(this.panel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "LinedefEditForm";
			this.Opacity = 0D;
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Edit Linedef";
			this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.LinedefEditForm_HelpRequested);
			this.panel.ResumeLayout(false);
			this.panel.PerformLayout();
			this.frontgroup.ResumeLayout(false);
			this.backgroup.ResumeLayout(false);
			this.flagsgroup.ResumeLayout(false);
			this.actiongroup.ResumeLayout(false);
			this.actiongroup.PerformLayout();
			this.hexenpanel.ResumeLayout(false);
			this.hexenpanel.PerformLayout();
			this.idgroup.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button cancel;
		private System.Windows.Forms.Button apply;
		private System.Windows.Forms.GroupBox actiongroup;
		private System.Windows.Forms.GroupBox flagsgroup;
		private CodeImp.DoomBuilder.Controls.CheckboxArrayControl flags;
		private System.Windows.Forms.Button browseaction;
		private CodeImp.DoomBuilder.Controls.ActionSelectorControl action;
		private System.Windows.Forms.GroupBox frontgroup;
		private System.Windows.Forms.CheckBox frontside;
		private System.Windows.Forms.CheckBox backside;
		private System.Windows.Forms.GroupBox backgroup;
		private CodeImp.DoomBuilder.Controls.TextureSelectorControl frontlow;
		private CodeImp.DoomBuilder.Controls.TextureSelectorControl frontmid;
		private CodeImp.DoomBuilder.Controls.TextureSelectorControl fronthigh;
		private CodeImp.DoomBuilder.Controls.TextureSelectorControl backlow;
		private CodeImp.DoomBuilder.Controls.TextureSelectorControl backmid;
		private CodeImp.DoomBuilder.Controls.TextureSelectorControl backhigh;
		private System.Windows.Forms.ComboBox activation;
		private System.Windows.Forms.Panel hexenpanel;
		private System.Windows.Forms.GroupBox idgroup;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox frontsector;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox backsector;
		private CodeImp.DoomBuilder.Controls.TagSelector tagSelector;
		private CodeImp.DoomBuilder.Controls.PairedIntControl frontTextureOffset;
		private CodeImp.DoomBuilder.Controls.PairedIntControl backTextureOffset;
		private System.Windows.Forms.Panel panel;
		private CodeImp.DoomBuilder.Controls.ActionSpecialHelpButton actionhelp;
		private System.Windows.Forms.ToolTip tooltip;
		private System.Windows.Forms.Label labelFrontTextureOffset;
		private System.Windows.Forms.Label labelBackTextureOffset;
		private CodeImp.DoomBuilder.Controls.ArgumentsControl argscontrol;
	}
}
