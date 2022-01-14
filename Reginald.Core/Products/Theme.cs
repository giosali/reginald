namespace Reginald.Core.Products
{
    using System;
    using System.Windows.Media;
    using Reginald.Core.AbstractProducts;
    using Reginald.Core.DataModels;
    using Reginald.Core.Helpers;

    public class Theme : Unit
    {
        private string _author;

        private bool _isEditable;

        private Color _backgroundColor;

        private double _tintOpacity;

        private Brush _inputColor;

        private Brush _caretBrush;

        private Brush _descriptionBrush;

        private Brush _captionBrush;

        private Brush _borderBrush;

        private Brush _highlightBrush;

        public Theme(ThemeDataModel model)
        {
            Name = model.Name;
            Author = model.Author;
            if (Guid.TryParse(model.Guid, out Guid guid))
            {
                Guid = guid;
            }

            IsEditable = model.IsEditable;

            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                BackgroundColor = ColorHelper.FromString(model.BackgroundColor);
                TintOpacity = model.TintOpacity;
                InputBrush = BrushHelper.SolidColorBrushFromString(model.InputBrush);
                CaretBrush = BrushHelper.SolidColorBrushFromString(model.CaretBrush);
                DescriptionBrush = BrushHelper.SolidColorBrushFromString(model.DescriptionBrush);
                CaptionBrush = BrushHelper.SolidColorBrushFromString(model.CaptionBrush);
                BorderBrush = BrushHelper.SolidColorBrushFromString(model.BorderBrush);
                HighlightBrush = BrushHelper.SolidColorBrushFromString(model.HighlightBrush);
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

        public Brush DescriptionBrush
        {
            get => _descriptionBrush;
            set
            {
                _descriptionBrush = value;
                NotifyOfPropertyChange(() => DescriptionBrush);
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

        public Brush BorderBrush
        {
            get => _borderBrush;
            set
            {
                _borderBrush = value;
                NotifyOfPropertyChange(() => BorderBrush);
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
    }
}
