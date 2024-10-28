
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
using System.Diagnostics;
using System.IO;
using CodeImp.DoomBuilder.Config;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.ZDoom.Scripting;

#endregion

namespace CodeImp.DoomBuilder.Compilers
{
	internal class AccCompiler : Compiler
	{
		#region ================== Internal classes

		protected class CompileContext { }

		#endregion

		#region ================== Constants

		private const string ACS_ERROR_FILE = "acs.err";

		#endregion

		#region ================== Variables

		private AcsParserSE parser; //mxd

		#endregion

		#region ================== Properties

		public bool SourceIsMapScriptsLump; //mxd
		internal AcsParserSE Parser { get { return parser; } } //mxd

		#endregion

		#region ================== Constructor

		// Constructor
		public AccCompiler(CompilerInfo info) : base(info, false)
		{
		}

		// Disposer
		public override void Dispose()
		{
			// Not already disposed?
			if (!isdisposed)
			{
				// Clean up

				// Done
				base.Dispose();
			}
		}

		#endregion

		#region ================== Methods

		protected virtual CompileContext OnBeforeProcessStart(ProcessStartInfo info)
		{
			return new CompileContext();
		}

		protected virtual void OnCheckError(HashSet<string> includes, ProcessStartInfo processinfo, Process process, CompileContext context)
		{
			int line = 0;

			// Now find the error file
			string errfile = Path.Combine(this.workingdir, ACS_ERROR_FILE);
			if (File.Exists(errfile))
			{
				try
				{
					// Regex to find error lines
					Regex errlinematcher = new Regex(":[0-9]+: ", RegexOptions.Compiled | RegexOptions.CultureInvariant);

					// Read all lines
					bool erroradded = false; //mxd
					string[] errlines = File.ReadAllLines(errfile);
					string temppath = this.tempdir.FullName + Path.DirectorySeparatorChar.ToString(); //mxd. Need trailing slash..
					while (line < errlines.Length)
					{
						// Check line
						string linestr = errlines[line];
						Match match = errlinematcher.Match(linestr);
						if (match.Success && (match.Index > 0))
						{
							CompilerError err = new CompilerError();

							// The match without spaces and semicolon is the line number
							string linenr = match.Value.Replace(":", "").Trim();
							if (!int.TryParse(linenr, out err.linenumber))
								err.linenumber = CompilerError.NO_LINE_NUMBER;
							else
								err.linenumber--;

							// Everything before the match is the filename
							err.filename = linestr.Substring(0, match.Index).Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);

							//mxd. Get rid of temp directory path
							if (err.filename.StartsWith(temppath)) err.filename = err.filename.Replace(temppath, string.Empty);

							if (!Path.IsPathRooted(err.filename))
							{
								//mxd. If the error is in an include file, try to find it in loaded resources
								if (includes.Contains(err.filename))
								{
									foreach (DataReader dr in General.Map.Data.Containers)
									{
										if (dr is DirectoryReader && dr.FileExists(err.filename))
										{
											err.filename = Path.Combine(dr.Location.location, err.filename);
											break;
										}
									}
								}
								else
								{
									// Add working directory to filename, so it could be recognized as map namespace lump in MapManager.CompileLump()
									err.filename = Path.Combine(processinfo.WorkingDirectory, err.filename);
								}
							}

							// Everything after the match is the description
							err.description = linestr.Substring(match.Index + match.Length).Trim();

							// Report the error
							ReportError(err);
							erroradded = true; //mxd
						}

						// Next line
						line++;
					}

					//mxd. Some ACC errors are not properly formatted. If that's the case, threat the whole acs.err as an error...
					if (!erroradded && errlines.Length > 0)
					{
						ReportError(new CompilerError(string.Join(Environment.NewLine, errlines)));
					}
				}
				catch (Exception e)
				{
					// Error reading errors (ironic, isn't it)
					ReportError(new CompilerError("Failed to retrieve compiler error report. " + e.GetType().Name + ": " + e.Message));
				}
			}
		}

