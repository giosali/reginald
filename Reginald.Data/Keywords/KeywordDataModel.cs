namespace Reginald.Data.Keywords
{
    /// <summary>
    /// Represents the contents of a keyword JSON file to use with Newtonsoft.Json.
    /// </summary>
    public class KeywordDataModel : IKeywordDataModel
    {
        public string Guid { get; set; }

        public string Name { get; set; }

        public string Keyword { get; set; }

        public string Icon { get; set; }

        public string Format { get; set; }

        public string Placeholder { get; set; }

        public string Caption { get; set; }

        public bool IsEnabled { get; set; }

        public string DisplayDescription { get; set; }

        public string PrimaryIcon { get; set; }
    }
}
