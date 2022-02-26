namespace Reginald.Services.Hooks
{
    using System;
    using System.Windows.Input;

    public class KeyPressedEventArgs : EventArgs
    {
        public KeyPressedEventArgs(int vkCode)
        {
            VirtualKeyCode = vkCode;
            Key key = Key = KeyInterop.KeyFromVirtualKey(vkCode);
            switch (key)
            {
                case Key.LeftAlt:
                case Key.RightAlt:
                case Key.LeftCtrl:
                case Key.RightCtrl:
                case Key.LeftShift:
                case Key.RightShift:
                case Key.LWin:
                case Key.RWin:
                    IsModifierPressed = true;
                    break;

                case Key.VolumeDown:
                case Key.VolumeMute:
                case Key.VolumeUp:
                    IsVolumeKeyPressed = true;
                    break;
            }

            IsCapsLockPressed = key == Key.CapsLock;
        }

        public int VirtualKeyCode { get; set; }

        public Key Key { get; set; }

        public bool IsModifierPressed { get; set; }

        public bool IsVolumeKeyPressed { get; set; }

        public bool IsCapsLockPressed { get; set; }

        public bool IsHotkeyPressed { get; set; }
    }
}
