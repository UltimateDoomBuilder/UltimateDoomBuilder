#region ================== Namespaces

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Types;

#endregion

namespace CodeImp.DoomBuilder.Controls
{
	public partial class TagsSelector : UserControl
	{
		#region ================== Structs

		private struct TagLinkData
		{
			public int Index;
			public int Tag;
		}

		#endregion

		#region ================== Variables

		private List<int> usedtags; // Tags already used in the map
		private List<TagInfo> infos;
		private List<List<int>> tagspermapelement; // One list per each map element
		private List<int> rangemodes;  // 0 - none, 1 - positive (>=), -1 - negative (<=)
		private List<int> offsetmodes; // 0 - none, 1 - positive (++), -1 - negative (--)
		private UniversalType elementtype;
		private const string TAGS_SEPARATOR = ", ";
		private int curtagindex;
		private bool blockupdate;

		#endregion

		#region ================== Constructor

		public TagsSelector()
		{
			InitializeComponent();
            Reset();
		}

		#endregion

		#region ================== Setup

		public void SetValues(ICollection<Sector> sectors)
		{
			List<IMultiTaggedMapElement> taglist = new List<IMultiTaggedMapElement>(sectors.Count);
			foreach(Sector s in sectors) taglist.Add(s);
			SetValues(taglist);
		}

		public void SetValues(ICollection<Linedef> lines)
		{
			List<IMultiTaggedMapElement> taglist = new List<IMultiTaggedMapElement>(lines.Count);
			foreach(Linedef l in lines) taglist.Add(l);
			SetValues(taglist);
		}

		private void SetValues(ICollection<IMultiTaggedMapElement> elements)
		{
			// Initial setup
			IMultiTaggedMapElement first = General.GetByIndex(elements, 0);
			if(first is Linedef)
				Setup(UniversalType.LinedefTag);
			else if(first is Sector)
				Setup(UniversalType.SectorTag);
			else
				throw new NotSupportedException(first + " doesn't support 'moreids' property!");

			// Create tags collection
			int maxtagscount = 0;
			foreach(IMultiTaggedMapElement me in elements)
			{
				tagspermapelement.Add(new List<int>(me.Tags));
				if(me.Tags.Count > maxtagscount) maxtagscount = me.Tags.Count;
			}
			
			// Make all lists the same length
			foreach(List<int> l in tagspermapelement)
			{
				if(l.Count < maxtagscount)
					for(int i = l.Count; i < maxtagscount; i++) l.Add(int.MaxValue);
			}

			// Update collections
			List<int> tags = GetDisplayTags();

			// Initialize modifier modes
			for(int i = 0; i < tags.Count; i++)
			{
				rangemodes.Add(0);
				offsetmodes.Add(0);
			}

			// Update controls
			UpdateTagPicker(tags[0]);
			UpdateTagsList(tags);
			removetag.Enabled = (tags.Count > 1);
			clear.Enabled = (tagpicker.Text.Trim() != "0");
		}

		private void Setup(UniversalType mapelementtype)
		{
			elementtype = mapelementtype;

			// Collect used tags from appropriate element type...
			switch(elementtype)
			{
				case UniversalType.SectorTag:
					foreach(Sector s in General.Map.Map.Sectors)
					{
						foreach(int tag in s.Tags)
						{
							if(tag == 0 || usedtags.Contains(tag)) continue;
							usedtags.Add(tag);
						}
					}
					break;

				case UniversalType.LinedefTag:
					if(General.Map.FormatInterface.HasLinedefTag)
					{
						foreach(Linedef l in General.Map.Map.Linedefs)
						{
							foreach(int tag in l.Tags)
							{
								if(tag == 0 || usedtags.Contains(tag)) continue;
								usedtags.Add(tag);
							}
						}
					}
					break;
			}

			// Now sort them in descending order
			usedtags.Sort((a, b) => -1 * a.CompareTo(b));

			// Create tag infos
			foreach(int tag in usedtags)
			{
				if(General.Map.Options.TagLabels.ContainsKey(tag)) // Tag labels
					infos.Add(new TagInfo(tag, General.Map.Options.TagLabels[tag]));
				else
					infos.Add(new TagInfo(tag, string.Empty));
			}
			foreach(TagInfo info in infos) tagpicker.Items.Add(info);
			tagpicker.DropDownWidth = DoomBuilder.Geometry.Tools.GetDropDownWidth(tagpicker);
		}

