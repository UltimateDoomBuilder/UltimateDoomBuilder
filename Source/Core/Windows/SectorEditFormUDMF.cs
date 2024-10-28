#region ================== Namespaces

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Controls;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Types;

#endregion

namespace CodeImp.DoomBuilder.Windows
{
	internal partial class SectorEditFormUDMF : DelayedForm
	{
		#region ================== Events

		public event EventHandler OnValuesChanged; //mxd

		#endregion

		#region ================== Constants

		private const string NO_SOUND_SEQUENCE = "None"; //mxd
		private const string NO_TERRAIN = "Default"; //mxd
		private const string NO_DAMAGETYPE = "None"; //mxd

		#endregion

		#region ================== Variables

		private ICollection<Sector> sectors;
		private Dictionary<Sector, SectorProperties> sectorprops; //mxd
		private bool preventchanges; //mxd
		private bool undocreated; //mxd
		private StepsList anglesteps; //mxd
		private readonly List<string> renderstyles; //mxd
		private readonly List<string> portalrenderstyles; //mxd
	
		//mxd. Slope pivots
		private Vector2D globalslopepivot;
		private Dictionary<Sector, Vector2D> slopepivots;

		private bool oldmapischanged;

		#endregion

		#region ================== Structs

		private struct SectorProperties //mxd
		{
			public readonly int Brightness;
			public readonly int FloorHeight;
			public readonly int CeilHeight;
			public readonly string FloorTexture;
			public readonly string CeilTexture;

			//UDMF stuff
			public readonly int LightColor;
			public readonly int FadeColor;
			public readonly int FogDensity;
			//public float Desaturation;

			//UDMF Ceiling
			public readonly double CeilOffsetX;
			public readonly double CeilOffsetY;
			public readonly double CeilScaleX;
			public readonly double CeilScaleY;
			//public float CeilAlpha;
			public readonly double CeilRotation;
			public readonly int CeilBrightness;
			public readonly bool CeilLightAbsoulte;
			public readonly int CeilGlowColor;
			public readonly double CeilGlowHeight;

			//UDMF Floor
			public readonly double FloorOffsetX;
			public readonly double FloorOffsetY;
			public readonly double FloorScaleX;
			public readonly double FloorScaleY;
			//public float FloorAlpha;
			public readonly double FloorRotation;
			public readonly int FloorBrightness;
			public readonly bool FloorLightAbsoulte;
			public readonly int FloorGlowColor;
			public readonly double FloorGlowHeight;

			//UDMF slopes. Angles are in degrees
			public readonly Vector3D FloorSlope;
			public readonly Vector3D CeilSlope;
			public readonly double FloorSlopeAngleXY;
			public readonly double FloorSlopeAngleZ;
			public readonly double FloorSlopeOffset;
			public readonly double CeilSlopeAngleXY;
			public readonly double CeilSlopeAngleZ;
			public readonly double CeilSlopeOffset;

            //[ZZ] UDMF Doom64 sector colors
            public readonly int D64ColorCeiling;
            public readonly int D64ColorWallTop;
            public readonly int D64ColorWallBottom;
            public readonly int D64ColorFloor;
            public readonly int D64ColorThings;

            public SectorProperties(Sector s) 
			{
				Brightness = s.Brightness;
				FloorHeight = s.FloorHeight;
				CeilHeight = s.CeilHeight;
				FloorTexture = s.FloorTexture;
				CeilTexture = s.CeilTexture;

				//UDMF stuff
				LightColor = UniFields.GetInteger(s.Fields, "lightcolor", PixelColor.INT_WHITE_NO_ALPHA);
				FadeColor = UniFields.GetInteger(s.Fields, "fadecolor", 0);
				FogDensity = UniFields.GetInteger(s.Fields, "fogdensity", 0);

				//UDMF Ceiling
				CeilOffsetX = UniFields.GetFloat(s.Fields, "xpanningceiling", 0.0);
				CeilOffsetY = UniFields.GetFloat(s.Fields, "ypanningceiling", 0.0);
				CeilScaleX = UniFields.GetFloat(s.Fields, "xscaleceiling", 1.0);
				CeilScaleY = UniFields.GetFloat(s.Fields, "yscaleceiling", 1.0);
				//CeilAlpha = UniFields.GetFloat(s.Fields, "alphaceiling", 1.0f);
				CeilRotation = s.Fields.GetValue("rotationceiling", 0.0);
				CeilBrightness = s.Fields.GetValue("lightceiling", 0);
				CeilLightAbsoulte = s.Fields.GetValue("lightceilingabsolute", false);
				CeilGlowColor = s.Fields.GetValue("ceilingglowcolor", 0);
				CeilGlowHeight = s.Fields.GetValue("ceilingglowheight", 0.0);

				//UDMF Floor
				FloorOffsetX = UniFields.GetFloat(s.Fields, "xpanningfloor", 0.0);
				FloorOffsetY = UniFields.GetFloat(s.Fields, "ypanningfloor", 0.0);
				FloorScaleX = UniFields.GetFloat(s.Fields, "xscalefloor", 1.0);
				FloorScaleY = UniFields.GetFloat(s.Fields, "yscalefloor", 1.0);
				//FloorAlpha = UniFields.GetFloat(s.Fields, "alphafloor", 1.0f);
				FloorRotation = s.Fields.GetValue("rotationfloor", 0.0);
				FloorBrightness = s.Fields.GetValue("lightfloor", 0);
				FloorLightAbsoulte = s.Fields.GetValue("lightfloorabsolute", false);
				FloorGlowColor = s.Fields.GetValue("floorglowcolor", 0);
				FloorGlowHeight = s.Fields.GetValue("floorglowheight", 0.0);

				//UDMF slopes
				if(s.FloorSlope.GetLengthSq() > 0)
				{
					FloorSlopeAngleXY = General.ClampAngle(Math.Round(Angle2D.RadToDeg(s.FloorSlope.GetAngleXY()) - 180, 1));
					FloorSlopeAngleZ = -Math.Round(Angle2D.RadToDeg(s.FloorSlope.GetAngleZ()) - 90, 1);
					FloorSlopeOffset = (double.IsNaN(s.FloorSlopeOffset) ? s.FloorHeight : s.FloorSlopeOffset);
				} 
				else 
				{
					FloorSlopeAngleXY = 0;
					FloorSlopeAngleZ = 0;
					FloorSlopeOffset = -s.FloorHeight;
				}
				FloorSlope = s.FloorSlope;

				if(s.CeilSlope.GetLengthSq() > 0)
				{
					CeilSlopeAngleXY = General.ClampAngle(Math.Round(Angle2D.RadToDeg(s.CeilSlope.GetAngleXY()) - 180, 1));
					CeilSlopeAngleZ = -Math.Round(270 - Angle2D.RadToDeg(s.CeilSlope.GetAngleZ()), 1);
					CeilSlopeOffset = (double.IsNaN(s.CeilSlopeOffset) ? s.CeilHeight : s.CeilSlopeOffset);
				} 
				else 
				{
					CeilSlopeAngleXY = 0;
					CeilSlopeAngleZ = 0;
					CeilSlopeOffset = s.CeilHeight;
				}

                CeilSlope = s.CeilSlope;

                D64ColorCeiling = s.Fields.GetValue("color_ceiling", PixelColor.INT_WHITE_NO_ALPHA);
                D64ColorWallTop = s.Fields.GetValue("color_walltop", PixelColor.INT_WHITE_NO_ALPHA);
                D64ColorThings = s.Fields.GetValue("color_sprites", PixelColor.INT_WHITE_NO_ALPHA);
                D64ColorWallBottom = s.Fields.GetValue("color_wallbottom", PixelColor.INT_WHITE_NO_ALPHA);
                D64ColorFloor = s.Fields.GetValue("color_floor", PixelColor.INT_WHITE_NO_ALPHA);
            }
		}

		#endregion

		#region ================== Constructor

