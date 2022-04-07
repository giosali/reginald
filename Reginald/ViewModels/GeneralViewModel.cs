namespace Reginald.ViewModels
{
    using System;
    using System.Windows;
    using System.Windows.Input;
    using Reginald.Core.IO;
    using Reginald.Services;
    using Reginald.Services.Utilities;

    public class GeneralViewModel : HotkeyViewModelBase
    {
        public GeneralViewModel(ConfigurationService configurationService)
            : base(configurationService, "General")
        {
        }

        public void LaunchOnStartupToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if (!FileOperations.TryCreateShortcut())
            {
                FileOperations.DeleteShortcut();
            }

            ConfigurationService.Settings.Save();
        }

        public void RestartButton_Click(object sender, RoutedEventArgs e) => ProcessUtility.RestartApplication();

        public void ShutdownButton_Click(object sender, RoutedEventArgs e) => Application.Current.Shutdown();

        protected override Key GetKey() => (Key)Enum.Parse(typeof(Key), ConfigurationService.Settings.ReginaldKey);

        protected override ModifierKeys GetModifiers() => (ModifierKeys)Enum.Parse(typeof(ModifierKeys), ConfigurationService.Settings.ReginaldModifiers);

        protected override void SaveHotkey(string key, string modifiers)
        {
            ConfigurationService.Settings.ReginaldKey = key;
            ConfigurationService.Settings.ReginaldModifiers = modifiers;
            ConfigurationService.Settings.Save();
        }
    }
}