		#endregion

		#region ================== Apply

		public void ApplyTo(ICollection<Sector> sectors)
		{
			List<IMultiTaggedMapElement> taglist = new List<IMultiTaggedMapElement>(sectors.Count);
			foreach(Sector s in sectors) taglist.Add(s);
			ApplyTo(taglist);
		}

		public void ApplyTo(ICollection<Linedef> lines)
		{
			List<IMultiTaggedMapElement> taglist = new List<IMultiTaggedMapElement>(lines.Count);
			foreach(Linedef l in lines) taglist.Add(l);
			ApplyTo(taglist);
		}

		private void ApplyTo(IEnumerable<IMultiTaggedMapElement> elements)
		{
			int offset = 0;
			foreach(IMultiTaggedMapElement me in elements)
			{
				// Create resulting tags list for this map element
				List<int> tags = tagspermapelement[offset];
				HashSet<int> newtags = new HashSet<int>();
				for(int i = 0; i < tags.Count; i++)
				{
					if(tags[i] == int.MaxValue) continue; // int.MaxValue is there only for padding
					if(tags[i] == int.MinValue && me.Tags.Count > i)
					{
						if(me.Tags[i] != 0 && !newtags.Contains(me.Tags[i])) newtags.Add(me.Tags[i]);
					}
					else if(tags[i] != 0 && tags[i] != int.MinValue)
					{
						int tag;
						if(rangemodes[i] != 0)
							tag = tags[i] + offset * rangemodes[i];
						else if(offsetmodes[i] != 0 && me.Tags.Count > i)
							tag = me.Tags[i] + tags[i] * offsetmodes[i];
						else
							tag = tags[i];

						if(!newtags.Contains(tag)) newtags.Add(tag);
					}
				}

				if(newtags.Count == 0) newtags.Add(0);

				// Apply it
				me.Tags = new List<int>(newtags);

				// We are making progress...
				offset++;
			}
		}

        #endregion

        #region ================== Methods
        public void Reset()
        {
            tagspermapelement = new List<List<int>>();
            usedtags = new List<int>();
            rangemodes = new List<int>();
            offsetmodes = new List<int>();
            infos = new List<TagInfo>();
            tagslist.Links.Clear();
            infos.Clear();
            tagpicker.Items.Clear();
            usedtags.Clear();
        }

		// Creates a single tag collection to display. int.MinValue means "mixed tag"
		private List<int> GetDisplayTags()
		{
			List<int> tags = new List<int>(tagspermapelement[0].Count);

			// Padding values should stay in tagspermapelement
			foreach(int tag in tagspermapelement[0])
			{
				tags.Add(tag == int.MaxValue ? int.MinValue : tag);
			}
			
			for(int i = 1; i < tagspermapelement.Count; i++)
			{
				// Check mixed values
				for(int c = 0; c < tagspermapelement[i].Count; c++)
				{
					if(tagspermapelement[i][c] != tags[c])
						tags[c] = int.MinValue;
				}
			}

			return tags;
		}

		private void UpdateTagsList(List<int> tags)
		{
			string[] displaytags = new string[tags.Count];
			int displaytagslen = 0;
            tagslist.Links.Clear();

            // Gather tags into a single string collection
            for (int i = 0; i < tags.Count; i++)
			{
				displaytags[i] = (tags[i] == int.MinValue ? "???" : tags[i].ToString());

				// Add modify mode markers
				if(offsetmodes[i] == -1) displaytags[i] = "--" + displaytags[i];
				else if(offsetmodes[i] == 1) displaytags[i] = "++" + displaytags[i];
				else if(rangemodes[i] == -1) displaytags[i] = "<=" + displaytags[i];
				else if(rangemodes[i] == 1) displaytags[i] = ">=" + displaytags[i];

				// Add selection indictor
				if(curtagindex == i) displaytags[i] = "[" + displaytags[i] + "]";
				else displaytags[i] = " " + displaytags[i] + " ";

				int start = displaytagslen + i * TAGS_SEPARATOR.Length;
				tagslist.Links.Add(new LinkLabel.Link(start, displaytags[i].Length, new TagLinkData { Index = i, Tag = tags[i] }));
				displaytagslen += displaytags[i].Length;
			}

			// Create label text
			tagslist.Text = string.Join(TAGS_SEPARATOR, displaytags);

			// Update current tag label
			curtaglabel.SetLeftExpandText("Tag " + (curtagindex + 1) + ":");
		}

