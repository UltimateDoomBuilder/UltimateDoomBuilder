
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
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.GZBuilder.Data;
using CodeImp.DoomBuilder.IO;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Types;
using CodeImp.DoomBuilder.VisualModes;
using CodeImp.DoomBuilder.GZBuilder;

#endregion

namespace CodeImp.DoomBuilder.Map
{
	public sealed class Thing : SelectableElement, ITaggedMapElement
	{
		#region ================== Constants

		public const int NUM_ARGS = 5;
		public static readonly HashSet<ThingRenderMode> AlignableRenderModes = new HashSet<ThingRenderMode>
		{
			ThingRenderMode.FLATSPRITE, ThingRenderMode.WALLSPRITE, ThingRenderMode.MODEL
		}; 

		#endregion

		#region ================== Variables

		// Map
		private MapSet map;

		// Sector
		private Sector sector;

		// List items
		private LinkedListNode<Thing> selecteditem;
		
		// Properties
		private int type;
        private GZGeneral.LightData dynamiclighttype;
		private Vector3D pos;
		private int angledoom;		// Angle as entered / stored in file
		private double anglerad;		// Angle in radians
		private Dictionary<string, bool> flags;
		private ushort rawflags; // The actual flags bitmap that also might include unknown flags
		private int tag;
		private int action;
		private int[] args;
		private double scaleX; //mxd
		private double scaleY; //mxd
		private SizeF spritescale; //mxd
		private int pitch; //mxd. Used in model rendering
		private int roll; //mxd. Used in model rendering
		private double pitchrad; //mxd
		private double rollrad; //mxd
		private bool highlighted; //mxd

		//mxd. GZDoom rendering properties
		private ThingRenderMode rendermode;
		private bool rollsprite; //mxd

		// Configuration
		private float size;
		private float rendersize;
		private float height; //mxd
		private PixelColor color;
		private bool fixedsize;
		private bool directional; //mxd. If true, we need to render an arrow

		// biwa. This should only ever be used for temporary player starts for the "test from current position" action
		private bool recordundo;

        // Rendering
        private int lastProcessed;

        #endregion

        #region ================== Properties

        public MapSet Map { get { return map; } }
		public int Type { get { return type; } set { BeforePropsChange(); type = value; } } //mxd
        public GZGeneral.LightData DynamicLightType { get { return dynamiclighttype; } internal set { BeforePropsChange(); dynamiclighttype = value; } }
		public Vector3D Position { get { return pos; } }
		public double ScaleX { get { return scaleX; } } //mxd. This is UDMF property, not actual scale!
		public double ScaleY { get { return scaleY; } } //mxd. This is UDMF property, not actual scale!
		public int Pitch { get { return pitch; } } //mxd
		public double PitchRad { get { return pitchrad; } }
		public int Roll { get { return roll; } } //mxd
		public double RollRad { get { return rollrad; } }
		public SizeF ActorScale { get { return spritescale; } } //mxd. Actor scale set in DECORATE
		public double Angle { get { return anglerad; } }
		public int AngleDoom { get { return angledoom; } }
		internal Dictionary<string, bool> Flags { get { return flags; } }
		public ushort RawFlags { get { return rawflags; } }
		public int Action { get { return action; } set { BeforePropsChange(); action = value; } }
		public int[] Args { get { return args; } }
		public float Size { get { return size; } }
		public float RenderSize { get { return rendersize; } }
		public float Height { get { return height; } } //mxd
		public PixelColor Color { get { return color; } }
		public bool FixedSize { get { return fixedsize; } }
		public int Tag { get { return tag; } set { BeforePropsChange(); tag = value; if((tag < General.Map.FormatInterface.MinTag) || (tag > General.Map.FormatInterface.MaxTag)) throw new ArgumentOutOfRangeException("Tag", "Invalid tag number"); } }
		public Sector Sector { get { return sector; } }
		public ThingRenderMode RenderMode { get { return rendermode; } } //mxd
		public bool IsDirectional { get { return directional; } } //mxd
		public bool Highlighted { get { return highlighted; } set { highlighted = value; } } //mxd
        internal int LastProcessed { get { return lastProcessed; } set { lastProcessed = value; } }

