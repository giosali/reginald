namespace Reginald.Core.IO.Hooks
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using static Reginald.Core.IO.Hooks.NativeMethods;

    public class MouseHook : Hook
    {
        /// <summary>
        /// Installs a hook procedure that monitors low-level mouse input events. For more information, see the <see href="https://docs.microsoft.com/en-us/previous-versions/windows/desktop/legacy/ms644985(v=vs.85)">LowLevelKeyboardProc hook procedure</see>.
        /// </summary>
        private const int WH_MOUSE_LL = 14;

        private const int WM_LBUTTONDOWN = 0x0201;

        private static LowLevelMouseProc _proc;

        public MouseHook()
        {
            _proc = HookCallback;
            ProcHandle = GCHandle.Alloc(_proc);
            HookId = IntPtr.Zero;
        }

        public event EventHandler<MouseClickEventArgs> MouseClick;

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
                case WM_LBUTTONDOWN:
                    MouseClickEventArgs args = GetCursorPos(out POINT p) ? new(WindowFromPoint(p)) : new();
                    MouseClick?.Invoke(this, args);
                    break;
            }

            return CallNextHookEx(HookId, nCode, wParam, lParam);
        }

        private static IntPtr SetHook(LowLevelMouseProc proc)
        {
            using Process curProcess = Process.GetCurrentProcess();
            using ProcessModule curModule = curProcess.MainModule;
            return SetWindowsHookEx(WH_MOUSE_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
        }
    }
}
