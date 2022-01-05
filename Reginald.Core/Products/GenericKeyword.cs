using Newtonsoft.Json;
using Reginald.Core.AbstractProducts;
using Reginald.Core.DataModels;
using Reginald.Core.Helpers;
using Reginald.Core.Utilities;
using Reginald.Extensions;
using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Reginald.Core.Products
{
    [JsonObject(MemberSerialization.OptIn)]
    public class GenericKeyword : Keyword
    {
        [JsonProperty("url")]
        private string _url;
        public string Url
        {
            get => _url;
            set
            {
                _url = value;
                NotifyOfPropertyChange(() => Url);
            }
        }

        [JsonProperty("altUrl")]
        private string _altUrl;
        public string AltUrl
        {
            get => _altUrl;
            set
            {
                _altUrl = value;
                NotifyOfPropertyChange(() => AltUrl);
            }
        }

        [JsonProperty("separator")]
        private string _separator;
        public string Separator
        {
            get => _separator;
            set
            {
                _separator = value;
                NotifyOfPropertyChange(() => Separator);
            }
        }

        [JsonProperty("altDescription")]
        private string _altDescription;
        public string AltDescription
        {
            get => _altDescription;
            set
            {
                _altDescription = value;
                NotifyOfPropertyChange(() => AltDescription);
            }
        }

        public GenericKeyword()
        {

        }

        public GenericKeyword(GenericKeywordDataModel model)
        {
            if (Guid.TryParse(model.Guid, out Guid guid))
            {
                Guid = guid;
            }
            Name = model.Name;
            Word = model.Keyword;
            Icon = BitmapImageHelper.FromUri(model.Icon);
            Format = model.Format;
            Placeholder = model.Placeholder;
            Caption = model.Caption;
            IsEnabled = model.IsEnabled;
            Url = model.Url;
            AltUrl = model.AltUrl;
            Separator = model.Separator;
            AltDescription = model.AltDescription;
        }

        public override bool Predicate(Keyword keyword, Regex rx, (string Keyword, string Separator, string Description) input)
        {
            Match match = rx.Match(keyword.Word);
            if (match.Success)
            {
                // If user enters a space after the keyword, both keywords must match
                if (input.Separator.Length > 0 && keyword.Word != match.Value)
                {
                    return false;
                }

                keyword.Completion = input.Description.Length > 0 ? input.Description : null;
                keyword.Description = keyword.Completion is null
                                    ? string.Format(keyword.Format, keyword.Placeholder)
                                    : string.Format(keyword.Format, keyword.Completion);
                return true;
            }
            return false;
        }

        public override Task<bool> PredicateAsync(Keyword keyword, Regex rx, (string Keyword, string Separator, string Description) input, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public override void EnterDown(Keyword keyword, bool isAltDown, Action action)
        {
            GenericKeyword genericKeyword = keyword as GenericKeyword;
            // If Completion is null, then only the keyword has been typed
            // For example, if the input is: "ddg what time is it?",
            // then Completion won't be null; Completion will be: "what time is it?"
            if (genericKeyword.Completion is not null || (isAltDown && !string.IsNullOrEmpty(genericKeyword.AltUrl)))
            {
                string url;
                string completion;
                if (isAltDown)
                {
                    url = genericKeyword.AltUrl;
                    completion = genericKeyword.Completion;
                }
                else
                {
                    url = genericKeyword.Url;
                    completion = string.IsNullOrEmpty(genericKeyword.Separator)
                               ? genericKeyword.Completion
                               : genericKeyword.Completion.Quote(genericKeyword.Separator);
                }
                string uri = string.Format(CultureInfo.InvariantCulture, url, completion);
                Processes.GoTo(uri);
            }
        }

        public override (string Description, string Caption) AltDown(Keyword keyword)
        {
            GenericKeyword genericKeyword = keyword as GenericKeyword;
            string description = string.IsNullOrEmpty(genericKeyword.AltDescription)
                               ? null
                               : genericKeyword.AltDescription;
            string caption = null;
            return (description, caption);
        }

        public override (string Description, string Caption) AltUp(Keyword keyword)
        {
            GenericKeyword genericKeyword = keyword as GenericKeyword;
            string description = string.Format(genericKeyword.Format, genericKeyword.Completion ?? genericKeyword.Placeholder);
            string caption = null;
            return (description, caption);
        }
    }
}
