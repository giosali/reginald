namespace Reginald.Core.DataExchange
{
    using System;
    using System.Windows;
    using System.Windows.Interop;
    using static Reginald.Core.DataExchange.NativeMethods;

    public class ClipboardListener
    {
        protected ClipboardListener()
        {
            if (Handle != IntPtr.Zero)
            {
                return;
            }

            HwndSourceParameters parameters = new("ReginaldClipboardListener") { WindowStyle = 0 };
            HwndSource source = new(parameters);
            source.AddHook(WndProc);
            Handle = source.Handle;
            _ = AddClipboardFormatListener(Handle);
        }

        protected ClipboardListener(Window window)
        {
            if (PresentationSource.FromVisual(window) is not HwndSource source)
            {
                return;
            }

            source.AddHook(WndProc);
            _ = AddClipboardFormatListener(new WindowInteropHelper(window).Handle);
        }

        public event EventHandler<EventArgs> ClipboardChanged;

        public static bool IsEnabled { get; set; }

        private static ClipboardListener Instance { get; set; }

        private static IntPtr Handle { get; set; }

        public static ClipboardListener GetClipboardListener()
        {
            return Instance ??= new ClipboardListener();
        }

        public static ClipboardListener GetClipboardListener(bool isEnabled, Window window)
        {
            IsEnabled = isEnabled;
            return Instance ??= new ClipboardListener(window);
        }

        private void OnClipboardChanged()
        {
            if (!IsEnabled)
            {
                return;
            }

            EventHandler<EventArgs> handler = ClipboardChanged;
            handler?.Invoke(this, EventArgs.Empty);
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            // 0x31D = WM_CLIPBOARDUPDATE
            if (msg == 0x031D)
            {
                OnClipboardChanged();
                handled = true;
            }

            return IntPtr.Zero;
        }
    }
}
