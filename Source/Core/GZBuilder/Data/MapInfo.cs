#region ================== Namespaces

using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.Rendering;

#endregion

namespace CodeImp.DoomBuilder.GZBuilder.Data 
{
	public sealed class MapInfo
	{
		#region ================== Enums

		public enum GZDoomLightMode
		{
			STANDARD,
			BRIGHT,
			DOOM,
			DARK,
			LEGACY,
			SOFTWARE,
			UNDEFINED
		}

		#endregion

		#region ================== Variables

		private bool isdefined;

		private string title;
		private string sky1;
		private float sky1scrollspeed;
		private string sky2;
		private float sky2scrollspeed;
		private bool doublesky;
		private bool hasfadecolor;
		private Color4 fadecolor;
		private bool hasoutsidefogcolor;
		private Color4 outsidefogcolor;
		private int fogdensity;
		private int outsidefogdensity;

		private bool evenlighting;
		private bool smoothlighting;
		private GZDoomLightMode lightmode;
		private int vertwallshade;
		private int horizwallshade;

        // [ZZ]
        private float pixelratio;

		// biwa
		private bool forceworldpanning;

		private string lightAttenuationMode;

		#endregion

		#region ================== Properties

		public bool IsDefined { get { return isdefined; } internal set { isdefined = value; } }

		public string Title { get { return title; } internal set { title = value; isdefined = true; } }
		public string Sky1 { get { return sky1; } internal set { sky1 = value; isdefined = true; } }
		public float Sky1ScrollSpeed { get { return sky1scrollspeed; } internal set { sky1scrollspeed = value; isdefined = true; } }
		public string Sky2 { get { return sky2; } internal set { sky2 = value; isdefined = true; } }
		public float Sky2ScrollSpeed { get { return sky2scrollspeed; } internal set { sky2scrollspeed = value; isdefined = true; } }
		public bool DoubleSky { get { return doublesky; } internal set { doublesky = value; isdefined = true; } }
		public bool HasFadeColor { get { return hasfadecolor; } internal set { hasfadecolor = value; isdefined = true; } }
		public Color4 FadeColor { get { return fadecolor; } internal set { fadecolor = value; isdefined = true; } }
		public bool HasOutsideFogColor { get { return hasoutsidefogcolor; } internal set { hasoutsidefogcolor = value; isdefined = true; } }
		public Color4 OutsideFogColor { get { return outsidefogcolor; } internal set { outsidefogcolor = value; isdefined = true; } }
		public int FogDensity { get { return fogdensity; } internal set { fogdensity = value; isdefined = true; } }
		public int OutsideFogDensity { get { return outsidefogdensity; } internal set { outsidefogdensity = value; isdefined = true; } }

		public bool EvenLighting { get { return evenlighting; } internal set { evenlighting = value; isdefined = true; } }
		public bool SmoothLighting { get { return smoothlighting; } internal set { smoothlighting = value; isdefined = true; } }
		public GZDoomLightMode LightMode { get { return lightmode; } internal set { lightmode = value; isdefined = true; } }
		public int VertWallShade { get { return vertwallshade; } internal set { vertwallshade = value; isdefined = true; } }
		public int HorizWallShade { get { return horizwallshade; } internal set { horizwallshade = value; isdefined = true; } }

        // [ZZ]
        public float PixelRatio { get { return pixelratio; } internal set { pixelratio = value; isdefined = true; } }

		// biwa
		public bool ForceWorldPanning { get { return forceworldpanning; } internal set { forceworldpanning = value; isdefined = true; } }
		public string LightAttenuationMode { get { return lightAttenuationMode; } internal set { lightAttenuationMode = value; isdefined = true; } }

		#endregion

		#region ================== Constructor

		public MapInfo() 
		{
			vertwallshade = 16;
			horizwallshade = -16;
			fogdensity = 255;
			outsidefogdensity = 255;
			lightmode = GZDoomLightMode.UNDEFINED;
            pixelratio = DataManager.DOOM_PIXEL_RATIO;
		}

		#endregion
	}
}
