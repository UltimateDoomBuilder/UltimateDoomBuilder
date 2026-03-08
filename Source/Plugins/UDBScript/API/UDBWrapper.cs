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
using System.Dynamic;
using System.Windows.Forms;
using Jint;
using Jint.Runtime.Interop;

#endregion

namespace CodeImp.DoomBuilder.UDBScript.Wrapper
{
	internal class UDBWrapper
	{
		#region ================== Variables

		private GameConfigurationWrapper gameconfiguration;
		private TypeReference queryoptions;
		private ExpandoObject scriptoptions;

		private TypeReference angle2d;
		private DataWrapper data;
		private TypeReference line2d;
		private MapWrapper map;
		private TypeReference univalue;
		private TypeReference vector2d;
		private TypeReference vector3d;

		private TypeReference linedef;
		private TypeReference sector;
		private TypeReference sidedef;
		private TypeReference thing;
		private TypeReference vertex;

		// Version 5
		private TypeReference plane;
		private TypeReference blockmap;

		private IProgress<int> progress;
		private IProgress<string> status;
		private IProgress<string> logger;

		#endregion

		#region ================== Properties

		/// <summary>
		/// Class containing methods related to the game configuration. See [GameConfiguration](GameConfiguration.md) for more information.
		/// </summary>
		public GameConfigurationWrapper GameConfiguration
		{
			get
			{
				return gameconfiguration;
			}
		}

		/// <summary>
		/// Class containing methods and properties related to querying options from the user at runtime. See [QueryOptions](QueryOptions.md) for more information.
		/// </summary>
		public TypeReference QueryOptions
		{
			get
			{
				return queryoptions;
			}
		}

		/// <summary>
		/// Object containing the script options. See [Setting script options](gettingstarted.md#setting-script-options).
		/// </summary>
		public ExpandoObject ScriptOptions
		{
			get
			{
				return scriptoptions;
			}
		}

		/// <summary>
		/// Class containing methods related to angles. See [Angle2D](Angle2D.md) for more information.
		/// ```js
		/// let rad = UDB.Angle2D.degToRad(46);
		/// ```
		/// </summary>
		public TypeReference Angle2D
		{
			get
			{
				return angle2d;
			}
		}

		/// <summary>
		/// Class containing methods related to the game data. See [Data](Data.md) for more information.
		/// ```js
		/// let hasfireblu = UDB.Data.textureExists('FIREBLU1');
		/// ```
		/// </summary>
		public DataWrapper Data
		{
			get
			{
				return data;
			}
		}

		/// <summary>
		/// Instantiable class that contains methods related to two-dimensional lines. See [Line2D](Line2D.md) for more information.
		/// ```js
		/// let line = new UDB.Line2D([ 32, 64 ], [ 96, 128 ]);
		/// ```
		/// </summary>
		public TypeReference Line2D
		{
			get
			{
				return line2d;
			}
		}

		/// <summary>
		/// Object containing methods related to the map. See [Map](Map.md) for more information.
		/// ```js
		/// let sectors = UDB.Map.getSelectedOrHighlightedSectors();
		/// ```
		/// </summary>
		public MapWrapper Map
		{
			get
			{
				return map;
			}
		}

		/// <summary>
		/// The `UniValue` class. Is only needed when trying to assign integer values to UDMF fields.
		/// ```js
		/// s.fields.user_myintfield = new UDB.UniValue(0, 25);
		/// ```
		/// </summary>
		public TypeReference UniValue
		{
			get
			{
				return univalue;
			}
		}

		/// <summary>
		/// Instantiable class that contains methods related to two-dimensional vectors. See [Vector2D](Vector2D.md) for more information.
		/// ```js
		/// let v = new UDB.Vector2D(32, 64);
		/// ```
		/// </summary>
		public TypeReference Vector2D
		{
			get
			{
				return vector2d;
			}
		}

		/// <summary>
		/// Instantiable class that contains methods related to three-dimensional vectors. See [Vector3D](Vector3D.md) for more information.
		/// ```js
		/// let v = new UDB.Vector3D(32, 64, 128);
		/// ```
		/// </summary>
		public TypeReference Vector3D
		{
			get
			{
				return vector3d;
			}
		}

		public TypeReference Linedef { get { return linedef; } }
		public TypeReference Sector { get { return sector; } }
		public TypeReference Sidedef { get { return sidedef; } }
		public TypeReference Thing { get { return thing; } }
		public TypeReference Vertex { get { return vertex; } }

		/// <summary>
		/// Instantiable class that contains methods related to a three-dimensional Plane. See [Plane](Plane.md) for more information.
		/// </summary>
		[UDBScriptSettings(MinVersion = 5)]
		public TypeReference Plane
		{
			get
			{
				return plane;
			}
		}