		private void UpdateTagPicker(int tag)
		{
			blockupdate = true;

			tagpicker.SelectedIndex = -1;

			if(tag == int.MinValue)
			{
				tagpicker.Text = string.Empty;
			}
			else if(rangemodes[curtagindex] != 0 && tag != 0)
			{
				tagpicker.Text = (rangemodes[curtagindex] == 1 ? ">=" : "<=") + tag;
			}
			else if(offsetmodes[curtagindex] != 0 && tag != 0)
			{
				tagpicker.Text = (offsetmodes[curtagindex] == 1 ? "++" : "--") + tag;
			}
			else
			{
				foreach(var item in tagpicker.Items)
					if(((TagInfo)item).Tag == tag) tagpicker.SelectedItem = item;

				if(tagpicker.SelectedIndex == -1) tagpicker.Text = tag.ToString();
			}

			clear.Enabled = (tagpicker.Text.Trim() != "0");

			blockupdate = false;
		}

		#endregion

		#region ================== Events

		private void newtag_Click(object sender, EventArgs e)
		{
			tagpicker.SelectedIndex = -1;
			tagpicker.Text = General.Map.Map.GetNewTag().ToString();
		}

		private void unusedtag_Click(object sender, EventArgs e)
		{
			tagpicker.SelectedIndex = -1;
			tagpicker.Text = General.Map.Map.GetNewTag(elementtype).ToString();
		}

		private void clear_Click(object sender, EventArgs e)
		{
			tagpicker.Focus();
			tagpicker.SelectedIndex = -1;
			tagpicker.Text = "0";
			tagpicker.SelectAll();
		}

		private void addtag_Click(object sender, EventArgs e)
		{
			// When an item has no tags, act like "New Tag" button
			List<int> tags = GetDisplayTags();
			if(tags.Count == 1 && tags[0] == 0)
			{
				newtag_Click(sender, e);
				return;
			}
			
			int nt = General.Map.Map.GetNewTag(tags);
			
			// Add to displayed tags list
			tags.Add(nt);

			// Add to real tag lists
			foreach(List<int> l in tagspermapelement) l.Add(nt);

			rangemodes.Add(0);
			offsetmodes.Add(0);
			curtagindex = tags.Count - 1;

			// Update controls
			blockupdate = true;
			tagpicker.Text = nt.ToString();
			blockupdate = false;

			removetag.Enabled = true;
			UpdateTagsList(tags);
		}

		private void removetag_Click(object sender, EventArgs e)
		{
			List<int> tags = GetDisplayTags();

			// Remove from displayed tags list
			tags.RemoveAt(curtagindex);

			// Remove from real tag lists
			foreach(List<int> l in tagspermapelement) l.RemoveAt(curtagindex);

			rangemodes.RemoveAt(curtagindex);
			offsetmodes.RemoveAt(curtagindex);
			if(curtagindex >= tags.Count) curtagindex = tags.Count - 1;

			// Update controls
			UpdateTagPicker(tags[curtagindex]);

			removetag.Enabled = (tags.Count > 1);
			UpdateTagsList(tags);
		}

		private void clearalltags_Click(object sender, EventArgs e)
		{
			curtagindex = 0;

			// Clear real tag lists
			for(int i = 0; i < tagspermapelement.Count; i++)
				tagspermapelement[i] = new List<int> { 0 };

			// Clear collections
			rangemodes = new List<int> { 0 };
			offsetmodes = new List<int> { 0 };

			// Update controls
			blockupdate = true;
			tagpicker.Text = "0";
			blockupdate = false;

			removetag.Enabled = false;
			UpdateTagsList(new List<int> { 0 });
		}

		private void tagslist_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			TagLinkData data = (TagLinkData)e.Link.LinkData;
			curtagindex = data.Index;
			curtaglabel.SetLeftExpandText("Tag " + (curtagindex + 1) + ":");

