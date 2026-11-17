// ULDB: development aid. With ULDB_CAPTURE=<file.png>, ULDB renders its own window (the dialog
// opened by ULDB_SHOW_FORM, else the main window) to that file a moment after it appears. This
// never captures the screen, so nothing else on the desktop can end up in the image.

using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Threading;

namespace ULDB.Shell
{
	internal static class DevCapture
	{
		private static readonly string path = Environment.GetEnvironmentVariable("ULDB_CAPTURE");
		private static readonly bool exitafter = Environment.GetEnvironmentVariable("ULDB_CAPTURE_EXIT") == "1";
		private static bool done;

		public static bool Enabled => !string.IsNullOrEmpty(path);

		/// <summary>
		/// Renders the window to ULDB_CAPTURE after the given delay (first call wins). The Errors window (missing
		/// resources in a test setup) is closed instead, so the window wanted is the one captured. With ULDB_CAPTURE_EXIT=1, ULDB exits after saving.
		/// </summary>
		public static void Schedule(Window window, TimeSpan delay, System.Windows.Forms.Form form = null)
		{
			// ULDB_CAPTURE_DELAY=<ms> waits longer (for steps that go on after a window opened)
			if(int.TryParse(Environment.GetEnvironmentVariable("ULDB_CAPTURE_DELAY"), out int extra) && extra > 0) delay = TimeSpan.FromMilliseconds(extra);
			if(!Enabled) return;
			// ULDB_CAPTURE_TITLE=<text>: only the window whose title contains it (modeless windows open later than the main one)
			string only = Environment.GetEnvironmentVariable("ULDB_CAPTURE_TITLE");
			if(!string.IsNullOrEmpty(only) && !(window.Title ?? "").Contains(only)) return;
			if(form is CodeImp.DoomBuilder.Windows.ErrorsForm)
			{
				Dispatcher.UIThread.Post(() => form.Close(), DispatcherPriority.Background);
				return;
			}
			DispatcherTimer.RunOnce(() =>
			{
				if(done) return;
				done = true;
				try
				{
					double scale = window.RenderScaling;
					PixelSize size = new PixelSize((int)(window.Bounds.Width * scale), (int)(window.Bounds.Height * scale));
					using(RenderTargetBitmap bitmap = new RenderTargetBitmap(size, new Vector(96 * scale, 96 * scale)))
					{
						bitmap.Render(window);
						bitmap.Save(path);
					}
					Console.WriteLine("[capture] saved " + path);
				}
				catch(Exception e) { Console.WriteLine("[capture] failed: " + e.Message); }
				if(exitafter) Environment.Exit(0);
			}, delay);
		}
	}
}
