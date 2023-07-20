
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
using CodeImp.DoomBuilder.Controls;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Types;

#endregion

namespace CodeImp.DoomBuilder.Windows
{
	internal partial class LinedefEditFormUDMF : DelayedForm
	{
		#region ================== Events

		public event EventHandler OnValuesChanged; //mxd

		#endregion

		#region ================== Variables

		private ICollection<Linedef> lines;
		private List<LinedefProperties> linedefprops; //mxd
		private bool preventchanges;
		private bool undocreated; //mxd
		private readonly string[] renderstyles; //mxd
		private readonly List<int> keynumbers; //mxd 
		private readonly List<PairedFieldsControl> frontUdmfControls; //mxd
		private readonly List<PairedFieldsControl> backUdmfControls; //mxd
		private bool oldmapischanged;

		private struct LinedefProperties //mxd
		{
			public readonly Dictionary<string, bool> Flags;
			public readonly double Alpha;

			public readonly SidedefProperties Front;
			public readonly SidedefProperties Back;

			public LinedefProperties(Linedef line) 
			{
				Front = (line.Front != null ? new SidedefProperties(line.Front) : null);
				Back = (line.Back != null ? new SidedefProperties(line.Back) : null);
				Alpha = UniFields.GetFloat(line.Fields, "alpha", 1.0f);
				Flags = line.GetFlags();
			}
		}

		private class SidedefProperties //mxd
		{
			public readonly Dictionary<string, bool> Flags;

			public readonly double ScaleTopX;
			public readonly double ScaleTopY;
			public readonly double ScaleMidX;
			public readonly double ScaleMidY;
			public readonly double ScaleBottomX;
			public readonly double ScaleBottomY;

			public readonly int OffsetX;
			public readonly int OffsetY;

			public readonly double OffsetTopX;
			public readonly double OffsetTopY;
			public readonly double OffsetMidX;
			public readonly double OffsetMidY;
			public readonly double OffsetBottomX;
			public readonly double OffsetBottomY;

			public readonly int Brightness;
			public readonly bool AbsoluteBrightness;

			public readonly string HighTexture;
			public readonly string MiddleTexture;
			public readonly string LowTexture;

			public SidedefProperties(Sidedef side) 
			{
				Flags = side.GetFlags();

				// Offset
				OffsetX = side.OffsetX;
				OffsetY = side.OffsetY;

				Brightness = UniFields.GetInteger(side.Fields, "light", 0);
				AbsoluteBrightness = side.Fields.GetValue("lightabsolute", false);

				// Scale
				ScaleTopX = UniFields.GetFloat(side.Fields, "scalex_top", 1.0f);
				ScaleTopY = UniFields.GetFloat(side.Fields, "scaley_top", 1.0f);
				ScaleMidX = UniFields.GetFloat(side.Fields, "scalex_mid", 1.0f);
				ScaleMidY = UniFields.GetFloat(side.Fields, "scaley_mid", 1.0f);
				ScaleBottomX = UniFields.GetFloat(side.Fields, "scalex_bottom", 1.0f);
				ScaleBottomY = UniFields.GetFloat(side.Fields, "scaley_bottom", 1.0f);

				// Local offsets
				OffsetTopX = UniFields.GetFloat(side.Fields, "offsetx_top", 0f);
				OffsetTopY = UniFields.GetFloat(side.Fields, "offsety_top", 0f);
				OffsetMidX = UniFields.GetFloat(side.Fields, "offsetx_mid", 0f);
				OffsetMidY = UniFields.GetFloat(side.Fields, "offsety_mid", 0f);
				OffsetBottomX = UniFields.GetFloat(side.Fields, "offsetx_bottom", 0f);
				OffsetBottomY = UniFields.GetFloat(side.Fields, "offsety_bottom", 0f);

				// Textures
				HighTexture = side.HighTexture;
				MiddleTexture = side.MiddleTexture;
				LowTexture = side.LowTexture;
			}
		}

		#endregion

		#region ================== Constructor

