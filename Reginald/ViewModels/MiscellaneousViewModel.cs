using Caliburn.Micro;
using Reginald.Core.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Reginald.ViewModels
{
    public class MiscellaneousViewModel : KeywordViewModel
    {
        public MiscellaneousViewModel() : base(ApplicationPaths.XmlSettingsPagesFileLocation, true)
        {
            Settings.IncludeSettingsPages = Properties.Settings.Default.IncludeSettingsPages;
        }

        public void IncludeSettingsPagesToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            bool value = !Properties.Settings.Default.IncludeSettingsPages;
            Properties.Settings.Default.IncludeSettingsPages = value;
            Properties.Settings.Default.Save();
            Settings.IncludeSettingsPages = value;
        }
    }
}
