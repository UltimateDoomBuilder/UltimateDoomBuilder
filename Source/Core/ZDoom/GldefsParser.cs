#region ================== Namespaces

using System;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.GZBuilder.Data;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.IO;
using CodeImp.DoomBuilder.GZBuilder;

#endregion

namespace CodeImp.DoomBuilder.ZDoom
{
	internal sealed class GldefsParser : ZDTextParser
	{
		#region ================== Constants

		private const int DEFAULT_GLOW_HEIGHT = 64;

		#endregion

		#region ================== Structs

		private struct GldefsLightType
		{
			public const string POINT = "pointlight";
			public const string PULSE = "pulselight";
			public const string FLICKER = "flickerlight";
			public const string FLICKER2 = "flickerlight2";
			public const string SECTOR = "sectorlight";

			public static readonly Dictionary<string, GZGeneral.LightModifier> GLDEFS_TO_GZDOOM_LIGHT_TYPE = new Dictionary<string, GZGeneral.LightModifier>(StringComparer.OrdinalIgnoreCase) {
                { POINT, GZGeneral.LightModifier.NORMAL },
                { PULSE, GZGeneral.LightModifier.PULSE },
                { FLICKER, GZGeneral.LightModifier.FLICKER },
                { FLICKER2, GZGeneral.LightModifier.FLICKERRANDOM },
                { SECTOR, GZGeneral.LightModifier.SECTOR }
            };
		}

		#endregion

		#region ================== Delegates

		public delegate void IncludeDelegate(GldefsParser parser, string includefile, bool clearerrors);
		public IncludeDelegate OnInclude;

		#endregion

		#region ================== Variables

		private readonly Dictionary<string, DynamicLightData> lightsbyname; //LightName, light definition
		private readonly Dictionary<string, string> objects; //ClassName, LightName
		private readonly Dictionary<long, GlowingFlatData> glowingflats;
		private readonly Dictionary<string, SkyboxInfo> skyboxes;
		private readonly HashSet<string> parsedlumps;

		#endregion

		#region ================== Properties

		internal override ScriptType ScriptType { get { return ScriptType.GLDEFS; } }
		
		internal Dictionary<string, DynamicLightData> LightsByName { get { return lightsbyname; } }
		internal Dictionary<string, string> Objects { get { return objects; } }
		internal Dictionary<long, GlowingFlatData> GlowingFlats { get { return glowingflats; } }
		internal Dictionary<string, SkyboxInfo> Skyboxes { get { return skyboxes; } }

		#endregion

		#region ================== Constructor

		public GldefsParser() 
		{
			// Syntax
			whitespace = "\n \t\r\u00A0";
			specialtokens = ",{}\n";
			
			parsedlumps = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			lightsbyname = new Dictionary<string, DynamicLightData>(StringComparer.OrdinalIgnoreCase); //LightName, Light params
			objects = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase); //ClassName, LightName
			glowingflats = new Dictionary<long, GlowingFlatData>(); // Texture name hash, Glowing Flat Data
			skyboxes = new Dictionary<string, SkyboxInfo>(StringComparer.OrdinalIgnoreCase);
		}

		#endregion

		#region ================== Parsing

