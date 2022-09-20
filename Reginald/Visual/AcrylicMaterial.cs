namespace Reginald.Visual
{
    using System;
    using System.Runtime.InteropServices;
    using System.Windows.Media;
    using static Reginald.Visual.NativeMethods;

    internal static class AcrylicMaterial
    {
        public static void Enable(IntPtr hWnd, uint opacity, SolidColorBrush backgroundBrush)
        {
            // Sets an unsigned integer in BGR format as the background color
            // of the acrylic material.
            uint backgroundColorHex = ToBgr(backgroundBrush);

            AccentPolicy accent = new()
            {
                AccentState = AccentState.ACCENT_ENABLE_ACRYLICBLURBEHIND,
                GradientColor = (opacity << 24) | (backgroundColorHex & 0xFFFFFF),
            };
            int accentStructSize = Marshal.SizeOf(accent);
            IntPtr accentPtr = Marshal.AllocHGlobal(accentStructSize);
            Marshal.StructureToPtr(accent, accentPtr, false);
            WindowCompositionAttributeData data = new()
            {
                Attribute = WindowCompositionAttribute.WCA_ACCENT_POLICY,
                SizeOfData = accentStructSize,
                Data = accentPtr,
            };
            _ = SetWindowCompositionAttribute(hWnd, ref data);
            Marshal.FreeHGlobal(accentPtr);
        }

        private static uint ToBgr(SolidColorBrush brush)
        {
            return (uint)brush.Color.B << 16 | (uint)brush.Color.G << 8 | brush.Color.R;
        }
    }
}
