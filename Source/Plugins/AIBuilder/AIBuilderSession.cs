using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CodeImp.DoomBuilder.AIBuilder
{
	internal sealed class AIBuilderSession
	{
		private const string TranscriptMarker = "---TRANSCRIPT---";

		public string Sid { get; private set; }
		public DateTime CreatedUtc { get; private set; }
		public DateTime UpdatedUtc { get; set; }
		public string Provider { get; set; }
		public string Transcript { get; set; }
		public bool SentResourceCatalog { get; set; }
		public string WadPath { get; set; }
		public string WadTitle { get; set; }
		public string LevelName { get; set; }
		public List<string> AttachedImages { get; private set; }

		public string DisplayName
		{
			get
			{
				string map = string.Empty;
				if(!string.IsNullOrEmpty(WadTitle) || !string.IsNullOrEmpty(LevelName))
					map = "  [" + (string.IsNullOrEmpty(WadTitle) ? "Unsaved map" : WadTitle) + (string.IsNullOrEmpty(LevelName) ? "" : " " + LevelName) + "]";

				return Sid + map + "  " + UpdatedUtc.ToLocalTime().ToString("yyyy-MM-dd HH:mm");
			}
		}

		private AIBuilderSession() { }

		public static AIBuilderSession Create()
		{
			DateTime now = DateTime.UtcNow;
			return new AIBuilderSession
			{
				Sid = "UDB-" + now.ToString("yyyyMMdd-HHmmss") + "-" + Guid.NewGuid().ToString("N").Substring(0, 8),
				CreatedUtc = now,
				UpdatedUtc = now,
				Provider = "Codex CLI",
				SentResourceCatalog = false,
				WadPath = string.Empty,
				WadTitle = string.Empty,
				LevelName = string.Empty,
				AttachedImages = new List<string>(),
				Transcript = "AI Builder ready.\nAsk for a plan or a map-building operation.\n\n"
			};
		}

		public static AIBuilderSession Load(string path)
		{
			string[] lines = File.ReadAllLines(path, Encoding.UTF8);
			AIBuilderSession session = new AIBuilderSession { AttachedImages = new List<string>() };
			int transcriptStart = -1;

			for(int i = 0; i < lines.Length; i++)
			{
				if(lines[i] == TranscriptMarker)
				{
					transcriptStart = i + 1;
					break;
				}

				int eq = lines[i].IndexOf('=');
				if(eq == -1) continue;

				string key = lines[i].Substring(0, eq);
				string value = lines[i].Substring(eq + 1);

				if(key == "sid") session.Sid = value;
				else if(key == "createdUtc") session.CreatedUtc = DateTime.Parse(value, null, System.Globalization.DateTimeStyles.RoundtripKind);
				else if(key == "updatedUtc") session.UpdatedUtc = DateTime.Parse(value, null, System.Globalization.DateTimeStyles.RoundtripKind);
				else if(key == "provider") session.Provider = value;
				else if(key == "sentResourceCatalog") session.SentResourceCatalog = string.Equals(value, "true", StringComparison.OrdinalIgnoreCase);
				else if(key == "wadPath") session.WadPath = value;
				else if(key == "wadTitle") session.WadTitle = value;
				else if(key == "levelName") session.LevelName = value;
				else if(key == "imagePath" && !string.IsNullOrWhiteSpace(value)) session.AttachedImages.Add(value);
			}

			if(transcriptStart >= 0 && transcriptStart < lines.Length)
				session.Transcript = string.Join(Environment.NewLine, lines, transcriptStart, lines.Length - transcriptStart);
			else
				session.Transcript = string.Empty;

			if(string.IsNullOrEmpty(session.Sid))
				session.Sid = Path.GetFileNameWithoutExtension(path);
			if(session.CreatedUtc == DateTime.MinValue)
				session.CreatedUtc = File.GetCreationTimeUtc(path);
			if(session.UpdatedUtc == DateTime.MinValue)
				session.UpdatedUtc = File.GetLastWriteTimeUtc(path);
			if(string.IsNullOrEmpty(session.Provider))
				session.Provider = "Codex CLI";
			if(session.WadPath == null) session.WadPath = string.Empty;
			if(session.WadTitle == null) session.WadTitle = string.Empty;
			if(session.LevelName == null) session.LevelName = string.Empty;
			if(session.AttachedImages == null) session.AttachedImages = new List<string>();

			return session;
		}

		public void Save(string directory)
		{
			Directory.CreateDirectory(directory);
			UpdatedUtc = DateTime.UtcNow;

			StringBuilder data = new StringBuilder();
			data.AppendLine("sid=" + Sid);
			data.AppendLine("createdUtc=" + CreatedUtc.ToString("O"));
			data.AppendLine("updatedUtc=" + UpdatedUtc.ToString("O"));
			data.AppendLine("provider=" + Provider);
			data.AppendLine("sentResourceCatalog=" + (SentResourceCatalog ? "true" : "false"));
			data.AppendLine("wadPath=" + (WadPath ?? string.Empty));
			data.AppendLine("wadTitle=" + (WadTitle ?? string.Empty));
			data.AppendLine("levelName=" + (LevelName ?? string.Empty));
			if(AttachedImages != null)
			{
				foreach(string image in AttachedImages)
					if(!string.IsNullOrWhiteSpace(image))
						data.AppendLine("imagePath=" + image);
			}
			data.AppendLine(TranscriptMarker);
			data.Append(Transcript ?? string.Empty);

			File.WriteAllText(Path.Combine(directory, Sid + ".udbai"), data.ToString(), Encoding.UTF8);
		}
	}
}
