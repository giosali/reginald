using System;
using System.Diagnostics;

namespace Reginald.Core.Products
{
    public class QuitKeyword : CommandKeyword
    {
        private Process _process;
        public Process Process
        {
            get => _process;
            set
            {
                _process = value;
                NotifyOfPropertyChange(() => Process);
            }
        }

        public QuitKeyword()
        {

        }

        public QuitKeyword(CommandKeyword keyword)
        {
            Guid = Guid.NewGuid();
            Name = keyword.Name;
            Word = keyword.Word;
            Icon = keyword.Icon;
            Format = keyword.Format;
            Placeholder = keyword.Placeholder;
            Caption = keyword.Caption;
            IsEnabled = keyword.IsEnabled;
            Command = keyword.Command;
            Description = keyword.Description;
        }

        public override void EnterDown(bool isAltDown, Action action)
        {
            action();
            Process.CloseMainWindow();
            Process.Close();
        }
    }
}