		public SectorEditFormUDMF() 
		{
			InitializeComponent();

			DoUDMFControls(this);

			// Plane equation slopes are handled internally instead through the UDMF fields, so they need special attention
			EnableDisableControlAndChildren(gbCeilingSLope, General.Map.Config.PlaneEquationSupport);
			EnableDisableControlAndChildren(gbFloorSlope, General.Map.Config.PlaneEquationSupport);

			//mxd. Load settings
			if (General.Settings.StoreSelectedEditTab)
			{
				int activetab = General.Settings.ReadSetting("windows." + configname + ".activetab", 0);
				tabs.SelectTab(activetab);
			}

			// Fill flags list
			foreach(KeyValuePair<string, string> lf in General.Map.Config.SectorFlags)
				flags.Add(lf.Value, lf.Key);
			flags.Enabled = General.Map.Config.SectorFlags.Count > 0;

			// Fill floor protal flags list
			foreach(KeyValuePair<string, string> lf in General.Map.Config.FloorPortalFlags)
				floorportalflags.Add(lf.Value, lf.Key);
			floorportalflags.Enabled = General.Map.Config.FloorPortalFlags.Count > 0;

			// Fill ceiling protal flags list
			foreach(KeyValuePair<string, string> lf in General.Map.Config.CeilingPortalFlags)
				ceilportalflags.Add(lf.Value, lf.Key);
			ceilportalflags.Enabled = General.Map.Config.CeilingPortalFlags.Count > 0;

			// Setup renderstyles
			if(General.Map.Config.SectorRenderStyles.Count > 0)
			{
				string[] rskeys = new string[General.Map.Config.SectorRenderStyles.Count];
				General.Map.Config.SectorRenderStyles.Keys.CopyTo(rskeys, 0);
				renderstyles = new List<string>(rskeys);
			}
			else
			{
				renderstyles = new List<string>();
			}

			// Fill renderstyles
			foreach(string name in General.Map.Config.SectorRenderStyles.Values) 
			{
				floorRenderStyle.Items.Add(name);
				ceilRenderStyle.Items.Add(name);
			}

			// Setup portal renderstyles
			if(General.Map.Config.SectorPortalRenderStyles.Count > 0)
			{
				string[] rskeys = new string[General.Map.Config.SectorPortalRenderStyles.Count];
				General.Map.Config.SectorPortalRenderStyles.Keys.CopyTo(rskeys, 0);
				portalrenderstyles = new List<string>(rskeys);
			}
			else
			{
				portalrenderstyles = new List<string>();
			}

			// Fill portal renderstyles
			foreach(string name in General.Map.Config.SectorPortalRenderStyles.Values)
			{
				floorportalrenderstyle.Items.Add(name);
				ceilportalrenderstyle.Items.Add(name);
			}

			// Fill effects list
			effect.GeneralizedOptions = General.Map.Config.GenEffectOptions; //mxd
			effect.AddInfo(General.Map.Config.SortedSectorEffects.ToArray());

			// Fill sound sequences list
			soundsequence.Items.Add(NO_SOUND_SEQUENCE);
			soundsequence.Items.AddRange(General.Map.Data.SoundSequences);

			// Fill damagetype list
			damagetype.Items.Add(NO_DAMAGETYPE);
			damagetype.Items.AddRange(General.Map.Data.DamageTypes);

			// Fill terrain type lists
			ceilterrain.Items.Add(NO_TERRAIN);
			ceilterrain.Items.AddRange(General.Map.Data.TerrainNames);
			floorterrain.Items.Add(NO_TERRAIN);
			floorterrain.Items.AddRange(General.Map.Data.TerrainNames);

			// Initialize custom fields editor
			fieldslist.Setup("sector");

			// Fill universal fields list
			fieldslist.ListFixedFields(General.Map.Config.SectorFields);

			// Initialize image selectors
			floortex.Initialize();
			ceilingtex.Initialize();

			// Set steps for brightness field
			brightness.StepValues = General.Map.Config.BrightnessLevels;

			// Apply settings
			ceilScale.LinkValues = General.Settings.ReadSetting("windows." + configname + ".linkceilingscale", false);
			floorScale.LinkValues = General.Settings.ReadSetting("windows." + configname + ".linkfloorscale", false);

			cbUseCeilLineAngles.Checked = General.Settings.ReadSetting("windows." + configname + ".useceillineangles", false);
			cbUseFloorLineAngles.Checked = General.Settings.ReadSetting("windows." + configname + ".usefloorlineangles", false);

			ceilingslopecontrol.UseLineAngles = General.Settings.ReadSetting("windows." + configname + ".useceilslopelineangles", false);
			floorslopecontrol.UseLineAngles = General.Settings.ReadSetting("windows." + configname + ".usefloorslopelineangles", false);

			ceilingslopecontrol.PivotMode = (SlopePivotMode)General.Settings.ReadSetting("windows." + configname + ".ceilpivotmode", (int)SlopePivotMode.LOCAL);
			floorslopecontrol.PivotMode = (SlopePivotMode)General.Settings.ReadSetting("windows." + configname + ".floorpivotmode", (int)SlopePivotMode.LOCAL);

			// Diable brightness controls?
			if(!General.Map.Config.DistinctFloorAndCeilingBrightness)
			{
				ceilBrightness.Enabled = false;
				ceilLightAbsolute.Enabled = false;
				resetceillight.Enabled = false;

				floorBrightness.Enabled = false;
				floorLightAbsolute.Enabled = false;
				resetfloorlight.Enabled = false;
			}
		}

		#endregion

		#region ================== Methods

