#region ================== Copyright (c) 2007 Pascal vd Heiden

/*
 * Copyright (c) 2007 Pascal vd Heiden, www.codeimp.com
 * Copyright (c) 2019 Boris Iwanski
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

using CodeImp.DoomBuilder.Map;
using System.Collections.Generic;
using System;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	public abstract class BaseCheckTextures : ErrorChecker
	{
		#region ================== Constants

		private const int PROGRESS_STEP = 1000;

		#endregion

		#region ================== Variables

		protected Dictionary<int, Flags3DFloor> sector3dfloors;
		protected ActionFloorLowerToLowestTextures floorlowertolowest;
		protected ActionFloorRaiseToNextHigherTextures floorraisetonexthigher;
		protected ActionFloorRaiseToHighestTextures floorraisetohighest;

		#endregion

		#region ================== Constructor / Destructor

		// Constructor
		public BaseCheckTextures()
		{
			// Total progress is done when all lines are checked
			SetTotalProgress(General.Map.Map.Sidedefs.Count / PROGRESS_STEP);

			sector3dfloors = new Dictionary<int, Flags3DFloor>();
			floorlowertolowest = new ActionFloorLowerToLowestTextures();
			floorraisetonexthigher = new ActionFloorRaiseToNextHigherTextures();
			floorraisetohighest = new ActionFloorRaiseToHighestTextures();
		}

		#endregion

		#region ================== Enum

		[Flags]
		protected enum Flags3DFloor
		{
			UseUpper = 1,
			UseLower = 2,
			RenderInside = 4
		}

		#endregion

		#region ================== Methods

		// Create a cache of sectors that have 3D floors, with their flags relevant to the error checker
		protected void Build3DFloorCache()
		{
			// Skip if linedef action 160 isn't the ZDoom 3D floor special.
			if (General.Map.Config.GetLinedefActionInfo(160).Id != "Sector_Set3dFloor")
			{
				return;
			}

			foreach (Linedef ld in General.Map.Map.Linedefs)
			{
				if (ld.Action == 160)
				{
					if ((ld.Args[1] & 4) == 4) // Type render inside
					{
						if (!sector3dfloors.ContainsKey(ld.Args[0]))
							sector3dfloors.Add(ld.Args[0], Flags3DFloor.RenderInside);
					}

					if ((ld.Args[2] & 16) == 16) // Flag use upper
					{
						if (!sector3dfloors.ContainsKey(ld.Args[0]))
							sector3dfloors.Add(ld.Args[0], Flags3DFloor.UseUpper);
						else
							sector3dfloors[ld.Args[0]] |= Flags3DFloor.UseUpper;
					}

					if ((ld.Args[2] & 32) == 32) // Flag use lower
					{
						if (!sector3dfloors.ContainsKey(ld.Args[0]))
							sector3dfloors.Add(ld.Args[0], Flags3DFloor.UseLower);
						else
							sector3dfloors[ld.Args[0]] |= Flags3DFloor.UseLower;
					}
				}
			}
		}

		#endregion
	}
}
