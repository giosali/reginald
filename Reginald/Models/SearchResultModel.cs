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

        public SearchResultModel(XmlNode node, string description, Category category)
        {
            Name = node["Name"].InnerText;
            Category = category;
            Icon = BitmapImageHelper.MakeFromUri(node["Icon"].InnerText);
            Keyword = node["Keyword"].InnerText;
            Format = node["Format"].InnerText;
            Description = description;
            Alt = node["Alt"].InnerText;
        }

        public SearchResultModel(string message, Utility utility)
        {
            Description = message;
            Icon = BitmapImageHelper.MakeFromUri(string.Format(Constants.AssemblyPath, "Images/exclamation.png"));
            Alt = "Confirmation Required";
            Category = Category.Confirmation;
            Utility = utility;
        }

        public string Name { get; set; }
        public Category Category { get; set; }
        public Utility Utility { get; set; }
        public ImageSource Icon { get; set; }
        public string ParsingName { get; set; }
        public int ID { get; set; }
        public string Keyword { get; set; }
        public string Separator { get; set; }
        public string URL { get; set; }
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

        public static List<SearchResultModel> MakeList(XmlDocument doc, string input, string attribute, Category category)
        {
            try
            {
                XmlNodeList nodes = doc.GetNodes(string.Format(Constants.NamespaceNameXpathFormat, attribute));
                if (nodes is null)
                    return new List<SearchResultModel>();
                else
                {
                    List<SearchResultModel> models = new();
                    for (int i = 0; i < nodes.Count; i++)
                    {
                        XmlNode node = nodes[i];
                        string name = node["Name"].InnerText;

                        XmlNode isEnabledNode = node["IsEnabled"];
                        if (isEnabledNode is not null)
                        {
                            bool isEnabled = bool.Parse(isEnabledNode.InnerText);
                            if (!isEnabled)
                                continue;
                        }
                        else
                            continue;

                        BitmapImage icon = BitmapImageHelper.MakeFromUri(node["Icon"].InnerText);
                        string keyword = node["Keyword"].InnerText;
                        string separator = node["Separator"].InnerText;
                        string url = node["URL"].InnerText;
                        string format = node["Format"].InnerText;
                        string defaultText = node["DefaultText"].InnerText;
                        string text = input == string.Empty ? defaultText : input;
                        string description = string.Format(format, text);
                        string alt = node["Alt"].InnerText;

                        models.Add(new SearchResultModel
                        {
                            Name = name,
                            Category = category,
                            Icon = icon,
                            Keyword = keyword,
                            Separator = separator,
                            URL = url,
                            Text = text,
                            Format = format,
                            DefaultText = defaultText,
                            Description = description,
                            Alt = alt
                        });
                    }
                    return models;
                }
            }
            catch (System.Xml.XPath.XPathException)
            {
                return new List<SearchResultModel>();
            }
        }

        public static SearchResultModel[] MakeArray(XmlDocument doc, string input, string attribute, Category category, string text = null)
        {
            try
            {
                XmlNodeList nodes = doc.GetNodes(string.Format(Constants.NamespaceNameXpathFormat, attribute));
                if (nodes is null)
                    return Array.Empty<SearchResultModel>();
                else
                {
                    SearchResultModel[] models = new SearchResultModel[nodes.Count];
                    for (int i = 0; i < nodes.Count; i++)
                    {
                        try
                        {
                            XmlNode node = nodes[i];
                            string name = node["Name"].InnerText;
                            int id = int.Parse(node.Attributes["ID"].Value);
                            BitmapImage icon = BitmapImageHelper.MakeFromUri(node["Icon"].InnerText);
                            string keyword = node["Keyword"].InnerText;
                            string separator = node["Separator"].InnerText;
                            string url = node["URL"].InnerText;
                            string format = node["Format"].InnerText;
                            text = text is not null ? text : input;
                            string description = string.Format(format, text);
                            string alt = node["Alt"].InnerText;

                            models[i] = new SearchResultModel()
                            {
                                Name = name,
                                Category = category,
                                Icon = icon,
                                ID = id,
                                Keyword = keyword,
                                Separator = separator,
                                URL = url,
                                Text = text,
                                Format = format,
                                Description = description,
                                Alt = alt
                            };
                        }
                        catch (NullReferenceException)
                        {
                            continue;
                        }
                    }
                    return models;
                }
            }
            catch (System.Xml.XPath.XPathException)
            {
                return Array.Empty<SearchResultModel>();
            }
        }

        public static SearchResultModel[] MakeArray(XmlDocument doc, string attribute, Category category)
        {
            try
            {
                XmlNodeList nodes = doc.GetNodes(string.Format(Constants.NamespaceNameXpathFormat, attribute));
                if (nodes is null)
                    return Array.Empty<SearchResultModel>();
                else
                {
                    SearchResultModel[] models = new SearchResultModel[nodes.Count];
                    for (int i = 0; i < nodes.Count; i++)
                    {
                        try
                        {
                            XmlNode node = nodes[i];
                            string name = node["Name"].InnerText;
                            int id = int.Parse(node.Attributes["ID"].Value);
                            BitmapImage icon = BitmapImageHelper.MakeFromUri(node["Icon"].InnerText);
                            string keyword = node["Keyword"].InnerText;
                            string separator = node["Separator"].InnerText;
                            string url = node["URL"].InnerText;
                            string format = node["Format"].InnerText;
                            string defaultText = node["DefaultText"].InnerText;
                            string text = defaultText;
                            string description = string.Format(format, text);
                            string alt = node["Alt"].InnerText;
                            bool isEnabled = bool.Parse(node["IsEnabled"].InnerText);

                            models[i] = new SearchResultModel()
                            {
                                Name = name,
                                Category = category,
                                Icon = icon,
                                ID = id,
                                Keyword = keyword,
                                Separator = separator,
                                URL = url,
                                Text = text,
                                Format = format,
                                DefaultText = defaultText,
                                Description = description,
                                Alt = alt,
                                IsEnabled = isEnabled
                            };
                        }
                        catch (NullReferenceException)
                        {
                            continue;
                        }
                    }
                    return models;
                }
            }
            catch (System.Xml.XPath.XPathException)
            {
                return Array.Empty<SearchResultModel>();
            }
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
                        catch (NullReferenceException)
                        {
                            continue;
                        }
                    }
                    return models;
                }
                return new List<SearchResultModel>();
            }
            catch (System.Xml.XPath.XPathException)
            {
                return new List<SearchResultModel>();
            }
        }

        public static List<SearchResultModel> MakeListForUtilities(XmlDocument doc, string attribute, Category category)
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
                                    Utility = (Utility)Enum.Parse(typeof(Utility), node["Utility"].InnerText),
                                    Icon = BitmapImageHelper.MakeFromUri(node["Icon"].InnerText),
                                    Keyword = node["Keyword"].InnerText,
                                    Description = node["Description"].InnerText,
                                    Alt = node["Alt"].InnerText,
                                    IsEnabled = bool.Parse(node["IsEnabled"].InnerText),
                                    RequiresConfirmation = bool.Parse(node["RequiresConfirmation"].InnerText),
                                    ConfirmationMessage = node["ConfirmationMessage"].InnerText
                                };
                                models.Add(model);
                            }
                        }
                        catch (NullReferenceException)
                        {
                            continue;
                        }
                    }
                    return models;
                }
                return new List<SearchResultModel>();
            }
            catch (System.Xml.XPath.XPathException)
            {
                return new List<SearchResultModel>();
            }
        }
    }
}