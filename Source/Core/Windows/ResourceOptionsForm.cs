
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
using System.Windows.Forms;
using System.IO;
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.Config;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using CodeImp.DoomBuilder.ZDoom;
using System.Threading;

#endregion

namespace CodeImp.DoomBuilder.Windows
{
	internal partial class ResourceOptionsForm : DelayedForm
	{
		// Variables
		private DataLocation res;
        private string startPath;
        private Controls.FolderSelectDialog dirdialog;
		private List<string> requiredarchives;

        // Properties
        public DataLocation ResourceLocation { get { return res; } }
		public GameConfiguration GameConfiguration { get; set; }

		//
		private bool _ischeckingrequiredarchives = false;
		private bool IsCheckingRequiredArchives
        {
			get
            {
				return _ischeckingrequiredarchives;
            }
			set
            {
				if (value)
                {
					apply.Enabled = false;
					cancel.Enabled = false;
					notfortesting.Enabled = false;
					dir_textures.Enabled = false;
					dir_flats.Enabled = false;
					ControlBox = false;
					checkingloader.Visible = true;
                }
				else
                {
					apply.Enabled = true;
					cancel.Enabled = true;
					notfortesting.Enabled = true;
					dir_textures.Enabled = true;
					dir_flats.Enabled = true;
					ControlBox = true;
					checkingloader.Visible = false;
                }
				_ischeckingrequiredarchives = value;
            }
        }

		// Constructor
		public ResourceOptionsForm(DataLocation settings, string caption, string startPath) //mxd. added startPath
		{
			// Initialize
			InitializeComponent();

#if NO_WIN32
			// No easy way to have case-insesitivity for non-Windows platforms
			wadfiledialog.Filter = "Doom WAD Files (*.wad)|*.wad;*.Wad;*.wAd;*.WAd;*.waD;*.WaD;*.wAD;*.WAD";
			pk3filedialog.Filter = "Doom PK3/PKE/PK7 Files (*.pk3;*.pke;*.pk7;*.ipk3;*.ipk7)|" +
				"*.pk3;*.Pk3;*.pK3;*.PK3;" +
				"*.pke;*.Pke;*.pKe;*.PKe;*.pkE;*.PkE;*.pKE;*.PKE;" +
				"*.pk7;*.Pk7;*.pK7;*.PK7;" +
				"*.ipk3;*.iPk3;*.ipK3;*.iPK3;*.Ipk3;*.IPk3;*.IpK3;*.IPK3;" +
				"*.ipk7;*.iPk7;*.ipK7;*.iPK7;*.Ipk7;*.IPk7;*.IpK7;*.IPK7";
#endif

			// Set caption
			this.Text = caption;

			//
			this.requiredarchives = new List<string>();

			// Apply settings from ResourceLocation
			this.res = settings;
			switch(res.type)
			{
				// Setup for WAD File
				case DataLocation.RESOURCE_WAD:
					wadlocation.Text = res.location;
					strictpatches.Checked = res.option1;
					break;

				// Setup for Directory
				case DataLocation.RESOURCE_DIRECTORY:
					dirlocation.Text = res.location;
					dir_textures.Checked = res.option1;
					dir_flats.Checked = res.option2;
					break;
					
				// Setup for PK3 File
				case DataLocation.RESOURCE_PK3:
					pk3location.Text = res.location;
					break;
			}
			
			// Select appropriate tab
			tabs.SelectedIndex = res.type;
			
			// Checkbox
			notfortesting.Checked = res.notfortesting;

            this.startPath = startPath;
		}

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
			if (IsCheckingRequiredArchives) e.Cancel = true;
        }

