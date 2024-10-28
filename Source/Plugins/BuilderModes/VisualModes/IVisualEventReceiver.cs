
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
	internal interface IVisualEventReceiver
	{
		//mxd. Properties
		bool Selected { get; }
		
		// The events that must be handled
		void OnSelectBegin();
		void OnSelectEnd();
		void OnEditBegin();
		void OnEditEnd();
		void OnMouseMove(MouseEventArgs e);
		void OnChangeTargetHeight(int amount);
		void OnChangeTargetBrightness(bool up);
		bool OnChangeTextureOffset(int horizontal, int vertical, bool doSurfaceAngleCorrection);
		void OnChangeScale(int incrementX, int incrementY); //mxd
		void OnResetTextureOffset();
		void OnResetLocalTextureOffset(); //mxd. This should reset upper/middle/lower offsets (UDMF only)
		void OnSelectTexture();
		void OnCopyTexture();
		void OnPasteTexture();
		void OnCopyTextureOffsets();
		void OnPasteTextureOffsets();
		void OnCopyProperties();
		void OnPasteProperties(bool usecopysettings); //mxd. Added "usecopysettings"
		void OnTextureAlign(bool alignx, bool aligny);
		void OnTextureFit(FitTextureOptions options); //mxd
		void OnTextureFloodfill();
		void OnToggleUpperUnpegged();
		void OnToggleLowerUnpegged();
		void OnProcess(long deltatime);
		void OnInsert();
		void OnDelete();
		void OnPaintSelectBegin(); // biwa
		void OnPaintSelectEnd(); // biwa

		// Assist functions
		void ApplyTexture(string texture);
		void ApplyUpperUnpegged(bool set);
		void ApplyLowerUnpegged(bool set);
		
		// Other methods
		string GetTextureName();
		void SelectNeighbours(bool select, bool matchtexture, bool matchheight, bool stopatselected); //mxd
	}
}
