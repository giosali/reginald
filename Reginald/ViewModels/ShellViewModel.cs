namespace Reginald.ViewModels
{
    using System.Collections.Generic;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using Caliburn.Micro;
    using HotkeyUtility.Input;
    using Reginald.Core.IO;
    using Reginald.Data.Expansions;
    using Reginald.Services;
    using Reginald.Services.Hooks;
    using Reginald.Services.Utilities;

    public class ShellViewModel : Conductor<object>
    {
        private readonly IWindowManager _windowManager;

        private readonly MainViewModel _mainViewModel;

        private readonly ClipboardManagerPopupViewModel _clipboardManagerPopupViewModel;

        private readonly TextExpansionManager _textExpansionManager;

        private bool _isEnabled = true;

        public ShellViewModel(IWindowManager windowManager, ConfigurationService configurationService)
        {
            _windowManager = windowManager;
            ConfigurationService = configurationService;

            _mainViewModel = new(configurationService);
            _clipboardManagerPopupViewModel = new(configurationService);
            if (configurationService.Settings.LaunchOnStartup)
            {
                FileOperations.DeleteShortcut();
                _ = FileOperations.TryCreateShortcut();
            }

            ToolTipText = $"{FileOperations.ApplicationName} v{Assembly.GetExecutingAssembly().GetName().Version}";

            // Adds a low-level hook for text expansions.
            KeyboardHook keyboardHook = new();
            keyboardHook.Add();
            _textExpansionManager = new(configurationService.Settings.AreExpansionsEnabled);
            keyboardHook.KeyPressed += _textExpansionManager.KeyPressed;

            // Adds a listener for the clipboard manager.
            _ = ClipboardUtility.GetClipboardUtility(ConfigurationService.Settings.IsClipboardManagerEnabled, GetView() as Window);
        }

        public ConfigurationService ConfigurationService { get; set; }

        public string ToolTipText { get; set; }

        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                _isEnabled = value;
                NotifyOfPropertyChange(() => IsEnabled);
            }
        }

        public async void SearchWindowHotkeyBinding_Pressed(object sender, HotkeyEventArgs e)
        {
            if (IsEnabled)
            {
                if (_mainViewModel.IsActive)
                {
                    _mainViewModel.Hide();
                }
                else
                {
                    Dictionary<string, object> settings = new();
                    settings.Add("Placement", PlacementMode.Absolute);
                    settings.Add("HorizontalOffset", (SystemParameters.FullPrimaryScreenWidth / 2) - (ConfigurationService.Theme.MainWidth / 2));
                    settings.Add("VerticalOffset", (SystemParameters.FullPrimaryScreenHeight / 2 * 0.325) - (ConfigurationService.Theme.MainHeight / 4));
                    await new WindowManager().ShowPopupAsync(_mainViewModel, settings: settings);
                }
            }
        }

        public async void ClipboardManagerPopupHotkeyBinding_Pressed(object sender, HotkeyEventArgs e)
        {
            if (ConfigurationService.Settings.IsClipboardManagerEnabled)
            {
                if (_clipboardManagerPopupViewModel.IsActive)
                {
                    _clipboardManagerPopupViewModel.Hide();
                }
                else
                {
                    Dictionary<string, object> settings = new();
                    settings.Add("PopupAnimation", PopupAnimation.Fade);

                    // StaysOpen must be true for the popup to remain draggable
                    // when sending its handle a WM_NCLBUTTONDOWN message.
                    settings.Add("StaysOpen", true);
                    await _windowManager.ShowPopupAsync(_clipboardManagerPopupViewModel, settings: settings);
                }
            }
        }

        public async void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            switch ((e.Source as MenuItem).Tag)
            {
                case "Settings":
                    object instance = IoC.GetInstance(typeof(SettingsViewModel), null);
                    await _windowManager.ShowWindowAsync(instance);
                    break;

                case "LaunchOnStartup":
                    // Checks if the shortcut already exists by checking if
                    // TryCreateShortcut returns false.
                    // If it returns false, the shortcut gets deleted.
                    if (!FileOperations.TryCreateShortcut())
                    {
                        FileOperations.DeleteShortcut();
                    }

                    ConfigurationService.Settings.Save();
                    break;

                case "Exit":
                    Application.Current.Shutdown();
                    break;
            }
        }
    }
}
