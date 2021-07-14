using System;
using System.Windows.Media.Imaging;

namespace Reginald.Core.Helpers
{
    public class BitmapImageHelper
    {
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
    }
}
