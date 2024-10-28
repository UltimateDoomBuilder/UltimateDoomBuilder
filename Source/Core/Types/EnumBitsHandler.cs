
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
using System.Drawing;
using System.Globalization;
using CodeImp.DoomBuilder.Config;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Windows;

#endregion

namespace CodeImp.DoomBuilder.Types
{
	[TypeHandler(UniversalType.EnumBits, "Options", false)]
	internal class EnumBitsHandler : TypeHandler
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		protected EnumList list;
		protected int value;
		protected int defaultvalue; //mxd

		#endregion

		#region ================== Properties

		public override bool IsBrowseable { get { return true; } }

		public override Image BrowseImage { get { return Properties.Resources.List; } }
		
		#endregion

		#region ================== Constructor

		// When set up for an argument
		public override void SetupArgument(TypeHandlerAttribute attr, ArgumentInfo arginfo)
		{
			defaultvalue = (int)arginfo.DefaultValue;//mxd
			base.SetupArgument(attr, arginfo);

			// Keep enum list reference
			list = arginfo.Enum;
		}

		// When set up for an UDMF field
		public override void SetupField(TypeHandlerAttribute attr, UniversalFieldInfo fieldinfo)
		{
			defaultvalue = (int)fieldinfo.Default;
			base.SetupField(attr, fieldinfo);

			// Keep enum list reference
			list = fieldinfo.Enum;
		}
		#endregion

		#region ================== Methods

		public override void Browse(IWin32Window parent)
		{
			value = BitFlagsForm.ShowDialog(parent, list, value);
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
