using System;
using System.Runtime.InteropServices;
using static Reginald.Core.Enums.RecycleBinEnums;

namespace Reginald.Core.Utilities
{
    public class RecycleBin
    {
        [DllImport("Shell32.dll", CharSet = CharSet.Unicode)]
        static extern uint SHEmptyRecycleBin(IntPtr hwnd, string pszRootPath, RecycleFlag dwFlags);

        public static void Empty()
        {
            _ = SHEmptyRecycleBin(IntPtr.Zero, null, RecycleFlag.SHERB_NOCONFIRMATION | RecycleFlag.SHERB_NOPROGRESSUI | RecycleFlag.SHERB_NOSOUND);
        }
    }
}
