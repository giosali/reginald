using Caliburn.Micro;
using HandyControl.Data;
using Reginald.Core.Helpers;
using Reginald.Core.IO;
using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Xml;

namespace Reginald.ViewModels
{
    public class CustomKeywordViewModel : KeywordViewModel
    {
        public CustomKeywordViewModel(string filename, bool overrideMethod = false) : base(filename, overrideMethod)
        {

        }

        protected string IconPath { get; set; }

        public void DeleteKeyword_Click(object sender, RoutedEventArgs e)
        {
            string message = $"Are you sure you would like to delete the '{SelectedKeywordSearchResult.Name}' keyword?";
            string caption = "Confirmation Required";
            MessageBoxResult result = HandyControl.Controls.MessageBox.Show(new MessageBoxInfo
            {
                Message = message,
                Caption = caption,
                Button = MessageBoxButton.OKCancel,
                IconBrushKey = ResourceToken.AccentBrush,
                IconKey = ResourceToken.AskGeometry
            });

            switch (result)
            {
                case MessageBoxResult.OK:
                    XmlDocument doc = XmlHelper.GetXmlDocument(Filename);
                    XmlNode currentNode = XmlHelper.GetCurrentNodeFromID(doc, SelectedKeywordSearchResult.ID);
                    string iconPath = currentNode["Icon"].InnerText;
                    if (File.Exists(iconPath))
                    {
                        File.Delete(iconPath);
                    }
                    _ = currentNode.ParentNode.RemoveChild(currentNode);
                    XmlHelper.SaveXmlDocument(doc, Filename);
                    _ = KeywordSearchResults.Remove(SelectedKeywordSearchResult);
                    break;

                case MessageBoxResult.Cancel:
                    break;

                default:
                    break;
            }
        }

        public async void EditKeyword_ClickAsync(object sender, RoutedEventArgs e)
        {
            IWindowManager manager = new WindowManager();
            await manager.ShowWindowAsync(new EditUserKeywordViewModel(KeywordSearchResults, SelectedKeywordSearchResult));
        }

        public async void CreateKeyword_ClickAsync(object sender, RoutedEventArgs e)
        {
            IWindowManager manager = new WindowManager();
            await manager.ShowWindowAsync(new CreateUserKeywordViewModel(KeywordSearchResults));
        }

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

        public virtual void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
