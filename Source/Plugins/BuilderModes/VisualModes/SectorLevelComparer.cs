#region === Copyright (c) 2010 Pascal van der Heiden ===

using System;
using System.Collections.Generic;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Map;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	internal class SectorLevelComparer : IComparer<SectorLevel>
	{
		// Center of sector to use for plane comparison
		public Vector2D center;
		
		// Constructor
		public SectorLevelComparer(Sector s)
		{
			this.center = new Vector2D(s.BBox.Left + s.BBox.Width / 2, s.BBox.Top + s.BBox.Height / 2);
		}

		// Comparer. -1 = x is less than y.
		public int Compare(SectorLevel x, SectorLevel y)
		{
			if(x == y) return 0; //mxd

			//mxd. Handle surfaces with the same height
			double diff = Math.Round(x.plane.GetZ(center) - y.plane.GetZ(center), 3);
			if(diff == 0)
			{
				bool xislight = (x.type == SectorLevelType.Light || x.type == SectorLevelType.Glow);
				bool yislight = (y.type == SectorLevelType.Light || y.type == SectorLevelType.Glow);

				// Compare regular and extrafloors
				if(!xislight && ! yislight && x.lighttype == LightLevelType.UNKNOWN && y.lighttype == LightLevelType.UNKNOWN)
				{
					// Both are 3d floors. Push extrafloors above extraceilings
					if(x.extrafloor != null && y.extrafloor != null)
					{
						if(x.type == SectorLevelType.Floor) return (y.type == SectorLevelType.Ceiling ? 1 : 0);
						return (y.type == SectorLevelType.Floor ? -1 : 0);
					}

					// None is 3d floor. Push ceilings above floors
					if(x.extrafloor == null && y.extrafloor == null)
					{
						if(x.type == SectorLevelType.Floor) return (y.type == SectorLevelType.Ceiling ? -1 : 0);
						return (y.type == SectorLevelType.Floor ? 1 : 0);
					}

					// One is 3d floor. Push it below the regular surface if it has "disablelighting" flag, and above otherwise
					return ((x.extrafloor != null && x.disablelighting) || (y.extrafloor != null && !y.disablelighting) ? -1 : 1);
				}
				
				// Push light levels above floor and ceiling levels when height is the same
				if(!xislight) return (yislight ? -1 : 0);
				if(!yislight) return 1;

				// Push light levels without lighttype (those should be lower levels of type 1 Transfer Brightness effect) above other ones
				if(x.lighttype == y.lighttype) return 0; //TODO: how this should be handled?
				if(x.lighttype == LightLevelType.TYPE1_BOTTOM) return 1;
				return -1;
			}

			return Math.Sign(diff);
		}
	}
}
