using System.Net.Sockets;
using System.Text;
using Fanex.NAP.SPU.DCS.Features.DataTrace.Model;
using Fanex.NAP.SPU.DCS.JobService.Models;
using Newtonsoft.Json;

namespace Fanex.NAP.SPU.DCS.JobService.Services
{
    public interface ILogstashService
    {
        Task PushLogAsync(IEnumerable<TransactionLogstashModel> transactions);
    }
    public class LogstashService: ILogstashService
    {
        private readonly IAppSettings appSettings;

        public LogstashService(IAppSettings appSettings)
        {
            this.appSettings = appSettings;
        }
        public async Task PushLogAsync(IEnumerable<TransactionLogstashModel> transactions)
        {
            if (!appSettings.IsEnableLogstash)
            {
                return;
            }

            using var client = new TcpClient(appSettings.LogstashTcpHostname, appSettings.LogstashTcpPort);
            using var stream = client.GetStream();
            foreach (var transaction in transactions)
            {
                if (appSettings.LogstashIgnoreSubscribers.Count != 0 && appSettings.LogstashIgnoreSubscribers.Contains(transaction.SubscriberName.ToLowerInvariant()))
                {
                    continue;
                }

                var logMessage = TransformLogMessage(transaction);

                string jsonLog = JsonConvert.SerializeObject(logMessage);

                byte[] data = Encoding.UTF8.GetBytes(jsonLog + "\n");
                await stream.WriteAsync(data);
            }
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
