namespace Reginald.Data.ObjectModels
{
    using System;
    using Reginald.Core.Extensions;
    using Reginald.Data.Inputs;
    using Reginald.Data.Producers;
    using Reginald.Data.Products;
    using Reginald.Services.Utilities;

    public class Url : DataModel, ISingleProducer<SearchResult>
    {
        public bool Check(string input)
        {
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
            SearchResult result = new(Caption, IconPath, Description.PrependScheme());
            result.EnterKeyPressed += OnEnterKeyPressed;
            return result;
        }

        private void OnEnterKeyPressed(object sender, InputProcessingEventArgs e)
        {
            ProcessUtility.GoTo(Uri.IsWellFormedUriString(Description, UriKind.Absolute) ? Description : Description.PrependScheme());
        }
    }
}
