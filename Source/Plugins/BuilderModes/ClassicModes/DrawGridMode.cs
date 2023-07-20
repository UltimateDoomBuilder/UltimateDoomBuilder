#region ================== Namespaces

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Actions;
using CodeImp.DoomBuilder.Controls;
using CodeImp.DoomBuilder.Editing;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Windows;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	[EditMode(DisplayName = "Draw Grid Mode",
			  SwitchAction = "drawgridmode",
			  ButtonImage = "DrawGridMode.png", //mxd
			  ButtonOrder = int.MinValue + 5, //mxd
			  ButtonGroup = "000_drawing", //mxd
			  AllowCopyPaste = false,
			  Volatile = true,
			  Optional = false)]

	public class DrawGridMode : DrawGeometryMode
	{
		#region ================== Enums

		public enum GridLockMode
		{
			NONE,
			HORIZONTAL,
			VERTICAL,
			BOTH,
		}

		#endregion

		#region ================== Variables

		// Settings
		private int horizontalslices;
		private int verticalslices;
		private bool triangulate;
		private bool relativeinterpolation;
		private GridLockMode gridlockmode;
		private InterpolationTools.Mode horizontalinterpolation;
		private InterpolationTools.Mode verticalinterpolation;

		// Drawing
		private readonly List<DrawnVertex[]> gridpoints;
		private HintLabel hintlabel;
		
		private int width;
		private int height;
		private int slicesH;
		private int slicesV;
		private Vector2D start;
		private Vector2D end;

		// Interface
		private DrawGridOptionsPanel panel;
		private Docker docker;

		#endregion

		#region ================== Constructor

		public DrawGridMode() 
		{
			snaptogrid = true;
			usefourcardinaldirections = true;
			autoclosedrawing = false;
			gridpoints = new List<DrawnVertex[]>();
		}

		#endregion

		#region ================== Events

		override public void OnAccept() 
		{
			Cursor.Current = Cursors.AppStarting;
			General.Settings.FindDefaultDrawSettings();

			// When we have a shape...
			if(gridpoints.Count > 0) 
			{
				// Make undo for the draw
				General.Map.UndoRedo.CreateUndo("Grid draw");

				// Make an analysis and show info
				string[] adjectives = new[]
				{ "beautiful", "lovely", "romantic", "stylish", "cheerful", "comical",
				  "awesome", "accurate", "adorable", "adventurous", "attractive", "cute",
				  "elegant", "glamorous", "gorgeous", "handsome", "magnificent", "unusual",
				  "outstanding", "mysterious", "amusing", "charming", "fantastic", "jolly" };
				string word = adjectives[new Random().Next(adjectives.Length - 1)];
				string a = ((word[0] == 'a') || (word[0] == 'e') || (word[0] == 'o') || (word[0] == 'u')) ? "an " : "a ";

				General.Interface.DisplayStatus(StatusType.Action, "Created " + a + word + " grid.");

				List<Sector> newsectors = new List<Sector>();
				foreach(DrawnVertex[] shape in gridpoints) 
				{
					if(Tools.DrawLines(shape, true, BuilderPlug.Me.AutoAlignTextureOffsetsOnCreate))
					{
						// Update cached values after each step...
						General.Map.Map.Update();

						newsectors.AddRange(General.Map.Map.GetMarkedSectors(true));

						// Snap to map format accuracy
						General.Map.Map.SnapAllToAccuracy();

						// Clear selection
						General.Map.Map.ClearAllSelected();

						//mxd. Outer sectors may require some splittin...
						if(General.Settings.SplitJoinedSectors) Tools.SplitOuterSectors(General.Map.Map.GetMarkedLinedefs(true));

						// Edit new sectors?
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
			}

			// Done
			Cursor.Current = Cursors.Default;

			if(continuousdrawing)
			{
				// Reset settings
				points.Clear();
				gridpoints.Clear();
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

		private void OptionsPanelOnValueChanged(object sender, EventArgs eventArgs) 
		{
			triangulate = panel.Triangulate;
			horizontalslices = panel.HorizontalSlices + 1;
			verticalslices = panel.VerticalSlices + 1;
			horizontalinterpolation = panel.HorizontalInterpolationMode;
			verticalinterpolation = panel.VerticalInterpolationMode;
			Update();
		}

		private void OptionsPanelOnGridLockChanged(object sender, EventArgs eventArgs) 
		{
			gridlockmode = panel.GridLockMode;
			General.Hints.ShowHints(this.GetType(), ((gridlockmode != GridLockMode.NONE) ? "gridlockhelp" : "general"));
			Update();
		}

		public override void OnHelp() 
		{
			General.ShowHelp("/gzdb/features/classic_modes/mode_drawgrid.html");
		}

		#endregion

		#region ================== Methods

		override protected void Update() 
		{
			PixelColor stitchcolor = General.Colors.Highlight;
			PixelColor losecolor = General.Colors.Selection;

			// We WANT snaptogrid and DON'T WANT snaptonearest when lock to grid is enabled
			snaptocardinaldirection = General.Interface.ShiftState && General.Interface.AltState; //mxd
			snaptogrid = (snaptocardinaldirection || gridlockmode != GridLockMode.NONE || (General.Interface.ShiftState ^ General.Interface.SnapToGrid));
			snaptonearest = (gridlockmode == GridLockMode.NONE && (General.Interface.CtrlState ^ General.Interface.AutoMerge));

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
					List<Vector2D[]> shapes = GetShapes(start, end);

					Vector2D startrotated = start.GetRotated(General.Map.Grid.GridRotate);
					Vector2D endrotated = end.GetRotated(General.Map.Grid.GridRotate);

					// Rotate the shape to fit the grid rotation
					foreach (Vector2D[] shape in shapes)
					{
						for(int i=0; i < shape.Length; i++)
							shape[i] = shape[i].GetRotated(General.Map.Grid.GridRotate);
					}

					// Render guidelines
					if(showguidelines)
						RenderGuidelines(startrotated, endrotated, General.Colors.Guideline.WithAlpha(80), -General.Map.Grid.GridRotate);

					//render shape
					foreach(Vector2D[] shape in shapes) 
					{
						for(int i = 1; i < shape.Length; i++)
						renderer.RenderLine(shape[i - 1], shape[i], LINE_THICKNESS, color, true);
					}

					//vertices
					foreach(Vector2D[] shape in shapes) 
					{
						for(int i = 0; i < shape.Length; i++)
							renderer.RenderRectangleFilled(new RectangleF((float)(shape[i].x - vsize), (float)(shape[i].y - vsize), vsize * 2.0f, vsize * 2.0f), color, true);
					}

					//and labels
					if(width == 0 || height == 0)
					{
						// Render label for line
						labels[0].Move(startrotated, endrotated);
						renderer.RenderText(labels[0].TextLabel);
					}
					else
					{
						// Render labels for grid
						Vector2D[] labelCoords = { startrotated, new Vector2D(end.x, start.y).GetRotated(General.Map.Grid.GridRotate), endrotated, new Vector2D(start.x, end.y).GetRotated(General.Map.Grid.GridRotate), startrotated };

						for (int i = 1; i < 5; i++)
						{
							labels[i - 1].Move(labelCoords[i], labelCoords[i - 1]);
							renderer.RenderText(labels[i - 1].TextLabel);
						}
					}

					//render hint
					if(horizontalslices > 1 || verticalslices > 1) 
					{
						string text = "H: " + (slicesH - 1) + "; V: " + (slicesV - 1);
						if(Math.Abs(width) > text.Length * vsize && Math.Abs(height) > 16 * vsize)
						{
							hintlabel.Text = text;
							hintlabel.Move(startrotated, endrotated);
							renderer.RenderText(hintlabel.TextLabel);
						}
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

			if(points.Count == 1) 
			{ 
				// Add labels
				labels.AddRange(new[] { new LineLengthLabel(false, true), new LineLengthLabel(false, true), new LineLengthLabel(false, true), new LineLengthLabel(false, true) });
				hintlabel = new HintLabel(General.Colors.InfoLine);
				Update();
			} 
			else if(points[0].pos == points[1].pos) 
			{
				// Nothing is drawn
				FinishDraw();
			} 
			else 
			{
				// Create vertices for final shape.
				UpdateReferencePoints(points[0], newpoint);
				List<Vector2D[]> shapes = GetShapes(start, end);

				foreach(Vector2D[] shape in shapes) 
				{
					DrawnVertex[] verts = new DrawnVertex[shape.Length];
					for(int i = 0; i < shape.Length; i++)
					{
						newpoint = new DrawnVertex {
							pos = shape[i].GetRotated(General.Map.Grid.GridRotate), // Take grid rotation into account
							stitch = true,
							stitchline = stitchline
						};
						verts[i] = newpoint;
					}

					gridpoints.Add(verts);
				}

				FinishDraw();
			}
			return true;
		}

		private List<Vector2D[]> GetShapes(Vector2D s, Vector2D e) 
		{
			// No shape
			if(s == e) return new List<Vector2D[]>();

			// Setup slices. Need to do some extra mathy stuff the make sure the rotated grid is taken into account
			switch(gridlockmode)
			{
				case GridLockMode.NONE:
					slicesH = horizontalslices;
					slicesV = verticalslices;
					break;

				case GridLockMode.HORIZONTAL:
					slicesH = Convert.ToInt32(Math.Ceiling(Math.Abs(width) / (double)General.Map.Grid.GridSize));
					slicesV = verticalslices;
					break;

				case GridLockMode.VERTICAL:
					slicesH = horizontalslices;
					slicesV = Convert.ToInt32(Math.Ceiling(Math.Abs(height) / (double)General.Map.Grid.GridSize));
					break;

				case GridLockMode.BOTH:
					slicesH = Convert.ToInt32(Math.Ceiling(Math.Abs(width) / (double)General.Map.Grid.GridSize));
					slicesV = Convert.ToInt32(Math.Ceiling(Math.Abs(height) / (double)General.Map.Grid.GridSize));
					break;
			}

			// Create a segmented line
			List<Vector2D[]> shapes;
			if(width == 0 || height == 0)
			{
				if(slicesH > 0 && width > 0)
				{
					shapes = new List<Vector2D[]>();
					int step = width / slicesH;
					for(int w = 0; w < slicesH; w++)
					{
						shapes.Add(new[] { new Vector2D(s.x + step * w, s.y), 
										   new Vector2D(s.x + step * w + step, s.y) });
					}
					return shapes;
				}

				if(slicesV > 0 && height > 0)
				{
					shapes = new List<Vector2D[]>();
					int step = height / slicesV;
					for(int h = 0; h < slicesV; h++)
					{
						shapes.Add(new[] { new Vector2D(s.x, s.y + step * h), 
										   new Vector2D(s.x, s.y + step * h + step) });
					}
					return shapes;
				}

				// Create a line
				return new List<Vector2D[]> {new[] {s, e}};
			}

			// Create grid shape
			List<Vector2D> rect = new List<Vector2D> { s, new Vector2D(s.x, e.y), e, new Vector2D(e.x, s.y), s };
			if(slicesH == 1 && slicesV == 1) 
			{
				if(triangulate) rect.AddRange(new[] { s, e });
				return new List<Vector2D[]> { rect.ToArray() };
			}

			// Create blocks
			shapes = new List<Vector2D[]> { rect.ToArray() };
			RectangleF[,] blocks = new RectangleF[slicesH, slicesV];
			for(int w = 0; w < slicesH; w++) 
			{
				for(int h = 0; h < slicesV; h++)
				{
					double left = (InterpolationTools.Interpolate(s.x, e.x, (double)w / slicesH, horizontalinterpolation));
					double top = (InterpolationTools.Interpolate(s.y, e.y, (double)h / slicesV, verticalinterpolation));
					double right = (InterpolationTools.Interpolate(s.x, e.x, (w + 1.0f) / slicesH, horizontalinterpolation));
					double bottom = (InterpolationTools.Interpolate(s.y, e.y, (h + 1.0f) / slicesV, verticalinterpolation));
					blocks[w, h] = RectangleF.FromLTRB((float)left, (float)top, (float)right, (float)bottom);
				}
			}

			// Add subdivisions
			if(slicesH > 1) 
			{
				for(int w = 1; w < slicesH; w++) 
				{
					float px = blocks[w, 0].X;
					shapes.Add(new[] { new Vector2D(px, s.y), new Vector2D(px, e.y) });
				}
			}
			if(slicesV > 1) 
			{
				for(int h = 1; h < slicesV; h++) 
				{
					float py = blocks[0, h].Y;
					shapes.Add(new[] { new Vector2D(s.x, py), new Vector2D(e.x, py) });
				}
			}

			// Triangulate?
			if(triangulate) 
			{
				bool startflip = ((int)Math.Round(((s.x + e.y) / General.Map.Grid.GridSizeF) % 2) == 0);
				bool flip = startflip;

				for(int w = 0; w < slicesH; w++) 
				{
					for(int h = slicesV - 1; h > -1; h--) 
					{
						if(flip)
							shapes.Add(new[] { new Vector2D(blocks[w, h].X, blocks[w, h].Y), new Vector2D(blocks[w, h].Right, blocks[w, h].Bottom) });
						else
							shapes.Add(new[] { new Vector2D(blocks[w, h].Right, blocks[w, h].Y), new Vector2D(blocks[w, h].X, blocks[w, h].Bottom) });

						flip = !flip;
					}

					startflip = !startflip;
					flip = startflip;
				}
			}

			return shapes;
		}

		// Update bottom-left and top-right points, which define drawing shape
		private void UpdateReferencePoints(DrawnVertex p1, DrawnVertex p2) 
		{
			if(!p1.pos.IsFinite() || !p2.pos.IsFinite()) return;

			// If relative interpolation is enabled the interpolation will use the point where the drawing
			// started as the origin, not always the top left corner
			if (relativeinterpolation)
			{
				start.x = p1.pos.x;
				start.y = p1.pos.y;
				end.x = p2.pos.x;
				end.y = p2.pos.y;
			}
			else
			{
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

				if (p1.pos.y < p2.pos.y)
				{
					start.y = p1.pos.y;
					end.y = p2.pos.y;
				}
				else
				{
					start.y = p2.pos.y;
					end.y = p1.pos.y;
				}
			}


			width = (int)(end.x - start.x);
			height = (int)(end.y - start.y);
		}

		#endregion

		#region ================== Settings panel

		protected override void SetupInterface()
		{
			// Load stored settings
			triangulate = General.Settings.ReadPluginSetting("drawgridmode.triangulate", false);
			gridlockmode = (GridLockMode)General.Settings.ReadPluginSetting("drawgridmode.gridlockmode", 0);
			horizontalslices = Math.Max(General.Settings.ReadPluginSetting("drawgridmode.horizontalslices", 3), 1);
			verticalslices = Math.Max(General.Settings.ReadPluginSetting("drawgridmode.verticalslices", 3), 1);
			relativeinterpolation = General.Settings.ReadPluginSetting("drawgridmode.relativeinterpolation", true);
			horizontalinterpolation = (InterpolationTools.Mode)General.Settings.ReadPluginSetting("drawgridmode.horizontalinterpolation", 0);
			verticalinterpolation = (InterpolationTools.Mode)General.Settings.ReadPluginSetting("drawgridmode.verticalinterpolation", 0);
			
			// Create and setup settings panel
			panel = new DrawGridOptionsPanel();
			panel.MaxHorizontalSlices = (int)General.Map.FormatInterface.MaxCoordinate;
			panel.MaxVerticalSlices = (int)General.Map.FormatInterface.MaxCoordinate;
			panel.Triangulate = triangulate;
			panel.GridLockMode = gridlockmode;
			panel.HorizontalSlices = horizontalslices - 1;
			panel.VerticalSlices = verticalslices - 1;
			panel.HorizontalInterpolationMode = horizontalinterpolation;
			panel.VerticalInterpolationMode = verticalinterpolation;

			panel.OnValueChanged += OptionsPanelOnValueChanged;
			panel.OnGridLockModeChanged += OptionsPanelOnGridLockChanged;
			panel.OnContinuousDrawingChanged += OnContinuousDrawingChanged;
			panel.OnShowGuidelinesChanged += OnShowGuidelinesChanged;
			panel.OnRelativeInterpolationChanged += OnRelativeInterpolationChanged;

			// Needs to be set after adding the OnContinuousDrawingChanged event...
			panel.ContinuousDrawing = General.Settings.ReadPluginSetting("drawgridmode.continuousdrawing", false);
			panel.ShowGuidelines = General.Settings.ReadPluginSetting("drawgridmode.showguidelines", false);
			panel.RelativeInterpolation = relativeinterpolation;
		}

		protected override void AddInterface()
		{
			// Add docker
			docker = new Docker("drawgrid", "Draw Grid", panel);
			General.Interface.AddDocker(docker, true);
			General.Interface.SelectDocker(docker);
		}

		protected override void RemoveInterface()
		{
			// Store settings
			General.Settings.WritePluginSetting("drawgridmode.triangulate", triangulate);
			General.Settings.WritePluginSetting("drawgridmode.gridlockmode", (int)gridlockmode);
			General.Settings.WritePluginSetting("drawgridmode.horizontalslices", horizontalslices);
			General.Settings.WritePluginSetting("drawgridmode.verticalslices", verticalslices);
			General.Settings.WritePluginSetting("drawgridmode.relativeinterpolation", relativeinterpolation);
			General.Settings.WritePluginSetting("drawgridmode.horizontalinterpolation", (int)horizontalinterpolation);
			General.Settings.WritePluginSetting("drawgridmode.verticalinterpolation", (int)verticalinterpolation);
			General.Settings.WritePluginSetting("drawgridmode.continuousdrawing", panel.ContinuousDrawing);
			General.Settings.WritePluginSetting("drawgridmode.showguidelines", panel.ShowGuidelines);

			// Remove docker
			General.Interface.RemoveDocker(docker);
			panel.Dispose();
			panel = null;
		}

		protected void OnRelativeInterpolationChanged(object value, EventArgs e)
		{
			relativeinterpolation = (bool)value;
			Update();
		}

		#endregion

		#region ================== Actions

		[BeginAction("increasebevel")]
		protected void IncreaseBevel()
		{
			if((gridlockmode == GridLockMode.NONE || gridlockmode == GridLockMode.VERTICAL) 
				&& (points.Count < 2 || horizontalslices < width - 2) 
				&& horizontalslices - 1 < panel.MaxHorizontalSlices) 
			{
				horizontalslices++;
				panel.HorizontalSlices = horizontalslices - 1;
				Update();
			}
		}

		[BeginAction("decreasebevel")]
		protected void DecreaseBevel()
		{
			if((gridlockmode == GridLockMode.NONE || gridlockmode == GridLockMode.VERTICAL) && horizontalslices > 1) 
			{
				horizontalslices--;
				panel.HorizontalSlices = horizontalslices - 1;
				Update();
			}
		}

		[BeginAction("increasesubdivlevel")]
		protected void IncreaseSubdivLevel()
		{
			if((gridlockmode == GridLockMode.NONE || gridlockmode == GridLockMode.HORIZONTAL) 
				&& (points.Count < 2 || verticalslices < height - 2) 
				&& verticalslices - 1 < panel.MaxVerticalSlices) 
			{
				verticalslices++;
				panel.VerticalSlices = verticalslices - 1;
				Update();
			}
		}

		[BeginAction("decreasesubdivlevel")]
		protected void DecreaseSubdivLevel()
		{
			if((gridlockmode == GridLockMode.NONE || gridlockmode == GridLockMode.HORIZONTAL) && verticalslices > 1) 
			{
				verticalslices--;
				panel.VerticalSlices = verticalslices - 1;
				Update();
			}
		}

		#endregion

	}
}
