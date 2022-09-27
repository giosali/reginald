namespace Reginald.Models.DataModels
{
    using Newtonsoft.Json;
    using Reginald.Models.Producers;
    using Reginald.Models.Products;

    internal sealed class FileSystemEntrySearch : DataModel, ISingleProducer<SearchResult>
    {
        [JsonProperty("isEnabled")]
        public bool IsEnabled { get; set; }

        [JsonProperty("key")]
        public string Key { get; set; }

        public bool Check(string input)
        {
            return IsEnabled && input.IndexOf(Key) == 0;
        }

        public SearchResult Produce()
        {
            return new SearchResult(Caption, IconPath, Description, Id);
        }
    }
}
