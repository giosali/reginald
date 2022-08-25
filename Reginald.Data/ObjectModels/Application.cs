namespace Reginald.Data.ObjectModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Media.Imaging;
    using Microsoft.WindowsAPICodePack.Shell;
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

            return Description.StartsWith(input, StringComparison.OrdinalIgnoreCase);
        }

        public SearchResult Produce()
        {
            SearchResult result = new(Caption, Source, Description);
            result.EnterKeyPressed += OnEnterKeyPressed;
            return result;
        }

        private void OnEnterKeyPressed(object sender, InputProcessingEventArgs e)
        {
            ProcessUtility.OpenFromPath(FilePath);
            e.Handled = true;
        }
    }
}
