// ULDB: the Avalonia view of upstream's MainForm. MainForm's own code (Windows/MainForm.cs
// and its designer file) runs unchanged and builds a model of menus, toolbars, status bar
// and display; this window lays them out like UDB and routes input into MainForm's handlers.
//
// Layout (as MainForm.Designer.cs docks it): menu bar and mode controls on one row, the
// main toolbar below; the edit-modes toolbar on the left; the map view in the middle;
// the info panel and status bar at the bottom.

using System;
using System.Linq;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CodeImp.DoomBuilder;
using CodeImp.DoomBuilder.Actions;
using CodeImp.DoomBuilder.Controls;
using CodeImp.DoomBuilder.Windows;
using ULDB.Input;
using WF = System.Windows.Forms;

namespace ULDB.Shell
{
	internal sealed class MainWindow : Window
	{
		private readonly MainForm form;
		private readonly RenderTargetControl display;
		private readonly ViewportControl viewport;
		private Panel input;
		private WF.Keys modifiers;
		private Point lastpointer;
		private bool closing;

		public MainWindow(MainForm form)
		{
			this.form = form;
			display = form.Display;

			Title = form.Text;
			Width = 1280;
			Height = 860;
			MinWidth = 640;
			MinHeight = 480;
			WindowStartupLocation = WindowStartupLocation.CenterScreen;
			// Development aid: ULDB_SIZE=<width>x<height> for the start size (checking layouts in small windows).
			string devsize = Environment.GetEnvironmentVariable("ULDB_SIZE");
			if(devsize != null && devsize.Split('x') is string[] wh && wh.Length == 2 && double.TryParse(wh[0], out double dw) && double.TryParse(wh[1], out double dh)) { Width = dw; Height = dh; }
			form.TextChanged += (s, e) => Dispatcher.UIThread.Post(() => Title = form.Text);

			// Top: menu bar with the mode's own controls beside it, then the main toolbar.
			Grid menurow = new Grid { ColumnDefinitions = new ColumnDefinitions("Auto,*") };
			// Help menu: no link to UDB's help file for now (it isn't ours), no update check (UDB's updater
			// isn't part of ULDB); the issues item goes to ULDB's repository (MainForm, #if ULDB).
			Field<WF.ToolStripItem>("itemhelprefmanual").Available = false;
			Field<WF.ToolStripItem>("itemhelpcheckupdates").Available = false;
			Field<WF.ToolStripItem>("itemhelpissues").Text = "&" + Branding.SHORT_NAME + " on GitHub";
			Field<WF.ToolStripItem>("itemhelpabout").Text = "&About " + Branding.NAME + "...";
			Menu menu = ToolStripViews.CreateMenuBar(Field<WF.MenuStrip>("menumain"));
			Control modecontrols = ToolStripViews.CreateToolBar(Field<WF.ToolStrip>("modecontrolsloolbar"), Orientation.Horizontal);
			Grid.SetColumn(modecontrols, 1);
			menurow.Children.Add(menu);
			menurow.Children.Add(modecontrols);
			Border menuborder = new Border { Child = menurow };
			menuborder.Classes.Add("uldb-bar");

			Control toolbar = ToolStripViews.CreateToolBar(Field<WF.ToolStrip>("toolbar"), Orientation.Horizontal);
			Border toolbarborder = new Border { Child = toolbar, Padding = new Thickness(2, 1) };
			toolbarborder.Classes.Add("uldb-bar");

			// Left: the edit-modes toolbar.
			Control modes = ToolStripViews.CreateToolBar(Field<WF.ToolStrip>("modestoolbar"), Orientation.Vertical);
			// With every plugin's modes it's taller than small windows: what doesn't fit goes into an overflow drop-down.
			Border modesborder = new Border { Child = modes, Padding = new Thickness(1, 2), ClipToBounds = true };
			modesborder.Classes.Add("uldb-bar");

			// Bottom: info panel and status bar.
			Control statusbar = ToolStripViews.CreateToolBar(Field<WF.StatusStrip>("statusbar"), Orientation.Horizontal);
			Border statusborder = new Border { Child = statusbar, Padding = new Thickness(2, 1), MinHeight = 24 };
			statusborder.Classes.Add("uldb-bar");
			Control infopanel = CreateInfoPanel();

			// Status bar icons show a state: which of upstream's status images is showing, and
			// whether there are warnings.
			ToolStripViews.IconKeyProvider = item =>
			{
				if(item.Name == "statuslabel" && item.Image != null)
				{
					string[] names = { "ready", "loading", "busy", "warning" };
					for(int flash = 0; flash < 2; flash++)
						for(int i = 0; i < 4; i++)
							if(ReferenceEquals(form.STATUS_IMAGES[flash, i], item.Image)) return "status:" + names[i] + (flash == 1 ? ":dim" : "");
					// The designer's own picture (before the first status change) is the "ready" one
					return "status:ready";
				}
				if(item.Name == "warnsLabel") return item.Text == "0" ? "warns:off" : "warns:on";
				return null;
			};

			// Middle: the map view, with upstream's splash logo while no map is open.
			viewport = new ViewportControl(display);
			Control view = CreateDisplay(viewport);

			DockPanel root = new DockPanel();
			DockPanel.SetDock(menuborder, Dock.Top);
			DockPanel.SetDock(toolbarborder, Dock.Top);
			DockPanel.SetDock(statusborder, Dock.Bottom);
			DockPanel.SetDock(infopanel, Dock.Bottom);
			DockPanel.SetDock(modesborder, Dock.Left);
			root.Children.Add(menuborder);
			root.Children.Add(toolbarborder);
			root.Children.Add(statusborder);
			root.Children.Add(infopanel);
			root.Children.Add(modesborder);
			root.Children.Add(CreateMiddle(view, modesborder));
			Content = new Grid { Children = { root, CreateBusyOverlay() } };

			HookInput();
			HookLifecycle();
		}

		private Border busyoverlay;
		private TextBlock busytext;
		private string busymessage = "";

