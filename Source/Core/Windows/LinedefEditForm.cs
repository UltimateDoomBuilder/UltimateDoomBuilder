
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
using System.Windows.Forms;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Types;

#endregion

namespace CodeImp.DoomBuilder.Windows
{
	internal partial class LinedefEditForm : DelayedForm
	{
		#region ================== Events

		public event EventHandler OnValuesChanged; //mxd

		#endregion

		#region ================== Variables

		private ICollection<Linedef> lines;
		private List<LinedefProperties> linedefprops; //mxd
		private bool preventchanges;
		private bool undocreated; //mxd
		private bool oldmapischanged;

		private struct LinedefProperties //mxd
		{
			public readonly Dictionary<string, bool> Flags;
			public readonly SidedefProperties Front;
			public readonly SidedefProperties Back;

			public LinedefProperties(Linedef line) 
			{
				Front = (line.Front != null ? new SidedefProperties(line.Front) : null);
				Back = (line.Back != null ? new SidedefProperties(line.Back) : null);
				Flags = line.GetFlags();
			}
		}

		private class SidedefProperties //mxd
		{
			public readonly int OffsetX;
			public readonly int OffsetY;

			public readonly string HighTexture;
			public readonly string MiddleTexture;
			public readonly string LowTexture;

			public SidedefProperties(Sidedef side) 
			{
				// Offset
				OffsetX = side.OffsetX;
				OffsetY = side.OffsetY;

				// Textures
				HighTexture = side.HighTexture;
				MiddleTexture = side.MiddleTexture;
				LowTexture = side.LowTexture;
			}
		}

		#endregion

		#region ================== Constructor

		public LinedefEditForm()
		{
			// Initialize
			InitializeComponent();
			
			// Fill flags lists
			foreach(KeyValuePair<string, string> lf in General.Map.Config.LinedefFlags)
				flags.Add(lf.Value, lf.Key);

			// Fill actions list
			action.GeneralizedCategories = General.Map.Config.GenActionCategories;
			action.AddInfo(General.Map.Config.SortedLinedefActions.ToArray());

			// Fill activations list
			activation.Items.AddRange(General.Map.Config.LinedefActivates.ToArray());
			
			// Initialize image selectors
			fronthigh.Initialize();
			frontmid.Initialize();
			frontlow.Initialize();
			backhigh.Initialize();
			backmid.Initialize();
			backlow.Initialize();
			
			// Mixed activations?
			if(General.Map.FormatInterface.HasPresetActivations)
				hexenpanel.Visible = true;
			
			// Action arguments?
			if(General.Map.FormatInterface.HasActionArgs) argscontrol.Visible = true;
			
			// Arrange panels
			if(!General.Map.FormatInterface.HasMixedActivations &&
					!General.Map.FormatInterface.HasActionArgs &&
					!General.Map.FormatInterface.HasPresetActivations)
			{
				actiongroup.Height = argscontrol.Top + argscontrol.Margin.Top; //mxd
			}
			
			// Arrange or hide Identification panel
			if(General.Map.FormatInterface.HasLinedefTag)
			{
				// Match position after the action group
				idgroup.Top = actiongroup.Bottom + actiongroup.Margin.Bottom + idgroup.Margin.Top;
				panel.Height = idgroup.Bottom + idgroup.Margin.Bottom * 2;
			}
			else
			{
				idgroup.Visible = false;
				panel.Height = actiongroup.Bottom + actiongroup.Margin.Bottom * 2;
			}

			// Arrange Apply/Cancel buttons
			apply.Top = panel.Bottom + panel.Margin.Bottom + apply.Margin.Top;
			cancel.Top = apply.Top;

			// Update window height
			this.Height = apply.Bottom + apply.Margin.Bottom * 2 + (this.Height - this.ClientRectangle.Height) + 1;
		}

		#endregion

		#region ================== Methods

