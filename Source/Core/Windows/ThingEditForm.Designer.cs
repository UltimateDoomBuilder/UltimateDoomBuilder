namespace CodeImp.DoomBuilder.Windows
{
	partial class ThingEditForm
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
            System.Windows.Forms.GroupBox groupBox2;
            System.Windows.Forms.Label label7;
            this.cbAbsoluteHeight = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label2.AutoSize = true;
            this.label1 = new System.Windows.Forms.Label();
            this.label1.AutoSize = true;
            this.posX = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
            this.posY = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
            this.posZ = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
            this.zlabel = new System.Windows.Forms.Label();
            this.zlabel.AutoSize = true;
            this.typegroup = new System.Windows.Forms.GroupBox();
            this.thingtype = new CodeImp.DoomBuilder.Controls.ThingBrowserControl();
            this.anglecontrol = new CodeImp.DoomBuilder.Controls.AngleControlEx();
            this.cbRandomAngle = new System.Windows.Forms.CheckBox();
            this.angle = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.settingsgroup = new System.Windows.Forms.GroupBox();
            this.missingflags = new System.Windows.Forms.PictureBox();
            this.flags = new CodeImp.DoomBuilder.Controls.CheckboxArrayControl();
            this.actiongroup = new System.Windows.Forms.GroupBox();
            this.argscontrol = new CodeImp.DoomBuilder.Controls.ArgumentsControl();
            this.actionhelp = new CodeImp.DoomBuilder.Controls.ActionSpecialHelpButton();
            this.action = new CodeImp.DoomBuilder.Controls.ActionSelectorControl();
            this.browseaction = new System.Windows.Forms.Button();
            this.idgroup = new System.Windows.Forms.GroupBox();
            this.tagSelector = new CodeImp.DoomBuilder.Controls.TagSelector();
            this.cancel = new System.Windows.Forms.Button();
            this.apply = new System.Windows.Forms.Button();
            this.hint = new System.Windows.Forms.PictureBox();
            this.hintlabel = new System.Windows.Forms.Label();
            this.hintlabel.AutoSize = true;
            this.tooltip = new System.Windows.Forms.ToolTip(this.components);
            this.panel = new System.Windows.Forms.Panel();
            this.applypanel = new System.Windows.Forms.Panel();
            groupBox2 = new System.Windows.Forms.GroupBox();
            label7 = new System.Windows.Forms.Label();
            groupBox2.SuspendLayout();
            this.typegroup.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.settingsgroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.missingflags)).BeginInit();
            this.actiongroup.SuspendLayout();
            this.idgroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.hint)).BeginInit();
            this.panel.SuspendLayout();
            this.applypanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(this.cbAbsoluteHeight);
            groupBox2.Controls.Add(this.label2);
            groupBox2.Controls.Add(this.label1);
            groupBox2.Controls.Add(this.posX);
            groupBox2.Controls.Add(this.posY);
            groupBox2.Controls.Add(this.posZ);
            groupBox2.Controls.Add(this.zlabel);
            groupBox2.Location = new System.Drawing.Point(260, 242);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new System.Drawing.Size(177, 134);
            groupBox2.TabIndex = 2;
            groupBox2.TabStop = false;
            groupBox2.Text = " Position ";
            // 
            // cbAbsoluteHeight
            // 
            this.cbAbsoluteHeight.AutoSize = true;
            this.cbAbsoluteHeight.Location = new System.Drawing.Point(12, 111);
            this.cbAbsoluteHeight.Name = "cbAbsoluteHeight";
            this.cbAbsoluteHeight.Size = new System.Drawing.Size(99, 17);
            this.cbAbsoluteHeight.TabIndex = 16;
            this.cbAbsoluteHeight.Text = "Absolute height";
            this.cbAbsoluteHeight.UseVisualStyleBackColor = true;
            this.cbAbsoluteHeight.CheckedChanged += new System.EventHandler(this.cbAbsoluteHeight_CheckedChanged);
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(5, 21);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(50, 14);
            this.label2.TabIndex = 15;
            this.label2.Text = "X:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(5, 51);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(50, 14);
            this.label1.TabIndex = 14;
            this.label1.Text = "Y:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // posX
            // 
            this.posX.AllowDecimal = false;
            this.posX.AllowExpressions = true;
            this.posX.AllowNegative = true;
            this.posX.AllowRelative = true;
            this.posX.ButtonStep = 8;
            this.posX.ButtonStepBig = 16F;
            this.posX.ButtonStepFloat = 1F;
            this.posX.ButtonStepSmall = 1F;
            this.posX.ButtonStepsUseModifierKeys = true;
            this.posX.ButtonStepsWrapAround = false;
            this.posX.Location = new System.Drawing.Point(61, 16);
            this.posX.Name = "posX";
            this.posX.Size = new System.Drawing.Size(72, 24);
            this.posX.StepValues = null;
            this.posX.TabIndex = 13;
            this.posX.WhenTextChanged += new System.EventHandler(this.posX_WhenTextChanged);
            // 
            // posY
            // 
            this.posY.AllowDecimal = false;
            this.posY.AllowExpressions = true;
            this.posY.AllowNegative = true;
            this.posY.AllowRelative = true;
            this.posY.ButtonStep = 8;
            this.posY.ButtonStepBig = 16F;
            this.posY.ButtonStepFloat = 1F;
            this.posY.ButtonStepSmall = 1F;
            this.posY.ButtonStepsUseModifierKeys = true;
            this.posY.ButtonStepsWrapAround = false;
            this.posY.Location = new System.Drawing.Point(61, 46);
            this.posY.Name = "posY";
            this.posY.Size = new System.Drawing.Size(72, 24);
            this.posY.StepValues = null;
            this.posY.TabIndex = 12;
            this.posY.WhenTextChanged += new System.EventHandler(this.posY_WhenTextChanged);
            // 
            // posZ
            // 
            this.posZ.AllowDecimal = false;
            this.posZ.AllowExpressions = true;
            this.posZ.AllowNegative = true;
            this.posZ.AllowRelative = true;
            this.posZ.ButtonStep = 8;
            this.posZ.ButtonStepBig = 16F;
            this.posZ.ButtonStepFloat = 1F;
            this.posZ.ButtonStepSmall = 1F;
            this.posZ.ButtonStepsUseModifierKeys = true;
            this.posZ.ButtonStepsWrapAround = false;
            this.posZ.Location = new System.Drawing.Point(61, 76);
            this.posZ.Name = "posZ";
            this.posZ.Size = new System.Drawing.Size(72, 24);
            this.posZ.StepValues = null;
            this.posZ.TabIndex = 11;
            this.posZ.WhenTextChanged += new System.EventHandler(this.posZ_WhenTextChanged);
            // 
            // zlabel
            // 
            this.zlabel.Location = new System.Drawing.Point(5, 81);
            this.zlabel.Name = "zlabel";
            this.zlabel.Size = new System.Drawing.Size(50, 14);
            this.zlabel.TabIndex = 9;
            this.zlabel.Text = "Height:";
            this.zlabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new System.Drawing.Point(15, 30);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(40, 13);
            label7.TabIndex = 9;
            label7.Text = "Action:";
            // 
            // typegroup
            // 
            this.typegroup.Controls.Add(this.thingtype);
            this.typegroup.Location = new System.Drawing.Point(4, 3);
            this.typegroup.Name = "typegroup";
            this.typegroup.Size = new System.Drawing.Size(250, 373);
            this.typegroup.TabIndex = 0;
            this.typegroup.TabStop = false;
            this.typegroup.Text = " Thing ";
            // 
            // thingtype
            // 
            this.thingtype.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.thingtype.Location = new System.Drawing.Point(9, 13);
            this.thingtype.Margin = new System.Windows.Forms.Padding(6);
            this.thingtype.Name = "thingtype";
            this.thingtype.Size = new System.Drawing.Size(232, 357);
            this.thingtype.TabIndex = 0;
            this.thingtype.UseMultiSelection = true;
            this.thingtype.OnTypeChanged += new CodeImp.DoomBuilder.Controls.ThingBrowserControl.TypeChangedDeletegate(this.thingtype_OnTypeChanged);
            this.thingtype.OnTypeDoubleClicked += new CodeImp.DoomBuilder.Controls.ThingBrowserControl.TypeDoubleClickDeletegate(this.thingtype_OnTypeDoubleClicked);
            // 
            // anglecontrol
            // 
            this.anglecontrol.Angle = 0;
            this.anglecontrol.AngleOffset = 0;
            this.anglecontrol.DoomAngleClamping = false;
            this.anglecontrol.Location = new System.Drawing.Point(20, 40);
            this.anglecontrol.Name = "anglecontrol";
            this.anglecontrol.Size = new System.Drawing.Size(69, 69);
            this.anglecontrol.TabIndex = 20;
            this.anglecontrol.AngleChanged += new System.EventHandler(this.anglecontrol_AngleChanged);
            // 
            // cbRandomAngle
            // 
            this.cbRandomAngle.AutoSize = true;
            this.cbRandomAngle.Location = new System.Drawing.Point(6, 111);
            this.cbRandomAngle.Name = "cbRandomAngle";
            this.cbRandomAngle.Size = new System.Drawing.Size(95, 17);
            this.cbRandomAngle.TabIndex = 17;
            this.cbRandomAngle.Text = "Random angle";
            this.cbRandomAngle.UseVisualStyleBackColor = true;
            this.cbRandomAngle.CheckedChanged += new System.EventHandler(this.cbRandomAngle_CheckedChanged);
            // 
            // angle
            // 
            this.angle.AllowDecimal = false;
            this.angle.AllowExpressions = false;
            this.angle.AllowNegative = true;
            this.angle.AllowRelative = true;
            this.angle.ButtonStep = 5;
            this.angle.ButtonStepBig = 15F;
            this.angle.ButtonStepFloat = 1F;
            this.angle.ButtonStepSmall = 1F;
            this.angle.ButtonStepsUseModifierKeys = true;
            this.angle.ButtonStepsWrapAround = false;
            this.angle.Location = new System.Drawing.Point(13, 16);
            this.angle.Name = "angle";
            this.angle.Size = new System.Drawing.Size(82, 24);
            this.angle.StepValues = null;
            this.angle.TabIndex = 10;
            this.angle.WhenTextChanged += new System.EventHandler(this.angle_WhenTextChanged);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.cbRandomAngle);
            this.groupBox4.Controls.Add(this.anglecontrol);
            this.groupBox4.Controls.Add(this.angle);
            this.groupBox4.Location = new System.Drawing.Point(443, 242);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(107, 134);
            this.groupBox4.TabIndex = 3;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = " Angle";
            // 
            // settingsgroup
            // 
            this.settingsgroup.Controls.Add(this.missingflags);
            this.settingsgroup.Controls.Add(this.flags);
            this.settingsgroup.Location = new System.Drawing.Point(260, 3);
            this.settingsgroup.Name = "settingsgroup";
            this.settingsgroup.Size = new System.Drawing.Size(290, 233);
            this.settingsgroup.TabIndex = 1;
            this.settingsgroup.TabStop = false;
            this.settingsgroup.Text = " Settings ";
            // 
            // missingflags
            // 
            this.missingflags.BackColor = System.Drawing.SystemColors.Window;
            this.missingflags.Image = global::CodeImp.DoomBuilder.Properties.Resources.Warning;
            this.missingflags.Location = new System.Drawing.Point(55, -2);
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
            this.flags.Location = new System.Drawing.Point(14, 19);
            this.flags.Name = "flags";
            this.flags.Size = new System.Drawing.Size(270, 211);
            this.flags.TabIndex = 0;
            this.flags.VerticalSpacing = 1;
            this.flags.OnValueChanged += new System.EventHandler(this.flags_OnValueChanged);
            // 
            // actiongroup
            // 
            this.actiongroup.Controls.Add(this.argscontrol);
            this.actiongroup.Controls.Add(this.actionhelp);
            this.actiongroup.Controls.Add(label7);
            this.actiongroup.Controls.Add(this.action);
            this.actiongroup.Controls.Add(this.browseaction);
            this.actiongroup.Location = new System.Drawing.Point(4, 382);
            this.actiongroup.Name = "actiongroup";
            this.actiongroup.Size = new System.Drawing.Size(546, 145);
            this.actiongroup.TabIndex = 22;
            this.actiongroup.TabStop = false;
            this.actiongroup.Text = " Action ";
            // 
            // argscontrol
            // 
            this.argscontrol.Location = new System.Drawing.Point(6, 57);
            this.argscontrol.Name = "argscontrol";
            this.argscontrol.Size = new System.Drawing.Size(534, 80);
            this.argscontrol.TabIndex = 15;
            // 
            // actionhelp
            // 
            this.actionhelp.Location = new System.Drawing.Point(512, 25);
            this.actionhelp.Name = "actionhelp";
            this.actionhelp.Size = new System.Drawing.Size(28, 25);
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
            this.action.Size = new System.Drawing.Size(414, 21);
            this.action.TabIndex = 0;
            this.action.Value = 402;
            this.action.ValueChanges += new System.EventHandler(this.action_ValueChanges);
            // 
            // browseaction
            // 
            this.browseaction.Image = global::CodeImp.DoomBuilder.Properties.Resources.List;
            this.browseaction.Location = new System.Drawing.Point(482, 25);
            this.browseaction.Name = "browseaction";
            this.browseaction.Size = new System.Drawing.Size(28, 25);
            this.browseaction.TabIndex = 1;
            this.browseaction.Text = " ";
            this.tooltip.SetToolTip(this.browseaction, "Browse Action");
            this.browseaction.UseVisualStyleBackColor = true;
            this.browseaction.Click += new System.EventHandler(this.browseaction_Click);
            // 
            // idgroup
            // 
            this.idgroup.Controls.Add(this.tagSelector);
            this.idgroup.Location = new System.Drawing.Point(4, 533);
            this.idgroup.Name = "idgroup";
            this.idgroup.Size = new System.Drawing.Size(546, 66);
            this.idgroup.TabIndex = 0;
            this.idgroup.TabStop = false;
            this.idgroup.Text = " Identification ";
            // 
            // tagSelector
            // 
            this.tagSelector.Location = new System.Drawing.Point(6, 19);
            this.tagSelector.Name = "tagSelector";
            this.tagSelector.Size = new System.Drawing.Size(534, 35);
            this.tagSelector.TabIndex = 8;
            // 
            // cancel
            // 
            this.cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancel.Location = new System.Drawing.Point(438, 4);
            this.cancel.Name = "cancel";
            this.cancel.Size = new System.Drawing.Size(112, 25);
            this.cancel.TabIndex = 2;
            this.cancel.Text = "Cancel";
            this.cancel.UseVisualStyleBackColor = true;
            this.cancel.Click += new System.EventHandler(this.cancel_Click);
            // 
            // apply
            // 
            this.apply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.apply.Location = new System.Drawing.Point(320, 4);
            this.apply.Name = "apply";
            this.apply.Size = new System.Drawing.Size(112, 25);
            this.apply.TabIndex = 1;
            this.apply.Text = "OK";
            this.apply.UseVisualStyleBackColor = true;
            this.apply.Click += new System.EventHandler(this.apply_Click);
            // 
            // hint
            // 
            this.hint.Image = global::CodeImp.DoomBuilder.Properties.Resources.Lightbulb;
            this.hint.Location = new System.Drawing.Point(0, 8);
            this.hint.Name = "hint";
            this.hint.Size = new System.Drawing.Size(16, 16);
            this.hint.TabIndex = 3;
            this.hint.TabStop = false;
            // 
            // hintlabel
            // 
            this.hintlabel.AutoSize = true;
            this.hintlabel.Location = new System.Drawing.Point(18, 3);
            this.hintlabel.Name = "hintlabel";
            this.hintlabel.Size = new System.Drawing.Size(195, 26);
            this.hintlabel.TabIndex = 4;
            this.hintlabel.Text = "Select categories or several thing types \r\nto randomly assign them to selection";
            // 
            // panel
            // 
            this.panel.BackColor = System.Drawing.SystemColors.Control;
            this.panel.Controls.Add(this.actiongroup);
            this.panel.Controls.Add(this.groupBox4);
            this.panel.Controls.Add(this.idgroup);
            this.panel.Controls.Add(this.typegroup);
            this.panel.Controls.Add(groupBox2);
            this.panel.Controls.Add(this.settingsgroup);
            this.panel.Location = new System.Drawing.Point(12, 12);
            this.panel.Name = "panel";
            this.panel.Size = new System.Drawing.Size(553, 606);
            this.panel.TabIndex = 5;
            // 
            // applypanel
            // 
            this.applypanel.Controls.Add(this.cancel);
            this.applypanel.Controls.Add(this.apply);
            this.applypanel.Controls.Add(this.hintlabel);
            this.applypanel.Controls.Add(this.hint);
            this.applypanel.Location = new System.Drawing.Point(12, 624);
            this.applypanel.Name = "applypanel";
            this.applypanel.Size = new System.Drawing.Size(553, 32);
            this.applypanel.TabIndex = 6;
            // 
            // ThingEditForm
            // 
            this.AcceptButton = this.apply;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.cancel;
            this.ClientSize = new System.Drawing.Size(577, 670);
            this.Controls.Add(this.applypanel);
            this.Controls.Add(this.panel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ThingEditForm";
            this.Opacity = 0D;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Edit Thing";
            this.Shown += new System.EventHandler(this.ThingEditForm_Shown);
            this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.ThingEditForm_HelpRequested);
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            this.typegroup.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.settingsgroup.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.missingflags)).EndInit();
            this.actiongroup.ResumeLayout(false);
            this.actiongroup.PerformLayout();
            this.idgroup.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.hint)).EndInit();
            this.panel.ResumeLayout(false);
            this.applypanel.ResumeLayout(false);
            this.applypanel.PerformLayout();
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button cancel;
		private System.Windows.Forms.Button apply;
		private System.Windows.Forms.GroupBox settingsgroup;
		private CodeImp.DoomBuilder.Controls.CheckboxArrayControl flags;
		private System.Windows.Forms.GroupBox idgroup;
		private System.Windows.Forms.GroupBox actiongroup;
		private CodeImp.DoomBuilder.Controls.ActionSelectorControl action;
		private System.Windows.Forms.Button browseaction;
		private System.Windows.Forms.Label zlabel;
		private CodeImp.DoomBuilder.Controls.ThingBrowserControl thingtype;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox angle;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox posZ;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox posY;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox posX;
		private System.Windows.Forms.CheckBox cbAbsoluteHeight;
		private System.Windows.Forms.CheckBox cbRandomAngle;
		private CodeImp.DoomBuilder.Controls.TagSelector tagSelector;
		private CodeImp.DoomBuilder.Controls.AngleControlEx anglecontrol;
		private System.Windows.Forms.PictureBox hint;
		private System.Windows.Forms.Label hintlabel;
		private System.Windows.Forms.PictureBox missingflags;
		private System.Windows.Forms.ToolTip tooltip;
		private System.Windows.Forms.GroupBox groupBox4;
		private CodeImp.DoomBuilder.Controls.ActionSpecialHelpButton actionhelp;
		private System.Windows.Forms.Panel panel;
		private System.Windows.Forms.GroupBox typegroup;
		private System.Windows.Forms.Panel applypanel;
		private CodeImp.DoomBuilder.Controls.ArgumentsControl argscontrol;
	}
}
