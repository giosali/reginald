namespace Reginald.Data.Keywords
{
    using System;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    public abstract partial class Keyword : Item
    {
        protected const string KeywordRegexFormat = @"^{0}";

        protected const string PreciseKeywordRegexFormat = @"^\b{0}\b";

        private bool _isEnabled;

        public override bool IsAltKeyDown { get; set; }

        [JsonProperty("isEnabled")]
        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                _isEnabled = value;
                NotifyOfPropertyChange(() => IsEnabled);
            }
        }

        [JsonProperty("keyword")]
        public string Word { get; set; }

        [JsonProperty("format")]
        public string Format { get; set; }

        [JsonProperty("placeholder")]
        public string Placeholder { get; set; }

        public string Completion { get; set; }

        public virtual bool Predicate(Regex rx, (string Keyword, string Separator, string Description) input)
        {
            throw new NotImplementedException();
        }

        public virtual Task<bool> PredicateAsync(Regex rx, (string Keyword, string Separator, string Description) input, CancellationToken token)
        {
            throw new NotImplementedException();
        }
    }
}
