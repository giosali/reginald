namespace Reginald.Services.Devices
{
    using System;
    using static Reginald.Services.Devices.NativeMethods;

    public static class Keyboard
    {
        public static IntPtr SetFocus(IntPtr hWnd)
        {
            return SetActiveWindow(hWnd);
        }
    }
}
