namespace Fanex.NAP.SPU.DCS.JobService.Services
{
    using Fanex.NAP.SPU.DCS.Features.Transactions;
    using Sentry;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ITransactionJobService
    {
        Task<bool> Execute(int numberOfTrans, int numberOfBatch);
    }

    public class TransactionJobService : ITransactionJobService
    {
        private readonly ISentryClient sentryClient;
        private readonly ITransactionServices transactionService;
        private readonly IAppSettings appSettings;

        private readonly IList<string> IgnoreMessages;
        private const int delayTime = 200; // milliseconds

        public TransactionJobService(ISentryClient sentryClient, ITransactionServices transactionService, IAppSettings appSettings)
        {
            this.sentryClient = sentryClient;
            this.transactionService = transactionService;
            this.appSettings = appSettings;
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
                   await JobRun(numberOfTrans, numberOfBatch);                    
                }

                return await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                if (!this.IgnoreMessages.Contains(ex.Message))
                {
                    sentryClient.CaptureException(ex);

                    return await Task.FromResult(false);
                }
                else
                {
                    return await Task.FromResult(true);
                }                             
            }           
        }

        private async Task JobRun(int numberOfTrans, int numberOfBatch)
        {
            var tasks = new List<Task>();

            if (appSettings.IsReadyDbExtra)
            {
                tasks.Add(Task.Run(() =>
                {
                    _= TransformTransaction(numberOfTrans, numberOfBatch, true);
                }));
            }
            tasks.Add(Task.Run(() =>
            {
                _= TransformTransaction(numberOfTrans, numberOfBatch, false);
            }));

            await Task.WhenAll(tasks.ToArray());

            sentryClient.CaptureMessage($"Now: {DateTime.Now} isReadyDbExtra: {appSettings.IsReadyDbExtra}", Sentry.Protocol.SentryLevel.Info);

        }

        private async Task TransformTransaction(int numberOfTrans, int numberOfBatch, bool isReadyDbExtra)
        {
            var rawTrans = this.transactionService.GetRawTransaction(0, numberOfTrans, numberOfBatch, isReadyDbExtra);
            var tasks = new List<Task>();

            foreach (var trans in rawTrans)
            {
                try
                {
                    var task = Task.Run(() =>
                    {
                        try
                        {
                            sentryClient.CaptureMessage($"MinTransID: {trans.MinTransId} MaxTransID: {trans.MaxTransId} isReadyDbExtra: {isReadyDbExtra}", Sentry.Protocol.SentryLevel.Info);

                            this.transactionService
                                .UnhandleSubscriberAccountProcessing(trans.MinTransId, trans.MaxTransId, isReadyDbExtra)
                                .UserAgentProcessing(trans.MinTransId, trans.MaxTransId, isReadyDbExtra)
                                .TransactionProcessing(trans.MinTransId, trans.MaxTransId, isReadyDbExtra);
                        }
                        catch (Exception e)
                        {
                            sentryClient.CaptureException(e);

                            throw;
                        }
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

            await Task.WhenAll(tasks.ToArray());
        }
    }
}
