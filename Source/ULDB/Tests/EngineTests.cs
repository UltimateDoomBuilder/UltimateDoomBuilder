using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using ULDB.Platform;
using Xunit;

namespace ULDB.Tests
{
	public class EngineTests
	{
		[Fact]
		public void ArgumentsSplitLikeAProgramWouldSeeThem()
		{
			Assert.Equal(new[] { "-iwad", "/a b/doom2.wad", "-file", "/tmp/x.wad", "+map", "MAP01" },
				Engines.SplitArguments("-iwad \"/a b/doom2.wad\" -file /tmp/x.wad +map MAP01"));
			Assert.Equal(new[] { "say \"hi\"" }, Engines.SplitArguments("\"say \\\"hi\\\"\""));
			Assert.Empty(Engines.SplitArguments("   "));
		}

		[Fact]
		public void PseudoPathsAreRecognisedAndNamed()
		{
			Assert.True(Engines.Exists("flatpak:org.zdoom.GZDoom"));
			Assert.True(Engines.Exists("host:/usr/bin/uzdoom"));
			Assert.False(Engines.Exists("/definitely/not/here/gzdoom"));
			Assert.False(Engines.Exists(""));
			Assert.Equal("GZDoom (Flatpak)", Engines.DisplayName("flatpak:org.zdoom.GZDoom"));
			Assert.Equal("UZDoom", Engines.DisplayName("host:/usr/games/uzdoom"));
			Assert.Equal("UZDoom", Engines.FamilyOf("/home/me/Games/UZDoom/uzdoom"));
			Assert.Equal("GZDoom", Engines.FamilyOf("/opt/gzdoom/gzdoom"));
			Assert.Null(Engines.FamilyOf("/usr/bin/zandronum"));
		}

		[Fact]
		public void ANativeEngineIsStartedDirectly()
		{
			Assert.Equal(new[] { "/opt/gzdoom/gzdoom", "-iwad", "/x/doom2.wad" },
				Engines.BuildCommand("/opt/gzdoom/gzdoom", new[] { "-iwad", "/x/doom2.wad" }, "/opt/gzdoom", false));
		}

		[Fact]
		public void AFlatpakEngineGetsReadAccessToTheFoldersItIsGiven()
		{
			string dir = Path.Combine(ULDB.Platform.Paths.TempDirectory, "uldb-engine-" + Guid.NewGuid().ToString("N")); // not /tmp: Flatpak won't share that
			Directory.CreateDirectory(dir);
			string wad = Path.Combine(dir, "test.wad");
			File.WriteAllText(wad, "");
			try
			{
				List<string> command = Engines.BuildCommand("flatpak:org.zdoom.GZDoom", new[] { "-file", wad, "+map", "MAP01" }, "", false);
				Assert.Equal(new[] { "flatpak", "run", "--filesystem=" + dir + ":ro", "org.zdoom.GZDoom", "-file", wad, "+map", "MAP01" }, command);
			}
			finally { Directory.Delete(dir, true); }
		}

		[Fact]
		public void FoldersFlatpakWontShareAreLeftOut()
		{
			Assert.Empty(Engines.FolderPermissions(new[] { "/usr/share/games/doom/doom2.wad", "/tmp/x/test.wad", "/etc/foo", "-iwad", "+map" }));
		}

		[Fact]
		public void FlatpakEngineGetsSystemFilesThroughHostOs()
		{
			List<string> command = Engines.BuildCommand("flatpak:org.zdoom.GZDoom", new[] { "-iwad", "/usr/share/games/doom/freedoom2.wad", "+map", "MAP01" }, "", false);
			Assert.Equal(new[] { "flatpak", "run", "--filesystem=host-os:ro", "org.zdoom.GZDoom", "-iwad", "/run/host/usr/share/games/doom/freedoom2.wad", "+map", "MAP01" }, command);
			// A native engine reads the file where it is
			Assert.Equal(new[] { "/usr/bin/gzdoom", "-iwad", "/usr/share/games/doom/freedoom2.wad" },
				Engines.BuildCommand("/usr/bin/gzdoom", new[] { "-iwad", "/usr/share/games/doom/freedoom2.wad" }, "", false));
			Assert.Equal(new[] { "/tmp/x/test.wad" }, Engines.Unreachable("flatpak:org.zdoom.GZDoom", new[] { "-file", "/tmp/x/test.wad", "/usr/share/a.wad", "/home/u/b.wad" }));
			Assert.Empty(Engines.Unreachable("/usr/bin/gzdoom", new[] { "/tmp/x/test.wad" }));
		}

