
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
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Actions;
using CodeImp.DoomBuilder.BuilderModes.Interface;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Editing;
using CodeImp.DoomBuilder.GZBuilder.Data;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Types;
using CodeImp.DoomBuilder.Windows;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	[EditMode(DisplayName = "Things Mode",
			  SwitchAction = "thingsmode",		// Action name used to switch to this mode
			  ButtonImage = "ThingsMode.png",	// Image resource name for the button
			  ButtonOrder = int.MinValue + 300,	// Position of the button (lower is more to the left)
			  ButtonGroup = "000_editing",
			  UseByDefault = true,
			  SafeStartMode = true)]
    
	public class ThingsMode : BaseClassicMode
	{
		#region ================== Constants

		private const int MAX_THING_LABELS = 256; //mxd

		#endregion

		#region ================== Variables

		// Highlighted item
		private Thing highlighted;
		private readonly Association highlightasso;

		// Interface
		new private bool editpressed;
		private bool thinginserted;
		private bool awaitingMouseClick; //mxd

		//mxd. Helper shapes
		private List<Line3D> persistenteventlines;
		private List<Line3D> dynamiclightshapes;
		private List<Line3D> ambientsoundshapes;

		//mxd. Text labels
		private Dictionary<Thing, TextLabel> labels;
		private Dictionary<Sector, TextLabel[]> sectorlabels;
		private Dictionary<Sector, string[]> sectortexts;

		// Stores sizes of the text for text labels so that they only have to be computed once
		private Dictionary<string, float> textlabelsizecache;

		// Things that will be edited
		private ICollection<Thing> editthings;

		#endregion

		#region ================== Properties

		public override object HighlightedObject { get { return highlighted; } }

		#endregion

		#region ================== Constructor / Disposer

		public ThingsMode()
		{
			//mxd. Associations now requre initializing...
			highlightasso = new Association(renderer);

			textlabelsizecache = new Dictionary<string, float>();
		}

		//mxd
		public override void Dispose()
		{
			// Not already disposed?
			if(!isdisposed)
			{
				// Dispose old labels
				if(labels != null) foreach(TextLabel l in labels.Values) l.Dispose();
				if(sectorlabels != null)
				{
					foreach(TextLabel[] larr in sectorlabels.Values)
						foreach(TextLabel l in larr) l.Dispose();
				}

				// Dispose base
				base.Dispose();
			}
		}

		#endregion

		#region ================== Methods

		//mxd. This makes a CRC for given selection
		private static int CreateSelectionCRC(ICollection<Thing> selection) 
		{
			CRC crc = new CRC();
			crc.Add(selection.Count);
			foreach(Thing t in selection) crc.Add(t.Index);
			return (int)(crc.Value & 0xFFFFFFFF);
		}

		public override void OnHelp()
		{
			General.ShowHelp("e_things.html");
		}

		// Cancel mode
		public override void OnCancel()
		{
			base.OnCancel();

			// Return to this mode
			General.Editing.ChangeMode(new ThingsMode());
		}

		// Mode engages
		public override void OnEngage()
		{
			base.OnEngage();
			renderer.SetPresentation(Presentation.Things);

			// Add toolbar buttons
			General.Interface.BeginToolbarUpdate(); //mxd
			General.Interface.AddButton(BuilderPlug.Me.MenusForm.CopyProperties);
			General.Interface.AddButton(BuilderPlug.Me.MenusForm.PasteProperties);
			General.Interface.AddButton(BuilderPlug.Me.MenusForm.PastePropertiesOptions); //mxd
			General.Interface.AddButton(BuilderPlug.Me.MenusForm.SeparatorCopyPaste); //mxd
			General.Interface.AddButton(BuilderPlug.Me.MenusForm.ViewSelectionNumbers); //mxd
			if(General.Map.FormatInterface.HasThingAction)
			{
				BuilderPlug.Me.MenusForm.ViewSelectionEffects.Text = "View Sector Tags"; //mxd
				General.Interface.AddButton(BuilderPlug.Me.MenusForm.ViewSelectionEffects); //mxd
			}
			General.Interface.AddButton(BuilderPlug.Me.MenusForm.SeparatorSectors1); //mxd
			General.Interface.AddButton(BuilderPlug.Me.MenusForm.AlignThingsToWall); //mxd

			//mxd. Add radii buttons/items...
			General.Interface.AddButton(BuilderPlug.Me.MenusForm.ButtonLightRadii, ToolbarSection.Helpers);
			General.Interface.AddButton(BuilderPlug.Me.MenusForm.ButtonSoundRadii, ToolbarSection.Helpers);
			General.Interface.AddMenu(BuilderPlug.Me.MenusForm.ItemLightRadii, MenuSection.ViewHelpers);
			General.Interface.AddMenu(BuilderPlug.Me.MenusForm.ItemSoundRadii, MenuSection.ViewHelpers);
			General.Interface.EndToolbarUpdate(); //mxd
			
			// Convert geometry selection to linedefs selection
			General.Map.Map.ConvertSelection(SelectionType.Linedefs);
			General.Map.Map.SelectionType = SelectionType.Things;
			UpdateSelectionInfo(); //mxd
			UpdateHelperObjects(); //mxd
			SetupSectorLabels(); //mxd
		}

		// Mode disengages
		public override void OnDisengage()
		{
			base.OnDisengage();

			// Remove toolbar buttons
			General.Interface.BeginToolbarUpdate(); //mxd
			General.Interface.RemoveButton(BuilderPlug.Me.MenusForm.CopyProperties);
			General.Interface.RemoveButton(BuilderPlug.Me.MenusForm.PasteProperties);
			General.Interface.RemoveButton(BuilderPlug.Me.MenusForm.PastePropertiesOptions); //mxd
			General.Interface.RemoveButton(BuilderPlug.Me.MenusForm.SeparatorCopyPaste); //mxd
			General.Interface.RemoveButton(BuilderPlug.Me.MenusForm.ViewSelectionNumbers); //mxd
			General.Interface.RemoveButton(BuilderPlug.Me.MenusForm.ViewSelectionEffects); //mxd
			General.Interface.RemoveButton(BuilderPlug.Me.MenusForm.SeparatorSectors1); //mxd
			General.Interface.RemoveButton(BuilderPlug.Me.MenusForm.AlignThingsToWall); //mxd

			//mxd. Remove radii buttons/items...
			General.Interface.RemoveButton(BuilderPlug.Me.MenusForm.ButtonLightRadii);
			General.Interface.RemoveButton(BuilderPlug.Me.MenusForm.ButtonSoundRadii);
			General.Interface.RemoveMenu(BuilderPlug.Me.MenusForm.ItemLightRadii);
			General.Interface.RemoveMenu(BuilderPlug.Me.MenusForm.ItemSoundRadii);
			General.Interface.EndToolbarUpdate(); //mxd

			//mxd. Do some highlight management...
			if(highlighted != null) highlighted.Highlighted = false;

			// Going to EditSelectionMode?
			EditSelectionMode mode = General.Editing.NewMode as EditSelectionMode;
			if(mode != null)
			{
				// Not pasting anything?
				if(!mode.Pasting)
				{
					// No selection made? But we have a highlight!
					if((General.Map.Map.GetSelectedThings(true).Count == 0) && (highlighted != null))
					{
						// Make the highlight the selection
						highlighted.Selected = true;
					}
				}
			} 

			// Hide highlight info and tooltip
			General.Interface.HideInfo();
			General.Interface.Display.HideToolTip(); //mxd
		}

		// This redraws the display
		public override void OnRedrawDisplay()
		{
			renderer.RedrawSurface();
			List<Line3D> eventlines = new List<Line3D>(); //mxd

			// Render lines and vertices
			if(renderer.StartPlotter(true)) 
			{
				renderer.PlotLinedefSet(General.Map.Map.Linedefs);
				renderer.PlotVerticesSet(General.Map.Map.Vertices);

				if (highlighted != null && !highlighted.IsDisposed) highlightasso.Plot();
				
				renderer.Finish();
			}

			// Render things
			if(renderer.StartThings(true))
			{
				float alpha = (General.Settings.FixedThingsScale ? Presentation.THINGS_ALPHA : General.Settings.ActiveThingsAlpha); //mxd
				renderer.RenderThingSet(General.Map.ThingsFilter.HiddenThings, General.Settings.HiddenThingsAlpha);
				renderer.RenderThingSet(General.Map.ThingsFilter.VisibleThings, alpha);
				
				if(highlighted != null && !highlighted.IsDisposed)
				{
					renderer.RenderThing(highlighted, General.Colors.Highlight, alpha);
					highlightasso.Render();
				}

				//mxd. Event lines
				if(General.Settings.GZShowEventLines) eventlines.AddRange(persistenteventlines);

				//mxd. Dynamic light radii
				if(BuilderPlug.Me.ShowLightRadii)
				{
					eventlines.AddRange(dynamiclightshapes);
					if(highlighted != null && !highlighted.IsDisposed)
						eventlines.AddRange(LinksCollector.GetDynamicLightShapes(new List<Thing> { highlighted }, true));
				}

				//mxd. Ambient sound radii
				if(BuilderPlug.Me.ShowSoundRadii)
				{
					eventlines.AddRange(ambientsoundshapes);
					if(highlighted != null && !highlighted.IsDisposed)
						eventlines.AddRange(LinksCollector.GetAmbientSoundShapes(new List<Thing> { highlighted }, true));
				}

				//mxd
				if(eventlines.Count > 0) renderer.RenderArrows(eventlines);

				renderer.Finish();
			}

			// Selecting?
			if(renderer.StartOverlay(true))
			{
				// Render selection
				if(selecting) RenderMultiSelection();

				//mxd. Render sector tag labels
				if(BuilderPlug.Me.ViewSelectionEffects && General.Map.FormatInterface.HasThingAction)
				{
					//mxd. sectorlabels will be null after switching map configuration from one 
					// without ThingAction to one with it while in Things mode
					if(sectorlabels == null) SetupSectorLabels(); 
					
					List<ITextLabel> torender = new List<ITextLabel>(sectorlabels.Count);
					foreach(KeyValuePair<Sector, string[]> group in sectortexts)
					{
						// Pick which text variant to use
						TextLabel[] labelarray = sectorlabels[group.Key];
						for(int i = 0; i < group.Key.Labels.Count; i++)
						{
							TextLabel l = labelarray[i];

							// Render only when enough space for the label to see
							if (!textlabelsizecache.ContainsKey(group.Value[0]))
								textlabelsizecache[group.Value[0]] = General.Interface.MeasureString(group.Value[0], l.Font).Width;

							float requiredsize = textlabelsizecache[group.Value[0]] / 2 / renderer.Scale;

							if (requiredsize > group.Key.Labels[i].radius)
							{
								if (!textlabelsizecache.ContainsKey(group.Value[1]))
									textlabelsizecache[group.Value[1]] = General.Interface.MeasureString(group.Value[1], l.Font).Width;

								requiredsize = textlabelsizecache[group.Value[1]] / 2 / renderer.Scale;

								string newtext;

								if (requiredsize > group.Key.Labels[i].radius)
									newtext = (requiredsize > group.Key.Labels[i].radius * 4 ? string.Empty : "+");
								else
									newtext = group.Value[1];

								if (l.Text != newtext)
									l.Text = newtext;
							}
							else
							{
								if (group.Value[0] != l.Text)
									l.Text = group.Value[0];
							}

							if (!string.IsNullOrEmpty(l.Text)) torender.Add(l);
						}
					}

					// Render labels
					renderer.RenderText(torender);
				}

				//mxd. Render selection labels
				if(BuilderPlug.Me.ViewSelectionNumbers)
				{
					List<ITextLabel> torender = new List<ITextLabel>(labels.Count);
					foreach(KeyValuePair<Thing, TextLabel> group in labels)
					{
						// Render only when enough space for the label to see
						float requiredsize = (group.Value.TextSize.Width) / renderer.Scale;
						if(group.Key.Size * 2 > requiredsize) torender.Add(group.Value);
					}

					renderer.RenderText(torender);
				}

				//mxd. Render comments
				if(General.Map.UDMF && General.Settings.RenderComments) foreach(Thing t in General.Map.Map.Things) RenderComment(t);

				renderer.Finish();
			}

			renderer.Present();
		}
		
		// This highlights a new item
		private void Highlight(Thing t)
		{
			// Set highlight association
			if(t != null)
			{
				//mxd. Update label color?
				if(labels.ContainsKey(t)) labels[t].Color = General.Colors.Selection;

				//check if this thing directly links to another type of thing
				ThingTypeInfo ti = General.Map.Data.GetThingInfoEx(t.Type);
				int linktype = 0;
				if (ti != null)
					linktype = ti.ThingLink;

				// New association highlights something?
				highlightasso.Set(t);
			}
			else
			{
				highlightasso.Clear();
			}

			if(highlighted != null) //mxd
			{
				//mxd. Update label color?
				if(labels.ContainsKey(highlighted)) labels[highlighted].Color = General.Colors.Highlight;
				
				highlighted.Highlighted = false;
			}

			// Set new highlight and redraw display
			highlighted = t;
			General.Interface.RedrawDisplay();
			
			// Show highlight info
			if((highlighted != null) && !highlighted.IsDisposed)
			{
				General.Interface.ShowThingInfo(highlighted);
			}
			else
			{
				General.Interface.Display.HideToolTip(); //mxd
				General.Interface.HideInfo();
			}
		}

		// Selection
		protected override void OnSelectBegin()
		{
			//mxd. Yep, it's kinda hackish...
			if(awaitingMouseClick) 
			{
				awaitingMouseClick = false;
				ThingPointAtCursor();
				return;
			}
			
			// Item highlighted?
			if((highlighted != null) && !highlighted.IsDisposed)
			{
				// Update display
				if(renderer.StartThings(false))
				{
					// Redraw highlight to show selection
					renderer.RenderThing(highlighted, renderer.DetermineThingColor(highlighted), General.Settings.FixedThingsScale ? Presentation.THINGS_ALPHA : General.Settings.ActiveThingsAlpha);
					renderer.Finish();
					renderer.Present();
				}
			}

			base.OnSelectBegin();
		}

		// End selection
		protected override void OnSelectEnd()
		{
			// Not ending from a multi-selection?
			if(!selecting)
			{
				// Item highlighted?
				if((highlighted != null) && !highlighted.IsDisposed)
				{
					//mxd. Flip selection
					highlighted.Selected = !highlighted.Selected;
					UpdateSelectionInfo(); //mxd

					//mxd. Full redraw when labels were changed
					if(BuilderPlug.Me.ViewSelectionNumbers)
					{
						General.Interface.RedrawDisplay();
					}
					// Update display
					else if(renderer.StartThings(false))
					{
						// Render highlighted item
						renderer.RenderThing(highlighted, General.Colors.Highlight, General.Settings.FixedThingsScale ? Presentation.THINGS_ALPHA : General.Settings.ActiveThingsAlpha);
						renderer.Finish();
						renderer.Present();
					}
				}
				//mxd
				else if(BuilderPlug.Me.AutoClearSelection && General.Map.Map.SelectedThingsCount > 0) 
				{
					General.Map.Map.ClearSelectedThings();
					UpdateSelectionInfo();
					General.Interface.RedrawDisplay();
				}
			}

			base.OnSelectEnd();
		}

		// Start editing
		protected override void OnEditBegin()
		{
			thinginserted = false;
			
			// Item highlighted?
			if((highlighted != null) && !highlighted.IsDisposed)
			{
				// Edit pressed in this mode
				editpressed = true;
				
				// Highlighted item not selected?
				if(!highlighted.Selected)
				{
					// Make this the only selection
					General.Map.Map.ClearSelectedThings();

					editthings = new List<Thing> { highlighted };

					UpdateSelectionInfo(); //mxd
					General.Interface.RedrawDisplay();
				}
				else
				{
					editthings = General.Map.Map.GetSelectedThings(true);
				}

				// Update display
				if(renderer.StartThings(false))
				{
					// Redraw highlight to show selection
					renderer.RenderThing(highlighted, General.Colors.Highlight, General.Settings.FixedThingsScale ? Presentation.THINGS_ALPHA : General.Settings.ActiveThingsAlpha);
					renderer.Finish();
					renderer.Present();
				}
			}
			else if(mouseinside && !selecting && BuilderPlug.Me.AutoDrawOnEdit) //mxd. We don't want to insert a thing when multiselecting
			{
				// Edit pressed in this mode
				editpressed = true;
				thinginserted = true;

				// Insert a new item and select it for dragging
				General.Map.UndoRedo.CreateUndo("Insert thing");
				Thing t = InsertThing(mousemappos);

				if(t == null) 
				{
					General.Map.UndoRedo.WithdrawUndo();
				} 
				else 
				{
					General.Map.Map.ClearSelectedThings();
					General.Map.Map.ClearMarkedThings(false);
					editthings = new List<Thing> { t };
					Highlight(t);
					General.Interface.RedrawDisplay();
				}
			}

			base.OnEditBegin();
		}

		// Done editing
		protected override void OnEditEnd()
		{
			// Edit pressed in this mode?
			if(editpressed)
			{
				if(editthings?.Count > 0)
				{
					if(General.Interface.IsActiveWindow)
					{
						// Edit only when preferred
						if(!thinginserted || BuilderPlug.Me.EditNewThing)
						{
							//mxd. Show realtime thing edit dialog
							General.Interface.OnEditFormValuesChanged += thingEditForm_OnValuesChanged;
							DialogResult result = General.Interface.ShowEditThings(editthings);
							General.Interface.OnEditFormValuesChanged -= thingEditForm_OnValuesChanged;

							//mxd. Update helper lines
							UpdateHelperObjects();

							//mxd. Update selection info
							UpdateSelectionInfo();

							// Update display
							General.Interface.RedrawDisplay();
						}
					}
				}
			}

			editpressed = false;
			base.OnEditEnd();
		}

		//mxd
		public override void OnUndoEnd()
		{
			base.OnUndoEnd();

			// Select changed map elements
			if (BuilderPlug.Me.SelectChangedafterUndoRedo)
			{
				General.Map.Map.SelectMarkedGeometry(true, true);
			}

			// If something is highlighted make sure to update the association so that it contains valid data
			if (highlighted != null && !highlighted.IsDisposed)
				highlightasso.Set(highlighted);

			UpdateSelectionInfo(); // Update selection info and labels
			UpdateHelperObjects(); // Update helper lines
			SetupSectorLabels(); // And sector labels
		}

		//mxd
		public override void OnRedoEnd()
		{
			base.OnRedoEnd();

			// Select changed map elements
			if (BuilderPlug.Me.SelectChangedafterUndoRedo)
			{
				General.Map.Map.SelectMarkedGeometry(true, true);
			}

			// If something is highlighted make sure to update the association so that it contains valid data
			if (highlighted != null && !highlighted.IsDisposed)
				highlightasso.Set(highlighted);

			UpdateSelectionInfo(); // Update selection info and labels
			UpdateHelperObjects(); // Update helper lines
			SetupSectorLabels(); // And sector labels
		}

		public override void OnScriptRunEnd()
		{
			base.OnScriptRunEnd();

			UpdateSelectionInfo();

			General.Interface.RedrawDisplay();
		}

		//mxd. Otherwise event lines won't be drawn after panning finishes.
		protected override void EndViewPan()
		{
			base.EndViewPan();
			if(General.Settings.GZShowEventLines) General.Interface.RedrawDisplay(); 
		}

		//mxd
		private void thingEditForm_OnValuesChanged(object sender, EventArgs e) 
		{
			// Update things filter
			General.Map.ThingsFilter.Update();

			// Update entire display
			General.Interface.RedrawDisplay();
		}

		// Mouse moves
		public override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			if(panning) return; //mxd. Skip all this jazz while panning

			//mxd
			if(selectpressed && !editpressed && !selecting) 
			{
				// Check if moved enough pixels for multiselect
				Vector2D delta = mousedownpos - mousepos;
				if((Math.Abs(delta.x) > BuilderPlug.Me.MouseSelectionThreshold) ||
				   (Math.Abs(delta.y) > BuilderPlug.Me.MouseSelectionThreshold)) 
				{
					// Start multiselecting
					StartMultiSelection();
				}
			}
			else if(paintselectpressed && !editpressed && !selecting) //mxd. Drag-select
			{
				// Find the nearest thing within highlight range
				Thing t = MapSet.NearestThingSquareRange(General.Map.ThingsFilter.VisibleThings, mousemappos, BuilderPlug.Me.HighlightThingsRange / renderer.Scale);

				if(t != null) 
				{
					if(t != highlighted) 
					{
						//toggle selected state
						if(General.Interface.ShiftState ^ BuilderPlug.Me.AdditivePaintSelect)
							t.Selected = true;
						else if(General.Interface.CtrlState)
							t.Selected = false;
						else
							t.Selected = !t.Selected;
						highlighted = t;

						UpdateSelectionInfo(); //mxd

						// Update entire display
						General.Interface.RedrawDisplay();
					}
				} 
				else if(highlighted != null) 
				{
					Highlight(null);
					
					// Update entire display
					General.Interface.RedrawDisplay();
				}
			}
			else if(e.Button == MouseButtons.None) // Not holding any buttons?
			{
				// Find the nearest thing within highlight range
				Thing t = MapSet.NearestThingSquareRange(General.Map.ThingsFilter.VisibleThings, mousemappos, BuilderPlug.Me.HighlightThingsRange / renderer.Scale);

				//mxd. Show tooltip?
				if(General.Map.UDMF && General.Settings.RenderComments && mouselastpos != mousepos && highlighted != null && !highlighted.IsDisposed && highlighted.Fields.ContainsKey("comment"))
				{
					string comment = highlighted.Fields.GetValue("comment", string.Empty);
					if(comment.Length > 2)
					{
						string type = comment.Substring(0, 3);
						int index = Array.IndexOf(CommentType.Types, type);
						if(index > 0) comment = comment.TrimStart(type.ToCharArray());
					}
					General.Interface.Display.ShowToolTip("Comment:", comment, (int)(mousepos.x + 32 * MainForm.DPIScaler.Width), (int)(mousepos.y + 8 * MainForm.DPIScaler.Height));
				}

				// Highlight if not the same
				if(t != highlighted) Highlight(t);
			} 
		}

		// Mouse leaves
		public override void OnMouseLeave(EventArgs e)
		{
			base.OnMouseLeave(e);

			// Highlight nothing
			Highlight(null);
		}

		//mxd
		protected override void OnPaintSelectBegin() 
		{
			// Highlight nothing
			Highlight(null);

			base.OnPaintSelectBegin();
		}

		// Mouse wants to drag
		protected override void OnDragStart(MouseEventArgs e)
		{
			base.OnDragStart(e);

			// Edit button used?
			if(General.Actions.CheckActionActive(null, "classicedit"))
			{
				// Anything highlighted?
				if((highlighted != null) && !highlighted.IsDisposed)
				{
					ICollection<Thing> dragthings;

					// Highlighted item not selected?
					if(!highlighted.Selected)
					{
						// Select only this thing for dragging
						General.Map.Map.ClearSelectedThings();
						dragthings = new List<Thing> { highlighted };
					}
					else
					{
						// Add all selected things to the things we want to drag
						dragthings = General.Map.Map.GetSelectedThings(true);
					}

					// Start dragging the selection
					if(!BuilderPlug.Me.DontMoveGeometryOutsideMapBoundary || CanDrag(dragthings)) //mxd
					{ 
						// Shift pressed? Clone things!
						bool thingscloned = false;
						if(General.Interface.ShiftState) 
						{
							ICollection<Thing> clonedthings = new List<Thing>(dragthings.Count);
							if(dragthings.Count > 0)
							{
								// Make undo
								General.Map.UndoRedo.CreateUndo((dragthings.Count == 1 ? "Clone-drag thing" : "Clone-drag " + dragthings.Count + " things"));

								// Clone things
								foreach(Thing t in dragthings)
								{
									Thing clone = InsertThing(t.Position);
									t.CopyPropertiesTo(clone);

									// If the cloned item is an interpolation point or patrol point, then insert the point in the path
									ThingTypeInfo info = General.Map.Data.GetThingInfo(t.Type);
									int nextpointtagargnum = -1;

									// Thing type can be changed in MAPINFO DoomEdNums block...
									switch(info.ClassName.ToLowerInvariant())
									{
										case "interpolationpoint":
											nextpointtagargnum = 3;
											break;

										case "patrolpoint":
											nextpointtagargnum = 0;
											break;
									}

									// Apply changes?
									if(nextpointtagargnum != -1)
									{
										if(t.Tag == 0) t.Tag = General.Map.Map.GetNewTag();
										t.Args[nextpointtagargnum] = clone.Tag = General.Map.Map.GetNewTag();
									}

									t.Selected = false;

									clonedthings.Add(clone);
								}

								// We'll want to skip creating additional Undo in DragThingsMode
								thingscloned = true;

								// All the cloned things are now the things we want to drag
								dragthings = clonedthings;

								// Update things filter
								General.Map.ThingsFilter.Update();
								General.Interface.RefreshInfo();

								//mxd. Update helper lines
								UpdateHelperObjects();

								// Redraw
								General.Interface.RedrawDisplay();
							}
						}

						General.Editing.ChangeMode(new DragThingsMode(new ThingsMode(), mousedownmappos, dragthings, !thingscloned));
					}
				}
			}
		}

		//mxd. Check if any selected thing is outside of map boundary
		private static bool CanDrag(ICollection<Thing> dragthings) 
		{
			int unaffectedCount = 0;

			foreach(Thing t in dragthings) 
			{
				// Make sure the vertex is inside the map boundary
				if(t.Position.x < General.Map.Config.LeftBoundary || t.Position.x > General.Map.Config.RightBoundary
					|| t.Position.y > General.Map.Config.TopBoundary || t.Position.y < General.Map.Config.BottomBoundary) 
				{
					unaffectedCount++;
				}
			}

			if (unaffectedCount == dragthings.Count)
			{
				General.Interface.DisplayStatus(StatusType.Warning, "Unable to drag selection: " + (dragthings.Count == 1 ? "selected thing is" : "all of selected things are") + " outside of map boundary!");
				General.Interface.RedrawDisplay();
				return false;
			}

			if (unaffectedCount > 0)
			{
				General.Interface.DisplayStatus(StatusType.Warning, unaffectedCount + " of selected vertices " + (unaffectedCount == 1 ? "is" : "are") + " outside of map boundary!");
				return false;
			}

			return true;
		}

		// This is called wheh selection ends
		protected override void OnEndMultiSelection()
		{
			bool selectionvolume = ((Math.Abs(selectionrect.Width) > 0.1f) && (Math.Abs(selectionrect.Height) > 0.1f));

			if(selectionvolume)
			{
				//mxd
				switch(marqueSelectionMode) 
				{
					case MarqueSelectionMode.SELECT:
						// Get ordered selection
						List<Thing> selectresult = GetOrderedSelection(base.selectstart, selectionrect);

						// First deselect everything...
						foreach(Thing t in General.Map.Map.Things) t.Selected = false;

						// Then select things in correct order
						foreach(Thing t in selectresult) t.Selected = true;
						break;

					case MarqueSelectionMode.ADD:
						// Get ordered selection
						List<Thing> addresult = GetOrderedSelection(selectstart, selectionrect);

						// First deselect everything inside of selection...
						foreach(Thing t in addresult) t.Selected = false;

						// Then reselect in correct order
						foreach(Thing t in addresult) t.Selected = true;
						break;

					case MarqueSelectionMode.SUBTRACT:
						// Selection order doesn't matter here
						foreach(Thing t in General.Map.ThingsFilter.VisibleThings)
							if(selectionrect.Contains((float)t.Position.x, (float)t.Position.y)) t.Selected = false;
						break;

					// Should be Intersect selection mode
					default:
						// Selection order doesn't matter here
						foreach(Thing t in General.Map.ThingsFilter.VisibleThings)
							if(!selectionrect.Contains((float)t.Position.x, (float)t.Position.y)) t.Selected = false;
						break;
				}

				UpdateSelectionInfo(); //mxd
			}
			
			base.OnEndMultiSelection();

			// Clear overlay
			if(renderer.StartOverlay(true)) renderer.Finish();

			// Redraw
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
			if((General.Map.Map.GetSelectedThings(true).Count == 0) && (highlighted != null))
			{
				// Make the highlight the selection
				highlighted.Selected = true;

				//mxd. Actually, we want it marked, not selected
				bool result = base.OnCopyBegin();
				highlighted.Selected = false;
				return result;
			}

			return base.OnCopyBegin();
		}

		//mxd
		private void RenderComment(Thing t)
		{
			if(t.Fields.ContainsKey("comment"))
			{
				float size = (((t.FixedSize || General.Settings.FixedThingsScale) && renderer.Scale > 1.0f) ? t.Size / renderer.Scale : t.Size);
				if(size * renderer.Scale < 1.5f) return; // Thing is too small to render

				int iconindex = 0;
				string comment = t.Fields.GetValue("comment", string.Empty);
				if(comment.Length > 2)
				{
					string type = comment.Substring(0, 3);
					int index = Array.IndexOf(CommentType.Types, type);
					if(index != -1) iconindex = index;
				}

				RectangleF rect = new RectangleF((float)(t.Position.x + size - 10 / renderer.Scale), (float)(t.Position.y + size + 18 / renderer.Scale), 16 / renderer.Scale, -16 / renderer.Scale);
				PixelColor c = (t == highlighted ? General.Colors.Highlight : (t.Selected ? General.Colors.Selection : PixelColor.FromColor(Color.White)));
				renderer.RenderRectangleFilled(rect, c, true, General.Map.Data.CommentTextures[iconindex]);
			}
		}

		//mxd. Gets map elements inside of selectionoutline and sorts them by distance to targetpoint
		private List<Thing> GetOrderedSelection(Vector2D targetpoint, RectangleF selection)
		{
			// Gather affected sectors
			List<Thing> result = new List<Thing>();
			foreach(Thing t in General.Map.ThingsFilter.VisibleThings)
			{
				if(selection.Contains((float)t.Position.x, (float)t.Position.y)) result.Add(t);
			}

			if(result.Count == 0) return result;

			// Sort by distance to targetpoint
			result.Sort(delegate(Thing t1, Thing t2)
			{
				if(t1 == t2) return 0;

				// Get closest distance from thing to selectstart
				double closest1 = Vector2D.DistanceSq(t1.Position, targetpoint);
				double closest2 = Vector2D.DistanceSq(t2.Position, targetpoint);

				// Return closer one
				return (int)(closest1 - closest2);
			});

			return result;
		}

		//mxd. This sets up new labels
		private void SetupSectorLabels()
		{
			if(!General.Map.FormatInterface.HasThingAction) return;

			// Dispose old labels
			if(sectorlabels != null)
			{
				foreach(TextLabel[] larr in sectorlabels.Values)
					foreach(TextLabel l in larr) l.Dispose();
			}

			// Make text labels for sectors
			sectorlabels = new Dictionary<Sector, TextLabel[]>();
			sectortexts = new Dictionary<Sector, string[]>();
			foreach(Sector s in General.Map.Map.Sectors)
			{
				// Setup labels
				if(s.Tag == 0) continue;

				// Make tag text
				string[] tagdescarr = new string[2];
				if(s.Tags.Count > 1)
				{
					string[] stags = new string[s.Tags.Count];
					for(int i = 0; i < s.Tags.Count; i++) stags[i] = s.Tags[i].ToString();
					tagdescarr[0] = "Tags " + string.Join(", ", stags);
					tagdescarr[1] = "T" + string.Join(",", stags);
				}
				else
				{
					tagdescarr[0] = "Tag " + s.Tag;
					tagdescarr[1] = "T" + s.Tag;
				}

				// Add to collection
				sectortexts.Add(s, tagdescarr);

				TextLabel[] larr = new TextLabel[s.Labels.Count];
				for(int i = 0; i < s.Labels.Count; i++)
				{
					TextLabel l = new TextLabel();
					l.TransformCoords = true;
					l.Location = s.Labels[i].position;
					l.AlignX = TextAlignmentX.Center;
					l.AlignY = TextAlignmentY.Middle;
					l.Color = General.Colors.InfoLine;
					l.BackColor = General.Colors.Background.WithAlpha(128);
					larr[i] = l;
				}

				// Add to collection
				sectorlabels.Add(s, larr);
			}
		}

		//mxd. Also update labels for the selected linedefs
		public override void UpdateSelectionInfo()
		{
			base.UpdateSelectionInfo();
			
			if(labels != null)
			{
				// Dispose old labels
				foreach(TextLabel l in labels.Values) l.Dispose();
			}

			// Make text labels for selected linedefs
			ICollection<Thing> orderedselection = General.Map.Map.GetSelectedThings(true);
			labels = new Dictionary<Thing, TextLabel>(orderedselection.Count);

			// Otherwise significant delays will occure.
			// Also we probably won't care about selection ordering when selecting this many anyway
			if(orderedselection.Count > MAX_THING_LABELS) return;

			int index = 0;
			foreach(Thing thing in orderedselection)
			{
				TextLabel l = new TextLabel();
				l.TransformCoords = true;

				if(thing.FixedSize)
				{
					l.Location = thing.Position;
					l.AlignX = TextAlignmentX.Center;
					l.AlignY = TextAlignmentY.Middle;
				}
				else
				{
					l.Location = new Vector2D(thing.Position.x - thing.Size + 1, thing.Position.y + thing.Size - 1);
					l.AlignX = TextAlignmentX.Left;
					l.AlignY = TextAlignmentY.Top;
				}
				
				l.Color = (thing == highlighted ? General.Colors.Selection : General.Colors.Highlight);
				l.BackColor = General.Colors.Background.WithAlpha(192);
				l.Text = (++index).ToString();
				labels.Add(thing, l);
			}
		}

		//mxd
		private void UpdateHelperObjects()
		{
			// Update event lines and argument shapes
			persistenteventlines = LinksCollector.GetHelperShapes(General.Map.ThingsFilter.VisibleThings);

			// Update light radii
			dynamiclightshapes = LinksCollector.GetDynamicLightShapes(General.Map.ThingsFilter.VisibleThings, false);

			// Update ambient sound radii
			ambientsoundshapes = LinksCollector.GetAmbientSoundShapes(General.Map.ThingsFilter.VisibleThings, false);
		}

		#endregion

		#region ================== Actions

		// This copies the properties
		[BeginAction("classiccopyproperties")]
		public void CopyProperties()
		{
			// Determine source things
			ICollection<Thing> sel = null;
			if(General.Map.Map.SelectedThingsCount > 0) sel = General.Map.Map.GetSelectedThings(true);
			else if(highlighted != null) sel = new List<Thing> { highlighted };

			if(sel != null)
			{
				// Copy properties from the first source thing
				BuilderPlug.Me.CopiedThingProps = new ThingProperties(General.GetByIndex(sel, 0));
				General.Interface.DisplayStatus(StatusType.Action, "Copied thing properties.");
			}
			else
			{
				//mxd
				General.Interface.DisplayStatus(StatusType.Warning, "This action requires highlight or selection!");
			}
		}

		// This pastes the properties
		[BeginAction("classicpasteproperties")]
		public void PasteProperties()
		{
			if(BuilderPlug.Me.CopiedThingProps != null)
			{
				// Determine target things
				ICollection<Thing> sel = null;
				if(General.Map.Map.SelectedThingsCount > 0) sel = General.Map.Map.GetSelectedThings(true);
				else if(highlighted != null) sel = new List<Thing> {highlighted};

				if(sel != null)
				{
					// Apply properties to selection
					string rest = (sel.Count == 1 ? "a single thing" : sel.Count + " things"); //mxd
					General.Map.UndoRedo.CreateUndo("Paste properties to " + rest);
					BuilderPlug.Me.CopiedThingProps.Apply(sel, false);
					foreach(Thing t in sel) t.UpdateConfiguration();
					General.Interface.DisplayStatus(StatusType.Action, "Pasted properties to " + rest + ".");
					
					// Update
					General.Map.IsChanged = true;
					General.Map.ThingsFilter.Update();
					General.Interface.RefreshInfo();

					//mxd. Update helper lines
					UpdateHelperObjects();

					// Redraw
					General.Interface.RedrawDisplay();
				}
				else
				{
					//mxd
					General.Interface.DisplayStatus(StatusType.Warning, "This action requires highlight or selection!");
				}
			}
			else
			{
				//mxd
				General.Interface.DisplayStatus(StatusType.Warning, "Copy thing properties first!");
			}
		}

		//mxd. This pastes the properties with options
		[BeginAction("classicpastepropertieswithoptions")]
		public void PastePropertiesWithOptions()
		{
			if(BuilderPlug.Me.CopiedThingProps != null)
			{
				// Determine target things
				ICollection<Thing> sel = null;
				if(General.Map.Map.SelectedThingsCount > 0) sel = General.Map.Map.GetSelectedThings(true);
				else if(highlighted != null) sel = new List<Thing> { highlighted };

				if(sel != null)
				{
					PastePropertiesOptionsForm form = new PastePropertiesOptionsForm();
					if(form.Setup(MapElementType.THING) && form.ShowDialog(General.Interface) == DialogResult.OK)
					{
						// Apply properties to selection
						string rest = (sel.Count == 1 ? "a single thing" : sel.Count + " things");
						General.Map.UndoRedo.CreateUndo("Paste properties with options to " + rest);
						BuilderPlug.Me.CopiedThingProps.Apply(sel, true);
						foreach(Thing t in sel) t.UpdateConfiguration();
						General.Interface.DisplayStatus(StatusType.Action, "Pasted properties with options to " + rest + ".");

						// Update
						General.Map.IsChanged = true;
						General.Map.ThingsFilter.Update();
						General.Interface.RefreshInfo();

						//mxd. Update helper lines
						UpdateHelperObjects();

						// Redraw
						General.Interface.RedrawDisplay();
					}
				}
				else
				{
					General.Interface.DisplayStatus(StatusType.Warning, "This action requires highlight or selection!");
				}
			}
			else
			{
				General.Interface.DisplayStatus(StatusType.Warning, "Copy thing properties first!");
			}
		}

		// This clears the selection
		[BeginAction("clearselection", BaseAction = true)]
		public void ClearSelection()
		{
			// Clear selection
			General.Map.Map.ClearAllSelected();

			//mxd. Clear selection info
			General.Interface.DisplayStatus(StatusType.Selection, string.Empty);

			//mxd. Clear selection labels
			foreach(TextLabel l in labels.Values) l.Dispose();
			labels.Clear();

			// Redraw
			General.Interface.RedrawDisplay();
		}
		
		// This creates a new thing at the mouse position
		[BeginAction("insertitem", BaseAction = true)]
		public void InsertThing()
		{
			// Mouse in window?
			if(mouseinside)
			{
				// Insert new thing
				General.Map.UndoRedo.CreateUndo("Insert thing");
				Thing t = InsertThing(mousemappos);

				if(t == null)
				{
					General.Map.UndoRedo.WithdrawUndo();
					return;
				}
				
				// Edit the thing?
				if(BuilderPlug.Me.EditNewThing)
				{
					// Redraw screen
					General.Interface.RedrawDisplay();
					General.Interface.ShowEditThings(new List<Thing> { t });
				}

				General.Interface.DisplayStatus(StatusType.Action, "Inserted a new thing.");

				// Update things filter
				General.Map.ThingsFilter.Update();

				//mxd. Update helper lines
				UpdateHelperObjects();

				// Redraw screen
				General.Interface.RedrawDisplay();
			}
		}

		// This creates a new thing
		private static Thing InsertThing(Vector2D pos)
		{
			if(pos.x < General.Map.Config.LeftBoundary || pos.x > General.Map.Config.RightBoundary ||
			   pos.y > General.Map.Config.TopBoundary || pos.y < General.Map.Config.BottomBoundary)
			{
				General.Interface.DisplayStatus(StatusType.Warning, "Failed to insert thing: outside of map boundaries.");
				return null;
			}

			// Create thing
			Thing t = General.Map.Map.CreateThing();
			if(t != null)
			{
				General.Settings.ApplyDefaultThingSettings(t);
				t.Move(pos);
				t.UpdateConfiguration();

				// Update things filter so that it includes this thing
				General.Map.ThingsFilter.Update();

				// Snap to grid enabled?
				if(General.Interface.SnapToGrid)
				{
					// Snap to grid
					t.SnapToGrid();
				}
				else
				{
					// Snap to map format accuracy
					t.SnapToAccuracy();
				}
			}
			
			return t;
		}

		[BeginAction("deleteitem", BaseAction = true)]
		public void DeleteItem()
		{
			// Make list of selected things
			List<Thing> selected = new List<Thing>(General.Map.Map.GetSelectedThings(true));
			if((selected.Count == 0) && (highlighted != null) && !highlighted.IsDisposed) selected.Add(highlighted);
			
			// Anything to do?
			if(selected.Count > 0)
			{
				// Make undo
				if(selected.Count > 1)
				{
					General.Map.UndoRedo.CreateUndo("Delete " + selected.Count + " things");
					General.Interface.DisplayStatus(StatusType.Action, "Deleted " + selected.Count + " things.");
				}
				else
				{
					General.Map.UndoRedo.CreateUndo("Delete thing");
					General.Interface.DisplayStatus(StatusType.Action, "Deleted a thing.");
				}

				DeleteThings(selected); //mxd
				
				// Update cache values
				General.Map.IsChanged = true;
				General.Map.ThingsFilter.Update();

				//mxd. Update helper lines
				UpdateHelperObjects();

				// Redraw screen
				UpdateSelectionInfo(); //mxd
				General.Interface.RedrawDisplay();

				// Invoke a new mousemove so that the highlighted item updates
				OnMouseMove(new MouseEventArgs(MouseButtons.None, 0, (int)mousepos.x, (int)mousepos.y, 0));
			}
		}

		//mxd
		[BeginAction("thingaligntowall")]
		public void AlignThingsToWall() 
		{
			// Make list of selected things
			List<Thing> selected = new List<Thing>(General.Map.Map.GetSelectedThings(true));
			if((selected.Count == 0) && (highlighted != null) && !highlighted.IsDisposed) selected.Add(highlighted);

			if(selected.Count == 0) 
			{
				General.Interface.DisplayStatus(StatusType.Warning, "This action requires a selection!");
				return;
			}

			List<Thing> toAlign = new List<Thing>();
			foreach(Thing t in selected)
			{
				if(Thing.AlignableRenderModes.Contains(t.RenderMode)) toAlign.Add(t);
			}

			if(toAlign.Count == 0) 
			{
				General.Interface.DisplayStatus(StatusType.Warning, "This action only works for models or things with FLATSPRITE/WALLSPRITE flags!");
				return;
			}
 
			// Make undo
			if(toAlign.Count > 1) 
			{
				General.Map.UndoRedo.CreateUndo("Align " + toAlign.Count + " things");
				General.Interface.DisplayStatus(StatusType.Action, "Aligned " + toAlign.Count + " things.");
			} 
			else 
			{
				General.Map.UndoRedo.CreateUndo("Align thing");
				General.Interface.DisplayStatus(StatusType.Action, "Aligned a thing.");
			}

			//align things
			foreach(Thing t in toAlign) 
			{
				HashSet<Linedef> excludedLines = new HashSet<Linedef>();
				bool aligned;

				do
				{
					Linedef l = General.Map.Map.NearestLinedef(t.Position, excludedLines);
					aligned = Tools.TryAlignThingToLine(t, l);
					
					if(!aligned) 
					{
						excludedLines.Add(l);
						if(excludedLines.Count == General.Map.Map.Linedefs.Count) 
						{
							ThingTypeInfo tti = General.Map.Data.GetThingInfo(t.Type);
							General.ErrorLogger.Add(ErrorType.Warning, "Unable to align " + tti.Title + " (index " + t.Index + ") to any linedef!");
							aligned = true;
						}
					}
				} while(!aligned);
			}

			// Update cache values
			General.Map.IsChanged = true;

			// Redraw screen
			General.Interface.RedrawDisplay();
		}

		[BeginAction("thinglookatcursor")]
		public void ThingPointAtCursor() 
		{
			// Make list of selected things
			List<Thing> selected = new List<Thing>(General.Map.Map.GetSelectedThings(true));
			if((selected.Count == 0) && (highlighted != null) && !highlighted.IsDisposed)
				selected.Add(highlighted);

			if(selected.Count == 0) 
			{
				General.Interface.DisplayStatus(StatusType.Warning, "This action requires a selection!");
				return;
			}

			//check mouse position
			if(!mousemappos.IsFinite()) 
			{
				awaitingMouseClick = true;
				General.Interface.DisplayStatus(StatusType.Warning, "Now click in the editing area!");
				return;
			}

			awaitingMouseClick = false;

			// Make undo
			if(selected.Count > 1) 
			{
				General.Map.UndoRedo.CreateUndo("Rotate " + selected.Count + " things");
				General.Interface.DisplayStatus(StatusType.Action, "Rotated " + selected.Count + " things.");
			} 
			else 
			{
				General.Map.UndoRedo.CreateUndo("Rotate thing");
				General.Interface.DisplayStatus(StatusType.Action, "Rotated a thing.");
			}

			// Change angle
			if(General.Interface.CtrlState) // Point away
			{ 
				foreach(Thing t in selected) 
				{
					ThingTypeInfo info = General.Map.Data.GetThingInfo(t.Type);
					if(info == null || info.Category == null || info.Category.Arrow == 0)
						continue;

					int newangle = Angle2D.RealToDoom(Vector2D.GetAngle(mousemappos, t.Position) + Angle2D.PI);
					if(General.Map.Config.DoomThingRotationAngles) newangle = (newangle + 22) / 45 * 45;

					t.Rotate(newangle);
				}
			} 
			else // Point at cursor
			{ 
				foreach(Thing t in selected) 
				{
					ThingTypeInfo info = General.Map.Data.GetThingInfo(t.Type);
					if(info == null || info.Category == null || info.Category.Arrow == 0)
						continue;

					int newangle = Angle2D.RealToDoom(Vector2D.GetAngle(mousemappos, t.Position));
					if(General.Map.Config.DoomThingRotationAngles) newangle = (newangle + 22) / 45 * 45;

					t.Rotate(newangle);
				}
			}

			// Redraw screen
			General.Interface.RedrawDisplay();
		}

		//mxd. rotate clockwise
		[BeginAction("rotateclockwise")]
		public void RotateCW() 
		{
			RotateThings(General.Map.Config.DoomThingRotationAngles ? -45 : -5);
		}

		//mxd. rotate counterclockwise
		[BeginAction("rotatecounterclockwise")]
		public void RotateCCW() 
		{
			RotateThings(General.Map.Config.DoomThingRotationAngles ? 45 : 5);
		}

		//mxd
		private void RotateThings(int increment) 
		{
			// Make list of selected things
			List<Thing> selected = new List<Thing>(General.Map.Map.GetSelectedThings(true));
			if(selected.Count == 0 && highlighted != null && !highlighted.IsDisposed)
				selected.Add(highlighted);

			if(selected.Count == 0) 
			{
				General.Interface.DisplayStatus(StatusType.Warning, "This action requires a selection!");
				return;
			}

			// Make undo
			if(selected.Count > 1) 
			{
				General.Map.UndoRedo.CreateUndo("Rotate " + selected.Count + " things", this, UndoGroup.ThingAngleChange, CreateSelectionCRC(selected));
				General.Interface.DisplayStatus(StatusType.Action, "Rotated " + selected.Count + " things.");
			} 
			else 
			{
				General.Map.UndoRedo.CreateUndo("Rotate thing", this, UndoGroup.ThingAngleChange, CreateSelectionCRC(selected));
				General.Interface.DisplayStatus(StatusType.Action, "Rotated a thing.");
			}

			// Change angle
			foreach(Thing t in selected)
			{
				int newangle = t.AngleDoom + increment;
				if(General.Map.Config.DoomThingRotationAngles) newangle = newangle / 45 * 45;
				t.Rotate(General.ClampAngle(newangle));
			}

			// Redraw screen
			General.Interface.RedrawDisplay();
			General.Interface.RefreshInfo();
		}

		//mxd
		[BeginAction("filterselectedthings")]
		public void ShowFilterDialog() 
		{
			ICollection<Thing> selection = General.Map.Map.GetSelectedThings(true);

			if(selection.Count == 0) 
			{
				General.Interface.DisplayStatus(StatusType.Warning, "This action requires a selection!");
				return;
			}

			new FilterSelectedThingsForm(selection, this).ShowDialog();
		}

		//mxd
		[BeginAction("selectsimilar")]
		public void SelectSimilar() 
		{
			ICollection<Thing> selection = General.Map.Map.GetSelectedThings(true);

			if(selection.Count == 0) 
			{
				General.Interface.DisplayStatus(StatusType.Warning, "This action requires a selection!");
				return;
			}

			var form = new SelectSimilarElementOptionsPanel();
			if(form.Setup(this)) form.ShowDialog(General.Interface);
		}

		[BeginAction("smartgridtransform", BaseAction = true)]
		protected void SmartGridTransform()
		{
			if(General.Map.Map.SelectedThingsCount > 1)
			{
				General.Interface.DisplayStatus(StatusType.Warning, "Either nothing or exactly one thing must be selected");
				General.Interface.MessageBeep(MessageBeepType.Warning);
				return;
			}

			Thing thing = null;

			if (General.Map.Map.SelectedThingsCount == 1)
				thing = General.Map.Map.GetSelectedThings(true).First();
			else if (highlighted != null)
				thing = highlighted;

			if(thing != null)
			{
				General.Map.Grid.SetGridOrigin(thing.Position.x, thing.Position.y);
				General.Map.GridVisibilityChanged();
				General.Interface.RedrawDisplay();
			}
			else
			{
				General.Map.Grid.SetGridRotation(0.0);
				General.Map.Grid.SetGridOrigin(0, 0);
				General.Map.GridVisibilityChanged();
				General.Interface.RedrawDisplay();
			}
		}

		[BeginAction("changemapelementindex")]
		private void ChangeMapElementIndex()
		{
			// Make list of selected linedefs
			List<Thing> selected = General.Map.Map.GetSelectedThings(true).ToList();
			if ((selected.Count == 0) && (highlighted != null) && !highlighted.IsDisposed) selected.Add(highlighted);
			if (selected.Count != 1)
			{
				General.ToastManager.ShowToast(ToastMessages.CHANGEMAPELEMENTINDEX, ToastType.WARNING, "Changing thing index failed", "You need to select or highlight exactly 1 thing.");
				return;
			}

			ChangeMapElementIndexForm f = new ChangeMapElementIndexForm("thing", selected[0].Index, General.Map.Map.Things.Count - 1);
			if (f.ShowDialog() == DialogResult.OK)
			{
				int newindex = f.GetNewIndex();
				int oldindex = selected[0].Index;
				General.Map.UndoRedo.CreateUndo("Change thing index");

				selected[0].ChangeIndex(newindex);

				General.ToastManager.ShowToast(ToastMessages.CHANGEMAPELEMENTINDEX, ToastType.INFO, "Successfully change thing index", $"Changed index of thing {oldindex} to {newindex}.");
			}
		}

		#endregion
	}
}
