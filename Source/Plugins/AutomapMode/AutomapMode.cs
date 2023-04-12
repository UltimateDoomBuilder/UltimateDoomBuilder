
#region ================== Copyright (c) 2016 Boris Iwanski

/*
 * Copyright (c) 2016 Boris Iwanski https://github.com/biwa/automapmode
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

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Editing;

#endregion

namespace CodeImp.DoomBuilder.AutomapMode
{
	[EditMode(DisplayName = "Automap Mode",
			  SwitchAction = "automapmode",	// Action name used to switch to this mode
			  ButtonImage = "automap.png",	// Image resource name for the button
			  ButtonOrder = int.MinValue + 503,	// Position of the button (lower is more to the bottom)
			  ButtonGroup = "000_editing",
			  UseByDefault = true)]

	public class AutomapMode : ClassicMode
	{
		#region ================== Enums

		internal enum ColorPreset
		{
			DOOM,
			HEXEN,
			STRIFE,
		}

		#endregion

		#region ================== Constants

		private const float LINE_LENGTH_SCALER = 0.001f; //mxd

		#endregion

		#region ================== Variables

		private CustomPresentation automappresentation;
		private List<Linedef> validlinedefs;
		private HashSet<Sector> secretsectors; //mxd

		// Highlighted item
		private Linedef highlighted;

		//mxd. UI
		private MenusForm menusform;

		//mxd. Colors
		private PixelColor ColorSingleSided;
		private PixelColor ColorSecret;
		private PixelColor ColorFloorDiff;
		private PixelColor ColorCeilDiff;
		private PixelColor ColorMatchingHeight;
		private PixelColor ColorHiddenFlag;
		private PixelColor ColorInvisible;
		private PixelColor ColorBackground;
		
		#endregion

		#region ================== Properties

		public override object HighlightedObject { get { return highlighted; } }
		
		#endregion

		#region ================== Constructor / Disposer

		//mxd
		public AutomapMode()
		{
			// Create and setup menu
			menusform = new MenusForm();
			menusform.ShowHiddenLines = General.Settings.ReadPluginSetting("automapmode.showhiddenlines", false);
			menusform.ShowSecretSectors = General.Settings.ReadPluginSetting("automapmode.showsecretsectors", false);
			menusform.ShowLocks = General.Settings.ReadPluginSetting("automapmode.showlocks", true);
            menusform.ShowTextures = General.Settings.ReadPluginSetting("automapmode.showtextures", true);
            menusform.ColorPreset = (ColorPreset)General.Settings.ReadPluginSetting("automapmode.colorpreset", (int)ColorPreset.DOOM);

			// Handle events
			menusform.OnShowHiddenLinesChanged += delegate
			{
				UpdateValidLinedefs();
				General.Interface.RedrawDisplay();
			};

			menusform.OnShowSecretSectorsChanged += delegate { General.Interface.RedrawDisplay(); };
			menusform.OnShowLocksChanged += delegate { General.Interface.RedrawDisplay(); };
            menusform.OnShowTexturesChanged += delegate { General.Interface.RedrawDisplay(); };

            menusform.OnColorPresetChanged += delegate
			{
				ApplyColorPreset(menusform.ColorPreset);
				General.Interface.RedrawDisplay();
			};

			// Apply color preset
			ApplyColorPreset(menusform.ColorPreset);
		}

		#endregion

		#region ================== Methods

		// This highlights a new item
		private void Highlight(Linedef l)
		{
			// Update display
			if(renderer.StartPlotter(false))
			{
				// Undraw previous highlight
				if((highlighted != null) && !highlighted.IsDisposed)
				{
					PixelColor c = LinedefIsValid(highlighted) ? DetermineLinedefColor(highlighted) : PixelColor.Transparent;
					renderer.PlotLine(highlighted.Start.Position, highlighted.End.Position, c, LINE_LENGTH_SCALER);
				}

				// Set new highlight
				highlighted = l;

				// Render highlighted item
				if((highlighted != null) && !highlighted.IsDisposed && LinedefIsValid(highlighted))
				{
					renderer.PlotLine(highlighted.Start.Position, highlighted.End.Position, General.Colors.InfoLine, LINE_LENGTH_SCALER);
				}

				// Done
				renderer.Finish();
				renderer.Present();
			}

			// Show highlight info
			if((highlighted != null) && !highlighted.IsDisposed)
				General.Interface.ShowLinedefInfo(highlighted);
			else
				General.Interface.HideInfo();
		}

		//mxd
		internal void UpdateValidLinedefs()
		{
			validlinedefs = new List<Linedef>();
			foreach(Linedef ld in General.Map.Map.Linedefs)
				if(LinedefIsValid(ld)) validlinedefs.Add(ld);
		}

		//mxd
		internal void UpdateSecretSectors()
		{
			secretsectors = new HashSet<Sector>();
			foreach(Sector s in General.Map.Map.Sectors)
				if(SectorIsSecret(s)) secretsectors.Add(s);
		}

		private PixelColor DetermineLinedefColor(Linedef ld)
		{
			//mxd
			if(menusform.ShowLocks)
			{
				PixelColor lockcolor = new PixelColor();
				if(GetLockColor(ld, ref lockcolor)) return lockcolor;
			}
			
			//mxd
			if(menusform.ShowSecretSectors &&
			   (ld.Front != null && secretsectors.Contains(ld.Front.Sector) || ld.Back != null && secretsectors.Contains(ld.Back.Sector)))
				return ColorSecret;

			if(ld.IsFlagSet(BuilderPlug.Me.HiddenFlag)) return ColorHiddenFlag;
			if(ld.Back == null || ld.Front == null || ld.IsFlagSet(BuilderPlug.Me.SecretFlag)) return ColorSingleSided;
			if(ld.Front.Sector.FloorHeight != ld.Back.Sector.FloorHeight) return ColorFloorDiff;
			if(ld.Front.Sector.CeilHeight != ld.Back.Sector.CeilHeight) return ColorCeilDiff;

			if(ld.Front.Sector.CeilHeight == ld.Back.Sector.CeilHeight && ld.Front.Sector.FloorHeight == ld.Back.Sector.FloorHeight)
				return ColorMatchingHeight;

			if(menusform.ShowHiddenLines ^ General.Interface.CtrlState) return ColorInvisible; 

			return new PixelColor(255, 255, 255, 255);
		}

		private bool LinedefIsValid(Linedef ld)
		{
			if(menusform.ShowHiddenLines ^ General.Interface.CtrlState) return true;
			if(ld.IsFlagSet(BuilderPlug.Me.HiddenFlag)) return false;
			if(ld.Back == null || ld.Front == null || ld.IsFlagSet(BuilderPlug.Me.SecretFlag)) return true;
			if(ld.Back != null && ld.Front != null && (ld.Front.Sector.FloorHeight != ld.Back.Sector.FloorHeight || ld.Front.Sector.CeilHeight != ld.Back.Sector.CeilHeight)) return true;

			return false;
		}

		private bool ShowTextures()
		{
			return menusform.ShowTextures ^ General.Interface.ShiftState;
		}

		//mxd
		private static bool SectorIsSecret(Sector s)
		{
			SectorEffectData data = General.Map.Config.GetSectorEffectData(s.Effect);

			if(General.Map.DOOM)
			{
				// Sector is secret when it's Special is 9 or it has generalized flag 128
				if(data.Effect == 9 || data.GeneralizedBits.Contains(128)) return true;
			}
			else
			{
				//Hexen/UDMF: sector is secret when it has generalized flag 1024
				if(data.GeneralizedBits.Contains(1024)) return true;
			}

			return false;
		}

		//mxd
		private static bool GetLockColor(Linedef l, ref PixelColor lockcolor)
		{
			int locknum = 0;

			// Check locknumber property
			if(General.Map.UDMF)
			{
				locknum = UniFields.GetInteger(l.Fields, "locknumber");
			}

			// Check action
			if(locknum == 0 && l.Action != 0 && General.Map.Data.LockableActions.ContainsKey(l.Action))
			{
				locknum = l.Args[General.Map.Data.LockableActions[l.Action]];
			}

			if(locknum != 0 && General.Map.Data.LockColors.ContainsKey(locknum))
			{
				lockcolor = General.Map.Data.LockColors[locknum];
				return true;
			}

			// No dice
			return false;
		}

		//mxd
		private void ApplyColorPreset(ColorPreset preset)
		{
			switch(preset)
			{
				case ColorPreset.DOOM:
					ColorSingleSided = new PixelColor(255, 252, 0, 0);
					ColorSecret = new PixelColor(255, 255, 0, 255);
					ColorFloorDiff = new PixelColor(255, 188, 120, 72);
					ColorCeilDiff = new PixelColor(255, 252, 252, 0);
					ColorHiddenFlag = new PixelColor(255, 192, 192, 192);
					ColorInvisible = new PixelColor(255, 192, 192, 192);
					ColorMatchingHeight = new PixelColor(255, 108, 108, 108);
					ColorBackground = new PixelColor(255, 0, 0, 0);
					break;

				case ColorPreset.HEXEN:
					ColorSingleSided = new PixelColor(255, 89, 64, 27);
					ColorSecret = new PixelColor(255, 255, 0, 255);
					ColorFloorDiff = new PixelColor(255, 208, 176, 133);
					ColorCeilDiff = new PixelColor(255, 103, 59, 31);
					ColorHiddenFlag = new PixelColor(255, 192, 192, 192);
					ColorInvisible = new PixelColor(255, 108, 108, 108);
					ColorMatchingHeight = new PixelColor(255, 108, 108, 108);
					ColorBackground = new PixelColor(255, 163, 129, 84);
					break;

				case ColorPreset.STRIFE:
					ColorSingleSided = new PixelColor(255, 199, 195, 195);
					ColorSecret = new PixelColor(255, 255, 0, 255);
					ColorFloorDiff = new PixelColor(255, 55, 59, 91);
					ColorCeilDiff = new PixelColor(255, 108, 108, 108);
					ColorHiddenFlag = new PixelColor(255, 0, 87, 130);
					ColorInvisible = new PixelColor(255, 192, 192, 192);
					ColorMatchingHeight = new PixelColor(255, 112, 112, 160);
					ColorBackground = new PixelColor(255, 0, 0, 0);
					break;
			}
		}

		#endregion
		
		#region ================== Events

		public override void OnHelp()
		{
			General.ShowHelp("/gzdb/features/classic_modes/mode_automap.html");
		}

		// Cancel mode
		public override void OnCancel()
		{
			base.OnCancel();

			// Return to this mode
			General.Editing.ChangeMode(new AutomapMode());
		}

		// Mode engages
		public override void OnEngage()
		{
			base.OnEngage();
			renderer.DrawMapCenter = false; //mxd

			// Automap presentation; now draws surfaces for textured mode support,
			// but the surfaces are covered up with a background layer.
			automappresentation = new CustomPresentation();
			automappresentation.AddLayer(new PresentLayer(RendererLayer.Surface, BlendingMode.Mask));
			automappresentation.AddLayer(new PresentLayer(RendererLayer.Overlay, BlendingMode.Mask));
			automappresentation.AddLayer(new PresentLayer(RendererLayer.Grid, BlendingMode.Mask));
			automappresentation.AddLayer(new PresentLayer(RendererLayer.Geometry, BlendingMode.Alpha, 1f, true));
			renderer.SetPresentation(automappresentation);

			UpdateValidLinedefs();
			UpdateSecretSectors(); //mxd

			//mxd. Show UI
			menusform.Register();
		}
		
		// Mode disengages
		public override void OnDisengage()
		{
			base.OnDisengage();

			//mxd. Store settings
			General.Settings.WritePluginSetting("automapmode.showhiddenlines", menusform.ShowHiddenLines);
			General.Settings.WritePluginSetting("automapmode.showsecretsectors", menusform.ShowSecretSectors);
			General.Settings.WritePluginSetting("automapmode.showlocks", menusform.ShowLocks);
            General.Settings.WritePluginSetting("automapmode.showtextures", menusform.ShowTextures);
            General.Settings.WritePluginSetting("automapmode.colorpreset", (int)menusform.ColorPreset);

			//mxd. Hide UI
			menusform.Unregister();

			// Hide highlight info
			General.Interface.HideInfo();
		}

		//mxd
		public override void OnUndoEnd()
		{
			UpdateValidLinedefs();
			UpdateSecretSectors();

			base.OnUndoEnd();
		}

		//mxd
		public override void OnRedoEnd()
		{
			UpdateValidLinedefs();
			UpdateSecretSectors();

			base.OnRedoEnd();
		}

		// This redraws the display
		public override void OnRedrawDisplay()
		{
			renderer.RedrawSurface();
			
			// Render lines
			if(renderer.StartPlotter(true))
			{
				foreach(Linedef ld in General.Map.Map.Linedefs)
				{
					if(LinedefIsValid(ld))
						renderer.PlotLine(ld.Start.Position, ld.End.Position, DetermineLinedefColor(ld), LINE_LENGTH_SCALER);
				}

				if((highlighted != null) && !highlighted.IsDisposed && LinedefIsValid(highlighted))
				{
					renderer.PlotLine(highlighted.Start.Position, highlighted.End.Position, General.Colors.InfoLine, LINE_LENGTH_SCALER);
				}

				renderer.Finish();
			}

			//mxd. Render background
			if(renderer.StartOverlay(true))
			{
				if(!ShowTextures()) {
					RectangleF screenrect = new RectangleF(0, 0, General.Interface.Display.Width, General.Interface.Display.Height);
					renderer.RenderRectangleFilled(screenrect, ColorBackground, false);
				}
				renderer.Finish();
			}

			renderer.Present();
		}

		protected override void OnSelectEnd()
		{
			// Item highlighted?
			if((highlighted != null) && !highlighted.IsDisposed)
			{
				General.Map.UndoRedo.CreateUndo("Toggle \"Shown as 1-sided on automap\" linedef flag");

				// Toggle flag
				highlighted.SetFlag(BuilderPlug.Me.SecretFlag, !highlighted.IsFlagSet(BuilderPlug.Me.SecretFlag));
				UpdateValidLinedefs();
			}

			base.OnSelectEnd();
		}
		
		protected override void OnEditEnd()
		{
			// Item highlighted?
			if((highlighted != null) && !highlighted.IsDisposed)
			{
				General.Map.UndoRedo.CreateUndo("Toggle \"Not shown on automap\" linedef flag");

				// Toggle flag
				highlighted.SetFlag(BuilderPlug.Me.HiddenFlag, !highlighted.IsFlagSet(BuilderPlug.Me.HiddenFlag));
				UpdateValidLinedefs();
				General.Interface.RedrawDisplay();
			}

			base.OnEditEnd();
		}
		
		// Mouse moves
		public override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);

			// Not holding any buttons?
			if(e.Button == MouseButtons.None)
			{
				// Find the nearest linedef within highlight range
				Linedef l = MapSet.NearestLinedefRange(validlinedefs, mousemappos, BuilderPlug.Me.HighlightRange / renderer.Scale);

				// Highlight if not the same
				if(l != highlighted) Highlight(l);
			}
		}

		// Mouse leaves
		public override void OnMouseLeave(EventArgs e)
		{
			base.OnMouseLeave(e);

			// Highlight nothing
			Highlight(null);
		}

		public override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);

			if(e.Control)
			{
				OnCtrl();
			}
			if(e.Shift)
			{
				OnShift();
			}
		}

		public override void OnKeyUp(KeyEventArgs e)
		{
			base.OnKeyUp(e);

			if(e.KeyCode == Keys.ControlKey)
			{
				OnCtrl();
			}
			else if(e.KeyCode == Keys.ShiftKey)
			{
				OnShift();
			}
		}

		private void OnCtrl()
		{
			UpdateValidLinedefs();
			General.Interface.RedrawDisplay();
		}

		private void OnShift()
		{
			General.Interface.RedrawDisplay();
		}

		#endregion
	}
}
