// ULDB: the real IWinFormsHost. Upstream's MessageBox, file and colour dialogs,
// clipboard, cursors, timers and cross-thread calls land here (through the compat
// layer) and are answered with Avalonia.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using DrawingColor = System.Drawing.Color;
using Avalonia.Input.Platform;
using DataFormats = System.Windows.Forms.DataFormats;
using DataObject = System.Windows.Forms.DataObject;
using WfCursor = System.Windows.Forms.Cursor;

namespace ULDB.Shell
{
	internal sealed class AvaloniaHost : IWinFormsHost
	{
		private DataObject clipboard = new DataObject();
		private Avalonia.Input.Cursor cursor;
		private System.Threading.CancellationTokenSource mainloop;

		/// <summary>Installs the host and the main loop handlers. Call once Avalonia is set up.</summary>
		public static void Install()
		{
			AvaloniaHost host = new AvaloniaHost();
			WinFormsHost.Current = host;
			FormWindows.TrackOpenWindows();
			FormViews.Install();
			ToolStripViews.InstallContextMenus();

			System.Windows.Forms.Application.RunHandler = host.Run;
			System.Windows.Forms.Application.ExitHandler = host.Exit;

			// Closing a form closes its window (whose Closing asks the form); a form
			// without a window is just a model.
			Form.CloseHandler = f =>
			{
				Window w = FormWindows.WindowFor(f);
				if(w != null) w.Close();
				else f.RequestClose(CloseReason.UserClosing);
			};

			// Exceptions on the UI thread go to upstream's handler (its error dialog), when
			// it installed one; otherwise they crash the program as usual.
			Dispatcher.UIThread.UnhandledException += (s, e) =>
			{
				ReportException(e.Exception);
				e.Handled = true;
			};
		}

		/// <summary>
		/// An exception escaped upstream code on the UI thread (an event handler or timer tick).
		/// Like WinForms: upstream's ThreadException handler shows it when installed (release
		/// builds); otherwise it's logged and reported, and the editor keeps running.
		/// </summary>
		private static void ReportException(Exception ex)
		{
			try { CodeImp.DoomBuilder.General.WriteLogLine("Unhandled exception on the UI thread: " + ex); }
			catch(Exception) { Console.Error.WriteLine(ex); }
			if(System.Windows.Forms.Application.RaiseThreadException(ex)) return;
			CodeImp.DoomBuilder.General.ErrorLogger?.Add(CodeImp.DoomBuilder.ErrorType.Error, ex.GetType().Name + ": " + ex.Message);
		}

		#region ================== Main loop

		private void Run(object mainwindow)
		{
			Form form = mainwindow as Form;
			Window window = FormWindows.WindowFor(form);

			if(window == null && form is CodeImp.DoomBuilder.Windows.MainForm mainform)
			{
				MainWindow main = new MainWindow(mainform);
				FormWindows.Register(form, main);
				main.Load();
				window = main;
			}
			if(window == null) throw new InvalidOperationException("No window for " + mainwindow);

			mainloop = new System.Threading.CancellationTokenSource();
			window.Closed += (s, e) => mainloop.Cancel();
			if(!window.IsVisible) window.Show();
			Dispatcher.UIThread.MainLoop(mainloop.Token);
		}

		private void Exit()
		{
			mainloop?.Cancel();
		}

		#endregion

		#region ================== Dialogs

		private static readonly System.Runtime.CompilerServices.ConditionalWeakTable<Window, object> hooked = new System.Runtime.CompilerServices.ConditionalWeakTable<Window, object>();

