
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
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.Geometry;
using System.Drawing;
using CodeImp.DoomBuilder.IO;

#endregion

namespace CodeImp.DoomBuilder.Map
{
	public sealed class Linedef : SelectableElement, IMultiTaggedMapElement
	{
		#region ================== Constants

		public const double SIDE_POINT_DISTANCE = 0.01;
		public const int NUM_ARGS = 5;
		
		#endregion

		#region ================== Variables

		// Map
		private MapSet map;

		// List items
		private LinkedListNode<Linedef> startvertexlistitem;
		private LinkedListNode<Linedef> endvertexlistitem;
		private LinkedListNode<Linedef> selecteditem;
		
		// Vertices
		private Vertex start;
		private Vertex end;
		
		// Sidedefs
		private Sidedef front;
		private Sidedef back;

		// Cache
		private bool updateneeded;
		private double lengthsq;
		private double lengthsqinv;
		private double length;
		private double lengthinv;
		private double angle;
		private RectangleF rect;
		private bool impassableflag;
		
		// Properties
		private Dictionary<string, bool> flags;
		private ushort rawflags; // The actual flags bitmap that also might include unknown flags
		private int action;
		private int activate;
		private List<int> tags; //mxd
		private int[] args;
		private bool frontinterior;		// for drawing only
		private int colorPresetIndex;//mxd

		// Clone
		private int serializedindex;

        // Rendering
        private int lastProcessed;
		
		#endregion

		#region ================== Properties

		public MapSet Map { get { return map; } }
		public Vertex Start { get { return start; } }
		public Vertex End { get { return end; } }
		public Sidedef Front { get { return front; } }
		public Sidedef Back { get { return back; } }
		public Line2D Line { get { return new Line2D(start.Position, end.Position); } }
		internal Dictionary<string, bool> Flags { get { return flags; } }
		public ushort RawFlags { get { return rawflags; } }
		public int Action { get { return action; } set { BeforePropsChange(); action = value; UpdateColorPreset(); } }
		public int Activate { get { return activate; } set { BeforePropsChange(); activate = value; UpdateColorPreset(); } }

		public int Tag { get { return tags[0]; } set { BeforePropsChange(); tags[0] = value; if((value < General.Map.FormatInterface.MinTag) || (value > General.Map.FormatInterface.MaxTag)) throw new ArgumentOutOfRangeException("Tag", "Invalid tag number"); } } //mxd
		public List<int> Tags { get { return tags; } set { BeforePropsChange(); tags = value; } } //mxd
		public double LengthSq { get { return lengthsq; } }
		public double Length { get { return length; } }
		public double LengthInv { get { return lengthinv; } }
		public double Angle { get { return angle; } }
		public int AngleDeg { get { return (int)(angle * Angle2D.PIDEG); } }
		public RectangleF Rect { get { return rect; } }
		public int[] Args { get { return args; } }
		internal int SerializedIndex { get { return serializedindex; } set { serializedindex = value; } }
        internal int LastProcessed { get { return lastProcessed; } set { lastProcessed = value; } }
        internal bool FrontInterior { get { return frontinterior; } set { frontinterior = value; } }
		internal bool ImpassableFlag { get { return impassableflag; } }
		internal int ColorPresetIndex { get { return colorPresetIndex; } } //mxd
		internal bool ExtraFloorFlag; //mxd
		
		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		internal Linedef(MapSet map, int listindex, Vertex start, Vertex end)
		{
			// Initialize
			this.elementtype = MapElementType.LINEDEF; //mxd
			this.map = map;
			this.listindex = listindex;
			this.updateneeded = true;
			this.args = new int[NUM_ARGS];
			this.tags = new List<int> { 0 }; //mxd
			this.flags = new Dictionary<string, bool>(StringComparer.Ordinal);
			this.colorPresetIndex = -1;//mxd
			
			// Attach to vertices
			this.start = start;
			this.startvertexlistitem = start.AttachLinedefP(this);
			this.end = end;
			this.endvertexlistitem = end.AttachLinedefP(this);
			
			if(map == General.Map.Map)
				General.Map.UndoRedo.RecAddLinedef(this);
			
			// We have no destructor
			GC.SuppressFinalize(this);
		}
		
		// Disposer
		public override void Dispose()
		{
			// Not already disposed?
			if(!isdisposed)
			{
				// Already set isdisposed so that changes can be prohibited
				isdisposed = true;

				// Dispose sidedefs
				if((front != null) && map.AutoRemove) front.Dispose(); else AttachFrontP(null);
				if((back != null) && map.AutoRemove) back.Dispose(); else AttachBackP(null);
				
				if(map == General.Map.Map)
					General.Map.UndoRedo.RecRemLinedef(this);

				// Remove from main list
				map.RemoveLinedef(listindex);
				
				// Detach from vertices
				if(startvertexlistitem != null) start.DetachLinedefP(startvertexlistitem);
				startvertexlistitem = null;
				start = null;
				if(endvertexlistitem != null) end.DetachLinedefP(endvertexlistitem);
				endvertexlistitem = null;
				end = null;
				
				// Clean up
				start = null;
				end = null;
				front = null;
				back = null;
				map = null;

				//mxd. Restore isdisposed so base classes can do their disposal job
				isdisposed = false;

				// Clean up base
				base.Dispose();
			}
		}

		#endregion

		#region ================== Management
		
		// Call this before changing properties
		protected override void BeforePropsChange()
		{
			if(map == General.Map.Map)
				General.Map.UndoRedo.RecPrpLinedef(this);
		}
		
		// Serialize / deserialize (passive: doesn't record)
		new internal void ReadWrite(IReadWriteStream s)
		{
			if(!s.IsWriting)
			{
				BeforePropsChange();
				updateneeded = true;
			}
			
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

			s.rwInt(ref action);
			s.rwInt(ref activate);

			//mxd
			if(s.IsWriting)
			{
				s.wInt(tags.Count);
				foreach(int tag in tags) s.wInt(tag);
			}
			else
			{
				int c; s.rInt(out c);
				tags = new List<int>(c);
				for(int i = 0; i < c; i++) 
				{
					int t; s.rInt(out t);
					tags.Add(t);
				}
			}

			for(int i = 0; i < NUM_ARGS; i++) s.rwInt(ref args[i]);

			//mxd
			if(!s.IsWriting) UpdateColorPreset();
		}

