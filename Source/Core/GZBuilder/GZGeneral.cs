#region ================== Namespaces

using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.GZBuilder.Data;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.ZDoom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

#endregion

namespace CodeImp.DoomBuilder.GZBuilder
{
	//mxd. should get rid of this class one day...
	public static class GZGeneral
    {
        #region ================== Properties

        //gzdoom light types

        public class LightDefNum : Attribute
        {
            public int[] DoomEdNums { get; private set; }

            public LightDefNum(params int[] doomEdNums)
            {
                DoomEdNums = doomEdNums;
            }
        }

        public class LightDefClass : Attribute
        {
            public string[] Classes { get; private set; }

            public LightDefClass(params string[] clses)
            {
                Classes = clses;
            }
        }

        public class LightDefModifier : Attribute
        {
            public LightModifier[] Modifiers { get; private set; }
            
            public LightDefModifier(params LightModifier[] mods)
            {
                Modifiers = mods;
            }
        }
        
        public class LightDefRenderStyle : Attribute
        {
            public LightRenderStyle RenderStyle { get; private set; }

            public LightDefRenderStyle(LightRenderStyle rs)
            {
                RenderStyle = rs;
            }
        }

        public enum LightDef
        {
            [LightDefRenderStyle(LightRenderStyle.NORMAL)]
            [LightDefNum(9800, 9801, 9802, 9803, 9804)]
            [LightDefClass("pointlight", "pointlightpulse", "pointlightflicker", "sectorpointlight", "pointlightflickerrandom")]
            [LightDefModifier(LightModifier.NORMAL, LightModifier.PULSE, LightModifier.FLICKER, LightModifier.SECTOR, LightModifier.FLICKERRANDOM)]
            POINT_NORMAL,

            [LightDefRenderStyle(LightRenderStyle.ADDITIVE)]
            [LightDefNum(9810, 9811, 9812, 9813, 9814)]
            [LightDefClass("pointlightadditive", "pointlightpulseadditive", "pointlightflickeradditive", "sectorpointlightadditive", "pointlightflickerrandomadditive")]
            [LightDefModifier(LightModifier.NORMAL, LightModifier.PULSE, LightModifier.FLICKER, LightModifier.SECTOR, LightModifier.FLICKERRANDOM)]
            POINT_ADDITIVE,

            [LightDefRenderStyle(LightRenderStyle.SUBTRACTIVE)]
            [LightDefNum(9820, 9821, 9822, 9823, 9824)]
            [LightDefClass("pointlightsubtractive", "pointlightpulsesubtractive", "pointlightflickersubtractive", "sectorpointlightsubtractive", "pointlightflickerrandomsubtractive")]
            [LightDefModifier(LightModifier.NORMAL, LightModifier.PULSE, LightModifier.FLICKER, LightModifier.SECTOR, LightModifier.FLICKERRANDOM)]
            POINT_SUBTRACTIVE,

            [LightDefRenderStyle(LightRenderStyle.ATTENUATED)]
            [LightDefNum(9830, 9831, 9832, 9833, 9834)]
            [LightDefClass("pointlightattenuated", "pointlightpulseattenuated", "pointlightflickerattenuated", "sectorpointlightattenuated", "pointlightflickerrandomattenuated")]
            [LightDefModifier(LightModifier.NORMAL, LightModifier.PULSE, LightModifier.FLICKER, LightModifier.SECTOR, LightModifier.FLICKERRANDOM)]
            POINT_ATTENUATED,

            [LightDefRenderStyle(LightRenderStyle.NORMAL)]
            [LightDefNum(9840, 9841, 9842, 9843, 8944)]
            [LightDefClass("spotlight", "spotlightpulse", "spotlightflicker", "sectorspotlight", "spotlightflickerrandom")]
            [LightDefModifier(LightModifier.NORMAL, LightModifier.PULSE, LightModifier.FLICKER, LightModifier.SECTOR, LightModifier.FLICKERRANDOM)]
            SPOT_NORMAL,

            [LightDefRenderStyle(LightRenderStyle.ADDITIVE)]
            [LightDefNum(9850, 9851, 9852, 9853, 8954)]
            [LightDefClass("spotlightadditive", "spotlightpulseadditive", "spotlightflickeradditive", "sectorspotlightadditive", "spotlightflickerrandomadditive")]
            [LightDefModifier(LightModifier.NORMAL, LightModifier.PULSE, LightModifier.FLICKER, LightModifier.SECTOR, LightModifier.FLICKERRANDOM)]
            SPOT_ADDITIVE,

