using Reginald.Core.Enums;
using Reginald.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;
using Reginald.Extensions;
using Reginald.Core.Base;

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
            XmlNode node = doc.GetNode(String.Format(Constants.NamespaceNameXpathFormat, attribute));
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
                string description = String.Format(format, text);
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

        public string Name { get; set; }
        public Category Category { get; set; }
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

        public static List<SearchResultModel> MakeList(XmlDocument doc, string input, string attribute, Category category)
        {
            XmlNodeList nodes = doc.GetNodes(String.Format(Constants.NamespaceNameXpathFormat, attribute));
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
                        bool isEnabled = Boolean.Parse(isEnabledNode.InnerText);
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
                    string text = input == String.Empty ? defaultText : input;
                    string description = String.Format(format, text);
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

        public static SearchResultModel[] MakeArray(XmlDocument doc, string input, string attribute, Category category, string text = null)
        {
            XmlNodeList nodes = doc.GetNodes(String.Format(Constants.NamespaceNameXpathFormat, attribute));
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
                        string description = String.Format(format, text);
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

        public static SearchResultModel[] MakeArray(XmlDocument doc, string attribute, Category category)
        {
            XmlNodeList nodes = doc.GetNodes(String.Format(Constants.NamespaceNameXpathFormat, attribute));
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
                        string description = String.Format(format, text);
                        string alt = node["Alt"].InnerText;
                        bool isEnabled = Boolean.Parse(node["IsEnabled"].InnerText);

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
    }
}