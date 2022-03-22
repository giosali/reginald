namespace Reginald.ViewModels
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using Caliburn.Micro;
    using Reginald.Core.Extensions;
    using Reginald.Core.IO;
    using Reginald.Data.Expansions;
    using Reginald.Services;

    public class ExpansionsViewModel : ScrollViewModelBase
    {
        private TextExpansion _selectedTextExpansion;

        private string _trigger;

        private string _replacement;

        private bool _isBeingEdited;

        public ExpansionsViewModel(ConfigurationService configurationService)
        {
            ConfigurationService = configurationService;
            IEnumerable<TextExpansion> textExpansions = FileOperations.GetGenericData<TextExpansion>(TextExpansion.Filename, false);
            TextExpansions.AddRange(textExpansions.OrderBy(e => e.Trigger));
        }

        public ConfigurationService ConfigurationService { get; set; }

        public BindableCollection<TextExpansion> TextExpansions { get; set; } = new();

        public TextExpansion SelectedTextExpansion
        {
            get => _selectedTextExpansion;
            set
            {
                _selectedTextExpansion = value;
                NotifyOfPropertyChange(() => SelectedTextExpansion);
            }
        }

        public string Trigger
        {
            get => _trigger;
            set
            {
                _trigger = value;
                NotifyOfPropertyChange(() => Trigger);
            }
        }

        public string Replacement
        {
            get => _replacement;
            set
            {
                _replacement = value;
                NotifyOfPropertyChange(() => Replacement);
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

        public void ScrollViewer_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!IsBeingEdited)
            {
                SelectedTextExpansion = null;
            }
        }

        public virtual void ExpansionsToggleButton_Click(object sender, RoutedEventArgs e)
        {
            ConfigurationService.Settings.Save();
        }

        public void TextExpansions_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            IsBeingEdited = true;
        }

        public void TextExpansions_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            IsBeingEdited = false;
        }

        public void TextExpansions_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!IsBeingEdited && SelectedTextExpansion is not null)
            {
                // Find out if there are any duplicate triggers
                bool exists = false;
                for (int i = 0; i < TextExpansions.Count; i++)
                {
                    TextExpansion expansion = TextExpansions[i];
                    for (int j = 0; j < TextExpansions.Count; j++)
                    {
                        if (i != j)
                        {
                            TextExpansion comparisonExpansion = TextExpansions[j];
                            if (expansion.Trigger == comparisonExpansion.Trigger)
                            {
                                exists = true;
                                break;
                            }
                        }
                    }

                    if (exists)
                    {
                        break;
                    }
                }

                // If there is a duplicate, inform user
                if (exists)
                {
                    string message = "Expansions cannot share the same trigger";
                    string title = "Error";
                    _ = HandyControl.Controls.MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    FileOperations.WriteFile(TextExpansion.Filename, TextExpansions.Serialize());
                }

                SelectedTextExpansion = null;

                // Reread from file to refresh the expansions
                TextExpansions.Clear();
                string filePath = FileOperations.GetFilePath(TextExpansion.Filename, false);
                TextExpansions.AddRange(FileOperations.GetGenericData<TextExpansion>(filePath));
            }
        }

        public void DeleteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (!IsBeingEdited && SelectedTextExpansion is not null)
            {
                if (TextExpansions.Remove(SelectedTextExpansion))
                {
                    FileOperations.WriteFile(TextExpansion.Filename, TextExpansions.Serialize());
                }
            }
        }

        public void AddButton_Click(object sender, RoutedEventArgs e)
        {
            // Verify that the trigger name doesn't already exist
            bool exists = false;
            for (int i = 0; i < TextExpansions.Count; i++)
            {
                TextExpansion expansion = TextExpansions[i];
                if (expansion.Trigger == Trigger)
                {
                    exists = true;
                    break;
                }
            }

            if (exists)
            {
                string message = "Expansions cannot share the same trigger";
                string title = "Error";
                _ = HandyControl.Controls.MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                TextExpansions.Add(new TextExpansion()
                {
                    Trigger = Trigger,
                    Replacement = Replacement,
                });

                FileOperations.WriteFile(TextExpansion.Filename, TextExpansions.Serialize());
                Trigger = Replacement = null;
            }
        }
    }
}
