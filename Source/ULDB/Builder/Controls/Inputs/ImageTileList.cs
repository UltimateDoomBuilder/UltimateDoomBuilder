// ULDB: shows upstream's ImageSelectorPanel (the model of the texture and flat browsers' tile
// grid): tiles laid out as upstream's UpdateRectangles does, drawn with the preview image, name and
// size, with selection, double-click, cursor keys, scrolling and typing into the browser's filter.
// Lives inside a ScrollViewer; only the tiles in view are drawn.

using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CodeImp.DoomBuilder;
using CodeImp.DoomBuilder.Controls;
using CodeImp.DoomBuilder.Data;
using ULDB.Shell;
using WF = System.Windows.Forms;

namespace ULDB.Controls
{
	internal sealed class ImageTileList : Control
	{
		private const double FontHeight = 18; // name line
		private const double PadVert = 2;
		private static readonly Typeface Face = Typeface.Default;

		private sealed class Tile
		{
			public ImageBrowserItem Item;
			public Rect Rect;   // the picture, in content coordinates
			public Rect Cell;   // picture and name
		}

		private sealed class Header
		{
			public string Text;
			public double Y;
		}

		private readonly ImageSelectorPanel model;
		private readonly List<Tile> tiles = new List<Tile>();
		private readonly List<Header> headers = new List<Header>();
		private ScrollViewer scroller;
		private double contentheight;
		private double layoutwidth;
		private ImageBrowserItem lastclicked;
		private bool pending;

		public ImageTileList(ImageSelectorPanel model)
		{
			this.model = model;
			Focusable = true;
			ClipToBounds = true;
			model.ContentChanged += () => Schedule(true);
			model.Refreshed += (s, e) => Schedule(false);
			model.Redraw += () => Schedule(true);
			model.ScrollRequested += ScrollTo;
			model.JumpRequested += Jump;
			model.FocusRequested += () => Dispatcher.UIThread.Post(() => Focus(), DispatcherPriority.Input);
		}

		private void Schedule(bool relayout)
		{
			if(relayout) layoutvalid = false;
			if(pending) return;
			pending = true;
			Dispatcher.UIThread.Post(() =>
			{
				pending = false;
				if(!layoutvalid) InvalidateMeasure();
				InvalidateVisual();
			}, DispatcherPriority.Render);
		}

		private bool layoutvalid;

		protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
		{
			base.OnAttachedToVisualTree(e);
			scroller = this.FindAncestorOfType<ScrollViewer>();
			if(scroller != null) scroller.ScrollChanged += (s, a) => InvalidateVisual();
		}

		#region ================== Layout

		protected override Size MeasureOverride(Size available)
		{
			double w = double.IsInfinity(available.Width) ? 600 : available.Width;
			Layout(w);
			return new Size(w, contentheight);
		}

		private int ShownImageSize => model.ImageSize;

		private static Size PreviewSize(ImageBrowserItem item)
		{
			System.Drawing.Image img = item.Icon.GetPreview();
			return img == null ? new Size(64, 64) : new Size(img.Width, img.Height);
		}

		private double TitleHeight => model.ClassicView ? 0 : FontHeight + 6;

