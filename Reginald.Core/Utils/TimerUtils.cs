using System.Threading;
using System.Threading.Tasks;

namespace Reginald.Core.Utils
{
    public static class TimerUtils
    {
        /// <summary>
        /// Executes a task every ten seconds.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns></returns>
        public static async Task DoEveryTenSecondsAsync(CancellationToken cancellationToken)
        {
            int millisecondsDelay = 10000;
            while (true)
            {
                await Task.Delay(millisecondsDelay, cancellationToken);
            }
        }
    }
}
