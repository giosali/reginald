namespace Reginald.Core.DataModels
{
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    public class ExpansionDataModel : DataModelBase
    {
        public const string CursorVariable = "{{__cursor__}}";

        [JsonProperty("trigger")]
        public string Trigger { get; set; }

        [JsonProperty("replacement")]
        public string Replacement { get; set; }
    }
}
