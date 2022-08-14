namespace Reginald.Services.Utilities
{
    using System;
    using System.Threading.Tasks;
    using static Reginald.Services.Utilities.NativeMethods;

    public static class WindowUtility
    {
        public static void Drag()
        {
            if (GetCursorPos(out POINT p))
            {
                IntPtr hWnd = WindowFromPoint(p);
                _ = ReleaseCapture();
                _ = SendMessage(hWnd, (int)WindowMessage.WM_NCLBUTTONDOWN, new IntPtr(HT_CAPTION), IntPtr.Zero);
            }
        }

        public static IntPtr RegisterInstance(string name)
        {
            return CreateMutex(IntPtr.Zero, true, name);
        }

        public static IntPtr SetFocus(IntPtr hWnd)
        {
            return SetActiveWindow(hWnd);
        }

        public static void UnregisterInstance(IntPtr hMutex)
        {
            _ = ReleaseMutex(hMutex);
        }

        public static async Task WaitForDeactivationAsync()
        {
            while (true)
            {
                if (!IsWindowVisible(GetActiveWindow()))
                {
                    break;
                }

                await Task.Delay(10);
            }
        }
    }
}
