
#region ================== Copyright (c) 2007 Pascal vd Heiden, 2014 Boris Iwanski

/*
 * Copyright (c) 2007 Pascal vd Heiden, www.codeimp.com
 * Copyright (c) 2014 Boris Iwanski
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
using System.ComponentModel;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Editing;
using CodeImp.DoomBuilder.Actions;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Windows;
using System.Diagnostics;

#endregion

namespace CodeImp.DoomBuilder.SoundPropagationMode
{
	[EditMode(DisplayName = "Sound Propagation Mode",
			  SwitchAction = "soundpropagationmode",		// Action name used to switch to this mode
			  ButtonImage = "SoundPropagationIcon.png",	// Image resource name for the button
			  ButtonOrder = int.MinValue + 501,	// Position of the button (lower is more to the left)
			  ButtonGroup = "000_editing",
			  UseByDefault = true,
			  SafeStartMode = false,
			  Volatile = false)]

	public class SoundPropagationMode : ClassicMode
	{
		#region ================== Variables
		
		// Highlighted item
		private Sector highlighted;
		private Linedef highlightedline; //mxd

		private FlatVertex[] overlayGeometry;

		private List<Thing> huntingThings;
		private List<SoundPropagationDomain> propagationdomains;
		private Dictionary<Sector, SoundPropagationDomain> sector2domain;
		private LeakFinder leakfinder;
		private PixelColor doublesidedcolor;

		// The blockmap makes is used to make finding lines faster
		private BlockMap<BlockEntry> blockmap;

		private Sector leakstartsector;
		private Sector leakendsector;
		private Vector2D leakstartposition;
		private Vector2D leakendposition;
		private TextLabel leakstartlabel;
		private TextLabel leakendlabel;
		private BackgroundWorker worker;

		#endregion

		#region ================== Properties

		public override object HighlightedObject { get { return highlighted; } }
		internal static string BlockSoundFlag { get { return (General.Map.UDMF ? "blocksound" : "64"); } } //mxd

		#endregion

		#region ================== Constructor / Disposer

		// Disposer
		public override void Dispose()
		{
			// Not already disposed?
			if(!isdisposed)
			{
				// Dispose base
				base.Dispose();
			}
		}

		#endregion

		#region ================== Methods

		private void UpdateData()
		{
			BuilderPlug.Me.DataIsDirty = false;
			List<FlatVertex> vertsList = new List<FlatVertex>();

			// Go for all sectors
			foreach(Sector s in General.Map.Map.Sectors) vertsList.AddRange(s.FlatVertices);
			overlayGeometry = vertsList.ToArray();

			for(int i = 0; i < overlayGeometry.Length; i++)
				overlayGeometry[i].c = BuilderPlug.Me.NoSoundColor.WithAlpha(128).ToInt();
		}

		// This highlights a new item
		private void Highlight(Sector s)
		{
			// Set new highlight
			highlighted = s;

			UpdateSoundPropagation();
		}

		//mxd
		private void ResetSoundPropagation()
		{
			sector2domain.Clear();
			propagationdomains.Clear();
			BuilderPlug.Me.BlockingLinedefs.Clear();
			UpdateSoundPropagation();
		}

		private void UpdateSoundPropagation()
		{
			huntingThings.Clear();
			BuilderPlug.Me.BlockingLinedefs.Clear();

			foreach(Linedef ld in General.Map.Map.Linedefs)
			{
				if(ld.IsFlagSet(BlockSoundFlag)) BuilderPlug.Me.BlockingLinedefs.Add(ld);
			}

			//mxd. Create sound propagation for the whole map
			int counter = 0;
			foreach(Sector sector in General.Map.Map.Sectors)
			{
				if(!sector2domain.ContainsKey(sector))
				{
					SoundPropagationDomain spd = new SoundPropagationDomain(sector);
					foreach(Sector s in spd.Sectors) sector2domain[s] = spd;
					spd.Color = BuilderPlug.Me.DistinctColors[counter++ % BuilderPlug.Me.DistinctColors.Count].WithAlpha(255).ToInt();
					propagationdomains.Add(spd);
				}
			}

			if(highlighted == null || highlighted.IsDisposed) return;

			//mxd. Create the list of sectors, which will be affected by noise made in highlighted sector
			SoundPropagationDomain curdomain = sector2domain[highlighted];
			Dictionary<int, Sector> noisysectors = new Dictionary<int, Sector>(curdomain.Sectors.Count);
			foreach(Sector s in curdomain.Sectors)
			{
				noisysectors.Add(s.Index, s);
			}

			foreach(Sector s in curdomain.AdjacentSectors)
			{
				SoundPropagationDomain aspd = sector2domain[s];
				foreach(Sector adjs in aspd.Sectors)
				{
					if(!noisysectors.ContainsKey(adjs.Index)) noisysectors.Add(adjs.Index, adjs);
				}
			}

			// Update the list of things that will actually go for the player when hearing a noise
			foreach(Thing thing in General.Map.Map.Things)
			{
				if(!General.Map.ThingsFilter.VisibleThings.Contains(thing)) continue;
				if(thing.IsFlagSet(General.Map.UDMF ? "ambush" : "8")) continue;
				if(thing.Sector != null && noisysectors.ContainsKey(thing.Sector.Index)) huntingThings.Add(thing);
			}
		}

		/// <summary>
		/// Create a blockmap containing linedefs. This is used to speed up determining the closest line
		/// to the mouse cursor
		/// </summary>
		private void CreateBlockmap()
		{
			RectangleF area = MapSet.CreateArea(General.Map.Map.Vertices);
			blockmap = new BlockMap<BlockEntry>(area);
			blockmap.AddLinedefsSet(General.Map.Map.Linedefs);
			blockmap.AddSectorsSet(General.Map.Map.Sectors);
		}

		#endregion

		#region ================== Events

		public override void OnHelp()
		{
			General.ShowHelp("gzdb/features/classic_modes/mode_soundpropagation.html");
		}

		// Cancel mode
		public override void OnCancel()
		{
			// Cancel base class
			base.OnCancel();

			// Return to previous mode
			General.Editing.ChangeMode(new SoundPropagationMode());
		}

		// Mode engages
		public override void OnEngage()
		{
			base.OnEngage();

			huntingThings = new List<Thing>();
			propagationdomains = new List<SoundPropagationDomain>();
			sector2domain = new Dictionary<Sector, SoundPropagationDomain>();
			BuilderPlug.Me.BlockingLinedefs = new List<Linedef>();

			doublesidedcolor = General.Colors.Linedefs.WithAlpha(General.Settings.DoubleSidedAlphaByte);

			UpdateData();

			General.Interface.AddButton(BuilderPlug.Me.MenusForm.ColorConfiguration);

			CustomPresentation presentation = new CustomPresentation();
			presentation.AddLayer(new PresentLayer(RendererLayer.Background, BlendingMode.Mask, General.Settings.BackgroundAlpha));
			presentation.AddLayer(new PresentLayer(RendererLayer.Grid, BlendingMode.Mask));
			presentation.AddLayer(new PresentLayer(RendererLayer.Overlay, BlendingMode.Alpha, 1.0f, true)); // First overlay (0)
			presentation.AddLayer(new PresentLayer(RendererLayer.Things, BlendingMode.Alpha, 1.0f));
			presentation.AddLayer(new PresentLayer(RendererLayer.Geometry, BlendingMode.Alpha, 1.0f, true));
			presentation.AddLayer(new PresentLayer(RendererLayer.Overlay, BlendingMode.Alpha, 1.0f, true)); // Second overlay (1)
			renderer.SetPresentation(presentation);

			leakstartlabel = new TextLabel
			{
				TransformCoords = true,
				AlignX = TextAlignmentX.Center,
				AlignY = TextAlignmentY.Middle,
				Color = General.Colors.Selection,
				BackColor = General.Colors.Background,
				Text = "S"
			};

			leakendlabel = new TextLabel
			{
				TransformCoords = true,
				AlignX = TextAlignmentX.Center,
				AlignY = TextAlignmentY.Middle,
				Color = General.Colors.Selection,
				BackColor = General.Colors.Background,
				Text = "E"
			};

			// Create the blockmap
			CreateBlockmap();

			// To show things that will wake up we need to know the sector they are in
			Parallel.ForEach(General.Map.Map.Things, t => t.DetermineSector(blockmap));

			// Convert geometry selection to sectors only
			General.Map.Map.ConvertSelection(SelectionType.Sectors);

			UpdateSoundPropagation();
		}

		// Mode disengages
		public override void OnDisengage()
		{
			base.OnDisengage();
			General.Interface.RemoveButton(BuilderPlug.Me.MenusForm.ColorConfiguration);

			// Hide highlight info
			General.Interface.HideInfo();

			if (worker != null)
			{
				worker.CancelAsync();
				worker.Dispose();
			}
		}

		// This redraws the display
		public override void OnRedrawDisplay()
		{
			List<SoundPropagationDomain> renderedspds = new List<SoundPropagationDomain>();
			if (BuilderPlug.Me.DataIsDirty) UpdateData();

			// We don't care for the actualy surfaces, but without this the render targets will not be recreated
			// when the window is resized
			renderer.RedrawSurface();

			// Render lines and vertices
			if (renderer.StartPlotter(true))
			{

				// Plot lines by hand, so that no coloring (line specials, 3D floors etc.) distracts from
				// the sound propagation. Also don't draw the line's normal. They are not needed here anyway
				// and can make it harder to see the sound environment propagation
				if (General.Settings.ParallelizedLinedefPlotting)
				{
					Parallel.ForEach(General.Map.Map.Linedefs, ld =>
					{
						PixelColor c = ld.IsFlagSet(General.Map.Config.ImpassableFlag) ? General.Colors.Linedefs : doublesidedcolor;
						renderer.PlotLine(ld.Start.Position, ld.End.Position, c, BuilderPlug.LINE_LENGTH_SCALER);
					});
				}
				else
				{
					foreach (Linedef ld in General.Map.Map.Linedefs)
					{
						PixelColor c = ld.IsFlagSet(General.Map.Config.ImpassableFlag) ? General.Colors.Linedefs : doublesidedcolor;
						renderer.PlotLine(ld.Start.Position, ld.End.Position, c, BuilderPlug.LINE_LENGTH_SCALER);
					}
				}

				// Since there will usually be way less blocking linedefs than total linedefs, it's presumably
				// faster to draw them on their own instead of checking if each linedef is in BlockingLinedefs
				foreach (Linedef ld in BuilderPlug.Me.BlockingLinedefs)
					renderer.PlotLine(ld.Start.Position, ld.End.Position, BuilderPlug.Me.BlockSoundColor, BuilderPlug.LINE_LENGTH_SCALER);

				//mxd. Render highlighted line
				if (highlightedline != null)
					renderer.PlotLine(highlightedline.Start.Position, highlightedline.End.Position, General.Colors.Highlight, BuilderPlug.LINE_LENGTH_SCALER);

				renderer.Finish();
			}

			// Render things
			if (renderer.StartThings(true))
			{
				renderer.RenderThingSet(General.Map.ThingsFilter.HiddenThings, General.Settings.HiddenThingsAlpha);
				renderer.RenderThingSet(General.Map.ThingsFilter.VisibleThings, General.Settings.InactiveThingsAlpha);
				renderer.RenderThingSet(huntingThings, General.Colors.Selection, General.Settings.ActiveThingsAlpha);

				renderer.Finish();
			}

			// The sound propagation domain overlay
			if (renderer.StartOverlay(true))
			{
				// Render highlighted domain and domains adjacent to it
				if (highlighted != null && !highlighted.IsDisposed)
				{
					renderer.RenderGeometry(overlayGeometry, null, true); //mxd

					SoundPropagationDomain spd = sector2domain[highlighted];
					renderer.RenderGeometry(spd.Level1Geometry, null, true);

					foreach (Sector s in spd.AdjacentSectors)
					{
						SoundPropagationDomain aspd = sector2domain[s];
						if (!renderedspds.Contains(aspd))
						{
							renderer.RenderGeometry(aspd.Level2Geometry, null, true);
							renderedspds.Add(aspd);
						}
					}

					renderer.RenderHighlight(highlighted.FlatVertices, BuilderPlug.Me.HighlightColor.WithAlpha(128).ToInt()); //mxd
				}
				else
				{
					//mxd. Render all domains using domain colors
					foreach (SoundPropagationDomain spd in propagationdomains)
						renderer.RenderHighlight(spd.Level1Geometry, spd.Color);
				}

				renderer.Finish();
			}

			// The sound leak overlay. This is done so that the path and labels are drawn at the very top
			if (renderer.StartOverlay(true, 1))
			{
				if (leakfinder != null && leakfinder.Finished)
					leakfinder.End.RenderPath(renderer);

				if (leakstartsector != null)
					renderer.RenderText(leakstartlabel);

				if (leakendsector != null)
					renderer.RenderText(leakendlabel);

				renderer.Finish();
			}

			renderer.Present();
		}

		//mxd. If a linedef is highlighted, toggle the sound blocking flag 
		protected override void OnSelectEnd()
		{
			if(highlightedline == null) return;

			// Make undo
			General.Map.UndoRedo.CreateUndo("Toggle Linedef Sound Blocking");

			// Toggle flag
			highlightedline.SetFlag(BlockSoundFlag, !highlightedline.IsFlagSet(BlockSoundFlag));
			
			// Update
			ResetSoundPropagation();

			FindSoundLeak();

			General.Interface.RedrawDisplay();
		}

		public override bool OnUndoBegin()
		{
			base.OnUndoBegin();

			if (worker != null)
			{
				worker.CancelAsync();
				worker.Dispose();
				worker = null;
			}

			return true;
		}

		//mxd
		public override void OnUndoEnd()
		{
			base.OnUndoEnd();

			// Recreate the blockmap
			CreateBlockmap();

			// To show things that will wake up we need to know the sector they are in
			Parallel.ForEach(General.Map.Map.Things, t => t.DetermineSector(blockmap));

			// Update
			ResetSoundPropagation();
			General.Interface.RedrawDisplay();
		}

		public override bool OnRedoBegin()
		{
			base.OnRedoBegin();

			if (worker != null)
			{
				worker.CancelAsync();
				worker.Dispose();
				worker = null;
			}

			return true;
		}

		//mxd
		public override void OnRedoEnd()
		{
			base.OnRedoEnd();

			// Recreate the blockmap
			CreateBlockmap();

			// To show things that will wake up we need to know the sector they are in
			Parallel.ForEach(General.Map.Map.Things, t => t.DetermineSector(blockmap));

			// Update
			ResetSoundPropagation();
			General.Interface.RedrawDisplay();
		}

		// Mouse moves
		public override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);

			// Not holding any buttons?
			if(e.Button == MouseButtons.None)
			{
				General.Interface.SetCursor(Cursors.Default);

				//mxd. Find the nearest linedef within default highlight range
				Linedef nl = MapSet.NearestLinedefRange(blockmap, mousemappos, 20 / renderer.Scale);
				//mxd. We are not interested in single-sided lines (unless they have "blocksound" flag set)...
				if(nl != null && (nl.Front == null || nl.Back == null) && !nl.IsFlagSet(BlockSoundFlag)) nl = null;

				//mxd. Set as highlighted
				bool redrawrequired = (highlightedline != nl);
				highlightedline = nl;
				
				// Find the nearest linedef within highlight range
				Linedef l = MapSet.NearestLinedef(blockmap, mousemappos);
				if(l != null)
				{
					// Check on which side of the linedef the mouse is
					double side = l.SideOfLine(mousemappos);
					if(side > 0)
					{
						// Is there a sidedef here?
						if(l.Back != null)
						{
							// Highlight if not the same
							if(l.Back.Sector != highlighted)
							{
								Highlight(l.Back.Sector);
								redrawrequired = true; //mxd
							}
						}
						else if(highlighted != null)
						{
							// Highlight nothing
							Highlight(null);
							redrawrequired = true; //mxd
						}
					}
					else
					{
						// Is there a sidedef here?
						if(l.Front != null)
						{
							// Highlight if not the same
							if(l.Front.Sector != highlighted)
							{
								Highlight(l.Front.Sector);
								redrawrequired = true; //mxd
							}
						}
						else if(highlighted != null)
						{
							// Highlight nothing
							Highlight(null);
							redrawrequired = true; //mxd
						}
					}
				}
				else if(highlighted != null)
				{
					// Highlight nothing
					Highlight(null);
					redrawrequired = true; //mxd
				}

				//mxd
				if(redrawrequired)
				{
					// Show highlight info
					if(highlightedline != null && !highlightedline.IsDisposed)
						General.Interface.ShowLinedefInfo(highlightedline);
					else if(highlighted != null && !highlighted.IsDisposed)
						General.Interface.ShowSectorInfo(highlighted);
					else
						General.Interface.HideInfo();

					// Redraw display
					General.Interface.RedrawDisplay();
				}
			}
		}

		// Mouse leaves
		public override void OnMouseLeave(EventArgs e)
		{
			base.OnMouseLeave(e);

			// Highlight nothing
			Highlight(null);
		}

		/// <summary>
		/// Starts finding a sound leak. Finding the actual leak is done by a background worker that's started from this method
		/// </summary>
		private void FindSoundLeak()
		{
			leakfinder = null;

			// Do not show an error if either start or end is not set, since that'll happen when you start out
			if (leakstartsector == null || leakendsector == null)
				return;

			if (leakendsector == leakstartsector)
			{
				General.ToastManager.ShowToast(ToastMessages.SOUNDPROPAGATIONMODE, ToastType.WARNING, "Sound propagation", "Stard and end position for sound leak are in the same sector");
				return;
			}

			HashSet<Sector> sectors = new HashSet<Sector>(sector2domain[leakstartsector].Sectors);

			// Mash all sectors from the leak start sector's domain and the adjacent domains into one hash set
			foreach (Sector s in sector2domain[leakstartsector].AdjacentSectors)
				sectors.UnionWith(sector2domain[s].Sectors);

			// If the leak end sector isn't in the list of sectors there's no way sound can travel between the start and end
			if (!sectors.Contains(leakendsector))
			{
				General.ToastManager.ShowToast(ToastMessages.SOUNDPROPAGATIONMODE, ToastType.WARNING, "Sound propagation", "Sound can not travel between the selected start and end positions");
				return;
			}

			if (worker != null)
			{
				worker.CancelAsync();
				worker.Dispose();
			}

			General.Interface.DisplayStatus(StatusType.Busy, "Searching for sound leak...");

			worker = new BackgroundWorker();
			worker.WorkerSupportsCancellation = true;
			worker.DoWork += FindSoundLeakStart;
			worker.RunWorkerCompleted += FindSoundLeakFinished;
			worker.RunWorkerAsync(sectors);
		}

		/// <summary>
		/// Method for the background worker that finds a sound leak.
		/// </summary>
		/// <param name="sender">The sender</param>
		/// <param name="e">The event arguments</param>
		private void FindSoundLeakStart(object sender, DoWorkEventArgs e)
		{
			Stopwatch sw = Stopwatch.StartNew();

			leakfinder = new LeakFinder(leakstartsector, leakstartposition, leakendsector, leakendposition, (HashSet<Sector>)e.Argument);

			if (leakfinder.FindLeak() == false)
				General.ToastManager.ShowToast(ToastMessages.SOUNDPROPAGATIONMODE, ToastType.WARNING, "Sound propagation", "Could not find a leak between the selected start and end positions, even though there should be one. This is weird");
			else
				General.Interface.DisplayStatus(StatusType.Info, string.Format(@"Searching for sound leak finished. Elapsed time: {0:mm\:ss\.ff}", sw.Elapsed));
		}


		/// <summary>
		/// Method that's called when finding a sound leak finished.
		/// </summary>
		/// <param name="sender">The sender</param>
		/// <param name="e">The event arguments</param>
		private void FindSoundLeakFinished(object sender, RunWorkerCompletedEventArgs e)
		{
			General.Interface.RedrawDisplay();
		}

		#endregion

		#region ================== Actions

		[BeginAction("soundpropagationcolorconfiguration")]
		public void ConfigureColors()
		{
			using(ColorConfiguration cc = new ColorConfiguration())
			{
				if (cc.ShowDialog((Form)General.Interface) == DialogResult.OK)
					General.Interface.RedrawDisplay();
			}
		}

		[BeginAction("setleakfinderstart")]
		public void SetLeakFinderStartSector()
		{
			leakstartsector = highlighted;
			leakstartposition = mousemappos;
			leakstartlabel.Location = mousemappos;
			leakfinder = null;

			// Redraw to show the label
			General.Interface.RedrawDisplay();

			FindSoundLeak();
		}

		[BeginAction("setleakfinderend")]
		public void SetLeakFinderEndSector()
		{
			leakendsector = highlighted;
			leakendposition = mousemappos;
			leakendlabel.Location = mousemappos;
			leakfinder = null;

			// Redraw to show the label
			General.Interface.RedrawDisplay();

			FindSoundLeak();
		}

		[BeginAction("clearselection", BaseAction = true)]
		public void ClearLeakFinder()
		{
			leakendsector = leakstartsector = null;
			leakfinder = null;

			General.Interface.RedrawDisplay();
		}

		#endregion
	}
}
