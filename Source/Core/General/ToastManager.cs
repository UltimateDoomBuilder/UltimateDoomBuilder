#region ================== Copyright (c) 2022 Boris Iwanski

/*
 * This program is free software: you can redistribute it and/or modify
 *
 * it under the terms of the GNU General Public License as published by
 * 
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 *    but WITHOUT ANY WARRANTY; without even the implied warranty of
 * 
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
 * 
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.If not, see<http://www.gnu.org/licenses/>.
 */

#endregion

#region ================== Namespaces

using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Controls;
using CodeImp.DoomBuilder.IO;
using CodeImp.DoomBuilder.Windows;

#endregion

namespace CodeImp.DoomBuilder
{
	public enum ToastType
	{
		INFO,
		WARNING,
		ERROR
	}

	public enum ToastAnchor
	{
		TOPLEFT = 1,
		TOPRIGHT,
		BOTTOMRIGHT,
		BOTTOMLEFT
	}

	internal class ToastRegistryEntry
	{
		public bool Enabled { get; set; }
		public string Name { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }

		public ToastRegistryEntry(string name, string title, string description, bool enabled)
		{
			Enabled = enabled;
			Name = name;
			Title = title;
			Description = description;
		}
	}

	public class ToastManager
	{
		#region ================== Static variables

		public static readonly string TITLE_INFO = "Information";
		public static readonly string TITLE_WARNING = "Warning";
		public static readonly string TITLE_ERROR = "Error";

		#endregion

		#region ================== Variables

		private List<ToastControl> toasts;
		private Control bindcontrol;
		private Timer timer;
		private bool enabled;
		private ToastAnchor anchor;
		private long duration;
		private Dictionary<string, ToastRegistryEntry> registry;

		#endregion

		#region ================== Properties

		internal bool Enabled { get => enabled; set => enabled = value; }
		internal ToastAnchor Anchor { get => anchor; set => anchor = value; }
		internal long Duration { get => duration; set => duration = value; }
		internal Dictionary<string, ToastRegistryEntry> Registry { get => registry; }

		#endregion

		#region ================== Constructors

		public ToastManager(Control bindcontrol)
		{
			toasts = new List<ToastControl>();

			this.bindcontrol = bindcontrol;

			// Create the timer that will handle moving the toasts. Do not start it, though
			timer = new Timer();
			timer.Interval = 1; // Actually only called every 1/64 second, because Windows
			timer.Tick += UpdateEvent;

			// Create registry and load toasts from actions
			registry = new Dictionary<string, ToastRegistryEntry>();
		}



		#endregion

		#region ================== Events

		private void UpdateEvent(object sender, EventArgs args)
		{
			if (toasts.Count == 0)
				return;

			// Go through all toasts and check if they should decay or not. Remove toasts that reached their lifetime
			for (int i = toasts.Count - 1; i >= 0; i--)
			{
				toasts[i].CheckDecay();

				if (!toasts[i].IsAlive())
				{
					bindcontrol.Controls.Remove(toasts[i]);
					toasts[i].Dispose(); // Dispose, otherwise it'll leak
					toasts.RemoveAt(i);
				}
			}

			// No toasts left, so we should stop the timer
			if (toasts.Count == 0)
			{
				timer.Stop();
				return;
			}

			ToastControl ft = toasts[0];

			// We only need to update the first toasts if it didn't reach it end position yet
			bool needsupdate =
				((anchor == ToastAnchor.TOPLEFT || anchor == ToastAnchor.TOPRIGHT) && ft.Location.Y != ft.Margin.Top)
				||
				((anchor == ToastAnchor.BOTTOMLEFT || anchor == ToastAnchor.BOTTOMRIGHT) && ft.Location.Y != bindcontrol.Height - ft.Height - ft.Margin.Bottom)
			;

			if(needsupdate)
			{
				int left;
				int top;

				if (anchor == ToastAnchor.TOPLEFT || anchor == ToastAnchor.BOTTOMLEFT)
					left = ft.Margin.Right;
				else
					left = bindcontrol.Width - ft.Width - ft.Margin.Right;

				// This moves the toast up or down a bit, depending on its anchor position. How fast this happens depends on
				// the control's height, i.e. no matter the height a toast will always take the same time to slide in
				// TODO: make it dependent on elapsed time
				if (anchor == ToastAnchor.TOPLEFT || anchor == ToastAnchor.TOPRIGHT)
					top = ft.Location.Y + ft.Height / 5;
				else
					top = ft.Location.Y - ft.Height / 5;

				Point newLocation = new Point(left, top);

				// If the movement overshot the final position snap it back to the final position
				if ((anchor == ToastAnchor.BOTTOMLEFT || anchor == ToastAnchor.BOTTOMRIGHT) && newLocation.Y < bindcontrol.Height - ft.Height - ft.Margin.Bottom)
					newLocation.Y = bindcontrol.Height - ft.Height - ft.Margin.Bottom;
				else if ((anchor == ToastAnchor.TOPLEFT || anchor == ToastAnchor.TOPRIGHT) && newLocation.Y > ft.Margin.Top)
					newLocation.Y = ft.Margin.Top;

				ft.Location = newLocation;
			}

			if (toasts.Count > 1)
			{
				// Align all other toasts to their predecessor
				for (int i = 1; i < toasts.Count; i++)
				{
					int top;

					if (anchor == ToastAnchor.TOPLEFT || anchor == ToastAnchor.TOPRIGHT)
						top = toasts[i - 1].Bottom + toasts[i - 1].Margin.Bottom;
					else
						top = toasts[i - 1].Location.Y - toasts[i].Height - toasts[i].Margin.Bottom;

					toasts[i].Location = new Point(
						ft.Location.X,
						top
					);
				}
			}
		}

