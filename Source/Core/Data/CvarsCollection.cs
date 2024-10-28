#region ================== Namespaces

using System;
using System.Collections.Generic;
using CodeImp.DoomBuilder.Rendering;

#endregion

namespace CodeImp.DoomBuilder.Data
{
	internal class CvarsCollection
	{
		#region ================== Variables

		internal readonly Dictionary<string, int> Integers;
		internal readonly Dictionary<string, float> Floats;
		internal readonly Dictionary<string, PixelColor> Colors;
		internal readonly Dictionary<string, bool> Booleans;
		internal readonly Dictionary<string, string> Strings;
		internal readonly HashSet<string> AllNames;

		#endregion

		#region ================== Constructor

		public CvarsCollection()
		{
			Integers = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
			Floats = new Dictionary<string, float>(StringComparer.OrdinalIgnoreCase);
			Colors = new Dictionary<string, PixelColor>(StringComparer.OrdinalIgnoreCase);
			Booleans = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);
			Strings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
			AllNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
		}

		#endregion

		#region ================== Methods

		public bool AddValue(string name, int value)
		{
			if(AllNames.Contains(name)) return false;
			AllNames.Add(name);
			Integers.Add(name, value);
			return true;
		}

		public bool AddValue(string name, float value)
		{
			if(AllNames.Contains(name)) return false;
			AllNames.Add(name);
			Floats.Add(name, value);
			return true;
		}

		public bool AddValue(string name, PixelColor value)
		{
			if(AllNames.Contains(name)) return false;
			AllNames.Add(name);
			Colors.Add(name, value);
			return true;
		}

		public bool AddValue(string name, bool value)
		{
			if(AllNames.Contains(name)) return false;
			AllNames.Add(name);
			Booleans.Add(name, value);
			return true;
		}

		public bool AddValue(string name, string value)
		{
			if(AllNames.Contains(name)) return false;
			AllNames.Add(name);
			Strings.Add(name, value);
			return true;
		}

		#endregion
	}
}
