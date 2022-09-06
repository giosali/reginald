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
    using Reginald.Services;
    using Reginald.Services.Utilities;

    internal class ShellViewModel : Conductor<object>
    {
        private readonly ClipboardManagerPopupViewModel _cmpvm;

        private readonly MainViewModel _mvm;

        private readonly IWindowManager _windowManager;

        private bool _isEnabled = true;

        public ShellViewModel(IWindowManager windowManager, DataModelService dms, MainViewModel mvm, ClipboardManagerPopupViewModel cmpvm)
        {
            _windowManager = windowManager;
            DMS = dms;
            _mvm = mvm;
            _cmpvm = cmpvm;

            if (dms.Settings.RunAtStartup)
            {
                FileOperations.DeleteShortcut();
                _ = FileOperations.TryCreateShortcut();
            }

            // Adds a listener for the clipboard manager.
            _ = ClipboardUtility.GetClipboardUtility(dms.Settings.IsClipboardManagerEnabled, GetView() as Window);

            ToolTipText = $"{FileOperations.ApplicationName} v{Assembly.GetExecutingAssembly().GetName().Version}";
        }

        public DataModelService DMS { get; set; }

        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                _isEnabled = value;
                NotifyOfPropertyChange(() => IsEnabled);
            }
        }

        public string ToolTipText { get; set; }

        public async void ClipboardManagerPopupHotkeyBinding_Pressed(object sender, HotkeyEventArgs e)
        {
            if (!DMS.Settings.IsClipboardManagerEnabled)
            {
                return;
            }

            if (_cmpvm.IsActive)
            {
                _cmpvm.Hide();
                return;
            }

            Dictionary<string, object> settings = new()
            {
                { "PopupAnimation", PopupAnimation.Fade },

                // StaysOpen must be true for the popup to remain draggable
                // when sending its handle a WM_NCLBUTTONDOWN message.
                { "StaysOpen", true },
            };
            await _windowManager.ShowPopupAsync(_cmpvm, settings: settings);
        }

        public async void MainHotkeyBinding_Pressed(object sender, HotkeyEventArgs e)
        {
            if (!IsEnabled)
            {
                return;
            }

            if (_mvm.IsActive)
            {
                _mvm.Hide();
                return;
            }

            Dictionary<string, object> settings = new()
            {
                { "Placement", PlacementMode.Absolute },
                { "HorizontalOffset", (SystemParameters.FullPrimaryScreenWidth / 2) - (DMS.Theme.MainWidth / 2) },
                { "VerticalOffset", (SystemParameters.FullPrimaryScreenHeight / 2 * 0.325) - (DMS.Theme.MainHeight / 4) },
            };
            await _windowManager.ShowPopupAsync(_mvm, settings: settings);
        }

        public async void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            switch ((e.Source as MenuItem).Tag)
            {
                case "Settings":
                    await _windowManager.ShowWindowAsync(IoC.GetInstance(typeof(SettingsViewModel), null));
                    break;
                case "RunAtStartup":
                    // Checks if the shortcut already exists by checking if
                    // TryCreateShortcut returns false.
                    // If it returns false, the shortcut gets deleted.
                    if (!FileOperations.TryCreateShortcut())
                    {
                        FileOperations.DeleteShortcut();
                    }

                    DMS.Settings.Save();
                    break;
                case "Exit":
                    Application.Current.Shutdown();
                    break;
            }
        }
    }
}
