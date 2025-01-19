namespace CodeImp.DoomBuilder.Controls
{
	partial class SectorInfoPanel
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
			System.Windows.Forms.Label label13;
			System.Windows.Forms.Label label5;
			this.labelfloor = new System.Windows.Forms.Label();
			this.labelfloor.AutoSize = true;
			this.labelceiling = new System.Windows.Forms.Label();
			this.labelceiling.AutoSize = true;
			this.taglabel = new System.Windows.Forms.Label();
			this.taglabel.AutoSize = true;
			this.effectlabel = new System.Windows.Forms.Label();
			this.effectlabel.AutoSize = true;
			this.ceilingLightLabel = new System.Windows.Forms.Label();
			this.ceilingLightLabel.AutoSize = true;
			this.ceilingScaleLabel = new System.Windows.Forms.Label();
			this.ceilingScaleLabel.AutoSize = true;
			this.ceilingAngleLabel = new System.Windows.Forms.Label();
			this.ceilingAngleLabel.AutoSize = true;
			this.ceilingOffsetLabel = new System.Windows.Forms.Label();
			this.ceilingOffsetLabel.AutoSize = true;
			this.floorOffsetLabel = new System.Windows.Forms.Label();
			this.floorOffsetLabel.AutoSize = true;
			this.floorLightLabel = new System.Windows.Forms.Label();
			this.floorLightLabel.AutoSize = true;
			this.floorAngleLabel = new System.Windows.Forms.Label();
			this.floorAngleLabel.AutoSize = true;
			this.floorScaleLabel = new System.Windows.Forms.Label();
			this.floorScaleLabel.AutoSize = true;
			this.sectorinfo = new System.Windows.Forms.GroupBox();
			this.panelFadeColor = new System.Windows.Forms.Panel();
			this.panelLightColor = new System.Windows.Forms.Panel();
			this.labelFade = new System.Windows.Forms.Label();
			this.labelFade.AutoSize = true;
			this.labelLight = new System.Windows.Forms.Label();
			this.labelLight.AutoSize = true;
			this.brightness = new System.Windows.Forms.Label();
			this.brightness.AutoSize = true;
			this.height = new System.Windows.Forms.Label();
			this.height.AutoSize = true;
			this.tag = new System.Windows.Forms.Label();
			this.tag.AutoSize = true;
			this.floor = new System.Windows.Forms.Label();
			this.floor.AutoSize = true;
			this.ceiling = new System.Windows.Forms.Label();
			this.ceiling.AutoSize = true;
			this.effect = new System.Windows.Forms.Label();
			this.effect.AutoSize = true;
			this.ceilingLight = new System.Windows.Forms.Label();
			this.ceilingLight.AutoSize = true;
			this.ceilingScale = new System.Windows.Forms.Label();
			this.ceilingScale.AutoSize = true;
			this.ceilingAngle = new System.Windows.Forms.Label();
			this.ceilingAngle.AutoSize = true;
			this.ceilingOffset = new System.Windows.Forms.Label();
			this.ceilingOffset.AutoSize = true;
			this.ceilingpanel = new System.Windows.Forms.GroupBox();
			this.labelCeilTextureSize = new System.Windows.Forms.Label();
			this.labelCeilTextureSize.AutoSize = true;
			this.ceilingtex = new CodeImp.DoomBuilder.Controls.ConfigurablePictureBox();
			this.ceilingname = new System.Windows.Forms.Label();
			this.ceilingname.AutoSize = true;
			this.floorpanel = new System.Windows.Forms.GroupBox();
			this.labelFloorTextureSize = new System.Windows.Forms.Label();
			this.labelFloorTextureSize.AutoSize = true;
			this.floortex = new CodeImp.DoomBuilder.Controls.ConfigurablePictureBox();
			this.floorLight = new System.Windows.Forms.Label();
			this.floorLight.AutoSize = true;
			this.floorScale = new System.Windows.Forms.Label();
			this.floorScale.AutoSize = true;
			this.floorOffset = new System.Windows.Forms.Label();
			this.floorOffset.AutoSize = true;
			this.floorAngle = new System.Windows.Forms.Label();
			this.floorAngle.AutoSize = true;
			this.floorname = new System.Windows.Forms.Label();
			this.floorname.AutoSize = true;
			this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
			this.flagsPanel = new System.Windows.Forms.GroupBox();
			this.flags = new CodeImp.DoomBuilder.Controls.TransparentListView();
			label13 = new System.Windows.Forms.Label();
			label5 = new System.Windows.Forms.Label();
			this.sectorinfo.SuspendLayout();
			this.ceilingpanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.ceilingtex)).BeginInit();
			this.floorpanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.floortex)).BeginInit();
			this.flowLayoutPanel1.SuspendLayout();
			this.flagsPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// label13
			// 
			label13.Location = new System.Drawing.Point(183, 64);
			label13.Name = "label13";
			label13.Size = new System.Drawing.Size(70, 14);
			label13.TabIndex = 14;
			label13.Text = "Brightness:";
			label13.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label5
			// 
			label5.Location = new System.Drawing.Point(8, 64);
			label5.Name = "label5";
			label5.Size = new System.Drawing.Size(44, 14);
			label5.TabIndex = 8;
			label5.Text = "Height:";
			label5.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// labelfloor
			// 
			this.labelfloor.Location = new System.Drawing.Point(8, 49);
			this.labelfloor.Name = "labelfloor";
			this.labelfloor.Size = new System.Drawing.Size(44, 14);
			this.labelfloor.TabIndex = 3;
			this.labelfloor.Text = "Floor:";
			this.labelfloor.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// labelceiling
			// 
			this.labelceiling.Location = new System.Drawing.Point(8, 34);
			this.labelceiling.Name = "labelceiling";
			this.labelceiling.Size = new System.Drawing.Size(44, 14);
			this.labelceiling.TabIndex = 2;
			this.labelceiling.Text = "Ceiling:";
			this.labelceiling.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// taglabel
			// 
			this.taglabel.Location = new System.Drawing.Point(8, 79);
			this.taglabel.Name = "taglabel";
			this.taglabel.Size = new System.Drawing.Size(44, 14);
			this.taglabel.TabIndex = 4;
			this.taglabel.Text = "Tag:";
			this.taglabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// effectlabel
			// 
			this.effectlabel.Location = new System.Drawing.Point(8, 19);
			this.effectlabel.Name = "effectlabel";
			this.effectlabel.Size = new System.Drawing.Size(44, 14);
			this.effectlabel.TabIndex = 0;
			this.effectlabel.Text = "Effect:";
			this.effectlabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// ceilingLightLabel
			// 
			this.ceilingLightLabel.Location = new System.Drawing.Point(77, 63);
			this.ceilingLightLabel.Name = "ceilingLightLabel";
			this.ceilingLightLabel.Size = new System.Drawing.Size(45, 14);
			this.ceilingLightLabel.TabIndex = 27;
			this.ceilingLightLabel.Text = "Light:";
			this.ceilingLightLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// ceilingScaleLabel
			// 
			this.ceilingScaleLabel.Location = new System.Drawing.Point(77, 47);
			this.ceilingScaleLabel.Name = "ceilingScaleLabel";
			this.ceilingScaleLabel.Size = new System.Drawing.Size(45, 14);
			this.ceilingScaleLabel.TabIndex = 26;
			this.ceilingScaleLabel.Text = "Scale:";
			this.ceilingScaleLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// ceilingAngleLabel
			// 
			this.ceilingAngleLabel.Location = new System.Drawing.Point(77, 31);
			this.ceilingAngleLabel.Name = "ceilingAngleLabel";
			this.ceilingAngleLabel.Size = new System.Drawing.Size(45, 14);
			this.ceilingAngleLabel.TabIndex = 24;
			this.ceilingAngleLabel.Text = "Angle:";
			this.ceilingAngleLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// ceilingOffsetLabel
			// 
			this.ceilingOffsetLabel.Location = new System.Drawing.Point(77, 15);
			this.ceilingOffsetLabel.Name = "ceilingOffsetLabel";
			this.ceilingOffsetLabel.Size = new System.Drawing.Size(45, 14);
			this.ceilingOffsetLabel.TabIndex = 22;
			this.ceilingOffsetLabel.Text = "Offset:";
			this.ceilingOffsetLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// floorOffsetLabel
			// 
			this.floorOffsetLabel.Location = new System.Drawing.Point(77, 15);
			this.floorOffsetLabel.Name = "floorOffsetLabel";
			this.floorOffsetLabel.Size = new System.Drawing.Size(45, 14);
			this.floorOffsetLabel.TabIndex = 22;
			this.floorOffsetLabel.Text = "Offset:";
			this.floorOffsetLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// floorLightLabel
			// 
			this.floorLightLabel.Location = new System.Drawing.Point(77, 63);
			this.floorLightLabel.Name = "floorLightLabel";
			this.floorLightLabel.Size = new System.Drawing.Size(45, 14);
			this.floorLightLabel.TabIndex = 27;
			this.floorLightLabel.Text = "Light:";
			this.floorLightLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// floorAngleLabel
			// 
			this.floorAngleLabel.Location = new System.Drawing.Point(77, 31);
			this.floorAngleLabel.Name = "floorAngleLabel";
			this.floorAngleLabel.Size = new System.Drawing.Size(45, 14);
			this.floorAngleLabel.TabIndex = 24;
			this.floorAngleLabel.Text = "Angle:";
			this.floorAngleLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// floorScaleLabel
			// 
			this.floorScaleLabel.Location = new System.Drawing.Point(77, 47);
			this.floorScaleLabel.Name = "floorScaleLabel";
			this.floorScaleLabel.Size = new System.Drawing.Size(45, 14);
			this.floorScaleLabel.TabIndex = 26;
			this.floorScaleLabel.Text = "Scale:";
			this.floorScaleLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// sectorinfo
			// 
			this.sectorinfo.Controls.Add(this.panelFadeColor);
			this.sectorinfo.Controls.Add(this.panelLightColor);
			this.sectorinfo.Controls.Add(this.labelFade);
			this.sectorinfo.Controls.Add(this.labelLight);
			this.sectorinfo.Controls.Add(this.brightness);
			this.sectorinfo.Controls.Add(label13);
			this.sectorinfo.Controls.Add(this.height);
			this.sectorinfo.Controls.Add(label5);
			this.sectorinfo.Controls.Add(this.tag);
			this.sectorinfo.Controls.Add(this.floor);
			this.sectorinfo.Controls.Add(this.ceiling);
			this.sectorinfo.Controls.Add(this.taglabel);
			this.sectorinfo.Controls.Add(this.labelfloor);
			this.sectorinfo.Controls.Add(this.labelceiling);
			this.sectorinfo.Controls.Add(this.effect);
			this.sectorinfo.Controls.Add(this.effectlabel);
			this.sectorinfo.Location = new System.Drawing.Point(0, 0);
			this.sectorinfo.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
			this.sectorinfo.Name = "sectorinfo";
			this.sectorinfo.Size = new System.Drawing.Size(300, 100);
			this.sectorinfo.TabIndex = 2;
			this.sectorinfo.TabStop = false;
			this.sectorinfo.Text = " Sector ";
			// 
			// panelFadeColor
			// 
			this.panelFadeColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panelFadeColor.Location = new System.Drawing.Point(260, 50);
			this.panelFadeColor.Name = "panelFadeColor";
			this.panelFadeColor.Size = new System.Drawing.Size(20, 12);
			this.panelFadeColor.TabIndex = 21;
			// 
			// panelLightColor
			// 
			this.panelLightColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panelLightColor.Location = new System.Drawing.Point(260, 35);
			this.panelLightColor.Name = "panelLightColor";
			this.panelLightColor.Size = new System.Drawing.Size(20, 12);
			this.panelLightColor.TabIndex = 20;
			// 
			// labelFade
			// 
			this.labelFade.Location = new System.Drawing.Point(183, 49);
			this.labelFade.Name = "labelFade";
			this.labelFade.Size = new System.Drawing.Size(70, 14);
			this.labelFade.TabIndex = 19;
			this.labelFade.Text = "Fade:";
			this.labelFade.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// labelLight
			// 
			this.labelLight.Location = new System.Drawing.Point(183, 34);
			this.labelLight.Name = "labelLight";
			this.labelLight.Size = new System.Drawing.Size(70, 14);
			this.labelLight.TabIndex = 18;
			this.labelLight.Text = "Light:";
			this.labelLight.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// brightness
			// 
			this.brightness.Location = new System.Drawing.Point(257, 64);
			this.brightness.Name = "brightness";
			this.brightness.Size = new System.Drawing.Size(38, 14);
			this.brightness.TabIndex = 17;
			this.brightness.Text = "0";
			// 
			// height
			// 
			this.height.Location = new System.Drawing.Point(55, 64);
			this.height.Name = "height";
			this.height.Size = new System.Drawing.Size(38, 14);
			this.height.TabIndex = 11;
			this.height.Text = "0";
			// 
			// tag
			// 
			this.tag.Location = new System.Drawing.Point(55, 79);
			this.tag.Name = "tag";
			this.tag.Size = new System.Drawing.Size(239, 14);
			this.tag.TabIndex = 7;
			this.tag.Text = "0";
			// 
			// floor
			// 
			this.floor.Location = new System.Drawing.Point(55, 49);
			this.floor.Name = "floor";
			this.floor.Size = new System.Drawing.Size(42, 14);
			this.floor.TabIndex = 6;
			this.floor.Text = "360";
			// 
			// ceiling
			// 
			this.ceiling.Location = new System.Drawing.Point(55, 34);
			this.ceiling.Name = "ceiling";
			this.ceiling.Size = new System.Drawing.Size(42, 14);
			this.ceiling.TabIndex = 5;
			this.ceiling.Text = "1024";
			// 
			// effect
			// 
			this.effect.AutoSize = true;
			this.effect.Location = new System.Drawing.Point(55, 19);
			this.effect.Name = "effect";
			this.effect.Size = new System.Drawing.Size(123, 13);
			this.effect.TabIndex = 1;
			this.effect.Text = "0 - Whacky Pool of Fluid";
			// 
			// ceilingLight
			// 
			this.ceilingLight.AutoSize = true;
			this.ceilingLight.Location = new System.Drawing.Point(124, 63);
			this.ceilingLight.Name = "ceilingLight";
			this.ceilingLight.Size = new System.Drawing.Size(13, 13);
			this.ceilingLight.TabIndex = 29;
			this.ceilingLight.Text = "--";
			// 
			// ceilingScale
			// 
			this.ceilingScale.AutoSize = true;
			this.ceilingScale.Location = new System.Drawing.Point(124, 47);
			this.ceilingScale.Name = "ceilingScale";
			this.ceilingScale.Size = new System.Drawing.Size(25, 13);
			this.ceilingScale.TabIndex = 28;
			this.ceilingScale.Text = "--, --";
			// 
			// ceilingAngle
			// 
			this.ceilingAngle.AutoSize = true;
			this.ceilingAngle.Location = new System.Drawing.Point(124, 31);
			this.ceilingAngle.Name = "ceilingAngle";
			this.ceilingAngle.Size = new System.Drawing.Size(13, 13);
			this.ceilingAngle.TabIndex = 25;
			this.ceilingAngle.Text = "--";
			// 
			// ceilingOffset
			// 
			this.ceilingOffset.AutoSize = true;
			this.ceilingOffset.Location = new System.Drawing.Point(124, 15);
			this.ceilingOffset.Name = "ceilingOffset";
			this.ceilingOffset.Size = new System.Drawing.Size(25, 13);
			this.ceilingOffset.TabIndex = 23;
			this.ceilingOffset.Text = "--, --";
			// 
			// ceilingpanel
			// 
			this.ceilingpanel.Controls.Add(this.labelCeilTextureSize);
			this.ceilingpanel.Controls.Add(this.ceilingtex);
			this.ceilingpanel.Controls.Add(this.ceilingOffsetLabel);
			this.ceilingpanel.Controls.Add(this.ceilingOffset);
			this.ceilingpanel.Controls.Add(this.ceilingLight);
			this.ceilingpanel.Controls.Add(this.ceilingAngleLabel);
			this.ceilingpanel.Controls.Add(this.ceilingAngle);
			this.ceilingpanel.Controls.Add(this.ceilingScaleLabel);
			this.ceilingpanel.Controls.Add(this.ceilingScale);
			this.ceilingpanel.Controls.Add(this.ceilingLightLabel);
			this.ceilingpanel.Controls.Add(this.ceilingname);
			this.ceilingpanel.Location = new System.Drawing.Point(506, 0);
			this.ceilingpanel.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
			this.ceilingpanel.Name = "ceilingpanel";
			this.ceilingpanel.Size = new System.Drawing.Size(200, 100);
			this.ceilingpanel.TabIndex = 3;
			this.ceilingpanel.TabStop = false;
			this.ceilingpanel.Text = " Ceiling ";
			// 
			// labelCeilTextureSize
			// 
			this.labelCeilTextureSize.AutoSize = true;
			this.labelCeilTextureSize.BackColor = System.Drawing.Color.Black;
			this.labelCeilTextureSize.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelCeilTextureSize.ForeColor = System.Drawing.Color.White;
			this.labelCeilTextureSize.Location = new System.Drawing.Point(10, 17);
			this.labelCeilTextureSize.MaximumSize = new System.Drawing.Size(0, 13);
			this.labelCeilTextureSize.Name = "labelCeilTextureSize";
			this.labelCeilTextureSize.Size = new System.Drawing.Size(48, 13);
			this.labelCeilTextureSize.TabIndex = 1;
			this.labelCeilTextureSize.Text = "128x128";
			// 
			// ceilingtex
			// 
			this.ceilingtex.BackColor = System.Drawing.SystemColors.AppWorkspace;
			this.ceilingtex.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.ceilingtex.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.Default;
			this.ceilingtex.Highlighted = false;
			this.ceilingtex.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
			this.ceilingtex.Location = new System.Drawing.Point(7, 14);
			this.ceilingtex.Name = "ceilingtex";
			this.ceilingtex.PageUnit = System.Drawing.GraphicsUnit.Pixel;
			this.ceilingtex.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.None;
			this.ceilingtex.Size = new System.Drawing.Size(64, 64);
			this.ceilingtex.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.ceilingtex.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.Default;
			this.ceilingtex.TabIndex = 31;
			this.ceilingtex.TabStop = false;
			// 
			// ceilingname
			// 
			this.ceilingname.AutoSize = true;
			this.ceilingname.Location = new System.Drawing.Point(6, 81);
			this.ceilingname.Name = "ceilingname";
			this.ceilingname.Size = new System.Drawing.Size(73, 13);
			this.ceilingname.TabIndex = 1;
			this.ceilingname.Text = "BROWNHUG";
			// 
			// floorpanel
			// 
			this.floorpanel.Controls.Add(this.labelFloorTextureSize);
			this.floorpanel.Controls.Add(this.floortex);
			this.floorpanel.Controls.Add(this.floorLight);
			this.floorpanel.Controls.Add(this.floorOffsetLabel);
			this.floorpanel.Controls.Add(this.floorLightLabel);
			this.floorpanel.Controls.Add(this.floorScale);
			this.floorpanel.Controls.Add(this.floorOffset);
			this.floorpanel.Controls.Add(this.floorAngleLabel);
			this.floorpanel.Controls.Add(this.floorScaleLabel);
			this.floorpanel.Controls.Add(this.floorAngle);
			this.floorpanel.Controls.Add(this.floorname);
			this.floorpanel.Location = new System.Drawing.Point(303, 0);
			this.floorpanel.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
			this.floorpanel.Name = "floorpanel";
			this.floorpanel.Size = new System.Drawing.Size(200, 100);
			this.floorpanel.TabIndex = 4;
			this.floorpanel.TabStop = false;
			this.floorpanel.Text = " Floor ";
			// 
			// labelFloorTextureSize
			// 
			this.labelFloorTextureSize.AutoSize = true;
			this.labelFloorTextureSize.BackColor = System.Drawing.Color.Black;
			this.labelFloorTextureSize.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelFloorTextureSize.ForeColor = System.Drawing.Color.White;
			this.labelFloorTextureSize.Location = new System.Drawing.Point(10, 17);
			this.labelFloorTextureSize.MaximumSize = new System.Drawing.Size(0, 13);
			this.labelFloorTextureSize.Name = "labelFloorTextureSize";
			this.labelFloorTextureSize.Size = new System.Drawing.Size(48, 13);
			this.labelFloorTextureSize.TabIndex = 0;
			this.labelFloorTextureSize.Text = "128x128";
			// 
			// floortex
			// 
			this.floortex.BackColor = System.Drawing.SystemColors.AppWorkspace;
			this.floortex.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.floortex.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.Default;
			this.floortex.Highlighted = false;
			this.floortex.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
			this.floortex.Location = new System.Drawing.Point(7, 14);
			this.floortex.Name = "floortex";
			this.floortex.PageUnit = System.Drawing.GraphicsUnit.Pixel;
			this.floortex.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.None;
			this.floortex.Size = new System.Drawing.Size(64, 64);
			this.floortex.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.floortex.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.Default;
			this.floortex.TabIndex = 30;
			this.floortex.TabStop = false;
			// 
			// floorLight
			// 
			this.floorLight.AutoSize = true;
			this.floorLight.Location = new System.Drawing.Point(124, 63);
			this.floorLight.Name = "floorLight";
			this.floorLight.Size = new System.Drawing.Size(13, 13);
			this.floorLight.TabIndex = 29;
			this.floorLight.Text = "--";
			// 
			// floorScale
			// 
			this.floorScale.AutoSize = true;
			this.floorScale.Location = new System.Drawing.Point(124, 47);
			this.floorScale.Name = "floorScale";
			this.floorScale.Size = new System.Drawing.Size(25, 13);
			this.floorScale.TabIndex = 28;
			this.floorScale.Text = "--, --";
			// 
			// floorOffset
			// 
			this.floorOffset.AutoSize = true;
			this.floorOffset.Location = new System.Drawing.Point(124, 15);
			this.floorOffset.Name = "floorOffset";
			this.floorOffset.Size = new System.Drawing.Size(25, 13);
			this.floorOffset.TabIndex = 23;
			this.floorOffset.Text = "--, --";
			// 
			// floorAngle
			// 
			this.floorAngle.AutoSize = true;
			this.floorAngle.Location = new System.Drawing.Point(124, 31);
			this.floorAngle.Name = "floorAngle";
			this.floorAngle.Size = new System.Drawing.Size(13, 13);
			this.floorAngle.TabIndex = 25;
			this.floorAngle.Text = "--";
			// 
			// floorname
			// 
			this.floorname.AutoSize = true;
			this.floorname.Location = new System.Drawing.Point(6, 81);
			this.floorname.Name = "floorname";
			this.floorname.Size = new System.Drawing.Size(73, 13);
			this.floorname.TabIndex = 1;
			this.floorname.Text = "BROWNHUG";
			// 
			// flowLayoutPanel1
			// 
			this.flowLayoutPanel1.Controls.Add(this.sectorinfo);
			this.flowLayoutPanel1.Controls.Add(this.floorpanel);
			this.flowLayoutPanel1.Controls.Add(this.ceilingpanel);
			this.flowLayoutPanel1.Controls.Add(this.flagsPanel);
			this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
			this.flowLayoutPanel1.Name = "flowLayoutPanel1";
			this.flowLayoutPanel1.Size = new System.Drawing.Size(1400, 100);
			this.flowLayoutPanel1.TabIndex = 5;
			this.flowLayoutPanel1.WrapContents = false;
			// 
			// flagsPanel
			// 
			this.flagsPanel.Controls.Add(this.flags);
			this.flagsPanel.Location = new System.Drawing.Point(709, 0);
			this.flagsPanel.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
			this.flagsPanel.Name = "flagsPanel";
			this.flagsPanel.Size = new System.Drawing.Size(455, 100);
			this.flagsPanel.TabIndex = 7;
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
			this.flags.Size = new System.Drawing.Size(443, 88);
			this.flags.TabIndex = 0;
			this.flags.UseCompatibleStateImageBehavior = false;
			this.flags.View = System.Windows.Forms.View.List;
			// 
			// SectorInfoPanel
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.Controls.Add(this.flowLayoutPanel1);
			this.MaximumSize = new System.Drawing.Size(10000, 100);
			this.MinimumSize = new System.Drawing.Size(100, 100);
			this.Name = "SectorInfoPanel";
			this.Size = new System.Drawing.Size(1400, 100);
			this.sectorinfo.ResumeLayout(false);
			this.sectorinfo.PerformLayout();
			this.ceilingpanel.ResumeLayout(false);
			this.ceilingpanel.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.ceilingtex)).EndInit();
			this.floorpanel.ResumeLayout(false);
			this.floorpanel.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.floortex)).EndInit();
			this.flowLayoutPanel1.ResumeLayout(false);
			this.flagsPanel.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label brightness;
		private System.Windows.Forms.Label height;
		private System.Windows.Forms.Label tag;
		private System.Windows.Forms.Label floor;
		private System.Windows.Forms.Label ceiling;
		private System.Windows.Forms.Label effect;
		private System.Windows.Forms.GroupBox ceilingpanel;
		private System.Windows.Forms.Label ceilingname;
		private System.Windows.Forms.GroupBox floorpanel;
		private System.Windows.Forms.Label floorname;
		private System.Windows.Forms.GroupBox sectorinfo;
		private System.Windows.Forms.Label ceilingLight;
		private System.Windows.Forms.Label ceilingScale;
		private System.Windows.Forms.Label ceilingAngle;
		private System.Windows.Forms.Label ceilingOffset;
		private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
		private System.Windows.Forms.Label floorLight;
		private System.Windows.Forms.Label floorScale;
		private System.Windows.Forms.Label floorOffset;
		private System.Windows.Forms.Label floorAngle;
		private System.Windows.Forms.Label ceilingLightLabel;
		private System.Windows.Forms.Label ceilingScaleLabel;
		private System.Windows.Forms.Label ceilingAngleLabel;
		private System.Windows.Forms.Label ceilingOffsetLabel;
		private System.Windows.Forms.Label floorOffsetLabel;
		private System.Windows.Forms.Label floorLightLabel;
		private System.Windows.Forms.Label floorAngleLabel;
		private System.Windows.Forms.Label floorScaleLabel;
		private System.Windows.Forms.Label labelCeilTextureSize;
		private System.Windows.Forms.Label labelFloorTextureSize;
		private System.Windows.Forms.Panel panelFadeColor;
		private System.Windows.Forms.Panel panelLightColor;
		private System.Windows.Forms.Label labelFade;
		private System.Windows.Forms.Label labelLight;
		private System.Windows.Forms.GroupBox flagsPanel;
		private CodeImp.DoomBuilder.Controls.TransparentListView flags;
		private System.Windows.Forms.Label taglabel;
		private System.Windows.Forms.Label effectlabel;
		private System.Windows.Forms.Label labelfloor;
		private System.Windows.Forms.Label labelceiling;
		private ConfigurablePictureBox ceilingtex;
		private ConfigurablePictureBox floortex;
	}
}
