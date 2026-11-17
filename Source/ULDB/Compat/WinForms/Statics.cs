// ULDB compatibility layer: WinForms static services (MessageBox, Cursor, Clipboard,
// Application, Help, Timer, TextRenderer), all routed through WinFormsHost.Current.

using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Threading;

namespace System.Windows.Forms
{
	public static class MessageBox
	{
		public static DialogResult Show(string text) => Show(null, text, "", MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
		public static DialogResult Show(string text, string caption) => Show(null, text, caption, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
		public static DialogResult Show(string text, string caption, MessageBoxButtons buttons) => Show(null, text, caption, buttons, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
		public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon) => Show(null, text, caption, buttons, icon, MessageBoxDefaultButton.Button1);
		public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton) => Show(null, text, caption, buttons, icon, defaultButton);
		public static DialogResult Show(IWin32Window owner, string text) => Show(owner, text, "", MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
		public static DialogResult Show(IWin32Window owner, string text, string caption) => Show(owner, text, caption, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
		public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons) => Show(owner, text, caption, buttons, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
		public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon) => Show(owner, text, caption, buttons, icon, MessageBoxDefaultButton.Button1);

		public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton)
		{
			return WinFormsHost.Current.ShowMessageBox(owner, text ?? "", caption ?? "", buttons, icon, defaultButton);
		}

		public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, MessageBoxOptions options)
		{
			return Show(owner, text, caption, buttons, icon, defaultButton);
		}
	}

	/// <summary>A cursor shape. ULDB cursors are named shapes the UI maps to Avalonia cursors.</summary>
	public sealed class Cursor : IDisposable
	{
		private static Cursor current;

		internal Cursor(string name) { Name = name; }

		public string Name { get; }

		public static Cursor Current
		{
			get => current ?? Cursors.Default;
			set
			{
				current = value;
				WinFormsHost.Current.SetCursor(value ?? Cursors.Default);
			}
		}

		/// <summary>Pointer position in screen coordinates. Set by the UI as the pointer moves.</summary>
		public static Point Position
		{
			get => PositionProvider?.Invoke() ?? Point.Empty;
			set => PositionSetter?.Invoke(value);
		}

		/// <summary>Installed by the UI to report and warp the pointer position.</summary>
		public static Func<Point> PositionProvider { get; set; }
		public static Action<Point> PositionSetter { get; set; }

		private static Rectangle clip;

		/// <summary>Confines the pointer to a screen rectangle (Empty: no limit). UDB uses it for 3D mouselook.</summary>
		public static Rectangle Clip
		{
			get => clip;
			set { clip = value; ClipSetter?.Invoke(value); }
		}

		public static void Hide() => VisibilitySetter?.Invoke(false);
		public static void Show() => VisibilitySetter?.Invoke(true);

		/// <summary>Installed by the UI: confine the pointer, and hide or show it.</summary>
		public static Action<Rectangle> ClipSetter { get; set; }
		public static Action<bool> VisibilitySetter { get; set; }

		public override string ToString() => "[Cursor: " + Name + "]";
		public void Dispose() { }
	}

	/// <summary>A display. Its geometry comes from the UI (BoundsProvider); without one, a 1920x1080 screen.</summary>
	public sealed class Screen
	{
		private Screen(Rectangle bounds, Rectangle workingarea, bool primary)
		{
			Bounds = bounds;
			WorkingArea = workingarea;
			Primary = primary;
		}

		public Rectangle Bounds { get; }
		public Rectangle WorkingArea { get; }
		public bool Primary { get; }
		public string DeviceName => Primary ? "Primary" : "Screen";

		/// <summary>ULDB: installed by the UI. Returns (bounds, working area) of the screen showing the control (null: primary).</summary>
		public static Func<Control, (Rectangle bounds, Rectangle workingArea)> BoundsProvider { get; set; }

		private static Screen For(Control c)
		{
			(Rectangle b, Rectangle w) = BoundsProvider?.Invoke(c) ?? (new Rectangle(0, 0, 1920, 1080), new Rectangle(0, 0, 1920, 1080));
			return new Screen(b, w, c == null);
		}

