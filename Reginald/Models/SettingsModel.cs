using System.Windows.Media;

namespace Reginald.Models
{
    public class SettingsModel
    {
        // SearchBoxAppearanceView
        public bool IsDefaultColorEnabled { get; set; }
        public bool IsSystemColorEnabled { get; set; }

        // AppearanceView
        public bool IsDarkModeEnabled { get; set; }
        public bool IsSearchBoxBorderEnabled { get; set; }

        // SearchView
        public Color SearchBackgroundColor { get; set; }
        public Brush SearchDescriptionTextBrush { get; set; }
        public Brush SearchAltTextBrush { get; set; }
        public Brush SearchInputTextBrush { get; set; }
        public Brush SearchInputCaretBrush { get; set; }
        public Brush SearchViewBorderBrush { get; set; }
        public Brush SpecialSearchResultSubBrush { get; set; }
        public Brush SpecialSearchResultBorderBrush { get; set; }
        public Brush SpecialSearchResultSecondaryBrush { get; set; }
        public Brush SpecialSearchResultMainBrush { get; set; }
        public Brush SearchResultHighlightColor { get; set; }

        // UtilitiesView
        public bool IncludeInstalledApplications { get; set; }
        public bool IncludeDefaultKeywords { get; set; }
        public bool IncludeSpecialKeywords { get; set; }
        public bool IncludeCommands { get; set; }
    }
}
