// ULDB: code-behind for AboutView.axaml, the About window.

using System;
using System.IO;
using Avalonia.Controls;
using Avalonia.Input.Platform;
using Avalonia.Media.Imaging;
using ULDB;
using ULDB.Shell;

namespace CodeImp.DoomBuilder.Windows
{
	public partial class AboutView : Window
	{
		public AboutView()
		{
			InitializeComponent();
		}

		internal AboutView(AboutForm form) : this()
		{
			Title = "About " + Branding.NAME;
			appname.Text = Branding.NAME;
			tagline.Text = Branding.TAGLINE;
			notice.Text = Branding.UNOFFICIAL_NOTICE;
			disclosure.Text = Branding.AI_DISCLOSURE;

			// Cacky, from Icons/ next to the program (like the window icon).
			string path = Path.Combine(AppContext.BaseDirectory, "Icons", "uldb-256.png");
			if(File.Exists(path)) cacky.Source = new Bitmap(path);

			string hash = string.IsNullOrEmpty(General.CommitHash) || General.CommitHash.Trim('0').Length == 0 ? "" : General.CommitHash;
			string versiontext = Branding.VERSION;
			if(hash.Length > 0) versiontext += ", " + hash;
			version.Text = "Version: " + versiontext;
			copyversion.Content = BreezeIcons.Create("builder_copyselection", 16);
			copyversion.Click += async (s, e) =>
			{
				if(TopLevel.GetTopLevel(this)?.Clipboard is IClipboard clipboard) await clipboard.SetTextAsync(Branding.NAME + " " + versiontext);
			};

			repolink.Content = "ULDB on GitHub";
			issueslink.Content = "Report a problem";
			upstreamlink.Content = Branding.UPSTREAM_NAME;
			repolink.Click += (s, e) => General.OpenWebsite(Branding.REPOSITORY_URL);
			issueslink.Click += (s, e) => General.OpenWebsite(Branding.ISSUES_URL);
			upstreamlink.Click += (s, e) => General.OpenWebsite(Branding.UPSTREAM_URL);

			credits.Text = "Based on " + Branding.UPSTREAM_NAME + ", GZDoom Builder and Doom Builder 2, by Pascal vd Heiden (CodeImp), "
				+ "MaxED, and the Ultimate Doom Builder team and contributors; all credit for the editor itself goes to them. "
				+ "Licensed under the GNU General Public License v3.0. Built with .NET and Avalonia.";
			closebutton.Click += (s, e) => Close();
		}
	}
}
