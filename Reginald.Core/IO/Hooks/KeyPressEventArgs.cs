namespace Reginald.Core.IO.Hooks
{
    using System;

    public sealed class KeyPressEventArgs : EventArgs
    {
        public KeyPressEventArgs(int vkCode)
        {
            VirtualKeyCode = vkCode;
        }

        public int VirtualKeyCode { get; private set; }
    }
}
