namespace Fanex.NAP.SPU.DCS.JobService.Services
{
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.SPU.DCS.Features.FPTransactions;
    using Sentry;
    using System;
    using System.Collections.Generic;    
    using System.Threading.Tasks;

    public interface IFPTransactionJobService
    {
        Task<bool> Execute(int numberOfTrans, int numberOfBatch);
    }

    public class FPTransactionJobService : IFPTransactionJobService
    {
        private readonly ISentryClient sentryClient;
        private readonly IFPTransactionServices transactionService;
        private readonly IList<string> IgnoreMessages;
        private const int delayTime = 200;

        public FPTransactionJobService(ISentryClient sentryClient, IFPTransactionServices transactionService)
        {
            this.sentryClient = sentryClient;
            this.transactionService = transactionService;
            IgnoreMessages = new List<string> {
                "Deadlock found when trying to get lock; try restarting transaction"
            };
        }

        public async Task<bool> Execute(int numberOfTrans, int numberOfBatch)
        {
            try
            {
                if (numberOfTrans > 0 && numberOfBatch > 0)
                {
                    var task = JobRun(numberOfTrans, numberOfBatch);

                    await task;

                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                sentryClient.CaptureException(ex);

                var logMessage = new LogMessage(ex.Message)
                  .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                  .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error($"DCS - FP Transaction Job Service Execute exception message:  {logMessage}");

                return false;
            }
        }

        private async Task JobRun(int numberOfTrans, int numberOfBatch)
        {
            var rawTrans = this.transactionService.GetFPRawTransaction(0, numberOfTrans, numberOfBatch);

            var tasks = new List<Task>();

            foreach (var trans in rawTrans)
            {
                var task = Task.Run(() =>
                {
                    try
                    {
                        this.transactionService
                           .UnhandleFPSubscriberActionProcessing(trans.MinTransId, trans.MaxTransId)
                           .UserAgentProcessing(trans.MinTransId, trans.MaxTransId)
                           .TransactionProcessing(trans.MinTransId, trans.MaxTransId);
                    }
                    catch (Exception ex)
                    {
                        var logMessage = new LogMessage(ex.Message)
                             .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                             .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                        LogService.Error($"DCS - FP Transaction Job Service JobRun exception message:  {logMessage}");
                    }
                });

                tasks.Add(task);

                await Task.Delay(delayTime);
            }

            var taskCompleted = Task.WhenAll(tasks.ToArray());

            await taskCompleted;
        }

    }
}
