
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
using CodeImp.DoomBuilder.IO;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Data;

#endregion

namespace CodeImp.DoomBuilder.Map
{
	public sealed class Sidedef : MapElement
	{
		#region ================== Variables

		// Map
		private MapSet map;

		// List items
		private LinkedListNode<Sidedef> sectorlistitem;

		// Owner
		private Linedef linedef;
		
		// Sector
		private Sector sector;

		// Properties
		private int offsetx;
		private int offsety;
		private string texnamehigh;
		private string texnamemid;
		private string texnamelow;
		private long longtexnamehigh;
		private long longtexnamemid;
		private long longtexnamelow;

		//mxd. UDMF properties
		private Dictionary<string, bool> flags;

		// Clone
		private int serializedindex;

        // Rendering
        private int lastProcessed;
		
		#endregion

		#region ================== Properties

		public MapSet Map { get { return map; } }
		public bool IsFront { get { return (linedef != null) && (this == linedef.Front); } }
		public Linedef Line { get { return linedef; } }
		public Sidedef Other { get { return (this == linedef.Front ? linedef.Back : linedef.Front); } }
		public Sector Sector { get { return sector; } }
		internal Dictionary<string, bool> Flags { get { return flags; } } //mxd
		public double Angle { get { return (IsFront ? linedef.Angle : Angle2D.Normalized(linedef.Angle + Angle2D.PI)); } }
		public int OffsetX { get { return offsetx; } set { BeforePropsChange(); offsetx = value; } }
		public int OffsetY { get { return offsety; } set { BeforePropsChange(); offsety = value; } }
		public string HighTexture { get { return texnamehigh; } }
		public string MiddleTexture { get { return texnamemid; } }
		public string LowTexture { get { return texnamelow; } }
		public long LongHighTexture { get { return longtexnamehigh; } }
		public long LongMiddleTexture { get { return longtexnamemid; } }
		public long LongLowTexture { get { return longtexnamelow; } }
		internal int SerializedIndex { get { return serializedindex; } set { serializedindex = value; } }
        internal int LastProcessed { get { return lastProcessed; } set { lastProcessed = value; } }
		
		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		internal Sidedef(MapSet map, int listindex, Linedef l, bool front, Sector s)
		{
			// Initialize
			this.elementtype = MapElementType.SIDEDEF; //mxd
			this.map = map;
			this.listindex = listindex;
			this.texnamehigh = "-";
			this.texnamemid = "-";
			this.texnamelow = "-";
			this.longtexnamehigh = MapSet.EmptyLongName;
			this.longtexnamemid = MapSet.EmptyLongName;
			this.longtexnamelow = MapSet.EmptyLongName;
			this.flags = new Dictionary<string, bool>(StringComparer.Ordinal); //mxd
			
			// Attach linedef
			this.linedef = l;
			if(l != null)
			{
				if(front)
					l.AttachFrontP(this);
				else
					l.AttachBackP(this);
			}
			
			// Attach sector
			SetSectorP(s);
			
			if(map == General.Map.Map)
				General.Map.UndoRedo.RecAddSidedef(this);
			
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

				if(map == General.Map.Map)
					General.Map.UndoRedo.RecRemSidedef(this);

				// Remove from main list
				map.RemoveSidedef(listindex);

				// Detach from linedef
				if(linedef != null) linedef.DetachSidedefP(this);
				
				// Detach from sector
				SetSectorP(null);

				// Clean up
				sectorlistitem = null;
				linedef = null;
				map = null;
				sector = null;

				//mxd. Restore isdisposed so base classes can do their disposal job
				isdisposed = false;

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
				General.Map.UndoRedo.RecPrpSidedef(this);
		}
		
		// Serialize / deserialize (passive: this doesn't record)
		new internal void ReadWrite(IReadWriteStream s)
		{
			if(!s.IsWriting) BeforePropsChange();
			
			base.ReadWrite(s);

			//mxd
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
				int c;
				s.rInt(out c);

				flags = new Dictionary<string, bool>(c, StringComparer.Ordinal);
				for(int i = 0; i < c; i++) 
				{
					string t;
					s.rString(out t);
					bool b;
					s.rBool(out b);
					flags.Add(t, b);
				}
			}