		[Fact]
		public void InsideFlatpakEverythingGoesThroughTheHost()
		{
			Assert.Equal(new[] { "flatpak-spawn", "--host", "--directory=/opt/uz", "/opt/uz/uzdoom", "+map", "MAP01" },
				Engines.BuildCommand("/opt/uz/uzdoom", new[] { "+map", "MAP01" }, "/opt/uz", true));
			Assert.Equal(new[] { "flatpak-spawn", "--host", "gzdoom", "-nosound" },
				Engines.BuildCommand("host:gzdoom", new[] { "-nosound" }, "", true));
			Assert.Equal(new[] { "flatpak-spawn", "--host", "flatpak", "run", "org.zdoom.UZDoom" },
				Engines.BuildCommand("flatpak:org.zdoom.UZDoom", new string[0], "", true));
		}

		[Fact]
		public void InstalledEnginesAreReadFromTheDetectionOutput()
		{
			string output = "/usr/bin/gzdoom\n/home/me/bin/uzdoom\norg.zdoom.GZDoom\norg.gnome.Calculator\nio.github.uzdoom.UZDoom\nnot an id\n";
			List<EngineInstall> found = new List<EngineInstall>();
			Engines.ParseDetected(output, false, found);
			Assert.Equal(new[] { "/usr/bin/gzdoom", "/home/me/bin/uzdoom", "flatpak:org.zdoom.GZDoom", "flatpak:io.github.uzdoom.UZDoom" }, found.Select(f => f.Path));
			Assert.Equal(new[] { "GZDoom", "UZDoom", "GZDoom", "UZDoom" }, found.Select(f => f.Family));

			List<EngineInstall> sandboxed = new List<EngineInstall>();
			Engines.ParseDetected("/usr/bin/gzdoom\n", true, sandboxed);
			Assert.Equal("host:/usr/bin/gzdoom", sandboxed.Single().Path); // the sandbox can't see /usr/bin
		}

		[Fact]
		public void ALaunchedEngineRunsWithTheArgumentsItWasGiven()
		{
			string dir = Path.Combine(Path.GetTempPath(), "uldb-engine-" + Guid.NewGuid().ToString("N"));
			Directory.CreateDirectory(dir);
			string script = Path.Combine(dir, "fakeengine"), outfile = Path.Combine(dir, "out.txt");
			File.WriteAllText(script, "#!/bin/sh\nprintf '%s|' \"$@\" > \"" + outfile + "\"\n");
			File.SetUnixFileMode(script, UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.UserExecute);
			try
			{
				// "host:" engines are started as the named program, as a ULDB outside a sandbox does
				ProcessStartInfo info = new ProcessStartInfo { FileName = "host:" + script, Arguments = "-file \"/a b/c.wad\" +map MAP01", UseShellExecute = true };
				if(Engines.InFlatpak) return; // the test machine isn't a Flatpak; nothing to prove here if it is
				Engines.Prepare(info);
				Assert.Equal(script, info.FileName);
				using(Process p = Process.Start(info)) Assert.True(p.WaitForExit(10000));
				Assert.Equal("-file|/a b/c.wad|+map|MAP01|", File.ReadAllText(outfile));
			}
			finally { Directory.Delete(dir, true); }
		}

		[Fact]
		public void LaunchOptionsAreAddedAndRemovedFromTheParameters()
		{
			LaunchOption windowed = EnginePresets.OptionsFor("GZDoom").Single(o => o.Arguments == "+vid_fullscreen 0");
			LaunchOption nomonsters = EnginePresets.OptionsFor("UZDoom").Single(o => o.Arguments == "-nomonsters");
			string parameters = "-iwad \"%WP/%WF\" -file \"%AP/%AF\" +map %L";

			Assert.False(EnginePresets.IsSet(parameters, windowed));
			string on = EnginePresets.Set(parameters, windowed, true);
			Assert.Equal(parameters + " +vid_fullscreen 0", on);
			Assert.True(EnginePresets.IsSet(on, windowed));
			Assert.Equal(on, EnginePresets.Set(on, windowed, true)); // not added twice

			string both = EnginePresets.Set(on, nomonsters, true);
			Assert.True(EnginePresets.IsSet(both, windowed) && EnginePresets.IsSet(both, nomonsters));
			Assert.Equal(parameters + " -nomonsters", EnginePresets.Set(both, windowed, false));
			Assert.Equal(parameters, EnginePresets.Set(EnginePresets.Set(both, windowed, false), nomonsters, false));
		}

		[Fact]
		public void OnlyTheKnownFamiliesHaveOptions()
		{
			Assert.NotEmpty(EnginePresets.OptionsFor("GZDoom"));
			Assert.NotEmpty(EnginePresets.OptionsFor("UZDoom"));
			Assert.Empty(EnginePresets.OptionsFor(null));
			Assert.Empty(EnginePresets.OptionsFor("Zandronum"));
		}
	}
}
