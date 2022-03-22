namespace Reginald.Data.Units
{
    using Newtonsoft.Json;

    public interface IUnitDataModel
    {
        [JsonProperty("guid")]
        public string Guid { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        public abstract bool Predicate(string guid);
    }
}
