
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

using System.Windows.Forms;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	// This doesn't do jack shit.
	internal class NullVisualEventReceiver : IVisualEventReceiver
	{
		public bool Selected { get { return false; } } //mxd
		
		public void OnSelectBegin() { }
		public void OnSelectEnd() { }
		public void OnEditBegin() {	}
		public void OnEditEnd()	{ }
		public void OnMouseMove(MouseEventArgs e) {	}
		public void OnChangeTargetHeight(int amount) { }
		public void OnChangeTargetBrightness(bool up) { }
		public bool OnChangeTextureOffset(int horizontal, int vertical, bool doSurfaceAngleCorrection) { return true; }
		public void OnChangeScale(int incrementX, int incrementY) { } //mxd
		public void OnResetTextureOffset() { }
		public void OnResetLocalTextureOffset() { } //mxd
		public void OnSelectTexture() { }
		public void OnCopyTexture() { }
		public void OnPasteTexture() { }
		public void OnCopyTextureOffsets() { }
		public void OnPasteTextureOffsets() { }
		public void OnCopyProperties() { }
		public void OnPasteProperties(bool useoptions) { } //mxd. Added "useoptions"
		public void OnTextureAlign(bool alignx, bool aligny) { }
		public void OnTextureFit(FitTextureOptions options) { } //mxd
		public void OnTextureFloodfill() { }
		public void OnToggleUpperUnpegged()	{ }
		public void OnToggleLowerUnpegged()	{ }
		public void OnProcess(long deltatime) { }
		public void OnInsert() { }
		public void OnDelete() { }
		public void OnPaintSelectBegin() { } // biwa
		public void OnPaintSelectEnd() { } // biwa
		public void ApplyTexture(string texture) { }
		public void ApplyUpperUnpegged(bool set) { }
		public void ApplyLowerUnpegged(bool set) { }
		public string GetTextureName() { return "";	}
		public void SelectNeighbours(bool select, bool withSameTexture, bool withSameHeight, bool stopatselected) { } //mxd
	}
}
