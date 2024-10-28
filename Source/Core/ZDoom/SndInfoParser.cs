#region ================== Namespaces

using System;
using System.Collections.Generic;
using System.IO;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Data;

#endregion

namespace CodeImp.DoomBuilder.ZDoom
{
	internal sealed class SndInfoParser : ZDTextParser
	{
		#region ================== Variables

		private Dictionary<int, AmbientSoundInfo> ambientsounds;
		private Dictionary<string, SoundInfo> sounds;
		private SoundInfo globalprops;
		
		// There are two formats supported, with the old format being
		//	logicalname lumpname
		// and the new format being
		//  logicalname = lumpname
		// Only one format may be used
		private enum SndInfoFormat { None, Old, New }
		private SndInfoFormat format = SndInfoFormat.None;

		#endregion

		#region ================== Properties

		internal override ScriptType ScriptType { get { return ScriptType.SNDINFO; } }

		internal Dictionary<int, AmbientSoundInfo> AmbientSounds { get { return ambientsounds; } }
		internal Dictionary<string, SoundInfo> Sounds { get { return sounds; } }

		#endregion

		#region ================== Constructor

		public SndInfoParser()
		{
			specialtokens = "{}";
			ambientsounds = new Dictionary<int, AmbientSoundInfo>();
			sounds = new Dictionary<string, SoundInfo>(StringComparer.OrdinalIgnoreCase);
			skipeditorcomments = true; // otherwise //$AMBIENT will be treated like one...
			globalprops = new SoundInfo();
		}

		#endregion

		#region ================== Parsing

		public override bool Parse(TextResourceData data, bool clearerrors)
		{
			// We have to reset the format, since it's per file, not globally
			format = SndInfoFormat.None;

			//mxd. Already parsed?
			if (!base.AddTextResource(data))
			{
				if(clearerrors) ClearError();
				return true;
			}

			// Cannot process?
			if(!base.Parse(data, clearerrors)) return false;

			// Continue until at the end of the stream
			string currentgametype = GameType.UNKNOWN;
			while(SkipWhitespace(true))
			{
				//INFO: For many commands, using * as the sound name will mean that 
				//INFO: the command will apply to all sounds that do not specify otherwise. 
				string token = StripTokenQuotes(ReadToken()).ToLowerInvariant();
				if(string.IsNullOrEmpty(token)) continue;

				// Skipping block for different game?
				if(currentgametype != GameType.UNKNOWN && currentgametype != General.Map.Config.BaseGame)
				{
					// Should we stop skipping?
					if(token == "$endif") currentgametype = GameType.UNKNOWN;
					continue;
				}

				switch(token)
				{
					// Must parse all commands to reliably get sound assignments...
					case "$alias": if(!ParseAlias()) return false; break;
					case "$ambient": if(!ParseAmbient()) return false; break;
					case "$archivepath": if(!SkipTokens(1)) return false; break;
					case "$attenuation": if(!ParseAttenuation()) return false; break;
					case "$edfoverride": break;
					case "$limit": if(!ParseLimit()) return false; break;
					case "$map": if(!SkipTokens(2)) return false; break;
					case "$mididevice": if(!ParseMidiDevice()) return false; break;
					case "$musicalias": if(!SkipTokens(2)) return false; break;
					case "$musicvolume": if(!SkipTokens(2)) return false; break;
					case "$pitchshift": if(!SkipTokens(2)) return false; break;
					case "$pitchshiftrange": if(!SkipTokens(1)) return false; break;
					case "$playeralias": if(!SkipTokens(4)) return false; break;
					case "$playercompat": if(!SkipTokens(4)) return false; break;
					case "$playersound": if(!SkipTokens(4)) return false; break;
					case "$playersounddup": if(!SkipTokens(4)) return false; break;
					case "$random": if(!ParseRandom()) return false; break;
					case "$registered": break;
					case "$rolloff": if(!ParseRolloff()) return false; break;
					case "$singular": if(!SkipTokens(1)) return false; break;
					case "$volume": if(!ParseVolume()) return false; break;

					// Game type blocks...
					case "$ifdoom": currentgametype = GameType.DOOM; break;
					case "$ifheretic": currentgametype = GameType.HERETIC; break;
					case "$ifhexen": currentgametype = GameType.HEXEN; break;
					case "$ifstrife": currentgametype = GameType.STRIFE; break;

					// Should be logicalname lumpname pair...
					default: if(!ParseSoundAssignment(token)) return false; break;
				}
			}

			return true;
		}

