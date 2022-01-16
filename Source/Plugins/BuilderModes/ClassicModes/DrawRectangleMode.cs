#region ================== Namespaces

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Editing;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Actions;
using CodeImp.DoomBuilder.Windows;
using CodeImp.DoomBuilder.Map;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	[EditMode(DisplayName = "Draw Rectangle Mode",
			  SwitchAction = "drawrectanglemode",
			  ButtonImage = "DrawRectangleMode.png", //mxd	
			  ButtonOrder = int.MinValue + 3, //mxd
			  ButtonGroup = "000_drawing", //mxd
			  AllowCopyPaste = false,
			  Volatile = true,
			  Optional = false)]

	public class DrawRectangleMode : DrawGeometryMode
	{
		#region ================== Variables

		protected HintLabel hintlabel;
		protected int bevelwidth;
		protected int currentbevelwidth;
		protected int subdivisions;

		protected int maxsubdivisions;
		protected int minsubdivisions;

		protected string undoname = "Rectangle draw";
		protected string shapename = "rectangle";

		protected Vector2D start;
		protected Vector2D end;
		protected int width;
		protected int height;
		protected int minpointscount;
		protected bool alwaysrendershapehints;
		protected bool radialdrawing;

		private bool blockupdate;

		// Interface
		private DrawRectangleOptionsPanel panel;

		#endregion

		#region ================== Constructor/Disposer

		public DrawRectangleMode() 
		{
			snaptogrid = true;
			usefourcardinaldirections = true;
			autoclosedrawing = false;
		}

		public override void Dispose() 
		{
			// Not already disposed?
			if(!isdisposed)
			{
				// Clean up
				if(hintlabel != null) hintlabel.Dispose();

				// Done
				base.Dispose();
			}
		}

		#endregion

		#region ================== Settings panel

		protected override void SetupInterface() 
		{
            minsubdivisions = 0;
			maxsubdivisions = 16;
			minpointscount = 4;

			// Load stored settings
			subdivisions = General.Clamp(General.Settings.ReadPluginSetting("drawrectanglemode.subdivisions", 0), minsubdivisions, maxsubdivisions);
			bevelwidth = General.Settings.ReadPluginSetting("drawrectanglemode.bevelwidth", 0);
			currentbevelwidth = bevelwidth;

			//Add options docker
			panel = new DrawRectangleOptionsPanel();
			panel.MaxSubdivisions = maxsubdivisions;
			panel.MinSubdivisions = minsubdivisions;
			panel.MaxBevelWidth = (int)General.Map.FormatInterface.MaxCoordinate;
			panel.MinBevelWidth = (int)General.Map.FormatInterface.MinCoordinate;
			panel.BevelWidth = bevelwidth;
			panel.Subdivisions = subdivisions;
			panel.OnValueChanged += OptionsPanelOnValueChanged;
			panel.OnContinuousDrawingChanged += OnContinuousDrawingChanged;
			panel.OnShowGuidelinesChanged += OnShowGuidelinesChanged;
			panel.OnRadialDrawingChanged += OnRadialDrawingChanged;

			// Needs to be set after adding the OnContinuousDrawingChanged event...
			panel.ContinuousDrawing = General.Settings.ReadPluginSetting("drawrectanglemode.continuousdrawing", false);
			panel.ShowGuidelines = General.Settings.ReadPluginSetting("drawrectanglemode.showguidelines", false);
			panel.RadialDrawing = General.Settings.ReadPluginSetting("drawrectanglemode.radialdrawing", false);
		}

		protected override void AddInterface() 
		{
			panel.Register();
		}

		protected override void RemoveInterface()
		{
			// Store settings
			General.Settings.WritePluginSetting("drawrectanglemode.subdivisions", subdivisions);
			General.Settings.WritePluginSetting("drawrectanglemode.bevelwidth", bevelwidth);
			General.Settings.WritePluginSetting("drawrectanglemode.continuousdrawing", panel.ContinuousDrawing);
			General.Settings.WritePluginSetting("drawrectanglemode.showguidelines", panel.ShowGuidelines);
			General.Settings.WritePluginSetting("drawrectanglemode.radialdrawing", panel.RadialDrawing);

			// Remove the buttons
			panel.Unregister();
		}

		#endregion

		#region ================== Methods

		override protected void Update() 
		{
			if(blockupdate) return;
			
			PixelColor stitchcolor = General.Colors.Highlight;
			PixelColor losecolor = General.Colors.Selection;

			snaptocardinaldirection = General.Interface.ShiftState && General.Interface.AltState; //mxd
			snaptogrid = (snaptocardinaldirection || General.Interface.ShiftState ^ General.Interface.SnapToGrid);
			snaptonearest = General.Interface.CtrlState ^ General.Interface.AutoMerge;

			DrawnVertex curp = GetCurrentPosition();
			Vector2D curvertexpos = curp.pos;
			float vsize = (renderer.VertexSize + 1.0f) / renderer.Scale;

			curp.pos = curp.pos.GetRotated(-General.Map.Grid.GridRotate);

			// Render drawing lines
			if(renderer.StartOverlay(true)) 
			{
				PixelColor color = snaptonearest ? stitchcolor : losecolor;

				if (points.Count == 1) 
				{
					UpdateReferencePoints(points[0], curp);

					Vector2D[] shape = GetShape(start, end);

					Vector2D startrotated = start.GetRotated(General.Map.Grid.GridRotate);
					Vector2D endrotated = end.GetRotated(General.Map.Grid.GridRotate);

					// Rotate the shape to fit the grid rotation
					for (int i = 0; i < shape.Length; i++)
						shape[i] = shape[i].GetRotated(General.Map.Grid.GridRotate);

					// Render guidelines
					if(showguidelines)
						RenderGuidelines(startrotated, endrotated, General.Colors.Guideline.WithAlpha(80), -General.Map.Grid.GridRotate);

					//render shape
					for(int i = 1; i < shape.Length; i++)
						renderer.RenderLine(shape[i - 1], shape[i], LINE_THICKNESS, color, true);

					//vertices
					for(int i = 0; i < shape.Length; i++)
						renderer.RenderRectangleFilled(new RectangleF((float)(shape[i].x - vsize), (float)(shape[i].y - vsize), vsize * 2.0f, vsize * 2.0f), color, true);

					//and labels
					if(shape.Length == 2)
					{
						// Render label for line
						labels[0].Move(startrotated, endrotated);
						renderer.RenderText(labels[0].TextLabel);
					}
					else if(shape.Length > 3)
					{
						// Render labels for rectangle
						Vector2D[] labelCoords = { startrotated, new Vector2D(end.x, start.y).GetRotated(General.Map.Grid.GridRotate), endrotated, new Vector2D(start.x, end.y).GetRotated(General.Map.Grid.GridRotate), startrotated };

						for (int i = 1; i < 5; i++) 
						{
							labels[i - 1].Move(labelCoords[i], labelCoords[i - 1]);
							renderer.RenderText(labels[i - 1].TextLabel);
						}

						//got beveled corners? 
						if(alwaysrendershapehints || shape.Length > minpointscount + 1)
						{
							//render hint
							if(width > 64 * vsize && height > 16 * vsize)
							{
								hintlabel.Move(startrotated, endrotated);
								hintlabel.Text = GetHintText();
								renderer.RenderText(hintlabel.TextLabel);
							}

							//and shape corners
							for(int i = 0; i < 4; i++)
								renderer.RenderRectangleFilled(new RectangleF((float)(labelCoords[i].x - vsize), (float)(labelCoords[i].y - vsize), vsize * 2.0f, vsize * 2.0f), General.Colors.InfoLine, true);
						}
					}
					else
					{
						// Render vertex at points[0]
						renderer.RenderRectangleFilled(new RectangleF((float)(start.x - vsize), (float)(start.y - vsize), vsize * 2.0f, vsize * 2.0f), General.Colors.InfoLine, true);
					}
				} 
				else 
				{
					// Render vertex at cursor
					renderer.RenderRectangleFilled(new RectangleF((float)(curvertexpos.x - vsize), (float)(curvertexpos.y - vsize), vsize * 2.0f, vsize * 2.0f), color, true);
				}

				// Done
				renderer.Finish();
			}

			// Done
			renderer.Present();
		}

		protected virtual Vector2D[] GetShape(Vector2D pStart, Vector2D pEnd) 
		{
			//no shape
			if(pStart == pEnd) 
			{
				currentbevelwidth = 0;
				return new Vector2D[0];
			}

			//line
			if(pEnd.x == pStart.x || pEnd.y == pStart.y) 
			{
				currentbevelwidth = 0;
				return new[] { pStart, pEnd };
			}

			//no corners
			if(bevelwidth == 0) 
			{
				currentbevelwidth = 0;
				return new[] { pStart, new Vector2D(pStart.x, pEnd.y), pEnd, new Vector2D(pEnd.x, pStart.y), pStart };
			}

			//got corners. TODO: check point order
			bool reverse = false;
			currentbevelwidth = Math.Min(Math.Abs(bevelwidth), Math.Min(width, height) / 2);
			
			if(bevelwidth < 0) 
			{
				currentbevelwidth *= -1;
				reverse = true;
			}

			List<Vector2D> shape = new List<Vector2D>();

			//top-left corner
			shape.AddRange(GetCornerPoints(pStart, currentbevelwidth, currentbevelwidth, !reverse));

			//top-right corner
			shape.AddRange(GetCornerPoints(new Vector2D(pEnd.x, pStart.y), -currentbevelwidth, currentbevelwidth, reverse));

			//bottom-right corner
			shape.AddRange(GetCornerPoints(pEnd, -currentbevelwidth, -currentbevelwidth, !reverse));

			//bottom-left corner
			shape.AddRange(GetCornerPoints(new Vector2D(pStart.x, pEnd.y), currentbevelwidth, -currentbevelwidth, reverse));

			//closing point
			shape.Add(shape[0]);

			return shape.ToArray();
		}

		private Vector2D[] GetCornerPoints(Vector2D startPoint, int bevel_width, int bevel_height, bool reverse) 
		{
			Vector2D[] points;
			Vector2D center = (bevelwidth > 0 ? new Vector2D(startPoint.x + bevel_width, startPoint.y + bevel_height) : startPoint);
			double curAngle = Angle2D.PI;

			int steps = subdivisions + 2;
			points = new Vector2D[steps];
			double stepAngle = Angle2D.PIHALF / (subdivisions + 1);

			for(int i = 0; i < steps; i++) 
			{
				points[i] = new Vector2D(center.x + Math.Sin(curAngle) * bevel_width, center.y + Math.Cos(curAngle) * bevel_height);
				curAngle += stepAngle;
			}

			if(reverse) Array.Reverse(points);
			return points;
		}

		protected virtual string GetHintText()
		{
			List<string> result = new List<string>();
			if(bevelwidth != 0) result.Add("BVL: " + bevelwidth);
			if(subdivisions != 0) result.Add("SUB: " + subdivisions);

			return string.Join("; ", result.ToArray());
		}

		// Update top-left and bottom-right points, which define drawing shape
		private void UpdateReferencePoints(DrawnVertex p1, DrawnVertex p2) 
		{
			if(!p1.pos.IsFinite() || !p2.pos.IsFinite()) return;

			if (radialdrawing)
			{
				Vector2D delta = p2.pos - p1.pos;
				p1.pos -= delta;
			}

			// Make sure start always stays at left and up from the end
			if (p1.pos.x < p2.pos.x) 
			{
				start.x = p1.pos.x;
				end.x = p2.pos.x;
			} 
			else 
			{
				start.x = p2.pos.x;
				end.x = p1.pos.x;
			}

			if(p1.pos.y < p2.pos.y) 
			{
				start.y = p1.pos.y;
				end.y = p2.pos.y;
			} 
			else 
			{
				start.y = p2.pos.y;
				end.y = p1.pos.y;
			}

			// Update size
			width = (int)Math.Round(end.x - start.x);
			height = (int)Math.Round(end.y - start.y);
		}

		// This draws a point at a specific location
		override public bool DrawPointAt(Vector2D pos, bool stitch, bool stitchline) 
		{
			if(pos.x < General.Map.Config.LeftBoundary || pos.x > General.Map.Config.RightBoundary ||
				pos.y > General.Map.Config.TopBoundary || pos.y < General.Map.Config.BottomBoundary)
				return false;

			DrawnVertex newpoint = new DrawnVertex();
			newpoint.pos = pos.GetRotated(-General.Map.Grid.GridRotate);
			newpoint.stitch = true; //stitch
			newpoint.stitchline = stitchline;
			points.Add(newpoint);

			if (points.Count == 1) //add point and labels
			{ 
				labels.AddRange(new[] { new LineLengthLabel(false, true), new LineLengthLabel(false, true), new LineLengthLabel(false, true), new LineLengthLabel(false, true) });
				hintlabel = new HintLabel(General.Colors.InfoLine);
				Update();
			} 
			else if(points[0].pos == points[1].pos) //nothing is drawn
			{ 
				points = new List<DrawnVertex>();
				FinishDraw();
			} 
			else 
			{
				//create vertices for final shape. 
				UpdateReferencePoints(points[0], newpoint);
				points = new List<DrawnVertex>(); //clear points
				Vector2D[] shape = GetShape(start, end);

				// Rotate the shape to fit the grid rotation
				for (int i = 0; i < shape.Length; i++)
					shape[i] = shape[i].GetRotated(General.Map.Grid.GridRotate);

				// We don't want base.DrawPointAt to call Update() here, because it will mess labels[] 
				// and trigger shape.Count number of display redraws...
				blockupdate = true;
				foreach(Vector2D t in shape) base.DrawPointAt(t, true, true);
				blockupdate = false;

				FinishDraw();
			}
			return true;
		}

		override public void RemovePoint() 
		{
			if(points.Count > 0) points.RemoveAt(points.Count - 1);
			if(labels.Count > 0) labels = new List<LineLengthLabel>();
			Update();
		}

		public override void RemoveFirstPoint()
		{
			RemovePoint();
		}

		#endregion

		#region ================== Events
		
		override public void OnAccept() 
		{
			Cursor.Current = Cursors.AppStarting;
			General.Settings.FindDefaultDrawSettings();

			// When we have a rectangle or a line
			if(points.Count > minpointscount || points.Count == 2) 
			{
				// Make undo for the draw
				General.Map.UndoRedo.CreateUndo(undoname);

				// Make an analysis and show info
				string[] adjectives = new[]
				{ "beautiful", "lovely", "romantic", "stylish", "cheerful", "comical",
				  "awesome", "accurate", "adorable", "adventurous", "attractive", "cute",
				  "elegant", "glamorous", "gorgeous", "handsome", "magnificent", "unusual",
				  "outstanding", "mysterious", "amusing", "charming", "fantastic", "jolly" };
				string word = adjectives[new Random().Next(adjectives.Length - 1)];
				string a = ((word[0] == 'a') || (word[0] == 'e') || (word[0] == 'o') || (word[0] == 'u')) ? "an " : "a ";

				General.Interface.DisplayStatus(StatusType.Action, "Created " + a + word + " " + shapename + ".");

				// Make the drawing
				if(Tools.DrawLines(points, true, BuilderPlug.Me.AutoAlignTextureOffsetsOnCreate))
				{
					// Snap to map format accuracy
					General.Map.Map.SnapAllToAccuracy();

					// Clear selection
					General.Map.Map.ClearAllSelected();

					// Update cached values
					General.Map.Map.Update();

					//mxd. Outer sectors may require some splittin...
					if(General.Settings.SplitJoinedSectors) Tools.SplitOuterSectors(General.Map.Map.GetMarkedLinedefs(true));

					// Edit new sectors?
					List<Sector> newsectors = General.Map.Map.GetMarkedSectors(true);
					if(BuilderPlug.Me.EditNewSector && (newsectors.Count > 0))
						General.Interface.ShowEditSectors(newsectors);

					// Update the used textures
					General.Map.Data.UpdateUsedTextures();

					//mxd
					General.Map.Renderer2D.UpdateExtraFloorFlag();

					// Map is changed
					General.Map.IsChanged = true;
				}
				else
				{
					// Drawing failed
					// NOTE: I have to call this twice, because the first time only cancels this volatile mode
					General.Map.UndoRedo.WithdrawUndo();
					General.Map.UndoRedo.WithdrawUndo();
				}
			}

			// Done
			Cursor.Current = Cursors.Default;

			if(continuousdrawing)
			{
				// Reset settings
				points.Clear();
				labels.Clear();
				drawingautoclosed = false;

				// Redraw display
				General.Interface.RedrawDisplay();
			}
			else
			{
				// Return to original mode
				General.Editing.ChangeMode(General.Editing.PreviousStableMode.Name);
			}
		}

		public override void OnDisengage()
		{
			if(hintlabel != null) hintlabel.Dispose();
			base.OnDisengage();
		}

		public override void OnHelp() 
		{
			General.ShowHelp("/gzdb/features/classic_modes/mode_drawrect.html");
		}

		private void OptionsPanelOnValueChanged(object sender, EventArgs eventArgs) 
		{
			bevelwidth = panel.BevelWidth;
			subdivisions = panel.Subdivisions;
			Update();
		}
		
		protected void OnRadialDrawingChanged(object value, EventArgs e)
		{
			radialdrawing = (bool)value;
		}

		#endregion

		#region ================== Actions

		[BeginAction("increasesubdivlevel")]
		protected virtual void IncreaseSubdivLevel() 
		{
			if(subdivisions < maxsubdivisions) 
			{
				subdivisions++;
				panel.Subdivisions = subdivisions;
				Update();
			}
		}

		[BeginAction("decreasesubdivlevel")]
		protected virtual void DecreaseSubdivLevel() 
		{
			if(subdivisions > minsubdivisions) 
			{
				subdivisions--;
				panel.Subdivisions = subdivisions;
				Update();
			}
		}

		[BeginAction("increasebevel")]
		protected virtual void IncreaseBevel() 
		{
			if(points.Count < 2 || currentbevelwidth == bevelwidth || bevelwidth < 0) 
			{
				bevelwidth = Math.Min(bevelwidth + General.Map.Grid.GridSize, panel.MaxBevelWidth);
				panel.BevelWidth = bevelwidth;
				Update();
			}
		}

		[BeginAction("decreasebevel")]
		protected virtual void DecreaseBevel() 
		{
			if(currentbevelwidth == bevelwidth || bevelwidth > 0) 
			{
				bevelwidth = Math.Max(bevelwidth - General.Map.Grid.GridSize, panel.MinBevelWidth);
				panel.BevelWidth = bevelwidth;
				Update();
			}
		}

		#endregion

	}
}
