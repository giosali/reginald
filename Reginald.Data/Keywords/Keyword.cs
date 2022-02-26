namespace Reginald.Data.Keywords
{
    using System;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Media;
    using Newtonsoft.Json;
    using Reginald.Data.Base;

    [JsonObject(MemberSerialization.OptIn)]
    public abstract class Keyword : InteractiveObjectBase
    {
        public const string KeywordRegexFormat = @"^{0}";

        public const string PreciseKeywordRegexFormat = @"^\b{0}\b";

        [JsonProperty("guid")]
        private Guid _guid;

        [JsonProperty("name")]
        private string _name;

        [JsonProperty("keyword")]
        private string _word;

        [JsonProperty("icon")]
        private ImageSource _icon;

        [JsonProperty("format")]
        private string _format;

        [JsonProperty("placeholder")]
        private string _placeholder;

        [JsonProperty("caption")]
        private string _caption;

        [JsonProperty("isEnabled")]
        private bool _isEnabled;

        private string _completion;

        private string _description;

        public Guid Guid
        {
            get => _guid;
            set
            {
                _guid = value;
                NotifyOfPropertyChange(() => Guid);
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                NotifyOfPropertyChange(() => Name);
            }
        }

        public string Word
        {
            get => _word;
            set
            {
                _word = value;
                NotifyOfPropertyChange(() => Word);
            }
        }

        public ImageSource Icon
        {
            get => _icon;
            set
            {
                _icon = value;
                NotifyOfPropertyChange(() => Icon);
            }
        }

        public string Format
        {
            get => _format;
            set
            {
                _format = value;
                NotifyOfPropertyChange(() => Format);
            }
        }

        public string Placeholder
        {
            get => _placeholder;
            set
            {
                _placeholder = value;
                NotifyOfPropertyChange(() => Placeholder);
            }
        }

        public string Caption
        {
            get => _caption;
            set
            {
                _caption = value;
                NotifyOfPropertyChange(() => Caption);
            }
        }

        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                _isEnabled = value;
                NotifyOfPropertyChange(() => IsEnabled);
            }
        }

        public string Completion
        {
            get => _completion;
            set
            {
                _completion = value;
                NotifyOfPropertyChange(() => Completion);
            }
        }

        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                NotifyOfPropertyChange(() => Description);
            }
        }

        public static bool operator ==(Keyword a, Keyword b)
        {
            return a is not null && b is not null && a.Guid == b.Guid;
        }

        public static bool operator !=(Keyword a, Keyword b)
        {
            return a is not null && b is not null && a.Guid != b.Guid;
        }

        public override bool Equals(object obj)
        {
            return obj is not null && obj is Keyword keyword && Guid == keyword.Guid;
        }

        public override int GetHashCode()
        {
            return Guid.GetHashCode();
        }

        public abstract bool Predicate(Regex rx, (string Keyword, string Separator, string Description) input);

        public abstract Task<bool> PredicateAsync(Regex rx, (string Keyword, string Separator, string Description) input, CancellationToken token);
    }
}
