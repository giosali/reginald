namespace Reginald.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using Microsoft.Data.Sqlite;
    using Reginald.Core.IO;
    using Reginald.Data.Inputs;
    using Reginald.Data.Products;
    using Reginald.Services;
    using Reginald.Services.Utilities;

    public class ClipboardManagerPopupViewModel : SearchPopupViewModelScreen<ClipboardItem>
    {
        private const uint ClipboardExceptionCantOpen = 0x800401D0;

        private const string ClipboardFilename = "Clipboard.db";

        private const int ClipboardLimit = 25;

        private readonly List<ClipboardItem> _clipboardItems = new();

        public ClipboardManagerPopupViewModel(ConfigurationService configurationService)
            : base(configurationService)
        {
            // Creates clipboard database file.
            CreateClipboardDatabase();

            ClipboardUtility utility = ClipboardUtility.GetClipboardUtility();
            utility.ClipboardChanged += OnClipboardChanged;

            _clipboardItems.AddRange(ReadClipboardDatabase().Select(r => new ClipboardItem(r)));
            Items.AddRange(_clipboardItems);
            Items.CollectionChanged += OnCollectionChanged;
        }

        public void Item_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Hide();
            SelectedItem?.PressEnter(new InputProcessingEventArgs());
        }

        public void Items_MouseLeave(object sender, MouseEventArgs e)
        {
            if (Items.Count == 0)
            {
                return;
            }

            SelectedItem = Items.Contains(LastSelectedItem) ? LastSelectedItem : Items[0];
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

        public void UserInput_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    Hide();
                    SelectedItem?.PressEnter(new InputProcessingEventArgs());
                    break;
                case Key.Up:
                    SelectedItem = Items[Math.Max(Items.IndexOf(SelectedItem) - 1, 0)];
                    IsMouseOverChanged = false;
                    break;
                case Key.Down:
                    SelectedItem = Items[Math.Min(Items.IndexOf(SelectedItem) + 1, Items.Count - 1)];
                    IsMouseOverChanged = false;
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

        public void UserInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            Items.Clear();
            IsMouseOverChanged = false;
            MousePosition = default;

            string userInput = UserInput;
            if (userInput.Length == 0)
            {
                Items.AddRange(_clipboardItems);
            }

            Items.AddRange(_clipboardItems.Where(i => i.Description.Contains(userInput, StringComparison.OrdinalIgnoreCase)));
        }

        protected override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            // Resets index of selected item to 0 if the selected itme is null
            // and if there are items.
            if (Items?.Count > 0)
            {
                SelectedItem = Items[0];
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
                    text TEXT,
                    datetime TEXT
                );
            ";
            command.ExecuteNonQuery();
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

        private static bool TryGetText(out string text)
        {
            text = null;
            if (!Clipboard.ContainsText())
            {
                return false;
            }

            for (int i = 0; i < 10; i++)
            {
                try
                {
                    text = Clipboard.GetText();
                    return true;
                }
                catch (COMException ex)
                {
                    if ((uint)ex.ErrorCode != ClipboardExceptionCantOpen)
                    {
                        throw;
                    }

                    Thread.Sleep(10);
                }
            }

            return false;
        }

        private void OnClipboardChanged(object sender, EventArgs e)
        {
            if (Clipboard.ContainsImage())
            {
                _clipboardItems.Insert(0, new ClipboardItem(Clipboard.GetImage()));
                return;
            }

            // Prevents duplicate clipboard items.
            if (!TryGetText(out string text) || text == _clipboardItems[0].Description)
            {
                return;
            }

            _clipboardItems.Insert(0, new ClipboardItem(text));
            if (_clipboardItems.Count > ClipboardLimit)
            {
                _clipboardItems.RemoveAt(ClipboardLimit);
            }

            Items.Clear();
            Items.AddRange(_clipboardItems);

            EmptyClipboardDatabase();
            CreateClipboardDatabase();
            WriteToClipboardDatabase();
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (Items.Count == 0)
            {
                return;
            }

            SelectedItem = Items[0];
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
                INSERT INTO 'clipboard'(id, text, datetime)
                VALUES($id, $text, $datetime)
            ";

            SqliteParameter idParameter = command.CreateParameter();
            idParameter.ParameterName = "$id";
            command.Parameters.Add(idParameter);

            SqliteParameter textParameter = command.CreateParameter();
            textParameter.ParameterName = "$text";
            command.Parameters.Add(textParameter);

            SqliteParameter datetimeParameter = command.CreateParameter();
            datetimeParameter.ParameterName = "$datetime";
            command.Parameters.Add(datetimeParameter);

            for (int i = 0; i < Items.Count; i++)
            {
                ClipboardItem item = Items[i];
                if (item.Image is not null)
                {
                    continue;
                }

                idParameter.Value = i + 1;
                textParameter.Value = item.Description;
                datetimeParameter.Value = item.DateTime.ToString();
                command.ExecuteNonQuery();
            }

            transaction.Commit();
        }
    }
}
