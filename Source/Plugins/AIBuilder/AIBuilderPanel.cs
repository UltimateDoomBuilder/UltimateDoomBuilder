using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Web.Script.Serialization;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Types;

namespace CodeImp.DoomBuilder.AIBuilder
{
	public sealed class AIBuilderPanel : UserControl
	{
		private readonly RichTextBox transcriptbox;
		private readonly TextBox inputbox;
		private readonly Button sendbutton;
		private readonly ComboBox providercombo;
		private readonly ComboBox sessioncombo;
		private readonly Button newsessionbutton;
		private readonly Button deletesessionbutton;
		private readonly Label statuslabel;
		private readonly System.Windows.Forms.Timer progresstimer;
		private readonly List<AIBuilderSession> sessions;
		private AIBuilderSession currentSession;
		private bool loadingsession;
		private string progressmessage;
		private int progressdots;

		private static string SessionsDirectory { get { return Path.Combine(General.AppPath, "AIBuilderSessions"); } }

		public AIBuilderPanel()
		{
			SuspendLayout();

			BackColor = Color.FromArgb(24, 25, 28);
			ForeColor = Color.FromArgb(235, 238, 242);
			MinimumSize = new Size(260, 220);

			Panel headerpanel = new Panel
			{
				Dock = DockStyle.Top,
				Height = 64,
				Padding = new Padding(8, 6, 8, 6),
				BackColor = Color.FromArgb(31, 33, 37)
			};

			Panel sessionrow = new Panel
			{
				Dock = DockStyle.Top,
				Height = 26,
				BackColor = Color.FromArgb(31, 33, 37)
			};

			Panel actionrow = new Panel
			{
				Dock = DockStyle.Top,
				Height = 26,
				Padding = new Padding(0, 4, 0, 0),
				BackColor = Color.FromArgb(31, 33, 37)
			};

			Label titlelabel = new Label
			{
				AutoSize = false,
				Dock = DockStyle.Fill,
				Font = new Font(Font.FontFamily, 9f, FontStyle.Bold),
				ForeColor = Color.FromArgb(245, 247, 250),
				Text = "AI Builder"
			};

			newsessionbutton = new Button
			{
				Dock = DockStyle.Right,
				FlatStyle = FlatStyle.Flat,
				Text = "New",
				Width = 58
			};
			newsessionbutton.Click += NewSessionButtonOnClick;

			deletesessionbutton = new Button
			{
				Dock = DockStyle.Right,
				FlatStyle = FlatStyle.Flat,
				Text = "Delete",
				Width = 72
			};
			deletesessionbutton.Click += DeleteSessionButtonOnClick;

			sessioncombo = new ComboBox
			{
				Dock = DockStyle.Fill,
				DropDownStyle = ComboBoxStyle.DropDownList,
				FlatStyle = FlatStyle.Flat,
				Width = 190
			};
			sessioncombo.SelectedIndexChanged += SessionComboOnSelectedIndexChanged;

			providercombo = new ComboBox
			{
				Dock = DockStyle.Right,
				DropDownStyle = ComboBoxStyle.DropDownList,
				FlatStyle = FlatStyle.Flat,
				Width = 98
			};
			providercombo.Items.Add("Codex CLI");
			providercombo.Items.Add("Claude Code");
			providercombo.SelectedIndex = 0;
			providercombo.SelectedIndexChanged += ProviderComboOnSelectedIndexChanged;

			sessionrow.Controls.Add(sessioncombo);
			sessionrow.Controls.Add(providercombo);
			actionrow.Controls.Add(titlelabel);
			actionrow.Controls.Add(deletesessionbutton);
			actionrow.Controls.Add(newsessionbutton);
			headerpanel.Controls.Add(actionrow);
			headerpanel.Controls.Add(sessionrow);

			transcriptbox = new RichTextBox
			{
				BackColor = Color.FromArgb(17, 18, 21),
				BorderStyle = BorderStyle.None,
				Dock = DockStyle.Fill,
				ForeColor = Color.FromArgb(225, 229, 235),
				ReadOnly = true,
				ScrollBars = RichTextBoxScrollBars.Vertical,
				DetectUrls = true,
				Font = new Font(Font.FontFamily, 9f),
				Text = "AI Builder ready.\nAsk for a plan or a map-building operation.\n\n"
			};

			statuslabel = new Label
			{
				AutoSize = false,
				Dock = DockStyle.Bottom,
				Height = 22,
				Padding = new Padding(8, 3, 8, 0),
				ForeColor = Color.FromArgb(158, 166, 178),
				Text = "Ready"
			};

			progresstimer = new System.Windows.Forms.Timer { Interval = 450 };
			progresstimer.Tick += ProgressTimerOnTick;

			Panel inputpanel = new Panel
			{
				Dock = DockStyle.Bottom,
				Height = 62,
				Padding = new Padding(8, 6, 8, 6),
				BackColor = Color.FromArgb(24, 25, 28)
			};

			sendbutton = new Button
			{
				Dock = DockStyle.Right,
				Height = 50,
				Width = 64,
				Text = "Send"
			};
			sendbutton.Click += SendButtonOnClick;

			inputbox = new TextBox
			{
				AcceptsReturn = false,
				BackColor = Color.FromArgb(36, 38, 43),
				BorderStyle = BorderStyle.FixedSingle,
				Dock = DockStyle.Fill,
				ForeColor = Color.FromArgb(245, 247, 250),
				Multiline = true,
				ScrollBars = ScrollBars.Vertical,
				Text = "Create a small starter room connected to the selected sector."
			};
			inputbox.KeyDown += InputBoxOnKeyDown;
			inputbox.AllowDrop = true;
			inputbox.DragEnter += AttachmentDragEnter;
			inputbox.DragDrop += AttachmentDragDrop;
			transcriptbox.AllowDrop = true;
			transcriptbox.DragEnter += AttachmentDragEnter;
			transcriptbox.DragDrop += AttachmentDragDrop;

			inputpanel.Controls.Add(inputbox);
			inputpanel.Controls.Add(sendbutton);

			Controls.Add(transcriptbox);
			Controls.Add(inputpanel);
			Controls.Add(statuslabel);
			Controls.Add(headerpanel);

			sessions = new List<AIBuilderSession>();
			LoadSessions();

			ResumeLayout(false);
		}

		private void InputBoxOnKeyDown(object sender, KeyEventArgs e)
		{
			if(e.Control && e.KeyCode == Keys.V && TryPasteAttachmentFromClipboard())
			{
				e.SuppressKeyPress = true;
				return;
			}

			if(e.KeyCode != Keys.Enter || e.Shift) return;

			e.SuppressKeyPress = true;
			SendPrompt();
		}

		private void SendButtonOnClick(object sender, EventArgs e)
		{
			SendPrompt();
		}

		private void NewSessionButtonOnClick(object sender, EventArgs e)
		{
			CreateAndSelectSession();
		}

		private void DeleteSessionButtonOnClick(object sender, EventArgs e)
		{
			DeleteSelectedSession();
		}

		private void SessionComboOnSelectedIndexChanged(object sender, EventArgs e)
		{
			if(loadingsession || sessioncombo.SelectedIndex < 0 || sessioncombo.SelectedIndex >= sessions.Count) return;
			SelectSession(sessions[sessioncombo.SelectedIndex]);
		}

		private void ProviderComboOnSelectedIndexChanged(object sender, EventArgs e)
		{
			if(loadingsession) return;
			if(currentSession == null) return;
			currentSession.Provider = providercombo.Text;
			SaveCurrentSession();
		}

