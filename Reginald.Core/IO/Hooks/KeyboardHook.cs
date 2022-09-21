namespace Reginald.Core.IO.Hooks
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using static Reginald.Core.IO.Hooks.NativeMethods;

    public class KeyboardHook : Hook
    {
        /// <summary>
        /// Installs a hook procedure that monitors low-level keyboard input events. For more information, see the <see href="https://docs.microsoft.com/en-us/previous-versions/windows/desktop/legacy/ms644985(v=vs.85)">LowLevelKeyboardProc hook procedure</see>.
        /// </summary>
        private const int WH_KEYBOARD_LL = 13;

        private const int WM_KEYDOWN = 0x0100;

        private const int WM_KEYUP = 0x0101;

        private const int WM_SYSKEYDOWN = 0x0104;

        private const int WM_SYSKEYUP = 0x0105;

        private readonly LowLevelKeyboardProc _proc;

        public KeyboardHook(bool isBlocking = false)
        {
            IsBlocking = isBlocking;
            _proc = HookCallback;
            ProcHandle = GCHandle.Alloc(_proc);
            HookId = IntPtr.Zero;
        }

        public event EventHandler<KeyPressEventArgs> KeyPress;

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
            if (nCode < 0)
            {
                return CallNextHookEx(HookId, nCode, wParam, lParam);
            }

            int msg = wParam.ToInt32();
            switch (msg)
            {
                case WM_KEYDOWN:
                case WM_SYSKEYDOWN:
                    KeyPressEventArgs args = new(Marshal.ReadInt32(lParam), true);
                    KeyPress?.Invoke(this, args);

                    // Blocks input to the foreground window
                    // if told to block
                    // and if an important key is pressed
                    if (IsBlocking && !args.IsImportantKeyPressed)
                    {
                        return new IntPtr(1);
                    }

                    break;

                case WM_KEYUP:
                case WM_SYSKEYUP:
                    KeyPress?.Invoke(this, new(Marshal.ReadInt32(lParam), false));
                    break;
            }

            return CallNextHookEx(HookId, nCode, wParam, lParam);
        }

        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using Process curProcess = Process.GetCurrentProcess();
            using ProcessModule curModule = curProcess.MainModule;
            return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
        }
    }
}
