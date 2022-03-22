namespace Reginald.Data.Keyphrases
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class KeyphraseFactory
    {
        public static Keyphrase[] CreateKeyphrases(IEnumerable<IKeyphraseDataModel> models)
        {
            Type type = models.GetType().GetElementType();
            return type switch
            {
                Type when type == typeof(MicrosoftSettingKeyphraseDataModel) => models.Select(m => new MicrosoftSettingKeyphrase(m as MicrosoftSettingKeyphraseDataModel)).ToArray(),
                Type when type == typeof(UtilityKeyphraseDataModel) => models.Select(m => new UtilityKeyphrase(m as UtilityKeyphraseDataModel)).ToArray(),
                _ => null,
            };
        }
    }
}