        #endregion

        #region ================== Constructor / Disposer

        // Constructor
        internal Thing(MapSet map, int listindex, bool recordundo = true)
		{
			// Initialize
			this.elementtype = MapElementType.THING; //mxd
			this.map = map;
			this.listindex = listindex;
			this.flags = new Dictionary<string, bool>(StringComparer.Ordinal);
			this.args = new int[NUM_ARGS];
			this.scaleX = 1.0f;
			this.scaleY = 1.0f;
			this.spritescale = new SizeF(1.0f, 1.0f);
			this.recordundo = recordundo;
			
			if(map == General.Map.Map && recordundo)
				General.Map.UndoRedo.RecAddThing(this);
			
			// We have no destructor
			GC.SuppressFinalize(this);
		}

		// Disposer
		public override void Dispose()
		{
			// Not already disposed?
			if(!isdisposed)
			{
				if(map == General.Map.Map && recordundo)
					General.Map.UndoRedo.RecRemThing(this);

				// Remove from main list
				map.RemoveThing(listindex);
				
				// Clean up
				map = null;
				sector = null;

				// Dispose base
				base.Dispose();
			}
		}

		#endregion

		#region ================== Management
		
		// Call this before changing properties
		protected override void BeforePropsChange()
		{
			if(map == General.Map.Map)
				General.Map.UndoRedo.RecPrpThing(this);
		}
		
		// Serialize / deserialize
		new internal void ReadWrite(IReadWriteStream s)
		{
			if(!s.IsWriting) BeforePropsChange();
			
			base.ReadWrite(s);

			if(s.IsWriting)
			{
				s.wInt(flags.Count);
				
				foreach(KeyValuePair<string, bool> f in flags)
				{
					s.wString(f.Key);
					s.wBool(f.Value);
				}
			}
			else
			{
				int c; s.rInt(out c);

				flags = new Dictionary<string, bool>(c, StringComparer.Ordinal);
				for(int i = 0; i < c; i++)
				{
					string t; s.rString(out t);
					bool b; s.rBool(out b);
					flags.Add(t, b);
				}
			}
			
			s.rwInt(ref type);
			s.rwVector3D(ref pos);
			s.rwInt(ref angledoom);
			s.rwInt(ref pitch); //mxd
			s.rwInt(ref roll); //mxd
			s.rwDouble(ref scaleX); //mxd
			s.rwDouble(ref scaleY); //mxd
			s.rwInt(ref tag);
			s.rwInt(ref action);
			for(int i = 0; i < NUM_ARGS; i++) s.rwInt(ref args[i]);

			if(!s.IsWriting) 
			{
				anglerad = Angle2D.DoomToReal(angledoom);
				UpdateCache(); //mxd
			}
		}

		// This copies all properties to another thing
		public void CopyPropertiesTo(Thing t)
		{
			t.BeforePropsChange();
			
			// Copy properties
			t.type = type;
            t.dynamiclighttype = dynamiclighttype;
			t.anglerad = anglerad;
			t.angledoom = angledoom;
			t.roll = roll; //mxd
			t.pitch = pitch; //mxd
			t.rollrad = rollrad; //mxd
			t.pitchrad = pitchrad; //mxd
			t.scaleX = scaleX; //mxd
			t.scaleY = scaleY; //mxd
			t.spritescale = spritescale; //mxd
			t.pos = pos;
			t.flags = new Dictionary<string,bool>(flags);
			t.rawflags = rawflags;
			t.tag = tag;
			t.action = action;
			t.args = (int[])args.Clone();
			t.size = size;
			t.rendersize = rendersize;
			t.height = height; //mxd
			t.color = color;
			t.directional = directional;
			t.fixedsize = fixedsize;
			t.rendermode = rendermode; //mxd
			t.rollsprite = rollsprite; //mxd

			base.CopyPropertiesTo(t);
		}

