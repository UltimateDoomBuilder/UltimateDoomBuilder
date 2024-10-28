#region ================== Namespaces

using System;
using System.Collections.Generic;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.VisualModes;

#endregion

namespace CodeImp.DoomBuilder.GZBuilder.Data 
{
	public static class LinksCollector
	{
		#region ================== SpecialThings

		private class SpecialThings 
		{
			public readonly Dictionary<int, List<Thing>> PatrolPoints; // PatrolPoint tag, list of PatrolPoints
			public readonly List<Thing> PatrolSpecials;
			public readonly Dictionary<int, List<PathNode>> InterpolationPoints; // InterpolationPoint tag, list of InterpolationPoints
			public readonly List<Thing> InterpolationSpecials;
			public readonly List<Thing> ThingsWithGoal;
			public readonly List<Thing> Cameras;
			public readonly Dictionary<int, List<Thing>> ActorMovers; // ActorMover target tag, list of ActorMovers
			public readonly List<Thing> PathFollowers;
			public readonly Dictionary<int, List<Thing>> PolyobjectAnchors; //angle, list of PolyobjectAnchors
			public readonly Dictionary<int, List<Thing>> PolyobjectStartSpots; //angle, list of PolyobjectStartSpots

			public SpecialThings()
			{
				PatrolPoints = new Dictionary<int, List<Thing>>();
				PatrolSpecials = new List<Thing>();
				InterpolationPoints = new Dictionary<int, List<PathNode>>();
				InterpolationSpecials = new List<Thing>();
				ThingsWithGoal = new List<Thing>();
				Cameras = new List<Thing>();
				ActorMovers = new Dictionary<int, List<Thing>>();
				PathFollowers = new List<Thing>();
				PolyobjectAnchors = new Dictionary<int, List<Thing>>();
				PolyobjectStartSpots = new Dictionary<int, List<Thing>>();
			}
		}

		#endregion

		#region ================== PathNode

		private class PathNode
		{
			private readonly Thing thing;
			private readonly Vector3D position;
			private readonly Dictionary<int, PathNode> nextnodes;
			private readonly Dictionary<int, PathNode> prevnodes;

			public Thing Thing { get { return thing; } }
			public Dictionary<int, PathNode> NextNodes { get { return nextnodes; } } // Thing index, PathNode
			public Dictionary<int, PathNode> PreviousNodes { get { return prevnodes; } } // Thing index, PathNode
			public Vector3D Position { get { return position; } }
			public bool IsCurved;

			public PathNode(Thing t, VisualBlockMap blockmap)
			{
				thing = t;
				position = t.Position;
				position.z += GetCorrectHeight(t, blockmap, true);
				nextnodes = new Dictionary<int, PathNode>();
				prevnodes = new Dictionary<int, PathNode>();
			}

			internal void PropagateCurvedFlag()
			{
				if(!IsCurved) return;
				foreach(PathNode node in nextnodes.Values)
				{
					if(node.IsCurved) continue;
					node.IsCurved = true;
					node.PropagateCurvedFlag();
				}
				foreach(PathNode node in prevnodes.Values)
				{
					if(node.IsCurved) continue;
					node.IsCurved = true;
					node.PropagateCurvedFlag();
				}
			}
		}

		#endregion

		#region ================== Constants

		private const int CIRCLE_SIDES = 24;

		#endregion

		#region ================== Shape creation methods

		private static IEnumerable<Line3D> MakeCircleLines(Vector3D pos, PixelColor color, double radius, int numsides)
		{
			List<Line3D> result = new List<Line3D>(numsides);
			Vector3D start = new Vector3D(pos.x, pos.y + radius, pos.z);
			double anglestep = Angle2D.PI2 / numsides;

			for(int i = 1; i < numsides + 1; i++)
			{
				Vector3D end = pos + new Vector3D(Math.Sin(anglestep * i) * radius, Math.Cos(anglestep * i) * radius, 0f);
				result.Add(new Line3D(start, end, color, false));
				start = end;
			}

			return result;
		}

