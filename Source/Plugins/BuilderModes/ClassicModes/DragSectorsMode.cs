
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
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Editing;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	// No action or button for this mode, it is automatic.
	// The EditMode attribute does not have to be specified unless the
	// mode must be activated by class name rather than direct instance.
	// In that case, just specifying the attribute like this is enough:
	// [EditMode]

	[EditMode(DisplayName = "Sectors",
			  AllowCopyPaste = false,
			  Volatile = true)]

	public sealed class DragSectorsMode : DragGeometryMode
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		private ICollection<Linedef> draglines;
		private ICollection<Sector> dragsectors;

		#endregion

		#region ================== Properties
		
		#endregion

		#region ================== Constructor / Disposer

		// Constructor to start dragging immediately
		public DragSectorsMode(Vector2D dragstartmappos, ICollection<Sector> sectors, ICollection<Thing> things)
		{
			// Mark what we are dragging
			General.Map.Map.ClearAllMarks(false);

			// Get geometry to drag
			dragsectors = new List<Sector>(sectors);
			draglines = new HashSet<Linedef>();
			foreach (Sector s in sectors)
			{
				foreach (Sidedef sd in s.Sidedefs)
				{
					draglines.Add(sd.Line);
					sd.Line.Start.Marked = true;
					sd.Line.End.Marked = true;
				}
			}

			// If we got things they will be dragged, otherwise 
			if (things != null)
				thingstodrag = things;
			
			// Initialize
			base.StartDrag(dragstartmappos);
			undodescription = (dragsectors.Count == 1 ? "Drag sector" : "Drag " + dragsectors.Count + " sectors"); //mxd
			
			// We have no destructor
			GC.SuppressFinalize(this);
		}

		// Disposer
		public override void Dispose()
		{
			// Not already disposed?
			if(!isdisposed)
			{
				// Clean up

				// Done
				base.Dispose();
			}
		}

		#endregion

		#region ================== Methods

		// Mode engages
		public override void OnEngage()
		{
			base.OnEngage();
			renderer.SetPresentation(Presentation.Standard);
		}
		
		// This redraws the display
		public override void OnRedrawDisplay()
		{
			renderer.RedrawSurface();

			UpdateRedraw();
			
			// Redraw things when view changed
			if(CheckViewChanged())
			{
				if(renderer.StartThings(true))
				{
					renderer.RenderThingSet(General.Map.Map.Things, General.Settings.ActiveThingsAlpha);
					renderer.Finish();
				}
			}

			renderer.Present();
		}
		
		// This redraws only the required things
		protected override void UpdateRedraw()
		{
			// Start rendering
			if(renderer.StartPlotter(true))
			{
				// Render lines and vertices
				renderer.PlotLinedefSet(snaptolines);
				renderer.PlotLinedefSet(unstablelines);
				renderer.PlotLinedefSet(draglines);
				renderer.PlotVerticesSet(General.Map.Map.Vertices);

				// Draw the dragged item highlighted
				// This is important to know, because this item is used
				// for snapping to the grid and snapping to nearest items
				renderer.PlotVertex(dragitem, ColorCollection.HIGHLIGHT, false);
				
				// Done
				renderer.Finish();
			}

			//mxd. Render things
			if(renderer.StartThings(true)) 
			{
				renderer.RenderThingSet(General.Map.ThingsFilter.HiddenThings, General.Settings.HiddenThingsAlpha);
				renderer.RenderThingSet(unselectedthings, General.Settings.ActiveThingsAlpha);
				renderer.RenderThingSet(selectedthings, General.Settings.ActiveThingsAlpha);
				renderer.Finish();
			}

			// Redraw overlay
			if(renderer.StartOverlay(true))
			{
				renderer.RenderText(labels);
				renderer.Finish();
			}
		}
		
		#endregion
	}
}
