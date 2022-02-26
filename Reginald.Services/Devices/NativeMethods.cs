namespace Reginald.Services.Devices
{
    using System;
    using System.Drawing;
    using System.Runtime.InteropServices;

    public static class NativeMethods
    {
        internal const int WM_NCLBUTTONDOWN = 0xA1;

        internal const int HT_CAPTION = 0x2;

        [DllImport("user32.dll")]
        internal static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll")]
        internal static extern bool GetCursorPos(out Point lpPoint);

        [DllImport("user32.dll")]
        internal static extern IntPtr WindowFromPoint(Point p);

        [DllImport("user32.dll")]
        internal static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        internal static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern IntPtr SetActiveWindow(IntPtr hWnd);
    }
}