		// This sets up the form to edit the given sectors
		public void Setup(ICollection<Sector> sectors) 
		{
			preventchanges = true; //mxd
			oldmapischanged = General.Map.IsChanged;
			undocreated = false;
            // Keep this list
            this.sectors = sectors;
			if(sectors.Count > 1) this.Text = "Edit Sectors (" + sectors.Count + ")";
			sectorprops = new Dictionary<Sector, SectorProperties>(sectors.Count); //mxd

			//mxd. Set default height offset
			heightoffset.Text = "0";

			CreateHelperProps(sectors); //mxd

			////////////////////////////////////////////////////////////////////////
			// Set all options to the first sector properties
			////////////////////////////////////////////////////////////////////////

			// Get first sector
			Sector sc = General.GetByIndex(sectors, 0);

			// Flags
			foreach(CheckBox c in flags.Checkboxes)
				if(sc.Flags.ContainsKey(c.Tag.ToString())) c.Checked = sc.Flags[c.Tag.ToString()];

			// Portal flags
			foreach(CheckBox c in floorportalflags.Checkboxes)
				if(sc.Flags.ContainsKey(c.Tag.ToString())) c.Checked = sc.Flags[c.Tag.ToString()];
			foreach(CheckBox c in ceilportalflags.Checkboxes)
				if(sc.Flags.ContainsKey(c.Tag.ToString())) c.Checked = sc.Flags[c.Tag.ToString()];

			// Effects
			effect.Value = sc.Effect;
			brightness.Text = sc.Brightness.ToString();

			// Floor/ceiling
			floorheight.Text = sc.FloorHeight.ToString();
			ceilingheight.Text = sc.CeilHeight.ToString();
			floortex.TextureName = sc.FloorTexture;
			ceilingtex.TextureName = sc.CeilTexture;

			// UDMF stuff
			// Texture offsets
			ceilOffsets.SetValuesFrom(sc.Fields, true);
			floorOffsets.SetValuesFrom(sc.Fields, true);

			// Texture scale
			ceilScale.SetValuesFrom(sc.Fields, true);
			floorScale.SetValuesFrom(sc.Fields, true);

			// Texture rotation
			double ceilAngle = sc.Fields.GetValue("rotationceiling", 0.0);
			double floorAngle = sc.Fields.GetValue("rotationfloor", 0.0);

			ceilRotation.Text = ceilAngle.ToString();
			floorRotation.Text = floorAngle.ToString();

			ceilAngleControl.Angle = General.ClampAngle(360 - (int)ceilAngle);
			floorAngleControl.Angle = General.ClampAngle(360 - (int)floorAngle);

			// Texture brightness
			ceilBrightness.Text = sc.Fields.GetValue("lightceiling", 0).ToString();
			floorBrightness.Text = sc.Fields.GetValue("lightfloor", 0).ToString();
			ceilLightAbsolute.Checked = sc.Fields.GetValue("lightceilingabsolute", false);
			floorLightAbsolute.Checked = sc.Fields.GetValue("lightfloorabsolute", false);

			// Portal alpha
			alphaceiling.Text = General.Clamp(sc.Fields.GetValue("alphaceiling", 1.0), 0.0, 1.0).ToString();
			alphafloor.Text = General.Clamp(sc.Fields.GetValue("alphafloor", 1.0), 0.0, 1.0).ToString();

			// Reflectivity
			ceiling_reflect.Text = General.Clamp(sc.Fields.GetValue("ceiling_reflect", 0.0), 0.0, 1.0).ToString();
			floor_reflect.Text = General.Clamp(sc.Fields.GetValue("floor_reflect", 0.0), 0.0, 1.0).ToString();

			// Fog density
			fogdensity.Text = General.Clamp(sc.Fields.GetValue("fogdensity", 0), 0, 510).ToString();

			// Floor/ceiling glow
			int ceilingglowcolorval = sc.Fields.GetValue("ceilingglowcolor", 0);
			int floorglowcolorval = sc.Fields.GetValue("floorglowcolor", 0);
			ceilingglowcolor.SetValueFrom(sc.Fields, true);
			floorglowcolor.SetValueFrom(sc.Fields, true);

			// Floor/ceiling glow height
			ceilingglowheight.Text = sc.Fields.GetValue("ceilingglowheight", 0.0).ToString();
			floorglowheight.Text = sc.Fields.GetValue("floorglowheight", 0.0).ToString();

			// Render style
			ceilRenderStyle.SelectedIndex = renderstyles.IndexOf(sc.Fields.GetValue("renderstyleceiling", "translucent"));
			floorRenderStyle.SelectedIndex = renderstyles.IndexOf(sc.Fields.GetValue("renderstylefloor", "translucent"));

			// Portal render style
			if (portalrenderstyles.Count > 0)
			{
				ceilportalrenderstyle.SelectedIndex = portalrenderstyles.IndexOf(sc.Fields.GetValue("portal_ceil_overlaytype", portalrenderstyles[0]));
				floorportalrenderstyle.SelectedIndex = portalrenderstyles.IndexOf(sc.Fields.GetValue("portal_floor_overlaytype", portalrenderstyles[0]));
			}

			// Damage
			damagetype.Text = sc.Fields.GetValue("damagetype", NO_DAMAGETYPE);
			damageamount.Text = sc.Fields.GetValue("damageamount", 0).ToString();
			damageinterval.Text = sc.Fields.GetValue("damageinterval", 32).ToString();
			leakiness.Text = General.Clamp(sc.Fields.GetValue("leakiness", 0), 0, 256).ToString();

			// Terrain
			ceilterrain.Text = sc.Fields.GetValue("ceilingterrain", NO_TERRAIN);
			floorterrain.Text = sc.Fields.GetValue("floorterrain", NO_TERRAIN);

			// Misc
			soundsequence.Text = sc.Fields.GetValue("soundsequence", NO_SOUND_SEQUENCE);
			gravity.Text = sc.Fields.GetValue("gravity", 1.0).ToString();
			desaturation.Text = General.Clamp(sc.Fields.GetValue("desaturation", 0.0), 0.0, 1.0).ToString();

			// Sector colors
			fadeColor.SetValueFrom(sc.Fields, true);
			lightColor.SetValueFrom(sc.Fields, true);

            // [ZZ]
            ceilingColor.SetValueFrom(sc.Fields, true);
            upperWallColor.SetValueFrom(sc.Fields, true);
            thingsColor.SetValueFrom(sc.Fields, true);
            lowerWallColor.SetValueFrom(sc.Fields, true);
            floorColor.SetValueFrom(sc.Fields, true);

            // Slopes
            SetupFloorSlope(sc, true);
			SetupCeilingSlope(sc, true);

			// Custom fields
			fieldslist.SetValues(sc.Fields, true);

			// Comments
			commenteditor.SetValues(sc.Fields, true);

			anglesteps = new StepsList();

			////////////////////////////////////////////////////////////////////////
			// Now go for all sectors and change the options when a setting is different
			////////////////////////////////////////////////////////////////////////

			// Go for all sectors
			foreach(Sector s in sectors) 
			{
				// Flags
				SetupFlags(flags, s);
				SetupFlags(ceilportalflags, s);
				SetupFlags(floorportalflags, s);

				// Effects
				if(s.Effect != effect.Value) effect.Empty = true;
				if(s.Brightness.ToString() != brightness.Text) brightness.Text = "";

				// Floor/Ceiling
				if(s.FloorHeight.ToString() != floorheight.Text) floorheight.Text = "";
				if(s.CeilHeight.ToString() != ceilingheight.Text) ceilingheight.Text = "";
				if(s.FloorTexture != floortex.TextureName) 
				{
					floortex.MultipleTextures = true; //mxd
					floortex.TextureName = "";
				}
				if(s.CeilTexture != ceilingtex.TextureName) 
				{
					ceilingtex.MultipleTextures = true; //mxd
					ceilingtex.TextureName = "";
				}

				// UDMF stuff
				// Texture offsets
				ceilOffsets.SetValuesFrom(s.Fields, false);
				floorOffsets.SetValuesFrom(s.Fields, false);

				// Texture scale
				ceilScale.SetValuesFrom(s.Fields, false);
				floorScale.SetValuesFrom(s.Fields, false);

				// Texture rotation
				if(s.Fields.GetValue("rotationceiling", 0.0).ToString() != ceilRotation.Text) 
				{
					ceilRotation.Text = "";
					ceilAngleControl.Angle = AngleControlEx.NO_ANGLE;
				}
				if(s.Fields.GetValue("rotationfloor", 0.0).ToString() != floorRotation.Text)
				{
					floorRotation.Text = "";
					floorAngleControl.Angle = AngleControlEx.NO_ANGLE;
				}

				// Texture brightness
				if(s.Fields.GetValue("lightceiling", 0).ToString() != ceilBrightness.Text) ceilBrightness.Text = "";
				if(s.Fields.GetValue("lightfloor", 0).ToString() != floorBrightness.Text) floorBrightness.Text = "";

				if(s.Fields.GetValue("lightceilingabsolute", false) != ceilLightAbsolute.Checked) 
				{
					ceilLightAbsolute.ThreeState = true;
					ceilLightAbsolute.CheckState = CheckState.Indeterminate;
				}
				if(s.Fields.GetValue("lightfloorabsolute", false) != floorLightAbsolute.Checked) 
				{
					floorLightAbsolute.ThreeState = true;
					floorLightAbsolute.CheckState = CheckState.Indeterminate;
				}

				// Portal alpha
				if(s.Fields.GetValue("alphaceiling", 1.0).ToString() != alphaceiling.Text) alphaceiling.Text = "";
				if(s.Fields.GetValue("alphafloor", 1.0).ToString() != alphafloor.Text) alphafloor.Text = "";

				// Reflectivity
				if(s.Fields.GetValue("ceiling_reflect", 0.0).ToString() != ceiling_reflect.Text) ceiling_reflect.Text = "";
				if(s.Fields.GetValue("floor_reflect", 0.0).ToString() != floor_reflect.Text) floor_reflect.Text = "";

				// Fog density
				if(s.Fields.GetValue("fogdensity", 0).ToString() != fogdensity.Text) fogdensity.Text = "";

				// Floor/ceiling glow
				if(floorglowcolorval != s.Fields.GetValue("floorglowcolor", 0)) floorglowcolorval = 0;
				if(ceilingglowcolorval != s.Fields.GetValue("ceilingglowcolor", 0)) ceilingglowcolorval = 0;
				ceilingglowcolor.SetValueFrom(s.Fields, false);
				floorglowcolor.SetValueFrom(s.Fields, false);

				// Floor/ceiling glow height
				if(s.Fields.GetValue("ceilingglowheight", 0.0).ToString() != ceilingglowheight.Text) ceilingglowheight.Text = "";
				if(s.Fields.GetValue("floorglowheight", 0.0).ToString() != floorglowheight.Text) floorglowheight.Text = "";

				// Render style
				if(ceilRenderStyle.SelectedIndex > -1 && ceilRenderStyle.SelectedIndex != renderstyles.IndexOf(s.Fields.GetValue("renderstyleceiling", "translucent")))
					ceilRenderStyle.SelectedIndex = -1;
				if(floorRenderStyle.SelectedIndex > -1 && floorRenderStyle.SelectedIndex != renderstyles.IndexOf(s.Fields.GetValue("renderstylefloor", "translucent")))
					floorRenderStyle.SelectedIndex = -1;

				// Portal render style
				if(ceilportalrenderstyle.SelectedIndex > -1 && ceilportalrenderstyle.SelectedIndex != portalrenderstyles.IndexOf(s.Fields.GetValue("portal_ceil_overlaytype", portalrenderstyles[0])))
					ceilportalrenderstyle.SelectedIndex = -1;
				if(floorportalrenderstyle.SelectedIndex > -1 && floorportalrenderstyle.SelectedIndex != portalrenderstyles.IndexOf(s.Fields.GetValue("portal_floor_overlaytype", portalrenderstyles[0])))
					floorportalrenderstyle.SelectedIndex = -1;

				// Damage
				if(damagetype.SelectedIndex > -1 && s.Fields.GetValue("damagetype", NO_DAMAGETYPE) != damagetype.Text) 
					damagetype.SelectedIndex = -1;
				if(s.Fields.GetValue("damageamount", 0).ToString() != damageamount.Text) damageamount.Text = "";
				if(s.Fields.GetValue("damageinterval", 32).ToString() != damageinterval.Text) damageinterval.Text = "";
				if(s.Fields.GetValue("leakiness", 0).ToString() != leakiness.Text) leakiness.Text = "";

				// Terrain
				if(ceilterrain.SelectedIndex > -1 && s.Fields.GetValue("ceilingterrain", NO_TERRAIN) != ceilterrain.Text)
					ceilterrain.SelectedIndex = -1;
				if(floorterrain.SelectedIndex > -1 && s.Fields.GetValue("floorterrain", NO_TERRAIN) != floorterrain.Text)
					floorterrain.SelectedIndex = -1;

				// Misc
				if(soundsequence.SelectedIndex > -1 && s.Fields.GetValue("soundsequence", NO_SOUND_SEQUENCE) != soundsequence.Text)
					soundsequence.SelectedIndex = -1;
				if(s.Fields.GetValue("gravity", 1.0).ToString() != gravity.Text) gravity.Text = "";
				if(s.Fields.GetValue("desaturation", 0.0).ToString() != desaturation.Text) desaturation.Text = "";

				// Sector colors
				fadeColor.SetValueFrom(s.Fields, false);
				lightColor.SetValueFrom(s.Fields, false);

                // [ZZ]
                ceilingColor.SetValueFrom(s.Fields, false);
                upperWallColor.SetValueFrom(s.Fields, false);
                thingsColor.SetValueFrom(s.Fields, false);
                lowerWallColor.SetValueFrom(s.Fields, false);
                floorColor.SetValueFrom(s.Fields, false);

                // Slopes
                SetupFloorSlope(s, false);
				SetupCeilingSlope(s, false);

				// Custom fields
				fieldslist.SetValues(s.Fields, false);

				//mxd. Comments
				commenteditor.SetValues(s.Fields, false);

				//mxd. Angle steps
				foreach(Sidedef side in s.Sidedefs)
				{
					int angle;
					if(side.Line.Front != null && side.Index == side.Line.Front.Index)
						angle = General.ClampAngle(270 - side.Line.AngleDeg);
					else
						angle = General.ClampAngle(90 - side.Line.AngleDeg);

					if(!anglesteps.Contains(angle)) anglesteps.Add(angle);
				}
			}

            //mxd. Glow is disabled?
            if (floorglowcolorval == -1)
                floorGlowEnabled.Checked = false;

            if (ceilingglowcolorval == -1)
                ceilingGlowEnabled.Checked = false;

            //mxd. Update "Reset" buttons...
            if(ceiling_reflect.Text == "0") reset_ceiling_reflect.Visible = false;
			if(floor_reflect.Text == "0") reset_floor_reflect.Visible = false;
			if(ceilingglowheight.Text == "0") resetceilingglowheight.Visible = false;
			if(floorglowheight.Text == "0") resetfloorglowheight.Visible = false;

			//mxd. Cause Graf was not into non-zero default glow height...
			UpdateCeilingGlowHeightWarning();
			UpdateFloorGlowHeightWarning();

			//mxd. Setup tags
			tagsselector.SetValues(sectors);

			//mxd. Update slope controls
			ceilingslopecontrol.UpdateControls();
			floorslopecontrol.UpdateControls();

			// Show sector height
			UpdateSectorHeight();

			//mxd. Update brightness reset buttons
			resetceillight.Visible = (ceilLightAbsolute.CheckState != CheckState.Unchecked || ceilBrightness.GetResult(0) != 0);
			resetfloorlight.Visible = (floorLightAbsolute.CheckState != CheckState.Unchecked || floorBrightness.GetResult(0) != 0);

			//mxd. Angle steps
			anglesteps.Sort();
			if(cbUseCeilLineAngles.Checked) ceilRotation.StepValues = anglesteps;
			if(cbUseFloorLineAngles.Checked) floorRotation.StepValues = anglesteps;
			if(ceilingslopecontrol.UseLineAngles) ceilingslopecontrol.StepValues = anglesteps;
			if(floorslopecontrol.UseLineAngles) floorslopecontrol.StepValues = anglesteps;

			//mxd. Comments
			commenteditor.FinishSetup();

			preventchanges = false; //mxd
		}

		//mxd
		private static void SetupFlags(CheckboxArrayControl control, Sector s)
		{
			foreach(CheckBox c in control.Checkboxes)
			{
				if(c.CheckState == CheckState.Indeterminate) continue; //mxd
				if(s.IsFlagSet(c.Tag.ToString()) != c.Checked)
				{
					c.ThreeState = true;
					c.CheckState = CheckState.Indeterminate;
				}
			}
		}

		//mxd
		private static void ApplyFlags(CheckboxArrayControl control, Sector s)
		{
			foreach(CheckBox c in control.Checkboxes)
			{
				switch(c.CheckState)
				{
					case CheckState.Checked: s.SetFlag(c.Tag.ToString(), true); break;
					case CheckState.Unchecked: s.SetFlag(c.Tag.ToString(), false); break;
				}
			}
		}

		//mxd
		private static void ApplyFloatProperty(ButtonsNumericTextbox control, Sector s, float defaultvalue)
		{
			if(!string.IsNullOrEmpty(control.Text))
			{
				double ceilAlphaVal = General.Clamp(control.GetResultFloat(s.Fields.GetValue(control.Name, defaultvalue)), 0.0, 1.0);
				UniFields.SetFloat(s.Fields, control.Name, ceilAlphaVal, defaultvalue);
			}
		}

		//mxd
		private void MakeUndo() 
		{
			if(undocreated) return;
			undocreated = true;

			//mxd. Make undo
			General.Map.UndoRedo.CreateUndo("Edit " + (sectors.Count > 1 ? sectors.Count + " sectors" : "sector"));
			foreach(Sector s in sectors) s.Fields.BeforeFieldsChange();
		}

