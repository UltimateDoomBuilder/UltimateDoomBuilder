using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using CodeImp.DoomBuilder;
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.Editing;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Plugins;

namespace ULDB.Tests
{
	// The in-scope plugins (docs/ULDB-Plan.md, section 0) build with placeholders for their
	// unported UI. This loads them the way General.Main does, then opens a map and switches
	// into every edit mode they add, so a placeholder that breaks a plugin's logic shows up
	// here instead of in the editor.
	[Collection("Editor")]
	public class PluginTests
	{
		private static readonly string[] PLUGINS =
		{
			"BuilderModes", "ThreeDFloorMode", "BuilderEffects", "StairSectorBuilder", "TagRange",
			"SoundPropagationMode", "ColorPicker", "TagExplorer", "CommentsPanel",
		};

		private readonly Xunit.Abstractions.ITestOutputHelper output;

		public PluginTests(Xunit.Abstractions.ITestOutputHelper output) { this.output = output; }

		// Plugins are copied next to the Builder executable (ULDB.<Name>.csproj, CopyToBuilderPlugins).
		private static string PluginsPath => Path.Combine(HeadlessEditor.RepoRoot, "Source", "ULDB", "Builder", "bin",
			new DirectoryInfo(AppContext.BaseDirectory).Parent.Name, "net10.0", "Plugins");

		/// <summary>Runs the test with every plugin loaded and an editing manager that knows their modes.</summary>
		private static void WithPlugins(Action<PluginManager> test)
		{
			HeadlessEditor.Initialize();
			object oldpath = HeadlessEditor.GetGeneralField("pluginspath");
			object oldplugins = HeadlessEditor.GetGeneralField("plugins");
			object oldediting = HeadlessEditor.GetGeneralField("editing");
			object oldactions = HeadlessEditor.GetGeneralField("actions");
			object oldhints = HeadlessEditor.GetGeneralField("hints");
			PluginManager plugins = new PluginManager();
			try
			{
				// Fresh actions each time, so loading the plugins again doesn't redefine theirs.
				HeadlessEditor.SetGeneralField("actions", new CodeImp.DoomBuilder.Actions.ActionManager());
				General.Actions.BindMethods(typeof(General));
				HeadlessEditor.SetGeneralField("hints", new CodeImp.DoomBuilder.Actions.HintsManager());
				HeadlessEditor.SetGeneralField("pluginspath", PluginsPath);
				HeadlessEditor.SetGeneralField("plugins", plugins);
				plugins.LoadAllPlugins();
				HeadlessEditor.SetGeneralField("editing", new EditingManager()); // Collects the plugins' modes
				test(plugins);
			}
			finally
			{
				HeadlessEditor.SetGeneralField("editing", oldediting);
				HeadlessEditor.SetGeneralField("actions", oldactions);
				HeadlessEditor.SetGeneralField("hints", oldhints);
				HeadlessEditor.SetGeneralField("plugins", oldplugins);
				HeadlessEditor.SetGeneralField("pluginspath", oldpath);
			}
		}

		[Fact]
		public void InScopePluginsLoad()
		{
			Assert.True(Directory.Exists(PluginsPath), "No plugins folder at " + PluginsPath);
			WithPlugins(plugins =>
			{
				List<string> loaded = plugins.Plugins.Select(p => p.Assembly.GetName().Name).ToList();
				output.WriteLine("Loaded: " + string.Join(", ", loaded));
				foreach(string name in PLUGINS)
					Assert.Contains(name, loaded);

				// Each plugin's own actions (from its embedded Actions.cfg) are registered.
				foreach(string action in new[] { "threedfloormode_threedfloorhelpermode", "buildereffects_applyjitter", "stairsectorbuilder_stairsectorbuildermode", "tagrange_rangetagselection", "soundpropagationmode_soundpropagationmode", "colorpicker_togglelightpannel" })
					Assert.True(General.Actions.Exists(action), "Action not registered: " + action);
			});
		}