		private static IEnumerable<Line3D> MakeRectangleLines(Vector3D pos, PixelColor color, float size)
		{
			float halfsize = size / 2;
			Vector3D tl = new Vector3D(pos.x - halfsize, pos.y - halfsize, pos.z);
			Vector3D tr = new Vector3D(pos.x + halfsize, pos.y - halfsize, pos.z);
			Vector3D bl = new Vector3D(pos.x - halfsize, pos.y + halfsize, pos.z);
			Vector3D br = new Vector3D(pos.x + halfsize, pos.y + halfsize, pos.z);
			
			return new List<Line3D>
			{
				new Line3D(tl, tr, color, false),
				new Line3D(tr, br, color, false),
				new Line3D(bl, br, color, false),
				new Line3D(bl, tl, color, false),
			};
		}

		#endregion

		#region ================== GetHelperShapes

		public static List<Line3D> GetHelperShapes(ICollection<Thing> things) { return GetHelperShapes(things, null); }
		public static List<Line3D> GetHelperShapes(ICollection<Thing> things, VisualBlockMap blockmap)
		{
			var lines = GetHelperShapes(GetSpecialThings(things, blockmap), blockmap);
			lines.AddRange(GetThingArgumentShapes(things, blockmap, CIRCLE_SIDES));
			return lines;
		}

		private static SpecialThings GetSpecialThings(ICollection<Thing> things, VisualBlockMap blockmap) 
		{
			SpecialThings result = new SpecialThings();

			// Process oh so special things
			foreach(Thing t in things)
			{
				ThingTypeInfo info = General.Map.Data.GetThingInfoEx(t.Type);
				if(info == null) continue;
				switch(info.ClassName.ToLowerInvariant())
				{
					case "patrolpoint":
						if(t.Tag != 0 || t.Args[0] != 0)
						{
							if(!result.PatrolPoints.ContainsKey(t.Tag)) result.PatrolPoints.Add(t.Tag, new List<Thing>());
							result.PatrolPoints[t.Tag].Add(t);
						}
						break;

					case "patrolspecial":
						result.PatrolSpecials.Add(t);
						break;

					case "$polyanchor":
						if(!result.PolyobjectAnchors.ContainsKey(t.AngleDoom)) result.PolyobjectAnchors[t.AngleDoom] = new List<Thing>();
						result.PolyobjectAnchors[t.AngleDoom].Add(t);
						break;

					case "$polyspawn":
					case "$polyspawncrush":
					case "$polyspawnhurt":
						if(!result.PolyobjectStartSpots.ContainsKey(t.AngleDoom)) result.PolyobjectStartSpots[t.AngleDoom] = new List<Thing>();
						result.PolyobjectStartSpots[t.AngleDoom].Add(t);
						break;
				}

				// Process Thing_SetGoal action
				if(t.Action != 0 
					&& General.Map.Config.LinedefActions.ContainsKey(t.Action) 
					&& General.Map.Config.LinedefActions[t.Action].Id.ToLowerInvariant() == "thing_setgoal"
					&& (t.Args[0] == 0 || t.Args[0] == t.Tag)
					&& t.Args[1] != 0)
				{
					result.ThingsWithGoal.Add(t);
				}
			}

			// We may need all of these actors...
			foreach(Thing t in General.Map.ThingsFilter.VisibleThings)
			{
				ThingTypeInfo info = General.Map.Data.GetThingInfoEx(t.Type);
				if(info == null) continue;
				switch(info.ClassName.ToLowerInvariant())
				{
					case "interpolationpoint":
						if(!result.InterpolationPoints.ContainsKey(t.Tag)) result.InterpolationPoints.Add(t.Tag, new List<PathNode>());
						result.InterpolationPoints[t.Tag].Add(new PathNode(t, blockmap));
						break;

					case "interpolationspecial":
						result.InterpolationSpecials.Add(t);
						break;

					case "movingcamera":
						if(t.Args[0] != 0 || t.Args[1] != 0) result.Cameras.Add(t);
						break;

					case "pathfollower":
						if(t.Args[0] != 0 || t.Args[1] != 0) result.PathFollowers.Add(t);
						break;

					case "actormover":
						if((t.Args[0] != 0 || t.Args[1] != 0) && t.Args[3] != 0)
						{
							if(!result.ActorMovers.ContainsKey(t.Args[3])) result.ActorMovers.Add(t.Args[3], new List<Thing>());
							result.ActorMovers[t.Args[3]].Add(t);
						}
						break;
				}
			}

			return result;
		}

