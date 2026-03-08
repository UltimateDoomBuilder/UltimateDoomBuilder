#region ================== Namespaces

using System.Collections.Generic;
using CodeImp.DoomBuilder.Actions;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Plugins;
using CodeImp.DoomBuilder.VisualModes;
using CodeImp.DoomBuilder.Windows;

#endregion

namespace CodeImp.DoomBuilder.BuilderEffects
{
	public class BuilderPlug : Plug
	{
		#region ================== Variables

		// Static instance
		private static BuilderPlug me;

		// Main objects
		private MenusForm menusform;

		#endregion

		#region ================== Properties

		public override string Name { get { return "Builder Effects"; } }
		public static BuilderPlug Me { get { return me; } }

		#endregion

		#region ================== Disposer

		public override void Dispose()
		{
			// Not already disposed?
			if(!IsDisposed)
			{
				menusform.Unregister();
				menusform.Dispose();
				menusform = null;

				// Done
				me = null;
				base.Dispose();
			}
		}

		#endregion

		#region ================== Events

		// When plugin is initialized
		public override void OnInitialize() 
		{
			// Setup
			base.OnInitialize();
			me = this;

			// Load menus form
			menusform = new MenusForm();

			General.Actions.BindMethods(this);
		}

		public override void OnMapNewEnd() 
		{
			base.OnMapNewEnd();
			menusform.Register();
		}

		public override void OnMapOpenEnd() 
		{
			base.OnMapOpenEnd();
			menusform.Register();
		}

		public override void OnMapCloseEnd() 
		{
			base.OnMapCloseEnd();
			menusform.Unregister();
		}

		public override void OnReloadResources() 
		{
			base.OnReloadResources();
			menusform.Register();
		}

		#endregion

		#region ================== Actions

		[BeginAction("applyjitter")]
		private void ApplyJitterTransform() 
		{
			if(General.Editing.Mode == null) return;
			string currentModeName = General.Editing.Mode.GetType().Name;
			DelayedForm form = null;

			// Display one of colorPickers or tell the user why we can't do that
			if(currentModeName == "ThingsMode") 
			{
				if(General.Map.Map.SelectedThingsCount == 0) 
				{
					General.Interface.DisplayStatus(StatusType.Warning, "Select some things first!");
					return;
				}
				form = new JitterThingsForm(currentModeName);
			} 
			else if(currentModeName == "SectorsMode") 
			{
				if(General.Map.Map.SelectedSectorsCount == 0) 
				{
					General.Interface.DisplayStatus(StatusType.Warning, "Select some sectors first!");
					return;
				}
				form = new JitterSectorsForm(currentModeName);
			} 
			else if(currentModeName == "LinedefsMode")
			{
				if(General.Map.Map.SelectedLinedefsCount == 0) 
				{
					General.Interface.DisplayStatus(StatusType.Warning, "Select some linedefs first!");
					return;
				}
				form = new JitterVerticesForm(currentModeName);
			} 
			else if(currentModeName == "VerticesMode")
			{
				if(General.Map.Map.SelectedVerticessCount == 0) 
				{
					General.Interface.DisplayStatus(StatusType.Warning, "Select some vertices first!");
					return;
				}
				form = new JitterVerticesForm(currentModeName);
			} 
			else if(currentModeName == "BaseVisualMode") 
			{
				// No visual things selected in visual mode?
				if(((VisualMode)General.Editing.Mode).GetSelectedVisualThings(true).Count == 0) 
				{
					// Check selected geometry
					List<VisualGeometry> list = ((VisualMode)General.Editing.Mode).GetSelectedSurfaces();
					if(list.Count > 0) 
					{
						foreach(VisualGeometry vg in list) 
						{
							if(vg.GeometryType == VisualGeometryType.CEILING 
								|| vg.GeometryType == VisualGeometryType.FLOOR) 
							{
								form = new JitterSectorsForm(currentModeName);
								break;
							}
						}

						if(form == null) form = new JitterVerticesForm(currentModeName);
					} 
					else 
					{
						General.Interface.DisplayStatus(StatusType.Warning, "Select some things, sectors or surfaces first!");
						return;
					}
				} 
				else 
				{
					form = new JitterThingsForm(currentModeName);
				}
			} 
			else // Wrong mode
			{ 
				General.Interface.DisplayStatus(StatusType.Warning, "Switch to Sectors, Things, Vertices, Linedefs or Visual mode first!");
				return;
			}

			form.ShowDialog(General.Interface);
		}

