namespace Reginald.Services.Hooks
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using static Reginald.Services.Hooks.NativeMethods;

    public class KeyboardHook : Hook
    {
        private readonly LowLevelKeyboardProc _proc;

        public KeyboardHook(bool isBlocking = false)
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
            if (nCode >= 0)
            {
                bool isDown;
                if ((isDown = wParam == (IntPtr)WindowMessage.WM_KEYDOWN || wParam == (IntPtr)WindowMessage.WM_SYSKEYDOWN) || wParam == (IntPtr)WindowMessage.WM_KEYUP || wParam == (IntPtr)WindowMessage.WM_SYSKEYUP)
                {
                    KeyPressedEventArgs args = new(Marshal.ReadInt32(lParam), isDown);
                    EventHandler<KeyPressedEventArgs> handler = KeyPressed;
                    handler?.Invoke(this, args);

                    // Blocks input to the foreground window if told to block
                    // and if an important key isn't pressed
                    if (IsBlocking && !args.IsImportantKeyPressed)
                    {
                        return new IntPtr(1);
                    }
                }
            }

            return CallNextHookEx(HookId, nCode, wParam, lParam);
        }

        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using Process curProcess = Process.GetCurrentProcess();
            using ProcessModule curModule = curProcess.MainModule;
            return SetWindowsHookEx((int)HookType.WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
        }
    }
}
