using Reginald.Core.Enums;

namespace Reginald.Core.Utilities
{
    public class UtilityBase
    {
        public static void HandleUtility(Utility utility)
        {
            switch (utility)
            {
                case Utility.Recycle:
                    RecycleBin.Empty();
                    break;

                default:
                    break;
            }
        }
    }
}
