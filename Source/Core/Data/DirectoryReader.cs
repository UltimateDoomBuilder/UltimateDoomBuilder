
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

using System;
using System.Collections.Generic;
using System.IO;
using CodeImp.DoomBuilder.Compilers;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.IO;

#endregion

namespace CodeImp.DoomBuilder.Data
{
	internal sealed class DirectoryReader : PK3StructuredReader
	{
		#region ================== Variables

		private readonly DirectoryFilesList files;

		#endregion
		
		#region ================== Constructor / Disposer

		// Constructor
		public DirectoryReader(DataLocation dl, GameConfiguration config, bool asreadonly) : base(dl, asreadonly)
		{
			General.WriteLogLine("Opening directory resource \"" + location.location + "\"");
			
			// Initialize
			files = new DirectoryFilesList(dl.location, config, true);
			Initialize(config);
			
			// We have no destructor
			GC.SuppressFinalize(this);
		}

		//mxd. Don't move directory wads anywhere
		protected override void Initialize(GameConfiguration config)
		{
			// Load all WAD files in the root as WAD resources
			string[] wadfiles = GetWadFiles();
			wads = new List<WADReader>(wadfiles.Length);
			foreach(string wadfile in wadfiles)
			{
				// Don't add the map file. Otherwise DataManager will try to load it twice (and fial).
				string wadfilepath = Path.Combine(location.location, wadfile);
				if(General.Map.FilePathName != wadfilepath)
				{
					DataLocation wdl = new DataLocation(DataLocation.RESOURCE_WAD, wadfilepath, false, false, true, null);
					wads.Add(new WADReader(wdl, config, isreadonly) { ParentResource = this } );
				}
			}
		}

		// Disposer
		public override void Dispose()
		{
			// Not already disposed?
			if(!isdisposed)
			{
				General.WriteLogLine("Closing directory resource \"" + location.location + "\"");
				
				// Done
				base.Dispose();
			}
		}
		
		#endregion

		#region ================== Textures

		// This finds and returns a patch stream
		public override Stream GetPatchData(string pname, bool longname, ref string patchlocation)
		{
			// Error when suspended
			if(issuspended) throw new Exception("Data reader is suspended");

			// Find in any of the wad files
			// Note the backward order, because the last wad's images have priority
			if(!longname) //mxd. Patches with long names can't be in wads
			{
				for(int i = wads.Count - 1; i > -1; i--)
				{
					Stream data = wads[i].GetPatchData(pname, false, ref patchlocation);
					if(data != null) return data;
				}
			}

			try
			{
				if(longname)
				{
					//mxd. Long names are absolute
					pname = pname.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
					if(FileExists(pname))
					{
						patchlocation = location.GetDisplayName();
						return LoadFile(pname);
					}
					return null;
				}
				
				if(General.Map.Config.MixTexturesFlats)
				{
					//mxd. Find in directories ZDoom expects them to be
					string dir = Path.GetDirectoryName(pname);
					string name = Path.GetFileName(pname);
					foreach(string loc in PatchLocations)
					{
						string path = Path.Combine(loc, dir);
						string filename = FindFirstFile(path, name, true);
						if(!string.IsNullOrEmpty(filename) && FileExists(filename))
						{
							patchlocation = location.GetDisplayName(); //mxd
							return LoadFile(filename);
						}
					}
				}
				else
				{
					// Find in patches directory
					string path = Path.Combine(PATCHES_DIR, Path.GetDirectoryName(pname));
					string filename = FindFirstFile(path, Path.GetFileName(pname), true);
					if(!string.IsNullOrEmpty(filename) && FileExists(filename))
					{
						patchlocation = location.GetDisplayName(); //mxd
						return LoadFile(filename);
					}
				}
			}
			catch(Exception e)
			{
				if (!Silent) General.ErrorLogger.Add(ErrorType.Error, e.GetType().Name + " while loading patch \"" + pname + "\" from directory: " + e.Message);
			}

			// Nothing found
			return null;
		}

