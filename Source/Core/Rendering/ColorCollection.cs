
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

using System;
using System.Globalization;
using System.Drawing;
using CodeImp.DoomBuilder.Data;
using Configuration = CodeImp.DoomBuilder.IO.Configuration;

#endregion

namespace CodeImp.DoomBuilder.Rendering
{
	public sealed class ColorCollection
	{
		#region ================== Constants

		// Assist color creation
		private const float BRIGHT_MULTIPLIER = 1.0f;
		private const float BRIGHT_ADDITION = 0.4f;
		private const float DARK_MULTIPLIER = 0.9f;
		private const float DARK_ADDITION = -0.2f;

		// Palette size
		private const int NUM_COLORS = 53;
		public const int NUM_THING_COLORS = 20;
		public const int THING_COLORS_OFFSET = 20;

		// Colors!
		public const int BACKGROUND = 0;
		public const int VERTICES = 1;
		public const int LINEDEFS = 2;
		public const int MODELWIRECOLOR = 3; //mxd
		public const int INFOLINECOLOR = 4; //mxd
		public const int HIGHLIGHT = 5;
		public const int SELECTION = 6;
		public const int INDICATION = 7;
		public const int GRID = 8;
		public const int GRID64 = 9;
		public const int CROSSHAIR3D = 10;
		public const int HIGHLIGHT3D = 11;
		public const int SELECTION3D = 12;
		public const int SCRIPTBACKGROUND = 13;
		public const int LINENUMBERS = 14;
		public const int PLAINTEXT = 15;
		public const int COMMENTS = 16;
		public const int KEYWORDS = 17;
		public const int LITERALS = 18;
		public const int CONSTANTS = 19;
		public const int THINGCOLOR00 = 20;
		public const int THINGCOLOR01 = 21;
		public const int THINGCOLOR02 = 22;
		public const int THINGCOLOR03 = 23;
		public const int THINGCOLOR04 = 24;
		public const int THINGCOLOR05 = 25;
		public const int THINGCOLOR06 = 26;
		public const int THINGCOLOR07 = 27;
		public const int THINGCOLOR08 = 28;
		public const int THINGCOLOR09 = 29;
		public const int THINGCOLOR10 = 30;
		public const int THINGCOLOR11 = 31;
		public const int THINGCOLOR12 = 32;
		public const int THINGCOLOR13 = 33;
		public const int THINGCOLOR14 = 34;
		public const int THINGCOLOR15 = 35;
		public const int THINGCOLOR16 = 36;
		public const int THINGCOLOR17 = 37;
		public const int THINGCOLOR18 = 38;
		public const int THINGCOLOR19 = 39;
		public const int THREEDFLOORCOLOR = 40; //mxd
		public const int SCRIPTINDICATOR = 41; //mxd. Additional Script Editor colors
		public const int SCRIPTBRACEHIGHLIGHT = 42;
		public const int SCRIPTBADBRACEHIGHLIGHT = 43;
		public const int SCRIPTWHITESPACE = 44;
		public const int SCRIPTSELECTIONFORE = 45;
		public const int SCRIPTSELECTIONBACK = 46;
		public const int STRINGS = 47;
		public const int INCLUDES = 48;
		public const int SCRIPTFOLDFORE = 49;
		public const int SCRIPTFOLDBACK = 50;
		public const int PROPERTIES = 51;
		public const int GUIDELINECOLOR = 52; //mxd
		
		#endregion

		#region ================== Variables

		// Colors
		private readonly PixelColor[] colors;
		private readonly PixelColor[] brightcolors;
		private readonly PixelColor[] darkcolors;
		
		// Color-correction table
		private byte[] correctiontable;
		
		#endregion
		
		#region ================== Properties
		
		public PixelColor[] Colors { get { return colors; } }
		public PixelColor[] BrightColors { get { return brightcolors; } }
		public PixelColor[] DarkColors { get { return darkcolors; } }
		
