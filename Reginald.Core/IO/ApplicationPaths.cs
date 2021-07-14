using System;

namespace Reginald.Core.IO
{
    public static class ApplicationPaths
    {
        public static string AppDataDirectoryPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        public static string ApplicationName = "Reginald";
        public static string IconsDirectoryName = "ApplicationIcons";
        public static string UserIconsDirectoryName = "UserIcons";
        public static string XmlKeywordFilename = "Search.xml";
        public static string XmlUserKeywordFilename = "UserSearch.xml";
        public static string TxtFilename = "Applications.txt";
    }
}