		/// <summary>
		/// Instantiable class that contains methods related to blockmaps. See [BlockMap][BlockMap.md) for more information.
		/// </summary>
		[UDBScriptSettings(MinVersion = 5)]
		public TypeReference BlockMap
		{
			get
			{
				return blockmap;
			}
		}

		#endregion

		#region ================== Constructors

		internal UDBWrapper(Engine engine, ScriptInfo scriptinfo, IProgress<int> progress, IProgress<string> status, IProgress<string> logger)
		{
			gameconfiguration = new GameConfigurationWrapper();
			queryoptions = TypeReference.CreateTypeReference(engine, typeof(QueryOptions));
			scriptoptions = scriptinfo.GetScriptOptionsObject();

			angle2d = TypeReference.CreateTypeReference(engine, typeof(Angle2DWrapper));
			data = new DataWrapper();
			line2d = TypeReference.CreateTypeReference(engine, typeof(Line2DWrapper));
			map = new MapWrapper();
			univalue = TypeReference.CreateTypeReference(engine, typeof(CodeImp.DoomBuilder.Map.UniValue));
			vector2d = TypeReference.CreateTypeReference(engine, typeof(Vector2DWrapper));
			vector3d = TypeReference.CreateTypeReference(engine, typeof(Vector3DWrapper));

			// These can not be directly instanciated and don't have static method, but it's required to
			// for example use "instanceof" in scripts
			linedef = TypeReference.CreateTypeReference(engine, typeof(LinedefWrapper));
			sector = TypeReference.CreateTypeReference(engine, typeof(SectorWrapper));
			sidedef = TypeReference.CreateTypeReference(engine, typeof(SidedefWrapper));
			thing = TypeReference.CreateTypeReference(engine, typeof(ThingWrapper));
			vertex = TypeReference.CreateTypeReference(engine, typeof(VertexWrapper));

			// Version 5
			plane = TypeReference.CreateTypeReference(engine, typeof(PlaneWrapper));
			blockmap = TypeReference.CreateTypeReference(engine, typeof(BlockMapWrapper));

			this.progress = progress;
			this.status = status;
			this.logger = logger;
		}

		#endregion

		#region ================== Methods

		/// <summary>
		/// Set the progress of the script in percent. Value can be between 0 and 100. Also shows the script running dialog.
		/// </summary>
		/// <param name="value">Number between 0 and 100</param>
		public void setProgress(int value)
		{
			progress.Report(value);
		}

		/*
		public void setStatus(string text)
		{
			status.Report(text);
		}
		*/

		/// <summary>
		/// Adds a line to the script log. Also shows the script running dialog.
		/// </summary>
		/// <param name="text">Line to add to the script log</param>
		public void log(object text)
		{
			if (text == null)
				return;

			logger.Report(text.ToString());
		}

		/// <summary>
		/// Shows a message box with an "OK" button.
		/// </summary>
		/// <param name="message">Message to show</param>
		public void showMessage(object message)
		{
			BuilderPlug.Me.ScriptRunnerForm.InvokePaused(new Action(() => {
				if (message == null)
					message = string.Empty;

				MessageForm mf = new MessageForm("OK", null, message.ToString());
				DialogResult result = mf.ShowDialog();

				if (result == DialogResult.Abort)
					throw new UserScriptAbortException();
			}));
		}

		/// <summary>
		/// Shows a message box with an "Yes" and "No" button.
		/// </summary>
		/// <param name="message">Message to show</param>
		/// <returns>true if "Yes" was clicked, false if "No" was clicked</returns>
		public bool showMessageYesNo(object message)
		{
			return (bool)BuilderPlug.Me.ScriptRunnerForm.InvokePaused(new Func<bool>(() =>
			{
				if (message == null)
					message = string.Empty;

				MessageForm mf = new MessageForm("Yes", "No", message.ToString());
				DialogResult result = mf.ShowDialog();

				if (result == DialogResult.Abort)
					throw new UserScriptAbortException();

				return result == DialogResult.OK;
			}));
		}

		/// <summary>
		/// Exist the script prematurely without undoing its changes.
		/// </summary>
		/// <param name="s">Text to show in the status bar (optional)</param>
		public void exit(string s=null)
		{
			if (string.IsNullOrEmpty(s))
				throw new ExitScriptException();

			throw new ExitScriptException(s);
		}

		/// <summary>
		/// Exist the script prematurely with undoing its changes.
		/// </summary>
		/// <param name="s">Text to show in the status bar (optional)</param>
		public void die(string s=null)
		{
			if (string.IsNullOrEmpty(s))
				throw new DieScriptException();

			throw new DieScriptException(s);
		}

		#endregion
	}
}
