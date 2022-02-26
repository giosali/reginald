namespace Reginald.Services.Hooks
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using static Reginald.Services.Hooks.NativeMethods;

    /// <summary>
    /// For more information, see the <see href="https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-mapvirtualkeya">Microsoft Documentation on virtual keys</see>.
    /// </summary>
    public enum MapVirtualKeyMapType : uint
    {
        /// <summary>
        /// The uCode parameter is a virtual-key code and is translated into a scan code. If it is a virtual-key code that does not distinguish between left- and right-hand keys, the left-hand scan code is returned. If there is no translation, the function returns 0.
        /// </summary>
        MAPVK_VK_TO_VSC = 0x0,

        /// <summary>
        /// The uCode parameter is a scan code and is translated into a virtual-key code that does not distinguish between left- and right-hand keys. If there is no translation, the function returns 0.
        /// </summary>
        MAPVK_VSC_TO_VK = 0x1,

        /// <summary>
        /// The uCode parameter is a virtual-key code and is translated into an unshifted character value in the low order word of the return value. Dead keys (diacritics) are indicated by setting the top bit of the return value. If there is no translation, the function returns 0.
        /// </summary>
        MAPVK_VK_TO_CHAR = 0x2,

        /// <summary>
        /// The uCode parameter is a scan code and is translated into a virtual-key code that distinguishes between left- and right-hand keys. If there is no translation, the function returns 0.
        /// </summary>
        MAPVK_VSC_TO_VK_EX = 0x3,

        /// <summary>
        /// Windows Vista and later: The uCode parameter is a virtual-key code and is translated into a scan code. If it is a virtual-key code that does not distinguish between left- and right-hand keys, the left-hand scan code is returned. If the scan code is an extended scan code, the high byte of the uCode value can contain either 0xe0 or 0xe1 to specify the extended scan code. If there is no translation, the function returns 0.
        /// </summary>
        MAPVK_VK_TO_VSC_EX = 0x04,
    }

    public class KeyboardHook : Hook
    {
        private readonly LowLevelKeyboardProc _proc;

        public KeyboardHook()
        {
            _proc = HookCallback;
            ProcHandle = GCHandle.Alloc(_proc);
            HookId = IntPtr.Zero;
        }

        public KeyboardHook(bool isBlocking)
        {
            IsBlocking = isBlocking;
            _proc = HookCallback;
            ProcHandle = GCHandle.Alloc(_proc);
            HookId = IntPtr.Zero;
        }

        public event EventHandler<KeyPressedEventArgs> KeyPressed;

        public bool IsBlocking { get; set; }

        public override void Add()
        {
            HookId = SetHook(_proc);
        }

        public override void Remove()
        {
            _ = UnhookWindowsHookEx(HookId);
            ProcHandle.Free();
        }

        protected override IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && (wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_SYSKEYDOWN))
            {
                KeyPressedEventArgs args = new(Marshal.ReadInt32(lParam));
                EventHandler<KeyPressedEventArgs> handler = KeyPressed;
                handler?.Invoke(this, args);
                if (IsBlocking && !args.IsModifierPressed && !args.IsCapsLockPressed && !args.IsHotkeyPressed)
                {
                    return new IntPtr(1);
                }
            }

            return CallNextHookEx(HookId, nCode, wParam, lParam);
        }

        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using Process curProcess = Process.GetCurrentProcess();
            using ProcessModule curModule = curProcess.MainModule;
            return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
        }

        private void OnKeyPressed(KeyPressedEventArgs e)
        {
            EventHandler<KeyPressedEventArgs> handler = KeyPressed;
            handler?.Invoke(this, e);
        }
    }
}
