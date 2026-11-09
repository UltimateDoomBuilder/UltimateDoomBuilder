namespace CodeImp.DoomBuilder.Rendering
{
	public enum ModelRenderMode
	{
		NONE,
		SELECTION,
		ACTIVE_THINGS_FILTER,
		ALL,
	}

	public enum LightRenderMode
	{
		NONE,
		ALL,
		ALL_ANIMATED,
	}

	public enum ThingRenderMode
	{
		NORMAL,
		MODEL,
		VOXEL,
		WALLSPRITE,
		FLATSPRITE,
	}

	public enum EventLinesRenderMode //Adding new enum to define the three states for showing event lines
	{
		NONE = 0,
		HIGHLIGHT = 1,
		SELECTION = 2,
	}
}
