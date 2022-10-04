namespace Reginald.Core.Services
{
    using System;
    using System.Threading.Tasks;
    using static Reginald.Core.Services.NativeMethods;

    public static class WindowService
    {
        private const int HT_CAPTION = 0x2;

        private const int WM_NCLBUTTONDOWN = 0x00A1;

        public static void Drag()
        {
            if (!GetCursorPos(out POINT p))
            {
                return;
            }

            IntPtr hWnd = WindowFromPoint(p);
            _ = ReleaseCapture();
            _ = SendMessage(hWnd, WM_NCLBUTTONDOWN, new IntPtr(HT_CAPTION), IntPtr.Zero);
        }

        public static void SetKeyboardFocus(IntPtr hWnd)
        {
            _ = SetForegroundWindow(hWnd);
        }

        public static async Task WaitForDeactivationAsync()
        {
            while (IsWindowVisible(GetActiveWindow()))
            {
                await Task.Delay(10);
            }
        }
    }
}
