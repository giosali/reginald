using System.Windows.Media;

namespace Reginald.Models
{
    public class SettingsModel
    {
        // SearchBoxAppearanceView
        public bool IsDefaultColorEnabled { get; set; }
        public bool IsSystemColorEnabled { get; set; }

        // UtilitiesView
        public bool IncludeInstalledApplications { get; set; }
        public bool IncludeDefaultKeywords { get; set; }
        public bool IncludeSpecialKeywords { get; set; }
        public bool IncludeCommands { get; set; }
        public bool IncludeUtilities { get; set; }
        public bool IncludeSettingsPages { get; set; }
    }
}
