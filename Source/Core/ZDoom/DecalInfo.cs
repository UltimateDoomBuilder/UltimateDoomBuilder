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

#endregion

namespace CodeImp.DoomBuilder.ZDoom
{
	public class DecalInfo
	{
		#region ================== Enums

		public enum DecalType
		{
			DECAL,
			DECALGROUP
		}

		#endregion

		#region ================== Variables

		private string name;
		private string picturename = string.Empty;
		private int index;
		private string description;
		private DecalType type;
		private Dictionary<string, DecalInfo> childdecals = new Dictionary<string, DecalInfo>(); // Children of decal groups

		#endregion

		#region ================== Properties

		public string Name { get { return name; } }
		public string PictureName { get { return picturename; } }
		public int Index { get { return index; } }
		public DecalType Type { get { return type; } }
		public string Description { get { return description; } }
		public Dictionary<string, DecalInfo> Children { get { return childdecals; } }

		#endregion

		#region ================== Constructor

		public DecalInfo(string name, int index, DecalType type)
		{
			this.name = name;
			this.index = index;
			this.type = type;

			description = index.ToString() + ": " + name;
		}

		#endregion

		#region ================== Methods

		internal void SetPictureName(string picturename)
		{
			this.picturename = picturename;
		}

		/// <summary>
		/// Parse a decal or decalgroup definition
		/// </summary>
		/// <param name="parser"></param>
		/// <returns></returns>
		internal bool Parse(DecalDefsParser parser)
		{
			switch(type)
			{
				case DecalType.DECAL:
					return ParseDecal(parser);
				case DecalType.DECALGROUP:
					return ParseDecalGroup(parser);
			}

			return false;
		}

		/// <summary>
		/// Parses a decal definition
		/// </summary>
		/// <param name="parser">the DecalDefsParser that right before the decal definition block</param>
		/// <returns></returns>
		private bool ParseDecal(DecalDefsParser parser)
		{
			parser.SkipWhitespace(true);

			string token = parser.ReadToken();

			if (token != "{")
			{
				parser.ReportError("Expected \"{\", got " + token);
				return false;
			}

			while (true)
			{
				parser.SkipWhitespace(true);

				token = parser.ReadToken().ToLowerInvariant();

				if (string.IsNullOrEmpty(token))
				{
					parser.ReportError("Expected property of }, got nothing");
					return false;
				}

				// Decal ends here
				if (token == "}")
				{
					break;
				}

				switch (token)
				{
					case "pic":
						parser.SkipWhitespace(false);
						token = parser.ReadToken();

						if (string.IsNullOrEmpty(token))
						{
							parser.ReportError("Expected image name, got nothing");
							return false;
						}

						picturename = token;

						break;
				}
			}

			return true;
		}

		/// <summary>
		/// Parses a decalgroup defition
		/// </summary>
		/// <param name="parser">The DecalDefsParser that right before the decalgroup definition block</param>
		/// <returns></returns>
		private bool ParseDecalGroup(DecalDefsParser parser)
		{
			parser.SkipWhitespace(true);

			string token = parser.ReadToken();

			if (token != "{")
			{
				parser.ReportError("Expected \"{\", got " + token);
				return false;
			}

			while (true)
			{
				parser.SkipWhitespace(true);

				token = parser.ReadToken().ToLowerInvariant();

				if (string.IsNullOrEmpty(token))
				{
					parser.ReportError("Expected property of }, got nothing");
					return false;
				}

				// Decal ends here
				if (token == "}")
				{
					break;
				}

				// Add name of child to the list of children
				if (childdecals.ContainsKey(token))
				{
					// TODO: report problem

					// Overwrite existing decal with new one (who knows if that's the correct way do handle duplicate entries?)
					childdecals[token] = null;
				}
				else
					childdecals.Add(token, null);

				// Read the probability wheight. We don't use it, though
				int weight = 0;
				parser.SkipWhitespace(false);
				token = parser.ReadToken();
				if(string.IsNullOrEmpty(token) || !parser.ReadSignedInt(token, ref weight))
				{
					parser.ReportError("Expected probability weight as number, got \"" + token + "\"");
					return false;
				}
			}

			return true;
		}

		#endregion
	}
}
