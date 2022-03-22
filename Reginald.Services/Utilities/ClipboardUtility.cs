namespace Reginald.Services.Utilities
{
    using System;
    using System.Windows;
    using System.Windows.Interop;
    using static Reginald.Services.Utilities.NativeMethods;

    public class ClipboardUtility
    {
        protected ClipboardUtility()
        {
            if (Handle == IntPtr.Zero)
            {
                HwndSourceParameters parameters = new("ClipboardUtility")
                {
                    WindowStyle = 0,
                };

                HwndSource source = new(parameters);
                source.AddHook(WndProc);
                Handle = source.Handle;
                _ = AddClipboardFormatListener(Handle);
            }
        }

        protected ClipboardUtility(Window window)
        {
            HwndSource source = PresentationSource.FromVisual(window) as HwndSource;
            if (source is not null)
            {
                source.AddHook(WndProc);
                IntPtr hWnd = new WindowInteropHelper(window).Handle;
                _ = AddClipboardFormatListener(hWnd);
            }
        }

        public event EventHandler<EventArgs> ClipboardChanged;

        private static ClipboardUtility Instance { get; set; }

        private static IntPtr Handle { get; set; }

        public static ClipboardUtility GetClipboardUtility()
        {
            return Instance ??= new ClipboardUtility();
        }

        public static ClipboardUtility GetClipboardUtility(Window window)
        {
            return Instance ??= new ClipboardUtility(window);
        }

        private void OnClipboardChanged()
        {
            EventHandler<EventArgs> handler = ClipboardChanged;
            handler?.Invoke(this, EventArgs.Empty);
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == (int)WindowMessage.WM_CLIPBOARDUPDATE)
            {
                OnClipboardChanged();
                handled = true;
            }

            return IntPtr.Zero;
        }
    }
}
