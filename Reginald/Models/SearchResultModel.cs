using Reginald.Core.Enums;
using Reginald.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;
using Reginald.Extensions;
using Reginald.Core.Base;
using Reginald.Core.Utils;
using System.Threading.Tasks;

namespace Reginald.Models
{
    public class SearchResultModel
    {
        public SearchResultModel() { }

        public SearchResultModel(string name, string parsingName, BitmapImage icon)
        {
            Name = name;
            Category = Category.Application;
            ParsingName = parsingName;
            Icon = icon;
            Text = name;
            Format = "{0}";
            Description = name;
            Alt = "Application";
        }

        public SearchResultModel(XmlDocument doc, string input, string attribute, Category category)
        {
            XmlNode node = doc.GetNode(string.Format(Constants.NamespaceNameXpathFormat, attribute));
            if (node is null) { }
            else
            {
                string name = node["Name"].InnerText;
                BitmapImage icon = BitmapImageHelper.MakeFromUri(node["Icon"].InnerText);
                string keyword = node["Keyword"].InnerText;
                string separator = node["Separator"].InnerText;
                string url = node["URL"].InnerText;
                string text = input;
                string format = node["Format"].InnerText;
                string description = string.Format(format, text);
                string alt = node["Alt"].InnerText;

                Name = name;
                Category = category;
                Icon = icon;
                Keyword = keyword;
                Separator = separator;
                URL = url;
                Text = text;
                Format = format;
                Description = description;
                Alt = alt;
            }
        }

        public SearchResultModel(string message, Utility? utility)
        {
            Description = message;
            Icon = BitmapImageHelper.MakeFromUri(string.Format(Constants.AssemblyPath, "Images/exclamation.png"));
            Alt = "Confirmation Required";
            Category = Category.Confirmation;
            Utility = utility;
        }

        public SearchResultModel(XmlNode node, Category category, string input = null, string text = null)
        {
            Name = node["Name"]?.InnerText;
            Category = category;

            _ = Enum.TryParse(typeof(Utility), node["Utility"]?.InnerText, out object utilityResult);
            Utility = utilityResult is null ? null : (Utility)utilityResult;

            _ = int.TryParse(node.Attributes["ID"]?.Value, out int idResult);
            ID = idResult;

            Icon = node["Icon"] is null ? null : BitmapImageHelper.MakeFromUri(node["Icon"].InnerText);
            Keyword = node["Keyword"]?.InnerText;
            Separator = node["Separator"]?.InnerText;
            URL = node["URL"]?.InnerText;
            URI = node["URI"]?.InnerText;

            string description = node["Description"]?.InnerText;
            DefaultText = node["DefaultText"]?.InnerText;
            input = string.IsNullOrEmpty(input) ? null : input;
            Text = description ?? text ?? input ?? DefaultText;
            Format = node["Format"]?.InnerText ?? "{0}";
            _ = StringHelper.TryFormat(Format, Text, out string descriptionResult);
            Description = descriptionResult;

            Alt = node["Alt"]?.InnerText;

            bool isEnabledConversionState = bool.TryParse(node["IsEnabled"]?.InnerText, out bool isEnabledResult);
            IsEnabled = isEnabledConversionState ? isEnabledResult : true;

            _ = bool.TryParse(node["RequiresConfirmation"]?.InnerText, out bool requiresConfirmation);
            RequiresConfirmation = requiresConfirmation;
            ConfirmationMessage = node["ConfirmationMessage"]?.InnerText;
        }

        public string Name { get; set; }
        public Category Category { get; set; }
        public Utility? Utility { get; set; }
        public ImageSource Icon { get; set; }
        public string ParsingName { get; set; }
        public int ID { get; set; }
        public string Keyword { get; set; }
        public string Separator { get; set; }
        public string URL { get; set; }
        public string URI { get; set; }
        public string Text { get; set; }
        public string Format { get; set; }
        public string DefaultText { get; set; }
        public string Description { get; set; }
        public string Alt { get; set; }
        public string Count { get; set; }
        public bool IsEnabled { get; set; }
        public double? Time { get; set; }
        public bool RequiresConfirmation { get; set; }
        public string ConfirmationMessage { get; set; }