		/// <summary>A dimmed "Loading…" cover shown while the UI thread is blocked (see ULDB.Platform.Busy).</summary>
		private Control CreateBusyOverlay()
		{
			busytext = new TextBlock { FontSize = 18, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
			busyoverlay = new Border { Background = new SolidColorBrush(Color.FromArgb(110, 0, 0, 0)), IsVisible = false, Child = busytext };
			ULDB.Platform.Busy.Show = message =>
			{
				busymessage = message;
				busytext.Text = message;
				busytext.TextAlignment = TextAlignment.Center;
				busytext.Foreground = Brushes.White;
				busyoverlay.IsVisible = true;
				// Lay out and paint one frame now; the work that follows won't return to the loop.
				Dispatcher.UIThread.RunJobs(DispatcherPriority.Render);
				Dispatcher.UIThread.RunJobs();
			};
			ULDB.Platform.Busy.Hide = () => busyoverlay.IsVisible = false;
			// Progress: the load's log lines become the cover's second line (UI thread only; the
			// background resource loader logs from its own thread).
			System.Diagnostics.Stopwatch since = System.Diagnostics.Stopwatch.StartNew();
			ULDB.Platform.Busy.Step = step =>
			{
				if(!busyoverlay.IsVisible || !Dispatcher.UIThread.CheckAccess() || since.ElapsedMilliseconds < 40) return;
				since.Restart();
				string line = step.Trim().TrimEnd('.');
				busytext.Text = busymessage + "\n" + (line.Length > 80 ? line.Substring(0, 80) + "…" : line);
				Dispatcher.UIThread.RunJobs(DispatcherPriority.Render);
			};
			return busyoverlay;
		}

		/// <summary>One of MainForm's designer fields (private upstream; names are stable).</summary>
		private T Field<T>(string name) where T : class
		{
			FieldInfo f = typeof(MainForm).GetField(name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
				?? throw new MissingFieldException(nameof(MainForm), name);
			return (T)f.GetValue(form);
		}

		#region ================== Parts

		// The map view with the dockers beside it. Upstream reserves "dockersspace" (the collapsed
		// tab strip, or the whole docker when it's pinned open) and lays "dockerspanel" over the
		// map at that edge, so an auto-hiding docker expands over the map.
		private Control CreateMiddle(Control view, Control modesborder)
		{
			DockersControl dockersmodel = Field<DockersControl>("dockerspanel");
			WF.Panel dockersspace = Field<WF.Panel>("dockersspace");
			WF.ToolStrip modestoolbar = Field<WF.ToolStrip>("modestoolbar");
			DockersView dockers = new DockersView(dockersmodel);
			Panel middle = new Panel();
			middle.Children.Add(view);
			middle.Children.Add(dockers);

			string last = null;
			void Layout()
			{
				bool right = dockersspace.Dock != WF.DockStyle.Left;
				double space = dockersspace.Visible ? dockersspace.Width : 0;
				string now = right + "|" + space + "|" + modestoolbar.Dock;
				if(now == last) return;
				last = now;
				dockers.HorizontalAlignment = right ? HorizontalAlignment.Right : HorizontalAlignment.Left;
				view.Margin = right ? new Thickness(0, 0, space, 0) : new Thickness(space, 0, 0, 0);
				DockPanel.SetDock(modesborder, modestoolbar.Dock == WF.DockStyle.Right ? Dock.Right : Dock.Left);
			}
			Layout();
			// Upstream changes these without events (SetupInterface, the splitter); check regularly.
			DispatcherTimer poll = new DispatcherTimer(TimeSpan.FromMilliseconds(100), DispatcherPriority.Background, (s, e) => Layout());
			poll.Start();
			return middle;
		}

		private Control CreateDisplay(ViewportControl gl)
		{
			// Upstream shows a logo on the display until a map is opened (RenderTargetControl.SetSplashLogoDisplay).
			Image splash = new Image { Stretch = Stretch.None, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
			Border splashlayer = new Border { Child = splash, IsHitTestVisible = false };
			void UpdateSplash()
			{
				splash.Source = AvaloniaImages.From(display.BackgroundImage);
				splashlayer.IsVisible = display.BackgroundImage != null;
				System.Drawing.Color c = display.BackColor;
				splashlayer.Background = new SolidColorBrush(c.A == 0 ? Colors.Black : Color.FromArgb(255, c.R, c.G, c.B));
			}
			UpdateSplash();
			display.StylesChanged += (s, e) => Dispatcher.UIThread.Post(UpdateSplash);

			// The OpenGL control draws itself and isn't hit-testable, so pointer input is taken
			// by this transparent panel over the same area.
			Panel layers = new Panel { Background = Brushes.Transparent, Focusable = true };
			layers.Children.Add(gl);
			layers.Children.Add(splashlayer);
			layers.Children.Add(new ToastOverlay(display));
			layers.Children.Add(new CommandPaletteOverlay(() => input?.Focus()));
			input = layers;
			Border frame = new Border { Child = layers, BorderThickness = new Thickness(1) };
			frame.Classes.Add("uldb-display");
			return frame;
		}

		// Info panel heights: expanded (cards) and collapsed (one line), in ULDB's layout.
		private const double InfoPanelExpandedHeight = 126;
		private const double InfoPanelCollapsedHeight = 26;

		private Control CreateInfoPanel()
		{
			// Upstream's info panels (model-ported) are shown by their .axaml views; MainForm shows
			// and hides them, the mode name and the statistics exactly as upstream does.
			ModelBinder binder = new ModelBinder(form);
			WF.Control panelinfo = binder.Watch(Field<WF.Panel>("panelinfo"));
			WF.Label modename = (WF.Label)binder.Watch(Field<WF.Label>("modename"));
			WF.Label collapsed = (WF.Label)binder.Watch(Field<WF.Label>("labelcollapsedinfo"));
			WF.Button toggle = Field<WF.Button>("buttontoggleinfo");
			StatisticsControl statsmodel = (StatisticsControl)binder.Watch(Field<StatisticsControl>("statistics"));
			// Debug builds of upstream show a debug console in place of the mode name; ULDB doesn't
			// show that console, so the mode name stands in for it.
			WF.Control console = binder.Watch(Field<WF.Control>("console"));

			Panel cards = new Panel { Margin = new Thickness(6, 6, 34, 6), ClipToBounds = true };
			void AddCard(WF.Control model, Control view)
			{
				binder.Watch(model);
				binder.Custom(() => view.IsVisible = model.Visible && form.IsInfoPanelExpanded);
				cards.Children.Add(view);
			}
			AddCard(Field<LinedefInfoPanel>("linedefinfo"), new LinedefInfoView(Field<LinedefInfoPanel>("linedefinfo")));
			AddCard(Field<SectorInfoPanel>("sectorinfo"), new SectorInfoView(Field<SectorInfoPanel>("sectorinfo")));
			AddCard(Field<ThingInfoPanel>("thinginfo"), new ThingInfoView(Field<ThingInfoPanel>("thinginfo")));
			AddCard(Field<VertexInfoPanel>("vertexinfo"), new VertexInfoView(Field<VertexInfoPanel>("vertexinfo")));

			// Nothing highlighted: the mode name, and the map statistics on the right.
			TextBlock title = new TextBlock { FontSize = 28, FontWeight = FontWeight.SemiBold, Opacity = 0.55, VerticalAlignment = VerticalAlignment.Center };
			// A thin accent bar beside the mode name ties it to the selected mode button
			Border titlebar = new Border { Width = 3, Height = 30, CornerRadius = new CornerRadius(2), VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(0, 0, 10, 0) };
			titlebar[!Border.BackgroundProperty] = titlebar.GetResourceObservable("UldbHotTextBrush").ToBinding();
			StackPanel titlerow = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(12, 0), Children = { titlebar, title } };
			StatisticsView stats = new StatisticsView(statsmodel) { HorizontalAlignment = HorizontalAlignment.Right, VerticalAlignment = VerticalAlignment.Center };
			Grid moderow = new Grid { ColumnDefinitions = new ColumnDefinitions("*,Auto") };
			Grid.SetColumn(stats, 1);
			moderow.Children.Add(titlerow);
			moderow.Children.Add(stats);
			cards.Children.Add(moderow);

			// Collapsed: one line of info (upstream's labelcollapsedinfo).
			TextBlock collapsedtext = new TextBlock { VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(10, 0, 34, 0), TextTrimming = TextTrimming.CharacterEllipsis };

			// Expand/collapse (upstream's buttontoggleinfo, which runs the "toggleinfopanel" action).
			PathIcon chevron = new PathIcon { Width = 10, Height = 7 };
			Button togglebutton = new Button
			{
				Content = chevron,
				HorizontalAlignment = HorizontalAlignment.Right,
				VerticalAlignment = VerticalAlignment.Top,
				Margin = new Thickness(0, 4, 6, 0),
				Width = 24, Height = 20, Padding = new Thickness(0),
				HorizontalContentAlignment = HorizontalAlignment.Center,
				VerticalContentAlignment = VerticalAlignment.Center,
			};
			togglebutton.Classes.Add("uldb-tool");
			togglebutton.Click += (s, e) => toggle.RaiseClick();

			Panel content = new Panel();
			content.Children.Add(cards);
			content.Children.Add(collapsedtext);
			content.Children.Add(togglebutton);
			Border border = new Border { Child = content, BorderThickness = new Thickness(0, 1, 0, 0) };
			border.Classes.Add("uldb-infopanel");

			binder.Custom(() =>
			{
				bool expanded = form.IsInfoPanelExpanded;
				border.IsVisible = panelinfo.Visible;
				border.Height = expanded ? InfoPanelExpandedHeight : InfoPanelCollapsedHeight;
				cards.IsVisible = expanded;
				bool showname = modename.Visible || console.Visible;
				moderow.IsVisible = showname || statsmodel.Visible;
				titlerow.IsVisible = showname;
				title.Text = modename.Text;
				stats.IsVisible = statsmodel.Visible;
				collapsedtext.IsVisible = collapsed.Visible && !expanded;
				collapsedtext.Text = collapsed.Text;
				chevron.Data = Geometry.Parse(expanded ? "M0,0 L5,5 L10,0 L10,1.8 L5,6.8 L0,1.8 Z" : "M0,6.8 L5,1.8 L10,6.8 L10,5 L5,0 L0,5 Z");
				ToolTip.SetTip(togglebutton, expanded ? "Collapse the info panel" : "Expand the info panel");
			});
			binder.UpdateNow();
			return border;
		}

		#endregion

		#region ================== Input

		// ULDB_TRACE_INPUT=1 logs the input events passed to upstream (for diagnosing input issues).
		private static readonly bool traceinput = Environment.GetEnvironmentVariable("ULDB_TRACE_INPUT") == "1";

		private static void Trace(string what, WF.MouseEventArgs e)
		{
			if(!traceinput) return;
			CodeImp.DoomBuilder.Actions.Action[] actions = General.Actions?.GetActionsByKey(KeyFor(e)) ?? new CodeImp.DoomBuilder.Actions.Action[0];
			General.WriteLogLine("[input] " + what + " button=" + e.Button + " clicks=" + e.Clicks + " at " + e.X + "," + e.Y + " delta=" + e.Delta +
				" actions=" + string.Join(",", Array.ConvertAll(actions, a => a.Name)));
		}

		private static int KeyFor(WF.MouseEventArgs e)
		{
			if(e.Delta > 0) return (int)SpecialKeys.MScrollUp;
			if(e.Delta < 0) return (int)SpecialKeys.MScrollDown;
			switch(e.Button)
			{
				case WF.MouseButtons.Left: return (int)WF.Keys.LButton;
				case WF.MouseButtons.Right: return (int)WF.Keys.RButton;
				case WF.MouseButtons.Middle: return (int)WF.Keys.MButton;
				default: return 0;
			}
		}

		private void HookInput()
		{
			WF.Control.ModifierKeysProvider = () => modifiers;

			// Screen coordinates are device pixels, as in X11. Upstream's visual mode reads the
			// pointer and warps it back to the middle of the window every frame (mouselook).
			display.ScreenMapper = new VisualMapper(viewport);
			form.ScreenMapper = new VisualMapper(this);
			WF.Cursor.PositionProvider = () =>
			{
				if(X11Pointer.GetPosition() is (int x, int y)) return new System.Drawing.Point(x, y);
				PixelPoint p = viewport.PointToScreen(lastpointer);
				return new System.Drawing.Point(p.X, p.Y);
			};
			WF.Cursor.PositionSetter = p => X11Pointer.SetPosition(p.X, p.Y);

			input.PointerEntered += (s, e) => { Track(e); display.RaiseMouseEnter(); };
			input.PointerExited += (s, e) => { Track(e); display.RaiseMouseLeave(); };
			input.PointerMoved += (s, e) => display.RaiseMouseMove(MouseArgs(e, 0, 0));

			input.PointerPressed += (s, e) =>
			{
				input.Focus();
				e.Pointer.Capture(input);
				WF.MouseEventArgs args = MouseArgs(e, e.ClickCount, 0, KeyMapping.ToMouseButton(e.GetCurrentPoint(viewport).Properties.PointerUpdateKind.GetMouseButton()));
				Trace("down", args);
				display.RaiseMouseDown(args);
				if(e.ClickCount == 2) display.RaiseMouseDoubleClick(args);
				e.Handled = true;
			};

			input.PointerReleased += (s, e) =>
			{
				WF.MouseEventArgs args = MouseArgs(e, 1, 0, KeyMapping.ToMouseButton(e.InitialPressMouseButton));
				Trace("up", args);
				display.RaiseMouseUp(args);
				if(new Rect(viewport.Bounds.Size).Contains(e.GetPosition(viewport))) display.RaiseMouseClick(args);
				e.Pointer.Capture(null);
				e.Handled = true;
			};

			// Upstream handles the wheel on the form (MainForm.OnMouseWheel), one notch = 120.
			input.PointerWheelChanged += (s, e) =>
			{
				Track(e);
				int delta = (int)Math.Round(e.Delta.Y * 120);
				WF.MouseEventArgs wheel = MouseArgs(e, 0, delta);
				Trace("wheel", wheel);
				if(delta != 0) form.RaiseMouseWheel(wheel);
				e.Handled = true;
			};

			// Keys go to MainForm (it overrides WinForms' Tab/arrow focus handling), unless
			// a text field has focus.
			AddHandler(KeyDownEvent, (s, e) =>
			{
				KeyModifiers mods = WithOwnModifier(e.Key, e.KeyModifiers, true);
				modifiers = KeyMapping.ToModifiers(mods);
				if(IsTextInput(e.Source)) return;
				WF.Keys keys = KeyMapping.ToKeys(e.Key, mods);
				if((keys & WF.Keys.KeyCode) == WF.Keys.None) return;
				display.RaisePreviewKeyDown(new WF.PreviewKeyDownEventArgs(keys));
				form.RaiseKeyDown(new WF.KeyEventArgs(keys));
				e.Handled = true;
			}, Avalonia.Interactivity.RoutingStrategies.Tunnel);

			AddHandler(KeyUpEvent, (s, e) =>
			{
				KeyModifiers mods = WithOwnModifier(e.Key, e.KeyModifiers, false);
				modifiers = KeyMapping.ToModifiers(mods);
				if(IsTextInput(e.Source)) return;
				WF.Keys keys = KeyMapping.ToKeys(e.Key, mods);
				if((keys & WF.Keys.KeyCode) == WF.Keys.None) return;
				WF.KeyEventArgs args = new WF.KeyEventArgs(keys);
				form.RaiseKeyUp(args);
				if(!args.Handled) display.RaiseKeyUp(args);
				e.Handled = true;
			}, Avalonia.Interactivity.RoutingStrategies.Tunnel);
		}

		// Avalonia (Wayland in particular) reports the modifier state from before the event: pressing Shift says
		// "no modifiers", releasing it says "Shift". UDB reads Shift/Ctrl/Alt from these events, so a Shift-drag
		// selection replaced the selection and Shift then looked stuck down (issue #28). A modifier key's own
		// press and release set and clear its flag.
		internal static KeyModifiers WithOwnModifier(Key key, KeyModifiers mods, bool down)
		{
			KeyModifiers own = key switch
			{
				Key.LeftShift or Key.RightShift => KeyModifiers.Shift,
				Key.LeftCtrl or Key.RightCtrl => KeyModifiers.Control,
				Key.LeftAlt or Key.RightAlt => KeyModifiers.Alt,
				_ => KeyModifiers.None,
			};
			return down ? mods | own : mods & ~own;
		}

		private static bool IsTextInput(object source) =>
			source is TextBox || source is NumericUpDown || source is AutoCompleteBox || (source as Visual)?.FindAncestorOfType<TextBox>() != null;

		private void Track(PointerEventArgs e)
		{
			lastpointer = e.GetPosition(viewport);
			modifiers = KeyMapping.ToModifiers(e.KeyModifiers);
		}

		// The renderer works in device pixels, so mouse positions are scaled to match.
		private WF.MouseEventArgs MouseArgs(PointerEventArgs e, int clicks, int delta, WF.MouseButtons? button = null)
		{
			Track(e);
			PointerPoint p = e.GetCurrentPoint(viewport);
			double scale = RenderScaling;
			WF.MouseButtons buttons = button ?? Buttons(p.Properties);
			return new WF.MouseEventArgs(buttons, clicks, (int)(p.Position.X * scale), (int)(p.Position.Y * scale), delta);
		}

		private static WF.MouseButtons Buttons(PointerPointProperties p)
		{
			WF.MouseButtons b = WF.MouseButtons.None;
			if(p.IsLeftButtonPressed) b |= WF.MouseButtons.Left;
			if(p.IsRightButtonPressed) b |= WF.MouseButtons.Right;
			if(p.IsMiddleButtonPressed) b |= WF.MouseButtons.Middle;
			if(p.IsXButton1Pressed) b |= WF.MouseButtons.XButton1;
			if(p.IsXButton2Pressed) b |= WF.MouseButtons.XButton2;
			return b;
		}

		#endregion

		#region ================== Lifecycle

		/// <summary>
		/// Development aid: ULDB_SHOW_FORM=vertex|linedef|sector|thing[:tab] opens that edit dialog, or
		/// ULDB_SHOW_FORM=action:name runs an action (e.g. switching mode), select:sectors selects all sectors
		/// (also linedefs, things, vertices), docker:name selects a docker; steps can be chained with "+"
		/// on the first such map element once a map is open (optionally on the given tab index),
		/// so dialogs can be checked and captured without clicking through the editor.
		/// </summary>
		private void ShowFormForDevelopment()
		{
			string what = Environment.GetEnvironmentVariable("ULDB_SHOW_FORM");
			if(string.IsNullOrEmpty(what) || (General.Map == null && !what.StartsWith("click"))) return;
			string[] parts = what.Split(':');
			int tab = parts.Length > 1 && int.TryParse(parts[1], out int t) ? t : -1;
			string configname = parts[0] switch { "vertex" => "vertexeditform", "linedef" => "linedefeditformudmf", "sector" => "sectoreditformudmf", "thing" => "thingeditformudmf", _ => parts[0] };
			if(tab >= 0) General.Settings.WriteSetting("windows." + configname + ".activetab", tab);
			Dispatcher.UIThread.Post(async () =>
			{
				// Steps joined by "+" run in order, e.g. "action:buildermodes_sectorsmode+select:sectors+action:...".
				foreach(string step in what.Split('+'))
				{
					string[] p = step.Split(':');
					switch(p[0])
					{
						case "vertex": form.ShowEditVertices(new[] { System.Linq.Enumerable.First(General.Map.Map.Vertices) }); break;
						case "linedef": form.ShowEditLinedefs(new[] { System.Linq.Enumerable.First(General.Map.Map.Linedefs, l => l.Back != null) }); break;
						case "sector": form.ShowEditSectors(new[] { System.Linq.Enumerable.First(General.Map.Map.Sectors) }); break;
						case "thing": form.ShowEditThings(new[] { System.Linq.Enumerable.First(General.Map.Map.Things) }); break;
						// The Doom/Hexen-format edit forms, opened on a UDMF map (their code runs, fields differ)
						case "oldsector": { var f = new CodeImp.DoomBuilder.Windows.SectorEditForm(); f.Setup(new[] { System.Linq.Enumerable.First(General.Map.Map.Sectors) }); f.ShowDialog(form); break; }
						case "oldlinedef": { var f = new CodeImp.DoomBuilder.Windows.LinedefEditForm(); f.Setup(new[] { System.Linq.Enumerable.First(General.Map.Map.Linedefs, l => l.Back != null) }); f.ShowDialog(form); break; }
						case "oldthing": { var f = new CodeImp.DoomBuilder.Windows.ThingEditForm(); f.Setup(new[] { System.Linq.Enumerable.First(General.Map.Map.Things) }); f.ShowDialog(form); break; }
						case "config": form.ShowConfigurationPage(int.Parse(p[1])); break; // config:<tab>, Game Configurations
						case "prefs": // prefs:<tab>, Preferences on that tab
						{
							var pf = new CodeImp.DoomBuilder.Windows.PreferencesForm();
							var tabsfield = typeof(CodeImp.DoomBuilder.Windows.PreferencesForm).GetField("tabs", BindingFlags.NonPublic | BindingFlags.Instance);
							((WF.TabControl)tabsfield.GetValue(pf)).SelectedIndex = int.Parse(p[1]);
							pf.ShowDialog(form);
							break;
						}
						case "newform": // newform:<form type with a default constructor>, shown modally
						{
							var nf = (WF.Form)Activator.CreateInstance(typeof(CodeImp.DoomBuilder.Windows.MapOptionsForm).Assembly.GetType("CodeImp.DoomBuilder.Windows." + p[1]), true);
							nf.ShowDialog(form);
							break;
						}
						case "scripteditor": // the script editor, reporting why it didn't open
							Console.WriteLine("[devstep] HasScriptLumps=" + General.Map.Config.HasScriptLumps());
							try { General.Map.ShowScriptEditor(); Console.WriteLine("[devstep] script editor shown: " + (General.Map.ScriptEditor != null && General.Map.ScriptEditor.Visible)); }
							catch(Exception ex) { Console.WriteLine("[devstep] script editor failed: " + ex); }
							Console.WriteLine("[devstep] script editor window: " + (FormWindows.WindowFor(General.Map.ScriptEditor)?.ToString() ?? "none") + " visible=" + FormWindows.WindowFor(General.Map.ScriptEditor)?.IsVisible);
							break;
						case "quicksearch": // quicksearch:<text>, types into the script editor's quick search box
						{
							var panel = General.Map.ScriptEditor.Editor;
							var box = (WF.ToolStripTextBox)typeof(CodeImp.DoomBuilder.Controls.ScriptEditorPanel).GetField("searchbox", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(panel);
							box.Text = p[1];
							break;
						}
						case "typeinscript": // typeinscript:<text>, inserts text at the start of the script editor's active script
							General.Map.ScriptEditor.Editor.ActiveTab.Editor.Scintilla.InsertText(0, p[1] + "\r\n"); Console.WriteLine("[devstep] changed=" + General.Map.ScriptEditor.Editor.ActiveTab.IsChanged + " title=" + General.Map.ScriptEditor.Editor.ActiveTab.Text + " undo=" + General.Map.ScriptEditor.Editor.ActiveTab.Editor.Scintilla.CanUndo);
							break;
						case "scriptkey": // scriptkey:<Avalonia Key>[:ctrl|alt|shift], a key press in the script editor's text
						{
							var win = FormWindows.WindowFor(General.Map.ScriptEditor);
							var area = System.Linq.Enumerable.FirstOrDefault(System.Linq.Enumerable.OfType<AvaloniaEdit.Editing.TextArea>(Avalonia.VisualTree.VisualExtensions.GetVisualDescendants(win)));
							var mods = p.Length > 2 ? (p[2] == "ctrl" ? Avalonia.Input.KeyModifiers.Control : p[2] == "alt" ? Avalonia.Input.KeyModifiers.Alt : Avalonia.Input.KeyModifiers.Shift) : Avalonia.Input.KeyModifiers.None;
							area.RaiseEvent(new Avalonia.Input.KeyEventArgs { RoutedEvent = Avalonia.Input.InputElement.KeyDownEvent, Key = Enum.Parse<Avalonia.Input.Key>(p[1]), KeyModifiers = mods });
							break;
						}
						case "scriptcaret": // scriptcaret:<offset>, moves the caret; scriptfold:<line> folds; scriptreport prints the state
							General.Map.ScriptEditor.Editor.ActiveTab.Editor.Scintilla.GotoPosition(int.Parse(p[1])); break;
						case "scripterror": General.Map.ScriptEditor.Editor.ActiveTab.Editor.Scintilla.Lines[int.Parse(p[1])].MarkerAdd(2); break; // scripterror:<line>, marks a line as a compile error
						case "scriptfold": General.Map.ScriptEditor.Editor.ActiveTab.Editor.Scintilla.Lines[int.Parse(p[1])].FoldLine(ScintillaNET.FoldAction.Contract); break;
						case "scriptreport":
						{
							var sci = General.Map.ScriptEditor.Editor.ActiveTab.Editor.Scintilla;
							var win = FormWindows.WindowFor(General.Map.ScriptEditor);
							var ed = System.Linq.Enumerable.FirstOrDefault(System.Linq.Enumerable.OfType<AvaloniaEdit.TextEditor>(Avalonia.VisualTree.VisualExtensions.GetVisualDescendants(win)));
							var foldedlines = new System.Collections.Generic.List<int>();
							for(int i = 0; i < sci.Lines.Count; i++) if(!sci.Lines[i].Expanded) foldedlines.Add(i);
							var fm = ed == null ? null : ed.TextArea.GetService(typeof(AvaloniaEdit.Folding.FoldingManager)) as AvaloniaEdit.Folding.FoldingManager;
							if(fm != null) Console.WriteLine("[devstep] sections: " + string.Join(" ", System.Linq.Enumerable.Select(fm.AllFoldings, f => f.StartOffset + "-" + f.EndOffset + (f.IsFolded ? "F" : ""))) + " line4end=" + ed.Document.GetLineByNumber(5).EndOffset);
							Console.WriteLine("[devstep] script: fontsize=" + ed?.FontSize + " lines=" + sci.Lines.Count + " caret=" + sci.CurrentPosition + " folded=[" + string.Join(",", foldedlines) + "]");
							break;
						}
						case "toast": // toast:info|warning|error, shows a toast
							General.ToastManager.ShowToast(p[1] == "error" ? CodeImp.DoomBuilder.ToastType.ERROR : p[1] == "warning" ? CodeImp.DoomBuilder.ToastType.WARNING : CodeImp.DoomBuilder.ToastType.INFO, "Information", "Example toast: a message that is long enough to wrap onto a second line in the card.", "short");
							break;
						case "effectbrowser": CodeImp.DoomBuilder.Windows.EffectBrowserForm.BrowseEffect(form, 0); break;
						case "thingbrowser": CodeImp.DoomBuilder.Windows.ThingBrowserForm.BrowseThing(form, 1); break;
						case "action": if(!General.Actions.InvokeAction(p[1])) Console.WriteLine("action: no action named " + p[1]); break; // e.g. action:buildermodes_sectorsmode
						case "docker": // docker:<short name>, e.g. docker:tagexplorerdockerpanel
							// The panel itself: MainForm.SelectDocker expects to be called from the docker's plugin.
							CodeImp.DoomBuilder.Controls.DockersControl dockers = Field<CodeImp.DoomBuilder.Controls.DockersControl>("dockerspanel");
							WF.TabControl tabs = typeof(CodeImp.DoomBuilder.Controls.DockersControl).GetField("tabs", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(dockers) as WF.TabControl;
							if(tabs != null) foreach(WF.TabPage page in tabs.TabPages)
								if(page.Tag is CodeImp.DoomBuilder.Controls.Docker d && d.Name == p[1]) dockers.SelectDocker(d);
							break;
						case "about": Field<WF.ToolStripItem>("itemhelpabout").PerformClick(); break;
						case "exception": // the unexpected-error dialog, for a real exception
							Exception thrown;
							try { throw new InvalidOperationException("Example exception for ULDB_SHOW_FORM=exception"); } catch(Exception ex) { thrown = ex; }
							CodeImp.DoomBuilder.Windows.ExceptionDialog exdlg = new CodeImp.DoomBuilder.Windows.ExceptionDialog(new System.Threading.ThreadExceptionEventArgs(thrown));
							exdlg.Setup();
							exdlg.ShowDialog(form);
							break;
						case "testmenu": // testmenu[:<text of an item to click>], lists the Test button's drop-down (and clicks one)
						{
							WF.ToolStripDropDownItem test = Field<WF.ToolStripDropDownItem>("buttontest");
							foreach(WF.ToolStripItem it in test.DropDownItems)
								Console.WriteLine("testmenu: " + (it is WF.ToolStripSeparator ? "---" : it.Text + (it is WF.ToolStripMenuItem mi && mi.Checked ? " [x]" : "") + (it.Enabled ? "" : " (header)")));
							if(p.Length > 1) foreach(WF.ToolStripItem it in test.DropDownItems) if(it.Text == p[1]) { it.PerformClick(); break; }
							break;
						}
						case "click": Field<WF.ToolStripItem>(p[1]).PerformClick(); break;
						case "wait": await System.Threading.Tasks.Task.Delay(int.Parse(p[1])); break; // wait:<ms>
						case "tree": // tree:<row>[:expander|text][:dbl] clicks a row of the first visible tree; tree:report prints its state
						{
							ULDB.Controls.TreeViewView tv = this.GetVisualDescendants().OfType<ULDB.Controls.TreeViewView>().FirstOrDefault(v => v.IsEffectivelyVisible);
							if(tv == null) { Console.WriteLine("[devstep] no tree showing"); break; }
							if(p[1] == "report") { tv.DevReport(p.Length > 2 ? p[2] : ""); break; }
							if(p[1] == "scroll") { tv.DevScrollTo(double.Parse(p[2])); break; }
							if(p[1] == "edit") { tv.DevEdit(p[2]); break; }
							int row = p[1].StartsWith("@") ? tv.DevFindRow(p[1].Substring(1)) : int.Parse(p[1]); // @text: the first row starting with it
							tv.DevClick(row, p.Length > 2 && p[2] == "expander", p.Length > 3 && p[3] == "dbl" || p.Length > 2 && p[2] == "dbl" ? 2 : 1);
							break;
						}
						case "windowbutton": // windowbutton:<field>: press a button of the newest open window's form
						{
							Window top = null;
							foreach(Window w in FormWindows.Open) if(w != this && w.IsVisible) top = w;
							WF.Form f = FormWindows.FormFor(top);
							object target = null;
							for(Type t = f?.GetType(); t != null && target == null; t = t.BaseType)
								target = t.GetField(p[1], BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly)?.GetValue(f);
							Console.WriteLine("[devstep] windowbutton " + p[1] + " on " + (f?.GetType().Name ?? "(no window)"));
							(target as WF.Button)?.PerformClick();
							break;
						}
						case "overflow": // describes every toolbar's overflow (and opens its popup)
							foreach(OverflowToolPanel bar in this.GetVisualDescendants().OfType<OverflowToolPanel>().ToList()) Console.WriteLine("[devstep] " + bar.DevOpen());
							break;
						case "report": // the open windows and whether they're showing
							foreach(Window w in FormWindows.Open) Console.WriteLine("[devstep] window \"" + w.Title + "\" visible=" + w.IsVisible);
							break;
						case "clickall": // every enabled menu item in turn (skipping ones that leave the program), logging each; with ULDB_AUTOCLOSE
							void ClickAll(WF.ToolStripItemCollection items)
							{
								foreach(WF.ToolStripItem item in items)
								{
									if(!item.Enabled || !item.Available) continue;
									if(item is WF.ToolStripDropDownItem dd && dd.DropDownItems.Count > 0) { ClickAll(dd.DropDownItems); continue; }
									string n = item.Name ?? ""; if(n.Length == 0 || item is WF.ToolStripSeparator) continue;
									if(n.Contains("exit", StringComparison.OrdinalIgnoreCase) || n.Contains("quit", StringComparison.OrdinalIgnoreCase) || n.Contains("open", StringComparison.OrdinalIgnoreCase) || n.Contains("save", StringComparison.OrdinalIgnoreCase) || n.Contains("recent", StringComparison.OrdinalIgnoreCase)) continue;
									Console.WriteLine("[clickall] " + n + " \"" + item.Text + "\"");
									try { item.PerformClick(); } catch(Exception e) { Console.WriteLine("[clickall] exception: " + e.Message); }
								}
							}
							ClickAll(Field<WF.ToolStrip>("menumain").Items);
							Console.WriteLine("[clickall] done");
							break; // a toolbar or menu item of MainForm, by its field name (works without a map)
						case "resource": // the Add Resource dialog (resource:<tab index>)
							CodeImp.DoomBuilder.Windows.ResourceOptionsForm resform = new CodeImp.DoomBuilder.Windows.ResourceOptionsForm(new CodeImp.DoomBuilder.Data.DataLocation(), "Add Resource", null);
							resform.ShowDialog(form);
							break;
						case "texbrowser": // texbrowser:textures|flats[:name], the texture/flat browser
							CodeImp.DoomBuilder.Windows.TextureBrowserForm.Browse(form, p.Length > 2 ? p[2] : "STARTAN3", p.Length > 1 && p[1] == "flats");
							break;
						case "window": // window:<width>x<height>, resizes the main window (to test saving its geometry)
						{
							string[] wh = p[1].Split('x');
							Width = double.Parse(wh[0]); Height = double.Parse(wh[1]);
							break;
						}
						case "closeapp": // closes the main window as the user would, after the pending geometry save
							DispatcherTimer.RunOnce(Close, TimeSpan.FromSeconds(1.5));
							break;
						case "reportdialogs": // reportdialogs:<ms>, after that time prints each open window's place (run before a modal step)
							DispatcherTimer.RunOnce(() =>
							{
								foreach(Window w in FormWindows.Open)
									Console.WriteLine($"[devstep] open window '{w.Title}' visible={w.IsVisible} {w.Bounds.Width}x{w.Bounds.Height} at {w.Position.X},{w.Position.Y}");
							}, TimeSpan.FromMilliseconds(int.Parse(p[1])));
							break;
						case "reportmap": Console.WriteLine("reportmap: " + General.Map.Map.Sectors.Count + " sectors, " + General.Map.Map.Linedefs.Count + " linedefs"); break; // the map's size (to check a paste)
						case "reportclip": Console.WriteLine("reportclip: " + string.Join(", ", WF.WinFormsHost.Current.GetClipboard().GetFormats())); break; // what the clipboard holds
						case "markchanged": General.Map.IsChanged = true; break; // makes the map count as edited (autosave only saves edited maps)
						case "mapbackup": General.Map.SaveMapBackup(); break; // what Quit in the error dialog does
						case "closetop": // closes the newest open window other than the main one, as the title bar's close button does
						{
							Window top = null;
							foreach(Window w in FormWindows.Open) if(w != this && w.IsVisible) top = w;
							Console.WriteLine("[devstep] closetop " + (top?.Title ?? "(no window)"));
							top?.Close();
							break;
						}
						case "clicktag": // clicktag:<tag>, clicks the toolbar or menu item with that Tag (like pressing the button)
						{
							WF.ToolStripItem found = null;
							void Look(System.Collections.IEnumerable items)
							{
								foreach(WF.ToolStripItem item in items)
							{
									if(found == null && (item.Tag as string) == p[1] && item.Available) found = item;
									if(item is WF.ToolStripDropDownItem d) Look(d.DropDownItems);
								}
							}
							foreach(string bar in new[] { "toolbar", "modestoolbar", "modecontrolsloolbar", "menumain" }) Look(Field<WF.ToolStrip>(bar).Items);
							Console.WriteLine("[devstep] clicktag " + p[1] + ": " + (found != null ? found.Text : "(not found)"));
							found?.PerformClick();
							break;
						}
						case "testengine": // testengine:<program or pseudo path>: sets it as this configuration's test engine and tests the map
							General.Map.ConfigSettings.TestProgram = string.Join(":", p.Skip(1));
							General.Map.Launcher.Test();
							break;
						case "unsavedautosave": // makes the map look never-saved and autosaves it once (prints the result)
							typeof(MapManager).GetField("filepathname", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(General.Map, "");
							Console.WriteLine("[devstep] unsaved autosave: " + typeof(MapManager).GetMethod("AutoSave", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(General.Map, null));
							break;
						case "setting": // setting:<key>:<number>, writes a program setting (for checking options without the dialog)
							General.Settings.WriteSetting(p[1], int.Parse(p[2]));
							break;
						case "reportwindow":
							Console.WriteLine($"[devstep] window {Width}x{Height} at {Position.X},{Position.Y} state {WindowState}");
							break;
						case "runcommand": // Run External Command window, running a short shell script (output and an error line)
						{
							var info = new System.Diagnostics.ProcessStartInfo("sh", "-c \"echo compiling...; echo all fine; echo broken thing >&2; sleep 1; echo done\"");
							new CodeImp.DoomBuilder.Windows.RunExternalCommandForm(info, new CodeImp.DoomBuilder.Config.ExternalCommandSettings { StdErrIsError = true }).ShowDialog(form);
							break;
						}
						case "flagsdialogs": // flagsdialogs:flags|bitflags|bitoptions|customfields, with sample data
						{
							var sample = new CodeImp.DoomBuilder.Config.EnumList();
							sample.Add(new CodeImp.DoomBuilder.Config.EnumItem("1", "First flag"));
							sample.Add(new CodeImp.DoomBuilder.Config.EnumItem("2", "Second flag"));
							sample.Add(new CodeImp.DoomBuilder.Config.EnumItem("4", "Third flag"));
							switch(p[1])
							{
								case "flags": CodeImp.DoomBuilder.Windows.FlagsForm.ShowDialog(form, "a", new System.Collections.Generic.Dictionary<string, string> { ["a"] = "Alpha", ["b"] = "Beta", ["c"] = "Gamma" }); break;
								case "bitflags": CodeImp.DoomBuilder.Windows.BitFlagsForm.ShowDialog(form, sample, 3); break;
								case "bitoptions": CodeImp.DoomBuilder.Windows.BitFlagsAndOptionsForm.ShowDialog(form, sample, sample, 5); break;
								case "customfields":
									CodeImp.DoomBuilder.Windows.CustomFieldsForm.ShowDialog(form, () => { }, "Front side custom fields", "sidedef",
										new System.Collections.Generic.List<CodeImp.DoomBuilder.Map.MapElement> { System.Linq.Enumerable.First(General.Map.Map.Sidedefs) }, General.Map.Config.SidedefFields);
									break;
							}
							break;
						}
						case "plugform": // plugform:<plugin assembly>:<form type>, shown modally; a Thing parameter gets the first thing
							foreach(System.Reflection.Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
							{
								if(asm.GetName().Name != p[1]) continue;
								Type ft = asm.GetTypes().First(t => t.Name == p[2]);
								System.Reflection.ConstructorInfo ci = ft.GetConstructors().First();
								object[] cargs = ci.GetParameters().Select(pi => pi.ParameterType == typeof(CodeImp.DoomBuilder.Map.Thing) ? (object)System.Linq.Enumerable.First(General.Map.Map.Things) : null).ToArray();
								((WF.Form)ci.Invoke(cargs)).ShowDialog(form);
							}
							break;
						case "call": // call:<plugin assembly>:<BuilderPlug method>, e.g. call:ThreeDFloorMode:ThreeDFloorEditor
							foreach(System.Reflection.Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
							{
								if(asm.GetName().Name != p[1]) continue;
								Type plug = asm.GetTypes().First(t => t.Name == "BuilderPlug");
								object me = plug.GetProperty("Me", BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic)?.GetValue(null);
								plug.GetMethod(p[2], BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Invoke(me, null);
							}
							break;
						case "selectone": // selectone:sectors|linedefs|things|vertices, the first of that type
							switch(p[1])
							{
								case "sectors": System.Linq.Enumerable.First(General.Map.Map.Sectors).Selected = true; break;
								case "linedefs": System.Linq.Enumerable.First(General.Map.Map.Linedefs).Selected = true; break;
								case "things": System.Linq.Enumerable.First(General.Map.Map.Things).Selected = true; break;
								case "vertices": System.Linq.Enumerable.First(General.Map.Map.Vertices).Selected = true; break;
							}
							break;
						case "select": // select:sectors|linedefs|things|vertices, everything of that type
							switch(p[1])
							{
								case "sectors": foreach(var e in General.Map.Map.Sectors) e.Selected = true; break;
								case "linedefs": foreach(var e in General.Map.Map.Linedefs) e.Selected = true; break;
								case "things": foreach(var e in General.Map.Map.Things) e.Selected = true; break;
								case "vertices": foreach(var e in General.Map.Map.Vertices) e.Selected = true; break;
							}
							break;
					}
				}
			}, DispatcherPriority.Background);
		}

		// Development aid: one line per item, submenus included; icon=False marks an item without an icon.
		private static void DumpItem(string path, WF.ToolStripItem item)
		{
			if(item is WF.ToolStripSeparator) return;
			Console.WriteLine($"[toolbar] {path} {item.GetType().Name} tag={item.Tag} name={item.Name} text={item.Text} icon={ToolStripViews.HasIcon(item)}");
			if(item is WF.ToolStripDropDownItem d) foreach(WF.ToolStripItem sub in d.DropDownItems) DumpItem(path + "/" + item.Name, sub);
		}

		private void HookLifecycle()
		{
			// Upstream opens command-line maps on Shown, which needs the renderer's OpenGL
			// context: wait for it first.
			Opened += async (s, e) =>
			{
				try { await ViewportContext.InitializeAsync(viewport); }
				catch(Exception) { } // Reported by the viewport; the editor runs without a map view
				ClampToScreen();
				form.RaiseShown();
				input.Focus();
				ShowFormForDevelopment();
				// Main window capture, unless a dialog opens first (it's scheduled sooner after opening).
				DevCapture.Schedule(this, TimeSpan.FromSeconds(4));
				// After a command-line map has opened, so mode-specific buttons are there too.
				if(Environment.GetEnvironmentVariable("ULDB_DUMP_TOOLBARS") == "1")
					DispatcherTimer.RunOnce(() =>
					{
						foreach(string bar in new[] { "toolbar", "modestoolbar", "modecontrolsloolbar", "menumain" })
							foreach(WF.ToolStripItem item in Field<WF.ToolStrip>(bar).Items)
								DumpItem(bar, item);
					}, TimeSpan.FromSeconds(3));
			};
			Activated += (s, e) => form.RaiseActivated();
			Deactivated += (s, e) => form.RaiseDeactivate();

			// Same as closing a WinForms form: upstream's OnFormClosing may cancel (unsaved map)
			// or end the program (General.Terminate).
			Closing += (s, e) =>
			{
				if(closing) return;
				if(!form.RequestClose(WF.CloseReason.UserClosing)) e.Cancel = true;
				else closing = true;
			};

			// Upstream's DelayedForm remembers the window's size and place from ResizeEnd (WinForms raises it when
			// a resize or drag ends); here that's half a second after the last change.
			DispatcherTimer geometrytimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(500) };
			geometrytimer.Tick += (s, e) => { geometrytimer.Stop(); SyncFormGeometry(); form.RaiseResizeEnd(); };
			Resized += (s, e) => { SyncFormGeometry(); geometrytimer.Stop(); geometrytimer.Start(); };
			PositionChanged += (s, e) => { SyncFormGeometry(); geometrytimer.Stop(); geometrytimer.Start(); };
			PropertyChanged += (s, e) => { if(e.Property == WindowStateProperty) SyncFormGeometry(); };
		}

		// A window bigger than the screen (the 1280x860 default on a 1366x768 laptop) has its top and bottom out of reach:
		// cut it to the working area and move it into view. The screen is only known once the window is open.
		private void ClampToScreen()
		{
			Avalonia.Platform.Screen screen = Screens.ScreenFromVisual(this) ?? Screens.Primary;
			if(screen == null || WindowState != WindowState.Normal) return;
			PixelRect work = screen.WorkingArea;
			double scale = screen.Scaling;
			double maxw = work.Width / scale, maxh = work.Height / scale - 32; // title bar and frame
			if(Width > maxw) Width = maxw;
			if(Height > maxh) Height = Math.Max(MinHeight, maxh);
			PixelPoint at = Position;
			int x = Math.Max(work.X, Math.Min(at.X, work.Right - (int)(Width * scale)));
			int y = Math.Max(work.Y, Math.Min(at.Y, work.Bottom - (int)((Height + 32) * scale)));
			if(x != at.X || y != at.Y) Position = new PixelPoint(x, y);
		}

		/// <summary>Runs before the window is shown, as WinForms raises Load.</summary>
		public void Load()
		{
			form.RaiseLoad();

			// DelayedForm.OnLoad has put the saved size and place on the form; the window takes them (only when
			// something was saved, so a first start keeps ULDB's default size).
			if(General.Settings.ReadSetting("windows.mainform.sizewidth", 0) > 0 && General.Settings.ReadSetting("windows.mainform.sizeheight", 0) > 0)
			{
				double scale = Screens.Primary?.Scaling ?? 1.0;
				Width = Math.Max(MinWidth, form.Width / scale);
				Height = Math.Max(MinHeight, form.Height / scale);
			}
			if(form.StartPosition == WF.FormStartPosition.Manual)
			{
				WindowStartupLocation = WindowStartupLocation.Manual;
				Position = new PixelPoint(form.Location.X, form.Location.Y);
			}
			if(form.WindowState == WF.FormWindowState.Maximized) WindowState = WindowState.Maximized;
		}

		private void SyncFormGeometry()
		{
			form.Location = new System.Drawing.Point(Position.X, Position.Y);
			form.Size = new System.Drawing.Size((int)(ClientSize.Width * RenderScaling), (int)(ClientSize.Height * RenderScaling));
			form.WindowState = WindowState switch
			{
				WindowState.Maximized => WF.FormWindowState.Maximized,
				WindowState.Minimized => WF.FormWindowState.Minimized,
				_ => WF.FormWindowState.Normal,
			};
		}

		#endregion

		/// <summary>Maps a visual's device-pixel client coordinates to screen pixels and back.</summary>
		private sealed class VisualMapper : WF.IScreenMapper
		{
			private readonly Visual visual;
			public VisualMapper(Visual visual) { this.visual = visual; }

			private double Scale => TopLevel.GetTopLevel(visual)?.RenderScaling ?? 1.0;

			public System.Drawing.Point ToScreen(System.Drawing.Point client)
			{
				PixelPoint p = visual.PointToScreen(new Point(client.X / Scale, client.Y / Scale));
				return new System.Drawing.Point(p.X, p.Y);
			}

			public System.Drawing.Point ToClient(System.Drawing.Point screen)
			{
				Point p = visual.PointToClient(new PixelPoint(screen.X, screen.Y));
				return new System.Drawing.Point((int)Math.Round(p.X * Scale), (int)Math.Round(p.Y * Scale));
			}
		}
	}
}