namespace Reginald.Models.Products
{
    using System;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using Microsoft.Data.Sqlite;
    using Reginald.Core.IO.Injection;
    using Reginald.Core.Services;
    using Reginald.Models.Drawing;
    using Reginald.Models.Inputs;

    internal sealed class ClipboardItem : KeyboardInput
    {
        private const int DescriptionLimit = 50;

        public ClipboardItem(BitmapSource bitmapSource)
        {
            Image = bitmapSource;
            Icon = new("72");
            DateTime = DateTime.Now;
            Description = $"{bitmapSource.Width}x{bitmapSource.Height}";
            ListBoxDescription = Description[..Math.Min(DescriptionLimit, Description.Length)];
        }

        public ClipboardItem(SqliteDataReader reader)
        {
            Icon = new("1");
            DateTime = DateTime.TryParse(reader["datetime"] as string, out DateTime dateTime) ? dateTime : DateTime.Now;
            if (reader["text"] is not string text)
            {
                ListBoxDescription = Description = string.Empty;
                return;
            }

            Description = text;
            try
            {
                string splitText = string.Join(' ', text.Split(Environment.NewLine, 5));
                ListBoxDescription = splitText[..Math.Min(DescriptionLimit, splitText.Length)];
            }
            catch (SystemException)
            {
                ListBoxDescription = text[..Math.Min(DescriptionLimit, text.Length)];
            }

            if (TryFromString(text, out Brush brush))
            {
                HexBrush = brush;
            }
        }

        public ClipboardItem(string text)
        {
            Icon = new("1");
            DateTime = DateTime.Now;
            if (text is null)
            {
                ListBoxDescription = Description = string.Empty;
                return;
            }

            Description = text;
            try
            {
                string splitText = string.Join(' ', text.Split(Environment.NewLine, 5));
                ListBoxDescription = splitText[..Math.Min(DescriptionLimit, splitText.Length)];
            }
            catch (SystemException)
            {
                ListBoxDescription = text[..Math.Min(DescriptionLimit, text.Length)];
            }

            if (TryFromString(text, out Brush brush))
            {
                HexBrush = brush;
            }
        }

        public DateTime DateTime { get; private set; }

        public string Description { get; private set; }

        public Brush HexBrush { get; private set; }

        public Icon Icon { get; private set; }

        public ImageSource Image { get; private set; }

        public string KeyboardShortcut { get; set; }

        public string ListBoxDescription { get; private set; }

        public override void PressAlt(InputProcessingEventArgs e)
        {
            OnAltKeyPressed(e);
        }

        public override void PressAltAndEnter(InputProcessingEventArgs e)
        {
            OnAltAndEnterKeysPressed(e);
        }

        public async override void PressEnter(InputProcessingEventArgs e)
        {
            OnEnterKeyPressed(e);

            // Ignores image-based ClipboardItems.
            if (Image is not null)
            {
                return;
            }

            // Pastes text to current active window.
            bool success = false;
            for (int i = 0; i < 10; i++)
            {
                try
                {
                    Clipboard.SetText(Description);
                    success = true;
                    break;
                }
                catch (COMException ex)
                {
                    // ClipboardExceptionCantOpen
                    if ((uint)ex.ErrorCode != 0x800401D0)
                    {
                        throw;
                    }

                    Thread.Sleep(10);
                }
            }

            if (!success)
            {
                return;
            }

            // Waits for clipboard window to deactivate.
            await WindowService.WaitForDeactivationAsync();

            // Simulates pasting through CTRL + V.
            InputInjector.InjectKeyboardInput(new InjectedInputKeyboardInfo(new VK[] { VK.CONTROL, VK.KEY_V }));
        }

        public override void PressTab(InputProcessingEventArgs e)
        {
            OnTabKeyPressed(e);
        }

        public override void ReleaseAlt(InputProcessingEventArgs e)
        {
            OnAltKeyReleased(e);
        }

        private static bool TryFromString(string expression, out Brush brush)
        {
            brush = null;
            int expressionLength = expression.Length;
            if (expressionLength < 6 || expressionLength > 7 || (expressionLength == 7 && !expression.StartsWith("#")))
            {
                return false;
            }

            if (expressionLength == 6)
            {
                expression = "#" + expression;
            }

            if (!int.TryParse(expression[1..], NumberStyles.HexNumber, null, out _))
            {
                return false;
            }

            try
            {
                brush = (Brush)new BrushConverter().ConvertFromString(expression);
            }
            catch (FormatException)
            {
                return false;
            }

            return true;
        }
    }
}
