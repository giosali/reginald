namespace Reginald.Visual
{
    using System;
    using System.Runtime.InteropServices;
    using static Reginald.Visual.NativeMethods;

    internal static class DarkTitleBar
    {
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
    }
}
