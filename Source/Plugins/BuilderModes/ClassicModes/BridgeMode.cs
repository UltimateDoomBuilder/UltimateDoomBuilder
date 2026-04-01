#region ================== Namespaces

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Editing;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Actions;
using CodeImp.DoomBuilder.Windows;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.BuilderModes.Interface;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes.ClassicModes 
{
	[EditMode(DisplayName = "Bridge Mode",
			  SwitchAction = "bridgemode",
			  ButtonImage = "BridgeMode.png",
			  ButtonOrder = 2,
			  ButtonGroup = "002_modify",
			  AllowCopyPaste = false,
			  Volatile = true,
			  Optional = false)]

	public class BridgeMode : BaseClassicMode
	{
		#region ================== Constants

		private const float GRIP_SIZE = 9.0f;
		private const float LINE_THICKNESS = 0.8f;

		internal const int MAX_SUBDIVISIONS = 32;
		internal const int MIN_SUBDIVISIONS = 0;

		#endregion

		#region ================== Variables

		private Vector2D[] pointGroup1;
		private Vector2D[] pointGroup2;

		private SectorProperties[] sectorProps1;
		private SectorProperties[] sectorProps2;

		private double[] relLenGroup1;
		private double[] relLenGroup2;

		private ControlHandle[] controlHandles;
		private int curControlHandle = -1;

		private PixelColor handleColor;

		private List<Vector2D[]> curves;
		private int segmentsCount;

		// Options
		private bool snaptogrid; // SHIFT to toggle

		//tools form
		private BridgeModeForm form;

		#endregion

		#region ================== Constructor / Disposer

		public BridgeMode() 
		{
			// We have no destructor
			GC.SuppressFinalize(this);
		}

		#endregion

		#region ================== Methods

		// Engaging
		public override void OnEngage() 
		{
			base.OnEngage();

			renderer.SetPresentation(Presentation.Standard);
			
			//check selection
			ICollection<Linedef> selection = General.Map.Map.GetSelectedLinedefs(true);

			List<Line> lines = new List<Line>();
			foreach(Linedef ld in selection) 
			{
				Line l = new Line(ld);
				lines.Add(l);
			}

			//do we have valid selection?
			if(!Setup(lines)) 
			{
				FinishDraw();
				return;
			}

			//show form
			form = new BridgeModeForm();
			form.OnCancelClick += form_OnCancelClick;
			form.OnOkClick += form_OnOkClick;
			form.OnFlipClick += form_OnFlipClick;
			form.OnSubdivisionChanged += form_OnSubdivisionChanged;
			form.Show(General.Interface);
			General.Interface.FocusDisplay();

			handleColor = General.Colors.BrightColors[new Random().Next(General.Colors.BrightColors.Length - 1)];
			Update();
		}

		// When select button is pressed
		protected override void OnSelectBegin() 
		{
			base.OnSelectBegin();

			//check if control handle is selected
			for(int i = 0; i < 4; i++) 
			{
				if(mousemappos.x <= controlHandles[i].Position.x + GRIP_SIZE 
					&& mousemappos.x >= controlHandles[i].Position.x - GRIP_SIZE 
					&& mousemappos.y <= controlHandles[i].Position.y + GRIP_SIZE 
					&& mousemappos.y >= controlHandles[i].Position.y - GRIP_SIZE) 
				{
					curControlHandle = i;
					General.Interface.SetCursor(Cursors.Cross);
					return;
				}
			}
			curControlHandle = -1;
		}

		// When select button is released
		protected override void OnSelectEnd() 
		{
			base.OnSelectEnd();
			General.Interface.SetCursor(Cursors.Default);
			curControlHandle = -1;
		}

		// Mouse moves
		public override void OnMouseMove(MouseEventArgs e) 
		{
			base.OnMouseMove(e);
			if(panning) return; //mxd. Skip all this jass while panning

			if(curControlHandle != -1) 
			{
				ControlHandle handle = controlHandles[curControlHandle];
				
				handle.Position = (snaptogrid ? General.Map.Grid.SnappedToGrid(mousemappos) : mousemappos);

				if(form.MirrorMode) 
				{
					Vector2D pos = handle.RelativePosition;
					//handle angle
					double angle = Math.Atan2(-pos.y, -pos.x) + Angle2D.PIHALF;
					//angle of line, connecting handles ControlledPoints
					double dirAngle = -Math.Atan2(handle.Pair.ControlledPoint.y - handle.ControlledPoint.y, handle.Pair.ControlledPoint.x - handle.ControlledPoint.x);
					double length = Math.Sqrt(Math.Pow(Math.Abs(pos.x), 2.0) + Math.Pow(Math.Abs(pos.y), 2.0));
					double mirroredAngle = angle + dirAngle * 2.0;

					handle.Pair.RelativePosition = new Vector2D(Math.Sin(mirroredAngle) * length, Math.Cos(mirroredAngle) * length);
				} 
				else if(form.CopyMode) 
				{
					handle.Pair.RelativePosition = handle.RelativePosition;
				}    
				Update();
			}
		}

		// Accepted
		public override void OnAccept() 
		{
			Cursor.Current = Cursors.AppStarting;
			General.Settings.FindDefaultDrawSettings();

			//get vertices
			List<List<Vector2D[]>> shapes = GetShapes();
			List<List<List<DrawnVertex>>> drawShapes = new List<List<List<DrawnVertex>>>();

			//set stitch range
			float stitchrange = BuilderPlug.Me.StitchRange;
			BuilderPlug.Me.StitchRange = 0.1f;

			for(int i = 0; i < shapes.Count; i++) 
			{
				List<List<DrawnVertex>> shapesRow = new List<List<DrawnVertex>>();
				for(int c = 0; c < shapes[i].Count; c++) 
				{
					List<DrawnVertex> points = new List<DrawnVertex>();
					for(int p = 0; p < shapes[i][c].Length; p++)
						points.Add(DrawGeometryMode.GetCurrentPosition(shapes[i][c][p], true, false, false, false, renderer, points));
					shapesRow.Add(points);
				}
				drawShapes.Add(shapesRow);
			}

			//restore stitch range
			BuilderPlug.Me.StitchRange = stitchrange;

			//draw lines
			if(drawShapes.Count > 0) 
			{
				// Make undo for the draw
				General.Map.UndoRedo.CreateUndo("Bridge ("+form.Subdivisions+" subdivisions)");

				List<List<SectorProperties>> sectorProps = new List<List<SectorProperties>>();
				List<List<HashSet<Sector>>> newSectors = new List<List<HashSet<Sector>>>();
				
				//create sector properties collection
				//sector row
				for(int i = 0; i < drawShapes.Count; i++) 
				{
					sectorProps.Add(new List<SectorProperties>());
					
					//sector in row
					for(int c = 0; c < drawShapes[i].Count; c++)
						sectorProps[i].Add(GetSectorProperties(i, c));
				}

				// Make the drawing
				//sector row
				for(int i = 0; i < drawShapes.Count; i++) 
				{
					newSectors.Add(new List<HashSet<Sector>>());

					//sector in row
					for(int c = 0; c < drawShapes[i].Count; c++) 
					{
						if(!Tools.DrawLines(drawShapes[i][c], false, true)) 
						{
							// Drawing failed
							// NOTE: I have to call this twice, because the first time only cancels this volatile mode
							General.Interface.DisplayStatus(StatusType.Warning, "Failed to create a Bezier Path...");
							General.Map.UndoRedo.WithdrawUndo();
							General.Map.UndoRedo.WithdrawUndo();
							return;
						}

						HashSet<Sector> newsectors = General.Map.Map.GetUnselectedSectorsFromLinedefs(General.Map.Map.GetMarkedLinedefs(true));
						newSectors[i].Add(newsectors);

						//set floor/ceiling heights and brightness
						foreach(Sector s in newsectors)
						{
							SectorProperties sp = sectorProps[i][c];
							s.Brightness = sp.Brightness;
							s.FloorHeight = sp.FloorHeight;
							s.CeilHeight = (sp.CeilingHeight < sp.FloorHeight ? sp.FloorHeight + 8 : sp.CeilingHeight);
						}
					}
				}

				//apply textures
				//sector row
				for(int i = 0; i < newSectors.Count; i++) 
				{
					//sector in row
					for(int c = 0; c < newSectors[i].Count; c++) 
					{
						foreach(Sector s in newSectors[i][c]) 
						{
							foreach(Sidedef sd in s.Sidedefs)
							{
								if(sd.LowRequired())
									sd.SetTextureLow(sectorProps[i][c].LowTexture);
								if(sd.HighRequired())
									sd.SetTextureHigh(sectorProps[i][c].HighTexture);    
							}
						}
					}
				}

				//apply textures to front/back sides of shape
				//sector row
				for(int i = 0; i < newSectors.Count; i++) 
				{
					//first/last sector in row
					for(int c = 0; c < newSectors[i].Count; c += newSectors[i].Count-1) 
					{
						foreach(Sector s in newSectors[i][c]) 
						{
							foreach(Sidedef sd in s.Sidedefs) 
							{
								if(sd.Other != null) 
								{
									if(sd.Other.LowRequired() && sd.Other.LowTexture == "-")
										sd.Other.SetTextureLow(sectorProps[i][c].LowTexture);
									if(sd.Other.HighRequired() && sd.Other.HighTexture == "-")
										sd.Other.SetTextureHigh(sectorProps[i][c].HighTexture);
								}
							}
						}
					}
				}

				General.Interface.DisplayStatus(StatusType.Action, "Created a Bridge with " + form.Subdivisions + " subdivisions.");

				// Snap to map format accuracy
				General.Map.Map.SnapAllToAccuracy();

				// Clear selection
				General.Map.Map.ClearAllSelected();

				// Update cached values
				General.Map.Map.Update();

				// Update the used textures
				General.Map.Data.UpdateUsedTextures();

				// Map is changed
				General.Map.IsChanged = true;
			}

			//close form
			if(form != null) form.Close();

			// Done
			Cursor.Current = Cursors.Default;

			// Return to original mode
			General.Editing.ChangeMode(General.Editing.PreviousStableMode.Name);
		}

		// Cancelled
		public override void OnCancel() 
		{
			// Cancel base class
			base.OnCancel();

			//close form
			if(form != null) form.Dispose();

			// Return to original mode
			General.Editing.ChangeMode(General.Editing.PreviousStableMode.Name);
		}

		// When a key is released
		public override void OnKeyUp(KeyEventArgs e) 
		{
			base.OnKeyUp(e);
			if(snaptogrid != (General.Interface.ShiftState ^ General.Interface.SnapToGrid)) Update();
		}

		// When a key is pressed
		public override void OnKeyDown(KeyEventArgs e) 
		{
			base.OnKeyDown(e);
			if(snaptogrid != (General.Interface.ShiftState ^ General.Interface.SnapToGrid)) Update();
		}

		// This redraws the display
		public override void OnRedrawDisplay() 
		{
			renderer.RedrawSurface();

			// Render lines
			if(renderer.StartPlotter(true)) 
			{
				renderer.PlotLinedefSet(General.Map.Map.Linedefs);
				renderer.PlotVerticesSet(General.Map.Map.Vertices);
				renderer.Finish();
			}

			// Render things
			if(renderer.StartThings(true)) 
			{
				renderer.RenderThingSet(General.Map.Map.Things, General.Settings.ActiveThingsAlpha);
				renderer.Finish();
			}

			// Normal update
			Update();
		}

		public override void OnHelp() 
		{
			General.ShowHelp("/gzdb/features/classic_modes/mode_drawbridge.html");
		}

		#endregion

		#region ================== Setup/Update/Utility

		//this checks if initial data is valid
		private bool Setup(List<Line> lines) 
		{
			if(!SetupPointGroups(lines)) return false;

			//setup control handles
			Vector2D center1 = CurveTools.GetPointOnLine(pointGroup1[0], pointGroup1[segmentsCount - 1], 0.5f);
			Vector2D center2 = CurveTools.GetPointOnLine(pointGroup2[0], pointGroup2[segmentsCount - 1], 0.5f);

			Vector2D loc1 = GetHandleLocation(pointGroup1[0], pointGroup1[segmentsCount - 1], center2);
			Vector2D loc2 = GetHandleLocation(pointGroup2[0], pointGroup2[segmentsCount - 1], center1);

			ControlHandle ch1 = new ControlHandle {ControlledPoint = pointGroup1[0], RelativePosition = loc1};
			ControlHandle ch2 = new ControlHandle {ControlledPoint = pointGroup2[0], RelativePosition = loc2};
			ControlHandle ch3 = new ControlHandle {ControlledPoint = pointGroup1[segmentsCount - 1], RelativePosition = loc1};
			ControlHandle ch4 = new ControlHandle {ControlledPoint = pointGroup2[segmentsCount - 1], RelativePosition = loc2};

			ch1.Pair = ch3;
			ch2.Pair = ch4;
			ch3.Pair = ch1;
			ch4.Pair = ch2;

			controlHandles = new[] {ch1, ch2, ch3, ch4};

			//setup relative segments lengths
			relLenGroup1 = GetRelativeLengths(pointGroup1);
			relLenGroup2 = GetRelativeLengths(pointGroup2);

			return true;
		}

		private void Update() 
		{
			if(renderer.StartOverlay(true)) 
			{
				snaptogrid = General.Interface.ShiftState ^ General.Interface.SnapToGrid;
				
				PixelColor linesColor = snaptogrid ? General.Colors.Selection : General.Colors.Highlight;
				
				//draw curves
				curves = new List<Vector2D[]>();

				for(int i = 0; i < segmentsCount; i++) 
				{
					Vector2D cp1 = CurveTools.GetPointOnLine(controlHandles[0].Position, controlHandles[2].Position, relLenGroup1[i]);
					Vector2D cp2 = CurveTools.GetPointOnLine(controlHandles[1].Position, controlHandles[3].Position, relLenGroup2[i]);
					curves.Add(CurveTools.GetCubicCurve(pointGroup1[i], pointGroup2[i], cp1, cp2, form.Subdivisions));

					for(int c = 1; c < curves[i].Length; c++)
						renderer.RenderLine(curves[i][c - 1], curves[i][c], LINE_THICKNESS, linesColor, true);
				}

				//draw connecting lines
				if(form.Subdivisions > 1) 
				{
					for(int i = 1; i < segmentsCount; i++) 
					{
						for(int c = 1; c < form.Subdivisions; c++) 
						{
							renderer.RenderLine(curves[i-1][c], curves[i][c], LINE_THICKNESS, linesColor, true);
						}
					}
				}

				//draw vertices
				float vsize = (renderer.VertexSize + 1.0f) / renderer.Scale;

				foreach(Vector2D[] points in curves)
				{
					for(int i = 1; i < points.Length - 1; i++ ) 
					{
						renderer.RenderRectangleFilled(new RectangleF((float)(points[i].x - vsize), (float)(points[i].y - vsize), vsize * 2.0f, vsize * 2.0f), linesColor, true);
					}
				}

				//draw handle lines
				renderer.RenderLine(pointGroup1[0], controlHandles[0].Position, LINE_THICKNESS, handleColor, true);
				renderer.RenderLine(pointGroup2[0], controlHandles[1].Position, LINE_THICKNESS, handleColor, true);
				renderer.RenderLine(pointGroup1[segmentsCount - 1], controlHandles[2].Position, LINE_THICKNESS, handleColor, true);
				renderer.RenderLine(pointGroup2[segmentsCount - 1], controlHandles[3].Position, LINE_THICKNESS, handleColor, true);

				//draw handles
				float gripsize = GRIP_SIZE / renderer.Scale;

				for(int i = 0; i < 4; i++) 
				{
					RectangleF handleRect = new RectangleF((float)(controlHandles[i].Position.x - gripsize * 0.5f), (float)(controlHandles[i].Position.y - gripsize * 0.5f), gripsize, gripsize);
					renderer.RenderRectangleFilled(handleRect, General.Colors.Background, true);
					renderer.RenderRectangle(handleRect, 2, General.Colors.Highlight, true);
				}
				renderer.Finish();
			}
			renderer.Present();
		}

		private SectorProperties GetSectorProperties(int lineIndex, int sectorIndex) 
		{
			float delta = sectorIndex / (float)form.Subdivisions;
			delta += (1.0f - delta) / form.Subdivisions;
			SectorProperties sp = new SectorProperties();

			sp.Brightness = IntepolateValue(sectorProps1[lineIndex].Brightness, sectorProps2[lineIndex].Brightness, delta, form.BrightnessMode);
			sp.FloorHeight = IntepolateValue(sectorProps1[lineIndex].FloorHeight, sectorProps2[lineIndex].FloorHeight, delta, form.FloorAlignMode);
			sp.CeilingHeight = IntepolateValue(sectorProps1[lineIndex].CeilingHeight, sectorProps2[lineIndex].CeilingHeight, delta, form.CeilingAlignMode);

			//textures
			sp.LowTexture = sectorProps1[lineIndex].LowTexture != "-" ? sectorProps1[lineIndex].LowTexture : sectorProps2[lineIndex].LowTexture;
			sp.HighTexture = sectorProps1[lineIndex].HighTexture != "-" ? sectorProps1[lineIndex].HighTexture : sectorProps2[lineIndex].HighTexture;

			return sp;
		}

		//this returns a list of shapes to draw
		private List<List<Vector2D[]>> GetShapes() 
		{
			List<List<Vector2D[]>> shapes = new List<List<Vector2D[]>>();

			for(int i = 1; i < segmentsCount; i++) 
			{
				List<Vector2D[]> segShapes = new List<Vector2D[]>();

				for(int c = 1; c <= form.Subdivisions; c++) 
				{
					Vector2D p0 = curves[i - 1][c - 1];
					Vector2D p1 = curves[i - 1][c];
					Vector2D p2 = curves[i][c];
					Vector2D p3 = curves[i][c - 1];
					segShapes.Add(new[] { p0, p1, p2, p3, p0 });
				}
				shapes.Add(segShapes);
			}

			return shapes;
		}

		#endregion

		#region ================== Point ops

		//this returns an array of linedef lengths relative to total segment length
		private double[] GetRelativeLengths(Vector2D[] pointGroup) 
		{
			double[] relLenGroup = new double[pointGroup.Length];
			relLenGroup[0] = 0.0f;

			//get length and angle of line, which defines the shape
			double length = Vector2D.Distance(pointGroup[0], pointGroup[segmentsCount - 1]);
			double angle = Math.Atan2(pointGroup[0].y - pointGroup[segmentsCount - 1].y, pointGroup[0].x - pointGroup[segmentsCount - 1].x);

			//get relative length of every line
			for(int i = 1; i < pointGroup.Length - 1; i++) 
			{
				Vector2D p0 = pointGroup[i - 1];
				Vector2D p1 = pointGroup[i];
				double curAngle = Math.Atan2(p0.y - p1.y, p0.x - p1.x);
				double diff = (angle + Angle2D.PI) - (curAngle + Angle2D.PI);
				double segLen = (int)(Vector2D.Distance(p0, p1) * Math.Cos(diff));
				relLenGroup[i] = relLenGroup[i - 1] + segLen / length;
			}

			relLenGroup[pointGroup.Length - 1] = 1.0f;

			return relLenGroup;
		}

		//this returns relative handle location
		private static Vector2D GetHandleLocation(Vector2D start, Vector2D end, Vector2D direction) 
		{
			double angle = -Math.Atan2(start.y - end.y, start.x - end.x);
			double dirAngle = -Math.Atan2(direction.y - start.y, direction.x - start.x);
			double length = Math.Sqrt(Math.Pow(Math.Abs(start.x - end.x), 2.0) + Math.Pow(Math.Abs(start.y - end.y), 2.0)) * 0.3f;
			double diff = (angle + Angle2D.PI) - (dirAngle + Angle2D.PI);

			if(diff > Angle2D.PI || (diff < 0 && diff > -Angle2D.PI)) angle += Angle2D.PI;

			return new Vector2D(Math.Sin(angle) * length, Math.Cos(angle) * length);
		}

//LINE DRAWING
		//returns true if 2 lines intersect
		private static bool LinesIntersect(Line line1, Line line2) 
		{
			double zn = (line2.End.y - line2.Start.y) * (line1.End.x - line1.Start.x) - (line2.End.x - line2.Start.x) * (line1.End.y - line1.Start.y);
			double ch1 = (line2.End.x - line2.Start.x) * (line1.Start.y - line2.Start.y) - (line2.End.y - line2.Start.y) * (line1.Start.x - line2.Start.x);
			double ch2 = (line1.End.x - line1.Start.x) * (line1.Start.y - line2.Start.y) - (line1.End.y - line1.Start.y) * (line1.Start.x - line2.Start.x);

			if(zn == 0) return false;
			return (ch1 / zn <= 1 && ch1 / zn >= 0) && (ch2 / zn <= 1 && ch2 / zn >= 0);
		}

		#endregion

		#region ================== Line sorting

		//this gets two arrays of connected points from given lines. Returns true if all went well.
		private bool SetupPointGroups(List<Line> linesList) 
		{
			//find prev/next lines for each line
			for(int i = 0; i < linesList.Count; i++) 
			{
				Line curLine = linesList[i];

				for(int c = 0; c < linesList.Count; c++) 
				{
					if(c != i) //don't wanna play with ourselves :)
					{
						Line line = linesList[c];

						//check start and end points
						if(curLine.Start == line.Start) 
						{
							line.Invert();
							curLine.Previous = line;
						} 
						else if(curLine.Start == line.End) 
						{
							curLine.Previous = line;
						} 
						else if(curLine.End == line.End) 
						{
							line.Invert();
							curLine.Next = line;
						} 
						else if(curLine.End == line.Start) 
						{
							curLine.Next = line;
						}
					}
				}
			}

			List<List<Vector2D>> pointGroups = new List<List<Vector2D>>();
			List<List<Line>> sortedLines = new List<List<Line>>();

			//now find start lines
			for(int i = 0; i < linesList.Count; i++) 
			{
				Line curLine = linesList[i];

				if(curLine.Previous == null) //found start
				{ 
					//collect points
					Line l = curLine;
					List<Vector2D> points = new List<Vector2D>();
					List<Line> lines = new List<Line>();
					points.Add(l.Start);

					do 
					{
						points.Add(l.End);
						lines.Add(l);
					} while((l = l.Next) != null);

					pointGroups.Add(points);
					sortedLines.Add(lines);
				}
			}

			if(pointGroups.Count != 2) 
			{
				General.Interface.DisplayStatus(StatusType.Warning, "Incorrect number of linedef groups! Expected 2, but got " + pointGroups.Count);
				return false;
			}

			if(pointGroups[0].Count != pointGroups[1].Count) 
			{
				General.Interface.DisplayStatus(StatusType.Warning, "Linedefs groups must have equal length! Got " + pointGroups[0].Count + " in first group and " + pointGroups[1].Count + " in second.");
				return false;
			}

			//check if lines from first group intersect with lines from second group
			foreach(Line l1 in sortedLines[0]) 
			{
				foreach(Line l2 in sortedLines[1]) 
				{
					if(LinesIntersect(l1, l2)) 
					{
						General.Interface.DisplayStatus(StatusType.Warning, "One or more lines from first group intersect with one or more lines from second group!");
						return false;
					}
				}
			}

			//both groups count should match at this point
			segmentsCount = pointGroups[0].Count;

			//collect sector properties
			sectorProps1 = new SectorProperties[sortedLines[0].Count];
			for(int i = 0; i < sortedLines[0].Count; i++ ) 
			{
				sectorProps1[i] = sortedLines[0][i].SectorProperties;
			}
			sectorProps2 = new SectorProperties[sortedLines[1].Count];
			for(int i = 0; i < sortedLines[1].Count; i++) 
			{
				sectorProps2[i] = sortedLines[1][i].SectorProperties;
			}

			//check if we need to reverse one of point groups
			Line line1 = new Line(pointGroups[0][0], pointGroups[1][0]);
			Line line2 = new Line(pointGroups[0][segmentsCount - 1], pointGroups[1][segmentsCount - 1]);

			if(LinesIntersect(line1, line2)) 
			{
				pointGroups[0].Reverse();
				Array.Reverse(sectorProps1);
			}

			//fill point groups
			pointGroup1 = new Vector2D[segmentsCount];
			pointGroup2 = new Vector2D[segmentsCount];

			pointGroups[0].CopyTo(pointGroup1);
			pointGroups[1].CopyTo(pointGroup2);

			return true;
		}

		#endregion

		#region ================== Easing functions

		private static int IntepolateValue(int val1, int val2, float delta, string mode) 
		{
			switch(mode) 
			{
				case BridgeInterpolationMode.HIGHEST:
				case BridgeInterpolationMode.BRIGHTNESS_HIGHEST:
					return Math.Max(val1, val2);

				case BridgeInterpolationMode.LOWEST:
				case BridgeInterpolationMode.BRIGHTNESS_LOWEST:
					return Math.Min(val1, val2);
				
				case BridgeInterpolationMode.LINEAR:
					return (int)Math.Round(InterpolationTools.Linear(val1, val2, delta));

				case BridgeInterpolationMode.IN_SINE:
					return (int)Math.Round(InterpolationTools.EaseInSine(val1, val2, delta));

				case BridgeInterpolationMode.OUT_SINE:
					return (int)Math.Round(InterpolationTools.EaseOutSine(val1, val2, delta));

				case BridgeInterpolationMode.IN_OUT_SINE:
					return (int)Math.Round(InterpolationTools.EaseInOutSine(val1, val2, delta));

				default:
					throw new Exception("DrawBezierPathMode.IntepolateValue: \"" + mode + "\" mode is not supported!");
			}
		}

		#endregion

		#region ================== Events

		private void form_OnSubdivisionChanged(object sender, EventArgs e) 
		{
			Update();
		}

		private void form_OnOkClick(object sender, EventArgs e) 
		{
			FinishDraw();
		}

		private void form_OnCancelClick(object sender, EventArgs e) 
		{
			OnCancel();
		}

		private void form_OnFlipClick(object sender, EventArgs e) 
		{
			Array.Reverse(pointGroup1);
			Array.Reverse(sectorProps1);
			
			//swap handles position
			Vector2D p = controlHandles[0].Position;
			controlHandles[0].Position = controlHandles[2].Position;
			controlHandles[2].Position = p;

			Update();
		}

		#endregion

		#region ================== Actions

		// Finish drawing
		[BeginAction("finishdraw")]
		private void FinishDraw() 
		{
			// Accept the changes
			General.Editing.AcceptMode();
		}

		[BeginAction("increasesubdivlevel")]
		private void IncreaseSubdivLevel() 
		{
			if(form != null && form.Subdivisions < MAX_SUBDIVISIONS) form.Subdivisions++;
		}

		[BeginAction("decreasesubdivlevel")]
		private void DecreaseSubdivLevel() 
		{
			if(form != null && form.Subdivisions > MIN_SUBDIVISIONS) form.Subdivisions--;
		}

		#endregion
	}

	#region ================== Helper classes

	internal struct SectorProperties 
	{
		public int FloorHeight;
		public int CeilingHeight;
		public int Brightness;
		public string HighTexture;
		public string LowTexture;
	}

	internal class ControlHandle
	{
		public Vector2D Position;
		public Vector2D ControlledPoint; //point, to which this handle is assigned
		public Vector2D RelativePosition 
		{
			get 
			{
				return new Vector2D(Position.x - ControlledPoint.x, Position.y - ControlledPoint.y);
			}
			set 
			{
				Position = new Vector2D(ControlledPoint.x + value.x, ControlledPoint.y + value.y);
			}
		}
		public ControlHandle Pair; //second handle, to which this handle is paired
	}

	internal class Line 
	{
		public Vector2D Start { get { return start; } }
		private Vector2D start;

		public Vector2D End { get { return end; } }
		private Vector2D end;
		
		public SectorProperties SectorProperties;

		public Line Previous;
		public Line Next;

		public Line(Linedef ld) 
		{
			start = new Vector2D((int)ld.Start.Position.x, (int)ld.Start.Position.y);
			end = new Vector2D((int)ld.End.Position.x, (int)ld.End.Position.y);
			SectorProperties = new SectorProperties();

			if(ld.Back != null) 
			{
				SectorProperties.CeilingHeight = ld.Back.Sector.CeilHeight;
				SectorProperties.FloorHeight = ld.Back.Sector.FloorHeight;
				SectorProperties.Brightness = ld.Back.Sector.Brightness;
				SectorProperties.HighTexture = ld.Back.HighTexture != "-" ? ld.Back.HighTexture : ld.Back.MiddleTexture;
				SectorProperties.LowTexture = ld.Back.LowTexture != "-" ? ld.Back.LowTexture : ld.Back.MiddleTexture;
			}
			else if(ld.Front != null)
			{
				SectorProperties.CeilingHeight = ld.Front.Sector.CeilHeight;
				SectorProperties.FloorHeight = ld.Front.Sector.FloorHeight;
				SectorProperties.Brightness = ld.Front.Sector.Brightness;
				SectorProperties.HighTexture = ld.Front.HighTexture != "-" ? ld.Front.HighTexture : ld.Front.MiddleTexture;
				SectorProperties.LowTexture = ld.Front.LowTexture != "-" ? ld.Front.LowTexture : ld.Front.MiddleTexture;
			}
			else
			{
				SectorProperties.CeilingHeight = 128;
				SectorProperties.FloorHeight = 0;
				SectorProperties.Brightness = 192;
				SectorProperties.HighTexture = "-";
				SectorProperties.LowTexture = "-";
			}
		}

		public Line(Vector2D start, Vector2D end) 
		{
			this.start = start;
			this.end = end;
		}

		public void Invert() 
		{
			Vector2D s = start;
			start = end;
			end = s;
		}
	}

	#endregion
}
