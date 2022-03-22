namespace Reginald.Data.DisplayItems
{
    using System;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using Newtonsoft.Json;
    using Reginald.Core.Helpers;
    using Reginald.Services.Input;

    public enum ClipboardItemType
    {
        /// <summary>
        /// The data is a text data format.
        /// </summary>
        Text,

        /// <summary>
        /// The data is a Bitmap data format.
        /// </summary>
        Image,
    }

    public class ClipboardItem : DisplayItem
    {
        private const string IconPathFormat = "pack://application:,,,/Reginald;component/Images/Results/{0}";

        public ClipboardItem(ClipboardItemDataModel model)
        {
            if (Guid.TryParse(model.Guid, out Guid guid))
            {
                Guid = guid;
            }

            Icon = BitmapImageHelper.FromUri(model.Icon);
            Description = model.Description;
            DateTime = DateTime.Parse(model.DateTime);
            ClipboardItemType = (ClipboardItemType)model.ClipboardItemType;
        }

        public ClipboardItem(string text)
        {
            Guid = Guid.NewGuid();
            string iconPath = string.Format(IconPathFormat, "text.png");
            Icon = BitmapImageHelper.FromUri(iconPath);
            Description = text;
            DateTime = DateTime.Now;
            ClipboardItemType = ClipboardItemType.Text;
            ParseText(text);
        }

        public ClipboardItem(BitmapSource bitmapSource)
        {
            Guid = Guid.NewGuid();
            string iconPath = string.Format(IconPathFormat, "image.png");
            Icon = BitmapImageHelper.FromUri(iconPath);
            Description = $"{bitmapSource.Width}x{bitmapSource.Height}";
            DateTime = DateTime.Now;
            ClipboardItemType = ClipboardItemType.Image;
            Image = bitmapSource;
        }

        [JsonProperty("dateTime")]
        public DateTime DateTime { get; set; }

        [JsonProperty("clipboardItemType")]
        public ClipboardItemType ClipboardItemType { get; set; }

        public ImageSource Image { get; set; }

        public Brush HexBrush { get; set; }

        public override void EnterKeyDown()
        {
            switch (ClipboardItemType)
            {
                case ClipboardItemType.Text:
                    if (ClipboardHelper.TrySetText(Description))
                    {
                        KeyboardInputInjector.PasteTo();
                    }

                    break;

                case ClipboardItemType.Image:
                    break;
            }
        }

        public override void AltKeyDown()
        {
            IsAltKeyDown = true;
        }

        public override void AltKeyUp()
        {
            IsAltKeyDown = false;
        }

        private void ParseText(string text)
        {
            if (BrushHelper.TryFromString(text, out Brush brush))
            {
                HexBrush = brush;
            }
        }
    }
}
