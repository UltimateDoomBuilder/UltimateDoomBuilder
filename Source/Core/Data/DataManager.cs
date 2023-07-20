
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
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Data.Scripting;
using CodeImp.DoomBuilder.GZBuilder.Data;
using CodeImp.DoomBuilder.GZBuilder.Models;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.IO;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Windows;
using CodeImp.DoomBuilder.ZDoom;
using Matrix = CodeImp.DoomBuilder.Rendering.Matrix;
using CodeImp.DoomBuilder.Controls;
using CodeImp.DoomBuilder.Dehacked;
using System.Diagnostics;

#endregion

namespace CodeImp.DoomBuilder.Data
{
	public sealed class DataManager
	{
		#region ================== Constants
		
		public const string INTERNAL_PREFIX = "internal:";
		public const int CLASIC_IMAGE_NAME_LENGTH = 8; //mxd
		private const int MAX_SKYTEXTURE_SIZE = 2048; //mxd
		internal static readonly char[] CATEGORY_SPLITTER = { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }; //mxd

		private long UNKNOWN_THING; //mxd
		private long MISSING_THING; //mxd
		
		#endregion

		#region ================== Variables
		
		// Data containers
		private List<DataReader> containers;
		private DataReader currentreader;
		
		// Palette
		private Playpal palette;
		private ColorMap mainColormap;

		// Textures, Flats and Sprites
		private Dictionary<long, ImageData> textures;
		private Dictionary<long, long> texturenamesshorttofull; //mxd
		private List<string> texturenames;
		private Dictionary<long, ImageData> flats;
		private Dictionary<long, long> flatnamesshorttofull; //mxd
		private Dictionary<long, long> flatnamesfulltoshort; //mxd
		private List<string> flatnames;
		private Dictionary<long, ImageData> sprites;
		private List<MatchingTextureSet> texturesets;
		private List<ResourceTextureSet> resourcetextures;
		private AllTextureSet alltextures;
		private AllTextureSet walltextureset;
		private AllTextureSet flattextureset;

		//mxd 
		private Dictionary<int, ModelData> modeldefentries; //Thing.Type, Model entry
		private Dictionary<int, DynamicLightData> gldefsentries; //Thing.Type, Light entry
		private MapInfo mapinfo;
		private Dictionary<string, KeyValuePair<int, int>> reverbs; //<name, <arg1, arg2> 
		private Dictionary<long, GlowingFlatData> glowingflats; // Texture name hash, Glowing Flat Data
		private Dictionary<string, SkyboxInfo> skyboxes; 
		private string[] soundsequences;
		private string[] terrainnames; 
		private string[] damagetypes;
		private Dictionary<string, PixelColor> knowncolors; // Colors parsed from X11R6RGB lump. Color names are lowercase without spaces
		private CvarsCollection cvars; // Variables parsed from CVARINFO
		private Dictionary<int, PixelColor> lockcolors; // Lock colors defined in LOCKDEFS
		private Dictionary<int, int> lockableactions; // <Action number, arg referenceing "keys" enum number>
		private Dictionary<int, AmbientSoundInfo> ambientsounds;

		//mxd. Text resources
		private Dictionary<ScriptType, HashSet<ScriptResource>> scriptresources;

        // Background loading
        private object syncobject = new object();
		private Queue<ImageData> imageque;
		private Thread[] backgroundloader;
		
		// Special images
		private ImageData missingtexture3d;
		private ImageData unknowntexture3d;
		private UnknownImage unknownimage; //mxd
		private ImageData hourglass3d;
		private ImageData crosshair;
		private ImageData crosshairbusy;
		private Dictionary<string, long> internalspriteslookup; //mxd
		private ImageData whitetexture;
		private ImageData blacktexture; //mxd
		private ImageData thingtexture; //mxd

		//mxd. Texture Browser images
		private ImageData foldertexture;
		private ImageData folderuptexture;

		//mxd. Sky textures
		private CubeTexture skybox; // GZDoom skybox

		//mxd. Comment icons
		private ImageData[] commenttextures;
		
		// Things combined with things created from Decorate
		private DecorateParser decorate;
        private ZScriptParser zscript;
        private Dictionary<string, ActorStructure> zdoomclasses;
		private List<ThingCategory> thingcategories;
		private Dictionary<int, ThingTypeInfo> thingtypes;

		// Dehacked
		private DehackedParser dehacked;
		
		// Disposing
		private bool isdisposed;

		#endregion

		#region ================== Properties

		//mxd
		internal Dictionary<int, ModelData> ModeldefEntries { get { return modeldefentries; } }
		internal Dictionary<int, DynamicLightData> GldefsEntries { get { return gldefsentries; } }
		public MapInfo MapInfo { get { return mapinfo; } }
		public Dictionary<string, KeyValuePair<int, int>> Reverbs { get { return reverbs; } }
		public Dictionary<long, GlowingFlatData> GlowingFlats { get { return glowingflats; } }
		public string[] SoundSequences { get { return soundsequences; } }
		public string[] TerrainNames { get { return terrainnames; } }
		public string[] DamageTypes { get { return damagetypes; } }
		public Dictionary<string, PixelColor> KnownColors { get { return knowncolors; } }
		internal Dictionary<ScriptType, HashSet<ScriptResource>> ScriptResources { get { return scriptresources; } }
		internal CvarsCollection CVars { get { return cvars; } }
		public Dictionary<int, PixelColor> LockColors { get { return lockcolors; } }
		public Dictionary<int, int> LockableActions { get { return lockableactions; } }
		public Dictionary<int, AmbientSoundInfo> AmbientSounds { get { return ambientsounds; } }

		//mxd
		internal IEnumerable<DataReader> Containers { get { return containers; } }

		public Playpal Palette { get { return palette; } }
		public ColorMap MainColorMap { get { return mainColormap; } }		
		public ICollection<ImageData> Textures { get { return textures.Values; } }
		public ICollection<ImageData> Flats { get { return flats.Values; } }
		public List<string> TextureNames { get { return texturenames; } }
		public List<string> FlatNames { get { return flatnames; } }
		public bool IsDisposed { get { return isdisposed; } }
		public ImageData MissingTexture3D { get { return missingtexture3d; } }
		public ImageData UnknownTexture3D { get { return unknowntexture3d; } }
        public ImageData UnknownImage {  get { return unknownimage; } }
		public ImageData Hourglass3D { get { return hourglass3d; } }
		public ImageData Crosshair3D { get { return crosshair; } }
		public ImageData CrosshairBusy3D { get { return crosshairbusy; } }
        public Texture LoadingTexture { get; private set; }
        public Texture FailedTexture { get; private set; }
        public ImageData WhiteTexture { get { return whitetexture; } }
		public ImageData BlackTexture { get { return blacktexture; } } //mxd
		public ImageData ThingTexture { get { return thingtexture; } } //mxd
		internal ImageData FolderTexture { get { return foldertexture; } } //mxd
		internal ImageData FolderUpTexture { get { return folderuptexture; } } //mxd
		public ImageData[] CommentTextures { get { return commenttextures; } } //mxd
		internal CubeTexture SkyBox { get { return skybox; } } //mxd
		public List<ThingCategory> ThingCategories { get { return thingcategories; } }
		public ICollection<ThingTypeInfo> ThingTypes { get { return thingtypes.Values; } }
		public DecorateParser Decorate { get { return decorate; } }
        public ZScriptParser ZScript { get { return zscript; } }
		internal ICollection<MatchingTextureSet> TextureSets { get { return texturesets; } }
		internal ICollection<ResourceTextureSet> ResourceTextureSets { get { return resourcetextures; } }
		internal AllTextureSet AllTextureSet { get { return alltextures; } }
		internal AllTextureSet WallTextureSet { get { return walltextureset; } }
		internal AllTextureSet FlatTextureSet { get { return flattextureset; } }
		
		public bool IsLoading
		{
			get
			{
				if(imageque != null)
					return (backgroundloader != null) && backgroundloader.Any(x => x.IsAlive) && ((imageque.Count > 0));
				return false;
			}
		}

        internal const float DOOM_PIXEL_RATIO = 1.2f;

        public float VerticalViewStretch
        {
            get
            {
                if (mapinfo == null)
                    return DOOM_PIXEL_RATIO;
                return mapinfo.PixelRatio;
            }
        }

        public float InvertedVerticalViewStretch
        {
            get
            {
                return 1.0f / VerticalViewStretch;
            }
        }

        #endregion

        #region ================== Constructor / Disposer

        // Constructor
        internal DataManager()
		{
            FailedTexture = new Texture(General.Map.Graphics, Properties.Resources.Failed);
            LoadingTexture = new Texture(General.Map.Graphics, Properties.Resources.Hourglass);

            // Load special images (mxd: the rest is loaded in LoadInternalTextures())
            whitetexture = new ResourceImage("CodeImp.DoomBuilder.Resources.White.png") { UseColorCorrection = false };
			blacktexture = new ResourceImage("CodeImp.DoomBuilder.Resources.Black.png") { UseColorCorrection = false }; //mxd
			unknownimage = new UnknownImage(); //mxd. There should be only one!

			//mxd. Textures browser images
			foldertexture = new ResourceImage("CodeImp.DoomBuilder.Resources.Folder96.png") { UseColorCorrection = false };
			folderuptexture = new ResourceImage("CodeImp.DoomBuilder.Resources.Folder96Up.png") { UseColorCorrection = false };

			//mxd. Create comment icons
			commenttextures = new ImageData[]
			                  {
				                  new ResourceImage("CodeImp.DoomBuilder.Resources.CommentRegular.png") { UseColorCorrection = false },
								  new ResourceImage("CodeImp.DoomBuilder.Resources.CommentInfo.png") { UseColorCorrection = false },
								  new ResourceImage("CodeImp.DoomBuilder.Resources.CommentQuestion.png") { UseColorCorrection = false },
								  new ResourceImage("CodeImp.DoomBuilder.Resources.CommentProblem.png") { UseColorCorrection = false },
								  new ResourceImage("CodeImp.DoomBuilder.Resources.CommentSmile.png") { UseColorCorrection = false },
			                  };
		}
		
		// Disposer
		internal void Dispose()
		{
			// Not already disposed?
			if(!isdisposed)
			{
				// Clean up
				Unload();
				missingtexture3d.Dispose();
				missingtexture3d = null;
				unknowntexture3d.Dispose();
				unknowntexture3d = null;
				hourglass3d.Dispose();
				hourglass3d = null;
				crosshair.Dispose();
				crosshair = null;
				crosshairbusy.Dispose();
				crosshairbusy = null;
				whitetexture.Dispose();
				whitetexture = null;
				blacktexture.Dispose(); //mxd
				blacktexture = null; //mxd
				thingtexture.Dispose(); //mxd
				thingtexture = null; //mxd
				unknownimage.Dispose(); //mxd
				unknownimage = null; //mxd
				foldertexture.Dispose(); //mxd
				foldertexture = null; //mxd
				folderuptexture.Dispose(); //mxd
				folderuptexture = null; //mxd
				for(int i = 0; i < commenttextures.Length; i++) //mxd
				{
					commenttextures[i].Dispose();
					commenttextures[i] = null;
				}
				commenttextures = null;
				if(skybox != null) //mxd
				{
					skybox.Dispose();
					skybox = null;
				}
				
				// Done
				isdisposed = true;
			}
		}

		#endregion

		#region ================== Loading / Unloading

		// This loads all data resources
		internal void Load(DataLocationList configlist, DataLocationList maplist, DataLocation maplocation)
		{
			//mxd. Don't modify original lists
			DataLocationList configlistcopy = new DataLocationList(configlist);
			DataLocationList maplistcopy = new DataLocationList(maplist);

			//mxd. If maplocation was already added as a resource, make sure it's singular and is the last in the list
			configlistcopy.Remove(maplocation);
			maplistcopy.Remove(maplocation);
			maplistcopy.Add(maplocation);

			Load(configlistcopy, maplistcopy);
		}