		// This sets new start vertex
		public void SetStartVertex(Vertex v)
		{
			if(map == General.Map.Map)
				General.Map.UndoRedo.RecRefLinedefStart(this);
			
			// Change start
			if(startvertexlistitem != null) start.DetachLinedefP(startvertexlistitem);
			startvertexlistitem = null;
			start = v;
			if(start != null) startvertexlistitem = start.AttachLinedefP(this);
			this.updateneeded = true;
		}

		// This sets new end vertex
		public void SetEndVertex(Vertex v)
		{
			if(map == General.Map.Map)
				General.Map.UndoRedo.RecRefLinedefEnd(this);

			// Change end
			if(endvertexlistitem != null) end.DetachLinedefP(endvertexlistitem);
			endvertexlistitem = null;
			end = v;
			if(end != null) endvertexlistitem = end.AttachLinedefP(this);
			this.updateneeded = true;
		}

		// This detaches a vertex
		internal void DetachVertexP(Vertex v)
		{
			if(v == start)
			{
				if(startvertexlistitem != null) start.DetachLinedefP(startvertexlistitem);
				startvertexlistitem = null;
				start = null;
			}
			else if(v == end)
			{
				if(endvertexlistitem != null) end.DetachLinedefP(endvertexlistitem);
				endvertexlistitem = null;
				end = null;
			}
			else 
				throw new Exception("Specified Vertex is not attached to this Linedef.");
		}
		
		// This copies all properties to another line
		public void CopyPropertiesTo(Linedef l)
		{
			l.BeforePropsChange();
			
			// Copy properties
			l.action = action;
			l.args = (int[])args.Clone();
			l.flags = new Dictionary<string, bool>(flags);
			l.rawflags = rawflags;
			l.tags = new List<int>(tags); //mxd
			l.updateneeded = true;
			l.activate = activate;
			l.impassableflag = impassableflag;
			l.UpdateColorPreset();//mxd
			base.CopyPropertiesTo(l);
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
			foreach (string fname in General.Map.Config.LinedefFlags.Keys)
			{
				if (ushort.TryParse(fname, out ushort fnum))
				{
					flags[fname] = (rawflags & fnum) == fnum;
				}
			}
		}

		// This attaches a sidedef on the front
		internal void AttachFront(Sidedef s)
		{
			if(map == General.Map.Map)
				General.Map.UndoRedo.RecRefLinedefFront(this);
			
			// Attach and recalculate
			AttachFrontP(s);
		}
		
		// Passive version, does not record the change
		internal void AttachFrontP(Sidedef s)
		{
			// Attach and recalculate
			front = s;
			if(front != null) front.SetLinedefP(this);
			updateneeded = true;
		}

		// This attaches a sidedef on the back
		internal void AttachBack(Sidedef s)
		{
			if(map == General.Map.Map)
				General.Map.UndoRedo.RecRefLinedefBack(this);

			// Attach and recalculate
			AttachBackP(s);
		}
		
		// Passive version, does not record the change
		internal void AttachBackP(Sidedef s)
		{
			// Attach and recalculate
			back = s;
			if(back != null) back.SetLinedefP(this);
			updateneeded = true;
		}

		// This detaches a sidedef from the front
		internal void DetachSidedefP(Sidedef s)
		{
			// Sidedef is on the front?
			if(front == s)
			{
				// Remove sidedef reference
				if(front != null) front.SetLinedefP(null);
				front = null;
				updateneeded = true;
			}
			// Sidedef is on the back?
			else if(back == s)
			{
				// Remove sidedef reference
				if(back != null) back.SetLinedefP(null);
				back = null;
				updateneeded = true;
			}
			//else throw new Exception("Specified Sidedef is not attached to this Linedef.");
		}
		
		// This updates the line when changes have been made
		public void UpdateCache()
		{
			// Update if needed
			if(updateneeded)
			{
				// Delta vector
				Vector2D delta = end.Position - start.Position;

				// Recalculate values
				lengthsq = delta.GetLengthSq();
				length = Math.Sqrt(lengthsq);
				if(length > 0.0) lengthinv = 1.0 / length; else lengthinv = 1.0 / 0.0000000001;
				if(lengthsq > 0.0) lengthsqinv = 1.0 / lengthsq; else lengthsqinv = 1.0 / 0.0000000001;
				angle = delta.GetAngle();
				double l = Math.Min(start.Position.x, end.Position.x);
				double t = Math.Min(start.Position.y, end.Position.y);
				double r = Math.Max(start.Position.x, end.Position.x);
				double b = Math.Max(start.Position.y, end.Position.y);
				rect = new RectangleF((float)l, (float)t, (float)(r - l), (float)(b - t));
				
				// Cached flags
				impassableflag = IsFlagSet(General.Map.Config.ImpassableFlag);

				//mxd. Color preset
				UpdateColorPreset();
				
				// Updated
				updateneeded = false;
			}
		}

		// This flags the line needs an update because it moved
		public void NeedUpdate()
		{
			// Update this line
			updateneeded = true;

			// Update sectors as well
			if(front != null) front.Sector.UpdateNeeded = true;
			if(back != null) back.Sector.UpdateNeeded = true;
		}
		
		// This translates the flags and activations into UDMF fields
		internal void TranslateToUDMF(Type previousmapformatinterfacetype)
		{
			// First make a single integer with all bits from activation and flags
			int bits = activate;
			int flagbit;
			foreach(KeyValuePair<string, bool> f in flags)
				if(int.TryParse(f.Key, out flagbit) && f.Value) bits |= flagbit;
			
			// Now make the new flags
			flags.Clear();

			//mxd. Add default activation flag if needed
			if(action != 0 && activate == 0 && !string.IsNullOrEmpty(General.Map.Config.DefaultLinedefActivationFlag)) 
				flags[General.Map.Config.DefaultLinedefActivationFlag] = true;

			foreach(FlagTranslation f in General.Map.Config.LinedefFlagsTranslation)
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
					{
						if(!flags.ContainsKey(f.Fields[i])) //mxd
							flags[f.Fields[i]] = !f.FieldValues[i];
					}
				}
			}

