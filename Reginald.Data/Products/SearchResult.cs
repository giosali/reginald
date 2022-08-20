namespace Reginald.Data.Products
{
    using System.Windows.Media;
    using Reginald.Data.Inputs;
    using Reginald.Services.Helpers;

    public class SearchResult : KeyboardInput
    {
        private string _caption;

        private string _description;

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

        public string IconPath { get; set; }

        public ImageSource IconSource { get; set; }

        public override void PressAltAndEnter(InputProcessingEventArgs e)
        {
            OnAltAndEnterKeysPressed(e);
        }

        public override void PressAlt(InputProcessingEventArgs e)
        {
            OnAltKeyPressed(e);
            if (e.IsAltKeyDown)
            {
                Description = e.Description;
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
            if (!e.IsAltKeyDown)
            {
                Description = e.Description;
            }
        }

        private void ProcessIcon(string icon)
        {
            if (!uint.TryParse(icon, out uint result))
            {
                IconPath = icon;
                return;
            }

            IconSource = BitmapSourceHelper.GetStockIcon(result);
        }
    }
}
