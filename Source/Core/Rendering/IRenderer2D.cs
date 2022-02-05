
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

using System.Collections.Generic;
using System.Drawing;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Data;

#endregion

namespace CodeImp.DoomBuilder.Rendering
{
	public interface IRenderer2D
	{
		// Properties
		float OffsetX { get; }
		float OffsetY { get; }
		float TranslateX { get; }
		float TranslateY { get; }
		float Scale { get; }
		int VertexSize { get; }
		bool DrawMapCenter { get; set; } //mxd
		ViewMode ViewMode { get; }

		// View methods
		Vector2D DisplayToMap(Vector2D mousepos);
		Vector2D MapToDisplay(Vector2D mappos);
		
		// Color methods
		PixelColor DetermineLinedefColor(Linedef l);
		PixelColor DetermineThingColor(Thing t);
		int DetermineVertexColor(Vertex v);
		int CalculateBrightness(int level);
		void UpdateExtraFloorFlag(); //mxd
		
		// Rendering management methods
		bool StartPlotter(bool clear);
		bool StartThings(bool clear);
		bool StartOverlay(bool clear);
		void Finish();
		void SetPresentation(Presentation present);
		void Present();

		// Drawing methods
		void PlotLine(Vector2D start, Vector2D end, PixelColor c);
		void PlotLine(Vector2D start, Vector2D end, PixelColor c, float lengthscaler); //mxd
		void PlotLinedef(Linedef l, PixelColor c);
		void PlotLinedefSet(ICollection<Linedef> linedefs);
		void PlotSector(Sector s);
		void PlotSector(Sector s, PixelColor c);
		void PlotVertex(Vertex v, int colorindex, bool checkMode = true);
		void PlotVertexAt(Vector2D v, int colorindex, bool checkMode = true);
		void PlotVerticesSet(ICollection<Vertex> vertices, bool checkMode = true);
		void RenderThing(Thing t, PixelColor c, float alpha);
		void RenderThingSet(ICollection<Thing> things, float alpha);
		void RenderRectangle(RectangleF rect, float bordersize, PixelColor c, bool transformrect);
		void RenderRectangleFilled(RectangleF rect, PixelColor c, bool transformrect);
		void RenderRectangleFilled(RectangleF rect, PixelColor c, bool transformrect, ImageData texture);
		void RenderLine(Vector2D start, Vector2D end, float thickness, PixelColor c, bool transformcoords);
		void RenderArrows(ICollection<Line3D> line); //mxd
		void RenderArrows(ICollection<Line3D> line, bool transformcoords); //mxd
		void RenderText(TextLabel text); //mxd, DB2 compatibility
		void RenderText(ITextLabel text); //mxd
		void RenderText(IList<ITextLabel> labels); //mxd
		void RenderGeometry(FlatVertex[] vertices, ImageData texture, bool transformcoords);
		void RenderHighlight(FlatVertex[] vertices, int color); //mxd
		void RedrawSurface();
	}
}
