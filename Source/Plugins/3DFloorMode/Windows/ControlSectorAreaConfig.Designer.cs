namespace CodeImp.DoomBuilder.ThreeDFloorMode
{
	partial class ControlSectorAreaConfig
	{
		/// <summary>
		/// Erforderliche Designervariable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Verwendete Ressourcen bereinigen.
		/// </summary>
		/// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Vom Windows Form-Designer generierter Code

		/// <summary>
		/// Erforderliche Methode für die Designerunterstützung.
		/// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
		/// </summary>
		private void InitializeComponent()
		{
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label2.AutoSize = true;
			this.label1 = new System.Windows.Forms.Label();
			this.label1.AutoSize = true;
			this.lastTag = new CodeImp.DoomBuilder.Controls.NumericTextbox();
			this.firstTag = new CodeImp.DoomBuilder.Controls.NumericTextbox();
			this.useTagRange = new System.Windows.Forms.CheckBox();
			this.okButton = new System.Windows.Forms.Button();
			this.cancelButton = new System.Windows.Forms.Button();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Controls.Add(this.lastTag);
			this.groupBox1.Controls.Add(this.firstTag);
			this.groupBox1.Controls.Add(this.useTagRange);
			this.groupBox1.Location = new System.Drawing.Point(12, 12);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(225, 57);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(120, 27);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(27, 13);
			this.label2.TabIndex = 4;
			this.label2.Text = "Last";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(6, 27);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(26, 13);
			this.label1.TabIndex = 3;
			this.label1.Text = "First";
			// 
			// lastTag
			// 
			this.lastTag.AllowDecimal = false;
			this.lastTag.AllowNegative = false;
			this.lastTag.AllowRelative = false;
			this.lastTag.Enabled = false;
			this.lastTag.ImeMode = System.Windows.Forms.ImeMode.Off;
			this.lastTag.Location = new System.Drawing.Point(153, 24);
			this.lastTag.Name = "lastTag";
			this.lastTag.Size = new System.Drawing.Size(62, 20);
			this.lastTag.TabIndex = 2;
			// 
			// firstTag
			// 
			this.firstTag.AllowDecimal = false;
			this.firstTag.AllowNegative = false;
			this.firstTag.AllowRelative = false;
			this.firstTag.Enabled = false;
			this.firstTag.ImeMode = System.Windows.Forms.ImeMode.Off;
			this.firstTag.Location = new System.Drawing.Point(38, 24);
			this.firstTag.Name = "firstTag";
			this.firstTag.Size = new System.Drawing.Size(62, 20);
			this.firstTag.TabIndex = 1;
			// 
			// useTagRange
			// 
			this.useTagRange.AutoSize = true;
			this.useTagRange.Location = new System.Drawing.Point(7, 1);
			this.useTagRange.Name = "useTagRange";
			this.useTagRange.Size = new System.Drawing.Size(93, 17);
			this.useTagRange.TabIndex = 0;
			this.useTagRange.Text = "Use tag range";
			this.useTagRange.UseVisualStyleBackColor = true;
			this.useTagRange.CheckedChanged += new System.EventHandler(this.useTagRange_CheckedChanged);
			// 
			// okButton
			// 
			this.okButton.Location = new System.Drawing.Point(47, 75);
			this.okButton.Name = "okButton";
			this.okButton.Size = new System.Drawing.Size(75, 23);
			this.okButton.TabIndex = 1;
			this.okButton.Text = "OK";
			this.okButton.UseVisualStyleBackColor = true;
			this.okButton.Click += new System.EventHandler(this.okButton_Click);
			// 
			// cancelButton
			// 
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.Location = new System.Drawing.Point(128, 75);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(75, 23);
			this.cancelButton.TabIndex = 2;
			this.cancelButton.Text = "Cancel";
			this.cancelButton.UseVisualStyleBackColor = true;
			this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
			// 
			// ControlSectorAreaConfig
			// 
			this.AcceptButton = this.okButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.cancelButton;
			this.ClientSize = new System.Drawing.Size(250, 109);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.okButton);
			this.Controls.Add(this.groupBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Name = "ControlSectorAreaConfig";
			this.ShowIcon = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Control Sector Area Configuration";
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox groupBox1;
		private CodeImp.DoomBuilder.Controls.NumericTextbox firstTag;
		private System.Windows.Forms.CheckBox useTagRange;
		private CodeImp.DoomBuilder.Controls.NumericTextbox lastTag;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.Button cancelButton;
	}
}
