namespace Reginald.ViewModels
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
    using System.Windows.Input;
    using System.Windows.Interop;
    using Reginald.Services;
    using Reginald.Services.Appearance;

    public class SearchPopupViewModelBase<T> : PopupViewModelBase<T>
    {
        private string _userInput;

        public SearchPopupViewModelBase(ConfigurationService configurationService)
        {
            ConfigurationService = configurationService;
        }

        public ConfigurationService ConfigurationService { get; set; }

        public virtual string UserInput
        {
            get => _userInput;
            set
            {
                _userInput = value;
                NotifyOfPropertyChange(() => UserInput);
            }
        }

        public void UserInput_Loaded(object sender, RoutedEventArgs e)
        {
            if (GetView() is Popup popup && PresentationSource.FromVisual(popup.Child) is HwndSource source)
            {
                // Brings popup to front without stealing focus from the foreground window
                _ = Services.Devices.Keyboard.SetFocus(ActiveHandle = source.Handle);

                if (ConfigurationService.Theme.IsAcrylicEnabled)
                {
                    AcrylicMaterial acrylicMaterial = new(source.Handle, ConfigurationService.Theme.AcrylicOpacity, ConfigurationService.Theme.BackgroundBrush);
                    acrylicMaterial.Enable();
                }
            }
        }

        public void UserInput_Unloaded(object sender, RoutedEventArgs e)
        {
            BindingOperations.ClearBinding(sender as TextBox, TextBox.TextProperty);
        }

        public void UserInput_LayoutUpdated(object sender, EventArgs e)
        {
            // Sets focus on the main textbox since, for some reason, the textbox loses focus
            // some time between the textbox being loaded and the texbox's layout being updated
            if (Keyboard.FocusedElement != sender)
            {
                Keyboard.Focus(sender as TextBox);
            }
        }
    }
}
