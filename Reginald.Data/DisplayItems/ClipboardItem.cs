namespace Reginald.Data.DisplayItems
{
    using System;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using Microsoft.Data.Sqlite;
    using Newtonsoft.Json;
    using Reginald.Core.Helpers;
    using Reginald.Services.Input;
    using Reginald.Services.Utilities;

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

        public ClipboardItem(SqliteDataReader reader)
        {
            Guid = Guid.NewGuid();
            Icon = BitmapImageHelper.FromUri((string)reader["icon"]);
            Description = (string)reader["text"];
            DateTime = DateTime.Parse((string)reader["datetime"]);
            ClipboardItemType = (ClipboardItemType)(long)reader["type"];
        }

        [JsonProperty("dateTime")]
        public DateTime DateTime { get; set; }

        [JsonProperty("clipboardItemType")]
        public ClipboardItemType ClipboardItemType { get; set; }

        public ImageSource Image { get; set; }

        public Brush HexBrush { get; set; }

        public override async void EnterKeyDown()
        {
            switch (ClipboardItemType)
            {
                case ClipboardItemType.Text:
                    if (ClipboardHelper.TrySetText(Description))
                    {
                        // Waits for clipboard window to deactivate.
                        await WindowUtility.WaitForDeactivationAsync();

                        KeyboardInputInjector.Paste();
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
