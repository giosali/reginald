namespace Reginald.Data.DisplayItems
{
    using System;
    using System.Collections.Generic;
    using System.Timers;
    using Reginald.Data.Keywords;
    using Reginald.Services.Notifications;

    public partial class TimerResult : SearchResult
    {
        private const string TimerDescriptionFormat = "{0}:{1:D2}:{2:D2}";

        private const string AltCaption = "Cancel Timer?";

        public TimerResult(TimerKeyword keyword)
            : base(keyword)
        {
            Guid = Guid.NewGuid();
            Icon = keyword.Icon;
            Caption = keyword.Completion;
            Time = keyword.Time;
            TimeSpan span = TimeSpan.FromMilliseconds(Time);
            Description = string.Format(TimerDescriptionFormat, span.Hours, span.Minutes, span.Seconds);
            OriginalCaption = Caption;
            OriginalDescription = keyword.Completion;
            Timers.Add(this);
        }

        private static List<TimerResult> Timers { get; set; } = new();

        private double Time { get; set; }

        private Timer Timer { get; set; }

        private string OriginalCaption { get; set; }

        private string OriginalDescription { get; set; }

        public static List<TimerResult> GetTimers(string input)
        {
            return input.Equals("timers", StringComparison.OrdinalIgnoreCase) ? Timers : new List<TimerResult>();
        }

        public override void EnterKeyDown()
        {
            if (IsAltKeyDown)
            {
                Timer.Stop();
                Timers.Remove(this);
            }
        }

        public override void AltKeyDown()
        {
            IsAltKeyDown = true;
            Caption = AltCaption;
        }

        public override void AltKeyUp()
        {
            IsAltKeyDown = false;
            Caption = OriginalCaption;
        }

        public override bool Predicate()
        {
            return Timer.Enabled;
        }

        public void OnTimeChanged(object sender, ElapsedEventArgs e)
        {
            Time -= 1000;
            if (Time <= 0)
            {
                Timer.Stop();
                Timers.Remove(this);
                ToastNotification notification = new(Name, OriginalDescription);
                notification.Show();
            }

            TimeSpan span = TimeSpan.FromMilliseconds(Time);
            Description = string.Format(TimerDescriptionFormat, span.Hours, span.Minutes, span.Seconds);
        }

        public void StartTimer()
        {
            Timer = new Timer(1000);
            Timer.Elapsed += OnTimeChanged;
            Timer.AutoReset = true;
            Timer.Enabled = true;
        }
    }
}
