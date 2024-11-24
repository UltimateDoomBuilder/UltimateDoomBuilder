namespace CodeImp.DoomBuilder.BuilderModes.Interface {
	partial class BridgeModeForm {
		/// <summary>
		/// Требуется переменная конструктора.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Освободить все используемые ресурсы.
		/// </summary>
		/// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
		protected override void Dispose(bool disposing) 
		{
			if(disposing && (components != null)) 
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Код, автоматически созданный конструктором форм Windows

		/// <summary>
		/// Обязательный метод для поддержки конструктора - не изменяйте
		/// содержимое данного метода при помощи редактора кода.
		/// </summary>
		private void InitializeComponent() 
		{
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.cbFloorAlign = new System.Windows.Forms.ComboBox();
			this.cbCeilingAlign = new System.Windows.Forms.ComboBox();
			this.cbBrightness = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label1.AutoSize = true;
			this.label2 = new System.Windows.Forms.Label();
			this.label2.AutoSize = true;
			this.label3 = new System.Windows.Forms.Label();
			this.label3.AutoSize = true;
			this.nudSubdivisions = new System.Windows.Forms.NumericUpDown();
			this.label4 = new System.Windows.Forms.Label();
			this.label4.AutoSize = true;
			this.buttonFlip = new System.Windows.Forms.Button();
			this.cbMirror = new System.Windows.Forms.CheckBox();
			this.cbCopy = new System.Windows.Forms.CheckBox();
			((System.ComponentModel.ISupportInitialize)(this.nudSubdivisions)).BeginInit();
			this.SuspendLayout();
			// 
			// buttonOK
			// 
			this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonOK.Location = new System.Drawing.Point(95, 148);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(75, 23);
			this.buttonOK.TabIndex = 7;
			this.buttonOK.Text = "OK";
			this.buttonOK.UseVisualStyleBackColor = true;
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(177, 148);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(75, 23);
			this.buttonCancel.TabIndex = 8;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// cbFloorAlign
			// 
			this.cbFloorAlign.FormattingEnabled = true;
			this.cbFloorAlign.Location = new System.Drawing.Point(93, 12);
			this.cbFloorAlign.Name = "cbFloorAlign";
			this.cbFloorAlign.Size = new System.Drawing.Size(158, 21);
			this.cbFloorAlign.TabIndex = 0;
			// 
			// cbCeilingAlign
			// 
			this.cbCeilingAlign.FormattingEnabled = true;
			this.cbCeilingAlign.Location = new System.Drawing.Point(93, 39);
			this.cbCeilingAlign.Name = "cbCeilingAlign";
			this.cbCeilingAlign.Size = new System.Drawing.Size(158, 21);
			this.cbCeilingAlign.TabIndex = 1;
			// 
			// cbBrightness
			// 
			this.cbBrightness.FormattingEnabled = true;
			this.cbBrightness.Location = new System.Drawing.Point(93, 66);
			this.cbBrightness.Name = "cbBrightness";
			this.cbBrightness.Size = new System.Drawing.Size(158, 21);
			this.cbBrightness.TabIndex = 2;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(31, 15);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(56, 13);
			this.label1.TabIndex = 5;
			this.label1.Text = "Align floor:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(21, 42);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(66, 13);
			this.label2.TabIndex = 6;
			this.label2.Text = "Align ceiling:";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(28, 69);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(59, 13);
			this.label3.TabIndex = 7;
			this.label3.Text = "Brightness:";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// nudSubdivisions
			// 
			this.nudSubdivisions.Location = new System.Drawing.Point(93, 94);
			this.nudSubdivisions.Name = "nudSubdivisions";
			this.nudSubdivisions.Size = new System.Drawing.Size(46, 20);
			this.nudSubdivisions.TabIndex = 3;
			this.nudSubdivisions.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
			this.nudSubdivisions.ValueChanged += new System.EventHandler(this.nudSubdivisions_ValueChanged);
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(16, 95);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(69, 13);
			this.label4.TabIndex = 9;
			this.label4.Text = "Subdivisions:";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// buttonFlip
			// 
			this.buttonFlip.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonFlip.Location = new System.Drawing.Point(13, 148);
			this.buttonFlip.Name = "buttonFlip";
			this.buttonFlip.Size = new System.Drawing.Size(75, 23);
			this.buttonFlip.TabIndex = 6;
			this.buttonFlip.Text = "Flip Lines";
			this.buttonFlip.UseVisualStyleBackColor = true;
			this.buttonFlip.Click += new System.EventHandler(this.buttonFlip_Click);
			// 
			// cbMirror
			// 
			this.cbMirror.AutoSize = true;
			this.cbMirror.Location = new System.Drawing.Point(168, 96);
			this.cbMirror.Name = "cbMirror";
			this.cbMirror.Size = new System.Drawing.Size(81, 17);
			this.cbMirror.TabIndex = 4;
			this.cbMirror.Text = "Mirror mode";
			this.cbMirror.UseVisualStyleBackColor = true;
			this.cbMirror.CheckStateChanged += new System.EventHandler(this.cbMirror_CheckStateChanged);
			// 
			// cbCopy
			// 
			this.cbCopy.AutoSize = true;
			this.cbCopy.Location = new System.Drawing.Point(168, 119);
			this.cbCopy.Name = "cbCopy";
			this.cbCopy.Size = new System.Drawing.Size(79, 17);
			this.cbCopy.TabIndex = 5;
			this.cbCopy.Text = "Copy mode";
			this.cbCopy.UseVisualStyleBackColor = true;
			this.cbCopy.CheckedChanged += new System.EventHandler(this.cbCopy_CheckedChanged);
			// 
			// BridgeModeForm
			// 
			this.AcceptButton = this.buttonOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(263, 175);
			this.Controls.Add(this.cbCopy);
			this.Controls.Add(this.cbMirror);
			this.Controls.Add(this.buttonFlip);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.nudSubdivisions);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.cbBrightness);
			this.Controls.Add(this.cbCeilingAlign);
			this.Controls.Add(this.cbFloorAlign);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOK);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "BridgeModeForm";
			this.Opacity = 0;
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.Text = "Options";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.BezierPathForm_FormClosed);
			this.MouseLeave += new System.EventHandler(this.BezierPathForm_MouseLeave);
			((System.ComponentModel.ISupportInitialize)(this.nudSubdivisions)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.ComboBox cbFloorAlign;
		private System.Windows.Forms.ComboBox cbCeilingAlign;
		private System.Windows.Forms.ComboBox cbBrightness;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.NumericUpDown nudSubdivisions;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Button buttonFlip;
		private System.Windows.Forms.CheckBox cbMirror;
		private System.Windows.Forms.CheckBox cbCopy;
	}
}
