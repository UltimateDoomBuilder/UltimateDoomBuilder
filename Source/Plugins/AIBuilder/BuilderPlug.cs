using System;
using System.Drawing;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Actions;
using CodeImp.DoomBuilder.Controls;
using CodeImp.DoomBuilder.Plugins;
using CodeImp.DoomBuilder.Windows;

namespace CodeImp.DoomBuilder.AIBuilder
{
	public class BuilderPlug : Plug
	{
		private static BuilderPlug me;

		private ToolStripActionButton toolbarbutton;
		private AIBuilderPanel panel;
		private Docker docker;

		public static BuilderPlug Me { get { return me; } }

		public override void OnInitialize()
		{
			base.OnInitialize();

			me = this;
			General.Actions.BindMethods(this);
			CreateToolbarButton();
		}

		public override void Dispose()
		{
			RemoveDocker();

			if(toolbarbutton != null)
			{
				General.Interface.RemoveButton(toolbarbutton);
				toolbarbutton.Dispose();
				toolbarbutton = null;
			}

			General.Actions.UnbindMethods(this);
			base.Dispose();
		}

		public override void OnMapNewEnd()
		{
			OnMapOpenEnd();
		}

		public override void OnMapOpenEnd()
		{
			CreateDocker(false);
		}

		public override void OnMapCloseBegin()
		{
			RemoveDocker();
		}

		[BeginAction("toggleaibuilder")]
		public void ToggleAIBuilder()
		{
			if(docker == null)
			{
				CreateDocker(true);
				return;
			}

			General.Interface.SelectDocker(docker);
		}

		private void CreateToolbarButton()
		{
			toolbarbutton = new ToolStripActionButton
			{
				DisplayStyle = ToolStripItemDisplayStyle.Image,
				Image = CreateAIIcon(),
				ImageTransparentColor = Color.Magenta,
				Name = "buttonaibuilder",
				Tag = "toggleaibuilder",
				Text = "AI Builder",
				ToolTipText = "AI Builder"
			};
			toolbarbutton.Click += General.Interface.InvokeTaggedAction;

			General.Interface.AddButton(toolbarbutton, ToolbarSection.Helpers);
		}

		private void CreateDocker(bool select)
		{
			if(panel == null)
			{
				panel = new AIBuilderPanel();
				docker = new Docker("aibuilderdockerpanel", "AI Builder", panel);
				General.Interface.AddDocker(docker);
			}

			if(select)
				General.Interface.SelectDocker(docker);
		}

		private void RemoveDocker()
		{
			if(panel == null) return;

			General.Interface.RemoveDocker(docker);
			docker = null;
			panel.Dispose();
			panel = null;
		}

		private static Bitmap CreateAIIcon()
		{
			Bitmap bitmap = new Bitmap(16, 16);

			using(Graphics g = Graphics.FromImage(bitmap))
			using(Pen outline = new Pen(Color.FromArgb(24, 78, 148)))
			using(SolidBrush bg = new SolidBrush(Color.FromArgb(230, 244, 255)))
			using(SolidBrush node = new SolidBrush(Color.FromArgb(36, 130, 220)))
			using(SolidBrush spark = new SolidBrush(Color.FromArgb(255, 188, 44)))
			{
				g.Clear(Color.Magenta);
				g.FillRectangle(bg, 1, 1, 14, 14);
				g.DrawRectangle(outline, 1, 1, 14, 14);

				g.DrawLine(outline, 5, 5, 10, 3);
				g.DrawLine(outline, 5, 5, 10, 10);
				g.DrawLine(outline, 10, 3, 12, 8);
				g.DrawLine(outline, 10, 10, 12, 8);

				g.FillEllipse(node, 3, 3, 4, 4);
				g.FillEllipse(node, 8, 1, 4, 4);
				g.FillEllipse(node, 8, 8, 4, 4);
				g.FillEllipse(spark, 11, 6, 4, 4);
			}

			return bitmap;
		}
	}
}
