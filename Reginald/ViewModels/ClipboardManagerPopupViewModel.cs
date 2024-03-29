﻿namespace Reginald.ViewModels
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
    using System.Windows.Media.Imaging;
    using Microsoft.Data.Sqlite;
    using Reginald.Core.DataExchange;
    using Reginald.Core.IO;
    using Reginald.Core.Services;
    using Reginald.Models.Inputs;
    using Reginald.Models.Products;

    internal sealed class ClipboardManagerPopupViewModel : SearchPopupViewModelScreen<ClipboardItem>
    {
        private const uint ClipboardExceptionCantOpen = 0x800401D0;

        private const string ClipboardFilename = "Clipboard.db";

        private const int ClipboardLimit = 25;

        private readonly List<ClipboardItem> _clipboardItems = new();

        public ClipboardManagerPopupViewModel()
        {
            // Creates clipboard database file.
            CreateClipboardDatabase();

            ClipboardListener.GetClipboardListener().ClipboardChanged += OnClipboardChanged;

            _clipboardItems.AddRange(ReadClipboardDatabase().Select(r => new ClipboardItem(r)));
            Items.CollectionChanged += OnCollectionChanged;
            Items.AddRange(_clipboardItems);
        }

        public void Clear()
        {
            _clipboardItems.Clear();
            Items.Clear();
            EmptyClipboardDatabase();
            SelectedItem = null;
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

        public void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not MenuItem menuItem)
            {
                return;
            }

            switch (menuItem.Tag)
            {
                case "Delete":
                    _ = _clipboardItems.Remove(SelectedItem);
                    _ = Items.Remove(SelectedItem);
                    EmptyClipboardDatabase();
                    CreateClipboardDatabase();
                    WriteToClipboardDatabase();
                    break;
            }
        }

        /// <summary>
        /// Drags the view.
        /// </summary>
        /// <param name="sender">The control that raised the event.</param>
        /// <param name="e">The event data.</param>
        public void Menu_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            WindowService.Drag();
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
                case Key.D1 when Keyboard.Modifiers is ModifierKeys.Control:
                case Key.D2 when Keyboard.Modifiers is ModifierKeys.Control:
                case Key.D3 when Keyboard.Modifiers is ModifierKeys.Control:
                case Key.D4 when Keyboard.Modifiers is ModifierKeys.Control:
                case Key.D5 when Keyboard.Modifiers is ModifierKeys.Control:
                case Key.D6 when Keyboard.Modifiers is ModifierKeys.Control:
                case Key.D7 when Keyboard.Modifiers is ModifierKeys.Control:
                case Key.D8 when Keyboard.Modifiers is ModifierKeys.Control:
                case Key.D9 when Keyboard.Modifiers is ModifierKeys.Control:
                    int index = e.Key - Key.D1;
                    if (index >= Items.Count || Items[index] is not ClipboardItem selectedItem)
                    {
                        break;
                    }

                    Hide();
                    selectedItem?.PressEnter(new InputProcessingEventArgs());
                    break;
                case Key.T when Keyboard.Modifiers is ModifierKeys.Control && !e.IsRepeat:
                    BorderOpacity = BorderOpacity == 1.0 ? DMS.Theme.BorderOpacity : 1.0;
                    break;
            }
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
                return;
            }

            List<ClipboardItem> items = new();
            for (int i = 0; i < _clipboardItems.Count; i++)
            {
                ClipboardItem item = _clipboardItems[i];
                if (item.Description.IndexOf(userInput, StringComparison.OrdinalIgnoreCase) != -1)
                {
                    items.Add(item);
                }
            }

            Items.AddRange(items);
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

        /// <summary>
        /// Attempts to query the Clipboard for the presence of data in the <see cref="DataFormats.Bitmap"/> data format.
        /// </summary>
        /// <param name="image">A <see cref="BitmapSource"/> containing the <see cref="DataFormats.Bitmap"/> data on the Clipboard.</param>
        /// <returns><see langword="true"/> if data in the <see cref="DataFormats.Bitmap"/> data format was successfully retrieved; otherwise <see langword="false"/>.</returns>
        /// <remarks>This is necessary due to a bug/feature in Terminal Services clipboard (and possible other things) and the .NET implementation of the clipboard. A delay in opening the clipboard causes the error, which usually passes within a few milliseconds. The solution is to try multiple times within a loop and sleep in between. This bug only affects WPF and not Windows Forms. See <see href="https://stackoverflow.com/questions/68666/clipbrd-e-cant-open-error-when-setting-the-clipboard-from-net">here</see> for more information.</remarks>
        private static bool TryGetImage(out BitmapSource image)
        {
            image = null;
            if (!Clipboard.ContainsImage())
            {
                return false;
            }

            for (int i = 0; i < 10; i++)
            {
                try
                {
                    image = Clipboard.GetImage();
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

        /// <summary>
        /// Attempts to query the Clipboard for the presence of data in the <see cref="DataFormats.UnicodeText"/> data format.
        /// </summary>
        /// <param name="text">A string containing the <see cref="DataFormats.UnicodeText"/> data on the Clipboard.</param>
        /// <returns><see langword="true"/> if data in the <see cref="DataFormats.UnicodeText"/> data format was successfully retrieved; otherwise <see langword="false"/>.</returns>
        /// <remarks>This is necessary due to a bug/feature in Terminal Services clipboard (and possible other things) and the .NET implementation of the clipboard. A delay in opening the clipboard causes the error, which usually passes within a few milliseconds. The solution is to try multiple times within a loop and sleep in between. This bug only affects WPF and not Windows Forms. See <see href="https://stackoverflow.com/questions/68666/clipbrd-e-cant-open-error-when-setting-the-clipboard-from-net">here</see> for more information.</remarks>
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
            // Prevents duplicate clipboard items.
            if (TryGetText(out string text))
            {
                for (int i = 0; i < _clipboardItems.Count; i++)
                {
                    ClipboardItem item = _clipboardItems[i];
                    if (text == item.Description)
                    {
                        _clipboardItems.RemoveAt(i);
                        _clipboardItems.Insert(0, item);
                        RefreshClipboardItems();
                        return;
                    }
                }

                _clipboardItems.Insert(0, new ClipboardItem(text));
            }
            else if (TryGetImage(out BitmapSource image))
            {
                for (int i = 0; i < _clipboardItems.Count; i++)
                {
                    ClipboardItem item = _clipboardItems[i];
                    if (image == item.Image)
                    {
                        _clipboardItems.RemoveAt(i);
                        _clipboardItems.Insert(0, item);
                        RefreshClipboardItems();
                        return;
                    }
                }

                _clipboardItems.Insert(0, new ClipboardItem(image));
            }
            else
            {
                return;
            }

            if (_clipboardItems.Count > ClipboardLimit)
            {
                _clipboardItems.RemoveAt(ClipboardLimit);
            }

            RefreshClipboardItems();

            void RefreshClipboardItems()
            {
                Items.Clear();
                Items.AddRange(_clipboardItems);

                EmptyClipboardDatabase();
                CreateClipboardDatabase();
                WriteToClipboardDatabase();
            }
        }

        private async void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            int count = Items.Count;
            if (count == 0)
            {
                return;
            }

            int limit = Math.Min(9, count);
            for (int i = 0; i < count; i++)
            {
                Items[i].KeyboardShortcut = i < limit ? "CTRL + " + (i + 1) : null;
            }

            await Task.Run(() => SelectedItem = Items[0]);
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
