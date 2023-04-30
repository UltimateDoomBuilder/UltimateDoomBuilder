
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
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;
using System.Linq;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.IO;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Types;
using CodeImp.DoomBuilder.Windows;
using CodeImp.DoomBuilder.VisualModes;
using System.Diagnostics;

#endregion

namespace CodeImp.DoomBuilder.Map
{
	/// <summary>
	/// Manages all geometry structures and things in a map. Also provides
	/// methods to works with selections and marking elements for any purpose.
	/// Note that most methods are of O(n) complexity.
	/// </summary>
	public sealed class MapSet
	{
		#region ================== Constants

		/// <summary>Stiching distance. This is only to get around inaccuracies. Basically,
		/// geometry only stitches when exactly on top of each other.</summary>
		public const float STITCH_DISTANCE = 0.005f; //mxd. 0.001f is not enough when drawing very long lines...
		
		// Virtual sector identification
		// This contains a character that is invalid in the UDMF standard, but valid
		// in our parser, so that it can only be used by Doom Builder and will never
		// conflict with any other valid UDMF field.
		public const string VIRTUAL_SECTOR_FIELD = "!virtual_sector";
		
		//mxd
		private const string SELECTION_GROUPS_PATH = "selectiongroups";
		
		// Handler for tag fields
		public delegate void TagHandler<T>(MapElement element, bool actionargument, UniversalType type, ref int value, T obj);
		
		#endregion

		#region ================== Variables

		// Sector indexing
		private List<int> indexholes;
		private int lastsectorindex;
		
		// Sidedef indexing for (de)serialization
		private Sidedef[] sidedefindices;
		
		// Map structures
		private Vertex[] vertices;
		private Linedef[] linedefs;
		private Sidedef[] sidedefs;
		private Sector[] sectors;
		private Thing[] things;
		private int numvertices;
		private int numlinedefs;
		private int numsidedefs;
		private int numsectors;
		private int numthings;
		
		// Behavior
		private int freezearrays;
		private bool autoremove;
		
		// Selected elements
		private LinkedList<Vertex> sel_vertices;
		private LinkedList<Linedef> sel_linedefs;
		private LinkedList<Sector> sel_sectors;
		private LinkedList<Thing> sel_things;
		private SelectionType sel_type;

		// Unknown UDMF data that needs to be preserved
		private List<UniversalEntry> unknownudmfdata;
		
		// Statics
		private static long emptylongname;
		private static UniValue virtualsectorvalue;

		// Concurrency
		private bool issafetoaccess;
		
		// Disposing
		private bool isdisposed;

		#endregion

		#region ================== Properties
		
		/// <summary>Returns the number of selected sectors.</summary>
		public int SelectedSectorsCount { get { return sel_sectors.Count; } }

		/// <summary>Returns the number of selected linedefs.</summary>
		public int SelectedLinedefsCount { get { return sel_linedefs.Count; } }

		/// <summary>Returns the number of selected vertices.</summary>
		public int SelectedVerticessCount { get { return sel_vertices.Count; } }

		/// <summary>Returns the number of selected things.</summary>
		public int SelectedThingsCount { get { return sel_things.Count; } }
		
		/// <summary>Returns a reference to the list of all vertices.</summary>
		public ICollection<Vertex> Vertices { get { if(freezearrays == 0) return vertices; else return new MapElementCollection<Vertex>(ref vertices, numvertices); } }

		/// <summary>Returns a reference to the list of all linedefs.</summary>
		public ICollection<Linedef> Linedefs { get { if(freezearrays == 0) return linedefs; else return new MapElementCollection<Linedef>(ref linedefs, numlinedefs); } }

		/// <summary>Returns a reference to the list of all sidedefs.</summary>
		public ICollection<Sidedef> Sidedefs { get { if(freezearrays == 0) return sidedefs; else return new MapElementCollection<Sidedef>(ref sidedefs, numsidedefs); } }

		/// <summary>Returns a reference to the list of all sectors.</summary>
		public ICollection<Sector> Sectors { get { if(freezearrays == 0) return sectors; else return new MapElementCollection<Sector>(ref sectors, numsectors); } }

		/// <summary>Returns a reference to the list of all things.</summary>
		public ICollection<Thing> Things { get { if(freezearrays == 0) return things; else return new MapElementCollection<Thing>(ref things, numthings); } }

		/// <summary>Indicates if the map is disposed.</summary>
		public bool IsDisposed { get { return isdisposed; } }
		
		/// <summary>Returns a reference to the list of selected vertices.</summary>
		internal LinkedList<Vertex> SelectedVertices { get { return sel_vertices; } }

		/// <summary>Returns a reference to the list of selected linedefs.</summary>
		internal LinkedList<Linedef> SelectedLinedefs { get { return sel_linedefs; } }

		/// <summary>Returns a reference to the list of selected sectors.</summary>
		internal LinkedList<Sector> SelectedSectors { get { return sel_sectors; } }

		/// <summary>Returns a reference to the list of selected things.</summary>
		internal LinkedList<Thing> SelectedThings { get { return sel_things; } }
		
		/// <summary>Returns the current type of selection.</summary>
		public SelectionType SelectionType { get { return sel_type; } set { sel_type = value; } }

		/// <summary>Long name to indicate "no texture". This is the long name for a single dash.</summary>
		public static long EmptyLongName { get { return emptylongname; } }

		/// <summary>Returns the name of the custom field that marks virtual sectors in pasted geometry.</summary>
		public static string VirtualSectorField { get { return VIRTUAL_SECTOR_FIELD; } }

		/// <summary>Returns the value of the custom field that marks virtual sectors in pasted geometry.</summary>
		public static UniValue VirtualSectorValue { get { return virtualsectorvalue; } }
		
		internal Sidedef[] SidedefIndices { get { return sidedefindices; } }

		internal bool AutoRemove { get { return autoremove; } set { autoremove = value; } }

		internal List<UniversalEntry> UnknownUDMFData { get { return unknownudmfdata; } set { unknownudmfdata = value; } }

