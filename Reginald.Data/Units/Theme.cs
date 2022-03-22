namespace Reginald.Data.Units
{
    using System;
    using System.Windows;
    using System.Windows.Media;
    using Reginald.Core.Helpers;

    public class Theme : Unit
    {
        public const string Filename = "Themes.json";

        private const int MinimumBuild = 18362;

        public Theme(ThemeDataModel model)
        {
            Name = model.Name;
            Author = model.Author;
            if (Guid.TryParse(model.Guid, out Guid guid))
            {
                Guid = guid;
            }

            IsEditable = model.IsEditable;
            IsAcrylicEnabled = model.IsAcrylicEnabled && Environment.OSVersion.Version.Build > MinimumBuild;
            AcrylicOpacity = model.AcrylicOpacity;
            MainWidth = model.MainWidth;
            MainHeight = model.MainHeight;
            FontFamily = new(model.FontFamily);
            InputFontSize = model.InputFontSize;
            InputFontWeight = (FontWeight)new FontWeightConverter().ConvertFromString(model.InputFontWeight);
            DescriptionFontSize = model.DescriptionFontSize;
            DescriptionFontWeight = (FontWeight)new FontWeightConverter().ConvertFromString(model.DescriptionFontWeight);
            CaptionFontSize = model.CaptionFontSize;
            CaptionFontWeight = (FontWeight)new FontWeightConverter().ConvertFromString(model.CaptionFontWeight);
            BorderThickness = model.BorderThickness;
            CornerRadius = model.CornerRadius;
            SelectionOpacity = model.SelectionOpacity;
            ClipboardWidth = model.ClipboardWidth;
            ClipboardHeight = model.ClipboardHeight;
            ClipboardItemFontSize = model.ClipboardItemFontSize;
            ClipboardDisplayFontSize = model.ClipboardDisplayFontSize;

            Application.Current.Dispatcher.Invoke(() =>
            {
                BackgroundBrush = BrushHelper.SolidColorBrushFromString(model.BackgroundBrush);
                InputBrush = BrushHelper.SolidColorBrushFromString(model.InputBrush);
                PlaceholderInputBrush = BrushHelper.SolidColorBrushFromString(model.PlaceholderInputBrush);
                CaretBrush = BrushHelper.SolidColorBrushFromString(model.CaretBrush);
                DescriptionBrush = BrushHelper.SolidColorBrushFromString(model.DescriptionBrush);
                SelectedDescriptionBrush = BrushHelper.SolidColorBrushFromString(model.SelectedDescriptionBrush);
                CaptionBrush = BrushHelper.SolidColorBrushFromString(model.CaptionBrush);
                SelectedCaptionBrush = BrushHelper.SolidColorBrushFromString(model.SelectedCaptionBrush);
                BorderBrush = BrushHelper.SolidColorBrushFromString(model.BorderBrush);
                HighlightBrush = BrushHelper.SolidColorBrushFromString(model.HighlightBrush);
                SelectionBrush = BrushHelper.SolidColorBrushFromString(model.SelectionBrush);
            });
        }

        public string Author { get; set; }

        public bool IsEditable { get; set; }

        public bool IsAcrylicEnabled { get; set; }

        public byte AcrylicOpacity { get; set; }

        public double MainWidth { get; set; }

        public double MainHeight { get; set; }

        public FontFamily FontFamily { get; set; }

        public Brush BackgroundBrush { get; set; }

        public double InputFontSize { get; set; }

        public FontWeight InputFontWeight { get; set; }

        public Brush InputBrush { get; set; }

        public Brush PlaceholderInputBrush { get; set; }

        public Brush CaretBrush { get; set; }

        public double DescriptionFontSize { get; set; }

        public FontWeight DescriptionFontWeight { get; set; }

        public Brush DescriptionBrush { get; set; }

        public Brush SelectedDescriptionBrush { get; set; }

        public double CaptionFontSize { get; set; }

        public FontWeight CaptionFontWeight { get; set; }

        public Brush CaptionBrush { get; set; }

        public Brush SelectedCaptionBrush { get; set; }

        public Brush BorderBrush { get; set; }

        public double BorderThickness { get; set; }

        public double CornerRadius { get; set; }

        public Brush HighlightBrush { get; set; }

        public Brush SelectionBrush { get; set; }

        public double SelectionOpacity { get; set; }

        public double ClipboardWidth { get; set; }

        public double ClipboardHeight { get; set; }

        public double ClipboardItemFontSize { get; set; }

        public double ClipboardDisplayFontSize { get; set; }
    }
}
