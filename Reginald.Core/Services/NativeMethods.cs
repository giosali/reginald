namespace Reginald.Core.Services
{
    using System;
    using System.Runtime.InteropServices;
    using System.Text;

    internal static class NativeMethods
    {
        internal delegate bool EnumDelegate(IntPtr hWnd, int lParam);

        /// <summary>
        /// Options used by the DwmGetWindowAttribute and DwmSetWindowAttribute functions.
        /// </summary>
        internal enum DWMWINDOWATTRIBUTE : uint
        {
            /// <summary>
            /// Use with DwmGetWindowAttribute. If the window is cloaked, provides one of the following values explaining why. DWM_CLOAKED_APP (value 0x0000001). The window was cloaked by its owner application. DWM_CLOAKED_SHELL(value 0x0000002). The window was cloaked by the Shell. DWM_CLOAKED_INHERITED(value 0x0000004). The cloak value was inherited from its owner window. Windows 7 and earlier: This value is not supported.
            /// </summary>
            DWMWA_CLOAKED,
        }

        [DllImport("dwmapi.dll")]
        internal static extern int DwmGetWindowAttribute(IntPtr hwnd, DWMWINDOWATTRIBUTE dwAttribute, out bool pvAttribute, int cbAttribute);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern IntPtr CreateMutex(IntPtr lpMutexAttributes, bool bInitialOwner, string lpName);

        [DllImport("kernel32.dll")]
        internal static extern bool ReleaseMutex(IntPtr hMutex);

        [DllImport("shell32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern IntPtr ShellExecute(IntPtr hwnd, string lpOperation, string lpFile, string lpParameters, string lpDirectory, int nShowCmd);

        [DllImport("user32.dll")]
        internal static extern bool EnumWindows(EnumDelegate lpEnumFunc, IntPtr lParam);

        [DllImport("user32.dll")]
        internal static extern IntPtr GetActiveWindow();

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll")]
        internal static extern uint GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

        [DllImport("user32.dll")]
        internal static extern bool GetCursorPos(out POINT lpPoint);

        [DllImport("user32.dll")]
        internal static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll")]
        internal static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        internal static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern IntPtr SetActiveWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        internal static extern IntPtr WindowFromPoint(POINT p);

        [StructLayout(LayoutKind.Sequential)]
        internal struct POINT
        {
            public int x;
            public int y;
        }
    }
}
