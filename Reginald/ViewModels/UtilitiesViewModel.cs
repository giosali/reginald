using Reginald.Core.DataModels;
using Reginald.Core.IO;

namespace Reginald.ViewModels
{
    public class UtilitiesViewModel : KeyphraseViewModelBase
    {
        public UtilitiesViewModel() : base(ApplicationPaths.UtilitiesJsonFilename)
        {
            Keyphrases.AddRange(UpdateKeyphrases<UtilityDataModel>(Filename));
        }
    }
}
