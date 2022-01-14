namespace Reginald.Core.Utilities
{
    using System;
    using System.Linq;
    using System.Runtime.InteropServices;
    using Microsoft.WindowsAPICodePack.Shell;
    using Reginald.Core.IO;

    /// <summary>
    /// Represents the Recycle Bin.
    /// </summary>
    public class RecycleBin
    {
        /// <summary>
        /// The GUID that identifies the Known Folder for the .Recycle Bin.
        /// </summary>
        public static readonly Guid RecycleBinFolderGuid = new("{b7534046-3ecb-4c18-be4e-64cd4cb7d6ac}");

        /// <summary>
        /// Specifies various values of confirmation for emptying the Recycle Bin.
        /// </summary>
        public enum RecycleFlag : int
        {
            /// <summary>
            /// No dialog box confirming the deletion of the objects will be displayed.
            /// </summary>
            SHERB_NOCONFIRMATION = 0x00000001,

            /// <summary>
            /// No dialog box indicating the progress will be displayed.
            /// </summary>
            SHERB_NOPROGRESSUI = 0x00000002,

            /// <summary>
            /// No sound will be played when the operation is complete.
            /// </summary>
            SHERB_NOSOUND = 0x00000004,
        }

        /// <summary>
        /// Permanently deletes all files in the Recycle Bin if it contains any.
        /// </summary>
        public static void Empty()
        {
            IKnownFolder folder = WindowsShell.GetKnownFolder(RecycleBinFolderGuid);
            if (folder.Any())
            {
                _ = SHEmptyRecycleBin(IntPtr.Zero, null, RecycleFlag.SHERB_NOCONFIRMATION | RecycleFlag.SHERB_NOPROGRESSUI | RecycleFlag.SHERB_NOSOUND);
            }
        }

        [DllImport("Shell32.dll", CharSet = CharSet.Unicode)]
        private static extern uint SHEmptyRecycleBin(IntPtr hwnd, string pszRootPath, RecycleFlag dwFlags);
    }
}
