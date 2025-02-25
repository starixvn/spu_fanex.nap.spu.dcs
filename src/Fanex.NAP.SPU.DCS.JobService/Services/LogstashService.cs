using System.Net.Sockets;
using System.Text;
using Fanex.Logging;
using Fanex.NAP.SPU.DCS.Features.DataTrace.Model;
using Fanex.NAP.SPU.DCS.JobService.Models;
using Newtonsoft.Json;

namespace Fanex.NAP.SPU.DCS.JobService.Services
{
    public interface ILogstashService
    {
        Task<IEnumerable<ulong>> PushLogAsync(IEnumerable<TransactionLogstashModel> transactions);
    }
    public class LogstashService: ILogstashService
    {
        private readonly IAppSettings appSettings;
        private readonly ILogService logService;

        public LogstashService(IAppSettings appSettings, ILogService logService)
        {
            this.appSettings = appSettings;
            this.logService = logService;
        }
        public async Task<IEnumerable<ulong>> PushLogAsync(IEnumerable<TransactionLogstashModel> transactions)
        {
            var processedTransIDs = new List<ulong>();

            if (!appSettings.IsEnableLogstash)
                return processedTransIDs;

            try
            {
                using var client = new TcpClient(appSettings.LogstashTcpHostname, appSettings.LogstashTcpPort);
                using var stream = client.GetStream();

                foreach (var transaction in transactions)
                {
                    if (appSettings.LogstashIgnoreSubscribers.Count != 0 && appSettings.LogstashIgnoreSubscribers.Contains(transaction.SubscriberName.ToLower()))
                        continue;

                    var logMessage = TransformLogMessage(transaction);

                    string jsonLog = JsonConvert.SerializeObject(logMessage);

                    byte[] data = Encoding.UTF8.GetBytes(jsonLog + "\n");

                    await stream.WriteAsync(data);

                    processedTransIDs.Add(transaction.TransID);
                }
            }
            catch (Exception ex)
            {
                await logService.ErrorMessageAsync($"DCS - DataTrace PushLogAsync execute exception message: {ex.Message}");

                return processedTransIDs;
            }

            return processedTransIDs;
        }

        private LogstashModel TransformLogMessage(TransactionLogstashModel transaction)
        {
            transaction.SubscriberName = transaction.SubscriberName.ToLowerInvariant();

            var logMessage = new LogstashModel
            {
                Message = transaction,
                Tags = "Transaction07",
            };

            return logMessage;
        }
    }
}