		// $ambient <index> <logicalsound> [type] <mode> <volume>
		private bool ParseAmbient()
		{
			if(!SkipWhitespace(true)) return false;
			AmbientSoundInfo asi = new AmbientSoundInfo();
			if(!asi.Setup(this)) return false;

			// Skip strange cases...
			if(asi.SoundName.StartsWith("*")) return true;

			// Check for duplicates
			if(ambientsounds.ContainsKey(asi.Index))
				LogWarning("Ambient sound " + asi.Index + " is double-defined as \"" + ambientsounds[asi.Index].SoundName + "\" and \"" + asi.SoundName + "\"");

			// Add to collection
			ambientsounds[asi.Index] = asi;

			return true;
		}

		// $alias aliasname soundname
		private bool ParseAlias()
		{
			// Read aliasname
			if(!SkipWhitespace(true)) return false;
			string aliasname = StripTokenQuotes(ReadToken());
			if(string.IsNullOrEmpty(aliasname)) return false;

			// Read soundname
			if(!SkipWhitespace(true)) return false;
			string soundname = StripTokenQuotes(ReadToken());
			if(string.IsNullOrEmpty(soundname)) return false;

			SoundInfo info = GetSoundInfo(soundname);

			// Check for duplicates
			if(sounds.ContainsKey(aliasname))
				LogWarning("$alias name \"" + aliasname + "\" is double-defined");

			// Add to collection
			sounds[aliasname] = info;

			return true;
		}

		// $attenuation aliasname value
		private bool ParseAttenuation()
		{
			// Read aliasname
			if(!SkipWhitespace(true)) return false;
			string aliasname = StripTokenQuotes(ReadToken());
			if(string.IsNullOrEmpty(aliasname)) return false;
			SoundInfo info = GetSoundInfo(aliasname);

			// Read value
			if(!SkipWhitespace(true)) return false;
			if(!ReadSignedFloat(ref info.Attenuation) || info.Attenuation < 0f)
			{
				ReportError("Expected $attenuation value");
				return false;
			}

			return true;
		}

		// Needed because of optional parameter...
		// $limit soundname <amount> [limitdistance]
		private bool ParseLimit()
		{
			// Read soundname
			if(!SkipWhitespace(true)) return false;
			string soundname = StripTokenQuotes(ReadToken());
			if(string.IsNullOrEmpty(soundname)) return false;

			// Must be <amount>
			if(!SkipWhitespace(false)) return false;
			int amount = 2;
			if(!ReadSignedInt(ref amount))
			{
				ReportError("Expected $limit <amount> value");
				return false;
			}

			// Can be [limitdistance]
			if(!SkipWhitespace(false)) return false;
			int limitdistance = 256;
			string next = ReadToken(false);
			if(!ReadSignedInt(next, ref limitdistance) || limitdistance < 0f)
			{
				// Rewind so this structure can be read again
				DataStream.Seek(-next.Length - 1, SeekOrigin.Current);
			}

			return true;
		}

		// Needed because of optional parameter...
		// $mididevice musicname device [parameter]
		private bool ParseMidiDevice()
		{
			// Read musicname
			if(!SkipWhitespace(true)) return false;
			string musicname = StripTokenQuotes(ReadToken());
			if(string.IsNullOrEmpty(musicname)) return false;

			// Read device
			if(!SkipWhitespace(true)) return false;
			string device = StripTokenQuotes(ReadToken());
			if(string.IsNullOrEmpty(device)) return false;

			// Try to read parameter
			if(!SkipWhitespace(true)) return false;
			string parameter = StripTokenQuotes(ReadToken()).ToLowerInvariant();
			if(string.IsNullOrEmpty(parameter)) return false;

			HashSet<string> validparams = new HashSet<string> { "opl", "fluidsynth", "timidity", "wildmidy" };
			if(!validparams.Contains(parameter))
			{
				// Rewind so this structure can be read again
				DataStream.Seek(-parameter.Length - 1, SeekOrigin.Current);
			}
			
			return true;
		}

