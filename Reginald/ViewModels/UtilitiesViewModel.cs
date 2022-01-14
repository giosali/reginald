namespace Reginald.ViewModels
{
    using Reginald.Core.DataModels;
    using Reginald.Core.IO;

    public class UtilitiesViewModel : KeyphraseViewModelBase
    {
        public UtilitiesViewModel()
            : base(ApplicationPaths.UtilitiesJsonFilename)
        {
            Keyphrases.AddRange(UpdateKeyphrases<UtilityDataModel>(Filename));
        }
    }
}
