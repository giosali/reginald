using Reginald.Core.Extensions;
using Reginald.Core.Helpers;
using SourceChord.FluentWPF;
using System;
using System.Windows;
using System.Windows.Media;
using System.Xml;

namespace Reginald.Models
{
    public class ThemeModel
    {
        public ThemeModel()
        {

        }

        public ThemeModel(XmlNode node)
        {
            Name = node.Attributes["Name"]?.InnerText;
            Author = node.Attributes["Author"]?.InnerText;
            _ = Guid.TryParse(node["GUID"]?.InnerText, out Guid identifier);
            Identifier = identifier;
            _ = bool.TryParse(node["IsEditable"]?.InnerText, out bool isEditableResult);
            IsEditable = isEditableResult;

            Application.Current.Dispatcher.Invoke(() =>
            {
                BackgroundColor = ColorHelper.FromString(node["BackgroundColor"]?.InnerText);
                InputColor = SolidColorBrushHelper.FromString(node["InputColor"]?.InnerText);
                CaretColor = SolidColorBrushHelper.FromString(node["CaretColor"]?.InnerText);
                DescriptionColor = SolidColorBrushHelper.FromString(node["DescriptionColor"]?.InnerText);
                AltColor = SolidColorBrushHelper.FromString(node["AltColor"]?.InnerText);
                BorderColor = SolidColorBrushHelper.FromString(node["BorderColor"]?.InnerText);

                string defaultHighlightColor = node["HighlightColor"]?.InnerText;
                string highlightColor = Properties.Settings.Default.IsSystemColorEnabled
                                            ? AccentColors.ImmersiveSystemAccentBrush.GetTransparentHex("#99")
                                            : defaultHighlightColor;
                HighlightColor = SolidColorBrushHelper.FromString(highlightColor);
                DefaultHighlightColor = SolidColorBrushHelper.FromString(defaultHighlightColor);

                SpecialMainColor = SolidColorBrushHelper.FromString(node["SpecialMainColor"]?.InnerText);
                SpecialSecondaryColor = SolidColorBrushHelper.FromString(node["SpecialSecondaryColor"]?.InnerText);
                SpecialSubColor = SolidColorBrushHelper.FromString(node["SpecialSubColor"]?.InnerText);
                SpecialBorderColor = SolidColorBrushHelper.FromString(node["SpecialBorderColor"]?.InnerText);
            });
        }

        public string Name { get; set; }
        public string Author { get; set; }
        public Guid Identifier { get; set; }
        public bool IsEditable { get; set; }

        public Color BackgroundColor { get; set; }
        public Brush InputColor { get; set; }
        public Brush CaretColor { get; set; }
        public Brush DescriptionColor { get; set; }
        public Brush AltColor { get; set; }
        public Brush BorderColor { get; set; }
        public Brush HighlightColor { get; set; }
        public Brush DefaultHighlightColor { get; set; }

        public Brush SpecialMainColor { get; set; }
        public Brush SpecialSecondaryColor { get; set; }
        public Brush SpecialSubColor { get; set; }
        public Brush SpecialBorderColor { get; set; }
    }
}
