
#region ================== Copyright (c) 2007 Pascal vd Heiden

/*
 * Copyright (c) 2007 Pascal vd Heiden, www.codeimp.com
 * This program is released under GNU General Public License
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 */

#endregion

#region ================== Namespaces

using System.Drawing;
using System.Windows.Forms;

#endregion

namespace CodeImp.DoomBuilder.Controls
{
	public class RenderTargetControl : Panel
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		private ToolTip tooltip; //mxd

		#endregion

		#region ================== Properties
		
		public event KeyEventHandler OnKeyReleased; //mxd. Sometimes it's handeled here, not by MainForm
		public Point LocationAbs { get { return this.PointToScreen(new Point(-(General.MainWindow.Width - General.MainWindow.ClientSize.Width) / 2, 0)); } } //mxd

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		internal RenderTargetControl()
		{
			// Initialize
			this.SetStyle(ControlStyles.FixedWidth, true);
			this.SetStyle(ControlStyles.FixedHeight, true);

			//mxd. Create tooltip
			tooltip = new ToolTip { UseAnimation = false, UseFading = false, InitialDelay = 0, AutoPopDelay = 9000 };
		}

		//mxd
		protected override void Dispose(bool disposing)
		{
			if(disposing)
			{
				tooltip.Dispose();
				tooltip = null;
			}
			base.Dispose(disposing);
		}

		#endregion

		#region ================== Overrides
		
		#if MONO_WINFORMS
		// This workarounds menubar getting the focus all the time
		protected override bool IsInputChar(char charCode)
		{
			return true;
		}
		#endif
		
		//mxd
		protected override void OnKeyUp(KeyEventArgs e) 
		{
			if(OnKeyReleased != null) OnKeyReleased(this, e);
		}
		
		#endregion

		#region ================== Methods

		// This sets up the control to display the splash logo
		public void SetSplashLogoDisplay()
		{
			// Change display to show splash logo
			this.SetStyle(ControlStyles.SupportsTransparentBackColor, false);
			this.SetStyle(ControlStyles.ContainerControl, true);
			this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			this.SetStyle(ControlStyles.UserPaint, true);
			this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			this.SetStyle(ControlStyles.Opaque, false);
			this.UpdateStyles();
            this.BackColor = Color.FromArgb(32, 32, 40);
			this.BackgroundImage = Properties.Resources.Splash3_trans;
			this.BackgroundImageLayout = ImageLayout.Center;
		}
		
		// This sets up the control for manual rendering
		public void SetManualRendering()
		{
			// Change display for rendering
			this.SetStyle(ControlStyles.SupportsTransparentBackColor, false);
			this.SetStyle(ControlStyles.ContainerControl, true);
			this.SetStyle(ControlStyles.OptimizedDoubleBuffer, false);
			this.SetStyle(ControlStyles.UserPaint, true);
			this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			this.SetStyle(ControlStyles.Opaque, true);
			this.UpdateStyles();
			this.BackColor = Color.Black;
			this.BackgroundImage = null;
			this.BackgroundImageLayout = ImageLayout.None;
		}

		//mxd. This shows tooltip at given position
		public void ShowToolTip(string title, string text, int x, int y)
		{
			tooltip.ToolTipTitle = title;
			tooltip.Show(text, this, x, y);
		}

		//mxd. This hides it
		public void HideToolTip()
		{
			tooltip.Hide(this);
		}

		#endregion
	}
}
