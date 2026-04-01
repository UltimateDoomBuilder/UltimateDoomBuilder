
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
using System.Globalization;
using System.Windows.Forms;
using CodeImp.DoomBuilder.GZBuilder.Data;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Types;
using CodeImp.DoomBuilder.GZBuilder;
using CodeImp.DoomBuilder.Data;

#endregion

namespace CodeImp.DoomBuilder.Controls
{
	internal partial class LinedefInfoPanel : UserControl
	{
		private readonly int hexenformatwidth;
		private readonly int doomformatwidth;
		
		// Constructor
		public LinedefInfoPanel()
		{
			// Initialize
			InitializeComponent();
			CodeImp.DoomBuilder.General.ApplyMonoListViewFix(flags);

			// Hide stuff when in Doom format
			hexenformatwidth = infopanel.Width;
			doomformatwidth = infopanel.Width - 190;

			// We have to set the parernt (and subsequently the new location relative to the parent) here since
			// we can't set the parent in the designer. And if the parent is not set to the ConfigurablePictureBox
			// the Label's background alpha will not work correctly
			labelTextureFrontTop.Parent = fronthightex;
			labelTextureFrontTop.Location = new Point(1, 1);
			labelTextureFrontMid.Parent = frontmidtex;
			labelTextureFrontMid.Location = new Point(1, 1);
			labelTextureFrontBottom.Parent = frontlowtex;
			labelTextureFrontBottom.Location = new Point(1, 1);
			labelTextureBackTop.Parent = backhightex;
			labelTextureBackTop.Location = new Point(1, 1);
			labelTextureBackMid.Parent = backmidtex;
			labelTextureBackMid.Location = new Point(1, 1);
			labelTextureBackBottom.Parent = backlowtex;
			labelTextureBackBottom.Location = new Point(1, 1);

			//mxd
			labelTextureFrontTop.BackColor = Color.FromArgb(128, labelTextureFrontTop.BackColor);
			labelTextureFrontMid.BackColor = Color.FromArgb(128, labelTextureFrontMid.BackColor);
			labelTextureFrontBottom.BackColor = Color.FromArgb(128, labelTextureFrontBottom.BackColor);

			labelTextureBackTop.BackColor = Color.FromArgb(128, labelTextureBackTop.BackColor);
			labelTextureBackMid.BackColor = Color.FromArgb(128, labelTextureBackMid.BackColor);
			labelTextureBackBottom.BackColor = Color.FromArgb(128, labelTextureBackBottom.BackColor);
		}
		
