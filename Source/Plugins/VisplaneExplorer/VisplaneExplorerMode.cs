#region === Copyright (c) 2010 Pascal van der Heiden ===

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.Editing;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Windows;

#endregion

namespace CodeImp.DoomBuilder.Plugins.VisplaneExplorer
{
	[EditMode(DisplayName = "Visplane Explorer",
			  SwitchAction = "visplaneexplorermode",
			  ButtonImage = "Gauge.png",
			  ButtonOrder = 300,
			  ButtonGroup = "002_tools",
			  Volatile = true,
			  UseByDefault = true,
			  SupportedMapFormats = new[] { "DoomMapSetIO", "HexenMapSetIO" }, //mxd
			  AllowCopyPaste = false)]
	public class VisplaneExplorerMode : ClassicMode
	{
		#region ================== Variables

		// The image is the ImageData resource for Doom Builder to work with
		private DynamicBitmapImage image;
		
		// This is the bitmap that we will be drawing on
		private Bitmap canvas;
		private ViewStats lastviewstats;
		
		// Temporary WAD file written for the vpo.dll library
		private string tempfile;

		// Rectangle around the map
		private Rectangle mapbounds;

		// 64x64 tiles in map space. These are discarded when outside view.
		private Dictionary<Point, Tile> tiles = new Dictionary<Point, Tile>();

		// Time when to do another update
		private long nextupdate;

		// Are we processing?
		private bool processingenabled;
		
		#endregion

		#region ================== Methods

		// This cleans up anything we used for this mode
		private void CleanUp()
		{
			BuilderPlug.VPO.Stop();

			if(processingenabled)
			{
				General.Interface.DisableProcessing();
				processingenabled = false;
			}
			
			if(!string.IsNullOrEmpty(tempfile))
			{
				File.Delete(tempfile);
				tempfile = null;
			}

			if(image != null)
			{
				image.Dispose();
				image = null;
			}

			if(canvas != null)
			{
				canvas.Dispose();
				canvas = null;
			}

			tiles.Clear();
			BuilderPlug.InterfaceForm.HideTooltip();
			BuilderPlug.InterfaceForm.RemoveFromInterface();
		}

		// This returns the tile position for the given map coordinate
		private static Point TileForPoint(float x, float y)
		{
			return new Point((int)Math.Floor(x / Tile.TILE_SIZE) * Tile.TILE_SIZE, (int)Math.Floor(y / Tile.TILE_SIZE) * Tile.TILE_SIZE);
		}

