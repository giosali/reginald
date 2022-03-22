namespace Reginald.Services
{
    using System.IO;
    using System.Linq;
    using Reginald.Core.Helpers;
    using Reginald.Core.IO;
    using Reginald.Data.Keyphrases;
    using Reginald.Data.Keywords;
    using Reginald.Data.Representations;

    public class DataFileService
    {
        private const string DefaultResultsFilename = "DefaultResults.json";

        private readonly FileSystemWatcher[] _watchers;

        public DataFileService()
        {
            DefaultKeywords = KeywordFactory.CreateKeywords(Union<GenericKeywordDataModel>(GenericKeyword.KeywordsFilename));
            UserKeywords = KeywordFactory.CreateKeywords(FileOperations.GetGenericData<GenericKeywordDataModel>(GenericKeyword.UserKeywordsFilename, false));
            Commands = Union<CommandKeywordDataModel>(CommandKeyword.Filename);
            HttpKeywords = KeywordFactory.CreateKeywords(FileOperations.GetGenericData<HttpKeywordDataModel>(HttpKeyword.Filename, true));
            DefaultResults = KeywordFactory.CreateKeywords(FileOperations.GetGenericData<GenericKeywordDataModel>(DefaultResultsFilename, true));

            Utilities = KeyphraseFactory.CreateKeyphrases(FileOperations.GetGenericData<UtilityKeyphraseDataModel>(UtilityKeyphrase.Filename, true));
            MicrosoftSettings = KeyphraseFactory.CreateKeyphrases(FileOperations.GetGenericData<MicrosoftSettingKeyphraseDataModel>(MicrosoftSettingKeyphrase.Filename, true));

            Calculator = RepresentationFactory.CreateRepresentation(FileOperations.GetGenericDatum<CalculatorDataModel>(Calculator.Filename, true)) as Calculator;
            Link = RepresentationFactory.CreateRepresentation(FileOperations.GetGenericDatum<LinkDataModel>(Link.Filename, true)) as Link;

            string directoryPath = FileOperations.ApplicationAppDataDirectoryPath;
            FileSystemWatcher defaultKeywordsWatcher = FileSystemWatcherHelper.Initialize(directoryPath, GenericKeyword.KeywordsFilename, OnDefaultKeywordsChanged);
            FileSystemWatcher userKeywordsWatcher = FileSystemWatcherHelper.Initialize(directoryPath, GenericKeyword.UserKeywordsFilename, OnUserKeywordsChanged);
            FileSystemWatcher commandsWatcher = FileSystemWatcherHelper.Initialize(directoryPath, CommandKeyword.Filename, OnCommandsChanged);
            _watchers = new FileSystemWatcher[]
            {
                defaultKeywordsWatcher,
                userKeywordsWatcher,
                commandsWatcher,
            };
        }

        public Keyword[] DefaultKeywords { get; set; }

        public Keyword[] UserKeywords { get; set; }

        public CommandKeywordDataModel[] Commands { get; set; }

        public Keyword[] HttpKeywords { get; set; }

        public Keyword[] DefaultResults { get; set; }

        public Keyphrase[] Utilities { get; set; }

        public Keyphrase[] MicrosoftSettings { get; set; }

        public Calculator Calculator { get; set; }

        public Link Link { get; set; }

        private static T[] Union<T>(string filename)
        {
            return FileOperations.GetGenericData<T>(filename, true)
                                 .Union(FileOperations.GetGenericData<T>(filename, false))
                                 .ToArray();
        }

        private void OnDefaultKeywordsChanged(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType == WatcherChangeTypes.Changed)
            {
                DefaultKeywords = KeywordFactory.CreateKeywords(Union<GenericKeywordDataModel>(GenericKeyword.KeywordsFilename));
            }
        }

        private void OnUserKeywordsChanged(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType == WatcherChangeTypes.Changed)
            {
                UserKeywords = KeywordFactory.CreateKeywords(FileOperations.GetGenericData<GenericKeywordDataModel>(GenericKeyword.UserKeywordsFilename, false));
            }
        }

        private void OnCommandsChanged(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType == WatcherChangeTypes.Changed)
            {
                Commands = Union<CommandKeywordDataModel>(CommandKeyword.Filename);
            }
        }
    }
}