		// This finds and returns a textue stream
		public override Stream GetTextureData(string pname, bool longname, ref string texturelocation)
		{
			// Error when suspended
			if(issuspended) throw new Exception("Data reader is suspended");

			// Find in any of the wad files
			// Note the backward order, because the last wad's images have priority
			if(!longname) //mxd. Textures with long names can't be in wads
			{
				for(int i = wads.Count - 1; i >= 0; i--)
				{
					Stream data = wads[i].GetTextureData(pname, false, ref texturelocation);
					if(data != null) return data;
				}
			}
			
			try
			{
				//mxd. Long names are absolute
				if(longname)
				{
					pname = pname.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
					if(FileExists(pname))
					{
						texturelocation = location.GetDisplayName();
						return LoadFile(pname);
					}
					return null;
				}
				else
				{
					// Find in textures directory
					string path = Path.Combine(TEXTURES_DIR, Path.GetDirectoryName(pname));
					string filename = FindFirstFile(path, Path.GetFileName(pname), true);
					if(!string.IsNullOrEmpty(filename) && FileExists(filename))
					{
						texturelocation = location.GetDisplayName(); //mxd
						return LoadFile(filename);
					}
				}
			}
			catch(Exception e)
			{
				if (!Silent) General.ErrorLogger.Add(ErrorType.Error, e.GetType().Name + " while loading texture \"" + pname + "\" from directory: " + e.Message);
			}

			// Nothing found
			return null;
		}

		//mxd. This finds and returns a HiRes textue stream
		public override Stream GetHiResTextureData(string name, ref string hireslocation)
		{
			// Error when suspended
			if(issuspended) throw new Exception("Data reader is suspended");

			// Find in any of the wad files
			// Note the backward order, because the last wad's images have priority
			for(int i = wads.Count - 1; i >= 0; i--)
			{
				Stream data = wads[i].GetHiResTextureData(name, ref hireslocation);
				if(data != null) return data;
			}

			try
			{
				// Find in hires directory
				string filename = FindFirstFile(HIRES_DIR, name, true);
				if(!string.IsNullOrEmpty(filename) && FileExists(filename))
				{
					hireslocation = location.GetDisplayName();
					return LoadFile(filename);
				}
			}
			catch(Exception e)
			{
				if (!Silent) General.ErrorLogger.Add(ErrorType.Error, e.GetType().Name + " while loading HiRes texture \"" + name + "\" from directory: " + e.Message);
			}

			// Nothing found
			return null;
		}

		// This finds and returns a colormap stream
		public override Stream GetColormapData(string pname)
		{
			// Error when suspended
			if(issuspended) throw new Exception("Data reader is suspended");

			// Find in any of the wad files
			// Note the backward order, because the last wad's images have priority
			for(int i = wads.Count - 1; i >= 0; i--)
			{
				Stream data = wads[i].GetColormapData(pname);
				if(data != null) return data;
			}

			try
			{
				// Find in patches directory
				string path = Path.Combine(COLORMAPS_DIR, Path.GetDirectoryName(pname));
				string filename = FindFirstFile(path, Path.GetFileName(pname), true);
				if((filename != null) && FileExists(filename))
				{
					return LoadFile(filename);
				}
			}
			catch(Exception e)
			{
				if (!Silent) General.ErrorLogger.Add(ErrorType.Error, e.GetType().Name + " while loading colormap \"" + pname + "\" from directory: " + e.Message);
			}

			// Nothing found
			return null;
		}

		#endregion

		#region ================== Sprites

		// This finds and returns a sprite stream
		public override Stream GetSpriteData(string pname, ref string spritelocation)
		{
			// Error when suspended
			if(issuspended) throw new Exception("Data reader is suspended");

			// Find in any of the wad files
			for(int i = wads.Count - 1; i >= 0; i--)
			{
				Stream sprite = wads[i].GetSpriteData(pname, ref spritelocation);
				if(sprite != null) return sprite;
			}
			
			try
			{
				// Find in sprites directory
				string path = Path.Combine(SPRITES_DIR, Path.GetDirectoryName(pname));
				string filename = FindFirstFile(path, Path.GetFileName(pname), true);
				if((filename != null) && FileExists(filename))
				{
					spritelocation = location.GetDisplayName(); //mxd
					return LoadFile(filename);
				}
			}
			catch(Exception e)
			{
				if (!Silent) General.ErrorLogger.Add(ErrorType.Error, e.GetType().Name + " while loading sprite \"" + pname + "\" from directory: " + e.Message);
			}
			
			// Nothing found
			return null;
		}