		private static List<Line3D> GetHelperShapes(SpecialThings result, VisualBlockMap blockmap) 
		{
			var lines = new List<Line3D>();
			var actormovertargets = new Dictionary<int, List<Thing>>();

			// Get ActorMover targets
			if(result.ActorMovers.Count > 0)
			{
				foreach(Thing t in General.Map.Map.Things) 
				{
					if(t.Tag == 0 || !result.ActorMovers.ContainsKey(t.Tag)) continue;
					if(!actormovertargets.ContainsKey(t.Tag)) actormovertargets[t.Tag] = new List<Thing>();
					actormovertargets[t.Tag].Add(t);
				}
			}

			Vector3D start, end;

			// Process patrol points
			foreach(KeyValuePair<int, List<Thing>> group in result.PatrolPoints) 
			{
				foreach(Thing t in group.Value) 
				{
					if(!result.PatrolPoints.ContainsKey(t.Args[0])) continue;
					
					start = t.Position;
					start.z += GetCorrectHeight(t, blockmap, true);

					foreach(Thing tt in result.PatrolPoints[t.Args[0]])
					{
						end = tt.Position;
						end.z += GetCorrectHeight(tt, blockmap, true);
						lines.Add(new Line3D(start, end));
					}
				}
			}

			// Process things with Thing_SetGoal
			foreach(Thing t in result.ThingsWithGoal) 
			{
				if(!result.PatrolPoints.ContainsKey(t.Args[1])) continue;

				start = t.Position;
				start.z += GetCorrectHeight(t, blockmap, true);

				foreach(Thing tt in result.PatrolPoints[t.Args[1]]) 
				{
					end = tt.Position;
					end.z += GetCorrectHeight(tt, blockmap, true);

					lines.Add(new Line3D(start, end, General.Colors.Selection));
				}
			}

			// Process patrol specials
			foreach (Thing t in result.PatrolSpecials)
			{
				if (!result.PatrolPoints.ContainsKey(t.Tag)) continue;

				start = t.Position;
				start.z += GetCorrectHeight(t, blockmap, true);

				foreach (Thing tt in result.PatrolPoints[t.Tag])
				{
					end = tt.Position;
					end.z += GetCorrectHeight(tt, blockmap, true);

					lines.Add(new Line3D(start, end, General.Colors.Selection));
				}
			}

			// Process cameras [CAN USE INTERPOLATION]
			foreach(Thing t in result.Cameras)
			{
				int targettag = t.Args[0] + (t.Args[1] << 8);
				if(targettag == 0 || !result.InterpolationPoints.ContainsKey(targettag)) continue; //no target / target doesn't exist
				bool interpolatepath = ((t.Args[2] & 1) != 1);

				start = t.Position;
				start.z += GetCorrectHeight(t, blockmap, true);

				foreach(PathNode node in result.InterpolationPoints[targettag]) 
				{
					node.IsCurved = interpolatepath;
					lines.Add(new Line3D(start, node.Position, General.Colors.Selection));
				}
			}

			//process actor movers [CAN USE INTERPOLATION]
			foreach(List<Thing> things in result.ActorMovers.Values) 
			{
				foreach(Thing t in things)
				{
					int targettag = t.Args[0] + (t.Args[1] << 8);

					// Add interpolation point targets
					if(targettag != 0 && result.InterpolationPoints.ContainsKey(targettag))
					{
						bool interpolatepath = ((t.Args[2] & 1) != 1);
						start = t.Position;
						start.z += GetCorrectHeight(t, blockmap, true);

						foreach(PathNode node in result.InterpolationPoints[targettag])
						{
							node.IsCurved = interpolatepath;
							lines.Add(new Line3D(start, node.Position, General.Colors.Selection));
						}
					}

					// Add thing-to-move targets
					if(actormovertargets.ContainsKey(t.Args[3]))
					{
						start = t.Position;
						start.z += GetCorrectHeight(t, blockmap, true);

						foreach(Thing tt in actormovertargets[t.Args[3]])
						{
							end = tt.Position;
							end.z += GetCorrectHeight(tt, blockmap, true);
							lines.Add(new Line3D(start, end, General.Colors.Selection));
						}
					}
				}
			}

			// Process path followers [CAN USE INTERPOLATION]
			foreach(Thing t in result.PathFollowers)
			{
				int targettag = t.Args[0] + (t.Args[1] << 8);
				if(targettag == 0 || !result.InterpolationPoints.ContainsKey(targettag)) continue; //no target / target doesn't exist
				bool interpolatepath = (t.Args[2] & 1) != 1;

				start = t.Position;
				start.z += GetCorrectHeight(t, blockmap, true);

				foreach(PathNode node in result.InterpolationPoints[targettag])
				{
					node.IsCurved = interpolatepath;
					lines.Add(new Line3D(start, node.Position, General.Colors.Selection));
				}
			}

			// Process polyobjects
			foreach(KeyValuePair<int, List<Thing>> group in result.PolyobjectAnchors)
			{
				if(!result.PolyobjectStartSpots.ContainsKey(group.Key)) continue;
				foreach(Thing anchor in group.Value)
				{
					start = anchor.Position;
					start.z += GetCorrectHeight(anchor, blockmap, true);

					foreach(Thing startspot in result.PolyobjectStartSpots[group.Key]) 
					{
						end = startspot.Position;
						end.z += GetCorrectHeight(startspot, blockmap, true);
						lines.Add(new Line3D(start, end, General.Colors.Selection));
					}
				}
			}

			// Process interpolation points [CAN BE INTERPOLATED]
			// 1. Connect PathNodes
			foreach(KeyValuePair<int, List<PathNode>> group in result.InterpolationPoints)
			{
				foreach(PathNode node in group.Value)
				{
					int targettag = node.Thing.Args[3] + (node.Thing.Args[4] << 8);
					if(targettag == 0 || !result.InterpolationPoints.ContainsKey(targettag)) continue;

					foreach(PathNode targetnode in result.InterpolationPoints[targettag])
					{
						// Connect both ways
						if(!node.NextNodes.ContainsKey(targetnode.Thing.Index)) node.NextNodes.Add(targetnode.Thing.Index, targetnode);
						if(!targetnode.PreviousNodes.ContainsKey(node.Thing.Index)) targetnode.PreviousNodes.Add(node.Thing.Index, node);
					}
				}
			}

			// 2. Propagate IsCurved flag
			foreach(KeyValuePair<int, List<PathNode>> group in result.InterpolationPoints)
			{
				foreach(PathNode node in group.Value) node.PropagateCurvedFlag();
			}

			// Process interpolation specials
			foreach (Thing t in result.InterpolationSpecials)
			{
				int targettag = t.Tag;
				if (targettag == 0 || !result.InterpolationPoints.ContainsKey(targettag)) continue; //no target / target doesn't exist

				start = t.Position;
				start.z += GetCorrectHeight(t, blockmap, true);

				foreach (PathNode node in result.InterpolationPoints[targettag])
				{
					//Do not connect specials to the first or last node of a curved path, since those are used as spline control points only
					if (node.IsCurved && (node.PreviousNodes.Count == 0 || node.NextNodes.Count == 0))
						continue;
					lines.Add(new Line3D(start, node.Position, General.Colors.Selection));
				}
			}

			// 3. Make lines
			HashSet<int> processedindices = new HashSet<int>();
			foreach(KeyValuePair<int, List<PathNode>> group in result.InterpolationPoints)
			{
				foreach(PathNode node in group.Value)
				{
					// Draw as a curve?
					if(node.IsCurved && !processedindices.Contains(node.Thing.Index) && node.NextNodes.Count > 0 && node.PreviousNodes.Count > 0)
					{
						PathNode prev = General.GetByIndex(node.PreviousNodes, 0).Value;
						PathNode next = General.GetByIndex(node.NextNodes, 0).Value;
						if(next.NextNodes.Count > 0)
						{
							PathNode nextnext = General.GetByIndex(next.NextNodes, 0).Value;
								
								// Generate curve points
								List<Vector3D> points = new List<Vector3D>(11);
								for(int i = 0; i < 11; i++)
								{
									double u = i * 0.1f;
									points.Add(new Vector3D(
										SplineLerp(u, prev.Position.x, node.Position.x, next.Position.x, nextnext.Position.x),
										SplineLerp(u, prev.Position.y, node.Position.y, next.Position.y, nextnext.Position.y),
										(blockmap == null ? 0 : SplineLerp(u, prev.Position.z, node.Position.z, next.Position.z, nextnext.Position.z))
									));
								}

								// Add line segments
								for(int i = 1; i < 11; i++)
								{
									lines.Add(new Line3D(points[i - 1], points[i], i == 10));
								}

								continue;
						}
					}

					// Draw regular lines
					bool startnode = (node.IsCurved && node.PreviousNodes.Count == 0); // When using curves, this node won't be used by camera (the last node won't be used as well), so draw them using different color
					foreach(PathNode targetnode in node.NextNodes.Values)
					{
						bool isskipped = (startnode || (targetnode.IsCurved && targetnode.NextNodes.Count == 0));
						lines.Add(new Line3D(node.Position, targetnode.Position, (isskipped ? General.Colors.Highlight : General.Colors.InfoLine), !isskipped));
					}
				}
			}

			return lines;
		}