		// This runs the compiler
		public override bool Run()
		{
			Process process;
			string sourcedir = Path.GetDirectoryName(sourcefile);

			// Preprocess the file
			parser = new AcsParserSE
			{
				IsMapScriptsLump = SourceIsMapScriptsLump,
				OnInclude = delegate (AcsParserSE se, string includefile, AcsParserSE.IncludeType includetype)
				{
					TextResourceData data = General.Map.Data.GetTextResourceData(includefile);
					if (data == null)
					{
						se.ReportError("Unable to find include file \"" + includefile + "\".");
						return false; // Fial
					}

					return se.Parse(data, true, includetype, false);
				}
			};

			string inputfilepath = Path.Combine(this.tempdir.FullName, inputfile);
			using (FileStream stream = File.OpenRead(inputfilepath))
			{
				// Map SCRIPTS lump is empty. Abort the process without generating any warnings or errors. 
				if (SourceIsMapScriptsLump && stream.Length == 0) return false;

				DataLocation dl = new DataLocation(DataLocation.RESOURCE_DIRECTORY, Path.GetDirectoryName(inputfilepath), false, false, false, null);
				//mxd. TextResourceData must point to temp path when compiling WAD lumps for lump to be recognized as map lump when reporting errors...
				TextResourceData data = new TextResourceData(stream, dl, (SourceIsMapScriptsLump ? inputfile : sourcefile));
				if (!parser.Parse(data, info.Files, true, AcsParserSE.IncludeType.NONE, false))
				{
					// Check for errors
					if (parser.HasError) ReportError(new CompilerError(parser.ErrorDescription, parser.ErrorSource, parser.ErrorLine));
					return true;
				}
			}

			//mxd. External lumps should be libraries
			if (!SourceIsMapScriptsLump && !parser.IsLibrary)
			{
				ReportError(new CompilerError("External ACS files can only be compiled as libraries.", sourcefile));
				return true;
			}

			//mxd. Update script names if we are compiling the map SCRIPTS lump
			if (SourceIsMapScriptsLump)
			{
				General.Map.UpdateScriptNames(parser);
			}

			//xabis
			// Copy includes from the resources into the compiler's folder, preserving relative pathing and naming
			HashSet<string> includes = parser.GetIncludes(); //mxd
			foreach (string include in includes)
			{
				// Grab the script text from the resources
				TextResourceData data = General.Map.Data.GetTextResourceData(include);
				if (data != null && data.Stream != null)
				{
					// Pull the pk3 or directory sub folder out if applicable
					FileInfo fi = new FileInfo(Path.Combine(this.tempdir.FullName, include));

					// Do not allow files to be overwritten, either accidentally or maliciously
					if (!fi.Exists)
					{
						General.WriteLogLine("Copying script include: " + include);

						// Create the directory path as needed
						if (!string.IsNullOrEmpty(fi.DirectoryName)) Directory.CreateDirectory(fi.DirectoryName);

						// Dump the script into the target file
						BinaryReader reader = new BinaryReader(data.Stream);
						File.WriteAllBytes(fi.FullName, reader.ReadBytes((int)data.Stream.Length));
					}
				}
			}

			// Create parameters
			string args = this.parameters;
			args = args.Replace("%FI", inputfile);
			args = args.Replace("%FO", outputfile);
			args = args.Replace("%FS", sourcefile);
			args = args.Replace("%PT", this.tempdir.FullName);
			args = args.Replace("%PS", sourcedir);
			args = args.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar); //mxd. This fixes include path when the map is in a root directory

			// Setup process info
			ProcessStartInfo processinfo = new ProcessStartInfo();
			processinfo.Arguments = args;
			processinfo.FileName = Path.Combine(info.Path, info.ProgramFile); //mxd
			processinfo.CreateNoWindow = false;
			processinfo.ErrorDialog = false;
			processinfo.UseShellExecute = true;
			processinfo.WindowStyle = ProcessWindowStyle.Hidden;
			processinfo.WorkingDirectory = this.workingdir;

			CompileContext context = OnBeforeProcessStart(processinfo);

			// Output info
			General.WriteLogLine("Running compiler...");
			General.WriteLogLine("Program:    " + processinfo.FileName);
			General.WriteLogLine("Arguments:  " + processinfo.Arguments);

			try
			{
				// Start the compiler
				process = Process.Start(processinfo);
			}
			catch (Exception e)
			{
				// Unable to start the compiler
				General.ShowErrorMessage("Unable to start the compiler (" + info.Name + "). " + e.GetType().Name + ": " + e.Message, MessageBoxButtons.OK);
				return false;
			}

			// Wait for compiler to complete
			process.WaitForExit();
			TimeSpan deltatime = TimeSpan.FromTicks(process.ExitTime.Ticks - process.StartTime.Ticks);
			General.WriteLogLine("Compiler process has finished.");
			General.WriteLogLine("Compile time: " + deltatime.TotalSeconds.ToString("########0.00") + " seconds");

			OnCheckError(includes, processinfo, process, context);

			return true;
		}

		#endregion
	}
}