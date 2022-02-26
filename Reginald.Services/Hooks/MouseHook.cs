namespace Reginald.Services.Hooks
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Reginald.Services.Devices;
    using static Reginald.Services.Hooks.NativeMethods;

    /// <summary>
    /// For more information, see <see href="https://docs.microsoft.com/en-us/windows/win32/inputdev/mouse-input-notifications">Microsoft Documentation on mouse input notifications</see>.
    /// </summary>
    public enum MouseMessages
    {
        /// <summary>
        /// Posted to a window when the cursor moves. If the mouse is not captured, the message is posted to the window that contains the cursor. Otherwise, the message is posted to the window that has captured the mouse.
        /// </summary>
        WM_MOUSEMOVE = 0x0200,

        /// <summary>
        /// Posted when the user presses the left mouse button while the cursor is in the client area of a window. If the mouse is not captured, the message is posted to the window beneath the cursor. Otherwise, the message is posted to the window that has captured the mouse.
        /// </summary>
        WM_LBUTTONDOWN = 0x0201,

        /// <summary>
        /// Posted when the user releases the left mouse button while the cursor is in the client area of a window. If the mouse is not captured, the message is posted to the window beneath the cursor. Otherwise, the message is posted to the window that has captured the mouse.
        /// </summary>
        WM_LBUTTONUP = 0x0202,

        /// <summary>
        /// Posted when the user presses the right mouse button while the cursor is in the client area of a window. If the mouse is not captured, the message is posted to the window beneath the cursor. Otherwise, the message is posted to the window that has captured the mouse.
        /// </summary>
        WM_RBUTTONDOWN = 0x0204,

        /// <summary>
        /// Posted when the user releases the right mouse button while the cursor is in the client area of a window. If the mouse is not captured, the message is posted to the window beneath the cursor. Otherwise, the message is posted to the window that has captured the mouse.
        /// </summary>
        WM_RBUTTONUP = 0x0205,

        /// <summary>
        /// Sent to the focus window when the mouse wheel is rotated. The DefWindowProc function propagates the message to the window's parent. There should be no internal forwarding of the message, since DefWindowProc propagates it up the parent chain until it finds a window that processes it.
        /// </summary>
        WM_MOUSEWHEEL = 0x020A,
    }

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
            if (nCode >= 0 && (MouseMessages)wParam == MouseMessages.WM_LBUTTONDOWN)
            {
                MouseClickEventArgs args = Mouse.TryGetWindowThreadProcessIdFromCursorPos(out uint threadProcessId) ? new(threadProcessId) : new();
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