		// This sets up the form to edit the given lines
		public void Setup(ICollection<Linedef> lines)
		{
			preventchanges = true;
			oldmapischanged = General.Map.IsChanged;

            argscontrol.Reset();
            undocreated = false;
            // Keep this list
            this.lines = lines;
			if(lines.Count > 1) this.Text = "Edit Linedefs (" + lines.Count + ")";
			linedefprops = new List<LinedefProperties>();
			
			////////////////////////////////////////////////////////////////////////
			// Set all options to the first linedef properties
			////////////////////////////////////////////////////////////////////////

			// Get first line
			Linedef fl = General.GetByIndex(lines, 0);
			
			// Flags
			foreach(CheckBox c in flags.Checkboxes)
				if(fl.Flags.ContainsKey(c.Tag.ToString())) c.Checked = fl.Flags[c.Tag.ToString()];
			
			// Activations
			foreach(LinedefActivateInfo ai in activation.Items)
				if((fl.Activate & ai.Index) == ai.Index) activation.SelectedItem = ai;

			// Action/tags. We have to make sure the action control is initialized properly even when the default value is set
			if (action.Value != fl.Action)
				action.Value = fl.Action;
			else
				action_ValueChanges(action, EventArgs.Empty);

			if (General.Map.FormatInterface.HasLinedefTag) //mxd
			{
				tagSelector.Setup(UniversalType.LinedefTag);
				tagSelector.SetTag(fl.Tag);
			}

			//mxd. Args
			argscontrol.SetValue(fl, true);
			
			// Front side and back side checkboxes
			frontside.Checked = (fl.Front != null);
			#if MONO_WINFORMS
			frontgroup.Enabled = (fl.Front != null);
			#endif
			backside.Checked = (fl.Back != null);
			#if MONO_WINFORMS
			backgroup.Enabled = (fl.Back != null);
			#endif

			// Front settings
			if(fl.Front != null)
			{
				fronthigh.TextureName = fl.Front.HighTexture;
				frontmid.TextureName = fl.Front.MiddleTexture;
				frontlow.TextureName = fl.Front.LowTexture;
				fronthigh.Required = fl.Front.HighRequired();
				frontmid.Required = fl.Front.MiddleRequired();
				frontlow.Required = fl.Front.LowRequired();
				frontsector.Text = fl.Front.Sector.Index.ToString();
				frontTextureOffset.SetValues(fl.Front.OffsetX, fl.Front.OffsetY, true); //mxd
			}

			// Back settings
			if(fl.Back != null)
			{
				backhigh.TextureName = fl.Back.HighTexture;
				backmid.TextureName = fl.Back.MiddleTexture;
				backlow.TextureName = fl.Back.LowTexture;
				backhigh.Required = fl.Back.HighRequired();
				backmid.Required = fl.Back.MiddleRequired();
				backlow.Required = fl.Back.LowRequired();
				backsector.Text = fl.Back.Sector.Index.ToString();
				backTextureOffset.SetValues(fl.Back.OffsetX, fl.Back.OffsetY, true); //mxd
			}

			////////////////////////////////////////////////////////////////////////
			// Now go for all lines and change the options when a setting is different
			////////////////////////////////////////////////////////////////////////

			// Go for all lines
			foreach(Linedef l in lines)
			{
				// Flags
				foreach(CheckBox c in flags.Checkboxes)
				{
					if(c.CheckState == CheckState.Indeterminate) continue; //mxd
					if(l.IsFlagSet(c.Tag.ToString()) != c.Checked) 
					{
						c.ThreeState = true;
						c.CheckState = CheckState.Indeterminate;
					}
				}

				// Activations
				if(activation.Items.Count > 0)
				{
					LinedefActivateInfo sai = (activation.Items[0] as LinedefActivateInfo);
					foreach(LinedefActivateInfo ai in activation.Items)
						if((l.Activate & ai.Index) == ai.Index) sai = ai;
					if(sai != activation.SelectedItem) activation.SelectedIndex = -1;
				}

				// Action/tags
				if(l.Action != action.Value) action.Empty = true;
				if(General.Map.FormatInterface.HasLinedefTag && l.Tag != fl.Tag) tagSelector.ClearTag(); //mxd

				//mxd. Arguments
				argscontrol.SetValue(l, false);
				
				// Front side checkbox
				if((l.Front != null) != frontside.Checked)
				{
					frontside.ThreeState = true;
					frontside.CheckState = CheckState.Indeterminate;
					frontside.AutoCheck = false;
				}

				// Back side checkbox
				if((l.Back != null) != backside.Checked)
				{
					backside.ThreeState = true;
					backside.CheckState = CheckState.Indeterminate;
					backside.AutoCheck = false;
				}

				// Front settings
				if(l.Front != null)
				{
					//mxd
					if(!string.IsNullOrEmpty(fronthigh.TextureName) && fronthigh.TextureName != l.Front.HighTexture) 
					{
						if(!fronthigh.Required && l.Front.HighRequired()) fronthigh.Required = true;
						fronthigh.MultipleTextures = true;
						fronthigh.TextureName = string.Empty;
					}
					if(!string.IsNullOrEmpty(frontmid.TextureName) && frontmid.TextureName != l.Front.MiddleTexture) 
					{
						if(!frontmid.Required && l.Front.MiddleRequired()) frontmid.Required = true;
						frontmid.MultipleTextures = true;
						frontmid.TextureName = string.Empty;
					}
					if(!string.IsNullOrEmpty(frontlow.TextureName) && frontlow.TextureName != l.Front.LowTexture) 
					{
						if(!frontlow.Required && l.Front.LowRequired()) frontlow.Required = true;
						frontlow.MultipleTextures = true;
						frontlow.TextureName = string.Empty;
					}
					if(frontsector.Text != l.Front.Sector.Index.ToString()) frontsector.Text = string.Empty;

					frontTextureOffset.SetValues(l.Front.OffsetX, l.Front.OffsetY, false); //mxd
				}

				// Back settings
				if(l.Back != null)
				{
					//mxd
					if(!string.IsNullOrEmpty(backhigh.TextureName) && backhigh.TextureName != l.Back.HighTexture) 
					{
						if(!backhigh.Required && l.Back.HighRequired()) backhigh.Required = true;
						backhigh.MultipleTextures = true;
						backhigh.TextureName = string.Empty;
					}
					if(!string.IsNullOrEmpty(backmid.TextureName) && backmid.TextureName != l.Back.MiddleTexture) 
					{
						if(!backmid.Required && l.Back.MiddleRequired()) backmid.Required = true;
						backmid.MultipleTextures = true;
						backmid.TextureName = string.Empty;
					}
					if(!string.IsNullOrEmpty(backlow.TextureName) && backlow.TextureName != l.Back.LowTexture) 
					{
						if(!backlow.Required && l.Back.LowRequired()) backlow.Required = true;
						backlow.MultipleTextures = true;
						backlow.TextureName = string.Empty;
					}
					if(backsector.Text != l.Back.Sector.Index.ToString()) backsector.Text = string.Empty;

					backTextureOffset.SetValues(l.Back.OffsetX, l.Back.OffsetY, false); //mxd
				}

				//mxd
				linedefprops.Add(new LinedefProperties(l));
			}
			
			// Refresh controls so that they show their image
			backhigh.Refresh();
			backmid.Refresh();
			backlow.Refresh();
			fronthigh.Refresh();
			frontmid.Refresh();
			frontlow.Refresh();

			preventchanges = false;

			argscontrol.UpdateScriptControls(); //mxd
			actionhelp.UpdateAction(action.GetValue()); //mxd
		}

