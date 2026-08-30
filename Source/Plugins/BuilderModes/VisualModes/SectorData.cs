#region === Copyright (c) 2010 Pascal van der Heiden ===

using System;
using System.Collections.Generic;
using CodeImp.DoomBuilder.GZBuilder.Data;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Rendering;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	internal class SectorData
	{
		#region ================== Variables
		
		// VisualMode
		private readonly BaseVisualMode mode;
		
		// Sector for which this data is
		private readonly Sector sector;
		
		// Levels have been updated?
		private bool updated;
		
		// This prevents recursion
		private bool isupdating;
		
		// All planes in the sector that cast or are affected by light
		private readonly List<SectorLevel> lightlevels;
		
		// Effects
		private readonly List<SectorEffect> alleffects;
		private readonly List<Effect3DFloor> extrafloors;
		private readonly EffectGlowingFlat glowingflateffect; //mxd

		internal GlowingFlatData CeilingGlow; //mxd
		internal GlowingFlatData FloorGlow; //mxd
		internal Plane FloorGlowPlane; //mxd
		internal Plane CeilingGlowPlane; //mxd

        // [ZZ] Doom64 lighting system
        internal PixelColor ColorCeiling;
        internal PixelColor ColorFloor;
        internal PixelColor ColorWallBottom;
        internal PixelColor ColorWallTop;
        internal PixelColor ColorSprites;
		
		// Sectors that must be updated when this sector is changed
		// The boolean value is the 'includeneighbours' of the UpdateSectorGeometry function which
		// indicates if the sidedefs of neighbouring sectors should also be rebuilt.
		private readonly Dictionary<Sector, bool> updatesectors;
		
		// Original floor and ceiling levels
		private readonly SectorLevel floor;
		private readonly SectorLevel floorbase; // mxd. Sector floor level, unaffected by glow / light properties transfer
		private readonly SectorLevel ceiling;
		private readonly SectorLevel ceilingbase; // mxd. Sector ceiling level, unaffected by glow / light properties transfer 
		
		// This helps keeping track of changes
		// otherwise we update ceiling/floor too much
		private bool floorchanged;
		private bool ceilingchanged;

		//mxd. Absolute lights are not affected by brightness transfers...
		private bool lightfloorabsolute;
		private bool lightceilingabsolute;
		private int lightfloor;
		private int lightceiling;

		#endregion
		
		#region ================== Properties
		
		public Sector Sector { get { return sector; } }
		public bool Updated { get { return updated; } }
		public bool FloorChanged { get { return floorchanged; } set { floorchanged |= value; } }
		public bool CeilingChanged { get { return ceilingchanged; } set { ceilingchanged |= value; } }
		public List<SectorLevel> LightLevels { get { return lightlevels; } }
		public List<Effect3DFloor> ExtraFloors { get { return extrafloors; } }
		public List<SectorEffect> Effects { get { return alleffects; } } //mxd
		public SectorLevel Floor { get { return floor; } }
		public SectorLevel Ceiling { get { return ceiling; } }
		public BaseVisualMode Mode { get { return mode; } }
		public Dictionary<Sector, bool> UpdateAlso { get { return updatesectors; } }
		
		#endregion
		
		#region ================== Constructor / Destructor
		
		// Constructor
		public SectorData(BaseVisualMode mode, Sector s)
		{
			// Initialize
			this.mode = mode;
			this.sector = s;
			this.updated = false;
			this.floorchanged = false;
			this.ceilingchanged = false;
			this.lightlevels = new List<SectorLevel>(2);
			this.extrafloors = new List<Effect3DFloor>(1);
			this.alleffects = new List<SectorEffect>(1);
			this.updatesectors = new Dictionary<Sector, bool>(2);
			this.floor = new SectorLevel(sector, SectorLevelType.Floor);
			this.floorbase = new SectorLevel(sector, SectorLevelType.Floor); //mxd
			this.ceiling = new SectorLevel(sector, SectorLevelType.Ceiling);
			this.ceilingbase = new SectorLevel(sector, SectorLevelType.Ceiling); //mxd
			this.glowingflateffect = new EffectGlowingFlat(this); //mxd
			
			// Add ceiling and floor
			lightlevels.Add(floor);
			lightlevels.Add(ceiling);

            BasicSetup();
		}
		
		#endregion
		
		#region ================== Public Methods
		
		// 3D Floor effect
		public void AddEffect3DFloor(Linedef sourcelinedef)
		{
			Effect3DFloor e = new Effect3DFloor(this, sourcelinedef);
			extrafloors.Add(e);
			alleffects.Add(e);

			//mxd. Extrafloor neighbours should be updated when extrafloor is changed
			foreach(Sidedef sd in this.Sector.Sidedefs)
			{
				if(sd.Other != null && sd.Other.Sector != null)
					AddUpdateSector(sd.Other.Sector, false);
			}
		}
		
		// Brightness level effect
		public void AddEffectBrightnessLevel(Linedef sourcelinedef)
		{
			EffectBrightnessLevel e = new EffectBrightnessLevel(this, sourcelinedef);
			alleffects.Add(e);
		}

		//mxd. Transfer Floor Brightness effect
		public void AddEffectTransferFloorBrightness(Linedef sourcelinedef) 
		{
			EffectTransferFloorBrightness e = new EffectTransferFloorBrightness(this, sourcelinedef);
			alleffects.Add(e);
		}

		//mxd. Transfer Floor Brightness effect
		public void AddEffectTransferCeilingBrightness(Linedef sourcelinedef) 
		{
			EffectTransferCeilingBrightness e = new EffectTransferCeilingBrightness(this, sourcelinedef);
			alleffects.Add(e);
		}

		// Line slope effect
		public void AddEffectLineSlope(Linedef sourcelinedef)
		{
			EffectLineSlope e = new EffectLineSlope(this, sourcelinedef);
			alleffects.Add(e);
		}

		//mxd. Plane copy slope effect
		public void AddEffectPlaneClopySlope(Linedef sourcelinedef, bool front) 
		{
			EffectPlaneCopySlope e = new EffectPlaneCopySlope(this, sourcelinedef, front);
			alleffects.Add(e);
		}

		// Copy slope effect
		public void AddEffectCopySlope(Thing sourcething)
		{
			EffectCopySlope e = new EffectCopySlope(this, sourcething);
			alleffects.Add(e);
		}

		// Thing line slope effect
		public void AddEffectThingLineSlope(Thing sourcething, Sidedef sourcesidedef)
		{
			EffectThingLineSlope e = new EffectThingLineSlope(this, sourcething, sourcesidedef);
			alleffects.Add(e);
		}

		// Thing slope effect
		public void AddEffectThingSlope(Thing sourcething)
		{
			EffectThingSlope e = new EffectThingSlope(this, sourcething);
			alleffects.Add(e);
		}

		// Thing vertex slope effect
		public void AddEffectThingVertexSlope(List<Thing> sourcethings, bool slopefloor)
		{
			EffectThingVertexSlope e = new EffectThingVertexSlope(this, sourcethings, slopefloor);
			alleffects.Add(e);
		}

		//mxd. Add UDMF vertex offset effect
		public void AddEffectVertexOffset() 
		{
			EffectUDMFVertexOffset e = new EffectUDMFVertexOffset(this);
			alleffects.Add(e);
		}
		
		// This adds a sector for updating
		public void AddUpdateSector(Sector s, bool includeneighbours)
		{
			updatesectors[s] = includeneighbours;
		}
		
		// This adds a sector level
		public void AddSectorLevel(SectorLevel level)
		{
			// Note: Inserting before the end so that the ceiling stays
			// at the end and the floor at the beginning
			lightlevels.Insert(lightlevels.Count - 1, level);
		}
		
		// This resets this sector data and all sectors that require updating after me
		/*public void Reset()
		{
			if(isupdating) return;
			isupdating = true;

			// This is set to false so that this sector is rebuilt the next time it is needed!
			updated = false;

			// The visual sector associated is now outdated
			if(mode.VisualSectorExists(sector))
			{
				BaseVisualSector vs = (BaseVisualSector)mode.GetVisualSector(sector);
				vs.UpdateSectorGeometry(false);
			}
			
			// Also reset the sectors that depend on this sector
			foreach(KeyValuePair<Sector, bool> s in updatesectors)
			{
				SectorData sd = mode.GetSectorData(s.Key);
				sd.Reset();
			}

			isupdating = false;
		}*/

		//mxd. This marks this sector data and all sector datas that require updating as not updated
		public void Reset(bool resetneighbours)
		{
			if(isupdating) return;
			isupdating = true;

			// This is set to false so that this sector is rebuilt the next time it is needed!
			updated = false;

			// The visual sector associated is now outdated
			if(mode.VisualSectorExists(sector))
			{
				BaseVisualSector vs = (BaseVisualSector)mode.GetVisualSector(sector);
				vs.Changed = true;
			}

			// Reset the sectors that depend on this sector
			if(resetneighbours)
			{
				foreach(KeyValuePair<Sector, bool> s in updatesectors)
				{
					SectorData sd = mode.GetSectorDataEx(s.Key);
					if(sd != null) sd.Reset(s.Value);
				}
			}

			isupdating = false;
		}

		// This sets up the basic floor and ceiling, as they would be in normal Doom circumstances
		private void BasicSetup()
		{
			//mxd
			if(sector.FloorSlope.GetLengthSq() > 0 && !double.IsNaN(sector.FloorSlopeOffset / sector.FloorSlope.z)) 
			{
				// Sloped plane
				floor.plane = new Plane(sector.FloorSlope, sector.FloorSlopeOffset);
			} 
			else 
			{
				// Normal (flat) floor plane
				floor.plane = new Plane(new Vector3D(0, 0, 1), -sector.FloorHeight);
			}

			if(sector.CeilSlope.GetLengthSq() > 0 && !double.IsNaN(sector.CeilSlopeOffset / sector.CeilSlope.z)) 
			{
				// Sloped plane
				ceiling.plane = new Plane(sector.CeilSlope, sector.CeilSlopeOffset);
			} 
			else 
			{
				// Normal (flat) ceiling plane
				ceiling.plane = new Plane(new Vector3D(0, 0, -1), sector.CeilHeight);
			}
			
			// Fetch ZDoom fields
			int color = sector.Fields.GetValue("lightcolor", -1);
			lightfloor = sector.Fields.GetValue("lightfloor", 0);
			lightfloorabsolute = sector.Fields.GetValue("lightfloorabsolute", false);
			lightceiling = sector.Fields.GetValue("lightceiling", 0);
			lightceilingabsolute = sector.Fields.GetValue("lightceilingabsolute", false);
            if (!lightfloorabsolute) lightfloor = sector.Brightness + lightfloor;
            if (!lightceilingabsolute) lightceiling = sector.Brightness + lightceiling;

            // Determine colors & light levels
            // [ZZ] Doom64 lighting
            //
            // ceiling/floor
            ColorCeiling = PixelColor.FromInt(sector.Fields.GetValue("color_ceiling", PixelColor.INT_WHITE));
            ColorFloor = PixelColor.FromInt(sector.Fields.GetValue("color_floor", PixelColor.INT_WHITE));
            ColorSprites = PixelColor.FromInt(sector.Fields.GetValue("color_sprites", PixelColor.INT_WHITE));
            ColorWallTop = PixelColor.FromInt(sector.Fields.GetValue("color_walltop", PixelColor.INT_WHITE));
            ColorWallBottom = PixelColor.FromInt(sector.Fields.GetValue("color_wallbottom", PixelColor.INT_WHITE));

            PixelColor floorbrightness = PixelColor.FromInt(mode.CalculateBrightness(lightfloor));
            PixelColor ceilingbrightness = PixelColor.FromInt(mode.CalculateBrightness(lightceiling));
            PixelColor lightcolor = PixelColor.FromInt(color);
            PixelColor floorcolor = PixelColor.Modulate(ColorFloor, PixelColor.Modulate(lightcolor, floorbrightness));
            PixelColor ceilingcolor = PixelColor.Modulate(ColorCeiling, PixelColor.Modulate(lightcolor, ceilingbrightness));
            floor.color = floorcolor.WithAlpha(255).ToInt();
            floor.brightnessbelow = sector.Brightness;
            floor.colorbelow = lightcolor.WithAlpha(255);
			floor.d64color = ColorFloor;
            ceiling.color = ceilingcolor.WithAlpha(255).ToInt();
            ceiling.brightnessbelow = sector.Brightness;
            ceiling.colorbelow = lightcolor.WithAlpha(255);
			ceiling.d64color = ColorCeiling;

            //mxd. Store a copy of initial settings
            floor.CopyProperties(floorbase);
			ceiling.CopyProperties(ceilingbase);

			//mxd. We need sector brightness here, unaffected by custom ceiling brightness...
			ceilingbase.brightnessbelow = sector.Brightness;
			ceilingbase.color = PixelColor.FromInt(mode.CalculateBrightness(sector.Brightness)).WithAlpha(255).ToInt();

			//mxd
			glowingflateffect.Update();
        }

		//mxd
		public void UpdateForced() 
		{
			updated = false;
			Update();
		}

		// When no geometry has been changed and no effects have been added or removed,
		// you can call this again to update existing effects. The effects will update
		// the existing SectorLevels to match with any changes.
		public void Update()
		{
			if(isupdating || updated) return;
			isupdating = true;
			
			// Set floor/ceiling to their original setup
			BasicSetup();

			// Update all effects
			foreach(SectorEffect e in alleffects) e.Update();
			
			//mxd. Do complicated light level shenanigans only when there are extrafloors
			if(lightlevels.Count > 2)
			{
				// Sort the levels
				SectorLevelComparer comparer = new SectorLevelComparer(sector);
				lightlevels.Sort(0, lightlevels.Count, comparer);

				// Now that we know the levels in this sector (and in the right order)
				// we can determine the lighting in between and on the levels.
				SectorLevel stored = ceilingbase;

				//mxd. Special cases...
				if(lightlevels[lightlevels.Count - 1].disablelighting)
				{
					lightlevels[lightlevels.Count - 1].colorbelow = stored.colorbelow;
					lightlevels[lightlevels.Count - 1].brightnessbelow = stored.brightnessbelow;
					lightlevels[lightlevels.Count - 1].color = GetLevelColor(stored, lightlevels[lightlevels.Count - 1]);
				}

				//mxd. Cast light properties from top to bottom
				for(int i = lightlevels.Count - 2; i >= 0; i--)
				{
					SectorLevel l = lightlevels[i];
					SectorLevel pl = lightlevels[i + 1];

					// Glow levels don't cast light
					if(pl.type == SectorLevelType.Glow && lightlevels.Count > i + 2) pl = lightlevels[i + 2];

					if(l.lighttype == LightLevelType.TYPE1)
					{
						stored = pl;
					}
					// Use stored light params when "disablelighting" flag is set
					else if(l.disablelighting)
					{
						l.colorbelow = stored.colorbelow;
						l.brightnessbelow = stored.brightnessbelow;
						l.color = GetLevelColor(stored, l);
					}
					else if(l.restrictlighting)
					{
						if(!pl.restrictlighting && pl != ceiling) stored = pl;
						l.color = GetLevelColor(stored, l);

						// This is the bottom side of extrafloor with "restrict lighting" flag. Make it cast stored light props. 
						if(l.type == SectorLevelType.Ceiling)
						{
							// Special case: 2 intersecting extrafloors with "restrictlighting" flag...
							if(pl.restrictlighting && pl.type == SectorLevelType.Floor && pl.sector.Index != l.sector.Index)
							{
								// Use light and color settings from previous layer
								l.colorbelow = pl.colorbelow;
								l.brightnessbelow = pl.brightnessbelow;
								l.color = GetLevelColor(pl, l);

								// Also colorize previous layer using next higher level color 
								if(i + 2 < lightlevels.Count) pl.color = GetLevelColor(lightlevels[i + 2], pl);
							}
							else
							{
								l.colorbelow = stored.colorbelow;
								l.brightnessbelow = stored.brightnessbelow;
							}
						}
					}
					// Bottom TYPE1 border requires special handling...
					else if(l.lighttype == LightLevelType.TYPE1_BOTTOM)
					{
						// Use brightness and color from previous light level when it's between TYPE1 and TYPE1_BOTTOM levels
						if(pl.type == SectorLevelType.Light && pl.lighttype != LightLevelType.TYPE1)
						{
							l.brightnessbelow = pl.brightnessbelow;
							l.colorbelow = pl.colorbelow;
						}
						// Use brightness and color from the light level above TYPE1 level
						else if(stored.type == SectorLevelType.Light)
						{
							l.brightnessbelow = stored.brightnessbelow;
							l.colorbelow = stored.colorbelow;
						}
						// Otherwise light values from the real ceiling are used 
					}
					else if(l.lighttype == LightLevelType.UNKNOWN)
					{
						// Use stored light level when previous one has "disablelighting" flag
						// or is the lower boundary of an extrafloor with "restrictlighting" flag
						SectorLevel src = (pl.disablelighting || (pl.restrictlighting && pl.type == SectorLevelType.Ceiling) ? stored : pl);

						// Don't change real ceiling light when previous level has "disablelighting" flag
						// Don't change anything when light properties were reset before hitting floor (otherwise floor UDMF brightness will be lost)
						if((src == ceilingbase && l == ceiling)
							|| (src == ceiling && l == floor && src.LightPropertiesMatch(ceilingbase)))
							continue;
						
						// Transfer color and brightness if previous level has them
						if(src.colorbelow.a > 0 && src.brightnessbelow != -1)
						{
							// Only surface brightness is retained when a glowing flat is used as extrafloor texture
							if(!l.affectedbyglow) l.color = GetLevelColor(src, l);

							// Transfer brightnessbelow and colorbelow if current level is not extrafloor top
							if(!(l.extrafloor != null && l.type == SectorLevelType.Floor))
							{
								l.brightnessbelow = src.brightnessbelow;
								l.colorbelow = src.colorbelow;
							}
						}

						// Store bottom extrafloor level if it doesn't have "restrictlighting" or "restrictlighting" flags set
						if(l.extrafloor != null && l.type == SectorLevelType.Ceiling && !l.restrictlighting && !l.disablelighting) stored = l;
					}

					// Reset lighting?
					if(l.resetlighting) stored = ceilingbase;
				}
			}

			//mxd. Apply ceiling glow effect?
			if(CeilingGlow != null && CeilingGlow.Fullbright)
			{
				ceiling.color = PixelColor.INT_WHITE;
			}

			//mxd. Apply floor glow effect?
			if(FloorGlow != null)
			{
				// Update floor color
				if(FloorGlow.Fullbright) floor.color = PixelColor.INT_WHITE;

				// Update brightness
				floor.brightnessbelow = (FloorGlow.Fullbright ? 255 : Math.Max(128, floor.brightnessbelow));
				
				if(floor.colorbelow.ToInt() == 0)
				{
					byte bb = (byte)floor.brightnessbelow;
					floor.colorbelow = new PixelColor(255, bb, bb, bb);
				}
			}

			//mxd
			floor.affectedbyglow = (FloorGlow != null);
			ceiling.affectedbyglow = (CeilingGlow != null);

			floorchanged = false;
			ceilingchanged = false;
			updated = true;
			isupdating = false;
		}

		// This returns the level above the given point
		public SectorLevel GetLevelAbove(Vector3D pos)
		{
			SectorLevel found = null;
			double dist = double.MaxValue;
			
			foreach(SectorLevel l in lightlevels)
			{
				double d = l.plane.GetZ(pos) - pos.z;
				if((d > 0.0f) && (d < dist))
				{
					dist = d;
					found = l;
				}
			}
			
			return found;
		}

		//mxd. This returns the level above the given point or the level given point is located on
		public SectorLevel GetLevelAboveOrAt(Vector3D pos)
		{
			SectorLevel found = null;
			double dist = double.MaxValue;

			foreach(SectorLevel l in lightlevels) 
			{
				double d = l.plane.GetZ(pos) - pos.z;
				if((d >= 0.0f) && (d < dist)) 
				{
					dist = d;
					found = l;
				}
			}

			return found;
		}

		// This returns the level above the given point
		public SectorLevel GetCeilingAbove(Vector3D pos)
		{
			SectorLevel found = null;
			double dist = double.MaxValue;

			foreach(SectorLevel l in lightlevels)
			{
				if(l.type == SectorLevelType.Ceiling)
				{
					double d = l.plane.GetZ(pos) - pos.z;
					if((d > 0.0f) && (d < dist))
					{
						dist = d;
						found = l;
					}
				}
			}

			return found;
		}

		// This returns the level below the given point
		public SectorLevel GetLevelBelow(Vector3D pos)
		{
			SectorLevel found = null;
			double dist = double.MaxValue;

			foreach(SectorLevel l in lightlevels)
			{
				double d = pos.z - l.plane.GetZ(pos);
				if((d > 0.0f) && (d < dist))
				{
					dist = d;
					found = l;
				}
			}

			return found;
		}

		// This returns the floor below the given point
		public SectorLevel GetFloorBelow(Vector3D pos)
		{
			SectorLevel found = null;
			double dist = double.MaxValue;

			foreach(SectorLevel l in lightlevels)
			{
				if(l.type == SectorLevelType.Floor)
				{
					double d = pos.z - l.plane.GetZ(pos);
					if((d > 0.0f) && (d < dist))
					{
						dist = d;
						found = l;
					}
				}
			}

			return found;
		}

		//mxd
		private int GetLevelColor(SectorLevel src, SectorLevel target)
		{
			PixelColor brightness;
			if(lightfloorabsolute && target == floor)
				brightness = PixelColor.FromInt(mode.CalculateBrightness(lightfloor));
			else if(lightceilingabsolute && target == ceiling)
				brightness = PixelColor.FromInt(mode.CalculateBrightness(lightceiling));
			else
				brightness = PixelColor.FromInt(mode.CalculateBrightness(src.brightnessbelow));

			PixelColor color = PixelColor.Modulate(target.d64color, PixelColor.Modulate(src.colorbelow, brightness));
			return color.WithAlpha(255).ToInt();
		}
		
		#endregion
	}
}
