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

using System.Collections;
using System.Collections.Generic;
using CodeImp.DoomBuilder.IO;

#endregion


namespace CodeImp.DoomBuilder.Dehacked
{
	public class DehackedData
	{
		#region ================== Variables

		private Dictionary<int, DehackedThing> things;
		private Dictionary<int, DehackedFrame> frames;
		private Dictionary<int, string> sprites;
		private Dictionary<long, string> bitmnemonics;
		private DehackedFrame defaultframe;
		private Configuration cfg;
		private string root;

		#endregion

		#region ================== Properties

		public Dictionary<int, DehackedThing> Things { get { return things; } }
		public Dictionary<int, DehackedFrame> Frames { get { return frames; } }
		public Dictionary<int, string> Sprites { get { return sprites; } }
		public Dictionary<long, string> BitMnemonics { get { return bitmnemonics; } }

		#endregion

		#region ================== Constructor

		internal DehackedData(Configuration cfg, string root)
		{
			things = new Dictionary<int, DehackedThing>();
			frames = new Dictionary<int, DehackedFrame>();
			sprites = new Dictionary<int, string>();
			bitmnemonics = new Dictionary<long, string>();

			this.cfg = cfg;
			this.root = root;

			IDictionary thingblocks = cfg.ReadSetting(root + ".things", new Hashtable());
			foreach(DictionaryEntry tb in thingblocks)
			{
				int dehackedid = int.Parse(tb.Key.ToString());
				things[dehackedid] = LoadThing(tb);
			}

			IDictionary frameblocks = cfg.ReadSetting(root + ".frames", new Hashtable());
			foreach (DictionaryEntry fb in frameblocks)
			{
				if (fb.Key.ToString() == "default")
					defaultframe = LoadFrame(fb);
				else
				{
					int frameindex = int.Parse(fb.Key.ToString());
					frames[frameindex] = LoadFrame(fb);
				}
			}

			IDictionary spriteblock = cfg.ReadSetting(root + ".sprites", new Hashtable());
			foreach (DictionaryEntry sb in spriteblock)
			{
				int key;

				if (int.TryParse(sb.Key.ToString(), out key))
					sprites[key] = cfg.ReadSetting(root + ".sprites." + key, "----");
			}

			IDictionary bitmnemonicsblock = cfg.ReadSetting(root + ".bitmnemonics", new Hashtable());
			foreach (DictionaryEntry bmb in bitmnemonicsblock)
			{
				int key;

				if (int.TryParse(bmb.Key.ToString(), out key))
					bitmnemonics[key] = cfg.ReadSetting(root + ".bitmnemonics." + key, "unset");
			}
		}

		#endregion

		#region ================== Methods

		private DehackedThing LoadThing(DictionaryEntry entry)
		{
			int dehackedid = int.Parse(entry.Key.ToString());
			string path = string.Format("{0}.things.{1}.", root, dehackedid);
			string name = cfg.ReadSetting(path + "name", "<No name>");
			int doomednum = cfg.ReadSetting(path + "doomednum", 0);
			bool hasdoomednum = cfg.SettingExists(path + "doomednum");
			int height = cfg.ReadSetting(path + "height", 0);
			int width = cfg.ReadSetting(path + "width", 0);
			int initialframe = cfg.ReadSetting(path + "initialframe", 0);
			long bits = cfg.ReadSetting(path + "bits", 0L);

			Dictionary<string, string> props = new Dictionary<string, string>
			{
				{ "initial frame", initialframe.ToString() },
				{ "width", width.ToString() },
				{ "height", height.ToString() },
				{ "bits", bits.ToString() }
			};

			if(hasdoomednum)
				props["id #"] = doomednum.ToString();

			return new DehackedThing(dehackedid, name, props) { HasDoomEdNum = hasdoomednum };
		}

		private DehackedFrame LoadFrame(DictionaryEntry entry)
		{
			int frameid;
			int.TryParse(entry.Key.ToString(), out frameid);
			string path = string.Format("{0}.frames.{1}.", root, entry.Key.ToString());
			int spritenumber = cfg.ReadSetting(path + "spritenumber", 0);
			long spritesubnumber = cfg.ReadSetting(path + "spritesubnumber", 0L);

			if (spritesubnumber >= 32768)
				spritesubnumber -= 32768;

			Dictionary<string, string> props = new Dictionary<string, string>
			{
				{ "sprite number", spritenumber.ToString() },
				{ "sprite subnumber", spritesubnumber.ToString() }
			};

			return new DehackedFrame(frameid, props);
		}

		#endregion
	}
}