			// Update interface
			UpdateTagPicker(data.Tag);
			UpdateTagsList(GetDisplayTags());
		}

		private void tagpicker_TextChanged(object sender, EventArgs e)
		{
			if(blockupdate) return;

			clear.Enabled = (tagpicker.Text.Trim() != "0");
			List<int> tags = GetDisplayTags();
			if(tagpicker.SelectedItem != null)
			{
				TagInfo info = (TagInfo)tagpicker.SelectedItem;
				
				// Set displayed tag
				tags[curtagindex] = info.Tag;

				// Apply to real tags
				foreach(List<int> l in tagspermapelement) l[curtagindex] = info.Tag;

				UpdateTagsList(tags);
				return;
			}

			string text = tagpicker.Text.Trim();
			if(string.IsNullOrEmpty(text))
			{
				// Set displayed tag
				tags[curtagindex] = int.MinValue;

				// Apply to real tags
				foreach(List<int> l in tagspermapelement) l[curtagindex] = int.MinValue;

				UpdateTagsList(tags);
				return;
			}

			// Incremental?
			int rangemode = 0;
			int offsetmode = 0;
			if(text.Length > 2)
			{
				if(text.StartsWith(">=")) // Range up
				{
					rangemode = 1;
					text = text.Substring(2, text.Length - 2);
				}
				else if(text.StartsWith("<=")) // Range down
				{
					rangemode = -1;
					text = text.Substring(2, text.Length - 2);
				}
				else if(text.StartsWith("++")) // Relative up
				{
					offsetmode = 1;
					text = text.Substring(2, text.Length - 2);
				}
				else if(text.StartsWith("--")) // Relative down
				{
					offsetmode = -1;
					text = text.Substring(2, text.Length - 2);
				}
			} 

			int tag;
			if(int.TryParse(text, out tag))
			{
				// Validate entered tag
				if((tag < General.Map.FormatInterface.MinTag) || (tag > General.Map.FormatInterface.MaxTag))
				{
					General.ShowWarningMessage("Sector tag must be between " + General.Map.FormatInterface.MinTag + " and " + General.Map.FormatInterface.MaxTag + ".", MessageBoxButtons.OK);
					return;
				}

				// Set displayed tag
				tags[curtagindex] = tag;

				// Apply to real tags
				foreach(List<int> l in tagspermapelement) l[curtagindex] = tag;

				rangemodes[curtagindex] = rangemode;
				offsetmodes[curtagindex] = offsetmode;
				UpdateTagsList(tags);
			}
		}

		//mxd. Because anchor-based alignment fails when using high-Dpi settings...
		private void TagsSelector_Resize(object sender, EventArgs e)
		{
			clear.Left = this.Width - clear.Width - clear.Margin.Right;
			unusedtag.Left = clear.Left - unusedtag.Margin.Right - unusedtag.Width;
			newtag.Left = unusedtag.Left - newtag.Margin.Right - newtag.Width;
			buttons.Left = newtag.Left - newtag.Margin.Left - buttons.Width;
			tagpicker.Width = buttons.Left - tagpicker.Margin.Right - tagpicker.Left;
			removetag.Left = clear.Left;
			addtag.Left = removetag.Left - addtag.Margin.Right - addtag.Width;
		}

		//mxd
		private void buttons_ValueChanged(object sender, EventArgs e)
		{
			if(buttons.Value == 0) return;
			
			int tag = 0;
			bool valid = false;

			// Get current tag
			if(tagpicker.SelectedItem != null)
			{
				TagInfo info = (TagInfo) tagpicker.SelectedItem;
				tag = info.Tag;
				valid = true;
			}
			else
			{
				string text = tagpicker.Text.Trim();
				if(!string.IsNullOrEmpty(text) && int.TryParse(text, out tag))
					valid = true;
			}

			// Increment it
			if(valid) tag = General.Clamp(tag + (buttons.Value < 0 ? 1 : -1), General.Map.FormatInterface.MinTag, General.Map.FormatInterface.MaxTag);

			// Apply it
			tagpicker.SelectedIndex = -1;
			tagpicker.Text = tag.ToString();
			buttons.Value = 0;
		}

		#endregion
	}
}