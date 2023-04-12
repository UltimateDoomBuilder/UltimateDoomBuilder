
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

using System.Drawing;

#endregion

namespace CodeImp.DoomBuilder.Rendering
{
	// This is an entry is the surface manager and contains the information
	// needed for a sector to place it's ceiling and floor surface geometry
	// in a vertexbuffer. Sectors keep a reference to this entry to tell the
	// surface manager to remove them if needed.
	internal class SurfaceEntry
	{
		// Number of vertices in the geometry and index of the buffer
		// This tells the surface manager which vertexbuffer this is in.
		public int numvertices;
		public int bufferindex;
		
		// Bounding box for fast culling
		public RectangleF bbox;
		
		// Offset in the buffer (in number of vertices)
		public int vertexoffset;
		
		// Sector geometry (local copy used to quickly refill buffers)
		// The sector must set these!
		public FlatVertex[] floorvertices;
		public FlatVertex[] ceilvertices;
		
		// Sector images
		// The sector must set these!
		public long floortexture;
		public long ceiltexture;

		// Sector flags
		public bool hidden; // sector is hidden on textured automap

        //
        public double desaturation;
		
		// Constructor
		internal SurfaceEntry(int numvertices, int bufferindex, int vertexoffset)
		{
			this.numvertices = numvertices;
			this.bufferindex = bufferindex;
			this.vertexoffset = vertexoffset;
		}

		// Constructor that copies the entry, but does not copy the vertices
		internal SurfaceEntry(SurfaceEntry oldentry)
		{
			this.numvertices = oldentry.numvertices;
			this.bufferindex = oldentry.bufferindex;
			this.vertexoffset = oldentry.vertexoffset;
		}

		// This calculates the bounding box from the vertices
		public void UpdateBBox()
		{
			float left = float.MaxValue;
			float right = float.MinValue;
			float top = float.MaxValue;
			float bottom = float.MinValue;
			
			for(int i = 0; i < floorvertices.Length; i++)
			{
				if(floorvertices[i].x < left) left = floorvertices[i].x;
				if(floorvertices[i].x > right) right = floorvertices[i].x;
				if(floorvertices[i].y < top) top = floorvertices[i].y;
				if(floorvertices[i].y > bottom) bottom = floorvertices[i].y;
			}

			bbox = new RectangleF(left, top, right - left, bottom - top);
		}
	}
}
