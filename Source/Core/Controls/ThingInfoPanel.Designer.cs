namespace CodeImp.DoomBuilder.Controls
{
	partial class ThingInfoPanel
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
			System.Windows.Forms.Label label5;
			System.Windows.Forms.Label label4;
			System.Windows.Forms.Label label3;
			System.Windows.Forms.Label label1;
			this.labelaction = new System.Windows.Forms.Label();
			this.labelaction.AutoSize = true;
			this.infopanel = new System.Windows.Forms.GroupBox();
			this.anglecontrol = new CodeImp.DoomBuilder.Controls.AngleControlEx();
			this.classname = new System.Windows.Forms.Label();
			this.classname.AutoSize = true;
			this.labelclass = new System.Windows.Forms.Label();
			this.labelclass.AutoSize = true;
			this.arg5 = new System.Windows.Forms.Label();
			this.arg5.AutoSize = true;
			this.arglbl5 = new System.Windows.Forms.Label();
			this.arglbl5.AutoSize = true;
			this.arglbl4 = new System.Windows.Forms.Label();
			this.arglbl4.AutoSize = true;
			this.arg4 = new System.Windows.Forms.Label();
			this.arg4.AutoSize = true;
			this.arglbl3 = new System.Windows.Forms.Label();
			this.arglbl3.AutoSize = true;
			this.arglbl2 = new System.Windows.Forms.Label();
			this.arglbl2.AutoSize = true;
			this.arg3 = new System.Windows.Forms.Label();
			this.arg3.AutoSize = true;
			this.arglbl1 = new System.Windows.Forms.Label();
			this.arglbl1.AutoSize = true;
			this.arg2 = new System.Windows.Forms.Label();
			this.arg2.AutoSize = true;
			this.arg1 = new System.Windows.Forms.Label();
			this.arg1.AutoSize = true;
			this.angle = new System.Windows.Forms.Label();
			this.angle.AutoSize = true;
			this.tag = new System.Windows.Forms.Label();
			this.tag.AutoSize = true;
			this.position = new System.Windows.Forms.Label();
			this.position.AutoSize = true;
			this.action = new System.Windows.Forms.Label();
			this.action.AutoSize = true;
			this.type = new System.Windows.Forms.Label();
			this.type.AutoSize = true;
			this.spritepanel = new System.Windows.Forms.GroupBox();
			this.spritename = new System.Windows.Forms.Label();
			this.spritename.AutoSize = true;
			this.panel1 = new System.Windows.Forms.Panel();
			this.spritetex = new CodeImp.DoomBuilder.Controls.ConfigurablePictureBox();
			this.flagsPanel = new System.Windows.Forms.GroupBox();
			this.flags = new CodeImp.DoomBuilder.Controls.TransparentListView();
			label5 = new System.Windows.Forms.Label();
			label4 = new System.Windows.Forms.Label();
			label3 = new System.Windows.Forms.Label();
			label1 = new System.Windows.Forms.Label();
			this.infopanel.SuspendLayout();
			this.spritepanel.SuspendLayout();
			this.panel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.spritetex)).BeginInit();
			this.flagsPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// label5
			// 
			label5.AutoSize = true;
			label5.Location = new System.Drawing.Point(165, 79);
			label5.Name = "label5";
			label5.Size = new System.Drawing.Size(37, 13);
			label5.TabIndex = 8;
			label5.Text = "Angle:";
			// 
			// label4
			// 
			label4.AutoSize = true;
			label4.Location = new System.Drawing.Point(30, 79);
			label4.Name = "label4";
			label4.Size = new System.Drawing.Size(29, 13);
			label4.TabIndex = 4;
			label4.Text = "Tag:";
			// 
			// label3
			// 
			label3.AutoSize = true;
			label3.Location = new System.Drawing.Point(11, 64);
			label3.Name = "label3";
			label3.Size = new System.Drawing.Size(47, 13);
			label3.TabIndex = 3;
			label3.Text = "Position:";
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(24, 19);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(34, 13);
			label1.TabIndex = 0;
			label1.Text = "Type:";
			// 
			// labelaction
			// 
			this.labelaction.AutoSize = true;
			this.labelaction.Location = new System.Drawing.Point(18, 49);
			this.labelaction.Name = "labelaction";
			this.labelaction.Size = new System.Drawing.Size(40, 13);
			this.labelaction.TabIndex = 2;
			this.labelaction.Text = "Action:";
			// 
			// infopanel
			// 
			this.infopanel.Controls.Add(this.anglecontrol);
			this.infopanel.Controls.Add(this.classname);
			this.infopanel.Controls.Add(this.labelclass);
			this.infopanel.Controls.Add(this.arg5);
			this.infopanel.Controls.Add(this.arglbl5);
			this.infopanel.Controls.Add(this.arglbl4);
			this.infopanel.Controls.Add(this.arg4);
			this.infopanel.Controls.Add(this.arglbl3);
			this.infopanel.Controls.Add(this.arglbl2);
			this.infopanel.Controls.Add(this.arg3);
			this.infopanel.Controls.Add(this.arglbl1);
			this.infopanel.Controls.Add(this.arg2);
			this.infopanel.Controls.Add(this.arg1);
			this.infopanel.Controls.Add(this.angle);
			this.infopanel.Controls.Add(label5);
			this.infopanel.Controls.Add(this.tag);
			this.infopanel.Controls.Add(this.position);
			this.infopanel.Controls.Add(this.action);
			this.infopanel.Controls.Add(label4);
			this.infopanel.Controls.Add(label3);
			this.infopanel.Controls.Add(this.labelaction);
			this.infopanel.Controls.Add(this.type);
			this.infopanel.Controls.Add(label1);
			this.infopanel.Location = new System.Drawing.Point(0, 0);
			this.infopanel.Name = "infopanel";
			this.infopanel.Size = new System.Drawing.Size(473, 100);
			this.infopanel.TabIndex = 4;
			this.infopanel.TabStop = false;
			this.infopanel.Text = " Thing ";
			// 
			// anglecontrol
			// 
			this.anglecontrol.Angle = 0;
			this.anglecontrol.AngleOffset = 0;
			this.anglecontrol.Location = new System.Drawing.Point(235, 73);
			this.anglecontrol.Name = "anglecontrol";
			this.anglecontrol.Size = new System.Drawing.Size(24, 24);
			this.anglecontrol.TabIndex = 38;
			// 
			// classname
			// 
			this.classname.AutoEllipsis = true;
			this.classname.Location = new System.Drawing.Point(61, 34);
			this.classname.Name = "classname";
			this.classname.Size = new System.Drawing.Size(210, 14);
			this.classname.TabIndex = 40;
			this.classname.Text = "CrazyFlyingShotgunSpawner";
			// 
			// labelclass
			// 
			this.labelclass.AutoSize = true;
			this.labelclass.Location = new System.Drawing.Point(23, 34);
			this.labelclass.Name = "labelclass";
			this.labelclass.Size = new System.Drawing.Size(35, 13);
			this.labelclass.TabIndex = 39;
			this.labelclass.Text = "Class:";
			// 
			// arg5
			// 
			this.arg5.AutoEllipsis = true;
			this.arg5.Location = new System.Drawing.Point(384, 79);
			this.arg5.Name = "arg5";
			this.arg5.Size = new System.Drawing.Size(83, 14);
			this.arg5.TabIndex = 37;
			this.arg5.Text = "Arg 1:";
			// 
			// arglbl5
			// 
			this.arglbl5.AutoEllipsis = true;
			this.arglbl5.BackColor = System.Drawing.Color.Transparent;
			this.arglbl5.Location = new System.Drawing.Point(257, 79);
			this.arglbl5.Name = "arglbl5";
			this.arglbl5.Size = new System.Drawing.Size(121, 14);
			this.arglbl5.TabIndex = 32;
			this.arglbl5.Text = "Arg 1:";
			this.arglbl5.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// arglbl4
			// 
			this.arglbl4.AutoEllipsis = true;
			this.arglbl4.BackColor = System.Drawing.Color.Transparent;
			this.arglbl4.Location = new System.Drawing.Point(257, 64);
			this.arglbl4.Name = "arglbl4";
			this.arglbl4.Size = new System.Drawing.Size(121, 14);
			this.arglbl4.TabIndex = 31;
			this.arglbl4.Text = "Arg 1:";
			this.arglbl4.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// arg4
			// 
			this.arg4.AutoEllipsis = true;
			this.arg4.Location = new System.Drawing.Point(384, 64);
			this.arg4.Name = "arg4";
			this.arg4.Size = new System.Drawing.Size(83, 14);
			this.arg4.TabIndex = 36;
			this.arg4.Text = "Arg 1:";
			// 
			// arglbl3
			// 
			this.arglbl3.AutoEllipsis = true;
			this.arglbl3.BackColor = System.Drawing.Color.Transparent;
			this.arglbl3.Location = new System.Drawing.Point(257, 49);
			this.arglbl3.Name = "arglbl3";
			this.arglbl3.Size = new System.Drawing.Size(121, 14);
			this.arglbl3.TabIndex = 30;
			this.arglbl3.Text = "Arg 1:";
			this.arglbl3.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// arglbl2
			// 
			this.arglbl2.AutoEllipsis = true;
			this.arglbl2.BackColor = System.Drawing.Color.Transparent;
			this.arglbl2.Location = new System.Drawing.Point(257, 34);
			this.arglbl2.Name = "arglbl2";
			this.arglbl2.Size = new System.Drawing.Size(121, 14);
			this.arglbl2.TabIndex = 29;
			this.arglbl2.Text = "Arg 1:";
			this.arglbl2.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// arg3
			// 
			this.arg3.AutoEllipsis = true;
			this.arg3.Location = new System.Drawing.Point(384, 49);
			this.arg3.Name = "arg3";
			this.arg3.Size = new System.Drawing.Size(83, 14);
			this.arg3.TabIndex = 35;
			this.arg3.Text = "Arg 1:";
			// 
			// arglbl1
			// 
			this.arglbl1.AutoEllipsis = true;
			this.arglbl1.BackColor = System.Drawing.Color.Transparent;
			this.arglbl1.Location = new System.Drawing.Point(257, 19);
			this.arglbl1.Name = "arglbl1";
			this.arglbl1.Size = new System.Drawing.Size(121, 14);
			this.arglbl1.TabIndex = 28;
			this.arglbl1.Text = "Arg 1:";
			this.arglbl1.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// arg2
			// 
			this.arg2.AutoEllipsis = true;
			this.arg2.Location = new System.Drawing.Point(384, 34);
			this.arg2.Name = "arg2";
			this.arg2.Size = new System.Drawing.Size(83, 14);
			this.arg2.TabIndex = 34;
			this.arg2.Text = "Arg 1:";
			// 
			// arg1
			// 
			this.arg1.AutoEllipsis = true;
			this.arg1.Location = new System.Drawing.Point(384, 19);
			this.arg1.Name = "arg1";
			this.arg1.Size = new System.Drawing.Size(83, 14);
			this.arg1.TabIndex = 33;
			this.arg1.Text = "Arg 1:";
			// 
			// angle
			// 
			this.angle.AutoSize = true;
			this.angle.Location = new System.Drawing.Point(206, 79);
			this.angle.Name = "angle";
			this.angle.Size = new System.Drawing.Size(25, 13);
			this.angle.TabIndex = 11;
			this.angle.Text = "270";
			// 
			// tag
			// 
			this.tag.AutoSize = true;
			this.tag.Location = new System.Drawing.Point(61, 79);
			this.tag.Name = "tag";
			this.tag.Size = new System.Drawing.Size(13, 13);
			this.tag.TabIndex = 7;
			this.tag.Text = "0";
			// 
			// position
			// 
			this.position.AutoSize = true;
			this.position.Location = new System.Drawing.Point(61, 64);
			this.position.Name = "position";
			this.position.Size = new System.Drawing.Size(91, 13);
			this.position.TabIndex = 6;
			this.position.Text = "1024, 1024, 1024";
			// 
			// action
			// 
			this.action.AutoEllipsis = true;
			this.action.Location = new System.Drawing.Point(61, 49);
			this.action.Name = "action";
			this.action.Size = new System.Drawing.Size(210, 14);
			this.action.TabIndex = 5;
			this.action.Text = "0 - Spawn a Blue Poopie and Ammo";
			// 
			// type
			// 
			this.type.AutoSize = true;
			this.type.Location = new System.Drawing.Point(61, 19);
			this.type.Name = "type";
			this.type.Size = new System.Drawing.Size(96, 13);
			this.type.TabIndex = 1;
			this.type.Text = "0 - Big Brown Pimp";
			// 
			// spritepanel
			// 
			this.spritepanel.Controls.Add(this.spritename);
			this.spritepanel.Controls.Add(this.panel1);
			this.spritepanel.Location = new System.Drawing.Point(479, 0);
			this.spritepanel.Name = "spritepanel";
			this.spritepanel.Size = new System.Drawing.Size(86, 100);
			this.spritepanel.TabIndex = 5;
			this.spritepanel.TabStop = false;
			this.spritepanel.Text = " Sprite ";
			// 
			// spritename
			// 
			this.spritename.Location = new System.Drawing.Point(7, 79);
			this.spritename.Name = "spritename";
			this.spritename.Size = new System.Drawing.Size(72, 13);
			this.spritename.TabIndex = 1;
			this.spritename.Text = "BROWNHUG";
			this.spritename.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// panel1
			// 
			this.panel1.BackColor = System.Drawing.SystemColors.AppWorkspace;
			this.panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.panel1.Controls.Add(this.spritetex);
			this.panel1.Location = new System.Drawing.Point(12, 14);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(64, 64);
			this.panel1.TabIndex = 0;
			// 
			// spritetex
			// 
			this.spritetex.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.Default;
			this.spritetex.Dock = System.Windows.Forms.DockStyle.Fill;
			this.spritetex.Highlighted = false;
			this.spritetex.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
			this.spritetex.Location = new System.Drawing.Point(0, 0);
			this.spritetex.Name = "spritetex";
			this.spritetex.PageUnit = System.Drawing.GraphicsUnit.Pixel;
			this.spritetex.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.None;
			this.spritetex.Size = new System.Drawing.Size(60, 60);
			this.spritetex.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.spritetex.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.Default;
			this.spritetex.TabIndex = 0;
			this.spritetex.TabStop = false;
			// 
			// flagsPanel
			// 
			this.flagsPanel.Controls.Add(this.flags);
			this.flagsPanel.Location = new System.Drawing.Point(571, 0);
			this.flagsPanel.Name = "flagsPanel";
			this.flagsPanel.Size = new System.Drawing.Size(568, 100);
			this.flagsPanel.TabIndex = 6;
			this.flagsPanel.TabStop = false;
			this.flagsPanel.Text = " Flags";
			// 
			// flags
			// 
			this.flags.BackColor = System.Drawing.SystemColors.Control;
			this.flags.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.flags.CheckBoxes = true;
			this.flags.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.flags.Location = new System.Drawing.Point(6, 18);
			this.flags.Name = "flags";
			this.flags.Scrollable = false;
			this.flags.ShowGroups = false;
			this.flags.Size = new System.Drawing.Size(556, 88);
			this.flags.TabIndex = 0;
			this.flags.UseCompatibleStateImageBehavior = false;
			this.flags.View = System.Windows.Forms.View.List;
			// 
			// ThingInfoPanel
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.Controls.Add(this.flagsPanel);
			this.Controls.Add(this.spritepanel);
			this.Controls.Add(this.infopanel);
			this.MaximumSize = new System.Drawing.Size(10000, 100);
			this.MinimumSize = new System.Drawing.Size(100, 100);
			this.Name = "ThingInfoPanel";
			this.Size = new System.Drawing.Size(1145, 100);
			this.infopanel.ResumeLayout(false);
			this.infopanel.PerformLayout();
			this.spritepanel.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.spritetex)).EndInit();
			this.flagsPanel.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox spritepanel;
		private System.Windows.Forms.Label spritename;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Label angle;
		private System.Windows.Forms.Label tag;
		private System.Windows.Forms.Label position;
		private System.Windows.Forms.Label action;
		private System.Windows.Forms.Label type;
		private System.Windows.Forms.Label arg5;
		private System.Windows.Forms.Label arglbl5;
		private System.Windows.Forms.Label arglbl4;
		private System.Windows.Forms.Label arg4;
		private System.Windows.Forms.Label arglbl3;
		private System.Windows.Forms.Label arglbl2;
		private System.Windows.Forms.Label arg3;
		private System.Windows.Forms.Label arglbl1;
		private System.Windows.Forms.Label arg2;
		private System.Windows.Forms.Label arg1;
		private System.Windows.Forms.GroupBox infopanel;
		private System.Windows.Forms.GroupBox flagsPanel;
		private CodeImp.DoomBuilder.Controls.TransparentListView flags;
		private System.Windows.Forms.Label labelaction;
		private CodeImp.DoomBuilder.Controls.AngleControlEx anglecontrol;
		private ConfigurablePictureBox spritetex;
		private System.Windows.Forms.Label classname;
		private System.Windows.Forms.Label labelclass;

	}
}
