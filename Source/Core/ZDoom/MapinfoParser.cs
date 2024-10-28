#region ================== Namespaces

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.GZBuilder.Data;

#endregion

namespace CodeImp.DoomBuilder.ZDoom 
{
	internal sealed class MapinfoParser : ZDTextParser
	{
		#region ================== Delegates

		public delegate void IncludeDelegate(MapinfoParser parser, string includefile, bool clearerror);
		public IncludeDelegate OnInclude;

		#endregion

		#region ================== Variables

		private MapInfo mapinfo;
		private string mapname;
		private readonly Dictionary<int, string> spawnnums;
		private readonly Dictionary<int, string> doomednums; // <DoomEdNum, <lowercase classname, List of default arguments>>
		private readonly HashSet<string> parsedlumps;

		#endregion

		#region ================== Properties

		internal override ScriptType ScriptType { get { return ScriptType.MAPINFO; } }
		
		public MapInfo MapInfo { get { return mapinfo; } }
		public Dictionary<int, string> SpawnNums { get { return spawnnums; } }
		public Dictionary<int, string> DoomEdNums { get { return doomednums; } }

		#endregion

		#region ================== Constructor

		public MapinfoParser()
		{
			// Syntax
			whitespace = "\n \t\r\u00A0";
			specialtokens = ",{}=\n";
			
			mapinfo = new MapInfo();
			spawnnums = new Dictionary<int, string>();
			doomednums = new Dictionary<int, string>();
			parsedlumps = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
		}

		#endregion

		#region ================== Parsing


		override public bool Parse(TextResourceData data, bool clearerrors)
		{
			if(string.IsNullOrEmpty(mapname)) throw new NotSupportedException("Map name required!");
			return Parse(data, mapname, clearerrors);
		}

		public bool Parse(TextResourceData data, string mapname, bool clearerrors) 
		{
			this.mapname = mapname.ToLowerInvariant();

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

			// Classic format skip stoppers...
			HashSet<string> breakat = new HashSet<string> { "map", "defaultmap", "adddefaultmap" };

			while(SkipWhitespace(true)) 
			{
				string token = ReadToken().ToLowerInvariant();
				if(string.IsNullOrEmpty(token) || token == "$gzdb_skip") break;

				switch(token)
				{
					case "adddefaultmap":
						// Parse properties
						if(!ParseMapBlock()) return false;
						break;

					case "defaultmap":
						// Reset MapInfo
						mapinfo = new MapInfo();

						// Parse properties
						if(!ParseMapBlock()) return false;
						break;

					case "map":
						// Get map lump name
						SkipWhitespace(true);
						token = ReadToken().ToLowerInvariant();
                        // [ZZ] Note: map name is sometimes in quotes. I think this was "fixed" some time ago, but apparently not.
                        token = StripQuotes(token);
                        if (token != this.mapname) 
						{
                            // Map number? Try to build map name from it...
                            int n;
							if(int.TryParse(token, NumberStyles.Integer, CultureInfo.InvariantCulture, out n))
							{
								token = ((n > 0 && n < 10) ? "map0" + n : "map" + n);
							}

							// Still no dice?
							if(token != this.mapname)
							{
								SkipStructure(breakat);
								continue; // Not finished, search for next "map", "defaultmap" or "adddefaultmap" block
							}
						}

						// Try to get map name
						SkipWhitespace(true);
						token = ReadToken();
						if(token.ToLowerInvariant() == "lookup")
						{
							// No dice...
							SkipWhitespace(true);
							ReadToken();
						}
						else
						{
							mapinfo.Title = StripQuotes(token);
						}

						// Parse properties
						if(!ParseMapBlock()) return false;
						
						// There is a map entry for current map, which makes it defined 
						mapinfo.IsDefined = true; 
						break;

					case "include":
						if(!ParseInclude(clearerrors)) return false;

						// Set our buffers back to continue parsing
						datastream = localstream;
						datareader = localreader;
						sourcename = localsourcename;
						sourcelumpindex = localsourcelumpindex;
						datalocation = locallocation;
						textresourcepath = localtextresourcepath;
						break;

					case "gameinfo":
						if(!ParseGameInfo()) return false;
						break;

					case "doomednums":
						if(!ParseDoomEdNums()) return false;
						break;

					case "spawnnums":
						if(!ParseSpawnNums()) return false;
						break;
				}
			}

			// Check values
			if(mapinfo.FogDensity > 0 && (mapinfo.FadeColor.Red > 0 || mapinfo.FadeColor.Green > 0 || mapinfo.FadeColor.Blue > 0))
				mapinfo.HasFadeColor = true;

			if(mapinfo.OutsideFogDensity > 0 && (mapinfo.OutsideFogColor.Red > 0 || mapinfo.OutsideFogColor.Green > 0 || mapinfo.OutsideFogColor.Blue > 0))
				mapinfo.HasOutsideFogColor = true;

			if(string.IsNullOrEmpty(mapinfo.Sky2) && mapinfo.DoubleSky)
			{
				LogWarning("\"doublesky\" flag is defined without \"Sky2\" property.");
				mapinfo.DoubleSky = false;
			}

			// All done
			return !this.HasError;
		}

