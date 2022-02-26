namespace Reginald.ViewModels
{
    using Reginald.Core.IO;
    using Reginald.Data.Keyphrases;

    public class UtilitiesViewModel : KeyphraseViewModelBase
    {
        public UtilitiesViewModel()
            : base(ApplicationPaths.UtilitiesJsonFilename, true)
        {
            Keyphrases.AddRange(KeyphraseHelper.ToKeyphrases(UpdateData<UtilityKeyphraseDataModel>(FilePath, IsResource)));
        }
    }
}
