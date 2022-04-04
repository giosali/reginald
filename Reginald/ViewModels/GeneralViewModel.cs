namespace Reginald.ViewModels
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Input;
    using Caliburn.Micro;
    using HotkeyUtility;
    using HotkeyUtility.Extensions;
    using Reginald.Core.IO;
    using Reginald.Messages;
    using Reginald.Services;
    using Reginald.Services.Utilities;

    public class GeneralViewModel : Screen
    {
        private string _hotkeyInput;

        public GeneralViewModel(ConfigurationService configurationService)
        {
            ConfigurationService = configurationService;
        }

        public ConfigurationService ConfigurationService { get; set; }

        public string HotkeyInput
        {
            get => _hotkeyInput;
            set
            {
                _hotkeyInput = value;
                NotifyOfPropertyChange(() => HotkeyInput);
            }
        }

        public void HotkeyTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            ModifierKeys modifiers = Keyboard.Modifiers;
            switch (modifiers)
            {
                // Selected when a modifier key was pressed (other than the Windows logo key).
                case not ModifierKeys.None when modifiers != ModifierKeys.Windows:
                    Key key;
                    switch (key = e.Key == Key.System ? e.SystemKey : e.Key)
                    {
                        // Selected when a key other than Shift, Control, or Alt is pressed.
                        case < Key.LeftShift:
                        case > Key.RightAlt:
                            // The F12 key is reserved for use by the debugger at all times, so it should not be registered as a hotkey. Even when you are not debugging an application, F12 is reserved in case a kernel-mode debugger or a just-in-time debugger is resident.
                            if (key == Key.F12)
                            {
                                break;
                            }

                            Key reginaldKey = (Key)Enum.Parse(typeof(Key), ConfigurationService.Settings.ReginaldKey);
                            ModifierKeys reginaldModifiers = (ModifierKeys)Enum.Parse(typeof(ModifierKeys), ConfigurationService.Settings.ReginaldModifiers);

                            // Ensures that if the user tries registering a key binding that's
                            // clearly already registered, the request will simply be ignored.
                            if (key == reginaldKey && modifiers == reginaldModifiers)
                            {
                                break;
                            }

                            HotkeyManager hotkeyManager = HotkeyManager.GetHotkeyManager();
                            Hotkey hotkey = hotkeyManager.GetHotkeys().Find(reginaldKey, reginaldModifiers);
                            if (hotkey is not null)
                            {
                                ConfigurationService.Settings.ReginaldKey = key.ToString();
                                ConfigurationService.Settings.ReginaldModifiers = modifiers.ToString();
                                ConfigurationService.Settings.Save();
                                HotkeyInput = ConvertKeysToString(key, modifiers);
                            }

                            break;
                    }

                    break;
            }
        }

        public void LaunchOnStartupToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if (!FileOperations.TryCreateShortcut())
            {
                FileOperations.DeleteShortcut();
            }

            ConfigurationService.Settings.Save();
        }

        public void RestartButton_Click(object sender, RoutedEventArgs e)
        {
            ProcessUtility.RestartApplication();
        }

        public void ShutdownButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        protected override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            HotkeyInput = ConvertStringToStringKeyRepresentation(ConfigurationService.Settings.ReginaldKey, ConfigurationService.Settings.ReginaldModifiers);

            IEventAggregator eventAggregator = IoC.Get<IEventAggregator>();
            _ = eventAggregator.PublishOnUIThreadAsync(new UpdatePageMessage("General"), cancellationToken);
            return base.OnActivateAsync(cancellationToken);
        }

        private static string ConvertStringToStringKeyRepresentation(string key, string modifiers)
        {
            return modifiers.Replace(",", " +") + " + " + key;
        }

        private static string ConvertKeysToString(Key key, ModifierKeys modifiers)
        {
            return modifiers.ToString().Replace(",", " +") + " + " + key.ToString();
        }
    }
}
