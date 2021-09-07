using Reginald.Core.IO;

namespace Reginald.ViewModels
{
    public class UserKeywordViewModel : CustomKeywordViewModel
    {
        public UserKeywordViewModel() : base(ApplicationPaths.XmlUserKeywordFilename)
        {

        }
    }
}