		private void Layout(double w)
		{
			layoutvalid = true;
			layoutwidth = w;
			tiles.Clear();
			headers.Clear();

			bool classic = model.ClassicView;
			double padhorz = classic ? 13 : 2;
			double cx = 0, cy = TitleHeight, my = 0;
			int imagesize = model.ImageSize;
			bool sizesbelow = General.Settings != null && General.Settings.ShowTextureSizes && General.Settings.TextureSizesBelow;

			IEnumerable<(string title, List<ImageBrowserItem> items)> groups = Groups();
			foreach((string title, List<ImageBrowserItem> items) group in groups)
			{
				if(classic && group.title != null)
				{
					cx = 0;
					cy += FontHeight + 6 + my + 16;
					my = 0;
					headers.Add(new Header { Text = group.title + ":", Y = cy - (FontHeight + 6) });
				}

				foreach(ImageBrowserItem ti in group.items)
				{
					Size preview = imagesize > 0 ? new Size(imagesize, imagesize) : PreviewSize(ti);
					double textwidth = Math.Max(ti.TextureNameWidth, 8);
					// With a set preview size every tile is the same width so the tiles line up in columns (issue #27);
					// a long name is trimmed to it. At the textures' own sizes a tile is as wide as its picture or name.
					double wid = (imagesize > 0 ? preview.Width : Math.Max(preview.Width, textwidth)) + padhorz * 2;
					double hei = preview.Height + PadVert * 2 + FontHeight;
					if(sizesbelow && ti.ItemType == ImageBrowserItemType.IMAGE) hei += FontHeight;

					if(w - cx < wid)
					{
						cx = 0;
						cy += my;
						my = 0;
					}

					my = Math.Max(my, hei);
					Rect rect = new Rect(cx + padhorz, cy + PadVert, wid - padhorz * 2, hei - PadVert * 2 - FontHeight);
					tiles.Add(new Tile { Item = ti, Rect = rect, Cell = new Rect(cx, cy, wid, hei) });
					cx += wid;
				}
			}

			contentheight = tiles.Count > 0 ? cy + my : 0;
		}

		private IEnumerable<(string, List<ImageBrowserItem>)> Groups()
		{
			if(model.ItemGroups.Count > 0)
			{
				foreach(ImageBrowserItemGroup g in model.ItemGroups) yield return (g.Title, g.Items);
			}
			else if(model.Items.Count > 0)
			{
				yield return (null, model.Items);
			}
		}

		#endregion

		#region ================== Drawing

		private Color AccentColor()
		{
			if(this.TryFindResource("SystemAccentColor", ActualThemeVariant, out object v) && v is Color c) return c;
			return Color.FromRgb(61, 174, 233);
		}

		private IBrush Fore()
		{
			if(FlatBackground() != null) return Brushes.White;
			return TextElement.GetForeground(this) ?? Brushes.Black;
		}

		private FormattedText Text(string text, IBrush brush, double size = 12, bool bold = false)
		{
			return new FormattedText(text ?? "", CultureInfo.CurrentUICulture, FlowDirection.LeftToRight,
				bold ? new Typeface(Face.FontFamily, FontStyle.Normal, FontWeight.Bold) : Face, size, brush);
		}

		/// <summary>
		/// ULDB's browser background choice (Preferences, "Texture browser background"; upstream issue #1216: white or black only
		/// hid dark textures): 0 follows the theme (and upstream's black option), 1 black, 2 dark grey, 3 grey, 4 checkerboard
		/// behind the pictures so transparent and black areas can be told apart.
		/// </summary>
		public static int BackgroundChoice => General.Settings != null ? General.Settings.ReadSetting("uldb.browserbackground", 0) : 0;

		/// <summary>The solid colour behind everything, or null to follow the theme.</summary>
		private static IBrush FlatBackground()
		{
			if(General.Settings != null && General.Settings.BlackBrowsers) return Brushes.Black;
			switch(BackgroundChoice)
			{
				case 1: return Brushes.Black;
				case 2: return new SolidColorBrush(Color.FromRgb(0x30, 0x30, 0x30));
				case 3: return new SolidColorBrush(Color.FromRgb(0x80, 0x80, 0x80));
				default: return null;
			}
		}

		private static readonly IBrush checklight = new SolidColorBrush(Color.FromRgb(0x9A, 0x9A, 0x9A));
		private static readonly IBrush checkdark = new SolidColorBrush(Color.FromRgb(0x6A, 0x6A, 0x6A));