		//mxd
		private void MakeUndo() 
		{
			if(undocreated) return;
			undocreated = true;

			//mxd. Make undo
			General.Map.UndoRedo.CreateUndo("Edit " + (lines.Count > 1 ? lines.Count + " linedefs" : "linedef"));
		}

		#endregion

		#region ================== Events

		// Apply clicked
		private void apply_Click(object sender, EventArgs e)
		{
			// Verify the tag
			if(General.Map.FormatInterface.HasLinedefTag)
			{
				tagSelector.ValidateTag(); //mxd
				if(((tagSelector.GetTag(0) < General.Map.FormatInterface.MinTag) || (tagSelector.GetTag(0) > General.Map.FormatInterface.MaxTag))) 
				{
					General.ShowWarningMessage("Linedef tag must be between " + General.Map.FormatInterface.MinTag + " and " + General.Map.FormatInterface.MaxTag + ".", MessageBoxButtons.OK);
					return;
				}
			}
			
			// Verify the action
			if((action.Value < General.Map.FormatInterface.MinAction) || (action.Value > General.Map.FormatInterface.MaxAction))
			{
				General.ShowWarningMessage("Linedef action must be between " + General.Map.FormatInterface.MinAction + " and " + General.Map.FormatInterface.MaxAction + ".", MessageBoxButtons.OK);
				return;
			}

			MakeUndo(); //mxd
			
			// Go for all the lines
			int offset = 0; //mxd
			foreach(Linedef l in lines)
			{
				// Apply chosen activation flag
				if(activation.SelectedIndex > -1)
					l.Activate = (activation.SelectedItem as LinedefActivateInfo).Index;
				
				// Action/tags
				l.Tag = General.Clamp(tagSelector.GetSmartTag(l.Tag, offset), General.Map.FormatInterface.MinTag, General.Map.FormatInterface.MaxTag); //mxd
				if(!action.Empty) l.Action = action.Value;

				//mxd. Apply args
				argscontrol.Apply(l, offset);
				
				// Remove front side?
				if((l.Front != null) && (frontside.CheckState == CheckState.Unchecked))
				{
					l.Front.Dispose();
				}
				// Create or modify front side?
				else if(frontside.CheckState == CheckState.Checked)
				{
					// Make sure we have a valid sector (make a new one if needed)
					int index = (l.Front != null ? l.Front.Sector.Index : -1);
					index = frontsector.GetResult(index);
					if((index > -1) && (index < General.Map.Map.Sectors.Count))
					{
						Sector s = (General.Map.Map.GetSectorByIndex(index) ?? General.Map.Map.CreateSector());
						if(s != null)
						{
							// Create new sidedef?
							if(l.Front == null) General.Map.Map.CreateSidedef(l, true, s);

							// Change sector?
							if(l.Front != null && l.Front.Sector != s) l.Front.SetSector(s);
						}
					}
				}

				// Remove back side?
				if((l.Back != null) && (backside.CheckState == CheckState.Unchecked))
				{
					l.Back.Dispose();
				}
				// Create or modify back side?
				else if(backside.CheckState == CheckState.Checked)
				{
					// Make sure we have a valid sector (make a new one if needed)
					int index = (l.Back != null ? l.Back.Sector.Index : -1);
					index = backsector.GetResult(index);
					if((index > -1) && (index < General.Map.Map.Sectors.Count))
					{
						Sector s = (General.Map.Map.GetSectorByIndex(index) ?? General.Map.Map.CreateSector());
						if(s != null)
						{
							// Create new sidedef?
							if(l.Back == null) General.Map.Map.CreateSidedef(l, false, s);
							
							// Change sector?
							if(l.Back != null && l.Back.Sector != s) l.Back.SetSector(s);
						}
					}
				}

				//mxd. Increase offset...
				offset++;
			}

			// Update the used textures
			General.Map.Data.UpdateUsedTextures();
			
			// Done
			General.Map.IsChanged = true;
			if(OnValuesChanged != null)	OnValuesChanged(this, EventArgs.Empty); //mxd
			this.DialogResult = DialogResult.OK;
			this.Close();
		}

