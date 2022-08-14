namespace Reginald.Services.Appearance
{
    using System;
    using System.Runtime.InteropServices;
    using System.Windows.Media;
    using static Reginald.Services.Appearance.NativeMethods;

    public class AcrylicMaterial
    {
        public AcrylicMaterial(IntPtr hWnd, byte opacity, Brush backgroundBrush)
        {
            Handle = hWnd;
            Opacity = opacity;
            BackgroundColorHex = ToBgr(backgroundBrush as SolidColorBrush);
        }

        private IntPtr Handle { get; set; }

        /// <summary>
        /// Gets or sets an unsigned integer in BGR format as the background color of the acrylic material.
        /// </summary>
        private uint BackgroundColorHex { get; set; }

        private uint Opacity { get; set; }

        public void Enable()
        {
            AccentPolicy accent = new()
            {
                AccentState = AccentState.ACCENT_ENABLE_ACRYLICBLURBEHIND,
                GradientColor = (Opacity << 24) | (BackgroundColorHex & 0xFFFFFF),
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
            _ = SetWindowCompositionAttribute(Handle, ref data);
            Marshal.FreeHGlobal(accentPtr);
        }

        private static uint ToBgr(SolidColorBrush brush)
        {
            return (uint)brush.Color.B << 16 | (uint)brush.Color.G << 8 | brush.Color.R;
        }
    }
}
