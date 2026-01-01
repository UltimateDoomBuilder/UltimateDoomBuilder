#region ================== Copyright (c) 2020 Boris Iwanski

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
using System.Collections.Generic;
using System.Dynamic;
using System.Windows.Forms;

#endregion

namespace CodeImp.DoomBuilder.UDBScript
{
	/// <summary>
	/// The `QueryOptions` class is used to query the user for their input. It effectively works the same as specifying script options in the script's metadata, except that the `QueryOptions` class works at run-time.
	/// 
	/// Example:
	/// ```js
	/// let qo = new QueryOptions();
	/// qo.addOption('length', 'Length of the sides', 0, 128);
	/// qo.addOption('numsides', 'Number of sides', 0, 5);
	/// qo.addOption('direction', 'Direction to go', 11, 1, { 1: 'Up', 2: 'Down' }); // Enumeration
	/// qo.query();
	/// 
	/// showMessage('You want ' + qo.options.numsides + ' sides with a length of ' + qo.options.length);
	/// ```
	/// </summary>
	class QueryOptions
	{
		#region ================== Variables

		private QueryOptionsForm form;

		#endregion

		#region ================== Properties

		/// <summary>
		/// Object containing all the added options as properties.
		/// </summary>
		public ExpandoObject options
		{
			get
			{
				return form.GetScriptOptions();
			}
		}

		#endregion

		#region ================== Constructor

		/// <summary>
		/// Initializes a new `QueryOptions` object.
		/// </summary>
		public QueryOptions()
		{
			form = new QueryOptionsForm();
		}

		#endregion

		#region ================== Methods

		/// <summary>
		/// Adds a parameter to query
		/// </summary>
		/// <param name="name">Name of the variable that the queried value is stored in</param>
		/// <param name="description">Textual description of the parameter</param>
		/// <param name="type">UniversalType value of the parameter</param>
		/// <param name="defaultvalue">Default value of the parameter</param>
		public void addOption(string name, string description, int type, object defaultvalue)
		{
			if (Array.FindIndex(ScriptOption.ValidTypes, t => (int)t == type) == -1)
			{
				string errordesc = "Error in script " + BuilderPlug.Me.CurrentScriptFile + ": option " + name + " has invalid type " + type;
				General.ErrorLogger.Add(ErrorType.Error, errordesc);
				General.WriteLogLine(errordesc);
				return;
			}

			form.AddOption(name, description, type, defaultvalue);
		}

		/// <summary>
		/// Adds a parameter to query
		/// </summary>
		/// <param name="name">Name of the variable that the queried value is stored in</param>
		/// <param name="description">Textual description of the parameter</param>
		/// <param name="type">UniversalType value of the parameter</param>
		/// <param name="defaultvalue">Default value of the parameter</param>
		public void addOption(string name, string description, int type, object defaultvalue, object enumvalues)
		{
			if (Array.FindIndex(ScriptOption.ValidTypes, t => (int)t == type) == -1)
			{
				string errordesc = "Error in script " + BuilderPlug.Me.CurrentScriptFile + ": option " + name + " has invalid type " + type;
				General.ErrorLogger.Add(ErrorType.Error, errordesc);
				General.WriteLogLine(errordesc);
				return;
			}

			if (enumvalues is Dictionary<string, object>)
				form.AddOption(name, description, type, defaultvalue, (Dictionary<string, object>)enumvalues);
			else if (enumvalues is ExpandoObject)
			{
				Dictionary<string, object> values = new Dictionary<string, object>();

				foreach(KeyValuePair<string, object> kvp in (ExpandoObject)enumvalues)
				{
					values[kvp.Key] = kvp.Value;
				}

				form.AddOption(name, description, type, defaultvalue, values);
			}
		}

		/// <summary>
		/// Removes all parameters
		/// </summary>
		public void clear()
		{
			form.Clear();
		}

		/// <summary>
		/// Queries all parameters. Options a window where the user can enter values for the options added through `addOption()`.
		/// </summary>
		/// <returns>True if OK was pressed, otherwise false</returns>
		public bool query()
		{
			return (bool)BuilderPlug.Me.ScriptRunnerForm.InvokePaused(new Func<bool>(() =>
			{
				DialogResult dr = form.ShowDialog();
				return dr == DialogResult.OK;
			}));
		}

		#endregion
	}
}
