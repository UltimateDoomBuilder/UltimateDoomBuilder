
#region ================== Copyright (c) 2014 Boris Iwanski

/*
 * Copyright (c) 2014 Boris Iwanski
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
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Linq;
using System.Diagnostics;
using CodeImp.DoomBuilder.Windows;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Editing;
using CodeImp.DoomBuilder.Plugins;
using CodeImp.DoomBuilder.Types;
using CodeImp.DoomBuilder.BuilderModes;
// using CodeImp.DoomBuilder.GZBuilder.Geometry;
using CodeImp.DoomBuilder.VisualModes;

#endregion

namespace CodeImp.DoomBuilder.ThreeDFloorMode
{
	//
	// MANDATORY: The plug!
	// This is an important class to the Doom Builder core. Every plugin must
	// have exactly 1 class that inherits from Plug. When the plugin is loaded,
	// this class is instantiated and used to receive events from the core.
	// Make sure the class is public, because only public classes can be seen
	// by the core.
	//
	[Flags]
	public enum PlaneType
	{
		Floor = 1,
		Ceiling = 2,
        Bottom = 4, // Floor of 3D floor control sector
        Top = 8 // Ceiling of 3D floor control sector
	}

	public enum LabelDisplayOption
	{
		Always,
		Never,
		WhenHighlighted,
	}

	public class BuilderPlug : Plug
	{
		#region ================== Variables

		private bool additiveselect;
		private bool additivepaintselect;
		private float highlightrange;
		private bool autoclearselection;
		private MenusForm menusform;
		private bool usehighlight;
		private ControlSectorArea controlsectorarea;
        private float highlightsloperange;
		private List<SlopeVertexGroup> slopevertexgroups;
		private float stitchrange;
		private Sector slopedatasector;
		private bool updateafteraction;
		private string updateafteractionname;
		private LabelDisplayOption sectorlabeldisplayoption;
		private LabelDisplayOption slopevertexlabeldisplayoption;
		private PreferencesForm preferencesform;
		private bool selectchangedafterundoredo;

		// TMP
		public List<Line3D> drawlines;
		public List<Vector3D> drawpoints;

		#endregion

		#region ================== Properties

		public bool AdditiveSelect { get { return additiveselect; } }
		public bool AdditivePaintSelect { get { return additivepaintselect; } }
		public float HighlightRange { get { return highlightrange; } }
		public bool AutoClearSelection { get { return autoclearselection; } }
		public MenusForm MenusForm { get { return menusform; } }
		public bool UseHighlight
		{
			get
			{
				return usehighlight;
			}
			set
			{
				usehighlight = value;
				General.Map.Renderer3D.ShowSelection = usehighlight;
				General.Map.Renderer3D.ShowHighlight = usehighlight;
			}
		}
		public ControlSectorArea ControlSectorArea { get { return controlsectorarea; } }
        public float HighlightSlopeRange { get { return highlightsloperange; } }
		public List<SlopeVertexGroup> SlopeVertexGroups { get { return slopevertexgroups; } set { slopevertexgroups = value; } }
		public float StitchRange { get { return stitchrange; } }
		public bool SelectChangedAfterUndoRedo { get { return selectchangedafterundoredo; } internal set { selectchangedafterundoredo = value; } }

		public Sector SlopeDataSector { get { return slopedatasector; } set { slopedatasector = value; } }

		public LabelDisplayOption SectorLabelDisplayOption { get { return sectorlabeldisplayoption; } set { sectorlabeldisplayoption = value; } }
		public LabelDisplayOption SlopeVertexLabelDisplayOption { get { return slopevertexlabeldisplayoption; } set { slopevertexlabeldisplayoption = value; } }

		#endregion

 		// Static instance. We can't use a real static class, because BuilderPlug must
		// be instantiated by the core, so we keep a static reference. (this technique
		// should be familiar to object-oriented programmers)
		private static BuilderPlug me;

		// Lines tagged to the selected sectors
		private static ThreeDFloorEditorWindow tdfew = new ThreeDFloorEditorWindow();

		public static ThreeDFloorEditorWindow TDFEW { get { return tdfew; } }

		// Static property to access the BuilderPlug
		public static BuilderPlug Me { get { return me; } }

      	// This plugin relies on some functionality that wasn't there in older versions
		public override int MinimumRevision { get { return 1310; } }

		// This event is called when the plugin is initialized
		public override void OnInitialize()
		{
			base.OnInitialize();

			usehighlight = true;

			LoadSettings();

			slopevertexgroups = new List<SlopeVertexGroup>();

			//controlsectorarea = new ControlSectorArea(-512, 0, 512, 0, -128, -64, 128, 64, 64, 56);

			// This binds the methods in this class that have the BeginAction
			// and EndAction attributes with their actions. Without this, the
			// attributes are useless. Note that in classes derived from EditMode
			// this is not needed, because they are bound automatically when the
			// editing mode is engaged.
            General.Actions.BindMethods(this);

			menusform = new MenusForm();

  			// TODO: Add DB2 version check so that old DB2 versions won't crash
			// General.ErrorLogger.Add(ErrorType.Error, "zomg!");

			// Keep a static reference
            me = this;

			// TMP
			drawlines = new List<Line3D>();
			drawpoints = new List<Vector3D>();

		}

		// This is called when the plugin is terminated
		public override void Dispose()
		{
			base.Dispose();

			// This must be called to remove bound methods for actions.
            General.Actions.UnbindMethods(this);
        }

		public override void OnMapNewEnd()
		{
			base.OnMapNewEnd();

			controlsectorarea = new ControlSectorArea(-512, 0, 512, 0, 64, 56);
			BuilderPlug.Me.ControlSectorArea.LoadConfig();

			slopevertexgroups.Clear();

			slopedatasector = null;
		}

		public override void OnMapOpenEnd()
		{
			base.OnMapOpenEnd();

			controlsectorarea = new ControlSectorArea(-512, 0, 512, 0, 64, 56);
			BuilderPlug.Me.ControlSectorArea.LoadConfig();

			// Try to find the slope data sector and store slope information in it
			slopedatasector = GetSlopeDataSector();

			if (slopedatasector != null)
				LoadSlopeVertexGroupsFromSector();
		}

		public override void OnUndoEnd()
		{
			base.OnUndoEnd();

			// Load slope vertex data from the dummy sector
			LoadSlopeVertexGroupsFromSector();
		}

		public override void OnRedoEnd()
		{
			base.OnRedoEnd();

			// Load slope vertex data from the dummy sector
			LoadSlopeVertexGroupsFromSector();
		}
	
		public override void OnActionBegin(CodeImp.DoomBuilder.Actions.Action action)
		{
			base.OnActionBegin(action);

			string[] monitoractions = {
				"buildermodes_raisesector8", "buildermodes_lowersector8", "buildermodes_raisesector1",
				"buildermodes_lowersector1", "builder_visualedit", "builder_classicedit"
			};

			if (General.Editing.Mode is SlopeMode)
				return;

			if (monitoractions.Contains(action.Name))
			{
				updateafteraction = true;
				updateafteractionname = action.Name;
			}
			//else
			//	updateafteraction = false;
		}

		public override void OnActionEnd(CodeImp.DoomBuilder.Actions.Action action)
		{
			base.OnActionEnd(action);

			if (!updateafteraction && action.Name != updateafteractionname)
				return;

			updateafteraction = false;

			Dictionary<SlopeVertexGroup, int> updatesvgs = new Dictionary<SlopeVertexGroup, int>();

			// Find SVGs that needs to be updated, and change the SV z positions
			foreach (SlopeVertexGroup svg in slopevertexgroups)
			{
				bool update = false;
				Dictionary<int, List<Sector>> newheights = new Dictionary<int, List<Sector>>();

				foreach (Sector s in svg.Sectors)
				{
					if (s.Fields == null)
						continue;

					if ((svg.SectorPlanes[s] & PlaneType.Floor) == PlaneType.Floor)
					{
						if (s.Fields.ContainsKey("user_floorplane_id") && s.Fields.GetValue("user_floorplane_id", -1) == svg.Id)
						{
							if (svg.Height != s.FloorHeight)
							{
								int diff = s.FloorHeight - svg.Height;

								if (!newheights.ContainsKey(diff))
									newheights.Add(diff, new List<Sector>() { s });
								else
									newheights[diff].Add(s);

								update = true;
								//break;
							}
						}
					}
					
					if ((svg.SectorPlanes[s] & PlaneType.Ceiling) == PlaneType.Ceiling)
					{
						if (s.Fields.ContainsKey("user_ceilingplane_id") && s.Fields.GetValue("user_ceilingplane_id", -1) == svg.Id)
						{
							if (svg.Height != s.CeilHeight)
							{
								int diff = s.CeilHeight - svg.Height;

								if (!newheights.ContainsKey(diff))
									newheights.Add(diff, new List<Sector>() { s });
								else
									newheights[diff].Add(s);

								update = true;
								//break;
							}
						}
					}
				}

				// Debug.WriteLine(String.Format("floordiff: {0} / ceilingdiff: {1} / height: {2}", floordiff, ceilingdiff, svg.Height));

				if (update)
				{
					if (newheights.Count > 1)
						Debug.WriteLine(String.Format("Slope: multiple new heights, doing nothing. Your map is fucked!"));
					else if (!updatesvgs.ContainsKey(svg))
							updatesvgs.Add(svg, newheights.First().Key);
				}
			}

			// Update the slopes, and also update the view if in visual mode
			foreach (SlopeVertexGroup svg in updatesvgs.Keys)
			{
				foreach (SlopeVertex sv in svg.Vertices)
					sv.Z += updatesvgs[svg];

				svg.ComputeHeight();

				foreach (Sector s in svg.Sectors)
					UpdateSlopes(s);

				// Save the updated data in the sector
				svg.StoreInSector(slopedatasector);

				if (General.Editing.Mode is BaseVisualMode)
				{
					List<Sector> sectors = new List<Sector>();
					List<VisualSector> visualsectors = new List<VisualSector>();
					BaseVisualMode mode = ((BaseVisualMode)General.Editing.Mode);

					foreach (Sector s in svg.Sectors)
					{
						sectors.Add(s);

						// Get neighbouring sectors and add them to the list
						foreach (Sidedef sd in s.Sidedefs)
						{
							if (sd.Other != null && !sectors.Contains(sd.Other.Sector))
								sectors.Add(sd.Other.Sector);
						}
					}

					foreach (Sector s in svg.TaggedSectors)
					{
						if(!sectors.Contains(s))
							sectors.Add(s);

						// Get neighbouring sectors and add them to the list
						foreach (Sidedef sd in s.Sidedefs)
						{
							if (sd.Other != null && !sectors.Contains(sd.Other.Sector))
								sectors.Add(sd.Other.Sector);
						}
					}

					foreach (Sector s in sectors)
						visualsectors.Add(mode.GetVisualSector(s));

					foreach (VisualSector vs in visualsectors) vs.UpdateSectorGeometry(true);
					foreach (VisualSector vs in visualsectors) vs.UpdateSectorData();
				}
			}
		}

		public override bool OnModeChange(EditMode oldmode, EditMode newmode)
		{
			if (newmode != null && oldmode != null)
			{
				if (newmode.GetType().Name == "DragSectorsMode")
				{
					foreach (SlopeVertexGroup svg in slopevertexgroups)
						if (svg.Reposition)
							svg.GetAnchor();
				}
				else if(oldmode.GetType().Name == "DragSectorsMode")
				{
					foreach (SlopeVertexGroup svg in slopevertexgroups)
						if (svg.Reposition)
						{
							svg.RepositionByAnchor();
							svg.StoreInSector(slopedatasector);
						}
				}

			}

			return base.OnModeChange(oldmode, newmode);
		}

		// When the Preferences dialog is shown
		public override void OnShowPreferences(PreferencesController controller)
		{
			base.OnShowPreferences(controller);

			// Load preferences
			preferencesform = new PreferencesForm();
			preferencesform.Setup(controller);
		}

		// When the Preferences dialog is closed
		public override void OnClosePreferences(PreferencesController controller)
		{
			base.OnClosePreferences(controller);

			// Apply settings that could have been changed
			LoadSettings();

			// Unload preferences
			preferencesform.Dispose();
			preferencesform = null;
		}

		#region ================== Actions

		#endregion

		#region ================== Methods

		private Sector GetSlopeDataSector()
		{
			foreach (Sector s in General.Map.Map.Sectors)
			{
				if (s.Fields.GetValue("user_slopedatasector", false) == true)
					return s;
			}

			return null;
		}

		public DialogResult ThreeDFloorEditor()
		{
			List<Sector> selectedSectors = new List<Sector>(General.Map.Map.GetSelectedSectors(true));

			if (selectedSectors.Count <= 0 && General.Editing.Mode.HighlightedObject is Sector)
				selectedSectors.Add((Sector)General.Editing.Mode.HighlightedObject);

			if (tdfew == null)
				tdfew = new ThreeDFloorEditorWindow();

			tdfew.ThreeDFloors = GetThreeDFloors(selectedSectors);
			DialogResult result = tdfew.ShowDialog((Form)General.Interface);

			return result;
		}

		// Use the same settings as the BuilderModes plugin
		private void LoadSettings()
		{
			additiveselect = General.Settings.ReadPluginSetting("BuilderModes", "additiveselect", false);
			additivepaintselect = General.Settings.ReadPluginSetting("BuilderModes", "additivepaintselect", false);
			highlightrange = General.Settings.ReadPluginSetting("BuilderModes", "highlightrange", 20);
			autoclearselection = General.Settings.ReadPluginSetting("BuilderModes", "autoclearselection", false);
			highlightsloperange = (float)General.Settings.ReadPluginSetting("BuilderModes", "highlightthingsrange", 10);
			stitchrange = (float)General.Settings.ReadPluginSetting("BuilderModes", "stitchrange", 20);
			slopevertexlabeldisplayoption = (LabelDisplayOption)General.Settings.ReadPluginSetting("slopevertexlabeldisplayoption", (int)LabelDisplayOption.Always);
			sectorlabeldisplayoption = (LabelDisplayOption)General.Settings.ReadPluginSetting("sectorlabeldisplayoption", (int)LabelDisplayOption.Always);
			selectchangedafterundoredo = General.Settings.ReadPluginSetting("BuilderModes", "selectchangedafterundoredo", false);
		}

		public void StoreSlopeVertexGroupsInSector()
		{
			if (slopedatasector != null && !slopedatasector.IsDisposed)
			{
				slopedatasector.Fields.BeforeFieldsChange();

				if (!slopedatasector.Fields.ContainsKey("user_slopedatasector"))
				{
					slopedatasector.Fields.Add("user_slopedatasector", new UniValue(UniversalType.Boolean, true));
				}

				slopedatasector.Fields["comment"] = new UniValue(UniversalType.String, "[!]DO NOT EDIT OR DELETE! This sector is used by the slope mode for undo/redo operations.");

				foreach (SlopeVertexGroup svg in slopevertexgroups)
					svg.StoreInSector(slopedatasector);
			}
		}

		public void LoadSlopeVertexGroupsFromSector()
		{
			Regex svgregex = new Regex(@"user_svg(\d+)_v0_x", RegexOptions.IgnoreCase);

			slopevertexgroups.Clear();

			if (slopedatasector == null || slopedatasector.IsDisposed)
				return;

			foreach (KeyValuePair<string, UniValue> kvp in slopedatasector.Fields)
			{
				Match svgmatch = svgregex.Match((string)kvp.Key);

				if (svgmatch.Success)
				{
					int svgid = Convert.ToInt32(svgmatch.Groups[1].ToString());

					slopevertexgroups.Add(new SlopeVertexGroup(svgid, slopedatasector));
				}
			}

			General.Map.Map.Update();
		}

		public void UpdateSlopes()
		{
			// foreach (Sector s in General.Map.Map.Sectors)
			//	UpdateSlopes(s);

			foreach (SlopeVertexGroup svg in slopevertexgroups)
			{
				foreach (Sector s in svg.Sectors)
				{
					if (s != null && !s.IsDisposed)
						UpdateSlopes(s);
				}
			}
		}

		public Vector3D CRS(Vector3D p0, Vector3D p1, Vector3D p2, Vector3D p3, float t)
		{
			return 0.5f * ((2 * p1) +
				(-p0 + p2) * t +
				(2 * p0 - 5 * p1 + 4 * p2 - p3) * (t * t) +
				(-p0 + 3 * p1 - 3 * p2 + p3) * (t * t * t)
			);
		}

		public void UpdateSlopes(Sector s)
		{
			string[] fieldnames = new string[] { "user_floorplane_id", "user_ceilingplane_id" };

			foreach (string fn in fieldnames)
			{
				int id = s.Fields.GetValue(fn, -1);

				if (id == -1)
				{
					if (fn == "user_floorplane_id")
					{
						s.FloorSlope = new Vector3D();
						s.FloorSlopeOffset = 0;
					}
					else
					{
						s.CeilSlope = new Vector3D();
						s.CeilSlopeOffset = 0;
					}

					continue;
				}

				List<Vector3D> sp = new List<Vector3D>();
				SlopeVertexGroup svg = GetSlopeVertexGroup(id);

				// If the SVG does not exist unbind the SVG info from this sector
				if (svg == null)
				{
					s.Fields.Remove(fn);
					continue;
				}

				if (svg.Spline)
				{
					Vector2D center = new Vector2D(s.BBox.Width / 2 + s.BBox.X, s.BBox.Height / 2 + s.BBox.Y);
					List<Line3D> splinelines = new List<Line3D>();
					List<Vector3D> tangents = new List<Vector3D>();

					sp.Add(new Vector3D(svg.Vertices[1].Pos.x, svg.Vertices[1].Pos.y, svg.Vertices[1].Z));
					sp.Add(new Vector3D(svg.Vertices[0].Pos.x, svg.Vertices[0].Pos.y, svg.Vertices[0].Z));
					sp.Add(new Vector3D(svg.Vertices[2].Pos.x, svg.Vertices[2].Pos.y, svg.Vertices[2].Z));

					sp.Add(new Vector3D(sp[2].x-96, sp[2].y, sp[2].z-64));
					sp.Insert(0, new Vector3D(sp[0].x+96, sp[0].y, sp[0].z-64));
					//sp.Add(new Vector3D(sp[2] + (sp[2] - sp[1])));
					//sp.Insert(0, new Vector3D(sp[0] + (sp[0] - sp[1])));


					// Create tangents
					tangents.Add(new Vector3D());

					for (int i = 1; i <= 3; i++)
						tangents.Add(new Vector3D((sp[i + 1] - sp[i - 1]) / 2.0f));

					tangents.Add(new Vector3D());

					Debug.Print("----- tangents -----");
					for (int i = 0; i < tangents.Count; i++)
						Debug.Print(tangents[i].ToString());

					for(float u=0.0f; u < 1.0f; u += 0.1f)
					{
						splinelines.Add(new Line3D(
							CRS(sp[0], sp[1], sp[2], sp[3], u),
							CRS(sp[0], sp[1], sp[2], sp[3], u+0.1f)
						));
						/*
						splinelines.Add(new Line3D(
								Tools.HermiteSpline(sp[1], tangents[1], sp[2], tangents[2], u),
								Tools.HermiteSpline(sp[1], tangents[1], sp[2], tangents[2], u+0.1f)
							)
						);
						*/
					}

					for (float u = 0.0f; u < 1.0f; u += 0.1f)
					{
					splinelines.Add(new Line3D(
						CRS(sp[1], sp[2], sp[3], sp[4], u),
						CRS(sp[1], sp[2], sp[3], sp[4], u + 0.1f)
					));
					/*
					splinelines.Add(new Line3D(
							Tools.HermiteSpline(sp[2], tangents[2], sp[3], tangents[3], u),
							Tools.HermiteSpline(sp[2], tangents[2], sp[3], tangents[3], u + 0.1f)
						)
					);
					*/
					}

					drawlines.Clear();
					drawlines.AddRange(splinelines);

					drawpoints.Clear();
					drawpoints.AddRange(sp);

					Line2D sl1 = new Line2D(sp[1], sp[2]);
					Line2D sl2 = new Line2D(sp[2], sp[3]);
					List<Vector3D> points = new List<Vector3D>();

					Debug.Print("----- spline lines -----");
					foreach(Line3D l in splinelines)
						Debug.Print(l.Start.ToString() + " / " + l.End.ToString());

					foreach (Sidedef sd in s.Sidedefs)
					{
						double u = 0.0f;
						Plane ldplane = new Plane(sd.Line.Start.Position, sd.Line.End.Position, new Vector3D(sd.Line.Start.Position.x, sd.Line.Start.Position.y, 128), true);

						foreach(Line3D l in splinelines)
						{
							if(ldplane.GetIntersection(l.Start, l.End, ref u))
							{
								if (u < 0.0f || u > 1.0f)
									continue;

								Vector3D v = (l.End - l.Start) * u + l.Start;
								points.Add(v);
							}
						}

						/*
						if(sd.Line.Line.GetIntersection(sl1, out u))
							points.Add(Tools.HermiteSpline(sp[1], tangents[1], sp[2], tangents[2], u));

						if (sd.Line.Line.GetIntersection(sl2, out u))
							points.Add(Tools.HermiteSpline(sp[2], tangents[2], sp[3], tangents[3], u));
						*/
					}

					
					if (fn == "user_floorplane_id")
					{
						/*
						s.FloorSlope = new Vector3D(p.a, p.b, p.c);
						s.FloorSlopeOffset = p.d;
						*/
					}
					else
					{
						List<Vector3D> ps = new List<Vector3D>();

						if (points.Count > 2)
							points.RemoveAt(0);

						Vector2D perp = new Line2D(points[0], points[1]).GetPerpendicular();

						ps.Add(points[0]);
						ps.Add(points[1]);
						ps.Add(new Vector3D(points[0].x+perp.x, points[0].y+perp.y, points[0].z));

						Debug.Print("----- points -----");
						for (int i = 0; i < ps.Count; i++)
							Debug.Print(ps[i].ToString());

						/*
						for(int i=0; i < ps.Count; i++)
						{
							ps[i] = new Vector3D(ps[i].x, ps[i].z, ps[i].y);
						}
						*/

						Debug.Print("-----");
						for (int i = 0; i < ps.Count; i++)
							Debug.Print(ps[i].ToString());

						Plane p = new Plane(ps[0], ps[1], ps[2], false);

						s.CeilSlope = new Vector3D(p.a, p.b, p.c);
						s.CeilSlopeOffset = p.d;
					}

				}
				else // No spline
				{
					for (int i = 0; i < svg.Vertices.Count; i++)
					{
						sp.Add(new Vector3D(svg.Vertices[i].Pos.x, svg.Vertices[i].Pos.y, svg.Vertices[i].Z));
					}

					if (svg.Vertices.Count == 2)
					{
						double z = sp[0].z;
						Line2D line = new Line2D(sp[0], sp[1]);
						Vector3D perpendicular = line.GetPerpendicular();

						Vector2D v = sp[0] + perpendicular;

						sp.Add(new Vector3D(v.x, v.y, z));
					}

					if (fn == "user_floorplane_id")
					{
						Plane p = new Plane(sp[0], sp[1], sp[2], true);

						s.FloorSlope = new Vector3D(p.a, p.b, p.c);
						s.FloorSlopeOffset = p.d;
						s.FloorHeight = svg.Height;
						svg.Height = s.FloorHeight;
					}
					else
					{
						Plane p = new Plane(sp[0], sp[1], sp[2], false);

						s.CeilSlope = new Vector3D(p.a, p.b, p.c);
						s.CeilSlopeOffset = p.d;
						s.CeilHeight = svg.Height;
						svg.Height = s.CeilHeight;
					}
				}
			}
		}

		public static List<ThreeDFloor> GetThreeDFloors(List<Sector> sectors)
		{
			List<ThreeDFloor> tdf = new List<ThreeDFloor>();
			HashSet<Sector> tmpsectors = new HashSet<Sector>();
			HashSet<Sector> potentialsectors = new HashSet<Sector>();
			Dictionary<int, List<Sector>> tags = new Dictionary<int, List<Sector>>();

			// Immediately return if the list is empty
			if (sectors.Count == 0)
				return tdf;

			// Build a dictionary of tags used by 3D floor action and which control sector they belong to
			foreach (Linedef ld in General.Map.Map.Linedefs)
			{
				// Make sure the front sector is managed by the 3D floor plugin
				if (ld.Front == null || ld.Front.Sector == null || ld.Front.Sector.IsDisposed || (General.Map.UDMF && ld.Front.Sector.Fields.GetValue("user_managed_3d_floor", false) == false))
					continue;

				if (ld.Action == 160 && ld.Args[0] != 0)
				{
					if (!tags.ContainsKey(ld.Args[0]))
						tags.Add(ld.Args[0], new List<Sector>() { ld.Front.Sector });
					else
						tags[ld.Args[0]].Add(ld.Front.Sector);
				}
			}

			// Create a list of 3D floor control sectors that reference the given sectors
			foreach (Sector s in sectors)
			{
				if (s == null || s.IsDisposed)
					continue;

				IEnumerable<int> intersecttags = tags.Keys.Intersect(s.Tags);

				if (intersecttags.Count() == 0)
					continue;

				// This sector is tagged to contain a 3D floor. Using this will speed up creating the 3D floors later
				potentialsectors.Add(s);

				foreach(int it in intersecttags)
				{
					foreach (Sector its in tags[it])
						tmpsectors.Add(its);
				}
			}

			// Create 3D floors from the found sectors
			foreach(Sector s in tmpsectors)
				if(s != null)
					tdf.Add(new ThreeDFloor(s, potentialsectors));

			return tdf;
		}

		public static void ProcessThreeDFloors(List<ThreeDFloor> threedfloors)
		{
			ProcessThreeDFloors(threedfloors, null);
		}

		public static void ProcessThreeDFloors(List<ThreeDFloor> threedfloors, List<Sector> selectedSectors)
		{
			// List<Sector> selectedSectors = new List<Sector>(General.Map.Map.GetSelectedSectors(true));
			var sectorsByTag = new Dictionary<int, List<Sector>>();
			var sectorsToThreeDFloors = new Dictionary<Sector, List<ThreeDFloor>>();
			var sectorGroups = new List<List<Sector>>();
			List<int> tagblacklist = new List<int>();
			int numnewcontrolsectors = 0;

			if(selectedSectors == null)
				selectedSectors = new List<Sector>(General.Map.Map.GetSelectedSectors(true));

			var tmpSelectedSectors = new List<Sector>(selectedSectors);

			foreach (ThreeDFloor tdf in GetThreeDFloors(selectedSectors))
			{
				bool add = true;

				foreach (ThreeDFloor tdf2 in threedfloors)
				{
					if (tdf.Sector == tdf2.Sector)
					{
						add = false;
						break;
					}
				}

				if (add)
				{
					threedfloors.Add(tdf);
				}
			}

			tmpSelectedSectors = new List<Sector>(selectedSectors);

			General.Map.UndoRedo.CreateUndo("Modify 3D floors");

			foreach (ThreeDFloor tdf in threedfloors)
			{
				// Create a list of all tags used by the control sectors. This is necessary so that
				// tags that will be assigned to not yet existing geometry will not be used
				foreach (int tag in tdf.Tags)
					if (!tagblacklist.Contains(tag))
						tagblacklist.Add(tag);

				// Collect the number of control sectors that have to be created
				if (tdf.IsNew)
					numnewcontrolsectors++;
			}

			try
			{
				List<DrawnVertex> drawnvertices = new List<DrawnVertex>();

				if (numnewcontrolsectors > 0)
					drawnvertices = Me.ControlSectorArea.GetNewControlSectorVertices(numnewcontrolsectors);

				foreach (ThreeDFloor tdf in threedfloors)
				{
					if (tdf.IsNew)
						tdf.CreateGeometry(tagblacklist, drawnvertices);

					tdf.UpdateGeometry();
				}
			}
			catch (Exception e)
			{
				MessageBox.Show(e.Message + "\nPlease increase the size of the control sector area.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				General.Map.UndoRedo.WithdrawUndo();
				return;
			}

			// Fill the sectorsToThreeDFloors dictionary, with a selected sector as key
			// and a list of all 3D floors, that should be applied to to this sector, as value
			foreach (Sector s in selectedSectors)
			{
				// Multiple tags is actually a UDMF field, so make sure it gets recorded for undo/redo
				s.Fields.BeforeFieldsChange();

				if (!sectorsToThreeDFloors.ContainsKey(s))
					sectorsToThreeDFloors.Add(s, new List<ThreeDFloor>());

				foreach (ThreeDFloor tdf in threedfloors)
				{
					if (tdf.TaggedSectors.Contains(s))
						sectorsToThreeDFloors[s].Add(tdf);
				}
			}

			// Group all selected sectors by their 3D floors. I.e. each element of sectorGroups
			// is a list of sectors that have the same 3D floors
			while (tmpSelectedSectors.Count > 0)
			{
				Sector s1 = tmpSelectedSectors.First();
				var list = new List<Sector>();
				var delsectors = new List<Sector>();

				foreach (Sector s2 in tmpSelectedSectors)
				{
					if (sectorsToThreeDFloors[s1].ContainsAllElements(sectorsToThreeDFloors[s2]))
					{
						list.Add(s2);
						delsectors.Add(s2);
					}
				}

				foreach (Sector s in delsectors)
					tmpSelectedSectors.Remove(s);

				tmpSelectedSectors.Remove(s1);

				sectorGroups.Add(list);
			}

			// Bind the 3D floors to the selected sectors
			foreach (List<Sector> sectors in sectorGroups)
			{
				if (General.Map.UDMF == true && General.Map.Config.SectorMultiTag)
				{
					foreach (Sector s in sectors)
					{
						// Remove all tags associated to 3D floors from the sector...
						foreach (ThreeDFloor tdf in threedfloors)
						{
							if (s.Tags.Contains(tdf.UDMFTag))
								s.Tags.Remove(tdf.UDMFTag);
						}

						// ... and re-add the ones that are still associated
						foreach (ThreeDFloor tdf in sectorsToThreeDFloors[s])
						{
							if (!s.Tags.Contains(tdf.UDMFTag))
								s.Tags.Add(tdf.UDMFTag);
						}

						// Remove tag 0 if there are other tags present, or add tag 0 if the sector has no tags
						if (s.Tags.Count > 1 && s.Tags.Contains(0))
							s.Tags.Remove(0);

						if(s.Tags.Count == 0)
							s.Tags.Add(0);

					}
				}
				else // No sector multi tagging support
				{
					int newtag;

					// Just use sectors.First(), all elements in sectors have the same 3D floors anyway
					// If there are no 3D floors associated set the tag to 0
					if (sectorsToThreeDFloors[sectors.First()].Count == 0)
						newtag = 0;
					else
						try
						{
							newtag = BuilderPlug.Me.ControlSectorArea.GetNewSectorTag(tagblacklist);
							tagblacklist.Add(newtag);
						}
						catch (Exception e)
						{
							MessageBox.Show(e.Message + "\nPlease increase the custom tag range.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
							General.Map.UndoRedo.WithdrawUndo();
							return;
						}


					foreach (Sector s in sectors)
						s.Tag = newtag;

					try
					{
						foreach (ThreeDFloor tdf in sectorsToThreeDFloors[sectors.First()])
							tdf.BindTag(newtag, null);
					}
					catch (Exception e)
					{
						MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
						General.Map.UndoRedo.WithdrawUndo();
						return;
					}
				}
			}

			// Remove unused tags from the 3D floors
			foreach (ThreeDFloor tdf in threedfloors)
				tdf.Cleanup();

			// Snap to map format accuracy
			General.Map.Map.SnapAllToAccuracy();

			// Update textures
			General.Map.Data.UpdateUsedTextures();

			// Update caches
			General.Map.Map.Update();
			General.Interface.RedrawDisplay();
			General.Map.IsChanged = true;
		}

		public SlopeVertexGroup AddSlopeVertexGroup(List<SlopeVertex> vertices, out int id)
		{
			for (int i = 1; i < int.MaxValue; i++)
			{
				if (!slopevertexgroups.Exists(x => x.Id == i))
				{
					SlopeVertexGroup svg = new SlopeVertexGroup(i, (List<SlopeVertex>)vertices);
					
					slopevertexgroups.Add(svg);

					id = i;

					return svg;
				}
			}

			throw new Exception("No free slope vertex group ids");
		}

		public SlopeVertexGroup GetSlopeVertexGroup(SlopeVertex sv)
		{
			foreach (SlopeVertexGroup svg in slopevertexgroups)
			{
				if (svg.Vertices.Contains(sv))
					return svg;
			}

			return null;
		}

		public SlopeVertexGroup GetSlopeVertexGroup(int id)
		{
			foreach (SlopeVertexGroup svg in slopevertexgroups)
			{
				if (svg.Id == id)
					return svg;
			}

			return null;
		}

		public SlopeVertexGroup GetSlopeVertexGroup(Sector s)
		{
			foreach (SlopeVertexGroup svg in slopevertexgroups)
			{
				if (svg.Sectors.Contains(s))
					return svg;
			}

			return null;
		}

		public static List<Sector> GetSectorsByTag(int tag)
		{
			return GetSectorsByTag(General.Map.Map.Sectors, tag);
		}

		public static List<Sector> GetSectorsByTag(IEnumerable sectors, int tag)
		{
			List<Sector> taggedsectors = new List<Sector>();

			foreach (Sector s in sectors)
				if (s.Tags.Contains(tag))
					taggedsectors.Add(s);

			return taggedsectors;
		}

		#endregion
	}

	public static class ThreeDFloorHelpers
	{
		public static bool ContainsAllElements<T>(this List<T> list1, List<T> list2)
		{
			if (list1.Count != list2.Count)
				return false;

			foreach (T i in list1)
				if (!list2.Contains(i))
					return false;

			return true;
		}

		// Taken from http://stackoverflow.com/questions/10816803/finding-next-available-key-in-a-dictionary-or-related-collection
		// Add item to sortedList (numeric key) to next available key item, and return key
		public static int AddNext<T>(this SortedList<int, T> sortedList, T item)
		{
			int key = 1; // Make it 0 to start from Zero based index
			int count = sortedList.Count;

			int counter = 0;
			do
			{
				if (count == 0) break;
				int nextKeyInList = sortedList.Keys[counter++];

				if (key != nextKeyInList) break;

				key = nextKeyInList + 1;

				if (count == 1 || counter == count) break;


				if (key != sortedList.Keys[counter])
					break;

			} while (true);

			sortedList.Add(key, item);
			return key;
		}
	}
}