		// This shows the info
		public void ShowInfo(Linedef l, Sidedef highlightside)
		{
			string peggedness;
			
			// Show/hide stuff depending on format
			if(!General.Map.FormatInterface.HasActionArgs)
			{
				arglbl1.Visible = false;
				arglbl2.Visible = false;
				arglbl3.Visible = false;
				arglbl4.Visible = false;
				arglbl5.Visible = false;
				arg1.Visible = false;
				arg2.Visible = false;
				arg3.Visible = false;
				arg4.Visible = false;
				arg5.Visible = false;
				infopanel.Width = doomformatwidth;
			}
			else
			{
				arglbl1.Visible = true;
				arglbl2.Visible = true;
				arglbl3.Visible = true;
				arglbl4.Visible = true;
				arglbl5.Visible = true;
				arg1.Visible = true;
				arg2.Visible = true;
				arg3.Visible = true;
				arg4.Visible = true;
				arg5.Visible = true;
				infopanel.Width = hexenformatwidth;
			}

			//mxd. Hide activation or tag and rearrange labels 
			if(!General.Map.FormatInterface.HasBuiltInActivations && General.Map.FormatInterface.HasNumericLinedefActivations) //Hexen map format?
			{ 
				activation.Visible = true;
				activationlabel.SetLeftExpandText("Activation:");
				activationlabel.Visible = true;
				taglabel.Visible = false;
				tag.Visible = false;
				
				//set activation
				foreach(LinedefActivateInfo ai in General.Map.Config.LinedefActivates) 
				{
					if(l.Activate == ai.Index) 
					{
						activation.Text = ai.Title;
						break;
					}
				}

				activation.Enabled = (l.Activate != 0 || l.Action != 0); //mxd
				activationlabel.Enabled = (l.Activate != 0 || l.Action != 0); //mxd
			} 
			else 
			{
				if(General.Map.UDMF)
				{
					// Hijack activation labels to show lock numer...
					activationlabel.SetLeftExpandText("Lock:");
					activationlabel.Visible = true;
					activation.Visible = true;

					int locknum = l.Fields.GetValue("locknumber", 0);
					if(locknum != 0)
					{
						activationlabel.Enabled = true;
						activation.Enabled = true;

						if(General.Map.Config.Enums.ContainsKey("keys"))
						{
							foreach(EnumItem item in General.Map.Config.Enums["keys"])
							{
								if(item.GetIntValue() == locknum)
								{
									activation.Text = locknum + " - " + item.Title;
									break;
								}
							}
						}
						else
						{
							activation.Text = locknum.ToString();
						}
					}
					else
					{
						activationlabel.Enabled = false;
						activation.Enabled = false;
						activation.Text = "None";
					}
				}
				else
				{
					// Should be Doom map format 
					activationlabel.Visible = false;
					activation.Visible = false;
				}
				
				taglabel.Visible = true;
				tag.Visible = true;

				//mxd. Set tag(s)
				if(l.Tags.Count > 1)
				{
					string[] tags = new string[l.Tags.Count];
					for(int i = 0; i < l.Tags.Count; i++) tags[i] = l.Tags[i].ToString();
					tag.Text = string.Join(", ", tags);
					tag.Enabled = true;
					taglabel.Enabled = true;
					taglabel.Text = "Tags:";
				}
				else
				{
					tag.Text = l.Tag + (General.Map.Options.TagLabels.ContainsKey(l.Tag) ? " - " + General.Map.Options.TagLabels[l.Tag] : string.Empty);
					tag.Enabled = (l.Tag != 0);
					taglabel.Enabled = (l.Tag != 0);
					taglabel.Text = "Tag:";
				}
			}
			
			// Get line action information
			LinedefActionInfo act = General.Map.Config.GetLinedefActionInfo(l.Action);
			
			// Determine peggedness
			bool upperunpegged = l.IsFlagSet(General.Map.Config.UpperUnpeggedFlag);
			bool lowerunpegged = l.IsFlagSet(General.Map.Config.LowerUnpeggedFlag);
			if(upperunpegged && lowerunpegged)
				peggedness = "Upper & Lower";
			else if(upperunpegged)
				peggedness = "Upper";
			else if(lowerunpegged)
				peggedness = "Lower";
			else
				peggedness = "None";
			
			// Linedef info
#if DEBUG
			infopanel.Text = " Linedef " + l.Index + " (vert. " + l.Start.Index + " - vert. " + l.End.Index + ") ";
#else
			infopanel.Text = " Linedef " + l.Index + " ";
#endif
			action.Text = act.ToString();
			length.Text = l.Length.ToString("0.##");
			angle.Text = l.AngleDeg + "\u00B0";
			unpegged.Text = peggedness;
			unpegged.Enabled = (peggedness != "None"); //mxd
			peglabel.Enabled = (peggedness != "None"); //mxd
			action.Enabled = (act.Index != 0);
			actionlabel.Enabled = (act.Index != 0);

			//mxd. ACS script argument names
			bool isacsscript = (Array.IndexOf(GZGeneral.ACS_SPECIALS, l.Action) != -1);
			bool isarg0str = (General.Map.UDMF && l.Fields.ContainsKey("arg0str"));
            string arg0str = isarg0str ? l.Fields.GetValue("arg0str", string.Empty) : string.Empty;
			ScriptItem scriptitem = null;

			//mxd. Set default label colors
			arg1.ForeColor = SystemColors.ControlText;
			arglbl1.ForeColor = SystemColors.ControlText;

			// Named script?
			if(isacsscript && isarg0str && General.Map.NamedScripts.ContainsKey(arg0str.ToLowerInvariant()))
			{
				scriptitem = General.Map.NamedScripts[arg0str.ToLowerInvariant()];
			}
			// Script number?
			else if(isacsscript && General.Map.NumberedScripts.ContainsKey(l.Args[0]))
			{
				scriptitem = General.Map.NumberedScripts[l.Args[0]];
				arg0str = (scriptitem.HasCustomName ? scriptitem.Name : scriptitem.Index.ToString());
			}

			// Apply script args?
			Label[] arglabels = { arglbl1, arglbl2, arglbl3, arglbl4, arglbl5 };
			Label[] args = { arg1, arg2, arg3, arg4, arg5 };

			if(scriptitem != null)
			{
                int first;
				string[] argnames = scriptitem.GetArgumentsDescriptions(l.Action, out first);
                for (int i = 0; i < first; i++)
                {
					int right = arglabels[i].Right;
					arglabels[i].Text = (isarg0str ? act.Args[i].TitleStr : act.Args[i].Title) + ":";
					arglabels[i].Left = right - arglabels[i].Width;
					arglabels[i].Enabled = act.Args[i].Used;
                    args[i].Enabled = act.Args[i].Used;
                }

                for (int i = first; i < argnames.Length; i++)
				{
					if(!string.IsNullOrEmpty(argnames[i]))
					{
						int right = arglabels[i].Right;
						arglabels[i].Text = argnames[i] + ":";
						arglabels[i].Left = right - arglabels[i].Width;
						arglabels[i].Enabled = true;
						args[i].Enabled = true;
					}
					else
					{
						int right = arglabels[i].Right;
                        arglabels[i].Text = (isarg0str ? act.Args[i].TitleStr : act.Args[i].Title) + ":";
						arglabels[i].Left = right - arglabels[i].Width;
                        arglabels[i].Enabled = act.Args[i].Used;
						args[i].Enabled = act.Args[i].Used;
					}
				}
			}
			else
			{
				for(int i = 0; i < act.Args.Length; i++)
				{
					int right = arglabels[i].Right;
                    arglabels[i].Text = (isarg0str ? act.Args[i].TitleStr : act.Args[i].Title) + ":";
					arglabels[i].Left = right - arglabels[i].Width;
                    arglabels[i].Enabled = act.Args[i].Used;
					args[i].Enabled = act.Args[i].Used;
				}

				// Special cases: unknown script name/index
				if(isacsscript)
				{
					int right = arglbl1.Right;
					arglbl1.Text = "Unknown script " + (isarg0str ? "name" : "number") + ":";
					arglbl1.Left = right - arglbl1.Width;
					arg1.ForeColor = Color.DarkRed;
					arglbl1.ForeColor = Color.DarkRed;
				}
			}

			//mxd. Set argument value and label
			if(isarg0str) arg1.Text = arg0str;
			else SetArgumentText(act.Args[0], arg1, l.Args[0]);
			SetArgumentText(act.Args[1], arg2, l.Args[1]);
			SetArgumentText(act.Args[2], arg3, l.Args[2]);
			SetArgumentText(act.Args[3], arg4, l.Args[3]);
			SetArgumentText(act.Args[4], arg5, l.Args[4]);

			// Front side available?
			if(l.Front != null)
			{
				//mxd. Extended info shown?
				bool hasTopFields = false;
				bool hasMiddleFields = false;
				bool hasBottomFields = false;

				//mxd. Highlight this side?
				bool highlight = (l.Front == highlightside);
				frontpanel.ForeColor = (highlight ? SystemColors.HotTrack : SystemColors.WindowText); //mxd
				
				// Show sidedef info
				frontpanel.Visible = true; //mxd
				frontpanel.Text = " Front Sidedef " + l.Front.Index;
				
				//mxd
				if(General.Map.UDMF) 
				{
					//light
					frontoffsetlabel.SetLeftExpandText("Front light:");
					SetUDMFLight(l.Front, frontoffsetlabel, frontoffset, highlight);

					//global offset, sector index
					frontpanel.Text += ". Offset " + l.Front.OffsetX + ", " + l.Front.OffsetY + ". Sector " + l.Front.Sector.Index + " ";
					
					//sidedef top
					hasTopFields = SetPairedUDMFFieldsLabel(l.Front.Fields, "offsetx_top", "offsety_top", 0.0, frontTopUDMFOffsetLabel, frontTopUDMFOffset, highlight);
					hasTopFields |= SetPairedUDMFFieldsLabel(l.Front.Fields, "scalex_top", "scaley_top", 1.0, frontTopUDMFScaleLabel, frontTopUDMFScale, highlight);

					//sidedef middle
					hasMiddleFields = SetPairedUDMFFieldsLabel(l.Front.Fields, "offsetx_mid", "offsety_mid", 0.0, frontMidUDMFOffsetLabel, frontMidUDMFOffset, highlight);
					hasMiddleFields |= SetPairedUDMFFieldsLabel(l.Front.Fields, "scalex_mid", "scaley_mid", 1.0, frontMidUDMFScaleLabel, frontMidUDMFScale, highlight);

					//sidedef bottom
					hasBottomFields = SetPairedUDMFFieldsLabel(l.Front.Fields, "offsetx_bottom", "offsety_bottom", 0.0, frontBottomUDMFOffsetLabel, frontBottomUDMFOffset, highlight);
					hasBottomFields |= SetPairedUDMFFieldsLabel(l.Front.Fields, "scalex_bottom", "scaley_bottom", 1.0, frontBottomUDMFScaleLabel, frontBottomUDMFScale, highlight);

					//visibility
					frontTopUDMFOffset.Visible = hasTopFields;
					frontTopUDMFOffsetLabel.Visible = hasTopFields;
					frontTopUDMFScale.Visible = hasTopFields;
					frontTopUDMFScaleLabel.Visible = hasTopFields;

					frontMidUDMFOffset.Visible = hasMiddleFields;
					frontMidUDMFOffsetLabel.Visible = hasMiddleFields;
					frontMidUDMFScale.Visible = hasMiddleFields;
					frontMidUDMFScaleLabel.Visible = hasMiddleFields;

					frontBottomUDMFOffset.Visible = hasBottomFields;
					frontBottomUDMFOffsetLabel.Visible = hasBottomFields;
					frontBottomUDMFScale.Visible = hasBottomFields;
					frontBottomUDMFScaleLabel.Visible = hasBottomFields;
				} 
				else 
				{
					frontoffsetlabel.SetLeftExpandText("Front offset:");
					if (l.Front.OffsetX != 0 || l.Front.OffsetY != 0)
					{
						frontoffset.Text = l.Front.OffsetX + ", " + l.Front.OffsetY;
						frontoffsetlabel.Enabled = true;
						frontoffset.Enabled = true;

						frontoffset.ForeColor = (highlight ? SystemColors.HotTrack : SystemColors.WindowText);
						frontoffsetlabel.ForeColor = frontoffset.ForeColor;
					}
					else
					{
						frontoffset.Text = "--, --";
						frontoffsetlabel.Enabled = false;
						frontoffset.Enabled = false;
					}

					//mxd. Sector index
					frontpanel.Text += ". Sector " + l.Front.Sector.Index + " ";

					//visibility
					frontTopUDMFOffsetLabel.Visible = false;
					frontTopUDMFScaleLabel.Visible = false;

					frontMidUDMFOffsetLabel.Visible = false;
					frontMidUDMFScaleLabel.Visible = false;

					frontBottomUDMFOffsetLabel.Visible = false;
					frontBottomUDMFScaleLabel.Visible = false;
				}

				//mxd. Set texture names, update panel sizes
				UpdateTexturePanel(panelFrontTop, l.Front.HighTexture, fronthighname, labelTextureFrontTop,
					Math.Max(frontTopUDMFOffset.Right, frontTopUDMFScale.Right) + 4, fronthightex,
					frontTopUDMFOffsetLabel.Left, hasTopFields, l.Front.HighRequired());

				UpdateTexturePanel(panelFrontMid, l.Front.MiddleTexture, frontmidname, labelTextureFrontMid,
					Math.Max(frontMidUDMFOffset.Right, frontMidUDMFScale.Right) + 4, frontmidtex,
					frontMidUDMFOffsetLabel.Left, hasMiddleFields, l.Front.MiddleRequired());

				UpdateTexturePanel(panelFrontLow, l.Front.LowTexture, frontlowname, labelTextureFrontBottom,
					Math.Max(frontBottomUDMFOffset.Right, frontBottomUDMFScale.Right) + 4, frontlowtex,
					frontBottomUDMFOffsetLabel.Left, hasBottomFields, l.Front.LowRequired());

				//mxd. Resize panel
				flowLayoutPanelFront.Width = panelFrontLow.Right;
				frontpanel.Width = flowLayoutPanelFront.Width + flowLayoutPanelFront.Left * 2 - 4;
			}
			else
			{
				// Show no info
				if(General.Map.UDMF) //mxd
				{
					frontoffsetlabel.SetLeftExpandText("Front light:");
					frontoffset.Text = "--";
				} 
				else 
				{
					frontoffsetlabel.SetLeftExpandText("Front offset:");
					frontoffset.Text = "--, --";
				}

				frontoffsetlabel.Enabled = false;
				frontoffset.Enabled = false;

				fronthightex.BackgroundImage = null;
				frontmidtex.BackgroundImage = null;
				frontlowtex.BackgroundImage = null;

				frontpanel.Visible = false; //mxd
			}

			// Back size available?
			if(l.Back != null)
			{
				//mxd. Extended info shown?
				bool hasTopFields = false;
				bool hasMiddleFields = false;
				bool hasBottomFields = false;

				//mxd. Highlight this side?
				bool highlight = l.Back == highlightside;
				backpanel.ForeColor = (highlight ? SystemColors.HotTrack : SystemColors.WindowText); //mxd

				// Show sidedef info
				backpanel.Visible = true; //mxd
				backpanel.Text = " Back Sidedef " + l.Back.Index;

				//mxd
				if(General.Map.UDMF) 
				{
					//light
					backoffsetlabel.SetLeftExpandText("Back light:");
					SetUDMFLight(l.Back, backoffsetlabel, backoffset, highlight);

					//global offset, sector index
					backpanel.Text += ". Offset " + l.Back.OffsetX + ", " + l.Back.OffsetY + ". Sector " + l.Back.Sector.Index + " ";

					//sidedef top
					hasTopFields = SetPairedUDMFFieldsLabel(l.Back.Fields, "offsetx_top", "offsety_top", 0.0, backTopUDMFOffsetLabel, backTopUDMFOffset, highlight);
					hasTopFields |= SetPairedUDMFFieldsLabel(l.Back.Fields, "scalex_top", "scaley_top", 1.0, backTopUDMFScaleLabel, backTopUDMFScale, highlight);

					//sidedef middle
					hasMiddleFields = SetPairedUDMFFieldsLabel(l.Back.Fields, "offsetx_mid", "offsety_mid", 0.0, backMidUDMFOffsetLabel, backMidUDMFOffset, highlight);
					hasMiddleFields |= SetPairedUDMFFieldsLabel(l.Back.Fields, "scalex_mid", "scaley_mid", 1.0, backMidUDMFScaleLabel, backMidUDMFScale, highlight);

					//sidedef bottom
					hasBottomFields = SetPairedUDMFFieldsLabel(l.Back.Fields, "offsetx_bottom", "offsety_bottom", 0.0, backBottomUDMFOffsetLabel, backBottomUDMFOffset, highlight);
					hasBottomFields |= SetPairedUDMFFieldsLabel(l.Back.Fields, "scalex_bottom", "scaley_bottom", 1.0, backBottomUDMFScaleLabel, backBottomUDMFScale, highlight);

					//visibility
					backTopUDMFOffset.Visible = hasTopFields;
					backTopUDMFOffsetLabel.Visible = hasTopFields;
					backTopUDMFScale.Visible = hasTopFields;
					backTopUDMFScaleLabel.Visible = hasTopFields;

					backMidUDMFOffset.Visible = hasMiddleFields;
					backMidUDMFOffsetLabel.Visible = hasMiddleFields;
					backMidUDMFScale.Visible = hasMiddleFields;
					backMidUDMFScaleLabel.Visible = hasMiddleFields;

					backBottomUDMFOffset.Visible = hasBottomFields;
					backBottomUDMFOffsetLabel.Visible = hasBottomFields;
					backBottomUDMFScale.Visible = hasBottomFields;
					backBottomUDMFScaleLabel.Visible = hasBottomFields;
				}
				else
				{
					backoffsetlabel.SetLeftExpandText("Back offset:");
					if(l.Back.OffsetX != 0 || l.Back.OffsetY != 0)
					{
						backoffset.Text = l.Back.OffsetX + ", " + l.Back.OffsetY;
						backoffsetlabel.Enabled = true;
						backoffset.Enabled = true;

						backoffset.ForeColor = (highlight ? SystemColors.HotTrack : SystemColors.WindowText);
						backoffsetlabel.ForeColor = backoffset.ForeColor;
					}
					else
					{
						backoffset.Text = "--, --";
						backoffsetlabel.Enabled = false;
						backoffset.Enabled = false;
					}

					// Sector index
					backpanel.Text += ". Sector " + l.Back.Sector.Index + " ";
				}

				//mxd. Set texture names, update panel sizes
				UpdateTexturePanel(panelBackTop, l.Back.HighTexture, backhighname, labelTextureBackTop, 
					Math.Max(backTopUDMFOffset.Right, backTopUDMFScale.Right) + 4, backhightex,
					backTopUDMFOffsetLabel.Left, hasTopFields, l.Back.HighRequired());
				
				UpdateTexturePanel(panelBackMid, l.Back.MiddleTexture, backmidname, labelTextureBackMid,
					Math.Max(backMidUDMFOffset.Right, backMidUDMFScale.Right) + 4, backmidtex,
					backMidUDMFOffsetLabel.Left, hasMiddleFields, l.Back.MiddleRequired());
				
				UpdateTexturePanel(panelBackLow, l.Back.LowTexture, backlowname, labelTextureBackBottom,
					Math.Max(backBottomUDMFOffset.Right, backBottomUDMFScale.Right) + 4, backlowtex,
					backBottomUDMFOffsetLabel.Left, hasBottomFields, l.Back.LowRequired());

				//mxd. Resize panel
				flowLayoutPanelBack.Width = panelBackLow.Right;
				backpanel.Width = flowLayoutPanelBack.Width + flowLayoutPanelBack.Left * 2 - 4;
			}
			else
			{
				// Show no info
				if(General.Map.UDMF) //mxd
				{ 
					backoffsetlabel.SetLeftExpandText("Back light:");
					backoffset.Text = "--";
				} 
				else 
				{
					backoffsetlabel.SetLeftExpandText("Back offset:");
					backoffset.Text = "--, --";
				}

				backoffsetlabel.Enabled = false;
				backoffset.Enabled = false;

				backhightex.BackgroundImage = null;
				backmidtex.BackgroundImage = null;
				backlowtex.BackgroundImage = null;

				backpanel.Visible = false; //mxd
			}

			//mxd. Flags and activations
			flags.Items.Clear();
			
			// Add activations
			foreach(LinedefActivateInfo ai in General.Map.Config.LinedefActivates)
			{
				if(l.Flags.ContainsKey(ai.Key) && l.Flags[ai.Key])
					flags.Items.Add(new ListViewItem(ai.Title) { Checked = true, ForeColor = SystemColors.HotTrack });
			}

			// And flags
			foreach(KeyValuePair<string, string> group in General.Map.Config.LinedefFlags) 
			{
				if(l.Flags.ContainsKey(group.Key) && l.Flags[group.Key])
					flags.Items.Add(new ListViewItem(group.Value) { Checked = true });
			}

			// And front flags
			if(l.Front != null)
			{
				foreach(KeyValuePair<string, string> group in General.Map.Config.SidedefFlags) 
				{
					if(l.Front.Flags.ContainsKey(group.Key) && l.Front.Flags[group.Key])
						flags.Items.Add(new ListViewItem("Front: " + group.Value) { Checked = true });
				}
			}

			// And back flags
			if(l.Back != null) 
			{
				foreach(KeyValuePair<string, string> group in General.Map.Config.SidedefFlags) 
				{
					if(l.Back.Flags.ContainsKey(group.Key) && l.Back.Flags[group.Key])
						flags.Items.Add(new ListViewItem("Back: " + group.Value) { Checked = true });
				}
			}

			//mxd. Flags panel visibility and size
			flagsPanel.Visible = (flags.Items.Count > 0);
			if(flags.Items.Count > 0) 
			{
				Rectangle rect = flags.GetItemRect(0);
				int itemspercolumn = 1;

				// Check how many items per column we have...
				for(int i = 1; i < flags.Items.Count; i++)
				{
					if(flags.GetItemRect(i).X != rect.X) break;
					itemspercolumn++;
				}

				flags.Width = rect.Width * (int)Math.Ceiling(flags.Items.Count / (float)itemspercolumn);
				flagsPanel.Width = flags.Width + flags.Left * 2;
			}

			// Show the whole thing
			this.Show();
            //this.Update(); // ano - don't think this is needed, and is slow
        }

