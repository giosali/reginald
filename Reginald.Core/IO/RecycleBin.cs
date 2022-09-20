namespace Reginald.Core.IO
{
    using System;
    using System.Runtime.InteropServices;
    using static Reginald.Core.IO.NativeMethods;

    public static class RecycleBin
    {
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
            SHQUERYRBINFO sqrbi = new();
            sqrbi.cbSize = Marshal.SizeOf(typeof(SHQUERYRBINFO));
            if (SHQueryRecycleBin(@"C:\", ref sqrbi) != 0 || sqrbi.i64NumItems <= 0)
            {
                return;
            }

            _ = SHEmptyRecycleBin(IntPtr.Zero, @"C:\", (uint)(RecycleFlag.SHERB_NOCONFIRMATION | RecycleFlag.SHERB_NOPROGRESSUI | RecycleFlag.SHERB_NOSOUND));
        }
    }
}
