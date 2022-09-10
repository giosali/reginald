namespace Reginald.ViewModels
{
    using System.Globalization;
    using System.Windows;
    using System.Windows.Controls;
    using Reginald.Services;

    internal class CalculatorViewModel : ItemScreen
    {
        private bool _isCommaChecked;

        private bool _isPointChecked;

        private bool _isSystemLocaleChecked;

        public CalculatorViewModel(DataModelService dms)
            : base("Features > Calculator")
        {
            DataModelService = dms;
            switch (dms.Settings.DecimalSeparator)
            {
                case '\0':
                    IsSystemLocaleChecked = true;
                    break;
                case ',':
                    IsCommaChecked = true;
                    break;
                case '.':
                    IsPointChecked = true;
                    break;
            }
        }

        public DataModelService DataModelService { get; set; }

        public bool IsCommaChecked
        {
            get => _isCommaChecked;
            set
            {
                _isCommaChecked = value;
                NotifyOfPropertyChange(() => IsCommaChecked);
            }
        }

        public bool IsPointChecked
        {
            get => _isPointChecked;
            set
            {
                _isPointChecked = value;
                NotifyOfPropertyChange(() => IsPointChecked);
            }
        }

        public bool IsSystemLocaleChecked
        {
            get => _isSystemLocaleChecked;
            set
            {
                _isSystemLocaleChecked = value;
                NotifyOfPropertyChange(() => IsSystemLocaleChecked);
            }
        }

        public void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is not RadioButton radioButton || radioButton.Tag is not string tag)
            {
                return;
            }

            DataModelService.Settings.DecimalSeparator = tag.Length == 1 ? tag[0] : (char)int.Parse(tag, NumberStyles.AllowHexSpecifier);
            DataModelService.Settings.Save();
        }
    }
}