        private static void UpdateTexturePanel(Panel panel, string texturename, Label texturenamelabel, Label sizelabel, int maxlabelright, ConfigurablePictureBox image, int sizeref, bool extendedinfoshown, bool required)
		{
			// Set texture name
			texturenamelabel.Text = (texturename.Length > DataManager.CLASIC_IMAGE_NAME_LENGTH ? texturename : texturename.ToUpperInvariant());

			// And image
			DisplayTextureImage(image, sizelabel, texturename, required);
			
			//Reposition texture name label?
			if(texturenamelabel.Width < image.Width + 2)
				texturenamelabel.Location = new Point(image.Location.X + (image.Width - texturenamelabel.Width) / 2, texturenamelabel.Location.Y);
			else
				texturenamelabel.Location = new Point(image.Location.X, texturenamelabel.Location.Y);

			// Resize panel
			if(!extendedinfoshown)
				panel.Width = Math.Max(texturenamelabel.Right + image.Location.X + 1, sizeref);
			else
				panel.Width = Math.Max(texturenamelabel.Right, maxlabelright) + image.Location.X;
		}

		//mxd
		private static bool SetPairedUDMFFieldsLabel(UniFields fields, string paramX, string paramY, double defaultvalue, Label namelabel, Label valuelabel, bool highlight)
		{
			double x = UniFields.GetFloat(fields, paramX, defaultvalue);
			double y = UniFields.GetFloat(fields, paramY, defaultvalue);

			if(fields.ContainsKey(paramX)) x = (double)fields[paramX].Value;
			if(fields.ContainsKey(paramY)) y = (double)fields[paramY].Value;

			if(x != defaultvalue || y != defaultvalue)
			{
				valuelabel.Text = x.ToString(CultureInfo.InvariantCulture) + ", " + y.ToString(CultureInfo.InvariantCulture);
				valuelabel.Enabled = true;
				namelabel.Enabled = true;
				return true;
			}
			else
			{
				valuelabel.Text = "--, --";
				valuelabel.Enabled = highlight;
				namelabel.Enabled = highlight;
				return false;
			}
		}

