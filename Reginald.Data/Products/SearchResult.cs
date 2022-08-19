namespace Reginald.Data.Products
{
    using System.Windows.Media;
    using Reginald.Core.Extensions;
    using Reginald.Data.Inputs;
    using Reginald.Services.Helpers;

    public class SearchResult : KeyboardInput
    {
        private string _caption;

        private string _description;

        public SearchResult(string caption, string description, string icon)
        {
            Caption = caption;
            Description = description;

            if (!icon.Contains(".dll"))
            {
                IconPath = icon;
                return;
            }

            (string dll, _, string index) = icon.RPartition('.');
            if (!int.TryParse(index, out int iconIndex))
            {
                IconPath = icon;
                return;
            }

            IconSource = BitmapSourceHelper.ExtractFromFile(dll, iconIndex);
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

        public override void ReleaseAlt(InputProcessingEventArgs e)
        {
            OnAltKeyReleased(e);
            if (!e.IsAltKeyDown)
            {
                Description = e.Description;
            }
        }
    }
}
