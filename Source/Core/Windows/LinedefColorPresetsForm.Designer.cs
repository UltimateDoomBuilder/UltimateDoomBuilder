namespace CodeImp.DoomBuilder.Windows
{
	partial class LinedefColorPresetsForm
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
			this.cancel = new System.Windows.Forms.Button();
			this.apply = new System.Windows.Forms.Button();
			this.moveup = new System.Windows.Forms.Button();
			this.movedown = new System.Windows.Forms.Button();
			this.deletepreset = new System.Windows.Forms.Button();
			this.addpreset = new System.Windows.Forms.Button();
			this.curpresetgroup = new System.Windows.Forms.GroupBox();
			this.errordescription = new System.Windows.Forms.Label();
			this.errordescription.AutoSize = true;
			this.erroricon = new System.Windows.Forms.PictureBox();
			this.presetsettings = new System.Windows.Forms.TabControl();
			this.tabFlags = new System.Windows.Forms.TabPage();
			this.useflags = new System.Windows.Forms.CheckBox();
			this.tabAction = new System.Windows.Forms.TabPage();
			this.useaction = new System.Windows.Forms.CheckBox();
			this.tabActivation = new System.Windows.Forms.TabPage();
			this.activation = new System.Windows.Forms.ComboBox();
			this.useactivation = new System.Windows.Forms.CheckBox();
			this.presetname = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label1.AutoSize = true;
			this.flags = new CodeImp.DoomBuilder.Controls.CheckboxArrayControl();
			this.action = new CodeImp.DoomBuilder.Controls.ActionSelectorControl();
			this.udmfactivates = new CodeImp.DoomBuilder.Controls.CheckboxArrayControl();
			this.presetcolor = new CodeImp.DoomBuilder.Controls.ColorControl();
			this.colorpresets = new CodeImp.DoomBuilder.Controls.CheckedColoredListBox();
			this.curpresetgroup.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.erroricon)).BeginInit();
			this.presetsettings.SuspendLayout();
			this.tabFlags.SuspendLayout();
			this.tabAction.SuspendLayout();
			this.tabActivation.SuspendLayout();
			this.SuspendLayout();
			// 
			// cancel
			// 
			this.cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancel.Location = new System.Drawing.Point(573, 428);
			this.cancel.Name = "cancel";
			this.cancel.Size = new System.Drawing.Size(112, 25);
			this.cancel.TabIndex = 7;
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
			this.apply.TabIndex = 6;
			this.apply.Text = "OK";
			this.apply.UseVisualStyleBackColor = true;
			this.apply.Click += new System.EventHandler(this.apply_Click);
			// 
			// moveup
			// 
			this.moveup.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.moveup.Image = global::CodeImp.DoomBuilder.Properties.Resources.ArrowUp;
			this.moveup.Location = new System.Drawing.Point(162, 424);
			this.moveup.Name = "moveup";
			this.moveup.Size = new System.Drawing.Size(25, 25);
			this.moveup.TabIndex = 12;
			this.moveup.UseVisualStyleBackColor = true;
			this.moveup.Click += new System.EventHandler(this.moveup_Click);
			// 
			// movedown
			// 
			this.movedown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.movedown.Image = global::CodeImp.DoomBuilder.Properties.Resources.ArrowDown;
			this.movedown.Location = new System.Drawing.Point(189, 424);
			this.movedown.Name = "movedown";
			this.movedown.Size = new System.Drawing.Size(25, 25);
			this.movedown.TabIndex = 11;
			this.movedown.UseVisualStyleBackColor = true;
			this.movedown.Click += new System.EventHandler(this.movedown_Click);
			// 
			// deletepreset
			// 
			this.deletepreset.Image = global::CodeImp.DoomBuilder.Properties.Resources.SearchClear;
			this.deletepreset.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.deletepreset.Location = new System.Drawing.Point(87, 424);
			this.deletepreset.Name = "deletepreset";
			this.deletepreset.Size = new System.Drawing.Size(70, 25);
			this.deletepreset.TabIndex = 10;
			this.deletepreset.Text = "Delete";
			this.deletepreset.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.deletepreset.UseVisualStyleBackColor = true;
			this.deletepreset.Click += new System.EventHandler(this.deletepreset_Click);
			// 
			// addpreset
			// 
			this.addpreset.Image = global::CodeImp.DoomBuilder.Properties.Resources.Add;
			this.addpreset.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.addpreset.Location = new System.Drawing.Point(12, 424);
			this.addpreset.Name = "addpreset";
			this.addpreset.Size = new System.Drawing.Size(70, 25);
			this.addpreset.TabIndex = 8;
			this.addpreset.Text = "Add";
			this.addpreset.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.addpreset.UseVisualStyleBackColor = true;
			this.addpreset.Click += new System.EventHandler(this.addpreset_Click);
			// 
			// curpresetgroup
			// 
			this.curpresetgroup.Controls.Add(this.errordescription);
			this.curpresetgroup.Controls.Add(this.erroricon);
			this.curpresetgroup.Controls.Add(this.presetsettings);
			this.curpresetgroup.Controls.Add(this.presetname);
			this.curpresetgroup.Controls.Add(this.presetcolor);
			this.curpresetgroup.Controls.Add(this.label1);
			this.curpresetgroup.Location = new System.Drawing.Point(220, 6);
			this.curpresetgroup.Name = "curpresetgroup";
			this.curpresetgroup.Size = new System.Drawing.Size(465, 417);
			this.curpresetgroup.TabIndex = 13;
			this.curpresetgroup.TabStop = false;
			this.curpresetgroup.Text = " Selected Preset ";
			// 
			// errordescription
			// 
			this.errordescription.AutoSize = true;
			this.errordescription.ForeColor = System.Drawing.Color.DarkRed;
			this.errordescription.Location = new System.Drawing.Point(31, 396);
			this.errordescription.Name = "errordescription";
			this.errordescription.Size = new System.Drawing.Size(96, 13);
			this.errordescription.TabIndex = 20;
			this.errordescription.Text = "Teh Error occured!";
			this.errordescription.Visible = false;
			// 
			// erroricon
			// 
			this.erroricon.BackColor = System.Drawing.SystemColors.Window;
			this.erroricon.Image = global::CodeImp.DoomBuilder.Properties.Resources.Warning;
			this.erroricon.Location = new System.Drawing.Point(11, 394);
			this.erroricon.Name = "erroricon";
			this.erroricon.Size = new System.Drawing.Size(16, 16);
			this.erroricon.TabIndex = 19;
			this.erroricon.TabStop = false;
			this.erroricon.Visible = false;
			// 
			// presetsettings
			// 
			this.presetsettings.Controls.Add(this.tabFlags);
			this.presetsettings.Controls.Add(this.tabAction);
			this.presetsettings.Controls.Add(this.tabActivation);
			this.presetsettings.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.presetsettings.Location = new System.Drawing.Point(6, 54);
			this.presetsettings.Name = "presetsettings";
			this.presetsettings.Padding = new System.Drawing.Point(24, 3);
			this.presetsettings.SelectedIndex = 0;
			this.presetsettings.Size = new System.Drawing.Size(453, 334);
			this.presetsettings.TabIndex = 18;
			// 
			// tabFlags
			// 
			this.tabFlags.Controls.Add(this.flags);
			this.tabFlags.Controls.Add(this.useflags);
			this.tabFlags.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.tabFlags.Location = new System.Drawing.Point(4, 22);
			this.tabFlags.Name = "tabFlags";
			this.tabFlags.Padding = new System.Windows.Forms.Padding(3);
			this.tabFlags.Size = new System.Drawing.Size(445, 308);
			this.tabFlags.TabIndex = 0;
			this.tabFlags.Text = "Flags";
			this.tabFlags.UseVisualStyleBackColor = true;
			// 
			// useflags
			// 
			this.useflags.AutoSize = true;
			this.useflags.Location = new System.Drawing.Point(6, 6);
			this.useflags.Name = "useflags";
			this.useflags.Size = new System.Drawing.Size(70, 17);
			this.useflags.TabIndex = 4;
			this.useflags.Text = "Use flags";
			this.useflags.UseVisualStyleBackColor = true;
			this.useflags.CheckedChanged += new System.EventHandler(this.useflags_CheckedChanged);
			// 
			// tabAction
			// 
			this.tabAction.Controls.Add(this.action);
			this.tabAction.Controls.Add(this.useaction);
			this.tabAction.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.tabAction.Location = new System.Drawing.Point(4, 22);
			this.tabAction.Name = "tabAction";
			this.tabAction.Padding = new System.Windows.Forms.Padding(3);
			this.tabAction.Size = new System.Drawing.Size(445, 308);
			this.tabAction.TabIndex = 1;
			this.tabAction.Text = "Action";
			this.tabAction.UseVisualStyleBackColor = true;
			// 
			// useaction
			// 
			this.useaction.AutoSize = true;
			this.useaction.Location = new System.Drawing.Point(6, 6);
			this.useaction.Name = "useaction";
			this.useaction.Size = new System.Drawing.Size(78, 17);
			this.useaction.TabIndex = 4;
			this.useaction.Text = "Use Action";
			this.useaction.UseVisualStyleBackColor = true;
			this.useaction.CheckedChanged += new System.EventHandler(this.useaction_CheckedChanged);
			// 
			// tabActivation
			// 
			this.tabActivation.Controls.Add(this.activation);
			this.tabActivation.Controls.Add(this.useactivation);
			this.tabActivation.Controls.Add(this.udmfactivates);
			this.tabActivation.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.tabActivation.Location = new System.Drawing.Point(4, 22);
			this.tabActivation.Name = "tabActivation";
			this.tabActivation.Size = new System.Drawing.Size(445, 308);
			this.tabActivation.TabIndex = 2;
			this.tabActivation.Text = "Activation type";
			this.tabActivation.UseVisualStyleBackColor = true;
			// 
			// activation
			// 
			this.activation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.activation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.activation.FormattingEnabled = true;
			this.activation.Location = new System.Drawing.Point(3, 30);
			this.activation.Name = "activation";
			this.activation.Size = new System.Drawing.Size(439, 21);
			this.activation.TabIndex = 0;
			this.activation.SelectedIndexChanged += new System.EventHandler(this.activation_SelectedIndexChanged);
			// 
			// useactivation
			// 
			this.useactivation.AutoSize = true;
			this.useactivation.Location = new System.Drawing.Point(6, 6);
			this.useactivation.Name = "useactivation";
			this.useactivation.Size = new System.Drawing.Size(94, 17);
			this.useactivation.TabIndex = 4;
			this.useactivation.Text = "Use activation";
			this.useactivation.UseVisualStyleBackColor = true;
			this.useactivation.CheckedChanged += new System.EventHandler(this.useactivation_CheckedChanged);
			// 
			// presetname
			// 
			this.presetname.Location = new System.Drawing.Point(92, 24);
			this.presetname.MaxLength = 50;
			this.presetname.Name = "presetname";
			this.presetname.Size = new System.Drawing.Size(208, 20);
			this.presetname.TabIndex = 17;
			this.presetname.Enter += new System.EventHandler(this.presetname_Enter);
			this.presetname.Validating += new System.ComponentModel.CancelEventHandler(this.presetname_Validating);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(48, 27);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(38, 13);
			this.label1.TabIndex = 16;
			this.label1.Text = "Name:";
			// 
			// flags
			// 
			this.flags.AutoScroll = true;
			this.flags.Columns = 2;
			this.flags.Location = new System.Drawing.Point(6, 29);
			this.flags.Name = "flags";
			this.flags.Size = new System.Drawing.Size(433, 273);
			this.flags.TabIndex = 5;
			this.flags.VerticalSpacing = 1;
			// 
			// action
			// 
			this.action.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.action.BackColor = System.Drawing.Color.Transparent;
			this.action.Cursor = System.Windows.Forms.Cursors.Default;
			this.action.Empty = false;
			this.action.GeneralizedCategories = null;
			this.action.GeneralizedOptions = null;
			this.action.Location = new System.Drawing.Point(6, 30);
			this.action.Name = "action";
			this.action.Size = new System.Drawing.Size(433, 21);
			this.action.TabIndex = 6;
			this.action.Value = 0;
			this.action.ValueChanges += new System.EventHandler(this.action_ValueChanges);
			// 
			// udmfactivates
			// 
			this.udmfactivates.AutoScroll = true;
			this.udmfactivates.Columns = 2;
			this.udmfactivates.Location = new System.Drawing.Point(6, 29);
			this.udmfactivates.Name = "udmfactivates";
			this.udmfactivates.Size = new System.Drawing.Size(435, 276);
			this.udmfactivates.TabIndex = 5;
			this.udmfactivates.VerticalSpacing = 1;
			// 
			// presetcolor
			// 
			this.presetcolor.BackColor = System.Drawing.Color.Transparent;
			this.presetcolor.Label = "Linedef color:";
			this.presetcolor.Location = new System.Drawing.Point(319, 23);
			this.presetcolor.MaximumSize = new System.Drawing.Size(10000, 23);
			this.presetcolor.MinimumSize = new System.Drawing.Size(100, 23);
			this.presetcolor.Name = "presetcolor";
			this.presetcolor.Size = new System.Drawing.Size(132, 23);
			this.presetcolor.TabIndex = 0;
			this.presetcolor.ColorChanged += new System.EventHandler(this.presetcolor_ColorChanged);
			// 
			// colorpresets
			// 
			this.colorpresets.Location = new System.Drawing.Point(12, 12);
			this.colorpresets.Name = "colorpresets";
			this.colorpresets.Size = new System.Drawing.Size(202, 409);
			this.colorpresets.TabIndex = 9;
			this.colorpresets.WarningIcon = global::CodeImp.DoomBuilder.Properties.Resources.Warning;
			this.colorpresets.SelectedIndexChanged += new System.EventHandler(this.colorpresets_SelectedIndexChanged);
			this.colorpresets.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.colorpresets_ItemCheck);
			// 
			// LinedefColorPresetsForm
			// 
			this.AcceptButton = this.apply;
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.CancelButton = this.cancel;
			this.ClientSize = new System.Drawing.Size(696, 459);
			this.Controls.Add(this.curpresetgroup);
			this.Controls.Add(this.moveup);
			this.Controls.Add(this.movedown);
			this.Controls.Add(this.deletepreset);
			this.Controls.Add(this.addpreset);
			this.Controls.Add(this.colorpresets);
			this.Controls.Add(this.cancel);
			this.Controls.Add(this.apply);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "LinedefColorPresetsForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Linedef Color Presets";
			this.curpresetgroup.ResumeLayout(false);
			this.curpresetgroup.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.erroricon)).EndInit();
			this.presetsettings.ResumeLayout(false);
			this.tabFlags.ResumeLayout(false);
			this.tabFlags.PerformLayout();
			this.tabAction.ResumeLayout(false);
			this.tabAction.PerformLayout();
			this.tabActivation.ResumeLayout(false);
			this.tabActivation.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button cancel;
		private System.Windows.Forms.Button apply;
		private System.Windows.Forms.Button moveup;
		private System.Windows.Forms.Button movedown;
		private System.Windows.Forms.Button deletepreset;
		private CodeImp.DoomBuilder.Controls.CheckedColoredListBox colorpresets;
		private System.Windows.Forms.Button addpreset;
		private System.Windows.Forms.GroupBox curpresetgroup;
		private CodeImp.DoomBuilder.Controls.ColorControl presetcolor;
		private System.Windows.Forms.TextBox presetname;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TabControl presetsettings;
		private System.Windows.Forms.TabPage tabFlags;
		private System.Windows.Forms.CheckBox useflags;
		private System.Windows.Forms.TabPage tabAction;
		private CodeImp.DoomBuilder.Controls.ActionSelectorControl action;
		private System.Windows.Forms.CheckBox useaction;
		private System.Windows.Forms.TabPage tabActivation;
		private System.Windows.Forms.ComboBox activation;
		private System.Windows.Forms.CheckBox useactivation;
		private System.Windows.Forms.PictureBox erroricon;
		private System.Windows.Forms.Label errordescription;
		private CodeImp.DoomBuilder.Controls.CheckboxArrayControl flags;
		private CodeImp.DoomBuilder.Controls.CheckboxArrayControl udmfactivates;
	}
}
