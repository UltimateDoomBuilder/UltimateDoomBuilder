
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
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;

#endregion

namespace CodeImp.DoomBuilder.Controls
{
#if !NO_FORMS_DESIGN
	[Designer(typeof(ButtonsNumericTextboxDesigner))]
#endif
	public partial class ButtonsNumericTextbox : UserControl
	{
		#region ================== Events

		public event EventHandler WhenTextChanged;
		public event EventHandler WhenButtonsClicked;
		public event EventHandler WhenEnterPressed;

		#endregion

		#region ================== Variables
		
		private bool ignorebuttonchange;
		private StepsList steps;
		private int stepsize = 1;
		private float stepsizeFloat = 1.0f; //mxd
		private float stepsizeBig = 10.0f; //mxd
		private float stepsizeSmall = 0.1f; //mxd
		private bool wrapsteps; //mxd
		private bool usemodifierkeys; //mxd
		
		#endregion

		#region ================== Properties

		public bool AllowDecimal { get { return textbox.AllowDecimal; } set { textbox.AllowDecimal = value; UpdateButtonsTooltip(); } }
		public bool AllowNegative { get { return textbox.AllowNegative; } set { textbox.AllowNegative = value; } }
		public bool AllowRelative { get { return textbox.AllowRelative; } set { textbox.AllowRelative = value; } }
		public bool AllowExpressions { get { return textbox.AllowExpressions; } set { textbox.AllowExpressions = value; } } //mxd/mgr_inz_rafal
		public int ButtonStep { get { return stepsize; } set { stepsize = value; } }
		public float ButtonStepFloat { get { return stepsizeFloat; } set { stepsizeFloat = value; } } //mxd. This is used when AllowDecimal is true
		public float ButtonStepBig { get { return stepsizeBig; } set { stepsizeBig = value; } } //mxd
		public float ButtonStepSmall { get { return stepsizeSmall; } set { stepsizeSmall = value; } } //mxd
		override public string Text { get { return textbox.Text; } set { textbox.Text = value; } }
		internal NumericTextbox Textbox { get { return textbox; } }
		public StepsList StepValues { get { return steps; } set { steps = value; UpdateButtonsTooltip(); } }
		public bool ButtonStepsWrapAround { get { return wrapsteps; } set { wrapsteps = value; } }
		public bool ButtonStepsUseModifierKeys { get { return usemodifierkeys; } set { usemodifierkeys = value; UpdateButtonsTooltip(); } }

		#endregion
		
		#region ================== Constructor / Disposer

		// Constructor
		public ButtonsNumericTextbox()
		{
			InitializeComponent();
			buttons.Value = 0;
			textbox.MouseWheel += textbox_MouseWheel;
			UpdateButtonsTooltip(); //mxd
		}

		#endregion

		#region ================== Interface

		// Client size changes
		protected override void OnClientSizeChanged(EventArgs e)
		{
			base.OnClientSizeChanged(e);
			ClickableNumericTextbox_Resize(this, e);
		}
		
		// Layout changes
		private void ClickableNumericTextbox_Layout(object sender, LayoutEventArgs e)
		{
			ClickableNumericTextbox_Resize(sender, e);
		}

		// Control resizes
		private void ClickableNumericTextbox_Resize(object sender, EventArgs e)
		{
			buttons.Height = textbox.Height + 4;
			textbox.Width = ClientRectangle.Width - buttons.Width - 2;
			buttons.Left = textbox.Width + 2;
			this.Height = buttons.Height;
		}
		
		// Text in textbox changes
		private void textbox_TextChanged(object sender, EventArgs e)
		{
			if(WhenTextChanged != null) WhenTextChanged(sender, e);
			buttons.Enabled = !textbox.CheckIsRelative();
		}
		
