namespace Reginald.Core.Notifications
{
    using Microsoft.Toolkit.Uwp.Notifications;

    public class ToastNotifications
    {
        public static void SendSimpleToastNotification(string header, string description)
        {
            new ToastContentBuilder().AddText(header)
                                     .AddText(description)
                                     .Show();
        }
    }
}
