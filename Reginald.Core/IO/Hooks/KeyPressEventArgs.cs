namespace Reginald.Core.IO.Hooks
{
    using System;
    using System.Windows.Input;

    public class KeyPressEventArgs : EventArgs
    {
        private readonly bool _isCapsLockPressed;

        private readonly bool _isModifierPressed;

        private readonly bool _isPrintScreenPressed;

        private readonly bool _isVolumeKeyPressed;

        public KeyPressEventArgs(int vkCode, bool isDown)
        {
            IsDown = isDown;
            VirtualKeyCode = vkCode;
            Key = KeyInterop.KeyFromVirtualKey(vkCode);
            switch (Key)
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

        public bool IsDown { get; set; }

        public bool IsHotkeyPressed { get; set; }

        public bool IsImportantKeyPressed => IsHotkeyPressed || _isModifierPressed || _isVolumeKeyPressed || _isCapsLockPressed || _isPrintScreenPressed;

        public Key Key { get; set; }

        public int VirtualKeyCode { get; set; }
    }
}
