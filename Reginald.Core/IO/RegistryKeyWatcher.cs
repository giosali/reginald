namespace Reginald.Core.IO
{
    using System;
    using System.Threading;
    using static Reginald.Core.IO.NativeMethods;

    /// <summary>
    /// An application can use handles to these keys as entry points to the registry.
    /// </summary>
    public enum RegistryHive : uint
    {
        /// <summary>
        /// The HKEY_CURRENT_USER subtree contains the user profile for the user who is currently logged on to the computer. The user profile includes environment variables, personal program groups, desktop settings, network connections, printers, and application preferences. The data in the user profile is similar to the data stored in the Win.ini file in Windows 3.x.
        /// </summary>
        CurrentUser = 0x80000001,

        /// <summary>
        /// The HKEY_LOCAL_MACHINE subtree contains information about the local computer system, including hardware and operating system data, such as bus type, system memory, device drivers, and startup control parameters.
        /// </summary>
        LocalMachine = 0x80000002,
    }

    public sealed class RegistryKeyWatcher
    {
        private const uint INFINITE = 0xFFFFFFFF;

        private readonly Thread _thread;

        public RegistryKeyWatcher(RegistryHive registryHive, string registrySubKey)
        {
            RegistryKey = new((uint)registryHive);
            RegistrySubKey = registrySubKey;
            _thread = new(WatchRegistryKey);
            _thread.IsBackground = true;
        }

        public event EventHandler<EventArgs> RegistryKeyChanged;

        [Flags]
        private enum WaitResult : uint
        {
            /// <summary>
            /// The function has failed. To get extended error information, call GetLastError.
            /// </summary>
            WAIT_FAILED = 0xFFFFFFFF,
        }

        private string RegistrySubKey { get; set; }

        private IntPtr RegistryKey { get; set; }

        public void Start()
        {
            _thread.Start();
        }

        private void WatchRegistryKey()
        {
            IntPtr hEvent = CreateEvent(IntPtr.Zero, true, false, "RegistryKeyWatcherEvent");
            if (hEvent == IntPtr.Zero)
            {
                return;
            }

            if (RegOpenKeyEx(RegistryKey, RegistrySubKey, 0, KeyAccessRight.KEY_NOTIFY, out IntPtr hKey) != 0)
            {
                return;
            }

            while (true)
            {
                switch (RegNotifyChangeKeyValue(hKey, false, REG_NOTIFY_CHANGE.NAME, hEvent, true))
                {
                    case 0:
                        if (WaitForSingleObject(hEvent, INFINITE) == (uint)WaitResult.WAIT_FAILED)
                        {
                            return;
                        }

                        RegistryKeyChanged?.Invoke(this, new EventArgs());
                        break;
                    default:
                        _ = RegCloseKey(hKey);
                        CloseHandle(hEvent);
                        return;
                }
            }
        }
    }
}
