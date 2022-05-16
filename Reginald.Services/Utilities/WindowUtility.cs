namespace Reginald.Services.Utilities
{
    using System;
    using System.Threading.Tasks;
    using static Reginald.Services.Utilities.NativeMethods;

    public static class WindowUtility
    {
        public static async Task WaitForDeactivationAsync()
        {
            while (true)
            {
                if (!IsWindowVisible(GetActiveWindow()))
                {
                    break;
                }

                await Task.Delay(10);
            }
        }

        public static IntPtr RegisterInstance(string name)
        {
            return CreateMutex(IntPtr.Zero, true, name);
        }

        public static void UnregisterInstance(IntPtr hMutex)
        {
            _ = ReleaseMutex(hMutex);
        }
    }
}
