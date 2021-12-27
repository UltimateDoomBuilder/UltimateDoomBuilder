
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
using System.IO;
using CodeImp.DoomBuilder.Compilers;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.ZDoom;

#endregion

namespace CodeImp.DoomBuilder.Data
{
	//mxd
	public class TextResourceData
	{
		private Stream stream;
		private DataReader source;
		private DataLocation sourcelocation;
		private string filename;
		private int lumpindex;
		private bool trackable;

		internal Stream Stream { get { return stream; } }
		internal DataReader Source { get { return source; } }
		internal DataLocation SourceLocation { get { return sourcelocation; } }
		internal string Filename { get { return filename; } } // Lump name/Filename
		internal int LumpIndex { get { return lumpindex; } } // Lump index in a WAD
		internal bool Trackable { get { return trackable; } } // When false, wont be added to DataManager.TextResources


		internal TextResourceData(DataReader source, Stream stream, string filename, bool trackable)
		{
			this.source = source;
			this.sourcelocation = source.Location;
			this.stream = stream;
			this.filename = filename;
			this.trackable = trackable;

			WADReader reader = source as WADReader;
			if(reader != null)
				this.lumpindex = reader.WadFile.FindLumpIndex(filename);
			else
				this.lumpindex = -1;
		}

		internal TextResourceData(DataReader source, Stream stream, string filename, int lumpindex, bool trackable)
		{
			this.source = source;
			this.sourcelocation = source.Location;
			this.stream = stream;
			this.filename = filename;
			this.lumpindex = lumpindex;
			this.trackable = trackable;
		}

		// Adds an untrackable resource without DataReader
		internal TextResourceData(Stream stream, DataLocation location, string filename)
		{
			this.source = null;
			this.sourcelocation = location;
			this.stream = stream;
			this.filename = filename;
			this.lumpindex = -1;
			this.trackable = false;
		}
	}
	
	internal abstract class DataReader : IDisposable
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		protected DataLocation location;
		protected bool issuspended;
		protected bool isdisposed;
		protected bool isreadonly; //mxd
        protected bool wasreadonly; // [ZZ]
		protected ResourceTextureSet textureset;

		#endregion

		#region ================== Properties