		private async void SendPrompt()
		{
			if(currentSession == null)
				CreateAndSelectSession();

			string prompt = inputbox.Text.Trim();
			if(string.IsNullOrWhiteSpace(prompt))
			{
				statuslabel.Text = "Write a request first";
				return;
			}

			inputbox.Clear();
			AppendMessage("You", prompt, Color.FromArgb(245, 247, 250));
			SaveCurrentSession();

			SetBusy(true);
			try
			{
				AppendMessage("AI Builder", "Voy a preparar un plan corto y despues empiezo a trabajar en el mapa.", Color.FromArgb(156, 230, 180));
				StartProgress("Planificando");
				string planoutput = await RunSelectedAgent(prompt, true, null);
				string plantasks;
				if(TryExtractAgentTasks(planoutput, out plantasks))
					AppendMessage("Plan", plantasks, Color.FromArgb(255, 218, 140));
				else
					AppendMessage(providercombo.Text, planoutput, Color.FromArgb(183, 218, 255));

				StartProgress("Procesando");
				AppendMessage("AI Builder", "Procesando el plan y aplicando cambios cuando el agente entregue acciones.", Color.FromArgb(156, 230, 180));
				string output = await RunSelectedAgent(prompt, false, planoutput);
				AppendMessage(providercombo.Text, output, Color.FromArgb(183, 218, 255));
				string tasksummary;
				if(TryExtractAgentTasks(output, out tasksummary))
					AppendMessage("Tasks", tasksummary, Color.FromArgb(255, 218, 140));
				string applyresult;
				if(TryApplyAgentActions(output, out applyresult))
					AppendMessage("AI Builder", applyresult, Color.FromArgb(156, 230, 180));
				StopProgress("Ready");
				statuslabel.Text = "Ready";
				SaveCurrentSession();
			}
			catch(Exception ex)
			{
				StopProgress("Agent failed");
				AppendMessage("Error", ex.Message, Color.FromArgb(255, 170, 170));
				statuslabel.Text = "Agent failed";
				SaveCurrentSession();
			}
			finally
			{
				SetBusy(false);
			}
		}

		private Task<string> RunSelectedAgent(string prompt)
		{
			return RunSelectedAgent(prompt, false, null);
		}

		private Task<string> RunSelectedAgent(string prompt, bool planningOnly, string planContext)
		{
			string provider = providercombo.SelectedItem as string;
			string enrichedPrompt = BuildAgentPrompt(prompt, planningOnly, planContext);
			List<string> images = GetExistingAttachedImages();
			return Task.Run(() =>
			{
				if(provider == "Claude Code")
					return RunClaude(enrichedPrompt);

				return RunCodex(enrichedPrompt, images);
			});
		}

		private string BuildAgentPrompt(string prompt)
		{
			return BuildAgentPrompt(prompt, false, null);
		}

		private string BuildAgentPrompt(string prompt, bool planningOnly, string planContext)
		{
			UpdateCurrentSessionMapIdentity();

			StringBuilder full = new StringBuilder();
			full.AppendLine("You are the AI Builder assistant running inside Ultimate Doom Builder.");
			full.AppendLine("UDB AI session SID: " + currentSession.Sid);
			full.AppendLine("Current WAD: " + (string.IsNullOrEmpty(currentSession.WadTitle) ? "(unsaved map)" : currentSession.WadTitle));
			full.AppendLine("Current level: " + (string.IsNullOrEmpty(currentSession.LevelName) ? "(unknown)" : currentSession.LevelName));
			full.AppendLine("Attached images: " + AttachedImagesText());
			full.AppendLine("Only this plugin owns sessions with SID prefix UDB-. Treat this SID as the conversation identity.");
			full.AppendLine("The map snapshot below is the current authoritative state. Use it as context before answering.");
			if(planningOnly)
			{
				full.AppendLine("Planning pass only: do not include executable map actions. Return a short natural-language intro and exactly one fenced JSON block with tasks only.");
				full.AppendLine("Use statuses pending and in_progress. Do not mark work done before the execution pass.");
			}
			else
			{
				full.AppendLine("Execution pass: briefly say what you are going to apply, then include exactly one JSON object in a fenced ```json block with updated tasks and executable actions.");
				if(!string.IsNullOrWhiteSpace(planContext))
				{
					full.AppendLine("Plan from the previous planning pass:");
					full.AppendLine(TrimForPrompt(planContext, 4000));
				}
			}
			full.AppendLine("When you want UDB to modify the map, include exactly one JSON object in a fenced ```json block.");
			full.AppendLine("Do not claim the map was changed unless you include an actions JSON block.");
			full.AppendLine("Supported action schema:");
			full.AppendLine("{ \"tasks\": [ { \"id\": \"layout\", \"title\": \"Create the connected layout\", \"status\": \"in_progress\" } ], \"actions\": [ { \"type\": \"create_sector\", \"vertices\": [{\"x\":0,\"y\":0},{\"x\":192,\"y\":0},{\"x\":192,\"y\":192},{\"x\":0,\"y\":192}], \"sector\": { \"floorHeight\": 0, \"ceilHeight\": 128, \"brightness\": 192, \"effect\": 0, \"floorTexture\": \"FLOOR0_1\", \"ceilTexture\": \"CEIL1_1\" }, \"walls\": { \"upper\": \"STARTAN3\", \"middle\": \"STARTAN3\", \"lower\": \"STARTAN3\", \"fillRequiredOnly\": true } } ] }");
			full.AppendLine("Additional executable actions:");
			full.AppendLine("{ \"type\": \"set_sector_properties\", \"sectors\": [0,1], \"sector\": { \"floorHeight\": 0, \"ceilHeight\": 128, \"brightness\": 192, \"effect\": 0, \"floorTexture\": \"FLOOR0_1\", \"ceilTexture\": \"CEIL1_1\" } }");
			full.AppendLine("{ \"type\": \"set_linedef_textures\", \"linedefs\": [0,1], \"side\": \"front\", \"textures\": { \"upper\": \"STARTAN3\", \"middle\": \"STARTAN3\", \"lower\": \"STARTAN3\", \"offsetX\": 0, \"offsetY\": 0 } }");
			full.AppendLine("{ \"type\": \"set_sidedef_textures\", \"sidedefs\": [75,81], \"textures\": { \"upper\": \"STARTAN3\", \"middle\": \"STARTAN3\", \"lower\": \"STARTAN3\", \"offsetX\": 0, \"offsetY\": 0 } }");
			full.AppendLine("{ \"type\": \"complete_sidedef_textures\", \"sidedefs\": [75,81], \"textures\": { \"upper\": \"STARTAN3\", \"middle\": \"STARTAN3\", \"lower\": \"STARTAN3\" } }");
			full.AppendLine("{ \"type\": \"create_thing\", \"things\": [ { \"type\": 3004, \"x\": 128, \"y\": 128, \"z\": 0, \"angle\": 90, \"tag\": 0, \"action\": 0, \"args\": [0,0,0,0,0], \"flags\": { \"skill1\": true, \"skill2\": true, \"skill3\": true, \"skill4\": true, \"skill5\": true } } ] }");
			full.AppendLine("{ \"type\": \"validate_map\" }");
			full.AppendLine("Vertices must describe a closed polygon without repeating the first point; UDB will close it.");
			full.AppendLine("For multi-step user requests, include tasks in the same JSON block. Task status must be pending, in_progress, done, or blocked.");
			full.AppendLine("Use tasks as a visible build checklist: split layout, connectivity, heights, tags, texture choices, and validation when useful.");
			full.AppendLine("Use set_sector_properties to adjust existing sector heights, brightness, tags, effects, and floor/ceiling flats.");
			full.AppendLine("Do not assign sector tags by default. Sector tags are only for gameplay links with linedef actions, switches, walk triggers, scripted/object references, or deliberate Doom specials such as tag 666 boss behavior.");
			full.AppendLine("If a sector has no trigger/action relationship, omit tag/tags entirely or keep tag 0 / tags [].");
			full.AppendLine("Use set_linedef_textures to adjust existing wall textures. side may be front, back, or both.");
			full.AppendLine("Use set_sidedef_textures when the snapshot gives exact sd indexes. Use complete_sidedef_textures to fill only missing required sidedef parts.");
			full.AppendLine("Never leave a required sidedef part with texture '-'. If requiresUpper/requiresMiddle/requiresLower is true, set that texture before marking the task done.");
			full.AppendLine("Use create_thing to place player starts, items, keys, decorations, monsters, and other thing types. Use exact thing type ids from the resource catalog.");
			full.AppendLine("If attached images are present, interpret them as map references, sketches, screenshots, or layout documents.");
			full.AppendLine("Use validate_map after edits when the user asks for a finished construction.");
			full.AppendLine();
			full.AppendLine(BuildMapSnapshot());
			if(!currentSession.SentResourceCatalog)
			{
				full.AppendLine();
				full.AppendLine(BuildResourceCatalog());
				currentSession.SentResourceCatalog = true;
				SaveSession(currentSession);
			}
			full.AppendLine();
			full.AppendLine("Conversation transcript for this UDB session:");
			full.AppendLine(TrimForPrompt(currentSession.Transcript, 12000));
			full.AppendLine();
			full.AppendLine("Current user request:");
			full.AppendLine(prompt);
			return full.ToString();
		}