		public LinedefEditFormUDMF(bool selectfront, bool selectback)
		{
			// Initialize
			InitializeComponent();

			// Widow setup
			if(General.Settings.StoreSelectedEditTab)
			{
				int activetab = General.Settings.ReadSetting("windows." + configname + ".activetab", 0);
				
				// When front or back tab was previously selected, switch to appropriate side (selectfront/selectback are set in BaseVisualGeometrySidedef.OnEditEnd)
				if((selectfront || selectback) && (activetab == 1 || activetab == 2))
					tabs.SelectTab(selectfront ? 1 : 2);
				else
					tabs.SelectTab(activetab);
			}
			
			// Fill flags lists
			foreach(KeyValuePair<string, string> lf in General.Map.Config.LinedefFlags)
				flags.Add(lf.Value, lf.Key);
			flags.Enabled = General.Map.Config.LinedefFlags.Count > 0;

			// Fill sidedef flags lists
			foreach(KeyValuePair<string, string> lf in General.Map.Config.SidedefFlags) 
			{
				flagsFront.Add(lf.Value, lf.Key);
				flagsBack.Add(lf.Value, lf.Key);
			}
			flagsFront.Enabled = General.Map.Config.SidedefFlags.Count > 0;
			flagsBack.Enabled = General.Map.Config.SidedefFlags.Count > 0;

			// Fill actions list
			action.AddInfo(General.Map.Config.SortedLinedefActions.ToArray());

			// Fill activations list
			foreach(LinedefActivateInfo ai in General.Map.Config.LinedefActivates) udmfactivates.Add(ai.Title, ai);

			// Fill keys list
			keynumbers = new List<int>();
			if(General.Map.Config.Enums.ContainsKey("keys"))
			{
				foreach(EnumItem item in General.Map.Config.Enums["keys"])
				{
					keynumbers.Add(item.GetIntValue());
					lockpick.Items.Add(item);
				}
			}
			lockpick.Enabled = (keynumbers.Count > 0);
			labellockpick.Enabled = (keynumbers.Count > 0);
			
			// Initialize image selectors
			fronthigh.Initialize();
			frontmid.Initialize();
			frontlow.Initialize();
			backhigh.Initialize();
			backmid.Initialize();
			backlow.Initialize();

			// Initialize custom fields editor
			fieldslist.Setup("linedef");

			// Fill universal fields list
			fieldslist.ListFixedFields(General.Map.Config.LinedefFields);

			// Initialize controls
			frontUdmfControls = new List<PairedFieldsControl> { pfcFrontOffsetTop, pfcFrontOffsetMid, pfcFrontOffsetBottom, pfcFrontScaleTop, pfcFrontScaleMid, pfcFrontScaleBottom };
			backUdmfControls = new List<PairedFieldsControl> { pfcBackOffsetTop, pfcBackOffsetMid, pfcBackOffsetBottom, pfcBackScaleTop, pfcBackScaleMid, pfcBackScaleBottom };

			// Setup renderstyles
			renderstyles = new string[General.Map.Config.LinedefRenderStyles.Count];
			General.Map.Config.LinedefRenderStyles.Keys.CopyTo(renderstyles, 0);
			renderStyle.Enabled = (General.Map.Config.LinedefRenderStyles.Count > 0);
			labelrenderstyle.Enabled = (General.Map.Config.LinedefRenderStyles.Count > 0);

			// Fill renderstyles
			foreach(KeyValuePair<string, string> lf in General.Map.Config.LinedefRenderStyles)
				renderStyle.Items.Add(lf.Value);

			// Restore value linking
			pfcFrontScaleTop.LinkValues = General.Settings.ReadSetting("windows." + configname + ".linkfronttopscale", false);
			pfcFrontScaleMid.LinkValues = General.Settings.ReadSetting("windows." + configname + ".linkfrontmidscale", false);
			pfcFrontScaleBottom.LinkValues = General.Settings.ReadSetting("windows." + configname + ".linkfrontbottomscale", false);
			pfcBackScaleTop.LinkValues = General.Settings.ReadSetting("windows." + configname + ".linkbacktopscale", false);
			pfcBackScaleMid.LinkValues = General.Settings.ReadSetting("windows." + configname + ".linkbackmidscale", false);
			pfcBackScaleBottom.LinkValues = General.Settings.ReadSetting("windows." + configname + ".linkbackbottomscale", false);

			// Upper/middle/lower brightness of front sidedef
			lightfrontupper.Setup(VisualModes.VisualGeometryType.WALL_UPPER);
			lightfrontmiddle.Setup(VisualModes.VisualGeometryType.WALL_MIDDLE);
			lightfrontlower.Setup(VisualModes.VisualGeometryType.WALL_LOWER);

			// Upper/middle/lower brightness of back sidedef
			lightbackupper.Setup(VisualModes.VisualGeometryType.WALL_UPPER);
			lightbackmiddle.Setup(VisualModes.VisualGeometryType.WALL_MIDDLE);
			lightbacklower.Setup(VisualModes.VisualGeometryType.WALL_LOWER);

			// Disable top/mid/bottom texture offset controls?
			if (!General.Map.Config.UseLocalSidedefTextureOffsets)
			{
				pfcFrontOffsetTop.Enabled = false;
				pfcFrontOffsetMid.Enabled = false;
				pfcFrontOffsetBottom.Enabled = false;

				pfcBackOffsetTop.Enabled = false;
				pfcBackOffsetMid.Enabled = false;
				pfcBackOffsetBottom.Enabled = false;

				labelFrontOffsetTop.Enabled = false;
				labelFrontOffsetMid.Enabled = false;
				labelFrontOffsetBottom.Enabled = false;

				labelBackOffsetTop.Enabled = false;
				labelBackOffsetMid.Enabled = false;
				labelBackOffsetBottom.Enabled = false;
			}

			// Diable brightness controls?
			if(!General.Map.Config.DistinctWallBrightness)
			{
				lightFront.Enabled = false;
				cbLightAbsoluteFront.Enabled = false;
				resetfrontlight.Enabled = false;

				lightBack.Enabled = false;
				cbLightAbsoluteBack.Enabled = false;
				resetbacklight.Enabled = false;
			}
		}

		#endregion

		#region ================== Methods

		// This sets up the form to edit the given lines
		public void Setup(ICollection<Linedef> lines, bool selectfront, bool selectback)
		{
            // Window setup
            // ano - moved this here because we don't reinstantiate the thing every time anymore
            if (General.Settings.StoreSelectedEditTab)
            {
                int activetab = General.Settings.ReadSetting("windows." + configname + ".activetab", 0);

                // When front or back tab was previously selected, switch to appropriate side (selectfront/selectback are set in BaseVisualGeometrySidedef.OnEditEnd)
                if ((selectfront || selectback) && (activetab == 1 || activetab == 2))
                    tabs.SelectTab(selectfront ? 1 : 2);
                else
                    tabs.SelectTab(activetab);
            }

            preventchanges = true;
			oldmapischanged = General.Map.IsChanged;
            undocreated = false;
            argscontrol.Reset();
            tagsselector.Reset();

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

			// UDMF Activations
			foreach(CheckBox c in udmfactivates.Checkboxes)
			{
				LinedefActivateInfo ai = (c.Tag as LinedefActivateInfo);
				if(fl.Flags.ContainsKey(ai.Key)) c.Checked = fl.Flags[ai.Key];
			}

			fieldslist.SetValues(fl.Fields, true); // Custom fields
			commenteditor.SetValues(fl.Fields, true); //mxd. Comments
			renderStyle.SelectedIndex = Array.IndexOf(renderstyles, fl.Fields.GetValue("renderstyle", "translucent"));
			alpha.Text = General.Clamp(fl.Fields.GetValue("alpha", 1.0), 0.0, 1.0).ToString();

			// Locknumber
			int locknumber = fl.Fields.GetValue("locknumber", 0);
			lockpick.SelectedIndex = keynumbers.IndexOf(locknumber);
			if(lockpick.SelectedIndex == -1) lockpick.Text = locknumber.ToString();

			// Action
			if (action.Value != fl.Action)
				action.Value = fl.Action;
			else
				action_ValueChanges(action, EventArgs.Empty);

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

				// Flags
				foreach(CheckBox c in flagsFront.Checkboxes)
					if(fl.Front.Flags.ContainsKey(c.Tag.ToString())) c.Checked = fl.Front.Flags[c.Tag.ToString()];

				// Front settings
				foreach(PairedFieldsControl pfc in frontUdmfControls)
					pfc.SetValuesFrom(fl.Front.Fields, true);

				lightFront.Text = UniFields.GetInteger(fl.Front.Fields, "light", 0).ToString();
				cbLightAbsoluteFront.Checked = fl.Front.Fields.GetValue("lightabsolute", false);

				frontTextureOffset.SetValues(fl.Front.OffsetX, fl.Front.OffsetY, true); //mxd

				// Upper/middle/lower brightness of front sidedef
				lightfrontupper.SetValues(fl.Front, true);
				lightfrontmiddle.SetValues(fl.Front, true);
				lightfrontlower.SetValues(fl.Front, true);
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

				// Flags
				foreach(CheckBox c in flagsBack.Checkboxes)
					if(fl.Back.Flags.ContainsKey(c.Tag.ToString())) c.Checked = fl.Back.Flags[c.Tag.ToString()];

				// Back settings
				foreach(PairedFieldsControl pfc in backUdmfControls)
					pfc.SetValuesFrom(fl.Back.Fields, true);

				lightBack.Text = UniFields.GetInteger(fl.Back.Fields, "light", 0).ToString();
				cbLightAbsoluteBack.Checked = fl.Back.Fields.GetValue("lightabsolute", false);
 
				backTextureOffset.SetValues(fl.Back.OffsetX, fl.Back.OffsetY, true); //mxd

				// Upper/middle/lower brightness of back sidedef
				lightbackupper.SetValues(fl.Back, true);
				lightbackmiddle.SetValues(fl.Back, true);
				lightbacklower.SetValues(fl.Back, true);
			}

