// ULDB compatibility layer: state-only stand-ins for WinForms' common controls, so
// upstream designer code (InitializeComponent) and logic compile and run unchanged.
// The Avalonia views read and write their state and raise their events.

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms
{
	/// <summary>A control that holds other controls and tracks which one is focused (Form, UserControl).</summary>
	public class ContainerControl : Control
	{
		public Control ActiveControl { get; set; }
		public SizeF AutoScaleDimensions { get; set; }
		/// <summary>ULDB: no WinForms autoscaling; Avalonia scales the views, so this equals AutoScaleDimensions.</summary>
		public SizeF CurrentAutoScaleDimensions => AutoScaleDimensions.IsEmpty ? new SizeF(96f, 96f) : AutoScaleDimensions;
		public AutoScaleMode AutoScaleMode { get; set; }
		public bool AutoScroll { get; set; }
		public Form ParentForm
		{
			get
			{
				for(Control c = Parent; c != null; c = c.Parent) if(c is Form f) return f;
				return null;
			}
		}
		public bool Validate() => true;
		public void PerformAutoScale() { }
	}

	public class UserControl : ContainerControl
	{
		public bool AutoSize { get; set; }
		public AutoSizeMode AutoSizeMode { get; set; }
		public BorderStyle BorderStyle { get; set; }
		public event EventHandler Load;
		protected virtual void OnLoad(EventArgs e) => Load?.Invoke(this, e);

		/// <summary>ULDB: the view raises Load when it first shows the control.</summary>
		public void RaiseLoad() => OnLoad(EventArgs.Empty);
	}

	public interface IContainerControl
	{
		Control ActiveControl { get; set; }
		bool ActivateControl(Control active);
	}

	/// <summary>Text box state: text, selection and options. Views show and edit Text.</summary>
	public class TextBoxBase : Control
	{
		public bool ReadOnly { get; set; }
		public bool Multiline { get; set; }
		public bool WordWrap { get; set; } = true;
		public bool HideSelection { get; set; } = true;
		public bool ShortcutsEnabled { get; set; } = true;
		// Clipboard commands: the view's text box handles them itself.
		public void Cut() { }
		public void Copy() { }
		public void Paste() { }
		public int MaxLength { get; set; } = 32767;
		public BorderStyle BorderStyle { get; set; } = BorderStyle.Fixed3D;
		public int SelectionStart { get; set; }
		public int SelectionLength { get; set; }
		public string SelectedText { get; set; } = "";
		public int TextLength => Text.Length;
		public string[] Lines { get => Text.Split('\n'); set => Text = string.Join("\n", value ?? new string[0]); }
		public void Clear() => Text = "";
		public void AppendText(string text) => Text += text;
		public void SelectAll() { SelectionStart = 0; SelectionLength = Text.Length; }
		public void Select(int start, int length) { SelectionStart = start; SelectionLength = length; }
		/// <summary>ULDB: raised by ScrollToCaret(); the view scrolls to the end.</summary>
		public event EventHandler CaretScrollRequested;
		public void ScrollToCaret() => CaretScrollRequested?.Invoke(this, EventArgs.Empty);
	}

	public class TextBox : TextBoxBase
	{
		public AutoCompleteMode AutoCompleteMode { get; set; }
		public AutoCompleteSource AutoCompleteSource { get; set; } = AutoCompleteSource.None;
		public AutoCompleteStringCollection AutoCompleteCustomSource { get; set; } = new AutoCompleteStringCollection();
		public ScrollBars ScrollBars { get; set; }
		public HorizontalAlignment TextAlign { get; set; }
		public bool UseSystemPasswordChar { get; set; }
		public char PasswordChar { get; set; }
		public CharacterCasing CharacterCasing { get; set; }
		public bool AcceptsReturn { get; set; }
		public bool AcceptsTab { get; set; }
	}

	/// <summary>Rich text, kept as RTF; views render the subset they understand.</summary>
	public class RichTextBox : TextBoxBase
	{
		private string rtf = "";

		/// <summary>The content as RTF; setting Text stores plain text.</summary>
		public string Rtf { get => rtf; set { rtf = value ?? ""; base.Text = value ?? ""; } }

		/// <summary>Inserts RTF at the caret (ULDB: appends; upstream only uses it after Clear()).</summary>
		public string SelectedRtf { get => ""; set => Rtf = string.IsNullOrEmpty(rtf) ? value : rtf + value; }

		public override string Text { get => base.Text; set { rtf = ""; runs.Clear(); base.Text = value; } }
		private readonly System.Collections.Generic.List<(int start, int length, Color color)> runs = new System.Collections.Generic.List<(int, int, Color)>();
		/// <summary>ULDB: text ranges coloured through SelectionColor (RunExternalCommand's error lines).</summary>
		public System.Collections.Generic.IReadOnlyList<(int start, int length, Color color)> ColorRuns => runs;
		public Color SelectionColor { get => Color.Empty; set { if(SelectionLength > 0) { runs.Add((SelectionStart, SelectionLength, value)); NotifyStylesChanged(); } } }
		public new void AppendText(string text) { base.Text += text; }
		public bool DetectUrls { get; set; } = true;
		public RichTextBoxScrollBars ScrollBars { get; set; } = RichTextBoxScrollBars.Both;
		public bool AutoWordSelection { get; set; }
	}

	/// <summary>A label with a link; views raise LinkClicked when it's clicked.</summary>
	public class LinkLabel : Label
	{
		public Color LinkColor { get; set; } = Color.Blue;
		public Color ActiveLinkColor { get; set; } = Color.Red;
		public Color VisitedLinkColor { get; set; } = Color.Purple;
		public Color DisabledLinkColor { get; set; } = Color.Gray;
		public LinkBehavior LinkBehavior { get; set; }
		public LinkArea LinkArea { get; set; }
		public bool LinkVisited { get; set; }
		public event LinkLabelLinkClickedEventHandler LinkClicked;
		protected virtual void OnLinkClicked(LinkLabelLinkClickedEventArgs e) => LinkClicked?.Invoke(this, e);

		/// <summary>Links within the text (upstream's TagsSelector makes each tag a link).
		/// Without any, the whole text is one link.</summary>
		public LinkCollection Links { get; }

		public LinkLabel() { Links = new LinkCollection(this); }

		/// <summary>ULDB: the user clicked a link (null: the whole label) in the view.</summary>
		public void RaiseLinkClicked(Link link = null, MouseButtons button = MouseButtons.Left)
		{
			OnLinkClicked(new LinkLabelLinkClickedEventArgs(link ?? (Links.Count > 0 ? Links[0] : new Link(0, Text.Length)), button));
		}

		public class Link
		{
			public Link() { }
			public Link(int start, int length) { Start = start; Length = length; }
			public Link(int start, int length, object linkData) : this(start, length) { LinkData = linkData; }
			public int Start { get; set; }
			public int Length { get; set; }
			public object LinkData { get; set; }
			public string Name { get; set; } = "";
			public string Description { get; set; } = "";
			public bool Visited { get; set; }
			public bool Enabled { get; set; } = true;
			public object Tag { get; set; }
		}

		public class LinkCollection : List<Link>
		{
			private readonly LinkLabel owner;
			internal LinkCollection(LinkLabel owner) { this.owner = owner; }
			public new Link Add(Link link) { base.Add(link); owner.NotifyLinksChanged(); return link; }
			public Link Add(int start, int length) => Add(new Link(start, length));
			public Link Add(int start, int length, object linkData) => Add(new Link(start, length, linkData));
			public new void Clear() { base.Clear(); owner.NotifyLinksChanged(); }
			public new void Remove(Link link) { base.Remove(link); owner.NotifyLinksChanged(); }
		}

		internal void NotifyLinksChanged() => NotifyStylesChanged();
	}

	public struct LinkArea
	{
		public LinkArea(int start, int length) { Start = start; Length = length; }
		public int Start { get; set; }
		public int Length { get; set; }
	}

	public delegate void LinkLabelLinkClickedEventHandler(object sender, LinkLabelLinkClickedEventArgs e);
	public class LinkLabelLinkClickedEventArgs : EventArgs
	{
		public LinkLabelLinkClickedEventArgs(LinkLabel.Link link) : this(link, MouseButtons.Left) { }
		public LinkLabelLinkClickedEventArgs(LinkLabel.Link link, MouseButtons button) { Link = link; Button = button; }
		public LinkLabel.Link Link { get; }
		public MouseButtons Button { get; }
	}

	/// <summary>A radio button: checking one unchecks the others with the same parent.</summary>
	public class RadioButton : Control
	{
		private bool check;
		public bool AutoCheck { get; set; } = true;
		public bool AutoSize { get; set; }
		public Appearance Appearance { get; set; }
		public ContentAlignment CheckAlign { get; set; } = ContentAlignment.MiddleLeft;
		public ContentAlignment TextAlign { get; set; } = ContentAlignment.MiddleLeft;
		public ContentAlignment ImageAlign { get; set; } = ContentAlignment.MiddleCenter;
		public FlatStyle FlatStyle { get; set; } = FlatStyle.Standard;
		public Image Image { get; set; }
		public bool UseVisualStyleBackColor { get; set; }

		public bool Checked
		{
			get => check;
			set
			{
				if(check == value) return;
				check = value;
				if(value && Parent != null)
					foreach(Control c in Parent.Controls)
						if(c != this && c is RadioButton r && r.AutoCheck) r.Checked = false;
				CheckedChanged?.Invoke(this, EventArgs.Empty);
			}
		}

		public event EventHandler CheckedChanged;

		protected override void OnClick(EventArgs e)
		{
			if(AutoCheck) Checked = true;
			base.OnClick(e);
		}
	}

	/// <summary>Two panels with a splitter; views lay them out (SplitterDistance is the first panel's size).</summary>
	public class SplitContainer : ContainerControl, ISupportInitialize
	{
		private bool collapse1, collapse2;

		public SplitContainer()
		{
			Panel1 = new SplitterPanel(this);
			Panel2 = new SplitterPanel(this);
			Controls.Add(Panel1);
			Controls.Add(Panel2);
		}

		public SplitterPanel Panel1 { get; }
		public SplitterPanel Panel2 { get; }
		public Orientation Orientation { get; set; } = Orientation.Vertical;
		public int SplitterDistance { get; set; } = 50;
		public int SplitterWidth { get; set; } = 4;
		public int SplitterIncrement { get; set; } = 1;
		public int Panel1MinSize { get; set; } = 25;
		public int Panel2MinSize { get; set; } = 25;
		public FixedPanel FixedPanel { get; set; }
		public bool IsSplitterFixed { get; set; }
		public BorderStyle BorderStyle { get; set; }
		public bool Panel1Collapsed { get => collapse1; set { collapse1 = value; NotifyStylesChanged(); } }
		public bool Panel2Collapsed { get => collapse2; set { collapse2 = value; NotifyStylesChanged(); } }
		public Rectangle SplitterRectangle => Rectangle.Empty;
		public event SplitterEventHandler SplitterMoved;
		public void RaiseSplitterMoved(int distance) { SplitterDistance = distance; SplitterMoved?.Invoke(this, new SplitterEventArgs(0, 0, distance, distance)); }
		public void BeginInit() { }
		public void EndInit() { }
	}

	public sealed class SplitterPanel : Panel
	{
		internal SplitterPanel(SplitContainer owner) { Owner = owner; }
		public SplitContainer Owner { get; }
	}

	public delegate void SplitterEventHandler(object sender, SplitterEventArgs e);
	public class SplitterEventArgs : EventArgs
	{
		public SplitterEventArgs(int x, int y, int splitX, int splitY) { X = x; Y = y; SplitX = splitX; SplitY = splitY; }
		public int X { get; }
		public int Y { get; }
		public int SplitX { get; set; }
		public int SplitY { get; set; }
	}

	public class GroupBox : Control
	{
		public FlatStyle FlatStyle { get; set; } = FlatStyle.Standard;
		public bool AutoSize { get; set; }
	}

	public class CheckBox : Control
	{
		private CheckState checkstate;

		public bool AutoCheck { get; set; } = true;
		public bool AutoSize { get; set; }
		public bool ThreeState { get; set; }
		public Appearance Appearance { get; set; }
		public ContentAlignment CheckAlign { get; set; } = ContentAlignment.MiddleLeft;
		public ContentAlignment TextAlign { get; set; } = ContentAlignment.MiddleLeft;
		public FlatStyle FlatStyle { get; set; } = FlatStyle.Standard;
		public Image Image { get; set; }
		public bool UseVisualStyleBackColor { get; set; }

		public bool Checked { get => checkstate != CheckState.Unchecked; set => CheckState = value ? CheckState.Checked : CheckState.Unchecked; }

		public CheckState CheckState
		{
			get => checkstate;
			set
			{
				if(checkstate == value) return;
				bool waschecked = Checked;
				checkstate = value;
				if(waschecked != Checked) CheckedChanged?.Invoke(this, EventArgs.Empty);
				CheckStateChanged?.Invoke(this, EventArgs.Empty);
			}
		}

		public event EventHandler CheckedChanged;
		public event EventHandler CheckStateChanged;

		protected override void OnClick(EventArgs e)
		{
			if(AutoCheck) CheckState = ThreeState
				? (CheckState)(((int)checkstate + 1) % 3)
				: (Checked ? CheckState.Unchecked : CheckState.Checked);
			base.OnClick(e);
		}
	}

	public class NumericUpDown : Control, ISupportInitialize
	{
		private decimal value, minimum, maximum = 100m;

		// As in WinForms, moving a limit past the value moves the value with it.
		public decimal Minimum { get => minimum; set { minimum = value; if(maximum < minimum) maximum = minimum; Value = Math.Clamp(this.value, minimum, maximum); } }
		public decimal Maximum { get => maximum; set { maximum = value; if(minimum > maximum) minimum = maximum; Value = Math.Clamp(this.value, minimum, maximum); } }
		public decimal Increment { get; set; } = 1m;
		public int DecimalPlaces { get; set; }
		public bool Hexadecimal { get; set; }
		public bool ThousandsSeparator { get; set; }
		public bool ReadOnly { get; set; }
		public bool InterceptArrowKeys { get; set; } = true;
		public HorizontalAlignment TextAlign { get; set; }
		public LeftRightAlignment UpDownAlign { get; set; } = LeftRightAlignment.Right;
		public BorderStyle BorderStyle { get; set; } = BorderStyle.Fixed3D;

		public decimal Value
		{
			get => value;
			set
			{
				if(value < minimum || value > maximum) throw new ArgumentOutOfRangeException(nameof(value), "Value must be between Minimum and Maximum");
				if(this.value == value) return;
				this.value = value;
				ValueChanged?.Invoke(this, EventArgs.Empty);
			}
		}

		public event EventHandler ValueChanged;

		public virtual void UpButton() => Value = Math.Min(maximum, value + Increment);
		public virtual void DownButton() => Value = Math.Max(minimum, value - Increment);
		protected virtual void OnTextBoxResize(object source, EventArgs e) { }

		// WinForms' internal parts, which subclasses find by reflection (BuilderEffects' NumericUpDownEx).
		// State only: the view draws its own text box and buttons.
		private readonly TextBox upDownEdit = new TextBox();
		private readonly Control upDownButtons = new Control();
		public void Select(int start, int length) { }
		public void BeginInit() { }
		public void EndInit() { }
	}

	public class TabControl : Control
	{
		private int selectedindex = -1;

		public TabControl() { TabPages = new TabPageCollection(this); }

		public TabPageCollection TabPages { get; }
		public TabAlignment Alignment { get; set; }
		public TabAppearance Appearance { get; set; }
		public TabSizeMode SizeMode { get; set; }
		public Size ItemSize { get; set; }
		public Point Padding { get; set; }
		public bool Multiline { get; set; }
		public bool ShowToolTips { get; set; }
		public int TabCount => TabPages.Count;
		public ImageList ImageList { get; set; }

		/// <summary>ULDB: raised when pages are added or removed, so views show the right tabs.</summary>
		public event EventHandler TabPagesChanged;
		internal void OnTabPagesChanged() => TabPagesChanged?.Invoke(this, EventArgs.Empty);

		/// <summary>ULDB: tab strip thickness the views use (vertical tabs are this wide).</summary>
		public const int TabStripThickness = 26;

		/// <summary>Tab bounds as a view lays them out: vertical tabs (Left/Right) are
		/// <see cref="TabStripThickness"/> wide, horizontal ones that tall; tabs are 100 long.</summary>
		public Rectangle GetTabRect(int index)
		{
			bool vertical = Alignment == TabAlignment.Left || Alignment == TabAlignment.Right;
			return vertical ? new Rectangle(0, index * 100, TabStripThickness, 100) : new Rectangle(index * 100, 0, 100, TabStripThickness);
		}

		public int SelectedIndex
		{
			get => selectedindex;
			set
			{
				if(value == selectedindex || value < -1 || value >= TabPages.Count) return;
				TabControlCancelEventArgs selecting = new TabControlCancelEventArgs(value >= 0 ? TabPages[value] : null, value, false, TabControlAction.Selecting);
				Selecting?.Invoke(this, selecting);
				if(selecting.Cancel) return;
				selectedindex = value;
				SelectedIndexChanged?.Invoke(this, EventArgs.Empty);
				Selected?.Invoke(this, new TabControlEventArgs(SelectedTab, value, TabControlAction.Selected));
			}
		}

		public TabPage SelectedTab
		{
			get => selectedindex >= 0 && selectedindex < TabPages.Count ? TabPages[selectedindex] : null;
			set => SelectedIndex = TabPages.IndexOf(value);
		}

		public void SelectTab(int index) => SelectedIndex = index;
		public void SelectTab(TabPage tabPage) => SelectedTab = tabPage;
		public void SelectTab(string tabPageName) => SelectedIndex = TabPages.FindIndex(p => p.Name == tabPageName);

		public event EventHandler SelectedIndexChanged;
		public event TabControlCancelEventHandler Selecting;
		public event TabControlEventHandler Selected;
		public event TabControlCancelEventHandler Deselecting;
		public event TabControlEventHandler Deselected;

		public class TabPageCollection : List<TabPage>
		{
			private readonly TabControl owner;
			internal TabPageCollection(TabControl owner) { this.owner = owner; }

			public new void Add(TabPage page)
			{
				if(page.OwnerForm == null) page.OwnerForm = page.FindForm(); // plugins move their pages into Preferences; the view needs the form they came from
				page.Parent = owner;
				base.Add(page);
				if(owner.selectedindex < 0) owner.selectedindex = 0;
				owner.OnTabPagesChanged();
			}

			public void Add(string text) => Add(new TabPage(text));
			public void AddRange(TabPage[] pages) { foreach(TabPage p in pages) Add(p); }

			public new bool Remove(TabPage page)
			{
				bool removed = base.Remove(page);
				if(owner.selectedindex >= Count) owner.selectedindex = Count - 1;
				owner.OnTabPagesChanged();
				return removed;
			}

			public new void RemoveAt(int index) => Remove(this[index]);
			public new void Insert(int index, TabPage page) { page.Parent = owner; base.Insert(index, page); owner.OnTabPagesChanged(); }
			public new void Clear() { base.Clear(); owner.selectedindex = -1; owner.OnTabPagesChanged(); }

			/// <summary>ULDB: moves a page to another place (the user dragged its tab); the selected page stays selected.</summary>
			public void Move(int from, int to)
			{
				if(from < 0 || from >= Count || to < 0 || to >= Count || from == to) return;
				TabPage selected = owner.selectedindex >= 0 && owner.selectedindex < Count ? this[owner.selectedindex] : null;
				TabPage page = this[from];
				base.RemoveAt(from);
				base.Insert(to, page);
				if(selected != null) owner.selectedindex = IndexOf(selected);
				owner.OnTabPagesChanged();
			}
		}
	}

	public class TabPage : Panel
	{
		public TabPage() { }
		public TabPage(string text) { Text = text; }
		public string ToolTipText { get; set; }
		public bool UseVisualStyleBackColor { get; set; }
		public int ImageIndex { get; set; } = -1;
		/// <summary>ULDB: the form this page was first on (a page can be moved to another tab control).</summary>
		public Form OwnerForm { get; internal set; }
	}

	public class TabControlEventArgs : EventArgs
	{
		public TabControlEventArgs(TabPage tabPage, int tabPageIndex, TabControlAction action)
		{
			TabPage = tabPage;
			TabPageIndex = tabPageIndex;
			Action = action;
		}

		public TabPage TabPage { get; }
		public int TabPageIndex { get; }
		public TabControlAction Action { get; }
	}

	public class TabControlCancelEventArgs : CancelEventArgs
	{
		public TabControlCancelEventArgs(TabPage tabPage, int tabPageIndex, bool cancel, TabControlAction action) : base(cancel)
		{
			TabPage = tabPage;
			TabPageIndex = tabPageIndex;
			Action = action;
		}

		public TabPage TabPage { get; }
		public int TabPageIndex { get; }
		public TabControlAction Action { get; }
	}

	public delegate void TabControlEventHandler(object sender, TabControlEventArgs e);
	public delegate void TabControlCancelEventHandler(object sender, TabControlCancelEventArgs e);

	/// <summary>Tooltips for controls. Show/Hide pop up a tooltip right away; routed to the UI.</summary>
	public class ToolTip : IDisposable
	{
		private readonly Dictionary<Control, string> tips = new Dictionary<Control, string>();

		public ToolTip() { }
		public ToolTip(IContainer cont) { }

		public bool Active { get; set; } = true;
		public int AutomaticDelay { get; set; } = 500;
		public int AutoPopDelay { get; set; } = 5000;
		public int InitialDelay { get; set; } = 500;
		public int ReshowDelay { get; set; } = 100;
		public bool ShowAlways { get; set; }
		public bool IsBalloon { get; set; }
		public bool UseAnimation { get; set; } = true;
		public bool UseFading { get; set; } = true;
		public string ToolTipTitle { get; set; } = "";
		public ToolTipIcon ToolTipIcon { get; set; }
		public object Tag { get; set; }

		public void SetToolTip(Control control, string caption)
		{
			if(string.IsNullOrEmpty(caption)) tips.Remove(control);
			else tips[control] = caption;
			TipChanged?.Invoke(control, caption);
			if(control != null) control.UldbToolTip = string.IsNullOrEmpty(caption) ? null : caption;
		}

		public string GetToolTip(Control control) => tips.TryGetValue(control, out string t) ? t : "";
		public void RemoveAll() { foreach(Control c in new List<Control>(tips.Keys)) SetToolTip(c, null); }

		public void Show(string text, IWin32Window window) => ShowHandler?.Invoke(this, text, window as Control, null);
		public void Show(string text, IWin32Window window, int duration) => ShowHandler?.Invoke(this, text, window as Control, null);
		public void Show(string text, IWin32Window window, Point point) => ShowHandler?.Invoke(this, text, window as Control, point);
		public void Show(string text, IWin32Window window, int x, int y) => ShowHandler?.Invoke(this, text, window as Control, new Point(x, y));
		public void Show(string text, IWin32Window window, int x, int y, int duration) => ShowHandler?.Invoke(this, text, window as Control, new Point(x, y));
		public void Hide(IWin32Window win) => HideHandler?.Invoke(this, win as Control);

		/// <summary>ULDB: installed by the UI. Point is relative to the control (null: at the pointer).</summary>
		public static Action<ToolTip, string, Control, Point?> ShowHandler { get; set; }
		public static Action<ToolTip, Control> HideHandler { get; set; }

		/// <summary>ULDB: a control's hover tooltip changed (null or empty: removed).</summary>
		public static event Action<Control, string> TipChanged;

		public void Dispose() { }
	}
}

namespace System.Windows.Forms.Design
{
	/// <summary>Where the Visual Studio designer offers a ToolStripItem type. No effect at runtime.</summary>
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class ToolStripItemDesignerAvailabilityAttribute : Attribute
	{
		public ToolStripItemDesignerAvailabilityAttribute() { }
		public ToolStripItemDesignerAvailabilityAttribute(ToolStripItemDesignerAvailability visibility) { ItemAdditionVisibility = visibility; }
		public ToolStripItemDesignerAvailability ItemAdditionVisibility { get; }
	}
}

namespace System.Drawing
{
	/// <summary>Visual Studio toolbox icon for a control. No effect at runtime.</summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class ToolboxBitmapAttribute : Attribute
	{
		public ToolboxBitmapAttribute(string imageFile) { }
		public ToolboxBitmapAttribute(Type t) { }
		public ToolboxBitmapAttribute(Type t, string name) { }
	}
}
