using System;
using System.IO;
using System.Windows.Media.Imaging;

namespace Reginald.Core.Helpers
{
    public class BitmapImageHelper
    {
        /// <summary>
        /// Returns a processed BitmapImage from a URI string.
        /// </summary>
        /// <param name="uriString">The URI string for the BitmapImage's UriSource.</param>
        /// <returns>A BitmapImage based on the URI string.</returns>
        public static BitmapImage MakeFromUri(string uriString)
        {
            BitmapImage icon = new();
            icon.BeginInit();
            icon.UriSource = new Uri(uriString);
            icon.CacheOption = BitmapCacheOption.OnLoad;
            icon.DecodePixelWidth = 75;
            icon.DecodePixelHeight = 75;
            icon.EndInit();
            icon.Freeze();
            return icon;
        }

        public static BitmapImage GetIcon(string path, string name)
        {
            string iconPath = Path.Combine(path, name + ".png");
            if (!File.Exists(iconPath))
            {
                iconPath = "pack://application:,,,/Reginald;component/Images/help-light.png";
            }

            BitmapImage icon = MakeFromUri(iconPath);
            return icon;
        }
    }
}