        public static SearchResultModel[] MakeArray(XmlDocument doc, string attribute, Category category, string input = null, string text = null)
        {
            try
            {
                XmlNodeList nodes = doc.GetNodes(string.Format(Constants.NamespaceNameXpathFormat, attribute));
                if (nodes is not null)
                {
                    SearchResultModel[] models = new SearchResultModel[nodes.Count];
                    for (int i = 0; i < nodes.Count; i++)
                    {
                        try
                        {
                            XmlNode node = nodes[i];
                            SearchResultModel model = new(node, category, input, text);
                            models[i] = model;
                        }
                        catch (NullReferenceException) { continue; }
                    }
                    return models;
                }
            }
            catch (System.Xml.XPath.XPathException) { }
            return Array.Empty<SearchResultModel>();
        }

        public static List<SearchResultModel> MakeList(XmlDocument doc, string attribute, Category category, string input = null, string text = null, bool overrideIsEnabled = false)
        {
            try
            {
                XmlNodeList nodes = doc.GetNodes(string.Format(Constants.NamespaceNameXpathFormat, attribute));
                if (nodes is not null)
                {
                    List<SearchResultModel> models = new();
                    for (int i = 0; i < nodes.Count; i++)
                    {
                        try
                        {
                            XmlNode node = nodes[i];
                            SearchResultModel model = new(node, category, input, text);
                            if (model.IsEnabled || overrideIsEnabled)
                            {
                                models.Add(model);
                            }
                        }
                        catch (NullReferenceException) { continue; }
                    }
                    return models;
                }
            }
            catch (System.Xml.XPath.XPathException) { }
            return new List<SearchResultModel>();
        }

        public static async Task<List<SearchResultModel>> MakeListForCommandsAsync(XmlDocument doc, string input, string attribute, Category category)
        {
            try
            {
                XmlNodeList nodes = doc.GetNodes(string.Format(Constants.NamespaceNameXpathFormat, attribute));
                if (nodes is not null)
                {
                    List<SearchResultModel> models = new();
                    for (int i = 0; i < nodes.Count; i++)
                    {
                        try
                        {
                            XmlNode node = nodes[i];
                            if (bool.Parse(node["IsEnabled"].InnerText))
                            {
                                SearchResultModel model = new()
                                {
                                    Name = node["Name"].InnerText,
                                    Category = category,
                                    Icon = BitmapImageHelper.MakeFromUri(node["Icon"].InnerText),
                                    Keyword = node["Keyword"].InnerText,
                                    Format = node["Format"].InnerText,
                                    DefaultText = node["DefaultText"].InnerText,
                                    Alt = node["Alt"].InnerText,
                                    IsEnabled = bool.Parse(node["IsEnabled"].InnerText)
                                };

                                Command command = (Command)Enum.Parse(typeof(Command), node["Command"].InnerText.Capitalize());
                                switch (command)
                                {
                                    case Command.Timer:
                                        (string description, double? seconds) = await TimerUtils.ParseTimeFromStringAsync(input, model.Format, node["Split"].InnerText, model.DefaultText);
                                        model.Description = description;
                                        model.Time = seconds;
                                        (_, string separator, string remainder) = description.Partition(": ");
                                        if (!string.IsNullOrEmpty(separator))
                                        {
                                            model.Text = remainder;
                                        }
                                        break;

                                    default:
                                        break;
                                }

                                if (model.Description is not null)
                                {
                                    models.Add(model);
                                }
                            }
                        }
                        catch (NullReferenceException) { continue; }
                    }
                    return models;
                }
            }
            catch (System.Xml.XPath.XPathException) { }
            return new List<SearchResultModel>();
        }
    }
}