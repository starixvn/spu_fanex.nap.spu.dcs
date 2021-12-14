namespace Fanex.NAP.SPU.DCS.JobService.Services
{
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.SPU.DCS.Features.Monitoring;
    using Fanex.NAP.SPU.DCS.Features.Monitoring.Model;
    using Fanex.NAP.SPU.Notification.Services;
    using Sentry;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public interface INotifyTransactionJobService
    {
        Task<bool> Execute();

        Task<bool> TestTelegramNotifyMessage();
    }

    public class NotifyTransactionJobService : INotifyTransactionJobService
    {
        private readonly ISentryClient sentryClient;
        private readonly IMonitoringServices monitoringServices;
        private readonly IAppSettings appSettings;

        public NotifyTransactionJobService(IMonitoringServices monitoringServices, ISentryClient sentryClient, IAppSettings appSettings)
        {
            this.sentryClient = sentryClient;
            this.monitoringServices = monitoringServices;
            this.appSettings = appSettings;
        }

        public Task<bool> Execute()
        {
            try
            {
                var transactions = this.monitoringServices.GetNotificationTransaction();

                NotifyMessage(transactions);
            }
            catch (Exception ex)
            {
                sentryClient.CaptureException(ex);

                var logMessage = new LogMessage(ex.Message)
                        .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                        .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error($"DCS - Notify Transaction job Execute exeption message:  {logMessage}");

                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }

        private void NotifyMessage(NotifyTransactionModel data)
        {
            var isNotify = !string.IsNullOrWhiteSpace(data.YesterdayLess50Percent)
                        || !string.IsNullOrWhiteSpace(data.ValidLess50Percent);                                               

            if (isNotify)
            {
                var message = $"Monitoring Transaction job run COMPLETED.";

                if (!string.IsNullOrWhiteSpace(data.YesterdayLess50Percent))
                    message += $"\n\r - Total Trans Today < 50% Yesterday: {data.YesterdayLess50Percent}";             

                if (!string.IsNullOrWhiteSpace(data.ValidLess50Percent))
                    message += $"\n\r - Valid Trans < 50% Total Trans: {data.ValidLess50Percent}";

                var telegramService = new NotificationService();
                telegramService
                   .Config("0A40aOc7CtpzjbBSJu8KNWQRoXyjRvf9", "-1001459619983", "https://bot.nexdev.net/skynex/api/messages/forwardwithtoken")
                   .Message(new MessageModel("DCS - Notify Transaction Service", DateTime.Now, appSettings.EnvironmentString, message))
                   .SendWithDelay(300, true);
            }
        }

        public Task<bool> TestTelegramNotifyMessage()
        {
            var telegramService = new NotificationService();
            telegramService
               .Config("0A40aOc7CtpzjbBSJu8KNWQRoXyjRvf9", "-1001459619983", "https://bot.nexdev.net/skynex/api/messages/forwardwithtoken")
               .Message(new MessageModel("DCS - Notify Transaction Service", DateTime.Now, appSettings.EnvironmentString, "Test"))
               .SendWithDelay(300, true);

            return Task.FromResult(true);
        }

    }
}
