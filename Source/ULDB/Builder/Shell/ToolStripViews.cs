// ULDB: shows upstream's menus, toolbars and status bars (ULDB.Compat ToolStrip models)
// as Avalonia controls, and keeps them in step: item changes (text, image, checked,
// visible, enabled) update the control, collection changes rebuild, and user actions
// go back to the model (PerformClick, drop-down events), where upstream's handlers run.

using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;
using WF = System.Windows.Forms;

namespace ULDB.Shell
{
	internal static class ToolStripViews
	{
		public const double ICON_SIZE = 16;

		#region ================== Menus

		/// <summary>Builds a menu bar from a MenuStrip and keeps it in step.</summary>
		public static Menu CreateMenuBar(WF.ToolStrip strip)
		{
			Menu menu = new Menu();
			Bind(strip.Items, menu.Items);
			return menu;
		}

		#region ================== Context menus

		private static readonly Dictionary<WF.ToolStripDropDown, ContextMenu> openmenus = new Dictionary<WF.ToolStripDropDown, ContextMenu>();

		/// <summary>
		/// Upstream's ContextMenuStrip.Show(...) opens an Avalonia context menu at the pointer (WinForms
		/// callers pass the pointer position too), with the same items, events and closing.
		/// </summary>
		internal static void InstallContextMenus()
		{
			WF.ToolStripDropDown.ShowHandler = (menu, control, position) => Post(() => ShowContextMenu(menu));
			WF.ToolStripDropDown.CloseHandler = menu => Post(() => { if(openmenus.TryGetValue(menu, out ContextMenu cm)) cm.Close(); });
		}

		/// <summary>
		/// An Avalonia context menu for a control's ContextMenuStrip (shown on right-click): upstream's
		/// Opening handler runs first, so it can enable and disable items.
		/// </summary>
		public static ContextMenu CreateContextMenu(WF.ToolStripDropDown menu)
		{
			ContextMenu cm = new ContextMenu();
			cm.Opening += (s, e) => { if(!menu.RaiseOpening()) e.Cancel = true; };
			cm.Opened += (s, e) => menu.RaiseOpened();
			cm.Closed += (s, e) => menu.RaiseClosed(WF.ToolStripDropDownCloseReason.CloseCalled);
			foreach(WF.ToolStripItem item in menu.Items) cm.Items.Add(CreateMenuEntry(item));
			return cm;
		}

		private static void ShowContextMenu(WF.ToolStripDropDown menu)
		{
			if(!menu.RaiseOpening()) return;
			var lifetime = Avalonia.Application.Current?.ApplicationLifetime as Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime;
			Window owner = lifetime?.Windows.FirstOrDefault(w => w.IsActive) ?? lifetime?.MainWindow;
			if(owner == null) return;

			ContextMenu cm = new ContextMenu { Placement = PlacementMode.Pointer };
			foreach(WF.ToolStripItem item in menu.Items) cm.Items.Add(CreateMenuEntry(item));
			cm.Opened += (s, e) => menu.RaiseOpened();
			cm.Closed += (s, e) =>
			{
				openmenus.Remove(menu);
				menu.RaiseClosed(WF.ToolStripDropDownCloseReason.CloseCalled);
			};
			openmenus[menu] = cm;
			cm.Open(owner);
		}

		#endregion

		/// <summary>Fills (and keeps filling) an Avalonia item list from a ToolStrip item collection.</summary>
		private static void Bind(WF.ToolStripItemCollection source, ItemCollection target)
		{
			void Rebuild()
			{
				target.Clear();
				foreach(WF.ToolStripItem item in source) target.Add(CreateMenuEntry(item));
			}

			Rebuild();
			source.Changed += (s, e) => Post(Rebuild);
		}

		/// <summary>Menu items whose action no longer exists upstream (clicking them does nothing; upstream issue #1315).</summary>
		private static readonly HashSet<string> DeadActions = new HashSet<string> { "thingsselectinsectors" };

