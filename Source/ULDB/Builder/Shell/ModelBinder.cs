// ULDB: shows the state of model-ported WinForms controls (upstream code running on ULDB.Compat
// models) in Avalonia controls. Upstream fills labels and picture boxes as it always has; the
// binder copies text, visibility, enabled state, colours and images across whenever one of the
// watched controls changes, at most once per dispatcher turn.

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Avalonia.Threading;
using SD = System.Drawing;
using WF = System.Windows.Forms;

namespace ULDB.Shell
{
	internal sealed partial class ModelBinder
	{
		private readonly object model;
		private readonly List<Action> updates = new List<Action>();
		private readonly HashSet<WF.Control> watched = new HashSet<WF.Control>();
		private bool pending;
		private static readonly bool trace = Environment.GetEnvironmentVariable("ULDB_TRACE_BINDER") == "1";

		/// <summary>Binds controls of <paramref name="model"/> (fields found by their upstream names).</summary>
		public ModelBinder(object model)
		{
			this.model = model;
		}

		/// <summary>One of the model's fields, by its upstream (designer) name.</summary>
		public T Field<T>(string name) where T : class
		{
			for(Type t = model.GetType(); t != null; t = t.BaseType)
			{
				FieldInfo f = t.GetField(name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly);
				if(f != null) return (T)f.GetValue(model);
			}
			throw new MissingFieldException(model.GetType().Name, name);
		}

		/// <summary>Raised after the bound controls were updated.</summary>
		public event Action Updated;

		/// <summary>Watches a model control: any change to it schedules an update.</summary>
		public WF.Control Watch(WF.Control c)
		{
			if(c != null && watched.Add(c))
			{
				c.TextChanged += Schedule;
				c.VisibleChanged += Schedule;
				c.EnabledChanged += Schedule;
				c.StylesChanged += Schedule;
				c.Refreshed += Schedule;
			}
			return c;
		}

		/// <summary>Runs <paramref name="update"/> on every update, after watching the given controls.</summary>
		public ModelBinder Custom(Action update, params WF.Control[] watch)
		{
			foreach(WF.Control c in watch) Watch(c);
			updates.Add(update);
			return this;
		}

		/// <summary>Text, visibility, enabled state and colour of a label.</summary>
		public ModelBinder Text(TextBlock target, string field) => Text(target, Field<WF.Control>(field));

		public ModelBinder Text(TextBlock target, WF.Control c)
		{
			return Custom(() =>
			{
				target.Text = c.Text.Trim();
				target.IsVisible = c.Visible;
				ApplyTip(target, c);
				target.Classes.Set("uldb-disabled", !c.Enabled); // dimmed by Styles.axaml
				SetForeground(target, c.ForeColor);
			}, c);
		}

		/// <summary>Shows <paramref name="target"/> only while the model control is visible.</summary>
		public ModelBinder Visible(Control target, string field) => Visible(target, Field<WF.Control>(field));

		public ModelBinder Visible(Control target, WF.Control c) => Custom(() => { target.IsVisible = c.Visible; ApplyTip(target, c); }, c);

		/// <summary>Dims <paramref name="target"/> while the model control is disabled.</summary>
		public ModelBinder Enabled(Control target, string field)
		{
			WF.Control c = Field<WF.Control>(field);
			return Custom(() => target.Opacity = c.Enabled ? 1.0 : 0.45, c);
		}

		/// <summary>A picture box shown as a ContentControl: ULDB's Breeze icon when its image is a known UDB resource, else the bitmap.</summary>
		public ModelBinder Picture(ContentControl target, string field)
		{
			WF.PictureBox c = Field<WF.PictureBox>(field);
			SD.Image shown = null;
			return Custom(() =>
			{
				if(c.Image != shown || target.Content == null)
				{
					shown = c.Image;
					target.Content = shown == null ? null : BreezeIcons.ForImage(shown, 16)
						?? new Image { Source = AvaloniaImages.From(shown), Width = shown.Width, Height = shown.Height };
				}
				target.IsVisible = c.Visible;
				ApplyTip(target, c);
			}, c);
		}

