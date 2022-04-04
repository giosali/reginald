namespace Reginald.Services
{
    using System.Diagnostics;
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
            DefaultKeywords = KeywordFactory.CreateKeywords(Complement<GenericKeywordDataModel>(GenericKeyword.KeywordsFilename));
            UserKeywords = KeywordFactory.CreateKeywords(FileOperations.GetGenericData<GenericKeywordDataModel>(GenericKeyword.UserKeywordsFilename, false).Where(m => m.IsEnabled));
            Commands = Complement<CommandKeywordDataModel>(CommandKeyword.Filename);
            HttpKeywords = KeywordFactory.CreateKeywords(Complement<HttpKeywordDataModel>(HttpKeyword.Filename));
            DefaultResults = KeywordFactory.CreateKeywords(FileOperations.GetGenericData<GenericKeywordDataModel>(DefaultResultsFilename, true));

            Utilities = KeyphraseFactory.CreateKeyphrases(Complement<UtilityKeyphraseDataModel>(UtilityKeyphrase.Filename));
            MicrosoftSettings = KeyphraseFactory.CreateKeyphrases(FileOperations.GetGenericData<MicrosoftSettingKeyphraseDataModel>(MicrosoftSettingKeyphrase.Filename, true));

            Calculator = RepresentationFactory.CreateRepresentation(FileOperations.GetGenericDatum<CalculatorDataModel>(Calculator.Filename, true)) as Calculator;
            Link = RepresentationFactory.CreateRepresentation(FileOperations.GetGenericDatum<LinkDataModel>(Link.Filename, true)) as Link;

            string directoryPath = FileOperations.ApplicationAppDataDirectoryPath;
            FileSystemWatcher defaultKeywordsWatcher = FileSystemWatcherHelper.Initialize(directoryPath, GenericKeyword.KeywordsFilename, OnDefaultKeywordsChanged);
            FileSystemWatcher userKeywordsWatcher = FileSystemWatcherHelper.Initialize(directoryPath, GenericKeyword.UserKeywordsFilename, OnUserKeywordsChanged);
            FileSystemWatcher commandsWatcher = FileSystemWatcherHelper.Initialize(directoryPath, CommandKeyword.Filename, OnCommandKeywordsChanged);
            FileSystemWatcher httpKeywordsWatcher = FileSystemWatcherHelper.Initialize(directoryPath, HttpKeyword.Filename, OnHttpKeywordsChanged);

            FileSystemWatcher utilityKeyphrasesWatcher = FileSystemWatcherHelper.Initialize(directoryPath, UtilityKeyphrase.Filename, OnUtilityKeyphrasesChanged);

            _watchers = new FileSystemWatcher[]
            {
                defaultKeywordsWatcher,
                userKeywordsWatcher,
                commandsWatcher,
                httpKeywordsWatcher,
                utilityKeyphrasesWatcher,
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

        private static T[] Complement<T>(string filename)
        {
            return FileOperations.GetGenericData<T>(filename, true)
                                 .Except(FileOperations.GetGenericData<T>(filename, false))
                                 .ToArray();
        }

        private void OnDefaultKeywordsChanged(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType == WatcherChangeTypes.Changed || e.ChangeType == WatcherChangeTypes.Created || e.ChangeType == WatcherChangeTypes.Deleted)
            {
                DefaultKeywords = KeywordFactory.CreateKeywords(Complement<GenericKeywordDataModel>(GenericKeyword.KeywordsFilename));
            }
        }

        private void OnUserKeywordsChanged(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType == WatcherChangeTypes.Changed || e.ChangeType == WatcherChangeTypes.Created || e.ChangeType == WatcherChangeTypes.Deleted)
            {
                UserKeywords = KeywordFactory.CreateKeywords(FileOperations.GetGenericData<GenericKeywordDataModel>(GenericKeyword.UserKeywordsFilename, false).Where(m => m.IsEnabled));
            }
        }

        private void OnCommandKeywordsChanged(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType == WatcherChangeTypes.Changed || e.ChangeType == WatcherChangeTypes.Created || e.ChangeType == WatcherChangeTypes.Deleted)
            {
                Commands = Complement<CommandKeywordDataModel>(CommandKeyword.Filename);
            }
        }

        private void OnHttpKeywordsChanged(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType == WatcherChangeTypes.Changed || e.ChangeType == WatcherChangeTypes.Created || e.ChangeType == WatcherChangeTypes.Deleted)
            {
                HttpKeywords = KeywordFactory.CreateKeywords(Complement<HttpKeywordDataModel>(HttpKeyword.Filename));
            }
        }

        private void OnUtilityKeyphrasesChanged(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType == WatcherChangeTypes.Changed || e.ChangeType == WatcherChangeTypes.Created || e.ChangeType == WatcherChangeTypes.Deleted)
            {
                Utilities = KeyphraseFactory.CreateKeyphrases(Complement<UtilityKeyphraseDataModel>(UtilityKeyphrase.Filename));
            }
        }
    }
}