			////////////////////////////////////////////////////////////////////////
			// Now go for all lines and change the options when a setting is different
			////////////////////////////////////////////////////////////////////////

			// Go for all lines
			foreach (Linedef l in lines)
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

				// UDMF Activations
				foreach(CheckBox c in udmfactivates.Checkboxes)
				{
					if(c.CheckState == CheckState.Indeterminate) continue; //mxd

					LinedefActivateInfo ai = (c.Tag as LinedefActivateInfo);
					if(l.IsFlagSet(ai.Key) != c.Checked) 
					{
						c.ThreeState = true;
						c.CheckState = CheckState.Indeterminate;
					}
				}

				//mxd. UDMF Settings

				// Render style
				if(renderStyle.SelectedIndex > -1 && renderStyle.SelectedIndex != Array.IndexOf(renderstyles, l.Fields.GetValue("renderstyle", "translucent")))
					renderStyle.SelectedIndex = -1;

				// Alpha
				if(!string.IsNullOrEmpty(alpha.Text) && General.Clamp(alpha.GetResultFloat(1.0), 0.0, 1.0) != l.Fields.GetValue("alpha", 1.0))
					alpha.Text = string.Empty;

				// Locknumber
				if(!string.IsNullOrEmpty(lockpick.Text)) 
				{
					if(lockpick.SelectedIndex == -1) 
					{
						if(int.TryParse(lockpick.Text, out locknumber) && locknumber != l.Fields.GetValue("locknumber", 0)) 
						{
							lockpick.SelectedIndex = -1;
							lockpick.Text = string.Empty;
						}
					} 
					else if(keynumbers[lockpick.SelectedIndex] != l.Fields.GetValue("locknumber", 0)) 
					{
						lockpick.SelectedIndex = -1;
						lockpick.Text = string.Empty;
					}
				}

				// Custom fields
				fieldslist.SetValues(l.Fields, false);

				//mxd. Comments
				commenteditor.SetValues(l.Fields, false);

				// Action
				if(l.Action != action.Value) action.Empty = true;

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
						frontlow.MultipleTextures = true; //mxd
						frontlow.TextureName = string.Empty;
					}
					if(frontsector.Text != l.Front.Sector.Index.ToString()) frontsector.Text = string.Empty;

					//flags
					foreach(CheckBox c in flagsFront.Checkboxes) 
					{
						if(c.CheckState == CheckState.Indeterminate) continue;
						if(l.Front.IsFlagSet(c.Tag.ToString()) != c.Checked) 
						{
							c.ThreeState = true;
							c.CheckState = CheckState.Indeterminate;
						}
					}

					//mxd
					foreach(PairedFieldsControl pfc in frontUdmfControls)
						pfc.SetValuesFrom(l.Front.Fields, false);

					if(!string.IsNullOrEmpty(lightFront.Text)) 
					{
						int light = UniFields.GetInteger(l.Front.Fields, "light", 0);
						if(light != lightFront.GetResult(light)) lightFront.Text = string.Empty;
					}

					if(l.Front.Fields.GetValue("lightabsolute", false) != cbLightAbsoluteFront.Checked) 
					{
						cbLightAbsoluteFront.ThreeState = true;
						cbLightAbsoluteFront.CheckState = CheckState.Indeterminate;
					}

					frontTextureOffset.SetValues(l.Front.OffsetX, l.Front.OffsetY, false); //mxd

					// Upper/middle/lower brightness of front sidedef
					lightfrontupper.SetValues(l.Front, false);
					lightfrontmiddle.SetValues(l.Front, false);
					lightfrontlower.SetValues(l.Front, false);
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

					//flags
					foreach(CheckBox c in flagsBack.Checkboxes) 
					{
						if(c.CheckState == CheckState.Indeterminate) continue;
						if(l.Back.IsFlagSet(c.Tag.ToString()) != c.Checked) 
						{
							c.ThreeState = true;
							c.CheckState = CheckState.Indeterminate;
						}
					}

					//mxd
					foreach(PairedFieldsControl pfc in backUdmfControls)
						pfc.SetValuesFrom(l.Back.Fields, false);

					if(!string.IsNullOrEmpty(lightBack.Text)) 
					{
						int light = UniFields.GetInteger(l.Back.Fields, "light", 0);
						if(light != lightBack.GetResult(light))
							lightBack.Text = string.Empty;
					}

					if(l.Back.Fields.GetValue("lightabsolute", false) != cbLightAbsoluteBack.Checked) 
					{
						cbLightAbsoluteBack.ThreeState = true;
						cbLightAbsoluteBack.CheckState = CheckState.Indeterminate;
					}

					backTextureOffset.SetValues(l.Back.OffsetX, l.Back.OffsetY, false); //mxd

