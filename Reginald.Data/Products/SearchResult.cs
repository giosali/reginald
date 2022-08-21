namespace Reginald.Data.Products
{
    using System.Windows.Media;
    using Reginald.Data.Inputs;
    using Reginald.Services.Helpers;

    public class SearchResult : KeyboardInput
    {
        private string _caption;

        private string _description;

        private string _icon;

        public SearchResult(string caption, string icon)
        {
            Caption = caption;
            Icon = icon;
        }

        public SearchResult(string caption, string description, string icon)
        {
            Caption = caption;
            Description = description;
            Icon = icon;
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

        public string Icon
        {
            get => _icon;
            set
            {
                _icon = value;
                NotifyOfPropertyChange(() => Icon);
            }
        }

        public override void PressAltAndEnter(InputProcessingEventArgs e)
        {
            OnAltAndEnterKeysPressed(e);
        }

        public override void PressAlt(InputProcessingEventArgs e)
        {
            OnAltKeyPressed(e);
            if (!string.IsNullOrEmpty(e.Caption))
            {
                Caption = e.Caption;
            }

            if (!string.IsNullOrEmpty(e.Description))
            {
                Description = e.Description;
            }

            if (!string.IsNullOrEmpty(e.Icon))
            {
                Icon = e.Icon;
            }
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
            if (!string.IsNullOrEmpty(e.Caption))
            {
                Caption = e.Caption;
            }

            if (!string.IsNullOrEmpty(e.Description))
            {
                Description = e.Description;
            }

            if (!string.IsNullOrEmpty(e.Icon))
            {
                Icon = e.Icon;
            }
        }
    }
}