		private void LoadSessions()
		{
			loadingsession = true;
			try
			{
				sessions.Clear();
				sessioncombo.Items.Clear();

				if(Directory.Exists(SessionsDirectory))
				{
					string[] files = Directory.GetFiles(SessionsDirectory, "*.udbai", SearchOption.TopDirectoryOnly);
					Array.Sort(files);
					Array.Reverse(files);

					foreach(string file in files)
					{
						try
						{
							AIBuilderSession session = AIBuilderSession.Load(file);
							if(session.Sid.StartsWith("UDB-", StringComparison.Ordinal))
								sessions.Add(session);
						}
						catch { }
					}
				}

				foreach(AIBuilderSession session in sessions)
					sessioncombo.Items.Add(session.DisplayName);
			}
			finally
			{
				loadingsession = false;
			}

			if(sessions.Count == 0)
				CreateAndSelectSession();
			else
				SelectSession(sessions[0]);
		}

		private void CreateAndSelectSession()
		{
			AIBuilderSession session = AIBuilderSession.Create();
			UpdateSessionMapIdentity(session);
			sessions.Insert(0, session);
			SaveSession(session);
			RefreshSessionCombo(session);
			SelectSession(session);
		}

		private void DeleteSelectedSession()
		{
			if(currentSession == null || sessions.Count == 0) return;

			AIBuilderSession delete = currentSession;
			DialogResult choice = MessageBox.Show(this,
				"Delete this AI Builder session?" + Environment.NewLine + delete.DisplayName,
				"AI Builder",
				MessageBoxButtons.YesNo,
				MessageBoxIcon.Question);
			if(choice != DialogResult.Yes) return;

			int oldindex = sessions.IndexOf(delete);
			sessions.Remove(delete);

			try
			{
				string sessionfile = Path.Combine(SessionsDirectory, delete.Sid + ".udbai");
				if(File.Exists(sessionfile)) File.Delete(sessionfile);

				string attachments = GetSessionAttachmentDirectory(delete);
				if(Directory.Exists(attachments)) Directory.Delete(attachments, true);
			}
			catch(Exception ex)
			{
				AppendMessage("Error", "Could not delete session: " + ex.Message, Color.FromArgb(255, 170, 170));
			}

			if(sessions.Count == 0)
			{
				CreateAndSelectSession();
				return;
			}

			int next = Math.Min(Math.Max(oldindex, 0), sessions.Count - 1);
			RefreshSessionCombo(sessions[next]);
			SelectSession(sessions[next]);
		}

		private void SelectSession(AIBuilderSession session)
		{
			if(session == null) return;

			loadingsession = true;
			try
			{
				currentSession = session;
				transcriptbox.Text = string.IsNullOrWhiteSpace(session.Transcript)
					? "AI Builder ready.\nAsk for a plan or a map-building operation.\n\n"
					: session.Transcript;

				int providerIndex = providercombo.Items.IndexOf(session.Provider);
				providercombo.SelectedIndex = providerIndex >= 0 ? providerIndex : 0;

				int sessionIndex = sessions.IndexOf(session);
				if(sessionIndex >= 0 && sessioncombo.SelectedIndex != sessionIndex)
					sessioncombo.SelectedIndex = sessionIndex;

				statuslabel.Text = "SID " + session.Sid;
				transcriptbox.SelectionStart = transcriptbox.TextLength;
				transcriptbox.ScrollToCaret();
			}
			finally
			{
				loadingsession = false;
			}
		}

		private void SaveCurrentSession()
		{
			if(currentSession == null) return;

			currentSession.Provider = providercombo.Text;
			currentSession.Transcript = transcriptbox.Text;
			UpdateCurrentSessionMapIdentity();
			SaveSession(currentSession);
			RefreshSessionCombo(currentSession);
		}

		private bool TryPasteAttachmentFromClipboard()
		{
			if(currentSession == null)
				CreateAndSelectSession();

			if(Clipboard.ContainsImage())
			{
				using(Image image = Clipboard.GetImage())
				{
					if(image == null) return false;
					AddImageAttachment(image, "clipboard");
				}

				return true;
			}

			if(Clipboard.ContainsFileDropList())
				return AddImageFiles(Clipboard.GetFileDropList());

			return false;
		}

		private void AttachmentDragEnter(object sender, DragEventArgs e)
		{
			if(e.Data.GetDataPresent(DataFormats.FileDrop))
				e.Effect = DragDropEffects.Copy;
		}

		private void AttachmentDragDrop(object sender, DragEventArgs e)
		{
			string[] files = e.Data.GetData(DataFormats.FileDrop) as string[];
			if(files == null || files.Length == 0) return;

			System.Collections.Specialized.StringCollection collection = new System.Collections.Specialized.StringCollection();
			collection.AddRange(files);
			AddImageFiles(collection);
		}

		private bool AddImageFiles(System.Collections.Specialized.StringCollection files)
		{
			if(files == null || files.Count == 0) return false;

			bool added = false;
			foreach(string file in files)
			{
				if(!File.Exists(file) || !IsImageFile(file)) continue;

				using(Image image = Image.FromFile(file))
					AddImageAttachment(image, Path.GetFileNameWithoutExtension(file));

				added = true;
			}

			return added;
		}

		private void AddImageAttachment(Image image, string sourceName)
		{
			string directory = GetSessionAttachmentDirectory(currentSession);
			Directory.CreateDirectory(directory);

			string basename = MakeSafeFileName(string.IsNullOrWhiteSpace(sourceName) ? "image" : sourceName);
			string filename = basename + "-" + DateTime.UtcNow.ToString("yyyyMMdd-HHmmss") + "-" + Guid.NewGuid().ToString("N").Substring(0, 6) + ".png";
			string path = Path.Combine(directory, filename);
			image.Save(path, ImageFormat.Png);

			currentSession.AttachedImages.Add(path);
			AppendMessage("Attachment", "Image attached: " + filename, Color.FromArgb(201, 188, 255));
			SaveCurrentSession();
		}

		private List<string> GetExistingAttachedImages()
		{
			List<string> images = new List<string>();
			if(currentSession == null || currentSession.AttachedImages == null) return images;

			foreach(string image in currentSession.AttachedImages)
				if(!string.IsNullOrWhiteSpace(image) && File.Exists(image))
					images.Add(image);

			return images;
		}

		private string AttachedImagesText()
		{
			List<string> images = GetExistingAttachedImages();
			if(images.Count == 0) return "(none)";

			StringBuilder sb = new StringBuilder();
			for(int i = 0; i < images.Count; i++)
			{
				if(i > 0) sb.Append("; ");
				sb.Append(Path.GetFileName(images[i]));
			}
			return sb.ToString();
		}

