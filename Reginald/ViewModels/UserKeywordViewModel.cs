namespace Reginald.ViewModels
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Media.Imaging;
    using Caliburn.Micro;
    using Microsoft.Win32;
    using Reginald.Core.Extensions;
    using Reginald.Core.Helpers;
    using Reginald.Core.IO;
    using Reginald.Data.Keywords;
    using Reginald.Messages;

    public class UserKeywordViewModel : ItemViewModelBase, IHandle<ModifyItemMessage>
    {
        private const string UserIconsDirectoryName = "UserIcons";

        private const int MaxImageDimension = 128;

        private readonly IEventAggregator _eventAggregator;

        private GenericKeyword _selectedGenericKeyword;

        private bool _isBeingCreated;

        private bool _isBeingEdited;

        public UserKeywordViewModel(IEventAggregator eventAggregator)
            : base(GenericKeyword.UserKeywordsFilename, false, "Keywords > Your Keywords")
        {
            _eventAggregator = eventAggregator;
        }

        public GenericKeyword SelectedGenericKeyword
        {
            get => _selectedGenericKeyword;
            set
            {
                _selectedGenericKeyword = value;
                NotifyOfPropertyChange(() => SelectedGenericKeyword);
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

        public Task HandleAsync(ModifyItemMessage message, CancellationToken cancellationToken)
        {
            Guid guid = message.Guid;
            switch (message.ModificationType)
            {
                case ModificationType.ToggleIsEnabled:
                    {
                        GenericKeyword[] keywords = FileOperations.GetGenericData<GenericKeyword>(Filename, false);
                        for (int i = 0; i < keywords.Length; i++)
                        {
                            GenericKeyword keyword = keywords[i];
                            if (keyword.Guid == guid)
                            {
                                keyword.IsEnabled = !keyword.IsEnabled;
                                break;
                            }
                        }

                        FileOperations.WriteFile(Filename, keywords.Serialize());
                        break;
                    }

                case ModificationType.Edit:
                    {
                        IsBeingEdited = true;
                        GenericKeyword[] keywords = FileOperations.GetGenericData<GenericKeyword>(Filename, false);
                        SelectedGenericKeyword = keywords.Single(k => k.Guid == guid);
                        break;
                    }

                case ModificationType.Delete:
                    {
                        GenericKeyword[] keywords = FileOperations.GetGenericData<GenericKeyword>(Filename, false);

                        // Deletes the icon saved in the UserIcons directory.
                        GenericKeyword selectedKeyword = keywords.Single(k => k.Guid == guid);
                        if (selectedKeyword.Icon is not null)
                        {
                            Uri imageUri = new(selectedKeyword.Icon.ToString());
                            File.Delete(imageUri.LocalPath);
                        }

                        // Removes the keyword from the file.
                        FileOperations.WriteFile(Filename, keywords.Where(k => k.Guid != guid).Serialize());
                        _ = _eventAggregator.PublishOnUIThreadAsync(new UpdateItemsMessage(Filename, IsResource), cancellationToken: cancellationToken);
                        break;
                    }
            }

            return Task.CompletedTask;
        }

        public void CreateKeywordButton_Click(object sender, RoutedEventArgs e)
        {
            IsBeingCreated = true;
            SelectedGenericKeyword = new GenericKeyword
            {
                Guid = Guid.NewGuid(),
                Separator = "+",
                Placeholder = "...",
                IsEnabled = true,
            };
        }

        public void IconBorder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog openFileDialog = new()
            {
                Filter = "Image files (*.jpg, *.jpeg, *.png)|*.jpg;*.jpeg;*.png",
            };
            if (openFileDialog.ShowDialog() == true)
            {
                // Images smaller than the specified dimension will be rejected.
                using FileStream stream = File.OpenRead(openFileDialog.FileName);
                BitmapDecoder decoder = BitmapDecoder.Create(stream, BitmapCreateOptions.IgnoreColorProfile, BitmapCacheOption.Default);
                int width = decoder.Frames[0].PixelWidth;
                int height = decoder.Frames[0].PixelHeight;
                if (width < MaxImageDimension || height < MaxImageDimension)
                {
                    string message = $"Images cannot be smaller than 128x128. This file's dimensions: {width}x{height}";
                    string caption = "Image Is Too Small";
                    _ = MessageBox.Show(message, caption, MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    // Creates a directory in %AppData% for storing user icons.
                    string userIconsDirectoryPath = Path.Combine(FileOperations.ApplicationAppDataDirectoryPath, UserIconsDirectoryName);
                    _ = Directory.CreateDirectory(userIconsDirectoryPath);
                    string[] results = openFileDialog.FileName.Split(@"\");
                    string path = Path.Combine(userIconsDirectoryPath, results[^1]);
                    while (true)
                    {
                        try
                        {
                            File.Copy(openFileDialog.FileName, path);
                            break;
                        }
                        catch (IOException)
                        {
                            (string Stem, string Separator, string Extension) name = path.RPartition(".");
                            path = name.Stem + "_copy" + name.Separator + name.Extension;
                        }
                    }

                    // Assigns icon to selected keyword to display it in the form.
                    SelectedGenericKeyword.Icon = BitmapImageHelper.FromUri(path);
                }
            }
        }

        public void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            IsBeingCreated = IsBeingEdited = false;
        }

        public async void SaveKeywordButton_Click(object sender, RoutedEventArgs e)
        {
            GenericKeyword[] keywords = FileOperations.GetGenericData<GenericKeyword>(Filename, false);
            if (IsBeingEdited)
            {
                for (int i = 0; i < keywords.Length; i++)
                {
                    GenericKeyword keyword = keywords[i];
                    if (keyword.Guid == SelectedGenericKeyword.Guid)
                    {
                        keywords[i] = SelectedGenericKeyword;
                    }
                }
            }

            FileOperations.WriteFile(Filename, (IsBeingCreated ? keywords.Append(SelectedGenericKeyword) : keywords).Serialize());
            await _eventAggregator.PublishOnUIThreadAsync(new UpdateItemsMessage(Filename, IsResource));
            IsBeingCreated = IsBeingEdited = false;
        }

        protected override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            _eventAggregator.SubscribeOnUIThread(this);
            return base.OnActivateAsync(cancellationToken);
        }

        protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            _eventAggregator.Unsubscribe(this);
            return base.OnDeactivateAsync(close, cancellationToken);
        }
    }
}