		private static Control CreateMenuEntry(WF.ToolStripItem item)
		{
			if(item is WF.ToolStripSeparator)
			{
				Separator sep = new Separator();
				Follow(item, () => sep.IsVisible = item.Available);
				return sep;
			}

			MenuItem mi = new MenuItem();
			Follow(item, () => UpdateMenuItem(mi, item));

			if(item is WF.ToolStripDropDownItem dd)
			{
				Bind(dd.DropDownItems, mi.Items);
				mi.SubmenuOpened += (s, e) =>
				{
					if(e.Source != mi) return; // Bubbled up from a nested submenu
					dd.RaiseDropDownOpening();
					dd.DropDown.RaiseOpening();
					dd.RaiseDropDownOpened();
					dd.DropDown.RaiseOpened();
				};
				mi.PropertyChanged += (s, e) =>
				{
					if(e.Property != MenuItem.IsSubMenuOpenProperty || (bool)e.NewValue) return;
					dd.RaiseDropDownClosed();
					dd.DropDown.RaiseClosed(WF.ToolStripDropDownCloseReason.CloseCalled);
				};
			}

			mi.Click += (s, e) =>
			{
				if(e.Source != mi) return;
				if(item is WF.ToolStripDropDownItem d && d.HasDropDownItems) return; // Opening a submenu
				e.Handled = true;
				item.PerformClick();
				(item.Owner as WF.ToolStripDropDown)?.OwnerItem?.Owner?.RaiseItemClicked(item);
				UpdateMenuItem(mi, item); // Checked may have changed
			};
			return mi;
		}

		private static void UpdateMenuItem(MenuItem mi, WF.ToolStripItem item)
		{
			mi.Header = MenuText(item.Text);
			mi.IsVisible = item.Available && !(item.Tag is string tag && DeadActions.Contains(tag));
			mi.IsEnabled = item.Enabled;
			mi.Icon = IconFor(item);
			ToolTip.SetTip(mi, string.IsNullOrEmpty(item.ToolTipText) || item.ToolTipText == item.Text ? null : item.ToolTipText);

			if(item is WF.ToolStripMenuItem m)
			{
				if(m.CheckOnClick || m.Checked) mi.ToggleType = MenuItemToggleType.CheckBox;
				mi.IsChecked = m.Checked;
				mi.InputGesture = Gesture(m);
			}
		}

		// UDB shows each action's current shortcut (ShortcutKeyDisplayString, e.g. "Ctrl+S").
		private static KeyGesture Gesture(WF.ToolStripMenuItem m)
		{
			string desc = m.ShortcutKeyDisplayString;
			if(string.IsNullOrEmpty(desc) && m.ShortcutKeys != WF.Keys.None)
				desc = Describe(m.ShortcutKeys);
			if(string.IsNullOrEmpty(desc) || !m.ShowShortcutKeys) return null;
			try { return KeyGesture.Parse(desc.Replace(" ", "")); }
			catch(Exception) { return null; } // Mouse buttons and the like have no gesture
		}

		private static string Describe(WF.Keys keys)
		{
			string s = "";
			if((keys & WF.Keys.Control) != 0) s += "Ctrl+";
			if((keys & WF.Keys.Shift) != 0) s += "Shift+";
			if((keys & WF.Keys.Alt) != 0) s += "Alt+";
			return s + (keys & WF.Keys.KeyCode);
		}

		/// <summary>WinForms mnemonics ("&amp;File", "&amp;&amp;" for a literal ampersand) to Avalonia's ("_File").</summary>
		public static string MenuText(string text)
		{
			if(string.IsNullOrEmpty(text)) return "";
			System.Text.StringBuilder sb = new System.Text.StringBuilder(text.Length + 4);
			for(int i = 0; i < text.Length; i++)
			{
				char c = text[i];
				if(c == '_') sb.Append("__");
				else if(c == '&' && i + 1 < text.Length && text[i + 1] == '&') { sb.Append('&'); i++; }
				else if(c == '&') sb.Append('_');
				else sb.Append(c);
			}
			return sb.ToString();
		}

		/// <summary>A popup menu (MenuFlyout) for a drop-down's items.</summary>
		public static MenuFlyout CreateFlyout(WF.ToolStripDropDownItem dd)
		{
			MenuFlyout flyout = new MenuFlyout();
			void Rebuild()
			{
				flyout.Items.Clear();
				foreach(WF.ToolStripItem item in dd.DropDownItems) flyout.Items.Add(CreateMenuEntry(item));
			}
			Rebuild();
			dd.DropDownItems.Changed += (s, e) => Post(Rebuild);
			flyout.Opening += (s, e) =>
			{
				dd.RaiseDropDownOpening();
				dd.DropDown.RaiseOpening();
			};
			flyout.Opened += (s, e) => { dd.RaiseDropDownOpened(); dd.DropDown.RaiseOpened(); };
			flyout.Closed += (s, e) => { dd.RaiseDropDownClosed(); dd.DropDown.RaiseClosed(WF.ToolStripDropDownCloseReason.CloseCalled); };
			return flyout;
		}

