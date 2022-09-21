namespace Reginald.Core.IO.Hooks
{
    using System.Text;
    using static Reginald.Core.IO.Hooks.NativeMethods;

    public static class VirtualKeyCodeConverter
    {
        public static char ConvertToChar(int vkCode)
        {
            // GetKeyState is necessary for detecting key combinations (Shift + 4)
            // or whether the CAPS LOCK key is toggled
            _ = GetKeyState(vkCode);
            byte[] keyboardState = new byte[256];
            _ = GetKeyboardState(keyboardState);

            uint scanCode = MapVirtualKey((uint)vkCode, MapVirtualKeyMapType.MAPVK_VK_TO_VSC);
            StringBuilder sb = new(2);
            int result = ToUnicode((uint)vkCode, scanCode, keyboardState, sb, sb.Capacity, 0);
            return result == 1 && sb.Length > 0 ? sb[0] : '\0';
        }
    }
}
