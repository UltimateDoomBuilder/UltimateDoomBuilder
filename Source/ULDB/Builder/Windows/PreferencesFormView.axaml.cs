// ULDB: code-behind for PreferencesFormView.axaml, the window of upstream's PreferencesForm (model-ported).

using System.Linq;
using Avalonia.Controls;
using ULDB.Platform;
using ULDB.Shell;

namespace CodeImp.DoomBuilder.Windows
{
	public partial class PreferencesFormView : Window
	{
		public PreferencesFormView()
		{
			InitializeComponent();
		}

		internal PreferencesFormView(PreferencesForm form) : this()
		{
			new ModelBinder(form).BindByName(this).UpdateNow();
			SetUpUldbOptions();
		}

		// The options only ULDB has (UiOptions): shown on the Interface tab and written when OK is pressed.
		private static readonly string[] Scales = { "Automatic", "100%", "125%", "150%", "175%", "200%", "250%", "300%" };
		private static readonly int[] ScalePercent = { 0, 100, 125, 150, 175, 200, 250, 300 };
		private static readonly string[] Backgrounds = { "Follow the theme", "Black", "Dark grey", "Grey", "Checkerboard behind the pictures" };
		private static readonly int[] Rotations = { 1, 5, 10, 15, 30, 45 };
		private static readonly string[] PrefabTags = { "As when pasting", "Keep the prefab's tags", "Renumber tags", "Remove tags" };

		private void SetUpUldbOptions()
		{
			var settings = CodeImp.DoomBuilder.General.Settings;
			uldb_scale.ItemsSource = Scales;
			uldb_browserbg.ItemsSource = Backgrounds;
			uldb_rotation.ItemsSource = Rotations.Select(r => r.ToString()).ToList();
			uldb_prefabtags.ItemsSource = PrefabTags;

			int percent = settings.ReadSetting(UiOptions.UI_SCALE_KEY, 0);
			uldb_scale.SelectedIndex = System.Math.Max(0, System.Array.IndexOf(ScalePercent, percent));
			uldb_browserbg.SelectedIndex = System.Math.Clamp(settings.ReadSetting(UiOptions.BROWSER_BACKGROUND, 0), 0, Backgrounds.Length - 1);
			uldb_rotation.SelectedIndex = System.Math.Max(0, System.Array.IndexOf(Rotations, UiOptions.ThingRotationStep));
			uldb_prefabtags.SelectedIndex = System.Math.Clamp(settings.ReadSetting(UiOptions.PREFAB_TAGS, 0), 0, PrefabTags.Length - 1);

			// Upstream's OK handler (the model's) saves its settings; these go along with it
			apply.Click += (s, e) =>
			{
				settings.WriteSetting(UiOptions.UI_SCALE_KEY, ScalePercent[System.Math.Max(0, uldb_scale.SelectedIndex)]);
				settings.WriteSetting(UiOptions.BROWSER_BACKGROUND, System.Math.Max(0, uldb_browserbg.SelectedIndex));
				settings.WriteSetting(UiOptions.THING_ROTATION, Rotations[System.Math.Max(0, uldb_rotation.SelectedIndex)]);
				settings.WriteSetting(UiOptions.PREFAB_TAGS, System.Math.Max(0, uldb_prefabtags.SelectedIndex));
			};
		}
	}
}
