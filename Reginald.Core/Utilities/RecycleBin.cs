using Microsoft.WindowsAPICodePack.Shell;
using Reginald.Core.Base;
using Reginald.Core.Enums;
using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace Reginald.Core.Utilities
{
    public class RecycleBin
    {
        [DllImport("Shell32.dll", CharSet = CharSet.Unicode)]
        private static extern uint SHEmptyRecycleBin(IntPtr hwnd, string pszRootPath, RecycleFlag dwFlags);

        public static void Empty()
        {
            IKnownFolder folder = Applications.GetKnownFolder(Constants.RecycleBinGuid);
            if (folder.Any())
            {
                _ = SHEmptyRecycleBin(IntPtr.Zero, null, RecycleFlag.SHERB_NOCONFIRMATION | RecycleFlag.SHERB_NOPROGRESSUI | RecycleFlag.SHERB_NOSOUND);
            }
        }
    }
}
