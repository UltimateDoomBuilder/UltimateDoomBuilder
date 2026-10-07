#region ================== Namespaces

using System;
using System.Collections.Generic;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.GZBuilder.Data;
using CodeImp.DoomBuilder.Rendering;
using System.IO;

#endregion

namespace CodeImp.DoomBuilder.ZDoom 
{
	internal class ModeldefParser : ZDTextParser
	{
        public delegate void IncludeDelegate(ModeldefParser parser, string includefile);

        public IncludeDelegate OnInclude;


        #region ================== Variables

        private readonly Dictionary<string, int> actorsbyclass;
		private Dictionary<string, ModelData> entries; //classname, entry
        //mxd. Includes tracking
        private HashSet<string> parsedlumps;


        #endregion

        #region ================== Properties

        internal override ScriptType ScriptType { get { return ScriptType.MODELDEF; } }
		internal Dictionary<string, ModelData> Entries { get { return entries; } }

		#endregion

		#region ================== Constructor

		internal ModeldefParser(Dictionary<string, int> actorsbyclass)
		{
			this.actorsbyclass = actorsbyclass;
			this.entries = new Dictionary<string, ModelData>(StringComparer.OrdinalIgnoreCase);
            this.parsedlumps = new HashSet<string>();

			// We don't want the '-' as a special token because commands can contain them (like "rotation-center")
			specialtokens = ":{}+\n;";
		}

		#endregion

		#region ================== Parsing

