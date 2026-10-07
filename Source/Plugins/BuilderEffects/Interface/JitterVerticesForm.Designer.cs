namespace CodeImp.DoomBuilder.BuilderEffects
{
	partial class JitterVerticesForm
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.bCancel = new System.Windows.Forms.Button();
			this.bApply = new System.Windows.Forms.Button();
			this.bUpdateTranslation = new System.Windows.Forms.Button();
			this.positionJitterAmmount = new IntControl();
			this.SuspendLayout();
			// 
			// bCancel
			// 
			this.bCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.bCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.bCancel.Location = new System.Drawing.Point(222, 44);
			this.bCancel.Name = "bCancel";
			this.bCancel.Size = new System.Drawing.Size(75, 25);
			this.bCancel.TabIndex = 7;
			this.bCancel.Text = "Cancel";
			this.bCancel.UseVisualStyleBackColor = true;
			this.bCancel.Click += new System.EventHandler(this.bCancel_Click);
			// 
			// bApply
			// 
			this.bApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.bApply.Location = new System.Drawing.Point(121, 44);
			this.bApply.Name = "bApply";
			this.bApply.Size = new System.Drawing.Size(95, 25);
			this.bApply.TabIndex = 6;
			this.bApply.Text = "Apply";
			this.bApply.UseVisualStyleBackColor = true;
			this.bApply.Click += new System.EventHandler(this.bApply_Click);
			// 
			// bUpdateTranslation
			// 
			this.bUpdateTranslation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.bUpdateTranslation.Image = global::CodeImp.DoomBuilder.BuilderEffects.Properties.Resources.Update;
			this.bUpdateTranslation.Location = new System.Drawing.Point(274, 11);
			this.bUpdateTranslation.Name = "bUpdateTranslation";
			this.bUpdateTranslation.Size = new System.Drawing.Size(23, 23);
			this.bUpdateTranslation.TabIndex = 8;
			this.bUpdateTranslation.UseVisualStyleBackColor = true;
			this.bUpdateTranslation.Click += new System.EventHandler(this.bUpdateTranslation_Click);
			// 
			// positionJitterAmmount
			// 
			this.positionJitterAmmount.AllowNegative = false;
			this.positionJitterAmmount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.positionJitterAmmount.ExtendedLimits = true;
			this.positionJitterAmmount.Label = "Position:";
			this.positionJitterAmmount.Location = new System.Drawing.Point(2, 12);
			this.positionJitterAmmount.Maximum = 100;
			this.positionJitterAmmount.Minimum = 0;
			this.positionJitterAmmount.Name = "positionJitterAmmount";
			this.positionJitterAmmount.Size = new System.Drawing.Size(266, 22);
			this.positionJitterAmmount.TabIndex = 9;
			this.positionJitterAmmount.Value = 0;
			this.positionJitterAmmount.OnValueChanging += new System.EventHandler(this.positionJitterAmmount_OnValueChanging);
			// 
			// JitterVerticesForm
			// 
			this.AcceptButton = this.bApply;
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.CancelButton = this.bCancel;
			this.ClientSize = new System.Drawing.Size(306, 72);
			this.Controls.Add(this.positionJitterAmmount);
			this.Controls.Add(this.bUpdateTranslation);
			this.Controls.Add(this.bCancel);
			this.Controls.Add(this.bApply);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "JitterVerticesForm";
			this.Opacity = 0D;
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "Jitter Settings";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.JitterVerticesForm_FormClosing);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button bUpdateTranslation;
		private System.Windows.Forms.Button bCancel;
		private System.Windows.Forms.Button bApply;
		private IntControl positionJitterAmmount;
	}
}