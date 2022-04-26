namespace Reginald.Data.Keywords
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Windows.Media;
    using Reginald.Services.Helpers;
    using Reginald.Services.Utilities;

    public class QuitKeyword : CommandKeyword
    {
        private const string QuitDescriptionFormat = "Quit {0}";

        private static readonly Dictionary<string, ImageSource> _icons = new();

        public QuitKeyword()
        {
        }

        public QuitKeyword(CommandKeywordDataModel model, Process process)
            : base(model)
        {
            Guid = Guid.NewGuid();
            string filename = process.MainModule.FileName;
            if (!_icons.ContainsKey(filename))
            {
                _icons.Add(filename, BitmapSourceHelper.ExtractAssociatedBitmapSource(process.MainModule.FileName));
            }

            Icon = _icons[filename];
            Description = string.Format(QuitDescriptionFormat, FileVersionInfo.GetVersionInfo(process.MainModule.FileName).FileDescription);
            ProcessId = process.Id;
        }

        public int ProcessId { get; set; }

        public override void EnterKeyDown()
        {
            ProcessUtility.QuitProcessById(ProcessId);
        }
    }
}
