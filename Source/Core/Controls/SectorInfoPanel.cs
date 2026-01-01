
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
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.Rendering;
using System.Collections.Generic;

#endregion

namespace CodeImp.DoomBuilder.Controls
{
	internal partial class SectorInfoPanel : UserControl
	{
		private readonly List<Label> floorinfolabels;
		private readonly List<Label> ceilinfolabels;
		private readonly List<Label> floorlabels;
		private readonly List<Label> ceillabels;
		
		// Constructor
		public SectorInfoPanel()
		{
			// Initialize
			InitializeComponent();
			CodeImp.DoomBuilder.General.ApplyMonoListViewFix(flags);

			
			// We have to set the parernt (and subsequently the new location relative to the parent) here since
			// we can't set the parent in the designer. And if the parent is not set to the ConfigurablePictureBox
			// the Label's background alpha will not work correctly
			labelFloorTextureSize.Parent = floortex;
			labelFloorTextureSize.Location = new Point(1, 1);
			labelCeilTextureSize.Parent = ceilingtex;
			labelCeilTextureSize.Location = new Point(1, 1);
			//mxd
			labelFloorTextureSize.BackColor = Color.FromArgb(128, labelFloorTextureSize.BackColor);
			labelCeilTextureSize.BackColor = Color.FromArgb(128, labelCeilTextureSize.BackColor);

			floorinfolabels = new List<Label> { floorAngle, floorLight, floorOffset, floorScale };
			ceilinfolabels = new List<Label> { ceilingAngle, ceilingLight, ceilingOffset, ceilingScale };
			floorlabels = new List<Label> { floorAngleLabel, floorLightLabel, floorOffsetLabel, floorScaleLabel };
			ceillabels = new List<Label> { ceilingAngleLabel, ceilingLightLabel, ceilingOffsetLabel, ceilingScaleLabel };
		}