		/// <summary>A picture box's image (or a control's background image).</summary>
		public ModelBinder Image(Image target, string field)
		{
			WF.Control c = Field<WF.Control>(field);
			return Custom(() =>
			{
				SD.Image image = (c as WF.PictureBox)?.Image ?? c.BackgroundImage;
				target.Source = AvaloniaImages.From(image);
				target.IsVisible = c.Visible;
				ApplyTip(target, c);
			}, c);
		}

		/// <summary>A control's background colour as a swatch.</summary>
		public ModelBinder Swatch(Border target, string field)
		{
			WF.Control c = Field<WF.Control>(field);
			return Custom(() =>
			{
				SD.Color col = c.BackColor;
				target.Background = new SolidColorBrush(Color.FromArgb(col.A == 0 ? (byte)255 : col.A, col.R, col.G, col.B));
				target.IsVisible = c.Visible;
			}, c);
		}

		/// <summary>
		/// Binds every named control under <paramref name="root"/> to the model field of the same name:
		/// inputs (TextBox, CheckBox, ComboBox, Button, NumericUpDown, TabControl) → two-way, see
		/// ModelBinder.Inputs.cs; ContentControl → the model control's registered view (ControlViews);
		/// TextBlock → label text, state and colour; "<i>name</i>Title" TextBlock → a group's text;
		/// Image → picture box image; Border with class uldb-swatch → background colour;
		/// ItemsControl → a ListView's items (as <see cref="ListItem"/>s); anything else → visibility.
		/// Also watches the model itself, so Show()/Refresh() on it updates the view.
		/// </summary>
		public ModelBinder BindByName(Control root)
		{
			if(model is WF.Control self) Watch(self);
			foreach(ILogical logical in OwnDescendants(root))
			{
				if(!(logical is Control c) || string.IsNullOrEmpty(c.Name)) continue;
				string name = c.Name;
				if(trace) Console.WriteLine($"[binder] {model.GetType().Name}.{name} ({c.GetType().Name}) field={HasField(name)}");
				if(c is TextBlock title && name.EndsWith("Title") && HasField(name.Substring(0, name.Length - 5)))
					Text(title, Field<WF.Control>(name.Substring(0, name.Length - 5)));
				else if(!HasField(name)) continue;
				else if(BindInput(c, Field<object>(name))) continue;
				else if(c is TextBlock tb) Text(tb, name);
				else if(c is Image img) Image(img, name);
				else if(c is ContentControl pic && Field<object>(name) is WF.PictureBox) Picture(pic, name);
				else if(c is Border sw && sw.Classes.Contains("uldb-swatch")) Swatch(sw, name);
				else if(c is ItemsControl items && Field<object>(name) is WF.ListView list) Items(items, list);
				else Visible(c, name);
			}
			return this;
		}

		/// <summary>
		/// The root and its logical descendants, without going into embedded views (other
		/// UserControls and windows): those belong to their own models, whose fields may share
		/// names with this model's (upstream designers reuse names like "label1").
		/// </summary>
		private static IEnumerable<ILogical> OwnDescendants(Control root)
		{
			Stack<ILogical> todo = new Stack<ILogical>();
			todo.Push(root);
			while(todo.Count > 0)
			{
				ILogical item = todo.Pop();
				yield return item;
				if(item != root && (item is UserControl || item is Window)) continue;
				IReadOnlyList<ILogical> children = item.LogicalChildren;
				for(int i = children.Count - 1; i >= 0; i--) todo.Push(children[i]);
			}
		}

		/// <summary>Opacity for a ListItem's Dim flag (greyed items), for item templates.</summary>
		public static readonly Avalonia.Data.Converters.IValueConverter DimOpacity = new Avalonia.Data.Converters.FuncValueConverter<bool, double>(dim => dim ? 0.45 : 1.0);

