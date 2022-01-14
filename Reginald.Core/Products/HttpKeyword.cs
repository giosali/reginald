namespace Reginald.Core.Products
{
    using System;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Media;
    using Reginald.Core.AbstractProducts;
    using Reginald.Core.Apis.Cloudflare;
    using Reginald.Core.Apis.Styvio;
    using Reginald.Core.DataModels;
    using Reginald.Core.Helpers;

    /// <summary>
    /// Specifies the possible types of APIs.
    /// </summary>
    public enum Api
    {
        /// <summary>
        /// The API is the Styvio API.
        /// </summary>
        Styvio,

        /// <summary>
        /// The API is the Cloudflare API.
        /// </summary>
        Cloudflare,
    }

    public class HttpKeyword : Keyword
    {
        private ImageSource _primaryIcon;

        private ImageSource _auxiliaryIcon;

        private string _captionFormat;

        private Api _api;

        private string _altDescription;

        public HttpKeyword(HttpKeywordDataModel model)
        {
            if (Guid.TryParse(model.Guid, out Guid guid))
            {
                Guid = guid;
            }

            Name = model.Name;
            Word = model.Keyword;
            PrimaryIcon = BitmapImageHelper.FromUri(model.PrimaryIcon);
            Icon = PrimaryIcon;
            AuxiliaryIcon = model.AuxiliaryIcon is null ? null : BitmapImageHelper.FromUri(model.AuxiliaryIcon);
            Format = model.Format;
            Description = model.Description;
            Caption = model.Caption;
            CaptionFormat = model.CaptionFormat;
            IsEnabled = model.IsEnabled;
            if (Enum.TryParse(model.Api, true, out Api api))
            {
                Api = api;
            }
        }

        public ImageSource PrimaryIcon
        {
            get => _primaryIcon;
            set
            {
                _primaryIcon = value;
                NotifyOfPropertyChange(() => PrimaryIcon);
            }
        }

        public ImageSource AuxiliaryIcon
        {
            get => _auxiliaryIcon;
            set
            {
                _auxiliaryIcon = value;
                NotifyOfPropertyChange(() => AuxiliaryIcon);
            }
        }

        public string CaptionFormat
        {
            get => _captionFormat;
            set
            {
                _captionFormat = value;
                NotifyOfPropertyChange(() => CaptionFormat);
            }
        }

        public Api Api
        {
            get => _api;
            set
            {
                _api = value;
                NotifyOfPropertyChange(() => Api);
            }
        }

        public string AltDescription
        {
            get => _altDescription;
            set
            {
                _altDescription = value;
                NotifyOfPropertyChange(() => AltDescription);
            }
        }

        public override bool Predicate(Regex rx, (string Keyword, string Separator, string Description) input)
        {
            throw new NotImplementedException();
        }

        public override async Task<bool> PredicateAsync(Regex rx, (string Keyword, string Separator, string Description) input, CancellationToken token)
        {
            try
            {
                if (rx.IsMatch(Name))
                {
                    token.ThrowIfCancellationRequested();
                    switch (Api)
                    {
                        case Api.Cloudflare:
                            CloudflareIpAddress ipAddress = !(input.Separator.Length > 0)
                                                          ? await CloudflareApi.GetIpAddress(token)
                                                          : null;
                            if (ipAddress is null)
                            {
                                return false;
                            }

                            Icon = PrimaryIcon;
                            Description = ipAddress.Origin;
                            break;

                        case Api.Styvio:
                            StyvioStock stock = input.Description.Length is > 0 and < 5
                                              ? await StyvioApi.GetStock(input.Description, token)
                                              : null;
                            if (stock is null)
                            {
                                return false;
                            }

                            float currentPrice = float.Parse(stock.CurrentPrice.Trim('$'));
                            float percentage = float.Parse(stock.PercentText.Trim(new char[] { '-', '+', '%' }));
                            double priceDifference = Math.Round(currentPrice - (currentPrice * 100 / (percentage + 100)), 2);
                            bool isPriceDifferenceNegative;
                            string priceDifferenceText = (isPriceDifferenceNegative = stock.PercentText.StartsWith("-"))
                                                       ? "-" + priceDifference.ToString()
                                                       : "+" + priceDifference.ToString();

                            Icon = isPriceDifferenceNegative ? AuxiliaryIcon : PrimaryIcon;
                            Description = string.Format(Format, currentPrice, priceDifferenceText);
                            AltDescription = string.Format(Format, currentPrice, stock.PercentText);
                            Caption = string.Format(CaptionFormat, stock.Ticker, stock.ShortName);
                            break;
                    }

                    token.ThrowIfCancellationRequested();
                    return true;
                }

                return false;
            }
            catch (OperationCanceledException)
            {
                return false;
            }
        }

        public override void EnterDown(bool isAltDown, Action action)
        {
        }

        public override Task<bool> EnterDownAsync(bool isAltDown, Action action, object o)
        {
            return Task.FromResult(true);
        }

        public override (string Description, string Caption) AltDown()
        {
            return (AltDescription, null);
        }

        public override (string Description, string Caption) AltUp()
        {
            return (Description, null);
        }
    }
}
