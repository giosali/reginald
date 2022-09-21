namespace Reginald.Core.IO.Hooks
{
    using System;

    public class MouseClickEventArgs : EventArgs
    {
        public MouseClickEventArgs()
        {
        }

        public MouseClickEventArgs(IntPtr hWnd)
        {
            Handle = hWnd;
        }

        public IntPtr Handle { get; set; }
    }
}