			s.rwInt(ref offsetx);
			s.rwInt(ref offsety);
			s.rwString(ref texnamehigh);
			s.rwString(ref texnamemid);
			s.rwString(ref texnamelow);
			s.rwLong(ref longtexnamehigh);
			s.rwLong(ref longtexnamemid);
			s.rwLong(ref longtexnamelow);
		}
		
		// This copies all properties to another sidedef
		public void CopyPropertiesTo(Sidedef s)
		{
			s.BeforePropsChange();
			
			// Copy properties
			s.offsetx = offsetx;
			s.offsety = offsety;
			s.texnamehigh = texnamehigh;
			s.texnamemid = texnamemid;
			s.texnamelow = texnamelow;
			s.longtexnamehigh = longtexnamehigh;
			s.longtexnamemid = longtexnamemid;
			s.longtexnamelow = longtexnamelow;
			s.flags = new Dictionary<string, bool>(flags); //mxd
			base.CopyPropertiesTo(s);
		}

		// This changes sector
		public void SetSector(Sector newsector)
		{
			if(map == General.Map.Map)
				General.Map.UndoRedo.RecRefSidedefSector(this);
			
			// Change sector
			SetSectorP(newsector);
		}

		internal void SetSectorP(Sector newsector)
		{
			// Detach from sector
			if(sector != null && !sector.IsDisposed) //mxd
				sector.DetachSidedefP(sectorlistitem);

			// Change sector
			sector = newsector;

			// Attach to sector
			if(sector != null && !sector.IsDisposed) //mxd
				sectorlistitem = sector.AttachSidedefP(this);

			General.Map.IsChanged = true;
		}

		// This sets the linedef
		public void SetLinedef(Linedef ld, bool front)
		{
			if(linedef != null) linedef.DetachSidedefP(this);
			
			if(ld != null)
			{
				if(front)
					ld.AttachFront(this);
				else
					ld.AttachBack(this);
			}
		}

		// This sets the linedef (passive: this doesn't tell the linedef and doesn't record)
		internal void SetLinedefP(Linedef ld)
		{
			linedef = ld;
		}

		//mxd. This translates UDMF fields back into the normal flags and activations
		internal void TranslateFromUDMF() 
		{
			// Try to translate UDMF texture offsets to regular ones
			if(longtexnamemid != MapSet.EmptyLongName && MiddleRequired()) 
			{
				offsetx += (int)UniFields.GetFloat(this.Fields, "offsetx_mid");
				offsety += (int)UniFields.GetFloat(this.Fields, "offsety_mid");
			}
			else if(longtexnamehigh != MapSet.EmptyLongName && HighRequired()) 
			{
				offsetx += (int)UniFields.GetFloat(this.Fields, "offsetx_top");
				offsety += (int)UniFields.GetFloat(this.Fields, "offsety_top");
			}
			else if(longtexnamelow != MapSet.EmptyLongName && LowRequired()) 
			{
				offsetx += (int)UniFields.GetFloat(this.Fields, "offsetx_bottom");
				offsety += (int)UniFields.GetFloat(this.Fields, "offsety_bottom");
			}
			
			// Clear UDMF-related properties
			this.Fields.Clear();
			this.Flags.Clear();
		}
		
		#endregion

		#region ================== Methods

		// This checks and returns a flag without creating it
		public bool IsFlagSet(string flagname) 
		{
			return (flags.ContainsKey(flagname) && flags[flagname]);
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
		}
		