		#endregion

		#region ================== Methods

		public void LoadSettings(Configuration cfg)
		{
			enabled = cfg.ReadSetting("toasts.enabled", true);
			anchor = GetAnchorFromNumber(cfg.ReadSetting("toasts.anchor", 3));
			duration = cfg.ReadSetting("toasts.duration", 3000);

			// Make sure the duration is set to something sensible
			if (duration <= 0)
				duration = 3000;
 
			IDictionary toastactionenableddict = cfg.ReadSetting("toasts.registry", new Hashtable());
			foreach (string key in toastactionenableddict.Keys)
			{
				//string key = de.Key.ToString();

				if (registry.ContainsKey(key))
					registry[key].Enabled = cfg.ReadSetting($"toasts.registry.{key}", true);
			}
		}

		/// <summary>
		/// Writes the settings to a configuration with a prefix.
		/// </summary>
		/// <param name="cfg">The Configuration</param>
		/// <param name="prefix">The prefix</param>
		public void WriteSettings(Configuration cfg)
		{
			cfg.WriteSetting("toasts.enabled", Enabled);
			cfg.WriteSetting("toasts.anchor", (int)Anchor);
			cfg.WriteSetting("toasts.duration", Duration);

			foreach (string key in Registry.Keys)
			{
				// true is the default value, so we only need to save it if it's false
				if (Registry[key].Enabled == false)
					cfg.WriteSetting($"toasts.registry.{key}", false);
				else
					cfg.DeleteSetting($"toasts.registry.{key}");
			}
		}

		/// <summary>
		/// Registers toast from all defined actions.
		/// </summary>
		public void RegisterActions()
		{
			foreach (Actions.Action action in General.Actions.GetAllActions().Where(a => a.RegisterToast))
			{
				if (!registry.ContainsKey(action.Name))
					registry[action.Name] = new ToastRegistryEntry(action.Name, action.Title, action.Description, true);
			}
		}

		/// <summary>
		/// Registers a toast by name. Automatically prepends the assembly name.
		/// </summary>
		/// <param name="name">Name of the toast (without assembly)</param>
		/// <param name="title">Title to show in the toast preferences dialog</param>
		/// <param name="description">Description to show in the toast preferences dialog</param>
		[MethodImpl(MethodImplOptions.NoInlining)]
		public void RegisterToast(string name, string title, string description)
		{
			string fullname = Assembly.GetCallingAssembly().GetName().Name.ToLowerInvariant() + $"_{name}";

			if (registry.ContainsKey(fullname))
			{
				General.WriteLogLine($"Tried to register toast \"{fullname}\", but it is already registered");
				return;
			}

			registry[fullname] = new ToastRegistryEntry(fullname, title, description, true);
		}

		/// <summary>
		/// Gets the ToastAnchor from a number. Makes sure the input is valid, otherwise returns a default.
		/// </summary>
		/// <param name="number">The number</param>
		/// <returns>The appropriate ToastAnchor, or BOTTOMRIGHT if input is not valid</returns>
		public static ToastAnchor GetAnchorFromNumber(int number)
		{
			return Enum.IsDefined(typeof(ToastAnchor), number) ? (ToastAnchor)number : ToastAnchor.BOTTOMRIGHT;
		}

		/// <summary>
		/// Shows a new toast.
		/// </summary>
		/// <param name="type">Toast type</param>
		/// <param name="message">The message body of the toast</param>
		public void ShowToast(ToastType type, string title, string message, string shortmessage = "")
		{
			StatusType st = type == ToastType.INFO ? StatusType.Info : StatusType.Warning;

			if (!enabled)
			{
				General.Interface.DisplayStatus(new StatusInfo(st, shortmessage));
				return;
			}

			if (type == ToastType.WARNING)
				title = "Warning";
			else if (type == ToastType.ERROR)
				title = "Error";

			CreateToast(type, title, message);
		}

