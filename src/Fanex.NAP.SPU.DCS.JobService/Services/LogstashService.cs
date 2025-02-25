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
        Task<ulong> PushLogAsync(IEnumerable<TransactionLogstashModel> transactions);
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
        public async Task<ulong> PushLogAsync(IEnumerable<TransactionLogstashModel> transactions)
        {
            ulong processedTransID = 0;

            if (!appSettings.IsEnableLogstash)
                return processedTransID;

            try
            {
                using var client = new TcpClient(appSettings.LogstashTcpHostname, appSettings.LogstashTcpPort);
                using var stream = client.GetStream();

                foreach (var transaction in transactions)
                {
                    var logMessage = TransformLogMessage(transaction);

                    string jsonLog = JsonConvert.SerializeObject(logMessage);

                    byte[] data = Encoding.UTF8.GetBytes(jsonLog + "\n");

                    await stream.WriteAsync(data);

                    processedTransID = Math.Max(processedTransID, transaction.TransID);
                }
            }
            catch (Exception ex)
            {
                await logService.ErrorMessageAsync($"DCS - DataTrace PushLogAsync execute exception message: {ex.Message}");
            }

            return processedTransID;
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