		/// <summary>
		/// Updates the raw flag bit map from the flags dictionary. Has to be called before the flags in the game config changed. Has to be called in conjunction with UpdateFlagsFromRawFlags.
		/// </summary>
		internal void UpdateRawFlagsFromFlags()
		{
			foreach (KeyValuePair<string, bool> f in flags)
			{
				if (ushort.TryParse(f.Key, out ushort fnum))
				{
					// Set bit to 0
					rawflags &= (ushort)~fnum;

					// Set bit if necessary
					if (f.Value)
						rawflags |= fnum;
				}
			}
		}

		/// <summary>
		/// Updates the flags dictionary from the raw flags. Has to be called after the flags in the game config changed. Has to be called in conjunction with UpdateRawFlagsFromFlags.
		/// </summary>
		internal void UpdateFlagsFromRawFlags()
		{
			foreach (string fname in General.Map.Config.ThingFlags.Keys)
			{
				if (ushort.TryParse(fname, out ushort fnum))
				{
					flags[fname] = (rawflags & fnum) == fnum;
				}
			}
		}

		// This determines which sector the thing is in and links it
		public void DetermineSector()
		{
			//mxd
			sector = map.GetSectorByCoordinates(pos);
		}

		/// <summary>
		/// Determines what sector a thing is in, given a blockmap
		/// </summary>
		/// <param name="blockmap">The blockmap to use</param>
		public void DetermineSector(BlockMap<BlockEntry> blockmap)
		{
			BlockEntry be = blockmap.GetBlockAt(pos);
			List<Sector> sectors = new List<Sector>(1);

			foreach (Sector s in be.Sectors)
				if (s.Intersect(pos))
					sectors.Add(s);

			if(sectors.Count == 0)
			{
				sector = null;
			}
			else if (sectors.Count == 1)
			{
				sector = sectors[0];
			}
			else
			{
				// Having multiple intersections indicates that there are self-referencing sectors in this spot.
				// In this case we have to check which side of the nearest linedef pos is on, and then use that sector
				HashSet<Linedef> linedefs = new HashSet<Linedef>(sectors[0].Sidedefs.Count * sectors.Count);

				foreach (Sector s in sectors)
					foreach (Sidedef sd in s.Sidedefs)
						linedefs.Add(sd.Line);

				Linedef nearest = MapSet.NearestLinedef(linedefs, pos);
				double d = nearest.SideOfLine(pos);

				if (d <= 0.0 && nearest.Front != null)
					sector = nearest.Front.Sector;
				else if (nearest.Back != null)
					sector = nearest.Back.Sector;
				else
					sector = null;
			}
		}

		// This determines which sector the thing is in and links it
		public void DetermineSector(VisualBlockMap blockmap)
		{
             sector = blockmap.GetSectorAt(pos);
		}

		// This translates the flags into UDMF fields
		internal void TranslateToUDMF()
		{
			// First make a single integer with all flags
			int bits = 0;
			int flagbit;
			foreach(KeyValuePair<string, bool> f in flags)
				if(int.TryParse(f.Key, out flagbit) && f.Value) bits |= flagbit;

			// Now make the new flags
			flags.Clear();
			foreach(FlagTranslation f in General.Map.Config.ThingFlagsTranslation)
			{
				// Flag found in bits?
				if((bits & f.Flag) == f.Flag)
				{
					// Add fields and remove bits
					bits &= ~f.Flag;
					for(int i = 0; i < f.Fields.Count; i++)
						flags[f.Fields[i]] = f.FieldValues[i];
				}
				else
				{
					// Add fields with inverted value
					for(int i = 0; i < f.Fields.Count; i++)
						flags[f.Fields[i]] = !f.FieldValues[i];
				}
			}
		}