		[SkippableFact]
		public void PluginModesEngageOnAMap()
		{
			string iwad = TestResources.FreedoomIwad;
			Skip.If(iwad == null, "Freedoom 2 IWAD not found; install the freedoom package or set ULDB_TEST_IWAD");
			// Modes run background workers whose progress callbacks redraw the map: in the editor
			// they run on the UI thread, so here they're queued and run on the test's thread.
			UIThread ui = new UIThread();
			SynchronizationContext oldcontext = SynchronizationContext.Current;
			SynchronizationContext.SetSynchronizationContext(ui);
			try { WithPlugins(plugins => EngageModes(plugins, iwad, ui)); }
			finally { SynchronizationContext.SetSynchronizationContext(oldcontext); }
		}

		private void EngageModes(PluginManager plugins, string iwad, UIThread ui)
		{
			{
				string source = Path.Combine(HeadlessEditor.TempDir, "plugins-" + Guid.NewGuid().ToString("N") + ".wad");
				File.Copy(HeadlessEditor.TestMap("3dfloors_slopes.wad"), source);
				using(OffscreenGL gl = new OffscreenGL())
				{
					MapOptions options = new MapOptions { ConfigFile = "UZDoom_DoomUDMF.cfg", CurrentName = "MAP01", ScriptCompiler = "zdoom_acs.cfg" };
					options.Resources.Add(new DataLocation(DataLocation.RESOURCE_WAD, iwad, false, false, false, null));
					MapManager manager = new MapManager();
					HeadlessEditor.SetGeneralField("map", manager);
					try
					{
						Assert.True(manager.InitializeOpenMap(source, options), "InitializeOpenMap failed");
						plugins.OnMapOpenEnd();

						// Every mode a plugin other than BuilderModes adds, from a classic mode and back.
						List<EditModeInfo> modes = General.Editing.ModesInfo.Where(m => m.Plugin != null && m.Plugin.Assembly.GetName().Name != "BuilderModes").ToList();
						output.WriteLine("Plugin modes: " + string.Join(", ", modes.Select(m => m.Type.Name)));
						Assert.NotEmpty(modes);
						List<string> failures = new List<string>();
						foreach(EditModeInfo mode in modes)
						{
							try
							{
								General.Editing.ChangeMode("SectorsMode");
								General.Editing.ChangeMode(mode.Type.Name);
								ui.RunUntilIdle(); // Background work the mode started (sound environments)
								output.WriteLine(mode.Type.Name + " → " + General.Editing.Mode?.GetType().Name);
								General.Editing.ChangeMode("SectorsMode");
								ui.RunUntilIdle();
							}
							catch(Exception e)
							{
								failures.Add(mode.Type.Name + ": " + e);
								try { General.Map.Renderer2D.Finish(); } catch(Exception) { } // A failed redraw leaves a layer open
							}
						}
						Assert.True(failures.Count == 0, string.Join("\n\n", failures));
						Assert.Equal("SectorsMode", General.Editing.Mode?.GetType().Name);
					}
					finally
					{
						try { General.Editing.ChangeMode((EditMode)null); } catch(Exception e) { output.WriteLine("Leaving mode: " + e.Message); }
						try { manager.Dispose(); } catch(Exception e) { output.WriteLine("Dispose: " + e); }
						HeadlessEditor.SetGeneralField("map", null);
					}
				}
			}
		}