		private static string GetSessionAttachmentDirectory(AIBuilderSession session)
		{
			return Path.Combine(SessionsDirectory, session.Sid);
		}

		private static bool IsImageFile(string path)
		{
			string ext = Path.GetExtension(path).ToLowerInvariant();
			return ext == ".png" || ext == ".jpg" || ext == ".jpeg" || ext == ".bmp" || ext == ".gif";
		}

		private static string MakeSafeFileName(string value)
		{
			foreach(char c in Path.GetInvalidFileNameChars())
				value = value.Replace(c, '_');

			return string.IsNullOrWhiteSpace(value) ? "image" : value;
		}

		private void UpdateCurrentSessionMapIdentity()
		{
			if(currentSession == null) return;
			UpdateSessionMapIdentity(currentSession);
		}

		private static void UpdateSessionMapIdentity(AIBuilderSession session)
		{
			if(session == null) return;

			if(General.Map == null)
			{
				session.WadPath = string.Empty;
				session.WadTitle = string.Empty;
				session.LevelName = string.Empty;
				return;
			}

			session.WadPath = General.Map.FilePathName ?? string.Empty;
			session.WadTitle = !string.IsNullOrEmpty(General.Map.FileTitle)
				? General.Map.FileTitle
				: (string.IsNullOrEmpty(session.WadPath) ? "Unsaved map" : Path.GetFileName(session.WadPath));

			if(General.Map.Options != null)
			{
				if(!string.IsNullOrEmpty(General.Map.Options.LevelName))
					session.LevelName = General.Map.Options.LevelName;
				else
					session.LevelName = string.Empty;
			}
			else
			{
				session.LevelName = string.Empty;
			}
		}

		private static void SaveSession(AIBuilderSession session)
		{
			session.Save(SessionsDirectory);
		}

		private void RefreshSessionCombo(AIBuilderSession selected)
		{
			loadingsession = true;
			try
			{
				sessioncombo.Items.Clear();
				foreach(AIBuilderSession session in sessions)
					sessioncombo.Items.Add(session.DisplayName);

				int index = sessions.IndexOf(selected);
				if(index >= 0)
					sessioncombo.SelectedIndex = index;
			}
			finally
			{
				loadingsession = false;
			}
		}

		private static string RunCodex(string prompt, List<string> images)
		{
			string promptfile = Path.Combine(Path.GetTempPath(), "udb-ai-prompt-" + Guid.NewGuid().ToString("N") + ".txt");
			File.WriteAllText(promptfile, prompt, new UTF8Encoding(false));

			try
			{
				StringBuilder arguments = new StringBuilder();
				arguments.Append("/c codex exec --sandbox read-only --cd ");
				arguments.Append(Quote(FindWorkspacePath()));

				if(images != null)
				{
					foreach(string image in images)
					{
						arguments.Append(" --image ");
						arguments.Append(Quote(image));
					}
				}

				arguments.Append(" - < ");
				arguments.Append(Quote(promptfile));
				return RunCommand("cmd.exe", arguments.ToString(), null);
			}
			finally
			{
				try { File.Delete(promptfile); }
				catch { }
			}
		}

		private static string RunClaude(string prompt)
		{
			if(!CommandExists("claude"))
				return "Claude Code CLI was not found in PATH. Install it or add it to PATH, then select Claude Code again.";

			string arguments = "/c claude -p " + Quote(prompt);
			return RunCommand("cmd.exe", arguments, null);
		}

		private static string RunCommand(string filename, string arguments, string stdin)
		{
			string workspace = FindWorkspacePath();
			StringBuilder output = new StringBuilder();
			StringBuilder error = new StringBuilder();
			ManualResetEvent outputdone = new ManualResetEvent(false);
			ManualResetEvent errordone = new ManualResetEvent(false);
			ProcessStartInfo startinfo = new ProcessStartInfo
			{
				FileName = filename,
				Arguments = arguments,
				WorkingDirectory = workspace,
				CreateNoWindow = true,
				UseShellExecute = false,
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				RedirectStandardInput = stdin != null,
				StandardOutputEncoding = Encoding.UTF8,
				StandardErrorEncoding = Encoding.UTF8
			};

			using(Process process = new Process())
			{
				process.StartInfo = startinfo;
				process.OutputDataReceived += delegate(object sender, DataReceivedEventArgs e)
				{
					if(e.Data == null) outputdone.Set();
					else output.AppendLine(e.Data);
				};
				process.ErrorDataReceived += delegate(object sender, DataReceivedEventArgs e)
				{
					if(e.Data == null) errordone.Set();
					else error.AppendLine(e.Data);
				};
				process.Start();
				process.BeginOutputReadLine();
				process.BeginErrorReadLine();

				if(stdin != null)
				{
					process.StandardInput.Write(stdin);
					process.StandardInput.Close();
				}

				if(!process.WaitForExit(120000))
				{
					try { process.Kill(); }
					catch { }

					throw new TimeoutException("The agent did not finish within 120 seconds. Try a smaller request or use a local AI Builder command.");
				}

				outputdone.WaitOne(1000);
				errordone.WaitOne(1000);

				if(process.ExitCode != 0)
					throw new InvalidOperationException(string.IsNullOrWhiteSpace(error.ToString()) ? output.ToString() : error.ToString());

				string outputtext = output.ToString();
				string errortext = error.ToString();

				if(!string.IsNullOrWhiteSpace(errortext))
					outputtext = outputtext.TrimEnd() + Environment.NewLine + Environment.NewLine + errortext.Trim();

				return string.IsNullOrWhiteSpace(outputtext) ? "(No output)" : outputtext.Trim();
			}
		}

		private static bool TryApplyAgentActions(string agentOutput, out string result)
		{
			result = null;
			string json = ExtractJsonBlock(agentOutput);
			if(string.IsNullOrWhiteSpace(json))
				return false;

			object rootobj = new JavaScriptSerializer().DeserializeObject(json);
			Dictionary<string, object> root = rootobj as Dictionary<string, object>;
			if(root == null || !root.ContainsKey("actions"))
				return false;

			if(General.Map == null || General.Map.Map == null)
				throw new InvalidOperationException("Open or create a map before applying AI actions.");

			object[] actions = root["actions"] as object[];
			if(actions == null || actions.Length == 0)
				return false;

			int applied = 0;
			StringBuilder summary = new StringBuilder();

			foreach(object actionobj in actions)
			{
				Dictionary<string, object> action = actionobj as Dictionary<string, object>;
				if(action == null || !action.ContainsKey("type")) continue;

				string type = Convert.ToString(action["type"]);
				if(type == "create_sector")
				{
					ApplyCreateSector(action, summary);
					applied++;
				}
				else if(type == "set_sector_properties")
				{
					ApplySetSectorProperties(action, summary);
					applied++;
				}
				else if(type == "set_linedef_textures")
				{
					ApplySetLinedefTextures(action, summary);
					applied++;
				}
				else if(type == "set_sidedef_textures")
				{
					ApplySetSidedefTextures(action, summary);
					applied++;
				}
				else if(type == "complete_sidedef_textures")
				{
					ApplyCompleteSidedefTextures(action, summary);
					applied++;
				}
				else if(type == "create_thing")
				{
					ApplyCreateThing(action, summary);
					applied++;
				}
				else if(type == "validate_map")
				{
					ApplyValidateMap(summary);
					applied++;
				}
			}

			if(applied == 0)
				return false;

			result = "Applied " + applied + " AI action" + (applied == 1 ? "" : "s") + "." + Environment.NewLine + summary.ToString().TrimEnd();
			return true;
		}