		public static Screen PrimaryScreen => For(null);
		public static Screen[] AllScreens => new[] { PrimaryScreen };
		public static Screen FromControl(Control control) => For(control);
		public static Rectangle GetWorkingArea(Control ctl) => For(ctl).WorkingArea;
		public static Rectangle GetBounds(Control ctl) => For(ctl).Bounds;
		public static Rectangle GetWorkingArea(Point pt) => PrimaryScreen.WorkingArea;
		public static Rectangle GetBounds(Point pt) => PrimaryScreen.Bounds;
	}

	/// <summary>Desktop metrics upstream asks for. The virtual screen comes from Screen (the UI).</summary>
	public static class SystemInformation
	{
		public static Rectangle VirtualScreen
		{
			get
			{
				Rectangle r = Rectangle.Empty;
				foreach(Screen s in Screen.AllScreens) r = r.IsEmpty ? s.Bounds : Rectangle.Union(r, s.Bounds);
				return r;
			}
		}

		public static int CaretBlinkTime => 530;
		public static int VerticalScrollBarWidth => 14;
		public static int HorizontalScrollBarHeight => 14;
		public static Size PrimaryMonitorSize => Screen.PrimaryScreen.Bounds.Size;
		public static Size DoubleClickSize => new Size(4, 4);
		public static int DoubleClickTime => 500;
	}

	public static class Cursors
	{
		public static Cursor Default { get; } = new Cursor("Default");
		public static Cursor Arrow { get; } = new Cursor("Arrow");
		public static Cursor WaitCursor { get; } = new Cursor("WaitCursor");
		public static Cursor AppStarting { get; } = new Cursor("AppStarting");
		public static Cursor Cross { get; } = new Cursor("Cross");
		public static Cursor Hand { get; } = new Cursor("Hand");
		public static Cursor IBeam { get; } = new Cursor("IBeam");
		public static Cursor No { get; } = new Cursor("No");
		public static Cursor SizeAll { get; } = new Cursor("SizeAll");
		public static Cursor SizeNESW { get; } = new Cursor("SizeNESW");
		public static Cursor SizeNS { get; } = new Cursor("SizeNS");
		public static Cursor SizeNWSE { get; } = new Cursor("SizeNWSE");
		public static Cursor SizeWE { get; } = new Cursor("SizeWE");
		public static Cursor Help { get; } = new Cursor("Help");
	}

	public static class Clipboard
	{
		public static void SetDataObject(object data) => SetDataObject(data, true);
		public static void SetDataObject(object data, bool copy) => SetDataObject(data, copy, 0, 0);

		public static void SetDataObject(object data, bool copy, int retryTimes, int retryDelay)
		{
			DataObject obj = data as DataObject ?? new DataObject(DataFormats.Text, data?.ToString() ?? "");
			WinFormsHost.Current.SetClipboard(obj);
		}

		public static void SetText(string text) => WinFormsHost.Current.SetClipboard(new DataObject(DataFormats.Text, text));
		public static string GetText() => WinFormsHost.Current.GetClipboard()?.GetData(DataFormats.Text) as string ?? "";
		public static bool ContainsText() => WinFormsHost.Current.GetClipboard()?.GetDataPresent(DataFormats.Text) ?? false;
		public static bool ContainsData(string format) => WinFormsHost.Current.GetClipboard()?.GetDataPresent(format) ?? false;
		public static object GetData(string format) => WinFormsHost.Current.GetClipboard()?.GetData(format);
		public static void Clear() => WinFormsHost.Current.SetClipboard(new DataObject());
	}

	public static class Application
	{
		/// <summary>Set by the application to brand message boxes etc. (ULDB.Branding).</summary>
		public static string ProductNameOverride { get; set; }

		public static string ProductName
		{
			get
			{
				if(!string.IsNullOrEmpty(ProductNameOverride)) return ProductNameOverride;
				Assembly asm = Assembly.GetEntryAssembly();
				return asm?.GetCustomAttribute<AssemblyProductAttribute>()?.Product ?? asm?.GetName().Name ?? "Ultimate Linux Doom Builder";
			}
		}

