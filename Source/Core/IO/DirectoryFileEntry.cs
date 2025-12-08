
#region ================== Copyright (c) 2007 Pascal vd Heiden

/*
 * Copyright (c) 2007 Pascal vd Heiden, www.codeimp.com
 * This program is released under GNU General Public License
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 */

#endregion

#region ================== Namespaces

using System.IO;

#endregion

namespace CodeImp.DoomBuilder.IO
{
	public enum FileTitleStyle
	{
		DEFAULT,
		ZDOOM,
		ETERNITYENGINE
	}

	internal struct DirectoryFileEntry
	{
		// Example for:   C:\WADs\Foo\Bar.WAD
		// Created from:  C:\WADs
		// Members
		public string filename;         // bar.wad
		public string filetitle;        // bar
		public string extension;        // wad
		public string path;             // foo\
		public string filepathname;     // Foo\Bar.WAD
		public string filepathtitle;    // Foo\Bar

		// Constructor
		public DirectoryFileEntry(string fullname, string frompath, FileTitleStyle filetitlestyle)
		{
			// Get the information we need
			filename = Path.GetFileName(fullname);
			filetitle = GetFileTitle(fullname, filetitlestyle);
			extension = Path.GetExtension(fullname);
			if (extension.Length > 1)
				extension = extension.Substring(1);
			else
				extension = "";
			path = Path.GetDirectoryName(fullname);
			if (path.Length > (frompath.Length + 1))
				path = path.Substring(frompath.Length + 1) + Path.DirectorySeparatorChar;
			else
				path = "";
			filepathname = Path.Combine(path, filename);
			filepathtitle = Path.Combine(path, filetitle);

			// Make some lowercase
			filename = filename.ToLowerInvariant();
			filetitle = filetitle.ToLowerInvariant();
			extension = extension.ToLowerInvariant();
			path = path.ToLowerInvariant();
		}

		// Constructor
		public DirectoryFileEntry(string fullname, FileTitleStyle filetitlestyle)
		{
			// Get the information we need
			filename = Path.GetFileName(fullname);
			filetitle = GetFileTitle(fullname, filetitlestyle);
			extension = Path.GetExtension(fullname);
			if (extension.Length > 1)
				extension = extension.Substring(1);
			else
				extension = "";
			path = Path.GetDirectoryName(fullname);
			if (!string.IsNullOrEmpty(path)) path += Path.DirectorySeparatorChar; //mxd
			filepathname = Path.Combine(path, filename);
			filepathtitle = Path.Combine(path, filetitle);

			// Make some lowercase
			filename = filename.ToLowerInvariant();
			filetitle = filetitle.ToLowerInvariant();
			extension = extension.ToLowerInvariant();
			path = path.ToLowerInvariant();
		}

		private static string GetFileTitle(string fullname, FileTitleStyle filetitlestyle)
		{
			if(filetitlestyle == FileTitleStyle.ZDOOM)
			{
				// ZDoom style: remove extension, then take the first 8 characters
				string t = Path.GetFileNameWithoutExtension(fullname);

				return t.Length > 8 ? t.Substring(0, 8) : t;
			}
			else if (filetitlestyle == FileTitleStyle.ETERNITYENGINE)
			{
				// Eternity Engine style: remove everything after the first dot, then take the first 8 characters
				string t = Path.GetFileName(fullname);
				int dotindex = t.IndexOf('.');

				if (dotindex > 0)
					t = t.Substring(0, dotindex);

				return t.Length > 8 ? t.Substring(0, 8) : t;
			}
			else
			{
				// Default style: just remove extension
				return Path.GetFileNameWithoutExtension(fullname);
			}
		}
	}
}
