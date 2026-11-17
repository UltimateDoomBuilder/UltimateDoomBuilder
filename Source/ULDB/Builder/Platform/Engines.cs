// ULDB: starting a test engine on Linux, including from inside the Flatpak sandbox, and finding the
// engines that are installed. Modelled on how other Linux Doom launchers (DoomRunner) do it:
//  - a native engine is started directly;
//  - inside ULDB's own Flatpak the sandbox can't run the host's engines, so everything goes through
//    `flatpak-spawn --host` (needs the org.freedesktop.Flatpak permission in the manifest);
//  - an engine that is itself a Flatpak is started with `flatpak run`, with read access to the folders
//    the command line names (the test map, the IWAD and resources), since Flatpak apps are sandboxed too.
// Engines that aren't ordinary files are stored in the game configuration as pseudo paths:
//  "flatpak:<application id>" (e.g. flatpak:org.zdoom.GZDoom) and "host:<command or path>" (a program on
//  the host that ULDB's sandbox can't see).

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ULDB.Platform
{
	public sealed class EngineInstall
	{
		/// <summary>What the list shows, e.g. "GZDoom (Flatpak)".</summary>
		public string Name { get; set; }
		/// <summary>The value for the game configuration's test program: a file path or a pseudo path.</summary>
		public string Path { get; set; }
		/// <summary>"GZDoom", "UZDoom" or null.</summary>
		public string Family { get; set; }
	}

	public static class Engines
	{
		public const string FLATPAK_PREFIX = "flatpak:";
		public const string HOST_PREFIX = "host:";

		private static readonly string[] ENGINE_COMMANDS = { "gzdoom", "uzdoom" };
		private static readonly Regex applicationid = new Regex(@"^[A-Za-z0-9_-]+(\.[A-Za-z0-9_-]+)+$", RegexOptions.Compiled);

		/// <summary>True when ULDB itself runs in a Flatpak sandbox.</summary>
		public static bool InFlatpak => File.Exists("/.flatpak-info");

		public static bool IsVirtual(string path) =>
			path != null && (path.StartsWith(FLATPAK_PREFIX, StringComparison.Ordinal) || path.StartsWith(HOST_PREFIX, StringComparison.Ordinal));

		/// <summary>Whether a test program setting is usable (pseudo paths can't be checked from here).</summary>
		public static bool Exists(string path) => !string.IsNullOrEmpty(path) && (IsVirtual(path) || File.Exists(path));

		/// <summary>"GZDoom" or "UZDoom" when the program's name says so, else null.</summary>
		public static string FamilyOf(string path)
		{
			string lower = (path ?? "").ToLowerInvariant();
			if(lower.Contains("uzdoom")) return "UZDoom";
			if(lower.Contains("gzdoom")) return "GZDoom";
			return null;
		}

		/// <summary>A readable name for a test program setting.</summary>
		public static string DisplayName(string path)
		{
			if(string.IsNullOrEmpty(path)) return "";
			if(path.StartsWith(FLATPAK_PREFIX, StringComparison.Ordinal)) return (FamilyOf(path) ?? path.Substring(FLATPAK_PREFIX.Length)) + " (Flatpak)";
			if(path.StartsWith(HOST_PREFIX, StringComparison.Ordinal)) return FamilyOf(path) ?? System.IO.Path.GetFileName(path.Substring(HOST_PREFIX.Length));
			return System.IO.Path.GetFileNameWithoutExtension(path);
		}

		#region ================== Starting

		/// <summary>
		/// Rewrites a start request (Launcher's: program in FileName, one argument string) for the way the engine has to be
		/// started here. Leaves an ordinary engine started from a native ULDB as it is.
		/// </summary>
		public static void Prepare(ProcessStartInfo info)
		{
			string program = info.FileName ?? "";
			bool plain = !IsVirtual(program);
			bool inflatpak = InFlatpak;
			if(plain && !inflatpak) return;

			string workdir = Directory.Exists(info.WorkingDirectory) ? info.WorkingDirectory : "";
			List<string> command = BuildCommand(program, SplitArguments(info.Arguments), workdir, inflatpak);

			info.FileName = command[0];
			info.Arguments = "";
			info.ArgumentList.Clear();
			foreach(string part in command.Skip(1)) info.ArgumentList.Add(part);
			info.UseShellExecute = false; // ArgumentList needs it, and these are real executables on the PATH
			info.WorkingDirectory = inflatpak ? "" : workdir;
		}

		/// <summary>The command line that starts the program (first item) with its arguments.</summary>
		public static List<string> BuildCommand(string program, IList<string> arguments, string workingdirectory, bool inflatpak)
		{
			List<string> command = new List<string>();
			if(program.StartsWith(FLATPAK_PREFIX, StringComparison.Ordinal))
			{
				command.Add("flatpak");
				command.Add("run");
				foreach(string dir in FolderPermissions(arguments)) command.Add("--filesystem=" + dir + ":ro");
				command.Add(program.Substring(FLATPAK_PREFIX.Length));
			}
			else if(program.StartsWith(HOST_PREFIX, StringComparison.Ordinal))
			{
				command.Add(program.Substring(HOST_PREFIX.Length));
			}
			else
			{
				command.Add(program);
			}
			command.AddRange(arguments);

			if(!inflatpak) return command;

			// The sandbox can't run the host's programs (or other Flatpaks); the host starts them for us.
			List<string> spawn = new List<string> { "flatpak-spawn", "--host" };
			if(!string.IsNullOrEmpty(workingdirectory)) spawn.Add("--directory=" + workingdirectory);
			spawn.AddRange(command);
			return spawn;
		}

		/// <summary>The folders a sandboxed engine has to read: where each absolute path in the arguments is.</summary>
		public static List<string> FolderPermissions(IEnumerable<string> arguments)
		{
			List<string> dirs = new List<string>();
			foreach(string argument in arguments)
			{
				if(string.IsNullOrEmpty(argument) || argument[0] != '/') continue;
				string dir = Directory.Exists(argument) ? argument : System.IO.Path.GetDirectoryName(argument);
				if(string.IsNullOrEmpty(dir) || dir == "/" || dirs.Contains(dir) || IsReserved(dir)) continue;
				dirs.Add(dir);
			}
			return dirs;
		}

		// Flatpak refuses --filesystem for the runtime's own folders (and /tmp is the engine's private one).
		private static readonly string[] RESERVED = { "/usr", "/etc", "/app", "/bin", "/sbin", "/lib", "/lib32", "/lib64", "/boot", "/dev", "/proc", "/sys", "/run", "/tmp" };

		private static bool IsReserved(string dir) =>
			RESERVED.Any(r => dir == r || dir.StartsWith(r + "/", StringComparison.Ordinal));

		/// <summary>Splits a command line the way a program's arguments are quoted: double quotes group, \" is a quote.</summary>
		public static List<string> SplitArguments(string line)
		{
			List<string> result = new List<string>();
			StringBuilder current = new StringBuilder();
			bool inquotes = false, has = false;
			string s = line ?? "";
			for(int i = 0; i < s.Length; i++)
			{
				char c = s[i];
				if(c == '\\' && i + 1 < s.Length && s[i + 1] == '"') { current.Append('"'); has = true; i++; }
				else if(c == '"') { inquotes = !inquotes; has = true; }
				else if(char.IsWhiteSpace(c) && !inquotes)
				{
					if(has) { result.Add(current.ToString()); current.Clear(); has = false; }
				}
				else { current.Append(c); has = true; }
			}
			if(has) result.Add(current.ToString());
			return result;
		}

		#endregion

		#region ================== Finding installed engines

		/// <summary>GZDoom and UZDoom installs: programs on the PATH and Flatpak applications. Slow (starts programs): call from a background thread.</summary>
		public static List<EngineInstall> Detect()
		{
			List<EngineInstall> found = new List<EngineInstall>();
			bool inflatpak = InFlatpak;
			string output;
			if(inflatpak)
			{
				output = Capture("flatpak-spawn", new[] { "--host", "sh", "-c", "for c in " + string.Join(" ", ENGINE_COMMANDS) + "; do command -v \"$c\"; done; flatpak list --app --columns=application 2>/dev/null" });
			}
			else
			{
				StringBuilder lines = new StringBuilder();
				foreach(string command in ENGINE_COMMANDS)
					foreach(string dir in (Environment.GetEnvironmentVariable("PATH") ?? "").Split(':', StringSplitOptions.RemoveEmptyEntries))
					{
						string candidate = System.IO.Path.Combine(dir, command);
						if(File.Exists(candidate)) { lines.AppendLine(candidate); break; }
					}
				lines.Append(Capture("flatpak", new[] { "list", "--app", "--columns=application" }));
				output = lines.ToString();
			}
			ParseDetected(output, inflatpak, found);
			return found;
		}

		/// <summary>Turns the lines Detect collects (paths, then Flatpak application ids) into installs.</summary>
		public static void ParseDetected(string output, bool inflatpak, List<EngineInstall> found)
		{
			foreach(string raw in (output ?? "").Split('\n'))
			{
				string line = raw.Trim();
				if(line.Length == 0) continue;
				string family = FamilyOf(line);
				if(family == null) continue;
				if(line[0] == '/')
				{
					string path = inflatpak ? HOST_PREFIX + line : line;
					if(found.All(f => f.Path != path)) found.Add(new EngineInstall { Name = family + " (" + line + ")", Path = path, Family = family });
				}
				else if(applicationid.IsMatch(line))
				{
					string path = FLATPAK_PREFIX + line;
					if(found.All(f => f.Path != path)) found.Add(new EngineInstall { Name = family + " (Flatpak)", Path = path, Family = family });
				}
			}
		}

		private static string Capture(string program, IEnumerable<string> arguments)
		{
			try
			{
				ProcessStartInfo info = new ProcessStartInfo(program) { RedirectStandardOutput = true, RedirectStandardError = true, UseShellExecute = false, CreateNoWindow = true };
				foreach(string a in arguments) info.ArgumentList.Add(a);
				using(Process p = Process.Start(info))
				{
					p.ErrorDataReceived += (s, e) => { };
					p.BeginErrorReadLine();
					Task_Read(p, out string text);
					if(!p.WaitForExit(5000)) { try { p.Kill(true); } catch(Exception) { } }
					return text;
				}
			}
			catch(Exception) { return ""; } // the program isn't there
		}

		private static void Task_Read(Process p, out string text)
		{
			// The reader may still be running after the wait: it hands over a finished string, never a buffer in use
			string result = "";
			System.Threading.Thread reader = new System.Threading.Thread(() => { try { result = p.StandardOutput.ReadToEnd(); } catch(Exception) { } }) { IsBackground = true };
			reader.Start();
			reader.Join(5000);
			text = System.Threading.Volatile.Read(ref result);
		}

		#endregion
	}
}
