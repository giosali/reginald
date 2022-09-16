namespace Reginald.Models.ObjectModels
{
    using System.Diagnostics;
    using System.IO;
    using Reginald.Core.Extensions;
    using Reginald.Models.Inputs;
    using Reginald.Models.Producers;
    using Reginald.Models.Products;

    internal class FileSystemEntry : ObjectModel, ISingleProducer<SearchResult>
    {
        public FileSystemEntry(string path)
        {
            Description = Path.GetFileName(path);
            Caption = path;
            try
            {
                if ((File.GetAttributes(path) & FileAttributes.Directory) == FileAttributes.Directory)
                {
                    IconPath = "3";
                    Type = EntryType.Directory;
                }
                else
                {
                    IconPath = "1";
                    Type = EntryType.File;
                }
            }
            catch (IOException)
            {
                Caption = null;
            }
        }

        private enum EntryType
        {
            Directory,
            File,
        }

        private string IconPath { get; set; }

        private EntryType Type { get; set; }

        public bool Check(string input)
        {
            return Description.ContainsPhrase(input);
        }

        public SearchResult Produce()
        {
            SearchResult result = new(Caption, IconPath, Description);
            result.EnterKeyPressed += OnEnterKeyPressed;
            return result;
        }

        private void OnEnterKeyPressed(object sender, InputProcessingEventArgs e)
        {
            _ = Process.Start("explorer.exe", (Type == EntryType.File ? "/select," : string.Empty) + Caption);
            e.Handled = true;
        }
    }
}
