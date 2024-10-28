
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
using System.Text;
using System.IO;
using System.Security.Cryptography;

#endregion

namespace CodeImp.DoomBuilder.IO
{
	public class WAD : IDisposable
	{
		#region ================== Constants

		// WAD types
		private const string TYPE_IWAD = "IWAD";
		private const string TYPE_PWAD = "PWAD";
		
		// Encoder
		public static readonly Encoding ENCODING = Encoding.ASCII;

		//mxd. Official IWAD SHA1 hashes
		// Source of hashes: https://github.com/Doom-Utils/iwad-patches
		private static readonly HashSet<string> IWAD_HASHES = new HashSet<string>
		{
			// Doom 1
			"df0040ccb29cc1622e74ceb3b7793a2304cca2c8",  // Doom 1.1
			"b5f86a559642a2b3bdfb8a75e91c8da97f057fe6",  // Doom 1.2
			"2e89b86859acd9fc1e552f587b710751efcffa8e",  // Doom 1.666
			"2c8212631b37f21ad06d18b5638c733a75e179ff",  // Doom 1.8
			"7742089b4468a736cadb659a7deca3320fe6dcbd",  // Doom 1.9
			"e5ec79505530e151ff0e6f517f3ce1fd65969c46",  // Doom Bfg
			"a89b39d91122882214c3088b8cd6b308713bd7c2",  // Doom Ppc
			"117015379c529573510be08cf59810aa10bb934e",  // Doom Psn
			"f770111ca9eb6d49aead51fcbd398719b462e64b",  // Doom Unity 1.0
			"08ab2507e1d525c4c06b6df4f6d5862568a6b009",  // Doom Unity 1.1
			"2a8a1ce0f29497a2781b2902c76115fd60d8bbf8",  // Doom Unity 1.3
			"9b07b02ab3c275a6a7570c3f73cc20d63a0e3833",  // Doom Ud
			"37de4510216eb3ce9a835dd939109443375d10c5",  // Doom Xbla
			"1d1d4f69fe14fa255228d8243470678b1b4efdc5",  // Doom Xbox
			"997bae5e5a190c5bb3b1fb9e7e3e75b2da88cb27",  // Doom 2024 Re-Re-Release

			// Doom 2
			"a4ce5128d57cb129fdd1441c12b58245be55c8ce",  // Doom2 1.666g
			"6d559b7ceece4f5ad457415049711992370d520a",  // Doom2 1.666
			"70192b8d5aba65c7e633a7c7bcfe7e3e90640c97",  // Doom2 1.7a
			"78009057420b792eacff482021db6fe13b370dcc",  // Doom2 1.7
			"79c283b18e61b9a989cfd3e0f19a42ea98fda551",  // Doom2 1.8
			"d510c877031bbd5f3d198581a2c8651e09b9861f",  // Doom2 1.8f
			"7ec7652fcfce8ddc6e801839291f0e28ef1d5ae7",  // Doom2 1.9
			"a59548125f59f6aa1a41c22f615557d3dd2e85a9",  // Doom2 Bfg
			"f1b6ba94352d53f646b67c01d2da88c5c40e3179",  // Doom2 Psn Eur
			"ca8db908a7c9fbac764f34c148f0bcc78d18553e",  // Doom2 Psn Usa
			"9b39107b5bcfd1f989bcfe46f68dbc1f49222922",  // Doom2 Unity 1.0
			"b723882122e90b61a1d92a11dcfcf9cbf95a407e",  // Doom2 Unity 1.1
			"9574851209c9dfbede56db0dee0660ecd51e6150",  // Doom2 Unity 1.3
			"55e445badd63d8841ebea887910c26c62c7f525e",  // Doom2 Xbla
			"1c91d86cd8a2f3817227986503a6672a5e1613f0",  // Doom2 Xbox
			"b7ba1c68631023ea1aab1d7b9f7f6e9afc508f39",  // Doom2 Xbox360bfg
			"2cda310805397ae44059bbcaed3cd602f4864a82",  // Doom2 Zodiac
			"c745f04a6abc2e6d2a2d52382f45500dd2a260be",  // Doom 2 2024 Re-Re-Release

			// Plutonia
			"90361e2a538d2388506657252ae41aceeb1ba360",  // Plutonia 1.9
			"f131cbe1946d7fddb3caec4aa258c83399c21e60",  // Plutonia Anthology
			"85c3517434135a5886111b324955f9288c01046c",  // Plutonia Psn Eur
			"327f8c41ebd4138354e9fca63cebbbd1b9489749",  // Plutonia Psn Usa
			"54e27b5791fbc5677bf7e83c1de3a92ea3ef935b",  // Plutonia Unity 1.0
			"20fd23ee410c466b263a741bbd53bbef573ab47d",  // Plutonia Unity 1.3
			"816c7c6b0098f66c299c9253f62bd908456efb63",  // Plutonia 2024 Re-Re-Release

			// TNT
			"9fbc66aedef7fe3bae0986cdb9323d2b8db4c9d3",  // Tnt 1.9
			"4a65c8b960225505187c36040b41a40b152f8f3e",  // Tnt Anthology
			"5066833da047117241cdda05a708b009eb266c91",  // Tnt Psn Eur
			"139e26d801a64b404b8d898defca10227a61867b",  // Tnt Psn Usa
			"503271390606ebded04a2cfaa1a4e249c0313a9d",  // Tnt Unity 1.0
			"ca0f0495a6c2813b49620202774c56560d6d7621",  // Tnt Unity 1.3
			"9820e2a3035f0cdd87f69a7d57c59a7a267c9409",  // Tnt 2024 Re-Re-Release

			// Heretic
			"b5a6cc79cde48d97905b44282e82c4c966a23a87",  // Heretic 1.0
			"a54c5d30629976a649119c5ce8babae2ddfb1a60",  // Heretic 1.2
			"f489d479371df32f6d280a0cb23b59a35ba2b833",  // Heretic 1.3

			// Hexen
			"ae797f5fdce845be24a7a24dd5bfc3e762a17bbe",  // Hexen Beta
			"ac129c4331bf26f0f080c4a56aaa40d64969c98a",  // Hexen 1.0
			"4b53832f0733c1e29e5f1de2428e5475e891af29",  // Hexen 1.1
			"4343fbe5aef905ef6d077a1517a50c919e5cc906",  // Hexen Mac

			// Strife
			"eb0f3e157b35c34d5a598701f775e789ec85b4ae",  // Strife1 1.1
			"64c13b951a845ca7f8081f68138a6181557458d1",  // Strife1 1.2
		};                                                      
		