		// This draws all tiles on the image
		// THIS MUST BE FAST! TOP PERFORMANCE REQUIRED!
		private unsafe void RedrawAllTiles()
		{
			if(canvas == null) return;

			// Determine viewport rectangle in map space
			Vector2D mapleftbot = Renderer.DisplayToMap(new Vector2D(0f, 0f));
			Vector2D maprighttop = Renderer.DisplayToMap(new Vector2D(General.Interface.Display.ClientSize.Width, General.Interface.Display.ClientSize.Height));
			Rectangle mapviewrect = new Rectangle((int)mapleftbot.x - Tile.TILE_SIZE, (int)maprighttop.y - Tile.TILE_SIZE, (int)maprighttop.x - (int)mapleftbot.x + Tile.TILE_SIZE, (int)mapleftbot.y - (int)maprighttop.y + Tile.TILE_SIZE);

			int viewstats = (int)BuilderPlug.InterfaceForm.ViewStats;
			Palette pal = (BuilderPlug.InterfaceForm.ShowHeatmap ? BuilderPlug.Palettes[(int)ViewStats.Heatmap] : BuilderPlug.Palettes[viewstats]);

			BitmapData bd = canvas.LockBits(new Rectangle(0, 0, canvas.Size.Width, canvas.Size.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
			uint* p = (uint*)bd.Scan0.ToPointer();

			int count = bd.Width * bd.Height;
			for (int i = 0; i < count; i++)
			{
				p[i] = 0;
			}

			foreach(KeyValuePair<Point, Tile> t in tiles)
			{
				if(!mapviewrect.Contains(t.Key)) continue;
				
				// Map this tile to screen space
				Vector2D lb = Renderer.MapToDisplay(new Vector2D(t.Value.Position.X, t.Value.Position.Y));
				Vector2D rt = Renderer.MapToDisplay(new Vector2D(t.Value.Position.X + Tile.TILE_SIZE, t.Value.Position.Y + Tile.TILE_SIZE));

				// Make sure the coordinates are aligned with canvas pixels
				float x1 = (float)Math.Round(lb.x);
				float x2 = (float)Math.Round(rt.x);
				float y1 = (float)Math.Round(rt.y);
				float y2 = (float)Math.Round(lb.y);

				// Determine width and height of the screen space area for this tile
				float w = x2 - x1;
				float h = y2 - y1;
				float winv = 1f / w;
				float hinv = 1f / h;

				// Loop ranges. These are relative to the left-top of the tile.
				float sx = 0f;
				float sy = 0f;
				float ex = w;
				float ey = h;
				int screenx = (int)x1;
				int screenystart = (int)y1;
				
				// Clipping the loop ranges against canvas boundary.
				if(x1 < 0f) { sx = -x1; screenx = 0; }
				if(y1 < 0f) { sy = -y1; screenystart = 0; }
				if(x2 > bd.Width) ex = w - (x2 - bd.Width);
				if(y2 > bd.Height) ey = h - (y2 - bd.Height);
				
				// Draw all pixels within this tile
				for(float x = sx; x < ex; x++, screenx++)
				{
					int screeny = screenystart;
					for(float y = sy; y < ey; y++, screeny++)
					{
						// Calculate the relative offset in map coordinates for this pixel
						float ux = x * winv * Tile.TILE_SIZE;
						float uy = y * hinv * Tile.TILE_SIZE;

						// Get the data and apply the color
						byte value = t.Value.GetHeatmapByte((int)ux, Tile.TILE_SIZE - 1 - (int)uy, viewstats);
						p[screeny * bd.Width + screenx] = (uint)pal.Colors[value];
					}
				}
			}
			
			canvas.UnlockBits(bd);
			image.UpdateTexture(canvas);
		}

		// This queues points for all current tiles
		private void QueuePoints(int pointsleft)
		{
			// Determine viewport rectangle in map space
			Vector2D mapleftbot = Renderer.DisplayToMap(new Vector2D(0f, 0f));
			Vector2D maprighttop = Renderer.DisplayToMap(new Vector2D(General.Interface.Display.ClientSize.Width, General.Interface.Display.ClientSize.Height));
			Rectangle mapviewrect = new Rectangle((int)mapleftbot.x - Tile.TILE_SIZE, (int)maprighttop.y - Tile.TILE_SIZE, (int)maprighttop.x - (int)mapleftbot.x + Tile.TILE_SIZE, (int)mapleftbot.y - (int)maprighttop.y + Tile.TILE_SIZE);
			
			while(pointsleft < (VPOManager.POINTS_PER_ITERATION * BuilderPlug.VPO.NumThreads * 5))
			{
				// Collect points from the tiles in the current view
				List<TilePoint> newpoints = new List<TilePoint>(tiles.Count);
				foreach(KeyValuePair<Point, Tile> t in tiles)
					if((!t.Value.IsComplete) && (mapviewrect.Contains(t.Key))) newpoints.Add(t.Value.GetNextPoint());
				
				// If the current view is complete, try getting points from all tiles
				if(newpoints.Count == 0)
				{
					foreach(KeyValuePair<Point, Tile> t in tiles)
						if(!t.Value.IsComplete) newpoints.Add(t.Value.GetNextPoint());
				}
				
				if(newpoints.Count == 0) break;
				pointsleft = BuilderPlug.VPO.EnqueuePoints(newpoints);
			}
		}

		// This updates the overlay
		private void UpdateOverlay()
		{
			// We must redraw the tiles to the canvas when the stats to view has changed
			if(lastviewstats != BuilderPlug.InterfaceForm.ViewStats)
			{
				RedrawAllTiles();
				lastviewstats = BuilderPlug.InterfaceForm.ViewStats;
			}
			
			// Render the overlay
			if(renderer.StartOverlay(true))
			{
				// Render the canvas to screen
				RectangleF r = new RectangleF(0, 0, canvas.Width, canvas.Height);
				renderer.RenderRectangleFilled(r, PixelColor.FromColor(Color.White), false, image);

				// Render any selection
				if(selecting) RenderMultiSelection();

				// Finish our rendering to this layer.
				renderer.Finish();
			}
		}

		#endregion

		#region ================== Events

		// Mode starts
		public override void OnEngage()
		{
			Cursor.Current = Cursors.WaitCursor;
			base.OnEngage();
			General.Interface.DisplayStatus(StatusType.Busy, "Setting up test environment...");

			BuilderPlug.InitVPO(); //mxd

			CleanUp();

			BuilderPlug.InterfaceForm.AddToInterface();
			BuilderPlug.InterfaceForm.OnVisplaneSettingsChanged += OnVisplaneSettingsChanged; //mxd
			lastviewstats = BuilderPlug.InterfaceForm.ViewStats;

			// Export the current map to a temporary WAD file
			tempfile = BuilderPlug.MakeTempFilename(".wad");
			if(!General.Map.ExportToFile(tempfile))
			{
				//mxd. Abort on export fail
				Cursor.Current = Cursors.Default;
				General.Interface.DisplayStatus(StatusType.Warning, "Unable to set test environment...");
				OnCancel();
				return;
			}

			// Load the map in VPO_DLL
			BuilderPlug.VPO.Start(tempfile, General.Map.Options.LevelName);

			// Determine map boundary
			mapbounds = Rectangle.Round(MapSet.CreateArea(General.Map.Map.Vertices));

			// Create tiles for all points inside the map
			CreateTiles(); //mxd

			QueuePoints(0);

			// Make an image to draw on.
			// The BitmapImage for Doom Builder's resources must be Format32bppArgb and NOT using color correction,
			// otherwise DB will make a copy of the bitmap when LoadImage() is called! This is normally not a problem,
			// but we want to keep drawing to the same bitmap.
			int width = General.NextPowerOf2(General.Interface.Display.ClientSize.Width);
			int height = General.NextPowerOf2(General.Interface.Display.ClientSize.Height);
			canvas = new Bitmap(width, height, PixelFormat.Format32bppArgb);
			image = new DynamicBitmapImage(canvas, "_CANVAS_");
			image.UseColorCorrection = false;
			image.MipMapLevels = 1;
			image.LoadImageNow();

			// Make custom presentation
			CustomPresentation p = new CustomPresentation();
			p.AddLayer(new PresentLayer(RendererLayer.Overlay, BlendingMode.Mask, 1f, false));
			p.AddLayer(new PresentLayer(RendererLayer.Grid, BlendingMode.Mask));
			p.AddLayer(new PresentLayer(RendererLayer.Geometry, BlendingMode.Alpha, 1f, true));
			renderer.SetPresentation(p);

			// Setup processing
			nextupdate = Clock.CurrentTime + 100;
			General.Interface.EnableProcessing();
			processingenabled = true;

			RedrawAllTiles();
			Cursor.Current = Cursors.Default;
			General.Interface.SetCursor(Cursors.Cross);
			General.Interface.DisplayReady();
		}

		//mxd
		private void CreateTiles()
		{
			Point lt = TileForPoint(mapbounds.Left - Tile.TILE_SIZE, mapbounds.Top - Tile.TILE_SIZE);
			Point rb = TileForPoint(mapbounds.Right + Tile.TILE_SIZE, mapbounds.Bottom + Tile.TILE_SIZE);
			Rectangle tilesrect = new Rectangle(lt.X, lt.Y, rb.X - lt.X, rb.Y - lt.Y);
			NearestLineBlockmap blockmap = new NearestLineBlockmap(tilesrect);
			for(int x = tilesrect.X; x <= tilesrect.Right; x += Tile.TILE_SIZE)
			{
				for(int y = tilesrect.Y; y <= tilesrect.Bottom; y += Tile.TILE_SIZE)
				{
					// If the tile is obviously outside the map, don't create it
					Vector2D pc = new Vector2D(x + (Tile.TILE_SIZE >> 1), y + (Tile.TILE_SIZE >> 1));
					Linedef ld = MapSet.NearestLinedef(blockmap.GetBlockAt(pc).Lines, pc);
					double distancesq = ld.DistanceToSq(pc, true);
					if(distancesq > (Tile.TILE_SIZE * Tile.TILE_SIZE))
					{
						double side = ld.SideOfLine(pc);
						if((side > 0.0f) && (ld.Back == null))
							continue;
					}

					Point tp = new Point(x, y);
					tiles.Add(tp, new Tile(tp));
				}
			}
		}

		// Mode ends
		public override void OnDisengage()
		{
			CleanUp();
			base.OnDisengage();
		}

		// Cancelled
		public override void OnCancel()
		{
			// Cancel base class
			base.OnCancel();

			// Return to previous mode
			General.Editing.ChangeMode(General.Editing.PreviousStableMode.Name);
		}

		// View position/scale changed!
		protected override void OnViewChanged()
		{
			base.OnViewChanged();

			RedrawAllTiles();
			
			// Update the screen sooner
			nextupdate = Clock.CurrentTime + 100;
		}

		// Draw the display
		public override void OnRedrawDisplay()
		{
			base.OnRedrawDisplay();
			
			// Render the overlay
			UpdateOverlay();

			// Render lines and vertices
			if(renderer.StartPlotter(true))
			{
				renderer.PlotLinedefSet(General.Map.Map.Linedefs);
				renderer.PlotVerticesSet(General.Map.Map.Vertices);
				renderer.Finish();
			}
			renderer.Present();
		}

		// Processing
		public override void OnProcess(long deltatime)
		{
			base.OnProcess(deltatime);
			if(Clock.CurrentTime >= nextupdate)
			{
				// Get the processed points from the VPO manager
				List<PointData> points = new List<PointData>();
				int pointsleft = BuilderPlug.VPO.DequeueResults(points);

				// Queue more points if needed
				QueuePoints(pointsleft);

				// Apply the points to the tiles
				foreach(PointData pd in points)
				{
					Tile t;
					Point tp = TileForPoint(pd.point.x, pd.point.y);
					if(tiles.TryGetValue(tp, out t))
						t.StorePointData(pd);
				}

				// Redraw
				RedrawAllTiles();
				General.Interface.RedrawDisplay();

				nextupdate = Clock.CurrentTime + 500;
			}
			else
			{
				// Queue more points if needed
				QueuePoints(BuilderPlug.VPO.GetRemainingPoints());
			}
		}

		// LMB pressed
		protected override void OnSelectBegin()
		{
			StartMultiSelection();
			BuilderPlug.InterfaceForm.HideTooltip();
			base.OnSelectBegin();
		}

		// Multiselecting
		protected override void OnUpdateMultiSelection()
		{
			base.OnUpdateMultiSelection();
			
			UpdateOverlay();
			renderer.Present();
		}

		// Multiselect ends
		protected override void OnEndMultiSelection()
		{
			base.OnEndMultiSelection();
			if((selectionrect.Width < 64f) && (selectionrect.Height < 64f))
				selectionrect.Inflate(64f, 64f);
			base.CenterOnArea(selectionrect, 0.1f);
		}

		// Mouse moves
		public override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);

			if(!selecting)
			{
				int viewstats = (int)BuilderPlug.InterfaceForm.ViewStats;
				
				// Get the tile data for the current position
				Point tp = TileForPoint((float)mousemappos.x, (float)mousemappos.y);
				Tile t;
				if(tiles.TryGetValue(tp, out t))
				{
					int x = (int)Math.Floor(mousemappos.x) - t.Position.X;
					int y = (int)Math.Floor(mousemappos.y) - t.Position.Y;
					byte b = t.GetPointByte(x, y, viewstats);
					if(b != Tile.POINT_VOID_B)
					{
						// Setup hoverlabel
						int value = t.GetPointValue(x, y, viewstats);
						Point p = new Point((int)mousepos.x + 5, (int)mousepos.y + 5);
						string appendoverflow = (b == Tile.POINT_OVERFLOW_B) ? "+" : "";
						BuilderPlug.InterfaceForm.ShowTooltip(value + appendoverflow + " / " + StaticLimit(BuilderPlug.InterfaceForm.ViewStats), p);
					}
					else
					{
						// Void
						BuilderPlug.InterfaceForm.HideTooltip();
					}
				}
				else
				{
					BuilderPlug.InterfaceForm.HideTooltip();
				}
			}
		}

		// Mouse leaves
		public override void OnMouseLeave(EventArgs e)
		{
			base.OnMouseLeave(e);
			BuilderPlug.InterfaceForm.HideTooltip();
		}

		//mxd
		private void OnVisplaneSettingsChanged(object sender, EventArgs e)
		{
			// Restart processing 
			BuilderPlug.VPO.Stop();
			tiles.Clear();
			CreateTiles();
			BuilderPlug.VPO.Start(tempfile, General.Map.Options.LevelName);
			General.Interface.RedrawDisplay();
		}

		// Get the configured static limit for the given stat.
		private uint StaticLimit(ViewStats stat)
		{
			switch (stat)
			{
				case ViewStats.Visplanes:
					return General.Map.Config.StaticLimits.Visplanes;
				case ViewStats.Drawsegs:
					return General.Map.Config.StaticLimits.Drawsegs;
				case ViewStats.Solidsegs:
					return General.Map.Config.StaticLimits.Solidsegs;
				case ViewStats.Openings:
					return General.Map.Config.StaticLimits.Openings;
				default:
					return 0;
			}
		}
		#endregion
	}
}
