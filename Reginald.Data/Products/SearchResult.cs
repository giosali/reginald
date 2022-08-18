namespace Reginald.Data.Products
{
    using System;
    using System.Windows.Media;
    using Reginald.Core.Extensions;
    using Reginald.Data.Inputs;
    using Reginald.Services.Helpers;

    public class SearchResult : KeyboardInput
    {
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

        public string Caption { get; set; }

        public string Description { get; set; }

        public string IconPath { get; set; }

        public ImageSource IconSource { get; set; }

        public override void PressEnter()
        {
            OnEnterKeyPressed(new EventArgs());
        }
    }
}
