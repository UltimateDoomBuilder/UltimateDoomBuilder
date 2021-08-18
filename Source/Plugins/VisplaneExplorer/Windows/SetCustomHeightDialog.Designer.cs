#region ================== Copyright (c) 2021 Derek MacDonald

/*
 * Copyright (c) 2021 Derek MacDonald
 * This program is released under GNU General Public License
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 */

#endregion

namespace CodeImp.DoomBuilder.Plugins.VisplaneExplorer.Windows
{
    partial class SetCustomHeightDialog
    {
        #region ================== Variables

        private System.Windows.Forms.Label label;
        private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox input;
        private System.Windows.Forms.Button apply;
        private System.Windows.Forms.Button cancel;

        #endregion

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
            this.label = new System.Windows.Forms.Label();
            this.input = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
            this.apply = new System.Windows.Forms.Button();
            this.cancel = new System.Windows.Forms.Button();

            this.SuspendLayout();
            // 
            // label
            // 
            this.label.Location = new System.Drawing.Point(15, 30);
            this.label.Name = "customheightlabel";
            this.label.Size = new System.Drawing.Size(68, 14);
            this.label.TabIndex = 0;
            this.label.Text = "View height:";
            this.label.TextAlign = System.Drawing.ContentAlignment.TopRight;
            //
            // input
            //
            this.input.AllowDecimal = false;
            this.input.AllowNegative = false;
            this.input.AllowRelative = true;
            this.input.ButtonStep = 1;
            this.input.ButtonStepBig = 64;
            this.input.ButtonStepSmall = 8;
            this.input.ButtonStepsUseModifierKeys = false;
            this.input.ButtonStepsWrapAround = false;
            this.input.Location = new System.Drawing.Point(88, 25);
            this.input.Name = "customheight";
            this.input.Size = new System.Drawing.Size(88, 24);
            this.input.StepValues = null;
            this.input.TabIndex = 1;
            this.input.WhenEnterPressed += new System.EventHandler(this.apply_Clicked);
            //
            // apply
            //
            this.apply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.apply.Location = new System.Drawing.Point(15, 74);
            this.apply.Name = "apply";
            this.apply.Size = new System.Drawing.Size(112, 25);
            this.apply.TabIndex = 2;
            this.apply.Text = "OK";
            this.apply.UseVisualStyleBackColor = true;
            this.apply.Click += new System.EventHandler(this.apply_Clicked);
            //
            // cancel
            //
            this.cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancel.Location = new System.Drawing.Point(142, 74);
            this.cancel.Name = "cancel";
            this.cancel.Size = new System.Drawing.Size(112, 25);
            this.cancel.TabIndex = 3;
            this.cancel.Text = "Cancel";
            this.cancel.UseVisualStyleBackColor = true;
            //
            // SetCustomHeightDialog
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancel;
            this.ClientSize = new System.Drawing.Size(269, 114);
            this.Controls.Add(this.label);
            this.Controls.Add(this.input);
            this.Controls.Add(this.apply);
            this.Controls.Add(this.cancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SetCustomHeightDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Visplane Explorer - Set custom height";
            this.Shown += new System.EventHandler(this.Dialog_Shown);
            this.ResumeLayout(false);
        }

        #endregion
    }
}
