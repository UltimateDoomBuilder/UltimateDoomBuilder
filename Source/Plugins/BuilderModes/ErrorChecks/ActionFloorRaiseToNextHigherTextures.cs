
#region ================== Namespaces

using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Map;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	public class ActionFloorRaiseToNextHigherTextures : BaseActionTextures
	{
		#region ================== Methods

		// Gather the raise floor to next higher floor specials from the configuration.
		protected override bool InspectsAction(LinedefActionInfo info)
		{
			return info.ErrorCheckerExemptions.FloorRaiseToNextHigher;
		}

		// Determine whether a lower texture is needed after the sector raises to the next higher neighbour floor.
		protected override bool HasAdjustedSector(Sidedef side)
		{
			int? nextheight = null;

			// Find height of the lowest neighbouring sector above this floor.
			foreach (Sidedef s in side.Sector.Sidedefs)
			{
				if (s.Other != null && s.Other.Sector.FloorHeight > side.Sector.FloorHeight && (!nextheight.HasValue || s.Other.Sector.FloorHeight < nextheight.Value))
				{
					nextheight = s.Other.Sector.FloorHeight;
				}
			}

			return nextheight.HasValue && side.Other.Sector.FloorHeight < nextheight.Value;
		}

		#endregion
	}
}