		// This translates UDMF fields back into the normal flags
		internal void TranslateFromUDMF()
		{
			//mxd. Clear UDMF-related properties
			this.Fields.Clear();
			scaleX = 1.0f;
			scaleY = 1.0f;
			pitch = 0;
			pitchrad = 0;
			roll = 0;
			rollrad = 0;
			
			// Make copy of the flags
			Dictionary<string, bool> oldfields = new Dictionary<string, bool>(flags);

			// Make the flags
			flags.Clear();
			foreach(KeyValuePair<string, string> f in General.Map.Config.ThingFlags)
			{
				// Flag must be numeric
				int flagbit;
				if(int.TryParse(f.Key, out flagbit))
				{
					foreach(FlagTranslation ft in General.Map.Config.ThingFlagsTranslation)
					{
						if(ft.Flag == flagbit)
						{
							// Only set this flag when the fields match
							bool fieldsmatch = true;
							for(int i = 0; i < ft.Fields.Count; i++)
							{
								if(!oldfields.ContainsKey(ft.Fields[i]) || (oldfields[ft.Fields[i]] != ft.FieldValues[i]))
								{
									fieldsmatch = false;
									break;
								}
							}

							// Field match? Then add the flag.
							if(fieldsmatch)
							{
								flags.Add(f.Key, true);
								break;
							}
						}
					}
				}
			}
		}

		// Selected
		protected override void DoSelect()
		{
			base.DoSelect();
			selecteditem = map.SelectedThings.AddLast(this);
		}

		// Deselect
		protected override void DoUnselect()
		{
			base.DoUnselect();
			if(selecteditem.List != null) selecteditem.List.Remove(selecteditem);
			selecteditem = null;
		}
		
		#endregion
		
		#region ================== Changes

		// This moves the thing
		// NOTE: This does not update sector! (call DetermineSector)
		public void Move(Vector3D newpos)
		{
			if (newpos != pos)
			{
				BeforePropsChange();

				// Change position
				this.pos = newpos;

				if (type != General.Map.Config.Start3DModeThingType)
					General.Map.IsChanged = true;
			}
		}

		// This moves the thing
		// NOTE: This does not update sector! (call DetermineSector)
		public void Move(Vector2D newpos)
		{
			Vector3D p = new Vector3D(newpos.x, newpos.y, pos.z);

			if (p != pos)
			{
				BeforePropsChange();

				// Change position
				this.pos = p;

				if (type != General.Map.Config.Start3DModeThingType)
					General.Map.IsChanged = true;
			}
		}

		// This moves the thing
		// NOTE: This does not update sector! (call DetermineSector)
		public void Move(double x, double y, double zoffset)
		{
			Move(new Vector3D(x, y, zoffset));
		}
		
		// This rotates the thing
		public void Rotate(double newangle)
		{
			BeforePropsChange();
			
			// Change angle
			this.anglerad = newangle;
			this.angledoom = Angle2D.RealToDoom(newangle);
			
			if(type != General.Map.Config.Start3DModeThingType)
				General.Map.IsChanged = true;
		}
		
		// This rotates the thing
		public void Rotate(int newangle)
		{
			BeforePropsChange();
			
			// Change angle
			anglerad = Angle2D.DoomToReal(newangle);
			angledoom = newangle;
			
			if(type != General.Map.Config.Start3DModeThingType)
				General.Map.IsChanged = true;
		}

		//mxd
		public void SetPitch(int newpitch)
		{
			BeforePropsChange();

			pitch = General.ClampAngle(newpitch);

            switch (rendermode)
			{
				case ThingRenderMode.MODEL:
					double pmult = General.Map.Config.BuggyModelDefPitch ? 1 : -1;
                    ModelData md = General.Map.Data.ModeldefEntries[type];
                    if (md.InheritActorPitch || md.UseActorPitch)
                        pitchrad = Angle2D.DegToRad(pmult * (md.InheritActorPitch ? -pitch : pitch));
                    else
                        pitchrad = 0;
					break;

				case ThingRenderMode.FLATSPRITE:
					pitchrad = Angle2D.DegToRad(pitch);
					break;

				default:
					pitchrad = 0;
					break;
			}

			if(type != General.Map.Config.Start3DModeThingType)
				General.Map.IsChanged = true;
		}

