namespace Reginald.Data.Keywords
{
    using System;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Media;
    using Reginald.Core.Apis.Cloudflare;
    using Reginald.Core.Apis.Styvio;
    using Reginald.Core.Helpers;

    /// <summary>
    /// Specifies the types of APIs.
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

    public partial class HttpKeyword : Keyword
    {
        public const string Filename = "HttpKeywords.json";

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

            CanReceiveKeyboardInput = false;
        }

        public ImageSource PrimaryIcon { get; set; }

        public ImageSource AuxiliaryIcon { get; set; }

        public string CaptionFormat { get; set; }

        public Api Api { get; set; }

        public string AltDescription { get; set; }

        private static CancellationTokenSource Source { get; set; } = new();

        public override void EnterKeyDown()
        {
        }

        public override void AltKeyDown()
        {
            IsAltKeyDown = true;
            TempCaption = Caption;
            TempDescription = AltDescription;
        }

        public override void AltKeyUp()
        {
            IsAltKeyDown = false;
            TempCaption = Caption;
            TempDescription = Description;
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
            catch (Exception)
            {
                return false;
            }
        }
    }
}
