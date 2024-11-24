namespace CodeImp.DoomBuilder.Windows
{
	partial class MapOptionsForm
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
            System.Windows.Forms.Label label3;
            System.Windows.Forms.Label label2;
            System.Windows.Forms.Label label1;
            System.Windows.Forms.GroupBox panelsettings;
            System.Windows.Forms.Label label4;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MapOptionsForm));
            this.examplelabel = new System.Windows.Forms.Label();
            this.examplelabel.AutoSize = true;
            this.scriptcompiler = new System.Windows.Forms.ComboBox();
            this.scriptcompilerlabel = new System.Windows.Forms.Label();
            this.scriptcompilerlabel.AutoSize = true;
            this.levelname = new System.Windows.Forms.TextBox();
            this.config = new System.Windows.Forms.ComboBox();
            this.apply = new System.Windows.Forms.Button();
            this.cancel = new System.Windows.Forms.Button();
            this.panelres = new System.Windows.Forms.GroupBox();
            this.longtexturenames = new System.Windows.Forms.CheckBox();
            this.strictpatches = new System.Windows.Forms.CheckBox();
            this.datalocations = new CodeImp.DoomBuilder.Controls.ResourceListEditor();
            this.tooltip = new System.Windows.Forms.ToolTip(this.components);
            this.label5 = new System.Windows.Forms.Label();
            this.label5.AutoSize = true;
            this.prepostcommands = new System.Windows.Forms.Button();
            label3 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            label1 = new System.Windows.Forms.Label();
            panelsettings = new System.Windows.Forms.GroupBox();
            label4 = new System.Windows.Forms.Label();
            panelsettings.SuspendLayout();
            this.panelres.SuspendLayout();
            this.SuspendLayout();
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(239, 83);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(52, 13);
            label3.TabIndex = 9;
            label3.Text = "example: ";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(58, 83);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(65, 13);
            label2.TabIndex = 7;
            label2.Text = "Level name:";
            // 
            // label1
            // 
            label1.Location = new System.Drawing.Point(13, 27);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(110, 14);
            label1.TabIndex = 5;
            label1.Text = "Game Configuration:";
            label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // panelsettings
            // 
            panelsettings.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            panelsettings.Controls.Add(this.examplelabel);
            panelsettings.Controls.Add(this.scriptcompiler);
            panelsettings.Controls.Add(this.scriptcompilerlabel);
            panelsettings.Controls.Add(label3);
            panelsettings.Controls.Add(this.levelname);
            panelsettings.Controls.Add(label2);
            panelsettings.Controls.Add(this.config);
            panelsettings.Controls.Add(label1);
            panelsettings.Location = new System.Drawing.Point(12, 12);
            panelsettings.Name = "panelsettings";
            panelsettings.Size = new System.Drawing.Size(397, 112);
            panelsettings.TabIndex = 0;
            panelsettings.TabStop = false;
            panelsettings.Text = " Settings ";
            // 
            // examplelabel
            // 
            this.examplelabel.AutoSize = true;
            this.examplelabel.Location = new System.Drawing.Point(288, 83);
            this.examplelabel.Name = "examplelabel";
            this.examplelabel.Size = new System.Drawing.Size(42, 13);
            this.examplelabel.TabIndex = 12;
            this.examplelabel.Text = "MAP01";
            // 
            // scriptcompiler
            // 
            this.scriptcompiler.DropDownHeight = 206;
            this.scriptcompiler.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.scriptcompiler.FormattingEnabled = true;
            this.scriptcompiler.IntegralHeight = false;
            this.scriptcompiler.Location = new System.Drawing.Point(129, 52);
            this.scriptcompiler.Name = "scriptcompiler";
            this.scriptcompiler.Size = new System.Drawing.Size(254, 21);
            this.scriptcompiler.TabIndex = 10;
            // 
            // scriptcompilerlabel
            // 
            this.scriptcompilerlabel.Location = new System.Drawing.Point(13, 55);
            this.scriptcompilerlabel.Name = "scriptcompilerlabel";
            this.scriptcompilerlabel.Size = new System.Drawing.Size(110, 14);
            this.scriptcompilerlabel.TabIndex = 11;
            this.scriptcompilerlabel.Text = "Script Type:";
            this.scriptcompilerlabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // levelname
            // 
            this.levelname.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.levelname.Location = new System.Drawing.Point(129, 80);
            this.levelname.MaxLength = 8;
            this.levelname.Name = "levelname";
            this.levelname.Size = new System.Drawing.Size(94, 20);
            this.levelname.TabIndex = 1;
            this.levelname.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.levelname_KeyPress);
            // 
            // config
            // 
            this.config.DropDownHeight = 206;
            this.config.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.config.FormattingEnabled = true;
            this.config.IntegralHeight = false;
            this.config.Location = new System.Drawing.Point(129, 24);
            this.config.Name = "config";
            this.config.Size = new System.Drawing.Size(254, 21);
            this.config.TabIndex = 0;
            this.config.SelectedIndexChanged += new System.EventHandler(this.config_SelectedIndexChanged);
            // 
            // label4
            // 
            label4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(13, 198);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(299, 52);
            label4.TabIndex = 17;
            label4.Text = resources.GetString("label4.Text");
            // 
            // apply
            // 
            this.apply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.apply.Location = new System.Drawing.Point(179, 429);
            this.apply.Name = "apply";
            this.apply.Size = new System.Drawing.Size(112, 25);
            this.apply.TabIndex = 2;
            this.apply.Text = "OK";
            this.apply.UseVisualStyleBackColor = true;
            this.apply.Click += new System.EventHandler(this.apply_Click);
            // 
            // cancel
            // 
            this.cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancel.Location = new System.Drawing.Point(297, 429);
            this.cancel.Name = "cancel";
            this.cancel.Size = new System.Drawing.Size(112, 25);
            this.cancel.TabIndex = 3;
            this.cancel.Text = "Cancel";
            this.cancel.UseVisualStyleBackColor = true;
            this.cancel.Click += new System.EventHandler(this.cancel_Click);
            // 
            // panelres
            // 
            this.panelres.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelres.Controls.Add(this.longtexturenames);
            this.panelres.Controls.Add(this.strictpatches);
            this.panelres.Controls.Add(this.datalocations);
            this.panelres.Controls.Add(label4);
            this.panelres.Location = new System.Drawing.Point(12, 130);
            this.panelres.Name = "panelres";
            this.panelres.Size = new System.Drawing.Size(397, 259);
            this.panelres.TabIndex = 1;
            this.panelres.TabStop = false;
            this.panelres.Text = " Resources ";
            // 
            // longtexturenames
            // 
            this.longtexturenames.AutoSize = true;
            this.longtexturenames.Location = new System.Drawing.Point(14, 45);
            this.longtexturenames.Name = "longtexturenames";
            this.longtexturenames.Size = new System.Drawing.Size(137, 17);
            this.longtexturenames.TabIndex = 21;
            this.longtexturenames.Text = "Use long texture names";
            this.longtexturenames.UseVisualStyleBackColor = true;
            // 
            // strictpatches
            // 
            this.strictpatches.AutoSize = true;
            this.strictpatches.Location = new System.Drawing.Point(14, 21);
            this.strictpatches.Name = "strictpatches";
            this.strictpatches.Size = new System.Drawing.Size(349, 17);
            this.strictpatches.TabIndex = 20;
            this.strictpatches.Text = "Strictly load patches between P_START and P_END only for this file";
            this.strictpatches.UseVisualStyleBackColor = true;
            // 
            // datalocations
            // 
            this.datalocations.AllowDrop = true;
            this.datalocations.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.datalocations.DialogOffset = new System.Drawing.Point(40, 20);
            this.datalocations.GameConfiguration = null;
            this.datalocations.Location = new System.Drawing.Point(14, 68);
            this.datalocations.Name = "datalocations";
            this.datalocations.Size = new System.Drawing.Size(368, 127);
            this.datalocations.TabIndex = 0;
            this.datalocations.OnWarningsChanged += new CodeImp.DoomBuilder.Controls.ResourceListEditor.WarningsChanged(this.datalocations_OnWarningsChanged);
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label5.Location = new System.Drawing.Point(13, 423);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(396, 2);
            this.label5.TabIndex = 21;
            // 
            // prepostcommands
            // 
            this.prepostcommands.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.prepostcommands.Location = new System.Drawing.Point(247, 395);
            this.prepostcommands.Name = "prepostcommands";
            this.prepostcommands.Size = new System.Drawing.Size(162, 25);
            this.prepostcommands.TabIndex = 20;
            this.prepostcommands.Text = "Edit pre and post commands";
            this.prepostcommands.UseVisualStyleBackColor = true;
            this.prepostcommands.Click += new System.EventHandler(this.prepostcommands_Click);
            // 
            // MapOptionsForm
            // 
            this.AcceptButton = this.apply;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.cancel;
            this.ClientSize = new System.Drawing.Size(421, 466);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.prepostcommands);
            this.Controls.Add(this.panelres);
            this.Controls.Add(this.cancel);
            this.Controls.Add(this.apply);
            this.Controls.Add(panelsettings);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MapOptionsForm";
            this.Opacity = 0D;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Map Options";
            this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.MapOptionsForm_HelpRequested);
            panelsettings.ResumeLayout(false);
            panelsettings.PerformLayout();
            this.panelres.ResumeLayout(false);
            this.panelres.PerformLayout();
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TextBox levelname;
		private System.Windows.Forms.ComboBox config;
		private System.Windows.Forms.Button apply;
		private System.Windows.Forms.Button cancel;
		private System.Windows.Forms.GroupBox panelres;
		private CodeImp.DoomBuilder.Controls.ResourceListEditor datalocations;
		private System.Windows.Forms.CheckBox strictpatches;
		private System.Windows.Forms.ComboBox scriptcompiler;
		private System.Windows.Forms.Label scriptcompilerlabel;
		private System.Windows.Forms.Label examplelabel;
		private System.Windows.Forms.CheckBox longtexturenames;
		private System.Windows.Forms.ToolTip tooltip;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Button prepostcommands;
	}
}
