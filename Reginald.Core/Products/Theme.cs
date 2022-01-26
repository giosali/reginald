namespace Reginald.Core.Products
{
    using System;
    using System.Windows;
    using System.Windows.Media;
    using Reginald.Core.AbstractProducts;
    using Reginald.Core.DataModels;
    using Reginald.Core.Helpers;
    using Windows.UI.ViewManagement;

    public class Theme : Unit
    {
        public const int Windows11Build = 22000;

        private const string SystemAccentColor = "SystemAccentColor";

        private string _author;

        private int _minimumBuild;

        private bool _isEditable;

        private bool _requiresRefresh;

        private bool _isAcrylicEnabled;

        private bool _isMicaEnabled;

        private FontFamily _fontFamily;

        private Color _backgroundColor;

        private double _tintOpacity;

        private FontWeight _inputFontWeight;

        private Brush _inputColor;

        private Brush _caretBrush;

        private FontWeight _descriptionFontWeight;

        private Brush _descriptionBrush;

        private Brush _selectedDescriptionBrush;

        private FontWeight _captionFontWeight;

        private Brush _captionBrush;

        private Brush _selectedCaptionBrush;

        private Brush _borderBrush;

        private double _borderThickness;

        private double _cornerRadius;

        private Brush _highlightBrush;

        private Brush _selectionBrush;

        private double _selectionOpacity;

        public Theme(ThemeDataModel model)
        {
            Name = model.Name;
            Author = model.Author;
            MinimumBuild = model.MinimumBuild;
            if (Guid.TryParse(model.Guid, out Guid guid))
            {
                Guid = guid;
            }

            IsEditable = model.IsEditable;
            RequiresRefresh = model.RequiresRefresh;
            IsAcrylicEnabled = model.IsAcrylicEnabled;
            IsMicaEnabled = model.IsMicaEnabled;
            FontFamily = new(model.FontFamily);
            TintOpacity = model.TintOpacity;
            InputFontWeight = (FontWeight)new FontWeightConverter().ConvertFromString(model.InputFontWeight);
            DescriptionFontWeight = (FontWeight)new FontWeightConverter().ConvertFromString(model.DescriptionFontWeight);
            CaptionFontWeight = (FontWeight)new FontWeightConverter().ConvertFromString(model.CaptionFontWeight);
            BorderThickness = model.BorderThickness;
            CornerRadius = model.CornerRadius;
            SelectionOpacity = model.SelectionOpacity;

            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                Windows.UI.Color accentColor = model.IsLightTheme
                                             ? new UISettings().GetColorValue(UIColorType.Accent)
                                             : new UISettings().GetColorValue(UIColorType.AccentLight2);
                Brush accentBrush = new SolidColorBrush(Color.FromArgb(accentColor.A, accentColor.R, accentColor.G, accentColor.B));

                BackgroundColor = ColorHelper.FromString(model.BackgroundColor);
                InputBrush = BrushHelper.SolidColorBrushFromString(model.InputBrush);
                CaretBrush = BrushHelper.SolidColorBrushFromString(model.CaretBrush);
                DescriptionBrush = BrushHelper.SolidColorBrushFromString(model.DescriptionBrush);
                SelectedDescriptionBrush = BrushHelper.SolidColorBrushFromString(model.SelectedDescriptionBrush);
                CaptionBrush = BrushHelper.SolidColorBrushFromString(model.CaptionBrush);
                SelectedCaptionBrush = model.SelectedCaptionBrush == SystemAccentColor
                                     ? accentBrush
                                     : BrushHelper.SolidColorBrushFromString(model.SelectedCaptionBrush);
                BorderBrush = BrushHelper.SolidColorBrushFromString(model.BorderBrush);
                HighlightBrush = BrushHelper.SolidColorBrushFromString(model.HighlightBrush);
                SelectionBrush = model.SelectionBrush == SystemAccentColor
                               ? accentBrush
                               : BrushHelper.SolidColorBrushFromString(model.SelectionBrush);
            });
        }

        public string Author
        {
            get => _author;
            set
            {
                _author = value;
                NotifyOfPropertyChange(() => Author);
            }
        }

        public int MinimumBuild
        {
            get => _minimumBuild;
            set
            {
                _minimumBuild = value;
                NotifyOfPropertyChange(() => MinimumBuild);
            }
        }

        public bool IsEditable
        {
            get => _isEditable;
            set
            {
                _isEditable = value;
                NotifyOfPropertyChange(() => IsEditable);
            }
        }

        public bool RequiresRefresh
        {
            get => _requiresRefresh;
            set
            {
                _requiresRefresh = value;
                NotifyOfPropertyChange(() => RequiresRefresh);
            }
        }

        public bool IsAcrylicEnabled
        {
            get => _isAcrylicEnabled;
            set
            {
                _isAcrylicEnabled = value;
                NotifyOfPropertyChange(() => IsAcrylicEnabled);
            }
        }

        public bool IsMicaEnabled
        {
            get => _isMicaEnabled;
            set
            {
                _isMicaEnabled = value;
                NotifyOfPropertyChange(() => IsMicaEnabled);
            }
        }

        public FontFamily FontFamily
        {
            get => _fontFamily;
            set
            {
                _fontFamily = value;
                NotifyOfPropertyChange(() => FontFamily);
            }
        }

        public Color BackgroundColor
        {
            get => _backgroundColor;
            set
            {
                _backgroundColor = value;
                NotifyOfPropertyChange(() => _backgroundColor);
            }
        }

        public double TintOpacity
        {
            get => _tintOpacity;
            set
            {
                _tintOpacity = value;
                NotifyOfPropertyChange(() => TintOpacity);
            }
        }

        public FontWeight InputFontWeight
        {
            get => _inputFontWeight;
            set
            {
                _inputFontWeight = value;
                NotifyOfPropertyChange(() => InputFontWeight);
            }
        }

        public Brush InputBrush
        {
            get => _inputColor;
            set
            {
                _inputColor = value;
                NotifyOfPropertyChange(() => InputBrush);
            }
        }

        public Brush CaretBrush
        {
            get => _caretBrush;
            set
            {
                _caretBrush = value;
                NotifyOfPropertyChange(() => CaretBrush);
            }
        }

        public FontWeight DescriptionFontWeight
        {
            get => _descriptionFontWeight;
            set
            {
                _descriptionFontWeight = value;
                NotifyOfPropertyChange(() => DescriptionFontWeight);
            }
        }

        public Brush DescriptionBrush
        {
            get => _descriptionBrush;
            set
            {
                _descriptionBrush = value;
                NotifyOfPropertyChange(() => DescriptionBrush);
            }
        }

        public Brush SelectedDescriptionBrush
        {
            get => _selectedDescriptionBrush;
            set
            {
                _selectedDescriptionBrush = value;
                NotifyOfPropertyChange(() => SelectedDescriptionBrush);
            }
        }

        public FontWeight CaptionFontWeight
        {
            get => _captionFontWeight;
            set
            {
                _captionFontWeight = value;
                NotifyOfPropertyChange(() => CaptionFontWeight);
            }
        }

        public Brush CaptionBrush
        {
            get => _captionBrush;
            set
            {
                _captionBrush = value;
                NotifyOfPropertyChange(() => CaptionBrush);
            }
        }

        public Brush SelectedCaptionBrush
        {
            get => _selectedCaptionBrush;
            set
            {
                _selectedCaptionBrush = value;
                NotifyOfPropertyChange(() => SelectedCaptionBrush);
            }
        }

        public Brush BorderBrush
        {
            get => _borderBrush;
            set
            {
                _borderBrush = value;
                NotifyOfPropertyChange(() => BorderBrush);
            }
        }

        public double BorderThickness
        {
            get => _borderThickness;
            set
            {
                _borderThickness = value;
                NotifyOfPropertyChange(() => BorderThickness);
            }
        }

        public double CornerRadius
        {
            get => _cornerRadius;
            set
            {
                _cornerRadius = value;
                NotifyOfPropertyChange(() => CornerRadius);
            }
        }

        public Brush HighlightBrush
        {
            get => _highlightBrush;
            set
            {
                _highlightBrush = value;
                NotifyOfPropertyChange(() => HighlightBrush);
            }
        }

        public Brush SelectionBrush
        {
            get => _selectionBrush;
            set
            {
                _selectionBrush = value;
                NotifyOfPropertyChange(() => SelectionBrush);
            }
        }

        public double SelectionOpacity
        {
            get => _selectionOpacity;
            set
            {
                _selectionOpacity = value;
                NotifyOfPropertyChange(() => SelectionOpacity);
            }
        }
    }
}
