namespace Reginald.Models.ObjectModels
{
    using System.Diagnostics;
    using System.IO;
    using Reginald.Core.Extensions;
    using Reginald.Models.Inputs;
    using Reginald.Models.Producers;
    using Reginald.Models.Products;

    internal enum EntryType
    {
        Directory,
        File,
    }

    internal class FileSystemEntry : ObjectModel, ISingleProducer<SearchResult>
    {
        public FileSystemEntry(string path)
        {
            Description = Path.GetFileName(path);
            try
            {
                if ((File.GetAttributes(path) & FileAttributes.Directory) == FileAttributes.Directory)
                {
                    Caption = "Folder";
                    IconPath = "3";
                    EntryType = EntryType.Directory;
                }
                else
                {
                    Caption = "File";
                    IconPath = "1";
                    EntryType = EntryType.File;
                }
            }
            catch (IOException)
            {
                Caption = null;
            }

            AltCaption = path;
        }

        public string AltCaption { get; set; }

        public EntryType EntryType { get; set; }

        public string IconPath { get; set; }

        public bool Check(string input)
        {
            return Description.ContainsPhrase(input);
        }

        public SearchResult Produce()
        {
            SearchResult result = new(Caption, IconPath, Description);
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

            result.Caption = AltCaption;
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
            _ = Process.Start("explorer.exe", (EntryType == EntryType.File ? "/select," : string.Empty) + AltCaption);
            e.Handled = true;
        }
    }
}