		/// <summary>
		/// A dialog taller than the screen leaves its OK/Cancel buttons out of reach (upstream issue #1051: the Linedef dialog
		/// at 1280x720). Sizes and minimum sizes are cut to the screen's working area, and the pages of a tabbed dialog
		/// scroll, while the buttons stay pinned below them.
		/// </summary>
		private static void FitToScreen(Window window, Window owner)
		{
			Avalonia.Platform.Screen screen = null;
			try { screen = (owner ?? window).Screens?.ScreenFromVisual(owner ?? window) ?? (owner ?? window).Screens?.Primary; }
			catch(Exception) { }
			if(screen != null)
			{
				double maxw = Math.Max(400, screen.WorkingArea.Width / screen.Scaling - 24);
				double maxh = Math.Max(300, screen.WorkingArea.Height / screen.Scaling - 56);
				if(!double.IsNaN(window.MinHeight) && window.MinHeight > maxh) window.MinHeight = maxh;
				if(!double.IsNaN(window.MinWidth) && window.MinWidth > maxw) window.MinWidth = maxw;
				if(!double.IsNaN(window.Height) && window.Height > maxh) window.Height = maxh;
				if(!double.IsNaN(window.Width) && window.Width > maxw) window.Width = maxw;
				window.MaxHeight = maxh;
				window.MaxWidth = maxw;
			}
			foreach(Avalonia.Controls.TabControl tabs in Avalonia.LogicalTree.LogicalExtensions.GetLogicalDescendants(window).OfType<Avalonia.Controls.TabControl>().ToList())
				foreach(Avalonia.Controls.TabItem item in tabs.Items.OfType<Avalonia.Controls.TabItem>())
					if(item.Content is Avalonia.Controls.Control content && !(content is Avalonia.Controls.ScrollViewer))
					{
						item.Content = null; // free the page before it goes into the ScrollViewer
						item.Content = new Avalonia.Controls.ScrollViewer { Content = content, HorizontalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Disabled };
					}
		}

