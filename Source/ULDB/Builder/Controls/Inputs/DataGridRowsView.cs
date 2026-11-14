// ULDB: a plain view of compat's DataGridView model, for upstream grids that just list rows
// (Filter Selected Things: type, title, count). A header row and one text row per grid row; row
// selection goes both ways, so upstream reads Rows[i].Selected as it does in WinForms. Grids with
// editing or custom drawing have their own views (FieldsEditorView, the Comments docker).

using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;
using WF = System.Windows.Forms;

namespace ULDB.Controls
{
	internal sealed class DataGridRowsView : UserControl
	{
		private readonly WF.DataGridView grid;
		private readonly ListBox list;
		private readonly Grid header;
		private readonly TextBox filterbox;
		private List<int> shown = new List<int>(); // the grid row behind each list row (a text filter hides rows)
		private bool updating, pending;

		public DataGridRowsView(WF.DataGridView grid)
		{
			this.grid = grid;
			header = new Grid { Margin = new Thickness(8, 4) };
			list = new ListBox { SelectionMode = SelectionMode.Multiple };
			list.ItemTemplate = new FuncDataTemplate<List<string>>((cells, _) => cells == null ? null : BuildRow(cells, false), true);
			list.SelectionChanged += (s, e) =>
			{
				if(updating) return;
				// Rows the filter hides keep the selection they had
				for(int i = 0; i < shown.Count; i++) grid.Rows[shown[i]].SetSelectedFromView(list.Selection.IsSelected(i));
			};
			// A long list (the thing types of a big map) can be narrowed by typing part of any column (upstream issue #1138)
			filterbox = new TextBox { Watermark = "Type to filter the list", Margin = new Thickness(0, 0, 0, 6), IsVisible = false };
			filterbox.TextChanged += (s, e) => Schedule();

			DockPanel panel = new DockPanel();
			DockPanel.SetDock(filterbox, Dock.Top);
			panel.Children.Add(filterbox);
			DockPanel.SetDock(header, Dock.Top);
			panel.Children.Add(header);
			panel.Children.Add(list);
			Content = new Border { Child = panel, BorderThickness = new Thickness(1), CornerRadius = new CornerRadius(3), ClipToBounds = true, Classes = { "uldb-grid" } };

			grid.GridChanged += (s, e) => Schedule();
			Schedule();
		}

		private void Schedule()
		{
			if(pending) return;
			pending = true;
			Dispatcher.UIThread.Post(Rebuild, DispatcherPriority.Render);
		}

		private void Rebuild()
		{
			pending = false;
			List<WF.DataGridViewColumn> columns = grid.Columns.Cast<WF.DataGridViewColumn>().Where(c => c.Visible).ToList();
			header.ColumnDefinitions.Clear();
			header.Children.Clear();
			for(int i = 0; i < columns.Count; i++)
			{
				header.ColumnDefinitions.Add(Definition(i, columns.Count));
				TextBlock t = new TextBlock { Text = columns[i].HeaderText, FontWeight = FontWeight.SemiBold, Opacity = 0.75 };
				Grid.SetColumn(t, i);
				header.Children.Add(t);
			}
			header.IsVisible = grid.ColumnHeadersVisible;

			List<List<string>> all = Enumerable.Range(0, grid.Rows.Count)
				.Select(r => columns.Select(c => grid.Rows[r].Cells[c.Index].Value?.ToString() ?? "").ToList()).ToList();
			filterbox.IsVisible = grid.Rows.Count > 12;
			string filter = (filterbox.IsVisible ? filterbox.Text : null)?.Trim() ?? "";
			shown = Enumerable.Range(0, all.Count)
				.Where(r => filter.Length == 0 || all[r].Any(cell => cell.IndexOf(filter, StringComparison.CurrentCultureIgnoreCase) >= 0)).ToList();

			updating = true;
			list.ItemsSource = shown.Select(r => all[r]).ToList();
			list.Selection.Clear();
			for(int i = 0; i < shown.Count; i++) if(grid.Rows[shown[i]].Selected) list.Selection.Select(i);
			updating = false;
		}

		// The last column takes the rest; the others are as wide as the designer made them (scaled down).
		private ColumnDefinition Definition(int index, int count) =>
			new ColumnDefinition(index == count - 1 ? GridLength.Star : new GridLength(Math.Max(48, grid.Columns[index].Width * 0.9)));

		private Control BuildRow(List<string> cells, bool head)
		{
			Grid g = new Grid();
			for(int i = 0; i < cells.Count; i++)
			{
				g.ColumnDefinitions.Add(Definition(i, cells.Count));
				TextBlock t = new TextBlock { Text = cells[i], TextTrimming = TextTrimming.CharacterEllipsis, VerticalAlignment = VerticalAlignment.Center };
				Grid.SetColumn(t, i);
				g.Children.Add(t);
			}
			return g;
		}
	}
}