		// This loads all data resources
		internal void Load(DataLocationList configlist, DataLocationList maplist)
		{
			Dictionary<long, ImageData> texturesonly = new Dictionary<long, ImageData>();
			Dictionary<long, ImageData> colormapsonly = new Dictionary<long, ImageData>();
			Dictionary<long, ImageData> flatsonly = new Dictionary<long, ImageData>();

			// Create collections
			containers = new List<DataReader>();
			textures = new Dictionary<long, ImageData>();
			flats = new Dictionary<long, ImageData>();
			sprites = new Dictionary<long, ImageData>();
			texturenames = new List<string>();
			flatnames = new List<string>();
			texturenamesshorttofull = new Dictionary<long, long>(); //mxd
			flatnamesshorttofull = new Dictionary<long, long>(); //mxd
			flatnamesfulltoshort = new Dictionary<long, long>(); //mxd
			imageque = new Queue<ImageData>();
			texturesets = new List<MatchingTextureSet>();
			thingcategories = General.Map.Config.GetThingCategories();
			thingtypes = General.Map.Config.GetThingTypes();

			//mxd. Create even more collections!
			modeldefentries = new Dictionary<int, ModelData>();
			gldefsentries = new Dictionary<int, DynamicLightData>();
			reverbs = new Dictionary<string, KeyValuePair<int, int>>(StringComparer.Ordinal);
			glowingflats = new Dictionary<long, GlowingFlatData>();
			skyboxes = new Dictionary<string, SkyboxInfo>(StringComparer.Ordinal);
			soundsequences = new string[0];
			terrainnames = new string[0];
			scriptresources = new Dictionary<ScriptType, HashSet<ScriptResource>>();
			damagetypes = new string[0];
			knowncolors = new Dictionary<string, PixelColor>(StringComparer.OrdinalIgnoreCase);
			cvars = new CvarsCollection();
			ambientsounds = new Dictionary<int, AmbientSoundInfo>();

			// Load texture sets
			foreach(DefinedTextureSet ts in General.Map.ConfigSettings.TextureSets)
				texturesets.Add(new MatchingTextureSet(ts));
			
			// Sort the texture sets
			texturesets.Sort();
			
			// Special textures sets
			alltextures = new AllTextureSet();
			walltextureset = new AllTextureSet();
			flattextureset = new AllTextureSet();
			resourcetextures = new List<ResourceTextureSet>();
			
			// Go for all locations
			DataLocationList locations = DataLocationList.Combined(configlist, maplist); //mxd
			string prevofficialiwad = string.Empty; //mxd
			foreach(DataLocation dl in locations)
			{
				// Nothing chosen yet
				DataReader c = null;

				// TODO: Make this work more elegant using reflection.
				// Make DataLocation.type of type Type and assign the
				// types of the desired reader classes.

				try
				{
					// Choose container type
					switch (dl.type)
					{
						//mxd. Load resource in read-only mode if:
						// 1. UseResourcesInReadonlyMode map option is set.
						// 2. OR file has "Read only" flag set.
						// 3. OR resource has "Exclude from testing parameters" flag set.
						// 4. OR resource is official IWAD.

						// WAD file container
						case DataLocation.RESOURCE_WAD:
							c = new WADReader(dl, General.Map.Config, true);
							if (((WADReader)c).WadFile.IsOfficialIWAD) //mxd
							{
								if (!string.IsNullOrEmpty(prevofficialiwad))
									General.ErrorLogger.Add(ErrorType.Warning, "Using more than one official IWAD as a resource is not recommended. Consider removing \"" + prevofficialiwad + "\" or \"" + dl.GetDisplayName() + "\".");
								prevofficialiwad = dl.GetDisplayName();
							}
							break;

						// Directory container
						case DataLocation.RESOURCE_DIRECTORY:
							c = new DirectoryReader(dl, General.Map.Config, true);
							break;

						// PK3 file container
						case DataLocation.RESOURCE_PK3:
							c = new PK3Reader(dl, General.Map.Config, true);
							break;
					}
				}
				catch (Exception e)
				{
					// Unable to load resource
					General.ErrorLogger.Add(ErrorType.Error, "Unable to load resources from location \"" + dl.location + "\". Please make sure the location is accessible and not in use by another program. The resources will now be loaded with this location excluded. You may reload the resources to try again.\n" + e.GetType().Name + " when creating data reader: " + e.Message);
					General.WriteLogLine(e.StackTrace);
					continue;
				}

				// Add container
				if(c != null)
				{
					containers.Add(c);
					resourcetextures.Add(c.TextureSet);
				}
			}

			// Load stuff
			LoadX11R6RGB(); //mxd
			LoadPalette();
			LoadMainColorMap();
			Dictionary<string, TexturesParser> cachedparsers = new Dictionary<string, TexturesParser>(); //mxd
			int texcount = LoadTextures(texturesonly, texturenamesshorttofull, cachedparsers);
			int flatcount = LoadFlats(flatsonly, flatnamesshorttofull, cachedparsers);
			int colormapcount = LoadColormaps(colormapsonly);
			LoadSprites(cachedparsers);
			cachedparsers = null; //mxd

			//mxd. Load MAPINFO and CVARINFO. Should happen before parisng DECORATE
			Dictionary<int, string> spawnnums, doomednums;
			LoadMapInfo(out spawnnums, out doomednums);
			LoadCvarInfo();
			LoadLockDefs();

			// Load DECALDEF
			LoadDecalDefs();

			//mxd. Load Script Editor-only stuff...
			LoadExtraTextLumps();

			LoadDehackedThings();
            LoadZScriptThings();
            LoadDecorateThings();
			ApplyDehackedThings();
			FixRenamedDehackedSprites();
            int thingcount = ApplyZDoomThings(spawnnums, doomednums);
			int spritecount = LoadThingSprites();
			LoadInternalSprites();
			LoadInternalTextures(); //mxd

			//mxd. Load more stuff
			LoadReverbs();
			LoadSndSeq();
			LoadSndInfo();
			LoadVoxels();
			General.MainWindow.DisplayReady();
			
			// Process colormaps (we just put them in as textures)
			foreach(KeyValuePair<long, ImageData> t in colormapsonly)
			{
				textures.Add(t.Key, t.Value);
				texturenames.Add(t.Value.Name);
			}
			
			// Process textures
			foreach(KeyValuePair<long, ImageData> t in texturesonly)
			{
				if(!textures.ContainsKey(t.Key))
				{
					textures.Add(t.Key, t.Value);

					//mxd. Add both short and long names?
					if(t.Value.HasLongName) texturenames.Add(t.Value.ShortName);
					texturenames.Add(t.Value.Name);
				}
			}

			// Process flats
			foreach(KeyValuePair<long, ImageData> f in flatsonly) 
			{
				flats.Add(f.Key, f.Value);

				//mxd. Add both short and long names?
				if(f.Value.HasLongName) flatnames.Add(f.Value.ShortName);
				flatnames.Add(f.Value.Name);
			}

			// Mixed textures and flats?
			if(General.Map.Config.MixTexturesFlats) 
			{
				// Add textures to flats
				foreach(KeyValuePair<long, ImageData> t in texturesonly) 
				{
					if(!flats.ContainsKey(t.Key)) 
					{
						flats.Add(t.Key, t.Value);

						//mxd. Add both short and long names?
						if(t.Value.HasLongName) flatnames.Add(t.Value.ShortName);
						flatnames.Add(t.Value.Name);
					}
					else if(t.Value.TextureNamespace == TextureNamespace.TEXTURE)
					{
						//TODO: check this!
						flats[t.Key] = t.Value;
					}
				}

				//mxd
				foreach(KeyValuePair<long, long> t in texturenamesshorttofull)
					if(!flatnamesshorttofull.ContainsKey(t.Key)) flatnamesshorttofull.Add(t.Key, t.Value);

				//mxd
				flatnamesfulltoshort = flatnamesshorttofull.ToDictionary(t => t.Value, t => t.Key); //flatnamesshorttofull.ToDictionary(kp => kp.Value, kp => kp.Key);

				// Add flats to textures
				foreach(KeyValuePair<long, ImageData> f in flatsonly) 
				{
					if(!textures.ContainsKey(f.Key)) 
					{
						textures.Add(f.Key, f.Value);

						//mxd. Add both short and long names?
						if(f.Value.HasLongName && !texturenames.Contains(f.Value.ShortName)) texturenames.Add(f.Value.ShortName);
						if(!texturenames.Contains(f.Value.Name)) texturenames.Add(f.Value.Name);
					}
				}

				//mxd
				foreach(KeyValuePair<long, long> t in flatnamesshorttofull)
					if(!texturenamesshorttofull.ContainsKey(t.Key)) texturenamesshorttofull.Add(t.Key, t.Value);

				// Do the same on the data readers
				foreach(DataReader dr in containers)
					dr.TextureSet.MixTexturesAndFlats();
			}

			//mxd. Should be done after loading textures...
			int hirestexcount = LoadHiResTextures();

			//mxd. Create camera textures. Should be done after loading textures.
			LoadAnimdefs();

			//mxd
			LoadTerrain();
			
			// Sort names
			texturenames.Sort();
			flatnames.Sort();

			// biwa. Moved model processing after texture processing, since the model might need one of those textures
			Dictionary<string, int> actorsbyclass = CreateActorsByClassList();
			LoadModeldefs(actorsbyclass);
			foreach (Thing t in General.Map.Map.Things) t.UpdateCache();

			LoadGldefs(actorsbyclass);

			// Sort things
			foreach (ThingCategory tc in thingcategories) tc.SortIfNeeded();

			// Update the used textures
			General.Map.Data.UpdateUsedTextures();
			
			// Add texture names to texture sets
			foreach(KeyValuePair<long, ImageData> img in textures)
			{
				// Add to all sets
				foreach(MatchingTextureSet ms in texturesets)
					ms.AddTexture(img.Value);

				// Add to all
				alltextures.AddTexture(img.Value);
				walltextureset.AddTexture(img.Value);
			}
			
			// Add flat names to texture sets
			foreach(KeyValuePair<long, ImageData> img in flats)
			{
				// Add to all sets
				foreach(MatchingTextureSet ms in texturesets)
					ms.AddFlat(img.Value);
				
				// Add to all
				alltextures.AddFlat(img.Value);
				flattextureset.AddFlat(img.Value);
			}

			//mxd. Create skybox texture(s)
			SetupSkybox();

            // [ZZ] clear texture/flat cache in ImageSelectorPanel
            ImageSelectorPanel.ClearCachedPreviews();
			
			// Start background loading
			StartBackgroundLoader();
			
			// Output info
			General.WriteLogLine("Loaded " + texcount + " textures, " + flatcount + " flats, " + hirestexcount + " hires textures, " +
				colormapcount + " colormaps, " + spritecount + " sprites, " + 
				thingcount + " decorate things, " + modeldefentries.Count + " model/voxel definitions, " + 
				gldefsentries.Count + " dynamic light definitions, " + 
				glowingflats.Count + " glowing flat definitions, " + skyboxes.Count + " skybox definitions, " +
				reverbs.Count + " sound environment definitions");

			if (General.ErrorLogger.HasChanged)
			{
				string key = Actions.Action.GetShortcutKeyDesc(General.Actions.GetActionByName("builder_showerrors").ShortcutKey);
				string keymessage = "";

				if (!string.IsNullOrEmpty(key))
					keymessage = $" ({key})";

				if (General.ErrorLogger.IsWarningAdded)
					General.ToastManager.ShowToast("resourcewarningsanderrors", ToastType.WARNING, ToastManager.TITLE_WARNING, $"There were warnings while loading the resources. Please review the messages in the Warnings and Errors window{keymessage}.", "There were warnings while loading the resources.");
				else if(General.ErrorLogger.IsErrorAdded)
					General.ToastManager.ShowToast("resourcewarningsanderrors", ToastType.ERROR, ToastManager.TITLE_ERROR, $"There were errors while loading the resources. Please review the messages in the Warnings and Errors window{keymessage}.", "There were errors while loading the resources.");
			}
		}
		
		// This unloads all data
		private void Unload()
		{
			// Stop background loader
			StopBackgroundLoader();
			
			// Dispose decorate
			decorate.Dispose();
			
			// Dispose resources
			foreach(KeyValuePair<long, ImageData> i in textures) i.Value.Dispose();
			foreach(KeyValuePair<long, ImageData> i in flats) i.Value.Dispose();
			foreach(KeyValuePair<long, ImageData> i in sprites) i.Value.Dispose();
			palette = null;

			//mxd. Dispose models
			foreach(ModelData md in modeldefentries.Values) md.Dispose();
		
			// Dispose containers
			foreach(DataReader c in containers) c.Dispose();
			containers.Clear();
			
			// Trash collections
			decorate = null;
			containers = null;
			textures = null;
			flats = null;
			sprites = null;
			modeldefentries = null; //mxd
			gldefsentries = null; //mxd
			reverbs = null; //mxd
			glowingflats = null; //mxd
			skyboxes = null; //mxd
			soundsequences = null; //mxd
			terrainnames = null; //mxd
			scriptresources = null; //mxd
			damagetypes = null; //mxd
			knowncolors = null; //mxd
			cvars = null; //mxd
			texturenames = null;
			flatnames = null;
			imageque = null;
			mapinfo = null; //mxd
		}
		
#endregion
		
#region ================== Suspend / Resume

		// This suspends data resources
		internal void Suspend()
		{
			// Stop background loader
			StopBackgroundLoader();
			
			// Go for all containers
			foreach(DataReader d in containers)
			{
				// Suspend
				General.WriteLogLine("Suspended data resource \"" + d.Location.location + "\"");
				d.Suspend();
			}
		}

		// This resumes data resources
		internal void Resume()
		{
			// Go for all containers
			foreach(DataReader d in containers)
			{
				try
				{
					// Resume
					General.WriteLogLine("Resumed data resource \"" + d.Location.location + "\"");
					d.Resume();
				}
				catch(Exception e)
				{
					// Unable to load resource
					General.ErrorLogger.Add(ErrorType.Error, "Unable to load resources from location \"" + d.Location.location + "\". Please make sure the location is accessible and not in use by another program. The resources will now be loaded with this location excluded. You may reload the resources to try again.\n" + e.GetType().Name + " when resuming data reader: " + e.Message + ")");
					General.WriteLogLine(e.StackTrace);
				}
			}
			
			// Start background loading
			StartBackgroundLoader();
		}
		
#endregion

#region ================== Background Loading
		
		// This starts background loading
		private void StartBackgroundLoader()
		{
			// If a loader is already running, stop it first
			if(backgroundloader != null) StopBackgroundLoader();

			// Start a low priority thread to load images in background
			General.WriteLogLine("Starting background resource loading...");
            int numThreads = Math.Min(Math.Max(Environment.ProcessorCount * 3 / 4, 1), 16); // Use 75% of processors available, minimum one and maximum 16
            backgroundloader = new Thread[numThreads];
            for (int i = 0; i < numThreads; i++)
            {
                backgroundloader[i] = new Thread(BackgroundLoad);
                backgroundloader[i].Name = "Background Loader";
                backgroundloader[i].Start();
            }
		}
		
		// This stops background loading
		private void StopBackgroundLoader()
		{
			General.WriteLogLine("Stopping background resource loading...");
			if(backgroundloader != null)
			{
                // Stop the thread and wait for it to end
                foreach (Thread t in backgroundloader) t.Interrupt();
                foreach (Thread t in backgroundloader) t.Join();

				// Reset load states on all images in the list
				while(imageque.Count > 0)
				{
					ImageData img = imageque.Dequeue();
					img.ImageState = ImageLoadState.None;
					img.PreviewState = ImageLoadState.None;
				}
				
				// Done
				backgroundloader = null;
				General.MainWindow.UpdateStatus();
			}
		}
		
		// The background loader
		private void BackgroundLoad()
		{
			try
			{
                // Wait a bit before loading to give the main thread a headstart on acquiring the locks in the resource loader part of the codebase..
                Thread.Sleep(666);

				while (true)
				{
					// Get next item
					ImageData image = null;
					lock(syncobject)
					{
						// Fetch next image to process
						if(imageque.Count > 0)
                            image = imageque.Dequeue();
                        else
                            Monitor.Wait(syncobject);
                    }

					image?.BackgroundLoadImage();
                }
			}
			catch(ThreadInterruptedException) { }
		}
		
		internal void QueueLoadImage(ImageData img)
		{
			if(img.ImageState == ImageLoadState.None)
			{
				img.ImageState = ImageLoadState.Loading;
                img.PreviewState = ImageLoadState.Loading;
                lock (syncobject)
                {
                    imageque.Enqueue(img);
                    Monitor.Pulse(syncobject);
                }
			}
		}

        internal void QueueLoadPreview(ImageData img)
        {
            if (img.PreviewState == ImageLoadState.None)
            {
                img.PreviewState = ImageLoadState.Loading;
                lock (syncobject)
                {
                    imageque.Enqueue(img);
                    Monitor.Pulse(syncobject);
                }
            }
        }

        //mxd. This loads a model
        internal bool ProcessModel(int type) 
		{
			if(modeldefentries[type].LoadState != ModelLoadState.None) return true;

			//create models
			ModelLoader.Load(modeldefentries[type], containers);

			if(modeldefentries[type].Model != null) 
			{
				modeldefentries[type].LoadState = ModelLoadState.Ready;
				return true;
			}

			modeldefentries.Remove(type);
			return false;
		}

#endregion

#region ================== Palette

        // This loads the PLAYPAL palette
        private void LoadPalette()
		{
			// Go for all opened containers
			for(int i = containers.Count - 1; i >= 0; i--)
			{
				// Load palette
				palette = containers[i].LoadPalette();
				if(palette != null) break;
			}

			// Make empty palette when still no palette found
			if(palette == null)
			{
				General.ErrorLogger.Add(ErrorType.Warning, "None of the loaded resources define a color palette. Did you forget to configure an IWAD for this game configuration?");
				palette = new Playpal();
			}
		}

        private void LoadMainColorMap() 
        {
	        // Go for all opened containers
	        for(int i = containers.Count - 1; i >= 0; i--)
	        {
		        // Load palette
		        mainColormap = containers[i].LoadMainColorMap(palette);
		        if(mainColormap != null) break;
	        }

	        // Make empty palette when still no palette found
	        if(mainColormap == null)
	        {
		        General.ErrorLogger.Add(ErrorType.Warning, "None of the loaded resources define a colormap. Did you forget to configure an IWAD for this game configuration?");
		        mainColormap = new ColorMap();
	        }
        }

#endregion

#region ================== Colormaps

		// This loads the colormaps
		private int LoadColormaps(Dictionary<long, ImageData> list)
		{
			int counter = 0;

			// Go for all opened containers
			foreach(DataReader dr in containers)
			{
				// Load colormaps
				ICollection<ImageData> images = dr.LoadColormaps();
				if(images != null)
				{
					// Go for all colormaps
					foreach(ImageData img in images)
					{
						// Add or replace in flats list
						list.Remove(img.LongName);
						list.Add(img.LongName, img);
						counter++;
					}
				}
			}

			// Output info
			return counter;
		}

		// This returns a specific colormap stream
		internal Stream GetColormapData(string pname)
		{
			// Go for all opened containers
			for(int i = containers.Count - 1; i >= 0; i--)
			{
				// This contain provides this flat?
				Stream colormap = containers[i].GetColormapData(pname);
				if(colormap != null) return colormap;
			}

			// No such patch found
			return null;
		}

#endregion

#region ================== Textures

		// This loads the textures
		private int LoadTextures(Dictionary<long, ImageData> list, Dictionary<long, long> nametranslation, Dictionary<string, TexturesParser> cachedparsers)
		{
			PatchNames pnames = new PatchNames();
			int counter = 0;

			// Go for all opened containers
			foreach(DataReader dr in containers)
			{
				// Load PNAMES info
				// Note that pnames is NOT set to null in the loop
				// because if a container has no pnames, the pnames
				// of the previous (higher) container should be used.
				PatchNames newpnames = dr.LoadPatchNames();
				if(newpnames != null) pnames = newpnames;

				// Load textures
				IEnumerable<ImageData> images = dr.LoadTextures(pnames, cachedparsers);
				if(images != null)
				{
					// Go for all textures
					foreach(ImageData img in images)
					{
						// Add or replace in textures list
						list[img.LongName] = img;
						counter++;

						//mxd. Also add as short name when texture name is longer than 8 chars
						// Or remove when a wad image with short name overrides previously added 
						// resource image with long name
						if(img.HasLongName) 
						{
							long longshortname = Lump.MakeLongName(Path.GetFileNameWithoutExtension(img.Name), false);
							nametranslation[longshortname] = img.LongName;
						} 
						else if(img is TextureImage && nametranslation.ContainsKey(img.LongName))
						{
							nametranslation.Remove(img.LongName);
						}
                    }
				}
			}
			
			// Output info
			return counter;
		}
		