		public static string ProductVersion
		{
			get
			{
				Assembly asm = Assembly.GetEntryAssembly();
				return asm?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? asm?.GetName().Version?.ToString() ?? "0.0";
			}
		}

		// The program's own folder, even when started as "dotnet Builder.dll" (where the
		// process path is the dotnet host). UDB finds Configurations/ etc. next to it.
		public static string StartupPath => AppContext.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar);
		public static string ExecutablePath => Path.Combine(StartupPath, (Assembly.GetEntryAssembly()?.GetName().Name ?? "Builder") + ".dll");

		/// <summary>Raised for exceptions on the UI thread. The UI forwards its unhandled exceptions here.</summary>
		public static event ThreadExceptionEventHandler ThreadException;

		/// <summary>Installed by the UI: runs the main loop with the given main window until it closes.</summary>
		public static Action<object> RunHandler { get; set; }

		/// <summary>Installed by the UI: ends the main loop.</summary>
		public static System.Action ExitHandler { get; set; }

		public static void Run(object mainWindow)
		{
			if(RunHandler == null) throw new InvalidOperationException("No UI is installed (Application.RunHandler is not set)");
			RunHandler(mainWindow);
		}

		public static void Exit() => ExitHandler?.Invoke();
		public static void DoEvents() => WinFormsHost.Current.DoEvents();

		// WinForms setup calls with no Avalonia equivalent.
		public static void EnableVisualStyles() { }
		public static void SetCompatibleTextRenderingDefault(bool defaultValue) { }

		/// <summary>Lets the UI raise ThreadException for exceptions it catches.</summary>
		public static bool RaiseThreadException(Exception e)
		{
			ThreadExceptionEventHandler handler = ThreadException;
			if(handler == null) return false;
			handler(null, new ThreadExceptionEventArgs(e));
			return true;
		}

		public static FormCollection OpenForms { get; } = new FormCollection();
	}

	public class FormCollection : Collections.ObjectModel.Collection<Form> { }

	public static class Help
	{
		public static void ShowHelp(Control parent, string url) => WinFormsHost.Current.ShowHelp(parent, url, null);
		public static void ShowHelp(Control parent, string url, string keyword) => WinFormsHost.Current.ShowHelp(parent, url, keyword);
		public static void ShowHelp(Control parent, string url, HelpNavigator navigator, object param) => WinFormsHost.Current.ShowHelp(parent, url, param?.ToString());
		public static void ShowHelp(IWin32Window parent, string url, HelpNavigator navigator, object param) => WinFormsHost.Current.ShowHelp(parent, url, param?.ToString());
	}

	public static class TextRenderer
	{
		public static Size MeasureText(string text, Font font)
		{
			SizeF size;
			using(Bitmap bmp = new Bitmap(1, 1))
			using(Graphics g = Graphics.FromImage(bmp))
				size = g.MeasureString(text, font);
			return new Size((int)Math.Ceiling(size.Width), (int)Math.Ceiling(size.Height));
		}
	}

	/// <summary>A UI-thread timer, like WinForms' Timer.</summary>
	public class Timer : IDisposable
	{
		private IDisposable running;
		private int interval = 100;

		public Timer() { }
		public Timer(ComponentModel.IContainer container) { }

		public event EventHandler Tick;

		public int Interval
		{
			get => interval;
			set
			{
				if(value <= 0) throw new ArgumentOutOfRangeException(nameof(value), "Interval must be positive");
				interval = value;
				if(running != null) { Stop(); Start(); }
			}
		}

		public bool Enabled
		{
			get => running != null;
			set { if(value) Start(); else Stop(); }
		}

		public object Tag { get; set; }

		/// <summary>While true no timer ticks (set during a map load that repaints the UI for progress: WinForms never ran timers inside a blocking call).</summary>
		public static bool Suspended;

		public void Start()
		{
			if(running != null) return;
			running = WinFormsHost.Current.StartTimer(interval, () => { if(!Suspended) Tick?.Invoke(this, EventArgs.Empty); });
		}

		public void Stop()
		{
			running?.Dispose();
			running = null;
		}

		public void Dispose() => Stop();
	}
}