		private bool ParseInclude(bool clearerrors)
		{
			SkipWhitespace(true);
			string includelump = StripQuotes(ReadToken(false)); // Don't skip newline

			//INFO: ZDoom MAPINFO include paths can't be relative ("../mapstuff.txt") 
			//or absolute ("d:/project/mapstuff.txt") 
			//or have backward slashes ("info\mapstuff.txt")
			//include paths are relative to the first parsed entry, not the current one 
			//also include paths may or may not be quoted
			if(!string.IsNullOrEmpty(includelump))
			{
				// Absolute paths are not supported...
				if(Path.IsPathRooted(includelump))
				{
					ReportError("Absolute include paths are not supported by ZDoom");
					return false;
				}

				// Relative paths are not supported
				if(includelump.StartsWith(RELATIVE_PATH_MARKER) || includelump.StartsWith(CURRENT_FOLDER_PATH_MARKER) ||
				   includelump.StartsWith(ALT_RELATIVE_PATH_MARKER) || includelump.StartsWith(ALT_CURRENT_FOLDER_PATH_MARKER))
				{
					ReportError("Relative include paths are not supported by ZDoom");
					return false;
				}

				// Backward slashes are not supported
				if(includelump.Contains("\\"))
				{
					ReportError("Only forward slashes are supported by ZDoom");
					return false;
				}

				// Check invalid path chars
				if(!CheckInvalidPathChars(includelump)) return false;

				// Already parsed?
				if(parsedlumps.Contains(includelump))
				{
					ReportError("Already parsed \"" + includelump + "\". Check your include directives");
					return false;
				}

				// Add to collection
				parsedlumps.Add(includelump);

				// Callback to parse this file
				if(OnInclude != null)
				{
					OnInclude(this, includelump, clearerrors);

					// Bail out on error
					if(this.HasError) return false;
				}
			}
			else
			{
				ReportError("Expected filename to include");
				return false;
			}

			// All done here
			return true;
		}

		private bool ParseGameInfo()
		{
			if(!NextTokenIs("{")) return false; // Finished with this file

			while(SkipWhitespace(true))
			{
				string token = ReadToken();
				if(string.IsNullOrEmpty(token))
				{
					ReportError("Failed to find the end of GameInfo block");
					return false; // Finished with this file
				}

				if(token == "}") break;

				if(token.ToLowerInvariant() == "skyflatname")
				{
					if(!NextTokenIs("=")) return false; // Finished with this file
					SkipWhitespace(true);
					string skyflatname = StripQuotes(ReadToken());
					if(string.IsNullOrEmpty(skyflatname))
					{
						ReportError("Expected SkyFlatName value");
						return false; // Finished with this file
					}

					General.Map.Config.SkyFlatName = skyflatname.ToUpperInvariant();
				}
			}

			// All done here
			return true;
		}