            [LightDefRenderStyle(LightRenderStyle.SUBTRACTIVE)]
            [LightDefNum(9860, 9861, 9862, 9863, 8964)]
            [LightDefClass("spotlightsubtractive", "spotlightpulsesubtractive", "spotlightflickersubtractive", "sectorspotlightsubtractive", "spotlightflickerrandomsubtractive")]
            [LightDefModifier(LightModifier.NORMAL, LightModifier.PULSE, LightModifier.FLICKER, LightModifier.SECTOR, LightModifier.FLICKERRANDOM)]
            SPOT_SUBTRACTIVE,

            [LightDefRenderStyle(LightRenderStyle.ATTENUATED)]
            [LightDefNum(9870, 9871, 9872, 9873, 8974)]
            [LightDefClass("spotlightattenuated", "spotlightpulseattenuated", "spotlightflickerattenuated", "sectorspotlightattenuated", "spotlightflickerrandomattenuated")]
            [LightDefModifier(LightModifier.NORMAL, LightModifier.PULSE, LightModifier.FLICKER, LightModifier.SECTOR, LightModifier.FLICKERRANDOM)]
            SPOT_ATTENUATED,

            [LightDefRenderStyle(LightRenderStyle.VAVOOM)]
            [LightDefNum(1502)]
            [LightDefClass("vavoomlightwhite")]
            VAVOOM_GENERIC,

            [LightDefRenderStyle(LightRenderStyle.VAVOOM)]
            [LightDefNum(1503)]
            [LightDefClass("vavoomlightcolor")]
            VAVOOM_COLORED,

            [LightDefRenderStyle(LightRenderStyle.LIGHTMAP)]
            [LightDefNum(9876, 9877, 9878, 9879)]
            [LightDefClass("pointlighttraceattenuated", "pointlightpulsetraceattenuated", "pointlightflickertraceattenuated", "pointlightflickerrandomtraceattenuated")]
            [LightDefModifier(LightModifier.NORMAL, LightModifier.PULSE, LightModifier.FLICKER, LightModifier.FLICKERRANDOM)]
            POINT_LIGHTMAP,

            [LightDefRenderStyle(LightRenderStyle.LIGHTMAP)]
            [LightDefNum(9881, 9882, 9883, 9884)]
            [LightDefClass("spotlighttraceattenuated", "spotlightpulsetraceattenuated", "spotlightflickertraceattenuated", "spotlightflickerrandomtraceattenuated")]
            [LightDefModifier(LightModifier.NORMAL, LightModifier.PULSE, LightModifier.FLICKER, LightModifier.FLICKERRANDOM)]
            SPOT_LIGHTMAP,

            [LightDefRenderStyle(LightRenderStyle.NONE)]
            [LightDefNum(9890)]
            [LightDefClass("zdraysun")]
            [LightDefModifier(LightModifier.NORMAL)]
            SUN,

            UNKNOWN
        }

        // divide these by 100 to get light color alpha
        // this, sadly, has to duplicate the enum in GZGeneral because it's shader-specific
        public enum LightRenderStyle
        {
            SUBTRACTIVE = 100,
            NORMAL = 99,
            ATTENUATED = 98,
            VAVOOM = 50,
            ADDITIVE = 25,
            LIGHTMAP = 98, // Same as attenuated
            NONE = 0,
        }

        public enum LightModifier
        {
            NORMAL,
            PULSE,
            FLICKER,
            SECTOR,
            FLICKERRANDOM
        }

        public enum LightType
        {
            POINT,
            SPOT,
            VAVOOM,
            SUN
        }

        public static LightDefNum GetLightDefNum(LightDef d)
        {
            FieldInfo fi = typeof(LightDef).GetField(d.ToString());
            LightDefNum[] attrs = (LightDefNum[])fi.GetCustomAttributes(typeof(LightDefNum), false);
            if (attrs.Length != 0)
                return attrs[0];
            return null;
        }

        public static LightDefClass GetLightDefClass(LightDef d)
        {
            FieldInfo fi = typeof(LightDef).GetField(d.ToString());
            LightDefClass[] attrs = (LightDefClass[])fi.GetCustomAttributes(typeof(LightDefClass), false);
            if (attrs.Length != 0)
                return attrs[0];
            return null;
        }

        public static LightDefModifier GetLightDefModifier(LightDef d)
        {
            FieldInfo fi = typeof(LightDef).GetField(d.ToString());
            LightDefModifier[] attrs = (LightDefModifier[])fi.GetCustomAttributes(typeof(LightDefModifier), false);
            if (attrs.Length != 0)
                return attrs[0];
            return null;
        }

