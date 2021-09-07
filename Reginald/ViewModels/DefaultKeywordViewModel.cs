using Reginald.Core.IO;
using System.Windows;

namespace Reginald.ViewModels
{
    public class DefaultKeywordViewModel : KeywordViewModel
    {
        public DefaultKeywordViewModel() : base(ApplicationPaths.XmlKeywordFilename)
        {
            Settings.IncludeDefaultKeywords = Properties.Settings.Default.IncludeDefaultKeywords;
        }

        public void IncludeDefaultKeywordsToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            bool value = !Properties.Settings.Default.IncludeDefaultKeywords;
            Properties.Settings.Default.IncludeDefaultKeywords = value;
            Properties.Settings.Default.Save();
            Settings.IncludeDefaultKeywords = value;
        }
    }
}
