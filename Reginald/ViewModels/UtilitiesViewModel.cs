using Reginald.Core.IO;
using System.Windows;

namespace Reginald.ViewModels
{
    public class UtilitiesViewModel : KeywordViewModel
    {
        public UtilitiesViewModel() : base(ApplicationPaths.XmlUtilitiesFilename)
        {
            Settings.IncludeUtilities = Properties.Settings.Default.IncludeUtilities;
        }

        public void IncludeUtilitiesToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            bool value = !Properties.Settings.Default.IncludeUtilities;
            Properties.Settings.Default.IncludeUtilities = value;
            Properties.Settings.Default.Save();
            Settings.IncludeUtilities = value;
        }
    }
}
