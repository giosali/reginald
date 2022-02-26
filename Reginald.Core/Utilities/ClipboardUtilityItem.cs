namespace Reginald.Core.Utilities
{
    using System;

    public class ClipboardUtilityItem
    {
        public ClipboardUtilityItem(string textData)
        {
            Text = textData;
            Date = DateTime.Now;
        }

        public string Text { get; set; }

        public DateTime Date { get; set; }
    }
}