		// This returns a specific patch stream
		internal Stream GetPatchData(string pname, bool longname, ref string patchlocation)
		{
			// Go for all opened containers
			for(int i = containers.Count - 1; i > -1; i--)
			{
				// This contain provides this patch?
				Stream patch = containers[i].GetPatchData(pname, longname, ref patchlocation);
				if(patch != null) return patch;
			}

			// No such patch found
			return null;
		}

		// This returns a specific texture stream
		internal Stream GetTextureData(string pname, bool longname, ref string texturelocation)
		{
			// Go for all opened containers
			for(int i = containers.Count - 1; i >= 0; i--)
			{
				// This container provides this patch?
				Stream patch = containers[i].GetTextureData(pname, longname, ref texturelocation);
				if(patch != null) return patch;
			}

			// No such patch found
			return null;
		}
		
		// This checks if a given texture is known
		public bool GetTextureExists(string name)
		{
			return GetTextureExists(Lump.MakeLongName(name)); //mxd
		}
		
		// This checks if a given texture is known
		public bool GetTextureExists(long longname)
		{
			return textures.ContainsKey(longname) || texturenamesshorttofull.ContainsKey(longname);
		}
		
		// This returns an image by string
		public ImageData GetTextureImage(string name)
		{
			// Get the long name
			long longname = Lump.MakeLongName(name);
			return GetTextureImage(longname);
		}
		
		// This returns an image by long
		public ImageData GetTextureImage(long longname)
		{
			// Does this texture exist?
			if (textures.ContainsKey(longname))
				return textures[longname];
			if (texturenamesshorttofull.ContainsKey(longname)) return textures[texturenamesshorttofull[longname]]; //mxd
			if (textures.ContainsKey(longname)) return textures[longname];

			// Return null image
			return unknownimage; //mxd
		}

		//mxd. This tries to find and load any image with given name
		internal Bitmap GetTextureBitmap(string name) { Vector2D unused = new Vector2D(); return GetTextureBitmap(name, out unused); }
		internal Bitmap GetTextureBitmap(string name, out Vector2D scale)
		{
			// Check the textures first...
			ImageData img = GetTextureImage(name);
			if(img is UnknownImage && !General.Map.Config.MixTexturesFlats)
				img = GetFlatImage(name);

			if(!(img is UnknownImage))
			{
				img.LoadImageNow();
				if(!img.LoadFailed)
				{
					// HiResImage will not give us it's actual scale
					Bitmap texture = img.GetSkyboxBitmap();
                    lock (texture)
                    {
                        scale = new Vector2D((float)img.Width / texture.Width, (float)img.Height / texture.Height);
                    }
					return texture;
				}
			}
			
			// Try to find any image...
			scale = new Vector2D(1.0f, 1.0f);
			for(int i = containers.Count - 1; i >= 0; i--)
			{
				// This container has a lump with given name?
				if(containers[i] is PK3StructuredReader)
				{
					string foundname = ((PK3StructuredReader)containers[i]).FindFirstFile(name, true);
					if(string.IsNullOrEmpty(foundname)) continue;
					name = foundname;
				}
				else if(!(containers[i] is WADReader))
				{
					throw new NotImplementedException("Unsupported container type!");
				}

				if(!containers[i].FileExists(name)) continue;
				MemoryStream mem = containers[i].LoadFile(name);
				if(mem == null) continue;

				// Is it an image?
				Bitmap bitmap = ImageDataFormat.TryLoadImage(mem, ImageDataFormat.DOOMPICTURE, General.Map.Data.Palette);
				if(bitmap != null) return bitmap;
			}

			// No such image found
			return null;
		}

		//mxd
		public string GetFullTextureName(string name)
		{
			if(string.IsNullOrEmpty(name)) return name;
			if(Path.GetFileNameWithoutExtension(name) == name && name.Length > CLASIC_IMAGE_NAME_LENGTH) 
				name = name.Substring(0, CLASIC_IMAGE_NAME_LENGTH);
			long hash = MurmurHash2.Hash(name.Trim().ToUpperInvariant());

			if(textures.ContainsKey(hash) && (textures[hash] is TEXTURESImage || textures[hash] is HiResImage))
				return textures[hash].Name; //TEXTURES and HiRes textures should still override regular ones...
			if(texturenamesshorttofull.ContainsKey(hash)) return textures[texturenamesshorttofull[hash]].Name;
			if(textures.ContainsKey(hash)) return textures[hash].Name;
			return name;
		}

		//mxd
		internal long GetFullLongTextureName(long hash)
		{
			if(textures.ContainsKey(hash) && (textures[hash] is TEXTURESImage || textures[hash] is HiResImage))
				return hash; //TEXTURES and HiRes textures should still override regular ones...
			return (General.Map.Config.UseLongTextureNames && texturenamesshorttofull.ContainsKey(hash) ? texturenamesshorttofull[hash] : hash);
		}

		//mxd
		private void LoadInternalTextures()
		{
			missingtexture3d = LoadInternalTexture("MissingTexture3D.png");
			unknowntexture3d = LoadInternalTexture("UnknownTexture3D.png");
			thingtexture = LoadInternalTexture("ThingTexture2D.png");
			hourglass3d = LoadInternalTexture("Hourglass3D.png");
			crosshair = LoadInternalTexture("Crosshair.png");
			crosshairbusy = LoadInternalTexture("CrosshairBusy.png");

			thingtexture.UseColorCorrection = false;
		}

		//mxd
		private static ImageData LoadInternalTexture(string name)
		{
			ImageData result;
			string path = Path.Combine(General.TexturesPath, name);
			if(File.Exists(path))
			{
				result = new FileImage(name, path) { AllowUnload = false };
                result.LoadImageNow();
            }
            else
			{
				General.ErrorLogger.Add(ErrorType.Warning, "Unable to load editor texture \"" + name + "\". Using built-in one instead.");
				result = new ResourceImage("CodeImp.DoomBuilder.Resources." + name);
			}

			return result;
		}
		
#endregion

#region ================== Flats

		// This loads the flats
		private int LoadFlats(Dictionary<long, ImageData> list, Dictionary<long, long> nametranslation, Dictionary<string, TexturesParser> cachedparsers)
		{
			int counter = 0;
			
			// Go for all opened containers
			foreach(DataReader dr in containers)
			{
				// Load flats
				IEnumerable<ImageData> images = dr.LoadFlats(cachedparsers);
				if(images != null)
				{
					// Go for all flats
					foreach(ImageData img in images)
					{
						// Add or replace in flats list
						list[img.LongName] = img; //mxd
						counter++;

						//mxd. Also add as short name when texture name is longer than 8 chars
						// Or remove when a wad image with short name overrides previously added 
						// resource image with long name
						if(img.HasLongName)
						{
							long longshortname = Lump.MakeLongName(Path.GetFileNameWithoutExtension(img.Name), false);
							nametranslation[longshortname] = img.LongName;
						} 
						else if(img is FlatImage && nametranslation.ContainsKey(img.LongName)) 
						{
							nametranslation.Remove(img.LongName);
						}
                    }
				}
			}

			// Output info
			return counter;
		}

		// This returns a specific flat stream
		internal Stream GetFlatData(string pname, bool longname, ref string flatlocation)
		{
			// Go for all opened containers
			for(int i = containers.Count - 1; i >= 0; i--)
			{
				// This contain provides this flat?
				Stream flat = containers[i].GetFlatData(pname, longname, ref flatlocation);
				if(flat != null) return flat;
			}

			// No such patch found
			return null;
		}

		// This checks if a flat is known
		public bool GetFlatExists(string name)
		{
			return GetFlatExists(Lump.MakeLongName(name)); //mxd
		}

        // This checks if a flat is known
        public bool GetFlatExists(long longname)
        {
            if (flats.ContainsKey(longname))
                return true;

            return flatnamesshorttofull.ContainsKey(longname);
        }
		
		// This returns an image by string
		public ImageData GetFlatImage(string name)
		{
			// Get the long name
			long longname = Lump.MakeLongName(name);
			return GetFlatImage(longname);
		}

		// This returns an image by long
		public ImageData GetFlatImage(long longname)
		{
			// Does this flat exist?
			if (flats.ContainsKey(longname) && (flats[longname] is TEXTURESImage || flats[longname] is HiResImage))
				return flats[longname]; //TEXTURES and HiRes flats should still override regular ones...
			if (flatnamesshorttofull.ContainsKey(longname))
				return flats[flatnamesshorttofull[longname]]; //mxd
			if (flats.ContainsKey(longname))
				return flats[longname];

			// Return null image
			return unknownimage; //mxd
		}

		// This returns an image by long and doesn't check if it exists
		/*public ImageData GetFlatImageKnown(long longname)
		{
			// Return flat
			return flatnamesshorttofull.ContainsKey(longname) ? flats[flatnamesshorttofull[longname]] : flats[longname]; //mxd
		}*/

		//mxd. Gets full flat name by short flat name
		public string GetFullFlatName(string name)
		{
			if(Path.GetFileNameWithoutExtension(name) == name && name.Length > CLASIC_IMAGE_NAME_LENGTH)
				name = name.Substring(0, CLASIC_IMAGE_NAME_LENGTH);
			long hash = MurmurHash2.Hash(name.ToUpperInvariant());

			if(flats.ContainsKey(hash) && (flats[hash] is TEXTURESImage || flats[hash] is HiResImage))
				return flats[hash].Name; //TEXTURES and HiRes flats should still override regular ones...
			if(flatnamesshorttofull.ContainsKey(hash)) return flats[flatnamesshorttofull[hash]].Name;
			if(flats.ContainsKey(hash)) return flats[hash].Name;
			return name;
		}

		//mxd
		internal long GetFullLongFlatName(long hash)
		{
			if(flats.ContainsKey(hash) && (flats[hash] is TEXTURESImage || flats[hash] is HiResImage))
				return hash; //TEXTURES and HiRes flats should still override regular ones...
			return (General.Map.Config.UseLongTextureNames && flatnamesshorttofull.ContainsKey(hash) ? flatnamesshorttofull[hash] : hash);
		}

		//mxd
		internal long GetShortLongFlatName(long hash)
		{
			return (flatnamesfulltoshort.ContainsKey(hash) ? flatnamesfulltoshort[hash] : hash);
		}
		
#endregion

#region ================== mxd. HiRes textures

		// This loads the textures
		private int LoadHiResTextures()
		{
			int counter = 0;

			// Go for all opened containers
			foreach(DataReader dr in containers)
			{
				//mxd. Load HiRes texures
				IEnumerable<HiResImage> hiresimages = dr.LoadHiResTextures();
				if(hiresimages != null)
				{
					// Go for all textures
					foreach(HiResImage img in hiresimages)
					{
						// Replace when HiRes image name exactly matches a regular texture name, 
						// or when regular texture filename is 8 or less chars long
						//bool replaced = false;
						
						// Replace texture?
						long hash = GetFullLongTextureName(img.LongName);
						if(textures.ContainsKey(hash) && (hash == img.LongName || Path.GetFileNameWithoutExtension(textures[hash].Name).Length <= CLASIC_IMAGE_NAME_LENGTH))
						{
							HiResImage replacer = new HiResImage(img);
							replacer.ApplySettings(textures[hash]);
							textures[img.LongName] = replacer;
                            //replaced = true;

                            counter++;
						}

						// Replace flat?
						hash = GetFullLongFlatName(img.LongName);
						if(flats.ContainsKey(hash) && (hash == img.LongName || Path.GetFileNameWithoutExtension(flats[hash].Name).Length <= CLASIC_IMAGE_NAME_LENGTH))
						{
							HiResImage replacer = new HiResImage(img);
							replacer.ApplySettings(flats[hash]);
							flats[img.LongName] = replacer;
                            //replaced = true;

                            counter++;
						}

						// Replace sprite?
						if(sprites.ContainsKey(img.LongName))
						{
							HiResImage replacer = new HiResImage(img);
							replacer.ApplySettings(sprites[img.LongName]);
							sprites[img.LongName] = replacer;
                            //replaced = true;

                            counter++;
						}

						// We don't load any graphics and most of the sprites, so this can result in a ton of false warnings...
						/*if(!replaced)
						{
							General.ErrorLogger.Add(ErrorType.Warning, "HiRes texture \"" + Path.Combine(dr.Location.GetDisplayName(), img.FilePathName.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar)) + "\" does not override any existing texture or flat.");
							dr.TextureSet.AddTexture(img);

							// Add to textures and flats
							textures[img.LongName] = img;
							flats[img.LongName] = img;

							// Add to preview manager
							previews.AddImage(img);
						}

						counter++;*/
					}
				}
			}

			// Output info
			return counter;
		}

		//mxd. This returns a specific HiRes texture stream
		internal Stream GetHiResTextureData(string name, ref string hireslocation)
		{
			// Go for all opened containers
			for(int i = containers.Count - 1; i >= 0; i--)
			{
				// This container provides this texture?
				Stream data = containers[i].GetHiResTextureData(name, ref hireslocation);
				if(data != null) return data;
			}

			// No such patch texture
			return null;
		}

#endregion

#region ================== Sprites

		// This loads the hard defined sprites (not all the lumps, we do that on a need-to-know basis, see LoadThingSprites)
		private int LoadSprites(Dictionary<string, TexturesParser> cachedparsers)
		{
			int counter = 0;
			
			// Load all defined sprites. Note that we do not use all sprites,
			// so we don't add them for previews just yet.
			foreach(DataReader dr in containers)
			{
				// Load sprites
				IEnumerable<ImageData> images = dr.LoadSprites(cachedparsers);
				if(images != null)
				{
					// Add or replace in sprites list
					foreach(ImageData img in images)
					{
						sprites[img.LongName] = img;
						counter++;
					}
				}
			}
			
			// Output info
			return counter;
		}
		
		// This loads the sprites that we really need for things
		private int LoadThingSprites()
		{
			//mxd. Get all sprite names
			HashSet<string> spritenames = new HashSet<string>(StringComparer.Ordinal);
            // [ZZ] in order to properly replace different rotation count, we need more complex processing here.
            HashSet<string> loadedspritenames = new HashSet<string>(StringComparer.Ordinal);
            for (int i = containers.Count-1; i >= 0; i--)
            {
                IEnumerable<string> result = containers[i].GetSpriteNames();
                if (result != null)
                {
                    // remove old sprites with this name
                    result = result.Where(str => !loadedspritenames.Contains(str.Substring(0, 4))); // only sprites that we still don't have. remember, reverse iteration!
                    // add new sprites with this name
                    spritenames.UnionWith(result);
                    // remember
                    foreach (string spr in result)
                        loadedspritenames.Add(spr.Substring(0, 4));
                }
            }

			//mxd. Add sprites from sprites collection (because GetSpriteNames() doesn't return TEXTURES sprites)
			foreach(ImageData data in sprites.Values)
				if(WADReader.IsValidSpriteName(data.Name))
					spritenames.Add(data.Name);
			
			//mxd. Get names of all voxel models, which can be used "as is" (these do not require corresponding sprite to work)
			HashSet<string> voxelnames = new HashSet<string>(StringComparer.Ordinal);
			foreach(DataReader dr in containers)
			{
				IEnumerable<string> result = dr.GetVoxelNames();
				if(result != null) voxelnames.UnionWith(result);
			}
			
			// Go for all things
			foreach(ThingTypeInfo ti in General.Map.Data.ThingTypes)
			{
				// Valid sprite name?
				if(ti.Sprite.Length == 0 || ti.Sprite.Length > CLASIC_IMAGE_NAME_LENGTH) continue; //mxd
					
				//mxd. Find all sprite angles
				bool isvoxel = ti.SetupSpriteFrame(spritenames, voxelnames);

				//mxd. Create voxel sprite?
				if(isvoxel)
				{
					if(!sprites.ContainsKey(Lump.MakeLongName(ti.Sprite)))
					{
						// Make new voxel image
						VoxelImage image = new VoxelImage(ti.Sprite, ti.Sprite);

						// Add to collection
						sprites.Add(image.LongName, image);
                    }
				}
				else
				{
					//mxd. Load all sprites
					foreach(SpriteFrameInfo info in ti.SpriteFrame)
					{
						ImageData image = null;

						// Sprite not in our collection yet?
						if(!sprites.ContainsKey(info.SpriteLongName))
						{
							//mxd. Go for all opened containers
							bool spritefound = false;
							if(!string.IsNullOrEmpty(info.Sprite))
							{
								for(int i = containers.Count - 1; i >= 0; i--)
								{
									// This container provides this sprite?
									if(containers[i].GetSpriteExists(info.Sprite))
									{
										spritefound = true;
										break;
									}
								}
							}

							if(spritefound)
							{
								// Make new sprite image
								image = new SpriteImage(info.Sprite);

								// Add to collection
								sprites.Add(info.SpriteLongName, image);
							}
							else
							{
                                if (!ti.Optional)
								    General.ErrorLogger.Add(ErrorType.Error, "Unable to find sprite lump \"" + info.Sprite + "\" used by actor \"" + ti.Title + "\":" + ti.Index + ". Forgot to include required resources?");
							}
						}
						else
						{
							image = sprites[info.SpriteLongName];
						}
                    }
				}
			}
			
			// Output info
			return sprites.Count;
		}
		