		// The Undo/Redo docker: picking a level in its list (the view sets the items' selection, raises
		// SelectedIndexChanged, then MouseUp) undoes to that level, as upstream's panel does on Windows.
		[SkippableFact]
		public void PickingALevelInTheUndoPanelUndoesToIt()
		{
			string iwad = TestResources.FreedoomIwad;
			Skip.If(iwad == null, "Freedoom 2 IWAD not found; install the freedoom package or set ULDB_TEST_IWAD");
			UIThread ui = new UIThread();
			SynchronizationContext oldcontext = SynchronizationContext.Current;
			SynchronizationContext.SetSynchronizationContext(ui);
			try
			{
				WithPlugins(plugins =>
				{
					string source = Path.Combine(HeadlessEditor.TempDir, "undopanel-" + Guid.NewGuid().ToString("N") + ".wad");
					File.Copy(HeadlessEditor.TestMap("small.wad"), source);
					using(OffscreenGL gl = new OffscreenGL())
					{
						MapOptions options = new MapOptions { ConfigFile = "UZDoom_DoomUDMF.cfg", CurrentName = "MAP01", ScriptCompiler = "zdoom_acs.cfg" };
						options.Resources.Add(new DataLocation(DataLocation.RESOURCE_WAD, iwad, false, false, false, null));
						MapManager manager = new MapManager();
						HeadlessEditor.SetGeneralField("map", manager);
						try
						{
							Assert.True(manager.InitializeOpenMap(source, options), "InitializeOpenMap failed");
							plugins.OnMapOpenEnd();

							System.Reflection.Assembly asm = plugins.Plugins.First(p => p.Assembly.GetName().Name == "BuilderModes").Assembly;
							Type plugtype = asm.GetType("CodeImp.DoomBuilder.BuilderModes.BuilderPlug");
							const System.Reflection.BindingFlags any = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic;
							object plug = plugtype.GetProperty("Me", any).GetValue(null);
							object panel = plugtype.GetField("undoredopanel", any).GetValue(plug);
							System.Windows.Forms.ListView list = (System.Windows.Forms.ListView)panel.GetType().GetField("list", any).GetValue(panel);

							Vertex v = manager.Map.Vertices.First();
							CodeImp.DoomBuilder.Geometry.Vector2D start = v.Position;
							manager.UndoRedo.CreateUndo("Move 1");
							v.Move(start + new CodeImp.DoomBuilder.Geometry.Vector2D(8, 0));
							manager.UndoRedo.CreateUndo("Move 2");
							v.Move(start + new CodeImp.DoomBuilder.Geometry.Vector2D(16, 0));

							Assert.Equal(3, list.Items.Count);
							Assert.Equal("Move 2", list.Items[2].Text);
							Assert.Equal(System.Drawing.SystemColors.Highlight, list.Items[2].BackColor); // the current level

							// What the view does for a click on the first level.
							foreach(System.Windows.Forms.ListViewItem i in list.Items) i.Selected = false;
							list.Items[0].Selected = true;
							list.RaiseSelectedIndexChanged();
							list.RaiseMouseUp(new System.Windows.Forms.MouseEventArgs(System.Windows.Forms.MouseButtons.Left, 1, 4, 11, 0));

							Assert.Equal(start, v.Position);
							Assert.Empty(manager.UndoRedo.GetUndoList());
							Assert.Equal(2, manager.UndoRedo.GetRedoList().Count);
							Assert.Equal(System.Drawing.SystemColors.Highlight, list.Items[0].BackColor); // now the current level
							Assert.Empty(list.SelectedIndices); // the pick is cleared once done

							// And forward again, to the last level.
							foreach(System.Windows.Forms.ListViewItem i in list.Items) i.Selected = false;
							list.Items[2].Selected = true;
							list.RaiseSelectedIndexChanged();
							list.RaiseMouseUp(new System.Windows.Forms.MouseEventArgs(System.Windows.Forms.MouseButtons.Left, 1, 4, 55, 0));
							Assert.Equal(start + new CodeImp.DoomBuilder.Geometry.Vector2D(16, 0), v.Position);
							Assert.Empty(manager.UndoRedo.GetRedoList());
						}
						finally
						{
							try { General.Editing.ChangeMode((EditMode)null); } catch(Exception) { }
							try { manager.Dispose(); } catch(Exception e) { output.WriteLine("Dispose: " + e); }
							HeadlessEditor.SetGeneralField("map", null);
						}
					}
				});
			}
			finally { SynchronizationContext.SetSynchronizationContext(oldcontext); }
		}