		// This removes textures that are not required
		public void RemoveUnneededTextures(bool removemiddle) { RemoveUnneededTextures(removemiddle, false, false); }
		public void RemoveUnneededTextures(bool removemiddle, bool force, bool shiftmiddle)
		{
			bool changed = false; //mxd

			// Check if the line or sectors have no action or tags because
			// if they do, any texture on this side could be needed
			if(force || ((linedef.Tag == 0) && (linedef.Action == 0) && (sector.Tag == 0) &&
				((Other == null) || (Other.sector.Tag == 0))))
			{
				if(General.Settings.AutoClearSidedefTextures && !HighRequired())
				{
					BeforePropsChange(); //mxd
					changed = true;
					this.texnamehigh = "-";
					this.longtexnamehigh = MapSet.EmptyLongName;
					General.Map.IsChanged = true;
				} 
				else if(shiftmiddle && this.longtexnamehigh == MapSet.EmptyLongName && HighRequired()) //mxd
				{
					SetTextureHigh(this.texnamemid);
					changed = true;
				}

				if(General.Settings.AutoClearSidedefTextures && !LowRequired())
				{
					if(!changed) //mxd
					{
						BeforePropsChange();
						changed = true;
					}
					this.texnamelow = "-";
					this.longtexnamelow = MapSet.EmptyLongName;
					General.Map.IsChanged = true;
				}
				else if(shiftmiddle && this.longtexnamelow == MapSet.EmptyLongName && LowRequired()) //mxd 
				{
					SetTextureLow(this.texnamemid);
					changed = true;
				}
			}

			// The middle texture can be removed regardless of any sector tag or linedef action
			if(!MiddleRequired() && removemiddle) 
			{
				if(!changed) BeforePropsChange(); //mxd
				this.texnamemid = "-";
				this.longtexnamemid = MapSet.EmptyLongName;
				General.Map.IsChanged = true;
			}
		}
		
		/// <summary>
		/// This checks if a texture is required
		/// </summary>
		public bool HighRequired()
		{
			// Doublesided?
			if(Other == null || Other.sector.HasSkyCeiling) return false;

			//mxd. Check sloped ceilings...
			if(General.Map.UDMF && this.sector != Other.Sector) 
			{
				double thisstartz = this.sector.CeilHeight;
				double thisendz = this.sector.CeilHeight;
				double otherstartz = Other.sector.CeilHeight;
				double otherendz = Other.sector.CeilHeight;

				// Check if this side is affected by UDMF slope (it overrides vertex heights, riiiiiight?..) TODO: check this!
				if(this.sector.CeilSlope.GetLengthSq() > 0) 
				{
					Plane ceil = new Plane(this.sector.CeilSlope, this.sector.CeilSlopeOffset);
					thisstartz = ceil.GetZ(this.Line.Start.Position);
					thisendz = ceil.GetZ(this.Line.End.Position);
				} 
				else if(this.sector.Sidedefs.Count == 3) // Check vertex heights on this side
				{
					if(!double.IsNaN(this.Line.Start.ZCeiling)) thisstartz = this.Line.Start.ZCeiling;
					if(!double.IsNaN(this.Line.End.ZCeiling)) thisendz = this.Line.End.ZCeiling;
				}

				// Check if other side is affected by UDMF slope (it overrides vertex heights, riiiiiight?..) TODO: check this!
				if(Other.sector.CeilSlope.GetLengthSq() > 0) 
				{
					Plane ceil = new Plane(Other.sector.CeilSlope, Other.sector.CeilSlopeOffset);
					otherstartz = ceil.GetZ(this.Line.Start.Position);
					otherendz = ceil.GetZ(this.Line.End.Position);
				} 
				else if(Other.sector.Sidedefs.Count == 3) // Check other line's vertex heights
				{
					if(!double.IsNaN(this.Line.Start.ZCeiling)) otherstartz = this.Line.Start.ZCeiling;
					if(!double.IsNaN(this.Line.End.ZCeiling)) otherendz = this.Line.End.ZCeiling;
				}

				// Texture is required when our start or end vertex is higher than on the other side.
				if(thisstartz > otherstartz || thisendz > otherendz) return true;
			}
				
			// Texture is required when ceiling of other side is lower
			return Other.sector.CeilHeight < this.sector.CeilHeight;
		}

		/// <summary>
		/// This checks if a texture is required
		/// </summary>
		public bool MiddleRequired()
		{
			// Texture is required when the line is singlesided
			return (Other == null);
		}