		public PixelColor Background { get { return colors[BACKGROUND]; } internal set { colors[BACKGROUND] = value; } }
		public PixelColor Vertices { get { return colors[VERTICES]; } internal set { colors[VERTICES] = value; } }
		public PixelColor Linedefs { get { return colors[LINEDEFS]; } internal set { colors[LINEDEFS] = value; } }
		public PixelColor Highlight { get { return colors[HIGHLIGHT]; } internal set { colors[HIGHLIGHT] = value; } }
		public PixelColor Selection { get { return colors[SELECTION]; } internal set { colors[SELECTION] = value; } }
		public PixelColor Indication { get { return colors[INDICATION]; } internal set { colors[INDICATION] = value; } }
		public PixelColor Grid { get { return colors[GRID]; } internal set { colors[GRID] = value; } }
		public PixelColor Grid64 { get { return colors[GRID64]; } internal set { colors[GRID64] = value; } }

		//mxd
		public PixelColor ModelWireframe { get { return colors[MODELWIRECOLOR]; } internal set { colors[MODELWIRECOLOR] = value; } }
		public PixelColor InfoLine { get { return colors[INFOLINECOLOR]; } internal set { colors[INFOLINECOLOR] = value; } }
		public PixelColor Guideline { get { return colors[GUIDELINECOLOR]; } internal set { colors[GUIDELINECOLOR] = value; } }
		public PixelColor ThreeDFloor { get { return colors[THREEDFLOORCOLOR]; } internal set { colors[THREEDFLOORCOLOR] = value;} }
		
		public PixelColor Crosshair3D { get { return colors[CROSSHAIR3D]; } internal set { colors[CROSSHAIR3D] = value; } }
		public PixelColor Highlight3D { get { return colors[HIGHLIGHT3D]; } internal set { colors[HIGHLIGHT3D] = value; } }
		public PixelColor Selection3D { get { return colors[SELECTION3D]; } internal set { colors[SELECTION3D] = value; } }
		
		public PixelColor ScriptBackground { get { return colors[SCRIPTBACKGROUND]; } internal set { colors[SCRIPTBACKGROUND] = value; } }
		public PixelColor ScriptIndicator { get { return colors[SCRIPTINDICATOR]; } internal set { colors[SCRIPTINDICATOR] = value; } } //mxd
		public PixelColor ScriptBraceHighlight { get { return colors[SCRIPTBRACEHIGHLIGHT]; } internal set { colors[SCRIPTBRACEHIGHLIGHT] = value; } } //mxd
		public PixelColor ScriptBadBraceHighlight { get { return colors[SCRIPTBADBRACEHIGHLIGHT]; } internal set { colors[SCRIPTBADBRACEHIGHLIGHT] = value; } } //mxd
		public PixelColor ScriptWhitespace { get { return colors[SCRIPTWHITESPACE]; } internal set { colors[SCRIPTWHITESPACE] = value; } } //mxd
		public PixelColor ScriptSelectionForeColor { get { return colors[SCRIPTSELECTIONFORE]; } internal set { colors[SCRIPTSELECTIONFORE] = value; } } //mxd
		public PixelColor ScriptSelectionBackColor { get { return colors[SCRIPTSELECTIONBACK]; } internal set { colors[SCRIPTSELECTIONBACK] = value; } } //mxd
		public PixelColor LineNumbers { get { return colors[LINENUMBERS]; } internal set { colors[LINENUMBERS] = value; } }
		public PixelColor PlainText { get { return colors[PLAINTEXT]; } internal set { colors[PLAINTEXT] = value; } }
		public PixelColor Comments { get { return colors[COMMENTS]; } internal set { colors[COMMENTS] = value; } }
		public PixelColor Keywords { get { return colors[KEYWORDS]; } internal set { colors[KEYWORDS] = value; } }
		public PixelColor Properties { get { return colors[PROPERTIES]; } internal set { colors[PROPERTIES] = value; } }
		public PixelColor Literals { get { return colors[LITERALS]; } internal set { colors[LITERALS] = value; } }
		public PixelColor Constants { get { return colors[CONSTANTS]; } internal set { colors[CONSTANTS] = value; } }
		public PixelColor Strings { get { return colors[STRINGS]; } internal set { colors[STRINGS] = value; } } //mxd
		public PixelColor Includes { get { return colors[INCLUDES]; } internal set { colors[INCLUDES] = value; } } //mxd
		public PixelColor ScriptFoldForeColor { get { return colors[SCRIPTFOLDFORE]; } internal set { colors[SCRIPTFOLDFORE] = value; } } //mxd
		public PixelColor ScriptFoldBackColor { get { return colors[SCRIPTFOLDBACK]; } internal set { colors[SCRIPTFOLDBACK] = value; } } //mxd
		
