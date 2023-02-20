
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
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

#endregion

namespace CodeImp.DoomBuilder.Controls
{
	public class OptimizedListView : ListView
	{
        #region ================== API Declarations

#if NO_WIN32

        private static int SendMessage(IntPtr window, int message, int wParam, ref LVGROUP lParam)
        {
            return 0;
        }

#else

        [DllImport("user32.dll")]
		private static extern int SendMessage(IntPtr window, int message, int wParam, ref LVGROUP lParam);

#endif

        #endregion

        #region ================== Structs

        [StructLayout(LayoutKind.Sequential)]
		private struct LVGROUP
		{
			public int cbSize;
			public int mask;
			[MarshalAs(UnmanagedType.LPTStr)]
			public string pszHeader;
			public int cchHeader;
			[MarshalAs(UnmanagedType.LPTStr)]
			public string pszFooter;
			public int cchFooter;
			public int iGroupId;
			public int stateMask;
			public int state;
			public int uAlign;
		}

#endregion

#region ================== Enums

		[Flags]
		private enum GroupState
		{
			COLLAPSIBLE = 8,
			COLLAPSED = 1,
			EXPANDED = 0
		}

#endregion

#region ================== Delegates

		private delegate bool CallBackSetGroupCollapsible(ListViewGroup group, bool collapsed);
		private delegate bool CallBackGetGroupCollapsed(ListViewGroup group);

#endregion

#region ================== Constructor

		// Constructor
		public OptimizedListView()
		{
			this.DoubleBuffered = true;
			SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
		}

#endregion

#region ================== Methods

		//mxd. Collapsible groups support. Created using http://www.codeproject.com/Articles/31276/Add-Group-Collapse-Behavior-on-a-Listview-Control as a reference
		public bool SetGroupCollapsed(ListViewGroup group, bool collapsed) 
		{
			// Insanity checks...
			if(!this.Groups.Contains(group)) return false;
			if(Environment.OSVersion.Version.Major < 6) return false; //Only Vista and forward allows collapse of ListViewGroups
			if(this.InvokeRequired) return (bool)this.Invoke(new CallBackSetGroupCollapsible(SetGroupCollapsed), group, collapsed);

			LVGROUP groupstruct = new LVGROUP();
			groupstruct.cbSize = Marshal.SizeOf(typeof(LVGROUP));
			groupstruct.state = (int)((collapsed ? GroupState.COLLAPSED : GroupState.EXPANDED) | GroupState.COLLAPSIBLE);
			groupstruct.stateMask = (int)(GroupState.COLLAPSIBLE | GroupState.COLLAPSED);
			groupstruct.mask = 4; // LVGF_STATE 
			SendMessage(this.Handle, 0x1000 + 147, GetGroupID(group), ref groupstruct); // #define LVM_SETGROUPINFO (LVM_FIRST + 147)

			return true;
		}

		//mxd. Collapsible groups support. Created using http://fed.googlecode.com/svn/trunk/Plain.Forms/ListViewGroupEx.cs as a reference
		public bool IsGroupCollapsed(ListViewGroup group) 
		{
			// Insanity checks...
			if(!this.Groups.Contains(group)) return false;
			if(Environment.OSVersion.Version.Major < 6) return false; //Only Vista and forward allows collapse of ListViewGroups
			if(this.InvokeRequired) return (bool)this.Invoke(new CallBackGetGroupCollapsed(IsGroupCollapsed), group);

			LVGROUP groupstruct = new LVGROUP();
			groupstruct.cbSize = Marshal.SizeOf(typeof(LVGROUP));
			groupstruct.stateMask = (int)(GroupState.COLLAPSIBLE | GroupState.COLLAPSED);
			groupstruct.mask = 4; // LVGF_STATE

			SendMessage(this.Handle, 0x1000 + 149, GetGroupID(group), ref groupstruct); // #define LVM_GETGROUPINFO (LVM_FIRST + 149)
			return (groupstruct.state & (int)GroupState.COLLAPSED) != 0;
		}

		//mxd.
		private static int GetGroupID(ListViewGroup group) 
		{
			int id = int.MinValue;
			Type grouptype = group.GetType();

			PropertyInfo pi = grouptype.GetProperty("ID", BindingFlags.NonPublic | BindingFlags.Instance);
			if(pi != null) 
			{
				object idprop = pi.GetValue(group, null);
				if(idprop != null) id = (int)idprop;
			}

			return id;
		}

		//mxd. Required to make "Expand/Collapse" group header button work.
		protected override void WndProc(ref Message m) 
		{
			switch(m.Msg) 
			{
				case 0x202: // WM_LBUTTONUP
					base.DefWndProc(ref m);
					base.WndProc(ref m);
					break;

				default:
					base.WndProc(ref m);
					break;
			}
		}

#endregion
	}
}
