namespace Reginald.Core.Services
{
    using System;
    using static Reginald.Core.Services.NativeMethods;

    public static class ApplicationService
    {
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
