namespace Reginald.Services.Watchers
{
    using System;
    using System.Runtime.InteropServices;
    using System.Security;

    internal class NativeMethods
    {
        [Flags]
        internal enum KeyAccessRight : uint
        {
            /// <summary>
            /// Required to request change notifications for a registry key or for subkeys of a registry key.
            /// </summary>
            KEY_NOTIFY = 0x0010,
        }

        [Flags]
        internal enum REG_NOTIFY_CHANGE : uint
        {
            /// <summary>
            /// Notify the caller if a subkey is added or deleted.
            /// </summary>
            NAME = 0x00000001,
        }

        [DllImport("advapi32.dll", SetLastError = true)]
        internal static extern int RegCloseKey(IntPtr hKey);

        [DllImport("advapi32.dll")]
        internal static extern int RegNotifyChangeKeyValue(IntPtr hKey, bool watchSubtree, REG_NOTIFY_CHANGE notifyFilter, IntPtr hEvent, bool asynchronous);

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode)]
        internal static extern int RegOpenKeyEx(IntPtr hKey, string subKey, int ulOptions, KeyAccessRight samDesired, out IntPtr hkResult);

        [DllImport("kernel32.dll", SetLastError = true)]
        [SuppressUnmanagedCodeSecurity]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        internal static extern IntPtr CreateEvent(IntPtr lpEventAttributes, bool bManualReset, bool bInitialState, string lpName);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern uint WaitForSingleObject(IntPtr hHandle, uint dwMilliseconds);
    }
}
