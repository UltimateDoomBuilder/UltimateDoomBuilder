namespace CodeImp.DoomBuilder.BuilderModes
{
	partial class ErrorCheckForm
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
			this.checks = new CodeImp.DoomBuilder.Controls.CheckboxArrayControl();
			this.buttoncheck = new System.Windows.Forms.Button();
			this.results = new System.Windows.Forms.ListBox();
			this.resultcontextmenustrip = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.resultselectcurrenttype = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.resultshowall = new System.Windows.Forms.ToolStripMenuItem();
			this.resulthidecurrent = new System.Windows.Forms.ToolStripMenuItem();
			this.resulthidecurrenttype = new System.Windows.Forms.ToolStripMenuItem();
			this.resultshowonlycurrent = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.resultcopytoclipboard = new System.Windows.Forms.ToolStripMenuItem();
			this.resultspanel = new System.Windows.Forms.Panel();
			this.fix3 = new System.Windows.Forms.Button();
			this.fix2 = new System.Windows.Forms.Button();
			this.resultinfo = new System.Windows.Forms.Label();
			this.resultinfo.AutoSize = true;
			this.fix1 = new System.Windows.Forms.Button();
			this.progress = new System.Windows.Forms.ProgressBar();
			this.closebutton = new System.Windows.Forms.Button();
			this.toggleall = new System.Windows.Forms.CheckBox();
			this.resultcontextmenustrip.SuspendLayout();
			this.resultspanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// checks
			// 
			this.checks.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.checks.AutoScroll = true;
			this.checks.AutoSize = true;
			this.checks.Columns = 2;
			this.checks.Location = new System.Drawing.Point(10, 32);
			this.checks.Margin = new System.Windows.Forms.Padding(1);
			this.checks.Name = "checks";
			this.checks.Size = new System.Drawing.Size(360, 53);
			this.checks.TabIndex = 0;
			this.checks.VerticalSpacing = 1;
			// 
			// buttoncheck
			// 
			this.buttoncheck.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.buttoncheck.Location = new System.Drawing.Point(10, 89);
			this.buttoncheck.Margin = new System.Windows.Forms.Padding(1);
			this.buttoncheck.Name = "buttoncheck";
			this.buttoncheck.Size = new System.Drawing.Size(360, 26);
			this.buttoncheck.TabIndex = 1;
			this.buttoncheck.Text = "Start Analysis";
			this.buttoncheck.UseVisualStyleBackColor = true;
			this.buttoncheck.Click += new System.EventHandler(this.buttoncheck_Click);
			// 
			// results
			// 
			this.results.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.results.ContextMenuStrip = this.resultcontextmenustrip;
			this.results.FormattingEnabled = true;
			this.results.HorizontalScrollbar = true;
			this.results.IntegralHeight = false;
			this.results.Location = new System.Drawing.Point(10, 34);
			this.results.Name = "results";
			this.results.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this.results.Size = new System.Drawing.Size(360, 290);
			this.results.Sorted = true;
			this.results.TabIndex = 0;
			this.results.SelectedIndexChanged += new System.EventHandler(this.results_SelectedIndexChanged);
			this.results.KeyUp += new System.Windows.Forms.KeyEventHandler(this.results_KeyUp);
			// 
			// resultcontextmenustrip
			// 
			this.resultcontextmenustrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.resultselectcurrenttype,
            this.toolStripSeparator2,
            this.resultshowall,
            this.resulthidecurrent,
            this.resulthidecurrenttype,
            this.resultshowonlycurrent,
            this.toolStripSeparator1,
            this.resultcopytoclipboard});
			this.resultcontextmenustrip.Name = "resultcontextmenustrip";
			this.resultcontextmenustrip.Size = new System.Drawing.Size(245, 148);
			this.resultcontextmenustrip.Opening += new System.ComponentModel.CancelEventHandler(this.resultcontextmenustrip_Opening);
			// 
			// resultselectcurrenttype
			// 
			this.resultselectcurrenttype.Image = global::CodeImp.DoomBuilder.BuilderModes.Properties.Resources.Show3;
			this.resultselectcurrenttype.Name = "resultselectcurrenttype";
			this.resultselectcurrenttype.Size = new System.Drawing.Size(244, 22);
			this.resultselectcurrenttype.Text = "Select results of this type(s)";
			this.resultselectcurrenttype.Click += new System.EventHandler(this.resultselectcurrenttype_Click);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(241, 6);
			// 
			// resultshowall
			// 
			this.resultshowall.Image = global::CodeImp.DoomBuilder.BuilderModes.Properties.Resources.Show;
			this.resultshowall.Name = "resultshowall";
			this.resultshowall.Size = new System.Drawing.Size(244, 22);
			this.resultshowall.Text = "Show all results";
			this.resultshowall.Click += new System.EventHandler(this.resultshowall_Click);
			// 
			// resulthidecurrent
			// 
			this.resulthidecurrent.Image = global::CodeImp.DoomBuilder.BuilderModes.Properties.Resources.Hide;
			this.resulthidecurrent.Name = "resulthidecurrent";
			this.resulthidecurrent.Size = new System.Drawing.Size(244, 22);
			this.resulthidecurrent.Text = "Hide selected results";
			this.resulthidecurrent.Click += new System.EventHandler(this.resulthidecurrent_Click);
			// 
			// resulthidecurrenttype
			// 
			this.resulthidecurrenttype.Image = global::CodeImp.DoomBuilder.BuilderModes.Properties.Resources.HideAll;
			this.resulthidecurrenttype.Name = "resulthidecurrenttype";
			this.resulthidecurrenttype.Size = new System.Drawing.Size(244, 22);
			this.resulthidecurrenttype.Text = "Hide results of this type(s)";
			this.resulthidecurrenttype.Click += new System.EventHandler(this.resulthidecurrenttype_Click);
			// 
			// resultshowonlycurrent
			// 
			this.resultshowonlycurrent.Image = global::CodeImp.DoomBuilder.BuilderModes.Properties.Resources.Show2;
			this.resultshowonlycurrent.Name = "resultshowonlycurrent";
			this.resultshowonlycurrent.Size = new System.Drawing.Size(244, 22);
			this.resultshowonlycurrent.Text = "Show only results of this type(s)";
			this.resultshowonlycurrent.Click += new System.EventHandler(this.resultshowonlycurrent_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(241, 6);
			// 
			// resultcopytoclipboard
			// 
			this.resultcopytoclipboard.Image = global::CodeImp.DoomBuilder.BuilderModes.Properties.Resources.Copy;
			this.resultcopytoclipboard.Name = "resultcopytoclipboard";
			this.resultcopytoclipboard.Size = new System.Drawing.Size(244, 22);
			this.resultcopytoclipboard.Text = "Copy description(s) to clipboard";
			this.resultcopytoclipboard.Click += new System.EventHandler(this.resultcopytoclipboard_Click);
			// 
			// resultspanel
			// 
			this.resultspanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.resultspanel.Controls.Add(this.fix3);
			this.resultspanel.Controls.Add(this.fix2);
			this.resultspanel.Controls.Add(this.resultinfo);
			this.resultspanel.Controls.Add(this.fix1);
			this.resultspanel.Controls.Add(this.progress);
			this.resultspanel.Controls.Add(this.results);
			this.resultspanel.Location = new System.Drawing.Point(0, 124);
			this.resultspanel.Name = "resultspanel";
			this.resultspanel.Size = new System.Drawing.Size(379, 442);
			this.resultspanel.TabIndex = 2;
			// 
			// fix3
			// 
			this.fix3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.fix3.Location = new System.Drawing.Point(256, 404);
			this.fix3.Name = "fix3";
			this.fix3.Size = new System.Drawing.Size(114, 26);
			this.fix3.TabIndex = 3;
			this.fix3.Text = "Fix 3";
			this.fix3.UseVisualStyleBackColor = true;
			this.fix3.Visible = false;
			this.fix3.Click += new System.EventHandler(this.fix3_Click);
			// 
			// fix2
			// 
			this.fix2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.fix2.Location = new System.Drawing.Point(133, 404);
			this.fix2.Name = "fix2";
			this.fix2.Size = new System.Drawing.Size(114, 26);
			this.fix2.TabIndex = 2;
			this.fix2.Text = "Fix 2";
			this.fix2.UseVisualStyleBackColor = true;
			this.fix2.Visible = false;
			this.fix2.Click += new System.EventHandler(this.fix2_Click);
			// 
			// resultinfo
			// 
			this.resultinfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.resultinfo.Enabled = false;
			this.resultinfo.Location = new System.Drawing.Point(12, 327);
			this.resultinfo.Name = "resultinfo";
			this.resultinfo.Size = new System.Drawing.Size(358, 74);
			this.resultinfo.TabIndex = 5;
			this.resultinfo.Text = "Select a result from the list to see more information.\r\nRight-click on a result t" +
				"o show context menu.";
			// 
			// fix1
			// 
			this.fix1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.fix1.Location = new System.Drawing.Point(10, 404);
			this.fix1.Name = "fix1";
			this.fix1.Size = new System.Drawing.Size(114, 26);
			this.fix1.TabIndex = 1;
			this.fix1.Text = "Fix 1";
			this.fix1.UseVisualStyleBackColor = true;
			this.fix1.Visible = false;
			this.fix1.Click += new System.EventHandler(this.fix1_Click);
			// 
			// progress
			// 
			this.progress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.progress.Location = new System.Drawing.Point(10, 3);
			this.progress.Margin = new System.Windows.Forms.Padding(1);
			this.progress.Name = "progress";
			this.progress.Size = new System.Drawing.Size(360, 18);
			this.progress.TabIndex = 3;
			this.progress.Value = 30;
			// 
			// closebutton
			// 
			this.closebutton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.closebutton.Location = new System.Drawing.Point(-500, 134);
			this.closebutton.Name = "closebutton";
			this.closebutton.Size = new System.Drawing.Size(116, 25);
			this.closebutton.TabIndex = 4;
			this.closebutton.Text = "Close";
			this.closebutton.UseVisualStyleBackColor = true;
			this.closebutton.Click += new System.EventHandler(this.closebutton_Click);
			// 
			// toggleall
			// 
			this.toggleall.AutoSize = true;
			this.toggleall.Checked = true;
			this.toggleall.CheckState = System.Windows.Forms.CheckState.Checked;
			this.toggleall.Location = new System.Drawing.Point(10, 12);
			this.toggleall.Name = "toggleall";
			this.toggleall.Size = new System.Drawing.Size(72, 17);
			this.toggleall.TabIndex = 5;
			this.toggleall.Text = "Toggle all";
			this.toggleall.UseVisualStyleBackColor = true;
			this.toggleall.CheckedChanged += new System.EventHandler(this.toggleall_CheckedChanged);
			// 
			// ErrorCheckForm
			// 
			this.AcceptButton = this.buttoncheck;
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.CancelButton = this.closebutton;
			this.ClientSize = new System.Drawing.Size(380, 566);
			this.Controls.Add(this.toggleall);
			this.Controls.Add(this.closebutton);
			this.Controls.Add(this.resultspanel);
			this.Controls.Add(this.buttoncheck);
			this.Controls.Add(this.checks);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ErrorCheckForm";
			this.Opacity = 0;
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "Map Analysis";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ErrorCheckForm_FormClosing);
			this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.ErrorCheckForm_HelpRequested);
			this.resultcontextmenustrip.ResumeLayout(false);
			this.resultspanel.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private CodeImp.DoomBuilder.Controls.CheckboxArrayControl checks;
		private System.Windows.Forms.Button buttoncheck;
		private System.Windows.Forms.ListBox results;
		private System.Windows.Forms.Panel resultspanel;
		private System.Windows.Forms.ProgressBar progress;
		private System.Windows.Forms.Button fix3;
		private System.Windows.Forms.Button fix2;
		private System.Windows.Forms.Label resultinfo;
		private System.Windows.Forms.Button fix1;
		private System.Windows.Forms.Button closebutton;
		private System.Windows.Forms.ContextMenuStrip resultcontextmenustrip;
		private System.Windows.Forms.ToolStripMenuItem resultshowall;
		private System.Windows.Forms.ToolStripMenuItem resulthidecurrenttype;
		private System.Windows.Forms.ToolStripMenuItem resultshowonlycurrent;
		private System.Windows.Forms.ToolStripMenuItem resultcopytoclipboard;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripMenuItem resulthidecurrent;
		private System.Windows.Forms.CheckBox toggleall;
		private System.Windows.Forms.ToolStripMenuItem resultselectcurrenttype;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
	}
}