		public override bool Parse(TextResourceData data, bool clearerrors) 
		{
			//mxd. Already parsed?
			if(!base.AddTextResource(data))
			{
				if(clearerrors) ClearError();
				return true;
			}

			// Cannot process?
			if(!base.Parse(data, clearerrors)) return false;

			// Keep local data
			Stream localstream = datastream;
			string localsourcename = sourcename;
			int localsourcelumpindex = sourcelumpindex;
			BinaryReader localreader = datareader;
			DataLocation locallocation = datalocation;
			string localtextresourcepath = textresourcepath;

			// Continue until at the end of the stream
			while(SkipWhitespace(true)) 
			{
				string token = ReadToken().ToLowerInvariant();
				if(string.IsNullOrEmpty(token)) continue;

				switch(token)
				{
					case GldefsLightType.POINT: case GldefsLightType.PULSE: case GldefsLightType.SECTOR:
					case GldefsLightType.FLICKER: case GldefsLightType.FLICKER2: 
						if(!ParseLight(token)) return false;
						break;

					case "object":
						if(!ParseObject()) return false;
						break;

					case "glow":
						if(!ParseGlowingFlats()) return false;
						break;

					case "skybox":
						if(!ParseSkybox()) return false;
						break;

					case "#include":
						if(!ParseInclude(clearerrors)) return false;

						// Set our buffers back to continue parsing
						datastream = localstream;
						datareader = localreader;
						sourcename = localsourcename;
						sourcelumpindex = localsourcelumpindex;
						datalocation = locallocation;
						textresourcepath = localtextresourcepath;
						break;

					case "$gzdb_skip": return !this.HasError;
					
					default:
						// Unknown structure!
						SkipStructure();
						break;
				}
			}

			// All done
			return !this.HasError;
		}

