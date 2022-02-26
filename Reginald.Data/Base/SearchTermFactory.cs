namespace Reginald.Data.Base
{
    using System;
    using Microsoft.WindowsAPICodePack.Shell;
    using Reginald.Data.Keyphrases;
    using Reginald.Data.Keywords;
    using Reginald.Data.ShellItems;

    public class SearchTermFactory : KeyFactory
    {
        public override ShellItem CreateShellItem(ShellObject shellObject)
        {
            return new Application(shellObject);
        }

        public override Keyword CreateKeyword(KeywordDataModelBase model)
        {
            Type type = model.GetType();
            return type switch
            {
                Type when type == typeof(GenericKeywordDataModel) => new GenericKeyword(model as GenericKeywordDataModel),
                Type when type == typeof(CommandKeywordDataModel) => new CommandKeyword(model as CommandKeywordDataModel),
                Type when type == typeof(HttpKeywordDataModel) => new HttpKeyword(model as HttpKeywordDataModel),
                _ => null,
            };
        }

        public override Keyword CreateCommand(Keyword keyword, string input)
        {
            CommandKeyword commandKeyword = keyword as CommandKeyword;
            switch (commandKeyword.Command)
            {
                case Command.Timer:
                    TimerKeyword timer = new(commandKeyword);
                    if (input.Length > 0)
                    {
                        if (timer.TryParseTimeFromString(input))
                        {
                            return timer;
                        }
                    }
                    else
                    {
                        timer.Description = string.Format(timer.Format, timer.Placeholder, timer.Placeholder);
                        return timer;
                    }

                    break;

                case Command.Quit:
                    return new QuitKeyword(commandKeyword);
            }

            return null;
        }

        public override Keyphrase CreateKeyphrase(KeyphraseDataModelBase model)
        {
            Type type = model.GetType();
            return type switch
            {
                Type when type == typeof(UtilityKeyphraseDataModel) => new UtilityKeyphrase(model as UtilityKeyphraseDataModel),
                Type when type == typeof(MicrosoftSettingKeyphraseDataModel) => new MicrosoftSettingKeyphrase(model as MicrosoftSettingKeyphraseDataModel),
                _ => null,
            };
        }
    }
}