		private static void DrawCheckerboard(DrawingContext context, Rect area)
		{
			const double cell = 8;
			using(context.PushClip(area))
			{
				context.FillRectangle(checklight, area);
				for(double y = 0; y < area.Height; y += cell)
					for(double x = ((int)(y / cell) % 2) * cell; x < area.Width; x += cell * 2)
						context.FillRectangle(checkdark, new Rect(area.X + x, area.Y + y, cell, cell));
			}
		}

		public override void Render(DrawingContext context)
		{
			if(!layoutvalid) Layout(layoutwidth > 0 ? layoutwidth : Bounds.Width);
			IBrush flat = FlatBackground();
			bool black = flat != null;
			bool dark = black || ActualThemeVariant == ThemeVariant.Dark;
			context.FillRectangle(flat ?? Brushes.Transparent, new Rect(Bounds.Size));
			bool checkerboard = BackgroundChoice == 4 && flat == null;

			double top = scroller?.Offset.Y ?? 0;
			double height = scroller?.Viewport.Height ?? Bounds.Height;
			if(height <= 0) height = Bounds.Height;
			Color accent = AccentColor();
			IBrush fore = Fore();
			bool classic = model.ClassicView;
			bool showsizes = General.Settings != null && General.Settings.ShowTextureSizes;
			bool sizesbelow = showsizes && General.Settings.TextureSizesBelow;
			IBrush usedbrush = new SolidColorBrush(Color.FromRgb(black ? (byte)0xFF : (byte)0x3D, black ? (byte)0xA5 : (byte)0xAE, black ? (byte)0 : (byte)0xE9));

			if(classic)
			{
				foreach(Header h in headers)
				{
					if(h.Y + FontHeight < top || h.Y > top + height) continue;
					context.DrawText(Text(h.Text, fore, 12, true), new Point(2, h.Y));
					context.DrawLine(new Pen(new SolidColorBrush(fore is ISolidColorBrush sb ? Color.FromArgb(0x7F, sb.Color.R, sb.Color.G, sb.Color.B) : Colors.Gray), 1), new Point(2, h.Y + FontHeight + 1), new Point(Bounds.Width - 4, h.Y + FontHeight + 1));
				}
			}

			foreach(Tile t in tiles)
			{
				if(t.Cell.Bottom < top) continue;
				if(t.Cell.Top > top + height) break;

				ImageBrowserItem item = t.Item;
				bool selected = model.SelectedItems.Contains(item);
				bool used = item.Icon.UsedInMap;
				Rect r = t.Rect;
				double pichight = r.Height;
				if(sizesbelow && item.ItemType == ImageBrowserItemType.IMAGE) pichight -= FontHeight;

				if(selected)
				{
					Rect bg = classic ? new Rect(r.X - 13, r.Y - 2, r.Width + 26, r.Height + FontHeight + 2) : new Rect(r.X - 2, r.Y - 2, r.Width + 4, r.Height + FontHeight + 4);
					context.DrawRectangle(new SolidColorBrush(Color.FromArgb(classic ? (byte)0xFF : (byte)0x55, accent.R, accent.G, accent.B)), null, bg, 3, 3);
				}

				// Picture, fitted into the tile, centred
				Rect pic = new Rect(r.X, r.Y, r.Width, Math.Max(1, pichight));
				bool folder = item.ItemType != ImageBrowserItemType.IMAGE;
				if(folder) DrawFolder(context, pic, item.ItemType == ImageBrowserItemType.FOLDER_UP, accent);
				Bitmap bmp = folder ? null : AvaloniaImages.From(item.Icon.GetPreview());
				if(checkerboard && !folder) DrawCheckerboard(context, pic);
				if(bmp != null)
				{
					double scale = Math.Min(pic.Width / bmp.Size.Width, pic.Height / bmp.Size.Height);
					if(item.ItemType != ImageBrowserItemType.IMAGE || model.ImageSize == 0) scale = Math.Min(scale, 1);
					Size drawn = new Size(Math.Max(1, bmp.Size.Width * scale), Math.Max(1, bmp.Size.Height * scale));
					Rect dest = new Rect(pic.X + (pic.Width - drawn.Width) / 2, pic.Y + (pic.Height - drawn.Height) / 2, drawn.Width, drawn.Height);
					using(context.PushRenderOptions(new RenderOptions { BitmapInterpolationMode = BitmapInterpolationMode.None }))
						context.DrawImage(bmp, dest);
				}

				// Frame
				if(!classic && item.ItemType == ImageBrowserItemType.IMAGE)
					context.DrawRectangle(null, new Pen(selected ? new SolidColorBrush(accent) : (used ? usedbrush : new SolidColorBrush(Color.FromArgb(0x60, 0x80, 0x80, 0x80))), selected ? 2 : 1), pic.Inflate(1));

				// Name
				IBrush namebrush = used ? usedbrush : fore;
				FormattedText name = Text(item.TextureName, namebrush, 12);
				if(model.ImageSize > 0 && !classic) { name.MaxTextWidth = Math.Max(8, r.Width + 2); name.MaxLineCount = 1; name.Trimming = TextTrimming.CharacterEllipsis; }
				double nx = classic ? r.X + (r.Width - name.Width) / 2 : r.X - 2;
				context.DrawText(name, new Point(nx, r.Y + pic.Height + 2));

				// Size
				if(showsizes && item.IsPreviewLoaded && item.ItemType == ImageBrowserItemType.IMAGE)
				{
					string size = Math.Abs(item.Icon.ScaledWidth) + "x" + Math.Abs(item.Icon.ScaledHeight);
					if(sizesbelow)
					{
						FormattedText f = Text("(" + size + ")", namebrush, 11);
						context.DrawText(f, new Point(classic ? r.X + (r.Width - f.Width) / 2 : r.X, r.Y + pic.Height + FontHeight));
					}
					else
					{
						FormattedText f = Text(size, namebrush, 11);
						context.DrawRectangle(new SolidColorBrush(Color.FromArgb(0xB0, dark ? (byte)0 : (byte)0xFF, dark ? (byte)0 : (byte)0xFF, dark ? (byte)0 : (byte)0xFF)), null, new Rect(r.X, r.Y, f.Width + 4, f.Height));
						context.DrawText(f, new Point(r.X + 2, r.Y));
					}
				}
			}

			// Title across the top (a bar; classic view has a heading per group instead)
			if(!classic && !string.IsNullOrEmpty(model.Title))
			{
				context.DrawRectangle(new SolidColorBrush(Color.FromArgb(0xE0, dark ? (byte)0x30 : (byte)0xE8, dark ? (byte)0x30 : (byte)0xE8, dark ? (byte)0x30 : (byte)0xE8)), null, new Rect(0, top, Bounds.Width, FontHeight + 2));
				context.DrawText(Text(model.Title, fore, 12, true), new Point(4, top + 1));
			}
		}

