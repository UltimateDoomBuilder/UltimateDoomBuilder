
#region ================== Copyright (c) 2007 Pascal vd Heiden, 2014 Boris Iwanski

/*
 * Copyright (c) 2007 Pascal vd Heiden, www.codeimp.com
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
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Linq;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Windows;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Editing;
using CodeImp.DoomBuilder.Actions;
using CodeImp.DoomBuilder.BuilderModes;
using CodeImp.DoomBuilder.Controls;
using CodeImp.DoomBuilder.Config;

#endregion

namespace CodeImp.DoomBuilder.ThreeDFloorMode
{
	[EditMode(DisplayName = "3D Floor Mode",
			  SwitchAction = "threedfloorhelpermode",		// Action name used to switch to this mode
			  ButtonImage = "ThreeDFloorIcon.png",	// Image resource name for the button
			  ButtonOrder = int.MinValue + 501,	// Position of the button (lower is more to the left)
			  ButtonGroup = "000_editing",
			  SupportedMapFormats = new[] { "HexenMapSetIO", "UniversalMapSetIO" },
			  RequiredMapFeatures = new[] { "Effect3DFloorSupport" },
			  UseByDefault = true,
			  SafeStartMode = false,
			  Volatile = false)]

	public class ThreeDFloorHelperMode : ClassicMode
	{
		#region ================== Constants

		private const string duplicateundodescription = "Duplicate 3D floor control sectors before pasting";

		#endregion

		#region ================== Variables

		// Highlighted item
		protected Sector highlighted;
		protected ThreeDFloor highlighted3dfloor;
		private Association highlightasso;
		private FlatVertex[] overlayGeometry;
		private FlatVertex[] overlaygeometry3dfloors;
		private FlatVertex[] overlaygeometry3dfloors_highlighted;
		private FlatVertex[] overlaygeometry3dfloors_selected;

		// Interface
		private ThreeDFloorPanel panel;
		private Docker docker;

		// Labels
		private Dictionary<Sector, TextLabel[]> labels;
		private Dictionary<Sector, string[]> selected3Dfloorlabels;
		private Dictionary<Sector, string[]> unselected3Dfloorlabels;

		List<ThreeDFloorHelperTooltipElementControl> tooltipelements;

		ControlSectorArea.Highlight csahighlight = ControlSectorArea.Highlight.None;
		bool dragging = false;
		bool withdrawduplicateundo;

		bool paintselectpressed;

		private List<ThreeDFloor> threedfloors;
		private BlockMap<BlockEntry> blockmap;

		#endregion

		#region ================== Properties

		public override object HighlightedObject { get { return highlighted; } }
		public List<ThreeDFloor> ThreeDFloors { get { return threedfloors; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public ThreeDFloorHelperMode()
		{
			threedfloors = BuilderPlug.GetThreeDFloors(General.Map.Map.Sectors.ToList());
			highlightasso = new Association(renderer);

			withdrawduplicateundo = false;

			// If we're coming from EditSelectionMode, and that modes was cancelled check if the last undo was to create
			// duplicated 3D floors. If that's the case we want to withdraw that undo, too. Don't do it here, though, as the other
			// mode is still active, do it in OnEngage instead
			if (General.Editing.Mode is EditSelectionMode &&
				((EditSelectionMode)General.Editing.Mode).Cancelled &&
				General.Map.UndoRedo.NextUndo != null &&
				General.Map.UndoRedo.NextUndo.Description == duplicateundodescription)
			{
				withdrawduplicateundo = true;
			}
		}

		// Disposer
		public override void Dispose()
		{
			// Not already disposed?
			if(!isdisposed)
			{
				// Dispose old labels
				foreach(KeyValuePair<Sector, TextLabel[]> lbl in labels)
					foreach(TextLabel l in lbl.Value) l.Dispose();

				// Dispose base
				base.Dispose();
			}
		}

		#endregion

		#region ================== Methods



		// This makes a CRC for the selection
		public int CreateSelectionCRC()
		{
			CRC crc = new CRC();
			ICollection<Sector> orderedselection = General.Map.Map.GetSelectedSectors(true);
			crc.Add(orderedselection.Count);
			foreach(Sector s in orderedselection)
			{
				crc.Add(s.FixedIndex);
			}
			return (int)(crc.Value & 0xFFFFFFFF);
		}

		// This sets up new labels
		private void SetupLabels()
		{
			if(labels != null)
			{
				// Dispose old labels
				foreach(KeyValuePair<Sector, TextLabel[]> lbl in labels)
					foreach(TextLabel l in lbl.Value) l.Dispose();
			}

			// Make text labels for sectors
			labels = new Dictionary<Sector, TextLabel[]>(General.Map.Map.Sectors.Count);
			foreach(Sector s in General.Map.Map.Sectors)
			{
				// Setup labels
				TextLabel[] labelarray = new TextLabel[s.Labels.Count];
				for(int i = 0; i < s.Labels.Count; i++)
				{
					labelarray[i] = new TextLabel();
					labelarray[i].TransformCoords = true;
					labelarray[i].Location = s.Labels[i].position;
					labelarray[i].AlignX = TextAlignmentX.Center;
					labelarray[i].AlignY = TextAlignmentY.Middle;
					labelarray[i].Color = General.Colors.Highlight.WithAlpha(255);
					labelarray[i].BackColor = General.Colors.Background.WithAlpha(128);
				}
				labels.Add(s, labelarray);
			}
		}

		// This updates the overlay
		private void UpdateOverlay()
		{
			if(renderer.StartOverlay(true))
			{
				if (BuilderPlug.Me.UseHighlight)
				{
					renderer.RenderHighlight(overlayGeometry, General.Colors.Selection.WithAlpha(64).ToInt());
				}

				if (BuilderPlug.Me.UseHighlight && highlighted != null)
				{
					renderer.RenderHighlight(highlighted.FlatVertices, General.Colors.Highlight.WithAlpha(64).ToInt());

					if (highlighted3dfloor != null)
					{
						renderer.RenderHighlight(overlaygeometry3dfloors, General.Colors.ModelWireframe.WithAlpha(64).ToInt());

						//show the selected sectors in a darker shade
						PixelColor darker = General.Colors.ModelWireframe;
						darker.r = (byte)(darker.r * 0.5);
						darker.g = (byte)(darker.g * 0.5);
						darker.b = (byte)(darker.b * 0.5);
						renderer.RenderHighlight(overlaygeometry3dfloors_selected, darker.WithAlpha(64).ToInt());

						//show the highlighted sectors in a lighter shade
						PixelColor lighter = General.Colors.ModelWireframe;
						lighter.r = (byte)(lighter.r + (0.5 * (255 - lighter.r)));
						lighter.g = (byte)(lighter.g + (0.5 * (255 - lighter.g)));
						lighter.b = (byte)(lighter.b + (0.5 * (255 - lighter.b)));
						renderer.RenderHighlight(overlaygeometry3dfloors_highlighted, lighter.WithAlpha(64).ToInt());
					}
				}

				if (BuilderModes.BuilderPlug.Me.ViewSelectionNumbers)
				{
					// Go for all selected sectors
					ICollection<Sector> orderedselection = General.Map.Map.GetSelectedSectors(true);
					foreach(Sector s in orderedselection)
					{
						// Render labels
						TextLabel[] labelarray = labels[s];
						for(int i = 0; i < s.Labels.Count; i++)
						{
							TextLabel l = labelarray[i];

							// Render only when enough space for the label to see
							float requiredsize = (l.TextSize.Height / 2) / renderer.Scale;
							if(requiredsize < s.Labels[i].radius) renderer.RenderText(l);
						}
					}
				}

				Render3DFloorLabels(unselected3Dfloorlabels);

				if (!BuilderModes.BuilderPlug.Me.ViewSelectionNumbers)
					Render3DFloorLabels(selected3Dfloorlabels);

				BuilderPlug.Me.ControlSectorArea.Draw(renderer, csahighlight);

				renderer.Finish();
			}
		}

		private void Render3DFloorLabels(Dictionary<Sector, string[]> labelsgroup)
		{
			Dictionary<string, float> sizecache = new Dictionary<string, float>();
			List<ITextLabel> textlabels = new List<ITextLabel>();

			foreach (KeyValuePair<Sector, string[]> group in labelsgroup)
			{
				// Render labels
				TextLabel[] labelarray = labels[group.Key];
				for (int i = 0; i < group.Key.Labels.Count; i++)
				{
					TextLabel l = labelarray[i];
					l.Color = General.Colors.InfoLine;

					// Render only when enough space for the label to see. MeasureString is expensive, so cache the result
					if (!sizecache.ContainsKey(group.Value[0]))
						sizecache[group.Value[0]] = (General.Interface.MeasureString(group.Value[0], l.Font).Width / 2) / renderer.Scale;

					float requiredsize = sizecache[group.Value[0]];

					if (requiredsize > group.Key.Labels[i].radius)
					{
						if (!sizecache.ContainsKey(group.Value[1]))
							sizecache[group.Value[1]] = (General.Interface.MeasureString(group.Value[1], l.Font).Width / 2) / renderer.Scale;

						requiredsize = sizecache[group.Value[1]];

						if (requiredsize > group.Key.Labels[i].radius)
							l.Text = (requiredsize > group.Key.Labels[i].radius * 4 ? string.Empty : "+");
						else
							l.Text = group.Value[1];
					}
					else
					{
						l.Text = group.Value[0];
					}

					if(!string.IsNullOrEmpty(l.Text))
						textlabels.Add(l);
				}
			}

			renderer.RenderText(textlabels);
		}

		// Generates the tooltip for the 3D floors
		private void UpdateDocker(Sector s)
		{
			List<ThreeDFloor> tdfs = new List<ThreeDFloor>();
			int count = 0;

			// Get all 3D floors that have the currently highlighted sector tagged. Also order them by their vertical position
			foreach (ThreeDFloor tdf in threedfloors.Where(o => o.TaggedSectors.Contains(s)).OrderByDescending(o => o.TopHeight))
				tdfs.Add(tdf);

			// Hide all controls if no sector is selected or selected sector has no 3D floors
			if (s == null || tdfs.Count == 0)
			{
				foreach (Control c in tooltipelements)
				{
					c.Visible = false;
				}

				return;
			}

			foreach (ThreeDFloor tdf in tdfs)
			{
				// Add another control if the list if full
				if (count >= tooltipelements.Count)
				{
					var tte = new ThreeDFloorHelperTooltipElementControl();
					panel.flowLayoutPanel1.Controls.Add(tte);
					tooltipelements.Add(tte);
				}

				tooltipelements[count].sectorBottomFlat.Image = General.Map.Data.GetFlatImage(tdf.BottomFlat).GetPreview();
				tooltipelements[count].sectorTopFlat.Image = General.Map.Data.GetFlatImage(tdf.TopFlat).GetPreview();
				tooltipelements[count].sectorBorderTexture.Image = General.Map.Data.GetFlatImage(tdf.BorderTexture).GetPreview();


				//General.DisplayZoomedImage(tooltipelements[count].sectorBottomFlat, General.Map.Data.GetFlatImage(tdf.BottomFlat).GetPreview());
				//General.DisplayZoomedImage(tooltipelements[count].sectorBorderTexture, General.Map.Data.GetFlatImage(tdf.BorderTexture).GetPreview());
				//General.DisplayZoomedImage(tooltipelements[count].sectorTopFlat, General.Map.Data.GetFlatImage(tdf.TopFlat).GetPreview());

				tooltipelements[count].bottomHeight.Text = tdf.BottomHeight.ToString();
				tooltipelements[count].topHeight.Text = tdf.TopHeight.ToString();
				tooltipelements[count].borderHeight.Text = (tdf.TopHeight - tdf.BottomHeight).ToString();

				if (tdf == highlighted3dfloor)
					// tooltipelements[count].BackColor = General.Colors.ModelWireframe.ToColor();
					tooltipelements[count].Highlighted = true;
				else
					// tooltipelements[count].BackColor = SystemColors.Control;
					tooltipelements[count].Highlighted = false;

				tooltipelements[count].Refresh();
				tooltipelements[count].Visible = true;

				count++;
			}

			// Hide superfluous controls
			for (; count < tooltipelements.Count; count++)
			{
				tooltipelements[count].Visible = false;
			}
		}

		private void updateOverlaySurfaces()
		{
			ICollection<Sector> orderedselection = General.Map.Map.GetSelectedSectors(true);
			List<FlatVertex> vertsList = new List<FlatVertex>();
			List<FlatVertex> vertsList_highlighted = new List<FlatVertex>();
			List<FlatVertex> vertsList_selected = new List<FlatVertex>();

			// Go for all selected sectors
			foreach (Sector s in orderedselection) vertsList.AddRange(s.FlatVertices);
			overlayGeometry = vertsList.ToArray();

			if (highlighted3dfloor != null)
			{
				vertsList = new List<FlatVertex>();
				vertsList_highlighted = new List<FlatVertex>();
				vertsList_selected = new List<FlatVertex>();

				foreach (Sector s in highlighted3dfloor.TaggedSectors)
				{
					//bin the verticies so that then can be properly colored
					if (s == highlighted)
						vertsList_highlighted.AddRange(s.FlatVertices);
					else if (s.Selected)
						vertsList_selected.AddRange(s.FlatVertices);
					else
						vertsList.AddRange(s.FlatVertices);
				}

				overlaygeometry3dfloors = vertsList.ToArray();
				overlaygeometry3dfloors_highlighted = vertsList_highlighted.ToArray();
				overlaygeometry3dfloors_selected = vertsList_selected.ToArray();
			}
		}
		
		// Support function for joining and merging sectors
		private void JoinMergeSectors(bool removelines)
		{
			// Remove lines in betwen joining sectors?
			if(removelines)
			{
				// Go for all selected linedefs
				List<Linedef> selectedlines = new List<Linedef>(General.Map.Map.GetSelectedLinedefs(true));
				foreach(Linedef ld in selectedlines)
				{
					// Front and back side?
					if((ld.Front != null) && (ld.Back != null))
					{
						// Both a selected sector, but not the same?
						if(ld.Front.Sector.Selected && ld.Back.Sector.Selected &&
						   (ld.Front.Sector != ld.Back.Sector))
						{
							// Remove this line
							ld.Dispose();
						}
					}
				}
			}

			// Find the first sector that is not disposed
			List<Sector> orderedselection = new List<Sector>(General.Map.Map.GetSelectedSectors(true));
			Sector first = null;
			foreach(Sector s in orderedselection)
				if(!s.IsDisposed) { first = s; break; }
			
			// Join all selected sectors with the first
			for(int i = 0; i < orderedselection.Count; i++)
				if((orderedselection[i] != first) && !orderedselection[i].IsDisposed)
					orderedselection[i].Join(first);

			// Clear selection
			General.Map.Map.ClearAllSelected();
			
			// Update
			General.Map.Map.Update();
			
			// Make text labels for sectors
			SetupLabels();
			UpdateLabels();
		}

		// This highlights a new item
		protected void Highlight(Sector s)
		{
			bool completeredraw = false;

			// Often we can get away by simply undrawing the previous
			// highlight and drawing the new highlight. But if associations
			// are or were drawn we need to redraw the entire display.

			// Previous association highlights something?
			if((highlighted != null) && (highlighted.Tag > 0)) completeredraw = true;

			// Set highlight association
			if (s != null)
			{
				Vector2D center = (s.Labels.Count > 0 ? s.Labels[0].position : new Vector2D(s.BBox.X + s.BBox.Width / 2, s.BBox.Y + s.BBox.Height / 2));
				highlightasso.Set(s);
			}
			else
				highlightasso.Clear();

			// New association highlights something?
			if((s != null) && (s.Tag > 0)) completeredraw = true;

			// Change label color
			if((highlighted != null) && !highlighted.IsDisposed)
			{
				TextLabel[] labelarray = labels[highlighted];
				foreach(TextLabel l in labelarray) l.Color = General.Colors.Selection;
			}
			
			// Change label color
			if((s != null) && !s.IsDisposed)
			{
				TextLabel[] labelarray = labels[s];
				foreach(TextLabel l in labelarray) l.Color = General.Colors.Highlight;
			}
			
			// If we're changing associations, then we
			// need to redraw the entire display
			if(completeredraw)
			{
				// Set new highlight and redraw completely
				highlighted = s;
				highlighted3dfloor = null;
				General.Interface.RedrawDisplay();
			}
			else
			{
				// Update display
				if(renderer.StartPlotter(false))
				{
					// Undraw previous highlight
					if((highlighted != null) && !highlighted.IsDisposed)
						renderer.PlotSector(highlighted);
					
					// Set new highlight
					highlighted = s;

					// Render highlighted item
					if((highlighted != null) && !highlighted.IsDisposed)
						renderer.PlotSector(highlighted, General.Colors.Highlight);
					
					// Done
					renderer.Finish();
				}
				
				UpdateOverlay();
				renderer.Present();
			}

			// Update the panel with the 3D floors
			UpdateDocker(s);

			// Show highlight info
			if((highlighted != null) && !highlighted.IsDisposed)
				General.Interface.ShowSectorInfo(highlighted);
			else
				General.Interface.HideInfo();
		}

		// This selectes or deselects a sector
		protected void SelectSector(Sector s, bool selectstate, bool update)
		{
			bool selectionchanged = false;

			if(!s.IsDisposed)
			{
				// Select the sector?
				if(selectstate && !s.Selected)
				{
					s.Selected = true;
					selectionchanged = true;
					
					// Setup labels
					ICollection<Sector> orderedselection = General.Map.Map.GetSelectedSectors(true);
					TextLabel[] labelarray = labels[s];
					foreach(TextLabel l in labelarray)
					{
						l.Text = orderedselection.Count.ToString();
						l.Color = General.Colors.Selection;
					}
				}
				// Deselect the sector?
				else if(!selectstate && s.Selected)
				{
					s.Selected = false;
					selectionchanged = true;

					// Clear labels
					TextLabel[] labelarray = labels[s];
					foreach(TextLabel l in labelarray) l.Text = "";
				}

				// Selection changed?
				if(selectionchanged)
				{
					// Make update lines selection
					foreach(Sidedef sd in s.Sidedefs)
					{
						bool front, back;
						if(sd.Line.Front != null) front = sd.Line.Front.Sector.Selected; else front = false;
						if(sd.Line.Back != null) back = sd.Line.Back.Sector.Selected; else back = false;
						sd.Line.Selected = front | back;
					}

					// Also (de)select things?
					if (General.Interface.AltState ^ BuilderModes.BuilderPlug.Me.SyncronizeThingEdit)
					{
						List<BlockEntry> belist = blockmap.GetSquareRange(s.BBox);

						foreach (BlockEntry be in belist)
						{
							foreach (Thing t in be.Things)
							{
								// Always determine the thing's current sector because it might have change since the last determination
								t.DetermineSector(blockmap);

								if (t.Sector == s && t.Selected != s.Selected) t.Selected = s.Selected;
							}
						}
					}

					// Update all other labels
					UpdateLabels();
				}

				if(update)
				{
					UpdateOverlay();
					renderer.Present();
				}
			}
		}

		private void UpdateLabels()
		{
			Update3DFloorLabels();

			if (BuilderModes.BuilderPlug.Me.ViewSelectionNumbers)
				UpdateSelectedLabels();
		}

		// This updates labels from the selected sectors
		private void UpdateSelectedLabels()
		{
			// Go for all labels in all selected sectors
			ICollection<Sector> orderedselection = General.Map.Map.GetSelectedSectors(true);
			int index = 0;
			foreach(Sector s in orderedselection)
			{
				TextLabel[] labelarray = labels[s];
				foreach(TextLabel l in labelarray)
				{
					// Make sure the text and color are right
					int labelnum = index + 1;
					l.Text = labelnum.ToString();
					l.Color = General.Colors.Selection;
				}
				index++;
			}
		}

		// Update labels for 3D floors
		private void Update3DFloorLabels()
		{
			Dictionary<Sector, int> num3dfloors = new Dictionary<Sector, int>();
			selected3Dfloorlabels = new Dictionary<Sector, string[]>();
			unselected3Dfloorlabels = new Dictionary<Sector, string[]>();

			foreach (ThreeDFloor tdf in threedfloors)
			{
				foreach (Sector s in tdf.TaggedSectors)
				{
					if (num3dfloors.ContainsKey(s))
						num3dfloors[s]++;
					else
						num3dfloors.Add(s, 1);
				}
			}

			foreach (KeyValuePair<Sector, int> group in num3dfloors)
			{
				if (group.Key.Selected)
					selected3Dfloorlabels.Add(group.Key, new string[] { group.Value + (group.Value == 1 ? " floor" : " floors"), group.Value.ToString() });
				else
					unselected3Dfloorlabels.Add(group.Key, new string[] { group.Value + (group.Value == 1 ? " floor" : " floors"), group.Value.ToString() });
			}

			/*
			foreach (Sector s in General.Map.Map.GetSelectedSectors(true))
			{
				List<ThreeDFloor> tdfs = BuilderPlug.GetThreeDFloors(new List<Sector> { s });

				if (tdfs.Count == 0)
					selected3Dfloorlabels.Add(s, new string[] { "", "" });
				else
					selected3Dfloorlabels.Add(s, new string[] { tdfs.Count + (tdfs.Count == 1 ? " floor" : " floors"), tdfs.Count.ToString() });
			}

			foreach (Sector s in General.Map.Map.GetSelectedSectors(false))
			{
				List<ThreeDFloor> tdfs = BuilderPlug.GetThreeDFloors(new List<Sector> { s });

				if (tdfs.Count == 0)
					unselected3Dfloorlabels.Add(s, new string[] { "", "" });
				else
					unselected3Dfloorlabels.Add(s, new string[] { tdfs.Count + (tdfs.Count == 1 ? " floor" : " floors"), tdfs.Count.ToString() });
			}
			*/
		}

		/// <summary>
		/// Create a blockmap containing sectors and things. This is used to speed determining which sector a
		/// thing is in when synchronized thing editing is enabled
		/// </summary>
		private void CreateBlockmap()
		{
			RectangleF area = MapSet.CreateArea(General.Map.Map.Vertices);
			area = MapSet.IncreaseArea(area, General.Map.Map.Things);
			blockmap = new BlockMap<BlockEntry>(area);
			blockmap.AddSectorsSet(General.Map.Map.Sectors);
			blockmap.AddThingsSet(General.Map.Map.Things);

			// Don't add linedefs here. They are only needed for paint select, so let's save some
			// time (and add them when paint select is used t he first time)
			//addedlinedefstoblockmap = false;
		}

		#endregion

		#region ================== Events

		public override void OnHelp()
		{
			General.ShowHelp("gzdb/features/classic_modes/mode_3dfloor.html");
		}

		// Cancel mode
		public override void OnCancel()
		{
			base.OnCancel();

			// Return to this mode
			General.Editing.ChangeMode(new SectorsMode());
		}

		// Mode engages
		public override void OnEngage()
		{
			base.OnEngage();

			renderer.SetPresentation(Presentation.Standard);

			tooltipelements = new List<ThreeDFloorHelperTooltipElementControl>();

			// Add docker
			panel = new ThreeDFloorPanel();
			docker = new Docker("threedfloorhelper", "3D floors", panel);
			General.Interface.AddDocker(docker);
			General.Interface.SelectDocker(docker);

			// Add the view selection number button from BuilderModes. Also add a click event handler
			// so we can update the labels when the button is pressed
			General.Interface.AddButton(BuilderModes.BuilderPlug.Me.MenusForm.ViewSelectionNumbers);
			BuilderModes.BuilderPlug.Me.MenusForm.ViewSelectionNumbers.Click += ViewSelectionNumbers_Click;

			// Synchronize thing editing from BuilderModes
			General.Interface.AddButton(BuilderModes.BuilderPlug.Me.MenusForm.SyncronizeThingEditButton);

			General.Interface.AddButton(BuilderPlug.Me.MenusForm.RelocateControlSectors);

			// Convert geometry selection to sectors only
			General.Map.Map.ConvertSelection(SelectionType.Sectors);

			// Create the blockmap
			CreateBlockmap();

			// Select things in the selected sectors if synchronized thing editing is enabled
			if (BuilderModes.BuilderPlug.Me.SyncronizeThingEdit)
			{
				ICollection<Sector> sectors = General.Map.Map.GetSelectedSectors(true);

				foreach (Sector s in sectors)
				{
					List<BlockEntry> belist = blockmap.GetSquareRange(s.BBox);

					foreach (BlockEntry be in belist)
					{
						foreach (Thing t in be.Things)
						{
							// Always determine the thing's current sector because it might have change since the last determination
							t.DetermineSector(blockmap);

							if (t.Sector == s && t.Selected != s.Selected) t.Selected = s.Selected;
						}
					}
				}
			}

			// Make text labels for sectors
			SetupLabels();
			
			// Update
			UpdateLabels();
			updateOverlaySurfaces();
			UpdateOverlay();

			// Withdraw the undo that was created when 
			if (withdrawduplicateundo)
				General.Map.UndoRedo.WithdrawUndo();
		}

		void ViewSelectionNumbers_Click(object sender, EventArgs e)
		{
			UpdateLabels();

			OnRedrawDisplay();
		}

		// Mode disengages
		public override void OnDisengage()
		{
			base.OnDisengage();

			// Remove docker
			General.Interface.RemoveDocker(docker);

			// Remove the button and event handler for view selection numbers
			General.Interface.RemoveButton(BuilderModes.BuilderPlug.Me.MenusForm.ViewSelectionNumbers);
			BuilderModes.BuilderPlug.Me.MenusForm.ViewSelectionNumbers.Click -= ViewSelectionNumbers_Click;

			General.Interface.RemoveButton(BuilderModes.BuilderPlug.Me.MenusForm.SyncronizeThingEditButton);

			General.Interface.RemoveButton(BuilderPlug.Me.MenusForm.RelocateControlSectors);

			// Keep only sectors selected
			General.Map.Map.ClearSelectedLinedefs();
			
			// Going to EditSelectionMode?
			if(General.Editing.NewMode is EditSelectionMode)
			{
				// Not pasting anything?
				EditSelectionMode editmode = (General.Editing.NewMode as EditSelectionMode);
				if(!editmode.Pasting)
				{
					// No selection made? But we have a highlight!
					if((General.Map.Map.GetSelectedSectors(true).Count == 0) && (highlighted != null))
					{
						// Make the highlight the selection
						SelectSector(highlighted, true, false);
					}
				}
			}

			BuilderPlug.Me.ControlSectorArea.SaveConfig();
			
			// Hide highlight info
			General.Interface.HideInfo();
		}

		// This redraws the display
		public override void OnRedrawDisplay()
		{
			renderer.RedrawSurface();
			
			// Render lines and vertices
			if(renderer.StartPlotter(true))
			{
				renderer.PlotLinedefSet(General.Map.Map.Linedefs);
				renderer.PlotVerticesSet(General.Map.Map.Vertices);
				if((highlighted != null) && !highlighted.IsDisposed)
				{
					renderer.PlotSector(highlighted, General.Colors.Highlight);
					// BuilderPlug.Me.PlotReverseAssociations(renderer, highlightasso);
				}

				renderer.Finish();
			}

			// Render things
			if(renderer.StartThings(true))
			{
				renderer.RenderThingSet(General.Map.ThingsFilter.HiddenThings, Presentation.THINGS_HIDDEN_ALPHA);
				renderer.RenderThingSet(General.Map.ThingsFilter.VisibleThings, 1.0f);
				renderer.Finish();
			}

			// Render selection
			if(renderer.StartOverlay(true))
			{

				// if((highlighted != null) && !highlighted.IsDisposed) BuilderPlug.Me.RenderReverseAssociations(renderer, highlightasso);
				if(selecting) RenderMultiSelection();

				renderer.Finish();
			}

			// Render overlay
			UpdateOverlay();
			
			renderer.Present();
		}

		// Selection
		protected override void OnSelectBegin()
		{
			// Item highlighted?
			if((highlighted != null) && !highlighted.IsDisposed)
			{
				// Update display
				if(renderer.StartPlotter(false))
				{
					// Redraw highlight to show selection
					renderer.PlotSector(highlighted);
					renderer.Finish();
					renderer.Present();
				}
			}

			base.OnSelectBegin();
		}

		// End selection
		protected override void OnSelectEnd()
		{
			// Not stopping from multiselection?
			if(!selecting)
			{
				// Item highlighted?
				if((highlighted != null) && !highlighted.IsDisposed)
				{
					// Flip selection
					SelectSector(highlighted, !highlighted.Selected, true);

					// Update display
					if (renderer.StartPlotter(false))
					{
						// Render highlighted item
						renderer.PlotSector(highlighted, General.Colors.Highlight);
						renderer.Finish();
						renderer.Present();
					}

					// Update overlay
					TextLabel[] labelarray = labels[highlighted];
					foreach(TextLabel l in labelarray) l.Color = General.Colors.Highlight;
					updateOverlaySurfaces();
					UpdateOverlay();
					renderer.Present();

					// Thing selection state may've changed
					if (General.Interface.AltState ^ BuilderModes.BuilderPlug.Me.SyncronizeThingEdit) General.Interface.RedrawDisplay();
				}
			}

			base.OnSelectEnd();
		}

		// Start editing
		protected override void OnEditBegin()
		{
			// Item highlighted?
			if (((highlighted != null) && !highlighted.IsDisposed) || csahighlight == ControlSectorArea.Highlight.Body)
			{
				// Edit pressed in this mode
				editpressed = true;

				if (csahighlight != ControlSectorArea.Highlight.Body)
				{
					// Highlighted item not selected?
					if (!highlighted.Selected && (BuilderPlug.Me.AutoClearSelection || (General.Map.Map.SelectedSectorsCount == 0)))
					{
						// Make this the only selection
						General.Map.Map.ClearSelectedSectors();
						General.Map.Map.ClearSelectedLinedefs();
						SelectSector(highlighted, true, false);
						updateOverlaySurfaces();
						General.Interface.RedrawDisplay();
					}

					// Update display
					if (renderer.StartPlotter(false))
					{
						// Redraw highlight to show selection
						renderer.PlotSector(highlighted);
						renderer.Finish();
						renderer.Present();
					}
				}
			}
			
			base.OnEditBegin();
		}

		// Done editing
		protected override void OnEditEnd()
		{
			// Edit pressed in this mode?
			if(editpressed && !dragging)
			{
				if (csahighlight == ControlSectorArea.Highlight.Body)
				{
					BuilderPlug.Me.ControlSectorArea.Edit();
				}
				else
				{
					// Anything selected?
					ICollection<Sector> selected = General.Map.Map.GetSelectedSectors(true);
					if (selected.Count > 0)
					{
						if (General.Interface.IsActiveWindow)
						{
							// Show sector edit dialog
							// General.Interface.ShowEditSectors(selected);
							DialogResult result = BuilderPlug.Me.ThreeDFloorEditor();

							if (result == DialogResult.OK)
							{
								BuilderPlug.ProcessThreeDFloors(BuilderPlug.TDFEW.ThreeDFloors);
								General.Map.Map.Update();

								threedfloors = BuilderPlug.GetThreeDFloors(General.Map.Map.Sectors.ToList());
							}

							// When a single sector was selected, deselect it now
							if (selected.Count == 1)
							{
								General.Map.Map.ClearSelectedSectors();
								General.Map.Map.ClearSelectedLinedefs();
							}

							General.Map.Renderer2D.UpdateExtraFloorFlag();

							SetupLabels();
							UpdateLabels();

							// Update entire display
							updateOverlaySurfaces();
							UpdateOverlay();
							General.Interface.RedrawDisplay();
						}
					}
				}
			}

			editpressed = false;
			base.OnEditEnd();
		}
		
		// Mouse moves
		public override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);

			if (selectpressed && !editpressed && !selecting)
			{
				// Check if moved enough pixels for multiselect
				Vector2D delta = mousedownpos - mousepos;
				if ((Math.Abs(delta.x) > 2) || (Math.Abs(delta.y) > 2))
				{
					// Start multiselecting
					StartMultiSelection();
				}
			}
			else if (paintselectpressed && !editpressed && !selecting) //mxd. Drag-select
			{
				// Find the nearest linedef within highlight range
				Linedef l = General.Map.Map.NearestLinedefRange(mousemappos, BuilderPlug.Me.HighlightRange / renderer.Scale);
				Sector s = null;

				if (l != null)
				{
					// Check on which side of the linedef the mouse is
					double side = l.SideOfLine(mousemappos);
					if (side > 0)
					{
						// Is there a sidedef here?
						if (l.Back != null) s = l.Back.Sector;
					}
					else
					{
						// Is there a sidedef here?
						if (l.Front != null) s = l.Front.Sector;
					}

					if (s != null)
					{
						if (s != highlighted)
						{
							//toggle selected state
							highlighted = s;
							if (General.Interface.ShiftState ^ BuilderPlug.Me.AdditivePaintSelect)
								SelectSector(highlighted, true, true);
							else if (General.Interface.CtrlState)
								SelectSector(highlighted, false, true);
							else
								SelectSector(highlighted, !highlighted.Selected, true);

							// Update entire display
							updateOverlaySurfaces();
							UpdateOverlay();
							General.Interface.RedrawDisplay();
						}
					}
					else if (highlighted != null)
					{
						Highlight(null);

						// Update entire display
						updateOverlaySurfaces();
						UpdateOverlay();
						General.Interface.RedrawDisplay();
					}

					UpdateSelectionInfo(); //mxd
				}
			}
			else if (e.Button == MouseButtons.None)
			{
				csahighlight = BuilderPlug.Me.ControlSectorArea.CheckHighlight(mousemappos, renderer.Scale);

				if (csahighlight != ControlSectorArea.Highlight.None)
				{
					switch (csahighlight)
					{
						case ControlSectorArea.Highlight.OuterTop:
						case ControlSectorArea.Highlight.OuterBottom:
							General.Interface.SetCursor(Cursors.SizeNS);
							break;
						case ControlSectorArea.Highlight.OuterLeft:
						case ControlSectorArea.Highlight.OuterRight:
							General.Interface.SetCursor(Cursors.SizeWE);
							break;
						case ControlSectorArea.Highlight.OuterTopLeft:
						case ControlSectorArea.Highlight.OuterBottomRight:
							General.Interface.SetCursor(Cursors.SizeNWSE);
							break;
						case ControlSectorArea.Highlight.OuterTopRight:
						case ControlSectorArea.Highlight.OuterBottomLeft:
							General.Interface.SetCursor(Cursors.SizeNESW);
							break;
						case ControlSectorArea.Highlight.Body:
							General.Interface.SetCursor(Cursors.Hand);
							break;
					}

					Highlight(null);
					return;
				}

				General.Interface.SetCursor(Cursors.Default);

				// Find the nearest linedef within highlight range
				Linedef l = General.Map.Map.NearestLinedef(mousemappos);
				if (l != null)
				{
					// Check on which side of the linedef the mouse is
					double side = l.SideOfLine(mousemappos);
					if (side > 0)
					{
						// Is there a sidedef here?
						if (l.Back != null)
						{
							// Highlight if not the same
							if (l.Back.Sector != highlighted) Highlight(l.Back.Sector);
						}
						else
						{
							// Highlight nothing
							Highlight(null);
						}
					}
					else
					{
						// Is there a sidedef here?
						if (l.Front != null)
						{
							// Highlight if not the same
							if (l.Front.Sector != highlighted) Highlight(l.Front.Sector);
						}
						else
						{
							// Highlight nothing
							Highlight(null);
						}
					}
				}
				else
				{
					// Highlight nothing
					Highlight(null);
				}
			}
			else if (dragging && csahighlight != ControlSectorArea.Highlight.None)
			{
				BuilderPlug.Me.ControlSectorArea.SnapToGrid(csahighlight, mousemappos, renderer.DisplayToMap(mouselastpos));
				Highlight(null);
			}
		}

		// Mouse leaves
		public override void OnMouseLeave(EventArgs e)
		{
			base.OnMouseLeave(e);

			// Highlight nothing
			Highlight(null);
		}

		// Mouse wants to drag
		protected override void OnDragStart(MouseEventArgs e)
		{
			base.OnDragStart(e);

			if(e.Button == MouseButtons.Right)
				dragging = true;

		}

		protected override void OnDragStop(MouseEventArgs e)
		{
			dragging = false;

			BuilderPlug.Me.ControlSectorArea.SaveConfig();
		}

		// This is called wheh selection ends
		protected override void OnEndMultiSelection()
		{
			bool selectionvolume = ((Math.Abs(base.selectionrect.Width) > 0.1f) && (Math.Abs(base.selectionrect.Height) > 0.1f));

			if(BuilderPlug.Me.AutoClearSelection && !selectionvolume)
			{
				General.Map.Map.ClearSelectedLinedefs();
				General.Map.Map.ClearSelectedSectors();
			}

			if(selectionvolume)
			{
				if(General.Interface.ShiftState ^ BuilderPlug.Me.AdditiveSelect)
				{
					// Go for all lines
					foreach(Linedef l in General.Map.Map.Linedefs)
					{
						l.Selected |= ((l.Start.Position.x >= selectionrect.Left) &&
									   (l.Start.Position.y >= selectionrect.Top) &&
									   (l.Start.Position.x <= selectionrect.Right) &&
									   (l.Start.Position.y <= selectionrect.Bottom) &&
									   (l.End.Position.x >= selectionrect.Left) &&
									   (l.End.Position.y >= selectionrect.Top) &&
									   (l.End.Position.x <= selectionrect.Right) &&
									   (l.End.Position.y <= selectionrect.Bottom));
					}
				}
				else
				{
					// Go for all lines
					foreach(Linedef l in General.Map.Map.Linedefs)
					{
						l.Selected = ((l.Start.Position.x >= selectionrect.Left) &&
									  (l.Start.Position.y >= selectionrect.Top) &&
									  (l.Start.Position.x <= selectionrect.Right) &&
									  (l.Start.Position.y <= selectionrect.Bottom) &&
									  (l.End.Position.x >= selectionrect.Left) &&
									  (l.End.Position.y >= selectionrect.Top) &&
									  (l.End.Position.x <= selectionrect.Right) &&
									  (l.End.Position.y <= selectionrect.Bottom));
					}
				}
				
				// Go for all sectors
				foreach(Sector s in General.Map.Map.Sectors)
				{
					// Go for all sidedefs
					bool allselected = true;
					foreach(Sidedef sd in s.Sidedefs)
					{
						if(!sd.Line.Selected)
						{
							allselected = false;
							break;
						}
					}
					
					// Sector completely selected?
					SelectSector(s, allselected, false);
				}
				
				// Make sure all linedefs reflect selected sectors
				foreach(Sidedef sd in General.Map.Map.Sidedefs)
					if(!sd.Sector.Selected && ((sd.Other == null) || !sd.Other.Sector.Selected))
						sd.Line.Selected = false;

				updateOverlaySurfaces();
			}
			
			base.OnEndMultiSelection();
			if(renderer.StartOverlay(true)) renderer.Finish();
			General.Interface.RedrawDisplay();
		}

		// This is called when the selection is updated
		protected override void OnUpdateMultiSelection()
		{
			base.OnUpdateMultiSelection();

			// Render selection
			if(renderer.StartOverlay(true))
			{
				RenderMultiSelection();
				renderer.Finish();
				renderer.Present();
			}
		}

		// When copying
		public override bool OnCopyBegin()
		{
			// No selection made? But we have a highlight!
			if((General.Map.Map.GetSelectedSectors(true).Count == 0) && (highlighted != null))
			{
				// Make the highlight the selection
				SelectSector(highlighted, true, true);
			}

			General.Map.Map.MarkAllSelectedGeometry(true, false, true, true, false);

			return General.Map.Map.GetMarkedSectors(true).Count > 0;
		}

		public override bool OnPasteBegin(PasteOptions options)
		{
			return true;
		}

		// This is called when something was pasted.
		public override void OnPasteEnd(PasteOptions options)
		{
			General.Map.Map.ClearAllSelected();
			General.Map.Map.SelectMarkedGeometry(true, true);
			General.Map.Renderer2D.UpdateExtraFloorFlag(); //mxd

			// Switch to EditSelectionMode
			EditSelectionMode editmode = new EditSelectionMode();
			editmode.Pasting = true;
			editmode.UpdateSlopes = true;
			editmode.PasteOptions = options;
			General.Editing.ChangeMode(editmode);
		}

		// When undo is used
		public override bool OnUndoBegin()
		{
			// Clear ordered selection
			General.Map.Map.ClearAllSelected();

			return base.OnUndoBegin();
		}

		// When undo is performed
		public override void OnUndoEnd()
		{
			// Get all 3D floors in case th undo did affect them
			threedfloors = BuilderPlug.GetThreeDFloors(General.Map.Map.Sectors.ToList());

			// Select changed map elements
			if (BuilderPlug.Me.SelectChangedAfterUndoRedo)
			{
				General.Map.Map.SelectMarkedGeometry(true, true);
				General.Map.Map.ConvertSelection(SelectionType.Sectors);
			}

			// Clear labels
			SetupLabels();
			UpdateLabels();
		}
		
		// When redo is used
		public override bool OnRedoBegin()
		{
			// Clear ordered selection
			General.Map.Map.ClearAllSelected();

			return base.OnRedoBegin();
		}

		// When redo is performed
		public override void OnRedoEnd()
		{
			// Get all 3D floors in case th redo did affect them
			threedfloors = BuilderPlug.GetThreeDFloors(General.Map.Map.Sectors.ToList());

			// Select changed map elements
			if (BuilderPlug.Me.SelectChangedAfterUndoRedo)
			{
				General.Map.Map.SelectMarkedGeometry(true, true);
				General.Map.Map.ConvertSelection(SelectionType.Sectors);
			}

			// Clear labels
			SetupLabels();
			UpdateLabels();
		}

		//mxd
		[BeginAction("classicpaintselect", Library = "BuilderModes" )]
		void OnPaintSelectBegin()
		{
			if (highlighted != null)
			{
				if (General.Interface.ShiftState ^ BuilderPlug.Me.AdditivePaintSelect)
					SelectSector(highlighted, true, true);
				else if (General.Interface.CtrlState)
					SelectSector(highlighted, false, true);
				else
					SelectSector(highlighted, !highlighted.Selected, true);

				// Update entire display
				updateOverlaySurfaces();
				UpdateOverlay();
				General.Interface.RedrawDisplay();
			}

			paintselectpressed = true;
		}

		[EndAction("classicpaintselect", Library = "BuilderModes")]
		void OnPaintSelectEnd()
		{
			paintselectpressed = false;
		}

		#endregion

		#region ================== Actions

		// This clears the selection
		[BeginAction("clearselection", BaseAction = true)]
		public void ClearSelection()
		{
			// Clear selection
			General.Map.Map.ClearAllSelected();

			// Clear labels
			foreach (TextLabel[] labelarray in labels.Values)
				foreach (TextLabel l in labelarray) l.Text = "";

			SetupLabels();
			UpdateLabels();

			updateOverlaySurfaces();

			// Redraw
			General.Interface.RedrawDisplay();
		}

		[BeginAction("cyclehighlighted3dfloorup")]
		public void CycleHighlighted3DFloorUp()
		{
			if (highlighted == null)
				return;

			List<ThreeDFloor> tdfs = new List<ThreeDFloor>();

			// Get all 3D floors that have the currently highlighted sector tagged. Also order them by their vertical position
			foreach (ThreeDFloor tdf in threedfloors.Where(o => o.TaggedSectors.Contains(highlighted)).OrderByDescending(o => o.TopHeight))
				tdfs.Add(tdf);

			if (tdfs.Count == 0) // Nothing to highlight
			{
				highlighted3dfloor = null;
			}
			else if (highlighted3dfloor == null) // No 3D floor currently highlighted? Just get the last one
			{
				highlighted3dfloor = tdfs.Last();
			}
			else // Find the currently highlighted 3D floor in the list and take the next one
			{
				int i;

				for (i = tdfs.Count-1; i >= 0; i--)
				{
					if (tdfs[i] == highlighted3dfloor)
					{
						if (i > 0)
						{
							highlighted3dfloor = tdfs[i - 1];
							break;
						}
					}
				}

				// Beginning of the list was reached, so don't highlight any 3D floor
				if (i < 0)
					highlighted3dfloor = null;
			}

			UpdateDocker(highlighted);

			updateOverlaySurfaces();

			General.Interface.RedrawDisplay();

		}

		[BeginAction("cyclehighlighted3dfloordown")]
		public void CycleHighlighted3DFloorDown()
		{
			if (highlighted == null)
				return;

			List<ThreeDFloor> tdfs = new List<ThreeDFloor>();

			// Get all 3D floors that have the currently highlighted sector tagged. Also order them by their vertical position
			foreach (ThreeDFloor tdf in threedfloors.Where(o => o.TaggedSectors.Contains(highlighted)).OrderByDescending(o => o.TopHeight))
				tdfs.Add(tdf);

			if (tdfs.Count == 0) // Nothing to highlight
			{
				highlighted3dfloor = null;
			}
			else if (highlighted3dfloor == null) // No 3D floor currently highlighted? Just get the first one
			{
				highlighted3dfloor = tdfs[0];
			}
			else // Find the currently highlighted 3D floor in the list and take the next one
			{
				int i;

				for (i = 0; i < tdfs.Count; i++)
				{
					if (tdfs[i] == highlighted3dfloor)
					{
						if (i < tdfs.Count-1)
						{
							highlighted3dfloor = tdfs[i + 1];
							break;
						}
					}
				}

				// End of the list was reached, so don't highlight any 3D floor
				if (i == tdfs.Count)
					highlighted3dfloor = null;
			}

			UpdateDocker(highlighted);

			updateOverlaySurfaces();

			General.Interface.RedrawDisplay();
		}

		[BeginAction("relocate3dfloorcontrolsectors")]
		public void RelocateControlSectors()
		{
			List<Vector2D> positions;
			Dictionary<Sector, List<Thing>> controlsectorthings = new Dictionary<Sector, List<Thing>>();

			if (threedfloors.Count == 0)
			{
				General.Interface.DisplayStatus(StatusType.Warning, "There are no control sectors to relocate");
				return;
			}

			try
			{
				 positions = BuilderPlug.Me.ControlSectorArea.GetRelocatePositions(threedfloors.Count);
			}
			catch (Exception e)
			{
				General.Interface.DisplayStatus(StatusType.Warning, e.Message + ". Please increase the size of the control sector area");
				return;
			}

			// Some sanity checks
			if (positions.Count != threedfloors.Count)
			{
				General.Interface.DisplayStatus(StatusType.Warning, "Mismatch between number of relocation points and control sectors. Aborted");
				return;
			}

			// Update all thing's sector, since we might have to move things that are in control sectors
			Parallel.ForEach(General.Map.Map.Things, t => {
				t.DetermineSector(blockmap);
			});

			// Control sectors are not allowed to be bigger than what the CSA expects, otherwise sectors might overlap
			// Abort relocation if one of the control sectors is too big (that should only happen if the user edited the
			// sector manually
			foreach (ThreeDFloor tdf in threedfloors)
			{
				tdf.Sector.UpdateBBox();

				if (tdf.Sector.BBox.Width > BuilderPlug.Me.ControlSectorArea.SectorSize || tdf.Sector.BBox.Height > BuilderPlug.Me.ControlSectorArea.SectorSize)
				{
					General.Interface.DisplayStatus(StatusType.Warning, string.Format("Control sector {0} exceeds horizontal or vertical dimension of {1}. Aborted", tdf.Sector.Index, BuilderPlug.Me.ControlSectorArea.SectorSize));
					return;
				}

				if (!controlsectorthings.ContainsKey(tdf.Sector))
					controlsectorthings.Add(tdf.Sector, new List<Thing>());

				// Find all things in the 3D floor's control sector so they can be also moved
				List<BlockEntry> belist = blockmap.GetSquareRange(tdf.Sector.BBox);
				foreach (BlockEntry be in belist)
					foreach (Thing t in be.Things)
						if(t.Sector == tdf.Sector)
							controlsectorthings[tdf.Sector].Add(t);
			}

			General.Map.UndoRedo.CreateUndo("Relocate 3D floor control sectors");

			// Counter for the new positions
			int i = 0;

			// Move the control sectors
			foreach (ThreeDFloor tdf in threedfloors)
			{
				Vector2D offset = new Vector2D(tdf.Sector.BBox.Left - positions[i].x, tdf.Sector.BBox.Bottom - positions[i].y);
				HashSet<Vertex> vertices = new HashSet<Vertex>();

				// Get all vertices
				foreach (Sidedef sd in tdf.Sector.Sidedefs)
				{
					vertices.Add(sd.Line.Start);
					vertices.Add(sd.Line.End);
				}

				// Move vertices
				foreach (Vertex v in vertices)
					v.Move(v.Position - offset);

				// Move the things in the control sector
				foreach (Thing t in controlsectorthings[tdf.Sector])
					t.Move(t.Position - new Vector3D(offset));

				// The bounding box of the sector has changed
				tdf.Sector.UpdateBBox();

				i++;
			}

			// Rebuild the blockmap
			CreateBlockmap();

			General.Map.Map.Update();
			General.Interface.RedrawDisplay();
		}

		[BeginAction("select3dfloorcontrolsector")]
		public void Select3DFloorControlSector()
		{
			//if there is no 3d floor highlighted, then try to select the first one from the top-down, if possible
			if (highlighted3dfloor == null)
			{
				CycleHighlighted3DFloorDown();
			}

			if (highlighted3dfloor == null)
			{
				General.Interface.DisplayStatus(StatusType.Warning, "You have to highlight a 3D floor to select its control sector");
				return;
			}

			SelectSector(highlighted3dfloor.Sector, true, true);

			updateOverlaySurfaces();

			General.Interface.RedrawDisplay();

			General.Interface.DisplayStatus(StatusType.Info, String.Format("3D floor control sector selected. {0} sector(s) selected.", General.Map.Map.GetSelectedSectors(true).Count));
		}

		[BeginAction("duplicate3dfloorgeometry")]
		public void Duplicate3DFloorGeometry()
		{
			List<Sector> selectedsectors;
			List<ThreeDFloor> duplicatethreedfloors;
			List<DrawnVertex> drawnvertices;
			Dictionary<int, int> tagreplacements = new Dictionary<int, int>();
			List<int> tagblacklist = new List<int>();

			// No selection made? But we have a highlight!
			if ((General.Map.Map.GetSelectedSectors(true).Count == 0) && (highlighted != null))
			{
				// Make the highlight the selection
				SelectSector(highlighted, true, true);
			}

			selectedsectors = General.Map.Map.GetSelectedSectors(true).ToList();

			// Get the 3D floors we need to duplicate
			duplicatethreedfloors = BuilderPlug.GetThreeDFloors(selectedsectors);

			// Create a list of all tags used by the control sectors. This is necessary so that
			// tags that will be assigned to not yet existing geometry will not be used
			foreach (ThreeDFloor tdf in threedfloors)
				foreach (int tag in tdf.Tags)
					if (!tagblacklist.Contains(tag))
						tagblacklist.Add(tag);

			if (duplicatethreedfloors.Count == 0)
			{
				General.Interface.DisplayStatus(StatusType.Warning, "Selected geometry doesn't contain 3D floors");
				return;
			}

			try
			{
				drawnvertices = BuilderPlug.Me.ControlSectorArea.GetNewControlSectorVertices(duplicatethreedfloors.Count);
			}
			catch (NoSpaceInCSAException e)
			{
				General.Interface.DisplayStatus(StatusType.Warning, string.Format("Could not create 3D floor control sector geometry: {0}", e.Message));
				return;
			}

			General.Map.UndoRedo.CreateUndo(duplicateundodescription);
			
			// Create a new control sector for each 3D floor that needs to be duplicated. Force it to generate
			// a new tag, and store the old (current) and new tag
			foreach (ThreeDFloor tdf in duplicatethreedfloors)
			{
				int newtag;
				int oldtag = tdf.UDMFTag;

				if(!tdf.CreateGeometry(tagblacklist, drawnvertices, tdf.LinedefProperties, tdf.SectorProperties, true, out newtag))
				{
					// No need to show a warning here, that was already done by CreateGeometry
					General.Map.UndoRedo.WithdrawUndo();
					return;
				}

				tagreplacements[oldtag] = newtag;
			}

			// Replace the old tags of the selected sectors with the new tags
			foreach (Sector s in selectedsectors)
			{
				foreach (int oldtag in tagreplacements.Keys)
				{
					if (s.Tags.Contains(oldtag))
					{
						s.Tags.Remove(oldtag);
						s.Tags.Add(tagreplacements[oldtag]);
					}
				}
			}

			// Store the selected sectors (with the new tags) in the clipboard
			if(!CopyPasteManager.DoCopySelection("3D floor test copy!"))
			{
				General.Interface.DisplayStatus(StatusType.Warning, "Something failed trying to copy the selection");
				General.Map.UndoRedo.WithdrawUndo();
				return;
			}

			// Now set the tags of the selected sectors back to the old tags
			foreach(Sector s in selectedsectors)
			{
				foreach(int oldtag in tagreplacements.Keys)
				{
					if(s.Tags.Contains(tagreplacements[oldtag]))
					{
						s.Tags.Remove(tagreplacements[oldtag]);
						s.Tags.Add(oldtag);
					}
				}
			}

			// For this operation we have to make sure the tags and actions are not changed, no matter
			// what the user preference is, otherwise it will not work
			PasteOptions po = new PasteOptions() { ChangeTags = 0, RemoveActions = false };

			CopyPasteManager.DoPasteSelection(po);
		}

		#endregion
	}
}
