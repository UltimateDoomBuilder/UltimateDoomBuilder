// ULDB: code-behind for DockersView.axaml, the view of upstream's DockersControl. Upstream's
// logic (Add/Remove/SelectDocker, Collapse/Expand, the splitter and the pin button) runs on the
// model; this shows its tabs and the selected docker, and passes pointer input back:
//  - pointer enter/leave → DockersControl's MouseEnter/MouseLeave (MainForm expands it, and its
//    collapser timer hides it again when the pointer is outside the model's bounds)
//  - tab click → tabs.SelectedIndex; pin button → pinbutton.Click; splitter drag → splitter mouse events
// Docker contents (upstream user controls) are shown through ControlViews.

using System;
using System.Collections.Generic;
using System.Text;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;
using ULDB.Shell;
using WF = System.Windows.Forms;

namespace CodeImp.DoomBuilder.Controls
{
	public partial class DockersView : UserControl
	{
		private readonly DockersControl model;
		private readonly DockersTabsControl tabsmodel;
		private readonly WF.Button pinmodel;
		private readonly WF.Control splittermodel;
		private readonly Dictionary<WF.Control, Control> pageviews = new Dictionary<WF.Control, Control>();
		private readonly DispatcherTimer poll;
		private string tabssignature;
		private string statesignature;

		public DockersView()
		{
			InitializeComponent();
		}

		internal DockersView(DockersControl model) : this()
		{
			this.model = model;
			ModelBinder binder = new ModelBinder(model);
			tabsmodel = binder.Field<DockersTabsControl>("tabs");
			pinmodel = binder.Field<WF.Button>("pinbutton");
			splittermodel = binder.Field<WF.Control>("splitter");
			binder.Watch(model);
			binder.Watch(tabsmodel);
			binder.Custom(Update);
			tabsmodel.SelectedIndexChanged += (s, e) => Dispatcher.UIThread.Post(Update);
			tabsmodel.NotifyChanged += (s, e) => Dispatcher.UIThread.Post(UpdateTabs);

			// Tab pages and widths change without events on the models; check a few times a second.
			poll = new DispatcherTimer(TimeSpan.FromMilliseconds(100), DispatcherPriority.Background, (s, e) => Update());
			AttachedToVisualTree += (s, e) => poll.Start();
			DetachedFromVisualTree += (s, e) => poll.Stop();

			frame.PointerEntered += (s, e) => model.RaiseMouseEnter();
			frame.PointerExited += (s, e) => model.RaiseMouseLeave();
			pinbutton.Click += (s, e) => pinmodel.RaiseClick();
			splitter.PointerPressed += splitter_PointerPressed;
			splitter.PointerMoved += splitter_PointerMoved;
			splitter.PointerReleased += splitter_PointerReleased;
			LayoutUpdated += (s, e) => UpdateModelBounds();

			binder.UpdateNow();
		}

		#region ================== Update

		// A pushpin: head, body and needle (12x12).
		private static readonly Avalonia.Media.Geometry PinGeometry = Avalonia.Media.Geometry.Parse("M3.5,0 H8.5 V1.2 H7.6 V5 L9.5,7 V8 H6.5 V12 H5.5 V8 H2.5 V7 L4.4,5 V1.2 H3.5 Z");

		private bool RightAlign => tabsmodel.Alignment != WF.TabAlignment.Left;

		private void Update()
		{
			if(model == null) return;

			string state = string.Join("|", model.Visible, model.Width, model.IsCollpased, tabsmodel.SelectedIndex, tabsmodel.Alignment, WF.Control.ModifierKeys == WF.Keys.None, General.Settings.CollapseDockers);
			UpdateTabs();
			if(state == statesignature) return;
			statesignature = state;

			IsVisible = model.Visible;
			Width = Math.Max(model.Width, WF.TabControl.TabStripThickness + 1);

			// Sides: tabs on the outer edge, splitter on the inner edge.
			DockPanel.SetDock(tabstrip, RightAlign ? Dock.Right : Dock.Left);
			DockPanel.SetDock(splitter, RightAlign ? Dock.Left : Dock.Right);
			frame.BorderThickness = RightAlign ? new Thickness(1, 0, 0, 0) : new Thickness(0, 0, 1, 0);

			bool collapsed = model.IsCollpased || tabsmodel.SelectedIndex < 0;
			page.IsVisible = !collapsed;
			splitter.IsVisible = !model.IsCollpased;

			// Pin (upstream: arrow images): an upright pin while the dockers stay open, a tilted,
			// faded one while they hide automatically.
			bool autohide = General.Settings.CollapseDockers;
			pinicon.Data = PinGeometry;
			pinicon.RenderTransform = new RotateTransform(autohide ? 45 : 0);
			pinicon.Opacity = autohide ? 0.55 : 1.0;
			ToolTip.SetTip(pinbutton, autohide ? "Keep the dockers open" : "Hide the dockers automatically");

			// Selected docker
			WF.TabPage selected = tabsmodel.SelectedTab;

			// As in WinForms, only the shown docker's control is Visible: plugins refresh on
			// VisibleChanged (the Comments docker fills its list then).
			foreach(WF.TabPage tp in tabsmodel.TabPages)
				if(tp.Controls.Count > 0) tp.Controls[0].Visible = tp == selected && !collapsed;
			if(selected != null && !collapsed)
			{
				pagetitle.Text = selected.Text;
				WF.Control docker = selected.Controls.Count > 0 ? selected.Controls[0] : null;
				content.Content = docker != null ? ViewFor(docker, selected.Text) : null;
			}
			else
			{
				content.Content = null;
			}
		}

