#region ================== Namespaces

using System;
using System.Collections.Generic;
using CodeImp.DoomBuilder.Actions;
using CodeImp.DoomBuilder.Editing;
using CodeImp.DoomBuilder.Geometry;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	[EditMode(DisplayName = "Draw Ellipse Mode",
			  SwitchAction = "drawellipsemode",
			  ButtonImage = "DrawEllipseMode.png", //mxd	
			  ButtonOrder = int.MinValue + 4, //mxd
			  ButtonGroup = "000_drawing", //mxd
			  AllowCopyPaste = false,
			  Volatile = true,
			  Optional = false)]
	
	public class DrawEllipseMode : DrawRectangleMode
	{
		#region ================== Variables
		
		// Interface
		private DrawEllipseOptionsPanel panel;

		// Drawing
		private double angle; // in radians

		#endregion

		#region ================== Constructor

		public DrawEllipseMode()
		{
			autoclosedrawing = false;
		}

		#endregion

		#region ================== Settings panel

		override protected void SetupInterface() 
		{
			maxsubdivisions = 512;
			minsubdivisions = 3;
			minpointscount = 3;
			alwaysrendershapehints = true;

			// Load stored settings
			subdivisions = General.Clamp(General.Settings.ReadPluginSetting("drawellipsemode.subdivisions", 8), minsubdivisions, maxsubdivisions);
			bevelwidth = General.Settings.ReadPluginSetting("drawellipsemode.bevelwidth", 0);
			int angledeg = General.Settings.ReadPluginSetting("drawellipsemode.angle", 0);
			angle = Angle2D.DegToRad(angledeg);
			currentbevelwidth = bevelwidth;

			//Add options docker
			panel = new DrawEllipseOptionsPanel();
			panel.MaxSubdivisions = maxsubdivisions;
			panel.MinSubdivisions = minsubdivisions;
			panel.MinSpikiness = (int)General.Map.FormatInterface.MinCoordinate;
			panel.MaxSpikiness = (int)General.Map.FormatInterface.MaxCoordinate;
			panel.Spikiness = bevelwidth;
			panel.Angle = angledeg;
			panel.Subdivisions = subdivisions;
			panel.OnValueChanged += OptionsPanelOnValueChanged;
			panel.OnContinuousDrawingChanged += OnContinuousDrawingChanged;
			panel.OnShowGuidelinesChanged += OnShowGuidelinesChanged;
			panel.OnRadialDrawingChanged += OnRadialDrawingChanged;

			// Needs to be set after adding the OnContinuousDrawingChanged event...
			panel.ContinuousDrawing = General.Settings.ReadPluginSetting("drawellipsemode.continuousdrawing", false);
			panel.ShowGuidelines = General.Settings.ReadPluginSetting("drawellipsemode.showguidelines", false);
			panel.RadialDrawing = General.Settings.ReadPluginSetting("drawellipsemode.radialdrawing", false);
		}

		override protected void AddInterface() 
		{
			panel.Register();
		}

		override protected void RemoveInterface() 
		{
			// Store settings
			General.Settings.WritePluginSetting("drawellipsemode.subdivisions", subdivisions);
			General.Settings.WritePluginSetting("drawellipsemode.bevelwidth", bevelwidth);
			General.Settings.WritePluginSetting("drawellipsemode.angle", panel.Angle);
			General.Settings.WritePluginSetting("drawellipsemode.continuousdrawing", panel.ContinuousDrawing);
			General.Settings.WritePluginSetting("drawellipsemode.showguidelines", panel.ShowGuidelines);
			General.Settings.WritePluginSetting("drawellipsemode.radialdrawing", panel.RadialDrawing);
			

			// Remove the buttons
			panel.Unregister();
		}

		#endregion

		#region ================== Methods

		override protected Vector2D[] GetShape(Vector2D pStart, Vector2D pEnd) 
		{
			// No shape
			if(pEnd.x == pStart.x && pEnd.y == pStart.y) return new Vector2D[0];

			// Line
			if(pEnd.x == pStart.x || pEnd.y == pStart.y) return new[] { pStart, pEnd };

			// Got shape
			if(subdivisions < 6)
				currentbevelwidth = 0; // Works strange otherwise
			else if(bevelwidth < 0) 
				currentbevelwidth = -Math.Min(Math.Abs(bevelwidth), Math.Min(width, height) / 2) + 1;
			else 
				currentbevelwidth = bevelwidth;

			Vector2D[] shape = new Vector2D[subdivisions + 1];

			bool doBevel = false;
			double hw = width / 2.0;
			double hh = height / 2.0;

			Vector2D center = new Vector2D(pStart.x + hw, pStart.y + hh);
			double curAngle = angle;
			double angleStep = -Angle2D.PI / subdivisions * 2;

			for(int i = 0; i < subdivisions; i++) 
			{
				double px, py;
				if(doBevel) 
				{
					px = (center.x - Math.Sin(curAngle) * (hw + currentbevelwidth));
					py = (center.y - Math.Cos(curAngle) * (hh + currentbevelwidth));
				} 
				else 
				{
					px = (center.x - Math.Sin(curAngle) * hw);
					py = (center.y - Math.Cos(curAngle) * hh);
				}
				doBevel = !doBevel;
				shape[i] = new Vector2D(px, py);
				curAngle += angleStep;
			}

			// Add final point
			shape[subdivisions] = shape[0];

			// Now fit it inside the bounding box
			double minx = double.MaxValue;
			double miny = double.MaxValue;
			double maxx = double.MinValue;
			double maxy = double.MinValue;

			// Calculate shape extents
			foreach(Vector2D v in shape)
			{
				if(v.x < minx) minx = v.x;
				if(v.x > maxx) maxx = v.x;
				if(v.y < miny) miny = v.y;
				if(v.y > maxy) maxy = v.y;
			}

			// Calculate scalers
			double scalerx = 1.0;
			double scalery = 1.0;
			
			if(minx != pStart.x || maxx != pEnd.x)
				scalerx = (pEnd.x - pStart.x) / (maxx - minx);
			if(miny != pStart.y)
				scalery = (pEnd.y - pStart.y) / (maxy - miny);

			// Apply scalers
			if(scalerx != 1.0f || scalery != 1.0f)
			{
				for(int i = 0; i < shape.Length; i++)
				{
					shape[i].x = ((center.x - shape[i].x) * scalerx + center.x);
					shape[i].y = ((center.y - shape[i].y) * scalery + center.y);
				}
			}

			// Calculate shape extents again...
			minx = double.MaxValue;
			miny = double.MaxValue;
			foreach(Vector2D v in shape)
			{
				if(v.x < minx) minx = v.x;
				if(v.y < miny) miny = v.y;
			}

			// Calculate shape offset...
			Vector2D offset = new Vector2D();
			if(minx != pStart.x) offset.x = pStart.x - minx;
			if(miny != pStart.y) offset.y = pStart.y - miny;

			// Apply offset...
			if(offset.x != 0.0 || offset.y != 0.0)
				for(int i = 0; i < shape.Length; i++) shape[i] += offset;

			// Done
			return shape;
		}

		protected override string GetHintText()
		{
			List<string> result = new List<string>();
			if(bevelwidth != 0) result.Add("BVL: " + bevelwidth);
			if(subdivisions != 0) result.Add("VERTS: " + subdivisions);
			if(panel.Angle != 0) result.Add("ANGLE: " + panel.Angle);
			
			return string.Join("; ", result.ToArray());
		}

		#endregion

		#region ================== Events

		public override void OnAccept()
		{
			switch(points.Count - 1) // Last point matches the first one
			{
				case 3:  undoname = "Triangle draw"; shapename = "triangle"; break;
				case 4:  undoname = "Rhombus draw"; shapename = "rhombus"; break;
				case 5:  undoname = "Pentagon draw"; shapename = "pentagon"; break;
				case 6:  undoname = "Hexagon draw"; shapename = "hexagon"; break;
				case 7:  undoname = "Heptagon draw"; shapename = "heptagon"; break;
				case 8:  undoname = "Octagon draw"; shapename = "octagon"; break;
				case 9:  undoname = "Enneagon draw"; shapename = "enneagon"; break;
				case 10: undoname = "Decagon draw"; shapename = "decagon"; break;
				case 11: undoname = "Hendecagon draw"; shapename = "hendecagon"; break;
				case 12: undoname = "Dodecagon draw"; shapename = "dodecagon"; break;
				case 13: undoname = "Tridecagon draw"; shapename = "tridecagon"; break;
				case 14: undoname = "Tetradecagon draw"; shapename = "tetradecagon"; break;
				case 15: undoname = "Pentadecagon draw"; shapename = "pentadecagon"; break;
				case 16: undoname = "Hexadecagon draw"; shapename = "hexadecagon"; break;
				case 17: undoname = "Heptadecagon draw"; shapename = "heptadecagon"; break;
				case 18: undoname = "Octadecagon draw"; shapename = "octadecagon"; break;
				case 19: undoname = "Enneadecagon draw"; shapename = "enneadecagon"; break;
				case 20: undoname = "Icosagon draw"; shapename = "icosagon"; break;
				default: undoname = "Ellipse draw"; shapename = "ellipse"; break;
			}
			
			base.OnAccept();
		}

		private void OptionsPanelOnValueChanged(object sender, EventArgs eventArgs) 
		{
			bevelwidth = panel.Spikiness;
			subdivisions = Math.Min(maxsubdivisions, panel.Subdivisions);
			angle = Angle2D.DegToRad(panel.Angle);
			Update();
		}

		public override void OnHelp() 
		{
			General.ShowHelp("/gzdb/features/classic_modes/mode_drawellipse.html");
		}

		#endregion

		#region ================== Actions

		override protected void IncreaseSubdivLevel() 
		{
			if(maxsubdivisions - subdivisions > 1) 
			{
				subdivisions += (subdivisions % 2 != 0 ? 1 : 2);
				panel.Subdivisions = subdivisions;
				Update();
			}
		}

		override protected void DecreaseSubdivLevel() 
		{
			if(subdivisions - minsubdivisions > 1) 
			{
				subdivisions -= (subdivisions % 2 != 0 ? 1 : 2);
				panel.Subdivisions = subdivisions;
				Update();
			}
		}

		protected override void IncreaseBevel() 
		{
			if(points.Count < 2 || currentbevelwidth == bevelwidth || bevelwidth < 0) 
			{
				bevelwidth = Math.Min(bevelwidth + General.Map.Grid.GridSize, panel.MaxSpikiness);
				panel.Spikiness = bevelwidth;
				Update();
			}
		}

		protected override void DecreaseBevel() 
		{
			if(bevelwidth > 0 || currentbevelwidth <= bevelwidth + 1) 
			{
				bevelwidth = Math.Max(bevelwidth - General.Map.Grid.GridSize, panel.MinSpikiness);
				panel.Spikiness = bevelwidth;
				Update();
			}
		}

		[BeginAction("rotateclockwise")]
		private void IncreaseAngle()
		{
			panel.Angle = General.ClampAngle(panel.Angle + 5);
			angle = Angle2D.DegToRad(panel.Angle);
			Update();
		}

		[BeginAction("rotatecounterclockwise")]
		private void DecreaseAngle()
		{
			panel.Angle = General.ClampAngle(panel.Angle - 5);
			angle = Angle2D.DegToRad(panel.Angle);
			Update();
		}

		#endregion
	}
}
