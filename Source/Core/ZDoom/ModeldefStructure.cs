#region ================== Namespaces

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Globalization;
using CodeImp.DoomBuilder.GZBuilder.Data;
using CodeImp.DoomBuilder.Rendering;

#endregion

namespace CodeImp.DoomBuilder.ZDoom
{
	internal sealed class ModeldefStructure
	{
		#region ================== Structs

		internal struct FrameStructure
		{
			public string SpriteName; // Stays here for HashSet duplicate checks
			public int ModelIndex;
			public int FrameIndex;
			public string FrameName;
		}

		#endregion

		#region ================== Variables

		private Dictionary<int, string> skinnames;
		private Dictionary<int, Dictionary<int, string>> surfaceskinenames;
		private Dictionary<int, string> modelnames;
		private string path;
		private Vector3f scale;
		private Vector3f offset;
		private float angleoffset;
		private float pitchoffset;
		private float rolloffset;
		private bool inheritactorpitch;
		private bool useactorpitch;
		private bool useactorroll;

		private Dictionary<string, HashSet<FrameStructure>> frames;

		#endregion

		#region ================== Properties

		public Dictionary<int, string> SkinNames { get { return skinnames; } }
		public Dictionary<int, Dictionary<int, string>> SurfaceSkinNames { get { return surfaceskinenames; } }
		public Dictionary<int, string> ModelNames { get { return modelnames; } }
		public Vector3f Scale { get { return scale; } }
		public Vector3f Offset { get { return offset; } }
		public float AngleOffset { get { return angleoffset; } }
		public float PitchOffset { get { return pitchoffset; } }
		public float RollOffset { get { return rolloffset; } }
		public bool InheritActorPitch { get { return inheritactorpitch; } }
		public bool UseActorPitch { get { return useactorpitch; } }
		public bool UseActorRoll { get { return useactorroll; } }
		public string DataPath { get { return path; } } // biwa

		public Dictionary<string, HashSet<FrameStructure>> Frames { get { return frames; } }

		#endregion

		#region ================== Constructor

		internal ModeldefStructure()
		{
			path = string.Empty;
			skinnames = new Dictionary<int, string>();
			modelnames = new Dictionary<int, string>();
			frames = new Dictionary<string, HashSet<FrameStructure>>(StringComparer.OrdinalIgnoreCase);
			scale = new Vector3f(1.0f, 1.0f, 1.0f);
			surfaceskinenames = new Dictionary<int, Dictionary<int, string>>();
		}

		#endregion

		#region ================== Parsing

