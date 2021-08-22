using Reginald.Core.Enums;
using System.Threading.Tasks;

namespace Reginald.Core.Utilities
{
    public class UtilityBase
    {
        public static async Task HandleUtilityAsync(Utility utility)
        {
            switch (utility)
            {
                case Utility.Recycle:
                    Task task = Task.Run(() =>
                    {
                        RecycleBin.Empty();
                    });
                    await task;
                    break;

                default:
                    break;
            }
        }
    }
}