		/// <summary>
		/// This checks if a texture is required
		/// </summary>
		public bool LowRequired()
		{
			// Doublesided?
			if (Other == null || Other.sector.HasSkyFloor) return false;

			//mxd. Check sloped floors...
			if(General.Map.UDMF && this.sector != Other.Sector)
			{
				double thisstartz = this.sector.FloorHeight;
				double thisendz = this.sector.FloorHeight;
				double otherstartz = Other.sector.FloorHeight;
				double otherendz = Other.sector.FloorHeight;

				// Check if this side is affected by UDMF slope (it overrides vertex heights, riiiiiight?..) TODO: check this!
				if(this.sector.FloorSlope.GetLengthSq() > 0) 
				{
					Plane floor = new Plane(this.sector.FloorSlope, this.sector.FloorSlopeOffset);
					thisstartz = floor.GetZ(this.Line.Start.Position);
					thisendz = floor.GetZ(this.Line.End.Position);
				} 
				else if(this.sector.Sidedefs.Count == 3) // Check vertex heights on this side
				{
					if(!double.IsNaN(this.Line.Start.ZFloor)) thisstartz = this.Line.Start.ZFloor;
					if(!double.IsNaN(this.Line.End.ZFloor)) thisendz = this.Line.End.ZFloor;
				}
					
				// Check if other side is affected by UDMF slope (it overrides vertex heights, riiiiiight?..) TODO: check this!
				if(Other.sector.FloorSlope.GetLengthSq() > 0)
				{
					Plane floor = new Plane(Other.sector.FloorSlope, Other.sector.FloorSlopeOffset);
					otherstartz = floor.GetZ(this.Line.Start.Position);
					otherendz = floor.GetZ(this.Line.End.Position);
				}
				else if(Other.sector.Sidedefs.Count == 3) // Check other line's vertex heights
				{
					if(!double.IsNaN(this.Line.Start.ZFloor)) otherstartz = this.Line.Start.ZFloor;
					if(!double.IsNaN(this.Line.End.ZFloor)) otherendz = this.Line.End.ZFloor;
				}

				// Texture is required when our start or end vertex is lower than on the other side.
				if(thisstartz < otherstartz || thisendz < otherendz) return true;
			}

			// Texture is required when floor of other side is higher
			return Other.sector.FloorHeight > this.sector.FloorHeight;
		}

		/// <summary>
		/// This returns the height of the upper wall part. Returns 0 when no upper part exists.
		/// </summary>
		public int GetHighHeight()
		{
			Sidedef other = this.Other;
			if(other != null)
			{
				int top = this.sector.CeilHeight;
				int bottom = other.sector.CeilHeight;
				int height = top - bottom;
				return (height > 0) ? height : 0;
			}

			return 0;
		}

		/// <summary>
		/// This returns the height of the middle wall part.
		/// </summary>
		public int GetMiddleHeight()
		{
			Sidedef other = this.Other;
			if(other != null)
			{
				int top = Math.Min(this.Sector.CeilHeight, other.Sector.CeilHeight);
				int bottom = Math.Max(this.Sector.FloorHeight, other.Sector.FloorHeight);
				int height = top - bottom;
				return (height > 0) ? height : 0;
			}
			else
			{
				int top = this.Sector.CeilHeight;
				int bottom = this.Sector.FloorHeight;
				int height = top - bottom;
				return (height > 0) ? height : 0;
			}
		}

		/// <summary>
		/// This returns the height of the lower wall part. Returns 0 when no lower part exists.
		/// </summary>
		public int GetLowHeight()
		{
			Sidedef other = this.Other;
			if(other != null)
			{
				int top = other.sector.FloorHeight;
				int bottom = this.sector.FloorHeight;
				int height = top - bottom;
				return (height > 0) ? height : 0;
			}

			return 0;
		}
		
