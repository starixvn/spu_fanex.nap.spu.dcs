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
        private const int delayTime = 100; // milliseconds

        public TransactionJobService(ISentryClient sentryClient, ITransactionServices transactionService)
        {
            this.sentryClient = sentryClient;
            this.transactionService = transactionService;
        }

        public Task<bool> Execute(int numberOfTrans, int numberOfBatch)
        {
            try
            {
                if (numberOfTrans > 0 && numberOfBatch > 0)
                {
                    JobRun(numberOfTrans, numberOfBatch);
                }
            }
            catch (Exception ex)
            {
                sentryClient.CaptureException(ex);

                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }

        private void JobRun(int numberOfTrans, int numberOfBatch)
        {
            var rawTrans = this.transactionService.GetRawTransaction(0, numberOfTrans, numberOfBatch);
            var tasks = new List<Task>();

            foreach (var trans in rawTrans)
            {
                var task = Task.Run(() =>
                {
                    this.transactionService                    
                    .UnhandleSubscriberAccountProcessing(trans.MinTransId, trans.MaxTransId)
                    .UserAgentProcessing(trans.MinTransId, trans.MaxTransId)
                    .TransactionProcessing(trans.MinTransId, trans.MaxTransId);

                });

                Task.Delay(delayTime);

                tasks.Add(task);
            }

            Task.WhenAll(tasks.ToArray());
        }
    }
}
