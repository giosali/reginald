namespace Reginald.Models.ObjectModels
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using Reginald.Core.Extensions;
    using Reginald.Core.Utilities;
    using Reginald.Models.Inputs;
    using Reginald.Models.Producers;
    using Reginald.Models.Products;

    public enum EntryType
    {
        Directory,
        File,
    }

    internal sealed class FileSystemEntry : ObjectModel, ISingleProducer<SearchResult>
    {
        public FileSystemEntry(string path)
        {
            UpdatePath(path);
        }

        public static string UserProfile { get; private set; } = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

        public EntryType Type { get; private set; }

        private string IconPath { get; set; }

        public bool Check(string input)
        {
            return Path.GetFileName(Caption)?.ContainsPhrase(input) ?? false;
        }

        public SearchResult Produce()
        {
            SearchResult result = new(UserProfile + Caption, IconPath, Path.GetFileName(Caption), StaticRandom.Next());
            result.EnterKeyPressed += OnEnterKeyPressed;
            return result;
        }

        public void UpdatePath(string path)
        {
            Caption = path;
            if (Directory.Exists(UserProfile + path))
            {
                IconPath = "3";
                Type = EntryType.Directory;
            }
            else
            {
                IconPath = GuessType(path);
                Type = EntryType.File;
            }
        }

        private static string GuessType(string path)
        {
            switch (Path.GetExtension(path).ToLower())
            {
                // Audio files.
                case ".3gpp2":
                case ".aac":
                case ".adts":
                case ".aif":
                case ".aifc":
                case ".aiff":
                case ".ass":
                case ".au":
                case ".flac":
                case ".loas":
                case ".mid":
                case ".midi":
                case ".mp2":
                case ".mp3":
                case ".ogg":
                case ".opus":
                case ".ra":
                case ".snd":
                case ".wav":
                    return "71";

                // Image files.
                case ".avif":
                case ".bmp":
                case ".gif":
                case ".heic":
                case ".heif":
                case ".ico":
                case ".ief":
                case ".jpe":
                case ".jpeg":
                case ".jpg":
                case ".pbm":
                case ".pgm":
                case ".png":
                case ".pnm":
                case ".ppm":
                case ".ras":
                case ".rgb":
                case ".svg":
                case ".tif":
                case ".tiff":
                case ".xbm":
                case ".xpm":
                case ".xwd":
                    return "72";

                // Video files.
                case ".3g2":
                case ".3gp":
                case ".3gpp":
                case ".avi":
                case ".m1v":
                case ".mov":
                case ".movie":
                case ".mp4":
                case ".mpa":
                case ".mpe":
                case ".mpeg":
                case ".mpg":
                case ".qt":
                case ".webm":
                    return "73";

                // Archive files.
                case ".7z":
                case ".rar":
                case ".tar":
                case ".zip":
                    return "105";
                default:
                    return "1";
            }
        }

        private void OnEnterKeyPressed(object sender, InputProcessingEventArgs e)
        {
            _ = Process.Start("explorer.exe", (Type == EntryType.File ? "/select," : string.Empty) + UserProfile + Caption);
        }
    }
}
