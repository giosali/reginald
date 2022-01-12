using Newtonsoft.Json;
using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Reginald.Core.AbstractProducts
{
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class Keyword : InteractiveAbstractProductBase
    {
        [JsonProperty("guid")]
        private Guid _guid;
        public Guid Guid
        {
            get => _guid;
            set
            {
                _guid = value;
                NotifyOfPropertyChange(() => Guid);
            }
        }

        [JsonProperty("name")]
        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                NotifyOfPropertyChange(() => Name);
            }
        }

        [JsonProperty("keyword")]
        private string _word;
        public string Word
        {
            get => _word;
            set
            {
                _word = value;
                NotifyOfPropertyChange(() => Word);
            }
        }

        [JsonProperty("icon")]
        private ImageSource _icon;
        public ImageSource Icon
        {
            get => _icon;
            set
            {
                _icon = value;
                NotifyOfPropertyChange(() => Icon);
            }
        }

        [JsonProperty("format")]
        private string _format;
        public string Format
        {
            get => _format;
            set
            {
                _format = value;
                NotifyOfPropertyChange(() => Format);
            }
        }

        [JsonProperty("placeholder")]
        private string _placeholder;
        public string Placeholder
        {
            get => _placeholder;
            set
            {
                _placeholder = value;
                NotifyOfPropertyChange(() => Placeholder);
            }
        }

        [JsonProperty("caption")]
        private string _caption;
        public string Caption
        {
            get => _caption;
            set
            {
                _caption = value;
                NotifyOfPropertyChange(() => Caption);
            }
        }

        [JsonProperty("isEnabled")]
        private bool _isEnabled;
        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                _isEnabled = value;
                NotifyOfPropertyChange(() => IsEnabled);
            }
        }

        private string _completion;
        public string Completion
        {
            get => _completion;
            set
            {
                _completion = value;
                NotifyOfPropertyChange(() => Completion);
            }
        }

        private string _description;
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