		#endregion

		#region ================== Toolbars and status bars

		/// <summary>
		/// Builds a toolbar (or status bar) from a ToolStrip. Left-aligned items come first,
		/// right-aligned ones last; a Spring status label takes the space in between.
		/// </summary>
		public static Control CreateToolBar(WF.ToolStrip strip, Orientation orientation)
		{
			// Toolbars show what fits and move the rest into an overflow drop-down (the status bar lays itself out).
			if(!(strip is WF.StatusStrip))
			{
				OverflowToolPanel bar = new OverflowToolPanel(orientation, item => CreateToolItem(item, orientation));
				bar.Classes.Add("uldb-toolbar");
				void Fill()
				{
					List<(Control, WF.ToolStripItem)> items = new List<(Control, WF.ToolStripItem)>();
					foreach(WF.ToolStripItem item in strip.Items) items.Add((CreateToolItem(item, orientation), item));
					bar.SetItems(items);
				}
				Fill();
				strip.Items.Changed += (s, e) => Post(Fill);
				return bar;
			}

			Panel host = orientation == Orientation.Vertical ? new StackPanel { Orientation = Orientation.Vertical, Spacing = 1 } : new Grid();
			host.Classes.Add("uldb-toolbar");

			void Rebuild()
			{
				host.Children.Clear();
				if(host is Grid grid)
				{
					grid.ColumnDefinitions.Clear();
					List<(Control control, WF.ToolStripItem item)> left = new(), right = new();
					foreach(WF.ToolStripItem item in strip.Items)
						(item.Alignment == WF.ToolStripItemAlignment.Right ? right : left).Add((CreateToolItem(item, orientation), item));
					right.Reverse(); // WinForms lays right-aligned items out from the right edge

					bool spring = false;
					foreach(var (control, item) in left)
					{
						bool fill = item is WF.ToolStripStatusLabel sl && sl.Spring;
						spring |= fill;
						Add(grid, control, fill ? GridLength.Star : GridLength.Auto);
					}
					if(!spring) Add(grid, new Panel(), GridLength.Star);
					foreach(var (control, item) in right) Add(grid, control, GridLength.Auto);
				}
				else
				{
					foreach(WF.ToolStripItem item in strip.Items) host.Children.Add(CreateToolItem(item, orientation));
				}
			}

			Rebuild();
			strip.Items.Changed += (s, e) => Post(Rebuild);
			return host;
		}

		private static void Add(Grid grid, Control control, GridLength width)
		{
			grid.ColumnDefinitions.Add(new ColumnDefinition(width));
			Grid.SetColumn(control, grid.ColumnDefinitions.Count - 1);
			grid.Children.Add(control);
		}

