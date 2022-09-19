namespace Reginald.Controls
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;

    internal sealed class LabeledTextBox : TextBox
    {
        public static readonly DependencyProperty LabelProperty = DependencyProperty.Register(
            "Label",
            typeof(string),
            typeof(LabeledTextBox),
            new FrameworkPropertyMetadata
            {
                DefaultValue = null,
                DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
            });

        public static readonly DependencyProperty PlaceholderProperty = DependencyProperty.Register(
            "Placeholder",
            typeof(string),
            typeof(LabeledTextBox),
            new FrameworkPropertyMetadata
            {
                DefaultValue = null,
                DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
            });

        public static readonly DependencyProperty IsRequiredProperty = DependencyProperty.Register(
            "IsRequired",
            typeof(bool),
            typeof(LabeledTextBox),
            new FrameworkPropertyMetadata
            {
                DefaultValue = false,
                DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
            });

        static LabeledTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LabeledTextBox), new FrameworkPropertyMetadata(typeof(LabeledTextBox)));
        }

        public string Label
        {
            get => (string)GetValue(LabelProperty);
            set => SetValue(LabelProperty, value);
        }

        public string Placeholder
        {
            get => (string)GetValue(PlaceholderProperty);
            set => SetValue(PlaceholderProperty, value);
        }

        public bool IsRequired
        {
            get => (bool)GetValue(IsRequiredProperty);
            set => SetValue(IsRequiredProperty, value);
        }
    }
}
