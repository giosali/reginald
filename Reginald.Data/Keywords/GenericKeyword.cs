namespace Reginald.Data.Keywords
{
    using System;
    using System.Globalization;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Reginald.Core.Extensions;
    using Reginald.Core.Helpers;
    using Reginald.Core.Utilities;

    [JsonObject(MemberSerialization.OptIn)]
    public class GenericKeyword : Keyword
    {
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

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("altUrl")]
        public string AltUrl { get; set; }

        [JsonProperty("separator")]
        public string Separator { get; set; }

        [JsonProperty("altDescription")]
        public string AltDescription { get; set; }

        public override bool Predicate(Regex rx, (string Keyword, string Separator, string Description) input)
        {
            Match match = rx.Match(Word);

            // If user enters a space after the keyword, both keywords must match
            if (match.Success && !(input.Separator.Length > 0 && Word != match.Value))
            {
                Completion = input.Description.Length > 0 ? input.Description : null;
                Description = Completion is null
                            ? string.Format(CultureInfo.InvariantCulture, Format, Placeholder)
                            : string.Format(CultureInfo.InvariantCulture, Format, Completion);
                return true;
            }

            return false;
        }

        public override Task<bool> PredicateAsync(Regex rx, (string Keyword, string Separator, string Description) input, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public override void EnterDown(bool isAltDown, Action action)
        {
            // If Completion is null, then only the keyword has been typed
            // For example, if the input is: "ddg what time is it?",
            // then Completion won't be null; Completion will be: "what time is it?"
            if (Completion is not null || (isAltDown && !string.IsNullOrEmpty(AltUrl)))
            {
                string uri = isAltDown
                           ? string.Format(CultureInfo.InvariantCulture, AltUrl, Completion)
                           : string.Format(CultureInfo.InvariantCulture, Url, string.IsNullOrEmpty(Separator) ? Completion : Completion.Quote(Separator));
                ProcessUtility.GoTo(uri);
            }
        }

        public override Task<bool> EnterDownAsync(bool isAltDown, Action action, object o)
        {
            throw new NotImplementedException();
        }

        public override (string Description, string Caption) AltDown()
        {
            return (string.IsNullOrEmpty(AltDescription) ? null : AltDescription, null);
        }

        public override (string Description, string Caption) AltUp()
        {
            return (string.Format(Format, Completion ?? Placeholder), null);
        }
    }
}
