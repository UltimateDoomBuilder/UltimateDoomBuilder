namespace CodeImp.DoomBuilder.Controls
{
	partial class VertexInfoPanel
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.Windows.Forms.Label label1;
			this.vertexinfo = new System.Windows.Forms.GroupBox();
			this.panelOffsets = new System.Windows.Forms.Panel();
			this.zfloor = new System.Windows.Forms.Label();
			this.zfloor.AutoSize = true;
			this.labelzceiling = new System.Windows.Forms.Label();
			this.labelzceiling.AutoSize = true;
			this.zceiling = new System.Windows.Forms.Label();
			this.zceiling.AutoSize = true;
			this.labelzfloor = new System.Windows.Forms.Label();
			this.labelzfloor.AutoSize = true;
			this.position = new System.Windows.Forms.Label();
			this.position.AutoSize = true;
			label1 = new System.Windows.Forms.Label();
			this.vertexinfo.SuspendLayout();
			this.panelOffsets.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(35, 28);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(47, 13);
			label1.TabIndex = 2;
			label1.Text = "Position:";
			// 
			// vertexinfo
			// 
			this.vertexinfo.Controls.Add(this.panelOffsets);
			this.vertexinfo.Controls.Add(this.position);
			this.vertexinfo.Controls.Add(label1);
			this.vertexinfo.Location = new System.Drawing.Point(0, 0);
			this.vertexinfo.Name = "vertexinfo";
			this.vertexinfo.Size = new System.Drawing.Size(200, 100);
			this.vertexinfo.TabIndex = 0;
			this.vertexinfo.TabStop = false;
			this.vertexinfo.Text = " Vertex ";
			// 
			// panelOffsets
			// 
			this.panelOffsets.Controls.Add(this.zfloor);
			this.panelOffsets.Controls.Add(this.labelzceiling);
			this.panelOffsets.Controls.Add(this.zceiling);
			this.panelOffsets.Controls.Add(this.labelzfloor);
			this.panelOffsets.Location = new System.Drawing.Point(6, 49);
			this.panelOffsets.Name = "panelOffsets";
			this.panelOffsets.Size = new System.Drawing.Size(151, 46);
			this.panelOffsets.TabIndex = 1;
			// 
			// zfloor
			// 
			this.zfloor.AutoSize = true;
			this.zfloor.Location = new System.Drawing.Point(86, 26);
			this.zfloor.Name = "zfloor";
			this.zfloor.Size = new System.Drawing.Size(28, 13);
			this.zfloor.TabIndex = 7;
			this.zfloor.Text = "-512";
			// 
			// labelzceiling
			// 
			this.labelzceiling.AutoSize = true;
			this.labelzceiling.Location = new System.Drawing.Point(5, 3);
			this.labelzceiling.Name = "labelzceiling";
			this.labelzceiling.Size = new System.Drawing.Size(70, 13);
			this.labelzceiling.TabIndex = 4;
			this.labelzceiling.Text = "Ceiling offset:";
			// 
			// zceiling
			// 
			this.zceiling.AutoSize = true;
			this.zceiling.Location = new System.Drawing.Point(86, 3);
			this.zceiling.Name = "zceiling";
			this.zceiling.Size = new System.Drawing.Size(28, 13);
			this.zceiling.TabIndex = 6;
			this.zceiling.Text = "-512";
			// 
			// labelzfloor
			// 
			this.labelzfloor.AutoSize = true;
			this.labelzfloor.Location = new System.Drawing.Point(12, 26);
			this.labelzfloor.Name = "labelzfloor";
			this.labelzfloor.Size = new System.Drawing.Size(62, 13);
			this.labelzfloor.TabIndex = 5;
			this.labelzfloor.Text = "Floor offset:";
			// 
			// position
			// 
			this.position.AutoSize = true;
			this.position.Location = new System.Drawing.Point(92, 28);
			this.position.Name = "position";
			this.position.Size = new System.Drawing.Size(25, 13);
			this.position.TabIndex = 3;
			this.position.Text = "0, 0";
			// 
			// VertexInfoPanel
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.Controls.Add(this.vertexinfo);
			this.MaximumSize = new System.Drawing.Size(10000, 100);
			this.MinimumSize = new System.Drawing.Size(100, 100);
			this.Name = "VertexInfoPanel";
			this.Size = new System.Drawing.Size(393, 100);
			this.vertexinfo.ResumeLayout(false);
			this.vertexinfo.PerformLayout();
			this.panelOffsets.ResumeLayout(false);
			this.panelOffsets.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label position;
		private System.Windows.Forms.GroupBox vertexinfo;
		private System.Windows.Forms.Label zfloor;
		private System.Windows.Forms.Label zceiling;
		private System.Windows.Forms.Panel panelOffsets;
		private System.Windows.Forms.Label labelzceiling;
		private System.Windows.Forms.Label labelzfloor;

	}
}
