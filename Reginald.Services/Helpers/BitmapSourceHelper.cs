﻿namespace Reginald.Services.Helpers
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Windows;
    using System.Windows.Interop;
    using System.Windows.Media.Imaging;
    using static Reginald.Services.Helpers.NativeMethods;

    public static class BitmapSourceHelper
    {
        private const int MaxPath = 260;

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

            if (!uri.IsFile)
            {
                return null;
            }

            // SECREVIEW : The File.Exists() below will do the demand for the FileIOPermission
            //             for us. So, we do not need an additional demand anymore.
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException(filePath);
            }

            StringBuilder sb = new(MaxPath);
            sb.Append(filePath);
            IntPtr hIcon = ExtractAssociatedIcon(NullHandleRef.Handle, sb, out _);
            if (hIcon == IntPtr.Zero)
            {
                return null;
            }

            BitmapSource bitmapSource = Imaging.CreateBitmapSourceFromHIcon(hIcon, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            DestroyIcon(hIcon);
            return bitmapSource;
        }

        public static BitmapSource GetStockIcon(uint stockIconId)
        {
            SHSTOCKICONINFO sii = new();
            sii.cbSize = (uint)Marshal.SizeOf(sii);
            SHSTOCKICONID siid = (SHSTOCKICONID)stockIconId;
            int result = SHGetStockIconInfo(siid, SHGSI.SHGSI_ICON | SHGSI.SHGSI_LARGEICON, ref sii);
            if (result != 0)
            {
                _ = SHGetStockIconInfo(siid, SHGSI.SHGSI_ICON | SHGSI.SHGSI_SMALLICON, ref sii);
            }

            BitmapSource bitmapSource = Imaging.CreateBitmapSourceFromHIcon(sii.hIcon, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            DestroyIcon(sii.hIcon);
            return bitmapSource;
        }
    }
}
