// ULDB: "busy" notice for long synchronous work on the UI thread (opening a map). WinForms
// repainted the status bar and showed the wait cursor while upstream code ran; Avalonia only paints
// when the UI thread returns to its loop, so Begin shows a "Loading…" overlay on the main window and
// renders one frame before the work starts. It is removed again when the wait cursor is reset.

using System;

namespace ULDB.Platform
{
	public static class Busy
	{
		internal static Action<string> Show;
		internal static Action Hide;
		internal static Action<string> Step;

		/// <summary>Shows the overlay and renders it once; call before blocking work.</summary>
		public static void Begin(string message) { System.Windows.Forms.Timer.Suspended = true; Show?.Invoke(message); }

		/// <summary>Shows the current step (a log line) while the overlay is up; repaints at most every 40 ms.</summary>
		public static void Update(string step) { Step?.Invoke(step); }

		/// <summary>Removes the overlay (also done when the cursor returns to default).</summary>
		public static void End() { System.Windows.Forms.Timer.Suspended = false; Hide?.Invoke(); }
	}
}
