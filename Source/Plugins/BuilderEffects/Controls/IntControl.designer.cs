namespace CodeImp.DoomBuilder.BuilderEffects
{
	partial class IntControl {
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

		#region Код, автоматически созданный конструктором компонентов

		/// <summary> 
		/// Обязательный метод для поддержки конструктора - не изменяйте 
		/// содержимое данного метода при помощи редактора кода.
		/// </summary>
		private void InitializeComponent() 
		{
			this.trackBar1 = new System.Windows.Forms.TrackBar();
			this.label1 = new System.Windows.Forms.Label();
			this.label1.AutoSize = true;
			#if !MONO_WINFORMS
			this.numericUpDown1 = new CodeImp.DoomBuilder.BuilderEffects.NumericUpDownEx();
			#else
			this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
			this.numericUpDown1.MouseWheel += (object sender, System.Windows.Forms.MouseEventArgs e) => {
				System.Windows.Forms.NumericUpDown control = (System.Windows.Forms.NumericUpDown) sender;
				control.Value += e.Delta;
			};
			#endif
			this.labelMaximum = new System.Windows.Forms.Label();
			this.labelMaximum.AutoSize = true;
			((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
			this.SuspendLayout();
			// 
			// trackBar1
			// 
			this.trackBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.trackBar1.LargeChange = 32;
			this.trackBar1.Location = new System.Drawing.Point(152, 1);
			this.trackBar1.Maximum = 512;
			this.trackBar1.Name = "trackBar1";
			this.trackBar1.Size = new System.Drawing.Size(86, 45);
			this.trackBar1.SmallChange = 4;
			this.trackBar1.TabIndex = 4;
			this.trackBar1.TickFrequency = 16;
			this.trackBar1.TickStyle = System.Windows.Forms.TickStyle.None;
			this.trackBar1.MouseLeave += new System.EventHandler(this.trackBar1_MouseLeave);
			this.trackBar1.ValueChanged += new System.EventHandler(this.trackBar1_ValueChanged);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(0, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(90, 24);
			this.label1.TabIndex = 7;
			this.label1.Text = "Some cool value:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// numericUpDown1
			// 
			this.numericUpDown1.Location = new System.Drawing.Point(96, 2);
			this.numericUpDown1.Name = "numericUpDown1";
			this.numericUpDown1.Size = new System.Drawing.Size(52, 20);
			this.numericUpDown1.TabIndex = 8;
			this.numericUpDown1.ValueChanged += new System.EventHandler(this.numericUpDown1_ValueChanged);
			// 
			// labelMaximum
			// 
			this.labelMaximum.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelMaximum.AutoSize = true;
			this.labelMaximum.Location = new System.Drawing.Point(237, 5);
			this.labelMaximum.Name = "labelMaximum";
			this.labelMaximum.Size = new System.Drawing.Size(25, 13);
			this.labelMaximum.TabIndex = 9;
			this.labelMaximum.Text = "512";
			// 
			// IntControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.Controls.Add(this.labelMaximum);
			this.Controls.Add(this.numericUpDown1);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.trackBar1);
			this.Name = "IntControl";
			this.Size = new System.Drawing.Size(266, 24);
			((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TrackBar trackBar1;
		private System.Windows.Forms.Label label1;
		#if !MONO_WINFORMS
		private NumericUpDownEx numericUpDown1;
		#else
		private System.Windows.Forms.NumericUpDown numericUpDown1;
		#endif
		private System.Windows.Forms.Label labelMaximum;
	}
}