		public void ShowForm(Form form, object view, IWin32Window owner, bool modal)
		{
			OnUIThread(() =>
			{
				Window window = (Window)view;
				Window ownerwindow = OwnerWindow(owner);
				double scale = ownerwindow?.RenderScaling ?? 1.0;

				// A form shown again (upstream keeps some modeless forms and Hide()s/Show()s them):
				// the window is set up once; showing it again just shows it.
				if(hooked.TryGetValue(window, out object _))
				{
					if(modal) { if(!window.IsVisible) Modal.ShowDialog(window, ownerwindow); }
					else if(!window.IsVisible) { if(ownerwindow != null && ownerwindow.IsVisible) window.Show(ownerwindow); else window.Show(); }
					window.Activate();
					return;
				}
				hooked.Add(window, new object());
				FormWindows.Register(form, window);

				// Title follows the form's Text.
				if(!string.IsNullOrEmpty(form.Text)) window.Title = form.Text;
				form.TextChanged += (s, e) => Dispatcher.UIThread.Post(() => window.Title = form.Text);

				// Load first (upstream's DelayedForm restores the saved size and position), then apply it.
				form.RaiseLoad();
				bool sizable = form.FormBorderStyle == FormBorderStyle.Sizable || form.FormBorderStyle == FormBorderStyle.SizableToolWindow;
				if(sizable && form.Width > 0 && form.Height > 0)
				{
					window.Width = form.Width / scale;
					window.Height = form.Height / scale;
				}
				window.CanResize = window.CanResize && sizable;
				window.CanMinimize = false; // a minimised dialog leaves the main window blocked and looking frozen (issue #17)
				FitToScreen(window, ownerwindow);
				// A dialog that has never had a saved position opens centred on the main window: upstream's
				// DelayedForm works out "centred" from the form's size, which isn't the window's size here.
				bool savedposition = CodeImp.DoomBuilder.General.Settings.ReadSetting("windows." + form.GetType().Name.ToLowerInvariant() + ".positionx", int.MaxValue) != int.MaxValue;
				if(ownerwindow != null && !savedposition && form is CodeImp.DoomBuilder.Windows.DelayedForm)
				{
					window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
				}
				else if(form.StartPosition == FormStartPosition.Manual || form.Location != System.Drawing.Point.Empty)
				{
					window.WindowStartupLocation = WindowStartupLocation.Manual;
					window.Position = new Avalonia.PixelPoint(form.Location.X, form.Location.Y);
				}
				else
				{
					window.WindowStartupLocation = ownerwindow != null ? WindowStartupLocation.CenterOwner : WindowStartupLocation.CenterScreen;
				}

				window.Opened += (s, e) =>
				{
					window.Activate(); // dialogs take the focus (KDE may otherwise leave it on the main window)
					form.RaiseShown();
					DevCapture.Schedule(window, TimeSpan.FromSeconds(1.5), form);
					// Development aid: ULDB_AUTOCLOSE=<ms> also closes modal windows, for scripted sweeps.
					if(modal && int.TryParse(Environment.GetEnvironmentVariable("ULDB_AUTOCLOSE"), out int autoclose) && autoclose > 0)
						DispatcherTimer.RunOnce(window.Close, TimeSpan.FromMilliseconds(autoclose));
				};
				window.Activated += (s, e) => form.RaiseActivated();
				window.Deactivated += (s, e) => form.RaiseDeactivate();

				// Closing the window (title bar, Escape, Close()) asks the form, like WinForms.
				bool closed = false, closing = false;

				// WinForms: Hide() (Visible = false) hides the window. Not while it's closing: closing sets Visible itself.
				form.VisibleChanged += (s, e) =>
				{
					if(!closing && !closed && !form.Visible && window.IsVisible) window.Hide();
				};

				window.Closing += (s, e) =>
				{
					if(closed) return;
					closing = true;
					double sc = window.RenderScaling;
					form.Location = new System.Drawing.Point(window.Position.X, window.Position.Y);
					form.Size = new System.Drawing.Size((int)(window.Bounds.Width * sc), (int)(window.Bounds.Height * sc));
					form.RaiseResizeEnd();
					try
					{
						if(!form.RequestClose(CloseReason.UserClosing))
						{
							e.Cancel = true;
							// A form that answers a close by hiding itself (Stair Sector Builder cancels its mode, which hides the
								// form) has had its Hide() ignored while closing: hide the window now (issue #32).
							if(!form.Visible) Dispatcher.UIThread.Post(() => { if(window.IsVisible) window.Hide(); });
						}
						else closed = true;
					}
					finally { closing = false; }
				};

				if(modal) Modal.ShowDialog(window, ownerwindow);
				else if(ownerwindow != null) window.Show(ownerwindow);
				else window.Show();
			});
		}

		public DialogResult ShowMessageBox(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton)
		{
			return OnUIThread(() =>
			{
				MessageBoxWindow box = new MessageBoxWindow(text, caption, buttons, icon, defaultButton);
				Modal.ShowDialog(box, OwnerWindow(owner));
				return box.Result;
			});
		}

		public DrawingColor? ShowColorDialog(IWin32Window owner, DrawingColor initial, int[] customColors)
		{
			return OnUIThread<DrawingColor?>(() =>
			{
				ColorDialogWindow dialog = new ColorDialogWindow(initial);
				Modal.ShowDialog(dialog, OwnerWindow(owner));
				return dialog.Accepted ? dialog.Color : null;
			});
		}

		public string[] ShowFileDialog(IWin32Window owner, FileDialogRequest request)
		{
			return OnUIThread(() =>
			{
				TopLevel top = OwnerWindow(owner);
				if(top == null) return null; // No window to attach the picker to yet
				return Modal.Wait(ShowFileDialogAsync(top.StorageProvider, request));
			});
		}

