namespace CodeImp.DoomBuilder.BuilderModes.Interface
{
	partial class MakeDoorForm
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.doortexture = new CodeImp.DoomBuilder.Controls.TextureSelectorControl();
			this.label1 = new System.Windows.Forms.Label();
			this.label1.AutoSize = true;
			this.label2 = new System.Windows.Forms.Label();
			this.label2.AutoSize = true;
			this.ceilingtexture = new CodeImp.DoomBuilder.Controls.FlatSelectorControl();
			this.floortexture = new CodeImp.DoomBuilder.Controls.FlatSelectorControl();
			this.label3 = new System.Windows.Forms.Label();
			this.label3.AutoSize = true;
			this.cancel = new System.Windows.Forms.Button();
			this.apply = new System.Windows.Forms.Button();
			this.resetoffsets = new System.Windows.Forms.CheckBox();
			this.tracktexture = new CodeImp.DoomBuilder.Controls.TextureSelectorControl();
			this.label4 = new System.Windows.Forms.Label();
			this.label4.AutoSize = true;
			this.applyactionspecials = new System.Windows.Forms.CheckBox();
			this.applytag = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// doortexture
			// 
			this.doortexture.Location = new System.Drawing.Point(12, 29);
			this.doortexture.MultipleTextures = false;
			this.doortexture.Name = "doortexture";
			this.doortexture.Required = true;
			this.doortexture.Size = new System.Drawing.Size(83, 105);
			this.doortexture.TabIndex = 0;
			this.doortexture.TextureName = "";
			// 
			// label1
			// 
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.label1.Location = new System.Drawing.Point(12, 10);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(83, 21);
			this.label1.TabIndex = 1;
			this.label1.Text = "Door";
			this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// label2
			// 
			this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.label2.Location = new System.Drawing.Point(190, 10);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(83, 21);
			this.label2.TabIndex = 2;
			this.label2.Text = "Ceiling";
			this.label2.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// ceilingtexture
			// 
			this.ceilingtexture.Location = new System.Drawing.Point(190, 29);
			this.ceilingtexture.MultipleTextures = false;
			this.ceilingtexture.Name = "ceilingtexture";
			this.ceilingtexture.Size = new System.Drawing.Size(83, 105);
			this.ceilingtexture.TabIndex = 1;
			this.ceilingtexture.TextureName = "";
			// 
			// floortexture
			// 
			this.floortexture.Location = new System.Drawing.Point(279, 29);
			this.floortexture.MultipleTextures = false;
			this.floortexture.Name = "floortexture";
			this.floortexture.Size = new System.Drawing.Size(83, 105);
			this.floortexture.TabIndex = 2;
			this.floortexture.TextureName = "";
			// 
			// label3
			// 
			this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.label3.Location = new System.Drawing.Point(279, 10);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(83, 21);
			this.label3.TabIndex = 4;
			this.label3.Text = "Floor";
			this.label3.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// cancel
			// 
			this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancel.Location = new System.Drawing.Point(279, 163);
			this.cancel.Name = "cancel";
			this.cancel.Size = new System.Drawing.Size(83, 25);
			this.cancel.TabIndex = 4;
			this.cancel.Text = "Cancel";
			this.cancel.UseVisualStyleBackColor = true;
			this.cancel.Click += new System.EventHandler(this.cancel_Click);
			// 
			// apply
			// 
			this.apply.Location = new System.Drawing.Point(190, 163);
			this.apply.Name = "apply";
			this.apply.Size = new System.Drawing.Size(83, 25);
			this.apply.TabIndex = 3;
			this.apply.Text = "OK";
			this.apply.UseVisualStyleBackColor = true;
			this.apply.Click += new System.EventHandler(this.apply_Click);
			// 
			// resetoffsets
			// 
			this.resetoffsets.AutoSize = true;
			this.resetoffsets.Checked = true;
			this.resetoffsets.CheckState = System.Windows.Forms.CheckState.Checked;
			this.resetoffsets.Location = new System.Drawing.Point(12, 140);
			this.resetoffsets.Name = "resetoffsets";
			this.resetoffsets.Size = new System.Drawing.Size(123, 17);
			this.resetoffsets.TabIndex = 5;
			this.resetoffsets.Text = "Reset texture offsets";
			this.resetoffsets.UseVisualStyleBackColor = true;
			// 
			// tracktexture
			// 
			this.tracktexture.Location = new System.Drawing.Point(101, 29);
			this.tracktexture.MultipleTextures = false;
			this.tracktexture.Name = "tracktexture";
			this.tracktexture.Required = false;
			this.tracktexture.Size = new System.Drawing.Size(83, 105);
			this.tracktexture.TabIndex = 6;
			this.tracktexture.TextureName = "";
			// 
			// label4
			// 
			this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.label4.Location = new System.Drawing.Point(101, 10);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(83, 21);
			this.label4.TabIndex = 7;
			this.label4.Text = "Track";
			this.label4.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// applyactionspecials
			// 
			this.applyactionspecials.AutoSize = true;
			this.applyactionspecials.Checked = true;
			this.applyactionspecials.CheckState = System.Windows.Forms.CheckState.Checked;
			this.applyactionspecials.Location = new System.Drawing.Point(148, 140);
			this.applyactionspecials.Name = "applyactionspecials";
			this.applyactionspecials.Size = new System.Drawing.Size(125, 17);
			this.applyactionspecials.TabIndex = 8;
			this.applyactionspecials.Text = "Apply action specials";
			this.applyactionspecials.UseVisualStyleBackColor = true;
			// 
			// applytag
			// 
			this.applytag.AutoSize = true;
			this.applytag.Checked = true;
			this.applytag.CheckState = System.Windows.Forms.CheckState.Checked;
			this.applytag.Location = new System.Drawing.Point(292, 140);
			this.applytag.Name = "applytag";
			this.applytag.Size = new System.Drawing.Size(70, 17);
			this.applytag.TabIndex = 9;
			this.applytag.Text = "Apply tag";
			this.applytag.UseVisualStyleBackColor = true;
			// 
			// MakeDoorForm
			// 
			this.AcceptButton = this.apply;
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.CancelButton = this.cancel;
			this.ClientSize = new System.Drawing.Size(372, 197);
			this.Controls.Add(this.applytag);
			this.Controls.Add(this.applyactionspecials);
			this.Controls.Add(this.tracktexture);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.resetoffsets);
			this.Controls.Add(this.cancel);
			this.Controls.Add(this.apply);
			this.Controls.Add(this.floortexture);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.ceilingtexture);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.doortexture);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "MakeDoorForm";
			this.Opacity = 0;
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Make Door";
			this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.MakeDoorForm_HelpRequested);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private CodeImp.DoomBuilder.Controls.TextureSelectorControl doortexture;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private CodeImp.DoomBuilder.Controls.FlatSelectorControl ceilingtexture;
		private CodeImp.DoomBuilder.Controls.FlatSelectorControl floortexture;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button cancel;
		private System.Windows.Forms.Button apply;
		private System.Windows.Forms.CheckBox resetoffsets;
		private CodeImp.DoomBuilder.Controls.TextureSelectorControl tracktexture;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.CheckBox applyactionspecials;
		private System.Windows.Forms.CheckBox applytag;
	}
}
