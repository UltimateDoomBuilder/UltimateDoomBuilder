namespace CodeImp.DoomBuilder.UDBScript
{
	partial class ScriptDockerControl
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
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.btnResetToDefaults = new System.Windows.Forms.Button();
			this.btnRunScript = new System.Windows.Forms.Button();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.label5 = new System.Windows.Forms.Label();
			this.label5.AutoSize = true;
			this.btnClearFilter = new System.Windows.Forms.Button();
			this.tbFilter = new System.Windows.Forms.TextBox();
			this.filetree = new CodeImp.DoomBuilder.Controls.MultiSelectTreeview();
			this.tbDescription = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.label3.AutoSize = true;
			this.label4 = new System.Windows.Forms.Label();
			this.label4.AutoSize = true;
			this.label1 = new System.Windows.Forms.Label();
			this.label1.AutoSize = true;
			this.label2 = new System.Windows.Forms.Label();
			this.label2.AutoSize = true;
			this.scriptoptions = new CodeImp.DoomBuilder.UDBScript.ScriptOptionsControl();
			this.tableLayoutPanel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnResetToDefaults
			// 
			this.btnResetToDefaults.Dock = System.Windows.Forms.DockStyle.Fill;
			this.btnResetToDefaults.Location = new System.Drawing.Point(3, 3);
			this.btnResetToDefaults.Name = "btnResetToDefaults";
			this.btnResetToDefaults.Size = new System.Drawing.Size(153, 27);
			this.btnResetToDefaults.TabIndex = 26;
			this.btnResetToDefaults.Text = "Reset";
			this.btnResetToDefaults.UseVisualStyleBackColor = true;
			this.btnResetToDefaults.Click += new System.EventHandler(this.btnResetToDefaults_Click);
			// 
			// btnRunScript
			// 
			this.btnRunScript.Dock = System.Windows.Forms.DockStyle.Fill;
			this.btnRunScript.Location = new System.Drawing.Point(162, 3);
			this.btnRunScript.Name = "btnRunScript";
			this.btnRunScript.Size = new System.Drawing.Size(153, 27);
			this.btnRunScript.TabIndex = 27;
			this.btnRunScript.Text = "Run";
			this.btnRunScript.UseVisualStyleBackColor = true;
			this.btnRunScript.Click += new System.EventHandler(this.btnRunScript_Click);
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tableLayoutPanel1.ColumnCount = 2;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.Controls.Add(this.btnResetToDefaults, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.btnRunScript, 1, 0);
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 505);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 1;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(318, 33);
			this.tableLayoutPanel1.TabIndex = 28;
			// 
			// splitContainer1
			// 
			this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.splitContainer1.Location = new System.Drawing.Point(3, 3);
			this.splitContainer1.Name = "splitContainer1";
			this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.label5);
			this.splitContainer1.Panel1.Controls.Add(this.btnClearFilter);
			this.splitContainer1.Panel1.Controls.Add(this.tbFilter);
			this.splitContainer1.Panel1.Controls.Add(this.filetree);
			this.splitContainer1.Panel1.RightToLeft = System.Windows.Forms.RightToLeft.No;
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.tbDescription);
			this.splitContainer1.Panel2.Controls.Add(this.label3);
			this.splitContainer1.Panel2.Controls.Add(this.label4);
			this.splitContainer1.Panel2.Controls.Add(this.label1);
			this.splitContainer1.Panel2.Controls.Add(this.label2);
			this.splitContainer1.Panel2.Controls.Add(this.scriptoptions);
			this.splitContainer1.Panel2.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.splitContainer1.Size = new System.Drawing.Size(312, 496);
			this.splitContainer1.SplitterDistance = 239;
			this.splitContainer1.TabIndex = 25;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(3, 9);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(32, 13);
			this.label5.TabIndex = 27;
			this.label5.Text = "Filter:";
			// 
			// btnClearFilter
			// 
			this.btnClearFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnClearFilter.Image = global::CodeImp.DoomBuilder.UDBScript.Properties.Resources.SearchClear;
			this.btnClearFilter.Location = new System.Drawing.Point(291, 5);
			this.btnClearFilter.Name = "btnClearFilter";
			this.btnClearFilter.Size = new System.Drawing.Size(22, 22);
			this.btnClearFilter.TabIndex = 26;
			this.btnClearFilter.UseVisualStyleBackColor = true;
			this.btnClearFilter.Click += new System.EventHandler(this.btnClearFilter_Click);
			// 
			// tbFilter
			// 
			this.tbFilter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tbFilter.Location = new System.Drawing.Point(41, 6);
			this.tbFilter.Name = "tbFilter";
			this.tbFilter.Size = new System.Drawing.Size(244, 20);
			this.tbFilter.TabIndex = 25;
			this.tbFilter.TextChanged += new System.EventHandler(this.tbFilter_TextChanged);
			// 
			// filetree
			// 
			this.filetree.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.filetree.HideSelection = false;
			this.filetree.Location = new System.Drawing.Point(0, 37);
			this.filetree.Margin = new System.Windows.Forms.Padding(8, 8, 9, 8);
			this.filetree.Name = "filetree";
			this.filetree.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			this.filetree.SelectionMode = CodeImp.DoomBuilder.Controls.TreeViewSelectionMode.SingleSelect;
			this.filetree.ShowNodeToolTips = true;
			this.filetree.Size = new System.Drawing.Size(312, 202);
			this.filetree.TabIndex = 24;
			this.filetree.BeforeCollapse += new System.Windows.Forms.TreeViewCancelEventHandler(this.filetree_BeforeCollapse);
			this.filetree.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.filetree_BeforeExpand);
			this.filetree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.filetree_AfterSelect);
			this.filetree.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.filetree_NodeMouseClick);
			// 
			// tbDescription
			// 
			this.tbDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tbDescription.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.tbDescription.Location = new System.Drawing.Point(3, 19);
			this.tbDescription.Margin = new System.Windows.Forms.Padding(0, 0, 3, 3);
			this.tbDescription.Multiline = true;
			this.tbDescription.Name = "tbDescription";
			this.tbDescription.ReadOnly = true;
			this.tbDescription.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.tbDescription.Size = new System.Drawing.Size(306, 101);
			this.tbDescription.TabIndex = 30;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label3.Location = new System.Drawing.Point(3, 3);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(88, 13);
			this.label3.TabIndex = 29;
			this.label3.Text = "Script description";
			// 
			// label4
			// 
			this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label4.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.label4.Location = new System.Drawing.Point(0, 10);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(313, 2);
			this.label4.TabIndex = 28;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(3, 123);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(71, 13);
			this.label1.TabIndex = 27;
			this.label1.Text = "Script options";
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.label2.Location = new System.Drawing.Point(0, 130);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(313, 2);
			this.label2.TabIndex = 2;
			// 
			// scriptoptions
			// 
			this.scriptoptions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.scriptoptions.Location = new System.Drawing.Point(0, 140);
			this.scriptoptions.Name = "scriptoptions";
			this.scriptoptions.Size = new System.Drawing.Size(312, 110);
			this.scriptoptions.TabIndex = 26;
			// 
			// ScriptDockerControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tableLayoutPanel1);
			this.Controls.Add(this.splitContainer1);
			this.Name = "ScriptDockerControl";
			this.Size = new System.Drawing.Size(318, 541);
			this.VisibleChanged += new System.EventHandler(this.ScriptDockerControl_VisibleChanged);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel1.PerformLayout();
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.Panel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.SplitContainer splitContainer1;
		private Controls.MultiSelectTreeview filetree;
		private System.Windows.Forms.Button btnResetToDefaults;
		private System.Windows.Forms.Button btnRunScript;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private ScriptOptionsControl scriptoptions;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox tbDescription;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Button btnClearFilter;
		private System.Windows.Forms.TextBox tbFilter;
	}
}