		private static bool TryExtractAgentTasks(string agentOutput, out string result)
		{
			result = null;
			string json = ExtractJsonBlock(agentOutput);
			if(string.IsNullOrWhiteSpace(json))
				return false;

			object rootobj = new JavaScriptSerializer().DeserializeObject(json);
			Dictionary<string, object> root = rootobj as Dictionary<string, object>;
			if(root == null || !root.ContainsKey("tasks"))
				return false;

			object[] tasks = root["tasks"] as object[];
			if(tasks == null || tasks.Length == 0)
				return false;

			StringBuilder summary = new StringBuilder();
			foreach(object taskobj in tasks)
			{
				Dictionary<string, object> task = taskobj as Dictionary<string, object>;
				if(task == null) continue;

				string status = task.ContainsKey("status") ? Convert.ToString(task["status"]) : "pending";
				string title = task.ContainsKey("title") ? Convert.ToString(task["title"]) : (task.ContainsKey("id") ? Convert.ToString(task["id"]) : "Task");
				summary.AppendLine(TaskMarker(status) + " " + title);
			}

			if(summary.Length == 0)
				return false;

			result = summary.ToString().TrimEnd();
			return true;
		}

		private static string TaskMarker(string status)
		{
			if(string.Equals(status, "done", StringComparison.OrdinalIgnoreCase)) return "[x]";
			if(string.Equals(status, "in_progress", StringComparison.OrdinalIgnoreCase)) return "[-]";
			if(string.Equals(status, "blocked", StringComparison.OrdinalIgnoreCase)) return "[!]";
			return "[ ]";
		}

		private static void ApplyCreateSector(Dictionary<string, object> action, StringBuilder summary)
		{
			if(!action.ContainsKey("vertices"))
				throw new InvalidOperationException("create_sector requires a vertices array.");

			object[] vertexobjects = action["vertices"] as object[];
			if(vertexobjects == null || vertexobjects.Length < 3)
				throw new InvalidOperationException("create_sector requires at least 3 vertices.");

			List<DrawnVertex> vertices = new List<DrawnVertex>();
			foreach(object vertexobj in vertexobjects)
			{
				Dictionary<string, object> vertex = vertexobj as Dictionary<string, object>;
				if(vertex == null || !vertex.ContainsKey("x") || !vertex.ContainsKey("y"))
					throw new InvalidOperationException("Each create_sector vertex requires x and y.");

				vertices.Add(CreateDrawnVertex(ToDouble(vertex["x"]), ToDouble(vertex["y"])));
			}

			if(vertices[0].pos != vertices[vertices.Count - 1].pos)
				vertices.Add(CreateDrawnVertex(vertices[0].pos.x, vertices[0].pos.y));

			General.Map.Map.ClearAllMarks(false);
			General.Map.UndoRedo.CreateUndo("AI Builder: create AI sector");

			bool success = Tools.DrawLines(vertices, true, true);
			if(!success)
			{
				General.Map.UndoRedo.WithdrawUndo();
				throw new InvalidOperationException("UDB rejected the create_sector geometry.");
			}

			General.Map.Map.SnapAllToAccuracy();
			General.Map.Map.ClearAllSelected();

			Dictionary<string, object> sectorprops = action.ContainsKey("sector") ? action["sector"] as Dictionary<string, object> : null;
			int sectors = 0;
			foreach(Sector sector in General.Map.Map.GetMarkedSectors(true))
			{
				sector.Selected = true;
				ApplySectorProperties(sector, sectorprops);
				sectors++;
			}

			Dictionary<string, object> walls = action.ContainsKey("walls") ? action["walls"] as Dictionary<string, object> : null;
			int sidedefs = 0;
			if(walls != null)
			{
				bool fillRequiredOnly = !walls.ContainsKey("fillRequiredOnly") || ToBool(walls["fillRequiredOnly"]);
				foreach(Linedef linedef in General.Map.Map.GetMarkedLinedefs(true))
				{
					if(linedef.Front != null)
					{
						ApplySidedefTextures(linedef.Front, walls, fillRequiredOnly, true);
						sidedefs++;
					}

					if(linedef.Back != null)
					{
						ApplySidedefTextures(linedef.Back, walls, fillRequiredOnly, true);
						sidedefs++;
					}
				}
			}

			FinishMapChange();

			summary.AppendLine("create_sector: " + (vertices.Count - 1) + " vertices, " + sectors + " sector(s) selected, " + sidedefs + " sidedef(s) textured.");
		}

		private static void ApplySetSectorProperties(Dictionary<string, object> action, StringBuilder summary)
		{
			object[] sectorids = GetObjectArray(action, "sectors");
			Dictionary<string, object> sectorprops = action.ContainsKey("sector") ? action["sector"] as Dictionary<string, object> : null;
			if(sectorids == null || sectorids.Length == 0)
				throw new InvalidOperationException("set_sector_properties requires a sectors array.");
			if(sectorprops == null)
				throw new InvalidOperationException("set_sector_properties requires a sector object.");

			General.Map.UndoRedo.CreateUndo("AI Builder: set sector properties");
			int changed = 0;
			foreach(object sectorid in sectorids)
			{
				Sector sector = General.Map.Map.GetSectorByIndex(ToInt(sectorid));
				if(sector == null || sector.IsDisposed) continue;

				ApplySectorProperties(sector, sectorprops);
				sector.Selected = true;
				changed++;
			}

			FinishMapChange();
			summary.AppendLine("set_sector_properties: " + changed + " sector(s) updated.");
		}

		private static void ApplySetLinedefTextures(Dictionary<string, object> action, StringBuilder summary)
		{
			object[] linedefids = GetObjectArray(action, "linedefs");
			Dictionary<string, object> textures = action.ContainsKey("textures") ? action["textures"] as Dictionary<string, object> : null;
			if(linedefids == null || linedefids.Length == 0)
				throw new InvalidOperationException("set_linedef_textures requires a linedefs array.");
			if(textures == null)
				throw new InvalidOperationException("set_linedef_textures requires a textures object.");

			string side = action.ContainsKey("side") ? Convert.ToString(action["side"]).ToLowerInvariant() : "front";
			General.Map.UndoRedo.CreateUndo("AI Builder: set linedef textures");
			int changed = 0;
			foreach(object linedefid in linedefids)
			{
				Linedef linedef = General.Map.Map.GetLinedefByIndex(ToInt(linedefid));
				if(linedef == null || linedef.IsDisposed) continue;

				if((side == "front" || side == "both") && linedef.Front != null)
				{
					ApplySidedefTextures(linedef.Front, textures);
					changed++;
				}

				if((side == "back" || side == "both") && linedef.Back != null)
				{
					ApplySidedefTextures(linedef.Back, textures);
					changed++;
				}

				linedef.Selected = true;
			}

			FinishMapChange();
			summary.AppendLine("set_linedef_textures: " + changed + " sidedef(s) updated.");
		}

		private static void ApplySetSidedefTextures(Dictionary<string, object> action, StringBuilder summary)
		{
			object[] sidedefids = GetObjectArray(action, "sidedefs");
			Dictionary<string, object> textures = action.ContainsKey("textures") ? action["textures"] as Dictionary<string, object> : null;
			if(sidedefids == null || sidedefids.Length == 0)
				throw new InvalidOperationException("set_sidedef_textures requires a sidedefs array.");
			if(textures == null)
				throw new InvalidOperationException("set_sidedef_textures requires a textures object.");

			General.Map.UndoRedo.CreateUndo("AI Builder: set sidedef textures");
			int changed = 0;
			foreach(object sidedefid in sidedefids)
			{
				Sidedef side = General.Map.Map.GetSidedefByIndex(ToInt(sidedefid));
				if(side == null || side.IsDisposed) continue;

				ApplySidedefTextures(side, textures);
				changed++;
			}

			FinishMapChange();
			summary.AppendLine("set_sidedef_textures: " + changed + " sidedef(s) updated.");
		}

