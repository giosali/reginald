namespace Reginald.Core.Utilities
{
    using System;
    using System.Windows;
    using System.Windows.Interop;

    public class ClipboardUtility
    {
        private const int WM_CLIPBOARDUPDATE = 0x031D;

        public event EventHandler<EventArgs> ClipboardChanged;

        public ClipboardUtility(Window window)
        {
            //HwndSource source = PresentationSource.FromVisual(window) as HwndSource;
            //if (source is not null)
            //{
            //    source.AddHook(WndProc);
            //    IntPtr hWnd = new WindowInteropHelper(window).Handle;
            //    _ = AddClipboardFormatListener(hWnd);
            //}
        }
    }

    internal static class NativeMethods
    {

    }
}
