/*
MIT License

Copyright (c) 2024 FlavorfulGecko5

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE. 
*/

#region ================== Namespaces

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.BuilderModes.Interface;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes.IO
{
	internal struct idStudioExportSettings
	{
		public string modPath;
		public string mapName;
		public float downscale;
		public float xShift;
		public float yShift;
		public float zShift;
		public bool exportTextures;

		public idStudioExportSettings(idStudioExporterForm form)
		{
			modPath = form.ModPath;
			mapName = form.MapName;
			downscale = form.Downscale;
			xShift = form.xShift;
			yShift = form.yShift;
			zShift = form.zShift;
			exportTextures = form.ExportTextures;
		}
	}

	internal class idStudioExporter
	{
		private idStudioExportSettings cfg;
		private idStudioExporterForm form;

		public void Export(idStudioExporterForm p_form)
		{
			form = p_form;
			cfg = new idStudioExportSettings(form);
		
			if (cfg.exportTextures)
				idStudioTextureExporter.ExportTextures(cfg.modPath);

			string mapPath = Path.Combine(cfg.modPath, "base/maps/");
			Directory.CreateDirectory(mapPath);

			idStudioMapWriter rootWriter = new idStudioMapWriter(cfg);
			idStudioMapWriter wadToBrushRef = rootWriter.AddRefmap("wadtobrush");
			idStudioMapWriter geoWriter = wadToBrushRef.AddRefmap("wadgeo");

			ExportGeometry(geoWriter);
			rootWriter.SaveFile();
		}

		private void ExportGeometry(idStudioMapWriter geoWriter)
		{
			// STEP 1: BUILD FLOOR/CEILING BRUSHES
			//General.ErrorLogger.Add(ErrorType.Warning, "We have " + General.Map.Map.Sectors.Count + " sectors");
			foreach(Sector s in General.Map.Map.Sectors)
			{
				List<idVertex> verts = new List<idVertex>();
				verts.Capacity = s.Triangles.Vertices.Count;
				foreach(Vector2D dv in s.Triangles.Vertices)
				{
					idVertex fv = new idVertex();
					fv.x = ((float)dv.x + cfg.xShift) / cfg.downscale;
					fv.y = ((float)dv.y + cfg.yShift) / cfg.downscale;
					verts.Add(fv);
				}
				float floorHeight = (s.FloorHeight + cfg.zShift) / cfg.downscale;
				float ceilingHeight = (s.CeilHeight + cfg.zShift) / cfg.downscale;

				// Given in clockwise winding order
				//General.ErrorLogger.Add(ErrorType.Warning, "HAs " + verts.Count + " verts");
				for (int i = 0; i < verts.Count;)
				{
					idVertex c = verts[i++];
					idVertex b = verts[i++];
					idVertex a = verts[i++];
					geoWriter.WriteFloorBrush(a, b, c, floorHeight, false, s.FloorTexture, s.Index);
					if(!s.CeilTexture.Equals("F_SKY1"))
						geoWriter.WriteFloorBrush(a, b, c, ceilingHeight, true, s.CeilTexture, s.Index);
				}
			}

			/*
			* STEP TWO: DRAW WALLS 
			* 
			* Draw Height Rules:
			*
			* One Sided: Ceiling (Default) / Floor (Lower Unpegged)
			* Lower Textures: Highest Floor (Default) / Ceiling the side is facing (Lower Unpegged) 
				- WIKI IS INCORRECT: Falsely asserts Lower Unpegged draws it from the higher ceiling downward
			* Upper Textures: Lowest Ceiling (Default) / Highest Ceiling (Upper Unpegged)
			* Middle Textures:
			*	- Do not repeat vertically - we must modify the brush bounds to account for this
			*		- TODO: THIS QUIRK IS NOT YET IMPLEMENTED
			*	- Highest Ceiling (Default) / Highest Floor (Lower Unpegged)
			*
			* No need for any crazy vector projection when calculating drawheight, so we can simply add in the
			* vertical offset right now
			*/
			foreach(Linedef line in General.Map.Map.Linedefs)
			{
				if (line.Front == null)
					continue;
				bool upperUnpegged = (line.RawFlags & 0x8) > 0;
				bool lowerUnpegged = (line.RawFlags & 0x10) > 0;
				idVertex v0 = new idVertex();
				idVertex v1 = new idVertex();
				{
					Vector2D vec = line.Start.Position;
					v0.x = ((float)vec.x + cfg.xShift) / cfg.downscale;
					v0.y = ((float)vec.y + cfg.yShift) / cfg.downscale;
					vec = line.End.Position;
					v1.x = ((float)vec.x + cfg.xShift) / cfg.downscale;
					v1.y = ((float)vec.y + cfg.yShift) / cfg.downscale;
				}

				Sidedef front = line.Front;
				float frontOffsetX = front.OffsetX / cfg.downscale;
				float frontOffsetY = front.OffsetY / cfg.downscale;
				float frontFloor = (front.Sector.FloorHeight + cfg.zShift) / cfg.downscale;
				float frontCeil = (front.Sector.CeilHeight + cfg.zShift) / cfg.downscale;
				int frontSectIndex = front.Sector.Index;

				// If true, this is a one-sided linedef
				if(front.MiddleRequired())
				{
					// level.minHeight, level.maxHeight
					float drawHeight = frontOffsetY + (lowerUnpegged ? frontFloor : frontCeil);
					geoWriter.WriteWallBrush(v0, v1, frontFloor, frontCeil, drawHeight, front.MiddleTexture, frontOffsetX, frontSectIndex);
					continue;
				}

				Sidedef back = line.Back;
				float backOffsetX = back.OffsetX / cfg.downscale;
				float backOffsetY = back.OffsetY / cfg.downscale;
				float backFloor = (back.Sector.FloorHeight + cfg.zShift) / cfg.downscale;
				float backCeil = (back.Sector.CeilHeight + cfg.zShift) / cfg.downscale;
				int backSectIndex = back.Sector.Index;

				// Texture pegging is based on the lowest/highest floor/ceiling - so we must distinguish
				// which values are smaller / larger - no way around this ugly chain of if statements unfortunately
				float lowerFloor, lowerCeiling, higherFloor, higherCeiling;
				if (frontCeil < backCeil) {
					lowerCeiling = frontCeil;
					higherCeiling = backCeil;
				}
				else {
					lowerCeiling = backCeil;
					higherCeiling = frontCeil;
				}
				if (frontFloor < backFloor) {
					lowerFloor = frontFloor;
					higherFloor = backFloor;
				}
				else {
					lowerFloor = backFloor;
					higherFloor = frontFloor;
				}

				// Brush the front sidedefs in relation to the back sector heights
				if (front.LowRequired()) // This function checks a LOT more than whether the texture exists
				{
					// level.minHeight, backSector.floorHeight
					float drawHeight = frontOffsetY + (lowerUnpegged ? frontCeil : higherFloor);
					geoWriter.WriteWallBrush(v0, v1, frontFloor, backFloor, drawHeight, front.LowTexture, frontOffsetX, backSectIndex);

					int stepHeightCheck = back.Sector.FloorHeight - front.Sector.FloorHeight;
					if (stepHeightCheck <= 24) // TODO: Consider adding a check for linedef's "impassable" flag
						geoWriter.WriteStepBrush(v0, v1, lowerFloor, higherFloor, backSectIndex);
				}
				if (!front.MiddleTexture.Equals("-"))
				{
					//float a = front.GetMiddleHeight();
					float drawHeight = frontOffsetY + (lowerUnpegged ? higherFloor : higherCeiling);
					geoWriter.WriteWallBrush(v0, v1, backFloor, backCeil, drawHeight, front.MiddleTexture, frontOffsetX, backSectIndex);
				}
				if (front.HighRequired())
				{
					// backSector.ceilHeight, level.maxHeight
					float drawHeight = frontOffsetY + (upperUnpegged ? higherCeiling : lowerCeiling);
					geoWriter.WriteWallBrush(v0, v1, backCeil, frontCeil, drawHeight, front.HighTexture, frontOffsetX, backSectIndex);
				}

				// Brush the back sidedefs in relation to the front sector heights
				// This approach results in two overlapping brushes if both sides have a middle texture
				// BUG FIXED: Must swap start/end vertices to ensure texture is drawn on correct face
				// and begins at correct position
				if (back.LowRequired())
				{
					// level.minHeight, frontSector.floorHeight
					float drawHeight = backOffsetY + (lowerUnpegged ? backCeil : higherFloor);
					geoWriter.WriteWallBrush(v1, v0, backFloor, frontFloor, drawHeight, back.LowTexture, backOffsetX, frontSectIndex);

					int stepHeightCheck = front.Sector.FloorHeight - back.Sector.FloorHeight;
					if (stepHeightCheck <= 24)
						geoWriter.WriteStepBrush(v1, v0, lowerFloor, higherFloor, frontSectIndex);
				}
				if (!back.MiddleTexture.Equals("-"))
				{
					float drawHeight = backOffsetY + (lowerUnpegged ? higherFloor : higherCeiling);
					geoWriter.WriteWallBrush(v1, v0, frontFloor, frontCeil, drawHeight, back.MiddleTexture, backOffsetX, frontSectIndex);
				}
				if (back.HighRequired())
				{
					float drawHeight = backOffsetY + (upperUnpegged ? higherCeiling : lowerCeiling);
					// frontSector.ceilHeight, level.maxHeight
					geoWriter.WriteWallBrush(v1, v0, frontCeil, backCeil, drawHeight, back.HighTexture, backOffsetX, frontSectIndex);
				}
			}
		}
	}

	enum BrushType
	{
		FLOOR,
		CEIL,
		WALL,
		STEPCLIP
	}

	#region 3D Math

	internal struct idVertex
	{
		public float x;
		public float y;

		// Default zero-constructor can be inferred

		public idVertex(float p_x, float p_y)
		{
			x = p_x; 
			y = p_y;
		}
	}

	internal struct idVector
	{
		public float x;
		public float y;
		public float z;

		// Default zero-constructor can be inferred

		public idVector(float p_x, float p_y, float p_z)
		{
			x = p_x; y = p_y; z = p_z;
		}

		public idVector(idVertex v0, idVertex v1)
		{
			x = v1.x - v0.x;
			y = v1.y - v0.y;
			z = 0.0f;
		}

		public void Normalize()
		{
			float magnitude = Magnitude();
			if(magnitude != 0)
			{
				x /= magnitude;
				y /= magnitude;
				z /= magnitude;
			}
		}

		public float Magnitude()
		{
			return (float)Math.Sqrt(x * x + y * y + z * z);
		}
	}

	internal struct idPlane
	{
		public idVector n;
		public float d;

		public void SetFrom(idVector p_normal, idVertex point)
		{
			n = p_normal;
			n.Normalize();
			d = n.x * point.x + n.y * point.y;
		}
	}

	#endregion


	#region Map Writer
	internal class idStudioMapWriter
	{
		#region entities
		private const string rootMap =
@"Version 7
HierarchyVersion 1
entity {
	entityDef world {
		inherit = ""worldspawn"";
		edit = {
		}
	}
";

		private const string rootRefmap =
@"Version 7
HierarchyVersion 1
entity {{
	entityDef world {{
		inherit = ""worldspawn"";
		edit = {{
			entityPrefix = ""{0}"";
		}}
	}}
";

		private const string entity_func_reference =
@"entity {{
	entityDef {0}func_reference_{1} {{
		inherit = ""func/reference"";
		edit = {{
			mapname = ""maps/{2}.refmap"";
		}}
	}}
// reference 0
	{{
	reference {{
		""maps/{2}.refmap""
	}}
}}
}}
";

		#endregion

		// Must increment with every written brush
		private static int brushHandle = 100000000; 


		private StringBuilder writer = new StringBuilder();
		private List<idStudioMapWriter> childMaps = new List<idStudioMapWriter>();
		private idStudioExportSettings cfg;

		private string fileName; // File name - EXCLUDING extension and any folder structure
		private string prefix;   // Refmap's prefix for entity names


		// Constructor for a root map file
		public idStudioMapWriter(idStudioExportSettings p_cfg)
		{
			cfg = p_cfg;
			fileName = cfg.mapName;
			prefix = "";

			writer.Append(rootMap);
		}

		private idStudioMapWriter(in idStudioMapWriter parent, string p_prefix)
		{
			cfg = parent.cfg;
			prefix = p_prefix;
			fileName = parent.fileName + "_" + prefix;

			writer.Append(String.Format(rootRefmap, prefix));
		}

		public idStudioMapWriter AddRefmap(string refmapPrefix)
		{
			idStudioMapWriter newMap = new idStudioMapWriter(this, refmapPrefix);
			childMaps.Add(newMap);
			return newMap;
		}

		public bool IsRoot()
		{
			return prefix.Length == 0;
		}

		public void SaveFile()
		{
			// Close Entity
			writer.Append("\n}");

			// Write all refmaps
			for(int i = 0; i < childMaps.Count; i++)
			{
				string refmapEntity = String.Format(entity_func_reference,
					IsRoot() ? "" : prefix + "_",
					i + 1,
					childMaps[i].fileName);
				writer.Append(refmapEntity);
			}

			string fullPath = Path.Combine(cfg.modPath, "base/maps/", fileName + (IsRoot() ? ".map" : ".refmap"));
			/*
			 * A very stupid problem:
			 * - idStudio's map parser will not accept uppercase scientific notation
			 * - .Net's default ToString behavior always produces uppercase scientific notation
			 * - No format specifier exists to simply lowercase the E without other side effects
			 *		( "e" forcibly inserts scientific notation into all numbers)
			 * - Thus, we have no choice but to iterate through the finished string and manually
			 *		lowercase any scientific notation
			 */
			char[] fileChars = new char[writer.Length];
			writer.CopyTo(0, fileChars, 0, writer.Length);
			for(int i = 0; i < fileChars.Length; i++)
			{
				if (fileChars[i] != 'E') continue;

				if (fileChars[i + 1] == '+' || fileChars[i + 1] == '-')
					fileChars[i] = 'e';
			}

			using (StreamWriter file = new StreamWriter(fullPath, false))
				file.Write(fileChars);

			foreach (idStudioMapWriter m in childMaps)
				m.SaveFile();
		}

		private void BeginBrushDef(BrushType type, int sectorNum)
		{
			void AddGroup(in string g)
			{
				writer.Append("\t\t\"" + g + "\"\n");
			}
			writer.Append("{\n\thandle = " + brushHandle++ + "\n\tgroups {\n");

			switch(type)
			{
				case BrushType.FLOOR:
				AddGroup("sectors/" + sectorNum + "/floor");
				AddGroup("floors/" + sectorNum);
				AddGroup("nav");
				break;

				case BrushType.CEIL:
				AddGroup("sectors/" + sectorNum + "/ceiling");
				AddGroup("ceilings/" + sectorNum);
				break;

				case BrushType.WALL:
				AddGroup("sectors/" + sectorNum + "/walls");
				AddGroup("walls/" + sectorNum);
				break;

				case BrushType.STEPCLIP:
				AddGroup("sectors/" + sectorNum + "/stepclip");
				AddGroup("stepclip/" + sectorNum);
				AddGroup("nav");
				break;
			}

			writer.Append("\t}\n\tbrushDef3 {\n");
		}

		private static string TEXTURE_SHADOWCASTER = "art/tile/common/shadow_caster";
		private static string TEXTURE_CLIP = "art/tile/common/clip/clip";

		private void WritePlane(idPlane p, string texture)
		{
			writer.AppendFormat("\t\t( {0} {1} {2} {3}", p.n.x, p.n.y, p.n.z, -p.d);
			writer.Append(" ) ( ( 1 0 0 ) ( 0 1 0 ) ) \"");
			writer.Append(texture);
			writer.Append("\" 0 0 0\n");
		}

		private void EndBrushDef()
		{
			writer.Append("\t}\n}\n");
		}

		public void WriteStepBrush(idVertex v0, idVertex v1, float minHeight, float maxHeight, int sectorNum)
		{
			float xyShift = (maxHeight - minHeight) * 2; // Creates a 30 degree slope
			idPlane[] bounds = new idPlane[5];
			idVector horizontal = new idVector(v0, v1);

			// Crossing horizontal X <0, 0, 1>
			idVector cross = new idVector(horizontal.y, -horizontal.x, 0);
			cross.Normalize();


			// Find the XY coordinates of the points at the base of our slope
			idVertex b0 = new idVertex(cross.x * xyShift + v0.x, cross.y * xyShift + v0.y);
			//idVertex b1 = new idVertex(cross.x * xyShift + v1.x, cross.y * xyShift + v1.y);


			// Plane 0 - The "Rear" wall of the staircase
			bounds[0].n.x = -cross.x;
			bounds[0].n.y = -cross.y;
			bounds[0].n.z = 0;
			bounds[0].d = bounds[0].n.x * v0.x + bounds[0].n.y * v0.y;

			// Plane 1 - The "Left" wall of the staircase
			idVector leftHori = new idVector(v0, b0);
			bounds[1].SetFrom(new idVector(leftHori.y, -leftHori.x, 0), v0);

			// Plane 2 - The "Right" wall of the staircase
			bounds[2].n.x = -bounds[1].n.x;
			bounds[2].n.y = -bounds[1].n.y;
			bounds[2].n.z = 0;
			bounds[2].d = bounds[2].n.x * v1.x + bounds[2].n.y * v1.y;

			// Plane 3 - The "Bottom" ceiling of the staircase
			bounds[3].n = new idVector(0, 0, -1);
			bounds[3].d = -minHeight;

			// Plane 4 - The inclined plane
			idVector a = new idVector(leftHori.x, leftHori.y, minHeight - maxHeight);
			idVector b = new idVector(horizontal.x, horizontal.y, 0);

			// Computing a x b to have a normal pointing upward
			idVector axb = new idVector(-b.y * a.z, b.x * a.z, a.x * b.y - b.x * a.y);
			axb.Normalize();
			bounds[4].n.x = axb.x;
			bounds[4].n.y = axb.y;
			bounds[4].n.z = axb.z;
			bounds[4].d = axb.x * v0.x + axb.y * v0.y + axb.z * maxHeight;
			//bounds[4].SetFrom(axb, v0);

			// Draw the Brush
			BeginBrushDef(BrushType.STEPCLIP, sectorNum);
			for (int i = 0; i < bounds.Length; i++)
				WritePlane(bounds[i], TEXTURE_CLIP);
			EndBrushDef();
		}

		public void WriteWallBrush(idVertex v0, idVertex v1, float minHeight, float maxHeight, float drawHeight, string texture, float offsetX, int sectorNum)
		{
			idPlane[] bounds = new idPlane[5]; // Untextured surfaces
			idPlane surface = new idPlane();   // Texture surface
			idVector horizontal = new idVector(v0, v1);

			// PART 1 - CONSTRUCT THE PLANES
			// Crossing horizontal X <0, 0, 1>
			surface.SetFrom(new idVector(horizontal.y, -horizontal.x, 0), v1);

			// Plane 0 - The "Back" SideDef to the LineDef's left
			bounds[0].n.x = -surface.n.x;
			bounds[0].n.y = -surface.n.y;
			bounds[0].n.z = 0;

			//idVertex d0 = new idVertex(bounds[0].n.x* 0.0075f + v0.x, bounds[0].n.y * 0.0075f + v0.y);
			idVertex d1 = new idVertex(bounds[0].n.x* 0.0075f + v1.x, bounds[0].n.y * 0.0075f + v1.y);
			bounds[0].d = bounds[0].n.x * d1.x + bounds[0].n.y * d1.y;

			// Plane 1: Forward Border Sliver: d1 - v1
			idVector deltaVector = new idVector(v1, d1);
			bounds[1].SetFrom(new idVector(deltaVector.y, -deltaVector.x, 0), d1);

			// Plane 2: Rear Border Sliver: v0 - d0
			bounds[2].n.x = -bounds[1].n.x;
			bounds[2].n.y = -bounds[1].n.y;
			bounds[2].n.z = 0;
			bounds[2].d = bounds[2].n.x * v0.x + bounds[2].n.y * v0.y;

			// Plane 3: Upper Bound:
			bounds[3].n = new idVector(0, 0, 1);
			bounds[3].d = maxHeight;

			// Plane 4: Lower Bound
			bounds[4].n = new idVector(0, 0, -1);
			bounds[4].d = minHeight * -1;


			// PART 2: DRAW THE SURFACE
			BeginBrushDef(BrushType.WALL, sectorNum);

			// Write untextured bounds
			for (int i = 0; i < bounds.Length; i++)
				WritePlane(bounds[i], TEXTURE_SHADOWCASTER);

			// Write Textured surface
			// POSSIBLE TODO: TEST IF TEXTURE DOES NOT EXIST, draw as regular plane if it doesn't

			ImageData dimensions = General.Map.Data.GetTextureImage(texture);
			float xScale = 1.0f / dimensions.Width * cfg.downscale;
			float yScale = 1.0f / dimensions.Height * cfg.downscale;

			/*
			* We must shift the texture grid such that the origin is centered on
			* the wall's left vertex. To do this accurately, we calculate the magnitude
			* of the projection of the shift vector onto the horizontal wall vector.
			* We finalize this by adding the texture X offset to this value.
			* The math works out such that the XY downscale cancels in both terms when
			* the texture's X scale is multiplied in at the end.
			*/
			float projection = ((horizontal.x * v0.x + horizontal.y * v0.y) / horizontal.Magnitude() - offsetX) * xScale * -1;

			writer.AppendFormat(
				"\t\t( {0} {1} {2} {3} ) ( ( {4} 0 {5} ) ( 0 {6} {7} ) ) \"art/wadtobrush/walls/{8}\" 0 0 0\n", 
				surface.n.x, surface.n.y, surface.n.z, -surface.d,
				xScale, projection, yScale, drawHeight * yScale, texture
			);
			EndBrushDef();
		}

		public void WriteFloorBrush(idVertex a, idVertex b, idVertex c, float height, bool isCeiling, string texture, int sectorNum)
		{
			idPlane[] bounds = new idPlane[4]; // Untextured surfaces
			idPlane surface = new idPlane(); // Texture surface

			// PART 1 - CONSTRUCT PLANE OBJECTS
			// We assume the points are given in a COUNTER-CLOCKWISE order
			// Hence, we cross horizontal X <0, 0, 1> to get our normal

			// Plane 0 - First Wall
			idVector h = new idVector(a, b);
			bounds[0].SetFrom(new idVector(h.y, -h.x, 0.0f), a);

			// Plane 1 - Second Wall
			h = new idVector(b, c);
			bounds[1].SetFrom(new idVector(h.y, -h.x, 0.0f), b);

			// Plane 2 - Last Wall
			h = new idVector(c, a);
			bounds[2].SetFrom(new idVector(h.y, -h.x, 0.0f), c);

			if (isCeiling) {
				bounds[3].n = new idVector(0, 0, 1);
				bounds[3].d = height + 0.0075f;
				surface.n = new idVector(0, 0, -1);
				surface.d = -height;
			}
			else {
				surface.n = new idVector(0, 0, 1);
				surface.d = height;
				bounds[3].n = new idVector(0, 0, -1);
				bounds[3].d = 0.0075f - height;
			}

			// PART 2: DRAW THE SURFACE
			BeginBrushDef(isCeiling ? BrushType.CEIL : BrushType.FLOOR, sectorNum);
			for(int i = 0; i < bounds.Length; i++)
				WritePlane(bounds[i], TEXTURE_SHADOWCASTER);

			ImageData dimensions = General.Map.Data.GetFlatImage(texture);
			float xRatio = 1.0f / dimensions.Width;
			float yRatio = 1.0f / dimensions.Height;
			float xScale = xRatio * cfg.downscale;
			float yScale = yRatio * cfg.downscale;
			float xShift = -xRatio * cfg.xShift;
			float yShift = yRatio * cfg.yShift;

			// horizontal: (0, -1) Vertical (1, 0) - Ensures proper rotation of textures (for floors)
			writer.AppendFormat(
				"\t\t( {0} {1} {2} {3} ) ( ( 0 {4} {5} ) ( {6} 0 {7} ) ) \"art/wadtobrush/flats/{8}\" 0 0 0\n",
				surface.n.x, surface.n.y, surface.n.z, -surface.d,
				isCeiling ? -xScale : xScale, xShift, -yScale, yShift, texture
			);

			EndBrushDef();
		}
	}
	#endregion

	#region Texture Exports

	internal class idStudioTextureExporter
	{
		private const string mat2_static =
@"declType( material2 ) {{
	inherit = ""template/pbr"";
	edit = {{
		RenderLayers = {{
			item[0] = {{
				parms = {{
					smoothness = {{
						filePath = ""art/wadtobrush/black.tga"";
					}}
					specular = {{
						filePath = ""art/wadtobrush/black.tga"";
					}}
					albedo = {{
						filePath = ""art/wadtobrush/{0}{1}.tga"";
					}}
				}}
			}}
		}}
	}}
}}";

		private const string mat2_staticAlpha =
@"declType( material2 ) {{
	inherit = ""template/pbr_alphatest"";
	edit = {{
		RenderLayers = {{
			item[0] = {{
				parms = {{
					cover = {{
						filePath = ""art/wadtobrush/{0}{1}.tga"";
					}}
					smoothness = {{
						filePath = ""art/wadtobrush/black.tga"";
					}}
					specular = {{
						filePath = ""art/wadtobrush/black.tga"";
					}}
					albedo = {{
						filePath = ""art/wadtobrush/{0}{1}.tga"";
					}}
				}}
			}}
		}}
	}}
}}";

		private const string dir_flats_art = "base/art/wadtobrush/flats/";
		private const string dir_flats_mat = "base/declTree/material2/art/wadtobrush/flats/";
		private const string dir_walls_art = "base/art/wadtobrush/walls/";
		private const string dir_walls_mat = "base/declTree/material2/art/wadtobrush/walls/";

		private const string path_black = "base/art/wadtobrush/black.tga";
		
		// Unable to export patches at this time
		//private const string dir_patches = "base/art/wadtobrush/patches/";
		//private const string dir_patches_mat = "base/declTree/material2/art/wadtobrush/patches/";

		/*
		 * Credits: This function is a modified port of https://gist.github.com/maluoi/ade07688e741ab188841223b8ffeed22
		 */
		private static void WriteTGA(in string filename, in Bitmap data)
		{
			byte[] header = { 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
				(byte)(data.Width % 256), (byte)(data.Width / 256), 
				(byte)(data.Height % 256), (byte)(data.Height / 256), 
				32, 0x20 };

			using(FileStream file = File.OpenWrite(filename))
			{
				lock(data)
				{
					file.Write(header, 0, header.Length);

					for (int h = 0; h < data.Height; h++)
					{
						for (int w = 0; w < data.Width; w++)
						{
							Color c = data.GetPixel(w, h);
							byte[] pixel = { c.B, c.G, c.R, c.A };
							file.Write(pixel, 0, pixel.Length);
						}
					}
				}
			} 
		}

		private static void WriteArtAsset(string artDir, string matDir, string subFolder, ImageData img)
		{
			// PART ONE - Write the art file
			// The way we get the bitmap ensures a "correct" bitmap independent
			// of UDB's brightness preference is produced
			string artPath = Path.Combine(artDir, subFolder, img.Name + ".tga");
			WriteTGA(artPath, new Bitmap(img.LocalGetBitmap(false)));


			// PART 2 - Write the material2 decl
			bool useAlpha = img.IsTranslucent || img.IsMasked;

			string matPath = Path.Combine(matDir, subFolder, img.Name + ".decl");

			string format;

			if (useAlpha)
				format = String.Format(mat2_staticAlpha, subFolder, img.Name);
			else format = String.Format(mat2_static, subFolder, img.Name);

			File.WriteAllText(matPath, format);
		}

		public static void ExportTextures(string modPath)
		{
			Directory.CreateDirectory(Path.Combine(modPath, dir_flats_art));
			Directory.CreateDirectory(Path.Combine(modPath, dir_flats_mat));
			Directory.CreateDirectory(Path.Combine(modPath, dir_walls_art));
			Directory.CreateDirectory(Path.Combine(modPath, dir_walls_mat));

			// Generate black texture
			{
				Color pixel = Color.FromArgb(255, 0, 0, 0);
				int blackWidth = 64, blackHeight = 64;
				Bitmap black = new Bitmap(blackWidth, blackHeight);
				

				for(int w = 0; w < blackWidth; w++)
					for(int h = 0; h < blackHeight; h++)
						black.SetPixel(w, h, pixel);

				WriteTGA(Path.Combine(modPath, path_black), black);
			}

			string artDir = Path.Combine(modPath, "base/art/wadtobrush/");
			string matDir = Path.Combine(modPath, "base/declTree/material2/art/wadtobrush/");

			foreach (ImageData img in General.Map.Data.Textures)
				WriteArtAsset(artDir, matDir, "walls/", img);

			foreach (ImageData img in General.Map.Data.Flats)
				WriteArtAsset(artDir, matDir, "flats/", img);
		}
	}

	#endregion
}