		#endregion

		#region ================== Structs (mxd)

		private struct LumpCopyData
		{
			public byte[] Data;
			public byte[] FixedName;
			public int Index;
		}

		#endregion

		#region ================== Variables

		// File objects
		private string filename;
		private FileStream file;
		private BinaryReader reader;
		private BinaryWriter writer;
		
		// Header
		private int numlumps;
		private int lumpsoffset;
		private bool isiwad; //mxd
		private bool isofficialiwad; //mxd
		
		// Lumps
		private List<Lump> lumps;
		
		// Status
		private bool isreadonly;
		private bool isdisposed;

		#endregion

		#region ================== Properties

		public string Filename { get { return filename; } }
		public Encoding Encoding { get { return ENCODING; } }
		public bool IsReadOnly { get { return isreadonly; } }
		public bool IsDisposed { get { return isdisposed; } }
		public bool IsIWAD { get { return isiwad; } set { isiwad = value; } } //mxd
		public bool IsOfficialIWAD { get { return isofficialiwad; } } //mxd
		public List<Lump> Lumps { get { return lumps; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor to open or create a WAD file
		public WAD(string pathfilename)
		{
			// Initialize
			this.isreadonly = false;
			this.Open(pathfilename);
		}

		// Constructor to open or create a WAD file
		public WAD(string pathfilename, bool openreadonly)
		{
			// Initialize
			this.isreadonly = openreadonly;
			this.Open(pathfilename);
		}

		// Destructor
		/*~WAD()
		{
			// Make sure everything is disposed
			this.Dispose();
		}*/
		
		// Disposer
		public void Dispose()
		{
			// Not already disposed?
			if(!isdisposed)
			{
				// Only possible when not read-only
				if(!isreadonly)
				{
					// Flush writing changes
					if(writer != null) writer.Flush();
					if(file != null) file.Flush();
				}
				
				// Clean up
				if(lumps != null) foreach(Lump l in lumps) l.Dispose();
				if(writer != null) writer.Close();
				if(reader != null) reader.Close();
				if(file != null) file.Dispose();
				
				// Done
				isdisposed = true;
				GC.SuppressFinalize(this); //mxd
			}
		}

		#endregion

		#region ================== IO

		// Open a WAD file
		private void Open(string pathfilename)
		{
			FileAccess access;
			FileShare share;

			// Keep filename
			filename = pathfilename;

			//mxd
			CheckHash();
			
			// Determine if opening for read only
			if(isreadonly)
			{
				// Read only
				access = FileAccess.Read;
				share = FileShare.ReadWrite;
			}
			else
			{
				// Private access
				access = FileAccess.ReadWrite;
				share = FileShare.Read;
			}
			
			// Open the file stream
			file = File.Open(pathfilename, FileMode.OpenOrCreate, access, share);

			// Create file handling tools
			reader = new BinaryReader(file, ENCODING);
			if(!isreadonly) writer = new BinaryWriter(file, ENCODING);

			// Is the WAD file zero length?
			if(file.Length < 4)
			{
				// Create the headers in file
				CreateHeaders();
			}
			else
			{
				// Read information from file
				ReadHeaders();
			}
		}

		// This creates new file headers
		private void CreateHeaders()
		{
			// Default settings
			isiwad = false; //mxd
			isofficialiwad = false; //mxd
			lumpsoffset = 12;

			// New lumps array
			lumps = new List<Lump>(numlumps);
			
			// Write the headers
			if(!isreadonly) WriteHeaders();
		}
		
		// This reads the WAD header and lumps table
		private void ReadHeaders()
		{
			// Make sure the write is finished writing
			if(!isreadonly) writer.Flush();

			// Seek to beginning
			file.Seek(0, SeekOrigin.Begin);

			// Read WAD type
			isiwad = (ENCODING.GetString(reader.ReadBytes(4)) == TYPE_IWAD); //mxd
			
			// Number of lumps
			numlumps = reader.ReadInt32();
			if(numlumps < 0) throw new IOException("Invalid number of lumps in wad file.");

			// Lumps table offset
			lumpsoffset = reader.ReadInt32();
			if(lumpsoffset < 0) throw new IOException("Invalid lumps offset in wad file.");

			// Seek to the lumps table
			file.Seek(lumpsoffset, SeekOrigin.Begin);
			
			// Dispose old lumps and create new list
			if(lumps != null) foreach(Lump l in lumps) l.Dispose();
			lumps = new List<Lump>(numlumps);

			// Go for all lumps
			for(int i = 0; i < numlumps; i++)
			{
				// Read lump information
				int offset = reader.ReadInt32();
				int length = reader.ReadInt32();
				byte[] fixedname = reader.ReadBytes(8);

				// Create the lump
				lumps.Add(new Lump(file, this, fixedname, offset, length));
			}
		}

		// This writes the WAD header and lumps table
		public void WriteHeaders()
		{
            // [ZZ] don't allow any edit actions on readonly archive
            if (isreadonly) return;

            // Seek to beginning
            file.Seek(0, SeekOrigin.Begin);

			// Write WAD type
			writer.Write(ENCODING.GetBytes(isiwad ? TYPE_IWAD : TYPE_PWAD));

			// Number of lumps
			writer.Write(numlumps);

			// Lumps table offset
			writer.Write(lumpsoffset);

			// Seek to the lumps table
			file.Seek(lumpsoffset, SeekOrigin.Begin);

			// Go for all lumps
			for(int i = 0; i < lumps.Count; i++)
			{
				// Write lump information
				writer.Write(lumps[i].Offset);
				writer.Write(lumps[i].Length);
				writer.Write(lumps[i].FixedName);
			}
		}

		//mxd
		private void CheckHash()
		{
			// Open the file stream
			FileStream fs = File.Open(filename, FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite);
			
			// Empty file can't be official iwad
			if(fs.Length > 4)
			{
				BinaryReader r = new BinaryReader(fs, ENCODING);

				// Read WAD type
				if(ENCODING.GetString(r.ReadBytes(4)) == TYPE_IWAD)
				{
					// Rewind
					r.BaseStream.Position = 0;
					//isofficialiwad = IWAD_HASHES.Contains(MD5Hash.Get(r.BaseStream));
					isofficialiwad = IWAD_HASHES.Contains(Hasher<SHA1>.Get(r.BaseStream));
					if (!isreadonly && isofficialiwad) isreadonly = true;
				}

				// Close the reader
				r.Close();
			}
			else
			{
				// Close the file
				fs.Dispose();
			}
		}

		//mxd. This rebuilds the WAD file, removing all "dead" entries
		// Tech info: WAD.Remove() doesn't remove lump data, so MapManager.TemporaryMapFile slowly gets bigger
		// with every map save/test, which leads to lumpsoffset overflowing when TemporaryMapFile size reaches 
		// int.MaxValue bytes in size (that's ~2Gb). 
		internal void Compress()
		{
			// No can't do...
			if(isreadonly) return;
			
			// Gather existing data
			int totaldatalength = 0;
			List<LumpCopyData> copydata = new List<LumpCopyData>(lumps.Count);
			for(int i = 0; i < lumps.Count; i++)
			{
				// Copy lump...
				LumpCopyData lcd = new LumpCopyData();
				Lump l = lumps[i];

				lcd.FixedName = l.FixedName;
				lcd.Index = i;
				lcd.Data = l.Stream.ReadAllBytes();

				// Store data
				copydata.Add(lcd);

				// Track total length
				totaldatalength += l.Length;

				// Dispose lump
				l.Dispose();
			}

			// Compression required?
			if(totaldatalength >= lumpsoffset + 12) return;

			// Set new file length
			file.SetLength(totaldatalength + lumps.Count * 16);

			// Reset lumpsoffset
			lumpsoffset = 12;

			// Reset lumps collection
			lumps = new List<Lump>(copydata.Count);
			numlumps = copydata.Count;

			// Recreate all lumps
			foreach(LumpCopyData lcd in copydata)
			{
				Lump l = new Lump(file, this, lcd.FixedName, lumpsoffset, lcd.Data.Length);
				l.Stream.Write(lcd.Data, 0, lcd.Data.Length);
				l.Stream.Seek(0, SeekOrigin.Begin);
				lumps.Insert(lcd.Index, l);

				lumpsoffset += lcd.Data.Length;
			}

			// Write new headers
			WriteHeaders();
		}
		
		// This flushes writing changes
		/*public void Flush()
		{
			// Only possible when not read-only
			if(!isreadonly)
			{
				// Flush writing changes
				if(writer != null) writer.Flush();
				if(file != null) file.Flush();
			}
		}*/
		
		#endregion
		
		#region ================== Lumps

		// This creates a new lump in the WAD file
        public Lump Insert(string name, int position, int datalength, bool writeheaders = true)
		{
            // [ZZ] don't allow any edit actions on readonly archive
            if (isreadonly) return null;

            // We will be adding a lump
            numlumps++;
			
			// Extend the file
			file.SetLength(file.Length + datalength + 16);
			
			// Create the lump
			Lump lump = new Lump(file, this, Lump.MakeFixedName(name, ENCODING), lumpsoffset, datalength);
			lumps.Insert(position, lump);
			
			// Advance lumps table offset
			lumpsoffset += datalength;

			// Write the new headers
			if(writeheaders) WriteHeaders();

			// Return the new lump
			return lump;
		}

		// This removes a lump from the WAD file by index
		public void RemoveAt(int index, bool writeheaders = true)
		{
            // [ZZ] don't allow any edit actions on readonly archive
            if (isreadonly) return;

            // Remove from list
            Lump l = lumps[index];
			lumps.RemoveAt(index);
			l.Dispose();
			numlumps--;
			
			// Write the new headers
			if (writeheaders) WriteHeaders();
		}
		
		// This removes a lump from the WAD file
		public void Remove(Lump lump)
		{
            // [ZZ] don't allow any edit actions on readonly archive
            if (isreadonly) return;

            // Remove from list
            lumps.Remove(lump);
			lump.Dispose();
			numlumps--;
			
			// Write the new headers
			WriteHeaders();
		}
		
		// This finds a lump by name, returns null when not found
		public Lump FindLump(string name)
		{
			int index = FindLumpIndex(name);
			return (index == -1 ? null : lumps[index]);
		}

		// This finds a lump by name, returns null when not found
		public Lump FindLump(string name, int start)
		{
			int index = FindLumpIndex(name, start);
			return (index == -1 ? null : lumps[index]);
		}

		// This finds a lump by name, returns null when not found
		public Lump FindLump(string name, int start, int end)
		{
			int index = FindLumpIndex(name, start, end);
			return (index == -1 ? null : lumps[index]);
		}

		// This finds a lump by name, returns -1 when not found
		public int FindLumpIndex(string name)
		{
			// Do search
			return FindLumpIndex(name, 0, lumps.Count - 1);
		}

		// This finds a lump by name, returns -1 when not found
		public int FindLumpIndex(string name, int start)
		{
			// Do search
			return FindLumpIndex(name, start, lumps.Count - 1);
		}
		
		// This finds a lump by name, returns -1 when not found
		public int FindLumpIndex(string name, int start, int end)
		{
			if(name.Length > 8 || lumps.Count == 0 || start > lumps.Count - 1) return -1; //mxd. Can't be here. Go away!
			
			long longname = Lump.MakeLongName(name);
			
			// Fix start/end when they exceed safe bounds
			start = Math.Max(start, 0);
			end = General.Clamp(end, 0, lumps.Count - 1);

			// Loop through the lumps
			for(int i = start; i < end + 1; i++)
			{
				// Check if the lump name matches
				if(lumps[i].LongName == longname)
				{
					// Found the lump!
					return i;
				}
			}

			// Nothing found
			return -1;
		}

		//mxd. Same as above, but searches in reversed order

		// This finds a lump by name, returns null when not found
		public Lump FindLastLump(string name)
		{
			int index = FindLastLumpIndex(name);
			return (index == -1 ? null : lumps[index]);
		}

		// This finds a lump by name, returns null when not found
		public Lump FindLastLump(string name, int start)
		{
			int index = FindLastLumpIndex(name, start);
			return (index == -1 ? null : lumps[index]);
		}

		// This finds a lump by name, returns null when not found
		public Lump FindLastLump(string name, int start, int end)
		{
			int index = FindLastLumpIndex(name, start, end);
			return (index == -1 ? null : lumps[index]);
		}

		// This finds a lump by name, returns -1 when not found
		public int FindLastLumpIndex(string name)
		{
			// Do search
			return FindLastLumpIndex(name, 0, lumps.Count - 1);
		}

		// This finds a lump by name, returns -1 when not found
		public int FindLastLumpIndex(string name, int start)
		{
			// Do search
			return FindLastLumpIndex(name, start, lumps.Count - 1);
		}

		// This finds a lump by name, returns -1 when not found
		public int FindLastLumpIndex(string name, int start, int end)
		{
			if(name.Length > 8 || lumps.Count == 0 || start > lumps.Count - 1) return -1; //mxd. Can't be here. Go away!

			long longname = Lump.MakeLongName(name);

			// Fix start/end when they exceed safe bounds
			start = Math.Max(start, 0);
			end = General.Clamp(end, 0, lumps.Count - 1);

			// Loop through the lumps in backwards order
			for(int i = end; i > start - 1; i--)
			{
				// Check if the lump name matches
				if(lumps[i].LongName == longname)
				{
					// Found the lump!
					return i;
				}
			}

			// Nothing found
			return -1;
		}

		#endregion
	}
}
