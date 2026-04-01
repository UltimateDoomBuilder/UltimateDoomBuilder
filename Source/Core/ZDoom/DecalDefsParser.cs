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

using System.Collections.Generic;
using System.IO;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Data;

#endregion

namespace CodeImp.DoomBuilder.ZDoom
{
	internal sealed class DecalDefsParser : ZDTextParser
	{
		#region ================== Variables

		private Dictionary<string, DecalInfo> decals = new Dictionary<string, DecalInfo>();

		#endregion

		#region ================== Properties

		internal override ScriptType ScriptType	{ get { return ScriptType.UNKNOWN; } }
		public Dictionary<string, DecalInfo> Decals { get { return decals; } }

		#endregion

		#region ================== Constructors

		/// <summary>
		/// Constructor
		/// </summary>
		public DecalDefsParser()
		{

		}

		#endregion

		#region ================== Methods

		/// <summary>
		/// Parses DECALDEF data
		/// </summary>
		/// <param name="data">The data to parse</param>
		/// <param name="clearerrors">If errors should be cleared</param>
		/// <returns>true if paring worked, otherwise false</returns>
		public override bool Parse(TextResourceData data, bool clearerrors)
		{
			if(!AddTextResource(data))
			{
				if (clearerrors) ClearError();
				return true;
			}

			// Cannot process?
			if (!base.Parse(data, clearerrors)) return false;

			while(SkipWhitespace(true))
			{
				string token = ReadToken().ToLowerInvariant();
				if (string.IsNullOrEmpty(token)) continue;

				switch(token)
				{
					case "decal":
					case "decalgroup":
						DecalInfo.DecalType type = token == "decal" ? DecalInfo.DecalType.DECAL : DecalInfo.DecalType.DECALGROUP;
						string decalname;
						int decalid = -1;

						SkipWhitespace(false);
						token = ReadToken();
						if (!string.IsNullOrEmpty(token))
							decalname = token;
						else
						{
							ReportError("Expected decal name, got nothing");
							return false;
						}

						SkipWhitespace(true);

						token = ReadToken();

						// Try to read the optional decal id
						if (token == "{")
						{
							datastream.Seek(-token.Length - 1, SeekOrigin.Current);
						}
						else
						{
							if (!string.IsNullOrEmpty(token))
								ReadSignedInt(token, ref decalid);
						}

						SkipWhitespace(true);

						DecalInfo di = new DecalInfo(decalname, decalid, type);

						if (!di.Parse(this))
							return false;

						if(decals.ContainsKey(decalname))
						{
							// TODO: report problem

							// Overwrite existing decal with new one (who knows if that's the correct way do handle duplicate entries?)
							decals[decalname] = di;
						}
						else
							decals.Add(decalname, di);

						break;
				}
			}

			return true;
		}

		/// <summary>
		/// Finishes parsing, getting the DecalInfo of chidren of decal groups
		/// </summary>
		public void Finish()
		{
			foreach(DecalInfo di in decals.Values)
			{
				if(di.Type == DecalInfo.DecalType.DECALGROUP && di.Children.Count > 0)
				{
					foreach(string childname in di.Children.Keys)
					{
						if(decals.ContainsKey(childname))
						{
							di.Children[childname] = decals[childname];
							break;
						}
					}
				}
			}
		}

		/// <summary>
		/// Gets the parsed DECALDEF data by id
		/// </summary>
		/// <returns>EnumList of parsed decals</returns>
		public Dictionary<int, DecalInfo> GetDecalDefsById()
		{
			Dictionary<int, DecalInfo> dict = new Dictionary<int, DecalInfo>();

			foreach (DecalInfo di in decals.Values)
			{
				if(di.Index != -1)
					dict[di.Index] = di;
			}

			return dict;
		}

		#endregion
	}
}
