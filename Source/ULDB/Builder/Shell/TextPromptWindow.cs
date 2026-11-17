// ULDB: a small "type a line of text" window (WinForms code has no equivalent in the BCL; upstream used
// a text box inside a drop-down menu, which Avalonia's menus can't host: they take the keyboard for themselves).

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Layout;

namespace ULDB.Shell
{
	internal sealed class TextPromptWindow : Window
	{
		private readonly TextBox box;
		private bool accepted;

		private TextPromptWindow(string title, string message, string hint, string current)
		{
			Title = title;
			SizeToContent = SizeToContent.Height;
			Width = 520;
			CanResize = false;
			ShowInTaskbar = false;
			WindowStartupLocation = WindowStartupLocation.CenterOwner;

			box = new TextBox { Text = current ?? "", PlaceholderText = hint, MinWidth = 400 };
			Button ok = new Button { Content = "OK", MinWidth = 80, IsDefault = true, HorizontalContentAlignment = HorizontalAlignment.Center };
			ok.Classes.Add("accent");
			ok.Click += (s, e) => { accepted = true; Close(); };
			Button cancel = new Button { Content = "Cancel", MinWidth = 80, IsCancel = true, HorizontalContentAlignment = HorizontalAlignment.Center };
			cancel.Click += (s, e) => Close();

			StackPanel buttons = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 8, HorizontalAlignment = HorizontalAlignment.Right };
			buttons.Children.Add(ok);
			buttons.Children.Add(cancel);
			Border footer = new Border { Padding = new Thickness(20, 12), Child = buttons };
			footer.Classes.Add("uldb-dialog-footer");

			StackPanel body = new StackPanel { Margin = new Thickness(20, 18, 20, 14), Spacing = 10 };
			body.Children.Add(new TextBlock { Text = message, TextWrapping = Avalonia.Media.TextWrapping.Wrap });
			body.Children.Add(box);

			DockPanel root = new DockPanel();
			DockPanel.SetDock(footer, Dock.Bottom);
			root.Children.Add(footer);
			root.Children.Add(body);
			Content = root;
			Opened += (s, e) => { box.Focus(); box.SelectAll(); };
		}

		/// <summary>Asks for a line of text; null when cancelled.</summary>
		public static string Ask(string title, string message, string hint, string current)
		{
			TextPromptWindow window = new TextPromptWindow(title, message, hint, current);
			Window owner = (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
			Modal.ShowDialog(window, owner);
			return window.accepted ? window.box.Text ?? "" : null;
		}
	}
}