		private static Control CreateToolItem(WF.ToolStripItem item, Orientation orientation)
		{
			switch(item)
			{
				case WF.ToolStripSeparator _:
				{
					Border sep = new Border
					{
						Width = orientation == Orientation.Horizontal ? 1 : double.NaN,
						Height = orientation == Orientation.Vertical ? 1 : double.NaN,
						// Wider gaps and inset ends, so groups of buttons read as groups
						Margin = orientation == Orientation.Horizontal ? new Thickness(5, 6) : new Thickness(6, 2),
					};
					sep.Classes.Add("uldb-separator");
					Follow(item, () => sep.IsVisible = item.Available);
					return sep;
				}

				case WF.ToolStripSplitButton split:
				{
					SplitButton b = new SplitButton { Flyout = CreateFlyout(split) };
					b.Classes.Add("uldb-tool");
					b.Click += (s, e) => split.PerformButtonClick();
					Follow(item, () => UpdateButton(b, item));
					return b;
				}

				case WF.ToolStripDropDownButton ddb:
				{
					DropDownButton b = new DropDownButton { Flyout = CreateFlyout(ddb) };
					b.Classes.Add("uldb-tool");
					Follow(item, () => UpdateButton(b, item));
					return b;
				}

				case WF.ToolStripButton button:
				{
					ToggleButton b = new ToggleButton();
					b.Classes.Add("uldb-tool");
					b.Click += (s, e) =>
					{
						item.PerformClick();
						b.IsChecked = button.Checked; // The model decides
					};
					Follow(item, () => { UpdateButton(b, item); b.IsChecked = button.Checked; });
					return b;
				}

				case WF.ToolStripComboBox combo:
				{
					ComboBox cb = new ComboBox { MinWidth = Math.Max(60, combo.Width), VerticalAlignment = VerticalAlignment.Center };
					cb.Classes.Add("uldb-tool");
					bool updating = false;
					void Refresh()
					{
						updating = true;
						cb.ItemsSource = new List<object>(combo.Items);
						cb.SelectedIndex = combo.SelectedIndex;
						cb.IsEnabled = combo.Enabled;
						cb.IsVisible = combo.Available;
						updating = false;
					}
					cb.SelectionChanged += (s, e) => { if(!updating) combo.SelectedIndex = cb.SelectedIndex; };
					cb.DropDownOpened += (s, e) => Refresh(); // Upstream fills items without notifying
					Follow(item, Refresh);
					combo.SelectedIndexChanged += (s, e) => Post(Refresh);
					return cb;
				}

				case WF.ToolStripControlHost host when host.Control is WF.NumericUpDown nud:
				{
					NumericUpDown n = new NumericUpDown
					{
						Minimum = nud.Minimum, Maximum = nud.Maximum, Increment = nud.Increment,
						Value = nud.Value, Width = Math.Max(70, host.Width), VerticalAlignment = VerticalAlignment.Center,
						FormatString = "F" + nud.DecimalPlaces,
					};
					bool updating = false;
					n.ValueChanged += (s, e) =>
					{
						if(updating || n.Value == null) return;
						decimal v = Math.Clamp(n.Value.Value, nud.Minimum, nud.Maximum);
						if(v != nud.Value) nud.Value = v;
					};
					nud.ValueChanged += (s, e) => Post(() => { updating = true; n.Value = nud.Value; updating = false; });
					Follow(item, () => { n.IsEnabled = item.Enabled; n.IsVisible = item.Available; n.Minimum = nud.Minimum; n.Maximum = nud.Maximum; });
					ToolTip.SetTip(n, item.ToolTipText);
					return n;
				}

				case WF.ToolStripControlHost host when host.Control is WF.CheckBox wcb:
				{
					CheckBox c = new CheckBox { Content = wcb.Text, IsChecked = wcb.Checked, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(4, 0) };
					c.IsCheckedChanged += (s, e) => { if(c.IsChecked != wcb.Checked) wcb.Checked = c.IsChecked == true; };
					wcb.CheckedChanged += (s, e) => Post(() => c.IsChecked = wcb.Checked);
					Follow(item, () => { c.Content = wcb.Text; c.IsEnabled = item.Enabled; c.IsVisible = item.Available; });
					return c;
				}

				case WF.ToolStripTextBox tb:
				{
					TextBox t = new TextBox { Width = Math.Max(80, tb.Width), VerticalAlignment = VerticalAlignment.Center };
					if(item is CodeImp.DoomBuilder.Controls.PlaceholderToolStripTextBox ph) t.Watermark = ph.PlaceholderText;
					bool updating = false;
					t.TextChanged += (s, e) => { if(!updating) tb.Text = t.Text ?? ""; };
					Follow(item, () => { updating = true; if(t.Text != tb.Text) t.Text = tb.Text; updating = false; t.IsEnabled = item.Enabled; t.IsVisible = item.Available; });
					return t;
				}

				default: // Labels, status labels and anything else: text and/or image
				{
					StackPanel p = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 4, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(4, 0) };
					p.Classes.Add("uldb-label");
					ContentControl img = new ContentControl { Width = ICON_SIZE, Height = ICON_SIZE };
					TextBlock tb = new TextBlock { VerticalAlignment = VerticalAlignment.Center, TextTrimming = TextTrimming.CharacterEllipsis };
					// Numbers that change while the mouse moves (coordinates, zoom) keep a fixed width and
					// even digit widths, so the status bar doesn't jitter
					if(item.Name == "xposlabel" || item.Name == "yposlabel" || item.Name == "zoomlabel")
					{
						tb.MinWidth = item.Name == "zoomlabel" ? 34 : 46;
						tb.TextAlignment = TextAlignment.Right;
						tb.FontFeatures = FontFeatureCollection.Parse("tnum");
					}
					p.Children.Add(img);
					p.Children.Add(tb);
					p.PointerPressed += (s, e) => item.PerformClick();
					Follow(item, () =>
					{
						img.Content = item.Image != null ? IconFor(item) : null;
						img.IsVisible = img.Content != null && item.DisplayStyle != WF.ToolStripItemDisplayStyle.Text;
						tb.Text = item.DisplayStyle == WF.ToolStripItemDisplayStyle.Image ? "" : item.Text;
						if(item.ForeColor.A > 0 && !item.ForeColor.IsSystemColor)
							tb.Foreground = new SolidColorBrush(Color.FromArgb(item.ForeColor.A, item.ForeColor.R, item.ForeColor.G, item.ForeColor.B));
						p.IsVisible = item.Available;
						p.IsEnabled = item.Enabled;
						ToolTip.SetTip(p, string.IsNullOrEmpty(item.ToolTipText) || item.ToolTipText == item.Text ? null : item.ToolTipText);
					});
					return p;
				}
			}
		}