		#endregion

		// Folder tiles (a texture set or "go up") are vector icons in the Breeze style, not UDB's bitmaps.
		private static readonly Geometry FolderBack = Geometry.Parse("M1,3.5 Q1,2.5 2,2.5 H6 L8,4.5 H14 Q15,4.5 15,5.5 V13 Q15,14 14,14 H2 Q1,14 1,13 Z");
		private static readonly Geometry FolderFront = Geometry.Parse("M1,6.5 Q1,5.5 2,5.5 H14 Q15,5.5 15,6.5 V13 Q15,14 14,14 H2 Q1,14 1,13 Z");
		private static readonly Geometry UpArrow = Geometry.Parse("M8,12 V8 M5.8,9.9 L8,7.7 L10.2,9.9");

		private static void DrawFolder(DrawingContext context, Rect box, bool up, Color accent)
		{
			double size = Math.Min(Math.Min(box.Width, box.Height) * 0.62, 96);
			if(size < 16) size = Math.Min(box.Width, box.Height);
			double scale = size / 16;
			Matrix m = Matrix.CreateScale(scale, scale) * Matrix.CreateTranslation(box.X + (box.Width - size) / 2, box.Y + (box.Height - size) / 2);
			using(context.PushTransform(m))
			{
				Color dark = Color.FromRgb((byte)(accent.R * 0.7), (byte)(accent.G * 0.7), (byte)(accent.B * 0.7));
				context.DrawGeometry(new SolidColorBrush(dark), null, FolderBack);
				context.DrawGeometry(new SolidColorBrush(accent), null, FolderFront);
				if(up) context.DrawGeometry(null, new Pen(Brushes.White, 1.3, lineCap: PenLineCap.Round, lineJoin: PenLineJoin.Round), UpArrow);
			}
		}