		// This creates a checksum from the sidedef properties
		// Used for faster sidedefs compression
		public uint GetChecksum()
		{
			CRC crc = new CRC();
			crc.Add(sector.FixedIndex);
			crc.Add(offsetx);
			crc.Add(offsety);
			crc.Add(longtexnamehigh);
			crc.Add(longtexnamelow);
			crc.Add(longtexnamemid);
			return (uint)(crc.Value & 0x00000000FFFFFFFF);
		}

		// This copies textures to another sidedef
		// And possibly also the offsets
		public void AddTexturesTo(Sidedef s)
		{
			int copyoffsets = 0;

			// s cannot be null
			if(s == null) return;

			s.BeforePropsChange();

			// Upper texture set?
			if((texnamehigh.Length > 0) && (texnamehigh != "-"))
			{
				// Copy upper texture
				s.texnamehigh = texnamehigh;
				s.longtexnamehigh = longtexnamehigh;

				// Counts as a half coice for copying offsets
				copyoffsets += 1;

				//mxd. Also copy UDMF offsets and scale
				if(General.Map.UDMF)
				{
					UniFields.SetFloat(s.Fields, "offsetx_top", Fields.GetValue("offsetx_top", 0.0), 0.0);
					UniFields.SetFloat(s.Fields, "offsety_top", Fields.GetValue("offsety_top", 0.0), 0.0);
					UniFields.SetFloat(s.Fields, "scalex_top",  Fields.GetValue("scalex_top", 1.0), 1.0);
					UniFields.SetFloat(s.Fields, "scaley_top",  Fields.GetValue("scaley_top", 1.0), 1.0);
				}
			}

			// Middle texture set?
			if((texnamemid.Length > 0) && (texnamemid != "-"))
			{
				// Copy middle texture
				s.texnamemid = texnamemid;
				s.longtexnamemid = longtexnamemid;

				// Counts for copying offsets
				copyoffsets += 2;

				//mxd. Also copy UDMF offsets and scale
				if(General.Map.UDMF)
				{
					UniFields.SetFloat(s.Fields, "offsetx_mid", Fields.GetValue("offsetx_mid", 0.0), 0.0);
					UniFields.SetFloat(s.Fields, "offsety_mid", Fields.GetValue("offsety_mid", 0.0), 0.0);
					UniFields.SetFloat(s.Fields, "scalex_mid",  Fields.GetValue("scalex_mid", 1.0), 1.0);
					UniFields.SetFloat(s.Fields, "scaley_mid",  Fields.GetValue("scaley_mid", 1.0), 1.0);
				}
			}

			// Lower texture set?
			if((texnamelow.Length > 0) && (texnamelow != "-"))
			{
				// Copy middle texture
				s.texnamelow = texnamelow;
				s.longtexnamelow = longtexnamelow;

				// Counts as a half coice for copying offsets
				copyoffsets += 1;

				//mxd. Also copy UDMF offsets and scale
				if(General.Map.UDMF)
				{
					UniFields.SetFloat(s.Fields, "offsetx_bottom", Fields.GetValue("offsetx_bottom", 0.0), 0.0);
					UniFields.SetFloat(s.Fields, "offsety_bottom", Fields.GetValue("offsety_bottom", 0.0), 0.0);
					UniFields.SetFloat(s.Fields, "scalex_bottom",  Fields.GetValue("scalex_bottom", 1.0), 1.0);
					UniFields.SetFloat(s.Fields, "scaley_bottom",  Fields.GetValue("scaley_bottom", 1.0), 1.0);
				}
			}

			// Copy offsets also?
			if(copyoffsets >= 2)
			{
				// Copy offsets
				s.offsetx = offsetx;
				s.offsety = offsety;
			}

			General.Map.IsChanged = true;
		}

		//mxd. String representation
		public override string ToString()
		{
#if DEBUG
			return "Sidedef " + listindex + (marked ? " (marked)" : "") + " (Line " + linedef.Index + (linedef.Marked ? " (marked)" : "") + ", Sector " + sector.Index + (sector.Marked ? " (marked)" : "") + ")";
#else
			return "Sidedef " + listindex;
#endif
		}

		
		#endregion

		#region ================== Changes

