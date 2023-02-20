using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Windows;

namespace CodeImp.DoomBuilder
{
    public static class SysCall
    {

#if NO_WIN32

        internal static void InvokeUIActions(MainForm mainform)
        {
            // This implementation really should work universally, but it seemed to hang sometimes on Windows.
            // Let's hope the mono implementation of Winforms works better.
            mainform.Invoke(new System.Action(() => { mainform.ProcessQueuedUIActions(); }));
        }

        internal static bool MessageBeep(MessageBeepType type)
        {
            System.Media.SystemSounds.Beep.Play();
            return true;
        }

        internal static bool LockWindowUpdate(IntPtr hwnd)
        {
            // This can be safely ignored. It is a performance/flicker optimization. It might not even be needed on Windows anymore.
            return true;
        }

        internal unsafe static void ZeroPixels(PixelColor* pixels, int size)
        {
            var transparent = new PixelColor(0, 0, 0, 0);
            for (int i = 0; i < size; i++)
                pixels[i] = transparent;
        }

        internal static int SendMessage(IntPtr hWnd, Int32 wMsg, bool wParam, Int32 lParam)
        {
            return 0;
        }

        internal static IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp)
        {
            return IntPtr.Zero;
        }

        internal static int SendMessage(IntPtr hWnd, int msg, int wParam, string lParam)
        {
            return 0;
        }

        internal static void SetComboBoxItemHeight(ComboBox combobox, int height)
        {
            // Only used by FieldsEditorControl. Not sure what its purpose is, might only be visual adjustment that isn't strictly needed?
        }

        public static int GetWindowLong(IntPtr hWnd, int nIndex)
        {
            return 0;
        }

#else

        [DllImport("user32.dll")]
        internal static extern bool LockWindowUpdate(IntPtr hwnd);

        [DllImport("kernel32.dll", EntryPoint = "RtlZeroMemory", SetLastError = false)]
        static extern void ZeroMemory(IntPtr dest, int size);

        internal unsafe static void ZeroPixels(PixelColor* pixels, int size) { ZeroMemory(new IntPtr(pixels), size * sizeof(PixelColor)); }

        [DllImport("user32.dll", EntryPoint = "SendMessage", SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        internal static extern int SendMessage(IntPtr hWnd, Int32 wMsg, bool wParam, Int32 lParam);

        [DllImport("user32.dll")]
        internal static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern int SendMessage(IntPtr hWnd, int msg, int wParam, string lParam);

        internal static void SetComboBoxItemHeight(ComboBox combobox, int height)
        {
            SendMessage(combobox.Handle, General.CB_SETITEMHEIGHT, new IntPtr(-1), new IntPtr(height));
        }

        [DllImport("user32.dll", EntryPoint = "PostMessage", SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        static extern int PostMessage(IntPtr hwnd, uint Msg, IntPtr wParam, IntPtr lParam);

        internal static void InvokeUIActions(MainForm mainform)
        {
            PostMessage(mainform.Handle, General.WM_UIACTION, IntPtr.Zero, IntPtr.Zero);
        }

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool MessageBeep(MessageBeepType type);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern uint GetShortPathName([MarshalAs(UnmanagedType.LPTStr)] string longpath, [MarshalAs(UnmanagedType.LPTStr)]StringBuilder shortpath, uint buffersize);

        [DllImport("user32.dll")]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

#endif

    }
}