		//mxd
		public void SetRoll(int newroll)
		{
			BeforePropsChange();

			roll = General.ClampAngle(newroll);
			rollrad = ((rollsprite || (rendermode == ThingRenderMode.MODEL && General.Map.Data.ModeldefEntries[type].UseActorRoll))
				? Angle2D.DegToRad(roll) : 0);

			if(type != General.Map.Config.Start3DModeThingType)
				General.Map.IsChanged = true;
		}

		//mxd
		public void SetScale(double scalex, double scaley)
		{
			BeforePropsChange();

			scaleX = scalex;
			scaleY = scaley;

			if(type != General.Map.Config.Start3DModeThingType)
				General.Map.IsChanged = true;
		}
		
		// This updates all properties
		// NOTE: This does not update sector! (call DetermineSector)
		public void Update(int type, double x, double y, double zoffset, int angle, int pitch, int roll, double scaleX, double scaleY,
						   Dictionary<string, bool> flags, ushort rawflags, int tag, int action, int[] args)
		{
			// Apply changes
			this.type = type;
			this.anglerad = Angle2D.DoomToReal(angle);
			this.angledoom = angle;
			this.pitch = pitch; //mxd
			this.roll = roll; //mxd
			this.scaleX = (scaleX == 0 ? 1.0f : scaleX); //mxd
			this.scaleY = (scaleY == 0 ? 1.0f : scaleY); //mxd
			this.flags = new Dictionary<string, bool>(flags);
			this.rawflags = rawflags;
			this.tag = tag;
			this.action = action;
			this.args = new int[NUM_ARGS];
			args.CopyTo(this.args, 0);
			this.Move(x, y, zoffset);

			UpdateCache(); //mxd
		}
		
		// This updates the settings from configuration
		public void UpdateConfiguration()
		{
			// Lookup settings
			ThingTypeInfo ti = General.Map.Data.GetThingInfo(type);

            // Apply size
            dynamiclighttype = GZGeneral.GetGZLightTypeByClass(ti.Actor);
            if (dynamiclighttype == null)
                dynamiclighttype = ti.DynamicLightType;
            //General.ErrorLogger.Add(ErrorType.Warning, string.Format("thing dynamiclighttype is {0}; class is {1}", dynamiclighttype, ti.Actor.ClassName));
			size = ti.Radius;
			rendersize = ti.RenderRadius;
			height = ti.Height; //mxd
			fixedsize = ti.FixedSize;
			spritescale = ti.SpriteScale; //mxd

			//mxd. Apply radius and height overrides?
			for(int i = 0; i < ti.Args.Length; i++)
			{
				if(ti.Args[i] == null) continue;
				if(ti.Args[i].Type == (int)UniversalType.ThingRadius && args[i] > 0)
					size = args[i];
				else if(ti.Args[i].Type == (int)UniversalType.ThingHeight && args[i] > 0)
					height = args[i];
			}
			
			// Color valid?
			if((ti.Color >= 0) && (ti.Color < ColorCollection.NUM_THING_COLORS))
			{
				// Apply color
				color = General.Colors.Colors[ti.Color + ColorCollection.THING_COLORS_OFFSET];
			}
			else
			{
				// Unknown thing color
				color = General.Colors.Colors[ColorCollection.THING_COLORS_OFFSET];
			}
			
			directional = ti.Arrow; //mxd
			rendermode = ti.RenderMode; //mxd
			rollsprite = ti.RollSprite; //mxd
			UpdateCache(); //mxd
		}

