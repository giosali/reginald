using System;

namespace Reginald.Core.IO
{
    public static class ApplicationPaths
    {
        public static string AppDataDirectoryPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        public const string ApplicationName = "Reginald";
        public const string IconsDirectoryName = "ApplicationIcons";
        public const string UserIconsDirectoryName = "UserIcons";
        public const string XmlKeywordFilename = "Search.xml";
        public const string XmlUserKeywordFilename = "UserSearch.xml";
        public const string XmlSpecialKeywordFilename = "SpecialKeywords.xml";
        public const string XmlCommandsFilename = "Commands.xml";
        public const string XmlSettingsFilename = "Settings.xml";
        public const string XmlUtilitiesFilename = "Utilities.xml";
        public const string XmlSettingsPagesFileLocation = "Resources/MSSettings.xml";
        public const string TxtFilename = "Applications.txt";
    }
}
