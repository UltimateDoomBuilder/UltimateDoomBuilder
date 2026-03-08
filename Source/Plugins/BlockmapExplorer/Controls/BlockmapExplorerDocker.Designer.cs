namespace CodeImp.DoomBuilder.BlockmapExplorer.Controls
{
	partial class BlockmapExplorerDocker
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
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.lblQuestionableOffsets = new System.Windows.Forms.Label();
			this.panel1 = new System.Windows.Forms.Panel();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.lblLumpSize = new System.Windows.Forms.Label();
			this.label12 = new System.Windows.Forms.Label();
			this.lblLinesNotInBlocks = new System.Windows.Forms.Label();
			this.label11 = new System.Windows.Forms.Label();
			this.lblTotalRows = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.lblTotalColumns = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.lblOffsetListEnd = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.lblUniqueBlocks = new System.Windows.Forms.Label();
			this.lblTotalBlocks = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.lblIsSublist = new System.Windows.Forms.Label();
			this.label13 = new System.Windows.Forms.Label();
			this.lblNumLinesInBlock = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.lblOffset = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.lblRow = new System.Windows.Forms.Label();
			this.lblColumn = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.cbShowQuestionableBlocks = new System.Windows.Forms.CheckBox();
			this.tableLayoutPanel1.SuspendLayout();
			this.panel1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tableLayoutPanel1.ColumnCount = 1;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Controls.Add(this.lblQuestionableOffsets, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.cbShowQuestionableBlocks, 0, 2);
			this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 3;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(275, 463);
			this.tableLayoutPanel1.TabIndex = 11;
			// 
			// lblQuestionableOffsets
			// 
			this.lblQuestionableOffsets.AutoSize = true;
			this.lblQuestionableOffsets.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lblQuestionableOffsets.Location = new System.Drawing.Point(3, 366);
			this.lblQuestionableOffsets.Name = "lblQuestionableOffsets";
			this.lblQuestionableOffsets.Size = new System.Drawing.Size(269, 13);
			this.lblQuestionableOffsets.TabIndex = 17;
			this.lblQuestionableOffsets.Text = "There are questionable offsets";
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.groupBox2);
			this.panel1.Controls.Add(this.groupBox1);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(3, 3);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(269, 360);
			this.panel1.TabIndex = 12;
			// 
			// groupBox2
			// 
			this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox2.Controls.Add(this.lblLumpSize);
			this.groupBox2.Controls.Add(this.label12);
			this.groupBox2.Controls.Add(this.lblLinesNotInBlocks);
			this.groupBox2.Controls.Add(this.label11);
			this.groupBox2.Controls.Add(this.lblTotalRows);
			this.groupBox2.Controls.Add(this.label10);
			this.groupBox2.Controls.Add(this.lblTotalColumns);
			this.groupBox2.Controls.Add(this.label9);
			this.groupBox2.Controls.Add(this.lblOffsetListEnd);
			this.groupBox2.Controls.Add(this.label6);
			this.groupBox2.Controls.Add(this.lblUniqueBlocks);
			this.groupBox2.Controls.Add(this.lblTotalBlocks);
			this.groupBox2.Controls.Add(this.label5);
			this.groupBox2.Controls.Add(this.label4);
			this.groupBox2.Location = new System.Drawing.Point(3, 153);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(263, 193);
			this.groupBox2.TabIndex = 21;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Blockmap info";
			// 
			// lblLumpSize
			// 
			this.lblLumpSize.AutoSize = true;
			this.lblLumpSize.Location = new System.Drawing.Point(111, 142);
			this.lblLumpSize.Name = "lblLumpSize";
			this.lblLumpSize.Size = new System.Drawing.Size(13, 13);
			this.lblLumpSize.TabIndex = 31;
			this.lblLumpSize.Text = "0";
			// 
			// label12
			// 
			this.label12.AutoSize = true;
			this.label12.Location = new System.Drawing.Point(12, 142);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(57, 13);
			this.label12.TabIndex = 30;
			this.label12.Text = "Lump size:";
			// 
			// lblLinesNotInBlocks
			// 
			this.lblLinesNotInBlocks.AutoSize = true;
			this.lblLinesNotInBlocks.Location = new System.Drawing.Point(111, 119);
			this.lblLinesNotInBlocks.Name = "lblLinesNotInBlocks";
			this.lblLinesNotInBlocks.Size = new System.Drawing.Size(13, 13);
			this.lblLinesNotInBlocks.TabIndex = 29;
			this.lblLinesNotInBlocks.Text = "0";
			// 
			// label11
			// 
			this.label11.AutoSize = true;
			this.label11.Location = new System.Drawing.Point(12, 119);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(98, 13);
			this.label11.TabIndex = 28;
			this.label11.Text = "Lines not in blocks:";
			// 
			// lblTotalRows
			// 
			this.lblTotalRows.AutoSize = true;
			this.lblTotalRows.Location = new System.Drawing.Point(111, 50);
			this.lblTotalRows.Name = "lblTotalRows";
			this.lblTotalRows.Size = new System.Drawing.Size(13, 13);
			this.lblTotalRows.TabIndex = 27;
			this.lblTotalRows.Text = "0";
			// 
			// label10
			// 
			this.label10.AutoSize = true;
			this.label10.Location = new System.Drawing.Point(13, 50);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(37, 13);
			this.label10.TabIndex = 26;
			this.label10.Text = "Rows:";
			// 
			// lblTotalColumns
			// 
			this.lblTotalColumns.AutoSize = true;
			this.lblTotalColumns.Location = new System.Drawing.Point(111, 27);
			this.lblTotalColumns.Name = "lblTotalColumns";
			this.lblTotalColumns.Size = new System.Drawing.Size(13, 13);
			this.lblTotalColumns.TabIndex = 25;
			this.lblTotalColumns.Text = "0";
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point(12, 27);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(50, 13);
			this.label9.TabIndex = 24;
			this.label9.Text = "Columns:";
			// 
			// lblOffsetListEnd
			// 
			this.lblOffsetListEnd.AutoSize = true;
			this.lblOffsetListEnd.Location = new System.Drawing.Point(111, 165);
			this.lblOffsetListEnd.Name = "lblOffsetListEnd";
			this.lblOffsetListEnd.Size = new System.Drawing.Size(13, 13);
			this.lblOffsetListEnd.TabIndex = 23;
			this.lblOffsetListEnd.Text = "0";
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(13, 165);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(74, 13);
			this.label6.TabIndex = 22;
			this.label6.Text = "Offset list end:";
			// 
			// lblUniqueBlocks
			// 
			this.lblUniqueBlocks.AutoSize = true;
			this.lblUniqueBlocks.Location = new System.Drawing.Point(111, 94);
			this.lblUniqueBlocks.Name = "lblUniqueBlocks";
			this.lblUniqueBlocks.Size = new System.Drawing.Size(13, 13);
			this.lblUniqueBlocks.TabIndex = 21;
			this.lblUniqueBlocks.Text = "0";
			// 
			// lblTotalBlocks
			// 
			this.lblTotalBlocks.AutoSize = true;
			this.lblTotalBlocks.Location = new System.Drawing.Point(111, 71);
			this.lblTotalBlocks.Name = "lblTotalBlocks";
			this.lblTotalBlocks.Size = new System.Drawing.Size(13, 13);
			this.lblTotalBlocks.TabIndex = 20;
			this.lblTotalBlocks.Text = "0";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(12, 96);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(78, 13);
			this.label5.TabIndex = 19;
			this.label5.Text = "Unique blocks:";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(12, 73);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(68, 13);
			this.label4.TabIndex = 18;
			this.label4.Text = "Total blocks:";
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.lblIsSublist);
			this.groupBox1.Controls.Add(this.label13);
			this.groupBox1.Controls.Add(this.lblNumLinesInBlock);
			this.groupBox1.Controls.Add(this.label7);
			this.groupBox1.Controls.Add(this.lblOffset);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.lblRow);
			this.groupBox1.Controls.Add(this.lblColumn);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Location = new System.Drawing.Point(3, 3);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(263, 144);
			this.groupBox1.TabIndex = 20;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Block info";
			// 
			// lblIsSublist
			// 
			this.lblIsSublist.AutoSize = true;
			this.lblIsSublist.Location = new System.Drawing.Point(111, 95);
			this.lblIsSublist.Name = "lblIsSublist";
			this.lblIsSublist.Size = new System.Drawing.Size(13, 13);
			this.lblIsSublist.TabIndex = 29;
			this.lblIsSublist.Text = "0";
			// 
			// label13
			// 
			this.label13.AutoSize = true;
			this.label13.Location = new System.Drawing.Point(12, 95);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(50, 13);
			this.label13.TabIndex = 28;
			this.label13.Text = "Is sublist:";
			// 
			// lblNumLinesInBlock
			// 
			this.lblNumLinesInBlock.AutoSize = true;
			this.lblNumLinesInBlock.Location = new System.Drawing.Point(111, 118);
			this.lblNumLinesInBlock.Name = "lblNumLinesInBlock";
			this.lblNumLinesInBlock.Size = new System.Drawing.Size(13, 13);
			this.lblNumLinesInBlock.TabIndex = 27;
			this.lblNumLinesInBlock.Text = "0";
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(12, 118);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(75, 13);
			this.label7.TabIndex = 26;
			this.label7.Text = "Lines in block:";
			// 
			// lblOffset
			// 
			this.lblOffset.AutoSize = true;
			this.lblOffset.Location = new System.Drawing.Point(111, 72);
			this.lblOffset.Name = "lblOffset";
			this.lblOffset.Size = new System.Drawing.Size(13, 13);
			this.lblOffset.TabIndex = 25;
			this.lblOffset.Text = "0";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(12, 72);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(72, 13);
			this.label3.TabIndex = 24;
			this.label3.Text = "Offset (bytes):";
			// 
			// lblRow
			// 
			this.lblRow.AutoSize = true;
			this.lblRow.Location = new System.Drawing.Point(111, 49);
			this.lblRow.Name = "lblRow";
			this.lblRow.Size = new System.Drawing.Size(13, 13);
			this.lblRow.TabIndex = 23;
			this.lblRow.Text = "0";
			// 
			// lblColumn
			// 
			this.lblColumn.AutoSize = true;
			this.lblColumn.Location = new System.Drawing.Point(111, 26);
			this.lblColumn.Name = "lblColumn";
			this.lblColumn.Size = new System.Drawing.Size(13, 13);
			this.lblColumn.TabIndex = 22;
			this.lblColumn.Text = "0";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(12, 49);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(32, 13);
			this.label2.TabIndex = 21;
			this.label2.Text = "Row:";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 26);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(45, 13);
			this.label1.TabIndex = 20;
			this.label1.Text = "Column:";
			// 
			// cbShowQuestionableBlocks
			// 
			this.cbShowQuestionableBlocks.AutoSize = true;
			this.cbShowQuestionableBlocks.Location = new System.Drawing.Point(3, 389);
			this.cbShowQuestionableBlocks.Margin = new System.Windows.Forms.Padding(3, 10, 3, 3);
			this.cbShowQuestionableBlocks.Name = "cbShowQuestionableBlocks";
			this.cbShowQuestionableBlocks.Size = new System.Drawing.Size(150, 17);
			this.cbShowQuestionableBlocks.TabIndex = 18;
			this.cbShowQuestionableBlocks.Text = "Show questionable blocks";
			this.cbShowQuestionableBlocks.UseVisualStyleBackColor = true;
			// 
			// BlockmapExplorerDocker
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tableLayoutPanel1);
			this.Name = "BlockmapExplorerDocker";
			this.Size = new System.Drawing.Size(281, 608);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.panel1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Label lblQuestionableOffsets;
		private System.Windows.Forms.CheckBox cbShowQuestionableBlocks;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Label lblOffsetListEnd;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label lblUniqueBlocks;
		private System.Windows.Forms.Label lblTotalBlocks;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label lblNumLinesInBlock;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label lblOffset;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label lblRow;
		private System.Windows.Forms.Label lblColumn;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label lblLinesNotInBlocks;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Label lblTotalRows;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label lblTotalColumns;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label lblLumpSize;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.Label lblIsSublist;
		private System.Windows.Forms.Label label13;
	}
}