		internal bool Parse(ModeldefParser parser)
		{
			// Read modeldef structure contents
			bool parsingfinished = false;
			while(!parsingfinished && parser.SkipWhitespace(true))
			{
				string token = parser.ReadToken().ToLowerInvariant();
				if(string.IsNullOrEmpty(token)) continue;

				switch(token)
				{
					case "path":
						parser.SkipWhitespace(true);
						path = parser.StripTokenQuotes(parser.ReadToken(false)).Replace("\\", "/"); // Don't skip newline
						if(string.IsNullOrEmpty(path))
						{
							parser.ReportError("Expected model path");
							return false;
						}
						break;

					case "model":
						parser.SkipWhitespace(true);

						// Model index
						int index = 0;
						token = parser.ReadToken();
						if(!parser.ReadSignedInt(token, ref index))
						{
							// Not numeric!
							parser.ReportError("Expected model index, but got \"" + token + "\"");
							return false;
						}

						if(index < 0)
						{
							// Out of bounds
							parser.ReportError("Model index must not be in negative");
							return false;
						}

						parser.SkipWhitespace(true);

						// Model path
						token = parser.StripTokenQuotes(parser.ReadToken(false)).ToLowerInvariant(); // Don't skip newline
						if(string.IsNullOrEmpty(token))
						{
							parser.ReportError("Expected model name");
							return false;
						}

						// Check invalid path chars
						if(!parser.CheckInvalidPathChars(token)) return false;

						// Check extension
						string modelext = Path.GetExtension(token);
						if(string.IsNullOrEmpty(modelext))
						{
							parser.ReportError("Model \"" + token + "\" won't be loaded. Models without extension are not supported by GZDoom");
							return false;
						}

						if(modelext != ".md3" && modelext != ".md2" && modelext != ".3d" && modelext != ".obj")
						{
							parser.ReportError("Model \"" + token + "\" won't be loaded. Only Unreal 3D, MD2, MD3, and OBJ models are supported");
							return false;
						}

						// GZDoom allows models with identical index, it uses the last one encountered
						modelnames[index] = Path.Combine(path, token).Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
						break;

					case "skin":
						parser.SkipWhitespace(true);

						// Skin index
						int skinindex = 0;
						token = parser.ReadToken();
						if(!parser.ReadSignedInt(token, ref skinindex))
						{
							// Not numeric!
							parser.ReportError("Expected skin index, but got \"" + token + "\"");
							return false;
						}

						if(skinindex < 0)
						{
							// Out of bounds
							parser.ReportError("Skin index must not be negative");
							return false;
						}

						parser.SkipWhitespace(true);

						// Skin path
						token = parser.StripTokenQuotes(parser.ReadToken(false)).ToLowerInvariant(); // Don't skip newline
						if(string.IsNullOrEmpty(token))
						{
							parser.ReportError("Expected skin path");
							return false;
						}

						// Check invalid path chars
						if(!parser.CheckInvalidPathChars(token)) return false;

						// GZDoom allows skins with identical index, it uses the last one encountered
						skinnames[skinindex] = Path.Combine(path, token);
						break;

					// SurfaceSkin <int modelindex> <int surfaceindex> <string skinfile>
					case "surfaceskin":
						parser.SkipWhitespace(true);

						// Model index
						int modelindex = 0;
						token = parser.ReadToken();
						if(!parser.ReadSignedInt(token, ref modelindex))
						{
							// Not numeric!
							parser.ReportError("Expected model index, but got \"" + token + "\"");
							return false;
						}

						if(modelindex < 0)
						{
							// Out of bounds
							parser.ReportError("Model index must not be negative");
							return false;
						}

						parser.SkipWhitespace(true);

						// Surfaceindex index
						int surfaceindex = 0;
						token = parser.ReadToken();
						if(!parser.ReadSignedInt(token, ref surfaceindex))
						{
							// Not numeric!
							parser.ReportError("Expected surface index, but got \"" + token + "\"");
							return false;
						}

						if(surfaceindex < 0)
						{
							// Out of bounds
							parser.ReportError("Surface index must be positive integer");
							return false;
						}

						parser.SkipWhitespace(true);

						// Skin path
						token = parser.StripTokenQuotes(parser.ReadToken(false)).ToLowerInvariant(); // Don't skip newline
						if(string.IsNullOrEmpty(token))
						{
							parser.ReportError("Expected skin path");
							return false;
						}

						// Check invalid path chars
						if(!parser.CheckInvalidPathChars(token)) return false;

						// Store
						if (!surfaceskinenames.ContainsKey(modelindex))
							surfaceskinenames[modelindex] = new Dictionary<int, string>();

						surfaceskinenames[modelindex][surfaceindex] = Path.Combine(path, token).Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
						break;

					case "scale":
						parser.SkipWhitespace(true);
						token = parser.ReadToken();
						if(!parser.ReadSignedFloat(token, ref scale.Y))
						{
							// Not numeric!
							parser.ReportError("Expected Scale X value, but got \"" + token + "\"");
							return false;
						}

						parser.SkipWhitespace(true);
						token = parser.ReadToken();
						if(!parser.ReadSignedFloat(token, ref scale.X))
						{
							// Not numeric!
							parser.ReportError("Expected Scale Y value, but got \"" + token + "\"");
							return false;
						}

						parser.SkipWhitespace(true);
						token = parser.ReadToken();
						if(!parser.ReadSignedFloat(token, ref scale.Z))
						{
							// Not numeric!
							parser.ReportError("Expected Scale Z value, but got \"" + token + "\"");
							return false;
						}
						break;

					case "offset":
						parser.SkipWhitespace(true);
						token = parser.ReadToken();
						if(!parser.ReadSignedFloat(token, ref offset.X))
						{
							// Not numeric!
							parser.ReportError("Expected Offset X value, but got \"" + token + "\"");
							return false;
						}

						parser.SkipWhitespace(true);
						token = parser.ReadToken();
						if(!parser.ReadSignedFloat(token, ref offset.Y))
						{
							// Not numeric!
							parser.ReportError("Expected Offset Y value, but got \"" + token + "\"");
							return false;
						}

						parser.SkipWhitespace(true);
						token = parser.ReadToken();
						if(!parser.ReadSignedFloat(token, ref offset.Z))
						{
							// Not numeric!
							parser.ReportError("Expected Offset Z value, but got \"" + token + "\"");
							return false;
						}
						break;

					case "zoffset":
						parser.SkipWhitespace(true);
						token = parser.ReadToken();
						if(!parser.ReadSignedFloat(token, ref offset.Z))
						{
							// Not numeric!
							parser.ReportError("Expected ZOffset value, but got \"" + token + "\"");
							return false;
						}
						break;

					case "angleoffset":
						parser.SkipWhitespace(true);
						token = parser.ReadToken();
						if(!parser.ReadSignedFloat(token, ref angleoffset))
						{
							// Not numeric!
							parser.ReportError("Expected AngleOffset value, but got \"" + token + "\"");
							return false;
						}
						break;

					case "pitchoffset":
						parser.SkipWhitespace(true);
						token = parser.ReadToken();
						if(!parser.ReadSignedFloat(token, ref pitchoffset))
						{
							// Not numeric!
							parser.ReportError("Expected PitchOffset value, but got \"" + token + "\"");
							return false;
						}
						break;

					case "rolloffset":
						parser.SkipWhitespace(true);
						token = parser.ReadToken();
						if(!parser.ReadSignedFloat(token, ref rolloffset))
						{
							// Not numeric!
							parser.ReportError("Expected RollOffset value, but got \"" + token + "\"");
							return false;
						}
						break;

					case "useactorpitch":
						inheritactorpitch = false;
						useactorpitch = true;
						break;

					case "useactorroll":
						useactorroll = true;
						break;

					case "inheritactorpitch":
						inheritactorpitch = true;
						useactorpitch = false;
						parser.LogWarning("INHERITACTORPITCH flag is deprecated. Consider using USEACTORPITCH flag instead");
						break;

					case "inheritactorroll":
						useactorroll = true;
						parser.LogWarning("INHERITACTORROLL flag is deprecated. Consider using USEACTORROLL flag instead");
						break;

					//FrameIndex <XXXX> <X> <model index> <frame number>
					case "frameindex":
						// Sprite name
						parser.SkipWhitespace(true);
						string fispritename = ZDTextParser.StripQuotes(parser.ReadToken());
						if(string.IsNullOrEmpty(fispritename))
						{
							parser.ReportError("Expected sprite name");
							return false;
						}
						if(fispritename.Length != 4)
						{
							parser.ReportError("Sprite name must be 4 characters long, got \"" + fispritename + "\"");
							return false;
						}

						// Sprite frame
						parser.SkipWhitespace(true);
						token = ZDTextParser.StripQuotes(parser.ReadToken());
						if(string.IsNullOrEmpty(token))
						{
							parser.ReportError("Expected sprite frame");
							return false;
						}
						if(token.Length != 1)
						{
							parser.ReportError("Sprite frame must be 1 character long");
							return false;
						}

						// Make full name
						fispritename += token;

						// Model index
						parser.SkipWhitespace(true);
						int fimodelindnex = 0;
						token = parser.ReadToken();
						if(!parser.ReadSignedInt(token, ref fimodelindnex))
						{
							// Not numeric!
							parser.ReportError("Expected model index, but got \"" + token + "\"");
							return false;
						}
						if(fimodelindnex < 0)
						{
							// Out of bounds
							parser.ReportError("Model index must not be negative");
							return false;
						}

						// Frame number
						parser.SkipWhitespace(true);
						int fiframeindnex = 0;
						token = parser.ReadToken();
						//INFO: setting frame index to a negative number disables model rendering in GZDoom
						if(!parser.ReadSignedInt(token, ref fiframeindnex))
						{
							// Not numeric!
							parser.ReportError("Expected frame index, but got \"" + token + "\"");
							return false;
						}

						// Add to collection
						FrameStructure fifs = new FrameStructure { FrameIndex = fiframeindnex, ModelIndex = fimodelindnex, SpriteName = fispritename };
						if(!frames.ContainsKey(fispritename))
						{
							frames.Add(fispritename, new HashSet<FrameStructure>());
							frames[fispritename].Add(fifs);
						}
						else if(frames[fispritename].Contains(fifs))
						{
							parser.LogWarning("Duplicate FrameIndex definition");
						}
						else
						{
							frames[fispritename].Add(fifs);
						}
						break;

					//Frame <XXXX> <X> <model index> <"frame name">
					case "frame":
						// Sprite name
						parser.SkipWhitespace(true);
						string spritename = parser.ReadToken();
						if(string.IsNullOrEmpty(spritename))
						{
							parser.ReportError("Expected sprite name");
							return false;
						}
						if(spritename.Length != 4)
						{
							parser.ReportError("Sprite name must be 4 characters long");
							return false;
						}

						// Sprite frame
						parser.SkipWhitespace(true);
						token = parser.ReadToken();
						if(string.IsNullOrEmpty(token))
						{
							parser.ReportError("Expected sprite frame");
							return false;
						}
						if(token.Length != 1)
						{
							parser.ReportError("Sprite frame must be 1 character long");
							return false;
						}

						// Make full name
						spritename += token;

						// Model index
						parser.SkipWhitespace(true);
						int modelindnex = 0;
						token = parser.ReadToken();
						if(!parser.ReadSignedInt(token, ref modelindnex))
						{
							// Not numeric!
							parser.ReportError("Expected model index, but got \"" + token + "\"");
							return false;
						}
						if(modelindnex < 0)
						{
							// Out of bounds
							parser.ReportError("Model index must not be negative");
							return false;
						}

						// Frame name
						parser.SkipWhitespace(true);
						string framename = parser.StripTokenQuotes(parser.ReadToken());
						if(string.IsNullOrEmpty(framename))
						{
							parser.ReportError("Expected frame name");
							return false;
						}

						// Add to collection
						FrameStructure fs = new FrameStructure { FrameName = framename, ModelIndex = modelindnex, SpriteName = spritename };
						if(!frames.ContainsKey(spritename))
						{
							frames.Add(spritename, new HashSet<FrameStructure>());
							frames[spritename].Add(fs);
						}
						else if(frames[spritename].Contains(fs))
						{
							parser.LogWarning("Duplicate Frame definition");
						}
						else
						{
							frames[spritename].Add(fs);
						}
						break;

					case "{":
						parser.ReportError("Unexpected scope start");
						return false;

					// Structure ends here
					case "}":
						parsingfinished = true;
						break;
				}
			}

			// Perform some integrity checks
			if(!parsingfinished)
			{
				parser.ReportError("Unclosed structure scope");
				return false;
			}

			// Any models defined?
			if(modelnames.Count == 0)
			{
				parser.ReportError("Structure doesn't define any models");
				return false;
			}

			foreach(int i in skinnames.Keys.OrderBy(k => k))
			{
				if (!string.IsNullOrEmpty(skinnames[i]) && !modelnames.ContainsKey(i))
				{
					parser.ReportError("No model is defined for skin " + i + ":\"" + skinnames[i] + "\"");
					return false;
				}
			}

			foreach(int i in surfaceskinenames.Keys.OrderBy(k => k))
			{
				if (surfaceskinenames[i].Count > 0 && !modelnames.ContainsKey(i))
				{
					parser.ReportError("No model is defined for surface skin " + i);
					return false;
				}
			}

			return true;
		}

		#endregion
	}
}
