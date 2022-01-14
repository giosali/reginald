namespace Reginald.Core.AbstractFactories
{
    using Microsoft.WindowsAPICodePack.Shell;
    using Reginald.Core.AbstractProducts;
    using Reginald.Core.DataModels;

    public abstract class KeyFactory
    {
        public abstract ShellItem CreateShellItem(ShellObject shellObject);

        public abstract Keyword CreateKeyword(KeywordDataModelBase model);

        public abstract Keyword CreateCommand(Keyword keyword, string input);

        public abstract Keyphrase CreateKeyphrase(KeyphraseDataModelBase model);
    }
}
