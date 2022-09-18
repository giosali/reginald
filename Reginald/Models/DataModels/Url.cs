namespace Reginald.Models.DataModels
{
    using System;
    using Newtonsoft.Json;
    using Reginald.Core.Extensions;
    using Reginald.Models.Inputs;
    using Reginald.Models.Producers;
    using Reginald.Models.Products;
    using Reginald.Services.Utilities;

    public class Url : DataModel, ISingleProducer<SearchResult>
    {
        [JsonProperty("isEnabled")]
        public bool IsEnabled { get; set; }

        public bool Check(string input)
        {
            if (!IsEnabled)
            {
                return false;
            }

            string uriString = input.Trim().Replace(" ", "%20");
            if (!Uri.IsWellFormedUriString(uriString, UriKind.Absolute))
            {
                if (!Uri.TryCreate("//" + uriString, UriKind.Absolute, out Uri uri))
                {
                    return false;
                }

                uriString = uri.ToString();
            }

            bool isTld = uriString.ContainsTopLevelDomain();
            if (isTld)
            {
                Description = input;
            }

            return isTld;
        }

        public SearchResult Produce()
        {
            SearchResult result = new(Caption, IconPath, Description.PrependScheme(), Id);
            result.EnterKeyPressed += OnEnterKeyPressed;
            return result;
        }

        private void OnEnterKeyPressed(object sender, InputProcessingEventArgs e)
        {
            e.Handled = true;
            ProcessUtility.GoTo(Uri.IsWellFormedUriString(Description, UriKind.Absolute) ? Description : Description.PrependScheme());
        }
    }
}
