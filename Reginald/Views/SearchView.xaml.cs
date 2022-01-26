namespace Reginald.Views
{
    using System;
    using System.Windows;

    /// <summary>
    /// Interaction logic for SearchView.xaml.
    /// </summary>
    public partial class SearchView : HandyControl.Controls.Window
    {
        public SearchView()
        {
            double windowWidth = 600;
            Left = (SystemParameters.WorkArea.Width - windowWidth) / 2;
            Top = (SystemParameters.WorkArea.Height - ActualHeight) / 8;
            InitializeComponent();
        }

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
    }
}
