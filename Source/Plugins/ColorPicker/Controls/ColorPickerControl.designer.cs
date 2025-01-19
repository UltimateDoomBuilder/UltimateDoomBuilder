using CodeImp.DoomBuilder.Controls;

namespace CodeImp.DoomBuilder.ColorPicker.Controls {
	partial class ColorPickerControl {
		/// <summary> 
		/// Требуется переменная конструктора.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Освободить все используемые ресурсы.
		/// </summary>
		/// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Код, автоматически созданный конструктором компонентов

		/// <summary> 
		/// Обязательный метод для поддержки конструктора - не изменяйте 
		/// содержимое данного метода при помощи редактора кода.
		/// </summary>
		private void InitializeComponent() {
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnOK = new System.Windows.Forms.Button();
			this.Label3 = new System.Windows.Forms.Label();
			this.Label3.AutoSize = true;
			this.nudRed = new System.Windows.Forms.NumericUpDown();
			this.pnlColor = new System.Windows.Forms.Panel();
			this.Label1 = new System.Windows.Forms.Label();
			this.Label1.AutoSize = true;
			this.pnlBrightness = new System.Windows.Forms.Panel();
			this.nudBlue = new System.Windows.Forms.NumericUpDown();
			this.nudGreen = new System.Windows.Forms.NumericUpDown();
			this.Label2 = new System.Windows.Forms.Label();
			this.Label2.AutoSize = true;
			this.tbFloatVals = new CodeImp.DoomBuilder.Controls.AutoSelectTextbox();
			this.pRGB = new System.Windows.Forms.Panel();
			this.cbColorInfo = new System.Windows.Forms.ComboBox();
			((System.ComponentModel.ISupportInitialize)(this.nudRed)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudBlue)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudGreen)).BeginInit();
			this.pRGB.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btnCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnCancel.Location = new System.Drawing.Point(214, 44);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(94, 42);
			this.btnCancel.TabIndex = 54;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// btnOK
			// 
			this.btnOK.BackColor = System.Drawing.SystemColors.Control;
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btnOK.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnOK.ForeColor = System.Drawing.SystemColors.ControlText;
			this.btnOK.Location = new System.Drawing.Point(214, 3);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(94, 42);
			this.btnOK.TabIndex = 53;
			this.btnOK.Text = "OK";
			this.btnOK.UseVisualStyleBackColor = true;
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// Label3
			// 
			this.Label3.Location = new System.Drawing.Point(3, 48);
			this.Label3.Name = "Label3";
			this.Label3.Size = new System.Drawing.Size(40, 23);
			this.Label3.TabIndex = 60;
			this.Label3.Text = "Blue:";
			this.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// nudRed
			// 
			this.nudRed.Location = new System.Drawing.Point(49, 4);
			this.nudRed.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
			this.nudRed.Name = "nudRed";
			this.nudRed.Size = new System.Drawing.Size(48, 20);
			this.nudRed.TabIndex = 55;
			this.nudRed.ValueChanged += new System.EventHandler(this.NudValueChanged);
			// 
			// pnlColor
			// 
			this.pnlColor.Location = new System.Drawing.Point(3, 3);
			this.pnlColor.Name = "pnlColor";
			this.pnlColor.Size = new System.Drawing.Size(176, 176);
			this.pnlColor.TabIndex = 61;
			this.pnlColor.Visible = false;
			// 
			// Label1
			// 
			this.Label1.Location = new System.Drawing.Point(3, 2);
			this.Label1.Name = "Label1";
			this.Label1.Size = new System.Drawing.Size(40, 23);
			this.Label1.TabIndex = 58;
			this.Label1.Text = "Red:";
			this.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// pnlBrightness
			// 
			this.pnlBrightness.Location = new System.Drawing.Point(185, 3);
			this.pnlBrightness.Name = "pnlBrightness";
			this.pnlBrightness.Size = new System.Drawing.Size(16, 176);
			this.pnlBrightness.TabIndex = 62;
			this.pnlBrightness.Visible = false;
			// 
			// nudBlue
			// 
			this.nudBlue.Location = new System.Drawing.Point(49, 50);
			this.nudBlue.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
			this.nudBlue.Name = "nudBlue";
			this.nudBlue.Size = new System.Drawing.Size(48, 20);
			this.nudBlue.TabIndex = 57;
			this.nudBlue.ValueChanged += new System.EventHandler(this.NudValueChanged);
			// 
			// nudGreen
			// 
			this.nudGreen.Location = new System.Drawing.Point(49, 27);
			this.nudGreen.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
			this.nudGreen.Name = "nudGreen";
			this.nudGreen.Size = new System.Drawing.Size(48, 20);
			this.nudGreen.TabIndex = 56;
			this.nudGreen.ValueChanged += new System.EventHandler(this.NudValueChanged);
			// 
			// Label2
			// 
			this.Label2.Location = new System.Drawing.Point(3, 25);
			this.Label2.Name = "Label2";
			this.Label2.Size = new System.Drawing.Size(40, 23);
			this.Label2.TabIndex = 59;
			this.Label2.Text = "Green:";
			this.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// tbFloatVals
			// 
			this.tbFloatVals.Location = new System.Drawing.Point(214, 115);
			this.tbFloatVals.Name = "tbFloatVals";
			this.tbFloatVals.Size = new System.Drawing.Size(95, 20);
			this.tbFloatVals.TabIndex = 63;
			this.tbFloatVals.Text = "1.01 0.55 0.33";
			this.tbFloatVals.TextChanged += new System.EventHandler(this.tbFloatVals_TextChanged);
			// 
			// pRGB
			// 
			this.pRGB.Controls.Add(this.Label2);
			this.pRGB.Controls.Add(this.nudGreen);
			this.pRGB.Controls.Add(this.nudBlue);
			this.pRGB.Controls.Add(this.Label1);
			this.pRGB.Controls.Add(this.Label3);
			this.pRGB.Controls.Add(this.nudRed);
			this.pRGB.Location = new System.Drawing.Point(212, 111);
			this.pRGB.Name = "pRGB";
			this.pRGB.Size = new System.Drawing.Size(104, 76);
			this.pRGB.TabIndex = 64;
			// 
			// cbColorInfo
			// 
			this.cbColorInfo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbColorInfo.FormattingEnabled = true;
			this.cbColorInfo.Location = new System.Drawing.Point(214, 89);
			this.cbColorInfo.Name = "cbColorInfo";
			this.cbColorInfo.Size = new System.Drawing.Size(94, 21);
			this.cbColorInfo.TabIndex = 65;
			this.cbColorInfo.SelectedIndexChanged += new System.EventHandler(this.cbColorInfo_SelectedIndexChanged);
			// 
			// ColorPickerControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.Controls.Add(this.cbColorInfo);
			this.Controls.Add(this.pRGB);
			this.Controls.Add(this.tbFloatVals);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.pnlColor);
			this.Controls.Add(this.pnlBrightness);
			this.Name = "ColorPickerControl";
			this.Size = new System.Drawing.Size(311, 183);
			this.Load += new System.EventHandler(this.ColorPickerControl_Load);
			this.MouseLeave += new System.EventHandler(this.OnMouseLeave);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.OnPaint);
			this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.OnMouseMove);
			this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ColorPickerControl_MouseDown);
			this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.OnMouseUp);
			((System.ComponentModel.ISupportInitialize)(this.nudRed)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudBlue)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudGreen)).EndInit();
			this.pRGB.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		internal System.Windows.Forms.Button btnCancel;
		internal System.Windows.Forms.Button btnOK;
		internal System.Windows.Forms.Label Label3;
		internal System.Windows.Forms.NumericUpDown nudRed;
		internal System.Windows.Forms.Panel pnlColor;
		internal System.Windows.Forms.Label Label1;
		internal System.Windows.Forms.Panel pnlBrightness;
		internal System.Windows.Forms.NumericUpDown nudBlue;
		internal System.Windows.Forms.NumericUpDown nudGreen;
		internal System.Windows.Forms.Label Label2;
		private CodeImp.DoomBuilder.Controls.AutoSelectTextbox tbFloatVals;
		private System.Windows.Forms.Panel pRGB;
		private System.Windows.Forms.ComboBox cbColorInfo;
	}
}
