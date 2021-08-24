
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
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Types;
using CodeImp.DoomBuilder.Windows;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	public struct UDMFFieldAssociationX
	{
		public string Property;
		public string Modify;
		public UniValue Value;

		public UDMFFieldAssociationX(string property, string modify, UniValue value)
		{
			Property = property;
			Modify = modify;
			Value = value;
		}

		public UniValue GetValue()
		{
			if(Value.Type == 0)
			{
				if (Modify == "abs")
					return new UniValue(0, Math.Abs((int)Value.Value));
			}

			return new UniValue(0, 0);
		}
	}

	public class Association
	{
		private HashSet<int> tags;
		private Vector2D center;
		private UniversalType type;
		private int directlinktype;
		private Dictionary<string, List<Line3D>> eventlines;
		private IRenderer2D renderer;
		private SelectableElement element;
		private List<PixelColor> distinctcolors;
		private Font font;
		private Dictionary<string, Vector2D> textwidths;
		private Dictionary<string, List<TextLabel>> textlabels;

		// Map elements that are associated
		private List<Thing> things;
		private List<Sector> sectors;
		private List<Linedef> linedefs;

		public HashSet<int> Tags { get { return tags; } }
		public Vector2D Center { get { return center; } }
		public UniversalType Type { get { return type; } }
		public int DirectLinkType { get { return directlinktype; } }
		public List<Thing> Things { get { return things; } }
		public List<Sector> Sectors { get { return sectors; } }
		public List<Linedef> Linedefs { get { return linedefs; } }
		public bool IsEmpty { get { return things.Count == 0 && sectors.Count == 0 && linedefs.Count == 0; } }

		//mxd. This sets up the association
		public Association(IRenderer2D renderer)
		{
			this.tags = new HashSet<int> { 0 };
			this.renderer = renderer;

			things = new List<Thing>();
			sectors = new List<Sector>();
			linedefs = new List<Linedef>();
			eventlines = new Dictionary<string, List<Line3D>>();

			try
			{
				font = new Font(new FontFamily(General.Settings.TextLabelFontName), General.Settings.TextLabelFontSize, (General.Settings.TextLabelFontBold ? FontStyle.Bold : FontStyle.Regular));
			}
			catch (Exception) // Fallback if the font couldn't be loaded
			{
				font = ((MainForm)General.Interface).Font;
			}

			distinctcolors = new List<PixelColor>
			{
				General.Colors.InfoLine,
				PixelColor.FromInt(0x84d5a4).WithAlpha(255),
				PixelColor.FromInt(0xc059cb).WithAlpha(255),
				PixelColor.FromInt(0xd0533d).WithAlpha(255),
				// PixelColor.FromInt(0x415354).WithAlpha(255), // too dark
				PixelColor.FromInt(0xcea953).WithAlpha(255),
				PixelColor.FromInt(0x91d44b).WithAlpha(255),
				PixelColor.FromInt(0xcd5b89).WithAlpha(255),
				PixelColor.FromInt(0xa8b6c0).WithAlpha(255),
				PixelColor.FromInt(0x797ecb).WithAlpha(255),
				// PixelColor.FromInt(0x567539).WithAlpha(255), // too dark
				// PixelColor.FromInt(0x72422f).WithAlpha(255), // too dark
				// PixelColor.FromInt(0x5d3762).WithAlpha(255), // too dark
				PixelColor.FromInt(0xffed6f).WithAlpha(255),
				PixelColor.FromInt(0xccebc5).WithAlpha(255),
				PixelColor.FromInt(0xbc80bd).WithAlpha(255),
				// PixelColor.FromInt(0xd9d9d9).WithAlpha(255), // too gray
				PixelColor.FromInt(0xfccde5).WithAlpha(255),
				PixelColor.FromInt(0x80b1d3).WithAlpha(255),
				PixelColor.FromInt(0xfdb462).WithAlpha(255),
				PixelColor.FromInt(0xb3de69).WithAlpha(255),
				PixelColor.FromInt(0xfb8072).WithAlpha(255),
				PixelColor.FromInt(0xbebada).WithAlpha(255), // too blue/gray?
				PixelColor.FromInt(0xffffb3).WithAlpha(255),
				PixelColor.FromInt(0x8dd3c7).WithAlpha(255),
			};
		}

		/// <summary>
		/// Sets the association to a map element. Only works with an instance of Thing, Sector, or Linedef.
		/// Also gets the forward and reverse associations
		/// </summary>
		/// <param name="element">An instance of Thing, Sector, or Linedef</param>
		public void Set(SelectableElement element)
		{
			this.element = element;
			things = new List<Thing>();
			sectors = new List<Sector>();
			linedefs = new List<Linedef>();
			eventlines = new Dictionary<string, List<Line3D>>();

			if (element is Sector)
			{
				Sector s = element as Sector;
				center = (s.Labels.Count > 0 ? s.Labels[0].position : new Vector2D(s.BBox.X + s.BBox.Width / 2, s.BBox.Y + s.BBox.Height / 2));

				type = UniversalType.SectorTag;
				tags = new HashSet<int>(s.Tags);
			}
			else if(element is Linedef)
			{
				Linedef ld = element as Linedef;
				center = ld.GetCenterPoint();

				type = UniversalType.LinedefTag;
				tags = new HashSet<int>(ld.Tags);
			}
			else if(element is Thing)
			{
				Thing t = element as Thing;
				center = t.Position;

				ThingTypeInfo ti = General.Map.Data.GetThingInfoEx(t.Type);

				if (ti != null)
					directlinktype = ti.ThingLink;
				else
					directlinktype = 0;

				type = UniversalType.ThingTag;
				tags = new HashSet<int>(new int[] { t.Tag });
			}

			// Remove the tag 0, because nothing sensible will come from it
			tags.Remove(0);

			// Get forward and reverse associations
			GetAssociations();

			// Cache width of label text and generate the labels
			textwidths = new Dictionary<string, Vector2D>(eventlines.Count);
			textlabels = new Dictionary<string, List<TextLabel>>(eventlines.Count);

			foreach(KeyValuePair<string, List<Line3D>> kvp in eventlines)
			{
				SizeF size = General.Interface.MeasureString(kvp.Key, font);
				textwidths[kvp.Key] = new Vector2D(size.Width, size.Height);

				// Create one label for each line. We might not need them all, but better
				// to have them all at the beginning than to generate them later
				textlabels[kvp.Key] = new List<TextLabel>(kvp.Value.Count);

				for (int i = 0; i < kvp.Value.Count; i++)
				{
					// We don't need to set the position here, since it'll be done on the fly later
					TextLabel l = new TextLabel();
					l.AlignX = TextAlignmentX.Center;
					l.AlignY = TextAlignmentY.Middle;
					l.TransformCoords = true;
					l.Text = kvp.Key;

					textlabels[kvp.Key].Add(l);
				}

				textwidths[kvp.Key] = new Vector2D(textlabels[kvp.Key][0].TextSize.Width, textlabels[kvp.Key][0].TextSize.Height);
			}

			SetEventLineColors();
		}

		/// <summary>
		/// Clears out all lists so that the association appears empty
		/// </summary>
		public void Clear()
		{
			tags = new HashSet<int>();
			things = new List<Thing>();
			sectors = new List<Sector>();
			linedefs = new List<Linedef>();
			eventlines = new Dictionary<string, List<Line3D>>();
		}

		/// <summary>
		/// Get the forward and reverse associations between the element and other map elements
		/// </summary>
		private void GetAssociations()
		{
			Dictionary<int, HashSet<int>> actiontags = new Dictionary<int, HashSet<int>>();
			bool showforwardlabel = BuilderPlug.Me.EventLineLabelVisibility == 1 || BuilderPlug.Me.EventLineLabelVisibility == 3;
			bool showreverselabel = BuilderPlug.Me.EventLineLabelVisibility == 2 || BuilderPlug.Me.EventLineLabelVisibility == 3;

			// Association from line to line according to the action and tag.
			if (tags.Count > 0 && element is Linedef)
			{
				Linedef ld = element as Linedef;

				if (ld.Action > 0)
				{
					foreach (Linedef otherlinedef in General.Map.Map.Linedefs)
					{
						if (ld.IsAssociatedWith(otherlinedef))
						{
							linedefs.Add(otherlinedef);

							AddLineToAction(showreverselabel ? GetActionDescription(ld) : string.Empty, center, otherlinedef.GetCenterPoint());
						}
					}
				}
			}

			// Special handling for Doom format maps where there the linedef's tag references sectors
			if (General.Map.Config.LineTagIndicatesSectors)
			{
				if (tags.Count == 0)
					return;

				// Forward association from linedef to sector
				if (element is Linedef)
				{
					foreach (Sector s in General.Map.Map.Sectors)
					{
						if (tags.Contains(s.Tag))
						{
							Vector2D sectorcenter = (s.Labels.Count > 0 ? s.Labels[0].position : new Vector2D(s.BBox.X + s.BBox.Width / 2, s.BBox.Y + s.BBox.Height / 2));

							sectors.Add(s);

							AddLineToAction(showforwardlabel ? GetActionDescription(element) : string.Empty, center, sectorcenter);
						}
					}
				}
				else if(element is Sector)
				{
					foreach(Linedef ld in General.Map.Map.Linedefs)
					{
						if(tags.Contains(ld.Tag))
						{
							linedefs.Add(ld);

							AddLineToAction(showreverselabel ? GetActionDescription(ld) : string.Empty, ld.GetCenterPoint(), center);
						}
					}
				}

				return;
			}

			// Get tags of map elements the element is referencing. This is used for the forward associations
			if (element is Linedef || element is Thing)
				actiontags = GetTagsByType();

			// Store presence of different types once, so that we don't have to do a lookup for each map element
			bool hassectortags = actiontags.ContainsKey((int)UniversalType.SectorTag);
			bool haslinedeftags = actiontags.ContainsKey((int)UniversalType.LinedefTag);
			bool hasthingtag = actiontags.ContainsKey((int)UniversalType.ThingTag);

			// Process all sectors in the map
			foreach (Sector s in General.Map.Map.Sectors)
			{
				bool addforward = false;
				bool addreverse = false;

				if (s == element)
					continue;

				// Check for forward association (from the element to the sector)
				if (hassectortags && actiontags[(int)UniversalType.SectorTag].Overlaps(s.Tags))
					addforward = true;

				// Check the reverse association (from the sector to the element)
				// Nothing here yet

				if (addforward || addreverse)
				{
					Vector2D sectorcenter = (s.Labels.Count > 0 ? s.Labels[0].position : new Vector2D(s.BBox.X + s.BBox.Width / 2, s.BBox.Y + s.BBox.Height / 2));

					sectors.Add(s);

					if (addforward)
						AddLineToAction(showforwardlabel ? GetActionDescription(element) : string.Empty, center, sectorcenter);

					if (addreverse)
						AddLineToAction(showreverselabel ? GetActionDescription(element) : string.Empty, sectorcenter, center);
				}

				// Check arbitrary UDMF field associations
				foreach (UniversalFieldInfo ufi in General.Map.Config.SectorFields)
				{
					if (ufi.Associations.Count == 0)
						continue;

					UniValue ouv;

					if (element.Fields.TryGetValue(ufi.Name, out ouv))
					{
						foreach (KeyValuePair<string, UDMFFieldAssociation> kvp in ufi.Associations)
						{
							UniValue uv;
							if (s.Fields.TryGetValue(kvp.Key, out uv))
							{
								if(UniValuesMatch(uv, ouv, kvp.Value.Modify, ufi.Default))
								{
									Vector2D sectorcenter = (s.Labels.Count > 0 ? s.Labels[0].position : new Vector2D(s.BBox.X + s.BBox.Width / 2, s.BBox.Y + s.BBox.Height / 2));

									if(showforwardlabel && kvp.Value.NeverShowEventLines == false)
										AddLineToAction(kvp.Key + ": " + uv.Value, center, sectorcenter);

									sectors.Add(s);
								}
							}
						}
					}
				}
			}

			// Process all linedefs in the map
			foreach(Linedef ld in General.Map.Map.Linedefs)
			{
				bool addforward = false;
				bool addreverse = false;

				// Check the forward association (from the element to the linedef)
				if (haslinedeftags && actiontags[(int)UniversalType.LinedefTag].Overlaps(ld.Tags))
					addforward = true;

				// Check the reverse association (from the linedef to the element)
				if (IsAssociatedToLinedef(ld))
					addreverse = true;

				if (addforward || addreverse)
				{
					linedefs.Add(ld);

					if (addforward)
						AddLineToAction(showforwardlabel ? GetActionDescription(element) : string.Empty, center, ld.GetCenterPoint());

					if (addreverse)
						AddLineToAction(showreverselabel ? GetActionDescription(ld) : string.Empty, ld.GetCenterPoint(), center);
				}
			}

			// Doom format only knows associations between linedefs and sectors, but not thing, so stop here
			if (General.Map.DOOM)
				return;

			// Process all things in the map
			foreach(Thing t in General.Map.Map.Things)
			{
				bool addforward = false;
				bool addreverse = false;

				// Check the forward association (from the element to the thing)
				if (hasthingtag && actiontags[(int)UniversalType.ThingTag].Contains(t.Tag))
					addforward = true;

				// Check the reverse association (from the thing to the element). Only works for Hexen and UDMF,
				// as Doom format doesn't have any way to reference other map elements
				if (IsAssociatedToThing(t))
					addreverse = true;

				if (addforward || addreverse)
				{
					things.Add(t);

					if (addforward)
						AddLineToAction(showforwardlabel ? GetActionDescription(element) : string.Empty, center, t.Position);

					if (addreverse)
						AddLineToAction(showreverselabel ? GetActionDescription(t) : string.Empty, t.Position, center);
				}
			}
		}

		/// <summary>
		/// Gets a dictionary of sector tags, linedef tags, and thing tags, grouped by their type, that the map element is referencing
		/// </summary>
		/// <returns>Dictionary of sector tags, linedef tags, and thing tags that the map element is referencing</returns>
		private Dictionary<int, HashSet<int>> GetTagsByType()
		{
			LinedefActionInfo action = null;
			int[] actionargs = new int[5];
			Dictionary<int, HashSet<int>> actiontags = new Dictionary<int, HashSet<int>>();

			// Get the action and its arguments from a linedef or a thing, if they have them
			if (element is Linedef)
			{
				Linedef ld = element as Linedef;

				if (ld.Action > 0 && General.Map.Config.LinedefActions.ContainsKey(ld.Action))
					action = General.Map.Config.LinedefActions[ld.Action];

				actionargs = ld.Args;
			}
			else if (element is Thing)
			{
				Thing t = element as Thing;

				if (t.Action > 0 && General.Map.Config.LinedefActions.ContainsKey(t.Action))
					action = General.Map.Config.LinedefActions[t.Action];

				actionargs = t.Args;
			}
			else // element is a Sector
			{
				return actiontags;
			}

			if (action != null)
			{
				// Collect what map element the action arguments are referencing. Ignore the argument if it's 0, so that they
				// are not associated to everything untagged
				for (int i = 0; i < Linedef.NUM_ARGS; i++)
				{
					if ((action.Args[i].Type == (int)UniversalType.SectorTag ||
						action.Args[i].Type == (int)UniversalType.LinedefTag ||
						action.Args[i].Type == (int)UniversalType.ThingTag) &&
						actionargs[i] > 0)
					{
						if (!actiontags.ContainsKey(action.Args[i].Type))
							actiontags[action.Args[i].Type] = new HashSet<int>();

						actiontags[action.Args[i].Type].Add(actionargs[i]);
					}
				}
			}
			else if (element is Thing && directlinktype >= 0 && Math.Abs(directlinktype) != ((Thing)element).Type)
			{
				// The direct link shenanigans if the thing doesn't have an action, but still reference something through
				// the action parameters
				Thing t = element as Thing;
				ThingTypeInfo ti = General.Map.Data.GetThingInfoEx(t.Type);

				if (ti != null && directlinktype >= 0 && Math.Abs(directlinktype) != t.Type)
				{
					for (int i = 0; i < Linedef.NUM_ARGS; i++)
					{
						if ((ti.Args[i].Type == (int)UniversalType.SectorTag ||
							ti.Args[i].Type == (int)UniversalType.LinedefTag ||
							ti.Args[i].Type == (int)UniversalType.ThingTag))
						{
							if (!actiontags.ContainsKey(ti.Args[i].Type))
								actiontags[ti.Args[i].Type] = new HashSet<int>();

							actiontags[ti.Args[i].Type].Add(actionargs[i]);
						}

					}
				}
			}

			return actiontags;
		}

		/// <summary>
		/// Checks if there's an association between the element and a Linedef
		/// </summary>
		/// <param name="linedef">Linedef to check the association against</param>
		/// <returns>true if the Linedef and the element are associated, false if not</returns>
		private bool IsAssociatedToLinedef(Linedef linedef)
		{
			// Doom style reference from linedef to sector?
			if (General.Map.Config.LineTagIndicatesSectors && element is Sector)
			{
				if (linedef.Action > 0 && tags.Overlaps(linedef.Tags))
					return true;
			}

			// Known action on this line?
			if ((linedef.Action > 0) && General.Map.Config.LinedefActions.ContainsKey(linedef.Action))
			{
				LinedefActionInfo action = General.Map.Config.LinedefActions[linedef.Action];
				if (((action.Args[0].Type == (int)type) && (linedef.Args[0] != 0) && (tags.Contains(linedef.Args[0]))) ||
					((action.Args[1].Type == (int)type) && (linedef.Args[1] != 0) && (tags.Contains(linedef.Args[1]))) ||
					((action.Args[2].Type == (int)type) && (linedef.Args[2] != 0) && (tags.Contains(linedef.Args[2]))) ||
					((action.Args[3].Type == (int)type) && (linedef.Args[3] != 0) && (tags.Contains(linedef.Args[3]))) ||
					((action.Args[4].Type == (int)type) && (linedef.Args[4] != 0) && (tags.Contains(linedef.Args[4]))))
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Checks if there's an association between the element and a Thing
		/// </summary>
		/// <param name="thing">Thing to check the association against</param>
		/// <returns>true if the Thing and the element are associated, false if not</returns>
		private bool IsAssociatedToThing(Thing thing)
		{
			// Get the thing type info
			ThingTypeInfo ti = General.Map.Data.GetThingInfoEx(thing.Type);

			// Known action on this thing?
			if ((thing.Action > 0) && General.Map.Config.LinedefActions.ContainsKey(thing.Action))
			{
				//Do not draw the association if this is a child link.
				//  This prevents a reverse link to a thing via an argument, when it should be a direct tag-to-tag link instead.
				if (ti != null && directlinktype < 0 && directlinktype != -thing.Type)
					return false;

				LinedefActionInfo action = General.Map.Config.LinedefActions[thing.Action];
				if (((action.Args[0].Type == (int)type) && (tags.Contains(thing.Args[0]))) ||
					 ((action.Args[1].Type == (int)type) && (tags.Contains(thing.Args[1]))) ||
					 ((action.Args[2].Type == (int)type) && (tags.Contains(thing.Args[2]))) ||
					 ((action.Args[3].Type == (int)type) && (tags.Contains(thing.Args[3]))) ||
					 ((action.Args[4].Type == (int)type) && (tags.Contains(thing.Args[4]))))
				{
					return true;
				}

				//If there is a link setup on this thing, and it matches the association, then draw a direct link to any matching tag
				if (ti != null && directlinktype == thing.Type && tags.Contains(thing.Tag))
				{
					return true;
				}
			}
			//mxd. Thing action on this thing?
			else if (thing.Action == 0)
			{
				// Gets the association, unless it is a child link.
				// This prevents a reverse link to a thing via an argument, when it should be a direct tag-to-tag link instead.
				if (ti != null && directlinktype >= 0 && Math.Abs(directlinktype) != thing.Type)
				{
					if (((ti.Args[0].Type == (int)type) && (tags.Contains(thing.Args[0]))) ||
						 ((ti.Args[1].Type == (int)type) && (tags.Contains(thing.Args[1]))) ||
						 ((ti.Args[2].Type == (int)type) && (tags.Contains(thing.Args[2]))) ||
						 ((ti.Args[3].Type == (int)type) && (tags.Contains(thing.Args[3]))) ||
						 ((ti.Args[4].Type == (int)type) && (tags.Contains(thing.Args[4]))))
					{
						return true;
					}
				}
			}

			return false;
		}

		/// <summary>
		/// Returns a string that contains the description of the action and its arguments, based on the given Linedef or Thing
		/// </summary>
		/// <param name="se">An instance of Thing or Linedef</param>
		/// <returns>String that contains the description of the action and its arguments for a given Linedef or Thing</returns>
		private string GetActionDescription(SelectableElement se)
		{
			int action = 0;
			int[] actionargs = new int[5];

			if (se is Thing)
			{
				action = ((Thing)se).Action;
				actionargs = ((Thing)se).Args;
			}
			else if(se is Linedef)
			{
				action = ((Linedef)se).Action;
				actionargs = ((Linedef)se).Args;
			}

			if (action > 0)
			{
				LinedefActionInfo lai = General.Map.Config.GetLinedefActionInfo(action);
				List<string> argdescription = new List<string>();

				string description = lai.Index + ": " + lai.Title;

				// Label style: only action, or if the element can't have any parameters
				if (BuilderPlug.Me.EventLineLabelStyle == 0 || General.Map.Config.LineTagIndicatesSectors)
					return description;

				for (int i=0; i < 5; i++)
				{
					if(lai.Args[i].Used)
					{
						string argstring = "";

						if(BuilderPlug.Me.EventLineLabelStyle == 2) // Label style: full arguments
							argstring = lai.Args[i].Title + ": ";

						EnumItem ei = lai.Args[i].Enum.GetByEnumIndex(actionargs[i].ToString());

						if (ei != null && BuilderPlug.Me.EventLineLabelStyle == 2) // Label style: full arguments
							argstring += ei.ToString();
						else // Argument has no EnumItem or label style: short arguments
							argstring += actionargs[i].ToString();

						argdescription.Add(argstring);
					}
				}

				description += " (" + string.Join(", ", argdescription) + ")";

				return description;
			}

			return null;
		}

		/// <summary>
		/// Sets a different color for each event
		/// </summary>
		private void SetEventLineColors()
		{
			int colorindex = 0;

			foreach(KeyValuePair<string, List<TextLabel>> kvp in textlabels)
			{
				foreach (Line3D l in eventlines[kvp.Key])
					l.Color = distinctcolors[colorindex];

				foreach (TextLabel l in kvp.Value)
					l.Color = distinctcolors[colorindex];

				if(BuilderPlug.Me.EventLineDistinctColors)
					if (++colorindex >= distinctcolors.Count)
						colorindex = 0;
			}
		}

		/// <summary>
		/// Adds a line to an action
		/// </summary>
		/// <param name="action">Name of the action</param>
		/// <param name="start">Start of the line</param>
		/// <param name="end">End of the line</param>
		private void AddLineToAction(string action, Vector2D start, Vector2D end)
		{
			if (action == null)
				return;

			if (!eventlines.ContainsKey(action))
				eventlines[action] = new List<Line3D>();

			eventlines[action].Add(new Line3D(start, end, true));
		}

		/// <summary>
		/// Generates a label position given a start and end point of a line. Taken (with modifications) from LineLengthLabel.Move()
		/// </summary>
		/// <param name="start">Start of the line</param>
		/// <param name="end">End of the line</param>
		/// <returns></returns>
		private Vector2D GetLabelPosition(Vector2D start, Vector2D end)
		{
			// Check if start/end point is on screen...
			Vector2D lt = General.Map.Renderer2D.DisplayToMap(new Vector2D(0.0, General.Interface.Display.Size.Height));
			Vector2D rb = General.Map.Renderer2D.DisplayToMap(new Vector2D(General.Interface.Display.Size.Width, 0.0));
			RectangleF viewport = new RectangleF((float)lt.x, (float)lt.y, (float)(rb.x - lt.x), (float)(rb.y - lt.y));
			bool startvisible = viewport.Contains((float)start.x, (float)start.y);
			bool endvisible = viewport.Contains((float)end.x, (float)end.y);

			// Do this only when one point is visible, an the other isn't 
			if ((!startvisible && endvisible) || (startvisible && !endvisible))
			{
				Line2D drawnline = new Line2D(start, end);
				Line2D[] viewportsides = new[] {
					new Line2D(lt, rb.x, lt.y), // top
					new Line2D(lt.x, rb.y, rb.x, rb.y), // bottom
					new Line2D(lt, lt.x, rb.y), // left
					new Line2D(rb.x, lt.y, rb.x, rb.y), // right
				};

				foreach (Line2D side in viewportsides)
				{
					// Modify the start point so it stays on screen
					double u;
					if (!startvisible && side.GetIntersection(drawnline, out u))
					{
						start = drawnline.GetCoordinatesAt(u);
						break;
					}

					// Modify the end point so it stays on screen
					if (!endvisible && side.GetIntersection(drawnline, out u))
					{
						end = drawnline.GetCoordinatesAt(u);
						break;
					}
				}
			}

			// Create position
			Vector2D delta = end - start;
			return new Vector2D(start.x + delta.x * 0.5, start.y + delta.y * 0.5);
		}

		/// <summary>
		/// Merges label positions based on a merge distance
		/// </summary>
		/// <param name="positions">Positions to merge</param>
		/// <param name="distance">Distance to merge positions at</param>
		/// <returns>List of new positions</returns>
		List<Vector2D> MergePositions(List<Vector2D> positions, Vector2D distance)
		{
			List<Vector2D> allpositions = positions.OrderBy(o => o.x).ToList();
			List<Vector2D> newpositions = new List<Vector2D>(positions.Count);
			Vector2D mergedistance = distance / renderer.Scale * 1.5;

			// Keep going while we have positions me might want to merge
			while (allpositions.Count > 0)
			{
				Vector2D curposition = allpositions[0];
				allpositions.RemoveAt(0);

				bool hasclosepositions = true;

				// Keep merging as long as there are close positions nearby
				while(hasclosepositions)
				{
					// Get all positions that are close to the current position
					List<Vector2D> closepositions = allpositions.Where(o => Math.Abs(curposition.x - o.x) < mergedistance.x && Math.Abs(curposition.y - o.y) < mergedistance.y).ToList();

					if (closepositions.Count > 0)
					{
						Vector2D tl = curposition;
						Vector2D br = curposition;

						// Get the max dimensions of the positions...
						foreach (Vector2D v in closepositions)
						{
							if (v.x < tl.x) tl.x = v.x;
							if (v.x > br.x) br.x = v.x;
							if (v.y > tl.y) tl.y = v.y;
							if (v.y < br.y) br.y = v.y;

							// Remove the position from the list so that it doesn't get checked again
							allpositions.Remove(v);
						}

						// ... and set the current position to the center of that
						curposition.x = tl.x + (br.x - tl.x) / 2.0;
						curposition.y = tl.y + (br.y - tl.y) / 2.0;
					}
					else
					{
						// The current position is a new final position
						newpositions.Add(curposition);
						hasclosepositions = false;
						allpositions.Reverse();
					}
				}
			}

			return newpositions;
		}

		/// <summary>
		/// Checks if the type and value of two UniValues match. Takes modifiers into account
		/// </summary>
		/// <param name="uv1">First UniValue</param>
		/// <param name="uv2">Second UniValue</param>
		/// <param name="ufam">Modifier</param>
		/// <returns>True if values match</returns>
		private bool UniValuesMatch(UniValue uv1, UniValue uv2, UDMFFieldAssociationModifier ufam, object def)
		{
			if (uv1.Type != uv2.Type)
				return false;

			switch ((UniversalType)uv1.Type)
			{
				case UniversalType.AngleRadians:
				case UniversalType.AngleDegreesFloat:
				case UniversalType.Float:
					double d1 = (double)uv1.Value;
					double d2 = (double)uv2.Value;

					if(ufam == UDMFFieldAssociationModifier.Absolute)
					{
						d1 = Math.Abs(d1);
						d2 = Math.Abs(d2);
					}

					if (d1 == d2)
						return true;

					break;

				case UniversalType.AngleDegrees:
				case UniversalType.AngleByte:
				case UniversalType.Color:
				case UniversalType.EnumBits:
				case UniversalType.EnumOption:
				case UniversalType.Integer:
				case UniversalType.LinedefTag:
				case UniversalType.LinedefType:
				case UniversalType.SectorEffect:
				case UniversalType.SectorTag:
				case UniversalType.ThingTag:
				case UniversalType.ThingType:
					int i1 = (int)uv1.Value;
					int i2 = (int)uv2.Value;

					if (ufam == UDMFFieldAssociationModifier.Absolute)
					{
						i1 = Math.Abs(i1);
						i2 = Math.Abs(i2);
					}

					if (i1 == i2 && i1 != (int)def)
						return true;

					break;

				case UniversalType.Boolean:
					if ((bool)uv1.Value == (bool)uv2.Value)
						return true;
					break;

				case UniversalType.Flat:
				case UniversalType.String:
				case UniversalType.Texture:
				case UniversalType.EnumStrings:
				case UniversalType.ThingClass:
					if ((string)uv1.Value == (string)uv2.Value)
						return true;
					break;
			}

			return false;
		}

		/// <summary>
		/// Checks if the given sector has UDMF fields that have associations
		/// </summary>
		/// <param name="sector">Sector to check</param>
		/// <returns>True if the sector has UDMF fiels associations</returns>
		public static bool SectorHasUDMFFieldAssociations(Sector sector)
		{
			if (sector == null || sector.IsDisposed || sector.Fields == null)
				return false;

			foreach (UniversalFieldInfo ufi in General.Map.Config.SectorFields)
			{
				if (sector.Fields.ContainsKey(ufi.Name))
					return true;
			}

			return false;
		}

		/// <summary>
		/// Renders associated things and sectors in the indication color.
		/// Also renders event lines, if that option is enabled
		/// </summary>
		public void Render()
		{
			bool showlabels = BuilderPlug.Me.EventLineLabelVisibility > 0; // Show labels at all?

			foreach (Thing t in things)
				renderer.RenderThing(t, General.Colors.Indication, General.Settings.ActiveThingsAlpha);

			// There must be a better way to do this
			foreach(Sector s in sectors)
			{
				int highlightedColor = General.Colors.Highlight.WithAlpha(128).ToInt();
				FlatVertex[] verts = new FlatVertex[s.FlatVertices.Length];
				s.FlatVertices.CopyTo(verts, 0);
				for (int i = 0; i < verts.Length; i++) verts[i].c = highlightedColor;
				renderer.RenderGeometry(verts, null, true);
			}

			if (General.Settings.GZShowEventLines)
			{
				List<Line3D> lines = new List<Line3D>(eventlines.Count);
				List<ITextLabel> labels = new List<ITextLabel>(eventlines.Count);

				foreach(KeyValuePair<string, List<Line3D>> kvp in eventlines)
				{
					bool emptylabel = string.IsNullOrEmpty(kvp.Key); // Can be true if only either forward or reverse labels are shown
					List<Vector2D> allpositions = new List<Vector2D>(kvp.Value.Count);

					foreach (Line3D line in kvp.Value)
					{
						if (showlabels && !emptylabel)
							allpositions.Add(GetLabelPosition(line.Start, line.End));

						lines.Add(line);
					}

					if (showlabels && !emptylabel)
					{
						List<Vector2D> positions = MergePositions(allpositions, textwidths[kvp.Key]);
						int labelcounter = 0;

						// Set the position of the pre-generated labels. Only add the labels that are needed
						foreach (Vector2D pos in positions)
						{
							textlabels[kvp.Key][labelcounter].Location = pos;
							labels.Add(textlabels[kvp.Key][labelcounter]);
							labelcounter++;
						}
					}
				}

				renderer.RenderArrows(lines);

				if (showlabels)
					renderer.RenderText(labels);
			}
		}

		/// <summary>
		/// Plots associated linedefs and sectors
		/// </summary>
		public void Plot()
		{
			foreach(Linedef ld in linedefs)
				if(!ld.IsDisposed) renderer.PlotLinedef(ld, General.Colors.Indication);

			foreach (Sector s in sectors)
				if(!s.IsDisposed) renderer.PlotSector(s, General.Colors.Indication);
		}

		// This compares an association
		public static bool operator ==(Association a, Association b)
		{
			if(!(a is Association) || !(b is Association)) return false; //mxd
			return (a.type == b.type) && a.tags.SetEquals(b.tags);
		}

		// This compares an association
		public static bool operator !=(Association a, Association b)
		{
			if(!(a is Association) || !(b is Association)) return true; //mxd
			return (a.type != b.type) || !a.tags.SetEquals(b.tags);
		}

		//mxd 
		public override int GetHashCode() 
		{
			return base.GetHashCode();
		}

		//mxd
		public override bool Equals(object obj) 
		{
			if(!(obj is Association)) return false;

			Association b = (Association)obj;
			return (type == b.type) && tags.SetEquals(b.tags);
		}
	}
}
