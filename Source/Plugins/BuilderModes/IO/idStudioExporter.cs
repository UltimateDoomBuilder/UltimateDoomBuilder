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

		public idStudioExportSettings(idStudioExporterForm form)
		{
			modPath = form.ModPath;
			mapName = form.MapName;
			downscale = form.Downscale;
			xShift = form.xShift;
			yShift = form.yShift;
			zShift = form.zShift;
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
		
			if (form.ExportTextures)
				idStudioTextureExporter.ExportTextures(form);

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
				if (s.Triangles.Vertices.Count == 0)
					continue;

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
				// Do the floors and ceilings separately to ensure their brushes are grouped to different statics
				//General.ErrorLogger.Add(ErrorType.Warning, "HAs " + verts.Count + " verts");
				if(!s.HasSkyFloor)
				{
					geoWriter.BeginFuncStatic("floor", s.Index);
					for (int i = 0; i < verts.Count;)
					{
						idVertex c = verts[i++];
						idVertex b = verts[i++];
						idVertex a = verts[i++];
						geoWriter.WriteFloorBrush(a, b, c, floorHeight, false, s.FloorTexture, s.Index);
					}
					geoWriter.EndFuncStatic();
				}

				if (!s.HasSkyCeiling)
				{
					geoWriter.BeginFuncStatic("ceil", s.Index);
					for (int i = 0; i < verts.Count;)
					{
						idVertex c = verts[i++];
						idVertex b = verts[i++];
						idVertex a = verts[i++];
						geoWriter.WriteFloorBrush(a, b, c, ceilingHeight, true, s.CeilTexture, s.Index);
					}
					geoWriter.EndFuncStatic();
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
				if (line.Back == null)
				{
					if (front.MiddleTexture == "-")
						continue;

					// level.minHeight, level.maxHeight
					float drawHeight = frontOffsetY + (lowerUnpegged ? frontFloor : frontCeil);
					geoWriter.BeginFuncStatic("wall", frontSectIndex);
					geoWriter.WriteWallBrush(v0, v1, frontFloor, frontCeil, drawHeight, front.MiddleTexture, frontOffsetX, frontSectIndex);
					geoWriter.EndFuncStatic();
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

				// Ensures we don't create a static model entity with no brushes in it
				bool drawLow = front.LowRequired();
				bool drawMid = front.MiddleTexture != "-";
				bool drawHigh = front.HighRequired();

				if(drawLow || drawMid || drawHigh)
				{
					geoWriter.BeginFuncStatic("wall", backSectIndex);
					// Brush the front sidedefs in relation to the back sector heights
					if (drawLow) // This function checks a LOT more than whether the texture exists
					{
						// level.minHeight, backSector.floorHeight
						float drawHeight = frontOffsetY + (lowerUnpegged ? frontCeil : higherFloor);
						geoWriter.WriteWallBrush(v0, v1, frontFloor, backFloor, drawHeight, front.LowTexture, frontOffsetX, backSectIndex);

						int stepHeightCheck = back.Sector.FloorHeight - front.Sector.FloorHeight;
						if (stepHeightCheck <= 24) // TODO: Consider adding a check for linedef's "impassable" flag
							geoWriter.WriteStepBrush(v0, v1, lowerFloor, higherFloor, backSectIndex);
					}
					if (drawMid)
					{
						// Since middle textures do not repeat vertically, we have to account for the texture height
						float midTextureHeight = General.Map.Data.GetTextureImage(front.MiddleTexture).Height / cfg.downscale;

						float midMinHeight = lowerUnpegged ? (backFloor + frontOffsetY) : (backCeil - midTextureHeight + frontOffsetY);
						float midMaxHeight = lowerUnpegged ? (backFloor + midTextureHeight + frontOffsetY) : (backCeil + frontOffsetY);

						if (midMinHeight < backFloor) midMinHeight = backFloor;
						if (midMaxHeight > backCeil) midMaxHeight = backCeil;

						float drawHeight = frontOffsetY + (lowerUnpegged ? higherFloor : higherCeiling);
						geoWriter.WriteWallBrush(v0, v1, midMinHeight, midMaxHeight, drawHeight, front.MiddleTexture, frontOffsetX, backSectIndex);
					}
					if (drawHigh)
					{
						// backSector.ceilHeight, level.maxHeight
						float drawHeight = frontOffsetY + (upperUnpegged ? higherCeiling : lowerCeiling);
						geoWriter.WriteWallBrush(v0, v1, backCeil, frontCeil, drawHeight, front.HighTexture, frontOffsetX, backSectIndex);
					}
					geoWriter.EndFuncStatic();
				}

				drawLow = back.LowRequired();
				drawMid = back.MiddleTexture != "-";
				drawHigh = back.HighRequired();
				if(drawLow || drawMid || drawHigh)
				{
					// Brush the back sidedefs in relation to the front sector heights
					// This approach results in two overlapping brushes if both sides have a middle texture
					// BUG FIXED: Must swap start/end vertices to ensure texture is drawn on correct face
					// and begins at correct position
					geoWriter.BeginFuncStatic("wall", frontSectIndex);
					if (drawLow)
					{
						// level.minHeight, frontSector.floorHeight
						float drawHeight = backOffsetY + (lowerUnpegged ? backCeil : higherFloor);
						geoWriter.WriteWallBrush(v1, v0, backFloor, frontFloor, drawHeight, back.LowTexture, backOffsetX, frontSectIndex);

						int stepHeightCheck = front.Sector.FloorHeight - back.Sector.FloorHeight;
						if (stepHeightCheck <= 24)
							geoWriter.WriteStepBrush(v1, v0, lowerFloor, higherFloor, frontSectIndex);
					}
					if (drawMid)
					{
						float midTextureHeight = General.Map.Data.GetTextureImage(back.MiddleTexture).Height / cfg.downscale;

						float midMinHeight = lowerUnpegged ? (frontFloor + backOffsetY) : (frontCeil - midTextureHeight + backOffsetY);
						float midMaxHeight = lowerUnpegged ? (frontFloor + midTextureHeight + backOffsetY) : (frontCeil + backOffsetY);

						if (midMinHeight < frontFloor) midMinHeight = frontFloor;
						if (midMaxHeight > frontCeil) midMaxHeight = frontCeil;

						float drawHeight = backOffsetY + (lowerUnpegged ? higherFloor : higherCeiling);
						geoWriter.WriteWallBrush(v1, v0, midMinHeight, midMaxHeight, drawHeight, back.MiddleTexture, backOffsetX, frontSectIndex);
					}
					if (drawHigh)
					{
						float drawHeight = backOffsetY + (upperUnpegged ? higherCeiling : lowerCeiling);
						// frontSector.ceilHeight, level.maxHeight
						geoWriter.WriteWallBrush(v1, v0, frontCeil, backCeil, drawHeight, back.HighTexture, backOffsetX, frontSectIndex);
					}
					geoWriter.EndFuncStatic();
				}
			}
		}
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

	#region Entity Writer

	internal class idEntityBuilder
	{
		public StringBuilder builder = new StringBuilder();

		public void WriteTo(StreamWriter file)
		{
			/*
			 * A very stupid problem:
			 * - idStudio's map parser will not accept uppercase scientific notation
			 * - .Net's default ToString behavior always produces uppercase scientific notation
			 * - No format specifier exists to simply lowercase the E without other side effects
			 *		( "e" forcibly inserts scientific notation into all numbers)
			 * - Thus, we have no choice but to iterate through the finished string and manually
			 *		lowercase any scientific notation
			 */
			char[] fileChars = new char[builder.Length];
			builder.CopyTo(0, fileChars, 0, builder.Length);

			for (int i = 0; i < fileChars.Length; i++)
			{
				if (fileChars[i] != 'E') continue;

				if (fileChars[i + 1] == '+' || fileChars[i + 1] == '-')
					fileChars[i] = 'e';
			}
			file.Write(fileChars);
		}

		public void BeginBrushDef(string group, int sectorNum)
		{
			const string brushStart =
@"{{
	groups {{
		""nav""
		""{0}/{1}""
	}}
	brushDef3 {{
";
			builder.AppendFormat(brushStart, group, sectorNum);
		}

		// When grouped to entities the grouping of brushes is irrelevant
		public void BeginBrushDef()
		{
			const string brushStart =
@"{
	brushDef3 {
";

			builder.Append(brushStart);
		}

		public void WriteClipPlane(idPlane p)
		{
			builder.AppendFormat("\t\t( {0} {1} {2} {3}", p.n.x, p.n.y, p.n.z, -p.d);
			builder.Append(" ) ( ( 1 0 0 ) ( 0 1 0 ) ) \"art/tile/common/clip/clip\" 0 0 0\n");
		}

		public void WriteCasterPlane(idPlane p)
		{
			builder.AppendFormat("\t\t( {0} {1} {2} {3}", p.n.x, p.n.y, p.n.z, -p.d);
			builder.Append(" ) ( ( 1 0 0 ) ( 0 1 0 ) ) \"art/tile/common/shadow_caster\" 0 0 0\n");
		}

		public void EndBrushDef()
		{
			builder.Append("\t}\n}\n");
		}
	}

	#endregion


	#region Map Writer
	internal class idStudioMapWriter
	{
		#region entities

		// .map files have a default entityPrefix of nothing
		private const string rootMap =
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
		// Parm 0 = entity name
		// Parm 1 = Map name
		// Parm 2 = Group
		// Parm 3 = Subgroup
		// Must close entity manually after adding brushes
		private const string entity_func_static =
@"entity {{
	groups {{
		""nav""
		""{2}/{3}""
	}}
	entityDef {0} {{
		inherit = ""func/static"";
		edit = {{
			renderModelInfo = {{
				model = ""maps/{1}/{0}"";
			}}
			clipModelInfo = {{
				clipModelName = ""maps/{1}/{0}"";
			}}
		}}
	}}
";

		#endregion

		public idEntityBuilder world = new idEntityBuilder();
		public idEntityBuilder ents = new idEntityBuilder();
		private List<idStudioMapWriter> childMaps = new List<idStudioMapWriter>();
		private idStudioExportSettings cfg;

		private string fileName;      // File name - EXCLUDING extension and any folder structure
		private string fileExtension; // File extension - INCLUDING the period
		private string prefix;        // Refmap's prefix for entity names - INCLUDING the underscore
		private int staticModels = 0; // Number of static model entities


		// Constructor for a root map file
		public idStudioMapWriter(idStudioExportSettings p_cfg)
		{
			cfg = p_cfg;
			fileName = cfg.mapName;
			fileExtension = ".map";
			
			world.builder.AppendFormat(rootMap, "");
			prefix = "";
		}

		// Parameter p_prefix should NOT include the underscore
		private idStudioMapWriter(in idStudioMapWriter parent, string p_prefix)
		{
			cfg = parent.cfg;
			fileName = parent.fileName + "_" + p_prefix;
			fileExtension = ".refmap";
			
			world.builder.AppendFormat(rootMap, p_prefix);
			prefix = p_prefix + "_";
		}

		// Parameter refmapPrefix should NOT include the underscore
		public idStudioMapWriter AddRefmap(string refmapPrefix)
		{
			idStudioMapWriter newMap = new idStudioMapWriter(this, refmapPrefix);
			childMaps.Add(newMap);

			ents.builder.AppendFormat(entity_func_reference, prefix, childMaps.Count, newMap.fileName);
			return newMap;
		}

		public void SaveFile()
		{
			// Close World Entity
			world.builder.Append("}\n");

			string fullPath = Path.Combine(cfg.modPath, "base/maps/", fileName + fileExtension);
			using (StreamWriter file = new StreamWriter(fullPath, false)) {
				world.WriteTo(file);
				ents.WriteTo(file);
			}
				

			foreach (idStudioMapWriter m in childMaps)
				m.SaveFile();
		}

		public void BeginFuncStatic(string group, int subGroup)
		{
			// We include the map name to ensure uniqueness when merging multiple level refmaps
			string entityName = prefix + cfg.mapName + "_func_static_" + ++staticModels;
			ents.builder.AppendFormat(entity_func_static, entityName, cfg.mapName, group, subGroup);
		}

		public void EndFuncStatic()
		{
			ents.builder.Append("}\n");
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
			world.BeginBrushDef("stepclip", sectorNum);
			for (int i = 0; i < bounds.Length; i++)
				world.WriteClipPlane(bounds[i]);
			world.EndBrushDef();
		}

		public void WriteWallBrush(idVertex v0, idVertex v1, float minHeight, float maxHeight, float drawHeight, string texture, float offsetX, int sectorNum)
		{
			// A just-incase to prevent broken brushes. 
			// idStudio won't manually delete impossible brushes so we must
			// ensure they're all tall enough to be selectable. 
			if (maxHeight - minHeight < 0.0075f)
			{
				minHeight -= 100.0f / cfg.downscale;
				maxHeight += 100.0f / cfg.downscale;
				// return;
			}

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
			ents.BeginBrushDef();

			// Write untextured bounds
			for (int i = 0; i < bounds.Length; i++)
				ents.WriteCasterPlane(bounds[i]);

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

			ents.builder.AppendFormat(
				"\t\t( {0} {1} {2} {3} ) ( ( {4} 0 {5} ) ( 0 {6} {7} ) ) \"art/wadtobrush/walls/{8}\" 0 0 0\n", 
				surface.n.x, surface.n.y, surface.n.z, -surface.d,
				xScale, projection, yScale, drawHeight * yScale, texture.ToLowerInvariant()
			);
			ents.EndBrushDef();
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
			ents.BeginBrushDef();
			for(int i = 0; i < bounds.Length; i++)
				ents.WriteCasterPlane(bounds[i]);

			ImageData dimensions = General.Map.Data.GetFlatImage(texture);
			float xRatio = 1.0f / dimensions.Width;
			float yRatio = 1.0f / dimensions.Height;
			float xScale = xRatio * cfg.downscale;
			float yScale = yRatio * cfg.downscale;
			float xShift = -xRatio * cfg.xShift;
			float yShift = yRatio * cfg.yShift;

			// horizontal: (0, -1) Vertical (1, 0) - Ensures proper rotation of textures (for floors)
			ents.builder.AppendFormat(
				"\t\t( {0} {1} {2} {3} ) ( ( 0 {4} {5} ) ( {6} 0 {7} ) ) \"art/wadtobrush/flats/{8}\" 0 0 0\n",
				surface.n.x, surface.n.y, surface.n.z, -surface.d,
				isCeiling ? -xScale : xScale, xShift, -yScale, yShift, texture.ToLowerInvariant()
			);

			ents.EndBrushDef();
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
						filePath = ""textures/system/constant_color/black.tga"";
					}}
					specular = {{
						filePath = ""textures/system/constant_color/black.tga"";
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
						filePath = ""textures/system/constant_color/black.tga"";
					}}
					specular = {{
						filePath = ""textures/system/constant_color/black.tga"";
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
			// idStudio requires all files be all-lowercase
			string imgName = img.Name.ToLowerInvariant();

			// PART ONE - Write the art file
			// The way we get the bitmap ensures a "correct" bitmap independent
			// of UDB's brightness preference is produced
			string artPath = Path.Combine(artDir, subFolder, imgName + ".tga");
			WriteTGA(artPath, new Bitmap(img.LocalGetBitmap(false)));


			// PART 2 - Write the material2 decl
			bool useAlpha = img.IsTranslucent || img.IsMasked;

			string matPath = Path.Combine(matDir, subFolder, imgName + ".decl");

			string format;

			if (useAlpha)
				format = String.Format(mat2_staticAlpha, subFolder, imgName);
			else format = String.Format(mat2_static, subFolder, imgName);

			File.WriteAllText(matPath, format);
		}

		public static void ExportTextures(idStudioExporterForm form)
		{
			string modpath = form.ModPath;
			Directory.CreateDirectory(Path.Combine(modpath, dir_flats_art));
			Directory.CreateDirectory(Path.Combine(modpath, dir_flats_mat));
			Directory.CreateDirectory(Path.Combine(modpath, dir_walls_art));
			Directory.CreateDirectory(Path.Combine(modpath, dir_walls_mat));

			string artDir = Path.Combine(modpath, "base/art/wadtobrush/");
			string matDir = Path.Combine(modpath, "base/declTree/material2/art/wadtobrush/");

			if(form.ExportAllTextures)
			{
				//General.ErrorLogger.Add(ErrorType.Warning, "All Textures");
				foreach (ImageData img in General.Map.Data.Textures)
					WriteArtAsset(artDir, matDir, "walls/", img);

				foreach (ImageData img in General.Map.Data.Flats)
					WriteArtAsset(artDir, matDir, "flats/", img);
			}
			else
			{
				//General.ErrorLogger.Add(ErrorType.Warning, "Map Textures");
				foreach (string name in form.MapTextures)
				{
					ImageData img = General.Map.Data.GetTextureImage(name);
					WriteArtAsset(artDir, matDir, "walls/", img);
				}

				foreach(string name in form.MapFlats)
				{
					ImageData img = General.Map.Data.GetFlatImage(name);
					WriteArtAsset(artDir, matDir, "flats/", img);
				}
			}

		}
	}

	#endregion
}