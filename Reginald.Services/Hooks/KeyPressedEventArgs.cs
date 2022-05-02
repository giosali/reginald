namespace Reginald.Services.Hooks
{
    using System;
    using System.Windows.Input;

    public class KeyPressedEventArgs : EventArgs
    {
        private readonly bool _isModifierPressed;

        private readonly bool _isVolumeKeyPressed;

        private readonly bool _isCapsLockPressed;

        private readonly bool _isPrintScreenPressed;

        public KeyPressedEventArgs(int vkCode, bool isDown)
        {
            IsDown = isDown;
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
                    _isModifierPressed = true;
                    break;

                case Key.VolumeDown:
                case Key.VolumeMute:
                case Key.VolumeUp:
                    _isVolumeKeyPressed = true;
                    break;

                case Key.CapsLock:
                    _isCapsLockPressed = true;
                    break;

                case Key.PrintScreen:
                    _isPrintScreenPressed = true;
                    break;
            }
        }

        public int VirtualKeyCode { get; set; }

        public Key Key { get; set; }

        public bool IsDown { get; set; }

        public bool IsHotkeyPressed { get; set; }

        public bool IsImportantKeyPressed => IsHotkeyPressed || _isModifierPressed || _isVolumeKeyPressed || _isCapsLockPressed || _isPrintScreenPressed;
    }
}
