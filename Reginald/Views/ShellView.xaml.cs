namespace Reginald.Views
{
    using System.Windows;
    using System.Windows.Input;
    using Reginald.Core.DataModels;
    using Reginald.Core.Helpers;
    using Reginald.Core.IO;

    /// <summary>
    /// Interaction logic for SettingsView.xaml.
    /// </summary>
    public partial class ShellView : Window
    {
        public ShellView()
        {
            SettingsDataModel settings = FileOperations.GetSettingsData(ApplicationPaths.SettingsFilename);
            SearchBoxKeyGesture = KeyGestureHelper.FromStrings(settings.SearchBoxKey, settings.SearchBoxModifierOne, settings.SearchBoxModifierTwo);

            InitializeComponent();
        }

        public static KeyGesture SearchBoxKeyGesture { get; set; }
    }
}
