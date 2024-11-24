namespace CodeImp.DoomBuilder.Windows
{
	partial class OpenMapOptionsForm
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
            System.Windows.Forms.ColumnHeader columnHeader1;
            System.Windows.Forms.Label label1;
            System.Windows.Forms.Label label2;
            System.Windows.Forms.Label label3;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OpenMapOptionsForm));
            this.panelres = new System.Windows.Forms.GroupBox();
            this.longtexturenames = new System.Windows.Forms.CheckBox();
            this.strictpatches = new System.Windows.Forms.CheckBox();
            this.datalocations = new CodeImp.DoomBuilder.Controls.ResourceListEditor();
            this.apply = new System.Windows.Forms.Button();
            this.cancel = new System.Windows.Forms.Button();
            this.config = new System.Windows.Forms.ComboBox();
            this.mapslist = new System.Windows.Forms.ListView();
            this.scriptcompiler = new System.Windows.Forms.ComboBox();
            this.scriptcompilerlabel = new System.Windows.Forms.Label();
            this.scriptcompilerlabel.AutoSize = true;
            this.tooltip = new System.Windows.Forms.ToolTip(this.components);
            columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            this.panelres.SuspendLayout();
            this.SuspendLayout();
            // 
            // columnHeader1
            // 
            columnHeader1.Text = "Map name";
            columnHeader1.Width = -1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(15, 24);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(120, 14);
            label1.TabIndex = 14;
            label1.Text = "Game Configuration:";
            label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(12, 89);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(360, 26);
            label2.TabIndex = 16;
            label2.Text = "With the above selected configuration, the maps shown below were found \r\nin the c" +
    "hosen WAD file. Please select the map to load for editing.";
            // 
            // label3
            // 
            label3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(11, 195);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(299, 52);
            label3.TabIndex = 17;
            label3.Text = resources.GetString("label3.Text");
            // 
            // panelres
            // 
            this.panelres.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelres.Controls.Add(this.longtexturenames);
            this.panelres.Controls.Add(this.strictpatches);
            this.panelres.Controls.Add(this.datalocations);
            this.panelres.Controls.Add(label3);
            this.panelres.Location = new System.Drawing.Point(12, 246);
            this.panelres.Name = "panelres";
            this.panelres.Size = new System.Drawing.Size(396, 259);
            this.panelres.TabIndex = 2;
            this.panelres.TabStop = false;
            this.panelres.Text = " Resources ";
            // 
            // longtexturenames
            // 
            this.longtexturenames.AutoSize = true;
            this.longtexturenames.Location = new System.Drawing.Point(14, 45);
            this.longtexturenames.Name = "longtexturenames";
            this.longtexturenames.Size = new System.Drawing.Size(137, 17);
            this.longtexturenames.TabIndex = 20;
            this.longtexturenames.Text = "Use long texture names";
            this.longtexturenames.UseVisualStyleBackColor = true;
            // 
            // strictpatches
            // 
            this.strictpatches.AutoSize = true;
            this.strictpatches.Location = new System.Drawing.Point(14, 21);
            this.strictpatches.Name = "strictpatches";
            this.strictpatches.Size = new System.Drawing.Size(349, 17);
            this.strictpatches.TabIndex = 19;
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
            this.datalocations.Size = new System.Drawing.Size(367, 124);
            this.datalocations.TabIndex = 0;
            this.datalocations.OnWarningsChanged += new CodeImp.DoomBuilder.Controls.ResourceListEditor.WarningsChanged(this.datalocations_OnWarningsChanged);
            // 
            // apply
            // 
            this.apply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.apply.Location = new System.Drawing.Point(178, 510);
            this.apply.Name = "apply";
            this.apply.Size = new System.Drawing.Size(112, 25);
            this.apply.TabIndex = 3;
            this.apply.Text = "OK";
            this.apply.UseVisualStyleBackColor = true;
            this.apply.Click += new System.EventHandler(this.apply_Click);
            // 
            // cancel
            // 
            this.cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancel.Location = new System.Drawing.Point(296, 510);
            this.cancel.Name = "cancel";
            this.cancel.Size = new System.Drawing.Size(112, 25);
            this.cancel.TabIndex = 4;
            this.cancel.Text = "Cancel";
            this.cancel.UseVisualStyleBackColor = true;
            this.cancel.Click += new System.EventHandler(this.cancel_Click);
            // 
            // config
            // 
            this.config.DropDownHeight = 206;
            this.config.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.config.FormattingEnabled = true;
            this.config.IntegralHeight = false;
            this.config.Location = new System.Drawing.Point(141, 21);
            this.config.Name = "config";
            this.config.Size = new System.Drawing.Size(267, 21);
            this.config.TabIndex = 0;
            this.config.SelectedIndexChanged += new System.EventHandler(this.config_SelectedIndexChanged);
            // 
            // mapslist
            // 
            this.mapslist.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mapslist.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            columnHeader1});
            this.mapslist.FullRowSelect = true;
            this.mapslist.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.mapslist.HideSelection = false;
            this.mapslist.LabelWrap = false;
            this.mapslist.Location = new System.Drawing.Point(12, 122);
            this.mapslist.MultiSelect = false;
            this.mapslist.Name = "mapslist";
            this.mapslist.ShowGroups = false;
            this.mapslist.Size = new System.Drawing.Size(396, 118);
            this.mapslist.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.mapslist.TabIndex = 1;
            this.mapslist.UseCompatibleStateImageBehavior = false;
            this.mapslist.View = System.Windows.Forms.View.List;
            this.mapslist.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.mapslist_ItemSelectionChanged);
            this.mapslist.DoubleClick += new System.EventHandler(this.mapslist_DoubleClick);
            // 
            // scriptcompiler
            // 
            this.scriptcompiler.DropDownHeight = 206;
            this.scriptcompiler.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.scriptcompiler.Enabled = false;
            this.scriptcompiler.FormattingEnabled = true;
            this.scriptcompiler.IntegralHeight = false;
            this.scriptcompiler.Location = new System.Drawing.Point(141, 49);
            this.scriptcompiler.Name = "scriptcompiler";
            this.scriptcompiler.Size = new System.Drawing.Size(267, 21);
            this.scriptcompiler.TabIndex = 17;
            // 
            // scriptcompilerlabel
            // 
            this.scriptcompilerlabel.Enabled = false;
            this.scriptcompilerlabel.Location = new System.Drawing.Point(15, 52);
            this.scriptcompilerlabel.Name = "scriptcompilerlabel";
            this.scriptcompilerlabel.Size = new System.Drawing.Size(120, 14);
            this.scriptcompilerlabel.TabIndex = 18;
            this.scriptcompilerlabel.Text = "Script Type:";
            this.scriptcompilerlabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // OpenMapOptionsForm
            // 
            this.AcceptButton = this.apply;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.cancel;
            this.ClientSize = new System.Drawing.Size(420, 541);
            this.Controls.Add(this.scriptcompiler);
            this.Controls.Add(this.scriptcompilerlabel);
            this.Controls.Add(this.mapslist);
            this.Controls.Add(label2);
            this.Controls.Add(this.config);
            this.Controls.Add(label1);
            this.Controls.Add(this.cancel);
            this.Controls.Add(this.apply);
            this.Controls.Add(this.panelres);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OpenMapOptionsForm";
            this.Opacity = 0D;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Open Map Options";
            this.Shown += new System.EventHandler(this.OpenMapOptionsForm_Shown);
            this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.OpenMapOptionsForm_HelpRequested);
            this.panelres.ResumeLayout(false);
            this.panelres.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button apply;
		private System.Windows.Forms.Button cancel;
		private System.Windows.Forms.ComboBox config;
		private System.Windows.Forms.GroupBox panelres;
		private System.Windows.Forms.ListView mapslist;
		private CodeImp.DoomBuilder.Controls.ResourceListEditor datalocations;
		private System.Windows.Forms.CheckBox strictpatches;
		private System.Windows.Forms.ComboBox scriptcompiler;
		private System.Windows.Forms.Label scriptcompilerlabel;
		private System.Windows.Forms.CheckBox longtexturenames;
		private System.Windows.Forms.ToolTip tooltip;
	}
}
