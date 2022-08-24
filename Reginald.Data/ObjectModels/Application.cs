namespace Reginald.Data.ObjectModels
{
    using System;
    using System.Windows.Media.Imaging;
    using Reginald.Data.Inputs;
    using Reginald.Data.Producers;
    using Reginald.Data.Products;
    using Reginald.Services.Utilities;

    public class Application : ObjectModel, ISingleProducer<SearchResult>
    {
        public Application(string description, BitmapSource source, string filePath)
        {
            Caption = "Application";
            Description = description;
            Source = source;
            FilePath = filePath;
        }

        public BitmapSource Source { get; set; }

        public string FilePath { get; set; }

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