		#region ================== Scrolling

		private void ScrollTo(ImageBrowserItem item, bool center)
		{
			if(scroller == null) return;
			if(!layoutvalid) { Layout(layoutwidth > 0 ? layoutwidth : Bounds.Width); }
			Tile t = tiles.Find(x => x.Item == item);
			if(t == null) return;
			double viewport = scroller.Viewport.Height > 0 ? scroller.Viewport.Height : Bounds.Height;
			double min = scroller.Offset.Y;
			if(t.Cell.Top - 3 >= min + TitleHeight && t.Cell.Bottom + 3 <= min + viewport) return;
			double y = center ? t.Cell.Top + t.Cell.Height / 2 - viewport / 2 : t.Cell.Top - TitleHeight - 3;
			scroller.Offset = new Vector(0, Math.Max(0, Math.Min(y, Math.Max(0, contentheight - viewport))));
			InvalidateVisual();
		}

		private void Jump(ImageBrowserItem item)
		{
			// The item exists twice (used textures first, then in the pile): go to the other copy.
			if(scroller == null) return;
			int first = tiles.FindIndex(x => x.Item == item), last = tiles.FindLastIndex(x => x.Item == item);
			if(first < 0 || first == last) return;
			double viewport = scroller.Viewport.Height;
			double middle = scroller.Offset.Y + viewport / 2;
			Rect a = tiles[first].Cell, b = tiles[last].Cell;
			bool toa = Math.Abs(a.Bottom - middle) > Math.Abs(b.Top - middle);
			Rect target = toa ? a : b;
			double y = model.CenterItem ? target.Top + target.Height / 2 - viewport / 2 : target.Top;
			scroller.Offset = new Vector(0, Math.Max(0, Math.Min(y, Math.Max(0, contentheight - viewport))));
			InvalidateVisual();
		}

		#endregion

		#region ================== Input

		private int TileAt(Point p)
		{
			double padhorz = model.ClassicView ? 13 : 2;
			for(int i = 0; i < tiles.Count; i++)
			{
				Rect r = tiles[i].Rect;
				if(r.X - padhorz <= p.X && r.Right + padhorz >= p.X && r.Y - PadVert <= p.Y && r.Bottom + PadVert + FontHeight >= p.Y) return i;
			}
			return -1;
		}

		protected override void OnPointerPressed(PointerPressedEventArgs e)
		{
			base.OnPointerPressed(e);
			Focus();
			if(!e.GetCurrentPoint(this).Properties.IsLeftButtonPressed) return;
			int index = TileAt(e.GetPosition(this));
			bool ctrl = (e.KeyModifiers & KeyModifiers.Control) != 0, shift = (e.KeyModifiers & KeyModifiers.Shift) != 0;

			if(index < 0)
			{
				if(model.SelectedItems.Count > 0) model.ClearSelection();
				return;
			}

			ImageBrowserItem item = tiles[index].Item;
			if(e.ClickCount >= 2 && !ctrl && !shift && model.SelectedItems.Count == 1)
			{
				model.DoubleClick(item);
				return;
			}

			if(shift && model.MultiSelect && lastclicked != null)
			{
				int a = model.Items.IndexOf(lastclicked), b = model.Items.IndexOf(item);
				model.SetSelectedItems(model.Items.GetRange(Math.Min(a, b), Math.Abs(a - b) + 1));
				return;
			}

			model.Select(item, ctrl);
			lastclicked = item;
			InvalidateVisual();
			e.Handled = true;
		}