		#endregion

		#region ================== GetThingArgumentShapes

		// Create argument value/min/max shapes
		private static List<Line3D> GetThingArgumentShapes(ICollection<Thing> things, VisualBlockMap blockmap, int numsides)
		{
			var lines = new List<Line3D>();
			
			foreach(Thing t in things)
			{
				if(t.Action != 0) continue;
				ThingTypeInfo tti = General.Map.Data.GetThingInfoEx(t.Type);
				if(tti == null) continue;

				Vector3D pos = t.Position;
				pos.z += GetCorrectHeight(t, blockmap, false);

				for(int i = 0; i < t.Args.Length; i++)
				{
					if(t.Args[i] == 0) continue; // Avoid visual noise
					var a = tti.Args[i]; //TODO: can this be null?
					
					switch(a.RenderStyle)
					{
						case ArgumentInfo.ArgumentRenderStyle.CIRCLE:
							lines.AddRange(MakeCircleLines(pos, a.RenderColor, t.Args[i], numsides));
							if(a.MinRange > 0) lines.AddRange(MakeCircleLines(pos, a.MinRangeColor, a.MinRange, numsides));
							if(a.MaxRange > 0) lines.AddRange(MakeCircleLines(pos, a.MaxRangeColor, a.MaxRange, numsides));
							break;

						case ArgumentInfo.ArgumentRenderStyle.RECTANGLE:
							lines.AddRange(MakeRectangleLines(pos, a.RenderColor, t.Args[i]));
							if(a.MinRange > 0) lines.AddRange(MakeRectangleLines(pos, a.MinRangeColor, a.MinRange));
							if(a.MaxRange > 0) lines.AddRange(MakeRectangleLines(pos, a.MaxRangeColor, a.MaxRange));
							break;

						case ArgumentInfo.ArgumentRenderStyle.NONE:
							break;

						default: throw new NotImplementedException("Unknown ArgumentRenderStyle");
					}
				}
			}

			return lines;
		}