		private bool ParseDoomEdNums()
		{
			if(!NextTokenIs("{")) return false; // Finished with this file

			while(SkipWhitespace(true))
			{
				string token = ReadToken();
				if(string.IsNullOrEmpty(token))
				{
					ReportError("Failed to find the end of DoomEdNums block");
					return false; // Finished with this file
				}

				if(token == "}") break;

				// First must be a number
				int id;
				if(!int.TryParse(token, NumberStyles.Integer, CultureInfo.InvariantCulture, out id))
				{
					// Not numeric!
					ReportError("Expected DoomEdNums entry number, but got \"" + token + "\"");
					return false; // Finished with this file
				}

				// Then "="
				if(!NextTokenIs("=")) return false; // Finished with this file

				// Then actor class
				SkipWhitespace(false);
				string classname = StripQuotes(ReadToken());
				if(string.IsNullOrEmpty(classname))
				{
					ReportError("Expected DoomEdNums class definition");
					return false; // Finished with this file
				}

				// Possible special and args. We'll skip them
				for(int i = 0; i < 6; i++)
				{
					if(!NextTokenIs(",", false)) break;

					// Read special name or arg value
					if(!SkipWhitespace(true) || string.IsNullOrEmpty(ReadToken())) return false;
				}

				// Add to collection?
				if(id != 0) doomednums[id] = classname.ToLowerInvariant();
			}

			// All done here
			return true;
		}

		private bool ParseSpawnNums()
		{
			if(!NextTokenIs("{")) return false; // Finished with this file

			while(SkipWhitespace(true))
			{
				string token = ReadToken();
				if(string.IsNullOrEmpty(token))
				{
					ReportError("Failed to find the end of SpawnNums block");
					return false; // Finished with this file
				}

				if(token == "}") break;

				// First must be a number
				int id;
				if(!int.TryParse(token, NumberStyles.Integer, CultureInfo.InvariantCulture, out id))
				{
					// Not numeric!
					ReportError("Expected SpawnNums number, but got \"" + token + "\"");
					return false; // Finished with this file
				}

				// Then "="
				if(!NextTokenIs("=")) return false; // Finished with this file

				// Then actor class
				SkipWhitespace(false);
				token = StripQuotes(ReadToken());
				if(string.IsNullOrEmpty(token))
				{
					ReportError("Unable to get SpawnNums entry class definition");
					return false;
				}

				// Range check
				if((id < General.Map.FormatInterface.MinThingType) || (id > General.Map.FormatInterface.MaxThingType))
				{
					// Out of bounds!
					ReportError("\"" + token + "\" actor's spawn number must be between " 
						+ General.Map.FormatInterface.MinThingType + " and " + General.Map.FormatInterface.MaxThingType);
					return false; // Finished with this file
				}

				// Add to collection
				spawnnums[id] = token.ToLowerInvariant();
			}

			// All done here
			return true;
		}

		#endregion

		#region ================== Map block parsing