		//mxd
		private static void SetUDMFLight(Sidedef sd, Label label, Label value, bool highlight) 
		{
			if(sd.Fields.ContainsKey("light")) 
			{
				int light = (int)sd.Fields["light"].Value;
				
				if(sd.Fields.GetValue("lightabsolute", false))
					value.Text = light + " (abs.)";
				else
					value.Text = light + " (" + Math.Min(255, Math.Max(0, (light + sd.Sector.Brightness))) + ")";

				value.Enabled = true;
				label.Enabled = true;
			} 
			else 
			{
				value.Text = "-- (" + sd.Sector.Brightness + ")";
				label.Enabled = highlight;
				value.Enabled = highlight;
			}

			label.ForeColor = (highlight ? SystemColors.HotTrack : SystemColors.WindowText);
			value.ForeColor = label.ForeColor;
		}

		//mxd
		private static void SetArgumentText(ArgumentInfo info, Label label, int value) 
		{
			TypeHandler th = General.Types.GetArgumentHandler(info);
			th.SetValue(value);
			label.Text = th.GetStringValue();
		}

		// When visible changed
		protected override void OnVisibleChanged(EventArgs e)
		{
			// Hiding panels
			if(!this.Visible)
			{
				fronthightex.BackgroundImage = null;
				frontmidtex.BackgroundImage = null;
				frontlowtex.BackgroundImage = null;
				backhightex.BackgroundImage = null;
				backmidtex.BackgroundImage = null;
				backlowtex.BackgroundImage = null;
			}

			// Call base
			base.OnVisibleChanged(e);
		}

		// This shows a sidedef texture in a panel
		private static void DisplayTextureImage(ConfigurablePictureBox panel, Label sizelabel, string name, bool required)
		{
			// Check if name is a "none" texture
			if((name.Length < 1) || (name == "-"))
			{
				sizelabel.Visible = false; //mxd

				// Determine image to show
				if (required)
					panel.Image = Properties.Resources.MissingTexture;
				else
					panel.Image = null;
			}
			else
			{
				//mxd
				ImageData texture = General.Map.Data.GetTextureImage(name);
				if(General.Settings.ShowTextureSizes && texture.ImageState == ImageLoadState.Ready && !(texture is UnknownImage)) 
				{
					sizelabel.Visible = true;
					sizelabel.Text = Math.Abs(texture.ScaledWidth) + "x" + Math.Abs(texture.ScaledHeight);
				} 
				else 
				{
					sizelabel.Visible = false;
				}

				// Set the image
				panel.Image = texture.GetPreview();
			}
		}
	}
}
