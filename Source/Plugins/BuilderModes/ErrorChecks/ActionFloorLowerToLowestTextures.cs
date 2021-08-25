
#region ================== Namespaces

using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Map;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	public class ActionFloorLowerToLowestTextures : BaseActionTextures
	{
		#region ================== Methods

		// Gather the lift and lower to lowest floor specials from the configuration.
		protected override bool InspectsAction(LinedefActionInfo info)
		{
			return info.ErrorCheckerExemptions.FloorLowerToLowest;
		}

		// Determine whether a lower texture is needed after the sector lowers to the lowest neighbour floor.
		protected override bool HasAdjustedSector(Sidedef side)
		{
			int lowestheight = side.Sector.FloorHeight;

			// Find height of the neighbouring sector with the lowest floor.
			foreach (Sidedef s in side.Sector.Sidedefs)
			{
				if (s.Other != null && s.Other.Sector != side.Sector && s.Other.Sector.FloorHeight < lowestheight)
				{
					lowestheight = s.Other.Sector.FloorHeight;
				}
			}

			return side.Other.Sector.FloorHeight > lowestheight;
		}

		#endregion
	}
}
