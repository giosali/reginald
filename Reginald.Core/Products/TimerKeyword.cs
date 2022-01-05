using Reginald.Core.AbstractProducts;
using Reginald.Core.Notifications;
using System;
using System.Threading.Tasks;

namespace Reginald.Core.Products
{
    public class TimerKeyword : CommandKeyword
    {
        private double _time;
        public double Time
        {
            get => _time;
            set
            {
                _time = value;
                NotifyOfPropertyChange(() => Time);
            }
        }

        private bool _isRunning;
        public bool IsRunning
        {
            get => _isRunning;
            set
            {
                _isRunning = value;
                NotifyOfPropertyChange(() => IsRunning);
            }
        }

        public TimerKeyword()
        {

        }

        public TimerKeyword(CommandKeyword keyword)
        {
            Guid = keyword.Guid;
            Name = keyword.Name;
            Word = keyword.Word;
            Icon = keyword.Icon;
            Format = keyword.Format;
            Placeholder = keyword.Placeholder;
            Caption = keyword.Caption;
            IsEnabled = keyword.IsEnabled;
            Command = keyword.Command;
        }

        public override async void EnterDown(Keyword keyword, bool isAltDown, Action action)
        {
            TimerKeyword timer = keyword as TimerKeyword;
            if (!string.IsNullOrEmpty(timer.Completion))
            {
                action();
                await Task.Delay((int)timer.Time);
                ToastNotifications.SendSimpleToastNotification(timer.Name, timer.Completion);
            }
        }
    }
}
