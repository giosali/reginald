using Reginald.Core.Api.Cloudflare;
using Reginald.Core.Api.Styvio;
using Reginald.Core.Helpers;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Xml;

namespace Reginald.Models
{
    public class SpecialSearchResultModel
    {
        public SpecialSearchResultModel() { }

        public SpecialSearchResultModel(XmlNode node, string description)
        {
            Name = node["Name"].InnerText;
            ID = int.Parse(node.Attributes["ID"].Value);
            Keyword = node["Keyword"].InnerText;
            API = node["API"].InnerText;
            Icon = BitmapImageHelper.MakeHigherQualityFromUri(node["Icon"].InnerText);
            Description = string.Format(node["Description"].InnerText, description);
            SubOneFormat = node["SubOneFormat"].InnerText;
            SubTwoFormat = node["SubTwoFormat"].InnerText;
            CanHaveSpaces = bool.Parse(node["CanHaveSpaces"].InnerText);
            IsEnabled = bool.Parse(node["IsEnabled"].InnerText);
        }

        // XML
        public string Name { get; set; }
        public int ID { get; set; }
        public string Keyword { get; set; }
        public string API { get; set; }
        public ImageSource Icon { get; set; }
        public string Description { get; set; }
        public string SubOneFormat { get; set; }
        public string SubTwoFormat { get; set; }
        public bool CanHaveSpaces { get; set; }
        public bool IsEnabled { get; set; }

        // API
        public string Major { get; set; }
        public string MajorTwo { get; set; }
        public string Minor { get; set; }
        public string MinorTwo { get; set; }
        public string SubOne { get; set; }
        public string SubTwo { get; set; }

        // Appearance
        public Brush MinorTextBrush { get; set; }

        // Miscellaneous
        public bool IsCancelled { get; set; }

        public static async Task<SpecialSearchResultModel> MakeStyvioSpecialSearchResultModelAsync(XmlNode node, string input, CancellationToken token)
        {
            try
            {
                StyvioStock stock = await StyvioApi.GetStock(input, token);
                if (stock is null)
                    return null;

                float currentPrice = float.Parse(stock.CurrentPrice.Trim('$'));
                float percentage = float.Parse(stock.PercentText.Trim(new char[] { '-', '+', '%' }));
                double priceDifference = Math.Round(currentPrice - (currentPrice * 100 / (percentage + 100)), 2);
                string priceDifferenceText = stock.PercentText.StartsWith("-") ? "-" + priceDifference.ToString() : "+" + priceDifference.ToString();

                SpecialSearchResultModel model = new(node, stock.Ticker);

                model.Icon = stock.PercentText.StartsWith("-") ? BitmapImageHelper.MakeHigherQualityFromUri(node["AltIcon"].InnerText) : BitmapImageHelper.MakeHigherQualityFromUri(node["Icon"].InnerText);

                model.Major = stock.CurrentPrice;
                model.MajorTwo = stock.ShortName;
                model.Minor = string.Format("{0} ({1})", priceDifferenceText, stock.PercentText);
                model.MinorTwo = stock.CompanyLocation;
                model.MinorTextBrush = stock.PercentText.StartsWith("-") ? Brushes.Red : Brushes.Green;
                model.SubOne = string.Format(model.SubOneFormat, stock.DailyPrices.Max());
                model.SubTwo = string.Format(model.SubTwoFormat, stock.DailyPrices.Min());

                if (token.IsCancellationRequested)
                    token.ThrowIfCancellationRequested();

                return model;
            }
            catch (OperationCanceledException)
            {
                return new SpecialSearchResultModel()
                {
                    IsCancelled = true
                };
            }
        }

        public static async Task<SpecialSearchResultModel> MakeCloudflareSpecialSearchResultModelAsync(XmlNode node, CancellationToken token)
        {
            try
            {
                CloudflareIpAddress ipAddress = await CloudflareApi.GetIpAddress(token);
                if (ipAddress is null)
                    return null;

                SpecialSearchResultModel model = new(node, ipAddress.Origin);

                if (token.IsCancellationRequested)
                    token.ThrowIfCancellationRequested();

                return model;
            }
            catch (OperationCanceledException)
            {
                return new SpecialSearchResultModel()
                {
                    IsCancelled = true
                };
            }
        }
    }
}