		//mxd. This checks if the thing has model override and whether pitch/roll values should be used
		internal void UpdateCache()
		{
			if(General.Map.Data == null) return;

			// Check if the thing has model override
			if(General.Map.Data.ModeldefEntries.ContainsKey(type))
			{
				ModelData md = General.Map.Data.ModeldefEntries[type];
				if((md.LoadState == ModelLoadState.None && General.Map.Data.ProcessModel(type)) || md.LoadState != ModelLoadState.None)
					rendermode = (General.Map.Data.ModeldefEntries[type].IsVoxel ? ThingRenderMode.VOXEL : ThingRenderMode.MODEL);
			} 
            else // reset rendermode if we SUDDENLY became a sprite out of a model. otherwise it crashes violently.
            {
                ThingTypeInfo ti = General.Map.Data.GetThingInfo(Type);
                rendermode = (ti != null) ? ti.RenderMode : ThingRenderMode.NORMAL;
            }

			// Update radian versions of pitch and roll
			switch(rendermode)
			{
				case ThingRenderMode.MODEL:
                    float pmult = General.Map.Config.BuggyModelDefPitch ? 1 : -1;
                    ModelData md = General.Map.Data.ModeldefEntries[type];
					rollrad = (md.UseActorRoll ? Angle2D.DegToRad(roll) : 0);
					pitchrad = ((md.InheritActorPitch || md.UseActorPitch) ? Angle2D.DegToRad(pmult * (md.InheritActorPitch ? -pitch : pitch)) : 0);
					break;

				case ThingRenderMode.FLATSPRITE:
					rollrad = Angle2D.DegToRad(roll);
					pitchrad = Angle2D.DegToRad(pitch);
					break;

				case ThingRenderMode.WALLSPRITE:
					rollrad = Angle2D.DegToRad(roll);
					pitchrad = 0;
					break;

				case ThingRenderMode.NORMAL:
					rollrad = (rollsprite ? Angle2D.DegToRad(roll) : 0);
					pitchrad = 0;
					break;

				case ThingRenderMode.VOXEL:
					rollrad = 0;
					pitchrad = 0;
					break;

				default: throw new NotImplementedException("Unknown ThingRenderMode");
			}
		}
		
		#endregion

		#region ================== Methods
		
		// This checks and returns a flag without creating it
		public bool IsFlagSet(string flagname)
		{
			return flags.ContainsKey(flagname) && flags[flagname];
		}
		
		// This sets a flag
		public void SetFlag(string flagname, bool value)
		{
			if(!flags.ContainsKey(flagname) || (IsFlagSet(flagname) != value))
			{
				BeforePropsChange();

				flags[flagname] = value;
			}
		}
		
		// This returns a copy of the flags dictionary
		public Dictionary<string, bool> GetFlags()
		{
			return new Dictionary<string,bool>(flags);
		}

		//mxd. This returns enabled flags
		public HashSet<string> GetEnabledFlags()
		{
			HashSet<string> result = new HashSet<string>();
			foreach(KeyValuePair<string, bool> group in flags)
				if(group.Value) result.Add(group.Key);
			return result;
		} 

		// This clears all flags
		public void ClearFlags()
		{
			BeforePropsChange();
			
			flags.Clear();
		}
		
		// This snaps the vertex to the grid
		public void SnapToGrid()
		{
			// Calculate nearest grid coordinates
			this.Move(General.Map.Grid.SnappedToGrid(pos));
		}

		// This snaps the vertex to the map format accuracy
		public void SnapToAccuracy()
		{
			SnapToAccuracy(true);
		}

		// This snaps the vertex to the map format accuracy
		public void SnapToAccuracy(bool usepreciseposition)
		{
			// Round the coordinates
			Vector3D newpos = new Vector3D(Math.Round(pos.x, (usepreciseposition ? General.Map.FormatInterface.VertexDecimals : 0)),
										   Math.Round(pos.y, (usepreciseposition ? General.Map.FormatInterface.VertexDecimals : 0)),
										   Math.Round(pos.z, (usepreciseposition ? General.Map.FormatInterface.VertexDecimals : 0)));
			this.Move(newpos);
		}
		
		// This returns the distance from given coordinates
		public double DistanceToSq(Vector2D p)
		{
			return Vector2D.DistanceSq(p, pos);
		}

		// This returns the distance from given coordinates
		public double DistanceTo(Vector2D p)
		{
			return Vector2D.Distance(p, pos);
		}

		/// <summary>
		/// Changes the thing's index to a new index.
		/// </summary>
		/// <param name="newindex">The new index to set</param>
		public void ChangeIndex(int newindex)
		{
			General.Map.UndoRedo.RecIndexThing(Index, newindex);
			map?.ChangeThingIndex(Index, newindex);
		}

		#endregion
	}
}