		public DataLocation Location { get { return location; } }
		public bool IsDisposed { get { return isdisposed; } }
		public bool IsSuspended { get { return issuspended; } }
		public bool IsReadOnly { get { return (issuspended?wasreadonly:isreadonly); } } //mxd, [ZZ]
		public ResourceTextureSet TextureSet { get { return textureset; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		protected DataReader(DataLocation dl, bool asreadonly)
		{
			// Keep information
			location = dl;
			isreadonly = asreadonly;
			textureset = new ResourceTextureSet(GetTitle(), dl);
		}

		// Disposer
		public virtual void Dispose()
		{
			// Not already disposed?
			if(!isdisposed)
			{
				// Done
				textureset = null;
				isdisposed = true;
			}
		}

		#endregion

		#region ================== Management

		// This returns a short name
		public abstract string GetTitle();

		// This suspends use of this resource
		public virtual void Suspend()
		{
            // [ZZ] validate
            if (issuspended) throw new Exception("Tried to suspend already suspended resource!");
			issuspended = true;
            wasreadonly = isreadonly;
            isreadonly = true;
		}

		// This resumes use of this resource
		public virtual void Resume()
		{
            // [ZZ] validate
            if (!issuspended) throw new Exception("Tried to resume already resumed resource!");
            issuspended = false;
            isreadonly = wasreadonly;
		}

		#endregion

		#region ================== Palette

		// When implemented, this should find and load a PLAYPAL palette
		public virtual Playpal LoadPalette() { return null; }

		public virtual ColorMap LoadMainColorMap(Playpal palette) { return null; }
		
		#endregion

		#region ================== Colormaps

		// When implemented, this loads the colormaps
		public virtual ICollection<ImageData> LoadColormaps() { return null; }

		// When implemented, this returns the colormap lump
		public virtual Stream GetColormapData(string pname) { return null; }

		#endregion

		#region ================== Textures

		// When implemented, this should read the patch names
		public abstract PatchNames LoadPatchNames();

		// When implemented, this returns the patch lump
		public abstract Stream GetPatchData(string pname, bool longname, ref string patchlocation);

		// When implemented, this returns the texture lump
		public abstract Stream GetTextureData(string pname, bool longname, ref string texturelocation);

		// When implemented, this loads the textures
		public abstract IEnumerable<ImageData> LoadTextures(PatchNames pnames, Dictionary<string, TexturesParser> cachedparsers);

		//mxd. When implemented, this returns the HiRes texture lump
		public abstract Stream GetHiResTextureData(string pname, ref string hireslocation);

		//mxd. When implemented, this loads the HiRes textures
		public abstract IEnumerable<HiResImage> LoadHiResTextures();
		
		#endregion

		#region ================== Flats
		
		// When implemented, this loads the flats
		public abstract IEnumerable<ImageData> LoadFlats(Dictionary<string, TexturesParser> cachedparsers);

		// When implemented, this returns the flat lump
		public abstract Stream GetFlatData(string pname, bool longname, ref string flatlocation);
		
		#endregion
		
		#region ================== Sprites

		// When implemented, this loads the sprites
		public abstract IEnumerable<ImageData> LoadSprites(Dictionary<string, TexturesParser> cachedparsers);
		
		// When implemented, this returns the sprite lump
		public abstract Stream GetSpriteData(string pname, ref string spritelocation);

		// When implemented, this checks if the given sprite lump exists
		public abstract bool GetSpriteExists(string pname);

		//mxd. When implemented, returns all sprites, which name starts with given string
		public abstract HashSet<string> GetSpriteNames();

		#endregion

		#region ================== Decorate, Modeldef, Mapinfo, Gldefs, etc...

		// When implemented, this returns DEHACKED lumps
		public abstract IEnumerable<TextResourceData> GetDehackedData();

		// When implemented, this returns DECORATE lumps
		public abstract IEnumerable<TextResourceData> GetDecorateData(string pname);

        // [ZZ] When implemented, this returns ZSCRIPT lumps
        public abstract IEnumerable<TextResourceData> GetZScriptData(string pname);

        // [ZZ] When implemented, this returns MODELDEF lumps
        public abstract IEnumerable<TextResourceData> GetModeldefData(string pname);

        //mxd. When implemented, this returns MAPINFO lumps
        public abstract IEnumerable<TextResourceData> GetMapinfoData();

		//mxd. When implemented, this returns GLDEFS lumps
		public abstract IEnumerable<TextResourceData> GetGldefsData(string basegame);

		//mxd. When implemented, this returns generic text lump data
		public abstract IEnumerable<TextResourceData> GetTextLumpData(ScriptType scripttype, bool singular, bool partialtitlematch);

		//mxd. When implemented, this returns the list of voxel model names
		public abstract HashSet<string> GetVoxelNames();

		//mxd. When implemented, this returns the voxel lump
		public abstract Stream GetVoxelData(string name, ref string voxellocation);

		// When implemented, this returns the list of IWAD infos
		public abstract List<IWadInfo> GetIWadInfos();

		#endregion

		#region ================== Load/Save (mxd)

		internal abstract MemoryStream LoadFile(string name);
		internal abstract MemoryStream LoadFile(string name, int lumpindex);
		internal abstract bool SaveFile(MemoryStream stream, string name);
		internal abstract bool SaveFile(MemoryStream stream, string name, int lumpindex);
		internal abstract bool FileExists(string filename);
		internal abstract bool FileExists(string filename, int lumpindex);

		#endregion

		#region ================== Compiling (mxd)

		internal abstract bool CompileLump(string lumpname, ScriptConfiguration scriptconfig, List<CompilerError> errors);
		internal abstract bool CompileLump(string lumpname, int lumpindex, ScriptConfiguration scriptconfig, List<CompilerError> errors);

		#endregion
	}
}
