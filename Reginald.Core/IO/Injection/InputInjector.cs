namespace Reginald.Core.IO.Injection
{
    using System;
    using static Reginald.Core.IO.Injection.NativeMethods;

    public static class InputInjector
    {
        public static void InjectKeyboardInput(InjectedInputKeyboardInfo info)
        {
            _ = SendInput((uint)info.Inputs.Length, info.Inputs, INPUT.Size);
        }

        public static bool SendKeyDown(IntPtr hWnd, int vkCode)
        {
            // 0x0100 = WM_KEYDOWN.
            return PostMessage(hWnd, 0x0100, vkCode, 0);
        }

        public static bool SendKeyUp(IntPtr hWnd, int vkCode)
        {
            // 0x0101 = WM_KEYUP.
            return PostMessage(hWnd, 0x0101, vkCode, 0);
        }
    }
}