			//mxd. Hexen -> UDMF action translation. Hardcoded for now...
			if(previousmapformatinterfacetype == typeof(HexenMapSetIO)) 
			{
				switch(Action) 
				{
					case 121: //Line_SetIdentification
						//Convert arg0 to tag
						tags[0] = args[0] + args[4] * 256;

						//Convert arg1 to flags
						ConvertArgToFlags(1);

						//clear action and arguments
						action = 0;
						for(int i = 0; i < args.Length; i++) args[i] = 0;
						break;

					case 208: //TranslucentLine
						//Convert arg0 to tag
						tags[0] = args[0];

						//Convert arg3 to flags
						ConvertArgToFlags(3);
						break;

					case 1: ConvertArgToTag(3, true); break; //Polyobj_StartLine
					case 5: ConvertArgToTag(4, true); break; //Polyobj_ExplicitLine
					case 181: ConvertArgToTag(2, true); break; //Plane_Align
					case 215: ConvertArgToTag(0, true); break; //Teleport_Line
					case 222: ConvertArgToTag(0, false); break; //Scroll_Texture_Model

					case 160: //Sector_3DFloor
						// Convert to UDMF
						if((args[1] & 8) == 8) // arg4 is LineID?
						{
							tags[0] = args[4];
							args[1] &= ~8; // Unset flag
						}
						else // It's sector's HiTag then
						{
							args[0] += args[4] * 256;
						}

						// Clear arg
						args[4] = 0;
						break;
				}
			}

			//mxd. Update cached flags
			impassableflag = IsFlagSet(General.Map.Config.ImpassableFlag);