					// Upper/middle/lower brightness of back sidedef
					lightbackupper.SetValues(l.Back, false);
					lightbackmiddle.SetValues(l.Back, false);
					lightbacklower.SetValues(l.Back, false);
				}

				//mxd
				linedefprops.Add(new LinedefProperties(l));
			}

			//mxd. Set tags
			tagsselector.SetValues(lines);
			
			// Refresh controls so that they show their image
			backhigh.Refresh();
			backmid.Refresh();
			backlow.Refresh();
			fronthigh.Refresh();
			frontmid.Refresh();
			frontlow.Refresh();

			preventchanges = false;

			CheckActivationFlagsRequired(); //mxd
			argscontrol.UpdateScriptControls(); //mxd
			actionhelp.UpdateAction(action.GetValue()); //mxd
			commenteditor.FinishSetup(); //mxd

			//mxd. Update "Reset" buttons
			resetfrontlight.Visible = (cbLightAbsoluteFront.CheckState != CheckState.Unchecked || lightFront.GetResult(0) != 0);
			resetbacklight.Visible = (cbLightAbsoluteBack.CheckState != CheckState.Unchecked || lightBack.GetResult(0) != 0);
			if(alpha.Text == "1") resetalpha.Visible = false;

			// Upper/middle/lower brightness of front sidedef
			lightfrontupper.FinalizeSetup();
			lightfrontmiddle.FinalizeSetup();
			lightfrontlower.FinalizeSetup();

			// Upper/middle/lower brightness of back sidedef
			lightbackupper.FinalizeSetup();
			lightbackmiddle.FinalizeSetup();
			lightbacklower.FinalizeSetup();
		}

		//mxd
		public void MakeUndo() 
		{
			if(undocreated) return;
			undocreated = true;

			//mxd. Make undo
			General.Map.UndoRedo.CreateUndo("Edit " + (lines.Count > 1 ? lines.Count + " linedefs" : "linedef"));

			if(General.Map.FormatInterface.HasCustomFields) 
			{
				foreach(Linedef l in lines)
				{
					l.Fields.BeforeFieldsChange();
					if(l.Front != null) l.Front.Fields.BeforeFieldsChange();
					if(l.Back != null) l.Back.Fields.BeforeFieldsChange();
				}
			}
		}

		//mxd
		private void CheckActivationFlagsRequired()
		{
			// Display a warning if we have an action and no activation flags
			if(action.Value != 0 
				&& General.Map.Config.LinedefActions.ContainsKey(action.Value) 
				&& General.Map.Config.LinedefActions[action.Value].RequiresActivation) 
			{
				bool haveactivationflag = false;
				foreach(CheckBox c in udmfactivates.Checkboxes) 
				{
					LinedefActivateInfo ai = (c.Tag as LinedefActivateInfo);
					if(ai.IsTrigger && c.CheckState != CheckState.Unchecked) 
					{
						haveactivationflag = true;
						break;
					}
				}

				missingactivation.Visible = !haveactivationflag;

				foreach (CheckBox c in udmfactivates.Checkboxes)
				{
					LinedefActivateInfo ai = (c.Tag as LinedefActivateInfo);
					if (ai.IsTrigger)
						c.ForeColor = (!haveactivationflag ? Color.DarkRed : SystemColors.ControlText);
				}
			} 
			else 
			{
				missingactivation.Visible = false;
				foreach (CheckBox c in udmfactivates.Checkboxes)
					c.ForeColor = SystemColors.ControlText;
			}
		}

		/// <summary>
		/// Runs the OnValuesChanged event. Use after updating properties from other controls.
		/// </summary>
		public void ValuesChangedExternal()
		{
			OnValuesChanged?.Invoke(this, EventArgs.Empty);
		}

		#endregion

		#region ================== Events

		// Apply clicked
		private void apply_Click(object sender, EventArgs e)
		{
			// Verify the action
			if((action.Value < General.Map.FormatInterface.MinAction) || (action.Value > General.Map.FormatInterface.MaxAction))
			{
				General.ShowWarningMessage("Linedef action must be between " + General.Map.FormatInterface.MinAction + " and " + General.Map.FormatInterface.MaxAction + ".", MessageBoxButtons.OK);
				return;
			}

			MakeUndo();

			//mxd
			int locknumber = 0;
			bool setlocknumber = false;
			if(!string.IsNullOrEmpty(lockpick.Text)) 
			{
				if(lockpick.SelectedIndex == -1) 
				{
					setlocknumber = int.TryParse(lockpick.Text, out locknumber);
				} 
				else 
				{
					locknumber = keynumbers[lockpick.SelectedIndex];
					setlocknumber = true;
				}
			}
			
			// Go for all the lines
			int offset = 0; //mxd
			foreach(Linedef l in lines)
			{
				// UDMF activations
				foreach(CheckBox c in udmfactivates.Checkboxes)
				{
					LinedefActivateInfo ai = (c.Tag as LinedefActivateInfo);
					switch(c.CheckState)
					{
						case CheckState.Checked: l.SetFlag(ai.Key, true); break;
						case CheckState.Unchecked: l.SetFlag(ai.Key, false); break;
					}
				}
				
				// Action
				if(!action.Empty) l.Action = action.Value;

				//mxd. Apply args
				argscontrol.Apply(l, offset++);
				
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

				//mxd. UDMF Settings
				fieldslist.Apply(l.Fields);
				if(setlocknumber) UniFields.SetInteger(l.Fields, "locknumber", locknumber, 0);
				commenteditor.Apply(l.Fields);
			}

			//mxd. Apply tags
			tagsselector.ApplyTo(lines);

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
			tabfront.ImageIndex = (frontside.CheckState == CheckState.Unchecked ? 1 : 0);
		}

		// Back side (un)checked
		private void backside_CheckStateChanged(object sender, EventArgs e) 
		{
			// Enable/disable panel
			// NOTE: Also enabled when checkbox is grayed!
			backgroup.Enabled = (backside.CheckState != CheckState.Unchecked);
			tabback.ImageIndex = (backside.CheckState == CheckState.Unchecked ? 1 : 0);
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
				CheckActivationFlagsRequired();
				argscontrol.UpdateScriptControls();
				actionhelp.UpdateAction(showaction);
			}
		}

		// Browse Action clicked
		private void browseaction_Click(object sender, EventArgs e)
		{
			action.Value = ActionBrowserForm.BrowseAction(this, action.Value);
		}

		//mxd
		private void tabcustom_MouseEnter(object sender, EventArgs e) 
		{
			fieldslist.Focus();
		}

		//mxd. Store window settings
		private void LinedefEditForm_FormClosing(object sender, FormClosingEventArgs e) 
		{
			// Save settings
			General.Settings.WriteSetting("windows." + configname + ".activetab", tabs.SelectedIndex);

			General.Settings.WriteSetting("windows." + configname + ".linkfronttopscale", pfcFrontScaleTop.LinkValues);
			General.Settings.WriteSetting("windows." + configname + ".linkfrontmidscale", pfcFrontScaleMid.LinkValues);
			General.Settings.WriteSetting("windows." + configname + ".linkfrontbottomscale", pfcFrontScaleBottom.LinkValues);
			General.Settings.WriteSetting("windows." + configname + ".linkbacktopscale", pfcBackScaleTop.LinkValues);
			General.Settings.WriteSetting("windows." + configname + ".linkbackmidscale", pfcBackScaleMid.LinkValues);
			General.Settings.WriteSetting("windows." + configname + ".linkbackbottomscale", pfcBackScaleBottom.LinkValues);
		}

		// Help!
		private void LinedefEditForm_HelpRequested(object sender, HelpEventArgs hlpevent)
		{
			General.ShowHelp("w_linedefedit.html");
			hlpevent.Handled = true;
		}

		#endregion

		#region ================== mxd. Realtime events (linedef)

		private void cbRenderStyle_SelectedIndexChanged(object sender, EventArgs e) 
		{
			if(preventchanges) return;
			MakeUndo(); //mxd

			//update values
			foreach(Linedef l in lines)
				UniFields.SetString(l.Fields, "renderstyle", renderstyles[renderStyle.SelectedIndex], "translucent");

			General.Map.IsChanged = true;
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
		}
		
		private void alpha_WhenTextChanged(object sender, EventArgs e) 
		{
			if(preventchanges) return;
			MakeUndo(); //mxd
			int i = 0;

			//restore values
			if(string.IsNullOrEmpty(alpha.Text)) 
			{
				foreach(Linedef l in lines) 
					UniFields.SetFloat(l.Fields, "alpha", linedefprops[i++].Alpha, 1.0);
			} 
			else //update values
			{
				foreach(Linedef l in lines) 
				{
					double value = General.Clamp(alpha.GetResultFloat(l.Fields.GetValue("alpha", 1.0)), 0.0, 1.0);
					UniFields.SetFloat(l.Fields, "alpha", value, 1.0);
				}
			}

			resetalpha.Visible = (alpha.GetResultFloat(1.0) != 1.0);

			General.Map.IsChanged = true;
			if(OnValuesChanged != null)	OnValuesChanged(this, EventArgs.Empty);
		}

		private void resetalpha_Click(object sender, EventArgs e)
		{
			alpha.Focus();
			alpha.Text = "1";
		}

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

		//mxd
		private void udmfactivates_OnValueChanged(object sender, EventArgs e) 
		{
			if(preventchanges) return;
			CheckActivationFlagsRequired();
		}

		#endregion

		#region ================== mxd. Realtime events (sides)

		#region Custom fields changed

		// Custom fields on front sides
		private void customfrontbutton_Click(object sender, EventArgs e) 
		{
			// Make collection of front sides
			List<MapElement> sides = new List<MapElement>(lines.Count);
			foreach(Linedef l in lines) if(l.Front != null) sides.Add(l.Front);

			if(!CustomFieldsForm.ShowDialog(this, MakeUndo, "Front side custom fields", "sidedef", sides, General.Map.Config.SidedefFields)) return;

			//Apply values
			Sidedef fs = General.GetByIndex(sides, 0) as Sidedef;

			//..to the first side
			foreach(PairedFieldsControl pfc in frontUdmfControls)
				pfc.SetValuesFrom(fs.Fields, true);

			lightFront.Text = UniFields.GetInteger(fs.Fields, "light", 0).ToString();
			cbLightAbsoluteFront.ThreeState = false;
			cbLightAbsoluteFront.Checked = fs.Fields.GetValue("lightabsolute", false);
					
			//flags
			foreach(CheckBox c in flagsFront.Checkboxes)
				if(fs.Flags.ContainsKey(c.Tag.ToString())) c.Checked = fs.Flags[c.Tag.ToString()];

			//..then to all of them
			foreach(Sidedef s in sides)
			{
				foreach(PairedFieldsControl pfc in frontUdmfControls)
					pfc.SetValuesFrom(s.Fields, false);

				if(!string.IsNullOrEmpty(lightFront.Text)) 
				{
					int light = UniFields.GetInteger(s.Fields, "light", 0);
					if(light != lightFront.GetResult(light)) lightFront.Text = string.Empty;
				}

				if(s.Fields.GetValue("lightabsolute", false) != cbLightAbsoluteFront.Checked) 
				{
					cbLightAbsoluteFront.ThreeState = true;
					cbLightAbsoluteFront.CheckState = CheckState.Indeterminate;
				}

				//flags
				foreach(CheckBox c in flagsFront.Checkboxes) 
				{
					if(c.CheckState == CheckState.Indeterminate) continue;

					if(s.Flags.ContainsKey(c.Tag.ToString())) 
					{
						if(s.Flags[c.Tag.ToString()] != c.Checked) 
						{
							c.ThreeState = true;
							c.CheckState = CheckState.Indeterminate;
						}
					}
				}
			}

			General.Map.IsChanged = true;
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
		}

		// Custom fields on back sides
		private void custombackbutton_Click(object sender, EventArgs e) 
		{
			// Make collection of back sides
			List<MapElement> sides = new List<MapElement>(lines.Count);
			foreach(Linedef l in lines) if(l.Back != null) sides.Add(l.Back);

			// Edit these
			if(!CustomFieldsForm.ShowDialog(this, MakeUndo, "Back side custom fields", "sidedef", sides, General.Map.Config.SidedefFields)) return;

			//Apply values
			Sidedef fs = General.GetByIndex(sides, 0) as Sidedef;

			//..to the first side
			foreach(PairedFieldsControl pfc in backUdmfControls)
				pfc.SetValuesFrom(fs.Fields, true);

			lightBack.Text = UniFields.GetInteger(fs.Fields, "light", 0).ToString();
			cbLightAbsoluteBack.ThreeState = false;
			cbLightAbsoluteBack.Checked = fs.Fields.GetValue("lightabsolute", false);

			//flags
			foreach(CheckBox c in flagsBack.Checkboxes)
				if(fs.Flags.ContainsKey(c.Tag.ToString())) c.Checked = fs.Flags[c.Tag.ToString()];

			//..then to all of them
			foreach(Sidedef s in sides) 
			{
				foreach(PairedFieldsControl pfc in backUdmfControls)
					pfc.SetValuesFrom(s.Fields, false);

				if(!string.IsNullOrEmpty(lightBack.Text)) 
				{
					int light = UniFields.GetInteger(s.Fields, "light", 0);
					if(light != lightBack.GetResult(light)) lightBack.Text = string.Empty;
				}

				if(s.Fields.GetValue("lightabsolute", false) != cbLightAbsoluteBack.Checked) 
				{
					cbLightAbsoluteBack.ThreeState = true;
					cbLightAbsoluteBack.CheckState = CheckState.Indeterminate;
				}

				//flags
				foreach(CheckBox c in flagsBack.Checkboxes) 
				{
					if(c.CheckState == CheckState.Indeterminate) continue;

					if(s.Flags.ContainsKey(c.Tag.ToString()) && s.Flags[c.Tag.ToString()] != c.Checked) 
					{
						c.ThreeState = true;
						c.CheckState = CheckState.Indeterminate;
					}
				}
			}

			General.Map.IsChanged = true;
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
		}

			#endregion

			#region Texture changed

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

			#endregion

			#region Brightness changed

		private void lightFront_WhenTextChanged(object sender, EventArgs e) 
		{
			if(preventchanges) return;
			MakeUndo(); //mxd
			int i = 0;

			// Reset increment steps, otherwise it's just keep counting and counting
			lightFront.ResetIncrementStep();

			//restore values
			if (string.IsNullOrEmpty(lightFront.Text)) 
			{
				foreach(Linedef l in lines) 
				{
					if(l.Front != null)
						UniFields.SetInteger(l.Front.Fields, "light", (linedefprops[i].Front != null ? linedefprops[i].Front.Brightness : 0), 0);
					i++;
				}
			} 
			else //update values
			{
				foreach(Linedef l in lines) 
				{
					if(l.Front != null) 
					{
						bool absolute = false;
						switch(cbLightAbsoluteFront.CheckState)
						{
							case CheckState.Indeterminate:
								absolute = l.Front.Fields.GetValue("lightabsolute", false);
								break;
							case CheckState.Checked:
								absolute = true;
								break;
						}

						int value = General.Clamp(lightFront.GetResult((linedefprops[i].Front != null ? linedefprops[i].Front.Brightness : 0)), (absolute ? 0 : -255), 255);
						UniFields.SetInteger(l.Front.Fields, "light", value, 0);
					}
					i++;
				}
			}

			resetfrontlight.Visible = (cbLightAbsoluteFront.CheckState != CheckState.Unchecked || lightFront.Text != "0");
			General.Map.IsChanged = true;
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
		}

		private void lightBack_WhenTextChanged(object sender, EventArgs e) 
		{
			if(preventchanges) return;
			MakeUndo(); //mxd
			int i = 0;

			//restore values
			if(string.IsNullOrEmpty(lightBack.Text)) 
			{
				foreach(Linedef l in lines) 
				{
					if(l.Back != null)
						UniFields.SetInteger(l.Back.Fields, "light", (linedefprops[i].Back != null ? linedefprops[i].Back.Brightness : 0), 0);
					i++;
				}
			} 
			else //update values
			{
				foreach(Linedef l in lines) 
				{
					if(l.Back != null) 
					{
						bool absolute = false;
						switch(cbLightAbsoluteBack.CheckState)
						{
							case CheckState.Indeterminate:
								absolute = l.Back.Fields.GetValue("lightabsolute", false);
								break;
							case CheckState.Checked:
								absolute = true;
								break;
						}

						int value = General.Clamp(lightBack.GetResult((linedefprops[i].Back != null ? linedefprops[i].Back.Brightness : 0)), (absolute ? 0 : -255), 255);
						UniFields.SetInteger(l.Back.Fields, "light", value, 0);
					}
					i++;
				}
			}

			resetbacklight.Visible = (cbLightAbsoluteBack.CheckState != CheckState.Unchecked || lightBack.Text != "0");
			General.Map.IsChanged = true;
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
		}

		private void cbLightAbsoluteFront_CheckedChanged(object sender, EventArgs e) 
		{
			if(preventchanges) return;
			MakeUndo(); //mxd

			if(cbLightAbsoluteFront.Checked) 
			{
				foreach(Linedef l in lines) 
				{
					if(l.Front == null) continue;
					l.Front.Fields["lightabsolute"] = new UniValue(UniversalType.Boolean, true);
				}
			} 
			else if(cbLightAbsoluteFront.CheckState == CheckState.Indeterminate) 
			{
				int i = 0;

				foreach(Linedef l in lines) 
				{
					if(l.Front != null) 
					{
						if(linedefprops[i].Front != null && linedefprops[i].Front.AbsoluteBrightness) 
						{
							l.Front.Fields["lightabsolute"] = new UniValue(UniversalType.Boolean, true);
						} 
						else if(l.Front.Fields.ContainsKey("lightabsolute")) 
						{
							l.Front.Fields.Remove("lightabsolute");
						}
					}
					i++;
				}
			} 
			else 
			{
				foreach(Linedef l in lines) 
				{
					if(l.Front == null) continue;
					if(l.Front.Fields.ContainsKey("lightabsolute")) l.Front.Fields.Remove("lightabsolute");
				}
			}

			resetfrontlight.Visible = (cbLightAbsoluteFront.CheckState != CheckState.Unchecked || lightFront.Text != "0");
			General.Map.IsChanged = true;
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
		}

		private void cbLightAbsoluteBack_CheckedChanged(object sender, EventArgs e) 
		{
			if(preventchanges) return;
			MakeUndo(); //mxd

			if(cbLightAbsoluteBack.Checked) 
			{
				foreach(Linedef l in lines) 
				{
					if(l.Back == null) continue;
					l.Back.Fields["lightabsolute"] = new UniValue(UniversalType.Boolean, true);
				}
			} 
			else if(cbLightAbsoluteBack.CheckState == CheckState.Indeterminate) 
			{
				int i = 0;
				
				foreach(Linedef l in lines) 
				{
					if(l.Back != null) 
					{
						if(linedefprops[i].Back != null && linedefprops[i].Back.AbsoluteBrightness) 
						{
							l.Back.Fields["lightabsolute"] = new UniValue(UniversalType.Boolean, true);
						} 
						else if(l.Back.Fields.ContainsKey("lightabsolute")) 
						{
							l.Back.Fields.Remove("lightabsolute");
						}
					}
					i++;
				}
			} 
			else 
			{
				foreach(Linedef l in lines) 
				{
					if(l.Back == null) continue;
					if(l.Back.Fields.ContainsKey("lightabsolute")) l.Back.Fields.Remove("lightabsolute");
				}
			}

			resetbacklight.Visible = (cbLightAbsoluteBack.CheckState != CheckState.Unchecked || lightBack.Text != "0");
			General.Map.IsChanged = true;
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
		}

		private void resetfrontlight_Click(object sender, EventArgs e)
		{
			MakeUndo(); //mxd

			preventchanges = true;

			cbLightAbsoluteFront.Checked = false;
			lightFront.Text = "0";

			foreach(Linedef l in lines)
			{
				if(l.Front == null) continue;
				if(l.Front.Fields.ContainsKey("lightabsolute")) l.Front.Fields.Remove("lightabsolute");
				if(l.Front.Fields.ContainsKey("light")) l.Front.Fields.Remove("light");
			}

			preventchanges = false;

			resetfrontlight.Visible = false;
			lightFront.Focus();
			General.Map.IsChanged = true;
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
		}

		private void resetbacklight_Click(object sender, EventArgs e)
		{
			MakeUndo(); //mxd

			preventchanges = true;

			cbLightAbsoluteBack.Checked = false;
			lightBack.Text = "0";

			foreach(Linedef l in lines)
			{
				if(l.Back == null) continue;
				if(l.Back.Fields.ContainsKey("lightabsolute")) l.Back.Fields.Remove("lightabsolute");
				if(l.Back.Fields.ContainsKey("light")) l.Back.Fields.Remove("light");
			}

			preventchanges = false;

			resetbacklight.Visible = false;
			lightBack.Focus();
			General.Map.IsChanged = true;
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
		}

			#endregion

			#region Global texture offsets changed

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

			#region Texture offsets changed

		private void pfcFrontOffsetTop_OnValuesChanged(object sender, EventArgs e)
		{
			if(preventchanges) return;
			MakeUndo(); //mxd
			int i = 0;

			// Reset increment steps, otherwise it's just keep counting and counting
			pfcFrontOffsetTop.ResetIncrementStep();

			foreach (Linedef l in lines) 
			{
				if(l.Front != null) 
				{
					double oldX = linedefprops[i].Front != null ? linedefprops[i].Front.OffsetTopX : 0f;
					double oldY = linedefprops[i].Front != null ? linedefprops[i].Front.OffsetTopY : 0f;
					pfcFrontOffsetTop.ApplyTo(l.Front.Fields, General.Map.FormatInterface.MinTextureOffset, General.Map.FormatInterface.MaxTextureOffset, oldX, oldY);
				}
				i++;
			}

			General.Map.IsChanged = true;
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
		}

		private void pfcFrontOffsetMid_OnValuesChanged(object sender, EventArgs e)
		{
			if(preventchanges) return;
			MakeUndo(); //mxd
			int i = 0;

			// Reset increment steps, otherwise it's just keep counting and counting
			pfcFrontOffsetMid.ResetIncrementStep();

			foreach (Linedef l in lines) 
			{
				if(l.Front != null) 
				{
					double oldX = linedefprops[i].Front != null ? linedefprops[i].Front.OffsetMidX : 0f;
					double oldY = linedefprops[i].Front != null ? linedefprops[i].Front.OffsetMidY : 0f;
					pfcFrontOffsetMid.ApplyTo(l.Front.Fields, General.Map.FormatInterface.MinTextureOffset, General.Map.FormatInterface.MaxTextureOffset, oldX, oldY);
				}

				i++;
			}

			General.Map.IsChanged = true;
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
		}

		private void pfcFrontOffsetBottom_OnValuesChanged(object sender, EventArgs e)
		{
			if(preventchanges) return;
			MakeUndo(); //mxd
			int i = 0;

			// Reset increment steps, otherwise it's just keep counting and counting
			pfcFrontOffsetBottom.ResetIncrementStep();

			foreach (Linedef l in lines) 
			{
				if(l.Front != null) 
				{
					double oldX = linedefprops[i].Front != null ? linedefprops[i].Front.OffsetBottomX : 0f;
					double oldY = linedefprops[i].Front != null ? linedefprops[i].Front.OffsetBottomY : 0f;
					pfcFrontOffsetBottom.ApplyTo(l.Front.Fields, General.Map.FormatInterface.MinTextureOffset, General.Map.FormatInterface.MaxTextureOffset, oldX, oldY);
				}

				i++;
			}

			General.Map.IsChanged = true;
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
		}

		private void pfcBackOffsetTop_OnValuesChanged(object sender, EventArgs e) 
		{
			if(preventchanges) return;
			MakeUndo(); //mxd
			int i = 0;

			// Reset increment steps, otherwise it's just keep counting and counting
			pfcBackOffsetTop.ResetIncrementStep();

			foreach (Linedef l in lines) 
			{
				if(l.Back != null) 
				{
					double oldX = linedefprops[i].Back != null ? linedefprops[i].Back.OffsetTopX : 0f;
					double oldY = linedefprops[i].Back != null ? linedefprops[i].Back.OffsetTopY : 0f;
					pfcBackOffsetTop.ApplyTo(l.Back.Fields, General.Map.FormatInterface.MinTextureOffset, General.Map.FormatInterface.MaxTextureOffset, oldX, oldY);
				}

				i++;
			}

			General.Map.IsChanged = true;
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
		}

		private void pfcBackOffsetMid_OnValuesChanged(object sender, EventArgs e) 
		{
			if(preventchanges) return;
			MakeUndo(); //mxd
			int i = 0;

			// Reset increment steps, otherwise it's just keep counting and counting
			pfcBackOffsetMid.ResetIncrementStep();

			foreach (Linedef l in lines) 
			{
				if(l.Back != null) 
				{
					double oldX = linedefprops[i].Back != null ? linedefprops[i].Back.OffsetMidX : 0f;
					double oldY = linedefprops[i].Back != null ? linedefprops[i].Back.OffsetMidY : 0f;
					pfcBackOffsetMid.ApplyTo(l.Back.Fields, General.Map.FormatInterface.MinTextureOffset, General.Map.FormatInterface.MaxTextureOffset, oldX, oldY);
				}

				i++;
			}

			General.Map.IsChanged = true;
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
		}

		private void pfcBackOffsetBottom_OnValuesChanged(object sender, EventArgs e) 
		{
			if(preventchanges) return;
			MakeUndo(); //mxd
			int i = 0;

			// Reset increment steps, otherwise it's just keep counting and counting
			pfcBackOffsetBottom.ResetIncrementStep();

			foreach (Linedef l in lines) 
			{
				if(l.Back != null) 
				{
					double oldX = linedefprops[i].Back != null ? linedefprops[i].Back.OffsetBottomX : 0f;
					double oldY = linedefprops[i].Back != null ? linedefprops[i].Back.OffsetBottomY : 0f;
					pfcBackOffsetBottom.ApplyTo(l.Back.Fields, General.Map.FormatInterface.MinTextureOffset, General.Map.FormatInterface.MaxTextureOffset, oldX, oldY);
				}

				i++;
			}

			General.Map.IsChanged = true;
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
		}

			#endregion

			#region Scale changed

		private void pfcFrontScaleTop_OnValuesChanged(object sender, EventArgs e) 
		{
			if(preventchanges) return;
			MakeUndo(); //mxd
			int i = 0;

			// Reset increment steps, otherwise it's just keep counting and counting
			pfcFrontScaleTop.ResetIncrementStep();

			foreach (Linedef l in lines) 
			{
				if(l.Front != null) 
				{
					double oldX = linedefprops[i].Front != null ? linedefprops[i].Front.ScaleTopX : 1.0f;
					double oldY = linedefprops[i].Front != null ? linedefprops[i].Front.ScaleTopY : 1.0f;
					pfcFrontScaleTop.ApplyTo(l.Front.Fields, General.Map.FormatInterface.MinTextureOffset, General.Map.FormatInterface.MaxTextureOffset, oldX, oldY);
				}

				i++;
			}

			General.Map.IsChanged = true;
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
		}

		private void pfcFrontScaleMid_OnValuesChanged(object sender, EventArgs e) 
		{
			if(preventchanges) return;
			MakeUndo(); //mxd
			int i = 0;

			// Reset increment steps, otherwise it's just keep counting and counting
			pfcFrontScaleMid.ResetIncrementStep();

			foreach (Linedef l in lines) 
			{
				if(l.Front != null) 
				{
					double oldX = linedefprops[i].Front != null ? linedefprops[i].Front.ScaleMidX : 1.0f;
					double oldY = linedefprops[i].Front != null ? linedefprops[i].Front.ScaleMidY : 1.0f;
					pfcFrontScaleMid.ApplyTo(l.Front.Fields, General.Map.FormatInterface.MinTextureOffset, General.Map.FormatInterface.MaxTextureOffset, oldX, oldY);
				}

				i++;
			}

			General.Map.IsChanged = true;
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
		}

		private void pfcFrontScaleBottom_OnValuesChanged(object sender, EventArgs e) 
		{
			if(preventchanges) return;
			MakeUndo(); //mxd
			int i = 0;

			// Reset increment steps, otherwise it's just keep counting and counting
			pfcFrontScaleBottom.ResetIncrementStep();

			foreach (Linedef l in lines) 
			{
				if(l.Front != null) 
				{
					double oldX = linedefprops[i].Front != null ? linedefprops[i].Front.ScaleBottomX : 1.0f;
					double oldY = linedefprops[i].Front != null ? linedefprops[i].Front.ScaleBottomY : 1.0f;
					pfcFrontScaleBottom.ApplyTo(l.Front.Fields, General.Map.FormatInterface.MinTextureOffset, General.Map.FormatInterface.MaxTextureOffset, oldX, oldY);
				}

				i++;
			}

			General.Map.IsChanged = true;
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
		}

		private void pfcBackScaleTop_OnValuesChanged(object sender, EventArgs e) 
		{
			if(preventchanges) return;
			MakeUndo(); //mxd
			int i = 0;

			// Reset increment steps, otherwise it's just keep counting and counting
			pfcBackScaleTop.ResetIncrementStep();

			foreach (Linedef l in lines) 
			{
				if(l.Back != null) 
				{
					double oldX = linedefprops[i].Back != null ? linedefprops[i].Back.ScaleTopX : 1.0f;
					double oldY = linedefprops[i].Back != null ? linedefprops[i].Back.ScaleTopY : 1.0f;
					pfcBackScaleTop.ApplyTo(l.Back.Fields, General.Map.FormatInterface.MinTextureOffset, General.Map.FormatInterface.MaxTextureOffset, oldX, oldY);
				}

				i++;
			}

			General.Map.IsChanged = true;
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
		}

		private void pfcBackScaleMid_OnValuesChanged(object sender, EventArgs e) 
		{
			if(preventchanges) return;
			MakeUndo(); //mxd
			int i = 0;

			// Reset increment steps, otherwise it's just keep counting and counting
			pfcBackScaleMid.ResetIncrementStep();

			foreach (Linedef l in lines) 
			{
				if(l.Back != null) 
				{
					double oldX = linedefprops[i].Back != null ? linedefprops[i].Back.ScaleMidX : 1.0f;
					double oldY = linedefprops[i].Back != null ? linedefprops[i].Back.ScaleMidY : 1.0f;
					pfcBackScaleMid.ApplyTo(l.Back.Fields, General.Map.FormatInterface.MinTextureOffset, General.Map.FormatInterface.MaxTextureOffset, oldX, oldY);
				}

				i++;
			}

			General.Map.IsChanged = true;
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
		}

		private void pfcBackScaleBottom_OnValuesChanged(object sender, EventArgs e) 
		{
			if(preventchanges) return;
			MakeUndo(); //mxd
			int i = 0;

			// Reset increment steps, otherwise it's just keep counting and counting
			pfcBackScaleBottom.ResetIncrementStep();

			foreach (Linedef l in lines) 
			{
				if(l.Back != null) 
				{
					double oldX = linedefprops[i].Back != null ? linedefprops[i].Back.ScaleBottomX : 1.0f;
					double oldY = linedefprops[i].Back != null ? linedefprops[i].Back.ScaleBottomY : 1.0f;
					pfcBackScaleBottom.ApplyTo(l.Back.Fields, General.Map.FormatInterface.MinTextureOffset, General.Map.FormatInterface.MaxTextureOffset, oldX, oldY);
				}

				i++;
			}

			General.Map.IsChanged = true;
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
		}

			#endregion

			#region Flags cahnged

		private void flagsFront_OnValueChanged(object sender, EventArgs e) 
		{
			if(preventchanges) return;
			MakeUndo(); //mxd
			int i = 0;

			foreach(Linedef l in lines) 
			{
				if(l.Front == null) continue;
				
				// Apply all flags
				foreach(CheckBox c in flagsFront.Checkboxes) 
				{
					if(c.CheckState == CheckState.Checked)
						l.Front.SetFlag(c.Tag.ToString(), true);
					else if(c.CheckState == CheckState.Unchecked)
						l.Front.SetFlag(c.Tag.ToString(), false);
					else if(linedefprops[i].Front.Flags.ContainsKey(c.Tag.ToString()))
						l.Front.SetFlag(c.Tag.ToString(), linedefprops[i].Front.Flags[c.Tag.ToString()]);
					else //linedefs created in the editor have empty Flags by default
						l.Front.SetFlag(c.Tag.ToString(), false);
				}

				i++;
			}

			General.Map.IsChanged = true;
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
		}

		private void flagsBack_OnValueChanged(object sender, EventArgs e) 
		{
			if(preventchanges) return;
			MakeUndo(); //mxd
			int i = 0;

			foreach(Linedef l in lines) 
			{
				if(l.Back == null) continue;

				// Apply all flags
				foreach(CheckBox c in flagsBack.Checkboxes) 
				{
					if(c.CheckState == CheckState.Checked)
						l.Back.SetFlag(c.Tag.ToString(), true);
					else if(c.CheckState == CheckState.Unchecked)
						l.Back.SetFlag(c.Tag.ToString(), false);
					else if(linedefprops[i].Back.Flags.ContainsKey(c.Tag.ToString()))
						l.Back.SetFlag(c.Tag.ToString(), linedefprops[i].Back.Flags[c.Tag.ToString()]);
					else //linedefs created in the editor have empty Flags by default
						l.Back.SetFlag(c.Tag.ToString(), false);
				}

				i++;
			}

			General.Map.IsChanged = true;
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
		}

		#endregion

		#endregion
	}
}
