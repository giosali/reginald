using Reginald.Core.AbstractProducts;
using Reginald.Core.DataModels;
using Reginald.Core.Helpers;
using System;
using System.Windows.Media;

namespace Reginald.Core.Products
{
    public class Theme : Unit
    {
        private string _author;
        public string Author
        {
            get => _author;
            set
            {
                _author = value;
                NotifyOfPropertyChange(() => Author);
            }
        }

        private bool _isEditable;
        public bool IsEditable
        {
            get => _isEditable;
            set
            {
                _isEditable = value;
                NotifyOfPropertyChange(() => IsEditable);
            }
        }

        // Main

        private Color _backgroundColor;
        public Color BackgroundColor
        {
            get => _backgroundColor;
            set
            {
                _backgroundColor = value;
                NotifyOfPropertyChange(() => _backgroundColor);
            }
        }

        private double _tintOpacity;
        public double TintOpacity
        {
            get => _tintOpacity;
            set
            {
                _tintOpacity = value;
                NotifyOfPropertyChange(() => TintOpacity);
            }
        }

        private Brush _inputColor;
        public Brush InputBrush
        {
            get => _inputColor;
            set
            {
                _inputColor = value;
                NotifyOfPropertyChange(() => InputBrush);
            }
        }

        private Brush _caretBrush;
        public Brush CaretBrush
        {
            get => _caretBrush;
            set
            {
                _caretBrush = value;
                NotifyOfPropertyChange(() => CaretBrush);
            }
        }

        private Brush _descriptionBrush;
        public Brush DescriptionBrush
        {
            get => _descriptionBrush;
            set
            {
                _descriptionBrush = value;
                NotifyOfPropertyChange(() => DescriptionBrush);
            }
        }

        private Brush _captionBrush;
        public Brush CaptionBrush
        {
            get => _captionBrush;
            set
            {
                _captionBrush = value;
                NotifyOfPropertyChange(() => CaptionBrush);
            }
        }

        private Brush _borderBrush;
        public Brush BorderBrush
        {
            get => _borderBrush;
            set
            {
                _borderBrush = value;
                NotifyOfPropertyChange(() => BorderBrush);
            }
        }

        private Brush _highlightBrush;
        public Brush HighlightBrush
        {
            get => _highlightBrush;
            set
            {
                _highlightBrush = value;
                NotifyOfPropertyChange(() => HighlightBrush);
            }
        }

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
    }
}