		// This returns a specific sprite stream
		internal Stream GetSpriteData(string pname, ref string spritelocation)
		{
			if(!string.IsNullOrEmpty(pname))
			{
				// Go for all opened containers
				for(int i = containers.Count - 1; i >= 0; i--)
				{
					// This container provides this sprite?
					Stream spritedata = containers[i].GetSpriteData(pname, ref spritelocation);
					if(spritedata != null)
                        return spritedata;
				}
			}
			
			// No such sprite found
			return null;
		}

		// This tests if a given sprite can be found
		internal bool GetSpriteExists(string pname)
		{
			if(!string.IsNullOrEmpty(pname))
			{
				long longname = Lump.MakeLongName(pname);
				if(sprites.ContainsKey(longname)) return true;
				
				// Go for all opened containers
				for(int i = containers.Count - 1; i >= 0; i--)
				{
					// This contain provides this sprite?
					if(containers[i].GetSpriteExists(pname)) return true;
				}
			}
			
			// No such sprite found
			return false;
		}
		
		// This loads the internal sprites
		private void LoadInternalSprites()
		{
			// Add sprite icon files from directory
			string name;
			string[] files = Directory.GetFiles(General.SpritesPath, "*.png", SearchOption.TopDirectoryOnly);
			internalspriteslookup = new Dictionary<string, long>(files.Length + 2); //mxd
			foreach(string spritefile in files)
			{
				ImageData img = new FileImage(Path.GetFileNameWithoutExtension(spritefile).ToLowerInvariant(), spritefile);
				img.LoadImageNow();
				img.AllowUnload = false;
				name = INTERNAL_PREFIX + img.Name;
				long hash = Lump.MakeLongName(name, true); //mxd
				sprites[hash] = img; //mxd
				internalspriteslookup[name] = hash; //mxd
			}
			
			// Add some internal resources.
			// mxd. Doesn't seem to be used anywhere
			/*name = INTERNAL_PREFIX + "nothing";
			if(!internalspriteslookup.ContainsKey(name))
			{
				ImageData img = new ResourceImage("CodeImp.DoomBuilder.Resources.Nothing.png");
				img.AllowUnload = false;
				long hash = Lump.MakeLongName(name, true); //mxd
				sprites[hash] = img; //mxd
				internalspriteslookup[name] = hash; //mxd
			}*/

			name = INTERNAL_PREFIX + "unknownthing";
			UNKNOWN_THING = Lump.MakeLongName(name, true);
			if(!internalspriteslookup.ContainsKey(name))
			{
				ImageData img = new ResourceImage("CodeImp.DoomBuilder.Resources.UnknownThing.png");
				img.AllowUnload = false;
				sprites[UNKNOWN_THING] = img; //mxd
				internalspriteslookup[name] = UNKNOWN_THING; //mxd
			}

			//mxd
			name = INTERNAL_PREFIX + "missingthing";
			MISSING_THING = Lump.MakeLongName(name, true);
			if(!internalspriteslookup.ContainsKey(name)) 
			{
				ImageData img = new ResourceImage("CodeImp.DoomBuilder.Resources.MissingThing.png");
				img.AllowUnload = false;
				sprites[MISSING_THING] = img; //mxd
				internalspriteslookup[name] = MISSING_THING; //mxd
			}
		}
		
		// This returns an image by name
		public ImageData GetSpriteImage(string name)
		{
			// Is this referring to an internal sprite image?
			if((name.Length > INTERNAL_PREFIX.Length) && name.ToLowerInvariant().StartsWith(INTERNAL_PREFIX))
			{
				// Get the internal sprite
				string internalname = name.ToLowerInvariant();
				if(internalspriteslookup.ContainsKey(internalname)) //mxd
					return sprites[internalspriteslookup[internalname]];

				ImageData img = sprites[UNKNOWN_THING]; //mxd
                if (img != null)
                {
                    img.UsedInMap = true;
                }
                return img;
			}
			else
			{
				// Get the long name
				long longname = Lump.MakeLongName(name);

				// Sprite already loaded?
				if(sprites.ContainsKey(longname))
				{
					// Return exiting sprite
					ImageData img = sprites[longname];
                    if (img != null)
                    {
                        img.UsedInMap = true;
                    }
                    return img;
                }
                else
				{
					//mxd. Go for all opened containers
					bool spritefound = false;
					if(!string.IsNullOrEmpty(name))
					{
						for(int i = containers.Count - 1; i >= 0; i--)
						{
							// This contain provides this sprite?
							if(containers[i].GetSpriteExists(name))
							{
								spritefound = true;
								break;
							}
						}
					}
					
					// Found anything?
					if(spritefound)
					{
						// Make new sprite image
						SpriteImage image = new SpriteImage(name);

						// Add to collection
						sprites.Add(longname, image);

                        // Return result
                        if (image != null)
                        {
                            image.UsedInMap = true;
                        }
                        return image;
					}
					else //mxd
					{
						ImageData img = string.IsNullOrEmpty(name) ? sprites[UNKNOWN_THING] : sprites[MISSING_THING];
						
						// Add to collection
						sprites.Add(longname, img);

                        // Return image
                        if (img != null)
                        {
                            img.UsedInMap = true;
                        }
                        return img; 
					}
				}
			}
		}

		//mxd. Returns all sprite names, which start with given string
		internal IEnumerable<string> GetSpriteNames()
		{
			HashSet<string> result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

			foreach(DataReader reader in containers)
				result.UnionWith(reader.GetSpriteNames());

			return result;
		}
		
#endregion

#region ================== mxd. Voxels

		// This returns a specific voxel stream
		internal Stream GetVoxelData(string pname, ref string voxellocation)
		{
			if(!string.IsNullOrEmpty(pname))
			{
				// Go for all opened containers
				for(int i = containers.Count - 1; i >= 0; i--)
				{
					// This container provides this sprite?
					Stream spritedata = containers[i].GetVoxelData(pname, ref voxellocation);
					if(spritedata != null) return spritedata;
				}
			}

			// No such voxel found
			return null;
		}

#endregion

#region ================== Things
		
        private void LoadZScriptThings()
        {
            // Create new parser
            zscript = new ZScriptParser { OnInclude = LoadZScriptFromLocation };

            // Only load these when the game configuration supports the use of decorate
            if (!string.IsNullOrEmpty(General.Map.Config.DecorateGames))
            {
                // Go for all opened containers
                foreach (DataReader dr in containers)
                {
					// Load Decorate info cumulatively (the last Decorate is added to the previous)
					// I'm not sure if this is the right thing to do though.
					Stopwatch t = new Stopwatch();
					t.Start();

					currentreader = dr;
                    foreach (TextResourceData data in dr.GetZScriptData("ZSCRIPT"))
                    {
                        // Parse the data
                        data.Stream.Seek(0, SeekOrigin.Begin);
                        zscript.Parse(data, true);

                        //mxd. DECORATE lumps are interdepandable. Can't carry on...
                        if (zscript.HasError)
                        {
                            zscript.LogError();
                            break;
                        }
					}

					DebugConsole.WriteLine(string.Format("Loading ZScript lumps from {0} took {1}ms", dr.Location.location, t.ElapsedMilliseconds));
                }

				Stopwatch t2 = new Stopwatch();
				t2.Start();
                zscript.Finalize();
				DebugConsole.WriteLine(string.Format("Finalizing ZScript lumps took {0}ms", t2.ElapsedMilliseconds));
				if (zscript.HasError)
                    zscript.LogError();

                //mxd. Add to text resources collection
                scriptresources[zscript.ScriptType] = new HashSet<ScriptResource>(zscript.ScriptResources.Values);
                currentreader = null;
            }

            if (zscript.HasError)
                zscript.ClearActors();
        }

		// This loads the things from Decorate
		private void LoadDecorateThings()
		{
			// Create new parser
			decorate = new DecorateParser(zscript.AllActorsByClass) { OnInclude = LoadDecorateFromLocation };

			// Only load these when the game configuration supports the use of decorate
			if(!string.IsNullOrEmpty(General.Map.Config.DecorateGames))
			{
				// Go for all opened containers
				foreach(DataReader dr in containers)
				{
					// Load Decorate info cumulatively (the last Decorate is added to the previous)
					// I'm not sure if this is the right thing to do though.
					currentreader = dr;
					foreach(TextResourceData data in dr.GetDecorateData("DECORATE"))
					{
						// Parse the data
						data.Stream.Seek(0, SeekOrigin.Begin);
						decorate.Parse(data, true);

						//mxd. DECORATE lumps are interdepandable. Can't carry on...
						if (decorate.HasError)
						{
							decorate.LogError();
							break;
						}
					}
				}

				//mxd. Add to text resources collection
				scriptresources[decorate.ScriptType] = new HashSet<ScriptResource>(decorate.ScriptResources.Values);
				currentreader = null;
			}

            if (decorate.HasError)
                decorate.ClearActors();
		}

		/// <summary>
		/// Loads Dehacked things
		/// </summary>
		private void LoadDehackedThings()
		{
			// Create new parser
			dehacked = new DehackedParser();

			HashSet<string> availablesprites = new HashSet<string>();

			foreach(DataReader dr in containers)
			{
				availablesprites.UnionWith(dr.GetSpriteNames());
			}

			foreach(DataReader dr in containers)
			{
				List<TextResourceData> dehackedstreams = new List<TextResourceData>(dr.GetDehackedData());

				foreach(TextResourceData trd in dehackedstreams)
				{
					trd.Stream.Seek(0, SeekOrigin.Begin);
					dehacked.Parse(trd, General.Map.Config.DehackedData, availablesprites);
				}
			}
		}

        // [ZZ] this retrieves ZDoom actor structure by class name.
        public ActorStructure GetZDoomActor(string classname)
        {
            classname = classname.ToLowerInvariant();
            ActorStructure outv;
            if (!zdoomclasses.TryGetValue(classname, out outv))
                return null;
            return outv;
        }

        // [ZZ] this merges in the parsed actor lists from zscript+decorate using the original DECORATE merging code
        private int ApplyZDoomThings(Dictionary<int, string> spawnnumsoverride, Dictionary<int, string> doomednumsoverride)
        {
            int counter = 0;

            /////////////// ====================== DAMAGETYPES
            //mxd. Create DamageTypes list
            // Combine damage types from config and decorate
            HashSet<string> dtset = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            dtset.UnionWith(General.Map.Config.DamageTypes);
            if (!decorate.HasError) dtset.UnionWith(decorate.DamageTypes);

            // Sort values
            List<string> dtypes = new List<string>(dtset);
            dtypes.Sort();

            // Apply to collection
            damagetypes = new string[dtypes.Count];
            dtypes.CopyTo(damagetypes);
            /////////////// ====================== /DAMAGETYPES

            // Step 0. Create ThingTypeInfo by classname collection...
            Dictionary<string, ThingTypeInfo> thingtypesbyclass = new Dictionary<string, ThingTypeInfo>(thingtypes.Count, StringComparer.OrdinalIgnoreCase);
            foreach (ThingTypeInfo info in thingtypes.Values)
                if (!string.IsNullOrEmpty(info.ClassName)) thingtypesbyclass[info.ClassName] = info;

            // [ZZ] create combined array of actors to process.
            IEnumerable<ActorStructure> mergedActors = zscript.Actors.Union(decorate.Actors);
            IEnumerable<ActorStructure> mergedAllActors = zscript.AllActors.Union(decorate.AllActors);
            Dictionary<string, ActorStructure> mergedActorsByClass = decorate.ActorsByClass.Concat(zscript.ActorsByClass.Where(x => !decorate.ActorsByClass.ContainsKey(x.Key))).ToDictionary(k => k.Key, v => v.Value);
            Dictionary<string, ActorStructure> mergedAllActorsByClass = decorate.AllActorsByClass.Concat(zscript.AllActorsByClass.Where(x => !decorate.AllActorsByClass.ContainsKey(x.Key))).ToDictionary(k => k.Key, v => v.Value);
            zdoomclasses = mergedAllActorsByClass;

			// Dictionary of replaced actors that have to be recategorized
			Dictionary<int, ActorStructure> recategorizeactors = new Dictionary<int, ActorStructure>();

            // Step 1. Go for all actors in the decorate to make things or update things
            foreach (ActorStructure actor in mergedActors)
            {
                //mxd. Apply "replaces" DECORATE override...
                if (!string.IsNullOrEmpty(actor.ReplacesClass) && thingtypesbyclass.ContainsKey(actor.ReplacesClass))
                {
					// Update info
					thingtypesbyclass[actor.ReplacesClass].ModifyByDecorateActor(actor);

					// A replaced actor might have to go to another category. Only store the last one in case the same actor is replaced multiple times
					if (actor.HasPropertyWithValue("$category"))
						recategorizeactors[thingtypesbyclass[actor.ReplacesClass].Index] = actor;
					else
						recategorizeactors.Remove(thingtypesbyclass[actor.ReplacesClass].Index);

					// Count
					counter++;
                }

                // Check if we want to add this actor
                if (actor.DoomEdNum > 0)
                {
                    // Check if we can find this thing in our existing collection
                    if (thingtypes.ContainsKey(actor.DoomEdNum))
                    {
                        // Update the thing
                        thingtypes[actor.DoomEdNum].ModifyByDecorateActor(actor);
                    }
                    else
                    {
                        // Find the category to put the actor in
                        ThingCategory cat = GetThingCategory(null, thingcategories, GetCategoryInfo(actor)); //mxd

                        // Add new thing
                        ThingTypeInfo t;

						// If the thing inherits from another actor use the base actor's thing type info, otherwise create a new one
						// This makes sure that inherited actors get all properties like the icon color
						if (!string.IsNullOrEmpty(actor.InheritsClass) && thingtypesbyclass.ContainsKey(actor.InheritsClass))
							t = new ThingTypeInfo(cat, actor, thingtypesbyclass[actor.InheritsClass]);
						else
							t = new ThingTypeInfo(cat, actor);

						cat.AddThing(t);
                        thingtypes.Add(t.Index, t);
                    }

                    // Count
                    counter++;
                }
            }

			// Step 1.1. Recategorize actors that replace other actors
			foreach(KeyValuePair<int, ActorStructure> kvp in recategorizeactors)
			{
				int i = kvp.Key;
				ActorStructure actor = kvp.Value;

				// Remove the thing from its old thing category
				thingtypes[i].Category.RemoveThing(thingtypes[i]);

				// Get the new thing category
				ThingCategory tc = GetThingCategory(null, thingcategories, GetCategoryInfo(actor));

				// Remove the existing ThingTypeInfo and create a new one (with the new DoomEdNum)
				thingtypes.Remove(thingtypesbyclass[actor.ReplacesClass].Index);
				thingtypes[i] = new ThingTypeInfo(i, thingtypesbyclass[actor.ReplacesClass]);

				// Re-add the ThingTypeInfo to the ThingCategory
				tc.AddThing(thingtypes[i]);
			}

            //mxd. Step 2. Apply DoomEdNum MAPINFO overrides, remove actors disabled in MAPINFO
            if (doomednumsoverride.Count > 0)
            {
                List<int> toremove = new List<int>();
                foreach (KeyValuePair<int, string> group in doomednumsoverride)
                {
                    // Remove thing from the list?
                    if (group.Value == "none")
                    {
                        toremove.Add(group.Key);
                        continue;
                    }

                    // Skip if already added.
                    if (thingtypes.ContainsKey(group.Key) && thingtypes[group.Key].ClassName.ToLowerInvariant() == group.Value)
                    {
                        continue;
                    }

                    // Try to find the actor...
                    ActorStructure actor = null;

                    //... in ActorsByClass
                    if (mergedActorsByClass.ContainsKey(group.Value))
                    {
                        actor = mergedActorsByClass[group.Value];
                    }
                    // Try finding in ArchivedActors
                    else if (mergedAllActorsByClass.ContainsKey(group.Value))
                    {
                        actor = mergedAllActorsByClass[group.Value];
                    }

                    if (actor != null)
                    {
                        // Find the category to put the actor in
                        ThingCategory cat = GetThingCategory(null, thingcategories, GetCategoryInfo(actor)); //mxd

                        // Add a new ThingTypeInfo, replacing already existing one if necessary
                        ThingTypeInfo info = new ThingTypeInfo(cat, actor, group.Key);
                        thingtypes[group.Key] = info;
                        cat.AddThing(info);
                    }
                    // Check thingtypes...
                    else if (thingtypesbyclass.ContainsKey(group.Value))
                    {
                        ThingTypeInfo t = new ThingTypeInfo(group.Key, thingtypesbyclass[group.Value]);

                        // Add new thing, replacing already existing one if necessary
                        t.Category.AddThing(t);
                        thingtypes[group.Key] = t;
                    }
                    // Loudly give up...
                    else
                    {
                        General.ErrorLogger.Add(ErrorType.Warning, "Failed to apply MAPINFO DoomEdNum override \"" + group.Key + " = " + group.Value + "\": failed to find corresponding actor class...");
                    }
                }

                // Remove items
                foreach (int id in toremove)
                {
                    if (thingtypes.ContainsKey(id))
                    {
                        thingtypes[id].Category.RemoveThing(thingtypes[id]);
                        thingtypes.Remove(id);
                    }
                }
            }

            //mxd. Step 3. Gather DECORATE SpawnIDs
            Dictionary<int, EnumItem> configspawnnums = new Dictionary<int, EnumItem>();

            // Update or create the main enums list
            if (General.Map.Config.Enums.ContainsKey("spawnthing"))
            {
                foreach (EnumItem item in General.Map.Config.Enums["spawnthing"])
                    configspawnnums.Add(item.GetIntValue(), item);
            }

            bool spawnidschanged = false;
            foreach (ActorStructure actor in mergedActors)
            {
                int spawnid = actor.GetPropertyValueInt("spawnid", 0);
                if (spawnid != 0)
                {
                    configspawnnums[spawnid] = new EnumItem(spawnid.ToString(), (actor.HasPropertyWithValue("$title") ? actor.GetPropertyAllValues("$title") : actor.ClassName));
                    spawnidschanged = true;
                }
            }

            //mxd. Step 4. Update SpawnNums using MAPINFO overrides
            if (spawnnumsoverride.Count > 0)
            {
                // Modify by MAPINFO data
                foreach (KeyValuePair<int, string> group in spawnnumsoverride)
                    configspawnnums[group.Key] = new EnumItem(group.Key.ToString(), (thingtypes.ContainsKey(group.Key) ? thingtypes[group.Key].Title : group.Value));

                spawnidschanged = true;
            }

            if (spawnidschanged)
            {
                // Update the main collection
                EnumList newenums = new EnumList();
                newenums.AddRange(configspawnnums.Values);
                newenums.Sort((a, b) => a.Title.CompareTo(b.Title));
                General.Map.Config.Enums["spawnthing"] = newenums;

                // Update all ArgumentInfos...
                foreach (ThingTypeInfo info in thingtypes.Values)
                {
                    foreach (ArgumentInfo ai in info.Args)
                        if (ai.Enum.Name == "spawnthing") ai.Enum = newenums;
                }

                foreach (LinedefActionInfo info in General.Map.Config.LinedefActions.Values)
                {
                    foreach (ArgumentInfo ai in info.Args)
                        if (ai.Enum.Name == "spawnthing") ai.Enum = newenums;
                }
            }

            return counter;
        }

