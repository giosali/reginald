namespace Reginald.Data.Keywords
{
    using System;
    using System.Diagnostics;
    using Reginald.Services.Helpers;
    using Reginald.Services.Utilities;

    public class QuitKeyword : CommandKeyword
    {
        private const string QuitDescriptionFormat = "Quit {0}";

        public QuitKeyword()
        {
        }

        public QuitKeyword(CommandKeywordDataModel model, Process process)
            : base(model)
        {
            Guid = Guid.NewGuid();
            Icon = BitmapSourceHelper.ExtractAssociatedBitmapSource(process.MainModule.FileName);
            string fileDescription = FileVersionInfo.GetVersionInfo(process.MainModule.FileName).FileDescription;
            Description = string.Format(QuitDescriptionFormat, fileDescription);
            ProcessId = process.Id;
        }

        public int ProcessId { get; set; }

        public override void EnterKeyDown()
        {
            ProcessUtility.QuitProcessById(ProcessId);
        }
    }
}