		public static List<string> CheckRequiredArchives(GameConfiguration config, DataLocation loc, CancellationToken token)
        {
#if DEBUG
			General.WriteLogLine(string.Format("CheckRequiredArchives (Config = {0})", config?.Name));
#endif

			if (config == null)
				return new List<string>();

			try
			{
				DataReader dr = null;
				List<string> requiredarchives = new List<string>();
				HashSet<string> classes = null;

				switch (loc.type)
				{
					case DataLocation.RESOURCE_WAD:
						dr = new WADReader(loc, config, true) { Silent = true };
						break;

					case DataLocation.RESOURCE_DIRECTORY:
						dr = new DirectoryReader(loc, config, true) { Silent = true };
						break;

					case DataLocation.RESOURCE_PK3:
						dr = new PK3Reader(loc, config, true) { Silent = true };
						break;
				}

				token.ThrowIfCancellationRequested();

				foreach (var arc in config.RequiredArchives)
				{
					bool found = true;

					token.ThrowIfCancellationRequested();

					foreach (RequiredArchiveEntry e in arc.Entries)
					{
						token.ThrowIfCancellationRequested();

						if (e.Class != null)
						{
							if (classes == null)
                            {
								classes = new HashSet<string>();

								// load ZScript
								var zscript = new ZScriptParser {
									NoWarnings = true,
									OnInclude = (parser, location) => {
										IEnumerable<TextResourceData> includeStreams = dr.GetZScriptData(location);
										foreach (TextResourceData data in includeStreams)
										{
											// Parse this data
											parser.Parse(data, false);

											//mxd. DECORATE lumps are interdepandable. Can't carry on...
											if (parser.HasError)
												break;
										}
									}
								};

								foreach (TextResourceData data in dr.GetZScriptData("ZSCRIPT"))
								{
									// Parse the data
									data.Stream.Seek(0, SeekOrigin.Begin);
									zscript.Parse(data, true);

									if (zscript.HasError)
										break;

									foreach (string cls in zscript.LastClasses)
										classes.Add(cls.ToLowerInvariant());
								}

#if DEBUG
								if (zscript.HasError)
									General.WriteLogLine(string.Format("CRA({0}): ZScript error: {1}", loc.location, zscript.ErrorDescription));
#endif

								// load DECORATE
								var decorate = new DecorateParser(zscript.AllActorsByClass) {
									NoWarnings = true,
									OnInclude = (parser, location) => {
										IEnumerable<TextResourceData> includeStreams = dr.GetDecorateData(location);
										foreach (TextResourceData data in includeStreams)
										{
											// Parse this data
											parser.Parse(data, false);

											//mxd. DECORATE lumps are interdepandable. Can't carry on...
											if (parser.HasError)
												break;
										}
									}
								};

								foreach (TextResourceData data in dr.GetDecorateData("DECORATE"))
								{
									// Parse the data
									data.Stream.Seek(0, SeekOrigin.Begin);
									decorate.Parse(data, true);

									if (decorate.HasError)
										break;

									foreach (string cls in decorate.LastClasses)
										classes.Add(cls.ToLowerInvariant());
								}

#if DEBUG
								if (decorate.HasError)
									General.WriteLogLine(string.Format("CRA({0}): DECORATE error: {1}", loc.location, decorate.ErrorDescription));
#endif
							}

							if (!classes.Contains(e.Class.ToLowerInvariant()))
                            {
#if DEBUG
								General.WriteLogLine(string.Format("CRA({2}): Does not contain class: {1} <- {0}", string.Join(",", classes), e.Class, loc.location));
#endif
								found = false;
								break;
                            }
						}

						if (e.Lump != null && !dr.FileExists(e.Lump))
						{
#if DEBUG
							General.WriteLogLine(string.Format("CRA({1}): Does not contain lump: {0}", e.Lump, loc.location));
#endif
							found = false;
							break;
						}
					}

					if (found)
                    {
						requiredarchives.Add(arc.ID);
                    }
				}

				dr.Dispose();

				return requiredarchives;
			}
			catch (OperationCanceledException)
            {
				throw;
            }
			catch (Exception e)
			{
				General.WriteLogLine(e.ToString());
				return new List<string>();
			}
		}

		private List<string> RunCheckRequiredArchives()
        {
			// thanks ms for making this a struct
			CancellationTokenSource dummySource = new CancellationTokenSource();
			List<string> output = CheckRequiredArchives(GameConfiguration, ToDataLocation(), dummySource.Token);
			dummySource.Dispose();
			return output;
        }

		private async void StartRequiredArchivesCheck()
        {
			IsCheckingRequiredArchives = true;

			try
			{
				requiredarchives = await Task.Run(() => RunCheckRequiredArchives());
			}
			catch
            {
				requiredarchives = new List<string>();
            }

			ApplyDefaultRequiredArchivesSetting();

			IsCheckingRequiredArchives = false;
        }

		private void ApplyDefaultRequiredArchivesSetting()
        {
			dir_textures.Checked = false;
			dir_flats.Checked = false;
			notfortesting.Checked = false;
			// if any of the detected required archives implies "not for testing" � disable it by default
			foreach (var arc in GameConfiguration.RequiredArchives)
            {
				if (requiredarchives.Contains(arc.ID) && arc.ExcludeFromTesting)
					notfortesting.Checked = true;
            }
        }

