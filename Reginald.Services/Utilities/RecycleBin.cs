namespace Reginald.Services.Utilities
{
    using System;
    using System.Runtime.InteropServices;
    using static Reginald.Services.Utilities.NativeMethods;

    public static class RecycleBin
    {
        private const string CRootDrive = @"C:\";

        /// <summary>
        /// Specifies various values of confirmation for emptying the Recycle Bin.
        /// </summary>
        [Flags]
        private enum RecycleFlag : uint
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
        /// Permanently deletes all items in the Recycle Bin of the C: drive if it contains any.
        /// </summary>
        public static void Empty()
        {
            if (GetItemCount() > 0)
            {
                _ = SHEmptyRecycleBin(IntPtr.Zero, CRootDrive, (uint)(RecycleFlag.SHERB_NOCONFIRMATION | RecycleFlag.SHERB_NOPROGRESSUI | RecycleFlag.SHERB_NOSOUND));
            }
        }

        public static long GetItemCount()
        {
            SHQUERYRBINFO sqrbi = new();
            sqrbi.cbSize = Marshal.SizeOf(typeof(SHQUERYRBINFO));
            HRESULT hResult = (HRESULT)SHQueryRecycleBin(CRootDrive, ref sqrbi);
            return hResult == HRESULT.S_OK ? sqrbi.i64NumItems : 0;
        }
    }
}