		/// <summary>
		/// Adds things defined in a Dehacked patch to the list of things
		/// </summary>
		/// <returns>Number of changed/added Dehacked things</returns>
		private int ApplyDehackedThings()
		{
			int numaddthings = 0;

			foreach(DehackedThing t in dehacked.Things)
			{
				// This is not a thing that can be placed in the map
				if (t.DoomEdNum <= 0)
					continue;

				DecorateCategoryInfo dci = GetCategoryInfo(t, thingcategories);
				ThingCategory cat = GetThingCategory(null, thingcategories, dci);
				ThingTypeInfo tti = new ThingTypeInfo(cat, t);

				if (!thingtypes.ContainsKey(t.DoomEdNum))
				{
					thingtypes[t.DoomEdNum] = tti;
					cat.AddThing(tti);
				}
				else
				{
					if (!string.IsNullOrEmpty(t.Category))
					{
						// Remove the thing from its old category...
						thingtypes[t.DoomEdNum].Category.RemoveThing(thingtypes[t.DoomEdNum]);
						thingtypes[t.DoomEdNum] = tti;

						// ... and add it to the new one
						cat.AddThing(tti);
					}
					else
					{
						thingtypes[t.DoomEdNum].ModifyByDehackedThing(t);
					}
				}

				numaddthings++;				
			}

			return numaddthings;
		}

		/// <summary>
		/// Fixes all thing type infos to use the sprites that were renamed through Dehacked
		/// </summary>
		private void FixRenamedDehackedSprites()
		{
			if (dehacked.Things.Count == 0)
				return;

			foreach(ThingTypeInfo tti in thingtypes.Values)
			{
				tti.ModifyBySpriteReplacement(dehacked.GetSpriteReplacements());
			}
		}

        //mxd
        private static ThingCategory GetThingCategory(ThingCategory parent, List<ThingCategory> categories, DecorateCategoryInfo catinfo) 
		{
			// Find the category to put the actor in
			ThingCategory cat = null;
			string catname = (catinfo.Category.Count > 0 ? catinfo.Category[0].Trim().ToLowerInvariant() : string.Empty); //catnames[0].ToLowerInvariant().Trim();
			if(string.IsNullOrEmpty(catname)) catname = "custom";

			// First search by Title...
			foreach(ThingCategory c in categories) 
			{
				if(string.Equals(c.Title, catname, StringComparison.OrdinalIgnoreCase))
				{
					cat = c;
					break;
				}
			}

			// Make full name
			if(parent != null) catname = parent.Name.ToLowerInvariant() + "." + catname;

			//...then - by Name
			if(cat == null) 
			{
				foreach(ThingCategory c in categories) 
				{
					if(string.Equals(c.Name, catname, StringComparison.OrdinalIgnoreCase))
					{
						cat = c;
						break;
					}
				}
			}

			// Make the category if needed
			if(cat == null)
			{
				string cattitle = (catinfo.Category.Count > 0 ? catinfo.Category[0].Trim() : string.Empty);
				if(string.IsNullOrEmpty(cattitle)) cattitle = "User-defined";
				cat = new ThingCategory(parent, catname, cattitle, catinfo);
				categories.Add(cat); // ^.^
			}

			// Still have subcategories?
			if(catinfo.Category.Count > 1)
			{
				catinfo.Category.RemoveAt(0);
				return GetThingCategory(cat, cat.Children, catinfo);
			}

			return cat;
		}

		private static DecorateCategoryInfo GetCategoryInfo(DehackedThing thing, List<ThingCategory> categories)
		{
			string catname = null;

			// Try to find which category the thing is in
			foreach(ThingCategory c in categories)
			{
				foreach (ThingTypeInfo tti in c.Things)
				{
					if (tti.Index == thing.DoomEdNum)
					{
						catname = c.Title;
						break;
					}
				}

				if (!string.IsNullOrEmpty(catname))
					break;
			}

			DecorateCategoryInfo catinfo = new DecorateCategoryInfo();
			if(string.IsNullOrEmpty(thing.Category)) // No category for the thing was set through Dehacked
			{
				if (!string.IsNullOrEmpty(catname)) // We did find the category the thing was originally in
					catinfo.Category = catname.Split(CATEGORY_SPLITTER, StringSplitOptions.RemoveEmptyEntries).ToList();
				else // The thing wasn't in a category before, put it in the "User-defined" category
					catinfo.Category = new List<string> { "User-defined" };
			}
			else // A category for the thing was set through Dehacked
			{
				catinfo.Category = thing.Category.Split(CATEGORY_SPLITTER, StringSplitOptions.RemoveEmptyEntries).ToList();
			}

			return catinfo;
		}

		//mxd
		private static DecorateCategoryInfo GetCategoryInfo(ActorStructure actor)
		{
			string catname = ZDTextParser.StripQuotes(actor.GetPropertyAllValues("$category")).Trim();
			
			DecorateCategoryInfo catinfo = new DecorateCategoryInfo();
			if(string.IsNullOrEmpty(catname))
			{
				if(actor.CategoryInfo != null)
				{
					catinfo.Category = new List<string>(actor.CategoryInfo.Category);
					catinfo.Properties = new Dictionary<string, List<string>>(actor.CategoryInfo.Properties);
				}
				else
				{
					catinfo.Category = new List<string> { "User-defined" };
				}
			}
			else
			{
				catinfo.Category = catname.Split(CATEGORY_SPLITTER, StringSplitOptions.RemoveEmptyEntries).ToList(); //mxd
			}

			return catinfo;
		}
		
		// This loads Decorate data from a specific file or lump name
		private void LoadDecorateFromLocation(DecorateParser parser, string location)
		{
			//General.WriteLogLine("Including DECORATE resource '" + location + "'...");
			IEnumerable<TextResourceData> decostreams = currentreader.GetDecorateData(location);
			foreach(TextResourceData data in decostreams)
			{
				// Parse this data
				parser.Parse(data, false);

				//mxd. DECORATE lumps are interdepandable. Can't carry on...
				if(parser.HasError)
				{
					parser.LogError();
					return;
				}
			}
		}

        private void LoadZScriptFromLocation(ZScriptParser parser, string location)
        {
            IEnumerable<TextResourceData> streams = currentreader.GetZScriptData(location);
            foreach (TextResourceData data in streams)
            {
                // Parse this data
                parser.Parse(data, false);

                //mxd. DECORATE lumps are interdepandable. Can't carry on...
                if (parser.HasError)
                {
                    parser.LogError();
                    return;
                }
            }
        }

        // This loads MODELDEF data from a specific file or lump name
        private void LoadModeldefFromLocation(ModeldefParser parser, string location)
        {
            IEnumerable<TextResourceData> streams = currentreader.GetModeldefData(location);
            foreach (TextResourceData data in streams)
            {
                // Parse this data
                parser.Parse(data, false);

                //mxd. DECORATE lumps are interdepandable. Can't carry on...
                if (parser.HasError)
                {
                    parser.LogError();
                    return;
                }
            }
        }
		
		// This gets thing information by index
		public ThingTypeInfo GetThingInfo(int thingtype)
		{
			// Index in config?
			if(thingtypes.ContainsKey(thingtype))
			{
				// Return from config
				return thingtypes[thingtype];
			}

			// Create unknown thing info
			return new ThingTypeInfo(thingtype);
		}

		// This gets thing information by index
		// Returns null when thing type info could not be found
		public ThingTypeInfo GetThingInfoEx(int thingtype)
		{
			// Index in config?
			if(thingtypes.ContainsKey(thingtype))
			{
				// Return from config
				return thingtypes[thingtype];
			}

			// No such thing type known
			return null;
		}
		
#endregion

#region ================== mxd. Modeldef, Voxeldef, Gldefs, Mapinfo

		//mxd. This creates <Actor Class, Thing.Type> dictionary. Should be called after all DECORATE actors are parsed
		private Dictionary<string, int> CreateActorsByClassList() 
		{
			Dictionary<string, int> actors = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
			if(string.IsNullOrEmpty(General.Map.Config.DecorateGames)) return actors;

			//read our new shiny ClassNames for default game things
			foreach(KeyValuePair<int, ThingTypeInfo> ti in thingtypes) 
			{
				if(!string.IsNullOrEmpty(ti.Value.ClassName))
				{
					if(actors.ContainsKey(ti.Value.ClassName) && actors[ti.Value.ClassName] != ti.Key)
						General.ErrorLogger.Add(ErrorType.Warning, "Actor \"" + ti.Value.ClassName + "\" has several editor numbers (" + actors[ti.Value.ClassName] + " and " + ti.Key + "). Only the last one will be used.");
					actors[ti.Value.ClassName] = ti.Key;
				}
			}

			if(actors.Count == 0) 
				General.ErrorLogger.Add(ErrorType.Warning, "Unable to find any DECORATE actor definitions!");

			return actors;
		}

		//mxd
		public void ReloadModeldef() 
		{
			if(modeldefentries != null) 
				foreach(ModelData md in modeldefentries.Values) md.Dispose();

			// Bail out when not supported by current game configuration
			if(string.IsNullOrEmpty(General.Map.Config.DecorateGames)) return;

			General.MainWindow.DisplayStatus(StatusType.Busy, "Reloading model definitions...");
			LoadModeldefs(CreateActorsByClassList());

			General.MainWindow.DisplayStatus(StatusType.Busy, "Reloading voxel definitions...");
			LoadVoxels();

			foreach(Thing t in General.Map.Map.Things) t.UpdateCache();

			// Rebuild geometry if in Visual mode
			if(General.Editing.Mode != null && General.Editing.Mode.GetType().Name == "BaseVisualMode") 
			{
				General.Editing.Mode.OnReloadResources();
			}

			General.MainWindow.DisplayReady();
		}

		//mxd
		public void ReloadGldefs() 
		{
			// Bail out when not supported by current game configuration
			if(string.IsNullOrEmpty(General.Map.Config.DecorateGames)) return;
			
			General.MainWindow.DisplayStatus(StatusType.Busy, "Reloading GLDEFS...");

			try 
			{
				LoadGldefs(CreateActorsByClassList());
			} 
			catch(ArgumentNullException) 
			{
				MessageBox.Show("GLDEFS reload failed. Try using 'Reload Resources' instead.\nCheck 'Errors and Warnings' window for more details.");
				General.MainWindow.DisplayReady();
				return;
			}

			// Rebuild skybox texture
			SetupSkybox();

			// Rebuild geometry if in Visual mode
			if(General.Editing.Mode != null && General.Editing.Mode.GetType().Name == "BaseVisualMode") 
			{
				General.Editing.Mode.OnReloadResources();
			}

			General.MainWindow.DisplayReady();
		}

		//mxd. This parses modeldefs. Should be called after all DECORATE actors are parsed
		private void LoadModeldefs(Dictionary<string, int> actorsbyclass) 
		{
			// Abort if no classnames are defined in DECORATE or game config...
			if(actorsbyclass.Count == 0) return;

			ModeldefParser parser = new ModeldefParser(actorsbyclass) { OnInclude = LoadModeldefFromLocation };
			foreach(DataReader dr in containers)
			{
				currentreader = dr;

				IEnumerable<TextResourceData> streams = dr.GetTextLumpData(ScriptType.MODELDEF, false, true);
				foreach(TextResourceData data in streams) 
				{
					// Parse the data
					parser.Parse(data, true);

					// Modeldefs are independable, so parsing fail in one file should not affect the others
					if(parser.HasError) parser.LogError();
				}
			}

			// Add to text resources collection
			scriptresources[parser.ScriptType] = new HashSet<ScriptResource>(parser.ScriptResources.Values);
			currentreader = null;

			foreach(KeyValuePair<string, ModelData> e in parser.Entries) 
			{
				if(actorsbyclass.ContainsKey(e.Key))
					modeldefentries[actorsbyclass[e.Key]] = parser.Entries[e.Key];
				else if(!decorate.ActorsByClass.ContainsKey(e.Key))
					General.ErrorLogger.Add(ErrorType.Warning, "MODELDEF model \"" + e.Key + "\" doesn't match any Decorate actor class");
			}
		}

