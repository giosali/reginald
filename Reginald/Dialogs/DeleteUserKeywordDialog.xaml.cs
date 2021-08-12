using ModernWpf.Controls;

namespace Reginald.Dialogs
{
    /// <summary>
    /// Interaction logic for DeleteUserKeywordDialog.xaml
    /// </summary>
    public partial class DeleteUserKeywordDialog : ContentDialog
    {
        public DeleteUserKeywordDialog(string name)
        {
            string message = $"Are you sure you want to delete the '{name}' keyword?";
            UserKeywordName = message;
            InitializeComponent();
        }

        public string UserKeywordName { get; set; }
    }
}
