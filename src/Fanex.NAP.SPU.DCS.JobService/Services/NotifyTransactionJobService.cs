namespace Fanex.NAP.SPU.DCS.JobService.Services
{
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.SPU.DCS.Features.Monitoring;
    using Fanex.NAP.SPU.DCS.Features.Monitoring.Model;
    using Fanex.NAP.SPU.DCS.JobService.Models;
    using Fanex.NAP.SPU.Notification.Services;

    public interface INotifyTransactionJobService
    {
        Task<bool> Execute();

        Task<bool> TestTelegramNotifyMessage();

        Task<bool> MonitorTransactionAndNotify(string rptDateStr, int batchSize, int limitShowCust);

        Task<bool> NotifyMessageByID(uint notifyMessageID);

        Task<bool> NotifyMessageByNotifyType(string notifyType, int batchSize);

        Task<bool> TestSentryCaptureMessage(string message);
    }

    public class NotifyTransactionJobService : INotifyTransactionJobService
    {
        private readonly ILogService logService;
        private readonly IMonitoringServices monitoringServices;
        private readonly IAppSettings appSettings;
        private readonly INotificationService notificationService;

        private readonly string proxy;
        private readonly string token = "0A40aOc7CtpzjbBSJu8KNWQRoXyjRvf9";
        private readonly int extraHour = 7;

        public NotifyTransactionJobService(IMonitoringServices monitoringServices, ILogService logService, IAppSettings appSettings, INotificationService notificationService)
        {
            this.logService = logService;
            this.monitoringServices = monitoringServices;
            this.appSettings = appSettings;
            this.notificationService = notificationService;

            proxy = appSettings.Environment == "PROD" ? appSettings.TelegramProxy : null;
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
                logService.ErrorAsync(ex);

                var logMessage = LogMessage(ex);

                logService.InfoMessageAsync($"DCS - Notify Transaction job Execute exeption message:  {logMessage}");

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
                {
                    message += $"\n\r - Total Trans Today < 50% Yesterday: {data.YesterdayLess50Percent}";
                }

                if (!string.IsNullOrWhiteSpace(data.ValidLess50Percent))
                {
                    message += $"\n\r - Valid Trans < 50% Total Trans: {data.ValidLess50Percent}";
                }

                var messageModel = new MessageModel("DCS - Notify Transaction Service", DateTime.UtcNow.AddHours(extraHour), appSettings.Environment, message);
                notificationService.Send(token, "-1001459619983", proxy, messageModel);
            }
        }

        public Task<bool> TestTelegramNotifyMessage()
        {
            notificationService.Send(token, "-1001459619983", proxy, new MessageModel("DCS - Notify Transaction Service", DateTime.UtcNow.AddHours(extraHour), appSettings.Environment, "test"));

            return Task.FromResult(true);
        }

        private static DateTime GetRptDate(string input)
        {
            const string format = "MM/dd/yyyy";
            DateTime parsedDate;

            if (DateTime.TryParseExact(input, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate))
            {
                return parsedDate;
            }

            return DateTime.Now.AddDays(-1);
        }

        public Task<bool> MonitorTransactionAndNotify(string rptDateStr, int batchSize, int limitShowCust)
        {
            try
            {
                var rptDate = GetRptDate(rptDateStr);
                var stopLoopFlag = false;
                var minCustID = 0;

                while (!stopLoopFlag)
                {
                    var mainCust = monitoringServices.GetMainCustByDate(rptDate, minCustID, batchSize);

                    if (mainCust != null && mainCust.Any())
                    {
                        monitoringServices.InsertMonitorTransaction(rptDate, JsonSerializer.Serialize(mainCust));
                        minCustID = mainCust.Max(c => c.CustID);
                    }
                    else
                    {
                        stopLoopFlag = true;
                    }
                }

                var notifyMessageID = CreateNotifyMessage(rptDate, NotifyType.MonitorTransaction.ToString(), appSettings.TelegramGroupIDMonitorTransaction, limitShowCust);
                NotifyMessageByID(notifyMessageID);
            }
            catch (Exception ex)
            {
                var logMessage = LogMessage(ex);

                logService.ErrorMessageAsync($"DCS - MonitorTransactionLevelAccount job Execute exeption message:  {logMessage}");

                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }

        private uint CreateNotifyMessage(DateTime rptDate, string notifyType, string groupId, int limitShowCust)
        {
            return monitoringServices.InsertNotifyMonitorTransaction(rptDate, notifyType, groupId, limitShowCust);
        }

        public Task<bool> NotifyMessageByID(uint notifyMessageID)
        {
            try
            {
                if (notifyMessageID > 0)
                {
                    var notifyMessage = monitoringServices.GetNotifyMessageById(notifyMessageID);
                    if (notifyMessage != null)
                    {
                        NotifyMessageToTelegram(notifyMessage);
                        monitoringServices.UpdateNotifyMessagesToNotified(notifyMessage.ID.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                var logMessage = LogMessage(ex);

                logService.ErrorMessageAsync($"DCS - NotifyMessageByID job Execute exeption message:  {logMessage}");

                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }

        public Task<bool> NotifyMessageByNotifyType(string notifyType, int batchSize)
        {
            try
            {
                var notifyMessages = monitoringServices.GetNotifyMessage(notifyType, batchSize);
                if (notifyMessages != null && notifyMessages.Any())
                {
                    foreach (var notifyMessage in notifyMessages)
                    {
                        NotifyMessageToTelegram(notifyMessage);
                    }
                    string ids = string.Join(",", notifyMessages.Select(x => x.ID));
                    monitoringServices.UpdateNotifyMessagesToNotified(ids);
                }
            }
            catch (Exception ex)
            {
                var logMessage = LogMessage(ex);

                logService.ErrorMessageAsync($"DCS - NotifyMessageByNotifyType job Execute exeption message:  {logMessage}");

                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }

        private void NotifyMessageToTelegram(NotifyMessageModel model)
        {
            if (model != null)
            {
                notificationService.Send(token, model.GroupID, proxy, new MessageModel(model.Title, DateTime.UtcNow.AddHours(extraHour), appSettings.Environment, model.Message, model.MoreInfo));
            }
        }

        private static string LogMessage(Exception ex)
        {
            return new LogMessage(ex.Message)
                        .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                        .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();
        }

        public Task<bool> TestSentryCaptureMessage(string message)
        {
            logService.InfoMessageAsync(message);
            return Task.FromResult(true);
        }
    }
}