namespace Reginald.Core.Helpers
{
    using System;
    using System.Windows.Media.Imaging;

    public class BitmapImageHelper
    {
        public static BitmapImage FromUri(string uriString, int width = 128, int height = 128)
        {
            if (!Uri.TryCreate(uriString, UriKind.Absolute, out Uri result))
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
            icon.UriSource = result;

            // End initialization
            icon.EndInit();
            icon.Freeze();
            return icon;
        }
    }
}
