
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
using CodeImp.DoomBuilder.GZBuilder.Data;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Geometry;

#endregion

namespace CodeImp.DoomBuilder.Rendering
{
	internal abstract class Renderer : IRenderResource, IDisposable
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		// Graphics
		protected RenderDevice graphics;
		protected static bool fullbrightness;

		// Disposing
		protected bool isdisposed;

		#endregion

		#region ================== Properties

		// Disposing
		public bool IsDisposed { get { return isdisposed; } }
		public static bool FullBrightness { get { return fullbrightness; } set { fullbrightness = value; } } //mxd
		
		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		protected Renderer(RenderDevice g)
		{
			// Initialize
			this.graphics = g;

			// Register as resource
			g.RegisterResource(this);
			
			// We have no destructor
			GC.SuppressFinalize(this);
		}

		// Disposer
		public virtual void Dispose()
		{
			// Not already disposed?
			if(!isdisposed)
			{
				// Clean up

				// Unregister resource
				graphics.UnregisterResource(this);
				
				// Done
				graphics = null;
				isdisposed = true;
			}
		}

		#endregion

		#region ================== Methods

		// This calculates the sector brightness level
		public int CalculateBrightness(int level)
		{
			// Simulate doom light levels
			bool usedoomlightlevels = General.Map.Config.DoomLightLevels; //mxd

			//mxd. Very limited LightMode support...
			switch(General.Map.Data.MapInfo.LightMode)
			{
				case MapInfo.GZDoomLightMode.DOOM: usedoomlightlevels = true; break;
				case MapInfo.GZDoomLightMode.STANDARD: usedoomlightlevels = false; break;
			}

			if(level < 192 && usedoomlightlevels)
				level = (int)(192.0f - (192 - level) * 1.5f);
			
			byte blevel = (byte)General.Clamp(level, 0, 255);
			return new PixelColor(255, blevel, blevel, blevel).ToInt();
		}

		//mxd. This calculates wall brightness level with doom-style shading
		public int CalculateBrightness(int level, Sidedef sd) 
		{
			if(level < 253 && sd != null) 
			{
				bool evenlighting = General.Map.Data.MapInfo.EvenLighting;
				bool smoothlighting = General.Map.Data.MapInfo.SmoothLighting;

				//check for possiburu UDMF overrides
				if(General.Map.UDMF) 
				{
					if(sd.IsFlagSet("lightabsolute") && sd.Fields.ContainsKey("light")) 
					{
						evenlighting = true;
					} 
					else 
					{
						if(sd.IsFlagSet("nofakecontrast"))
							evenlighting = true;
						if(sd.IsFlagSet("smoothlighting"))
							smoothlighting = true;
					}
				}

				if(!evenlighting) 
				{
					//all walls are shaded by their angle
					if(smoothlighting) 
					{
						float ammount = Math.Abs((float)Math.Sin(sd.Angle));
						int hAmmount = (int)((1.0f - ammount) * General.Map.Data.MapInfo.HorizWallShade);
						int vAmmount = (int)(ammount * General.Map.Data.MapInfo.VertWallShade);

						level = General.Clamp(level - hAmmount - vAmmount, 0, 255);

					} 
					else //only horizontal/verticel walls are shaded
					{
						switch((int)Angle2D.RadToDeg(sd.Angle))
						{
							// Horizontal wall
							case 90:
							case 270:
								level = General.Clamp(level + General.Map.Data.MapInfo.HorizWallShade, 0, 255);
								break;
							// Vertical wall
							case 180:
							case 0:
								level = General.Clamp(level + General.Map.Data.MapInfo.VertWallShade, 0, 255);
								break;
						}
					}
				}
			}

			return CalculateBrightness(level);
		}

		// This is called when the graphics need to be reset
		//public virtual void Reset() { }

		// For DirectX resources
		public virtual void UnloadResource() { }
		public virtual void ReloadResource() { }

		#endregion
	}
}
