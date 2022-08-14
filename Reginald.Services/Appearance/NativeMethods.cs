namespace Reginald.Services.Appearance
{
    using System;
    using System.Runtime.InteropServices;

    internal static class NativeMethods
    {
        internal enum AccentState
        {
            ACCENT_ENABLE_ACRYLICBLURBEHIND = 4,
        }

        internal enum WindowCompositionAttribute
        {
            WCA_ACCENT_POLICY = 19,
            WCA_USEDARKMODECOLORS = 26,
        }

        [DllImport("dwmapi.dll")]
        internal static extern int DwmGetWindowAttribute(IntPtr hwnd, uint dwAttribute, out bool pvAttribute, int cbAttribute);

        [DllImport("user32.dll")]
        internal static extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WindowCompositionAttributeData data);

        [StructLayout(LayoutKind.Sequential)]
        internal struct AccentPolicy
        {
            public AccentState AccentState;
            public uint AccentFlags;
            public uint GradientColor;
            public uint AnimationId;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct WindowCompositionAttributeData
        {
            public WindowCompositionAttribute Attribute;
            public IntPtr Data;
            public int SizeOfData;
        }
    }
}
