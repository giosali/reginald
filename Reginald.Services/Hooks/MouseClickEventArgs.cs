namespace Reginald.Services.Hooks
{
    using System;

    public class MouseClickEventArgs : EventArgs
    {
        public MouseClickEventArgs()
        {
        }

        public MouseClickEventArgs(uint threadProcessId)
        {
            ThreadProcessId = threadProcessId;
        }

        public uint ThreadProcessId { get; set; }
    }
}