		private static void ApplyCompleteSidedefTextures(Dictionary<string, object> action, StringBuilder summary)
		{
			Dictionary<string, object> textures = action.ContainsKey("textures") ? action["textures"] as Dictionary<string, object> : null;
			if(textures == null)
				throw new InvalidOperationException("complete_sidedef_textures requires a textures object.");

			General.Map.UndoRedo.CreateUndo("AI Builder: complete sidedef textures");
			int changed = 0;
			object[] sidedefids = GetObjectArray(action, "sidedefs");
			if(sidedefids != null && sidedefids.Length > 0)
			{
				foreach(object sidedefid in sidedefids)
				{
					Sidedef side = General.Map.Map.GetSidedefByIndex(ToInt(sidedefid));
					if(side == null || side.IsDisposed) continue;

					changed += ApplySidedefTextures(side, textures, true, true);
				}
			}
			else
			{
				foreach(Sidedef side in General.Map.Map.Sidedefs)
				{
					if(side == null || side.IsDisposed) continue;
					changed += ApplySidedefTextures(side, textures, true, true);
				}
			}

			FinishMapChange();
			summary.AppendLine("complete_sidedef_textures: " + changed + " required missing texture slot(s) filled.");
		}

		private static void ApplyCreateThing(Dictionary<string, object> action, StringBuilder summary)
		{
			object[] thingobjects = GetObjectArray(action, "things");
			if(thingobjects == null || thingobjects.Length == 0)
				throw new InvalidOperationException("create_thing requires a things array or object.");

			General.Map.UndoRedo.CreateUndo("AI Builder: create thing");
			General.Map.Map.ClearAllSelected();

			int created = 0;
			foreach(object thingobj in thingobjects)
			{
				Dictionary<string, object> props = thingobj as Dictionary<string, object>;
				if(props == null)
					throw new InvalidOperationException("Each create_thing entry must be an object.");

				Thing thing = General.Map.Map.CreateThing();
				if(thing == null) continue;

				int type = props.ContainsKey("type") ? ToInt(props["type"]) : General.Settings.DefaultThingType;
				General.Settings.ApplyCleanThingSettings(thing, type);

				double x = props.ContainsKey("x") ? ToDouble(props["x"]) : 0.0;
				double y = props.ContainsKey("y") ? ToDouble(props["y"]) : 0.0;
				double z = props.ContainsKey("z") ? ToDouble(props["z"]) : 0.0;
				thing.Move(x, y, z);

				if(props.ContainsKey("angle")) thing.Rotate(ToInt(props["angle"]));
				if(props.ContainsKey("pitch")) thing.SetPitch(ToInt(props["pitch"]));
				if(props.ContainsKey("roll")) thing.SetRoll(ToInt(props["roll"]));
				if(props.ContainsKey("scaleX") || props.ContainsKey("scaleY"))
				{
					double sx = props.ContainsKey("scaleX") ? ToDouble(props["scaleX"]) : thing.ScaleX;
					double sy = props.ContainsKey("scaleY") ? ToDouble(props["scaleY"]) : thing.ScaleY;
					thing.SetScale(sx, sy);
				}

				if(props.ContainsKey("tag")) thing.Tag = ToInt(props["tag"]);
				if(props.ContainsKey("action")) thing.Action = ToInt(props["action"]);
				if(props.ContainsKey("args")) ApplyThingArgs(thing, props["args"]);
				if(props.ContainsKey("flags")) ApplyThingFlags(thing, props["flags"]);

				thing.DetermineSector();
				thing.UpdateConfiguration();
				thing.Selected = true;
				created++;
			}

			General.Map.ThingsFilter.Update();
			FinishMapChange();
			summary.AppendLine("create_thing: " + created + " thing(s) placed.");
		}

		private static void ApplyValidateMap(StringBuilder summary)
		{
			MapSet map = General.Map.Map;
			int disposed = 0;
			int missingMiddle = 0;
			int missingUpper = 0;
			int missingLower = 0;
			int badSectorHeights = 0;

			foreach(Linedef line in map.Linedefs)
			{
				if(line.IsDisposed)
				{
					disposed++;
					continue;
				}

				if(line.Front == null && line.Back == null)
					missingMiddle++;

				if(line.Front != null)
				{
					if(line.Back == null && line.Front.MiddleRequired() && line.Front.MiddleTexture == "-") missingMiddle++;
					if(line.Front.HighRequired() && line.Front.HighTexture == "-") missingUpper++;
					if(line.Front.LowRequired() && line.Front.LowTexture == "-") missingLower++;
				}

				if(line.Back != null)
				{
					if(line.Front == null && line.Back.MiddleRequired() && line.Back.MiddleTexture == "-") missingMiddle++;
					if(line.Back.HighRequired() && line.Back.HighTexture == "-") missingUpper++;
					if(line.Back.LowRequired() && line.Back.LowTexture == "-") missingLower++;
				}
			}

			foreach(Sector sector in map.Sectors)
				if(!sector.IsDisposed && sector.CeilHeight <= sector.FloorHeight)
					badSectorHeights++;

			summary.AppendLine("validate_map: disposed=" + disposed +
				", missingMiddle=" + missingMiddle +
				", missingUpper=" + missingUpper +
				", missingLower=" + missingLower +
				", badSectorHeights=" + badSectorHeights + ".");
		}

		private static void ApplySidedefTextures(Sidedef side, Dictionary<string, object> textures)
		{
			ApplySidedefTextures(side, textures, false, false);
		}

		private static int ApplySidedefTextures(Sidedef side, Dictionary<string, object> textures, bool requiredOnly, bool missingOnly)
		{
			int changed = 0;
			string upper = GetTextureValue(textures, "upper", "high");
			string middle = GetTextureValue(textures, "middle", "mid");
			string lower = GetTextureValue(textures, "lower", "low");

			if(ShouldSetSidedefTexture(side.HighRequired(), side.HighTexture, upper, requiredOnly, missingOnly))
			{
				side.SetTextureHigh(upper);
				changed++;
			}

			if(ShouldSetSidedefTexture(side.MiddleRequired(), side.MiddleTexture, middle, requiredOnly, missingOnly))
			{
				side.SetTextureMid(middle);
				changed++;
			}

			if(ShouldSetSidedefTexture(side.LowRequired(), side.LowTexture, lower, requiredOnly, missingOnly))
			{
				side.SetTextureLow(lower);
				changed++;
			}

			if(textures.ContainsKey("offsetX")) side.OffsetX = ToInt(textures["offsetX"]);
			if(textures.ContainsKey("offsetY")) side.OffsetY = ToInt(textures["offsetY"]);
			return changed;
		}

		private static bool ShouldSetSidedefTexture(bool required, string current, string next, bool requiredOnly, bool missingOnly)
		{
			if(string.IsNullOrEmpty(next)) return false;
			if(requiredOnly && !required) return false;
			if(missingOnly && current != "-") return false;
			return true;
		}

		private static string GetTextureValue(Dictionary<string, object> textures, string preferred, string alternate)
		{
			if(textures.ContainsKey(preferred)) return Convert.ToString(textures[preferred]);
			if(textures.ContainsKey(alternate)) return Convert.ToString(textures[alternate]);
			return null;
		}

		private static void ApplyThingArgs(Thing thing, object value)
		{
			object[] args = value as object[];
			if(args == null)
			{
				thing.Args[0] = ToInt(value);
				return;
			}

			int count = Math.Min(args.Length, Thing.NUM_ARGS);
			for(int i = 0; i < count; i++)
				thing.Args[i] = ToInt(args[i]);
		}

