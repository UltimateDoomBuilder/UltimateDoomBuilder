


#region ================== Copyright (c) 2026 Boris Iwanski

/*
 * Copyright (c) 2026 Boris Iwanski 
 *
 * This file is part of Ultimate Doom Builder.
 *
 * Ultimate Doom Builder is free software: you can redistribute it and/or
 * modify it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or (at your
 * option) any later version.
 *
 * Ultimate Doom Builder is distributed in the hope that it will be useful, but
 * WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
 * or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for
 * more details.
 *
 * You should have received a copy of the GNU General Public License along with
 * Ultimate Doom Builder. If not, see <https://www.gnu.org/licenses/>. 
 * 
 */

#endregion
#region ================== Namespaces

using CodeImp.DoomBuilder.Actions;
using CodeImp.DoomBuilder.Editing;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Rendering;
using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Forms;


#endregion

namespace CodeImp.DoomBuilder.RejectExplorer
{
	[EditMode(DisplayName = "Reject Explorer",
			  SwitchAction = "rejectexplorermode",	// Action name used to switch to this mode
			  ButtonImage = "reject.png",	// Image resource name for the button
			  ButtonOrder = int.MinValue + 504,	// Position of the button (lower is more to the bottom)
			  ButtonGroup = "000_editing",
			  SupportedMapFormats = new[] { "DoomMapSetIO", "HexenMapSetIO" },
			  UseByDefault = true,
			  Volatile = true )]

	public class RejectExplorerMode : ClassicMode
	{

		#region ================== Variables

		// Highlighted items
		private Sector highlightedSector;

		private byte[] rejectData;
		private FlatVertex[] overlayGeometry;

		#endregion

		#region ================== Properties

		public override object HighlightedObject { get { return highlightedSector; } }
		
		#endregion

		#region ================== Constructor / Disposer

		//mxd
		public RejectExplorerMode()
		{
			// Do something
		}

		#endregion

		#region ================== Methods

		/// <summary>
		/// Loads the REJECT data from the map and validates it. If the lump is missing or invalid, the mode will be cancelled and a warning will be shown.
		/// See https://doomwiki.org/wiki/Reject for more information on the REJECT lump format.
		/// </summary>
		private void LoadRejectData()
		{
			if(!General.Map.LumpExists("REJECT"))
			{
				General.ToastManager.ShowToast(ToastMessages.REJECTEXPLORER, ToastType.ERROR, "Failed to engage Reject Explorer Mode", "Map has no REJECT lump.");
				General.Editing.CancelMode();
				return;
			}

			long expectedSize = (long)Math.Ceiling(General.Map.Map.Sectors.Count * General.Map.Map.Sectors.Count / 8.0);

			using (MemoryStream ms = General.Map.GetLumpData("REJECT"))
			{
				if (ms.Length == 0)
				{
					General.ToastManager.ShowToast(ToastMessages.REJECTEXPLORER, ToastType.ERROR, "Failed to engage Reject Explorer Mode", "REJECT lump is empty.");
					General.Editing.CancelMode();
					return;
				}
				else if (ms.Length < expectedSize)
				{
					General.ToastManager.ShowToast(ToastMessages.REJECTEXPLORER, ToastType.ERROR, "Failed to engage Reject Explorer Mode", $"REJECT lump is too small. Expected {expectedSize} bytes, got {ms.Length} bytes.");
					General.Editing.CancelMode();
					return;
				}
				else if (ms.Length > expectedSize)
				{
					General.ToastManager.ShowToast(ToastMessages.REJECTEXPLORER, ToastType.WARNING, "Reject Explorer Mode", $"REJECT lump is too large. Expected {expectedSize} bytes, got {ms.Length} bytes.");
				}

				using (BinaryReader br = new BinaryReader(ms))
				{
					rejectData = br.ReadBytes((int)ms.Length);
				}
			}
		}

		/// <summary>
		/// Checks if there is line of sight from sector s1 to sector s2 according to the REJECT data. Note that this is not necessarily symmetric;
		/// it's possible for s1 to have line of sight to s2 while s2 does not have line of sight to s1, depending on how the REJECT lump was generated.
		/// </summary>
		/// <param name="s1">The first sector.</param>
		/// <param name="s2">The second sector.</param>
		/// <returns>True if there is line of sight from s1 to s2, false otherwise.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private bool SectorHasLineofSight(Sector s1, Sector s2)
		{
			int index = s1.Index * General.Map.Map.Sectors.Count + s2.Index;
			int byteindex = index / 8;
			int bitindex = 1 << (index % 8);
			return (rejectData[byteindex] & bitindex) == 0;
		}

		/// <summary>
		/// Creates the overlay geometry for the mode.
		/// </summary>
		private void CreateOverlayGeometry()
		{
			overlayGeometry = General.Map.Map.Sectors.SelectMany(s => s.FlatVertices).ToArray();

			for (int i = 0; i < overlayGeometry.Length; i++)
				overlayGeometry[i].c = BuilderPlug.Me.ColorSettings.Default;
		}

