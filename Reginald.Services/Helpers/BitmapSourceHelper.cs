namespace Reginald.Services.Helpers
{
    using System;
    using System.IO;
    using System.Text;
    using System.Windows;
    using System.Windows.Interop;
    using System.Windows.Media.Imaging;
    using static Reginald.Services.Helpers.NativeMethods;

    public static class BitmapSourceHelper
    {
        public static BitmapSource ExtractAssociatedBitmapSource(string filePath)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            Uri uri;
            try
            {
                uri = new Uri(filePath);
            }
            catch (UriFormatException)
            {
                // Gets its full path as a file if it's a relative pathname.
                // If the path exists, the caller must have FileIOPermissionAccess.PathDiscovery permission.
                // Note that unlike most members of the Path class, this method accesses the file system.
                filePath = Path.GetFullPath(filePath);
                uri = new Uri(filePath);
            }

            if (uri.IsUnc)
            {
                throw new ArgumentException($"{nameof(filePath)} is a UNC path");
            }

            if (uri.IsFile)
            {
                // SECREVIEW : The File.Exists() below will do the demand for the FileIOPermission
                //             for us. So, we do not need an additional demand anymore.
                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException(filePath);
                }

                StringBuilder sb = new(MAX_PATH);
                sb.Append(filePath);
                IntPtr hIcon = ExtractAssociatedIcon(NullHandleRef.Handle, sb, out _);
                if (hIcon != IntPtr.Zero)
                {
                    BitmapSource bitmapSource = Imaging.CreateBitmapSourceFromHIcon(hIcon, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                    DestroyIcon(hIcon);
                    return bitmapSource;
                }
            }

            return null;
        }
    }
}