		// Cancel clicked
		private void cancel_Click(object sender, EventArgs e)
		{
			//mxd. Let's pretend nothing of this really happened...
			if (undocreated)
			{
				General.Map.UndoRedo.WithdrawUndo();

				// Changing certain properties of the linedef, like textures will set General.Map.IsChanged to true.
				// But if cancel is pressed and the changes are discarded, and the map was not changed before, we have to force
				// General.Map.IsChanged back to false
				if (General.Map.IsChanged && oldmapischanged == false)
					General.Map.ForceMapIsChangedFalse();
			}
			
			// Be gone
			this.DialogResult = DialogResult.Cancel;
			this.Close();
		}

		// Front side (un)checked
		private void frontside_CheckStateChanged(object sender, EventArgs e) 
		{
			// Enable/disable panel
			// NOTE: Also enabled when checkbox is grayed!
			frontgroup.Enabled = (frontside.CheckState != CheckState.Unchecked);
		}

		// Back side (un)checked
		private void backside_CheckStateChanged(object sender, EventArgs e) 
		{
			// Enable/disable panel
			// NOTE: Also enabled when checkbox is grayed!
			backgroup.Enabled = (backside.CheckState != CheckState.Unchecked);
		}

		// Action changes
		private void action_ValueChanges(object sender, EventArgs e)
		{
			int showaction = 0;
			
			// Only when line type is known
			if(General.Map.Config.LinedefActions.ContainsKey(action.Value)) showaction = action.Value;
			
			//mxd. Change the argument descriptions
			argscontrol.UpdateAction(showaction, preventchanges);

			if(!preventchanges) 
			{
				MakeUndo(); //mxd

				//mxd. Update what must be updated
				argscontrol.UpdateScriptControls();
				actionhelp.UpdateAction(showaction);
			} 
		}