		private bool ParseLight(string lighttype)
		{
            DynamicLightData light = new DynamicLightData(new GZGeneral.LightData(GldefsLightType.GLDEFS_TO_GZDOOM_LIGHT_TYPE[lighttype], GZGeneral.LightRenderStyle.NORMAL));

			// Find classname
			SkipWhitespace(true);
			string lightname = StripQuotes(ReadToken());

			if(string.IsNullOrEmpty(lightname))
			{
				ReportError("Expected " + lighttype + " name");
				return false;
			}

			// Now find opening brace
			if(!NextTokenIs("{", false))
			{
				ReportError("Expected opening brace");
				return false;
			}

			// Read gldefs light structure
			while(SkipWhitespace(true))
			{
				string token = ReadToken().ToLowerInvariant();
				if(string.IsNullOrEmpty(token)) continue;

				switch(token)
				{
					case "color":
						SkipWhitespace(true);
						token = ReadToken();
						if(!float.TryParse(token, NumberStyles.Float, CultureInfo.InvariantCulture, out light.Color.Red))
						{
							// Not numeric!
							ReportError("Expected Red color value, but got \"" + token + "\"");
							return false;
						}

						SkipWhitespace(true);
						token = ReadToken();
						if(!float.TryParse(token, NumberStyles.Float, CultureInfo.InvariantCulture, out light.Color.Green))
						{
							// Not numeric!
							ReportError("Expected Green color value, but got \"" + token + "\"");
							return false;
						}

						SkipWhitespace(true);
						token = ReadToken();
						if(!float.TryParse(token, NumberStyles.Float, CultureInfo.InvariantCulture, out light.Color.Blue))
						{
							// Not numeric!
							ReportError("Expected Blue color value, but got \"" + token + "\"");
							return false;
						}

						// Check the value
						if(light.Color.Red == 0.0f && light.Color.Green == 0.0f && light.Color.Blue == 0.0f)
						{
							LogWarning("\"" + lightname + "\" light Color value is " 
								+ light.Color.Red + "," + light.Color.Green + "," + light.Color.Blue 
								+ ". It won't be shown in GZDoom");
						}
						else if(light.Color.Red > 1.0f || light.Color.Green > 1.0f || light.Color.Blue > 1.0f ||
								light.Color.Red < 0.0f || light.Color.Green < 0.0f || light.Color.Blue < 0.0f)
						{
							// Clamp values
							light.Color.Red = General.Clamp(light.Color.Red, 0.0f, 1.0f);
							light.Color.Green = General.Clamp(light.Color.Green, 0.0f, 1.0f);
							light.Color.Blue = General.Clamp(light.Color.Blue, 0.0f, 1.0f);

							// Notify user
							LogWarning("\"" + lightname + "\" light Color value was clamped. Color values must be in [0.0 .. 1.0] range");
						}
					break;

					case "size":
						if(lighttype != GldefsLightType.SECTOR)
						{
							SkipWhitespace(true);

							token = ReadToken();
							if(!int.TryParse(token, NumberStyles.Integer, CultureInfo.InvariantCulture, out light.PrimaryRadius))
							{
								// Not numeric!
								ReportError("Expected Size value, but got \"" + token + "\"");
								return false;
							}

							if(light.PrimaryRadius < 0)
							{
								ReportError("Size value should be positive, but is \"" + light.PrimaryRadius + "\"");
								return false;
							}

							light.PrimaryRadius *= 2;

						}
						else
						{
							ReportError("\"" + token + "\" is not valid property for " + lighttype);
							return false;
						}
					break;

					case "offset":
						SkipWhitespace(true);
						token = ReadToken();
						if(!ReadSignedFloat(token, ref light.Offset.X))
						{
							// Not numeric!
							ReportError("Expected Offset X value, but got \"" + token + "\"");
							return false;
						}

						SkipWhitespace(true);
						token = ReadToken();
						if(!ReadSignedFloat(token, ref light.Offset.Z))
						{
							// Not numeric!
							ReportError("Expected Offset Y value, but got \"" + token + "\"");
							return false;
						}

						SkipWhitespace(true);
						token = ReadToken();
						if(!ReadSignedFloat(token, ref light.Offset.Y))
						{
							// Not numeric!
							ReportError("Expected Offset Z value, but got \"" + token + "\"");
							return false;
						}
					    break;

					case "subtractive":
					{
						SkipWhitespace(true);

						token = ReadToken();
						int i;
						if(!int.TryParse(token, NumberStyles.Integer, CultureInfo.InvariantCulture, out i))
						{
							// Not numeric!
							ReportError("expected Subtractive value, but got \"" + token + "\"");
							return false;
						}

                        light.Type.SetRenderStyle((i == 1) ? GZGeneral.LightRenderStyle.SUBTRACTIVE : GZGeneral.LightRenderStyle.NORMAL);
                        break;
                    }

                    case "attenuate":
                    {
                        SkipWhitespace(true);

                        token = ReadToken();
                        int i;
                        if (!int.TryParse(token, NumberStyles.Integer, CultureInfo.InvariantCulture, out i))
                        {
                            // Not numeric!
                            ReportError("expected Attenuate value, but got \"" + token + "\"");
                            return false;
                        }

                        light.Type.SetRenderStyle((i == 1) ? GZGeneral.LightRenderStyle.ATTENUATED : GZGeneral.LightRenderStyle.NORMAL);
                        break;
                    }

                    case "dontlightself":
					{
						SkipWhitespace(true);

						token = ReadToken();
						int i;
						if(!int.TryParse(token, NumberStyles.Integer, CultureInfo.InvariantCulture, out i))
						{
							// Not numeric!
							ReportError("Expected DontLightSelf value, but got \"" + token + "\"");
							return false;
						}

						light.DontLightSelf = (i == 1);
					}
					break;

					case "interval":
						if(lighttype == GldefsLightType.PULSE || lighttype == GldefsLightType.FLICKER2)
						{
							SkipWhitespace(true);

							token = ReadToken();
							float interval = 0f;
							if(!ReadSignedFloat(token, ref interval))
							{
								// Not numeric!
								ReportError("Expected Interval value, but got \"" + token + "\"");
								return false;
							}

							if(interval <= 0)
							{
								ReportError("Interval value should be greater than zero, but is \"" + interval + "\"");
								return false;
							}

							//I wrote logic for dynamic lights animation first, so here I modify gldefs settings to fit in existing logic
							if(lighttype == GldefsLightType.PULSE)
							{
								light.Interval = (int)(interval * 35); //measured in tics (35 per second) in PointLightPulse, measured in seconds in gldefs' PulseLight
							}
							else //FLICKER2. Seems like PointLightFlickerRandom to me
							{
								light.Interval = (int)(interval * 350); //0.1 is one second for FlickerLight2
							}
						}
						else
						{
							ReportError("\"" + token + "\" is not valid property for " + lighttype);
							return false;
						}
					break;

					case "secondarysize":
						if(lighttype == GldefsLightType.PULSE || lighttype == GldefsLightType.FLICKER || lighttype == GldefsLightType.FLICKER2)
						{
							SkipWhitespace(true);

							token = ReadToken();
							if(!ReadSignedInt(token, ref light.SecondaryRadius))
							{
								// Not numeric!
								ReportError("Expected SecondarySize value, but got \"" + token + "\"");
								return false;
							}

							if(light.SecondaryRadius < 0)
							{
								ReportError("SecondarySize value should be positive, but is \"" + light.SecondaryRadius + "\"");
								return false;
							}

							light.SecondaryRadius *= 2;
						}
						else
						{
							ReportError("\"" + token + "\" is not valid property for " + lighttype);
							return false;
						}
					break;

					case "chance":
						if(lighttype == GldefsLightType.FLICKER)
						{
							SkipWhitespace(true);

							token = ReadToken();
							float chance = 0f;
							if(!ReadSignedFloat(token, ref chance))
							{
								// Not numeric!
								ReportError("Expected Chance value, but got \"" + token + "\"");
								return false;
							}

							// Check range
							if(chance > 1.0f || chance < 0.0f)
							{
								ReportError("Chance must be in [0.0 .. 1.0] range, but is " + chance);
								return false;
							}

							// Transforming from 0.0 .. 1.0 to 0 .. 359 to fit in existing logic
							light.Interval = (int)(chance * 359.0f);
						}
						else
						{
							ReportError("\"" + token + "\" is not valid property for " + lighttype);
							return false;
						}
					break;

					case "scale":
						if(lighttype == GldefsLightType.SECTOR)
						{
							SkipWhitespace(true);

							token = ReadToken();
							float scale = 0f;
							if(!ReadSignedFloat(token, ref scale))
							{
								// Not numeric!
								ReportError("Expected Scale value, but got \"" + token + "\"");
								return false;
							}

							// Check range
							if(scale > 1.0f || scale < 0.0f)
							{
								ReportError("Scale must be in [0.0 .. 1.0] range, but is " + scale);
								return false;
							}

							//sector light doesn't have animation, so we will store it's size in Interval
							//transforming from 0.0 .. 1.0 to 0 .. 10 to preserve value.
							light.Interval = (int)(scale * 10.0f);
						}
						else
						{
							ReportError("\"" + token + "\" is not valid property for " + lighttype);
							return false;
						}
					break;

					case "}":
					{
						bool skip = (light.Color.Red == 0.0f && light.Color.Green == 0.0f && light.Color.Blue == 0.0f);

						// Light-type specific checks
						if(light.Type.LightModifier == GZGeneral.LightModifier.NORMAL && light.PrimaryRadius == 0)
						{
							LogWarning("\"" + lightname + "\" light Size is 0. It won't be shown in GZDoom");
							skip = true;
						}

						if(light.Type.LightAnimated)
						{
							if(light.PrimaryRadius == 0 && light.SecondaryRadius == 0)
							{
								LogWarning("\"" + lightname + "\" light Size and SecondarySize are 0. It won't be shown in GZDoom");
								skip = true;
							}
						}

						// Add to the collection?
						if(!skip) lightsbyname[lightname] = light;

						// Break out of this parsing loop
						return true;
					}
				}
			}

			// All done here
			return true;
		}

