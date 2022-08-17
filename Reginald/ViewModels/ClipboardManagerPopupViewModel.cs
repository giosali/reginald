namespace Reginald.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Input;
    using Caliburn.Micro;
    using Microsoft.Data.Sqlite;
    using Reginald.Core.Helpers;
    using Reginald.Core.IO;
    using Reginald.Data.Comparers;
    using Reginald.Data.DisplayItems;
    using Reginald.Services;
    using Reginald.Services.Utilities;

    public class ClipboardManagerPopupViewModel : SearchPopupViewModelScreen<ClipboardItem>
    {
        private const string ClipboardFilename = "Clipboard.db";

        private const int ClipboardLimit = 25;

        private string _userInput;

        public ClipboardManagerPopupViewModel(ConfigurationService configurationService)
            : base(configurationService)
        {
            // Creates clipboard database file.
            CreateClipboardDatabase();

            ClipboardUtility utility = ClipboardUtility.GetClipboardUtility();
            utility.ClipboardChanged += OnClipboardChanged;

            Items = new(ReadClipboardDatabase().Select(r => new ClipboardItem(r)));
            Items.CollectionChanged += OnCollectionChanged;
        }

        public override string UserInput
        {
            get => _userInput;
            set
            {
                _userInput = value;
                NotifyOfPropertyChange(() => UserInput);
                NotifyOfPropertyChange(() => DisplayItems);
                OnCollectionChanged(null, null);
            }
        }

        public BindableCollection<ClipboardItem> DisplayItems => new(Items.Where(item =>
        {
            if (!string.IsNullOrEmpty(UserInput))
            {
                return item.Description.Contains(UserInput, StringComparison.OrdinalIgnoreCase);
            }

            return true;
        }));

        public void UserInput_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Up:
                    SelectedItem = DisplayItems[Math.Max(DisplayItems.IndexOf(SelectedItem) - 1, 0)];
                    IsMouseOverChanged = false;
                    break;

                case Key.Down:
                    SelectedItem = DisplayItems[Math.Min(DisplayItems.IndexOf(SelectedItem) + 1, DisplayItems.Count - 1)];
                    IsMouseOverChanged = false;
                    break;

                case Key.Enter:
                    SelectedItem.EnterKeyDown();
                    Hide();
                    break;
            }
        }

        public void UserInput_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            // Prevents premature input from appearing. For example, if the user
            // binds the clipboard manager to Control + Space and we didn't set e.Handled
            // to true, there will be a starting space character in the text each time
            // the user activates the clipboard manager by pressing those keys.
            e.Handled = true;
        }

        /// <summary>
        /// Drags the view.
        /// </summary>
        /// <param name="sender">The control that raised the event.</param>
        /// <param name="e">The event data.</param>
        public void Menu_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            WindowUtility.Drag();
        }

        public void PopupCloseBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Hide();
        }

        public void Item_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // Keeps a copy of the SelectedItem because SelectedItem will be null
            // after calling Hide.
            ClipboardItem selectedItem = SelectedItem;
            if (selectedItem is not null)
            {
                Hide();
                selectedItem.EnterKeyDown();
            }
        }

        public void Items_MouseLeave(object sender, MouseEventArgs e)
        {
            try
            {
                SelectedItem = DisplayItems.Contains(LastSelectedItem) ? LastSelectedItem : DisplayItems[0];
            }
            catch (ArgumentOutOfRangeException)
            {
            }
        }

        protected override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            // Resets index of selected item to 0 if there are any items
            if (DisplayItems?.Count > 0)
            {
                SelectedItem = DisplayItems[0];
            }

            return base.OnActivateAsync(cancellationToken);
        }

        protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            UserInput = string.Empty;
            return base.OnDeactivateAsync(close, cancellationToken);
        }

        private static void CreateClipboardDatabase()
        {
            string filePath = FileOperations.GetFilePath(ClipboardFilename);
            using SqliteConnection connection = new($"Data Source={filePath}");
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText =
            @"
                CREATE TABLE IF NOT EXISTS 'clipboard'(
                    id INTEGER PRIMARY KEY,
                    type INTEGER NOT NULL,
                    text TEXT,
                    icon BLOB,
                    datetime TEXT
                );
            ";
            command.ExecuteNonQuery();
        }

        private static IEnumerable<SqliteDataReader> ReadClipboardDatabase()
        {
            string filePath = FileOperations.GetFilePath(ClipboardFilename);
            using SqliteConnection connection = new($"Data Source={filePath}");
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText =
            @"
                SELECT * FROM 'clipboard'
            ";
            SqliteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                yield return reader;
            }
        }

        private static void EmptyClipboardDatabase()
        {
            string filePath = FileOperations.GetFilePath(ClipboardFilename);
            using SqliteConnection connection = new($"Data Source={filePath}");
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText =
            @"
                DROP TABLE IF EXISTS 'clipboard'
            ";
            command.ExecuteNonQuery();
        }

        private void OnClipboardChanged(object sender, EventArgs e)
        {
            if (ClipboardHelper.TryGetText(out string text))
            {
                Items.Insert(0, new ClipboardItem(text));
                if (Items.Count > ClipboardLimit)
                {
                    Items.RemoveAt(ClipboardLimit - 1);
                }

                Items = new(Items.Distinct(new ClipboardItemComparer()));
                EmptyClipboardDatabase();
                CreateClipboardDatabase();
                WriteToClipboardDatabase();
            }
            else if (Clipboard.ContainsImage())
            {
                Items.Insert(0, new ClipboardItem(Clipboard.GetImage()));
            }
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (DisplayItems.Count > 0)
            {
                SelectedItem = DisplayItems[0];
            }
        }

        private void WriteToClipboardDatabase()
        {
            string filePath = FileOperations.GetFilePath(ClipboardFilename);
            using SqliteConnection connection = new($"Data Source={filePath}");
            connection.Open();
            using SqliteTransaction transaction = connection.BeginTransaction();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText =
            @"
                INSERT INTO 'clipboard'(id, type, icon, text, datetime)
                VALUES($id, $type, $icon, $text, $datetime)
            ";

            SqliteParameter idParameter = command.CreateParameter();
            idParameter.ParameterName = "$id";
            command.Parameters.Add(idParameter);

            SqliteParameter typeParameter = command.CreateParameter();
            typeParameter.ParameterName = "$type";
            command.Parameters.Add(typeParameter);

            SqliteParameter iconParameter = command.CreateParameter();
            iconParameter.ParameterName = "$icon";
            command.Parameters.Add(iconParameter);

            SqliteParameter textParameter = command.CreateParameter();
            textParameter.ParameterName = "$text";
            command.Parameters.Add(textParameter);

            SqliteParameter datetimeParameter = command.CreateParameter();
            datetimeParameter.ParameterName = "$datetime";
            command.Parameters.Add(datetimeParameter);

            for (int i = 0; i < Items.Count; i++)
            {
                ClipboardItem item = Items[i];
                if (item.ClipboardItemType == ClipboardItemType.Text)
                {
                    idParameter.Value = i + 1;
                    typeParameter.Value = ClipboardItemType.Text;
                    iconParameter.Value = item.Icon.ToString();
                    textParameter.Value = item.Description;
                    datetimeParameter.Value = item.DateTime.ToString();
                    command.ExecuteNonQuery();
                }
            }

            transaction.Commit();
        }
    }
}
