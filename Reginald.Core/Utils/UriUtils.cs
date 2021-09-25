using System;
using System.Diagnostics;
using System.Windows;

namespace Reginald.Core.Utils
{
    public static class UriUtils
    {
        public static void GoTo(string uri)
        {
            try
            {
                ProcessStartInfo startInfo = new()
                {
                    FileName = uri,
                    UseShellExecute = true
                };
                _ = Process.Start(startInfo);
            }
            catch (System.ComponentModel.Win32Exception ex)
            {
                if (ex.ErrorCode == -2147467259)
                {
                    _ = MessageBox.Show(ex.Message);
                }
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.Message);
            }
        }
    }
}