		// mxd
		private void CreateHelperProps(ICollection<Sector> sectors) 
		{
			slopepivots = new Dictionary<Sector, Vector2D>(sectors.Count);
			
			foreach(Sector s in sectors)
			{
				if(slopepivots.ContainsKey(s)) continue;
				Vector2D pivot = new Vector2D(s.BBox.X + s.BBox.Width / 2, s.BBox.Y + s.BBox.Height / 2);
				globalslopepivot += pivot;
				slopepivots.Add(s, pivot);

				// Store initial properties
				sectorprops.Add(s, new SectorProperties(s));
			}

			globalslopepivot /= sectors.Count;
		}

		// This updates the sector height field
		private void UpdateSectorHeight() 
		{
			int delta = int.MinValue;

			// Check all selected sectors
			foreach(Sector s in sectors) 
			{
				if(delta == int.MinValue) 
				{
					// First sector in list
					delta = s.CeilHeight - s.FloorHeight;
				} 
				else if(delta != (s.CeilHeight - s.FloorHeight)) 
				{
					// We can't show heights because the delta
					// heights for the sectors is different
					delta = int.MinValue;
					break;
				}
			}

			if(delta != int.MinValue) 
			{
				sectorheight.Text = delta.ToString();
				sectorheight.Visible = true;
				sectorheightlabel.Visible = true;
			} 
			else 
			{
				sectorheight.Visible = false;
				sectorheightlabel.Visible = false;
			}
		}

		//mxd
		private void UpdateCeilingHeight() 
		{
			int offset;

			if(heightoffset.Text == "++" || heightoffset.Text == "--") // Raise or lower by sector height
			{
				int sign = (heightoffset.Text == "++" ? 1 : -1);
				foreach(Sector s in sectors)
				{
					offset = sectorprops[s].CeilHeight - sectorprops[s].FloorHeight;
					s.CeilHeight += offset * sign;
				}
			}
			else
			{
				//restore values
				if(string.IsNullOrEmpty(ceilingheight.Text))
				{
					// Reset increment steps, otherwise it's just keep counting and counting
					heightoffset.ResetIncrementStep();

					foreach (Sector s in sectors)
					{
						// To get the steps for ---/+++ into effect the offset has to be retrieved again for each sector
						offset = heightoffset.GetResult(0);
						s.CeilHeight = sectorprops[s].CeilHeight + offset;
					}
				}
				else //update values
				{
					// Reset increment steps, otherwise it's just keep counting and counting
					heightoffset.ResetIncrementStep();

					foreach (Sector s in sectors)
					{
						// To get the steps for ---/+++ into effect the offset has to be retrieved again for each sector
						offset = heightoffset.GetResult(0);
						s.CeilHeight = ceilingheight.GetResult(sectorprops[s].CeilHeight) + offset;
					}
				}
			}
		}

		//mxd
		private void UpdateFloorHeight() 
		{
			int offset;

			if(heightoffset.Text == "++" || heightoffset.Text == "--")
			{
				// Raise or lower by sector height
				int sign = (heightoffset.Text == "++" ? 1 : -1);
				foreach(Sector s in sectors)
				{
					offset = sectorprops[s].CeilHeight - sectorprops[s].FloorHeight;
					s.FloorHeight += offset * sign;
				}
			}
			else
			{
				// Reset increment steps, otherwise it's just keep counting and counting
				heightoffset.ResetIncrementStep();
				
				//restore values
				if(string.IsNullOrEmpty(floorheight.Text))
				{
					foreach(Sector s in sectors)
					{
						// To get the steps for ---/+++ into effect the offset has to be retrieved again for each sector
						offset = heightoffset.GetResult(0);
						s.FloorHeight = sectorprops[s].FloorHeight + offset;
					}
				}
				else //update values
				{
					foreach(Sector s in sectors)
					{
						// To get the steps for ---/+++ into effect the offset has to be retrieved again for each sector
						offset = heightoffset.GetResult(0);
						s.FloorHeight = floorheight.GetResult(sectorprops[s].FloorHeight) + offset;
					}
				}
			}
		}

		/// <summary>
		/// Enables or disables controls depending on if their tag is one of the UDMF fields set in the game config.
		/// </summary>
		/// <param name="control">Control to process</param>
		private void DoUDMFControls(Control control)
		{
			if (control.Tag is string name && !string.IsNullOrWhiteSpace(name))
			{
				EnableDisableControlAndChildren(control, General.Map.Config.SectorFields.Any(f => f.Name == name));
			}
			else
			{
				foreach (Control c in control.Controls)
					DoUDMFControls(c);
			}
		}

		/// <summary>
		/// Enables or disables a control and all its children.
		/// </summary>
		/// <param name="control">Control the enable or disable</param>
		/// <param name="state">If to enable or disable</param>
		private void EnableDisableControlAndChildren(Control control, bool state)
		{
			control.Enabled = state;

			foreach (Control c in control.Controls)
				EnableDisableControlAndChildren(c, state);
		}

		#endregion

		#region ================== Events

		private void apply_Click(object sender, EventArgs e) 
		{
			// Verify the effect
			if((effect.Value < General.Map.FormatInterface.MinEffect) || (effect.Value > General.Map.FormatInterface.MaxEffect)) 
			{
				General.ShowWarningMessage("Sector effect must be between " + General.Map.FormatInterface.MinEffect + " and " + General.Map.FormatInterface.MaxEffect + ".", MessageBoxButtons.OK);
				return;
			}

			MakeUndo(); //mxd

			// Go for all sectors
			foreach(Sector s in sectors) 
			{
				// Apply all flags
				ApplyFlags(flags, s);
				ApplyFlags(ceilportalflags, s);
				ApplyFlags(floorportalflags, s);

				// Effects
				if(!effect.Empty) s.Effect = effect.Value;

				// Fields
				fieldslist.Apply(s.Fields);

				//mxd. Comments
				commenteditor.Apply(s.Fields);

				// Portal alpha
				ApplyFloatProperty(alphaceiling, s, 1.0f);
				ApplyFloatProperty(alphafloor, s, 1.0f);

				// Reflectivity
				ApplyFloatProperty(ceiling_reflect, s, 0.0f);
				ApplyFloatProperty(floor_reflect, s, 0.0f);

				// Renderstyle
				if(renderstyles.Count > 0) 
				{
					if(ceilRenderStyle.SelectedIndex > -1)
						UniFields.SetString(s.Fields, "renderstyleceiling", renderstyles[ceilRenderStyle.SelectedIndex], "translucent");
					if(floorRenderStyle.SelectedIndex > -1)
						UniFields.SetString(s.Fields, "renderstylefloor", renderstyles[floorRenderStyle.SelectedIndex], "translucent");
				}

				// Portal renderstyles
				if(portalrenderstyles.Count > 0)
				{
					if(ceilportalrenderstyle.SelectedIndex > -1)
						UniFields.SetString(s.Fields, "portal_ceil_overlaytype", portalrenderstyles[ceilportalrenderstyle.SelectedIndex], portalrenderstyles[0]);
					if(floorportalrenderstyle.SelectedIndex > -1)
						UniFields.SetString(s.Fields, "portal_floor_overlaytype", portalrenderstyles[floorportalrenderstyle.SelectedIndex], portalrenderstyles[0]);
				}

				//Damage
				if(!string.IsNullOrEmpty(damagetype.Text))
					UniFields.SetString(s.Fields, "damagetype", damagetype.Text, NO_DAMAGETYPE);
				if(!string.IsNullOrEmpty(damageamount.Text))
					UniFields.SetInteger(s.Fields, "damageamount", damageamount.GetResult(s.Fields.GetValue("damageamount", 0)), 0);
				if(!string.IsNullOrEmpty(damageinterval.Text))
					UniFields.SetInteger(s.Fields, "damageinterval", damageinterval.GetResult(s.Fields.GetValue("damageinterval", 32)), 32);
				if(!string.IsNullOrEmpty(leakiness.Text))
					UniFields.SetInteger(s.Fields, "leakiness", General.Clamp(leakiness.GetResult(s.Fields.GetValue("leakiness", 0)), 0, 256), 0);

				//Terrain
				if(!string.IsNullOrEmpty(ceilterrain.Text))
					UniFields.SetString(s.Fields, "ceilingterrain", ceilterrain.Text, NO_TERRAIN);
				if(!string.IsNullOrEmpty(floorterrain.Text))
					UniFields.SetString(s.Fields, "floorterrain", floorterrain.Text, NO_TERRAIN);

				// Misc
				if(!string.IsNullOrEmpty(soundsequence.Text))
					UniFields.SetString(s.Fields, "soundsequence", soundsequence.Text, NO_SOUND_SEQUENCE);
				if(!string.IsNullOrEmpty(gravity.Text)) 
					UniFields.SetFloat(s.Fields, "gravity", gravity.GetResultFloat(s.Fields.GetValue("gravity", 1.0)), 1.0);
				if(!string.IsNullOrEmpty(desaturation.Text)) 
				{
					double val = General.Clamp(desaturation.GetResultFloat(s.Fields.GetValue("desaturation", 0.0)), 0.0, 1.0);
					UniFields.SetFloat(s.Fields, "desaturation", val, 0.0);
				}

				// Clear horizontal slopes
				double diff = Math.Abs(Math.Round(s.FloorSlopeOffset) - s.FloorSlopeOffset);
				if (Math.Abs(s.FloorSlope.z) == 1.0 && diff < 0.000000001)
				{
					s.FloorHeight = -Convert.ToInt32(s.FloorSlopeOffset);
					s.FloorSlope = new Vector3D();
					s.FloorSlopeOffset = double.NaN;
				}

				diff = Math.Abs(Math.Round(s.CeilSlopeOffset) - s.CeilSlopeOffset);
				if (Math.Abs(s.CeilSlope.z) == 1.0 && diff < 0.000000001)
				{
					s.CeilHeight = Convert.ToInt32(s.CeilSlopeOffset);
					s.CeilSlope = new Vector3D();
					s.CeilSlopeOffset = double.NaN;
				}
			}

			//mxd. Apply tags
			tagsselector.ApplyTo(sectors);

			// Update the used textures
			General.Map.Data.UpdateUsedTextures();

			// Done
			General.Map.IsChanged = true;
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty); //mxd
			this.DialogResult = DialogResult.OK;
			this.Close();
		}