		private static void ApplyThingFlags(Thing thing, object value)
		{
			Dictionary<string, object> flags = value as Dictionary<string, object>;
			if(flags == null) return;

			foreach(KeyValuePair<string, object> flag in flags)
				thing.SetFlag(flag.Key, Convert.ToBoolean(flag.Value, System.Globalization.CultureInfo.InvariantCulture));
		}

		private static void FinishMapChange()
		{
			General.Map.Map.Update();
			General.Map.Data.UpdateUsedTextures();
			General.Map.Renderer2D.UpdateExtraFloorFlag();
			General.Map.IsChanged = true;
			General.Editing.Mode.OnScriptRunEnd();
		}

		private static void ApplySectorProperties(Sector sector, Dictionary<string, object> props)
		{
			if(props == null) return;

			if(props.ContainsKey("floorHeight")) sector.FloorHeight = ToInt(props["floorHeight"]);
			if(props.ContainsKey("ceilHeight")) sector.CeilHeight = ToInt(props["ceilHeight"]);
			if(props.ContainsKey("ceilingHeight")) sector.CeilHeight = ToInt(props["ceilingHeight"]);
			if(props.ContainsKey("brightness")) sector.Brightness = ToInt(props["brightness"]);
			if(props.ContainsKey("effect")) sector.Effect = ToInt(props["effect"]);
			if(props.ContainsKey("tag")) sector.Tag = ToInt(props["tag"]);
			if(props.ContainsKey("tags")) sector.Tags = ToIntList(props["tags"]);
			if(props.ContainsKey("floorTexture")) sector.SetFloorTexture(Convert.ToString(props["floorTexture"]));
			if(props.ContainsKey("ceilTexture")) sector.SetCeilTexture(Convert.ToString(props["ceilTexture"]));
			if(props.ContainsKey("ceilingTexture")) sector.SetCeilTexture(Convert.ToString(props["ceilingTexture"]));
		}

		private static string ExtractJsonBlock(string text)
		{
			if(string.IsNullOrWhiteSpace(text)) return null;

			const string fence = "```json";
			int start = text.IndexOf(fence, StringComparison.OrdinalIgnoreCase);
			if(start >= 0)
			{
				start += fence.Length;
				int end = text.IndexOf("```", start, StringComparison.Ordinal);
				if(end > start)
					return text.Substring(start, end - start).Trim();
			}

			start = text.IndexOf('{');
			int last = text.LastIndexOf('}');
			if(start >= 0 && last > start)
				return text.Substring(start, last - start + 1);

			return null;
		}

		private static int ToInt(object value)
		{
			return Convert.ToInt32(value, System.Globalization.CultureInfo.InvariantCulture);
		}

		private static double ToDouble(object value)
		{
			return Convert.ToDouble(value, System.Globalization.CultureInfo.InvariantCulture);
		}

		private static bool ToBool(object value)
		{
			return Convert.ToBoolean(value, System.Globalization.CultureInfo.InvariantCulture);
		}

		private static List<int> ToIntList(object value)
		{
			object[] values = value as object[];
			if(values == null)
				return new List<int> { ToInt(value) };

			List<int> result = new List<int>(values.Length);
			foreach(object item in values)
				result.Add(ToInt(item));

			if(result.Count == 0)
				result.Add(0);

			return result;
		}

		private static object[] GetObjectArray(Dictionary<string, object> source, string key)
		{
			if(!source.ContainsKey(key)) return null;

			object[] values = source[key] as object[];
			if(values != null) return values;

			return new object[] { source[key] };
		}

		private static DrawnVertex CreateDrawnVertex(double x, double y)
		{
			return new DrawnVertex
			{
				pos = new Vector2D(x, y),
				stitch = true,
				stitchline = true
			};
		}

		private static string BuildMapSnapshot()
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendLine("MAP SNAPSHOT");

			if(General.Map == null || General.Map.Map == null)
			{
				sb.AppendLine("mapOpen=false");
				return sb.ToString();
			}

			MapSet map = General.Map.Map;
			sb.AppendLine("mapOpen=true");
			sb.AppendLine("format=" + General.Map.Config.FormatInterface);
			sb.AppendLine("game=" + General.Map.Config.EngineName);
			sb.AppendLine("counts vertices=" + map.Vertices.Count + " linedefs=" + map.Linedefs.Count + " sidedefs=" + map.Sidedefs.Count + " sectors=" + map.Sectors.Count + " things=" + map.Things.Count);
			sb.AppendLine();

			sb.AppendLine("vertices:");
			foreach(Vertex v in map.Vertices)
				if(!v.IsDisposed)
					sb.AppendLine("  v" + v.Index + " x=" + F(v.Position.x) + " y=" + F(v.Position.y) + FieldsText(v.Fields));

			sb.AppendLine("linedefs:");
			foreach(Linedef l in map.Linedefs)
			{
				if(l.IsDisposed) continue;
				sb.AppendLine("  l" + l.Index +
					" start=v" + l.Start.Index +
					" end=v" + l.End.Index +
					" action=" + l.Action +
					" tags=" + TagsText(l.Tags) +
					" args=" + IntArrayText(l.Args) +
					" flags=" + FlagsText(l.GetEnabledFlags()) +
					" frontSector=" + (l.Front == null || l.Front.Sector == null ? "null" : l.Front.Sector.Index.ToString()) +
					" backSector=" + (l.Back == null || l.Back.Sector == null ? "null" : l.Back.Sector.Index.ToString()) +
					FieldsText(l.Fields));
			}

			sb.AppendLine("sidedefs:");
			foreach(Sidedef sd in map.Sidedefs)
			{
				if(sd.IsDisposed) continue;
				sb.AppendLine("  sd" + sd.Index +
					" line=l" + (sd.Line == null ? "null" : sd.Line.Index.ToString()) +
					" side=" + (sd.IsFront ? "front" : "back") +
					" sector=" + (sd.Sector == null ? "null" : sd.Sector.Index.ToString()) +
					" upperTex=" + sd.HighTexture +
					" middleTex=" + sd.MiddleTexture +
					" lowerTex=" + sd.LowTexture +
					" requiresUpper=" + BoolText(sd.HighRequired()) +
					" requiresMiddle=" + BoolText(sd.MiddleRequired()) +
					" requiresLower=" + BoolText(sd.LowRequired()) +
					" complete=" + BoolText(IsSidedefTextureComplete(sd)) +
					" offsetX=" + sd.OffsetX +
					" offsetY=" + sd.OffsetY +
					" flags=" + FlagsText(sd.GetEnabledFlags()) +
					FieldsText(sd.Fields));
			}

			sb.AppendLine("sectors:");
			foreach(Sector s in map.Sectors)
			{
				if(s.IsDisposed) continue;
				sb.AppendLine("  s" + s.Index +
					" floorHeight=" + s.FloorHeight +
					" ceilHeight=" + s.CeilHeight +
					" brightness=" + s.Brightness +
					" effect=" + s.Effect +
					" tags=" + TagsText(s.Tags) +
					" floorTex=" + s.FloorTexture +
					" ceilTex=" + s.CeilTexture +
					" flags=" + FlagsText(s.GetEnabledFlags()) +
					FieldsText(s.Fields));
			}

			sb.AppendLine("things:");
			foreach(Thing t in map.Things)
			{
				if(t.IsDisposed) continue;
				sb.AppendLine("  thing" + t.Index +
					" type=" + t.Type +
					" x=" + F(t.Position.x) +
					" y=" + F(t.Position.y) +
					" z=" + F(t.Position.z) +
					" angle=" + t.AngleDoom +
					" action=" + t.Action +
					" tag=" + t.Tag +
					" args=" + IntArrayText(t.Args) +
					" flags=" + FlagsText(t.GetEnabledFlags()) +
					FieldsText(t.Fields));
			}

			return sb.ToString();
		}

