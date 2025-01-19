namespace CodeImp.DoomBuilder.Windows
{
	partial class TextureSetForm
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
            this.name = new System.Windows.Forms.TextBox();
            this.filters = new System.Windows.Forms.ListView();
            this.filtercolumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label2 = new System.Windows.Forms.Label();
            this.label2.AutoSize = true;
            this.label3 = new System.Windows.Forms.Label();
            this.label3.AutoSize = true;
            this.label4 = new System.Windows.Forms.Label();
            this.label4.AutoSize = true;
            this.apply = new System.Windows.Forms.Button();
            this.cancel = new System.Windows.Forms.Button();
            this.addfilter = new System.Windows.Forms.Button();
            this.removefilter = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.filterstimer = new System.Windows.Forms.Timer(this.components);
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.nomatchesbutton = new System.Windows.Forms.RadioButton();
            this.matchesbutton = new System.Windows.Forms.RadioButton();
            this.matcheslist = new CodeImp.DoomBuilder.Controls.ImageBrowserControl();
            this.noresultlabel = new System.Windows.Forms.Label();
            this.noresultlabel.AutoSize = true;
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // name
            // 
            this.name.Location = new System.Drawing.Point(36, 19);
            this.name.Name = "name";
            this.name.Size = new System.Drawing.Size(212, 20);
            this.name.TabIndex = 0;
            // 
            // filters
            // 
            this.filters.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.filters.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.filtercolumn});
            this.filters.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.filters.HideSelection = false;
            this.filters.LabelEdit = true;
            this.filters.Location = new System.Drawing.Point(21, 110);
            this.filters.Name = "filters";
            this.filters.ShowGroups = false;
            this.filters.Size = new System.Drawing.Size(227, 279);
            this.filters.TabIndex = 0;
            this.filters.UseCompatibleStateImageBehavior = false;
            this.filters.View = System.Windows.Forms.View.Details;
            this.filters.AfterLabelEdit += new System.Windows.Forms.LabelEditEventHandler(this.filters_AfterLabelEdit);
            this.filters.SelectedIndexChanged += new System.EventHandler(this.filters_SelectedIndexChanged);
            this.filters.DoubleClick += new System.EventHandler(this.filters_DoubleClick);
            // 
            // filtercolumn
            // 
            this.filtercolumn.Text = "Filter";
            this.filtercolumn.Width = 192;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(18, 28);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(248, 42);
            this.label2.TabIndex = 3;
            this.label2.Text = "Add the names of the textures in this set below. You can use the following wildca" +
    "rds:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(28, 65);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(170, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "? = matches exactly one character";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(28, 83);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(177, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "* = matches zero or more characters";
            // 
            // apply
            // 
            this.apply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.apply.Location = new System.Drawing.Point(493, 507);
            this.apply.Name = "apply";
            this.apply.Size = new System.Drawing.Size(105, 25);
            this.apply.TabIndex = 3;
            this.apply.Text = "OK";
            this.apply.UseVisualStyleBackColor = true;
            this.apply.Click += new System.EventHandler(this.apply_Click);
            // 
            // cancel
            // 
            this.cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancel.Location = new System.Drawing.Point(604, 507);
            this.cancel.Name = "cancel";
            this.cancel.Size = new System.Drawing.Size(105, 25);
            this.cancel.TabIndex = 4;
            this.cancel.Text = "Cancel";
            this.cancel.UseVisualStyleBackColor = true;
            this.cancel.Click += new System.EventHandler(this.cancel_Click);
            // 
            // addfilter
            // 
            this.addfilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.addfilter.Location = new System.Drawing.Point(21, 395);
            this.addfilter.Name = "addfilter";
            this.addfilter.Size = new System.Drawing.Size(110, 24);
            this.addfilter.TabIndex = 1;
            this.addfilter.Text = "Add Texture";
            this.addfilter.UseVisualStyleBackColor = true;
            this.addfilter.Click += new System.EventHandler(this.addfilter_Click);
            // 
            // removefilter
            // 
            this.removefilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.removefilter.Enabled = false;
            this.removefilter.Location = new System.Drawing.Point(138, 395);
            this.removefilter.Name = "removefilter";
            this.removefilter.Size = new System.Drawing.Size(110, 24);
            this.removefilter.TabIndex = 2;
            this.removefilter.Text = "Remove Selection";
            this.removefilter.UseVisualStyleBackColor = true;
            this.removefilter.Click += new System.EventHandler(this.removefilter_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox1.Controls.Add(this.removefilter);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.addfilter);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.filters);
            this.groupBox1.Location = new System.Drawing.Point(12, 66);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(270, 430);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = " Filters ";
            // 
            // filterstimer
            // 
            this.filterstimer.Interval = 1;
            this.filterstimer.Tick += new System.EventHandler(this.filterstimer_Tick);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.nomatchesbutton);
            this.groupBox2.Controls.Add(this.matchesbutton);
            this.groupBox2.Controls.Add(this.matcheslist);
            this.groupBox2.Controls.Add(this.noresultlabel);
            this.groupBox2.Location = new System.Drawing.Point(288, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(421, 484);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = " Results ";
            // 
            // nomatchesbutton
            // 
            this.nomatchesbutton.Appearance = System.Windows.Forms.Appearance.Button;
            this.nomatchesbutton.Location = new System.Drawing.Point(141, 25);
            this.nomatchesbutton.Name = "nomatchesbutton";
            this.nomatchesbutton.Size = new System.Drawing.Size(117, 24);
            this.nomatchesbutton.TabIndex = 1;
            this.nomatchesbutton.Text = "Show Not Matching";
            this.nomatchesbutton.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.nomatchesbutton.UseVisualStyleBackColor = true;
            this.nomatchesbutton.Click += new System.EventHandler(this.matchesbutton_Click);
            // 
            // matchesbutton
            // 
            this.matchesbutton.Appearance = System.Windows.Forms.Appearance.Button;
            this.matchesbutton.Checked = true;
            this.matchesbutton.Location = new System.Drawing.Point(18, 25);
            this.matchesbutton.Name = "matchesbutton";
            this.matchesbutton.Size = new System.Drawing.Size(117, 24);
            this.matchesbutton.TabIndex = 0;
            this.matchesbutton.TabStop = true;
            this.matchesbutton.Text = "Show Matches";
            this.matchesbutton.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.matchesbutton.UseVisualStyleBackColor = true;
            this.matchesbutton.Click += new System.EventHandler(this.matchesbutton_Click);
            // 
            // matcheslist
            // 
            this.matcheslist.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.matcheslist.HideInputBox = true;
            this.matcheslist.ImageSize = 0;
            this.matcheslist.Location = new System.Drawing.Point(18, 55);
            this.matcheslist.Name = "matcheslist";
            this.matcheslist.PreventSelection = true;
            this.matcheslist.Size = new System.Drawing.Size(387, 418);
            this.matcheslist.TabIndex = 2;
            this.matcheslist.SelectedItemDoubleClicked += new CodeImp.DoomBuilder.Controls.ImageBrowserControl.SelectedItemDoubleClickDelegate(this.matcheslist_SelectedItemDoubleClicked);
            // 
            // noresultlabel
            // 
            this.noresultlabel.Location = new System.Drawing.Point(15, 28);
            this.noresultlabel.Name = "noresultlabel";
            this.noresultlabel.Size = new System.Drawing.Size(272, 43);
            this.noresultlabel.TabIndex = 33;
            this.noresultlabel.Text = "An example result cannot be displayed, because it requires a map to be loaded.";
            this.noresultlabel.Visible = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::CodeImp.DoomBuilder.Properties.Resources.KnownTextureSet;
            this.pictureBox1.Location = new System.Drawing.Point(11, 21);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(19, 16);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 12;
            this.pictureBox1.TabStop = false;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.name);
            this.groupBox3.Controls.Add(this.pictureBox1);
            this.groupBox3.Location = new System.Drawing.Point(12, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(270, 48);
            this.groupBox3.TabIndex = 13;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = " Texture Set Name ";
            // 
            // TextureSetForm
            // 
            this.AcceptButton = this.apply;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.cancel;
            this.ClientSize = new System.Drawing.Size(721, 536);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.cancel);
            this.Controls.Add(this.apply);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TextureSetForm";
            this.Opacity = 0D;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Edit Texture Set";
            this.Shown += new System.EventHandler(this.TextureSetForm_Shown);
            this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.TextureSetForm_HelpRequested);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TextBox name;
		private System.Windows.Forms.ListView filters;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Button apply;
		private System.Windows.Forms.Button cancel;
		private System.Windows.Forms.ColumnHeader filtercolumn;
		private System.Windows.Forms.Button addfilter;
		private System.Windows.Forms.Button removefilter;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Timer filterstimer;
		private System.Windows.Forms.GroupBox groupBox2;
		private CodeImp.DoomBuilder.Controls.ImageBrowserControl matcheslist;
		private System.Windows.Forms.Label noresultlabel;
		private System.Windows.Forms.RadioButton nomatchesbutton;
		private System.Windows.Forms.RadioButton matchesbutton;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.GroupBox groupBox3;
	}
}
