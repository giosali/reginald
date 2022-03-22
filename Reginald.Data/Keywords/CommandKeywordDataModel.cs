namespace Reginald.Data.Keywords
{
    using Newtonsoft.Json;

    public class CommandKeywordDataModel : DataModelBase, IKeywordDataModel
    {
        public string Name { get; set; }

        public string Keyword { get; set; }

        public string Icon { get; set; }

        public string Format { get; set; }

        public string Placeholder { get; set; }

        public string Caption { get; set; }

        public bool IsEnabled { get; set; }

        [JsonProperty("CommandType")]
        public string CommandType { get; set; }
    }
}
