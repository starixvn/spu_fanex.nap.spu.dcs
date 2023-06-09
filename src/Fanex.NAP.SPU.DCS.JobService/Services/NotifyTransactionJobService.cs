namespace Fanex.NAP.SPU.DCS.JobService.Services
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.SPU.DCS.Features.Monitoring;
    using Fanex.NAP.SPU.DCS.Features.Monitoring.Model;
    using Fanex.NAP.SPU.Notification.Services;
    using Sentry;

    public interface INotifyTransactionJobService
    {
        Task<bool> Execute();

        Task<bool> TestTelegramNotifyMessage();

        Task<bool> MonitorTransactionLevelAccount();
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

        public Task<bool> MonitorTransactionLevelAccount()
        {
            try
            {
                var cfgLimitShowCust = 10;
                var date = DateTime.Today;
                var rptDate = date.AddDays(-1);

                sentryClient.CaptureMessage($"MonitorTransactionLevelAccount on date: {date:dd/MM/yyyy} for date: {rptDate:dd/MM/yyyy}");

                var ctsCust = monitoringServices.GetCTSCustByDate(rptDate);
                if (ctsCust != null && ctsCust.Any())
                {
                    var dcsCust = monitoringServices.GetDCSCustByDate(rptDate);
                    var ctsCustNotInDcs = ctsCust.Where(c => !dcsCust.Contains(c.CustID));

                    if (ctsCustNotInDcs != null && ctsCustNotInDcs.Any())
                    {
                        var countListCustToSend = ctsCustNotInDcs.Count();

                        var message = "Please kindly check the following Usersname without Transaction";
                        message += $"\n - Total Usersname: {countListCustToSend}";
                        message += $"\n - Top {cfgLimitShowCust} Usersname: {string.Join(", ", ctsCustNotInDcs.Take(cfgLimitShowCust).OrderByDescending(x => x.Username).Select(x => x.Username))}";

                        if (countListCustToSend > cfgLimitShowCust)
                        {
                            message += ", ...";
                        }

                        NotifyMessageMonitorTransaction("FPS-Cust-MissedTrans " + rptDate.ToString("MM/dd/yyyy"), message, string.Empty);
                    }
                }
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                        .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                        .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error($"DCS - MonitorTransactionLevelAccount job Execute exeption message:  {logMessage}");

                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }

        private void NotifyMessageMonitorTransaction(string jobName, string message, string moreInfo)
        {
            var telegramService = new NotificationService();
            telegramService
               .Config("0A40aOc7CtpzjbBSJu8KNWQRoXyjRvf9", "-506613866", "https://bot.nexdev.net/skynex/api/messages/forwardwithtoken")
               .Message(new MessageModel(jobName, DateTime.Now, appSettings.EnvironmentString, message, moreInfo))
               .SendWithDelay(300, true);
        }
    }
}