		/// <summary>
		/// If it's safe to access (either reading or modifying) the map data. May only be read or set from the UI thread to
		/// avoid racing conditions. Code that wants to access the map data on on a timer or in another thread must honor
		/// this setting to avoid exceptions
		/// </summary>
		public bool IsSafeToAccess { get { return issafetoaccess; } set { issafetoaccess = value; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor for new empty map
		internal MapSet()
		{
			// Initialize
			vertices = new Vertex[0];
			linedefs = new Linedef[0];
			sidedefs = new Sidedef[0];
			sectors = new Sector[0];
			things = new Thing[0];
			sel_vertices = new LinkedList<Vertex>();
			sel_linedefs = new LinkedList<Linedef>();
			sel_sectors = new LinkedList<Sector>();
			sel_things = new LinkedList<Thing>();
			indexholes = new List<int>();
			lastsectorindex = 0;
			autoremove = true;
			unknownudmfdata = new List<UniversalEntry>();

			issafetoaccess = true;
			
			// We have no destructor
			GC.SuppressFinalize(this);
		}

		// Constructor for map to deserialize
		internal MapSet(MemoryStream stream)
		{
			// Initialize
			vertices = new Vertex[0];
			linedefs = new Linedef[0];
			sidedefs = new Sidedef[0];
			sectors = new Sector[0];
			things = new Thing[0];
			sel_vertices = new LinkedList<Vertex>();
			sel_linedefs = new LinkedList<Linedef>();
			sel_sectors = new LinkedList<Sector>();
			sel_things = new LinkedList<Thing>();
			indexholes = new List<int>();
			lastsectorindex = 0;
			autoremove = true;
			unknownudmfdata = new List<UniversalEntry>();

			issafetoaccess = true;

			// Deserialize
			Deserialize(stream);

			// We have no destructor
			GC.SuppressFinalize(this);
		}

		// Disposer
		internal void Dispose()
		{
			// Not already disposed?
			if(!isdisposed)
			{
				// Already set isdisposed so that changes can be prohibited
				isdisposed = true;
				BeginAddRemove();
				
				// Dispose all things
				while((things.Length > 0) && (things[0] != null))
					things[0].Dispose();

				// Dispose all sectors
				while((sectors.Length > 0) && (sectors[0] != null))
					sectors[0].Dispose();

				// Dispose all sidedefs
				while((sidedefs.Length > 0) && (sidedefs[0] != null))
					sidedefs[0].Dispose();

				// Dispose all linedefs
				while((linedefs.Length > 0) && (linedefs[0] != null))
					linedefs[0].Dispose();

				// Dispose all vertices
				while((vertices.Length > 0) && (vertices[0] != null))
					vertices[0].Dispose();

				// Clean up
				vertices = null;
				linedefs = null;
				sidedefs = null;
				sectors = null;
				things = null;
				sel_vertices = null;
				sel_linedefs = null;
				sel_sectors = null;
				sel_things = null;
				indexholes = null;
				
				// Done
				isdisposed = true;
			}
		}

		// Static initializer
		internal static void Initialize()
		{
			emptylongname = Lump.MakeLongName("-", false);
			virtualsectorvalue = new UniValue((int)UniversalType.Integer, 0);
		}

		#endregion

		#region ================== Management
		
		// This begins large add/remove operations
		public void BeginAddRemove()
		{
			freezearrays++;
		}

		// This allocates the arrays to a minimum size so that
		// a lot of items can be created faster. This function will never
		// allocate less than the current number of items.
		public void SetCapacity(int nvertices, int nlinedefs, int nsidedefs, int nsectors, int nthings)
		{
			if(freezearrays == 0)
				throw new Exception("You must call BeginAddRemove before setting the reserved capacity.");

			if(numvertices < nvertices)
				Array.Resize(ref vertices, nvertices);

			if(numlinedefs < nlinedefs)
				Array.Resize(ref linedefs, nlinedefs);

			if(numsidedefs < nsidedefs)
				Array.Resize(ref sidedefs, nsidedefs);

			if(numsectors < nsectors)
				Array.Resize(ref sectors, nsectors);

			if(numthings < nthings)
				Array.Resize(ref things, nthings);
		}
		
		// This ends add/remove operations and crops the arrays
		public void EndAddRemove()
		{
			if(freezearrays > 0)
				freezearrays--;

			if(freezearrays == 0)
			{
				if(numvertices < vertices.Length)
					Array.Resize(ref vertices, numvertices);

				if(numlinedefs < linedefs.Length)
					Array.Resize(ref linedefs, numlinedefs);

				if(numsidedefs < sidedefs.Length)
					Array.Resize(ref sidedefs, numsidedefs);

				if(numsectors < sectors.Length)
					Array.Resize(ref sectors, numsectors);

				if(numthings < things.Length)
					Array.Resize(ref things, numthings);
			}
		}
		
		/// <summary>
		/// This makes a deep copy and returns the new MapSet.
		/// </summary>
		public MapSet Clone()
		{
			// Create the map set
			MapSet newset = new MapSet();
			newset.BeginAddRemove();
			newset.SetCapacity(numvertices, numlinedefs, numsidedefs, numsectors, numthings);
			
			// Go for all vertices
			foreach(Vertex v in vertices)
			{
				// Make new vertex
				v.Clone = newset.CreateVertex(v.Position);
				v.CopyPropertiesTo(v.Clone);
			}

			// Go for all sectors
			foreach(Sector s in sectors)
			{
				// Make new sector
				s.Clone = newset.CreateSector();
				s.CopyPropertiesTo(s.Clone);
			}

			// Go for all linedefs
			foreach(Linedef l in linedefs)
			{
				// Make new linedef
				Linedef nl = newset.CreateLinedef(l.Start.Clone, l.End.Clone);
				l.CopyPropertiesTo(nl);

				// Linedef has a front side?
				if(l.Front != null)
				{
					// Make new sidedef
					Sidedef nd = newset.CreateSidedef(nl, true, l.Front.Sector.Clone);
					l.Front.CopyPropertiesTo(nd);
				}

				// Linedef has a back side?
				if(l.Back != null)
				{
					// Make new sidedef
					Sidedef nd = newset.CreateSidedef(nl, false, l.Back.Sector.Clone);
					l.Back.CopyPropertiesTo(nd);
				}
			}
			
			// Go for all things
			foreach(Thing t in things)
			{
				// Make new thing
				Thing nt = newset.CreateThing();
				t.CopyPropertiesTo(nt);
			}
			
			// Remove clone references
			foreach(Vertex v in vertices) v.Clone = null;
			foreach(Sector s in sectors) s.Clone = null;

			// Copy unknown UDMF data
			newset.UnknownUDMFData = new List<UniversalEntry>();
			foreach(UniversalEntry e in unknownudmfdata)
			{
				if(e.Value is UniversalCollection)
				{
					// The UniversalEntry value is a collection, so we have to copy all sub-elements
					UniversalCollection uc = new UniversalCollection();

					foreach(UniversalEntry ie in (UniversalCollection)e.Value)
						uc.Add(ie.Key, ie.Value);

					newset.UnknownUDMFData.Add(new UniversalEntry(e.Key, uc));
				}
				else
				{
					// Just a normal UniversalEntry
					newset.UnknownUDMFData.Add(new UniversalEntry(e.Key, e.Value));
				}
			}
			
			// Return the new set
			newset.EndAddRemove();
			return newset;
		}

		// This makes a deep copy of the marked geometry and binds missing sectors to a virtual sector
		internal MapSet CloneMarked()
		{
			Sector virtualsector = null;
			
			// Create the map set
			MapSet newset = new MapSet();
			newset.BeginAddRemove();

			// Get marked geometry
			ICollection<Vertex> mvertices = GetMarkedVertices(true);
			ICollection<Linedef> mlinedefs = GetMarkedLinedefs(true);
			ICollection<Sector> msectors = GetMarkedSectors(true);
			ICollection<Thing> mthings = GetMarkedThings(true);
			newset.SetCapacity(mvertices.Count, mlinedefs.Count, numsidedefs, msectors.Count, mthings.Count);
			
			// Go for all vertices
			foreach(Vertex v in mvertices)
			{
				// Make new vertex
				v.Clone = newset.CreateVertex(v.Position);
				v.CopyPropertiesTo(v.Clone);
			}

			// Go for all sectors
			foreach(Sector s in msectors)
			{
				// Make new sector
				s.Clone = newset.CreateSector();
				s.CopyPropertiesTo(s.Clone);
			}

			// Go for all linedefs
			foreach(Linedef l in mlinedefs)
			{
				// Make new linedef
				Linedef nl = newset.CreateLinedef(l.Start.Clone, l.End.Clone);
				l.CopyPropertiesTo(nl);

				// Linedef has a front side?
				if(l.Front != null)
				{
					Sidedef nd;
					
					// Sector on front side marked?
					if(l.Front.Sector.Marked)
					{
						// Make new sidedef
						nd = newset.CreateSidedef(nl, true, l.Front.Sector.Clone);
					}
					else
					{
						// Make virtual sector if needed
						if(virtualsector == null)
						{
							virtualsector = newset.CreateSector();
							l.Front.Sector.CopyPropertiesTo(virtualsector);
							virtualsector.Fields.BeforeFieldsChange();
							virtualsector.Fields[VIRTUAL_SECTOR_FIELD] = new UniValue(virtualsectorvalue);
						}
						
						// Make new sidedef that links to the virtual sector
						nd = newset.CreateSidedef(nl, true, virtualsector);
					}
					
					l.Front.CopyPropertiesTo(nd);
				}

				// Linedef has a back side?
				if(l.Back != null)
				{
					Sidedef nd;

					// Sector on front side marked?
					if(l.Back.Sector.Marked)
					{
						// Make new sidedef
						nd = newset.CreateSidedef(nl, false, l.Back.Sector.Clone);
					}
					else
					{
						// Make virtual sector if needed
						if(virtualsector == null)
						{
							virtualsector = newset.CreateSector();
							l.Back.Sector.CopyPropertiesTo(virtualsector);
							virtualsector.Fields.BeforeFieldsChange();
							virtualsector.Fields[VIRTUAL_SECTOR_FIELD] = new UniValue(virtualsectorvalue);
						}

						// Make new sidedef that links to the virtual sector
						nd = newset.CreateSidedef(nl, false, virtualsector);
					}
					
					l.Back.CopyPropertiesTo(nd);
				}
			}

			// Go for all things
			foreach(Thing t in mthings)
			{
				// Make new thing
				Thing nt = newset.CreateThing();
				t.CopyPropertiesTo(nt);
			}

			// Remove clone references
			foreach(Vertex v in vertices) v.Clone = null;
			foreach(Sector s in sectors) s.Clone = null;

			// Return the new set
			newset.EndAddRemove();
			return newset;
		}
		
		/// <summary>This creates a new vertex and returns it.</summary>
		public Vertex CreateVertex(Vector2D pos)
		{
			if(numvertices == General.Map.FormatInterface.MaxVertices)
			{
				General.Interface.DisplayStatus(StatusType.Warning, "Failed to complete operation: maximum number of vertices reached.");
				return null;
			}
			
			// Make the vertex
			Vertex v = new Vertex(this, numvertices, pos);
			AddItem(v, ref vertices, numvertices, ref numvertices);
			return v;
		}

		/// <summary>This creates a new vertex and returns it.</summary>
		public Vertex CreateVertex(int index, Vector2D pos)
		{
			if(numvertices == General.Map.FormatInterface.MaxVertices)
			{
				General.Interface.DisplayStatus(StatusType.Warning, "Failed to complete operation: maximum number of vertices reached.");
				return null;
			}

			// Make the vertex
			Vertex v = new Vertex(this, index, pos);
			AddItem(v, ref vertices, index, ref numvertices);
			return v;
		}

		/// <summary>This creates a new linedef and returns it.</summary>
		public Linedef CreateLinedef(Vertex start, Vertex end)
		{
			if(numlinedefs == General.Map.FormatInterface.MaxLinedefs)
			{
				General.Interface.DisplayStatus(StatusType.Warning, "Failed to complete operation: maximum number of linedefs reached.");
				return null;
			}

			// Make the linedef
			Linedef l = new Linedef(this, numlinedefs, start, end);
			AddItem(l, ref linedefs, numlinedefs, ref numlinedefs);
			return l;
		}

		/// <summary>This creates a new linedef and returns it.</summary>
		public Linedef CreateLinedef(int index, Vertex start, Vertex end)
		{
			if(numlinedefs == General.Map.FormatInterface.MaxLinedefs)
			{
				General.Interface.DisplayStatus(StatusType.Warning, "Failed to complete operation: maximum number of linedefs reached.");
				return null;
			}

			// Make the linedef
			Linedef l = new Linedef(this, index, start, end);
			AddItem(l, ref linedefs, index, ref numlinedefs);
			return l;
		}

		/// <summary>This creates a new sidedef and returns it.</summary>
		public Sidedef CreateSidedef(Linedef l, bool front, Sector s)
		{
			if(numsidedefs == int.MaxValue)
			{
				General.Interface.DisplayStatus(StatusType.Warning, "Failed to complete operation: maximum number of sidedefs reached.");
				return null;
			}
			
			// Make the sidedef
			Sidedef sd = new Sidedef(this, numsidedefs, l, front, s);
			AddItem(sd, ref sidedefs, numsidedefs, ref numsidedefs);
			return sd;
		}

		/// <summary>This creates a new sidedef and returns it.</summary>
		public Sidedef CreateSidedef(int index, Linedef l, bool front, Sector s)
		{
			if(numsidedefs == int.MaxValue)
			{
				General.Interface.DisplayStatus(StatusType.Warning, "Failed to complete operation: maximum number of sidedefs reached.");
				return null;
			}

			// Make the sidedef
			Sidedef sd = new Sidedef(this, index, l, front, s);
			AddItem(sd, ref sidedefs, index, ref numsidedefs);
			return sd;
		}

		/// <summary>This creates a new sector and returns it.</summary>
		public Sector CreateSector()
		{
			// Make the sector
			return CreateSector(numsectors);
		}

		/// <summary>This creates a new sector and returns it.</summary>
		public Sector CreateSector(int index)
		{
			int fixedindex;

			if(numsectors == General.Map.FormatInterface.MaxSectors)
			{
				General.Interface.DisplayStatus(StatusType.Warning, "Failed to complete operation: maximum number of sectors reached.");
				return null;
			}

			// Do we have any index holes we can use?
			if(indexholes.Count > 0)
			{
				// Take one of the index holes
				fixedindex = indexholes[indexholes.Count - 1];
				indexholes.RemoveAt(indexholes.Count - 1);
			}
			else
			{
				// Make a new index
				fixedindex = lastsectorindex++;
			}
			
			// Make the sector
			return CreateSectorEx(fixedindex, index);
		}

		// This creates a new sector with a specific fixed index
		private Sector CreateSectorEx(int fixedindex, int index)
		{
			if(numsectors == General.Map.FormatInterface.MaxSectors)
			{
				General.Interface.DisplayStatus(StatusType.Warning, "Failed to complete operation: maximum number of sectors reached.");
				return null;
			}

			// Make the sector
			Sector s = new Sector(this, index, fixedindex);
			AddItem(s, ref sectors, index, ref numsectors);
			return s;
		}

		/// <summary>This creates a new thing and returns it.</summary>
		public Thing CreateThing()
		{
			if(numthings == General.Map.FormatInterface.MaxThings)
			{
				General.Interface.DisplayStatus(StatusType.Warning, "Failed to complete operation: maximum number of things reached.");
				return null;
			}

			// Make the thing
			Thing t = new Thing(this, numthings);
			AddItem(t, ref things, numthings, ref numthings);
			return t;
		}

		/// <summary>This creates a new thing and returns it.</summary>
		public Thing CreateThing(int index)
		{
			if(numthings == General.Map.FormatInterface.MaxThings)
			{
				General.Interface.DisplayStatus(StatusType.Warning, "Failed to complete operation: maximum number of things reached.");
				return null;
			}

			// Make the thing
			Thing t = new Thing(this, index);
			AddItem(t, ref things, index, ref numthings);
			return t;
		}

		/// <summary>
		/// This creates a temporary thing. The difference to normal thing creation is that the creation will not be recorded by the undo manager.
		/// This should only be used in very specific cases, like creating a temporary player start for the "test from current position" action
		/// </summary>
		/// <returns>The new thing</returns>
		internal Thing CreateTempThing()
		{
			if (numthings == General.Map.FormatInterface.MaxThings)
			{
				General.Interface.DisplayStatus(StatusType.Warning, "Failed to complete operation: maximum number of things reached.");
				return null;
			}

			// Make the thing
			Thing t = new Thing(this, numthings, false);
			AddItem(t, ref things, numthings, ref numthings);
			return t;

		}

		// This increases the size of the array to add an item
		private void AddItem<T>(T item, ref T[] array, int index, ref int counter) where T: MapElement
		{
			// Only resize when there are no more free entries
			if(counter == array.Length)
			{
				if(freezearrays == 0)
					Array.Resize(ref array, counter + 1);
				else
					Array.Resize(ref array, counter + 10);
			}
			
			// Move item at the given index if the new item is not added at the end
			if(index != counter)
			{
				array[counter] = array[index];
				array[counter].Index = counter;
			}

			// Add item
			array[index] = item;
			counter++;
		}
		
		// This adds a sector index hole
		internal void AddSectorIndexHole(int index)
		{
			indexholes.Add(index);
		}
		
		private void RemoveItem<T>(ref T[] array, int index, ref int counter) where T: MapElement
		{
			if(index == (counter - 1))
			{
				array[index] = null;
			}
			else
			{
				array[index] = array[counter - 1];
				array[index].Index = index;
				array[counter - 1] = null;
			}
			
			counter--;

			if(freezearrays == 0)
				Array.Resize(ref array, counter);
		}
		
		internal void RemoveVertex(int index)
		{
			RemoveItem(ref vertices, index, ref numvertices);
		}

		internal void RemoveLinedef(int index)
		{
			RemoveItem(ref linedefs, index, ref numlinedefs);
		}

		internal void RemoveSidedef(int index)
		{
			RemoveItem(ref sidedefs, index, ref numsidedefs);
		}

		internal void RemoveSector(int index)
		{
			RemoveItem(ref sectors, index, ref numsectors);
		}

		internal void RemoveThing(int index)
		{
			RemoveItem(ref things, index, ref numthings);
		}		
		
		#endregion

		#region ================== Serialization

		// This serializes the MapSet
		internal MemoryStream Serialize()
		{
			MemoryStream stream = new MemoryStream(20000000);	// Yes that is about 20 MB.
			SerializerStream serializer = new SerializerStream(stream);
			
			// Index the sidedefs
			int sidedefindex = 0;
			foreach(Sidedef sd in sidedefs)
				sd.SerializedIndex = sidedefindex++;

			serializer.Begin();

			// Write private data
			serializer.wInt(lastsectorindex);
			serializer.wInt(indexholes.Count);
			foreach(int i in indexholes) serializer.wInt(i);

			// Write map data
			WriteVertices(serializer);
			WriteSectors(serializer);
			WriteLinedefs(serializer);
			WriteSidedefs(serializer);
			WriteThings(serializer);

			serializer.End();

			// Reallocate to keep only the used memory
			stream.Capacity = (int)stream.Length;
			
			return stream;
		}

		// This serializes things
		private void WriteThings(SerializerStream stream)
		{
			stream.wInt(numthings);
			
			// Go for all things
			foreach(Thing t in things)
			{
				t.ReadWrite(stream);
			}
		}

		// This serializes vertices
		private void WriteVertices(SerializerStream stream)
		{
			stream.wInt(numvertices);

			// Go for all vertices
			int index = 0;
			foreach(Vertex v in vertices)
			{
				v.SerializedIndex = index++;
				
				v.ReadWrite(stream);
			}
		}

		// This serializes linedefs
		private void WriteLinedefs(SerializerStream stream)
		{
			stream.wInt(numlinedefs);

			// Go for all lines
			int index = 0;
			foreach(Linedef l in linedefs)
			{
				l.SerializedIndex = index++;
				
				stream.wInt(l.Start.SerializedIndex);
				
				stream.wInt(l.End.SerializedIndex);

				l.ReadWrite(stream);
			}
		}

		// This serializes sidedefs
		private void WriteSidedefs(SerializerStream stream)
		{
			stream.wInt(numsidedefs);

			// Go for all sidedefs
			foreach(Sidedef sd in sidedefs)
			{
				stream.wInt(sd.Line.SerializedIndex);
				
				stream.wInt(sd.Sector.SerializedIndex);

				stream.wBool(sd.IsFront);

				sd.ReadWrite(stream);
			}
		}

		// This serializes sectors
		private void WriteSectors(SerializerStream stream)
		{
			stream.wInt(numsectors);

			// Go for all sectors
			int index = 0;
			foreach(Sector s in sectors)
			{
				s.SerializedIndex = index++;

				s.ReadWrite(stream);
				s.Triangles.ReadWrite(stream); //mxd
			}
		}

		#endregion

		#region ================== Deserialization

		// This deserializes the MapSet
		internal void Deserialize(MemoryStream stream)
		{
			stream.Seek(0, SeekOrigin.Begin);
			DeserializerStream deserializer = new DeserializerStream(stream);

			deserializer.Begin();

			// Read private data
			int c;
			deserializer.rInt(out lastsectorindex);
			deserializer.rInt(out c);
			indexholes = new List<int>(c);
			for(int i = 0; i < c; i++)
			{
				int index; deserializer.rInt(out index);
				indexholes.Add(index);
			}

			// Read map data
			Vertex[] verticesarray = ReadVertices(deserializer);
			Sector[] sectorsarray = ReadSectors(deserializer);
			Linedef[] linedefsarray = ReadLinedefs(deserializer, verticesarray);
			ReadSidedefs(deserializer, linedefsarray, sectorsarray);
			ReadThings(deserializer);

			deserializer.End();
			deserializer.Dispose(); //mxd

			// Make table of sidedef indices
			sidedefindices = new Sidedef[numsidedefs];
			foreach(Sidedef sd in sidedefs)
				sidedefindices[sd.SerializedIndex] = sd;
				
			// Call PostDeserialize
			foreach(Sector s in sectors)
				s.PostDeserialize(this);
		}
		
		// This deserializes things
		private void ReadThings(DeserializerStream stream)
		{
			int c; stream.rInt(out c);

			// Go for all things
			for(int i = 0; i < c; i++)
			{
				Thing t = CreateThing();
				t.ReadWrite(stream);
			}
		}

		// This deserializes vertices
		private Vertex[] ReadVertices(DeserializerStream stream)
		{
			int c; stream.rInt(out c);

			Vertex[] array = new Vertex[c];

			// Go for all vertices
			for(int i = 0; i < c; i++)
			{
				array[i] = CreateVertex(new Vector2D());
				array[i].ReadWrite(stream);
			}

			return array;
		}

		// This deserializes linedefs
		private Linedef[] ReadLinedefs(DeserializerStream stream, Vertex[] verticesarray)
		{
			int c; stream.rInt(out c);

			Linedef[] array = new Linedef[c];

			// Go for all lines
			for(int i = 0; i < c; i++)
			{
				int start, end;
				
				stream.rInt(out start);

				stream.rInt(out end);

				array[i] = CreateLinedef(verticesarray[start], verticesarray[end]);
				array[i].ReadWrite(stream);
			}

			return array;
		}

		// This deserializes sidedefs
		private void ReadSidedefs(DeserializerStream stream, Linedef[] linedefsarray, Sector[] sectorsarray)
		{
			int c; stream.rInt(out c);

			// Go for all sidedefs
			for(int i = 0; i < c; i++)
			{
				int lineindex, sectorindex;
				bool front;
				
				stream.rInt(out lineindex);

				stream.rInt(out sectorindex);

				stream.rBool(out front);

				Sidedef sd = CreateSidedef(linedefsarray[lineindex], front, sectorsarray[sectorindex]);
				sd.ReadWrite(stream);
			}
		}

		// This deserializes sectors
		private Sector[] ReadSectors(DeserializerStream stream)
		{
			int c; stream.rInt(out c);

			Sector[] array = new Sector[c];

			// Go for all sectors
			for(int i = 0; i < c; i++)
			{
				array[i] = CreateSector();
				array[i].ReadWrite(stream);
				array[i].Triangles.ReadWrite(stream); //mxd
			}

			return array;
		}

		#endregion

		#region ================== Updating

		/// <summary>
		/// This updates the cache of all elements where needed. You must call this after making changes to the map.
		/// </summary>
		public void Update()
		{
			// Update all!
			Update(true, true);
		}

		/// <summary>
		/// This updates the cache of all elements where needed. It is not recommended to use this version, please use Update() instead.
		/// </summary>
		public void Update(bool dolines, bool dosectors)
		{
			// Update all linedefs
			if(dolines) foreach(Linedef l in linedefs) l.UpdateCache();
			
			// Update all sectors
			if(dosectors)
			{
				foreach (Sector s in sectors)
				{
					s.Triangulate();
					s.UpdateBBox();
				}

				General.Map.CRenderer2D.Surfaces.AllocateBuffers();
				foreach (Sector s in sectors) s.CreateSurfaces();
				General.Map.CRenderer2D.Surfaces.UnlockBuffers();
			}
		}
		
		/// <summary>
		/// This updates the cache of all elements that is required after a configuration or settings change.
		/// </summary>
		public void UpdateConfiguration()
		{
			// Update all things
			foreach(Thing t in things) t.UpdateConfiguration();
		}
		
		#endregion

		#region ================== Selection
		
		// This checks a flag in a selection type
		private static bool InSelectionType(SelectionType value, SelectionType bits)
		{
			return (value & bits) == bits;
		}

		/// <summary>This converts the current selection to a different type of selection as specified.
		/// Note that this function uses the markings to convert the selection.</summary>
		public void ConvertSelection(SelectionType target)
		{
			ConvertSelection(SelectionType.All, target);
		}

		/// <summary>This converts the current selection to a different type of selection as specified.
		/// Note that this function uses the markings to convert the selection.</summary>
		public void ConvertSelection(SelectionType source, SelectionType target)
		{
			ClearAllMarks(false);
			
			switch(target)
			{
				// Convert geometry selection to vertices only
				case SelectionType.Vertices:
					if(InSelectionType(source, SelectionType.Linedefs)) MarkSelectedLinedefs(true, true);
					if(InSelectionType(source, SelectionType.Sectors)) General.Map.Map.MarkSelectedSectors(true, true);
					ICollection<Vertex> verts = General.Map.Map.GetVerticesFromLinesMarks(true);
					foreach(Vertex v in verts) v.Selected = true;
					verts = General.Map.Map.GetVerticesFromSectorsMarks(true);
					foreach(Vertex v in verts) v.Selected = true;
					General.Map.Map.ClearSelectedSectors();
					General.Map.Map.ClearSelectedLinedefs();
					break;
					
				// Convert geometry selection to linedefs only
				case SelectionType.Linedefs:
					if(InSelectionType(source, SelectionType.Vertices)) MarkSelectedVertices(true, true);
					if(!InSelectionType(source, SelectionType.Linedefs)) ClearSelectedLinedefs();
					ICollection<Linedef> lines = General.Map.Map.LinedefsFromMarkedVertices(false, true, false);
					foreach(Linedef l in lines) l.Selected = true;
					if(InSelectionType(source, SelectionType.Sectors))
					{
						foreach(Sector s in General.Map.Map.Sectors)
						{
							if(s.Selected)
							{
								foreach(Sidedef sd in s.Sidedefs)
									sd.Line.Selected = true;
							}
						}
					}
					General.Map.Map.ClearSelectedSectors();
					General.Map.Map.ClearSelectedVertices();
					break;
					
				// Convert geometry selection to sectors only
				case SelectionType.Sectors:
					if(InSelectionType(source, SelectionType.Vertices)) MarkSelectedVertices(true, true);
					if(!InSelectionType(source, SelectionType.Linedefs)) ClearSelectedLinedefs();
					lines = LinedefsFromMarkedVertices(false, true, false);
					foreach(Linedef l in lines) l.Selected = true;
					ClearMarkedSectors(true);
					foreach(Linedef l in linedefs)
					{
						if(!l.Selected)
						{
							if(l.Front != null) l.Front.Sector.Marked = false;
							if(l.Back != null) l.Back.Sector.Marked = false;
						}
					}
					ClearSelectedLinedefs();
					ClearSelectedVertices();
					if(InSelectionType(source, SelectionType.Sectors))
					{
						foreach(Sector s in General.Map.Map.Sectors)
						{
							if(s.Selected || (s.Marked && s.Sidedefs.Count > 0))
							{
								s.Selected = true;
								foreach(Sidedef sd in s.Sidedefs)
									sd.Line.Selected = true;
							}
						}
					}
					else
					{
						foreach(Sector s in General.Map.Map.Sectors)
						{
							if(s.Marked && s.Sidedefs.Count > 0)
							{
								s.Selected = true;
								foreach(Sidedef sd in s.Sidedefs)
									sd.Line.Selected = true;
							}
							else
							{
								s.Selected = false;
							}
						}
					}
					break;
					
				default:
					throw new ArgumentException("Unsupported selection target conversion");
			}
			
			// New selection type
			sel_type = target;
		}

		/// <summary>This clears all selected items</summary>
		public void ClearAllSelected()
		{
			ClearSelectedVertices();
			ClearSelectedThings();
			ClearSelectedLinedefs();
			ClearSelectedSectors();
		}

		/// <summary>This clears selected vertices.</summary>
		public void ClearSelectedVertices()
		{
			sel_vertices.Clear();
			foreach(Vertex v in vertices) v.Selected = false;
		}

		/// <summary>This clears selected things.</summary>
		public void ClearSelectedThings()
		{
			sel_things.Clear();
			foreach(Thing t in things) t.Selected = false;
		}

		/// <summary>This clears selected linedefs.</summary>
		public void ClearSelectedLinedefs()
		{
			sel_linedefs.Clear();
			foreach(Linedef l in linedefs) l.Selected = false;
		}

		/// <summary>This clears selected sectors.</summary>
		public void ClearSelectedSectors()
		{
			sel_sectors.Clear();
			foreach(Sector s in sectors) s.Selected = false;
		}

		/// <summary>Returns a collection of vertices that match a selected state.</summary>
		public ICollection<Vertex> GetSelectedVertices(bool selected)
		{
			if(selected)
			{
				return new List<Vertex>(sel_vertices);
			}
			else
			{
				List<Vertex> list = new List<Vertex>(numvertices - sel_vertices.Count);
				foreach(Vertex v in vertices) if(!v.Selected) list.Add(v);
				return list;
			}
		}

		/// <summary>Returns a collection of things that match a selected state.</summary>
		public ICollection<Thing> GetSelectedThings(bool selected)
		{
			if(selected)
			{
				return new List<Thing>(sel_things);
			}
			else
			{
				List<Thing> list = new List<Thing>(numthings - sel_things.Count);
				foreach(Thing t in things) if(!t.Selected) list.Add(t);
				return list;
			}
		}

		/// <summary>Returns a collection of linedefs that match a selected state.</summary>
		public ICollection<Linedef> GetSelectedLinedefs(bool selected)
		{
			if(selected)
			{
				return new List<Linedef>(sel_linedefs);
			}
			else
			{
				List<Linedef> list = new List<Linedef>(numlinedefs - sel_linedefs.Count);
				foreach(Linedef l in linedefs) if(!l.Selected) list.Add(l);
				return list;
			}
		}

		/// <summary>Returns a collection of sidedefs that match a selected linedefs state.</summary>
		public ICollection<Sidedef> GetSidedefsFromSelectedLinedefs(bool selected)
		{
			if(selected)
			{
				List<Sidedef> list = new List<Sidedef>(sel_linedefs.Count);
				foreach(Linedef ld in sel_linedefs)
				{
					if(ld.Front != null) list.Add(ld.Front);
					if(ld.Back != null) list.Add(ld.Back);
				}
				return list;
			}
			else
			{
				List<Sidedef> list = new List<Sidedef>(numlinedefs - sel_linedefs.Count);
				foreach(Linedef ld in linedefs)
				{
					if(!ld.Selected && (ld.Front != null)) list.Add(ld.Front);
					if(!ld.Selected && (ld.Back != null)) list.Add(ld.Back);
				}
				return list;
			}
		}

		/// <summary>Returns a collection of sectors that match a selected state.</summary>
		public ICollection<Sector> GetSelectedSectors(bool selected)
		{
			if(selected)
			{
				return new List<Sector>(sel_sectors);
			}
			else
			{
				List<Sector> list = new List<Sector>(numsectors - sel_sectors.Count);
				foreach(Sector s in sectors) if(!s.Selected) list.Add(s);
				return list;
			}
		}

		/// <summary>This selects or deselectes geometry based on marked elements.</summary>
		public void SelectMarkedGeometry(bool mark, bool select)
		{
			SelectMarkedVertices(mark, select);
			SelectMarkedLinedefs(mark, select);
			SelectMarkedSectors(mark, select);
			SelectMarkedThings(mark, select);
		}

		/// <summary>This selects or deselectes geometry based on marked elements.</summary>
		public void SelectMarkedVertices(bool mark, bool select)
		{
			foreach(Vertex v in vertices) if(v.Marked == mark) v.Selected = select;
		}

		/// <summary>This selects or deselectes geometry based on marked elements.</summary>
		public void SelectMarkedLinedefs(bool mark, bool select)
		{
			foreach(Linedef l in linedefs) if(l.Marked == mark) l.Selected = select;
		}

		/// <summary>This selects or deselectes geometry based on marked elements.</summary>
		public void SelectMarkedSectors(bool mark, bool select)
		{
			foreach(Sector s in sectors) if(s.Marked == mark) s.Selected = select;
		}

		/// <summary>This selects or deselectes geometry based on marked elements.</summary>
		public void SelectMarkedThings(bool mark, bool select)
		{
			foreach(Thing t in things) if(t.Marked == mark) t.Selected = select;
		}

		#endregion

		#region ================== Selection groups

		/// <summary>This selects geometry by selection group index.</summary>
		public void SelectVerticesByGroup(int groupmask)
		{
			foreach(Vertex e in vertices) e.SelectByGroup(groupmask);
		}

		/// <summary>This selects geometry by selection group index.</summary>
		public void SelectLinedefsByGroup(int groupmask)
		{
			foreach(Linedef e in linedefs) e.SelectByGroup(groupmask);
		}

		/// <summary>This selects geometry by selection group index.</summary>
		public void SelectSectorsByGroup(int groupmask)
		{
			foreach(Sector e in sectors) e.SelectByGroup(groupmask);
		}

		/// <summary>This selects geometry by selection group index.</summary>
		public void SelectThingsByGroup(int groupmask)
		{
			foreach(Thing e in things) e.SelectByGroup(groupmask);
		}

		/// <summary>This adds the current selection to the specified selection group.</summary>
		//mxd. switched groupmask to groupindex
		public void AddSelectionToGroup(int groupindex)
		{
			int groupmask = 0x01 << groupindex;
			foreach(Vertex e in vertices) if(e.Selected) e.AddToGroup(groupmask);
			foreach(Linedef e in linedefs) if(e.Selected) e.AddToGroup(groupmask);
			foreach(Sector e in sectors) if(e.Selected) e.AddToGroup(groupmask);
			foreach(Thing e in things) if(e.Selected) e.AddToGroup(groupmask);
		}

		/// <summary>This clears specified selection group.</summary>
		//mxd
		public void ClearGroup(int groupmask) 
		{
			foreach(Vertex e in vertices)  e.RemoveFromGroup(groupmask);
			foreach(Linedef e in linedefs) e.RemoveFromGroup(groupmask);
			foreach(Sector e in sectors)   e.RemoveFromGroup(groupmask);
			foreach(Thing e in things)     e.RemoveFromGroup(groupmask);
		}

		//mxd
		internal GroupInfo GetGroupInfo(int groupindex) 
		{
			int numSectors = 0;
			int numLines = 0;
			int numVerts = 0;
			int numThings = 0;
			int groupmask = 0x01 << groupindex;

			foreach(Vertex e in vertices)  if(e.IsInGroup(groupmask)) numVerts++; //mxd
			foreach(Linedef e in linedefs) if(e.IsInGroup(groupmask)) numLines++; //mxd
			foreach(Sector e in sectors)   if(e.IsInGroup(groupmask)) numSectors++; //mxd
			foreach(Thing e in things)     if(e.IsInGroup(groupmask)) numThings++; //mxd

			return new GroupInfo(groupindex + 1, numSectors, numLines, numVerts, numThings);
		}

		//mxd
		internal void WriteSelectionGroups(Configuration cfg) 
		{
			// Fill structure
			IDictionary groups = new ListDictionary();
			for(int i = 0; i < 10; i++) 
			{
				IDictionary group = new ListDictionary();
				int groupmask = 0x01 << i;

				//store verts
				List<string> indices = new List<string>();
				foreach(Vertex e in vertices) if(e.IsInGroup(groupmask)) indices.Add(e.Index.ToString());
				if(indices.Count > 0) group.Add("vertices", string.Join(" ", indices.ToArray()));

				//store linedefs
				indices.Clear();
				foreach(Linedef e in linedefs) if(e.IsInGroup(groupmask)) indices.Add(e.Index.ToString());
				if(indices.Count > 0) group.Add("linedefs", string.Join(" ", indices.ToArray()));

				//store sectors
				indices.Clear();
				foreach(Sector e in sectors) if(e.IsInGroup(groupmask)) indices.Add(e.Index.ToString());
				if(indices.Count > 0) group.Add("sectors", string.Join(" ", indices.ToArray()));

				//store things
				indices.Clear();
				foreach(Thing e in things) if(e.IsInGroup(groupmask)) indices.Add(e.Index.ToString());
				if(indices.Count > 0) group.Add("things", string.Join(" ", indices.ToArray()));

				//add to main collection
				if(group.Count > 0) groups.Add(i, group);
			}

			// Write to config
			if(groups.Count > 0) cfg.WriteSetting(SELECTION_GROUPS_PATH, groups);
		}

		//mxd
		internal void ReadSelectionGroups(Configuration cfg) 
		{
			IDictionary grouplist = cfg.ReadSetting(SELECTION_GROUPS_PATH, new Hashtable());

			foreach(DictionaryEntry mp in grouplist) 
			{
				// Item is a structure?
				if(mp.Value is IDictionary) 
				{
					//get group number
					int groupnum;
					if(!int.TryParse(mp.Key as string, out groupnum)) continue;

					int groupmask = 0x01 << General.Clamp(groupnum, 0, 10);
					IDictionary groupinfo = (IDictionary)mp.Value;

					if(groupinfo.Contains("vertices")) 
					{
						string s = groupinfo["vertices"] as string;
						if(!string.IsNullOrEmpty(s)) 
						{
							List<int> indices = GetIndices(s);
							foreach(int index in indices) 
							{
								if(index > vertices.Length) continue;
								vertices[index].AddToGroup(groupmask);
							}
						}
					}

					if(groupinfo.Contains("linedefs")) 
					{
						string s = groupinfo["linedefs"] as string;
						if(!string.IsNullOrEmpty(s)) 
						{
							List<int> indices = GetIndices(s);
							foreach(int index in indices) 
							{
								if(index > linedefs.Length) continue;
								linedefs[index].AddToGroup(groupmask);
							}
						}
					}

					if(groupinfo.Contains("sectors")) 
					{
						string s = groupinfo["sectors"] as string;
						if(!string.IsNullOrEmpty(s)) 
						{
							List<int> indices = GetIndices(s);
							foreach(int index in indices) 
							{
								if(index > sectors.Length) continue;
								sectors[index].AddToGroup(groupmask);
							}
						}
					}

					if(groupinfo.Contains("things")) 
					{
						string s = groupinfo["things"] as string;
						if(!string.IsNullOrEmpty(s)) 
						{
							List<int> indices = GetIndices(s);
							foreach(int index in indices) 
							{
								if(index > things.Length) continue;
								things[index].AddToGroup(groupmask);
							}
						}
					}
				}
			}
		}

		//mxd
		private static List<int> GetIndices(string input) 
		{
			string[] parts = input.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
			int index;
			List<int> result = new List<int>(parts.Length);

			foreach(string part in parts) if(int.TryParse(part, out index)) result.Add(index);

			return result;
		}
		
		#endregion

		#region ================== Marking

		/// <summary>This clears all marks on all elements.</summary>
		public void ClearAllMarks(bool mark)
		{
			ClearMarkedVertices(mark);
			ClearMarkedThings(mark);
			ClearMarkedLinedefs(mark);
			ClearMarkedSectors(mark);
			ClearMarkedSidedefs(mark);
		}

		/// <summary>This clears all marks on all vertices.</summary>
		public void ClearMarkedVertices(bool mark)
		{
			foreach(Vertex v in vertices) v.Marked = mark;
		}

		/// <summary>This clears all marks on all things.</summary>
		public void ClearMarkedThings(bool mark)
		{
			foreach(Thing t in things) t.Marked = mark;
		}

		/// <summary>This clears all marks on all linedefs.</summary>
		public void ClearMarkedLinedefs(bool mark)
		{
			foreach(Linedef l in linedefs) l.Marked = mark;
		}

		/// <summary>This clears all marks on all sidedefs.</summary>
		public void ClearMarkedSidedefs(bool mark)
		{
			foreach(Sidedef s in sidedefs) s.Marked = mark;
		}

		/// <summary>This clears all marks on all sectors.</summary>
		public void ClearMarkedSectors(bool mark)
		{
			foreach(Sector s in sectors) s.Marked = mark;
		}

		/// <summary>This inverts all marks on all elements.</summary>
		public void InvertAllMarks()
		{
			InvertMarkedVertices();
			InvertMarkedThings();
			InvertMarkedLinedefs();
			InvertMarkedSectors();
			InvertMarkedSidedefs();
		}

		/// <summary>This inverts all marks on all vertices.</summary>
		public void InvertMarkedVertices()
		{
			foreach(Vertex v in vertices) v.Marked = !v.Marked;
		}

		/// <summary>This inverts all marks on all things.</summary>
		public void InvertMarkedThings()
		{
			foreach(Thing t in things) t.Marked = !t.Marked;
		}

		/// <summary>This inverts all marks on all linedefs.</summary>
		public void InvertMarkedLinedefs()
		{
			foreach(Linedef l in linedefs) l.Marked = !l.Marked;
		}

		/// <summary>This inverts all marks on all sidedefs.</summary>
		public void InvertMarkedSidedefs()
		{
			foreach(Sidedef s in sidedefs) s.Marked = !s.Marked;
		}

		/// <summary>This inverts all marks on all sectors.</summary>
		public void InvertMarkedSectors()
		{
			foreach(Sector s in sectors) s.Marked = !s.Marked;
		}

		/// <summary>Returns a collection of vertices that match a marked state.</summary>
		public List<Vertex> GetMarkedVertices(bool mark)
		{
			List<Vertex> list = new List<Vertex>(numvertices >> 1);
			foreach(Vertex v in vertices) if(v.Marked == mark) list.Add(v);
			return list;
		}

		/// <summary>Returns a collection of things that match a marked state.</summary>
		public List<Thing> GetMarkedThings(bool mark)
		{
			List<Thing> list = new List<Thing>(numthings >> 1);
			foreach(Thing t in things) if(t.Marked == mark) list.Add(t);
			return list;
		}

		/// <summary>Returns a collection of linedefs that match a marked state.</summary>
		public List<Linedef> GetMarkedLinedefs(bool mark)
		{
			List<Linedef> list = new List<Linedef>(numlinedefs >> 1);
			foreach(Linedef l in linedefs) if(l.Marked == mark) list.Add(l);
			return list;
		}

		/// <summary>Returns a collection of sidedefs that match a marked state.</summary>
		public List<Sidedef> GetMarkedSidedefs(bool mark)
		{
			List<Sidedef> list = new List<Sidedef>(numsidedefs >> 1);
			foreach(Sidedef s in sidedefs) if(s.Marked == mark) list.Add(s);
			return list;
		}

		/// <summary>Returns a collection of sectors that match a marked state.</summary>
		public List<Sector> GetMarkedSectors(bool mark)
		{
			List<Sector> list = new List<Sector>(numsectors >> 1);
			foreach(Sector s in sectors) if(s.Marked == mark) list.Add(s);
			return list;
		}

		/// <summary>This marks vertices based on selected vertices.</summary>
		public void MarkSelectedVertices(bool selected, bool mark)
		{
			foreach(Vertex v in sel_vertices) v.Marked = mark;
		}

		/// <summary>This marks linedefs based on selected linedefs.</summary>
		public void MarkSelectedLinedefs(bool selected, bool mark)
		{
			foreach(Linedef l in sel_linedefs) l.Marked = mark;
		}

		/// <summary>This marks sectors based on selected sectors.</summary>
		public void MarkSelectedSectors(bool selected, bool mark)
		{
			foreach(Sector s in sel_sectors) s.Marked = mark;
		}

		/// <summary>This marks things based on selected things.</summary>
		public void MarkSelectedThings(bool selected, bool mark)
		{
			foreach(Thing t in sel_things) t.Marked = mark;
		}

		/// <summary>
		/// This marks the front and back sidedefs on linedefs with the matching mark.
		/// </summary>
		public void MarkSidedefsFromLinedefs(bool matchmark, bool setmark)
		{
			foreach(Linedef l in linedefs)
			{
				if(l.Marked == matchmark)
				{
					if(l.Front != null) l.Front.Marked = setmark;
					if(l.Back != null) l.Back.Marked = setmark;
				}
			}
		}

		/// <summary>
		/// This marks the sidedefs that make up the sectors with the matching mark.
		/// </summary>
		public void MarkSidedefsFromSectors(bool matchmark, bool setmark)
		{
			foreach(Sidedef sd in sidedefs)
			{
				if(sd.Sector.Marked == matchmark) sd.Marked = setmark;
			}
		}

		/// <summary>
		/// Returns a collection of vertices that match a marked state on the linedefs.
		/// </summary>
		public ICollection<Vertex> GetVerticesFromLinesMarks(bool mark)
		{
			List<Vertex> list = new List<Vertex>(numvertices >> 1);
			foreach(Vertex v in vertices)
			{
				foreach(Linedef l in v.Linedefs)
				{
					if(l.Marked == mark)
					{
						list.Add(v);
						break;
					}
				}
			}
			return list;
		}

		/// <summary>
		/// Returns a collection of vertices that match a marked state on the linedefs.
		/// The difference with GetVerticesFromLinesMarks is that in this method
		/// ALL linedefs of a vertex must match the specified marked state.
		/// </summary>
		public ICollection<Vertex> GetVerticesFromAllLinesMarks(bool mark)
		{
			List<Vertex> list = new List<Vertex>(numvertices >> 1);
			foreach(Vertex v in vertices)
			{
				bool qualified = true;
				foreach(Linedef l in v.Linedefs)
				{
					if(l.Marked != mark)
					{
						qualified = false;
						break;
					}
				}
				if(qualified) list.Add(v);
			}
			return list;
		}

		/// <summary>
		/// Returns a collection of vertices that match a marked state on the linedefs.
		/// </summary>
		public ICollection<Vertex> GetVerticesFromSectorsMarks(bool mark)
		{
			List<Vertex> list = new List<Vertex>(numvertices >> 1);
			foreach(Vertex v in vertices)
			{
				foreach(Linedef l in v.Linedefs)
				{
					if(((l.Front != null) && (l.Front.Sector != null) && (l.Front.Sector.Marked == mark)) ||
						((l.Back != null) && (l.Back.Sector != null) && (l.Back.Sector.Marked == mark)))
					{
						list.Add(v);
						break;
					}
				}
			}
			return list;
		}

		/// <summary>
		/// This marks all selected geometry, including sidedefs from sectors.
		/// When sidedefsfromsectors is true, then the sidedefs are marked according to the
		/// marked sectors. Otherwise the sidedefs are marked according to the marked linedefs.
		/// </summary>
		public void MarkAllSelectedGeometry(bool mark, bool linedefsfromvertices, bool verticesfromlinedefs, bool sectorsfromlinedefs, bool sidedefsfromsectors)
		{
			General.Map.Map.ClearAllMarks(!mark);

			// Direct vertices
			General.Map.Map.MarkSelectedVertices(true, mark);

			// Direct linedefs
			General.Map.Map.MarkSelectedLinedefs(true, mark);

			// Linedefs from vertices
			// We do this before "vertices from lines" because otherwise we get lines marked that we didn't select
			if(linedefsfromvertices)
			{
				ICollection<Linedef> lines = General.Map.Map.LinedefsFromMarkedVertices(!mark, mark, !mark);
				foreach(Linedef l in lines) l.Marked = mark;
			}
			
			// Vertices from linedefs
			if(verticesfromlinedefs)
			{
				ICollection<Vertex> verts = General.Map.Map.GetVerticesFromLinesMarks(mark);
				foreach(Vertex v in verts) v.Marked = mark;
			}
			
			// Mark sectors from linedefs (note: this must be the first to mark
			// sectors, because this clears the sector marks!)
			if(sectorsfromlinedefs)
			{
				General.Map.Map.ClearMarkedSectors(mark);
				foreach(Linedef l in General.Map.Map.Linedefs)
				{
					if(!l.Selected)
					{
						if(l.Front != null) l.Front.Sector.Marked = !mark;
						if(l.Back != null) l.Back.Sector.Marked = !mark;
					}
				}
			}
			
			// Direct sectors
			General.Map.Map.MarkSelectedSectors(true, mark);

			// Direct things
			General.Map.Map.MarkSelectedThings(true, mark);

			// Sidedefs from linedefs or sectors
			if(sidedefsfromsectors)
				General.Map.Map.MarkSidedefsFromSectors(true, mark);
			else
				General.Map.Map.MarkSidedefsFromLinedefs(true, mark);
		}
		
		#endregion

		#region ================== Indexing
		
		/// <summary>
		/// Returns the vertex at the specified index. Returns null when index is out of range. This is an O(1) operation.
		/// </summary>
		public Vertex GetVertexByIndex(int index)
		{
			return index < numvertices ? vertices[index] : null;
		}

		/// <summary>
		/// Returns the linedef at the specified index. Returns null when index is out of range. This is an O(1) operation.
		/// </summary>
		public Linedef GetLinedefByIndex(int index)
		{
			return index < numlinedefs ? linedefs[index] : null;
		}

		/// <summary>
		/// Returns the sidedef at the specified index. Returns null when index is out of range. This is an O(1) operation.
		/// </summary>
		public Sidedef GetSidedefByIndex(int index)
		{
			return index < numsidedefs ? sidedefs[index] : null;
		}

		/// <summary>
		/// Returns the sector at the specified index. Returns null when index is out of range. This is an O(1) operation.
		/// </summary>
		public Sector GetSectorByIndex(int index)
		{
			return index < numsectors ? sectors[index] : null;
		}

		/// <summary>
		/// Returns the thing at the specified index. Returns null when index is out of range. This is an O(1) operation.
		/// </summary>
		public Thing GetThingByIndex(int index)
		{
			return index < numthings ? things[index] : null;
		}

		#endregion
		
		#region ================== Areas

		/// <summary>This creates an initial, undefined area.</summary>
		public static RectangleF CreateEmptyArea()
		{
			return new RectangleF(float.MaxValue / 2, float.MaxValue / 2, -float.MaxValue, -float.MaxValue);
		}

		/// <summary>This creates an area from vertices.</summary>
		public static RectangleF CreateArea(ICollection<Vertex> verts)
		{
			// Do NOT use double here, otherwise the width and height of the RectangleF will become NaN
			float l = float.MaxValue;
			float t = float.MaxValue;
			float r = float.MinValue;
			float b = float.MinValue;

			// Go for all vertices
			foreach(Vertex v in verts)
			{
				// Adjust boundaries by vertices
				if(v.Position.x < l) l = (float)v.Position.x;
				if(v.Position.x > r) r = (float)v.Position.x;
				if(v.Position.y < t) t = (float)v.Position.y;
				if(v.Position.y > b) b = (float)v.Position.y;
			}

			// Return a rect
			return new RectangleF(l, t, r - l, b - t);
		}

		/// <summary>This increases and existing area with the given vertices.</summary>
		public static RectangleF IncreaseArea(RectangleF area, ICollection<Vertex> verts)
		{
			// Do NOT use double here, otherwise the width and height of the RectangleF will become NaN
			float l = area.Left;
			float t = area.Top;
			float r = area.Right;
			float b = area.Bottom;
			
			// Go for all vertices
			foreach(Vertex v in verts)
			{
				// Adjust boundaries by vertices
				if(v.Position.x < l) l = (float)v.Position.x;
				if(v.Position.x > r) r = (float)v.Position.x;
				if(v.Position.y < t) t = (float)v.Position.y;
				if(v.Position.y > b) b = (float)v.Position.y;
			}
			
			// Return a rect
			return new RectangleF(l, t, r - l, b - t);
		}

		/// <summary>This increases and existing area with the given things.</summary>
		public static RectangleF IncreaseArea(RectangleF area, ICollection<Thing> things)
		{
			// Do NOT use double here, otherwise the width and height of the RectangleF will become NaN
			float l = area.Left;
			float t = area.Top;
			float r = area.Right;
			float b = area.Bottom;
			
			// Go for all vertices
			foreach(Thing th in things)
			{
				// Adjust boundaries by vertices
				if(th.Position.x < l) l = (float)th.Position.x;
				if(th.Position.x > r) r = (float)th.Position.x;
				if(th.Position.y < t) t = (float)th.Position.y;
				if(th.Position.y > b) b = (float)th.Position.y;
			}
			
			// Return a rect
			return new RectangleF(l, t, r - l, b - t);
		}

		/// <summary>This increases and existing area with the given vertices.</summary>
		public static RectangleF IncreaseArea(RectangleF area, ICollection<Vector2D> verts)
		{
			// Do NOT use double here, otherwise the width and height of the RectangleF will become NaN
			float l = area.Left;
			float t = area.Top;
			float r = area.Right;
			float b = area.Bottom;
			
			// Go for all vertices
			foreach(Vector2D v in verts)
			{
				// Adjust boundaries by vertices
				if(v.x < l) l = (float)v.x;
				if(v.x > r) r = (float)v.x;
				if(v.y < t) t = (float)v.y;
				if(v.y > b) b = (float)v.y;
			}
			
			// Return a rect
			return new RectangleF(l, t, r - l, b - t);
		}

		/// <summary>This increases and existing area with the given vertex.</summary>
		public static RectangleF IncreaseArea(RectangleF area, Vector2D vert)
		{
			// Do NOT use double here, otherwise the width and height of the RectangleF will become NaN
			float l = area.Left;
			float t = area.Top;
			float r = area.Right;
			float b = area.Bottom;
			
			// Adjust boundaries by vertices
			if(vert.x < l) l = (float)vert.x;
			if(vert.x > r) r = (float)vert.x;
			if(vert.y < t) t = (float)vert.y;
			if(vert.y > b) b = (float)vert.y;

			// Return a rect
			return new RectangleF(l, t, r - l, b - t);
		}

		/// <summary>This creates an area from linedefs.</summary>
		public static RectangleF CreateArea(ICollection<Linedef> lines)
		{
			// Do NOT use double here, otherwise the width and height of the RectangleF will become NaN
			float l = float.MaxValue;
			float t = float.MaxValue;
			float r = float.MinValue;
			float b = float.MinValue;

			// Go for all linedefs
			foreach(Linedef ld in lines)
			{
				// Adjust boundaries by vertices
				if(ld.Start.Position.x < l) l = (float)ld.Start.Position.x;
				if(ld.Start.Position.x > r) r = (float)ld.Start.Position.x;
				if(ld.Start.Position.y < t) t = (float)ld.Start.Position.y;
				if(ld.Start.Position.y > b) b = (float)ld.Start.Position.y;
				if(ld.End.Position.x < l) l = (float)ld.End.Position.x;
				if(ld.End.Position.x > r) r = (float)ld.End.Position.x;
				if(ld.End.Position.y < t) t = (float)ld.End.Position.y;
				if(ld.End.Position.y > b) b = (float)ld.End.Position.y;
			}

			// Return a rect
			return new RectangleF(l, t, r - l, b - t);
		}

		/// <summary>This increases and existing area with the given linedefs.</summary>
		public static RectangleF IncreaseArea(RectangleF area, ICollection<Linedef> lines) //mxd
		{
			// Do NOT use double here, otherwise the width and height of the RectangleF will become NaN
			float l = area.Left;
			float t = area.Top;
			float r = area.Right;
			float b = area.Bottom;

			// Go for all vertices
			foreach(Linedef ld in lines)
			{
				// Adjust boundaries by vertices
				if(ld.Start.Position.x < l) l = (float)ld.Start.Position.x;
				if(ld.Start.Position.x > r) r = (float)ld.Start.Position.x;
				if(ld.Start.Position.y < t) t = (float)ld.Start.Position.y;
				if(ld.Start.Position.y > b) b = (float)ld.Start.Position.y;
				if(ld.End.Position.x < l) l = (float)ld.End.Position.x;
				if(ld.End.Position.x > r) r = (float)ld.End.Position.x;
				if(ld.End.Position.y < t) t = (float)ld.End.Position.y;
				if(ld.End.Position.y > b) b = (float)ld.End.Position.y;
			}

			// Return a rect
			return new RectangleF(l, t, r - l, b - t);
		}

		/// <summary>This filters lines by a rectangular area.</summary>
		public static HashSet<Linedef> FilterByArea(ICollection<Linedef> lines, ref RectangleF area)
		{
			HashSet<Linedef> newlines = new HashSet<Linedef>();
			
			// Go for all lines
			foreach(Linedef l in lines)
			{
				// Check the cs field bits
				if((GetCSFieldBits(l.Start.Position, area) & GetCSFieldBits(l.End.Position, area)) == 0) 
				{
					// The line could be in the area
					newlines.Add(l);
				}
			}
			
			// Return result
			return newlines;
		}

		/// <summary> This returns the cohen-sutherland field bits for a vector in a rectangle area</summary>
		public static int GetCSFieldBits(Vector2D v, RectangleF area) 
		{
			int bits = 0;
			if(v.y < area.Top) bits |= 0x01;
			if(v.y > area.Bottom) bits |= 0x02;
			if(v.x < area.Left) bits |= 0x04;
			if(v.x > area.Right) bits |= 0x08;
			return bits;
		}

		/// <summary>This filters vertices by a rectangular area.</summary>
		public static ICollection<Vertex> FilterByArea(ICollection<Vertex> verts, ref RectangleF area)
		{
			ICollection<Vertex> newverts = new List<Vertex>(verts.Count);

			// Go for all verts
			foreach(Vertex v in verts)
			{
				// Within rect?
				if((v.Position.x < area.Left) || (v.Position.x > area.Right) ||
					(v.Position.y < area.Top) || (v.Position.y > area.Bottom)) continue;

				// The vertex is in the area
				newverts.Add(v);
			}

			// Return result
			return newverts;
		}

		#endregion

		#region ================== Stitching

		/// <summary>
		/// Stitches marked geometry with non-marked geometry. Returns false when the operation failed.
		/// </summary>
		public bool StitchGeometry() { return StitchGeometry(MergeGeometryMode.CLASSIC); } //mxd. Compatibility
		public bool StitchGeometry(MergeGeometryMode mergemode)
		{
			// Find vertices
			HashSet<Vertex> movingverts = new HashSet<Vertex>(General.Map.Map.GetMarkedVertices(true));
			HashSet<Vertex> fixedverts = new HashSet<Vertex>(General.Map.Map.GetMarkedVertices(false));
			
			// Find lines that moved during the drag
			HashSet<Linedef> movinglines = new HashSet<Linedef>(LinedefsFromMarkedVertices(false, true, true));
			
			// Find all non-moving lines
			HashSet<Linedef> fixedlines = new HashSet<Linedef>(LinedefsFromMarkedVertices(true, false, false));
			
			// Determine area in which we are editing
			RectangleF editarea = CreateArea(movinglines);
			editarea = IncreaseArea(editarea, movingverts);
			editarea.Inflate(1.0f, 1.0f);
			
			// Join nearby vertices
			BeginAddRemove();
			JoinVertices(fixedverts, movingverts, true, STITCH_DISTANCE);
			EndAddRemove();
			
			// Update cached values of lines because we need their length/angle
			Update(true, false);

			BeginAddRemove();
			
			// Split moving lines with unselected vertices
			ICollection<Vertex> nearbyfixedverts = FilterByArea(fixedverts, ref editarea);
			if (!SplitLinesByVertices(movinglines, nearbyfixedverts, STITCH_DISTANCE, movinglines, mergemode))
			{
				EndAddRemove(); // Unfreeze arrays before returning
				return false;
			}

            // Split non-moving lines with selected vertices
            fixedlines = new HashSet<Linedef>(fixedlines.Where(fixedline => !fixedline.IsDisposed));
			if (!SplitLinesByVertices(fixedlines, movingverts, STITCH_DISTANCE, movinglines, mergemode))
			{
				EndAddRemove(); // Unfreeze arrays before returning
				return false;
			}

			//mxd. Split moving lines with fixed lines
			if (!SplitLinesByLines(fixedlines, movinglines, mergemode))
			{
				EndAddRemove(); // Unfreeze arrays before returning
				return false;
			}
			
			// Remove looped linedefs
			RemoveLoopedLinedefs(movinglines);

			// Join overlapping lines
			if (!JoinOverlappingLines(movinglines))
			{
				EndAddRemove(); // Unfreeze arrays before returning
				return false;
			}

			//mxd. Remove remaining new verts from dragged shape if possible
			if(mergemode == MergeGeometryMode.REPLACE)
			{
				// Collect verts created by splitting. Can't use GetMarkedVertices here, because we are in the middle of AddRemove
				HashSet<Vertex> tocheck = new HashSet<Vertex>();
				foreach(Vertex v in vertices)
				{
					if(v != null && v.Marked && !movingverts.Contains(v)) tocheck.Add(v);
				}

				// Remove verts, which are not part of initially dragged verts
				foreach(Vertex v in tocheck)
				{
					if(!v.IsDisposed && v.Linedefs.Count == 2)
					{
						Linedef ld1 = General.GetByIndex(v.Linedefs, 0);
						Linedef ld2 = General.GetByIndex(v.Linedefs, 1);

						Vertex v2 = (ld2.Start == v) ? ld2.End : ld2.Start;
						if(ld1.Start == v) ld1.SetStartVertex(v2); else ld1.SetEndVertex(v2);
						ld2.Dispose();

						// Trash vertex
						v.Dispose();
					}
				}
			}

			EndAddRemove();

			// Collect changed lines... We need those in by-vertex-index order
			// (otherwise SectorBuilder logic in some cases will incorrectly assign sector propertes)
			List<Vertex> markedverts = GetMarkedVertices(true);
			List<Linedef> changedlines = new List<Linedef>(markedverts.Count / 2);
			HashSet<Linedef> changedlineshash = new HashSet<Linedef>();
			foreach(Vertex v in markedverts)
			{
				foreach(Linedef l in v.Linedefs)
				{
					if(!changedlineshash.Contains(l))
					{
						changedlines.Add(l);
						changedlineshash.Add(l);
					}
				}
			}

			//mxd. Correct sector references
			if(mergemode != MergeGeometryMode.CLASSIC)
			{
				// Linedefs cache needs to be up to date...
				Update(true, false);
				
				// Fix stuff...
				CorrectSectorReferences(changedlines, true);
				CorrectOuterSides(new HashSet<Linedef>(changedlines));

				// Mark only fully selected sectors
				ClearMarkedSectors(false);
				HashSet<Sector> changedsectors = GetSectorsFromLinedefs(changedlines);
				foreach(Sector s in changedsectors) s.Marked = true;
			}
			else
			{
				FlipBackwardLinedefs(changedlines);
			}
			
			return true;
		}

		//mxd. Shameless SLADEMap::correctSectors ripoff... Corrects/builds sectors for all lines in [lines]
		private static void CorrectSectorReferences(List<Linedef> lines, bool existing_only)
		{
			//DebugConsole.Clear();
			//DebugConsole.WriteLine("CorrectSectorReferences for " + lines.Count + " lines");
			
            // ano - set a bunch of foreaches to be for()s because they're faster

			// Create a list of sidedefs to perform sector creation with
			List<LinedefSide> edges = new List<LinedefSide>();
			if(existing_only)
			{
                int lineCount = lines.Count;
				for(int i = 0; i < lineCount; i++)
				{
                    Linedef l = lines[i];
					// Add only existing sides as edges (or front side if line has none)
					if(l.Front != null || l.Back == null)
						edges.Add(new LinedefSide(l, true));
					if(l.Back != null)
						edges.Add(new LinedefSide(l, false));
				}
			}
			else
			{
                int lineCount = lines.Count;
                for (int i = 0; i < lineCount; i++)
                {
                    Linedef l = lines[i];
                    // Add front side
                    edges.Add(new LinedefSide(l, true));

					// Add back side if there's a sector
					if(General.Map.Map.GetSectorByCoordinates(l.GetSidePoint(false)) != null)
						edges.Add(new LinedefSide(l, false));
				}
			}

			HashSet<Sidedef> sides_correct = new HashSet<Sidedef>();
            int edgeCount = edges.Count;
            for (int i = 0; i < edgeCount; i++)
            {
                LinedefSide ls = edges[i];
                if (ls.Front && ls.Line.Front != null)
					sides_correct.Add(ls.Line.Front);
				else if(!ls.Front && ls.Line.Back != null)
					sides_correct.Add(ls.Line.Back);
			}

			//mxd. Get affected sectors
			HashSet<Sector> affectedsectors = new HashSet<Sector>(General.Map.Map.GetSelectedSectors(true));
			affectedsectors.UnionWith(General.Map.Map.GetUnselectedSectorsFromLinedefs(lines));

			//mxd. Collect their sidedefs
			HashSet<Sidedef> sectorsides = new HashSet<Sidedef>();
			foreach(Sector s in affectedsectors) sectorsides.UnionWith(s.Sidedefs);

			// Build sectors
			SectorBuilder builder = new SectorBuilder();
			List<Sector> sectors_reused = new List<Sector>();

            for (int i = 0; i < edgeCount; i++)
            {
                LinedefSide ls = edges[i];
                // Skip if edge is ignored
                //DebugConsole.WriteLine((ls.Ignore ? "Ignoring line " : "Processing line ") + ls.Line.Index);
                if (ls.Ignore) continue;

                // Run sector builder on current edge
                Stopwatch watch = new Stopwatch();
                watch.Start();
                if (!builder.TraceSector(ls.Line, ls.Front))
                {
                    //General.ErrorLogger.Add(ErrorType.Warning, string.Format("TraceSector: took {0}ms, failed!", watch.ElapsedMilliseconds));
                    continue; // Don't create sector if trace failed
                }
                //General.ErrorLogger.Add(ErrorType.Warning, string.Format("TraceSector: took {0}ms", watch.ElapsedMilliseconds));

                // Find any subsequent edges that were part of the sector created
                bool has_existing_lines = false;
				bool has_existing_sides = false;
				//bool has_zero_sided_lines = false;
				bool has_dragged_sides = false; //mxd
				List<LinedefSide> edges_in_sector = new List<LinedefSide>();
				foreach(LinedefSide edge in builder.SectorEdges)
				{
					bool line_is_ours = false;
					bool side_exists = (edge.Front ? edge.Line.Front != null : edge.Line.Back != null); //mxd
					if(side_exists && sectorsides.Contains(edge.Front ? edge.Line.Front : edge.Line.Back))
						has_dragged_sides = true; //mxd

                    for (int k = 0; k < edgeCount; k++)
                    {
                        LinedefSide ls2 = edges[k];
                        if (ls2.Line == edge.Line)
						{
							line_is_ours = true;
							if(ls2.Front == edge.Front)
							{
								edges_in_sector.Add(ls2);
								break;
							}
						}
					}

                    // ano - so this inner part was already commented out
                    // so i just put the /* */ around it
					/*if(line_is_ours)
					{
						//if(edge.Line.Front == null && edge.Line.Back == null)
							//has_zero_sided_lines = true;
					}
					else*/

                    if(!line_is_ours)
					{
						has_existing_lines = true;
						has_existing_sides |= side_exists; //mxd
					}
				}

				// Pasting or moving a two-sided line into an enclosed void should NOT
				// create a new sector out of the entire void.
				// Heuristic: if the traced sector includes any edges that are NOT
				// "ours", and NONE of those edges already exist, that sector must be
				// in an enclosed void, and should not be drawn.
				// However, if existing_only is false, the caller expects us to create
				// new sides anyway; skip this check.
				if(existing_only && has_existing_lines && !has_existing_sides && !has_dragged_sides)
					continue;

				// Ignore traced edges when trying to create any further sectors
				foreach(LinedefSide ls3 in edges_in_sector) ls3.Ignore = true;

				// Check if sector traced is already valid
				if(builder.IsValidSector()) continue;

				// Check if we traced over an existing sector (or part of one)
				Sector sector = builder.FindExistingSector(sides_correct);
				if(sector != null)
				{
					// Check if it's already been (re)used
					bool reused = false;
					foreach(Sector s in sectors_reused)
					{
						if(s == sector)
						{
							reused = true;
							break;
						}
					}

					// If we can reuse the sector, do so
					if(!reused)
						sectors_reused.Add(sector);
					else
						sector = null;
				}

				// Create sector
				builder.CreateSector(sector, null);
			}

            // Remove any sides that weren't part of a sector
            for (int i = 0; i < edgeCount; i++)
            {
                LinedefSide ls = edges[i];
                if (ls.Ignore || ls.Line == null) continue;
                if (ls.Line.Start == null || ls.Line.End == null)
                    throw new Exception("ls line is null");

				if(ls.Front)
				{
					if(ls.Line.Front != null)
					{
						ls.Line.Front.Dispose();

						// Update doublesided flag
						ls.Line.ApplySidedFlags();
					}
				}
				else
				{
					if(ls.Line.Back != null)
					{
						ls.Line.Back.Dispose();

						// Update doublesided flag
						ls.Line.ApplySidedFlags();
					}
				}
			}

			// Check if any lines need to be flipped
			FlipBackwardLinedefs(lines);

			// Find an adjacent sector to copy properties from
			Sector sector_copy = null;
			foreach(Linedef l in lines)
			{
				// Check front sector
				Sector sector = (l.Front != null ? l.Front.Sector : null);
				if(sector != null && !sector.Marked)
				{
					// Copy this sector if it isn't newly created
					sector_copy = sector;
					break;
				}

				// Check back sector
				sector = (l.Back != null ? l.Back.Sector : null);
				if(sector != null && !sector.Marked)
				{
					// Copy this sector if it isn't newly created
					sector_copy = sector;
					break;
				}
			}

			// Go through newly created sectors
			List<Sector> newsectors = General.Map.Map.GetMarkedSectors(true); //mxd
			foreach(Sector s in newsectors)
			{
				// Skip if sector already has properties
				if(s.CeilTexture != "-" || s.FloorTexture != "-"
					|| s.FloorHeight != General.Settings.DefaultFloorHeight
					|| s.CeilHeight != General.Settings.DefaultCeilingHeight)
					continue;

				// Copy from adjacent sector if any
				if(sector_copy != null)
				{
					sector_copy.CopyPropertiesTo(s);
					continue;
				}

				// Otherwise, use defaults from game configuration
				s.SetFloorTexture(General.Map.Options.DefaultFloorTexture);
				s.SetCeilTexture(General.Map.Options.DefaultCeilingTexture);
				s.FloorHeight = General.Settings.DefaultFloorHeight;
				s.CeilHeight = General.Settings.DefaultCeilingHeight;
				s.Brightness = General.Settings.DefaultBrightness;
			}

			// Update line textures
			List<Sidedef> newsides = General.Map.Map.GetMarkedSidedefs(true);
			foreach(Sidedef side in newsides)
			{
				// Clear any unneeded textures
				side.RemoveUnneededTextures(side.Other != null, false, true);

				// Set middle texture if needed
				if(side.MiddleRequired() && side.MiddleTexture == "-")
				{
					// Find adjacent texture (any)
					string tex = GetAdjacentMiddleTexture(side.Line.Start);
					if(tex == "-") tex = GetAdjacentMiddleTexture(side.Line.End);

					// If no adjacent texture, get default from game configuration
					if(tex == "-") tex = General.Settings.DefaultTexture;

					// Set texture
					side.SetTextureMid(tex);
				}

				// Update sided flags
				side.Line.ApplySidedFlags();
			}

			// Remove any extra sectors
			General.Map.Map.RemoveUnusedSectors(false);
		}

		//mxd. Try to create outer sidedefs if needed
		private static void CorrectOuterSides(HashSet<Linedef> changedlines)
		{
			HashSet<Linedef> linesmissingfront = new HashSet<Linedef>();
			HashSet<Linedef> linesmissingback = new HashSet<Linedef>();

			// Collect lines without front/back sides
			foreach(Linedef line in changedlines)
			{
				if(line.Back == null) linesmissingback.Add(line);
				if(line.Front == null) linesmissingfront.Add(line);
			}

			// Anything to do?
			if(linesmissingfront.Count == 0 && linesmissingback.Count == 0) return;

			// Let's use a blockmap...
			RectangleF area = CreateArea(linesmissingfront);
			area = IncreaseArea(area, linesmissingback);
			BlockMap<BlockEntry> blockmap = new BlockMap<BlockEntry>(area);
			blockmap.AddSectorsSet(General.Map.Map.Sectors);

			// Find sectors to join singlesided lines
			Dictionary<Linedef, Sector> linefrontsectorref = new Dictionary<Linedef, Sector>();
			foreach(Linedef line in linesmissingfront)
			{
				// Line is now inside a sector?
				Sector nearest = FindSectorContaining(blockmap, line);

				// We can reattach our line!
				if(nearest != null) linefrontsectorref[line] = nearest;
			}

			Dictionary<Linedef, Sector> linebacksectorref = new Dictionary<Linedef, Sector>();
			foreach(Linedef line in linesmissingback)
			{
				// Line is now inside a sector?
				Sector nearest = FindSectorContaining(blockmap, line);

				// We can reattach our line!
				if(nearest != null) linebacksectorref[line] = nearest;
			}

			// Check single-sided lines. Add new sidedefs if necessary
			// Key is dragged single-sided line, value is a sector dragged line ended up in.
			foreach(KeyValuePair<Linedef, Sector> group in linefrontsectorref)
			{
				Linedef line = group.Key;

				// Create new sidedef
				Sidedef newside = General.Map.Map.CreateSidedef(line, true, group.Value);

				// Copy props from the other side
				Sidedef propssource = (line.Front ?? line.Back);
				propssource.CopyPropertiesTo(newside);

				// Correct the linedef
				if((line.Front == null) && (line.Back != null))
				{
					line.FlipVertices();
					line.FlipSidedefs();
				}

				// Adjust textures
				if(line.Front != null) line.Front.RemoveUnneededTextures(line.Back != null, false, true);
				if(line.Back != null) line.Back.RemoveUnneededTextures(line.Front != null, false, true);

				// Correct the sided flags
				line.ApplySidedFlags();
			}

			foreach(KeyValuePair<Linedef, Sector> group in linebacksectorref)
			{
				Linedef line = group.Key;

				// Create new sidedef
				Sidedef newside = General.Map.Map.CreateSidedef(line, false, group.Value);

				// Copy props from the other side
				Sidedef propssource = (line.Front ?? line.Back);
				propssource.CopyPropertiesTo(newside);

				// Correct the linedef
				if((line.Front == null) && (line.Back != null))
				{
					line.FlipVertices();
					line.FlipSidedefs();
				}

				// Adjust textures
				if(line.Front != null) line.Front.RemoveUnneededTextures(line.Back != null, false, true);
				if(line.Back != null) line.Back.RemoveUnneededTextures(line.Front != null, false, true);

				// Correct the sided flags
				line.ApplySidedFlags();
			}
		}

		//mxd
		private static Sector FindSectorContaining(BlockMap<BlockEntry> sectorsmap, Linedef line)
		{
			HashSet<BlockEntry> blocks = new HashSet<BlockEntry>
			{
				sectorsmap.GetBlockAt(line.Start.Position),
				sectorsmap.GetBlockAt(line.End.Position),
			};

			foreach(BlockEntry be in blocks)
			{
				foreach(Sector sector in be.Sectors)
				{
					// Check if target line is inside the found sector
					if(sector.Intersect(line.Start.Position, false) && sector.Intersect(line.End.Position, false))
						return sector;
				}
			}

			return null;
		}

		//mxd
		private static string GetAdjacentMiddleTexture(Vertex v)
		{
			// Go through adjacent lines
			foreach(Linedef l in v.Linedefs)
			{
				if(l.Front != null && l.Front.MiddleTexture != "-") return l.Front.MiddleTexture;
				if(l.Back != null && l.Back.MiddleTexture != "-") return l.Back.MiddleTexture;
			}

			return "-";
		}
		
		#endregion
		
		#region ================== Geometry Tools

		/// <summary>This removes any virtual sectors in the map and returns the number of sectors removed.</summary>
		public int RemoveVirtualSectors()
		{
			int count = 0;
			int index = 0;
			
			// Go for all sectors
			while(index < numsectors)
			{
				// Remove when virtual
				if(sectors[index].Fields.ContainsKey(VIRTUAL_SECTOR_FIELD))
				{
					sectors[index].Dispose();
					count++;
				}
				else
				{
					index++;
				}
			}
			
			return count;
		}

		/// <summary>This removes unused sectors and returns the number of removed sectors.</summary>
		public int RemoveUnusedSectors(bool reportwarnings)
		{
			int count = 0;
			int index = numsectors - 1;
			
			// Go for all sectors
			while(index >= 0)
			{
				// Remove when unused
				if(sectors[index].Sidedefs.Count == 0)
				{
					if(reportwarnings)
						General.ErrorLogger.Add(ErrorType.Warning, "Sector " + index + " was unused and has been removed.");

					sectors[index].Dispose();
					count++;
				}

				index--;
			}
			
			return count;
		}

		/// <summary>This joins overlapping lines together. Returns false when the operation failed.</summary>
		public static bool JoinOverlappingLines(ICollection<Linedef> lines)
		{
			bool joined;
			
			do
			{
				// No joins yet
				joined = false;

				// Go for all the lines
				foreach(Linedef l1 in lines)
				{
					// Check if these vertices have lines that overlap
					foreach(Linedef l2 in l1.Start.Linedefs)
					{
						//mxd. The same line?
						if(l1.Index == l2.Index) continue;
						
						// Sharing vertices?
						if(l1.End == l2.End || l1.End == l2.Start)
						{
							bool oppositedirection = (l1.End == l2.Start);
							bool l2marked = l2.Marked;

							// Merge these two linedefs
							while(lines.Remove(l2));
							if(!l2.Join(l1)) return false;

							// If l2 was marked as new geometry, we have to make sure
							// that l1's FrontInterior is correct for the drawing procedure
							if(l2marked) 
							{
								l1.FrontInterior = l2.FrontInterior ^ oppositedirection;
							}
							// If l1 is marked as new geometry, we may need to flip it to preserve
							// orientation of the original geometry, and update its FrontInterior
							else if(l1.Marked) 
							{
								if(oppositedirection) 
								{
									l1.FlipVertices();		// This also flips FrontInterior
									l1.FlipSidedefs();
								}
							}

							joined = true;
							break;
						}
					}
					
					// Will have to restart when joined
					if(joined) break;
					
					// Check if these vertices have lines that overlap
					foreach(Linedef l2 in l1.End.Linedefs)
					{
						//mxd. The same line?
						if(l1.Index == l2.Index) continue;
						
						// Sharing vertices?
						if(l1.Start == l2.End || l1.Start == l2.Start)
						{
							bool oppositedirection = (l1.Start == l2.End);
							bool l2marked = l2.Marked;

							// Merge these two linedefs
							while(lines.Remove(l2));
							if(!l2.Join(l1)) return false;

							// If l2 was marked as new geometry, we have to make sure
							// that l1's FrontInterior is correct for the drawing procedure
							if(l2marked) 
							{
								l1.FrontInterior = l2.FrontInterior ^ oppositedirection;
							}
							// If l1 is marked as new geometry, we may need to flip it to preserve
							// orientation of the original geometry, and update its FrontInterior
							else if(l1.Marked) 
							{
								if(oppositedirection) 
								{
									l1.FlipVertices();		// This also flips FrontInterior
									l1.FlipSidedefs();
								}
							}

							joined = true;
							break;
						}
					}
					
					// Will have to restart when joined
					if(joined) break;
				}
			}
			while(joined);

			// Return result
			return true;
		}

		/// <summary>This removes looped linedefs (linedefs which reference the same vertex for
		/// start and end) and returns the number of linedefs removed.</summary>
		public static int RemoveLoopedLinedefs(ICollection<Linedef> lines)
		{
			int linesremoved = 0;
			bool removedline;

			do
			{
				// Nothing removed yet
				removedline = false;

				// Go for all the lines
				foreach(Linedef l in lines)
				{
					// Check if referencing the same vertex twice (mxd. Or if both verts are null)
					if(l.Start == l.End || l.Start.Position == l.End.Position)
					{
						// Remove this line
						while(lines.Remove(l));
						l.Dispose();
						linesremoved++;
						removedline = true;
						break;
					}
				}
			}
			while(removedline);

			// Return result
			return linesremoved;
		}

		/// <summary>This joins nearby vertices from two collections. This does NOT join vertices
		/// within the same collection, only if they exist in both collections.
		/// The vertex from the second collection is moved to match the first vertex.
		/// When keepsecond is true, the vertex in the second collection is kept,
		/// otherwise the vertex in the first collection is kept.
		/// Returns the number of joins made.</summary>
		public static int JoinVertices(ICollection<Vertex> set1, ICollection<Vertex> set2, bool keepsecond, float joindist)
		{
			float joindist2 = joindist * joindist;
			int joinsdone = 0;
			bool joined;

			do
			{
				//mxd. Create blockmap
				ICollection<Vertex> biggerset, smallerset;
				bool keepsmaller;
				if(set1.Count > set2.Count)
				{
					biggerset = set1;
					smallerset = set2;
					keepsmaller = !keepsecond;
				}
				else
				{
					biggerset = set2;
					smallerset = set1;
					keepsmaller = keepsecond;
				}
				
				RectangleF area = CreateArea(biggerset);
				BlockMap<BlockEntry> blockmap = new BlockMap<BlockEntry>(area);
				blockmap.AddVerticesSet(biggerset);
				
				// No joins yet
				joined = false;

				// Go for all vertices in the smaller set
				foreach(Vertex v1 in smallerset)
				{
					HashSet<BlockEntry> blocks = new HashSet<BlockEntry>
					{
						blockmap.GetBlockAt(v1.Position), 
						blockmap.GetBlockAt(new Vector2D(v1.Position.x + joindist, v1.Position.y + joindist)), 
						blockmap.GetBlockAt(new Vector2D(v1.Position.x + joindist, v1.Position.y - joindist)), 
						blockmap.GetBlockAt(new Vector2D(v1.Position.x - joindist, v1.Position.y + joindist)), 
						blockmap.GetBlockAt(new Vector2D(v1.Position.x - joindist, v1.Position.y - joindist))
					};

					foreach(BlockEntry be in blocks)
					{
						if(be == null) continue;
						foreach(Vertex v2 in be.Vertices)
						{
							// Check if vertices are close enough
							if(v1.DistanceToSq(v2.Position) <= joindist2)
							{
								// Check if not the same vertex
								if(v1 != v2)
								{
									// Move the second vertex to match the first
									v2.Move(v1.Position);

									// Check which one to keep
									if(keepsmaller)
									{
										// Join the first into the second
										// Second is kept, first is removed
										v1.Join(v2);
										biggerset.Remove(v1);
										smallerset.Remove(v1);
									}
									else
									{
										// Join the second into the first
										// First is kept, second is removed
										v2.Join(v1);
										biggerset.Remove(v2);
										smallerset.Remove(v2);
									}

									// Count the join
									joinsdone++;
									joined = true;
									break;
								}
							}
						}
					}

					// Will have to restart when joined
					if(joined) break;
				}
			}
			while(joined);

			// Return result
			return joinsdone;
		}

		/// <summary>This joins nearby vertices in the same collection </summary>
		public static int JoinVertices(List<Vertex> set, float joindist) 
		{
			float joindist2 = joindist * joindist;
			int joinsdone = 0;
			bool joined;

			do 
			{
				// No joins yet
				joined = false;

				// Go for all vertices in the first set
				for(int i = 0; i < set.Count - 1; i++) 
				{
					for(int c = i + 1; c < set.Count; c++) 
					{
						Vertex v1 = set[i];
						Vertex v2 = set[c];

						// Check if vertices are close enough
						if(v1.DistanceToSq(v2.Position) <= joindist2) 
						{
							// Check if not the same vertex
							if(v1.Index != v2.Index) 
							{
								// Move the second vertex to match the first
								v2.Move(v1.Position);

								// Join the second into the first
								v2.Join(v1);
								set.Remove(v2);

								// Count the join
								joinsdone++;
								joined = true;
								break;
							}
						}
					}
				}
			} while(joined);

			// Return result
			return joinsdone;
		}

		/// <summary>This corrects lines that have a back sidedef but no front sidedef by flipping them. Returns the number of flips made.</summary>
		public static int FlipBackwardLinedefs(ICollection<Linedef> lines)
		{
			int flipsdone = 0;
			
			// Examine all lines
			foreach(Linedef l in lines)
			{
				// Back side but no front side?
				if((l.Back != null) && (l.Front == null))
				{
					// Flip that linedef!
					l.FlipVertices();
					l.FlipSidedefs();
					flipsdone++;
				}
			}

			// Return result
			return flipsdone;
		}

		/// <summary>This splits the given lines with the given vertices. All affected lines
		/// will be added to changedlines. Returns false when the operation failed.</summary>
		public static bool SplitLinesByVertices(ICollection<Linedef> lines, ICollection<Vertex> verts, float splitdist, ICollection<Linedef> changedlines) { return SplitLinesByVertices(lines, verts, splitdist, changedlines, MergeGeometryMode.CLASSIC); }
		public static bool SplitLinesByVertices(ICollection<Linedef> lines, ICollection<Vertex> verts, float splitdist, ICollection<Linedef> changedlines, MergeGeometryMode mergemode)
		{
			if(verts.Count == 0 || lines.Count == 0) return true; //mxd
			
			float splitdist2 = splitdist * splitdist;

			//mxd. Create blockmap
			RectangleF area = CreateArea(lines);
			IncreaseArea(area, verts);
			BlockMap<BlockEntry> blockmap = new BlockMap<BlockEntry>(area);
			blockmap.AddVerticesSet(verts);
			blockmap.AddLinedefsSet(lines);
			int bmWidth = blockmap.Size.Width;
			int bmHeight = blockmap.Size.Height;
			BlockEntry[,] bmap = blockmap.Map;

			//mxd
			HashSet<Vertex> splitverts = new HashSet<Vertex>();
			HashSet<Sector> changedsectors = (mergemode == MergeGeometryMode.REPLACE ? General.Map.Map.GetSectorsFromLinedefs(changedlines) : new HashSet<Sector>());
			HashSet<Vertex> lineverts = new HashSet<Vertex>();
			foreach(Linedef l in lines)
			{
				lineverts.Add(l.Start);
				lineverts.Add(l.End);
			}

			foreach (Vertex v in verts)
			{
				HashSet<BlockEntry> blocks;

				// If a vertex is in the very bottom left of it's block, and a linedef passes through the vertex it can
				// happen (depending on the linedef's direction) that the vertex and the linedef are not in the same
				// block. This will cause the linedef to not be split correctly. The prevent this we'll get all blocks
				// surrounding the vertex, since the linedef will definitely be in at least one of them. Otherwise
				// just take the block the vertex is in.
				if ((blockmap.Range.Left - (int)v.Position.x) % blockmap.BlockSize == 0 && (blockmap.Range.Bottom - (int)v.Position.y) % blockmap.BlockSize == 0)
				{
					blocks = blockmap.GetSquareRange(v.Position.x - 1, v.Position.y - 1, 2, 2);
				}
				else
				{
					blocks = new HashSet<BlockEntry>() { blockmap.GetBlockAt(v.Position) };
				}

				foreach (BlockEntry be in blocks)
				{
					if (be == null) continue;

					for (int i = 0; i < be.Lines.Count; i++)
					{
						Linedef l = be.Lines[i];

						// Check if v is close enough to l for splitting
						if (l.DistanceToSq(v.Position, true) <= splitdist2)
						{
							// Line is not already referencing v?
							Vector2D deltastart = l.Start.Position - v.Position;
							Vector2D deltaend = l.End.Position - v.Position;
							if (((Math.Abs(deltastart.x) > 0.001) || (Math.Abs(deltastart.y) > 0.001)) &&
								((Math.Abs(deltaend.x) > 0.001) || (Math.Abs(deltaend.y) > 0.001)))
							{
								// Split line l with vertex v
								Linedef nl = l.Split(v);
								if (nl == null) return false;
								v.Marked = true; //mxd
								splitverts.Add(v); //mxd

								// Add the new line to the list
								lines.Add(nl);
								blockmap.AddLinedef(nl);

								// Both lines must be updated because their new length is relevant for next iterations!
								l.UpdateCache();
								nl.UpdateCache();

								// Add both lines to changedlines
								if (changedlines != null)
								{
									changedlines.Add(l);
									changedlines.Add(nl);
								}
							}
						}
					}
				}
			}

			//mxd. Remove lines, which are inside affected sectors
			if(mergemode == MergeGeometryMode.REPLACE && changedsectors.Count > 0)
			{
				HashSet<Linedef> alllines = new HashSet<Linedef>(lines);
				if(changedlines != null) alllines.UnionWith(changedlines);

				foreach(Linedef l in alllines) l.UpdateCache();
				foreach(Sector s in changedsectors) s.UpdateBBox();
				foreach(Linedef l in alllines)
				{
					// Remove line when it's start, center and end are inside a changed sector and neither side references it
					if(l.Start != null && l.End != null &&
					  (l.Front == null || !changedsectors.Contains(l.Front.Sector)) &&
					  (l.Back == null || !changedsectors.Contains(l.Back.Sector)))
					{
						foreach(Sector s in changedsectors)
						{
							if(s.Intersect(l.Start.Position) && s.Intersect(l.End.Position) && s.Intersect(l.GetCenterPoint()))
							{
								Vertex[] tocheck = { l.Start, l.End };
								while(lines.Remove(l));
								if(changedlines != null) while(changedlines.Remove(l));
								l.Dispose();

								foreach(Vertex v in tocheck)
								{
									// If the newly created vertex only has 2 linedefs attached, then merge the linedefs
									if(!v.IsDisposed && v.Linedefs.Count == 2 && splitverts.Contains(v))
									{
										Linedef ld1 = General.GetByIndex(v.Linedefs, 0);
										Linedef ld2 = General.GetByIndex(v.Linedefs, 1);
										if(!ld1.Marked && !ld2.Marked)
										{
											Vertex v2 = (ld2.Start == v) ? ld2.End : ld2.Start;
											if(ld1.Start == v) ld1.SetStartVertex(v2); else ld1.SetEndVertex(v2);
											while(lines.Remove(ld2));
											if(changedlines != null) while(changedlines.Remove(ld2));
											ld2.Dispose();

											// Trash vertex
											v.Dispose();
                                        }
                                    }
								}

								break;
							}
						}
					}
				}
			}

            // [ZZ] note: disposing a vertex means also disposing all attached linedefs!
            //      we need to iterate through our lines collection and make sure no disposed linedefs exist there.
            //      also, just in case, do it for vertices as well, because vertices can be chain-disposed.
            foreach (Linedef line in lines.Where(line => line.IsDisposed).ToList())
                while (lines.Remove(line));
            foreach (Vertex vert in verts.Where(vert => vert.IsDisposed).ToList())
                while (verts.Remove(vert));

            return true;
		}

		/// <summary>Splits lines by lines. Adds new lines to the second collection. Returns false when the operation failed.</summary>
		public static bool SplitLinesByLines(ICollection<Linedef> lines, HashSet<Linedef> changedlines, MergeGeometryMode mergemode) //mxd
		{
			if(lines.Count == 0 || changedlines.Count == 0 || mergemode == MergeGeometryMode.CLASSIC) return true;
			
			// Create blockmap
			HashSet<Vertex> verts = new HashSet<Vertex>(); //mxd
			foreach(Linedef l in lines)
			{
				verts.Add(l.Start);
				verts.Add(l.End);
			}
			foreach(Linedef l in changedlines)
			{
				verts.Add(l.Start);
				verts.Add(l.End);
			}

			RectangleF area = RectangleF.Union(CreateArea(lines), CreateArea(changedlines));
			BlockMap<BlockEntry> blockmap = new BlockMap<BlockEntry>(area);
			blockmap.AddLinedefsSet(lines);
			blockmap.AddLinedefsSet(changedlines);
			blockmap.AddVerticesSet(verts); //mxd
			int bmWidth = blockmap.Size.Width;
			int bmHeight = blockmap.Size.Height;
			BlockEntry[,] bmap = blockmap.Map;

			//mxd
			HashSet<Vertex> splitverts = new HashSet<Vertex>();
			HashSet<Sector> changedsectors = (mergemode == MergeGeometryMode.REPLACE ? General.Map.Map.GetSectorsFromLinedefs(changedlines) : new HashSet<Sector>());

			// Check for intersections
			for(int w = 0; w < bmWidth; w++)
			{
				for(int h = 0; h < bmHeight; h++)
				{
					BlockEntry block = bmap[w, h];
					if(block.Lines.Count == 0) continue;

					for(int i = 0; i < block.Lines.Count; i++)
					{
						Linedef l1 = block.Lines[i];
						for(int c = 0; c < block.Lines.Count; c++)
						{
							if(i == c) continue;

							Linedef l2 = block.Lines[c];
							if(l1 == l2 
								|| l1.Start.Position == l2.Start.Position
								|| l1.Start.Position == l2.End.Position
								|| l1.End.Position == l2.Start.Position
								|| l1.End.Position == l2.End.Position) continue;

							// Check for intersection
							Vector2D intersection = Line2D.GetIntersectionPoint(new Line2D(l1), new Line2D(l2), true);
							if(!double.IsNaN(intersection.x))
							{
								//mxd. Round to map format precision
								intersection.x = Math.Round(intersection.x, General.Map.FormatInterface.VertexDecimals);
								intersection.y = Math.Round(intersection.y, General.Map.FormatInterface.VertexDecimals);

								//mxd. Skip when intersection matches start/end position.
								// Otherwise infinite ammount of 0-length lines will be created...
								if( l1.Start.Position == intersection || l1.End.Position == intersection ||
									l2.Start.Position == intersection || l2.End.Position == intersection) continue;

								//mxd. Do we already have a vertex here?
								bool existingvert = false;
								Vertex splitvertex = null;
								foreach(Vertex v in block.Vertices)
								{
									if(v.Position == intersection)
									{
										splitvertex = v;
										existingvert = true;
										break;
									}
								}

								//mxd. Create split vertex?
								if(splitvertex == null) splitvertex = General.Map.Map.CreateVertex(intersection);
								if(splitvertex == null) return false;

								// Split both lines
								Linedef nl1 = l1.Split(splitvertex);
								if(nl1 == null) return false;

								Linedef nl2 = l2.Split(splitvertex);
								if(nl2 == null) return false;

								// Mark split vertex?
								if(!existingvert)
								{
									splitvertex.Marked = true;
									splitverts.Add(splitvertex); //mxd
								}

								// Add to the second collection
								changedlines.Add(nl1);
								changedlines.Add(nl2);

								// And to the block entry
								blockmap.AddLinedef(nl1);
								blockmap.AddLinedef(nl2);
							}
						}
					}
				}
			}

			//mxd. Remove lines, which are inside affected sectors
			if(mergemode == MergeGeometryMode.REPLACE)
			{
				HashSet<Linedef> alllines = new HashSet<Linedef>(lines);
				alllines.UnionWith(changedlines);

				foreach(Linedef l in alllines) l.UpdateCache();
				foreach(Sector s in changedsectors) s.UpdateBBox();
				foreach(Linedef l in alllines)
				{
					// Remove line when it's start, center and end are inside a changed sector and neither side references it
					if(l.Start != null && l.End != null 
						&& (l.Front == null || !changedsectors.Contains(l.Front.Sector)) 
						&& (l.Back == null || !changedsectors.Contains(l.Back.Sector)))
					{
						foreach(Sector s in changedsectors)
						{
							if(s.Intersect(l.Start.Position) && s.Intersect(l.End.Position) && s.Intersect(l.GetCenterPoint()))
							{
								Vertex[] tocheck = { l.Start, l.End };
								l.Dispose();

								foreach(Vertex v in tocheck)
								{
									// If the newly created vertex only has 2 linedefs attached, then merge the linedefs
									if(!v.IsDisposed && v.Linedefs.Count == 2 && splitverts.Contains(v))
									{
										Linedef ld1 = General.GetByIndex(v.Linedefs, 0);
										Linedef ld2 = General.GetByIndex(v.Linedefs, 1);
										Vertex v2 = (ld2.Start == v) ? ld2.End : ld2.Start;
										if(ld1.Start == v) ld1.SetStartVertex(v2); else ld1.SetEndVertex(v2);
										ld2.Dispose();

										// Trash vertex
										v.Dispose();
									}
								}

								break;
							}
						}
					}
				}
			}

			return true;
		}

		/// <summary>This finds the side closest to the specified position.</summary>
		public static Sidedef NearestSidedef(ICollection<Sidedef> selection, Vector2D pos)
		{
			Sidedef closest = null;
			double distance = double.MaxValue;
			
			// Go for all sidedefs in selection
			foreach(Sidedef sd in selection)
			{
				// Calculate distance and check if closer than previous find
				double d = sd.Line.SafeDistanceToSq(pos, true);
				if(d == distance)
				{
					// Same distance, so only pick the one that is on the right side of the line
					double side = sd.Line.SideOfLine(pos);
					if(((side <= 0.0f) && sd.IsFront) || ((side > 0.0f) && !sd.IsFront))
					{
						closest = sd;
						distance = d;
					}
				}
				else if(d < distance)
				{
					// This one is closer
					closest = sd;
					distance = d;
				}
			}
			
			// Return result
			return closest;
		}

		/// <summary>This finds the line closest to the specified position.</summary>
		public static Linedef NearestLinedef(BlockMap<BlockEntry> selectionmap, Vector2D pos) //mxd
		{
			Linedef closest = null;
			double distance = double.MaxValue;

			Point p = selectionmap.GetBlockCoordinates(pos);
			int minx = p.X;
			int maxx = p.X;
			int miny = p.Y;
			int maxy = p.Y;
			int step = 0;

			// Check square block ranges around pos...
			while(true)
			{
				bool noblocksfound = true;
				for(int x = minx; x < maxx + 1; x++)
				{
					for(int y = miny; y < maxy + 1; y++)
					{
						// Skip inner blocks...
						if(x > minx && x < maxx && y > miny && y < maxy) continue;
						if(!selectionmap.IsInRange(new Point(x, y))) continue;

						// Go for all linedefs in block
						BlockEntry be = selectionmap.Map[x, y];
						foreach(Linedef l in be.Lines)
						{
							// Calculate distance and check if closer than previous find
							double d = l.SafeDistanceToSq(pos, true);
							if(d < distance)
							{
								// This one is closer
								closest = l;
								distance = d;
							}
						}

						noblocksfound = false;
					}
				}

				// Abort if line was found or when outside of blockmap range...
				// Check at least 3x3 blocks, because there's a possibility that a line closer to pos exists in a nearby block than in the first block
				if(noblocksfound || (closest != null && step > 0)) return closest;

				// Increase search range...
				minx--;
				maxx++;
				miny--;
				maxy++;
				step++;
			}
		}

		/// <summary>This finds the line closest to the specified position.</summary>
		public static Linedef NearestLinedef(ICollection<Linedef> selection, Vector2D pos)
		{
			Linedef closest = null;
			double distance = double.MaxValue;

			// Go for all linedefs in selection
			foreach(Linedef l in selection)
			{
				// Calculate distance and check if closer than previous find
				double d = l.SafeDistanceToSq(pos, true);
				if(d < distance)
				{
					// This one is closer
					closest = l;
					distance = d;
				}
			}

			// Return result
			return closest;
		}

		/// <summary>This finds the line closest to the specified position.</summary>
		public static Linedef NearestLinedefRange(ICollection<Linedef> selection, Vector2D pos, double maxrange)
		{
			Linedef closest = null;
			double distance = float.MaxValue;
			double maxrangesq = maxrange * maxrange;

			// Go for all linedefs in selection
			foreach(Linedef l in selection)
			{
				// Calculate distance and check if closer than previous find
				double d = l.SafeDistanceToSq(pos, true);
				if(d < distance && d <= maxrangesq)
				{
					// This one is closer
					closest = l;
					distance = d;
				}
			}
			
			// Return result
			return closest;
		}

		/// <summary>This finds the line closest to the specified position.</summary>
		public static Linedef NearestLinedefRange(BlockMap<BlockEntry> selectionmap, Vector2D pos, double maxrange) //mxd
		{
			if (selectionmap == null)
				return null;

			Linedef closest = null;
			double distance = double.MaxValue;
			double maxrangesq = maxrange * maxrange;
			HashSet<Linedef> processed = new HashSet<Linedef>();

			HashSet<BlockEntry> blocks;

			// If the max range is lower or equal to the blockmap's block size we can take a shortcut got the the possible blocks.
			// Otherwise get an rectangular range of blocks. This happens when maxrange is computed from low zoom levels
			if (maxrange <= selectionmap.BlockSize)
			{
				blocks = new HashSet<BlockEntry>
				{
					selectionmap.GetBlockAt(pos),
					selectionmap.GetBlockAt(new Vector2D(pos.x + maxrange, pos.y + maxrange)),
					selectionmap.GetBlockAt(new Vector2D(pos.x + maxrange, pos.y - maxrange)),
					selectionmap.GetBlockAt(new Vector2D(pos.x - maxrange, pos.y + maxrange)),
					selectionmap.GetBlockAt(new Vector2D(pos.x - maxrange, pos.y - maxrange))
				};
			}
			else
			{
				blocks = selectionmap.GetSquareRange(pos.x - maxrange, pos.y - maxrange, maxrange * 2, maxrange * 2);
			}

			foreach(BlockEntry be in blocks)
			{
				if(be == null) continue;

				foreach(Linedef l in be.Lines)
				{
					if(processed.Contains(l)) continue;

					// Calculate distance and check if closer than previous find
					double d = l.SafeDistanceToSq(pos, true);
					if(d < distance && d <= maxrangesq)
					{
						// This one is closer
						closest = l;
						distance = d;
					}

					processed.Add(l);
				}
			}

			// Return result
			return closest;
		}

		/// <summary>mxd. This finds the line closest to the specified position excluding given list of linedefs.</summary>
		public Linedef NearestLinedef(Vector2D pos, HashSet<Linedef> linesToExclude) 
		{
			Linedef closest = null;
			double distance = double.MaxValue;

			// Go for all linedefs in selection
			foreach(Linedef l in linedefs) 
			{
				if(linesToExclude.Contains(l)) continue;
				// Calculate distance and check if closer than previous find
				double d = l.SafeDistanceToSq(pos, true);
				if(d < distance) 
				{
					// This one is closer
					closest = l;
					distance = d;
				}
			}

			// Return result
			return closest;
		}

		/// <summary>This finds the vertex closest to the specified position.</summary>
		public static Vertex NearestVertex(ICollection<Vertex> selection, Vector2D pos)
		{
			Vertex closest = null;
			double distance = double.MaxValue;

			// Go for all vertices in selection
			foreach(Vertex v in selection)
			{
				// Calculate distance and check if closer than previous find
				double d = v.DistanceToSq(pos);
				if(d < distance)
				{
					// This one is closer
					closest = v;
					distance = d;
				}
			}

			// Return result
			return closest;
		}

		/// <summary>This finds the thing closest to the specified position.</summary>
		public static Thing NearestThing(ICollection<Thing> selection, Vector2D pos)
		{
			Thing closest = null;
			double distance = double.MaxValue;

			// Go for all things in selection
			foreach(Thing t in selection)
			{
				// Calculate distance and check if closer than previous find
				double d = t.DistanceToSq(pos);
				if(d < distance)
				{
					// This one is closer
					closest = t;
					distance = d;
				}
			}

			// Return result
			return closest;
		}

		/// <summary>mxd. This finds the thing closest to the specified thing.</summary>
		public static Thing NearestThing(ICollection<Thing> selection, Thing thing) 
		{
			Thing closest = null;
			double distance = double.MaxValue;

			// Go for all things in selection
			foreach(Thing t in selection) 
			{
				if(t == thing) continue;

				// Calculate distance and check if closer than previous find
				double d = t.DistanceToSq(thing.Position);
				if(d < distance) 
				{
					// This one is closer
					closest = t;
					distance = d;
				}
			}

			// Return result
			return closest;
		}

		/// <summary>This finds the vertex closest to the specified position.</summary>
		public static Vertex NearestVertexSquareRange(ICollection<Vertex> selection, Vector2D pos, double maxrange)
		{
			RectangleF range = RectangleF.FromLTRB((float)(pos.x - maxrange), (float)(pos.y - maxrange), (float)(pos.x + maxrange), (float)(pos.y + maxrange));
			Vertex closest = null;
			double distance = double.MaxValue;

			// Go for all vertices in selection
			foreach(Vertex v in selection)
			{
				double px = v.Position.x;
				double py = v.Position.y;
				
				//mxd. Within range?
				if((v.Position.x < range.Left) || (v.Position.x > range.Right) 
					|| (v.Position.y < range.Top) || (v.Position.y > range.Bottom))
					continue;

				// Close than previous find?
				double d = Math.Abs(px - pos.x) + Math.Abs(py - pos.y);
				if(d < distance) 
				{
					// This one is closer
					closest = v;
					distance = d;
				}
			}

			// Return result
			return closest;
		}

		/// <summary>This finds the thing closest to the specified position.</summary>
		public static Thing NearestThingSquareRange(ICollection<Thing> selection, Vector2D pos, double maxrange)
		{
			RectangleF range = RectangleF.FromLTRB((float)(pos.x - maxrange), (float)(pos.y - maxrange), (float)(pos.x + maxrange), (float)(pos.y + maxrange));
			Thing closest = null;
			double distance = double.MaxValue;
			double size = double.MaxValue; //mxd

			// Go for all things in selection
			foreach(Thing t in selection)
			{
				double px = t.Position.x;
				double py = t.Position.y;

				//mxd. Determine displayed size
				double ts;
				if(t.FixedSize && General.Map.Renderer2D.Scale > 1.0f)
					ts = t.Size / General.Map.Renderer2D.Scale;
				else if(General.Settings.FixedThingsScale && t.Size * General.Map.Renderer2D.Scale > Renderer2D.FIXED_THING_SIZE)
					ts = Renderer2D.FIXED_THING_SIZE / General.Map.Renderer2D.Scale;
				else
					ts = t.Size;

				//mxd. Within range?
				if(px < range.Left - ts || px > range.Right + ts || py < range.Top - ts || py > range.Bottom + ts) continue;

				// Closer than previous find? mxd. Or smaller when distance is the same?
				double d = Math.Abs(px - pos.x) + Math.Abs(py - pos.y);
				if(d < distance || (d == distance && ts < size))
				{
					// This one is closer
					closest = t;
					distance = d;
					size = ts; //mxd
				}
			}

			// Return result
			return closest;
		}

		#endregion

		#region ================== Tools

		/// <summary>This snaps all vertices to the map format accuracy. Call this to ensure the vertices are at valid coordinates.</summary>
		public void SnapAllToAccuracy()
		{
			SnapAllToAccuracy(true);
		}

		/// <summary>This snaps all vertices to the map format accuracy. Call this to ensure the vertices are at valid coordinates.</summary>
		public void SnapAllToAccuracy(bool usepreciseposition)
		{
			foreach(Vertex v in vertices) v.SnapToAccuracy(usepreciseposition);
			foreach(Thing t in things) t.SnapToAccuracy(usepreciseposition);
		}

		/// <summary>This returns the next unused tag number.</summary>
		public int GetNewTag()
		{
			Dictionary<int, bool> usedtags = new Dictionary<int, bool>();
			ForAllTags(NewTagHandler, false, usedtags);
			ForAllTags(NewTagHandler, true, usedtags);
			
			// Now find the first unused index
			for(int i = 1; i <= General.Map.FormatInterface.MaxTag; i++)
				if(!usedtags.ContainsKey(i)) return i;
			
			// All tags used!
			return 0;
		}

		//mxd
		/// <summary>This returns the next unused tag number.</summary>
		public int GetNewTag(List<int> moreusedtags)
		{
			Dictionary<int, bool> usedtags = new Dictionary<int, bool>();
			foreach(int t in moreusedtags) if(!usedtags.ContainsKey(t)) usedtags.Add(t, true); 
			ForAllTags(NewTagHandler, false, usedtags);
			ForAllTags(NewTagHandler, true, usedtags);

			// Now find the first unused index
			for(int i = 1; i <= General.Map.FormatInterface.MaxTag; i++)
				if(!usedtags.ContainsKey(i)) return i;

			// All tags used!
			return 0;
		}

		//mxd
		/// <summary>This returns the tag number, which is not used by any map element of given type. This method doesn't check action arguments!</summary>
		public int GetNewTag(UniversalType elementType) 
		{
			Dictionary<int, bool> usedtags = new Dictionary<int, bool>();

			switch(elementType) 
			{
				case UniversalType.ThingTag:
					for(int i = 0; i < things.Length; i++) 
					{
						if(things[i].Tag > 0 && !usedtags.ContainsKey(things[i].Tag))
							usedtags.Add(things[i].Tag, false);
					}
					break;

				case UniversalType.LinedefTag:
					for(int i = 0; i < linedefs.Length; i++) 
					{
						foreach(int tag in linedefs[i].Tags)
						{
							if(tag == 0) continue;
							if(!usedtags.ContainsKey(tag)) usedtags.Add(tag, false);
						}
					}
					break;

				case UniversalType.SectorTag:
					for(int i = 0; i < sectors.Length; i++) 
					{
						foreach(int tag in sectors[i].Tags)
						{
							if(tag == 0) continue;
							if(!usedtags.ContainsKey(tag)) usedtags.Add(tag, false);
						}
					}
					break;
			}

			// Now find the first unused index
			for(int i = 1; i <= General.Map.FormatInterface.MaxTag; i++)
				if(!usedtags.ContainsKey(i)) return i;
			
			// All tags used!
			return 0;
		}

		/// <summary>This returns the next unused tag number within the marked geometry.</summary>
		public int GetNewTag(bool marked)
		{
			Dictionary<int, bool> usedtags = new Dictionary<int, bool>();
			ForAllTags(NewTagHandler, marked, usedtags);

			// Now find the first unused index
			for(int i = 1; i <= General.Map.FormatInterface.MaxTag; i++)
				if(!usedtags.ContainsKey(i)) return i;

			// All tags used!
			return 0;
		}

		/// <summary>This returns the next unused tag number.</summary>
		public List<int> GetMultipleNewTags(int count)
		{
			List<int> newtags = new List<int>(count);
			if(count > 0)
			{
				Dictionary<int, bool> usedtags = new Dictionary<int, bool>();
				ForAllTags(NewTagHandler, false, usedtags);
				ForAllTags(NewTagHandler, true, usedtags);
				
				// Find unused tags and add them
				for(int i = 1; i <= General.Map.FormatInterface.MaxTag; i++)
				{
					if(!usedtags.ContainsKey(i))
					{
						newtags.Add(i);
						if(newtags.Count == count) break;
					}
				}
			}
			
			return newtags;
		}

		/// <summary>This returns the next unused tag number within the marked geometry.</summary>
		public List<int> GetMultipleNewTags(int count, bool marked)
		{
			List<int> newtags = new List<int>(count);
			if(count > 0)
			{
				Dictionary<int, bool> usedtags = new Dictionary<int, bool>();
				ForAllTags(NewTagHandler, marked, usedtags);

				// Find unused tags and add them
				for(int i = 1; i <= General.Map.FormatInterface.MaxTag; i++)
				{
					if(!usedtags.ContainsKey(i))
					{
						newtags.Add(i);
						if(newtags.Count == count) break;
					}
				}
			}
			
			return newtags;
		}

		// Handler for finding a new tag
		private static void NewTagHandler(MapElement element, bool actionargument, UniversalType type, ref int value, Dictionary<int, bool> usedtags)
		{
			usedtags[value] = true;
		}

		/// <summary>This calls a function for all tag fields in the marked or unmarked geometry. The obj parameter can be anything you wish to pass on to your TagHandler function.</summary>
		public void ForAllTags<T>(TagHandler<T> handler, bool marked, T obj)
		{
			// Call handler on sectors tags
			foreach(Sector s in sectors)
			{
				if(s.Marked == marked)
				{
					//mxd. Multiple tags support...
					bool changed = false;
					// Make a copy of tags, otherwise BeforePropsChange will be triggered after tag changes
					List<int> tags = new List<int>(s.Tags);
					for(int i = 0; i < tags.Count; i++)
					{
						int tag = tags[i];
						handler(s, false, UniversalType.SectorTag, ref tag, obj);
						if(tag != tags[i])
						{
							tags[i] = tag;
							changed = true;
						}
					}

					if(changed) s.Tags = tags.Distinct().ToList();
				}
			}

			// Call handler on things tags
			if(General.Map.FormatInterface.HasThingTag)
			{
				foreach(Thing t in things)
				{
					if(t.Marked == marked)
					{
						int tag = t.Tag;
						handler(t, false, UniversalType.ThingTag, ref tag, obj);
						if(tag != t.Tag) t.Tag = tag;
					}
				}
			}

			// Call handler on things action
			if(General.Map.FormatInterface.HasThingAction && General.Map.FormatInterface.HasActionArgs)
			{
				foreach(Thing t in things)
				{
					if(t.Marked == marked)
					{
						LinedefActionInfo info = General.Map.Config.GetLinedefActionInfo(t.Action);
						for(int i = 0; i < Thing.NUM_ARGS; i++)
						{
							if(info.Args[i].Used && CheckIsTagType(info.Args[i].Type))
							{
								int tag = t.Args[i];
								handler(t, true, (UniversalType)(info.Args[i].Type), ref tag, obj);
								if(tag != t.Args[i]) t.Args[i] = tag;
							}
						}
					}
				}
			}

			// Call handler on linedefs tags
			if(General.Map.FormatInterface.HasLinedefTag)
			{
				foreach(Linedef l in linedefs)
				{
					if(l.Marked == marked)
					{
						//mxd. Multiple tags support...
						bool changed = false;
						// Make a copy of tags, otherwise BeforePropsChange will be triggered after tag changes
						List<int> tags = new List<int>(l.Tags);
						for(int i = 0; i < tags.Count; i++)
						{
							int tag = tags[i];
							handler(l, false, UniversalType.LinedefTag, ref tag, obj);
							if(tag != tags[i])
							{
								tags[i] = tag;
								changed = true;
							}
						}

						if(changed) l.Tags = tags.Distinct().ToList();
					}
				}
			}

			// Call handler on linedefs action
			if(General.Map.FormatInterface.HasActionArgs)
			{
				foreach(Linedef l in linedefs)
				{
					if(l.Marked == marked)
					{
						LinedefActionInfo info = General.Map.Config.GetLinedefActionInfo(l.Action);
						for(int i = 0; i < Linedef.NUM_ARGS; i++)
						{
							if(info.Args[i].Used && CheckIsTagType(info.Args[i].Type))
							{
								int tag = l.Args[i];
								handler(l, true, (UniversalType)(info.Args[i].Type), ref tag, obj);
								if(tag != l.Args[i]) l.Args[i] = tag;
							}
						}
					}
				}
			}
		}
		
		// This checks if the given action argument type is a tag type
		private static bool CheckIsTagType(int argtype)
		{
			return (argtype == (int)UniversalType.LinedefTag) ||
				   (argtype == (int)UniversalType.SectorTag) ||
				   (argtype == (int)UniversalType.ThingTag);
		}
		
		/// <summary>This makes a list of lines related to marked vertices.
		/// A line is unstable when one vertex is marked and the other isn't.</summary>
		public List<Linedef> LinedefsFromMarkedVertices(bool includeunmarked, bool includestable, bool includeunstable)
		{
			List<Linedef> list = new List<Linedef>((numlinedefs / 2) + 1);
			
			// Go for all lines
			foreach(Linedef l in linedefs)
			{
				// Check if this is to be included
				if((includestable && (l.Start.Marked && l.End.Marked)) ||
				   (includeunstable && (l.Start.Marked ^ l.End.Marked)) ||
				   (includeunmarked && (!l.Start.Marked && !l.End.Marked)))
				{
					// Add to list
					list.Add(l);
				}
			}

			// Return result
			return list;
		}

		/// <summary>This makes a list of unstable lines from the given vertices.
		/// A line is unstable when one vertex is selected and the other isn't.</summary>
		public static ICollection<Linedef> UnstableLinedefsFromVertices(ICollection<Vertex> verts)
		{
			Dictionary<Linedef, Linedef> lines = new Dictionary<Linedef, Linedef>();

			// Go for all vertices
			foreach(Vertex v in verts)
			{
				// Go for all lines
				foreach(Linedef l in v.Linedefs)
				{
					// If the line exists in the list
					if(lines.ContainsKey(l))
					{
						// Remove it
						lines.Remove(l);
					}
					// Otherwise add it
					else
					{
						// Add the line
						lines.Add(l, l);
					}
				}
			}
			
			// Return result
			return new List<Linedef>(lines.Values);
		}

		//mxd
		/// <summary>This returns a sector if given coordinates are inside one.</summary>
		public Sector GetSectorByCoordinates(Vector2D pos) 
		{
			foreach(Sector s in sectors) 
			{
				if(s.Intersect(pos)) return s;
			}
			return null;
		}

		//mxd
		/// <summary>This returns a sector if given coordinates are inside one.</summary>
		public Sector GetSectorByCoordinates(Vector2D pos, VisualBlockMap blockmap) 
		{
            return blockmap.GetSectorAt(pos);
		}

		//mxd
		/// <summary>Gets unselected sectors, which have all their linedefs selected</summary>
		public HashSet<Sector> GetUnselectedSectorsFromLinedefs(IEnumerable<Linedef> lines)
		{
			HashSet<Sector> result = new HashSet<Sector>();
			Dictionary<Sector, HashSet<Sidedef>> sectorsbysides = new Dictionary<Sector, HashSet<Sidedef>>();
			HashSet<Sector> selectedsectors = new HashSet<Sector>(General.Map.Map.GetSelectedSectors(true));

			// Collect unselected sectors, which sidedefs belong to selected lines 
			foreach(Linedef line in lines)
			{
				if(line.Front != null && line.Front.Sector != null && !selectedsectors.Contains(line.Front.Sector))
				{
					if(!sectorsbysides.ContainsKey(line.Front.Sector)) sectorsbysides.Add(line.Front.Sector, new HashSet<Sidedef>());
					sectorsbysides[line.Front.Sector].Add(line.Front);
				}
				if(line.Back != null && line.Back.Sector != null && !selectedsectors.Contains(line.Back.Sector))
				{
					if(!sectorsbysides.ContainsKey(line.Back.Sector)) sectorsbysides.Add(line.Back.Sector, new HashSet<Sidedef>());
					sectorsbysides[line.Back.Sector].Add(line.Back);
				}
			}

			// Add sectors, which have all their lines selected
			foreach(var group in sectorsbysides)
			{
				if(group.Key.Sidedefs.Count == group.Value.Count) result.Add(group.Key);
			}

			return result;
		}

		//mxd
		/// <summary>Gets sectors, which have all their linedefs selected</summary>
		public HashSet<Sector> GetSectorsFromLinedefs(IEnumerable<Linedef> lines)
		{
			HashSet<Sector> result = new HashSet<Sector>();
			Dictionary<Sector, HashSet<Sidedef>> sectorsbysides = new Dictionary<Sector, HashSet<Sidedef>>();

			// Collect unselected sectors, which sidedefs belong to selected lines 
			foreach(Linedef line in lines)
			{
				if (line.Front != null && line.Front.Sector != null)
				{
					if(!sectorsbysides.ContainsKey(line.Front.Sector)) sectorsbysides.Add(line.Front.Sector, new HashSet<Sidedef>());
					sectorsbysides[line.Front.Sector].Add(line.Front);
				}
				if (line.Back != null && line.Back.Sector != null)
				{
					if(!sectorsbysides.ContainsKey(line.Back.Sector)) sectorsbysides.Add(line.Back.Sector, new HashSet<Sidedef>());
					sectorsbysides[line.Back.Sector].Add(line.Back);
				}
			}

			// Add sectors, which have all their lines selected
			foreach(var group in sectorsbysides)
			{
				if(group.Key.Sidedefs.Count == group.Value.Count) result.Add(group.Key);
			}

			return result;
		}

		/// <summary>This finds the line closest to the specified position.</summary>
		public Linedef NearestLinedef(Vector2D pos) { return MapSet.NearestLinedef(linedefs, pos); }

		/// <summary>This finds the line closest to the specified position.</summary>
		public Linedef NearestLinedefRange(Vector2D pos, double maxrange) { return MapSet.NearestLinedefRange(linedefs, pos, maxrange); }

		/// <summary>This finds the vertex closest to the specified position.</summary>
		public Vertex NearestVertex(Vector2D pos) { return MapSet.NearestVertex(vertices, pos); }

		/// <summary>This finds the vertex closest to the specified position.</summary>
		public Vertex NearestVertexSquareRange(Vector2D pos, double maxrange) { return MapSet.NearestVertexSquareRange(vertices, pos, maxrange); }

		/// <summary>This finds the thing closest to the specified position.</summary>
		public Thing NearestThingSquareRange(Vector2D pos, double maxrange) { return MapSet.NearestThingSquareRange(things, pos, maxrange); }

		/// <summary>This finds the closest unselected linedef that is not connected to the given vertex.</summary>
		public Linedef NearestUnselectedUnreferencedLinedef(Vector2D pos, double maxrange, Vertex v, out double distance)
		{
			Linedef closest = null;
			distance = double.MaxValue;
			double maxrangesq = maxrange * maxrange;

			// Go for all linedefs in selection
			foreach(Linedef l in linedefs)
			{
				// Calculate distance and check if closer than previous find
				double d = l.SafeDistanceToSq(pos, true);
				if((d <= maxrangesq) && (d < distance))
				{
					// Check if not selected

					// Check if linedef is not connected to v
					if((l.Start != v) && (l.End != v))
					{
						// This one is closer
						closest = l;
						distance = d;
					}
				}
			}

			// Return result
			return closest;
		}

		// This performs sidedefs compression
		// Note: Only use this for saving, because this messes up the expected data structure horribly.
		internal void CompressSidedefs()
		{
			Dictionary<uint, List<Sidedef>> storedsides = new Dictionary<uint, List<Sidedef>>(numsidedefs);
			int originalsidescount = numsidedefs;
			long starttime = Clock.CurrentTime;

			BeginAddRemove();
			
			int sn = 0;
			while(sn < numsidedefs)
			{
				Sidedef stored = null;
				Sidedef snsd = sidedefs[sn];

				//mxd. Skip sidedef if it belongs to a linedef with an action or tag?
				if(!General.Map.Config.SidedefCompressionIgnoresAction && (snsd.Line.Action != 0 || snsd.Line.Tag != 0))
				{
					// Next!
					sn++;
					continue;
				}

				// Check if checksum is stored
				bool samesidedef = false;
				uint checksum = snsd.GetChecksum();
				bool checksumstored = storedsides.ContainsKey(checksum);
				if(checksumstored)
				{
					List<Sidedef> othersides = storedsides[checksum];
					foreach(Sidedef os in othersides)
					{
						// They must be in the same sector
						if(snsd.Sector == os.Sector)
						{
							// Check if sidedefs are really the same
							stored = os;
							MemoryStream sidemem = new MemoryStream(1024);
							SerializerStream sidedata = new SerializerStream(sidemem);
							MemoryStream othermem = new MemoryStream(1024);
							SerializerStream otherdata = new SerializerStream(othermem);
							snsd.ReadWrite(sidedata);
							os.ReadWrite(otherdata);
							if(sidemem.Length == othermem.Length)
							{
								samesidedef = true;
								sidemem.Seek(0, SeekOrigin.Begin);
								othermem.Seek(0, SeekOrigin.Begin);
								for(int i = 0; i < sidemem.Length; i++)
								{
									if(sidemem.ReadByte() != othermem.ReadByte())
									{
										samesidedef = false;
										break;
									}
								}
							}

							if(samesidedef) break;
						}
					}
				}

				// Same sidedef?
				if(samesidedef)
				{
					// Replace with stored sidedef
					bool isfront = snsd.IsFront;
					Linedef ld = snsd.Line;
					snsd.Line.DetachSidedefP(snsd);
					if(isfront)
						ld.AttachFront(stored);
					else
						ld.AttachBack(stored);
					
					// Remove the sidedef
					snsd.SetSector(null);
					RemoveSidedef(sn);
				}
				else
				{
					// Store this new one
					if(checksumstored)
					{
						storedsides[checksum].Add(snsd);
					}
					else
					{
						List<Sidedef> newlist = new List<Sidedef>(4) {snsd};
						storedsides.Add(checksum, newlist);
					}
					
					// Next
					sn++;
				}
			}

			EndAddRemove();

			// Output info
			long endtime = Clock.CurrentTime;
			float deltatimesec = (endtime - starttime) / 1000.0f;
			float ratio = 100.0f - ((numsidedefs / (float)originalsidescount) * 100.0f);
			General.WriteLogLine("Sidedefs compressed: " + numsidedefs + " remaining out of " + originalsidescount + " (" + ratio.ToString("########0.00") + "%) in " + deltatimesec.ToString("########0.00") + " seconds");
		}

		// This converts flags and activations to UDMF fields
		internal void TranslateToUDMF(Type previousmapformatinterfacetype)
		{
			foreach(Linedef l in linedefs) l.TranslateToUDMF(previousmapformatinterfacetype);
			foreach(Thing t in things) t.TranslateToUDMF();
		}

		// This converts UDMF fields back into flags and activations
		// NOTE: Only converts the marked items
		internal void TranslateFromUDMF()
		{
			foreach(Linedef l in linedefs) if(l.Marked) l.TranslateFromUDMF();
			foreach(Sidedef s in sidedefs) if(s.Marked) s.TranslateFromUDMF(); //mxd
			foreach(Sector s in sectors) if(s.Marked) s.TranslateFromUDMF(); //mxd
			foreach(Thing t in things) if(t.Marked) t.TranslateFromUDMF();
		}

		/// <summary>This removes unused vertices.</summary>
		public void RemoveUnusedVertices()
		{
			// Go for all vertices
			int index = numvertices - 1;
			while(index >= 0)
			{
				if((vertices[index] != null) && (vertices[index].Linedefs.Count == 0))
					vertices[index].Dispose();
				else
					index--;
			}
		}

		//mxd
		public void UpdateCustomLinedefColors() 
		{
			foreach(Linedef l in linedefs) l.UpdateColorPreset();
		}
		
		#endregion
	}
}
