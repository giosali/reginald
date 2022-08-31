namespace Reginald.ViewModels
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
    using System.Windows.Input;
    using System.Windows.Interop;
    using Caliburn.Micro;
    using Reginald.Services;
    using Reginald.Services.Appearance;
    using Reginald.Services.Utilities;

    internal class SearchPopupViewModelScreen<T> : PopupViewModelScreen<T>
    {
        private string _userInput;

        public SearchPopupViewModelScreen()
        {
            DMS = IoC.Get<DataModelService>();
        }

        public DataModelService DMS { get; set; }

        public string UserInput
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
                _ = WindowUtility.SetFocus(ActiveHandle = source.Handle);

                if (DMS.Theme.IsAcrylicEnabled)
                {
                    AcrylicMaterial acrylicMaterial = new(source.Handle, DMS.Theme.AcrylicOpacity, DMS.Theme.BackgroundBrush);
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