		private static void UpdateButton(ContentControl b, WF.ToolStripItem item)
		{
			bool showimage = item.Image != null && item.DisplayStyle != WF.ToolStripItemDisplayStyle.Text && item.DisplayStyle != WF.ToolStripItemDisplayStyle.None;
			bool showtext = !string.IsNullOrEmpty(item.Text) && (item.DisplayStyle == WF.ToolStripItemDisplayStyle.Text || item.DisplayStyle == WF.ToolStripItemDisplayStyle.ImageAndText || !showimage);
			if(showimage && !showtext) b.Content = IconFor(item);
			else if(!showimage) b.Content = item.Text;
			else
			{
				StackPanel p = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 4 };
				p.Children.Add(IconFor(item));
				p.Children.Add(new TextBlock { Text = item.Text, VerticalAlignment = VerticalAlignment.Center });
				b.Content = p;
			}
			b.IsVisible = item.Available;
			b.IsEnabled = item.Enabled;
			string tip = item.ToolTipText;
			ToolTip.SetTip(b, string.IsNullOrEmpty(tip) ? (showtext ? null : item.Text) : tip);
		}

		#endregion

		#region ================== Helpers

		/// <summary>Whether the item shows an icon (development aid: ULDB_DUMP_TOOLBARS lists the ones that don't).</summary>
		internal static bool HasIcon(WF.ToolStripItem item) => IconFor(item) != null;

		/// <summary>The item's icon: ULDB's Breeze-style icon for its action (Tag), name or resource image, else upstream's bitmap.</summary>
		private static Control IconFor(WF.ToolStripItem item)
		{
			string key = IconKeyProvider?.Invoke(item);
			return (key != null ? BreezeIcons.Create(key, ICON_SIZE) : null)
				?? BreezeIcons.Create(item.Tag, ICON_SIZE) ?? BreezeIcons.Create(item.Name, ICON_SIZE)
				?? BreezeIcons.ForImage(item.Image, ICON_SIZE) ?? IconFor(item.Image);
		}

		/// <summary>Set by the main window: an icon key for items whose icon shows a state (status bar), or null.</summary>
		public static Func<WF.ToolStripItem, string> IconKeyProvider { get; set; }

		private static Image IconFor(System.Drawing.Image image)
		{
			if(image == null) return null;
			Image img = new Image { Source = AvaloniaImages.From(image), Width = ICON_SIZE, Height = ICON_SIZE };
			RenderOptions.SetBitmapInterpolationMode(img, Avalonia.Media.Imaging.BitmapInterpolationMode.HighQuality);
			return img;
		}

		/// <summary>Runs update now and whenever the item changes (on the UI thread).</summary>
		private static void Follow(WF.ToolStripItem item, Action update)
		{
			update();
			item.Changed += (s, e) => Post(update);
		}

		// Model changes can come from any thread, and in bursts: coalesce onto the UI thread.
		private static void Post(Action a)
		{
			if(Dispatcher.UIThread.CheckAccess()) a();
			else Dispatcher.UIThread.Post(a);
		}

		#endregion
	}
}
