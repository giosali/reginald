namespace Reginald.Models.Products
{
    using System.Windows.Media.Imaging;
    using Reginald.Models.Drawing;
    using Reginald.Models.Inputs;

    internal sealed class SearchResult : KeyboardInput
    {
        private string _caption;

        private string _description;

        private Icon _icon;

        public SearchResult(string caption, string iconPath, int id)
        {
            Id = id;
            Caption = caption;
            Icon = new Icon(iconPath);
        }

        public SearchResult(string caption, string iconPath, string description, int id)
        {
            Id = id;
            Caption = caption;
            Icon = new Icon(iconPath);
            Description = description;
        }

        public SearchResult(string caption, BitmapSource bitmapSource, string description, int id)
        {
            Id = id;
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

        public string KeyboardShortcut { get; set; }

        private int Id { get; set; }

        public static bool operator ==(SearchResult a, SearchResult b)
        {
            return a is not null && b is not null && a.Id == b.Id && a.Description == b.Description;
        }

        public static bool operator !=(SearchResult a, SearchResult b)
        {
            return a is not null && b is not null && a.Id != b.Id && a.Description != b.Description;
        }

        public override bool Equals(object obj)
        {
            return obj is SearchResult result && Id == result.Id && Description == result.Description;
        }

        public override int GetHashCode()
        {
            return Id ^ Description.GetHashCode();
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
