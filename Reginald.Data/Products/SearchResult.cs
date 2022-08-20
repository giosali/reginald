namespace Reginald.Data.Products
{
    using System.Windows.Media;
    using Reginald.Data.Inputs;
    using Reginald.Services.Helpers;

    public class SearchResult : KeyboardInput
    {
        private string _caption;

        private string _description;

        private string _iconPath;

        private ImageSource _iconSource;

        public SearchResult(string caption, string icon)
        {
            Caption = caption;
            ProcessIcon(icon);
        }

        public SearchResult(string caption, string description, string icon)
        {
            Caption = caption;
            Description = description;
            ProcessIcon(icon);
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

        public string IconPath
        {
            get => _iconPath;
            set
            {
                _iconPath = value;
                NotifyOfPropertyChange(() => IconPath);
            }
        }

        public ImageSource IconSource
        {
            get => _iconSource;
            set
            {
                _iconSource = value;
                NotifyOfPropertyChange(() => IconSource);
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
                ProcessIcon(e.Icon);
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
                ProcessIcon(e.Icon);
            }
        }

        private void ProcessIcon(string icon)
        {
            if (!uint.TryParse(icon, out uint result))
            {
                IconPath = icon;
                IconSource = null;
                return;
            }

            IconSource = BitmapSourceHelper.GetStockIcon(result);
            IconPath = null;
        }
    }
}
