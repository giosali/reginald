namespace Reginald.Services.Devices
{
    using System;
    using System.Drawing;
    using static Reginald.Services.Devices.NativeMethods;

    public static class Mouse
    {
        public static bool TryGetWindowThreadProcessIdFromCursorPos(out uint threadProcessId)
        {
            if (GetCursorPos(out Point p))
            {
                IntPtr hWnd = WindowFromPoint(p);
                _ = GetWindowThreadProcessId(hWnd, out threadProcessId);
                return true;
            }

            threadProcessId = 0;
            return false;
        }

        /// <summary>
        /// Moves window when dragging.
        /// </summary>
        public static void Drag()
        {
            if (GetCursorPos(out Point p))
            {
                IntPtr hWnd = WindowFromPoint(p);
                _ = ReleaseCapture();
                _ = SendMessage(hWnd, (int)WindowMessage.WM_NCLBUTTONDOWN, new IntPtr(HT_CAPTION), IntPtr.Zero);
            }
        }
    }
}
