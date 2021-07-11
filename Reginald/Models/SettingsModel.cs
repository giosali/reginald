using System.Windows.Media;

namespace Reginald.Models
{
    public class SettingsModel
    {
        // AppearanceView
        public bool IsDarkModeEnabled { get; set; }

        // SearchView
        public Color SearchBackgroundColor { get; set; }
        public Brush SearchDescriptionTextBrush { get; set; }
        public Brush SearchAltTextBrush { get; set; }
        public Brush SearchInputTextBrush { get; set; }
        public Brush SearchInputCaretBrush { get; set; }

        // UtilitiesView
        public bool IncludeInstalledApplications { get; set; }
        public bool IncludeDefaultKeywords { get; set; }
    }
}
