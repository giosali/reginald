namespace Reginald.Data.Keywords
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Reginald.Services.Utilities;

    public static class KeywordFactory
    {
        public static Keyword[] CreateKeywords(IEnumerable<IKeywordDataModel> models)
        {
            Type baseType = models.GetType();
            Type type = baseType.GetElementType() ?? baseType.GenericTypeArguments.ElementAtOrDefault(0);
            return type switch
            {
                Type when type == typeof(GenericKeywordDataModel) => models.Select(m => new GenericKeyword(m as GenericKeywordDataModel)).ToArray(),
                Type when type == typeof(HttpKeywordDataModel) => models.Select(m => new HttpKeyword(m as HttpKeywordDataModel)).ToArray(),
                _ => null,
            };
        }

        public static IEnumerable<CommandKeyword> CreateCommandKeywords(CommandKeywordDataModel model, string input)
        {
            CommandType commandType = (CommandType)Enum.Parse(typeof(CommandType), model.CommandType);
            switch (commandType)
            {
                case CommandType.Timer:
                    TimerKeyword timerKeyword = new(model, input);
                    if (timerKeyword.Description is not null)
                    {
                        return new CommandKeyword[] { timerKeyword };
                    }

                    break;

                case CommandType.Quit:
                    Stopwatch stopwatch = Stopwatch.StartNew();
                    IEnumerable<Process> processes = ProcessUtility.GetTopLevelProcesses(input);
                    stopwatch.Stop();
                    Debug.WriteLine(stopwatch.ElapsedMilliseconds);
                    return processes.Select(process => new QuitKeyword(model, process));
            }

            return Enumerable.Empty<CommandKeyword>();
        }
    }
}