		private void UpdateTabs()
		{
			if(model == null) return;
			StringBuilder sig = new StringBuilder();
			foreach(WF.TabPage p in tabsmodel.TabPages) sig.Append(p.Text).Append('\n');
			sig.Append(tabsmodel.SelectedIndex).Append(RightAlign).Append(tabsmodel.NotifyHighlightTab);
			if(sig.ToString() == tabssignature) return;
			tabssignature = sig.ToString();

			tabs.Children.Clear();
			for(int i = 0; i < tabsmodel.TabPages.Count; i++)
			{
				int index = i;
				TextBlock text = new TextBlock { Text = tabsmodel.TabPages[i].Text, FontSize = 12, FontWeight = i == tabsmodel.SelectedIndex ? FontWeight.SemiBold : FontWeight.Normal };
				LayoutTransformControl rotated = new LayoutTransformControl
				{
					Child = text,
					LayoutTransform = new RotateTransform(RightAlign ? 90 : -90),
				};
				Button tab = new Button
				{
					Content = rotated,
					HorizontalAlignment = HorizontalAlignment.Stretch,
					HorizontalContentAlignment = HorizontalAlignment.Center,
					Padding = new Thickness(0, 9),
					Margin = new Thickness(1, 0),
					CornerRadius = new CornerRadius(5),
					// An accent bar on the outer edge marks the selected tab (see Styles.axaml)
					BorderThickness = RightAlign ? new Thickness(0, 0, 2, 0) : new Thickness(2, 0, 0, 0),
				};
				tab.Classes.Add("uldb-tool");
				tab.Classes.Add("uldb-docker-tab");
				tab.Classes.Set("selected", i == tabsmodel.SelectedIndex);
				tab.Classes.Set("notify", i == tabsmodel.NotifyHighlightTab);
				tab.Click += (s, e) =>
				{
					if(model.IsCollpased) model.Expand();
					tabsmodel.SelectedIndex = index;
				};
				tabs.Children.Add(tab);
			}
		}

		private Control ViewFor(WF.Control docker, string title)
		{
			if(pageviews.TryGetValue(docker, out Control view)) return view;
			view = ControlViews.Create(docker) ?? new TextBlock
			{
				Text = "“" + title + "” isn't ported to ULDB yet.",
				TextWrapping = TextWrapping.Wrap,
				Opacity = 0.6,
				Margin = new Thickness(4),
			};
			pageviews[docker] = view;
			return view;
		}

		// MainForm's collapser compares the pointer (form client coordinates: device pixels from
		// the window's corner, see MainWindow.VisualMapper) with the model's bounds; keep them current.
		private void UpdateModelBounds()
		{
			if(model == null || !IsVisible) return;
			TopLevel top = TopLevel.GetTopLevel(this);
			Point? origin = top != null ? this.TranslatePoint(new Point(0, 0), top) : null;
			if(origin == null) return;
			double scale = top.RenderScaling;
			model.Location = new System.Drawing.Point((int)Math.Round(origin.Value.X * scale), (int)Math.Round(origin.Value.Y * scale));
			model.Height = (int)Math.Round(Bounds.Height * scale);
		}

		#endregion

		#region ================== Splitter

		private static WF.MouseEventArgs SplitterArgs(PointerEventArgs e, Visual relativeto, bool pressed)
		{
			Point p = e.GetPosition(relativeto);
			return new WF.MouseEventArgs(pressed ? WF.MouseButtons.Left : WF.MouseButtons.None, 1, (int)Math.Round(p.X), (int)Math.Round(p.Y), 0);
		}

		private void splitter_PointerPressed(object sender, PointerPressedEventArgs e)
		{
			if(!e.GetCurrentPoint(splitter).Properties.IsLeftButtonPressed) return;
			e.Pointer.Capture(splitter);
			splittermodel.RaiseMouseDown(SplitterArgs(e, splitter, true));
			e.Handled = true;
		}

		private void splitter_PointerMoved(object sender, PointerEventArgs e)
		{
			if(!e.GetCurrentPoint(splitter).Properties.IsLeftButtonPressed) return;
			splittermodel.RaiseMouseMove(SplitterArgs(e, splitter, true));
			Update();
		}

		private void splitter_PointerReleased(object sender, PointerReleasedEventArgs e)
		{
			e.Pointer.Capture(null);
			splittermodel.RaiseMouseUp(SplitterArgs(e, splitter, false));
			Update();
		}

		#endregion
	}
}