		#endregion

		#region ================== Constructor / Disposer

		// Constructor for settings from configuration
		internal ColorCollection(Configuration cfg)
		{
			// Initialize
			colors = new PixelColor[NUM_COLORS];
			brightcolors = new PixelColor[NUM_COLORS];
			darkcolors = new PixelColor[NUM_COLORS];
			
			// Read all colors from config
			for(int i = 0; i < NUM_COLORS; i++)
			{
				// Read color
				colors[i] = PixelColor.FromInt(cfg.ReadSetting("colors.color" + i.ToString(CultureInfo.InvariantCulture), 0));
			}

			//mxd. Set new colors (previously these were defined in GZBuilder.default.cfg)
			if(colors[BACKGROUND].ToInt() == 0) colors[BACKGROUND] = PixelColor.FromInt(-16777216);
			if(colors[VERTICES].ToInt() == 0) colors[VERTICES] = PixelColor.FromInt(-11425537);
			if(colors[LINEDEFS].ToInt() == 0) colors[LINEDEFS] = PixelColor.FromInt(-1);
			if(colors[MODELWIRECOLOR].ToInt() == 0) colors[MODELWIRECOLOR] = PixelColor.FromInt(-4259937);
			if(colors[INFOLINECOLOR].ToInt() == 0) colors[INFOLINECOLOR] = PixelColor.FromInt(-3750145);
			if(colors[HIGHLIGHT].ToInt() == 0) colors[HIGHLIGHT] = PixelColor.FromInt(-21504);
			if(colors[SELECTION].ToInt() == 0) colors[SELECTION] = PixelColor.FromInt(-49152);
			if(colors[INDICATION].ToInt() == 0) colors[INDICATION] = PixelColor.FromInt(-128);
			if(colors[GRID].ToInt() == 0) colors[GRID] = PixelColor.FromInt(-12171706);
			if(colors[GRID64].ToInt() == 0) colors[GRID64] = PixelColor.FromInt(-13018769);
			if(colors[CROSSHAIR3D].ToInt() == 0) colors[CROSSHAIR3D] = PixelColor.FromInt(-16711681); // Unused!
			if(colors[HIGHLIGHT3D].ToInt() == 0) colors[HIGHLIGHT3D] = PixelColor.FromInt(-24576);
			if(colors[SELECTION3D].ToInt() == 0) colors[SELECTION3D] = PixelColor.FromInt(-49152);
			if(colors[SCRIPTBACKGROUND].ToInt() == 0) colors[SCRIPTBACKGROUND] = PixelColor.FromInt(-1);
			if(colors[LINENUMBERS].ToInt() == 0) colors[LINENUMBERS] = PixelColor.FromInt(-13921873);
			if(colors[PLAINTEXT].ToInt() == 0) colors[PLAINTEXT] = PixelColor.FromInt(-16777216);
			if(colors[COMMENTS].ToInt() == 0) colors[COMMENTS] = PixelColor.FromInt(-16744448);
			if(colors[KEYWORDS].ToInt() == 0) colors[KEYWORDS] = PixelColor.FromInt(-16741493);
			if(colors[LITERALS].ToInt() == 0) colors[LITERALS] = PixelColor.FromInt(-16776999);
			if(colors[CONSTANTS].ToInt() == 0) colors[CONSTANTS] = PixelColor.FromInt(-8372160);
			if(colors[GUIDELINECOLOR].ToInt() == 0) colors[GUIDELINECOLOR] = PixelColor.FromInt(-256);

			// Set new thing colors
			if(colors[THINGCOLOR00].ToInt() == 0) colors[THINGCOLOR00] = PixelColor.FromColor(Color.DimGray);
			if(colors[THINGCOLOR01].ToInt() == 0) colors[THINGCOLOR01] = PixelColor.FromColor(Color.RoyalBlue);
			if(colors[THINGCOLOR02].ToInt() == 0) colors[THINGCOLOR02] = PixelColor.FromColor(Color.ForestGreen);
			if(colors[THINGCOLOR03].ToInt() == 0) colors[THINGCOLOR03] = PixelColor.FromColor(Color.LightSeaGreen);
			if(colors[THINGCOLOR04].ToInt() == 0) colors[THINGCOLOR04] = PixelColor.FromColor(Color.Firebrick);
			if(colors[THINGCOLOR05].ToInt() == 0) colors[THINGCOLOR05] = PixelColor.FromColor(Color.DarkViolet);
			if(colors[THINGCOLOR06].ToInt() == 0) colors[THINGCOLOR06] = PixelColor.FromColor(Color.DarkGoldenrod);
			if(colors[THINGCOLOR07].ToInt() == 0) colors[THINGCOLOR07] = PixelColor.FromColor(Color.Silver);
			if(colors[THINGCOLOR08].ToInt() == 0) colors[THINGCOLOR08] = PixelColor.FromColor(Color.Gray);
			if(colors[THINGCOLOR09].ToInt() == 0) colors[THINGCOLOR09] = PixelColor.FromColor(Color.DeepSkyBlue);
			if(colors[THINGCOLOR10].ToInt() == 0) colors[THINGCOLOR10] = PixelColor.FromColor(Color.LimeGreen);
			if(colors[THINGCOLOR11].ToInt() == 0) colors[THINGCOLOR11] = PixelColor.FromColor(Color.PaleTurquoise);
			if(colors[THINGCOLOR12].ToInt() == 0) colors[THINGCOLOR12] = PixelColor.FromColor(Color.Tomato);
			if(colors[THINGCOLOR13].ToInt() == 0) colors[THINGCOLOR13] = PixelColor.FromColor(Color.Violet);
			if(colors[THINGCOLOR14].ToInt() == 0) colors[THINGCOLOR14] = PixelColor.FromColor(Color.Yellow);
			if(colors[THINGCOLOR15].ToInt() == 0) colors[THINGCOLOR15] = PixelColor.FromColor(Color.WhiteSmoke);
			if(colors[THINGCOLOR16].ToInt() == 0) colors[THINGCOLOR16] = PixelColor.FromColor(Color.LightPink);
			if(colors[THINGCOLOR17].ToInt() == 0) colors[THINGCOLOR17] = PixelColor.FromColor(Color.DarkOrange);
			if(colors[THINGCOLOR18].ToInt() == 0) colors[THINGCOLOR18] = PixelColor.FromColor(Color.DarkKhaki);
			if(colors[THINGCOLOR19].ToInt() == 0) colors[THINGCOLOR19] = PixelColor.FromColor(Color.Goldenrod);

			//mxd. Set the rest of new colors (previously these were also defined in GZBuilder.default.cfg)
			if(colors[THREEDFLOORCOLOR].ToInt() == 0) colors[THREEDFLOORCOLOR] = PixelColor.FromInt(-65536);
			if(colors[SCRIPTINDICATOR].ToInt() == 0) colors[SCRIPTINDICATOR] = PixelColor.FromInt(-16711936);
			if(colors[SCRIPTBRACEHIGHLIGHT].ToInt() == 0) colors[SCRIPTBRACEHIGHLIGHT] = PixelColor.FromInt(-16711681);
			if(colors[SCRIPTBADBRACEHIGHLIGHT].ToInt() == 0) colors[SCRIPTBADBRACEHIGHLIGHT] = PixelColor.FromInt(-65536);
			if(colors[SCRIPTWHITESPACE].ToInt() == 0) colors[SCRIPTWHITESPACE] = PixelColor.FromInt(-8355712);
			if(colors[SCRIPTSELECTIONFORE].ToInt() == 0) colors[SCRIPTSELECTIONFORE] = PixelColor.FromInt(-1);
			if(colors[SCRIPTSELECTIONBACK].ToInt() == 0) colors[SCRIPTSELECTIONBACK] = PixelColor.FromInt(-13395457);
			if(colors[STRINGS].ToInt() == 0) colors[STRINGS] = PixelColor.FromInt(-8388608);
			if(colors[INCLUDES].ToInt() == 0) colors[INCLUDES] = PixelColor.FromInt(-9868951);
			if(colors[SCRIPTFOLDFORE].ToInt() == 0) colors[SCRIPTFOLDFORE] = PixelColor.FromColor(SystemColors.ControlDark);
			if(colors[SCRIPTFOLDBACK].ToInt() == 0) colors[SCRIPTFOLDBACK] = PixelColor.FromColor(SystemColors.ControlLightLight);
			if(colors[PROPERTIES].ToInt() == 0) colors[PROPERTIES] = PixelColor.FromInt(-16752191);

			// Create assist colors
			CreateAssistColors();
			
			// Create color correction table
			CreateCorrectionTable();
			
			// We have no destructor
			GC.SuppressFinalize(this);
		}

