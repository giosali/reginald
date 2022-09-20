namespace Reginald.Converters
{
    using System;
    using System.Runtime.InteropServices;
    using System.Text;

    internal static class NativeMethods
    {
        internal static readonly HandleRef NullHandleRef = new(null, IntPtr.Zero);

        [Flags]
        internal enum SHGSI : uint
        {
            /// <summary>
            /// Modifies the SHGSI_ICON value by causing the function to retrieve the large version of the icon, as specified by the SM_CXICON and SM_CYICON system metrics.
            /// </summary>
            SHGSI_LARGEICON = 0x000000000,

            /// <summary>
            /// Modifies the SHGSI_ICON value by causing the function to retrieve the small version of the icon, as specified by the SM_CXSMICON and SM_CYSMICON system metrics.
            /// </summary>
            SHGSI_SMALLICON = 0x000000001,

            /// <summary>
            /// The hIcon member of the SHSTOCKICONINFO structure receives a handle to the specified icon.
            /// </summary>
            SHGSI_ICON = 0x000000100,
        }

        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        internal static extern IntPtr ExtractAssociatedIcon(IntPtr hInst, StringBuilder lpIconPath, out ushort lpiIcon);

        [DllImport("shell32.dll", SetLastError = false)]
        internal static extern int SHGetStockIconInfo(uint siid, SHGSI uFlags, ref SHSTOCKICONINFO psii);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern bool DestroyIcon(IntPtr handle);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct SHSTOCKICONINFO
        {
            public uint cbSize;
            public IntPtr hIcon;
            public int iSysIconIndex;
            public int iIcon;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szPath;
        }
    }
}