		// "Select Connected Linedefs" (ULDB): from one linedef, everything reachable through shared vertices.
		[SkippableFact]
		public void SelectConnectedLinedefsSelectsTheWholeChain()
		{
			string iwad = TestResources.FreedoomIwad;
			Skip.If(iwad == null, "Freedoom 2 IWAD not found; install the freedoom package or set ULDB_TEST_IWAD");
			UIThread ui = new UIThread();
			SynchronizationContext oldcontext = SynchronizationContext.Current;
			SynchronizationContext.SetSynchronizationContext(ui);
			try
			{
				WithPlugins(plugins =>
				{
					string source = Path.Combine(HeadlessEditor.TempDir, "selconn-" + Guid.NewGuid().ToString("N") + ".wad");
					File.Copy(HeadlessEditor.TestMap("small.wad"), source);
					using(OffscreenGL gl = new OffscreenGL())
					{
						MapOptions options = new MapOptions { ConfigFile = "UZDoom_DoomUDMF.cfg", CurrentName = "MAP01", ScriptCompiler = "zdoom_acs.cfg" };
						options.Resources.Add(new DataLocation(DataLocation.RESOURCE_WAD, iwad, false, false, false, null));
						MapManager manager = new MapManager();
						HeadlessEditor.SetGeneralField("map", manager);
						try
						{
							Assert.True(manager.InitializeOpenMap(source, options), "InitializeOpenMap failed");
							plugins.OnMapOpenEnd();
							General.Editing.ChangeMode("LinedefsMode");
							Assert.Equal("LinedefsMode", General.Editing.Mode?.GetType().Name);

							Linedef start = manager.Map.Linedefs.First();
							manager.Map.ClearAllSelected();
							start.Selected = true;
							Assert.True(General.Actions.Exists("buildermodes_selectconnectedlinedefs"));
							General.Actions.InvokeAction("buildermodes_selectconnectedlinedefs");

							HashSet<Linedef> expected = new HashSet<Linedef> { start };
							Queue<Linedef> queue = new Queue<Linedef>();
							queue.Enqueue(start);
							while(queue.Count > 0)
							{
								Linedef l = queue.Dequeue();
								foreach(Vertex v in new[] { l.Start, l.End })
									foreach(Linedef o in v.Linedefs)
										if(expected.Add(o)) queue.Enqueue(o);
							}
							HashSet<Linedef> selected = new HashSet<Linedef>(manager.Map.Linedefs.Where(l => l.Selected));
							Assert.True(expected.Count > 1);
							Assert.True(expected.SetEquals(selected), expected.Count + " expected, " + selected.Count + " selected");
						}
						finally
						{
							try { General.Editing.ChangeMode((EditMode)null); } catch(Exception) { }
							try { manager.Dispose(); } catch(Exception e) { output.WriteLine("Dispose: " + e); }
							HeadlessEditor.SetGeneralField("map", null);
						}
					}
				});
			}
			finally { SynchronizationContext.SetSynchronizationContext(oldcontext); }
		}

		/// <summary>Stands in for the UI thread: posted callbacks queue up and run on the caller's thread.</summary>
		private sealed class UIThread : SynchronizationContext
		{
			private readonly BlockingCollection<(SendOrPostCallback, object)> queue = new BlockingCollection<(SendOrPostCallback, object)>();

			public override void Post(SendOrPostCallback d, object state) => queue.Add((d, state));
			public override void Send(SendOrPostCallback d, object state) => d(state);

			/// <summary>Runs queued callbacks until none arrive for a while (or a time limit passes).</summary>
			public void RunUntilIdle()
			{
				Stopwatch limit = Stopwatch.StartNew();
				while(limit.Elapsed < TimeSpan.FromSeconds(30) && queue.TryTake(out var item, 500))
					item.Item1(item.Item2);
			}
		}
	}
}
