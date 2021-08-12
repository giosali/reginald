using ModernWpf.Controls;

namespace Reginald.Dialogs
{
    /// <summary>
    /// Interaction logic for WarningDialog.xaml
    /// </summary>
    public partial class WarningDialog : ContentDialog
    {
        public WarningDialog(string message)
        {
            Message = message;
            InitializeComponent();
        }

        public string Message { get; set; }
    }
}
