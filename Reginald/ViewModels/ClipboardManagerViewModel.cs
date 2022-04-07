namespace Reginald.ViewModels
{
    using System;
    using System.Windows;
    using System.Windows.Input;
    using Reginald.Services;
    using Reginald.Services.Utilities;

    public class ClipboardManagerViewModel : HotkeyViewModelScreen
    {
        public ClipboardManagerViewModel(ConfigurationService configurationService)
            : base(configurationService, "Clipboard Manager")
        {
        }

        public void ClipboardManagerToggleButton_Click(object sender, RoutedEventArgs e)
        {
            ClipboardUtility.IsEnabled = ConfigurationService.Settings.IsClipboardManagerEnabled;
            ConfigurationService.Settings.Save();
        }

        protected override Key GetKey() => (Key)Enum.Parse(typeof(Key), ConfigurationService.Settings.ClipboardManagerKey);

        protected override ModifierKeys GetModifiers() => (ModifierKeys)Enum.Parse(typeof(ModifierKeys), ConfigurationService.Settings.ClipboardManagerModifiers);

        protected override void SaveHotkey(string key, string modifiers)
        {
            ConfigurationService.Settings.ClipboardManagerKey = key;
            ConfigurationService.Settings.ClipboardManagerModifiers = modifiers;
            ConfigurationService.Settings.Save();
        }
    }
}