		#endregion

		#region ================== Methods
		
		// This generates a color-correction table
		internal void CreateCorrectionTable()
		{
			// Determine amounts
			float gamma = (General.Settings.ImageBrightness + 10) * 0.1f;
			float bright = General.Settings.ImageBrightness * 5f;
			
			// Make table
			correctiontable = new byte[256];
			
			// Fill table
			for(int i = 0; i < 256; i++)
			{
				byte b;
				float a = i * gamma + bright;
				if(a < 0f) b = 0; else if(a > 255f) b = 255; else b = (byte)a;
				correctiontable[i] = b;
			}
		}
		
		// This applies color-correction over a block of pixel data
		internal unsafe void ApplyColorCorrection(PixelColor* pixels, int numpixels)
		{
			for(PixelColor* cp = pixels + numpixels - 1; cp >= pixels; cp--)
			{
				cp->r = correctiontable[cp->r];
				cp->g = correctiontable[cp->g];
				cp->b = correctiontable[cp->b];
			}
		}

		// This quantizes an image to a PLAYPAL lump, putting indices into an array of integers.
		internal unsafe void QuantizeColorsToPlaypal(PixelColor* inPixels, int[] indices, Playpal playpal)
		{
			System.Threading.Tasks.Parallel.For(0, indices.Length, (i) => indices[i] = playpal.FindClosestColor(inPixels[i]));
		}
		