		/// <summary>
		/// Updates the colors of the overlay geometry based on the currently highlighted sector and the REJECT data.
		/// </summary>
		private void UpdateOverlayGeometry()
		{
			int pos = 0;

			foreach (Sector s in General.Map.Map.Sectors)
			{
				int color = BuilderPlug.Me.ColorSettings.Default;

				if (s == highlightedSector)
					color = BuilderPlug.Me.ColorSettings.Highlight;
				else if (highlightedSector != null)
				{
					bool fromHighlighted = SectorHasLineofSight(highlightedSector, s);
					bool toHighlighted = SectorHasLineofSight(s, highlightedSector);

					if (fromHighlighted && toHighlighted)
						color = BuilderPlug.Me.ColorSettings.Bidirectional;
					else if (fromHighlighted)
						color = BuilderPlug.Me.ColorSettings.UnidirectionalFrom;
					else if (toHighlighted)
						color = BuilderPlug.Me.ColorSettings.UnidirectionalTo;
				}

				// Set the color for all vertices of this sector. Since we know that the vertices of each sector are stored
				// contiguously in the overlay geometry, we can just set the color for the next s.FlatVertices.Length vertices
				// and increment pos accordingly.
				for (int i = 0; i < s.FlatVertices.Length; i++, pos++)
					overlayGeometry[pos].c = color;
			}
		}

		/// <summary>
		/// Update the current highlight.
		/// </summary>
		private void UpdateHighlight()
		{
			Sector s = General.Map.Map.GetSectorByCoordinates(mousemappos);

			if (s != highlightedSector) HighlightSector(s);
		}

		/// <summary>
		/// This highlights a new sector.
		/// </summary>
		/// <param name="sector">Sector to highlight</param>		
		private void HighlightSector(Sector sector)
		{
			if(sector != highlightedSector)
			{
				highlightedSector = sector;
				UpdateOverlayGeometry();
			}

			General.Interface.RedrawDisplay();

			// Show highlight info
			if ((highlightedSector != null) && !highlightedSector.IsDisposed)
				General.Interface.ShowSectorInfo(highlightedSector);
			else
				General.Interface.HideInfo();
		}

		#endregion
		
		#region ================== Events

		public override void OnHelp()
		{
			General.ShowHelp("/gzdb/features/classic_modes/mode_rejectexplorer.html");
		}

		public override void OnCancel()
		{
			base.OnCancel();

			// Return to the last stable mode
			General.Editing.ChangeMode(General.Editing.PreviousStableMode.Name);
		}

		public override void OnEngage()
		{
			base.OnEngage();

			General.Interface.AddButton(BuilderPlug.Me.MenusForm.ColorConfiguration);

			CustomPresentation presentation = new CustomPresentation();
			presentation.AddLayer(new PresentLayer(RendererLayer.Background, BlendingMode.Mask, General.Settings.BackgroundAlpha));
			presentation.AddLayer(new PresentLayer(RendererLayer.Grid, BlendingMode.Mask));
			presentation.AddLayer(new PresentLayer(RendererLayer.Overlay, BlendingMode.Alpha, 1.0f, true)); // First overlay (0)
			presentation.AddLayer(new PresentLayer(RendererLayer.Overlay, BlendingMode.Alpha, 1.0f, true)); // Second overlay (1)
			presentation.AddLayer(new PresentLayer(RendererLayer.Geometry, BlendingMode.Alpha, 1.0f, true));
			renderer.SetPresentation(presentation);

			LoadRejectData();

			CreateOverlayGeometry();
		}

		public override void OnDisengage()
		{
			base.OnDisengage();

			General.Interface.RemoveButton(BuilderPlug.Me.MenusForm.ColorConfiguration);

			General.Interface.HideInfo();
		}

		public override void OnUndoEnd()
		{
			base.OnUndoEnd();
		}

		public override void OnRedoEnd()
		{
			base.OnRedoEnd();
		}

		public override void OnRedrawDisplay()
		{
			renderer.RedrawSurface();
			
			// Render lines
			if(renderer.StartPlotter(true))
			{
				renderer.PlotLinedefSet(General.Map.Map.Linedefs);
				renderer.Finish();
			}

			// Render overlay
			if (renderer.StartOverlay(true))
			{
				renderer.RenderGeometry(overlayGeometry, null, true);
				renderer.Finish();
			}

			renderer.Present();
		}

		public override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);

			// Not holding any buttons?
			if(e.Button == MouseButtons.None)
			{
				UpdateHighlight();
			}
		}

		public override void OnMouseLeave(EventArgs e)
		{
			base.OnMouseLeave(e);

			// Highlight nothing
			HighlightSector(null);
		}

		#endregion

		#region ================== Actions

		[BeginAction("rejectexplorercolorconfiguration")]
		public void ConfigureColors()
		{
			using (ColorConfiguration cc = new ColorConfiguration())
			{
				if (cc.ShowDialog((Form)General.Interface) == DialogResult.OK)
				{
					UpdateOverlayGeometry();
					General.Interface.RedrawDisplay();
				}
			}
		}

		#endregion
	}
}
