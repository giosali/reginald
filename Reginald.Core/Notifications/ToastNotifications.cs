using Microsoft.Toolkit.Uwp.Notifications;

namespace Reginald.Core.Notifications
{
    public class ToastNotifications
    {
        public static void SendSimpleToastNotification(string header, string description)
        {
            new ToastContentBuilder()
                .AddText(header)
                .AddText(description)
                .Show();
        }
    }
}
