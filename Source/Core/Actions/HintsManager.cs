#region ================== Namespaces

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

#endregion

namespace CodeImp.DoomBuilder.Actions
{
	public class HintsManager
	{
		#region ================== Public constants

		public const string GENERAL = "general";
		public const string MULTISELECTION = "multiselection";

		#endregion
		
		#region ================== Constants

		private const string HINTS_RESOURCE = "Hints.cfg";
		private const string CLASS_MARKER = "class";
		private const string GROUP_MARKER = "group";
		private const string DEFAULT_HINT = "{\\rtf1 Press {\\b F1} to show help for current editing mode}";

		#endregion

		#region ================== Variables

		private readonly Dictionary<string, Dictionary<string, string>> hints; //<classname, <group, hints as rtf string>>

		#endregion

		#region ================== Constructor

		public HintsManager() 
		{
			hints = new Dictionary<string, Dictionary<string, string>>(StringComparer.Ordinal);
		}

		#endregion

		#region ================== Hints

		//Hints.cfg is dev-only stuff so bare minimum of boilerplate is present 
		//(e.g. create your Hints.cfg exactly the way it's done in the main project). 
		internal void LoadHints(Assembly asm) 
		{
			// Find a resource named Hints.cfg
			string[] resnames = asm.GetManifestResourceNames();
			string asmname = asm.GetName().Name.ToLowerInvariant() + "_";

			foreach(string rn in resnames) 
			{
				// Found one?
				if(rn.EndsWith(HINTS_RESOURCE, StringComparison.OrdinalIgnoreCase)) 
				{
					string classname = string.Empty;
					string groupname = string.Empty;
					List<string> lines = new List<string>(2);
					
					// Get a stream from the resource
					Stream data = asm.GetManifestResourceStream(rn);
					if(data == null) return;
					using(StreamReader reader = new StreamReader(data, Encoding.ASCII)) 
					{
						while(!reader.EndOfStream) lines.Add(reader.ReadLine());
					}

					Dictionary<string, List<string>> group = new Dictionary<string, List<string>>(StringComparer.Ordinal);

					foreach(string s in lines) 
					{
						string line = s.Trim();
						if(string.IsNullOrEmpty(line) || line.StartsWith("//"))
							continue;

						//class declaration?
						if(line.StartsWith(CLASS_MARKER)) 
						{
							if(!string.IsNullOrEmpty(classname)) 
							{
								hints.Add(asmname + classname, ProcessHints(group));
							}

							classname = line.Substring(6, line.Length - 6);
							groupname = string.Empty;
							group.Clear();
							continue;
						}

						//group declaration?
						if(line.StartsWith(GROUP_MARKER)) 
						{
							groupname = line.Substring(6, line.Length - 6);
							group.Add(groupname, new List<string>());
							continue;
						}

						//regular lines
						line = line.Replace("\"", string.Empty).Replace("<b>", "{\\b ").Replace("</b>", "}").Replace("<br>", "\\par ");

						//replace action names with keys
						int start = line.IndexOf("<k>");
						while(start != -1) 
						{
							int end = line.IndexOf("</k>");
							string key = line.Substring(start + 3, end - start - 3);
#if ULDB
							// ULDB: marks a key, so the Help docker can draw it as a key cap or a mouse button
							line = line.Substring(0, start) + "{\\b\\uldbkey " + Action.GetShortcutKeyDesc(key) + "}" + line.Substring(end + 4, line.Length - end - 4);
#else
							line = line.Substring(0, start) + "{\\b " + Action.GetShortcutKeyDesc(key) + "}" + line.Substring(end + 4, line.Length - end - 4);
#endif
							start = line.IndexOf("<k>");
						}

						group[groupname].Add(line);
					}

					//add the last class
					hints.Add(asmname + classname, ProcessHints(group));
					return;
				}
			}
		}

		private static Dictionary<string, string> ProcessHints(Dictionary<string, List<string>> hintsgroup) 
		{
			var result = new Dictionary<string, string>(StringComparer.Ordinal);
			foreach(KeyValuePair<string, List<string>> group in hintsgroup) 
			{
				result.Add(group.Key, "{\\rtf1" + string.Join("\\par\\par ", group.Value.ToArray()) + "}");
			}
			return result;
		}

		public void ShowHints(Type type, string groupname) 
		{
			string fullname = type.Assembly.GetName().Name.ToLowerInvariant() + "_" + type.Name;

			if(!hints.ContainsKey(fullname) || !hints[fullname].ContainsKey(groupname)) 
			{
				General.Interface.ShowHints(DEFAULT_HINT);
#if DEBUG
				Console.WriteLine("WARNING: Unable to get hints for class \"" + fullname + "\", group \"" + groupname + "\"");
#endif
				return;
			}
			General.Interface.ShowHints(hints[fullname][groupname]);
		}

		#endregion

		#region ================== Utility

		public static string GetRtfString(string text) 
		{
			text = text.Replace("<b>", "{\\b ").Replace("</b>", "}").Replace("<br>", "\\par\\par ");
			return "{\\rtf1" + text + "}";
		}

		#endregion
	}
}
