namespace Reginald.ViewModels
{
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using Caliburn.Micro;
    using HotkeyUtility;
    using HotkeyUtility.Extensions;
    using Reginald.Messages;
    using Reginald.Services;

    public abstract class HotkeyViewModelScreen : Screen
    {
        private readonly string _pageName;

        private string _hotkeyInput;

        public HotkeyViewModelScreen(ConfigurationService configurationService, string pageName)
        {
            ConfigurationService = configurationService;
            _pageName = pageName;
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

                            Key hotkeyKey = GetKey();
                            ModifierKeys hotkeyModifiers = GetModifiers();

                            // Ensures that if the user tries registering a key binding that's
                            // clearly already registered, the request will simply be ignored.
                            if (key == hotkeyKey && modifiers == hotkeyModifiers)
                            {
                                break;
                            }

                            HotkeyManager hotkeyManager = HotkeyManager.GetHotkeyManager();
                            Hotkey hotkey = hotkeyManager.GetHotkeys().Find(hotkeyKey, hotkeyModifiers);
                            if (hotkey is not null)
                            {
                                SaveHotkey(key.ToString(), modifiers.ToString());
                                HotkeyInput = ConvertKeysToString(key, modifiers);
                            }

                            break;
                    }

                    break;
            }
        }

        protected abstract Key GetKey();

        protected abstract ModifierKeys GetModifiers();

        protected abstract void SaveHotkey(string key, string modifiers);

        protected override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            HotkeyInput = ConvertKeysToString(GetKey(), GetModifiers());

            IEventAggregator eventAggregator = IoC.Get<IEventAggregator>();
            _ = eventAggregator.PublishOnUIThreadAsync(new UpdatePageMessage(_pageName), cancellationToken);
            return base.OnActivateAsync(cancellationToken);
        }

        private static string ConvertKeysToString(Key key, ModifierKeys modifiers)
        {
            return modifiers.ToString().Replace(",", " +") + " + " + key.ToString();
        }
    }
}
