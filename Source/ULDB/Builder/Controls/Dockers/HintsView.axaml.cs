// ULDB: code-behind for HintsView.axaml. Upstream's HintsPanel (model-ported) receives the hints
// as RTF from HintsManager, which uses only {\b ...} and \par; this renders that subset.

using System.Text;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Media;
using ULDB.Shell;
using WF = System.Windows.Forms;

namespace CodeImp.DoomBuilder.Controls
{
	public partial class HintsView : UserControl
	{
		public HintsView()
		{
			InitializeComponent();
		}

		internal HintsView(HintsPanel model) : this()
		{
			ModelBinder binder = new ModelBinder(model);
			WF.RichTextBox text = binder.Field<WF.RichTextBox>("hints");
			binder.Custom(() => Render(text.Rtf, hints.Inlines), text);
			binder.UpdateNow();
		}

		private static readonly string[] MODIFIERS = { "Ctrl+", "Alt+", "Shift+" };

		/// <summary>A shortcut as key caps ("Ctrl" "D"), or a mouse with the button lit (LButton, RButton, MButton, scroll).</summary>
		private static void AddKey(InlineCollection target, string desc)
		{
			System.Collections.Generic.List<Control> parts = new System.Collections.Generic.List<Control>();
			bool more = true;
			while(more)
			{
				more = false;
				foreach(string m in MODIFIERS)
					if(desc.StartsWith(m) && desc.Length > m.Length) { parts.Add(KeyCap(m.TrimEnd('+'))); desc = desc.Substring(m.Length); more = true; break; }
			}
			switch(desc)
			{
				case "LButton": parts.Add(Mouse("left")); break;
				case "RButton": parts.Add(Mouse("right")); break;
				case "MButton": parts.Add(Mouse("middle")); break;
				case "ScrollUp": parts.Add(Mouse("wheel", "\u25B2")); break;
				case "ScrollDown": parts.Add(Mouse("wheel", "\u25BC")); break;
				case "ScrollLeft": parts.Add(Mouse("wheel", "\u25C0")); break;
				case "ScrollRight": parts.Add(Mouse("wheel", "\u25B6")); break;
				default: parts.Add(KeyCap(desc)); break;
			}
			StackPanel row = new StackPanel { Orientation = Avalonia.Layout.Orientation.Horizontal, Spacing = 2 };
			foreach(Control c in parts) row.Children.Add(c);
			// Same height as a text line, nudged down onto the text's baseline
			row.Height = 18;
			row.RenderTransform = new TranslateTransform(0, 3);
			target.Add(new InlineUIContainer { Child = row, BaselineAlignment = BaselineAlignment.Center });
		}

		private static Control KeyCap(string text)
		{
			Border cap = new Border
			{
				BorderThickness = new Avalonia.Thickness(1, 1, 1, 2), CornerRadius = new Avalonia.CornerRadius(4),
				Padding = new Avalonia.Thickness(5, 0), MinWidth = 18, Height = 18, Margin = new Avalonia.Thickness(1, 0),
				Child = new TextBlock { Text = text, FontSize = 11, FontWeight = FontWeight.SemiBold, LineHeight = 14, Margin = new Avalonia.Thickness(0, 0, 0, 3), VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center, HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center },
			};
			cap.Classes.Add("uldb-keycap");
			return cap;
		}

		// A small mouse; the lit part is the button (or the wheel, with an arrow for its direction)
		private static Control Mouse(string lit, string arrow = null)
		{
			Panel body = new Panel { Width = 14, Height = 18 };
			Avalonia.Controls.Shapes.Path Part(string data, string style, double thickness)
			{
				Avalonia.Controls.Shapes.Path p = new Avalonia.Controls.Shapes.Path { Data = Avalonia.Media.Geometry.Parse(data), StrokeThickness = thickness, StrokeLineCap = PenLineCap.Round, StrokeJoin = PenLineJoin.Round };
				p.Classes.Add(style); // the brushes come from Styles.axaml (theme resources aren't found before the control is in the window)
				return p;
			}
			if(lit == "left") body.Children.Add(Part("M2,7 A5,5 0 0 1 7,2 V7 Z", "uldb-mouse-lit", 0));
			if(lit == "right") body.Children.Add(Part("M7,2 A5,5 0 0 1 12,7 H7 Z", "uldb-mouse-lit", 0));
			if(lit == "middle" || lit == "wheel") body.Children.Add(Part("M5.8,3 H8.2 V7 H5.8 Z", "uldb-mouse-lit", 0));
			body.Children.Add(Part("M2,7 A5,5 0 0 1 7,2 A5,5 0 0 1 12,7 V11 A5,5 0 0 1 7,16 A5,5 0 0 1 2,11 Z M2,7 H12 M7,2 V7", "uldb-mouse-line", 1.2));
			Control mouse = new Viewbox { Width = 13, Height = 17, Margin = new Avalonia.Thickness(2, 0), Child = body };
			if(arrow == null) return mouse;
			StackPanel withArrow = new StackPanel { Orientation = Avalonia.Layout.Orientation.Horizontal, Spacing = 1 };
			withArrow.Children.Add(mouse);
			withArrow.Children.Add(new TextBlock { Text = arrow, FontSize = 8, VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center, Opacity = 0.8 });
			return withArrow;
		}

		/// <summary>Renders simple RTF: groups, \b / \b0, \par and \line; other control words are skipped.</summary>
		internal static void Render(string rtf, InlineCollection target)
		{
			target.Clear();
			if(string.IsNullOrEmpty(rtf)) return;

			StringBuilder run = new StringBuilder();
			System.Collections.Generic.Stack<(bool, bool)> boldstack = new System.Collections.Generic.Stack<(bool, bool)>();
			bool bold = false, key = false;

			void Flush()
			{
				if(run.Length == 0) return;
				// A key (ULDB marks them with \uldbkey) is drawn as key caps or a mouse, unless it's text like "X (not bound to a key)"
				if(key && !run.ToString().Contains("(")) AddKey(target, run.ToString().Trim());
				else target.Add(new Run(run.ToString()) { FontWeight = bold ? FontWeight.Bold : FontWeight.Normal });
				run.Clear();
			}

			for(int i = 0; i < rtf.Length; i++)
			{
				char c = rtf[i];
				if(c == '{') { Flush(); boldstack.Push((bold, key)); key = false; }
				else if(c == '}') { Flush(); if(boldstack.Count > 0) (bold, key) = boldstack.Pop(); }
				else if(c == '\\' && i + 1 < rtf.Length && !char.IsLetter(rtf[i + 1]))
				{
					run.Append(rtf[++i]); // escaped \ { }
				}
				else if(c == '\\')
				{
					int start = ++i;
					while(i < rtf.Length && char.IsLetter(rtf[i])) i++;
					string word = rtf.Substring(start, i - start);
					int numstart = i;
					while(i < rtf.Length && (char.IsDigit(rtf[i]) || rtf[i] == '-')) i++;
					string num = rtf.Substring(numstart, i - numstart);
					if(i < rtf.Length && rtf[i] == ' ') { } else i--; // a space ends the word and is consumed

					switch(word)
					{
						case "b": Flush(); bold = num != "0"; break;
						case "uldbkey": Flush(); key = true; break;
						case "par":
						case "line": Flush(); target.Add(new LineBreak()); break;
					}
				}
				else if(c != '\r' && c != '\n') run.Append(c);
			}
			Flush();
		}
	}
}
