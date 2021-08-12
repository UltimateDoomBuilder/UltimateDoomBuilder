#region ================== Namespaces

using System;
using System.Collections.Generic;
using System.Drawing;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.VisualModes;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	internal sealed class VisualFogBoundary : BaseVisualGeometrySidedef
	{
		#region ================== Variables

		#endregion
		
		#region ================== Constructor / Setup

		// Constructor
		public VisualFogBoundary(BaseVisualMode mode, VisualSector vs, Sidedef s) : base(mode, vs, s)
		{
			//mxd
			geometrytype = VisualGeometryType.FOG_BOUNDARY;
			
			// Set render pass
			this.RenderPass = RenderPass.Additive;
			
			// We have no destructor
			GC.SuppressFinalize(this);
		}
		
		// This builds the geometry. Returns false when no geometry created.
		public override bool Setup()
		{
			if(!IsFogBoundary()) return false;

			//mxd. lightfog flag support
			int lightvalue;
			bool lightabsolute;
			GetLightValue(out lightvalue, out lightabsolute);

			// Left and right vertices for this sidedef
			Vector2D vl, vr;
			if(Sidedef.IsFront)
			{
				vl = new Vector2D(Sidedef.Line.Start.Position.x, Sidedef.Line.Start.Position.y);
				vr = new Vector2D(Sidedef.Line.End.Position.x, Sidedef.Line.End.Position.y);
			}
			else
			{
				vl = new Vector2D(Sidedef.Line.End.Position.x, Sidedef.Line.End.Position.y);
				vr = new Vector2D(Sidedef.Line.Start.Position.x, Sidedef.Line.Start.Position.y);
			}

			// Load sector data
			SectorData sd = mode.GetSectorData(Sidedef.Sector);
			SectorData osd = mode.GetSectorData(Sidedef.Other.Sector);
			if(!osd.Updated) osd.Update();

			// Set texture
			base.Texture = General.Map.Data.BlackTexture;

			// Determine texture coordinates plane as they would be in normal circumstances.
			TexturePlane tp = new TexturePlane();
			float floorbias = (Sidedef.Sector.CeilHeight == Sidedef.Sector.FloorHeight) ? 1.0f : 0.0f;
			float zoffset = Sidedef.Sector.CeilHeight - Sidedef.Other.Sector.CeilHeight; //mxd

			if(zoffset > 0) tp.tlt.y -= zoffset; //mxd
			tp.trb.x = tp.tlt.x + Sidedef.Line.Length;
			tp.trb.y = tp.tlt.y + (Sidedef.Sector.CeilHeight - (Sidedef.Sector.FloorHeight + floorbias));

			// Left top and right bottom of the geometry that
			tp.vlt = new Vector3D(vl.x, vl.y, Sidedef.Sector.CeilHeight);
			tp.vrb = new Vector3D(vr.x, vr.y, Sidedef.Sector.FloorHeight + floorbias);

			// Make the right-top coordinates
			tp.trt = new Vector2D(tp.trb.x, tp.tlt.y);
			tp.vrt = new Vector3D(tp.vrb.x, tp.vrb.y, tp.vlt.z);

			// Keep top and bottom planes for intersection testing
			top = sd.Ceiling.plane;
			bottom = sd.Floor.plane;

			// Create initial polygon, which is just a quad between floor and ceiling
			WallPolygon poly = new WallPolygon();
			poly.Add(new Vector3D(vl.x, vl.y, sd.Floor.plane.GetZ(vl)));
			poly.Add(new Vector3D(vl.x, vl.y, sd.Ceiling.plane.GetZ(vl)));
			poly.Add(new Vector3D(vr.x, vr.y, sd.Ceiling.plane.GetZ(vr)));
			poly.Add(new Vector3D(vr.x, vr.y, sd.Floor.plane.GetZ(vr)));

			// Determine initial color
			int lightlevel = sd.Ceiling.brightnessbelow + lightvalue;

			// Calculate fog density
			fogfactor = CalculateFogFactor(lightlevel);
			poly.color = PixelColor.INT_WHITE;

			// Cut off the part below the other floor and above the other ceiling
			CropPoly(ref poly, osd.Ceiling.plane, true);
			CropPoly(ref poly, osd.Floor.plane, true);

			List<WallPolygon> polygons = new List<WallPolygon> { poly };

			// Keep top and bottom planes for intersection testing
			top = osd.Ceiling.plane;
			bottom = osd.Floor.plane;

			// Process the polygon and create vertices
			List<WorldVertex> verts = CreatePolygonVertices(polygons, tp, sd, lightvalue, lightabsolute);
			if(verts.Count > 2)
			{
				base.SetVertices(verts);
				return true;
			}

			base.SetVertices(null);
			return false;
		}

		#endregion

		#region ================== Methods

		//==========================================================================
		//
		// Check if the current linedef is a candidate for a fog boundary
		//
		// Requirements for a fog boundary:
		// - front sector has no fog
		// - back sector has fog
		// - at least one of both does not have a sky ceiling.
		//
		//==========================================================================
		private bool IsFogBoundary()
		{
			if(Sidedef.Sector.Index == Sidedef.Other.Sector.Index) return false; // There can't be a boundary if both sides are in the same sector.
			if(Sidedef.Sector.HasSkyCeiling && Sidedef.Other.Sector.HasSkyCeiling) return false;
			return (Sidedef.Sector.FogMode > SectorFogMode.CLASSIC && Sidedef.Other.Sector.FogMode <= SectorFogMode.CLASSIC);
		}

		// This performs a fast test in object picking
		public override bool PickFastReject(Vector3D from, Vector3D to, Vector3D dir) { return false; }

		// This performs an accurate test for object picking
		public override bool PickAccurate(Vector3D from, Vector3D to, Vector3D dir, ref double u_ray) { return false; }

		// Unused
		protected override void SetTextureOffsetX(int x) { }
		protected override void SetTextureOffsetY(int y) { }
		protected override void MoveTextureOffset(int offsetx, int offsety) { }
		protected override Point GetTextureOffset() { return Point.Empty; }

		#endregion
	}
}
