using System;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace CodeImp.DoomBuilder.Controls
{
	[ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.ToolStrip)]
	public class ToolStripNumericUpDown : ToolStripControlHost
	{
		public event EventHandler ValueChanged;
		
		public decimal Value { get { return nud.Value; } set { nud.Value = value; } }
		public decimal Minimum { get { return nud.Minimum; } set { nud.Minimum = value; } }
		public decimal Maximum { get { return nud.Maximum; } set { nud.Maximum = value; } }
		public decimal Increment { get { return nud.Increment; } set { nud.Increment = value; } }

		private NumericUpDown nud;

		public ToolStripNumericUpDown() : base(new NumericUpDown()) { }

        protected override void OnSubscribeControlEvents(Control control)
        {
            base.OnSubscribeControlEvents(control);
	        nud = control as NumericUpDown;
	        nud.ValueChanged += OnValueChanged;
        }

        protected override void OnUnsubscribeControlEvents(Control control)
        {
            base.OnUnsubscribeControlEvents(control);
			nud.ValueChanged -= OnValueChanged;
        }

        public void OnValueChanged(object sender, EventArgs e)
        {
            if(ValueChanged != null) ValueChanged(this, e);
        }

#if MONO_WINFORMS
		// Mono fix. Ensure the control minimum size is updated when the bounds change.
		// Otherwise the NumericUpDown appears very small.
		protected override void OnBoundsChanged()
		{
			base.OnBoundsChanged();

			if (Control != null)
				Control.MinimumSize = Size;
		}
#endif
	}
}