		// Buttons changed
		private void buttons_ValueChanged(object sender, EventArgs e)
		{
			if(!ignorebuttonchange)
			{
				ignorebuttonchange = true;
				if(!textbox.CheckIsRelative())
				{
					bool ctrl = ((ModifierKeys & Keys.Control) == Keys.Control); //mxd
					bool shift = ((ModifierKeys & Keys.Shift) == Keys.Shift); //mxd

					if(steps != null && (!usemodifierkeys || (!ctrl && !shift)))
					{
						if(buttons.Value < 0)
							textbox.Text = steps.GetNextHigherWrap(textbox.GetResult(0), wrapsteps).ToString(); //mxd
						else if(buttons.Value > 0)
							textbox.Text = steps.GetNextLowerWrap(textbox.GetResult(0), wrapsteps).ToString(); //mxd
					}
					else if(textbox.AllowDecimal)
					{
						double stepsizemod; //mxd
						if(usemodifierkeys)
							stepsizemod = (ctrl ? stepsizeSmall : (shift ? stepsizeBig : stepsizeFloat));
						else
							stepsizemod = stepsizeFloat;
						
						double newvalue = Math.Round(textbox.GetResultFloat(0.0f) - (buttons.Value * stepsizemod), General.Map.FormatInterface.VertexDecimals);
						if((newvalue < 0.0f) && !textbox.AllowNegative) newvalue = 0.0f;
						textbox.Text = newvalue.ToString();
					}
					else
					{
						int stepsizemod; //mxd
						if(usemodifierkeys) 
							stepsizemod = (ctrl ? (int)stepsizeSmall : (shift ? (int)stepsizeBig : stepsize));
						else
							stepsizemod = stepsize;
						
						int newvalue = textbox.GetResult(0) - (buttons.Value * stepsizemod);
						if((newvalue < 0) && !textbox.AllowNegative) newvalue = 0;
						textbox.Text = newvalue.ToString();
					}
				}
				
				buttons.Value = 0;
				
				if(WhenButtonsClicked != null)
					WhenButtonsClicked(this, EventArgs.Empty);
				
				ignorebuttonchange = false;
			}
		}

		// Mouse wheel used
		private void textbox_MouseWheel(object sender, MouseEventArgs e)
		{
			if(steps != null && (!usemodifierkeys || ((ModifierKeys & Keys.Control) != Keys.Control && (ModifierKeys & Keys.Shift) != Keys.Shift)))
			{
				if(e.Delta > 0)
					textbox.Text = steps.GetNextHigher(textbox.GetResult(0)).ToString();
				else if(e.Delta < 0)
					textbox.Text = steps.GetNextLower(textbox.GetResult(0)).ToString();
			}
			else
			{
				buttons.Value -= Math.Sign(e.Delta);
			}
		}

		// Key pressed in textbox
		private void textbox_KeyDown(object sender, KeyEventArgs e)
		{
			// Enter key?
			if((e.KeyData == Keys.Enter) && (WhenEnterPressed != null))
				WhenEnterPressed(this, EventArgs.Empty);
		}
		
		#endregion
		
		#region ================== Methods
		
		// This checks if the number is relative
		public bool CheckIsRelative()
		{
			return textbox.CheckIsRelative();
		}
		
		// This determines the result value
		public int GetResult(int original)
		{
			return textbox.GetResult(original);
		}

		//mxd. This determines the result value at given inremental step
		public int GetResult(int original, int step)
		{
			return textbox.GetResult(original, step);
		}
		
		// This determines the result value
		public double GetResultFloat(double original)
		{
			return textbox.GetResultFloat(original);
		}

		//mxd. This determines the result value at given inremental step
		public double GetResultFloat(double original, int step)
		{
			return textbox.GetResultFloat(original, step);
		}

		//mxd
		public void UpdateButtonsTooltip()
		{
			if(usemodifierkeys)
			{
				string tip = "Hold Ctrl to change value by " + stepsizeSmall.ToString(CultureInfo.CurrentCulture) + "." + Environment.NewLine +
							 "Hold Shift to change value by " + stepsizeBig.ToString(CultureInfo.CurrentCulture) + ".";
				tooltip.SetToolTip(buttons, tip);
				textbox.UpdateTextboxStyle(tip);
			}
			else
			{
				tooltip.RemoveAll();
				textbox.UpdateTextboxStyle();
			}
		}

		// biwa
		public void ResetIncrementStep()
		{
			textbox.ResetIncrementStep();
		}

		public void SelectAll()
		{
			textbox.SelectAll();
		}

		#endregion
	}
}
