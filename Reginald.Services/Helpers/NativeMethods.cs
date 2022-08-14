namespace Reginald.Services.Helpers
{
    using System;
    using System.Runtime.InteropServices;
    using System.Text;

    internal class NativeMethods
    {
        internal static readonly HandleRef NullHandleRef = new(null, IntPtr.Zero);

        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        internal static extern IntPtr ExtractAssociatedIcon(IntPtr hInst, StringBuilder lpIconPath, out ushort lpiIcon);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern bool DestroyIcon(IntPtr handle);
    }
}
