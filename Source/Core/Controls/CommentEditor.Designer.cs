namespace CodeImp.DoomBuilder.Controls
{
	partial class CommentEditor
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.textbox = new System.Windows.Forms.TextBox();
			this.clear = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.label1.AutoSize = true;
			this.radioButton1 = new System.Windows.Forms.RadioButton();
			this.radioButton2 = new System.Windows.Forms.RadioButton();
			this.radioButton3 = new System.Windows.Forms.RadioButton();
			this.radioButton4 = new System.Windows.Forms.RadioButton();
			this.radioButton5 = new System.Windows.Forms.RadioButton();
			this.panel1 = new System.Windows.Forms.Panel();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// textbox
			// 
			this.textbox.Location = new System.Drawing.Point(3, 29);
			this.textbox.Multiline = true;
			this.textbox.Name = "textbox";
			this.textbox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textbox.Size = new System.Drawing.Size(481, 268);
			this.textbox.TabIndex = 0;
			this.textbox.TabStop = false;
			this.textbox.TextChanged += new System.EventHandler(this.textbox_TextChanged);
			this.textbox.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.textbox_PreviewKeyDown);
			this.textbox.Leave += new System.EventHandler(this.textbox_Leave);
			this.textbox.Enter += new System.EventHandler(this.textbox_Enter);
			// 
			// clear
			// 
			this.clear.Image = global::CodeImp.DoomBuilder.Properties.Resources.Clear;
			this.clear.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.clear.Location = new System.Drawing.Point(424, 2);
			this.clear.Name = "clear";
			this.clear.Size = new System.Drawing.Size(60, 23);
			this.clear.TabIndex = 1;
			this.clear.Text = "Clear";
			this.clear.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.clear.UseVisualStyleBackColor = true;
			this.clear.Click += new System.EventHandler(this.clear_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(6, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(80, 13);
			this.label1.TabIndex = 2;
			this.label1.Text = "Comment type: ";
			// 
			// radioButton1
			// 
			this.radioButton1.Image = global::CodeImp.DoomBuilder.Properties.Resources.CommentRegular;
			this.radioButton1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.radioButton1.Location = new System.Drawing.Point(3, -1);
			this.radioButton1.Name = "radioButton1";
			this.radioButton1.Size = new System.Drawing.Size(36, 24);
			this.radioButton1.TabIndex = 3;
			this.radioButton1.TabStop = true;
			this.radioButton1.Tag = 0;
			this.radioButton1.UseVisualStyleBackColor = true;
			this.radioButton1.CheckedChanged += new System.EventHandler(this.radiobutton_CheckedChanged);
			// 
			// radioButton2
			// 
			this.radioButton2.Image = global::CodeImp.DoomBuilder.Properties.Resources.CommentInfo;
			this.radioButton2.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.radioButton2.Location = new System.Drawing.Point(45, -1);
			this.radioButton2.Name = "radioButton2";
			this.radioButton2.Size = new System.Drawing.Size(36, 24);
			this.radioButton2.TabIndex = 4;
			this.radioButton2.TabStop = true;
			this.radioButton2.Tag = 1;
			this.radioButton2.UseVisualStyleBackColor = true;
			this.radioButton2.CheckedChanged += new System.EventHandler(this.radiobutton_CheckedChanged);
			// 
			// radioButton3
			// 
			this.radioButton3.Image = global::CodeImp.DoomBuilder.Properties.Resources.CommentQuestion;
			this.radioButton3.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.radioButton3.Location = new System.Drawing.Point(87, -1);
			this.radioButton3.Name = "radioButton3";
			this.radioButton3.Size = new System.Drawing.Size(36, 24);
			this.radioButton3.TabIndex = 5;
			this.radioButton3.TabStop = true;
			this.radioButton3.Tag = 2;
			this.radioButton3.UseVisualStyleBackColor = true;
			this.radioButton3.CheckedChanged += new System.EventHandler(this.radiobutton_CheckedChanged);
			// 
			// radioButton4
			// 
			this.radioButton4.Image = global::CodeImp.DoomBuilder.Properties.Resources.CommentProblem;
			this.radioButton4.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.radioButton4.Location = new System.Drawing.Point(129, -1);
			this.radioButton4.Name = "radioButton4";
			this.radioButton4.Size = new System.Drawing.Size(36, 24);
			this.radioButton4.TabIndex = 6;
			this.radioButton4.TabStop = true;
			this.radioButton4.Tag = 3;
			this.radioButton4.UseVisualStyleBackColor = true;
			this.radioButton4.CheckedChanged += new System.EventHandler(this.radiobutton_CheckedChanged);
			// 
			// radioButton5
			// 
			this.radioButton5.Image = global::CodeImp.DoomBuilder.Properties.Resources.CommentSmile;
			this.radioButton5.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.radioButton5.Location = new System.Drawing.Point(171, -1);
			this.radioButton5.Name = "radioButton5";
			this.radioButton5.Size = new System.Drawing.Size(36, 24);
			this.radioButton5.TabIndex = 7;
			this.radioButton5.TabStop = true;
			this.radioButton5.Tag = 4;
			this.radioButton5.UseVisualStyleBackColor = true;
			this.radioButton5.CheckedChanged += new System.EventHandler(this.radiobutton_CheckedChanged);
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.radioButton1);
			this.panel1.Controls.Add(this.radioButton2);
			this.panel1.Controls.Add(this.radioButton3);
			this.panel1.Controls.Add(this.radioButton4);
			this.panel1.Controls.Add(this.radioButton5);
			this.panel1.Location = new System.Drawing.Point(88, 4);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(330, 23);
			this.panel1.TabIndex = 8;
			// 
			// CommentEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.Controls.Add(this.clear);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textbox);
			this.Name = "CommentEditor";
			this.Size = new System.Drawing.Size(487, 300);
			this.Resize += new System.EventHandler(this.CommentEditor_Resize);
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox textbox;
		private System.Windows.Forms.Button clear;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.RadioButton radioButton1;
		private System.Windows.Forms.RadioButton radioButton2;
		private System.Windows.Forms.RadioButton radioButton3;
		private System.Windows.Forms.RadioButton radioButton4;
		private System.Windows.Forms.RadioButton radioButton5;
		private System.Windows.Forms.Panel panel1;
	}
}
