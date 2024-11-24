
namespace CodeImp.DoomBuilder.Controls
{
	partial class CommandPaletteControl
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.Windows.Forms.ListViewGroup listViewGroup1 = new System.Windows.Forms.ListViewGroup("Recent", System.Windows.Forms.HorizontalAlignment.Left);
			System.Windows.Forms.ListViewGroup listViewGroup2 = new System.Windows.Forms.ListViewGroup("Usable actions", System.Windows.Forms.HorizontalAlignment.Left);
			System.Windows.Forms.ListViewGroup listViewGroup3 = new System.Windows.Forms.ListViewGroup("Not usable in this context", System.Windows.Forms.HorizontalAlignment.Left);
			this.commandsearch = new System.Windows.Forms.TextBox();
			this.noresults = new System.Windows.Forms.Label();
			this.noresults.AutoSize = true;
			this.commandlist = new CodeImp.DoomBuilder.Controls.OptimizedListView();
			this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.SuspendLayout();
			// 
			// commandsearch
			// 
			this.commandsearch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.commandsearch.Location = new System.Drawing.Point(3, 2);
			this.commandsearch.Name = "commandsearch";
			this.commandsearch.Size = new System.Drawing.Size(864, 20);
			this.commandsearch.TabIndex = 2;
			this.commandsearch.TextChanged += new System.EventHandler(this.commandsearch_TextChanged);
			this.commandsearch.KeyDown += new System.Windows.Forms.KeyEventHandler(this.commandsearch_KeyDown);
			// 
			// noresults
			// 
			this.noresults.AutoSize = true;
			this.noresults.Location = new System.Drawing.Point(6, 28);
			this.noresults.Name = "noresults";
			this.noresults.Size = new System.Drawing.Size(84, 13);
			this.noresults.TabIndex = 4;
			this.noresults.Text = "No results found";
			this.noresults.Visible = false;
			// 
			// commandlist
			// 
			this.commandlist.Activation = System.Windows.Forms.ItemActivation.OneClick;
			this.commandlist.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.commandlist.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader3,
            this.columnHeader2});
			this.commandlist.FullRowSelect = true;
			listViewGroup1.Header = "Recent";
			listViewGroup1.Name = "recent";
			listViewGroup2.Header = "Usable actions";
			listViewGroup2.Name = "usableactions";
			listViewGroup3.Header = "Not usable in this context";
			listViewGroup3.Name = "notusableactions";
			this.commandlist.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            listViewGroup1,
            listViewGroup2,
            listViewGroup3});
			this.commandlist.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.commandlist.Location = new System.Drawing.Point(3, 25);
			this.commandlist.MultiSelect = false;
			this.commandlist.Name = "commandlist";
			this.commandlist.Size = new System.Drawing.Size(864, 173);
			this.commandlist.TabIndex = 3;
			this.commandlist.TabStop = false;
			this.commandlist.UseCompatibleStateImageBehavior = false;
			this.commandlist.View = System.Windows.Forms.View.Details;
			this.commandlist.ItemActivate += new System.EventHandler(this.commandlist_ItemActivate);
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "Action";
			this.columnHeader1.Width = 275;
			// 
			// columnHeader3
			// 
			this.columnHeader3.Text = "Section";
			this.columnHeader3.Width = 196;
			// 
			// columnHeader2
			// 
			this.columnHeader2.Text = "Key";
			this.columnHeader2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.columnHeader2.Width = 117;
			// 
			// CommandPaletteControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.noresults);
			this.Controls.Add(this.commandlist);
			this.Controls.Add(this.commandsearch);
			this.DoubleBuffered = true;
			this.Name = "CommandPaletteControl";
			this.Size = new System.Drawing.Size(870, 201);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OptimizedListView commandlist;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.TextBox commandsearch;
		private System.Windows.Forms.ColumnHeader columnHeader3;
		private System.Windows.Forms.Label noresults;
	}
}