		private static async Task<string[]> ShowFileDialogAsync(IStorageProvider storage, FileDialogRequest request)
		{
			IStorageFolder start = null;
			if(!string.IsNullOrEmpty(request.InitialDirectory) && System.IO.Directory.Exists(request.InitialDirectory))
				start = await storage.TryGetFolderFromPathAsync(new Uri(System.IO.Path.GetFullPath(request.InitialDirectory)));
			else if(!string.IsNullOrEmpty(request.FileName) && System.IO.Path.IsPathRooted(request.FileName))
			{
				string dir = System.IO.Path.GetDirectoryName(request.FileName);
				if(System.IO.Directory.Exists(dir)) start = await storage.TryGetFolderFromPathAsync(new Uri(dir));
			}

			if(request.SelectFolder)
			{
				IReadOnlyList<IStorageFolder> folders = await storage.OpenFolderPickerAsync(new FolderPickerOpenOptions
				{
					Title = request.Title,
					SuggestedStartLocation = start,
					AllowMultiple = request.Multiselect,
				});
				return LocalPaths(folders);
			}

			List<FilePickerFileType> types = request.ParseFilter()
				.Select(f => new FilePickerFileType(f.Key) { Patterns = f.Value })
				.ToList();

			// WinForms' FilterIndex is 1-based and picks the initially selected filter. The
			// portal shows the first one first, so move the chosen filter to the front.
			int index = request.FilterIndex - 1;
			if(index > 0 && index < types.Count)
			{
				FilePickerFileType chosen = types[index];
				types.RemoveAt(index);
				types.Insert(0, chosen);
			}

			if(request.Save)
			{
				IStorageFile file = await storage.SaveFilePickerAsync(new FilePickerSaveOptions
				{
					Title = request.Title,
					SuggestedStartLocation = start,
					SuggestedFileName = string.IsNullOrEmpty(request.FileName) ? null : System.IO.Path.GetFileName(request.FileName),
					DefaultExtension = request.DefaultExt,
					FileTypeChoices = types,
					ShowOverwritePrompt = request.OverwritePrompt,
				});
				string path = file?.TryGetLocalPath();
				if(path == null) return null;

				// WinForms adds the default extension when the name has none.
				if(!string.IsNullOrEmpty(request.DefaultExt) && string.IsNullOrEmpty(System.IO.Path.GetExtension(path)))
					path += "." + request.DefaultExt.TrimStart('.');
				return new[] { path };
			}

			IReadOnlyList<IStorageFile> files = await storage.OpenFilePickerAsync(new FilePickerOpenOptions
			{
				Title = request.Title,
				SuggestedStartLocation = start,
				SuggestedFileName = string.IsNullOrEmpty(request.FileName) ? null : System.IO.Path.GetFileName(request.FileName),
				AllowMultiple = request.Multiselect,
				FileTypeFilter = types,
			});
			return LocalPaths(files);
		}

		private static string[] LocalPaths(IEnumerable<IStorageItem> items)
		{
			string[] paths = items?.Select(i => i.TryGetLocalPath()).Where(p => p != null).ToArray();
			return paths == null || paths.Length == 0 ? null : paths;
		}

		public void ShowHelp(IWin32Window owner, string url, string topic)
		{
			// Upstream's help is a Windows .chm file; ULDB opens the online documentation.
			if(string.IsNullOrEmpty(url) || !url.Contains("://")) return;
			try { Process.Start(new ProcessStartInfo(url) { UseShellExecute = true }); }
			catch(Exception e) { Trace.WriteLine("Unable to open " + url + ": " + e.Message); }
		}

		private static Window OwnerWindow(IWin32Window owner)
		{
			Window w = FormWindows.WindowFor(owner);
			if(w != null && w.IsVisible) return w;
			return FormWindows.Active;
		}

		#endregion

		#region ================== Cursor and clipboard

		public void SetCursor(WfCursor value)
		{
			OnUIThread(() =>
			{
				cursor = value == null || value == Cursors.Default || value == Cursors.Arrow ? null : new Avalonia.Input.Cursor(CursorType(value));
				foreach(Window w in FormWindows.Open) w.Cursor = cursor;
			});
		}

