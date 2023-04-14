
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

using System.Collections.Generic;

#endregion

namespace CodeImp.DoomBuilder.Rendering
{
	/// <summary>
	/// Presentation settings
	/// </summary>
	public class Presentation
	{
		// Constants for static instances
		public const float THINGS_BACK_ALPHA = 0.3f;
		public const float THINGS_HIDDEN_ALPHA = 0.66f;
		public const float THINGS_ALPHA = 1.0f; //mxd
		
		// Static instances
		private static Presentation standard;
		private static Presentation things;
		
		// Static properties
		public static Presentation Standard { get { return standard; } }
		public static Presentation Things { get { return things; } }

		// Public properties
		public bool SkipHiddenSectors { get; set; } // Skip drawing sectors with the "hidden" flag set (i.e. UDMF's "hide on textured automap" property)
		
		// Variables
		protected internal List<PresentLayer> layers;
		
		// Constructor
		public Presentation()
		{
			// Initialize
			layers = new List<PresentLayer>();
			SkipHiddenSectors = false;
		}

		// Copy constructor
		public Presentation(Presentation p)
		{
			layers = new List<PresentLayer>(p.layers);
			SkipHiddenSectors = p.SkipHiddenSectors;
		}

		// This creates the static instances
		internal static void Initialize()
		{
			// Standard classic mode
			standard = new Presentation();
			standard.layers.Add(new PresentLayer(RendererLayer.Background, BlendingMode.Mask, General.Settings.BackgroundAlpha));
			standard.layers.Add(new PresentLayer(RendererLayer.Surface, BlendingMode.Mask));
			standard.layers.Add(new PresentLayer(RendererLayer.Things, BlendingMode.Alpha, General.Settings.InactiveThingsAlpha));
			standard.layers.Add(new PresentLayer(RendererLayer.Grid, BlendingMode.Mask));
			standard.layers.Add(new PresentLayer(RendererLayer.Geometry, BlendingMode.Alpha, 1f, true));
			standard.layers.Add(new PresentLayer(RendererLayer.Overlay, BlendingMode.Alpha, 1f, true));
			
			// Things classic mode
			things = new Presentation();
			things.layers.Add(new PresentLayer(RendererLayer.Background, BlendingMode.Mask, General.Settings.BackgroundAlpha));
			things.layers.Add(new PresentLayer(RendererLayer.Surface, BlendingMode.Mask));
			things.layers.Add(new PresentLayer(RendererLayer.Things, BlendingMode.Alpha, 1f));
			things.layers.Add(new PresentLayer(RendererLayer.Grid, BlendingMode.Mask));
			things.layers.Add(new PresentLayer(RendererLayer.Geometry, BlendingMode.Alpha, 1f, true));
			things.layers.Add(new PresentLayer(RendererLayer.Things, BlendingMode.Alpha, 0.5f)); //mxd
			things.layers.Add(new PresentLayer(RendererLayer.Overlay, BlendingMode.Alpha, 1f, true));
		}
	}

	/// <summary>
	/// Customizable Presentation
	/// </summary>
	public class CustomPresentation : Presentation
	{
		// Allow public adding
		public void AddLayer(PresentLayer layer)
		{
			this.layers.Add(layer);
		}
	}
	
	public struct PresentLayer
	{
		public RendererLayer layer;
		public BlendingMode blending;
		public float alpha;
		public bool antialiasing;

		// Constructor
		public PresentLayer(RendererLayer layer, BlendingMode blending, float alpha, bool antialiasing)
		{
			this.layer = layer;
			this.blending = blending;
			this.alpha = alpha;
			this.antialiasing = antialiasing;
		}

		// Constructor
		public PresentLayer(RendererLayer layer, BlendingMode blending, float alpha)
		{
			this.layer = layer;
			this.blending = blending;
			this.alpha = alpha;
			this.antialiasing = false;
		}

		// Constructor
		public PresentLayer(RendererLayer layer, BlendingMode blending)
		{
			this.layer = layer;
			this.blending = blending;
			this.alpha = 1f;
			this.antialiasing = false;
		}

		// Constructor
		public PresentLayer(RendererLayer layer)
		{
			this.layer = layer;
			this.blending = BlendingMode.None;
			this.alpha = 1f;
			this.antialiasing = false;
		}
	}
	
	// The different layers
	public enum RendererLayer
	{
		Background,
		Grid,
		Things,
		Geometry,
		Overlay,
		Surface
	}

	// Blending modes
	public enum BlendingMode
	{
		None,
		Mask,
		Alpha,
		Additive,
	}
}