		// This checks if the given sprite exists
		public override bool GetSpriteExists(string pname)
		{
			// Error when suspended
			if(issuspended) throw new Exception("Data reader is suspended");

			// Find in any of the wad files
			for(int i = wads.Count - 1; i >= 0; i--)
			{
				if(wads[i].GetSpriteExists(pname)) return true;
			}

			// Find in sprites directory
			try
			{
				string path = Path.Combine(SPRITES_DIR, Path.GetDirectoryName(pname));
				string filename = FindFirstFile(path, Path.GetFileName(pname), true);
				if((filename != null) && FileExists(filename))
				{
					return true;
				}
			}
			catch(Exception e)
			{
				if (!Silent) General.ErrorLogger.Add(ErrorType.Error, e.GetType().Name + " while checking sprite \"" + pname + "\" existance in directory: " + e.Message);
			}
			
			// Nothing found
			return false;
		}

		#endregion

		#region ================== Voxels (mxd)

		//mxd.  This finds and returns a voxel stream
		public override Stream GetVoxelData(string name, ref string voxellocation)
		{
			// Error when suspended
			if(issuspended) throw new Exception("Data reader is suspended");

			// Find in any of the wad files
			for(int i = wads.Count - 1; i >= 0; i--)
			{
				Stream voxel = wads[i].GetVoxelData(name, ref voxellocation);
				if(voxel != null) return voxel;
			}

			try
			{
				// Find in voxels directory
				string filename = null;

				// There are different places we have to look for the file, depending on how it was defined. Right now
				// there's no way to know if the definition comes from VOXELDEF or spreite replacement, so we're assuming
				// it's from VOXELDEF if there's an extension given.
				// - Auto-detect for sprite name: must be in the "voxels" directory
				// - Just given as file name without path in VOXELDEF: must be in the root directory
				// - Given as full path and file name in VOXELDEF: must be in exactly that spot
				string path = Path.GetDirectoryName(name);
				if (string.IsNullOrWhiteSpace(path))
				{
					if (string.IsNullOrWhiteSpace(Path.GetExtension(name)))
						filename = FindFirstFile(VOXELS_DIR, Path.GetFileName(name), true);
					else
						filename = FindFirstFileWithExt("", Path.GetFileName(name), true);
				}
				else
				{
					filename = FindFirstFileWithExt(path, Path.GetFileName(name), false);
				}

				if ((filename != null) && FileExists(filename))
				{
					voxellocation = location.GetDisplayName();
					return LoadFile(filename);
				}
			}
			catch(Exception e)
			{
				if (!Silent) General.ErrorLogger.Add(ErrorType.Error, e.GetType().Name + " while loading voxel \"" + name + "\" from directory: " + e.Message);
			}

			// Nothing found
			return null;
		}

		#endregion

		#region ================== Methods

		// Return a short name for this data location
		public override string GetTitle()
		{
			return Path.GetFileName(location.location);
		}

		// This creates an image
		protected override ImageData CreateImage(string filename, int imagetype)
		{
			switch(imagetype)
			{
				case ImageDataFormat.DOOMFLAT:
					return new FileImage(filename, Path.Combine(location.location, filename), true);
				
				case ImageDataFormat.DOOMPICTURE:
					return new FileImage(filename, Path.Combine(location.location, filename), false);
					
				case ImageDataFormat.DOOMCOLORMAP:
					return new ColormapImage(Path.GetFileNameWithoutExtension(filename));

				default:
					throw new ArgumentException("Invalid image format specified!");
			}
		}