		protected override void OnPointerMoved(PointerEventArgs e)
		{
			base.OnPointerMoved(e);
			int index = TileAt(e.GetPosition(this));
			string tip = index >= 0 && tiles[index].Item.ItemType == ImageBrowserItemType.IMAGE ? tiles[index].Item.ToolTip : null;
			ToolTip.SetTip(this, string.IsNullOrEmpty(tip) ? null : tip);
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);
			switch(e.Key)
			{
				case Key.Left: Step(-1, 0); break;
				case Key.Right: Step(1, 0); break;
				case Key.Up: Step(0, -1); break;
				case Key.Down: Step(0, 1); break;
				case Key.PageDown: Scroll(scroller?.Viewport.Height ?? 0); break;
				case Key.PageUp: Scroll(-(scroller?.Viewport.Height ?? 0)); break;
				case Key.Home: Scroll(-contentheight); break;
				case Key.End: Scroll(contentheight); break;
				case Key.Enter:
					if(model.SelectedItems.Count > 0) model.DoubleClick(model.SelectedItems[0]);
					break;
				case Key.Tab:
					if(model.SelectedItems.Count > 0) model.JumpBetweenItems(model.SelectedItems[0]);
					break;
				case Key.Back:
					model.RaiseKeyPress(new WF.KeyPressEventArgs((char)((e.KeyModifiers & KeyModifiers.Control) != 0 ? 127 : 8)));
					break;
				default: return;
			}
			e.Handled = true;
		}

		protected override void OnTextInput(TextInputEventArgs e)
		{
			base.OnTextInput(e);
			// Typing filters the list: upstream's KeyPress handler moves the characters to the filter box.
			foreach(char c in e.Text ?? "") model.RaiseKeyPress(new WF.KeyPressEventArgs(char.ToLowerInvariant(c)));
			e.Handled = true;
		}

		private void Scroll(double amount)
		{
			if(scroller == null) return;
			double max = Math.Max(0, contentheight - scroller.Viewport.Height);
			scroller.Offset = new Vector(0, Math.Max(0, Math.Min(max, scroller.Offset.Y + amount)));
		}

		// Cursor keys: left/right by tile, up/down to the nearest tile in the row above or below.
		private void Step(int dx, int dy)
		{
			if(model.HideSelection) return;
			if(model.SelectedItems.Count == 0)
			{
				if(model.Items.Count > 0) model.SetSelectedItem(model.Items[0]);
				return;
			}

			int current = tiles.FindIndex(t => t.Item == model.SelectedItems[0]);
			if(current < 0) return;
			int target = -1;
			if(dy == 0)
			{
				target = current + dx;
			}
			else
			{
				double cy = tiles[current].Cell.Y, cxm = tiles[current].Cell.X + tiles[current].Cell.Width / 2;
				int i = current;
				do { i += dy; } while(i >= 0 && i < tiles.Count && tiles[i].Cell.Y == cy);
				if(i >= 0 && i < tiles.Count)
				{
					double row = tiles[i].Cell.Y, best = double.MaxValue;
					for(; i >= 0 && i < tiles.Count && tiles[i].Cell.Y == row; i += dy)
					{
						double d = Math.Abs(tiles[i].Cell.X + tiles[i].Cell.Width / 2 - cxm);
						if(d < best) { best = d; target = i; }
					}
				}
			}
			if(target >= 0 && target < tiles.Count) model.SetSelectedItem(tiles[target].Item);
		}

		#endregion
	}
}
