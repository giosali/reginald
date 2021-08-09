using System.Windows;
using System.Windows.Input;

namespace Reginald.Views
{
    /// <summary>
    /// Interaction logic for SettingsView.xaml
    /// </summary>
    public partial class ShellView : Window
    {
        public ShellView()
        {
            SearchGesture = new KeyGesture(Key.Space, ModifierKeys.Alt);
            InitializeComponent();
        }

        public static KeyGesture SearchGesture { get; set; }
    }
}
