﻿namespace Reginald.ViewModels
{
    using System;
    using System.Windows;
    using System.Windows.Input;
    using Reginald.Core.IO;
    using Reginald.Services.Utilities;

    internal class GeneralViewModel : HotkeyViewModelScreen
    {
        public GeneralViewModel()
            : base("General")
        {
        }

        public void LaunchOnStartupToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if (!FileOperations.TryCreateShortcut())
            {
                FileOperations.DeleteShortcut();
            }

            DMS.Settings.Save();
        }

        public void RestartButton_Click(object sender, RoutedEventArgs e) => ProcessUtility.RestartApplication();

        public void ShutdownButton_Click(object sender, RoutedEventArgs e) => Application.Current.Shutdown();

        protected override Key GetKey() => (Key)Enum.Parse(typeof(Key), DMS.Settings.MainKey);

        protected override ModifierKeys GetModifiers() => (ModifierKeys)Enum.Parse(typeof(ModifierKeys), DMS.Settings.MainModifiers);

        protected override void SaveHotkey(string key, string modifiers)
        {
            DMS.Settings.MainKey = key;
            DMS.Settings.MainModifiers = modifiers;
            DMS.Settings.Save();
        }
    }
}
