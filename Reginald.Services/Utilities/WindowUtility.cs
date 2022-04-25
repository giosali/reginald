namespace Reginald.Services.Utilities
{
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
    }
}
