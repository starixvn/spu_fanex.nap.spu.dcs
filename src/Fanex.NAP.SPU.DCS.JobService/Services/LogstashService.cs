using Fanex.NAP.SPU.DCS.Features.DataTrace.Model;
using Newtonsoft.Json;
using System.Net.Sockets;
using System.Text;

namespace Fanex.NAP.SPU.DCS.JobService.Services
{
    public interface ILogstashService
    {
        ulong PushLog(IEnumerable<TransactionLogstashModel> transactions);
        ulong PushMBLog(IEnumerable<MBTransactionLogstashModel> transactions);
    }

    public class LogstashService : ILogstashService
    {
        private readonly IAppSettings _appSettings;

        public LogstashService(IAppSettings appSettings, ILogService logService)
        {
            _appSettings = appSettings;
        }

        public ulong PushLog(IEnumerable<TransactionLogstashModel> transactions)
            => PushLogInternal(transactions, "Transaction07");

        public ulong PushMBLog(IEnumerable<MBTransactionLogstashModel> transactions)
            => PushLogInternal(transactions, "MBTransaction07");

        private ulong PushLogInternal<T>(IEnumerable<T> transactions, string tag)
            where T : class
        {
            ulong processedTransID = 0;

            if (transactions == null)
            {
                return 0;
            }

            using var client = new TcpClient();
            client.Connect(_appSettings.LogstashTcpHostname, _appSettings.LogstashTcpPort);

            using var stream = client.GetStream();

            foreach (var transaction in transactions)
            {
                dynamic dyn = transaction;
                dyn.SubscriberName = dyn.SubscriberName?.ToLower() ?? string.Empty;
                dyn.LoginName = dyn.LoginName?.ToLower() ?? string.Empty;

                var logMessage = new
                {
                    Message = transaction,
                    Tags = tag
                };

                var json = JsonConvert.SerializeObject(logMessage);
                var bytes = Encoding.UTF8.GetBytes(json + "\n");

                stream.Write(bytes, 0, bytes.Length);

                processedTransID = Math.Max(processedTransID, (ulong)dyn.TransID);
            }

            return processedTransID;
        }
    }
}