		private bool ParseObject()
		{
			SkipWhitespace(true);

			// Read object class
			string objectclass = StripQuotes(ReadToken());

			if(string.IsNullOrEmpty(objectclass))
			{
				ReportError("Expected object class");
				return false;
			}

			// Check if actor exists
			if(General.Map.Data.GetZDoomActor(objectclass) == null)
				LogWarning("DECORATE class \"" + objectclass + "\" does not exist");

			// Now find opening brace
			if(!NextTokenIs("{", false))
			{
				ReportError("Expected opening brace");
				return false;
			}

			int bracescount = 1;
			bool foundlight = false;
			bool foundframe = false;

			// Read frames structure
			while(SkipWhitespace(true))
			{
				string token = ReadToken().ToLowerInvariant();
				if(string.IsNullOrEmpty(token)) continue;

				if(!foundlight && !foundframe && token == "frame")
				{
					SkipWhitespace(true);
					token = ReadToken().ToLowerInvariant(); // Should be frame name

					// Use this frame if it's 4 characters long or it's the first frame
					foundframe = (token.Length == 4 || (token.Length > 4 && token[4] == 'a'));
				}
				else if(!foundlight && foundframe && token == "light") // Just use first light and be done with it
				{
					SkipWhitespace(true);
					token = StripQuotes(ReadToken()); // Should be light name

					if(!string.IsNullOrEmpty(token))
					{
						objects[objectclass] = token;
						foundlight = true;
					}
				}
				else if(token == "{") // Continue in this loop until object structure ends
				{
					bracescount++;
				}
				else if(token == "}")
				{
					if(--bracescount < 1) break; // This was Cave Johnson. And we are done here.
				}
			}

			// All done here
			return true;
		}

