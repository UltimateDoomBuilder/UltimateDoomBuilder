namespace CodeImp.DoomBuilder.ThreeDFloorMode
{
	partial class ThreeDFloorEditorWindow
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
			this.components = new System.ComponentModel.Container();
			this.threeDFloorPanel = new System.Windows.Forms.FlowLayoutPanel();
			this.okButton = new System.Windows.Forms.Button();
			this.cancelButton = new System.Windows.Forms.Button();
			this.addThreeDFloorButton = new System.Windows.Forms.Button();
			this.sharedThreeDFloorsCheckBox = new System.Windows.Forms.CheckBox();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.no3dfloorspanel = new System.Windows.Forms.Panel();
			this.add3dfloorbutton2 = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.label1.AutoSize = true;
			this.detachAllButton = new System.Windows.Forms.Button();
			this.splitAllButton = new System.Windows.Forms.Button();
			this.checkAllButton = new System.Windows.Forms.Button();
			this.uncheckAllButton = new System.Windows.Forms.Button();
			this.no3dfloorspanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// threeDFloorPanel
			// 
			this.threeDFloorPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.threeDFloorPanel.AutoScroll = true;
			this.threeDFloorPanel.BackColor = System.Drawing.SystemColors.Window;
			this.threeDFloorPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.threeDFloorPanel.Location = new System.Drawing.Point(12, 6);
			this.threeDFloorPanel.Name = "threeDFloorPanel";
			this.threeDFloorPanel.Size = new System.Drawing.Size(760, 500);
			this.threeDFloorPanel.TabIndex = 0;
			// 
			// okButton
			// 
			this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.okButton.Location = new System.Drawing.Point(616, 513);
			this.okButton.Name = "okButton";
			this.okButton.Size = new System.Drawing.Size(75, 23);
			this.okButton.TabIndex = 3;
			this.okButton.Text = "OK";
			this.okButton.UseVisualStyleBackColor = true;
			this.okButton.Click += new System.EventHandler(this.okButton_Click);
			// 
			// cancelButton
			// 
			this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.Location = new System.Drawing.Point(697, 513);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(75, 23);
			this.cancelButton.TabIndex = 4;
			this.cancelButton.Text = "Cancel";
			this.cancelButton.UseVisualStyleBackColor = true;
			this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
			// 
			// addThreeDFloorButton
			// 
			this.addThreeDFloorButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.addThreeDFloorButton.Location = new System.Drawing.Point(12, 513);
			this.addThreeDFloorButton.Name = "addThreeDFloorButton";
			this.addThreeDFloorButton.Size = new System.Drawing.Size(75, 23);
			this.addThreeDFloorButton.TabIndex = 1;
			this.addThreeDFloorButton.Text = "Add 3D floor";
			this.addThreeDFloorButton.UseVisualStyleBackColor = true;
			this.addThreeDFloorButton.Click += new System.EventHandler(this.addThreeDFloorButton_Click);
			// 
			// sharedThreeDFloorsCheckBox
			// 
			this.sharedThreeDFloorsCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.sharedThreeDFloorsCheckBox.AutoSize = true;
			this.sharedThreeDFloorsCheckBox.Location = new System.Drawing.Point(93, 517);
			this.sharedThreeDFloorsCheckBox.Name = "sharedThreeDFloorsCheckBox";
			this.sharedThreeDFloorsCheckBox.Size = new System.Drawing.Size(155, 17);
			this.sharedThreeDFloorsCheckBox.TabIndex = 2;
			this.sharedThreeDFloorsCheckBox.Text = "Show shared 3D floors only";
			this.toolTip1.SetToolTip(this.sharedThreeDFloorsCheckBox, "If checked only shows the 3D floors that\r\nare shared among all selected sectors");
			this.sharedThreeDFloorsCheckBox.UseVisualStyleBackColor = true;
			this.sharedThreeDFloorsCheckBox.CheckedChanged += new System.EventHandler(this.sharedThreeDFloorsCheckBox_CheckedChanged);
			// 
			// no3dfloorspanel
			// 
			this.no3dfloorspanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.no3dfloorspanel.Controls.Add(this.add3dfloorbutton2);
			this.no3dfloorspanel.Controls.Add(this.label1);
			this.no3dfloorspanel.Location = new System.Drawing.Point(12, 6);
			this.no3dfloorspanel.Name = "no3dfloorspanel";
			this.no3dfloorspanel.Size = new System.Drawing.Size(760, 500);
			this.no3dfloorspanel.TabIndex = 0;
			// 
			// add3dfloorbutton2
			// 
			this.add3dfloorbutton2.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.add3dfloorbutton2.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.add3dfloorbutton2.Location = new System.Drawing.Point(320, 254);
			this.add3dfloorbutton2.Name = "add3dfloorbutton2";
			this.add3dfloorbutton2.Size = new System.Drawing.Size(145, 33);
			this.add3dfloorbutton2.TabIndex = 1;
			this.add3dfloorbutton2.Text = "Add 3D floor";
			this.add3dfloorbutton2.UseVisualStyleBackColor = true;
			this.add3dfloorbutton2.Click += new System.EventHandler(this.addThreeDFloorButton_Click);
			// 
			// label1
			// 
			this.label1.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(279, 212);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(227, 25);
			this.label1.TabIndex = 0;
			this.label1.Text = "There are no 3D floors";
			// 
			// detachAllButton
			// 
			this.detachAllButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.detachAllButton.Location = new System.Drawing.Point(335, 513);
			this.detachAllButton.Name = "detachAllButton";
			this.detachAllButton.Size = new System.Drawing.Size(75, 23);
			this.detachAllButton.TabIndex = 5;
			this.detachAllButton.Text = "Detach all";
			this.detachAllButton.UseVisualStyleBackColor = true;
			this.detachAllButton.Click += new System.EventHandler(this.detachAllButton_Click);
			// 
			// splitAllButton
			// 
			this.splitAllButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.splitAllButton.Location = new System.Drawing.Point(254, 513);
			this.splitAllButton.Name = "splitAllButton";
			this.splitAllButton.Size = new System.Drawing.Size(75, 23);
			this.splitAllButton.TabIndex = 6;
			this.splitAllButton.Text = "Split all";
			this.splitAllButton.UseVisualStyleBackColor = true;
			this.splitAllButton.Click += new System.EventHandler(this.splitAllButton_Click);
			// 
			// checkAllButton
			// 
			this.checkAllButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.checkAllButton.Location = new System.Drawing.Point(416, 513);
			this.checkAllButton.Name = "checkAllButton";
			this.checkAllButton.Size = new System.Drawing.Size(75, 23);
			this.checkAllButton.TabIndex = 7;
			this.checkAllButton.Text = "Check all";
			this.checkAllButton.UseVisualStyleBackColor = true;
			this.checkAllButton.Click += new System.EventHandler(this.checkAllButton_Click);
			// 
			// uncheckAllButton
			// 
			this.uncheckAllButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.uncheckAllButton.Location = new System.Drawing.Point(497, 513);
			this.uncheckAllButton.Name = "uncheckAllButton";
			this.uncheckAllButton.Size = new System.Drawing.Size(75, 23);
			this.uncheckAllButton.TabIndex = 8;
			this.uncheckAllButton.Text = "Uncheck all";
			this.uncheckAllButton.UseVisualStyleBackColor = true;
			this.uncheckAllButton.Click += new System.EventHandler(this.uncheckAllButton_Click);
			// 
			// ThreeDFloorEditorWindow
			// 
			this.AcceptButton = this.okButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.cancelButton;
			this.ClientSize = new System.Drawing.Size(784, 548);
			this.Controls.Add(this.uncheckAllButton);
			this.Controls.Add(this.checkAllButton);
			this.Controls.Add(this.splitAllButton);
			this.Controls.Add(this.detachAllButton);
			this.Controls.Add(this.no3dfloorspanel);
			this.Controls.Add(this.sharedThreeDFloorsCheckBox);
			this.Controls.Add(this.addThreeDFloorButton);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.okButton);
			this.Controls.Add(this.threeDFloorPanel);
			this.MaximizeBox = false;
			this.MaximumSize = new System.Drawing.Size(800, 2000);
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(800, 200);
			this.Name = "ThreeDFloorEditorWindow";
			this.ShowIcon = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "3D floors";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ThreeDFloorEditorWindow_FormClosed);
			this.Load += new System.EventHandler(this.ThreeDFloorEditorWindow_Load);
			this.no3dfloorspanel.ResumeLayout(false);
			this.no3dfloorspanel.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.FlowLayoutPanel threeDFloorPanel;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.Button addThreeDFloorButton;
		private System.Windows.Forms.CheckBox sharedThreeDFloorsCheckBox;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.Panel no3dfloorspanel;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button add3dfloorbutton2;
		private System.Windows.Forms.Button detachAllButton;
		private System.Windows.Forms.Button splitAllButton;
		private System.Windows.Forms.Button checkAllButton;
		private System.Windows.Forms.Button uncheckAllButton;
	}
}
