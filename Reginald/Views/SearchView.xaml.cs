using System;
using System.ComponentModel;
using System.Windows;

namespace Reginald.Views
{
    /// <summary>
    /// Interaction logic for SearchView.xaml
    /// </summary>
    public partial class SearchView : Window
    {
        private bool _isClosing;
        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            Activate();
        }

        protected override void OnDeactivated(EventArgs e)
        {
            base.OnDeactivated(e);
            if (!_isClosing)
            {
                Close();
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            _isClosing = true;
        }

        public SearchView()
        {
            Left = (SystemParameters.WorkArea.Width - ActualWidth) / 4;
            Top = (SystemParameters.WorkArea.Height - ActualHeight) / 8;
            InitializeComponent();

            UserInput.Focus();
        }
    }
}