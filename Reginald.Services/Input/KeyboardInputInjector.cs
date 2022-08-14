namespace Reginald.Services.Input
{
    using System;
    using System.Collections.Generic;
    using static Reginald.Services.Input.NativeMethods;

    /// <summary>
    /// Specifies various aspects of a keystroke.
    /// </summary>
    [Flags]
    public enum KEYEVENTF : uint
    {
        /// <summary>
        /// If specified, the key is being pressed.
        /// </summary>
        KEYDOWN = 0x0000,

        /// <summary>
        /// If specified, the key is being released. If not specified, the key is being pressed.
        /// </summary>
        KEYUP = 0x0002,

        /// <summary>
        /// If specified, the system synthesizes a VK_PACKET keystroke. The wVk parameter must be zero. This flag can only be combined with the KEYEVENTF_KEYUP flag. For more information, see the Remarks section.
        /// </summary>
        UNICODE = 0x0004,
    }

    /// <summary>
    /// Specifies the values for each virtual key.
    /// </summary>
    public enum VirtualKeyShort : short
    {
        /// <summary>
        /// BACKSPACE key.
        /// </summary>
        BACK = 0x08,

        /// <summary>
        /// TAB key.
        /// </summary>
        TAB = 0x09,

        /// <summary>
        /// ENTER key.
        /// </summary>
        RETURN = 0x0D,

        /// <summary>
        /// SHIFT key.
        /// </summary>
        SHIFT = 0x10,

        /// <summary>
        /// CTRL key.
        /// </summary>
        CONTROL = 0x11,

        /// <summary>
        /// LEFT ARROW key.
        /// </summary>
        LEFT = 0x25,

        /// <summary>
        /// V key.
        /// </summary>
        KEY_V = 0x56,
    }

    public static class KeyboardInputInjector
    {
        public static void InjectInput(List<INPUT> inputs)
        {
            _ = SendInput((uint)inputs.Count, inputs.ToArray(), INPUT.Size);
        }

        public static void Paste()
        {
            VirtualKeyShort[] vks = new VirtualKeyShort[2] { VirtualKeyShort.CONTROL, VirtualKeyShort.KEY_V };
            INPUT[] inputs = InjectedKeyboardInput.FromVirtualKeys(vks);
            _ = SendInput((uint)inputs.Length, inputs, INPUT.Size);
        }

        public static bool SendKeyDown(IntPtr hWnd, int vkCode)
        {
            return PostMessage(hWnd, (uint)WindowMessage.WM_KEYDOWN, vkCode, 0);
        }

        public static bool SendKeyUp(IntPtr hWnd, int vkCode)
        {
            return PostMessage(hWnd, (uint)WindowMessage.WM_KEYUP, vkCode, 0);
        }
    }
}
