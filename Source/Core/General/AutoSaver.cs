#region ================== Copyright (c) 2023 Boris Iwanski

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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeImp.DoomBuilder
{
	internal enum AutosaveResult
	{
		Success,
		Error,
		NoFileName
	}

	internal class AutoSaver
	{
		private static long lasttime;
		private static System.Windows.Forms.Timer timer;

		/// <summary>
		/// Initialized and starts the autosave timer.
		/// </summary>
		internal void InitializeTimer()
		{
			if(timer != null)
			{
				timer.Tick -= TryAutosave;
				timer.Dispose();
				timer = null;
			}

			if (General.Settings.Autosave)
			{
				lasttime = Clock.CurrentTime;
				timer = new System.Windows.Forms.Timer() { Interval = 1000 };
				timer.Tick += TryAutosave;
				timer.Enabled = true;
			}
		}

		/// <summary>
		/// Stops the autosave timer.
		/// </summary>
		internal void StopTimer()
		{
			if (timer != null) timer.Enabled = false;
		}

		/// <summary>
		/// Resets the autosave timer to the current time.
		/// </summary>
		internal void ResetTimer()
		{
			lasttime = Clock.CurrentTime;
		}

		/// <summary>
		/// Makes the autosave timer aware of a clock reset, so that the interval until the next autosave is unaffected.
		/// </summary>
		internal void BeforeClockReset()
		{
			lasttime = -(Clock.CurrentTime - lasttime);
		}

		/// <summary>
		/// Tries to perform the autosave.
		/// </summary>
		/// <param name="sender">The sender</param>
		/// <param name="args">The event arguments</param>
		private static void TryAutosave(object sender, EventArgs args)
		{
			if (Clock.CurrentTime > lasttime + General.Settings.AutosaveInterval * 60 * 1000 && General.Map != null && General.Map.Map != null && General.Map.Map.IsSafeToAccess && General.Map.IsChanged)
			{
				// Check if the current editing mode prevents autosaving. If it does return without setting the time,
				// so that autosaving will be retried ASAP
				if (!General.Editing.Mode.OnAutoSaveBegin())
					return;

				lasttime = Clock.CurrentTime;

				long start = Clock.CurrentTime;
				AutosaveResult success = General.Map.AutoSave();
				long duration = Clock.CurrentTime - start;

				// Show a toast appropriate for the result of the autosave
				if (success == AutosaveResult.Success)
					General.ToastManager.ShowToast("autosave", ToastType.INFO, "Autosave", $"Autosave completed successfully in {duration} ms.");
				else if (success == AutosaveResult.Error)
					General.ToastManager.ShowToast("autosave", ToastType.ERROR, "Autosave", "Autosave failed.");
				else if (success == AutosaveResult.NoFileName)
					General.ToastManager.ShowToast("autosave", ToastType.WARNING, "Autosave", "Could not autosave because this is a new WAD that wasn't saved yet.");
			}
		}
	}
}