        public static LightDefRenderStyle GetLightDefRenderStyle(LightDef d)
        {
            FieldInfo fi = typeof(LightDef).GetField(d.ToString());
            LightDefRenderStyle[] attrs = (LightDefRenderStyle[])fi.GetCustomAttributes(typeof(LightDefRenderStyle), false);
            if (attrs.Length != 0)
                return attrs[0];
            return null;
        }

        public class LightData
        {
            public LightDef LightDef { get; private set; }
            private LightDefNum LightDefNum;
            private LightDefClass LightDefClass;
            private LightDefModifier LightDefModifier;
            private LightDefRenderStyle LightDefRenderStyle;
            public string LightClass { get; private set; }
            public int LightNum { get; private set; }
            public LightModifier LightModifier { get; private set; }
            public LightRenderStyle LightRenderStyle { get; private set; }
            public bool LightAnimated { get; private set; }
            public bool LightInternal { get; private set; }
            public bool LightVavoom { get; private set; }
            public LightType LightType { get; private set; }

            private void UpdateLightType()
            {
                switch (LightDef)
                {
                    default:
                    case LightDef.POINT_NORMAL:
                    case LightDef.POINT_ADDITIVE:
                    case LightDef.POINT_SUBTRACTIVE:
                    case LightDef.POINT_ATTENUATED:
                    case LightDef.POINT_LIGHTMAP:
                        LightType = LightType.POINT;
                        break;
                    case LightDef.SPOT_NORMAL:
                    case LightDef.SPOT_ADDITIVE:
                    case LightDef.SPOT_SUBTRACTIVE:
                    case LightDef.SPOT_ATTENUATED:
                    case LightDef.SPOT_LIGHTMAP:
                        LightType = LightType.SPOT;
                        break;
                    case LightDef.VAVOOM_GENERIC:
                    case LightDef.VAVOOM_COLORED:
                        LightType = LightType.VAVOOM;
                        break;
                    case LightDef.SUN:
                        LightType = LightType.SUN;
                        break;
                }
            }

            public LightData(LightDef d, int num)
            {
                LightDef = d;
                LightNum = num;
                LightDefNum = GetLightDefNum(LightDef);
                LightDefClass = GetLightDefClass(LightDef);
                LightDefModifier = GetLightDefModifier(LightDef);
                LightDefRenderStyle = GetLightDefRenderStyle(LightDef);
                LightClass = LightDefClass.Classes[Array.IndexOf(LightDefNum.DoomEdNums, LightNum)];
                if (LightDefModifier != null)
                    LightModifier = LightDefModifier.Modifiers[Array.IndexOf(LightDefNum.DoomEdNums, LightNum)];
                else LightModifier = LightModifier.NORMAL;
                if (LightDefRenderStyle != null)
                    LightRenderStyle = LightDefRenderStyle.RenderStyle;
                else LightRenderStyle = LightRenderStyle.NONE;
                LightAnimated = (LightModifier == LightModifier.PULSE || LightModifier == LightModifier.FLICKER || LightModifier == LightModifier.FLICKERRANDOM);
                LightInternal = true;
                UpdateLightType();
                LightVavoom = (LightType == LightType.VAVOOM);
            }

            public LightData(LightDef d, string cls)
            {
                LightDef = d;
                LightClass = cls;
                LightDefNum = GetLightDefNum(LightDef);
                LightDefClass = GetLightDefClass(LightDef);
                LightDefModifier = GetLightDefModifier(LightDef);
                LightDefRenderStyle = GetLightDefRenderStyle(LightDef);
                LightNum = LightDefNum.DoomEdNums[Array.IndexOf(LightDefClass.Classes, cls)];
                if (LightDefModifier != null)
                    LightModifier = LightDefModifier.Modifiers[Array.IndexOf(LightDefClass.Classes, cls)];
                else LightModifier = LightModifier.NORMAL;
                if (LightDefRenderStyle != null)
                    LightRenderStyle = LightDefRenderStyle.RenderStyle;
                else LightRenderStyle = LightRenderStyle.NONE;
                LightAnimated = (LightModifier == LightModifier.PULSE || LightModifier == LightModifier.FLICKER || LightModifier == LightModifier.FLICKERRANDOM);
                LightInternal = true;
                LightVavoom = (LightDef == LightDef.VAVOOM_GENERIC || LightDef == LightDef.VAVOOM_COLORED);
                UpdateLightType();
                LightVavoom = (LightType == LightType.VAVOOM);
            }

