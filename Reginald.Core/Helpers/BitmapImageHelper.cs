namespace Reginald.Core.Helpers
{
    using System;
    using System.Windows.Media.Imaging;

    public class BitmapImageHelper
    {
        public static BitmapImage FromUri(string uri, int width = 128, int height = 128)
        {
            if (uri is null)
            {
                return null;
            }

            // Define a BitmapImage
            BitmapImage icon = new();

            // Begin initialization
            icon.BeginInit();

            // Assign properties
            icon.CacheOption = BitmapCacheOption.OnLoad;
            icon.DecodePixelWidth = width;
            icon.DecodePixelHeight = height;
            try
            {
                icon.UriSource = new Uri(uri);
            }
            catch (UriFormatException)
            {
                return null;
            }

            // End initialization
            icon.EndInit();
            icon.Freeze();
            return icon;
        }
    }
}
