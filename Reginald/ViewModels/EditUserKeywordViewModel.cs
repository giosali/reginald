using Caliburn.Micro;
using Reginald.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Xml;

namespace Reginald.ViewModels
{
    public class EditUserKeywordViewModel : Screen
    {
        protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            NotifyOfPropertyChange(() => UserKeywordSearchResults);
            NotifyOfPropertyChange(() => SelectedKeywordSearchResult);
            return base.OnDeactivateAsync(close, cancellationToken);
        }

        public EditUserKeywordViewModel(BindableCollection<SearchResultModel> collection, SearchResultModel model)
        {
            UserKeywordSearchResults = collection;
            SelectedKeywordSearchResult = model;
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
                    MessageBox.Show($"Images cannot be smaller than 75x75. This file's dimensions: {image.Width}x{image.Height}");
                }
                else
                {
                    string[] results = openFileDialog.FileName.Split(@"\");
                    string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Reginald", "UserIcons", results[^1]);
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
            SelectedKeywordSearchResult.Alt = ((TextBox)sender).Text;
            string format = SelectedKeywordSearchResult.Format;
            string defaultText = SelectedKeywordSearchResult.DefaultText;

            string appDataDirectoryPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string applicationName = "Reginald";
            string xmlUserKeywordFilename = "UserSearch.xml";
            string path = Path.Combine(appDataDirectoryPath, applicationName, xmlUserKeywordFilename);
            XmlDocument doc = new();
            doc.Load(path);
            XmlNode currentNode = GetCurrentNode(doc, SelectedKeywordSearchResult.ID);
            UpdateCurrentNode(currentNode);
            doc.Save(path);

            SelectedKeywordSearchResult.Description = String.Format(format, defaultText);
            NotifyOfPropertyChange(() => SelectedKeywordSearchResult);
            NotifyOfPropertyChange(() => UserKeywordSearchResults);
            UserKeywordSearchResults.Refresh();
            TryCloseAsync();
        }

        private static XmlNode GetCurrentNode(XmlDocument doc, int id)
        {
            string xpath = $"//Searches/Namespace[@ID='{id}']";
            return doc.SelectSingleNode(xpath);
        }

        private void UpdateCurrentNode(XmlNode currentNode)
        {
            currentNode["Name"].InnerText = SelectedKeywordSearchResult.Name;
            currentNode["Keyword"].InnerText = SelectedKeywordSearchResult.Keyword;
            if (IconPath is not null)
            {
                string iconPath = currentNode["Icon"].InnerText;
                File.Delete(iconPath);
                currentNode["Icon"].InnerText = IconPath;
            }
            currentNode["URL"].InnerText = SelectedKeywordSearchResult.URL;
            currentNode["Separator"].InnerText = SelectedKeywordSearchResult.Separator;
            currentNode["Format"].InnerText = SelectedKeywordSearchResult.Format;
            currentNode["DefaultText"].InnerText = SelectedKeywordSearchResult.DefaultText;
            currentNode["Alt"].InnerText = SelectedKeywordSearchResult.Alt;
        }
    }
}