		//mxd
		private void LoadVoxels() 
		{
			// Bail out when not supported by current game configuration
			if(string.IsNullOrEmpty(General.Map.Config.DecorateGames)) return;
			
			// Go for all things
			Dictionary<string, List<int>> allsprites = new Dictionary<string, List<int>>(StringComparer.Ordinal);
			foreach(ThingTypeInfo ti in thingtypes.Values) 
			{
				// Valid sprite name?
				if(string.IsNullOrEmpty(ti.Sprite) || ti.Sprite.Length > CLASIC_IMAGE_NAME_LENGTH) continue;
				if(!allsprites.ContainsKey(ti.Sprite)) allsprites.Add(ti.Sprite, new List<int>());
				allsprites[ti.Sprite].Add(ti.Index);
			}

			VoxeldefParser parser = new VoxeldefParser();
			HashSet<string> processed = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

			// Parse VOXLEDEF
			foreach(DataReader dr in containers) 
			{
				currentreader = dr;

				IEnumerable<TextResourceData> streams = dr.GetTextLumpData(ScriptType.VOXELDEF, false, false);
				foreach(TextResourceData data in streams)
				{
					if(parser.Parse(data, true))
					{
						foreach(KeyValuePair<string, ModelData> entry in parser.Entries)
						{
							foreach(KeyValuePair<string, List<int>> sc in allsprites)
							{
								if(sc.Key.StartsWith(entry.Key, StringComparison.OrdinalIgnoreCase))
								{
									foreach(int id in sc.Value) modeldefentries[id] = entry.Value;
									processed.Add(sc.Key);

									// Create preview image if it doesn't exist...
									ImageData sprite = GetSpriteImage(sc.Key);
									if(sprite == null)
									{
										// Make new voxel image
										sprite = new VoxelImage(sc.Key, entry.Value.ModelNames[0]);

										// Add to collection
										sprites.Add(sprite.LongName, sprite);
                                    }

									// Apply VOXELDEF settings to the preview image...
									VoxelImage vi = sprite as VoxelImage;
									if(vi != null)
									{
										vi.AngleOffset = (int)Math.Round(entry.Value.AngleOffset);
										vi.OverridePalette = entry.Value.OverridePalette;
									}
								}
							}
						}
					}

					// Report errors?
					if(parser.HasError) parser.LogError();
				}
			}

			// Add to text resources collection
			scriptresources[parser.ScriptType] = new HashSet<ScriptResource>(parser.ScriptResources.Values);
			currentreader = null;

			// Get voxel models
			foreach(KeyValuePair<string, List<int>> sc in allsprites)
			{
				if(processed.Contains(sc.Key)) continue;
				
				VoxelImage vi = GetSpriteImage(sc.Key) as VoxelImage;
				if(vi != null)
				{
					// It's a model without a definition, and it corresponds to a sprite we can display, so let's add it
					ModelData data = new ModelData { IsVoxel = true };
					data.ModelNames.Add(vi.VoxelName);

					foreach(int id in sc.Value) modeldefentries[id] = data;
				}
			}
		}

		//mxd. This parses gldefs. Should be called after all DECORATE actors are parsed
		private void LoadGldefs(Dictionary<string, int> actorsbyclass) 
		{
			// Skip if no actors defined in DECORATE or game config...
			if(actorsbyclass.Count == 0) return;

			GldefsParser parser = new GldefsParser { OnInclude = ParseFromLocation };

			// Load gldefs from resources
			foreach(DataReader dr in containers) 
			{
				if(parser.HasError) break;

				currentreader = dr;
				parser.ClearIncludesList();
				IEnumerable<TextResourceData> streams = dr.GetGldefsData(General.Map.Config.BaseGame);

				foreach(TextResourceData data in streams)
				{
					parser.Parse(data, false);
					
					// Gldefs can be interdependable. Can't carry on
					if(parser.HasError)
					{
						parser.LogError();
						break;
					}
				}
			}

			//mxd. Add to text resources collection
			scriptresources[parser.ScriptType] = new HashSet<ScriptResource>(parser.ScriptResources.Values);
			currentreader = null;

			//mxd. Abort on errors, but after adding parsed text resources
			if(parser.HasError) return;

			// Create Gldefs Entries dictionary
			foreach(KeyValuePair<string, string> e in parser.Objects) //<ClassName, Light name>
			{
				// Check if we have decorate actor and light definition for given ClassName
				//INFO: objects without corresponding actors are already reported by the parser
				if(actorsbyclass.ContainsKey(e.Key))
				{
					if(parser.LightsByName.ContainsKey(e.Value))
					{
						gldefsentries[actorsbyclass[e.Key]] = parser.LightsByName[e.Value];
					}
					else
					{
						//INFO: Lights CAN be defiend after Objects, so we can't perform any object->light matching checks while parsing
						General.ErrorLogger.Add(ErrorType.Error, "GLDEFS object \"" + e.Key + "\" references undefined light \"" + e.Value + "\"");
					}
				}
			}

			// Apply dynamic lights defined using Light() state expression
			foreach(ThingTypeInfo info in thingtypes.Values)
			{
				if(string.IsNullOrEmpty(info.LightName)) continue;
				if(parser.LightsByName.ContainsKey(info.LightName))
					gldefsentries[info.Index] = parser.LightsByName[info.LightName];
				else
					General.ErrorLogger.Add(ErrorType.Error, "Actor \"" + info.Title + "\":" + info.Index + " references undefined light \"" + info.LightName + "\"");
			}

			// Grab them glowy flats!
			glowingflats = parser.GlowingFlats;

			// And skyboxes
			skyboxes = parser.Skyboxes;
		}

		//mxd. This updates mapinfo class only
		internal void ReloadMapInfoPartial()
		{
			Dictionary<int, string> spawnnums, doomednums;
			LoadMapInfo(out spawnnums, out doomednums);
		}

		//mxd. This loads (Z)MAPINFO
		private void LoadMapInfo(out Dictionary<int, string> spawnnums, out Dictionary<int, string> doomednums)
		{
			MapinfoParser parser = new MapinfoParser { OnInclude = ParseFromLocation };

			// Parse mapinfo 
			foreach(DataReader dr in containers)
			{
				currentreader = dr;
				IEnumerable<TextResourceData> streams = dr.GetMapinfoData();

				foreach(TextResourceData data in streams)
				{
					// Parse the data
					parser.Parse(data, General.Map.Options.LevelName, false);

					//MAPINFO lumps are interdependable. Can't carry on...
					if(parser.HasError)
					{
						parser.LogError();
						break;
					}
				}
			}
			
			if(!parser.HasError)
			{
				// Store parsed data
				spawnnums = parser.SpawnNums;
				doomednums = parser.DoomEdNums;
				mapinfo = parser.MapInfo;
			}
			else
			{
				// No nulls allowed!
				spawnnums = new Dictionary<int, string>();
				doomednums = new Dictionary<int, string>();
				mapinfo = new MapInfo();
			}

			//mxd. Add to text resources collection
			scriptresources[parser.ScriptType] = new HashSet<ScriptResource>(parser.ScriptResources.Values);
			currentreader = null;
		}

		private void ParseFromLocation(ZDTextParser parser, string location, bool clearerrors)
		{
			if(currentreader.IsSuspended) throw new Exception("Data reader is suspended");
			parser.Parse(new TextResourceData(currentreader, currentreader.LoadFile(location), location, true), clearerrors);
		}

		//mxd. This loads REVERBS
		private void LoadReverbs() 
		{
			reverbs.Clear();

			// Bail out when not supported by current game configuration
			if(string.IsNullOrEmpty(General.Map.Config.DecorateGames)) return;

			ReverbsParser parser = new ReverbsParser();
			foreach(DataReader dr in containers) 
			{
				currentreader = dr;
				IEnumerable<TextResourceData> streams = dr.GetTextLumpData(ScriptType.REVERBS, false, false);
				foreach(TextResourceData data in streams) 
				{
					// Parse the data
					parser.Parse(data, true);

					// Report errors?
					if(parser.HasError) parser.LogError();
				}
			}

			//mxd. Add to text resources collection
			scriptresources[parser.ScriptType] = new HashSet<ScriptResource>(parser.ScriptResources.Values);
			currentreader = null;
			reverbs = parser.GetReverbs();
		}

		//mxd. This loads SNDINFO
		private void LoadSndInfo()
		{
			// Bail out when not supported by current game configuration
			if(string.IsNullOrEmpty(General.Map.Config.DecorateGames)) return;

			SndInfoParser parser = new SndInfoParser();
			foreach(DataReader dr in containers)
			{
				currentreader = dr;
				IEnumerable<TextResourceData> streams = dr.GetTextLumpData(ScriptType.SNDINFO, false, false);

				// Parse the data
				foreach(TextResourceData data in streams)
				{
					parser.Parse(data, true);

					// Report errors?
					if(parser.HasError) parser.LogError();
				}
			}

			// Add to text resources collection
			scriptresources[parser.ScriptType] = new HashSet<ScriptResource>(parser.ScriptResources.Values);
			currentreader = null;

			// Anything to do?
			parser.FinishSetup();
			if(parser.AmbientSounds.Count > 0)
			{
				// Update or create the main enums list
				Dictionary<int, EnumItem> configenums = new Dictionary<int, EnumItem>();
				if(General.Map.Config.Enums.ContainsKey("ambient_sounds"))
				{
					foreach(EnumItem item in General.Map.Config.Enums["ambient_sounds"])
						configenums.Add(item.GetIntValue(), item);
				}
				if(configenums.ContainsKey(0)) configenums.Remove(0);

				foreach(KeyValuePair<int, AmbientSoundInfo> group in parser.AmbientSounds)
				{
					configenums[group.Key] = new EnumItem(group.Key.ToString(), group.Value.SoundDescription);
				}

				// Store results in "ambient_sounds" enum
				EnumList newenums = new EnumList();
				newenums.AddRange(configenums.Values);
				newenums.Sort(); // Sort by ambient sound index
				newenums.Insert(0, new EnumItem("0", "None")); // Add "None" value
				General.Map.Config.Enums["ambient_sounds"] = newenums;

				// Update all ArgumentInfos...
				foreach(ThingTypeInfo info in thingtypes.Values)
				{
					foreach(ArgumentInfo ai in info.Args)
						if(ai.Enum.Name == "ambient_sounds") ai.Enum = newenums;
				}

				foreach(LinedefActionInfo info in General.Map.Config.LinedefActions.Values)
				{
					foreach(ArgumentInfo ai in info.Args)
						if(ai.Enum.Name == "ambient_sounds") ai.Enum = newenums;
				}

				// Update "Ambient Sound XX" thing names. Hardcoded for things 14001 - 14064 for now...
				for(int i = 14001; i < 14065; i++)
				{
					int ambsoundindex = i - 14000;

					// Attach AmbientSoundInfo
					if(parser.AmbientSounds.ContainsKey(ambsoundindex)) 
						thingtypes[i].AmbientSound = parser.AmbientSounds[ambsoundindex];

					// Update title
					if(configenums.ContainsKey(ambsoundindex) && thingtypes.ContainsKey(i) && string.IsNullOrEmpty(thingtypes[i].ClassName))
						thingtypes[i].Title += " (" + configenums[ambsoundindex] + ")";
				}
			}

			// Store collection
			ambientsounds = parser.AmbientSounds;
		}

		//mxd. This loads SNDSEQ
		private void LoadSndSeq()
		{
			// Bail out when not supported by current game configuration
			if(string.IsNullOrEmpty(General.Map.Config.DecorateGames)) return;

			SndSeqParser parser = new SndSeqParser();
			foreach(DataReader dr in containers) 
			{
				currentreader = dr;
				IEnumerable<TextResourceData> streams = dr.GetTextLumpData(ScriptType.SNDSEQ, false, false);

				// Parse the data
				foreach(TextResourceData data in streams)
				{
					parser.Parse(data, true);

					// Report errors?
					if(parser.HasError) parser.LogError();
				}
			}

			// Add to text resources collection
			scriptresources[parser.ScriptType] = new HashSet<ScriptResource>(parser.ScriptResources.Values);
			currentreader = null;
			soundsequences = parser.GetSoundSequences();
		}

		//mxd. This loads cameratextures from ANIMDEFS
		private void LoadAnimdefs()
		{
			// Bail out when not supported by current game configuration
			if(string.IsNullOrEmpty(General.Map.Config.DecorateGames)) return;

			AnimdefsParser parser = new AnimdefsParser();
			foreach(DataReader dr in containers)
			{
				currentreader = dr;
				IEnumerable<TextResourceData> streams = dr.GetTextLumpData(ScriptType.ANIMDEFS, false, false);

				// Parse the data
				foreach(TextResourceData data in streams)
				{
					parser.Parse(data, true);

					// Report errors?
					if(parser.HasError) parser.LogError();

					// Create images
					foreach(var g in parser.CameraTextures)
					{
						// Grab a local copy
						CameraTextureData camtexdata = g.Value;

						// Apply texture size override?
						if(!camtexdata.FitTexture)
						{
							long longname = Lump.MakeLongName(camtexdata.Name);

							if(textures.ContainsKey(longname))
							{
								camtexdata.ScaleX = (float)textures[longname].Width / camtexdata.Width;
								camtexdata.ScaleY = (float)textures[longname].Height / camtexdata.Height;
							}
							else if(flats.ContainsKey(longname))
							{
								camtexdata.ScaleX = (float)flats[longname].Width / camtexdata.Width;
								camtexdata.ScaleY = (float)flats[longname].Height / camtexdata.Height;
							}
						}

						// Create texture
						CameraTextureImage camteximage = new CameraTextureImage(camtexdata);

						// Add to flats and textures
						texturenames.Add(camteximage.Name);
						flatnames.Add(camteximage.Name);

						//TODO: Do cameratextures override stuff like this?..
						textures[camteximage.LongName] = camteximage;
						flats[camteximage.LongName] = camteximage;

                        // Add to container's texture set
                        currentreader.TextureSet.AddFlat(camteximage);
						currentreader.TextureSet.AddTexture(camteximage);
					}
				}
			}

			//mxd. Add to text resources collection
			scriptresources[parser.ScriptType] = new HashSet<ScriptResource>(parser.ScriptResources.Values);
			currentreader = null;
		}

		//mxd. This loads TERRAIN
		private void LoadTerrain()
		{
			// Bail out when not supported by current game configuration
			if(string.IsNullOrEmpty(General.Map.Config.DecorateGames)) return;

			TerrainParser parser = new TerrainParser();
			foreach(DataReader dr in containers)
			{
				currentreader = dr;
				IEnumerable<TextResourceData> streams = dr.GetTextLumpData(ScriptType.TERRAIN, false, false);

				// Parse the data
				foreach(TextResourceData data in streams)
				{
					parser.Parse(data, true);

					// Report errors?
					if(parser.HasError) parser.LogError();
				}
			}

			// Add to text resources collection
			scriptresources[parser.ScriptType] = new HashSet<ScriptResource>(parser.ScriptResources.Values);
			currentreader = null;

			// Sort
			List<string> names = new List<string>(parser.TerrainNames);
			names.Sort();

			// Set as collection
			terrainnames = names.ToArray();
		}

		//mxd. This loads X11R6RGB
		private void LoadX11R6RGB()
		{
			X11R6RGBParser parser = new X11R6RGBParser();

			foreach(DataReader dr in containers)
			{
				currentreader = dr;
				IEnumerable<TextResourceData> streams = dr.GetTextLumpData(ScriptType.X11R6RGB, true, false);

				// Parse the data
				foreach(TextResourceData data in streams)
				{
					parser.Parse(data, true);

					// Report errors?
					if(parser.HasError) parser.LogError();
				}
			}

			// Add to text resources collection
			scriptresources[parser.ScriptType] = new HashSet<ScriptResource>(parser.ScriptResources.Values);
			currentreader = null;

			// Set as collection
			knowncolors = parser.KnownColors;
		}

		//mxd. This loads CVARINFO lumps
		private void LoadCvarInfo()
		{
			CvarInfoParser parser = new CvarInfoParser();

			foreach(DataReader dr in containers)
			{
				currentreader = dr;
				IEnumerable<TextResourceData> streams = dr.GetTextLumpData(ScriptType.CVARINFO, false, false);

				// Parse the data
				foreach(TextResourceData data in streams)
				{
					parser.Parse(data, true);

					// Report errors?
					if(parser.HasError) parser.LogError();
				}
			}

			// Add to text resources collection
			scriptresources[parser.ScriptType] = new HashSet<ScriptResource>(parser.ScriptResources.Values);
			currentreader = null;

			// Set as collection
			cvars = parser.Cvars;
		}