		// This shows the info
		public void ShowInfo(Sector s, bool highlightceiling, bool highlightfloor)
		{
			int sheight = s.CeilHeight - s.FloorHeight;

			// Lookup effect description in config
			string effectinfo = s.Effect + " - " + General.Map.Config.GetSectorEffectInfo(s.Effect).Title; //mxd

			// Sector info
			sectorinfo.Text = " Sector " + s.Index + " (" + (s.Sidedefs == null ? "no" : s.Sidedefs.Count.ToString()) + " sidedefs)"; //mxd
			effect.Text = effectinfo;
			ceiling.Text = s.CeilHeight.ToString();
			floor.Text = s.FloorHeight.ToString();
			height.Text = sheight.ToString();
			brightness.Text = s.Brightness.ToString();
			floorname.Text = (s.FloorTexture.Length > DataManager.CLASIC_IMAGE_NAME_LENGTH ? s.FloorTexture : s.FloorTexture.ToUpperInvariant());
			ceilingname.Text = (s.CeilTexture.Length > DataManager.CLASIC_IMAGE_NAME_LENGTH ? s.CeilTexture : s.CeilTexture.ToUpperInvariant());

			//mxd. Set tags
			if(s.Tags.Count > 1)
			{
				string[] tags = new string[s.Tags.Count];
				for(int i = 0; i < s.Tags.Count; i++) tags[i] = s.Tags[i].ToString();
				tag.Text = string.Join(", ", tags);
				tag.Enabled = true;
				taglabel.Enabled = true;
				taglabel.Text = "Tags:";
			}
			else
			{
				tag.Text = s.Tag + (General.Map.Options.TagLabels.ContainsKey(s.Tag) ? " - " + General.Map.Options.TagLabels[s.Tag] : string.Empty);
				tag.Enabled = (s.Tag != 0);
				taglabel.Enabled = (s.Tag != 0);
				taglabel.Text = "Tag:";
			}

			//mxd
			effect.Enabled = (s.Effect != 0);
			effectlabel.Enabled = (s.Effect != 0);

			//mxd. Texture size
			if(s.LongFloorTexture == MapSet.EmptyLongName)
			{
				labelFloorTextureSize.Visible = false;
				floortex.Image = Properties.Resources.MissingTexture;
			} 
			else 
			{
				ImageData image = General.Map.Data.GetFlatImage(s.FloorTexture);
				DisplayTextureSize(labelFloorTextureSize, image);
				floortex.Image = image.GetPreview();
			}

			if(s.LongCeilTexture == MapSet.EmptyLongName) 
			{
				labelCeilTextureSize.Visible = false;
				ceilingtex.Image = Properties.Resources.MissingTexture;
			} 
			else 
			{
				ImageData image = General.Map.Data.GetFlatImage(s.CeilTexture);
				DisplayTextureSize(labelCeilTextureSize, image); //mxd
				ceilingtex.Image = image.GetPreview();
			}

			//mxd
			bool showExtededFloorInfo = false;
			bool showExtededCeilingInfo = false;
			if(General.Map.UDMF) 
			{
				if(s.Fields != null) 
				{
					//sector colors
					labelLight.Visible = true;
					labelFade.Visible = true;
					panelLightColor.Visible = true;
					panelFadeColor.Visible = true;

					if(s.Fields.ContainsKey("lightcolor")) 
					{
						panelLightColor.BackColor = PixelColor.FromInt(s.Fields.GetValue("lightcolor", 0xFFFFFF)).WithAlpha(255).ToColor();
						labelLight.Enabled = true;
					} 
					else 
					{
						panelLightColor.BackColor = SystemColors.Control;
						labelLight.Enabled = false;
					}

					if(s.Fields.ContainsKey("fadecolor")) 
					{
						panelFadeColor.BackColor = PixelColor.FromInt(s.Fields.GetValue("fadecolor", 0)).WithAlpha(255).ToColor();
						labelFade.Enabled = true;
					} 
					else 
					{
						panelFadeColor.BackColor = SystemColors.Control;
						labelFade.Enabled = false;
					}

					//light
					if(s.Fields.ContainsKey("lightceiling") || s.Fields.ContainsKey("lightceilingabsolute")) 
					{
						showExtededCeilingInfo = true;
						ceilingLight.Enabled = true;
						ceilingLightLabel.Enabled = true;

						int cl = s.Fields.GetValue("lightceiling", 0);

						if(s.Fields.GetValue("lightceilingabsolute", false))
							ceilingLight.Text = cl + " (abs.)";
						else
							ceilingLight.Text = cl + " (" + Math.Min(255, Math.Max(0, (cl + s.Brightness))) + ")";

					} 
					else 
					{
						ceilingLight.Text = "--";
						ceilingLight.Enabled = false;
						ceilingLightLabel.Enabled = false;
					}

					if(s.Fields.ContainsKey("lightfloor") || s.Fields.ContainsKey("lightfloorabsolute")) 
					{
						showExtededFloorInfo = true;
						floorLight.Enabled = true;
						floorLightLabel.Enabled = true;

						int fl = s.Fields.GetValue("lightfloor", 0);

						if(s.Fields.GetValue("lightfloorabsolute", false))
							floorLight.Text = fl + " (abs.)";
						else
							floorLight.Text = fl + " (" + Math.Min(255, Math.Max(0, (fl + s.Brightness))) + ")";

					} 
					else 
					{
						floorLight.Text = "--";
						floorLight.Enabled = false;
						floorLightLabel.Enabled = false;
					}

					//ceiling offsets
					double panX = s.Fields.GetValue("xpanningceiling", 0.0);
					double panY = s.Fields.GetValue("ypanningceiling", 0.0);

					if(panX != 0 || panY != 0) 
					{
						showExtededCeilingInfo = true;
						ceilingOffset.Enabled = true;
						ceilingOffsetLabel.Enabled = true;
						ceilingOffset.Text = panX.ToString(CultureInfo.InvariantCulture) + ", " + panY.ToString(CultureInfo.InvariantCulture);
					} 
					else 
					{
						ceilingOffset.Text = "--, --";
						ceilingOffset.Enabled = false;
						ceilingOffsetLabel.Enabled = false;
					}

					//floor offsets
					panX = s.Fields.GetValue("xpanningfloor", 0.0);
					panY = s.Fields.GetValue("ypanningfloor", 0.0);

					if(panX != 0 || panY != 0) 
					{
						showExtededFloorInfo = true;
						floorOffset.Enabled = true;
						floorOffsetLabel.Enabled = true;
						floorOffset.Text = panX.ToString(CultureInfo.InvariantCulture) + ", " + panY.ToString(CultureInfo.InvariantCulture);
					} 
					else 
					{
						floorOffset.Text = "--, --";
						floorOffset.Enabled = false;
						floorOffsetLabel.Enabled = false;
					}

					//ceiling scale
					double scaleX = s.Fields.GetValue("xscaleceiling", 1.0);
					double scaleY = s.Fields.GetValue("yscaleceiling", 1.0);

					if(scaleX != 1.0f || scaleY != 1.0f) 
					{
						showExtededCeilingInfo = true;
						ceilingScale.Enabled = true;
						ceilingScaleLabel.Enabled = true;
						ceilingScale.Text = scaleX.ToString(CultureInfo.InvariantCulture) + ", " + scaleY.ToString(CultureInfo.InvariantCulture);
					} 
					else 
					{
						ceilingScale.Text = "--, --";
						ceilingScale.Enabled = false;
						ceilingScaleLabel.Enabled = false;
					}

					//floor scale
					scaleX = s.Fields.GetValue("xscalefloor", 1.0);
					scaleY = s.Fields.GetValue("yscalefloor", 1.0);

					if(scaleX != 1.0 || scaleY != 1.0) 
					{
						showExtededFloorInfo = true;
						floorScale.Enabled = true;
						floorScaleLabel.Enabled = true;
						floorScale.Text = scaleX.ToString(CultureInfo.InvariantCulture) + ", " + scaleY.ToString(CultureInfo.InvariantCulture);
					} 
					else 
					{
						floorScale.Text = "--, --";
						floorScale.Enabled = false;
						floorScaleLabel.Enabled = false;
					}

					//rotation
					double ceilangle = s.Fields.GetValue("rotationceiling", 0.0);
					if(ceilangle != 0.0) 
					{
						showExtededCeilingInfo = true;
						ceilingAngle.Enabled = true;
						ceilingAngleLabel.Enabled = true;
						ceilingAngle.Text = ceilangle + "\u00B0";
					} 
					else 
					{
						ceilingAngle.Text = "--";
						ceilingAngle.Enabled = false;
						ceilingAngleLabel.Enabled = false;
					}

					double floorangle = s.Fields.GetValue("rotationfloor", 0.0);
					if(floorangle != 0f) 
					{
						showExtededFloorInfo = true;
						floorAngle.Enabled = true;
						floorAngleLabel.Enabled = true;
						floorAngle.Text = floorangle + "\u00B0";
					} 
					else 
					{
						floorAngle.Text = "--";
						floorAngle.Enabled = false;
						floorAngleLabel.Enabled = false;
					}
				}

				//Flags
				flags.Items.Clear();
				foreach(KeyValuePair<string, string> group in General.Map.Config.SectorFlags) 
				{
					if(s.Flags.ContainsKey(group.Key) && s.Flags[group.Key])
						flags.Items.Add(new ListViewItem(group.Value) { Checked = true });
				}
				foreach(KeyValuePair<string, string> group in General.Map.Config.CeilingPortalFlags)
				{
					if(s.Flags.ContainsKey(group.Key) && s.Flags[group.Key])
						flags.Items.Add(new ListViewItem(group.Value + " (ceil. portal)") { Checked = true });
				}
				foreach(KeyValuePair<string, string> group in General.Map.Config.FloorPortalFlags)
				{
					if(s.Flags.ContainsKey(group.Key) && s.Flags[group.Key])
						flags.Items.Add(new ListViewItem(group.Value + " (floor portal)") { Checked = true });
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

				//mxd. Toggle visibility
				foreach(Label label in floorinfolabels) label.Visible = showExtededFloorInfo;
				foreach(Label label in floorlabels) label.Visible = showExtededFloorInfo;
				foreach(Label label in ceilinfolabels) label.Visible = showExtededCeilingInfo;
				foreach(Label label in ceillabels) label.Visible = showExtededCeilingInfo;
			} 
			else 
			{
				panelFadeColor.Visible = false;
				panelLightColor.Visible = false;
				labelFade.Visible = false;
				labelLight.Visible = false;
				flagsPanel.Visible = false;
			}

			//mxd. Resize panels
			UpdateTexturePanel(ceilingpanel, ceilingname, ceilinfolabels, ceilingtex, ceilingOffsetLabel.Location.X - 1, showExtededCeilingInfo);
			UpdateTexturePanel(floorpanel, floorname, floorinfolabels, floortex, floorOffsetLabel.Location.X - 1, showExtededFloorInfo);

			//mxd. Highlight ceiling or floor?
			Color floorhighlightcolor = (highlightfloor ? SystemColors.HotTrack : SystemColors.WindowText);
			Color ceilinghighlightcolor = (highlightceiling ? SystemColors.HotTrack : SystemColors.WindowText);
			
			floorpanel.ForeColor = floorhighlightcolor;
			floor.ForeColor = floorhighlightcolor;
			labelfloor.ForeColor = floorhighlightcolor;

			ceilingpanel.ForeColor = ceilinghighlightcolor;
			ceiling.ForeColor = ceilinghighlightcolor;
			labelceiling.ForeColor = ceilinghighlightcolor;

			// Show the whole thing
			this.Show();
            //this.Update(); // ano - don't think this is needed, and is slow
        }

        //mxd
        private static void UpdateTexturePanel(GroupBox panel, Label texturename, List<Label> proplabels, ConfigurablePictureBox image, int sizeref, bool extendedinfoshown)
		{
			//Reposition texture name label?
			if(texturename.Width < image.Width + 2) 
				texturename.Location = new Point(image.Location.X + (image.Width - texturename.Width) / 2, texturename.Location.Y);
			else 
				texturename.Location = new Point(image.Location.X - 1, texturename.Location.Y);
			
			// Resize panel
			if(!extendedinfoshown)
				panel.Width = Math.Max(texturename.Right + image.Location.X - 1, sizeref);
			else
				panel.Width = Math.Max(texturename.Right, GetMaxRight(proplabels)) + image.Location.X;
		}

		//mxd
		private static int GetMaxRight(IEnumerable<Label> labels)
		{
			int max = 0;
			foreach(Label label in labels) if(label.Right > max) max = label.Right;
			return max;
		}

		//mxd
		private static void DisplayTextureSize(Label label, ImageData texture) 
		{
			if(General.Settings.ShowTextureSizes && texture.ImageState == ImageLoadState.Ready 
				&& !string.IsNullOrEmpty(texture.Name) && !(texture is UnknownImage)) 
			{
				label.Visible = true;
				label.Text = Math.Abs(texture.ScaledWidth) + "x" + Math.Abs(texture.ScaledHeight);
			} 
			else 
			{
				label.Visible = false;
			}
		}

		// When visible changed
		protected override void OnVisibleChanged(EventArgs e)
		{
			// Hiding panels
			if(!this.Visible)
			{
				floortex.BackgroundImage = null;
				ceilingtex.BackgroundImage = null;
			}

			// Call base
			base.OnVisibleChanged(e);
		}
	}
}