		private bool ParseGlowingFlats()
		{
			// Next sould be opening brace
			if(!NextTokenIs("{", false))
			{
				ReportError("Expected opening brace");
				return false;
			}

			// Parse inner blocks
			while(SkipWhitespace(true))
			{
				string token = ReadToken().ToLowerInvariant();
				if(token == "}") break; // End of Glow structure
				
				switch(token)
				{
					case "walls":
					case "flats":
						if(!NextTokenIs("{", false))
						{
							ReportError("Expected opening brace");
							return false;
						}

						while(SkipWhitespace(true))
						{
							token = StripQuotes(ReadToken(false));
							if(string.IsNullOrEmpty(token)) continue;
							if(token == "}") break;

							// Add glow data. Hash the name exactly as given. 
							long flatnamehash = Lump.MakeLongName(token, true);
							glowingflats[flatnamehash] = new GlowingFlatData
														 {
                                                             Height = DEFAULT_GLOW_HEIGHT * 2,
                                                             Fullbright = true,
                                                             Color = new PixelColor(255, 255, 255, 255),
                                                             CalculateTextureColor = true
                                                         };
						}
					break;

					case "texture":
					{
						PixelColor color = new PixelColor();
						int glowheight = DEFAULT_GLOW_HEIGHT;
						string texturename;

						if(!ReadTextureName(out texturename)) return false;
						if(string.IsNullOrEmpty(texturename))
						{
							ReportError("Expected " + token + " name");
							return false;
						}

						// Now we should find a comma
						if(!NextTokenIs(",", false))
						{
							ReportError("Expected a comma");
							return false;
						}

						// Next is color
						SkipWhitespace(true);
						token = ReadToken(false);

						if(!GetColorFromString(token, out color))
						{
							ReportError("Expected glow color value, but got \"" + token + "\"");
							return false;
						}

						// The glow data is valid at thispoint. Hash the name exactly as given. 
						long texturehash = Lump.MakeLongName(texturename, true);

						// Now we can find a comma
						if(!NextTokenIs(",", false))
						{
							// Add glow data
							glowingflats[texturehash] = new GlowingFlatData
							                            {
								                            Height = glowheight * 2,
								                            Color = color.WithAlpha(255),
								                            CalculateTextureColor = false
							                            };
							continue;
						}

						// Can be glow height
						SkipWhitespace(true);
						token = ReadToken();

						int h;
						if(int.TryParse(token, NumberStyles.Integer, CultureInfo.InvariantCulture, out h))
						{
							// Can't pass glowheight directly cause TryParse will unconditionally set it to 0
							glowheight = h;

							// Now we can find a comma
							if(!NextTokenIs(",", false))
							{
								// Add glow data
								glowingflats[texturehash] = new GlowingFlatData
								                            {
									                            Height = glowheight * 2,
									                            Color = color.WithAlpha(255),
									                            CalculateTextureColor = false
								                            };
								continue;
							}

							// Read the flag
							SkipWhitespace(true);
							token = ReadToken().ToLowerInvariant();
						}

						// Next must be "fullbright" flag
						if(token != "fullbright")
						{
							ReportError("Expected \"fullbright\" flag, but got \"" + token + "\"");
							return false;
						}

						// Add glow data
						glowingflats[texturehash] = new GlowingFlatData
						                            {
							                            Height = glowheight * 2,
							                            Fullbright = true,
							                            Color = color.WithAlpha(255),
							                            CalculateTextureColor = false
						                            };
					}
					break;
				}
			}

			// All done here
			return true;
		}

