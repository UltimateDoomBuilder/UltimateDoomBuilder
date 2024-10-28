
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

using CodeImp.DoomBuilder.Config;
using System;
using System.Globalization;

#endregion

namespace CodeImp.DoomBuilder.Types
{
	[TypeHandler(UniversalType.Float, "Decimal", true)]
	internal class FloatHandler : TypeHandler
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		private double value;
		private double defaultvalue;

		#endregion

		#region ================== Properties

		#endregion

		#region ================== Methods

		public override void SetupArgument(TypeHandlerAttribute attr, ArgumentInfo arginfo)
		{
			defaultvalue = (double)arginfo.DefaultValue;
			base.SetupArgument(attr, arginfo);
		}

		public override void SetupField(TypeHandlerAttribute attr, UniversalFieldInfo fieldinfo)
		{
			// The default value might be given as int instead as a floating point number, so try to convert it
			defaultvalue = (fieldinfo == null || fieldinfo.Default == null) ? 0.0 : Convert.ToDouble(fieldinfo.Default);
			base.SetupField(attr, fieldinfo);
		}

		public override void SetValue(object value)
		{
			// Null?
			if(value == null)
			{
				this.value = 0.0;
			}
			// Compatible type?
			else if((value is int) || (value is float) || (value is double) || (value is bool))
			{
				// Set directly
				this.value = Convert.ToDouble(value);
			}
			else
			{
				// Try parsing as string
				double result;
				if(double.TryParse(value.ToString(), NumberStyles.Float, CultureInfo.CurrentCulture, out result))
				{
					this.value = result;
				}
				else
				{
					this.value = 0.0;
				}
			}
		}

		public override object GetValue()
		{
			return this.value;
		}

		public override int GetIntValue()
		{
			return (int)this.value;
		}

		public override string GetStringValue()
		{
			return this.value.ToString();
		}

		public override object GetDefaultValue()
		{
			return defaultvalue;
		}

		#endregion
	}
}