		// Browse Action clicked
		private void browseaction_Click(object sender, EventArgs e)
		{
			action.Value = ActionBrowserForm.BrowseAction(this, action.Value);
		}

		// Help!
		private void LinedefEditForm_HelpRequested(object sender, HelpEventArgs hlpevent)
		{
			General.ShowHelp("w_linedefedit.html");
			hlpevent.Handled = true;
		}

		#endregion

		#region ================== Linedef realtime events (mxd)

		private void flags_OnValueChanged(object sender, EventArgs e) 
		{
			if(preventchanges) return;
			MakeUndo(); //mxd
			int i = 0;

			foreach(Linedef l in lines) 
			{
				// Apply all flags
				foreach(CheckBox c in flags.Checkboxes) 
				{
					if(c.CheckState == CheckState.Checked)
						l.SetFlag(c.Tag.ToString(), true);
					else if(c.CheckState == CheckState.Unchecked)
						l.SetFlag(c.Tag.ToString(), false);
					else if(linedefprops[i].Flags.ContainsKey(c.Tag.ToString()))
						l.SetFlag(c.Tag.ToString(), linedefprops[i].Flags[c.Tag.ToString()]);
					else //linedefs created in the editor have empty Flags by default
						l.SetFlag(c.Tag.ToString(), false);
				}

				i++;
			}
			
			General.Map.IsChanged = true;
			if(OnValuesChanged != null)	OnValuesChanged(this, EventArgs.Empty);
		}

		#endregion

		#region ================== Sidedef reltime events (mxd)

		private void fronthigh_OnValueChanged(object sender, EventArgs e) 
		{
			if(preventchanges) return;
			MakeUndo();

			// Restore values
			if(string.IsNullOrEmpty(fronthigh.TextureName)) 
			{
				int i = 0;
				foreach(Linedef l in lines) 
				{
					if(l.Front != null) l.Front.SetTextureHigh(linedefprops[i].Front != null ? linedefprops[i].Front.HighTexture : "-");
					i++;
				}
			}
			// Update values
			else
			{
				foreach(Linedef l in lines)
				{
					if(l.Front != null) l.Front.SetTextureHigh(fronthigh.GetResult(l.Front.HighTexture));
				}
			}

			// Update the used textures
			General.Map.Data.UpdateUsedTextures();

			General.Map.IsChanged = true;
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
		}

		private void frontmid_OnValueChanged(object sender, EventArgs e) 
		{
			if(preventchanges) return;
			MakeUndo();

			// Restore values
			if(string.IsNullOrEmpty(frontmid.TextureName)) 
			{
				int i = 0;
				foreach(Linedef l in lines) 
				{
					if(l.Front != null) l.Front.SetTextureMid(linedefprops[i].Front != null ? linedefprops[i].Front.MiddleTexture : "-");
					i++;
				}
			}
			// Update values
			else
			{
				foreach(Linedef l in lines)
				{
					if(l.Front != null) l.Front.SetTextureMid(frontmid.GetResult(l.Front.MiddleTexture));
				}
			}

			// Update the used textures
			General.Map.Data.UpdateUsedTextures();

			General.Map.IsChanged = true;
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
		}

		private void frontlow_OnValueChanged(object sender, EventArgs e) 
		{
			if(preventchanges) return;
			MakeUndo();

			// Restore values
			if(string.IsNullOrEmpty(frontlow.TextureName)) 
			{
				int i = 0;
				foreach(Linedef l in lines) 
				{
					if(l.Front != null) l.Front.SetTextureLow(linedefprops[i].Front != null ? linedefprops[i].Front.LowTexture : "-");
					i++;
				}
			}
			// Update values
			else
			{
				foreach(Linedef l in lines)
				{
					if(l.Front != null) l.Front.SetTextureLow(frontlow.GetResult(l.Front.LowTexture));
				}
			}

			// Update the used textures
			General.Map.Data.UpdateUsedTextures();

			General.Map.IsChanged = true;
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
		}