		/// <summary>One ListView item, as shown by a view.</summary>
		public sealed class ListItem
		{
			public string Text { get; init; }
			/// <summary>The item's position in the ListView (a checkbox in the row writes its state back by it).</summary>
			public int Index { get; init; }
			/// <summary>The item's group header (ListView groups), or null.</summary>
			public string Group { get; init; }
			/// <summary>The item's second and third columns (SubItems 1 and 2), for list views with columns.</summary>
			public string Sub1 { get; init; }
			public string Sub2 { get; init; }
			public object Tag { get; init; }
			/// <summary>The item's other columns (SubItems 1 and up), joined for a second line or column.</summary>
			public string SubText { get; init; }
			public bool Checked { get; init; }
			public bool Hot { get; init; }
			/// <summary>Upstream colours it with the highlight colour (Undo/Redo's current level, or the target being picked).</summary>
			public bool Highlight { get; init; }
			/// <summary>Upstream greys it out (GrayText), e.g. Undo/Redo's redo levels.</summary>
			public bool Dim { get; init; }
		}

		/// <summary>
		/// A ListView's items. A ListBox view also selects: its selection sets the model items'
		/// Selected (upstream reads SelectedIndices), and a double-click raises the model's DoubleClick.
		/// </summary>
		public ModelBinder Items(ItemsControl target, WF.ListView list)
		{
			bool updating = false, initialraised = false;
			if(target is ListBox box)
			{
				box.SelectionMode = list.MultiSelect ? SelectionMode.Multiple : SelectionMode.Single;
				box.SelectionChanged += (s, e) =>
				{
					if(updating) return;
					for(int i = 0; i < list.Items.Count; i++) list.Items[i].Selected = box.Selection.IsSelected(i);
					list.RaiseSelectedIndexChanged();
				};
				// A checkbox in a row (ListView.CheckBoxes) sets the model item's Checked.
				box.AddHandler(Avalonia.Controls.Primitives.ToggleButton.IsCheckedChangedEvent, (s, e) =>
				{
					if(updating || !(e.Source is CheckBox cb) || !(cb.DataContext is ListItem row)) return;
					if(row.Index >= 0 && row.Index < list.Items.Count) list.Items[row.Index].Checked = cb.IsChecked == true;
				});
				// A TextBox in a row edits the label (ListView.LabelEdit): committed when it loses the focus.
				box.AddHandler(Avalonia.Input.InputElement.LostFocusEvent, (s, e) =>
				{
					if(!list.LabelEdit || updating || !(e.Source is TextBox tb) || !(tb.DataContext is ListItem row)) return;
					if(row.Index >= 0 && row.Index < list.Items.Count && tb.Text != row.Text) { list.EndLabelEdit(list.Items[row.Index], tb.Text); Schedule(s, e); }
				});
				box.KeyUp += (s, e) => list.RaiseKeyUp(new WF.KeyEventArgs(WF.Keys.None));
				box.DoubleTapped += (s, e) => { if(box.SelectedIndex >= 0) { list.RaiseDoubleClick(); Schedule(s, e); } };
				// Upstream acts on MouseUp (Undo/Redo performs the undo there), after the selection changed.
				box.AddHandler(Avalonia.Input.InputElement.PointerReleasedEvent, (s, e) =>
				{
					WF.MouseButtons button = e.InitialPressMouseButton == Avalonia.Input.MouseButton.Right ? WF.MouseButtons.Right : WF.MouseButtons.Left;
					list.RaiseMouseUp(new WF.MouseEventArgs(button, 1, 4, Math.Max(0, box.SelectedIndex) * 22 + 11, 0));
					Schedule(s, e);
				}, Avalonia.Interactivity.RoutingStrategies.Bubble, true);
			}
			string shown = null;
			return Custom(() =>
			{
				List<ListItem> items = list.Items.Select((i, index) => new ListItem
				{
					Text = i.Text,
					Index = index,
					Group = i.Group?.Header,
					Sub1 = i.SubItems.Count > 1 ? i.SubItems[1].Text : null,
					Sub2 = i.SubItems.Count > 2 ? i.SubItems[2].Text : null,
					Tag = i.Tag,
					SubText = string.Join(" · ", i.SubItems.Skip(1).Select(si => si.Text)),
					Checked = i.Checked,
					Hot = i.ForeColor.IsKnownColor && i.ForeColor.ToKnownColor() == SD.KnownColor.HotTrack,
					Dim = i.ForeColor.IsKnownColor && (i.ForeColor.ToKnownColor() == SD.KnownColor.GrayText || i.ForeColor.ToKnownColor() == SD.KnownColor.InactiveCaptionText),
					Highlight = i.BackColor.IsKnownColor && i.BackColor.ToKnownColor() == SD.KnownColor.Highlight,
				}).ToList();
				// Only when something changed: replacing the items resets the selection and scrolling.
				string sig = string.Join("\n", items.Select(i => i.Text + "\t" + i.SubText + "\t" + i.Checked + i.Hot + i.Dim + i.Highlight));
				if(sig != shown)
				{
					shown = sig;
					updating = true;
					target.ItemsSource = items;
					if(target is ListBox lb)
						for(int i = 0; i < list.Items.Count; i++) if(list.Items[i].Selected) lb.Selection.Select(i);
					updating = false;
					// As WinForms raises SelectedIndexChanged when the control is created with items selected.
					if(!initialraised && list.Items.Any(i => i.Selected))
					{
						initialraised = true;
						Dispatcher.UIThread.Post(list.RaiseSelectedIndexChanged, DispatcherPriority.Background);
					}
				}
				// Selection set by upstream's code (SelectedIndices.Clear(), Selected = ...) reaches the view.
				if(target is ListBox sel)
				{
					bool differs = false;
					for(int i = 0; i < list.Items.Count; i++) if(list.Items[i].Selected != sel.Selection.IsSelected(i)) { differs = true; break; }
					if(differs)
					{
						updating = true;
						sel.Selection.Clear();
						for(int i = 0; i < list.Items.Count; i++) if(list.Items[i].Selected) sel.Selection.Select(i);
						updating = false;
					}
				}
				if(target is ListBox scroll && list.EnsureVisibleItem != null)
				{
					int index = list.EnsureVisibleItem.Index;
					list.EnsureVisibleItem = null;
					if(index >= 0) scroll.ScrollIntoView(index);
				}
				target.IsVisible = list.Visible;
			}, list);
		}