            public LightData(LightModifier mod = LightModifier.NORMAL, LightRenderStyle rs = LightRenderStyle.NONE)
            {
                LightDef = LightDef.UNKNOWN;
                LightClass = null;
                LightNum = -1;
                LightModifier = mod;
                LightRenderStyle = rs;
                LightAnimated = (LightModifier == LightModifier.PULSE || LightModifier == LightModifier.FLICKER || LightModifier == LightModifier.FLICKERRANDOM);
                LightInternal = false;
                LightVavoom = false;
                LightType = LightType.POINT; // always point in GLDEFS
            }

            public void SetRenderStyle(LightRenderStyle rs)
            {
                if (LightInternal)
                    return;
                LightRenderStyle = rs;
            }
        }

        static IEnumerable<LightDef> _gldbn_ldefs;
        public static LightData GetLightDataByNum(int doomednum)
        {
            if (_gldbn_ldefs == null)
                _gldbn_ldefs = Enum.GetValues(typeof(LightDef)).Cast<LightDef>();
            foreach (LightDef ldef in _gldbn_ldefs)
            {
                // 
                FieldInfo fi = typeof(LightDef).GetField(ldef.ToString());
                LightDefNum[] attrs = (LightDefNum[])fi.GetCustomAttributes(typeof(LightDefNum), false);
                if (attrs.Length == 0) continue;
                // 
                LightDefNum attr = attrs[0];
                if (Array.IndexOf(attr.DoomEdNums, doomednum) != -1)
                    return new LightData(ldef, doomednum);
            }

            return null;
        }

        public static LightData GetLightDataByClass(string cls)
        {
            cls = cls.ToLowerInvariant();
            if (_gldbn_ldefs == null)
                _gldbn_ldefs = Enum.GetValues(typeof(LightDef)).Cast<LightDef>();
            foreach (LightDef ldef in _gldbn_ldefs)
            {
                // 
                FieldInfo fi = typeof(LightDef).GetField(ldef.ToString());
                LightDefClass[] attrs = (LightDefClass[])fi.GetCustomAttributes(typeof(LightDefClass), false);
                if (attrs.Length == 0) continue;
                // 
                LightDefClass attr = attrs[0];
                if (Array.IndexOf(attr.Classes, cls) != -1)
                    return new LightData(ldef, cls);
            }

            return null;
        }

		//asc script action specials
		private static readonly int[] acsSpecials = { 80, 81, 82, 83, 84, 85, 226 };
		public static int[] ACS_SPECIALS { get { return acsSpecials; } }

        // [ZZ] this is for proper inheritance of lights.
        //      technically this can be found by parsing gzdoom.pk3/mapinfo/common.txt, but I wouldn't do that without a good reason for now.

        public static LightData GetGZLightTypeByClass(ActorStructure actor)
        {
            ActorStructure p = actor;
            while (p != null)
            {
                // found dynamic light type. alter it by actor flags.
                // +MISSILEMORE makes it additive.
                // +MISSILEEVENMORE makes it subtractive.
                // +INCOMBAT makes it attenuated.
                LightData ld = GetLightDataByClass(p.ClassName);
                if (ld != null)
                {
                    if (ld.LightDef != LightDef.VAVOOM_GENERIC && ld.LightDef != LightDef.VAVOOM_COLORED) // not vavoom
                    {
                        int baseType = ld.LightNum % 10;
                        int dispType = ld.LightNum - baseType;
                        if (actor.GetFlagValue("MISSILEMORE", false) || actor.GetFlagValue("DYNAMICLIGHT.ADDITIVE", false))
                            dispType = 9810;
                        else if (actor.GetFlagValue("MISSILEEVENMORE", false) || actor.GetFlagValue("DYNAMICLIGHT.SUBTRACTIVE", false))
                            dispType = 9820;
                        else if (actor.GetFlagValue("INCOMBAT", false) || actor.GetFlagValue("DYNAMICLIGHT.ATTENUATE", false))
                            dispType = 9830;
                        if (!actor.GetFlagValue("DYNAMICLIGHT.SPOT", false) && dispType >= 9840)
                            dispType -= 40;
                        if (actor.GetFlagValue("DYNAMICLIGHT.SPOT", false) && dispType < 9840)
                            dispType += 40;
                        return GetLightDataByNum(dispType + baseType);
                    }
                    else return null;
                }

                p = p.BaseClass;
            }

            return null;
        }

        // this is here so that I can see all dirty patches by listing references to this method.
        public static void AssertNotNull(object o, string whatisit)
        {
            if (o == null)
            {
                throw new NullReferenceException(whatisit + " is null");
            }
        }

        #endregion

    }
}