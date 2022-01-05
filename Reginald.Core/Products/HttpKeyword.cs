using Reginald.Core.AbstractProducts;
using Reginald.Core.Apis.Cloudflare;
using Reginald.Core.Apis.Styvio;
using Reginald.Core.DataModels;
using Reginald.Core.Enums;
using Reginald.Core.Helpers;
using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Reginald.Core.Products
{
    public class HttpKeyword : Keyword
    {
        private ImageSource _primaryIcon;
        public ImageSource PrimaryIcon
        {
            get => _primaryIcon;
            set
            {
                _primaryIcon = value;
                NotifyOfPropertyChange(() => PrimaryIcon);
            }
        }

        private ImageSource _auxiliaryIcon;
        public ImageSource AuxiliaryIcon
        {
            get => _auxiliaryIcon;
            set
            {
                _auxiliaryIcon = value;
                NotifyOfPropertyChange(() => AuxiliaryIcon);
            }
        }

        private string _captionFormat;
        public string CaptionFormat
        {
            get => _captionFormat;
            set
            {
                _captionFormat = value;
                NotifyOfPropertyChange(() => CaptionFormat);
            }
        }

        private Api _api;
        public Api Api
        {
            get => _api;
            set
            {
                _api = value;
                NotifyOfPropertyChange(() => Api);
            }
        }

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

        public override bool Predicate(Keyword keyword, Regex rx, (string Keyword, string Separator, string Description) input)
        {
            throw new NotImplementedException();
        }

        public override async Task<bool> PredicateAsync(Keyword keyword, Regex rx, (string Keyword, string Separator, string Description) input, CancellationToken token)
        {
            try
            {
                if (rx.IsMatch(keyword.Name))
                {
                    HttpKeyword httpKeyword = keyword as HttpKeyword;
                    token.ThrowIfCancellationRequested();
                    switch (httpKeyword.Api)
                    {
                        case Api.Cloudflare:
                            CloudflareIpAddress ipAddress = !(input.Separator.Length > 0)
                                                          ? await CloudflareApi.GetIpAddress(token)
                                                          : null;
                            if (ipAddress is null)
                            {
                                return false;
                            }

                            httpKeyword.Icon = httpKeyword.PrimaryIcon;
                            httpKeyword.Description = ipAddress.Origin;
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

                            httpKeyword.Icon = isPriceDifferenceNegative
                                             ? httpKeyword.AuxiliaryIcon
                                             : httpKeyword.PrimaryIcon;
                            httpKeyword.Description = string.Format(httpKeyword.Format, currentPrice, priceDifferenceText);
                            httpKeyword.AltDescription = string.Format(httpKeyword.Format, currentPrice, stock.PercentText);
                            httpKeyword.Caption = string.Format(httpKeyword.CaptionFormat, stock.Ticker, stock.ShortName);
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

        public override void EnterDown(Keyword keyword, bool isAltDown, Action action)
        {

        }

        public override (string Description, string Caption) AltDown(Keyword keyword)
        {
            HttpKeyword httpKeyword = keyword as HttpKeyword;
            string description = httpKeyword.AltDescription;
            string caption = null;
            return (description, caption);
        }

        public override (string Description, string Caption) AltUp(Keyword keyword)
        {
            HttpKeyword httpKeyword = keyword as HttpKeyword;
            string description = httpKeyword.Description;
            string caption = null;
            return (description, caption);
        }
    }
}