		// This updates all properties
		public void Update(int offsetx, int offsety, string thigh, string tmid, string tlow) 
		{
			Update(offsetx, offsety, thigh, tmid, tlow, new Dictionary<string, bool>(StringComparer.Ordinal));
		}

		//mxd. This updates all properties (UDMF version)
		public void Update(int offsetx, int offsety, string thigh, string tmid, string tlow, Dictionary<string, bool> flags)
		{
			BeforePropsChange();
			
			// Apply changes
			this.offsetx = offsetx;
			this.offsety = offsety;
			this.flags = new Dictionary<string, bool>(flags); //mxd
			//SetTextureMid(tmid);
			//SetTextureLow(tlow);
			//SetTextureHigh(thigh);

			//mxd. Set mid texture
			texnamemid = string.IsNullOrEmpty(tmid) ? "-" : tmid;
			longtexnamemid = Lump.MakeLongName(tmid);

			//mxd. Set low texture
			texnamelow = string.IsNullOrEmpty(tlow) ? "-" : tlow;
			longtexnamelow = Lump.MakeLongName(tlow);

			//mxd. Set high texture
			texnamehigh = string.IsNullOrEmpty(thigh) ? "-" : thigh;
			longtexnamehigh = Lump.MakeLongName(texnamehigh);

			//mxd. Map is changed
			General.Map.IsChanged = true;
		}

		// This sets texture
		public void SetTextureHigh(string name)
		{
			BeforePropsChange();
			
			texnamehigh = string.IsNullOrEmpty(name) ? "-" : name; //mxd
			longtexnamehigh = Lump.MakeLongName(name);
			General.Map.IsChanged = true;
		}

		// This sets texture
		public void SetTextureMid(string name)
		{
			BeforePropsChange();
			
			texnamemid = string.IsNullOrEmpty(name) ? "-" : name; //mxd;
			longtexnamemid = Lump.MakeLongName(name);
			General.Map.IsChanged = true;
		}

		// This sets texture
		public void SetTextureLow(string name)
		{
			BeforePropsChange();
			
			texnamelow = string.IsNullOrEmpty(name) ? "-" : name; //mxd;
			longtexnamelow = Lump.MakeLongName(name);
			General.Map.IsChanged = true;
		}

		// This sets udmf texture offset
		public void SetUdmfTextureOffsetX(int offset) 
		{
			this.Fields.BeforeFieldsChange();

			//top
			if(longtexnamehigh != MapSet.EmptyLongName && General.Map.Data.GetTextureExists(texnamehigh)) 
			{
				ImageData texture = General.Map.Data.GetTextureImage(texnamehigh);
				double scaleTop = Fields.GetValue("scalex_top", 1.0);

				double value = Fields.GetValue("offsetx_top", 0.0);
				double result = Math.Round(value + offset * scaleTop);
				if(texture.IsImageLoaded) result %= texture.Width;
				UniFields.SetFloat(Fields, "offsetx_top", result);
			}

			//middle
			if(longtexnamemid != MapSet.EmptyLongName && General.Map.Data.GetTextureExists(texnamemid)) 
			{
				ImageData texture = General.Map.Data.GetTextureImage(texnamemid);
				double scaleMid = Fields.GetValue("scalex_mid", 1.0);

				double value = Fields.GetValue("offsetx_mid", 0.0);
				double result = Math.Round(value + offset * scaleMid);
				if(texture.IsImageLoaded) result %= texture.Width;
				UniFields.SetFloat(Fields, "offsetx_mid", result);
			}

			//bottom
			if(longtexnamelow != MapSet.EmptyLongName && General.Map.Data.GetTextureExists(texnamelow)) 
			{
				ImageData texture = General.Map.Data.GetTextureImage(texnamelow);
				double scaleLow = Fields.GetValue("scalex_bottom", 1.0);

				double value = Fields.GetValue("offsetx_bottom", 0.0);
				double result = Math.Round(value + offset * scaleLow);
				if(texture.IsImageLoaded) result %= texture.Width;
				UniFields.SetFloat(Fields, "offsetx_bottom", result);
			}
		}
		
		#endregion
	}
}
