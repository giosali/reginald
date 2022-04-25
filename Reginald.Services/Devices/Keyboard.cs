namespace Reginald.Services.Devices
{
    using System;
    using static Reginald.Services.Devices.NativeMethods;

    public static class Keyboard
    {
        public static bool LoseFocus()
        {
            return SetForegroundWindow(GetDesktopWindow());
        }

        public static IntPtr SetFocus(IntPtr hWnd)
        {
            return SetActiveWindow(hWnd);
        }
    }
}
