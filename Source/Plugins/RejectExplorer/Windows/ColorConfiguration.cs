using System;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Windows;

namespace CodeImp.DoomBuilder.RejectExplorer
{
	public partial class ColorConfiguration : DelayedForm
	{
		public ColorConfiguration()
		{
			InitializeComponent();

			defaultcolor.Color = PixelColor.FromInt(BuilderPlug.Me.ColorSettings.Default);
			highlightcolor.Color = PixelColor.FromInt(BuilderPlug.Me.ColorSettings.Highlight)	;
			bidirectionalcolor.Color = PixelColor.FromInt(BuilderPlug.Me.ColorSettings.Bidirectional);
			unidirectionalfromcolor.Color = PixelColor.FromInt(BuilderPlug.Me.ColorSettings.UnidirectionalFrom);
			unidirectionaltocolor.Color = PixelColor.FromInt(BuilderPlug.Me.ColorSettings.UnidirectionalTo);
		}

		private void okbutton_Click(object sender, EventArgs e)
		{
			BuilderPlug.Me.ColorSettings = new ColorSettings
			{
				Default = defaultcolor.Color.ToInt(),
				Highlight = highlightcolor.Color.ToInt(),
				Bidirectional = bidirectionalcolor.Color.ToInt(),
				UnidirectionalFrom = unidirectionalfromcolor.Color.ToInt(),
				UnidirectionalTo = unidirectionaltocolor.Color.ToInt()
			};

			General.Settings.WritePluginSetting("colors.default", defaultcolor.Color.ToInt());
			General.Settings.WritePluginSetting("colors.highlight", highlightcolor.Color.ToInt());
			General.Settings.WritePluginSetting("colors.bidirectional", bidirectionalcolor.Color.ToInt());
			General.Settings.WritePluginSetting("colors.unidirectionalfrom", unidirectionalfromcolor.Color.ToInt());
			General.Settings.WritePluginSetting("colors.unidirectionalto", unidirectionaltocolor.Color.ToInt());

			this.Close();
		}

		private void resetcolors_Click(object sender, EventArgs e)
		{
			defaultcolor.Color = PixelColor.FromInt(BuilderPlug.Me.DefaultColorSettings.Default);
			highlightcolor.Color = PixelColor.FromInt(BuilderPlug.Me.DefaultColorSettings.Highlight);
			bidirectionalcolor.Color = PixelColor.FromInt(BuilderPlug.Me.DefaultColorSettings.Bidirectional);
			unidirectionalfromcolor.Color = PixelColor.FromInt(BuilderPlug.Me.DefaultColorSettings.UnidirectionalFrom);
			unidirectionaltocolor.Color = PixelColor.FromInt(BuilderPlug.Me.DefaultColorSettings.UnidirectionalTo);
		}
	}
}
