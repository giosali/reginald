namespace Reginald.ViewModels
{
    using System;
    using System.Windows;
    using System.Windows.Input;
    using Reginald.Core.IO;
    using Reginald.Core.Services;

    internal sealed class GeneralViewModel : HotkeyViewModelScreen
    {
        public GeneralViewModel()
            : base("General")
        {
        }

        public void RunAtStartupToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if (!FileOperations.TryCreateShortcut())
            {
                FileOperations.DeleteShortcut();
            }

            DMS.Settings.Save();
        }

        public void RestartButton_Click(object sender, RoutedEventArgs e)
        {
            ProcessService.RestartApplication();
        }

        public void ShutdownButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        protected override Key GetKey()
        {
            return (Key)Enum.Parse(typeof(Key), DMS.Settings.MainKey);
        }

        protected override ModifierKeys GetModifiers()
        {
            return (ModifierKeys)Enum.Parse(typeof(ModifierKeys), DMS.Settings.MainModifiers);
        }

        protected override void SaveHotkey(string key, string modifiers)
        {
            DMS.Settings.MainKey = key;
            DMS.Settings.MainModifiers = modifiers;
            DMS.Settings.Save();
        }
    }
}
