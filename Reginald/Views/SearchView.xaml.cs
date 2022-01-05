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
            _ = Activate();
        }

        protected override void OnDeactivated(EventArgs e)
        {
            base.OnDeactivated(e);
            Hide();
        }

        public SearchView()
        {
            double windowWidth = 600;
            Left = (SystemParameters.WorkArea.Width - windowWidth) / 2;
            Top = (SystemParameters.WorkArea.Height - ActualHeight) / 8;
            InitializeComponent();
        }
    }
}