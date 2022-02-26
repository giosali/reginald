namespace Reginald.Data.Units
{
    using System;
    using System.Windows;
    using System.Windows.Media;
    using Reginald.Core.Helpers;

    public class Theme : Unit
    {
        private string _author;

        private bool _isEditable;

        private FontFamily _fontFamily;

        private Color _backgroundColor;

        private double _tintOpacity;

        private double _inputFontSize;

        private FontWeight _inputFontWeight;

        private Brush _inputBrush;

        private Brush _placeholderInputBrush;

        private Brush _caretBrush;

        private double _descriptionFontSize;

        private FontWeight _descriptionFontWeight;

        private Brush _descriptionBrush;

        private Brush _selectedDescriptionBrush;

        private double _captionFontSize;

        private FontWeight _captionFontWeight;

        private Brush _captionBrush;

        private Brush _selectedCaptionBrush;

        private Brush _borderBrush;

        private double _borderThickness;

        private double _cornerRadius;

        private Brush _highlightBrush;

        private Brush _selectionBrush;

        private double _selectionOpacity;

        private double _clipboardItemFontSize;

        private double _clipboardDisplayFontSize;

        public Theme(ThemeDataModel model)
        {
            Name = model.Name;
            Author = model.Author;
            if (Guid.TryParse(model.Guid, out Guid guid))
            {
                Guid = guid;
            }

            IsEditable = model.IsEditable;
            FontFamily = new(model.FontFamily);
            TintOpacity = model.TintOpacity;
            InputFontSize = model.InputFontSize;
            InputFontWeight = (FontWeight)new FontWeightConverter().ConvertFromString(model.InputFontWeight);
            DescriptionFontSize = model.DescriptionFontSize;
            DescriptionFontWeight = (FontWeight)new FontWeightConverter().ConvertFromString(model.DescriptionFontWeight);
            CaptionFontSize = model.CaptionFontSize;
            CaptionFontWeight = (FontWeight)new FontWeightConverter().ConvertFromString(model.CaptionFontWeight);
            BorderThickness = model.BorderThickness;
            CornerRadius = model.CornerRadius;
            SelectionOpacity = model.SelectionOpacity;
            ClipboardItemFontSize = model.ClipboardItemFontSize;
            ClipboardDisplayFontSize = model.ClipboardDisplayFontSize;

            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                BackgroundColor = ColorHelper.FromString(model.BackgroundColor);
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

        public string Author
        {
            get => _author;
            set
            {
                _author = value;
                NotifyOfPropertyChange(() => Author);
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

        public double InputFontSize
        {
            get => _inputFontSize;
            set
            {
                _inputFontSize = value;
                NotifyOfPropertyChange(() => InputFontSize);
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
            get => _inputBrush;
            set
            {
                _inputBrush = value;
                NotifyOfPropertyChange(() => InputBrush);
            }
        }

        public Brush PlaceholderInputBrush
        {
            get => _placeholderInputBrush;
            set
            {
                _placeholderInputBrush = value;
                NotifyOfPropertyChange(() => PlaceholderInputBrush);
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

        public double DescriptionFontSize
        {
            get => _descriptionFontSize;
            set
            {
                _descriptionFontSize = value;
                NotifyOfPropertyChange(() => DescriptionFontSize);
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

        public double CaptionFontSize
        {
            get => _captionFontSize;
            set
            {
                _captionFontSize = value;
                NotifyOfPropertyChange(() => CaptionFontSize);
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

        public double ClipboardItemFontSize
        {
            get => _clipboardItemFontSize;
            set
            {
                _clipboardItemFontSize = value;
                NotifyOfPropertyChange(() => ClipboardItemFontSize);
            }
        }

        public double ClipboardDisplayFontSize
        {
            get => _clipboardDisplayFontSize;
            set
            {
                _clipboardDisplayFontSize = value;
                NotifyOfPropertyChange(() => ClipboardDisplayFontSize);
            }
        }
    }
}