		private void cancel_Click(object sender, EventArgs e) 
		{
			//mxd. Let's pretend nothing of this really happened...
			if (undocreated)
			{
				General.Map.UndoRedo.WithdrawUndo();

				// Changing certain properties of the sector, like floor/ceiling textures will set General.Map.IsChanged to true.
				// But if cancel is pressed and the changes are discarded, and the map was not changed before, we have to force
				// General.Map.IsChanged back to false
				if (General.Map.IsChanged && oldmapischanged == false)
					General.Map.ForceMapIsChangedFalse();
			}
			
			// Be gone
			this.DialogResult = DialogResult.Cancel;
			this.Close();
		}

		private void browseeffect_Click(object sender, EventArgs e) 
		{
			effect.Value = EffectBrowserForm.BrowseEffect(this, effect.Value);
		}

		//mxd
		private void SectorEditFormUDMF_FormClosing(object sender, FormClosingEventArgs e) 
		{
			// Save settings
			General.Settings.WriteSetting("windows." + configname + ".activetab", tabs.SelectedIndex);

			General.Settings.WriteSetting("windows." + configname + ".linkceilingscale", ceilScale.LinkValues);
			General.Settings.WriteSetting("windows." + configname + ".linkfloorscale", floorScale.LinkValues);

			General.Settings.WriteSetting("windows." + configname + ".useceillineangles", cbUseCeilLineAngles.Checked);
			General.Settings.WriteSetting("windows." + configname + ".usefloorlineangles", cbUseFloorLineAngles.Checked);

			General.Settings.WriteSetting("windows." + configname + ".useceilslopelineangles", ceilingslopecontrol.UseLineAngles);
			General.Settings.WriteSetting("windows." + configname + ".usefloorslopelineangles", floorslopecontrol.UseLineAngles);

			General.Settings.WriteSetting("windows." + configname + ".ceilpivotmode", (int)ceilingslopecontrol.PivotMode);
			General.Settings.WriteSetting("windows." + configname + ".floorpivotmode", (int)floorslopecontrol.PivotMode);
		}

		private void SectorEditFormUDMF_HelpRequested(object sender, HelpEventArgs hlpevent) 
		{
			General.ShowHelp("w_sectoredit.html");
			hlpevent.Handled = true;
		}

		private void tabcustom_MouseEnter(object sender, EventArgs e) 
		{
			fieldslist.Focus();
		}

		private void ceilAngleControl_AngleChanged(object sender, EventArgs e) 
		{
			ceilRotation.Text = (General.ClampAngle(360 - ceilAngleControl.Angle)).ToString();
		}

		private void floorAngleControl_AngleChanged(object sender, EventArgs e) 
		{
			floorRotation.Text = (General.ClampAngle(360 - floorAngleControl.Angle)).ToString();
		}

		private void cbUseCeilLineAngles_CheckedChanged(object sender, EventArgs e) 
		{
			ceilRotation.ButtonStepsWrapAround = cbUseCeilLineAngles.Checked;
			ceilRotation.StepValues = (cbUseCeilLineAngles.Checked ? anglesteps : null);
		}

		private void cbUseFloorLineAngles_CheckedChanged(object sender, EventArgs e) 
		{
			floorRotation.ButtonStepsWrapAround = cbUseFloorLineAngles.Checked;
			floorRotation.StepValues = (cbUseFloorLineAngles.Checked ? anglesteps : null);
		}

		private void resetfloorterrain_Click(object sender, EventArgs e)
		{
			floorterrain.Focus();
			floorterrain.Text = NO_TERRAIN;
		}

		private void floorterrain_TextChanged(object sender, EventArgs e)
		{
			resetfloorterrain.Visible = (floorterrain.Text != NO_TERRAIN);
		}

		private void floorterrain_MouseDown(object sender, MouseEventArgs e)
		{
			if(floorterrain.Text == NO_TERRAIN) floorterrain.SelectAll();
		}

		private void resetceilterrain_Click(object sender, EventArgs e)
		{
			ceilterrain.Focus();
			ceilterrain.Text = NO_TERRAIN;
		}

		private void ceilterrain_TextChanged(object sender, EventArgs e)
		{
			resetceilterrain.Visible = (ceilterrain.Text != NO_TERRAIN);
		}

		private void ceilterrain_MouseDown(object sender, MouseEventArgs e)
		{
			if(ceilterrain.Text == NO_TERRAIN) ceilterrain.SelectAll();
		}

		private void resetdamagetype_Click(object sender, EventArgs e)
		{
			damagetype.Focus();
			damagetype.Text = NO_DAMAGETYPE;
		}

		private void damagetype_TextChanged(object sender, EventArgs e)
		{
			resetdamagetype.Visible = (damagetype.Text != NO_DAMAGETYPE);
		}

		private void damagetype_MouseDown(object sender, MouseEventArgs e)
		{
			if(damagetype.Text == NO_DAMAGETYPE) damagetype.SelectAll();
		}

		private void resetsoundsequence_Click(object sender, EventArgs e) 
		{
			soundsequence.Focus();
			soundsequence.Text = NO_SOUND_SEQUENCE;
		}

		private void soundsequence_TextChanged(object sender, EventArgs e)
		{
			resetsoundsequence.Visible = (soundsequence.Text != NO_SOUND_SEQUENCE);
		}

		private void soundsequence_MouseDown(object sender, MouseEventArgs e) 
		{
			if(soundsequence.Text == NO_SOUND_SEQUENCE) soundsequence.SelectAll();
		}

		private void ceiling_reflect_WhenTextChanged(object sender, EventArgs e)
		{
			reset_ceiling_reflect.Visible = (string.IsNullOrEmpty(ceiling_reflect.Text) || ceiling_reflect.GetResultFloat(0.0f) != 0.0f);
		}

		private void floor_reflect_WhenTextChanged(object sender, EventArgs e)
		{
			reset_floor_reflect.Visible = (string.IsNullOrEmpty(floor_reflect.Text) || floor_reflect.GetResultFloat(0.0f) != 0.0f);
		}

		private void reset_ceiling_reflect_Click(object sender, EventArgs e)
		{
			ceiling_reflect.Focus();
			ceiling_reflect.Text = "0";
		}

		private void reset_floor_reflect_Click(object sender, EventArgs e)
		{
			floor_reflect.Focus();
			floor_reflect.Text = "0";
		}

		private void alphaceiling_WhenTextChanged(object sender, EventArgs e)
		{
			resetalphaceiling.Visible = (string.IsNullOrEmpty(alphaceiling.Text) || alphaceiling.GetResultFloat(1.0f) != 1.0f);
		}

		private void alphafloor_WhenTextChanged(object sender, EventArgs e)
		{
			resetalphafloor.Visible = (string.IsNullOrEmpty(alphafloor.Text) || alphafloor.GetResultFloat(1.0f) != 1.0f);
		}

		private void resetalphafloor_Click(object sender, EventArgs e)
		{
			alphafloor.Focus();
			alphafloor.Text = "1";
		}

		private void resetalphaceiling_Click(object sender, EventArgs e)
		{
			alphaceiling.Focus();
			alphaceiling.Text = "1";
		}

		#endregion

		#region ================== Sector Realtime events (mxd)

		private void ceilingheight_WhenTextChanged(object sender, EventArgs e) 
		{
			if(preventchanges)	return;
			MakeUndo(); //mxd

			UpdateCeilingHeight();
			UpdateSectorHeight();

			General.Map.IsChanged = true;
			if(OnValuesChanged != null)	OnValuesChanged(this, EventArgs.Empty);
		}

		private void floorheight_WhenTextChanged(object sender, EventArgs e) 
		{
			if(preventchanges)	return;
			MakeUndo(); //mxd

			UpdateFloorHeight();
			UpdateSectorHeight();

			General.Map.IsChanged = true;
			if(OnValuesChanged != null)	OnValuesChanged(this, EventArgs.Empty);
		}

		private void heightoffset_WhenTextChanged(object sender, EventArgs e) 
		{
			if(preventchanges) return;
			MakeUndo(); //mxd

			UpdateFloorHeight();
			UpdateCeilingHeight();
			UpdateSectorHeight();

			General.Map.IsChanged = true;
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
		}

		private void brightness_WhenTextChanged(object sender, EventArgs e) 
		{
			if(preventchanges)	return;
			MakeUndo(); //mxd

			// Reset increment steps, otherwise it's just keep counting and counting
			brightness.ResetIncrementStep();

			//restore values
			if (string.IsNullOrEmpty(brightness.Text)) 
			{
				foreach(Sector s in sectors)
					s.Brightness = sectorprops[s].Brightness;
			
			} 
			else //update values
			{
				foreach(Sector s in sectors)
					s.Brightness = General.Clamp(brightness.GetResult(sectorprops[s].Brightness), General.Map.FormatInterface.MinBrightness, General.Map.FormatInterface.MaxBrightness);
			}

			General.Map.IsChanged = true;
			if(OnValuesChanged != null)	OnValuesChanged(this, EventArgs.Empty);
		}

		private void ceilingtex_OnValueChanged(object sender, EventArgs e) 
		{
			if(preventchanges)	return;
			MakeUndo(); //mxd

			//restore values
			if(string.IsNullOrEmpty(ceilingtex.TextureName)) 
			{
				foreach(Sector s in sectors)
					s.SetCeilTexture(sectorprops[s].CeilTexture);
			
			} 
			else //update values
			{
				foreach(Sector s in sectors)
					s.SetCeilTexture(ceilingtex.GetResult(s.CeilTexture));
			}

			// Update the used textures
			General.Map.Data.UpdateUsedTextures();

			General.Map.IsChanged = true;
			if(OnValuesChanged != null)	OnValuesChanged(this, EventArgs.Empty);
		}

		private void floortex_OnValueChanged(object sender, EventArgs e) 
		{
			if(preventchanges)	return;
			MakeUndo(); //mxd

			//restore values
			if(string.IsNullOrEmpty(floortex.TextureName)) 
			{
				foreach(Sector s in sectors)
					s.SetFloorTexture(sectorprops[s].FloorTexture);

			} 
			else //update values
			{
				foreach(Sector s in sectors)
					s.SetFloorTexture(floortex.GetResult(s.FloorTexture));
			}

			// Update the used textures
			General.Map.Data.UpdateUsedTextures();

			General.Map.IsChanged = true;
			if(OnValuesChanged != null)	OnValuesChanged(this, EventArgs.Empty);
		}

