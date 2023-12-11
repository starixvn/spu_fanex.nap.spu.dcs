
namespace Fanex.NAP.SPU.DCS.JobService.Services
{
    using Fanex.NAP.SPU.Notification.Services;

    public interface INotificationService
    {
        void Send(string token, string groupId, string proxy, MessageModel message);
    }

    public class NotificationService : INotificationService
    {
        private readonly int delaySecond = 300;

        public virtual void Send(string token, string groupId, string proxy, MessageModel message)
        {
            var notificationService = new Notification.Services.NotificationService();
            notificationService
               .Config(token, groupId, "https://bot.nexdev.net/skynex/api/messages/forwardwithtoken", proxy)
               .Message(message)
               .SendWithDelay(delaySecond, true);
        }
    }
}
