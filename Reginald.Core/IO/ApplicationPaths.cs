namespace Reginald.Core.IO
{
    using System;

    public static class ApplicationPaths
    {
        public static string AppDataDirectoryPath { get; } = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        public static string ApplicationName { get; } = "Reginald";

        public static string ApplicationShortcutName { get; } = "Reginald.lnk";

        public static string UserIconsDirectoryName { get; } = "UserIcons";

        public static string SettingsFilename { get; } = "Settings.json";

        public static string KeywordsJsonFilename { get; } = "Keywords.json";

        public static string UserKeywordsJsonFilename { get; } = "UserKeywords.json";

        public static string CalculatorJsonFilename { get; } = "Calculator.json";

        public static string LinkJsonFilename { get; } = "Link.json";

        public static string DefaultResultsJsonFilename { get; } = "DefaultResults.json";

        public static string CommandsJsonFilename { get; } = "Commands.json";

        public static string HttpKeywordsJsonFilename { get; } = "HttpKeywords.json";

        public static string UtilitiesJsonFilename { get; } = "Utilities.json";

        public static string MicrosoftSettingsJsonFilename { get; } = "MicrosoftSettings.json";

        public static string ExpansionsJsonFilename { get; } = "Expansions.json";

        public static string ThemesJsonFilename { get; } = "Themes.json";

        public static string TopLevelDomainsTxtFilename { get; } = "TopLevelDomains.txt";
    }
}
