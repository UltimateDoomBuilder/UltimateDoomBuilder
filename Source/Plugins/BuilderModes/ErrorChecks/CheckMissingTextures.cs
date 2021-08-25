
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

using CodeImp.DoomBuilder.Map;
using System.Collections.Generic;
using System.Threading;
using System;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	[ErrorChecker("Check missing textures", true, 80)]
	public class CheckMissingTextures : BaseCheckTextures
	{
		#region ================== Constants

		private const int PROGRESS_STEP = 1000;

		#endregion

		#region ================== Constructor / Destructor

		// Constructor
		public CheckMissingTextures() : base()
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
				// Check upper texture. Also make sure not to return a false
				// positive if the sector on the other side has the ceiling
				// set to be sky
				if (sd.LongHighTexture == MapSet.EmptyLongName) {
					if (sd.HighRequired())
					{
						if (sd.Line.Action == 181 && sd.Line.Args[1] > 0) continue; //mxd. Ceiling slopes doesn't require upper texture

						SubmitResult(new ResultMissingTexture(sd, SidedefPart.Upper));
					}
					else if (sd.Other != null)
					{
						// Check if the sidedef's sector is a 3D floor. Since it points toward the 3D floor it only needs a texture if inside rendering is enabled
						if (sd.Sector.Tags.Count > 0)
						{
							foreach (int tag in sd.Sector.Tags)
							{
								if (sector3dfloors.ContainsKey(tag) && sector3dfloors[tag].HasFlag(Flags3DFloor.UseUpper) && sector3dfloors[tag].HasFlag(Flags3DFloor.RenderInside))
								{
									SubmitResult(new ResultMissingTexture(sd, SidedefPart.Upper));
									break;
								}
							}
						}

						// Check if the other sidedef's sector is a 3D floor, since we still might need a texture on this one depending on the flags
						if (sd.Other.Sector.Tags.Count > 0)
						{
							foreach (int tag in sd.Other.Sector.Tags)
							{
								if (sector3dfloors.ContainsKey(tag) && sector3dfloors[tag].HasFlag(Flags3DFloor.UseUpper))
								{
									SubmitResult(new ResultMissingTexture(sd, SidedefPart.Upper));
									break;
								}
							}
						}
					}
				}

				// Check middle texture
				if(sd.MiddleRequired() && sd.MiddleTexture == "-")
				{
					SubmitResult(new ResultMissingTexture(sd, SidedefPart.Middle));
				}

				// Check lower texture. Also make sure not to return a false
				// positive if the sector on the other side has the floor
				// set to be sky
				if(sd.LongLowTexture == MapSet.EmptyLongName)
				{
					if (sd.LowRequired())
					{
						if (sd.Line.Action == 181 && sd.Line.Args[0] > 0) continue; //mxd. Floor slopes doesn't require lower texture

						SubmitResult(new ResultMissingTexture(sd, SidedefPart.Lower));
					}
					else if (sd.Other != null)
					{
						if (sd.Sector.Tags.Count > 0)
						{
							foreach (int tag in sd.Sector.Tags)
							{
								// Check if the sidedef's sector is a 3D floor. Since it points toward the 3D floor it only needs a texture if inside rendering is enabled
								if (sector3dfloors.ContainsKey(tag) && sector3dfloors[tag].HasFlag(Flags3DFloor.UseLower) && sector3dfloors[tag].HasFlag(Flags3DFloor.RenderInside))
								{
									SubmitResult(new ResultMissingTexture(sd, SidedefPart.Lower));
									break;
								}

								// Check if the sector is a floor to be lowered.
								if (floorlowertolowest.RequiresTexture(sd, tag))
								{
									SubmitResult(new ResultMissingTexture(sd, SidedefPart.Lower));
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
									SubmitResult(new ResultMissingTexture(sd, SidedefPart.Lower));
									break;
								}

								// Check if the other sidedef's sector is a floor to be raised.
								if (floorraisetonexthigher.RequiresTexture(sd.Other, tag) || floorraisetohighest.RequiresTexture(sd.Other, tag))
								{
									SubmitResult(new ResultMissingTexture(sd, SidedefPart.Lower));
									break;
								}
							}
						}
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
