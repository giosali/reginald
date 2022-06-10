namespace Reginald.Services.Hooks
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
