#region ================== Namespaces

using System;
using System.Globalization;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Properties;

#endregion

namespace CodeImp.DoomBuilder.Controls
{
	public partial class PairedFieldsControl : UserControl
	{
		#region ================== Events

		public event EventHandler OnValuesChanged;

		#endregion

		#region ================== Variables

		private float defaultValue;
		private string field1;
		private string field2;
		private bool allowValueLinking;
		private bool linkValues;
		private bool blockUpdate;
		private readonly int bResetOffsetX;
		private bool changed;

		#endregion

		#region ================== Properties

		public bool NonDefaultValue { get { return changed; } }
		public float DefaultValue { get { return defaultValue; } set { defaultValue = value; } }
		public string Field1 { get { return field1; } set { field1 = value; } }
		public string Field2 { get { return field2; } set { field2 = value; } }
		public bool AllowDecimal { get { return value1.AllowDecimal; } set { value1.AllowDecimal = value; value2.AllowDecimal = value; } }
		public int ButtonStep { get { return value1.ButtonStep; } set { value1.ButtonStep = value; value2.ButtonStep = value; } }
		public float ButtonStepFloat { get { return value1.ButtonStepFloat; } set { value1.ButtonStepFloat = value; value2.ButtonStepFloat = value; } }
		public float ButtonStepBig { get { return value1.ButtonStepBig; } set { value1.ButtonStepBig = value; value2.ButtonStepBig = value; } }
		public float ButtonStepSmall { get { return value1.ButtonStepSmall; } set { value1.ButtonStepSmall = value; value2.ButtonStepSmall = value; } }
		public bool ButtonStepsUseModifierKeys { get { return value1.ButtonStepsUseModifierKeys; } set { value1.ButtonStepsUseModifierKeys = value; value2.ButtonStepsUseModifierKeys = value; } }
		public bool AllowValueLinking { get { return allowValueLinking; } set { allowValueLinking = value; UpdateButtons(); } }
		public bool LinkValues { get { return linkValues; } set { linkValues = value; UpdateButtons(); } }

		#endregion

		#region ================== Constructor

		public PairedFieldsControl() 
		{
			InitializeComponent();
			bResetOffsetX = this.Width - bReset.Left;
		}

		#endregion

		#region ================== Methods

		public void SetValuesFrom(UniFields fields, bool first) 
		{
			blockUpdate = true;
			
			string newValue1;
			string newValue2;

			if(AllowDecimal) 
			{
				newValue1 = UniFields.GetFloat(fields, field1, defaultValue).ToString();
				newValue2 = UniFields.GetFloat(fields, field2, defaultValue).ToString();
			} 
			else 
			{
				newValue1 = Math.Round(UniFields.GetFloat(fields, field1, defaultValue)).ToString();
				newValue2 = Math.Round(UniFields.GetFloat(fields, field2, defaultValue)).ToString();
			}

			if(first) 
			{
				value1.Text = newValue1;
				value2.Text = newValue2;
			} 
			else
			{
				if(!string.IsNullOrEmpty(value1.Text)) value1.Text = (value1.Text != newValue1 ? string.Empty : newValue1);
				if(!string.IsNullOrEmpty(value2.Text)) value2.Text = (value2.Text != newValue2 ? string.Empty : newValue2);
			}
			CheckValues();

			blockUpdate = false;
		}

		public void ApplyTo(UniFields fields, int min, int max, double oldValue1, double oldValue2) 
		{
			if(!string.IsNullOrEmpty(value1.Text))
				UniFields.SetFloat(fields, field1, General.Clamp(value1.GetResultFloat(oldValue1), min, max), defaultValue);
			else
				UniFields.SetFloat(fields, field1, oldValue1, defaultValue);

			if(!string.IsNullOrEmpty(value2.Text))
				UniFields.SetFloat(fields, field2, General.Clamp(value2.GetResultFloat(oldValue2), min, max), defaultValue);
			else
				UniFields.SetFloat(fields, field2, oldValue2, defaultValue);
		}

		private void CheckValues() 
		{
			changed = string.IsNullOrEmpty(value1.Text) || string.IsNullOrEmpty(value2.Text)
				|| value1.GetResultFloat(defaultValue, 0) != defaultValue || value2.GetResultFloat(defaultValue, 0) != defaultValue;
			bReset.Visible = changed;

			if(!blockUpdate && OnValuesChanged != null)	OnValuesChanged(this, EventArgs.Empty);
		}

		private void UpdateButtons() 
		{
			bLink.Visible = allowValueLinking;
			
			if(!allowValueLinking) 
			{
				bReset.Left = bLink.Left;
			} 
			else 
			{
				bReset.Left = this.Width - bResetOffsetX;
				bLink.Image = (linkValues ? Resources.Link : Resources.Unlink);
			}
		}

		public void ResetIncrementStep()
		{
			value1.ResetIncrementStep();
			value2.ResetIncrementStep();
		}

		#endregion

		#region ================== Events

		private void bLink_Click(object sender, EventArgs e) 
		{
			linkValues = !linkValues;
			bLink.Image = (linkValues ? Resources.Link : Resources.Unlink);
		}

		private void bReset_Click(object sender, EventArgs e) 
		{
			value1.Text = defaultValue.ToString(CultureInfo.CurrentCulture);
			value2.Text = defaultValue.ToString(CultureInfo.CurrentCulture);
			CheckValues();
		}

		private void value1_WhenTextChanged(object sender, EventArgs e) 
		{
			if(blockUpdate) return;
			
			if(linkValues) 
			{
				blockUpdate = true;
				value2.Text = value1.Text;
				blockUpdate = false;
			}
			
			CheckValues();
		}

		private void value2_WhenTextChanged(object sender, EventArgs e) 
		{
			if(blockUpdate)	return;

			if(linkValues) 
			{
				blockUpdate = true;
				value1.Text = value2.Text;
				blockUpdate = false;
			}

			CheckValues();
		}

		#endregion
	}
}
