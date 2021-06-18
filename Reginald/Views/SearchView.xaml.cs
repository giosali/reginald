using System;
using System.Windows;

namespace Reginald.Views
{
    /// <summary>
    /// Interaction logic for SearchView.xaml
    /// </summary>
    public partial class SearchView : Window
    {
        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            Activate();
        }

        protected override void OnDeactivated(EventArgs e)
        {
            base.OnDeactivated(e);
            try
            {
                Close();
            }
            catch (InvalidOperationException) { }
        }

        public SearchView()
        {
            Left = ((SystemParameters.WorkArea.Width - ActualWidth) / 4);
            Top = ((SystemParameters.WorkArea.Height - ActualHeight) / 8);
            InitializeComponent();

            SearchResults.Visibility = Visibility.Hidden;
            UserInput.Focus();
        }
    }
}