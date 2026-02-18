#region ================== Namespaces

using System;
using System.Linq;
using System.Windows.Forms;

#endregion

namespace CodeImp.DoomBuilder.Windows
{
	// Multiselection variant of ThingBrowserForm
	public partial class ThingMultipleBrowserForm : DelayedForm
	{
		// Variables
		private int[] selectedtypes;

		// Properties
		public int[] SelectedTypes { get { return selectedtypes; } }

		// Constructor
		public ThingMultipleBrowserForm(int[] types)
		{
			InitializeComponent();

			// Setup list
			thingslist.Setup();

			// Preselect given types
			thingslist.SelectMultipleTypes(types);
		}

		// This browses for thing types with multiselection
		// Returns new thing types or the same thing types when cancelled
		public static int[] BrowseThings(IWin32Window owner, int[] type)
		{
			ThingMultipleBrowserForm f = new ThingMultipleBrowserForm(type);
			if (f.ShowDialog(owner) == DialogResult.OK) type = f.SelectedTypes;
			f.Dispose();
			return type;
		}

		// OK clicked
		private void apply_Click(object sender, EventArgs e)
		{
			// Get the result
			selectedtypes = thingslist.GetMultiResult(selectedtypes);

			// Done
			this.DialogResult = DialogResult.OK;
			this.Close();
		}

		// Cancel clicked
		private void cancel_Click(object sender, EventArgs e)
		{
			// Leave
			this.DialogResult = DialogResult.Cancel;
			this.Close();
		}

		// Double-clicked an item
		private void thingslist_OnTypeDoubleClicked()
		{
			// OK
			apply_Click(this, EventArgs.Empty);
		}

		//mxd
		private void ThingMultipleBrowserForm_Shown(object sender, EventArgs e)
		{
			thingslist.FocusTextbox();
		}
	}
}