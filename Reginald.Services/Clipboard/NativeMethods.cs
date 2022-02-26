namespace Reginald.Services.Clipboard
{
    using System;
    using System.Runtime.InteropServices;

    public static class NativeMethods
    {
        internal const int WM_CLIPBOARDUPDATE = 0x031D;

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool AddClipboardFormatListener(IntPtr hwnd);
    }
}
