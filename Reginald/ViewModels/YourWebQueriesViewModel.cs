namespace Reginald.ViewModels
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media.Imaging;
    using Caliburn.Micro;
    using Microsoft.Win32;
    using Reginald.Core.Extensions;
    using Reginald.Core.IO;
    using Reginald.Data.DataModels;
    using Reginald.Services;

    internal class YourWebQueriesViewModel : ItemsScreen<WebQuery>
    {
        private const int MinImageDimension = 128;

        private const string UserIconsDirectoryName = "UserIcons";

        private bool _isBeingCreated;

        private bool _isBeingEdited;

        public YourWebQueriesViewModel(ConfigurationService configurationService)
            : base("Features > Your Web Queries")
        {
            ConfigurationService = configurationService;
        }

        public ConfigurationService ConfigurationService { get; set; }

        public bool IsBeingCreated
        {
            get => _isBeingCreated;
            set
            {
                _isBeingCreated = value;
                NotifyOfPropertyChange(() => IsBeingCreated);
            }
        }

        public bool IsBeingEdited
        {
            get => _isBeingEdited;
            set
            {
                _isBeingEdited = value;
                NotifyOfPropertyChange(() => IsBeingEdited);
            }
        }

        public void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedItem = null;
            IsBeingCreated = IsBeingEdited = false;
        }

        public void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            IsBeingCreated = true;
            SelectedItem = new WebQuery
            {
                Description = string.Empty,
                DescriptionFormat = string.Empty,
                EncodeInput = true,
                Guid = Guid.NewGuid(),
                IsCustom = true,
                IsEnabled = true,
                Placeholder = "...",
                Url = string.Empty,
                UrlFormat = string.Empty,
            };
        }

        public void IconBorder_MouseUp(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog openFileDialog = new();
            openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.png)|*.jpg;*.jpeg;*.png";
            if (openFileDialog.ShowDialog() == false)
            {
                return;
            }

            string filePath = openFileDialog.FileName;
            using FileStream stream = File.OpenRead(filePath);
            BitmapDecoder decoder = BitmapDecoder.Create(stream, BitmapCreateOptions.IgnoreColorProfile, BitmapCacheOption.Default);
            int width = decoder.Frames[0].PixelWidth;
            int height = decoder.Frames[0].PixelHeight;

            // Rejects images smaller than the minimum dimensions.
            if (width < MinImageDimension || height < MinImageDimension)
            {
                _ = MessageBox.Show($"Images cannot be smaller than 128x128. This file's dimensions: {width}x{height}", "Image Dimensions Are Too Small", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string path = Path.Combine(FileOperations.ApplicationAppDataDirectoryPath, UserIconsDirectoryName, Path.GetFileName(filePath));

            // Creates a directory in %APPDATA% for storing user icons.
            _ = Directory.CreateDirectory(Path.GetDirectoryName(path));
            if (File.Exists(path))
            {
                (string stem, string period, string extension) = path.RPartition(".");
                (string fileName, string hyphen, string number) = stem.RPartition(".");
                if (hyphen.Length != 0 && int.TryParse(number, out int n))
                {
                    path = fileName + hyphen + ++n + period + extension;
                }
                else
                {
                    path = stem + -1 + period + extension;
                }
            }

            File.Copy(openFileDialog.FileName, path);
            SelectedItem.IconPath = path;
        }

        public override void IsEnabled_Click(object sender, RoutedEventArgs e)
        {
            FileOperations.WriteFile(WebQuery.UserFileName, Items.Serialize());
        }

        public void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not MenuItem menuItem)
            {
                return;
            }

            switch (menuItem.Tag)
            {
                case "Edit":
                    IsBeingEdited = true;
                    break;
                case "Delete":
                    Items.Remove(SelectedItem);
                    FileOperations.WriteFile(WebQuery.UserFileName, Items.Serialize());
                    SelectedItem = null;
                    break;
            }
        }

        public void SaveKeywordButton_Click(object sender, RoutedEventArgs e)
        {
            if (IsBeingCreated)
            {
                Items.Add(SelectedItem);
            }

            SelectedItem = null;
            FileOperations.WriteFile(WebQuery.UserFileName, Items.Serialize());
            IsBeingCreated = IsBeingEdited = false;
        }

        protected override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            DataModelService dataModelService = IoC.Get<DataModelService>();
            Items.AddRange(dataModelService.SingleProducers
                                           .Where(sp => sp is WebQuery wq && wq.IsCustom)
                                           .Select(sp =>
                                           {
                                               WebQuery wq = sp as WebQuery;
                                               if (string.IsNullOrEmpty(wq.Description))
                                               {
                                                   wq.Description = string.Format(wq.DescriptionFormat ?? string.Empty, wq.Placeholder ?? string.Empty);
                                               }

                                               return wq;
                                           }));
            return base.OnActivateAsync(cancellationToken);
        }

        protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            Items.Clear();
            return base.OnDeactivateAsync(close, cancellationToken);
        }
    }
}
