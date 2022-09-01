namespace Reginald.Data.Products
{
    using System;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using Microsoft.Data.Sqlite;
    using Reginald.Data.Drawing;
    using Reginald.Data.Inputs;
    using Reginald.Services.Input;
    using Reginald.Services.Utilities;

    public class ClipboardItem : KeyboardInput
    {
        public ClipboardItem(BitmapSource bitmapSource)
        {
            Image = bitmapSource;
            Icon = new("72");
            Description = $"{bitmapSource.Width}x{bitmapSource.Height}";
            DateTime = DateTime.Now;
        }

        public ClipboardItem(SqliteDataReader reader)
        {
            Icon = new("1");
            DateTime = DateTime.TryParse(reader["datetime"] as string, out DateTime dateTime) ? dateTime : DateTime.Now;
            string text = reader["text"] as string;
            Description = text;
            if (TryFromString(text, out Brush brush))
            {
                HexBrush = brush;
            }
        }

        public ClipboardItem(string description)
        {
            Icon = new("1");
            Description = description;
            DateTime = DateTime.Now;
            if (TryFromString(description, out Brush brush))
            {
                HexBrush = brush;
            }
        }

        public DateTime DateTime { get; set; }

        public string Description { get; set; }

        public Brush HexBrush { get; set; }

        public Icon Icon { get; set; }

        public ImageSource Image { get; set; }

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
            await WindowUtility.WaitForDeactivationAsync();

            KeyboardInputInjector.Paste();
        }

        public override void PressTab(InputProcessingEventArgs e)
        {
            OnTabKeyPressed(e);
        }

        public override void ReleaseAlt(InputProcessingEventArgs e)
        {
            OnAltKeyReleased(e);
        }

        public static bool TryFromString(string expression, out Brush brush)
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