		private void floorRotation_WhenTextChanged(object sender, EventArgs e) 
		{
			if(preventchanges)	return;
			MakeUndo(); //mxd

			// Reset increment steps, otherwise it's just keep counting and counting
			floorRotation.ResetIncrementStep();

			//restore values
			if (string.IsNullOrEmpty(floorRotation.Text))
			{
				floorAngleControl.Angle = AngleControlEx.NO_ANGLE;
				
				foreach(Sector s in sectors) 
				{
					UniFields.SetFloat(s.Fields, "rotationfloor", sectorprops[s].FloorRotation, 0.0);
					s.UpdateNeeded = true;
				}
			} 
			else //update values
			{
				floorAngleControl.Angle = (int)General.ClampAngle(360 - floorRotation.GetResultFloat(0));
				
				foreach(Sector s in sectors) 
				{
					UniFields.SetFloat(s.Fields, "rotationfloor", floorRotation.GetResultFloat(sectorprops[s].FloorRotation), 0.0);
					s.UpdateNeeded = true;
				}
			}

			General.Map.IsChanged = true;
			if(OnValuesChanged != null)	OnValuesChanged(this, EventArgs.Empty);
		}

		private void ceilRotation_WhenTextChanged(object sender, EventArgs e) 
		{
			if(preventchanges) return;
			MakeUndo(); //mxd

			// Reset increment steps, otherwise it's just keep counting and counting
			ceilRotation.ResetIncrementStep();

			//restore values
			if (string.IsNullOrEmpty(ceilRotation.Text))
			{
				ceilAngleControl.Angle = AngleControlEx.NO_ANGLE;
				
				foreach(Sector s in sectors) 
				{
					UniFields.SetFloat(s.Fields, "rotationceiling", sectorprops[s].CeilRotation, 0.0);
					s.UpdateNeeded = true;
				}
			} 
			else //update values
			{
				ceilAngleControl.Angle = (int)General.ClampAngle(360 - ceilRotation.GetResultFloat(0));
				
				foreach(Sector s in sectors) 
				{
					UniFields.SetFloat(s.Fields, "rotationceiling", ceilRotation.GetResultFloat(sectorprops[s].CeilRotation), 0.0);
					s.UpdateNeeded = true;
				}
			}

			General.Map.IsChanged = true;
			if(OnValuesChanged != null)	OnValuesChanged(this, EventArgs.Empty);
		}

		private void lightColor_OnValueChanged(object sender, EventArgs e) 
		{
			if(preventchanges) return;
			MakeUndo(); //mxd

			foreach(Sector s in sectors) 
			{
				lightColor.ApplyTo(s.Fields, sectorprops[s].LightColor);
				s.UpdateNeeded = true;
			}

			General.Map.IsChanged = true;
			if(OnValuesChanged != null)	OnValuesChanged(this, EventArgs.Empty);
		}

		private void fadeColor_OnValueChanged(object sender, EventArgs e) 
		{
			if(preventchanges) return;
			MakeUndo(); //mxd

			foreach(Sector s in sectors) 
			{
				fadeColor.ApplyTo(s.Fields, sectorprops[s].FadeColor);
				s.UpdateNeeded = true;
			}

			General.Map.IsChanged = true;
			if(OnValuesChanged != null)	OnValuesChanged(this, EventArgs.Empty);
		}

		#endregion

		#region ================== Ceiling/Floor realtime events (mxd)

		private void ceilOffsets_OnValuesChanged(object sender, EventArgs e) 
		{
			if(preventchanges) return;
			MakeUndo(); //mxd

			// Reset increment steps, otherwise it's just keep counting and counting
			ceilOffsets.ResetIncrementStep();

			foreach (Sector s in sectors) 
			{
				ceilOffsets.ApplyTo(s.Fields, General.Map.FormatInterface.MinTextureOffset, General.Map.FormatInterface.MaxTextureOffset, sectorprops[s].CeilOffsetX, sectorprops[s].CeilOffsetY);
				s.UpdateNeeded = true;
			}

			General.Map.IsChanged = true;
			if(OnValuesChanged != null)	OnValuesChanged(this, EventArgs.Empty);
		}

		private void floorOffsets_OnValuesChanged(object sender, EventArgs e) 
		{
			if(preventchanges) return;
			MakeUndo(); //mxd

			// Reset increment steps, otherwise it's just keep counting and counting
			floorOffsets.ResetIncrementStep();

			foreach (Sector s in sectors) 
			{
				floorOffsets.ApplyTo(s.Fields, General.Map.FormatInterface.MinTextureOffset, General.Map.FormatInterface.MaxTextureOffset, sectorprops[s].FloorOffsetX, sectorprops[s].FloorOffsetY);
				s.UpdateNeeded = true;
			}

			General.Map.IsChanged = true;
			if(OnValuesChanged != null)	OnValuesChanged(this, EventArgs.Empty);
		}

		private void ceilScale_OnValuesChanged(object sender, EventArgs e) 
		{
			if(preventchanges) return;
			MakeUndo(); //mxd

			// Reset increment steps, otherwise it's just keep counting and counting
			ceilScale.ResetIncrementStep();

			foreach (Sector s in sectors) 
			{
				ceilScale.ApplyTo(s.Fields, General.Map.FormatInterface.MinTextureOffset, General.Map.FormatInterface.MaxTextureOffset, sectorprops[s].CeilScaleX, sectorprops[s].CeilScaleY);
				s.UpdateNeeded = true;
			}

			General.Map.IsChanged = true;
			if(OnValuesChanged != null)	OnValuesChanged(this, EventArgs.Empty);
		}

		private void floorScale_OnValuesChanged(object sender, EventArgs e) 
		{
			if(preventchanges) return;
			MakeUndo(); //mxd

			// Reset increment steps, otherwise it's just keep counting and counting
			floorScale.ResetIncrementStep();

			foreach (Sector s in sectors) 
			{
				floorScale.ApplyTo(s.Fields, General.Map.FormatInterface.MinTextureOffset, General.Map.FormatInterface.MaxTextureOffset, sectorprops[s].FloorScaleX, sectorprops[s].FloorScaleY);
				s.UpdateNeeded = true;
			}

			General.Map.IsChanged = true;
			if(OnValuesChanged != null)	OnValuesChanged(this, EventArgs.Empty);
		}

		private void ceilBrightness_WhenTextChanged(object sender, EventArgs e) 
		{
			if(preventchanges) return;
			MakeUndo(); //mxd

			// Reset increment steps, otherwise it's just keep counting and counting
			ceilBrightness.ResetIncrementStep();

			//restore values
			if (string.IsNullOrEmpty(ceilBrightness.Text)) 
			{
				foreach(Sector s in sectors) 
				{
					UniFields.SetInteger(s.Fields, "lightceiling", sectorprops[s].CeilBrightness, 0);
					s.UpdateNeeded = true;
				}
			} 
			else //update values
			{
				foreach(Sector s in sectors) 
				{
					bool absolute = false;
					switch(ceilLightAbsolute.CheckState)
					{
						case CheckState.Indeterminate:
							absolute = s.Fields.GetValue("lightceilingabsolute", false);
							break;
						case CheckState.Checked:
							absolute = true;
							break;
					}

					int value = General.Clamp(ceilBrightness.GetResult(sectorprops[s].CeilBrightness), (absolute ? 0 : -255), 255);
					UniFields.SetInteger(s.Fields, "lightceiling", value, 0);
					s.UpdateNeeded = true;
				}
			}

			resetceillight.Visible = (ceilLightAbsolute.CheckState != CheckState.Unchecked || ceilBrightness.Text != "0");
			General.Map.IsChanged = true;
			if(OnValuesChanged != null)	OnValuesChanged(this, EventArgs.Empty);
		}

		private void floorBrightness_WhenTextChanged(object sender, EventArgs e) 
		{
			if(preventchanges) return;
			MakeUndo(); //mxd

			// Reset increment steps, otherwise it's just keep counting and counting
			floorBrightness.ResetIncrementStep();

			//restore values
			if (string.IsNullOrEmpty(floorBrightness.Text)) 
			{
				foreach(Sector s in sectors) 
				{
					UniFields.SetInteger(s.Fields, "lightfloor", sectorprops[s].FloorBrightness, 0);
					s.UpdateNeeded = true;
				}
			} 
			else //update values
			{
				foreach(Sector s in sectors) 
				{
					bool absolute = false;
					switch(floorLightAbsolute.CheckState)
					{
						case CheckState.Indeterminate:
							absolute = s.Fields.GetValue("lightfloorabsolute", false);
							break;
						case CheckState.Checked:
							absolute = true;
							break;
					}

					int value = General.Clamp(floorBrightness.GetResult(sectorprops[s].FloorBrightness), (absolute ? 0 : -255), 255);
					UniFields.SetInteger(s.Fields, "lightfloor", value, 0);
					s.UpdateNeeded = true;
				}
			}

			resetfloorlight.Visible = (floorLightAbsolute.CheckState != CheckState.Unchecked || floorBrightness.Text != "0");
			General.Map.IsChanged = true;
			if(OnValuesChanged != null)	OnValuesChanged(this, EventArgs.Empty);
		}

		private void ceilLightAbsolute_CheckedChanged(object sender, EventArgs e) 
		{
			if(preventchanges) return;
			MakeUndo(); //mxd

			if(ceilLightAbsolute.Checked) 
			{
				foreach(Sector s in sectors) 
				{
					s.Fields["lightceilingabsolute"] = new UniValue(UniversalType.Boolean, true);
					s.UpdateNeeded = true;
				}
			} 
			else if(ceilLightAbsolute.CheckState == CheckState.Indeterminate) 
			{
				foreach(Sector s in sectors) 
				{
					if(sectorprops[s].CeilLightAbsoulte) 
					{
						s.Fields["lightceilingabsolute"] = new UniValue(UniversalType.Boolean, true);
						s.UpdateNeeded = true;
					}
					else if(s.Fields.ContainsKey("lightceilingabsolute")) 
					{
						s.Fields.Remove("lightceilingabsolute");
						s.UpdateNeeded = true;
					}
				}
			} 
			else 
			{
				foreach(Sector s in sectors) 
				{
					if(s.Fields.ContainsKey("lightceilingabsolute")) 
					{
						s.Fields.Remove("lightceilingabsolute");
						s.UpdateNeeded = true;
					}
				}
			}

			resetceillight.Visible = (ceilLightAbsolute.CheckState != CheckState.Unchecked || ceilBrightness.Text != "0");
			General.Map.IsChanged = true;
			if(OnValuesChanged != null)	OnValuesChanged(this, EventArgs.Empty);
		}

