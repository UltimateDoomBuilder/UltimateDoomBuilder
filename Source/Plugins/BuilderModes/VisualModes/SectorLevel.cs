#region === Copyright (c) 2010 Pascal van der Heiden ===

using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Rendering;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	internal class SectorLevel
	{
		// Type of level
		public SectorLevelType type;

		//mxd. Type of extralight
		public LightLevelType lighttype = LightLevelType.UNKNOWN;  

		// Sector where this level originates from
		public Sector sector;
		
		// Plane in the sector
		public Plane plane;
		
		// Alpha for translucency (255=opaque)
		public int alpha;
		
		// Color of the plane (includes brightness)
		// When this is 0, it takes the color from the sector above
		public int color;
		
		// Color and brightness below the plane
		// When this is 0, it takes the color from the sector above
		public int brightnessbelow;
		public PixelColor colorbelow;
		public PixelColor d64color; // own color of the plane
		public bool disablelighting; //mxd
		public bool restrictlighting; //mxd
		public bool resetlighting; //mxd
		public bool affectedbyglow; //mxd
		public Effect3DFloor extrafloor; //mxd
		public bool splitsides; //mxd
		
		// Constructor
		public SectorLevel(Sector s, SectorLevelType type)
		{
			this.type = type;
			this.sector = s;
			this.alpha = 255;
			this.splitsides = true; //mxd
		}
		
		// Copy constructor
		public SectorLevel(SectorLevel source)
		{
			source.CopyProperties(this);
		}

		// Copy properties
		public void CopyProperties(SectorLevel target)
		{
			target.sector = this.sector;
			target.type = this.type;
			target.lighttype = this.lighttype; //mxd
			target.plane = this.plane;
			target.alpha = this.alpha;
			target.color = this.color;
			target.brightnessbelow = this.brightnessbelow;
			target.colorbelow = this.colorbelow;
			target.d64color = this.d64color;
			target.affectedbyglow = this.affectedbyglow; //mxd
			target.disablelighting = this.disablelighting; //mxd
			target.restrictlighting = this.restrictlighting; //mxd
			target.resetlighting = this.resetlighting; //mxd
			target.splitsides = this.splitsides; //mxd
		}

		//mxd. Compare light properties
		public bool LightPropertiesMatch(SectorLevel other)
		{
			return (this.type == other.type && this.lighttype == other.lighttype && this.alpha == other.alpha && this.splitsides == other.splitsides
				&& this.color == other.color && this.brightnessbelow == other.brightnessbelow && this.colorbelow.ToInt() == other.colorbelow.ToInt()
				&& this.disablelighting == other.disablelighting && this.restrictlighting == other.restrictlighting 
				&& this.resetlighting == other.resetlighting);
		}

#if DEBUG
		//mxd. Handy when debugging
		public override string ToString()
		{
			switch(type)
			{
				case SectorLevelType.Ceiling: return (extrafloor != null ? "ExtraCeiling" : "Ceiling");
				case SectorLevelType.Floor: return (extrafloor != null ? "ExtraFloor" : "Floor");
				case SectorLevelType.Glow: return "Glow Level";
				case SectorLevelType.Light: return "Light Level (" + GetLightType() + ")";
				default: return "Unknown Level Type!!!";
			}
		}

		//mxd. Handy when debugging
		private string GetLightType()
		{
			switch(lighttype)
			{
				case LightLevelType.TYPE0: return "Type 0";
				case LightLevelType.TYPE1: return "Type 1";
				case LightLevelType.TYPE1_BOTTOM: return "Type 1 (bottom)";
				case LightLevelType.TYPE2: return "Type 2";
				default: return "Unknown";
			}
		}
#endif
	}
}
