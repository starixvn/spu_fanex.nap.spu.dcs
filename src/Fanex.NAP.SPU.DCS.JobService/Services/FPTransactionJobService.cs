namespace Fanex.NAP.SPU.DCS.JobService.Services
{
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
                if (!this.IgnoreMessages.Contains(ex.Message))
                {
                    sentryClient.CaptureException(ex);

                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        private async Task JobRun(int numberOfTrans, int numberOfBatch)
        {
            var rawTrans = this.transactionService.GetFPRawTransaction(0, numberOfTrans, numberOfBatch);
            var tasks = new List<Task>();

            foreach (var trans in rawTrans)
            {
                try
                {
                    var task = Task.Run(() =>
                    {
                        this.transactionService
                               .UnhandleFPSubscriberActionProcessing(trans.MinTransId, trans.MaxTransId)
                               .UserAgentProcessing(trans.MinTransId, trans.MaxTransId)
                               .TransactionProcessing(trans.MinTransId, trans.MaxTransId);
                    });

                    await Task.Delay(delayTime);
                }
                catch (Exception ex)
                {
                    sentryClient.CaptureException(ex);

                    throw;
                }
            }

            var taskCompleted = Task.WhenAll(tasks.ToArray());

            await taskCompleted;
        }

    }
}
