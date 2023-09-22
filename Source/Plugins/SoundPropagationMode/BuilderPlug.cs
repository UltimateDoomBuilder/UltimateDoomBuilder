
#region ================== Copyright (c) 2014 Boris Iwanski

/*
 * Copyright (c) 2014 Boris Iwanski
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
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using CodeImp.DoomBuilder.Editing;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Plugins;
using CodeImp.DoomBuilder.Rendering;

#endregion

namespace CodeImp.DoomBuilder.SoundPropagationMode
{
	//
	// MANDATORY: The plug!
	// This is an important class to the Doom Builder core. Every plugin must
	// have exactly 1 class that inherits from Plug. When the plugin is loaded,
	// this class is instantiated and used to receive events from the core.
	// Make sure the class is public, because only public classes can be seen
	// by the core.
	//

	internal class ToastMessages
	{
		public static readonly string SOUNDPROPAGATIONMODE = "soundpropagationmode";
	}

	public class BuilderPlug : Plug
	{
		#region ================== Constants

		internal const int SOUND_ENVIROMNEMT_THING_TYPE = 9048; //mxd
		internal const float LINE_LENGTH_SCALER = 0.001f; //mxd

		#endregion

		#region ================== Variables

		// Interface
		private MenusForm menusform;

		// Colors
		private PixelColor highlightcolor;
		private PixelColor level1color;
		private PixelColor level2color;
		private PixelColor blocksoundcolor;
		private PixelColor nosoundcolor;

		private List<PixelColor> distinctcolors;
		private List<Bitmap> distincticons; //mxd 
		private List<SoundEnvironment> soundenvironments;
		private List<Linedef> blockinglinedefs;
		private FlatVertex[] overlayGeometry;
		private bool soundenvironmentisupdated;
		private bool dataisdirty;

		#endregion

		#region ================== Properties

		// Interface
		public MenusForm MenusForm { get { return menusform; } }
		internal List<Bitmap> DistinctIcons { get { return distincticons; } } //mxd
		internal List<PixelColor> DistinctColors { get { return distinctcolors; } } //mxd

		// Colors
		public PixelColor HighlightColor { get { return highlightcolor; } set { highlightcolor = value; } }
		public PixelColor Level1Color { get { return level1color; } set { level1color = value; } }
		public PixelColor Level2Color { get { return level2color; } set { level2color = value; } }
		public PixelColor BlockSoundColor { get { return blocksoundcolor; } set { blocksoundcolor = value; } }
		public PixelColor NoSoundColor { get { return nosoundcolor; } set { nosoundcolor = value; } }

		public List<SoundEnvironment> SoundEnvironments { get { return soundenvironments; } set { soundenvironments = value; } }
		public List<Linedef> BlockingLinedefs { get { return blockinglinedefs; } set { blockinglinedefs = value; } }
		public FlatVertex[] OverlayGeometry { get { return overlayGeometry; } set { overlayGeometry = value; } }
		public bool SoundEnvironmentIsUpdated { get { return soundenvironmentisupdated; } }
		public bool DataIsDirty { get { return dataisdirty; } set { dataisdirty = value; } }

		#endregion

 		// Static instance. We can't use a real static class, because BuilderPlug must
		// be instantiated by the core, so we keep a static reference. (this technique
		// should be familiar to object-oriented programmers)
		private static BuilderPlug me;

		// Static property to access the BuilderPlug
		public static BuilderPlug Me { get { return me; } }

      	// This plugin relies on some functionality that wasn't there in older versions
		public override int MinimumRevision { get { return 2201; } }

		// This event is called when the plugin is initialized
		public override void OnInitialize()
		{
			base.OnInitialize();

			highlightcolor = PixelColor.FromInt(General.Settings.ReadPluginSetting("highlightcolor", new PixelColor(255, 0, 192, 0).ToInt()));
			level1color = PixelColor.FromInt(General.Settings.ReadPluginSetting("level1color", new PixelColor(255, 0, 255, 0).ToInt()));
			level2color = PixelColor.FromInt(General.Settings.ReadPluginSetting("level2color", new PixelColor(255, 255, 255, 0).ToInt()));
			nosoundcolor = PixelColor.FromInt(General.Settings.ReadPluginSetting("nosoundcolor", new PixelColor(255, 160, 160, 160).ToInt()));
			blocksoundcolor = PixelColor.FromInt(General.Settings.ReadPluginSetting("blocksoundcolor", new PixelColor(255, 255, 0, 0).ToInt()));

			distinctcolors = new List<PixelColor> 
			{
				PixelColor.FromInt(0x84d5a4),
				PixelColor.FromInt(0xc059cb),
				PixelColor.FromInt(0xd0533d),
				PixelColor.FromInt(0x415354),
				PixelColor.FromInt(0xcea953),
				PixelColor.FromInt(0x91d44b),
				PixelColor.FromInt(0xcd5b89),
				PixelColor.FromInt(0xa8b6c0),
				PixelColor.FromInt(0x797ecb),
				PixelColor.FromInt(0x567539),
				PixelColor.FromInt(0x72422f),
				PixelColor.FromInt(0x5d3762),
				PixelColor.FromInt(0xffed6f),
				PixelColor.FromInt(0xccebc5),
				PixelColor.FromInt(0xbc80bd),
				PixelColor.FromInt(0xd9d9d9),
				PixelColor.FromInt(0xfccde5),
				PixelColor.FromInt(0x80b1d3),
				PixelColor.FromInt(0xfdb462),
				PixelColor.FromInt(0xb3de69),
				PixelColor.FromInt(0xfb8072),
				PixelColor.FromInt(0xbebada),
				PixelColor.FromInt(0xffffb3),
				PixelColor.FromInt(0x8dd3c7),
			};

			//mxd. Create coloured icons
			distincticons = new List<Bitmap>(distinctcolors.Count);
			foreach(PixelColor color in distinctcolors)
			{
				distincticons.Add(MakeTintedImage(Properties.Resources.Status0, color));
			}

			soundenvironments = new List<SoundEnvironment>();
			blockinglinedefs = new List<Linedef>();
			soundenvironmentisupdated = false;
			dataisdirty = true;

			// This binds the methods in this class that have the BeginAction
			// and EndAction attributes with their actions. Without this, the
			// attributes are useless. Note that in classes derived from EditMode
			// this is not needed, because they are bound automatically when the
			// editing mode is engaged.
            General.Actions.BindMethods(this);

			menusform = new MenusForm();

  			// TODO: Add DB2 version check so that old DB2 versions won't crash
			// General.ErrorLogger.Add(ErrorType.Error, "zomg!");

			// Keep a static reference
            me = this;

			// Register toasts
			General.ToastManager.RegisterToast(ToastMessages.SOUNDPROPAGATIONMODE, "Sound propagation mode", "Toasts related to sound propagation mode");
		}

		public override void OnMapOpenBegin()
		{
			base.OnMapOpenBegin();
			ResetData();
		}

		public override void OnMapNewBegin()
		{
			base.OnMapNewBegin();
			ResetData();
		}

		public override void OnMapSaveBegin(SavePurpose purpose)
		{
			base.OnMapSaveBegin(purpose);
			soundenvironmentisupdated = false;
		}

		public override void OnEditEngage(EditMode oldmode, EditMode newmode)
		{
			base.OnEditEngage(oldmode, newmode);
			ResetData();
		}

		// This is called when the plugin is terminated
		public override void Dispose()
		{
			base.Dispose();

			//mxd
			foreach(Bitmap b in distincticons) b.Dispose();

			// This must be called to remove bound methods for actions.
            General.Actions.UnbindMethods(this);
        }

		// Resets all data. This will trigger both rediscovering sound environments and recalculating
		// sound propagation domains
		private void ResetData()
		{
			dataisdirty = true;
			soundenvironmentisupdated = false;
			soundenvironments = new List<SoundEnvironment>();
			blockinglinedefs = new List<Linedef>();
		}

		public void UpdateSoundEnvironments(object sender, DoWorkEventArgs e)
		{
			List<Sector> sectorstocheck = new List<Sector>();
			List<Sector> checkedsectors = new List<Sector>();
			List<Sector> allsectors = new List<Sector>();
			BackgroundWorker worker = sender as BackgroundWorker;
			List<FlatVertex> vertsList = new List<FlatVertex>();
			Dictionary<Thing, PixelColor> secolor = new Dictionary<Thing, PixelColor>();
			Dictionary<Thing, int> senumber = new Dictionary<Thing, int>();

			soundenvironments.Clear();
			blockinglinedefs.Clear();
			SoundEnvironmentMode.SoundEnvironmentPanel.BeginUpdate(); //mxd

			// Keep track of all the sectors in the map. Sectors that belong to a sound environment
			// will be removed from the list, so in the end only sectors that don't belong to any
			// sound environment will be in this list
			foreach(Sector s in General.Map.Map.Sectors) allsectors.Add(s);

			List<Thing> soundenvironmenthings = GetSoundEnvironmentThings(General.Map.Map.Sectors.ToList());
			int numthings = soundenvironmenthings.Count;

			// Assign each thing a color and a number, so each sound environment will always have the same color
			// and id, no matter in what order they are discovered
			for(int i = 0; i < soundenvironmenthings.Count; i++)
			{
				//mxd. Make sure same environments use the same color
				int seid = (Math.Abs(soundenvironmenthings[i].Args[0])%256 << 8) + Math.Abs(soundenvironmenthings[i].Args[1])%256;
				secolor[soundenvironmenthings[i]] = distinctcolors[seid % distinctcolors.Count];
				senumber.Add(soundenvironmenthings[i], i + 1);
			}

			while(soundenvironmenthings.Count > 0 && !worker.CancellationPending)
			{
				// Sort things by distance to center of the screen, so that sound environments the user want to look at will (hopefully) be discovered first
				Vector2D center = General.Map.Renderer2D.DisplayToMap(new Vector2D(General.Interface.Display.Width / 2f, General.Interface.Display.Height / 2f));
				soundenvironmenthings = soundenvironmenthings.OrderBy(o => Math.Abs(Vector2D.Distance(center, o.Position))).ToList();

				Thing thing = soundenvironmenthings[0];
				if(thing.Sector == null) thing.DetermineSector();

				// Ignore things that are outside the map
				if(thing.Sector == null)
				{
					soundenvironmenthings.Remove(thing);
					continue;
				}

				SoundEnvironment environment = new SoundEnvironment();

				// Add initial sector. Additional adjacant sectors will be added later
				// as they are discovered
				sectorstocheck.Add(thing.Sector);

				while(sectorstocheck.Count > 0)
				{
					Sector sector = sectorstocheck[0];

					if(!environment.Sectors.Contains(sector)) environment.Sectors.Add(sector);
					if(!checkedsectors.Contains(sector)) checkedsectors.Add(sector);

					sectorstocheck.Remove(sector);
					allsectors.Remove(sector);

					// Find adjacant sectors and add them to the list of sectors to check if necessary
					foreach(Sidedef sd in sector.Sidedefs)
					{
						if(LinedefBlocksSoundEnvironment(sd.Line))
						{
							if(!environment.Linedefs.Contains(sd.Line)) environment.Linedefs.Add(sd.Line);
							continue;
						}

						if(sd.Other == null) continue;

						Sector oppositesector = (sd.Line.Front.Sector == sector ? sd.Line.Back.Sector : sd.Line.Front.Sector);

						if(!sectorstocheck.Contains(oppositesector) && !checkedsectors.Contains(oppositesector))
							sectorstocheck.Add(oppositesector);
					}
				}

				// Get all things that are in the current sound environment...
				environment.Things = GetSoundEnvironmentThings(environment.Sectors);

				// ... and remove them from the list of sound environment things to check, because we know that
				// they already belong to a sound environment
				foreach(Thing t in environment.Things)
				{
					if(soundenvironmenthings.Contains(t)) soundenvironmenthings.Remove(t);
				}

				// Set color and id of the sound environment
				environment.Color = secolor[environment.Things[0]];
				environment.ID = senumber[environment.Things[0]];

				// Create the data for the overlay geometry
				List<FlatVertex> severtslist = new List<FlatVertex>(environment.Sectors.Count * 3); //mxd
				foreach(Sector s in environment.Sectors)
				{
					FlatVertex[] fv = new FlatVertex[s.FlatVertices.Length];
					s.FlatVertices.CopyTo(fv, 0);
					for(int j = 0; j < fv.Length; j++) fv[j].c = environment.Color.WithAlpha(128).ToInt();

					vertsList.AddRange(fv);
					severtslist.AddRange(fv); //mxd

					// Get all Linedefs that will block sound environments
					foreach(Sidedef sd in s.Sidedefs)
					{
						if(!LinedefBlocksSoundEnvironment(sd.Line)) continue;
						lock(blockinglinedefs)
						{
							blockinglinedefs.Add(sd.Line);
						}
					}
				}

				//mxd. Store sector environment verts
				environment.SectorsGeometry = severtslist.ToArray();

				// Update the overlay geometry with the newly added sectors
				if(overlayGeometry == null)
				{
					overlayGeometry = vertsList.ToArray();
				}
				else
				{
					lock(overlayGeometry)
					{
						overlayGeometry = vertsList.ToArray();
					}
				}

				environment.Things = environment.Things.OrderBy(o => o.Index).ToList();
				environment.Linedefs = environment.Linedefs.OrderBy(o => o.Index).ToList();

				//mxd. Find the first non-dormant thing
				Thing activeenv = null;
				foreach(Thing t in environment.Things) 
				{
					if(!ThingDormant(t)) 
					{
						activeenv = t;
						break;
					}
				}

				//mxd. Update environment name
				if(activeenv != null) 
				{
					foreach(KeyValuePair<string, KeyValuePair<int, int>> group in General.Map.Data.Reverbs) 
					{
						if(group.Value.Key == activeenv.Args[0] && group.Value.Value == activeenv.Args[1]) 
						{
							environment.Name = group.Key + " (" + activeenv.Args[0] + " " + activeenv.Args[1] + ")";
							break;
						}
					}

					//mxd. No suitable name found?..
					if(environment.Name == SoundEnvironment.DEFAULT_NAME)
					{
						environment.Name += " (" + activeenv.Args[0] + " " + activeenv.Args[1] + ")";
					}
				}

				//mxd. Still no suitable name?..
				if(environment.Name == SoundEnvironment.DEFAULT_NAME) environment.Name += " " + environment.ID;

				lock(soundenvironments)
				{
					soundenvironments.Add(environment);
				}

				// Tell the worker that discovering a sound environment is finished. This will update the tree view, and also
				// redraw the interface, so the sectors of this sound environment are colored
				worker.ReportProgress((int)((1.0f - (soundenvironmenthings.Count / (float)numthings)) * 100), environment);
			}

			// Create overlay geometry for sectors that don't belong to a sound environment
			foreach(Sector s in allsectors)
			{
				FlatVertex[] fv = new FlatVertex[s.FlatVertices.Length];
				s.FlatVertices.CopyTo(fv, 0);
				for(int j = 0; j < fv.Length; j++) fv[j].c = NoSoundColor.WithAlpha(128).ToInt();
				vertsList.AddRange(fv);
			}

			if(overlayGeometry == null)
			{
				overlayGeometry = vertsList.ToArray();
			}
			else
			{
				lock(overlayGeometry)
				{
					overlayGeometry = vertsList.ToArray();
				}
			}

			soundenvironmentisupdated = true;
			SoundEnvironmentMode.SoundEnvironmentPanel.EndUpdate(); //mxd
		}

		private static List<Thing> GetSoundEnvironmentThings(List<Sector> sectors)
		{
			List<Thing> things = new List<Thing>();

			foreach(Thing thing in General.Map.Map.Things)
			{
				// SoundEnvironment thing, see http://zdoom.org/wiki/Classes:SoundEnvironment
				if(thing.Type != SOUND_ENVIROMNEMT_THING_TYPE) continue;
				if(thing.Sector == null) thing.DetermineSector();
				if(thing.Sector != null && sectors.Contains(thing.Sector)) things.Add(thing);
			}

			return things;
		}

		private static bool LinedefBlocksSoundEnvironment(Linedef linedef)
		{
			if(General.Map.UDMF) return linedef.IsFlagSet(SoundEnvironmentMode.ZoneBoundaryFlag); //mxd. Fancier this way :)
			
			// In Hexen format the line must have action 121 (Line_SetIdentification) and bit 1 of
			// the second argument set (see http://zdoom.org/wiki/Line_SetIdentification)
			return (linedef.Action == 121 && (linedef.Args[1] & 1) == 1); //mxd. Fancier this way :)
		}

		//mxd
		internal static bool ThingDormant(Thing thing) 
		{
			return thing.IsFlagSet(General.Map.UDMF ? "dormant" : "14");
		}

		//mxd
		internal static void SetThingDormant(Thing thing, bool dormant) 
		{
			thing.SetFlag(General.Map.UDMF ? "dormant" : "14", dormant);
		}

		//mxd. Based on http://www.getcodesamples.com/src/792A0BB0/6CD40E7B 
		private static Bitmap MakeTintedImage(Bitmap source, PixelColor tint) 
		{
			BitmapData sourcedata = source.LockBits(new Rectangle(0, 0, source.Width, source.Height),
									ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

			byte[] pixelBuffer = new byte[sourcedata.Stride * sourcedata.Height];
			Marshal.Copy(sourcedata.Scan0, pixelBuffer, 0, pixelBuffer.Length);
			source.UnlockBits(sourcedata);

			//Translate tint color to 0.0-1.0 range
			float redtint = tint.r / 256.0f;
			float greentint = tint.g / 256.0f; 
			float bluetint = tint.b / 256.0f;

			for(int k = 0; k + 4 < pixelBuffer.Length; k += 4) 
			{
				float blue = 60 + pixelBuffer[k] * bluetint;
				float green = 60 + pixelBuffer[k + 1] * greentint;
				float red = 60 + pixelBuffer[k + 2] * redtint;

				pixelBuffer[k] = (byte)Math.Min(255, blue);
				pixelBuffer[k + 1] = (byte)Math.Min(255, green);
				pixelBuffer[k + 2] = (byte)Math.Min(255, red);
			}

			Bitmap result = new Bitmap(source.Width, source.Height);
			BitmapData resultdata = result.LockBits(new Rectangle(0, 0, result.Width, result.Height),
									ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

			Marshal.Copy(pixelBuffer, 0, resultdata.Scan0, pixelBuffer.Length);
			result.UnlockBits(resultdata);

			return result;
		}
	}
}