		// $rolloff soundname <mindist> <maxdist>
		// $rolloff soundname <type>
		private bool ParseRolloff()
		{
			// Read soundname
			if(!SkipWhitespace(true)) return false;
			string soundname = StripTokenQuotes(ReadToken());
			SoundInfo info = GetSoundInfo(soundname);

			// Next token can be <type>...
			if(!SkipWhitespace(true)) return false;
			string token = ReadToken(false).ToLowerInvariant();

			if(token == "custom" || token == "linear" || token == "log")
			{
				if(token == "linear")
				{
					// Must be <min distance> <max distance> pair
					if(!SkipWhitespace(false)) return false;
					if(!ReadSignedInt(ref info.MinimumDistance) || info.MinimumDistance < 0)
					{
						ReportError("Expected $rolloff linear <mindist> value");
						return false;
					}

					if(!SkipWhitespace(false)) return false;
					if(!ReadSignedInt(ref info.MaximumDistance) || info.MaximumDistance < 0)
					{
						ReportError("Expected $rolloff linear <maxdist> value");
						return false;
					}
				}
				else if(token == "log")
				{
					// Must be <min distance> <rolloff factor> pair
					if(!SkipWhitespace(false)) return false;
					if(!ReadSignedInt(ref info.MinimumDistance) || info.MinimumDistance < 0)
					{
						ReportError("Expected $rolloff log <mindist> value");
						return false;
					}

					if(!SkipWhitespace(false)) return false;
					if(!ReadSignedFloat(ref info.RolloffFactor) || info.RolloffFactor < 0f)
					{
						ReportError("Expected $rolloff log <rolloff factor> value");
						return false;
					}
				}

				// Store type
				switch(token)
				{
					case "custom": info.Rolloff = SoundInfo.RolloffType.CUSTOM; break;
					case "linear": info.Rolloff = SoundInfo.RolloffType.LINEAR; break;
					case "log": info.Rolloff = SoundInfo.RolloffType.LOG; break;
				}
			}
			// Must be <mindist> <maxdist> pair
			else
			{
				if(!ReadSignedInt(token, ref info.MinimumDistance) || info.MinimumDistance < 0)
				{
					ReportError("Expected $rolloff <mindist> value");
					return false;
				}

				if(!SkipWhitespace(false)) return false;
				if(!ReadSignedInt(ref info.MaximumDistance) || info.MaximumDistance < 0)
				{
					ReportError("Expected $rolloff <maxdist> value");
					return false;
				}
			}

			return true;
		}

		// $volume soundname <volume>
		private bool ParseVolume()
		{
			// Read soundname
			if(!SkipWhitespace(true)) return false;
			string soundname = StripTokenQuotes(ReadToken());
			if(string.IsNullOrEmpty(soundname)) return false;
			SoundInfo info = GetSoundInfo(soundname);

			// Read value
			if(!SkipWhitespace(true)) return false;
			if(!ReadSignedFloat(ref info.Volume) || info.Volume < 0f)
			{
				ReportError("Expected $volume value");
				return false;
			}

			// Clamp it
			info.Volume = General.Clamp(info.Volume, 0.0f, 1.0f);

			return true;
		}

		// $random aliasname { logicalname1 logicalname2 logicalname3 ... }
		private bool ParseRandom()
		{
			// Read aliasname
			if(!SkipWhitespace(true)) return false;
			string aliasname = StripTokenQuotes(ReadToken());
			if(string.IsNullOrEmpty(aliasname)) return false;
			SoundInfo info = GetSoundInfo(aliasname);

			// Must be opening brace
			if(!SkipWhitespace(true) || !NextTokenIs("{")) return false;

			// Read logicalnames
			List<string> logicalnames = new List<string>();
			while(true)
			{
				if(!SkipWhitespace(true)) return false;
				string token = StripTokenQuotes(ReadToken());
				if(string.IsNullOrEmpty(token) || token == "}") break;
				logicalnames.Add(token);
			}

			if(logicalnames.Count == 0)
			{
				ReportError("$random " + aliasname + " definition is empty");
				return false;
			}

			if(logicalnames.Contains(aliasname))
			{
				ReportError("$random " + aliasname + " references itself");
				return false;
			}

			// Assign logicalnames
			info.Type = SoundInfo.SoundInfoType.GROUP_RANDOM;
			foreach(string name in logicalnames)
			{
				SoundInfo rinfo = GetSoundInfo(name);
				info.Children.Add(rinfo);
			}

			return true;
		}

