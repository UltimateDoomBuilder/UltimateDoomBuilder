
namespace CodeImp.DoomBuilder.BuilderModes.Interface
{
	partial class ChangeMapElementIndexForm
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.label1 = new System.Windows.Forms.Label();
			this.label1.AutoSize = true;
			this.label2 = new System.Windows.Forms.Label();
			this.label2.AutoSize = true;
			this.label3 = new System.Windows.Forms.Label();
			this.label3.AutoSize = true;
			this.bntNewIndex = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.lbCurrentIndex = new System.Windows.Forms.Label();
			this.lbCurrentIndex.AutoSize = true;
			this.lbMaximumIndex = new System.Windows.Forms.Label();
			this.lbMaximumIndex.AutoSize = true;
			this.btnOk = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.pbWarning = new System.Windows.Forms.PictureBox();
			this.toolTip = new System.Windows.Forms.ToolTip(this.components);
			((System.ComponentModel.ISupportInitialize)(this.pbWarning)).BeginInit();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(22, 22);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(72, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Current index:";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(22, 55);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(82, 13);
			this.label2.TabIndex = 1;
			this.label2.Text = "Maximum index:";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(22, 88);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(60, 13);
			this.label3.TabIndex = 2;
			this.label3.Text = "New index:";
			// 
			// bntNewIndex
			// 
			this.bntNewIndex.AllowDecimal = false;
			this.bntNewIndex.AllowExpressions = false;
			this.bntNewIndex.AllowNegative = false;
			this.bntNewIndex.AllowRelative = false;
			this.bntNewIndex.ButtonStep = 1;
			this.bntNewIndex.ButtonStepBig = 10F;
			this.bntNewIndex.ButtonStepFloat = 1F;
			this.bntNewIndex.ButtonStepSmall = 0.1F;
			this.bntNewIndex.ButtonStepsUseModifierKeys = false;
			this.bntNewIndex.ButtonStepsWrapAround = false;
			this.bntNewIndex.Location = new System.Drawing.Point(111, 83);
			this.bntNewIndex.Name = "bntNewIndex";
			this.bntNewIndex.Size = new System.Drawing.Size(100, 24);
			this.bntNewIndex.StepValues = null;
			this.bntNewIndex.TabIndex = 3;
			this.bntNewIndex.WhenTextChanged += new System.EventHandler(this.bntNewIndex_WhenTextChanged);
			// 
			// lbCurrentIndex
			// 
			this.lbCurrentIndex.AutoSize = true;
			this.lbCurrentIndex.Location = new System.Drawing.Point(110, 22);
			this.lbCurrentIndex.Name = "lbCurrentIndex";
			this.lbCurrentIndex.Size = new System.Drawing.Size(25, 13);
			this.lbCurrentIndex.TabIndex = 4;
			this.lbCurrentIndex.Text = "123";
			// 
			// lbMaximumIndex
			// 
			this.lbMaximumIndex.AutoSize = true;
			this.lbMaximumIndex.Location = new System.Drawing.Point(110, 55);
			this.lbMaximumIndex.Name = "lbMaximumIndex";
			this.lbMaximumIndex.Size = new System.Drawing.Size(37, 13);
			this.lbMaximumIndex.TabIndex = 5;
			this.lbMaximumIndex.Text = "65535";
			// 
			// btnOk
			// 
			this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOk.Location = new System.Drawing.Point(55, 133);
			this.btnOk.Name = "btnOk";
			this.btnOk.Size = new System.Drawing.Size(75, 23);
			this.btnOk.TabIndex = 6;
			this.btnOk.Text = "OK";
			this.btnOk.UseVisualStyleBackColor = true;
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(136, 133);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 7;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			// 
			// pbWarning
			// 
			this.pbWarning.Image = global::CodeImp.DoomBuilder.BuilderModes.Properties.Resources.Warning;
			this.pbWarning.Location = new System.Drawing.Point(217, 87);
			this.pbWarning.Name = "pbWarning";
			this.pbWarning.Size = new System.Drawing.Size(16, 16);
			this.pbWarning.TabIndex = 8;
			this.pbWarning.TabStop = false;
			this.toolTip.SetToolTip(this.pbWarning, "The new index is too high");
			this.pbWarning.Visible = false;
			// 
			// ChangeMapElementIndexForm
			// 
			this.AcceptButton = this.btnOk;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(264, 168);
			this.Controls.Add(this.pbWarning);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOk);
			this.Controls.Add(this.lbMaximumIndex);
			this.Controls.Add(this.lbCurrentIndex);
			this.Controls.Add(this.bntNewIndex);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ChangeMapElementIndexForm";
			this.Text = "ChangeMapElementIndexForm";
			((System.ComponentModel.ISupportInitialize)(this.pbWarning)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private Controls.ButtonsNumericTextbox bntNewIndex;
		private System.Windows.Forms.Label lbCurrentIndex;
		private System.Windows.Forms.Label lbMaximumIndex;
		private System.Windows.Forms.Button btnOk;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.PictureBox pbWarning;
		private System.Windows.Forms.ToolTip toolTip;
	}
}