		private DataLocation ToDataLocation()
        {
			DataLocation res = new DataLocation();
			res.location = "";
			res.requiredarchives = requiredarchives;

			// Apply settings to ResourceLocation
			switch (tabs.SelectedIndex)
			{
				// Setup WAD File
				case DataLocation.RESOURCE_WAD:

					// Check if file is specified
					if ((wadlocation.Text.Length == 0) ||
					   (!File.Exists(wadlocation.Text)))
					{
						break;
					}
					else
					{
						// Apply settings
						res.type = DataLocation.RESOURCE_WAD;
						res.location = wadlocation.Text;
						res.option1 = strictpatches.Checked;
						res.option2 = false;
						res.notfortesting = notfortesting.Checked;
					}
					break;

				// Setup Directory
				case DataLocation.RESOURCE_DIRECTORY:

					// Check if directory is specified
					if ((dirlocation.Text.Length == 0) ||
					   (!Directory.Exists(dirlocation.Text)))
					{
						break;
					}
					else
					{
						// Apply settings
						res.type = DataLocation.RESOURCE_DIRECTORY;
						res.location = dirlocation.Text;
						res.option1 = dir_textures.Checked;
						res.option2 = dir_flats.Checked;
						res.notfortesting = notfortesting.Checked;
					}
					break;

				// Setup PK3 File
				case DataLocation.RESOURCE_PK3:

					// Check if file is specified
					if ((pk3location.Text.Length == 0) ||
					   (!File.Exists(pk3location.Text)))
					{
						break;
					}
					else
					{
						// Apply settings
						res.type = DataLocation.RESOURCE_PK3;
						res.location = pk3location.Text;
						res.option1 = false;
						res.option2 = false;
						res.notfortesting = notfortesting.Checked;
					}
					break;
			}

			return res;
		}

        // OK clicked
        private void apply_Click(object sender, EventArgs e)
		{
			res = ToDataLocation();
			if (res.location == "")
			{
				switch (tabs.SelectedIndex)
				{
					case DataLocation.RESOURCE_WAD:
						MessageBox.Show(this, "Please select a valid WAD File resource.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
						break;

					case DataLocation.RESOURCE_PK3:
						MessageBox.Show(this, "Please select a valid PK3, PKE, or PK7 File resource.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
						break;

					case DataLocation.RESOURCE_DIRECTORY:
						MessageBox.Show(this, "Please select a valid directory resource.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
						break;
				}
			}
			else
            {
				this.DialogResult = DialogResult.OK;
				this.Close();
			}
		}
		
		// Cancel clicked
		private void cancel_Click(object sender, EventArgs e)
		{
			// Just hide
			this.DialogResult = DialogResult.Cancel;
			this.Close();
		}

		// Browse WAD File clicked
		private void browsewad_Click(object sender, EventArgs e)
		{
			// Browse for WAD File
			if(wadfiledialog.ShowDialog(this) == DialogResult.OK)
			{
				// Use this file
				wadlocation.Text = wadfiledialog.FileName;
				StartRequiredArchivesCheck();
			}
		}

		// Browse Directory clicked
		private void browsedir_Click(object sender, EventArgs e)
		{
            // Browse for Directory
            dirdialog = new Controls.FolderSelectDialog();
            dirdialog.Title = "Select Resource Folder";

            if (string.IsNullOrEmpty(dirlocation.Text) || !Directory.Exists(dirlocation.Text))
            {
                //mxd
                if (!string.IsNullOrEmpty(startPath))
                {
                    string startDir = Path.GetDirectoryName(startPath);
                    if (Directory.Exists(startDir)) dirdialog.InitialDirectory = startDir + '\\';
                }
            }
            else
            {
                dirdialog.InitialDirectory = dirlocation.Text;
            }

            if (dirdialog.ShowDialog(this.Handle))
			{
                // Use this directory
                dirlocation.Text = dirdialog.FileName;
				StartRequiredArchivesCheck();
                dirdialog = null;
			}
		}

		// Browse PK3 File clicked
		private void browsepk3_Click(object sender, EventArgs e)
		{
			// Browse for PK3 File
			if(pk3filedialog.ShowDialog(this) == DialogResult.OK)
			{
				// Use this file
				pk3location.Text = pk3filedialog.FileName;
				StartRequiredArchivesCheck();
			}
		}

		// Link clicked
		private void pk3_link_Click(object sender, LinkLabelLinkClickedEventArgs e)
		{
			General.OpenWebsite("http://www.zdoom.org/wiki/Using_ZIPs_as_WAD_replacement");
		}

		private void pke_link_Click(object sender, LinkLabelLinkClickedEventArgs e)
		{
			General.OpenWebsite("https://eternity.youfailit.net/wiki/ZIP");
		}

		// Help
		private void ResourceOptionsForm_HelpRequested(object sender, HelpEventArgs hlpevent)
		{
			General.ShowHelp("w_resourceoptions.html");
			hlpevent.Handled = true;
		}

		private void wadlocation_Enter(object sender, EventArgs e)
		{
			// Stop textbox control from getting focus
			browsewad.Focus();
		}

		private void dirlocation_Enter(object sender, EventArgs e)
		{
			// Stop textbox control from getting focus
			browsedir.Focus();
		}

		private void pk3location_Enter(object sender, EventArgs e)
		{
			// Stop textbox control from getting focus
			browsepk3.Focus();
		}
	}
}