		private bool HasField(string name)
		{
			for(Type t = model.GetType(); t != null; t = t.BaseType)
				if(t.GetField(name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly) != null) return true;
			return false;
		}

		/// <summary>Applies every binding now.</summary>
		public void UpdateNow()
		{
			pending = false;
			foreach(Action update in updates) update();
			Updated?.Invoke();
		}

		private void Schedule(object sender, EventArgs e)
		{
			if(pending) return;
			pending = true;
			Dispatcher.UIThread.Post(UpdateNow, DispatcherPriority.Render);
		}

		/// <summary>Upstream's text colours, mapped to the theme: normal text keeps the theme's
		/// colour, HotTrack (highlighted) gets class uldb-hot, reds get uldb-red (Styles.axaml).</summary>
		public static void SetForeground(TextBlock target, SD.Color c)
		{
			bool hot = c.IsKnownColor && c.ToKnownColor() == SD.KnownColor.HotTrack;
			bool red = !c.IsEmpty && !c.IsKnownColor && c.R > 0x7F && c.G < 0x60 && c.B < 0x60 || c == SD.Color.Red || c == SD.Color.DarkRed;
			target.Classes.Set("uldb-hot", hot);
			target.Classes.Set("uldb-red", red);
			if(hot || red || c.IsEmpty || IsThemeText(c)) target.ClearValue(TextBlock.ForegroundProperty);
			else target.Foreground = new SolidColorBrush(Color.FromArgb(c.A, c.R, c.G, c.B));
		}

		private static bool IsThemeText(SD.Color c)
		{
			if(!c.IsKnownColor) return false;
			switch(c.ToKnownColor())
			{
				case SD.KnownColor.ControlText:
				case SD.KnownColor.WindowText:
				case SD.KnownColor.GrayText:
				case SD.KnownColor.InfoText:
				case SD.KnownColor.MenuText:
					return true;
				default:
					return false;
			}
		}
	}
}