		private bool ParseMapBlock()
		{
			bool classicformat = !NextTokenIs("{", false);
			
			// Track brace level
			int bracelevel = (classicformat ? 0 : 1); // Track map block opening brace

			// Parse required values
			while(SkipWhitespace(true))
			{
				string token = ReadToken().ToLowerInvariant();
				switch(token)
				{
					//TODO: are there any other blocks available in the classic format?..
					case "map": case "defaultmap": case "adddefaultmap":
						if(classicformat)
						{
							// We parsed too greadily, step back
							DataStream.Seek(-token.Length - 1, SeekOrigin.Current);

							// Finished with this block
							return true;
						}
						else
						{
							ReportError("Unexpected token \"" + token + "\"");
							return false;
						}
					
					case "sky2": case "sky1":
						if(!ParseSky(token)) return false;
						break;

					case "skybox":
						if(!ParseSkybox()) return false;
						break;

					case "outsidefog": case "fade":
						if(!ParseFade(token)) return false;
						break;

					case "horizwallshade": case "vertwallshade":
						if(!ParseWallShade(token)) return false;
						break;

					case "outsidefogdensity": case "fogdensity":
						if(!ParseFogDensity(token)) return false;
						break;

					case "doublesky":
						mapinfo.DoubleSky = true;
						break;

					case "evenlighting":
						mapinfo.EvenLighting = true;
						break;

					case "smoothlighting":
						mapinfo.SmoothLighting = true;
						break;

					case "lightmode":
						if(!ParseLightMode()) return false;
						break;

					case "lightattenuationmode":
						if(!ParseLightAttenuationMode()) return false;
						break;

                    // [ZZ]
                    case "pixelratio":
                        if (!ParsePixelRatio()) return false;
                        break;

					// biwa
					case "forceworldpanning":
						mapinfo.ForceWorldPanning = true;
						break;

					case "}": return true; // Block end

					case "{": // Skip inner blocks
						bracelevel++;
						if(bracelevel > 1)
						{
							do
							{
								SkipWhitespace(true);
								token = ReadToken();
								if(token == "{") bracelevel++;
								else if(token == "}") bracelevel--;
							} while(!string.IsNullOrEmpty(token) && bracelevel > 1);
						}
						break;
				}
			}

			// All done here
			return true;
		}

		private bool ParseSky(string skytype)
		{
			SkipWhitespace(true);
			string token = StripQuotes(ReadToken());

			// New format
			if(token == "=")
			{
				SkipWhitespace(true);

				// Should be sky texture name
				token = StripQuotes(ReadToken());
				bool gotcomma = (token.IndexOf(",", StringComparison.Ordinal) != -1);
				if(gotcomma) token = token.Replace(",", "");
				string skytexture = token.ToUpperInvariant();

				if(string.IsNullOrEmpty(skytexture))
				{
					ReportError("Expected " + skytype + " texture name");
					return false;
				}

				if(skytype == "sky1")
					mapinfo.Sky1 = skytexture;
				else
					mapinfo.Sky2 = skytexture;

				// Check if we have scrollspeed
				SkipWhitespace(true);
				token = ReadToken();

				if(!gotcomma && token == ",")
				{
					gotcomma = true;
					SkipWhitespace(true);
					token = ReadToken();
				}

				if(gotcomma)
				{
					float scrollspeed = 0;
					if(!ReadSignedFloat(token, ref scrollspeed))
					{
						// Not numeric!
						ReportError("Expected " + skytype + " scroll speed value, but got \"" + token + "\"");
						return false;
					}

					if(skytype == "sky1")
						mapinfo.Sky1ScrollSpeed = scrollspeed;
					else
						mapinfo.Sky2ScrollSpeed = scrollspeed;
				}
				else
				{
					datastream.Seek(-token.Length - 1, SeekOrigin.Current); // Step back and try parsing this token again
				}
			}
			// Old format
			else
			{
				// Token should be sky1/2 name
				if(string.IsNullOrEmpty(token))
				{
					ReportError("Expected " + skytype + " texture name");
					return false;
				}

				if(skytype == "sky1")
					mapinfo.Sky1 = token.ToUpperInvariant();
				else
					mapinfo.Sky2 = token.ToUpperInvariant();

				// Try to read scroll speed
				SkipWhitespace(true);
				token = ReadToken();

				float scrollspeed = 0;
				if(!ReadSignedFloat(token, ref scrollspeed))
				{
					// Not numeric!
					datastream.Seek(-token.Length - 1, SeekOrigin.Current); // Step back and try parsing this token again
					return true;
				}

				if(skytype == "sky1")
					mapinfo.Sky1ScrollSpeed = scrollspeed;
				else
					mapinfo.Sky2ScrollSpeed = scrollspeed;
			}

			// All done here
			return true;
		}