		private void floorLightAbsolute_CheckedChanged(object sender, EventArgs e) 
		{
			if(preventchanges) return;
			MakeUndo(); //mxd

			if(floorLightAbsolute.Checked)
			{
				foreach(Sector s in sectors) 
				{
					s.Fields["lightfloorabsolute"] = new UniValue(UniversalType.Boolean, true);
					s.UpdateNeeded = true;
				}
			} 
			else if(floorLightAbsolute.CheckState == CheckState.Indeterminate) 
			{
				foreach(Sector s in sectors) 
				{
					if(sectorprops[s].FloorLightAbsoulte) 
					{
						s.Fields["lightfloorabsolute"] = new UniValue(UniversalType.Boolean, true);
						s.UpdateNeeded = true;
					} 
					else if(s.Fields.ContainsKey("lightfloorabsolute")) 
					{
						s.Fields.Remove("lightfloorabsolute");
						s.UpdateNeeded = true;
					}
				}
			} 
			else 
			{
				foreach(Sector s in sectors) 
				{
					if(s.Fields.ContainsKey("lightfloorabsolute")) 
					{
						s.Fields.Remove("lightfloorabsolute");
						s.UpdateNeeded = true;
					}
				}
			}

			resetfloorlight.Visible = (floorLightAbsolute.CheckState != CheckState.Unchecked || floorBrightness.Text != "0");
			General.Map.IsChanged = true;
			if(OnValuesChanged != null)	OnValuesChanged(this, EventArgs.Empty);
		}

		private void resetceillight_Click(object sender, EventArgs e)
		{
			MakeUndo(); //mxd

			preventchanges = true;

			ceilLightAbsolute.Checked = false;
			ceilBrightness.Text = "0";

			foreach(Sector s in sectors)
			{
				if(s.Fields.ContainsKey("lightceilingabsolute")) s.Fields.Remove("lightceilingabsolute");
				if(s.Fields.ContainsKey("lightceiling")) s.Fields.Remove("lightceiling");
			}

			preventchanges = false;

			resetceillight.Visible = false;
			ceilBrightness.Focus();
			General.Map.IsChanged = true;
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
		}

		private void resetfloorlight_Click(object sender, EventArgs e)
		{
			MakeUndo(); //mxd

			preventchanges = true;

			floorLightAbsolute.Checked = false;
			floorBrightness.Text = "0";

			foreach(Sector s in sectors)
			{
				if(s.Fields.ContainsKey("lightfloorabsolute")) s.Fields.Remove("lightfloorabsolute");
				if(s.Fields.ContainsKey("lightfloor")) s.Fields.Remove("lightfloor");
			}

			preventchanges = false;

			resetfloorlight.Visible = false;
			floorBrightness.Focus();
			General.Map.IsChanged = true;
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
		}

		private void fogdensity_WhenTextChanged(object sender, EventArgs e)
		{
			if(preventchanges) return;
			MakeUndo();

			// Reset increment steps, otherwise it's just keep counting and counting
			fogdensity.ResetIncrementStep();

			// Restore values
			if (string.IsNullOrEmpty(fogdensity.Text))
			{
				foreach(Sector s in sectors)
				{
					UniFields.SetInteger(s.Fields, "fogdensity", sectorprops[s].FogDensity, 0);
					s.UpdateNeeded = true;
				}
			}
			else // Update values
			{
				foreach(Sector s in sectors)
				{
					int value = General.Clamp(fogdensity.GetResult(sectorprops[s].FogDensity), 0, 510);
					UniFields.SetInteger(s.Fields, "fogdensity", value, 0);
					s.UpdateNeeded = true;
				}
			}

			General.Map.IsChanged = true;
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
		}

		#endregion

		#region ================== Slope Utility (mxd)

		private void SetupFloorSlope(Sector s, bool first) 
		{
			if(s.FloorSlope.GetLengthSq() > 0) 
			{
				double anglexy = General.ClampAngle(Math.Round(Angle2D.RadToDeg(s.FloorSlope.GetAngleXY()) - 180, 1));
				double anglez = -Math.Round(Angle2D.RadToDeg(s.FloorSlope.GetAngleZ()) - 90, 1);
				double offset = Math.Round(GetVirtualSlopeOffset(s, floorslopecontrol.PivotMode, true), 1);

				if(anglexy >= 180 && anglez < 0) 
				{
					anglexy -= 180;
					anglez = -anglez;
				}

				floorslopecontrol.SetValues(anglexy, anglez, offset, first);
			} 
			else 
			{
				floorslopecontrol.SetValues(0.0, 0.0, s.FloorHeight, first);
			}
		}

		private void SetupCeilingSlope(Sector s, bool first) 
		{
			if(s.CeilSlope.GetLengthSq() > 0) 
			{
				double anglexy = General.ClampAngle(Math.Round(Angle2D.RadToDeg(s.CeilSlope.GetAngleXY()) - 180, 1));
				double anglez = -(270 - Math.Round(Angle2D.RadToDeg(s.CeilSlope.GetAngleZ()), 1));
				double offset = Math.Round(GetVirtualSlopeOffset(s, ceilingslopecontrol.PivotMode, false), 1);

				if(anglexy >= 180 && anglez < 0) 
				{
					anglexy -= 180;
					anglez = -anglez;
				}

				ceilingslopecontrol.SetValues(anglexy, anglez, offset, first);
			} 
			else 
			{
				ceilingslopecontrol.SetValues(0.0, 0.0, s.CeilHeight, first);
			}
		}

		// Gets the offset to be displayed in a SectorSlopeControl
		private double GetVirtualSlopeOffset(Sector s, SlopePivotMode mode, bool floor) 
		{
			double offset = (floor ? s.FloorSlopeOffset : s.CeilSlopeOffset);
			if(double.IsNaN(offset))
			{
				offset = (floor ? s.FloorHeight : s.CeilHeight);
			}

			Vector3D normal = (floor ? s.FloorSlope : s.CeilSlope);
			
			if(normal.GetLengthSq() > 0)
			{
				Vector3D center = GetSectorCenter(s, 0, mode);
				Plane p = new Plane(normal, offset);
				return p.GetZ(center);
			}

			return offset;
		}

		// Gets the offset to be displayed in a SectorSlopeControl
		private double GetInitialVirtualSlopeOffset(Sector s, SlopePivotMode mode, bool floor) 
		{
			double offset = (floor ? sectorprops[s].FloorSlopeOffset : sectorprops[s].CeilSlopeOffset);
			Vector3D normal = (floor ? sectorprops[s].FloorSlope : sectorprops[s].CeilSlope);

			if(normal.GetLengthSq() > 0) 
			{
				Vector3D center = GetSectorCenter(s, 0, mode);
				Plane p = new Plane(normal, offset);
				return p.GetZ(center);
			}

			return offset;
		}

		private Vector3D GetSectorCenter(Sector s, double offset, SlopePivotMode mode)
		{
			switch(mode) 
			{
				case SlopePivotMode.GLOBAL: //translate from the center of selection 
					return new Vector3D(globalslopepivot, offset);

				case SlopePivotMode.LOCAL: //translate from sector's bounding box center
					return new Vector3D(slopepivots[s], offset);

				case SlopePivotMode.ORIGIN: //don't translate
					return new Vector3D(0, 0, offset);

				default:
					throw new NotImplementedException("Unknown SlopePivotMode: " + (int)mode);
			}
		}

		#endregion

		#region ================== Slopes realtime events (mxd)

		private void ceilingslopecontrol_OnAnglesChanged(object sender, EventArgs e) 
		{
			if(preventchanges) return;
			MakeUndo(); //mxd

			//Set or restore values
			foreach(Sector s in sectors) 
			{
				double anglexy = General.ClampAngle(ceilingslopecontrol.GetAngleXY(sectorprops[s].CeilSlopeAngleXY) + 270);
				double anglez = -(ceilingslopecontrol.GetAngleZ(sectorprops[s].CeilSlopeAngleZ) + 90);

				double virtualoffset = GetInitialVirtualSlopeOffset(s, ceilingslopecontrol.PivotMode, false);
				Vector3D center = GetSectorCenter(s, ceilingslopecontrol.GetOffset(virtualoffset), ceilingslopecontrol.PivotMode);
				Plane p = new Plane(center, Angle2D.DegToRad(anglexy), Angle2D.DegToRad(anglez), false);
				s.CeilSlope = p.Normal;
				s.CeilSlopeOffset = p.Offset;

				s.UpdateNeeded = true;
			}

			General.Map.IsChanged = true;
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
		}

		private void floorslopecontrol_OnAnglesChanged(object sender, EventArgs e) 
		{
			if(preventchanges) return;
			MakeUndo(); //mxd

			//Set or restore values
			foreach(Sector s in sectors)
			{
				double anglexy = General.ClampAngle(floorslopecontrol.GetAngleXY(sectorprops[s].FloorSlopeAngleXY) + 90);
				double anglez = -(floorslopecontrol.GetAngleZ(sectorprops[s].FloorSlopeAngleZ) + 90);

				double virtualoffset = GetInitialVirtualSlopeOffset(s, floorslopecontrol.PivotMode, true);
				Vector3D center = GetSectorCenter(s, floorslopecontrol.GetOffset(virtualoffset), floorslopecontrol.PivotMode);
				Plane p = new Plane(center, Angle2D.DegToRad(anglexy), Angle2D.DegToRad(anglez), true);
				s.FloorSlope = p.Normal;
				s.FloorSlopeOffset = p.Offset;

				s.UpdateNeeded = true;
			}

			General.Map.IsChanged = true;
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
		}

		// Update displayed ceiling offset value
		private void ceilingslopecontrol_OnPivotModeChanged(object sender, EventArgs e) 
		{
			MakeUndo(); //mxd
			bool first = true;
			foreach(Sector s in sectors)
			{
				SetupCeilingSlope(s, first);
				first = false;
			}
		}

		// Update displayed floor offset value
		private void floorslopecontrol_OnPivotModeChanged(object sender, EventArgs e) 
		{
			MakeUndo(); //mxd
			bool first = true;
			foreach(Sector s in sectors)
			{
				SetupFloorSlope(s, first);
				first = false;
			}
		}

