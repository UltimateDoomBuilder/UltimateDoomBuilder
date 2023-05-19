using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace CodeImp.DoomBuilder
{
	internal static class FileLockChecker
	{
		internal class FileLockCheckResult //mxd
		{
			public string Error;
			public List<Process> Processes = new List<Process>();
		}
		
		[StructLayout(LayoutKind.Sequential)]
		private struct RM_UNIQUE_PROCESS
		{
			public int dwProcessId;
			public System.Runtime.InteropServices.ComTypes.FILETIME ProcessStartTime;
		}

		private const int RmRebootReasonNone = 0;
		private const int CCH_RM_MAX_APP_NAME = 255;
		private const int CCH_RM_MAX_SVC_NAME = 63;

		private enum RM_APP_TYPE
		{
			RmUnknownApp = 0,
			RmMainWindow = 1,
			RmOtherWindow = 2,
			RmService = 3,
			RmExplorer = 4,
			RmConsole = 5,
			RmCritical = 1000
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		private struct RM_PROCESS_INFO
		{
			public RM_UNIQUE_PROCESS Process;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCH_RM_MAX_APP_NAME + 1)]
			public string strAppName;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCH_RM_MAX_SVC_NAME + 1)]
			public string strServiceShortName;

			public RM_APP_TYPE ApplicationType;
			public uint AppStatus;
			public uint TSSessionId;
			[MarshalAs(UnmanagedType.Bool)]
			public bool bRestartable;
		}

		[DllImport("rstrtmgr.dll", CharSet = CharSet.Unicode)]
		private static extern int RmRegisterResources(uint pSessionHandle,
											  UInt32 nFiles,
											  string[] rgsFilenames,
											  UInt32 nApplications,
											  [In] RM_UNIQUE_PROCESS[] rgApplications,
											  UInt32 nServices,
											  string[] rgsServiceNames);

		[DllImport("rstrtmgr.dll", CharSet = CharSet.Auto)]
		private static extern int RmStartSession(out uint pSessionHandle, int dwSessionFlags, string strSessionKey);

		[DllImport("rstrtmgr.dll")]
		private static extern int RmEndSession(uint pSessionHandle);

		[DllImport("rstrtmgr.dll")]
		private static extern int RmGetList(uint dwSessionHandle,
									out uint pnProcInfoNeeded,
									ref uint pnProcInfo,
									[In, Out] RM_PROCESS_INFO[] rgAffectedApps,
									ref uint lpdwRebootReasons);

		/// <summary>
		/// Find out what process(es) have a lock on the specified file.
		/// </summary>
		/// <param name="path">Path of the file.</param>
		/// <returns>Processes locking the file</returns>
		/// <remarks>See also:
		/// http://msdn.microsoft.com/en-us/library/windows/desktop/aa373661(v=vs.85).aspx
		/// http://wyupdate.googlecode.com/svn-history/r401/trunk/frmFilesInUse.cs (no copyright in code at time of viewing)
		/// 
		/// </remarks>
		static public FileLockCheckResult CheckFile(string path)
		{
			//mxd. Do it the clunky way? (non-Windows)
			if(Environment.OSVersion.Platform == PlatformID.Unix)
			{
				bool locked = false;
				
				try
				{
					using(File.Open(path, FileMode.Open)) { }
				}
				catch(IOException e)
				{
					int errorcode = Marshal.GetHRForException(e) & ((1 << 16) - 1);
					locked = (errorcode == 32 || errorcode == 33);
				}

				return new FileLockCheckResult { Error = (locked ? "Unable to save the map. Map file is locked by another process." : string.Empty) };
			}

			//mxd. Needs Vista or newer...
			uint handle;
			string key = Guid.NewGuid().ToString();
			FileLockCheckResult result = new FileLockCheckResult(); //mxd
			string errorstart = "Unable to save the map: target file is locked by another process."
			                    + Environment.NewLine + "Also, unable to get the name of the offending process:"
			                    + Environment.NewLine + Environment.NewLine;

			int res = RmStartSession(out handle, 0, key);
			if(res != 0)
			{
				RmEndSession(handle); //mxd
				result.Error = errorstart + "Error " + res + ". Could not begin restart session. Unable to determine file locker."; //mxd
				return result;
			}

			try
			{
				const int ERROR_MORE_DATA = 234;
				uint pnProcInfoNeeded,
					 pnProcInfo = 0,
					 lpdwRebootReasons = RmRebootReasonNone;

				string[] resources = new[] { path }; // Just checking on one resource.
				res = RmRegisterResources(handle, (uint)resources.Length, resources, 0, null, 0, null);
				if(res != 0)
				{
					result.Error = errorstart + "Error " + res + ". Could not register resource."; //mxd
					return result;
				}

				//Note: there's a race condition here -- the first call to RmGetList() returns
				//      the total number of process. However, when we call RmGetList() again to get
				//      the actual processes this number may have increased.
				res = RmGetList(handle, out pnProcInfoNeeded, ref pnProcInfo, null, ref lpdwRebootReasons);
				if(res == ERROR_MORE_DATA)
				{
					// Create an array to store the process results
					RM_PROCESS_INFO[] processInfo = new RM_PROCESS_INFO[pnProcInfoNeeded];
					pnProcInfo = pnProcInfoNeeded;

					// Get the list
					res = RmGetList(handle, out pnProcInfoNeeded, ref pnProcInfo, processInfo, ref lpdwRebootReasons);
					if(res == 0)
					{
						result.Processes = new List<Process>((int)pnProcInfo);

						// Enumerate all of the results and add them to the list to be returned
						for(int i = 0; i < pnProcInfo; i++)
						{
							try
							{
								Process process = Process.GetProcessById(processInfo[i].Process.dwProcessId);
								result.Processes.Add(process);
							}
							// catch the error -- in case the process is no longer running
							catch(ArgumentException) {}
						}

						//mxd
						if(result.Processes.Count > 0)
						{
							result.Error = "Unable to save the map: target file is locked by the following process"
							               + (result.Processes.Count > 1 ? "es" : "") + ":"
							               + Environment.NewLine + Environment.NewLine;

							foreach(Process process in result.Processes)
							{
								string processpath = string.Empty;
								try
								{
									// All manner of exceptions are possible here...
									processpath = process.MainModule.FileName;
								}catch {}
								
								result.Error += process.ProcessName 
									+ " (" + (!string.IsNullOrEmpty(processpath) ? "\"" + processpath + "\"" : "")
									+ ", started at " + process.StartTime + ")" 
									+ Environment.NewLine;
							}
						}
					}
					else
					{
						result.Error = "Error " + res + ". Could not list processes locking the resource."; //mxd
						return result;
					}
				}
				else if(res != 0)
				{
					result.Error = "Error " + res + ". Could not list processes locking resource. Failed to get result size."; //mxd
					return result;
				}
			}
			finally
			{
				RmEndSession(handle);
			}

			return result;
		}
	}
}
