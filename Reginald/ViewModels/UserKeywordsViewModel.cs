using Reginald.Core.AbstractProducts;
using Reginald.Core.DataModels;
using Reginald.Core.Extensions;
using Reginald.Core.Helpers;
using Reginald.Core.IO;
using Reginald.Core.Products;
using Reginald.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Reginald.ViewModels
{
    public class UserKeywordsViewModel : KeywordViewModelBase
    {
        private GenericKeyword _selectedGenericKeyword;
        public GenericKeyword SelectedGenericKeyword
        {
            get => _selectedGenericKeyword;
            set
            {
                _selectedGenericKeyword = value;
                NotifyOfPropertyChange(() => SelectedGenericKeyword);
            }
        }

        private Visibility _visibility = Visibility.Collapsed;
        public Visibility Visibility
        {
            get => _visibility;
            set
            {
                _visibility = value;
                NotifyOfPropertyChange(() => Visibility);
            }
        }

        private bool _isBeingEdited;
        public bool IsBeingEdited
        {
            get => _isBeingEdited;
            set
            {
                _isBeingEdited = value;
                NotifyOfPropertyChange(() => IsBeingEdited);
            }
        }

        public UserKeywordsViewModel() : base(ApplicationPaths.UserKeywordsJsonFilename)
        {
            IEnumerable<Keyword> keywords = UpdateKeywords<GenericKeywordDataModel>(Filename, false, false);
            foreach (Keyword keyword in keywords)
            {
                keyword.Description = string.Format(keyword.Format, keyword.Placeholder);
            }
            Keywords.AddRange(keywords);
        }

        public override void KeywordIsEnabled_Click(object sender, RoutedEventArgs e)
        {
            FileOperations.WriteFile(Filename, Keywords.OfType<GenericKeyword>()
                                                       .Serialize());
        }

        public void DeleteKeywordButton_Click(object sender, RoutedEventArgs e)
        {
            string message = $"Are you sure you would like to delete the '{SelectedKeyword.Name}' keyword?";
            string caption = "Confirmation Required";
            MessageBoxResult result = HandyControl.Controls.MessageBox.Show(message, caption, MessageBoxButton.OKCancel, MessageBoxImage.Warning);

            switch (result)
            {
                case MessageBoxResult.OK:
                    if (SelectedKeyword.Icon is not null)
                    {
                        Uri imageUri = new(SelectedKeyword.Icon.ToString());
                        File.Delete(imageUri.LocalPath);
                    }
                    _ = Keywords.Remove(SelectedKeyword);
                    FileOperations.WriteFile(Filename, Keywords.OfType<GenericKeyword>()
                                                               .Serialize());
                    break;
            }
        }

        public void EditKeywordButton_Click(object sender, RoutedEventArgs e)
        {
            IsBeingEdited = true;
            Visibility = Visibility.Visible;
            GenericKeyword selectedGenericKeyword = SelectedKeyword as GenericKeyword;
            SelectedGenericKeyword = new GenericKeyword
            {
                Guid = selectedGenericKeyword.Guid,
                Name = selectedGenericKeyword.Name,
                Word = selectedGenericKeyword.Word,
                Url = selectedGenericKeyword.Url,
                Separator = selectedGenericKeyword.Separator,
                Format = selectedGenericKeyword.Format,
                Placeholder = selectedGenericKeyword.Placeholder,
                Description = string.Format(selectedGenericKeyword.Format, selectedGenericKeyword.Placeholder),
                Caption = selectedGenericKeyword.Caption,
                Icon = selectedGenericKeyword.Icon,
                AltDescription = selectedGenericKeyword.AltDescription,
                AltUrl = selectedGenericKeyword.AltUrl,
                IsEnabled = selectedGenericKeyword.IsEnabled
            };
        }

        public void CreateKeywordButton_Click(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Visible;
            SelectedGenericKeyword = new GenericKeyword
            {
                Guid = Guid.NewGuid(),
                Separator = "+",
                Placeholder = "...",
                IsEnabled = true
            };
        }

        public void SeparatorComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem comboBoxItem = (sender as ComboBox).SelectedItem as ComboBoxItem;
            if (comboBoxItem is not null)
            {
                SelectedGenericKeyword.Separator = comboBoxItem.Content.ToString();
            }
        }

        public void SeparatorComboBox_KeyUp(object sender, KeyEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            SelectedGenericKeyword.Separator = comboBox.Text;
        }

        public void FormatTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                string result = string.Format(SelectedGenericKeyword.Format, SelectedGenericKeyword.Placeholder);
                SelectedGenericKeyword.Description = result;
            }
            catch (Exception ex)
            {
                if (ex is FormatException or ArgumentNullException)
                {
                    SelectedGenericKeyword.Description = null;
                }
                else
                {
                    throw;
                }
            }
        }

        public void PlaceholderComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem comboBoxItem = (sender as ComboBox).SelectedItem as ComboBoxItem;
            if (comboBoxItem is not null)
            {
                SelectedGenericKeyword.Placeholder = comboBoxItem.Content.ToString();
                FormatTextBox_TextChanged(null, null);
            }
        }

        public void PlaceholderComboBox_KeyUp(object sender, KeyEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            SelectedGenericKeyword.Placeholder = comboBox.Text;
            FormatTextBox_TextChanged(null, null);
        }

        public void IconBorder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new();
            openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.png)|*.jpg;*.jpeg;*.png";
            if (openFileDialog.ShowDialog() == true)
            {
                // Images smaller than 75x75 will be rejected
                System.Drawing.Image image = System.Drawing.Image.FromFile(openFileDialog.FileName);
                if (image.Width < 128 || image.Height < 128)
                {
                    string message = $"Images cannot be smaller than 128x128. This file's dimensions: {image.Width}x{image.Height}";
                    string caption = "Image Is Too Small";
                    _ = HandyControl.Controls.MessageBox.Show(message, caption, MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    string[] results = openFileDialog.FileName.Split(@"\");
                    string path = Path.Combine(ApplicationPaths.AppDataDirectoryPath, ApplicationPaths.ApplicationName, ApplicationPaths.UserIconsDirectoryName, results[^1]);
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
                    SelectedGenericKeyword.Icon = BitmapImageHelper.FromUri(path);
                }
            }
        }

        public void SaveKeywordButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(SelectedGenericKeyword.Name)
                && !string.IsNullOrEmpty(SelectedGenericKeyword.Word)
                && !string.IsNullOrEmpty(SelectedGenericKeyword.Url)
                && !string.IsNullOrEmpty(SelectedGenericKeyword.Format)
                && !string.IsNullOrEmpty(SelectedGenericKeyword.Placeholder)
                && !string.IsNullOrEmpty(SelectedGenericKeyword.Caption))
            {
                // If we're editing a pre-existing keyword, replace the
                // old version with the new version
                if (IsBeingEdited)
                {
                    int index = Keywords.IndexOf(SelectedGenericKeyword);
                    Keywords.RemoveAt(index);
                    Keywords.Insert(index, SelectedGenericKeyword);
                }
                else // Otherwise, save our new keyword to our current keywords
                {
                    Keywords.Add(SelectedGenericKeyword);
                }

                IsBeingEdited = false;
                Visibility = Visibility.Collapsed;
                FileOperations.WriteFile(Filename, Keywords.OfType<GenericKeyword>()
                                                           .Serialize());
            }
            else
            {
                string message = "There are one or more required fields that have been left blank. Please fill them in to save this keyword.";
                string caption = "Required Fields Left Blank";
                _ = HandyControl.Controls.MessageBox.Show(message, caption, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            IsBeingEdited = false;
            Visibility = Visibility.Collapsed;
        }
    }
}
