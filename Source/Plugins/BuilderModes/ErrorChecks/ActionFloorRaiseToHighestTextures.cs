
#region ================== Namespaces

using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Map;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
    public class ActionFloorRaiseToHighestTextures : BaseActionTextures
    {
        #region ================== Methods

        // Gather the raise floor to highest floor specials from the configuration.
        protected override bool InspectsAction(LinedefActionInfo info)
        {
            return info.ErrorCheckerExemptions.FloorRaiseToHighest;
        }

        // Determine whether a lower texture is needed after the sector raises to the highest neighbour floor.
        protected override bool HasAdjustedSector(Sidedef side)
        {
            int highestheight = side.Sector.FloorHeight;

            // Find height of the highest neighbouring sector floor.
            foreach (Sidedef s in side.Sector.Sidedefs)
            {
                if (s.Other != null && s.Other.Sector != side.Sector && s.Other.Sector.FloorHeight > highestheight)
                {
                    highestheight = s.Other.Sector.FloorHeight;
                }
            }

            return side.Other.Sector.FloorHeight < highestheight;
        }

        #endregion
    }
}