		//mxd. This loads LOCKDEFS lumps
		private void LoadLockDefs()
		{
			LockDefsParser parser = new LockDefsParser();

			foreach(DataReader dr in containers)
			{
				currentreader = dr;
				IEnumerable<TextResourceData> streams = dr.GetTextLumpData(ScriptType.LOCKDEFS, false, false);

				// Parse the data
				foreach(TextResourceData data in streams)
				{
					parser.Parse(data, true);

					// Report errors?
					if(parser.HasError) parser.LogError();
				}
			}

			// Add to text resources collection
			scriptresources[parser.ScriptType] = new HashSet<ScriptResource>(parser.ScriptResources.Values);
			currentreader = null;

			// Apply to the enums list?
			EnumList keys = parser.GetLockDefs();
			lockableactions = new Dictionary<int, int>();
			if(keys.Count > 0)
			{
				keys.Sort((a, b) => a.Title.CompareTo(b.Title));
				keys.Insert(0, new EnumItem("0", "None"));
				General.Map.Config.Enums["keys"] = keys;
				
				// Update all ArgumentInfos...
				foreach(ThingTypeInfo info in thingtypes.Values)
				{
					foreach(ArgumentInfo ai in info.Args)
						if(ai.Enum.Name == "keys") ai.Enum = General.Map.Config.Enums["keys"];
				}

				foreach(LinedefActionInfo info in General.Map.Config.LinedefActions.Values)
				{
					for(int i = 0; i < info.Args.Length; i++)
					{
						if(info.Args[i].Enum.Name == "keys")
						{
							info.Args[i].Enum = General.Map.Config.Enums["keys"];
							lockableactions[info.Index] = i;
						}
					}
				}

				// Also store lock colors
				lockcolors = parser.MapColors;
			}
			else
			{
				lockcolors = new Dictionary<int, PixelColor>();
			}
		}

		/// <summary>
		/// Load DECALDEF decal definitions
		/// </summary>
		private void LoadDecalDefs()
		{
			// Bail out when not supported by current game configuration
			if (string.IsNullOrEmpty(General.Map.Config.DecorateGames)) return;

			DecalDefsParser parser = new DecalDefsParser();

			foreach(DataReader dr in containers)
			{
				currentreader = dr; // Why?
				IEnumerable<TextResourceData> streams = dr.GetTextLumpData(ScriptType.DECALDEF, false, false);

				// Parse the data
				foreach(TextResourceData data in streams)
				{
					parser.Parse(data, true);

					// Report errors?
					if (parser.HasError) parser.LogError();
				}
			}

			currentreader = null; // Why?

			if(parser.Decals.Count > 0)
			{
				// Update or create the main enums list
				Dictionary<int, EnumItem> configenums = new Dictionary<int, EnumItem>();
				if (General.Map.Config.Enums.ContainsKey("decals"))
				{
					foreach (EnumItem item in General.Map.Config.Enums["decals"])
						configenums.Add(item.GetIntValue(), item);
				}
				if (configenums.ContainsKey(0)) configenums.Remove(0);

				foreach (KeyValuePair<int, DecalInfo> group in parser.GetDecalDefsById())
				{
					configenums[group.Key] = new EnumItem(group.Key.ToString(), group.Value.Description);
				}

				// Store results in "decals" enum
				EnumList newenums = new EnumList();
				newenums.AddRange(configenums.Values);
				newenums.Sort();
				newenums.Insert(0, new EnumItem("0", "None"));
				General.Map.Config.Enums["decals"] = newenums;

				// Update all ArgumentInfos...
				foreach (ThingTypeInfo info in thingtypes.Values)
				{
					foreach (ArgumentInfo ai in info.Args)
						if (ai.Enum.Name == "decals") ai.Enum = newenums;
				}
			}
		}

		//mxd. This collects ZDoom text lumps, which are not used by the editor anywhere outside the Script Editor
		private void LoadExtraTextLumps()
		{
			Dictionary<ScriptType, bool> extralumptypes = new Dictionary<ScriptType, bool> // <script type, singular>
			{
				{ ScriptType.MENUDEF, true },
				{ ScriptType.SBARINFO, true }, //TODO: SBARINFO supports #include
				{ ScriptType.GAMEINFO, true },
				{ ScriptType.KEYCONF, true },
				{ ScriptType.FONTDEFS, true },
				//TODO: load/create SCRIPTS and DIALOG here?
			};

			foreach(KeyValuePair<ScriptType, bool> group in extralumptypes)
			{
				HashSet<ScriptResource> resources = new HashSet<ScriptResource>();
				foreach(DataReader dr in containers)
				{
					currentreader = dr;
					IEnumerable<TextResourceData> streams = dr.GetTextLumpData(group.Key, group.Value, false);

					// Add text tesources
					foreach(TextResourceData data in streams) resources.Add(new ScriptResource(data, group.Key));
				}

				// Add to collection
				if(resources.Count > 0) scriptresources[group.Key] = resources;
			}
		}

		//mxd
		internal TextResourceData GetTextResourceData(string name) 
		{
			// Filesystem path?
			try
			{
				if (Path.IsPathRooted(name))
				{
					if (File.Exists(name))
					{
						DataLocation location = new DataLocation { location = name, type = DataLocation.RESOURCE_DIRECTORY };
						return new TextResourceData(File.OpenRead(name), location, name);
					}

					return null;
				}
			}
			catch (ArgumentException e)
			{
				// File and/or path contained illegal characters
				return null;
			}

			// Search in resources
			for(int i = containers.Count - 1; i >= 0; i--)
			{
				if(containers[i].FileExists(name))
					return new TextResourceData(containers[i], containers[i].LoadFile(name), name, true);
			}

			return null;
		}

#endregion

#region ================== Tools

		// This finds the first IWAD or IPK3 resource
		// Returns false when not found
		public bool FindFirstIWAD(out DataLocation result)
		{
			// Go for all data containers
			foreach(DataReader dr in containers)
			{
				// Container is a WAD file?
				if(dr is WADReader)
				{
					// Check if it is an IWAD
					WADReader wr = dr as WADReader;
					if(wr.IsIWAD)
					{
						// Return location!
						result = wr.Location;
						return true;
					}
				}
				else if(dr is PK3Reader)
				{
					PK3Reader pk3r = dr as PK3Reader;
					if(pk3r.GetIWadInfos().Count == 1)
					{
						result = pk3r.Location;
						return true;
					}
				}
			}

			// No IWAD found
			result = new DataLocation();
			return false;
		}

		// This signals the background thread to update the
		// used-in-map status on all textures and flats
		public void UpdateUsedTextures()
		{
            var usedtextures = new Dictionary<long, bool>();
            var usedflats = new Dictionary<long, bool>();

            if (General.Map.Config.MixTexturesFlats)
			{
				// Go through the map to find the used textures
				foreach(Sidedef sd in General.Map.Map.Sidedefs)
				{
					// Add used textures to dictionary
					if(sd.LongHighTexture != MapSet.EmptyLongName)
					{
						usedtextures[sd.LongHighTexture] = true;

						//mxd. Part of long name support shennanigans
						if(texturenamesshorttofull.ContainsKey(sd.LongHighTexture))
							usedtextures[texturenamesshorttofull[sd.LongHighTexture]] = true;
					}
					if(sd.LongMiddleTexture != MapSet.EmptyLongName)
					{
						usedtextures[sd.LongMiddleTexture] = true;

						//mxd. Part of long name support shennanigans
						if(texturenamesshorttofull.ContainsKey(sd.LongMiddleTexture))
							usedtextures[texturenamesshorttofull[sd.LongMiddleTexture]] = true;
					}
					if(sd.LongLowTexture != MapSet.EmptyLongName)
					{
						usedtextures[sd.LongLowTexture] = true;

						//mxd. Part of long name support shennanigans
						if(texturenamesshorttofull.ContainsKey(sd.LongLowTexture))
							usedtextures[texturenamesshorttofull[sd.LongLowTexture]] = true;
					}
				}

				// Go through the map to find the used flats
				foreach(Sector s in General.Map.Map.Sectors)
				{
					// Add used flats to dictionary
					usedtextures[s.LongFloorTexture] = false;
					usedtextures[s.LongCeilTexture] = false;

					//mxd. Part of long name support shennanigans
					if(flatnamesshorttofull.ContainsKey(s.LongFloorTexture))
						usedtextures[flatnamesshorttofull[s.LongFloorTexture]] = false;
					if(flatnamesshorttofull.ContainsKey(s.LongCeilTexture))
						usedtextures[flatnamesshorttofull[s.LongCeilTexture]] = false;
				}
			}
			else
			{

				// Go through the map to find the used textures
				foreach(Sidedef sd in General.Map.Map.Sidedefs)
				{
					// Add used textures to dictionary
					if(sd.LongHighTexture != MapSet.EmptyLongName)
					{
						usedtextures[sd.LongHighTexture] = true;

						//mxd. Part of long name support shennanigans
						if(texturenamesshorttofull.ContainsKey(sd.LongHighTexture))
							usedtextures[texturenamesshorttofull[sd.LongHighTexture]] = true;
					}
					if(sd.LongMiddleTexture != MapSet.EmptyLongName)
					{
						usedtextures[sd.LongMiddleTexture] = true;

						//mxd. Part of long name support shennanigans
						if(texturenamesshorttofull.ContainsKey(sd.LongMiddleTexture))
							usedtextures[texturenamesshorttofull[sd.LongMiddleTexture]] = true;
					}
					if(sd.LongLowTexture != MapSet.EmptyLongName)
					{
						usedtextures[sd.LongLowTexture] = true;

						//mxd. Part of long name support shennanigans
						if(texturenamesshorttofull.ContainsKey(sd.LongLowTexture))
							usedtextures[texturenamesshorttofull[sd.LongLowTexture]] = true;
					}
				}

				// Go through the map to find the used flats
				foreach(Sector s in General.Map.Map.Sectors)
				{
					// Add used flats to dictionary
					usedflats[s.LongFloorTexture] = false;
					usedflats[s.LongCeilTexture] = false;

					//mxd. Part of long name support shennanigans
					if(flatnamesshorttofull.ContainsKey(s.LongFloorTexture))
						usedflats[flatnamesshorttofull[s.LongFloorTexture]] = false;
					if(flatnamesshorttofull.ContainsKey(s.LongCeilTexture))
						usedflats[flatnamesshorttofull[s.LongCeilTexture]] = false;
				}
			}

			// Notify the background thread that it needs to update the images
			if(General.Map.Config.MixTexturesFlats)
			{
				// Set used on all textures
				foreach(KeyValuePair<long, ImageData> i in textures)
				{
                    if (i.Value.LoadFailed)
                        continue;
					i.Value.UsedInMap = usedtextures.ContainsKey(i.Key);
				}

				// Set used on all flats
				foreach(KeyValuePair<long, ImageData> i in flats)
				{
                    if (i.Value.LoadFailed)
                        continue;
                    i.Value.UsedInMap = usedtextures.ContainsKey(i.Key);
				}
			}
			//mxd. Use separate collections
			else
			{
				// Set used on all textures
				foreach(KeyValuePair<long, ImageData> i in textures)
				{
                    if (i.Value.LoadFailed)
                        continue;
                    i.Value.UsedInMap = usedtextures.ContainsKey(i.Key);
				}

				// Set used on all flats
				foreach(KeyValuePair<long, ImageData> i in flats)
				{
                    if (i.Value.LoadFailed)
                        continue;
                    i.Value.UsedInMap = usedflats.ContainsKey(i.Key);
				}
			}

            // Update icon
            General.MainWindow.UpdateStatus();
        }

#endregion

#region ================== mxd. Skybox Making

        internal void SetupSkybox()
		{
			// Get rid of old texture
			if(skybox != null) skybox.Dispose(); skybox = null;

			// Determine which texture name to use
			string skytex = string.Empty;
			if(!string.IsNullOrEmpty(mapinfo.Sky1))
			{
				skytex = mapinfo.Sky1;
			}
			// Use vanilla sky only when current map doesn't have a MAPINFO entry
			else if(!mapinfo.IsDefined)
			{
				if(General.Map.Config.DefaultSkyTextures.ContainsKey(General.Map.Options.CurrentName))
					skytex = General.Map.Config.DefaultSkyTextures[General.Map.Options.CurrentName];
				else
					skytex = General.GetByIndex(General.Map.Config.DefaultSkyTextures, 0).Value;
			}
			
			// Create sky texture
			if(!string.IsNullOrEmpty(skytex))
			{
				if(skyboxes.ContainsKey(skytex))
				{
					// Create cubemap texture
					skybox = (skyboxes[skytex].Textures.Count == 6 ? MakeSkyBox6(skyboxes[skytex]) : MakeSkyBox3(skyboxes[skytex]));
				}
				else
				{
					// Create classic texture
					Vector2D scale;
					Bitmap sky1 = GetTextureBitmap(skytex, out scale);
					if(sky1 != null)
					{
						// Special handling for wide skies. They are drawn from the east, but normal skyboxes are not,
						// to we have to rearrange the texture a bit (paste the right half to the left and vice versa)
						if(sky1.Width == 1024)
						{
							Bitmap tmpbmp = new Bitmap(sky1);
							Graphics g = Graphics.FromImage(tmpbmp);
							g.DrawImage(sky1, 512, 0, new Rectangle(0, 0, 512, sky1.Height), GraphicsUnit.Pixel);
							g.DrawImage(sky1, 0, 0, new Rectangle(512, 0, 512, sky1.Height), GraphicsUnit.Pixel);
							sky1 = tmpbmp;
						}

						// Double skies?
						if(mapinfo.DoubleSky)
						{
							Bitmap sky2 = GetTextureBitmap(mapinfo.Sky2);
							if(sky2 != null)
							{
								// Special handling for wide skies. They are drawn from the east, but normal skyboxes are not,
								// to we have to rearrange the texture a bit (paste the right half to the left and vice versa)
								if (sky2.Width == 1024)
								{
									Bitmap tmpbmp = new Bitmap(sky2);
									Graphics g = Graphics.FromImage(tmpbmp);
									g.DrawImage(sky2, 512, 0, new Rectangle(0, 0, 512, sky2.Height), GraphicsUnit.Pixel);
									g.DrawImage(sky2, 0, 0, new Rectangle(512, 0, 512, sky2.Height), GraphicsUnit.Pixel);
									sky2 = tmpbmp;
								}

								// Resize if needed
								if (sky2.Width != sky1.Width || sky2.Height != sky1.Height)
									ResizeImage(sky2, sky1.Width, sky1.Height);

								// Combine both textures. Sky2 is below Sky1
								using(Graphics g = Graphics.FromImage(sky2))
									g.DrawImageUnscaled(sky1, 0, 0);

								// Use the composite one
								sky1 = sky2;
							}
						}

						skybox = MakeClassicSkyBox(sky1, scale);
					}
				}
			}

			// Sky texture will be missing in ZDoom. Use internal texture
			if(skybox == null)
			{
				// Whine and moan
				if(string.IsNullOrEmpty(skytex))
					General.ErrorLogger.Add(ErrorType.Warning, "Skybox creation failed: Sky1 property is missing from the MAPINFO map definition");
				else
					General.ErrorLogger.Add(ErrorType.Warning, "Skybox creation failed: unable to load texture \"" + skytex + "\"");
				
				// Use the built-in texture
				ImageData tex = LoadInternalTexture("MissingSky3D.png");
                Bitmap bmp = tex.GetSkyboxBitmap();
                Bitmap sky;
                lock (bmp)
                {
                    sky = new Bitmap(bmp);
                }
				sky.RotateFlip(RotateFlipType.RotateNoneFlipX); // We don't want our built-in image mirrored...
				skybox = MakeClassicSkyBox(sky);
				tex.Dispose();
			}
		}

