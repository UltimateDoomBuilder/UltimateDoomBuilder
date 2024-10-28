
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
using System.Globalization;
using CodeImp.DoomBuilder.Config;

#endregion

namespace CodeImp.DoomBuilder.Types
{
	[TypeHandler(UniversalType.Integer, "Integer", true)]
	internal class IntegerHandler : TypeHandler
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		private int value;
		private int defaultvalue; //mxd
		
		#endregion

		#region ================== Properties

		#endregion

		#region ================== Methods

		//mxd
		public override void SetupArgument(TypeHandlerAttribute attr, ArgumentInfo arginfo) 
		{
			defaultvalue = (int)arginfo.DefaultValue;
			base.SetupArgument(attr, arginfo);
		}

		public override void SetupField(TypeHandlerAttribute attr, UniversalFieldInfo fieldinfo)
		{
			defaultvalue = (fieldinfo == null || fieldinfo.Default == null) ? 0 : (int)fieldinfo.Default;
			base.SetupField(attr, fieldinfo);
		}

		public override void SetValue(object value)
		{
			// Null?
			if(value == null)
			{
				this.value = 0;
			}
			// Compatible type?
			else if((value is int) || (value is float) || (value is bool))
			{
				// Set directly
				this.value = Convert.ToInt32(value);
			}
			else
			{
				// Try parsing as string
				int result;
				if(int.TryParse(value.ToString(), NumberStyles.Integer, CultureInfo.CurrentCulture, out result))
				{
					this.value = result;
				}
				else
				{
					this.value = 0;
				}
			}

			if(forargument)
			{
				this.value = General.Clamp(this.value, General.Map.FormatInterface.MinArgument, General.Map.FormatInterface.MaxArgument);
			}
		}

		//mxd
		public override void ApplyDefaultValue() 
		{
			value = defaultvalue;
		}

		public override object GetValue()
		{
			return this.value;
		}
		
		public override int GetIntValue()
		{
			return this.value;
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
