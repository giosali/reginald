using Caliburn.Micro;
using Reginald.Core.DataModels;
using Reginald.Core.Helpers;
using Reginald.Core.IO;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Reginald.ViewModels
{
    public class ExpansionsViewModel : Screen
    {
        private BindableCollection<ExpansionDataModel> _expansions = new();
        public BindableCollection<ExpansionDataModel> Expansions
        {
            get => _expansions;
            set
            {
                _expansions = value;
                NotifyOfPropertyChange(() => Expansions);
            }
        }

        private ExpansionDataModel _selectedExpansion;
        public ExpansionDataModel SelectedExpansion
        {
            get => _selectedExpansion;
            set
            {
                _selectedExpansion = value;
                NotifyOfPropertyChange(() => SelectedExpansion);
            }
        }

        private string _trigger;
        public string Trigger
        {
            get => _trigger;
            set
            {
                _trigger = value;
                NotifyOfPropertyChange(() => Trigger);
            }
        }

        private string _replacement;
        public string Replacement
        {
            get => _replacement;
            set
            {
                _replacement = value;
                NotifyOfPropertyChange(() => Replacement);
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

        public ExpansionsViewModel()
        {
            List<ExpansionDataModel> expansions = FileOperations.GetExpansionData(ApplicationPaths.ExpansionsJsonFilename);
            if (expansions is not null)
            {
                Expansions.AddRange(expansions);
            }
        }

        public void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }

        public void ScrollViewer_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!IsBeingEdited)
            {
                SelectedExpansion = null;
            }
        }

        public void Expansions_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            IsBeingEdited = true;
        }

        public void Expansions_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            IsBeingEdited = false;
        }

        public void Expansions_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!IsBeingEdited && SelectedExpansion is not null)
            {
                // Find out if there are any duplicate triggers
                bool exists = false;
                for (int i = 0; i < Expansions.Count; i++)
                {
                    ExpansionDataModel expansion = Expansions[i];
                    for (int j = 0; j < Expansions.Count; j++)
                    {
                        if (i != j)
                        {
                            ExpansionDataModel comparisonExpansion = Expansions[j];
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

                // If there is a duplicate, inform user that's not allowed
                if (exists)
                {
                    string message = "Expansions cannot share the same trigger";
                    string title = "Error";
                    _ = HandyControl.Controls.MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    ExpansionDataModelHelper.Save(Expansions);
                }
                SelectedExpansion = null;

                // Reread from file to refresh the expansions
                Expansions.Clear();
                Expansions.AddRange(FileOperations.GetExpansionData(ApplicationPaths.ExpansionsJsonFilename));
            }
        }

        public void DeleteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (!IsBeingEdited && SelectedExpansion is not null)
            {
                if (Expansions.Remove(SelectedExpansion))
                {
                    ExpansionDataModelHelper.Save(Expansions);
                }
            }
        }

        public void AddButton_Click(object sender, RoutedEventArgs e)
        {
            // Verify that the trigger name doesn't already exist
            bool exists = false;
            for (int i = 0; i < Expansions.Count; i++)
            {
                ExpansionDataModel expansion = Expansions[i];
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
                Expansions.Add(new ExpansionDataModel()
                {
                    Trigger = Trigger,
                    Replacement = Replacement
                });

                ExpansionDataModelHelper.Save(Expansions);
                Trigger = Replacement = null;
            }
        }
    }
}