		//INFO: 1. Looks like GZDoom tries to tile a sky texture into a 1024 pixel width texture.
		//INFO: 2. If sky texture width <= height, it will be tiled to fit into 512 pixel height texture vertically.
		private static CubeTexture MakeClassicSkyBox(Bitmap img) { return MakeClassicSkyBox(img, new Vector2D(1.0f, 1.0f)); }
		private static CubeTexture MakeClassicSkyBox(Bitmap img, Vector2D scale)
		{
			// Get averaged top and bottom colors from the original image
			int tr = 0, tg = 0, tb = 0, br = 0, bg = 0, bb = 0;
			int defaultcolorsampleheight = (int)Math.Round(28 / scale.x);
			int colorsampleheight = Math.Max(1, Math.Min(defaultcolorsampleheight, img.Height / 2)); // TODO: is this value calculated from the image's height?
			int scaledwidth = (int)Math.Round(img.Width * scale.x);
			int scaledheight = (int)Math.Round(img.Height * scale.y);
			bool dogradients = colorsampleheight < img.Height / 2;

			for(int w = 0; w < img.Width; w++)
			{
				for(int h = 0; h < colorsampleheight; h++)
				{
					Color c = img.GetPixel(w, h);
					tr += c.R;
					tg += c.G;
					tb += c.B;

					c = img.GetPixel(w, img.Height - 1 - h);
					br += c.R;
					bg += c.G;
					bb += c.B;
				}
			}

			int pixelscount = img.Width * colorsampleheight;
			Color topcolor = Color.FromArgb(255, tr / pixelscount, tg / pixelscount, tb / pixelscount);
			Color bottomcolor = Color.FromArgb(255, br / pixelscount, bg / pixelscount, bb / pixelscount);

			// Make tiling image. Take custom scale into account
			int horiztiles = (int)Math.Ceiling(1024.0f / scaledwidth);
			int verttiles = scaledheight > 256 ? 1 : 2;

			Bitmap skyimage = new Bitmap((int)Math.Round(1024 / scale.x), img.Height * verttiles, img.PixelFormat);

			// Draw original image
			using(Graphics g = Graphics.FromImage(skyimage))
			{
				for(int w = 0; w < horiztiles; w++)
				{
					for(int h = 0; h < verttiles; h++)
					{
						g.DrawImage(img, img.Width * w, img.Height * h);
					}
				}
			}

			// Make top and bottom images
			int capsimgsize = (int)Math.Round(16 / scale.x);
			Bitmap topimg = new Bitmap(capsimgsize, capsimgsize);
			using(Graphics g = Graphics.FromImage(topimg))
			{
				using(SolidBrush b = new SolidBrush(topcolor))
					g.FillRectangle(b, 0, 0, capsimgsize, capsimgsize);
			}

			Bitmap bottomimg = new Bitmap(capsimgsize, capsimgsize);
			using(Graphics g = Graphics.FromImage(bottomimg))
			{
				using(SolidBrush b = new SolidBrush(bottomcolor))
					g.FillRectangle(b, 0, 0, capsimgsize, capsimgsize);
			}

			// Apply top/bottom gradients
			if(dogradients)
			{
				using(Graphics g = Graphics.FromImage(skyimage))
				{
					Rectangle area = new Rectangle(0, 0, skyimage.Width, colorsampleheight);
					using(LinearGradientBrush b = new LinearGradientBrush(area, topcolor, Color.FromArgb(0, topcolor), 90f))
					{
						g.FillRectangle(b, area);
					}

					area = new Rectangle(0, skyimage.Height - colorsampleheight, skyimage.Width, colorsampleheight);
					using(LinearGradientBrush b = new LinearGradientBrush(area, Color.FromArgb(0, bottomcolor), bottomcolor, 90f))
					{
						area.Y += 1;
						g.FillRectangle(b, area);
					}
				}
			}

			// Rendering errors occure when image size exceeds MAX_SKYTEXTURE_SIZE...
			if(skyimage.Width > MAX_SKYTEXTURE_SIZE || skyimage.Height > MAX_SKYTEXTURE_SIZE)
			{
				float scaler = (float)MAX_SKYTEXTURE_SIZE / Math.Max(skyimage.Width, skyimage.Height);
				skyimage = ResizeImage(skyimage, (int)Math.Round(skyimage.Width * scaler), (int)Math.Round(skyimage.Height * scaler));
			}

			// Make custom rendertarget
			const int cubemaptexsize = 1024;
			Texture rendertarget = new Texture(cubemaptexsize, cubemaptexsize, TextureFormat.Rgba8);

            // Start rendering
            General.Map.Graphics.StartRendering(true, new Color4(), rendertarget, true);

			// Load the skysphere model...
			BoundingBoxSizes bbs = new BoundingBoxSizes();
			Stream modeldata = General.ThisAssembly.GetManifestResourceStream("CodeImp.DoomBuilder.Resources.SkySphere.md3");
			ModelLoadResult meshes = MD3ModelLoader.Load(ref bbs, new Dictionary<int, string>(), modeldata, 0);
			if(meshes.Meshes.Count != 3) throw new Exception("Skybox creation failed: " 
				+ (string.IsNullOrEmpty(meshes.Errors) ? "skybox model must contain 3 surfaces" : meshes.Errors));

			// Make skysphere textures...
			Texture texside = TextureFromBitmap(skyimage);
			Texture textop = TextureFromBitmap(topimg);
			Texture texbottom = TextureFromBitmap(bottomimg);

			// Calculate model scaling (gl.skydone.cpp:RenderDome() in GZDoom)
			float yscale;
			if(scaledheight < 128) yscale = 128 / 230.0f;
			else if(scaledheight < 200) yscale = scaledheight / 230.0f;
			else if(scaledheight < 241) yscale = 1.0f + ((scaledheight - 200.0f) / 200.0f) * 1.17f;
			else yscale = 1.2f * 1.17f;

			// I guess my sky model doesn't exactly match the one GZDoom generates...
			yscale *= 1.65f;

			// Make cubemap texture
			CubeTexture cubemap = new CubeTexture(General.Map.Graphics, cubemaptexsize);

            // Set render settings...
            General.Map.Graphics.SetZEnable(false);
            General.Map.Graphics.SetCullMode(Cull.None);
            General.Map.Graphics.SetSamplerState(TextureAddress.Clamp);
			
			// Setup matrices
			Vector3f offset = new Vector3f(0f, 0f, -1.8f); // Sphere size is 10 mu
			Matrix mworld = Matrix.Translation(offset) * Matrix.Scaling(1.0f, 1.0f, yscale);
			Matrix mprojection = Matrix.PerspectiveFov((float)Angle2D.PIHALF, 1.0f, 0.5f, 100.0f);

            // Place camera at origin
            General.Map.Graphics.SetUniform(UniformName.campos, new Vector4f());

            // Begin fullbright shaderpass
            General.Map.Graphics.SetShader(ShaderName.world3d_fullbright);
            General.Map.Graphics.SetUniform(UniformName.fogsettings, new Vector4f(-1.0f));

            // Render to the six faces of the cube map
            for (int i = 0; i < 6; i++)
			{
				Matrix faceview = GetCubeMapViewMatrix((CubeMapFace)i);
                General.Map.Graphics.SetUniform(UniformName.world, mworld);
                General.Map.Graphics.SetUniform(UniformName.view, faceview);
                General.Map.Graphics.SetUniform(UniformName.projection, mprojection);

				// Render the skysphere meshes
				for(int j = 0; j < meshes.Meshes.Count; j++)
				{
					// Set appropriate texture
					switch(meshes.Skins[j])
					{
						case "top.png": General.Map.Graphics.SetTexture(textop); break;
						case "bottom.png": General.Map.Graphics.SetTexture(texbottom); break;
						case "side.png": General.Map.Graphics.SetTexture(texside); break;
						default: throw new Exception("Unexpected skin!");
					}

					// Render mesh
					meshes.Meshes[j].Draw(General.Map.Graphics);
				}

                General.Map.Graphics.CopyTexture(cubemap, (CubeMapFace)i);
			}

			// End rendering
			General.Map.Graphics.FinishRendering();

			// Dispose unneeded stuff
			rendertarget.Dispose();
			textop.Dispose();
			texside.Dispose();
			texbottom.Dispose();

			// Dispose skybox meshes
			foreach(Mesh m in meshes.Meshes) m.Dispose();

			// All done...
			return cubemap;
		}

		// Makes CubeTexture from 6 images
		private CubeTexture MakeSkyBox6(SkyboxInfo info)
		{
			// Gather images. They should be defined in this order: North, East, South, West, Top, Bottom
			Bitmap[] sides = new Bitmap[6];
			int targetsize = 0;
			for(int i = 0; i < info.Textures.Count; i++)
			{
				sides[i] = GetTextureBitmap(info.Textures[i]);
				if(sides[i] != null)
				{
					targetsize = Math.Max(targetsize, Math.Max(sides[i].Width, sides[i].Height));
				}
				else
				{
					General.ErrorLogger.Add(ErrorType.Error, "Unable to create \"" + info.Name + "\" skybox: unable to find \"" + info.Textures[i] + "\" texture");
					return null;
				}
			}

			// All images must be square and have the same size
			if(targetsize == 0)
			{
				General.ErrorLogger.Add(ErrorType.Error, "Unable to create \"" + info.Name + "\" skybox: invalid texture size");
				return null;
			}

			// Make it Po2
			targetsize = Math.Min(General.NextPowerOf2(targetsize), MAX_SKYTEXTURE_SIZE);

			for(int i = 0; i < sides.Length; i++)
			{
				if(sides[i].Width != targetsize || sides[i].Height != targetsize)
					sides[i] = ResizeImage(sides[i], targetsize, targetsize);
			}

			// Return cubemap texture
			return MakeSkyBox(sides, targetsize, info.FlipTop);
		}

		// Makes CubeTexture from 3 images
		private CubeTexture MakeSkyBox3(SkyboxInfo info)
		{
			// Gather images. They should be defined in this order: Sides, Top, Bottom
			Bitmap[] sides = new Bitmap[6];
			int targetsize = 0;

			// Create NWSE images from the side texture
			Bitmap sideimg = GetTextureBitmap(info.Textures[0]);
			if(sideimg != null)
			{
				// This should be 4x1 format image. If it's not, we'll need to resize it
				targetsize = Math.Max(sideimg.Width / 4, sideimg.Height);

				if(targetsize == 0)
				{
					General.ErrorLogger.Add(ErrorType.Error, "Unable to create \"" + info.Name + "\" skybox: invalid texture size");
					return null;
				}

				// Make it Po2
				targetsize = Math.Min(General.NextPowerOf2(targetsize), MAX_SKYTEXTURE_SIZE);

				// Resize if needed
				if(sideimg.Width != targetsize * 4 || sideimg.Height != targetsize)
				{
					sideimg = ResizeImage(sideimg, targetsize * 4, targetsize);
				}

				// Chop into tiny pieces
				for(int i = 0; i < 4; i++)
				{
					// Create square image
					Bitmap img = new Bitmap(targetsize, targetsize);
					using(Graphics g = Graphics.FromImage(img))
					{
						// Copy area from the side image
						g.DrawImage(sideimg, 0, 0, new Rectangle(targetsize * i, 0, targetsize, targetsize), GraphicsUnit.Pixel);
					}

					// Add to collection
					sides[i] = img;
				}
			}

			// Sanity check...
			if(sides[0] == null || sides[1] == null || sides[2] == null || sides[3] == null)
			{
				General.ErrorLogger.Add(ErrorType.Error, "Unable to create \"" + info.Name + "\" skybox: unable to find \"" + info.Textures[0] + "\" texture");
				return null;
			}

			// Create top
			Bitmap topimg = GetTextureBitmap(info.Textures[1]);
			if(topimg != null)
			{
				// Resize if needed
				if(topimg.Width != targetsize || topimg.Height != targetsize)
					topimg = ResizeImage(topimg, targetsize, targetsize);

				// Add to collection
				sides[4] = topimg;
			}
			else
			{
				General.ErrorLogger.Add(ErrorType.Error, "Unable to create \"" + info.Name + "\" skybox: unable to find \"" + info.Textures[1] + "\" texture");
				return null;
			}

			// Create bottom
			Bitmap bottomimg = GetTextureBitmap(info.Textures[2]);
			if(bottomimg != null)
			{
				// Resize if needed
				if(bottomimg.Width != targetsize || bottomimg.Height != targetsize)
					bottomimg = ResizeImage(bottomimg, targetsize, targetsize);

				// Add to collection
				sides[5] = bottomimg;
			}
			else
			{
				General.ErrorLogger.Add(ErrorType.Error, "Unable to create \"" + info.Name + "\" skybox: unable to find \"" + info.Textures[2] + "\" texture");
				return null;
			}

			// Return cubemap texture
			return MakeSkyBox(sides, targetsize, info.FlipTop);
		}

		// Makes CubeTexture from 6 images.
		// sides[] must contain 6 square Po2 images in this order: North, East, South, West, Top, Bottom
		private static CubeTexture MakeSkyBox(Bitmap[] sides, int targetsize, bool fliptop)
		{
			CubeTexture cubemap = new CubeTexture(General.Map.Graphics, targetsize);

			// Draw faces
			sides[3].RotateFlip(RotateFlipType.Rotate90FlipNone);
			DrawCubemapFace(cubemap, CubeMapFace.NegativeX, sides[3]); // West

			DrawCubemapFace(cubemap, CubeMapFace.NegativeY, sides[0]); // North

			sides[1].RotateFlip(RotateFlipType.Rotate270FlipNone);
			DrawCubemapFace(cubemap, CubeMapFace.PositiveX, sides[1]); // East

			sides[2].RotateFlip(RotateFlipType.Rotate180FlipNone);
			DrawCubemapFace(cubemap, CubeMapFace.PositiveY, sides[2]); // South

			if(!fliptop) sides[4].RotateFlip(RotateFlipType.Rotate180FlipX);
			DrawCubemapFace(cubemap, CubeMapFace.PositiveZ, sides[4]); // Top

			sides[5].RotateFlip(RotateFlipType.Rotate180FlipNone);
			DrawCubemapFace(cubemap, CubeMapFace.NegativeZ, sides[5]); // Bottom

			// All done...
			return cubemap;
		}

		private static void DrawCubemapFace(CubeTexture texture, CubeMapFace face, Bitmap image)
		{
            General.Map.Graphics.SetPixels(texture, face, image);
		}

		private static Bitmap ResizeImage(Image image, int width, int height)
		{
			var destrect = new Rectangle(0, 0, width, height);
			var destimage = new Bitmap(width, height);

			destimage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

			using(var graphics = Graphics.FromImage(destimage))
			{
				graphics.CompositingMode = CompositingMode.SourceCopy;
				graphics.CompositingQuality = CompositingQuality.HighQuality;
				graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
				graphics.SmoothingMode = SmoothingMode.HighQuality;
				graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

				using(var wrapmode = new ImageAttributes())
				{
					wrapmode.SetWrapMode(WrapMode.TileFlipXY);
					graphics.DrawImage(image, destrect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapmode);
				}
			}

			return destimage;
		}

		private static Matrix GetCubeMapViewMatrix(CubeMapFace face)
		{
			Vector3f lookdir, updir;

			switch(face)
			{
				case CubeMapFace.PositiveX:
					lookdir = new Vector3f(-1.0f, 0.0f, 0.0f);
					updir = new Vector3f(0.0f, 1.0f, 0.0f);
					break;

				case CubeMapFace.NegativeX:
					lookdir = new Vector3f(1.0f, 0.0f, 0.0f);
					updir = new Vector3f(0.0f, 1.0f, 0.0f);
					break;

				case CubeMapFace.PositiveY:
					lookdir = new Vector3f(0.0f, -1.0f, 0.0f);
					updir = new Vector3f(0.0f, 0.0f, 1.0f);
					break;

				case CubeMapFace.NegativeY:
					lookdir = new Vector3f(0.0f, 1.0f, 0.0f);
					updir = new Vector3f(0.0f, 0.0f, -1.0f);
					break;

				case CubeMapFace.PositiveZ:
					lookdir = new Vector3f(0.0f, 0.0f, 1.0f);
					updir = new Vector3f(0.0f, 1.0f, 0.0f);
					break;

				case CubeMapFace.NegativeZ:
					lookdir = new Vector3f(0.0f, 0.0f, -1.0f);
					updir = new Vector3f(0.0f, 1.0f, 0.0f);
					break;

				default:
					throw new Exception("Unknown CubeMapFace!");
			}

			Vector3f eye = new Vector3f();
			return Matrix.LookAt(eye, lookdir, updir);
		}

		private static Texture TextureFromBitmap(Image image)
		{
            using (var bitmap = new Bitmap(image))
            {
                return new Texture(General.Map.Graphics, bitmap);
            }
		}
		
#endregion
	}
}
