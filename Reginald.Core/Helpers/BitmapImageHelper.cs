namespace Reginald.Core.Helpers
{
    using System;
    using System.Windows.Media.Imaging;

    public class BitmapImageHelper
    {
        public static BitmapImage FromUri(string uri, int width = 128, int height = 128)
        {
            try
            {
                if (uri is null)
                {
                    throw new ArgumentNullException(uri);
                }

                // Define a BitmapImage
                BitmapImage icon = new();

                // Begin initialization
                icon.BeginInit();

                // Assign properties
                icon.CacheOption = BitmapCacheOption.OnLoad;
                icon.DecodePixelWidth = width;
                icon.DecodePixelHeight = height;
                icon.UriSource = new Uri(uri);

                // End initialization
                icon.EndInit();
                icon.Freeze();
                return icon;
            }
            catch (Exception ex)
            {
                if (ex is UriFormatException or ArgumentNullException)
                {
                    return null;
                }
                else
                {
                    throw;
                }
            }
        }
    }
}
