namespace Reginald.Data.ObjectModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Media.Imaging;
    using Microsoft.WindowsAPICodePack.Shell;
    using Reginald.Core.Extensions;
    using Reginald.Data.Inputs;
    using Reginald.Data.Producers;
    using Reginald.Data.Products;
    using Reginald.Services.Utilities;

    public class Application : ObjectModel, ISingleProducer<SearchResult>
    {
        private static readonly Guid ApplicationsFolderGuid = new("{1e87508d-89c2-42f0-8a7e-645a0f50ca58}");

        public Application(string description, BitmapSource source, string filePath)
        {
            Caption = "Application";
            Description = description;
            Source = source;
            FilePath = filePath;
        }

        public BitmapSource Source { get; set; }

        public string FilePath { get; set; }

        public static IEnumerable<Application> GetApplications()
        {
            List<ShellObject> sos = new();
            IKnownFolder applicationsFolder = KnownFolderHelper.FromKnownFolderId(ApplicationsFolderGuid);
            foreach (ShellObject so in applicationsFolder)
            {
                string parsingName = so.ParsingName;
                if (parsingName.EndsWith(".exe"))
                {
                    sos.Add(so);
                    continue;
                }

                if (so.Name.EndsWith(".url", StringComparison.OrdinalIgnoreCase) || parsingName.EndsWith(".url", StringComparison.OrdinalIgnoreCase) || parsingName.StartsWith("https://", StringComparison.OrdinalIgnoreCase) || parsingName.EndsWith(".chm", StringComparison.OrdinalIgnoreCase) || parsingName.EndsWith(".zip", StringComparison.OrdinalIgnoreCase) || parsingName.EndsWith(".bat", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                sos.Add(so);
            }

            return sos.Select(so => new Application(so.Name, so.Thumbnail.MediumBitmapSource, so.Properties.System.Link.TargetParsingPath.Value is string path ? path : @"shell:AppsFolder\" + so.ParsingName));
        }

        public bool Check(string input)
        {
            if (input.Length > Description.Length)
            {
                return false;
            }

            // Handles instances where the application name is
            // "Microsoft Software Developer Kit"
            // and the input is
            // "software" or "dev" or "kit".
            char firstCh = input[0];
            int index = 0;
            while (true)
            {
                index = Description.IndexOf(firstCh.ToString(), index, StringComparison.OrdinalIgnoreCase);
                if (index == -1)
                {
                    break;
                }

                if (index == 0 || Description[index - 1] == ' ')
                {
                    bool isMatch = true;
                    for (int i = 0, j = index; i < input.Length; i++, j++)
                    {
                        if (!Description.TryElementAt(j, out char descriptionCh))
                        {
                            return false;
                        }

                        if (char.ToUpper(input[i]) != char.ToUpper(descriptionCh))
                        {
                            isMatch = false;
                            break;
                        }
                    }
                    
                    if (isMatch)
                    {
                        return true;
                    }
                }
                
                index++;
            }

            // Handles instances where the application name is
            // "Visual Studio Code"
            // and the input is
            // "vsc" or "sc".
            List<char> chars = new();
            for (int i = 0; i < Description.Length; i++)
            {
                char ch = Description[i];
                if (char.IsUpper(ch))
                {
                    chars.Add(ch);
                }
            }
            
            bool isLetterMatch = false;
            for (int i = 0, j = 0; i < input.Length; i++, j++)
            {
                if (j >= chars.Count)
                {
                    return false;
                }

                bool match = char.ToUpper(input[i]) == chars[j];
                if (!match && !isLetterMatch)
                {
                    i--;
                    continue;
                }
                
                if (!match && isLetterMatch)
                {
                    isLetterMatch = false;
                    break;
                }
                
                isLetterMatch = match;
            }

            return isLetterMatch;
        }

        public SearchResult Produce()
        {
            SearchResult result = new(Caption, Source, Description);
            result.AltKeyPressed += OnAltKeyPressed;
            result.AltKeyReleased += OnAltKeyReleased;
            result.EnterKeyPressed += OnEnterKeyPressed;
            return result;
        }

        private void OnAltKeyPressed(object sender, InputProcessingEventArgs e)
        {
            if (sender is not SearchResult result)
            {
                return;
            }

            result.Caption = FilePath;
        }

        private void OnAltKeyReleased(object sender, InputProcessingEventArgs e)
        {
            if (sender is not SearchResult result)
            {
                return;
            }

            result.Caption = Caption;
        }

        private void OnEnterKeyPressed(object sender, InputProcessingEventArgs e)
        {
            ProcessUtility.OpenFromPath(FilePath);
            e.Handled = true;
        }
    }
}