		private void ceilingslopecontrol_OnResetClicked(object sender, EventArgs e) 
		{
			MakeUndo(); //mxd
			Sector fs = General.GetByIndex(sectors, 0);

			// biwa. Do not reset to the z position of the plane of the center of the sector anymore, since 
			// that will result in pretty crazy values of 3D floor control sectors
			/*
			if (fs.CeilSlope.IsNormalized())
			{
				fs.UpdateBBox();
				Plane p = new Plane(fs.CeilSlope, fs.CeilSlopeOffset);
				ceilingslopecontrol.SetOffset((int)Math.Round(p.GetZ(fs.BBox.X + fs.BBox.Width / 2, fs.BBox.Y + fs.BBox.Height / 2)), true);
			}
			else
				ceilingslopecontrol.SetOffset(fs.CeilHeight, true);
			*/
			ceilingslopecontrol.SetOffset(fs.CeilHeight, true);

			foreach (Sector s in sectors) 
			{
				// biwa. Do not reset to the z position of the plane of the center of the sector anymore, since 
				// that will result in pretty crazy values of 3D floor control sectors
				/*
				if (s.CeilSlope.IsNormalized())
				{
					s.UpdateBBox();
					Plane p = new Plane(s.CeilSlope, s.CeilSlopeOffset);
					s.CeilHeight = (int)Math.Round(p.GetZ(s.BBox.X + s.BBox.Width / 2, s.BBox.Y + s.BBox.Height / 2));
				}
				*/

				s.CeilSlope = new Vector3D();
				s.CeilSlopeOffset = double.NaN;
				s.UpdateNeeded = true;
				ceilingslopecontrol.SetOffset(s.CeilHeight, false);
			}

			ceilingslopecontrol.UpdateOffset();

			General.Map.IsChanged = true;
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
		}

		private void floorslopecontrol_OnResetClicked(object sender, EventArgs e) 
		{
			MakeUndo(); //mxd
			Sector fs = General.GetByIndex(sectors, 0);

			// biwa. Do not reset to the z position of the plane of the center of the sector anymore, since 
			// that will result in pretty crazy values of 3D floor control sectors
			/*
			if (fs.FloorSlope.IsNormalized())
			{
				fs.UpdateBBox();
				Plane p = new Plane(fs.FloorSlope, fs.FloorSlopeOffset);
				floorslopecontrol.SetOffset((int)Math.Round(p.GetZ(fs.BBox.X + fs.BBox.Width / 2, fs.BBox.Y + fs.BBox.Height / 2)), true);
			}
			else
				floorslopecontrol.SetOffset(fs.FloorHeight, true);
			*/

			floorslopecontrol.SetOffset(fs.FloorHeight, true);

			foreach (Sector s in sectors) 
			{
				// biwa. Do not reset to the z position of the plane of the center of the sector anymore, since 
				// that will result in pretty crazy values of 3D floor control sectors
				/*
				if (s.FloorSlope.IsNormalized())
				{
					s.UpdateBBox();
					Plane p = new Plane(s.FloorSlope, s.FloorSlopeOffset);
					s.FloorHeight = (int)Math.Round(p.GetZ(s.BBox.X + s.BBox.Width / 2, s.BBox.Y + s.BBox.Height / 2));
				}
				*/

				s.FloorSlope = new Vector3D();
				s.FloorSlopeOffset = double.NaN;
				s.UpdateNeeded = true;
				floorslopecontrol.SetOffset(s.FloorHeight, false);
			}

			floorslopecontrol.UpdateOffset();

			General.Map.IsChanged = true;
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
		}

		private void ceilingslopecontrol_OnUseLineAnglesChanged(object sender, EventArgs e) 
		{
			ceilingslopecontrol.StepValues = (ceilingslopecontrol.UseLineAngles ? anglesteps : null);
		}

		private void floorslopecontrol_OnUseLineAnglesChanged(object sender, EventArgs e) 
		{
			floorslopecontrol.StepValues = (floorslopecontrol.UseLineAngles ? anglesteps : null);
		}

		#endregion

		#region ================== Glow realtime events (mxd)

		private void UpdateCeilingGlowHeightWarning()
		{
			ceilingglowheightrequired.Visible = (ceilingglowcolor.Color.WithAlpha(0).ToInt() != ceilingglowcolor.DefaultValue
				&& ceilingglowheight.GetResultFloat(0f) == 0f);
		}

		private void UpdateFloorGlowHeightWarning()
		{
			floorglowheightrequired.Visible = (floorglowcolor.Color.WithAlpha(0).ToInt() != floorglowcolor.DefaultValue
				&& floorglowheight.GetResultFloat(0f) == 0f);
		}

		private void ceilingglowcolor_OnValueChanged(object sender, EventArgs e)
		{
			if(preventchanges) return;
			MakeUndo();

			foreach(Sector s in sectors)
			{
				ceilingglowcolor.ApplyTo(s.Fields, sectorprops[s].CeilGlowColor);
				s.UpdateNeeded = true;
			}

			// Show height warning?
			UpdateCeilingGlowHeightWarning();

			General.Map.IsChanged = true;
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
		}

		private void floorglowcolor_OnValueChanged(object sender, EventArgs e)
		{
			if(preventchanges) return;
			MakeUndo();

			foreach(Sector s in sectors)
			{
				floorglowcolor.ApplyTo(s.Fields, sectorprops[s].FloorGlowColor);
				s.UpdateNeeded = true;
			}

			// Show height warning?
			UpdateFloorGlowHeightWarning();

			General.Map.IsChanged = true;
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
		}

		private void ceilingglowheight_WhenTextChanged(object sender, EventArgs e)
		{
			if(preventchanges) return;
			MakeUndo();

			// Restore values
			if(string.IsNullOrEmpty(ceilingglowheight.Text))
			{
				foreach(Sector s in sectors)
				{
					UniFields.SetFloat(s.Fields, "ceilingglowheight", sectorprops[s].CeilGlowHeight, 0.0);
					s.UpdateNeeded = true;
				}
			}
			else // Update values
			{
				foreach(Sector s in sectors)
				{
					double value = General.Clamp(ceilingglowheight.GetResultFloat(sectorprops[s].CeilGlowHeight), 0.0, double.MaxValue);
					UniFields.SetFloat(s.Fields, "ceilingglowheight", value, 0.0);
					s.UpdateNeeded = true;
				}
			}

			// Update "Reset" button
			resetceilingglowheight.Visible = (ceilingglowheight.GetResultFloat(0.0) != 0.0);

			// Show height warning?
			UpdateCeilingGlowHeightWarning();

			General.Map.IsChanged = true;
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
		}

		private void floorglowheight_WhenTextChanged(object sender, EventArgs e)
		{
			if(preventchanges) return;
			MakeUndo();

			// Restore values
			if(string.IsNullOrEmpty(floorglowheight.Text))
			{
				foreach(Sector s in sectors)
				{
					UniFields.SetFloat(s.Fields, "floorglowheight", sectorprops[s].FloorGlowHeight, 0.0);
					s.UpdateNeeded = true;
				}
			}
			else // Update values
			{
				foreach(Sector s in sectors)
				{
					double value = General.Clamp(floorglowheight.GetResultFloat(sectorprops[s].FloorGlowHeight), 0.0, double.MaxValue);
					UniFields.SetFloat(s.Fields, "floorglowheight", value, 0.0);
					s.UpdateNeeded = true;
				}
			}

			// Update "Reset" button
			resetfloorglowheight.Visible = (floorglowheight.GetResultFloat(0.0) != 0.0);

			// Show height warning?
			UpdateFloorGlowHeightWarning();

			General.Map.IsChanged = true;
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
		}


        private void ceilingGlowEnabled_CheckedChanged(object sender, EventArgs e)
        {
            if (preventchanges) return;
            MakeUndo();

            // Update controls
            ceilingglowcolor.Enabled = ceilingGlowEnabled.Checked;
            ceilingglowcolor.Color = PixelColor.FromInt(0);
            ceilingglowheight.Enabled = ceilingGlowEnabled.Checked;
            ceilingglowheightlabel.Enabled = ceilingGlowEnabled.Checked;

            if (!ceilingGlowEnabled.Checked)
            {
                // Set glow color to -1
                foreach (Sector s in sectors)
                {
                    UniFields.SetInteger(s.Fields, "ceilingglowcolor", -1, 0);
                    s.UpdateNeeded = true;
                }

                // Hide height warning
                ceilingglowheightrequired.Visible = false;

                // Trigger update
                General.Map.IsChanged = true;
                if (OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
            }
            else
            {
                // Trigger update to restore/update values
                ceilingglowcolor_OnValueChanged(this, EventArgs.Empty);
            }
        }

        private void floorGlowEnabled_CheckedChanged(object sender, EventArgs e)
        {
            if (preventchanges) return;
            MakeUndo();

            // Update controls
            floorglowcolor.Enabled = floorGlowEnabled.Checked;
            floorglowcolor.Color = PixelColor.FromInt(0);
            floorglowheight.Enabled = floorGlowEnabled.Checked;
            floorglowheightlabel.Enabled = floorGlowEnabled.Checked;

            if (!floorGlowEnabled.Checked)
            {
                // Set glow color to -1
                foreach (Sector s in sectors)
                {
                    UniFields.SetInteger(s.Fields, "floorglowcolor", -1, 0);
                    s.UpdateNeeded = true;
                }

                // Hide height warning
                floorglowheightrequired.Visible = false;

                // Trigger update
                General.Map.IsChanged = true;
                if (OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
            }
            else
            {
                // Trigger glow color update to restore/update values
                floorglowcolor_OnValueChanged(this, EventArgs.Empty);
            }
        }

        private void resetceilingglowheight_Click(object sender, EventArgs e)
		{
			ceilingglowheight.Focus();
			ceilingglowheight.Text = "0";
		}

		private void resetfloorglowheight_Click(object sender, EventArgs e)
		{
			floorglowheight.Focus();
			floorglowheight.Text = "0";
		}

        #endregion
        #region ================== D64 colors realtime events (mxd)

        // generic function: use sender
        private void d64color_OnValueChanged(object sender, EventArgs e)
        {
            if (preventchanges) return;
            MakeUndo();

            ColorFieldsControl colorCtl = (ColorFieldsControl)sender;

            foreach (Sector s in sectors)
            {
                int prevv = PixelColor.INT_WHITE_NO_ALPHA;
                SectorProperties props = sectorprops[s];
                switch (colorCtl.Field)
                {
                    case "color_ceiling":
                        prevv = props.D64ColorCeiling;
                        break;
                    case "color_walltop":
                        prevv = props.D64ColorWallTop;
                        break;
                    case "color_sprites":
                        prevv = props.D64ColorThings;
                        break;
                    case "color_wallbottom":
                        prevv = props.D64ColorWallBottom;
                        break;
                    case "color_floor":
                        prevv = props.D64ColorFloor;
                        break;
                }
                colorCtl.ApplyTo(s.Fields, prevv);
                s.UpdateNeeded = true;
            }

            General.Map.IsChanged = true;
            if (OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
        }

        #endregion
    }
}
