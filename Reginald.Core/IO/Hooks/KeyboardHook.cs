namespace Reginald.Core.IO.Hooks
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using static Reginald.Core.IO.Hooks.NativeMethods;

    public class KeyboardHook : Hook
    {
        private readonly LowLevelKeyboardProc _proc;

        public KeyboardHook()
        {
            _proc = HookCallback;
            ProcHandle = GCHandle.Alloc(_proc);
            HookId = IntPtr.Zero;
        }

        public event EventHandler<KeyPressEventArgs> KeyPress;

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

            // 0x0100 = WM_KEYDOWN.
            if (wParam.ToInt32() == 0x0100)
            {
                KeyPress?.Invoke(this, new(Marshal.ReadInt32(lParam)));
            }

            return CallNextHookEx(HookId, nCode, wParam, lParam);
        }

        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using Process curProcess = Process.GetCurrentProcess();
            using ProcessModule curModule = curProcess.MainModule;

            // 13 = WH_KEYBOARD_LL.
            return SetWindowsHookEx(13, proc, GetModuleHandle(curModule.ModuleName), 0);
        }
    }
}