		/// <summary>
		/// Shows a new toast. Deducts the title from the type.
		/// </summary>
		/// <param name="name">Name of the toast</param>
		/// <param name="type">Type of the toast</param>
		/// <param name="message">Message to show</param>
		/// <param name="statusinfo">StatusInfo to use when toasts are disabled</param>
		[MethodImpl(MethodImplOptions.NoInlining)]
		public void ShowToast(string name, ToastType type, string message, StatusInfo statusinfo)
		{
			string fullname = Assembly.GetCallingAssembly().GetName().Name.ToLowerInvariant() + $"_{name}";
			string title = "Information";

			if (type == ToastType.WARNING)
				title = "Warning";
			else if (type == ToastType.ERROR)
				title = "Error";

			CreateToast(fullname, type, title, message, statusinfo);
		}

		/// <summary>
		/// Shows a new toast.
		/// </summary>
		/// <param name="name">Name of the toast</param>
		/// <param name="type">Type of the toast</param>
		/// <param name="title">Title to show</param>
		/// <param name="message">Message to show</param>
		/// <param name="statusinfo">StatusInfo to use when toasts are disabled</param>
		[MethodImpl(MethodImplOptions.NoInlining)]
		public void ShowToast(string name, ToastType type, string title, string message, StatusInfo statusinfo)
		{
			string fullname = Assembly.GetCallingAssembly().GetName().Name.ToLowerInvariant() + $"_{name}";

			CreateToast(fullname, type, title, message, statusinfo);
		}

		/// <summary>
		/// Shows a new toast.
		/// </summary>
		/// <param name="name">Name of the toast</param>
		/// <param name="type">Type of the toast</param>
		/// <param name="title">Title to show</param>
		/// <param name="message">Message to show</param>
		/// <param name="shortmessage">Message to show in the status bar if toasts are disabled. Should not include line breaks</param>
		[MethodImpl(MethodImplOptions.NoInlining)]
		public void ShowToast(string name, ToastType type, string title, string message, string shortmessage = null)
		{
			string fullname = Assembly.GetCallingAssembly().GetName().Name.ToLowerInvariant() + $"_{name}";
			StatusType st = type == ToastType.INFO ? StatusType.Info : StatusType.Warning;

			if (string.IsNullOrWhiteSpace(shortmessage))
				shortmessage = message;

			CreateToast(fullname, type, title, message, new StatusInfo(st, shortmessage));
		}

		/// <summary>
		/// Creates a toast.
		/// </summary>
		/// <param name="fullname">Full name (i.e. assembly and toast name) of the toast</param>
		/// <param name="type">Type of the toast</param>
		/// <param name="title">Title to show</param>
		/// <param name="message">Message to show</param>
		/// <param name="statusinfo">StatusInfo to use when toasts are disabled</param>
		private void CreateToast(string fullname, ToastType type, string title, string message, StatusInfo statusinfo)
		{
			if (!enabled || registry[fullname]?.Enabled == false)
			{
				General.Interface.DisplayStatus(statusinfo);
				return;
			}

			if (!registry.ContainsKey(fullname))
			{
				General.ErrorLogger.Add(ErrorType.Warning, $"Toast setting for \"{fullname}\" is not in the registry. Defaulting to show the toast.");
			}
			else if (registry[fullname].Enabled == false)
			{
				General.Interface.DisplayStatus(statusinfo);
				return;
			}

			CreateToast(type, title, message);
		}

		/// <summary>
		/// Creates a toast.
		/// </summary>
		/// <param name="type">Type of the toast</param>
		/// <param name="title">Title to show</param>
		/// <param name="message">Message to show</param>
		private void CreateToast(ToastType type, string title, string message)
		{ 
			ToastControl tc = new ToastControl(type, title, message, duration);

			// Set the initial y position of the control so that it's outside of the control the toast manager is bound to.
			// No need to care about the x position, since that will be set in the update event anyway
			if (anchor == ToastAnchor.TOPLEFT || anchor == ToastAnchor.TOPRIGHT)
				tc.Location = new Point(0, -tc.Height);
			else
				tc.Location = new Point(0, bindcontrol.Height);

			toasts.Insert(0, tc);
			bindcontrol.Controls.Add(tc);

			// Need to set the toast to be at the front, otherwise the new control would be behind the control the toast manager
			// is bound to
			bindcontrol.Controls.SetChildIndex(tc, 0);

			// Start the timer so that the toast is moved into view
			if (!timer.Enabled)
				timer.Start();

			// Play a sound for warnings and errors
			if (type == ToastType.WARNING)
				General.MessageBeep(MessageBeepType.Warning);
			else if (type == ToastType.ERROR)
				General.MessageBeep(MessageBeepType.Error);
		}

		#endregion
	}
}
