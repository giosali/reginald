namespace Reginald.Data.ObjectModels
{
    using System;
    using Reginald.Core.Extensions;
    using Reginald.Data.Inputs;
    using Reginald.Data.Producers;
    using Reginald.Data.Products;
    using Reginald.Services.Utilities;

    public class Url : ObjectModel, ISingleProducer<SearchResult>
    {
        private string _input;

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
                _input = input;
            }

            return isTld;
        }

        public SearchResult Produce()
        {
            SearchResult result = new(Caption, Icon, _input.PrependScheme());
            result.EnterKeyPressed += OnEnterKeyPressed;
            return result;
        }

        private void OnEnterKeyPressed(object sender, InputProcessingEventArgs e)
        {
            ProcessUtility.GoTo(Uri.IsWellFormedUriString(_input, UriKind.Absolute) ? _input : _input.PrependScheme());
        }
    }
}
