namespace CodeImp.DoomBuilder.Windows
{
	partial class LinedefEditFormUDMF
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
			System.Windows.Forms.Label label2;
			System.Windows.Forms.Label label11;
			System.Windows.Forms.Label label12;
			System.Windows.Forms.Label label6;
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LinedefEditFormUDMF));
			this.labelrenderstyle = new System.Windows.Forms.Label();
			this.labellockpick = new System.Windows.Forms.Label();
			this.labelLightFront = new System.Windows.Forms.Label();
			this.cancel = new System.Windows.Forms.Button();
			this.apply = new System.Windows.Forms.Button();
			this.actiongroup = new System.Windows.Forms.GroupBox();
			this.argscontrol = new CodeImp.DoomBuilder.Controls.ArgumentsControl();
			this.actionhelp = new CodeImp.DoomBuilder.Controls.ActionSpecialHelpButton();
			this.action = new CodeImp.DoomBuilder.Controls.ActionSelectorControl();
			this.browseaction = new System.Windows.Forms.Button();
			this.udmfactivates = new CodeImp.DoomBuilder.Controls.CheckboxArrayControl();
			this.flagsgroup = new System.Windows.Forms.GroupBox();
			this.flags = new CodeImp.DoomBuilder.Controls.CheckboxArrayControl();
			this.tabs = new System.Windows.Forms.TabControl();
			this.tabproperties = new System.Windows.Forms.TabPage();
			this.groupsettings = new System.Windows.Forms.GroupBox();
			this.resetalpha = new System.Windows.Forms.Button();
			this.lockpick = new System.Windows.Forms.ComboBox();
			this.alpha = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.renderStyle = new System.Windows.Forms.ComboBox();
			this.activationGroup = new System.Windows.Forms.GroupBox();
			this.missingactivation = new System.Windows.Forms.PictureBox();
			this.idgroup = new System.Windows.Forms.GroupBox();
			this.tagsselector = new CodeImp.DoomBuilder.Controls.TagsSelector();
			this.tabfront = new System.Windows.Forms.TabPage();
			this.frontside = new System.Windows.Forms.CheckBox();
			this.frontgroup = new System.Windows.Forms.GroupBox();
			this.frontflagsgroup = new System.Windows.Forms.GroupBox();
			this.flagsFront = new CodeImp.DoomBuilder.Controls.CheckboxArrayControl();
			this.frontscalegroup = new System.Windows.Forms.GroupBox();
			this.labelFrontScaleBottom = new System.Windows.Forms.Label();
			this.labelFrontScaleMid = new System.Windows.Forms.Label();
			this.labelFrontScaleTop = new System.Windows.Forms.Label();
			this.pfcFrontScaleTop = new CodeImp.DoomBuilder.Controls.PairedFieldsControl();
			this.pfcFrontScaleBottom = new CodeImp.DoomBuilder.Controls.PairedFieldsControl();
			this.pfcFrontScaleMid = new CodeImp.DoomBuilder.Controls.PairedFieldsControl();
			this.groupBox6 = new System.Windows.Forms.GroupBox();
			this.labelFrontTextureOffset = new System.Windows.Forms.Label();
			this.labelFrontOffsetBottom = new System.Windows.Forms.Label();
			this.frontTextureOffset = new CodeImp.DoomBuilder.Controls.PairedIntControl();
			this.labelFrontOffsetMid = new System.Windows.Forms.Label();
			this.pfcFrontOffsetTop = new CodeImp.DoomBuilder.Controls.PairedFieldsControl();
			this.labelFrontOffsetTop = new System.Windows.Forms.Label();
			this.pfcFrontOffsetMid = new CodeImp.DoomBuilder.Controls.PairedFieldsControl();
			this.pfcFrontOffsetBottom = new CodeImp.DoomBuilder.Controls.PairedFieldsControl();
			this.groupBox5 = new System.Windows.Forms.GroupBox();
			this.lightfrontlower = new CodeImp.DoomBuilder.Controls.SidedefPartLightControl();
			this.lightfrontmiddle = new CodeImp.DoomBuilder.Controls.SidedefPartLightControl();
			this.lightfrontupper = new CodeImp.DoomBuilder.Controls.SidedefPartLightControl();
			this.resetfrontlight = new System.Windows.Forms.Button();
			this.frontsector = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.customfrontbutton = new System.Windows.Forms.Button();
			this.lightFront = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.cbLightAbsoluteFront = new System.Windows.Forms.CheckBox();
			this.frontlow = new CodeImp.DoomBuilder.Controls.TextureSelectorControl();
			this.frontmid = new CodeImp.DoomBuilder.Controls.TextureSelectorControl();
			this.fronthigh = new CodeImp.DoomBuilder.Controls.TextureSelectorControl();
			this.tabback = new System.Windows.Forms.TabPage();
			this.backside = new System.Windows.Forms.CheckBox();
			this.backgroup = new System.Windows.Forms.GroupBox();
			this.groupBox4 = new System.Windows.Forms.GroupBox();
			this.lightbacklower = new CodeImp.DoomBuilder.Controls.SidedefPartLightControl();
			this.lightbackmiddle = new CodeImp.DoomBuilder.Controls.SidedefPartLightControl();
			this.lightbackupper = new CodeImp.DoomBuilder.Controls.SidedefPartLightControl();
			this.resetbacklight = new System.Windows.Forms.Button();
			this.backsector = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.custombackbutton = new System.Windows.Forms.Button();
			this.lightBack = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.labelLightBack = new System.Windows.Forms.Label();
			this.cbLightAbsoluteBack = new System.Windows.Forms.CheckBox();
			this.backflagsgroup = new System.Windows.Forms.GroupBox();
			this.flagsBack = new CodeImp.DoomBuilder.Controls.CheckboxArrayControl();
			this.backscalegroup = new System.Windows.Forms.GroupBox();
			this.labelBackScaleBottom = new System.Windows.Forms.Label();
			this.labelBackScaleMid = new System.Windows.Forms.Label();
			this.labelBackScaleTop = new System.Windows.Forms.Label();
			this.pfcBackScaleTop = new CodeImp.DoomBuilder.Controls.PairedFieldsControl();
			this.pfcBackScaleBottom = new CodeImp.DoomBuilder.Controls.PairedFieldsControl();
			this.pfcBackScaleMid = new CodeImp.DoomBuilder.Controls.PairedFieldsControl();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.labelBackTextureOffset = new System.Windows.Forms.Label();
			this.labelBackOffsetBottom = new System.Windows.Forms.Label();
			this.labelBackOffsetMid = new System.Windows.Forms.Label();
			this.labelBackOffsetTop = new System.Windows.Forms.Label();
			this.pfcBackOffsetTop = new CodeImp.DoomBuilder.Controls.PairedFieldsControl();
			this.pfcBackOffsetMid = new CodeImp.DoomBuilder.Controls.PairedFieldsControl();
			this.pfcBackOffsetBottom = new CodeImp.DoomBuilder.Controls.PairedFieldsControl();
			this.backTextureOffset = new CodeImp.DoomBuilder.Controls.PairedIntControl();
			this.backlow = new CodeImp.DoomBuilder.Controls.TextureSelectorControl();
			this.backmid = new CodeImp.DoomBuilder.Controls.TextureSelectorControl();
			this.backhigh = new CodeImp.DoomBuilder.Controls.TextureSelectorControl();
			this.tabcomment = new System.Windows.Forms.TabPage();
			this.commenteditor = new CodeImp.DoomBuilder.Controls.CommentEditor();
			this.tabcustom = new System.Windows.Forms.TabPage();
			this.fieldslist = new CodeImp.DoomBuilder.Controls.FieldsEditorControl();
			this.imagelist = new System.Windows.Forms.ImageList(this.components);
			this.tooltip = new System.Windows.Forms.ToolTip(this.components);
			label2 = new System.Windows.Forms.Label();
			label11 = new System.Windows.Forms.Label();
			label12 = new System.Windows.Forms.Label();
			label6 = new System.Windows.Forms.Label();
			this.actiongroup.SuspendLayout();
			this.flagsgroup.SuspendLayout();
			this.tabs.SuspendLayout();
			this.tabproperties.SuspendLayout();
			this.groupsettings.SuspendLayout();
			this.activationGroup.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.missingactivation)).BeginInit();
			this.idgroup.SuspendLayout();
			this.tabfront.SuspendLayout();
			this.frontgroup.SuspendLayout();
			this.frontflagsgroup.SuspendLayout();
			this.frontscalegroup.SuspendLayout();
			this.groupBox6.SuspendLayout();
			this.groupBox5.SuspendLayout();
			this.tabback.SuspendLayout();
			this.backgroup.SuspendLayout();
			this.groupBox4.SuspendLayout();
			this.backflagsgroup.SuspendLayout();
			this.backscalegroup.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.tabcomment.SuspendLayout();
			this.tabcustom.SuspendLayout();
			this.SuspendLayout();
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.Location = new System.Drawing.Point(15, 22);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(40, 13);
			label2.TabIndex = 9;
			label2.Text = "Action:";
			// 
			// label11
			// 
			label11.Location = new System.Drawing.Point(15, 23);
			label11.Name = "label11";
			label11.Size = new System.Drawing.Size(80, 14);
			label11.TabIndex = 13;
			label11.Text = "Sector index:";
			label11.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label12
			// 
			label12.Location = new System.Drawing.Point(15, 23);
			label12.Name = "label12";
			label12.Size = new System.Drawing.Size(80, 14);
			label12.TabIndex = 16;
			label12.Text = "Sector index:";
			label12.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label6
			// 
			label6.AutoSize = true;
			label6.Location = new System.Drawing.Point(189, 21);
			label6.Name = "label6";
			label6.Size = new System.Drawing.Size(37, 13);
			label6.TabIndex = 17;
			label6.Text = "Alpha:";
			// 
			// labelrenderstyle
			// 
			this.labelrenderstyle.AutoSize = true;
			this.labelrenderstyle.Location = new System.Drawing.Point(15, 21);
			this.labelrenderstyle.Name = "labelrenderstyle";
			this.labelrenderstyle.Size = new System.Drawing.Size(69, 13);
			this.labelrenderstyle.TabIndex = 11;
			this.labelrenderstyle.Text = "Render style:";
			// 
			// labellockpick
			// 
			this.labellockpick.AutoSize = true;
			this.labellockpick.Location = new System.Drawing.Point(330, 21);
			this.labellockpick.Name = "labellockpick";
			this.labellockpick.Size = new System.Drawing.Size(72, 13);
			this.labellockpick.TabIndex = 15;
			this.labellockpick.Text = "Lock number:";
			// 
			// labelLightFront
			// 
			this.labelLightFront.Location = new System.Drawing.Point(15, 53);
			this.labelLightFront.Name = "labelLightFront";
			this.labelLightFront.Size = new System.Drawing.Size(80, 14);
			this.labelLightFront.TabIndex = 25;
			this.labelLightFront.Tag = "";
			this.labelLightFront.Text = "Brightness:";
			this.labelLightFront.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// cancel
			// 
			this.cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancel.Location = new System.Drawing.Point(453, 665);
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
			this.apply.Location = new System.Drawing.Point(335, 665);
			this.apply.Name = "apply";
			this.apply.Size = new System.Drawing.Size(112, 25);
			this.apply.TabIndex = 1;
			this.apply.Text = "OK";
			this.apply.UseVisualStyleBackColor = true;
			this.apply.Click += new System.EventHandler(this.apply_Click);
			// 
			// actiongroup
			// 
			this.actiongroup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.actiongroup.Controls.Add(this.argscontrol);
			this.actiongroup.Controls.Add(this.actionhelp);
			this.actiongroup.Controls.Add(label2);
			this.actiongroup.Controls.Add(this.action);
			this.actiongroup.Controls.Add(this.browseaction);
			this.actiongroup.Location = new System.Drawing.Point(8, 245);
			this.actiongroup.Name = "actiongroup";
			this.actiongroup.Size = new System.Drawing.Size(533, 126);
			this.actiongroup.TabIndex = 1;
			this.actiongroup.TabStop = false;
			this.actiongroup.Text = " Action ";
			// 
			// argscontrol
			// 
			this.argscontrol.Location = new System.Drawing.Point(6, 44);
			this.argscontrol.Name = "argscontrol";
			this.argscontrol.Size = new System.Drawing.Size(521, 80);
			this.argscontrol.TabIndex = 11;
			// 
			// actionhelp
			// 
			this.actionhelp.Location = new System.Drawing.Point(497, 17);
			this.actionhelp.Name = "actionhelp";
			this.actionhelp.Size = new System.Drawing.Size(28, 25);
			this.actionhelp.TabIndex = 10;
			// 
			// action
			// 
			this.action.BackColor = System.Drawing.Color.Transparent;
			this.action.Cursor = System.Windows.Forms.Cursors.Default;
			this.action.Empty = false;
			this.action.GeneralizedCategories = null;
			this.action.GeneralizedOptions = null;
			this.action.Location = new System.Drawing.Point(62, 19);
			this.action.Name = "action";
			this.action.Size = new System.Drawing.Size(402, 21);
			this.action.TabIndex = 0;
			this.action.Value = 402;
			this.action.ValueChanges += new System.EventHandler(this.action_ValueChanges);
			// 
			// browseaction
			// 
			this.browseaction.Image = global::CodeImp.DoomBuilder.Properties.Resources.List;
			this.browseaction.Location = new System.Drawing.Point(467, 17);
			this.browseaction.Name = "browseaction";
			this.browseaction.Size = new System.Drawing.Size(28, 25);
			this.browseaction.TabIndex = 1;
			this.tooltip.SetToolTip(this.browseaction, "Browse Action");
			this.browseaction.UseVisualStyleBackColor = true;
			this.browseaction.Click += new System.EventHandler(this.browseaction_Click);
			// 
			// udmfactivates
			// 
			this.udmfactivates.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.udmfactivates.AutoScroll = true;
			this.udmfactivates.Columns = 2;
			this.udmfactivates.Location = new System.Drawing.Point(18, 17);
			this.udmfactivates.Name = "udmfactivates";
			this.udmfactivates.Size = new System.Drawing.Size(509, 130);
			this.udmfactivates.TabIndex = 0;
			this.udmfactivates.VerticalSpacing = 0;
			this.udmfactivates.OnValueChanged += new System.EventHandler(this.udmfactivates_OnValueChanged);
			// 
			// flagsgroup
			// 
			this.flagsgroup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.flagsgroup.Controls.Add(this.flags);
			this.flagsgroup.Location = new System.Drawing.Point(8, 3);
			this.flagsgroup.Name = "flagsgroup";
			this.flagsgroup.Size = new System.Drawing.Size(533, 184);
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
			this.flags.Location = new System.Drawing.Point(18, 15);
			this.flags.Name = "flags";
			this.flags.Size = new System.Drawing.Size(509, 162);
			this.flags.TabIndex = 0;
			this.flags.VerticalSpacing = 0;
			this.flags.OnValueChanged += new System.EventHandler(this.flags_OnValueChanged);
			// 
			// tabs
			// 
			this.tabs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tabs.Controls.Add(this.tabproperties);
			this.tabs.Controls.Add(this.tabfront);
			this.tabs.Controls.Add(this.tabback);
			this.tabs.Controls.Add(this.tabcomment);
			this.tabs.Controls.Add(this.tabcustom);
			this.tabs.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.tabs.ImageList = this.imagelist;
			this.tabs.Location = new System.Drawing.Point(10, 10);
			this.tabs.Margin = new System.Windows.Forms.Padding(1);
			this.tabs.Name = "tabs";
			this.tabs.Padding = new System.Drawing.Point(12, 3);
			this.tabs.SelectedIndex = 0;
			this.tabs.Size = new System.Drawing.Size(557, 648);
			this.tabs.TabIndex = 0;
			// 
			// tabproperties
			// 
			this.tabproperties.Controls.Add(this.flagsgroup);
			this.tabproperties.Controls.Add(this.groupsettings);
			this.tabproperties.Controls.Add(this.actiongroup);
			this.tabproperties.Controls.Add(this.activationGroup);
			this.tabproperties.Controls.Add(this.idgroup);
			this.tabproperties.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tabproperties.Location = new System.Drawing.Point(4, 23);
			this.tabproperties.Name = "tabproperties";
			this.tabproperties.Padding = new System.Windows.Forms.Padding(5);
			this.tabproperties.Size = new System.Drawing.Size(549, 621);
			this.tabproperties.TabIndex = 0;
			this.tabproperties.Text = " Properties ";
			this.tabproperties.UseVisualStyleBackColor = true;
			// 
			// groupsettings
			// 
			this.groupsettings.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupsettings.Controls.Add(this.resetalpha);
			this.groupsettings.Controls.Add(this.lockpick);
			this.groupsettings.Controls.Add(this.alpha);
			this.groupsettings.Controls.Add(label6);
			this.groupsettings.Controls.Add(this.labellockpick);
			this.groupsettings.Controls.Add(this.renderStyle);
			this.groupsettings.Controls.Add(this.labelrenderstyle);
			this.groupsettings.Location = new System.Drawing.Point(8, 193);
			this.groupsettings.Name = "groupsettings";
			this.groupsettings.Size = new System.Drawing.Size(533, 46);
			this.groupsettings.TabIndex = 3;
			this.groupsettings.TabStop = false;
			this.groupsettings.Text = " Settings";
			// 
			// resetalpha
			// 
			this.resetalpha.Image = global::CodeImp.DoomBuilder.Properties.Resources.Reset;
			this.resetalpha.Location = new System.Drawing.Point(301, 16);
			this.resetalpha.Name = "resetalpha";
			this.resetalpha.Size = new System.Drawing.Size(23, 23);
			this.resetalpha.TabIndex = 70;
			this.tooltip.SetToolTip(this.resetalpha, "Reset");
			this.resetalpha.UseVisualStyleBackColor = true;
			this.resetalpha.Click += new System.EventHandler(this.resetalpha_Click);
			// 
			// lockpick
			// 
			this.lockpick.FormattingEnabled = true;
			this.lockpick.Location = new System.Drawing.Point(408, 17);
			this.lockpick.Name = "lockpick";
			this.lockpick.Size = new System.Drawing.Size(115, 21);
			this.lockpick.TabIndex = 19;
			// 
			// alpha
			// 
			this.alpha.AllowDecimal = true;
			this.alpha.AllowExpressions = false;
			this.alpha.AllowNegative = false;
			this.alpha.AllowRelative = false;
			this.alpha.ButtonStep = 1;
			this.alpha.ButtonStepBig = 0.25F;
			this.alpha.ButtonStepFloat = 0.1F;
			this.alpha.ButtonStepSmall = 0.01F;
			this.alpha.ButtonStepsUseModifierKeys = true;
			this.alpha.ButtonStepsWrapAround = false;
			this.alpha.Location = new System.Drawing.Point(233, 16);
			this.alpha.Name = "alpha";
			this.alpha.Size = new System.Drawing.Size(65, 24);
			this.alpha.StepValues = null;
			this.alpha.TabIndex = 18;
			this.alpha.WhenTextChanged += new System.EventHandler(this.alpha_WhenTextChanged);
			// 
			// renderStyle
			// 
			this.renderStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.renderStyle.FormattingEnabled = true;
			this.renderStyle.Location = new System.Drawing.Point(92, 17);
			this.renderStyle.Name = "renderStyle";
			this.renderStyle.Size = new System.Drawing.Size(86, 21);
			this.renderStyle.TabIndex = 12;
			this.renderStyle.SelectedIndexChanged += new System.EventHandler(this.cbRenderStyle_SelectedIndexChanged);
			// 
			// activationGroup
			// 
			this.activationGroup.Controls.Add(this.missingactivation);
			this.activationGroup.Controls.Add(this.udmfactivates);
			this.activationGroup.Location = new System.Drawing.Point(8, 375);
			this.activationGroup.Name = "activationGroup";
			this.activationGroup.Size = new System.Drawing.Size(533, 152);
			this.activationGroup.TabIndex = 1;
			this.activationGroup.TabStop = false;
			this.activationGroup.Text = " Activation ";
			// 
			// missingactivation
			// 
			this.missingactivation.BackColor = System.Drawing.SystemColors.Window;
			this.missingactivation.Image = global::CodeImp.DoomBuilder.Properties.Resources.Warning;
			this.missingactivation.Location = new System.Drawing.Point(64, -1);
			this.missingactivation.Name = "missingactivation";
			this.missingactivation.Size = new System.Drawing.Size(16, 16);
			this.missingactivation.TabIndex = 4;
			this.missingactivation.TabStop = false;
			this.tooltip.SetToolTip(this.missingactivation, "Selected action requires activation!");
			this.missingactivation.Visible = false;
			// 
			// idgroup
			// 
			this.idgroup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.idgroup.Controls.Add(this.tagsselector);
			this.idgroup.Location = new System.Drawing.Point(8, 533);
			this.idgroup.Name = "idgroup";
			this.idgroup.Size = new System.Drawing.Size(533, 85);
			this.idgroup.TabIndex = 2;
			this.idgroup.TabStop = false;
			this.idgroup.Text = " Identification ";
			// 
			// tagsselector
			// 
			this.tagsselector.Location = new System.Drawing.Point(6, 21);
			this.tagsselector.Name = "tagsselector";
			this.tagsselector.Size = new System.Drawing.Size(524, 60);
			this.tagsselector.TabIndex = 0;
			// 
			// tabfront
			// 
			this.tabfront.Controls.Add(this.frontside);
			this.tabfront.Controls.Add(this.frontgroup);
			this.tabfront.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tabfront.ForeColor = System.Drawing.SystemColors.ControlText;
			this.tabfront.ImageIndex = 0;
			this.tabfront.Location = new System.Drawing.Point(4, 23);
			this.tabfront.Name = "tabfront";
			this.tabfront.Padding = new System.Windows.Forms.Padding(5);
			this.tabfront.Size = new System.Drawing.Size(549, 621);
			this.tabfront.TabIndex = 1;
			this.tabfront.Text = " Front ";
			this.tabfront.UseVisualStyleBackColor = true;
			// 
			// frontside
			// 
			this.frontside.AutoSize = true;
			this.frontside.Location = new System.Drawing.Point(20, 6);
			this.frontside.Name = "frontside";
			this.frontside.Size = new System.Drawing.Size(72, 17);
			this.frontside.TabIndex = 0;
			this.frontside.Text = "Front side";
			this.frontside.UseVisualStyleBackColor = true;
			this.frontside.CheckStateChanged += new System.EventHandler(this.frontside_CheckStateChanged);
			// 
			// frontgroup
			// 
			this.frontgroup.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.frontgroup.Controls.Add(this.frontflagsgroup);
			this.frontgroup.Controls.Add(this.frontscalegroup);
			this.frontgroup.Controls.Add(this.groupBox6);
			this.frontgroup.Controls.Add(this.groupBox5);
			this.frontgroup.Controls.Add(this.frontlow);
			this.frontgroup.Controls.Add(this.frontmid);
			this.frontgroup.Controls.Add(this.fronthigh);
			this.frontgroup.Enabled = false;
			this.frontgroup.Location = new System.Drawing.Point(8, 8);
			this.frontgroup.Name = "frontgroup";
			this.frontgroup.Size = new System.Drawing.Size(535, 605);
			this.frontgroup.TabIndex = 1;
			this.frontgroup.TabStop = false;
			this.frontgroup.Text = "     ";
			// 
			// frontflagsgroup
			// 
			this.frontflagsgroup.Controls.Add(this.flagsFront);
			this.frontflagsgroup.Location = new System.Drawing.Point(12, 490);
			this.frontflagsgroup.Name = "frontflagsgroup";
			this.frontflagsgroup.Size = new System.Drawing.Size(290, 109);
			this.frontflagsgroup.TabIndex = 45;
			this.frontflagsgroup.TabStop = false;
			this.frontflagsgroup.Text = " Flags ";
			// 
			// flagsFront
			// 
			this.flagsFront.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.flagsFront.AutoScroll = true;
			this.flagsFront.Columns = 2;
			this.flagsFront.Location = new System.Drawing.Point(16, 16);
			this.flagsFront.Name = "flagsFront";
			this.flagsFront.Size = new System.Drawing.Size(269, 87);
			this.flagsFront.TabIndex = 0;
			this.flagsFront.VerticalSpacing = 3;
			this.flagsFront.OnValueChanged += new System.EventHandler(this.flagsFront_OnValueChanged);
			// 
			// frontscalegroup
			// 
			this.frontscalegroup.Controls.Add(this.labelFrontScaleBottom);
			this.frontscalegroup.Controls.Add(this.labelFrontScaleMid);
			this.frontscalegroup.Controls.Add(this.labelFrontScaleTop);
			this.frontscalegroup.Controls.Add(this.pfcFrontScaleTop);
			this.frontscalegroup.Controls.Add(this.pfcFrontScaleBottom);
			this.frontscalegroup.Controls.Add(this.pfcFrontScaleMid);
			this.frontscalegroup.Location = new System.Drawing.Point(12, 377);
			this.frontscalegroup.Name = "frontscalegroup";
			this.frontscalegroup.Size = new System.Drawing.Size(290, 112);
			this.frontscalegroup.TabIndex = 44;
			this.frontscalegroup.TabStop = false;
			this.frontscalegroup.Text = " Texture scale ";
			// 
			// labelFrontScaleBottom
			// 
			this.labelFrontScaleBottom.Location = new System.Drawing.Point(14, 86);
			this.labelFrontScaleBottom.Name = "labelFrontScaleBottom";
			this.labelFrontScaleBottom.Size = new System.Drawing.Size(80, 14);
			this.labelFrontScaleBottom.TabIndex = 42;
			this.labelFrontScaleBottom.Tag = "";
			this.labelFrontScaleBottom.Text = "Lower scale:";
			this.labelFrontScaleBottom.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// labelFrontScaleMid
			// 
			this.labelFrontScaleMid.Location = new System.Drawing.Point(14, 54);
			this.labelFrontScaleMid.Name = "labelFrontScaleMid";
			this.labelFrontScaleMid.Size = new System.Drawing.Size(80, 14);
			this.labelFrontScaleMid.TabIndex = 41;
			this.labelFrontScaleMid.Tag = "";
			this.labelFrontScaleMid.Text = "Middle scale:";
			this.labelFrontScaleMid.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// labelFrontScaleTop
			// 
			this.labelFrontScaleTop.Location = new System.Drawing.Point(14, 22);
			this.labelFrontScaleTop.Name = "labelFrontScaleTop";
			this.labelFrontScaleTop.Size = new System.Drawing.Size(80, 14);
			this.labelFrontScaleTop.TabIndex = 28;
			this.labelFrontScaleTop.Tag = "";
			this.labelFrontScaleTop.Text = "Upper scale:";
			this.labelFrontScaleTop.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// pfcFrontScaleTop
			// 
			this.pfcFrontScaleTop.AllowDecimal = true;
			this.pfcFrontScaleTop.AllowValueLinking = true;
			this.pfcFrontScaleTop.ButtonStep = 1;
			this.pfcFrontScaleTop.ButtonStepBig = 1F;
			this.pfcFrontScaleTop.ButtonStepFloat = 0.1F;
			this.pfcFrontScaleTop.ButtonStepSmall = 0.01F;
			this.pfcFrontScaleTop.ButtonStepsUseModifierKeys = true;
			this.pfcFrontScaleTop.DefaultValue = 1F;
			this.pfcFrontScaleTop.Field1 = "scalex_top";
			this.pfcFrontScaleTop.Field2 = "scaley_top";
			this.pfcFrontScaleTop.LinkValues = false;
			this.pfcFrontScaleTop.Location = new System.Drawing.Point(96, 17);
			this.pfcFrontScaleTop.Name = "pfcFrontScaleTop";
			this.pfcFrontScaleTop.Size = new System.Drawing.Size(186, 26);
			this.pfcFrontScaleTop.TabIndex = 38;
			this.pfcFrontScaleTop.OnValuesChanged += new System.EventHandler(this.pfcFrontScaleTop_OnValuesChanged);
			// 
			// pfcFrontScaleBottom
			// 
			this.pfcFrontScaleBottom.AllowDecimal = true;
			this.pfcFrontScaleBottom.AllowValueLinking = true;
			this.pfcFrontScaleBottom.ButtonStep = 1;
			this.pfcFrontScaleBottom.ButtonStepBig = 1F;
			this.pfcFrontScaleBottom.ButtonStepFloat = 0.1F;
			this.pfcFrontScaleBottom.ButtonStepSmall = 0.01F;
			this.pfcFrontScaleBottom.ButtonStepsUseModifierKeys = true;
			this.pfcFrontScaleBottom.DefaultValue = 1F;
			this.pfcFrontScaleBottom.Field1 = "scalex_bottom";
			this.pfcFrontScaleBottom.Field2 = "scaley_bottom";
			this.pfcFrontScaleBottom.LinkValues = false;
			this.pfcFrontScaleBottom.Location = new System.Drawing.Point(96, 81);
			this.pfcFrontScaleBottom.Name = "pfcFrontScaleBottom";
			this.pfcFrontScaleBottom.Size = new System.Drawing.Size(186, 26);
			this.pfcFrontScaleBottom.TabIndex = 40;
			this.pfcFrontScaleBottom.OnValuesChanged += new System.EventHandler(this.pfcFrontScaleBottom_OnValuesChanged);
			// 
			// pfcFrontScaleMid
			// 
			this.pfcFrontScaleMid.AllowDecimal = true;
			this.pfcFrontScaleMid.AllowValueLinking = true;
			this.pfcFrontScaleMid.ButtonStep = 1;
			this.pfcFrontScaleMid.ButtonStepBig = 1F;
			this.pfcFrontScaleMid.ButtonStepFloat = 0.1F;
			this.pfcFrontScaleMid.ButtonStepSmall = 0.01F;
			this.pfcFrontScaleMid.ButtonStepsUseModifierKeys = true;
			this.pfcFrontScaleMid.DefaultValue = 1F;
			this.pfcFrontScaleMid.Field1 = "scalex_mid";
			this.pfcFrontScaleMid.Field2 = "scaley_mid";
			this.pfcFrontScaleMid.LinkValues = false;
			this.pfcFrontScaleMid.Location = new System.Drawing.Point(96, 49);
			this.pfcFrontScaleMid.Name = "pfcFrontScaleMid";
			this.pfcFrontScaleMid.Size = new System.Drawing.Size(186, 26);
			this.pfcFrontScaleMid.TabIndex = 39;
			this.pfcFrontScaleMid.OnValuesChanged += new System.EventHandler(this.pfcFrontScaleMid_OnValuesChanged);
			// 
			// groupBox6
			// 
			this.groupBox6.Controls.Add(this.labelFrontTextureOffset);
			this.groupBox6.Controls.Add(this.labelFrontOffsetBottom);
			this.groupBox6.Controls.Add(this.frontTextureOffset);
			this.groupBox6.Controls.Add(this.labelFrontOffsetMid);
			this.groupBox6.Controls.Add(this.pfcFrontOffsetTop);
			this.groupBox6.Controls.Add(this.labelFrontOffsetTop);
			this.groupBox6.Controls.Add(this.pfcFrontOffsetMid);
			this.groupBox6.Controls.Add(this.pfcFrontOffsetBottom);
			this.groupBox6.Location = new System.Drawing.Point(12, 228);
			this.groupBox6.Name = "groupBox6";
			this.groupBox6.Size = new System.Drawing.Size(290, 143);
			this.groupBox6.TabIndex = 43;
			this.groupBox6.TabStop = false;
			this.groupBox6.Text = " Texture offsets ";
			// 
			// labelFrontTextureOffset
			// 
			this.labelFrontTextureOffset.Location = new System.Drawing.Point(14, 23);
			this.labelFrontTextureOffset.Name = "labelFrontTextureOffset";
			this.labelFrontTextureOffset.Size = new System.Drawing.Size(80, 14);
			this.labelFrontTextureOffset.TabIndex = 46;
			this.labelFrontTextureOffset.Tag = "";
			this.labelFrontTextureOffset.Text = "Sidedef offset:";
			this.labelFrontTextureOffset.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// labelFrontOffsetBottom
			// 
			this.labelFrontOffsetBottom.Location = new System.Drawing.Point(14, 118);
			this.labelFrontOffsetBottom.Name = "labelFrontOffsetBottom";
			this.labelFrontOffsetBottom.Size = new System.Drawing.Size(80, 14);
			this.labelFrontOffsetBottom.TabIndex = 45;
			this.labelFrontOffsetBottom.Tag = "";
			this.labelFrontOffsetBottom.Text = "Lower offset:";
			this.labelFrontOffsetBottom.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// frontTextureOffset
			// 
			this.frontTextureOffset.ButtonStep = 1;
			this.frontTextureOffset.ButtonStepBig = 16F;
			this.frontTextureOffset.ButtonStepSmall = 0.1F;
			this.frontTextureOffset.ButtonStepsUseModifierKeys = true;
			this.frontTextureOffset.DefaultValue = 0;
			this.frontTextureOffset.Location = new System.Drawing.Point(96, 17);
			this.frontTextureOffset.Name = "frontTextureOffset";
			this.frontTextureOffset.Size = new System.Drawing.Size(186, 26);
			this.frontTextureOffset.TabIndex = 41;
			this.frontTextureOffset.OnValuesChanged += new System.EventHandler(this.frontTextureOffset_OnValuesChanged);
			// 
			// labelFrontOffsetMid
			// 
			this.labelFrontOffsetMid.Location = new System.Drawing.Point(14, 86);
			this.labelFrontOffsetMid.Name = "labelFrontOffsetMid";
			this.labelFrontOffsetMid.Size = new System.Drawing.Size(80, 14);
			this.labelFrontOffsetMid.TabIndex = 44;
			this.labelFrontOffsetMid.Tag = "";
			this.labelFrontOffsetMid.Text = "Middle offset:";
			this.labelFrontOffsetMid.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// pfcFrontOffsetTop
			// 
			this.pfcFrontOffsetTop.AllowDecimal = true;
			this.pfcFrontOffsetTop.AllowValueLinking = false;
			this.pfcFrontOffsetTop.ButtonStep = 1;
			this.pfcFrontOffsetTop.ButtonStepBig = 16F;
			this.pfcFrontOffsetTop.ButtonStepFloat = 1F;
			this.pfcFrontOffsetTop.ButtonStepSmall = 0.1F;
			this.pfcFrontOffsetTop.ButtonStepsUseModifierKeys = true;
			this.pfcFrontOffsetTop.DefaultValue = 0F;
			this.pfcFrontOffsetTop.Field1 = "offsetx_top";
			this.pfcFrontOffsetTop.Field2 = "offsety_top";
			this.pfcFrontOffsetTop.LinkValues = false;
			this.pfcFrontOffsetTop.Location = new System.Drawing.Point(96, 49);
			this.pfcFrontOffsetTop.Name = "pfcFrontOffsetTop";
			this.pfcFrontOffsetTop.Size = new System.Drawing.Size(186, 26);
			this.pfcFrontOffsetTop.TabIndex = 35;
			this.pfcFrontOffsetTop.OnValuesChanged += new System.EventHandler(this.pfcFrontOffsetTop_OnValuesChanged);
			// 
			// labelFrontOffsetTop
			// 
			this.labelFrontOffsetTop.Location = new System.Drawing.Point(14, 54);
			this.labelFrontOffsetTop.Name = "labelFrontOffsetTop";
			this.labelFrontOffsetTop.Size = new System.Drawing.Size(80, 14);
			this.labelFrontOffsetTop.TabIndex = 43;
			this.labelFrontOffsetTop.Tag = "";
			this.labelFrontOffsetTop.Text = "Upper offset:";
			this.labelFrontOffsetTop.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// pfcFrontOffsetMid
			// 
			this.pfcFrontOffsetMid.AllowDecimal = true;
			this.pfcFrontOffsetMid.AllowValueLinking = false;
			this.pfcFrontOffsetMid.ButtonStep = 1;
			this.pfcFrontOffsetMid.ButtonStepBig = 16F;
			this.pfcFrontOffsetMid.ButtonStepFloat = 1F;
			this.pfcFrontOffsetMid.ButtonStepSmall = 0.1F;
			this.pfcFrontOffsetMid.ButtonStepsUseModifierKeys = true;
			this.pfcFrontOffsetMid.DefaultValue = 0F;
			this.pfcFrontOffsetMid.Field1 = "offsetx_mid";
			this.pfcFrontOffsetMid.Field2 = "offsety_mid";
			this.pfcFrontOffsetMid.LinkValues = false;
			this.pfcFrontOffsetMid.Location = new System.Drawing.Point(96, 81);
			this.pfcFrontOffsetMid.Name = "pfcFrontOffsetMid";
			this.pfcFrontOffsetMid.Size = new System.Drawing.Size(186, 26);
			this.pfcFrontOffsetMid.TabIndex = 36;
			this.pfcFrontOffsetMid.OnValuesChanged += new System.EventHandler(this.pfcFrontOffsetMid_OnValuesChanged);
			// 
			// pfcFrontOffsetBottom
			// 
			this.pfcFrontOffsetBottom.AllowDecimal = true;
			this.pfcFrontOffsetBottom.AllowValueLinking = false;
			this.pfcFrontOffsetBottom.ButtonStep = 1;
			this.pfcFrontOffsetBottom.ButtonStepBig = 16F;
			this.pfcFrontOffsetBottom.ButtonStepFloat = 1F;
			this.pfcFrontOffsetBottom.ButtonStepSmall = 0.1F;
			this.pfcFrontOffsetBottom.ButtonStepsUseModifierKeys = true;
			this.pfcFrontOffsetBottom.DefaultValue = 0F;
			this.pfcFrontOffsetBottom.Field1 = "offsetx_bottom";
			this.pfcFrontOffsetBottom.Field2 = "offsety_bottom";
			this.pfcFrontOffsetBottom.LinkValues = false;
			this.pfcFrontOffsetBottom.Location = new System.Drawing.Point(96, 113);
			this.pfcFrontOffsetBottom.Name = "pfcFrontOffsetBottom";
			this.pfcFrontOffsetBottom.Size = new System.Drawing.Size(186, 26);
			this.pfcFrontOffsetBottom.TabIndex = 37;
			this.pfcFrontOffsetBottom.OnValuesChanged += new System.EventHandler(this.pfcFrontOffsetBottom_OnValuesChanged);
			// 
			// groupBox5
			// 
			this.groupBox5.Controls.Add(this.lightfrontlower);
			this.groupBox5.Controls.Add(this.lightfrontmiddle);
			this.groupBox5.Controls.Add(this.lightfrontupper);
			this.groupBox5.Controls.Add(this.resetfrontlight);
			this.groupBox5.Controls.Add(this.frontsector);
			this.groupBox5.Controls.Add(label11);
			this.groupBox5.Controls.Add(this.customfrontbutton);
			this.groupBox5.Controls.Add(this.lightFront);
			this.groupBox5.Controls.Add(this.labelLightFront);
			this.groupBox5.Controls.Add(this.cbLightAbsoluteFront);
			this.groupBox5.Location = new System.Drawing.Point(12, 19);
			this.groupBox5.Name = "groupBox5";
			this.groupBox5.Size = new System.Drawing.Size(290, 203);
			this.groupBox5.TabIndex = 42;
			this.groupBox5.TabStop = false;
			// 
			// lightfrontlower
			// 
			this.lightfrontlower.Location = new System.Drawing.Point(0, 137);
			this.lightfrontlower.Name = "lightfrontlower";
			this.lightfrontlower.Size = new System.Drawing.Size(262, 29);
			this.lightfrontlower.TabIndex = 29;
			// 
			// lightfrontmiddle
			// 
			this.lightfrontmiddle.Location = new System.Drawing.Point(0, 107);
			this.lightfrontmiddle.Name = "lightfrontmiddle";
			this.lightfrontmiddle.Size = new System.Drawing.Size(262, 29);
			this.lightfrontmiddle.TabIndex = 29;
			// 
			// lightfrontupper
			// 
			this.lightfrontupper.Location = new System.Drawing.Point(0, 77);
			this.lightfrontupper.Name = "lightfrontupper";
			this.lightfrontupper.Size = new System.Drawing.Size(262, 29);
			this.lightfrontupper.TabIndex = 29;
			// 
			// resetfrontlight
			// 
			this.resetfrontlight.Image = global::CodeImp.DoomBuilder.Properties.Resources.Reset;
			this.resetfrontlight.Location = new System.Drawing.Point(236, 50);
			this.resetfrontlight.Name = "resetfrontlight";
			this.resetfrontlight.Size = new System.Drawing.Size(23, 23);
			this.resetfrontlight.TabIndex = 28;
			this.tooltip.SetToolTip(this.resetfrontlight, "Reset Front Brightness");
			this.resetfrontlight.UseVisualStyleBackColor = true;
			this.resetfrontlight.Click += new System.EventHandler(this.resetfrontlight_Click);
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
			this.frontsector.Location = new System.Drawing.Point(99, 19);
			this.frontsector.Name = "frontsector";
			this.frontsector.Size = new System.Drawing.Size(130, 24);
			this.frontsector.StepValues = null;
			this.frontsector.TabIndex = 14;
			// 
			// customfrontbutton
			// 
			this.customfrontbutton.Location = new System.Drawing.Point(98, 170);
			this.customfrontbutton.Name = "customfrontbutton";
			this.customfrontbutton.Size = new System.Drawing.Size(130, 25);
			this.customfrontbutton.TabIndex = 3;
			this.customfrontbutton.Text = "Custom fields...";
			this.customfrontbutton.UseVisualStyleBackColor = true;
			this.customfrontbutton.Click += new System.EventHandler(this.customfrontbutton_Click);
			// 
			// lightFront
			// 
			this.lightFront.AllowDecimal = false;
			this.lightFront.AllowExpressions = false;
			this.lightFront.AllowNegative = true;
			this.lightFront.AllowRelative = true;
			this.lightFront.ButtonStep = 16;
			this.lightFront.ButtonStepBig = 32F;
			this.lightFront.ButtonStepFloat = 1F;
			this.lightFront.ButtonStepSmall = 1F;
			this.lightFront.ButtonStepsUseModifierKeys = true;
			this.lightFront.ButtonStepsWrapAround = false;
			this.lightFront.Location = new System.Drawing.Point(99, 49);
			this.lightFront.Name = "lightFront";
			this.lightFront.Size = new System.Drawing.Size(62, 24);
			this.lightFront.StepValues = null;
			this.lightFront.TabIndex = 26;
			this.lightFront.Tag = "";
			this.lightFront.WhenTextChanged += new System.EventHandler(this.lightFront_WhenTextChanged);
			// 
			// cbLightAbsoluteFront
			// 
			this.cbLightAbsoluteFront.AutoSize = true;
			this.cbLightAbsoluteFront.Location = new System.Drawing.Point(167, 54);
			this.cbLightAbsoluteFront.Name = "cbLightAbsoluteFront";
			this.cbLightAbsoluteFront.Size = new System.Drawing.Size(67, 17);
			this.cbLightAbsoluteFront.TabIndex = 27;
			this.cbLightAbsoluteFront.Tag = "lightabsolute";
			this.cbLightAbsoluteFront.Text = "Absolute";
			this.cbLightAbsoluteFront.UseVisualStyleBackColor = true;
			this.cbLightAbsoluteFront.CheckedChanged += new System.EventHandler(this.cbLightAbsoluteFront_CheckedChanged);
			// 
			// frontlow
			// 
			this.frontlow.Location = new System.Drawing.Point(309, 409);
			this.frontlow.MultipleTextures = false;
			this.frontlow.Name = "frontlow";
			this.frontlow.Required = false;
			this.frontlow.Size = new System.Drawing.Size(220, 189);
			this.frontlow.TabIndex = 6;
			this.frontlow.TextureName = "";
			this.frontlow.OnValueChanged += new System.EventHandler(this.frontlow_OnValueChanged);
			// 
			// frontmid
			// 
			this.frontmid.Location = new System.Drawing.Point(309, 214);
			this.frontmid.MultipleTextures = false;
			this.frontmid.Name = "frontmid";
			this.frontmid.Required = false;
			this.frontmid.Size = new System.Drawing.Size(220, 189);
			this.frontmid.TabIndex = 5;
			this.frontmid.TextureName = "";
			this.frontmid.OnValueChanged += new System.EventHandler(this.frontmid_OnValueChanged);
			// 
			// fronthigh
			// 
			this.fronthigh.Location = new System.Drawing.Point(309, 19);
			this.fronthigh.MultipleTextures = false;
			this.fronthigh.Name = "fronthigh";
			this.fronthigh.Required = false;
			this.fronthigh.Size = new System.Drawing.Size(220, 189);
			this.fronthigh.TabIndex = 4;
			this.fronthigh.TextureName = "";
			this.fronthigh.OnValueChanged += new System.EventHandler(this.fronthigh_OnValueChanged);
			// 
			// tabback
			// 
			this.tabback.Controls.Add(this.backside);
			this.tabback.Controls.Add(this.backgroup);
			this.tabback.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tabback.ImageIndex = 1;
			this.tabback.Location = new System.Drawing.Point(4, 23);
			this.tabback.Name = "tabback";
			this.tabback.Padding = new System.Windows.Forms.Padding(5);
			this.tabback.Size = new System.Drawing.Size(549, 621);
			this.tabback.TabIndex = 3;
			this.tabback.Text = " Back ";
			this.tabback.UseVisualStyleBackColor = true;
			// 
			// backside
			// 
			this.backside.AutoSize = true;
			this.backside.Location = new System.Drawing.Point(20, 6);
			this.backside.Name = "backside";
			this.backside.Size = new System.Drawing.Size(73, 17);
			this.backside.TabIndex = 0;
			this.backside.Text = "Back side";
			this.backside.UseVisualStyleBackColor = true;
			this.backside.CheckStateChanged += new System.EventHandler(this.backside_CheckStateChanged);
			// 
			// backgroup
			// 
			this.backgroup.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.backgroup.Controls.Add(this.groupBox4);
			this.backgroup.Controls.Add(this.backflagsgroup);
			this.backgroup.Controls.Add(this.backscalegroup);
			this.backgroup.Controls.Add(this.groupBox1);
			this.backgroup.Controls.Add(this.backlow);
			this.backgroup.Controls.Add(this.backmid);
			this.backgroup.Controls.Add(this.backhigh);
			this.backgroup.Enabled = false;
			this.backgroup.Location = new System.Drawing.Point(8, 8);
			this.backgroup.Name = "backgroup";
			this.backgroup.Size = new System.Drawing.Size(535, 605);
			this.backgroup.TabIndex = 1;
			this.backgroup.TabStop = false;
			this.backgroup.Text = "     ";
			// 
			// groupBox4
			// 
			this.groupBox4.Controls.Add(this.lightbacklower);
			this.groupBox4.Controls.Add(this.lightbackmiddle);
			this.groupBox4.Controls.Add(this.lightbackupper);
			this.groupBox4.Controls.Add(this.resetbacklight);
			this.groupBox4.Controls.Add(this.backsector);
			this.groupBox4.Controls.Add(label12);
			this.groupBox4.Controls.Add(this.custombackbutton);
			this.groupBox4.Controls.Add(this.lightBack);
			this.groupBox4.Controls.Add(this.labelLightBack);
			this.groupBox4.Controls.Add(this.cbLightAbsoluteBack);
			this.groupBox4.Location = new System.Drawing.Point(12, 19);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Size = new System.Drawing.Size(290, 203);
			this.groupBox4.TabIndex = 46;
			this.groupBox4.TabStop = false;
			// 
			// lightbacklower
			// 
			this.lightbacklower.Location = new System.Drawing.Point(0, 137);
			this.lightbacklower.Name = "lightbacklower";
			this.lightbacklower.Size = new System.Drawing.Size(262, 29);
			this.lightbacklower.TabIndex = 32;
			// 
			// lightbackmiddle
			// 
			this.lightbackmiddle.Location = new System.Drawing.Point(0, 107);
			this.lightbackmiddle.Name = "lightbackmiddle";
			this.lightbackmiddle.Size = new System.Drawing.Size(262, 29);
			this.lightbackmiddle.TabIndex = 33;
			// 
			// lightbackupper
			// 
			this.lightbackupper.Location = new System.Drawing.Point(0, 77);
			this.lightbackupper.Name = "lightbackupper";
			this.lightbackupper.Size = new System.Drawing.Size(262, 29);
			this.lightbackupper.TabIndex = 34;
			// 
			// resetbacklight
			// 
			this.resetbacklight.Image = global::CodeImp.DoomBuilder.Properties.Resources.Reset;
			this.resetbacklight.Location = new System.Drawing.Point(236, 50);
			this.resetbacklight.Name = "resetbacklight";
			this.resetbacklight.Size = new System.Drawing.Size(23, 23);
			this.resetbacklight.TabIndex = 31;
			this.tooltip.SetToolTip(this.resetbacklight, "Reset Back Brightness");
			this.resetbacklight.UseVisualStyleBackColor = true;
			this.resetbacklight.Click += new System.EventHandler(this.resetbacklight_Click);
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
			this.backsector.Location = new System.Drawing.Point(99, 19);
			this.backsector.Name = "backsector";
			this.backsector.Size = new System.Drawing.Size(130, 24);
			this.backsector.StepValues = null;
			this.backsector.TabIndex = 17;
			// 
			// custombackbutton
			// 
			this.custombackbutton.Location = new System.Drawing.Point(98, 170);
			this.custombackbutton.Name = "custombackbutton";
			this.custombackbutton.Size = new System.Drawing.Size(130, 25);
			this.custombackbutton.TabIndex = 3;
			this.custombackbutton.Text = "Custom fields...";
			this.custombackbutton.UseVisualStyleBackColor = true;
			this.custombackbutton.Click += new System.EventHandler(this.custombackbutton_Click);
			// 
			// lightBack
			// 
			this.lightBack.AllowDecimal = false;
			this.lightBack.AllowExpressions = false;
			this.lightBack.AllowNegative = true;
			this.lightBack.AllowRelative = true;
			this.lightBack.ButtonStep = 1;
			this.lightBack.ButtonStepBig = 16F;
			this.lightBack.ButtonStepFloat = 1F;
			this.lightBack.ButtonStepSmall = 1F;
			this.lightBack.ButtonStepsUseModifierKeys = true;
			this.lightBack.ButtonStepsWrapAround = false;
			this.lightBack.Location = new System.Drawing.Point(99, 49);
			this.lightBack.Name = "lightBack";
			this.lightBack.Size = new System.Drawing.Size(62, 24);
			this.lightBack.StepValues = null;
			this.lightBack.TabIndex = 29;
			this.lightBack.Tag = "light";
			this.lightBack.WhenTextChanged += new System.EventHandler(this.lightBack_WhenTextChanged);
			// 
			// labelLightBack
			// 
			this.labelLightBack.Location = new System.Drawing.Point(15, 53);
			this.labelLightBack.Name = "labelLightBack";
			this.labelLightBack.Size = new System.Drawing.Size(80, 14);
			this.labelLightBack.TabIndex = 28;
			this.labelLightBack.Tag = "";
			this.labelLightBack.Text = "Brightness:";
			this.labelLightBack.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// cbLightAbsoluteBack
			// 
			this.cbLightAbsoluteBack.AutoSize = true;
			this.cbLightAbsoluteBack.Location = new System.Drawing.Point(167, 54);
			this.cbLightAbsoluteBack.Name = "cbLightAbsoluteBack";
			this.cbLightAbsoluteBack.Size = new System.Drawing.Size(67, 17);
			this.cbLightAbsoluteBack.TabIndex = 30;
			this.cbLightAbsoluteBack.Tag = "lightabsolute";
			this.cbLightAbsoluteBack.Text = "Absolute";
			this.cbLightAbsoluteBack.UseVisualStyleBackColor = true;
			this.cbLightAbsoluteBack.CheckedChanged += new System.EventHandler(this.cbLightAbsoluteBack_CheckedChanged);
			// 
			// backflagsgroup
			// 
			this.backflagsgroup.Controls.Add(this.flagsBack);
			this.backflagsgroup.Location = new System.Drawing.Point(12, 490);
			this.backflagsgroup.Name = "backflagsgroup";
			this.backflagsgroup.Size = new System.Drawing.Size(290, 109);
			this.backflagsgroup.TabIndex = 45;
			this.backflagsgroup.TabStop = false;
			this.backflagsgroup.Text = " Flags ";
			// 
			// flagsBack
			// 
			this.flagsBack.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.flagsBack.AutoScroll = true;
			this.flagsBack.Columns = 2;
			this.flagsBack.Location = new System.Drawing.Point(16, 16);
			this.flagsBack.Name = "flagsBack";
			this.flagsBack.Size = new System.Drawing.Size(269, 87);
			this.flagsBack.TabIndex = 1;
			this.flagsBack.VerticalSpacing = 3;
			this.flagsBack.OnValueChanged += new System.EventHandler(this.flagsBack_OnValueChanged);
			// 
			// backscalegroup
			// 
			this.backscalegroup.Controls.Add(this.labelBackScaleBottom);
			this.backscalegroup.Controls.Add(this.labelBackScaleMid);
			this.backscalegroup.Controls.Add(this.labelBackScaleTop);
			this.backscalegroup.Controls.Add(this.pfcBackScaleTop);
			this.backscalegroup.Controls.Add(this.pfcBackScaleBottom);
			this.backscalegroup.Controls.Add(this.pfcBackScaleMid);
			this.backscalegroup.Location = new System.Drawing.Point(12, 377);
			this.backscalegroup.Name = "backscalegroup";
			this.backscalegroup.Size = new System.Drawing.Size(290, 112);
			this.backscalegroup.TabIndex = 44;
			this.backscalegroup.TabStop = false;
			this.backscalegroup.Text = " Texture scale";
			// 
			// labelBackScaleBottom
			// 
			this.labelBackScaleBottom.Location = new System.Drawing.Point(14, 86);
			this.labelBackScaleBottom.Name = "labelBackScaleBottom";
			this.labelBackScaleBottom.Size = new System.Drawing.Size(80, 14);
			this.labelBackScaleBottom.TabIndex = 45;
			this.labelBackScaleBottom.Tag = "";
			this.labelBackScaleBottom.Text = "Lower scale:";
			this.labelBackScaleBottom.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// labelBackScaleMid
			// 
			this.labelBackScaleMid.Location = new System.Drawing.Point(14, 54);
			this.labelBackScaleMid.Name = "labelBackScaleMid";
			this.labelBackScaleMid.Size = new System.Drawing.Size(80, 14);
			this.labelBackScaleMid.TabIndex = 44;
			this.labelBackScaleMid.Tag = "";
			this.labelBackScaleMid.Text = "Middle scale:";
			this.labelBackScaleMid.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// labelBackScaleTop
			// 
			this.labelBackScaleTop.Location = new System.Drawing.Point(14, 22);
			this.labelBackScaleTop.Name = "labelBackScaleTop";
			this.labelBackScaleTop.Size = new System.Drawing.Size(80, 14);
			this.labelBackScaleTop.TabIndex = 43;
			this.labelBackScaleTop.Tag = "";
			this.labelBackScaleTop.Text = "Upper scale:";
			this.labelBackScaleTop.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// pfcBackScaleTop
			// 
			this.pfcBackScaleTop.AllowDecimal = true;
			this.pfcBackScaleTop.AllowValueLinking = true;
			this.pfcBackScaleTop.ButtonStep = 1;
			this.pfcBackScaleTop.ButtonStepBig = 1F;
			this.pfcBackScaleTop.ButtonStepFloat = 0.1F;
			this.pfcBackScaleTop.ButtonStepSmall = 0.01F;
			this.pfcBackScaleTop.ButtonStepsUseModifierKeys = true;
			this.pfcBackScaleTop.DefaultValue = 1F;
			this.pfcBackScaleTop.Field1 = "scalex_top";
			this.pfcBackScaleTop.Field2 = "scaley_top";
			this.pfcBackScaleTop.LinkValues = false;
			this.pfcBackScaleTop.Location = new System.Drawing.Point(96, 17);
			this.pfcBackScaleTop.Name = "pfcBackScaleTop";
			this.pfcBackScaleTop.Size = new System.Drawing.Size(186, 26);
			this.pfcBackScaleTop.TabIndex = 38;
			this.pfcBackScaleTop.OnValuesChanged += new System.EventHandler(this.pfcBackScaleTop_OnValuesChanged);
			// 
			// pfcBackScaleBottom
			// 
			this.pfcBackScaleBottom.AllowDecimal = true;
			this.pfcBackScaleBottom.AllowValueLinking = true;
			this.pfcBackScaleBottom.ButtonStep = 1;
			this.pfcBackScaleBottom.ButtonStepBig = 1F;
			this.pfcBackScaleBottom.ButtonStepFloat = 0.1F;
			this.pfcBackScaleBottom.ButtonStepSmall = 0.01F;
			this.pfcBackScaleBottom.ButtonStepsUseModifierKeys = true;
			this.pfcBackScaleBottom.DefaultValue = 1F;
			this.pfcBackScaleBottom.Field1 = "scalex_bottom";
			this.pfcBackScaleBottom.Field2 = "scaley_bottom";
			this.pfcBackScaleBottom.LinkValues = false;
			this.pfcBackScaleBottom.Location = new System.Drawing.Point(96, 81);
			this.pfcBackScaleBottom.Name = "pfcBackScaleBottom";
			this.pfcBackScaleBottom.Size = new System.Drawing.Size(186, 26);
			this.pfcBackScaleBottom.TabIndex = 40;
			this.pfcBackScaleBottom.OnValuesChanged += new System.EventHandler(this.pfcBackScaleBottom_OnValuesChanged);
			// 
			// pfcBackScaleMid
			// 
			this.pfcBackScaleMid.AllowDecimal = true;
			this.pfcBackScaleMid.AllowValueLinking = true;
			this.pfcBackScaleMid.ButtonStep = 1;
			this.pfcBackScaleMid.ButtonStepBig = 1F;
			this.pfcBackScaleMid.ButtonStepFloat = 0.1F;
			this.pfcBackScaleMid.ButtonStepSmall = 0.01F;
			this.pfcBackScaleMid.ButtonStepsUseModifierKeys = true;
			this.pfcBackScaleMid.DefaultValue = 1F;
			this.pfcBackScaleMid.Field1 = "scalex_mid";
			this.pfcBackScaleMid.Field2 = "scaley_mid";
			this.pfcBackScaleMid.LinkValues = false;
			this.pfcBackScaleMid.Location = new System.Drawing.Point(96, 49);
			this.pfcBackScaleMid.Name = "pfcBackScaleMid";
			this.pfcBackScaleMid.Size = new System.Drawing.Size(186, 26);
			this.pfcBackScaleMid.TabIndex = 39;
			this.pfcBackScaleMid.OnValuesChanged += new System.EventHandler(this.pfcBackScaleMid_OnValuesChanged);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.labelBackTextureOffset);
			this.groupBox1.Controls.Add(this.labelBackOffsetBottom);
			this.groupBox1.Controls.Add(this.labelBackOffsetMid);
			this.groupBox1.Controls.Add(this.labelBackOffsetTop);
			this.groupBox1.Controls.Add(this.pfcBackOffsetTop);
			this.groupBox1.Controls.Add(this.pfcBackOffsetMid);
			this.groupBox1.Controls.Add(this.pfcBackOffsetBottom);
			this.groupBox1.Controls.Add(this.backTextureOffset);
			this.groupBox1.Location = new System.Drawing.Point(12, 228);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(290, 143);
			this.groupBox1.TabIndex = 43;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = " Texture offsets ";
			// 
			// labelBackTextureOffset
			// 
			this.labelBackTextureOffset.Location = new System.Drawing.Point(14, 23);
			this.labelBackTextureOffset.Name = "labelBackTextureOffset";
			this.labelBackTextureOffset.Size = new System.Drawing.Size(80, 14);
			this.labelBackTextureOffset.TabIndex = 50;
			this.labelBackTextureOffset.Tag = "";
			this.labelBackTextureOffset.Text = "Sidedef offset:";
			this.labelBackTextureOffset.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// labelBackOffsetBottom
			// 
			this.labelBackOffsetBottom.Location = new System.Drawing.Point(14, 118);
			this.labelBackOffsetBottom.Name = "labelBackOffsetBottom";
			this.labelBackOffsetBottom.Size = new System.Drawing.Size(80, 14);
			this.labelBackOffsetBottom.TabIndex = 49;
			this.labelBackOffsetBottom.Tag = "";
			this.labelBackOffsetBottom.Text = "Lower offset:";
			this.labelBackOffsetBottom.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// labelBackOffsetMid
			// 
			this.labelBackOffsetMid.Location = new System.Drawing.Point(14, 86);
			this.labelBackOffsetMid.Name = "labelBackOffsetMid";
			this.labelBackOffsetMid.Size = new System.Drawing.Size(80, 14);
			this.labelBackOffsetMid.TabIndex = 48;
			this.labelBackOffsetMid.Tag = "";
			this.labelBackOffsetMid.Text = "Middle offset:";
			this.labelBackOffsetMid.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// labelBackOffsetTop
			// 
			this.labelBackOffsetTop.Location = new System.Drawing.Point(14, 54);
			this.labelBackOffsetTop.Name = "labelBackOffsetTop";
			this.labelBackOffsetTop.Size = new System.Drawing.Size(80, 14);
			this.labelBackOffsetTop.TabIndex = 47;
			this.labelBackOffsetTop.Tag = "";
			this.labelBackOffsetTop.Text = "Upper offset:";
			this.labelBackOffsetTop.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// pfcBackOffsetTop
			// 
			this.pfcBackOffsetTop.AllowDecimal = true;
			this.pfcBackOffsetTop.AllowValueLinking = false;
			this.pfcBackOffsetTop.ButtonStep = 1;
			this.pfcBackOffsetTop.ButtonStepBig = 16F;
			this.pfcBackOffsetTop.ButtonStepFloat = 1F;
			this.pfcBackOffsetTop.ButtonStepSmall = 0.1F;
			this.pfcBackOffsetTop.ButtonStepsUseModifierKeys = true;
			this.pfcBackOffsetTop.DefaultValue = 0F;
			this.pfcBackOffsetTop.Field1 = "offsetx_top";
			this.pfcBackOffsetTop.Field2 = "offsety_top";
			this.pfcBackOffsetTop.LinkValues = false;
			this.pfcBackOffsetTop.Location = new System.Drawing.Point(96, 49);
			this.pfcBackOffsetTop.Name = "pfcBackOffsetTop";
			this.pfcBackOffsetTop.Size = new System.Drawing.Size(186, 26);
			this.pfcBackOffsetTop.TabIndex = 35;
			this.pfcBackOffsetTop.OnValuesChanged += new System.EventHandler(this.pfcBackOffsetTop_OnValuesChanged);
			// 
			// pfcBackOffsetMid
			// 
			this.pfcBackOffsetMid.AllowDecimal = true;
			this.pfcBackOffsetMid.AllowValueLinking = false;
			this.pfcBackOffsetMid.ButtonStep = 1;
			this.pfcBackOffsetMid.ButtonStepBig = 16F;
			this.pfcBackOffsetMid.ButtonStepFloat = 1F;
			this.pfcBackOffsetMid.ButtonStepSmall = 0.1F;
			this.pfcBackOffsetMid.ButtonStepsUseModifierKeys = true;
			this.pfcBackOffsetMid.DefaultValue = 0F;
			this.pfcBackOffsetMid.Field1 = "offsetx_mid";
			this.pfcBackOffsetMid.Field2 = "offsety_mid";
			this.pfcBackOffsetMid.LinkValues = false;
			this.pfcBackOffsetMid.Location = new System.Drawing.Point(96, 81);
			this.pfcBackOffsetMid.Name = "pfcBackOffsetMid";
			this.pfcBackOffsetMid.Size = new System.Drawing.Size(186, 26);
			this.pfcBackOffsetMid.TabIndex = 36;
			this.pfcBackOffsetMid.OnValuesChanged += new System.EventHandler(this.pfcBackOffsetMid_OnValuesChanged);
			// 
			// pfcBackOffsetBottom
			// 
			this.pfcBackOffsetBottom.AllowDecimal = true;
			this.pfcBackOffsetBottom.AllowValueLinking = false;
			this.pfcBackOffsetBottom.ButtonStep = 1;
			this.pfcBackOffsetBottom.ButtonStepBig = 16F;
			this.pfcBackOffsetBottom.ButtonStepFloat = 1F;
			this.pfcBackOffsetBottom.ButtonStepSmall = 0.1F;
			this.pfcBackOffsetBottom.ButtonStepsUseModifierKeys = true;
			this.pfcBackOffsetBottom.DefaultValue = 0F;
			this.pfcBackOffsetBottom.Field1 = "offsetx_bottom";
			this.pfcBackOffsetBottom.Field2 = "offsety_bottom";
			this.pfcBackOffsetBottom.LinkValues = false;
			this.pfcBackOffsetBottom.Location = new System.Drawing.Point(96, 113);
			this.pfcBackOffsetBottom.Name = "pfcBackOffsetBottom";
			this.pfcBackOffsetBottom.Size = new System.Drawing.Size(186, 26);
			this.pfcBackOffsetBottom.TabIndex = 37;
			this.pfcBackOffsetBottom.OnValuesChanged += new System.EventHandler(this.pfcBackOffsetBottom_OnValuesChanged);
			// 
			// backTextureOffset
			// 
			this.backTextureOffset.ButtonStep = 1;
			this.backTextureOffset.ButtonStepBig = 16F;
			this.backTextureOffset.ButtonStepSmall = 0.1F;
			this.backTextureOffset.ButtonStepsUseModifierKeys = true;
			this.backTextureOffset.DefaultValue = 0;
			this.backTextureOffset.Location = new System.Drawing.Point(96, 17);
			this.backTextureOffset.Name = "backTextureOffset";
			this.backTextureOffset.Size = new System.Drawing.Size(186, 26);
			this.backTextureOffset.TabIndex = 42;
			this.backTextureOffset.OnValuesChanged += new System.EventHandler(this.backTextureOffset_OnValuesChanged);
			// 
			// backlow
			// 
			this.backlow.Location = new System.Drawing.Point(309, 409);
			this.backlow.MultipleTextures = false;
			this.backlow.Name = "backlow";
			this.backlow.Required = false;
			this.backlow.Size = new System.Drawing.Size(220, 189);
			this.backlow.TabIndex = 6;
			this.backlow.TextureName = "";
			this.backlow.OnValueChanged += new System.EventHandler(this.backlow_OnValueChanged);
			// 
			// backmid
			// 
			this.backmid.Location = new System.Drawing.Point(309, 214);
			this.backmid.MultipleTextures = false;
			this.backmid.Name = "backmid";
			this.backmid.Required = false;
			this.backmid.Size = new System.Drawing.Size(220, 189);
			this.backmid.TabIndex = 5;
			this.backmid.TextureName = "";
			this.backmid.OnValueChanged += new System.EventHandler(this.backmid_OnValueChanged);
			// 
			// backhigh
			// 
			this.backhigh.Location = new System.Drawing.Point(309, 19);
			this.backhigh.MultipleTextures = false;
			this.backhigh.Name = "backhigh";
			this.backhigh.Required = false;
			this.backhigh.Size = new System.Drawing.Size(220, 189);
			this.backhigh.TabIndex = 4;
			this.backhigh.TextureName = "";
			this.backhigh.OnValueChanged += new System.EventHandler(this.backhigh_OnValueChanged);
			// 
			// tabcomment
			// 
			this.tabcomment.Controls.Add(this.commenteditor);
			this.tabcomment.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.tabcomment.Location = new System.Drawing.Point(4, 23);
			this.tabcomment.Name = "tabcomment";
			this.tabcomment.Size = new System.Drawing.Size(549, 621);
			this.tabcomment.TabIndex = 4;
			this.tabcomment.Text = "Comment";
			this.tabcomment.UseVisualStyleBackColor = true;
			// 
			// commenteditor
			// 
			this.commenteditor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.commenteditor.Location = new System.Drawing.Point(3, 3);
			this.commenteditor.Name = "commenteditor";
			this.commenteditor.Size = new System.Drawing.Size(543, 615);
			this.commenteditor.TabIndex = 0;
			// 
			// tabcustom
			// 
			this.tabcustom.Controls.Add(this.fieldslist);
			this.tabcustom.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tabcustom.Location = new System.Drawing.Point(4, 23);
			this.tabcustom.Name = "tabcustom";
			this.tabcustom.Padding = new System.Windows.Forms.Padding(3);
			this.tabcustom.Size = new System.Drawing.Size(549, 621);
			this.tabcustom.TabIndex = 2;
			this.tabcustom.Text = "Custom";
			this.tabcustom.UseVisualStyleBackColor = true;
			this.tabcustom.MouseEnter += new System.EventHandler(this.tabcustom_MouseEnter);
			// 
			// fieldslist
			// 
			this.fieldslist.AllowInsert = true;
			this.fieldslist.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.fieldslist.AutoInsertUserPrefix = true;
			this.fieldslist.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.fieldslist.Location = new System.Drawing.Point(11, 11);
			this.fieldslist.Margin = new System.Windows.Forms.Padding(8);
			this.fieldslist.Name = "fieldslist";
			this.fieldslist.PropertyColumnVisible = true;
			this.fieldslist.PropertyColumnWidth = 150;
			this.fieldslist.ShowFixedFields = true;
			this.fieldslist.Size = new System.Drawing.Size(527, 602);
			this.fieldslist.TabIndex = 0;
			this.fieldslist.TypeColumnVisible = true;
			this.fieldslist.TypeColumnWidth = 100;
			this.fieldslist.ValueColumnVisible = true;
			// 
			// imagelist
			// 
			this.imagelist.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imagelist.ImageStream")));
			this.imagelist.TransparentColor = System.Drawing.Color.Transparent;
			this.imagelist.Images.SetKeyName(0, "Check.png");
			this.imagelist.Images.SetKeyName(1, "SearchClear.png");
			// 
			// LinedefEditFormUDMF
			// 
			this.AcceptButton = this.apply;
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.CancelButton = this.cancel;
			this.ClientSize = new System.Drawing.Size(577, 697);
			this.Controls.Add(this.tabs);
			this.Controls.Add(this.cancel);
			this.Controls.Add(this.apply);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "LinedefEditFormUDMF";
			this.Opacity = 0D;
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Edit Linedef";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.LinedefEditForm_FormClosing);
			this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.LinedefEditForm_HelpRequested);
			this.actiongroup.ResumeLayout(false);
			this.actiongroup.PerformLayout();
			this.flagsgroup.ResumeLayout(false);
			this.tabs.ResumeLayout(false);
			this.tabproperties.ResumeLayout(false);
			this.groupsettings.ResumeLayout(false);
			this.groupsettings.PerformLayout();
			this.activationGroup.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.missingactivation)).EndInit();
			this.idgroup.ResumeLayout(false);
			this.tabfront.ResumeLayout(false);
			this.tabfront.PerformLayout();
			this.frontgroup.ResumeLayout(false);
			this.frontflagsgroup.ResumeLayout(false);
			this.frontscalegroup.ResumeLayout(false);
			this.groupBox6.ResumeLayout(false);
			this.groupBox5.ResumeLayout(false);
			this.groupBox5.PerformLayout();
			this.tabback.ResumeLayout(false);
			this.tabback.PerformLayout();
			this.backgroup.ResumeLayout(false);
			this.groupBox4.ResumeLayout(false);
			this.groupBox4.PerformLayout();
			this.backflagsgroup.ResumeLayout(false);
			this.backscalegroup.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.tabcomment.ResumeLayout(false);
			this.tabcustom.ResumeLayout(false);
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
		private System.Windows.Forms.TabControl tabs;
		private System.Windows.Forms.TabPage tabproperties;
		private System.Windows.Forms.TabPage tabfront;
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
		private System.Windows.Forms.TabPage tabcustom;
		private CodeImp.DoomBuilder.Controls.FieldsEditorControl fieldslist;
		private System.Windows.Forms.GroupBox idgroup;
		private CodeImp.DoomBuilder.Controls.CheckboxArrayControl udmfactivates;
		private System.Windows.Forms.Button customfrontbutton;
		private System.Windows.Forms.Button custombackbutton;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox frontsector;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox backsector;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox lightFront;
		private System.Windows.Forms.CheckBox cbLightAbsoluteFront;
		private CodeImp.DoomBuilder.Controls.PairedFieldsControl pfcFrontOffsetTop;
		private CodeImp.DoomBuilder.Controls.PairedFieldsControl pfcFrontOffsetBottom;
		private CodeImp.DoomBuilder.Controls.PairedFieldsControl pfcFrontOffsetMid;
		private CodeImp.DoomBuilder.Controls.PairedFieldsControl pfcFrontScaleBottom;
		private CodeImp.DoomBuilder.Controls.PairedFieldsControl pfcFrontScaleMid;
		private CodeImp.DoomBuilder.Controls.PairedFieldsControl pfcFrontScaleTop;
		private CodeImp.DoomBuilder.Controls.PairedFieldsControl pfcBackScaleBottom;
		private CodeImp.DoomBuilder.Controls.PairedFieldsControl pfcBackScaleMid;
		private CodeImp.DoomBuilder.Controls.PairedFieldsControl pfcBackScaleTop;
		private CodeImp.DoomBuilder.Controls.PairedFieldsControl pfcBackOffsetBottom;
		private CodeImp.DoomBuilder.Controls.PairedFieldsControl pfcBackOffsetMid;
		private CodeImp.DoomBuilder.Controls.PairedFieldsControl pfcBackOffsetTop;
		private System.Windows.Forms.GroupBox groupsettings;
		private System.Windows.Forms.ComboBox renderStyle;
		private System.Windows.Forms.Label labelLightFront;
		private System.Windows.Forms.CheckBox cbLightAbsoluteBack;
		private System.Windows.Forms.Label labelLightBack;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox lightBack;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox alpha;
		private CodeImp.DoomBuilder.Controls.PairedIntControl frontTextureOffset;
		private CodeImp.DoomBuilder.Controls.PairedIntControl backTextureOffset;
		private CodeImp.DoomBuilder.Controls.CheckboxArrayControl flagsFront;
		private CodeImp.DoomBuilder.Controls.CheckboxArrayControl flagsBack;
		private System.Windows.Forms.GroupBox activationGroup;
		private System.Windows.Forms.PictureBox missingactivation;
		private System.Windows.Forms.ToolTip tooltip;
		private System.Windows.Forms.ComboBox lockpick;
		private System.Windows.Forms.TabPage tabback;
		private System.Windows.Forms.GroupBox backflagsgroup;
		private System.Windows.Forms.GroupBox backscalegroup;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.GroupBox groupBox5;
		private System.Windows.Forms.GroupBox frontflagsgroup;
		private System.Windows.Forms.GroupBox frontscalegroup;
		private System.Windows.Forms.GroupBox groupBox6;
		private System.Windows.Forms.ImageList imagelist;
		private System.Windows.Forms.Label labelrenderstyle;
		private System.Windows.Forms.Label labellockpick;
		private CodeImp.DoomBuilder.Controls.ActionSpecialHelpButton actionhelp;
		private System.Windows.Forms.Label labelFrontScaleMid;
		private System.Windows.Forms.Label labelFrontScaleTop;
		private System.Windows.Forms.Label labelFrontScaleBottom;
		private System.Windows.Forms.Label labelFrontOffsetBottom;
		private System.Windows.Forms.Label labelFrontOffsetMid;
		private System.Windows.Forms.Label labelFrontOffsetTop;
		private System.Windows.Forms.Label labelFrontTextureOffset;
		private System.Windows.Forms.Label labelBackScaleBottom;
		private System.Windows.Forms.Label labelBackScaleMid;
		private System.Windows.Forms.Label labelBackScaleTop;
		private System.Windows.Forms.Label labelBackTextureOffset;
		private System.Windows.Forms.Label labelBackOffsetBottom;
		private System.Windows.Forms.Label labelBackOffsetMid;
		private System.Windows.Forms.Label labelBackOffsetTop;
		private System.Windows.Forms.TabPage tabcomment;
		private CodeImp.DoomBuilder.Controls.CommentEditor commenteditor;
		private CodeImp.DoomBuilder.Controls.ArgumentsControl argscontrol;
		private CodeImp.DoomBuilder.Controls.TagsSelector tagsselector;
		private System.Windows.Forms.Button resetfrontlight;
		private System.Windows.Forms.Button resetbacklight;
		private System.Windows.Forms.Button resetalpha;
		private Controls.SidedefPartLightControl lightfrontupper;
		private Controls.SidedefPartLightControl lightfrontlower;
		private Controls.SidedefPartLightControl lightfrontmiddle;
		private Controls.SidedefPartLightControl lightbacklower;
		private Controls.SidedefPartLightControl lightbackmiddle;
		private Controls.SidedefPartLightControl lightbackupper;
	}
}