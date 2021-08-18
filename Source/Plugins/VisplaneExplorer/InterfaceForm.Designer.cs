#region ================== Namespaces

using System.Collections.Generic;
using System.Globalization;

#endregion

namespace CodeImp.DoomBuilder.Plugins.VisplaneExplorer
{
	partial class InterfaceForm
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
			this.components = new System.ComponentModel.Container();
			this.toolstrip = new System.Windows.Forms.ToolStrip();
			this.statsbutton = new System.Windows.Forms.ToolStripDropDownButton();
			this.vpstats = new System.Windows.Forms.ToolStripMenuItem();
			this.dsstats = new System.Windows.Forms.ToolStripMenuItem();
			this.ssstats = new System.Windows.Forms.ToolStripMenuItem();
			this.opstats = new System.Windows.Forms.ToolStripMenuItem();
			this.separator = new System.Windows.Forms.ToolStripSeparator();
			this.cbopendoors = new CodeImp.DoomBuilder.Controls.ToolStripCheckBox();
			this.cbheatmap = new CodeImp.DoomBuilder.Controls.ToolStripCheckBox();
			this.heightbutton = new System.Windows.Forms.ToolStripDropDownButton();
			this.heightitems = new System.Windows.Forms.ToolStripMenuItem[General.Map.Config.VisplaneViewHeights.Count];
			this.heightcustomitem = new System.Windows.Forms.ToolStripMenuItem();
			this.heightcustomadd = new System.Windows.Forms.ToolStripMenuItem();
			this.customheightdialog = new Windows.SetCustomHeightDialog();
			this.tooltip = new System.Windows.Forms.ToolTip(this.components);
			this.toolstrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// toolstrip
			// 
			this.toolstrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
				this.statsbutton,
				this.separator,
				this.cbopendoors,
				this.cbheatmap,
				this.heightbutton
			});
			this.toolstrip.Location = new System.Drawing.Point(0, 0);
			this.toolstrip.Name = "toolstrip";
			this.toolstrip.Size = new System.Drawing.Size(561, 25);
			this.toolstrip.TabIndex = 0;
			// 
			// statsbutton
			// 
			this.statsbutton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.statsbutton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
				this.vpstats,
				this.dsstats,
				this.ssstats,
				this.opstats
			});
			this.statsbutton.Image = global::CodeImp.DoomBuilder.Plugins.VisplaneExplorer.Properties.Resources.Visplanes;
			this.statsbutton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.statsbutton.Name = "statsbutton";
			this.statsbutton.Size = new System.Drawing.Size(29, 22);
			this.statsbutton.ToolTipText = "Statistics to view";
			// 
			// vpstats
			// 
			this.vpstats.Checked = true;
			this.vpstats.CheckState = System.Windows.Forms.CheckState.Checked;
			this.vpstats.Image = global::CodeImp.DoomBuilder.Plugins.VisplaneExplorer.Properties.Resources.Visplanes;
			this.vpstats.Name = "vpstats";
			this.vpstats.Size = new System.Drawing.Size(125, 22);
			this.vpstats.Tag = "0";
			this.vpstats.Text = "Visplanes";
			this.vpstats.Click += new System.EventHandler(this.stats_Click);
			// 
			// dsstats
			// 
			this.dsstats.Image = global::CodeImp.DoomBuilder.Plugins.VisplaneExplorer.Properties.Resources.Drawsegs;
			this.dsstats.Name = "dsstats";
			this.dsstats.Size = new System.Drawing.Size(125, 22);
			this.dsstats.Tag = "1";
			this.dsstats.Text = "Drawsegs";
			this.dsstats.Click += new System.EventHandler(this.stats_Click);
			// 
			// ssstats
			// 
			this.ssstats.Image = global::CodeImp.DoomBuilder.Plugins.VisplaneExplorer.Properties.Resources.Solidsegs;
			this.ssstats.Name = "ssstats";
			this.ssstats.Size = new System.Drawing.Size(125, 22);
			this.ssstats.Tag = "2";
			this.ssstats.Text = "Solidsegs";
			this.ssstats.Click += new System.EventHandler(this.stats_Click);
			// 
			// opstats
			// 
			this.opstats.Image = global::CodeImp.DoomBuilder.Plugins.VisplaneExplorer.Properties.Resources.Openings;
			this.opstats.Name = "opstats";
			this.opstats.Size = new System.Drawing.Size(125, 22);
			this.opstats.Tag = "3";
			this.opstats.Text = "Openings";
			this.opstats.Click += new System.EventHandler(this.stats_Click);
			// 
			// separator
			// 
			this.separator.Name = "separator";
			this.separator.Size = new System.Drawing.Size(6, 25);
			// 
			// cbopendoors
			// 
			this.cbopendoors.Checked = false;
			this.cbopendoors.Margin = new System.Windows.Forms.Padding(12, 1, 0, 2);
			this.cbopendoors.Name = "cbopendoors";
			this.cbopendoors.Size = new System.Drawing.Size(89, 22);
			this.cbopendoors.Text = "Open Doors";
			this.cbopendoors.Click += new System.EventHandler(this.cbopendoors_Click);
			// 
			// cbheatmap
			// 
			this.cbheatmap.Checked = false;
			this.cbheatmap.Name = "cbheatmap";
			this.cbheatmap.Size = new System.Drawing.Size(88, 22);
			this.cbheatmap.Text = "Heat Colors";
			this.cbheatmap.Click += new System.EventHandler(this.cbheatmap_Click);
			//
			// heightbutton
			//
			this.heightbutton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.heightbutton.Text = "View Height";
			this.heightbutton.Name = "heightbutton";
			this.heightbutton.Size = new System.Drawing.Size(125, 22);
			this.heightbutton.ToolTipText = "Position above the floor to calculate stats";
			//
			// heightitems
			//
			int i = 0;
			foreach (KeyValuePair<string, string> viewheight in General.Map.Config.VisplaneViewHeights)
			{
				System.Windows.Forms.ToolStripMenuItem heightitem = new System.Windows.Forms.ToolStripMenuItem();
				heightitem.Checked = false;
				heightitem.Name = "viewheight" + viewheight.Key;
				heightitem.Size = new System.Drawing.Size(125, 22);
				heightitem.Tag = viewheight.Key;
				heightitem.Text = viewheight.Key + " - " +
					viewheight.Value +
					(viewheight.Key == ViewHeightDefault.ToString() ? " (default)" : "");
				heightitem.Click += new System.EventHandler(this.viewheight_Click);
				this.heightbutton.DropDownItems.Add(heightitem);

				this.heightitems[i] = heightitem;
				++i;
			}
			//
			// heightcustomitem
			//
			this.heightcustomitem.Checked = false;
			this.heightcustomitem.Name = "heightcustomitem";
			this.heightcustomitem.Size = new System.Drawing.Size(125, 22);
			this.heightcustomitem.Tag = "0";
			this.heightcustomitem.Text = "0 - Custom";
			this.heightcustomitem.Visible = false;
			this.heightcustomitem.Click += new System.EventHandler(this.viewheight_Click);
			this.heightbutton.DropDownItems.Add(this.heightcustomitem);
			//
			// heightcustomadd
			//
			this.heightcustomadd.Image = global::CodeImp.DoomBuilder.Plugins.VisplaneExplorer.Properties.Resources.Add;
			this.heightcustomadd.Checked = false;
			this.heightcustomadd.Name = "heightcustomadd";
			this.heightcustomadd.Size = new System.Drawing.Size(125, 22);
			this.heightcustomadd.Tag = "-1";
			this.heightcustomadd.Text = "Set custom height";
			this.heightcustomadd.Click += new System.EventHandler(this.heightcustomadd_Click);
			this.heightbutton.DropDownItems.Add(this.heightcustomadd);
			// 
			// tooltip
			// 
			this.tooltip.AutomaticDelay = 1;
			this.tooltip.AutoPopDelay = 1000;
			this.tooltip.InitialDelay = 1;
			this.tooltip.ReshowDelay = 0;
			this.tooltip.ShowAlways = true;
			// 
			// InterfaceForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.ClientSize = new System.Drawing.Size(561, 273);
			this.Controls.Add(this.toolstrip);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "InterfaceForm";
			this.Opacity = 1;
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.Text = "InterfaceForm";
			this.toolstrip.ResumeLayout(false);
			this.toolstrip.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ToolStrip toolstrip;
		private System.Windows.Forms.ToolStripDropDownButton statsbutton;
		private System.Windows.Forms.ToolStripMenuItem vpstats;
		private System.Windows.Forms.ToolStripMenuItem dsstats;
		private System.Windows.Forms.ToolStripMenuItem ssstats;
		private System.Windows.Forms.ToolStripMenuItem opstats;
		private System.Windows.Forms.ToolTip tooltip;
		private CodeImp.DoomBuilder.Controls.ToolStripCheckBox cbopendoors;
		private CodeImp.DoomBuilder.Controls.ToolStripCheckBox cbheatmap;
		private System.Windows.Forms.ToolStripDropDownButton heightbutton;
		private System.Windows.Forms.ToolStripMenuItem[] heightitems;
		private System.Windows.Forms.ToolStripMenuItem heightcustomitem;
		private System.Windows.Forms.ToolStripMenuItem heightcustomadd;
		private Windows.SetCustomHeightDialog customheightdialog;
		private System.Windows.Forms.ToolStripSeparator separator;

	}
}