        #endregion

        #region ================== GetDynamicLightShapes

        public static List<Line3D> GetPointLightShape(Thing t, bool highlight, GZGeneral.LightData ld, int linealpha)
        {
            // TODO: this basically duplicates VisualThing.UpdateLight()...
            // Determine light radiii
            int primaryradius;
            int secondaryradius = 0;

            if (ld.LightDef != GZGeneral.LightDef.VAVOOM_GENERIC &&
                ld.LightDef != GZGeneral.LightDef.VAVOOM_COLORED) //if it's gzdoom light
            {
                if (ld.LightModifier == GZGeneral.LightModifier.SECTOR)
                {
                    if (t.Sector == null) t.DetermineSector();
                    int scaler = (t.Sector != null ? t.Sector.Brightness / 4 : 2);
                    primaryradius = t.Args[3] * scaler;
                }
                else
                {
                    primaryradius = t.Args[3] * 2; //works... that.. way in GZDoom
                    if (ld.LightAnimated) secondaryradius = t.Args[4] * 2;
                }
            }
            else //it's one of vavoom lights
            {
                primaryradius = t.Args[0] * 8;
            }

            // Check radii...
            if (primaryradius < 1 && secondaryradius < 1) return null;

            // Determine light color
            PixelColor color;
            if (highlight)
            {
                color = General.Colors.Highlight.WithAlpha((byte)linealpha);
            }
            else
            {
                switch (t.DynamicLightType.LightDef)
                {
                    case GZGeneral.LightDef.VAVOOM_GENERIC: // Vavoom light
                        color = new PixelColor((byte)linealpha, 255, 255, 255);
                        break;

                    case GZGeneral.LightDef.VAVOOM_COLORED: // Vavoom colored light
                        color = new PixelColor((byte)linealpha, (byte)t.Args[1], (byte)t.Args[2], (byte)t.Args[3]);
                        break;

					case GZGeneral.LightDef.POINT_LIGHTMAP:
						// ZDRay static lights have an intensity that's set through the thing's alpha value
						double intensity = t.Fields.GetValue("alpha", 1.0);
						byte r = (byte)General.Clamp(t.Args[0] * intensity, 0.0, 255.0);
						byte g = (byte)General.Clamp(t.Args[1] * intensity, 0.0, 255.0);
						byte b = (byte)General.Clamp(t.Args[2] * intensity, 0.0, 255.0);
						color = new PixelColor((byte)linealpha, r, g, b);
						break;
                    default:
                        color = new PixelColor((byte)linealpha, (byte)t.Args[0], (byte)t.Args[1], (byte)t.Args[2]);
                        break;
                }
            }

            // Add lines if visible
            List<Line3D> circles = new List<Line3D>();
            if (primaryradius > 0) circles.AddRange(MakeCircleLines(t.Position, color, primaryradius, CIRCLE_SIDES));
            if (secondaryradius > 0) circles.AddRange(MakeCircleLines(t.Position, color, secondaryradius, CIRCLE_SIDES));
            return circles;
        }

