using Caliburn.Micro;
using Reginald.Core.Helpers;
using Reginald.Core.IO;
using Reginald.Extensions;
using Reginald.Models;
using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Xml;

namespace Reginald.ViewModels
{
    public class CreateUserKeywordViewModel : Screen
    {
        public CreateUserKeywordViewModel(BindableCollection<SearchResultModel> collection)
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
            get => _userKeywordSearchResults;
            set
            {
                _userKeywordSearchResults = value;
                NotifyOfPropertyChange(() => UserKeywordSearchResults);
            }
        }

        private SearchResultModel _selectedKeywordSearchResult;
        public SearchResultModel SelectedKeywordSearchResult
        {
            get => _selectedKeywordSearchResult;
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
                    string path = Path.Combine(ApplicationPaths.AppDataDirectoryPath, ApplicationPaths.ApplicationName, ApplicationPaths.UserIconsDirectoryName, results[^1]);
                    while (File.Exists(path))
                    {
                        path += "_copy";
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
            string keyword = SelectedKeywordSearchResult.Keyword;
            string name = SelectedKeywordSearchResult.Alt.Capitalize();
            string url = SelectedKeywordSearchResult.URL;
            string separator = SelectedKeywordSearchResult.Separator;
            string format = SelectedKeywordSearchResult.Format;
            string defaultText = SelectedKeywordSearchResult.DefaultText;
            string alt = SelectedKeywordSearchResult.Alt;

            XmlDocument doc = XmlHelper.GetXmlDocument(ApplicationPaths.XmlUserKeywordFilename);
            XmlNode lastNode = XmlHelper.GetLastNode(doc);
            int id = lastNode is null ? 0 : int.Parse(lastNode.Attributes["ID"].Value) + 1;
            XmlNode parentNode = lastNode is null ? doc.SelectSingleNode(@"//Searches") : lastNode.ParentNode;

            XmlNode node = XmlHelper.MakeXmlNode(keyword, id, name, IconPath, url, separator, format, defaultText, alt);
            XmlNode importedNode = parentNode.OwnerDocument.ImportNode(node, true);
            parentNode.AppendChild(importedNode);
            XmlHelper.SaveXmlDocument(doc, ApplicationPaths.XmlUserKeywordFilename);

            SelectedKeywordSearchResult.Description = string.Format(format, defaultText);
            SelectedKeywordSearchResult.IsEnabled = true;
            UserKeywordSearchResults.Add(SelectedKeywordSearchResult);
            NotifyOfPropertyChange(() => SelectedKeywordSearchResult);
            NotifyOfPropertyChange(() => UserKeywordSearchResults);
            TryCloseAsync();
        }
    }
}
