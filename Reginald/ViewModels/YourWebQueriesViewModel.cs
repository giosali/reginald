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
    using Reginald.Models.DataModels;
    using Reginald.Services;

    internal class YourWebQueriesViewModel : ItemsScreen<WebQuery>
    {
        private const int MinImageDimension = 128;

        private const string UserIconsDirectoryName = "UserIcons";

        private string _iconPath;

        private bool _isBeingCreated;

        private bool _isBeingEdited;

        private string _tempIconPath;

        public YourWebQueriesViewModel()
            : base("Features > Your Web Queries")
        {
        }

        public string IconPath
        {
            get => _iconPath;
            set
            {
                _iconPath = value;
                NotifyOfPropertyChange(() => IconPath);
            }
        }

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
            IconPath = _tempIconPath = null;
            IsBeingCreated = IsBeingEdited = false;
        }

        public void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            IsBeingCreated = true;
            SelectedItem = new WebQuery
            {
                Caption = string.Empty,
                Description = string.Empty,
                DescriptionFormat = string.Empty,
                EncodeInput = true,
                Guid = Guid.NewGuid(),
                IsCustom = true,
                IsEnabled = true,
                Key = string.Empty,
                Name = string.Empty,
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

            IconPath = _tempIconPath = filePath;
        }

        public override void IsEnabled_Click(object sender, RoutedEventArgs e)
        {
            DataModelService dms = IoC.Get<DataModelService>();
            if (SelectedItem.IsEnabled)
            {
                _ = dms.Settings.DisabledWebQueries.Remove(SelectedItem.Guid);
            }
            else
            {
                dms.Settings.DisabledWebQueries.Add(SelectedItem.Guid);
            }

            dms.Settings.Save();
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
                    _tempIconPath = null;
                    IconPath = SelectedItem.IconPath;
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
            if (!string.IsNullOrEmpty(_tempIconPath))
            {
                // Creates a directory in %APPDATA% for storing user icons.
                string directory = Path.Combine(FileOperations.ApplicationAppDataDirectoryPath, UserIconsDirectoryName);
                _ = Directory.CreateDirectory(directory);

                string path = Path.Combine(directory, Path.GetFileName(_tempIconPath));
                while (File.Exists(path))
                {
                    (string fileName, string hyphen, string number) = Path.GetFileNameWithoutExtension(path).RPartition("-");
                    if (hyphen.Length != 0 && int.TryParse(number, out int n))
                    {
                        path = Path.Combine(directory, fileName + hyphen + ++n + Path.GetExtension(path));
                    }
                    else
                    {
                        path = Path.Combine(directory, fileName + -1 + Path.GetExtension(path));
                    }
                }

                File.Copy(_tempIconPath, path);
                SelectedItem.IconPath = path;
            }

            if (IsBeingCreated)
            {
                Items.Add(SelectedItem);
            }

            FileOperations.WriteFile(WebQuery.UserFileName, Items.Serialize());
            SelectedItem = null;
            IsBeingCreated = IsBeingEdited = false;
            IconPath = _tempIconPath = null;
        }

        protected override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            DataModelService dms = IoC.Get<DataModelService>();
            Items.AddRange(dms.SingleProducers
                              .Where(sp => sp is WebQuery wq && wq.IsCustom)
                              .Select(sp => sp as WebQuery));
            return base.OnActivateAsync(cancellationToken);
        }

        protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            Items.Clear();
            return base.OnDeactivateAsync(close, cancellationToken);
        }
    }
}
