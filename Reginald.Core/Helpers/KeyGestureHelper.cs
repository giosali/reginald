namespace Reginald.Core.Helpers
{
    using System;
    using System.Windows.Input;

    public static class KeyGestureHelper
    {
        public static KeyGesture FromStrings(string key, string firstModifierKey, string secondModifierKey)
        {
            Key sbKey = (Key)Enum.Parse(typeof(Key), key);
            ModifierKeys sbModifierKeyOne = (ModifierKeys)Enum.Parse(typeof(ModifierKeys), firstModifierKey);
            ModifierKeys sbModifierKeyTwo = (ModifierKeys)Enum.Parse(typeof(ModifierKeys), secondModifierKey);
            KeyGesture gesture = new(sbKey, sbModifierKeyOne | sbModifierKeyTwo);
            return gesture;
        }
    }
}
