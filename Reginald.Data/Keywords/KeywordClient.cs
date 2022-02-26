namespace Reginald.Data.Keywords
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Reginald.Core.Utilities;
    using Reginald.Data.Base;
    using Reginald.Data.ShellItems;

    public class KeywordClient
    {
        private const string CommandQuitDescriptorFormat = "Quit {0}";

        public KeywordClient(KeyFactory factory, KeywordDataModelBase model)
        {
            Keyword = factory.CreateKeyword(model);
        }

        public KeywordClient(KeyFactory factory, IEnumerable<KeywordDataModelBase> models)
        {
            List<Keyword> keywords = new(models.Count());
            foreach (KeywordDataModelBase model in models)
            {
                keywords.Add(factory.CreateKeyword(model));
            }

            Keywords = keywords;
        }

        public KeywordClient(KeyFactory factory, Keyword keyword, string input, IEnumerable<ShellItem> items)
        {
            CommandKeyword commandKeyword = keyword as CommandKeyword;
            switch (commandKeyword.Command)
            {
                case Command.Timer:
                    Keyword = factory.CreateCommand(keyword, input);
                    break;

                case Command.Quit:
                    Process[] processes = ProcessUtility.GetTopLevelProcesses(input);
                    int processesLength = processes.Length;
                    List<Keyword> quits = new(processesLength);
                    for (int i = 0; i < processesLength; i++)
                    {
                        Process process = processes[i];
                        string productName = process.MainModule.FileVersionInfo.ProductName;
                        string fileDescription = process.MainModule.FileVersionInfo.FileDescription;
                        foreach (ShellItem item in items)
                        {
                            string itemName = item.Name;
                            if (productName.Equals(itemName, StringComparison.OrdinalIgnoreCase) || fileDescription.IndexOf(itemName) > 0)
                            {
                                keyword.Icon = item.Icon;
                                keyword.Description = string.Format(CommandQuitDescriptorFormat, itemName);
                                QuitKeyword quit = factory.CreateCommand(keyword, input) as QuitKeyword;
                                quit.Process = process;
                                quits.Add(quit);
                                break;
                            }
                        }
                    }

                    Keywords = quits;
                    break;
            }
        }

        public Keyword Keyword { get; set; }

        public IEnumerable<Keyword> Keywords { get; set; }
    }
}
