using Reginald.Core.DataModels;
using Reginald.Core.Helpers;
using Reginald.Core.IO;
using System.Windows;
using System.Windows.Input;

namespace Reginald.Views
{
    /// <summary>
    /// Interaction logic for SettingsView.xaml
    /// </summary>
    public partial class ShellView : Window
    {
        public static KeyGesture SearchBoxKeyGesture { get; set; }

        public ShellView()
        {
            SettingsDataModel Settings = FileOperations.GetSettingsData(ApplicationPaths.SettingsFilename);
            SearchBoxKeyGesture = KeyGestureHelper.FromStrings(Settings.SearchBoxKey, Settings.SearchBoxModifierOne, Settings.SearchBoxModifierTwo);

            InitializeComponent();
        }
    }
}