		// This returns true if the specified file exists
		internal override bool FileExists(string filename, int unused) { return files.FileExists(filename); } //mxd
		internal override bool FileExists(string filename)
		{
			return files.FileExists(filename);
		}
		
		// This returns all files in a given directory
		protected override string[] GetAllFiles(string path, bool subfolders)
		{
			return files.GetAllFiles(path, subfolders).ToArray();
		}

		//mxd. This returns wad files in the root directory
		protected override string[] GetWadFiles()
		{
			return files.GetWadFiles().ToArray();
		}
		
		// This returns all files in a given directory that have the given file title
		protected override string[] GetAllFilesWithTitle(string path, string title, bool subfolders)
		{
			return files.GetAllFilesWithTitle(path, title, subfolders).ToArray();
		}

		//mxd. This returns all files in a given directory which title starts with given title
		protected override string[] GetAllFilesWhichTitleStartsWith(string path, string title, bool subfolders) 
		{
			return files.GetAllFilesWhichTitleStartsWith(path, title, subfolders).ToArray();
		}
		
		// This returns all files in a given directory that match the given extension
		internal override string[] GetFilesWithExt(string path, string extension, bool subfolders)
		{
			return files.GetAllFiles(path, extension, subfolders).ToArray();
		}

		// This finds the first file that has the specific name, regardless of file extension
		internal override string FindFirstFile(string beginswith, bool subfolders)
		{
			return files.GetFirstFile(beginswith, subfolders);
		}

		// This finds the first file that has the specific name, regardless of file extension
		protected override string FindFirstFile(string path, string beginswith, bool subfolders)
		{
			return files.GetFirstFile(path, beginswith, subfolders);
		}

		// This finds the first file that has the specific name
		protected override string FindFirstFileWithExt(string path, string beginswith, bool subfolders)
		{
			string title = Path.GetFileNameWithoutExtension(beginswith);
			string ext = Path.GetExtension(beginswith);
			ext = (!string.IsNullOrEmpty(ext) && ext.Length > 1 ? ext.Substring(1) : string.Empty);
			return files.GetFirstFile(path, title, subfolders, ext);
		}
		
		// This loads an entire file in memory and returns the stream
		// NOTE: Callers are responsible for disposing the stream!
		internal override MemoryStream LoadFile(string filename)
		{
			MemoryStream s = null;
			string casecorrectfilename = GetCorrectCaseForFile(filename);

			if(casecorrectfilename == null)
			{
				if (!Silent) General.ErrorLogger.Add(ErrorType.Error, "Unable to load file " + filename + ": file doesn't exist");
				return null;
			}

			try
			{
				lock(this)
				{
					s = new MemoryStream(File.ReadAllBytes(Path.Combine(location.location, casecorrectfilename)));
				}
			} 
			catch(Exception e) 
			{
				if (!Silent) General.ErrorLogger.Add(ErrorType.Error, "Unable to load file: " + e.Message);
			}
			return s;
		}

		//mxd
		internal override bool SaveFile(MemoryStream stream, string filename, int unused) { return SaveFile(stream, filename); }
		internal override bool SaveFile(MemoryStream stream, string filename)
		{
			if(isreadonly) return false;

			try
			{
				lock(this)
				{
					File.WriteAllBytes(Path.Combine(location.location, filename), stream.ToArray());
				}
			}
			catch(Exception e)
			{
				if (!Silent) General.ErrorLogger.Add(ErrorType.Error, "Unable to save file: " + e.Message);
				return false;
			}

			return true;
		}

		// This creates a temp file for the speciied file and return the absolute path to the temp file
		// NOTE: Callers are responsible for removing the temp file when done!
		protected override string CreateTempFile(string filename)
		{
			// Just copy the file
			string tempfile = General.MakeTempFilename(General.Map.TempPath, "wad");
			File.Copy(Path.Combine(location.location, filename), tempfile);
			return tempfile;
		}

