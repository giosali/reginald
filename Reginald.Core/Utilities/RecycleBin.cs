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
        public static readonly Guid RecycleBinGuid = new("{b7534046-3ecb-4c18-be4e-64cd4cb7d6ac}");

        [DllImport("Shell32.dll", CharSet = CharSet.Unicode)]
        private static extern uint SHEmptyRecycleBin(IntPtr hwnd, string pszRootPath, RecycleFlag dwFlags);

        public static void Empty()
        {
            IKnownFolder folder = Applications.GetKnownFolder(RecycleBinGuid);
            if (folder.Any())
            {
                _ = SHEmptyRecycleBin(IntPtr.Zero, null, RecycleFlag.SHERB_NOCONFIRMATION | RecycleFlag.SHERB_NOPROGRESSUI | RecycleFlag.SHERB_NOSOUND);
            }
        }
    }
}