			// Update color preset
			UpdateColorPreset();
		}
		
		// This translates UDMF fields back into the normal flags and activations
		internal void TranslateFromUDMF()
		{
			//mxd. Clear UDMF-related properties
			this.Fields.Clear();
			ExtraFloorFlag = false;
			
			// Make copy of the flags
			Dictionary<string, bool> oldfields = new Dictionary<string, bool>(flags);

			// Make the flags
			flags.Clear();
			foreach(KeyValuePair<string, string> f in General.Map.Config.LinedefFlags)
			{
				// Flag must be numeric
				int flagbit;
				if(int.TryParse(f.Key, out flagbit))
				{
					foreach(FlagTranslation ft in General.Map.Config.LinedefFlagsTranslation)
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
			
			// Make the activation
			foreach(LinedefActivateInfo a in General.Map.Config.LinedefActivates)
			{
				bool foundactivation = false;
				foreach(FlagTranslation ft in General.Map.Config.LinedefFlagsTranslation)
				{
					if(ft.Flag == a.Index)
					{
						// Only set this activation when the fields match
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
							activate = a.Index;
							foundactivation = true;
							break;
						}
					}
				}
				if(foundactivation) break;
			}

			//mxd. UDMF -> Hexen action translation. Hardcoded for now...
			if(General.Map.FormatInterface is HexenMapSetIO)
			{
				switch(action)
				{
					case 208: //TranslucentLine
						//Convert tag to arg0
						if(tags[0] < General.Map.FormatInterface.MinArgument || tags[0] > General.Map.FormatInterface.MaxArgument)
						{
							string message = "Linedef " + Index + ": unable to convert Tag (" + tags[0] + ") to LineID because it's outside of supported argument range [" + General.Map.FormatInterface.MinArgument + ".." + General.Map.FormatInterface.MaxArgument + "].";
							General.ErrorLogger.Add(new MapElementErrorItem(ErrorType.Warning, this, message));
						}
						else
						{
							args[0] = tags[0];
						}

						//Convert flags to arg3
						ConvertFlagsToArg(oldfields, 3);
						break;

					case 1: ConvertTagToArg(3); break; //Polyobj_StartLine
					case 5: ConvertTagToArg(4); break; //Polyobj_ExplicitLine
					case 181: ConvertTagToArg(2); break; //Plane_Align
					case 215: ConvertTagToArg(0); break; //Teleport_Line
					case 222: ConvertTagToArg(0); break; //Scroll_Texture_Model

					case 160: //Sector_3DFloor
						if(args[0] > General.Map.FormatInterface.MaxArgument) // Split sector tag?
						{
							int hitag = args[0] / 256;
							int lotag = args[0] % 256;

							args[0] = lotag;
							args[4] = hitag;

							if(tags[0] != 0)
							{
								string message = "Linedef " + Index + ": unable to convert Tag (" + tags[0] + ") to LineID, because target sector tag (arg0) is greater than " + General.Map.FormatInterface.MaxArgument + ".";
								General.ErrorLogger.Add(new MapElementErrorItem(ErrorType.Warning, this, message));
							}
						}
						else if(args[0] < General.Map.FormatInterface.MinArgument) 
						{
							string message = "Linedef " + Index + ": unable to convert arg0 (" + args[0] + "), because it's outside of supported argument range [" + General.Map.FormatInterface.MinArgument + ".." + General.Map.FormatInterface.MaxArgument + "].";
							General.ErrorLogger.Add(new MapElementErrorItem(ErrorType.Warning, this, message));
						} 
						else if(tags[0] > General.Map.FormatInterface.MinArgument) // Convert to LineID?
						{
							if(tags[0] > General.Map.FormatInterface.MaxArgument)
							{
								string message = "Linedef " + Index + ": unable to convert Tag (" + tags[0] + ") to LineID, because linedef tag is greater than " + General.Map.FormatInterface.MaxArgument + ".";
								General.ErrorLogger.Add(new MapElementErrorItem(ErrorType.Warning, this, message));
							}
							else
							{
								args[4] = tags[0];
								args[1] |= 8; // Add "Use arg4 as LineID" flag
							}
						}
						break;

					default: // Convert tag to Line_SetIdentification?
						if(tags[0] > General.Map.FormatInterface.MaxArgument)
						{
							if(action != 0)
							{
								string message = "Linedef " + Index + ": unable to convert Tag (" + tags[0] + ") to LineID, because linedef already has an action.";
								General.ErrorLogger.Add(new MapElementErrorItem(ErrorType.Warning, this, message));
							}
							else // Convert to Line_SetIdentification
							{
								int hiid = tags[0] / 256;
								int loid = tags[0] % 256;

								action = 121;
								args[0] = loid;
								args[4] = hiid;
								ConvertFlagsToArg(oldfields, 1);
							}
						} 
						else if(tags[0] < General.Map.FormatInterface.MinArgument) 
						{
							string message = "Linedef " + Index + ": unable to convert Tag (" + tags[0] + ") to LineID, because it's outside of supported argument range [" + General.Map.FormatInterface.MinArgument + ".." + General.Map.FormatInterface.MaxArgument + "].";
							General.ErrorLogger.Add(new MapElementErrorItem(ErrorType.Warning, this, message));
						}
						break;
				}

				// Clear tag
				tags[0] = 0;
			}

			//mxd. Update cached flags
			impassableflag = IsFlagSet(General.Map.Config.ImpassableFlag);

			// Update color preset
			UpdateColorPreset();
		}

		//mxd
		private void ConvertArgToTag(int argnum, bool cleararg) 
		{
			// Convert arg to tag
			tags[0] = args[argnum];

			// Clear obsolete arg
			if(cleararg) args[argnum] = 0;
		}

		//mxd
		private void ConvertTagToArg(int argnum) 
		{
			if(tags[0] < General.Map.FormatInterface.MinArgument || tags[0] > General.Map.FormatInterface.MaxArgument)
			{
				General.ErrorLogger.Add(ErrorType.Warning, "Linedef " + Index + ": unable to convert Tag (" + tags[0] + ") to LineID because it's outside of supported argument range [" + General.Map.FormatInterface.MinArgument + ".." + General.Map.FormatInterface.MaxArgument + "].");
			}
			else
			{
				args[argnum] = tags[0];
			}
		}

		//mxd
		private void ConvertArgToFlags(int argnum) 
		{
			if(args[argnum] == 0) return;

			// Convert to flags
			if((args[argnum] & 1) == 1) flags["zoneboundary"] = true;
			if((args[argnum] & 2) == 2) flags["jumpover"] = true;
			if((args[argnum] & 4) == 4) flags["blockfloaters"] = true;
			if((args[argnum] & 8) == 8) flags["clipmidtex"] = true;
			if((args[argnum] & 16) == 16) flags["wrapmidtex"] = true;
			if((args[argnum] & 32) == 32) flags["midtex3d"] = true;
			if((args[argnum] & 64) == 64) flags["checkswitchrange"] = true;
			if((args[argnum] & 128) == 128) flags["firstsideonly"] = true;

			// Clear obsolete arg
			args[argnum] = 0;
		}

		//mxd
		private void ConvertFlagsToArg(Dictionary<string, bool> oldflags, int argnum)
		{
			int bits = 0;
			if(oldflags.ContainsKey("zoneboundary") && oldflags["zoneboundary"]) bits &= 1;
			if(oldflags.ContainsKey("jumpover") && oldflags["jumpover"]) bits &= 2;
			if(oldflags.ContainsKey("blockfloaters") && oldflags["blockfloaters"]) bits &= 4;
			if(oldflags.ContainsKey("clipmidtex") && oldflags["clipmidtex"]) bits &= 8;
			if(oldflags.ContainsKey("wrapmidtex") && oldflags["wrapmidtex"]) bits &= 16;
			if(oldflags.ContainsKey("midtex3d") && oldflags["midtex3d"]) bits &= 32;
			if(oldflags.ContainsKey("checkswitchrange") && oldflags["checkswitchrange"]) bits &= 64;
			if(oldflags.ContainsKey("firstsideonly") && oldflags["firstsideonly"]) bits &= 128;
			
			// Set arg
			args[argnum] = bits;
		}

		// Selected
		protected override void DoSelect()
		{
			base.DoSelect();
			selecteditem = map.SelectedLinedefs.AddLast(this);
		}

		// Deselect
		protected override void DoUnselect()
		{
			base.DoUnselect();
			if(selecteditem.List != null) selecteditem.List.Remove(selecteditem);
			selecteditem = null;
		}
		
		#endregion
		
		#region ================== Methods

		// Plane Align (181) (see http://zdoom.org/wiki/Plane_Align
		public bool HasActionPlaneAlign()
		{
			return Action > 0 && General.Map.Config.GetLinedefActionInfo(Action).Id.ToLowerInvariant() == "plane_align";
    }

		// Determine if this line defines the sky upper texture transferred to a sector.
		public bool HasSkyTransfer()
		{
			return HasSkyTransferStaticInit() ||
				General.Map.Config.GetLinedefActionInfo(Action).ErrorCheckerExemptions.RequiresUpperTexture;
		}

		// Determine if this line uses Static_Init that mimics MBF's sky transfer linedef specials.
		// This also enables an optional lower texture to be shown during a lightning weather effect.
		public bool HasSkyTransferStaticInit()
		{
			return General.Map.Config.GetLinedefActionInfo(Action).Id.ToLowerInvariant() == "static_init" &&
				Args[1] == 255;
		}

		// Determine if this line and another line are associated by action and tag.
		public bool IsAssociatedWith(Linedef ld)
		{
			LinedefActionInfo actioninfo;

			return this != ld &&
				Action > 0 &&
				ld.Tag > 0 &&
				tags.Contains(ld.Tag) &&
				(actioninfo = General.Map.Config.GetLinedefActionInfo(Action)).LineToLineTag &&
				(!actioninfo.LineToLineSameAction || Action == ld.Action);
		}

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

				// Cached flags
				if(flagname == General.Map.Config.ImpassableFlag) impassableflag = value;

				//mxd
				UpdateColorPreset();
			}
		}

		// This returns a copy of the flags dictionary
		public Dictionary<string, bool> GetFlags()
		{
			return new Dictionary<string, bool>(flags);
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
			impassableflag = false;

			//mxd
			UpdateColorPreset();
		}
		
		// This flips the linedef's vertex attachments
		public void FlipVertices()
		{
			// make sure the start/end vertices are not automatically
			// deleted if they do not belong to any other line
			General.Map.Map.AutoRemove = false;

			// Flip vertices
			Vertex oldstart = start;
			Vertex oldend = end;
			SetStartVertex(oldend);
			SetEndVertex(oldstart);

			General.Map.Map.AutoRemove = true;
			
			// For drawing, the interior now lies on the other side
			frontinterior = !frontinterior;

			// Update required (angle changed)
			NeedUpdate();
			General.Map.IsChanged = true;
		}

		// This flips the sidedefs
		public void FlipSidedefs()
		{
			// Flip sidedefs
			Sidedef oldfront = front;
			Sidedef oldback = back;
			AttachFront(oldback);
			AttachBack(oldfront);
			
			General.Map.IsChanged = true;
		}
		
		// This returns a point for testing on one side
		public Vector2D GetSidePoint(bool front)
		{
			Vector2D n = new Vector2D();
			n.x = (end.Position.x - start.Position.x) * lengthinv * SIDE_POINT_DISTANCE;
			n.y = (end.Position.y - start.Position.y) * lengthinv * SIDE_POINT_DISTANCE;

			if(front)
			{
				n.x = -n.x;
				n.y = -n.y;
			}

			Vector2D p = new Vector2D();
			p.x = start.Position.x + (end.Position.x - start.Position.x) * 0.5 - n.y;
			p.y = start.Position.y + (end.Position.y - start.Position.y) * 0.5 + n.x;

			return p;
		}

		// This returns a point in the middle of the line
		public Vector2D GetCenterPoint()
		{
			return start.Position + (end.Position - start.Position) * 0.5;
		}
		
		// This applies single/double sided flags
		public void ApplySidedFlags()
		{
			// Doublesided?
			if((front != null) && (back != null))
			{
				// Apply or remove flags for doublesided line
				SetFlag(General.Map.Config.SingleSidedFlag, false);
				SetFlag(General.Map.Config.DoubleSidedFlag, true);
			}
			else
			{
				// Apply or remove flags for singlesided line
				SetFlag(General.Map.Config.SingleSidedFlag, true);
				SetFlag(General.Map.Config.DoubleSidedFlag, false);
			}
			
			General.Map.IsChanged = true;
		}

		// This returns all points at which the line intersects with the grid
		public List<Vector2D> GetGridIntersections()
		{
			return GetGridIntersections(0.0);
		}

		
		public List<Vector2D> GetGridIntersections(double gridrotation, double gridoriginx = 0.0, double gridoriginy = 0.0) 
		{
			return GetGridIntersections(new Vector2D(), gridrotation, gridoriginx, gridoriginy);
		}

		// This returns all points at which the line intersects with the grid
		public List<Vector2D> GetGridIntersections(Vector2D gridoffset, double gridrotation = 0.0, double gridoriginx = 0.0, double gridoriginy = 0.0)
		{
			List<Vector2D> coords = new List<Vector2D>();
			Vector2D v = new Vector2D();
			double minx, maxx, miny, maxy;
			bool reversex, reversey;

			Vector2D v1 = start.Position;
			Vector2D v2 = end.Position;

			bool transformed = Math.Abs(gridrotation) > 1e-4 || Math.Abs(gridoriginx) > 1e-4 || Math.Abs(gridoriginy) > 1e-4;
			if (transformed)
			{
				v1 = (v1 - new Vector2D(gridoriginx, gridoriginy)).GetRotated(-gridrotation);
				v2 = (v2 - new Vector2D(gridoriginx, gridoriginy)).GetRotated(-gridrotation);
			}
			
			if(v1.x > v2.x)
			{
				minx = v2.x;
				maxx = v1.x;
				reversex = true;
			}
			else
			{
				minx = v1.x;
				maxx = v2.x;
				reversex = false;
			}

			if(v1.y > v2.y)
			{
				miny = v2.y;
				maxy = v1.y;
				reversey = true;
			}
			else
			{
				miny = v1.y;
				maxy = v2.y;
				reversey = false;
			}

			// Go for all vertical grid lines in between line start and end
			double gx = General.Map.Grid.GetHigher(minx) + gridoffset.x;
			if(gx < maxx)
			{
				for(; gx < maxx; gx += General.Map.Grid.GridSizeF)
				{
					// Add intersection point at this x coordinate
					double u = (gx - minx) / (maxx - minx);
					if(reversex) u = 1.0 - u;
					v.x = gx;
					v.y = v1.y + (v2.y - v1.y) * u;
					coords.Add(v);
				}
			}

			// Go for all horizontal grid lines in between line start and end
			double gy = General.Map.Grid.GetHigher(miny) + gridoffset.y;
			if(gy < maxy)
			{
				for(; gy < maxy; gy += General.Map.Grid.GridSizeF)
				{
					// Add intersection point at this y coordinate
					double u = (gy - miny) / (maxy - miny);
					if(reversey) u = 1.0 - u;
					v.x = v1.x + (v2.x - v1.x) * u;
					v.y = gy;
					coords.Add(v);
				}
			}

			if (transformed)
			{
				for (int i = 0; i < coords.Count; i++)
				{
					coords[i] = coords[i].GetRotated(gridrotation) + new Vector2D(gridoriginx, gridoriginy);
				}
			}
			
			// Profit
			return coords;
		}
		
		// This returns the closest coordinates ON the line
		public Vector2D NearestOnLine(Vector2D pos)
		{
			double u = Line2D.GetNearestOnLine(start.Position, end.Position, pos);
			if(u < 0.0) u = 0.0; else if(u > 1.0) u = 1.0;
			return Line2D.GetCoordinatesAt(start.Position, end.Position, u);
		}

		// This returns the shortest distance from given coordinates to line
		public double SafeDistanceToSq(Vector2D p, bool bounded)
		{
			Vector2D v1 = start.Position;
			Vector2D v2 = end.Position;

			// Calculate intersection offset
			double u = ((p.x - v1.x) * (v2.x - v1.x) + (p.y - v1.y) * (v2.y - v1.y)) * lengthsqinv;

			// Limit intersection offset to the line
			if (bounded)
			{
				// We really don't want u to be 0 or 1, because that'd mean the distance will be measured
				// from the vertices, which will result in linedefs being equally far away. We still need 
				// special handling for linedefs that are shorter than 1 mu (which is possible in UDMF)
				// Detailed explanation here: https://github.com/jewalky/GZDoom-Builder-Bugfix/issues/307
				if (lengthinv > 1.0)
				{
					u = Math.Max(0, Math.Min(1.0, u));
				}
				else
				{
					u = Math.Max(lengthinv, Math.Min(1.0 - lengthinv, u));
				}
			}

			/*
			// Calculate intersection point
			Vector2D i = v1 + u * (v2 - v1);

			// Return distance between intersection and point
			// which is the shortest distance to the line
			float ldx = p.x - i.x;
			float ldy = p.y - i.y;
			*/

			// ano - let's check to see if we can do the previous faster without using operator overloading and etc
			// the answer: running it  int.MaxValue / 64 times it tended to be around 100ms faster
			double ldx = p.x - (v1.x + u * (v2.x - v1.x));
			double ldy = p.y - (v1.y + u * (v2.y - v1.y));
			return ldx * ldx + ldy * ldy;
		}

		// This returns the shortest distance from given coordinates to line
		public double SafeDistanceTo(Vector2D p, bool bounded)
		{
			return Math.Sqrt(SafeDistanceToSq(p, bounded));
		}

		// This returns the shortest distance from given coordinates to line
		public double DistanceToSq(Vector2D p, bool bounded)
		{
			Vector2D v1 = start.Position;
			Vector2D v2 = end.Position;

			// Calculate intersection offset
			double u = ((p.x - v1.x) * (v2.x - v1.x) + (p.y - v1.y) * (v2.y - v1.y)) * lengthsqinv;

			// Limit intersection offset to the line
			if(bounded) if(u < 0.0) u = 0.0; else if(u > 1.0) u = 1.0;
			
			// Calculate intersection point
			Vector2D i = v1 + u * (v2 - v1);

			// Return distance between intersection and point
			// which is the shortest distance to the line
			double ldx = p.x - i.x;
			double ldy = p.y - i.y;
			return ldx * ldx + ldy * ldy;
		}

		// This returns the shortest distance from given coordinates to line
		public double DistanceTo(Vector2D p, bool bounded)
		{
			return Math.Sqrt(DistanceToSq(p, bounded));
		}

		// This tests on which side of the line the given coordinates are
		// returns < 0 for front (right) side, > 0 for back (left) side and 0 if on the line
		public double SideOfLine(Vector2D p)
		{
			Vector2D v1 = start.Position;
			Vector2D v2 = end.Position;
			
			// Calculate and return side information
			return (p.y - v1.y) * (v2.x - v1.x) - (p.x - v1.x) * (v2.y - v1.y);
		}

		// This splits this line by vertex v
		// Returns the new line resulting from the split, or null when it failed
		public Linedef Split(Vertex v)
		{
			Sidedef nsd;

			// Copy linedef and change vertices
			Linedef nl = map.CreateLinedef(v, end);
			if(nl == null) return null;
			CopyPropertiesTo(nl);
			SetEndVertex(v);
			nl.Selected = this.Selected;
			nl.marked = this.marked;
			nl.ExtraFloorFlag = this.ExtraFloorFlag; //mxd
			
			// Copy front sidedef if exists
			if(front != null)
			{
				nsd = map.CreateSidedef(nl, true, front.Sector);
				if(nsd == null) return null;
				front.CopyPropertiesTo(nsd);
				nsd.Marked = front.Marked;
			}

			// Copy back sidedef if exists
			if(back != null)
			{
				nsd = map.CreateSidedef(nl, false, back.Sector);
				if(nsd == null) return null;
				back.CopyPropertiesTo(nsd);
				nsd.Marked = back.Marked;
			}

			//mxd
			AdjustSplitCoordinates(this, nl, General.Settings.SplitLineBehavior);

			// Return result
			General.Map.IsChanged = true;
			return nl;
		}
		
		// This joins the line with another line
		// This line will be disposed
		// Returns false when the operation could not be completed
		public bool Join(Linedef other)
		{
			// Check which lines were 2 sided
			bool otherwas2s = ((other.Front != null) && (other.Back != null));
			bool thiswas2s = ((this.Front != null) && (this.Back != null));
			
			// Get sector references
			Sector otherfs = (other.front != null ? other.front.Sector : null);
			Sector otherbs = (other.back != null ? other.back.Sector : null);
			Sector thisfs = (this.front != null ? this.front.Sector : null);
			Sector thisbs = (this.back != null ? this.back.Sector : null);

			// This line has no sidedefs?
			if((thisfs == null) && (thisbs == null))
			{
				// We have no sidedefs, so we have no influence
				// Nothing to change on the other line
			}
			// Other line has no sidedefs?
			else if((otherfs == null) && (otherbs == null))
			{
				// The other has no sidedefs, so it has no influence
				// Copy my sidedefs to the other
				if(this.Start == other.Start)
				{
					if(!JoinChangeSidedefs(other, true, front)) return false;
					if(!JoinChangeSidedefs(other, false, back)) return false;
				}
				else
				{
					if(!JoinChangeSidedefs(other, false, front)) return false;
					if(!JoinChangeSidedefs(other, true, back)) return false;
				}

				// Copy my properties to the other
				this.CopyPropertiesTo(other);
			}
			else
			{
				// Compare front sectors
				if((otherfs != null) && (otherfs == thisfs))
				{
					// Copy textures
					if(other.front != null) other.front.AddTexturesTo(this.back);
					if(this.front != null) this.front.AddTexturesTo(other.back);

					// Change sidedefs?
					if(!JoinChangeSidedefs(other, true, back)) return false;
				}
				// Compare back sectors
				else if((otherbs != null) && (otherbs == thisbs))
				{
					// Copy textures
					if(other.back != null) other.back.AddTexturesTo(this.front);
					if(this.back != null) this.back.AddTexturesTo(other.front);

					// Change sidedefs?
					if(!JoinChangeSidedefs(other, false, front)) return false;
				}
				// Compare front and back
				else if((otherfs != null) && (otherfs == thisbs))
				{
					// Copy textures
					if(other.front != null) other.front.AddTexturesTo(this.front);
					if(this.back != null) this.back.AddTexturesTo(other.back);

					// Change sidedefs?
					if(!JoinChangeSidedefs(other, true, front)) return false;
				}
				// Compare back and front
				else if((otherbs != null) && (otherbs == thisfs))
				{
					// Copy textures
					if(other.back != null) other.back.AddTexturesTo(this.back);
					if(this.front != null) this.front.AddTexturesTo(other.front);

					// Change sidedefs?
					if(!JoinChangeSidedefs(other, false, back)) return false;
				}
				else
				{
					// Other line single sided?
					if(other.back == null)
					{
						// This line with its back to the other?
						if(this.start == other.end)
						{
							// Copy textures
							if(this.back != null) this.back.AddTexturesTo(other.front);

							// Change sidedefs?
							if(!JoinChangeSidedefs(other, false, front)) return false;
						}
						else
						{
							// Copy textures
							if(this.front != null) this.front.AddTexturesTo(other.front);

							// Change sidedefs?
							if(!JoinChangeSidedefs(other, false, back)) return false;
						}
					}
					// This line single sided?
					else if(this.back == null)
					{
						// Other line with its back to this?
						if(other.start == this.end)
						{
							if(otherbs == null)
							{
								// Copy textures
								if(other.back != null) other.back.AddTexturesTo(this.front);

								// Change sidedefs
								if(!JoinChangeSidedefs(other, false, front)) return false;
							}
						}
						else
						{
							if(otherfs == null)
							{
								// Copy textures
								if(other.front != null) other.front.AddTexturesTo(this.front);

								// Change sidedefs
								if(!JoinChangeSidedefs(other, true, front)) return false;
							}
						}
					}
					else
					{
						// This line with its back to the other?
						if(this.start == other.end)
						{
							// Copy textures
							if(other.back != null) other.back.AddTexturesTo(this.front);
							if(this.back != null) this.back.AddTexturesTo(other.front);

							// Change sidedefs
							if(!JoinChangeSidedefs(other, false, front)) return false;
						}
						// Both lines face the same way
						else
						{
							// Copy textures
							if(other.back != null) other.back.AddTexturesTo(this.back);
							if(this.front != null) this.front.AddTexturesTo(other.front);

							// Change sidedefs
							if(!JoinChangeSidedefs(other, false, back)) return false;
						}
					}
				}
				
				// Apply single/double sided flags if the double-sided-ness changed
				if( (!otherwas2s && (other.Front != null && other.Back != null)) ||
					 (otherwas2s && (other.Front == null || other.Back == null)) )
					other.ApplySidedFlags();
				
				// Remove unneeded textures
				if(other.front != null) other.front.RemoveUnneededTextures(!(otherwas2s && thiswas2s));
				if(other.back != null) other.back.RemoveUnneededTextures(!(otherwas2s && thiswas2s));
			}
			
			// If either of the two lines was selected, keep the other selected
			if(this.Selected) other.Selected = true;
			if(this.marked) other.marked = true;
			
			// I got killed by the other.
			this.Dispose();
			General.Map.IsChanged = true;
			return true;
		}
		
		// This changes sidedefs (used for joining lines)
		// target:		The linedef on which to remove or create a new sidedef
		// front:		Side on which to remove or create the sidedef (true for front side)
		// newside:		The side from which to copy the properties to the new sidedef.
		//				If this is null, no sidedef will be created (only removed)
		// Returns false when the operation could not be completed.
		private bool JoinChangeSidedefs(Linedef target, bool front, Sidedef newside)
		{
			// Change sidedefs
			if(front)
			{
				if(target.front != null) target.front.Dispose();
			}
			else
			{
				if(target.back != null) target.back.Dispose();
			}
			
			if(newside != null)
			{
				Sidedef sd = map.CreateSidedef(target, front, newside.Sector);
				if(sd == null) return false;
				newside.CopyPropertiesTo(sd);
				sd.Marked = newside.Marked;
			}

			return true;
		}

		//mxd
		internal void UpdateColorPreset() 
		{
			for(int i = 0; i < General.Map.ConfigSettings.LinedefColorPresets.Length; i++) 
			{
				if(General.Map.ConfigSettings.LinedefColorPresets[i].Matches(this)) 
				{
					colorPresetIndex = i;
					return;
				}
			}
			colorPresetIndex = -1;
		}

		/// <summary>
		/// Changes the linedef's index to a new index.
		/// </summary>
		/// <param name="newindex">The new index to set</param>
		public void ChangeIndex(int newindex)
		{
			General.Map.UndoRedo.RecIndexLinedef(Index, newindex);
			map?.ChangeLindefIndex(Index, newindex);
		}

		// String representation
		public override string ToString()
		{
#if DEBUG
			string starttext = (start != null ? " (" + start : string.Empty);
			string endtext = (end != null ? ", " + end + ")" : string.Empty);
			return "Linedef " + listindex + (marked ? " (marked)" : "") + starttext + endtext; //mxd
#else
			return "Linedef " + listindex;
#endif
		}
		
		#endregion

		#region ================== Changes
		
		// This updates all properties
		public void Update(Dictionary<string, bool> flags, ushort rawflags, int activate, List<int> tags, int action, int[] args)
		{
			BeforePropsChange();
			
			// Apply changes
			this.flags = new Dictionary<string, bool>(flags);
			this.rawflags = rawflags;
			this.tags = new List<int>(tags); //mxd
			this.activate = activate;
			this.action = action;
			this.args = new int[NUM_ARGS];
			args.CopyTo(this.args, 0);
			this.updateneeded = true;
		}

		// mxd. Moved here from BuilderModes.BuilderPlug
		// This adjusts texture coordinates for splitted lines according to the user preferences
		private static void AdjustSplitCoordinates(Linedef oldline, Linedef newline, SplitLineBehavior splitlinebehavior) 
		{
			switch(splitlinebehavior) 
			{
				case SplitLineBehavior.Interpolate:
					//Make texture offset adjustments
					if(oldline.back != null) 
					{
						if((oldline.back.MiddleRequired() && oldline.back.LongMiddleTexture != MapSet.EmptyLongName) || oldline.back.HighRequired() || oldline.back.LowRequired()) 
						{
							int distance = (int)Vector2D.Distance(newline.start.Position, newline.end.Position);
							if(General.Map.UDMF && General.Map.Config.UseLocalSidedefTextureOffsets) 
							{
								if(distance != 0) oldline.back.SetUdmfTextureOffsetX(distance);
							} 
							else 
							{
								oldline.back.OffsetX += distance;
							}
						}
					}

					if(newline.front != null && ((newline.front.MiddleRequired() || newline.front.LongMiddleTexture != MapSet.EmptyLongName) || newline.front.HighRequired() || newline.front.LowRequired())) 
					{
						int distance = (int)Vector2D.Distance(oldline.start.Position, oldline.end.Position);
						if(General.Map.UDMF && General.Map.Config.UseLocalSidedefTextureOffsets) 
						{
							if(distance != 0) newline.front.SetUdmfTextureOffsetX(distance);
						} 
						else 
						{
							newline.front.OffsetX += distance;
						}
					}

					//Clamp texture coordinates
					if((oldline.front != null) && (newline.front != null)) 
					{
						//get texture
						ImageData texture = null;

						if(newline.front.MiddleRequired() && newline.front.LongMiddleTexture != MapSet.EmptyLongName && General.Map.Data.GetTextureExists(newline.front.LongMiddleTexture)) 
							texture = General.Map.Data.GetTextureImage(newline.front.MiddleTexture);
						else if(newline.front.HighRequired() && newline.front.LongHighTexture != MapSet.EmptyLongName && General.Map.Data.GetTextureExists(newline.front.LongHighTexture)) 
							texture = General.Map.Data.GetTextureImage(newline.front.HighTexture);
						else if(newline.front.LowRequired() && newline.front.LongLowTexture != MapSet.EmptyLongName && General.Map.Data.GetTextureExists(newline.front.LongLowTexture)) 
							texture = General.Map.Data.GetTextureImage(newline.front.LowTexture);

						//clamp offsetX
						if(texture != null && texture.IsImageLoaded) 
							newline.front.OffsetX %= texture.Width;
					}

					if((oldline.back != null) && (newline.back != null)) 
					{
						//get texture
						ImageData texture = null;

						if(newline.back.MiddleRequired() && newline.back.LongMiddleTexture != MapSet.EmptyLongName && General.Map.Data.GetTextureExists(newline.back.LongMiddleTexture))
							texture = General.Map.Data.GetTextureImage(newline.back.MiddleTexture);
						else if(newline.back.HighRequired() && newline.back.LongHighTexture != MapSet.EmptyLongName && General.Map.Data.GetTextureExists(newline.back.LongHighTexture))
							texture = General.Map.Data.GetTextureImage(newline.back.HighTexture);
						else if(newline.back.LowRequired() && newline.back.LongLowTexture != MapSet.EmptyLongName && General.Map.Data.GetTextureExists(newline.back.LongLowTexture))
							texture = General.Map.Data.GetTextureImage(newline.back.LowTexture);

						//clamp offsetX
						if(texture != null && texture.IsImageLoaded) 
							newline.back.OffsetX %= texture.Width;
					}

					break;

				case SplitLineBehavior.CopyXY:
					if((oldline.front != null) && (newline.front != null)) 
					{
						newline.front.OffsetX = oldline.front.OffsetX;
						newline.front.OffsetY = oldline.front.OffsetY;

						//mxd. Copy UDMF offsets as well
						if(General.Map.UDMF && General.Map.Config.UseLocalSidedefTextureOffsets) 
						{
							UniFields.SetFloat(newline.front.Fields, "offsetx_top", oldline.front.Fields.GetValue("offsetx_top", 0.0));
							UniFields.SetFloat(newline.front.Fields, "offsetx_mid", oldline.front.Fields.GetValue("offsetx_mid", 0.0));
							UniFields.SetFloat(newline.front.Fields, "offsetx_bottom", oldline.front.Fields.GetValue("offsetx_bottom", 0.0));

							UniFields.SetFloat(newline.front.Fields, "offsety_top", oldline.front.Fields.GetValue("offsety_top", 0.0));
							UniFields.SetFloat(newline.front.Fields, "offsety_mid", oldline.front.Fields.GetValue("offsety_mid", 0.0));
							UniFields.SetFloat(newline.front.Fields, "offsety_bottom", oldline.front.Fields.GetValue("offsety_bottom", 0.0));
						}
					}

					if((oldline.back != null) && (newline.back != null)) 
					{
						newline.back.OffsetX = oldline.back.OffsetX;
						newline.back.OffsetY = oldline.back.OffsetY;

						//mxd. Copy UDMF offsets as well
						if(General.Map.UDMF && General.Map.Config.UseLocalSidedefTextureOffsets) 
						{
							UniFields.SetFloat(newline.back.Fields, "offsetx_top", oldline.back.Fields.GetValue("offsetx_top", 0.0));
							UniFields.SetFloat(newline.back.Fields, "offsetx_mid", oldline.back.Fields.GetValue("offsetx_mid", 0.0));
							UniFields.SetFloat(newline.back.Fields, "offsetx_bottom", oldline.back.Fields.GetValue("offsetx_bottom", 0.0));

							UniFields.SetFloat(newline.back.Fields, "offsety_top", oldline.back.Fields.GetValue("offsety_top", 0.0));
							UniFields.SetFloat(newline.back.Fields, "offsety_mid", oldline.back.Fields.GetValue("offsety_mid", 0.0));
							UniFields.SetFloat(newline.back.Fields, "offsety_bottom", oldline.back.Fields.GetValue("offsety_bottom", 0.0));
						}
					}
					break;

				case SplitLineBehavior.ResetXCopyY:
					if((oldline.front != null) && (newline.front != null)) 
					{
						newline.front.OffsetX = 0;
						newline.front.OffsetY = oldline.front.OffsetY;

						//mxd. Reset UDMF X offset as well
						if(General.Map.UDMF && General.Map.Config.UseLocalSidedefTextureOffsets) 
						{
							UniFields.SetFloat(newline.front.Fields, "offsetx_top", 0.0);
							UniFields.SetFloat(newline.front.Fields, "offsetx_mid", 0.0);
							UniFields.SetFloat(newline.front.Fields, "offsetx_bottom", 0.0);
						}
					}

					if((oldline.back != null) && (newline.back != null)) 
					{
						newline.back.OffsetX = 0;
						newline.back.OffsetY = oldline.back.OffsetY;

						//mxd. Reset UDMF X offset and copy Y offset as well
						if(General.Map.UDMF && General.Map.Config.UseLocalSidedefTextureOffsets) 
						{
							UniFields.SetFloat(newline.back.Fields, "offsetx_top", 0.0);
							UniFields.SetFloat(newline.back.Fields, "offsetx_mid", 0.0);
							UniFields.SetFloat(newline.back.Fields, "offsetx_bottom", 0.0);

							UniFields.SetFloat(newline.back.Fields, "offsety_top", oldline.back.Fields.GetValue("offsety_top", 0.0));
							UniFields.SetFloat(newline.back.Fields, "offsety_mid", oldline.back.Fields.GetValue("offsety_mid", 0.0));
							UniFields.SetFloat(newline.back.Fields, "offsety_bottom", oldline.back.Fields.GetValue("offsety_bottom", 0.0));
						}
					}
					break;

				case SplitLineBehavior.ResetXY:
					if(newline.front != null) 
					{
						newline.front.OffsetX = 0;
						newline.front.OffsetY = 0;

						if(General.Map.UDMF && General.Map.Config.UseLocalSidedefTextureOffsets) 
						{
							UniFields.SetFloat(newline.front.Fields, "offsetx_top", 0.0);
							UniFields.SetFloat(newline.front.Fields, "offsetx_mid", 0.0);
							UniFields.SetFloat(newline.front.Fields, "offsetx_bottom", 0.0);

							UniFields.SetFloat(newline.front.Fields, "offsety_top", 0.0);
							UniFields.SetFloat(newline.front.Fields, "offsety_mid", 0.0);
							UniFields.SetFloat(newline.front.Fields, "offsety_bottom", 0.0);
						}
					}

					if(newline.back != null) 
					{
						newline.back.OffsetX = 0;
						newline.back.OffsetY = 0;

						if(General.Map.UDMF && General.Map.Config.UseLocalSidedefTextureOffsets) 
						{
							UniFields.SetFloat(newline.back.Fields, "offsetx_top", 0.0);
							UniFields.SetFloat(newline.back.Fields, "offsetx_mid", 0.0);
							UniFields.SetFloat(newline.back.Fields, "offsetx_bottom", 0.0);

							UniFields.SetFloat(newline.back.Fields, "offsety_top", 0.0);
							UniFields.SetFloat(newline.back.Fields, "offsety_mid", 0.0);
							UniFields.SetFloat(newline.back.Fields, "offsety_bottom", 0.0);
						}
					}
					break;
			}
		}

		#endregion
	}
}