		// Should be called after all decorate actors are parsed 
		public override bool Parse(TextResourceData data, bool clearerrors)
		{
			// Already parsed?
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
            BinaryReader localreader = datareader;
            DataLocation locallocation = datalocation; //mxd
            string localtextresourcepath = textresourcepath; //mxd

            // Continue until at the end of the stream
            while (SkipWhitespace(true)) 
			{
				string token = ReadToken();

				if(string.IsNullOrEmpty(token) || token.ToLowerInvariant() != "model")
                {
                    if (token != null && token.ToLowerInvariant() == "#include")
                    {
                        //INFO: ZDoom DECORATE include paths can't be relative ("../actor.txt") 
                        //or absolute ("d:/project/actor.txt") 
                        //or have backward slashes ("info\actor.txt")
                        //include paths are relative to the first parsed entry, not the current one 
                        //also include paths may or may not be quoted
                        SkipWhitespace(true);
                        string filename = StripQuotes(ReadToken(false)); //mxd. Don't skip newline

                        //mxd. Sanity checks
                        if (string.IsNullOrEmpty(filename))
                        {
                            ReportError("Expected file name to include");
                            return false;
                        }

                        //mxd. Check invalid path chars
                        if (!CheckInvalidPathChars(filename)) return false;

                        //mxd. Absolute paths are not supported...
                        if (Path.IsPathRooted(filename))
                        {
                            ReportError("Absolute include paths are not supported by ZDoom");
                            return false;
                        }

                        //mxd. Relative paths are not supported
                        if (filename.StartsWith(RELATIVE_PATH_MARKER) || filename.StartsWith(CURRENT_FOLDER_PATH_MARKER) ||
                            filename.StartsWith(ALT_RELATIVE_PATH_MARKER) || filename.StartsWith(ALT_CURRENT_FOLDER_PATH_MARKER))
                        {
                            ReportError("Relative include paths are not supported by ZDoom");
                            return false;
                        }

                        //mxd. Backward slashes are not supported
                        if (filename.Contains("\\"))
                        {
                            ReportError("Only forward slashes are supported by ZDoom");
                            return false;
                        }

                        //mxd. Already parsed?
                        if (parsedlumps.Contains(filename))
                        {
                            ReportError("Already parsed \"" + filename + "\". Check your include directives");
                            return false;
                        }

                        //mxd. Add to collection
                        parsedlumps.Add(filename);

                        // Callback to parse this file now
                        if (OnInclude != null) OnInclude(this, filename);

                        //mxd. Bail out on error
                        if (this.HasError) return false;

                        // Set our buffers back to continue parsing
                        datastream = localstream;
                        datareader = localreader;
                        sourcename = localsourcename;
                        datalocation = locallocation; //mxd
                        textresourcepath = localtextresourcepath; //mxd
                    }

                    continue;
                }

				// Find classname
				SkipWhitespace(true);
				string classname = StripQuotes(ReadToken(ActorStructure.ACTOR_CLASS_SPECIAL_TOKENS));
				if(string.IsNullOrEmpty(classname))
				{
					ReportError("Expected actor class");
					return false;
				}

				// Check if actor exists
				bool haveplaceableactor = actorsbyclass.ContainsKey(classname);
				if(!haveplaceableactor && (General.Map.Data.GetZDoomActor(classname) == null))
					LogWarning("DECORATE class \"" + classname + "\" does not exist");
				
				// Now find opening brace
				if(!NextTokenIs("{")) return false;

				// Parse the structure
				ModeldefStructure mds = new ModeldefStructure();
				if(mds.Parse(this))
				{
					// Fetch Actor info
					if(haveplaceableactor)
					{
						ThingTypeInfo info = General.Map.Data.GetThingInfoEx(actorsbyclass[classname]);
						if(info != null)
						{
							// Already have a voxel model?
							if(General.Map.Data.ModeldefEntries.ContainsKey(info.Index) && General.Map.Data.ModeldefEntries[info.Index].IsVoxel)
							{
								LogWarning("Both voxel(s) and model(s) are defined for actor\"" + classname + "\". Consider using either former or latter");
							}
							// Actor has a valid sprite?
							else if(!string.IsNullOrEmpty(info.Sprite) && !info.Sprite.ToLowerInvariant().StartsWith(DataManager.INTERNAL_PREFIX)
								&& (info.Sprite.Length == 6 || info.Sprite.Length == 8))
							{
								string targetsprite = info.Sprite.Substring(0, 5);
								if(mds.Frames.ContainsKey(targetsprite))
								{
									// Create model data
									ModelData md = new ModelData { InheritActorPitch = mds.InheritActorPitch, UseActorPitch = mds.UseActorPitch, UseActorRoll = mds.UseActorRoll, UseRotationCenter = mds.UseRotationCenter, RotationCenter = mds.RotationCenter, Path = mds.DataPath };

									// Things are complicated in GZDoom...
									Matrix moffset = Matrix.Translation(mds.Offset.Y, -mds.Offset.X, mds.Offset.Z);
                                    //Matrix mrotation = Matrix.RotationZ(Angle2D.DegToRad(mds.AngleOffset)) * Matrix.RotationY(-Angle2D.DegToRad(mds.RollOffset)) * Matrix.RotationX(-Angle2D.DegToRad(mds.PitchOffset));
                                    Matrix mrotation = Matrix.RotationY((float)-Angle2D.DegToRad(mds.RollOffset)) * Matrix.RotationX((float)- Angle2D.DegToRad(mds.PitchOffset)) * Matrix.RotationZ((float)Angle2D.DegToRad(mds.AngleOffset));
                                    md.SetTransform(mrotation, moffset, mds.Scale);

									// Add models
									int disabledframescount = 0;
									foreach(var fs in mds.Frames[targetsprite])
									{
										// Sanity checks
										if (!mds.ModelNames.ContainsKey(fs.ModelIndex) || string.IsNullOrEmpty(mds.ModelNames[fs.ModelIndex]))
										{
											LogWarning("Model definition \"" + classname + "\", frame \"" + fs.SpriteName + " " + fs.FrameName + "\" references undefined model index " + fs.ModelIndex);
											continue;
										}

										//INFO: setting frame index to a negative number disables model rendering in GZDoom
										if(fs.FrameIndex < 0)
										{
											disabledframescount++;
											continue;
										}

										// Texture name will be empty when skin path is embedded in the model
										string skinname = mds.SkinNames.ContainsKey(fs.ModelIndex) ? mds.SkinNames[fs.ModelIndex].ToLowerInvariant() : string.Empty;

										md.SkinNames.Add(skinname);

										if (mds.SurfaceSkinNames.ContainsKey(fs.ModelIndex))
											md.SurfaceSkinNames.Add(mds.SurfaceSkinNames[fs.ModelIndex]);
										else
											md.SurfaceSkinNames.Add(new Dictionary<int, string>());

										if (mds.ModelNames.ContainsKey(fs.ModelIndex))
											md.ModelNames.Add(mds.ModelNames[fs.ModelIndex].ToLowerInvariant());
										else
											md.ModelNames.Add(string.Empty);

										md.FrameNames.Add(fs.FrameName);
										md.FrameIndices.Add(fs.FrameIndex);
									}

									// More sanity checks...
									if(md.ModelNames.Count == 0)
									{
										// Show warning only when frames were not delibeartely disabled
										if(mds.Frames[targetsprite].Count > 0 && disabledframescount < mds.Frames[targetsprite].Count)
											LogWarning("Model definition \"" + classname + "\" has no defined models");
									}
									else
									{
										// Add to collection
										entries[classname] = md;
									}
								}
							}
						}
					}
				}
							
				if(HasError)
				{
					LogError();
					ClearError();
				}
			}

			return true;
		}

		#endregion
	}
}