		private bool ParseSkybox()
		{
			SkipWhitespace(true);
			string name;

			if(!ReadTextureName(out name)) return false;
			if(string.IsNullOrEmpty(name))
			{
				ReportError("Expected skybox name");
				return false;
			}

			if(skyboxes.ContainsKey(name)) LogWarning("Skybox \"" + name + "\" is double defined");

			SkyboxInfo info = new SkyboxInfo(name.ToUpperInvariant()); 

			// FlipTop / opening brace
			SkipWhitespace(true);
			string token = ReadToken();
			if(token.ToLowerInvariant() == "fliptop")
			{
				info.FlipTop = true;
				if(!NextTokenIs("{")) return false;
			}
			else if(token != "{")
			{
				ReportError("Expected opening brace or \"fliptop\" keyword");
				return false;
			}

			// Read skybox texture names
			while(SkipWhitespace(true))
			{
				token = ReadToken();
				if(token == "}") break;
				info.Textures.Add(token);
			}

			// Sanity check. Should have 3 or 6 textrues
			if(info.Textures.Count != 3 && info.Textures.Count != 6)
			{
				ReportError("Expected 3 or 6 skybox textures");
				return false;
			}

			// Add to collection
			skyboxes[name] = info;

			// All done here
			return true;
		}

		private bool ParseInclude(bool clearerrors)
		{
			//INFO: GZDoom GLDEFS include paths can't be relative ("../glstuff.txt") 
			//or absolute ("d:/project/glstuff.txt") 
			//or have backward slashes ("info\glstuff.txt")
			//include paths are relative to the first parsed entry, not the current one 
			//also include paths may or may not be quoted
			SkipWhitespace(true);
			string includelump = StripQuotes(ReadToken(false)); // Don't skip newline

			// Sanity checks
			if(string.IsNullOrEmpty(includelump))
			{
				ReportError("Expected file name to include");
				return false;
			}

			// Check invalid path chars
			if(!CheckInvalidPathChars(includelump)) return false;

			// Absolute paths are not supported...
			if(Path.IsPathRooted(includelump))
			{
				ReportError("Absolute include paths are not supported by GZDoom");
				return false;
			}

			// Relative paths are not supported
			if(includelump.StartsWith(RELATIVE_PATH_MARKER) || includelump.StartsWith(CURRENT_FOLDER_PATH_MARKER) ||
			   includelump.StartsWith(ALT_RELATIVE_PATH_MARKER) || includelump.StartsWith(ALT_CURRENT_FOLDER_PATH_MARKER))
			{
				ReportError("Relative include paths are not supported by GZDoom");
				return false;
			}

			// Backward slashes are not supported
			if(includelump.Contains("\\"))
			{
				ReportError("Only forward slashes are supported by GZDoom");
				return false;
			}

			// Already parsed?
			if(parsedlumps.Contains(includelump))
			{
				ReportError("Already parsed \"" + includelump + "\". Check your #include directives");
				return false;
			}

			// Add to collection
			parsedlumps.Add(includelump);

			// Callback to parse this file
			if(OnInclude != null) OnInclude(this, includelump, clearerrors);

			// All done here
			return !this.HasError;
		}

		#endregion

		#region ================== Methods

		internal void ClearIncludesList() 
		{
			parsedlumps.Clear();
		}

		#endregion
	}
}