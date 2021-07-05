using Caliburn.Micro;
using Reginald.Extensions;
using Reginald.Models;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Xml;

namespace Reginald.ViewModels
{
    public class NewUserKeywordViewModel : Screen
    {
        public NewUserKeywordViewModel(BindableCollection<SearchResultModel> collection)
        {
            UserKeywordSearchResults = collection;
            SelectedKeywordSearchResult = new SearchResultModel
            {
                Separator = "+",
                DefaultText = "..."
            };
            NotifyOfPropertyChange(() => SelectedKeywordSearchResult);
        }

        private BindableCollection<SearchResultModel> _userKeywordSearchResults;
        public BindableCollection<SearchResultModel> UserKeywordSearchResults
        {
            get
            {
                return _userKeywordSearchResults;
            }
            set
            {
                _userKeywordSearchResults = value;
                NotifyOfPropertyChange(() => UserKeywordSearchResults);
            }
        }

        private SearchResultModel _selectedKeywordSearchResult;
        public SearchResultModel SelectedKeywordSearchResult
        {
            get
            {
                return _selectedKeywordSearchResult;
            }
            set
            {
                _selectedKeywordSearchResult = value;
                NotifyOfPropertyChange(() => SelectedKeywordSearchResult);
            }
        }

        private string IconPath { get; set; }

        public void IconPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new();
            openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.png)|*.jpg;*.jpeg;*.png";
            if (openFileDialog.ShowDialog() == true)
            {
                System.Drawing.Image image = System.Drawing.Image.FromFile(openFileDialog.FileName);
                if (image.Width < 75 || image.Height < 75)
                {
                    MessageBox.Show($"Images cannot be smaller than 75x75. This file: {image.Width}x{image.Height}");
                }
                else
                {
                    string[] results = openFileDialog.FileName.Split(@"\");
                    string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Reginald", "UserIcons", results[results.Count() - 1]);
                    while (File.Exists(path))
                    {
                        (string Left, string Separator, string Right) rpartition = path.RPartition(".");
                        path = rpartition.Left + "_copy" + rpartition.Separator + rpartition.Right;
                    }
                    File.Copy(openFileDialog.FileName, path);
                    IconPath = path;

                    BitmapImage icon = new();
                    icon.BeginInit();
                    icon.UriSource = new Uri(path);
                    icon.CacheOption = BitmapCacheOption.OnLoad;
                    icon.DecodePixelWidth = 75;
                    icon.DecodePixelHeight = 75;
                    icon.EndInit();
                    icon.Freeze();
                    SelectedKeywordSearchResult.Icon = icon;
                    NotifyOfPropertyChange(() => SelectedKeywordSearchResult);
                }
            }
        }

        public void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedKeywordSearchResult.Alt = ((TextBox)sender).Text;
            string keyword = SelectedKeywordSearchResult.Keyword;
            string name = SelectedKeywordSearchResult.Name;
            string url = SelectedKeywordSearchResult.URL;
            string separator = SelectedKeywordSearchResult.Separator;
            string format = SelectedKeywordSearchResult.Format;
            string defaultText = SelectedKeywordSearchResult.DefaultText;
            string alt = SelectedKeywordSearchResult.Alt;

            string appDataDirectoryPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string applicationName = "Reginald";
            string xmlUserKeywordFilename = "UserSearch.xml";
            string path = Path.Combine(appDataDirectoryPath, applicationName, xmlUserKeywordFilename);
            XmlDocument doc = new();
            doc.Load(path);
            XmlNode lastNode = GetLastNode(doc);
            int id = lastNode is null ? 0 : int.Parse(lastNode.Attributes["ID"].Value) + 1;
            XmlNode parentNode = lastNode is null ? doc.SelectSingleNode(@"//Searches") : lastNode.ParentNode;

            XmlNode node = MakeXmlNode(keyword, id, name, IconPath, url, separator, format, defaultText, alt);
            XmlNode importedNode = parentNode.OwnerDocument.ImportNode(node, true);
            parentNode.AppendChild(importedNode);
            doc.Save(path);

            SelectedKeywordSearchResult.Description = String.Format(format, defaultText);
            SelectedKeywordSearchResult.IsEnabled = true;
            UserKeywordSearchResults.Add(SelectedKeywordSearchResult);
            NotifyOfPropertyChange(() => SelectedKeywordSearchResult);
            NotifyOfPropertyChange(() => UserKeywordSearchResults);
            TryCloseAsync();
        }

        private static XmlNode GetLastNode(XmlDocument doc)
        {
            string xpath = @"//Searches//Namespace[position() = last()]";
            return doc.SelectSingleNode(xpath);
        }

        private static XmlNode MakeXmlNode(string keyword, int id, string name, string icon, string url,
                                           string separator, string format, string defaultText, string alt)
        {
            string xml = $"<Namespace Name=\"{keyword}\" ID=\"{id}\">" +
               $"    <Name>{name}</Name> \n" +
               $"    <Keyword>{keyword}</Keyword> \n" +
               $"    <Icon>{icon}</Icon> \n" +
               $"    <URL>{url}</URL> \n" +
               $"    <Separator>{separator}</Separator> \n" +
               $"    <Format>{format}</Format> \n" +
               $"    <DefaultText>{defaultText}</DefaultText> \n" +
               $"    <Alt>{alt}</Alt> \n" +
               $"    <IsEnabled>true</IsEnabled> \n" +
               "</Namespace>";
            XmlDocument doc = new();
            doc.LoadXml(xml);
            XmlNode node = doc.DocumentElement;
            return node;
        }
    }
}
