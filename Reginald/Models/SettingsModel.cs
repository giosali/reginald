using System.Windows.Media;

namespace Reginald.Models
{
    public class SettingsModel
    {
        public bool IsDarkModeEnabled { get; set; }
        public Color SearchBackgroundColor { get; set; }
        public Brush SearchDescriptionTextBrush { get; set; }
        public Brush SearchAltTextBrush { get; set; }
        public Brush SearchInputTextBrush { get; set; }
        public Brush SearchInputCaretBrush { get; set; }
    }
}
