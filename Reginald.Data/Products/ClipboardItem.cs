namespace Reginald.Data.Products
{
    using System;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using Microsoft.Data.Sqlite;
    using Reginald.Core.Helpers;
    using Reginald.Data.Drawing;
    using Reginald.Data.Inputs;

    public class ClipboardItem : KeyboardInput
    {
        private string _description;

        private Icon _icon;

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
            if (BrushHelper.TryFromString(text, out Brush brush))
            {
                HexBrush = brush;
            }
        }

        public ClipboardItem(string description)
        {
            Icon = new("1");
            Description = description;
            DateTime = DateTime.Now;
            if (BrushHelper.TryFromString(description, out Brush brush))
            {
                HexBrush = brush;
            }
        }

        public DateTime DateTime { get; set; }

        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                NotifyOfPropertyChange(() => Description);
            }
        }

        public Brush HexBrush { get; set; }

        public Icon Icon
        {
            get => _icon;
            set
            {
                _icon = value;
                NotifyOfPropertyChange(() => Icon);
            }
        }

        public ImageSource Image { get; set; }

        public override void PressAlt(InputProcessingEventArgs e)
        {
            OnAltKeyPressed(e);
        }
        
        public override void PressAltAndEnter(InputProcessingEventArgs e)
        {
            OnAltAndEnterKeysPressed(e);
        }

        public override void PressEnter(InputProcessingEventArgs e)
        {
            OnEnterKeyPressed(e);
        }

        public override void PressTab(InputProcessingEventArgs e)
        {
            OnTabKeyPressed(e);
        }

        public override void ReleaseAlt(InputProcessingEventArgs e)
        {
            OnAltKeyReleased(e);
        }
    }
}
