namespace Reginald.Data.Base
{
    using Microsoft.WindowsAPICodePack.Shell;
    using Reginald.Data.Keyphrases;
    using Reginald.Data.Keywords;
    using Reginald.Data.ShellItems;

    public abstract class KeyFactory
    {
        public abstract ShellItem CreateShellItem(ShellObject shellObject);

        public abstract Keyword CreateKeyword(KeywordDataModelBase model);

        public abstract Keyword CreateCommand(Keyword keyword, string input);

        public abstract Keyphrase CreateKeyphrase(KeyphraseDataModelBase model);
    }
}