		private void backhigh_OnValueChanged(object sender, EventArgs e) 
		{
			if(preventchanges) return;
			MakeUndo();

			// Restore values
			if(string.IsNullOrEmpty(backhigh.TextureName)) 
			{
				int i = 0;
				foreach(Linedef l in lines) 
				{
					if(l.Back != null) l.Back.SetTextureHigh(linedefprops[i].Back != null ? linedefprops[i].Back.HighTexture : "-");
					i++;
				}
			}
			// Update values
			else
			{
				foreach(Linedef l in lines)
				{
					if(l.Back != null) l.Back.SetTextureHigh(backhigh.GetResult(l.Back.HighTexture));
				}
			}

			// Update the used textures
			General.Map.Data.UpdateUsedTextures();

			General.Map.IsChanged = true;
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
		}

		private void backmid_OnValueChanged(object sender, EventArgs e) 
		{
			if(preventchanges) return;
			MakeUndo();

			// Restore values
			if(string.IsNullOrEmpty(backmid.TextureName)) 
			{
				int i = 0;
				foreach(Linedef l in lines) 
				{
					if(l.Back != null) l.Back.SetTextureMid(linedefprops[i].Back != null ? linedefprops[i].Back.MiddleTexture : "-");
					i++;
				}
			}
			// Update values
			else 
			{
				foreach(Linedef l in lines)
				{
					if(l.Back != null) l.Back.SetTextureMid(backmid.GetResult(l.Back.MiddleTexture));
				}
			}

			// Update the used textures
			General.Map.Data.UpdateUsedTextures();

			General.Map.IsChanged = true;
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
		}

		private void backlow_OnValueChanged(object sender, EventArgs e) 
		{
			if(preventchanges) return;
			MakeUndo();

			// Restore values
			if(string.IsNullOrEmpty(backlow.TextureName)) 
			{
				int i = 0;
				foreach(Linedef l in lines) 
				{
					if(l.Back != null) l.Back.SetTextureLow(linedefprops[i].Back != null ? linedefprops[i].Back.LowTexture : "-");
					i++;
				}
			}
			// Update values
			else
			{
				foreach(Linedef l in lines)
				{
					if(l.Back != null) l.Back.SetTextureLow(backlow.GetResult(l.Back.LowTexture));
				}
			}

			// Update the used textures
			General.Map.Data.UpdateUsedTextures();

			General.Map.IsChanged = true;
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
		}

		private void frontTextureOffset_OnValuesChanged(object sender, EventArgs e) 
		{
			if(preventchanges) return;
			MakeUndo(); //mxd
			int i = 0;

			// Reset increment steps, otherwise it's just keep counting and counting
			frontTextureOffset.ResetIncrementStep();

			foreach (Linedef l in lines) 
			{
				if(l.Front != null) 
				{
					if(linedefprops[i].Front != null) 
					{
						l.Front.OffsetX = frontTextureOffset.GetValue1(linedefprops[i].Front.OffsetX);
						l.Front.OffsetY = frontTextureOffset.GetValue2(linedefprops[i].Front.OffsetY);
					} 
					else 
					{
						l.Front.OffsetX = frontTextureOffset.GetValue1(0);
						l.Front.OffsetY = frontTextureOffset.GetValue2(0);
					}
				}

				i++;
			}

			General.Map.IsChanged = true;
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
		}

		private void backTextureOffset_OnValuesChanged(object sender, EventArgs e) 
		{
			if(preventchanges) return;
			MakeUndo(); //mxd
			int i = 0;

			// Reset increment steps, otherwise it's just keep counting and counting
			backTextureOffset.ResetIncrementStep();

			foreach (Linedef l in lines) 
			{
				if(l.Back != null) 
				{
					if(linedefprops[i].Back != null) 
					{
						l.Back.OffsetX = backTextureOffset.GetValue1(linedefprops[i].Back.OffsetX);
						l.Back.OffsetY = backTextureOffset.GetValue2(linedefprops[i].Back.OffsetY);
					} 
					else
					{
						l.Back.OffsetX = backTextureOffset.GetValue1(0);
						l.Back.OffsetY = backTextureOffset.GetValue2(0);
					}
				}

				i++;
			}
			
			General.Map.IsChanged = true;
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
		}

		#endregion

	}
}
