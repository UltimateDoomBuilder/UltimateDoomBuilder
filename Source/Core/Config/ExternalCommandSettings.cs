#region ================== Copyright (c) 2021 Boris Iwanski

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

using CodeImp.DoomBuilder.IO;

#endregion

namespace CodeImp.DoomBuilder.Config
{
	public class ExternalCommandSettings
	{
		#region ================== Variables

		private string workingdirectory;
		private string commands;
		private bool autocloseonsuccess;
		private bool exitcodeiserror;
		private bool stderriserror;

		#endregion

		#region ================== Properties

		public string WorkingDirectory { get { return workingdirectory; } set { workingdirectory = value; } }
		public string Commands { get { return commands; } set { commands = value; } }
		public bool AutoCloseOnSuccess { get { return autocloseonsuccess; } set { autocloseonsuccess = value; } }
		public bool ExitCodeIsError { get { return exitcodeiserror; } set { exitcodeiserror = value; } }
		public bool StdErrIsError { get { return stderriserror; } set { stderriserror = value; } }

		#endregion

		#region ================== Constructors

		public ExternalCommandSettings()
		{
			WorkingDirectory = string.Empty;
			Commands = string.Empty;
			AutoCloseOnSuccess = true;
			ExitCodeIsError = true;
			StdErrIsError = true;
		}

		/// <summary>
		/// Initialize with the settings loaded from a given section in a configuration.
		/// </summary>
		/// <param name="cfg">The configuration</param>
		/// <param name="section">The section to load the settings from</param>
		public ExternalCommandSettings(Configuration cfg, string section)
		{
			LoadSettings(cfg, section);
		}

		#endregion

		#region ================== Methods

		/// <summary>
		/// Loads the settings from a given section in a configuration.
		/// </summary>
		/// <param name="cfg">The configuration</param>
		/// <param name="section">The section to load the settings from</param>
		public void LoadSettings(Configuration cfg, string section)
		{
			WorkingDirectory = cfg.ReadSetting(section + ".workingdirectory", string.Empty);
			Commands = cfg.ReadSetting(section + ".commands", string.Empty);
			AutoCloseOnSuccess = cfg.ReadSetting(section + ".autocloseonsuccess", true);
			ExitCodeIsError = cfg.ReadSetting(section + ".exitcodeiserror", true);
			StdErrIsError = cfg.ReadSetting(section + ".stderriserror", true);
		}

		/// <summary>
		/// Writes the settings to a given section in a configuration.
		/// </summary>
		/// <param name="cfg">The configuration</param>
		/// <param name="section">The section to write the settings to</param>
		public void WriteSettings(Configuration cfg, string section)
		{
			if (!string.IsNullOrWhiteSpace(Commands))
				cfg.WriteSetting(section + ".commands", Commands);
			else
				cfg.DeleteSetting(section + ".commands");

			if (!string.IsNullOrWhiteSpace(WorkingDirectory))
				cfg.WriteSetting(section + ".workingdirectory", WorkingDirectory);
			else
				cfg.DeleteSetting(section + ".workingdirectory");

			cfg.WriteSetting(section + ".autocloseonsuccess", AutoCloseOnSuccess);
			cfg.WriteSetting(section + ".exitcodeiserror", ExitCodeIsError);
			cfg.WriteSetting(section + ".stderriserror", StdErrIsError);
		}

		#endregion
	}
}