		private static string BuildResourceCatalog()
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendLine("RESOURCE CATALOG");
			sb.AppendLine("This catalog is sent once for this UDB AI session. Reuse these exact names for texture decisions.");

			if(General.Map == null || General.Map.Data == null)
			{
				sb.AppendLine("resourcesAvailable=false");
				return sb.ToString();
			}

			sb.AppendLine("resourcesAvailable=true");
			AppendNameCatalog(sb, "wallTextures", General.Map.Data.TextureNames, 600);
			AppendNameCatalog(sb, "flatsForFloorsAndCeilings", General.Map.Data.FlatNames, 600);
			AppendThingCatalog(sb, 450);
			return sb.ToString();
		}

		private static void AppendNameCatalog(StringBuilder sb, string title, IList<string> names, int max)
		{
			sb.Append(title);
			sb.Append(" count=");
			sb.Append(names == null ? 0 : names.Count);
			sb.AppendLine(":");

			if(names == null || names.Count == 0)
			{
				sb.AppendLine("  []");
				return;
			}

			int limit = Math.Min(names.Count, max);
			StringBuilder line = new StringBuilder("  ");
			for(int i = 0; i < limit; i++)
			{
				string name = names[i];
				if(string.IsNullOrEmpty(name)) continue;

				if(line.Length + name.Length + 2 > 120)
				{
					sb.AppendLine(line.ToString().TrimEnd());
					line.Length = 0;
					line.Append("  ");
				}

				line.Append(name);
				if(i < limit - 1) line.Append(", ");
			}

			if(line.Length > 2)
				sb.AppendLine(line.ToString().TrimEnd());

			if(names.Count > limit)
				sb.AppendLine("  ... truncated; ask the user to browse textures or use known names from this list.");
		}

		private static void AppendThingCatalog(StringBuilder sb, int max)
		{
			sb.AppendLine("thingTypes:");

			if(General.Map == null || General.Map.Data == null || General.Map.Data.ThingTypes == null)
			{
				sb.AppendLine("  []");
				return;
			}

			List<ThingTypeInfo> things = new List<ThingTypeInfo>(General.Map.Data.ThingTypes);
			things.Sort();

			int count = 0;
			foreach(ThingTypeInfo info in things)
			{
				if(info == null) continue;
				if(count >= max) break;

				sb.AppendLine("  type=" + info.Index +
					" title=\"" + info.Title + "\"" +
					" category=\"" + (info.Category == null ? "" : info.Category.Title) + "\"");
				count++;
			}

			if(things.Count > max)
				sb.AppendLine("  ... truncated; use common DoomEdNums or ask for a thing browser lookup.");
		}

		private static string FieldsText(UniFields fields)
		{
			if(fields == null || fields.Count == 0) return string.Empty;

			StringBuilder sb = new StringBuilder();
			sb.Append(" fields={");
			bool first = true;
			foreach(KeyValuePair<string, UniValue> field in fields)
			{
				if(!first) sb.Append(", ");
				first = false;
				sb.Append(field.Key);
				sb.Append(":");
				sb.Append(field.Value.Value);
			}
			sb.Append("}");
			return sb.ToString();
		}

		private static string FlagsText(IEnumerable<string> flags)
		{
			StringBuilder sb = new StringBuilder();
			bool first = true;
			sb.Append("[");
			foreach(string flag in flags)
			{
				if(!first) sb.Append(",");
				first = false;
				sb.Append(flag);
			}
			sb.Append("]");
			return sb.ToString();
		}

		private static string TagsText(IList<int> tags)
		{
			if(tags == null || tags.Count == 0) return "[]";
			return "[" + string.Join(",", tags) + "]";
		}

		private static string IntArrayText(int[] values)
		{
			if(values == null || values.Length == 0) return "[]";
			return "[" + string.Join(",", values) + "]";
		}

		private static string F(double value)
		{
			return value.ToString("0.###", System.Globalization.CultureInfo.InvariantCulture);
		}

		private static string BoolText(bool value)
		{
			return value ? "true" : "false";
		}

		private static bool IsSidedefTextureComplete(Sidedef side)
		{
			if(side.HighRequired() && side.HighTexture == "-") return false;
			if(side.MiddleRequired() && side.MiddleTexture == "-") return false;
			if(side.LowRequired() && side.LowTexture == "-") return false;
			return true;
		}

		private static string TrimForPrompt(string value, int maxChars)
		{
			if(string.IsNullOrEmpty(value) || value.Length <= maxChars)
				return value ?? string.Empty;

			return "... transcript trimmed ...\n" + value.Substring(value.Length - maxChars);
		}

		private static string FindWorkspacePath()
		{
			DirectoryInfo directory = new DirectoryInfo(General.AppPath);

			while(directory != null)
			{
				if(File.Exists(Path.Combine(directory.FullName, "Builder.sln")) || Directory.Exists(Path.Combine(directory.FullName, ".git")))
					return directory.FullName;

				directory = directory.Parent;
			}

			return General.AppPath;
		}

		private static bool CommandExists(string command)
		{
			try
			{
				using(Process process = new Process())
				{
					process.StartInfo = new ProcessStartInfo
					{
						FileName = "cmd.exe",
						Arguments = "/c where " + command,
						CreateNoWindow = true,
						UseShellExecute = false,
						RedirectStandardOutput = true,
						RedirectStandardError = true
					};
					process.Start();
					process.WaitForExit();
					return process.ExitCode == 0;
				}
			}
			catch
			{
				return false;
			}
		}

		private void SetBusy(bool busy)
		{
			if(InvokeRequired)
			{
				BeginInvoke(new Action<bool>(SetBusy), busy);
				return;
			}

			sendbutton.Enabled = !busy;
			inputbox.Enabled = !busy;
			providercombo.Enabled = !busy;
			sessioncombo.Enabled = !busy;
			newsessionbutton.Enabled = !busy;
			deletesessionbutton.Enabled = !busy;
			if(!busy && !progresstimer.Enabled)
				statuslabel.Text = statuslabel.Text;
		}

		private void StartProgress(string message)
		{
			if(InvokeRequired)
			{
				BeginInvoke(new Action<string>(StartProgress), message);
				return;
			}

			progressmessage = message;
			progressdots = 0;
			statuslabel.Text = progressmessage + "...";
			progresstimer.Start();
		}

		private void StopProgress(string message)
		{
			if(InvokeRequired)
			{
				BeginInvoke(new Action<string>(StopProgress), message);
				return;
			}

			progresstimer.Stop();
			progressmessage = string.Empty;
			progressdots = 0;
			statuslabel.Text = message;
		}

		private void ProgressTimerOnTick(object sender, EventArgs e)
		{
			progressdots = (progressdots + 1) % 4;
			statuslabel.Text = progressmessage + new string('.', progressdots + 1);
		}

		private void AppendMessage(string author, string message, Color color)
		{
			if(InvokeRequired)
			{
				BeginInvoke(new Action<string, string, Color>(AppendMessage), author, message, color);
				return;
			}

			transcriptbox.SelectionStart = transcriptbox.TextLength;
			transcriptbox.SelectionLength = 0;
			transcriptbox.SelectionColor = color;
			transcriptbox.SelectionFont = new Font(transcriptbox.Font, FontStyle.Bold);
			transcriptbox.AppendText(author + Environment.NewLine);
			transcriptbox.SelectionFont = transcriptbox.Font;
			transcriptbox.SelectionColor = Color.FromArgb(225, 229, 235);
			transcriptbox.AppendText(message + Environment.NewLine + Environment.NewLine);
			transcriptbox.SelectionStart = transcriptbox.TextLength;
			transcriptbox.ScrollToCaret();

			if(currentSession != null && !loadingsession)
				currentSession.Transcript = transcriptbox.Text;
		}

		private static string Quote(string value)
		{
			return "\"" + value.Replace("\"", "\\\"") + "\"";
		}
	}
}
