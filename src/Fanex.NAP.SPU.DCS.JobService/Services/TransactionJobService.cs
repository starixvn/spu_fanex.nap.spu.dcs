using Fanex.NAP.SPU.DCS.Features.Transactions;
using Sentry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fanex.NAP.SPU.DCS.JobService.Services
{
    public interface ITransactionJobService
    {
        Task<bool> Execute(int numberOfTrans, int numberOfBatch);
    }

    public class TransactionJobService : ITransactionJobService
    {

        private readonly ISentryClient sentryClient;
        private readonly ITransactionServices transactionService;

        private readonly IList<string> IgnoreMessages;
        private const int delayTime = 200; // milliseconds

        public TransactionJobService(ISentryClient sentryClient, ITransactionServices transactionService)
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
                }
            }
            catch (Exception ex)
            {
                if (!this.IgnoreMessages.Contains(ex.Message))
                {
                    sentryClient.CaptureException(ex);

                    return true;
                }                

                return false;
            }

            return true;
        }

        private async Task JobRun(int numberOfTrans, int numberOfBatch)
        {
            var rawTrans = this.transactionService.GetRawTransaction(0, numberOfTrans, numberOfBatch);
            var tasks = new List<Task>();

            foreach (var trans in rawTrans)
            {
                try
                {
                    var task = Task.Run(() =>
                            {
                                this.transactionService
                                .UnhandleSubscriberAccountProcessing(trans.MinTransId, trans.MaxTransId)
                                .UserAgentProcessing(trans.MinTransId, trans.MaxTransId)
                                .TransactionProcessing(trans.MinTransId, trans.MaxTransId);

                            });

                    await Task.Delay(delayTime);

                    tasks.Add(task);
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
