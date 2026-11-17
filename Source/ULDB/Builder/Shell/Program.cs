// ULDB: program entry point. Sets up Avalonia, installs the UI services the compat
// layer forwards to (WinFormsHost, Application.Run/Exit), then hands over to upstream's
// own startup, General.Main, unchanged.

using System;
using System.IO;
using System.Runtime.InteropServices;
using Avalonia;
using CodeImp.DoomBuilder;

namespace ULDB.Shell
{
	internal static class Program
	{
		[STAThread]
		public static int Main(string[] args)
		{
			// Errors nobody awaited (a background task failing) are logged instead of vanishing.
			System.Threading.Tasks.TaskScheduler.UnobservedTaskException += (s, e) =>
			{
				try { General.WriteLogLine("Unobserved task exception: " + e.Exception); } catch(Exception) { }
				e.SetObserved();
			};

			try
			{
				return Start(args);
			}
			catch(Exception ex)
			{
				// Anything that fails before upstream's own error dialog exists (no display, a broken
				// installation, a bad settings file): leave a report, and say where it is.
				string path = ReportStartupCrash(ex);
				Console.Error.WriteLine(Branding.NAME + " could not start:" + Environment.NewLine + ex);
				if(path != null) Console.Error.WriteLine(Environment.NewLine + "A report was written to " + path);
				return 1;
			}
		}

		private static int Start(string[] args)
		{
			// Windows code pages (upstream's script editor reads scripts as Windows-1251, as UDB did).
			System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

			// The interface scale chosen in Preferences (upstream issue #1146: too small on a 2K Linux display). Avalonia reads
			// its global scale from the environment when it starts, before the program configuration is loaded.
			if(string.IsNullOrEmpty(Environment.GetEnvironmentVariable("AVALONIA_GLOBAL_SCALE_FACTOR")))
			{
				// As General does: with -portable the settings are next to the program, when that folder can be written to
				string programdir = AppContext.BaseDirectory;
				bool portable = Array.Exists(args, x => string.Equals(x, "-portable", StringComparison.OrdinalIgnoreCase)) && ULDB.Platform.Paths.IsDirectoryWritable(programdir);
				int percent = ULDB.Platform.UiOptions.ScaleFromFile(Path.Combine(portable ? programdir : ULDB.Platform.Paths.SettingsDirectory, "UDBuilder.cfg"));
				if(percent > 0) Environment.SetEnvironmentVariable("AVALONIA_GLOBAL_SCALE_FACTOR", (percent / 100.0).ToString(System.Globalization.CultureInfo.InvariantCulture));
			}

			BuildAvaloniaApp().SetupWithoutStarting();
			AvaloniaHost.Install();

			// First run: offer to import existing UDB settings before upstream loads (and creates) ours.
			// Also handles "--import-udb-settings[=PATH]", which upstream's argument parser mustn't see.
			args = ULDB.Settings.UdbSettingsImport.RunAtStartup(args);

			// Builds MainForm, loads configurations and plugins, then calls Application.Run,
			// which runs the Avalonia main loop until the main window closes.
			General.Main(args);
			return 0;
		}

		/// <summary>Writes a startup failure to the settings folder (UDBCrash.txt) and returns its path, or null.</summary>
		private static string ReportStartupCrash(Exception ex)
		{
			try
			{
				string dir = ULDB.Platform.Paths.SettingsDirectory;
				Directory.CreateDirectory(dir);
				string path = Path.Combine(dir, "UDBCrash.txt");
				File.WriteAllText(path,
					"***********STARTUP FAILURE***********" + Environment.NewLine +
					"OS: " + RuntimeInformation.OSDescription + " (" + RuntimeInformation.OSArchitecture + ")" + Environment.NewLine +
					"Desktop: " + Environment.GetEnvironmentVariable("XDG_CURRENT_DESKTOP") + " (" + Environment.GetEnvironmentVariable("XDG_SESSION_TYPE") + ")" + Environment.NewLine +
					"Runtime: " + RuntimeInformation.FrameworkDescription + Environment.NewLine + Environment.NewLine +
					ex);
				return path;
			}
			catch(Exception) { return null; }
		}

		public static AppBuilder BuildAvaloniaApp()
		{
			return AppBuilder.Configure<App>()
				.UsePlatformDetect()
				.With(X11Options())
				.LogToTrace();
		}

		// Development aid: ULDB_RENDERING=glx|egl|software forces Avalonia's X11 rendering mode
		// (headless Xvfb sessions otherwise fall back to software, which has no map view).
		private static X11PlatformOptions X11Options()
		{
			X11PlatformOptions options = new X11PlatformOptions { WmClass = Branding.EXECUTABLE };
			switch(Environment.GetEnvironmentVariable("ULDB_RENDERING")?.ToLowerInvariant())
			{
				case "glx": options.RenderingMode = new[] { X11RenderingMode.Glx }; break;
				case "egl": options.RenderingMode = new[] { X11RenderingMode.Egl }; break;
				case "software": options.RenderingMode = new[] { X11RenderingMode.Software }; break;
			}
			return options;
		}
	}
}