        private static Vector3D GetRotatedVertex(Vector3D bp, double angle, double pitch)
        {
            Vector3D bp_rotated = bp;
            bp_rotated.x = bp.x * Math.Cos(pitch) - bp.z * Math.Sin(pitch);
            bp_rotated.z = bp.z * Math.Cos(pitch) + bp.x * Math.Sin(pitch);
            bp = bp_rotated;
            bp_rotated.x = bp.x * Math.Cos(angle) - bp.y * Math.Sin(angle);
            bp_rotated.y = bp.y * Math.Cos(angle) + bp.x * Math.Sin(angle);
            return bp_rotated;
        }

        public static List<Line3D> GetSpotLightShape(Thing t, bool highlight, GZGeneral.LightData ld, int linealpha)
        {
            PixelColor color;
            if (t.Fields.ContainsKey("arg0str"))
            {
                ZDoom.ZDTextParser.GetColorFromString(t.Fields["arg0str"].Value.ToString(), out color);
                color.a = (byte)linealpha;
            }
            else color = new PixelColor((byte)linealpha, (byte)((t.Args[0] & 0xFF0000) >> 16), (byte)((t.Args[0] & 0x00FF00) >> 8), (byte)((t.Args[0] & 0x0000FF)));

			// ZDRay static lights have an intensity that's set through the thing's alpha value
			if (t.DynamicLightType.LightDef == GZGeneral.LightDef.SPOT_LIGHTMAP)
			{
				double intensity = t.Fields.GetValue("alpha", 1.0);
				if (intensity != 1.0)
				{
					byte r = (byte)General.Clamp(color.r * intensity, 0.0, 255.0);
					byte g = (byte)General.Clamp(color.g * intensity, 0.0, 255.0);
					byte b = (byte)General.Clamp(color.b * intensity, 0.0, 255.0);
					color = new PixelColor((byte)linealpha, r, g, b);
				}
			}


            if (highlight)
            {
                color = General.Colors.Highlight.WithAlpha((byte)linealpha);
            }

            PixelColor color_secondary = color;
            color_secondary.a /= 2;

            List<Line3D> shapes = new List<Line3D>();
            double _lAngle1 = Angle2D.DegToRad(t.Args[1]);
            double _lAngle2 = Angle2D.DegToRad(t.Args[2]);
            double lAngle1 = _lAngle1;
            double lAngle2 = _lAngle2;
            
            double lRadius = t.Args[3]*2;
            double lDirY1 = Math.Sin(-lAngle1) * lRadius;
            double lDirX1 = Math.Cos(-lAngle1) * lRadius;
            double lDirY2 = Math.Sin(-lAngle2) * lRadius;
            double lDirX2 = Math.Cos(-lAngle2) * lRadius;
            
            IEnumerable<Line3D> circleLines = MakeCircleLines(new Vector3D(0, 0, 0), color, Math.Abs(lDirY1), CIRCLE_SIDES);
            foreach (Line3D l3d in circleLines)
            {
                shapes.Add(new Line3D(new Vector3D(lDirX1, l3d.Start.x, l3d.Start.y),
                                      new Vector3D(lDirX1, l3d.End.x, l3d.End.y),
                                      color, false));
            }

            if (lAngle2 != lAngle1)
            {
                circleLines = MakeCircleLines(new Vector3D(0, 0, 0), color_secondary, Math.Abs(lDirY2), CIRCLE_SIDES);
                foreach (Line3D l3d in circleLines)
                {
                    shapes.Add(new Line3D(new Vector3D(lDirX2, l3d.Start.x, l3d.Start.y),
                                          new Vector3D(lDirX2, l3d.End.x, l3d.End.y),
                                          color_secondary, false));
                }
            }

            // draw another circle to show the front cone shape
            int numsides = CIRCLE_SIDES * 2;
            double anglestep = Angle2D.PI2 / numsides;
            for (int j = -1; j <= 1; j++)
            {
                if (j == 0) continue;
                List<Line3D> tmplines = new List<Line3D>();
                PixelColor ccol = color;
                for (int i = 1; i < numsides + 1; i++)
                {
					double angc = j * i * anglestep;
					double angp = j * (i - 1) * anglestep;
                    if (i * anglestep > lAngle1 && ccol.a == color.a)
                    {
                        shapes.Add(new Line3D(new Vector3D(Math.Cos(angp) * lRadius, Math.Sin(angp) * lRadius, 0),
                                                new Vector3D(Math.Cos(j * lAngle1) * lRadius, Math.Sin(j * lAngle1) * lRadius, 0),
                                                ccol, false));
                        bool dobreak = false;
                        if (i * anglestep > lAngle2)
                        {
                            angc = j * lAngle2;
                            dobreak = true;
                        }
                        shapes.Add(new Line3D(new Vector3D(Math.Cos(j * lAngle1) * lRadius, Math.Sin(j * lAngle1) * lRadius, 0),
                                                new Vector3D(Math.Cos(angc) * lRadius, Math.Sin(angc) * lRadius, 0),
                                                color_secondary, false));
                        ccol = color_secondary;
                        if (dobreak) break;
                    }
                    else if (i * anglestep > lAngle2)
                    {
                        angc = j * lAngle2;
                        shapes.Add(new Line3D(new Vector3D(Math.Cos(angp) * lRadius, Math.Sin(angp) * lRadius, 0),
                                                new Vector3D(Math.Cos(angc) * lRadius, Math.Sin(angc) * lRadius, 0),
                                                ccol, false));
                        break;
                    }
                    else
                    {
                        shapes.Add(new Line3D(new Vector3D(Math.Cos(angp) * lRadius, Math.Sin(angp) * lRadius, 0),
                                                new Vector3D(Math.Cos(angc) * lRadius, Math.Sin(angc) * lRadius, 0),
                                                ccol, false));
                    }
                }
            }

            shapes.Add(new Line3D(new Vector3D(0, 0, 0), new Vector3D(lDirX1, lDirY1, 0), color, false));
            shapes.Add(new Line3D(new Vector3D(0, 0, 0), new Vector3D(lDirX1, -lDirY1, 0), color, false));
            if (lAngle2 != lAngle1)
            {
                shapes.Add(new Line3D(new Vector3D(0, 0, 0), new Vector3D(lDirX2, lDirY2, 0), color_secondary, false));
                shapes.Add(new Line3D(new Vector3D(0, 0, 0), new Vector3D(lDirX2, -lDirY2, 0), color_secondary, false));
            }

            // do translation and rotation
            foreach (Line3D l3d in shapes)
            {
                // rotate
                l3d.Start = GetRotatedVertex(l3d.Start, t.Angle-1.5708f, Angle2D.DegToRad(t.Pitch));
                l3d.End = GetRotatedVertex(l3d.End, t.Angle-1.5708f, Angle2D.DegToRad(t.Pitch));
                // translate
                l3d.Start += t.Position;
                l3d.End += t.Position;
            }

            return shapes;
        }

