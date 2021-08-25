#region ================== Namespaces

using CodeImp.DoomBuilder.Map;
using System.Threading;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	[ErrorChecker("Check unused textures", true, 60)]
	public class CheckUnusedTextures : BaseCheckTextures
	{
		#region ================== Constants

		private const int PROGRESS_STEP = 1000;

		#endregion

		#region ================== Constructor / Destructor

		// Constructor
		public CheckUnusedTextures() : base()
		{
		}

		#endregion

		#region ================== Methods

		// This runs the check
		public override void Run()
		{
			int progress = 0;
			int stepprogress = 0;

			Build3DFloorCache();

			// Go for all the sidedefs
			foreach(Sidedef sd in General.Map.Map.Sidedefs)
			{
				// Check upper texture
				if(!sd.HighRequired() && sd.LongHighTexture != MapSet.EmptyLongName)
				{
					if (sd.Other == null)
						SubmitResult(new ResultUnusedTexture(sd, SidedefPart.Upper));
					else
					{
						bool unused = true;

						// Check if the sidedef's sector is a 3D floor. Since it points toward the 3D floor it only needs a texture if inside rendering is enabled
						if (sd.Sector.Tags.Count > 0)
						{
							foreach (int tag in sd.Sector.Tags)
							{
								if (sector3dfloors.ContainsKey(tag) && sector3dfloors[tag].HasFlag(Flags3DFloor.UseUpper) && sector3dfloors[tag].HasFlag(Flags3DFloor.RenderInside))
								{
									unused = false;
									break;
								}
							}
						}

						// Check if the other sidedef's sector is a 3D floor, since we still might need a texture on this one depending on the flags
						if(sd.Other.Sector.Tags.Count > 0)
						{
							foreach(int tag in sd.Other.Sector.Tags)
							{
								if(sector3dfloors.ContainsKey(tag) && sector3dfloors[tag].HasFlag(Flags3DFloor.UseUpper))
								{
									unused = false;
									break;
								}
							}
						}

						if (unused) SubmitResult(new ResultUnusedTexture(sd, SidedefPart.Upper));
					}
				}

				// Check lower texture
				if(!sd.LowRequired() && sd.LongLowTexture != MapSet.EmptyLongName)
				{
					if (sd.Other == null)
						SubmitResult(new ResultUnusedTexture(sd, SidedefPart.Lower));
					else
					{
						bool unused = true;

						if (sd.Sector.Tags.Count > 0)
						{
							foreach (int tag in sd.Sector.Tags)
							{
								// Check if the sidedef's sector is a 3D floor. Since it points toward the 3D floor it only needs a texture if inside rendering is enabled
								if (sector3dfloors.ContainsKey(tag) && sector3dfloors[tag].HasFlag(Flags3DFloor.UseLower) && sector3dfloors[tag].HasFlag(Flags3DFloor.RenderInside))
								{
									unused = false;
									break;
								}

								// Check if the sector is a floor to be lowered.
								if (floorlowertolowest.RequiresTexture(sd, tag))
								{
									unused = false;
									break;
								}
							}
						}

						if (sd.Other.Sector.Tags.Count > 0)
						{
							foreach (int tag in sd.Other.Sector.Tags)
							{
								// Check if the other sidedef's sector is a 3D floor, since we still might need a texture on this one depending on the flags
								if (sector3dfloors.ContainsKey(tag) && sector3dfloors[tag].HasFlag(Flags3DFloor.UseLower))
								{
									unused = false;
									break;
								}

								// Check if the other sidedef's sector is a floor to be raised.
								if (floorraisetonexthigher.RequiresTexture(sd.Other, tag) || floorraisetohighest.RequiresTexture(sd.Other, tag))
								{
									unused = false;
									break;
								}
							}
						}

						if (unused) SubmitResult(new ResultUnusedTexture(sd, SidedefPart.Lower));
					}
				}
				
				// Handle thread interruption
				try { Thread.Sleep(0); }
				catch(ThreadInterruptedException) { return; }

				// We are making progress!
				if((++progress / PROGRESS_STEP) > stepprogress)
				{
					stepprogress = (progress / PROGRESS_STEP);
					AddProgress(1);
				}
			}
		}

		#endregion
	}
}