		/// <summary>
		/// Returns the correctly cased file from a path/file. This is required for case sensitive file systems.
		/// </summary>
		/// <param name="filepathname">File name get the the correctly cased name from</param>
		/// <returns></returns>
		protected override string GetCorrectCaseForFile(string filepathname)
		{
			try
			{
				return files.GetFileInfo(filepathname).filepathname;
			}
			catch(KeyNotFoundException)
			{
				return null;
			}
		}

		#endregion

		#region ================== Compiling (mxd)

		// This compiles a script lump and returns any errors that may have occurred
		// Returns true when our code worked properly (even when the compiler returned errors)
		internal override bool CompileLump(string filepathname, int unused, ScriptConfiguration scriptconfig, List<CompilerError> errors) { return CompileLump(filepathname, scriptconfig, errors); }
		internal override bool CompileLump(string filepathname, ScriptConfiguration scriptconfig, List<CompilerError> errors)
		{
			return CompileScriptLump(filepathname, scriptconfig, errors);
		}

		internal static bool CompileScriptLump(string filepathname, ScriptConfiguration scriptconfig, List<CompilerError> errors)
		{
			// No compiling required
			if(scriptconfig.Compiler == null) return true;

			// Initialize compiler
			Compiler compiler;
			try
			{
				compiler = scriptconfig.Compiler.Create();
			}
			catch(Exception e)
			{
				// Fail
				errors.Add(new CompilerError("Unable to initialize compiler. " + e.GetType().Name + ": " + e.Message));
				return false;
			}

			// Copy the source file into the temporary directory
			string inputfile = Path.Combine(compiler.Location, Path.GetFileName(filepathname));
			File.Copy(filepathname, inputfile);

			// Make random output filename
			string outputfile = General.MakeTempFilename(compiler.Location, "tmp");

			// Run compiler
			compiler.Parameters = scriptconfig.Parameters;
			compiler.InputFile = inputfile;
			compiler.OutputFile = Path.GetFileName(outputfile);
			compiler.SourceFile = filepathname;
			compiler.WorkingDirectory = Path.GetDirectoryName(inputfile);
			if(compiler.Run())
			{
				// Fetch errors
				foreach(CompilerError e in compiler.Errors)
				{
					CompilerError newerr = e;

					// If the error's filename equals our temporary file, replace it with the original source filename
					if(String.Compare(e.filename, inputfile, true) == 0) newerr.filename = filepathname;

					errors.Add(newerr);
				}

				// No errors and output file exists?
				if(compiler.Errors.Length == 0)
				{
					// Output file exists?
					if(!File.Exists(outputfile))
					{
						// Fail
						compiler.Dispose();
						errors.Add(new CompilerError("Output file \"" + outputfile + "\" doesn't exist."));
						return false;
					}

					//mxd. Move and rename the result file
					string targetfilename;
					if(compiler is AccCompiler)
					{
						AccCompiler acccompiler = (AccCompiler)compiler;
						targetfilename = Path.Combine(Path.GetDirectoryName(filepathname), acccompiler.Parser.LibraryName + ".o");
					}
					else
					{
						//mxd. No can't do...
						if(String.IsNullOrEmpty(scriptconfig.ResultLump))
						{
							// Fail
							compiler.Dispose();
							errors.Add(new CompilerError("Unable to create target file: unable to determine target filename. Make sure \"ResultLump\" property is set in the \"" + scriptconfig + "\" script configuration."));
							return false;
						}
						
						targetfilename = Path.Combine(Path.GetDirectoryName(filepathname), scriptconfig.ResultLump);
					}

					// Rename and copy to source file directory
					try
					{
						File.Copy(outputfile, targetfilename, true);
					}
					catch(Exception e)
					{
						// Fail
						compiler.Dispose();
						errors.Add(new CompilerError("Unable to create library file \"" + targetfilename + "\". " + e.GetType().Name + ": " + e.Message));
						return false;
					}
				}

				// Done
				compiler.Dispose();
				return true;
			}

			// Fail
			compiler.Dispose();
			return false;
		}

		#endregion
	}
}