		// This clamps a value between 0 and 1
		private static float Saturate(float v)
		{
			if(v < 0f) return 0f; else if(v > 1f) return 1f; else return v;
		}

		// This creates assist colors
		internal void CreateAssistColors()
		{
			// Go for all colors
			for(int i = 0; i < NUM_COLORS; i++)
			{
				// Create assist colors
				brightcolors[i] = CreateBrightVariant(colors[i]);
				darkcolors[i] = CreateDarkVariant(colors[i]);
			}
		}
		
		// This creates a brighter color
		public PixelColor CreateBrightVariant(PixelColor pc)
		{
			Color4 o = pc.ToColorValue();
			Color4 c = new Color4(0f, 0f, 0f, 1f);
						
			// Create brighter color
			c.Red = Saturate(o.Red * BRIGHT_MULTIPLIER + BRIGHT_ADDITION);
			c.Green = Saturate(o.Green * BRIGHT_MULTIPLIER + BRIGHT_ADDITION);
			c.Blue = Saturate(o.Blue * BRIGHT_MULTIPLIER + BRIGHT_ADDITION);
			return PixelColor.FromInt(c.ToArgb());
		}

		// This creates a darker color
		public PixelColor CreateDarkVariant(PixelColor pc)
		{
			Color4 o = pc.ToColorValue();
			Color4 c = new Color4(0f, 0f, 0f, 1f);

			// Create darker color
			c.Red = Saturate(o.Red * DARK_MULTIPLIER + DARK_ADDITION);
			c.Green = Saturate(o.Green * DARK_MULTIPLIER + DARK_ADDITION);
			c.Blue = Saturate(o.Blue * DARK_MULTIPLIER + DARK_ADDITION);
			return PixelColor.FromInt(c.ToArgb());
		}

		// This saves colors to configuration
		internal void SaveColors(Configuration cfg)
		{
			// Write all colors to config
			for(int i = 0; i < NUM_COLORS; i++)
			{
				// Write color
				cfg.WriteSetting("colors.color" + i.ToString(CultureInfo.InvariantCulture), colors[i].ToInt());
			}
		}
		
		#endregion
	}
}
