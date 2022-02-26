namespace Reginald.ViewModels
{
    using System.Collections.Generic;
    using System.Windows;
    using Caliburn.Micro;
    using HotkeyUtility.Input;
    using Reginald.Core.IO;
    using Reginald.Data.Expansions;
    using Reginald.Services.Clipboard;
    using Reginald.Services.Hooks;

    public class ShellViewModel : Conductor<object>
    {
        private bool _isEnabled = true;

        public ShellViewModel()
        {
            FileOperations.SetUp();
            SearchView.Settings.Save();
            if (SearchView.Settings.LaunchOnStartup)
            {
                _ = FileOperations.TryCreateShortcut();
            }

            // Adds low-level hook for text expansions
            KeyboardHook keyboardHook = new();
            keyboardHook.Add();
            TextExpansionManager = new(SearchView.Settings);
            keyboardHook.KeyPressed += TextExpansionManager.KeyPressed;

            // Adds listener for Clipboard
            ClipboardUtility.GetClipboardUtility(GetView() as Window);
        }

        public SearchViewModel SearchView { get; set; } = new();

        public ClipboardManagerPopupViewModel ClipboardManagerPopup { get; set; } = new();

        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                _isEnabled = value;
                NotifyOfPropertyChange(() => IsEnabled);
            }
        }

        private TextExpansionManager TextExpansionManager { get; set; }

        public async void SearchWindowHotkeyBinding_Pressed(object sender, HotkeyEventArgs e)
        {
            if (IsEnabled)
            {
                IWindowManager manager = new WindowManager();
                if (SearchView.IsActive)
                {
                    // Closes search window and empties the input
                    SearchView.UserInput = string.Empty;
                    SearchView.ShowOrHide();
                }
                else
                {
                    // Opens search window
                    await manager.ShowWindowAsync(SearchView);
                }
            }
        }

        public async void ClipboardManagerPopupHotkeyBinding_Pressed(object sender, HotkeyEventArgs e)
        {
            if (ClipboardManagerPopup.IsActive)
            {
                ClipboardManagerPopup.ShowOrHide();
            }
            else
            {
                IWindowManager manager = new WindowManager();
                Dictionary<string, object> settings = new();

                // Allows popup to use acrylic material
                settings.Add("AllowsTransparency", false);
                settings.Add("PopupAnimation", System.Windows.Controls.Primitives.PopupAnimation.Fade);

                // Allows popup to remain draggable when sending its handle WM_NCLBUTTONDOWN
                settings.Add("StaysOpen", true);
                await manager.ShowPopupAsync(ClipboardManagerPopup, settings: settings);
            }
        }

        public void OpenSettingsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            IWindowManager manager = new WindowManager();
            _ = manager.ShowWindowAsync(new SettingsViewModel());
        }

        public void LaunchOnStartupMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (!FileOperations.TryCreateShortcut())
            {
                FileOperations.DeleteShortcut();
            }

            SearchView.Settings.Save();
        }
    }
}