		private static StandardCursorType CursorType(WfCursor c)
		{
			switch(c.Name)
			{
				case "WaitCursor": return StandardCursorType.Wait;
				case "AppStarting": return StandardCursorType.AppStarting;
				case "Cross": return StandardCursorType.Cross;
				case "Hand": return StandardCursorType.Hand;
				case "IBeam": return StandardCursorType.Ibeam;
				case "No": return StandardCursorType.No;
				case "SizeAll": return StandardCursorType.SizeAll;
				case "SizeNESW": return StandardCursorType.BottomLeftCorner;
				case "SizeNS": return StandardCursorType.SizeNorthSouth;
				case "SizeNWSE": return StandardCursorType.TopLeftCorner;
				case "SizeWE": return StandardCursorType.SizeWestEast;
				case "Help": return StandardCursorType.Help;
				default: return StandardCursorType.Arrow;
			}
		}

		// UDB's own formats (copied geometry, prefabs) stay in this process; text also goes
		// to the desktop clipboard, so it can be pasted into other programs and back.
		public void SetClipboard(DataObject data)
		{
			clipboard = data ?? new DataObject();
			string text = clipboard.GetData(DataFormats.Text) as string ?? clipboard.GetData(DataFormats.UnicodeText) as string;
			OnUIThread(() =>
			{
				IClipboard system = FormWindows.Active?.Clipboard;
				if(system == null) return;
				if(text != null) system.SetTextAsync(text);
				else system.ClearAsync();
			});
		}

		public DataObject GetClipboard()
		{
			return OnUIThread(() =>
			{
				IClipboard system = FormWindows.Active?.Clipboard;
				if(system == null) return clipboard;

				string systemtext = Modal.Wait(system.TryGetTextAsync());
				string owntext = clipboard.GetData(DataFormats.Text) as string;
				if(systemtext == null || systemtext == owntext) return clipboard;

				// Something else was copied since: that's what pasting should use.
				return new DataObject(DataFormats.Text, systemtext);
			});
		}

		#endregion

		#region ================== Threads and timers

		public void DoEvents() => OnUIThread(() => Dispatcher.UIThread.RunJobs());
		public bool CheckAccess() => Dispatcher.UIThread.CheckAccess();
		public void Invoke(Action action) => Dispatcher.UIThread.Invoke(action);
		public void BeginInvoke(Action action) => Dispatcher.UIThread.Post(action);

		public IDisposable StartTimer(int interval, Action tick)
		{
			return OnUIThread(() =>
			{
				Stopper stopper = new Stopper();
				DispatcherTimer timer = new DispatcherTimer(TimeSpan.FromMilliseconds(interval), DispatcherPriority.Normal, (s, e) =>
				{
					if(stopper.Stopped) return;
					try { tick(); }
					catch(Exception ex) { ReportException(ex); }
				});
				stopper.Timer = timer;
				timer.Start();
				return stopper;
			});
		}

		// Like WinForms, Stop takes effect at once: upstream restarts timers from their own
		// Tick handlers (the status bar), so a late tick would start them again and again.
		private sealed class Stopper : IDisposable
		{
			public DispatcherTimer Timer;
			public volatile bool Stopped;

			public void Dispose()
			{
				Stopped = true;
				if(Dispatcher.UIThread.CheckAccess()) Timer.Stop();
				else Dispatcher.UIThread.Post(Timer.Stop);
			}
		}

		public Form ActiveForm => OnUIThread(() =>
		{
			foreach(Window w in FormWindows.Open) if(w.IsActive) return FormWindows.FormFor(w);
			return null;
		});

		private static T OnUIThread<T>(Func<T> f) => Dispatcher.UIThread.CheckAccess() ? f() : Dispatcher.UIThread.Invoke(f);
		private static void OnUIThread(Action a) { if(Dispatcher.UIThread.CheckAccess()) a(); else Dispatcher.UIThread.Invoke(a); }

		#endregion
	}
}