        public static List<Line3D> GetDynamicLightShapes(IEnumerable<Thing> things, bool highlight)
        {
            List<Line3D> circles = new List<Line3D>();
            if (General.Map.DOOM) return circles;

            const int linealpha = 128;
            foreach (Thing t in things)
            {
                GZGeneral.LightData ld = t.DynamicLightType;
                if (ld == null || ld.LightType == GZGeneral.LightType.SUN) continue;

                if (ld.LightType != GZGeneral.LightType.SPOT)
                {
                    List<Line3D> lshape = GetPointLightShape(t, highlight, ld, linealpha);
                    if (lshape != null) circles.AddRange(lshape);
                }
                else
                {
                    List<Line3D> lshape = GetSpotLightShape(t, highlight, ld, linealpha);
                    if (lshape != null) circles.AddRange(lshape);
                }
            }

            // Done
            return circles;
        }

		#endregion

		#region ================== GetAmbientSoundShapes

		public static List<Line3D> GetAmbientSoundShapes(IEnumerable<Thing> things, bool highlight)
		{
			List<Line3D> circles = new List<Line3D>();
			const int linealpha = 128;

			foreach(Thing t in things)
			{
				ThingTypeInfo info = General.Map.Data.GetThingInfoEx(t.Type);
				if(info == null) continue;

				float minradius, maxradius;
				if(info.AmbientSound != null)
				{
					minradius = info.AmbientSound.MinimumRadius;
					maxradius = info.AmbientSound.MaximumRadius;
				}
				else if(!General.Map.DOOM && (info.ClassName == "AmbientSound" || info.ClassName == "AmbientSoundNoGravity"))
				{
					//arg0: ambient slot
					//arg1: (optional) sound volume, in percent. 1 is nearly silent, 100 and above are full volume. If left to zero, full volume is also used.
					//arg2: (optional) minimum distance, in map units, at which volume attenuation begins. Note that arg3 must also be set. If both are left to zero, normal rolloff is used instead.
					//arg3: (optional) maximum distance, in map units, at which the sound can be heard. If left to zero or lower than arg2, normal rolloff is used instead.
					//arg4: (optional) scalar by which to multiply the values of arg2 and arg3. If left to zero, no multiplication takes place.

					if(t.Args[0] == 0 || !General.Map.Data.AmbientSounds.ContainsKey(t.Args[0]))
						continue;

					// Use custom radii?
					if(t.Args[2] > 0 && t.Args[3] > 0 && t.Args[3] > t.Args[2])
					{
						minradius = t.Args[2] * (t.Args[4] != 0 ? t.Args[4] : 1.0f);
						maxradius = t.Args[3] * (t.Args[4] != 0 ? t.Args[4] : 1.0f);
					}
					else
					{
						minradius = General.Map.Data.AmbientSounds[t.Args[0]].MinimumRadius;
						maxradius = General.Map.Data.AmbientSounds[t.Args[0]].MaximumRadius;
					}
				}
				else
				{
					continue;
				}

				// Determine color
				PixelColor color = (highlight ? General.Colors.Highlight.WithAlpha(linealpha) : t.Color.WithAlpha(linealpha));

				// Add lines if visible
				if(minradius > 0) circles.AddRange(MakeCircleLines(t.Position, color, minradius, CIRCLE_SIDES));
				if(maxradius > 0) circles.AddRange(MakeCircleLines(t.Position, color, maxradius, CIRCLE_SIDES));
			}

			return circles;
		}

		#endregion

		#region ================== Utility

		// Taken from Xabis' "curved interpolation points paths" patch.
		private static double SplineLerp(double u, double p1, double p2, double p3, double p4)
		{
			double t2 = u;
			double res = 2 * p2;
			res += (p3 - p1) * u;
			t2 *= u;
			res += (2 * p1 - 5 * p2 + 4 * p3 - p4) * t2;
			t2 *= u;
			res += (3 * p2 - 3 * p3 + p4 - p1) * t2;
			return 0.5f * res;
		}

		// Required only when called from VisualMode
		private static double GetCorrectHeight(Thing thing, VisualBlockMap blockmap, bool usethingcenter)
		{
			if(blockmap == null) return 0f;
			double height = (usethingcenter ? thing.Height / 2f : 0f);
			if(thing.Sector == null) thing.DetermineSector(blockmap);
			if(thing.Sector != null) height += thing.Sector.FloorHeight;
			return height;
		}

		#endregion
	}
}
