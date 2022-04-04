namespace Reginald.Services.Appearance
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using static Reginald.Services.Appearance.NativeMethods;

    public static class DarkTitleBar
    {
        private const int MinimumBuild = 17763;

        public static void Enable(IntPtr hWnd)
        {
            bool useDarkMode = true;
            GCHandle gch = GCHandle.Alloc(useDarkMode);
            IntPtr hBool = GCHandle.ToIntPtr(gch);
            WindowCompositionAttributeData data = new()
            {
                Attribute = WindowCompositionAttribute.WCA_USEDARKMODECOLORS,
                Data = hBool,
                SizeOfData = sizeof(int),
            };
            _ = SetWindowCompositionAttribute(hWnd, ref data);
            gch.Free();
        }

        public static void Enable(IntPtr hWnd, bool useDarkMode)
        {
            if (Environment.OSVersion.Version.Build > MinimumBuild)
            {
                int result = DwmGetWindowAttribute(hWnd, 20, out useDarkMode, sizeof(int));
                Debug.WriteLine($"result = {result}");
            }
        }
    }
}
