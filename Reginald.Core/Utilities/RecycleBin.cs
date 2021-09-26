using Shell32;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using static Reginald.Core.Enums.RecycleBinEnums;

namespace Reginald.Core.Utilities
{
    public class RecycleBin
    {
        [DllImport("Shell32.dll", CharSet = CharSet.Unicode)]
        static extern uint SHEmptyRecycleBin(IntPtr hwnd, string pszRootPath, RecycleFlag dwFlags);

        public static void Empty()
        {
            bool result = true;
            Application.Current.Dispatcher.Invoke(() =>
            {
                result = IsRecycleBinEmpty();
            });
            if (!result)
            {
                _ = SHEmptyRecycleBin(IntPtr.Zero, null, RecycleFlag.SHERB_NOCONFIRMATION | RecycleFlag.SHERB_NOPROGRESSUI | RecycleFlag.SHERB_NOSOUND);
            }
        }

        public static bool IsRecycleBinEmpty()
        {
            Shell shell = new();
            Folder recycleBin = shell.NameSpace(10);
            return recycleBin.Items().Count == 0;
        }
    }
}
