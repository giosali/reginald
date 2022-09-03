namespace Reginald.Models.Products
{
    using System.Windows.Media.Imaging;
    using Reginald.Models.Drawing;
    using Reginald.Models.Inputs;

    public class SearchResult : KeyboardInput
    {
        private string _caption;

        private string _description;

        private Icon _icon;

        public SearchResult(string caption, string iconPath)
        {
            Caption = caption;
            Icon = new Icon(iconPath);
        }

        public SearchResult(string caption, string iconPath, string description)
        {
            Caption = caption;
            Icon = new Icon(iconPath);
            Description = description;
        }

        public SearchResult(string caption, BitmapSource bitmapSource, string description)
        {
            Caption = caption;
            Icon = new Icon(bitmapSource);
            Description = description;
        }

        public string Caption
        {
            get => _caption;
            set
            {
                _caption = value;
                NotifyOfPropertyChange(() => Caption);
            }
        }

        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                NotifyOfPropertyChange(() => Description);
            }
        }

        public Icon Icon
        {
            get => _icon;
            set
            {
                _icon = value;
                NotifyOfPropertyChange(() => Icon);
            }
        }

        public override void PressAlt(InputProcessingEventArgs e)
        {
            OnAltKeyPressed(e);
        }

        public override void PressAltAndEnter(InputProcessingEventArgs e)
        {
            OnAltAndEnterKeysPressed(e);
        }

        public override void PressEnter(InputProcessingEventArgs e)
        {
            OnEnterKeyPressed(e);
        }

        public override void PressTab(InputProcessingEventArgs e)
        {
            OnTabKeyPressed(e);
        }

        public override void ReleaseAlt(InputProcessingEventArgs e)
        {
            OnAltKeyReleased(e);
        }
    }
}