		// Reads logicalname lumpname pair
		private bool ParseSoundAssignment(string logicalname)
		{
			// Check logicalname
			logicalname = StripTokenQuotes(logicalname);
			if(string.IsNullOrEmpty(logicalname)) return false;

			// Check sound assignment format and report an error if both the old and new format is used
			if ((format == SndInfoFormat.New && !NextTokenIs("=", false)) || (format == SndInfoFormat.Old && NextTokenIs("=", false)))
			{
				ReportError("Mixing old and new sound assignment format is not permitted");
				return false;
			}
			else if(format == SndInfoFormat.None)
			{
				format = NextTokenIs("=", false) ? SndInfoFormat.New : SndInfoFormat.Old;
			}				

			// Read lumpname
			if(!SkipWhitespace(true)) return false;
			string lumpname = StripTokenQuotes(ReadToken());
			if(string.IsNullOrEmpty(lumpname)) return false;

			SoundInfo info = GetSoundInfo(logicalname);
			info.LumpName = lumpname;

			return true;
		}

		private bool SkipTokens(int count)
		{
			for(int i = 0; i < count; i++)
			{
				if(!SkipWhitespace(true)) return false;
				if(string.IsNullOrEmpty(ReadToken(false))) return false;
			}
			return true;
		}

		#endregion

		#region ================== Methods

		private SoundInfo GetSoundInfo(string soundname)
		{
			if(soundname == "*") return globalprops;
			if(!sounds.ContainsKey(soundname)) sounds[soundname] = new SoundInfo(soundname);
			return sounds[soundname];
		}

		internal void FinishSetup()
		{
			// Check undefined sounds
			List<SoundInfo> toremove = new List<SoundInfo>();
			foreach(SoundInfo sound in sounds.Values)
			{
				if(!IsValid(sound))
				{
					if(sound.Type == SoundInfo.SoundInfoType.SOUND)
						General.ErrorLogger.Add(ErrorType.Warning, ScriptType + " warning: sound \"" + sound.Name + "\" is not defined.");

					toremove.Add(sound);
				}
				else
				{
					// Apply settings from the first child...
					if(sound.Type == SoundInfo.SoundInfoType.GROUP_RANDOM)
					{
						SoundInfo src = sound;
						do
						{
							src = src.Children[0];

						}while(src.Type != SoundInfo.SoundInfoType.SOUND);

						if(src.Type == SoundInfo.SoundInfoType.SOUND)
						{
							sound.Volume = src.Volume;
							sound.Attenuation = src.Attenuation;
							sound.MinimumDistance = src.MinimumDistance;
							sound.MaximumDistance = src.MaximumDistance;
							sound.Rolloff = src.Rolloff;
							sound.RolloffFactor = src.RolloffFactor;
						}
					}
					
					// Apply global settings...
					SoundInfo defprops = new SoundInfo("#DEFAULT_PROPERTIES#");
					if(sound.Volume == defprops.Volume) sound.Volume = globalprops.Volume;
					if(sound.Attenuation == defprops.Attenuation) sound.Attenuation = globalprops.Attenuation;
					if(sound.MinimumDistance == defprops.MinimumDistance) sound.MinimumDistance = globalprops.MinimumDistance;
					if(sound.MaximumDistance == defprops.MaximumDistance) sound.MaximumDistance = globalprops.MaximumDistance;
					if(sound.Rolloff == defprops.Rolloff) sound.Rolloff = globalprops.Rolloff;
					if(sound.RolloffFactor == defprops.RolloffFactor) sound.RolloffFactor = globalprops.RolloffFactor;
				}
			}
			
			// Connect SoundInfos to AmbientSoundInfos...
			foreach(AmbientSoundInfo info in ambientsounds.Values)
			{
				if(!sounds.ContainsKey(info.SoundName))
				{
					General.ErrorLogger.Add(ErrorType.Warning, ScriptType + " warning: $ambient sound " + info.Index + " has undefined sound \"" + info.SoundName + "\".");
					continue;
				}
				
				info.SetupSound(sounds[info.SoundName]);
			}

			// Remove invalid sounds
			foreach(SoundInfo info in toremove) sounds.Remove(info.Name);
		}

		private static bool IsValid(SoundInfo info)
		{
			switch(info.Type)
			{
				case SoundInfo.SoundInfoType.SOUND:
					return !string.IsNullOrEmpty(info.LumpName) || General.Map.Config.InternalSoundNames.Contains(info.Name);

				case SoundInfo.SoundInfoType.GROUP_RANDOM:
					foreach(SoundInfo child in info.Children)
						if(!IsValid(child)) return false;
					return true;
				
				default:
					throw new NotImplementedException("Unknown SoundInfoType");
			}
		}

		#endregion
	}
}