		// Skybox = "<texture>"
		private bool ParseSkybox()
		{
			// New format only!
			if(!NextTokenIs("=")) return false;

			SkipWhitespace(true);
			string token = StripQuotes(ReadToken());

			if(string.IsNullOrEmpty(token))
			{
				ReportError("Expected Skybox texture name");
				return false;
			}

			mapinfo.Sky1 = token.ToUpperInvariant();
			mapinfo.Sky1ScrollSpeed = 0;

			// All done here
			return true;
		}

		private bool ParseFade(string fadetype)
		{
			SkipWhitespace(true);
			string token = ReadToken();

			// New format?
			if(token == "=")
			{
				SkipWhitespace(true);
				token = ReadToken();
			}

			// Get the color value
			string colorval = StripQuotes(token).ToLowerInvariant().Replace(" ", "");

			if(string.IsNullOrEmpty(colorval))
			{
				ReportError("Expected " + fadetype + " color value");
				return false;
			}
			
			// Try to get the color...
			PixelColor color = new PixelColor();
			if(GetColorFromString(colorval, out color))
			{
				if(fadetype == "fade")
					mapinfo.FadeColor = color.ToColorValue();
				else
					mapinfo.OutsideFogColor = color.ToColorValue();
			}
			else //...or not
			{
				ReportError("Failed to parse " + fadetype + " value from string \"" + colorval + "\"");
				return false;
			}

			// All done here
			return true;
		}

		private bool ParseWallShade(string shadetype)
		{
			SkipWhitespace(true);
			string token = ReadToken();

			// New format
			if(token == "=")
			{
				SkipWhitespace(true);
				token = ReadToken();
			}

			int val = 0;
			if(!ReadSignedInt(token, ref val))
			{
				// Not numeric!
				ReportError("Expected " + shadetype + " value, but got \"" + token + "\"");
				return false;
			}

			if(shadetype == "vertwallshade")
				mapinfo.VertWallShade = General.Clamp(val, -255, 255);
			else
				mapinfo.HorizWallShade = General.Clamp(val, -255, 255);

			// All done here
			return true;
		}

		private bool ParseFogDensity(string densitytype)
		{
			SkipWhitespace(true);
			if(!NextTokenIs("=")) return false; // New format only
			SkipWhitespace(true);
			string token = StripQuotes(ReadToken());

            int val;
			if(!int.TryParse(token, NumberStyles.Integer, CultureInfo.InvariantCulture, out val))
			{
				// Not numeric!
				ReportError("Expected " + densitytype + " value, but got \"" + token + "\"");
				return false;
			}

			if(densitytype == "fogdensity") mapinfo.FogDensity = val;
			else mapinfo.OutsideFogDensity = val;

			// All done here
			return true;
		}

		private bool ParseLightMode()
		{
			SkipWhitespace(true);
			if(!NextTokenIs("=")) return false; // New format only
			SkipWhitespace(true);
			string token = ReadToken();

			int val;
			if(!int.TryParse(token, NumberStyles.Integer, CultureInfo.InvariantCulture, out val))
			{
				// Not numeric!
				ReportError("Expected LightMode value, but got \"" + token + "\"");
				return false;
			}

			// Store
			mapinfo.LightMode = (MapInfo.GZDoomLightMode)val;

			// All done here
			return true;
		} 
		private bool ParseLightAttenuationMode()
		{
			SkipWhitespace(true);
			if(!NextTokenIs("=")) return false; // New format only
			SkipWhitespace(true);
			string token = StripQuotes(ReadToken());

			// Store
			mapinfo.LightAttenuationMode = token;

			// All done here
			return true;
		} 

        private bool ParsePixelRatio()
        {
            SkipWhitespace(true);
            if (!NextTokenIs("=")) return false; // New format only
            SkipWhitespace(true);
            string token = ReadToken();

            float val;
            if (!float.TryParse(token, NumberStyles.Float, CultureInfo.InvariantCulture, out val))
            {
                // Not numeric!
                ReportError("Expected PixelRatio value, but got \"" + token + "\"");
                return false;
            }

            // Store
            mapinfo.PixelRatio = val;

            // All done here
            return true;

        }

		#endregion

	}
}