		[BeginAction("applydirectionalshading")]
		private void ApplyDirectionalShading()
		{
			// Boilerplate
			if(General.Editing.Mode == null) return;
			if(!General.Map.UDMF)
			{
				General.Interface.DisplayStatus(StatusType.Warning, "This action is available only in UDMF map format!");
				return;
			}

			DirectionalShadingForm form;
			string currentmodename = General.Editing.Mode.GetType().Name;

			// Create the form or tell the user why we can't do that
			if(currentmodename == "SectorsMode")
			{
				if(General.Map.Map.SelectedSectorsCount == 0)
				{
					General.Interface.DisplayStatus(StatusType.Warning, "Select some sectors first!");
					return;
				}

				// Collect sectors
				ICollection<Sector> sectors = General.Map.Map.GetSelectedSectors(true);

				// Collect sidedefs
				HashSet<Sidedef> sides = new HashSet<Sidedef>();
				foreach(Sector s in sectors)
				{
					foreach(Sidedef sd in s.Sidedefs)
					{
						sides.Add(sd);
						if(sd.Other != null) sides.Add(sd.Other);
					}
				}

				// Create the form
				form = new DirectionalShadingForm(sectors, sides, null);
			}
			else if(currentmodename == "LinedefsMode")
			{
				if(General.Map.Map.SelectedLinedefsCount == 0)
				{
					General.Interface.DisplayStatus(StatusType.Warning, "Select some linedefs first!");
					return;
				}

				// Collect linedefs
				ICollection<Linedef> linedefs = General.Map.Map.GetSelectedLinedefs(true);

				// Collect sectors
				ICollection<Sector> sectors = General.Map.Map.GetSectorsFromLinedefs(linedefs);

				// Collect sidedefs from linedefs
				HashSet<Sidedef> sides = new HashSet<Sidedef>();
				foreach(Linedef l in linedefs)
				{
					if(l.Front != null) sides.Add(l.Front);
					if(l.Back != null) sides.Add(l.Back);
				}

				// Collect sidedefs from sectors
				foreach(Sector s in sectors)
				{
					foreach(Sidedef sd in s.Sidedefs)
					{
						sides.Add(sd);
						if(sd.Other != null) sides.Add(sd.Other);
					}
				}

				// Create the form
				form = new DirectionalShadingForm(sectors, sides, null);
			}
			else if(currentmodename == "BaseVisualMode")
			{
				// Check selected geometry
				VisualMode mode = (VisualMode)General.Editing.Mode;
				List<VisualGeometry> list = mode.GetSelectedSurfaces();
				HashSet<VisualSector> selectedgeo = new HashSet<VisualSector>();
				List<Sector> sectors = new List<Sector>();
				HashSet<Sidedef> sides = new HashSet<Sidedef>();
				
				// Collect sectors and sides
				if(list.Count > 0)
				{
					foreach(VisualGeometry vg in list)
					{
						switch(vg.GeometryType)
						{
							case VisualGeometryType.FLOOR:
								selectedgeo.Add(vg.Sector);
								sectors.Add(vg.Sector.Sector);
								break;

							case VisualGeometryType.WALL_UPPER:
							case VisualGeometryType.WALL_MIDDLE:
							case VisualGeometryType.WALL_LOWER:
								sides.Add(vg.Sidedef);
								selectedgeo.Add(mode.GetVisualSector(vg.Sidedef.Sector));
								break;
						}
					}
				}

				// Add sides from selected sectors
				foreach(Sector s in sectors)
				{
					foreach(Sidedef sd in s.Sidedefs)
					{
						sides.Add(sd);
						if(sd.Other != null) sides.Add(sd.Other);
					}
				}

				// Create the form?
				if(sectors.Count > 0 || sides.Count > 0)
				{
					form = new DirectionalShadingForm(sectors, sides, selectedgeo);
				}
				else
				{
					General.Interface.DisplayStatus(StatusType.Warning, "Select some floor or wall surfaces first!");
					return;
				}
			}
			else // Wrong mode
			{
				General.Interface.DisplayStatus(StatusType.Warning, "Switch to Sectors, Linedefs or Visual mode first!");
				return;
			}

			// Show the form
			form.ShowDialog(General.Interface);
		}

		#endregion
	}
}
