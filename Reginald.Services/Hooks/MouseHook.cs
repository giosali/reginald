namespace Reginald.Services.Hooks
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using static Reginald.Services.Hooks.NativeMethods;

    public class MouseHook : Hook
    {
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
            if (nCode >= 0 && (WindowMessage)wParam == WindowMessage.WM_LBUTTONDOWN)
            {
                MouseClickEventArgs args = GetCursorPos(out POINT p) ? new(WindowFromPoint(p)) : new();
                OnMouseClick(args);
            }

            return CallNextHookEx(HookId, nCode, wParam, lParam);
        }

        private static IntPtr SetHook(LowLevelMouseProc proc)
        {
            using Process curProcess = Process.GetCurrentProcess();
            using ProcessModule curModule = curProcess.MainModule;
            return SetWindowsHookEx(WH_MOUSE_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
        }

        private void OnMouseClick(MouseClickEventArgs e)
        {
            EventHandler<MouseClickEventArgs> handler = MouseClick;
            handler?.Invoke(this, e);
        }
    }
}
