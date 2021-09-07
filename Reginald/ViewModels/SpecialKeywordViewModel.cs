using Reginald.Core.IO;
using System.Windows;

namespace Reginald.ViewModels
{
    public class SpecialKeywordViewModel : KeywordViewModel
    {
        public SpecialKeywordViewModel() : base(ApplicationPaths.XmlSpecialKeywordFilename)
        {
            Settings.IncludeSpecialKeywords = Properties.Settings.Default.IncludeSpecialKeywords;
        }       

        public void IncludeSpecialKeywordsToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            bool value = !Properties.Settings.Default.IncludeSpecialKeywords;
            Properties.Settings.Default.IncludeSpecialKeywords = value;
            Properties.Settings.Default.Save();
            Settings.IncludeSpecialKeywords = value;
        }
    }
}
