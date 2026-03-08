
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

using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Editing;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	[EditMode(DisplayName = "Ceiling Align Mode",
			  SwitchAction = "ceilingalignmode",
			  ButtonImage = "CeilingAlign.png",
			  ButtonOrder = int.MinValue + 311,
			  ButtonGroup = "000_editing",
			  UseByDefault = true, //mxd
			  SupportedMapFormats = new[] { "UniversalMapSetIO" }, //mxd
			  Volatile = true)]

	public class CeilingAlignMode : FlatAlignMode
	{
		#region ================== Properties

		protected override string XScaleName { get { return "xscaleceiling"; } }
		protected override string YScaleName { get { return "yscaleceiling"; } }
		protected override string XOffsetName { get { return "xpanningceiling"; } }
		protected override string YOffsetName { get { return "ypanningceiling"; } }
		protected override string RotationName { get { return "rotationceiling"; } }
		protected override string UndoDescription { get { return "Ceiling Alignment"; } }
		
		#endregion

		#region ================== Methods

		// Get the texture data to align
		protected override ImageData GetTexture(Sector editsector)
		{
			return General.Map.Data.GetFlatImage(editsector.LongCeilTexture);
		}

		#endregion

		#region ================== Events

		// Mode engages
		public override void OnEngage()
		{
			base.OnEngage();
			General.Actions.InvokeAction("builder_viewmodeceilings");
		}

		#endregion
	}
}
