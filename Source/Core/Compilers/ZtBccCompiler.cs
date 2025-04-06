using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace CodeImp.DoomBuilder.Compilers
{
	internal class ZtBccCompiler : BccCompiler
	{
		public ZtBccCompiler(CompilerInfo info) : base(info) { }

		protected override void OnCheckError(HashSet<string> includes, ProcessStartInfo processinfo, Process process, CompileContext context)
		{
			if (process.ExitCode == 0)
			{
				return;
			}

			var foundAnyErrors = false;
			var errorLines = process.StandardError.ReadToEnd().Split('\n');

			// Note, the logic down here remains unchanged compared to traditional BCC compilation.
			// The main difference is that zt-bcc outputs warnings/errors through stderr rather than stdout.

			foreach (string rawErrorLine in errorLines)
			{
				string[] rawError = rawErrorLine.Split(new char[] { ':' }, 4);
				if (rawError.Length != 4)
					continue;
				string errorFile = rawError[0];
				int errorLine;
				if (!int.TryParse(rawError[1], out errorLine))
					continue;
				errorLine--;
				// rawError[2] is ignored. in BCC, this contains the column at which the error happened. not supported in error viewer.
				string errorContent = rawError[3].Trim();

				// logic copied from AccCompiler
				string temppath = this.tempdir.FullName + Path.DirectorySeparatorChar.ToString(); //mxd. Need trailing slash..
				if (errorFile.StartsWith(temppath)) errorFile = errorFile.Replace(temppath, string.Empty);

				if (!Path.IsPathRooted(errorFile))
				{
					//mxd. If the error is in an include file, try to find it in loaded resources
					if (includes.Contains(errorFile))
					{
						foreach (DataReader dr in General.Map.Data.Containers)
						{
							if (dr is DirectoryReader && dr.FileExists(errorFile))
							{
								errorFile = Path.Combine(dr.Location.location, errorFile);
								break;
							}
						}
					}
					else
					{
						// Add working directory to filename, so it could be recognized as map namespace lump in MapManager.CompileLump()
						errorFile = Path.Combine(processinfo.WorkingDirectory, errorFile);
					}
				}
				// end logic copied from AccCompiler

				CompilerError err = new CompilerError();
				err.linenumber = errorLine;
				err.filename = errorFile;
				err.description = errorContent;

				ReportError(err);
				foundAnyErrors = true;
			}

			if (!foundAnyErrors)
				ReportError(new CompilerError(string.Join(Environment.NewLine, errorLines)));
		}
	}
}
