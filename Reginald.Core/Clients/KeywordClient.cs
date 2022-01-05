using Reginald.Core.AbstractFactories;
using Reginald.Core.AbstractProducts;
using Reginald.Core.Base;
using Reginald.Core.DataModels;
using Reginald.Core.Enums;
using Reginald.Core.Products;
using Reginald.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Reginald.Core.Clients
{
    public class KeywordClient
    {
        public Keyword Keyword { get; set; }

        public IEnumerable<Keyword> Keywords { get; set; }

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
                    Process[] processes = Processes.GetTopLevelProcesses(input);
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
                                keyword.Description = string.Format(Constants.CommandQuitDescriptorFormat, itemName);
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
    }
}
