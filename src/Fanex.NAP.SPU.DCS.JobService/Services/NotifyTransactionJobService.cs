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
    using Newtonsoft.Json;
    using Sentry;

    public interface INotifyTransactionJobService
    {
        Task<bool> Execute();

        Task<bool> TestTelegramNotifyMessage();

        Task<bool> MonitorTransactionLevelAccount(int batchSize);
        Task<bool> NotifyMessage(string groupId, string notifyType, int batchSize);
        
        Task<bool> TestSentryCaptureMessage(string message);
    }

    public class NotifyTransactionJobService : INotifyTransactionJobService
    {
        private readonly ISentryClient sentryClient;
        private readonly IMonitoringServices monitoringServices;
        private readonly IAppSettings appSettings;

        private readonly string proxy;

        public NotifyTransactionJobService(IMonitoringServices monitoringServices, ISentryClient sentryClient, IAppSettings appSettings)
        {
            this.sentryClient = sentryClient;
            this.monitoringServices = monitoringServices;
            this.appSettings = appSettings;

            proxy = appSettings.EnvironmentString == "PROD" ? appSettings.TelegramProxy : null;
        }

        public Task<bool> Execute()
        {
            try
            {
                var transactions = this.monitoringServices.GetNotificationTransaction();

                NotifyTransaction(transactions);
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

        private void NotifyTransaction(NotifyTransactionModel data)
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
                   .Config("0A40aOc7CtpzjbBSJu8KNWQRoXyjRvf9", "-1001459619983", "https://bot.nexdev.net/skynex/api/messages/forwardwithtoken", proxy)
                   .Message(new MessageModel("DCS - Notify Transaction Service", DateTime.UtcNow.AddHours(7), appSettings.EnvironmentString, message))
                   .SendWithDelay(300, true);
            }
        }

        public Task<bool> TestTelegramNotifyMessage()
        {
            var telegramService = new NotificationService();
            telegramService
               .Config("0A40aOc7CtpzjbBSJu8KNWQRoXyjRvf9", "-1001459619983", "https://bot.nexdev.net/skynex/api/messages/forwardwithtoken", proxy)
               .Message(new MessageModel("DCS - Notify Transaction Service", DateTime.UtcNow.AddHours(7), appSettings.EnvironmentString, "Test"))
               .SendWithDelay(300, true);

            return Task.FromResult(true);
        }

        public Task<bool> MonitorTransactionLevelAccount(int batchSize)
        {
            try
            {
                var curDate = new DateTime(2023, 4, 2);
                var rptDate = curDate.AddDays(-1);
                var cfgLimitShowCust = 10;

                var stopLoopFlag = false;
                var minCustID = 0;
                var notifyType = "MonitorTransaction";
                while (!stopLoopFlag)
                {
                    var ctsCust = monitoringServices.GetCTSCustByDate(rptDate, minCustID, batchSize);

                    if (ctsCust != null && ctsCust.Any())
                    {
                        monitoringServices.InsertMonitorTransaction(rptDate, JsonConvert.SerializeObject(ctsCust));
                        minCustID = ctsCust.Max(c => c.CustID);
                    }
                    else
                    {
                        stopLoopFlag = true;
                        CreateNotifyMessage(rptDate, notifyType, cfgLimitShowCust);
                        NotifyMonitorTransactionLevelAccount(notifyType, cfgLimitShowCust);
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

        private void CreateNotifyMessage(DateTime rptDate, string notifyType, int cfgLimitShowCust)
        {
            monitoringServices.InsertNotifyMonitorTransaction(rptDate, notifyType, cfgLimitShowCust);
        }

        public void NotifyMonitorTransactionLevelAccount(string notifyType, int batchSize)
        {
            var notifyMessages = monitoringServices.GetNotifyMessage(notifyType, batchSize);
            if (notifyMessages != null && notifyMessages.Any())
            {
                var groupId = appSettings.EnvironmentString == "PROD" ? "-1001586405982" : "-1001971031115";
                foreach (var notifyMessage in notifyMessages)
                {
                    NotifyMessageToTelegram(groupId, notifyMessage);
                }
                string ids = string.Join(",", notifyMessages.Select(x => x.ID));
                monitoringServices.UpdateNotifyMessage(ids);
            }
        }

        public Task<bool> NotifyMessage(string groupId, string notifyType, int batchSize)
        {
            try
            {
                var notifyMessages = monitoringServices.GetNotifyMessage(notifyType, batchSize);
                if (notifyMessages != null && notifyMessages.Any())
                {
                    foreach (var notifyMessage in notifyMessages)
                    {
                        NotifyMessageToTelegram(groupId, notifyMessage);
                    }
                    string ids = string.Join(",", notifyMessages.Select(x => x.ID));
                    monitoringServices.UpdateNotifyMessage(ids);
                }
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                        .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                        .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error($"DCS - NotifyMessage job Execute exeption message:  {logMessage}");

                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }

        private void NotifyMessageToTelegram(string groupId, NotifyMessageModel model)
        {
            if (model != null)
            {
                var telegramService = new NotificationService();

                telegramService
                   .Config("0A40aOc7CtpzjbBSJu8KNWQRoXyjRvf9", groupId, "https://bot.nexdev.net/skynex/api/messages/forwardwithtoken", proxy)
                   .Message(new MessageModel(model.Title, DateTime.UtcNow.AddHours(7), appSettings.EnvironmentString, model.Message, model.MoreInfo))
                   .SendWithoutDelay(true);
            }
        }
        }
        
        public Task<bool> TestSentryCaptureMessage(string message)
        {
            sentryClient.CaptureMessage(message);
            return Task.FromResult(true);
    }
}