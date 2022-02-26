namespace Reginald.Services.Notifications
{
    using Microsoft.Toolkit.Uwp.Notifications;

    public class ToastNotification
    {
        public ToastNotification(string header, string description)
        {
            Builder = new ToastContentBuilder().AddText(header)
                                               .AddText(description);
        }

        private ToastContentBuilder Builder { get; set; }

        public void Show()
        {
            Builder.Show();
        }
    }
}
