using Newtonsoft.Json;
using System;

namespace Reginald.Core.DataModels
{
    [Serializable]
    public class UtilityDataModel : KeyphraseDataModelBase
    {
        [JsonProperty("altDescription")]
        public string AltDescription { get; set; }

        [JsonProperty("requiresConfirmation")]
        public bool RequiresConfirmation { get; set; }

        [JsonProperty("confirmationMessage")]
        public string ConfirmationMessage { get; set; }

        [JsonProperty("utility")]
        public string Utility { get; set; }
    }
}
