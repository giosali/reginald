﻿namespace Reginald.ViewModels
{
    using System;
    using System.Windows;
    using System.Windows.Input;
    using Reginald.Core.DataExchange;

    internal sealed class ClipboardManagerViewModel : HotkeyViewModelScreen
    {
        public ClipboardManagerViewModel()
            : base("Features > Clipboard Manager")
        {
        }

        public void ClipboardManagerToggleButton_Click(object sender, RoutedEventArgs e)
        {
            ClipboardListener.IsEnabled = DMS.Settings.IsClipboardManagerEnabled;
            DMS.Settings.Save();
        }

        public void Include_Click(object sender, RoutedEventArgs e)
        {
            DMS.Settings.Save();
        }

        protected override Key GetKey()
        {
            return (Key)Enum.Parse(typeof(Key), DMS.Settings.ClipboardManagerKey);
        }

        protected override ModifierKeys GetModifiers()
        {
            return (ModifierKeys)Enum.Parse(typeof(ModifierKeys), DMS.Settings.ClipboardManagerModifiers);
        }

        protected override void SaveHotkey(string key, string modifiers)
        {
            DMS.Settings.ClipboardManagerKey = key;
            DMS.Settings.ClipboardManagerModifiers = modifiers;
            DMS.Settings.Save();
        }
    }
}
