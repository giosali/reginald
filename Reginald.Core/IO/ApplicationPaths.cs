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
        public const string TxtFilename = "Applications.txt";
    }
}
